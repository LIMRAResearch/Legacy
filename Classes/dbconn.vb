Imports System.Data
Imports System.Data.SqlClient


Public Class dbconn


    '###########################################################################
    '#  Hides the connection string in a class, returns an open sql connection
    '#
    '#  Example:
    '#          Dim conn As SqlConnection = New SqlConnection
    '#          Dim lconn As dbconn = New dbconn
    '#          conn = lconn.GetUserConnection
    '#
    '###########################################################################


    Private dbConnection As String

    Protected gl As Globals.GlobalFunctions = New Globals.GlobalFunctions

    'db connection for the average application user - user will only have permissions necessary to run the application
    'replace the prod connection strings with the connection for the production server
    Public Function GetUserConnection() As SqlConnection
       
        dbConnection = ConfigurationManager.ConnectionStrings("SalesSurveysConnectionString").ToString
                
        Dim oConnection As New SqlConnection(dbConnection)

        'MB (3/25/2011) - remove Try/Catch block and use global error handlng
        'Try

        oConnection.Open()

        'Catch ex As Exception
        'gl.NotifyWebmaster(ex, gl.GetCurrentUrl, "Sales Survey DBConn GetUserConnectionError")
        'End Try

        Return oConnection

    End Function


    'db connection for admin - has additional permissions for activities outside the normal ones necessary to run the application
    'replace the prod connection strings with the connection for the production server
    Public Function GetAdminConnection() As SqlConnection
        
        dbConnection = ConfigurationManager.ConnectionStrings("SalesSurveysConnectionString").ToString
                
        Dim oConnection As New SqlConnection(dbConnection)

        'MB (3/25/2011) - remove Try/Catch block and use global error handlng
        'Try

        oConnection.Open()

        'Catch ex As Exception
        'gl.NotifyWebmaster(ex, gl.GetCurrentUrl, "Sales Survey DBConn GetAdminConnection Error")
        'End Try

        Return oConnection

    End Function

End Class
