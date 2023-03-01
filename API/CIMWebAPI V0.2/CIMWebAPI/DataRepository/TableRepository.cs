using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using CIMWebAPI.Data.Entities;
using CIMWebAPI.DataRepository;
using ConsoleApp1;

namespace CIMWebAPI.DataRepository
{
    public class TableRepository
    {
        #region [ Column Table Load ]
        public DataTable ColumnTableLoad(DataTable table)
        {
            table.Columns.Add("SERVICEID", typeof(int));
            table.Columns.Add("LOADID", typeof(int));
            table.Columns.Add("STATUS", typeof(char));
            table.Columns.Add("DESCRIPTION", typeof(string));
            table.Columns.Add("RDATE", typeof(DateTime));
            table.Columns.Add("RECORDCOUNT", typeof(int));
            table.Columns.Add("PRIORITYTYPE", typeof(int));
            table.Columns.Add("PRIORITYVALUE", typeof(int));

            return table;
        }
        #endregion

        #region [ Column Table Presence ]
        public DataTable ColumnTablePresence(DataTable table)
        {
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("SERVICEID", typeof(int));
            table.Columns.Add("NAME", typeof(string));
            table.Columns.Add("PHONE", typeof(string));
            table.Columns.Add("CALLINGHOURS", typeof(string));
            table.Columns.Add("SOURCEID", typeof(long));
            table.Columns.Add("STATUS", typeof(int));
            table.Columns.Add("SCHEDULETYPE", typeof(char));
            table.Columns.Add("SCHEDULEDATE", typeof(DateTime));
            table.Columns.Add("LOADID", typeof(int));
            table.Columns.Add("LASTAGENT", typeof(int));
            table.Columns.Add("LASTQCODE", typeof(int));
            table.Columns.Add("FIRSTHANDLINGDATE", typeof(DateTime));
            table.Columns.Add("LASTHANDLINGDATE", typeof(DateTime));
            table.Columns.Add("DAILYCOUNTER", typeof(int));
            table.Columns.Add("TOTALCOUNTER", typeof(int));
            table.Columns.Add("BUSYSIGNALCOUNTER", typeof(int));
            table.Columns.Add("NOANSWERCOUNTER", typeof(int));
            table.Columns.Add("ANSWERMACHINECOUNTER", typeof(int));
            table.Columns.Add("FAXCOUNTER", typeof(int));
            table.Columns.Add("INVGENREASONCOUNTER", typeof(int));
            table.Columns.Add("PRIORITY", typeof(int));
            table.Columns.Add("CAPTURINGAGENT", typeof(int));
            table.Columns.Add("PHONE1", typeof(string));
            table.Columns.Add("PHONE2", typeof(string));
            table.Columns.Add("PHONE3", typeof(string));
            table.Columns.Add("PHONE4", typeof(string));
            table.Columns.Add("PHONE5", typeof(string));
            table.Columns.Add("PHONE6", typeof(string));
            table.Columns.Add("PHONE7", typeof(string));
            table.Columns.Add("PHONE8", typeof(string));
            table.Columns.Add("PHONE9", typeof(string));
            table.Columns.Add("PHONE10", typeof(string));
            table.Columns.Add("PHONEDESC1", typeof(int));
            table.Columns.Add("PHONEDESC2", typeof(int));
            table.Columns.Add("PHONEDESC3", typeof(int));
            table.Columns.Add("PHONEDESC4", typeof(int));
            table.Columns.Add("PHONEDESC5", typeof(int));
            table.Columns.Add("PHONEDESC6", typeof(int));
            table.Columns.Add("PHONEDESC7", typeof(int));
            table.Columns.Add("PHONEDESC8", typeof(int));
            table.Columns.Add("PHONEDESC9", typeof(int));
            table.Columns.Add("PHONEDESC10", typeof(int));
            table.Columns.Add("PHONESTATUS1", typeof(int));
            table.Columns.Add("PHONESTATUS2", typeof(int));
            table.Columns.Add("PHONESTATUS3", typeof(int));
            table.Columns.Add("PHONESTATUS4", typeof(int));
            table.Columns.Add("PHONESTATUS5", typeof(int));
            table.Columns.Add("PHONESTATUS6", typeof(int));
            table.Columns.Add("PHONESTATUS7", typeof(int));
            table.Columns.Add("PHONESTATUS8", typeof(int));
            table.Columns.Add("PHONESTATUS9", typeof(int));
            table.Columns.Add("PHONESTATUS10", typeof(int));
            table.Columns.Add("PHONETIMEZONEID", typeof(string));
            table.Columns.Add("PHONETIMEZONEID1", typeof(string));
            table.Columns.Add("PHONETIMEZONEID2", typeof(string));
            table.Columns.Add("PHONETIMEZONEID3", typeof(string));
            table.Columns.Add("PHONETIMEZONEID4", typeof(string));
            table.Columns.Add("PHONETIMEZONEID5", typeof(string));
            table.Columns.Add("PHONETIMEZONEID6", typeof(string));
            table.Columns.Add("PHONETIMEZONEID7", typeof(string));
            table.Columns.Add("PHONETIMEZONEID8", typeof(string));
            table.Columns.Add("PHONETIMEZONEID9", typeof(string));
            table.Columns.Add("PHONETIMEZONEID10", typeof(string));
            table.Columns.Add("CURRENTPHONE", typeof(int));
            table.Columns.Add("CURRENTPHONECOUNTER", typeof(int));
            table.Columns.Add("TIMEZONEID", typeof(string));
            table.Columns.Add("COMMENTS", typeof(string));
            table.Columns.Add("CUSTOMDATA1", typeof(string));
            table.Columns.Add("CUSTOMDATA2", typeof(string));
            table.Columns.Add("CUSTOMDATA3", typeof(string));
            table.Columns.Add("CALLERID", typeof(string));
            table.Columns.Add("CALLERNAME", typeof(string));
            table.Columns.Add("LASTDATASOURCEID", typeof(string));
            table.Columns.Add("LASTDATASOURCECUSTOMERID", typeof(string));
            table.Columns.Add("RDATE", typeof(DateTime));
            return table;
        }
        #endregion

        #region [ Column Table Queue Phone Complete ]
        public DataTable ColumnTableQueuePhoneComplete(DataTable table)
        {
            table.Columns.Add("QueuePhoneCompleteID", typeof(int));
            table.Columns.Add("Command", typeof(string));
            table.Columns.Add("Input", typeof(string));
            table.Columns.Add("InputName", typeof(string));
            table.Columns.Add("Status", typeof(string));
            table.Columns.Add("Received", typeof(DateTime));
            table.Columns.Add("NextExecute", typeof(DateTime));
            table.Columns.Add("Actioned", typeof(DateTime));
            table.Columns.Add("RetryCount", typeof(int));
            table.Columns.Add("RetryDate", typeof(DateTime));
            table.Columns.Add("PersonID", typeof(int));
            table.Columns.Add("SourceID", typeof(long));
            table.Columns.Add("ServiceID", typeof(int));
            table.Columns.Add("LoadID", typeof(int));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Phone", typeof(string));
            table.Columns.Add("ScheduleDate", typeof(DateTime));
            table.Columns.Add("Priority", typeof(int));
            table.Columns.Add("CapturingAgent", typeof(int));
            table.Columns.Add("Phone01", typeof(string));
            table.Columns.Add("Phone02", typeof(string));
            table.Columns.Add("Phone03", typeof(string));
            table.Columns.Add("Phone04", typeof(string));
            table.Columns.Add("Phone05", typeof(string));
            table.Columns.Add("Phone06", typeof(string));
            table.Columns.Add("Phone07", typeof(string));
            table.Columns.Add("Phone08", typeof(string));
            table.Columns.Add("Phone09", typeof(string));
            table.Columns.Add("Phone10", typeof(string));
            table.Columns.Add("Comments", typeof(string));
            table.Columns.Add("CustomData1", typeof(string));
            table.Columns.Add("CustomData2", typeof(string));
            table.Columns.Add("CustomData3", typeof(string));
            table.Columns.Add("CallerID", typeof(string));
            table.Columns.Add("CallerName", typeof(string));

            return table;
        }
        #endregion

        #region [ Column Table QueuePhone Controller Table ]
        public DataTable ColumnTableQueuePhoneController(DataTable table)
        {
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("Received", typeof(DateTime));
            table.Columns.Add("Input", typeof(string));
            table.Columns.Add("ReturnMessage", typeof(string));
            table.Columns.Add("ReturnTime", typeof(DateTime));
            table.Columns.Add("SourceID", typeof(long));
            return table;
        }
        #endregion

        #region [ Column Table API Table ]
        public DataTable ColumnTableAPITable(DataTable table)
        {
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("Command", typeof(string));
            table.Columns.Add("SourceID", typeof(long));
            table.Columns.Add("ServiceID", typeof(int));
            table.Columns.Add("LoadID", typeof(int));
            table.Columns.Add("NewLoadID", typeof(int));
            table.Columns.Add("NewServiceID", typeof(int));
            table.Columns.Add("Batch", typeof(int));
            table.Columns.Add("Received", typeof(DateTime));
            table.Columns.Add("LeadProvider", typeof(string));
            return table;
        }
        #endregion

        #region [ Map Table Presence ]
        public DataTable MapTablePresence(DataTable table, OutQueuePhone data)
        {
            try
            {
                NumberOps numberOps = new NumberOps();
                DataRow Row = table.NewRow();
                Row["ID"] = DBNull.Value;
                Row["SERVICEID"] = data.ServiceID;
                Row["NAME"] = (data.Name == "") ? "Unknown Name" : data.Name;
                if (data.CallingHours != String.Empty)
                {
                    Row["CALLINGHOURS"] = data.CallingHours;
                }
                Row["SOURCEID"] = data.SourceID;
                bool enabled;
                SQLRepository sql = new SQLRepository();
                enabled = sql.CheckLoad(data);
                if (!(sql.CheckPrimaryPhoneDNC(data).GetAwaiter().GetResult()))
                {
                    Row["PHONE"] = DBNull.Value;
                    data.PhoneStatus1 = 1;
                    if (data.Status == 0)
                    {
                        if (enabled)
                        {
                            Row["STATUS"] = 1;
                        }
                        else
                        {
                            Row["STATUS"] = 41;
                        }
                    }
                    else
                    {
                        if (data.Status < 10 && enabled)
                        {
                            Row["STATUS"] = data.Status;
                        }
                        else if (data.Status < 10 && !enabled)
                        {
                            Row["STATUS"] = data.Status + 40;
                        }
                        else if ((data.Status == 41 || data.Status == 42 || data.Status == 43) && enabled)
                        {
                            Row["STATUS"] = data.Status - 40;
                        }
                    }
                }
                else
                {
                    string number = (numberOps.FormatNumber(data.Phone) == String.Empty) ? null : numberOps.FormatNumber(data.Phone);
                    if (number == null)
                    {
                        Row["PHONE"] = DBNull.Value;
                    }
                    else
                    {
                        Row["PHONE"] = number;
                    }
                    data.PhoneStatus1 = 0;
                    if (data.Status == 0)
                    {
                        if (enabled)
                        {
                            Row["STATUS"] = 1;
                        }
                        else
                        {
                            Row["STATUS"] = 41;
                        }
                    }
                    else
                    {
                        if (data.Status < 10 && enabled)
                        {
                            Row["STATUS"] = data.Status;
                        }
                        else if (data.Status < 10 && !enabled)
                        {
                            Row["STATUS"] = data.Status + 40;
                        }
                        else if ((data.Status == 41 || data.Status == 42 || data.Status == 43) && enabled)
                        {
                            Row["STATUS"] = data.Status - 40;
                        }
                    }
                }

                //Row["STATUS"] = 41;                       //data.Status;    // INT nie String nie.. - hier is jou Error... Nie Dai Status nie..
                Row["SCHEDULETYPE"] = DBNull.Value;
                if (data.ScheduleDate != null)
                {
                    Row["SCHEDULEDATE"] = data.ScheduleDate;
                }
                Row["LOADID"] = data.LoadID;
                Row["LASTAGENT"] = DBNull.Value;
                if (data.LastQCode != 0 && data.LastQCode != null)
                {
                    Row["LASTQCODE"] = data.LastQCode;
                }
                if (data.FirstHandlingDate != null)
                {
                    Row["FIRSTHANDLINGDATE"] = data.FirstHandlingDate;
                }
                if (data.LastHandlingDate != null)
                {
                    Row["LASTHANDLINGDATE"] = data.LastHandlingDate;
                }
                Row["DAILYCOUNTER"] = DBNull.Value;
                Row["TOTALCOUNTER"] = DBNull.Value;
                Row["BUSYSIGNALCOUNTER"] = DBNull.Value;
                Row["NOANSWERCOUNTER"] = DBNull.Value;
                Row["ANSWERMACHINECOUNTER"] = DBNull.Value;
                Row["FAXCOUNTER"] = DBNull.Value;
                Row["INVGENREASONCOUNTER"] = DBNull.Value;
                Row["PRIORITY"] = (data.Priority == 0) ? 1 : data.Priority;
                //Applies LIFO rule
                if (data.CapturingAgent == 0)
                {
                    Row["CAPTURINGAGENT"] = DBNull.Value;
                }
                else
                {
                    Row["CAPTURINGAGENT"] = data.CapturingAgent;
                }
                if (data.Phone1 != null && data.Phone1 != "")
                {
                    string number = (numberOps.FormatNumber(data.Phone1) == String.Empty) ? null : numberOps.FormatNumber(data.Phone1);

                    if (number == null)
                    {
                        Row["PHONE1"] = DBNull.Value;
                    }
                    else
                    {
                        Row["PHONE1"] = number;
                    }
                    data.Phone1 = number;
                }
                else if (Row["PHONE"] != DBNull.Value)
                {
                    Row["PHONE1"] = Row["PHONE"];
                }
                else if (data.Phone != null && data.Phone != "")
                {
                    string number = (numberOps.FormatNumber(data.Phone) == String.Empty) ? null : numberOps.FormatNumber(data.Phone);
                    if (number == null)
                    {
                        Row["PHONE1"] = DBNull.Value;
                    }
                    else
                    {
                        Row["PHONE1"] = number;
                    }
                }
                if (data.Phone2 != null && data.Phone2 != "")
                {
                    string number = (numberOps.FormatNumber(data.Phone2) == String.Empty) ? null : numberOps.FormatNumber(data.Phone2);
                    if (number == null)
                    {
                        Row["PHONE2"] = DBNull.Value;
                    }
                    else
                    {
                        Row["PHONE2"] = number;
                        data.Phone2 = number;
                        if (!sql.CheckSinglePhoneDNC(data.Phone2, data.connStringPresence, data.ServiceID).GetAwaiter().GetResult())
                        {
                            data.PhoneStatus2 = 1;
                        }
                        else
                        {
                            data.PhoneStatus2 = 0;
                        }
                    }
                }
                else
                {
                    data.PhoneDesc2 = null;
                    data.PhoneStatus2 = null;
                }
                if (data.Phone3 != null && data.Phone3 != "")
                {
                    string number = (numberOps.FormatNumber(data.Phone3) == String.Empty) ? null : numberOps.FormatNumber(data.Phone3);
                    if (number == null)
                    {
                        Row["PHONE3"] = DBNull.Value;
                    }
                    else
                    {
                        Row["PHONE3"] = number;
                        data.Phone3 = number;
                        if (!sql.CheckSinglePhoneDNC(data.Phone3, data.connStringPresence, data.ServiceID).GetAwaiter().GetResult())
                        {
                            data.PhoneStatus3 = 1;
                        }
                        else
                        {
                            data.PhoneStatus3 = 0;
                        }
                    }
                    
                }
                else
                {
                    data.PhoneDesc3 = null;
                    data.PhoneStatus3 = null;
                }
                if (data.Phone4 != null && data.Phone4 != "")
                {
                    string number = (numberOps.FormatNumber(data.Phone4) == String.Empty) ? null : numberOps.FormatNumber(data.Phone4);
                    if (number == null)
                    {
                        Row["PHONE4"] = DBNull.Value;
                    }
                    else
                    {
                        Row["PHONE4"] = number;
                        data.Phone4 = number;
                        if (!sql.CheckSinglePhoneDNC(data.Phone4, data.connStringPresence, data.ServiceID).GetAwaiter().GetResult())
                        {
                            data.PhoneStatus4 = 1;
                        }
                        else
                        {
                            data.PhoneStatus4 = 0;
                        }
                    }
                    
                }
                else
                {
                    data.PhoneDesc4 = null;
                    data.PhoneStatus4 = null;
                }
                if (data.Phone5 != null && data.Phone5 != "")
                {
                    string number = (numberOps.FormatNumber(data.Phone5) == String.Empty) ? null : numberOps.FormatNumber(data.Phone5);
                    if (number == null)
                    {
                        Row["PHONE5"] = DBNull.Value;
                    }
                    else
                    {
                        Row["PHONE5"] = number;
                        data.Phone5 = number; if (!sql.CheckSinglePhoneDNC(data.Phone5, data.connStringPresence, data.ServiceID).GetAwaiter().GetResult())
                        {
                            data.PhoneStatus5 = 1;
                        }
                        else
                        {
                            data.PhoneStatus5 = 0;
                        }
                    }
                    
                }
                else
                {
                    data.PhoneDesc5 = null;
                    data.PhoneStatus5 = null;
                }
                if (data.Phone6 != null && data.Phone6 != "")
                {
                    string number = (numberOps.FormatNumber(data.Phone6) == String.Empty) ? null : numberOps.FormatNumber(data.Phone6);
                    if (number == null)
                    {
                        Row["PHONE6"] = DBNull.Value;
                    }
                    else
                    {
                        Row["PHONE6"] = number;
                        data.Phone6 = number;
                        if (!sql.CheckSinglePhoneDNC(data.Phone6, data.connStringPresence, data.ServiceID).GetAwaiter().GetResult())
                        {
                            data.PhoneStatus6 = 1;
                        }
                        else
                        {
                            data.PhoneStatus6 = 0;
                        }
                    }
                    
                }
                else
                {
                    data.PhoneDesc6 = null;
                    data.PhoneStatus6 = null;
                }
                if (data.Phone7 != null && data.Phone7 != "")
                {
                    string number = (numberOps.FormatNumber(data.Phone7) == String.Empty) ? null : numberOps.FormatNumber(data.Phone7);
                    if (number == null)
                    {
                        Row["PHONE7"] = DBNull.Value;
                    }
                    else
                    {
                        Row["PHONE7"] = number;
                        data.Phone7 = number;
                        if (!sql.CheckSinglePhoneDNC(data.Phone7, data.connStringPresence, data.ServiceID).GetAwaiter().GetResult())
                        {
                            data.PhoneStatus7 = 1;
                        }
                        else
                        {
                            data.PhoneStatus7 = 0;
                        }
                    }
                    
                }
                else
                {
                    data.PhoneDesc7 = null;
                    data.PhoneStatus7 = null;
                }
                if (data.Phone8 != null && data.Phone8 != "")
                {
                    string number = (numberOps.FormatNumber(data.Phone8) == String.Empty) ? null : numberOps.FormatNumber(data.Phone8);
                    if (number == null)
                    {
                        Row["PHONE8"] = DBNull.Value;
                    }
                    else
                    {
                        Row["PHONE8"] = number;
                        data.Phone8 = number;
                        if (!sql.CheckSinglePhoneDNC(data.Phone8, data.connStringPresence, data.ServiceID).GetAwaiter().GetResult())
                        {
                            data.PhoneStatus8 = 1;
                        }
                        else
                        {
                            data.PhoneStatus8 = 0;
                        }
                    }

                }
                else
                {
                    data.PhoneDesc8 = null;
                    data.PhoneStatus8 = null;
                }
                if (data.Phone9 != null && data.Phone9 != "")
                {
                    string number = (numberOps.FormatNumber(data.Phone9) == String.Empty) ? null : numberOps.FormatNumber(data.Phone9);
                    if (number == null)
                    {
                        Row["PHONE9"] = DBNull.Value;
                    }
                    else
                    {
                        Row["PHONE9"] = number;
                        data.Phone9 = number;
                        if (!sql.CheckSinglePhoneDNC(data.Phone9, data.connStringPresence, data.ServiceID).GetAwaiter().GetResult())
                        {
                            data.PhoneStatus9 = 1;
                        }
                        else
                        {
                            data.PhoneStatus9 = 0;
                        }
                    }

                }
                else
                {
                    data.PhoneDesc9 = null;
                    data.PhoneStatus9 = null;
                }
                if (data.Phone10 != null && data.Phone10 != "")
                {
                    string number = (numberOps.FormatNumber(data.Phone10) == String.Empty) ? null : numberOps.FormatNumber(data.Phone10);
                    if (number == null)
                    {
                        Row["PHONE10"] = DBNull.Value;
                    }
                    else
                    {
                        Row["PHONE10"] = number;
                        data.Phone10 = number;
                        if (!sql.CheckSinglePhoneDNC(data.Phone10, data.connStringPresence, data.ServiceID).GetAwaiter().GetResult())
                        {
                            data.PhoneStatus10 = 1;
                        }
                        else
                        {
                            data.PhoneStatus10 = 0;
                        }
                    }

                }
                else
                {
                    data.PhoneDesc10 = null;
                    data.PhoneStatus10 = null;
                }
                if (data.PhoneDesc1 != null)
                {
                    Row["PHONEDESC1"] = data.PhoneDesc1.ToString();
                }
                if (data.PhoneDesc2 != null)
                {
                    Row["PHONEDESC2"] = data.PhoneDesc2.ToString();
                }
                if (data.PhoneDesc3 != null)
                {
                    Row["PHONEDESC3"] = data.PhoneDesc3.ToString();
                }
                if (data.PhoneDesc4 != null)
                {
                    Row["PHONEDESC4"] = data.PhoneDesc4.ToString();
                }
                if (data.PhoneDesc5 != null)
                {
                    Row["PHONEDESC5"] = data.PhoneDesc5.ToString();
                }
                if (data.PhoneDesc6 != null)
                {
                    Row["PHONEDESC6"] = data.PhoneDesc6.ToString();
                }
                if (data.PhoneDesc7 != null)
                {
                    Row["PHONEDESC7"] = data.PhoneDesc7.ToString();
                }
                if (data.PhoneDesc8 != null)
                {
                    Row["PHONEDESC8"] = data.PhoneDesc8.ToString();
                }
                if (data.PhoneDesc9 != null)
                {
                    Row["PHONEDESC9"] = data.PhoneDesc9.ToString();
                }
                if (data.PhoneDesc10 != null)
                {
                    Row["PHONEDESC10"] = data.PhoneDesc10.ToString();
                }
                if (data.PhoneStatus1 != null)
                {
                    if (!(sql.CheckPrimaryPhoneDNC(data).GetAwaiter().GetResult()))
                    {
                        Row["PHONESTATUS1"] = 1;
                    }
                    else
                    {
                        Row["PHONESTATUS1"] = data.PhoneStatus1;
                    }
                }
                if (data.PhoneStatus2 != null)
                {
                    Row["PHONESTATUS2"] = data.PhoneStatus2;

                }
                else
                {
                    Row["PHONESTATUS2"] = DBNull.Value;
                }
                if (data.PhoneStatus3 != null)
                {
                    Row["PHONESTATUS3"] = data.PhoneStatus3;

                }
                else
                {
                    Row["PHONESTATUS3"] = DBNull.Value;
                }
                if (data.PhoneStatus4 != null)
                {
                    Row["PHONESTATUS4"] = data.PhoneStatus4;

                }
                else
                {
                    Row["PHONESTATUS4"] = DBNull.Value;
                }
                if (data.PhoneStatus5 != null)
                {
                    Row["PHONESTATUS5"] = data.PhoneStatus5;

                }
                else
                {
                    Row["PHONESTATUS5"] = DBNull.Value;
                }
                if (data.PhoneStatus6 != null)
                {
                    Row["PHONESTATUS6"] = data.PhoneStatus6;

                }
                else
                {
                    Row["PHONESTATUS6"] = DBNull.Value;
                }
                if (data.PhoneStatus7 != null)
                {
                    Row["PHONESTATUS7"] = data.PhoneStatus7;

                }
                else
                {
                    Row["PHONESTATUS7"] = DBNull.Value;
                }
                if (data.PhoneStatus8 != null)
                {
                    Row["PHONESTATUS8"] = data.PhoneStatus8;

                }
                else
                {
                    Row["PHONESTATUS8"] = DBNull.Value;
                }
                if (data.PhoneStatus9 != null)
                {
                    Row["PHONESTATUS9"] = data.PhoneStatus9;

                }
                else
                {
                    Row["PHONESTATUS9"] = DBNull.Value;
                }
                if (data.PhoneStatus10 != null)
                {
                    Row["PHONESTATUS10"] = data.PhoneStatus10;
                }
                else
                {
                    Row["PHONESTATUS10"] = DBNull.Value;
                }

                int PhoneS1 = data.PhoneStatus1 ?? 1;
                int PhoneS2 = data.PhoneStatus2 ?? 1;
                int PhoneS3 = data.PhoneStatus3 ?? 1;
                int PhoneS4 = data.PhoneStatus4 ?? 1;
                int PhoneS5 = data.PhoneStatus5 ?? 1;
                int PhoneS6 = data.PhoneStatus6 ?? 1;
                int PhoneS7 = data.PhoneStatus7 ?? 1;
                int PhoneS8 = data.PhoneStatus8 ?? 1;
                int PhoneS9 = data.PhoneStatus9 ?? 1;
                int PhoneS10 = data.PhoneStatus10 ?? 1;

                if (PhoneS1 == 1 &&
                    PhoneS2 == 1 &&
                    PhoneS3 == 1 &&
                    PhoneS4 == 1 &&
                    PhoneS5 == 1 &&
                    PhoneS6 == 1 &&
                    PhoneS7 == 1 &&
                    PhoneS8 == 1 &&
                    PhoneS9 == 1 &&
                    PhoneS10 == 1)
                {
                    Row["STATUS"] = 95;

                }
                Row["PHONETIMEZONEID"] = "Presence_Server";
                Row["PHONETIMEZONEID1"] = "Presence_Server";
                Row["PHONETIMEZONEID2"] = "Presence_Server";
                Row["PHONETIMEZONEID3"] = "Presence_Server";
                Row["PHONETIMEZONEID4"] = "Presence_Server";
                Row["PHONETIMEZONEID5"] = "Presence_Server";
                Row["PHONETIMEZONEID6"] = "Presence_Server";
                Row["PHONETIMEZONEID7"] = "Presence_Server";
                Row["PHONETIMEZONEID8"] = "Presence_Server";
                Row["PHONETIMEZONEID9"] = "Presence_Server";
                Row["PHONETIMEZONEID10"] = "Presence_Server";
                Row["CURRENTPHONE"] = "0";
                //if (Row["PHONE"] != DBNull.Value)
                //{
                //    Row["CURRENTPHONE"] = 0;
                //}
                //else if (Row["PHONE1"] != DBNull.Value)
                //{
                //    Row["CURRENTPHONE"] = (Convert.ToInt32(Row["PHONESTATUS1"]) == 0) ? 1 : 2;
                //}
                //else if (Row["PHONE2"] != DBNull.Value)
                //{
                //    Row["CURRENTPHONE"] = (Convert.ToInt32(Row["PHONESTATUS2"]) == 0) ? 2 : 3;
                //}
                //else if (Row["PHONE3"] != DBNull.Value)
                //{
                //    Row["CURRENTPHONE"] = (Convert.ToInt32(Row["PHONESTATUS3"]) == 0) ? 3 : 4;
                //}
                //else if (Row["PHONE4"] != DBNull.Value)
                //{
                //    Row["CURRENTPHONE"] = (Convert.ToInt32(Row["PHONESTATUS4"]) == 0) ? 4 : 5;
                //}
                //else if (Row["PHONE5"] != DBNull.Value)
                //{
                //    Row["CURRENTPHONE"] = (Convert.ToInt32(Row["PHONESTATUS5"]) == 0) ? 5 : 6;
                //}
                //else if (Row["PHONE6"] != DBNull.Value)
                //{
                //    Row["CURRENTPHONE"] = (Convert.ToInt32(Row["PHONESTATUS6"]) == 0) ? 6 : 7;
                //}
                //else if (Row["PHONE7"] != DBNull.Value)
                //{
                //    Row["CURRENTPHONE"] = (Convert.ToInt32(Row["PHONESTATUS7"]) == 0) ? 7 : 8;
                //}
                //else if (Row["PHONE8"] != DBNull.Value)
                //{
                //    Row["CURRENTPHONE"] = (Convert.ToInt32(Row["PHONESTATUS8"]) == 0) ? 8 : 9;
                //}
                //else if (Row["PHONE9"] != DBNull.Value)
                //{
                //    Row["CURRENTPHONE"] = (Convert.ToInt32(Row["PHONESTATUS9"]) == 0) ? 9 : 10;
                //}
                //else if (Row["PHONE10"] != DBNull.Value)
                //{
                //    Row["CURRENTPHONE"] = 10;
                //}
                //else
                //{
                //    Row["CURRENTPHONE"] = 0;
                //}
                Row["CURRENTPHONECOUNTER"] = "0";
                Row["TIMEZONEID"] = "Presence_Server";
                Row["COMMENTS"] = data.Comments;
                Row["RDATE"] = DateTime.Now;
                Row["CUSTOMDATA1"] = data.CustomData1;
                Row["CUSTOMDATA2"] = data.CustomData2;
                Row["CUSTOMDATA3"] = data.CustomData3;
                Row["CALLERID"] = data.CallerID;
                Row["CALLERNAME"] = data.CallerName;
                table.Rows.Add(Row);
                return table;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region [ Map Table Queue Phone Complete ]
        public DataTable MapQueuePhoneComplete(DataTable table, OutQueuePhone data)
        {
            try
            {
                SQLRepository sql = new SQLRepository();
                DataRow Row = table.NewRow();
                Row["QueuePhoneCompleteID"] = DBNull.Value;
                Row["Command"] = data.Command;
                Row["Input"] = "Web API";
                Row["InputName"] = DBNull.Value;
                bool enabled = sql.CheckLoad(data);
                if (data.Status == 0)
                {
                    if (enabled)
                    {
                        Row["Status"] = 1.ToString();
                    }
                    else
                    {
                        Row["Status"] = 41.ToString();
                    }
                }
                else
                {
                    if (data.Status < 10 && enabled)
                    {
                        Row["Status"] = data.Status.ToString();
                    }
                    else if (data.Status < 10 && !enabled)
                    {
                        Row["Status"] = (data.Status + 40).ToString();
                    }
                    else
                    {
                        Row["Status"] = data.Status.ToString();
                    }
                }
                Row["Received"] = DateTime.Now;
                Row["NextExecute"] = DateTime.Now.AddDays(-1);
                Row["Actioned"] = DateTime.Now;
                Row["RetryCount"] = 0;
                Row["RetryDate"] = DBNull.Value; // Fok dit, moer net iets in...
                Row["PersonID"] = 0;
                Row["SourceID"] = data.SourceID;
                Row["ServiceID"] = data.ServiceID;
                Row["LoadID"] = data.LoadID;
                Row["Name"] = data.Name;
                Row["Phone"] = data.Phone;
                Row["ScheduleDate"] = DBNull.Value;
                if (data.Priority == 0)
                {
                    Row["Priority"] = 1;
                }
                else
                {
                    Row["Priority"] = data.Priority;
                }
                Row["Priority"] = data.Priority;
                if (data.CapturingAgent == 0)
                {
                    Row["CapturingAgent"] = 0;
                }
                else
                {
                    Row["CapturingAgent"] = data.CapturingAgent;
                }
                Row["Phone01"] = data.Phone1;
                Row["Phone02"] = data.Phone2;
                Row["Phone03"] = data.Phone3;
                Row["Phone04"] = data.Phone4;
                Row["Phone05"] = data.Phone5;
                Row["Phone06"] = data.Phone6;
                Row["Phone07"] = data.Phone7;
                Row["Phone08"] = data.Phone8;
                Row["Phone09"] = data.Phone9;
                Row["Phone10"] = data.Phone10;
                Row["Comments"] = data.Comments;
                Row["CustomData1"] = data.CustomData1;
                Row["CustomData2"] = data.CustomData2;
                Row["CustomData3"] = data.CustomData3;
                Row["CallerID"] = data.CallerID;
                Row["CallerName"] = data.CallerName;
                table.Rows.Add(Row);
                return table;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region [ Map Table API Table ]
        public DataTable MapQueueAPI(DataTable table, OutQueuePhone data)
        {
            try
            {
                DataRow Row = table.NewRow();
                Row["ID"] = DBNull.Value;
                Row["Command"] = data.Command;
                Row["SourceID"] = data.SourceID;
                Row["ServiceID"] = data.ServiceID;
                Row["LoadID"] = data.LoadID;
                Row["NewLoadID"] = 0;
                Row["NewServiceID"] = 0;
                Row["Batch"] = data.BatchNumber;
                Row["Received"] = DBNull.Value;
                Row["LeadProvider"] = data.leadProvider;
                table.Rows.Add(Row);
                return table;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion

        public DataTable MapLoadTable(DataTable table, OutQueuePhone data)
        {
            try
            {
                DataRow Row = table.NewRow();
                Row["SERVICEID"] = data.ServiceID;
                Row["LOADID"] = data.LoadID;
                Row["STATUS"] = 'D';
                Row["DESCRIPTION"] = (data.LoadDescription == null || data.LoadDescription == "") ? "APILoad" + Convert.ToString(DateTime.Now) + " " + data.leadProvider : data.LoadDescription;
                Row["RDATE"] = DateTime.Now;
                Row["RECORDCOUNT"] = DBNull.Value;
                Row["PRIORITYTYPE"] = DBNull.Value;
                Row["PRIORITYVALUE"] = DBNull.Value;
                table.Rows.Add(Row);
                return table;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
