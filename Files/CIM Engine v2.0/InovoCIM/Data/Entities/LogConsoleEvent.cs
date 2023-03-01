#region [ Using ]
using InovoCIM.Data.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace InovoCIM.Data.Entities
{
    public class LogConsoleEvent
    {
        public string InstanceID { get; set; }
        public string Class { get; set; }
        public string Method { get; set; }
        public string Message { get; set; }
        public DateTime Updated { get; set; }

        public LogConsoleEvent() { }
        public LogConsoleEvent(string _InstanceID)
        {
            this.InstanceID = _InstanceID;
            this.Updated = DateTime.Now;
        }

        public async Task<int> SaveAsync(string _Class, string _Method, string _Message)
        {
            try
            {
                this.Class = _Class;
                this.Method = _Method;
                this.Message = _Message;

                string query = @"INSERT INTO [ECM].[LogConsoleEvent] ([InstanceID],[Class],[Method],[Message],[Updated]) VALUES (@InstanceID,@Class,@Method,@Message,@Updated)";

                using (var conn = new SqlConnection(Database.dbInovoCIM))
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddWithValue("@InstanceID", this.InstanceID);
                    cmd.Parameters.AddWithValue("@Class", this.Class);
                    cmd.Parameters.AddWithValue("@Method", this.Method);
                    cmd.Parameters.AddWithValue("@Message", this.Message);
                    cmd.Parameters.AddWithValue("@Updated", this.Updated);

                    await conn.OpenAsync().ConfigureAwait(false);
                    int ResultID = (int)await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
                    return ResultID;
                }
            }
            catch (Exception) { return -1; }
        }
    }
}
