
namespace TagExtractorsLib.DataModels
{
    public class Picture
    {
        /// <summary>
        /// MIME тип изображения
        /// </summary>
        public string MimeType { get; set; }
        /// <summary>
        /// Описание изображения
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// ID3 тип изображения
        /// </summary>
        public byte? Type { get; set; }
        /// <summary>
        /// Байты изображения
        /// </summary>

        public byte[] Data { get; set; }
    }
}
