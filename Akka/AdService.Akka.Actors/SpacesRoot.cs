using AdService.Akka.Messages;
using Akka.Actor;

namespace AdService.Akka.Actors
{
    public class SpacesRoot : ReceiveActor
    {
        public SpacesRoot()
        {
            Receive<CreateSpace>(message =>
            {
                Props props = Props.Create<Space>(message); // Specify that we want 'message' passed into the constructor of the Space actor
                Context.ActorOf(props, message.UniqueSpaceName); // Create the space actor.
            });
        }
    }
}
