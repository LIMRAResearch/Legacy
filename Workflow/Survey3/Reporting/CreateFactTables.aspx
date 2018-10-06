<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Masters/Site.Master" CodeBehind="CreateFactTables.aspx.vb" Inherits="SalesSurveysApplication.CreateFactTables1" %>
<%@ Register src="../../../usercontrols/CreateFactTable.ascx" tagname="CreateFactTable" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:CreateFactTable ID="CreateFactTable1" runat="server" />
</asp:Content>
