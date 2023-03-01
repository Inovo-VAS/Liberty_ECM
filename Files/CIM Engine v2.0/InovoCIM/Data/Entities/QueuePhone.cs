#region [ Using ]
using InovoCIM.Data.Models;
using InovoCIM.Data.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace InovoCIM.Data.Entities
{
    [Serializable]
    public class QueuePhone
    {
        public int QueuePhoneID { get; set; }
        public string Command { get; set; }
        public string Input { get; set; }
        public string InputName { get; set; }
        public string Status { get; set; }
        public DateTime Received { get; set; }
        public DateTime NextExecute { get; set; }
        public DateTime? Actioned { get; set; }
        public int RetryCount { get; set; }
        public DateTime? RetryDate { get; set; }
        public int PersonID { get; set; }
        public int SourceID { get; set; }
        public int ServiceID { get; set; }
        public int LoadID { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public DateTime? ScheduleDate { get; set; }
        public int Priority { get; set; }
        public int CapturingAgent { get; set; }
        public string Phone01 { get; set; }
        public string Phone02 { get; set; }
        public string Phone03 { get; set; }
        public string Phone04 { get; set; }
        public string Phone05 { get; set; }
        public string Phone06 { get; set; }
        public string Phone07 { get; set; }
        public string Phone08 { get; set; }
        public string Phone09 { get; set; }
        public string Phone10 { get; set; }
        public string Comments { get; set; }
        public string CustomData1 { get; set; }
        public string CustomData2 { get; set; }
        public string CustomData3 { get; set; }
        public string CallerID { get; set; }
        public string CallerName { get; set; }

        public QueuePhone() { }

        public async Task<List<QueuePhone>> GetListAsync(string InstanceID)
        {
            try
            {
                string query = @"SELECT * FROM [ECM].[QueuePhone]".Replace("[ECM]",Database.dbSchema);

                using (var conn = new SqlConnection(Database.dbInovoCIM))
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    await conn.OpenAsync();
                    cmd.CommandTimeout = 0;
                    cmd.CommandText = query;
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        List<QueuePhone> model = await new ModelRepository<QueuePhone>().ConvertToList(reader);
                        return model;
                    }
                }
            }
            catch (Exception ex)
            {
                var log = new LogConsoleError(InstanceID, "QueuePhone", "GetListAsync()", ex.Message);
                await log.SaveSync();

                return null;
            }
        }
    }
}
