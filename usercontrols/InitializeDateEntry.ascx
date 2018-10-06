<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="InitializeDateEntry.ascx.vb" Inherits="SalesSurveysApplication.InitializeDateEntry" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
 <%@ Register Src="NotAuthorized.ascx" TagName="NotAuthorized" TagPrefix="uc1" %>
<%@ Register Src="ErrorMessage.ascx" TagName="ErrorMessage" TagPrefix="uc2" %> 
 
<asp:ValidationSummary ID="DateSeriesValidation" runat="server" ShowMessageBox="True"
        ShowSummary="False" />
    
    <h1>Survey Date Series Maintenance
    <asp:Label ID="lblSurveyIDSelect" runat="server"></asp:Label>
    </h1>
    
    <asp:MultiView ID="MultiView1" runat="server">
        
        <asp:View ID="vwDateSeriesSelect" runat="server">
           
           
            
                  
            <div class="divFullWidth">
                Please choose a date series to edit or add a new date series
             </div>
            <div class="divChoose">
                    
                Please Choose a Date Series:&nbsp;<asp:DropDownList ID="ddlDateSeries" runat="server" AppendDataBoundItems="True">
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="RequiredDateSeries" runat="server" ErrorMessage="Required for Edit"
                    ControlToValidate="ddlDateSeries" Display="Dynamic" InitialValue="NA">*</asp:RequiredFieldValidator>
                
                <asp:Button ID="btnEditDateSeries" runat="server" Text="Edit Date Series" Style="position:relative;left:10px" />
                <asp:Button ID="btnAddDateSeries" runat="server" Text="Add Date Series"  Style="position:relative;left:20px" CausesValidation="false"/> 
                
            </div>
        </asp:View>
        
        <asp:View ID="vwDateSeriesEdit" runat="server">
          <!-- <div class="Container LabelText RedText"><asp:Literal ID="litStatus" runat="server"></asp:Literal></div>
            <h6>Survey ID:<asp:Label ID="lblSurveyIDEdit" runat="server"></asp:Label></h6>-->

            <div class="divFullWidth">
                Edit date series
             </div>
            <div class="divChoose">
                
                Series Date:&nbsp;<telerik:RadDatePicker ID="RadDtPckEditDateSeries" 
                    Runat="server" Culture="English (United States)"></telerik:RadDatePicker><asp:RequiredFieldValidator ID="RequiredDateSeriesEdit" runat="server" ErrorMessage="Valid Date Required" ControlToValidate="RadDtPckEditDateSeries" Text="A Valid Date is Required"></asp:RequiredFieldValidator>
                               
                <br/><br/>
                <asp:Button ID="btnSaveEditChanges" runat="server" Text="Save Changes" 
                    Style="position:relative;left:10px" />
                <asp:Button ID="btnEditCancel" runat="server" Text="Cancel"  
                    Style="position:relative;left:20px" CausesValidation="false"/> 
                              
            </div>
        </asp:View>
        
        <asp:View ID="vwDateSeriesAdd" runat="server">
        
            <h6>Survey ID:<asp:Label ID="lblSurveyIDAdd" runat="server"></asp:Label></h6>

            <div class="divFullWidth">
                Add date series
             </div>
            <div class="divChoose">
                
                Series Date:&nbsp;<telerik:RadDatePicker ID="RadDtPckAddDateSeries" 
                    Runat="server" Culture="English (United States)"></telerik:RadDatePicker><asp:RequiredFieldValidator ID="RequiredDateSeriesAdd" runat="server" ErrorMessage="Valid Date Required" ControlToValidate="RadDtPckAddDateSeries" Text="A Valid Date is Required"></asp:RequiredFieldValidator>
                               
                <br/><br/>
                <asp:Button ID="btnSaveAdd" runat="server" Text="Save Changes" 
                    Style="position:relative;left:10px" />
                <asp:Button ID="btnCancelAdd" runat="server" Text="Cancel"  Style="position:relative;left:20px" CausesValidation="false"/> 
                              
            </div>
        </asp:View>
        
       <asp:View ID="vwNotAuthorized" runat="server">
            <uc1:NotAuthorized ID="NotAuthorized1" runat="server" />
        </asp:View>

    </asp:MultiView>
    
    <asp:Panel ID="pnlMessageEdit" runat="server" Visible = "false">
   
        <div style="padding-top:25px" visible="false">
            Last Updated SurveyID : <asp:Label ID="lblLastEditSurvey" runat="server" Enabled="False"></asp:Label><br />         
            Last Updated Date : <asp:Label ID="lblLastEditDate" runat="server" Text=""  enabled="false"></asp:Label>
        </div>
    
     </asp:Panel>
     
     <asp:Panel ID="pnlMessageAdd" runat="server" Visible = "false">
   
        <div style="padding-top:25px" visible="false">
            Last Updated SurveyID : <asp:Label ID="lblLastAddSurvey" runat="server" Enabled="False"></asp:Label><br />         
            Last Updated Date : <asp:Label ID="lblLastAddDate" runat="server" Text=""  enabled="false"></asp:Label>
        </div>
    
     </asp:Panel>
     
