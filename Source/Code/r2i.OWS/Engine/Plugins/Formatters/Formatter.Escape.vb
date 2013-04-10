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
Imports r2i.OWS.Framework.Utilities
Imports r2i.OWS.Framework.Plugins.Formatters

Namespace r2i.OWS.Formatters
    Public Class Escape : Inherits FormatterBase

        Public Overrides Function Handle_Render(ByRef Caller as EngineBase, ByVal Index As Integer, ByRef Value As String, ByRef Formatter As String, ByRef Source As String, ByRef DS As System.Data.DataSet, ByRef DR As System.Data.DataRow, ByRef RuntimeMessages As System.Collections.Generic.SortedList(Of String, String), ByVal NullReturn As Boolean, ByVal NullOverride As Boolean, ByVal ProtectSession As Boolean, ByVal SessionDelimiter As String, ByVal useSessionQuotes As Boolean, ByVal useAggregations As Boolean, ByRef FilterText As String, ByRef FilterField As String, ByRef Debugger As Framework.Debugger) As Boolean
            Select Case True
                Case Formatter.ToUpper.StartsWith("{ESCAPE}")
                    'VERSION: 1.9.9 - Added ESCAPE
                    Try
                        Source = AddEscapes(Value)
                    Catch ex As Exception

                    End Try
                    Return True
                Case Formatter.ToUpper.StartsWith("{ESCAPE:")
                    Dim fParameters As String() = ParameterizeString(Formatter.Substring(7).TrimEnd(New Char() {"}"c}), ","c, """"c, "\"c)
                    Dim repeat As Integer = 1
                    If fParameters.Length = 1 Then
                        If Microsoft.VisualBasic.IsNumeric(fParameters(0)) Then
                            repeat = CInt(fParameters(0))
                            If repeat < 1 Then repeat = 1
                        End If
                    End If
                    Dim strEscape As String = New String("\"c, repeat)
                    Source = Value.Replace("{", strEscape & "{").Replace("}", strEscape & "}").Replace("[", strEscape & "[").Replace("]", strEscape & "]")
                    Return True
            End Select
            Return False
        End Function

        Public Overrides ReadOnly Property RenderTag() As String
            Get
                Return "escape"
            End Get
        End Property
    End Class
End Namespace