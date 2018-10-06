Imports Telerik.Web.UI
Imports Telerik.Web.UI.Upload

Partial Public Class CreateFactTable1
    Inherits System.Web.UI.UserControl

    Protected SurveyID As Integer
    Protected progress As RadProgressContext = RadProgressContext.Current



    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Page.User.IsInRole("SuperAdmin") OrElse Page.User.IsInRole("Admin") OrElse _
                 Page.User.IsInRole("SuperAnalyst") OrElse Page.User.IsInRole("Analyst") Then

            SurveyID = CType(Session("SurveyID").ToString(), Integer)

            If Not Page.IsPostBack Then

                LoadSurveySeries()
                MultiView1.SetActiveView(vwDefault)
                litSurveyName.Text = " - " & Session("SurveyName")

            End If
        Else
            MultiView1.SetActiveView(vwNotAuthorized)
        End If
    End Sub

    Private Sub LoadSurveySeries()
        Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext

        Dim query = From p In db.dSurveySeries _
                    Where p.SurveyID = SurveyID _
                    Order By p.DateID Descending _
                    Select p.SurveySeriesID, DateSeries = p.DateID.ToShortDateString

        ddlSurveySeries.Items.Clear()
        ddlSurveySeries.AppendDataBoundItems = True
        ddlSurveySeries.Items.Add(New ListItem("Please Choose", "NA"))
        ddlSurveySeries.DataValueField = "SurveySeriesID"
        ddlSurveySeries.DataTextField = "DateSeries"
        ddlSurveySeries.DataSource = query
        ddlSurveySeries.DataBind()

    End Sub

    Protected Sub btnSelectDateSeries_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSelectDateSeries.Click

        If Page.IsValid Then

            progress.PrimaryTotal = 100
            progress.PrimaryValue = 0
            progress.PrimaryPercent = 0

            Dim cft As Projects.Algorithms = New Projects.Algorithms
            Dim SurveySeriesID As Integer = CType(ddlSurveySeries.SelectedValue, Integer)

            If cft.CreateFactTables(SurveySeriesID, progress) Then
                MultiView1.SetActiveView(vwConfirm)
            Else
                MultiView1.SetActiveView(vwError)
            End If

          
        End If
    End Sub
End Class