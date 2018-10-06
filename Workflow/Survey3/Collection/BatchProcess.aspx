<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/Masters/Site.Master" CodeBehind="BatchProcess.aspx.vb" Inherits="SalesSurveysApplication.BatchProcess_Survey3" %>
<%@ Register src="../../../usercontrols/WorkflowBatchProcess.ascx" tagname="WorkflowBatchProcess" tagprefix="uc1" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:WorkflowBatchProcess ID="WorkflowBatchProcess1" runat="server" />
</asp:Content>

