#region [ Using ]
using InovoCIM.Data.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace InovoCIM.Data.Entities
{
    public class PhoneModel
    {
        public string InstanceID { get; set; }
        public string PH01 { get; set; }
        public string PH02 { get; set; }
        public string PH03 { get; set; }
        public string PH04 { get; set; }
        public string PH05 { get; set; }
        public string PH06 { get; set; }
        public string PH07 { get; set; }
        public string PH08 { get; set; }
        public string PH09 { get; set; }
        public string PH10 { get; set; }

        public PhoneModel() { }
        public PhoneModel(string _InstanceID, string _PH01, string _PH02, string _PH03, string _PH04, string _PH05, string _PH06, string _PH07, string _PH08, string _PH09, string _PH10)
        {
            this.InstanceID = _InstanceID;
            this.PH01 = _PH01;
            this.PH02 = _PH02;
            this.PH03 = _PH03;
            this.PH04 = _PH04;
            this.PH05 = _PH05;
            this.PH06 = _PH06;
            this.PH07 = _PH07;
            this.PH08 = _PH08;
            this.PH09 = _PH09;
            this.PH10 = _PH10;
        }

        public PhoneModel(string _InstanceID, string _PH01, string _PH02, string _PH03)
        {
            this.InstanceID = _InstanceID;
            this.PH01 = _PH01;
            this.PH02 = "";
            this.PH03 = "";
            this.PH04 = "";
            this.PH05 = "";
            this.PH06 = "";
            this.PH07 = "";
            this.PH08 = "";
            this.PH09 = "";
            this.PH10 = "";
        }

        public PhoneModel(string _InstanceID, string _PH01, string _PH02, string _PH03, string _PH04)
        {
            this.InstanceID = _InstanceID;
            this.PH01 = _PH01;
            this.PH02 = _PH02;
            this.PH03 = _PH03;
            this.PH04 = _PH04;
            this.PH05 = "";
            this.PH06 = "";
            this.PH07 = "";
            this.PH08 = "";
            this.PH09 = "";
            this.PH10 = "";
        }

        public async Task<PhoneModel> GetPhoneNumbers()
        {
            try
            {
                DataTable TempData = new DataTable();

                StringBuilder variables = new StringBuilder();
                #region [ Create all Variables to be passed ]
                variables.Append(string.Format("DECLARE @PH01 VARCHAR(30) = '{0}' ", this.PH01));
                variables.Append(string.Format("DECLARE @PH02 VARCHAR(30) = '{0}' ", this.PH02));
                variables.Append(string.Format("DECLARE @PH03 VARCHAR(30) = '{0}' ", this.PH03));
                variables.Append(string.Format("DECLARE @PH04 VARCHAR(30) = '{0}' ", this.PH04));
                variables.Append(string.Format("DECLARE @PH05 VARCHAR(30) = '{0}' ", this.PH05));
                variables.Append(string.Format("DECLARE @PH06 VARCHAR(30) = '{0}' ", this.PH06));
                variables.Append(string.Format("DECLARE @PH07 VARCHAR(30) = '{0}' ", this.PH07));
                variables.Append(string.Format("DECLARE @PH08 VARCHAR(30) = '{0}' ", this.PH08));
                variables.Append(string.Format("DECLARE @PH09 VARCHAR(30) = '{0}' ", this.PH09));
                variables.Append(string.Format("DECLARE @PH10 VARCHAR(30) = '{0}' ", this.PH10));
                #endregion

                string query = variables.ToString() + " " +
                                @"SELECT ISNULL([ECM].[CIMNumber](@PH01),'') AS 'PH01',ISNULL([ECM].[CIMNumber](@PH02),'') AS 'PH02',ISNULL([ECM].[CIMNumber](@PH03),'') AS 'PH03',
	                               ISNULL([ECM].[CIMNumber](@PH04),'') AS 'PH04',ISNULL([ECM].[CIMNumber](@PH05),'') AS 'PH05',ISNULL([ECM].[CIMNumber](@PH06),'') AS 'PH06',
	                               ISNULL([ECM].[CIMNumber](@PH07),'') AS 'PH07',ISNULL([ECM].[CIMNumber](@PH08),'') AS 'PH08',ISNULL([ECM].[CIMNumber](@PH09),'') AS 'PH09',
	                               ISNULL([ECM].[CIMNumber](@PH10),'') AS 'PH10'";

                using (var conn = new SqlConnection(Database.dbInovoCIM))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandText = query;
                        SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        TempData.Load(reader);
                    }
                    conn.Close();
                }

                foreach (DataRow row in TempData.Rows)
                {
                    this.PH01 = row["PH01"].ToString();
                    this.PH02 = row["PH02"].ToString();
                    this.PH03 = row["PH03"].ToString();
                    this.PH04 = row["PH04"].ToString();
                    this.PH05 = row["PH05"].ToString();
                    this.PH06 = row["PH06"].ToString();
                    this.PH07 = row["PH07"].ToString();
                    this.PH08 = row["PH08"].ToString();
                    this.PH09 = row["PH09"].ToString();
                    this.PH10 = row["PH10"].ToString();
                }

                return this;
            }
            catch (Exception ex)
            {
                var log = new LogConsoleError(InstanceID, "PhoneModel", "GetPhoneNumbers()", ex.Message);
                await log.SaveSync();

                return this;
            }
        }

        public async Task<string> GetStatus()
        {
            try
            {
                if (string.IsNullOrEmpty(this.PH01) && string.IsNullOrEmpty(this.PH02) && string.IsNullOrEmpty(this.PH03) && string.IsNullOrEmpty(this.PH04) && string.IsNullOrEmpty(this.PH05) && string.IsNullOrEmpty(this.PH06) && string.IsNullOrEmpty(this.PH07) && string.IsNullOrEmpty(this.PH08) && string.IsNullOrEmpty(this.PH09) && string.IsNullOrEmpty(this.PH10))
                {
                    return "Invalid Numbers";
                }
                else
                {
                    return "Valid Numbers";
                }
            }
            catch (Exception ex)
            {
                var log = new LogConsoleError(InstanceID, "PhoneModel", "GetStatus()", ex.Message);
                await log.SaveSync();

                return "Could not Parse Numbers";
            }
        }
    }
}
