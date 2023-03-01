#region [ Using ]
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
#endregion

namespace CIMWebAPI
{
   public class Startup
   {
      public IConfiguration Configuration { get; }

      #region [ Startup ]
      public Startup(IConfiguration configuration)
      {
         Configuration = configuration;
      }
      #endregion

      #region [ Configure Services ]
      public void ConfigureServices(IServiceCollection services)
      {
         services.AddControllers();
      }
      #endregion

      #region [ Configure ]
      public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
      {
         if (env.IsDevelopment())
         {
            app.UseDeveloperExceptionPage();
         }
         app.UseMiddleware<RequestLoggingMiddleware>();
         app.UseHttpsRedirection();
         app.UseRouting();
         app.UseAuthorization();
         app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
      }
      #endregion
   }
}