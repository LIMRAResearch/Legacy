Imports System.Data
Imports System.Data.SqlClient

Partial Public Class login
    Inherits System.Web.UI.Page

    Protected gl As Globals.GlobalFunctions = New Globals.GlobalFunctions

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            'Ensure page is running in secure mode
            If (Not Request.Url.AbsoluteUri.ToLower.Contains("localhost") AndAlso Not Request.Url.AbsoluteUri.ToLower.Contains("dev")) AndAlso Not Request.IsSecureConnection Then
                Response.Redirect(Request.Url.AbsoluteUri.Replace("http:", "https:"))
            End If
            MultiView1.SetActiveView(vwLogin)
            lblInvalid.Visible = False
        End If
    End Sub



    Private Sub btnSubmit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubmit.Click
        Dim ReturnURL As String = "/workflow/default.aspx"
        If AuthenticateUser() > 0 Then
            Response.Redirect(ReturnURL, False)
        Else
            lblInvalid.Visible = True
            txtPassword.Text = ""
        End If

    End Sub

   
    'Return ONYXID upon successful login
    Private Function AuthenticateUser() As Integer
        Dim thisSec As Security = New Security
        Dim Onyx As Integer = 0
        Dim roles As String = String.Empty


        Dim db As SalesSurveyDBDataContext = New SalesSurveyDBDataContext()

        Dim query = From p In db.admUsers _
                    Where p.Password = thisSec.Encrypt(txtPassword.Text) And p.LoginID = txtUserName.Text _
        Select p.FirstName, p.LastName, p.OnyxID

        If query.Count > 0 Then
            For Each item In query
                Session("FirstName") = item.FirstName
                Session("LastName") = item.LastName
                Session("OnyxID") = item.OnyxID
                Onyx = item.OnyxID
            Next

            If Onyx > 0 Then
                Dim role = From ug In db.admUserGroups _
                           Join g In db.admGroups _
                           On ug.GroupID Equals g.GroupID _
                           Where ug.OnyxID = Onyx _
                           Select g.GroupName
                If role.Count > 0 Then
                    roles = String.Join("|", role.ToArray)
                End If
                CreateAuthTicket(roles)
            End If
        End If
        Return Onyx
    End Function


    Private Sub CreateAuthTicket(ByVal Roles As String)

        'Create FormsAuthenticationTicket (FAT)
        Dim authTicket As New FormsAuthenticationTicket(1, txtUserName.Text, System.DateTime.Now, _
          System.DateTime.Now.AddMinutes(ConfigurationManager.AppSettings("FATExpireMinutes")), False, Roles)

        'Encrypt FAT
        Dim encryptedTicket As String = FormsAuthentication.Encrypt(authTicket)

        'Create auth cookie and add encrypted FAT
        Dim authCookie As New HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
        Response.Cookies.Add(authCookie)

    End Sub


    Protected Sub lnkForgot_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkForgot.Click
        Response.Redirect("forgotpassword.aspx", False)
    End Sub
End Class