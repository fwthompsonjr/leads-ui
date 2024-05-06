namespace legallead.permissions.api.Models
{
    public class PaymentCreateModel(HttpRequest request, User user, string searchId, string productType)
    {
        private readonly string _successUrl = $"{request.Scheme}://{request.Host}/payment-result?sts=success&id=~0";
        private readonly string _guid = searchId;
        private readonly string _productType = productType;
        private readonly User _user = user;

        public string SuccessUrlFormat => _successUrl;
        public string SearchId => _guid;
        public string ProductType => _productType;
        public User CurrentUser => _user;
    }
}
