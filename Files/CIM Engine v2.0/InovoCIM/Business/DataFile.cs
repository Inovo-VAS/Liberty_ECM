#region [ Using ]
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using InovoCIM.Data.Entities;
using InovoCIM.Data.Repository;
using InovoCIM.FileProcess;
#endregion

namespace InovoCIM.Business
{
    public class DataFile
    {
        public LogRepository LogRepo { get; set; }
        public string InstanceID { get; set; }
        public string Class { get; set; }
        private EmailRepository _email;

        #region [ Default Constructor ]
        public DataFile(string _InstanceID)
        {
            this.LogRepo = new LogRepository();
            this.InstanceID = _InstanceID;
            this.Class = "File";
            this._email = new EmailRepository();
        }
        #endregion

        //---------------------------------------------------------------------------//

        #region [ Master ]
        public async Task<bool> Master()
        {
            DateTime StartTime = DateTime.Now;
            var Event = new LogConsoleEvent(this.InstanceID);
            try
            {
                await Event.SaveAsync(this.Class, "Master()", "Start");
                ImportFile FileObject = new ImportFile();

                List<ImportFile> ImportFileList = new List<ImportFile>();
                ImportFileList = await FileObject.GetListAsync(this.InstanceID);                            // Gets the file configs in the database

                ConfigurationDirectory Directory = new ConfigurationDirectory();
                Directory = await Directory.GetSingleAsync(this.InstanceID);                                                                    // Get the shared location of where the files are being dropped, as per our config in database

                DirectoryInfo inputFTP = new DirectoryInfo(Directory.FTP);
                //DirectoryInfo inputFTP = new DirectoryInfo("C:\\0.InovoCIM\\Busy");

                List<string> unMapped = await CompareLists(ImportFileList, inputFTP);

                foreach(string name in unMapped)
                {
                    var log = new LogConsoleError(this.InstanceID, this.Class, "GetFileList()", "File Not Mapped: " + name);
                    await log.SaveSync();

                    await _email.SendError(this.InstanceID, "File Not Mapped", name);
                }

                foreach (ImportFile ImportFileModel in ImportFileList)                                      // Loops each file config
                {
                    List<FileInfo> files = await GetFileList(ImportFileModel);                              // This is Step 1 below.
                    foreach (FileInfo file in files.OrderBy(x => x.LastWriteTime))
                    {
                        if (file.Exists)
                        {
                            string result = await RouteToMethod(ImportFileModel, file);
                            await Event.SaveAsync(this.Class, "Master() - RouteToMethod()", result);
                        }
                    }
                }

                await Event.SaveAsync(this.Class, "Master()", "End");
                var Runtime = new LogConsoleRuntime(this.InstanceID, this.Class, "Master()", StartTime);
                await Runtime.SaveSync();

                return true;
            }
            catch (Exception ex)
            {
                var log = new LogConsoleError(this.InstanceID, this.Class, "Master()", ex.Message);
                await log.SaveSync();

                return false;
            }
        }
        #endregion

        //---------------------------------------------------------------------------//

        #region [ Step 0 ] - [ Compare Configuration ]
        private async Task<List<string>> CompareLists(List<ImportFile> inFiles, DirectoryInfo inputDir)
        {
            List<FileInfo> files = new List<FileInfo>();
            var Event = new LogConsoleEvent(this.InstanceID);
            try
            {
                FileInfo[] FilesInFTP = inputDir.GetFiles("*.*");
                List<FileInfo> AllFiles = FilesInFTP.ToList();
                List<FileInfo> NotMapped = new List<FileInfo>();
                List<FileInfo> Mapped = new List<FileInfo>();
                foreach(FileInfo inf in FilesInFTP)
                {
                    int cnt = inFiles.Count(o => inf.Name.ToUpper().Contains(o.Partial));
                    if(cnt > 0)
                    {
                        Mapped.Add(inf);
                    }
                }

                List<string> outNames = AllFiles.Except(Mapped).Select(o => o.FullName).ToList();

                return outNames;
            }
            catch(Exception ex)
            {
                var log = new LogConsoleError(this.InstanceID, this.Class, "GetFileList()", ex.Message);
                await log.SaveSync();

                return new List<string>();
            }
        }
        #endregion

        #region [ Step 1 ] - [ Get File List ]
        private async Task<List<FileInfo>> GetFileList(ImportFile FileObject)
        {
            List<FileInfo> files = new List<FileInfo>();
            var Event = new LogConsoleEvent(this.InstanceID);
            try
            {
                ConfigurationDirectory Directory = new ConfigurationDirectory();
                Directory = await Directory.GetSingleAsync(this.InstanceID);                                                                    // Get the shared location of where the files are being dropped, as per our config in database
                if (Directory != null)
                {
                    DirectoryInfo inputFTP = new DirectoryInfo(Directory.FTP);
                    //DirectoryInfo inputFTP = new DirectoryInfo("C:\\0.InovoCIM\\Busy");
                    FileInfo[] FilesInFTP = inputFTP.GetFiles("*.*");
                    foreach (FileInfo file in FilesInFTP.Where(x => x.Name.Contains(FileObject.Partial)))                                       // Loop 1 -> check each file in directory to see if the file name contains the Partial Name as per our config
                    {
                        if (!file.IsReadOnly)
                        {
                            string source = Path.Combine(Directory.FTP, file.Name);
                            //string source = Path.Combine("C:\\0.InovoCIM\\Busy", file.Name);
                            string destination = Path.Combine(Directory.Input, file.Name);
                            //string destination = Path.Combine("C:\\0.InovoCIM\\Busy", file.Name);
                            File.Move(source, destination);
                            await Event.SaveAsync(this.Class, "GetFileList()", string.Format("File moved from FTP to Input: {0}", file.Name));
                        }
                        else
                        {
                            await Event.SaveAsync(this.Class, "GetFileList()", string.Format("File in FTP Is Read Only: {0}", file.Name));
                        }
                    }

                    DirectoryInfo input = new DirectoryInfo(Directory.Input);
                    //DirectoryInfo input = new DirectoryInfo("C:\\0.InovoCIM\\Busy");
                    FileInfo[] FilesInInput = input.GetFiles("*.*");


                    foreach (FileInfo file in FilesInInput.Where(x => x.Name.Contains(FileObject.Partial)))                                     // Loop 2 -> check each file in directory to see if the file name contains the Partial Name as per our config
                    {
                        if (!file.IsReadOnly)
                        {
                            files.Add(file);
                            await Event.SaveAsync(this.Class, "GetFileList()", string.Format("File in Queue to be worked: {0}", file.Name));
                        }
                        else
                        {
                            await Event.SaveAsync(this.Class, "GetFileList()", string.Format("File in Input Is Read Only: {0}", file.Name));
                        }
                    }
                }
                return files;
            }
            catch (Exception ex)
            {
                var log = new LogConsoleError(this.InstanceID, this.Class, "GetFileList()", ex.Message);
                await log.SaveSync();

                return files;
            }
        }
        #endregion

        #region [ Step 2 ] - [ Route To Method ]
        private async Task<string> RouteToMethod(ImportFile ImportFileModel, FileInfo file)
        {
            var Event = new LogConsoleEvent(this.InstanceID);
            bool isMapped = false;
            try
            {
                string Partial = ImportFileModel.Partial;

                if (Partial.ToUpper() == "ECMDS_ACQ_BMI_SMS")
                {
                    FileBMISMS DataModel = new FileBMISMS(this.InstanceID, ImportFileModel, file);
                    await Event.SaveAsync(this.Class, "RouteToMethod()", string.Format("Routed to File ECMDS_ACQ_BMI_SMS: {0}", file.Name));
                    await DataModel.Master();
                    isMapped = true;
                }
                if (Partial.ToUpper() == "ECMDS_ACQ_BMI_WEB")
                {
                    FileBMIWeb DataModel = new FileBMIWeb(this.InstanceID, ImportFileModel, file);
                    await Event.SaveAsync(this.Class, "RouteToMethod()", string.Format("Routed to File ECMDS_ACQ_BMI_WEB: {0}", file.Name));
                    await DataModel.Master();
                    isMapped = true;
                }
                if (Partial.ToUpper() == "ECMDS_ACQ_MONEY_SHOP")
                {
                    FileBMIOlicoMoneyshop DataModel = new FileBMIOlicoMoneyshop(this.InstanceID, ImportFileModel, file);
                    await Event.SaveAsync(this.Class, "RouteToMethod()", string.Format("Routed to File ECMDS_ACQ_MONEY_SHOP: {0}", file.Name));
                    await DataModel.Master();
                    isMapped = true;
                }
                if (Partial.ToUpper() == "ECMDS_ACQ_OLICO_SMS")
                {
                    FileBMIOlicoSMS DataModel = new FileBMIOlicoSMS(this.InstanceID, ImportFileModel, file);
                    await Event.SaveAsync(this.Class, "RouteToMethod()", string.Format("Routed to File ECMDS_ACQ_OLICO_SMS: {0}", file.Name));
                    await DataModel.Master();
                    isMapped = true;
                }
                if (Partial.ToUpper() == "ECMDS_AFF_RCVRY_SRC_BATCH")
                {
                    FileRecoveries DataModel = new FileRecoveries(this.InstanceID, ImportFileModel, file);
                    await Event.SaveAsync(this.Class, "RouteToMethod()", string.Format("Routed to File ECMDS_AFF_RCVRY_SRC_BATCH: {0}", file.Name));
                    await DataModel.Master();
                    isMapped = true;
                }
                if (Partial.ToUpper() == "ECMDS_ACQ_PCM")
                {
                    FilePleaseCallMe DataModel = new FilePleaseCallMe(this.InstanceID, ImportFileModel, file);
                    await Event.SaveAsync(this.Class, "RouteToMethod()", string.Format("Routed to File ECMDS_ACQ_PCM: {0}", file.Name));
                    await DataModel.Master();
                    isMapped = true;
                }
                if (Partial.ToUpper() == "ECMDR_RETENTION_QLINK_NTU")
                {
                    FileQLINKNTU DataModel = new FileQLINKNTU(this.InstanceID, ImportFileModel, file);
                    await Event.SaveAsync(this.Class, "RouteToMethod()", string.Format("Routed to File ECMDR_RETENTION_QLINK_NTU: {0}", file.Name));
                    await DataModel.Master();
                    isMapped = true;
                }
                if (Partial.ToUpper() == "ECMDR_RETENTION_QLINK_LAPSE")
                {
                    FileQLINKLapse DataModel = new FileQLINKLapse(this.InstanceID, ImportFileModel, file);
                    await Event.SaveAsync(this.Class, "RouteToMethod()", string.Format("Routed to File ECMDR_RETENTION_QLINK_LAPSE: {0}", file.Name));
                    await DataModel.Master();
                    isMapped = true;
                }
                if (Partial.ToUpper() == "ECMDR_RETENTION_COMPASS_LAPSE")
                {
                    FileCompassLapse DataModel = new FileCompassLapse(this.InstanceID, ImportFileModel, file);
                    await Event.SaveAsync(this.Class, "RouteToMethod()", string.Format("Routed to File ECMDR_RETENTION_COMPASS_LAPSE: {0}", file.Name));
                    await DataModel.Master();
                    isMapped = true;
                }
                if (Partial.ToUpper() == "ECMDR_RETENTION_COMPASS_NTU")
                {
                    FileCompassNTU DataModel = new FileCompassNTU(this.InstanceID, ImportFileModel, file);
                    await Event.SaveAsync(this.Class, "RouteToMethod()", string.Format("Routed to File ECMDR_RETENTION_COMPASS_NTU: {0}", file.Name));
                    await DataModel.Master();
                    isMapped = true;
                }
                if (Partial.ToUpper() == "ECMDR_RETENTION_ECM_NTU")
                {
                    FileECMNTU DataModel = new FileECMNTU(this.InstanceID, ImportFileModel, file);
                    await Event.SaveAsync(this.Class, "RouteToMethod()", string.Format("Routed to File ECMDR_RETENTION_ECM_NTU: {0}", file.Name));
                    await DataModel.Master();
                    isMapped = true;
                }
                if (Partial.ToUpper() == "ECMDR_RETENTION_ECM_LAPSE")
                {
                    FileECMLapse DataModel = new FileECMLapse(this.InstanceID, ImportFileModel, file);
                    await Event.SaveAsync(this.Class, "RouteToMethod()", string.Format("Routed to File ECMDR_RETENTION_ECM_LAPSE: {0}", file.Name));
                    await DataModel.Master();
                    isMapped = true;
                }
                if (Partial.ToUpper() == "ECMDR_RETENTION_BROKER_LAPSE")
                {
                    FileBrokerLapse DataModel = new FileBrokerLapse(this.InstanceID, ImportFileModel, file);
                    await Event.SaveAsync(this.Class, "RouteToMethod()", string.Format("Routed to File ECMDR_RETENTION_BROKER_LAPSE: {0}", file.Name));
                    await DataModel.Master();
                    isMapped = true;
                }
                if (Partial.ToUpper() == "ECMDR_WC_COMPASS")
                {
                    FileWelcomeCallCompass DataModel = new FileWelcomeCallCompass(this.InstanceID, ImportFileModel, file);
                    await Event.SaveAsync(this.Class, "RouteToMethod()", string.Format("Routed to File ECMDR_WC_COMPASS: {0}", file.Name));
                    await DataModel.Master();
                    isMapped = true;
                }
                if (Partial.ToUpper() == "ECMDS_YES_SOURCE")
                {
                    FileQuotedHotLeads DataModel = new FileQuotedHotLeads(this.InstanceID, ImportFileModel, file);
                    await Event.SaveAsync(this.Class, "RouteToMethod()", string.Format("Routed to File ECMDS_YES_SOURCE: {0}", file.Name));
                    await DataModel.Master();
                    isMapped = true;
                }
                if (Partial.ToUpper() == "ECMDS_ACQ_3WAY_NO_NAME")
                {
                    File3WayNoName DataModel = new File3WayNoName(this.InstanceID, ImportFileModel, file);
                    await Event.SaveAsync(this.Class, "RouteToMethod()", string.Format("Routed to File ECMDS_ACQ_3WAY_NO_NAME: {0}", file.Name));
                    await DataModel.Master();
                    isMapped = true;
                }
                if (Partial.ToUpper() == "ECMDS_ACQ_3WAY_SMS")
                {
                    File3WaySMS DataModel = new File3WaySMS(this.InstanceID, ImportFileModel, file);
                    await Event.SaveAsync(this.Class, "RouteToMethod()", string.Format("Routed to File ECMDS_ACQ_3WAY_SMS: {0}", file.Name));
                    await DataModel.Master();
                    isMapped = true;
                }
                if (Partial.ToUpper() == "ECMDS_ACQ_UNUSED_YES_BATCH")
                {
                    FileUnusedDel DataModel = new FileUnusedDel(this.InstanceID, ImportFileModel, file);
                    await Event.SaveAsync(this.Class, "RouteToMethod()", string.Format("Routed to File ECMDS_ACQ_UNUSED_YES_BATCH: {0}", file.Name));
                    await DataModel.Master();
                    isMapped = true;
                }
                if (Partial.ToUpper() == "ECMDS_ACQ_UNUSED_DEL_BATCH")
                {
                    FileUnusedYes DataModel = new FileUnusedYes(this.InstanceID, ImportFileModel, file);
                    await Event.SaveAsync(this.Class, "RouteToMethod()", string.Format("Routed to File ECMDS_ACQ_UNUSED_DEL_BATCH: {0}", file.Name));
                    await DataModel.Master();
                    isMapped = true;
                }
                if (Partial.ToUpper() == "ECMDR_RETENTION_L@W_LAPSE")
                {
                    FileLatWLapse DataModel = new FileLatWLapse(this.InstanceID, ImportFileModel, file);
                    await Event.SaveAsync(this.Class, "RouteToMethod()", string.Format("Routed to File ECMDR_RETENTION_L@W_LAPSE: {0}", file.Name));
                    await DataModel.Master();
                    isMapped = true;
                }
                if (Partial.ToUpper() == "ECMDR_RETENTION_L@W_NTU")
                {
                    FileLatWNTU DataModel = new FileLatWNTU(this.InstanceID, ImportFileModel, file);
                    await Event.SaveAsync(this.Class, "RouteToMethod()", string.Format("Routed to File ECMDR_RETENTION_L@W_NTU: {0}", file.Name));
                    await DataModel.Master();
                    isMapped = true;
                }
                if (Partial.ToUpper() == "ECMDS_DFS_REFERRAL")
                {
                    FileDFSReferrals DataModel = new FileDFSReferrals(this.InstanceID, ImportFileModel, file);
                    await Event.SaveAsync(this.Class, "RouteToMethod()", string.Format("Routed to File ECMDS_REF_DFS_REFERRAL: {0}", file.Name));
                    await DataModel.Master();
                    isMapped = true;
                }
                if (Partial.ToUpper() == "ECMDS_REF_PRS_TSF_REFERRAL")
                {
                    FileTSFReferrals DataModel = new FileTSFReferrals(this.InstanceID, ImportFileModel, file);
                    await Event.SaveAsync(this.Class, "RouteToMethod()", string.Format("Routed to File ECMDS_REF_PRS_TSF_REFERRAL: {0}", file.Name));
                    await DataModel.Master();
                    isMapped = true;
                }
                if (Partial.ToUpper() == "ECMDS_BLUE_LABEL_XDS_SMS")
                {
                    FileBlueLabel DataModel = new FileBlueLabel(this.InstanceID, ImportFileModel, file);
                    await Event.SaveAsync(this.Class, "RouteToMethod()", string.Format("Routed to File ECMDS_BLUE_LABEL_XDS_SMS: {0}", file.Name));
                    await DataModel.Master();
                    isMapped = true;
                }
                if (Partial.ToUpper() == "ECMDR_RETENTION_RE_INSTATEMENT")
                {
                    FileRetentionReinstatement DataModel = new FileRetentionReinstatement(this.InstanceID, ImportFileModel, file);
                    await Event.SaveAsync(this.Class, "RouteToMethod()", string.Format("Routed to File ECMDR_RETENTION_RE_INSTATEMENT: {0}", file.Name));
                    await DataModel.Master();
                    isMapped = true;
                }
                if (Partial.ToUpper() == "ECMDR_RETENTION_BROKER_NTU")
                {
                    FileRetentionPotentialNTU DataModel = new FileRetentionPotentialNTU(this.InstanceID, ImportFileModel, file);
                    await Event.SaveAsync(this.Class, "RouteToMethod()", string.Format("Routed to File ECMDR_RETENTION_BROKER_NTU: {0}", file.Name));
                    await DataModel.Master();
                    isMapped = true;
                }
                if (Partial.ToUpper() == "ECMDS_ACQ_DFS_DNQ_DEL_BATCH")
                {
                    FileDFSDNQDEL DataModel = new FileDFSDNQDEL(this.InstanceID, ImportFileModel, file);
                    await Event.SaveAsync(this.Class, "RouteToMethod()", string.Format("Routed to File ECMDS_ACQ_DFS_DNQ_DEL_BATCH: {0}", file.Name));
                    await DataModel.Master();
                    isMapped = true;
                }
                if (Partial.ToUpper() == "ECMDS_ACQ_DFS_DNQ_YES_BATCH")
                {
                    FileDFSDNQYES DataModel = new FileDFSDNQYES(this.InstanceID, ImportFileModel, file);
                    await Event.SaveAsync(this.Class, "RouteToMethod()", string.Format("Routed to File ECMDS_ACQ_DFS_DNQ_YES_BATCH: {0}", file.Name));
                    await DataModel.Master();
                    isMapped = true;
                }
                if (Partial.ToUpper() == "ECMDR_COMPASS_PRES_BATCH")
                {
                    FileCompassSMSCampaign DataModel = new FileCompassSMSCampaign(this.InstanceID, ImportFileModel, file);
                    await Event.SaveAsync(this.Class, "RouteToMethod()", string.Format("Routed to File ECMDR_COMPASS_PRES_BATCH: {0}", file.Name));
                    await DataModel.Master();
                    isMapped = true;
                }
                if (Partial.ToUpper() == "ECMDR_ILANGA_PRES_BATCH")
                {
                    FileIlangaSMSCampaign DataModel = new FileIlangaSMSCampaign(this.InstanceID, ImportFileModel, file);
                    await Event.SaveAsync(this.Class, "RouteToMethod()", string.Format("Routed to File ECMDR_ILANGA_PRES_BATCH: {0}", file.Name));
                    await DataModel.Master();
                    isMapped = true;
                }
                return "Done";
            }
            catch (Exception ex)
            {
                var log = new LogConsoleError(this.InstanceID, this.Class, "RouteToMethod()", ex.Message);
                await log.SaveSync();

                return "Error";
            }
        }
        #endregion
    }
}
