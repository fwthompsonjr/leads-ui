using legallead.jdbc.entities;
using legallead.jdbc.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace legallead.jdbc.interfaces
{
    public interface IUserMangementRepository
    {
        Task<List<LeadUserAccountBo>?> GetAccountsAsync(UserManagementRequest request);
        Task<List<LeadUserCountyViewBo>?> GetCountyAsync(UserManagementRequest request);
        Task<List<LeadUserInvoiceBo>?> GetInvoiceAsync(UserManagementRequest request);
        Task<List<LeadCountyPricingBo>?> GetPricingAsync(UserManagementRequest request);
        Task<List<LeadUserProfileBo>?> GetProfileAsync(UserManagementRequest request);
        Task<List<LeadUserSearchBo>?> GetSearchAsync(UserManagementRequest request);
    }
}
