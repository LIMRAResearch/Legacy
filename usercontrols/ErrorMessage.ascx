<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ErrorMessage.ascx.vb" Inherits="SalesSurveysApplication.ErrorMessage" %>
<div style="margin-left:20px;">
    <asp:Image ID="imgNA" ImageUrl="~/images/Warning20p.GIF" AlternateText="Server Error" ImageAlign="AbsMiddle" runat="server" />&nbsp;An ERROR has ocurred and we cannot complete your request at this time. IT has been notified. Please direct all questions to 
    <asp:Literal ID="litEmail" runat="server"></asp:Literal>.
</div>