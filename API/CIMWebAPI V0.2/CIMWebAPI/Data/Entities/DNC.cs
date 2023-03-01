using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CIMWebAPI.Data.Entities
{
    public class DNC
    {
        public string Phone { get; set; }
        public string DNCListID { get; set; }
        public string DateAdded { get; set; }
        public string DNCListPendingVerify { get; set; }
        public string DNCListStatus { get; set; }
        public int AddIfNotExists { get; set; }
        public string DNCListDescription { get; set; }
        public string DNCListServiceID { get; set; }
        public string DNCListServiceName { get; set; }
        public string DNCListServiceType { get; set; }
        private string connStringPresence;
        private string connStringCIM;
        public string GetPresConnString()
        {
            return connStringPresence;
        }
        public string GetCIMConnString()
        {
            return connStringCIM;
        }
        public void SetPresConnString(string inConnStringPres)
        {
            this.connStringPresence = inConnStringPres;
        }
        public void SetCIMConnString(string inConnStringCIM)
        {
            this.connStringCIM = inConnStringCIM;
        }
    }
}
