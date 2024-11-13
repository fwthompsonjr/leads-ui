using legallead.permissions.api.Entities;

namespace legallead.permissions.api.Interfaces
{
    public interface ILeadSecurityService
    {
        LeadUserSecurityModel CreateSecurityModel(string cleartext);
        string GetCountyData(LeadUserModel model);
        string GetCountyPermissionData(LeadUserModel model);
        string GetPassPhrase();
        LeadUserModel GetModel(LeadUserBo user);
        string GetUserData(LeadUserModel model);
    }
}
