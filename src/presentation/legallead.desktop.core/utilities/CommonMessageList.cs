using legallead.desktop.entities;

namespace legallead.desktop.utilities
{
    internal class CommonMessageList
    {
        private List<CommonMessage>? messages;
        public List<CommonMessage> Messages => messages ??= GetMessages();

        private static List<CommonMessage> GetMessages()
        {
            var content = Properties.Resources.common_status;
            return ObjectExtensions.TryGet<List<CommonMessage>>(content);
        }
    }
}
