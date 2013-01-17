using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Strathweb.HybridApiActionSelector.WebHost
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Services.Replace(typeof(IHttpActionSelector), new HybridActionSelector());

            config.Routes.MapHttpRoute(
                  name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}/{action}/{actionid}/{subaction}/{subactionid}",
                defaults: new { id = RouteParameter.Optional, action = RouteParameter.Optional, actionid = RouteParameter.Optional, subaction = RouteParameter.Optional, subactionid = RouteParameter.Optional }
            );
        }
    }
}
