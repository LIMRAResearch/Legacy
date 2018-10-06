<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WorkflowImport.ascx.vb" Inherits="SalesSurveysApplication.WorkflowImport" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register src="NotAuthorized.ascx" tagname="NotAuthorized" tagprefix="uc1" %>
<asp:MultiView ID="MultiView1" runat="server">
    <asp:View ID="vwDefault" runat="server">
     <div id="pagecopy">
        <!-- note use of padding-left: 24.0%; padding-right: 24%; -->
        <div id="description" style="padding: 0% 24% 0% 24%; width: 50%;">
            <p style="padding: 2px; border: 1px dotted #C0C0C0; width: 100%; text-align: justify">
                This step does one final check then uploads the data in the database.
                Pressing <i>Move &GT </i> uploads the survey and moves the workbook to 
                the <i>Completed folder</i>. 
            </p>
            &nbsp;
        </div>
        <div id="labels" style="padding: 0%; width: 100%;">
            <!-- note use of padding-left: 24.0%; padding-right: 24%; -->
            <span style="float: left; padding-left: 24.0%; width: 20%; padding-right: 2.5%; text-align: center;">
                Surveys in Validation </span><span style="float: left; width: 5%; text-align: center;">
                    &nbsp; </span><span style="float: left; padding-left: 2.5%; width: 20%; padding-right: 24%;
                        text-align: center;">Surveys Completed </span>
        </div>
        <div style="clear: both">
            &nbsp;</div>
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
                    BorderColor="#999999" BorderStyle="Solid" BorderWidth="1px" ToolTip="Open workbook" Visible="False" />
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
            <div style="padding: 0% 30% 0% 30%; width: 30%;">
                <telerik:RadProgressManager ID="RadProgressManager1" runat="server" />
                <telerik:RadProgressArea ID="RadProgressArea1" runat="server" Language="" ProgressIndicators="TotalProgressBar, TimeElapsed"
                    Skin="Sitefinity">
                </telerik:RadProgressArea>
            </div>
            <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
                <AjaxSettings>
                    <telerik:AjaxSetting AjaxControlID="RadProgressArea1">
                        <UpdatedControls>
                            <telerik:AjaxUpdatedControl ControlID="RadProgressArea1" />
                        </UpdatedControls>
                    </telerik:AjaxSetting>
                </AjaxSettings>
            </telerik:RadAjaxManager>
                
            </div>
        </div>
    </asp:View>
    <asp:View ID="vwNotAuthorized" runat="server">
    
        <uc1:NotAuthorized ID="NotAuthorized1" runat="server" />
    
    </asp:View>
</asp:MultiView>
