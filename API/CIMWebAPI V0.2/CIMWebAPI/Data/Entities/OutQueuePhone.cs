#region [ Using ]
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CIMWebAPI.DataRepository;
#endregion

namespace CIMWebAPI.Data.Entities
{
   public class OutQueuePhone
   {
        public int ServiceID { get; set; }
        public int ServiceIDOld { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public long SourceID { get; set; }
        public Nullable<int> Status { get; set; }
        public int LoadID { get; set; }
        public int LoadIDOld { get; set; }
        public int Priority { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Phone3 { get; set; }
        public string Phone4 { get; set; }
        public string Phone5 { get; set; }
        public string Phone6 { get; set; }
        public string Phone7 { get; set; }
        public string Phone8 { get; set; }
        public string Phone9 { get; set; }
        public string Phone10 { get; set; }
        public Nullable<int> PhoneDesc1 { get; set; }
        public Nullable<int> PhoneDesc2 { get; set; }
        public Nullable<int> PhoneDesc3 { get; set; }
        public Nullable<int> PhoneDesc4 { get; set; }
        public Nullable<int> PhoneDesc5 { get; set; }
        public Nullable<int> PhoneDesc6 { get; set; }
        public Nullable<int> PhoneDesc7 { get; set; }
        public Nullable<int> PhoneDesc8 { get; set; }
        public Nullable<int> PhoneDesc9 { get; set; }
        public Nullable<int> PhoneDesc10 { get; set; }
        public Nullable<int> PhoneStatus1 { get; set; }
        public Nullable<int> PhoneStatus2 { get; set; }
        public Nullable<int> PhoneStatus3 { get; set; }
        public Nullable<int> PhoneStatus4 { get; set; }
        public Nullable<int> PhoneStatus5 { get; set; }
        public Nullable<int> PhoneStatus6 { get; set; }
        public Nullable<int> PhoneStatus7 { get; set; }
        public Nullable<int> PhoneStatus8 { get; set; }
        public Nullable<int> PhoneStatus9 { get; set; }
        public Nullable<int> PhoneStatus10 { get; set; }
        public int CurrentPhone { get; set; }
        public string Comments { get; set; }
        public string CustomData1 { get; set; }
        public string CustomData2 { get; set; }
        public string CustomData3 { get; set; }
        public string CallerID { get; set; }
        public string CallerName { get; set; }
        public int BatchNumber { get; set; }
        public string Command { get; set; }
        public int CapturingAgent { get; set; }
        //public int LastAgent { get; set; }
        public Nullable<int> LastQCode { get; set; }
        public string CallingHours { get; set; }
        public int leadSource { get; set; }
        public string affinity { get; set; }
        public string leadProvider { get; set; }
        public string campaignName { get; set; }
        public Nullable<DateTime> FirstHandlingDate { get; set; }
        public Nullable<DateTime> LastHandlingDate { get; set; }
        public Nullable<DateTime> ScheduleDate { get; set; }
        public string LoadDescription { get; set; }
        public string connStringCIM;
        public string connStringPresence;

        public bool PresenceDuplicate(bool isDev)
        {
            OutQueuePhone newPhone = new OutQueuePhone();
            if(BatchNumber != 0)
            {
                newPhone = this.GetBatch(isDev).GetAwaiter().GetResult();
                this.LoadID = newPhone.LoadID;
            }
            SQLRepository sql = new SQLRepository();
            return sql.PresenceDuplicate(this.SourceID, this.LoadID, this.ServiceID, connStringPresence);
        }
        public async Task<OutQueuePhone> GetBatch(bool isDev)
        {
            try
            {
                if (!String.IsNullOrEmpty(this.leadProvider))
                {
                    //if(this.Phone == "" && this.Phone1 != "")
                    //{
                    //    this.Phone = this.Phone1;
                    //}
                    SQLRepository sql = new SQLRepository();
                    return await sql.QueryBatchService(this, isDev);
                }
                else
                {
                    return this;
                }
            }
            catch(Exception ex)
            {
                return new OutQueuePhone();
            }

        }



    }
}
