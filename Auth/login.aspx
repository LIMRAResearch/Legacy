<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Masters/Site.Master"
    CodeBehind="login.aspx.vb" Inherits="SalesSurveysApplication.login" %>

<%@ Register Src="~/usercontrols/ErrorMessage.ascx" TagName="ErrorMessage" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
   </asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
        ShowSummary="False" />
    <h1>
        Sales Survey Analysis Login</h1>
    <asp:MultiView ID="MultiView1" runat="server">
        <asp:View ID="vwLogin" runat="server">
            <div class="divTopText">
                Welcome to the Sales Surveys Analysis Application. Please enter your login credentials
                below. A link is provided to request a reminder if you cannot remember your credentials.</div>
            <div class="divContainer">
                <div class="divLeft">
                </div>
                <div class="divRight">
                    <asp:Label ID="lblInvalid" Visible="false" runat="server" Style="color: Red;">Invalid Username or Password.</asp:Label>
                </div>
            </div>
            <div class="divContainer">
                <div class="divLeft">
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Username Required"
                        ControlToValidate="txtUserName">*</asp:RequiredFieldValidator>
                    &nbsp;Username:</div>
                <div class="divRight">
                    <asp:TextBox ID="txtUserName" runat="server" Width="200px"></asp:TextBox>
                </div>
            </div>
            <div class="divContainer">
                <div class="divLeft">
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="Password Required"
                        ControlToValidate="txtPassword">*</asp:RequiredFieldValidator>
                    &nbsp;Password:</div>
                <div class="divRight">
                    <asp:TextBox ID="txtPassword" runat="server" Width="200px" TextMode="Password"></asp:TextBox>
                </div>
            </div>
            <div class="divContainer">
                <div class="divLeft">
                </div>
                <div class="divRight">
                    <asp:Button ID="btnSubmit" runat="server" Text="Log in" />
                </div>
            </div>
            <div class="divContainer">
                <div class="divLeft">
                </div>
                <div class="divRight">
                </div>
            </div>
            <div class="divContainer">
                <div class="divLeft">
                </div>
                <div class="divRight">
                    <asp:Image ID="imgArrow" runat="server" ImageUrl="~/Images/learnmorearrow.gif" />
                    &nbsp;<asp:LinkButton ID="lnkForgot" runat="server" CausesValidation="False">Forget Username / Password?</asp:LinkButton>
                </div>
            </div>
        </asp:View>
        <asp:View ID="vwError" runat="server">
            <uc1:ErrorMessage ID="ErrorMessage1" runat="server" />
        </asp:View>
    </asp:MultiView>
</asp:Content>
