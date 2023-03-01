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
using Microsoft.Extensions.Logging;
#endregion

namespace CIMWebAPI.Controllers
{
    [ApiController]
    [Route("DoNotCallData")]
    public class DoNotCallDataController : ControllerBase
    {
        public IConfiguration Config { get; }
        private readonly ILogger<DoNotCallDataController> _logger;

        #region [ Default Constructor ]
        public DoNotCallDataController(ILogger<DoNotCallDataController> logger, IConfiguration configuration)
        {
            this._logger = logger;
            this.Config = configuration;
        }
        //    #region [ Default Constructor ]
        //    public DoNotCallDataController(IConfiguration configuration)
        //{
        //    this.Config = configuration;
        //}
        #endregion

        //--------------------------------------------------------------//

        #region [ HttpGet ] - [ Get ]
        public async Task<IActionResult> Get([FromBody] DNC data)
        {
            try
            {
                AuthRepository Auth = new AuthRepository(this.Config, Request.Headers["APIKey"]);
                bool IsAuthorized = await Auth.ValidateKey();
                if (IsAuthorized)
                {
                    InovoCIMRepository Database = new InovoCIMRepository(this.Config);
                    SQLRepository sql = new SQLRepository();
                    data.SetCIMConnString(Database.db.dbInovoCIM);
                    data.SetPresConnString(Database.db.dbPresence);
                    if (data.Phone == String.Empty)
                    {
                        return BadRequest("No Phone Number Provided.");
                    }
                    else
                    {
                        data.Phone = data.Phone.TrimStart('0');
                        if (data.AddIfNotExists == 0)
                        {
                            try
                            {
                                data = await sql.GetDNCListData(data);
                                if(data.DNCListID == String.Empty)
                                {
                                    return BadRequest("Phone number not found in DNC lists");
                                }
                                else
                                {
                                    return new JsonResult(data);
                                }
                            }
                            catch (Exception ex)
                            {
                                return BadRequest(ex.Message);
                            }
                        }
                        else
                        {
                            try
                            {
                                if (await sql.AddDNCIfNotExists(data))
                                {
                                    return Ok(200);
                                }
                                else
                                {
                                    return BadRequest("Phone number already in DNCList");
                                }
                            }
                            catch (Exception ex)
                            {
                                return BadRequest(ex.Message);
                            }
                        }
                    }
                }
                else
                {
                    return BadRequest("Bad Authorization");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion
    }
}