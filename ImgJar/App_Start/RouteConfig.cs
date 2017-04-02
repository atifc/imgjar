using System.Web.Mvc;
using System.Web.Routing;

namespace ImgJar
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Upload",
                url: "Upload",
                defaults: new { controller = "Upload", action = "Upload" }
            );

            routes.MapRoute(
                name: "Media",
                url: "r/{removalKey}",
                defaults: new { controller = "Media", action = "Delete" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{action}",
                defaults: new { controller = "Home", action = "Index" }
            );
        }
    }
}
