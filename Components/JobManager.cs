using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Controllers;
using DotNetNuke.Entities.Host;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Scheduling;

namespace FortyFingers.FilecuumCleaner.Components
{
    public class JobManager
    {

        private static string RunningJobHostSetting
        {
            get { return String.Format("40F.FCln.RunningJob"); }
        }
        private static string JobLastStartedHostSetting(string jobId)
        {
            return String.Format("40F.FCln.Lst.{0}", jobId);
        }

        public static string GetRunningJob()
        {
            return HostController.Instance.GetString(RunningJobHostSetting, "");
            //var dr = HsCtrl.GetHostSetting(RunningJobHostSetting);
            //var retval = "";
            //try
            //{
            //    while (dr.Read())
            //    {
            //        retval = Convert.ToString(dr["SettingValue"]);
            //    }
            //}
            //finally
            //{
            //    if (!dr.IsClosed)
            //        dr.Close();
            //}
            //return retval;
        }

        public static bool SetRunningJob(string jobId)
        {
            var retval = false;

            // update the job
            HostController.Instance.Update(RunningJobHostSetting, jobId, true);

            // doublecheck to see if the update succeeded
            retval = (jobId == GetRunningJob());

            return retval;
        }

        public static DateTime GetJobLastStarted(string jobId)
        {
            var retval = Null.NullDate;
            try
            {
                    var s = HostController.Instance.GetString(JobLastStartedHostSetting(jobId), "");
                    DateTime.TryParse(s, out retval);
            }
            catch
            {
            }
            return retval;
        }

        public static bool SetJobLastStarted(string jobId)
        {
            var retval = false;

            var t = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

            // update the job
            HostController.Instance.Update(JobLastStartedHostSetting(jobId), t, true);
            
            // doublecheck to see if the update succeeded
            retval = (t == GetJobLastStarted(jobId).ToString("yyyy/MM/dd HH:mm:ss"));

            return retval;
        }


        public static int RunCleanerBatch(SchedulerClient schedulerClient = null)
        {
            var batchConfig = FilecuumCleanerConfig.GetConfig(false);

            var logger = Common.GetLogger();

            // check if the job exsists
            if (batchConfig == null || batchConfig.FilecuumCleanerJobs == null)
            {
                logger.Info(
                    String.Format(
                        "No cleanerjobs found in config file. This could be an issue when the site has just restarted. Next run should go better."));

                return -1;
            }

            logger.Info(String.Format("Starting batch: {0} jobs enabled", batchConfig.FilecuumCleanerJobs.Count(job => job.Enabled == true)));

            int totalCnt = 0;

            foreach (var cleanerJob in batchConfig.FilecuumCleanerJobs)
            {
                if (schedulerClient != null) schedulerClient.Progressing();

                int cnt = 0;
                // check if this job is allowed to start
                // is it enabled?
                if (!cleanerJob.Enabled)
                {
                    logger.Info(String.Format("Job: {0} is currently disabled.", cleanerJob.Name));
                    continue;
                }

                // we're allowed to start
                    logger.Info(String.Format("Starting job: {0}", cleanerJob.Name));
                    cnt = RunCleanerJob(cleanerJob.Id);
                    logger.Info(String.Format("Finished job: {0}. files deleted {1}", cleanerJob.Name, cnt));
                    totalCnt += cnt;
            }

            logger.Info(String.Format("Finished batch. Files deleted: {0}.", totalCnt));

            return totalCnt;
        }


        public static int RunCleanerJob(string jobId)
        {
            var logger = Common.GetLogger();
            var batch = FilecuumCleanerConfig.GetConfig(false);

            var filesHit = new List<string>();
            var filesSkipped = new List<string>();

            return RunCleanerJob(batch, jobId, logger, false, ref filesHit, ref filesSkipped);
        }

        public static int TestCleanerJob(string jobId)
        {
            var logger = Common.GetLogger();
            var batch = FilecuumCleanerConfig.GetConfig(false);

            var filesHit = new List<string>();
            var filesSkipped = new List<string>();

            return RunCleanerJob(batch, jobId, logger, true, ref filesHit, ref filesSkipped);
        }

        public static void GetCleanerJobFiles(string jobId, ref List<string> filesHit, ref List<string> filesSkipped)
        {
            var logger = Common.GetLogger(NLog.LogLevel.Off);
            var batch = FilecuumCleanerConfig.GetConfig(false);

            filesHit = new List<string>();
            filesSkipped = new List<string>();

            RunCleanerJob(batch, jobId, logger, true, ref filesHit, ref filesSkipped);
        }

        private static int RunCleanerJob(FilecuumCleanerConfig batchConfig, string jobId, NLog.Logger logger, bool testMode, ref List<string> filesHit, ref List<string> filesSkipped)
        {
            int retval = 0;

            logger.Info(String.Format("Starting job: {0}. Testmode: {1}", jobId, testMode));

            logger.Debug(String.Format("Setting running job to: {0}", jobId));
            // set this job as running job
            if (!SetRunningJob(jobId))
            {
                logger.Error(String.Format("Fail to register job as running: {0}", jobId));
                // this is killing for the job, cannot continue
                return retval;
            }

            logger.Debug(String.Format("Setting last started time for: {0}", jobId));
            // set the last time started
            if (!SetJobLastStarted(jobId))
            {
                logger.Error(String.Format("Fail to register job starttime: {0}", jobId));
                return retval;
            }

            // find the job to perform
            var job = batchConfig.FilecuumCleanerJobs.FirstOrDefault(j => j.Id == jobId);
            if (job == null)
            {
                logger.Error(String.Format("Job not found in configuration: {0}", jobId));
                return retval;
            }

            PortalSettings ps;
            string mapPath = "";
            switch (job.RootPath)
            {
                case Common.RootPathWebsite:
                    ps = new PortalSettings(job.PortalId);
                    mapPath = ps.HomeDirectoryMapPath.Substring(0, ps.HomeDirectoryMapPath.IndexOf("\\Portals"));
                    break;
                case Common.RootPathHost:
                    ps = new PortalSettings(job.PortalId);
                    mapPath = ps.HomeDirectoryMapPath.TrimEnd('\\');
                    mapPath = mapPath.Substring(0, mapPath.LastIndexOf("\\")) + "\\_Default";
                    break;
                default:
                    ps= new PortalSettings(job.RootPath);
                    mapPath = ps.HomeDirectoryMapPath;
                    break;
            }

            mapPath = Path.Combine(mapPath, job.Path);

            logger.Debug(String.Format("Job mappath set to: {0}", mapPath));

            if (Directory.Exists(mapPath))
            {
                CleanDirectory(mapPath, job, logger, testMode, ref filesHit, ref filesSkipped);
            }
            else
            {
                logger.Info(String.Format("Directory not found: {0}.", mapPath));
            }

            // TODO: Add folder synchronisation: we need to have a portalID for that in job settings
            // FileSystemUtils.SynchronizeFolder();

            // register job as finished
            SetRunningJob(Guid.Empty.ToString());

            return retval;
        }

        private static void CleanDirectory(string mapPath, FilecuumCleanerJob job, NLog.Logger logger, bool testMode, ref List<string> filesHit, ref List<string> filesSkipped )
        {
            // first call this method recursively for subdirectories: we want the deepest level to be cleansed first
            if(job.IncludeSubFolders)
                foreach (var subdir in Directory.GetDirectories(mapPath))
                {
                    CleanDirectory(Path.Combine(mapPath, subdir), job, logger, testMode, ref filesHit, ref filesSkipped);
                }

            logger.Debug(String.Format("Cleaning files in: {0}", mapPath));

            // then, for each file in the folder, check conditions and optionally delete it
            foreach (var filename in Directory.GetFiles(mapPath))
            {

                var file = new FileInfo(Path.Combine(mapPath, filename));

                // see if the file should be deleted because of it's size
                var deleteSize = job.MaxBytes > 0 && ( job.MaxBytes == Null.NullInteger || file.Length > job.MaxBytes);
                // see if the file should be deleted because of it's age
                var deleteAge = job.MaxAgeDays > 0 && (job.MaxAgeDays == Null.NullInteger || file.LastWriteTime.CompareTo(DateTime.Today.AddDays(-1 * job.MaxAgeDays)) < 0);

                // now also check the file's extension
                if ((deleteAge || deleteSize) && job.IncludedExtensionsList.Contains(file.Extension))
                {
                    logger.Debug(String.Format("Deleting file: {0}. Too old: {1}. Too big: {2}", filename, deleteAge, deleteSize));

                    filesHit.Add(filename);

                    if (!testMode)
                        File.Delete(file.FullName);
                }
                else
                {
                    filesSkipped.Add(filename);
                }

            }
            // now check if the folder itself needs to be deleted
            if (job.DeleteEmptyFolders && Directory.GetFiles(mapPath).Count() == 0 && Directory.GetDirectories(mapPath).Count() == 0)
            {
                logger.Debug(String.Format("Deleting folder: {0}", mapPath));
                Directory.Delete(mapPath);
                filesHit.Add(mapPath);
            }
            else
            {
                filesSkipped.Add(mapPath);
            }

        }

    }
}