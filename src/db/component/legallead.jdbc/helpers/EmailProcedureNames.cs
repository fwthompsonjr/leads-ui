namespace legallead.jdbc.helpers
{
    internal static class EmailProcedureNames
    {
        public const string GetEmailBody = "CALL PRC_EMAIL_GET_MAIL_MESSAGE_BODY( ?, ? );";
        public const string GetCount = "CALL PRC_EMAIL_GET_MAIL_COUNT( ? );";
        public const string GetMailMessages = "CALL PRC_EMAIL_GET_MAIL_MESSAGES( ?, ? );";
    }
}
