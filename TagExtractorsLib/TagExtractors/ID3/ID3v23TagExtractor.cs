
namespace TagExtractorsLib.TagExtractors.ID3
{
    internal class ID3v24TagExtractor : ID3TagExtractor
    {

        protected override int GetFrameSize(Span<byte> sizeBytes) =>
            ((sizeBytes[0] & 0x7F) << 21) |
            ((sizeBytes[1] & 0x7F) << 14) |
            ((sizeBytes[2] & 0x7F) << 7) |
            (sizeBytes[3] & 0x7F);

    };
}
