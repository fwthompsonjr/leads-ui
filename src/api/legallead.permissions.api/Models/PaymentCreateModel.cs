namespace legallead.permissions.api.Models
{
    public class PaymentCreateModel
    {
        private readonly string _successUrl;
        private readonly string _guid;
        private readonly string _productType;
        private readonly User _user;
        public PaymentCreateModel(HttpRequest request, User user, string searchId, string productType)
        {
            _successUrl = $"{request.Scheme}://{request.Host}/payment-result?sts=success&id=~0";
            _user = user;
            _guid = searchId;
            _productType = productType;
        }
        public string SuccessUrlFormat => _successUrl;
        public string SearchId => _guid;
        public string ProductType => _productType;
        public User CurrentUser => _user;
    }
}
