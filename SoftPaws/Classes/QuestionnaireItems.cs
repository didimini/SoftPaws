using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftPaws.Classes
{
    class QuestionnaireItems
    {
        public int IdInt { get; set; }
        public string NumQue { get; set; }
        public string CardQue { get; set; }
        public string GuardianQue { get; set; }
        public string AdresQue { get; set; }
        public string PhoneQue { get; set; }
        public string DateDischQue { get; set; }
        public string ImgInSheater { get; set; }
        // Добавляем новое свойство для хранения полного пути к изображению
        public string FullImagePath { get; set; }

    }
}
