#region [ Using ]
using CIMWebAPI.Data.Entities;
using CIMWebAPI.DataRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Serialization;
using Microsoft.Extensions.Logging;
using System.Text.Encodings.Web;
using Newtonsoft.Json;
#endregion

namespace CIMWebAPI.Controllers
{
    [ApiController]
    [Route("QueuePhone")]
    public class QueuePhoneController : ControllerBase
    {
        public IConfiguration Config { get; }
        private readonly ILogger<QueuePhoneController> _logger;

        #region [ Default Constructor ]
        public QueuePhoneController(ILogger<QueuePhoneController> logger, IConfiguration configuration)
        {
            this._logger = logger;
            this.Config = configuration;
        }
        #endregion

        //--------------------------------------------------------------//

        #region [HttpPost] - [ Post ]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] OutQueuePhone data)
        {

            //_logger.LogInformation("JSON Packet Received - \n\n" + data.ToString());
            TableRepository tableRep = new TableRepository();
            DataTable APILog = tableRep.ColumnTableQueuePhoneController(new DataTable());
            DataRow APILogRow = APILog.NewRow();
            APILogRow["Received"] = DateTime.Now;
            APILogRow["Input"] = JsonConvert.SerializeObject(data);
            if (data.SourceID != 0)
            {
                APILogRow["SourceID"] = data.SourceID;
            }
            try
            {
                bool isDev = true;
                SQLRepository sqlRepo = new SQLRepository();

                AuthRepository Auth = new AuthRepository(this.Config, Request.Headers["APIKey"]);
                bool IsAuthorized = await Auth.ValidateKey();
                if (IsAuthorized)
                {
                    InovoCIMRepository Database = new InovoCIMRepository(this.Config);
                    data.connStringCIM = Database.db.dbInovoCIM;
                    data.connStringPresence = Database.db.dbPresence;
                    try
                    {
                        sqlRepo.TestConnection(data);
                    }
                    catch (Exception ex)
                    {
                        this._logger.LogError(ex.Message);
                    }
                    if (data.Command.ToLower() == "fifo")
                    {
                        if (data.ServiceID == 0)
                        {
                            APILogRow["ReturnMessage"] = "Bad Request Status 400 - No ServiceID provided";
                            APILogRow["ReturnTime"] = DateTime.Now;
                            APILog.Rows.Add(APILogRow);
                            await sqlRepo.LogAPI(APILog, data.connStringCIM);
                            return BadRequest("No ServiceID provided");
                        }
                        else
                        {
                            try
                            {
                                string tempString = await sqlRepo.FirstInFirstOut(data);
                                if (tempString != String.Empty)
                                {
                                    APILogRow["ReturnMessage"] = "Bad Request Status 400 - First In First Out Failed";
                                    APILogRow["ReturnTime"] = DateTime.Now;
                                    APILog.Rows.Add(APILogRow);
                                    await sqlRepo.LogAPI(APILog, data.connStringCIM);
                                    return BadRequest("First In First Out Failed");
                                }
                            }
                            catch (Exception ex)
                            {
                                this._logger.LogError(ex.Message);
                                APILogRow["ReturnMessage"] = "Bad Request Status 400 - " + ex.Message;
                                APILogRow["ReturnTime"] = DateTime.Now;
                                APILog.Rows.Add(APILogRow);
                                await sqlRepo.logError(data, "OutQueuePhone", "FIFO", ex.Message);
                                await sqlRepo.LogAPI(APILog, data.connStringCIM);
                                return BadRequest(ex.Message);
                            }
                        }
                    }
                    else
                    {
                        if (data.LoadID == 0 && String.IsNullOrEmpty(data.leadProvider) && data.Command.ToLower() == "addcall")
                        {
                            APILogRow["ReturnMessage"] = "Bad Request Status 400 - No Load ID or Lead Provider provided.";
                            APILogRow["ReturnTime"] = DateTime.Now;
                            APILog.Rows.Add(APILogRow);
                            await sqlRepo.LogAPI(APILog, data.connStringCIM);
                            return BadRequest("No Load ID or Batch Number provided.");
                        }
                        else if ((data.Command.ToLower() == "modifycall") && (data.LoadID == 0 || data.LoadIDOld == 0 || data.ServiceID == 0 || data.ServiceIDOld == 0))
                        {
                            APILogRow["ReturnMessage"] = "Bad Request Status 400 - Not enough information provided for LoadID's or ServiceID's.";
                            APILogRow["ReturnTime"] = DateTime.Now;
                            APILog.Rows.Add(APILogRow);
                            await sqlRepo.LogAPI(APILog, data.connStringCIM);
                            return BadRequest("Not enough information provided for LoadID's or ServiceID's.");
                        }
                        else if (data.ServiceID == 0)
                        {
                            APILogRow["ReturnMessage"] = "Bad Request Status 400 - No Service ID provided.";
                            APILogRow["ReturnTime"] = DateTime.Now;
                            APILog.Rows.Add(APILogRow);
                            await sqlRepo.LogAPI(APILog, data.connStringCIM);
                            return BadRequest("No Service ID provided.");
                        }
                        else if (data.SourceID == 0)
                        {
                            APILogRow["ReturnMessage"] = "Bad Request Status 400 - No Source ID provided.";
                            APILogRow["ReturnTime"] = DateTime.Now;
                            APILog.Rows.Add(APILogRow);
                            await sqlRepo.LogAPI(APILog, data.connStringCIM);
                            return BadRequest("No Source ID provided.");
                        }
                        else if (data.Command == "")
                        {
                            APILogRow["ReturnMessage"] = "Bad Request Status 400 - No command provided.";
                            APILogRow["ReturnTime"] = DateTime.Now;
                            APILog.Rows.Add(APILogRow);
                            await sqlRepo.LogAPI(APILog, data.connStringCIM);
                            return BadRequest("No command provided.");
                        }
                        else if ((data.Phone == "" || data.Phone == null) && (data.Phone1 == "" || data.Phone1 == null))
                        {
                            APILogRow["ReturnMessage"] = "Bad Request Status 400 - No primary contact number provided.";
                            APILogRow["ReturnTime"] = DateTime.Now;
                            APILog.Rows.Add(APILogRow);
                            await sqlRepo.LogAPI(APILog, data.connStringCIM);
                            return BadRequest("No primary contact number provided.");
                        }
                        //if (!(await sqlRepo.CheckPrimaryPhoneDNC(data)))
                        //{
                        //    return BadRequest("Number in DNC");
                        //}
                        DataTable Presence = new DataTable();
                        DataTable CIM = new DataTable();
                        DataTable API = new DataTable();
                        DataTable Load = new DataTable();

                        Load = tableRep.ColumnTableLoad(Load);
                        Presence = tableRep.ColumnTablePresence(Presence);
                        CIM = tableRep.ColumnTableQueuePhoneComplete(CIM);
                        API = tableRep.ColumnTableAPITable(API);

                        string ConnStringPresence = Database.db.dbPresence;     //"Data Source=10.122.143.224;Initial Catalog=SQLPR1;User Id=PREP;Password=PREP;MultipleActiveResultSets=true;";
                        string ConnStringCIM = Database.db.dbInovoCIM;          //"Data Source=10.122.143.224;Initial Catalog=CIM;User Id=cim_user;Password=Gr!@#$tj!@#$2019;MultipleActiveResultSets=true;";

                        try
                        {
                            if (data.Command.ToLower() == "addcall")
                            {

                                //APIRepository api = new APIRepository();
                                //return BadRequest(api.BufferReload(data.ServiceID, this.Config.GetValue<string>("Presence-Server-URI:URI")));
                                try
                                {
                                    if (!String.IsNullOrEmpty(data.leadProvider) && data.LoadID == 0)
                                    {
                                        data = await data.GetBatch(isDev);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    this._logger.LogError(ex.Message);
                                    APILogRow["ReturnMessage"] = "Bad Request Status 400 - " + ex.Message;
                                    APILogRow["ReturnTime"] = DateTime.Now;
                                    APILog.Rows.Add(APILogRow);
                                    await sqlRepo.logError(data, "OutQueuePhone", "GetBatch", ex.Message);
                                    await sqlRepo.LogAPI(APILog, data.connStringCIM);
                                    return BadRequest(ex.Message);
                                }
                                if (sqlRepo.PresenceDuplicate(data.SourceID, data.LoadID, data.ServiceID, ConnStringPresence))
                                {
                                    #region [ Map Table Load ]
                                    Load = tableRep.MapLoadTable(Load, data);
                                    #endregion

                                    Presence = tableRep.MapTablePresence(Presence, data);
                                    try
                                    {
                                        //string ServicePriority = (await sqlRepo.GetServicePriority(data.ServiceID, data.connStringCIM)).ToLower();
                                        #region[ Update Presence Priorities LIFO ]
                                        //if (ServicePriority.ToLower() == "lifo")
                                        //{
                                            //await sqlRepo.ApplyLIFO(data);
                                        //}
                                        //else if(ServicePriority.ToLower() == "fifo")
                                        //{
                                        //    int NewPriority = await sqlRepo.FIFOPriority(data);
                                        //    Presence.Rows[0]["PRIORITY"] = NewPriority;
                                        //}
                                    }
                                    catch (Exception ex)
                                    {
                                        this._logger.LogError(ex.Message);
                                        APILogRow["ReturnMessage"] = "Bad Request Status 400 - " + ex.Message;
                                        APILogRow["ReturnTime"] = DateTime.Now;
                                        APILog.Rows.Add(APILogRow);
                                        await sqlRepo.logError(data, "OutQueuePhone", "Apply LIFO", ex.Message);
                                        await sqlRepo.LogAPI(APILog, data.connStringCIM);
                                        return BadRequest(ex.Message);
                                    }
                                    #endregion
                                    #region [ Bulk Send to Presence ]
                                    try
                                    {
                                        if (Presence.Rows[0]["PHONE"] != DBNull.Value || Presence.Rows[0]["PHONE1"] != DBNull.Value || Presence.Rows[0]["PHONE2"] != DBNull.Value || Presence.Rows[0]["PHONE3"] != DBNull.Value || Presence.Rows[0]["PHONE4"] != DBNull.Value || Presence.Rows[0]["PHONE5"] != DBNull.Value || Presence.Rows[0]["PHONE6"] != DBNull.Value || Presence.Rows[0]["PHONE7"] != DBNull.Value || Presence.Rows[0]["PHONE8"] != DBNull.Value || Presence.Rows[0]["PHONE9"] != DBNull.Value || Presence.Rows[0]["PHONE10"] != DBNull.Value)
                                        {
                                            using (SqlBulkCopy sqlBulk = new SqlBulkCopy(ConnStringPresence))
                                            {
                                                sqlBulk.DestinationTableName = "[PREP].[PCO_OUTBOUNDQUEUE]";
                                                sqlBulk.BatchSize = 1;
                                                sqlBulk.WriteToServer(Presence);
                                            }
                                        }
                                        else
                                        {
                                            APILogRow["ReturnMessage"] = "Bad Request On Presence Insert - No Phone Numbers";
                                            APILogRow["ReturnTime"] = DateTime.Now;
                                            APILog.Rows.Add(APILogRow);
                                            await sqlRepo.logError(data, "OutQueuePhone", "Send To Outbound Queue", "Bad Insert - No Phone Numbers");
                                            await sqlRepo.LogAPI(APILog, data.connStringCIM);
                                            return BadRequest("Bad Insert - Invalid Phone Numbers");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        this._logger.LogError(ex.Message);
                                        APILogRow["ReturnMessage"] = "Bad Request Status 400 - " + ex.Message;
                                        APILogRow["ReturnTime"] = DateTime.Now;
                                        APILog.Rows.Add(APILogRow);
                                        await sqlRepo.logError(data, "OutQueuePhone", "Send To Outbound Queue", ex.Message);
                                        await sqlRepo.LogAPI(APILog, data.connStringCIM);
                                        return BadRequest(ex.Message);
                                    }
                                    #endregion

                                }
                                else
                                {
                                    APILogRow["ReturnMessage"] = "Bad Request Status 400 - Is Presence Duplicate";
                                    APILogRow["ReturnTime"] = DateTime.Now;
                                    APILog.Rows.Add(APILogRow);
                                    await sqlRepo.LogAPI(APILog, data.connStringCIM);
                                    return BadRequest("Is Presence Duplicate");
                                }
                            }
                            else if (data.Command.ToLower() == "removecall")
                            {
                                if (data.SourceID == 0 || data.LoadID == 0 || data.ServiceID == 0)
                                {
                                    APILogRow["ReturnMessage"] = "Bad Request Status 400 - SourceID or LoadID or ServiceID not provided.";
                                    APILogRow["ReturnTime"] = DateTime.Now;
                                    APILog.Rows.Add(APILogRow);
                                    await sqlRepo.LogAPI(APILog, data.connStringCIM);
                                    return BadRequest("SourceID or LoadID or ServiceID not provided.");
                                }
                                else if (sqlRepo.PresenceDuplicate(data.SourceID, data.LoadID, data.ServiceID, ConnStringPresence))
                                {
                                    APILogRow["ReturnMessage"] = "Bad Request Status 400 - Record does not exist in Presence.";
                                    APILogRow["ReturnTime"] = DateTime.Now;
                                    APILog.Rows.Add(APILogRow);
                                    await sqlRepo.LogAPI(APILog, data.connStringCIM);
                                    return BadRequest("Record does not exist in Presence.");
                                }
                                else
                                {
                                    await sqlRepo.RemoveFromPresence(data);
                                }
                            }
                            else if (data.Command.ToLower() == "modifycallresetall")
                            {
                                if (sqlRepo.PresenceDuplicate(data.SourceID, data.LoadIDOld, data.ServiceIDOld, ConnStringPresence))
                                {
                                    APILogRow["ReturnMessage"] = "Bad Request Status 400 - Record does not exist in Presence.";
                                    APILogRow["ReturnTime"] = DateTime.Now;
                                    APILog.Rows.Add(APILogRow);
                                    await sqlRepo.LogAPI(APILog, data.connStringCIM);
                                    return BadRequest("Record does not exist in Presence.");
                                }
                                else
                                {
                                    try
                                    {
                                        await sqlRepo.ModifyQueueResetAll(data);
                                    }
                                    catch (Exception ex)
                                    {
                                        this._logger.LogError(ex.Message);
                                        APILogRow["ReturnMessage"] = "Bad Request Status 400 - " + ex.Message;
                                        APILogRow["ReturnTime"] = DateTime.Now;
                                        APILog.Rows.Add(APILogRow);
                                        await sqlRepo.logError(data, "OutQueuePhone", "ModifyQueueResetAll", ex.Message);
                                        await sqlRepo.LogAPI(APILog, data.connStringCIM);
                                        return BadRequest(ex.Message);
                                    }
                                }
                            }
                            else if (data.Command.ToLower() == "modifycall")
                            {
                                if (sqlRepo.PresenceDuplicate(data.SourceID, data.LoadIDOld, data.ServiceIDOld, ConnStringPresence))
                                {
                                    APILogRow["ReturnMessage"] = "Bad Request Status 400 - Record does not exist in Presence.";
                                    APILogRow["ReturnTime"] = DateTime.Now;
                                    APILog.Rows.Add(APILogRow);
                                    await sqlRepo.LogAPI(APILog, data.connStringCIM);
                                    return BadRequest("Record does not exist in Presence.");
                                }
                                else
                                {
                                    try
                                    {
                                        await sqlRepo.ModifyQueue(data);
                                    }
                                    catch (Exception ex)
                                    {
                                        this._logger.LogError(ex.Message);
                                        APILogRow["ReturnMessage"] = "Bad Request Status 400 - Record does not exist in Presence.";
                                        APILogRow["ReturnTime"] = DateTime.Now;
                                        APILog.Rows.Add(APILogRow);
                                        await sqlRepo.LogAPI(APILog, data.connStringCIM);
                                        return BadRequest(ex.Message);
                                    }
                                }
                            }
                            #region [ Map Table Queue Complete ]
                            tableRep.MapQueuePhoneComplete(CIM, data);
                            #endregion

                            #region [ Bulk Send to CIM ]
                            using (SqlBulkCopy sqlBulk = new SqlBulkCopy(ConnStringCIM))
                            {
                                if(isDev)
                                {
                                    sqlBulk.DestinationTableName = "[ECM].[QueuePhoneCompleteDev]";
                                }
                                else if(!isDev)
                                {
                                    sqlBulk.DestinationTableName = "[ECM].[QueuePhoneComplete]";
                                }
                                sqlBulk.BatchSize = 1;
                                sqlBulk.WriteToServer(CIM);
                            }
                            #endregion

                            #region [ Map Table API Table ]
                            tableRep.MapQueueAPI(API, data);
                            #endregion

                            #region [ Bulk send to API Table ]
                            using (SqlBulkCopy sqlBulk_ = new SqlBulkCopy(ConnStringCIM))
                            {
                                if (isDev)
                                {
                                    sqlBulk_.DestinationTableName = "[ECM].[APITableDev]";
                                }
                                else
                                {
                                    sqlBulk_.DestinationTableName = "[ECM].[APITable]";
                                }
                                sqlBulk_.BatchSize = 1;
                                sqlBulk_.WriteToServer(API);
                            }
                            #endregion

                        }
                        catch (Exception ex)
                        {
                            this._logger.LogError(ex.Message);
                            APILogRow["ReturnMessage"] = "Bad Request Status 400 - " + ex.Message;
                            APILogRow["ReturnTime"] = DateTime.Now;
                            await sqlRepo.logError(data, "OutQueuePhone", "Controller", ex.Message);
                            APILog.Rows.Add(APILogRow);
                            await sqlRepo.LogAPI(APILog, data.connStringCIM);
                            return BadRequest(ex.Message);
                        }
                    }
                    APILogRow["ReturnMessage"] = "Status Code 200 - OK";
                    APILogRow["ReturnTime"] = DateTime.Now;
                    APILog.Rows.Add(APILogRow);
                    await sqlRepo.LogAPI(APILog, data.connStringCIM);
                    return Ok(200);
                }
                else
                {
                    APILogRow["ReturnMessage"] = "Bad Request Status 400 - Bad Authorization";
                    APILogRow["ReturnTime"] = DateTime.Now;
                    APILog.Rows.Add(APILogRow);
                    await sqlRepo.LogAPI(APILog, data.connStringCIM);
                    return BadRequest("Bad Authorization");
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex.Message);
                APILogRow["ReturnMessage"] = "Bad Request Status 400 - " + ex.Message;
                APILogRow["ReturnTime"] = DateTime.Now;
                APILog.Rows.Add(APILogRow);
                SQLRepository sqlRepo = new SQLRepository();
                await sqlRepo.logError(data, "OutQueuePhone", "Controller", ex.Message);
                await sqlRepo.LogAPI(APILog, data.connStringCIM);
                return BadRequest(ex);
            }
        }
        #endregion
    }
}