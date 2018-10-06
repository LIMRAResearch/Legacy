Public Partial Class SurveyGroupsMnt
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
        LoadGroups()
    End Sub

    'populates drop down list with existing groups based on survey selected
    Private Sub LoadGroups()
        Using db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext
            Dim query = From g In db.dGroups _
                        Where g.SurveyID = CType(ddlSurveys.SelectedValue, Integer) _
                        Order By g.PrefGroupName _
                        Select g.GroupID, g.PrefGroupName

            ddlSurveyGroups.Items.Clear()
            ddlSurveyGroups.AppendDataBoundItems = True
            ddlSurveyGroups.Items.Add(New ListItem("Please Choose", "NA"))
            ddlSurveyGroups.DataValueField = "GroupID"
            ddlSurveyGroups.DataTextField = "PrefGroupName"
            ddlSurveyGroups.DataSource = query
            ddlSurveyGroups.DataBind()

            litSurveyName.Text = ddlSurveys.SelectedItem.Text
            litSurveyName2.Text = ddlSurveys.SelectedItem.Text

            MultiView1.SetActiveView(vwChoose)


        End Using
    End Sub


    Private Sub ClearForm()
        txtGroupPreferredName.Text = String.Empty
    End Sub

    'displays form to add new Group
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


    'dislays selected Survey Group for edit
    Protected Sub btnEdit_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnEdit.Click
        If Page.IsValid Then
            Using db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext
                Dim query = From grp In db.dGroups _
                            Where grp.GroupID = CType(ddlSurveyGroups.SelectedValue, Integer) _
                            Select grp
                For Each item In query
                    txtGroupPreferredName.Text = item.PrefGroupName.ToString
                Next
            End Using

            btnAdd.Visible = False
            btnSave.Visible = True
            btnDelete.Visible = CanDelete(CType(ddlSurveyGroups.SelectedItem.Value, Integer))

            MultiView1.SetActiveView(vwEdit)
        End If
    End Sub

    'Checks for existing survey data using selected group
    Protected Function CanDelete(ByVal GroupID As Integer) As Boolean
        Using db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext

            Dim query = From s In db.dSources _
                        Where s.GroupID = GroupID Or s.ParentID = GroupID _
                        Select s.SourceID
            Return query.Count = 0

        End Using
    End Function

    'Adds a new Survey Group
    Protected Sub btnAdd_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAdd.Click

        If Page.IsValid Then
            Using db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext

                Dim dGroup As New dGroup With {.PrefGroupName = txtGroupPreferredName.Text.ToString, _
                                               .SurveyID = CType(ddlSurveys.SelectedItem.Value, Integer)}

                db.dGroups.InsertOnSubmit(dGroup)
                db.SubmitChanges()

                LoadGroups()

            End Using
        End If
    End Sub

    'updates selected Survey Group
    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSave.Click
        If Page.IsValid Then
            Using db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext

                Dim dGroup = (From g In db.dGroups _
                              Select g _
                              Where g.GroupID = CType(ddlSurveyGroups.SelectedItem.Value, Integer)).Single

                dGroup.PrefGroupName = txtGroupPreferredName.Text.ToString
                db.SubmitChanges()

                LoadGroups()

            End Using
        End If
    End Sub

    'Sends user to previous screen - select a survey group to edit, or add new
    Protected Sub btnCancel2_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCancel2.Click
        ddlSurveyGroups.ClearSelection()
        MultiView1.SetActiveView(vwChoose)
    End Sub

    'deletes selected survey group
    Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnDelete.Click

        If CanDelete(CType(ddlSurveyGroups.SelectedItem.Value, Integer)) Then
            Using db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext

                Dim dGroup = From g In db.dGroups _
                             Where g.GroupID = CType(ddlSurveyGroups.SelectedItem.Value, Integer)
                db.dGroups.DeleteAllOnSubmit(dGroup)
                db.SubmitChanges()

            End Using
        End If

        LoadGroups()

    End Sub
End Class