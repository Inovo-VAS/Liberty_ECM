using CIMWebAPI.DataRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CIMWebAPI.Controllers
{
    [Route("Priority")]
    [ApiController]
    public class PriorityController : Controller
    {
        public IConfiguration Config { get; }
        private readonly ILogger<LoadController> _logger;
        #region [ Default Constructor ]
        public PriorityController(ILogger<LoadController> logger, IConfiguration configuration)
        {
            this._logger = logger;
            this.Config = configuration;
        }
        #endregion

        #region [ HttpPost - Post ]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Data.Entities.Priority data)
        {
            TableRepository tableRep = new TableRepository();
            DataTable APILog = tableRep.ColumnTableQueuePhoneController(new DataTable());
            DataRow APILogRow = APILog.NewRow();
            APILogRow["Received"] = DateTime.Now;
            APILogRow["Input"] = JsonConvert.SerializeObject(data);
            SQLRepository sqlRepo = new SQLRepository();
            InovoCIMRepository Database = new InovoCIMRepository(this.Config);
            sqlRepo.ConnStringPresence = Database.db.dbPresence;
            sqlRepo.ConnStringCIM = Database.db.dbInovoCIM;
            try
            {
                AuthRepository Auth = new AuthRepository(this.Config, Request.Headers["APIKey"]);
                bool IsAuthorized = await Auth.ValidateKey();
                if (IsAuthorized)
                {
                    if (data.ServiceID == 0)
                    {
                        APILogRow["ReturnMessage"] = "Bad Request Status 400 - No Service ID Provided";
                        APILogRow["ReturnTime"] = DateTime.Now;
                        APILog.Rows.Add(APILogRow);
                        await sqlRepo.LogAPI(APILog, Database.db.dbInovoCIM);
                        return BadRequest("No Service ID provided.");
                    }
                    else if (data.PriorityCommand == String.Empty && data.PriorityCommand.ToLower() != "lifo" && data.PriorityCommand.ToLower() != "fifo")
                    {
                        APILogRow["ReturnMessage"] = "Bad Request Status 400 - Invalid Command Provided";
                        APILogRow["ReturnTime"] = DateTime.Now;
                        APILog.Rows.Add(APILogRow);
                        await sqlRepo.LogAPI(APILog, Database.db.dbInovoCIM);
                        return BadRequest("Invalid Command Provided.");
                    }
                    else
                    {
                        await sqlRepo.SetServicePriority(data);
                        return Ok(200);
                    }
                }
                else
                {
                    APILogRow["ReturnMessage"] = "Bad Request Status 400 - Not Authorized";
                    APILogRow["ReturnTime"] = DateTime.Now;
                    APILog.Rows.Add(APILogRow);
                    await sqlRepo.LogAPI(APILog, Database.db.dbInovoCIM);
                    return BadRequest("Not Authorized");
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex.Message);
                APILogRow["ReturnMessage"] = "Bad Request Status 400 - " + ex.Message;
                APILogRow["ReturnTime"] = DateTime.Now;
                APILog.Rows.Add(APILogRow);
                await sqlRepo.LogAPI(APILog, Database.db.dbInovoCIM);
                return BadRequest(ex.Message);
            }
        }
        #endregion

        //#region [ HttpGet - Get ]
        //[HttpGet]
        //public async Task<IActionResult> Get(int ServiceID)
        //{
        //    TableRepository tableRep = new TableRepository();
        //    DataTable APILog = tableRep.ColumnTableQueuePhoneController(new DataTable());
        //    DataRow APILogRow = APILog.NewRow();
        //    APILogRow["Received"] = DateTime.Now;
        //    APILogRow["Input"] = JsonConvert.SerializeObject(ServiceID);
        //    SQLRepository sqlRepo = new SQLRepository();
        //    InovoCIMRepository Database = new InovoCIMRepository(this.Config);
        //    sqlRepo.ConnStringPresence = Database.db.dbPresence;
        //    sqlRepo.ConnStringCIM = Database.db.dbInovoCIM;
        //    try
        //    {
        //        AuthRepository Auth = new AuthRepository(this.Config, Request.Headers["APIKey"]);
        //        bool IsAuthorized = await Auth.ValidateKey();
        //        if (IsAuthorized)
        //        {
        //            //if (ServiceID != 0)
        //            //{
        //            //    string outResult = await sqlRepo.GetServicePriority(ServiceID, Database.db.dbInovoCIM);
        //            //    return new JsonResult(outResult);
        //            //}
        //            //else
        //            //{
        //            //    APILogRow["ReturnMessage"] = "Bad Request Status 400 - No Service ID Provided";
        //            //    APILogRow["ReturnTime"] = DateTime.Now;
        //            //    APILog.Rows.Add(APILogRow);
        //            //    await sqlRepo.LogAPI(APILog, Database.db.dbInovoCIM);
        //            //    return BadRequest("No Service ID provided.");
        //            //}
        //        }
        //        else
        //        {
        //            APILogRow["ReturnMessage"] = "Bad Request Status 400 - Not Authorized";
        //            APILogRow["ReturnTime"] = DateTime.Now;
        //            APILog.Rows.Add(APILogRow);
        //            await sqlRepo.LogAPI(APILog, Database.db.dbInovoCIM);
        //            return BadRequest("Not Authorized");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        this._logger.LogError(ex.Message);
        //        APILogRow["ReturnMessage"] = "Bad Request Status 400 - " + ex.Message;
        //        APILogRow["ReturnTime"] = DateTime.Now;
        //        APILog.Rows.Add(APILogRow);
        //        await sqlRepo.LogAPI(APILog, Database.db.dbInovoCIM);
        //        return BadRequest(ex.Message);
        //    }
        //    #endregion
        //}
    }
}