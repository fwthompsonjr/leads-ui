namespace legallead.jdbc
{
    public enum ProfileChangeTypes
    {
        AccountCreatedBySystem = 0, // User account created by system
        AccountRegistrationCompleted = 2, // User account registration completed.
        UserContactNameChanged = 10, // User contact name changed by user.
        UserPhoneNumberChanged = 20, // User phone changed by user.
        UserEmailAddressChanged = 30, // User email changed by user.
        UserAddressDetailChanged = 40, // User address changed by user.
    }
}