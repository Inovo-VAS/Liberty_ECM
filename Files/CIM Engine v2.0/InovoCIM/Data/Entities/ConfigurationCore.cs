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
    public class ConfigurationCore
    {
        public int ConfigurationCoreID { get; set; }
        public string Area { get; set; }
        public string SubArea { get; set; }
        public string Item { get; set; }
        public string Value { get; set; }
        public DateTime Updated { get; set; }

        public ConfigurationCore() { }

        public async Task<List<ConfigurationCore>> GetListAsync(string InstanceID)
        {
            try
            {
                using (var conn = new SqlConnection(Database.dbInovoCIM))
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    await conn.OpenAsync();
                    cmd.CommandTimeout = 0;
                    cmd.CommandText = "SELECT * FROM [ECM].[ConfigurationCore]";
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        List<ConfigurationCore> model = await new ModelRepository<ConfigurationCore>().ConvertToList(reader);
                        return model;
                    }
                }
            }
            catch (Exception ex)
            {
                var log = new LogConsoleError(InstanceID, "ConfigurationCore", "GetListAsync()", ex.Message);
                await log.SaveSync();

                return null;
            }
        }


    }
}
