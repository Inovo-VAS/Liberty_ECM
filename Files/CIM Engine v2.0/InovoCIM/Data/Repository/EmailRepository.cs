#region [ Using ]
using InovoCIM.Data.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace InovoCIM.Data.Repository
{
    public class EmailRepository
    {
        #region [Private] - [ Send Email Async ]
        private async Task<bool> SendEmailAsync(string InstanceID, string subject, string message)
        {
            try
            {
                ConfigurationCore ConfigObject = new ConfigurationCore();
                List<ConfigurationCore> ConfigList = new List<ConfigurationCore>();
                ConfigList = await ConfigObject.GetListAsync(InstanceID);

                List<ConfigurationCore> ConfigMail = new List<ConfigurationCore>();
                foreach (ConfigurationCore item in ConfigList.Where(x => x.Area == "Mail" && x.SubArea == "Internal")) { ConfigMail.Add(item); }

                string _MailFrom = ConfigMail.Where(x => x.Item == "From").Select(x => x.Value).FirstOrDefault();
                string _MailDisplay = ConfigMail.Where(x => x.Item == "Display").Select(x => x.Value).FirstOrDefault();
                string _MailServer = ConfigMail.Where(x => x.Item == "Server").Select(x => x.Value).FirstOrDefault();
                string _MailPort = ConfigMail.Where(x => x.Item == "Port").Select(x => x.Value).FirstOrDefault();
                string _MailSSL = ConfigMail.Where(x => x.Item == "SSL").Select(x => x.Value).FirstOrDefault();
                string _MailUsername = ConfigMail.Where(x => x.Item == "Username").Select(x => x.Value).FirstOrDefault();
                string _MailPassword = ConfigMail.Where(x => x.Item == "Password").Select(x => x.Value).FirstOrDefault();

                List<ConfigurationCore> ReportUsers = new List<ConfigurationCore>();
                foreach (ConfigurationCore item in ConfigList.Where(x => x.Area == "Report" && x.SubArea == "Users")) { ReportUsers.Add(item); }

                var emailMsg = new MailMessage { From = new MailAddress(_MailFrom, _MailDisplay) };
                foreach (var account in ReportUsers) { emailMsg.To.Add(account.Value); }

                emailMsg.Subject = subject;
                emailMsg.Body = message;
                emailMsg.IsBodyHtml = true;

                string path = Directory.GetCurrentDirectory() + @"\Content\logo.png";
                LinkedResource img = new LinkedResource(path, MediaTypeNames.Image.Jpeg);
                img.ContentId = "CompanyLogo";

                AlternateView av = AlternateView.CreateAlternateViewFromString(message, null, MediaTypeNames.Text.Html);
                av.LinkedResources.Add(img);
                emailMsg.AlternateViews.Add(av);

                using (var smtpClient = new SmtpClient(_MailServer))
                {
                    smtpClient.Port = int.Parse(_MailPort);
                    smtpClient.EnableSsl = (_MailSSL == "Yes") ? true : false;
                    smtpClient.UseDefaultCredentials = true;
                    smtpClient.Credentials = new NetworkCredential(_MailUsername, _MailPassword);
                    await smtpClient.SendMailAsync(emailMsg);
                }

                return true;
            }
            catch (Exception ex)
            {
                var log = new LogConsoleError("MailRepo", "Email Repository", "SendEmailAsync()", ex.Message);
                await log.SaveSync();
                return false;
            }
        }
        #endregion

        //---------------------------------------------------------------------//

        #region [ Send Report File ]
        public async Task<bool> SendReportFile(string InstanceID, DateTime StartTime, string FileName, int StatsLineTotal, int StatsFileDuplicate, int StatsPresenceDuplicate, int StatsScriptDuplicate)
        {
            bool sendClient = false;
            try
            {
                DateTime EndTime = DateTime.Now;
                TimeSpan runtime = (EndTime - StartTime);

                double seconds = runtime.TotalSeconds;
                double recordsPerSeconds = Math.Round((StatsLineTotal / seconds), 0);

                string TempClient = File.ReadAllText(Directory.GetCurrentDirectory() + @"\EmailTemplates\ReportFile.html");

                TempClient = TempClient.Replace("#NOTIFICATION#", "<span style='color:blue;'>File Notification</span>");
                TempClient = TempClient.Replace("#FILENAME#", FileName);
                TempClient = TempClient.Replace("#RECEIVED#", StartTime.ToString("dd-MM-yyyy hh:mm:ss"));
                TempClient = TempClient.Replace("#TOTALLINES#", StatsLineTotal.ToString());
                TempClient = TempClient.Replace("#FILEDUPLICATES#", StatsFileDuplicate.ToString());
                TempClient = TempClient.Replace("#PRESENCEDUPLICATES#", StatsPresenceDuplicate.ToString());
                TempClient = TempClient.Replace("#SCRIPTDUPLICATES#", StatsScriptDuplicate.ToString());
                TempClient = TempClient.Replace("#LINESPERSECOND#", recordsPerSeconds.ToString());
                TempClient = TempClient.Replace("#TOTALRUNTIME#", runtime.ToString());
                TempClient = TempClient.Replace("#DATE#", DateTime.Now.ToString("yyyy-MM-dd hh:mm tt", CultureInfo.InvariantCulture));

                sendClient = await SendEmailAsync(InstanceID, "InovoCIM V4 - File Notification", TempClient.ToString());
                return sendClient;
            }
            catch (Exception ex)
            {

                return false;
            }
        }
        #endregion

        #region [ Send Error ]
        public async Task<bool> SendError(string InstanceID, string heading, string message)
        {
            bool sendClient = false;
            try
            {
                string TempClient = File.ReadAllText(Directory.GetCurrentDirectory() + @"\EmailTemplates\Error.html");

                TempClient = TempClient.Replace("#HEADING#", heading);
                TempClient = TempClient.Replace("#MESSAGE#", message);
                TempClient = TempClient.Replace("#DATE#", DateTime.Now.ToString("yyyy-MM-dd hh:mm tt", CultureInfo.InvariantCulture));

                sendClient = await SendEmailAsync(InstanceID, "InovoCIM V4 - Error Notification", TempClient.ToString());
                return sendClient;
            }
            catch (Exception ex)
            {

                return false;
            }
        }
        #endregion

        #region [ Send Header Failed ]
        public async Task<bool> SendHeaderFailed(string InstanceID, string FileName, string Received, string Required)
        {
            bool sendClient = false;
            try
            {
                DateTime StartTime = DateTime.Now;

                string TempClient = File.ReadAllText(Directory.GetCurrentDirectory() + @"\EmailTemplates\ReportHeaderFailed.html");

                TempClient = TempClient.Replace("#NOTIFICATION#", "<span style='color:blue;'>File Header Validation Failed</span>");
                TempClient = TempClient.Replace("#FILENAME#", FileName);
                TempClient = TempClient.Replace("#DATEINPUT#", StartTime.ToString("dd-MM-yyyy hh:mm:ss"));
                TempClient = TempClient.Replace("#RECEIVED#", Received);
                TempClient = TempClient.Replace("#REQUIRED#", Required);

                TempClient = TempClient.Replace("#DATE#", DateTime.Now.ToString("yyyy-MM-dd hh:mm tt", CultureInfo.InvariantCulture));

                sendClient = await SendEmailAsync(InstanceID, "InovoCIM V4 - Header Validation Failed", TempClient.ToString());
                return sendClient;
            }
            catch (Exception ex)
            {

                return false;
            }
        }
        #endregion
    }
}
