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
   public class InovoCIMRepository
   {
      public IConfiguration Config { get; }
      public Database db { get; set; }

      #region [ Default Constructor ]
      public InovoCIMRepository(IConfiguration configuration)
      {
            this.Config = configuration;

            this.db = new Database();
            this.Config.GetSection("Database").Bind(this.db); // CHANGE THIS BACK
            //this.Config.GetSection("Database-Local-BackUp").Bind(this.db); // CHANGE THIS BACK
        }
      #endregion

      //--------------------------------------------------------------//





   }
}
