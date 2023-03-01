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
    public class QueueSMSReply
    {
        public int QueueSMSReplyID { get; set; }
        public string Input { get; set; }
        public string InputName { get; set; }
        public string Status { get; set; }
        public DateTime Received { get; set; }
        public DateTime NextExecute { get; set; }
        public DateTime? Actioned { get; set; }
        public int RetryCount { get; set; }
        public DateTime? RetryDate { get; set; }
        public int PersonID { get; set; }
        public string SendTo { get; set; }
        public string Message { get; set; }

        public QueueSMSReply() { }

        public async Task<List<QueueSMSReply>> GetListAsync(string InstanceID)
        {
            try
            {
                string query = @"SELECT * FROM [ECM].[QueueSMSReply]";

                using (var conn = new SqlConnection(Database.dbInovoCIM))
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    await conn.OpenAsync();
                    cmd.CommandTimeout = 0;
                    cmd.CommandText = query;
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        List<QueueSMSReply> model = await new ModelRepository<QueueSMSReply>().ConvertToList(reader);
                        return model;
                    }
                }
            }
            catch (Exception ex)
            {
                var log = new LogConsoleError(InstanceID, "QueueSMSReply", "GetListAsync()", ex.Message);
                await log.SaveSync();

                return null;
            }
        }
    }
}
