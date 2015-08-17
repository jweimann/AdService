using AdService.Akka.Messages;
using Akka.Actor;

namespace AdService.Akka.Actors
{
    public class AdsRoot : ReceiveActor
    {
        public AdsRoot()
        {
            Receive<CreateAd>(message =>
                {
                    Props props = Props.Create<Ad>(message);
                    IActorRef adActor = Context.ActorOf(props, message.UniqueAdName); // Creates the actor
                    Sender.Tell(adActor); // Respond to the Sender with the the address of the created Ad actor
                });
        }
    }
}
