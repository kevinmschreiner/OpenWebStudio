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
Imports System.Collections.Generic
Imports r2i.OWS.Framework

Namespace r2i.OWS.Framework.Plugins.Formatters
    Public Class ParserBase
        Public Shared startvalues As Char() = New Char() {"["c, "{"c}
        Public Shared endvalues As Char() = New Char() {"]"c, "}"c}
        Public Shared escapechar As Char = "\"c
        Public Shared mathstartvalues As Char() = New Char() {"("c}
        Public Shared mathendvalues As Char() = New Char() {")"c}
        Public Shared mathescapechar As Char = " "c

        Public Shared Function ParameterizeString(ByRef Source As String, ByVal Delimiter As Char, ByVal Literal As Char, ByVal EscapeChar As Char, Optional ByVal SkipEscaping As Boolean = False) As String()
            'HANDLES THE FORMATTING OF A STRING
            Dim thisPosition As Positional = Match(Source, Nothing, Delimiter, Literal, EscapeChar)
            Dim Arr As New List(Of Positional)
            While Not thisPosition Is Nothing
                Dim p As New Positional(thisPosition.Starting, thisPosition.Ending)
                p.StartChar = thisPosition.StartChar
                p.EndChar = thisPosition.EndChar
                Arr.Add(p)

                'MOVE THE POINTER FORWARD
                thisPosition.Starting = thisPosition.Ending

                thisPosition = Match(Source, thisPosition, Delimiter, Literal, EscapeChar)

                If thisPosition Is Nothing AndAlso p.Ending < Source.Length Then
                    Dim last As New Positional(p.Ending, Source.Length)
                    last.StartChar = Delimiter
                    last.EndChar = Char.MinValue
                    Arr.Add(last)
                End If
            End While
            Dim result As New List(Of String)
            Dim lastChar As Char
            Dim lastEnd As Integer = -1
            Dim appendLiteral As Boolean = False
            If Arr.Count > 0 Then
                For Each thisPosition In Arr
                    Select Case thisPosition.StartChar
                        Case Char.MinValue
                            appendLiteral = False
                            result.Add(Source.Substring(thisPosition.Starting, thisPosition.Ending - thisPosition.Starting))
                        Case Delimiter
                            appendLiteral = False
                            If Not lastChar = Literal Then
                                result.Add(Source.Substring(thisPosition.Starting + 1, thisPosition.Ending - thisPosition.Starting - 1).Trim)
                            End If
                        Case Literal
                            Dim value As String
                            value = Source.Substring(thisPosition.Starting + 1, thisPosition.Ending - thisPosition.Starting - 1)
                            If thisPosition.Starting > lastEnd And (lastEnd > -1 OrElse thisPosition.Starting > 0) Then
                                Select Case True
                                    Case lastEnd > -1 AndAlso Source.Substring(lastEnd, thisPosition.Starting - lastEnd) = Literal
                                        'TE""ST
                                        value = Literal & value
                                    Case Source.Substring(lastEnd + 1, thisPosition.Starting - lastEnd - 1).Trim.Length > 0
                                        'VALUE="TEST"
                                        appendLiteral = True
                                        value = Source.Substring(lastEnd + 1, thisPosition.Starting - lastEnd - 1).Trim & Literal & value & Literal
                                End Select
                            End If
                            If Not lastChar = Literal Then
                                result.Add(value)
                            Else
                                If appendLiteral Then
                                    If value.StartsWith(Literal) Then
                                        value = value.Remove(0, 1)
                                    End If
                                    value = value & Literal
                                End If
                                result.Item(result.Count - 1) = result.Item(result.Count - 1) & value  'Source.Substring(thisPosition.Starting, thisPosition.Ending - thisPosition.Starting)
                            End If
                    End Select
                    lastEnd = thisPosition.Ending
                    lastChar = thisPosition.StartChar
                Next
            End If
            If Not Source Is Nothing AndAlso Source.Length > 0 AndAlso lastEnd < Source.Length Then
                If lastEnd = -1 Then lastEnd = 0
                result.Add(Source.Substring(lastEnd, Source.Length - lastEnd))
            End If

            'STIP ESCAPES
            If Not result Is Nothing AndAlso result.Count > 0 AndAlso Not SkipEscaping Then
                Dim resulti As Integer = 0
                For resulti = 0 To result.Count - 1
                    StripEscapes(result(resulti), New Char() {Delimiter, Literal, "{"c, "}"c, "["c, "]"c}, EscapeChar)
                Next
            End If
            'ROMAIN: Generic replacement - 08/20/2007
            'Return result.ToArray(GetType(String)
            Return result.ToArray
        End Function
        Public Shared Function ParameterizeString(ByRef Source As String, ByVal Delimiters As String(), ByVal Literal As Char, ByVal EscapeChar As Char, Optional ByVal SkipEscaping As Boolean = False, Optional ByVal includeSplitter As Boolean = False, Optional ByRef StartDelimiters As String() = Nothing) As String()

            Dim thisPosition As Positional = Match(Source, Nothing, Delimiters, Literal, EscapeChar, StartDelimiters)
            Dim Arr As New List(Of Positional)

            While Not thisPosition Is Nothing
                Dim p As New Positional(thisPosition.Starting, thisPosition.Ending)
                p.StartStr = thisPosition.StartStr
                p.EndStr = thisPosition.EndStr
                Arr.Add(p)

                If p.StartStr Is Nothing Then p.StartStr = ""
                If p.EndStr Is Nothing Then p.EndStr = ""

                'MOVE THE POINTER FORWARD
                thisPosition.Starting = thisPosition.Ending + p.EndStr.Length

                thisPosition = Match(Source, thisPosition, Delimiters, Literal, EscapeChar, StartDelimiters)

                If thisPosition Is Nothing AndAlso p.Ending < Source.Length Then
                    Dim last As New Positional(p.Ending + p.EndStr.Length, Source.Length)
                    last.StartStr = "" 'p.EndStr 'Delimiter????
                    last.EndStr = ""
                    Arr.Add(last)
                End If
            End While
            'ROMAIN: Generic replacement - 08/20/2007
            'Dim result As New ArrayList
            Dim result As New List(Of String)
            Dim laststr As String
            Dim lastEnd As Integer = -1
            Dim appendLiteral As Boolean = False
            If Arr.Count > 0 Then
                For Each thisPosition In Arr
                    If lastEnd < thisPosition.Starting And lastEnd > 0 Then
                        result.Add(Source.Substring(lastEnd, thisPosition.Starting - lastEnd).Trim)
                    End If
                    result.Add(Source.Substring(thisPosition.Starting, thisPosition.Ending - thisPosition.Starting))
                    lastEnd = thisPosition.Ending
                    laststr = thisPosition.StartStr
                Next
            End If
            If Not Source Is Nothing AndAlso Source.Length > 0 AndAlso lastEnd < Source.Length Then
                If lastEnd = -1 Then lastEnd = 0
                result.Add(Source.Substring(lastEnd, Source.Length - lastEnd))
            End If
            'ROMAIN: Generic replacement - 08/20/2007
            'Return result.ToArray(GetType(String))
            Return result.ToArray
        End Function

        Protected Function ParameterMerge(ByRef Parameters As Array, ByVal Source As String, ByVal Start As Integer) As String
            Dim result As String = ""
            If Parameters.Length > Start Then
                Dim index As Integer = Source.IndexOf(Parameters(Start))
                If index >= 0 Then
                    result = Source.Substring(index)
                End If
            End If
            Return result
        End Function

        Public Shared Sub StripEscapes(ByRef Source As String, ByRef Chars() As Char, ByVal Escape As Char)
            Dim chr As Char
            Dim strR As String = ""
            If Escape = "\"c OrElse Escape = "["c Then
                strR &= "\"
            End If
            strR &= Escape & "(["
            For Each chr In Chars
                If chr = "["c OrElse chr = "]"c Then
                    strR &= "\"
                End If
                strR &= chr
            Next
            strR &= "])"
            Source = System.Text.RegularExpressions.Regex.Replace(Source, strR, "$1")
        End Sub
        Public Shared Sub StripEscapes(ByRef Source As String)
            Dim tArray(startvalues.Length + endvalues.Length) As Char
            Array.Copy(startvalues, tArray, startvalues.Length)
            Array.Copy(endvalues, 0, tArray, startvalues.Length, endvalues.Length)

            StripEscapes(Source, tArray, escapechar)
        End Sub

        Public Shared Function AddEscapes(ByRef Source As String, ByRef Chars() As Char, ByVal Escape As Char) As String
            Dim c As Char
            For Each c In Chars
                Source = Source.Replace(c, Escape & c)
            Next
            Return Source
        End Function
        Protected Function AddEscapes(ByRef Source As String) As String
            Dim tArray(startvalues.Length + endvalues.Length) As Char
            Array.Copy(startvalues, tArray, startvalues.Length)
            Array.Copy(endvalues, 0, tArray, startvalues.Length, endvalues.Length)

            Return AddEscapes(Source, tArray, escapechar)
        End Function



        '' Now defined in Core.EngineBase
        Private Shared Function CharPosition(ByRef Source As String, ByVal Starting As Integer, ByRef Values As Char(), ByVal EscapeValue As Char, Optional ByVal Reverse As Boolean = False) As Integer
            'IDENTIFIES THE STARTING POSITION OF THE FIRST OCCURING VALUE WITHIN THE SOURCE
            'WHICH CONTAINS ONE OF THE PROVIDED VALUE CHARS AND IS NOT PRECEEDED BY THE ESCAPE VALUE
            'THIS CHECK CAN BE FROM THE LASTINDEX OF A CHAR POSITION BACKWARDS OR FROM THE FIRST APPEARANCE FORWARDS

            If Starting = -1 Then
                If Reverse Then
                    Starting = Source.LastIndexOfAny(Values)
                Else
                    Starting = Source.IndexOfAny(Values)
                End If
            Else
                If Reverse Then
                    If Starting = 0 Then
                        Starting = Source.LastIndexOfAny(Values, Starting)
                        If Starting = 0 Then
                            Starting = -1
                        End If
                    Else
                        Starting = Source.LastIndexOfAny(Values, Starting - 1)
                    End If
                Else
                    Starting = Source.IndexOfAny(Values, Starting + 1)
                End If
            End If
            Dim Found As Boolean = False
            While Not Found AndAlso Starting > -1
                'CHECK TO ESCAPECHAR
                If Starting > 0 Then
                    If EscapeValue = " "c OrElse Not Source.Chars(Starting - 1) = EscapeValue Then
                        Found = True
                    Else
                        If Reverse Then
                            Starting = Source.LastIndexOfAny(Values, Starting - 1)
                        Else
                            Starting = Source.IndexOfAny(Values, Starting + 1)
                        End If
                    End If
                Else
                    Found = True
                End If
            End While
            Return Starting
        End Function
        Private Shared Function IndexOfAnyString(ByVal value As String, ByVal values As String(), ByVal Last As Boolean, ByRef Result As String, Optional ByVal starting As Integer = 0) As Integer
            Dim str As String
            Dim index As Integer = -1
            Dim xindex As Integer = -1
            Result = Nothing
            value = value.ToUpper
            For Each str In values
                str = str.ToUpper
                If starting < value.Length Then
                    If Last Then
                        xindex = value.LastIndexOf(str, starting)
                        If index > -1 And xindex > index Then
                            Result = str
                            index = xindex
                        End If
                    Else
                        xindex = value.IndexOf(str, starting)
                        If xindex > -1 And (xindex < index Or index = -1) Then
                            Result = str
                            index = xindex
                        End If
                    End If
                End If
            Next
            Return index
        End Function
        '07/21/2010 KMS: ADDED TO MERGE STRING VALUES TOGETHER WHEN THEY APPEAR AS QUOTED REGIONS ('-1'='whatever')
        '<ticket:234>
        Private Shared Function ContainsAnyString(ByVal value As String, ByVal values As String()) As Boolean
            Dim str As String
            Dim xindex As Integer = -1
            Dim found As Boolean = False
            value = value.ToUpper
            For Each str In values
                str = str.ToUpper
                xindex = value.IndexOf(str)
                If xindex > -1 Then
                    found = True
                    Exit For
                End If
            Next
            Return found
        End Function
        '</ticket:234>

        Private Shared Function StringPosition(ByRef Source As String, ByVal Starting As Integer, ByRef Values As String(), ByVal EscapeValue As Char, ByRef Result As String, Optional ByVal Reverse As Boolean = False) As Integer
            'IDENTIFIES THE STARTING POSITION OF THE FIRST OCCURING VALUE WITHIN THE SOURCE
            'WHICH CONTAINS ONE OF THE PROVIDED VALUE CHARS AND IS NOT PRECEEDED BY THE ESCAPE VALUE
            'THIS CHECK CAN BE FROM THE LASTINDEX OF A CHAR POSITION BACKWARDS OR FROM THE FIRST APPEARANCE FORWARDS

            Result = Nothing
            If Starting = -1 Then
                If Reverse Then
                    Starting = IndexOfAnyString(Source, Values, True, Result)
                Else
                    Starting = IndexOfAnyString(Source, Values, False, Result)
                End If
            Else
                If Reverse Then
                    If Starting = 0 Then
                        Starting = IndexOfAnyString(Source, Values, True, Result, Starting)
                        If Starting = 0 Then
                            Starting = -1
                        End If
                    Else
                        Starting = IndexOfAnyString(Source, Values, True, Result, Starting - 1)
                    End If
                Else
                    Starting = IndexOfAnyString(Source, Values, False, Result, Starting + 1)
                End If
            End If
            Dim Found As Boolean = False
            While Not Found AndAlso Starting > -1
                'CHECK TO ESCAPECHAR
                If Starting > 0 Then
                    If EscapeValue = " "c OrElse Not Source.Chars(Starting - 1) = EscapeValue Then
                        Found = True
                    Else
                        If Reverse Then
                            Starting = IndexOfAnyString(Source, Values, True, Result, Starting - 1)
                        Else
                            Starting = IndexOfAnyString(Source, Values, False, Result, Starting + 1)
                        End If
                    End If
                Else
                    Found = True
                End If
            End While
            Return Starting
        End Function

        Public Shared Function Match(ByRef Source As String, ByVal Position As Positional, ByRef StartValues As Char(), ByRef EndValues As Char(), ByRef EscapeValue As Char) As Positional
            'The Match Function attempts to find the latest occurence of the StartValue Item with matching EndValue item
            'both of which have not been immediatly preceeded by the escape value
            '
            'The Position parameter provides information at to the current position of the last Match, so
            'the new match must occur outside of its boundaries.
            '
            'If the Position is nothing, orelse the Position.Starting  = -1 then, the objective is to find the last match.
            '
            '1. Check to see if the Position is nothing - if so, Position = new Position(-1,-1)
            '2. Check to see if the Position.Starting > -1, if not Set Starting = Position of the LastIndexOf any of the StartValues.
            '   Check to see if the current position Starting element is preceeded by the Escape character. If so - find the next Starting.


            If Position Is Nothing Then
                Position = New Positional(-1, -1)
            End If
            Dim Found As Boolean = False
            While Not Found AndAlso Not Position Is Nothing
                Position.Starting = CharPosition(Source, Position.Starting, StartValues, EscapeValue, True)
                If Position.Starting > -1 Then
                    'THE START POSITION HAS BEEN IDENTFIED
                    Dim i As Integer = 0
                    Position.EndChar = Char.MinValue
                    For i = 0 To StartValues.Length - 1
                        If StartValues(i) = Source.Chars(Position.Starting) Then
                            Position.StartChar = StartValues(i)
                            Position.EndChar = EndValues(i)
                        End If
                    Next
                    Position.Ending = CharPosition(Source, Position.Starting, New Char() {Position.EndChar}, EscapeValue)
                    If Position.Ending > -1 AndAlso Position.Ending > Position.Starting Then
                        Found = True
                    ElseIf Position.Ending = Position.Starting OrElse Position.Ending = -1 Then
                        Position = Nothing
                    End If
                Else
                    Position = Nothing
                End If
            End While
            Return Position
        End Function
        '07/21/2010 KMS: ADDED TO MERGE STRING VALUES TOGETHER WHEN THEY APPEAR AS QUOTED REGIONS ('-1'='whatever')
        '<ticket:234>
        Public Shared Function Match(ByRef Source As String, ByVal Position As Positional, ByRef Delimiters As String(), ByRef Literal As Char, ByRef EscapeValue As Char, ByRef StartDelimiters As String()) As Positional
            'The Match Function attempts to find the latest occurence of the StartValue Item with matching EndValue item
            'both of which have not been immediatly preceeded by the escape value
            '
            'The Position parameter provides information at to the current position of the last Match, so
            'the new match must occur outside of its boundaries.
            '
            'If the Position is nothing, orelse the Position.Starting  = -1 then, the objective is to find the last match.
            '
            '1. Check to see if the Position is nothing - if so, Position = new Position(-1,-1)
            '2. Check to see if the Position.Starting > -1, if not Set Starting = Position of the LastIndexOf any of the StartValues.
            '   Check to see if the current position Starting element is preceeded by the Escape character. If so - find the next Starting.


            If Position Is Nothing Then
                Position = New Positional(-1, -1)
            End If
            Dim Found As Boolean = False
            Dim Start As Integer = Position.Starting
            If Start = -1 AndAlso Source.Length > 0 Then
                Start = 0
            End If
            Dim delimiter As String = ""
            'ROMAIN: Generic replacement - 08/20/2007
            'Dim DelimiterArray As New ArrayList(Delimiters)
            Dim DelimiterArray As New List(Of String)(Delimiters)
            DelimiterArray.Add(Literal.ToString)
            'ROMAIN: Generic replacement - 08/20/2007
            'Dim AllDelimiters As String() = DelimiterArray.ToArray(GetType(System.String))
            Dim AllDelimiters As String() = DelimiterArray.ToArray
            DelimiterArray.Remove(Literal.ToString)
            While Not Found AndAlso Not Position Is Nothing
                Dim len As Integer = Source.Length
                Dim result As String = Nothing
                Position.Starting = StringPosition(Source, Position.Starting, AllDelimiters, EscapeValue, result)
                If Position.Starting > -1 Then
                    'THE START POSITION HAS BEEN IDENTFIED
                    Dim i As Integer = 0
                    Dim allowedStartDelimiter As Boolean
                    allowedStartDelimiter = False
                    Position.EndStr = ""

                    Select Case True
                        Case DelimiterArray.Contains(result)
                            'IF THE START IS A DELIMITER - THE START IS THE END, THE END IS STARTING.
                            If Position.Starting = 0 AndAlso Not StartDelimiters Is Nothing AndAlso StartDelimiters.Length > 0 AndAlso ContainsAnyString(result, StartDelimiters) Then
                                allowedStartDelimiter = True
                            Else
                                If Not Start = 0 Then
                                    Position.StartStr = result
                                Else
                                    Position.StartStr = ""
                                End If
                                Position.EndStr = ""
                                delimiter = result
                                Position.Ending = Position.Starting '?? + result.length ??
                                Position.Starting = Start
                            End If
                        Case result = Literal
                            'IF THE START IS A LITERAL - THE END MUST BE A LITERAL.
                            Position.StartStr = Literal
                            Position.EndStr = Literal
                    End Select
                    If Not Position.EndStr = "" Then
                        Position.Ending = StringPosition(Source, Position.Starting, New String() {Position.EndStr}, EscapeValue, result)
                        If Position.Ending > -1 AndAlso Position.Ending > Position.Starting Then
                            Found = True
                        ElseIf Position.Ending = Position.Starting OrElse Position.Ending = -1 Then
                            If Not allowedStartDelimiter Then
                                Position = Nothing
                            End If
                        End If
                    Else
                        If Position.Ending > -1 AndAlso Position.Ending > Position.Starting Then
                            Found = True
                        ElseIf Position.Ending = Position.Starting OrElse Position.Ending = -1 Then
                            If Not allowedStartDelimiter Then
                                Position = Nothing
                            End If
                        End If
                    End If
                Else
                    Position = Nothing
                End If
            End While
            If Not Position Is Nothing AndAlso (Position.EndStr Is Nothing OrElse Position.EndStr.Length = 0) Then
                Position.EndStr = delimiter
            End If
            Return Position
        End Function
        '</ticket:234>

        Public Shared Function Match(ByRef Source As String, ByVal Position As Positional, ByRef Delimiter As Char, ByRef Literal As Char, ByRef EscapeValue As Char) As Positional
            'The Match Function attempts to find the latest occurence of the StartValue Item with matching EndValue item
            'both of which have not been immediatly preceeded by the escape value
            '
            'The Position parameter provides information at to the current position of the last Match, so
            'the new match must occur outside of its boundaries.
            '
            'If the Position is nothing, orelse the Position.Starting  = -1 then, the objective is to find the last match.
            '
            '1. Check to see if the Position is nothing - if so, Position = new Position(-1,-1)
            '2. Check to see if the Position.Starting > -1, if not Set Starting = Position of the LastIndexOf any of the StartValues.
            '   Check to see if the current position Starting element is preceeded by the Escape character. If so - find the next Starting.


            If Position Is Nothing Then
                Position = New Positional(-1, -1)
            End If
            Dim Found As Boolean = False
            Dim Start As Integer = Position.Starting
            If Start = -1 AndAlso Source.Length > 0 Then
                Start = 0
            End If
            While Not Found AndAlso Not Position Is Nothing
                Dim len As Integer = Source.Length
                Position.Starting = CharPosition(Source, Position.Starting, New Char() {Delimiter, Literal}, EscapeValue)
                If Position.Starting > -1 Then
                    'THE START POSITION HAS BEEN IDENTFIED
                    Dim i As Integer = 0
                    Position.EndChar = Char.MinValue

                    Select Case Source.Chars(Position.Starting)
                        Case Delimiter
                            'IF THE START IS A DELIMITER - THE START IS THE END, THE END IS STARTING.
                            If Not Start = 0 Then
                                Position.StartChar = Delimiter
                            Else
                                Position.StartChar = Char.MinValue
                            End If
                            Position.EndChar = Char.MinValue
                            Position.Ending = Position.Starting
                            Position.Starting = Start
                        Case Literal
                            'IF THE START IS A LITERAL - THE END MUST BE A LITERAL.
                            Position.StartChar = Literal
                            Position.EndChar = Literal
                    End Select
                    If Not Position.EndChar = Char.MinValue Then
                        Position.Ending = CharPosition(Source, Position.Starting, New Char() {Position.EndChar}, EscapeValue)
                        If Position.Ending > -1 AndAlso Position.Ending > Position.Starting Then
                            Found = True
                        ElseIf Position.Ending = Position.Starting OrElse Position.Ending = -1 Then
                            Position = Nothing
                        End If
                    Else
                        If Position.Ending > -1 AndAlso Position.Ending > Position.Starting Then
                            Found = True
                        ElseIf Position.Ending = Position.Starting OrElse Position.Ending = -1 Then
                            Position = Nothing
                        End If
                    End If
                Else
                    Position = Nothing
                End If
            End While
            Return Position
        End Function

        Public Class Positional
            Public Sub New()
            End Sub
            Public Sub New(ByVal LeftIndex As Integer, ByVal RightIndex As Integer)
                Starting = LeftIndex
                Ending = RightIndex
            End Sub
            Public Starting As Integer
            Public Ending As Integer
            Public StartChar As Char
            Public EndChar As Char
            Public StartStr As String
            Public EndStr As String
        End Class

    End Class
End Namespace