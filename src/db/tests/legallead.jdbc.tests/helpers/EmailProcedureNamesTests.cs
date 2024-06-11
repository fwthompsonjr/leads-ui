using legallead.jdbc.helpers;

namespace legallead.jdbc.tests.helpers
{
    public class EmailProcedureNamesTests
    {

        [Theory]
        [InlineData(0, "CALL PRC_EMAIL_GET_MAIL_MESSAGE_BODY( ?, ? );")]
        [InlineData(1, "CALL PRC_EMAIL_GET_MAIL_COUNT( ? );")]
        [InlineData(2, "CALL PRC_EMAIL_GET_MAIL_MESSAGES( ?, ? );")]
        public void ProcContainsName(int index, string expected)
        {
            var prcName = index switch
            {
                0 => EmailProcedureNames.GetEmailBody,
                1 => EmailProcedureNames.GetCount,
                2 => EmailProcedureNames.GetMailMessages,
                _ => string.Empty
            };
            Assert.Equal(expected, prcName);
        }
    }
}
