Partial Public Class DataDictionaryDataContext
    Inherits System.Data.Linq.DataContext

    Protected gl As Globals.GlobalFunctions = New Globals.GlobalFunctions

    Private Sub OnCreated()

        Me.Connection.ConnectionString = gl.GetDBConnection().ToString

        'Me.Connection.ConnectionString = ConfigurationManager.ConnectionStrings("SalesSurveysConnectionString").ToString
      

    End Sub

End Class