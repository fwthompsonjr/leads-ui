using legallead.desktop.entities;
using System.Diagnostics.CodeAnalysis;

namespace legallead.desktop.utilities
{
    internal class CommonMessageList
    {
        private List<CommonMessage>? messages;
        public List<CommonMessage> Messages => messages ??= GetMessages();

        [ExcludeFromCodeCoverage(Justification = "Private member tested from public method.")]
        private static List<CommonMessage> GetMessages()
        {
            var content = Properties.Resources.common_status;
            return ObjectExtensions.TryGet<List<CommonMessage>>(content);
        }
    }
}
