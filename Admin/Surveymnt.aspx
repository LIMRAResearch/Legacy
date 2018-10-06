<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Masters/Admin.Master"
    CodeBehind="Surveymnt.aspx.vb" Inherits="SalesSurveysApplication.Surveymnt" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="../usercontrols/ErrorMessage.ascx" TagName="ErrorMessage" TagPrefix="uc1" %>
<%@ Register Src="../usercontrols/NotAuthorized.ascx" TagName="NotAuthorized" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
        ShowSummary="False" />
    <h1>
        Survey Maintenance</h1>
    <asp:MultiView ID="MultiView1" runat="server">
        <asp:View ID="vwDefault" runat="server">
            <div class="divFullWidth">
                Please choose a survey to edit, or click on Add New.</div>
            <div class="divChoose">
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Required for Edit"
                    ControlToValidate="ddlSurveys" Display="Dynamic" InitialValue="NA">*</asp:RequiredFieldValidator>
                &nbsp;Choose a Survey:&nbsp;<asp:DropDownList ID="ddlSurveys" runat="server" AppendDataBoundItems="True">
                </asp:DropDownList>
                &nbsp;<asp:Button ID="btnGoToEdit" runat="server" Text="Edit Survey" />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Button ID="btnGoToAdd" runat="server" Text="Add New Survey" CausesValidation="False" />
            </div>
        </asp:View>
        <asp:View ID="vwEdit" runat="server">
            <div class="divContainer">
                <div class="divLeft">
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtSurveyName"
                        Display="Dynamic" ErrorMessage="Survey Name Required">*</asp:RequiredFieldValidator>
                    &nbsp;Survey Name:</div>
                <div class="divRight">
                    <asp:TextBox ID="txtSurveyName" runat="server" Width="500px"></asp:TextBox>
                </div>
            </div>
            <div class="divContainer">
                <div class="divLeft">
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="ddlFrequency"
                        Display="Dynamic" ErrorMessage="Survey Frequency Required">*</asp:RequiredFieldValidator>
                    &nbsp;Survey Frequency:</div>
                <div class="divRight">
                    <asp:DropDownList ID="ddlFrequency" runat="server">
                        <asp:ListItem Value="NA">Please Choose</asp:ListItem>
                        <asp:ListItem Value="4">Quarterly</asp:ListItem>
                        <asp:ListItem Value="2">Biannual</asp:ListItem>
                        <asp:ListItem Value="1">Yearly</asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>
            <div class="divContainer">
                <div class="divLeft">
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="txtSurveyDescription"
                        Display="Dynamic" ErrorMessage="Survey Description Required">*</asp:RequiredFieldValidator>
                    &nbsp;Survey Description:
                </div>
                <div class="divRight">
                    <asp:TextBox ID="txtSurveyDescription" runat="server" Width="500px"></asp:TextBox>
                </div>
            </div>
            <div class="divContainer">
                <div class="divLeft">
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="txtFolderPath"
                        Display="Dynamic" ErrorMessage="Folder Path Required">*</asp:RequiredFieldValidator>
                    &nbsp;Folder Path:</div>
                <div class="divRight">
                    <asp:TextBox ID="txtFolderPath" runat="server" Width="500px"></asp:TextBox>
                </div>
            </div>
            <div class="divContainer">
                <div class="divLeft">
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ControlToValidate="txtSurveyHomePath"
                        Display="Dynamic" ErrorMessage="Survey Home Path Required">*</asp:RequiredFieldValidator>
                    &nbsp;Survey Home Page Path:</div>
                <div class="divRight">
                    <asp:TextBox ID="txtSurveyHomePath" runat="server" Width="500px"></asp:TextBox>
                </div>
            </div>
            <div class="msg_head">
                Hide/Display Users</div>
            <div class="msg_body">
                <div class="divContainer">
                    <div class="divLeft">
                        Users:</div>
                    <div class="divRight">
                        <asp:CheckBoxList ID="cblUser" runat="server" RepeatColumns="3" Width="525px">
                        </asp:CheckBoxList>
                    </div>
                </div>
            </div>
            <div class="divContainer">
                <div class="divLeft">
                </div>
                <div class="divRight">
                    <asp:Button ID="btnAdd" runat="server" Text="Add New Survey" />
                    &nbsp;
                    <asp:Button ID="btnSave" runat="server" Text="Save Changes" />
                    &nbsp;
                    <asp:Button ID="btnCancel" runat="server" CausesValidation="False" Text="Cancel" />
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:Button ID="btnDelete" runat="server" CausesValidation="False" Text="Delete" />
                </div>
            </div>

            <script src="../jquery/jquery-1.5.2.min.js" language="javascript" type="text/javascript"></script>

            <script type="text/javascript">
                $(document).ready(function() {
                    //hide the all of the element with class msg_body  
                    $(".msg_body").hide();
                    //toggle the componenet with class msg_body  
                    $(".msg_head").click(function() {
                        $(this).next(".msg_body").slideToggle(50);
                    });
                });
            </script>

        </asp:View>
        <asp:View ID="vwNotAuthorized" runat="server">
            <uc2:NotAuthorized ID="NotAuthorized1" runat="server" />
        </asp:View>
        <asp:View ID="vwError" runat="server">
            <uc1:ErrorMessage ID="ErrorMessage1" runat="server" />
        </asp:View>
    </asp:MultiView>
</asp:Content>
