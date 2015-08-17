using AdService.Akka.Messages;
using Akka.Actor;
using System.Collections.Generic;

namespace AdService.Akka.Actors
{
    public class Space : ReceiveActor
    {
        private string _uniqueSpaceName;
        private AdSpaceSize _size;

        private string[] _keywords;
        private List<ActorSelection> _keywordActors;

        // This is the current bottleneck
        public Space(CreateSpace createSpaceMessage)
        {
            _uniqueSpaceName = createSpaceMessage.UniqueSpaceName;
            _size = createSpaceMessage.Size;
            _keywords = createSpaceMessage.Keywords;

            _keywordActors = new List<ActorSelection>();

            foreach (var keyword in _keywords)
            {
                ActorSelection keywordActor = Context.System.ActorSelection($"/user/KeywordsRoot/{keyword}");
                _keywordActors.Add(keywordActor);
            }

            Receive<RequestAd>(message =>
            {
                var request = new GetBestAdForSpace(_uniqueSpaceName, _size); // Create a request for the best ad, send it to our keywords.

                Props props = Props.Create<ImpressionRequest>(Sender, request, _keywordActors);
                IActorRef impressionRequest = Context.ActorOf(props);
            });

         
        }
    }
}