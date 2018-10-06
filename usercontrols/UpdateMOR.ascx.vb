Public Partial Class UpdateMOR3
    Inherits System.Web.UI.UserControl

    Protected sf As Projects.Survey2Functions = New Projects.Survey2Functions
    Protected gf As Globals.GlobalFunctions = New Globals.GlobalFunctions

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Page.User.IsInRole("SuperAdmin") Or Page.User.IsInRole("Admin") Or _
             Page.User.IsInRole("SuperAnalyst") Or Page.User.IsInRole("Analyst") Then

            litStatus.Text = ""
            If Not Page.IsPostBack Then
                LoadDates()
                LoadDistributions()
                SetPageTitle(Session("SurveyName"))
            End If
            btnContinue.Attributes.Add("onclick", "return confirm('Are you sure you want to change the MOR?');")
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
            MultiView1.SetActiveView(vwChooseSurveySeries)
        End If

        'Catch ex As Exception
        'gf.NotifyWebmaster(ex, gf.GetCurrentUrl, "Load dates into combo box list error")
        'MultiView1.SetActiveView(vwError)
        'End Try

    End Sub

    'Loads the choice of workbooks based on the Date Series selected
    Private Sub ddlDateSeries_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlDateSeries.SelectedIndexChanged

        Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext

        ddlWorkbooks.Items.Clear()
        ddlWorkbooks.Items.Add(New ListItem("Please Choose", "NA"))

        Dim query = From p In db.dWorkbooks _
                    Where p.SurveySeriesID = CType(ddlDateSeries.SelectedValue, Integer) _
                    Order By p.WorkbookName, p.DateID Descending _
                    Select p.WorkbookID, p.WorkbookName

        If query.Count > 0 Then
            For Each item In query
                ddlWorkbooks.Items.Add(New ListItem(item.WorkbookName.ToString, item.WorkbookID))
            Next
            End If

    End Sub

    'Populates a list of existing distributions for a given survey series and participant
    Protected Sub LoadDistributions()

        'MB (3/25/2011) - remove Try/Catch block and use global error handlng
        'Try

        Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext

        Dim query = From p In db.fPages _
                    Where p.SurveyID = CType(Session("SurveyID"), Integer) _
                    Select p.PageNumber, p.PageDescription

        'Sets the properties for the distributions drop down for a specified survey
        'Adds the first item, maps the value and text properties to an item in the query
        'Sets the datasource of the drop down to the query results, displays the appropriate view
        ddlDistributions.Items.Clear()
        ddlDistributions.AppendDataBoundItems = True
        ddlDistributions.Items.Add(New ListItem("Please Choose", "NA"))
        ddlDistributions.DataTextField = "PageDescription"
        ddlDistributions.DataValueField = "PageNumber"
        ddlDistributions.DataSource = query
        ddlDistributions.DataBind()
        MultiView1.SetActiveView(vwChooseSurveySeries)

        'Catch ex As Exception
        'gf.NotifyWebmaster(ex, gf.GetCurrentUrl, "Load distributions into drop down list error")
        'MultiView1.SetActiveView(vwError)
        'End Try

    End Sub

    Private Sub btnContinue_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnContinue.Click

        Dim sMessage As String = ""

        'MB (3/25/2011) - remove Try/Catch block and use global error handlng
        'Try

        Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext()

        Dim query = From d In db.fDatas _
                    Join l In db.fLayouts On d.FieldID Equals l.FieldID _
                    Where d.SurveySeriesID = CType(ddlDateSeries.SelectedValue, Integer) _
                    And l.PageNumber = CType(ddlDistributions.SelectedValue, Integer) _
                    And d.WorkbookID = CType(ddlWorkbooks.SelectedValue, Integer) _
                    And d.MOR = CType(txtMOR.Text, Integer) _
                    Select d.DataID

        'If the query returns results, we loop through the results
        'for each item(data row) in query, we update the old MOR code to the new MOR code
        Dim holditem As Integer
        If query.Count > 0 Then
            For Each item In query
                holditem = item
                Dim query2 = (From x In db.fDatas _
                              Select x _
                              Where (x.DataID = CType(holditem, Integer))).Single
                query2.MOR = CType(txtMORNew.Text, Integer)
            Next
            db.SubmitChanges()
            sMessage = "MOR updated successfully"
            displayMessageBox(sMessage)
        Else
            litStatus.Text = "There is no data that matches the search parameters entered. Please try again"
        End If

        'Catch ex As Exception
        'gf.NotifyWebmaster(ex, Request.ServerVariables.Item("PATH_INFO"), "MOR update error")
        'MultiView1.SetActiveView(vwError)
        'End Try

    End Sub

    Private Sub SetPageTitle(ByVal TitleText As String)

        If Me.Page.Form.Parent IsNot Nothing Then
            Dim litPageTitle As Literal = Me.Page.Form.Parent.FindControl("litPageTitle")

            If litPageTitle IsNot Nothing Then
                litPageTitle.Text = TitleText
            End If
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