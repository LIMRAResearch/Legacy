<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WorkflowFormat.ascx.vb" Inherits="SalesSurveysApplication.WorkflowFormat" %>
<%@ Register src="NotAuthorized.ascx" tagname="NotAuthorized" tagprefix="uc1" %>
<asp:MultiView ID="MultiView1" runat="server">
    <asp:View ID="vwDefault" runat="server">
    <div id="pagecopy">
        <!-- note use of padding-left: 24.0%; padding-right: 24%; -->
    <div id="description" style="padding: 0% 24% 0% 24%; width: 50%; ">
        <p style="padding: 2px; border: 1px dotted #C0C0C0; width:100%; text-align:justify">
            This step prepares a participant's submission for validation. Formatting ensures
            the submitted response follows the standard survey layout. Pressing <i>Copy &GT
            </i>copies the selected survey from the <i>Submitted folder</i> to the <i>Formatting
            folder</i>. When copied, some basic formatting is done on the submitted response.
            Final formatting is done manually.
        </p>
        &nbsp;
    </div>
    <div id="labels" style="padding: 0%; width: 100%; ">
        <!-- note use of padding-left: 24.0%; padding-right: 24%; -->
        <span style="float: left; padding-left: 24.0%; width: 20%; padding-right: 2.5%; text-align: center;">
            New Surveys to Format 
        </span>
        <span style="float: left; width: 5%; text-align: center; "> &nbsp; </span>
        <span style="float: left; padding-left: 2.5%; width: 20%; padding-right: 24%; text-align: center;">
            Surveys in Formatting 
        </span>
    </div>
    <div style="clear:both">
        &nbsp;</div>
    <div id="controls" style="padding: 0%; width: 100%;">
        <div id="left" style="float: left; padding-left: 24.0%; width: 20%; padding-right: 2.5%; height: 350px;">
                <asp:ListBox ID="Source" runat="server" Width="100%" Height="300px"></asp:ListBox>
        </div>
        <div id="middle" style="float: left; width: 5%; ">
            <br />
            <br />
            <asp:Button ID="btnCopy" runat="server" Text=" Copy &gt;" Width="100%" BackColor="#E1E1E1"
                BorderColor="#999999" BorderStyle="Solid" BorderWidth="1px" />
            <br />
            <br />
            <asp:Button ID="btnOpen" runat="server" Text="Open &gt;" Width="100%" BackColor="#E1E1E1"
                BorderColor="#999999" BorderStyle="Solid" BorderWidth="1px" ToolTip="Open workbook" />
        </div>
        <div id="right" style="float: left; padding-left: 2.5%; width: 20%; padding-right: 24%; height: 350px;">
            <asp:ListBox ID="Target" runat="server" Width="100%" Height="300px"></asp:ListBox>
        </div>
        <div style="clear:both"> &nbsp; </div>
        </div>
        <!-- note use of padding-left: 24.0%; padding-right: 24%; -->
        <div id="Messages" style="padding: 0% 24% 0% 24%; width: 50%;">
           <asp:Literal ID="Literal1" runat="server" ></asp:Literal>           
            &nbsp;
        </div>
  </div>

    </asp:View>
    <asp:View ID="vwNotAuthorized" runat="server">    
        <uc1:NotAuthorized ID="NotAuthorized1" runat="server" />    
    </asp:View>
    
</asp:MultiView>

