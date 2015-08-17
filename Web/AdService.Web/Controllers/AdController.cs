using AdService.Akka.Messages;
using Akka.Actor;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace AdService.Web.Controllers
{
    public class AdController : ApiController
    {
        [HttpGet]
        public async Task<string> GetBestAdUrl(string uniqueSpaceName)
        {
            //ActorSelection spaceActorSelection =
            //    WebApiApplication.Cluster.ActorSelection($"akka://AdService/user/RequestListener");// $"/user/SpacesRoot/{uniqueSpaceName}");

            ActorSelection spaceActorSelection =
                WebApiApplication.Cluster.ActorSelection($"akka.tcp://user/SpacesRoot/");//{uniqueSpaceName}");

             IActorRef actor = await spaceActorSelection.ResolveOne(TimeSpan.FromSeconds(1));

            var bestAdUrl = await spaceActorSelection.Ask<string>(new RequestAd(), TimeSpan.FromSeconds(1));
            return bestAdUrl;
        }
    }
}
