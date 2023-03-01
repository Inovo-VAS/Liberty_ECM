#region [ Using ]
using InovoCIM.Data.Models;
using InovoCIM.Data.Repository;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace InovoCIM.Data.Entities
{
    [Serializable]
    public class ImportFile
    {
        public int ImportFileID { get; set; }
        public string Name { get; set; }
        public string Partial { get; set; }
        public string FileDelimiter { get; set; }
        public char Delimiter { get; set; }
        public string Header { get; set; }

        public int QueueSourceID { get; set; }
        public int QueueServiceID { get; set; }
        public int QueueLoadID { get; set; }
        public int QueueName { get; set; }
        public int QueuePhone { get; set; }
        public int QueuePriority { get; set; }

        public int QueuePhone01 { get; set; }
        public int QueuePhone02 { get; set; }
        public int QueuePhone03 { get; set; }
        public int QueuePhone04 { get; set; }
        public int QueuePhone05 { get; set; }
        public int QueuePhone06 { get; set; }
        public int QueuePhone07 { get; set; }
        public int QueuePhone08 { get; set; }
        public int QueuePhone09 { get; set; }
        public int QueuePhone10 { get; set; }

        public int QueueComments { get; set; }
        public int QueueCustomData1 { get; set; }
        public int QueueCustomData2 { get; set; }
        public int QueueCustomData3 { get; set; }
        public int QueueCallerID { get; set; }
        public int QueueCallerName { get; set; }

        public int PersonTitle { get; set; }
        public int PersonName { get; set; }
        public int PersonSurname { get; set; }
        public int PersonIDNumber { get; set; }
        public int PersonPassport { get; set; }
        public int PersonPhone01 { get; set; }
        public int PersonPhone02 { get; set; }
        public int PersonPhone03 { get; set; }
        public int PersonPhone04 { get; set; }
        public int PersonEmail01 { get; set; }
        public int PersonEmail02 { get; set; }
        public int PersonSourceExtID { get; set; }

        public DateTime Updated { get; set; }

        public ImportFile() { }

        public async Task<List<ImportFile>> GetListAsync(string InstanceID)
        {
            try
            {
                string query = @"SELECT * FROM [ECM].[ImportFile]";

                using (var conn = new SqlConnection(Database.dbInovoCIM))
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    await conn.OpenAsync();
                    cmd.CommandTimeout = 0;
                    cmd.CommandText = query;
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        List<ImportFile> model = await new ModelRepository<ImportFile>().ConvertToList(reader);
                        return model;
                    }
                }
            }
            catch (Exception ex)
            {
                var log = new LogConsoleError(InstanceID, "ImportFile", "GetListAsync()", ex.Message);
                await log.SaveSync();

                return null;
            }
        }
    }
}
