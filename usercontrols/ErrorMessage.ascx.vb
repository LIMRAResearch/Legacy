Public Partial Class ErrorMessage
    Inherits System.Web.UI.UserControl

    'Displays a link to email the webmaster"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        litEmail.Text = "<a href=""mailto:" & ConfigurationManager.AppSettings("WebMaster") & """>" & ConfigurationManager.AppSettings("WebMaster") & "</a>"
    End Sub

End Class