

Partial Public Class SurveySourceMnt
    Inherits System.Web.UI.Page

    Protected gf As Globals.GlobalFunctions = New Globals.GlobalFunctions

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not User.IsInRole("SuperAdmin") And Not User.IsInRole("Admin") Then
            MultiView1.SetActiveView(vwNotAuthorized)
        Else
            If Not Page.IsPostBack Then
                MultiView1.SetActiveView(vwDefault)
                LoadSurveys()
            End If
        End If

    End Sub


    'populate dropdown list with existing surveys
    Private Sub LoadSurveys()

        Using db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext()

            Dim query = From p In db.dSurveys _
                        Order By p.SurveyName _
                        Select p.SurveyID, p.SurveyName

            'Sets the properties for the survey drop down list
            'Adds the first item, maps the value and text properties to items in the query
            'Sets the datasource of the drop down list to the query results, displays the appropriate view
            ddlSurveys.Items.Clear()
            ddlSurveys.AppendDataBoundItems = True
            ddlSurveys.Items.Add(New ListItem("Please Choose", "NA"))
            ddlSurveys.DataTextField = "SurveyName"
            ddlSurveys.DataValueField = "SurveyID"
            ddlSurveys.DataSource = query
            ddlSurveys.DataBind()
            MultiView1.SetActiveView(vwDefault)

        End Using

    End Sub


    Protected Sub btnContinue_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnContinue.Click
        If Page.IsValid Then
            LoadSources()
            LoadGroups()
        End If
    End Sub

    'populates drop down list with existing Sources based on survey selected
    Private Sub LoadSources()
        Using db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext
            Dim query = From s In db.dSources _
                        Where s.SurveyID = CType(ddlSurveys.SelectedValue, Integer) _
                        Order By s.SourceName _
                        Select s.SourceID, s.SourceName

            ddlSurveySource.Items.Clear()
            ddlSurveySource.AppendDataBoundItems = True
            ddlSurveySource.Items.Add(New ListItem("Please Choose", "NA"))
            ddlSurveySource.DataValueField = "SourceID"
            ddlSurveySource.DataTextField = "SourceName"
            ddlSurveySource.DataSource = query
            ddlSurveySource.DataBind()

            litSurveyName.Text = ddlSurveys.SelectedItem.Text
            litSurveyName2.Text = ddlSurveys.SelectedItem.Text

            MultiView1.SetActiveView(vwChoose)

        End Using
    End Sub


    Private Sub ClearForm()
        txtSourceName.Text = String.Empty
        txtPreferredName.Text = String.Empty
        ddlGroup.ClearSelection()
        ddlParent.ClearSelection()
        rcbOrganizations.Text = String.Empty
        rcbOrganizations.Items.Clear()
    End Sub

    'displays form to add new Source
    Protected Sub btnGoToAdd_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnGoToAdd.Click
        ClearForm()
        btnAdd.Visible = True
        btnDelete.Visible = False
        btnSave.Visible = False
        MultiView1.SetActiveView(vwEdit)
    End Sub

    'returns user to initial scren
    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCancel.Click
        ddlSurveys.ClearSelection()
        MultiView1.SetActiveView(vwDefault)
    End Sub


    'dislays selected Survey Source for edit
    Protected Sub btnEdit_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnEdit.Click
        If Page.IsValid Then
            ClearForm()

            Using db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext
                Dim query = From src In db.dSources _
                            Join org In db.dOrgMasters _
                            On src.OrganizationID Equals org.OrganizationID _
                            Where src.SourceID = CType(ddlSurveySource.SelectedItem.Value, Integer) _
                            Select src.SourceName, src.PrefName, src.GroupID, src.ParentID, src.OrganizationID, org.LegalName

                For Each item In query
                    txtSourceName.Text = item.SourceName.ToString
                    txtPreferredName.Text = item.PrefName.ToString
                    If Not String.IsNullOrEmpty(item.GroupID.ToString) Then
                        ddlGroup.SelectedValue = item.GroupID.ToString
                    Else
                        ddlGroup.SelectedIndex = 1
                    End If
                    If Not String.IsNullOrEmpty(item.ParentID.ToString) Then
                        ddlParent.SelectedValue = item.ParentID.ToString
                    Else
                        ddlParent.SelectedIndex = 1
                    End If
                    rcbOrganizations.Items.Add(New Telerik.Web.UI.RadComboBoxItem(item.LegalName, item.OrganizationID.ToString))
                    rcbOrganizations.Text = item.LegalName
                    ViewState("oid") = item.OrganizationID.ToString
                Next
            End Using

            btnAdd.Visible = False
            btnSave.Visible = True
            btnDelete.Visible = CanDelete(CType(ddlSurveySource.SelectedItem.Value, Integer))

            MultiView1.SetActiveView(vwEdit)
        End If
    End Sub

    'Checks for existing survey data using selected source
    Protected Function CanDelete(ByVal SourceID As Integer) As Boolean
        Using db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext

            Dim query = From w In db.dWorkbooks _
                        Where w.SourceID = SourceID _
                        Select w.WorkbookID
            Return query.Count = 0

        End Using
    End Function

    'Adds a new Survey Source
    Protected Sub btnAdd_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAdd.Click

        If Page.IsValid Then

            Using db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext

                Dim GroupID, ParentID As Nullable(Of Integer)

                If ddlGroup.SelectedIndex > 1 Then
                    GroupID = CType(ddlGroup.SelectedItem.Value, Integer)
                End If
                If ddlParent.SelectedIndex > 1 Then
                    ParentID = CType(ddlParent.SelectedItem.Value, Integer)
                End If

                Dim src As New dSource With {.SourceName = txtSourceName.Text.ToString, _
                                               .PrefName = txtPreferredName.Text.ToString, _
                                               .GroupID = GroupID, _
                                               .ParentID = ParentID, _
                                               .SurveyID = CType(ddlSurveys.SelectedItem.Value, Integer), _
                                               .OrganizationID = CType(rcbOrganizations.SelectedValue, Integer)}

                db.dSources.InsertOnSubmit(src)
                db.SubmitChanges()

                LoadSources()

            End Using
        End If
    End Sub

    'updates selected Survey Source
    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSave.Click

        If Page.IsValid Then
            Using db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext

                Dim GroupID, ParentID As Nullable(Of Integer)

                If ddlGroup.SelectedIndex > 1 Then
                    GroupID = CType(ddlGroup.SelectedItem.Value, Integer)
                End If
                If ddlParent.SelectedIndex > 1 Then
                    ParentID = CType(ddlParent.SelectedItem.Value, Integer)
                End If

                Dim dSource = (From s In db.dSources _
                              Select s _
                              Where s.SourceID = CType(ddlSurveySource.SelectedItem.Value, Integer)).Single

                dSource.SourceName = txtSourceName.Text.ToString
                dSource.PrefName = txtPreferredName.Text.ToString
                If rcbOrganizations.SelectedIndex < 0 Then
                    dSource.OrganizationID = ViewState("oid")
                Else
                    dSource.OrganizationID = CType(rcbOrganizations.SelectedItem.Value, Integer)
                End If
                dSource.GroupID = GroupID
                dSource.ParentID = ParentID
                db.SubmitChanges()

                LoadSources()

            End Using
        End If
    End Sub

    'Sends user to previous screen - select a survey source to edit, or add new
    Protected Sub btnCancel2_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCancel2.Click
        ddlSurveySource.ClearSelection()
        MultiView1.SetActiveView(vwChoose)
    End Sub

    'deletes selected survey source
    Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnDelete.Click

        If CanDelete(CType(ddlSurveySource.SelectedItem.Value, Integer)) Then
            Using db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext

                Dim dSource = From s In db.dSources _
                             Where s.SourceID = CType(ddlSurveySource.SelectedItem.Value, Integer)
                db.dSources.DeleteAllOnSubmit(dSource)
                db.SubmitChanges()

            End Using
        End If

        LoadSources()

    End Sub

    Protected Sub LoadGroups()
        Using db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext

            Dim query = From g In db.dGroups _
                        Where g.SurveyID = CType(ddlSurveys.SelectedItem.Value, Integer) _
                        Order By g.PrefGroupName _
                        Select g.GroupID, g.PrefGroupName

            ddlGroup.Items.Clear()
            ddlGroup.Items.Add(New ListItem("Please Choose", "NA"))
            ddlGroup.Items.Add(New ListItem("None", "None"))
            ddlGroup.AppendDataBoundItems = True
            ddlGroup.DataValueField = "GroupID"
            ddlGroup.DataTextField = "PrefGroupName"
            ddlGroup.DataSource = query
            ddlGroup.DataBind()

            ddlParent.Items.Clear()
            ddlParent.Items.Add(New ListItem("Please Choose", "NA"))
            ddlParent.Items.Add(New ListItem("None", "None"))
            ddlParent.AppendDataBoundItems = True
            ddlParent.DataValueField = "GroupID"
            ddlParent.DataTextField = "PrefGroupName"
            ddlParent.DataSource = query
            ddlParent.DataBind()

        End Using

    End Sub

    Protected Sub rcbOrganizations_ItemDataBound(ByVal sender As Object, ByVal e As Telerik.Web.UI.RadComboBoxItemEventArgs) Handles rcbOrganizations.ItemDataBound
        ' e.Item.Text = (DirectCast(e.Item.DataItem, DataRowView))("LegalName").ToString()
        ' e.Item.Value = (DirectCast(e.Item.DataItem, DataRowView))("OrganizationID").ToString()
    End Sub

    'calls a function to populate the radcombobox based on characters entered
    Protected Sub rcbOrganizations_ItemsRequested(ByVal sender As Object, ByVal e As Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs) Handles rcbOrganizations.ItemsRequested

        Using db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext

            Dim query = From o In db.dOrgMasters _
                        Where o.LegalName.StartsWith(e.Text) _
                        Order By o.LegalName _
                        Select o.OrganizationID, o.LegalName

            rcbOrganizations.Items.Clear()
            rcbOrganizations.Text = String.Empty
            rcbOrganizations.DataTextField = "LegalName"
            rcbOrganizations.DataValueField = "OrganizationID"

            rcbOrganizations.DataSource = query
            rcbOrganizations.DataBind()

        End Using

    End Sub

End Class