using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.SelfHost;

namespace Strathweb.HybridApiActionSelector.SelfHost
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new HttpSelfHostConfiguration("http://localhost:999");
            config.Services.Replace(typeof(IHttpActionSelector), new HybridActionSelector());

            config.Routes.MapHttpRoute(
                  name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}/{action}/{actionid}/{subaction}/{subactionid}",
                defaults: new { id = RouteParameter.Optional, action = RouteParameter.Optional, actionid = RouteParameter.Optional, subaction = RouteParameter.Optional, subactionid = RouteParameter.Optional }
            );
            var server = new HttpSelfHostServer(config);

            server.OpenAsync().Wait();

            Console.ReadKey();

            server.CloseAsync().Wait();
        }
    }
}
