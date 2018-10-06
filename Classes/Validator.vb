Imports Microsoft.VisualBasic

Public Class Validator

    Public Shared Sub ValidateSum(ByRef bIsValid As Boolean, ByVal sSum As String, ParamArray aryValues As String())
        ValidateSum(bIsValid, sSum, 2, aryValues)
    End Sub
    Public Shared Sub ValidateSum(ByRef bIsValid As Boolean, ByVal sSum As String, iRange As Integer, ParamArray aryValues As String())

        '****************************************** bValid only used as a one way flag; can only be set to false *************************** 

        If Not ValidateSum(sSum, iRange, aryValues) Then
            bIsValid = False
        End If


    End Sub



    Public Shared Sub ValidateSum(ByRef bIsValid As Boolean, ByRef txtSum As TextBox, ParamArray aryValues As String())
        ValidateSum(bIsValid, txtSum, 2, aryValues)
    End Sub
    Public Shared Sub ValidateSum(ByRef bIsValid As Boolean, ByRef txtSum As TextBox, iRange As Integer, ParamArray aryValues As String())

        '****************************************** bValid only used as a one way flag; can only be set to false *************************** 

        Dim sSum As String = txtSum.Text.Trim

        If Not ValidateSum(sSum, iRange, aryValues) Then
            bIsValid = False
            txtSum.CssClass = "TextRight Yellow"
        Else
            txtSum.CssClass = "TextRight"
        End If


    End Sub


    Public Shared Function ValidateSum(ByRef txtSum As TextBox, ParamArray aryValues As String()) As Boolean
        ValidateSum(txtSum, 2, aryValues)
    End Function
    Public Shared Function ValidateSum(ByRef txtSum As TextBox, iRange As Integer, ParamArray aryValues As String()) As Boolean

        Dim sSum As String = txtSum.Text.Trim

        Dim bIsValid As Boolean = ValidateSum(sSum, iRange, aryValues)
        txtSum.CssClass = IIf(bIsValid, "TextRight", "TextRight Yellow")

        Return bIsValid

    End Function


    Public Shared Function ValidateSum(ByVal sSum As String, ParamArray aryValues As String()) As Boolean
        Return ValidateSum(sSum, 2, aryValues)

    End Function
    Public Shared Function ValidateSum(ByVal sSum As String, iRange As Integer, ParamArray aryValues As String()) As Boolean
        Dim dSum As Double
        Dim dCalculatedSum As Double
        Dim dValue As Double
        Dim qResult As IEnumerable(Of String)

        Dim iValueCnt As Integer = aryValues.Count




        'Check for A's in the list of values. "A" (Addendum) indicates that the field was added to data dictionary after the survey was imported. If all A's 
        'then ignore the check (return true). If there are A's and other value treat the A's as empty strings in subsequent check
        qResult = From value As String In aryValues
                      Where value = "A"
                      Select value

        If qResult.Count = iValueCnt Then
            Return True
        End If


        'Check for M's in the list of values, if all M's then submiited sum must be M or number; if any M's but not all then sum must be M
        qResult = From value As String In aryValues
                      Where value = "M"
                      Select value

        If qResult.Count = iValueCnt Then
            Return sSum = "M" OrElse Double.TryParse(sSum, dSum)
        ElseIf qResult.Count > 0 Then 'And qResult < iValueCnt
            Return sSum = "M"
        End If



        'Check for all empty strings, if so then submitted sum must be empty string
        qResult = From value As String In aryValues
                     Where String.IsNullOrEmpty(value)
                     Select value

        If qResult.Count = iValueCnt Then
            Return String.IsNullOrEmpty(sSum)
        End If


        'Check for mixture of numbers empty strings or "A", if so then sum must be within range
        qResult = From value As String In aryValues
                  Where Double.TryParse(value, dValue) OrElse String.IsNullOrEmpty(value) OrElse value.Trim = "A"
                  Select value

        If qResult.Count = iValueCnt AndAlso Double.TryParse(sSum, dSum) Then
            For Each value In qResult
                If Double.TryParse(value, dValue) Then
                    dCalculatedSum += dValue
                End If
            Next

            Return ValidateSumRange(dSum, dCalculatedSum, iRange)

        Else
            Return False
        End If



        'if falls through all if blocks then invalid
        Return False

    End Function


    Private Shared Function ValidateSumRange(dSum As Double, dCalculatedSum As Double, iRange As Integer) As Boolean

        'TODO RP may increase flexibility by defining RANGE through configuration
        'Const RANGE As Double = 2

        Dim dHigh As Double = dCalculatedSum + iRange
        Dim dLow As Double = dCalculatedSum - iRange

        Return dSum <= dHigh And dSum >= dLow

    End Function


    'Example of GLIS validation

    ''iii. Subtotal (row 3)
    '    Validator.ValidateSum(P1Row3Col1, bTotalsOK, P1Row1Col1.Text, P1Row2Col1.Text)
    '    Validator.ValidateSum(P1Row3Col2, bTotalsOK, P1Row1Col2.Text, P1Row2Col2.Text)
    '    Validator.ValidateSum(P1Row3Col3, bTotalsOK, P1Row1Col3.Text, P1Row2Col3.Text)
    '    Validator.ValidateSum(P1Row3Col4, bTotalsOK, P1Row1Col4.Text, P1Row2Col4.Text)

    ''d.Total Term (Row 6)
    '    Validator.ValidateSum(P1Row6Col1, bTotalsOK, P1Row3Col1.Text, P1Row4Col1.Text, P1Row5Col1.Text)
    '    Validator.ValidateSum(P1Row6Col2, bTotalsOK, P1Row3Col2.Text, P1Row4Col2.Text, P1Row5Col2.Text)
    '    Validator.ValidateSum(P1Row6Col3, bTotalsOK, P1Row3Col3.Text, P1Row4Col3.Text, P1Row5Col3.Text)
    '    Validator.ValidateSum(P1Row6Col4, bTotalsOK, P1Row3Col4.Text, P1Row4Col4.Text, P1Row5Col4.Text)

    ''v.Subtotal (Row 11)
    '    Validator.ValidateSum(P1Row11Col1, bTotalsOK, P1Row7Col1.Text, P1Row8Col1.Text, P1Row9Col1.Text, P1Row10Col1.Text)
    '    Validator.ValidateSum(P1Row11Col2, bTotalsOK, P1Row7Col2.Text, P1Row8Col2.Text, P1Row9Col2.Text, P1Row10Col2.Text)
    '    Validator.ValidateSum(P1Row11Col3, bTotalsOK, P1Row7Col3.Text, P1Row8Col3.Text, P1Row9Col3.Text, P1Row10Col3.Text)
    '    Validator.ValidateSum(P1Row11Col4, bTotalsOK, P1Row7Col4.Text, P1Row8Col4.Text, P1Row9Col4.Text, P1Row10Col4.Text)


    ''c.Total Permanent (Row 13)
    '    Validator.ValidateSum(P1Row13Col1, bTotalsOK, P1Row11Col1.Text, P1Row12Col1.Text)
    '    Validator.ValidateSum(P1Row13Col2, bTotalsOK, P1Row11Col2.Text, P1Row12Col2.Text)
    '    Validator.ValidateSum(P1Row13Col3, bTotalsOK, P1Row11Col3.Text, P1Row12Col3.Text)
    '    Validator.ValidateSum(P1Row13Col4, bTotalsOK, P1Row11Col4.Text, P1Row12Col4.Text)


    ''5.Total (Row 16)
    '    Validator.ValidateSum(P1Row16Col1, bTotalsOK, P1Row6Col1.Text, P1Row13Col1.Text, P1Row14Col1.Text, P1Row15Col1.Text)
    '    Validator.ValidateSum(P1Row16Col2, bTotalsOK, P1Row6Col2.Text, P1Row13Col2.Text, P1Row14Col2.Text, P1Row15Col2.Text)
    '    Validator.ValidateSum(P1Row16Col3, bTotalsOK, P1Row6Col3.Text, P1Row13Col3.Text, P1Row14Col3.Text, P1Row15Col3.Text)
    '    Validator.ValidateSum(P1Row16Col4, bTotalsOK, P1Row6Col4.Text, P1Row13Col4.Text, P1Row14Col4.Text, P1Row15Col4.Text)


    ''8.Total (Row 19)
    '    Validator.ValidateSum(P1Row19Col1, bTotalsOK, P1Row16Col1.Text, P1Row17Col1.Text, P1Row18Col1.Text)
    '    Validator.ValidateSum(P1Row19Col2, bTotalsOK, P1Row16Col2.Text, P1Row17Col2.Text, P1Row18Col2.Text)
    '    Validator.ValidateSum(P1Row19Col3, bTotalsOK, P1Row16Col3.Text, P1Row17Col3.Text, P1Row18Col3.Text)
    '    Validator.ValidateSum(P1Row19Col4, bTotalsOK, P1Row16Col4.Text, P1Row17Col4.Text, P1Row18Col4.Text)


    ''iii.Subtotal (Row 22)
    '    Validator.ValidateSum(P1Row22Col1, bTotalsOK, P1Row20Col1.Text, P1Row21Col1.Text)
    '    Validator.ValidateSum(P1Row22Col2, bTotalsOK, P1Row20Col2.Text, P1Row21Col2.Text)
    '    Validator.ValidateSum(P1Row22Col3, bTotalsOK, P1Row20Col3.Text, P1Row21Col3.Text)
    '    Validator.ValidateSum(P1Row22Col4, bTotalsOK, P1Row20Col4.Text, P1Row21Col4.Text)


    ''c.Total AD&D (Row 24)
    '    Validator.ValidateSum(P1Row33Col1, bTotalsOK, P1Row22Col1.Text, P1Row23Col1.Text)
    '    Validator.ValidateSum(P1Row33Col2, bTotalsOK, P1Row22Col2.Text, P1Row23Col2.Text)
    '    Validator.ValidateSum(P1Row33Col3, bTotalsOK, P1Row22Col3.Text, P1Row23Col3.Text)
    '    Validator.ValidateSum(P1Row33Col4, bTotalsOK, P1Row22Col4.Text, P1Row23Col4.Text)


End Class