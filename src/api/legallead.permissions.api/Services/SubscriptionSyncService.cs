using legallead.jdbc.interfaces;
using Stripe;

namespace legallead.permissions.api.Services
{
    [ExcludeFromCodeCoverage(Justification = "This process directly interacts with data services and is for integration testing only.")]
    public class SubscriptionSyncService(
        ILogger<SubscriptionSyncService> log,
        ICustomerRepository infrastructure,
        bool isTestMode = false) : BackgroundService
    {
        private readonly ICustomerRepository _custDb = infrastructure;
        private readonly ILogger<SubscriptionSyncService> logger = log;
        private readonly bool _testMode = isTestMode;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Subscription Sync Service is running.");
            await BackgroundProcessing(stoppingToken);
        }

        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            if (stoppingToken.IsCancellationRequested) return;

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(25), stoppingToken);
                // do work
                var items = await Task.Run(async () =>
                {
                    var queue = await _custDb.GetUserSubscriptions(true);
                    return queue;
                }, stoppingToken);
                await ProcessSubscriptions(items, stoppingToken);
                if (_testMode) await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                else await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
            }
        }

        private async Task ProcessSubscriptions(List<SubscriptionDetailBo>? items, CancellationToken stoppingToken)
        {
            if (items == null || !items.Any()) return;
            var customers = items
                .GroupBy(x => x.CustomerId)
                .Select(s =>
                {
                    var item = s.First();
                    _ = item.SubscriptionType ?? "";
                    var details = items.FindAll(d => d.CustomerId == item.CustomerId);
                    details.Sort((b, a) => a.CreateDate.GetValueOrDefault().CompareTo(b.CreateDate.GetValueOrDefault()));
                    return new CustomerRef
                    {
                        CustomerId = item.CustomerId ?? "",
                        Email = item.Email ?? "",
                        UserId = item.UserId ?? "",

                        Details = details
                    };
                }).ToList();
            var service = new SubscriptionService();
            await Task.Run(() =>
            {
                customers.ForEach(async c => await ProcessCustomer(c, service, stoppingToken));
            }, stoppingToken);
        }

        private async Task ProcessCustomer(CustomerRef c, SubscriptionService service, CancellationToken stoppingToken)
        {
            if (!c.IsValid() || stoppingToken.IsCancellationRequested) return;
            var user = await _custDb.GetCustomer(new() { UserId = c.UserId });
            if (user == null || user.CustomerId == null || !user.CustomerId.Equals(c.CustomerId)) return;

            var subscriptionTypeNames = c.Details
                .Where(w => !string.IsNullOrEmpty(w.SubscriptionType))
                .Select(s => s.SubscriptionType ?? string.Empty).Distinct().ToList();
            var options = new SubscriptionListOptions { Customer = c.CustomerId };
            var subscriptions = service.List(options);
            if (subscriptions == null || !subscriptions.Any()) return;
            var cancelOption = new SubscriptionCancelOptions { Prorate = true, CancellationDetails = new() { Comment = "Subscription cancelled by system background process." } };
            foreach (var subscriptionType in subscriptionTypeNames)
            {
                var activeSubscription = c.Details.Find(x => x.SubscriptionType == subscriptionType);
                if (activeSubscription == null || string.IsNullOrWhiteSpace(activeSubscription.ExternalId)) continue;
                var activeId = activeSubscription.ExternalId;
                var remotes = Find(subscriptions, subscriptionType);
                // if the item reported from db as active subscription does not exist... exit without taking action
                if (!remotes.Any() || !remotes.Exists(x => x.ExternalId.Equals(activeId, StringComparison.OrdinalIgnoreCase))) continue;
                // find any active subscriptions in remote server that should be cancelled
                var staleItems = remotes.FindAll(x => !x.ExternalId.Equals(activeSubscription.ExternalId, StringComparison.OrdinalIgnoreCase));
                staleItems.ForEach(r =>
                {
                    r.IsProcessed = CancelItem(service, r.Id, cancelOption);
                });
                if (!staleItems.Exists(a => !a.IsProcessed))
                {
                    await CompleteVerification(c.Details.FindAll(x => x.SubscriptionType == subscriptionType));
                }
            }
        }

        private async Task CompleteVerification(List<SubscriptionDetailBo> subscriptions)
        {
            foreach (var subscription in subscriptions)
            {
                subscription.IsSubscriptionVerified = true;
                subscription.VerificationDate = DateTime.UtcNow;
                await _custDb.UpdateSubscriptionVerification(subscription);
            }
        }

        private static bool CancelItem(SubscriptionService service, string subscriptionId, SubscriptionCancelOptions options)
        {
            try
            {
                service.Cancel(subscriptionId, options);
                return true;
            }
            catch
            {
                return true;
            }
        }

        private static List<SubscriptionRef> Find(StripeList<Subscription> subscriptions, string subscriptionType)
        {
            const string subType = "SubscriptionType";
            const string extId = "ExternalId";
            var found = new List<SubscriptionRef>();
            foreach (var subscription in subscriptions)
            {
                var data = subscription.Metadata;
                _ = data.TryGetValue(extId, out var externalId);
                _ = data.TryGetValue(subType, out var remoteType);
                if (string.IsNullOrEmpty(externalId) || string.IsNullOrEmpty(remoteType)) continue;
                if (remoteType.Equals(subscriptionType, StringComparison.OrdinalIgnoreCase))
                {
                    found.Add(new()
                    {
                        Id = subscription.Id,
                        ExternalId = externalId,
                        SubscriptionType = subscriptionType,
                    });
                }
            }
            return found;
        }

        [ExcludeFromCodeCoverage(Justification = "This process directly interacts with data services and is for integration testing only.")]
        private sealed class CustomerRef
        {
            public string CustomerId { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string UserId { get; set; } = string.Empty;
            public List<SubscriptionDetailBo> Details { get; set; } = new();
            public bool IsValid()
            {
                if (string.IsNullOrWhiteSpace(CustomerId)) return false;
                if (string.IsNullOrWhiteSpace(Email)) return false;
                if (string.IsNullOrWhiteSpace(UserId)) return false;
                return true;
            }
        }

        [ExcludeFromCodeCoverage(Justification = "This process directly interacts with data services and is for integration testing only.")]
        private sealed class SubscriptionRef
        {
            public string Id { get; set; } = string.Empty;
            public string ExternalId { get; set; } = string.Empty;
            public string SubscriptionType { get; set; } = string.Empty;
            public bool IsProcessed { get; set; }
        }
    }
}