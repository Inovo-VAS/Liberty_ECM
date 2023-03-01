#region [ Using ]
using InovoCIM.Data.Models;
using InovoCIM.Data.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace InovoCIM.Data.Entities
{
    [Serializable]
    public class ConfigurationDirectory
    {
        public int ConfigurationDirectoryID { get; set; }
        public string FTP { get; set; }
        public string Input { get; set; }
        public string Complete { get; set; }
        public string Fail { get; set; }
        public string Log { get; set; }
        public DateTime Updated { get; set; }

        public ConfigurationDirectory() { }

        public async Task<List<ConfigurationDirectory>> GetListAsync(string InstanceID)
        {
            try
            {
                using (var conn = new SqlConnection(Database.dbInovoCIM))
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    await conn.OpenAsync();
                    cmd.CommandTimeout = 0;
                    cmd.CommandText = "SELECT * FROM [ECM].[ConfigurationDirectory]";
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        List<ConfigurationDirectory> model = await new ModelRepository<ConfigurationDirectory>().ConvertToList(reader);
                        return model;
                    }
                }
            }
            catch (Exception ex)
            {
                var log = new LogConsoleError(InstanceID, "ConfigurationDirectory", "GetListAsync()", ex.Message);
                await log.SaveSync();

                return null;
            }
        }

        public async Task<ConfigurationDirectory> GetSingleAsync(string InstanceID)
        {
            ConfigurationDirectory Single = new ConfigurationDirectory();
            try
            {
                using (var conn = new SqlConnection(Database.dbInovoCIM))
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    await conn.OpenAsync();
                    cmd.CommandTimeout = 0;
                    cmd.CommandText = "SELECT * FROM [ECM].[ConfigurationDirectory]";
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        List<ConfigurationDirectory> model = await new ModelRepository<ConfigurationDirectory>().ConvertToList(reader);
                        if (model.Count > 0)
                        {
                            //Single = model.Last(); // Dev
                            Single = model.Where(o => o.ConfigurationDirectoryID == 1).FirstOrDefault();
                            //Single = model.Where(o => o.ConfigurationDirectoryID == 2).FirstOrDefault();
                        }
                    }
                }
                return Single;
            }
            catch (Exception ex)
            {
                var log = new LogConsoleError(InstanceID, "ConfigurationDirectory", "GetSingleAsync()", ex.Message);
                await log.SaveSync();

                return null;
            }
        }
    }
}
