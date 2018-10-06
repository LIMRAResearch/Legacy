' Add reference to Microsoft Office 12.0 Object Library aka Microsoft.Office.Core via project>add reference>COM
' Add reference to Microsoft Excel 12.0 Object Library aka Microsoft.Office.Interop.Excel via project>add reference>COM
' Add reference to Microsoft.Office.Tools.Excel via project>add reference>.NET
' Add reference for Microsoft.VisualStudio.Tools.Applications.Runtime via project>add reference>.NET

Imports System.Data.SqlClient
Imports System.Data
Imports System.Configuration
Imports System.Diagnostics
Imports Microsoft.VisualStudio.Tools.Applications.Runtime
Imports Office = Microsoft.Office.Core
Imports Excel = Microsoft.Office.Interop.Excel
Imports Microsoft.Office.Tools.Excel
Imports System.Net.Mail
Imports System.Text
Imports Telerik.Web.UI
Imports Telerik.Web.UI.Upload

Imports SalesSurveysApplication.Globals
Imports SalesSurveysApplication.Projects

Namespace Projects

    Public Class ProjectVariables

    End Class

    Public Class ProjectFunctions
        'TODO RP create global constant for textbox style classes to indicate valid/invalid

        Protected gF As Globals.GlobalFunctions = New Globals.GlobalFunctions
        Protected gV As Globals.GlobalVariables = New Globals.GlobalVariables
        Private Const INPROC_STATUS As String = "In Process"

        Public Function getNewlySubmittedWorkbooks(ByVal SurveyID As Integer) As IEnumerable(Of String)
            Dim files As IEnumerable(Of String)
            Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext()
            Dim strSurveyFilePath As String = ""

            Try
                strSurveyFilePath = getSurveyFilePath(SurveyID)

                files = gF.getXLWBDirectory(strSurveyFilePath & "Data\1-Formatting")
                files = files.Union(gF.getXLWBDirectory(strSurveyFilePath & "Data\2-Validating"))
                files = files.Union(gF.getXLWBDirectory(strSurveyFilePath & "Data\3-Completed"))

                'get list of files that are currently in a batch process
                Dim inproc = (From id In db.ImportDetails
                              Where id.ImportResult = INPROC_STATUS
                              Select id.WorkbookName)
                files = files.Union(inproc)

                getNewlySubmittedWorkbooks = (gF.getXLWBDirectory(strSurveyFilePath & "Data\0-Submitted")).Except(files)


            Catch ex As Exception
                getNewlySubmittedWorkbooks = Nothing
            End Try


        End Function

        Public Function getDataDictionary() As System.Data.Linq.Table(Of DataDictionary)
            Try
                Dim db As DataDictionaryDataContext = New DataDictionaryDataContext()

                Dim dd = db.DataDictionaries
                getDataDictionary = dd
            Catch ex As Exception
                getDataDictionary = Nothing
            End Try

        End Function

        ''' <summary>
        ''' Replaces getting data from datadictionary view - we only need data from ddFields
        ''' </summary>
        ''' <param name="thisSurveyID">Integer</param>
        ''' <returns>Array</returns>
        ''' <remarks></remarks>
        Public Function getDataDictionary(ByVal thisSurveyID As Integer) As Array
            Try
                Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext
                Dim dd = (From ddF In db.ddFields Join ddt In db.ddDataTypes On ddF.DataTypeID Equals ddt.DataTypeID _
                         Join ddM In db.ddMetrics On ddF.MetricID Equals ddM.MetricID _
                         Join ddU In db.ddUnits On ddF.UnitID Equals ddU.UnitID _
                         Join ddS In db.ddScales On ddF.ScaleID Equals ddS.ScaleID _
                        Where ddF.SurveyID = thisSurveyID _
                         Order By ddF.SortKey _
                         Select ddF.SurveyID, ddF.FieldID, ddF.Field, ddF.SortKey, ddF.xlSheet, _
                         ddF.xlRow, ddF.xlColumn, ddF.Source, ddF.Description, ddt.DataType, _
                         ddM.Metric, ddU.Unit, ddS.Scale).ToArray
                getDataDictionary = dd
            Catch ex As Exception
                getDataDictionary = Nothing
            End Try
        End Function



        ''  replaces the datadictionary view in the database
        'Public Function getDataDictionary(ByVal Survey As Integer) As Array
        '    Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext
        '    '### uncomment when adding more surveys
        '    'Select Case Survey
        '    '    Case 1

        '    '    Case 2
        '    Dim qChannel As IEnumerable
        '    Dim qMarket As IEnumerable
        '    Dim qProduct As IEnumerable
        '    Dim qDD As IEnumerable

        '    qChannel = From qc In db.ddAttributes, c In db.ddCodes _
        '         Where qc.CodeSetID = 2 AndAlso qc.CodeID = c.CodeID _
        '         Select qc.FieldID, c.Text

        '    qMarket = From qm In db.ddAttributes, c In db.ddCodes _
        '         Where qm.CodeSetID = 1 AndAlso qm.CodeID = c.CodeID _
        '         Select qm.FieldID, c.Text

        '    qProduct = From qp In db.ddAttributes, c In db.ddCodes _
        '       Where qp.CodeSetID = 3 AndAlso qp.CodeID = c.CodeID _
        '       Select qp.FieldID, c.Text

        '    qDD = From a In qChannel _
        '          Join b In qMarket On a.FieldID Equals b.FieldID _
        '          Join d In qProduct On a.FieldID Equals d.FieldID _
        '    Select FieldID = CInt(a.FieldID), Channel = CStr(a.text), Market = (b.text), Product = (d.text)

        '    Dim dF = (From f In db.ddFields _
        '    Where f.SurveyID = Survey _
        '    Select _
        '     f.SurveyID, _
        '     f.FieldID, _
        '     f.Field, _
        '     f.SortKey, _
        '     f.xlSheet, _
        '     f.xlRow, _
        '     f.xlColumn, _
        '     f.Source, _
        '     f.Description, _
        '     f.ddDataType.DataType, _
        '     f.ddMetric.Metric, _
        '     f.ddScale.Scale, _
        '     f.ddUnit.Unit).ToArray

        '    Dim dD = (From a In qDD, _
        '    f In dF Where a.FieldID = f.FieldID _
        '    Order By f.SortKey Ascending _
        '    Select f.SurveyID, _
        '    f.FieldID, _
        '    f.Field, _
        '    f.SortKey, _
        '    f.xlSheet, _
        '    f.xlRow, _
        '    f.xlColumn, _
        '    f.Source, _
        '    f.Description, _
        '    f.DataType, _
        '    f.Metric, _
        '    f.Scale, _
        '    f.Unit, _
        '    Market = CStr(a.Market), _
        '    Channel = CStr(a.Channel), _
        '    Product = (a.Product)).ToArray

        '    Return dD
        '    '    Case 999

        '    'End Select



        'End Function

        Public Function getSurveyFilePath(ByVal SurveyID As Integer) As String
            ' Get survey's file path 
            Dim strSurveyFilePath As String = ""

            Try

                Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext()

                Dim query = From paths In db.dSurveys _
                            Where paths.SurveyID = SurveyID _
                            Select paths.FolderPath

                If query.Count = 1 Then
                    strSurveyFilePath = query.First.ToString

                    If (Not strSurveyFilePath.EndsWith("\")) And (Not strSurveyFilePath.EndsWith("/")) Then
                        strSurveyFilePath = strSurveyFilePath & "\"
                    End If

                    '*********************** this conditional compilation statement 
                    '***********************ensures that we are using development surveys 
                    '***********************(not production)
#If DEBUG Then

                    strSurveyFilePath = strSurveyFilePath.Replace("Sales Survey Analysis System", "SalesSurveyDev")
#End If

                    '**************************



                    getSurveyFilePath = strSurveyFilePath
                Else
                    getSurveyFilePath = ""
                    'throw exception needs to added
                End If


            Catch ex As Exception
                getSurveyFilePath = ""
            End Try

        End Function

        Public Function getSurveySeriesID(ByVal SurveyID As Integer, ByVal DateID As Date) As Integer


            Dim theSurveySeriesID As Integer = 0

            Try
                Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext()

                Dim query = From sID In db.dSurveySeries _
                            Where sID.SurveyID = SurveyID _
                              And sID.DateID = DateID _
                            Select sID.SurveySeriesID

                If query.Count = 1 Then
                    theSurveySeriesID = query.First
                Else
                    theSurveySeriesID = 0
                    'throw exception needs to be added
                End If

                getSurveySeriesID = theSurveySeriesID

            Catch ex As Exception
                getSurveySeriesID = 0
            End Try

        End Function

        Public Function isWorkbookInDatabase(ByVal sWBName As String, ByVal dtDateID As Date) As Boolean

            isWorkbookInDatabase = True

            Try
                Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext()

                Dim query = From rWB In db.dWorkbooks _
                            Where rWB.WorkbookName = sWBName _
                                And rWB.DateID = dtDateID

                If query.Count = 0 Then
                    isWorkbookInDatabase = False
                Else
                    isWorkbookInDatabase = True

                End If

            Catch ex As Exception
                isWorkbookInDatabase = True
            End Try

        End Function


        Public Function verifySourceID(ByVal SourceID As Integer) As Boolean

            Try
                Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext()
                Dim query = From sID In db.dSources _
                            Where sID.SourceID = SourceID

                Return query.Count > 0

            Catch ex As Exception
                Return False
            End Try

        End Function

        'Public Function verifyRespondentID(ByVal RespondentID As Integer) As Boolean

        'verifyRespondentID = False

        'Try
        '    Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext()

        '    Dim query = From rID In db.dRespondents _
        '                Where rID.RespondentID = RespondentID

        '    If query.Count = 1 Then
        '        verifyRespondentID = True
        '    Else
        '        verifyRespondentID = False
        '        'throw exception needs to be added
        '    End If

        'Catch ex As Exception
        '    verifyRespondentID = False
        'End Try

        ' End Function

    End Class

    Public Class Survey1Functions

        'Overloaded function for validating subtotals from the corrections page form - passes in the textbox by reference
        'If false, sets the background of the textbox to yellow, if true sets to white
        Public Function CheckSubTotals6(ByVal Value1 As String, ByVal Value2 As String, ByVal Value3 As String, _
                                       ByVal Value4 As String, ByVal Value5 As String, ByVal Value6 As String, ByVal SubTotal As String, ByRef SubTotalTextBox As TextBox) As Boolean
            Dim Number1 As Double = 0
            Dim Number2 As Double = 0
            Dim Number3 As Double = 0
            Dim Number4 As Double = 0
            Dim Number5 As Double = 0
            Dim Number6 As Double = 0
            Dim Total As Double = 0
            Dim isNumber As Boolean = False
            Dim isEmpty As Boolean = False
            Dim isM As Boolean = False
            Dim thisTotal As Double = 0

            'Check for M

            If Value1 = "M" AndAlso Value2 = "M" AndAlso Value3 = "M" AndAlso Value4 = "M" AndAlso Value5 = "M" AndAlso Value6 = "M" Then
                If SubTotal = "M" OrElse Double.TryParse(SubTotal, Total) Then
                    SubTotalTextBox.CssClass = "TextRight"
                    Return True
                Else
                    SubTotalTextBox.CssClass = "TextRight Yellow"
                    Return False
                End If
            End If

            'check for all numbers, then check if they total up
            If Double.TryParse(Value1, Number1) AndAlso Double.TryParse(Value2, Number2) AndAlso Double.TryParse(Value3, Number3) AndAlso Double.TryParse(Value4, Number4) AndAlso Double.TryParse(Value5, Number5) AndAlso Double.TryParse(Value6, Number6) AndAlso Double.TryParse(SubTotal, Total) Then
                If (Total < ((Number1 + Number2 + Number3 + Number4 + Number5 + Number6) - 2)) Or _
                   (Total > ((Number1 + Number2 + Number3 + Number4 + Number5 + Number6) + 2)) Then
                    SubTotalTextBox.CssClass = "TextRight Yellow"
                    Return False
                Else
                    SubTotalTextBox.CssClass = "TextRight"
                    Return True
                End If
            End If


            'Checks for all empty strings, then checks if the subtotal has an empty string
            If String.IsNullOrEmpty(Value1) AndAlso String.IsNullOrEmpty(Value2) AndAlso String.IsNullOrEmpty(Value3) AndAlso String.IsNullOrEmpty(Value4) AndAlso String.IsNullOrEmpty(Value5) AndAlso String.IsNullOrEmpty(Value6) Then
                If String.IsNullOrEmpty(SubTotal) Then
                    SubTotalTextBox.CssClass = "TextRight"
                    Return True
                Else
                    SubTotalTextBox.CssClass = "TextRight Yellow"
                    Return False
                End If
            End If

            'Checks for the existence of 1 M, and checks if the sub total is also an M
            If Value1 = "M" OrElse Value2 = "M" OrElse Value3 = "M" OrElse Value4 = "M" OrElse Value5 = "M" OrElse Value6 = "M" Then
                If SubTotal = "M" Then
                    SubTotalTextBox.CssClass = "TextRight"
                    Return True
                Else
                    SubTotalTextBox.CssClass = "TextRight Yellow"
                    Return False
                End If
            End If

            'Tries to convert to double, then checks totals
            Double.TryParse(Value1, Number1)
            Double.TryParse(Value2, Number2)
            Double.TryParse(Value3, Number3)
            Double.TryParse(Value4, Number4)
            Double.TryParse(Value5, Number5)
            Double.TryParse(Value6, Number6)
            Double.TryParse(SubTotal, Total)

            If (Total < ((Number1 + Number2 + Number3 + Number4 + Number5 + Number6) - 2)) Or (Total > ((Number1 + Number2 + Number3 + Number4 + Number5 + Number6) + 2)) Then
                SubTotalTextBox.CssClass = "TextRight Yellow"
                Return False
            Else
                SubTotalTextBox.CssClass = "TextRight"
                Return True
            End If
        End Function

        'Takes in data point values, and validates subtotal
        Public Function CheckSubTotals6(ByVal Value1 As String, ByVal Value2 As String, ByVal Value3 As String, _
                                      ByVal Value4 As String, ByVal Value5 As String, ByVal Value6 As String, ByVal SubTotal As String) As Boolean
            Dim Number1 As Double = 0
            Dim Number2 As Double = 0
            Dim Number3 As Double = 0
            Dim Number4 As Double = 0
            Dim Number5 As Double = 0
            Dim Number6 As Double = 0
            Dim Total As Double = 0
            Dim isNumber As Boolean = False
            Dim isEmpty As Boolean = False
            Dim isM As Boolean = False
            Dim thisTotal As Double = 0

            'Check for M

            If Value1 = "M" AndAlso Value2 = "M" AndAlso Value3 = "M" AndAlso Value4 = "M" AndAlso Value5 = "M" AndAlso Value6 = "M" Then
                If SubTotal = "M" OrElse Double.TryParse(SubTotal, Total) Then
                    Return True
                Else
                    Return False
                End If
            End If

            'check for all numbers, then check if they total up
            If Double.TryParse(Value1, Number1) AndAlso Double.TryParse(Value2, Number2) AndAlso Double.TryParse(Value3, Number3) AndAlso Double.TryParse(Value4, Number4) AndAlso Double.TryParse(Value5, Number5) AndAlso Double.TryParse(Value6, Number6) AndAlso Double.TryParse(SubTotal, Total) Then
                If (Total < ((Number1 + Number2 + Number3 + Number4 + Number5 + Number6) - 2)) Or _
                   (Total > ((Number1 + Number2 + Number3 + Number4 + Number5 + Number6) + 2)) Then
                    Return False
                Else
                    Return True
                End If
            End If


            'Checks for all empty strings, then checks if the subtotal has an empty string
            If String.IsNullOrEmpty(Value1) AndAlso String.IsNullOrEmpty(Value2) AndAlso String.IsNullOrEmpty(Value3) AndAlso String.IsNullOrEmpty(Value4) AndAlso String.IsNullOrEmpty(Value5) AndAlso String.IsNullOrEmpty(Value6) Then
                If String.IsNullOrEmpty(SubTotal) Then
                    Return True
                Else
                    Return False
                End If
            End If

            'Checks for the existence of 1 M, and checks if the sub total is also an M
            If Value1 = "M" OrElse Value2 = "M" OrElse Value3 = "M" OrElse Value4 = "M" OrElse Value5 = "M" OrElse Value6 = "M" Then
                If SubTotal = "M" Then
                    Return True
                Else
                    Return False
                End If
            End If

            'Tries to convert to double, then checks totals
            Double.TryParse(Value1, Number1)
            Double.TryParse(Value2, Number2)
            Double.TryParse(Value3, Number3)
            Double.TryParse(Value4, Number4)
            Double.TryParse(Value5, Number5)
            Double.TryParse(Value6, Number6)
            Double.TryParse(SubTotal, Total)

            If (Total < ((Number1 + Number2 + Number3 + Number4 + Number5 + Number6) - 2)) Or _
               (Total > ((Number1 + Number2 + Number3 + Number4 + Number5 + Number6) + 2)) Then
                Return False
            Else
                Return True
            End If


        End Function

        'Overloaded function for validating subtotals from the corrections page form - passes in the textbox by reference
        'If false, sets the background of the textbox to yellow, if true sets to white
        Public Function CheckSubTotals5(ByVal Value1 As String, ByVal Value2 As String, ByVal Value3 As String, _
                                       ByVal Value4 As String, ByVal Value5 As String, ByVal SubTotal As String, ByRef SubTotalTextBox As TextBox) As Boolean
            Dim Number1 As Double = 0
            Dim Number2 As Double = 0
            Dim Number3 As Double = 0
            Dim Number4 As Double = 0
            Dim Number5 As Double = 0
            Dim Total As Double = 0
            Dim isNumber As Boolean = False
            Dim isEmpty As Boolean = False
            Dim isM As Boolean = False
            Dim thisTotal As Double = 0

            'Check for M

            If Value1 = "M" AndAlso Value2 = "M" AndAlso Value3 = "M" AndAlso Value4 = "M" AndAlso Value5 = "M" Then
                If SubTotal = "M" OrElse Double.TryParse(SubTotal, Total) Then
                    SubTotalTextBox.CssClass = "TextRight"
                    Return True
                Else
                    SubTotalTextBox.CssClass = "TextRight Yellow"
                    Return False
                End If
            End If

            'check for all numbers, then check if they total up
            If Double.TryParse(Value1, Number1) AndAlso Double.TryParse(Value2, Number2) AndAlso Double.TryParse(Value3, Number3) AndAlso Double.TryParse(Value4, Number4) AndAlso Double.TryParse(Value5, Number5) AndAlso Double.TryParse(SubTotal, Total) Then
                If (Total < ((Number1 + Number2 + Number3 + Number4 + Number5) - 2)) Or _
                   (Total > ((Number1 + Number2 + Number3 + Number4 + Number5) + 2)) Then
                    SubTotalTextBox.CssClass = "TextRight Yellow"
                    Return False
                Else
                    SubTotalTextBox.CssClass = "TextRight"
                    Return True
                End If
            End If


            'Checks for all empty strings, then checks if the subtotal has an empty string
            If String.IsNullOrEmpty(Value1) AndAlso String.IsNullOrEmpty(Value2) AndAlso String.IsNullOrEmpty(Value3) AndAlso String.IsNullOrEmpty(Value4) AndAlso String.IsNullOrEmpty(Value5) Then
                If String.IsNullOrEmpty(SubTotal) Then
                    SubTotalTextBox.CssClass = "TextRight"
                    Return True
                Else
                    SubTotalTextBox.CssClass = "TextRight Yellow"
                    Return False
                End If
            End If

            'Checks for the existence of 1 M, and checks if the sub total is also an M
            If Value1 = "M" OrElse Value2 = "M" OrElse Value3 = "M" OrElse Value4 = "M" OrElse Value5 = "M" Then
                If SubTotal = "M" Then
                    SubTotalTextBox.CssClass = "TextRight"
                    Return True
                Else
                    SubTotalTextBox.CssClass = "TextRight Yellow"
                    Return False
                End If
            End If

            'Tries to convert to double, then checks totals
            Double.TryParse(Value1, Number1)
            Double.TryParse(Value2, Number2)
            Double.TryParse(Value3, Number3)
            Double.TryParse(Value4, Number4)
            Double.TryParse(Value5, Number5)
            Double.TryParse(SubTotal, Total)

            If (Total < ((Number1 + Number2 + Number3 + Number4 + Number5) - 2)) Or (Total > ((Number1 + Number2 + Number3 + Number4 + Number5) + 2)) Then
                SubTotalTextBox.CssClass = "TextRight Yellow"
                Return False
            Else
                SubTotalTextBox.CssClass = "TextRight"
                Return True
            End If
        End Function

        'Takes in data point values, and validates subtotal
        Public Function CheckSubTotals5(ByVal Value1 As String, ByVal Value2 As String, ByVal Value3 As String, _
                                      ByVal Value4 As String, ByVal Value5 As String, ByVal SubTotal As String) As Boolean
            Dim Number1 As Double = 0
            Dim Number2 As Double = 0
            Dim Number3 As Double = 0
            Dim Number4 As Double = 0
            Dim Number5 As Double = 0
            Dim Total As Double = 0
            Dim isNumber As Boolean = False
            Dim isEmpty As Boolean = False
            Dim isM As Boolean = False
            Dim thisTotal As Double = 0

            'Check for M

            If Value1 = "M" AndAlso Value2 = "M" AndAlso Value3 = "M" AndAlso Value4 = "M" AndAlso Value5 = "M" Then
                If SubTotal = "M" OrElse Double.TryParse(SubTotal, Total) Then
                    Return True
                Else
                    Return False
                End If
            End If

            'check for all numbers, then check if they total up
            If Double.TryParse(Value1, Number1) AndAlso Double.TryParse(Value2, Number2) AndAlso Double.TryParse(Value3, Number3) AndAlso Double.TryParse(Value4, Number4) AndAlso Double.TryParse(Value5, Number5) AndAlso Double.TryParse(SubTotal, Total) Then
                If (Total < ((Number1 + Number2 + Number3 + Number4 + Number5) - 2)) Or _
                   (Total > ((Number1 + Number2 + Number3 + Number4 + Number5) + 2)) Then
                    Return False
                Else
                    Return True
                End If
            End If


            'Checks for all empty strings, then checks if the subtotal has an empty string
            If String.IsNullOrEmpty(Value1) AndAlso String.IsNullOrEmpty(Value2) AndAlso String.IsNullOrEmpty(Value3) AndAlso String.IsNullOrEmpty(Value4) AndAlso String.IsNullOrEmpty(Value5) Then
                If String.IsNullOrEmpty(SubTotal) Then
                    Return True
                Else
                    Return False
                End If
            End If

            'Checks for the existence of 1 M, and checks if the sub total is also an M
            If Value1 = "M" OrElse Value2 = "M" OrElse Value3 = "M" OrElse Value4 = "M" OrElse Value5 = "M" Then
                If SubTotal = "M" Then
                    Return True
                Else
                    Return False
                End If
            End If

            'Tries to convert to double, then checks totals
            Double.TryParse(Value1, Number1)
            Double.TryParse(Value2, Number2)
            Double.TryParse(Value3, Number3)
            Double.TryParse(Value4, Number4)
            Double.TryParse(Value5, Number5)
            Double.TryParse(SubTotal, Total)

            If (Total < ((Number1 + Number2 + Number3 + Number4 + Number5) - 2)) Or _
               (Total > ((Number1 + Number2 + Number3 + Number4 + Number5) + 2)) Then
                Return False
            Else
                Return True
            End If


        End Function
        'Overloaded function for validating subtotals from the corrections page form - passes in the textbox by reference
        'If false, sets the background of the textbox to yellow, if true sets to white
        Public Function CheckSubTotals4(ByVal Value1 As String, ByVal Value2 As String, ByVal Value3 As String, _
                                       ByVal Value4 As String, ByVal SubTotal As String, ByRef SubTotalTextBox As TextBox) As Boolean
            Dim Number1 As Double = 0
            Dim Number2 As Double = 0
            Dim Number3 As Double = 0
            Dim Number4 As Double = 0
            Dim Total As Double = 0
            Dim isNumber As Boolean = False
            Dim isEmpty As Boolean = False
            Dim isM As Boolean = False
            Dim thisTotal As Double = 0

            'Check for M

            If Value1 = "M" AndAlso Value2 = "M" AndAlso Value3 = "M" AndAlso Value4 = "M" Then
                If SubTotal = "M" OrElse Double.TryParse(SubTotal, Total) Then
                    SubTotalTextBox.CssClass = "TextRight"
                    Return True
                Else
                    SubTotalTextBox.CssClass = "TextRight Yellow"
                    Return False
                End If
            End If

            'check for all numbers, then check if they total up
            If Double.TryParse(Value1, Number1) AndAlso Double.TryParse(Value2, Number2) AndAlso Double.TryParse(Value3, Number3) AndAlso Double.TryParse(Value4, Number4) AndAlso Double.TryParse(SubTotal, Total) Then
                If (Total < ((Number1 + Number2 + Number3 + Number4) - 2)) Or _
                   (Total > ((Number1 + Number2 + Number3 + Number4) + 2)) Then
                    SubTotalTextBox.CssClass = "TextRight Yellow"
                    Return False
                Else
                    SubTotalTextBox.CssClass = "TextRight"
                    Return True
                End If
            End If


            'Checks for all empty strings, then checks if the subtotal has an empty string
            If String.IsNullOrEmpty(Value1) AndAlso String.IsNullOrEmpty(Value2) AndAlso String.IsNullOrEmpty(Value3) AndAlso String.IsNullOrEmpty(Value4) Then
                If String.IsNullOrEmpty(SubTotal) Then
                    SubTotalTextBox.CssClass = "TextRight"
                    Return True
                Else
                    SubTotalTextBox.CssClass = "TextRight Yellow"
                    Return False
                End If
            End If

            'Checks for the existence of 1 M, and checks if the sub total is also an M
            If Value1 = "M" OrElse Value2 = "M" OrElse Value3 = "M" OrElse Value4 = "M" Then
                If SubTotal = "M" Then
                    SubTotalTextBox.CssClass = "TextRight"
                    Return True
                Else
                    SubTotalTextBox.CssClass = "TextRight Yellow"
                    Return False
                End If
            End If

            'Tries to convert to double, then checks totals
            Double.TryParse(Value1, Number1)
            Double.TryParse(Value2, Number2)
            Double.TryParse(Value3, Number3)
            Double.TryParse(Value4, Number4)
            Double.TryParse(SubTotal, Total)

            If (Total < ((Number1 + Number2 + Number3 + Number4) - 2)) Or (Total > ((Number1 + Number2 + Number3 + Number4) + 2)) Then
                SubTotalTextBox.CssClass = "TextRight Yellow"
                Return False
            Else
                SubTotalTextBox.CssClass = "TextRight"
                Return True
            End If
        End Function

        'Takes in data point values, and validates subtotal
        Public Function CheckSubTotals4(ByVal Value1 As String, ByVal Value2 As String, ByVal Value3 As String, _
                                      ByVal Value4 As String, ByVal SubTotal As String) As Boolean
            Dim Number1 As Double = 0
            Dim Number2 As Double = 0
            Dim Number3 As Double = 0
            Dim Number4 As Double = 0
            Dim Total As Double = 0
            Dim isNumber As Boolean = False
            Dim isEmpty As Boolean = False
            Dim isM As Boolean = False
            Dim thisTotal As Double = 0

            'Check for M

            If Value1 = "M" AndAlso Value2 = "M" AndAlso Value3 = "M" AndAlso Value4 = "M" Then
                If SubTotal = "M" OrElse Double.TryParse(SubTotal, Total) Then
                    Return True
                Else
                    Return False
                End If
            End If

            'check for all numbers, then check if they total up
            If Double.TryParse(Value1, Number1) AndAlso Double.TryParse(Value2, Number2) AndAlso Double.TryParse(Value3, Number3) AndAlso Double.TryParse(Value4, Number4) AndAlso Double.TryParse(SubTotal, Total) Then
                If (Total < ((Number1 + Number2 + Number3 + Number4) - 2)) Or _
                   (Total > ((Number1 + Number2 + Number3 + Number4) + 2)) Then
                    Return False
                Else
                    Return True
                End If
            End If


            'Checks for all empty strings, then checks if the subtotal has an empty string
            If String.IsNullOrEmpty(Value1) AndAlso String.IsNullOrEmpty(Value2) AndAlso String.IsNullOrEmpty(Value3) AndAlso String.IsNullOrEmpty(Value4) Then
                If String.IsNullOrEmpty(SubTotal) Then
                    Return True
                Else
                    Return False
                End If
            End If

            'Checks for the existence of 1 M, and checks if the sub total is also an M
            If Value1 = "M" OrElse Value2 = "M" OrElse Value3 = "M" OrElse Value4 = "M" Then
                If SubTotal = "M" Then
                    Return True
                Else
                    Return False
                End If
            End If

            'Tries to convert to double, then checks totals
            Double.TryParse(Value1, Number1)
            Double.TryParse(Value2, Number2)
            Double.TryParse(Value3, Number3)
            Double.TryParse(Value4, Number4)
            Double.TryParse(SubTotal, Total)

            If (Total < ((Number1 + Number2 + Number3 + Number4) - 2)) Or _
               (Total > ((Number1 + Number2 + Number3 + Number4) + 2)) Then
                Return False
            Else
                Return True
            End If


        End Function
        'Overloaded function for validating subtotals from the corrections page form - passes in the textbox by reference
        'If false, sets the background of the textbox to yellow, if true sets to white
        Public Function CheckSubTotals3(ByVal Value1 As String, ByVal Value2 As String, ByVal Value3 As String, _
                                       ByVal SubTotal As String, ByRef SubTotalTextBox As TextBox) As Boolean
            Dim Number1 As Double = 0
            Dim Number2 As Double = 0
            Dim Number3 As Double = 0
            Dim Total As Double = 0
            Dim isNumber As Boolean = False
            Dim isEmpty As Boolean = False
            Dim isM As Boolean = False
            Dim thisTotal As Double = 0

            'Check for M

            If Value1 = "M" AndAlso Value2 = "M" AndAlso Value3 = "M" Then
                If SubTotal = "M" OrElse Double.TryParse(SubTotal, Total) Then
                    SubTotalTextBox.CssClass = "TextRight"
                    Return True
                Else
                    SubTotalTextBox.CssClass = "TextRight Yellow"
                    Return False
                End If
            End If

            'check for all numbers, then check if they total up
            If Double.TryParse(Value1, Number1) AndAlso Double.TryParse(Value2, Number2) AndAlso Double.TryParse(Value3, Number3) AndAlso Double.TryParse(SubTotal, Total) Then
                If (Total < ((Number1 + Number2 + Number3) - 2)) Or _
                   (Total > ((Number1 + Number2 + Number3) + 2)) Then
                    SubTotalTextBox.CssClass = "TextRight Yellow"
                    Return False
                Else
                    SubTotalTextBox.CssClass = "TextRight"
                    Return True
                End If
            End If


            'Checks for all empty strings, then checks if the subtotal has an empty string
            If String.IsNullOrEmpty(Value1) AndAlso String.IsNullOrEmpty(Value2) AndAlso String.IsNullOrEmpty(Value3) Then
                If String.IsNullOrEmpty(SubTotal) Then
                    SubTotalTextBox.CssClass = "TextRight"
                    Return True
                Else
                    SubTotalTextBox.CssClass = "TextRight Yellow"
                    Return False
                End If
            End If

            'Checks for the existence of 1 M, and checks if the sub total is also an M
            If Value1 = "M" OrElse Value2 = "M" OrElse Value3 = "M" Then
                If SubTotal = "M" Then
                    SubTotalTextBox.CssClass = "TextRight"
                    Return True
                Else
                    SubTotalTextBox.CssClass = "TextRight Yellow"
                    Return False
                End If
            End If

            'Tries to convert to double, then checks totals
            Double.TryParse(Value1, Number1)
            Double.TryParse(Value2, Number2)
            Double.TryParse(Value3, Number3)
            Double.TryParse(SubTotal, Total)

            If (Total < ((Number1 + Number2 + Number3) - 2)) Or _
               (Total > ((Number1 + Number2 + Number3) + 2)) Then
                SubTotalTextBox.CssClass = "TextRight Yellow"
                Return False
            Else
                SubTotalTextBox.CssClass = "TextRight"
                Return True
            End If

        End Function


        'Takes in data point values, and validates subtotal
        Public Function CheckSubTotals3(ByVal Value1 As String, ByVal Value2 As String, ByVal Value3 As String, _
                                       ByVal SubTotal As String) As Boolean
            Dim Number1 As Double = 0
            Dim Number2 As Double = 0
            Dim Number3 As Double = 0
            Dim Total As Double = 0
            Dim isNumber As Boolean = False
            Dim isEmpty As Boolean = False
            Dim isM As Boolean = False
            Dim thisTotal As Double = 0

            'Check for M

            If Value1 = "M" AndAlso Value2 = "M" AndAlso Value3 = "M" Then
                If SubTotal = "M" OrElse Double.TryParse(SubTotal, Total) Then
                    Return True
                Else
                    Return False
                End If
            End If

            'check for all numbers, then check if they total up
            If Double.TryParse(Value1, Number1) AndAlso Double.TryParse(Value2, Number2) AndAlso Double.TryParse(Value3, Number3) AndAlso Double.TryParse(SubTotal, Total) Then
                If (Total < ((Number1 + Number2 + Number3) - 2)) Or _
                   (Total > ((Number1 + Number2 + Number3) + 2)) Then
                    Return False
                Else
                    Return True
                End If
            End If


            'Checks for all empty strings, then checks if the subtotal has an empty string
            If String.IsNullOrEmpty(Value1) AndAlso String.IsNullOrEmpty(Value2) AndAlso String.IsNullOrEmpty(Value3) Then
                If String.IsNullOrEmpty(SubTotal) Then
                    Return True
                Else
                    Return False
                End If
            End If

            'Checks for the existence of 1 M, and checks if the sub total is also an M
            If Value1 = "M" OrElse Value2 = "M" OrElse Value3 = "M" Then
                If SubTotal = "M" Then
                    Return True
                Else
                    Return False
                End If
            End If

            'Tries to convert to double, then checks totals
            Double.TryParse(Value1, Number1)
            Double.TryParse(Value2, Number2)
            Double.TryParse(Value3, Number3)
            Double.TryParse(SubTotal, Total)

            If (Total < ((Number1 + Number2 + Number3) - 2)) Or _
               (Total > ((Number1 + Number2 + Number3) + 2)) Then
                Return False
            Else
                Return True
            End If

        End Function


        'Overloaded function for validating subtotals from the corrections page form - passes in the textbox by reference
        'If false, sets the background of the textbox to yellow, if true sets to white
        Public Function CheckSubTotals2(ByVal Value1 As String, ByVal Value2 As String, ByVal SubTotal As String, ByRef SubTotalTextBox As TextBox) As Boolean
            Dim Number1 As Double = 0
            Dim Number2 As Double = 0
            Dim Total As Double = 0
            Dim isNumber As Boolean = False
            Dim isEmpty As Boolean = False
            Dim isM As Boolean = False
            Dim thisTotal As Double = 0

            'Check for M

            If Value1 = "M" AndAlso Value2 = "M" Then
                If SubTotal = "M" OrElse Double.TryParse(SubTotal, Total) Then
                    SubTotalTextBox.CssClass = "TextRight"
                    Return True
                Else
                    SubTotalTextBox.CssClass = "TextRight Yellow"
                    Return False
                End If
            End If

            'check for all numbers, then check if they total up
            If Double.TryParse(Value1, Number1) AndAlso Double.TryParse(Value2, Number2) AndAlso Double.TryParse(SubTotal, Total) Then
                If (Total < ((Number1 + Number2) - 2)) Or _
                   (Total > ((Number1 + Number2) + 2)) Then
                    SubTotalTextBox.CssClass = "TextRight Yellow"
                    Return False
                Else
                    SubTotalTextBox.CssClass = "TextRight"
                    Return True
                End If
            End If


            'Checks for all empty strings, then checks if the subtotal has an empty string
            If String.IsNullOrEmpty(Value1) AndAlso String.IsNullOrEmpty(Value2) Then
                If String.IsNullOrEmpty(SubTotal) Then
                    SubTotalTextBox.CssClass = "TextRight"
                    Return True
                Else
                    SubTotalTextBox.CssClass = "TextRight Yellow"
                    Return False
                End If
            End If

            'Checks for the existence of 1 M, and checks if the sub total is also an M
            If Value1 = "M" OrElse Value2 = "M" Then
                If SubTotal = "M" Then
                    SubTotalTextBox.CssClass = "TextRight"
                    Return True
                Else
                    SubTotalTextBox.CssClass = "TextRight Yellow"
                    Return False
                End If
            End If

            'Tries to convert to double, then checks totals
            Double.TryParse(Value1, Number1)
            Double.TryParse(Value2, Number2)
            Double.TryParse(SubTotal, Total)

            If (Total < ((Number1 + Number2) - 2)) Or _
               (Total > ((Number1 + Number2) + 2)) Then
                SubTotalTextBox.CssClass = "TextRight Yellow"
                Return False
            Else
                SubTotalTextBox.CssClass = "TextRight"
                Return True
            End If


        End Function


        'Takes in data point values, and validates subtotal
        Public Function CheckSubTotals2(ByVal Value1 As String, ByVal Value2 As String, ByVal SubTotal As String) As Boolean
            Dim Number1 As Double = 0
            Dim Number2 As Double = 0
            Dim Total As Double = 0
            Dim isNumber As Boolean = False
            Dim isEmpty As Boolean = False
            Dim isM As Boolean = False
            Dim thisTotal As Double = 0

            'Check for M

            If Value1 = "M" AndAlso Value2 = "M" Then
                If SubTotal = "M" OrElse Double.TryParse(SubTotal, Total) Then
                    Return True
                Else
                    Return False
                End If
            End If

            'check for all numbers, then check if they total up
            If Double.TryParse(Value1, Number1) AndAlso Double.TryParse(Value2, Number2) AndAlso Double.TryParse(SubTotal, Total) Then
                If (Total < ((Number1 + Number2) - 2)) Or _
                   (Total > ((Number1 + Number2) + 2)) Then
                    Return False
                Else
                    Return True
                End If
            End If


            'Checks for all empty strings, then checks if the subtotal has an empty string
            If String.IsNullOrEmpty(Value1) AndAlso String.IsNullOrEmpty(Value2) Then
                If String.IsNullOrEmpty(SubTotal) Then
                    Return True
                Else
                    Return False
                End If
            End If

            'Checks for the existence of 1 M, and checks if the sub total is also an M
            If Value1 = "M" OrElse Value2 = "M" Then
                If SubTotal = "M" Then
                    Return True
                Else
                    Return False
                End If
            End If

            'Tries to convert to double, then checks totals
            Double.TryParse(Value1, Number1)
            Double.TryParse(Value2, Number2)
            Double.TryParse(SubTotal, Total)

            If (Total < ((Number1 + Number2) - 2)) Or _
               (Total > ((Number1 + Number2) + 2)) Then
                Return False
            Else
                Return True
            End If

        End Function

    End Class

    Public Class Survey2Functions

        'Overloaded function for validating subtotals from the corrections page form - passes in the textbox by reference
        'If false, sets the background of the textbox to yellow, if true sets to white
        Public Function CheckSubTotals4(ByVal Value1 As String, ByVal Value2 As String, ByVal Value3 As String, _
                                       ByVal Value4 As String, ByVal SubTotal As String, ByRef SubTotalTextBox As TextBox) As Boolean
            Dim Number1 As Double = 0
            Dim Number2 As Double = 0
            Dim Number3 As Double = 0
            Dim Number4 As Double = 0
            Dim Total As Double = 0
            Dim isNumber As Boolean = False
            Dim isEmpty As Boolean = False
            Dim isM As Boolean = False
            Dim thisTotal As Double = 0

            'Check for M

            If Value1 = "M" AndAlso Value2 = "M" AndAlso Value3 = "M" AndAlso Value4 = "M" Then
                If SubTotal = "M" OrElse Double.TryParse(SubTotal, Total) Then
                    SubTotalTextBox.CssClass = "TextRight"
                    Return True
                Else
                    SubTotalTextBox.CssClass = "TextRight Yellow"
                    Return False
                End If
            End If

            'check for all numbers, then check if they total up
            If Double.TryParse(Value1, Number1) AndAlso Double.TryParse(Value2, Number2) AndAlso Double.TryParse(Value3, Number3) AndAlso Double.TryParse(Value4, Number4) AndAlso Double.TryParse(SubTotal, Total) Then
                If (Total < ((Number1 + Number2 + Number3 + Number4) - 2)) Or _
                   (Total > ((Number1 + Number2 + Number3 + Number4) + 2)) Then
                    SubTotalTextBox.CssClass = "TextRight Yellow"
                    Return False
                Else
                    SubTotalTextBox.CssClass = "TextRight"
                    Return True
                End If
            End If


            'Checks for all empty strings, then checks if the subtotal has an empty string
            If String.IsNullOrEmpty(Value1) AndAlso String.IsNullOrEmpty(Value2) AndAlso String.IsNullOrEmpty(Value3) AndAlso String.IsNullOrEmpty(Value4) Then
                If String.IsNullOrEmpty(SubTotal) Then
                    SubTotalTextBox.CssClass = "TextRight"
                    Return True
                Else
                    SubTotalTextBox.CssClass = "TextRight Yellow"
                    Return False
                End If
            End If

            'Checks for the existence of 1 M, and checks if the sub total is also an M
            If Value1 = "M" OrElse Value2 = "M" OrElse Value3 = "M" OrElse Value4 = "M" Then
                If SubTotal = "M" Then
                    SubTotalTextBox.CssClass = "TextRight"
                    Return True
                Else
                    SubTotalTextBox.CssClass = "TextRight Yellow"
                    Return False
                End If
            End If

            'Tries to convert to double, then checks totals
            Double.TryParse(Value1, Number1)
            Double.TryParse(Value2, Number2)
            Double.TryParse(Value3, Number3)
            Double.TryParse(Value4, Number4)
            Double.TryParse(SubTotal, Total)

            If (Total < ((Number1 + Number2 + Number3 + Number4) - 2)) Or (Total > ((Number1 + Number2 + Number3 + Number4) + 2)) Then
                SubTotalTextBox.CssClass = "TextRight Yellow"
                Return False
            Else
                SubTotalTextBox.CssClass = "TextRight"
                Return True
            End If
        End Function

        'Takes in data point values, and validates subtotal
        Public Function CheckSubTotals4(ByVal Value1 As String, ByVal Value2 As String, ByVal Value3 As String, _
                                      ByVal Value4 As String, ByVal SubTotal As String) As Boolean
            Dim Number1 As Double = 0
            Dim Number2 As Double = 0
            Dim Number3 As Double = 0
            Dim Number4 As Double = 0
            Dim Total As Double = 0
            Dim isNumber As Boolean = False
            Dim isEmpty As Boolean = False
            Dim isM As Boolean = False
            Dim thisTotal As Double = 0

            'Check for M

            If Value1 = "M" AndAlso Value2 = "M" AndAlso Value3 = "M" AndAlso Value4 = "M" Then
                If SubTotal = "M" OrElse Double.TryParse(SubTotal, Total) Then
                    Return True
                Else
                    Return False
                End If
            End If

            'check for all numbers, then check if they total up
            If Double.TryParse(Value1, Number1) AndAlso Double.TryParse(Value2, Number2) AndAlso Double.TryParse(Value3, Number3) AndAlso Double.TryParse(Value4, Number4) AndAlso Double.TryParse(SubTotal, Total) Then
                If (Total < ((Number1 + Number2 + Number3 + Number4) - 2)) Or _
                   (Total > ((Number1 + Number2 + Number3 + Number4) + 2)) Then
                    Return False
                Else
                    Return True
                End If
            End If


            'Checks for all empty strings, then checks if the subtotal has an empty string
            If String.IsNullOrEmpty(Value1) AndAlso String.IsNullOrEmpty(Value2) AndAlso String.IsNullOrEmpty(Value3) AndAlso String.IsNullOrEmpty(Value4) Then
                If String.IsNullOrEmpty(SubTotal) Then
                    Return True
                Else
                    Return False
                End If
            End If

            'Checks for the existence of 1 M, and checks if the sub total is also an M
            If Value1 = "M" OrElse Value2 = "M" OrElse Value3 = "M" OrElse Value4 = "M" Then
                If SubTotal = "M" Then
                    Return True
                Else
                    Return False
                End If
            End If

            'Tries to convert to double, then checks totals
            Double.TryParse(Value1, Number1)
            Double.TryParse(Value2, Number2)
            Double.TryParse(Value3, Number3)
            Double.TryParse(Value4, Number4)
            Double.TryParse(SubTotal, Total)

            If (Total < ((Number1 + Number2 + Number3 + Number4) - 2)) Or _
               (Total > ((Number1 + Number2 + Number3 + Number4) + 2)) Then
                Return False
            Else
                Return True
            End If


        End Function
        'Overloaded function for validating subtotals from the corrections page form - passes in the textbox by reference
        'If false, sets the background of the textbox to yellow, if true sets to white
        Public Function CheckSubTotals5(ByVal Value1 As String, ByVal Value2 As String, ByVal Value3 As String, _
                                       ByVal Value4 As String, ByVal Value5 As String, ByVal SubTotal As String, ByRef SubTotalTextBox As TextBox) As Boolean
            Dim Number1 As Double = 0
            Dim Number2 As Double = 0
            Dim Number3 As Double = 0
            Dim Number4 As Double = 0
            Dim Number5 As Double = 0
            Dim Total As Double = 0
            Dim isNumber As Boolean = False
            Dim isEmpty As Boolean = False
            Dim isM As Boolean = False
            Dim thisTotal As Double = 0

            'Check for M

            If Value1 = "M" AndAlso Value2 = "M" AndAlso Value3 = "M" AndAlso Value4 = "M" AndAlso Value5 = "M" Then
                If SubTotal = "M" OrElse Double.TryParse(SubTotal, Total) Then
                    SubTotalTextBox.CssClass = "TextRight"
                    Return True
                Else
                    SubTotalTextBox.CssClass = "TextRight Yellow"
                    Return False
                End If
            End If

            'check for all numbers, then check if they total up
            If Double.TryParse(Value1, Number1) AndAlso Double.TryParse(Value2, Number2) AndAlso Double.TryParse(Value3, Number3) AndAlso Double.TryParse(Value4, Number4) AndAlso Double.TryParse(Value5, Number5) AndAlso Double.TryParse(SubTotal, Total) Then
                If (Total < ((Number1 + Number2 + Number3 + Number4 + Number5) - 2)) Or _
                   (Total > ((Number1 + Number2 + Number3 + Number4 + Number5) + 2)) Then
                    SubTotalTextBox.CssClass = "TextRight Yellow"
                    Return False
                Else
                    SubTotalTextBox.CssClass = "TextRight"
                    Return True
                End If
            End If


            'Checks for all empty strings, then checks if the subtotal has an empty string
            If String.IsNullOrEmpty(Value1) AndAlso String.IsNullOrEmpty(Value2) AndAlso String.IsNullOrEmpty(Value3) AndAlso String.IsNullOrEmpty(Value4) AndAlso String.IsNullOrEmpty(Value5) Then
                If String.IsNullOrEmpty(SubTotal) Then
                    SubTotalTextBox.CssClass = "TextRight"
                    Return True
                Else
                    SubTotalTextBox.CssClass = "TextRight Yellow"
                    Return False
                End If
            End If

            'Checks for the existence of 1 M, and checks if the sub total is also an M
            If Value1 = "M" OrElse Value2 = "M" OrElse Value3 = "M" OrElse Value4 = "M" OrElse Value5 = "M" Then
                If SubTotal = "M" Then
                    SubTotalTextBox.CssClass = "TextRight"
                    Return True
                Else
                    SubTotalTextBox.CssClass = "TextRight Yellow"
                    Return False
                End If
            End If

            'Tries to convert to double, then checks totals
            Double.TryParse(Value1, Number1)
            Double.TryParse(Value2, Number2)
            Double.TryParse(Value3, Number3)
            Double.TryParse(Value4, Number4)
            Double.TryParse(Value5, Number5)
            Double.TryParse(SubTotal, Total)

            If (Total < ((Number1 + Number2 + Number3 + Number4 + Number5) - 2)) Or (Total > ((Number1 + Number2 + Number3 + Number4 + Number5) + 2)) Then
                SubTotalTextBox.CssClass = "TextRight Yellow"
                Return False
            Else
                SubTotalTextBox.CssClass = "TextRight"
                Return True
            End If
        End Function

        'Overloaded function for validating subtotals from the corrections page form - passes in the textbox by reference
        'If false, sets the background of the textbox to yellow, if true sets to white
        Public Function CheckSubTotals3(ByVal Value1 As String, ByVal Value2 As String, ByVal Value3 As String, _
                                       ByVal SubTotal As String, ByRef SubTotalTextBox As TextBox) As Boolean
            Dim Number1 As Double = 0
            Dim Number2 As Double = 0
            Dim Number3 As Double = 0
            Dim Total As Double = 0
            Dim isNumber As Boolean = False
            Dim isEmpty As Boolean = False
            Dim isM As Boolean = False
            Dim thisTotal As Double = 0

            'Check for M

            If Value1 = "M" AndAlso Value2 = "M" AndAlso Value3 = "M" Then
                If SubTotal = "M" OrElse Double.TryParse(SubTotal, Total) Then
                    SubTotalTextBox.CssClass = "TextRight"
                    Return True
                Else
                    SubTotalTextBox.CssClass = "TextRight Yellow"
                    Return False
                End If
            End If

            'check for all numbers, then check if they total up
            If Double.TryParse(Value1, Number1) AndAlso Double.TryParse(Value2, Number2) AndAlso Double.TryParse(Value3, Number3) AndAlso Double.TryParse(SubTotal, Total) Then
                If (Total < ((Number1 + Number2 + Number3) - 2)) Or _
                   (Total > ((Number1 + Number2 + Number3) + 2)) Then
                    SubTotalTextBox.CssClass = "TextRight Yellow"
                    Return False
                Else
                    SubTotalTextBox.CssClass = "TextRight"
                    Return True
                End If
            End If


            'Checks for all empty strings, then checks if the subtotal has an empty string
            If String.IsNullOrEmpty(Value1) AndAlso String.IsNullOrEmpty(Value2) AndAlso String.IsNullOrEmpty(Value3) Then
                If String.IsNullOrEmpty(SubTotal) Then
                    SubTotalTextBox.CssClass = "TextRight"
                    Return True
                Else
                    SubTotalTextBox.CssClass = "TextRight Yellow"
                    Return False
                End If
            End If

            'Checks for the existence of 1 M, and checks if the sub total is also an M
            If Value1 = "M" OrElse Value2 = "M" OrElse Value3 = "M" Then
                If SubTotal = "M" Then
                    SubTotalTextBox.CssClass = "TextRight"
                    Return True
                Else
                    SubTotalTextBox.CssClass = "TextRight Yellow"
                    Return False
                End If
            End If

            'Tries to convert to double, then checks totals
            Double.TryParse(Value1, Number1)
            Double.TryParse(Value2, Number2)
            Double.TryParse(Value3, Number3)
            Double.TryParse(SubTotal, Total)

            If (Total < ((Number1 + Number2 + Number3) - 2)) Or _
               (Total > ((Number1 + Number2 + Number3) + 2)) Then
                SubTotalTextBox.CssClass = "TextRight Yellow"
                Return False
            Else
                SubTotalTextBox.CssClass = "TextRight"
                Return True
            End If

        End Function


        'Takes in data point values, and validates subtotal
        Public Function CheckSubTotals3(ByVal Value1 As String, ByVal Value2 As String, ByVal Value3 As String, _
                                       ByVal SubTotal As String) As Boolean
            Dim Number1 As Double = 0
            Dim Number2 As Double = 0
            Dim Number3 As Double = 0
            Dim Total As Double = 0
            Dim isNumber As Boolean = False
            Dim isEmpty As Boolean = False
            Dim isM As Boolean = False
            Dim thisTotal As Double = 0

            'Check for M

            If Value1 = "M" AndAlso Value2 = "M" AndAlso Value3 = "M" Then
                If SubTotal = "M" OrElse Double.TryParse(SubTotal, Total) Then
                    Return True
                Else
                    Return False
                End If
            End If

            'check for all numbers, then check if they total up
            If Double.TryParse(Value1, Number1) AndAlso Double.TryParse(Value2, Number2) AndAlso Double.TryParse(Value3, Number3) AndAlso Double.TryParse(SubTotal, Total) Then
                If (Total < ((Number1 + Number2 + Number3) - 2)) Or _
                   (Total > ((Number1 + Number2 + Number3) + 2)) Then
                    Return False
                Else
                    Return True
                End If
            End If


            'Checks for all empty strings, then checks if the subtotal has an empty string
            If String.IsNullOrEmpty(Value1) AndAlso String.IsNullOrEmpty(Value2) AndAlso String.IsNullOrEmpty(Value3) Then
                If String.IsNullOrEmpty(SubTotal) Then
                    Return True
                Else
                    Return False
                End If
            End If

            'Checks for the existence of 1 M, and checks if the sub total is also an M
            If Value1 = "M" OrElse Value2 = "M" OrElse Value3 = "M" Then
                If SubTotal = "M" Then
                    Return True
                Else
                    Return False
                End If
            End If

            'Tries to convert to double, then checks totals
            Double.TryParse(Value1, Number1)
            Double.TryParse(Value2, Number2)
            Double.TryParse(Value3, Number3)
            Double.TryParse(SubTotal, Total)

            If (Total < ((Number1 + Number2 + Number3) - 2)) Or _
               (Total > ((Number1 + Number2 + Number3) + 2)) Then
                Return False
            Else
                Return True
            End If

        End Function


        'Overloaded function for validating subtotals from the corrections page form - passes in the textbox by reference
        'If false, sets the background of the textbox to yellow, if true sets to white
        Public Function CheckSubTotals2(ByVal Value1 As String, ByVal Value2 As String, ByVal SubTotal As String, ByRef SubTotalTextBox As TextBox) As Boolean
            Dim Number1 As Double = 0
            Dim Number2 As Double = 0
            Dim Total As Double = 0
            Dim isNumber As Boolean = False
            Dim isEmpty As Boolean = False
            Dim isM As Boolean = False
            Dim thisTotal As Double = 0

            'Check for M

            If Value1 = "M" AndAlso Value2 = "M" Then
                If SubTotal = "M" OrElse Double.TryParse(SubTotal, Total) Then
                    SubTotalTextBox.CssClass = "TextRight"
                    Return True
                Else
                    SubTotalTextBox.CssClass = "TextRight Yellow"
                    Return False
                End If
            End If

            'check for all numbers, then check if they total up
            If Double.TryParse(Value1, Number1) AndAlso Double.TryParse(Value2, Number2) AndAlso Double.TryParse(SubTotal, Total) Then
                If (Total < ((Number1 + Number2) - 2)) Or _
                   (Total > ((Number1 + Number2) + 2)) Then
                    SubTotalTextBox.CssClass = "TextRight Yellow"
                    Return False
                Else
                    SubTotalTextBox.CssClass = "TextRight"
                    Return True
                End If
            End If


            'Checks for all empty strings, then checks if the subtotal has an empty string
            If String.IsNullOrEmpty(Value1) AndAlso String.IsNullOrEmpty(Value2) Then
                If String.IsNullOrEmpty(SubTotal) Then
                    SubTotalTextBox.CssClass = "TextRight"
                    Return True
                Else
                    SubTotalTextBox.CssClass = "TextRight Yellow"
                    Return False
                End If
            End If

            'Checks for the existence of 1 M, and checks if the sub total is also an M
            If Value1 = "M" OrElse Value2 = "M" Then
                If SubTotal = "M" Then
                    SubTotalTextBox.CssClass = "TextRight"
                    Return True
                Else
                    SubTotalTextBox.CssClass = "TextRight Yellow"
                    Return False
                End If
            End If

            'Tries to convert to double, then checks totals
            Double.TryParse(Value1, Number1)
            Double.TryParse(Value2, Number2)
            Double.TryParse(SubTotal, Total)

            If (Total < ((Number1 + Number2) - 2)) Or _
               (Total > ((Number1 + Number2) + 2)) Then
                SubTotalTextBox.CssClass = "TextRight Yellow"
                Return False
            Else
                SubTotalTextBox.CssClass = "TextRight"
                Return True
            End If


        End Function


        'Takes in data point values, and validates subtotal
        Public Function CheckSubTotals2(ByVal Value1 As String, ByVal Value2 As String, ByVal SubTotal As String) As Boolean
            Dim Number1 As Double = 0
            Dim Number2 As Double = 0
            Dim Total As Double = 0
            Dim isNumber As Boolean = False
            Dim isEmpty As Boolean = False
            Dim isM As Boolean = False
            Dim thisTotal As Double = 0

            'Check for M

            If Value1 = "M" AndAlso Value2 = "M" Then
                If SubTotal = "M" OrElse Double.TryParse(SubTotal, Total) Then
                    Return True
                Else
                    Return False
                End If
            End If

            'check for all numbers, then check if they total up
            If Double.TryParse(Value1, Number1) AndAlso Double.TryParse(Value2, Number2) AndAlso Double.TryParse(SubTotal, Total) Then
                If (Total < ((Number1 + Number2) - 2)) Or _
                   (Total > ((Number1 + Number2) + 2)) Then
                    Return False
                Else
                    Return True
                End If
            End If


            'Checks for all empty strings, then checks if the subtotal has an empty string
            If String.IsNullOrEmpty(Value1) AndAlso String.IsNullOrEmpty(Value2) Then
                If String.IsNullOrEmpty(SubTotal) Then
                    Return True
                Else
                    Return False
                End If
            End If

            'Checks for the existence of 1 M, and checks if the sub total is also an M
            If Value1 = "M" OrElse Value2 = "M" Then
                If SubTotal = "M" Then
                    Return True
                Else
                    Return False
                End If
            End If

            'Tries to convert to double, then checks totals
            Double.TryParse(Value1, Number1)
            Double.TryParse(Value2, Number2)
            Double.TryParse(SubTotal, Total)

            If (Total < ((Number1 + Number2) - 2)) Or _
               (Total > ((Number1 + Number2) + 2)) Then
                Return False
            Else
                Return True
            End If

        End Function

    End Class

    Public Class Algorithms

        Protected gf As Globals.GlobalFunctions = New Globals.GlobalFunctions

        Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext(gf.GetDBConnection.ToString)
        Dim dd As DataDictionaryDataContext = New DataDictionaryDataContext(gf.GetDBConnection.ToString)

        ' Instantiate the DataSet variable which is used by updateFactTable
        Dim dataSet As New DataSet


        ''' <summary>
        ''' Runs functions that create fact tables - passes in RadProgressContext object for the progress bar
        ''' </summary>
        ''' <param name="SurveySeriesID">Integer</param>
        ''' <param name="progress">RadProgressContext</param>
        ''' <returns>Boolean</returns>
        ''' <remarks></remarks>
        Public Function CreateFactTables(ByVal SurveySeriesID As Integer, ByRef progress As RadProgressContext) As Boolean
            progress.PrimaryTotal = 100
            progress.PrimaryValue = 0
            progress.PrimaryPercent = 0

            ' Execution sequence should be runAlgorithms > calcVarStandard > updateFactTable
            If runAlgorithms(SurveySeriesID, progress) AndAlso calcVarStandard(SurveySeriesID, progress) AndAlso updateFactTable(SurveySeriesID, progress) Then
                progress.PrimaryValue = 100
                progress.PrimaryPercent = 100
                Return True
            Else
                Return False
            End If
        End Function

#Region "calculated variables standard"

        ''' <summary>
        ''' runs calculations on submitted data
        ''' </summary>
        ''' <param name="SurveySeries">Integer</param>
        ''' <returns>Boolean</returns>
        ''' <remarks></remarks>
        Public Function calcVarStandard(ByVal SurveySeries As Integer, ByRef progress As RadProgressContext) As Boolean
            Dim theYear As Integer
            Dim theQuarter As Integer
            Dim theSurveyID As Integer

            Try

                Dim q = (From s In db.dSurveySeries Where s.SurveySeriesID = SurveySeries Select s.Year, s.Quarter, s.SurveyID).ToArray

                theYear = q(0).Year
                theQuarter = q(0).Quarter
                theSurveyID = q(0).SurveyID

                If calcYTD(theSurveyID, theQuarter, theYear, progress) AndAlso calcPCYA(theSurveyID, theQuarter, theYear, progress) AndAlso calcPYTD(theSurveyID, theQuarter, theYear, progress) AndAlso _
                   calcCYTD(theSurveyID, theQuarter, theYear, progress) AndAlso calcCPCYA(theSurveyID, theQuarter, theYear, progress) AndAlso calcCPYTD(theSurveyID, theQuarter, theYear, progress) _
                Then
                    Return True
                Else
                    Return False
                End If
            Catch ex As Exception
                gf.NotifyWebmaster(ex, gf.GetCurrentUrl, "calcVarStandard error - SurveySeriesID: " & SurveySeries.ToString)
                Return False
            End Try
        End Function


        ''' <summary>
        ''' Calculates YTD
        ''' </summary>
        ''' <param name="theSurvey">Integer</param>
        ''' <param name="theQtr">Integer</param>
        ''' <param name="theYear">Integer</param>
        ''' <returns>Boolean</returns>
        ''' <remarks></remarks>
        Private Function calcYTD(ByVal theSurvey As Integer, ByVal theQtr As Integer, ByVal theYear As Integer, ByRef progress As RadProgressContext) As Boolean

            Try

                'Delete affected records in cYTD
                Dim ieToDelete As IEnumerable(Of cYTD) = _
                   (From eToDelete In db.cYTDs _
                    Where _
                            eToDelete.dSurveySeries.SurveyID = theSurvey _
                    ).ToList()
                db.cYTDs.DeleteAllOnSubmit(ieToDelete)
                db.SubmitChanges()

                'Project cData and create two fields to count missing and NA
                Dim dM = From n In db.fDatas _
                 Select _
                  n.dSurveySeries.SurveyID, _
                  n.ddField.ScaleID, _
                  n.dSurveySeries.Year, _
                  n.dSurveySeries.Quarter, _
                  n.FieldID, _
                  n.dWorkbook.SourceID, _
                  n.SurveySeriesID, _
                  n.MOR, _
                  n.Value, _
                  n.Number _
                 Let Missing = If(Value = "M", 1, 0), _
                     NA = If(Value Is Nothing, 1, 0) _
                 Where ScaleID = 3 And SurveyID = theSurvey And Quarter <= theQtr And Year = theYear

                'Calculate YTD using group by and create two fields:
                '  DataID for YTD matches DataID for fData
                Dim sN = From n In dM Group By _
                           n.SurveyID, _
                           n.SourceID, _
                           n.Year, _
                           n.FieldID, _
                           n.MOR _
                          Into cCount = Count(), _
                               cM = Sum(n.Missing), _
                               cNA = Sum(n.NA), _
                               YTD = Sum(n.Number) _
                          Order By SourceID, Year, FieldID _
                          Select _
                               SurveyID, _
                               SourceID, _
                               Year, _
                               FieldID, _
                               MOR, _
                               YTD, _
                               cM, _
                               cNA, _
                               cCount _
                          Let SurveySeriesID = db.dSurveySeries.Where(Function(s) s.SurveyID = theSurvey And s.Quarter = theQtr And s.Year = Year _
                                                                     ).Select(Function(ss) ss.SurveySeriesID).Single _
                          Let DataID = db.fDatas.Where(Function(s) s.SurveySeriesID = SurveySeriesID AndAlso s.FieldID = FieldID AndAlso s.dWorkbook.SourceID = SourceID _
                                                                     ).Select(Function(ss) ss.DataID).Single
                'Project to appropriate layout.
                Dim qYTD = (From c In sN _
                              Select _
                                   c.DataID, _
                                   c.FieldID, _
                                   c.SourceID, _
                                   c.SurveySeriesID, _
                                   c.MOR, _
                                   Value = If(c.cM > 0, "M", If(c.cNA = theQtr, Nothing, c.YTD.ToString)), _
                                   Number = If(c.cM = 0, c.YTD, Nothing) _
                              Order By SourceID, FieldID).ToList

                'copy qYTD to cYTD table
                Dim cY As cYTD
                Dim cYList As New List(Of cYTD)
                For Each dF In qYTD 'df is data field
                    cY = New cYTD
                    cY.DataID = dF.DataID
                    cY.FieldID = dF.FieldID
                    cY.SourceID = dF.SourceID
                    cY.SurveySeriesID = dF.SurveySeriesID
                    cY.MOR = dF.MOR
                    cY.Value = dF.Value
                    cY.Number = dF.Number

                    cYList.Add(cY)
                Next

                db.cYTDs.InsertAllOnSubmit(cYList)
                db.SubmitChanges()

                progress.PrimaryValue = 5
                progress.PrimaryPercent = 5

                Return True
            Catch ex As Exception
                gf.NotifyWebmaster(ex, gf.GetCurrentUrl, "calcYTD Error - SurveyID: " & theSurvey.ToString & " " & theQtr.ToString & "\" & theYear.ToString)
                Return False
            End Try

        End Function

        ''' <summary>
        ''' calculates
        ''' </summary>
        ''' <param name="theSurvey">Integer</param>
        ''' <param name="theQtr">Integer</param>
        ''' <param name="theYear">Integer</param>
        ''' <returns>Boolean</returns>
        ''' <remarks></remarks>
        Protected Function calcPCYA(ByVal theSurvey As Integer, ByVal theQtr As Integer, ByVal theYear As Integer, ByRef progress As RadProgressContext) As Boolean

            Try

                'Dim Survey As Integer = 999
                'Dim theQtr As Integer = 4
                'Dim theYear As Integer = 2010

                Dim validScaleIDs() As Integer = {3, 4}

                'Delete affected records in cpCYA
                Dim ieToDelete As IEnumerable(Of cPCYA) = _
                   (From eToDelete In db.cPCYAs _
                    Where _
                            eToDelete.dSurveySeries.SurveyID = theSurvey _
                    ).ToList()
                db.cPCYAs.DeleteAllOnSubmit(ieToDelete)
                db.SubmitChanges()

                Dim cY = From n In db.fDatas _
                 Select _
                  n.ddField.SurveyID, _
                  n.ddField.ScaleID, _
                  n.dSurveySeries.Year, _
                  n.dSurveySeries.Quarter, _
                  n.FieldID, _
                  n.dWorkbook.SourceID, _
                  n.SurveySeriesID, _
                  n.MOR, _
                  n.Value, _
                  n.Number _
                 Let Missing = If(Value = "M", 1, 0), _
                   NA = If(Value Is Nothing, 1, 0) _
                 Where validScaleIDs.Contains(ScaleID) AndAlso SurveyID = theSurvey AndAlso Year = theYear AndAlso Quarter = theQtr

                Dim pY = From n In db.fDatas _
                 Select _
                  n.ddField.SurveyID, _
                  n.ddField.ScaleID, _
                  n.dSurveySeries.Year, _
                  n.dSurveySeries.Quarter, _
                  n.FieldID, _
                  n.dWorkbook.SourceID, _
                  n.SurveySeriesID, _
                  n.MOR, _
                  n.Value, _
                  n.Number _
                 Let Missing = If(Value = "M", 1, 0), _
                          NA = If(Value Is Nothing, 1, 0) _
                 Where validScaleIDs.Contains(ScaleID) AndAlso SurveyID = theSurvey AndAlso Year = theYear - 1 AndAlso Quarter = theQtr

                Dim bothY = From c In cY, p In pY _
                    Where c.FieldID = p.FieldID AndAlso c.SourceID = p.SourceID AndAlso c.MOR = p.MOR _
                    Select _
                      c.FieldID, _
                      c.SourceID, _
                      c.SurveySeriesID, _
                      cV = c.Value, _
                      pV = p.Value, _
                      cN = c.Number, _
                      pN = p.Number

                Dim pCYG = From b In bothY _
                    Select _
                      b.FieldID, _
                      b.SourceID, _
                      b.SurveySeriesID, _
                      b.cV, _
                      b.pV, _
                      b.cN, _
                      b.pN _
                     Let _
                      Number = If(pN <> 0, 100 * (cN / pN - 1), Nothing), _
                      Value = If(pV = "M" Or cV = "M", "M", If(pV Is Nothing Or cV Is Nothing, Nothing, Number.ToString))

                Dim cYall = (From c In cY _
                             Group Join p In pCYG _
                             On c.SourceID Equals p.SourceID And c.FieldID Equals p.FieldID Into Group _
                             From p In Group.DefaultIfEmpty() _
                             Select _
                             c.FieldID, _
                             c.SourceID, _
                             c.SurveySeriesID, _
                             c.MOR, _
                             p.Value, _
                             p.Number _
                             Let DataID = db.fDatas.Where(Function(s) s.SurveySeriesID = SurveySeriesID AndAlso s.FieldID = FieldID AndAlso s.dWorkbook.SourceID = SourceID _
                                                                     ).Select(Function(ss) ss.DataID).Single _
                             Order By SourceID, FieldID).ToList


                'copy cYall to cPCYA table
                Dim o As cPCYA
                Dim oList As New List(Of cPCYA)
                For Each dF In cYall 'df is data field
                    o = New cPCYA
                    o.DataID = dF.DataID
                    o.FieldID = dF.FieldID
                    o.SourceID = dF.SourceID
                    o.SurveySeriesID = dF.SurveySeriesID
                    o.MOR = dF.MOR
                    o.Value = dF.Value
                    o.Number = dF.Number

                    oList.Add(o)
                Next

                db.cPCYAs.InsertAllOnSubmit(oList)
                db.SubmitChanges()

                progress.PrimaryValue = 10
                progress.PrimaryPercent = 10

                Return True
            Catch ex As Exception
                gf.NotifyWebmaster(ex, gf.GetCurrentUrl, "calcPCYA Error - SurveyID: " & theSurvey.ToString & " " & theQtr.ToString & "\" & theYear.ToString)
                Return False
            End Try


        End Function

        ''' <summary>
        ''' calculates 
        ''' </summary>
        ''' <param name="theSurvey">Integer</param>
        ''' <param name="theQtr">Integer</param>
        ''' <param name="theYear">Integer</param>
        ''' <returns>Boolean</returns>
        ''' <remarks></remarks>
        Protected Function calcPYTD(ByVal theSurvey As Integer, ByVal theQtr As Integer, ByVal theYear As Integer, ByRef progress As RadProgressContext) As Boolean

            Try
                'Dim Survey As Integer = 999
                'Dim theQtr As Integer = 4
                'Dim theYear As Integer = 2010

                Dim validScaleIDs() As Integer = {3, 4}

                'Delete affected records in cpYTD
                Dim ieToDelete As IEnumerable(Of cPYTD) = _
                   (From eToDelete In db.cPYTDs _
                    Where _
                            eToDelete.dSurveySeries.SurveyID = theSurvey _
                    ).ToList()
                db.cPYTDs.DeleteAllOnSubmit(ieToDelete)
                db.SubmitChanges()

                Dim cY = From n In db.cYTDs _
                 Select _
                    n.dSurveySeries.SurveyID, _
                    n.fData.ddField.ScaleID, _
                    n.dSurveySeries.Year, _
                    n.dSurveySeries.Quarter, _
                    n.FieldID, _
                    n.SourceID, _
                    n.SurveySeriesID, _
                    n.MOR, _
                    n.Value, _
                    n.Number _
                 Let Missing = If(Value = "M", 1, 0), _
                   NA = If(Value Is Nothing, 1, 0) _
                 Where validScaleIDs.Contains(ScaleID) AndAlso SurveyID = theSurvey AndAlso Year = theYear AndAlso Quarter = theQtr

                Dim pY = From n In db.cYTDs _
                 Select _
                    n.dSurveySeries.SurveyID, _
                    n.fData.ddField.ScaleID, _
                    n.dSurveySeries.Year, _
                    n.dSurveySeries.Quarter, _
                    n.FieldID, _
                    n.SourceID, _
                    n.SurveySeriesID, _
                    n.MOR, _
                    n.Value, _
                    n.Number _
                 Let Missing = If(Value = "M", 1, 0), _
                          NA = If(Value Is Nothing, 1, 0) _
                 Where validScaleIDs.Contains(ScaleID) AndAlso SurveyID = theSurvey AndAlso Year = theYear - 1 AndAlso Quarter = theQtr

                Dim bothY = From c In cY, p In pY _
                    Where c.FieldID = p.FieldID AndAlso c.SourceID = p.SourceID AndAlso c.MOR = p.MOR _
                    Select _
                      c.FieldID, _
                      c.SourceID, _
                      c.SurveySeriesID, _
                      cV = c.Value, _
                      pV = p.Value, _
                      cN = c.Number, _
                      pN = p.Number

                Dim pCYG = From b In bothY _
                    Select _
                      b.FieldID, _
                      b.SourceID, _
                      b.SurveySeriesID, _
                      b.cV, _
                      b.pV, _
                      b.cN, _
                      b.pN _
                     Let _
                      Number = If(pN <> 0, 100 * (cN / pN - 1), Nothing), _
                      Value = If(pV = "M" Or cV = "M", "M", If(pV Is Nothing Or cV Is Nothing, Nothing, Number.ToString))

                Dim cYall = (From c In cY _
                             Group Join p In pCYG _
                             On c.SourceID Equals p.SourceID And c.FieldID Equals p.FieldID Into Group _
                             From p In Group.DefaultIfEmpty() _
                             Select _
                             c.FieldID, _
                             c.SourceID, _
                             c.SurveySeriesID, _
                             c.MOR, _
                             p.Value, _
                             p.Number _
                             Let DataID = db.cYTDs.Where(Function(s) s.SurveySeriesID = SurveySeriesID AndAlso s.FieldID = FieldID AndAlso s.SourceID = SourceID _
                                                         ).Select(Function(ss) ss.DataID).Single _
                             Order By SourceID, FieldID).ToList


                'copy cYall to cpYTD table
                Dim o As cPYTD
                Dim oList As New List(Of cPYTD)
                For Each dF In cYall 'df is data field
                    o = New cPYTD
                    o.DataID = dF.DataID
                    o.FieldID = dF.FieldID
                    o.SourceID = dF.SourceID
                    o.SurveySeriesID = dF.SurveySeriesID
                    o.MOR = dF.MOR
                    o.Value = dF.Value
                    o.Number = dF.Number

                    oList.Add(o)
                Next
                db.cPYTDs.InsertAllOnSubmit(oList)
                db.SubmitChanges()

                progress.PrimaryValue = 15
                progress.PrimaryPercent = 15

                Return True
            Catch ex As Exception
                gf.NotifyWebmaster(ex, gf.GetCurrentUrl, "calcPYTD Error - SurveyID: " & theSurvey.ToString & " " & theQtr.ToString & "\" & theYear.ToString)
                Return False
            End Try

        End Function

        ''' <summary>
        ''' calculates
        ''' </summary>
        ''' <param name="theSurvey">Integer</param>
        ''' <param name="theQtr">Integer</param>
        ''' <param name="theYear">Integer</param>
        ''' <returns>Boolean</returns>
        ''' <remarks></remarks>
        Private Function calcCYTD(ByVal theSurvey As Integer, ByVal theQtr As Integer, ByVal theYear As Integer, ByRef progress As RadProgressContext) As Boolean

            Try

                'Delete affected records in cCYTD
                Dim ieToDelete As IEnumerable(Of ccYTD) = _
                   (From eToDelete In db.ccYTDs _
                    Where _
                            eToDelete.dSurveySeries.SurveyID = theSurvey _
                    ).ToList()
                db.ccYTDs.DeleteAllOnSubmit(ieToDelete)
                db.SubmitChanges()

                'Project cData and create two fields to count missing and NA
                Dim dM = From n In db.cDatas _
                 Select _
                  n.dSurveySeries.SurveyID, _
                  n.ddcField.ScaleID, _
                  n.dSurveySeries.Year, _
                  n.dSurveySeries.Quarter, _
                  n.FieldID, _
                  n.dWorkbook.SourceID, _
                  n.SurveySeriesID, _
                  n.MOR, _
                  n.Value, _
                  n.Number _
                 Let Missing = If(Value = "M", 1, 0), _
                     NA = If(Value Is Nothing, 1, 0) _
                 Where ScaleID = 3 And SurveyID = theSurvey And Quarter <= theQtr And Year = theYear

                'Calculate YTD using group by and create two fields:
                '  DataID for YTD matches DataID for cData
                Dim sN = From n In dM Group By _
                           n.SurveyID, _
                           n.SourceID, _
                           n.Year, _
                           n.FieldID, _
                           n.MOR _
                          Into cCount = Count(), _
                               cM = Sum(n.Missing), _
                               cNA = Sum(n.NA), _
                               YTD = Sum(n.Number) _
                          Order By SourceID, Year, FieldID _
                          Select _
                               SurveyID, _
                               SourceID, _
                               Year, _
                               FieldID, _
                               MOR, _
                               YTD, _
                               cM, _
                               cNA, _
                               cCount _
                          Let SurveySeriesID = db.dSurveySeries.Where(Function(s) s.SurveyID = theSurvey And s.Quarter = theQtr And s.Year = Year _
                                                                     ).Select(Function(ss) ss.SurveySeriesID).Single _
                          Let DataID = db.cDatas.Where(Function(s) s.SurveySeriesID = SurveySeriesID AndAlso s.FieldID = FieldID AndAlso s.dWorkbook.SourceID = SourceID _
                                                                     ).Select(Function(ss) ss.DataID).Single
                'Project to appropriate layout.
                Dim qYTD = (From c In sN _
                              Select _
                                   c.DataID, _
                                   c.FieldID, _
                                   c.SourceID, _
                                   c.SurveySeriesID, _
                                   c.MOR, _
                                   Value = If(c.cM > 0, "M", If(c.cNA = theQtr, Nothing, c.YTD.ToString)), _
                                   Number = If(c.cM = 0, c.YTD, Nothing) _
                              Order By SourceID, FieldID).ToList

                'copy qYTD to cCYTD table
                Dim cY As ccYTD
                Dim cYList As New List(Of ccYTD)
                For Each dF In qYTD 'df is data field
                    cY = New ccYTD
                    cY.DataID = dF.DataID
                    cY.FieldID = dF.FieldID
                    cY.SourceID = dF.SourceID
                    cY.SurveySeriesID = dF.SurveySeriesID
                    cY.MOR = dF.MOR
                    cY.Value = dF.Value
                    cY.Number = dF.Number

                    cYList.Add(cY)
                Next
                db.ccYTDs.InsertAllOnSubmit(cYList)
                db.SubmitChanges()

                progress.PrimaryValue = 20
                progress.PrimaryPercent = 20

                Return True
            Catch ex As Exception
                gf.NotifyWebmaster(ex, gf.GetCurrentUrl, "calcCYTD Error - SurveyID: " & theSurvey.ToString & " " & theQtr.ToString & "\" & theYear.ToString)
                Return False
            End Try


        End Function

        ''' <summary>
        ''' calculates
        ''' </summary>
        ''' <param name="theSurvey">Integer</param>
        ''' <param name="theQtr">Integer</param>
        ''' <param name="theYear">Integer</param>
        ''' <returns>Boolean</returns>
        ''' <remarks></remarks>
        Protected Function calcCPCYA(ByVal theSurvey As Integer, ByVal theQtr As Integer, ByVal theYear As Integer, ByRef progress As RadProgressContext) As Boolean

            Try

                'Dim Survey As Integer = 999
                'Dim theQtr As Integer = 4
                'Dim theYear As Integer = 2010

                Dim validScaleIDs() As Integer = {3, 4}

                'Delete affected records in cCPCYA
                Dim ieToDelete As IEnumerable(Of ccPCYA) = _
                   (From eToDelete In db.ccPCYAs _
                    Where _
                            eToDelete.dSurveySeries.SurveyID = theSurvey _
                    ).ToList()
                db.ccPCYAs.DeleteAllOnSubmit(ieToDelete)
                db.SubmitChanges()

                Dim cY = From n In db.cDatas _
                 Select _
                  n.ddcField.SurveyID, _
                  n.ddcField.ScaleID, _
                  n.dSurveySeries.Year, _
                  n.dSurveySeries.Quarter, _
                  n.FieldID, _
                  n.dWorkbook.SourceID, _
                  n.SurveySeriesID, _
                  n.MOR, _
                  n.Value, _
                  n.Number _
                 Let Missing = If(Value = "M", 1, 0), _
                   NA = If(Value Is Nothing, 1, 0) _
                 Where validScaleIDs.Contains(ScaleID) AndAlso SurveyID = theSurvey AndAlso Year = theYear AndAlso Quarter = theQtr

                Dim pY = From n In db.cDatas _
                 Select _
                  n.ddcField.SurveyID, _
                  n.ddcField.ScaleID, _
                  n.dSurveySeries.Year, _
                  n.dSurveySeries.Quarter, _
                  n.FieldID, _
                  n.dWorkbook.SourceID, _
                  n.SurveySeriesID, _
                  n.MOR, _
                  n.Value, _
                  n.Number _
                 Let Missing = If(Value = "M", 1, 0), _
                          NA = If(Value Is Nothing, 1, 0) _
                 Where validScaleIDs.Contains(ScaleID) AndAlso SurveyID = theSurvey AndAlso Year = theYear - 1 AndAlso Quarter = theQtr

                Dim bothY = From c In cY, p In pY _
                    Where c.FieldID = p.FieldID AndAlso c.SourceID = p.SourceID AndAlso c.MOR = p.MOR _
                    Select _
                      c.FieldID, _
                      c.SourceID, _
                      c.SurveySeriesID, _
                      cV = c.Value, _
                      pV = p.Value, _
                      cN = c.Number, _
                      pN = p.Number

                Dim pCYG = From b In bothY _
                    Select _
                      b.FieldID, _
                      b.SourceID, _
                      b.SurveySeriesID, _
                      b.cV, _
                      b.pV, _
                      b.cN, _
                      b.pN _
                     Let _
                      Number = If(pN <> 0, 100 * (cN / pN - 1), Nothing), _
                      Value = If(pV = "M" Or cV = "M", "M", If(pV Is Nothing Or cV Is Nothing, Nothing, Number.ToString))

                Dim cYall = (From c In cY _
                             Group Join p In pCYG _
                             On c.SourceID Equals p.SourceID And c.FieldID Equals p.FieldID Into Group _
                             From p In Group.DefaultIfEmpty() _
                             Select _
                             c.FieldID, _
                             c.SourceID, _
                             c.SurveySeriesID, _
                             c.MOR, _
                             p.Value, _
                             p.Number _
                             Let DataID = db.cDatas.Where(Function(s) s.SurveySeriesID = SurveySeriesID AndAlso s.FieldID = FieldID AndAlso s.dWorkbook.SourceID = SourceID _
                                                                     ).Select(Function(ss) ss.DataID).Single _
                             Order By SourceID, FieldID).ToList


                'copy cYall to cCPCYA table
                Dim o As ccPCYA
                Dim oList As New List(Of ccPCYA)
                For Each dF In cYall 'df is data field
                    o = New ccPCYA
                    o.DataID = dF.DataID
                    o.FieldID = dF.FieldID
                    o.SourceID = dF.SourceID
                    o.SurveySeriesID = dF.SurveySeriesID
                    o.MOR = dF.MOR
                    o.Value = dF.Value
                    o.Number = dF.Number

                    oList.Add(o)
                Next

                db.ccPCYAs.InsertAllOnSubmit(oList)
                db.SubmitChanges()

                progress.PrimaryValue = 25
                progress.PrimaryPercent = 25

                Return True

            Catch ex As Exception
                gf.NotifyWebmaster(ex, gf.GetCurrentUrl, "calcCPCYA Error - SurveyID: " & theSurvey.ToString & " " & theQtr.ToString & "\" & theYear.ToString)
                Return False
            End Try

        End Function

        ''' <summary>
        ''' calculates
        ''' </summary>
        ''' <param name="theSurvey">Integer</param>
        ''' <param name="theQtr">Integer</param>
        ''' <param name="theYear">Integer</param>
        ''' <returns>Boolean</returns>
        ''' <remarks></remarks>
        Protected Function calcCPYTD(ByVal theSurvey As Integer, ByVal theQtr As Integer, ByVal theYear As Integer, ByRef progress As RadProgressContext) As Boolean

            Try
                'Dim Survey As Integer = 999
                'Dim theQtr As Integer = 4
                'Dim theYear As Integer = 2010

                Dim validScaleIDs() As Integer = {3, 4}

                'Delete affected records in cCPYTD
                Dim ieToDelete As IEnumerable(Of ccPYTD) = _
                   (From eToDelete In db.ccPYTDs _
                    Where _
                            eToDelete.dSurveySeries.SurveyID = theSurvey _
                    ).ToList()
                db.ccPYTDs.DeleteAllOnSubmit(ieToDelete)
                db.SubmitChanges()

                Dim cY = From n In db.ccYTDs _
                 Select _
                    n.dSurveySeries.SurveyID, _
                    n.cData.ddcField.ScaleID, _
                    n.dSurveySeries.Year, _
                    n.dSurveySeries.Quarter, _
                    n.FieldID, _
                    n.SourceID, _
                    n.SurveySeriesID, _
                    n.MOR, _
                    n.Value, _
                    n.Number _
                 Let Missing = If(Value = "M", 1, 0), _
                   NA = If(Value Is Nothing, 1, 0) _
                 Where validScaleIDs.Contains(ScaleID) AndAlso SurveyID = theSurvey AndAlso Year = theYear AndAlso Quarter = theQtr

                Dim pY = From n In db.ccYTDs _
                 Select _
                    n.dSurveySeries.SurveyID, _
                    n.cData.ddcField.ScaleID, _
                    n.dSurveySeries.Year, _
                    n.dSurveySeries.Quarter, _
                    n.FieldID, _
                    n.SourceID, _
                    n.SurveySeriesID, _
                    n.MOR, _
                    n.Value, _
                    n.Number _
                 Let Missing = If(Value = "M", 1, 0), _
                          NA = If(Value Is Nothing, 1, 0) _
                 Where validScaleIDs.Contains(ScaleID) AndAlso SurveyID = theSurvey AndAlso Year = theYear - 1 AndAlso Quarter = theQtr

                Dim bothY = From c In cY, p In pY _
                    Where c.FieldID = p.FieldID AndAlso c.SourceID = p.SourceID AndAlso c.MOR = p.MOR _
                    Select _
                      c.FieldID, _
                      c.SourceID, _
                      c.SurveySeriesID, _
                      cV = c.Value, _
                      pV = p.Value, _
                      cN = c.Number, _
                      pN = p.Number

                Dim pCYG = From b In bothY _
                    Select _
                      b.FieldID, _
                      b.SourceID, _
                      b.SurveySeriesID, _
                      b.cV, _
                      b.pV, _
                      b.cN, _
                      b.pN _
                     Let _
                      Number = If(pN <> 0, 100 * (cN / pN - 1), Nothing), _
                      Value = If(pV = "M" Or cV = "M", "M", If(pV Is Nothing Or cV Is Nothing, Nothing, Number.ToString))

                Dim cYall = (From c In cY _
                             Group Join p In pCYG _
                             On c.SourceID Equals p.SourceID And c.FieldID Equals p.FieldID Into Group _
                             From p In Group.DefaultIfEmpty() _
                             Select _
                             c.FieldID, _
                             c.SourceID, _
                             c.SurveySeriesID, _
                             c.MOR, _
                             p.Value, _
                             p.Number _
                             Let DataID = db.ccYTDs.Where(Function(s) s.SurveySeriesID = SurveySeriesID AndAlso s.FieldID = FieldID AndAlso s.SourceID = SourceID _
                                                         ).Select(Function(ss) ss.DataID).Single _
                             Order By SourceID, FieldID).ToList


                'copy cYall to cCPYTD table
                Dim o As ccPYTD
                Dim oList As New List(Of ccPYTD)
                For Each dF In cYall 'df is data field
                    o = New ccPYTD
                    o.DataID = dF.DataID
                    o.FieldID = dF.FieldID
                    o.SourceID = dF.SourceID
                    o.SurveySeriesID = dF.SurveySeriesID
                    o.MOR = dF.MOR
                    o.Value = dF.Value
                    o.Number = dF.Number

                    oList.Add(o)
                Next

                db.ccPYTDs.InsertAllOnSubmit(oList)
                db.SubmitChanges()

                progress.PrimaryValue = 30
                progress.PrimaryPercent = 30

                Return True

            Catch ex As Exception
                gf.NotifyWebmaster(ex, gf.GetCurrentUrl, "calcCPYTD Error - SurveyID: " & theSurvey.ToString & " " & theQtr.ToString & "\" & theYear.ToString)
                Return False
            End Try

        End Function


#End Region

#Region "calculates variables custom"

        ''' <summary>
        ''' runs algorithms on reported and calculated data
        ''' </summary>
        ''' <param name="theSeriesID">Integer</param>
        ''' <returns>Boolean</returns>
        ''' <remarks>some of the algorithms have to run multiple times, as they use calculated data from algorithms - we limited this to 3</remarks>
        Public Function runAlgorithms(ByVal theSeriesID As Integer, ByRef progress As RadProgressContext) As Boolean


            Dim iCount As Integer

            Try
                ' First step delete calculated data for current SurveySeriesID
                ' Query the database for the rows to be deleted.
                If deleteCalculatedData(theSeriesID) Then
                    Dim qA As IEnumerable

                    For iCount = 1 To 3
                        qA = (From a In db.ddAlgorithms _
                                  Select _
                                      a.FieldID, _
                                      Algorithm = a.Algorithm.Trim.ToUpper, _
                                      a.Args, _
                                      IsReady = checkForArgs(theSeriesID, a.FieldID, a.Algorithm.Trim.ToUpper, a.Args) _
                                  ).ToList
                        Dim qReady = (From a In qA Where a.IsReady).ToList

                        If qReady.Count > 0 Then
                            For Each alg In qReady
                                If calculateCustomVar(theSeriesID, alg.FieldID, alg.Algorithm, alg.Args) = False Then
                                    Return False
                                End If
                            Next

                        End If
                        progress.PrimaryValue += 15
                        progress.PrimaryPercent += 15
                    Next
                    Return True
                Else
                    Return False
                End If

            Catch ex As Exception
                gf.NotifyWebmaster(ex, gf.GetCurrentUrl, "runAlgorithms; Series: " & theSeriesID.ToString)
                Return False
            End Try
        End Function


        ''' <summary>
        ''' Checks is field is already calculated, if not ,checks for proper syntax
        ''' </summary>
        ''' <param name="SeriesID">Integer</param>
        ''' <param name="FID">Integer</param>
        ''' <param name="Algorithm">String</param>
        ''' <param name="Args">String</param>
        ''' <returns>Boolean</returns>
        ''' <remarks></remarks>
        Private Function checkForArgs(ByVal SeriesID As Integer, ByVal FID As Integer, ByVal Algorithm As String, ByVal Args As String) As Boolean

            Dim B As Boolean = False

            Try
                Dim qfD = From f In db.fDatas Where f.SurveySeriesID = SeriesID Select f.FieldID
                Dim qcD = From c In db.cDatas Where c.SurveySeriesID = SeriesID Select c.FieldID
                Dim qFields = (From f In qfD.Concat(qcD) Select Field = f.ToString).ToArray

                'Check to see if field already calculated
                If qcD.Contains(FID) Then
                    Return False
                Else

                    Select Case Algorithm.Trim.ToUpper
                        Case "SUMOF"

                            'Add code to check Args has proper syntax
                            Dim aArgs() = Args.Split(",")
                            B = (aArgs.Length = qFields.Intersect(aArgs).Count)
                            Return B

                        Case "RATIOOF"

                            'Add code to check Args has proper syntax

                            Dim nArgs() = Args.Split(";")(0).Split(",")
                            Dim dArgs() = Args.Split(";")(1).Split(",")
                            B = (nArgs.Length = qFields.Intersect(nArgs).Count) And _
                                (dArgs.Length = qFields.Intersect(dArgs).Count)
                            Return B

                        Case "RECODEMTON"

                            'Add code to check Args has proper syntax
                            B = qFields.Contains(Args)
                            Return B

                        Case Else
                            Return False
                    End Select
                End If

            Catch ex As Exception
                gf.NotifyWebmaster(ex, gf.GetCurrentUrl, "")
                Return False
            End Try
        End Function

        ''' <summary>
        ''' deletes previous calculated data based on Series ID
        ''' </summary>
        ''' <param name="SeriesID">Integer</param>
        ''' <returns>Boolean</returns>
        ''' <remarks></remarks>
        Protected Function deleteCalculatedData(ByVal SeriesID As Integer) As Boolean

            Try

                ' Query the database for the rows to be deleted.
                Dim deletePCYA = _
                    From d In db.cPCYAs() _
                    Where d.SurveySeriesID = SeriesID _
                    Select d

                For Each d As cPCYA In deletePCYA
                    db.cPCYAs.DeleteOnSubmit(d)
                Next

                db.SubmitChanges()


                Dim deletePYTD = _
                    From d In db.cPYTDs() _
                    Where d.SurveySeriesID = SeriesID _
                    Select d

                For Each d As cPYTD In deletePYTD
                    db.cPYTDs.DeleteOnSubmit(d)
                Next

                db.SubmitChanges()


                Dim deleteYTD = _
                    From d In db.cYTDs() _
                    Where d.SurveySeriesID = SeriesID _
                    Select d

                For Each d As cYTD In deleteYTD
                    db.cYTDs.DeleteOnSubmit(d)
                Next

                db.SubmitChanges()

                Dim deleteCPCYA = _
                    From d In db.ccPCYAs() _
                    Where d.SurveySeriesID = SeriesID _
                    Select d

                For Each d As ccPCYA In deleteCPCYA
                    db.ccPCYAs.DeleteOnSubmit(d)
                Next

                db.SubmitChanges()


                Dim deletecPYTD = _
                    From d In db.ccPYTDs() _
                    Where d.SurveySeriesID = SeriesID _
                    Select d

                For Each d As ccPYTD In deletecPYTD
                    db.ccPYTDs.DeleteOnSubmit(d)
                Next

                db.SubmitChanges()


                Dim deletecYTD = _
                    From d In db.ccYTDs() _
                    Where d.SurveySeriesID = SeriesID _
                    Select d

                For Each d As ccYTD In deletecYTD
                    db.ccYTDs.DeleteOnSubmit(d)
                Next

                db.SubmitChanges()
                Dim deleteData = _
                    From d In db.cDatas() _
                    Where d.SurveySeriesID = SeriesID _
                    Select d

                For Each d As cData In deleteData
                    db.cDatas.DeleteOnSubmit(d)
                Next

                db.SubmitChanges()

                Return True

            Catch ex As Exception
                gf.NotifyWebmaster(ex, gf.GetCurrentUrl, "deleteCalculatedData; Series: " & SeriesID.ToString)
                Return False
            End Try


        End Function

        ''' <summary>
        ''' runs algorithms with supplied instructons
        ''' </summary>
        ''' <param name="SeriesID">Integer</param>
        ''' <param name="FID">Integer</param>
        ''' <param name="Algorithm">string</param>
        ''' <param name="Args">string</param>
        ''' <returns>Boolean</returns>
        ''' <remarks></remarks>
        Protected Function calculateCustomVar(ByVal SeriesID As Integer, ByVal FID As Integer, ByVal Algorithm As String, ByVal Args As String) As Boolean

            Select Case Algorithm.Trim.ToUpper
                Case "SUMOF"

                    'Add code to check Args has proper syntax 
                    Return calculateSumOf(SeriesID, FID, Args)

                Case "RATIOOF"
                    'Add code to check Args has proper syntax 
                    Return calculateRatioOf(SeriesID, FID, Args)

                Case "RECODEMTON"

                    Return recodeMtoN(SeriesID, FID, Args)

                Case Else
                    Return False
            End Select

        End Function

        ''' <summary>
        ''' Calculates Sum Of
        ''' </summary>
        ''' <param name="SeriesID">Integer</param>
        ''' <param name="theFieldID">Integer</param>
        ''' <param name="theArgs">String</param>
        ''' <returns>Boolean</returns>
        ''' <remarks></remarks>
        Protected Function calculateSumOf(ByVal SeriesID As Integer, ByVal theFieldID As Integer, ByVal theArgs As String) As Boolean
            'Dim theSurvey As Integer = 999
            'Dim theQtr As Integer = 4
            'Dim theYear As Integer = 2010
            'Dim theArgs As String = "999000005,999000008"
            'Dim theFieldID As Integer = 999900001

            Try

                Dim aArgs() = theArgs.Split(",")

                Dim qF = From n In db.fDatas _
                    Select _
                        n.SurveySeriesID, _
                        n.WorkbookID, _
                        n.FieldID, _
                        n.MOR, _
                        n.Value, _
                        n.Number _
                    Let _
                        Missing = If(Value = "M", 1, 0), _
                             NA = If(Value Is Nothing, 1, 0) _
                    Where SurveySeriesID = SeriesID AndAlso aArgs.Contains(FieldID.ToString)

                Dim qC = From n In db.cDatas _
                    Select _
                        n.SurveySeriesID, _
                        n.WorkbookID, _
                        n.FieldID, _
                        n.MOR, _
                        n.Value, _
                        n.Number _
                    Let _
                        Missing = If(Value = "M", 1, 0), _
                             NA = If(Value Is Nothing, 1, 0) _
                    Where SurveySeriesID = SeriesID AndAlso aArgs.Contains(FieldID.ToString)

                Dim qFC = qF.Concat(qC).ToArray

                Dim sN = From n In qFC _
                         Group By _
                            n.SurveySeriesID, _
                            n.WorkbookID, _
                            n.MOR _
                         Into cCount = Count(), _
                            cM = Sum(n.Missing), _
                            cNA = Sum(n.NA), _
                            theSum = Sum(n.Number) _
                         Order By WorkbookID _
                         Select _
                            SurveySeriesID, _
                            WorkbookID, _
                            MOR, _
                            theSum, _
                            cM, _
                            cNA

                'Project to appropriate layout.
                Dim qResult = From c In sN _
                  Select _
                   c.WorkbookID, _
                   c.SurveySeriesID, _
                   c.MOR, _
                   Value = If(c.cM > 0, "M", If(c.cNA = aArgs.Count, Nothing, c.theSum.ToString)), _
                   Number = If(c.cM = 0, c.theSum, Nothing) _
                  Let FieldID = theFieldID _
                  Order By WorkbookID

                'copy qResult to cData table
                Dim cResult As cData
                Dim cRList As New List(Of cData)
                For Each dF In qResult 'df is data field

                    cResult = New cData
                    cResult.FieldID = dF.FieldID
                    cResult.WorkbookID = dF.WorkbookID
                    cResult.SurveySeriesID = dF.SurveySeriesID
                    cResult.MOR = dF.MOR
                    cResult.Value = dF.Value
                    cResult.Number = dF.Number

                    cRList.Add(cResult)

                Next
                db.cDatas.InsertAllOnSubmit(cRList)
                db.SubmitChanges()
                Return True

            Catch ex As Exception
                gf.NotifyWebmaster(ex, gf.GetCurrentUrl, "calculateSumOf; Series: " & SeriesID.ToString & "; Field: " & theFieldID.ToString & "; Args: " & theArgs.ToString)
                Return False

            End Try
        End Function

        ''' <summary>
        ''' Calculates Ratio
        ''' </summary>
        ''' <param name="SeriesID">Integer</param>
        ''' <param name="theFieldID">Integer</param>
        ''' <param name="theArgs">string</param>
        ''' <returns>Boolean</returns>
        ''' <remarks></remarks>
        Protected Function calculateRatioOf(ByVal SeriesID As Integer, ByVal theFieldID As Integer, ByVal theArgs As String) As Boolean
            'Dim theSurvey As Integer = 999
            'Dim theQtr As Integer = 4
            'Dim theYear As Integer = 2010
            'Dim theArgs As String = "999000005;999000005,999000008"
            'Dim theFieldID As Integer = 999900001
            Try

                Dim nArgs() = theArgs.Split(";")(0).Split(",")
                Dim dArgs() = theArgs.Split(";")(1).Split(",")

                Dim qnF = From n In db.fDatas _
                 Select _
                  n.FieldID, _
                  n.WorkbookID, _
                  n.SurveySeriesID, _
                  n.MOR, _
                  n.Value, _
                  n.Number _
                 Let Missing = If(Value = "M", 1, 0), _
                     NA = If(Value Is Nothing, 1, 0) _
                 Where SurveySeriesID = SeriesID AndAlso nArgs.Contains(FieldID.ToString)

                Dim qnC = From n In db.cDatas _
                 Select _
                  n.FieldID, _
                  n.WorkbookID, _
                  n.SurveySeriesID, _
                  n.MOR, _
                  n.Value, _
                  n.Number _
                 Let Missing = If(Value = "M", 1, 0), _
                     NA = If(Value Is Nothing, 1, 0) _
                 Where SurveySeriesID = SeriesID AndAlso nArgs.Contains(FieldID.ToString)

                Dim dmN = qnF.Concat(qnC).ToArray
                Dim snN = From n In dmN Group By _
                   n.SurveySeriesID, _
                   n.WorkbookID, _
                   n.MOR _
                  Into cCount = Count(), _
                       cM = Sum(n.Missing), _
                       cNA = Sum(n.NA), _
                       theSum = Sum(n.Number) _
                  Order By WorkbookID _
                  Select _
                   SurveySeriesID, _
                   WorkbookID, _
                   MOR, _
                   theSum, _
                   cM, _
                   cNA, _
                   cCount

                Dim Numerator = From c In snN _
                  Select _
                   c.WorkbookID, _
                   c.SurveySeriesID, _
                   c.MOR, _
                   Value = If(c.cM > 0, "M", If(c.cNA = nArgs.Count, Nothing, c.theSum.ToString)), _
                   Number = If(c.cM = 0, c.theSum, Nothing) _
                  Order By WorkbookID

                Dim qdF = From n In db.fDatas _
                 Select _
                  n.FieldID, _
                  n.WorkbookID, _
                  n.SurveySeriesID, _
                  n.MOR, _
                  n.Value, _
                  n.Number _
                 Let Missing = If(Value = "M", 1, 0), _
                     NA = If(Value Is Nothing, 1, 0) _
                 Where SurveySeriesID = SeriesID AndAlso dArgs.Contains(FieldID.ToString)

                Dim qdC = From n In db.cDatas _
                 Select _
                  n.FieldID, _
                  n.WorkbookID, _
                  n.SurveySeriesID, _
                  n.MOR, _
                  n.Value, _
                  n.Number _
                 Let Missing = If(Value = "M", 1, 0), _
                     NA = If(Value Is Nothing, 1, 0) _
                 Where SurveySeriesID = SeriesID AndAlso dArgs.Contains(FieldID.ToString)

                Dim dmD = qnF.Concat(qdC)
                Dim snD = From n In dmD Group By _
                   n.SurveySeriesID, _
                   n.WorkbookID, _
                   n.MOR _
                  Into cCount = Count(), _
                       cM = Sum(n.Missing), _
                       cNA = Sum(n.NA), _
                       theSum = Sum(n.Number) _
                  Order By WorkbookID _
                  Select _
                   SurveySeriesID, _
                   WorkbookID, _
                   MOR, _
                   theSum, _
                   cM, _
                   cNA, _
                   cCount

                Dim Denominator = From c In snD _
                  Select _
                   c.WorkbookID, _
                   c.SurveySeriesID, _
                   c.MOR, _
                   Value = If(c.cM > 0, "M", If(c.cNA = dArgs.Count, Nothing, c.theSum.ToString)), _
                   Number = If(c.cM = 0, c.theSum, Nothing) _
                  Order By WorkbookID

                Dim Both = From n In Numerator, d In Denominator _
                  Where n.WorkbookID = d.WorkbookID AndAlso n.MOR = d.MOR _
                  Select _
                   n.WorkbookID, _
                   n.SurveySeriesID, _
                   n.MOR, _
                   nV = n.Value, _
                   dV = d.Value, _
                   nN = n.Number, _
                   dN = d.Number

                Dim theRatio = (From b In Both _
                                  Select _
                                    b.WorkbookID, _
                                    b.SurveySeriesID, _
                                    b.MOR, _
                                    b.nV, _
                                    b.dV, _
                                    b.nN, _
                                    b.dN _
                                   Let _
                                    Number = If(dN <> 0, nN / dN, Nothing), _
                                    Value = If(dV = "M" Or nV = "M", "M", If(dV Is Nothing Or nV Is Nothing, Nothing, Number.ToString)) _
                                    Order By WorkbookID).ToList

                Dim qResult = (From c In theRatio _
                                  Select _
                                   c.WorkbookID, _
                                   c.SurveySeriesID, _
                                   c.MOR, _
                                   c.Value, _
                                   c.Number _
                                  Let FieldID = theFieldID _
                                  Order By WorkbookID).ToList


                'copy qResult to cData table
                Dim cResult As cData
                Dim cRList As New List(Of cData)
                For Each dF In qResult 'df is data field

                    cResult = New cData
                    cResult.FieldID = dF.FieldID
                    cResult.WorkbookID = dF.WorkbookID
                    cResult.SurveySeriesID = dF.SurveySeriesID
                    cResult.MOR = dF.MOR
                    cResult.Value = dF.Value
                    cResult.Number = dF.Number

                    cRList.Add(cResult)

                Next
                db.cDatas.InsertAllOnSubmit(cRList)
                db.SubmitChanges()

                Return True
            Catch ex As Exception
                gf.NotifyWebmaster(ex, gf.GetCurrentUrl, "calculateRatioOf; Series: " & SeriesID.ToString & "; Field: " & theFieldID.ToString & "; Args: " & theArgs.ToString)
                Return False
            End Try
        End Function


        ''' <summary>
        ''' handles "M" and "N"
        ''' </summary>
        ''' <param name="SeriesID">Integer</param>
        ''' <param name="theFieldID">Integer</param>
        ''' <param name="theArgs">String</param>
        ''' <returns>Boolean</returns>
        ''' <remarks></remarks>
        Protected Function recodeMtoN(ByVal SeriesID As Integer, ByVal theFieldID As Integer, ByVal theArgs As String) As Boolean

            'Dim theSurvey As Integer = 999
            'Dim theQtr As Integer = 4
            'Dim theYear As Integer = 2010
            'Dim theArg As String = "999000007"
            'Dim theFieldID As Integer = 999900001
            Try

                Dim nArgs() = theArgs.Split(";")(0).Split(",")

                Dim qF = From n In db.fDatas _
                    Select _
                        n.SurveySeriesID, _
                        n.WorkbookID, _
                        n.FieldID, _
                        n.MOR, _
                        n.Value, _
                        n.Number _
                    Let _
                           FID = theFieldID, _
                             V = If(Value = "M", Nothing, If(Value Is Nothing, Nothing, Number.ToString)), _
                             N = If(Value = "M", 0, Number) _
                    Where SurveySeriesID = SeriesID AndAlso nArgs.Contains(FieldID.ToString)

                Dim qC = From n In db.cDatas _
                    Select _
                        n.SurveySeriesID, _
                        n.WorkbookID, _
                        n.FieldID, _
                        n.MOR, _
                        n.Value, _
                        n.Number _
                    Let _
                          FID = theFieldID, _
                            V = If(Value = "M", Nothing, If(Value Is Nothing, Nothing, Number.ToString)), _
                            N = If(Value = "M", 0, Number) _
                    Where SurveySeriesID = SeriesID AndAlso nArgs.Contains(FieldID.ToString)

                Dim qFC = qF.Concat(qC).ToArray

                Dim qResult = From f In qFC _
                Select _
                 FieldID = f.FID, _
                 f.WorkbookID, _
                 f.SurveySeriesID, _
                 f.MOR, _
                 Value = f.V, _
                 Number = f.N

                'copy qResult to cData table
                Dim cResult As cData
                Dim cRList As New List(Of cData)
                For Each dF In qResult 'df is data field

                    cResult = New cData
                    cResult.FieldID = dF.FieldID
                    cResult.WorkbookID = dF.WorkbookID
                    cResult.SurveySeriesID = dF.SurveySeriesID
                    cResult.MOR = dF.MOR
                    cResult.Value = dF.Value
                    cResult.Number = dF.Number

                    cRList.Add(cResult)

                Next
                db.cDatas.InsertAllOnSubmit(cRList)
                db.SubmitChanges()

                Return True

            Catch ex As Exception
                gf.NotifyWebmaster(ex, gf.GetCurrentUrl, "Recode M to N; Series: " & SeriesID.ToString & "; Field: " & theFieldID.ToString & "; Args: " & theArgs.ToString)
                Return False
            End Try

        End Function

#End Region

#Region "Update Fact Table"



        Public Function updateFactTable(ByVal SurveySeries As Integer, ByRef progress As RadProgressContext) As Boolean
            updateFactTable = False
            'Note dataSet is a public variable

            Dim theYear As Integer
            Dim theQuarter As Integer
            Dim theSurveyID As Integer

            Dim sFTName As String

            progress.PrimaryValue = 80
            progress.PrimaryPercent = 80

            Try

                Dim q = (From s In db.dSurveySeries Where s.SurveySeriesID = SurveySeries Select s.Year, s.Quarter, s.SurveyID).ToArray

                theYear = q(0).Year
                theQuarter = q(0).Quarter
                theSurveyID = q(0).SurveyID


                'Code to determine Fact Table name; for example, ft999FactTable0
                sFTName = "ft" & theSurveyID.ToString & "FactTable0"
                If CheckForTable(sFTName) Then


                    'http://msdn.microsoft.com/en-us/library/system.data.datatable.aspx		

                    initializeFactTable(theSurveyID, theYear)
                    createNamesTable(theSurveyID, theYear, "S")
                    createMeasureTable(theSurveyID, theYear)
                    createAttributeTable(theSurveyID, theYear)
                    makeDataRelation()

                    Dim FTable As DataTable = dataSet.Tables("FactTable")
                    Dim Measures As DataTable = dataSet.Tables("Measures")
                    Dim Attributes As DataTable = dataSet.Tables("Attributes")
                    Dim Names As DataTable = dataSet.Tables("Names")

                    Dim dCS = (From f In db.ddFields, a In db.ddAttributes, c In db.ddCodeSets _
                               Where f.SurveyID = theSurveyID AndAlso f.FieldID = a.FieldID AndAlso a.CodeSetID = c.CodeSetID _
                               Select c.CodeSet).Distinct.ToList


                    Dim qM = _
                      From m In Measures _
                      From n In Names _
                      Where m.Field(Of Integer)("OID") = _
                            n.Field(Of Integer)("OID") _
                      Select _
                       OName = n.Field(Of String)("OName"), _
                       DataID = m.Field(Of System.Int32)("DataID"), _
                       DateID = m.Field(Of System.DateTime)("DateID"), _
                       OID = m.Field(Of System.Int32)("OID"), _
                       FieldID = m.Field(Of System.Int32)("FieldID"), _
                       Metric = m.Field(Of System.String)("Metric"), _
                       Scale = m.Field(Of System.String)("Scale"), _
                       Unit = m.Field(Of System.String)("Unit"), _
                       Value = m.Field(Of System.String)("Value"), _
                       Number = m.Field(Of System.Double?)("Number"), _
                       YTD = m.Field(Of System.Double?)("YTD"), _
                       PCYA = m.Field(Of System.Double?)("PCYA"), _
                       PYTD = m.Field(Of System.Double?)("PYTD")

                    Dim qF = From m In qM _
                             From a In Attributes _
                             Where m.FieldID = _
                                   a.Field(Of Integer)("FieldID")

                    ' Declare variables for DataRow objects.
                    Dim row As DataRow

                    For Each obs In qF

                        row = FTable.NewRow()
                        row("OName") = obs.m.OName
                        row("DataID") = obs.m.DataID
                        row("DateID") = obs.m.DateID
                        row("OID") = obs.m.OID
                        row("FieldID") = obs.m.FieldID
                        row("Metric") = obs.m.Metric
                        row("Scale") = obs.m.Scale
                        row("Unit") = obs.m.Unit
                        row("Value") = obs.m.Value
                        row("Number") = NullToDBNull(obs.m.Number)
                        row("YTD") = NullToDBNull(obs.m.YTD)
                        row("PCYA") = NullToDBNull(obs.m.PCYA)
                        row("PYTD") = NullToDBNull(obs.m.PYTD)

                        For Each code In dCS
                            row(code) = obs.a.Field(Of String)(code)
                        Next code
                        FTable.Rows.Add(row)
                    Next obs

                    progress.PrimaryPercent = 90
                    progress.PrimaryValue = 90

                    'writeToDataBase returns true or false where true means successful
                    updateFactTable = writeToDataBase(FTable, sFTName)


                Else
                    'Code to create FactTable TBD
                    Return False
                End If

            Catch ex As Exception
                gf.NotifyWebmaster(ex, gf.GetCurrentUrl, "updateFactTable error - SurveySeriesID: " & SurveySeries.ToString)
                Return False
            End Try

        End Function

        Private Function CheckForTable(ByVal tableName As String) As Boolean
            CheckForTable = False
            Dim conn As New SqlConnection
            Dim cmd As New SqlCommand    'Creates a new command object
            Dim count As Integer           'Creates a variable that will store the cmd return value
            Try



                conn.ConnectionString = gf.GetDBConnection.ToString
                conn.Open()
                cmd.CommandText = "SELECT COUNT(*) " & _
                                  "FROM sys.objects " & _
                                  "WHERE object_id = OBJECT_ID(N'[" & tableName & "]') " & _
                                  "AND type in (N'U')"


                cmd.Connection = conn    'Instructs the cmd object to use conn as its connection when executing
                count = cmd.ExecuteScalar            'Use ExecuteScalar to return a single value, the count, and assign it to the exists variable
                conn.Close()
                If count = 1 Then
                    Return True
                Else
                    Return False
                End If
            Catch ex As Exception
                gf.NotifyWebmaster(ex, gf.GetCurrentUrl, "CheckForTable error: " & tableName)
                Return False
            End Try

        End Function


        Protected Function NullToDBNull(ByVal d As Double?)
            If d.ToString = Nothing Then
                Return DBNull.Value
            Else
                Return d
            End If
        End Function

        Protected Function initializeFactTable(ByVal theSurveyID As Integer, ByVal theYear As Integer) As Boolean
            initializeFactTable = False
            ' Create a new DataTable.
            Dim FTable As DataTable = New DataTable("FactTable")

            ' Declare variables for DataColumn and DataRow objects.
            Dim column As DataColumn

            'NOTE: Order of columns important
            ' Create new DataColumn,set DataType, ColumnName 
            ' and add to DataTable.   
            Dim sC1(,) As String = New String(,) _
                 { _
                  {"DataID", "System.Int32"}, _
                  {"DateID", "System.DateTime"}, _
                  {"FieldID", "System.Int32"}, _
                  {"OID", "System.Int32"}, _
                  {"OName", "System.String"}, _
                  {"Metric", "System.String"}, _
                  {"Scale", "System.String"}, _
                  {"Unit", "System.String"} _
                 }

            For iC = 0 To sC1.GetUpperBound(0)
                column = New DataColumn()
                column.DataType = System.Type.GetType(sC1(iC, 1))
                column.ColumnName = sC1(iC, 0)
                column.AllowDBNull = True
                ' Add the Column to the DataColumnCollection.
                FTable.Columns.Add(column)
            Next iC


            ' Create attribute columns.
            Dim dCS = (From f In db.ddFields, a In db.ddAttributes, c In db.ddCodeSets _
                  Where f.SurveyID = theSurveyID AndAlso f.FieldID = a.FieldID AndAlso a.CodeSetID = c.CodeSetID _
                  Select c.CodeSet).Distinct.ToList
            For Each code In dCS
                column = New DataColumn()
                column.DataType = System.Type.GetType("System.String")
                column.ColumnName = code
                FTable.Columns.Add(column)
            Next code

            Dim sC2(,) As String = New String(,) _
                 { _
                  {"Value", "System.String"}, _
                  {"Number", "System.Double"}, _
                  {"YTD", "System.Double"}, _
                  {"PCYA", "System.Double"}, _
                  {"PYTD", "System.Double"} _
                 }
            For iC = 0 To sC2.GetUpperBound(0)
                column = New DataColumn()
                column.DataType = System.Type.GetType(sC2(iC, 1))
                column.ColumnName = sC2(iC, 0)
                column.AllowDBNull = True
                ' Add the Column to the DataColumnCollection.
                FTable.Columns.Add(column)
            Next iC

            ' Make the DataID column the primary key column.
            Dim PrimaryKeyColumns(0) As DataColumn
            PrimaryKeyColumns(0) = FTable.Columns("DataID")
            FTable.PrimaryKey = PrimaryKeyColumns

            ' Add the new DataTable to the DataSet.
            dataSet.Tables.Add(FTable)

        End Function


        Protected Function createNamesTable(ByVal theSurveyID As Integer, ByVal theYear As Integer, ByVal Hierarchy As String) As Boolean

            ' Create a new DataTable.
            Dim NTable As DataTable = New DataTable("Names")

            ' Declare variables for DataColumn and DataRow objects.
            Dim column As DataColumn
            Dim row As DataRow

            ' Create new DataColumn, set DataType, ColumnName 
            ' and add to DataTable.    
            column = New DataColumn()
            column.DataType = System.Type.GetType("System.Int32")
            column.ColumnName = "OID"

            ' Add the Column to the DataColumnCollection.
            NTable.Columns.Add(column)

            ' Create another new DataColumn, set DataType, ColumnName 
            ' and add to DataTable.    
            column = New DataColumn()
            column.DataType = System.Type.GetType("System.String")
            column.ColumnName = "OName"

            ' Add the Column to the DataColumnCollection.
            NTable.Columns.Add(column)


            ' Make the OID column the primary key column.
            Dim PrimaryKeyColumns(0) As DataColumn
            PrimaryKeyColumns(0) = NTable.Columns("OID")
            NTable.PrimaryKey = PrimaryKeyColumns

            ' Add the new DataTable to the DataSet.
            dataSet.Tables.Add(NTable)

            Dim qNames As IEnumerable = Nothing

            Select Case Hierarchy
                Case "S"
                    qNames = (From n In db.dSources _
                              Where n.SurveyID = theSurveyID _
                              Select _
                                OID = n.SourceID, _
                                OName = n.PrefName).ToArray

                Case "G"
                    '[TBD]
                Case "P"
                    '[TBD]

            End Select

            ' Create new DataRow objects and add 
            ' them to the DataTable
            For Each eRow In qNames
                row = NTable.NewRow()
                row("OID") = eRow.OID
                row("OName") = eRow.OName
                NTable.Rows.Add(row)
            Next

        End Function

        Protected Function createMeasureTable(ByVal theSurveyID As Integer, ByVal theYear As Integer) As Boolean
            createMeasureTable = False
            'Note OID is a generic reference to either SourceID, GroupID, or ParentID
            ' Create a new DataTable.
            Dim table As DataTable = New DataTable("Measures")

            ' Declare variables for DataColumn and DataRow objects.
            Dim column As DataColumn
            Dim row As DataRow


            ' Create new DataColumn,set DataType, ColumnName 
            ' and add to DataTable.   
            Dim sC(,) As String = New String(,) _
                 { _
                  {"DataID", "System.Int32"}, _
                  {"DateID", "System.DateTime"}, _
                  {"OID", "System.Int32"}, _
                  {"FieldID", "System.Int32"}, _
                  {"Metric", "System.String"}, _
                  {"Scale", "System.String"}, _
                  {"Unit", "System.String"}, _
                  {"Value", "System.String"}, _
                  {"Number", "System.Double"}, _
                  {"YTD", "System.Double"}, _
                  {"PCYA", "System.Double"}, _
                  {"PYTD", "System.Double"} _
                 }

            For iC = 0 To sC.GetUpperBound(0)
                column = New DataColumn()
                column.DataType = System.Type.GetType(sC(iC, 1))
                column.ColumnName = sC(iC, 0)
                column.AllowDBNull = True
                ' Add the Column to the DataColumnCollection.
                table.Columns.Add(column)
            Next iC

            ' Make the DataID column the primary key column.
            Dim PrimaryKeyColumns(0) As DataColumn
            PrimaryKeyColumns(0) = table.Columns("DataID")
            table.PrimaryKey = PrimaryKeyColumns

            ' Add the new DataTable to the DataSet.
            dataSet.Tables.Add(table)

            'Add reported Data to the new DataTable

            Dim dD = (From f In db.ddFields _
               Where f.SurveyID = theSurveyID _
               Select _
              f.FieldID, _
              f.DataTypeID, _
              f.ddMetric.Metric, _
              f.ddScale.Scale, _
              f.ddUnit.Unit).ToArray

            Dim fD = From f In db.fDatas _
             Where f.dSurveySeries.SurveyID = theSurveyID AndAlso _
                   (f.dSurveySeries.Year = (theYear - 1) Or f.dSurveySeries.Year = theYear) _
             Select _
             f.DataID, _
             f.dSurveySeries.DateID, _
             f.dWorkbook.SourceID, _
             f.FieldID, _
             f.Value, _
             f.Number

            'http://msdn.microsoft.com/en-us/library/bb918093.aspx
            ' Left Join in LINQ 
            Dim yD = (From f In fD _
               Group Join y In db.cYTDs _
               On f.DataID Equals y.DataID _
               Into children = Group _
               From child In children.DefaultIfEmpty _
                  Select New With { _
               .DataID = f.DataID, _
               .DateID = f.DateID, _
               .OID = f.SourceID, _
               .FieldID = f.FieldID, _
               .Value = f.Value, _
               .Number = f.Number, _
               .YTD = child.Number})

            Dim pyD = (From f In yD _
               Group Join p In db.cPCYAs _
               On f.DataID Equals p.DataID _
               Into children = Group _
               From child In children.DefaultIfEmpty _
                  Select New With { _
               .DataID = f.DataID, _
               .DateID = f.DateID, _
               .OID = f.OID, _
               .FieldID = f.FieldID, _
               .Value = f.Value, _
               .Number = f.Number, _
               .YTD = f.YTD, _
               .PCYA = child.Number})

            Dim ppyD = (From f In pyD _
             Group Join p In db.cPYTDs _
             On f.DataID Equals p.DataID _
             Into children = Group _
             From child In children.DefaultIfEmpty _
             Select New With { _
             .DataID = f.DataID, _
             .DateID = f.DateID, _
             .OID = f.OID, _
             .FieldID = f.FieldID, _
             .Value = f.Value, _
             .Number = f.Number, _
             .YTD = f.YTD, _
             .PCYA = f.PCYA, _
             .PYTD = child.Number})

            Dim qppyD = (From d In dD, f In ppyD _
              Where f.FieldID = d.FieldID _
              Select f.DataID, f.DateID, f.OID, d.FieldID, d.Metric, d.Scale, d.Unit, f.Value, f.Number, f.YTD, f.PCYA, f.PYTD _
              ).ToArray

            ' Create new DataRow objects and add 
            ' them to the DataTable
            For Each eRow In qppyD
                row = table.NewRow()
                row("DataID") = eRow.DataID
                row("DateID") = eRow.DateID
                row("OID") = eRow.OID
                row("FieldID") = eRow.FieldID
                row("Metric") = eRow.Metric
                row("Scale") = eRow.Scale
                row("Unit") = eRow.Unit
                row("Value") = eRow.Value
                row("Number") = NullToDBNull(eRow.Number)
                row("YTD") = NullToDBNull(eRow.YTD)
                row("PCYA") = NullToDBNull(eRow.PCYA)
                row("PYTD") = NullToDBNull(eRow.PYTD)
                table.Rows.Add(row)
            Next eRow


            'Add custom calculated Data to the new DataTable
            Dim cdD = (From f In db.ddcFields _
                       Where f.SurveyID = theSurveyID _
                       Select _
                          f.FieldID, _
                          f.DataTypeID, _
                          f.ddMetric.Metric, _
                          f.ddScale.Scale, _
                          f.ddUnit.Unit).ToArray

            Dim cD = From f In db.cDatas _
              Where f.dSurveySeries.SurveyID = theSurveyID AndAlso f.dSurveySeries.Year > (theYear - 1) _
              Select _
              f.DataID, _
              f.dSurveySeries.DateID, _
              f.dWorkbook.SourceID, _
              f.FieldID, _
              f.Value, _
              f.Number

            'http://msdn.microsoft.com/en-us/library/bb918093.aspx
            ' Left Join in LINQ 
            Dim cyD = (From f In cD _
               Group Join y In db.ccYTDs _
               On f.DataID Equals y.DataID _
               Into children = Group _
               From child In children.DefaultIfEmpty _
                  Select New With { _
               .DataID = f.DataID, _
               .DateID = f.DateID, _
               .OID = f.SourceID, _
               .FieldID = f.FieldID, _
               .Value = f.Value, _
               .Number = f.Number, _
               .YTD = child.Number})

            Dim cpyD = (From f In cyD _
               Group Join p In db.ccPCYAs _
               On f.DataID Equals p.DataID _
               Into children = Group _
               From child In children.DefaultIfEmpty _
                  Select New With { _
               .DataID = f.DataID, _
               .DateID = f.DateID, _
               .OID = f.OID, _
               .FieldID = f.FieldID, _
               .Value = f.Value, _
               .Number = f.Number, _
               .YTD = f.YTD, _
               .PCYA = child.Number})

            Dim cppyD = (From f In cpyD _
             Group Join p In db.ccPYTDs _
             On f.DataID Equals p.DataID _
             Into children = Group _
             From child In children.DefaultIfEmpty _
             Select New With { _
             .DataID = f.DataID, _
             .DateID = f.DateID, _
             .OID = f.OID, _
             .FieldID = f.FieldID, _
             .Value = f.Value, _
             .Number = f.Number, _
             .YTD = f.YTD, _
             .PCYA = f.PCYA, _
             .PYTD = child.Number})

            Dim qcppyD = (From d In cdD, f In cppyD _
              Where f.FieldID = d.FieldID _
              Select f.DataID, f.DateID, f.OID, d.FieldID, d.Metric, d.Scale, d.Unit, f.Value, f.Number, f.YTD, f.PCYA, f.PYTD _
              ).ToArray

            ' Create new DataRow objects and add 
            ' them to the DataTable
            For Each eRow In qcppyD
                row = table.NewRow()
                row("DataID") = eRow.DataID
                row("DateID") = eRow.DateID
                row("OID") = eRow.OID
                row("FieldID") = eRow.FieldID
                row("Metric") = eRow.Metric
                row("Scale") = eRow.Scale
                row("Unit") = eRow.Unit
                row("Value") = eRow.Value
                row("Number") = NullToDBNull(eRow.Number)
                row("YTD") = NullToDBNull(eRow.YTD)
                row("PCYA") = NullToDBNull(eRow.PCYA)
                row("PYTD") = NullToDBNull(eRow.PYTD)
                table.Rows.Add(row)
            Next eRow

            createMeasureTable = True

        End Function

        Protected Function createAttributeTable(ByVal theSurveyID As Integer, ByVal theYear As Integer) As Boolean
            createAttributeTable = False
            Dim dCS = (From f In db.ddFields, a In db.ddAttributes, c In db.ddCodeSets _
                Where f.SurveyID = theSurveyID AndAlso f.FieldID = a.FieldID AndAlso a.CodeSetID = c.CodeSetID _
                Select a.CodeSetID, c.CodeSet).Distinct.ToList

            Dim dA = (From f In db.ddFields _
               Where f.SurveyID = theSurveyID _
               Select New With _
                 { _
               .FieldId = f.FieldID, _
               .Attributes = _
                From a In db.ddAttributes, c In db.ddCodes _
                Where f.FieldID = a.FieldID AndAlso a.CodeID = c.CodeID _
                Select c.ddCodeSet.CodeSet, c.Text _
                 } _
               ).ToList

            ' Create a new DataTable.
            Dim ATable As DataTable = New DataTable("Attributes")

            ' Declare variables for DataColumn and DataRow objects.
            Dim column As DataColumn
            Dim row As DataRow

            ' Create new DataColumn, set DataType, ColumnName 
            ' and add to DataTable.    
            column = New DataColumn()
            column.DataType = System.Type.GetType("System.Int32")
            column.ColumnName = "FieldID"

            ' Add the Column to the DataColumnCollection.
            ATable.Columns.Add(column)

            ' Create attribute columns.
            For Each eC In dCS
                column = New DataColumn()
                column.DataType = System.Type.GetType("System.String")
                column.ColumnName = eC.CodeSet
                ATable.Columns.Add(column)
            Next

            ' Make the FieldID column the primary key column.
            Dim PrimaryKeyColumns(0) As DataColumn
            PrimaryKeyColumns(0) = ATable.Columns("FieldID")
            ATable.PrimaryKey = PrimaryKeyColumns

            ' Add the new DataTable to the DataSet.
            dataSet.Tables.Add(ATable)

            ' Create new DataRow objects and add 
            ' them to the DataTable
            For Each eRow In dA
                row = ATable.NewRow()
                row("FieldID") = eRow.FieldId
                For Each eC In eRow.Attributes
                    row(eC.CodeSet) = eC.Text
                Next
                ATable.Rows.Add(row)
            Next

            Dim dcA = (From f In db.ddcFields _
                      Where f.SurveyID = theSurveyID _
                      Select New With _
                       { _
                       .FieldId = f.FieldID, _
                       .Attributes = _
                        From a In db.ddAttributes, c In db.ddCodes _
                        Where f.FieldID = a.FieldID AndAlso a.CodeID = c.CodeID _
                        Select c.ddCodeSet.CodeSet, c.Text _
                         } _
                       ).ToList
            ' Create new DataRow objects and add 
            ' them to the DataTable
            For Each eRow In dcA
                row = ATable.NewRow()
                row("FieldID") = eRow.FieldId
                For Each eC In eRow.Attributes
                    row(eC.CodeSet) = eC.Text
                Next
                ATable.Rows.Add(row)
            Next

            createAttributeTable = True

        End Function

        Protected Sub makeDataRelation()
            ' DataRelation requires two DataColumn 
            ' (parent and child) and a name.
            Dim parentColumn As DataColumn = _
             dataSet.Tables("Measures").Columns("FieldID")
            Dim childColumn As DataColumn = _
             dataSet.Tables("Attributes").Columns("FieldID")
            Dim relation As DataRelation = New  _
             DataRelation("Measures2Attributes", parentColumn, childColumn, False)
            dataSet.Tables("Attributes").ParentRelations.Add(relation)
        End Sub


        Protected Function writeToDataBase(ByVal T As DataTable, ByVal sTableName As String) As Boolean
            writeToDataBase = False
            Try

                Dim connectionString As String = gf.GetDBConnection.ToString

                ' Open the destination connection. 
                Using destinationConnection As SqlConnection = New SqlConnection(connectionString)

                    destinationConnection.Open()
                    Dim nonqueryCommand As SqlCommand = destinationConnection.CreateCommand()

                    'Set up and execute DELETE Command
                    nonqueryCommand.CommandText = "DELETE FROM " & sTableName
                    nonqueryCommand.ExecuteNonQuery()

                    ' Set up the bulk copy object. 
                    ' The column positions in the source datatable 
                    ' match the column positions in the destination table, 
                    ' so there is no need to map columns.
                    Using bulkCopy As SqlBulkCopy = New SqlBulkCopy(destinationConnection)
                        bulkCopy.DestinationTableName = sTableName
                        Try
                            ' Write from the source to the destination.
                            bulkCopy.WriteToServer(T)
                            writeToDataBase = True
                        Catch ex As Exception
                            Console.WriteLine(ex.Message)
                            gf.NotifyWebmaster(ex, gf.GetCurrentUrl, "writeToDataBase error - Table name: " & sTableName.ToString)
                            writeToDataBase = False
                        Finally
                            'The SqlBulkCopy object is automatically closed 
                            ' at the end of the Using block.
                            destinationConnection.Close()
                        End Try
                    End Using
                End Using

                Return True
            Catch ex As Exception
                gf.NotifyWebmaster(ex, gf.GetCurrentUrl, "Create Fact Table writeToDatabase error: " & sTableName)
                Return False
            End Try

        End Function


#End Region



    End Class

End Namespace
