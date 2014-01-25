using System.Web.Mvc;
using System.Web.Routing;

namespace Watpage
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute("Resource", "Resource/{page}/{name}", new { controller = "Page", action = "Resource" });
            routes.MapRoute("Page", "{page}", new { controller = "Page", action = "Index", page = UrlParameter.Optional });
        }
    }
}