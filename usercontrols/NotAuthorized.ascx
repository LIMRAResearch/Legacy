<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="NotAuthorized.ascx.vb" Inherits="SalesSurveysApplication.NotAuthorized" %>
<div style="margin-left:20px;">
    <asp:Image ID="imgNA" ImageUrl="~/images/Warning20p.GIF" AlternateText="Not Authorized" ImageAlign="AbsMiddle" runat="server" />&nbsp;You are not authorized to access this page. Please direct all questions to 
    <asp:Literal ID="litEmail" runat="server"></asp:Literal>.
</div>