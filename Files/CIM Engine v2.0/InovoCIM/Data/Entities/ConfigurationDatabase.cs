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
    public class ConfigurationDatabase
    {
        public int ConfigurationDatabaseID { get; set; }
        public string Name { get; set; }
        public string Connection { get; set; }
        public DateTime Updated { get; set; }

        public ConfigurationDatabase() { }

        public async Task<List<ConfigurationDatabase>> GetListAsync(string InstanceID)
        {
            try
            {
                using (var conn = new SqlConnection(Database.dbInovoCIM))
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    await conn.OpenAsync();
                    cmd.CommandTimeout = 0;
                    cmd.CommandText = "SELECT * FROM [ECM].[ConfigurationDatabase]";
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        List<ConfigurationDatabase> model = await new ModelRepository<ConfigurationDatabase>().ConvertToList(reader);
                        return model;
                    }
                }
            }
            catch (Exception ex)
            {
                var log = new LogConsoleError(InstanceID, "ConfigurationDatabase", "GetListAsync()", ex.Message);
                await log.SaveSync();

                return null;
            }
        }
    }
}
