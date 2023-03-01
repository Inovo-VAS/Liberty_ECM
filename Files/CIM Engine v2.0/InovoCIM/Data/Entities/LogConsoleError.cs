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
    public class LogConsoleError
    {
        public string InstanceID { get; set; }
        public string Class { get; set; }
        public string Method { get; set; }
        public string Error { get; set; }
        public DateTime Updated { get; set; }

        public LogConsoleError() { }
        public LogConsoleError(string InstanceID, string Class, string Method, string Error)
        {
            this.InstanceID = InstanceID;
            this.Class = Class;
            this.Method = Method;
            this.Error = Error;
            this.Updated = DateTime.Now;
        }

        public async Task<int> SaveSync()
        {
            try
            {
                string query = @"INSERT INTO [ECM].[LogConsoleError] ([InstanceID],[Class],[Method],[Error],[Updated]) VALUES (@InstanceID,@Class,@Method,@Error,@Updated)";

                using (var conn = new SqlConnection(Database.dbInovoCIM))
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddWithValue("@InstanceID", this.InstanceID);
                    cmd.Parameters.AddWithValue("@Class", this.Class);
                    cmd.Parameters.AddWithValue("@Method", this.Method);
                    cmd.Parameters.AddWithValue("@Error", this.Error);
                    cmd.Parameters.AddWithValue("@Updated", this.Updated);

                    await conn.OpenAsync().ConfigureAwait(false);
                    int ResultID = (int)await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
                    return ResultID;
                }
            }
            catch (Exception ex) {
                string exc = ex.ToString();
                return -1; }
        }
    }
}
