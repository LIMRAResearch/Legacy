<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="UpdateMOR.ascx.vb" Inherits="SalesSurveysApplication.UpdateMOR3" %>

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
                    <asp:DropDownList ID="ddlDateSeries" runat="server" AutoPostBack="true">
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
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" 
                        ErrorMessage="Page/ Worksheet Required" 
                        ControlToValidate="ddlDistributions" Display="Dynamic" InitialValue="NA">*</asp:RequiredFieldValidator>
                    &nbsp;Page/ Worksheet:</div>
                <div class="divRight">
                    <asp:DropDownList ID="ddlDistributions" runat="server" 
                        AppendDataBoundItems="True" Width="200px">
                    </asp:DropDownList>
                </div>
            </div>
            <div class="divContainer">
                <div class="divLeft">
                    <asp:CompareValidator ID="CompareValidator1" runat="server" 
                        ControlToValidate="txtMOR" Display="Dynamic" 
                        ErrorMessage="Integers Only for MOR" Operator="DataTypeCheck" Type="Integer">*</asp:CompareValidator>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" 
                        ErrorMessage="MOR Required" ControlToValidate="txtMOR" Display="Dynamic" 
                        InitialValue="NA">*</asp:RequiredFieldValidator>
                    &nbsp;Current MOR:</div>
                <div class="divRight">
                    <asp:TextBox ID="txtMOR" runat="server" Width="50px"></asp:TextBox>
                </div>
            </div>
            
            <div class="divContainer">
                <div class="divLeft">
                    <asp:CompareValidator ID="CompareValidator2" runat="server" 
                        ControlToValidate="txtMORNew" Display="Dynamic" 
                        ErrorMessage="Integers Only for New MOR" Operator="DataTypeCheck" Type="Integer">*</asp:CompareValidator>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                        ErrorMessage="New MOR Required" ControlToValidate="txtMORNew" Display="Dynamic" 
                        InitialValue="NA">*</asp:RequiredFieldValidator>
                    &nbsp;New MOR:</div>
                <div class="divRight">
                    <asp:TextBox ID="txtMORNew" runat="server" Width="50px"></asp:TextBox>
                </div>
            </div>           
            <div class="divContainer">
                <div class="divLeft">
                </div>
                <div class="divRight">
                    <asp:Button ID="btnContinue" runat="server" Text="Update MOR"
                    Width="87px" />
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