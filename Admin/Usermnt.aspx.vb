Partial Public Class Usermnt
    Inherits System.Web.UI.Page

    Protected gl As Globals.GlobalFunctions = New Globals.GlobalFunctions

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Not User.IsInRole("SuperAdmin") Then
            MultiView1.SetActiveView(vwNotAuthorized)
        Else
            If Not Page.IsPostBack Then
                LoadUsers()
                LoadRoles()
                LoadSurveys()
            End If
            btnDelete.Attributes.Add("onclick", "return confirm('Are you sure you want to delete?');")
        End If

    End Sub

    'Populates the drop down list with existing users
    Private Sub LoadUsers()

        'MB (3/25/2011) - remove Try/Catch block and use global error handlng
        'Try

        Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext(gl.GetDBConnection.ToString)

        Dim query = From p In db.admUsers _
                    Order By p.LastName _
                    Select p.OnyxID, UserName = String.Concat(p.LastName, ", ", p.FirstName)

        'Sets the properties for the user drop down list
        'Adds the first item, maps the value and text properties to items in the query
        'Sets the datasource of the drop down list to the query results, displays the appropriate view
        ddlUsers.Items.Clear()
        ddlUsers.AppendDataBoundItems = True
        ddlUsers.Items.Add(New ListItem("Please Choose", "NA"))
        ddlUsers.DataTextField = "UserName"
        ddlUsers.DataValueField = "OnyxID"
        ddlUsers.DataSource = query
        ddlUsers.DataBind()
        MultiView1.SetActiveView(vwDefault)

        'Catch ex As Exception
        'gl.NotifyWebmaster(ex, gl.GetCurrentUrl, "Load users into drop down list error")
        'MultiView1.SetActiveView(vwError)
        'End Try

    End Sub

    'Populates the check box list with Roles
    Private Sub LoadRoles()

        'MB (3/25/2011) - remove Try/Catch block and use global error handlng
        'Try

        Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext(gl.GetDBConnection.ToString)

        Dim query = From p In db.admGroups _
                    Order By p.GroupName _
                    Select p.GroupID, p.GroupName

        cblGroup.DataTextField = "GroupName"
        cblGroup.DataValueField = "GroupID"
        cblGroup.DataSource = query
        cblGroup.DataBind()

        'Catch ex As Exception
        'gl.NotifyWebmaster(ex, gl.GetCurrentUrl, "Load Roles User Maintenance Error")
        'MultiView1.SetActiveView(vwError)
        'End Try

    End Sub
    'Populates the check box list with Surveys
    Private Sub LoadSurveys()

        'MB (3/25/2011) - remove Try/Catch block and use global error handlng
        'Try

        Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext(gl.GetDBConnection.ToString)

        Dim query = From p In db.dSurveys _
                    Order By p.SurveyName _
                    Select p.SurveyID, p.SurveyName

        cblSurvey.DataTextField = "SurveyName"
        cblSurvey.DataValueField = "SurveyID"
        cblSurvey.DataSource = query
        cblSurvey.DataBind()

        'Catch ex As Exception
        'gl.NotifyWebmaster(ex, gl.GetCurrentUrl, "Load Surveys User Maintenance Error")
        'MultiView1.SetActiveView(vwError)
        'End Try

    End Sub

    'Creates a check box list from the admGroups table, creates a check box list
    'from the dSurveys table, and loads the selected user for edit
    'hides and displays applicable buttons
    Protected Sub btnGoToEdit_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnGoToEdit.Click

        LoadUserForEdit(CType(ddlUsers.SelectedValue, Integer))
        btnAdd.Visible = False
        btnSave.Visible = True
        btnDelete.Visible = True

    End Sub

    'queries the admUsers table by ONYX ID, loads the edit form with existing information

    Private Sub LoadUserForEdit(ByVal OnyxID As Integer)

        'MB (3/25/2011) - remove Try/Catch block and use global error handlng
        'Try

        'Creates a new Security object used to decrypt the password
        Dim thisSec As Security = New Security
        Dim thisItem As ListItem = New ListItem
        Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext(gl.GetDBConnection.ToString)

        ClearForm(vwEdit)

        Dim query = From p In db.admUsers _
                    Where p.OnyxID = OnyxID _
                    Select p
        If query.Count > 0 Then
            ViewState("OnyxID") = OnyxID
            For Each item In query
                If Not item.Email Is Nothing Then
                    txtEmail.Text = item.Email
                End If
                If Not item.FirstName Is Nothing Then
                    txtFirstName.Text = item.FirstName.ToString
                End If
                If Not item.LastName Is Nothing Then
                    txtLastName.Text = item.LastName.ToString
                End If
                If Not item.LoginID Is Nothing Then
                    txtLoginID.Text = item.LoginID.ToString
                End If
                txtOnyxID.Text = OnyxID.ToString
                Try
                    txtPassword.Text = thisSec.Decrypt(item.Password.ToString)
                Catch ex As Exception
                    txtPassword.Text = item.Password.ToString
                End Try
            Next

            'Retrieves existing roles by ONYX ID, and pre-selects the roles in the checkboxlist control
            Dim roles = From q In db.admUserGroups _
                        Where q.OnyxID = OnyxID _
                        Select q.GroupID

            For Each r As Integer In roles
                For Each thisItem In cblGroup.Items
                    If thisItem.Value = r.ToString Then
                        thisItem.Selected = True
                    End If
                Next
            Next

            'Retrieves existing surveys by SURVEY ID, and pre-selects the surveys in the checkboxlist control
            Dim surveys = From s In db.admUserSurveys _
                        Where s.OnyxID = OnyxID _
                        Select s.SurveyID

            For Each s As Integer In surveys
                For Each thisItem In cblSurvey.Items
                    If thisItem.Value = s.ToString Then
                        thisItem.Selected = True
                    End If
                Next
            Next
            MultiView1.SetActiveView(vwEdit)
        End If

        'Catch ex As Exception
        'gl.NotifyWebmaster(ex, gl.GetCurrentUrl, "Load User for Edit Error")
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

    'Clears and displays the edit view to add a new user.
    Protected Sub btnGoToAdd_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnGoToAdd.Click

        ClearForm(vwEdit)
        btnAdd.Visible = True
        btnSave.Visible = False
        btnDelete.Visible = False
        MultiView1.SetActiveView(vwEdit)

    End Sub

    'Clears the selected user in the drop down list, and returns the admin to the first screen
    'No changes are made to the database
    Private Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click

        ddlUsers.ClearSelection()
        MultiView1.SetActiveView(vwDefault)

    End Sub

    'Deletes the user from the data base, and also deletes user group associations
    Private Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click

        'MB (3/25/2011) - remove Try/Catch block and use global error handlng
        'Try

        Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext(gl.GetDBConnection.ToString)
        Dim OnyxID As Integer = CType(ViewState("OnyxID"), Integer)

        'selects user group associations from admUserGroups by OnyxID
        Dim groups = From p In db.admUserGroups _
                     Where p.OnyxID = OnyxID

        db.admUserGroups.DeleteAllOnSubmit(groups)
        db.SubmitChanges()

        'selects user survey associations from admUserSurveys by OnyxID
        Dim surveys = From p In db.admUserSurveys _
                      Where p.OnyxID = OnyxID

        db.admUserSurveys.DeleteAllOnSubmit(surveys)
        db.SubmitChanges()

        'selects user from admUsers by OnyxID
        Dim users = From u In db.admUsers _
                    Where u.OnyxID = OnyxID

        'Deletes user from admUsers using the results from the query above
        db.admUsers.DeleteAllOnSubmit(users)
        db.SubmitChanges()
        LoadUsers()

        'Catch ex As Exception
        'gl.NotifyWebmaster(ex, gl.GetCurrentUrl, "Delete User Error")
        'MultiView1.SetActiveView(vwError)
        'End Try
       

    End Sub

    'Adds a new user to admUsers and also Groups to admUserGroups
    Private Sub btnAdd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAdd.Click

        'validates the page server side = protects against javascript being disabled and bypassing client side validation
        If Page.IsValid Then

            'MB (3/25/2011) - remove Try/Catch block and use global error handlng
            'Try

            'Creates a new Security object used to encrypt the password
            Dim thisSec As Security = New Security

            Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext(gl.GetDBConnection.ToString)

            Dim user As New admUser With {.Email = txtEmail.Text.ToString, _
                                           .FirstName = txtFirstName.Text.ToString, _
                                           .LastName = txtLastName.Text.ToString, _
                                           .LoginID = txtLoginID.Text.ToString, _
                                           .OnyxID = CType(txtOnyxID.Text, Integer), _
                                           .Password = thisSec.Encrypt(txtPassword.Text.ToString)}
            db.admUsers.InsertOnSubmit(user)
            db.SubmitChanges()

            Dim thisItem As ListItem = New ListItem
            For Each thisItem In cblGroup.Items
                If thisItem.Selected Then
                    Dim role As New admUserGroup With {.GroupID = CType(thisItem.Value, Integer), .OnyxID = CType(txtOnyxID.Text, Integer)}
                    db.admUserGroups.InsertOnSubmit(role)
                    db.SubmitChanges()
                End If
            Next
            Dim thisItem2 As ListItem = New ListItem
            For Each thisItem2 In cblSurvey.Items
                If thisItem2.Selected Then
                    Dim survey As New admUserSurvey With {.surveyID = CType(thisItem2.Value, Integer), .OnyxID = CType(txtOnyxID.Text, Integer)}
                    db.admUserSurveys.InsertOnSubmit(survey)
                    db.SubmitChanges()
                End If
            Next
            LoadUsers()

            'Catch ex As Exception
            'gl.NotifyWebmaster(ex, gl.GetCurrentUrl, "Add User Error")
            'MultiView1.SetActiveView(vwError)
            'End Try

        End If

    End Sub

    'Updated the admUsers table, deletes existing User-Group associations.
    'Loops through the check box list control of roles, and adds a row to the admUserGroups table when selected.
    Private Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click

        'validates the page server side = protects against javascript being disabled and bypassing client side validation
        If Page.IsValid Then

            'MB (3/25/2011) - remove Try/Catch block and use global error handlng
            'Try

            'Creates a new Security object used to encrypt the password
            Dim thisSec As Security = New Security

            Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext(gl.GetDBConnection.ToString)
            Dim OnyxID As Integer = CType(ViewState("OnyxID"), Integer)

            Dim user As admUser = (From p In db.admUsers _
                       Select p _
                       Where p.OnyxID = OnyxID).Single

            'user.OnyxID = OnyxID
            user.FirstName = txtFirstName.Text
            user.LastName = txtLastName.Text
            user.LoginID = txtLoginID.Text
            user.Password = thisSec.Encrypt(txtPassword.Text.ToString)
            user.Email = txtEmail.Text

            db.SubmitChanges()


            'selects user group associations from admUserGroups by OnyxID
            Dim groups = From p In db.admUserGroups _
                         Where p.OnyxID = OnyxID

            db.admUserGroups.DeleteAllOnSubmit(groups)
            db.SubmitChanges()

            'Loops through the check box list, when it finds one selected, adds a row to admUserGroups
            Dim thisItem As ListItem = New ListItem
            For Each thisItem In cblGroup.Items
                If thisItem.Selected Then
                    Dim role As New admUserGroup With {.GroupID = CType(thisItem.Value, Integer), .OnyxID = CType(txtOnyxID.Text, Integer)}
                    db.admUserGroups.InsertOnSubmit(role)
                    db.SubmitChanges()
                End If
            Next

            ''selects user survey associations from admUserSurveys by OnyxID
            Dim surveys = From p In db.admUserSurveys _
                          Where p.OnyxID = OnyxID

            db.admUserSurveys.DeleteAllOnSubmit(surveys)
            db.SubmitChanges()

            'Loops through the check box list, when it finds one selected, adds a row to admUserSurveys
            Dim thisItem2 As ListItem = New ListItem
            For Each thisItem2 In cblSurvey.Items
                If thisItem2.Selected Then
                    Dim survey As New admUserSurvey With {.surveyID = CType(thisItem2.Value, Integer), .OnyxID = CType(txtOnyxID.Text, Integer)}
                    db.admUserSurveys.InsertOnSubmit(survey)
                    db.SubmitChanges()
                End If
            Next
            LoadUsers()

            'Catch ex As Exception
            'gl.NotifyWebmaster(ex, gl.GetCurrentUrl, "Update User Error")
            'MultiView1.SetActiveView(vwError)
            'End Try

        End If

    End Sub

End Class