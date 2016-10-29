using System.Web.Mvc;
using System.Web.Routing;

namespace FirstREST
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            /* routes.MapRoute(
                 name: "Agenda",
                 url: "{controller}/{type}/{when}/{status}",
                 defaults: new
                 {
                     controller = "Agenda",
                     type = UrlParameter.Optional,
                     when = UrlParameter.Optional,
                     status = UrlParameter.Optional
                 }
             );*/
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new
                {
                    controller = "Home",
                    action = "Index",
                    id = UrlParameter.Optional
                }
            );
        }
    }
}