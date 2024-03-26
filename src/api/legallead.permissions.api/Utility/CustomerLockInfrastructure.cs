using legallead.permissions.api.Interfaces;

namespace legallead.permissions.api.Utility
{
    public class CustomerLockInfrastructure : ICustomerLockInfrastructure
    {
        public async Task AddIncident(string userId)
        {
            await Task.Run(() => { Console.WriteLine(userId); });
        }

        public async Task<bool> IsAccountLocked(string userId)
        {
            var isLocked = await Task.Run(() => {
                Console.WriteLine(userId); 
                return false;
            });
            return isLocked;
        }
        public async Task<bool> LockAccount(string userId)
        {
            var isLocked = await Task.Run(() => {
                Console.WriteLine(userId);
                return true;
            });
            return isLocked;
        }
        public async Task<bool> UnLockAccount(string userId)
        {
            var isLocked = await Task.Run(() => {
                Console.WriteLine("Unlocking account {0}", userId);
                return false;
            });
            return isLocked;
        }
    }
}
