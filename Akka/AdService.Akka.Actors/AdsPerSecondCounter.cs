using AdService.Akka.Messages;
using Akka.Actor;
using System;

namespace AdService.Akka.Actors
{
    public class AdsPerSecondCounter : ReceiveActor
    {
        private int _adsProcessedThisSecond;
        public AdsPerSecondCounter()
        {
            Receive<AdProcessed>(_ => _adsProcessedThisSecond++);
            Receive<PrintStats>(_ =>
            {
                Console.WriteLine($"{_adsProcessedThisSecond} ads processed per second");
                _adsProcessedThisSecond = 0;
            });

            Context.System.Scheduler.ScheduleTellRepeatedly(1000, 1000, Self, new PrintStats(), Self);
        }
    }
}
