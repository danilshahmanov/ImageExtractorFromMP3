
using TagExtractorsLib.TagExtractors;

namespace TagExtractorsLib.DataModels
{

    public class TagData
    {
        public bool IsChanged = false;
        /// <summary>
        /// Исполнитель трека
        /// </summary>
        public string? Performer { get; set; }
        public (int? Size, long? Start) PerformerTagPosition { get; set; }
        /// <summary>
        /// Название трека
        /// </summary>
        public string? Title { get; set; }
        public (int? Size, long? Start) TitleTagPosition { get; set; }
        /// <summary>
        /// Год записи
        /// </summary>
        public uint? Year { get; set; }
        public (int? Size, long? Start) YearTagPosition { get; set; }
        /// <summary>
        /// Название альбома
        /// </summary>
        public string? Album { get; set; }
        public (int? Size, long? Start) AlbumTagPosition { get; set; }
        /// <summary>
        /// Прикрепленное изображение
        /// </summary>
        public Picture? AttachedPicture { get; set; }
        public (int Size, long Start)? AttachedPictureTagPosition { get; set; }
       
    }
   
}
