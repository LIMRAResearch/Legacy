<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WorkflowValidate.ascx.vb" Inherits="SalesSurveysApplication.WorkflowValidate" %>
<%@ Register src="NotAuthorized.ascx" tagname="NotAuthorized" tagprefix="uc1" %>
<asp:MultiView id="MultiView1" runat="server">
    <asp:View ID="vwDefault" runat="server">
    <div id="pagecopy">
        <!-- note use of padding-left: 24.0%; padding-right: 24%; -->
        <div id="description" style="padding: 0% 24% 0% 24%; width: 50%;">
            <p style="padding: 2px; border: 1px dotted #C0C0C0; width: 100%; text-align: justify">
                This step moves a formatted form to the validation folder for one final check before
                uploading the into the database. Pressing <i>Copy &GT </i>moves the selected survey
                from the <i>Formatting folder</i> to the <i>Validating folder</i>. Data are shaded
                green if validated and yellow othewise.
            </p>
            &nbsp;
        </div>
        <div id="labels" style="padding: 0%; width: 100%;">
            <!-- note use of padding-left: 24.0%; padding-right: 24%; -->
            <span style="float: left; padding-left: 24.0%; width: 20%; padding-right: 2.5%; text-align: center;">
                Surveys in Formatting </span><span style="float: left; width: 5%; text-align: center;">
                    &nbsp; </span><span style="float: left; padding-left: 2.5%; width: 20%; padding-right: 24%;
                        text-align: center;">Surveys in Validation </span>
        </div>
        <div style="clear: both">
            &nbsp;
        </div>
        <div id="controls" style="padding: 0%; width: 100%;">
            <div id="left" style="float: left; padding-left: 24.0%; width: 20%; padding-right: 2.5%;
                height: 350px;">
                <asp:ListBox ID="Source" runat="server" Width="100%" Height="300px"></asp:ListBox>
            </div>
            <div id="middle" style="float: left; width: 5%;">
                <br />
                <br />
                <asp:Button ID="btnMove" runat="server" Text="Move &gt;" Width="100%" BackColor="#E1E1E1"
                    BorderColor="#999999" BorderStyle="Solid" BorderWidth="1px" />
                <br />
                <br />
                <asp:Button ID="btnOpenRight" runat="server" Text="Open &gt;" Width="100%" BackColor="#E1E1E1"
                    BorderColor="#999999" BorderStyle="Solid" BorderWidth="1px" ToolTip="Open workbook" />
                <br />
                <br />
                <asp:Button ID="btnOpenLeft" runat="server" Text="&lt; Open" Width="100%" BackColor="#E1E1E1"
                    BorderColor="#999999" BorderStyle="Solid" BorderWidth="1px" ToolTip="Open workbook" />
            </div>
            <div id="right" style="float: left; padding-left: 2.5%; width: 20%; padding-right: 24%;
                height: 350px;">
                <asp:ListBox ID="Target" runat="server" Width="100%" Height="300px"></asp:ListBox>
            </div>
            <div style="clear: both">
                &nbsp;</div>
            <!-- note use of padding-left: 24.0%; padding-right: 24%; -->
            <div id="Messages" style="padding: 0% 24% 0% 24%; width: 50%;">
                <asp:Literal ID="Literal1" runat="server"></asp:Literal>
            </div>
        </div>
    </div>

    </asp:View>
    <asp:View ID="vwNotAuthorized" runat="server">
    
        <uc1:NotAuthorized ID="NotAuthorized1" runat="server" />
    
    </asp:View>
</asp:MultiView>
