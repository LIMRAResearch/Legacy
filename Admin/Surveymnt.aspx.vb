Partial Public Class Surveymnt
    Inherits System.Web.UI.Page
    Protected gl As Globals.GlobalFunctions = New Globals.GlobalFunctions

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Not User.IsInRole("SuperAdmin") Then
            MultiView1.SetActiveView(vwNotAuthorized)
        Else
            If Not Page.IsPostBack Then
                LoadSurveys()
                LoadUsers()
            End If
            btnDelete.Attributes.Add("onclick", "return confirm('Are you sure you want to delete?');")
        End If

    End Sub

    'Populates the drop down list with existing surveys
    Private Sub LoadSurveys()

        'MB (3/25/2011) - remove Try/Catch block and use global error handlng
        'Try

        Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext(gl.GetDBConnection.ToString)


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

        'Catch ex As Exception
        'gl.NotifyWebmaster(ex, gl.GetCurrentUrl, "Load surveys into drop down list error")
        'MultiView1.SetActiveView(vwError)
        'End Try

    End Sub
    'Populates the check box list with Users
    Private Sub LoadUsers()

        'MB (3/25/2011) - remove Try/Catch block and use global error handlng
        'Try

        Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext(gl.GetDBConnection.ToString)

        Dim query = From p In db.admUsers _
                    Order By p.LastName _
                    Select p.OnyxID, UserName = String.Concat(p.LastName, ", ", p.FirstName)

        cblUser.DataTextField = "UserName"
        cblUser.DataValueField = "OnyxID"
        cblUser.DataSource = query
        cblUser.DataBind()

        'Catch ex As Exception
        'gl.NotifyWebmaster(ex, gl.GetCurrentUrl, "Load Users Survey Maintenance Error")
        'MultiView1.SetActiveView(vwError)
        'End Try

    End Sub

   

    'Creates a check box list from the admUser table, loads the selected survey for edit
    'hides and displays applicable buttons
    Protected Sub btnGoToEdit_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnGoToEdit.Click

        LoadSurveyForEdit(CType(ddlSurveys.SelectedValue, Integer))

    End Sub

    'queries the dSurveys table by SURVEY ID, loads the edit form with existing information

    Private Sub LoadSurveyForEdit(ByVal SurveyID As Integer)

        'MB (3/25/2011) - remove Try/Catch block and use global error handlng
        'Try

        Dim thisItem As ListItem = New ListItem
        Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext(gl.GetDBConnection.ToString)
        ClearForm(vwEdit)

        Dim query = From p In db.dSurveys _
                    Where p.SurveyID = SurveyID _
                    Select p

        If query.Count > 0 Then
            ViewState("SurveyID") = SurveyID
            For Each item In query
                txtSurveyName.Text = item.SurveyName.ToString
                ddlFrequency.SelectedValue = item.SurveyFrequency.ToString
                txtSurveyDescription.Text = item.SurveyDescription.ToString
                txtFolderPath.Text = item.FolderPath.ToString
                txtSurveyHomePath.Text = item.SurveyHomePage.ToString
            Next

            'Retrieves existing users by SURVEY ID, and pre-selects
            'the users in the checkboxlist control
            Dim users = From q In db.admUserSurveys _
                        Where q.SurveyID = SurveyID _
                        Select q.OnyxID

            For Each r As Integer In users
                For Each thisItem In cblUser.Items
                    If thisItem.Value = r.ToString Then
                        thisItem.Selected = True
                    End If
                Next
            Next

            btnAdd.Visible = False
            btnSave.Visible = True
            btnDelete.Visible = CanDelete(SurveyID)
            MultiView1.SetActiveView(vwEdit)

        End If

        'Catch ex As Exception
        ' gl.NotifyWebmaster(ex, gl.GetCurrentUrl, "Load Survey for Edit Error")
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

    'Clears and displays the edit view to add a new survey.
    Protected Sub btnGoToAdd_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnGoToAdd.Click

        ClearForm(vwEdit)
        btnAdd.Visible = True
        btnSave.Visible = False
        btnDelete.Visible = False
        MultiView1.SetActiveView(vwEdit)

    End Sub

    'Clears the selected survey in the drop down list, and returns the admin to the first screen
    'No changes are made to the database
    Private Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click

        ddlSurveys.ClearSelection()
        MultiView1.SetActiveView(vwDefault)

    End Sub

    'Deletes the survey from the data base, and also deletes user survey associations
    'Deletes any existing records in tables in the database
    Private Sub btnDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDelete.Click

        Dim SurveyID As Integer = CType(ViewState("SurveyID"), Integer)
        If CanDelete(SurveyID) Then

            Using db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext

                Dim survey = From p In db.dSurveys _
                            Where p.SurveyID = SurveyID
                db.dSurveys.DeleteAllOnSubmit(survey)
                db.SubmitChanges()

            End Using

        End If

    End Sub

    'Adds a new survey to dSurveys and also Users to admUserSurveys
    Private Sub btnAdd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAdd.Click

        'MB (3/25/2011) - remove Try/Catch block and use global error handlng
        'Try

        Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext(gl.GetDBConnection.ToString)

        ' Change by Hetal" Max(surveyid+1)
        Dim surveyno = (From s In db.dSurveys _
                        Select s.SurveyID).Max()

        Dim survey As New dSurvey With {.SurveyID = surveyno + 1, _
                                        .SurveyName = txtSurveyName.Text.ToString, _
                                       .SurveyFrequency = ddlFrequency.SelectedValue.ToString, _
                                       .SurveyDescription = txtSurveyDescription.Text.ToString, _
                                       .FolderPath = txtFolderPath.Text.ToString, _
                                       .SurveyHomePage = txtSurveyHomePath.Text.ToString}
        db.dSurveys.InsertOnSubmit(survey)
        db.SubmitChanges()

        Dim thisitem As ListItem = New ListItem
        For Each thisitem In cblUser.Items
            If thisitem.Selected Then
                Dim user As New admUserSurvey With {.OnyxID = CType(thisitem.Value, Integer), .SurveyID = CType(ddlSurveys.SelectedValue, Integer)}
                db.admUserSurveys.InsertOnSubmit(user)
                db.SubmitChanges()
            End If
        Next

        LoadSurveys()

        'Catch ex As Exception
        'gl.NotifyWebmaster(ex, gl.GetCurrentUrl, "Add Survey Error")
        'MultiView1.SetActiveView(vwError)
        'End Try

    End Sub

    'Updates the dSurveys table, deletes existing User-Survey associations.
    'Loops through the check box list control of users, and adds a row to the admUserSurveys table when selected.
    Private Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click

        'MB (3/25/2011) - remove Try/Catch block and use global error handlng
        'Try

        Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext(gl.GetDBConnection.ToString)
        Dim SurveyID As Integer = CType(ViewState("SurveyID"), Integer)

        Dim survey = (From s In db.dSurveys _
                   Select s _
                   Where s.SurveyID = SurveyID).Single

        ' Change by hetal: to add the surveyid, max(surveyid)+1

        survey.SurveyName = txtSurveyName.Text
        survey.SurveyFrequency = ddlFrequency.SelectedValue.ToString
        survey.SurveyDescription = txtSurveyDescription.Text
        survey.FolderPath = txtFolderPath.Text
        survey.SurveyHomePage = txtSurveyHomePath.Text

        db.SubmitChanges()

        'selects survey user associations from admUserSurveys by SurveyID

        Dim users = From p In db.admUserSurveys _
                    Where p.SurveyID = SurveyID

        db.admUserSurveys.DeleteAllOnSubmit(users)

        'Loops through the check box list, when it finds one selected, adds a row to admUserSurveys
        Dim thisitem As ListItem = New ListItem
        For Each thisitem In cblUser.Items
            If thisitem.Selected Then
                Dim user As New admUserSurvey With {.OnyxID = CType(thisitem.Value, Integer), .SurveyID = CType(ddlSurveys.SelectedValue, Integer)}
                db.admUserSurveys.InsertOnSubmit(user)
            End If
        Next
        db.SubmitChanges()
        LoadSurveys()

        'Catch ex As Exception
        'gl.NotifyWebmaster(ex, gl.GetCurrentUrl, "Update Survey Error")
        'MultiView1.SetActiveView(vwError)
        'End Try

    End Sub


    'checks for existing survey data to see if a survey can be deleted.
    Protected Function CanDelete(ByVal SurveyID As Integer) As Boolean
        Using db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext

            Dim query = From p In db.ddFields _
                        Where p.SurveyID = SurveyID _
                        Select p.FieldID
            If query.Count > 0 Then
                Return False
            End If



            Dim query1 = From q In db.dSurveySeries _
                    Where q.SurveyID = SurveyID _
                    Select q.DateID


            If query1.Count > 0 Then
                Return False
            End If

            Dim query2 = From p In db.fLayouts _
                    Where p.SurveyID = SurveyID _
                    Select p.fColumn

            If query2.Count > 0 Then
                Return False
            End If

            Dim query3 = From p In db.fPages _
                    Where p.SurveyID = SurveyID _
                    Select p.PageNumber

            If query3.Count > 0 Then
                Return False
            End If

            Dim query4 = From p In db.fSections _
                    Where p.SurveyID = SurveyID _
                    Select p.SectionNumber

            If query4.Count > 0 Then
                Return False
            End If

            Dim query5 = From p In db.fRows _
                    Where p.SurveyID = SurveyID _
                    Select p.RowNumber

            If query5.Count > 0 Then
                Return False
            End If

            Dim query6 = From p In db.fColumns _
                    Where p.SurveyID = SurveyID _
                    Select p.ColumnNumber

            If query6.Count > 0 Then
                Return False
            End If

            Dim query7 = From p In db.dSources _
                    Where p.SurveyID = SurveyID _
                    Select p.SourceID

            If query7.Count > 0 Then
                Return False
            End If

            Dim query8 = From p In db.dGroups _
                    Where p.SurveyID = SurveyID _
                    Select p.GroupID

            If query8.Count > 0 Then
                Return False
            End If

            Dim query9 = From p In db.admUserSurveys _
                    Where p.SurveyID = SurveyID _
                    Select p.OnyxID

            If query9.Count > 0 Then
                Return False
            End If

            Return True

        End Using
    End Function

   
End Class