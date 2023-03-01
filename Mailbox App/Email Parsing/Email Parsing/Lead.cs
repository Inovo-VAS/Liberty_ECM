using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Email_Parsing
{
    public class Lead
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Purpose { get; set; }
        public string Source { get; set; }
        public string IDNumber { get; set; }
        public int MailID { get; set; }

        public bool IsNaturalNumber()
        {
            try
            {
                if(Phone == null)
                {
                    return false;
                }
                if(Phone.Contains("+"))
                {
                    Phone = Phone.Replace("+", "");
                }
                Regex objNotNaturalPattern = new Regex("[^0-9]");
                Regex objNaturalPattern = new Regex("0*[1-9][0-9]*");
                return !objNotNaturalPattern.IsMatch(Phone) &&
                objNaturalPattern.IsMatch(Phone);
            }
            catch(Exception)
            {
                return false;
            }
        }
    }
}
