<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Masters/Site.Master" CodeBehind="DeleteEntireWorkbook.aspx.vb" Inherits="SalesSurveysApplication.DeleteEntireWorkbook4" %>
<%@ Register Src="../../../usercontrols/DeleteEntireWorkbook.ascx" tagname="DeleteWorkBook" tagprefix="tp3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
     <tp3:DeleteWorkBook ID="DeleteWorkbook" runat="server" />

</asp:Content>
