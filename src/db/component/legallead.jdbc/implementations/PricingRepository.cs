using Dapper;
using legallead.jdbc.entities;
using legallead.jdbc.helpers;
using legallead.jdbc.interfaces;
using legallead.jdbc.models;
using Newtonsoft.Json;

namespace legallead.jdbc.implementations
{
    public class PricingRepository : BaseRepository<PricingCodeDto>, IPricingRepository
    {
        public PricingRepository(DataContext context) : base(context)
        {
        }
        public async Task<PricingCodeBo?> CreatePricingTemplate(string templateId, ProductPricingModel model)
        {
            var json = JsonConvert.SerializeObject(model);
            const string prc = "CALL USP_CREATE_PRICING_TEMPLATE( ?, ? );";
            try
            {
                using var connection = _context.CreateConnection();
                var parms = new DynamicParameters();
                parms.Add("permission_code_index", templateId);
                parms.Add("pricing_json", json);
                var response = await _command.QuerySingleOrDefaultAsync<PricingCodeDto>(connection, prc);
                if (response == null) return null;
                var js = JsonConvert.SerializeObject(response);
                var bo = JsonConvert.DeserializeObject<PricingCodeBo>(js);
                return bo;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<PricingCodeBo>> GetPricingTemplateHistory()
        {
            const string prc = "CALL USP_GET_PRICING_TEMPLATES_HISTORY();";
            try
            {
                using var connection = _context.CreateConnection();
                var response = await _command.QueryAsync<PricingCodeDto>(connection, prc);
                if (response == null) return new();
                var json = JsonConvert.SerializeObject(response);
                var bo = JsonConvert.DeserializeObject<List<PricingCodeBo>>(json) ?? new();
                return bo;
            }
            catch (Exception)
            {
                return new();
            }
        }

        public async Task<List<PricingCodeBo>> GetPricingTemplates()
        {
            const string prc = "CALL USP_GET_PRICING_TEMPLATES();";
            try
            {
                using var connection = _context.CreateConnection();
                var response = await _command.QueryAsync<PricingCodeDto>(connection, prc);
                if (response == null) return new();
                var json = JsonConvert.SerializeObject(response);
                var bo = JsonConvert.DeserializeObject<List<PricingCodeBo>>(json) ?? new();
                return bo;
            }
            catch (Exception)
            {
                return new();
            }
        }

        public async Task<PricingCodeBo?> SetActivePricingTemplate(string templateId, ProductPricingModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Product.Code))
                throw new ArgumentOutOfRangeException(nameof(model), "Product code is required to create a price template");

            if (string.IsNullOrWhiteSpace(model.PriceCode.Annual))
                throw new ArgumentOutOfRangeException(nameof(model), "Annual pricing code is required to create a price template");

            if (string.IsNullOrWhiteSpace(model.PriceCode.Monthly))
                throw new ArgumentOutOfRangeException(nameof(model), "Monthly pricing code is required to create a price template");

            var json = JsonConvert.SerializeObject(model);
            const string prc = "CALL USP_SET_ACTIVE_PRICING_TEMPLATE( ?, ? );";
            try
            {
                using var connection = _context.CreateConnection();
                var parms = new DynamicParameters();
                parms.Add("permission_code_index", templateId);
                parms.Add("pricing_json", json);
                var response = await _command.QuerySingleOrDefaultAsync<PricingCodeDto>(connection, prc);
                if (response == null) return null;
                var js = JsonConvert.SerializeObject(response);
                var bo = JsonConvert.DeserializeObject<PricingCodeBo>(js);
                return bo;
            }
            catch (Exception)
            {
                return null;
            }
        }


        public async Task<PricingCodeBo?> SetActivePricingTemplate(string templateId, string modeljs)
        {
            var model = JsonConvert.DeserializeObject<ProductPricingModel>(modeljs);
            if (model == null) return null;
            var response = await SetActivePricingTemplate(templateId, model);
            return response;
        }
    }
}
