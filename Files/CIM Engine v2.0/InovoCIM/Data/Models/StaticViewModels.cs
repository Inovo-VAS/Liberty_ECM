#region [ Using ]
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
#endregion

namespace InovoCIM.Data.Models
{
    public static class Database
    {
        public static string dbInovoCIM { get; set; }
        public static string dbPresence { get; set; }

        public static string dbSchema { get; set; }
    }
}
