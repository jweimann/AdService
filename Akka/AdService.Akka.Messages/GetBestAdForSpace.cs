namespace AdService.Akka.Messages
{
    public class GetBestAdForSpace
    {
        public string UniqueSpaceName { get; private set; }
        public AdSpaceSize Size { get; private set; }

        public GetBestAdForSpace(string uniqueSpaceName, AdSpaceSize adSpaceSize)
        {
            UniqueSpaceName = uniqueSpaceName;
            Size = adSpaceSize;
        }
    }
}