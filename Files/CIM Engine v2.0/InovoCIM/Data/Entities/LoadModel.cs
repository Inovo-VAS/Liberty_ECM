#region [ Using ]
using InovoCIM.Data.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace InovoCIM.Data.Entities
{
    public class LoadModel
    {
        public string InstanceID { get; set; }
        public int LoadID { get; set; }
        public int ServiceID { get; set; }
        public string Indicator { get; set; }
        public string Partial { get; set; }
        public string Field { get; set; }

        public LoadModel() { }
        public LoadModel(string _InstanceID, int _ServiceID, string _Indicator, string _Partial, string _Field)
        {
            this.InstanceID = _InstanceID;
            this.LoadID = -1;
            this.ServiceID = _ServiceID;
            this.Indicator = _Indicator;
            this.Partial = _Partial;
            this.Field = _Field;
        }

        public async Task<int> GetLoadID()
        {
            DataTable TempData = new DataTable();
            int LoadID = 1;
            try
            {
                string Description = this.Partial + " [ " + this.Field + " = " + this.Indicator + " ]";

                StringBuilder variables = new StringBuilder();
                #region [ Create all Variables to be passed ]
                variables.Append(string.Format("DECLARE @ServiceID INT = {0} ", this.ServiceID));
                variables.Append(string.Format("DECLARE @LoadID INT = {0} ", this.LoadID));
                variables.Append(string.Format("DECLARE @Indicator VARCHAR(150) = '{0}' ", Description));
                variables.Append(string.Format("DECLARE @RDate DATETIME = GETDATE()"));
                #endregion

                string query = variables.ToString() + " " + 
                    @"SELECT TOP 1 [LOADID] FROM [PREP].[PCO_LOAD] WHERE [SERVICEID] = @ServiceID AND [DESCRIPTION] = @Indicator";

                using (var conn = new SqlConnection(Database.dbPresence))
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    await conn.OpenAsync();
                    cmd.CommandTimeout = 0;
                    cmd.CommandText = query;
                    SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    TempData.Load(reader);
                }

                if (TempData.Rows.Count == 0)
                {
                    LoadID = await CreateLoadID();
                }
                else
                {
                    foreach (DataRow row in TempData.Rows)
                    {
                        LoadID = int.Parse(row["LOADID"].ToString());
                    }
                }

                return LoadID;
            }
            catch (Exception ex)
            {
                var log = new LogConsoleError(InstanceID, "LoadModel", "GetLoadID()", ex.Message);
                await log.SaveSync();

                return LoadID;
            }
        }

        public async Task<int> GetLoadIDAdhocServiceID()
        {
            DataTable TempData = new DataTable();
            int LoadID = 1;
            try
            {
                string Description = this.Partial + "_" + DateTime.Now.ToString("yyyyMMdd");

                StringBuilder variables = new StringBuilder();
                #region [ Create all Variables to be passed ]
                variables.Append(string.Format("DECLARE @ServiceID INT = {0} ", this.ServiceID));
                variables.Append(string.Format("DECLARE @LoadID INT = {0} ", this.LoadID));
                variables.Append(string.Format("DECLARE @Indicator VARCHAR(150) = '{0}' ", Description));
                variables.Append(string.Format("DECLARE @RDate DATETIME = GETDATE()"));
                #endregion

                string query = variables.ToString() + " " +
                    @"SELECT TOP 1 [LOADID] FROM [PREP].[PCO_LOAD] WHERE [SERVICEID] = @ServiceID AND [DESCRIPTION] = @Indicator";

                using (var conn = new SqlConnection(Database.dbPresence))
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    await conn.OpenAsync();
                    cmd.CommandTimeout = 0;
                    cmd.CommandText = query;
                    SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    TempData.Load(reader);
                }

                if (TempData.Rows.Count == 0)
                {
                    LoadID = await CreateLoadID();
                }
                else
                {
                    foreach (DataRow row in TempData.Rows)
                    {
                        LoadID = int.Parse(row["LOADID"].ToString());
                    }
                }

                return LoadID;
            }
            catch (Exception ex)
            {
                var log = new LogConsoleError(InstanceID, "LoadModel", "GetLoadID()", ex.Message);
                await log.SaveSync();

                return LoadID;
            }
        }

        public async Task<int> CreateLoadID()
        {
            DataTable TempData = new DataTable();
            int LoadID = 1;
            try
            {
                string Description = this.Partial + " [ " + this.Field + " = " + this.Indicator + " ]";

                StringBuilder variables = new StringBuilder();
                #region [ Create all Variables to be passed ]
                variables.Append(string.Format("DECLARE @ServiceID INT = {0} ", this.ServiceID));
                variables.Append(string.Format("DECLARE @LoadID INT = NULL "));
                variables.Append(string.Format("DECLARE @Indicator VARCHAR(150) = '{0}' ", Description));
                variables.Append(string.Format("DECLARE @RDate DATETIME = GETDATE()"));
                #endregion

                string query = variables.ToString() + " " +
                                    @"USE SQLPR1

SET NOCOUNT ON

                                        DECLARE @TOTAL INT = 0
                                        SELECT @TOTAL = COUNT(*) FROM [PREP].[PCO_LOAD] WHERE [SERVICEID] = @ServiceID
                                        IF(@TOTAL >= 750)
                                        BEGIN
	
	                                        DECLARE @TempTable TABLE([LOADID] INT,[ROW] INT,[RDATE] DATETIME)

	                                        INSERT INTO @TempTable
	                                        SELECT	[LOADID],ROW_NUMBER() OVER(ORDER BY [RDATE] ASC) AS [ROW],[RDATE]
	                                        FROM [PREP].[PCO_LOAD]
	                                        WHERE [SERVICEID] = @ServiceID

	                                        DECLARE @TempTotal INT = 0
	                                        SELECT @TempTotal = COUNT(*) FROM @TempTable

	                                        DECLARE @DeleteRows INT = 0 
	                                        SET @DeleteRows = (@TempTotal - 750)

	                                        DELETE FROM [PREP].[PCO_LOAD]
	                                        WHERE [SERVICEID] = @ServiceID AND [LOADID] IN (SELECT [LOADID] FROM @TempTable WHERE [ROW] < @DeleteRows)

	                                        --DELETE FROM [PREP].[PCO_OUTBOUNDQUEUE]
	                                        --WHERE [SERVICEID] = @ServiceID AND [LOADID] IN (SELECT [LOADID] FROM @TempTable WHERE [ROW] < @DeleteRows)
                                        END

                                        SELECT @LoadID = MAX([LOADID]) FROM [PREP].[PCO_LOAD] WHERE [SERVICEID] = @ServiceID AND [DESCRIPTION] = @Indicator
                                        IF (ISNULL(@LoadID,0) = 0)
                                        BEGIN
	                                        SELECT @LoadID = ISNULL(MAX([LOADID]),0) FROM [PREP].[PCO_LOAD] WHERE [SERVICEID] = @ServiceID
	                                        SET @LoadID = @LoadID + 1

	                                        INSERT INTO [PREP].[PCO_LOAD]
	                                        ([SERVICEID],[LOADID],[STATUS],[DESCRIPTION],[RDATE],[RECORDCOUNT],[PRIORITYTYPE],[PRIORITYVALUE])
	                                        VALUES
	                                        (@ServiceID,@LoadID,'D',@Indicator,GETDATE(),0,0,'')
                                        END

                                        SELECT TOP 1 [LOADID] 
                                        FROM [PREP].[PCO_LOAD] 
                                        WHERE [SERVICEID] = @ServiceID AND [DESCRIPTION] = @Indicator";

                using (var conn = new SqlConnection(Database.dbPresence))
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    await conn.OpenAsync();
                    cmd.CommandTimeout = 0;
                    cmd.CommandText = query;
                    SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    TempData.Load(reader);
                }

                foreach (DataRow row in TempData.Rows)
                {
                    LoadID = int.Parse(row["LOADID"].ToString());
                }

                return LoadID;
            }
            catch (Exception ex)
            {
                var log = new LogConsoleError(InstanceID, "LoadModel", "CreateLoadID()", ex.Message);
                await log.SaveSync();

                return LoadID;
            }
        }

        public async Task<int> CreateLoadIDAdhocServiceID()
        {
            DataTable TempData = new DataTable();
            int LoadID = 1;
            try
            {
                string Description = this.Partial;

                StringBuilder variables = new StringBuilder();
                #region [ Create all Variables to be passed ]
                variables.Append(string.Format("DECLARE @ServiceID INT = {0} ", this.ServiceID));
                variables.Append(string.Format("DECLARE @LoadID INT = NULL "));
                variables.Append(string.Format("DECLARE @Indicator VARCHAR(150) = '{0}' ", Description));
                variables.Append(string.Format("DECLARE @RDate DATETIME = GETDATE()"));
                #endregion

                string query = variables.ToString() + " " +
                                    @"SET NOCOUNT ON

                                        DECLARE @TOTAL INT = 0
                                        SELECT @TOTAL = COUNT(*) FROM [PREP].[PCO_LOAD] WHERE [SERVICEID] = @ServiceID
                                        IF(@TOTAL >= 750)
                                        BEGIN
	
	                                        DECLARE @TempTable TABLE([LOADID] INT,[ROW] INT,[RDATE] DATETIME)

	                                        INSERT INTO @TempTable
	                                        SELECT	[LOADID],ROW_NUMBER() OVER(ORDER BY [RDATE] ASC) AS [ROW],[RDATE]
	                                        FROM [PREP].[PCO_LOAD]
	                                        WHERE [SERVICEID] = @ServiceID

	                                        DECLARE @TempTotal INT = 0
	                                        SELECT @TempTotal = COUNT(*) FROM @TempTable

	                                        DECLARE @DeleteRows INT = 0 
	                                        SET @DeleteRows = (@TempTotal - 750)

	                                        DELETE FROM [PREP].[PCO_LOAD]
	                                        WHERE [SERVICEID] = @ServiceID AND [LOADID] IN (SELECT [LOADID] FROM @TempTable WHERE [ROW] < @DeleteRows)

	                                        --DELETE FROM [PREP].[PCO_OUTBOUNDQUEUE]
	                                        --WHERE [SERVICEID] = @ServiceID AND [LOADID] IN (SELECT [LOADID] FROM @TempTable WHERE [ROW] < @DeleteRows)
                                        END

                                        SELECT @LoadID = MAX([LOADID]) FROM [PREP].[PCO_LOAD] WHERE [SERVICEID] = @ServiceID AND [DESCRIPTION] = @Indicator AND CONVERT(DATE,[RDATE]) = CONVERT(DATE,@RDate)
                                        IF (ISNULL(@LoadID,0) = 0)
                                        BEGIN
	                                        SELECT @LoadID = ISNULL(MAX([LOADID]),0) FROM [PREP].[PCO_LOAD] WHERE [SERVICEID] = @ServiceID
	                                        SET @LoadID = @LoadID + 1

	                                        INSERT INTO [PREP].[PCO_LOAD]
	                                        ([SERVICEID],[LOADID],[STATUS],[DESCRIPTION],[RDATE],[RECORDCOUNT],[PRIORITYTYPE],[PRIORITYVALUE])
	                                        VALUES
	                                        (@ServiceID,@LoadID,'D',@Indicator,GETDATE(),0,0,'')
                                        END

                                        SELECT TOP 1 [LOADID] 
                                        FROM [PREP].[PCO_LOAD] 
                                        WHERE [SERVICEID] = @ServiceID AND [DESCRIPTION] = @Indicator AND CONVERT(DATE,[RDATE]) = CONVERT(DATE,@RDate)";

                using (var conn = new SqlConnection(Database.dbPresence))
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    await conn.OpenAsync();
                    cmd.CommandTimeout = 0;
                    cmd.CommandText = query;
                    SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    TempData.Load(reader);
                }

                foreach (DataRow row in TempData.Rows)
                {
                    LoadID = int.Parse(row["LOADID"].ToString());
                }

                return LoadID;
            }
            catch (Exception ex)
            {
                var log = new LogConsoleError(InstanceID, "LoadModel", "CreateLoadID()", ex.Message);
                await log.SaveSync();

                return LoadID;
            }
        }
    }
}
