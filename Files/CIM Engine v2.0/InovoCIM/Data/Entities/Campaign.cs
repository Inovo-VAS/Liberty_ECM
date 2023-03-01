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
    public class Campaign
    {
        public int CampaignID { get; set; }
        public string Name { get; set; }
        public int ServiceID { get; set; }
        public string Script { get; set; }
        public int ConfigurationEmailID { get; set; }
        public int ConfigurationSMSID { get; set; }
        public DateTime Updated { get; set; }

        public Campaign() { }

        public async Task<List<Campaign>> GetListAsync(string InstanceID)
        {
            try
            {
                using (var conn = new SqlConnection(Database.dbInovoCIM))
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    await conn.OpenAsync();
                    cmd.CommandTimeout = 0;
                    cmd.CommandText = "SELECT * FROM [ECM].[Campaign]";
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        List<Campaign> model = await new ModelRepository<Campaign>().ConvertToList(reader);
                        return model;
                    }
                }
            }
            catch (Exception ex)
            {
                var log = new LogConsoleError(InstanceID, "Campaign", "GetListAsync()", ex.Message);
                await log.SaveSync();

                return null;
            }
        }
    }
}
