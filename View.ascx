<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="FortyFingers.FilecuumCleaner.View" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<script type="text/javascript">
    jQuery(function ($) {
        var setupModule = function () {
            $('#filecuumCleanerPanels').dnnPanels();
            $('#filecuumCleanerFiles').dnnTabs();
        };
        setupModule();
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            // note that this will fire when _any_ UpdatePanel is triggered,
            // which may or may not cause an issue
            setupModule();
        });
    });
</script>

<asp:Label runat="server" ID="OnlyForSuperUsers" resourcekey="OnlyForSuperUsers" CssClass="dnnFormMessage dnnFormValidationSummary" Visible="False"></asp:Label>

<asp:Panel runat="server" ID="ModuleContent">
    <div class="dnnForm" id="filecuumCleanerPanels">
        <h2 class="dnnFormSectionHead" id='panel1-<%= TabModuleId.ToString() %>'>
            <a href="#">
                <asp:Label runat="server" ID="lblStatus" resourcekey="lblStatus" CssClass="SubSubHead">Status</asp:Label></a>
        </h2>
        <fieldset>
            <div class="dnnFormItem">
                <dnn:Label resourcekey="lblCurrentlyExecuting" ID="lblCurrentlyExecuting" runat="server" Text="currently executing" CssClass="dnnLabel" />
                <div>
                    <asp:Label ID="lblExecutingJob" runat="server" CssClass="NormalBold" Width="150"></asp:Label>
                    <asp:LinkButton runat="server" ID="ClearRunningJobButton" resourcekey="ClearRunningJobButton" Text="clear (does NOT stop job)" CssClass="dnnPrimaryAction" OnClick="ClearRunningJobButton_Click"></asp:LinkButton>
                </div>
            </div>
            <div class="dnnFormItem">
                <dnn:Label runat="server" ID="lblSchedulerStatus" resourcekey="lblSchedulerStatus" Text="scheduler status" CssClass="dnnLabel"></dnn:Label>
                <div>
                    <asp:Label runat="server" ID="lblSchedulerEnabledDisabled" Width="150"></asp:Label>&nbsp;<asp:LinkButton runat="server" ID="btnEnableDisable" CssClass="dnnPrimaryAction" OnClick="btnEnableDisable_Click"></asp:LinkButton>
                </div>
            </div>
        </fieldset>
        <h2 class="dnnFormSectionHead" id='panel2-<%= TabModuleId.ToString() %>'>
            <a href="#">
                <asp:Label runat="server" ID="lblJobs" resourcekey="lblJobs" CssClass="SubSubHead"></asp:Label></a>
        </h2>
        <fieldset class="dnnClear">
            <div class="dnnFormMessage dnnFormInfo">
                <asp:Label runat="server" ID="lblInformation" resourcekey="lblInformation"></asp:Label>
            </div>
                <div class="dnnFormItem">
                    <asp:HyperLink runat="server" ID="AddJobButton" resourcekey="AddJobButton" CssClass="dnnPrimaryAction" ></asp:HyperLink>
                </div>
            <asp:Repeater runat="server" ID="rptJobs" OnItemDataBound="rptJobs_ItemDataBound" OnItemCommand="rptJobs_ItemCommand">
                <HeaderTemplate>
                    <table width="100%">
                        <tr class="dnnGridHeader">
                            <th>Name</th>
                            <th>Commands</th>
                            <th style="text-align: right;">Reorder</th>
                        </tr>
                </HeaderTemplate>
                <FooterTemplate>
                    </table>
                </FooterTemplate>
                <ItemTemplate>
                    <tr class="dnnGridItem">
                        <td>
                            <asp:Label runat="server" ID="lblJobName"></asp:Label>
                        </td>
                        <td>
                            <asp:HyperLink runat="server" ID="btnEditJob" resourcekey="btnEditJob" CssClass="dnnSecondaryAction"></asp:HyperLink>
                            <asp:LinkButton runat="server" ID="btnDeleteJob" resourcekey="btnDeleteJob" CommandName="DeleteJob" CssClass="dnnSecondaryAction"></asp:LinkButton>
                            <asp:LinkButton runat="server" ID="btnRunJob" resourcekey="btnRunJob" CommandName="RunJob" CssClass="dnnSecondaryAction"></asp:LinkButton>
                            <asp:LinkButton runat="server" ID="btnTestJob" resourcekey="btnTestJob" CommandName="TestJob" CssClass="dnnSecondaryAction" Visible="False"></asp:LinkButton>
                            <asp:LinkButton runat="server" ID="btnShowFiles" resourcekey="btnShowFiles" CommandName="ShowFiles" CssClass="dnnSecondaryAction"></asp:LinkButton>
                            <asp:LinkButton runat="server" ID="btnHideFiles" resourcekey="btnHideFiles" CommandName="HideFiles" CssClass="dnnSecondaryAction"></asp:LinkButton>
                        </td>
                        <td style="text-align: right;">
                            <asp:LinkButton runat="server" ID="btnJobUp" resourcekey="btnJobUp" CommandName="JobUp"></asp:LinkButton>
                            <asp:LinkButton runat="server" ID="btnJobDn" resourcekey="btnJobDn" CommandName="JobDn"></asp:LinkButton>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </fieldset>
    </div>
    <asp:Panel runat="server" ID="ShowFilesPanel" Visible="False" EnableViewState="False" CssClass="ShowFilesPanel">
        <div class="dnnForm" id="filecuumCleanerFiles">
            <ul class="dnnAdminTabNav">
                <li><a href="#FilesHit">
                    <asp:Label resourcekey="plFilesHit" ID="plFilesHit" controlname="lstFilesHit" runat="server" Text="id" /></a></li>
                <li><a href="#FilesSkipped">
                    <asp:Label resourcekey="plFilesSkipped" ID="plFilesSkipped" controlname="lstFilesSkipped" runat="server" Text="id" /></a></li>
            </ul>
            <div id="FilesHit">
                <asp:Repeater runat="server" OnItemDataBound="rptFiles_ItemDataBound" ID="rptFilesHit" EnableViewState="False">
                    <HeaderTemplate>
                        <ul>
                    </HeaderTemplate>
                    <FooterTemplate>
                        </ul>
                    </FooterTemplate>
                    <ItemTemplate>
                        <li>
                            <asp:Label runat="server" ID="FileLabel"></asp:Label></li>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
            <div id="FilesSkipped">
                <asp:Repeater runat="server" OnItemDataBound="rptFiles_ItemDataBound" ID="rptFilesSkipped" EnableViewState="False">
                    <HeaderTemplate>
                        <ul>
                    </HeaderTemplate>
                    <FooterTemplate>
                        </ul>
                    </FooterTemplate>
                    <ItemTemplate>
                        <li>
                            <asp:Label runat="server" ID="FileLabel"></asp:Label></li>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
    </asp:Panel>

</asp:Panel>
