Public Class _default21
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        MultiView1.SetActiveView(vwSurvey3)
        SetPageTitle(Session("SurveyName"))
    End Sub
    Private Sub SetPageTitle(ByVal TitleText As String)
        If Me.Form.Parent IsNot Nothing Then
            Dim litPageTitle As Literal = Me.Form.Parent.FindControl("litPageTitle")

            If litPageTitle IsNot Nothing Then
                litPageTitle.Text = TitleText
            End If
        End If
    End Sub
End Class