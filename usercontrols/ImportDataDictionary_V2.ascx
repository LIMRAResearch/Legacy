﻿<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ImportDataDictionary_V2.ascx.vb"
    Inherits="SalesSurveysApplication.ImportDataDictionary_V2" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="NotAuthorized.ascx" TagName="NotAuthorized" TagPrefix="uc1" %>
<%@ Register Src="ErrorMessage.ascx" TagName="ErrorMessage" TagPrefix="uc2" %>
<asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
    ShowSummary="False" />
<h1>
    Import Data Dictionary
    <asp:Literal ID="litSurveyName" runat="server"></asp:Literal>
</h1>
<asp:MultiView ID="MultiView1" runat="server">
    <asp:View ID="vwDefault" runat="server">
        <div class="divChoose">
            Running this program will validate the data in the Data Dictionary workbook for
            PK=&gt;FK relationshipts, and will aslo validate against existing data in the fData,
            cData, ddFields, ddcFields, ddAlgorithms and ddAttbributes tables in the database.
            Finally, the program will validate the integrity of the data between the four worksheets
            being imported. In the event of a validation failure, the import program will abort,
            and display a message stating where the failure occurred, and list the FieldID&#39;s
            affected. Upon a successful import, a confirmation message will be displayed.
        </div>
        <div style="clear: both;" />
        <div class="divFullWidth" style="text-align: center; margin-top: 50px;">
         <asp:Button ID="btnImport" runat="server" Text="Import Data Dictionary" /></div>
          <div class="divFullWidth" style="text-align: center; margin-top: 50px; padding-left: 250px;">
          
            <telerik:RadProgressManager ID="RadProgressManager1" runat="server" />
            <telerik:RadProgressArea  ID="RadProgressArea1" runat="server" Language="" 
                ProgressIndicators="TotalProgressBar, TimeElapsed" Skin="Sitefinity" 
                  HeaderText="Import Data Dictionary Progress">
            <Localization Uploaded="Uploaded"></Localization>
            </telerik:RadProgressArea>
          
        </div>       
         <telerik:RadAjaxManager  ID="RadAjaxManager1" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="RadProgressManager1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl  ControlID="RadProgressManager1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    </asp:View>
    <asp:View ID="vwAbort" runat="server">
        <div class="divChoose" style="padding-bottom: 15px">
            <span style="color: Red; font-weight: bold; font-size: 18px;">Import Aborted</span><br />
            <br />
            <asp:Literal ID="litAbort" runat="server"></asp:Literal>
        </div>
    </asp:View>
    <asp:View ID="vwConfirm" runat="server">
        <div class="divChoose" style="padding-bottom: 15px">
            You have successfully imported the data dictionary worksheets for this survey.
        </div>
    </asp:View>
    <asp:View ID="vwNotAuthorized" runat="server">
        <uc1:NotAuthorized ID="NotAuthorized1" runat="server" />
    </asp:View>
    <asp:View ID="vwError" runat="server">
        <uc2:ErrorMessage ID="ErrorMessage1" runat="server" />
    </asp:View>
</asp:MultiView>