namespace legallead.jdbc
{
    public enum PermissionChangeTypes
    {
        AccountCreatedBySystem = 0, // User account created by system
        AccountRegistrationCompleted = 2, // User account registration completed.
        PermissionLevelChanged = 10, // User Permission level changed 
        SubscriptionStateChanged = 20, // User State Subscription changed 
        SubscriptionCountyChanged = 22, // User County Subscription changed 
        LockedAccountExcessiveLogin = 30, // User account temporarily locked, excessive login failures 
        LockedAccountNonPayment = 32, // "User account locked for non-payment 
        LockedAccountByAdmin = 34, // User account locked by administrator 
    }
}
