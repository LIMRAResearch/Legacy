Partial Public Class SalesSurveyDBDataContext
    Inherits System.Data.Linq.DataContext

    Protected gl As Globals.GlobalFunctions = New Globals.GlobalFunctions

    Private Sub OnCreated()

        Me.Connection.ConnectionString = gl.GetDBConnection().ToString
        Dim tmSeconds As Integer
        If Integer.TryParse(ConfigurationManager.AppSettings("sqlTimeout"), tmSeconds) Then
            Me.CommandTimeout = tmSeconds
        Else
            Me.CommandTimeout = 900
        End If

        'Me.Connection.ConnectionString = ConfigurationManager.ConnectionStrings("SalesSurveysConnectionString").ToString       

    End Sub

End Class

