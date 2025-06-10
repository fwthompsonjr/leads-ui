using legallead.permissions.api.Services;
using Microsoft.AspNetCore.Mvc;

namespace legallead.permissions.api.Controllers
{
    [Route("/help")]
    public class HelpController : Controller
    {
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            var content = HelpContentService.GetKey("help-base-layout");
            if (string.IsNullOrEmpty(content)) return NoContent();
            return Content(content, "text/html");
        }
        [HttpGet]
        [Route("{topic}")]
        public IActionResult Topic(string topic)
        {
            if (!topicsMap.TryGetValue(topic, out var result)) return NoContent();
            var content = HelpContentService.GetChildPage(result);
            if (string.IsNullOrEmpty(content)) return NoContent();
            return Content(content, "text/html");
        }


        private static readonly Dictionary<string, string> topicsMap = new()
        {
            { "account-settings", HelpContentService.SectionNames.TopicAccountSettings },
            { "getting-started", HelpContentService.SectionNames.TopicGettingStarted },
            { "billing", HelpContentService.SectionNames.TopicBilling },
        };
    }
}
