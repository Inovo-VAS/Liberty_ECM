using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Dapper;

namespace Email_Parsing
{
    public class SQLRepository
    {
        private string _sqlsource = "Data Source=PSQLPresnce1\\INST01;Initial Catalog=SQLPR1;User Id=cim_user;Password=Gr!@#$tj!@#$2019;MultipleActiveResultSets=true;Connection Timeout=0";
        private string _sqltarget = "Data Source=PSQLPresnce1\\INST01;Initial Catalog=SQLPR1;User Id=cim_user;Password=Gr!@#$tj!@#$2019;MultipleActiveResultSets=true;Connection Timeout=0";
        //private string _sqltarget = "Data Source=10.122.156.194\\PRESENCE;Initial Catalog=SQLPR1;User Id=PTOOLS;Password=PTOOLS;MultipleActiveResultSets=true;";
        private string _sqlcim = "Data Source=PSQLPresnce1a\\INST02;Initial Catalog=CIM;User Id=cim_user;Password=Gr!@#$tj!@#$2019;MultipleActiveResultSets=true;";

        /*
                     keyValuePairs.Add("what would you like to speak to us about?", "Purpose");
            keyValuePairs.Add("your name", "FirstName");
            keyValuePairs.Add("surname", "LastName");
            keyValuePairs.Add("email address", "Email");
            keyValuePairs.Add("contact number", "Phone");
            //"Source", "Policy Number", "Description of customer request"
            keyValuePairs.Add("source", "Source");
         */

        public List<int> GetProcessedMails()
        {
            try
            {
                List<int> outIDs = new();
                string query = @"SELECT [MAILID]
                                      ,[Created]
                                  FROM[CIM].[ECM].[ProcessedMails]
                                  WHERE CONVERT(DATE, Created) = CONVERT(DATE, GETDATE())";
                var conn = new SqlConnection(_sqlcim);
                conn.Open();
                var outRes = conn.Query(query, null, null, true, 0);
                conn.Close();
                foreach(var item in outRes)
                {
                    outIDs.Add((int)item.MAILID);
                }
                return outIDs;
            }
            catch (Exception)
            {
                return new List<int>();
            }
        }

        public bool MoveToCompletedMailbox(int inMailID)
        {
            try
            {
                string queryInsert = @"INSERT INTO [PREP].[PMSG_INBOUNDMAILCOMPLETED]
                                               ([INBOUNDMAILID]
                                               ,[MAILBOXID]
                                               ,[SENDERNAME]
                                               ,[SUBJECT]
                                               ,[RECEIVEDDATE]
                                               ,[INBOUNDCONTACTID]
                                               ,[SENDERADDRESS]
                                               ,[SENDDATE]
                                               ,[COMPLETEDDATE]
                                               ,[LOGIN]
                                               ,[INBOUNDSERVICEID]
                                               ,[QCODE]
                                               ,[USERID]
                                               ,[PRIORITY]
                                               ,[COMPLETEDMODE])
                                         SELECT
                                               [INBOUNDMAILID]
			                                      ,[MAILBOXID]
			                                      ,[SENDERNAME]
			                                      ,[SUBJECT]
			                                      ,[RECEIVEDDATE]
			                                      ,9999
			                                      ,[SENDERADDRESS]
			                                      ,[SENDDATE]
			                                      ,GETDATE()
			                                      ,99999
			                                      ,9999
			                                      ,100
			                                      ,9999
			                                      ,[PRIORITY]
			                                      ,0
	                                     FROM [PREP].[PMSG_INBOUNDMAILUNFINISHED] WHERE INBOUNDMAILID = @MailID";
                string queryDelete = "DELETE FROM [PREP].[PMSG_INBOUNDMAILUNFINISHED] WHERE INBOUNDMAILID = @MailID";
                queryDelete = queryDelete.Replace("@MailID", inMailID.ToString());
                queryInsert = queryInsert.Replace("@MailID", inMailID.ToString());
                using var conn = new SqlConnection(_sqltarget);
                conn.Query(queryInsert,null,null,true,0);
                conn.Query(queryDelete,null,null,true,0);
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public int PutLoad(Lead inLead, string inEmailFrom)
        {
            try
            {
                inLead.Email = inEmailFrom;
                var substringemail = (inLead.Email.Contains('@')) ? inLead.Email.Substring(inLead.Email.IndexOf('@'), inLead.Email.Length - inLead.Email.IndexOf('@')) : "unknown.co.za";
                string tempFrom = substringemail.Split('.').First().Trim('@');
                string tempDescription = "Mailbox Leads - " + tempFrom + " " + DateTime.Now.ToString("MMMM") + " " + DateTime.Now.Year.ToString() ;
                string putMaxLoad = @"IF EXISTS (SELECT * FROM [PREP].[PCO_LOAD] WHERE SERVICEID = @SERVICEIDIN AND DESCRIPTION = '@DESCRIPTIONIN')
                                        BEGIN
	                                        SELECT LOADID FROM [PREP].[PCO_LOAD] WHERE SERVICEID = @SERVICEIDIN AND DESCRIPTION = '@DESCRIPTIONIN'
                                        END
                                        ELSE
                                        BEGIN
	                                        INSERT INTO [PREP].[PCO_LOAD]
	                                        (
	                                        SERVICEID,
	                                        LOADID,
	                                        DESCRIPTION,
	                                        PRIORITYTYPE,
	                                        PRIORITYVALUE,
	                                        STATUS,
                                            RDATE
	                                        )
	                                        VALUES
	                                        (
		                                        @SERVICEIDIN,
		                                        (SELECT MAX(LOADID) + 1 FROM [PREP].[PCO_LOAD] WHERE SERVICEID = @SERVICEIDIN),
		                                        '@DESCRIPTIONIN',
		                                        0,
		                                        1,
		                                        'E',
                                                GETDATE()
	                                        )
                                            SELECT LOADID FROM [PREP].[PCO_LOAD] WHERE SERVICEID = @SERVICEIDIN AND DESCRIPTION = '@DESCRIPTIONIN'
                                        END";

                putMaxLoad = putMaxLoad.Replace("@SERVICEIDIN", 533.ToString());
                putMaxLoad = putMaxLoad.Replace("@DESCRIPTIONIN", tempDescription);

                using var connloadid = new SqlConnection(_sqltarget);
                var outRes = connloadid.Query(putMaxLoad);

                if(outRes.Count() > 0)
                {
                    return Convert.ToInt32(outRes.First().LOADID);
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public int PutPerson(Lead inLead)
        {
            try
            {
                string querypersonID = @"SELECT [PersonID]
                                          FROM [CIM].[ECM].[Person]
                                          WHERE SourceExtID = '@MailID'";

                querypersonID = querypersonID.Replace("@MailID", inLead.MailID.ToString());

                using var connpersonid = new SqlConnection(_sqlcim);
                var outRes = connpersonid.Query(querypersonID);
                if(outRes.Count() > 0)
                {
                    return (int)outRes.First().PersonID; 
                }
                else
                {
                    string queryperson = @"INSERT INTO [ECM].[Person]
                                           ([PersonType]
                                           ,[Status]
                                           ,[Title]
                                           ,[Name]
                                           ,[Surname]
                                           ,[IDNumber]
                                           ,[Passport]
                                           ,[Phone01]
                                           ,[Phone02]
                                           ,[Phone03]
                                           ,[Phone04]
                                           ,[Email01]
                                           ,[Email02]
                                           ,[SourceExtID]
                                           ,[Updated])
                                     VALUES
                                           (NULL
                                           ,NULL
                                           ,NULL
                                           ,'@Name'
                                           ,'@Surname'
                                           ,NULL
                                           ,NULL
                                           ,'@Phone'
                                           ,NULL
                                           ,NULL
                                           ,NULL
                                           ,'@Email'
                                           ,NULL
                                           ,'@MailID'
                                           ,GETDATE())";
                    queryperson = queryperson.Replace("@Name", inLead.FirstName);
                    queryperson = queryperson.Replace("@Surname", inLead.LastName);
                    queryperson = queryperson.Replace("@Phone", inLead.Phone);
                    queryperson = queryperson.Replace("@Email", inLead.Email);
                    queryperson = queryperson.Replace("@MailID", inLead.MailID.ToString());

                    using (SqlConnection connperson = new SqlConnection(_sqlcim))
                    {
                        connperson.Open();
                        SqlCommand cmd = connperson.CreateCommand();
                        cmd.CommandText = queryperson;
                        cmd.CommandTimeout = 0;
                        cmd.ExecuteNonQuery();
                        connperson.Close();
                    }

                    return PutPerson(inLead);
                }
            }
            catch(Exception ex)
            {
                return 0;
            }
        }

        public bool InsertQueuePhone(Lead inLead, string inEmailFrom)
        {
            try
            {
                int outPersonID = PutPerson(inLead);
                int outLoadID = PutLoad(inLead, inEmailFrom);
                string query = @"INSERT INTO [ECM].[QueuePhone] (
                Command,Input,InputName,Status,Received,NextExecute,Actioned,RetryCount,RetryDate,PersonID,SourceID,ServiceID,LoadID,Name,Phone,Phone01,ScheduleDate,Priority,CapturingAgent,Comments,CustomData1,CustomData2,CustomData3,CallerID,CallerName) 
                VALUES ('addcall','MailBox','Mail ID @MailID','Received',GETDATE(),GETDATE(),NULL,0,@PersonIN,@PersonIN,@MailID,533,@LoadID,'@Name',[ECM].[CIMNumber]('@Phone'),[ECM].[CIMNumber]('@Phone'),NULL,1,NULL,'@Source',NULL,NULL,'@Purpose',NULL,NULL)";
                query = query.Replace("@MailID", inLead.MailID.ToString());
                query = query.Replace("@Name", inLead.FirstName + " " + inLead.LastName);
                query = query.Replace("@Email", inLead.Email);
                query = query.Replace("@Phone", inLead.Phone);
                query = query.Replace("@Purpose", inLead.Purpose);
                query = query.Replace("@Source", inLead.Source);
                query = query.Replace("@PersonIN", outPersonID.ToString());
                query = query.Replace("@LoadID", outLoadID.ToString());
                using (SqlConnection conn = new SqlConnection(_sqlcim))
                {
                    conn.Open();
                    SqlCommand cmd = conn.CreateCommand();
                    cmd.CommandText = query;
                    cmd.CommandTimeout = 0;
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
                return MoveToCompletedMailbox(inLead.MailID);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool InsertProcessedIDs(List<int> inMails, List<int> alreadyProcessed)
        {
            try
            {
                foreach(var item in inMails)
                {
                    if(alreadyProcessed.Contains(item))
                    {
                        //
                    }
                    else
                    {
                        string query = "INSERT INTO [ECM].[ProcessedMails] (MAILID) VALUES (@InID)";
                        query = query.Replace("@InID", item.ToString());
                        using var conn = new SqlConnection(_sqlcim);
                        conn.Execute(query, null, null, 0);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public Dictionary<int,string> GetMailData(List<int> inProcessed)
        {
            try
            {
                Dictionary<int,string> mailData = new();
                string query = @"SELECT [ID],[MAILDATA]
                                FROM [SQLPR1].[PREP].[PMSG_INBOUNDMAIL] WHERE 
                (
                ID IN(SELECT[INBOUNDMAILID] FROM[SQLPR1].[PREP].[PMSG_INBOUNDMAILUNFINISHED] WHERE CONVERT(DATE, RECEIVEDDATE) = CONVERT(DATE, GETDATE()) AND MAILBOXID = 385)
                OR ID IN(SELECT[INBOUNDMAILID] FROM[SQLPR1].[PREP].[PMSG_INBOUNDMAILDELETED] WHERE CONVERT(DATE, RECEIVEDDATE) = CONVERT(DATE, GETDATE()) AND MAILBOXID = 385)
                OR ID IN(SELECT[INBOUNDMAILID] FROM[SQLPR1].[PREP].[PMSG_INBOUNDMAILCOMPLETED] WHERE CONVERT(DATE, RECEIVEDDATE) = CONVERT(DATE, GETDATE()) AND MAILBOXID = 385)
                )";
                for (int i = 0; i < inProcessed.Count; i++)
                {
                    if(i == 0)
                    {
                        query += " AND ID NOT IN (";
                    }
                    query += inProcessed[i].ToString();
                    if(i == inProcessed.Count - 1)
                    {
                        query += ")";
                    }
                    else
                    {
                        query += ",";
                    }
                }
                var conn = new SqlConnection(_sqlsource);
                //conn.Open();
                var outRes = conn.Query(query, null, null, true, 0);
                //conn.Close();
                foreach(var item in outRes)
                {
                    mailData.Add((int)item.ID, item.MAILDATA.ToString());
                }
                return mailData;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new Dictionary<int, string>();
            }
        }
    }
}
