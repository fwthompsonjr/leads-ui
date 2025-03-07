﻿using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace legallead.permissions.api.Controllers
{
    [Route("/")]
    [SuppressMessage("Sonar Qube Violation", "S6931:ASP.NET controller actions should not have a route template starting with \"/\"", Justification = "<Pending>")]
    public partial class HomeController(
        IPaymentHtmlTranslator service,
        ISearchInfrastructure search,
        ISubscriptionInfrastructure subscriptionSvc,
        ICustomerLockInfrastructure lockingDb,
        IStripeInfrastructure stripe,
        IClientSecretService secretService) : Controller
    {
        private readonly IPaymentHtmlTranslator paymentSvc = service;
        private readonly ISearchInfrastructure infrastructure = search;
        private readonly ISubscriptionInfrastructure subscriptionSvc = subscriptionSvc;
        private readonly ICustomerLockInfrastructure _lockingDb = lockingDb;
        private readonly IStripeInfrastructure stripeSvc = stripe;
        private readonly IClientSecretService secretSvc = secretService;

        [HttpGet]
        [Route("/")]
        public IActionResult Index()
        {
            var content = IndexHtml;
            return Content(content, "text/html");
        }
        private static readonly string NoPaymentItem = Guid.Empty.ToString("D");

        private static string? _index;
        private static string IndexHtml => _index ??= GetIndex();

        private static string GetIndex()
        {
            var operations = new List<string>()
            {
                "Account Management",
                "Account Registration",
                "Account Login",
                "Account Password Reset",
                "Common Lists",
                "Manage User Permssions",
                "Manage User Profile",
            };
            var listitems = "<ul>" + string.Join(Environment.NewLine, operations.Select(x => $"<li>{x}</li>")) + "</ul>";
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
            content.AppendLine(" <rux-container name=`tha-container` style=`padding: 10px`>");
            content.AppendLine("   <div slot=`header`>");
            content.AppendLine("     legallead: permissions api");
            content.AppendLine("   </div>");
            content.AppendLine("   <div>");
            content.AppendLine("     <h2Permissions Api</h2>");
            content.AppendLine("     <blockquote>");
            content.AppendLine("      The legallead.permissions.api is a component used to manage authentication and authorization of <br/>");
            content.AppendLine("      all key activities pertaining to users and applications in the leadlead applications suite.");
            content.AppendLine("     </blockquote>");
            content.AppendLine("   </div>");
            content.AppendLine("   <div>");
            content.AppendLine("     <h2>Operations Overview: </h2>");
            content.AppendLine(listitems);
            content.AppendLine("   </div>");
            content.AppendLine(" </rux-container>");
            content.AppendLine("</body>");
            content.AppendLine("</html>");
            content.Replace('`', '"');
            return content.ToString();
        }
    }
}