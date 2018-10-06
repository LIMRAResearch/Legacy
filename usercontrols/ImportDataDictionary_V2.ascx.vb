Imports SpreadsheetGear
Imports System.IO
Imports System.Linq
Imports System.Data.Linq
Imports Telerik.Web.UI
Imports Telerik.Web.UI.Upload


Partial Public Class ImportDataDictionary_V2
    Inherits System.Web.UI.UserControl


    Protected ddFilePath As String
    Protected db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext
    Protected SurveyID As Integer
    Protected sMessage As StringBuilder = New StringBuilder
    Protected workbook As SpreadsheetGear.IWorkbook
    Protected gl As Globals.GlobalFunctions = New Globals.GlobalFunctions
    Protected progress As RadProgressContext = RadProgressContext.Current


    '''<summary>
    '''checks if user is in role, checks for page postback, sets page title to survey name, displays default view
    '''</summary>
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

    '''<summary>
    '''event handler for button click, checks if the data dictionary file exists, then creates arrays from each of the worksheets being imported.
    '''Runs validations for each worksheet, checking for PK-->FK relations, and also existing data in database.
    '''Runs validations between worksheets.
    '''If a validation fails, the import is aborted, and a details message is displayed.
    '''If all the validations are successful, the database gets updated, either inserting new records, or updating existing records
    '''</summary>
    Protected Sub btnImport_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnImport.Click

        progress.PrimaryTotal = 100
        progress.PrimaryValue = 0
        progress.PrimaryPercent = 0

        Dim bContinue As Boolean = True
        Dim ddFieldsWS, ddcFieldsWS, ddAttributesWS, ddAlgorithmsWS

        Try
            If Not validateFilePath() Then
                MultiView1.SetActiveView(vwAbort)
                litAbort.Text = "Data Dictionary at: " & ddFilePath.ToString & " does not exist."
                bContinue = False
            End If


            If bContinue Then
                ddFieldsWS = (From f In workbook.GetDataSet("ddFields", Data.GetDataFlags.NoColumnTypes).Tables("ddFields").AsEnumerable _
                           Select FieldID = Convert.ToInt32(f.Item("FieldID")), _
                           SortKey = Convert.ToInt32(f.Item("SortKey")), _
                           SurveyID = Convert.ToInt32(f.Item("SurveyID")), _
                           Source = f.Item("Source"), _
                           Field = f.Item("Field"), _
                           xlRow = Convert.ToInt32(f.Item("xlRow")), _
                           xlColumn = Convert.ToInt32(f.Item("xlColumn")), _
                           xlSheet = f.Item("xlSheet"), _
                           xlCell = f.Item("xlCell"), _
                           DataTypeID = Convert.ToInt32(f.Item("DataTypeID")), _
                           ExceptionSetID = Convert.ToInt32(f.Item("ExceptionSetID")), _
                           MetricID = Convert.ToInt32(f.Item("MetricID")), _
                           ScaleID = Convert.ToInt32(f.Item("ScaleID")), _
                           UnitID = Convert.ToInt32(f.Item("UnitID")), _
                           Description = f.Item("Description")).ToArray

                ddcFieldsWS = (From f In workbook.GetDataSet("ddcFields", Data.GetDataFlags.NoColumnTypes).Tables("ddcFields").AsEnumerable _
                                  Select FieldID = Convert.ToInt32(f.Item("FieldID")), _
                                  SurveyID = Convert.ToInt32(f.Item("SurveyID")), _
                                  Source = f.Item("Source"), _
                                  DataTypeID = Convert.ToInt32(f.Item("DataTypeID")), _
                                  ExceptionSetID = Convert.ToInt32(f.Item("ExceptionSetID")), _
                                  MetricID = Convert.ToInt32(f.Item("MetricID")), _
                                  ScaleID = Convert.ToInt32(f.Item("ScaleID")), _
                                  UnitID = Convert.ToInt32(f.Item("UnitID")), _
                                  Description = f.Item("Description")).ToArray

                ddAttributesWS = (From f In workbook.GetDataSet("ddattributes", Data.GetDataFlags.NoColumnTypes).Tables("ddattributes").AsEnumerable _
                                   Select FieldID = Convert.ToInt32(f.Item("FieldID")), _
                                   CodeSetID = Convert.ToInt32(f.Item("CodeSetID")), _
                                   CodeID = Convert.ToInt32(f.Item("CodeID"))).ToArray

                ddAlgorithmsWS = (From f In workbook.GetDataSet("ddalgorithms", Data.GetDataFlags.NoColumnTypes).Tables("ddalgorithms").AsEnumerable _
                                     Select FieldID = Convert.ToInt32(f.Item("FieldID")), _
                                     Algorithm = f.Item("Algorithm"), _
                                     Args = f.Item("Args")).ToArray

                workbook = Nothing
            End If



            If bContinue AndAlso
               IsExistingDictionary(ddFieldsWS, progress) Then

                'TODO: add code to allow user to confirm update

                'Dim m As MsgBoxResult
                'm = MsgBox("Do you want to update existing data dictionary?", MsgBoxStyle.OkCancel)

                'If m = vbCancel Then
                '    litAbort.Text = "Import Canceled"
                '    MultiView1.SetActiveView(vwAbort)
                '    bContinue = False
                'End If



            End If

            'Make sure ddFields FK values in worksheet exist in db
            If bContinue AndAlso
               Not validateFK(ddFieldsWS) Then
                litAbort.Text = sMessage.ToString()
                MultiView1.SetActiveView(vwAbort)
                progress.PrimaryValue = 4
                progress.PrimaryPercent = 4
                bContinue = False
            End If

            'Make sure ddcFields FK values in worksheet exist in db
            If bContinue AndAlso
               Not validateFK(ddcFieldsWS) Then
                litAbort.Text = sMessage.ToString()
                MultiView1.SetActiveView(vwAbort)
                progress.PrimaryValue = 8
                progress.PrimaryPercent = 8
                bContinue = False
            End If



            If bContinue AndAlso
               Not validateAttributes(ddAttributesWS, progress) Then
                MultiView1.SetActiveView(vwAbort)
                litAbort.Text = sMessage.ToString()
                bContinue = False
            End If



            If bContinue AndAlso
               Not validateFinal(ddFieldsWS, ddcFieldsWS, ddAttributesWS, ddAlgorithmsWS, progress) Then
                MultiView1.SetActiveView(vwAbort)
                litAbort.Text = sMessage.ToString()
                bContinue = False
            End If

            'Validations were successful, import data dictionary
            If bContinue AndAlso
               ImportddFields(ddFieldsWS, progress) AndAlso
               ImportddcFields(ddcFieldsWS, progress) AndAlso
               ImportddAttributes(ddAttributesWS, progress) AndAlso
               ImportddAlgorithms(ddAlgorithmsWS) Then

                MultiView1.SetActiveView(vwConfirm)
            Else
                MultiView1.SetActiveView(vwAbort)
                litAbort.Text = sMessage.ToString()
            End If

            progress.PrimaryValue = 100
            progress.PrimaryPercent = 100


        Catch ex As Exception
            gl.NotifyWebmaster(ex, gl.GetCurrentUrl, "Import Data Dictionary Error Survey: " & litSurveyName.Text)
            MultiView1.SetActiveView(vwError)

        End Try


    End Sub


    '''<summary>
    '''gets filepath from dSurveys and web.config file, then checks if file exists
    '''</summary>
    Protected Function validateFilePath() As Boolean

        Dim query = From s In db.dSurveys _
                    Where s.SurveyID = SurveyID _
                    Select s.FolderPath

        If query.Count > 0 Then
            For Each item In query
                ddFilePath = item.ToString & ConfigurationManager.AppSettings("DataDictionaryPath").ToString
            Next

            '*********************** this conditional compilation statement 
            '***********************ensures that we are using development surveys 
            '***********************(not production)
#If DEBUG Then

            ddFilePath = ddFilePath.Replace("Sales Survey Analysis System", "SalesSurveyDev")
#End If
            '************************

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

    Protected Function IsExistingDictionary(ByVal worksheet As Array, ByRef progress As RadProgressContext) As Boolean
        progress.PrimaryValue = 4
        progress.PrimaryPercent = 4
        

        Dim FieldIDs_DB = (From f In db.ddFields _
                           Where f.SurveyID = SurveyID _
                           Select f.FieldID).ToArray



        If FieldIDs_DB.Count > 0 Then
            Return True
        Else
            Return False
        End If

    End Function

    
    Protected Function HasSurveyData(ByVal worksheet As Array, ByRef progress As RadProgressContext) As Boolean
        progress.PrimaryValue = 4
        progress.PrimaryPercent = 4
        

        'Check for existing FieldIDs in fData in database
        Dim FieldIDs_WS = (From t In worksheet _
                          Select t.FieldID).ToArray



        Dim FieldIDs_DB = (From f In db.fDatas _
                           Join ss In db.dSurveySeries _
                           On f.SurveySeriesID Equals ss.SurveySeriesID _
                           Where ss.SurveyID = SurveyID _
                           Select f.FieldID).ToArray

        Dim cF = (From x In FieldIDs_WS Where FieldIDs_DB.Contains(x) Select x).ToArray

        If cF.Count > 0 Then
            Return True
        Else
            Return False
        End If

    End Function

   

    '''<summary>
    '''validates Attributes for PK-->FK relations, then checks for existing Attributes in database
    '''</summary>
    '''<param name="worksheet">Array</param>
    '''<returns>Boolean</returns>
    Protected Function validateAttributes(ByVal worksheet As Array, ByRef progress As RadProgressContext) As Boolean
        progress.PrimaryValue = 12
        progress.PrimaryPercent = 12
        Dim wsCodeSetIDs = (From t In worksheet _
                        Select t.CodeSetID).ToArray

        Dim ddF = (From f In db.ddCodeSets _
                    Select f.CodeSetID).ToArray

        Dim eF = (From f In ddF _
                  Select f).ToArray

        Dim cF = (From x In wsCodeSetIDs Where eF.Contains(x) Select x).ToArray
        Dim uF = (wsCodeSetIDs.Except(cF)).ToArray

        If uF.Count > 0 Then
            sMessage.Append("ddAttributes Missing CodeSetID's: ")
            For Each item In uF
                sMessage.Append(item.ToString & ", ")
            Next
            Return False
        Else
            Return True
        End If

    End Function


   

    '''<summary>
    '''validates between worksheets
    '''checks that ddFields has corresponding entry in ddAttributes
    '''checks that ddcFields has corresponding entry in ddAttributes
    '''checks that ddcFields has corresponding entry in ddAlgorithms
    '''</summary>
    '''<param name="ddFieldsWS">Array</param>
    '''<param name="ddcFieldsWS">Array</param>
    '''<param name="ddAttributesWS">Array</param>
    '''<param name="ddAlgorithmsWS">Array</param>
    '''<returns>Boolean</returns>
    Protected Function validateFinal(ByVal ddFieldsWS As Array, ByVal ddcFieldsWS As Array, ByVal ddAttributesWS As Array, ByVal ddAlgorithmsWS As Array, ByRef progress As RadProgressContext) As Boolean
        progress.PrimaryValue = 20
        progress.PrimaryPercent = 20
        'ddfields ==> ddAttributes
        Dim ddF = (From df In ddFieldsWS _
                  Select df.FieldID).ToArray

        Dim ddA = (From da In ddAttributesWS _
                  Select da.FieldID).ToArray

        Dim ddAF = (From x In ddF Where ddA.Contains(x)).ToArray
        Dim ex = (ddF.Except(ddAF)).ToArray
        If ex.Count > 0 Then
            sMessage.Append("ddFields is missing corresponding fields in ddAttributes: <br />")
            For Each item In ex
                sMessage.Append(item.ToString & ", ")
            Next
            Return False
        End If

        'ddcFields ==> ddAttributes
        Dim ddcF = (From dcf In ddcFieldsWS _
                  Select dcf.FieldID).ToArray

        Dim ddcAF = (From x In ddcF Where ddA.Contains(x)).ToArray
        Dim exc = (ddcF.Except(ddcAF)).ToArray
        'If exc.Count > 0 Then
        '    sMessage.Append("ddcFields is missing corresponding fields in ddAttributes: <br />")
        '    For Each item In exc
        '        sMessage.Append(item.ToString & ", ")
        '    Next
        '    Return False
        'End If

        'ddcFields ==> ddAlgorithms

        Dim dalg = (From a In ddAlgorithmsWS _
                  Select a.FieldID).ToArray

        Dim dcfdalg = (From x In ddcF Where dalg.Contains(x)).ToArray
        Dim exa = (ddcF.Except(dcfdalg)).ToArray
        If exa.Count > 0 Then
            sMessage.Append("ddAlgorithms is missing corresponding fields in ddcFields: </br >")
            For Each aItem In exa
                sMessage.Append(aItem.ToString & ", ")
            Next
            Return False
        End If

        'ddcFields.Count = ddAlgorithms.count?

        If dalg.Count <> ddcF.Count Then
            sMessage.Append("The number of items in ddcFields does not match the number of items in ddAlgorithms.")
            Return False
        Else
            Return True
        End If

    End Function

    '''<summary>
    '''Imports ddFields from the data dictionary into the database inserting new rows, and updating existing ones.
    '''</summary>
    '''<param name="ddFieldsWS">Array</param>
    '''<return>Boolean</return>
    Private Function ImportddFields(ByVal ddFieldsWS As Array, ByRef progress As RadProgressContext) As Boolean
        progress.PrimaryValue = 40
        progress.PrimaryPercent = 40

        'Check for ddFields not in database, then insert
        Dim worksheet2 = (From t In ddFieldsWS _
                        Select t.FieldID).ToArray

        Dim ddF = (From f In db.ddFields _
                  Where f.SurveyID = SurveyID _
                    Select f.FieldID).ToArray

        Dim eF = (From f In ddF _
                  Select f).ToArray
        'cF
        Dim CrntFieldIds = (From x In worksheet2 Where eF.Contains(x) Select x).ToArray

        'uF
        Dim NewFieldIds = (worksheet2.Except(CrntFieldIds)).ToArray


            If CrntFieldIds.Count > 0 Then
                'add code to update existing fields
            Dim CrntWSFieldRows = From p In ddFieldsWS _
                                  Join o In CrntFieldIds On p.FieldID Equals o _
                                  Select p
            For Each WSFieldRow In CrntWSFieldRows
                Dim CrntFieldId As Integer = WSFieldRow.FieldID
                Dim DBFieldRow = (From s In db.ddFields _
                                  Select s _
                                  Where s.SurveyID = SurveyID AndAlso s.FieldID = CrntFieldId).Single
                DBFieldRow.SortKey = WSFieldRow.SortKey
                DBFieldRow.DataTypeID = WSFieldRow.DataTypeID
                DBFieldRow.Source = WSFieldRow.Source
                DBFieldRow.Field = WSFieldRow.Field
                DBFieldRow.xlRow = WSFieldRow.xlRow
                DBFieldRow.xlColumn = WSFieldRow.xlColumn
                DBFieldRow.xlSheet = WSFieldRow.xlSheet
                DBFieldRow.xlCell = WSFieldRow.xlCell
                DBFieldRow.ExceptionSetID = WSFieldRow.ExceptionSetID
                DBFieldRow.MetricID = WSFieldRow.MetricID
                DBFieldRow.ScaleID = WSFieldRow.ScaleID
                DBFieldRow.UnitID = WSFieldRow.UnitID
                DBFieldRow.Description = WSFieldRow.Description
            Next
                db.SubmitChanges()
            End If

            'Insert new ddFields
            If NewFieldIds.Count > 0 Then
                Dim cFields = (From w In ddFieldsWS _
                                 Join f In NewFieldIds On w.FieldID Equals f _
                                 Select w).ToArray
                For Each item In cFields
                    Dim cFlds As New ddField With {.FieldID = item.FieldID, _
                                                    .SortKey = item.SortKey, _
                                                    .SurveyID = item.SurveyID, _
                                                    .Source = item.Source, _
                                                    .Field = item.Field, _
                                                    .xlRow = item.xlRow, _
                                                    .xlColumn = item.xlColumn, _
                                                    .xlSheet = item.xlSheet, _
                                                    .xlCell = item.xlCell, _
                                                    .DataTypeID = item.DataTypeID, _
                                                    .ExceptionSetID = item.ExceptionSetID, _
                                                    .MetricID = item.MetricID, _
                                                    .ScaleID = item.ScaleID, _
                                                    .UnitID = item.UnitID, _
                                                    .Description = item.Description}

                    db.ddFields.InsertOnSubmit(cFlds)
                Next
                db.SubmitChanges()
            End If

            Return True
    End Function

    '''<summary>
    '''Imports ddcFields from the data dictionary into the database inserting new rows, and updating existing ones.
    '''</summary>
    '''<param name="ddcFieldsWS">Array</param>
    '''<return>Boolean</return>
    Private Function ImportddcFields(ByVal ddcFieldsWS As Array, ByRef progress As RadProgressContext) As Boolean
        progress.PrimaryValue = 60
        progress.PrimaryPercent = 60
        Dim uFieldID As Integer
        'Check for ddcFields not in database, then insert
        Dim worksheet2 = (From t In ddcFieldsWS _
                        Select t.FieldID).ToArray

        Dim ddF = (From f In db.ddcFields _
                  Where f.SurveyID = SurveyID _
                  Select f.FieldID).ToArray

        Dim eF = (From f In ddF _
                  Select f).ToArray

        Dim cF = (From x In worksheet2 Where eF.Contains(x) Select x).ToArray
        Dim uF = (worksheet2.Except(cF)).ToArray
        If cF.Count > 0 Then
            'add code to update existing fields
            Dim uFields = (From p In ddcFieldsWS _
                          Join o In cF On p.FieldID Equals o _
                          Select p).ToArray

            For Each nItem In uFields
                uFieldID = nItem.FieldID
                Dim nFields = (From s In db.ddcFields _
                                Select s _
                                Where s.SurveyID = SurveyID AndAlso s.FieldID = uFieldID).Single

                nFields.DataTypeID = nItem.DataTypeID
                nFields.Source = nItem.Source
                nFields.ExceptionSetID = nItem.ExceptionSetID
                nFields.MetricID = nItem.MetricID
                nFields.ScaleID = nItem.ScaleID
                nFields.UnitID = nItem.UnitID
                nFields.Description = nItem.Description
            Next
            db.SubmitChanges()

        End If

        'Insert new ddcFields
        If uF.Count > 0 Then
            Dim cFields = (From w In ddcFieldsWS _
                             Join f In uF On w.FieldID Equals f _
                             Select w).ToArray
            For Each item In cFields
                Dim cFlds As New ddcField With {.FieldID = item.FieldID, _
                                                .SurveyID = item.SurveyID, _
                                                .Source = item.Source, _
                                                .DataTypeID = item.DataTypeID, _
                                                .ExceptionSetID = item.ExceptionSetID, _
                                                .MetricID = item.MetricID, _
                                                .ScaleID = item.ScaleID, _
                                                .UnitID = item.UnitID, _
                                                .Description = item.Description}

                db.ddcFields.InsertOnSubmit(cFlds)
            Next
            db.SubmitChanges()
        End If

        Return True
    End Function

    '<summary>
    'Imports ddAttributes from the data dictionary into the database inserting new rows, and updating existing ones.
    '</summary>
    '<param name="ddAttributesWS">Array</param>
    '<return>Boolean</return>
    Private Function ImportddAttributes(ByVal ddAttributesWS As Array, ByRef progress As RadProgressContext) As Boolean
        progress.PrimaryValue = 80
        progress.PrimaryPercent = 80
        Dim uFieldID As Integer
        Dim uCodeSetID As Integer

        Dim worksheet2 = (From t In ddAttributesWS _
                   Select t.FieldID).ToArray

        Dim ddF = (From f In db.ddAttributes _
                  Where f.SurveyID = SurveyID _
                    Select f.FieldID).ToArray

        Dim eF = (From f In ddF _
                  Select f).ToArray

        Dim cF = (From x In worksheet2 Where eF.Contains(x) Select x).ToArray
        'lists attributes not in database
        Dim uF = (worksheet2.Except(cF)).ToArray

        'Update existing Attributes
        If cF.Count > 0 Then
            Dim uAttributes = (From w In ddAttributesWS _
                            Join f In cF On w.FieldID Equals f _
                            Select w).ToArray

            For Each cItem In uAttributes
                uFieldID = cItem.FieldID
                uCodeSetID = cItem.CodeSetID
                Dim nAtt = (From t In db.ddAttributes _
                           Where t.SurveyID = SurveyID AndAlso t.FieldID = uFieldID AndAlso t.CodeSetID = uCodeSetID).Single
                nAtt.CodeSetID = cItem.CodeSetID
                nAtt.CodeID = cItem.CodeID
            Next
            db.SubmitChanges()
        End If

        'Insert new attributes
        If uF.Count > 0 Then

            Dim Attributes = (From w In ddAttributesWS _
                             Join f In uF On w.FieldID Equals f _
                             Select w).ToArray
            For Each item In Attributes

                Dim atrbt As New ddAttribute With {.FieldID = item.FieldID, _
                                                   .CodeID = item.CodeID, _
                                                   .CodeSetID = item.CodeSetID, _
                                                   .SurveyID = SurveyID}

                db.ddAttributes.InsertOnSubmit(atrbt)
            Next
            db.SubmitChanges()
        End If

        Return True
    End Function


    '''<summary>
    '''Imports ddAttributes from the data dictionary into the database inserting new rows, and updating existing ones.
    '''</summary>
    '''<param name="ddAlgorithmsWS">Array</param>
    '''<return>Boolean</return>
    Private Function ImportddAlgorithms(ByVal ddAlgorithmsWS As Array) As Boolean

        Dim uFieldID As Integer
        Dim worksheet3 = (From t In ddAlgorithmsWS _
                                     Select t.FieldID).ToArray

        Dim ddFA = (From f In db.ddAlgorithms _
                  Where f.SurveyID = SurveyID _
                  Select f.FieldID).ToArray


        Dim eFA = (From f In ddFA _
                  Select f).ToArray

        Dim cFA = (From x In worksheet3 Where eFA.Contains(x) Select x).ToArray
        Dim uFA = (worksheet3.Except(cFA)).ToArray
        If cFA.Count > 0 Then
            Dim uAlgorithms = (From w In ddAlgorithmsWS _
                           Join f In cFA On w.FieldID Equals f _
                           Select w).ToArray

            For Each cItem In uAlgorithms
                uFieldID = cItem.FieldID
                Dim nAlg = (From t In db.ddAlgorithms _
                           Where t.SurveyID = SurveyID AndAlso t.FieldID = uFieldID).Single
                nAlg.Algorithm = cItem.Algorithm
                nAlg.Args = cItem.Args
            Next
            db.SubmitChanges()
        End If

        'Insert new algorithms
        If uFA.Count > 0 Then
            Dim Algorithms = (From w In ddAlgorithmsWS _
                             Join f In uFA On w.FieldID Equals f _
                             Select w).ToArray
            For Each item In Algorithms
                Dim algs As New ddAlgorithm With {.Algorithm = item.Algorithm, _
                                                  .Args = item.Args, _
                                                  .FieldID = item.FieldID, _
                                                  .SurveyID = SurveyID}
                db.ddAlgorithms.InsertOnSubmit(algs)
            Next
            db.SubmitChanges()
        End If
        Return True
    End Function

    '''<summary>
    '''performs PK==>FK relationship validations for ddFields or ddcFields - data structure is the same
    '''</summary>
    '''<param name="worksheet">Array</param>
    '''<returns>Boolean</returns>
    Protected Function validateFK(ByVal worksheet As Array) As Boolean

        Dim FKValidate As Boolean = True

        'Check DataTypeID for PF-FK Relationship
        Dim worksheet2 = (From t In worksheet _
                        Select t.DataTypeID).Distinct

        Dim ddF = From f In db.ddDataTypes _
                    Select f.DataTypeID

        Dim eF = (From f In ddF _
                  Select f).ToArray

        Dim cF = (From x In worksheet2 Where eF.Contains(x) Select x).ToArray
        Dim uF = (worksheet2.Except(cF)).ToArray
        If uF.Count > 0 Then
            FKValidate = False
            sMessage.Append("<br />DataTypeIDs not in ddDataTypes table: ")
            For Each item In uF
                sMessage.Append(item.ToString & ", ")
            Next
        End If

        'Check ExceptionSetID PK-FK Relationship

        worksheet2 = (From t In worksheet _
                     Select t.ExceptionSetID).Distinct

        ddF = From e In db.ddExceptionSets _
              Select e.ExceptionSetID

        eF = (From f In ddF _
              Select f).ToArray

        cF = (From x In worksheet2 Where eF.Contains(x) Select x).ToArray
        uF = (worksheet2.Except(cF)).ToArray
        If uF.Count > 0 Then
            FKValidate = False
            sMessage.Append("<br />ExceptionSetIDs not in ddExceptionSets table: ")
            For Each item In uF
                sMessage.Append(item.ToString & ", ")
            Next
        End If

        'Check MetricID PK-FK RelationShip

        worksheet2 = (From t In worksheet _
                     Select t.MetricID).Distinct

        ddF = From e In db.ddMetrics _
              Select e.MetricID

        eF = (From f In ddF _
              Select f).ToArray

        cF = (From x In worksheet2 Where eF.Contains(x) Select x).ToArray
        uF = (worksheet2.Except(cF)).ToArray

        If uF.Count > 0 Then
            FKValidate = False
            sMessage.Append("<br />MetricIDs not in ddMetrics table: ")
            For Each item In uF
                sMessage.Append(item.ToString & ", ")
            Next
        End If

        'Check ScaleID PK-FK RelationShip

        worksheet2 = (From t In worksheet _
                  Select t.ScaleID).Distinct

        ddF = From e In db.ddScales _
              Select e.ScaleID

        eF = (From f In ddF _
              Select f).ToArray

        cF = (From x In worksheet2 Where eF.Contains(x) Select x).ToArray
        uF = (worksheet2.Except(cF)).ToArray

        If uF.Count > 0 Then
            FKValidate = False
            sMessage.Append("<br />ScaleIDs not in ddScales table: ")
            For Each item In uF
                sMessage.Append(item.ToString & ", ")
            Next
        End If

        'Check UnitID PK-FK RelationShip

        worksheet2 = (From t In worksheet _
                  Select t.UnitID).Distinct

        ddF = From e In db.ddUnits _
              Select e.UnitID

        eF = (From f In ddF _
              Select f).ToArray

        cF = (From x In worksheet2 Where eF.Contains(x) Select x).ToArray
        uF = (worksheet2.Except(cF)).ToArray

        If uF.Count > 0 Then
            FKValidate = False
            sMessage.Append("<br />UnitIDs not in ddUnits table: ")
            For Each item In uF
                sMessage.Append(item.ToString & ", ")
            Next
        End If

        Return FKValidate

    End Function


    Private Sub displayMessageBox(ByVal msg As String)

        ' define a javascript alertbox containing
        ' the string passed in as argument

        ' create a new label
        Dim lbl As New Label()

        ' add the javascript to fire an alertbox to the label and
        ' add the string argument passed to the subroutine as the
        ' message payload for the alertbox
        lbl.Text = "<script language='javascript'>" & Environment.NewLine & _
                   "return confirm('" + msg + "');</script>"

        ' add the label to the page to display the alertbox
        Page.Controls.Add(lbl)

    End Sub

End Class