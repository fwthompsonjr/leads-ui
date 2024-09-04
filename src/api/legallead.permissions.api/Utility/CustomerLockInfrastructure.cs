using legallead.jdbc.interfaces;

namespace legallead.permissions.api.Utility
{
    public class CustomerLockInfrastructure(IUserLockStatusRepository repo) : ICustomerLockInfrastructure
    {
        private readonly IUserLockStatusRepository lockRepo = repo;

        public async Task AddIncidentAsync(string userId)
        {
            await lockRepo.AddIncident(new() { Id = userId });
        }

        public async Task<bool> IsAccountLockedAsync(string userId)
        {
            var response = await lockRepo.GetStatus(userId);
            if (response == null) { return false; }
            if (!response.IsLocked.GetValueOrDefault())
            {
                // if this account is not locked, confirm that attempts are not exceeded
                var verification = await CalculateLockStatusAsync(response);
                return verification;
            }
            if (response.CanResetAccount.GetValueOrDefault())
            {
                // account is locked, but reset date has passed
                await UnLockAccountAsync(userId);
                response = await lockRepo.GetStatus(userId);
                return response != null && !response.IsLocked.GetValueOrDefault();
            }
            return response.IsLocked.GetValueOrDefault();
        }
        public async Task<bool> LockAccountAsync(string userId)
        {
            var isLocked = await Task.Run(async () =>
            {
                for (var x = 0; x < 10; x++) await AddIncidentAsync(userId);
                return true;
            });
            return isLocked;
        }
        public async Task<bool> UnLockAccountAsync(string userId)
        {
            var response = await lockRepo.Unlock(new() { UserId = userId });
            return response.Key;
        }

        private async Task<bool> CalculateLockStatusAsync(UserLockStatusBo bo)
        {
            var failed = bo.FailedAttemptCount.GetValueOrDefault(-1);
            var max = bo.MaxFailedAttempts.GetValueOrDefault();
            var attemptsExceeded = failed >= max;
            var isWindowExpired = bo.FailedAttemptResetDt.GetValueOrDefault() <= DateTime.UtcNow;
            if (attemptsExceeded && !isWindowExpired)
            {
                await LockAccountAsync(bo.UserId ?? "");
                return true;
            }
            return false;
        }
    }
}
