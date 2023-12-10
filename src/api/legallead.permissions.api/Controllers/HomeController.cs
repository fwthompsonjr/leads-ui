using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace legallead.permissions.api.Controllers
{
    [Route("/")]
    public class HomeController : Controller
    {
        [HttpGet]
        [Route("/")]
        public IActionResult Index()
        {
            var content = IndexHtml;
            return Content(content, "text/html");
        }

        private static string? _index;
        private static string IndexHtml => _index ??= GetIndex();
        private static string GetIndex()
        {
            var content = new StringBuilder("<html>");
            content.AppendLine();
            content.AppendLine("<head>");
            content.AppendLine(" <meta charset=`UTF-8` />");
            content.AppendLine(" <meta name=`viewport` content=`width=device-width, initial-scale=1.0` />");
            content.AppendLine(" <meta http-equiv=`X-UA-Compatible` content=`ie=edge` />");
            content.AppendLine(" <title>legallead: permissions api</title>");
            content.AppendLine(" <link rel=`preconnect` href=`https://fonts.gstatic.com` />");
            content.AppendLine(" <link href=`https://fonts.googleapis.com/css2?family=Roboto:wght@300;400;500;700&display=swap`");
            content.AppendLine(" rel=`stylesheet` />");
            content.AppendLine(" <link rel=`stylesheet`");
            content.AppendLine(" href=`https://unpkg.com/@astrouxds/astro-web-components/dist/astro-web-components/astro-web-components.css` />");
            content.AppendLine(" <script type=`module`");
            content.AppendLine(" src=`https://unpkg.com/@astrouxds/astro-web-components/dist/astro-web-components/astro-web-components.esm.js`></script>");
            content.AppendLine("</head>");
            content.AppendLine("<body>");
            content.AppendLine(" <rux-container name=`tha-container`>");
            content.AppendLine("   <div slot=`header`>");
            content.AppendLine("     legallead: permissions api");
            content.AppendLine("   </div>");
            content.AppendLine("   <div>");
            content.AppendLine("     <h2>permissions api</h2>");
            content.AppendLine("     <blockquote>");
            content.AppendLine("      The legallead.permissions.api is a component used to manage authentication and authorization of");
            content.AppendLine("      all key activities pertaining to users and applications in the leadlead applications suite.");
            content.AppendLine("     </blockquote>");
            content.AppendLine("   </div>");
            content.AppendLine(" </rux-container>");
            content.AppendLine("</body>");
            content.AppendLine("</html>");
            content.Replace('`', '"');
            return content.ToString();
        }
    }


}
