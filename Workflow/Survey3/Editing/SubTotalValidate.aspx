<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Masters/Site.Master" CodeBehind="SubTotalValidate.aspx.vb" Inherits="SalesSurveysApplication.SubTotalValidate3" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/usercontrols/ErrorMessage.ascx" TagName="ErrorMessage" TagPrefix="uc1" %>
<%@ Register Src="~/usercontrols/NotAuthorized.ascx" TagName="NotAuthorized" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
  <asp:MultiView ID="MultiView1" runat="server">
            <asp:View ID="vwChooseSurveySeries" runat="server">
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" 
        ShowMessageBox="True" ShowSummary="False" />
            <div class="Container">
            </div>
            <div class="Container LabelText">
                Please choose a workbook to validate by choosing a Survey Series, then a workbook:</div>
            <div class="Spacer">
                &nbsp;</div>
            <div class="Container LabelText RedText"><asp:Literal ID="litStatus" runat="server"></asp:Literal></div>
            <div class="divContainer">
                <div class="divLeft">
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                        ErrorMessage="Date Series Required" ControlToValidate="ddlDateSeries" 
                        Display="Dynamic" InitialValue="NA">*</asp:RequiredFieldValidator>
                    &nbsp;Date Series:</div>
                <div class="divRight">
                    <asp:DropDownList ID="ddlDateSeries" runat="server" AutoPostBack="True">
                    </asp:DropDownList>
                    &nbsp;<asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" 
                        ControlToValidate="ddlDateSeries" Display="Dynamic" 
                        ErrorMessage="Date Series Required" InitialValue="NA">*</asp:RequiredFieldValidator>
                </div>
            </div>
            <div class="divContainer">
                <div class="divLeft">
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" 
                        ErrorMessage="Workbook Required" ControlToValidate="ddlWorkbooks" 
                        Display="Dynamic" InitialValue="NA">*</asp:RequiredFieldValidator>
                    &nbsp;Workbook:</div>
                <div class="divRight">
                    <asp:DropDownList ID="ddlWorkbooks" runat="server">
                    </asp:DropDownList>
                    &nbsp;<asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" 
                        ControlToValidate="ddlWorkbooks" Display="Dynamic" 
                        ErrorMessage="Workbook Required" InitialValue="NA">*</asp:RequiredFieldValidator>
                </div>
            </div>
             <div class="Spacer">
                &nbsp;</div>
            <div class="divContainer" >
                <div class="divLeft">
                   </div>
                <div class="divRight">
                    <asp:Button ID="btnValidate" runat="server" text="Validate Workbook" />
                </div>
            </div>                    
                      
                      
            <div class="divContainer">

            <div style="padding: 0% 24% 0% 24%; width: 50%;">
                <telerik:RadProgressManager ID="RadProgressManager1" runat="server" />
                <telerik:RadProgressArea ID="RadProgressArea1" runat="server" Language="" 
                    ProgressIndicators="TotalProgressBar, TimeElapsed" Skin="Sitefinity">
                </telerik:RadProgressArea>
            </div>
                <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
                    <ajaxsettings>
                        <telerik:AjaxSetting AjaxControlID="RadProgressArea1">
                            <updatedcontrols>
                                <telerik:AjaxUpdatedControl ControlID="RadProgressArea1" />
                            </updatedcontrols>
                        </telerik:AjaxSetting>
                    </ajaxsettings>
                </telerik:RadAjaxManager>
            </div>  
                 
        </asp:View>
        <asp:View ID="vwReport" runat="server">
        <div style="text-align: left;">
          <asp:Literal ID="litBadTotals" runat="server"></asp:Literal><br /><br /><br />
          </div>
          <div class="divContainer TextCenter">
            <asp:Button ID="btnSubmit" runat="server" Text="Run another Validation" />
        </div>
        </asp:View>
         <asp:View ID="vwError" runat="server">
            <uc1:ErrorMessage ID="ErrorMessage1" runat="server" />
        </asp:View>
        <asp:View ID="vwNotAuthorized" runat="server">
            <uc2:NotAuthorized ID="NotAuthorized1" runat="server" />
        </asp:View>
    </asp:MultiView>
</asp:Content>
