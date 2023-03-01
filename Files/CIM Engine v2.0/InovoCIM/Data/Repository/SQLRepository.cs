#region [ Using ]
using InovoCIM.Data.Entities;
using InovoCIM.Data.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace InovoCIM.Data.Repository
{
    public class SQLRepository
    {
        #region [ Is Presence Duplicate ]
        public async Task<bool> IsPresenceDuplicate(int SourceID, int ServiceID, int LoadID)
        {
            DataTable TempData = new DataTable();
            string query = @"SELECT 1 AS [ID] FROM [PREP].[PCO_OUTBOUNDQUEUE] WHERE [SOURCEID] = " + SourceID.ToString() + " AND [SERVICEID] = " + ServiceID.ToString() + " AND [LOADID] = " + LoadID.ToString();

            try
            {
                using (var conn = new SqlConnection(Database.dbPresence))
                {
                    await conn.OpenAsync();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandText = query;
                        SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        TempData.Load(reader);
                    }
                    conn.Close();
                }
                return (TempData.Rows.Count >= 1) ? true : false;
            }
            catch (Exception)
            {
                return true;
            }
        }
        #endregion

        #region [ Is Active ]
        public async Task<bool> IsActive(int ServiceID, int LoadID)
        {
            DataTable TempData = new DataTable();
            string query = @"SELECT
	                            *
                            FROM [PREP].[PCO_OUTBOUNDSERVICE] S
                            INNER JOIN
                            [PREP].[PCO_LOAD] L
                            ON
                            L.SERVICEID = S.ID
                            WHERE S.ID = " + ServiceID.ToString() + @" AND L.LOADID = " + LoadID.ToString() + @" 
                            AND S.STATUS = 'E' AND L.STATUS = 'E'";

            try
            {
                using (var conn = new SqlConnection(Database.dbPresence))
                {
                    await conn.OpenAsync();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandText = query;
                        SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        TempData.Load(reader);
                    }
                    conn.Close();
                }
                return (TempData.Rows.Count >= 1) ? true : false;
            }
            catch (Exception)
            {
                return true;
            }
        }
        #endregion

        #region [ Delete From Queue Phone ]
        public async Task<bool> DeleteFromQPhone(List<QueuePhone> inIDs)
        {
            try
            {
                if (inIDs.Count > 0)
                {
                    using (var conn = new SqlConnection(Database.dbInovoCIM))
                    {
                        await conn.OpenAsync();
                        foreach (var item in inIDs)
                        {
                            string query = "DELETE FROM [ECM].[QueuePhone] WHERE QueuePhoneID = @inID".Replace("[ECM]",Database.dbSchema);
                            query = query.Replace("@inID", item.QueuePhoneID.ToString());
                            using (SqlCommand cmd = conn.CreateCommand())
                            {
                                cmd.CommandTimeout = 0;
                                cmd.CommandText = query;
                                cmd.ExecuteNonQuery();
                            }
                        }
                        conn.Close();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region [ Is Script Duplicate ]
        public async Task<bool> IsScriptDuplicate(int SourceID, string Table)
        {
            DataTable TempData = new DataTable();
            string query = @"SELECT 1 AS [ID] FROM " + Table + " WHERE [ID] = " + SourceID.ToString();

            try
            {
                using (var conn = new SqlConnection(Database.dbPresence))
                {
                    await conn.OpenAsync();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandText = query;
                        SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        TempData.Load(reader);
                    }
                    conn.Close();
                }
                return (TempData.Rows.Count >= 1) ? true : false;
            }
            catch (Exception)
            {
                return true;
            }
        }
        #endregion
    }
}
