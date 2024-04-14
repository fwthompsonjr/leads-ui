using legallead.jdbc.entities;
using legallead.jdbc.interfaces;
using legallead.permissions.api.Interfaces;

namespace legallead.permissions.api.Utility
{
    public class CustomerLockInfrastructure : ICustomerLockInfrastructure
    {
        private readonly IUserLockStatusRepository lockRepo;
        public CustomerLockInfrastructure(IUserLockStatusRepository repo)
        {
            lockRepo = repo;
        }
        public async Task AddIncident(string userId)
        {
            await lockRepo.AddIncident(new() { Id = userId });
        }

        public async Task<bool> IsAccountLocked(string userId)
        {
            var response = await lockRepo.GetStatus(userId);
            if (response == null) { return false; }
            if (!response.IsLocked.GetValueOrDefault())
            {
                // if this account is not locked, confirm that attempts are not exceeded
                var verification = await CalculateLockStatus(response);
                return verification;
            }
            if (response.CanResetAccount.GetValueOrDefault())
            {
                // account is locked, but reset date has passed
                await UnLockAccount(userId);
                response = await lockRepo.GetStatus(userId);
                return response != null && !response.IsLocked.GetValueOrDefault();
            }
            return response.IsLocked.GetValueOrDefault();
        }
        public async Task<bool> LockAccount(string userId)
        {
            var isLocked = await Task.Run(async () =>
            {
                for (var x=0; x < 10; x++) await AddIncident(userId);
                return true;
            });
            return isLocked;
        }
        public async Task<bool> UnLockAccount(string userId)
        {
            var response = await lockRepo.Unlock(new() { UserId = userId });
            return response.Key;
        }

        private async Task<bool> CalculateLockStatus(UserLockStatusBo bo)
        {
            var failed = bo.FailedAttemptCount.GetValueOrDefault(-1);
            var max = bo.MaxFailedAttempts.GetValueOrDefault();
            var attemptsExceeded = failed >= max;
            var isWindowExpired = bo.FailedAttemptResetDt.GetValueOrDefault() <= DateTime.UtcNow;
            if (attemptsExceeded && !isWindowExpired)
            {
                await LockAccount(bo.UserId ?? "");
                return true;
            }
            return false;
        }
    }
}
