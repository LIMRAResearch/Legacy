Imports Telerik.Web.UI
Imports System.Threading

Public Class WorkflowBatchProcess
    Inherits System.Web.UI.UserControl
    Protected pF As Projects.ProjectFunctions = New Projects.ProjectFunctions
    Protected gF As Globals.GlobalFunctions = New Globals.GlobalFunctions
    Private m_dtDateId As Date
    Private m_iSurveyId, m_iSourceId As Integer
    Private m_lstMORs As List(Of Double?)
    Private m_lstData As List(Of String)
    Private m_db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext()
    'get data dictionary from db
    Private m_DD As Array
    Private m_bImported As Boolean = False
    Private Const INPROC_STATUS As String = "In Process"


    Protected Delegate Sub AsyncTaskDelegate()
    'TODO: RP Wrap import error text in Target listbox
    Protected Enum ListType
        [New] = 1
        Input = 2
        [Error] = 3
    End Enum

    Protected Enum ListStatus
        InProc = 1
        Complete = 2
    End Enum

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


        If Page.User.IsInRole("SuperAdmin") OrElse Page.User.IsInRole("Admin") OrElse _
           Page.User.IsInRole("SuperAnalyst") OrElse Page.User.IsInRole("Analyst") Then
            MultiView1.SetActiveView(vwDefault)

            If Not Page.IsPostBack Then
                Dim sMessage As String = ""

                hdnScollTop.Value = "0"

                Try
                    SetPageTitle(Session("SurveyName"))
                    getFileLists(ListType.New)

                    


                Catch ex As Exception
                    gF.NotifyWebmaster(ex, Request.ServerVariables.Item("PATH_INFO"), "Format Page Load Error")
                End Try
            Else
                'fires javascript function to maintain scroll position
                Source.Focus()
            End If



            'javascript functions used to maintain list box scroll position on postback
            Source.Attributes.Add("onclick", "GetListBoxScrollPosition();")
            Source.Attributes.Add("onfocus", "SetListBoxScrollPosition();")
        Else
            MultiView1.SetActiveView(vwNotAuthorized)
        End If
    End Sub

    Protected Sub setWbkStatus(wbks As IEnumerable(Of String), sStatus As String, ByRef dctImportResults As Dictionary(Of String, String))

        Dim ImportDetails = (From id In m_db.ImportDetails
                                                       Where wbks.Contains(id.WorkbookName))

        For Each sWbkName In wbks

            Dim sWbk As String = sWbkName

            'check whether wkbk failed previously
            Dim detail = (From det In ImportDetails
                          Where det.WorkbookName = sWbk).FirstOrDefault

            'if failed previously update existing record
            If Not detail Is Nothing Then
                detail.ImportDate = Now
                detail.ImportResult = sStatus

                'otherwise add import detail
            Else
                Dim id As New ImportDetail With {.ImportDate = Now.ToString("G"),
                                                 .ImportResult = sStatus,
                                                 .WorkbookName = sWbkName,
                                                 .SurveyId = CType(Session("SurveyID"), Integer)
                                                }
                m_db.ImportDetails.InsertOnSubmit(id)
            End If

            dctImportResults.Add(sWbkName, sStatus)
        Next

        m_db.SubmitChanges()

    End Sub

    Protected Sub btnImport_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnImport.Click


        'ImportWorkbooks()

        'create the asynchronous task instance  
        Dim asyncTask As PageAsyncTask = New PageAsyncTask(
          AddressOf OnBegin,
          AddressOf OnEnd,
          AddressOf OnTimeout,
          Nothing)



        'getFileLists(ListType.New)

        spnImportStatus.InnerText = "Import Status"
        'Target.Height = Unit.Parse("300px")
        Target.Items.Clear()

        'register the asynchronous task instance with the page  
        Page.RegisterAsyncTask(asyncTask)


        Page.ExecuteRegisteredAsyncTasks()

    End Sub

    Protected Function OnBegin(sender As Object, e As EventArgs, callback As AsyncCallback, state As Object)

        Dim dlgt As New AsyncTaskDelegate(AddressOf ImportWorkbooks)
        Dim result As IAsyncResult = dlgt.BeginInvoke(callback, state)
        Return result

    End Function

    Protected Sub OnEnd(ar As IAsyncResult)
        'getFileLists(ListType.New)
        displayMessageBox("Batch Process completed")
        getFileStatus(ListStatus.InProc)
    End Sub

    Protected Sub OnTimeout(ar As IAsyncResult)
        'getFileLists(ListType.New)
        displayMessageBox("Batch Process started")
        getFileStatus(ListStatus.InProc)
    End Sub

    Protected Sub ImportWorkbooks()
        Dim sFilePath As String = String.Empty
        Dim sDir As String
        Dim wbks As IEnumerable(Of String)

        Dim wbkTemplate, wbkSurvey As SpreadsheetGear.IWorkbook
        Dim iSurveySeriesId As Integer
        Dim dctImportResults As Dictionary(Of String, String) = New Dictionary(Of String, String)
        Dim sCurrWkbkName As String = ""
        'Literal1.Text = ""

        Try
            'Thread.Sleep(TimeSpan.FromSeconds(20.0))
            sDir = pF.getSurveyFilePath(CType(Session("SurveyID"), Integer))
            m_DD = pF.getDataDictionary(CType(Session("SurveyID"), Integer))
            m_iSurveyId = CType(Session("SurveyID"), Integer)

            If Not m_DD Is Nothing Then

                Dim sSrcDir As String = sDir & "Data\0-Submitted\"
                Dim sTrgDir As String = sDir & "Data\3-Completed\"
                Dim sErrDir As String = sDir & "Data\2-Validating\"
                Dim sWorkDir As String = sDir & "Data\BatchProcess\"


                wbks = From item As ListItem In Source.Items
                       Where item.Selected
                       Select item.Text



                If wbks.Count > 0 Then
                    wbkTemplate = SpreadsheetGear.Factory.GetWorkbook(sDir & "Data\8-Templates\CurrentSurvey.xls")

                    'sets import result (in db) to In Process for each selected wbk 
                    setWbkStatus(wbks, INPROC_STATUS, dctImportResults)
                End If


                'create BatchProcess if necessary
                If (Not System.IO.Directory.Exists(sWorkDir)) Then
                    System.IO.Directory.CreateDirectory(sWorkDir)
                End If

                For Each sWbkName In wbks

                    Try
                        'all changes made to copy of survey in batchprocess dir
                        'create copy if necessary   
                        If Not System.IO.File.Exists(sWorkDir & sWbkName) Then
                            copyWorkbook(sSrcDir, sWorkDir, sWbkName, dctImportResults)

                            If dctImportResults(sWbkName) <> INPROC_STATUS Then
                                GoTo CleanupAndExit
                            End If
                        End If

                        wbkSurvey = SpreadsheetGear.Factory.GetWorkbook(sWorkDir & sWbkName)

                        addMissingWorksheets(wbkTemplate, wbkSurvey, dctImportResults)


                        If dctImportResults(sWbkName) <> INPROC_STATUS Then
                            GoTo CleanupAndExit
                        End If

                        formatWorksheets(wbkSurvey, m_DD, dctImportResults)

                        If dctImportResults(sWbkName) <> INPROC_STATUS Then
                            GoTo CleanupAndExit
                        End If

                        'verify workbook not already in Database
                        If pF.isWorkbookInDatabase(wbkSurvey.Name, m_dtDateId) Then
                            dctImportResults(wbkSurvey.Name) = "Workbook is already in Database"
                            GoTo CleanupAndExit
                        End If

                        If Not pF.verifySourceID(m_iSourceId) Then
                            dctImportResults(wbkSurvey.Name) = "Source ID not found"
                            GoTo CleanupAndExit
                        End If

                        'verify SurveySeries exists
                        iSurveySeriesId = pF.getSurveySeriesID(m_iSurveyId, m_dtDateId)
                        If iSurveySeriesId = 0 Then
                            dctImportResults(wbkSurvey.Name) = "Survey Date not found in Database."
                            GoTo CleanupAndExit
                        End If



                        'verify MORs
                        If m_lstMORs.Count > 0 Then
                            For Each MOR In m_lstMORs
                                If Not MOR.HasValue OrElse MOR < 0 Then
                                    dctImportResults(wbkSurvey.Name) = "Survey has invalid MOR."
                                    GoTo CleanupAndExit
                                End If
                            Next
                        Else
                            dctImportResults(wbkSurvey.Name) = "Survey has missing MOR."
                            GoTo CleanupAndExit
                        End If





                        'Upload Data and set WorkbookStatus to A for Added.
                        Dim dSubmission As New dWorkbook With { _
                                                              .SurveySeriesID = iSurveySeriesId, _
                                                              .SourceID = m_iSourceId, _
                                                              .DateID = m_dtDateId, _
                                                              .WorkbookName = wbkSurvey.Name, _
                                                              .WorkbookStatus = "A", _
                                                              .Responses = String.Join(vbTab, m_lstData.ToArray) _
                                                            }
                        m_db.dWorkbooks.InsertOnSubmit(dSubmission)
                        m_db.SubmitChanges()

                        'load survey data
                        copySubmissionTofData(m_lstData.ToArray, iSurveySeriesId, dSubmission.WorkbookID, wbkSurvey.Name, dctImportResults)
                        If dctImportResults(sWbkName) <> INPROC_STATUS Then
                            GoTo CleanupAndExit
                        End If

                        'survey imported successfully move to completed folder
                        m_bImported = True

CleanupAndExit:
                        If Not wbkSurvey Is Nothing Then

                            Dim ffSurveyFormat As SpreadsheetGear.FileFormat

                            wbkSurvey.Worksheets("Cover").Select()


                            Select Case wbkSurvey.FileFormat
                                Case SpreadsheetGear.FileFormat.Excel8
                                    ffSurveyFormat = SpreadsheetGear.FileFormat.Excel8
                                Case SpreadsheetGear.FileFormat.OpenXMLWorkbook
                                    ffSurveyFormat = SpreadsheetGear.FileFormat.OpenXMLWorkbook
                                Case Else
                                    Throw New Exception("Invalid Excel Format")
                            End Select

                            If m_bImported Then

                                sFilePath = sTrgDir & sWbkName
                                dctImportResults(sWbkName) = "Imported"

                                'save file to completed directory
                                wbkSurvey.SaveAs(sFilePath, ffSurveyFormat)

                                'delete wbk from batchprocess dir
                                DeleteWbk(sWbkName, dctImportResults)

                            Else
                                sFilePath = sWorkDir & sWbkName

                                'resave file to Batch Process directory
                                wbkSurvey.SaveAs(sFilePath, ffSurveyFormat)

                                If dctImportResults(sWbkName) = INPROC_STATUS Then
                                    dctImportResults(sWbkName) = "Unknown Import Error"
                                End If
                            End If

                            'update db
                            updateImportDetail(sWbkName, dctImportResults(sWbkName))

                        End If

                        'reset Imported flag for next wbk
                        m_bImported = False




                    Catch ex As Exception
                        updateImportDetail(sWbkName, ex.Message)

                        gF.NotifyWebmaster(ex, Request.ServerVariables.Item("PATH_INFO"), ex.Message)
                        dctImportResults.Add(sWbkName, "Import Error")
                    End Try

                   
                Next

                If Not wbkSurvey Is Nothing Then
                    wbkSurvey.Close()
                    wbkSurvey = Nothing
                End If

                If Not wbkTemplate Is Nothing Then
                    wbkTemplate.Close()
                    wbkTemplate = Nothing
                End If

                'refresh Import info dictionaries
                getFileLists(ListType.New)

            End If  'm_DD Is  Nothing

        Catch ex As Exception

            gF.NotifyWebmaster(ex, Request.ServerVariables.Item("PATH_INFO"), "Import Error")
            dctImportResults.Add(sCurrWkbkName, "Import  Error")


        Finally

            '********* on postback this block fires after load event **************

            'Dim sWbkName As String = String.Empty
            'Dim sResult As String = String.Empty

            ''get imported wbks
            'Dim Details = (From id In m_db.ImportDetails
            '                         Where dctImportResults.Keys.Contains(id.WorkbookName))

            ''for each imported wbk                  
            'For Each kvp As KeyValuePair(Of String, String) In dctImportResults
            '    sWbkName = kvp.Key
            '    sResult = kvp.Value

            '    'get  import detail from db
            '    Dim detail = (From det In Details
            '                  Where det.WorkbookName = sWbkName).FirstOrDefault

            '    'update existing record
            '    If Not detail Is Nothing Then
            '        detail.ImportDate = Now
            '        If sResult = INPROC_STATUS Then
            '            sResult += " Error"
            '        End If
            '        detail.ImportResult = sResult
            '    End If
            'Next

            'm_db.SubmitChanges()

            If Page.IsPostBack Then
                'updates Import detail dictionaries
                getFileLists(ListType.New)
            End If

        End Try
    End Sub
    Protected Sub copyWorkbook(sSrcDir As String, sTargetDir As String, sWbkName As String, ByRef dctImportErrs As Dictionary(Of String, String))
        Try
            Dim wbkSurvey As SpreadsheetGear.IWorkbook = SpreadsheetGear.Factory.GetWorkbook(sSrcDir & sWbkName)

            Select Case wbkSurvey.FileFormat
                Case SpreadsheetGear.FileFormat.Excel8
                    wbkSurvey.SaveAs(sTargetDir & sWbkName, SpreadsheetGear.FileFormat.Excel8)
                Case SpreadsheetGear.FileFormat.OpenXMLWorkbook
                    wbkSurvey.SaveAs(sTargetDir & sWbkName, SpreadsheetGear.FileFormat.OpenXMLWorkbook)
                Case Else
                    Throw New Exception("Invalid Excel Format")
            End Select
        Catch ex As Exception
            dctImportErrs(sWbkName) = "Unable to create working copy of survey."
        End Try


    End Sub
    Protected Sub addMissingWorksheets(ByRef wbkSurveySource As SpreadsheetGear.IWorkbook,
                                       ByRef wbkSurveyTarget As SpreadsheetGear.IWorkbook,
                                       ByRef dctImportErrs As Dictionary(Of String, String))

        Dim oXLWorksheet As SpreadsheetGear.IWorksheet
        Dim iIndex As Integer = 0
        Dim ltIndex As Integer = 0
        Dim idx As Integer
        Dim lTargetName As String = ""

        Try

            Dim xlWSSource = From xlWBSheet As SpreadsheetGear.IWorksheet In wbkSurveySource.Worksheets Select xlWBSheet.Name
            Dim xlWSTarget = From xlWBSheet As SpreadsheetGear.IWorksheet In wbkSurveyTarget.Worksheets Select xlWBSheet.Name

            '' if intersection is empty, xlNames.Count will be zero(0)
            Dim xlWSsMissing = xlWSSource.Except(xlWSTarget)
            'For I = 0 To xlWSTarget.Count - 1
            '    lTargetName = wbkSurveyTarget.Worksheets(I).Name
            'Next

            'get index of last worksheet in  wkbk
            idx = xlWSTarget.Count - 1
            ltIndex = wbkSurveySource.Worksheets(wbkSurveyTarget.Worksheets(idx).Name).Index

            Dim sbErr As New StringBuilder

            If xlWSsMissing.Count > 0 Then
                sbErr.Append("The following worksheets were missing and added to the workbook: ")
                For Each xlSheet In xlWSsMissing
                    sbErr.Append("<br />" & xlSheet.ToString)
                    iIndex = wbkSurveySource.Worksheets(xlSheet.ToString).Index
                    If iIndex < ltIndex Then
                        oXLWorksheet = wbkSurveySource.Worksheets(xlSheet.ToString).CopyBefore(wbkSurveyTarget.Worksheets(iIndex))
                        oXLWorksheet.Name = xlSheet.ToString
                    Else
                        oXLWorksheet = wbkSurveySource.Worksheets(xlSheet.ToString).CopyAfter(wbkSurveyTarget.Worksheets(iIndex - 1))
                        oXLWorksheet.Name = xlSheet.ToString
                    End If
                Next
                wbkSurveyTarget.Save()
                dctImportErrs(wbkSurveyTarget.Name) = sbErr.ToString
            End If



        Catch ex As Exception
            gF.NotifyWebmaster(ex, Request.ServerVariables.Item("PATH_INFO"), "Add Missing Worksheet Error")
            dctImportErrs(wbkSurveyTarget.Name) = "Add Missing Worksheet Error"
            'displayMessageBox("An error has occurredn (Add Missing Worksheet Error) see systems administrator!")
        End Try

    End Sub

    Protected Sub formatWorksheets(ByRef wbk As SpreadsheetGear.IWorkbook, ByVal DataDict As Array,
                                   ByRef dctImportErrs As Dictionary(Of String, String))

        m_lstData = New List(Of String)
        m_lstMORs = New List(Of Double?)
        Dim sbErrs As New StringBuilder()
        Dim iErrCnt As Integer

        For Each field In DataDict

            Dim wksht As SpreadsheetGear.IWorksheet = wbk.Worksheets(field.xlSheet.ToString)
            Dim iRow As Integer = field.xlRow - 1
            Dim iCol As Integer = field.xlColumn - 1

            If Len(Trim(wksht.Cells(iRow, iCol).Value)) > 0 Then
                ' The workbook may have the character code of 160 for the space rather than the character code of 32. 
                ' The trim function will not clean this type of space.
                wksht.Cells(iRow, iCol).Value = (wksht.Cells(iRow, iCol).Value.ToString).Replace(Chr(160), Chr(32))

            Else
                'Cleanout space fill cells
                wksht.Cells(iRow, iCol).Value = ""
            End If

            wksht.Cells(iRow, iCol).Interior.Color = System.Drawing.Color.LightYellow

            Select Case field.DataType
                Case "Number"
                    If Len(Trim(wksht.Cells(iRow, iCol).Value)) = 0 Then
                        wksht.Cells(iRow, iCol).Interior.Color = System.Drawing.Color.LightGreen
                    ElseIf IsNumeric(wksht.Cells(iRow, iCol).Value) Then
                        wksht.Cells(iRow, iCol).Interior.Color = System.Drawing.Color.LightGreen
                    ElseIf wksht.Cells(iRow, iCol).Value.ToString = "M" Then
                        wksht.Cells(iRow, iCol).Interior.Color = System.Drawing.Color.LightGreen
                    Else
                        sbErrs.Append("Worksheet:" & field.xlSheet & "<br/>" & " Row:" & field.xlRow & " Column:" & field.xlColumn & "<br/><br/>")
                        iErrCnt += 1
                    End If

                Case "Date"
                    Dim TestDate As Date = wbk.NumberToDateTime(wksht.Cells(iRow, iCol).Value)

                    If Len(Trim(wksht.Cells(iRow, iCol).Value)) = 0 Then
                        wksht.Cells(iRow, iCol).Interior.Color = System.Drawing.Color.LightGreen
                    ElseIf IsDate(TestDate) Then
                        wksht.Cells(iRow, iCol).Interior.Color = System.Drawing.Color.LightGreen
                    ElseIf wksht.Cells(iRow, iCol).Value.ToString = "M" Then
                        wksht.Cells(iRow, iCol).Interior.Color = System.Drawing.Color.LightGreen
                    Else
                        sbErrs.Append("Worksheet:" & field.xlSheet & "<br/>" & " Row:" & field.xlRow & " Column:" & field.xlColumn & "<br/><br/>")
                        iErrCnt += 1
                    End If

                Case "Text"
                    'If CType(Session("SurveyID"), Integer) = 4 Then
                    'text cells not validated
                    wksht.Cells(iRow, iCol).Interior.Color = System.Drawing.Color.LightGreen
                    'End If

            End Select


            If sbErrs.Length = 0 Then 'only take this step workbook is valid

                Select Case field.Field
                    Case "DateID"
                        m_dtDateId = wbk.NumberToDateTime(wksht.Cells(iRow, iCol).Value)
                    Case "SourceID"
                        m_iSourceId = wksht.Cells(iRow, iCol).Value
                    Case "MOR"
                        m_lstMORs.Add(wksht.Cells(iRow, iCol).Value)

                        'used to capture MOR of sueveys with MOR by  worksheet
                    Case Else
                        If field.Description.ToString.Contains("Method of Reporting") Then
                            m_lstMORs.Add(wksht.Cells(iRow, iCol).Value)
                        End If
                End Select



                m_lstData.Add(CheckNull(wksht.Cells(iRow, iCol).Value))

            End If
        Next

        If sbErrs.Length > 0 Then
            If iErrCnt > 5 Then
                dctImportErrs(wbk.Name) = "More than 5 data validation errors."
            Else
                dctImportErrs(wbk.Name) = sbErrs.ToString
            End If
        End If

    End Sub

    Protected Sub copySubmissionTofData(ByRef arySurveyData As String(), ByRef iSeriesID As Integer, ByRef iWorkbookID As Integer, sWbkName As String,
                                         ByRef dctImportErrs As Dictionary(Of String, String))
        Dim sMessage As String = ""
        Try
            Dim nPages As Integer
            Dim nMORs As Integer


            'amount by which to increment progrees bar
            Dim iProgressIncrement As Integer = 1000



            Dim fLayout_DB = (From fL In m_db.fLayouts Where fL.SurveyID = CType(Session("SurveyID"), Integer)).ToArray

            'Implicit inner join
            ' to access the value in a field for a given record here is a syntax example: fDFs(0).dF.SortKey
            Dim fDFs = From dF In m_DD, lF In fLayout_DB Where dF.FieldID = lF.FieldID Order By dF.SortKey

            '########### The new getDataDictionary is already filtered by surveyID
            'Dim queryMORs = From fld In dd _
            '                Select fld.FieldID, fld.SurveyID, fld.SortKey, fld.Description _
            '                Order By SortKey Ascending _
            '                Where (SurveyID = CType(Session("SurveyID"), Integer) _
            '                      And Description.StartsWith("Method of Reporting"))

            Dim queryMORs = From fld In m_DD _
                           Select fld.FieldID, fld.SurveyID, fld.SortKey, fld.Description _
                           Order By SortKey Ascending _
                           Where Description.Contains("Method of Reporting")

            Dim MORs = (From sk In queryMORs Select sk.SortKey Order By SortKey Ascending).ToArray


            'Check number MORs equal 1 or number of fPages(worksheets or pages in survey)
            nPages = (From p In m_db.fPages Where p.SurveyID = CType(Session("SurveyID"), Integer)).Count
            nMORs = 1 + UBound(MORs)
            If (nMORs <> 1) And (nMORs <> nPages) Then
                dctImportErrs(sWbkName) = "The number of MORs does not equal 1 or number of worksheets or pages in survey."
                Return
            End If




            'copy SurveyData to fData table
            Dim fD As fData
            Dim fDList As New List(Of fData)
            m_db.fDatas.InsertAllOnSubmit(fDList)
            For Each fld In fDFs
                fD = New fData
                fD.SurveySeriesID = iSeriesID
                fD.WorkbookID = iWorkbookID
                fD.FieldID = fld.dF.FieldID
                fD.Value = arySurveyData(fld.dF.SortKey - 1) ' Remember arrays are base zero sortkey starts with 1.
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
                        ' fD.MOR = arySurveyData(MORs(0)) ' Remember arrays are base zero
                        fD.MOR = arySurveyData(MORs(0) - 1) ' Remember arrays are base zero
                        ' End Change
                    Case Is > 1
                        fD.MOR = arySurveyData(MORs(fld.lF.fPage.PageNumber - 1) - 1) ' Remember arrays are base zero page number starts with 1.
                End Select
                fDList.Add(fD)

            Next



            m_db.fDatas.InsertAllOnSubmit(fDList)
            m_db.SubmitChanges()

            fLayout_DB = Nothing
            fDFs = Nothing
            queryMORs = Nothing
            MORs = Nothing
            fD = Nothing
            fDList = Nothing

        Catch ex As Exception
            gF.NotifyWebmaster(ex, Request.ServerVariables.Item("PATH_INFO"), "Import CopySubmissionTofData Error")

            dctImportErrs(sWbkName) = "Import CopySubmissionTofData Error"

        End Try

    End Sub

    Protected Sub btnOpen_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnOpen.Click


        Dim sFilePath As String
        Dim sMessage As String = ""

        Dim sDir As String = pF.getSurveyFilePath(CType(Session("SurveyID"), Integer))
        Dim sWorkDir As String = sDir & "Data\BatchProcess\"

        Try
            If Source.SelectedValue = Nothing Then
                'Literal1.Text = "No Workbook Selected!"
            Else

                sFilePath = "File:" & sWorkDir & Source.SelectedValue
                Response.Redirect(sFilePath, False)
            End If
        Catch ex As Exception
            gF.NotifyWebmaster(ex, Request.ServerVariables.Item("PATH_INFO"), "Open Button Click Error")
            
        End Try

    End Sub

    Protected Sub getFileLists(lt As ListType)

        'Dim sMessage As String = ""
        Dim sPath As String = pF.getSurveyFilePath(CType(Session("SurveyID"), Integer))

        Dim CurrentFileList As IEnumerable(Of String)

        Try

            Dim files = pF.getNewlySubmittedWorkbooks(CType(Session("SurveyID"), Integer)) 'Need to use surveyID

            Dim ImpDets = From id In m_db.ImportDetails
                        Where files.Contains(id.WorkbookName)


            Select Case lt
                Case ListType.New
                    CurrentFileList = (pF.getNewlySubmittedWorkbooks(CType(Session("SurveyID"), Integer))).Except(From file In ImpDets
                                                             Select file.WorkbookName)
                    spnSource.InnerText = "New Surveys"
                    btnOpen.Enabled = False

                Case ListType.Input
                    CurrentFileList = From file In ImpDets
                                       Where file.ImportResult.StartsWith("The following worksheets")
                                       Select file.WorkbookName
                    spnSource.InnerText = "Input Required"
                    btnOpen.Enabled = True
                Case ListType.Error
                    CurrentFileList = From file In ImpDets
                                       Where Not file.ImportResult.StartsWith("The following worksheets")
                                       Select file.WorkbookName
                    spnSource.InnerText = "Import Errors"
                    btnOpen.Enabled = True
            End Select

            'cature current list to control whether to enable "Open" button

            Session("CurrentFileList") = lt

            spnImportStatus.InnerText = "Import Status"

            If CurrentFileList.Count > 0 Then
                Source.DataSource = CurrentFileList.ToList()
                Source.DataBind()
            Else
                Source.Items.Clear()
            End If


            Dim dctImportDates As Dictionary(Of String, String) = New Dictionary(Of String, String)
            Dim dctImportResults As Dictionary(Of String, String) = New Dictionary(Of String, String)

            For Each det In ImpDets
                dctImportDates.Add(det.WorkbookName, det.ImportDate)
                dctImportResults.Add(det.WorkbookName, det.ImportResult)
            Next

            Cache("ImportDates") = dctImportDates
            Cache("ImportResults") = dctImportResults

            Target.Items.Clear()

        Catch ex As Exception
            gF.NotifyWebmaster(ex, Request.ServerVariables.Item("PATH_INFO"), "Import Upload Workbook Error")
        End Try

    End Sub

    Protected Sub getFileStatus(ls As ListStatus)

        'Dim sMessage As String = ""
        'Dim sPath As String = pF.getSurveyFilePath(CType(Session("SurveyID"), Integer))



        Dim CurrentFileList As IEnumerable(Of String)

        Try

            'Dim files = pF.getNewlySubmittedWorkbooks(CType(Session("SurveyID"), Integer)) 'Need to use surveyID

            'Dim ImpDets = From id In m_db.ImportDetails
            'Where(files.Contains(ID.WorkbookName))

            'Dim ImportedFileList = From files In ImpDets
            'Select files.WorkbookName

            'getNewlySubmittedWorkbooks = (gF.getXLWBDirectory(strSurveyFilePath & "Data\0-Submitted")).Except(files)


            Select Case ls
                Case ListStatus.InProc
                    CurrentFileList = (From id In m_db.ImportDetails
                                      Where id.ImportResult = INPROC_STATUS AndAlso
                                            id.SurveyId = CType(Session("SurveyID"), Integer)
                                      Select id.WorkbookName).ToList
                    spnImportStatus.InnerText = "Surveys In Process"

                Case ListStatus.Complete

                    CurrentFileList = (From id In m_db.ImportDetails
                                      Where id.SurveyId = CType(Session("SurveyID"), Integer) AndAlso
                                            id.ImportResult = "Imported"
                                      Select id.WorkbookName).ToList

                    spnImportStatus.InnerText = "Imported Surveys"
            End Select


            If CurrentFileList.Count > 0 Then
                Target.DataSource = CurrentFileList.ToList()
                Target.DataBind()
            Else
                Target.Items.Clear()
            End If

        Catch ex As Exception
            gF.NotifyWebmaster(ex, Request.ServerVariables.Item("PATH_INFO"), "Import Upload Workbook Error")
        End Try

    End Sub
    

    Protected Sub Source_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Source.SelectedIndexChanged

        Dim wbks = From item As ListItem In Source.Items
                      Where item.Selected
                      Select item.Text


        If wbks.Count = 1 Then

            Dim dctImportDates As Dictionary(Of String, String) = Cache("ImportDates")
            Dim dctImportResults As Dictionary(Of String, String) = Cache("ImportResults")




            If dctImportDates.ContainsKey(wbks(0)) Then
                'lblStatus.Height = Unit.Empty
                spnImportStatus.InnerText = "Import Status (" & dctImportDates(wbks(0)) & ")"
                Target.Items.Clear()
                Target.Items.Add(dctImportResults(wbks(0)))


            Else
                spnImportStatus.InnerText = "Import Status"
                'lblStatus.Height = Unit.Parse("300px")
                Target.Items.Clear()
                Target.Items.Add("Ready To Import")
            End If
        Else
            spnImportStatus.InnerText = "Import Status"
            'lblStatus.Height = Unit.Parse("300px")
            Target.Items.Clear()
        End If
    End Sub


    Private Sub SetPageTitle(ByVal TitleText As String)

        If Me.Page.Form.Parent IsNot Nothing Then
            Dim litPageTitle As Literal = Me.Page.Form.Parent.FindControl("litPageTitle")

            If litPageTitle IsNot Nothing Then
                litPageTitle.Text = TitleText
            End If
        End If

    End Sub

    Protected Sub displayMessageBox(ByVal msg As String)

        Page.ClientScript.RegisterStartupScript(Me.GetType(), "AlertMessage", "alert('" + msg + "');", True)

    End Sub

    Private Function CheckNull(value As String) As String
        If value Is Nothing Then
            value = String.Empty
        End If

        Return value
    End Function

    Protected Sub DeleteWbk(sWbkName As String, ByRef dctImportErrs As Dictionary(Of String, String))


        Try
            Dim sDir As String = pF.getSurveyFilePath(CType(Session("SurveyID"), Integer)) & "Data\BatchProcess\"


            If System.IO.File.Exists(sDir & sWbkName) Then
                System.IO.File.Delete(sDir & sWbkName)
            End If

        Catch ex As Exception
            gF.NotifyWebmaster(ex, Request.ServerVariables.Item("PATH_INFO"), "Import CopySubmissionTofData Error")
            dctImportErrs(sWbkName) = "Unable to delete  workbook."
        End Try




    End Sub

    Protected Sub updateImportDetail(sWbkName As String, sStatus As String)
        'get  import detail from db
        Dim detail = (From det In m_db.ImportDetails
                      Where det.WorkbookName = sWbkName).FirstOrDefault

        'update existing record
        If Not detail Is Nothing Then
            detail.ImportDate = Now
            detail.ImportResult = sStatus
            m_db.SubmitChanges()
        End If
    End Sub



    Protected Sub btnNew_Click(sender As Object, e As EventArgs) Handles btnNew.Click
        getFileLists(ListType.New)

    End Sub

    Protected Sub btnInput_Click(sender As Object, e As EventArgs) Handles btnInput.Click
        getFileLists(ListType.Input)

    End Sub

    Protected Sub btnErr_Click(sender As Object, e As EventArgs) Handles btnErr.Click
        getFileLists(ListType.Error)

    End Sub

    Protected Sub btnInProc_Click(sender As Object, e As EventArgs) Handles btnInProc.Click
        getFileStatus(ListStatus.InProc)
    End Sub

    Protected Sub btnComplete_Click(sender As Object, e As EventArgs) Handles btnComplete.Click
        getFileStatus(ListStatus.Complete)
    End Sub

    Protected Sub Source_Click(sender As Object, e As EventArgs) Handles Source.SelectedIndexChanged
        Dim lt As ListType = CType(Session("CurrentFileList"), ListType)

        Select Case lt
            Case ListType.New, 0 '0=no list selected
                btnOpen.Enabled = False
            Case ListType.Error, ListType.Input
                btnOpen.Enabled = True
        End Select

    End Sub
End Class