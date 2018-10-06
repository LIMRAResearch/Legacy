<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Masters/Site.Master" CodeBehind="Format.aspx.vb" Inherits="SalesSurveysApplication.Format4" %>
<%@ Register src="../../../usercontrols/WorkflowFormat.ascx" tagname="WorkflowFormat" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:WorkflowFormat ID="WorkflowFormat1" runat="server" />
</asp:Content>
