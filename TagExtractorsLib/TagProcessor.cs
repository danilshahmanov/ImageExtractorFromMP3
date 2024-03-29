using TagExtractorsLib.DataModels;
using TagExtractorsLib.TagExtractors;


namespace TagExtractorsLib
{
    public static class TagProcessor
    {
        /// <summary>
        /// Выдает метаданные из тегов файла согласно формату
        /// </summary>
        /// <param name="fileStream">Путь к файлу</param>
        /// <returns>Метаданные из тегов файла</returns>
        public static TagData GetTagData(string filePath)
        {
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return TagExtractorFactory.GetTagExtractor(fileStream).ExtractTagData(fileStream);
        }
        public static void ChangeTagData(string filePath)
        {
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite);
        }
    }
}
