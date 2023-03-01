#region [ Using ]
using CIMWebAPI.Data.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace CIMWebAPI.DataRepository
{
   public class AuthRepository
   {
      public IConfiguration Config { get; }
      public string InputAPIKey { get; set; }
      public string APIKey { get; set; }

      #region [ Default Constructor ]
      public AuthRepository(IConfiguration configuration, string inputAPIKey)
      {
         this.InputAPIKey = inputAPIKey;

         this.Config = configuration;
         this.APIKey = this.Config.GetValue<string>("API-Key:Key");
      }
      #endregion

      //--------------------------------------------------------------//

      #region [ Validate Key ]
      public async Task<bool> ValidateKey()
      {
         try
         {
            return (this.InputAPIKey == this.APIKey) ? true : false;
         }
         catch (Exception)
         {
            return false;
         }
      }
      #endregion
   }
}