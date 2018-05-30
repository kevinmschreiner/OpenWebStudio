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
Imports System
Imports System.Collections
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Diagnostics
Imports r2i.OWS.Framework
Imports r2i.OWS.Framework.Plugins.Formatters
Namespace r2i.OWS.Formatters
    Public Class Encode : Inherits FormatterBase

        Public Overrides Function Handle_Render(ByRef Caller As EngineBase, ByVal Index As Integer, ByRef Value As String, ByRef Formatter As String, ByRef Source As String, ByRef DS As System.Data.DataSet, ByRef DR As System.Data.DataRow, ByRef RuntimeMessages As System.Collections.Generic.SortedList(Of String, String), ByVal NullReturn As Boolean, ByVal NullOverride As Boolean, ByVal ProtectSession As Boolean, ByVal SessionDelimiter As String, ByVal useSessionQuotes As Boolean, ByVal useAggregations As Boolean, ByRef FilterText As String, ByRef FilterField As String, ByRef Debugger As Framework.Debugger) As Boolean
            Dim fParameter As String = Formatter.Substring(8, Formatter.Length - 9)
            If fParameter.Length > 0 Then
                Select Case fParameter.ToUpper
                    Case "HTML", "HTM"
                        Source = Web.HttpUtility.HtmlEncode(Value)
                    Case "HASH", "HASHCODE"
                        Source = Value.GetHashCode()
                    Case "REVERSE", "BACKWARD", "BACKWARDS", "FLIP"
                        Dim chr() As Char = System.Text.UTF8Encoding.UTF8.GetChars(System.Text.UTF8Encoding.UTF8.GetBytes(Value))
                        System.Array.Reverse(chr)
                        Source = chr.ToString
                    Case "JAVASCRIPT", "JSCRIPT"
                        Source = Newtonsoft.Json.JavaScriptConvert.ToString(Value)
                    Case "URI", "URL"
                        Source = Web.HttpUtility.UrlEncode(Value)
                    Case "UTF8", "UTF-8"
                        Source = System.Text.Encoding.UTF8.GetString(System.Text.UTF8Encoding.UTF8.GetBytes(Value))
                    Case "ASCII"
                        Source = System.Text.Encoding.ASCII.GetString(System.Text.UTF8Encoding.UTF8.GetBytes(Value))
                    Case "BASE64"
                        Dim bytestoEncode As Byte()
                        bytestoEncode = Encoding.UTF8.GetBytes(Value)
                        Source = Convert.ToBase64String(bytestoEncode)
                    Case "BIGENDIANUNICODE", "BIGENDIAN", "BIGE", "BE", "BEUNICODE", "BEUNI", "BIGEU", "BEU"
                        Source = System.Text.Encoding.BigEndianUnicode.GetString(System.Text.UTF8Encoding.UTF8.GetBytes(Value))
                    Case "UNICODE", "UNI", "UNIC", "UC", "U", "UNC"
                        Source = System.Text.Encoding.Unicode.GetString(System.Text.UTF8Encoding.UTF8.GetBytes(Value))
                    Case "UTF32", "UTF-32"
                        Source = System.Text.Encoding.UTF32.GetString(System.Text.UTF8Encoding.UTF8.GetBytes(Value))
                    Case "UTF7", "UTF-7"
                        Source = System.Text.Encoding.UTF7.GetString(System.Text.UTF8Encoding.UTF8.GetBytes(Value))
                        'just for fun
                    Case "QUERY"
                        Source = Caller.RenderQuery(DS, FilterField, FilterText, Caller.RecordsPerPage, RuntimeMessages, Nothing, Value)
                    Case "PIGLATIN"
                        Source = PigLatinize(Value)
                    Case "BACKSLANG"
                        Source = BackSlang(Value)
                    Case "UBBIDUBBI"
                        Source = UbbiDubbi(Value)
                    Case "IZZLE"
                        Source = Izzle(Value)
                    Case "BODI"
                        Source = Bodi(Value)
                    Case "WIKI"
                        Dim wikiM As New WikiTokenType.WikiTokenManager
                        With wikiM
                            Dim s As String
                            s = Caller.ActionVariable("ows.Wiki.CodeFormat")
                            If Not s Is Nothing AndAlso s.Length > 0 Then
                                .CodeFormat = s
                            End If
                            s = Caller.ActionVariable("ows.Wiki.LinkClassFormat")
                            If Not s Is Nothing AndAlso s.Length > 0 Then
                                .LinkClassFormat = s
                            End If
                            s = Caller.ActionVariable("ows.Wiki.LinkExternalFormat")
                            If Not s Is Nothing AndAlso s.Length > 0 Then
                                .LinkExternalFormat = s
                            End If
                            s = Caller.ActionVariable("ows.Wiki.LinkFormat")
                            If Not s Is Nothing AndAlso s.Length > 0 Then
                                .LinkFormat = s
                            End If
                            s = Caller.ActionVariable("ows.Wiki.LinkInternalFormat")
                            If Not s Is Nothing AndAlso s.Length > 0 Then
                                .LinkInternalFormat = s
                            End If
                            s = Caller.ActionVariable("ows.Wiki.LinkSectionValueFormat")
                            If Not s Is Nothing AndAlso s.Length > 0 Then
                                .LinkSectionValueFormat = s
                            End If
                            s = Caller.ActionVariable("ows.Wiki.LinkValueFormat")
                            If Not s Is Nothing AndAlso s.Length > 0 Then
                                .LinkValueFormat = s
                            End If
                            s = Caller.ActionVariable("ows.Wiki.Section0Format")
                            If Not s Is Nothing AndAlso s.Length > 0 Then
                                .Section0Format = s
                            End If
                            s = Caller.ActionVariable("ows.Wiki.Section1Format")
                            If Not s Is Nothing AndAlso s.Length > 0 Then
                                .Section1Format = s
                            End If
                            s = Caller.ActionVariable("ows.Wiki.Section2Format")
                            If Not s Is Nothing AndAlso s.Length > 0 Then
                                .Section2Format = s
                            End If
                            s = Caller.ActionVariable("ows.Wiki.Section3Format")
                            If Not s Is Nothing AndAlso s.Length > 0 Then
                                .Section3Format = s
                            End If
                            s = Caller.ActionVariable("ows.Wiki.TOC0Format")
                            If Not s Is Nothing AndAlso s.Length > 0 Then
                                .TOC0Format = s
                            End If
                            s = Caller.ActionVariable("ows.Wiki.TOC1Format")
                            If Not s Is Nothing AndAlso s.Length > 0 Then
                                .TOC1Format = s
                            End If
                            s = Caller.ActionVariable("ows.Wiki.TOC2Format")
                            If Not s Is Nothing AndAlso s.Length > 0 Then
                                .TOC2Format = s
                            End If
                            s = Caller.ActionVariable("ows.Wiki.TOCFormat3")
                            If Not s Is Nothing AndAlso s.Length > 0 Then
                                .TOC3Format = s
                            End If
                            s = Caller.ActionVariable("ows.Wiki.TOCFormat")
                            If Not s Is Nothing AndAlso s.Length > 0 Then
                                .TOCFormat = s
                            End If
                            s = Caller.ActionVariable("ows.Wiki.TOCBlockFormat")
                            If Not s Is Nothing AndAlso s.Length > 0 Then
                                .TOCBlockFormat = s
                            End If
                            s = Caller.ActionVariable("ows.Wiki.Quote")
                            If Not s Is Nothing AndAlso s.Length > 0 Then
                                .QuoteFormat = s
                            End If
                            Dim sidebarKeys As String() = Caller.ActionVariableSearch("ows.Wiki.Sidebar.")
                            If Not sidebarKeys Is Nothing AndAlso sidebarKeys.Length > 0 Then
                                Dim strKey As String
                                For Each strKey In sidebarKeys
                                    s = Caller.ActionVariable(strKey)
                                    If Not s Is Nothing AndAlso s.Length > 0 Then
                                        strKey = strKey.Substring(17)
                                        .SidebarLayout(strKey) = s
                                    End If
                                Next
                            End If
                        End With
                        Source = wikiM.Render(Value, Caller, DS, RuntimeMessages, False, False, Debugger)
                        wikiM = Nothing
                    Case Else
                        Source = Value
                End Select
            Else
                Source = Value
            End If
            Return True
        End Function
        Private Function isVowel(ByRef character As Char, ByVal lastCharacter As Boolean) As Boolean
            Select Case character
                Case "A", "E", "I", "O", "U", "a", "e", "i", "o", "u"
                    Return True
                Case Else
                    If lastCharacter = True AndAlso (character = "y"c Or character = "Y"c) Then
                        Return True
                    Else
                        Return False
                    End If
            End Select
        End Function
        Private Function BackSlang(ByVal value As String) As String
            Dim source As String = ""
            If Not value Is Nothing AndAlso value.Length > 0 Then
                'split on the spaces and symbols (other than single quote) to find word divisions
                Dim i As Integer = 0
                Dim word As String = ""
                While i < value.Length
                    'Zip Forward and Stop if the character is not a through z or single quote
                    Dim current As Char = value(i)
                    If Char.IsLetter(current) OrElse current = ("'"c) Then
                        word &= current
                    Else
                        If word.Length > 0 Then
                            Dim capFirst As Boolean = False
                            If Char.IsUpper(word(0)) Then
                                capFirst = True
                            End If
                            Dim chrs As Char() = System.Text.UTF8Encoding.UTF8.GetChars(System.Text.UTF8Encoding.UTF8.GetBytes(word))
                            Array.Reverse(chrs)
                            Dim x As Integer = 0
                            For x = 0 To chrs.Length - 1
                                chrs(x) = Char.ToLower(chrs(x))
                            Next
                            If capFirst = True Then
                                chrs(0) = Char.ToUpper(chrs(0))
                            End If
                            source &= chrs
                        End If
                        source &= current
                        word = ""
                        End If
                        i += 1
                End While
                If word.Length > 0 Then
                    Dim chrs As Char() = System.Text.UTF8Encoding.UTF8.GetChars(System.Text.UTF8Encoding.UTF8.GetBytes(word))
                    Array.Reverse(chrs)
                    source &= chrs
                End If
            End If
            Return source
        End Function
        Private Function PigLatinize(ByVal value As String) As String
            Dim source As String = ""
            If Not value Is Nothing AndAlso value.Length > 0 Then
                'split on the spaces and symbols (other than single quote) to find word divisions
                Dim i As Integer = 0
                Dim word As String = ""
                While i < value.Length
                    'Zip Forward and Stop if the character is not a through z or single quote
                    Dim current As Char = value(i)
                    If Char.IsLetter(current) OrElse current = ("'"c) Then
                        word &= current
                    Else
                        source &= PLWord(word)
                        source &= current
                        word = ""
                    End If
                    i += 1
                End While
                If word.Length > 0 Then
                    source &= PLWord(word)
                End If
            End If
            Return source
        End Function
        Private Function Izzle(ByVal value As String) As String
            Dim source As String = ""
            If Not value Is Nothing AndAlso value.Length > 0 Then
                'split on the spaces and symbols (other than single quote) to find word divisions
                Dim i As Integer = 0
                Dim word As String = ""
                While i < value.Length
                    'Zip Forward and Stop if the character is not a through z or single quote
                    Dim current As Char = value(i)
                    If Char.IsLetter(current) OrElse current = ("'"c) Then
                        word &= current
                    Else
                        source &= IzzleWord(word)
                        source &= current
                        word = ""
                    End If
                    i += 1
                End While
                If word.Length > 0 Then
                    source &= IzzleWord(word)
                End If
            End If
            Return source
        End Function
        Private Function UbbiDubbi(ByVal value As String) As String
            Dim source As String = ""
            If Not value Is Nothing AndAlso value.Length > 0 Then
                'split on the spaces and symbols (other than single quote) to find word divisions
                Dim i As Integer = 0
                Dim word As String = ""
                While i < value.Length
                    'Zip Forward and Stop if the character is not a through z or single quote
                    Dim current As Char = value(i)
                    If Char.IsLetter(current) OrElse current = ("'"c) Then
                        word &= current
                    Else
                        source &= UDWord(word)
                        source &= current
                        word = ""
                    End If
                    i += 1
                End While
                If word.Length > 0 Then
                    source &= UDWord(word)
                End If
            End If
            Return source
        End Function
        Private Function Bodi(ByVal value As String) As String
            Dim source As String = ""
            If Not value Is Nothing AndAlso value.Length > 0 Then
                'split on the spaces and symbols (other than single quote) to find word divisions
                Dim i As Integer = 0
                Dim word As String = ""
                While i < value.Length
                    'Zip Forward and Stop if the character is not a through z or single quote
                    Dim current As Char = value(i)
                    If Char.IsLetter(current) OrElse current = ("'"c) Then
                        word &= current
                    Else
                        source &= BodiWord(word)
                        source &= current
                        word = ""
                    End If
                    i += 1
                End While
                If word.Length > 0 Then
                    source &= BodiWord(word)
                End If
            End If
            Return source
        End Function
        Private Function PLWord(ByVal word As String) As String
            Dim capFirst As Boolean = False
            If word.Length > 0 Then
                If Char.IsUpper(word(0)) Then
                    capFirst = True
                End If
                Dim removeTo As Integer = -1
                Dim appendValue As String = ""
                If isVowel(word(0), (0 = word.Length - 1)) Then
                    appendValue = "way"
                Else
                    Dim j As Integer = 0
                    Dim b As Boolean = False
                    While j < word.Length AndAlso Not b
                        If isVowel(word(j), (j = word.Length - 1)) Then
                            b = True
                            removeTo = j
                        Else
                            j += 1
                        End If
                    End While
                    If removeTo > 0 Then
                        appendValue = word.Substring(0, removeTo).ToLower & "ay"
                    End If
                End If
                If removeTo > 0 Then
                    word = word.Remove(0, removeTo)
                End If
                word &= appendValue
            End If
            If capFirst Then
                word = word.Substring(0, 1).ToUpper & word.Substring(1).ToLower
            End If
            Return word
        End Function
        Private Function IzzleWord(ByVal word As String) As String
            If word.Length > 3 Then
                'Randomly izzle the wizzle
                If OWS.Framework.Utilities.Utility.Randomizer.Next Mod 2 = 0 Then
                    Dim removeTo As Integer = -1
                    Dim appendValue As String = ""

                    Dim j As Integer = 0
                    Dim b As Boolean = False
                    Dim c As Boolean = False
                    While j < word.Length AndAlso Not b
                        If isVowel(word(j), (j = word.Length - 1)) Then
                            If c Then
                                b = True
                                removeTo = j
                            Else
                                j += 1
                            End If
                        Else
                            c = True
                            j += 1
                        End If
                    End While
                    If removeTo > 0 Then
                        Select Case OWS.Framework.Utilities.Utility.Randomizer.Next(0, 3)
                            Case 0
                                appendValue = "iz"
                            Case 1
                                appendValue = "izzle"
                            Case 2
                                appendValue = "izzo"
                            Case 3
                                appendValue = "ilz"
                        End Select
                    End If

                    If removeTo > 0 Then
                        word = word.Remove(removeTo)
                    End If
                    word &= appendValue
                End If
            End If
            Return word
        End Function
        Private Function UDWord(ByVal word As String) As String
            Dim capFirst As Boolean = False
            Dim output As String = ""
            If word.Length > 0 Then
                If Char.IsUpper(word(0)) Then
                    capFirst = True
                End If
                Dim j As Integer
                Dim wasVowel As Boolean = False
                For j = 0 To word.Length - 1
                    Dim replaced As Boolean = False
                    If isVowel(word(j), (j = word.Length - 1)) Then
                        If Not wasVowel Then
                            output &= "ub" & word(j)
                            replaced = True
                        End If
                        wasVowel = True
                    Else
                        wasVowel = False
                    End If
                    If Not replaced Then output &= word(j)
                Next
            End If
            If capFirst Then
                output = output.Substring(0, 1).ToUpper & output.Substring(1).ToLower
            End If
            Return output
        End Function
        Private Function BodiWord(ByVal word As String) As String
            Dim capFirst As Boolean = False
            Dim output As String = ""
            If word.Length > 0 Then
                If Char.IsUpper(word(0)) Then
                    capFirst = True
                End If
                Dim j As Integer
                Dim wasVowel As Boolean = False
                For j = 0 To word.Length - 1
                    Dim replaced As Boolean = False
                    If isVowel(word(j), (j = word.Length - 1)) Then
                        If Not wasVowel Then
                            output &= word(j)
                            Dim appendValue As String = ""
                            If OWS.Framework.Utilities.Utility.Randomizer.Next Mod 2 = 0 Then
                                If Char.ToUpper(word(j)) = "I" Then
                                    appendValue = "di"
                                Else
                                    appendValue = "bo"
                                End If
                            End If
                            output &= appendValue
                            replaced = True
                        End If
                        wasVowel = True
                    Else
                        wasVowel = False
                    End If
                    If Not replaced Then output &= word(j)
                Next
            End If
            If capFirst Then
                output = output.Substring(0, 1).ToUpper & output.Substring(1).ToLower
            End If
            Return output
        End Function
        Public Overrides ReadOnly Property RenderTag() As String
            Get
                Return "encode"
            End Get
        End Property
    End Class
End Namespace