Imports System.Net.Mail
Imports System.Data
Imports System.Data.SqlClient

Partial Public Class ForgotPassword
    Inherits System.Web.UI.Page

    '#########################################################################################
    '#
    '#   This module uses email to retrieve login credentials for a maintenance user
    '#
    '#   Depending on which button is selected, either the user name or password is sent
    '#
    '#      Tom Wieleba 01/27/2010
    '#
    '#########################################################################################

    Protected gl As Globals.GlobalFunctions

    'Upon initial page load, sets the default properties for labes, etc.
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Not Page.IsPostBack Then
            litPassword.Visible = False
            litUsername.Visible = False
            txtEmail.Text = String.Empty
            litNotFound.Text = String.Empty
            MultiView1.SetActiveView(vwForm)
        End If

    End Sub


    'Uses LINQ to query the database for the username and password based on the email 
    '
    Private Sub CredentialReminder(ByVal ReminderType As String)
      
        Dim UserName As String = String.Empty
        Dim MyPassword As String = String.Empty

        Dim Subject As String = String.Empty
        Dim Body As String = String.Empty

        'MB (3/25/2011) - remove Try/Catch block and use global error handlng
        'Try

        Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext

        Dim query = From p In db.admUsers _
                    Where p.Email = txtEmail.Text _
                    Select p.LoginID, p.Password

        If query.Count > 0 Then
            Dim SS_Sec As Security = New Security
            For Each item In query
                UserName = item.LoginID.ToString
                MyPassword = SS_Sec.Decrypt(item.Password.ToString)
            Next

            Select Case ReminderType
                Case "username"
                    Subject = "Sales Survey maintenance Username Reminder"
                    Body = "Your username for the Sales Survey Maintenance site is : " & UserName
                    litUsername.Visible = True
                    litPassword.Visible = False

                Case "password"
                    Subject = "Sales Survey maintenance Password Reminder"
                    Body = "Your password for the Sales Survey Maintenance site is : " & MyPassword
                    litUsername.Visible = False
                    litPassword.Visible = True
            End Select

            Dim SmtpClient As SmtpClient = New SmtpClient()
            SmtpClient.Host = System.Configuration.ConfigurationManager.AppSettings("mailServer")
            Dim Mail As MailMessage = New MailMessage()

            'To/From:
            Mail.To.Add(txtEmail.Text)
            Mail.From = New System.Net.Mail.MailAddress(System.Configuration.ConfigurationManager.AppSettings("webmaster"))

            Mail.Subject = Subject

            'Message Body
            Mail.IsBodyHtml = False
            Mail.Body = Body
            SmtpClient.Send(Mail)

        Else
            litNotFound.Text = "No credentials associated with the email entered."
            litNotFound.Visible = True
            txtEmail.Text = String.Empty
        End If

        'Catch ex As Exception
        'gl.NotifyWebmaster(ex, gl.GetCurrentUrl, "Sales Survey Admin Login Credentials Error")
        'MultiView1.SetActiveView(vwError)
        'End Try

    End Sub


    Protected Sub btnUserName_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnUserName.Click

        If Page.IsValid Then
            CredentialReminder("username")
        End If

    End Sub

    Protected Sub btnPassword_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnPassword.Click

        If Page.IsValid Then
            CredentialReminder("password")
        End If

    End Sub

End Class