<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Masters/Admin.Master"
    CodeBehind="SurveySourceMnt.aspx.vb" Inherits="SalesSurveysApplication.SurveySourceMnt" %>

<%@ Register Src="../usercontrols/NotAuthorized.ascx" TagName="NotAuthorized" TagPrefix="uc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    
    <script type="text/javascript">
        function OnClientItemsRequesting(sender, eventArgs) {

            if (sender.get_text().length < 1) {
                eventArgs.set_cancel(true);
            }

            var context = eventArgs.get_context();
            context["FilterString"] = eventArgs.get_text();
        }
    </script>

    <script type="text/javascript">
        //will not allow the user to submit a company name that is not in the list
        function OnClientBlurHandler(sender, eventArgs) {
            var textInTheCombo = sender.get_text();
            var item = sender.findItemByText(textInTheCombo);
            //if there is no item with that text
            if (!item) {
                sender.set_text("");
                setTimeout(function() {
                    var inputElement = sender.get_inputDomElement();
                    inputElement.focus();
                    // inputElement.style.backgroundColor = "red";
                }, 20);
            }
        }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
        ShowSummary="False" />
    <h1>
        Survey Source Maintenance</h1>
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
                Please choose a survey source to edit, or click on Add New.</div>
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
                    &nbsp;<asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="Required for Edit"
                        ControlToValidate="ddlSurveySource" InitialValue="NA">*</asp:RequiredFieldValidator>
                    Choose Source:
                </div>
                <div class="divRight">
                    <asp:DropDownList ID="ddlSurveySource" runat="server" AppendDataBoundItems="True">
                    </asp:DropDownList>
                    &nbsp;
                    <asp:Button ID="btnEdit" runat="server" Text="Edit" />
                    &nbsp;&nbsp;
                    <asp:Button ID="btnGoToAdd" runat="server" Text="Add New" CausesValidation="False" />
                    &nbsp;
                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" CausesValidation="False" />
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
                    &nbsp;<asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="Source Name Required"
                        ControlToValidate="txtSourceName">*</asp:RequiredFieldValidator>
                    Source Name:</div>
                <div class="divRight">
                    <asp:TextBox ID="txtSourceName" runat="server" Width="500px"></asp:TextBox>
                </div>
            </div>
            <div class="divContainer">
                <div class="divLeft">
                    &nbsp;<asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="txtPreferredName"
                        ErrorMessage="Preferred Name Required">*</asp:RequiredFieldValidator>
                    Preferred Name:</div>
                <div class="divRight">
                    <asp:TextBox ID="txtPreferredName" runat="server" Width="500px"></asp:TextBox>
                </div>
            </div>
            <telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server">
                <div class="divContainer">
                    <div class="divLeft">
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="rcbOrganizations"
                            ErrorMessage="Organization Required">*</asp:RequiredFieldValidator>
                        &nbsp;Organization:</div>
                    <div class="divRight">
                        <telerik:RadComboBox ID="rcbOrganizations" runat="server" EmptyMessage="Start typing Organization Name"
                            EnableLoadOnDemand="True" Height="200px" MarkFirstMatch="true" OnClientBlur="OnClientBlurHandler"
                            OnClientItemsRequesting="OnClientItemsRequesting" OnItemDataBound="rcbOrganizations_ItemDataBound"
                            OnItemsRequested="rcbOrganizations_ItemsRequested" Width="300px">
                        </telerik:RadComboBox>
                    </div>
                </div>
            </telerik:RadAjaxPanel>
            <div class="divContainer">
                <div class="divLeft">
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ControlToValidate="ddlGroup"
                        ErrorMessage="Group Required" InitialValue="NA">*</asp:RequiredFieldValidator>
                    &nbsp;Group:</div>
                <div class="divRight">
                    <asp:DropDownList ID="ddlGroup" runat="server">
                    </asp:DropDownList>
                </div>
            </div>
            <div class="divContainer">
                <div class="divLeft">
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" ControlToValidate="ddlParent"
                        ErrorMessage="Parent Required" InitialValue="NA">*</asp:RequiredFieldValidator>
                    &nbsp;Parent:</div>
                <div class="divRight">
                    <asp:DropDownList ID="ddlParent" runat="server">
                    </asp:DropDownList>
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
