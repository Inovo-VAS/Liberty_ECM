using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIMWebAPI.Data.Entities
{
    public class Load
    {
        public int ServiceID { get; set; }
        public int LoadID { get; set; }
        public string LoadName { get; set; }
        public string Command { get; set; }
        public int CreateIfExists { get; set; }
        public string connStringCIM;
        public string connStringPresence;
    }
}
