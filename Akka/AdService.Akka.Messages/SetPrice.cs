namespace AdService.Akka.Messages
{
    public class SetPrice
    {
        public decimal Price { get; private set; }
        public SetPrice(decimal price)
        {
            Price = price;
        }
    }
}
