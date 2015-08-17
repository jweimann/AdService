using AdService.Akka.Messages;
using Akka.Actor;

namespace AdService.Akka.Actors
{
    public class KeywordsRoot : ReceiveActor
    {
        public KeywordsRoot()
        {
            Receive<CreateKeyword>(message =>
            {
                Props props = Props.Create<Keyword>(message); // Specify that we want 'message' passed into the constructor of the Keyword actor
                IActorRef createdKeyword = Context.ActorOf(props, message.Keyword); // Create the keyword actor
                Sender.Tell(createdKeyword); // Reply to the Sender with the created keyword reference.
            });
        }
    }
}
