using System;
using System.Collections.Generic;
using System.Text;
using MimeKit.Encodings;
using MimeKit.Text;
using HtmlAgilityPack;
using Email_Parsing.DataRepository;
using System.IO;
using MimeKit;
using System.Data;
using System.Linq;

namespace Email_Parsing
{
    internal class Program
    {
        static void Main(string[] args)
        {
            SQLRepository sql = new SQLRepository();
            Console.WriteLine("Getting Processed Mails");
            List<int> alreadyProcessed = sql.GetProcessedMails();
            //List<int> alreadyProcessed = new List<int>();
            Console.WriteLine(alreadyProcessed.Count.ToString() + " Processed Mails Found");
            Dictionary<int, string> mailIDFrom = new();
            Console.WriteLine("Getting Mail Data");
            var mails = sql.GetMailData(alreadyProcessed);
            Console.WriteLine(mails.Count.ToString() + " New Mails Found");
            var leads = new List<Lead>();

            foreach (var str in mails)
            {
                Console.WriteLine("Starting Mail ID " + str.Key.ToString());
                string code = String.Empty;
                var lead = new Lead();
                lead.MailID = str.Key;
                var html = new HtmlUtilities();
                string tempstring = String.Empty;
                string outString = String.Empty;

                byte[] byteArray = Encoding.ASCII.GetBytes(str.Value);
                MemoryStream stream = new MemoryStream(byteArray);
                var parser = new MimeParser(stream, MimeFormat.Entity);
                var message = parser.ParseMessage();
                var tempFrom = message.From.Mailboxes.First().Address;
                mailIDFrom.Add(str.Key, tempFrom);
                DataTable temp = new DataTable();
                try
                {
                    string tablestring = message.HtmlBody;
                    if(message.HtmlBody.Contains("<table"))
                    {
                        var tablestart = message.HtmlBody.IndexOf("<table");
                        var lengthEnd = 8; // Length of </table>
                        var totallength = message.HtmlBody.Length;
                        var tablend = message.HtmlBody.IndexOf("</table>");
                        tablestring = message.HtmlBody.Substring(tablestart, tablend - tablestart + lengthEnd);
                    }
                    temp = HtmlUtilities.ConvertHTMLTablesToDataTable(tablestring);
                }
                catch (Exception)
                {
                    //
                }
                temp = (temp == null) ? new System.Data.DataTable() : temp;
                Lead outLead = new();

                outLead.MailID = str.Key;

                for (int i = 0; i < temp.Rows.Count; i++)
                {
                    if (temp.Columns.Count > 2 && temp.Rows.Count == 6)
                    {
                        switch(i)
                        {
                            case 0:
                                outLead.Purpose = HtmlUtilities.ConvertToPlainText((string)temp.Rows[i][0]).Replace("\r", "").Replace("\n", "");
                                break;
                            case 1:
                                outLead.FirstName = HtmlUtilities.ConvertToPlainText((string)temp.Rows[i][0]).Replace("\r", "").Replace("\n", "");
                                break;
                            case 2:
                                outLead.LastName = HtmlUtilities.ConvertToPlainText((string)temp.Rows[i][0]).Replace("\r", "").Replace("\n", "");
                                break;
                            case 3:
                                outLead.Phone = HtmlUtilities.ConvertToPlainText((string)temp.Rows[i][0]).Replace("\r", "").Replace("\n", "");
                                break;
                            case 4:
                                outLead.Email = HtmlUtilities.ConvertToPlainText((string)temp.Rows[i][0]).Replace("\r", "").Replace("\n", "");
                                break;
                            case 5:
                                outLead.Source = HtmlUtilities.ConvertToPlainText((string)temp.Rows[i][0]).Replace("\r", "").Replace("\n", "");
                                break;
                        }
                    }
                    if (temp.Columns.Count > 2 && temp.Rows.Count == 5)
                    {
                        switch (i)
                        {
                            case 0:
                                outLead.FirstName = HtmlUtilities.ConvertToPlainText((string)temp.Rows[i][0]).Replace("\r", "").Replace("\n", "").Split(' ').FirstOrDefault();
                                outLead.LastName = HtmlUtilities.ConvertToPlainText((string)temp.Rows[i][0]).Replace("\r", "").Replace("\n", "").Split(' ').LastOrDefault();
                                break;
                            case 1:
                                outLead.Phone = HtmlUtilities.ConvertToPlainText((string)temp.Rows[i][0]).Replace("\r", "").Replace("\n", "");
                                break;
                            case 2:
                                outLead.Email = HtmlUtilities.ConvertToPlainText((string)temp.Rows[i][0]).Replace("\r", "").Replace("\n", "");
                                break;
                            case 4:
                                outLead.Source = HtmlUtilities.ConvertToPlainText((string)temp.Rows[i][0]).Replace("\r", "").Replace("\n", "");
                                break;
                        }
                    }
                    else
                    {
                        switch (HtmlUtilities.ConvertToPlainText((string)temp.Rows[i][0]).Replace("\r", "").Replace("\n", ""))
                        {
                            case "What would you like to speak to us about?":
                                outLead.Purpose = HtmlUtilities.ConvertToPlainText((string)temp.Rows[i][1]).Replace("\r", "").Replace("\n", "");
                                break;
                            case "Your name":
                                outLead.FirstName = HtmlUtilities.ConvertToPlainText((string)temp.Rows[i][1]).Replace("\r", "").Replace("\n", "");
                                break;
                            case "Surname":
                                outLead.LastName = HtmlUtilities.ConvertToPlainText((string)temp.Rows[i][1]).Replace("\r", "").Replace("\n", "");
                                break;
                            case "Contact number":
                                outLead.Phone = HtmlUtilities.ConvertToPlainText((string)temp.Rows[i][1]).Replace("\r", "").Replace("\n", "");
                                break;
                            case "Email address":
                                outLead.Email = HtmlUtilities.ConvertToPlainText((string)temp.Rows[i][1]).Replace("\r", "").Replace("\n", "");
                                break;
                            case "Source":
                                outLead.Source = HtmlUtilities.ConvertToPlainText((string)temp.Rows[i][1]).Replace("\r", "").Replace("\n", "");
                                break;
                        }
                    }
                }

                if (!String.IsNullOrEmpty(outLead.FirstName) && outLead.IsNaturalNumber())
                {
                    Console.WriteLine("Mail ID " + str.Key.ToString() + " was successfully parsed");
                    leads.Add(outLead);
                }
                else
                {

                }
            }
            List<int> toInsert = mails.Keys.ToList();
            Console.WriteLine("Inserting Processed Mails");
            var success = sql.InsertProcessedIDs(toInsert, alreadyProcessed);
            foreach (var item in leads)
            {
                sql.InsertQueuePhone(item, mailIDFrom[item.MailID]);
                Console.WriteLine("Mail ID " + item.MailID.ToString() + " Queued Successfully");
            }
        }
    }
}
