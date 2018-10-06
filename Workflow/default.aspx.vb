Partial Public Class _default0
    Inherits System.Web.UI.Page
    Protected gl As Globals.GlobalFunctions = New Globals.GlobalFunctions

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'If Not User.IsInRole("SuperAdmin") Then
        '    MultiView1.SetActiveView(vwNotAuthorized)
        'Else
        If Not Page.IsPostBack Then
            LoadSurveys(CType(Session("OnyxID"), Integer))
        End If
        'End If
    End Sub
    'Populates the drop down list with existing surveys for the logged in user
    Private Sub LoadSurveys(ByVal OnyxID As Integer)

        'MB (3/25/2011) - remove Try/Catch block and use global error handlng
        'Try

        Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext()

        Dim query = From u In db.admUserSurveys Join p In db.dSurveys _
                    On u.SurveyID Equals p.SurveyID _
                    Order By p.SurveyName _
                    Where u.OnyxID = OnyxID _
                    Select p.SurveyID, p.SurveyName

        'Sets the properties for the survey drop down list for the logged in user
        'Adds the first item, maps the value and text properties to items in the query
        'Sets the datasource of the drop down list to the query results, displays the appropriate view

        ddlSurveys.Items.Clear()
        ddlSurveys.AppendDataBoundItems = True
        ddlSurveys.Items.Add(New ListItem("Please Choose", "NA"))
        ddlSurveys.DataTextField = "SurveyName"
        ddlSurveys.DataValueField = "SurveyID"
        ddlSurveys.DataSource = query
        ddlSurveys.DataBind()

        MultiView1.SetActiveView(vwChooseSurvey)

        'Catch ex As Exception
        'gl.NotifyWebmaster(ex, gl.GetCurrentUrl, "Load surveys into drop down list error")
        'MultiView1.SetActiveView(vwError)
        'End Try

    End Sub

    Private Sub btnContinue_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnContinue.Click

        Session("SurveyID") = ddlSurveys.SelectedValue
        Session("SurveyName") = ddlSurveys.SelectedItem.Text

        'MB (3/25/2011) - remove Try/Catch block and use global error handlng
        'Try
        Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext
        Dim SurveyID As Integer

        If Integer.TryParse(ddlSurveys.SelectedValue, SurveyID) Then

            Dim query = From s In db.dSurveys _
                        Where s.SurveyID = SurveyID _
                        Select s

            'Sets the default survey home page path based on the survey selected
            Dim SurveyHome As String = String.Empty
            If query.Count > 0 Then
                For Each item In query
                    SurveyHome = item.SurveyHomePage.ToString
                Next
                Response.Redirect(SurveyHome, False)
            End If
        Else

        End If


        'Catch ex As Exception
        'gl.NotifyWebmaster(ex, gl.GetCurrentUrl, "Select survey home page error")
        'MultiView1.SetActiveView(vwError)
        'End Try

    End Sub
End Class