<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="DeleteEntireWorkbook.ascx.vb" Inherits="SalesSurveysApplication.DeleteEntireWorkbook3" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/usercontrols/ErrorMessage.ascx" TagName="ErrorMessage" TagPrefix="uc1" %>
<%@ Register Src="~/usercontrols/NotAuthorized.ascx" TagName="NotAuthorized" TagPrefix="uc2" %>




        <asp:MultiView ID="MultiView1" runat="server">
            <asp:View ID="vwChooseSurveySeries" runat="server">
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" 
        ShowMessageBox="True" ShowSummary="False" />
            <div class="Container">
            </div>
            <div class="Container LabelText">
                Please choose a survey by selecting all of the parameters below:</div>
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
                </div>
            </div>
            <div class="divContainer">
                <div class="divLeft">
                </div>
                <div class="divRight">
                    <asp:Button ID="btnContinue" runat="server" Text="Delete Workbook"
                    Width="120px" />
                </div>
            </div>            
        </asp:View>
        <asp:View ID="vwError" runat="server">
            <uc1:ErrorMessage ID="ErrorMessage1" runat="server" />
        </asp:View>
        <asp:View ID="vwNotAuthorized" runat="server">
            <uc2:NotAuthorized ID="NotAuthorized1" runat="server" />
        </asp:View>
    </asp:MultiView>
