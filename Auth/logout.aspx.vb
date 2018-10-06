Public Partial Class logout
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        System.Web.Security.FormsAuthentication.SignOut()
        Response.Redirect("login.aspx", False)
    End Sub

End Class