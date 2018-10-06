
<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="WorkflowBatchProcess.ascx.vb" Inherits="SalesSurveysApplication.WorkflowBatchProcess" %>
<%@ Register src="NotAuthorized.ascx" tagname="NotAuthorized" tagprefix="uc1" %>


<script type="text/javascript">
    function GetListBoxScrollPosition() {
        var sel = document.getElementById('<%=Source.ClientID%>');
        var hdnScrollTop = document.getElementById('<%=hdnScollTop.ClientID %>');
        hdnScrollTop.innerText = sel.scrollTop;
    }
    function SetListBoxScrollPosition() {
        var sel = document.getElementById('<%=Source.ClientID%>');
        var hdnScrollTop = document.getElementById('<%=hdnScollTop.ClientID %>');
        sel.scrollTop = hdnScrollTop.value; //not sure why it's in value when I clearly put it in innerText. This is what works.
    }
</script>

<asp:MultiView ID="MultiView1" runat="server">
    <asp:View ID="vwDefault" runat="server">
    
    <!-- note use of padding-left: 24.0%; padding-right: 24%; -->
    <div id="description" style="padding: 0% 24% 0% 24%;">
        <p style="padding: 2px; border: 1px dotted #C0C0C0; width:100%; text-align:justify">
            When the <b>Import</b> button is clicked this process will ensure that each selected survey in the source (left) list    
            complies with the current survey layout and load it into the database. 
            Successfully imported surveys are moved to the <b>Completed</b> directory.
            Selecting a survey will cause it's import status to be displayed in the status (right) list.
        </p>
        &nbsp;
    </div>
    <div id="labels" style="clear:both; padding: 0%; width: 100%; ">
        <!-- note use of padding-left: 24.0%; padding-right: 24%; -->
        <span id="spnSource" runat="server"  
              style="margin-left:23.5%;float: left; width: 21%; text-align: center;">
            New Surveys
        </span>
         <span id="spnImportStatus" runat="server"  
               style="float: left; margin-left:8.5%;text-align:center; width :21.5%;">
            Import Status
        </span>
       
    </div>
    <div style="clear:both">
        &nbsp;</div>
  
   <div id="controls" style="padding: 0%; width: 100%;">
       
        <div style="float:left; padding-left: 24.0%; height:350px; width:20%;padding-right: 2.5%;">
            <asp:ListBox ID="Source" runat="server" Width="100%" Height="350px" SelectionMode="Multiple"  AutoPostBack="true"></asp:ListBox>
        </div>
        
       <div id="middle" style="float: left; width: 5%;">
             <br />
            <br />
            <asp:Button ID="btnImport" runat="server" Text=" Import &gt;" class="button" BackColor="#E1E1E1"
                Width="100%" BorderColor="#999999" BorderStyle="Solid" BorderWidth="1px" />
            <br />
            <br />
            <asp:Button ID="btnOpen" runat="server" Text="&lt; Open " class="button" BackColor="#E1E1E1"
                Width="100%" BorderColor="#999999" BorderStyle="Solid" BorderWidth="1px" ToolTip="Open workbook" />
            <br />
            <br />
            <br />
            <asp:Button ID="btnNew" runat="server" Text="&lt; New" class="button" BackColor="#E1E1E1"
                Width="100%" BorderColor="#999999" BorderStyle="Solid" BorderWidth="1px" ToolTip="Display first time imports." />
                <br />
                <br />
            <asp:Button ID="btnInput" runat="server" Text="&lt; Input" class="button" BackColor="#E1E1E1"
                Width="100%" BorderColor="#999999" BorderStyle="Solid" BorderWidth="1px" ToolTip="Display imports requiring data input." />
                <br />
                <br />
            <asp:Button ID="btnErr" runat="server" Text="&lt; Error" class="button" BackColor="#E1E1E1"
                Width="100%" BorderColor="#999999" BorderStyle="Solid" BorderWidth="1px" ToolTip="Display failed imports." />
             <br />
            <br />
            <br />
             <asp:Button ID="btnInProc" runat="server" Text="In Proc &gt;" class="button" BackColor="#E1E1E1"
                Width="100%" BorderColor="#999999" BorderStyle="Solid" BorderWidth="1px" ToolTip="Display imports in process." />
                 <br />
                <br />
             <asp:Button ID="btnComplete" runat="server" Text="Done &gt;" class="button" BackColor="#E1E1E1"
                Width="100%" BorderColor="#999999" BorderStyle="Solid" BorderWidth="1px" ToolTip="Display imported surveys." />
            </div>
          
         <div id="right" style="float: left; padding-left: 2.5%; width: 20%; padding-right: 24%; height: 350px;">
            <asp:ListBox ID="Target" runat="server" Width="100%" Height="350px"  ></asp:ListBox>
            <%--<asp:Label ID="lblStatus" runat="server" Height="300px" Width="100%" style="min-height:300px; border:1px solid black;"/>--%>
       </div>
       <asp:HiddenField ID ="hdnScollTop" EnableViewState ="true" runat="server" />
   </div>
  <%-- <div class="Container" style="padding-left:15%;">     
            <asp:Menu ID="Menu1"
                        runat="server" Orientation="horizontal"
                        StaticEnableDefaultPopOutImage="False" ForeColor="Black">

                <staticselectedstyle Font-Bold="true"    
                                        BorderStyle="Outset"  BorderWidth="1px"
                                        Width="50%"/>
                <Items>
                    <asp:MenuItem Text="&nbsp; New &nbsp;" Value="0"></asp:MenuItem>
                        <asp:MenuItem Text="|" ></asp:MenuItem>
                    <asp:MenuItem Text="&nbsp; Input &nbsp;" Value="1"></asp:MenuItem>
                    <asp:MenuItem Text="|"></asp:MenuItem>
                    <asp:MenuItem Text="&nbsp; Error &nbsp;" Value="2"></asp:MenuItem>
                </Items>
            </asp:Menu> 
      
    </div>   --%>
    </asp:View>
    <asp:View ID="vwNotAuthorized" runat="server">    
        <uc1:NotAuthorized ID="NotAuthorized1" runat="server" />    
    </asp:View>
    
</asp:MultiView>