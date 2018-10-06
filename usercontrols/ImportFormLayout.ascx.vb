Imports SpreadsheetGear
Imports System.IO
Imports System.Linq
Imports System.Data.Linq
Imports Telerik.Web.UI
Imports Telerik.Web.UI.Upload
Public Class ImportFormLayout
    Inherits System.Web.UI.UserControl
    Protected ddFilePath As String
    Protected db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext
    Protected SurveyID As Integer
    Protected sMessage As StringBuilder = New StringBuilder
    Protected workbook As SpreadsheetGear.IWorkbook
    Protected worksheet As SpreadsheetGear.IWorksheet
    Protected gl As Globals.GlobalFunctions = New Globals.GlobalFunctions
    Protected progress As RadProgressContext = RadProgressContext.Current
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Page.User.IsInRole("SuperAdmin") OrElse Page.User.IsInRole("Admin") OrElse _
                  Page.User.IsInRole("SuperAnalyst") OrElse Page.User.IsInRole("Analyst") Then
            If Not Page.IsPostBack Then
                MultiView1.SetActiveView(vwDefault)
                litSurveyName.Text = " - " & Session("SurveyName")
            End If
            SurveyID = CType(Session("SurveyID").ToString(), Integer)
        Else
            MultiView1.SetActiveView(vwNotAuthorized)
        End If
    End Sub

    Private Sub btnImport_Click(sender As Object, e As System.EventArgs) Handles btnImport.Click

        Dim bContinue As Boolean = True

        Dim flayoutWS, fpagews, fsectionws, frowws, fcolumnws
        Try

            'Check for Data Dictionary/fLayout spreadsheet
            If Not validateFilePath() Then
                MultiView1.SetActiveView(vwAbort)
                litAbort.Text = "Data Dictionary at: " & ddFilePath.ToString & " does not exist."
                bContinue = False
            End If


            'if data dictionary is valid for survey get survey layout data
            If bContinue AndAlso IsValidDataDictionary() Then

                flayoutWS = (From f In workbook.GetDataSet("flayout", SpreadsheetGear.Data.GetDataFlags.NoColumnTypes).Tables("flayout").AsEnumerable _
                Where Not IsDBNull(f.Item("FieldID"))
                  Select FieldID = Convert.ToInt32(f.Item("FieldID")), _
                  SurveyID = f.Item("SurveyID"), _
                  PageNumber = f.Item("PageNumber"), _
                  sectionnumber = f.Item("SectionNumber"), _
                  RowNumber = f.Item("RowNumber"), _
                  ColumnNumber = f.Item("ColumnNumber"), _
                  DataTypeId = f.Item("DataTypeID")).ToArray

                fpagews = (From f In workbook.GetDataSet("fpages", SpreadsheetGear.Data.GetDataFlags.NoColumnTypes).Tables("fpages").AsEnumerable _
                      Select SurveyID = f.Item("SurveyID"), _
                      PageNumber = f.Item("PageNumber"), _
                      PageDes = f.Item("PageDescription")).ToArray

                fsectionws = (From f In workbook.GetDataSet("fsections", SpreadsheetGear.Data.GetDataFlags.NoColumnTypes).Tables("fsections").AsEnumerable _
                  Select SurveyID = f.Item("SurveyID"), _
                  SecNumber = f.Item("SectionNumber"), _
                  SecDes = f.Item("SectionDescription"), _
                  SecPageNum = f.Item("PageNumber")).ToArray

                frowws = (From f In workbook.GetDataSet("frows", SpreadsheetGear.Data.GetDataFlags.NoColumnTypes).Tables("frows").AsEnumerable _
                  Select SurveyID = f.Item("SurveyID"), _
                  RowNumber = f.Item("RowNumber"), _
                  RowDes = f.Item("RowDescription"), _
                  RowPageNum = f.Item("PageNumber"), _
                  RowSecNum = f.Item("SectionNumber")).ToArray

                fcolumnws = (From f In workbook.GetDataSet("fcolumns", SpreadsheetGear.Data.GetDataFlags.NoColumnTypes).Tables("fcolumns").AsEnumerable _
                  Select SurveyID = f.Item("SurveyID"), _
                  ColNumber = f.Item("ColumnNumber"), _
                  ColDes = f.Item("ColumnDescription"), _
                  ColPageNum = f.Item("PageNumber"), _
                  ColSecNum = f.Item("SectionNumber")).ToArray

                workbook = Nothing
            ElseIf bContinue Then
                    ' stop the process: The data dictionary does not exist for the selected survey
                sMessage.Append("Data Dictionary does not exists for selected survery...Please import Data Dictionary.. ")
                MultiView1.SetActiveView(vwAbort)
                litAbort.Text = sMessage.ToString()
                bContinue = False
            End If



            'Check FK relationships among layout components
            If bContinue AndAlso Not validateFK(flayoutWS, fpagews, fsectionws, frowws, fcolumnws) Then

                litAbort.Text = sMessage.ToString()
                MultiView1.SetActiveView(vwAbort)
                bContinue = False
            End If


            'Check that each FieldID from Worksheet is exist into the ddField table.
            If bContinue AndAlso Not validateddFields(flayoutWS, progress) Then
                MultiView1.SetActiveView(vwAbort)
                litAbort.Text = sMessage.ToString()
            End If

            'Check whether layout already exists in db
            'If bContinue AndAlso IsExistingLayout(progress) Then
            '    'Ask to user to stop or continue
            '    Dim m As MsgBoxResult
            '    m = MsgBox("Do you want to update existing survey layout?", MsgBoxStyle.OkCancel)
            '    If m = vbCancel Then
            '        sMessage.Append("Import Canceled")
            '        litAbort.Text = sMessage.ToString()
            '        MultiView1.SetActiveView(vwAbort)
            '        bContinue = False
            '    End If

            'End If


            'Process Layout
            If bContinue AndAlso
               Importfpage(fpagews, progress) AndAlso
               ImportfColumn(fcolumnws, progress) AndAlso
               Importfrow(frowws, progress) AndAlso
               ImportfSection(fsectionws, progress) AndAlso
               ImportfLayout(flayoutWS, progress) Then

                MultiView1.SetActiveView(vwConfirm)
            Else
                'DeleteRecords()
                litAbort.Text = sMessage.ToString()
                MultiView1.SetActiveView(vwAbort)
            End If

            progress.PrimaryValue = 100
            progress.PrimaryPercent = 100

        Catch ex As Exception
            gl.NotifyWebmaster(ex, gl.GetCurrentUrl, "Import Form Layout Error Survey: " & litSurveyName.Text)
            MultiView1.SetActiveView(vwError)
        End Try





    End Sub
    Protected Function validateFilePath() As Boolean

        Dim query = From s In db.dSurveys _
                    Where s.SurveyID = SurveyID _
                    Select s.FolderPath

        If query.Count > 0 Then
            For Each item In query
                ddFilePath = item.ToString & ConfigurationManager.AppSettings("DataDictionaryPath").ToString
            Next


#If DEBUG Then

            ddFilePath = ddFilePath.Replace("Sales Survey Analysis System", "SalesSurveyDev")
#End If
            If File.Exists(ddFilePath) Then

                workbook = SpreadsheetGear.Factory.GetWorkbook(ddFilePath)

                Return True
            Else
                Return False
            End If
        Else
            Return False
        End If

    End Function

    Private Function IsValidDataDictionary() As Boolean

        Dim surveyidq = From s In db.ddFields _
                      Where s.SurveyID = SurveyID _
                      Select s
        If surveyidq.Count > 0 Then
            Return True
        Else
            Return False
        End If
    End Function
    Protected Function validateddFields(ByVal worksheet As Array, ByRef progress As RadProgressContext) As Boolean
        progress.PrimaryValue = 15
        progress.PrimaryPercent = 15


        Dim wsFieldIDs = (From t In worksheet _
                        Select t.FieldID)

        Dim ddF = From f In db.ddFields _
                  Where f.SurveyID = SurveyID _
                  Select f.FieldID

        Dim cF = (From x In wsFieldIDs Where ddF.Contains(x) Select x).ToArray

        If cF.Count > 0 Then
            Return True
        Else
            sMessage.Append("Data from Workbook does not exists in DataDictinory- ddfield")
            ' ''For Each item In cF
            ' ''    sMessage.Append(item.ToString & ", ")
            ' ''Next
            Return False
        End If


    End Function
    Protected Function Validatefdata(ByVal worksheet As Array, ByRef progress As RadProgressContext) As Boolean
        progress.PrimaryValue = 30
        progress.PrimaryPercent = 30

        Dim wsFieldIDs = (From t In worksheet _
                      Select t.FieldID)


        Dim ddF = (From f In db.fDatas _
                 Join ss In db.dSurveySeries _
                 On f.SurveySeriesID Equals ss.SurveySeriesID _
                 Where ss.SurveyID = SurveyID _
                 Select f.FieldID).ToArray

        Dim cF = (From x In wsFieldIDs Where ddF.Contains(x) Select x).ToArray

        If cF.Count > 0 Then

            sMessage.Append("FData exists for " & cF.Count.ToString & " fields. The selected survey is not a new survey. <br />")
            For Each item In cF
                sMessage.Append(item.ToString & ", ")
            Next
            Return False
        Else

            Return True
        End If

    End Function
    Protected Function IsExistingLayout(ByRef progress As RadProgressContext) As Boolean
        progress.PrimaryValue = 45
        progress.PrimaryPercent = 45

        Dim fpage = From f In db.fPages _
                Where f.SurveyID = SurveyID _
                Select f

        Dim frow = From f In db.fRows _
               Where f.SurveyID = SurveyID _
               Select f

        Dim fcolumn = From f In db.fColumns _
               Where f.SurveyID = SurveyID _
               Select f

        Dim fsection = From f In db.fSections _
               Where f.SurveyID = SurveyID _
               Select f

        Dim flayout = From f In db.fLayouts _
                    Where f.SurveyID = SurveyID _
                    Select f

        If fpage.Count > 0 Or frow.Count > 0 Or fcolumn.Count > 0 Or fsection.Count > 0 Or flayout.Count > 0 Then

            Return True
        Else

            Return False
        End If



    End Function
    Protected Function DeleteRecords() As Boolean
        
        Dim fpage = From f In db.fPages _
               Where f.SurveyID = SurveyID _
               Select f

        Dim frow = From f In db.fRows _
               Where f.SurveyID = SurveyID _
               Select f

        Dim fcolumn = From f In db.fColumns _
               Where f.SurveyID = SurveyID _
               Select f

        Dim fsection = From f In db.fSections _
               Where f.SurveyID = SurveyID _
               Select f

        Dim flayout = From f In db.fLayouts _
                    Where f.SurveyID = SurveyID _
                    Select f

        db.fPages.DeleteAllOnSubmit(fpage)
        db.fRows.DeleteAllOnSubmit(frow)
        db.fColumns.DeleteAllOnSubmit(fcolumn)
        db.fSections.DeleteAllOnSubmit(fsection)
        db.fLayouts.DeleteAllOnSubmit(flayout)

        Return True

    End Function
    Protected Function Importfpage(ByVal fpagews As Array, ByRef progress As RadProgressContext) As Boolean
        progress.PrimaryValue = 55
        progress.PrimaryPercent = 55
        Try


            For Each item In fpagews
                Dim pgnum As Integer = item.PageNumber

                Dim fPages_DBRow = (From fp In db.fPages
                                    Select fp
                                    Where fp.SurveyID = SurveyID AndAlso
                                          fp.PageNumber = pgnum).SingleOrDefault

                If fPages_DBRow Is Nothing Then 'new page

                    Dim page As New fPage With {.SurveyID = item.SurveyId, _
                                           .PageNumber = item.PageNumber, _
                                           .PageDescription = item.PageDes}
                    db.fPages.InsertOnSubmit(page)
                Else

                    With fPages_DBRow
                        .PageDescription = item.PageDes
                    End With
                End If
                db.SubmitChanges()
            Next

            '   db.SubmitChanges() moved this into the loop above

            Return True
        Catch ex As Exception
            sMessage.Append(" - ImportFPage Error")
            Return False
        End Try

    End Function
    Protected Function ImportfColumn(ByVal fcolumnws As Array, ByRef progress As RadProgressContext) As Boolean

        progress.PrimaryValue = 65
        progress.PrimaryPercent = 65
        Try


            For Each item In fcolumnws

                Dim pgnum As Integer = item.ColPageNum
                Dim secnum As Integer = item.ColSecNum
                Dim colnum As Integer = item.ColNumber

                Dim fCols_DBRow = (From fc In db.fColumns
                                   Select fc
                                   Where fc.SurveyID = SurveyID AndAlso
                                         fc.PageNumber = pgnum AndAlso
                                         fc.SectionNumber = secnum AndAlso
                                         fc.ColumnNumber = colnum).SingleOrDefault
                If fCols_DBRow Is Nothing Then 'new column

                    Dim col As New fColumn With {.SurveyID = item.SurveyID, _
                                                 .ColumnNumber = item.ColNumber, _
                                                 .ColumnDescription = item.ColDes, _
                                                 .PageNumber = item.ColPageNum, _
                                                 .SectionNumber = item.ColSecNum}
                    db.fColumns.InsertOnSubmit(col)

                Else
                    fCols_DBRow.ColumnDescription = item.ColDes

                End If
                db.SubmitChanges()
            Next

            'db.SubmitChanges() moved this into the loop above

            Return True
        Catch ex As Exception
            sMessage.Append(" - ImportFCol Error")
            Return False
        End Try
    End Function
    Protected Function Importfrow(ByVal frowws As Array, ByRef progress As RadProgressContext) As Boolean

        progress.PrimaryValue = 75
        progress.PrimaryPercent = 75
        Try


            For Each item In frowws

                Dim pgnum As Integer = item.RowPageNum
                Dim secnum As Integer = item.RowSecNum
                Dim rownum As Integer = item.RowNumber

                Dim fRows_DBRow = (From fr In db.fRows
                                   Select fr
                                   Where fr.SurveyID = SurveyID AndAlso
                                        fr.PageNumber = pgnum AndAlso
                                        fr.SectionNumber = secnum AndAlso
                                        fr.RowNumber = rownum).SingleOrDefault

                If fRows_DBRow Is Nothing Then

                    Dim row As New fRow With {.SurveyID = item.surveyid, _
                                         .RowNumber = item.RowNumber, _
                                         .RowDescription = item.RowDes, _
                                         .PageNumber = item.RowPageNum, _
                                         .SectionNumber = item.RowSecNum}
                    db.fRows.InsertOnSubmit(row)

                Else

                    fRows_DBRow.RowDescription = item.RowDes

                End If
                db.SubmitChanges()
               
            Next
            'db.SubmitChanges() moved this into the loop above
            Return True
        Catch ex As Exception
            sMessage.Append(" - ImportFRow Error")
            Return False
        End Try
    End Function
    Protected Function ImportfSection(ByVal fsectionws As Array, ByRef progress As RadProgressContext) As Boolean
        progress.PrimaryValue = 85
        progress.PrimaryPercent = 85

        Try

            For Each item In fsectionws

                Dim pgnum As Integer = item.SecPageNum
                Dim secnum As Integer = item.SecNumber

                Dim fSections_DBRow = (From fs In db.fSections
                                       Select fs
                                       Where fs.SurveyID = SurveyID AndAlso
                                             fs.PageNumber = pgnum AndAlso
                                             fs.SectionNumber = secnum).SingleOrDefault

                If fSections_DBRow Is Nothing Then
                    Dim section As New fSection With {.SurveyID = item.SurveyID, _
                                                  .SectionNumber = item.SecNumber, _
                                                  .SectionDescription = item.SecDes, _
                                                  .PageNumber = item.SecPageNum}
                    db.fSections.InsertOnSubmit(section)

                Else
                    fSections_DBRow.SectionDescription = item.SecDes
                End If
                db.SubmitChanges()
                
            Next
            'db.SubmitChanges()  moved this into the loop above
            Return True
        Catch ex As Exception
            sMessage.Append(" - ImportFSection Error")
            Return False
        End Try
    End Function
    Protected Function ImportfLayout(ByVal flayoutws As Array, ByRef progress As RadProgressContext) As Boolean
        progress.PrimaryValue = 95
        progress.PrimaryPercent = 95

        Dim fid As Integer
        Dim pid As Integer
        Dim cid As Integer
        Dim sid As Integer
        Dim rid As Integer
        Dim did As Integer

        Try


            For Each item In flayoutws

                '************** verify foreign keys **************
                fid = item.FieldID
                pid = item.PageNumber
                cid = item.ColumnNumber
                rid = item.RowNumber
                sid = item.sectionnumber
                did = item.DataTypeId



                Dim nddfield = (From t In db.ddFields _
                                           Where t.SurveyID = SurveyID AndAlso t.FieldID = fid).Single

                Dim pagenumber = (From fp In db.fPages _
                                  Where fp.SurveyID = SurveyID AndAlso
                                        fp.PageNumber = pid).Single

                Dim secid = (From fp In db.fSections _
                             Where fp.SurveyID = SurveyID AndAlso
                                   fp.SectionNumber = sid AndAlso
                                   fp.PageNumber = pid).Single

                Dim Colid = (From fp In db.fColumns _
                             Where fp.SurveyID = SurveyID AndAlso
                                   fp.ColumnNumber = cid AndAlso
                                   fp.PageNumber = pid AndAlso
                                   fp.SectionNumber = sid).Single

                Dim rownumber = (From fp In db.fRows _
                                 Where fp.SurveyID = SurveyID AndAlso
                                       fp.RowNumber = rid AndAlso
                                       fp.PageNumber = pid AndAlso
                                       fp.SectionNumber = sid).Single

                '******************************


                Dim fLayouts_DBRow = (From fl In db.fLayouts
                                      Select fl
                                      Where fl.FieldID = fid AndAlso
                                            fl.SurveyID = SurveyID).SingleOrDefault

                If fLayouts_DBRow Is Nothing Then
                    Dim formlayout As New fLayout With {.FieldID = item.Fieldid, _
                                                        .SurveyID = item.SurveyID, _
                                                        .PageNumber = pagenumber.PageNumber, _
                                                        .SectionNumber = secid.SectionNumber, _
                                                        .RowNumber = rownumber.RowNumber, _
                                                        .ColumnNumber = Colid.ColumnNumber, _
                                                        .DataTypeID = nddfield.DataTypeID}

                    db.fLayouts.InsertOnSubmit(formlayout)

                Else
                    With fLayouts_DBRow
                        .PageNumber = pid
                        .SectionNumber = sid
                        .RowNumber = rid
                        .ColumnNumber = cid
                        .DataTypeID = did
                        .Inactive = Nothing
                    End With

                    nddfield.Source = "R"

                End If
            Next

            Dim FieldIds_WS = (From f In flayoutws
                              Select Convert.ToInt32(f.FieldId)).ToArray

            Dim FieldIds_Inactive = (From f In db.fLayouts _
                                    Where f.SurveyID = SurveyID _
                                    Select f.FieldID).ToArray.Except(FieldIds_WS)


            Dim fLayouts_DBRowInactive = (From l In db.fLayouts
                                         Where FieldIds_Inactive.Contains(l.FieldID)
                                         Select l).ToArray




            For Each item In fLayouts_DBRowInactive
                If item.Inactive Is Nothing Then
                    item.Inactive = DateTime.Now
                End If

                Dim iFieldId As Integer = item.FieldID

                Dim DBFieldRow = (From s In db.ddFields _
                                 Select s _
                                 Where s.SurveyID = SurveyID AndAlso s.FieldID = iFieldId).Single
                DBFieldRow.Source = "A" 'sets corresponding ddFields row to (A)rchive

            Next

            db.SubmitChanges()
            Return True
        Catch ex As Exception
            sMessage.Append(" - ImportFLayout Error")
            Return False
        End Try
    End Function

    Protected Function validateFK(ByVal worksheet As Array) As Boolean

        Dim FKValidate As Boolean = True

        'Check PageNumber for PF-FK Relationship
        Dim worksheet2 = (From t In worksheet _
                        Select t.pagenumber).Distinct

        Dim fpage = (From f In db.fPages _
                    Select f.PageNumber).ToArray

        
        Dim pF = (From x In worksheet2 Where fpage.Contains(x) Select x).ToArray
        Dim pF1 = (worksheet2.Except(pF)).ToArray
        If pF1.Count > 0 Then
            FKValidate = False
            sMessage.Append("<br />PageNumber does not exist into the FPage. Please check flayout worksheet agaist fpage: ")
            For Each item In pF1
                sMessage.Append(item.ToString & ", ")
            Next
        End If

        'Check ColumnNumber PK-FK Relationship
        Dim worksheet3 = (From t In worksheet _
                       Select t.ColumnNumber).Distinct

        Dim fcolumn = (From f In db.fColumns _
                    Select f.ColumnNumber).ToArray


        Dim cF = (From x In worksheet3 Where fcolumn.Contains(x) Select x).ToArray
        Dim cF1 = (worksheet2.Except(cF)).ToArray
        If cF1.Count > 0 Then
            FKValidate = False
            sMessage.Append("<br />PageNumber does not exist into the FColumn. Please check flayout worksheet agaist fpage: ")
            For Each item In cF1
                sMessage.Append(item.ToString & ", ")
            Next
        End If


        'Check section id PK-FK Relationship

        Dim worksheet4 = (From t In worksheet _
                       Select t.sectionnumber).Distinct

        Dim fsection = (From f In db.fSections _
                    Select f.SectionNumber).ToArray


        Dim sF = (From x In worksheet4 Where fsection.Contains(x) Select x).ToArray
        Dim sF1 = (worksheet2.Except(cF)).ToArray
        If sF1.Count > 0 Then
            FKValidate = False
            sMessage.Append("<br />PageNumber does not exist into the FColumn. Please check flayout worksheet agaist fpage: ")
            For Each item In sF1
                sMessage.Append(item.ToString & ", ")
            Next
        End If


        'Check row id PK-FK Relationship

        Dim worksheet5 = (From t In worksheet _
                       Select t.rownumber).Distinct

        Dim frow = (From f In db.fRows _
                    Select f.RowNumber).ToArray


        Dim rF = (From x In worksheet5 Where frow.Contains(x) Select x).ToArray
        Dim rF1 = (worksheet2.Except(cF)).ToArray
        If sF1.Count > 0 Then
            FKValidate = False
            sMessage.Append("<br />PageNumber does not exist into the FColumn. Please check flayout worksheet agaist fpage: ")
            For Each item In rF1
                sMessage.Append(item.ToString & ", ")
            Next
        End If
        Return FKValidate

    End Function

    Protected Function validateFK(ByVal flayout As Array, ByVal fpage As Array, ByVal fsection As Array, ByVal frow As Array, ByVal fcolumn As Array) As Boolean
        Dim FKValidate As Boolean = True

        'PK->FK for fPage
        Dim PgNbrs_fLayout = (From t In flayout _
                              Select t.PageNumber).Distinct

        Dim PgNbrs_fPages = (From f In fpage _
                    Select f.PageNumber).ToArray


        'Dim pF = (From x In PgNbrs_fPages Where PgNbrs_fLayout.Contains(x) Select x).ToArray

        Dim PgNbrs_Orphan = (PgNbrs_fLayout.Except(PgNbrs_fPages)).ToArray
        If PgNbrs_Orphan.Count > 0 Then
            FKValidate = False
            sMessage.Append("<br />fLayout PageNumber does not exist in fPages: ")
            For i As Integer = 1 To PgNbrs_Orphan.Count
                Dim item = PgNbrs_Orphan(i - 1)
                sMessage.Append(item.ToString)
                If i < PgNbrs_Orphan.Count Then
                    sMessage.Append(", ")
                End If
            Next
        End If

        'PK->FK for fcolumn
        Dim ColNbrs_fLayout = (From t In flayout _
                               Select t.ColumnNumber).Distinct

        Dim ColNbrs_fColumns = (From f In fcolumn _
                                Select f.colnumber).ToArray


        'Dim pc = (From x In ColNbrs_fColumns Where ColNbrs_fLayout.Contains(x) Select x).ToArray

        Dim ColNbrs_Orphan = (ColNbrs_fLayout.Except(ColNbrs_fColumns)).ToArray
        If ColNbrs_Orphan.Count > 0 Then
            FKValidate = False
            sMessage.Append("<br />fLayout ColumnNumber does not exist in fColumns: ")
            For i As Integer = 1 To ColNbrs_Orphan.Count
                Dim item = ColNbrs_Orphan(i - 1)
                sMessage.Append(item.ToString)
                If i < ColNbrs_Orphan.Count Then
                    sMessage.Append(", ")
                End If
            Next
        End If

        'PK->FK for fRow

        Dim RowNbrs_fLayout = (From t In flayout _
                               Select t.rownumber).Distinct

        Dim RowNbrs_fRows = (From f In frow _
                             Select f.rownumber).ToArray


        'Dim pr = (From x In RowNbrs_fRows Where RowNbrs_fLayout.Contains(x) Select x).ToArray

        Dim RowNbrs_Orphan = (RowNbrs_fLayout.Except(RowNbrs_fRows)).ToArray

        If RowNbrs_Orphan.Count > 0 Then
            FKValidate = False
            sMessage.Append("<br />fLayout RowNumber does not exist in fRows: ")
            For i As Integer = 1 To RowNbrs_Orphan.Count
                Dim item = RowNbrs_Orphan(i - 1)
                sMessage.Append(item.ToString)
                If i < RowNbrs_Orphan.Count Then
                    sMessage.Append(", ")
                End If
            Next
        End If


        'PK->FK for fSection

        Dim SecNbrs_fLayout = (From t In flayout _
                      Select t.sectionnumber).Distinct

        Dim SecNbrs_fSections = (From f In fsection _
                    Select f.secnumber).ToArray


        'Dim ps = (From x In fs Where fworksheets.Contains(x) Select x).ToArray

        Dim SecNbrs_Orphan = (SecNbrs_fLayout.Except(SecNbrs_fSections)).ToArray
        If SecNbrs_Orphan.Count > 0 Then
            FKValidate = False
            sMessage.Append("<br />fLayout SectionNumber does not exist in fSections: ")
           
            For i As Integer = 1 To SecNbrs_Orphan.Count
                Dim item = SecNbrs_Orphan(i - 1)
                sMessage.Append(item.ToString)
                If i < SecNbrs_Orphan.Count Then
                    sMessage.Append(", ")
                End If
            Next

        End If
        Return FKValidate

    End Function
End Class