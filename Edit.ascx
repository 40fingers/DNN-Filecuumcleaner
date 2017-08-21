<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Edit.ascx.cs" Inherits="FortyFingers.FilecuumCleaner.Edit" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<div class="FilecuumCleanerEdit dnnForm">
    <asp:Label ID="InstructionsLabel" runat="server" CssClass="dnnFormMessage dnnFormInfo" ResourceKey="InstructionsLabel" />
    <div class="dnnFormItem dnnFormHelp dnnClear">
        <p class="dnnFormRequired"><asp:Label ID="RequiredIndicatorLabel" runat="server" ResourceKey="RequiredIndicatorLabel" /></p>
    </div>
    <fieldset>
        <div class="dnnFormItem">
            <dnn:Label resourcekey="plId" id="plId" controlname="lblId" runat="server" Text="id" suffix=":" />
            <asp:Label ID="lblId" runat="server" CssClass="NormalBold" Width="300"></asp:Label>
        </div>
        <div class="dnnFormItem">
            <dnn:Label resourcekey="plName" id="plName" controlname="txtName" runat="server" Text="name" suffix=":" />
            <asp:TextBox ID="txtName" runat="server" Width="300"></asp:TextBox>
        </div>
        <div class="dnnFormItem">
            <dnn:Label resourcekey="plRootPath" id="plRootPath" controlname="ddlRootPath" runat="server" Text="RootPath" suffix=":" />
            <asp:DropDownList runat="server" ID="ddlRootPath"/>
        </div>
        <div class="dnnFormItem">
            <dnn:Label resourcekey="plPath" id="plPath" controlname="txtPath" runat="server" Text="Path" suffix=":" />
            <asp:TextBox ID="txtPath" runat="server" CssClass="dnnFormRequired" Width="200"></asp:TextBox>
            <asp:RequiredFieldValidator runat="server" ID="rfvPath" resourcekey="Path.Required" ControlToValidate="txtPath" 
                ValidationGroup="EditFilecuumCleanerJob" Display="Dynamic" CssClass="dnnFormMessage dnnFormError"></asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator runat="server" ID="PathValidator" ControlToValidate="txtPath" resourcekey="Path.Invalid" ValidationExpression="^[^\.\.]*$" CssClass="dnnFormMessage dnnFormError"></asp:RegularExpressionValidator>
        </div>
        <div class="dnnFormItem">
            <dnn:Label runat="server" id="plIncludedExtensions" controlname="txtIncludedExtensions" resourcekey="plIncludedExtensions" Text="included extensions" suffix=":"></dnn:Label>
            <asp:TextBox runat="server" ID="txtIncludedExtensions" CssClass="dnnFormRequired"></asp:TextBox>
            <asp:RequiredFieldValidator runat="server" ID="rfvIncludedExtensions" resourcekey="IncludedExtensions.Required" ControlToValidate="txtIncludedExtensions" 
                ValidationGroup="EditFilecuumCleanerJob" Display="Dynamic" CssClass="dnnFormMessage dnnFormError"></asp:RequiredFieldValidator>
        </div>
        <div class="dnnFormItem">
            <dnn:Label resourcekey="plIncludeSubFolders" id="plIncludeSubFolders" controlname="chkIncludeSubFolders" runat="server" Text="IncludeSubFolders" suffix=":" />
            <asp:CheckBox ID="chkIncludeSubFolders" runat="server"></asp:CheckBox>
        </div>
        <div class="dnnFormItem">
            <dnn:Label resourcekey="plDeleteEmptyFolders" id="plDeleteEmptyFolders" controlname="chkDeleteEmptyFolders" runat="server" Text="DeleteEmptyFolders" suffix=":" />
            <asp:CheckBox ID="chkDeleteEmptyFolders" runat="server"></asp:CheckBox>
        </div>
        <div class="dnnFormItem">
            <dnn:Label resourcekey="plMaxAgeDays" id="plMaxAgeDays" controlname="txtMaxAgeDays" runat="server" Text="MaxAgeDays" suffix=":" />
            <asp:TextBox ID="txtMaxAgeDays" runat="server" Width="50" MaxLength="3"></asp:TextBox>
            <asp:RegularExpressionValidator runat="server" ID="MaxAgeDaysValidator" ControlToValidate="txtMaxAgeDays" CssClass="dnnFormMessage dnnFormError" ValidationExpression="^\d*$" resourcekey="MaxAgeDays.Invalid"></asp:RegularExpressionValidator>
        </div>
        <div class="dnnFormItem">
            <dnn:Label resourcekey="plMaxBytes" id="plMaxBytes" controlname="txtMaxBytes" runat="server" Text="MaxBytes" suffix=":" />
            <asp:TextBox ID="txtMaxBytes" runat="server" Width="100" MaxLength="10"></asp:TextBox>
            <asp:RegularExpressionValidator runat="server" ID="MaxBytesValidator" ControlToValidate="txtMaxBytes" CssClass="dnnFormMessage dnnFormError" ValidationExpression="^\d*$" resourcekey="MaxBytes.Invalid"></asp:RegularExpressionValidator>
        </div>
        <div class="dnnFormItem">
            <dnn:Label resourcekey="plJobEnabled" id="plJobEnabled" controlStartTime="chkJobEnabled" runat="server" Text="enabled" suffix=":" />
            <asp:CheckBox runat="server" ID="chkJobEnabled"/>
        </div>
    </fieldset>
    <ul class="dnnActions dnnClear">
        <li><asp:LinkButton runat="server" ID="btnUpdate" resourcekey="btnUpdate" CssClass="dnnPrimaryAction" ValidationGroup="EditFilecuumCleanerJob" OnClick="btnUpdate_Click"></asp:LinkButton></li>
        <li><asp:LinkButton runat="server" ID="btnCancel" resourcekey="btnCancel" CssClass="dnnSecondaryAction" OnClick="btnCancel_Click"></asp:LinkButton></li>
    </ul>

</div>
