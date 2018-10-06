<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Masters/Site.Master" CodeBehind="default.aspx.vb" Inherits="SalesSurveysApplication._default21" %>
<%@ Register src="~/usercontrols/ErrorMessage.ascx" tagname="ErrorMessage" tagprefix="uc1" %>
<%@ Register src="~/usercontrols/NotAuthorized.ascx" tagname="NotAuthorized" tagprefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:MultiView ID="MultiView1" runat="server">
       <asp:View ID="vwSurvey3" runat="server">
            <div class="Container">
            Welcome - please choose from the top menu.
            </div>
            <div class="Container">
            </div>
            <div class="Container">
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
