Public Partial Class Groupmnt
    Inherits System.Web.UI.Page
    Protected gl As Globals.GlobalFunctions = New Globals.GlobalFunctions

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not User.IsInRole("SuperAdmin") Then
            MultiView1.SetActiveView(vwNotAuthorized)
        Else
            If Not Page.IsPostBack Then
                LoadGroups()
            End If
            btnDelete.Attributes.Add("onclick", "return confirm('Are you sure you want to delete?');")
        End If
    End Sub

    'Populates the drop down list with existing groups
    Private Sub LoadGroups()

        'MB (3/25/2011) - remove Try/Catch block and use global error handlng
        'Try

        Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext(gl.GetDBConnection.ToString)

        Dim query = From p In db.admGroups _
                    Order By p.GroupName _
                    Select p.GroupID, p.GroupName

        'Sets the properties for the group drop down list
        'Adds the first item, maps the value and text properties to items in the query
        'Sets the datasource of the drop down list to the query results, displays the appropriate view
        ddlGroups.Items.Clear()
        ddlGroups.AppendDataBoundItems = True
        ddlGroups.Items.Add(New ListItem("Please Choose", "NA"))
        ddlGroups.DataTextField = "GroupName"
        ddlGroups.DataValueField = "GroupID"
        ddlGroups.DataSource = query
        ddlGroups.DataBind()
        MultiView1.SetActiveView(vwDefault)

        'Catch ex As Exception
        'gl.NotifyWebmaster(ex, gl.GetCurrentUrl, "Load groups into drop down list error")
        'MultiView1.SetActiveView(vwError)
        'End Try

    End Sub

    'Loads the selected groups for edit
    'hides and displays applicable buttons
    Protected Sub btnGoToEdit_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnGoToEdit.Click

        LoadGroupForEdit(CType(ddlGroups.SelectedValue, Integer))
        btnAdd.Visible = False
        btnSave.Visible = True
        btnDelete.Visible = True

    End Sub

    'queries the admGroups table by GROUP ID, loads the edit form with existing information

    Private Sub LoadGroupForEdit(ByVal GroupID As Integer)

        'MB (3/25/2011) - remove Try/Catch block and use global error handlng
        'Try

        Dim thisItem As ListItem = New ListItem
        Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext(gl.GetDBConnection.ToString)

        ClearForm(vwEdit)

        Dim query = From p In db.admGroups _
                    Where p.GroupID = GroupID _
                    Select p

        If query.Count > 0 Then
            ViewState("GroupID") = GroupID
            For Each item In query
                txtGroupName.Text = item.GroupName.ToString
                txtGroupDescription.Text = item.GroupDescription.ToString
            Next
            MultiView1.SetActiveView(vwEdit)
        End If

        'Catch ex As Exception
        'gl.NotifyWebmaster(ex, gl.GetCurrentUrl, "Load Group for Edit Error")
        'MultiView1.SetActiveView(vwError)
        'End Try

    End Sub

    'Takes in a View control, loops through the controls in the View,
    'and resets the value of each control
    Private Sub ClearForm(ByRef thisView As View)

        Dim thisControl As Control = New Control

        For Each thisControl In thisView.Controls

            Select Case thisControl.GetType.FullName.ToString

                Case "System.Web.UI.WebControls.TextBox"
                    Dim thisText As TextBox = New TextBox
                    thisText = CType(thisControl, TextBox)
                    thisText.Text = ""

                Case "System.Web.UI.WebControls.DropDownList"
                    Dim thisDDL As DropDownList = New DropDownList
                    thisDDL = CType(thisControl, DropDownList)
                    thisDDL.ClearSelection()

                Case "System.Web.UI.WebControls.CheckBoxList"
                    Dim thisCBL As CheckBoxList = New CheckBoxList
                    thisCBL = CType(thisControl, CheckBoxList)
                    thisCBL.ClearSelection()
            End Select
        Next

    End Sub

    'Clears and displays the edit view to add a new group.
    Protected Sub btnGoToAdd_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnGoToAdd.Click
        ClearForm(vwEdit)
        btnAdd.Visible = True
        btnSave.Visible = False
        btnDelete.Visible = False
        MultiView1.SetActiveView(vwEdit)
    End Sub

    'Clears the selected group in the drop down list, and returns the admin to the first screen
    'No changes are made to the database
    Private Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        ddlGroups.ClearSelection()
        MultiView1.SetActiveView(vwDefault)
    End Sub

    'Deletes the group from the data base.
    Private Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click

        'MB (3/25/2011) - remove Try/Catch block and use global error handlng
        'Try

        Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext(gl.GetDBConnection.ToString)
        Dim GroupID As Integer = ViewState("GroupID")

        'selects user group associations from admUserGroups by GroupID
        Dim users = From r In db.admUserGroups _
                    Where r.GroupID = GroupID

        'Deletes user group associations from admUserGroups using the results from the query above
        db.admUserGroups.DeleteAllOnSubmit(users)
        db.SubmitChanges()

        'selects group from admGroups by GroupID
        Dim groups = From s In db.admGroups _
                     Where s.GroupID = GroupID

        'Deletes group from admGroups using the results from the query above
        db.admGroups.DeleteAllOnSubmit(groups)
        db.SubmitChanges()
        LoadGroups()

        'Catch ex As Exception
        'gl.NotifyWebmaster(ex, gl.GetCurrentUrl, "Delete Group Error")
        'MultiView1.SetActiveView(vwError)
        'End Try


    End Sub

    'Adds a new group to admGroups
    Private Sub btnAdd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAdd.Click

        'MB (3/25/2011) - remove Try/Catch block and use global error handlng
        'Try

        Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext(gl.GetDBConnection.ToString)

        Dim group As New admGroup With {.Groupname = txtGroupName.Text.ToString, _
                                       .GroupDescription = txtGroupDescription.Text.ToString}
        db.admGroups.InsertOnSubmit(group)
        db.SubmitChanges()
        LoadGroups()

        'Catch ex As Exception
        'gl.NotifyWebmaster(ex, gl.GetCurrentUrl, "Add Group Error")
        'MultiView1.SetActiveView(vwError)
        'End Try

    End Sub

    'Updates the admGroups table.
    Private Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click

        'MB (3/25/2011) - remove Try/Catch block and use global error handlng
        'Try
        Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext(gl.GetDBConnection.ToString)
        Dim GroupID As Integer = ViewState("GroupID")

        Dim group = (From s In db.admGroups _
                   Select s _
                   Where s.GroupID = GroupID).Single

        group.GroupName = txtGroupName.Text
        group.GroupDescription = txtGroupDescription.Text

        db.SubmitChanges()
        LoadGroups()

        'Catch ex As Exception
        'gl.NotifyWebmaster(ex, gl.GetCurrentUrl, "Update Group Error")
        'MultiView1.SetActiveView(vwError)
        'End Try

    End Sub

End Class