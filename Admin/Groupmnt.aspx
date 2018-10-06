<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Masters/Admin.Master" CodeBehind="Groupmnt.aspx.vb" Inherits="SalesSurveysApplication.Groupmnt" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="../usercontrols/ErrorMessage.ascx" TagName="ErrorMessage" TagPrefix="uc1" %>
<%@ Register Src="../usercontrols/NotAuthorized.ascx" TagName="NotAuthorized" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
        ShowSummary="False" />
    <h1>
        Group Maintenance</h1>
        
    <asp:MultiView ID="MultiView1" runat="server">
    
        <asp:View ID="vwDefault" runat="server">
            <div class="divFullWidth">
                Please choose a group to edit, or click on Add New.</div>
            <div class="divChoose">
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Required for Edit"
                    ControlToValidate="ddlGroups" Display="Dynamic" InitialValue="NA">*</asp:RequiredFieldValidator>
                &nbsp;Choose a Group:&nbsp;<asp:DropDownList ID="ddlGroups" 
                    runat="server" AppendDataBoundItems="True" Width="200px">
                </asp:DropDownList>
                &nbsp;<asp:Button ID="btnGoToEdit" runat="server" Text="Edit Group" 
                    Width="110px" />
                &nbsp;
                <asp:Button ID="btnGoToAdd" runat="server" Text="Add New Group" 
                    CausesValidation="False" Width="140px" />
            </div>
        </asp:View>
        
        <asp:View ID="vwEdit" runat="server">
            <div class="divContainer">
                <div class="divLeft">
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                        ControlToValidate="txtGroupName" Display="Dynamic" 
                        ErrorMessage="Group Name Required">*</asp:RequiredFieldValidator>
                    &nbsp;Group Name:</div>
                <div class="divRight">
                    <asp:TextBox ID="txtGroupName" runat="server" Width="100px"></asp:TextBox>                      
                </div>
            </div>
             <div class="divContainer">
                <div class="divLeft">
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" 
                        ControlToValidate="txtGroupDescription" Display="Dynamic" 
                        ErrorMessage="Group Description Required">*</asp:RequiredFieldValidator>
                    &nbsp;Group Description:</div>
                <div class="divRight">
                    <asp:TextBox ID="txtGroupDescription" runat="server" Width="200px"></asp:TextBox>
                </div>
            </div>            

             <div class="divContainer">
                <div class="divLeft">
                </div>
                <div class="divRight">
                    <asp:Button ID="btnAdd" runat="server" Text="Add New Group" />
                    &nbsp;
                    <asp:Button ID="btnSave" runat="server" Text="Save Changes" />
                    &nbsp;
                    <asp:Button ID="btnCancel" runat="server" CausesValidation="False" 
                        Text="Cancel" />
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:Button ID="btnDelete" runat="server" CausesValidation="False" 
                        Text="Delete" />
                </div>
            </div>
        </asp:View>
        <asp:View ID="vwNotAuthorized" runat="server">
            <uc2:NotAuthorized ID="NotAuthorized1" runat="server" />
        </asp:View>
        <asp:View ID="vwError" runat="server">
            <uc1:ErrorMessage ID="ErrorMessage1" runat="server" />
        </asp:View>
    </asp:MultiView>
</asp:Content>
