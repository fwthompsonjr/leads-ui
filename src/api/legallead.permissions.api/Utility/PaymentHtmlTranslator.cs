using legallead.jdbc.interfaces;
using legallead.permissions.api.Interfaces;

namespace legallead.permissions.api.Utility
{
    public class PaymentHtmlTranslator : IPaymentHtmlTranslator
    {
        private readonly IUserSearchRepository _repo;
        public PaymentHtmlTranslator(IUserSearchRepository db)
        { 
            _repo = db;
        }
        public bool IsRequestValid(string? status, string? id)
        {
            if (string.IsNullOrWhiteSpace(status)) return false;
            if (string.IsNullOrWhiteSpace(id)) return false;
            if (!requestNames.Contains(status)) return false;
            
            return true;
        }


        private static string[] requestNames = new[] { "success", "failed" };
    }
}
