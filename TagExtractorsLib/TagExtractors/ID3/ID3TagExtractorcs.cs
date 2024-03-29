using System.IO;
using System.Text;
using TagExtractorsLib.DataModels;
using TagExtractorsLib.Extensions;

namespace TagExtractorsLib.TagExtractors.ID3
{
    internal abstract class ID3TagExtractor : ITagExtractor
    {
        /// <summary>
        /// Сопоставляет байт кодировки кодировке согласно формату ID3
        /// </summary>
        /// <param name="encodingByte">Байт кодировки согласно формату ID3</param>
        /// <returns>Encoding, соответсвующий байту кодировки</returns>
        protected static Encoding ToEncoding(byte encodingByte) => encodingByte switch
        {
            0x0 => Encoding.Latin1,
            0x1 => Encoding.Unicode,
            0x2 => Encoding.BigEndianUnicode,
            0x3 => Encoding.UTF8,
            _ => throw new ArgumentOutOfRangeException("Некорректный байт кодировки."),
        };
      
        /// <summary>
        /// Читает и извлекает информацию из заголовка тега ID3 (10 бвйт)
        /// </summary>
        /// <param name="fileStream">Поток файла с тегом</param>
        /// <returns>Название тега и его размер за исключением заголовка (10 байт)</returns>
        protected (string frameName, int frameSize) ExtractFrameHeader(FileStream fileStream)
        {
            Span<byte> headerBytes = stackalloc byte[10];
            fileStream.Read(headerBytes);
            var frameName = Encoding.ASCII.GetString(headerBytes[..4]);
            var frameSize = GetFrameSize(headerBytes.Slice(4, 4));
            return (frameName, frameSize);
        }
        /// <summary>
        /// Переводит 4 байта размера в число согласно версии ID3
        /// </summary>
        /// <param name="sizeBytes">4 байта для перевода</param>
        /// <returns>Размер в виде числа</returns>
        protected abstract int GetFrameSize(Span<byte> sizeBytes);
        /// <summary>
        /// Читает и извлекает информацию из текстового тега
        /// </summary>
        /// <param name="fileStream">Поток файла с тегом</param>
        /// <param name="frameSize">Размер содержимого тега</param>
        /// <returns>Извлеченный текст</returns>
        protected string ExtractTextFrame(Stream fileStream, int frameSize)
        {
            Span<byte> dataBytes = stackalloc byte[frameSize];
            fileStream.Read(dataBytes);
            return ToEncoding(dataBytes[0])
                .GetString(dataBytes.Slice(1));
        }
        /// <summary>
        /// Читает и извлекает информацию из тега APIC
        /// </summary>
        /// <param name="fileStream">Поток файла с тегом</param>
        /// <param name="frameSize">Размер содержимого тега</param>
        /// <returns>Извлеченное изображение с метаданными</returns>
        protected Picture ExtractAttachedPicture(Stream fileStream, int frameSize)
        { 
            Picture picture = new();
            int offset = 0;

            byte encodingByte = (byte)fileStream.ReadByte();
            var mimeTypeBytes = fileStream.ReadTillTerminator(64, 0x0).ToArray();
            picture.MimeType = ToEncoding(encodingByte).GetString(mimeTypeBytes);
            picture.Type = (byte)fileStream.ReadByte();
            var descriptionBytes = fileStream.ReadTillTerminator(64, 0x0).ToArray();
            picture.Description = ToEncoding(encodingByte).GetString(descriptionBytes);

            offset += mimeTypeBytes.Length + descriptionBytes.Length + 2;
            picture.Data = fileStream.ReadAsByteArray(frameSize - offset);
            return picture;
        }
        /// <summary>
        /// Проверяет является ли заголовок ID3 расширенным
        /// </summary>
        /// <param name="flagByte">Флаговый байт из заголовка</param>
        private bool HasExtendedHeader(byte flagByte) => (flagByte & 0x40) != 0;
        /// <summary>
        /// Читает и извлекает информацию заголовка ID3
        /// </summary>
        /// <param name="fileStream">Поток файла с тегом</param>
        /// <returns>Размер всех тегов за исключением самого заголовка (10 байт)</returns>
        private int ProcessHeader(FileStream fileStream)
        {
            Span<byte> headerBytes = stackalloc byte[6];
            fileStream.Read(headerBytes);
            if (HasExtendedHeader(headerBytes[1]))
                ProcessExtendedHeader(fileStream);
            var metaDataSize = GetFrameSize(headerBytes[2..]);
            return metaDataSize;
        }
        /// <summary>
        /// Читает информацию расширенного заголовка ID3
        /// </summary>
        /// <param name="fileStream">Поток файла с тегом</param>
        private void ProcessExtendedHeader(FileStream fileStream)
        {
            Span<byte> headerSizeBytes = stackalloc byte[4];
            fileStream.Read(headerSizeBytes);
            fileStream.Seek(
                GetFrameSize(headerSizeBytes),
                SeekOrigin.Current);
        }
        /// <summary>
        /// Читает и извлекает информацию из тегов ID3
        /// </summary>
        /// <param name="fileStream">Поток файла с тегом</param>
        /// <returns>Метаданные тегов</returns>
        public TagData ExtractTagData(FileStream fileStream)
        {
            var tagData = new TagData();
            var totalFramesSize = ProcessHeader(fileStream);
            while (fileStream.Position < totalFramesSize)
            {
                var frameStart = fileStream.Position;
                (string frameName, int frameSize) = ExtractFrameHeader(fileStream);

                if (frameSize <= 0 || frameSize > totalFramesSize - fileStream.Position)
                    return tagData;

                switch (frameName)
                {
                    case "APIC":
                        tagData.AttachedPictureTagPosition = (frameSize, frameStart);
                        tagData.AttachedPicture = ExtractAttachedPicture(fileStream, frameSize);
                        break;
                    case "TIT2":
                        tagData.TitleTagPosition = (frameSize, frameStart);
                        tagData.Title = ExtractTextFrame(fileStream, frameSize);
                        break;
                    case "TPE1":
                        tagData.PerformerTagPosition = (frameSize, frameStart); 
                        tagData.Performer = ExtractTextFrame(fileStream, frameSize);
                        break;
                    case "TALB":
                        tagData.AlbumTagPosition = (frameSize, frameStart);
                        tagData.Album = ExtractTextFrame(fileStream, frameSize); 
                        break;
                    case "TYER":
                        tagData.AlbumTagPosition = (frameSize, frameStart);
                        tagData.Year = uint.Parse(ExtractTextFrame(fileStream, frameSize));
                        break;
                    default:
                        fileStream.Seek(frameSize, SeekOrigin.Current);
                        break;
                }

            }
            return tagData;
        }
        /// <summary>
        /// Проверяет содержит ли файл метаданные в формате ID3. Инициализирует version версией ID3 при успехе, иначе 0
        /// </summary>
        /// <param name="fileStream">Поток файла с тегом</param>
        /// <param name="fileStream">Поток файла с тегом</param>
        public static bool IsID3Format(FileStream fileStream, out int version)
        {
            int id3FormatSize = 3;
            Span<byte> formatBytes = stackalloc byte[id3FormatSize + 1];
            fileStream.Read(formatBytes);
            if (formatBytes[0] == 0x49 && formatBytes[1] == 0x44 && formatBytes[2] == 0x33)
            {
                version = formatBytes[3];
                return true;
            }
            version = 0;
            fileStream.Position -= id3FormatSize + 1;
            return false;
        }
    }
}
