using legallead.jdbc.entities;

namespace legallead.jdbc.interfaces
{
    public interface ILeadUserRepository
    {
        Task<LeadUserBo?> GetUser(string userName);
    }
}
