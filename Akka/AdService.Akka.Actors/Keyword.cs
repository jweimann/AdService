using AdService.Akka.Messages;
using Akka.Actor;
using System.Collections.Generic;

namespace AdService.Akka.Actors
{
    public class Keyword : ReceiveActor
    {
        private string _keyword;
        private HashSet<IActorRef> _adActors;

        public Keyword(CreateKeyword createKeywordMessage)
        {
            _keyword = createKeywordMessage.Keyword;

            _adActors = new HashSet<IActorRef>();

            Receive<RegisterAdWithKeyword>(message =>
            {
                if (!_adActors.Contains(Sender))
                    _adActors.Add(Sender);
            });

            Receive<UnregisterAdWithKeyword>(message =>
            {
                if (_adActors.Contains(Sender))
                    _adActors.Remove(Sender);
            });

            Receive<GetBestAdForSpace>(message =>
            {
                foreach(var adActor in _adActors)
                    adActor.Tell(message, Sender); // Pass the space on as the sender so the ad replies directly to it.
            });
        }
    }
}
