using AdService.Akka.Actors;
using Akka.Actor;
using Akka.Routing;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace AdService.Web
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        public static ActorSystem Cluster { get; private set; }
        public static IActorRef Router { get; private set; }
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Cluster = ActorSystem.Create("AdService");
            Router = Cluster.ActorOf(Props.Create(() => new WebQueryProcessActor()).WithRouter(FromConfig.Instance), "RequestListener");

            var resolved = Router.Path;
            //var bestAdUrl = Router.Ask<string>(new Akka.Messages.RequestAd(), System.TimeSpan.FromSeconds(10)).Result;

        }
    }
}
