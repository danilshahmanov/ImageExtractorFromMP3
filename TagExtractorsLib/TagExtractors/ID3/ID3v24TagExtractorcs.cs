namespace TagExtractorsLib.TagExtractors.ID3
{
    internal class ID3v23TagExtractor : ID3TagExtractor
    {
        protected override int GetFrameSize(Span<byte> sizeBytes) =>
            ((sizeBytes[0]) << 24) |
            ((sizeBytes[1]) << 16) |
            ((sizeBytes[2]) << 8) |
            (sizeBytes[3]);
    }

}
