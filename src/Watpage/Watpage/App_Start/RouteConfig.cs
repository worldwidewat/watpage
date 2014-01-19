using System.Web.Mvc;
using System.Web.Routing;

namespace Watpage
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Page",
                url: "{name}",
                defaults: new { controller = "Page", action = "Index" }
            );
        }
    }
}