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
Imports System.Web.HttpContext


Namespace Globals

    Public Class GlobalVariables

        'Public strRootFolder As String = "M:\Sales Survey Analysis System\2-ColiBoli\Data" & "\"
        Public blnExcelWasAlreadyRunning As Boolean

    End Class

    Public Class GlobalFunctions

        Dim gVariables As Globals.GlobalVariables = New Globals.GlobalVariables



        Public Function addWorksheet(ByVal sheetName As String, ByRef oxlWB As Excel.Workbook) As Boolean
            'Add a worksheet to beginning of the open workbook, oxlWB 
            'Assumes Excel is running
            'Calls: isExcelRunning
            'Needs: code to check worksheet not already in workbook

            Dim oxlApp As Excel.Application = Nothing
            Dim oxlWS As Excel.Worksheet
            Dim blnSuccess As Boolean

            Try
                ' Get Excel Application object. 
                blnSuccess = isExcelRunning(oxlApp)

                ' Add worksheet to beginning of workbook
                oxlWS = oxlWB.Worksheets.Add(Before:=oxlWB.Worksheets(1))
                oxlWS = oxlWB.ActiveSheet
                oxlWS.Name = sheetName
                addWorksheet = True

            Catch ex As Exception
                addWorksheet = False

            End Try

        End Function

        Public Function copyWorksheet(ByRef oxlWSSource As Excel.Worksheet, ByRef oXLWBTarget As Excel.Workbook) As Boolean
            'Copy a worksheet to beginning of the open workbook, oxlWB 
            'Assumes Excel is running
            'Calls: isExcelRunning
            'Needs: 
            '      code to check Excel is running
            '      code to check oxlWSSource not already in oXLWBTarget

            Dim oxlApp As Excel.Application = Nothing
            Dim blnSuccess As Boolean
            Dim iBefore As Integer

            Try
                ' Get Excel Application object. 
                blnSuccess = isExcelRunning(oxlApp)

                ' Add worksheet to to target workbook in same order as source
                iBefore = oxlWSSource.Index
                oxlWSSource.Copy(Before:=oXLWBTarget.Worksheets(iBefore))
                copyWorksheet = True

            Catch ex As Exception
                copyWorksheet = False

            End Try
        End Function

        Public Function isWorksheet(ByVal sheetName As String, ByRef oxlWB As Excel.Workbook) As Boolean
            'Determines if worksheet in the open workbook, oxlWB 
            'Assumes Excel is running
            'Calls: isExcelRunning

            Dim oxlApp As Excel.Application = Nothing
            Dim oxlWS As Excel.Worksheet
            Dim blnSuccess As Boolean

            Try
                blnSuccess = isExcelRunning(oxlApp)
                Select Case blnSuccess
                    Case True
                        isWorksheet = False
                        For Each oxlWS In oxlWB.Worksheets
                            If oxlWS.Name = sheetName Then isWorksheet = True
                        Next
                    Case False
                        isWorksheet = False
                End Select

            Catch ex As Exception
                isWorksheet = False
            End Try

        End Function

        Public Function closeExcel() As Boolean
            Dim oxlApp As Excel.Application = Nothing
            Try
                Select Case gVariables.blnExcelWasAlreadyRunning
                    Case True
                        closeExcel = True
                    Case False
                        If isExcelRunning() Then
                            oxlApp = CType(GetObject(, "Excel.Application"), Excel.Application)
                            oxlApp.Quit()
                        End If
                        closeExcel = True
                End Select

            Catch ex As Exception
                closeExcel = False

            End Try
        End Function

        Public Function isExcelRunning(Optional ByRef oxlApp As Excel.Application = Nothing) As Boolean
            'Reference: http://xldennis.wordpress.com/2007/11/25/access-running-instances-of-excel-in-vb/
            Dim sMessage As String
            'sMessage = "isExcelRunning got to"
            'displayMessageBox(sMessage)
            Try
                Select Case Process.GetProcessesByName("Excel").Length
                    Case 0
                        isExcelRunning = False
                        gVariables.blnExcelWasAlreadyRunning = False
                    Case Is > 0
                        isExcelRunning = True
                        gVariables.blnExcelWasAlreadyRunning = True
                        oxlApp = CType(GetObject(, "Excel.Application"), Excel.Application)
                End Select

            Catch ex As Exception
                sMessage = "isExcelRunning Function Error"
                displayMessageBox(sMessage)
                isExcelRunning = False
            End Try

        End Function

        Public Function isWorkbookOpen(ByVal workbookName As String) As Boolean
            'Assumes Excel is running
            'Calls: isExcelRunning

            Dim oxlApp As Excel.Application = Nothing
            Dim oxlWB As Excel.Workbook = Nothing
            Dim blnSuccess As Boolean

            Try
                ' Get Excel Application object. 
                blnSuccess = isExcelRunning(oxlApp)
                Select Case blnSuccess
                    Case True
                        isWorkbookOpen = False
                        For Each oxlWB In oxlApp.Workbooks
                            If oxlWB.Name.ToString = workbookName Then isWorkbookOpen = True
                        Next
                    Case False
                        isWorkbookOpen = False
                End Select

            Catch ex As Exception

            End Try
        End Function

        Public Function openWorkbook(ByVal workbookPath As String, Optional ByRef oxlWB As Excel.Workbook = Nothing) As Boolean
            'Assumes Excel is running
            'Calls: isExcelRunning
            'Calls: isWorkbookOpen to check workbook not already open
            'Needs: code to check workbook exists

            Dim oxlApp As Excel.Application = Nothing
            Dim blnSuccess As Boolean

            Try
                ' Get Excel Application object. 
                blnSuccess = isExcelRunning(oxlApp)
                Select Case blnSuccess
                    Case True
                        If isWorkbookOpen(workbookPath) Then
                            openWorkbook = True
                        Else
                            oxlWB = oxlApp.Workbooks.Open(workbookPath)
                            openWorkbook = True
                        End If

                    Case False
                        openWorkbook = False
                End Select

            Catch ex As Exception
                openWorkbook = False

            End Try


        End Function

        Public Function startExcel(ByRef oxlApp As Excel.Application, Optional ByVal blnVisible As Boolean = False) As Boolean
            'Starts excel if not running
            'Return False if Excel is already running and pass Instance
            'Calls: isExcelRunning

            Try
                Select Case isExcelRunning(oxlApp) 'Note: isExcelRunning, using byRef oxlApp, returns Excel Instance if Excel running
                    Case True
                        startExcel = False
                    Case False
                        oxlApp = New Excel.Application
                        oxlApp.Visible = blnVisible
                        startExcel = True
                End Select

            Catch ex As Exception

            End Try

        End Function


        Public Function getXLWBDirectory(ByVal strPath) As IEnumerable(Of String)
            Dim files = From file In My.Computer.FileSystem.GetFiles(strPath) _
                    Order By file _
                    Select file

            'TODO: RP used "Take" because filesInfo can only handle ~2000 rows'
            Dim filesInfo = From file In files _
                        Select My.Computer.FileSystem.GetFileInfo(file).Name Take (1500)

            Dim myFileInfo = From file In filesInfo _
                             Select filename = file
                             Where filename.EndsWith(".xls") Or filename.EndsWith(".xlsx") And Not filename.StartsWith("~")
            getXLWBDirectory = myFileInfo
        End Function

        Public Function copyFile(ByVal SFile As String, ByVal DFile As String) As Boolean

            Try
                My.Computer.FileSystem.CopyFile(SFile, DFile, True)
                copyFile = True
            Catch ex As Exception
                copyFile = False
            End Try

        End Function

        Public Function moveFile(ByVal SFile As String, ByVal DFile As String) As Boolean

            Try
                My.Computer.FileSystem.MoveFile(SFile, DFile, True)
            Catch ex As Exception
                moveFile = False
            End Try

        End Function


        ''' <summary>
        ''' Emails a message to the webmaster - used for errors outside of try catch statements
        ''' </summary>
        ''' <param name="ErrMsgSubject">String</param>
        ''' <param name="CurrentURL">String</param>
        ''' <param name="PrgMsg">String</param>
        ''' <remarks></remarks>
        Public Sub NotifyWebmaster(ByVal ErrMsgSubject As String, ByVal CurrentURL As String, ByVal PrgMsg As String)
            Dim SmtpClient As SmtpClient = New SmtpClient()
            SmtpClient.Host = System.Configuration.ConfigurationManager.AppSettings("mailServer")
            Dim Mail As MailMessage = New MailMessage()

            'To/From:
            Mail.To.Add(System.Configuration.ConfigurationManager.AppSettings("WebMaster"))
            Mail.From = New System.Net.Mail.MailAddress(System.Configuration.ConfigurationManager.AppSettings("WebMaster"))

            'Sets the subject line including the type of error, and the description of the error.
            Mail.Subject = "Sales Surveys Error: " & ErrMsgSubject

            'Message Body
            Mail.IsBodyHtml = False
            Mail.Body = ""
            Mail.Body &= Now().ToShortDateString & " " & Now().ToShortTimeString & vbCrLf & vbCrLf
            Mail.IsBodyHtml = False
            Mail.Body = "A  error occurred while displaying this page: " & GetCurrentUrl() & vbCrLf & vbCrLf
            Mail.Body &= "User: " & Current.Session("FirstName") & " " & Current.Session("LastName") & " (OnyxID-" & Current.Session("OnyxID") & ")" & vbCrLf & vbCrLf
            Mail.Body &= "Message: " & PrgMsg & vbCrLf & vbCrLf

            Try
                '#If Not Debug Then
                SmtpClient.Send(Mail)
'#End If
            Catch ex As Exception
                'Fail Silently
            End Try


        End Sub

        'part of the error handling process - sends a detailed email message to the email on file as "WebMaster" in the web.config file
        Public Sub NotifyWebmaster(ByVal Exc As System.Exception, ByVal CurrentURL As String, ByVal PrgMsg As String)
            Dim SmtpClient As SmtpClient = New SmtpClient()
            SmtpClient.Host = System.Configuration.ConfigurationManager.AppSettings("mailServer")
            Dim Mail As MailMessage = New MailMessage()

            'To/From:
            Mail.To.Add(System.Configuration.ConfigurationManager.AppSettings("WebMaster"))
            Mail.From = New System.Net.Mail.MailAddress(System.Configuration.ConfigurationManager.AppSettings("WebMaster"))

            If Not IsNothing(System.Configuration.ConfigurationManager.AppSettings("WebMasterBCC")) AndAlso Not String.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings("WebMasterBCC").ToString) Then
                Mail.Bcc.Add(System.Configuration.ConfigurationManager.AppSettings("WebMasterBCC").ToString)
            End If


            'Sets the subject line including the type of error, and the description of the error.
            Mail.Subject = "Sales Surveys Error: " & Exc.GetType.ToString

            'Message Body
            Mail.IsBodyHtml = False
            Mail.Body = ""
            Mail.Body &= Now().ToShortDateString & " " & Now().ToShortTimeString & vbCrLf & vbCrLf
            If PrgMsg.Length > 0 Then
                Mail.Body &= PrgMsg & vbCrLf & vbCrLf
            End If
            Mail.Body &= " " & Exc.ToString
            Mail.IsBodyHtml = False
            Mail.Body = "A run-time error occurred while displaying this page: " & GetCurrentUrl() & vbCrLf & vbCrLf
            Mail.Body &= "User: " & Current.Session("FirstName") & " " & Current.Session("LastName") & " (OnyxID-" & Current.Session("OnyxID") & ")" & vbCrLf & vbCrLf
            Mail.Body &= "Message: " & PrgMsg & vbCrLf & vbCrLf
            Mail.Body &= Exc.ToString & vbCrLf & vbCrLf


            Try
                '#If Not Debug Then
                SmtpClient.Send(Mail)
                '#End If
            Catch ex As Exception
                'Fail Silently
            End Try


        End Sub

        Public Function GetCurrentUrl() As String
            'returns fully qualified URL with query string for the current page -- KMD, 9/20/04

            Dim strUrl As String
            Dim strHttp As String = "http://"
            Dim strHost, strPath, strQueryString As String

            If UCase(HttpContext.Current.Request.ServerVariables.Item("HTTPS")) = "ON" Then strHttp = "https://"

            strHost = HttpContext.Current.Request.ServerVariables.Item("HTTP_HOST")

            strPath = HttpContext.Current.Request.ServerVariables.Item("PATH_INFO")

            strQueryString = HttpContext.Current.Request.ServerVariables.Item("QUERY_STRING")
            If strQueryString <> "" Then strQueryString = "?" & strQueryString

            strUrl = strHttp & strHost & strPath & strQueryString

            Return strUrl
        End Function

        Public Function GetDBConnection()
            Return ConfigurationManager.ConnectionStrings("SalesSurveysConnectionString")
        End Function
     

        'connection string for ONYX
        Public Function GetCRMDBConnection()
            Return ConfigurationManager.ConnectionStrings("CRMConnectionString")
        End Function


        Public Function CRMOrgByID(ByVal OrgCRMID As String) As DataTable
            Dim iCompanyID As Integer
            Dim dt As New DataTable
            If Integer.TryParse(OrgCRMID, iCompanyID) Then
                Dim sql As String = String.Empty
                sql = "SELECT Top 50 c.iCompanyID As ID, c.vchCompanyName As Name, c.vchAddress1 As Address, c.vchCity As City, c.chRegionCode As Region, c.chCountryCode As Country, "
                sql &= " r.vchParameterDesc As Type FROM Company c INNER JOIN ReferenceParameters r on c.iCompanyTypeCode = r.iParameterID  WHERE c.iCompanyID = '"
                sql &= iCompanyID.ToString() & "' "
                sql &= " and c.tirecordstatus = 1 and c.iStatusID=328 ORDER BY c.vchCompanyName"

                Using conn As SqlConnection = New SqlConnection(GetCRMDBConnection.ToString)
                    Dim cmd As SqlCommand = New SqlCommand(sql, conn)
                    cmd.CommandType = CommandType.Text

                    Dim da As New SqlDataAdapter
                    da.SelectCommand = cmd

                    da.Fill(dt)

                End Using
            End If
            Return dt
        End Function

        'Populates the RadComboBox with lists of organizations from the current CRM based on characters entered
        'We are currently filtering out organization in countries other than US and Canada, and performing further 
        'filtering using company type code
        Public Function CRMOrgs(ByVal sqlString As String) As DataTable
            Dim sql As String = String.Empty
            sql = "SELECT Top 50 c.iCompanyID As ID, c.vchCompanyName As Name, c.vchAddress1 As Address, c.vchCity As City, c.chRegionCode As Region, c.chCountryCode As Country, "
            sql &= " r.vchParameterDesc As Type FROM Company c INNER JOIN ReferenceParameters r on c.iCompanyTypeCode = r.iParameterID  WHERE c.vchCompanyName LIKE '" & sqlString & "%' "
            sql &= "and c.tirecordstatus = 1 and c.iCompanyTypecode Not In (" & ConfigurationManager.AppSettings("CompanyCodes") & ")  and c.iStatusID=328 and "
            sql &= " (RTRIM(c.chCountryCode)='US' or RTRIM(c.chCountryCode)='CA') ORDER BY c.vchCompanyName"

            Using conn As SqlConnection = New SqlConnection(GetCRMDBConnection.ToString)
                Dim cmd As SqlCommand = New SqlCommand(sql, conn)
                cmd.CommandType = CommandType.Text

                Dim da As New SqlDataAdapter
                da.SelectCommand = cmd
                Dim dt As New DataTable

                da.Fill(dt)
                Return dt

            End Using
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
                       "window.alert('" + msg + "')</script>"


        End Sub

    End Class

End Namespace