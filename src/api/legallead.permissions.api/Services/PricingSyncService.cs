using legallead.jdbc.entities;
using legallead.jdbc.implementations;
using legallead.jdbc.interfaces;
using legallead.jdbc.models;
using Newtonsoft.Json;
using Stripe;
using System.Diagnostics.CodeAnalysis;

namespace legallead.permissions.api.Services
{
    [ExcludeFromCodeCoverage(Justification = "This process directly interacts with data services and is for integration testing only.")]
    public class PricingSyncService : BackgroundService
    {
        private readonly IPricingRepository _pricingInfrastructure;
        private readonly ILogger<PricingSyncService> logger;
        private readonly bool _testMode;
        public PricingSyncService(
            ILogger<PricingSyncService> log,
            IPricingRepository infrastructure,
            bool isTestMode = false)
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

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
                var items = await _pricingInfrastructure.GetPricingTemplates();
                var svc = new ProductService();
                var existing = svc.List(new ProductListOptions
                {
                    Active = true,
                });
                await SynchronizePricing(existing, items, stoppingToken);
                items = (await _pricingInfrastructure.GetPricingTemplates()).FindAll(x => x.IsActive.GetValueOrDefault());
                PricingLookupService.Append(items);
                if (_testMode) await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                else await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
            }
        }


        private async Task SynchronizePricing(StripeList<Product> existing, List<PricingCodeBo> items, CancellationToken stoppingToken)
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
                        var pname = GetProductName(item);
                        var current = existing.FirstOrDefault(c => c.Name.Equals(pname));
                        if (current != null && item != null)
                        {
                            // create price db record
                            await CreateProductEntry(current, item);
                        }
                        else
                        {
                            await CreatePricing(item);
                        }

                    }
                });
            }, stoppingToken);
        }


        private async Task CreateProductEntry(Product current, PricingCodeBo item)
        {
            var dtojs = JsonConvert.SerializeObject(item);
            var dto = JsonConvert.DeserializeObject<PricingCodeDto>(dtojs);
            if (dto == null) return;
            if (_pricingInfrastructure is not BaseRepository<PricingCodeDto> coderepo) return;
            var pricing = new PriceService();
            var prices = pricing.List(new PriceListOptions { Active = true, Product = current.Id });
            dto.Id = Guid.NewGuid().ToString("D");
            dto.ProductCode = current.Id;
            dto.PriceCodeMonthly = prices.First(x => x.Nickname.Contains("Month"))?.Id ?? item.PriceCodeMonthly;
            dto.PriceCodeAnnual = prices.First(x => x.Nickname.Contains("Annual"))?.Id ?? item.PriceCodeAnnual;
            dto.KeyJs = UpdateJson(dto);
            dto.CreateDate = DateTime.UtcNow;
            dto.IsActive = true;
            await coderepo.Create(dto);
            var related = (await coderepo.GetAll()).ToList().FindAll(x =>
                x.KeyName == item.KeyName &&
                x.Id != dto.Id &&
                x.IsActive.GetValueOrDefault());
            related.ForEach(async r =>
            {
                r.IsActive = false;
                await coderepo.Update(r);
            });

        }


        private async Task<bool> CreatePricing(PricingCodeBo? item)
        {
            if (item == null || string.IsNullOrEmpty(item.Id) || string.IsNullOrEmpty(item.KeyName)) return false;
            var model = item.GetModel();
            if (model == null) return false;
            var service = new ProductService();
            var pricing = new PriceService();
            var iscreated = TryCreateProduct(item.KeyName, service, pricing, model);
            if (!iscreated) return false;
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
                    Interval = i == 0 ? "month" : "year"
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


        private static string GetProductName(PricingCodeBo? codeBo)
        {
            var model = codeBo?.GetModel();
            if (model == null) return string.Empty;
            return model.Product.Name;

        }


        private static string UpdateJson(PricingCodeDto dto)
        {
            var js = dto.KeyJs;
            if (string.IsNullOrEmpty(js)) { return string.Empty; }
            var model = JsonConvert.DeserializeObject<ProductPricingModel>(js);
            if (model == null ||
                dto.ProductCode == null ||
                dto.PriceCodeAnnual == null ||
                dto.PriceCodeMonthly == null) return js;
            model.Product.Code = dto.ProductCode;
            model.PriceCode.Annual = dto.PriceCodeAnnual;
            model.PriceCode.Monthly = dto.PriceCodeMonthly;
            return JsonConvert.SerializeObject(model);
        }
    }
}