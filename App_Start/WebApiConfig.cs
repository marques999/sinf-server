using System.Web.Http;
using System.Web.Http.Cors;

namespace FirstREST
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.EnableCors(new EnableCorsAttribute("*", "*", "*"));
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new
                {
                    id = RouteParameter.Optional
                }
            );
            config.Routes.MapHttpRoute(
                name: "DefaultApiTwoParams",
                routeTemplate: "api/{controller}/{id}/{sid}",
                defaults: new
                {
                    id = RouteParameter.Optional,
                    sid = RouteParameter.Optional
                }
            );
            config.Routes.MapHttpRoute(
                name: "DefaultApiThreeParams",
                routeTemplate: "api/{controller}/{id}/{sid}/{tid}",
                defaults: new
                {
                    id = RouteParameter.Optional,
                    sid = RouteParameter.Optional,
                    tid = RouteParameter.Optional
                }
            );
        }
    }
}