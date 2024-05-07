using legallead.email.interfaces;

namespace legallead.email.models
{
    public class ProfileChangedModel : ProfileChangedResponse
    {
        public ProfileChangedModel() { }
        public ProfileChangedModel(ProfileChangedResponse response)
        {
            Email = response.Email;
            Name = response.Name;
            Message = response.Message;
            JsonData = response.JsonData;
        }
        public List<IProfileChangeItem> ChangeItems { get; set; } = [];
        public bool IsValid
        {
            get
            {
                if (string.IsNullOrEmpty(Email)) return false;
                if (ChangeItems.Count == 0) return false;
                return true;
            }
        }
    }
}
