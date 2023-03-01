using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIMWebAPI.Data.Entities
{
    public class Priority
    {
        public int ServiceID { get; set; }
        public string PriorityCommand { get; set; }

        public Priority()
        {

        }
    }
}
