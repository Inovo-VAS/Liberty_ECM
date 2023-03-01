#region [ Using ]
using InovoCIM.Business;
using InovoCIM.Data.Entities;
using InovoCIM.Data.Repository;
using System;
using System.Threading.Tasks;
#endregion

namespace InovoCIM
{
    public class Program
    {
        static void Main(string[] args)
        {
            DateTime StartTime = DateTime.Now;

            Guid Inst = Guid.NewGuid();
            string InstanceID = Inst.ToString();

            ApplicationStart Start = new ApplicationStart(InstanceID);
            bool IsActive = true;
            Task.Run(async () => IsActive = await Start.Master()).GetAwaiter().GetResult();
            if (IsActive)
            {
                /*DataFile File = new DataFile(InstanceID);
                Task.Run(async () => IsActive = await File.Master()).GetAwaiter().GetResult();

                if (IsActive)
                {
                    DataFilePriority Priority = new DataFilePriority(InstanceID);
                    Task.Run(async () => IsActive = await Priority.Master()).GetAwaiter().GetResult();
                }*/

                DataPhone Phone = new DataPhone(InstanceID);
                Task.Run(async () => IsActive = await Phone.Master()).GetAwaiter().GetResult();

                /*MediaEmail Email = new MediaEmail(InstanceID);
                Task.Run(async () => IsActive = await Email.Master()).GetAwaiter().GetResult();

                MediaSMS SMS = new MediaSMS(InstanceID);
                Task.Run(async () => IsActive = await SMS.Master()).GetAwaiter().GetResult();

                MediaSMSReply SMSReply = new MediaSMSReply(InstanceID);
                Task.Run(async () => IsActive = await SMSReply.Master()).GetAwaiter().GetResult();

                Reporting Report = new Reporting(InstanceID);
                Task.Run(async () => IsActive = await Report.Master()).GetAwaiter().GetResult();*/

                var Runtime = new LogConsoleRuntime(InstanceID, "Shutdown Application", "-----", StartTime);
                Task.Run(async () => await Runtime.SaveSync()).GetAwaiter().GetResult();
            }
            else
            {
                EmailRepository Email = new EmailRepository();

                Console.WriteLine("Application Is Not Active");

            }
        }
    }
}