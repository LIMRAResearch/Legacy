<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="CreateFactTablesII.ascx.vb" Inherits="SalesSurveysApplication.CreateFactTablesII" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="ErrorMessage.ascx" TagName="ErrorMessage" TagPrefix="uc1" %>
<%@ Register Src="NotAuthorized.ascx" TagName="NotAuthorized" TagPrefix="uc2" %>

<asp:ValidationSummary ID="DateSeriesValidation" runat="server" ShowMessageBox="True"
    ShowSummary="False" />
<telerik:RadProgressManager ID="RadProgressManager1" runat="server" />
<h1>Create Fact Tables
    <asp:Literal ID="litSurveyName" runat="server"></asp:Literal></h1>
<asp:MultiView ID="MultiView1" runat="server">
    <asp:View ID="vwDefault" runat="server">
        <div class="divFullWidth">
            Please Select beginning and ending dates to create fact table:
        </div>
        <div class="divFullWidth" style="text-align: left; width: 750px; margin-top: 25px;">
            <div style="float: left; text-align: right; width: 250px; padding: 4px;">
                <asp:RequiredFieldValidator ID="ReqBeginDate" runat="server" ErrorMessage="Begin Date Required"
                    ControlToValidate="txtBeginDate" Display="Dynamic">*</asp:RequiredFieldValidator>
                &nbsp;Select Beginning Date:&nbsp;
            </div>
            <div style="float: right; width: 475px; text-align: left; padding: 4px;">
                <telerik:RadDatePicker ID="txtBeginDate" runat="server"></telerik:RadDatePicker>
            </div>
        </div>
        <br style="clear: both;" />
        <div class="divFullWidth" style="text-align: left; width: 750px;">
            <div style="float: left; text-align: right; width: 250px; padding: 4px;">
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="End Date Required"
                    ControlToValidate="txtEndDate" Display="Dynamic">*</asp:RequiredFieldValidator>
                &nbsp;Select End Date:&nbsp;
            </div>
            <div style="float: right; width: 475px; text-align: left; padding: 4px;">
                <telerik:RadDatePicker ID="txtEndDate" runat="server"></telerik:RadDatePicker>
            </div>
        </div>
        <br style="clear: both;" />
        <div class="divFullWidth" style="text-align: left; width: 750px;">
            <div style="float: left; text-align: right; width: 250px; padding: 4px;">
            </div>
            <div style="float: right; width: 475px; text-align: left; padding: 4px;">
                <asp:Button ID="btnCreateFaceTable" runat="server" Text="Create Fact Table" />
            </div>
        </div>
        <br style="clear: both;" />

        <div class="divFullWidth" style="text-align: center; width: 750px; margin-top: 50px;">
        </div>
        <div class="divFullWidth" style="text-align: center; width: 750px; margin-top: 25px; padding-left: 250px;">
            <telerik:RadProgressArea ID="RadProgressArea1" runat="server" HeaderText="Create Fact Table - Processing" ProgressIndicators="TimeElapsed" Skin="Vista" Width="250px"></telerik:RadProgressArea>

        </div>
    </asp:View>
    <asp:View ID="vwNotAuthorized" runat="server">
        <uc2:NotAuthorized ID="NotAuthorized1" runat="server" />
    </asp:View>
    <asp:View ID="vwError" runat="server">
        <uc1:ErrorMessage ID="ErrorMessage1" runat="server" />
    </asp:View>
    <asp:View ID="vwConfirm" runat="server">
        <div class="divChoose">
            You have successfully created a fact table. You may now export the data.</div>
        <div class="Spacer">&nbsp;</div>
        <div class="divFullWidth" style="text-align: left; width: 750px;">
            <div style="float: left; text-align: right; width: 250px; padding: 4px;">
                Export Fact Table: 
            </div>
            <div style="float: right; width: 475px; text-align: left; padding: 4px;"><asp:Button runat="server" ID="btnExport" Text="Export Fact Table"/>&nbsp;&nbsp;&nbsp;
                <asp:Button ID="btnMainScreen" runat="server" Text="Back to Main Screen" />
            </div>
        </div>
        <br style="clear: both;" />
    </asp:View>
</asp:MultiView>



