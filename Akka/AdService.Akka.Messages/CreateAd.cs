namespace AdService.Akka.Messages
{
    public class CreateAd
    {
        public string UniqueAdName { get; private set; }
       
        public CreateAd(string uniqueAdName)
        {
            UniqueAdName = uniqueAdName;
        }
    }
}