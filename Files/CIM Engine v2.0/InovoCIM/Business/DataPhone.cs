#region [ Using ]
using InovoCIM.Data.Entities;
using InovoCIM.Data.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
#endregion

namespace InovoCIM.Business
{
    public class DataPhone
    {
        public LogRepository LogRepo { get; set; }
        public string InstanceID { get; set; }
        public string Class { get; set; }

        #region [ Default Constructor ]
        public DataPhone(string _InstanceID)
        {
            this.LogRepo = new LogRepository();
            this.InstanceID = _InstanceID;
            this.Class = "Queue Phone";
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

                SQLRepository sql = new SQLRepository();

                QueuePhone phone = new QueuePhone();
                List<QueuePhone> phones = await phone.GetListAsync(this.InstanceID);

                TableRepository table = new TableRepository();
                DataTable outboundQ = new DataTable();
                DataTable Qphonecomplete = new DataTable();
                outboundQ = table.ColumnTablePresence(outboundQ);
                Qphonecomplete = table.ColumnTableQueuePhoneComplete(Qphonecomplete);

                foreach(var phoneq in phones)
                {
                    var inPresence = await sql.IsPresenceDuplicate(phoneq.SourceID, phoneq.ServiceID, phoneq.LoadID);
                    var isActive = await sql.IsActive(phoneq.ServiceID, phoneq.LoadID);

                    if(inPresence)
                    {
                        phoneq.Status = "Already In Presence";
                        DataRow row = table.MapQPhoneComplete(phoneq, Qphonecomplete);
                        Qphonecomplete.Rows.Add(row);
                    }
                    else
                    {
                        if(isActive)
                        {
                            phoneq.Status = "Inserted Into Presence";
                            DataRow row = table.MapPresenceQ(phoneq, outboundQ, true);
                            DataRow rowCIM = table.MapQPhoneComplete(phoneq, Qphonecomplete);
                            outboundQ.Rows.Add(row);
                            Qphonecomplete.Rows.Add(rowCIM);
                        }
                        else
                        {
                            phoneq.Status = "Inserted Into Presence";
                            DataRow row = table.MapPresenceQ(phoneq, outboundQ, true);
                            DataRow rowCIM = table.MapQPhoneComplete(phoneq, Qphonecomplete);
                            outboundQ.Rows.Add(row);
                            Qphonecomplete.Rows.Add(rowCIM);
                        }
                    }
                }

                SqlBulkRepository bulk = new SqlBulkRepository();

                bulk.SendToInovoCIM(Qphonecomplete, "[ECM].[QueuePhoneComplete]".Replace("[ECM]", InovoCIM.Data.Models.Database.dbSchema));
                bulk.SendToPresence(outboundQ, "[PREP].[PCO_OUTBOUNDQUEUE]");

                await sql.DeleteFromQPhone(phones);

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
