<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Masters/Site.Master"
    CodeBehind="ForgotPassword.aspx.vb" Inherits="SalesSurveysApplication.ForgotPassword" %>

<%@ Register Src="../usercontrols/ErrorMessage.ascx" TagName="ErrorMessage" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
   
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" 
        ShowMessageBox="True" ShowSummary="False" />
    <h1>
        Login Credential Reminder</h1>
 
    <asp:MultiView ID="MultiView1" runat="server">
        <asp:View ID="vwForm" runat="server">
            <div class="divContainer">
                To send yourself a username or password reminder, fill in your corporate email address
                in the textbox below, then choose the applicable button.
            </div>
           
            <div class="divContainer">
                <div class="divLeft">
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtEmail"
                        Display="Dynamic" ErrorMessage="Email Required">*</asp:RequiredFieldValidator>
                    &nbsp; Your Email:</div>
                <div class="divRight">
                    <asp:TextBox ID="txtEmail" runat="server" Width="300px"></asp:TextBox>
                </div>
            </div>
            <div class="divContainer">
                <div class="divLeft">
                    <asp:Literal ID="litUsername" runat="server" Text="Username reminder sent!"></asp:Literal>
                </div>
                <div class="divRight">
                    <asp:Button ID="btnUserName" runat="server" Text="Send Username" />
                    &nbsp;</div>
            </div>
            <div class="divContainer">
                <div class="divLeft">
                    <asp:Literal ID="litPassword" runat="server" Text="Password reminder sent!"></asp:Literal>
                </div>
                <div class="divRight">
                    <asp:Button ID="btnPassword" runat="server" Text="Send Password" />
                    &nbsp;</div>
            </div>
            <div class="divContainer" style="color: Red;">
                <asp:Literal ID="litNotFound" runat="server"></asp:Literal>
            </div>
        </asp:View>
        <asp:View ID="vwError" runat="server">
            <uc1:ErrorMessage ID="ErrorMessage1" runat="server" />
        </asp:View>
    </asp:MultiView>
</asp:Content>
