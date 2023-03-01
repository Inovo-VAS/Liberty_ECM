#region [ Using ]
using InovoCIM.Data.Entities;
using InovoCIM.Data.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace InovoCIM.Business
{
    public class MediaSMSReply
    {
        public LogRepository LogRepo { get; set; }
        public string InstanceID { get; set; }
        public string Class { get; set; }

        #region [ Default Constructor ]
        public MediaSMSReply(string _InstanceID)
        {
            this.LogRepo = new LogRepository();
            this.InstanceID = _InstanceID;
            this.Class = "Queue SMS Reply";
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
                await Event.SaveAsync(this.Class, "Master()", "Start");






                await Event.SaveAsync(this.Class, "Master()", "End");
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

        //---------------------------------------------------------------------------//
    }
}
