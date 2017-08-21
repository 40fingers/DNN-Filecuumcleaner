using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Framework;
using DotNetNuke.Security;
using DotNetNuke.Services.Localization;
using FortyFingers.FilecuumCleaner.Components;
using FortyFingers.FilecuumCleaner.Library;

namespace FortyFingers.FilecuumCleaner
{
    public partial class Edit : PortalModuleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!UserInfo.IsSuperUser)
            {
                Response.Redirect(Globals.NavigateURL(TabId));
            }

            BatchConfig = FilecuumCleanerConfig.GetConfig(false);

            RegisterResources();

            if (!IsPostBack)
            {
                BindData();
            }
        }


		// docu bind de data haha
        private void BindData()
        {
            // we don't want the website root. At least not for now.
            // ddlRootPath.Items.Add(new ListItem(Localization.GetString("RootPath.Website", LocalResourceFile), Common.RootPathWebsite.ToString()));
            ddlRootPath.Items.Add(new ListItem(Localization.GetString("RootPath.Host", LocalResourceFile), Common.RootPathHost.ToString()));

            var portalFormatString = Localization.GetString("RootPath.Portal", LocalResourceFile);
            foreach (PortalInfo portal in new PortalController().GetPortals())
            {
                ddlRootPath.Items.Add(new ListItem(String.Format(portalFormatString, portal.PortalID, portal.PortalName), portal.PortalID.ToString()));
            }

            var job = CurrentJob();

            lblId.Text = job.Id;
            txtName.Text = job.Name;
            txtMaxAgeDays.Text = job.MaxAgeDays.ToString();
            txtMaxBytes.Text = job.MaxBytes.ToString();
            txtIncludedExtensions.Text = job.IncludedExtensions;
            chkDeleteEmptyFolders.Checked = job.DeleteEmptyFolders;
            chkIncludeSubFolders.Checked = job.IncludeSubFolders;
            chkJobEnabled.Checked = job.Enabled;
            txtPath.Text = job.Path;
            ddlRootPath.SelectValue(job.RootPath.ToString());
        }

        private FilecuumCleanerJob CurrentJob()
        {
            // get the id from the qstring
            var id = Request.QueryString["jobId"];
            FilecuumCleanerJob job;
            // create new job if no id
            if (String.IsNullOrEmpty(id))
            {
                job = new FilecuumCleanerJob();
            }
            else if (BatchConfig.FilecuumCleanerJobs.Any(j => j.Id == id))
                job = BatchConfig.FilecuumCleanerJobs.First(j => j.Id == id);
            else
            {
                // create new job with exsisting id if job not found
                // this should not happen
                job = new FilecuumCleanerJob() { Id = id };
            }

            return job;
        }

        private FilecuumCleanerConfig BatchConfig { get; set; }

        private void RegisterResources()
        {
            // jQuery.RequestRegistration();
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            var job = CurrentJob();

            if (string.IsNullOrEmpty(job.Id))
            {
                job.Id = Guid.NewGuid().ToString();
                BatchConfig.FilecuumCleanerJobs.Add(job);
            }

            job.Name = txtName.Text;
            job.MaxAgeDays = txtMaxAgeDays.TextAsInt();
            job.MaxBytes = txtMaxBytes.TextAsInt();
            job.IncludeSubFolders = chkIncludeSubFolders.Checked;
            job.IncludedExtensions = txtIncludedExtensions.Text;
            job.DeleteEmptyFolders = chkDeleteEmptyFolders.Checked;
            job.Enabled = chkJobEnabled.Checked;
            job.Path = txtPath.Text;
            job.RootPath = ddlRootPath.SelectedValueInt();
            // reset the PortalId, this ensures it will always be a valid PortalId after saving
            job.PortalId = PortalId;

            BatchConfig.ToFile();
            Response.Redirect(Globals.NavigateURL());
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(Globals.NavigateURL());
        }
    }
}