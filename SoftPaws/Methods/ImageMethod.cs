using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace SoftPaws.Methods
{
    class ImageMethod
    {
        // Функция для проверки, является ли файл изображением
        public static bool IsImageFile(string filePath)
        {
            try
            {
                using (var imageStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    var decoder = BitmapDecoder.Create(imageStream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                    return decoder.Frames[0] != null;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
