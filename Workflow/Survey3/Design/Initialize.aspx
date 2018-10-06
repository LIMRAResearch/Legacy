<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Masters/Site.Master" CodeBehind="Initialize.aspx.vb" Inherits="SalesSurveysApplication.Initialize3" %>
<%@ Register Src="~/usercontrols/InitializeDateEntry.ascx" TagName="IntializeDateEntry" TagPrefix="tc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
 <tc1:IntializeDateEntry ID="InitializeDateEntry1" runat="server" />
</asp:Content>
