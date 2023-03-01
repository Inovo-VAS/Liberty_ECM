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
    public class ConfigurationSMS
    {
        public int ConfigurationSMSID { get; set; }
        public string Campaign { get; set; }
        public int HourStart { get; set; }
        public int HourEnd { get; set; }
        public string Saterday { get; set; }
        public string Sunday { get; set; }
        public string Provider { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime Updated { get; set; }

        public ConfigurationSMS() { }

        public async Task<List<ConfigurationSMS>> GetListAsync(string InstanceID)
        {
            try
            {
                string query = @"SELECT * FROM [ECM].[ConfigurationSMS]";

                using (var conn = new SqlConnection(Database.dbInovoCIM))
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    await conn.OpenAsync();
                    cmd.CommandTimeout = 0;
                    cmd.CommandText = query;
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        List<ConfigurationSMS> model = await new ModelRepository<ConfigurationSMS>().ConvertToList(reader);
                        return model;
                    }
                }
            }
            catch (Exception ex)
            {
                var log = new LogConsoleError(InstanceID, "ConfigurationSMS", "GetListAsync()", ex.Message);
                await log.SaveSync();

                return null;
            }
        }
    }
}
