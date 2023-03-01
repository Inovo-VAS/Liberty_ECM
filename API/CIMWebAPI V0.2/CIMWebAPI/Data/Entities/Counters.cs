using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CIMWebAPI.Data.Entities
{
    public class Counters
    {
        public int? DAILYCOUNTER { get; set; }
        public int? TOTALCOUNTER { get; set; }
        public int? BUSYSIGNALCOUNTER { get; set; }
        public int? NOANSWERCOUNTER { get; set; }
        public int? ANSWERMACHINECOUNTER { get; set; }
        public int? FAXCOUNTER { get; set; }
        public int? INVGENREASONCOUNTER { get; set; }

        public Counters() { }

        public Counters(DataRow inRow)
        {

            if(inRow["DAILYCOUNTER"] != DBNull.Value)
            {
                this.DAILYCOUNTER = Convert.ToInt32(inRow["DAILYCOUNTER"]);
            }
            else
            {
                this.DAILYCOUNTER = null;
            }
            if(inRow["TOTALCOUNTER"] != DBNull.Value)
            {
                this.TOTALCOUNTER = Convert.ToInt32(inRow["TOTALCOUNTER"]);
            }
            else
            {
                this.TOTALCOUNTER = null;
            }
            if(inRow["BUSYSIGNALCOUNTER"] != DBNull.Value)
            {
                this.BUSYSIGNALCOUNTER = Convert.ToInt32(inRow["BUSYSIGNALCOUNTER"]);
            }
            else
            {
                this.BUSYSIGNALCOUNTER = null;
            }
            if(inRow["NOANSWERCOUNTER"] != DBNull.Value)
            {
                this.NOANSWERCOUNTER = Convert.ToInt32(inRow["NOANSWERCOUNTER"]);
            }
            else
            {
                this.NOANSWERCOUNTER = null;
            }
            if(inRow["ANSWERMACHINECOUNTER"] != DBNull.Value)
            {
                this.ANSWERMACHINECOUNTER = Convert.ToInt32(inRow["ANSWERMACHINECOUNTER"]);
            }
            else
            {
                this.ANSWERMACHINECOUNTER = null;
            }
            if(inRow["FAXCOUNTER"] != DBNull.Value)
            {
                this.FAXCOUNTER = Convert.ToInt32(inRow["FAXCOUNTER"]);
            }
            else
            {
                this.FAXCOUNTER = null;
            }
            if(inRow["INVGENREASONCOUNTER"] != DBNull.Value)
            {
                this.INVGENREASONCOUNTER = Convert.ToInt32(inRow["INVGENREASONCOUNTER"]);
            }
            else
            {
                this.INVGENREASONCOUNTER = null;
            }
        }
    }
}
