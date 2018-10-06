Imports System.Web.SessionState
Imports System.Security.Principal
Imports System.Web.Security
Imports System.Web.SessionState.HttpSessionState
Imports System.Configuration

Public Class Global_asax
    Inherits System.Web.HttpApplication

    Dim gf As Globals.GlobalFunctions = New Globals.GlobalFunctions

    Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when the application is started
    End Sub

    Sub Session_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when the session is started
    End Sub

    Sub Application_BeginRequest(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires at the beginning of each request
    End Sub

    Sub Application_AuthenticateRequest(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires upon attempting to authenticate the user
        Dim cookieName As String = FormsAuthentication.FormsCookieName
        Dim authCookie As HttpCookie = Context.Request.Cookies(cookieName)
        If (authCookie) Is Nothing Then
            'Person is not logged in...
            Return
        End If

        'Decrypt cookie info
        Dim authTicket As FormsAuthenticationTicket
        Try
            authTicket = FormsAuthentication.Decrypt(authCookie.Value)
        Catch ex As Exception
            Return
        End Try

        'Retrieve roles list into GenericPrincipal User object
        Dim Roles() As String = authTicket.UserData.Split("|")
        Dim id As FormsIdentity = New FormsIdentity(authTicket)
        Dim principal As GenericPrincipal = New GenericPrincipal(id, Roles)
        Context.User = principal


    End Sub

    Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when an error occurs

        Dim ex As New Exception

        'Get the last exception that has occurred     
        ex = Server.GetLastError().GetBaseException()

        'Notify WebMaster
        gf.NotifyWebmaster(ex, "", "An Error has occurred in the Sales Survey Application")


        'Redirect to Error page 
        'Server.Transfer(ConfigurationManager.AppSettings("ErrorPage"))

    End Sub

    Sub Session_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when the session ends
    End Sub

    Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when the application ends
    End Sub

End Class