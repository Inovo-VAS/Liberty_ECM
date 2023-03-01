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
    public class ConfigurationEmail
    {
        public int ConfigurationEmailID { get; set; }
        public string Campaign { get; set; }
        public int HourStart { get; set; }
        public int HourEnd { get; set; }
        public string Saterday { get; set; }
        public string Sunday { get; set; }
        public string Display { get; set; }
        public string From { get; set; }
        public string Server { get; set; }
        public int Port { get; set; }
        public string SSL { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Logo { get; set; }
        public string SendCopy { get; set; }
        public string SendBlindCopy { get; set; }
        public DateTime Updated { get; set; }

        public ConfigurationEmail() { }

        public async Task<List<ConfigurationEmail>> GetListAsync(string InstanceID)
        {
            try
            {
                using (var conn = new SqlConnection(Database.dbInovoCIM))
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    await conn.OpenAsync();
                    cmd.CommandTimeout = 0;
                    cmd.CommandText = "SELECT * FROM [ECM].[ConfigurationEmail]";
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        List<ConfigurationEmail> model = await new ModelRepository<ConfigurationEmail>().ConvertToList(reader);
                        return model;
                    }
                }
            }
            catch (Exception ex)
            {
                var log = new LogConsoleError(InstanceID, "ConfigurationEmail", "GetListAsync()", ex.Message);
                await log.SaveSync();

                return null;
            }
        }
    }
}
