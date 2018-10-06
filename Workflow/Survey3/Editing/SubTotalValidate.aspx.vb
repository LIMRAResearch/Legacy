Imports Telerik.Web.UI
Imports Telerik.Web.UI.Upload


Partial Public Class SubTotalValidate3
    Inherits System.Web.UI.Page


    Protected gf As Globals.GlobalFunctions = New Globals.GlobalFunctions
    Protected sf As Projects.Survey2Functions = New Projects.Survey2Functions

    Protected WorkbookID As Integer = 0

    ''' <summary>
    ''' Checks for perms, loads dropdown lists, displays default page
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Page.User.IsInRole("SuperAdmin") Or Page.User.IsInRole("Admin") Or _
           Page.User.IsInRole("SuperAnalyst") Or Page.User.IsInRole("Analyst") Then
            litStatus.Text = ""
            If Not Page.IsPostBack Then
                LoadDates()
                SetPageTitle(Session("SurveyName"))
            End If
            ' btnCancel.Attributes.Add("onclick", "return confirm('Are you sure you want to cancel?\nAny changes made to the data will be lost.');")
        Else
            MultiView1.SetActiveView(vwNotAuthorized)
        End If

    End Sub

    ''' <summary>
    ''' Populates a list of existing survey series (year and quarter) for a given survey
    ''' </summary>
    ''' <remarks></remarks>
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

    ''' <summary>
    ''' Sets the page title on the master page
    ''' </summary>
    ''' <param name="TitleText">String</param>
    ''' <remarks></remarks>
    Private Sub SetPageTitle(ByVal TitleText As String)
        If Me.Form.Parent IsNot Nothing Then
            Dim litPageTitle As Literal = Me.Form.Parent.FindControl("litPageTitle")

            If litPageTitle IsNot Nothing Then
                litPageTitle.Text = TitleText
            End If
        End If
    End Sub


    ''' <summary>
    ''' Loads the choice of workbooks based on the Date Series selected
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub ddlDateSeries_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ddlDateSeries.SelectedIndexChanged
        Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext

        ddlWorkbooks.Items.Clear()
        ddlWorkbooks.Items.Add(New ListItem("Please Choose", "NA"))
        Dim query = From p In db.dWorkbooks _
                    Where p.SurveySeriesID = CType(ddlDateSeries.SelectedValue, Integer) _
                    And p.WorkbookStatus.ToString.ToLower <> "p" _
                    Order By p.DateID Descending _
                    Select p.WorkbookID, p.WorkbookName

        If query.Count > 0 Then
            For Each item In query
                ddlWorkbooks.Items.Add(New ListItem(item.WorkbookName.ToString, item.WorkbookID))
            Next
        End If
    End Sub


    ''' <summary>
    ''' Gets a list of page id's and section id's based on workbook id
    ''' loops through the list, running the validateWorksheet sub for each set of Page id's and section id's
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub btnValidate_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnValidate.Click

        Dim progress As RadProgressContext = RadProgressContext.Current
        Dim iprogress As Integer = 0
        Dim iprogressTotal As Integer = 0

       
        WorkbookID = CType(ddlWorkbooks.SelectedValue, Integer)
        Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext
        Dim EditStatus As Boolean = True
        Session("WkBkStatus") = EditStatus

        Dim query = From p In db.fDatas _
                Join l In db.fLayouts On p.FieldID Equals l.FieldID _
                Where p.WorkbookID = WorkbookID _
                And l.Inactive Is Nothing _
                Select l.PageNumber, l.SectionNumber Distinct

        iprogressTotal = 40
        progress.PrimaryTotal = iprogressTotal
        litBadTotals.Text = ""

        For Each item In query
            validateWorksheet(WorkbookID, item.PageNumber, item.SectionNumber, db)
            iprogress = iprogress + 1
            progress.PrimaryValue = iprogress
            progress.PrimaryPercent = 100 * iprogress / iprogressTotal
        Next

        Dim StatusQuery = (From w In db.dWorkbooks _
                           Select w _
                           Where (w.WorkbookID = WorkbookID)).Single
        If Session("WkBkStatus") = True Then
            StatusQuery.WorkbookStatus = "P"
            litBadTotals.Text &= "<br /> No Errors Found -- Workbook passed the edit "
        Else
            StatusQuery.WorkbookStatus = "F"
        End If
        db.SubmitChanges()
        progress.PrimaryValue = iprogressTotal
        progress.PrimaryPercent = 100
        MultiView1.SetActiveView(vwReport)
        SetPageTitle(Session("SurveyName") & " • " & CType(StatusQuery.WorkbookName, String) & " • " & _
                     CType(StatusQuery.DateID, String))


    End Sub

    ''' <summary>
    ''' Runs custom validations for sub totals
    ''' </summary>
    ''' <param name="WorkbookID"></param>
    ''' <param name="PageNo"></param>
    ''' <param name="SectionNumber"></param>
    ''' <param name="db"></param>
    ''' <remarks></remarks>
    Protected Sub validateWorksheet(ByVal WorkbookID As Integer, ByVal PageNo As Integer, ByVal SectionNumber As Integer, ByRef db As SalesSurveyDBDataContext)

        Dim ColumnNum, rowNum, iColCnt As Integer
        Dim cols() As Integer

        'Section 1 of the survey has 4 cols, the subtotals of second section /second page are not validated
        If SectionNumber = 1 And
           PageNo = 1 Then

            iColCnt = 4
            cols = New Integer() {1, 2, 3, 4}



            'Chooses all data points in a workbook based on workbookID
            Dim query = From d In db.fDatas _
                        Join l In db.fLayouts On d.FieldID Equals l.FieldID _
                        Join p In db.fPages On l.PageNumber Equals p.PageNumber And _
                                               l.SurveyID Equals p.SurveyID _
                        Join s In db.fSections On l.SectionNumber Equals s.SectionNumber And _
                                                  l.SurveyID Equals s.SurveyID And _
                                                  l.PageNumber Equals s.PageNumber _
                        Join r In db.fRows On l.RowNumber Equals r.RowNumber And _
                                              l.SurveyID Equals r.SurveyID And _
                                              l.PageNumber Equals r.PageNumber And _
                                              l.SectionNumber Equals r.SectionNumber _
                        Join c In db.fColumns On l.ColumnNumber Equals c.ColumnNumber And _
                                                 l.SurveyID Equals c.SurveyID And _
                                                 l.PageNumber Equals c.PageNumber And _
                                                 l.SectionNumber Equals c.SectionNumber _
                        Join w In db.dWorkbooks On d.WorkbookID Equals w.WorkbookID _
                        Where d.WorkbookID = WorkbookID _
                        And l.PageNumber = PageNo _
                        And l.SectionNumber = SectionNumber _
                        And l.Inactive Is Nothing _
                        Select p.PageDescription, s.SectionDescription, r.RowDescription, c.ColumnDescription, d.Value, _
                        w.WorkbookName, l.RowNumber, l.ColumnNumber, l.PageNumber _
                        Order By RowNumber, ColumnNumber

            'Loops through the column numbers, creating a query based on the column number, and the row numbers in each series of data points making up a sub total
            'Checks Total variable annuities
            Dim rows() As Integer = {1, 2, 3, 5, 6, 7}
            For ColumnNum = 1 To iColCnt
                'Creates a query for the data points for validating the sub total for rows 1 - 5 Total Universal Life
                Dim query2 = From x In query _
                        Where (x.ColumnNumber = ColumnNum _
                        And rows.Contains(x.RowNumber) And x.PageNumber = 1) _
                        Select x

                'converts the query to a list, so we can access row number by index
                Dim thisList = query2.ToList

                Try
                    'runs the validation function for 4 numbers and a total - if the numbers do not validate, it creates a Bad Sub total line in a literal 
                    'If an exception is thrown, a missing or duplicate data line is written to the literal
                    'Exception subscript is set to 0 in case less than 4 rows are entered
                    If Not sf.CheckSubTotals2(thisList(0).Value, thisList(1).Value, thisList(2).Value) Then
                        litBadTotals.Text &= "<br /> Bad SubTotal: " & thisList(2).PageDescription & " • " & thisList(2).SectionDescription & " • " & thisList(2).RowDescription & " • " & thisList(2).ColumnDescription
                        Session("WkBkStatus") = False
                    End If
                Catch ex As Exception
                    litBadTotals.Text &= "<br /> Missing or duplicate data: " & thisList(0).PageDescription & " • " & thisList(0).SectionDescription & " • " & thisList(0).RowDescription & " • " & thisList(0).ColumnDescription
                    Session("WkBkStatus") = False
                End Try
                Try
                    If Not sf.CheckSubTotals2(thisList(3).Value, thisList(4).Value, thisList(5).Value) Then
                        litBadTotals.Text &= "<br /> Bad SubTotal: " & thisList(5).PageDescription & " • " & thisList(5).SectionDescription & " • " & thisList(5).RowDescription & " • " & thisList(5).ColumnDescription
                        Session("WkBkStatus") = False
                    End If
                Catch ex As Exception
                    litBadTotals.Text &= "<br /> Missing or duplicate data: " & thisList(3).PageDescription & " • " & thisList(3).SectionDescription & " • " & thisList(3).RowDescription & " • " & thisList(3).ColumnDescription
                    Session("WkBkStatus") = False
                End Try
            Next

            'Checks total NFP

            For rowNum = 1 To 16
                Dim query3 = From x In query _
                      Where (x.RowNumber = rowNum _
                      And cols.Contains(x.ColumnNumber) And x.PageNumber = 1) _
                      Select x

                'converts the query to a list, so we can access row number by index
                Dim thisList = query3.ToList

                Try
                    If Not sf.CheckSubTotals3(thisList(0).Value, thisList(1).Value, thisList(2).Value, thisList(3).Value) Then
                        litBadTotals.Text &= "<br /> Bad SubTotal: " & thisList(3).PageDescription & " • " & thisList(3).SectionDescription & " • " & thisList(3).RowDescription & " • " & thisList(3).ColumnDescription
                        Session("WkBkStatus") = False
                    End If
                Catch ex As Exception
                    litBadTotals.Text &= "<br /> Missing or duplicate data: " & thisList(0).PageDescription & " • " & thisList(0).SectionDescription & " • " & thisList(0).RowDescription & " • " & thisList(0).ColumnDescription
                    Session("WkBkStatus") = False
                End Try

            Next

        End If 'SectionNumber = 1
    End Sub

    ''' <summary>
    ''' Returns user to default page to run another validation
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub btnSubmit_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSubmit.Click
        MultiView1.SetActiveView(vwChooseSurveySeries)
    End Sub


End Class