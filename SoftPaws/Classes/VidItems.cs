using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftPaws.Classes
{
    class VidItems
    {
        public int Id { get; set; }
        public string Vid { get; set; }

        public VidItems(int id, string vid)
        {
            Id = id;
            Vid = vid;
        }

        public override string ToString() { return Vid; }
    }
}
