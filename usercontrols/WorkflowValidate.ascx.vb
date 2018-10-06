Imports System.Data.SqlClient
Imports System.Data
Imports System.Configuration
Imports System.Diagnostics

Imports Telerik.Web.UI
Imports Telerik.Web.UI.Upload

Imports SalesSurveysApplication.Globals   'located in Classes folder
Imports SalesSurveysApplication.Projects  'located in Classes folder

Partial Public Class WorkflowValidate
    Inherits System.Web.UI.UserControl

    Protected gF As Globals.GlobalFunctions = New Globals.GlobalFunctions
    Protected gV As Globals.GlobalVariables = New Globals.GlobalVariables
    Protected pF As Projects.ProjectFunctions = New Projects.ProjectFunctions

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
                    gF.NotifyWebmaster(ex, Request.ServerVariables.Item("PATH_INFO"), "Validate Page Load Complete File List Error")
                    sMessage = "Validate Page Load Complete File List Error"
                    displayMessageBox(sMessage)
                End Try
            End If
          
        Else
            MultiView1.SetActiveView(vwNotAuthorized)
        End If

    End Sub

    Protected Sub btnMove_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnMove.Click

        Dim sMessage As String = ""
        Literal1.Text = ""

        Try
            If Source.SelectedValue = Nothing Then
                Literal1.Text = "No Workbook Selected!"
            Else
                moveWorkbook()
                GetFileLists()
            End If
        Catch ex As Exception
            gF.NotifyWebmaster(ex, Request.ServerVariables.Item("PATH_INFO"), "Validate Workbook Move Button Error")
            sMessage = "Validate Workbook Move Button Error"
            displayMessageBox(sMessage)
        End Try

    End Sub

    Protected Sub btnOpenLeft_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnOpenLeft.Click

        Dim sPath As String
        Dim sFilePath As String
        Dim sMessage As String = ""

        Try
            If Source.SelectedValue = Nothing Then
                Literal1.Text = "No Workbook Selected!"
            Else
                sPath = pF.getSurveyFilePath(CType(Session("SurveyID"), Integer))
                sFilePath = sPath & "Data\1-Formatting\" & Source.SelectedValue
                openAWorkbook(sFilePath)
            End If
        Catch ex As Exception
            gF.NotifyWebmaster(ex, Request.ServerVariables.Item("PATH_INFO"), "Validate Open Left Workbook Error")
            sMessage = "Validate Open Left Workbook Error"
            displayMessageBox(sMessage)
        End Try

    End Sub

    Protected Sub btnOpenRight_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnOpenRight.Click
        Dim sPath As String
        Dim sFilePath As String
        Dim sMessage As String = ""

        Try
            If Target.SelectedValue = Nothing Then
                Literal1.Text = "No Workbook Selected!"
            Else
                sPath = pF.getSurveyFilePath(CType(Session("SurveyID"), Integer))
                sFilePath = sPath & "Data\2-Validating\" & Target.SelectedValue
                openAWorkbook(sFilePath)

            End If
        Catch ex As Exception
            gF.NotifyWebmaster(ex, Request.ServerVariables.Item("PATH_INFO"), "Validate Open Right Workbook Error")
            sMessage = "Validate Open Right Workbook Error"
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

    Protected Sub moveWorkbook()

        Dim oxlWB As SpreadsheetGear.IWorkbook
        Dim oxlWS As SpreadsheetGear.IWorksheet



        Dim intR As Integer
        Dim intC As Integer
        Dim sPath As String = ""
        Dim sFilePath As String = ""
        Dim tFilePath As String = ""
        Dim sMessage As String = ""
        Dim strSrc As String = ""
        Literal1.Text = ""

        Try

            Dim oDD = pF.getDataDictionary(CType(Session("SurveyID").ToString, Integer))

            sPath = pF.getSurveyFilePath(CType(Session("SurveyID"), Integer))
            sFilePath = sPath & "Data\1-Formatting\" & Source.SelectedValue
            tFilePath = sPath & "Data\2-Validating\" & Source.SelectedValue

            oxlWB = SpreadsheetGear.Factory.GetWorkbook(sFilePath)

            'Validate numeric data assumes standard exception values 'M' or <null>
            'Dim Fields = From Field In oDD _
            '             Select Field.SurveyID, Field.Field, Field.xlSheet, Field.xlRow, Field.xlColumn, Field.DataType, Field.SortKey _
            '             Order By SortKey Ascending

            For Each Field In oDD
                oxlWS = oxlWB.Worksheets(Field.xlSheet.ToString)
                intR = Field.xlRow
                intC = Field.xlColumn
                strSrc = Field.Source

                Select Case Field.DataType
                    Case "Number"
                        ' The workbook may have the character code of 160 for the space rather than the character code of 32. 
                        ' The trim function will not clean this type of space.
                        ' Use =TRIM(SUBSTITUTE(<cell reference>,CHR(160),CHR(32))) 
                        ' Also spreadsheetgear use base zero(0) 
                        If Len(Trim(oxlWS.Cells(intR - 1, intC - 1).Value)) > 0 Then
                            oxlWS.Cells(intR - 1, intC - 1).Value = (oxlWS.Cells(intR - 1, intC - 1).Value.ToString).Replace(Chr(160), Chr(32))
                        End If

                        If Len(Trim(oxlWS.Cells(intR - 1, intC - 1).Value)) = 0 Then
                            oxlWS.Cells(intR - 1, intC - 1).Interior.Color = System.Drawing.Color.LightGreen  'set to light green
                        ElseIf IsNumeric(oxlWS.Cells(intR - 1, intC - 1).Value) Then
                            oxlWS.Cells(intR - 1, intC - 1).Interior.Color = System.Drawing.Color.LightGreen  'set to light green
                        ElseIf oxlWS.Cells(intR - 1, intC - 1).Value.ToString = "M" Then
                            oxlWS.Cells(intR - 1, intC - 1).Interior.Color = System.Drawing.Color.LightGreen  'set to light green
                        Else
                            sMessage = sMessage & "Worksheet:" & Field.xlSheet & " Row:" & Field.xlRow & " Column:" & Field.xlColumn & vbCrLf
                        End If
                    Case "Date"
                        ' The workbook may have the character code of 160 for the space rather than the character code of 32. 
                        ' The trim function will not clean this type of space.
                        ' Use =TRIM(SUBSTITUTE(<cell reference>,CHR(160),CHR(32))) 
                        ' Also spreadsheetgear use base zero(0) 
                        If Len(Trim(oxlWS.Cells(intR - 1, intC - 1).Value)) > 0 Then
                            oxlWS.Cells(intR - 1, intC - 1).Value = (oxlWS.Cells(intR - 1, intC - 1).Value.ToString).Replace(Chr(160), Chr(32))
                        End If

                        Dim TestDate As Date
                        TestDate = oxlWB.NumberToDateTime(oxlWS.Cells(intR - 1, intC - 1).Value)

                        If Len(Trim(oxlWS.Cells(intR - 1, intC - 1).Value)) = 0 Then
                            oxlWS.Cells(intR - 1, intC - 1).Interior.Color = System.Drawing.Color.LightGreen  'set to light green
                        ElseIf IsDate(TestDate) Then
                            oxlWS.Cells(intR - 1, intC - 1).Interior.Color = System.Drawing.Color.LightGreen  'set to light green
                        ElseIf oxlWS.Cells(intR - 1, intC - 1).Value.ToString = "M" Then
                            oxlWS.Cells(intR - 1, intC - 1).Interior.Color = System.Drawing.Color.LightGreen  'set to light green
                        Else
                            sMessage = sMessage & "Worksheet:" & Field.xlSheet & " Row:" & Field.xlRow & " Column:" & Field.xlColumn & vbCrLf
                        End If
                    Case "Text"
                        'If CType(Session("SurveyID"), Integer) = 4 Then
                        'text cells not validated
                        If strSrc <> "A" Then
                            oxlWS.Cells(intR - 1, intC - 1).Interior.Color = System.Drawing.Color.LightGreen
                        End If
                        'End If
                End Select
            Next


            Select Case Len(sMessage)
                Case 0
                    sMessage = "No validation errors!"
                Case Is > 512
                    sMessage = "Too many validation errors to list!"
            End Select
            displayMessageBox(sMessage)

            oxlWB.Worksheets("Cover").Select()

            Select Case oxlWB.FileFormat
                Case SpreadsheetGear.FileFormat.Excel8
                    oxlWB.SaveAs(sFilePath, SpreadsheetGear.FileFormat.Excel8)
                Case SpreadsheetGear.FileFormat.OpenXMLWorkbook
                    oxlWB.SaveAs(sFilePath, SpreadsheetGear.FileFormat.OpenXMLWorkbook)
                Case Else
                    Throw New Exception("Invalid Excel Format")
            End Select


            oxlWB.Close()
            gF.moveFile(sFilePath, tFilePath)

            oxlWB = Nothing
            oxlWS = Nothing

            Literal1.Text = sMessage

        Catch ex As Exception
            gF.NotifyWebmaster(ex, Request.ServerVariables.Item("PATH_INFO"), "Validate Move Workbook Error")
            sMessage = "Validate Move Workbook Error"
            displayMessageBox(sMessage)
        End Try

    End Sub

    Protected Sub openAWorkbook(ByVal sFilePath As String)
        Dim sMessage As String = ""

        Try
            sFilePath = "File:" & sFilePath
            Response.Redirect(sFilePath, False)
        Catch ex As Exception
            gF.NotifyWebmaster(ex, Request.ServerVariables.Item("PATH_INFO"), "Validate Open A Workbook Error")
            sMessage = "Validate Open A Workbook Error"
            displayMessageBox(sMessage)
        End Try

    End Sub

    Protected Sub GetFileLists()

        Dim sMessage As String = ""

        Try
            Dim myFileInfo As IEnumerable(Of String)
            Dim sPath As String = ""

            sPath = pF.getSurveyFilePath(CType(Session("SurveyID"), Integer))
            myFileInfo = gF.getXLWBDirectory(sPath & "Data\1-Formatting\")
            Source.DataSource = myFileInfo.ToList()
            Source.DataBind()

            myFileInfo = gF.getXLWBDirectory(sPath & "Data\2-Validating\")
            Target.DataSource = myFileInfo.ToList()
            Target.DataBind()

        Catch ex As Exception
            gF.NotifyWebmaster(ex, Request.ServerVariables.Item("PATH_INFO"), "Validate Get File Lists Error")
            sMessage = "Validate Get File Lists Error"
            displayMessageBox(sMessage)
        End Try

    End Sub

    Private Sub displayMessageBox(ByVal msg As String)

        Page.ClientScript.RegisterStartupScript(Me.GetType(), "AlertMessage", "alert('" + msg + "');", True)

    End Sub

End Class