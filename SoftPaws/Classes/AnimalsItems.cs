using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftPaws.Classes
{
    public class AnimalsItems
    {
        public int IdAnimalsClass { get; set; }
        public string NumInSheater { get; set; }
        public string NameInSheater { get; set; }
        public string YearoldInSheater { get; set; }
        public string CategInSheater { get; set; }
        public string GenderInSheater { get; set; }
        public string VidInSheater { get; set; }
        public string StatusSheater { get; set; }
        public string HealtInSheater { get; set; }
        public string ImgInSheater { get; set; }
        // Добавляем новое свойство для хранения полного пути к изображению
        public string FullImagePath { get; set; }
        public string IsMale { get; set; }

       
    }
}
