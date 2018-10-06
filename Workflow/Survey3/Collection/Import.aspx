<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Masters/Site.Master" CodeBehind="Import.aspx.vb" Inherits="SalesSurveysApplication.Import4" %>
<%@ Register src="../../../usercontrols/WorkflowImport.ascx" tagname="WorkflowImport" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:WorkflowImport ID="WorkflowImport1" runat="server" />
</asp:Content>
