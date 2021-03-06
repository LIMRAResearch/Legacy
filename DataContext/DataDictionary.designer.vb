﻿'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:4.0.30319.18052
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Data.Linq
Imports System.Data.Linq.Mapping
Imports System.Linq
Imports System.Linq.Expressions
Imports System.Reflection


<Global.System.Data.Linq.Mapping.DatabaseAttribute(Name:="SalesSurveys")>  _
Partial Public Class DataDictionaryDataContext
	Inherits System.Data.Linq.DataContext
	
	Private Shared mappingSource As System.Data.Linq.Mapping.MappingSource = New AttributeMappingSource()
	
  #Region "Extensibility Method Definitions"
  Partial Private Sub OnCreated()
  End Sub
  #End Region
	
	Public Sub New()
        MyBase.New(Global.System.Configuration.ConfigurationManager.ConnectionStrings("SalesSurveysConnectionString").ConnectionString, mappingSource)
		OnCreated
	End Sub
	
	Public Sub New(ByVal connection As String)
		MyBase.New(connection, mappingSource)
		OnCreated
	End Sub
	
	Public Sub New(ByVal connection As System.Data.IDbConnection)
		MyBase.New(connection, mappingSource)
		OnCreated
	End Sub
	
	Public Sub New(ByVal connection As String, ByVal mappingSource As System.Data.Linq.Mapping.MappingSource)
		MyBase.New(connection, mappingSource)
		OnCreated
	End Sub
	
	Public Sub New(ByVal connection As System.Data.IDbConnection, ByVal mappingSource As System.Data.Linq.Mapping.MappingSource)
		MyBase.New(connection, mappingSource)
		OnCreated
	End Sub
	
	Public ReadOnly Property DataDictionaries() As System.Data.Linq.Table(Of DataDictionary)
		Get
			Return Me.GetTable(Of DataDictionary)
		End Get
	End Property
End Class

<Global.System.Data.Linq.Mapping.TableAttribute(Name:="dbo.DataDictionary")>  _
Partial Public Class DataDictionary
	
	Private _SurveyID As System.Nullable(Of Integer)
	
	Private _FieldID As Integer
	
	Private _Field As String
	
	Private _SortKey As Integer
	
	Private _xlSheet As String
	
	Private _xlRow As System.Nullable(Of Integer)
	
	Private _xlColumn As System.Nullable(Of Integer)
	
	Private _Source As System.Nullable(Of Char)
	
	Private _Description As String
	
	Private _DataType As String
	
	Private _Metric As String
	
	Private _Scale As String
	
	Private _Unit As String
	
	Private _Market As String
	
	Private _Channel As String
	
	Private _Product As String
	
	Public Sub New()
		MyBase.New
	End Sub
	
	<Global.System.Data.Linq.Mapping.ColumnAttribute(Storage:="_SurveyID", DbType:="Int")>  _
	Public Property SurveyID() As System.Nullable(Of Integer)
		Get
			Return Me._SurveyID
		End Get
		Set
			If (Me._SurveyID.Equals(value) = false) Then
				Me._SurveyID = value
			End If
		End Set
	End Property
	
	<Global.System.Data.Linq.Mapping.ColumnAttribute(Storage:="_FieldID", DbType:="Int NOT NULL")>  _
	Public Property FieldID() As Integer
		Get
			Return Me._FieldID
		End Get
		Set
			If ((Me._FieldID = value)  _
						= false) Then
				Me._FieldID = value
			End If
		End Set
	End Property
	
	<Global.System.Data.Linq.Mapping.ColumnAttribute(Storage:="_Field", DbType:="NVarChar(15) NOT NULL", CanBeNull:=false)>  _
	Public Property Field() As String
		Get
			Return Me._Field
		End Get
		Set
			If (String.Equals(Me._Field, value) = false) Then
				Me._Field = value
			End If
		End Set
	End Property
	
	<Global.System.Data.Linq.Mapping.ColumnAttribute(Storage:="_SortKey", DbType:="Int NOT NULL")>  _
	Public Property SortKey() As Integer
		Get
			Return Me._SortKey
		End Get
		Set
			If ((Me._SortKey = value)  _
						= false) Then
				Me._SortKey = value
			End If
		End Set
	End Property
	
	<Global.System.Data.Linq.Mapping.ColumnAttribute(Storage:="_xlSheet", DbType:="NVarChar(50)")>  _
	Public Property xlSheet() As String
		Get
			Return Me._xlSheet
		End Get
		Set
			If (String.Equals(Me._xlSheet, value) = false) Then
				Me._xlSheet = value
			End If
		End Set
	End Property
	
	<Global.System.Data.Linq.Mapping.ColumnAttribute(Storage:="_xlRow", DbType:="Int")>  _
	Public Property xlRow() As System.Nullable(Of Integer)
		Get
			Return Me._xlRow
		End Get
		Set
			If (Me._xlRow.Equals(value) = false) Then
				Me._xlRow = value
			End If
		End Set
	End Property
	
	<Global.System.Data.Linq.Mapping.ColumnAttribute(Storage:="_xlColumn", DbType:="Int")>  _
	Public Property xlColumn() As System.Nullable(Of Integer)
		Get
			Return Me._xlColumn
		End Get
		Set
			If (Me._xlColumn.Equals(value) = false) Then
				Me._xlColumn = value
			End If
		End Set
	End Property
	
	<Global.System.Data.Linq.Mapping.ColumnAttribute(Storage:="_Source", DbType:="NVarChar(1)")>  _
	Public Property Source() As System.Nullable(Of Char)
		Get
			Return Me._Source
		End Get
		Set
			If (Me._Source.Equals(value) = false) Then
				Me._Source = value
			End If
		End Set
	End Property
	
	<Global.System.Data.Linq.Mapping.ColumnAttribute(Storage:="_Description", DbType:="NVarChar(50)")>  _
	Public Property Description() As String
		Get
			Return Me._Description
		End Get
		Set
			If (String.Equals(Me._Description, value) = false) Then
				Me._Description = value
			End If
		End Set
	End Property
	
	<Global.System.Data.Linq.Mapping.ColumnAttribute(Storage:="_DataType", DbType:="NVarChar(50) NOT NULL", CanBeNull:=false)>  _
	Public Property DataType() As String
		Get
			Return Me._DataType
		End Get
		Set
			If (String.Equals(Me._DataType, value) = false) Then
				Me._DataType = value
			End If
		End Set
	End Property
	
	<Global.System.Data.Linq.Mapping.ColumnAttribute(Storage:="_Metric", DbType:="NVarChar(50) NOT NULL", CanBeNull:=false)>  _
	Public Property Metric() As String
		Get
			Return Me._Metric
		End Get
		Set
			If (String.Equals(Me._Metric, value) = false) Then
				Me._Metric = value
			End If
		End Set
	End Property
	
	<Global.System.Data.Linq.Mapping.ColumnAttribute(Storage:="_Scale", DbType:="NVarChar(50)")>  _
	Public Property Scale() As String
		Get
			Return Me._Scale
		End Get
		Set
			If (String.Equals(Me._Scale, value) = false) Then
				Me._Scale = value
			End If
		End Set
	End Property
	
	<Global.System.Data.Linq.Mapping.ColumnAttribute(Storage:="_Unit", DbType:="NVarChar(50)")>  _
	Public Property Unit() As String
		Get
			Return Me._Unit
		End Get
		Set
			If (String.Equals(Me._Unit, value) = false) Then
				Me._Unit = value
			End If
		End Set
	End Property
	
	<Global.System.Data.Linq.Mapping.ColumnAttribute(Storage:="_Market", DbType:="NVarChar(50)")>  _
	Public Property Market() As String
		Get
			Return Me._Market
		End Get
		Set
			If (String.Equals(Me._Market, value) = false) Then
				Me._Market = value
			End If
		End Set
	End Property
	
	<Global.System.Data.Linq.Mapping.ColumnAttribute(Storage:="_Channel", DbType:="NVarChar(50)")>  _
	Public Property Channel() As String
		Get
			Return Me._Channel
		End Get
		Set
			If (String.Equals(Me._Channel, value) = false) Then
				Me._Channel = value
			End If
		End Set
	End Property
	
	<Global.System.Data.Linq.Mapping.ColumnAttribute(Storage:="_Product", DbType:="NVarChar(50)")>  _
	Public Property Product() As String
		Get
			Return Me._Product
		End Get
		Set
			If (String.Equals(Me._Product, value) = false) Then
				Me._Product = value
			End If
		End Set
	End Property
End Class
