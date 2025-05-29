using Microsoft.AspNetCore.Mvc;
using legallead.permissions.api.Extensions;
namespace legallead.permissions.api.Controllers
{
    [Route("")]
    public class ResourceController : Controller
    {
        [HttpGet]
        [Route("/img/{name}")]
        public IActionResult GetImage(string name)
        {
            const string mimeType = "image/png";
            if (string.IsNullOrEmpty(name)) return BadRequest();
            var shortName = Path.GetFileNameWithoutExtension(name) ?? name;
            if (ImageContent.TryGetValue(shortName, out var scriptObj)) return File(scriptObj, mimeType);
            var imageObj = Properties.Resources.ResourceManager.GetObject(shortName);
            if (imageObj is not byte[] image) return NotFound($"Image {name} not found.");
            ImageContent.Add(shortName, image);
            return File(image, mimeType);
        }

        [HttpGet]
        [Route("/js/{name}")]
        public IActionResult GetScript(string name)
        {
            const string mimeType = "text/javascript";
            if (string.IsNullOrEmpty(name)) return BadRequest();
            var shortName = Path.GetFileNameWithoutExtension(name) ?? name;
            if (JsContent.TryGetValue(shortName, out var scriptObj)) return File(scriptObj, mimeType);
            var obj = Properties.Resources.ResourceManager.GetObject(shortName);
            if (obj is not byte[] jscript) return NotFound($"Script {name} not found.");
            var innerContent = jscript.GetInnerText("script");
            if (null == innerContent) return BadRequest($"Script {name} is invalid.");
            JsContent.Add(shortName, innerContent);
            return File(innerContent, mimeType);
        }

        [HttpGet]
        [Route("/css/{name}")]
        public IActionResult GetCss(string name)
        {
            const string mimeType = "text/css";
            if (string.IsNullOrEmpty(name)) return BadRequest();
            var shortName = Path.GetFileNameWithoutExtension(name) ?? name;
            if (CssContent.TryGetValue(shortName, out var scriptObj)) return File(scriptObj, mimeType);
            var obj = Properties.Resources.ResourceManager.GetObject(shortName);
            if (obj is not byte[] css) return NotFound($"Script {name} not found.");
            var innerContent = css.GetInnerText("style");
            if (null == innerContent) return BadRequest($"Script {name} is invalid.");
            CssContent.Add(shortName, innerContent);
            return File(innerContent, "text/css");
        }

        private static readonly Dictionary<string, byte[]> ImageContent = [];
        private static readonly Dictionary<string, byte[]> JsContent = [];
        private static readonly Dictionary<string, byte[]> CssContent = [];
    }
}
