using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Framework;
using DotNetNuke.Services.Localization;
using FortyFingers.FilecuumCleaner.Components;
using FortyFingers.FilecuumCleaner.Library;

namespace FortyFingers.FilecuumCleaner
{
    public partial class View : PortalModuleBase, IActionable
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!UserInfo.IsSuperUser)
            {
                OnlyForSuperUsers.Visible = true;
                ModuleContent.Visible = false;
                return;
            }
            if (!IsPostBack)
            {
                FillForm();
            }
        }


        private FilecuumCleanerConfig BatchConfig
        {
            get
            {
                if (_batchConfig == null)
                    _batchConfig = FilecuumCleanerConfig.GetConfig(false);

                return _batchConfig;
            }
            set { _batchConfig = value; }
        }

        private FilecuumCleanerJob _runningJob;
        private FilecuumCleanerJob RunningJob
        {
            get
            {
                if (_runningJob == null)
                {
                    var id = JobManager.GetRunningJob();
                    _runningJob = BatchConfig.FilecuumCleanerJobs.FirstOrDefault(j => j.Id == id);
                }
                return _runningJob;
            }
        }

        private void FillForm()
        {

            AddJobButton.NavigateUrl = EditUrl();

            var job = RunningJob;

            if (job != null)
                lblExecutingJob.Text = String.Format("{0} ({1})", job.Name, job.Id);
            else
                lblExecutingJob.Text = Localization.GetString("NoneExecuting.Text", LocalResourceFile);

            var scheduleItem = SchedulerHelper.GetScheduleItem(Common.SchedulerItemFullTypeName);
            bool enabled = false;
            if (scheduleItem != null)
            {
                enabled = scheduleItem.Enabled;
            }

            if (enabled)
            {
                lblSchedulerEnabledDisabled.Text = Localization.GetString("Enabled.Text", LocalResourceFile);
                btnEnableDisable.Text = Localization.GetString("Disable.Text", LocalResourceFile);
                btnEnableDisable.CommandName = "Disable";
            }
            else
            {
                lblSchedulerEnabledDisabled.Text = Localization.GetString("Disabled.Text", LocalResourceFile);
                btnEnableDisable.Text = Localization.GetString("Enable.Text", LocalResourceFile);
                btnEnableDisable.CommandName = "Enable";
            }

            rptJobs.DataSource = BatchConfig.FilecuumCleanerJobs;
            rptJobs.DataBind();
        }

        public ModuleActionCollection ModuleActions
        {
            get
            {
                var actions = new ModuleActionCollection();
                if (UserInfo.IsSuperUser)
                {
                    actions.Add(GetNextActionID(),
                                Localization.GetString("AddJob.Action", LocalResourceFile),
                                ModuleActionType.EditContent,
                                "",
                                "",
                                EditUrl(),
                                false, DotNetNuke.Security.SecurityAccessLevel.Edit,
                                true,
                                false);

                }
                return actions;
            }
        }

        protected void rptJobs_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            switch (e.Item.ItemType)
            {
                case ListItemType.Header:
                    BindrptJobsHeader();
                    break;
                case ListItemType.Footer:
                    BindrptJobsFooter();
                    break;
                case ListItemType.Item:
                case ListItemType.AlternatingItem:
                    BindrptJobsItem(e);
                    break;
            }
        }

        private void BindrptJobsFooter()
        {
        }

        private void BindrptJobsHeader()
        {
        }

        private int _itemsDataBound = 0;
        private FilecuumCleanerConfig _batchConfig;

        private void BindrptJobsItem(RepeaterItemEventArgs e)
        {
            var data = (FilecuumCleanerJob)e.Item.DataItem;

            var lbl = (Label)e.Item.FindControl("lblJobName");
            lbl.Text = String.Format("{0} ({1})", data.Name, data.Id);

            var lnk = (HyperLink)e.Item.FindControl("btnEditJob");
            lnk.NavigateUrl = EditUrl("jobId", data.Id);

            var btn = (LinkButton)e.Item.FindControl("btnDeleteJob");
            btn.CommandArgument = data.Id;
            var msg = Localization.GetString("btnDeleteJobConfirm.Text", LocalResourceFile);
            btn.Attributes.Add("onclick", String.Format("return postBackConfirm('{0}', event, '350px', '175px', '', '');", msg));

            btn = (LinkButton)e.Item.FindControl("btnRunJob");
            btn.CommandArgument = data.Id;
            btn.Enabled = RunningJob == null || RunningJob.Id != data.Id;

            btn = (LinkButton)e.Item.FindControl("btnTestJob");
            btn.CommandArgument = data.Id;
            btn.Enabled = RunningJob == null || RunningJob.Id != data.Id;

            btn = (LinkButton)e.Item.FindControl("btnShowFiles");
            btn.CommandArgument = data.Id;
            btn.Enabled = RunningJob == null || RunningJob.Id != data.Id;

            btn = (LinkButton)e.Item.FindControl("btnHideFiles");
            btn.Visible = showFilesForJob == data.Id;

            btn = (LinkButton)e.Item.FindControl("btnJobUp");
            btn.CommandArgument = data.Id;
            btn.Enabled = _itemsDataBound > 0;

            btn = (LinkButton)e.Item.FindControl("btnJobDn");
            btn.CommandArgument = data.Id;
            btn.Enabled = _itemsDataBound < (BatchConfig.FilecuumCleanerJobs.Count - 1);

            _itemsDataBound++;
        }

        protected void rptJobs_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "DeleteJob":
                    BatchConfig.DeleteJob(e.CommandArgument.ToString());
                    break;
                case "RunJob":
                    Func<string, int> jobRunner = JobManager.RunCleanerJob;
                    jobRunner.BeginInvoke(e.CommandArgument.ToString(), null, null);
                    // BatchConfig.RunJob(e.CommandArgument.ToString());
                    break;
                case "TestJob":
                    Func<string, int> jobTester = JobManager.TestCleanerJob;
                    jobTester.BeginInvoke(e.CommandArgument.ToString(), null, null);
                    // BatchConfig.RunJob(e.CommandArgument.ToString());
                    break;
                case "ShowFiles":
                    var filesHit = new List<string>();
                    var filesSkipped = new List<string>();

                    showFilesForJob = e.CommandArgument.ToString();

                    JobManager.GetCleanerJobFiles(showFilesForJob, ref filesHit, ref filesSkipped);
                    rptFilesHit.DataSource = filesHit;
                    rptFilesHit.DataBind();
                    rptFilesSkipped.DataSource = filesSkipped;
                    rptFilesSkipped.DataBind();

                    ShowFilesPanel.Visible = true;

                    break;
                case "HideFiles":
                    rptFilesHit.DataSource = new List<string>();
                    rptFilesHit.DataBind();
                    rptFilesSkipped.DataSource = new List<string>();
                    rptFilesSkipped.DataBind();
                    ShowFilesPanel.Visible = false;
                    break;
                case "JobUp":
                    BatchConfig.MoveJobUp(e.CommandArgument.ToString());
                    break;
                case "JobDn":
                    BatchConfig.MoveJobDown(e.CommandArgument.ToString());
                    break;
            }

            // refresh saved config
            BatchConfig = null;
            // refill stuff
            FillForm();
        }

        private string showFilesForJob { get; set; }
        protected void rptFiles_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            switch (e.Item.ItemType)
            {
                case ListItemType.Header:
                    BindrptFilesHeader();
                    break;
                case ListItemType.Footer:
                    BindrptFilesFooter();
                    break;
                case ListItemType.Item:
                case ListItemType.AlternatingItem:
                    BindrptFilesItem(e);
                    break;
            }
        }

        private void BindrptFilesFooter()
        {
        }

        private void BindrptFilesHeader()
        {
        }

        private void BindrptFilesItem(RepeaterItemEventArgs e)
        {
            var data = (string)e.Item.DataItem;

            var lbl = (Label)e.Item.FindControl("FileLabel");
            lbl.Text = data;
        }



        protected void ClearRunningJobButton_Click(object sender, EventArgs e)
        {
            JobManager.SetRunningJob("");
            FillForm();
        }

        protected void btnEnableDisable_Click(object sender, EventArgs e)
        {
            switch (((LinkButton)sender).CommandName)
            {
                case "Enable":
                    SchedulerHelper.EnableScheduleItem(Common.SchedulerItemFullTypeName);
                    break;
                case "Disable":
                    SchedulerHelper.DisableScheduleItem(Common.SchedulerItemFullTypeName);
                    break;
            }
            FillForm();
        }
    }
}