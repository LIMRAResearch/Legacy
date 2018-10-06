<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Masters/Site.Master" CodeBehind="ImportDataDictionary.aspx.vb" Inherits="SalesSurveysApplication.ImportDataDictionary5" %>
<%@ Register src="../../../usercontrols/ImportDataDictionary_V2.ascx" tagname="ImportDataDictionary_V2" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:ImportDataDictionary_V2 ID="ImportDataDictionary_V21" runat="server" />
</asp:Content>
