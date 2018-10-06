

Imports System.Data.SqlClient
Imports System.Data
Imports System.Configuration
Imports System.Diagnostics

Imports Telerik.Web.UI
Imports Telerik.Web.UI.Upload

Imports SalesSurveysApplication.Globals   'located in Classes folder
Imports SalesSurveysApplication.Projects  'located in Classes folder


Partial Public Class WorkflowImport
    Inherits System.Web.UI.UserControl

    Protected gF As Globals.GlobalFunctions = New Globals.GlobalFunctions
    Protected gV As Globals.GlobalVariables = New Globals.GlobalVariables
    Protected pF As Projects.ProjectFunctions = New Projects.ProjectFunctions

    Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext()



    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Page.User.IsInRole("SuperAdmin") OrElse Page.User.IsInRole("Admin") OrElse _
           Page.User.IsInRole("SuperAnalyst") OrElse Page.User.IsInRole("Analyst") Then

            MultiView1.SetActiveView(vwDefault)

            Dim sMessage As String = ""
            If Not Page.IsPostBack Then
                Try
                    SetPageTitle(Session("SurveyName"))
                    GetFileLists()
                Catch ex As Exception
                    gF.NotifyWebmaster(ex, Request.ServerVariables.Item("PATH_INFO"), "Import PageLoad Complete Error")
                    sMessage = "Import PageLoad Complete Error"
                    displayMessageBox(sMessage)
                End Try
            End If
           
        Else
            MultiView1.SetActiveView(vwNotAuthorized)
        End If

    End Sub

    Protected Sub btnOpenLeft_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnOpenLeft.Click

        Dim sPath As String
        Dim sFilePath As String
        Dim sMessage As String = ""

        Try
            If Source.SelectedValue = Nothing Then
                Literal1.Text = "No Workbook Selected!"
            Else
                Literal1.Text = " "
                sPath = pF.getSurveyFilePath(CType(Session("SurveyID"), Integer))
                sFilePath = sPath & "Data\2-Validating\" & Source.SelectedValue
                openAWorkbook(sFilePath)
            End If
        Catch ex As Exception
            gF.NotifyWebmaster(ex, Request.ServerVariables.Item("PATH_INFO"), "Import Open Left Click Error")
            sMessage = "Import Open Left Click Error"
            displayMessageBox(sMessage)
        End Try

    End Sub

    Protected Sub btnMove_Click1(ByVal sender As Object, ByVal e As EventArgs) Handles btnMove.Click

        Dim sMessage As String = ""
        Literal1.Text = ""

        Try
            If Source.SelectedValue = Nothing Then
                Literal1.Text = "No Workbook Selected!"
            Else
                Literal1.Text = " "
                uploadWorkbook()
                GetFileLists()
            End If
        Catch ex As Exception
            gF.NotifyWebmaster(ex, Request.ServerVariables.Item("PATH_INFO"), "Import Move Button Click Error")
            sMessage = "Import Move Button Click Error"
            displayMessageBox(sMessage)
        End Try
    End Sub


    Private Sub SetPageTitle(ByVal TitleText As String)

        If Me.Page.Form.Parent IsNot Nothing Then
            Dim litPageTitle As Literal = Me.Page.Form.Parent.FindControl("litPageTitle")

            If litPageTitle IsNot Nothing Then
                litPageTitle.Text = TitleText
            End If
        End If

    End Sub

    Protected Function validateWorkbook( _
        ByRef oxlWB As SpreadsheetGear.IWorkbook, ByRef progress As RadProgressContext) As Boolean

        'Validate workbook

        Dim iprogress As Integer = progress.PrimaryValue
        Dim iprogressTotal As Integer = progress.PrimaryTotal

        Dim oxlWS As SpreadsheetGear.IWorksheet

        Dim iRow As Integer
        Dim iCol As Integer
        Dim sPath As String = ""
        Dim sFilePath As String = ""
        Dim tFilePath As String = ""
        Dim sMessage As String = ""

        validateWorkbook = False

        Try

            Dim oDD = pF.getDataDictionary(CType(Session("SurveyID").ToString, Integer))

            If Not oDD Is Nothing Then


                oxlWS = oxlWB.Worksheets(1)

                'Validate numeric data assumes standard exception values 'M' or <null>
                'Dim Fields = From Field In oDD _
                '             Select Field.SurveyID, Field.Field, Field.xlSheet, Field.xlRow, Field.xlColumn, Field.DataType, Field.SortKey _
                '             Where SurveyID = CType(Session("SurveyID"), Integer) _
                '             Order By SortKey Ascending

                For Each Field In oDD
                    oxlWS = oxlWB.Worksheets(Field.xlSheet.ToString)
                    iRow = Field.xlRow
                    iCol = Field.xlColumn

                    Select Case Field.DataType
                        Case "Number"
                            'Note SpreadsheetGear uses zero(0) base
                            If Len(oxlWS.Cells(iRow - 1, iCol - 1).Value) = 0 Then
                                oxlWS.Cells(iRow - 1, iCol - 1).Interior.Color = System.Drawing.Color.LightGreen  'set to light green
                            ElseIf IsNumeric(oxlWS.Cells(iRow - 1, iCol - 1).Value) Then
                                oxlWS.Cells(iRow - 1, iCol - 1).Interior.Color = System.Drawing.Color.LightGreen  'set to light green
                            ElseIf oxlWS.Cells(iRow - 1, iCol - 1).Value.ToString = "M" Then
                                oxlWS.Cells(iRow - 1, iCol - 1).Interior.Color = System.Drawing.Color.LightGreen  'set to light green
                            Else
                                oxlWS.Cells(iRow - 1, iCol - 1).Interior.Color = System.Drawing.Color.LightYellow  'set to light yellow
                                sMessage = sMessage & "Worksheet:" & Field.xlSheet & " Row:" & Field.xlRow & " Column:" & Field.xlColumn & vbCrLf
                            End If
                        Case "Date"
                            If Len(oxlWS.Cells(iRow - 1, iCol - 1).Value) = 0 Then
                                oxlWS.Cells(iRow - 1, iCol - 1).Interior.Color = System.Drawing.Color.LightGreen  'set to light green
                            ElseIf IsDate(oxlWB.NumberToDateTime(oxlWS.Cells(iRow - 1, iCol - 1).Value)) Then
                                oxlWS.Cells(iRow - 1, iCol - 1).Interior.Color = System.Drawing.Color.LightGreen  'set to light green
                            ElseIf oxlWS.Cells(iRow - 1, iCol - 1).Value.ToString = "M" Then
                                oxlWS.Cells(iRow - 1, iCol - 1).Interior.Color = System.Drawing.Color.LightGreen  'set to light green
                            Else
                                oxlWS.Cells(iRow - 1, iCol - 1).Interior.Color = System.Drawing.Color.LightYellow  'set to light yellow
                                sMessage = sMessage & "Worksheet:" & Field.xlSheet & " Row:" & Field.xlRow & " Column:" & Field.xlColumn & vbCrLf
                            End If
                    End Select

                    'iprogress = iprogress + 1
                    'progress.PrimaryValue = iprogress
                    'progress.PrimaryPercent = 100 * iprogress / iprogressTotal

                Next


                Select Case Len(sMessage)
                    Case 0
                        validateWorkbook = True
                        displayMessageBox("Workbook Validated.")
                    Case 1 To 512
                        validateWorkbook = False
                        displayMessageBox(sMessage)

                    Case Is > 512
                        validateWorkbook = False
                        sMessage = "Too many validation errors to list!"
                        displayMessageBox(sMessage)
                End Select

                'displayMessageBox(sMessage)
                Literal1.Text = sMessage
            Else
                Throw New System.Exception("getDataDictionary Error for Survey " & Session("SurveyID").ToString & " - Workflow Import")
            End If
        Catch ex As Exception
            gF.NotifyWebmaster(ex, Request.ServerVariables.Item("PATH_INFO"), "Import Validate Workbook Error")
            sMessage = "Import Validate Workbook Error"
            displayMessageBox(sMessage)
        End Try

    End Function

    Protected Sub uploadWorkbook()

        Dim progress As RadProgressContext = RadProgressContext.Current
        Dim iprogress As Integer = 0
        Dim iprogressTotal As Integer = 0

        Dim oxlWB As SpreadsheetGear.IWorkbook
        Dim oxlWS As SpreadsheetGear.IWorksheet

        Dim oDD = pF.getDataDictionary(CType(Session("SurveyID").ToString, Integer))

        Dim sPath As String = ""
        Dim sFilePath As String = ""
        Dim tFilePath As String = ""
        Dim sMessage As String = ""

        Dim sData As String = ""
        Dim sValue As String = ""
        Dim dtDateID As Date
        Dim iRow As Integer
        Dim iCol As Integer
        Dim iSurveyID As Integer
        Dim iSurveySeriesID As Integer
        ' Dim iRespondentID As Integer
        Dim iSourceID As Integer
        Dim iWorkbookID As Integer

        Try

            iprogressTotal = 2 * oDD.Length() + 3000
            'progress.PrimaryTotal = iprogressTotal 'assumes oDD.Count>0

            progress.PrimaryTotal = iprogressTotal
            progress.PrimaryValue = 2
            progress.PrimaryPercent = 2
           

            sPath = pF.getSurveyFilePath(CType(Session("SurveyID"), Integer))
            sFilePath = sPath & "Data\2-Validating\" & Source.SelectedValue
            tFilePath = sPath & "Data\3-Completed\" & Source.SelectedValue

            oxlWB = SpreadsheetGear.Factory.GetWorkbook(sFilePath)
            oxlWS = oxlWB.Worksheets(1)

            For Each sField In oDD
                oxlWS = oxlWB.Worksheets(sField.xlSheet.ToString)
                iRow = sField.xlRow
                iCol = sField.xlColumn
                Select Case sField.Field
                    Case "SurveyID"
                        iSurveyID = oxlWS.Cells(iRow - 1, iCol - 1).Value
                    Case "DateID"
                        dtDateID = oxlWB.NumberToDateTime(oxlWS.Cells(iRow - 1, iCol - 1).Value)
                    Case "SourceID"
                        iSourceID = oxlWS.Cells(iRow - 1, iCol - 1).Value
                        'Case "RespondentID"
                        '    iRespondentID = oxlWS.Cells(iRow - 1, iCol - 1).Value
                End Select

                iprogress = iprogress + 1
                progress.PrimaryValue = iprogress
                progress.PrimaryPercent = 100 * iprogress / iprogressTotal
            Next


            'verify workbook not already in Database
            If pF.isWorkbookInDatabase(oxlWB.Name, dtDateID) Then
                displayMessageBox("A workbook with this name and Date is already in Database:" & oxlWB.Name)
                Literal1.Text = "A workbook with this name and Date is already in Database:" & oxlWB.Name
                oxlWB.Worksheets("Cover").Select()
                oxlWB.Close()
                GoTo CleanupAndExit
            End If

            'verify RespondentID exists
            'If Not pF.verifyRespondentID(iRespondentID) Then
            '    displayMessageBox("RespondentID not found:" & iRespondentID)
            '    Literal1.Text = "RespondentID not found:" & iRespondentID
            '    oxlWB.Worksheets("Cover").Select()
            '    oxlWB.Close()
            '    GoTo CleanupAndExit
            'End If

            If Not pF.verifySourceID(iSourceID) Then
                displayMessageBox("Source ID not found:" & iSourceID)
                Literal1.Text = "Source ID not found:" & iSourceID
                oxlWB.Worksheets("Cover").Select()
                oxlWB.Close()
                GoTo CleanupAndExit
            End If

            'verify SurveySeries exists
            iSurveySeriesID = pF.getSurveySeriesID(iSurveyID, dtDateID)
            If iSurveySeriesID = 0 Then
                displayMessageBox("Survey Date not found in DataBase")
                Literal1.Text = "Survey Date not found in DataBase"
                oxlWB.Worksheets("Cover").Select()
                oxlWB.Close()
                GoTo CleanupAndExit
            End If


            'Check number MORs have value zero(0) or higher
            Dim queryMORs = From qMOR In oDD _
                Select qMOR.SurveyID, qMOR.Field, qMOR.xlSheet, qMOR.xlRow, qMOR.xlColumn, qMOR.SortKey, qMOR.Description _
                       Where (SurveyID = CType(Session("SurveyID"), Integer) _
                              And Description.Contains("Method of Reporting")) _
                       Order By SortKey Ascending



            If queryMORs.Count > 0 Then
                For Each qMOR In queryMORs
                    oxlWS = oxlWB.Worksheets(qMOR.xlSheet.ToString)
                    iRow = qMOR.xlRow
                    iCol = qMOR.xlColumn
                    If Len(" " & oxlWS.Cells(iRow - 1, iCol - 1).Value) = 1 _
                        OrElse (Not IsNumeric(oxlWS.Cells(iRow - 1, iCol - 1).Value)) _
                        OrElse oxlWS.Cells(iRow - 1, iCol - 1).Value < 0 Then
                        displayMessageBox("Survey has invalid MOR.")
                        Literal1.Text = "Survey has invalid MOR."
                        oxlWB.Worksheets("Cover").Select()
                        oxlWB.Close()
                        GoTo CleanupAndExit
                    End If
                Next
            Else
                displayMessageBox("Survey has missing MOR.")
                Literal1.Text = "Survey has missing MOR."
                oxlWB.Worksheets("Cover").Select()
                oxlWB.Close()
                GoTo CleanupAndExit
            End If

            'verify number and date fields have only numeric or date values
            If Not validateWorkbook(oxlWB, progress) Then
                'displayMessageBox("Workbook still contains invalid data see cells shaded yellow")
                Literal1.Text = "Workbook still contains invalid data see cells shaded yellow"
                oxlWB.Worksheets("Cover").Select()
                oxlWB.Close()
                GoTo CleanupAndExit
            End If

            'Dim Fields = From Field In oDD _
            '    Select Field.SurveyID, Field.Field, Field.xlSheet, Field.xlRow, Field.xlColumn, Field.SortKey _
            '           Where SurveyID = CType(Session("SurveyID"), Integer) _
            '           Order By SortKey Ascending

            iprogress = progress.PrimaryValue
            For Each Field In oDD
                oxlWS = oxlWB.Worksheets(Field.xlSheet.ToString)
                iRow = Field.xlRow
                iCol = Field.xlColumn

                'Exception error if oxlWS.Cells(iRow-1, iCol-1).Value.ToString is null
                If Len(" " & oxlWS.Cells(iRow - 1, iCol - 1).Value) = 1 Then
                    sValue = ""
                Else
                    sValue = oxlWS.Cells(iRow - 1, iCol - 1).Value.ToString
                End If
                sData = sData & sValue & vbTab

                iprogress = iprogress + 1
                progress.PrimaryValue = iprogress
                progress.PrimaryPercent = 100 * iprogress / iprogressTotal

            Next
            sData = sData & Now().ToString  'add timestamp to end


            'Upload Data and set WorkbookStatus to A for Added.
            Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext()



            'Dim dSubmission As New dWorkbook With { _
            '                                        .SurveySeriesID = iSurveySeriesID, _
            '                                        .RespondentID = iRespondentID, _
            '                                        .DateID = dtDateID, _
            '                                        .WorkbookName = oxlWB.Name, _
            '                                        .WorkbookStatus = "A", _
            '                                        .Responses = sData _
            '                                      }

            Dim dSubmission As New dWorkbook With { _
                                                  .SurveySeriesID = iSurveySeriesID, _
                                                  .SourceID = iSourceID, _
                                                  .DateID = dtDateID, _
                                                  .WorkbookName = oxlWB.Name, _
                                                  .WorkbookStatus = "A", _
                                                  .Responses = sData _
                                                }
            db.dWorkbooks.InsertOnSubmit(dSubmission)
            db.SubmitChanges()

            'Save new WorkbookID for use later
            iWorkbookID = dSubmission.WorkbookID

            'Copy sData to fData
            CopySubmissionTofData(sData, iSurveySeriesID, iWorkbookID, progress)

            dSubmission = Nothing
            db = Nothing

            oxlWB.Worksheets("Cover").Select()
            oxlWB.Close()
            gF.moveFile(sFilePath, tFilePath)

            Literal1.Text = "Data uploaded successfully."

            'displayMessageBox(UBound(Split(sData, vbTab)))
            'displayMessageBox(Left(sData, 30) & Right(sData, 15))

CleanupAndExit:


            oxlWB = Nothing
            oxlWS = Nothing

            progress.PrimaryValue = iprogressTotal
            progress.PrimaryPercent = 100

        Catch ex As Exception

            gF.NotifyWebmaster(ex, Request.ServerVariables.Item("PATH_INFO"), "Import Upload Workbook Error")
            sMessage = "Import Upload Workbook Error"
            displayMessageBox(sMessage)
        End Try

    End Sub

    Protected Sub openAWorkbook(ByVal sFilePath As String)
        Dim sMessage As String = ""

        Try
            sFilePath = "File:" & sFilePath
            Response.Redirect(sFilePath, False)
        Catch ex As Exception
            gF.NotifyWebmaster(ex, Request.ServerVariables.Item("PATH_INFO"), "Import Open A Workbook Error")
            sMessage = "Import Open A Workbook Error"
            displayMessageBox(sMessage)
        End Try

    End Sub

    Protected Sub GetFileLists()

        Dim sMessage As String = ""

        Try
            Dim myFileInfo As IEnumerable(Of String)
            Dim sPath As String = ""

            sPath = pF.getSurveyFilePath(CType(Session("SurveyID"), Integer))
            myFileInfo = gF.getXLWBDirectory(sPath & "Data\2-Validating\")
            Source.DataSource = myFileInfo.ToList()
            Source.DataBind()

            myFileInfo = gF.getXLWBDirectory(sPath & "Data\3-Completed\")
            Target.DataSource = myFileInfo.ToList()
            Target.DataBind()

        Catch ex As Exception
            gF.NotifyWebmaster(ex, Request.ServerVariables.Item("PATH_INFO"), "Import Get File Lists Error")
            sMessage = "Import Get File Lists Error"
            displayMessageBox(sMessage)
        End Try

    End Sub

    Protected Sub CopySubmissionTofData(ByRef aSubmission As String, ByRef iSeriesID As Integer, ByRef iWorkbookID As Integer, progress As RadProgressContext)
        Dim sMessage As String = ""
        Try
            Dim nPages As Integer
            Dim nMORs As Integer
            Dim arrayData() As String

            'amount by which to increment progrees bar
            Dim iProgressIncrement As Integer = 1000

            Dim dd = pF.getDataDictionary(CType(Session("SurveyID").ToString, Integer))


            '############# The new getDataDictionary returns an array sorted by sortkey - Ascending is the default value for sort
            'Dim Fs = (From fld In dd _
            '          Where fld.SurveyID = CType(Session("SurveyID"), Integer) _
            '          Order By fld.SortKey Ascending).ToArray

            Dim fLs = (From fL In db.fLayouts Where fL.SurveyID = CType(Session("SurveyID"), Integer) And fL.Inactive Is Nothing).ToArray

            'Implicit inner join
            ' to access the value in a field for a given record here is a syntax example: fDFs(0).dF.SortKey
            Dim fDFs = From dF In dd, lF In fLs Where dF.FieldID = lF.FieldID Order By dF.SortKey

            '########### The new getDataDictionary is already filtered by surveyID
            'Dim queryMORs = From fld In dd _
            '                Select fld.FieldID, fld.SurveyID, fld.SortKey, fld.Description _
            '                Order By SortKey Ascending _
            '                Where (SurveyID = CType(Session("SurveyID"), Integer) _
            '                      And Description.StartsWith("Method of Reporting"))

            Dim queryMORs = From fld In dd _
                           Select fld.FieldID, fld.SurveyID, fld.SortKey, fld.Description _
                           Order By SortKey Ascending _
                           Where Description.Contains("Method of Reporting")

            Dim MORs = (From sk In queryMORs Select sk.SortKey Order By SortKey Ascending).ToArray

            'Update progress bar; assuming PrimaryValue is 300 less than PrimaryTotal
            With progress
                .PrimaryValue += iProgressIncrement
                .PrimaryPercent = 100 * .PrimaryValue / .PrimaryTotal
            End With

            'Check number MORs equal 1 or number of fPages(worksheets or pages in survey)
            nPages = (From p In db.fPages Where p.SurveyID = CType(Session("SurveyID"), Integer)).Count
            nMORs = 1 + UBound(MORs)
            If (nMORs <> 1) And (nMORs <> nPages) Then
                displayMessageBox("The number of MORs does not equal 1 or number of worksheets or pages in survey")
                Return
            End If

            'Split(aSubmission.Responses,vbTab) will convert responses to string array
            arrayData = Split(aSubmission, vbTab) 'remember last entry is timestamp and not mapped to fData


            'copy arraysData to fData table
            Dim fD As fData
            Dim fDList As New List(Of fData)
            db.fDatas.InsertAllOnSubmit(fDList)
            For Each fld In fDFs
                fD = New fData
                fD.SurveySeriesID = iSeriesID
                fD.WorkbookID = iWorkbookID
                fD.FieldID = fld.dF.FieldID
                fD.Value = arrayData(fld.dF.SortKey - 1) ' Remember arrays are base zero sortkey starts with 1.
                Select Case fld.dF.DataType
                    Case "Number"
                        If Len(fD.Value) = 0 Then
                            fD.Number = 0 'convert nulls which mean not applicable to zero(0)
                        ElseIf IsNumeric(fD.Value) Then
                            fD.Number = CType(fD.Value, Double) 'convert #'s to actual number
                        ElseIf Trim(fD.Value) = "M" Then
                            fD.Number = Nothing  'convert missing data to null
                        Else
                            'sMessage = sMessage TBD
                        End If
                    Case "Date"
                        'Dates are not converted but left as string value
                    Case Else
                        'All other data types are not converted
                End Select

                Select Case nMORs
                    Case 1
                        ' Change by Hetal: To solve the MOR issue for NFP. 09/18/2011
                        ' fD.MOR = arrayData(MORs(0)) ' Remember arrays are base zero
                        fD.MOR = arrayData(MORs(0) - 1) ' Remember arrays are base zero
                        ' End Change
                    Case Is > 1
                        fD.MOR = arrayData(MORs(fld.lF.fPage.PageNumber - 1) - 1) ' Remember arrays are base zero page number starts with 1.
                End Select
                fDList.Add(fD)

                'Update progress bar
                With progress
                    .PrimaryValue += 100
                    .PrimaryPercent = 100 * .PrimaryValue / .PrimaryTotal
                End With
            Next



            db.fDatas.InsertAllOnSubmit(fDList)
            db.SubmitChanges()

            'Update progress bar
            With progress
                .PrimaryValue += iProgressIncrement
                .PrimaryPercent = 100 * .PrimaryValue / .PrimaryTotal
            End With

            dd = Nothing
            fLs = Nothing
            fDFs = Nothing
            queryMORs = Nothing
            MORs = Nothing
            fD = Nothing
            fDList = Nothing

        Catch ex As Exception
            gF.NotifyWebmaster(ex, Request.ServerVariables.Item("PATH_INFO"), "Import CopySubmissionTofData Error")
            sMessage = "Import CopySubmissionTofData Error"
            Throw New Exception(sMessage)
            displayMessageBox(sMessage)
        End Try

    End Sub

    Private Sub displayMessageBox(ByVal msg As String)

        Page.ClientScript.RegisterStartupScript(Me.GetType(), "AlertMessage", "alert('" + msg + "');", True)

    End Sub


End Class