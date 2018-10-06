Imports Telerik.Web.UI


Public Class IDLookups
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            MultiView1.SetActiveView(vwDefault)
            LoadSurveys()
            litSurvey.Visible = False
        End If
    End Sub

    Protected Sub LoadSurveys()
        Try

            Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext()


            Dim query = From p In db.dSurveys _
                        Order By p.SurveyName _
                        Select p.SurveyID, p.SurveyName

     
            ddlSurveys.Items.Clear()
            ddlSurveys.AppendDataBoundItems = True
            ddlSurveys.Items.Add(New ListItem("Please Choose", "NA"))
            ddlSurveys.DataTextField = "SurveyName"
            ddlSurveys.DataValueField = "SurveyID"
            ddlSurveys.DataSource = query
            ddlSurveys.DataBind()
            MultiView1.SetActiveView(vwDefault)

        Catch ex As Exception
            Dim gf As Globals.GlobalFunctions = New Globals.GlobalFunctions
            gf.NotifyWebmaster(ex, gf.GetCurrentUrl, "Load surveys into drop down list error")
            MultiView1.SetActiveView(vwError)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub RadGridSourceIds_NeedDataSource(sender As Object, e As GridNeedDataSourceEventArgs)
        Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext()
        Dim survey As Integer = CType(ddlSurveys.SelectedItem.Value, Integer)

        Try
            Dim surveyIds = From p In db.dSources
                            Where p.SurveyID = survey
                            Order By p.SourceName
                            Select p.SourceID, p.SourceName, p.PrefName, p.OrganizationID, p.SurveyID, p.ParentID, p.GroupID

            RadGridSourceIds.DataSource = surveyIds.ToList()


        Catch ex As Exception
            Dim gf As Globals.GlobalFunctions = New Globals.GlobalFunctions
            gf.NotifyWebmaster(ex, gf.GetCurrentUrl, "ID Lookups Source IDS Need Data Source Error")
            MultiView1.SetActiveView(vwError)
        Finally
            db.Dispose()
        End Try


    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub RadGridGroupIds_NeedDataSource(sender As Object, e As GridNeedDataSourceEventArgs)
        Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext()
        Dim surveyIDs As Integer = CType(ddlSurveys.SelectedItem.Value, Integer)

        Try
            Dim groupIds = From p In db.dGroups
                           Where p.SurveyID = surveyIDs
                           Order By p.PrefGroupName
                           Select p.GroupID, p.SurveyID, p.PrefGroupName

            RadGridGroupIds.DataSource = groupIds.ToList()

        Catch ex As Exception
            Dim gf As Globals.GlobalFunctions = New Globals.GlobalFunctions
            gf.NotifyWebmaster(ex, gf.GetCurrentUrl, "ID Lookups Group IDS Need Data Source Error")
            MultiView1.SetActiveView(vwError)

        Finally
            db.Dispose()

        End Try

    End Sub


    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub RadGridOrganizations_NeedDataSource(sender As Object, e As GridNeedDataSourceEventArgs)
        Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext()

        Try
            Dim orgs = From p In db.dOrgMasters
                       Order By p.LegalName
                       Select p.OrganizationID, p.CRMID, p.LegalName, p.InactiveB

            RadGridOrganizations.DataSource = orgs.ToList()
        Catch ex As Exception
            Dim gf As Globals.GlobalFunctions = New Globals.GlobalFunctions
            gf.NotifyWebmaster(ex, gf.GetCurrentUrl, "ID Lookups Organization IDS Need Data Source Error")
            MultiView1.SetActiveView(vwError)

        Finally
            db.Dispose()

        End Try

    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub btnGetSourceIDs_Click(sender As Object, e As EventArgs) Handles btnGetSourceIDs.Click
        RadGridSourceIds.Rebind()
        MultiView1.SetActiveView(vwSourceIds)
        litSurvey.Text = " - " + ddlSurveys.SelectedItem.Text
        litSurvey.Visible = True
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub btnGetGroupIDs_Click(sender As Object, e As EventArgs) Handles btnGetGroupIDs.Click
        RadGridGroupIds.Rebind()
        MultiView1.SetActiveView(vwGroupIds)
        litSurvey.Text = " - " + ddlSurveys.SelectedItem.Text
        litSurvey.Visible = True
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub btnGetOrganizations_Click(sender As Object, e As EventArgs) Handles btnGetOrganizations.Click
        RadGridOrganizations.Rebind()
        MultiView1.SetActiveView(vwOrganizations)
        litSurvey.Visible = False
    End Sub

    Protected Sub btnBack_Click(sender As Object, e As EventArgs) Handles btnBack.Click, btnBack1.Click, btnBack2.Click
        MultiView1.SetActiveView(vwDefault)
        ddlSurveys.ClearSelection()
        litSurvey.Visible = False
    End Sub

   
End Class