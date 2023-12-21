using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftPaws.Classes
{
    class GuardianItems
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public GuardianItems(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string ToString() { return Name; }
    }
}
