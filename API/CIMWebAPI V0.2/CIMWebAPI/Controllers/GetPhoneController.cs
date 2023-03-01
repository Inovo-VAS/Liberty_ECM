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
    [Route("GetPhone")]
    public class GetPhoneController : ControllerBase
    {
        public IConfiguration Config { get; }
        private readonly ILogger<GetPhoneController> _logger;

        #region [ Default Constructor ]
        public GetPhoneController(ILogger<GetPhoneController> logger, IConfiguration configuration)
        {
            this._logger = logger;
            this.Config = configuration;
        }
        #endregion
        //#region [ Default Constructor ]
        //public GetPhoneController(IConfiguration configuration)
        //{
        //    this.Config = configuration;
        //}
        //#endregion

        //--------------------------------------------------------------//

        #region [ HttpGet ] - [ Get ]
        public async Task<IActionResult> Get([FromBody] OutQueuePhone data)
        {
            try
            {
                AuthRepository Auth = new AuthRepository(this.Config, Request.Headers["APIKey"]);
                bool IsAuthorized = await Auth.ValidateKey();
                if (IsAuthorized)
                {
                    InovoCIMRepository Database = new InovoCIMRepository(this.Config);
                    data.connStringCIM = Database.db.dbInovoCIM;
                    data.connStringPresence = Database.db.dbPresence;
                    if (data.SourceID == 0 || data.LoadID == 0 || data.ServiceID == 0)
                    {
                        return BadRequest("Primary key not provided (SourceID, ServiceID, LoadID).");
                    }
                    else
                    {
                        SQLRepository sql = new SQLRepository();
                        if (!sql.PresenceDuplicate(data.SourceID, data.LoadID, data.ServiceID, data.connStringPresence))
                        {
                            data = await sql.GetPresenceContactData(data);
                            return new JsonResult(data);
                        }
                        else
                        {
                            return BadRequest("Record does not exist in Presence.");
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