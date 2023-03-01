using CIMWebAPI.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using CIMWebAPI.Data.Entities;
using Newtonsoft.Json;

namespace CIMWebAPI.DataRepository
{
    public class SQLRepository
    {
        public string ConnStringPresence { get; set; }
        public string ConnStringCIM { get; set; }

        public bool PresenceDuplicate(long SourceID, int LoadID, int ServiceID, string connString)
        {
            string query = @"SELECT TOP (1) * FROM [PREP].[PCO_OUTBOUNDQUEUE] WHERE [SOURCEID] = " + SourceID.ToString() + " AND [SERVICEID] = " + ServiceID.ToString() + " AND [LOADID] = " + LoadID.ToString();
            try
            {
                DataTable temp = new DataTable();
                using (var conn = new SqlConnection(connString))
                {
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        conn.Open();
                        cmd.CommandTimeout = 0;
                        cmd.CommandText = query;
                        SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        temp.Load(reader);
                    }
                    conn.Close();
                }
                return (temp.Rows.Count >= 1) ? false : true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> SetServicePriority(Priority data)
        {
            try
            {
                string query = @"EXEC [ECM].[sp_SetServicePriority] @ServiceID = @inServiceID, @Command = @inCommand";
                using(var conn = new SqlConnection(ConnStringCIM))
                {
                    conn.Open();
                    using(SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandTimeout = 0; 
                        cmd.CommandText = query;
                        cmd.Parameters.AddWithValue("@inServiceID", data.ServiceID);
                        cmd.Parameters.AddWithValue("@inCommand", data.PriorityCommand);
                        await cmd.ExecuteNonQueryAsync();
                    }
                    conn.Close();
                }
                return true;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        //public async Task<string> GetServicePriority(int ServiceID, string inConnStringCIM)
        //{
        //    try
        //    {
        //        string outPriority = String.Empty;
        //        string query = @"EXEC [ECM].[sp_GetServicePriority] @ServiceID = @inServiceID";
        //        using (var conn = new SqlConnection(inConnStringCIM))
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd = conn.CreateCommand())
        //            {
        //                cmd.CommandTimeout = 10;
        //                cmd.CommandText = query;
        //                cmd.Parameters.AddWithValue("@inServiceID", ServiceID);
        //                DataTable dataTable = new DataTable();
        //                SqlDataReader reader = cmd.ExecuteReader();
        //                dataTable.Load(reader);
        //                outPriority = dataTable.Rows[0]["PrioritySetting"].ToString().ToUpper();
        //            }
        //            conn.Close();
        //        }
        //        return outPriority;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public bool TestConnection(OutQueuePhone InData)
        {
            try
            {
                using (var conn = new SqlConnection(InData.connStringPresence))
                {
                    conn.Open();
                    conn.Close();
                }
                return true;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public bool CheckLoad(OutQueuePhone InData)
        {
            try
            {
                bool toReturn = true;
                DataTable result = new DataTable();
                using (var conn = new SqlConnection(InData.connStringPresence))
                {
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        conn.Open();
                        string query = @"SELECT STATUS FROM PREP.PCO_LOAD WHERE SERVICEID = @ServiceID AND LOADID = @LoadID";
                        cmd.CommandText = query;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.AddWithValue("@ServiceID", InData.ServiceID);
                        cmd.Parameters.AddWithValue("@LoadID", InData.LoadID);
                        SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        result.Load(reader);
                    }
                    conn.Close();
                }
                try
                {
                    if(result.Rows.Count == 0)
                    {
                        return false;
                    }
                    else
                    {
                        if (result.Rows[0]["STATUS"].ToString() == "E")
                        {
                            return true;
                        }
                        else if (result.Rows[0]["STATUS"].ToString() == "D")
                        {
                            return false;
                        }
                    }
                }
                catch(Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    toReturn = false;
                }
                return toReturn;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<OutQueuePhone> QueryBatchService(OutQueuePhone InData, bool isDev)
        {
            try
            {
                var date = DateTime.Now.Date;
                DateTime firstMonthDay = new DateTime(date.Year, date.Month, 1);
                DateTime firstMonthMonday = firstMonthDay.AddDays((DayOfWeek.Monday + 7 - firstMonthDay.DayOfWeek) % 7);
                if (firstMonthMonday > date)
                {
                    firstMonthDay = firstMonthDay.AddMonths(-1);
                    firstMonthMonday = firstMonthDay.AddDays((DayOfWeek.Monday + 7 - firstMonthDay.DayOfWeek) % 7);
                }
                int Batch = (date - firstMonthMonday).Days / 7 + 1;
                InData.BatchNumber = Batch; 
                DataTable result = new DataTable();
                using (var conn = new SqlConnection(InData.connStringCIM))
                {
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        conn.Open();
                        if(isDev)
                        {
                            cmd.CommandText = @"SELECT 
	                                                COUNT(*) AS CountBatch 
                                                FROM ECM.APITableDev 
                                                WHERE 
	                                                DATEPART(YEAR, Received) = DATEPART(YEAR, GETDATE()) 
                                                AND 
	                                                DATEPART(MONTH, Received) = DATEPART(MONTH, GETDATE()) 
                                                AND 
	                                                Batch = @BatchNumber 
                                                AND 
	                                                ServiceID = @ServiceID 
                                                AND 
	                                                LeadProvider = @LeadProvider";
                        }
                        else if(!isDev)
                        {
                            cmd.CommandText = @"SELECT 
	                                                COUNT(*) AS CountBatch 
                                                FROM ECM.APITable
                                                WHERE 
	                                                DATEPART(YEAR, Received) = DATEPART(YEAR, GETDATE()) 
                                                AND 
	                                                DATEPART(MONTH, Received) = DATEPART(MONTH, GETDATE()) 
                                                AND 
	                                                Batch = @BatchNumber 
                                                AND 
	                                                ServiceID = @ServiceID 
                                                AND 
	                                                LeadProvider = @LeadProvider";
                        }
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.AddWithValue("@BatchNumber", Batch);
                        cmd.Parameters.AddWithValue("@LeadProvider", InData.leadProvider);
                        cmd.Parameters.AddWithValue("@ServiceID", InData.ServiceID);
                        SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        result.Load(reader);
                    }
                    conn.Close();
                }
                if (Convert.ToInt32(result.Rows[0]["CountBatch"]) > 0)
                {
                    InData = await GetLastLoadID(InData, isDev);
                }
                else if (Convert.ToInt32(result.Rows[0]["CountBatch"]) == 0)
                {
                    InData = await GetNextLoadID(InData);
                }
                return InData;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<OutQueuePhone> GetNextLoadID(OutQueuePhone InData)
        {
            try
            {
                DataTable result = new DataTable();
                using (var conn = new SqlConnection(InData.connStringPresence))
                {
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        conn.Open();
                        cmd.CommandText = @"DECLARE @LoadID INT
                                            SET @LoadID = (SELECT TOP 1 LOADID FROM [PREP].[PCO_LOAD] WHERE SERVICEID = @ServiceID ORDER BY LOADID DESC)
                                            SELECT COALESCE(@LoadID, 0) AS LOADID";
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.AddWithValue("@ServiceID", InData.ServiceID);
                        SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        result.Load(reader);
                    }
                    conn.Close();
                }
                InData.LoadID = Convert.ToInt32(result.Rows[0]["LOADID"]) + 1;
                await CreateNewLoad(InData);
                return InData;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Load> GetNextLoadID(Load InData)
        {
            try
            {
                DataTable result = new DataTable();
                using (var conn = new SqlConnection(InData.connStringPresence))
                {
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        conn.Open();
                        cmd.CommandText = @"DECLARE @LoadID INT
                                            SET @LoadID = (SELECT TOP 1 LOADID FROM [PREP].[PCO_LOAD] WHERE SERVICEID = @ServiceID ORDER BY LOADID DESC)
                                            SELECT COALESCE(@LoadID, 0) AS LOADID";
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.AddWithValue("@ServiceID", InData.ServiceID);
                        SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        result.Load(reader);
                    }
                    conn.Close();
                }
                InData.LoadID = Convert.ToInt32(result.Rows[0]["LOADID"]) + 1;
                return InData;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> CheckLoad(Load inData)
        {
            try
            {
                using (var conn = new SqlConnection(inData.connStringPresence))
                {
                    DataTable tempTable = new DataTable();
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandText = @"SELECT COUNT(*) AS AMOUNTLOAD FROM PREP.PCO_LOAD
                                            WHERE SERVICEID = @ServiceID AND LOADID = @LoadID";
                        cmd.Parameters.AddWithValue("@ServiceID", inData.ServiceID);
                        cmd.Parameters.AddWithValue("@LoadID", inData.LoadID);
                        SqlDataReader readerTemp = cmd.ExecuteReader(CommandBehavior.SingleRow);
                        tempTable.Load(readerTemp);
                    }
                    if (Convert.ToInt32(tempTable.Rows[0]["AMOUNTLOAD"]) == 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task ApplyLIFO(OutQueuePhone phoneIn)
        {
            try
            {
                string SqlQuery = @"UPDATE [PREP].[PCO_OUTBOUNDQUEUE]
                                    SET PRIORITY = PRIORITY + 1
                                        WHERE SERVICEID = @ServiceID
                                        AND PRIORITY >= 5000
                                        AND STATUS IN (1,41)";
                using (SqlConnection conn = new SqlConnection(phoneIn.connStringPresence))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandText = SqlQuery;
                        cmd.Parameters.AddWithValue("@ServiceID", phoneIn.ServiceID);
                        cmd.ExecuteNonQuery();
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> FIFOPriority(OutQueuePhone phoneIn)
        {
            int newPriority;
            string SqlQuery = @"SELECT 
									TOP 1  ( PRIORITY + 1 ) AS 'PRIORITY'
                                FROM PREP.PCO_OUTBOUNDQUEUE
                                WHERE SERVICEID = @InServiceID
								ORDER BY PRIORITY DESC";
            using (SqlConnection conn = new SqlConnection(phoneIn.connStringPresence))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandTimeout = 0;
                    cmd.CommandText = SqlQuery;
                    cmd.Parameters.AddWithValue("@InServiceID", phoneIn.ServiceID);
                    DataTable storeResult = new DataTable();
                    SqlDataReader reader = cmd.ExecuteReader();
                    storeResult.Load(reader);
                    newPriority = Convert.ToInt32(storeResult.Rows[0]["PRIORITY"]);
                }
                conn.Close();
            }
            return newPriority;
        }

        public async Task<string> FirstInFirstOut(OutQueuePhone phoneIn)
        {
            try
            {
                string SqlQuery = @"WITH cte AS
                                    (
                                    SELECT 
                                        SOURCEID, 
                                        SERVICEID, 
                                        LOADID, 
                                        ROW_NUMBER() OVER (ORDER BY RDATE ASC) AS RowNumber
                                    FROM PREP.PCO_OUTBOUNDQUEUE
                                    WHERE SERVICEID = @ServiceID AND LOADID = @LoadID
                                    )
                                    UPDATE PREP.PCO_OUTBOUNDQUEUE 
                                    SET PRIORITY = b.RowNumber
                                        FROM PREP.PCO_OUTBOUNDQUEUE a 
                                        LEFT JOIN cte b 
                                            ON a.SOURCEID = b.SOURCEID 
                                            AND a.SERVICEID = b.SERVICEID 
                                            AND a.LOADID = b.LOADID
                                    WHERE a.SOURCEID = b.SOURCEID 
                                        AND a.SERVICEID = b.SERVICEID 
                                        AND a.LOADID = b.LOADID";
                using (SqlConnection conn = new SqlConnection(phoneIn.connStringPresence))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandText = SqlQuery;
                        cmd.Parameters.AddWithValue("@ServiceID", phoneIn.ServiceID);
                        cmd.Parameters.AddWithValue("@LoadID", phoneIn.LoadID);
                        cmd.ExecuteNonQuery();
                    }
                    conn.Close();
                }
                return String.Empty;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task AlterLoad(Load InData)
        {
            try
            {
                using (var conn = new SqlConnection(InData.connStringPresence))
                {
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        conn.Open();
                        cmd.CommandText = @"UPDATE PREP.PCO_LOAD
                                            SET DESCRIPTION = @Description,
                                            RDATE = GETDATE()
                                            WHERE SERVICEID = @ServiceID
                                            AND LOADID = @LoadID";
                        cmd.Parameters.AddWithValue("@Description", InData.LoadName);
                        cmd.Parameters.AddWithValue("@ServiceID", InData.ServiceID);
                        cmd.Parameters.AddWithValue("@LoadID", InData.LoadID);
                        cmd.CommandTimeout = 0;
                        cmd.ExecuteNonQuery();
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task CreateNewLoad(OutQueuePhone InData)
        {
            try
            {
                using (var conn = new SqlConnection(InData.connStringPresence))
                {
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        conn.Open();
                        cmd.CommandText = @"INSERT INTO [PREP].[PCO_LOAD]
                                            (
                                                [SERVICEID]
                                                ,[LOADID]
                                                ,[STATUS]
                                                ,[DESCRIPTION]
                                                ,[RDATE]
                                            )
                                            VALUES
                                            (
                                                @ServiceID
                                                ,@LoadID
                                                ,'D'
                                                ,@Description
                                                ,GETDATE()
                                            )";
                        cmd.Parameters.AddWithValue("@ServiceID", InData.ServiceID);
                        cmd.Parameters.AddWithValue("@LoadID", InData.LoadID);
                        cmd.CommandTimeout = 0;
                        DataTable result = new DataTable();
                        var date = DateTime.Now.Date;
                        DateTime firstMonthDay = new DateTime(date.Year, date.Month, 1);
                        DateTime firstMonthMonday = firstMonthDay.AddDays((DayOfWeek.Monday + 7 - firstMonthDay.DayOfWeek) % 7);
                        if (firstMonthMonday > date)
                        {
                            firstMonthDay = firstMonthDay.AddMonths(-1);
                            firstMonthMonday = firstMonthDay.AddDays((DayOfWeek.Monday + 7 - firstMonthDay.DayOfWeek) % 7);
                        }
                        int Batch = (date - firstMonthMonday).Days / 7 + 1;
                        if (InData.LoadDescription == String.Empty || InData.LoadDescription == null)
                        {
                            InData.LoadDescription = "CIMAPILoad" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + " Week - " + Batch.ToString() + "; - Lead Provider: " + InData.leadProvider;
                        }
                        cmd.Parameters.AddWithValue("@Description", InData.LoadDescription);
                        cmd.ExecuteNonQuery();
                    }
                    conn.Close();
                }
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task CreateNewLoad(Load InData)
        {
            try
            {
                using (var conn = new SqlConnection(InData.connStringPresence))
                {
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        conn.Open();
                        cmd.CommandTimeout = 0;
                        cmd.CommandText = @"INSERT INTO [PREP].[PCO_LOAD]
                                            (
                                                [SERVICEID]
                                                ,[LOADID]
                                                ,[STATUS]
                                                ,[DESCRIPTION]
                                                ,[RDATE]
                                            )
                                            VALUES
                                            (
                                                @ServiceID
                                                ,@LoadID
                                                ,'D'
                                                ,@Description
                                                ,GETDATE()
                                            )";
                        cmd.Parameters.AddWithValue("@ServiceID", InData.ServiceID);
                        cmd.Parameters.AddWithValue("@LoadID", InData.LoadID);
                        if (InData.LoadName == String.Empty)
                        {
                            InData.LoadName = "CIMAPILoad" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString();
                        }
                        cmd.Parameters.AddWithValue("@Description", InData.LoadName);
                        cmd.ExecuteNonQuery();
                    }
                    conn.Close();
                }
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<OutQueuePhone> GetPresenceContactData(OutQueuePhone InData)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(InData.connStringPresence))
                {
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        DataTable tempTable = new DataTable();
                        cmd.CommandText = @"SELECT 
                                                a.*,
                                                b.DESCRIPTION
                                            FROM PREP.PCO_OUTBOUNDQUEUE a 
                                            LEFT JOIN PREP.PCO_LOAD b 
                                                ON a.SERVICEID = b.SERVICEID 
                                                AND a.LOADID = b.LOADID 
                                            WHERE a.SOURCEID = @SourceID 
                                                AND a.SERVICEID = @ServiceID 
                                                AND a.LOADID = @LoadID";
                        cmd.Parameters.AddWithValue("@SourceID", InData.SourceID);
                        cmd.Parameters.AddWithValue("@ServiceID", InData.ServiceID);
                        cmd.Parameters.AddWithValue("@LoadID", InData.LoadID);
                        cmd.CommandTimeout = 0;
                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                        tempTable.Load(reader);
                        foreach (DataRow row in tempTable.Rows)
                        {
                            #region [ Mapping ]
                            if (row["NAME"] != DBNull.Value)
                            {
                                InData.Name = row["NAME"].ToString();
                            }
                            if (row["PHONE"] != DBNull.Value)
                            {
                                InData.Phone = row["PHONE"].ToString();
                            }
                            if (row["STATUS"] != DBNull.Value)
                            {
                                InData.Status = Convert.ToInt32(row["STATUS"]);
                            }
                            //if (row["LASTAGENT"] != DBNull.Value)
                            //{
                            //    InData.LastAgent = Convert.ToInt32(row["LASTAGENT"]);
                            //}
                            if (row["CAPTURINGAGENT"] != DBNull.Value)
                            {
                                InData.CapturingAgent = Convert.ToInt32(row["CAPTURINGAGENT"]);
                            }
                            if (row["DESCRIPTION"] != DBNull.Value)
                            {
                                InData.LoadDescription = Convert.ToString(row["DESCRIPTION"]);
                            }
                            if (row["LASTQCODE"] != DBNull.Value)
                            {
                                InData.LastQCode = Convert.ToInt32(row["LASTQCODE"]);
                            }
                            if (row["FIRSTHANDLINGDATE"] != DBNull.Value)
                            {
                                InData.FirstHandlingDate = Convert.ToDateTime(row["FIRSTHANDLINGDATE"]);
                            }
                            if (row["LASTHANDLINGDATE"] != DBNull.Value)
                            {
                                InData.LastHandlingDate = Convert.ToDateTime(row["LASTHANDLINGDATE"]);
                            }
                            if (row["PRIORITY"] != DBNull.Value)
                            {
                                InData.Priority = Convert.ToInt32(row["PRIORITY"]);
                            }
                            if (row["PHONE1"] != DBNull.Value)
                            {
                                InData.Phone1 = row["PHONE1"].ToString();
                            }
                            if (row["PHONE2"] != DBNull.Value)
                            {
                                InData.Phone2 = row["PHONE2"].ToString();
                            }
                            if (row["PHONE3"] != DBNull.Value)
                            {
                                InData.Phone3 = row["PHONE3"].ToString();
                            }
                            if (row["PHONE4"] != DBNull.Value)
                            {
                                InData.Phone4 = row["PHONE4"].ToString();
                            }
                            if (row["PHONE5"] != DBNull.Value)
                            {
                                InData.Phone5 = row["PHONE5"].ToString();
                            }
                            if (row["PHONE6"] != DBNull.Value)
                            {
                                InData.Phone6 = row["PHONE6"].ToString();
                            }
                            if (row["PHONE7"] != DBNull.Value)
                            {
                                InData.Phone7 = row["PHONE7"].ToString();
                            }
                            if (row["PHONE8"] != DBNull.Value)
                            {
                                InData.Phone8 = row["PHONE8"].ToString();
                            }
                            if (row["PHONE9"] != DBNull.Value)
                            {
                                InData.Phone9 = row["PHONE9"].ToString();
                            }
                            if (row["PHONE10"] != DBNull.Value)
                            {
                                InData.Phone10 = row["PHONE10"].ToString();
                            }
                            if (row["PHONEDESC1"] != DBNull.Value)
                            {
                                InData.PhoneDesc1 = Convert.ToInt32(row["PHONEDESC1"]);
                            }
                            if (row["PHONEDESC2"] != DBNull.Value)
                            {
                                InData.PhoneDesc2 = Convert.ToInt32(row["PHONEDESC2"]);
                            }
                            if (row["PHONEDESC3"] != DBNull.Value)
                            {
                                InData.PhoneDesc3 = Convert.ToInt32(row["PHONEDESC3"]);
                            }
                            if (row["PHONEDESC4"] != DBNull.Value)
                            {
                                InData.PhoneDesc4 = Convert.ToInt32(row["PHONEDESC4"]);
                            }
                            if (row["PHONEDESC5"] != DBNull.Value)
                            {
                                InData.PhoneDesc5 = Convert.ToInt32(row["PHONEDESC5"]);
                            }
                            if (row["PHONEDESC6"] != DBNull.Value)
                            {
                                InData.PhoneDesc6 = Convert.ToInt32(row["PHONEDESC6"]);
                            }
                            if (row["PHONEDESC7"] != DBNull.Value)
                            {
                                InData.PhoneDesc7 = Convert.ToInt32(row["PHONEDESC7"]);
                            }
                            if (row["PHONEDESC8"] != DBNull.Value)
                            {
                                InData.PhoneDesc8 = Convert.ToInt32(row["PHONEDESC8"]);
                            }
                            if (row["PHONEDESC9"] != DBNull.Value)
                            {
                                InData.PhoneDesc9 = Convert.ToInt32(row["PHONEDESC9"]);
                            }
                            if (row["PHONEDESC10"] != DBNull.Value)
                            {
                                InData.PhoneDesc10 = Convert.ToInt32(row["PHONEDESC10"]);
                            }
                            if (row["PHONESTATUS1"] != DBNull.Value)
                            {
                                InData.PhoneStatus1 = Convert.ToInt32(row["PHONESTATUS1"]);
                            }
                            if (row["PHONESTATUS2"] != DBNull.Value)
                            {
                                InData.PhoneStatus2 = Convert.ToInt32(row["PHONESTATUS2"]);
                            }
                            if (row["PHONESTATUS3"] != DBNull.Value)
                            {
                                InData.PhoneStatus3 = Convert.ToInt32(row["PHONESTATUS3"]);
                            }
                            if (row["PHONESTATUS4"] != DBNull.Value)
                            {
                                InData.PhoneStatus4 = Convert.ToInt32(row["PHONESTATUS4"]);
                            }
                            if (row["PHONESTATUS5"] != DBNull.Value)
                            {
                                InData.PhoneStatus5 = Convert.ToInt32(row["PHONESTATUS5"]);
                            }
                            if (row["PHONESTATUS6"] != DBNull.Value)
                            {
                                InData.PhoneStatus6 = Convert.ToInt32(row["PHONESTATUS6"]);
                            }
                            if (row["PHONESTATUS7"] != DBNull.Value)
                            {
                                InData.PhoneStatus7 = Convert.ToInt32(row["PHONESTATUS7"]);
                            }
                            if (row["PHONESTATUS8"] != DBNull.Value)
                            {
                                InData.PhoneStatus8 = Convert.ToInt32(row["PHONESTATUS8"]);
                            }
                            if (row["PHONESTATUS9"] != DBNull.Value)
                            {
                                InData.PhoneStatus9 = Convert.ToInt32(row["PHONESTATUS9"]);
                            }
                            if (row["PHONESTATUS10"] != DBNull.Value)
                            {
                                InData.PhoneStatus10 = Convert.ToInt32(row["PHONESTATUS10"]);
                            }
                            if (row["CURRENTPHONE"] != DBNull.Value)
                            {
                                InData.CurrentPhone = Convert.ToInt32(row["CURRENTPHONE"]);
                            }
                            if (row["COMMENTS"] != DBNull.Value)
                            {
                                InData.Comments = row["COMMENTS"].ToString();
                            }
                            if (row["CUSTOMDATA1"] != DBNull.Value)
                            {
                                InData.CustomData1 = row["CUSTOMDATA1"].ToString();
                            }
                            InData.CustomData2 = JsonConvert.SerializeObject(new Counters(row));
                            if (row["CUSTOMDATA3"] != DBNull.Value)
                            {
                                InData.CustomData3 = row["CUSTOMDATA3"].ToString();
                            }
                            if (row["SCHEDULEDATE"] != DBNull.Value)
                            {
                                InData.ScheduleDate = Convert.ToDateTime(row["SCHEDULEDATE"]);
                            }
                            #endregion
                        }
                    }
                }
                return InData;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<DNC> GetDNCListData(DNC inData)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(inData.GetPresConnString()))
                {
                    conn.Open();
                    DataTable result = new DataTable();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        #region [ SQL Query ]
                        cmd.CommandText = @"SELECT 
	                                            a.DONOTCALLLISTID,
	                                            a.RDATE,
	                                            b.DESCRIPTION,
	                                            b.STATUS,
	                                            b.PENDINGVERIFY,    
	                                            c.SERVICEID,
	                                            d.NAME,
	                                            d.TYPE
                                            FROM PREP.PCO_DONOTCALLPHONE a 
	                                            LEFT JOIN PREP.PCO_DONOTCALLLIST b 
	                                                ON a.DONOTCALLLISTID = b.ID 
	                                            LEFT JOIN PREP.PCO_DNCLISTSERVICERELATION c 
	                                                ON a.DONOTCALLLISTID = c.DONOTCALLLISTID
	                                            LEFT JOIN PVIEW.SERVICE d
	                                                ON c.SERVICEID = d.ID
                                            WHERE a.PHONE LIKE @Phone";
                        #endregion
                        cmd.Parameters.AddWithValue("@Phone", inData.Phone.TrimStart('0'));
                        cmd.CommandTimeout = 0;
                        SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        result.Load(reader);
                    }
                    inData.DNCListID = String.Empty;
                    foreach (DataRow row in result.Rows)
                    {
                        #region [ Create Comma Delimited List ]
                        inData.DNCListID += row["DONOTCALLLISTID"].ToString() + ",";
                        inData.DateAdded += row["RDATE"].ToString() + ",";
                        inData.DNCListDescription += row["DESCRIPTION"].ToString() + ",";
                        inData.DNCListStatus += row["STATUS"].ToString() + ",";
                        inData.DNCListPendingVerify += row["PENDINGVERIFY"].ToString() + ",";
                        inData.DNCListServiceID += row["SERVICEID"].ToString() + ",";
                        inData.DNCListServiceName += row["NAME"].ToString() + ",";
                        inData.DNCListServiceType += row["TYPE"].ToString() + ",";
                        #endregion
                    }
                    #region [ Trim Trailing Comma ]
                    inData.DNCListID = inData.DNCListID.Trim(',');
                    inData.DateAdded = inData.DateAdded.Trim(',');
                    inData.DNCListDescription = inData.DNCListDescription.Trim(',');
                    inData.DNCListStatus = inData.DNCListStatus.Trim(',');
                    inData.DNCListPendingVerify = inData.DNCListPendingVerify.Trim(',');
                    inData.DNCListServiceID = inData.DNCListServiceID.Trim(',');
                    inData.DNCListServiceName = inData.DNCListServiceName.Trim(',');
                    inData.DNCListServiceType = inData.DNCListServiceType.Trim(',');
                    #endregion
                }
                return inData;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> RemoveFromPresence(OutQueuePhone inData)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(inData.connStringPresence))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"DELETE FROM PREP.PCO_OUTBOUNDQUEUE WHERE SOURCEID = @SourceID AND LOADID = @LoadID AND SERVICEID = @ServiceID";
                        cmd.Parameters.AddWithValue("@SourceID", inData.SourceID);
                        cmd.Parameters.AddWithValue("@LoadID", inData.LoadID);
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.AddWithValue("@ServiceID", inData.ServiceID);
                        cmd.ExecuteNonQuery();
                    }
                    conn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> ModifyQueueResetAll(OutQueuePhone inData)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(inData.connStringPresence))
                {
                    string CommandText = @"UPDATE [PREP].[PCO_OUTBOUNDQUEUE]
                                            SET [SERVICEID] = @ServiceID
                                            ,[LOADID] = @LoadID
                                            ,[LASTAGENT] = NULL
                                            ,[LASTQCODE] = NULL
                                            ,[FIRSTHANDLINGDATE] = NULL
                                            ,[LASTHANDLINGDATE] = NULL
                                            ,[DAILYCOUNTER] = NULL
                                            ,[TOTALCOUNTER] = NULL
                                            ,[BUSYSIGNALCOUNTER] = NULL
                                            ,[NOANSWERCOUNTER] = NULL
                                            ,[ANSWERMACHINECOUNTER] = NULL
                                            ,[FAXCOUNTER] = NULL
                                            ,[INVGENREASONCOUNTER] = NULL
                                            ,[CAPTURINGAGENT] = NULL
                                            ,[CURRENTPHONECOUNTER] = NULL";
                    #region [ Conditional Addition to Query String ]
                    if (inData.Name != String.Empty && inData.Name != null)
                    {
                        CommandText += @",[NAME] = @Name";
                    }
                    if (inData.Phone != String.Empty && inData.Phone != null)
                    {
                        CommandText += @",[PHONE] = @Phone";
                    }
                    if (inData.Phone1 != String.Empty && inData.Phone1 != null)
                    {
                        CommandText += @",[PHONE1] = @Phone1";
                    }
                    if (inData.Phone2 != String.Empty && inData.Phone2 != null)
                    {
                        CommandText += @",[PHONE2] = @Phone2";
                    }
                    if (inData.Phone3 != String.Empty && inData.Phone3 != null)
                    {
                        CommandText += @",[PHONE3] = @Phone3";
                    }
                    if (inData.Phone4 != String.Empty && inData.Phone4 != null)
                    {
                        CommandText += @",[PHONE4] = @Phone4";
                    }
                    if (inData.Phone5 != String.Empty && inData.Phone5 != null)
                    {
                        CommandText += @",[PHONE5] = @Phone5";
                    }
                    if (inData.Phone6 != String.Empty && inData.Phone6 != null)
                    {
                        CommandText += @",[PHONE6] = @Phone6";
                    }
                    if (inData.Phone7 != String.Empty && inData.Phone7 != null)
                    {
                        CommandText += @",[PHONE7] = @Phone7";
                    }
                    if (inData.Phone8 != String.Empty && inData.Phone8 != null)
                    {
                        CommandText += @",[PHONE8] = @Phone8";
                    }
                    if (inData.Phone9 != String.Empty && inData.Phone9 != null)
                    {
                        CommandText += @",[PHONE9] = @Phone9";
                    }
                    if (inData.Phone10 != String.Empty && inData.Phone10 != null)
                    {
                        CommandText += @",[PHONE10] = @Phone10";
                    }
                    if (inData.PhoneDesc1 != 0 && inData.PhoneDesc1 != null)
                    {
                        CommandText += @",[PHONEDESC1] = @PhoneDesc1";
                    }
                    if (inData.PhoneDesc2 != 0 && inData.PhoneDesc2 != null)
                    {
                        CommandText += @",[PHONEDESC2] = @PhoneDesc2";
                    }
                    if (inData.PhoneDesc3 != 0 && inData.PhoneDesc3 != null)
                    {
                        CommandText += @",[PHONEDESC3] = @PhoneDesc3";
                    }
                    if (inData.PhoneDesc4 != 0 && inData.PhoneDesc4 != null)
                    {
                        CommandText += @",[PHONEDESC4] = @PhoneDesc4";
                    }
                    if (inData.PhoneDesc5 != 0 && inData.PhoneDesc5 != null)
                    {
                        CommandText += @",[PHONEDESC5] = @PhoneDesc5";
                    }
                    if (inData.PhoneDesc6 != 0 && inData.PhoneDesc6 != null)
                    {
                        CommandText += @",[PHONEDESC6] = @PhoneDesc6";
                    }
                    if (inData.PhoneDesc7 != 0 && inData.PhoneDesc7 != null)
                    {
                        CommandText += @",[PHONEDESC7] = @PhoneDesc7";
                    }
                    if (inData.PhoneDesc8 != 0 && inData.PhoneDesc8 != null)
                    {
                        CommandText += @",[PHONEDESC8] = @PhoneDesc8";
                    }
                    if (inData.PhoneDesc9 != 0 && inData.PhoneDesc9 != null)
                    {
                        CommandText += @",[PHONEDESC9] = @PhoneDesc9";
                    }
                    if (inData.PhoneDesc10 != 0 && inData.PhoneDesc10 != null)
                    {
                        CommandText += @",[PHONEDESC10] = @PhoneDesc10";
                    }
                    if (inData.PhoneStatus1 != 0 && inData.PhoneStatus1 != null)
                    {
                        CommandText += @",[PHONESTATUS1] = @PhoneStatus1";
                    }
                    if (inData.PhoneStatus2 != 0 && inData.PhoneStatus2 != null)
                    {
                        CommandText += @",[PHONESTATUS2] = @PhoneStatus2";
                    }
                    if (inData.PhoneStatus3 != 0 && inData.PhoneStatus3 != null)
                    {
                        CommandText += @",[PHONESTATUS3] = @PhoneStatus3";
                    }
                    if (inData.PhoneStatus4 != 0 && inData.PhoneStatus4 != null)
                    {
                        CommandText += @",[PHONESTATUS4] = @PhoneStatus4";
                    }
                    if (inData.PhoneStatus5 != 0 && inData.PhoneStatus5 != null)
                    {
                        CommandText += @",[PHONESTATUS5] = @PhoneStatus5";
                    }
                    if (inData.PhoneStatus6 != 0 && inData.PhoneStatus6 != null)
                    {
                        CommandText += @",[PHONESTATUS6] = @PhoneStatus6";
                    }
                    if (inData.PhoneStatus7 != 0 && inData.PhoneStatus7 != null)
                    {
                        CommandText += @",[PHONESTATUS7] = @PhoneStatus7";
                    }
                    if (inData.PhoneStatus8 != 0 && inData.PhoneStatus8 != null)
                    {
                        CommandText += @",[PHONESTATUS8] = @PhoneStatus8";
                    }
                    if (inData.PhoneStatus9 != 0 && inData.PhoneStatus9 != null)
                    {
                        CommandText += @",[PHONESTATUS9] = @PhoneStatus9";
                    }
                    if (inData.PhoneStatus10 != 0 && inData.PhoneStatus10 != null)
                    {
                        CommandText += @",[PHONESTATUS10] = @PhoneStatus10";
                    }
                    if (inData.Comments != String.Empty && inData.Comments != null)
                    {
                        CommandText += @",[COMMENTS] = @Comments";
                    }
                    if (inData.CustomData1 != String.Empty && inData.CustomData1 != null)
                    {
                        CommandText += @",[CUSTOMDATA1] = @CustomData1";
                    }
                    if (inData.CustomData2 != String.Empty && inData.CustomData2 != null)
                    {
                        CommandText += @",[CUSTOMDATA2] = @CustomData2";
                    }
                    if (inData.CustomData3 != String.Empty && inData.CustomData3 != null)
                    {
                        CommandText += @",[CUSTOMDATA3] = @CustomData3";
                    }
                    if (inData.CallerID != String.Empty && inData.CallerID != null)
                    {
                        CommandText += @",[CALLERID] = @CallerID";
                    }
                    if (inData.Status != null)
                    {
                        CommandText += @",[STATUS] = @Status";
                    }
                    if (inData.ScheduleDate != null)
                    {
                        CommandText += @",[SCHEDULEDATE] = @ScheduleDate";
                    }
                    #endregion
                    CommandText += @" WHERE SERVICEID = @ServiceIDOld AND LOADID = @LoadIDOld AND SOURCEID = @SourceID";
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        #region [ Command Text ]
                        cmd.CommandText = CommandText;
                        #endregion

                        #region [ Set Query Parameters ]
                        if (inData.ServiceID != 0 && inData.ServiceID != null)
                        {
                            cmd.Parameters.AddWithValue("@ServiceID", inData.ServiceID);
                        }
                        if (inData.ServiceIDOld != 0 && inData.ServiceIDOld != null)
                        {
                            cmd.Parameters.AddWithValue("@ServiceIDOld", inData.ServiceIDOld);
                        }
                        if (inData.Name != String.Empty && inData.Name != null)
                        {
                            cmd.Parameters.AddWithValue("@Name", inData.Name);
                        }
                        if (inData.Phone != String.Empty && inData.Phone != null)
                        {
                            cmd.Parameters.AddWithValue("@Phone", inData.Phone);
                        }
                        if (inData.SourceID != 0 && inData.SourceID != null)
                        {
                            cmd.Parameters.AddWithValue("@SourceID", inData.SourceID);
                        }
                        if (inData.CallingHours != String.Empty && inData.CallingHours != null)
                        {
                            cmd.Parameters.AddWithValue("@CallingHours", inData.CallingHours);
                        }
                        if (inData.Status != null)
                        {
                            cmd.Parameters.AddWithValue("@Status", inData.Status);
                        }
                        if (inData.LoadIDOld != 0 && inData.LoadIDOld != null)
                        {
                            cmd.Parameters.AddWithValue("@LoadIDOld", inData.LoadIDOld);
                        }
                        if (inData.LoadID != 0 && inData.LoadID != null)
                        {
                            cmd.Parameters.AddWithValue("@LoadID", inData.LoadID);
                        }
                        if (inData.Priority != 0 && inData.Priority != null)
                        {
                            cmd.Parameters.AddWithValue("@Priority", inData.Priority);
                        }
                        if (inData.Phone1 != String.Empty && inData.Phone1 != null)
                        {
                            cmd.Parameters.AddWithValue("@Phone1", inData.Phone1);
                        }
                        if (inData.Phone2 != String.Empty && inData.Phone2 != null)
                        {
                            cmd.Parameters.AddWithValue("@Phone2", inData.Phone2);
                        }
                        if (inData.Phone3 != String.Empty && inData.Phone3 != null)
                        {
                            cmd.Parameters.AddWithValue("@Phone3", inData.Phone3);
                        }
                        if (inData.Phone4 != String.Empty && inData.Phone4 != null)
                        {
                            cmd.Parameters.AddWithValue("@Phone4", inData.Phone4);
                        }
                        if (inData.Phone5 != String.Empty && inData.Phone5 != null)
                        {
                            cmd.Parameters.AddWithValue("@Phone5", inData.Phone5);
                        }
                        if (inData.Phone6 != String.Empty && inData.Phone6 != null)
                        {
                            cmd.Parameters.AddWithValue("@Phone6", inData.Phone6);
                        }
                        if (inData.Phone7 != String.Empty && inData.Phone7 != null)
                        {
                            cmd.Parameters.AddWithValue("@Phone7", inData.Phone7);
                        }
                        if (inData.Phone8 != String.Empty && inData.Phone8 != null)
                        {
                            cmd.Parameters.AddWithValue("@Phone8", inData.Phone8);
                        }
                        if (inData.Phone9 != String.Empty && inData.Phone9 != null)
                        {
                            cmd.Parameters.AddWithValue("@Phone9", inData.Phone9);
                        }
                        if (inData.Phone10 != String.Empty && inData.Phone10 != null)
                        {
                            cmd.Parameters.AddWithValue("@Phone10", inData.Phone10);
                        }
                        if (inData.PhoneDesc1 != 0 && inData.PhoneDesc1 != null)
                        {
                            cmd.Parameters.AddWithValue("@PhoneDesc1", inData.PhoneDesc1);
                        }
                        if (inData.PhoneDesc2 != 0 && inData.PhoneDesc2 != null)
                        {
                            cmd.Parameters.AddWithValue("@PhoneDesc2", inData.PhoneDesc2);
                        }
                        if (inData.PhoneDesc3 != 0 && inData.PhoneDesc3 != null)
                        {
                            cmd.Parameters.AddWithValue("@PhoneDesc3", inData.PhoneDesc3);
                        }
                        if (inData.PhoneDesc4 != 0 && inData.PhoneDesc4 != null)
                        {
                            cmd.Parameters.AddWithValue("@PhoneDesc4", inData.PhoneDesc4);
                        }
                        if (inData.PhoneDesc5 != 0 && inData.PhoneDesc5 != null)
                        {
                            cmd.Parameters.AddWithValue("@PhoneDesc5", inData.PhoneDesc5);
                        }
                        if (inData.PhoneDesc6 != 0 && inData.PhoneDesc6 != null)
                        {
                            cmd.Parameters.AddWithValue("@PhoneDesc6", inData.PhoneDesc6);
                        }
                        if (inData.PhoneDesc7 != 0 && inData.PhoneDesc7 != null)
                        {
                            cmd.Parameters.AddWithValue("@PhoneDesc7", inData.PhoneDesc7);
                        }
                        if (inData.PhoneDesc8 != 0 && inData.PhoneDesc8 != null)
                        {
                            cmd.Parameters.AddWithValue("@PhoneDesc8", inData.PhoneDesc8);
                        }
                        if (inData.PhoneDesc9 != 0 && inData.PhoneDesc9 != null)
                        {
                            cmd.Parameters.AddWithValue("@PhoneDesc9", inData.PhoneDesc9);
                        }
                        if (inData.PhoneDesc10 != 0 && inData.PhoneDesc10 != null)
                        {
                            cmd.Parameters.AddWithValue("@PhoneDesc10", inData.PhoneDesc10);
                        }
                        if (inData.PhoneStatus1 != 0 && inData.PhoneStatus1 != null)
                        {
                            cmd.Parameters.AddWithValue("@PhoneStatus1", inData.PhoneStatus1);
                        }
                        if (inData.PhoneStatus2 != 0 && inData.PhoneStatus2 != null)
                        {
                            cmd.Parameters.AddWithValue("@PhoneStatus2", inData.PhoneStatus2);
                        }
                        if (inData.PhoneStatus3 != 0 && inData.PhoneStatus3 != null)
                        {
                            cmd.Parameters.AddWithValue("@PhoneStatus3", inData.PhoneStatus3);
                        }
                        if (inData.PhoneStatus4 != 0 && inData.PhoneStatus4 != null)
                        {
                            cmd.Parameters.AddWithValue("@PhoneStatus4", inData.PhoneStatus4);
                        }
                        if (inData.PhoneStatus5 != 0 && inData.PhoneStatus5 != null)
                        {
                            cmd.Parameters.AddWithValue("@PhoneStatus5", inData.PhoneStatus5);
                        }
                        if (inData.PhoneStatus6 != 0 && inData.PhoneStatus6 != null)
                        {
                            cmd.Parameters.AddWithValue("@PhoneStatus6", inData.PhoneStatus6);
                        }
                        if (inData.PhoneStatus7 != 0 && inData.PhoneStatus7 != null)
                        {
                            cmd.Parameters.AddWithValue("@PhoneStatus7", inData.PhoneStatus7);
                        }
                        if (inData.PhoneStatus8 != 0 && inData.PhoneStatus8 != null)
                        {
                            cmd.Parameters.AddWithValue("@PhoneStatus8", inData.PhoneStatus8);
                        }
                        if (inData.PhoneStatus9 != 0 && inData.PhoneStatus9 != null)
                        {
                            cmd.Parameters.AddWithValue("@PhoneStatus9", inData.PhoneStatus9);
                        }
                        if (inData.PhoneStatus10 != 0 && inData.PhoneStatus10 != null)
                        {
                            cmd.Parameters.AddWithValue("@PhoneStatus10", inData.PhoneStatus10);
                        }
                        if (inData.Comments != String.Empty && inData.Comments != null)
                        {
                            cmd.Parameters.AddWithValue("@Comments", inData.Comments);
                        }
                        if (inData.CustomData1 != String.Empty && inData.CustomData1 != null)
                        {
                            cmd.Parameters.AddWithValue("@CustomData1", inData.CustomData1);
                        }
                        if (inData.CustomData2 != String.Empty && inData.CustomData2 != null)
                        {
                            cmd.Parameters.AddWithValue("@CustomData2", inData.CustomData2);
                        }
                        if (inData.CustomData3 != String.Empty && inData.CustomData3 != null)
                        {
                            cmd.Parameters.AddWithValue("@CustomData3", inData.CustomData3);
                        }
                        if (inData.ScheduleDate != null)
                        {
                            cmd.Parameters.AddWithValue("@ScheduleDate", inData.ScheduleDate);
                        }
                        #endregion
                        cmd.CommandTimeout = 0;
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> ModifyQueue(OutQueuePhone inData)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(inData.connStringPresence))
                {
                    string CommandText = @"UPDATE [PREP].[PCO_OUTBOUNDQUEUE]
                                            SET [SERVICEID] = @ServiceID
                                            ,[LOADID] = @LoadID";
                    #region [ Conditional Addition to Query String ]
                    if (inData.Name != String.Empty && inData.Name != null)
                    {
                        CommandText += @",[NAME] = @Name";
                    }
                    if (inData.Phone != String.Empty && inData.Phone != null)
                    {
                        CommandText += @",[PHONE] = @Phone";
                    }
                    if (inData.Phone1 != String.Empty && inData.Phone1 != null)
                    {
                        CommandText += @",[PHONE1] = @Phone1";
                    }
                    if (inData.Phone2 != String.Empty && inData.Phone2 != null)
                    {
                        CommandText += @",[PHONE2] = @Phone2";
                    }
                    if (inData.Phone3 != String.Empty && inData.Phone3 != null)
                    {
                        CommandText += @",[PHONE3] = @Phone3";
                    }
                    if (inData.Phone4 != String.Empty && inData.Phone4 != null)
                    {
                        CommandText += @",[PHONE4] = @Phone4";
                    }
                    if (inData.Phone5 != String.Empty && inData.Phone5 != null)
                    {
                        CommandText += @",[PHONE5] = @Phone5";
                    }
                    if (inData.Phone6 != String.Empty && inData.Phone6 != null)
                    {
                        CommandText += @",[PHONE6] = @Phone6";
                    }
                    if (inData.Phone7 != String.Empty && inData.Phone7 != null)
                    {
                        CommandText += @",[PHONE7] = @Phone7";
                    }
                    if (inData.Phone8 != String.Empty && inData.Phone8 != null)
                    {
                        CommandText += @",[PHONE8] = @Phone8";
                    }
                    if (inData.Phone9 != String.Empty && inData.Phone9 != null)
                    {
                        CommandText += @",[PHONE9] = @Phone9";
                    }
                    if (inData.Phone10 != String.Empty && inData.Phone10 != null)
                    {
                        CommandText += @",[PHONE10] = @Phone10";
                    }
                    if (inData.PhoneDesc1 != 0 && inData.PhoneDesc1 != null)
                    {
                        CommandText += @",[PHONEDESC1] = @PhoneDesc1";
                    }
                    if (inData.PhoneDesc2 != 0 && inData.PhoneDesc2 != null)
                    {
                        CommandText += @",[PHONEDESC2] = @PhoneDesc2";
                    }
                    if (inData.PhoneDesc3 != 0 && inData.PhoneDesc3 != null)
                    {
                        CommandText += @",[PHONEDESC3] = @PhoneDesc3";
                    }
                    if (inData.PhoneDesc4 != 0 && inData.PhoneDesc4 != null)
                    {
                        CommandText += @",[PHONEDESC4] = @PhoneDesc4";
                    }
                    if (inData.PhoneDesc5 != 0 && inData.PhoneDesc5 != null)
                    {
                        CommandText += @",[PHONEDESC5] = @PhoneDesc5";
                    }
                    if (inData.PhoneDesc6 != 0 && inData.PhoneDesc6 != null)
                    {
                        CommandText += @",[PHONEDESC6] = @PhoneDesc6";
                    }
                    if (inData.PhoneDesc7 != 0 && inData.PhoneDesc7 != null)
                    {
                        CommandText += @",[PHONEDESC7] = @PhoneDesc7";
                    }
                    if (inData.PhoneDesc8 != 0 && inData.PhoneDesc8 != null)
                    {
                        CommandText += @",[PHONEDESC8] = @PhoneDesc8";
                    }
                    if (inData.PhoneDesc9 != 0 && inData.PhoneDesc9 != null)
                    {
                        CommandText += @",[PHONEDESC9] = @PhoneDesc9";
                    }
                    if (inData.PhoneDesc10 != 0 && inData.PhoneDesc10 != null)
                    {
                        CommandText += @",[PHONEDESC10] = @PhoneDesc10";
                    }
                    if (inData.PhoneStatus1 != 0 && inData.PhoneStatus1 != null)
                    {
                        CommandText += @",[PHONESTATUS1] = @PhoneStatus1";
                    }
                    if (inData.PhoneStatus2 != 0 && inData.PhoneStatus2 != null)
                    {
                        CommandText += @",[PHONESTATUS2] = @PhoneStatus2";
                    }
                    if (inData.PhoneStatus3 != 0 && inData.PhoneStatus3 != null)
                    {
                        CommandText += @",[PHONESTATUS3] = @PhoneStatus3";
                    }
                    if (inData.PhoneStatus4 != 0 && inData.PhoneStatus4 != null)
                    {
                        CommandText += @",[PHONESTATUS4] = @PhoneStatus4";
                    }
                    if (inData.PhoneStatus5 != 0 && inData.PhoneStatus5 != null)
                    {
                        CommandText += @",[PHONESTATUS5] = @PhoneStatus5";
                    }
                    if (inData.PhoneStatus6 != 0 && inData.PhoneStatus6 != null)
                    {
                        CommandText += @",[PHONESTATUS6] = @PhoneStatus6";
                    }
                    if (inData.PhoneStatus7 != 0 && inData.PhoneStatus7 != null)
                    {
                        CommandText += @",[PHONESTATUS7] = @PhoneStatus7";
                    }
                    if (inData.PhoneStatus8 != 0 && inData.PhoneStatus8 != null)
                    {
                        CommandText += @",[PHONESTATUS8] = @PhoneStatus8";
                    }
                    if (inData.PhoneStatus9 != 0 && inData.PhoneStatus9 != null)
                    {
                        CommandText += @",[PHONESTATUS9] = @PhoneStatus9";
                    }
                    if (inData.PhoneStatus10 != 0 && inData.PhoneStatus10 != null)
                    {
                        CommandText += @",[PHONESTATUS10] = @PhoneStatus10";
                    }
                    if (inData.Comments != String.Empty && inData.Comments != null)
                    {
                        CommandText += @",[COMMENTS] = @Comments";
                    }
                    if (inData.CustomData1 != String.Empty && inData.CustomData1 != null)
                    {
                        CommandText += @",[CUSTOMDATA1] = @CustomData1";
                    }
                    if (inData.CustomData2 != String.Empty && inData.CustomData2 != null)
                    {
                        CommandText += @",[CUSTOMDATA2] = @CustomData2";
                    }
                    if (inData.CustomData3 != String.Empty && inData.CustomData3 != null)
                    {
                        CommandText += @",[CUSTOMDATA3] = @CustomData3";
                    }
                    if (inData.CallerID != String.Empty && inData.CallerID != null)
                    {
                        CommandText += @",[CALLERID] = @CallerID";
                    }
                    if (inData.Status != null)
                    {
                        CommandText += @",[STATUS] = @Status";
                    }
                    if (inData.ScheduleDate != null)
                    {
                        CommandText += @",[SCHEDULEDATE] = @ScheduleDate";
                    }
                    #endregion
                    CommandText += @" WHERE SERVICEID = @ServiceIDOld AND LOADID = @LoadIDOld AND SOURCEID = @SourceID";
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        #region [ Command Text ]
                        cmd.CommandText = CommandText;
                        #endregion

                        #region [ Set Query Parameters ]
                        if (inData.ServiceID != 0 && inData.ServiceID != null)
                        {
                            cmd.Parameters.AddWithValue("@ServiceID", inData.ServiceID);
                        }
                        if (inData.ServiceIDOld != 0 && inData.ServiceIDOld != null)
                        {
                            cmd.Parameters.AddWithValue("@ServiceIDOld", inData.ServiceIDOld);
                        }
                        if (inData.Name != String.Empty && inData.Name != null)
                        {
                            cmd.Parameters.AddWithValue("@Name", inData.Name);
                        }
                        if (inData.Phone != String.Empty && inData.Phone != null)
                        {
                            cmd.Parameters.AddWithValue("@Phone", inData.Phone);
                        }
                        if (inData.SourceID != 0 && inData.SourceID != null)
                        {
                            cmd.Parameters.AddWithValue("@SourceID", inData.SourceID);
                        }
                        if (inData.CallingHours != String.Empty && inData.CallingHours != null)
                        {
                            cmd.Parameters.AddWithValue("@CallingHours", inData.CallingHours);
                        }
                        if (inData.Status != null)
                        {
                            cmd.Parameters.AddWithValue("@Status", inData.Status);
                        }
                        if (inData.LoadIDOld != 0 && inData.LoadIDOld != null)
                        {
                            cmd.Parameters.AddWithValue("@LoadIDOld", inData.LoadIDOld);
                        }
                        if (inData.LoadID != 0 && inData.LoadID != null)
                        {
                            cmd.Parameters.AddWithValue("@LoadID", inData.LoadID);
                        }
                        if (inData.Priority != 0 && inData.Priority != null)
                        {
                            cmd.Parameters.AddWithValue("@Priority", inData.Priority);
                        }
                        if (inData.Phone1 != String.Empty && inData.Phone1 != null)
                        {
                            cmd.Parameters.AddWithValue("@Phone1", inData.Phone1);
                        }
                        if (inData.Phone2 != String.Empty && inData.Phone2 != null)
                        {
                            cmd.Parameters.AddWithValue("@Phone2", inData.Phone2);
                        }
                        if (inData.Phone3 != String.Empty && inData.Phone3 != null)
                        {
                            cmd.Parameters.AddWithValue("@Phone3", inData.Phone3);
                        }
                        if (inData.Phone4 != String.Empty && inData.Phone4 != null)
                        {
                            cmd.Parameters.AddWithValue("@Phone4", inData.Phone4);
                        }
                        if (inData.Phone5 != String.Empty && inData.Phone5 != null)
                        {
                            cmd.Parameters.AddWithValue("@Phone5", inData.Phone5);
                        }
                        if (inData.Phone6 != String.Empty && inData.Phone6 != null)
                        {
                            cmd.Parameters.AddWithValue("@Phone6", inData.Phone6);
                        }
                        if (inData.Phone7 != String.Empty && inData.Phone7 != null)
                        {
                            cmd.Parameters.AddWithValue("@Phone7", inData.Phone7);
                        }
                        if (inData.Phone8 != String.Empty && inData.Phone8 != null)
                        {
                            cmd.Parameters.AddWithValue("@Phone8", inData.Phone8);
                        }
                        if (inData.Phone9 != String.Empty && inData.Phone9 != null)
                        {
                            cmd.Parameters.AddWithValue("@Phone9", inData.Phone9);
                        }
                        if (inData.Phone10 != String.Empty && inData.Phone10 != null)
                        {
                            cmd.Parameters.AddWithValue("@Phone10", inData.Phone10);
                        }
                        if (inData.PhoneDesc1 != 0 && inData.PhoneDesc1 != null)
                        {
                            cmd.Parameters.AddWithValue("@PhoneDesc1", inData.PhoneDesc1);
                        }
                        if (inData.PhoneDesc2 != 0 && inData.PhoneDesc2 != null)
                        {
                            cmd.Parameters.AddWithValue("@PhoneDesc2", inData.PhoneDesc2);
                        }
                        if (inData.PhoneDesc3 != 0 && inData.PhoneDesc3 != null)
                        {
                            cmd.Parameters.AddWithValue("@PhoneDesc3", inData.PhoneDesc3);
                        }
                        if (inData.PhoneDesc4 != 0 && inData.PhoneDesc4 != null)
                        {
                            cmd.Parameters.AddWithValue("@PhoneDesc4", inData.PhoneDesc4);
                        }
                        if (inData.PhoneDesc5 != 0 && inData.PhoneDesc5 != null)
                        {
                            cmd.Parameters.AddWithValue("@PhoneDesc5", inData.PhoneDesc5);
                        }
                        if (inData.PhoneDesc6 != 0 && inData.PhoneDesc6 != null)
                        {
                            cmd.Parameters.AddWithValue("@PhoneDesc6", inData.PhoneDesc6);
                        }
                        if (inData.PhoneDesc7 != 0 && inData.PhoneDesc7 != null)
                        {
                            cmd.Parameters.AddWithValue("@PhoneDesc7", inData.PhoneDesc7);
                        }
                        if (inData.PhoneDesc8 != 0 && inData.PhoneDesc8 != null)
                        {
                            cmd.Parameters.AddWithValue("@PhoneDesc8", inData.PhoneDesc8);
                        }
                        if (inData.PhoneDesc9 != 0 && inData.PhoneDesc9 != null)
                        {
                            cmd.Parameters.AddWithValue("@PhoneDesc9", inData.PhoneDesc9);
                        }
                        if (inData.PhoneDesc10 != 0 && inData.PhoneDesc10 != null)
                        {
                            cmd.Parameters.AddWithValue("@PhoneDesc10", inData.PhoneDesc10);
                        }
                        if (inData.PhoneStatus1 != 0 && inData.PhoneStatus1 != null)
                        {
                            cmd.Parameters.AddWithValue("@PhoneStatus1", inData.PhoneStatus1);
                        }
                        if (inData.PhoneStatus2 != 0 && inData.PhoneStatus2 != null)
                        {
                            cmd.Parameters.AddWithValue("@PhoneStatus2", inData.PhoneStatus2);
                        }
                        if (inData.PhoneStatus3 != 0 && inData.PhoneStatus3 != null)
                        {
                            cmd.Parameters.AddWithValue("@PhoneStatus3", inData.PhoneStatus3);
                        }
                        if (inData.PhoneStatus4 != 0 && inData.PhoneStatus4 != null)
                        {
                            cmd.Parameters.AddWithValue("@PhoneStatus4", inData.PhoneStatus4);
                        }
                        if (inData.PhoneStatus5 != 0 && inData.PhoneStatus5 != null)
                        {
                            cmd.Parameters.AddWithValue("@PhoneStatus5", inData.PhoneStatus5);
                        }
                        if (inData.PhoneStatus6 != 0 && inData.PhoneStatus6 != null)
                        {
                            cmd.Parameters.AddWithValue("@PhoneStatus6", inData.PhoneStatus6);
                        }
                        if (inData.PhoneStatus7 != 0 && inData.PhoneStatus7 != null)
                        {
                            cmd.Parameters.AddWithValue("@PhoneStatus7", inData.PhoneStatus7);
                        }
                        if (inData.PhoneStatus8 != 0 && inData.PhoneStatus8 != null)
                        {
                            cmd.Parameters.AddWithValue("@PhoneStatus8", inData.PhoneStatus8);
                        }
                        if (inData.PhoneStatus9 != 0 && inData.PhoneStatus9 != null)
                        {
                            cmd.Parameters.AddWithValue("@PhoneStatus9", inData.PhoneStatus9);
                        }
                        if (inData.PhoneStatus10 != 0 && inData.PhoneStatus10 != null)
                        {
                            cmd.Parameters.AddWithValue("@PhoneStatus10", inData.PhoneStatus10);
                        }
                        if (inData.Comments != String.Empty && inData.Comments != null)
                        {
                            cmd.Parameters.AddWithValue("@Comments", inData.Comments);
                        }
                        if (inData.CustomData1 != String.Empty && inData.CustomData1 != null)
                        {
                            cmd.Parameters.AddWithValue("@CustomData1", inData.CustomData1);
                        }
                        if (inData.CustomData2 != String.Empty && inData.CustomData2 != null)
                        {
                            cmd.Parameters.AddWithValue("@CustomData2", inData.CustomData2);
                        }
                        if (inData.CustomData3 != String.Empty && inData.CustomData3 != null)
                        {
                            cmd.Parameters.AddWithValue("@CustomData3", inData.CustomData3);
                        }
                        if (inData.ScheduleDate != null)
                        {
                            cmd.Parameters.AddWithValue("@ScheduleDate", inData.ScheduleDate);
                        }
                        #endregion
                        cmd.CommandTimeout = 0;
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> CheckPrimaryPhoneDNC(OutQueuePhone inData)
        {
            try
            {
                if (inData.Phone == String.Empty)
                {
                    inData.Phone = inData.Phone1 ?? inData.Phone2 ?? inData.Phone3 ?? inData.Phone4 ?? inData.Phone5 ?? inData.Phone6 ?? inData.Phone7 ?? inData.Phone8 ?? inData.Phone9 ?? inData.Phone10;
                }
                if (inData.Phone != String.Empty)
                {
                    string sqlQuery = @"SELECT 
	                                    COUNT(*) as IsPresent
                                    FROM PREP.PCO_DONOTCALLPHONE a 
	                                    LEFT JOIN PREP.PCO_DONOTCALLLIST b 
	                                        ON a.DONOTCALLLISTID = b.ID 
	                                    LEFT JOIN PREP.PCO_DNCLISTSERVICERELATION c 
	                                        ON a.DONOTCALLLISTID = c.DONOTCALLLISTID
	                                    LEFT JOIN PVIEW.SERVICE d
	                                        ON c.SERVICEID = d.ID
                                    WHERE (a.PHONE = '" + inData.Phone.TrimStart('0') + @"'
                                        OR a.PHONE = '" + inData.Phone + @"')
                                        AND d.ID = @ServiceID
                                        AND b.STATUS = 'E'";
                    using (SqlConnection conn = new SqlConnection(inData.connStringPresence))
                    {
                        conn.Open();
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = sqlQuery;
                            cmd.CommandTimeout = 0;
                            cmd.Parameters.AddWithValue("@ServiceID", inData.ServiceID);
                            DataTable tempResult = new DataTable();
                            SqlDataReader reader = cmd.ExecuteReader();
                            tempResult.Load(reader);
                            DataRow row = tempResult.AsEnumerable().First();
                            if (Convert.ToInt32(row["IsPresent"]) > 0)
                            {
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                    }
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> CheckSinglePhoneDNC(string inPhone, string inConnString, int inServiceID)
        {
            try
            {
                string sqlQuery = @"SELECT 
	                                COUNT(*) as IsPresent
                                FROM PREP.PCO_DONOTCALLPHONE a 
	                                LEFT JOIN PREP.PCO_DONOTCALLLIST b 
	                                    ON a.DONOTCALLLISTID = b.ID 
	                                LEFT JOIN PREP.PCO_DNCLISTSERVICERELATION c 
	                                    ON a.DONOTCALLLISTID = c.DONOTCALLLISTID
	                                LEFT JOIN PVIEW.SERVICE d
	                                    ON c.SERVICEID = d.ID
                                WHERE a.PHONE = '" + inPhone.TrimStart('0') + @"'
                                OR a.PHONE = '" + inPhone + @"'
                                    AND d.ID = @ServiceID
                                    AND b.STATUS = 'E'";
                using (SqlConnection conn = new SqlConnection(inConnString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sqlQuery;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.AddWithValue("@ServiceID", inServiceID);
                        DataTable tempResult = new DataTable();
                        SqlDataReader reader = cmd.ExecuteReader();
                        tempResult.Load(reader);
                        DataRow row = tempResult.AsEnumerable().First();
                        if (Convert.ToInt32(row["IsPresent"]) > 0)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> FormatNumber(string inNumber, string connString)
        {
            try
            {
                string query = @"SELECT [ECM].[CIMNumber] (@NumberIn) AS ResultOut";
                string outNumber = null;
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = query;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.AddWithValue("@NumberIn", inNumber);
                        DataTable tableRes = new DataTable();
                        SqlDataReader reader = cmd.ExecuteReader();
                        tableRes.Load(reader);
                        if (tableRes.Rows[0]["ResultOut"] != DBNull.Value)
                        {
                            outNumber = tableRes.Rows[0]["ResultOut"].ToString();
                        }
                    }
                    conn.Close();
                }
                return outNumber;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> LogAPI(DataTable inTable, string connString)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                SqlBulkCopy bulkCopy = new SqlBulkCopy(conn);
                bulkCopy.DestinationTableName = "[ECM].[APILog]";
                bulkCopy.BatchSize = 1;
                bulkCopy.BulkCopyTimeout = 0;
                bulkCopy.WriteToServer(inTable);
                return true;
            }
        }

        public async Task<bool> logError(OutQueuePhone data, string Class, string method, string exception)
        {
            try
            {
                string query = @"INSERT INTO [ECM].[LogConsoleError]
                                (
                                    [InstanceID]
                                    ,[Class]
                                    ,[Method]
                                    ,[Error]
                                    ,[Updated]
                                )
                                VALUES
                                (
                                    'API Error Log'
                                    ,'OutQueuePhone - " + Class + @"'
                                    ,'" + method + @"'
                                    ,'" + exception + @"'
                                    ,GETDATE()
                                )";
                string outNumber = String.Empty;
                using (SqlConnection conn = new SqlConnection(data.connStringCIM))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandText = query;
                        cmd.ExecuteNonQuery();
                    }
                    conn.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> AddDNCIfNotExists(DNC inData)
        {
            try
            {
                int tempInt;
                using (SqlConnection conn = new SqlConnection(inData.GetPresConnString()))
                {
                    conn.Open();
                    DataTable result = new DataTable();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"IF NOT EXISTS(SELECT * FROM PREP.PCO_DONOTCALLPHONE WHERE PHONE LIKE @Phone AND DONOTCALLLISTID = @DNCListID)
                                                BEGIN
	                                                INSERT INTO PREP.PCO_DONOTCALLPHONE
	                                                (
		                                                DONOTCALLLISTID,
		                                                PHONE,
		                                                LOADERTYPE,
		                                                LOADERID,
		                                                RDATE
	                                                )
	                                                VALUES
	                                                (
		                                                @DNCListID,
		                                                @Phone,
		                                                3,
		                                                0,
		                                                GETDATE()
	                                                )
                                                SELECT 1 AS ReturnValue
                                                END
                                            ELSE
                                            BEGIN
                                                SELECT 0 AS ReturnValue
                                            END";
                        cmd.Parameters.AddWithValue("@Phone", inData.Phone.TrimStart('0'));
                        cmd.Parameters.AddWithValue("@DNCListID", inData.DNCListID.ToString());
                        cmd.CommandTimeout = 0;
                        SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        result.Load(reader);
                        tempInt = Convert.ToInt32(result.Rows[0]["ReturnValue"]);
                    }
                }
                if (tempInt == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<OutQueuePhone> GetLastLoadID(OutQueuePhone InData, bool isDev)
        {
            try
            {
                DataTable result = new DataTable();
                var date = DateTime.Now.Date;
                DateTime firstMonthDay = new DateTime(date.Year, date.Month, 1);
                DateTime firstMonthMonday = firstMonthDay.AddDays((DayOfWeek.Monday + 7 - firstMonthDay.DayOfWeek) % 7);
                if (firstMonthMonday > date)
                {
                    firstMonthDay = firstMonthDay.AddMonths(-1);
                    firstMonthMonday = firstMonthDay.AddDays((DayOfWeek.Monday + 7 - firstMonthDay.DayOfWeek) % 7);
                }
                int Batch = (date - firstMonthMonday).Days / 7 + 1;
                using (var conn = new SqlConnection(InData.connStringCIM))
                {
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        conn.Open();
                        if(isDev)
                        {
                            cmd.CommandText = @"SELECT 
                                                    TOP 1 LoadID
                                                FROM
                                                    ECM.APITableDev
                                                WHERE
                                                    DATEPART(YEAR, Received) = DATEPART(YEAR, GETDATE())
                                                AND
                                                    DATEPART(MONTH, Received) = DATEPART(MONTH, GETDATE())
                                                AND
                                                    Batch = @BatchNumber
                                                AND
                                                    ServiceID = @ServiceID
                                                AND
                                                    LeadProvider = @LeadProvider
                                                ORDER BY LOADID DESC";
                        }
                        else if(!isDev)
                        {
                            cmd.CommandText = @"SELECT 
                                                    TOP 1 LoadID
                                                FROM
                                                    ECM.APITable
                                                WHERE
                                                    DATEPART(YEAR, Received) = DATEPART(YEAR, GETDATE())
                                                AND
                                                    DATEPART(MONTH, Received) = DATEPART(MONTH, GETDATE())
                                                AND
                                                    Batch = @BatchNumber
                                                AND
                                                    ServiceID = @ServiceID
                                                AND
                                                    LeadProvider = @LeadProvider
                                                ORDER BY LOADID DESC";
                        }
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.AddWithValue("@BatchNumber", Batch);
                        cmd.Parameters.AddWithValue("@ServiceID", InData.ServiceID);
                        cmd.Parameters.AddWithValue("@LeadProvider", InData.leadProvider);
                        SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        result.Load(reader);
                    }
                    conn.Close();
                }
                InData.LoadID = Convert.ToInt32(result.Rows[0]["LoadID"]);
                return InData;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
