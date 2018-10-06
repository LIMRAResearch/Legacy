<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="CreateFactTable.ascx.vb"
    Inherits="SalesSurveysApplication.CreateFactTable1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="ErrorMessage.ascx" TagName="ErrorMessage" TagPrefix="uc1" %>
<%@ Register Src="NotAuthorized.ascx" TagName="NotAuthorized" TagPrefix="uc2" %>

<asp:ValidationSummary ID="DateSeriesValidation" runat="server" ShowMessageBox="True"
    ShowSummary="False" />
<h1>Create Fact Tables
    <asp:Literal ID="litSurveyName" runat="server"></asp:Literal></h1>
<asp:MultiView ID="MultiView1" runat="server">
    <asp:View ID="vwDefault" runat="server">
        <div class="divFullWidth">
            Please choose a survey series to Create Fact Tables
        </div>
        <div class="divFullWidth" style="text-align: center; width: 750px; margin-top: 25px;">
            <div class="divFullWidth" style="text-align: center; width: 750px; margin-top: 25px;">
                <asp:RequiredFieldValidator ID="RequiredSurvey" runat="server" ErrorMessage="Required for Edit"
                    ControlToValidate="ddlSurveySeries" Display="Dynamic" InitialValue="NA">*</asp:RequiredFieldValidator>
                Survey Series:&nbsp;
                <asp:DropDownList ID="ddlSurveySeries" runat="server" AppendDataBoundItems="True">
                </asp:DropDownList>
                <asp:Button ID="btnSelectDateSeries" runat="server" Text="Continue" Style="position: relative; left: 10px" />
            </div>
            <div class="divFullWidth" style="text-align: center; margin-top: 50px; padding-left: 250px;">
                <asp:Panel ID="Panel1" runat="server" HorizontalAlign="Center" Height="275px" Style="padding-top: 15px; padding-left: 15px">

                    <div style="padding-top: 250px;"></div>
                </asp:Panel>
            </div>
            <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
                <AjaxSettings>
                    <telerik:AjaxSetting AjaxControlID="Panel1">
                        <UpdatedControls>
                            <telerik:AjaxUpdatedControl ControlID="Panel1" LoadingPanelID="RadAjaxLoadingPanel1"></telerik:AjaxUpdatedControl>
                        </UpdatedControls>
                    </telerik:AjaxSetting>
                </AjaxSettings>
            </telerik:RadAjaxManager>
            <div>
                <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" Skin="Default"></telerik:RadAjaxLoadingPanel>
            </div>
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
            You have successfully created fact tables. You may now export data for final reports.
        </div>
    </asp:View>
</asp:MultiView>
