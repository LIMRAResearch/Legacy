<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Masters/Admin.Master"
    CodeBehind="SurveyGroupsMnt.aspx.vb" Inherits="SalesSurveysApplication.SurveyGroupsMnt" %>
    


<%@ Register src="../usercontrols/NotAuthorized.ascx" tagname="NotAuthorized" tagprefix="uc1" %>

<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
       
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
        ShowSummary="False" />
    <h1>
        Survey Groups Maintenance</h1>
    <asp:MultiView ID="MultiView1" runat="server">
        <asp:View ID="vwDefault" runat="server">
            <div class="divChoose">
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Required for Edit"
                    ControlToValidate="ddlSurveys" Display="Dynamic" InitialValue="NA">*</asp:RequiredFieldValidator>
                &nbsp;Choose a Survey:&nbsp;<asp:DropDownList ID="ddlSurveys" runat="server" AppendDataBoundItems="True">
                </asp:DropDownList>
                &nbsp;<asp:Button ID="btnContinue" runat="server" Text="Continue" />
            </div>
        </asp:View>
        <asp:View ID="vwChoose" runat="server">
            <div class="divFullWidth">
                Please choose a survey group to edit, or click on Add New.</div>
            <div class="divContainer">
                <div class="divLeft">
                    <strong>Survey: </strong>
                </div>
                <div class="divRight">
                    <strong>
                        <asp:Literal ID="litSurveyName" runat="server"></asp:Literal></strong>
                </div>
            </div>
            <div class="divContainer">
                <div class="divLeft">
                    &nbsp;<asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server"
                        ErrorMessage="Required for Edit" ControlToValidate="ddlSurveyGroups" 
                        InitialValue="NA">*</asp:RequiredFieldValidator>
                    Choose Group:
                </div>
                <div class="divRight">
                    <asp:DropDownList ID="ddlSurveyGroups" runat="server" AppendDataBoundItems="True">
                    </asp:DropDownList>
                    &nbsp;
                    <asp:Button ID="btnEdit" runat="server" Text="Edit" />
                    &nbsp;&nbsp;
                    <asp:Button ID="btnGoToAdd" runat="server" Text="Add New" 
                        CausesValidation="False" />
                    &nbsp;
                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" 
                        CausesValidation="False" />
                </div>
            </div>
        </asp:View>
        <asp:View ID="vwEdit" runat="server">
            <div class="divContainer">
                <div class="divLeft">
                    <strong>Survey:</strong>
                </div>
                <div class="divRight">
                    <strong>
                        <asp:Literal ID="litSurveyName2" runat="server"></asp:Literal>
                    </strong>
                </div>
            </div>
            <div class="divContainer">
                <div class="divLeft">
                    &nbsp;<asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server"
                        ErrorMessage="Group Preferred Name Required" 
                        ControlToValidate="txtGroupPreferredName" InitialValue="NA">*</asp:RequiredFieldValidator>
                    Group Preferred Name:</div>
                <div class="divRight">
                    <asp:TextBox ID="txtGroupPreferredName" runat="server" Width="650px"></asp:TextBox>
                </div>
            </div>
            <div class="divContainer">
                <div class="divLeft">
                </div>
                <div class="divRight">
                    <asp:Button ID="btnAdd" runat="server" Text="Add New" />
                    &nbsp;&nbsp;
                    <asp:Button ID="btnSave" runat="server" Text="Save Changes" />
                    &nbsp;&nbsp;
                    <asp:Button ID="btnCancel2" runat="server" CausesValidation="False" Text="Cancel" />
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:Button ID="btnDelete" runat="server" CausesValidation="False" Text="Delete" />
                </div>
            </div>
        </asp:View>
        <asp:View ID="vwNotAuthorized" runat="server">
        
            <uc1:NotAuthorized ID="NotAuthorized1" runat="server" />
        
        </asp:View>
    </asp:MultiView>
</asp:Content>
