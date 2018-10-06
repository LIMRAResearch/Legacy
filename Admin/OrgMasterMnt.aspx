<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Masters/Admin.Master"
    CodeBehind="OrgMasterMnt.aspx.vb" Inherits="SalesSurveysApplication.OrgMasterMnt" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="../usercontrols/NotAuthorized.ascx" TagName="NotAuthorized" TagPrefix="uc1" %>
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
    <asp:ValidationSummary ID="ValidationSummary2" runat="server" ShowMessageBox="True"
        ShowSummary="False" ValidationGroup="ByCRM" />
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
        ShowSummary="False" />
    <h1>
        Organization Master Maintenance</h1>
    <asp:MultiView ID="MultiView1" runat="server">
        <asp:View ID="vwDefault" runat="server">
            <div class="divFullWidth">
                <asp:Label ID="Label1" runat="server" Text="Listed below are the Organizations currently in the dOrgMaster table. Select one
                to edit, or select Add New to add a new Organization from our current CRM system."></asp:Label>
                </div>
            <div class="divContainer">
                <div class="divLeft">
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" ControlToValidate="ddlOrgMaster"
                        Display="Dynamic" ErrorMessage="Required for Edit" InitialValue="NA">*</asp:RequiredFieldValidator>
                    &nbsp;Select an Organization:&nbsp;
                </div>
                <div class="divRight">
                    <asp:DropDownList ID="ddlOrgMaster" runat="server" AppendDataBoundItems="True" 
                        Width="400px" AutoPostBack="True">
                    </asp:DropDownList>
                </div>
            </div>
            <div class="divClear" />
            <div class="divContainer">
                <div class="divLeft">
                </div>
                
                    <div class="divRight">
                     <asp:Button ID="btnGoToAdd" runat="server" Text="Add New Organization" ValidationGroup="Add" />
                    <asp:Button ID="btnGoToEdit" runat="server" Text="Edit Organization" />&nbsp;
                        <asp:Button ID="btnDelete2" runat="server" CausesValidation="False" Enabled="False" Text="Delete" />
                </div>
            </div>
            <div class="divClear" />
        </asp:View>
        <asp:View ID="vwEdit" runat="server">
            <div class="divContainer">
                <asp:Literal ID="litAddEditMsg" runat="server"></asp:Literal>
                <asp:Literal ID="litOrgError" runat="server"></asp:Literal>
            </div>
            <div class="divContainer">
                <div class="divLeft">
                    Organization Name:</div>
                <div class="divRight">
                    <asp:Literal ID="litOrganizationName" runat="server"></asp:Literal>
                    <asp:TextBox ID="InactiveOrgTB" runat="server" Visible = "false"></asp:TextBox>
                </div>
            </div>
            <div class="divClear" />
            <div class="divContainer">
                <div class="divLeft">
                    &nbsp;<asp:Label ID="OrgMasterIDLabel" runat="server" Text="dOrgMaster ID:"></asp:Label></div>
                <div class="divRight">
                    <asp:Literal ID="litdOrgMasterID" runat="server"></asp:Literal>
                </div>
            </div>
            <div class="divClear" />
            <div class="divContainer">
                <div class="divLeft">
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" ControlToValidate="RadComboBoxOrgs"
                        Display="Dynamic" ErrorMessage="Organization Required">*</asp:RequiredFieldValidator>
                    <asp:Label ID="OrganizationLabel" runat="server" Text="Organization:"></asp:Label>
                </div>
                <div class="divRight">
                    <telerik:RadComboBox ID="RadComboBoxOrgs" runat="server" AllowCustomText="false"
                        DropDownWidth="725px" EmptyMessage="Start typing Organization Name" EnableLoadOnDemand="True"
                        Height="200px" MarkFirstMatch="true" OnClientBlur="OnClientBlurHandler" OnClientItemsRequesting="OnClientItemsRequesting"
                        OnItemDataBound="RadComboBoxOrgs_ItemDataBound" OnItemsRequested="RadComboBoxOrgs_ItemsRequested"
                        Width="300px">
                        <HeaderTemplate>
                            <table style="width: 700px; text-align: left">
                                <tr>
                                    <td align="center" style="width: 75px;" valign="top">
                                        Company ID
                                    </td>
                                    <td align="left" style="width: 200px;" valign="top">
                                        Company Name
                                    </td>
                                    <td align="left" style="width: 250px;" valign="top">
                                        Location
                                    </td>
                                    <td align="left" style="width: 200px;" valign="top">
                                        Type
                                    </td>
                                </tr>
                            </table>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <table style="width: 700px; text-align: left">
                                <tr>
                                    <td align="center" style="width: 75px;" valign="top">
                                        <%#DataBinder.Eval(Container.DataItem, "ID")%>
                                    </td>
                                    <td align="left" style="width: 200px;" valign="top">
                                        <%#DataBinder.Eval(Container.DataItem, "Name")%>
                                    </td>
                                    <td align="left" style="width: 250px;" valign="top">
                                        <%#DataBinder.Eval(Container.DataItem, "Address")%><br />
                                        <%#DataBinder.Eval(Container.DataItem, "City")%>,
                                        <%#DataBinder.Eval(Container.DataItem, "Region") %>
                                        <%#DataBinder.Eval(Container.DataItem, "Country")%>
                                    </td>
                                    <td align="left" style="width: 250px;" valign="top">
                                        <%#DataBinder.Eval(Container.DataItem, "Type")%>
                                    </td>
                                </tr>
                            </table>
                        </ItemTemplate>
                    </telerik:RadComboBox>
                </div>
            </div>
            <div class="divClear" />
            <div class="divContainer">
                <div class="divLeft">
                </div>
                <div class="divRight">
                    <asp:Button ID="btnAddNew" runat="server" Text="Add New" />
                    &nbsp;
                    <asp:Button ID="btnSave" runat="server" Text="Save Changes" />
                    &nbsp;
                    <asp:Button ID="CbtnCancel" runat="server" CausesValidation="False" Text="Cancel" />
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    
                    <asp:CheckBox ID="InactiveCheckBox" runat="server" AutoPostBack="True" 
                        Text="Inactive Organization" Visible="false" />
                </div>
            </div>
            <div class="divClear" />
            <div class="divChoose"style="height: 50px;">
            &nbsp;
            </div>
              
            <div class="divContainer">
                <asp:Label ID="AddByCRMLabel" runat="server" Text="You may also add an Organization by entering its' CRM ID in the box below, then
                select Add by CRM ID."></asp:Label>
                
            </div>
            <div class="divContainer">
                <div class="divLeft">
                </div>
                <div class="divRight">
                </div>
            </div>
            <div class="divClear" />
            <div class="divContainer">
                <div title="CRMText" class="divLeft">
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server" ControlToValidate="RadComboBoxOrgID"
                        Display="Dynamic" ErrorMessage="CRM ID Required" ValidationGroup="ByCRM">*</asp:RequiredFieldValidator>
                    <asp:Label ID="OrgCRMIDLabel" runat="server" Text="Organization's CRM ID:"></asp:Label>
                </div>
                <div class="divRight">
                    <telerik:RadComboBox ID="RadComboBoxOrgID" runat="server" AllowCustomText="false"
                        DropDownWidth="725px" EmptyMessage="Start typing Organization ID" EnableLoadOnDemand="True"
                        Height="200px" MarkFirstMatch="true" OnClientBlur="OnClientBlurHandler" OnClientItemsRequesting="OnClientItemsRequesting"
                        OnItemDataBound="RadComboBoxOrgs_ItemDataBound" OnItemsRequested="RadComboBoxOrgID_ItemsRequested"
                        Width="300px" ValidationGroup="ByCRM">
                        <HeaderTemplate>
                            <table style="width: 700px; text-align: left">
                                <tr>
                                    <td align="center" style="width: 75px;" valign="top">
                                        Company ID
                                    </td>
                                    <td align="left" style="width: 200px;" valign="top">
                                        Company Name
                                    </td>
                                    <td align="left" style="width: 250px;" valign="top">
                                        Location
                                    </td>
                                    <td align="left" style="width: 200px;" valign="top">
                                        Type
                                    </td>
                                </tr>
                            </table>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <table style="width: 700px; text-align: left">
                                <tr>
                                    <td align="center" style="width: 75px;" valign="top">
                                        <%#DataBinder.Eval(Container.DataItem, "ID")%>
                                    </td>
                                    <td align="left" style="width: 200px;" valign="top">
                                        <%#DataBinder.Eval(Container.DataItem, "Name")%>
                                    </td>
                                    <td align="left" style="width: 250px;" valign="top">
                                        <%#DataBinder.Eval(Container.DataItem, "Address")%><br />
                                        <%#DataBinder.Eval(Container.DataItem, "City")%>,
                                        <%#DataBinder.Eval(Container.DataItem, "Region") %>
                                        <%#DataBinder.Eval(Container.DataItem, "Country")%>
                                    </td>
                                    <td align="left" style="width: 250px;" valign="top">
                                        <%#DataBinder.Eval(Container.DataItem, "Type")%>
                                    </td>
                                </tr>
                            </table>
                        </ItemTemplate>
                    </telerik:RadComboBox>
                </div>
            </div>
            <div class="divClear" />
             <div class="divContainer">
                <div class="divLeft">
                </div>
                <div class="divRight">
                <asp:Button ID="btnAddByCRMID" runat="server" Text="Add By CRM ID" ValidationGroup="ByCRM" />
                </div>
            </div>
            <div class="divClear" />
        </asp:View>
        
        <asp:View ID="vwNotAuthorized" runat="server">
            <uc1:NotAuthorized ID="NotAuthorized1" runat="server" />
        </asp:View>
    </asp:MultiView>
</asp:Content>
