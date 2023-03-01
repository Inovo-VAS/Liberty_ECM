#region [ Using ]
using System;
using System.Data;
using System.Threading.Tasks;
using InovoCIM.Data.Entities;
using InovoCIM.Data.Repository;
#endregion

namespace InovoCIM.FileProcess
{
    public class FileFYC
    {
        public LogRepository LogRepo { get; set; }
        public string InstanceID { get; set; }
        public string Class { get; set; }

        public DataTable TableKYC = new DataTable();
        public DataTable TableKYCScript = new DataTable();

        public DataTable TablePresence = new DataTable();
        public DataTable TableQueuePhoneComplete = new DataTable();

        #region [ Default Constructor ]
        public FileFYC(string _InstanceID)
        {
            this.LogRepo = new LogRepository();
            this.InstanceID = _InstanceID;
            this.Class = "File FYC";
        }
        #endregion

        //---------------------------------------------------------------------------//

        #region [ Master ]
        public async Task<bool> Master()
        {
            DateTime StartTime = DateTime.Now;
            var Event = new LogConsoleEvent(this.InstanceID);
            try
            {
                await Event.SaveSync(this.Class, "Master()", "Start");



                await Event.SaveSync(this.Class, "Master()", "End");
                var Runtime = new LogConsoleRuntime(this.InstanceID, this.Class, "Master()", StartTime);
                await Runtime.SaveSync();

                return true;
            }
            catch (Exception ex)
            {
                var log = new LogConsoleError(this.InstanceID, this.Class, "Master()", ex.Message);
                await log.SaveSync();

                return false;
            }
        }
        #endregion

    }
}
