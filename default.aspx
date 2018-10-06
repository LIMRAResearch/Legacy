<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Masters/Site.Master" CodeBehind="default.aspx.vb" Inherits="SalesSurveysApplication._default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<asp:MultiView ID="MultiView1" runat="server">
    <asp:View ID="vwDefault" runat="server">
    Welcome 
    </asp:View>
    <asp:View ID="vwError" runat="server">
    
    </asp:View>

</asp:MultiView>
</asp:Content>
