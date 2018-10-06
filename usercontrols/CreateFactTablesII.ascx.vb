Imports Telerik.Web.UI
Imports Telerik.Web.UI.Upload
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO

Public Class CreateFactTablesII
    Inherits System.Web.UI.UserControl

    Protected SurveyID As Integer
    Protected gF As Globals.GlobalFunctions = New Globals.GlobalFunctions
    Protected progress As RadProgressContext = RadProgressContext.Current

    ''' <summary>
    ''' when inserting the user control into the fact table page, set the Sproc property to the applicable stored procedure
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Sproc() As String
        Get
            Return _sproc
        End Get
        Set(value As String)
            _sproc = value
        End Set
    End Property

    Public Property FactTableName() As String
        Get
            Return _factTableName
        End Get
        Set(value As String)
            _factTableName = value
        End Set

    End Property

    Private _sproc As String
    Private _factTableName As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Page.User.IsInRole("SuperAdmin") OrElse Page.User.IsInRole("Admin") OrElse _
                Page.User.IsInRole("SuperAnalyst") OrElse Page.User.IsInRole("Analyst") Then

            SurveyID = CType(Session("SurveyID").ToString(), Integer)

            If Not Page.IsPostBack Then

                MultiView1.SetActiveView(vwDefault)
                litSurveyName.Text = " - " & Session("SurveyName")

            End If
        Else
            MultiView1.SetActiveView(vwNotAuthorized)
        End If
    End Sub



    ''' <summary>
    ''' executes the stored procedure indicated as a property on the user control
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub btnCreateFaceTable_Click(sender As Object, e As EventArgs) Handles btnCreateFaceTable.Click
        If Page.IsValid Then

            progress.PrimaryTotal = 100
            progress.PrimaryValue = 0
            progress.PrimaryPercent = 0

            Dim conn As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("SalesSurveysConnectionString").ToString())
            Dim cmd As SqlCommand = New SqlCommand(_sproc, conn)
            cmd.CommandType = CommandType.StoredProcedure
            cmd.CommandTimeout = 480

            Dim parameterSurveyID As SqlParameter = New SqlParameter("@SurveyID", SqlDbType.Int)
            parameterSurveyID.Value = SurveyID
            cmd.Parameters.Add(parameterSurveyID)

            Dim parameterBeginDate As SqlParameter = New SqlParameter("@BeginDate", SqlDbType.Date)
            parameterBeginDate.Value = txtBeginDate.SelectedDate
            cmd.Parameters.Add(parameterBeginDate)

            Dim parameterEndDate As SqlParameter = New SqlParameter("@EndDate", SqlDbType.Date)
            parameterEndDate.Value = txtEndDate.SelectedDate
            cmd.Parameters.Add(parameterEndDate)



            Try
                conn.Open()
                cmd.ExecuteNonQuery()

                MultiView1.SetActiveView(vwConfirm)
            Catch ex As Exception
                gF.NotifyWebmaster("Create Fact Table Error - Survey: " & SurveyID.ToString(), "Create FactTablesII.ascx", "Sproc: " & _sproc & "  \n\n" & ex.ToString())
                MultiView1.SetActiveView(vwError)
            Finally
                If conn.State = ConnectionState.Open Then
                    conn.Close()
                End If
            End Try
        End If

    End Sub

    Protected Sub btnMainScreen_Click(sender As Object, e As EventArgs) Handles btnMainScreen.Click
        MultiView1.SetActiveView(vwDefault)
        txtBeginDate.Clear()
        txtEndDate.Clear()

    End Sub

    Protected Sub btnExport_Click(sender As Object, e As EventArgs) Handles btnExport.Click
        Dim sql As String = "Select SourceID, WorkbookName, DateID, MOR, MaxMORSheet, MaxMORSource, MaxMORWB, xlSheet, Attribute1, Attribute2, Measure1, Measure2, Measure3, Measure4, Measure5,"
        sql &= "Measure6, Measure7, Value1, Value2, Value3, Value4,Value5, Value6,Value7 from ft001n0"

        Dim conn As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("SalesSurveysConnectionString").ToString)
        Dim cmd As SqlCommand = New SqlCommand(sql, conn)
        cmd.CommandType = CommandType.Text


        Dim da As SqlDataAdapter = New SqlDataAdapter()
        da.SelectCommand = cmd
        Dim ds As DataSet = New DataSet()

        Dim header As String = """SourceID"",""WorkbookName"",""DateID"",""MOR"",""MaxMORSheet"",""MaxMORSource"",""MaxMORWB"",""xlSheet"",""Attribute1"",""Attribute2"",""Measure1"",""Measure2"",""Measure3"","
        header &= """Measure4"",""Measure5"",""Measure6"",""Measure7"",""Value1"",""Value2"",""Value3"",""Value4"",""Value5"",""Value6"",""Value7""" & vbNewLine


        Dim s As New StringBuilder()

        Try
            da.Fill(ds, "NetFlows")
            If ds.Tables("NetFlows").Rows.Count() > 0 Then
                s.Append(header)
                For Each dr As DataRow In ds.Tables("NetFlows").Rows
                    For Each field As Object In dr.ItemArray
                        s.Append("""" & field.ToString & """" & ",")
                    Next
                    s.Replace(",", vbNewLine, s.Length - 1, 1)
                Next

                Using sw As New StreamWriter(Server.MapPath("/downloads/" & _factTableName + ".txt"))
                    sw.WriteLine(s.ToString())
                    sw.Close()
                End Using

                Dim fs As FileStream = Nothing
                fs = File.Open(Server.MapPath("/downloads/" & _factTableName + ".txt"), FileMode.Open)
                Dim btFile(fs.Length) As Byte
                fs.Read(btFile, 0, fs.Length)
                fs.Close()

                With Response
                    .AddHeader("Content-disposition", "attachment;filename=" & _factTableName & ".csv")
                    .ContentType = "application/octet-stream"
                    .BinaryWrite(btFile)
                    .End()
                End With

            End If
        Catch ex As Exception
            gF.NotifyWebmaster("Sales Survey Fact Table Export Error", "netflowsextractquarterly.aspx", ex.ToString())
            MultiView1.SetActiveView(vwError)
        Finally
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
    End Sub
End Class