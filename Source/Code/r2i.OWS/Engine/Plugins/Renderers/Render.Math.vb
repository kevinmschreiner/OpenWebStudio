'<LICENSE>
'   Open Web Studio - http://www.OpenWebStudio.com
'   Copyright (c) 2007-2008
'   by R2Integrated Inc. http://www.R2integrated.com
'      
'   Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
'   documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
'   the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
'   to permit persons to whom the Software is furnished to do so, subject to the following conditions:
'    
'   The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
'   the Software.
'   
'   This Software and associated documentation files are subject to the terms and conditions of the Open Web Studio 
'   End User License Agreement and version 2 of the GNU General Public License.
'    
'   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
'   TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
'   THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
'   CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
'   DEALINGS IN THE SOFTWARE.
'</LICENSE>
Imports r2i.OWS.Framework
Imports r2i.OWS.Framework.Utilities
Imports r2i.OWS.Framework.Plugins.Renderers
Imports System.Collections.Generic

Namespace r2i.OWS.Renderers
    Public Class RenderMath
        Inherits RenderBase

        Public Overrides ReadOnly Property RenderTag() As String
            Get
                Return "MATH"
            End Get
        End Property

        Public Overrides ReadOnly Property RenderType() As RenderTypes
            Get
                Return RenderTypes.Functional
            End Get
        End Property

        Public Overrides ReadOnly Property CanPreRender() As Boolean
            Get
                Return False
            End Get
        End Property

        Public Overrides Function Handle_Render(ByRef Caller as EngineBase, ByVal Index As Integer, ByRef Source As String, ByRef DS As System.Data.DataSet, ByRef DR As System.Data.DataRow, ByRef RuntimeMessages As System.Collections.Generic.SortedList(Of String, String), ByVal NullReturn As Boolean, ByVal NullOverride As Boolean, ByVal ProtectSession As Boolean, ByVal SessionDelimiter As String, ByVal useSessionQuotes As Boolean, ByVal useAggregations As Boolean, ByRef FilterText As String, ByRef FilterField As String, ByRef Debugger As r2i.OWS.Framework.Debugger) As Boolean
            Dim REPLACED As Boolean = False
            Dim parameters As String() = ParameterizeString(Source, ","c, """"c, "\"c)
            If Not parameters Is Nothing AndAlso parameters.Length > 1 Then
                If parameters.Length = 2 Then
                    Dim VALUE As String = parameters(1)
                    Dim b As Boolean = RenderMath(VALUE)
                    If b Then
                        Source = VALUE
                    End If
                    REPLACED = b
                End If
            End If
            Return REPLACED
        End Function

        Private Shared Function fromBitArray(ByVal Value As BitArray) As String
            Dim i As Integer
            Dim str As String = ""
            For i = 0 To Value.Length - 1
                If Value.Item(i) Then
                    str &= "1"
                Else
                    str &= "0"
                End If
            Next
            Return str
        End Function
        Private Shared Function toBitArray(ByVal Value As String) As BitArray
            If Value.Replace("0", "").Replace("1", "").Length = 0 Then
                Dim ba As New BitArray(Value.Length)

                'THIS IS A VALID BIT ARRAY
                Dim i As Integer
                For i = 0 To Value.Length - 1
                    If Value.Chars(i) = "0"c Then
                        ba(i) = False
                    Else
                        ba(i) = True
                    End If
                Next
                Return ba
            ElseIf IsNumeric(Value) Then
                'CONVERT TO BIT ARRAY
                If Value.IndexOf(".") > -1 Then
                    Dim ba As New BitArray(System.BitConverter.GetBytes(CDbl(Value)))
                    Return ba
                Else
                    Dim ba As New BitArray(System.BitConverter.GetBytes(CInt(Value)))
                    Return ba
                End If
            Else
                Dim b() As Byte = System.Text.UTF8Encoding.UTF8.GetBytes(Value)
                Dim ba As New BitArray(b)
                Return ba
            End If
        End Function
        Private Shared Sub lengthBitArray(ByRef lArray As BitArray, ByRef rArray As BitArray)
            If lArray.Length < rArray.Length Then
                lArray.Length = rArray.Length
            ElseIf lArray.Length > rArray.Length Then
                rArray.Length = lArray.Length
            End If
        End Sub

        Public Function RenderMath(ByRef Value As String) As Boolean
            Dim changed As Boolean = False
            changed = RenderMath(Value, mathstartvalues, mathendvalues, mathescapechar)
            If changed Then
                Return True
            Else
                Return False
            End If
        End Function
        Private Function RenderMath(ByRef Source As String, ByRef StartValues As Char(), ByRef EndValues As Char(), ByRef EscapeChar As Char) As Boolean
            Dim thisPosition As Positional = Match(Source, Nothing, StartValues, EndValues, EscapeChar)

            Dim b As Boolean = False
            While Not thisPosition Is Nothing
                If RenderMath(Source, thisPosition) Then
                    b = True
                    thisPosition = Nothing
                End If
                thisPosition = Match(Source, thisPosition, StartValues, EndValues, EscapeChar)
            End While
            If RenderMath(Source, Nothing) Then
                b = True
            End If
            Return b
        End Function
        Private Function RenderMath(ByRef Source As String, ByVal Position As Positional) As Boolean
            Dim b As Boolean = False
            Dim rValue As String = ""
            If Not Position Is Nothing Then
                If Position.Starting + 1 < Position.Ending Then rValue = Source.Substring(Position.Starting + 1, Position.Ending - 1 - Position.Starting)
            Else
                rValue = Source
            End If
            If rValue.Length > 0 Then
                b = RenderMath_Evaluate(rValue)
            Else
                b = True
            End If

            If b Then
                Dim cb As Boolean = False
                If Not Position Is Nothing Then
                    Dim Lead5 As String
                    Dim Lead4 As String
                    Dim Lead3 As String
                    Dim Lead2 As String
                    If Position.Starting >= 5 Then
                        Lead5 = Source.Substring(Position.Starting - 5, 5).ToUpper
                    Else
                        Lead5 = ""
                    End If
                    If Position.Starting >= 4 Then
                        Lead4 = Source.Substring(Position.Starting - 4, 4).ToUpper
                    Else
                        Lead4 = ""
                    End If
                    If Position.Starting >= 3 Then
                        Lead3 = Source.Substring(Position.Starting - 3, 3).ToUpper
                    Else
                        Lead3 = ""
                    End If
                    If Position.Starting >= 2 Then
                        Lead2 = Source.Substring(Position.Starting - 2, 2).ToUpper
                    Else
                        Lead2 = ""
                    End If
                    If Not Lead5.Length = 0 OrElse Not Lead4.Length = 0 OrElse Not Lead3.Length = 0 OrElse Not Lead2.Length = 0 Then
                        Select Case True
                            Case Lead5 = "FLOOR"
                                If IsNumeric(rValue) Then
                                    rValue = Math.Floor(CType(rValue, Double))
                                    Position.Starting -= 5
                                    cb = True
                                End If
                            Case Lead5 = "ROUND"
                                If IsNumeric(rValue) Then
                                    rValue = Math.Round(CType(rValue, Double))
                                    Position.Starting -= 5
                                    cb = True
                                End If
                            Case Lead4 = "SQRT"
                                If IsNumeric(rValue) Then
                                    rValue = Math.Sqrt(CType(rValue, Double))
                                    Position.Starting -= 4
                                    cb = True
                                End If
                            Case Lead4 = "ACOS"
                                If IsNumeric(rValue) Then
                                    rValue = Math.Acos(CType(rValue, Double))
                                    Position.Starting -= 4
                                    cb = True
                                End If
                            Case Lead4 = "SIGN"
                                If IsNumeric(rValue) Then
                                    rValue = Math.Sign(CType(rValue, Double))
                                    Position.Starting -= 4
                                    cb = True
                                End If
                            Case Lead4 = "SINH"
                                If IsNumeric(rValue) Then
                                    rValue = Math.Sinh(CType(rValue, Double))
                                    Position.Starting -= 4
                                    cb = True
                                End If
                            Case Lead4 = "TANH"
                                If IsNumeric(rValue) Then
                                    rValue = Math.Tanh(CType(rValue, Double))
                                    Position.Starting -= 4
                                    cb = True
                                End If
                            Case Lead4 = "COSH"
                                If IsNumeric(rValue) Then
                                    rValue = Math.Cosh(CType(rValue, Double))
                                    Position.Starting -= 4
                                    cb = True
                                End If
                            Case Lead4 = "CEIL"
                                If IsNumeric(rValue) Then
                                    rValue = Math.Ceiling(CType(rValue, Double))
                                    Position.Starting -= 4
                                    cb = True
                                End If
                            Case Lead4 = "ASIN"
                                If IsNumeric(rValue) Then
                                    rValue = Math.Asin(CType(rValue, Double))
                                    Position.Starting -= 4
                                    cb = True
                                End If
                            Case Lead4 = "ATAN"
                                If IsNumeric(rValue) Then
                                    rValue = Math.Atan(CType(rValue, Double))
                                    Position.Starting -= 4
                                    cb = True
                                End If
                            Case Lead3 = "ABS"
                                If IsNumeric(rValue) Then
                                    rValue = Math.Abs(CType(rValue, Double))
                                    Position.Starting -= 3
                                    cb = True
                                End If
                            Case Lead3 = "EXP"
                                If IsNumeric(rValue) Then
                                    rValue = Math.Exp(CType(rValue, Double))
                                    Position.Starting -= 3
                                    cb = True
                                End If
                            Case Lead3 = "LOG"
                                If IsNumeric(rValue) Then
                                    rValue = Math.Log10(CType(rValue, Double))
                                    Position.Starting -= 3
                                    cb = True
                                End If
                            Case Lead3 = "SIN"
                                If IsNumeric(rValue) Then
                                    rValue = Math.Sin(CType(rValue, Double))
                                    Position.Starting -= 3
                                    cb = True
                                End If
                            Case Lead3 = "TAN"
                                If IsNumeric(rValue) Then
                                    rValue = Math.Tan(CType(rValue, Double))
                                    Position.Starting -= 3
                                    cb = True
                                End If
                            Case Lead3 = "COS"
                                If IsNumeric(rValue) Then
                                    rValue = Math.Cos(CType(rValue, Double))
                                    Position.Starting -= 3
                                    cb = True
                                End If
                            Case Lead2 = "LN"
                                If IsNumeric(rValue) Then
                                    rValue = Math.Log(CType(rValue, Double))
                                    Position.Starting -= 2
                                    cb = True
                                End If
                            Case Lead2 = "PI"
                                rValue = Math.PI
                                Position.Starting -= 2
                                cb = True
                        End Select
                    End If
                    If Position.Starting > 0 Then
                        rValue = Source.Substring(0, Position.Starting) + rValue
                    End If
                    If Position.Ending + 1 < Source.Length Then
                        rValue &= Source.Substring(Position.Ending + 1)
                    End If
                    Source = rValue
                Else
                    Source = rValue
                End If
            End If
            Return b
        End Function
        Private Function RenderMath_Evaluate(ByRef Source As String) As Boolean
            Dim mathe As New MathEvaluator
            If mathe.Evaluate(Source) Then
                mathe = Nothing
                Return True
            Else
                mathe = Nothing
                Return False
            End If
        End Function

        Private Class MathEvaluator
            'ROMAIN: Generic replacement - 08/20/2007
            'Private _items As New ArrayList
            Private _items As New List(Of MathItem)
            Private Class ExpressionItem
                Implements IComparable
                Public Value As String
                Public Position As Integer
                Public Function CompareTo(ByVal obj As Object) As Integer Implements System.IComparable.CompareTo
                    Dim mi As ExpressionItem = CType(obj, ExpressionItem)
                    If mi.Position < Me.Position Then
                        Return 1
                    ElseIf mi.Position > Me.Position Then
                        Return -1
                    Else
                        Return 0
                    End If
                End Function
            End Class
            Private Class MathItem
                Implements IComparable
                Public Enum MathItemType
                    Symbol
                    Number
                End Enum
                Public Enum MathValueType
                    Numeric
                    [DateTime]
                    [Boolean]
                End Enum
                Public MathType As MathItemType
                Public objLeft As Object
                Public objRight As Object
                Public Value As String
                Public ValueType As MathValueType
                Public Position As Integer
                Public Function GetExpressionValue() As ExpressionItem
                    Dim eI As New ExpressionItem
                    eI.Position = Me.Position
                    eI.Value = Me.Value
                    Return eI
                End Function
                Public ReadOnly Property SymbolPosition() As Integer
                    Get
                        If MathType = MathItemType.Symbol Then
                            Select Case Value.ToUpper
                                Case "AND", "OR", "XOR", "AND:BIT", "OR:BIT", "XOR:BIT"
                                    Return 3
                                Case "<", ">", "=", "<=", "<>", ">=", "!="
                                    Return 2
                                Case "-", "+"
                                    Return 1
                                Case "*", "^", "%", "/", "\"
                                    Return 0
                            End Select
                        Else
                            Return 4
                        End If
                    End Get
                End Property

                Public Function CompareTo(ByVal obj As Object) As Integer Implements System.IComparable.CompareTo
                    If TypeOf obj Is MathItem Then
                        'ROMAIN:08/23/2007
                        'NOTE: Remove cast
                        'Dim mi As MathItem = CType(obj, MathItem)
                        Dim mi As MathItem = obj
                        If mi.SymbolPosition < Me.SymbolPosition Then
                            Return -1
                        ElseIf mi.SymbolPosition > Me.SymbolPosition Then
                            Return 1
                        Else
                            If mi.Position < Me.Position Then
                                Return -1
                            ElseIf mi.Position > Me.Position Then
                                Return 1
                            Else
                                Return 0
                            End If
                        End If
                    End If
                End Function
            End Class

            '        Public Info As String
            Private Function notSymbol(ByVal Value As String, ByVal index As Integer) As Boolean
                If index > 0 Then
                    Select Case Value.ToUpper.Trim
                        Case "+", "-", "/", "\", "<", ">", "<>", "!=", "=", "<=", ">=", "*", "%", "^", "AND", "OR", "XOR", "AND:BIT", "OR:BIT", "XOR:BIT"
                            Return False
                    End Select
                End If
                Return True
            End Function

            Private Function GetMathArray(ByVal StringArray As String()) As List(Of MathItem)
                Dim CurrentType As MathItem.MathItemType = Nothing
                Dim CurrentItem As String = Nothing
                Dim ReturnValue As New List(Of MathItem)
                CurrentType = MathItem.MathItemType.Number
                Dim cMathItem As MathItem = Nothing
                Dim lMathItem As MathItem = Nothing
                Dim i As Integer = 0
                For Each CurrentItem In StringArray
                    CurrentItem = CurrentItem.Trim
                    Select Case CurrentType
                        Case MathItem.MathItemType.Number
                            If cMathItem Is Nothing Then
                                cMathItem = New MathItem
                                If Not lMathItem Is Nothing AndAlso cMathItem.objLeft Is Nothing Then
                                    cMathItem.objLeft = lMathItem
                                    lMathItem.objRight = cMathItem
                                End If
                                cMathItem.MathType = MathItem.MathItemType.Number
                                cMathItem.Value = ""
                                cMathItem.Position = i
                            End If
                            cMathItem.Value &= CurrentItem
                            If IsNumeric(CurrentItem) OrElse CurrentItem.ToUpper = "TRUE" OrElse CurrentItem.ToUpper = "FALSE" OrElse (notSymbol(CurrentItem, i)) Then
                                'THE ELEMENT IS A NUMERIC VALUE - END THE LEFT
                                CurrentType = MathItem.MathItemType.Symbol
                                If CurrentItem.ToUpper = "TRUE" OrElse CurrentItem.ToUpper = "FALSE" Then
                                    cMathItem.ValueType = MathItem.MathValueType.Boolean
                                Else
                                    cMathItem.ValueType = MathItem.MathValueType.Numeric
                                End If

                                lMathItem = cMathItem
                                ReturnValue.Add(cMathItem)
                                cMathItem = Nothing
                                i += 1
                            End If
                        Case MathItem.MathItemType.Symbol
                            '07/21/2010 KMS: ADDED TO MERGE STRING VALUES TOGETHER WHEN THEY APPEAR AS QUOTED REGIONS ('-1'='whatever')
                            '<ticket:234>
                            Dim merged As Boolean = False
                            'If the previous item exists, its marked as a number and its not numeric
                            If Not lMathItem Is Nothing AndAlso lMathItem.ValueType = MathItem.MathValueType.Numeric AndAlso Not IsNumeric(lMathItem.Value) Then
                                If lMathItem.Value.Length > 0 AndAlso (lMathItem.Value(0) = "'"c OrElse lMathItem.Value(0) = """"c OrElse lMathItem.Value(0) = "#"c) Then
                                    'The previous item starts a quoted text value
                                    If lMathItem.Value.Length = 1 OrElse lMathItem.Value(0) <> lMathItem.Value(lMathItem.Value.Length - 1) Then
                                        'The previos item starts a text value, but it doesnt END in the previous item
                                        lMathItem.Value += CurrentItem
                                        merged = True
                                    End If
                                End If
                            End If
                            '</ticket:234>

                            If Not merged Then
                                'THERE CAN ONLY BE ONE SYMBOL VALUE
                                If cMathItem Is Nothing Then
                                    cMathItem = New MathItem
                                End If
                                If Not lMathItem Is Nothing AndAlso cMathItem.objLeft Is Nothing Then
                                    cMathItem.objLeft = lMathItem
                                    lMathItem.objRight = cMathItem
                                    cMathItem.MathType = MathItem.MathItemType.Symbol
                                    cMathItem.Value = ""
                                    cMathItem.Position = i
                                End If

                                cMathItem.Value &= CurrentItem
                                'THE ELEMENT IS A SYMBOL VALUE - END
                                lMathItem = cMathItem
                                ReturnValue.Add(cMathItem)
                                CurrentType = MathItem.MathItemType.Number
                                cMathItem = Nothing
                            End If
                            i += 1
                    End Select
                Next

                ReturnValue.Sort()
                ReturnValue.Reverse()

                Return ReturnValue
            End Function
            Public Function EvaluateValue(ByRef Value As String) As String
                If Value.ToUpper.StartsWith("NOT ") Then
                    Dim xvalue As String = Value.Substring(4)
                    If xvalue.Length > 0 Then
                        If xvalue.Trim.ToUpper = "TRUE" Then
                            Return (False).ToString
                        ElseIf xvalue.Trim.ToUpper = "FALSE" Then
                            Return (True).ToString
                        End If
                    End If
                ElseIf Value.ToUpper.StartsWith("NOT:BIT ") Then
                    Dim xvalue As String = Value.Substring(8)
                    If xvalue.Length > 0 Then
                        Dim rValue As BitArray = toBitArray(xvalue)
                        Return fromBitArray(rValue.Not)
                    End If
                End If
                Return Value
            End Function
            Private Class DateTimeSpan
                Public Enum DateTimeTypeEnum
                    Number
                    DateTime
                    [Date]
                    [Time]
                    [TimeSpan]
                    [RelativeTimeSpan]
                    [Boolean]
                End Enum
                Private _obj As Object
                Private _type As DateTimeTypeEnum
                Public ReadOnly Property DateType() As DateTimeTypeEnum
                    Get
                        Select Case _type
                            Case DateTimeTypeEnum.Date, DateTimeTypeEnum.DateTime
                                Return DateTimeTypeEnum.DateTime
                            Case DateTimeTypeEnum.RelativeTimeSpan
                                Return DateTimeTypeEnum.RelativeTimeSpan
                            Case Else
                                Return DateTimeTypeEnum.TimeSpan
                        End Select
                        Return _type
                    End Get
                End Property
                Public ReadOnly Property DateTime() As DateTime
                    Get
                        Select Case _type
                            Case DateTimeTypeEnum.Date
                                Return CType(_obj, DateTime)
                            Case DateTimeTypeEnum.DateTime
                                Return CType(_obj, DateTime)
                            Case Else
                                Return Nothing
                        End Select
                    End Get
                End Property
                Public ReadOnly Property TimeSpan() As TimeSpan
                    Get
                        Select Case _type
                            Case DateTimeTypeEnum.Time
                                Return CType(_obj, TimeSpan)
                            Case DateTimeTypeEnum.TimeSpan
                                Return CType(_obj, TimeSpan)
                            Case DateTimeTypeEnum.Number
                                Return New TimeSpan(CType(_obj, Double))
                            Case Else
                                Return Nothing
                        End Select
                    End Get
                End Property
                Public ReadOnly Property RTimeSpan() As RelativeTimespan
                    Get
                        Select Case _type
                            Case DateTimeTypeEnum.RelativeTimeSpan
                                Return CType(_obj, RelativeTimeSpan)
                            Case Else
                                Return Nothing
                        End Select
                    End Get
                End Property

                Public Sub New(ByVal value As String)
                    Dim forceTimeSpan As Boolean = False
                    If value.StartsWith("#") And value.EndsWith("#") Then
                        value = value.Remove(value.Length - 1, 1).Remove(0, 1)
                    End If
                    If IsNumeric(value) Then
                        _type = DateTimeTypeEnum.Number
                        _obj = CDbl(value)
                    ElseIf value.ToLower.EndsWith("m") Then
                        _type = DateTimeTypeEnum.DateTime
                        _obj = DateTime.Parse(value)
                    Else
                        If value.ToLower.EndsWith("ts") Then
                            forceTimeSpan = True
                            value = value.Remove(value.Length - 2, 2)
                        End If
                        Dim xsplit As String() = value.Replace("\", "/").Split(" ")
                        Dim dsplit As String() = Nothing
                        Dim tsplit As String() = Nothing
                        Dim ts As TimeSpan = Nothing
                        Dim tsNil As Boolean = True
                        Dim dtNil As Boolean = True
                        Dim dt As DateTime = Nothing
                        Dim m As Integer = 0
                        Dim d As Integer = 0
                        Dim y As Integer = 0
                        Dim days As Integer = 0
                        Dim h As Integer = 0
                        Dim mn As Integer = 0
                        Dim s As Integer = 0
                        Dim ms As Integer = 0
                        If xsplit.Length > 0 Then
                            Dim i As Integer
                            For i = 0 To xsplit.Length - 1
                                If dsplit Is Nothing AndAlso xsplit(i).Contains("/") Then
                                    dsplit = xsplit(i).Split("/")
                                ElseIf tsplit Is Nothing AndAlso xsplit(i).Contains(":") Then
                                    tsplit = xsplit(i).Split(":")
                                End If
                            Next
                            If dsplit Is Nothing AndAlso IsNumeric(xsplit(0).Trim) Then
                                days = CInt(xsplit(0).Trim)
                            ElseIf Not dsplit Is Nothing Then
                                For i = 0 To dsplit.Length - 1
                                    If i = 0 Then
                                        m = CInt(dsplit(i))
                                    ElseIf i = 1 Then
                                        d = CInt(dsplit(i))
                                    ElseIf i = 2 Then
                                        y = CInt(dsplit(i))
                                    End If
                                Next
                            End If
                            If Not tsplit Is Nothing Then
                                For i = 0 To tsplit.Length - 1
                                    If i = 0 Then
                                        'hours
                                        If IsNumeric(tsplit(i)) Then
                                            h = CInt(tsplit(i))
                                        End If
                                    ElseIf i = 1 Then
                                        'minutes
                                        If IsNumeric(tsplit(i)) Then
                                            mn = CInt(tsplit(i))
                                        End If
                                    ElseIf i = 2 Then
                                        'seconds
                                        If IsNumeric(tsplit(i)) Then
                                            Dim v As Integer = CInt(tsplit(i))
                                            Dim r As Integer = 0
                                            If tsplit(i).Contains(".") Then
                                                Dim f As Double = CDbl(tsplit(i))
                                                f = f - v
                                                r = f * 1000
                                            End If
                                            s = v
                                            ms = r
                                        End If
                                    End If
                                Next
                            End If

                            If Not forceTimeSpan And (y > 0 And m > 0 And d > 0 And m < 13 And d < 32 And h < 25 And mn < 60 And s < 60) Then
                                'This is a date!
                                _obj = New DateTime(y, m, d, h, mn, s, ms)
                                _type = DateTimeTypeEnum.DateTime
                            ElseIf Not forceTimeSpan And (y > 0 And m > 0 And m < 13 And d > 0 And d < 32 And h + mn + s + ms = 0) Then
                                _obj = New DateTime(y, m, d)
                                _type = DateTimeTypeEnum.Date
                            ElseIf Not forceTimeSpan And (y = 0 And m = 0 And d = 0 And h < 13 And m < 60 And s < 60) Then
                                _obj = New TimeSpan(0, h, m, s, ms)
                                _type = DateTimeTypeEnum.Time
                            Else
                                'This is NOT a date!
                                If y + m = 0 Then
                                    _obj = New TimeSpan(d + days, h, mn, s, ms)
                                    _type = DateTimeTypeEnum.TimeSpan
                                Else
                                    _obj = New RelativeTimeSpan(y, m, d + days, h, mn, s, ms)
                                    _type = DateTimeTypeEnum.RelativeTimeSpan
                                End If
                            End If

                        End If
                    End If
                End Sub
                Public Structure RelativeTimeSpan
                    Public Years As Integer
                    Public Months As Integer
                    Public Days As Integer
                    Public Hours As Integer
                    Public Minutes As Integer
                    Public Seconds As Integer
                    Public Milliseconds As Integer
                    Public Ticks As Integer
                    Public Sub New(ByVal y As Integer, ByVal m As Integer, ByVal d As Integer, ByVal h As Integer, ByVal min As Integer, ByVal s As Integer, ByVal ms As Integer)
                        Years = y
                        Months = m
                        Days = d
                        Hours = h
                        Minutes = min
                        Seconds = s
                        Milliseconds = ms
                    End Sub
                    Public Function toTimespan() As TimeSpan
                        If Years + Months = 0 Then
                            Return New TimeSpan(Days, Hours, Minutes, Seconds, milliseconds)
                        End If
                        Return Nothing
                    End Function
                End Structure
            End Class
            Public Function DateMath(ByRef Left As String, ByRef Right As String, ByRef Symbol As String, ByRef Result As String) As Boolean
                Dim outputType As DateTimeSpan.DateTimeTypeEnum
                Dim lrel As Boolean = False
                Dim rrel As Boolean = False
                Dim rts As DateTimeSpan.RelativeTimeSpan
                Dim lft As New DateTimeSpan(Left)
                Dim rgt As New DateTimeSpan(Right)
                Dim lnlft As Long = 0
                Dim lnrgt As Long = 0
                Dim lnResult As Long = 0
                Dim bReturn As Boolean = True

                'Get the correct Ticks
                If lft.DateType = DateTimeSpan.DateTimeTypeEnum.DateTime Then
                    lnlft = lft.DateTime.Ticks
                Else
                    If lft.DateType = DateTimeSpan.DateTimeTypeEnum.RelativeTimeSpan Then
                        lnlft = (New TimeSpan(lft.RTimeSpan.Years * 365 + lft.RTimeSpan.Months * 28 + lft.RTimeSpan.Days, lft.RTimeSpan.Hours, lft.RTimeSpan.Minutes, lft.RTimeSpan.Seconds, lft.RTimeSpan.Milliseconds)).Ticks
                        lrel = True
                    Else
                        lnlft = lft.TimeSpan.Ticks
                    End If
                End If
                If rgt.DateType = DateTimeSpan.DateTimeTypeEnum.DateTime Then
                    lnrgt = rgt.DateTime.Ticks
                Else
                    If rgt.DateType = DateTimeSpan.DateTimeTypeEnum.RelativeTimeSpan Then
                        lnrgt = (New TimeSpan(rgt.RTimeSpan.Years * 365 + rgt.RTimeSpan.Months * 28 + rgt.RTimeSpan.Days, rgt.RTimeSpan.Hours, rgt.RTimeSpan.Minutes, rgt.RTimeSpan.Seconds, rgt.RTimeSpan.Milliseconds)).Ticks
                        rrel = True
                    Else
                        lnrgt = rgt.TimeSpan.Ticks
                    End If
                End If

                'Determine the correct output type (timespan format or datetime format)
                If lft.DateType = rgt.DateType Then
                    outputType = DateTimeSpan.DateTimeTypeEnum.TimeSpan
                Else
                    If lft.DateType = DateTimeSpan.DateTimeTypeEnum.DateTime Then
                        outputType = DateTimeSpan.DateTimeTypeEnum.DateTime
                    Else
                        outputType = DateTimeSpan.DateTimeTypeEnum.TimeSpan
                    End If
                End If

                'perform the math into lnResult. If boolean, set outputtype to boolean and the lnResult to 1 for true, 0 for false
                'if the symbol is invalid, return boolean false and false for evaluation
                Select Case Symbol.ToUpper
                    Case "-"
                        If lrel Or rrel Then
                            If lrel And rrel Then
                                rts = New DateTimeSpan.RelativeTimeSpan(lft.RTimeSpan.Years - rgt.RTimeSpan.Years, lft.RTimeSpan.Months - rgt.RTimeSpan.Months, lft.RTimeSpan.Days - rgt.RTimeSpan.Days, lft.RTimeSpan.Hours - rgt.RTimeSpan.Hours, lft.RTimeSpan.Minutes - rgt.RTimeSpan.Minutes, lft.RTimeSpan.Seconds - rgt.RTimeSpan.Seconds, lft.RTimeSpan.Milliseconds - rgt.RTimeSpan.Milliseconds)
                                outputType = DateTimeSpan.DateTimeTypeEnum.RelativeTimeSpan
                            ElseIf lrel Then
                                If rgt.DateType = DateTimeSpan.DateTimeTypeEnum.DateTime Then
                                    Dim dt As New DateTime(lnrgt)
                                    dt = dt.AddYears(lft.RTimeSpan.Years * -1)
                                    dt = dt.AddMonths(lft.RTimeSpan.Months * -1)
                                    dt = dt.AddDays(lft.RTimeSpan.Days * -1)
                                    dt = dt.AddHours(lft.RTimeSpan.Hours * -1)
                                    dt = dt.AddMinutes(lft.RTimeSpan.Minutes * -1)
                                    dt = dt.AddSeconds(lft.RTimeSpan.Seconds * -1)
                                    dt = dt.AddMilliseconds(lft.RTimeSpan.Milliseconds * -1)
                                    lnResult = dt.Ticks
                                    outputType = DateTimeSpan.DateTimeTypeEnum.DateTime
                                ElseIf rgt.DateType = DateTimeSpan.DateTimeTypeEnum.TimeSpan Then
                                    rts = New DateTimeSpan.RelativeTimeSpan(lft.RTimeSpan.Years, lft.RTimeSpan.Months, lft.RTimeSpan.Days - rgt.TimeSpan.Days, lft.RTimeSpan.Hours - rgt.TimeSpan.Hours, lft.RTimeSpan.Minutes - rgt.TimeSpan.Minutes, lft.RTimeSpan.Seconds - rgt.TimeSpan.Seconds, lft.RTimeSpan.Milliseconds - rgt.TimeSpan.Milliseconds)
                                    outputType = DateTimeSpan.DateTimeTypeEnum.RelativeTimeSpan
                                Else
                                    Dim ts As New TimeSpan(lnrgt)
                                    rts = New DateTimeSpan.RelativeTimeSpan(lft.RTimeSpan.Years, lft.RTimeSpan.Months, lft.RTimeSpan.Days - ts.Days, lft.RTimeSpan.Hours - ts.Hours, lft.RTimeSpan.Minutes - ts.Minutes, lft.RTimeSpan.Seconds - ts.Seconds, lft.RTimeSpan.Milliseconds - ts.Milliseconds)
                                    outputType = DateTimeSpan.DateTimeTypeEnum.RelativeTimeSpan
                                End If
                            Else
                                If lft.DateType = DateTimeSpan.DateTimeTypeEnum.DateTime Then
                                    Dim dt As New DateTime(lnlft)
                                    dt = dt.AddYears(rgt.RTimeSpan.Years * -1)
                                    dt = dt.AddMonths(rgt.RTimeSpan.Months * -1)
                                    dt = dt.AddDays(rgt.RTimeSpan.Days * -1)
                                    dt = dt.AddHours(rgt.RTimeSpan.Hours * -1)
                                    dt = dt.AddMinutes(rgt.RTimeSpan.Minutes * -1)
                                    dt = dt.AddSeconds(rgt.RTimeSpan.Seconds * -1)
                                    dt = dt.AddMilliseconds(rgt.RTimeSpan.Milliseconds * -1)
                                    lnResult = dt.Ticks
                                    outputType = DateTimeSpan.DateTimeTypeEnum.DateTime
                                ElseIf lft.DateType = DateTimeSpan.DateTimeTypeEnum.TimeSpan Then
                                    rts = New DateTimeSpan.RelativeTimeSpan(rgt.RTimeSpan.Years, rgt.RTimeSpan.Months, lft.TimeSpan.Days - rgt.RTimeSpan.Days, lft.TimeSpan.Hours - rgt.RTimeSpan.Hours, lft.TimeSpan.Minutes - rgt.RTimeSpan.Minutes, lft.TimeSpan.Seconds - rgt.RTimeSpan.Seconds, lft.TimeSpan.Milliseconds - rgt.RTimeSpan.Milliseconds)
                                    outputType = DateTimeSpan.DateTimeTypeEnum.RelativeTimeSpan
                                Else
                                    Dim ts As New TimeSpan(lnlft)
                                    rts = New DateTimeSpan.RelativeTimeSpan(rgt.RTimeSpan.Years, rgt.RTimeSpan.Months, rgt.RTimeSpan.Days - ts.Days, ts.Hours - rgt.RTimeSpan.Hours, ts.Minutes - rgt.RTimeSpan.Minutes, ts.Seconds - rgt.RTimeSpan.Seconds, ts.Milliseconds - rgt.RTimeSpan.Milliseconds)
                                    outputType = DateTimeSpan.DateTimeTypeEnum.RelativeTimeSpan
                                End If
                            End If
                        Else
                            lnResult = lnlft - lnrgt
                        End If
                    Case "+"
                        If lrel Or rrel Then
                            If lrel And rrel Then
                                rts = New DateTimeSpan.RelativeTimeSpan(lft.RTimeSpan.Years + rgt.RTimeSpan.Years, lft.RTimeSpan.Months + rgt.RTimeSpan.Months, lft.RTimeSpan.Days + rgt.RTimeSpan.Days, lft.RTimeSpan.Hours + rgt.RTimeSpan.Hours, lft.RTimeSpan.Minutes + rgt.RTimeSpan.Minutes, lft.RTimeSpan.Seconds + rgt.RTimeSpan.Seconds, lft.RTimeSpan.Milliseconds + rgt.RTimeSpan.Milliseconds)
                                outputType = DateTimeSpan.DateTimeTypeEnum.RelativeTimeSpan
                            ElseIf lrel Then
                                If rgt.DateType = DateTimeSpan.DateTimeTypeEnum.DateTime Then
                                    Dim dt As New DateTime(lnrgt)
                                    dt = dt.AddYears(lft.RTimeSpan.Years)
                                    dt = dt.AddMonths(lft.RTimeSpan.Months)
                                    dt = dt.AddDays(lft.RTimeSpan.Days)
                                    dt = dt.AddHours(lft.RTimeSpan.Hours)
                                    dt = dt.AddMinutes(lft.RTimeSpan.Minutes)
                                    dt = dt.AddSeconds(lft.RTimeSpan.Seconds)
                                    dt = dt.AddMilliseconds(lft.RTimeSpan.Milliseconds)
                                    lnResult = dt.Ticks
                                    outputType = DateTimeSpan.DateTimeTypeEnum.DateTime
                                ElseIf rgt.DateType = DateTimeSpan.DateTimeTypeEnum.TimeSpan Then
                                    rts = New DateTimeSpan.RelativeTimeSpan(lft.RTimeSpan.Years, lft.RTimeSpan.Months, lft.RTimeSpan.Days + rgt.TimeSpan.Days, lft.RTimeSpan.Hours + rgt.TimeSpan.Hours, lft.RTimeSpan.Minutes + rgt.TimeSpan.Minutes, lft.RTimeSpan.Seconds + rgt.TimeSpan.Seconds, lft.RTimeSpan.Milliseconds + rgt.TimeSpan.Milliseconds)
                                    outputType = DateTimeSpan.DateTimeTypeEnum.RelativeTimeSpan
                                Else
                                    Dim ts As New TimeSpan(lnrgt)
                                    rts = New DateTimeSpan.RelativeTimeSpan(lft.RTimeSpan.Years, lft.RTimeSpan.Months, lft.RTimeSpan.Days + ts.Days, lft.RTimeSpan.Hours + ts.Hours, lft.RTimeSpan.Minutes + ts.Minutes, lft.RTimeSpan.Seconds + ts.Seconds, lft.RTimeSpan.Milliseconds + ts.Milliseconds)
                                    outputType = DateTimeSpan.DateTimeTypeEnum.RelativeTimeSpan
                                End If
                            Else
                                If lft.DateType = DateTimeSpan.DateTimeTypeEnum.DateTime Then
                                    Dim dt As New DateTime(lnlft)
                                    dt = dt.AddYears(rgt.RTimeSpan.Years)
                                    dt = dt.AddMonths(rgt.RTimeSpan.Months)
                                    dt = dt.AddDays(rgt.RTimeSpan.Days)
                                    dt = dt.AddHours(rgt.RTimeSpan.Hours)
                                    dt = dt.AddMinutes(rgt.RTimeSpan.Minutes)
                                    dt = dt.AddSeconds(rgt.RTimeSpan.Seconds)
                                    dt = dt.AddMilliseconds(rgt.RTimeSpan.Milliseconds)
                                    lnResult = dt.Ticks
                                    outputType = DateTimeSpan.DateTimeTypeEnum.DateTime
                                ElseIf lft.DateType = DateTimeSpan.DateTimeTypeEnum.TimeSpan Then
                                    rts = New DateTimeSpan.RelativeTimeSpan(rgt.RTimeSpan.Years, rgt.RTimeSpan.Months, lft.TimeSpan.Days + rgt.RTimeSpan.Days, lft.TimeSpan.Hours + rgt.RTimeSpan.Hours, lft.TimeSpan.Minutes + rgt.RTimeSpan.Minutes, lft.TimeSpan.Seconds + rgt.RTimeSpan.Seconds, lft.TimeSpan.Milliseconds + rgt.RTimeSpan.Milliseconds)
                                    outputType = DateTimeSpan.DateTimeTypeEnum.RelativeTimeSpan
                                Else
                                    Dim ts As New TimeSpan(lnlft)
                                    rts = New DateTimeSpan.RelativeTimeSpan(rgt.RTimeSpan.Years, rgt.RTimeSpan.Months, rgt.RTimeSpan.Days + ts.Days, ts.Hours + rgt.RTimeSpan.Hours, ts.Minutes + rgt.RTimeSpan.Minutes, ts.Seconds + rgt.RTimeSpan.Seconds, ts.Milliseconds + rgt.RTimeSpan.Milliseconds)
                                    outputType = DateTimeSpan.DateTimeTypeEnum.RelativeTimeSpan
                                End If
                            End If
                        Else
                            lnResult = lnlft + lnrgt
                        End If
                    Case "*"
                        lnResult = lnlft * lnrgt
                    Case "/", "\"
                        lnResult = lnlft / lnrgt
                    Case "<"
                        outputType = DateTimeSpan.DateTimeTypeEnum.Boolean
                        If lnlft < lnrgt Then
                            lnResult = 1
                        Else
                            lnResult = 0
                        End If
                    Case ">"
                        outputType = DateTimeSpan.DateTimeTypeEnum.Boolean
                        If lnlft > lnrgt Then
                            lnResult = 1
                        Else
                            lnResult = 0
                        End If
                    Case "="
                        outputType = DateTimeSpan.DateTimeTypeEnum.Boolean
                        If lnlft = lnrgt Then
                            lnResult = 1
                        Else
                            lnResult = 0
                        End If
                    Case "<>", "!="
                        outputType = DateTimeSpan.DateTimeTypeEnum.Boolean
                        If lnlft <> lnrgt Then
                            lnResult = 1
                        Else
                            lnResult = 0
                        End If
                    Case "<="
                        outputType = DateTimeSpan.DateTimeTypeEnum.Boolean
                        If lnlft <= lnrgt Then
                            lnResult = 1
                        Else
                            lnResult = 0
                        End If
                    Case ">="
                        outputType = DateTimeSpan.DateTimeTypeEnum.Boolean
                        If lnlft >= lnrgt Then
                            lnResult = 1
                        Else
                            lnResult = 0
                        End If
                    Case Else
                        outputType = DateTimeSpan.DateTimeTypeEnum.Boolean
                        lnResult = 0
                        bReturn = False
                End Select

                Select Case outputType
                    Case DateTimeSpan.DateTimeTypeEnum.Boolean
                        If lnResult = 1 Then
                            Result = (True).ToString
                        Else
                            Result = (False).ToString
                        End If
                    Case DateTimeSpan.DateTimeTypeEnum.DateTime
                        Dim dt As New DateTime(lnResult)
                        Result = "#" & dt.ToString("G") & "#"
                        dt = Nothing
                    Case DateTimeSpan.DateTimeTypeEnum.RelativeTimeSpan
                        Result = "#" & String.Format("{0}/{1}/{2} {3}:{4}:{5}.{6}", New Object() {rts.Months, rts.Days, rts.Years, rts.Hours, rts.Minutes, rts.Seconds, rts.Milliseconds}) & "#"
                        rts = Nothing
                    Case DateTimeSpan.DateTimeTypeEnum.TimeSpan
                        Dim ts As New TimeSpan(lnResult)
                        Result = "#" & String.Format("0/{0}/0 {1}:{2}:{3}.{4}", New Object() {ts.Days, ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds}) & "#"
                        ts = Nothing
                End Select

                Return bReturn

            End Function
            Public Function Evaluate(ByRef Value As String) As Boolean
                '            Info = ""
                '            Info &= ("Evaluate: " & Value & "<br>")
                Dim strs() As String
                strs = ParameterizeString(Value, New String() {"<>", "!=", "<=", ">=", "=", "+", "-", "/", "\", "<", ">", "*", "%", "^", " AND ", " OR ", " XOR ", " AND:BIT ", " OR:BIT ", " XOR:BIT "}, Nothing, " "c, True, True, New String() {"-"})
                Dim i As Integer = 0
                Dim abort As Boolean = False

                _items = GetMathArray(strs)
                '            Info &= ("Items: " & _items.Count & "<br>")
                'Infos()

                If _items.Count > 0 Then
                    'RUN EVALUATION

                    Dim lastCount As Integer = 0
                    While _items.Count > 1 And _items.Count <> lastCount
                        lastCount = _items.Count
                        Dim nItemIndex As Integer = 0
                        Dim foundSymbol As Boolean = False
                        Dim cItem As MathItem = Nothing
                        While Not foundSymbol And nItemIndex < _items.Count
                            cItem = _items(nItemIndex)
                            If cItem.MathType = MathItem.MathItemType.Symbol Then
                                'LEFT AND RIGHT MUST BE NUMBERS
                                If Not cItem.objLeft Is Nothing AndAlso Not cItem.objRight Is Nothing Then
                                    If CType(cItem.objLeft, MathItem).MathType = MathItem.MathItemType.Number AndAlso CType(cItem.objRight, MathItem).MathType = MathItem.MathItemType.Number Then
                                        foundSymbol = True
                                    End If
                                End If
                            End If
                            If Not foundSymbol Then
                                nItemIndex += 1
                            End If
                        End While
                        If foundSymbol Then
                            Dim leftObj As MathItem = cItem.objLeft
                            Dim rightObj As MathItem = cItem.objRight
                            leftObj.Value = EvaluateValue(leftObj.Value)
                            If leftObj.Value.ToUpper = "TRUE" Or leftObj.Value.ToUpper = "FALSE" Then
                                leftObj.ValueType = MathItem.MathValueType.Boolean
                            End If
                            rightObj.Value = EvaluateValue(rightObj.Value)
                            If rightObj.Value.ToUpper = "TRUE" Or rightObj.Value.ToUpper = "FALSE" Then
                                rightObj.ValueType = MathItem.MathValueType.Boolean
                            End If
                            If leftObj.Value.StartsWith("#") AndAlso leftObj.Value.EndsWith("#") Then
                                leftObj.ValueType = MathItem.MathValueType.DateTime
                            End If
                            If rightObj.Value.StartsWith("#") AndAlso rightObj.Value.EndsWith("#") Then
                                rightObj.ValueType = MathItem.MathValueType.DateTime
                            End If
                            Select Case cItem.Value.ToUpper
                                Case "AND"
                                    If leftObj.ValueType = MathItem.MathValueType.Boolean AndAlso rightObj.ValueType = MathItem.MathValueType.Boolean Then
                                        cItem.Value = (CBool(leftObj.Value) And CBool(rightObj.Value)).ToString
                                    ElseIf IsNumeric(leftObj.Value) AndAlso IsNumeric(rightObj.Value) Then
                                        cItem.Value = (CDbl(leftObj.Value) And CDbl(rightObj.Value))
                                    Else
                                        cItem.Value = (False).ToString
                                    End If
                                Case "OR"
                                    If leftObj.ValueType = MathItem.MathValueType.Boolean AndAlso rightObj.ValueType = MathItem.MathValueType.Boolean Then
                                        cItem.Value = (CBool(leftObj.Value) Or CBool(rightObj.Value)).ToString
                                    ElseIf IsNumeric(leftObj.Value) AndAlso IsNumeric(rightObj.Value) Then
                                        cItem.Value = (CDbl(leftObj.Value) Or CDbl(rightObj.Value))
                                    Else
                                        cItem.Value = (True).ToString
                                    End If
                                Case "XOR"
                                    If leftObj.ValueType = MathItem.MathValueType.Boolean AndAlso rightObj.ValueType = MathItem.MathValueType.Boolean Then
                                        cItem.Value = (CBool(leftObj.Value) Xor CBool(rightObj.Value)).ToString
                                    ElseIf IsNumeric(leftObj.Value) AndAlso IsNumeric(rightObj.Value) Then
                                        cItem.Value = (CDbl(leftObj.Value) Xor CDbl(rightObj.Value))
                                    Else
                                        cItem.Value = (True).ToString
                                    End If
                                Case "AND:BIT"
                                    Dim lValue As BitArray = toBitArray(leftObj.Value)
                                    Dim rValue As BitArray = toBitArray(rightObj.Value)
                                    lengthBitArray(lValue, rValue)
                                    cItem.Value = fromBitArray(lValue.And(rValue))
                                Case "OR:BIT"
                                    Dim lValue As BitArray = toBitArray(leftObj.Value)
                                    Dim rValue As BitArray = toBitArray(rightObj.Value)
                                    lengthBitArray(lValue, rValue)
                                    cItem.Value = fromBitArray(lValue.Or(rValue))
                                Case "XOR:BIT"
                                    Dim lValue As BitArray = toBitArray(leftObj.Value)
                                    Dim rValue As BitArray = toBitArray(rightObj.Value)
                                    lengthBitArray(lValue, rValue)
                                    cItem.Value = fromBitArray(lValue.Xor(rValue))
                                Case "-"
                                    If IsNumeric(leftObj.Value) AndAlso IsNumeric(rightObj.Value) Then
                                        cItem.Value = (CDbl(leftObj.Value) - CDbl(rightObj.Value))
                                    ElseIf leftObj.ValueType = MathItem.MathValueType.Boolean AndAlso rightObj.ValueType = MathItem.MathValueType.Boolean Then
                                        cItem.Value = False
                                    ElseIf leftObj.ValueType = MathItem.MathValueType.DateTime OrElse rightObj.ValueType = MathItem.MathValueType.DateTime Then
                                        DateMath(leftObj.Value, rightObj.Value, cItem.Value, cItem.Value)
                                    Else
                                        cItem.Value = CStr(leftObj.Value).Replace(CStr(rightObj.Value), "")
                                    End If
                                Case "+"
                                    If IsNumeric(leftObj.Value) AndAlso IsNumeric(rightObj.Value) Then
                                        cItem.Value = (CDbl(leftObj.Value) + CDbl(rightObj.Value))
                                    ElseIf leftObj.ValueType = MathItem.MathValueType.Boolean AndAlso rightObj.ValueType = MathItem.MathValueType.Boolean Then
                                        cItem.Value = False
                                    ElseIf leftObj.ValueType = MathItem.MathValueType.DateTime OrElse rightObj.ValueType = MathItem.MathValueType.DateTime Then
                                        DateMath(leftObj.Value, rightObj.Value, cItem.Value, cItem.Value)
                                    Else
                                        cItem.Value = CStr(leftObj.Value) + CStr(rightObj.Value)
                                    End If
                                Case "*"
                                    If IsNumeric(leftObj.Value) AndAlso IsNumeric(rightObj.Value) Then
                                        cItem.Value = (CDbl(leftObj.Value) * CDbl(rightObj.Value))
                                    ElseIf leftObj.ValueType = MathItem.MathValueType.Boolean AndAlso rightObj.ValueType = MathItem.MathValueType.Boolean Then
                                        cItem.Value = False
                                    ElseIf leftObj.ValueType = MathItem.MathValueType.DateTime OrElse rightObj.ValueType = MathItem.MathValueType.DateTime Then
                                        DateMath(leftObj.Value, rightObj.Value, cItem.Value, cItem.Value)
                                    Else
                                        'cItem.Value = CStr(leftObj.Value) * CStr(rightObj.Value)
                                        Return False
                                    End If
                                Case "/", "\"
                                    If IsNumeric(leftObj.Value) AndAlso IsNumeric(rightObj.Value) Then
                                        cItem.Value = (CDbl(leftObj.Value) / CDbl(rightObj.Value))
                                    ElseIf leftObj.ValueType = MathItem.MathValueType.Boolean AndAlso rightObj.ValueType = MathItem.MathValueType.Boolean Then
                                        cItem.Value = False
                                    ElseIf leftObj.ValueType = MathItem.MathValueType.DateTime OrElse rightObj.ValueType = MathItem.MathValueType.DateTime Then
                                        DateMath(leftObj.Value, rightObj.Value, cItem.Value, cItem.Value)
                                    Else
                                        'cItem.Value = CStr(leftObj.Value) / CStr(rightObj.Value)
                                        Return False
                                    End If
                                Case "%"
                                    If IsNumeric(leftObj.Value) AndAlso IsNumeric(rightObj.Value) Then
                                        cItem.Value = (CDbl(leftObj.Value) Mod CDbl(rightObj.Value))
                                    ElseIf leftObj.ValueType = MathItem.MathValueType.Boolean AndAlso rightObj.ValueType = MathItem.MathValueType.Boolean Then
                                        cItem.Value = False
                                    Else
                                        'cItem.Value = CStr(leftObj.Value) Mod CStr(rightObj.Value)
                                        Return False
                                    End If
                                Case "^"
                                    If IsNumeric(leftObj.Value) AndAlso IsNumeric(rightObj.Value) Then
                                        cItem.Value = (CDbl(leftObj.Value) ^ CDbl(rightObj.Value))
                                    ElseIf leftObj.ValueType = MathItem.MathValueType.Boolean AndAlso rightObj.ValueType = MathItem.MathValueType.Boolean Then
                                        cItem.Value = False
                                    Else
                                        'cItem.Value = CStr(leftObj.Value) ^ CStr(rightObj.Value)
                                        Return False
                                    End If
                                Case "<"
                                    If IsNumeric(leftObj.Value) AndAlso IsNumeric(rightObj.Value) Then
                                        cItem.Value = (CDbl(leftObj.Value) < CDbl(rightObj.Value))
                                    ElseIf leftObj.ValueType = MathItem.MathValueType.Boolean AndAlso rightObj.ValueType = MathItem.MathValueType.Boolean Then
                                        cItem.Value = False
                                    ElseIf leftObj.ValueType = MathItem.MathValueType.DateTime OrElse rightObj.ValueType = MathItem.MathValueType.DateTime Then
                                        DateMath(leftObj.Value, rightObj.Value, cItem.Value, cItem.Value)
                                    Else
                                        cItem.Value = CStr(leftObj.Value) < CStr(rightObj.Value)
                                    End If
                                Case ">"
                                    If IsNumeric(leftObj.Value) AndAlso IsNumeric(rightObj.Value) Then
                                        cItem.Value = (CDbl(leftObj.Value) > CDbl(rightObj.Value))
                                    ElseIf leftObj.ValueType = MathItem.MathValueType.Boolean AndAlso rightObj.ValueType = MathItem.MathValueType.Boolean Then
                                        cItem.Value = False
                                    ElseIf leftObj.ValueType = MathItem.MathValueType.DateTime OrElse rightObj.ValueType = MathItem.MathValueType.DateTime Then
                                        DateMath(leftObj.Value, rightObj.Value, cItem.Value, cItem.Value)
                                    Else
                                        cItem.Value = CStr(leftObj.Value) > CStr(rightObj.Value)
                                    End If
                                Case "="
                                    If IsNumeric(leftObj.Value) AndAlso IsNumeric(rightObj.Value) Then
                                        cItem.Value = (CDbl(leftObj.Value) = CDbl(rightObj.Value))
                                    ElseIf leftObj.ValueType = MathItem.MathValueType.Boolean AndAlso rightObj.ValueType = MathItem.MathValueType.Boolean Then
                                        cItem.Value = (CBool(leftObj.Value) = CBool(rightObj.Value))
                                    ElseIf leftObj.ValueType = MathItem.MathValueType.DateTime OrElse rightObj.ValueType = MathItem.MathValueType.DateTime Then
                                        DateMath(leftObj.Value, rightObj.Value, cItem.Value, cItem.Value)
                                    Else
                                        cItem.Value = CStr(leftObj.Value) = CStr(rightObj.Value)
                                    End If
                                Case "<>", "!="
                                    If IsNumeric(leftObj.Value) AndAlso IsNumeric(rightObj.Value) Then
                                        cItem.Value = (CDbl(leftObj.Value) <> CDbl(rightObj.Value))
                                    ElseIf leftObj.ValueType = MathItem.MathValueType.Boolean AndAlso rightObj.ValueType = MathItem.MathValueType.Boolean Then
                                        cItem.Value = (CBool(leftObj.Value) <> CBool(rightObj.Value))
                                    ElseIf leftObj.ValueType = MathItem.MathValueType.DateTime OrElse rightObj.ValueType = MathItem.MathValueType.DateTime Then
                                        DateMath(leftObj.Value, rightObj.Value, cItem.Value, cItem.Value)
                                    Else
                                        cItem.Value = CStr(leftObj.Value) <> CStr(rightObj.Value)
                                    End If
                                Case "<="
                                    If IsNumeric(leftObj.Value) AndAlso IsNumeric(rightObj.Value) Then
                                        cItem.Value = (CDbl(leftObj.Value) <= CDbl(rightObj.Value))
                                    ElseIf leftObj.ValueType = MathItem.MathValueType.Boolean AndAlso rightObj.ValueType = MathItem.MathValueType.Boolean Then
                                        cItem.Value = False
                                    ElseIf leftObj.ValueType = MathItem.MathValueType.DateTime OrElse rightObj.ValueType = MathItem.MathValueType.DateTime Then
                                        DateMath(leftObj.Value, rightObj.Value, cItem.Value, cItem.Value)
                                    Else
                                        cItem.Value = CStr(leftObj.Value) <= CStr(rightObj.Value)
                                    End If
                                Case ">="
                                    If IsNumeric(leftObj.Value) AndAlso IsNumeric(rightObj.Value) Then
                                        cItem.Value = (CDbl(leftObj.Value) >= CDbl(rightObj.Value))
                                    ElseIf leftObj.ValueType = MathItem.MathValueType.Boolean AndAlso rightObj.ValueType = MathItem.MathValueType.Boolean Then
                                        cItem.Value = False
                                    ElseIf leftObj.ValueType = MathItem.MathValueType.DateTime OrElse rightObj.ValueType = MathItem.MathValueType.DateTime Then
                                        DateMath(leftObj.Value, rightObj.Value, cItem.Value, cItem.Value)
                                    Else
                                        cItem.Value = CStr(leftObj.Value) >= CStr(rightObj.Value)
                                    End If
                            End Select
                            cItem.MathType = MathItem.MathItemType.Number
                            If cItem.Value.ToUpper = "TRUE" OrElse cItem.Value.ToUpper = "FALSE" Then
                                cItem.ValueType = MathItem.MathValueType.Boolean
                            Else
                                cItem.ValueType = MathItem.MathValueType.Numeric
                            End If
                            _items.Remove(leftObj)
                            _items.Remove(rightObj)
                            If Not leftObj.objLeft Is Nothing Then
                                Dim mI As MathItem = leftObj.objLeft
                                mI.objRight = cItem
                                cItem.objLeft = mI
                            Else
                                cItem.objLeft = Nothing
                            End If
                            If Not rightObj.objRight Is Nothing Then
                                Dim mi As MathItem = rightObj.objRight
                                mi.objLeft = cItem
                                cItem.objRight = mi
                            Else
                                cItem.objRight = Nothing
                            End If
                            'RE-SORT TO GET THE CURRENT - FIRST - ENTITY
                            _items.Sort()
                            _items.Reverse()
                            '                        Infos()
                        End If
                    End While
                    If Not _items Is Nothing AndAlso _items.Count = 1 Then
                        'ROMAIN: Generic replacement - 08/20/2007
                        'Value = EvaluateValue(CType(_items(0), MathItem).Value)
                        Value = EvaluateValue(_items(0).Value)
                        Return True
                    End If
                End If
                Return False
            End Function
            'Public Function Infos() As String
            '    Dim i As Integer = 0
            '    Dim str As String = ""
            '    If _items.Count > 0 Then
            '        'RUN EVALUATION
            '        '1+2-3*4/5*6-7*8-9
            '        'ROMAIN: Generic replacement - 08/20/2007
            '        'Dim expression As New ArrayList
            '        Dim expression As New List(Of ExpressionItem)
            '        For i = 0 To _items.Count - 1
            '            'ROMAIN: Generic replacement - 08/20/2007
            '            'expression.Add(CType(_items.Item(i), MathItem).GetExpressionValue)
            '            expression.Add(_items.Item(i).GetExpressionValue)
            '        Next
            '        expression.Sort()
            '        Dim exp As ExpressionItem
            '        For Each exp In expression
            '            str &= exp.Value
            '        Next
            '        expression.Clear()
            '        expression = Nothing
            '    End If
            '    Debug.WriteLine(str)
            '    Return Nothing
            'End Function
        End Class

        Public Function EvaluateCondition(ByVal Caller As Engine, ByVal CurrentDS As DataSet, ByVal CapturedMessages As SortedList(Of String, String), ByVal LeftHand As String, ByVal [Operator] As String, ByVal RightHand As String, ByRef Description As String, ByVal Debugger As Debugger) As Boolean
            If [Operator] Is Nothing OrElse [Operator].Length = 0 Then
                'ADVANCED
                LeftHand = Caller.RenderString(CurrentDS, LeftHand, CapturedMessages, False, isPreRender:=False)
                If Not Description Is Nothing Then
                    Description = LeftHand
                End If
            Else
                'CLASSIC
                LeftHand = Caller.RenderString(CurrentDS, LeftHand, CapturedMessages, False, isPreRender:=False)
                RightHand = Caller.RenderString(CurrentDS, RightHand, CapturedMessages, False, isPreRender:=False)
                If Not Description Is Nothing Then
                    Description = LeftHand & " " & [Operator] & " " & RightHand
                End If
            End If
            Return EvaluateCondition(Caller, LeftHand, [Operator], RightHand, Debugger)
        End Function
        Public Function EvaluateCondition(ByVal Caller As Engine, ByVal LeftHand As String, ByVal [Operator] As String, ByVal RightHand As String, ByVal Debugger As Debugger) As Boolean
            If [Operator] Is Nothing OrElse [Operator].Length = 0 Then
                Try
                    Dim b As Boolean = RenderMath(LeftHand)
                    If b Then
                        Return Boolean.Parse(LeftHand)
                    Else
                        Throw New Exception("Failed to process the invalid statement.")
                    End If
                Catch ex As Exception
                    If Not Debugger Is Nothing Then
                        Debugger.AppendLine("Unable to appropriately process the requested condition """ & LeftHand & """: " & ex.ToString)
                    End If
                End Try
                Return False
            Else
                Dim iComp As String

                'Get both values to same format      
                If IsDate(LeftHand) And IsDate(RightHand) Then
                    iComp = Date.Compare(CDate(LeftHand), CDate(RightHand))
                ElseIf IsNumeric(LeftHand) And IsNumeric(RightHand) Then
                    iComp = Decimal.Compare(CDec(LeftHand), CDec(RightHand))
                Else
                    If IsDate(LeftHand) Then
                        LeftHand = CDate(LeftHand).ToString("u")
                    ElseIf IsNumeric(LeftHand) Then
                        LeftHand = CDec(LeftHand).ToString("#.0000000000")
                    End If
                    If IsDate(RightHand) Then
                        RightHand = CDate(RightHand).ToString("u")
                    ElseIf IsNumeric(RightHand) Then
                        RightHand = CDec(RightHand).ToString("#.0000000000")
                    End If
                    iComp = String.Compare(LeftHand, RightHand)
                End If
                Select Case [Operator]
                    Case "="
                        Return (iComp = 0)
                    Case "<>"
                        Return (iComp <> 0)
                    Case ">"
                        Return (iComp > 0)
                    Case "<"
                        Return (iComp < 0)
                    Case ">="
                        Return (iComp >= 0)
                    Case "<="
                        Return (iComp <= 0)
                End Select
            End If
        End Function
    End Class
End Namespace