#region [ Using ]
using InovoCIM.Data.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
#endregion

namespace InovoCIM.Data.Repository
{
    public class LogRepository
    {
        #region [ API Log Exception ]
        private void APILogException(string InstanceID, string ClassName, string Method, string Error)
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.Append(string.Format("DECLARE @InstanceID VARCHAR(50) = '{0}' ", InstanceID));
                query.Append(string.Format("DECLARE @Class VARCHAR(50) = '{0}' ", ClassName));
                query.Append(string.Format("DECLARE @Method VARCHAR(50) = '{0}' ", Method));
                query.Append(string.Format("DECLARE @Error VARCHAR(MAX) = '{0}' ", Error.Replace("\"", "").Replace("\'", "")));
                query.Append(string.Format("DECLARE @Received DATETIME = GETDATE() "));

                string InsertQuery = query.ToString() + " " + @"INSERT INTO [ECM].[APILogException] ([InstanceID],[Class],[Method],[Error],[Received]) VALUES (@InstanceID,@Class,@Method,@Error,@Received)";

                using (var conn = new SqlConnection(Database.dbInovoCIM))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandText = InsertQuery;
                        cmd.ExecuteNonQuery();
                    }
                    conn.Close();
                }
            }
            catch { }
        }
        #endregion
    }
}
