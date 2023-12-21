using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftPaws.Classes
{
    class CategoryItems
    {

        public int Id { get; set; }
        public string Cateegory { get; set; }

        public CategoryItems(int id, string cateegory)
        {
            Id = id;
            Cateegory = cateegory;
        }

        public override string ToString() { return Cateegory; }

        public static string GetGenderString(bool isMale)
        {
            return isMale ? "Мужской" : "Женский";
        }

    }
}
