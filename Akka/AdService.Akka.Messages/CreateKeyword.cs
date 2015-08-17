namespace AdService.Akka.Messages
{
    public class CreateKeyword
    {
        public string Keyword { get; private set; }
        public CreateKeyword(string keyword)
        {
            Keyword = keyword;
        }
    }
}
