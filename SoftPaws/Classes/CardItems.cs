using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftPaws.Classes
{
    class CardItems
    {
        public int Id { get; set; }
        public string Gender { get; set; }

        public CardItems(int id, string gender)
        {
            Id = id;
            Gender = gender;
        }

        public override string ToString() { return Gender; }
    }
}
