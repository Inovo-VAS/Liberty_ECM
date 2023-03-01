#region
using CIMWebAPI.Data.Entities;
using CIMWebAPI.DataRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
#endregion

namespace CIMWebAPI.Controllers
{
    [ApiController]
    [Route("Load")]
    public class LoadController : Controller
    {
        public IConfiguration Config { get; }
        private readonly ILogger<LoadController> _logger;

        #region [ Default Constructor ]
        public LoadController(ILogger<LoadController> logger, IConfiguration configuration)
        {
            this._logger = logger;
            this.Config = configuration;
        }
        #endregion
        //#region [ Default Constructor ]
        //public LoadController(IConfiguration configuration)
        //{
        //    this.Config = configuration;
        //}
        //#endregion
        //--------------------------------------------------------------//

        #region [HttpPost] - [ Post ]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Load data)
        {
            try
            {
                AuthRepository Auth = new AuthRepository(this.Config, Request.Headers["APIKey"]);
                bool IsAuthorized = await Auth.ValidateKey();
                if (IsAuthorized)
                {
                    InovoCIMRepository Database = new InovoCIMRepository(this.Config);
                    SQLRepository sqlRepo = new SQLRepository();
                    data.connStringPresence = Database.db.dbPresence;
                    if (data.LoadID == 0 || data.ServiceID == 0)
                    {
                        return BadRequest("No Load ID or Service ID provided.");
                    }
                    else if (data.LoadName == "")
                    {
                        return BadRequest("No Load Name provided.");
                    }
                    else if (data.Command == String.Empty)
                    {
                        return BadRequest("No Command Provided.");
                    }
                    if(data.Command.ToLower() == "addload")
                    {
                        try
                        {
                            if (await sqlRepo.CheckLoad(data) && data.CreateIfExists == 0)
                            {
                                return BadRequest("Load already exists in service.");
                            }
                            else if (await sqlRepo.CheckLoad(data) && data.CreateIfExists == 1)
                            {
                                data = await sqlRepo.GetNextLoadID(data);
                                await sqlRepo.CreateNewLoad(data);
                            }
                        }
                        catch(Exception ex)
                        {
                            return BadRequest(ex.Message);
                        }
                    }
                    else if(data.Command.ToLower() == "modifyload")
                    {
                        try
                        {
                            if (await sqlRepo.CheckLoad(data))
                            {
                                await sqlRepo.AlterLoad(data);
                            }
                            else
                            {
                                return BadRequest("Load does not exist in service.");
                            }
                        }
                        catch (Exception ex)
                        {
                            return BadRequest(ex.Message);
                        }
                    }
                    
                }
                return Ok(200);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion
    }
}