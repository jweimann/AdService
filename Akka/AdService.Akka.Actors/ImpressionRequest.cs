using AdService.Akka.Messages;
using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdService.Akka.Actors
{
    public class ImpressionRequest : ReceiveActor
    {
        private IActorRef _webActor;
        private decimal _bestBidPrice;
        private IActorRef _bestBidActor;
        private string _bestBidAdUrl;

        public ImpressionRequest(IActorRef webActor, GetBestAdForSpace requestToSend, List<ActorSelection> keywordActors)
        {
            _webActor = webActor;

            foreach (var keywordActor in keywordActors)
                keywordActor.Tell(requestToSend);

            Receive<BidForSpace>(message =>
            {
                if (message.Bid > _bestBidPrice)
                {
                    _bestBidActor?.Tell(new RejectBid());

                    _bestBidPrice = message.Bid;
                    _bestBidActor = Sender;
                    _bestBidAdUrl = message.AdUrl;
                    //Console.WriteLine($"New Best Bid found: {message.Bid} from {message.AdUrl}");
                }
                else
                {
                    Sender.Tell(new RejectBid());
                    //Console.WriteLine($"Inferior Bid rejected: {message.Bid} from {message.AdUrl}");
                }
            });

            Receive<SendBestReceivedBidSoFar>(_ =>
            {
                if (_bestBidPrice == 0)
                    _webActor.Tell("No Ads available");
                else
                {
                    _webActor.Tell($"{_bestBidAdUrl} @ {_bestBidPrice}");
                    _bestBidActor.Tell(new BidAccepted(Context.Parent));
                    Self.Tell(PoisonPill.Instance);
                }
            });

            Receive<PrintStats>(_ => HandlePrintStats());

            Context.System.Scheduler.ScheduleTellOnce(TimeSpan.FromMilliseconds(100), Self, new SendBestReceivedBidSoFar(), Self);
        }

        private void HandlePrintStats()
        {
            Console.WriteLine($"Impression {Context.Parent.Path} BestBid: {_bestBidPrice}");
        }
    }
}
