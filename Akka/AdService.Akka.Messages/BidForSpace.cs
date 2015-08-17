namespace AdService.Akka.Messages
{
    public class BidForSpace
    {
        public decimal Bid { get; private set; }
        public string AdUrl { get; private set; }

        public BidForSpace(decimal bid, string adUrl)
        {
            Bid = bid;
            AdUrl = adUrl;
        }
    }
}
