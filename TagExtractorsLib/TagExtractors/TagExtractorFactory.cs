using TagExtractorsLib.TagExtractors.ID3;

namespace TagExtractorsLib.TagExtractors
{
    //add flyweight????
    internal static class TagExtractorFactory
    {
        /// <summary>
        /// Выдает определенный TagExtractor согласно формату метаданных файла
        /// </summary>
        /// <param name="fileStream">Поток файла с тегом</param>
        /// <returns>TagExtractor согласно формату метаданных файла</returns>

        public static ITagExtractor GetTagExtractor(FileStream fileStream)
        {
            if (ID3TagExtractor.IsID3Format(fileStream, out int version))
            {
                if (version == 4)
                    return new ID3v24TagExtractor();
                if (version == 3)
                    return new ID3v23TagExtractor();
                else
                    throw new FormatException("Неподдерживаемая версия ID3.");
            }
            else
                throw new FormatException("Неподдерживаемый формат файла.");
        }

    }
}
