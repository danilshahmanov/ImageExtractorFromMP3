
using System.Drawing.Imaging;
using System.Drawing;
using TagExtractorsLib;
using System.IO;

Console.WriteLine("Введите путь к файлу MP3:");
var filePath = Console.ReadLine();

if (!File.Exists(filePath))
{
    Console.WriteLine("Файл не найден.");
    return;
}

try
{
    var tagData  = TagProcessor.GetTagData(filePath);
    Console.WriteLine("Информация о треке:");
    Console.WriteLine($"Название: {tagData.Title ?? "название отсутствует"}");
    Console.WriteLine($"Исполнитель: {tagData.Performer ?? "имя исполнителя отсутствует"}");
    Console.WriteLine($"Альбом: {tagData.Album ?? "название альбома отсутствует"}");
    Console.WriteLine($"Год выпуска: {tagData.Year ?? 0}");

    // Вывод обложки альбома, если она есть
    if (tagData.AttachedPicture?.Data?.Length > 0)
    {
        Console.WriteLine("Сохранить обложку альбома? Введите путь куда сохранить и название файла (например, C:\\Images):");
        var imagePath = Console.ReadLine();
        Console.WriteLine("Введите имя для обложки:");

        var imageName = Console.ReadLine();
        var imageExtensionName = tagData.AttachedPicture.MimeType.Substring(tagData.AttachedPicture.MimeType.LastIndexOf('/') + 1).ToLower();
        ImageFormat imageFormat= ImageFormat.Jpeg;
        switch (imageExtensionName) 
        {
            case "png":
                imageFormat = ImageFormat.Png;
                break;
            case "jpg":
            case "jpeg":
                imageFormat= ImageFormat.Jpeg; 
                break;
            default:
                imageExtensionName = "jpg";
                imageFormat = ImageFormat.Jpeg;
                break;
        }


        var ms = new MemoryStream(tagData.AttachedPicture.Data);
        Console.WriteLine(tagData.AttachedPicture.Data.Length);
        Image imag = Image.FromStream(ms);
        imag.Save(imagePath+'\\'+imageName+'.'+imageExtensionName, imageFormat);
        Console.WriteLine("Обложка альбома сохранена.");
    }
    else
    {
        Console.WriteLine("Обложка альбома отсутствует");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Произошла ошибка: {ex.Message}");
}

//imagePath + '\\' + imageName, imageFormat
/*TagLib.File tagFile = TagLib.File.Create(mp3Path);

if (tagFile.Tag.Pictures.Length > 0)
{
    IPicture pic = tagFile.Tag.Pictures[0];
    byte[] imageData = pic.Data.Data;
    var ms = new MemoryStream(imageData);
    Image imag = Image.FromStream(ms);
    imag.Save("ms.jpg", ImageFormat.Jpeg);
    Console.WriteLine("Image Size: " + imageData.Length + " bytes");
}
else
{
    Console.WriteLine("No picture found in the file");
}*/

