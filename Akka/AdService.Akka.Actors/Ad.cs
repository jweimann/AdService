using AdService.Akka.Messages;
using Akka.Actor;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace AdService.Akka.Actors
{
    public class Ad : ReceiveActor
    {
        private string _uniqueAdName;
        private decimal _price;
        private string _adUrl;
        private decimal _remainingMoney;
        private decimal _spentMoney;

        private decimal _pendingSpend;

        private string[] _keywords;
        private List<ActorSelection> _keywordActors;
        private Dictionary<IActorRef, int> _spaceServedCounts;
        private HashSet<IActorRef> _spacesWithPendingBid;

        private bool FundsAreAvailable {  get { return (_remainingMoney - _pendingSpend) >= _price; } }

        public Ad(CreateAd creationMessage)
        {
            _uniqueAdName = creationMessage.UniqueAdName;

            _keywordActors = new List<ActorSelection>();
            _spaceServedCounts = new Dictionary<IActorRef, int>();
            _spacesWithPendingBid = new HashSet<IActorRef>();

            Receive<SetPrice>(message =>
            {
                _price = message.Price;
            });

            Receive<AddFunds>(message =>
            {
                _remainingMoney += message.AmountToAdd;
            });

            Receive<SetKeywords>(message =>
            {
                foreach (var keyword in _keywordActors)
                    keyword.Tell(new UnregisterAdWithKeyword());

                _keywords = message.Keywords;

                _keywordActors.Clear();
                foreach(var keyword in _keywords)
                {
                    ActorSelection keywordActor = Context.System.ActorSelection($"/user/KeywordsRoot/{keyword}");
                    keywordActor.Tell(new RegisterAdWithKeyword());
                    _keywordActors.Add(keywordActor);
                }
            });

            Receive<SetAdUrl>(message =>
            {
                _adUrl = message.AdUrl;
            });

            Receive<GetBestAdForSpace>(message =>
            {
                if (FundsAreAvailable && _spacesWithPendingBid.Contains(Sender) == false)
                {
                    _pendingSpend += _price;
                    //Console.WriteLine($"Bid Sent - New PendingSpend: {_pendingSpend}");
                    Sender.Tell(new BidForSpace(_price, _adUrl));
                    _spacesWithPendingBid.Add(Sender);
                }
            });

            Receive<RejectBid>(_ =>
            {
                _pendingSpend -= _price;
                //Console.WriteLine($"Bid Rejected - New PendingSpend: {_pendingSpend}");
                _spacesWithPendingBid.Remove(Sender);
            });

            Receive<BidAccepted>(message =>
            {
                _remainingMoney -= _price;
                _spentMoney += _price;

                _spacesWithPendingBid.Remove(Sender);

                if (_remainingMoney < 0)
                    throw new Exception("Out of Money!!");

                _pendingSpend -= _price;
                if (_remainingMoney < _price)
                    DeregisterWithKeywords();

                UpdateStatistics(message);
            });

            Receive<PrintStats>(_ => HandlePrintStats());
        }

        private void HandlePrintStats()
        {
            Console.WriteLine("{0,10}{1,10}{2,10}{3,10}", _uniqueAdName, _price, _remainingMoney, _spentMoney);
            //Console.WriteLine($"Ad {_uniqueAdName} Remaining Money: {_remainingMoney}  Spent: {_spentMoney}");
            //Console.WriteLine($"{_uniqueAdName}\t{_remainingMoney}\t{_spentMoney}");
        }

        private void DeregisterWithKeywords()
        {
            foreach (var keywordActor in _keywordActors)
                keywordActor.Tell(new UnregisterAdWithKeyword());
        }

        private void UpdateStatistics(BidAccepted message)
        {
            if (_spaceServedCounts.ContainsKey(Sender) == false)
            {
                _spaceServedCounts.Add(Sender, 1);
                return;
            }
            _spaceServedCounts[Sender]++;
        }
    }
}
