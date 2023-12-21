using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftPaws.Classes
{
    class StatusItems
    {
        public int Id { get; set; }
        public string Status { get; set; }

        public StatusItems(int id, string status)
        {
            Id = id;
            Status = status;
        }

        public override string ToString() { return Status; }
    }
}
