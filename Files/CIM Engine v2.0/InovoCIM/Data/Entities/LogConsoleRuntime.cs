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
    public class LogConsoleRuntime
    {
        public string InstanceID { get; set; }
        public string Class { get; set; }
        public string Method { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Runtime { get; set; }

        public LogConsoleRuntime() { }
        public LogConsoleRuntime(string _InstanceID, string _Class, string _Method, DateTime _StartTime)
        {
            this.InstanceID = _InstanceID;
            this.Class = _Class;
            this.Method = _Method;
            this.StartTime = _StartTime;
            this.EndTime = DateTime.Now;
        }

        public async Task<int> SaveSync()
        {
            try
            {
                TimeSpan runtime = (this.EndTime - this.StartTime);
                this.Runtime = runtime.ToString();

                string query = @"INSERT INTO [ECM].[LogConsoleRuntime] ([InstanceID],[Class],[Method],[StartTime],[EndTime],[Runtime]) VALUES (@InstanceID,@Class,@Method,@StartTime,@EndTime,@Runtime)";

                using (var conn = new SqlConnection(Database.dbInovoCIM))
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddWithValue("@InstanceID", this.InstanceID);
                    cmd.Parameters.AddWithValue("@Class", this.Class);
                    cmd.Parameters.AddWithValue("@Method", this.Method);
                    cmd.Parameters.AddWithValue("@StartTime", this.StartTime);
                    cmd.Parameters.AddWithValue("@EndTime", this.EndTime);
                    cmd.Parameters.AddWithValue("@Runtime", this.Runtime);

                    await conn.OpenAsync().ConfigureAwait(false);
                    int ResultID = (int)await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
                    return ResultID;
                }
            }
            catch (Exception) { return -1; }
        }
    }
}
