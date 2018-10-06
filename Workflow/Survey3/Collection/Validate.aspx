<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Masters/Site.Master" CodeBehind="Validate.aspx.vb" Inherits="SalesSurveysApplication.Validate4" %>
<%@ Register src="../../../usercontrols/WorkflowValidate.ascx" tagname="WorkflowValidate" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:WorkflowValidate ID="WorkflowValidate1" runat="server" />
</asp:Content>
