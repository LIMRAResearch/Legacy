<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Masters/Admin.Master"
    CodeBehind="Usermnt.aspx.vb" Inherits="SalesSurveysApplication.Usermnt" %>

<%@ Register Src="../usercontrols/ErrorMessage.ascx" TagName="ErrorMessage" TagPrefix="uc1" %>
<%@ Register Src="../usercontrols/NotAuthorized.ascx" TagName="NotAuthorized" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
   
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
        ShowSummary="False" />
    <h1>
        User Maintenance</h1>
    <asp:MultiView ID="MultiView1" runat="server">
        <asp:View ID="vwDefault" runat="server">
            <div class="divFullWidth">
                Please choose a user to edit, or click on Add New.</div>
            <div class="divChoose">
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Required for Edit"
                    ControlToValidate="ddlUsers" Display="Dynamic" InitialValue="NA">*</asp:RequiredFieldValidator>
                &nbsp;Choose a User:&nbsp;<asp:DropDownList ID="ddlUsers" runat="server" AppendDataBoundItems="True">
                </asp:DropDownList>
                &nbsp;<asp:Button ID="btnGoToEdit" runat="server" Text="Edit User" />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Button ID="btnGoToAdd" runat="server" Text="Add New User" CausesValidation="False" />
            </div>
        </asp:View>
        <asp:View ID="vwEdit" runat="server">
            <div class="divContainer">
                <div class="divLeft">
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                        ControlToValidate="txtFirstName" Display="Dynamic" 
                        ErrorMessage="First Name Required">*</asp:RequiredFieldValidator>
                    &nbsp;First Name:</div>
                <div class="divRight">
                    <asp:TextBox ID="txtFirstName" runat="server" Width="200px"></asp:TextBox>
                </div>
            </div>
             <div class="divContainer">
                <div class="divLeft">
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" 
                        ControlToValidate="txtLastName" Display="Dynamic" 
                        ErrorMessage="Last Name Required">*</asp:RequiredFieldValidator>
                    &nbsp;Last Name:</div>
                <div class="divRight">
                    <asp:TextBox ID="txtLastName" runat="server" Width="200px"></asp:TextBox>
                </div>
            </div>            
             <div class="divContainer">
                <div class="divLeft">
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" 
                        ControlToValidate="txtEmail" Display="Dynamic" ErrorMessage="Email Required">*</asp:RequiredFieldValidator>
                    &nbsp;Email:
                </div>
                <div class="divRight">
                    <asp:TextBox ID="txtEmail" runat="server" Width="300px"></asp:TextBox>
                </div>
            </div>
             <div class="divContainer">
                <div class="divLeft">
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" 
                        ControlToValidate="txtOnyxID" Display="Dynamic" ErrorMessage="Onyx ID Required">*</asp:RequiredFieldValidator>
                    &nbsp;Onyx ID:</div>
                <div class="divRight">
                    <asp:TextBox ID="txtOnyxID" runat="server" Width="100px"></asp:TextBox>
                    &nbsp;<asp:CompareValidator ID="CompareValidator1" runat="server" 
                        ControlToValidate="txtOnyxID" Display="Dynamic" ErrorMessage="Integers Only" 
                        Operator="DataTypeCheck" Type="Integer">Integers Only</asp:CompareValidator>
                </div>
            </div>
             <div class="divContainer">
                <div class="divLeft">
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" 
                        ControlToValidate="txtLoginID" Display="Dynamic" 
                        ErrorMessage="Login ID Required">*</asp:RequiredFieldValidator>
                    &nbsp;Login ID:
                </div>
                <div class="divRight">
                    <asp:TextBox ID="txtLoginID" runat="server" Width="200px"></asp:TextBox>
                </div>
            </div>
             <div class="divContainer">
                <div class="divLeft">
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" 
                        ControlToValidate="txtPassword" Display="Dynamic" 
                        ErrorMessage="Password Required">*</asp:RequiredFieldValidator>
                    &nbsp;Password:
                </div>
                <div class="divRight">
                    <asp:TextBox ID="txtPassword" runat="server" Width="200px"></asp:TextBox>
                </div>
            </div>
             <div class="divContainer">
                <div class="divLeft">
                    Groups:</div>
                <div class="divRight">
                    <asp:CheckBoxList ID="cblGroup" runat="server" RepeatLayout="Flow">
                    </asp:CheckBoxList>
                </div>
            </div>
             <div class="divContainer">
                <div class="divLeft">
                    Surveys:</div>
                <div class="divRight">
                    <asp:CheckBoxList ID="cblSurvey" runat="server" RepeatLayout="Flow">
                    </asp:CheckBoxList>
                </div>
            </div>
             <div class="divContainer">
                <div class="divLeft">
                </div>
                <div class="divRight">
                    <asp:Button ID="btnAdd" runat="server" Text="Add New User" />
                    &nbsp;
                    <asp:Button ID="btnSave" runat="server" Text="Save Changes" />
                    &nbsp;
                    <asp:Button ID="btnCancel" runat="server" CausesValidation="False" 
                        Text="Cancel" />
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:Button ID="btnDelete" runat="server" CausesValidation="False" 
                        Text="Delete" />
                </div>
            </div>
        </asp:View>
        <asp:View ID="vwNotAuthorized" runat="server">
            <uc2:NotAuthorized ID="NotAuthorized1" runat="server" />
        </asp:View>
        <asp:View ID="vwError" runat="server">
            <uc1:ErrorMessage ID="ErrorMessage1" runat="server" />
        </asp:View>
    </asp:MultiView>
</asp:Content>
