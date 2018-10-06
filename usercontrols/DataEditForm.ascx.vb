Public Partial Class DataEditForm
    Inherits System.Web.UI.UserControl


    'Property set when inserting this control into a page
    Private _SurveyID As Integer

    Public Property SurveyID() As Integer
        Get
            Return _SurveyID
        End Get
        Set(ByVal value As Integer)
            _SurveyID = value
        End Set
    End Property



    'The page will check to see if the person requesting the page is authorized to see this page
    'If they are not logged in, they will be sent to the login page. If they are logging in but not
    'authorized, the Not Authorized view will be displayed
    'Since this page will postback multiple times, we first check to see if the page was loading after a postback
    'If not, we display the default screen. Postbacks will be handled by the On Click Event Handlers for the buttons
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' If Page.User.IsInRole("Admin") Or Page.User.IsInRole("COLIBOLI") Then
        If Not Page.IsPostBack Then
            LoadSurveySeries(_SurveyID)
        End If
        btnCancel.Attributes.Add("onclick", "return confirm('Are you sure you want to cancel?\nAny usaved changes made to the data will be lost.');")
        'Else
        'MultiView1.SetActiveView(vwNotAuthorized)
        'End If
      
    End Sub

    'Populates a list of existing participations for a given survey
    Private Sub LoadSurveySeries(ByVal SurveyID As Integer)
        Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext

        Dim query = From ss In db.dSurveySeries Join s In db.dSurveys _
                    On ss.SurveyID Equals s.SurveyID _
                    Join f In db.fDatas On ss.SurveySeriesID Equals f.SurveySeriesID _
                    Join p In db.dWorkbooks On f.WorkbookID Equals p.WorkbookID _
                    Where s.SurveyID = SurveyID _
                    Select ss.SurveySeriesID, ss.Quarter, ss.Year, s.SurveyName, p.WorkbookName
        If query.Count > 0 Then
            ddlSurveySeries.Items.Clear()
            ddlSurveySeries.Items.Add(New ListItem("Please Choose", "NA"))
            For Each item In query
                ddlSurveySeries.Items.Add(New ListItem(item.WorkbookName & ": Q" & item.Quarter.ToString & " " & item.Year.ToString, item.SurveySeriesID.ToString))
                litPageTitle.Text = item.SurveyName
            Next
        End If
        MultiView1.SetActiveView(vwChooseSurveySeries)
    End Sub

    'We need the SurveySeriesID, PageNumber, and SectionNumber to populate the edit form
    Private Sub LoadFormDataForEdit(ByVal SurveySeriesID As Integer, ByVal PageNumber As Integer, _
                                    ByVal SectionNumber As Integer, ByVal WorkbookID As Integer)

        'Stores the values for updating
        ViewState("SurveySeriesID") = SurveySeriesID
        ViewState("PageNumber") = PageNumber
        ViewState("SectionNumber") = SectionNumber
        ViewState("WorkbookID") = WorkbookID

        ClearEditForm()

        Try
            Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext


            Dim query = From d In db.fDatas _
                        Join l In db.fLayouts On d.FieldID Equals l.FieldID _
                        Where d.SurveySeriesID = SurveySeriesID _
                        And l.PageNumber = PageNumber _
                        And l.SectionNumber = SectionNumber _
                        And d.WorkbookID = WorkbookID _
                        Select d.DataID, l.RowNumber, l.ColumnNumber, d.Value, d.Number

            'If the query returns results, we loop through the results
            'for each item(data row) in query, we construct the textbox id from the RowNumber and columid, find the textbox
            'in the controls in the edit view - if we find the textbox, we set the text value to the data value, then store
            'the dataID in a viewstate variable named after the textbox.
            If query.Count > 0 Then
                Dim thisText As TextBox
                For Each item In query
                    thisText = CType(vwEditData.FindControl("Row" & item.RowNumber.ToString & "Col" & item.ColumnNumber.ToString), TextBox)
                    If thisText IsNot Nothing Then
                        If item.Value IsNot Nothing Then
                            thisText.Text = item.Value
                        End If
                        ViewState(thisText.ID.ToString) = item.DataID
                    End If
                Next
                MultiView1.SetActiveView(vwEditData)
            Else
                'No Data Found
            End If
        Catch ex As Exception

        End Try

    End Sub
    Private Sub ClearEditForm()
        For Each thisControl As Control In vwEditData.Controls
            Select Case thisControl.GetType.ToString
                Case "System.Web.UI.WebControls.TextBox"
                    Dim thisTextBox As TextBox = New TextBox
                    thisTextBox = CType(thisControl, TextBox)
                    thisTextBox.Text = ""
                Case Else
            End Select
        Next
    End Sub


    'This function will calculate the proper value to put into the textbox based on business rules
    'For now, we are just returning the value, and not applying any rules
    'We might move this function into the global functions

    Private Function GetTextboxValue(ByVal DataValue As String, ByVal Number As Double) As String
        Return DataValue
    End Function


    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCancel.Click
      
        ClearEditForm()
        LoadSurveySeries(CType(ViewState("SurveyID"), Integer))
    End Sub

    Private Sub btnGoToEdit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnGoToEdit.Click
        LoadFormDataForEdit(1, 11, 1, 3)
    End Sub

    Protected Sub btnUpdate_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnUpdate.Click
        If ValidateForm() Then
            Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext

            For Each thisControl As Control In vwEditData.Controls
                Select Case thisControl.GetType.ToString
                    Case "System.Web.UI.WebControls.TextBox"
                        Dim thisTextBox As TextBox = New TextBox
                        thisTextBox = CType(thisControl, TextBox)
                        Dim query = (From p In db.fDatas _
                                     Select p _
                                    Where (p.DataID = CType(ViewState(thisTextBox.ID.ToString), Integer))).Single

                        Select Case thisTextBox.Text.ToString.Trim
                            Case String.Empty
                                query.Value = String.Empty
                                query.Number = 0

                            Case "M"
                                query.Value = "M"
                                'sets the value of number = null
                                query.Number = Nothing

                            Case Else
                                query.Value = thisTextBox.Text
                                query.Number = CType(thisTextBox.Text, Double)
                        End Select
                        db.SubmitChanges()

                    Case Else
                End Select
            Next
        End If
    End Sub

    'validates the data in the form - checks totals, etc.
    Protected Function ValidateForm() As Boolean
        Return True
    End Function

End Class