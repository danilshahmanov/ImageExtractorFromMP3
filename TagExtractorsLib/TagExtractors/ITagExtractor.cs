using TagExtractorsLib.DataModels;

namespace TagExtractorsLib.TagExtractors
{
    internal interface ITagExtractor
    {
        public TagData ExtractTagData(FileStream stream);
    }
}
