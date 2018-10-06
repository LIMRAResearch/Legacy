<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Masters/Admin.Master" CodeBehind="IDLookups.aspx.vb" Inherits="SalesSurveysApplication.IDLookups" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
     <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
        ShowSummary="False" />
    <div class="Spacer"></div>
    <h1>
        Sales Survey ID Lookups <asp:Literal id="litSurvey" runat="server"></asp:Literal></h1>
    <asp:MultiView ID="MultiView1" runat="server">
        <asp:View ID="vwDefault" runat="server">
            <div class="divFullWidth">
                Please choose a survey to display source or group ids. For organizations, just click the button.</div>
            <div class="divChoose">
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Required for Source IDs and Group IDs"
                    ControlToValidate="ddlSurveys" Display="Dynamic" InitialValue="NA">*</asp:RequiredFieldValidator>
                &nbsp;Choose a Survey:&nbsp;<asp:DropDownList ID="ddlSurveys" runat="server" AppendDataBoundItems="True">
                </asp:DropDownList>
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <asp:Button ID="btnGetSourceIDs" runat="server" Text="Get Source IDs" />
                &nbsp;&nbsp;&nbsp;&nbsp;<asp:Button ID="btnGetGroupIDs" runat="server" Text="Get Group IDs" />
            </div>
            <div class="Spacer"></div>
            <div class="divChoose"> List Organizations:  <asp:Button id="btnGetOrganizations" runat="server" Text="Get Organizations" CausesValidation="false" /></div>
             <div class="Spacer"></div>
        </asp:View>
        <asp:View id="vwSourceIds" runat="server">
            <div class="Spacer"></div>
            <h2>Source ID Lookup</h2>
            <div class="divFullWidth" style="text-align: right; width: 1090px;">
                <asp:Button ID="btnBack" runat="server" Text="Back to Surveys" />
            </div>
             <div class="divFullWidth" style="width: 1100px">
                <telerik:RadGrid id="RadGridSourceIds" runat="server" Skin="Web20" OnNeedDataSource="RadGridSourceIds_NeedDataSource" AllowFilteringByColumn="True" AllowPaging="True" AllowSorting="True" CellSpacing="0" GridLines="None" AutoGenerateColumns="False" Width="1090px">
                     <GroupingSettings CaseSensitive="false"></GroupingSettings>
                    <ClientSettings>
                        <Scrolling AllowScroll="True" UseStaticHeaders="True" />
                    </ClientSettings>
                    <MasterTableView>
                        <CommandItemSettings ExportToPdfText="Export to PDF" />
                        <RowIndicatorColumn FilterControlAltText="Filter RowIndicator column" Visible="True">
                            <HeaderStyle Width="20px" />
                        </RowIndicatorColumn>
                        <ExpandCollapseColumn FilterControlAltText="Filter ExpandColumn column" Visible="True">
                            <HeaderStyle Width="20px" />
                        </ExpandCollapseColumn>
                        <Columns>
                            <telerik:GridBoundColumn DataField="SourceID" FilterControlAltText="Filter column column" FilterControlWidth="50px" HeaderText="Source ID" UniqueName="column">
                                <HeaderStyle HorizontalAlign="Left" VerticalAlign="Top" Width="100px" />
                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Top" Width="100px" />
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="SourceName" FilterControlAltText="Filter column1 column" FilterControlWidth="200px" HeaderText="Source Name" UniqueName="column1">
                                <HeaderStyle HorizontalAlign="Left" VerticalAlign="Top" Width="300px" />
                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Top" Width="300px" />
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="PrefName" FilterControlAltText="Filter column2 column" FilterControlWidth="170px" HeaderText="Preferred Name" UniqueName="column2">
                                <HeaderStyle HorizontalAlign="Left" VerticalAlign="Top" Width="240px" />
                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Top" Width="240px" />
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="OrganizationID" FilterControlAltText="Filter column3 column" FilterControlWidth="85px" HeaderText="Organization ID" UniqueName="column3">
                                <HeaderStyle HorizontalAlign="Left" VerticalAlign="Top" Width="150px" />
                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Top" Width="150px" />
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="SurveyID" FilterControlAltText="Filter column4 column" FilterControlWidth="50px" HeaderText="Survey ID" UniqueName="column4">
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Top" Width="100px" />
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Top" Width="100px" />
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="ParentID" FilterControlAltText="Filter column5 column" FilterControlWidth="50px" HeaderText="Parent ID" UniqueName="column5">
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Top" Width="100px" />
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Top" Width="100px" />
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="GroupID" FilterControlAltText="Filter column6 column" FilterControlWidth="50px" HeaderText="Group ID" UniqueName="column6">
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Top" Width="100px" />
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Top" Width="100px" />
                            </telerik:GridBoundColumn>
                        </Columns>
                        <EditFormSettings>
                            <EditColumn FilterControlAltText="Filter EditCommandColumn column">
                            </EditColumn>
                        </EditFormSettings>
                        <PagerStyle PageSizeControlType="RadComboBox" />
                    </MasterTableView>
                    <PagerStyle PageSizeControlType="RadComboBox" />
                    <FilterMenu EnableImageSprites="False">
                    </FilterMenu>

                </telerik:RadGrid>
             </div>
        </asp:View>
          <asp:View id="vwGroupIds" runat="server">
                <div class="Spacer"></div>
            <h2>Group ID Lookup</h2>
            <div class="divFullWidth" style="text-align: right; width: 500px;">
                <asp:Button  ID="btnBack1" runat="server" Text="Back to Surveys" />
            </div>
             <div class="divFullWidth">
                <telerik:RadGrid id="RadGridGroupIds" runat="server" Skin="Web20" OnNeedDataSource="RadGridGroupIds_NeedDataSource" AllowFilteringByColumn="True" AllowPaging="True" AllowSorting="True" CellSpacing="0" GridLines="None" AutoGenerateColumns="False" Width="500px">
                     <GroupingSettings CaseSensitive="false"></GroupingSettings>
                    <ClientSettings>
                        <Scrolling AllowScroll="True" UseStaticHeaders="True" />
                    </ClientSettings>
                    <MasterTableView>
                        <CommandItemSettings ExportToPdfText="Export to PDF" />
                        <RowIndicatorColumn FilterControlAltText="Filter RowIndicator column" Visible="True">
                            <HeaderStyle Width="20px" />
                        </RowIndicatorColumn>
                        <ExpandCollapseColumn FilterControlAltText="Filter ExpandColumn column" Visible="True">
                            <HeaderStyle Width="20px" />
                        </ExpandCollapseColumn>
                        <Columns>
                            <telerik:GridBoundColumn DataField="GroupID" FilterControlAltText="Filter column column" FilterControlWidth="50px" HeaderText="Group ID" UniqueName="column">
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Top" Width="100px" />
                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Top" Width="100px" />
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="SurveyID" FilterControlAltText="Filter column1 column" FilterControlWidth="50px" HeaderText="Survey ID" UniqueName="column1">
                                <HeaderStyle HorizontalAlign="Left" VerticalAlign="Top" Width="100px" />
                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Top" Width="100px" />
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="PrefGroupName" FilterControlAltText="Filter column2 column" FilterControlWidth="200px" HeaderText="Preferred Group Name" UniqueName="column2">
                                <HeaderStyle HorizontalAlign="Left" VerticalAlign="Top" Width="300px" />
                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Top" Width="300px" />
                            </telerik:GridBoundColumn>
                        </Columns>
                        <EditFormSettings>
                            <EditColumn FilterControlAltText="Filter EditCommandColumn column">
                            </EditColumn>
                        </EditFormSettings>
                        <PagerStyle PageSizeControlType="RadComboBox" />
                    </MasterTableView>
                    <PagerStyle PageSizeControlType="RadComboBox" />
                    <FilterMenu EnableImageSprites="False">
                    </FilterMenu>

                </telerik:RadGrid>
             </div>
        </asp:View>
          <asp:View id="vwOrganizations" runat="server">
                <div class="Spacer"></div>
            <h2>Organization Lookup</h2>
            <div class="divFullWidth" style="text-align: right; width: 690px;">
                <asp:Button ID="btnBack2" runat="server" Text="Back to Surveys" />
            </div>
            <div class="divFullWidth">
                <telerik:RadGrid id="RadGridOrganizations" runat="server" Skin="Web20" OnNeedDataSource="RadGridOrganizations_NeedDataSource" AllowFilteringByColumn="True" AllowPaging="True" AllowSorting="True" CellSpacing="0" GridLines="None" AutoGenerateColumns="False" Width="690px">
                     <GroupingSettings CaseSensitive="false"></GroupingSettings>
                    <ClientSettings>
                        <Scrolling AllowScroll="True" UseStaticHeaders="True" />
                    </ClientSettings>
                    <MasterTableView>
                        <CommandItemSettings ExportToPdfText="Export to PDF" />
                        <RowIndicatorColumn FilterControlAltText="Filter RowIndicator column" Visible="True">
                            <HeaderStyle Width="20px" />
                        </RowIndicatorColumn>
                        <ExpandCollapseColumn FilterControlAltText="Filter ExpandColumn column" Visible="True">
                            <HeaderStyle Width="20px" />
                        </ExpandCollapseColumn>
                        <Columns>
                            <telerik:GridBoundColumn DataField="OrganizationID" FilterControlAltText="Filter column column" FilterControlWidth="85px" HeaderText="Organization ID" UniqueName="column">
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Top" Width="150px" />
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Top" Width="150px" />
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="CRMID" FilterControlAltText="Filter column1 column" FilterControlWidth="75px" HeaderText="Onyx ID" UniqueName="column1">
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Top" Width="140px" />
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Top" Width="140px" />
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="LegalName" FilterControlAltText="Filter column2 column" FilterControlWidth="200px" HeaderText="Legal Name" UniqueName="column2">
                                <HeaderStyle HorizontalAlign="Left" VerticalAlign="Top" Width="300px" Wrap="True" />
                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Top" Width="300px" />
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="InactiveB" FilterControlAltText="Filter column3 column" FilterControlWidth="30px" HeaderText="Inactive" UniqueName="column3">
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Top" Width="75px" />
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Top" Width="75px" />
                            </telerik:GridBoundColumn>
                        </Columns>
                        <EditFormSettings>
                            <EditColumn FilterControlAltText="Filter EditCommandColumn column">
                            </EditColumn>
                        </EditFormSettings>
                        <PagerStyle PageSizeControlType="RadComboBox" />
                    </MasterTableView>
                    <PagerStyle PageSizeControlType="RadComboBox" />
                    <FilterMenu EnableImageSprites="False">
                    </FilterMenu>

                </telerik:RadGrid>
             </div>
        </asp:View>
        <asp:View id="vwError" runat="server">

        </asp:View>
        <asp:View id="vwNotAuthorized" runat="server">

        </asp:View>
        </asp:MultiView>
</asp:Content>
