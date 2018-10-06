Public Partial Class Corrections
    Inherits System.Web.UI.Page

    Protected sf As Projects.Survey2Functions = New Projects.Survey2Functions
    Protected gf As Globals.GlobalFunctions = New Globals.GlobalFunctions
    Private Const CSSCLASS_VALIDATED = "TextRight"

    ''' <summary>
    ''' Checks for user perms, loads dropdown lists, displays default page view
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Page.User.IsInRole("SuperAdmin") OrElse Page.User.IsInRole("Admin") OrElse _
                     Page.User.IsInRole("SuperAnalyst") OrElse Page.User.IsInRole("Analyst") Then

            litStatus.Text = ""
            If Not Page.IsPostBack Then
                LoadDates()
                LoadDistributions()
                SetPageTitle(Session("SurveyName"))
            End If
            btnCancel.Attributes.Add("onclick", "return confirm('Are you sure you want to cancel?\nAny unsaved changes made to the data will be lost.');")
        Else
            MultiView1.SetActiveView(vwNotAuthorized)
        End If

    End Sub


    ''' <summary>
    ''' Loads form with data to edit
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub btnContinue_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnContinue.Click
        'MultiView1.SetActiveView(vwEdit)
        LoadFormDataForEdit(CType(ddlDateSeries.SelectedValue, Integer), _
                         CType(ddlWorkbooks.SelectedValue, Integer), _
                         CType(ddlDistributions.SelectedValue, Integer), _
                         CType(txtMOR.Text, Integer))
    End Sub



    ''' <summary>
    ''' Sets all textboxes to String.Empty
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ClearEditForm(ByRef vw As View)
        For Each thisControl As Control In vw.Controls
            Select Case thisControl.GetType.ToString
                Case "System.Web.UI.WebControls.TextBox"
                    Dim thisTextBox As TextBox = New TextBox
                    thisTextBox = CType(thisControl, TextBox)
                    thisTextBox.Text = ""
                Case Else
            End Select
        Next
    End Sub


    ''' <summary>
    ''' Sets the title of the page on the master page
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
    '''  Populates a list of existing survey series (year and quarter) for a given survey
    ''' </summary>
    ''' <remarks></remarks>
    Protected Sub LoadDates()
        ddlDateSeries.Items.Clear()
        ddlDateSeries.Items.Add(New ListItem("Please Choose", "NA"))

        'MB (3/25/2011) - remove Try/Catch block and use global error handlng
        'Try

        Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext()

        Dim query = From p In db.dSurveySeries _
                    Where p.SurveyID = CType(Session("SurveyID"), Integer) _
                    Order By p.DateID Descending _
                    Select p.DateID, p.SurveySeriesID

        'loops through the query, truncates the date to get rid of the time value
        If query.Count > 0 Then
            For Each item In query
                ddlDateSeries.Items.Add(New ListItem(item.DateID.ToShortDateString, item.SurveySeriesID.ToString))
            Next
        End If

        'Catch ex As Exception
        'gf.NotifyWebmaster(ex, gf.GetCurrentUrl, "Load dates into combo box list error")
        'MultiView1.SetActiveView(vwError)
        'End Try

    End Sub


    ''' <summary>
    '''  Populates a list of existing workbooks for a given survey series
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ddlDateSeries_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlDateSeries.SelectedIndexChanged

        'MB (3/25/2011) - remove Try/Catch block and use global error handlng
        'Try

        Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext()

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

        'Catch ex As Exception
        'gf.NotifyWebmaster(ex, gf.GetCurrentUrl, "Load workbooks into combo box list error")
        'MultiView1.SetActiveView(vwError)
        'End Try

    End Sub

    ''' <summary>
    ''' Populates a list of existing distributions (pages) for a given survey series and participant
    ''' </summary>
    ''' <remarks></remarks>
    Protected Sub LoadDistributions()

        'MB (3/25/2011) - remove Try/Catch block and use global error handlng
        'Try

        Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext()

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
        MultiView1.SetActiveView(vwDefault)

        'Catch ex As Exception
        'gf.NotifyWebmaster(ex, gf.GetCurrentUrl, "Load distributions into drop down list error")
        'MultiView1.SetActiveView(vwError)
        'End Try

    End Sub

    ''' <summary>
    '''  define a javascript alertbox containing the string passed in as argument
    ''' </summary>
    ''' <param name="msg"></param>
    ''' <remarks></remarks>
    Private Sub displayMessageBox(ByVal msg As String)

       

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


    ''' <summary>
    ''' We need the SurveySeriesID, PageNumber, and SectionNumber to populate the edit form
    ''' </summary>
    ''' <param name="SurveySeriesID"></param>
    ''' <param name="WorkbookID"></param>
    ''' <param name="pageNum"></param>
    ''' <param name="MOR"></param>
    ''' <remarks></remarks>
    Private Sub LoadFormDataForEdit(ByVal SurveySeriesID As Integer, ByVal WorkbookID As Integer, _
                                    ByVal pageNum As Integer, ByVal MOR As Integer)
        Dim currentView As View = New View

        'Stores the values for updating
        ViewState("SurveySeriesID") = SurveySeriesID
        ViewState("PageNumber") = pageNum
        ViewState("WorkbookID") = WorkbookID
        ViewState("MOR") = MOR
        Session("WorkbookID") = WorkbookID


        If pageNum = 1 Then
            currentView = vwEdit
        ElseIf pageNum = 2 Then
            currentView = vwEdit2
        End If

        MultiView1.SetActiveView(currentView)

        ClearEditForm(currentView)

        

        Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext()

        Dim query = From d In db.fDatas _
                    Join l In db.fLayouts On d.FieldID Equals l.FieldID _
                    Join p In db.fPages On l.PageNumber Equals p.PageNumber And _
                                           l.SurveyID Equals p.SurveyID
                    Where l.SurveyID = CType(Session("SurveyID"), Integer) And _
                          d.SurveySeriesID = SurveySeriesID And _
                          l.PageNumber = pageNum And _
                          d.WorkbookID = WorkbookID And _
                          d.MOR = MOR _
                          And l.Inactive Is Nothing _
                    Select d.DataID, l.SectionNumber, l.RowNumber, l.ColumnNumber, l.PageNumber, d.Value, d.Number Distinct
                    Order By SectionNumber, RowNumber, ColumnNumber

        ' Changed by Hetal: 09/30/2011- Select d.DataID, l.RowNumber, l.ColumnNumber, p.PageNumber, d.Value, d.Number

        'If the query returns results, we loop through the results
        'for each item(data row) in query, we construct the textbox id from the RowNumber and columid, find the textbox
        'in the controls in the edit view - if we find the textbox, we set the text value to the data value, then store
        'the dataID in a viewstate variable named after the textbox.

        'RP:5/8/2012 
        'changes to fLayout db schema added Section entity so that item.RowNumber repeats per section
        'to accomodate this change in existing surveys we will use iRowIdx to specify correct textbox for
        'a given data point
        'new surveys should use the following convention for textbox id's: P##S##R###C### (Page, Section, Row, Column)
        'this will allow the database values to be used directly to specify a corresponding textbox

        Dim iRowIdx As Integer

        If query.Count > 0 Then
            Dim thisText As TextBox
            For Each item In query

                If item.ColumnNumber = 1 Then iRowIdx += 1

                thisText = CType(currentView.FindControl("P" & pageNum.ToString & "Row" & iRowIdx.ToString & "Col" & Trim(item.ColumnNumber.ToString)), TextBox)
                If thisText IsNot Nothing Then
                    If item.Value IsNot Nothing Then
                        thisText.Text = item.Value
                    End If
                    'TODO RP this is too much data in ViewState
                    ViewState(thisText.ID.ToString) = item.DataID
                End If

            Next
            ValidateTotals()
            'MultiView1.SetActiveView(vwEdit)
            SetPageTitle(Session("SurveyName") & " • " & ddlWorkbooks.SelectedItem.Text & " • " & _
            ddlDateSeries.SelectedItem.Text & " • " & ddlDistributions.SelectedItem.Text & " • MOR: " & txtMOR.Text)
        Else
            litStatus.Text = "There is no data that matches the search parameters entered. Please try again"
            MultiView1.SetActiveView(vwDefault)
        End If

        'Catch ex As Exception
        'gf.NotifyWebmaster(ex, Request.ServerVariables.Item("PATH_INFO"), "Load form data for edit error")
        'MultiView1.SetActiveView(vwError)
        'End Try

    End Sub

    ''' <summary>
    ''' performs subtotal validation
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    ''' 
    Protected Function ValidateTotals() As Boolean
        'TODO RP no need for .text.tostring; text is sufficient

        Dim SubTotalsOK As Boolean = True

        'Total Variable Annuities
        If Not sf.CheckSubTotals2(P1Row1Col1.Text.ToString, P1Row2Col1.Text.ToString, P1Row3Col1.Text.ToString, P1Row3Col1) Then
            SubTotalsOK = False
        End If
        If Not sf.CheckSubTotals2(P1Row1Col2.Text.ToString, P1Row2Col2.Text.ToString, P1Row3Col2.Text.ToString, P1Row3Col2) Then
            SubTotalsOK = False
        End If
        If Not sf.CheckSubTotals2(P1Row1Col3.Text.ToString, P1Row2Col3.Text.ToString, P1Row3Col3.Text.ToString, P1Row3Col3) Then
            SubTotalsOK = False
        End If
        If Not sf.CheckSubTotals2(P1Row1Col4.Text.ToString, P1Row2Col4.Text.ToString, P1Row3Col4.Text.ToString, P1Row3Col4) Then
            SubTotalsOK = False
        End If

        'Total group annuities
        If Not sf.CheckSubTotals2(P1Row5Col1.Text.ToString, P1Row6Col1.Text.ToString, P1Row7Col1.Text.ToString, P1Row7Col1) Then
            SubTotalsOK = False
        End If
        If Not sf.CheckSubTotals2(P1Row5Col2.Text.ToString, P1Row6Col2.Text.ToString, P1Row7Col2.Text.ToString, P1Row7Col2) Then
            SubTotalsOK = False
        End If
        If Not sf.CheckSubTotals2(P1Row5Col3.Text.ToString, P1Row6Col3.Text.ToString, P1Row7Col3.Text.ToString, P1Row7Col3) Then
            SubTotalsOK = False
        End If
        If Not sf.CheckSubTotals2(P1Row5Col4.Text.ToString, P1Row6Col4.Text.ToString, P1Row7Col4.Text.ToString, P1Row7Col4) Then
            SubTotalsOK = False
        End If

        'Total Contributions
        If Not sf.CheckSubTotals5(P1Row3Col1.Text, P1Row4Col1.Text, P1Row7Col1.Text, P1Row8Col1.Text, P1Row9Col1.Text, P1Row10Col1.Text, P1Row10Col1) Then
            SubTotalsOK = False
        End If
        If Not sf.CheckSubTotals5(P1Row3Col2.Text, P1Row4Col2.Text, P1Row7Col2.Text, P1Row8Col2.Text, P1Row9Col2.Text, P1Row10Col2.Text, P1Row10Col2) Then
            SubTotalsOK = False
        End If
        If Not sf.CheckSubTotals5(P1Row3Col3.Text, P1Row4Col3.Text, P1Row7Col3.Text, P1Row8Col3.Text, P1Row9Col3.Text, P1Row10Col3.Text, P1Row10Col3) Then
            SubTotalsOK = False
        End If
        If Not sf.CheckSubTotals5(P1Row3Col4.Text, P1Row4Col4.Text, P1Row7Col4.Text, P1Row8Col4.Text, P1Row9Col4.Text, P1Row10Col4.Text, P1Row10Col4) Then
            SubTotalsOK = False
        End If

        'Total Assets
        If Not sf.CheckSubTotals3(P1Row11Col1.Text, P1Row12Col1.Text, P1Row13Col1.Text, P1Row14Col1.Text, P1Row14Col1) Then
            SubTotalsOK = False
        End If
        If Not sf.CheckSubTotals3(P1Row11Col2.Text, P1Row12Col2.Text, P1Row13Col2.Text, P1Row14Col2.Text, P1Row14Col2) Then
            SubTotalsOK = False
        End If
        If Not sf.CheckSubTotals3(P1Row11Col3.Text, P1Row12Col3.Text, P1Row13Col3.Text, P1Row14Col3.Text, P1Row14Col3) Then
            SubTotalsOK = False
        End If
        If Not sf.CheckSubTotals3(P1Row11Col4.Text, P1Row12Col4.Text, P1Row13Col4.Text, P1Row14Col4.Text, P1Row14Col4) Then
            SubTotalsOK = False
        End If




        'Line totals (Horizontal Totals)
        'Variable subAccounts - Total NFP
        If Not sf.CheckSubTotals3(P1Row1Col1.Text.ToString, P1Row1Col2.Text.ToString, P1Row1Col3.Text.ToString, P1Row1Col4.Text.ToString, P1Row1Col4) Then
            SubTotalsOK = False
        End If

        'Fixed subaccounts - Total NFP
        If Not sf.CheckSubTotals3(P1Row2Col1.Text.ToString, P1Row2Col2.Text.ToString, P1Row2Col3.Text.ToString, P1Row2Col4.Text.ToString, P1Row2Col4) Then
            SubTotalsOK = False
        End If

        'Variable annuities - Total NFP
        'Only perform horizontal validation if cell passed vertical validation
        If P1Row3Col4.CssClass = CSSCLASS_VALIDATED Then
            If Not sf.CheckSubTotals3(P1Row3Col1.Text.ToString, P1Row3Col2.Text.ToString, P1Row3Col3.Text.ToString, P1Row3Col4.Text.ToString, P1Row3Col4) Then
                SubTotalsOK = False
            End If
        End If

        'Fixed individual annuities - Total NFP
        If Not sf.CheckSubTotals3(P1Row4Col1.Text.ToString, P1Row4Col2.Text.ToString, P1Row4Col3.Text.ToString, P1Row4Col4.Text.ToString, P1Row4Col4) Then
            SubTotalsOK = False
        End If

        'GIA - Total NFP
        If Not sf.CheckSubTotals3(P1Row5Col1.Text.ToString, P1Row5Col2.Text.ToString, P1Row5Col3.Text.ToString, P1Row5Col4.Text.ToString, P1Row5Col4) Then
            SubTotalsOK = False
        End If

        'SIA - Total NFP
        If Not sf.CheckSubTotals3(P1Row6Col1.Text.ToString, P1Row6Col2.Text.ToString, P1Row6Col3.Text.ToString, P1Row6Col4.Text.ToString, P1Row6Col4) Then
            SubTotalsOK = False
        End If

        'Total Group annuity contract = Total NFP
        'Only perform horizontal validation if cell passed vertical validation
        If P1Row7Col4.CssClass = CSSCLASS_VALIDATED Then
            If Not sf.CheckSubTotals3(P1Row7Col1.Text.ToString, P1Row7Col2.Text.ToString, P1Row7Col3.Text.ToString, P1Row7Col4.Text.ToString, P1Row7Col4) Then
                SubTotalsOK = False
            End If
        End If

        'Group annuity contracts - Total NFP
        If Not sf.CheckSubTotals3(P1Row8Col1.Text.ToString, P1Row8Col2.Text.ToString, P1Row8Col3.Text.ToString, P1Row8Col4.Text.ToString, P1Row8Col4) Then
            SubTotalsOK = False
        End If

        'Mutual Funds - Total NFP
        If Not sf.CheckSubTotals3(P1Row9Col1.Text.ToString, P1Row9Col2.Text.ToString, P1Row9Col3.Text.ToString, P1Row9Col4.Text.ToString, P1Row9Col4) Then
            SubTotalsOK = False
        End If

        'Total Contributions - Total NFP
        'Only perform horizontal validation if cell passed vertical validation
        If P1Row10Col4.CssClass = CSSCLASS_VALIDATED Then
            If Not sf.CheckSubTotals3(P1Row10Col1.Text.ToString, P1Row10Col2.Text.ToString, P1Row10Col3.Text.ToString, P1Row10Col4.Text.ToString, P1Row10Col4) Then
                SubTotalsOK = False
            End If
        End If

        'Full Service Assets - Total NFP
        If Not sf.CheckSubTotals3(P1Row11Col1.Text.ToString, P1Row11Col2.Text.ToString, P1Row11Col3.Text.ToString, P1Row11Col4.Text.ToString, P1Row11Col4) Then
            SubTotalsOK = False
        End If

        'Administrative Assets - Total NFP
        If Not sf.CheckSubTotals3(P1Row12Col1.Text.ToString, P1Row12Col2.Text.ToString, P1Row12Col3.Text.ToString, P1Row12Col4.Text.ToString, P1Row12Col4) Then
            SubTotalsOK = False
        End If

        'Investment only - Total NFP
        If Not sf.CheckSubTotals3(P1Row13Col1.Text.ToString, P1Row13Col2.Text.ToString, P1Row13Col3.Text.ToString, P1Row13Col4.Text.ToString, P1Row13Col4) Then
            SubTotalsOK = False
        End If

        'Total Assets - Total NFP
        'Only perform horizontal validation if cell passed vertical validation
        If P1Row14Col4.CssClass = CSSCLASS_VALIDATED Then
            If Not sf.CheckSubTotals3(P1Row14Col1.Text.ToString, P1Row14Col2.Text.ToString, P1Row14Col3.Text.ToString, P1Row14Col4.Text.ToString, P1Row14Col4) Then
                SubTotalsOK = False
            End If
        End If

        'Participants new for the qtr - Toal NPF
        If Not sf.CheckSubTotals3(P1Row15Col1.Text.ToString, P1Row15Col2.Text.ToString, P1Row15Col3.Text.ToString, P1Row15Col4.Text.ToString, P1Row15Col4) Then
            SubTotalsOK = False
        End If

        'Total Participants - Total NPF
        If Not sf.CheckSubTotals3(P1Row16Col1.Text.ToString, P1Row16Col2.Text.ToString, P1Row16Col3.Text.ToString, P1Row16Col4.Text.ToString, P1Row16Col4) Then
            SubTotalsOK = False
        End If





       
        If Not SubTotalsOK Then
            Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext()
            Dim StatusQuery = (From w In db.dWorkbooks _
                               Select w _
                               Where (w.WorkbookID = CType(ViewState("WorkbookID"), Integer))).Single
            StatusQuery.WorkbookStatus = "F"
            db.SubmitChanges()
        End If


        ''Mark the workbook pass/fail
        'Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext()
        'Dim StatusQuery = (From w In db.dWorkbooks _
        '                   Select w _
        '                   Where (w.WorkbookID = CType(ViewState("WorkbookID"), Integer))).Single
        'If Not SubTotalsOK Then
        '    StatusQuery.WorkbookStatus = "F"
        'Else
        '    StatusQuery.WorkbookStatus = "P"
        'End If
        'db.SubmitChanges()

        Return SubTotalsOK

    End Function

    'Runs ValidateForm, which checks each textbox for allowable values, also checks totals (not programmed yet)
    'loops through the controls, and when it finds a textbox, retrieves the DataField value stored in a Viewstate
    'variable with a key the same name as the textbox id, and runs a query based on the DataID
    'Sets the value of Number based on a couple of rules: if empty string, then 0; if M then null
    'Finally, we submit the changes to the database, and return to the previous screen
    Protected Sub btnUpdate_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnUpdate.Click, btnUpdate2.Click


        If Not ValidateForm() Then

            displayMessageBox("Invalid data has been entered.  Please correct.")
        Else
            Dim currentView As View = New View
            Dim pageNo As Integer = ViewState("PageNumber")
            If pageNo = 1 Then
                currentView = vwEdit
            ElseIf pageNo = 2 Then
                currentView = vwEdit2
            End If


            'save data to db
            Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext()
            For Each thisControl As Control In currentView.Controls
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
                                'converts the value to a double
                                query.Number = CType(thisTextBox.Text, Double)
                        End Select
                    Case Else
                End Select
            Next
            db.SubmitChanges()



            If ValidateTotals() Then

                ClearEditForm(currentView)
                MultiView1.SetActiveView(vwDefault)
                SetPageTitle(Session("SurveyName"))
            Else
                displayMessageBox("The Workbook Status has been set to F.  Please rerun SubTotal Validate to validate the entire workbook")
            End If


        End If

    End Sub

    'validates the data in the form - checks totals, etc.
    Protected Function ValidateForm() As Boolean
        Dim FormDataOK As Boolean = True
        Dim NumberData As Double
        For Each thisControl As Control In vwEdit.Controls
            Select Case thisControl.GetType.ToString
                Case "System.Web.UI.WebControls.TextBox"
                    Dim thisTextBox As TextBox = New TextBox
                    thisTextBox = CType(thisControl, TextBox)
                    thisTextBox.CssClass = "TextRight"
                    If thisTextBox.Text = "" OrElse thisTextBox.Text = "M" OrElse Double.TryParse(thisTextBox.Text, NumberData) Then
                    Else
                        FormDataOK = False
                        thisTextBox.CssClass = "TextRight Red"
                    End If

                Case Else
            End Select
        Next
        Return FormDataOK
    End Function

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click, btnCancel2.Click
        Dim pageNo As Integer = ViewState("PageNumber")
        If pageNo = 1 Then
            ClearEditForm(vwEdit)
        ElseIf pageNo = 2 Then
            ClearEditForm(vwEdit2)
        End If

        MultiView1.SetActiveView(vwDefault)
        SetPageTitle(Session("SurveyName"))
    End Sub
End Class