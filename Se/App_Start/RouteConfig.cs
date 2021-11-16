using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Se
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            //routes.LowercaseUrls = true;
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new {
                    controller = nameof(Controllers.Public.HomeController).Replace(nameof(Controller), ""), //strogo tipizirano ime kontrolera i akcije
                    action = nameof(Controllers.Public.HomeController.Index), id = UrlParameter.Optional }
            );
        }
    }
}
