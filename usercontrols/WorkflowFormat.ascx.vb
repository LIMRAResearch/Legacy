
Imports System.Configuration
Imports System.Data.SqlClient
Imports System.Data

Imports System.Diagnostics

Imports Telerik.Web.UI
Imports Telerik.Web.UI.Upload

Imports SalesSurveysApplication.Globals   'located in Classes folder
Imports SalesSurveysApplication.Projects

Partial Public Class WorkflowFormat
    Inherits System.Web.UI.UserControl

   
    Protected gF As Globals.GlobalFunctions = New Globals.GlobalFunctions
    Protected gV As Globals.GlobalVariables = New Globals.GlobalVariables
    Protected pF As Projects.ProjectFunctions = New Projects.ProjectFunctions

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Page.User.IsInRole("SuperAdmin") OrElse Page.User.IsInRole("Admin") OrElse _
            Page.User.IsInRole("SuperAnalyst") OrElse Page.User.IsInRole("Analyst") Then
            MultiView1.SetActiveView(vwDefault)
            If Not Page.IsPostBack Then
                Dim sMessage As String = ""
                Try
                    SetPageTitle(Session("SurveyName"))
                    GetFileLists()
                    '   displayMessageBox("User Control Load")
                Catch ex As Exception
                    gF.NotifyWebmaster(ex, Request.ServerVariables.Item("PATH_INFO"), "Format Page LoadComplete Error")
                    sMessage = "Format Page LoadComplete Error"
                    displayMessageBox(sMessage)
                End Try
            End If
        Else
            MultiView1.SetActiveView(vwNotAuthorized)
        End If

    End Sub


    Protected Sub btnCopy_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCopy.Click

        Dim sMessage As String = ""
        Literal1.Text = ""

        Try
            copyWorkbook()
            GetFileLists()
        Catch ex As Exception
            gF.NotifyWebmaster(ex, Request.ServerVariables.Item("PATH_INFO"), "Format Copy Button Click Error")
            sMessage = "Format Copy Button Click Error"
            displayMessageBox(sMessage)
        End Try

    End Sub

    Protected Sub btnOpen_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnOpen.Click

        Dim sPath As String
        Dim sFilePath As String
        Dim sMessage As String = ""

        Try
            If Target.SelectedValue = Nothing Then
                Literal1.Text = "No Workbook Selected!"
            Else
                sPath = pF.getSurveyFilePath(CType(Session("SurveyID"), Integer))
                sFilePath = "File:" & sPath & "Data\1-Formatting\" & Target.SelectedValue
                Response.Redirect(sFilePath, False)
            End If
        Catch ex As Exception
            gF.NotifyWebmaster(ex, Request.ServerVariables.Item("PATH_INFO"), "Format Button Open Click Error")
            sMessage = "Format Button Open Click Error"
            displayMessageBox(sMessage)
        End Try

    End Sub

    Protected Sub copyWorkbook()

        Dim oxlWB As SpreadsheetGear.IWorkbook
        Dim oxlWBTemplate As SpreadsheetGear.IWorkbook
        Dim oxlWS As SpreadsheetGear.IWorksheet

       

        Dim intR As Integer
        Dim intC As Integer
        Dim sPath As String = ""
        Dim sFilePath As String = ""
        Dim tFilePath As String = ""
        Dim sMessage As String = ""
        Dim strSrc As String = ""

        Try

            Dim oDD = pF.getDataDictionary(CType(Session("SurveyID"), Integer))

            If Not oDD Is Nothing Then


                sPath = pF.getSurveyFilePath(CType(Session("SurveyID"), Integer))
                sFilePath = sPath & "Data\0-Submitted\" & Source.SelectedValue

                oxlWBTemplate = SpreadsheetGear.Factory.GetWorkbook(sPath & "Data\8-Templates\CurrentSurvey.xls")
                oxlWB = SpreadsheetGear.Factory.GetWorkbook(sFilePath)

                'This code addresses an issue with worksheet names
                'They may have these worksheets:
                '|Wirehouse |Financial Institutions|Other |
                'Which are renamed to these worksheets (note extra space and rename):
                '|Wirehouse|Banks&SavingsInstitutions|Other|
                For Each oxlWS In oxlWB.Worksheets
                    If oxlWS.Name = "Wirehouse " Then oxlWS.Name = "Wirehouse"
                    If oxlWS.Name = "Financial Institutions" Then oxlWS.Name = "Banks&SavingsInstitutions"
                    If oxlWS.Name = "Other " Then oxlWS.Name = "Other"
                Next

                compareWorkbooks(oxlWBTemplate, oxlWB, sMessage)
                oxlWBTemplate.Close()

                oxlWS = oxlWB.Worksheets(1)

                'Dim Fields = From Field In oDD _
                '             Select Field.SurveyID, Field.Field, Field.xlSheet, Field.xlRow, Field.xlColumn, Field.SortKey _
                '             Where SurveyID = CType(Session("SurveyID"), Integer) _
                '             Order By SortKey Ascending

                For Each Field In oDD
                    oxlWS = oxlWB.Worksheets(Field.xlSheet.ToString)
                    intR = Field.xlRow
                    intC = Field.xlColumn
                    strSrc = Field.Source

                    ' The workbook may have the character code of 160 for the space rather than the character code of 32. 
                    ' The trim function will not clean this type of space.
                    ' Also spreadsheetgear use base zero(0) 
                    ' If oxlWS.Cells(intR - 1, intC - 1).Value = True OrElse oxlWS.Cells(intR - 1, intC - 1).Value = False Then


                    If Len(Trim(oxlWS.Cells(intR - 1, intC - 1).Value)) > 0 Then
                        oxlWS.Cells(intR - 1, intC - 1).Value = (oxlWS.Cells(intR - 1, intC - 1).Value.ToString).Replace(Chr(160), Chr(32))
                    End If
                    ' Cleanout space fill cells
                    If Len(Trim(oxlWS.Cells(intR - 1, intC - 1).Value)) = 0 Then oxlWS.Cells(intR - 1, intC - 1).Value = ""

                    If strSrc <> "A" Then
                        oxlWS.Cells(intR - 1, intC - 1).Interior.Color = System.Drawing.Color.LightYellow  ' RGB(255, 255, 153) Light Yellow see http://www.mvps.org/dmcritchie/excel/colors.htm
                    End If

                Next


                tFilePath = sPath & "Data\1-Formatting\" & oxlWB.Name
                oxlWB.Worksheets("Cover").Select()

                Select Case oxlWB.FileFormat
                    Case SpreadsheetGear.FileFormat.Excel8
                        oxlWB.SaveAs(tFilePath, SpreadsheetGear.FileFormat.Excel8)
                    Case SpreadsheetGear.FileFormat.OpenXMLWorkbook
                        oxlWB.SaveAs(tFilePath, SpreadsheetGear.FileFormat.OpenXMLWorkbook)
                    Case Else
                        Throw New Exception("Invalid Excel Format")
                End Select

                oxlWB.Close()

                oxlWB = Nothing
                oxlWS = Nothing

                Literal1.Text = sMessage.ToString
            Else
                Throw New System.Exception("getDataDictionary error for survey " & Session("SurveyID").ToString & "- workflow format module")
            End If
        Catch ex As Exception
            displayMessageBox("An error has occurred see systems administrator!")
            gF.NotifyWebmaster(ex, Request.ServerVariables.Item("PATH_INFO"), "Format Copy Workbook Error")
            sMessage = "Format Copy Workbook Error"
            displayMessageBox(sMessage)
        End Try
       

    End Sub

    Protected Sub compareWorkbooks(ByRef oXLWBSource As SpreadsheetGear.IWorkbook, ByRef oXLWBTarget As SpreadsheetGear.IWorkbook, ByRef sMessage As String)

        Dim oXLWorksheet As SpreadsheetGear.IWorksheet
        Dim iIndex As Integer = 0
        Dim ltIndex As Integer = 0
        Dim I As Integer
        Dim lTargetName As String = ""

        Try

            Dim xlWSSource = From xlWBSheet As SpreadsheetGear.IWorksheet In oXLWBSource.Worksheets Select xlWBSheet.Name
            Dim xlWSTarget = From xlWBSheet As SpreadsheetGear.IWorksheet In oXLWBTarget.Worksheets Select xlWBSheet.Name

            ' if intersection is empty, xlNames.Count will be zero(0)
            Dim xlWSsMissing = xlWSSource.Except(xlWSTarget)
            For I = 0 To xlWSTarget.Count - 1
                lTargetName = oXLWBTarget.Worksheets(I).Name
            Next
            ltIndex = oXLWBSource.Worksheets(lTargetName).Index


            If xlWSsMissing.Count > 0 Then
                'Copy missing sheets syntax is:
                'Dim workbook1 As IWorkbook = Factory.GetWorkbook("workbook1.xlsx")
                'Dim workbook2 As IWorkbook = Factory.GetWorkbook("workbook2.xlsx")
                'workbook2.Worksheets("SheetToCopy").CopyBefore(workbook1.Worksheets("DestinationSheet"))

                sMessage = sMessage & "The following worksheets were missing and added to the workbook: "
                For Each xlSheet In xlWSsMissing
                    'sMessage = sMessage & vbCrLf & xlSheet.ToString
                    sMessage = sMessage & "<br />" & xlSheet.ToString
                    iIndex = oXLWBSource.Worksheets(xlSheet.ToString).Index
                    If iIndex < ltIndex Then
                        oXLWorksheet = oXLWBSource.Worksheets(xlSheet.ToString).CopyBefore(oXLWBTarget.Worksheets(iIndex))
                        oXLWorksheet.Name = xlSheet.ToString
                    Else
                        oXLWorksheet = oXLWBSource.Worksheets(xlSheet.ToString).CopyAfter(oXLWBTarget.Worksheets(iIndex - 1))
                        oXLWorksheet.Name = xlSheet.ToString
                    End If
                Next
            Else
                sMessage = sMessage & "No missing worksheets."
            End If

            'Response.Write("<script language='javascript'>alert('" & sMessage & "');</script>")

        Catch ex As Exception
            displayMessageBox("An error has occurred see systems administrator!")
            gF.NotifyWebmaster(ex, Request.ServerVariables.Item("PATH_INFO"), "Format Compare Workbook Error")
            sMessage = "Format Compare Workbook Error"
            displayMessageBox(sMessage)
        End Try

    End Sub


    Protected Sub GetFileLists()

        Dim sMessage As String = ""

        Try
            Dim myFileInfo As IEnumerable(Of String)
            Dim sPath As String = ""

            sPath = pF.getSurveyFilePath(CType(Session("SurveyID"), Integer))
            myFileInfo = pF.getNewlySubmittedWorkbooks(CType(Session("SurveyID"), Integer)) 'Need to use surveyID
            Source.DataSource = myFileInfo.ToList()
            Source.DataBind()

            myFileInfo = gF.getXLWBDirectory(sPath & "Data\1-Formatting\")
            Target.DataSource = myFileInfo.ToList()
            Target.DataBind()

        Catch ex As Exception
            ' gF.NotifyWebmaster(ex, Request.ServerVariables.Item("PATH_INFO"), "Format Get File Lists Error")
            sMessage = "Format Get File Lists Error"
            displayMessageBox(sMessage)
        End Try

    End Sub

    Protected Sub displayMessageBox(ByVal msg As String)

        Page.ClientScript.RegisterStartupScript(Me.GetType(), "AlertMessage", "alert('" + msg + "');", True)

    End Sub

    Private Sub SetPageTitle(ByVal TitleText As String)

        If Me.Page.Form.Parent IsNot Nothing Then
            Dim litPageTitle As Literal = Me.Page.Form.Parent.FindControl("litPageTitle")

            If litPageTitle IsNot Nothing Then
                litPageTitle.Text = TitleText
            End If
        End If

    End Sub

End Class