using AdService.Akka.Actors;
using AdService.Akka.Messages;
using Akka.Actor;
using System;
using System.Collections.Generic;

namespace AdService.Akka.Host
{
    class Program
    {
        private const decimal STARTING_MONEY = 100.0m;
        private const int IMPRESSIONS = 25000;

        private static ActorSystem _system;
        private static IActorRef _spacesRoot; // Used for debug printing.
        private static List<IActorRef> _allAdActors; // Used for debug printing.
        private static int _countComplete = 0;
        private static IActorRef _adsPerSecondStatisticsActor;
        static void Main(string[] args)
        {
            _system = ActorSystem.Create("AdService");
            CreateKeywords();
            CreateSpaces();
            CreateAds();
            _system.ActorOf<WebQueryProcessActor>("RequestListener");


            _adsPerSecondStatisticsActor = _system.ActorOf<AdsPerSecondCounter>();

            System.Threading.Thread.Sleep(1000); // Wait for system to initialize.
            //Console.WriteLine("Press [Enter] to start.");
            Console.ReadLine();

            for (int i = 0; i < IMPRESSIONS; i++)
            {
                GetBestAdForSpace("JasonsBanner", i);
                GetBestAdForSpace("JasonsSide", i);
                GetBestAdForSpace("SomeonesMobileApp", i);
                GetBestAdForSpace("DogGameBanner", i);
                //System.Threading.Thread.Sleep(1);
            }
        
            //while (_countComplete < IMPRESSIONS * 3)
            //    System.Threading.Thread.Sleep(1);

            Console.WriteLine("Done\nPress [Enter] for stats.");
            Console.ReadLine();

            foreach (var adActor in _allAdActors)
                adActor.Tell(new PrintStats());

            ActorSelection allImpressions = new ActorSelection(_spacesRoot, "*/*"); // This isn't sending right now.
            allImpressions.Tell(new PrintStats());

            Console.ReadLine();
        }
        
        private static void CreateAds()
        {
            System.Threading.Thread.Sleep(10);
            _allAdActors = new List<IActorRef>();

            IActorRef adsRoot = _system.ActorOf<AdsRoot>("AdsRoot");
            
            CreateAd(adsRoot, "Powerade", "www.powerade.com", 0.20m, STARTING_MONEY, "Sports", "Food");
            CreateAd(adsRoot, "VitaminWater", "www.vitaminwater.com", 0.10m, STARTING_MONEY, "Sports", "Food");
            CreateAd(adsRoot, "SoBe", "www.SoBe.com", 0.18m, STARTING_MONEY, "Sports", "Food");
            CreateAd(adsRoot, "Mizone", "www.Mizone.com", 0.02m, STARTING_MONEY, "Sports", "Food");
            CreateAd(adsRoot, "Gatorade", "www.gatorade.com", 0.25m, STARTING_MONEY, "Sports", "Food");
            CreateAd(adsRoot, "AllSport", "www.AllSport.com", 0.05m, STARTING_MONEY, "Sports", "Food");
            CreateAd(adsRoot, "Accelerade", "www.Accelerade.com", 0.01m, STARTING_MONEY, "Sports", "Food");
            CreateAd(adsRoot, "Staminade", "www.Staminade.com", 0.01m, STARTING_MONEY, "Sports", "Food");
            CreateAd(adsRoot, "PetSmart", "www.PetSmart.com", 0.01m, STARTING_MONEY*100, "Pets");
            CreateAd(adsRoot, "PetCo", "www.PetCo.com", 0.01m, STARTING_MONEY, "Dogs");
        }

        private static void CreateAd(IActorRef adsRoot, string adName, string adUrl, decimal pricePerImpression, decimal availableFunds, params string[] keywords)
        {
            IActorRef adActor = adsRoot.Ask<IActorRef>(new CreateAd(adName)).Result; // Create the ad and keep a reference.
            adActor.Tell(new SetPrice(pricePerImpression));
            adActor.Tell(new SetKeywords(keywords));
            adActor.Tell(new SetAdUrl(adUrl));
            adActor.Tell(new AddFunds(availableFunds));

            _allAdActors.Add(adActor);
        }

        private static void CreateKeywords()
        {
            System.Threading.Thread.Sleep(10);
            IActorRef keywordsRoot = _system.ActorOf<KeywordsRoot>("KeywordsRoot");
            keywordsRoot.Tell(new CreateKeyword("Sports")); // Create the Sports actor.  Don't need to use Ask<> here because we don't need the reference.
            keywordsRoot.Tell(new CreateKeyword("Food"));
            keywordsRoot.Tell(new CreateKeyword("Pets"));
        }
        
        private static void CreateSpaces()
        {
            System.Threading.Thread.Sleep(10);
            _spacesRoot = _system.ActorOf<SpacesRoot>("SpacesRoot");
            _spacesRoot.Tell(new CreateSpace("JasonsBanner", AdSpaceSize.Large, "Sports", "Food")); // Create a space for Jason's banner.  Add the Sports and Food keywords
            _spacesRoot.Tell(new CreateSpace("JasonsSide", AdSpaceSize.Medium, "Sports", "Food"));
            _spacesRoot.Tell(new CreateSpace("SomeonesMobileApp", AdSpaceSize.Small, "Sports", "Food"));
            _spacesRoot.Tell(new CreateSpace("DogGameBanner", AdSpaceSize.Small, "Pets", "Dogs"));
        }
        private static async void GetBestAdForSpace(string uniqueSpaceName, int bidOrder)
        {
            ActorSelection spaceActorSelection = _system.ActorSelection($"/user/SpacesRoot/{uniqueSpaceName}");
            string adUrl = await spaceActorSelection.Ask<string>(new RequestAd());//, TimeSpan.FromMilliseconds(100));
            _countComplete++;
            _adsPerSecondStatisticsActor.Tell(new AdProcessed());
            //Console.WriteLine($"{bidOrder} Best Ad is: {adUrl}");
        }
    }
}
