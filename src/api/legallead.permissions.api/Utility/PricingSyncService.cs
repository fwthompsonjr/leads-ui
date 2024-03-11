using legallead.jdbc.entities;
using legallead.jdbc.interfaces;
using legallead.jdbc.models;
using Stripe;

namespace legallead.permissions.api
{
    public class PricingSyncService : BackgroundService
    {
        private readonly IPricingRepository _pricingInfrastructure;
        private readonly ILogger<PricingSyncService> logger;
        private readonly bool _testMode;
        public PricingSyncService(
            ILogger<PricingSyncService> log, 
            IPricingRepository infrastructure,
            bool isTestMode = true)
        {
            _pricingInfrastructure = infrastructure;
            logger = log;
            _testMode = isTestMode;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Pricing Sync Service is running.");
            await BackgroundProcessing(stoppingToken);
        }

        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            if (stoppingToken.IsCancellationRequested) return;

            await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
            var items = await _pricingInfrastructure.GetPricingTemplates();
            await SynchronizePricing(items, stoppingToken);
        }

        private async Task SynchronizePricing(List<PricingCodeBo> items, CancellationToken stoppingToken)
        {
            var names = items.Select(x => x.KeyName).Distinct().ToList();
            if (!names.Any()) { return; }
            await Task.Run(() =>
            {
                names.ForEach(async x =>
                {
                    if (stoppingToken.IsCancellationRequested) return;
                    var dataset = items.Where(w => w.KeyName == x).ToList();
                    if (!dataset.Exists(a => a.IsActive.GetValueOrDefault()))
                    {
                        var item = dataset.Find(x => !x.IsActive.GetValueOrDefault());
                        await CreatePricing(item);
                    }
                });
            }, stoppingToken);
        }

        private async Task<bool> CreatePricing(PricingCodeBo? item)
        {
            if (item == null || string.IsNullOrEmpty(item.Id) || string.IsNullOrEmpty(item.KeyName)) return false;
            var model = item.GetModel();
            if (model == null) return false;
            if (_testMode)
            {
                model.Product.Code = item.Id;
                model.PriceCode.Annual = item.Id;
                model.PriceCode.Monthly = item.Id;
            } 
            else
            {
                var service = new ProductService();
                var pricing = new PriceService();
                var iscreated = TryCreateProduct(item.KeyName, service, pricing, model);
                if (!iscreated) return false;
            }
            try
            {
                var businessObj = await _pricingInfrastructure.SetActivePricingTemplate(item.Id, model);
                return businessObj != null;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to create pricing : {0}", item.KeyName);
                return false;
            }
        }

        private bool TryCreateProduct(
            string productName,
            ProductService service, 
            PriceService pricing, 
            ProductPricingModel model)
        {
            try
            {
                var product = service.Create(GetProductOptions(model));
                if (product == null) return false;
                model.Product.Code = product.Id;
                for (var i = 0; i < 2; i++)
                {
                    var priceName = i == 0 ? $"{productName} Monthy" : $"{productName} Annual";
                    PriceCreateOptions price = GetPricingOption(model, product, i, priceName);
                    var created = pricing.Create(price);
                    if (created == null) return false;
                    if (i == 0) model.PriceCode.Monthly = created.Id;
                    if (i == 1) model.PriceCode.Annual = created.Id;
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return false;
            }
        }

        private static PriceCreateOptions GetPricingOption(ProductPricingModel model, Product product, int i, string priceName)
        {
            return new PriceCreateOptions
            {
                Currency = "usd",
                Active = true,
                UnitAmount = (i == 0 ? model.PriceAmount.Monthly : model.PriceAmount.Annual) * 100,
                Recurring = new()
                {
                    Interval = (i == 0 ? "month" : "year")
                },
                Nickname = priceName,
                Metadata = new()
                    {
                        { "index", i.ToString() },
                        { "name", priceName }
                    },
                Product = product.Id
            };
        }

        private static ProductCreateOptions GetProductOptions(ProductPricingModel item)
        {
            if (string.IsNullOrEmpty(item.Product.Name)) return new();
            if (string.IsNullOrEmpty(item.Product.Description)) return new();
            if (item.PriceAmount.Annual == 0) return new();
            if (item.PriceAmount.Monthly == 0) return new();
            return new ProductCreateOptions
            {
                Description = item.Product.Description,
                Name = item.Product.Name
            };
        }
    }
}