#region [ Using ]
using System;
using System.Collections.Generic;
using System.Text;
#endregion

namespace InovoCIM.Data.Entities
{
    public class FileReportModel
    {
        public string InstanceID { get; set; }
        public string FileName { get; set; }
        public DateTime Received { get; set; }



    }
}
