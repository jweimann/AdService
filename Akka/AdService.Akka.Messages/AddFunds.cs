namespace AdService.Akka.Messages
{
    public class AddFunds
    {
        public decimal AmountToAdd { get; private set; }
        public AddFunds(decimal amountToAdd)
        {
            AmountToAdd = amountToAdd;
        }
    }
}
