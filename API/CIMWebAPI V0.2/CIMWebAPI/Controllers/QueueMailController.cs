#region [ Using ]
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CIMWebAPI.Data.Entities;
using CIMWebAPI.Data.Models;
using CIMWebAPI.DataRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
#endregion

namespace CIMWebAPI.Controllers
{
   [ApiController]
   [Route("QueueMail")]
   public class QueueMailController : ControllerBase
   {
      public IConfiguration Config { get; }

      #region [ Default Constructor ]
      public QueueMailController(IConfiguration configuration)
      {
         this.Config = configuration;
      }
      #endregion

      //--------------------------------------------------------------// 

      #region [HttpPost] - [ Post ]
      [HttpPost]
      public async Task<IActionResult> Post([FromBody] OutQueuePhone data)
      {
         try
         {
            AuthRepository Auth = new AuthRepository(this.Config, Request.Headers["APIKey"]);
            bool IsAuthorized = await Auth.ValidateKey();
            if (IsAuthorized)
            {



               return Ok(200);
            }
            else
            {
               return BadRequest();
            }
         }
         catch (Exception ex)
         {
            return BadRequest();
         }
      }
      #endregion
   }
}
