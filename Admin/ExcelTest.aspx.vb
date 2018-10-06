Imports SpreadsheetGear


Partial Public Class ExcelTest
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            Dim ssFile As String = "\\San01\Projects\Sales Survey Analysis System\2-ColiBoli\Data\8-Templates\CurrentDataDictionary.xlsx"
            Dim workbook As SpreadsheetGear.IWorkbook = SpreadsheetGear.Factory.GetWorkbook(ssFile)
            Dim wrksheet As IWorksheet

            wrksheet = workbook.Worksheets("ddAttributes")

            '  Dim workbook2 As IWorkbook

            Dim cells As IRange = wrksheet.UsedRange

            ' Get a DataSet from an existing defined name
            ' Dim dataSet As DataSet = workbook.GetDataSet(cells.Address, SpreadsheetGear.Data.GetDataFlags.FormattedText)
            'Dim ds As DataSet = workbook.GetDataSet(Data.GetDataFlags.FormattedText)
            Dim ds As DataSet = workbook.GetDataSet(cells.Address, Data.GetDataFlags.FormattedText)

            Dim dv As New DataView(ds.Tables(0))
            dv.RowFilter = "FieldID is not null and FieldID<>''"

            GridView1.DataSource = dv
            GridView1.DataBind()



        Catch ex As Exception

        End Try
       
    End Sub

End Class