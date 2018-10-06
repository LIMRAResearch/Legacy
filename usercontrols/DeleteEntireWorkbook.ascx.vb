Public Class DeleteEntireWorkbook3
    Inherits System.Web.UI.UserControl
    Protected sf As Projects.Survey2Functions = New Projects.Survey2Functions
    Protected gf As Globals.GlobalFunctions = New Globals.GlobalFunctions

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Page.User.IsInRole("SuperAdmin") Or Page.User.IsInRole("Admin") Or _
               Page.User.IsInRole("SuperAnalyst") Or Page.User.IsInRole("Analyst") Then

            litStatus.Text = ""
            If Not Page.IsPostBack Then
                LoadDates()
                SetPageTitle(Session("SurveyName"))
            End If
            btnContinue.Attributes.Add("onclick", "return confirm('Are you sure you want to delete the entire workbook?');")
        Else
            MultiView1.SetActiveView(vwNotAuthorized)
        End If


    End Sub
    'Populates a list of existing survey series (year and quarter) for a given survey
    Protected Sub LoadDates()

        ddlDateSeries.Items.Clear()
        ddlDateSeries.Items.Add(New ListItem("Please Choose", "NA"))

        'MB (3/25/2011) - remove Try/Catch block and use global error handlng
        'Try

        Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext

        Dim query = From p In db.dSurveySeries _
                    Where p.SurveyID = CType(Session("SurveyID"), Integer) _
                    Order By p.DateID Descending _
                    Select p.DateID, p.SurveySeriesID

        'loops through the query, truncates the date to get rid of the time value
        If query.Count > 0 Then
            For Each item In query
                ddlDateSeries.Items.Add(New ListItem(item.DateID.ToShortDateString, item.SurveySeriesID.ToString))
            Next
        Else
            ddlDateSeries.Items.Add(New ListItem("No Dates for this Survey"))
            ddlDateSeries.SelectedIndex = 1
            ddlDateSeries.Enabled = False
            btnContinue.Enabled = False
        End If

        MultiView1.SetActiveView(vwChooseSurveySeries)

        'Catch ex As Exception
        'gf.NotifyWebmaster(ex, gf.GetCurrentUrl, "Load dates into combo box list error")
        'MultiView1.SetActiveView(vwError)
        'End Try

    End Sub
    'Populates a list of existing workbooks for a given survey series
    Private Sub ddlDateSeries_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlDateSeries.SelectedIndexChanged

        Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext

        ddlWorkbooks.Items.Clear()
        ddlWorkbooks.Items.Add(New ListItem("Please Choose", "NA"))

        Dim query = From p In db.dWorkbooks _
                    Where p.SurveySeriesID = CType(ddlDateSeries.SelectedValue, Integer) _
                    Order By p.DateID Descending _
                    Select p.WorkbookID, p.WorkbookName

        If query.Count > 0 Then
            For Each item In query
                ddlWorkbooks.Items.Add(New ListItem(item.WorkbookName.ToString, item.WorkbookID))
            Next
        End If

    End Sub
    Private Sub btnContinue_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnContinue.Click

        Dim sMessage As String = ""

        'MB (3/25/2011) - remove Try/Catch block and use global error handlng
        'Try
        If ddlWorkbooks.SelectedIndex > 0 Then


            Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext()

            Dim query = From details In db.fDatas() _
                        Where details.SurveySeriesID = CType(ddlDateSeries.SelectedValue, Integer) _
                          And details.WorkbookID = CType(ddlWorkbooks.SelectedValue, Integer) _
                        Select details

            If query.Count > 0 Then
                For Each detail As fData In query
                    db.fDatas.DeleteOnSubmit(detail)   'mark workbook rows in fDatas for deletion
                Next

                Try
                    Dim query2 = (From x In db.dWorkbooks() _
                                 Where x.SurveySeriesID = CType(ddlDateSeries.SelectedValue, Integer) _
                                  And x.WorkbookID = CType(ddlWorkbooks.SelectedValue, Integer) _
                                 Select x).First()
                    db.dWorkbooks.DeleteOnSubmit(query2)   'mark workbook row in dWorkbooks for deletion

                    Dim pF As Projects.ProjectFunctions = New Projects.ProjectFunctions
                    Dim sPath As String = pF.getSurveyFilePath(CType(Session("SurveyID"), Integer))

                    sPath += "Data\3-Completed\" & ddlWorkbooks.SelectedItem.Text

                    If System.IO.File.Exists(sPath) Then
                        System.IO.File.Delete(sPath)
                    End If

                Catch ex As Exception
                    sMessage = "Error marking workbook for deletion in dWorkbooks"
                    displayMessageBox(sMessage)
                End Try

                Try
                    db.SubmitChanges()
                    sMessage = "Workbook deleted successfully"
                    displayMessageBox(sMessage)
                Catch ex As Exception
                    sMessage = "Error deleting Workbook"
                    displayMessageBox(sMessage)
                End Try
            Else
                litStatus.Text = "There is no data that matches the search parameters entered. Please try again"
            End If

            'reload survey list
            ddlDateSeries_SelectedIndexChanged(Nothing, Nothing)

        End If 'ddlWorkbooks.SelectedIndex > 0 

    End Sub

    Private Sub SetPageTitle(ByVal TitleText As String)


        Dim litPageTitle As Literal = FindControl("litPageTitle")

        If litPageTitle IsNot Nothing Then
            litPageTitle.Text = TitleText
        End If

    End Sub

    Private Sub displayMessageBox(ByVal msg As String)

        ' define a javascript alertbox containing
        ' the string passed in as argument

        ' create a new label
        Dim lbl As New Label()

        ' add the javascript to fire an alertbox to the label and
        ' add the string argument passed to the subroutine as the
        ' message payload for the alertbox
        lbl.Text = "<script language='javascript'>" & Environment.NewLine & _
                   "window.alert('" + msg + "')</script>"

        ' add the label to the page to display the alertbox
        Page.Controls.Add(lbl)

    End Sub

End Class