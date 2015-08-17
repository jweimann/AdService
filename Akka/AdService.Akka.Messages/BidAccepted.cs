using Akka.Actor;

namespace AdService.Akka.Messages
{
    public class BidAccepted
    {
        public IActorRef SpaceActor { get; private set; }

        public BidAccepted(IActorRef spaceActor)
        {
            SpaceActor = spaceActor;
        }
    }
}
