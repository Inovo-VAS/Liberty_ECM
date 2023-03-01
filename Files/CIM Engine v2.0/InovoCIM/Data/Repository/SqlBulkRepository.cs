#region [ Using ]
using InovoCIM.Data.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
#endregion

namespace InovoCIM.Data.Repository
{
    public class SqlBulkRepository
    {
        #region [ Send To InovoCIM ]
        public int SendToInovoCIM(DataTable model, string TableName)
        {
            try
            {
                using (SqlBulkCopy SqlBulk = new SqlBulkCopy(Database.dbInovoCIM))
                {
                    SqlBulk.DestinationTableName = TableName;
                    SqlBulk.BatchSize = 5000;
                    SqlBulk.WriteToServer(model);
                }
                return 1;
            }
            catch (Exception ex)
            {
                return 0;   
            }
        }
        #endregion

        #region [ Send To Presence ]
        public int SendToPresence(DataTable model, string TableName)
        {
            try
            {
                using (SqlBulkCopy SqlBulk = new SqlBulkCopy(Database.dbPresence))
                {
                    SqlBulk.DestinationTableName = TableName;
                    SqlBulk.BatchSize = 5000;
                    SqlBulk.WriteToServer(model);
                }
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        #endregion
    }
}
