using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Services.Scheduling;

namespace FortyFingers.FilecuumCleaner.Components
{
    public class FilecuumCleanerTask : SchedulerClient
    {
        public FilecuumCleanerTask(ScheduleHistoryItem oItem)
            : base()
        {
            this.ScheduleHistoryItem = oItem;
        }

        #region Overrides of SchedulerClient

        public override void DoWork()
        {
            var jobGuid = Guid.NewGuid();

            try
            {
                //Perform required items for logging
                ScheduleHistoryItem.AddLogNote(String.Format("Cleaner started on thread {0}, jobguid: {1}.<br />", ThreadID, jobGuid));
                Progressing();
                // ScheduleHistoryItem.AddLogNote(String.Format("{0}<br />", "my message"));

                var cnt = JobManager.RunCleanerBatch(this);
                if (cnt >= 0)
                {
                    // Show success
                    ScheduleHistoryItem.AddLogNote(String.Format("Files deleted: {0}<br />", cnt));
                    ScheduleHistoryItem.AddLogNote(String.Format("Cleaner succeeded on thread {0}, jobguid: {1}.<br />", ThreadID, jobGuid));
                    ScheduleHistoryItem.Succeeded = true;
                }
            }
            catch (Exception ex)
            {
                ScheduleHistoryItem.Succeeded = false;
                ScheduleHistoryItem.AddLogNote(String.Format("Exception occurred: {0}<br />", ex.Message));
                ScheduleHistoryItem.AddLogNote(String.Format("Cleaner failed on thread {0}, jobguid: {1}.<br />", ThreadID, jobGuid));
                DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
                Errored(ref ex);
            }
        }

        #endregion
    }
}