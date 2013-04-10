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
Imports System.Collections.Generic
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Diagnostics
Imports r2i.OWS.Framework.Plugins.Renderers
Imports r2i.OWS.Framework
Imports r2i.OWS.Framework.Utilities

Namespace r2i.OWS.Renderers
    Public Class Pager
        Inherits RenderBase

        Public Overrides ReadOnly Property RenderTag() As String
            Get
                Return "PAGER"
            End Get
        End Property

        Public Overrides ReadOnly Property RenderType() As RenderTypes
            Get
                Return RenderTypes.Functional
            End Get
        End Property

        Private Function RenderValue(ByRef Value As String, ByRef Caller As EngineBase, ByVal RuntimeMessages As SortedList(Of String, String)) As String
            Return Caller.RenderString(Nothing, Value, RuntimeMessages, True, False)
        End Function
        Public Overrides Function Handle_Render(ByRef Caller As EngineBase, ByVal Index As Integer, ByRef Source As String, ByRef SharedDS As System.Data.DataSet, ByRef DR As System.Data.DataRow, ByRef RuntimeMessages As System.Collections.Generic.SortedList(Of String, String), ByVal NullReturn As Boolean, ByVal NullOverride As Boolean, ByVal ProtectSession As Boolean, ByVal SessionDelimiter As String, ByVal useSessionQuotes As Boolean, ByVal useAggregations As Boolean, ByRef FilterText As String, ByRef FilterField As String, ByRef Debugger As r2i.OWS.Framework.Debugger) As Boolean
            Dim REPLACED As Boolean = False
            Dim parameters As String() = ParameterizeString(Source, ","c, """"c, "\"c)

            If Not parameters Is Nothing AndAlso parameters.Length > 1 Then
                ''    '{PAGER, Current="", Pages="", Format="", CurrentFormat="", LeftFormat="", DisabledLeftFormat="", RightFormat="", DisabledRightFormat="", EmptyFormat="", Delimiter="", PageDelimiter=""}
                Dim sbvalue As New System.Text.StringBuilder
                Try
                    Dim Current As Integer = 0
                    Dim Pages As Integer = 0
                    Dim Show As Integer = 0
                    Dim Format As String = ""
                    Dim CurrentFormat As String = ""
                    Dim DelimiterFormat As String = ""
                    Dim LeftFormat As String = ""
                    Dim DisabledLeftFormat As String = ""
                    Dim RightFormat As String = ""
                    Dim DisabledRightFormat As String = ""
                    Dim EmptyFormat As String = ""
                    Dim PageDelimiterFormat As String = ""
                    Dim usesPageDelimiter As Boolean = False
                    Dim str As String = Nothing
                    Dim i As Integer = 0
                    For Each str In parameters
                        Select Case True
                            Case str.ToUpper.StartsWith("CURRENT=")
                                Current = CInt(RenderValue(str.Remove(0, 9).TrimEnd(New Char() {""""c}), Caller, RuntimeMessages))
                            Case str.ToUpper.StartsWith("PAGES=")
                                Pages = CInt(RenderValue(str.Remove(0, 7).TrimEnd(New Char() {""""c}), Caller, RuntimeMessages))
                            Case str.ToUpper.StartsWith("SHOW=")
                                Show = CInt(RenderValue(str.Remove(0, 6).TrimEnd(New Char() {""""c}), Caller, RuntimeMessages))
                            Case str.ToUpper.StartsWith("FORMAT=")
                                Format = RenderValue(str.Remove(0, 8).TrimEnd(New Char() {""""c}), Caller, RuntimeMessages)
                            Case str.ToUpper.StartsWith("PAGEDELIMITER=")
                                PageDelimiterFormat = RenderValue(str.Remove(0, 15).TrimEnd(New Char() {""""c}), Caller, RuntimeMessages)
                                usesPageDelimiter = True
                            Case str.ToUpper.StartsWith("DELIMITER=")
                                DelimiterFormat = RenderValue(str.Remove(0, 11).TrimEnd(New Char() {""""c}), Caller, RuntimeMessages)
                            Case str.ToUpper.StartsWith("CURRENTFORMAT=")
                                CurrentFormat = RenderValue(str.Remove(0, 15).TrimEnd(New Char() {""""c}), Caller, RuntimeMessages)
                            Case str.ToUpper.StartsWith("LEFTFORMAT=")
                                LeftFormat = RenderValue(str.Remove(0, 12).TrimEnd(New Char() {""""c}), Caller, RuntimeMessages)
                            Case str.ToUpper.StartsWith("DISABLEDLEFTFORMAT=")
                                DisabledLeftFormat = RenderValue(str.Remove(0, 20).TrimEnd(New Char() {""""c}), Caller, RuntimeMessages)
                            Case str.ToUpper.StartsWith("RIGHTFORMAT=")
                                RightFormat = RenderValue(str.Remove(0, 13).TrimEnd(New Char() {""""c}), Caller, RuntimeMessages)
                            Case str.ToUpper.StartsWith("DISABLEDRIGHTFORMAT=")
                                DisabledRightFormat = RenderValue(str.Remove(0, 21).TrimEnd(New Char() {""""c}), Caller, RuntimeMessages)
                            Case str.ToUpper.StartsWith("EMPTYFORMAT=")
                                EmptyFormat = RenderValue(str.Remove(0, 13).TrimEnd(New Char() {""""c}), Caller, RuntimeMessages)
                        End Select
                    Next

                    'EXIT IMMEDIATELY IF THIS IS NOT YET DEFINED, AND DOES NOT PROVIDE THE DEFINITION
                    If Current <= Pages AndAlso Current >= 1 AndAlso Pages > 1 Then
                        Dim startIndex As Integer = 1
                        Dim endIndex As Integer = Pages
                        If Show > 0 Then
                            Show -= 1 'REMOVE THE CURRENT PAGE FROM THE LIST
                            Dim lDistance As Integer = Show / 2
                            Dim rDistance As Integer = Show / 2
                            If lDistance * 2 < Show Then
                                rDistance += 1
                            End If
                            startIndex = Current - (lDistance)
                            endIndex = Current + (rDistance)
                            If startIndex < 1 Then
                                endIndex = endIndex + (startIndex * -1)
                                startIndex = 1
                            End If
                            If endIndex > Pages Then
                                endIndex = Pages
                            End If
                        End If


                        'LEFT
                        If Current > 1 Then
                            'ENABLED
                            sbvalue.Append(LeftFormat.Replace("#PAGE#", (Current - 1).ToString))
                        Else
                            'DISABLED
                            sbvalue.Append(DisabledLeftFormat)
                        End If
                        sbvalue.Append(DelimiterFormat)

                        For i = startIndex To endIndex
                            'DETAIL
                            If i = Current Then
                                'CURRENT FORMAT
                                sbvalue.Append(CurrentFormat.Replace("#PAGE#", i.ToString))
                            Else
                                'FORMAT
                                sbvalue.Append(Format.Replace("#PAGE#", i.ToString))
                            End If
                            If i < endIndex Then
                                If usesPageDelimiter Then
                                    sbvalue.Append(PageDelimiterFormat)
                                Else
                                    sbvalue.Append(DelimiterFormat)
                                End If
                            End If
                        Next

                        'RIGHT
                        sbvalue.Append(DelimiterFormat)
                        If Current < Pages Then
                            'ENABLED
                            sbvalue.Append(RightFormat.Replace("#PAGE#", (Current + 1).ToString))
                        Else
                            'DISABLED
                            sbvalue.Append(DisabledRightFormat)
                        End If
                    Else
                        'EMPTY
                        sbvalue.Append(EmptyFormat)
                    End If
                Catch ex As Exception
                Debugger.AppendLine("Failed to generate the pager output: " & ex.ToString)
            End Try

                REPLACED = True
                Source = sbvalue.ToString
            End If
            Return REPLACED
        End Function
    End Class
End Namespace