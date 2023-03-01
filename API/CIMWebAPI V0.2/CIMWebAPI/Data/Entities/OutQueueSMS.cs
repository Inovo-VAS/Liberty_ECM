#region [ Using ]
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace CIMWebAPI.Data.Entities
{
   public class OutQueueSMS
   {
      public int OutQueueSMSID { get; set; }
      public string Input { get; set; }
      public string InputName { get; set; }
      public string Status { get; set; }
      public DateTime Received { get; set; }
      public DateTime NextExecute { get; set; }
      public DateTime? Actioned { get; set; }
      public int RetryCount { get; set; }
      public DateTime? RetryDate { get; set; }
      public int PersonID { get; set; }
      public string SendTo { get; set; }
      public string Message { get; set; }
      public int ConfigurationSMSID { get; set; }
      public int TemplateSMSID { get; set; }
   }
}
