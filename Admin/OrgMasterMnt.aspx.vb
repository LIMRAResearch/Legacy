

Partial Public Class OrgMasterMnt
    Inherits System.Web.UI.Page




    Protected gl As Globals.GlobalFunctions = New Globals.GlobalFunctions

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not User.IsInRole("SuperAdmin") AndAlso Not User.IsInRole("Admin") Then
            MultiView1.SetActiveView(vwNotAuthorized)
        Else
            btnDelete2.Attributes.Add("language", "javascript")
            btnDelete2.Attributes.Add("OnClick", "return confirm('Are you sure you want to delete the selected organization?');")
            If Not Page.IsPostBack Then
                LoadOrgsFromMaster()
            End If
            litdOrgMasterID.Text = ddlOrgMaster.SelectedValue
            If InactiveCheckBox.Checked Then
                inactiveOrgs()
            Else
                activeOrgs()
            End If
            If ddlOrgMaster.SelectedValue = "NA" Then
                btnDelete2.Enabled = False

            ElseIf litdOrgMasterID.Text <> "" And litdOrgMasterID.Text <> "N/A" And litdOrgMasterID.Text <> "NA" Then
                btnDelete2.Enabled = CanDeleteOrg(CType(litdOrgMasterID.Text, Integer))
            End If
        End If
    End Sub


    'load the choice of organizations from dOrgMaster table
    Protected Sub LoadOrgsFromMaster()

        ddlOrgMaster.Items.Clear()
        ddlOrgMaster.Items.Add(New ListItem("Please Choose", "NA"))

        Using db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext
            Dim Query = From p In db.dOrgMasters _
                          Order By p.LegalName _
                          Select p.OrganizationID, p.LegalName

            For Each item In Query
                ddlOrgMaster.Items.Add(New ListItem(item.LegalName, item.OrganizationID.ToString))
            Next

            MultiView1.SetActiveView(vwDefault)
            InactiveCheckBox.Visible = False
        End Using
    End Sub

    'Sets the text and value properties for the dropdownlist portion of the RadComboBox for each item
    Protected Sub RadComboBoxOrgs_ItemDataBound(ByVal sender As Object, ByVal e As Telerik.Web.UI.RadComboBoxItemEventArgs) Handles RadComboBoxOrgs.ItemDataBound, RadComboBoxOrgID.ItemDataBound
        e.Item.Text = (DirectCast(e.Item.DataItem, DataRowView))("Name").ToString()
        e.Item.Value = (DirectCast(e.Item.DataItem, DataRowView))("ID").ToString()
    End Sub

    'calls a function to populate the radcombobox based on characters entered
    Protected Sub RadComboBoxOrgs_ItemsRequested(ByVal sender As Object, ByVal e As Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs) Handles RadComboBoxOrgs.ItemsRequested
        Dim OrgMasterdt As DataTable = gl.CRMOrgs(e.Text)
        If Not OrgMasterdt Is Nothing Then
            RadComboBoxOrgs.DataSource = OrgMasterdt
            RadComboBoxOrgs.DataBind()
        End If
    End Sub


    Protected Sub RadComboBoxOrgID_ItemsRequested(ByVal sender As Object, ByVal e As Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs) Handles RadComboBoxOrgID.ItemsRequested
        Dim OrgMasterdt As DataTable = gl.CRMOrgByID(e.Text)
        If Not OrgMasterdt Is Nothing Then
            RadComboBoxOrgID.DataSource = OrgMasterdt
            RadComboBoxOrgID.DataBind()
        End If
    End Sub


    Protected Sub btnGoToEdit_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnGoToEdit.Click
        'removed 2/14/2012. Left commented code because Lucian's team suggested they may want to use this functionality in the future. - JC

        If Page.IsValid Then

            litAddEditMsg.Text = "Listed below is the organization you wish to edit. Please use the type-ahead box below to generate a list of organizations." _
            & "The Sales Survey system uses a generated Organization ID which is associated with surveys. Editing the organization listed below with a new organization " _
            & "will change the organization already associated with existing survey data. The Delete button will only be enabled if the organization selected has no " _
            & "survey data."
            litdOrgMasterID.Text = ddlOrgMaster.SelectedValue
            litOrganizationName.Text = ddlOrgMaster.SelectedItem.Text
            litOrgError.Text = String.Empty

            ClearRadComboBox()

            btnAddNew.Visible = False
            btnSave.Visible = True
            btnDelete2.Visible = True
            btnDelete2.Enabled = CanDeleteOrg(CType(litdOrgMasterID.Text, Integer))

            MultiView1.SetActiveView(vwEdit)

        End If
    End Sub


    Protected Sub btnGoToAdd_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnGoToAdd.Click

        litAddEditMsg.Text = "Please select an organization to add to the Sales Survey organization master table from the CRM - a list of organizations will be generated based on " _
         & "characters you type in the textbox below. Additional information will be displayed like company type and location."
        litdOrgMasterID.Text = "N/A"
        litOrganizationName.Text = "N/A"
        litOrgError.Text = String.Empty

        btnAddNew.Visible = True
        btnSave.Visible = False
        'btnDelete.Visible = False -- commented out because we removed the edit screen JC 2/15/2012

        MultiView1.SetActiveView(vwEdit)

        ClearRadComboBox()
        InactiveCheckBox.Visible = True
        InactiveCheckBox.Checked = False
        activeOrgs()

    End Sub


    'Checks if organization can safely be deleted from dOrgMaster table - checks the dSource table
    Protected Function CanDeleteOrg(ByVal OrganizationID As Integer) As Boolean

        Using db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext
            Dim query = From Orgs In db.dSources _
                        Where Orgs.OrganizationID = OrganizationID _
                        Select Orgs.SourceID
            If query.Count > 0 Then
                Return False
            Else
                Return True
            End If

        End Using

    End Function

    'Returns to initial screen, reloads organizations form dOrgMaster
    Protected Sub CbtnCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles CbtnCancel.Click

        InactiveCheckBox.Checked = False
        InactiveOrgTB.Text = ""
        MultiView1.SetActiveView(vwDefault)

    End Sub

    'Adds new dOrgMaster row
    Protected Sub btnAddNew_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAddNew.Click

        If Page.IsValid Then

            Using db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext
                Dim numFound As Integer = 0
                'check if the selected organization is already in the dOrgMaster table
                If RadComboBoxOrgs.SelectedValue = "NA" Then
                    Dim query = From p In db.dOrgMasters _
                                Where p.LegalName = InactiveOrgTB.Text _
                                Select p.OrganizationID

                    numFound = query.Count
                Else

                    Dim query = From p In db.dOrgMasters _
                                Where p.CRMID = CType(RadComboBoxOrgs.SelectedValue, Integer) _
                                Select p.OrganizationID

                    numFound = query.Count
                End If
                If numFound = 0 Then
                    If InactiveCheckBox.Checked Then
                        Dim dOrg As New dOrgMaster With {.LegalName = InactiveOrgTB.Text, _
                                                         .InactiveB = 1}

                        db.dOrgMasters.InsertOnSubmit(dOrg)
                        db.SubmitChanges()

                        LoadOrgsFromMaster()
                        InactiveOrgTB.Text = ""

                    Else

                        Dim dOrg As New dOrgMaster With {.LegalName = RadComboBoxOrgs.Text, _
                                                        .CRMID = CType(RadComboBoxOrgs.SelectedValue, Integer)}

                        db.dOrgMasters.InsertOnSubmit(dOrg)
                        db.SubmitChanges()

                        LoadOrgsFromMaster()


                    End If

                    btnDelete2.Enabled = False

                Else
                    litOrgError.Text = "<span style=""color: red;""><p>The organization you have tried to add - " & RadComboBoxOrgs.Text & " - is already in the Organization Master table.</p></span>"
                    ClearRadComboBox()
                End If

            End Using
        End If

    End Sub

    'Updates dOrgMaster with new organization from CRM
    'no longer used because the edit screen is no longer used - JC 2/15/2012
    'Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSave.Click

    '    If Page.IsValid Then

    '        Using db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext

    '            Dim dOrg = (From orgs In db.dOrgMasters _
    '                        Select orgs _
    '                        Where orgs.OrganizationID = CType(litdOrgMasterID.Text, Integer)).Single

    '            dOrg.CRMID = CType(RadComboBoxOrgs.SelectedValue, Integer)
    '            dOrg.LegalName = RadComboBoxOrgs.Text
    '            db.SubmitChanges()

    '            LoadOrgsFromMaster()

    '        End Using
    '    End If
    'End Sub

    'doubles checks if organization can be deleted, then deletes 
    'removed because the edit page was removed and the delete button was added to the default page -JC 2/14/2012
    'Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnDelete.Click

    '    If CanDeleteOrg(CType(litdOrgMasterID.Text, Integer)) Then

    '        Using db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext

    '            Dim dOrgs = From orgs In db.dOrgMasters _
    '               Where orgs.OrganizationID = CType(litdOrgMasterID.Text, Integer)
    '            db.dOrgMasters.DeleteAllOnSubmit(dOrgs)
    '            db.SubmitChanges()

    '            LoadOrgsFromMaster()

    '        End Using

    '    End If
    'End Sub

    Protected Sub btnAddByCRMID_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAddByCRMID.Click

        Using db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext

            Dim Query = From orgs In db.dOrgMasters _
                            Where orgs.CRMID = RadComboBoxOrgID.SelectedValue _
                            Select orgs

            If Query.Count = 0 Then

                Dim dOrg As New dOrgMaster With {.LegalName = RadComboBoxOrgID.Text, _
                                                .CRMID = CType(RadComboBoxOrgID.SelectedValue, Integer)}

                db.dOrgMasters.InsertOnSubmit(dOrg)
                db.SubmitChanges()

                LoadOrgsFromMaster()
            Else
                litOrgError.Text = "<span style=""color: red;""><p>The organization you have tried to add - " & RadComboBoxOrgID.Text & " - is already in the Organization Master table.</p></span>"
                ClearRadComboBox()
            End If

        End Using
    End Sub

    Private Sub ClearRadComboBox()
        RadComboBoxOrgID.Text = String.Empty
        RadComboBoxOrgID.Items.Clear()

        RadComboBoxOrgs.Text = String.Empty
        RadComboBoxOrgs.Items.Clear()
    End Sub

    'ask user to verify delete then doubles checks if organization can be deleted, then deletes 
    Protected Sub btnDelete2_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnDelete2.Click
        Dim cs As ClientScriptManager = Page.ClientScript
        Dim cstype As Type = Me.[GetType]()

        If CanDeleteOrg(CType(litdOrgMasterID.Text, Integer)) Then

            Using db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext

                Dim dOrgs = From orgs In db.dOrgMasters _
                   Where orgs.OrganizationID = CType(litdOrgMasterID.Text, Integer)
                db.dOrgMasters.DeleteAllOnSubmit(dOrgs)
                db.SubmitChanges()

                LoadOrgsFromMaster()

            End Using

        End If
        btnDelete2.Enabled = False
    End Sub

    Private Sub inactiveOrgs()
        RadComboBoxOrgs.SelectedValue = "NA"
        InactiveOrgTB.Visible = True
        litOrganizationName.Visible = False
        litdOrgMasterID.Visible = False
        RadComboBoxOrgID.Visible = False
        RadComboBoxOrgs.Visible = False
        btnAddByCRMID.Visible = False
        AddByCRMLabel.Visible = False
        OrgCRMIDLabel.Visible = False
        OrgMasterIDLabel.Visible = False
        OrganizationLabel.Visible = False
        RequiredFieldValidator8.Enabled = False
        litAddEditMsg.Text = "Please enter the name of the organization you would like to add and select Add New"
    End Sub

    Private Sub activeOrgs()
        InactiveOrgTB.Visible = False
        litOrganizationName.Visible = True
        litdOrgMasterID.Visible = True
        RadComboBoxOrgID.Visible = True
        RadComboBoxOrgs.Visible = True
        btnAddByCRMID.Visible = True
        AddByCRMLabel.Visible = True
        OrgCRMIDLabel.Visible = True
        OrgMasterIDLabel.Visible = True
        OrganizationLabel.Visible = True
        RequiredFieldValidator8.Enabled = True
        litAddEditMsg.Text = "Please select an organization to add to the Sales Survey organization master table" _
            & "from the CRM - a list of organizations will be generated based on " _
            & "characters you type in the textbox below. Additional information will be displayed like company type and location."
    End Sub

End Class