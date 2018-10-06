<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="GlobalError.aspx.vb" Inherits="SalesSurveysApplication.GlobalError" MasterPageFile="~/Masters/Site.Master"%>

<%@ Register src="../usercontrols/ErrorMessage.ascx" tagname="ErrorMessage" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <div>
      
            <uc1:ErrorMessage ID="ErrorMessage1" runat="server" />
            
        </div>
</asp:Content>