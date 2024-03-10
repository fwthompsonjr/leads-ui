using legallead.jdbc.entities;
using legallead.jdbc.models;

namespace legallead.jdbc.interfaces
{
    public interface IPricingRepository
    {
        /// <summary>
        /// provides list of most recent pricing code templates
        /// includes both active and inactive templates
        /// inactive templates are used to contruct active templates
        /// inactive templates also store any history of pricing changes
        /// active template will contain the correct product and pricing codes to bill a customer 
        /// </summary>
        Task<List<PricingCodeBo>> GetPricingTemplates();

        /// <summary>
        /// provides list of all pricing code templates
        /// </summary>
        Task<List<PricingCodeBo>> GetPricingTemplateHistory();


        /// <summary>
        /// creates an inactive pricing model record using input parameters
        /// </summary>
        Task<PricingCodeBo?> CreatePricingTemplate(string templateId, ProductPricingModel model);


        /// <summary>
        /// sets the active pricing model record using input parameters
        /// </summary>
        Task<PricingCodeBo?> SetActivePricingTemplate(string templateId, ProductPricingModel model);
    }
}
