namespace AdService.Akka.Messages
{
    public class SetAdUrl
    {
        public string AdUrl { get; private set; }

        public SetAdUrl(string adUrl)
        {
            AdUrl = adUrl;
        }
    }
}
