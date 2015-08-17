namespace AdService.Akka.Messages
{
    public class CreateSpace
    {
        public string UniqueSpaceName { get; private set; }
        public AdSpaceSize Size { get; private set; }
        public string[] Keywords { get; private set; }

        public CreateSpace(string uniqueSpaceName, AdSpaceSize adSpaceSize, params string[] keywords)
        {
            UniqueSpaceName = uniqueSpaceName;
            Size = adSpaceSize;
            Keywords = keywords;
        }
    }
}