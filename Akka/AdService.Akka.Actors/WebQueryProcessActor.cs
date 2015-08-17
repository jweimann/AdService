using AdService.Akka.Messages;
using Akka.Actor;
using System;

namespace AdService.Akka.Actors
{
    public class WebQueryProcessActor : ReceiveActor
    {
        public WebQueryProcessActor()
        {
            Console.WriteLine("CREATED WebQueryProcessActor");
            //return;
            //System.Threading.Thread.Sleep(1000);
            string uniqueSpaceName = "JasonsBanner";
            //ActorSelection spaceActorSelection =
            //Context.System.ActorSelection($"akka://AdService/user/RequestListener");///SpacesRoot/{uniqueSpaceName}");

            //IActorRef actor = spaceActorSelection.ResolveOne(TimeSpan.FromSeconds(1)).Result;

            Receive<RequestAd>(message =>
            {
                Console.WriteLine("Received RequestAd");
            });
        }


    }
}
