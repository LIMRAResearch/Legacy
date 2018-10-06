<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Masters/Site.Master" CodeBehind="Importflayout.aspx.vb" Inherits="SalesSurveysApplication.Importflayout3" %>
<%@ Register Src="../../../usercontrols/ImportFormLayout.ascx" tagname="Importflayout" tagprefix="tp3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
     <tp3:Importflayout ID="Importformlayout" runat="server" />

</asp:Content>
