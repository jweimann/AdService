namespace AdService.Akka.Messages
{
    public class SetKeywords
    {
        public string[] Keywords { get; private set; }
        public SetKeywords(params string[] keywords)
        {
            Keywords = keywords;
        }
    }
}
