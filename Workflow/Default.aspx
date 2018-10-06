<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Masters/Site.Master"
    CodeBehind="default.aspx.vb" Inherits="SalesSurveysApplication._default0" %>

<%@ Register Src="~/usercontrols/ErrorMessage.ascx" TagName="ErrorMessage" TagPrefix="uc1" %>
<%@ Register Src="~/usercontrols/NotAuthorized.ascx" TagName="NotAuthorized" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
   
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:MultiView ID="MultiView1" runat="server">
        <asp:View ID="vwChooseSurvey" runat="server">
            <div class="divContainer">
                <h1>
                    <asp:Literal ID="litWelcomeTitle" runat="server">Welcome to the Sales Survey System</asp:Literal></h1>
            </div>
            <div class="divContainer">
                <div class="divLeft">
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Required for Edit"
                        ControlToValidate="ddlSurveys" Display="Dynamic" InitialValue="NA">*</asp:RequiredFieldValidator>
                    &nbsp;Please choose a Survey:&nbsp;</div>
                <div class="divRight">
                    <asp:DropDownList ID="ddlSurveys" runat="server" AppendDataBoundItems="True" Width="200px">
                    </asp:DropDownList>
                </div>
            </div>
            <div class="divClear" />
            <div class="divContainer">
                <div class="divLeft">
                </div>
                <div class="divRight">
                    <asp:Button ID="btnContinue" runat="server" CausesValidation="True" Text="Continue" />
                </div>
            </div>
            <div class="divClear" />
        </asp:View>
        <asp:View ID="vwError" runat="server">
            <uc1:ErrorMessage ID="ErrorMessage1" runat="server" />
        </asp:View>
        <asp:View ID="vwNotAuthorized" runat="server">
            <uc2:NotAuthorized ID="NotAuthorized1" runat="server" />
        </asp:View>
    </asp:MultiView>
</asp:Content>
