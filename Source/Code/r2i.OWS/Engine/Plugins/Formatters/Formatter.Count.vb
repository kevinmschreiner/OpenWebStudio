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
Imports System.Collections.Generic
Imports r2i.OWS.Framework.Entities
Imports r2i.OWS.Framework, r2i.OWS.Framework.Utilities, r2i.OWS.Framework.Plugins.Formatters

Namespace r2i.OWS.Formatters
    Public Class Count : Inherits FormatterBase
        Public Overrides Function Handle_Render(ByRef Caller As EngineBase, ByVal Index As Integer, ByRef Value As String, ByRef Formatter As String, ByRef Source As String, ByRef DS As System.Data.DataSet, ByRef DR As System.Data.DataRow, ByRef RuntimeMessages As System.Collections.Generic.SortedList(Of String, String), ByVal NullReturn As Boolean, ByVal NullOverride As Boolean, ByVal ProtectSession As Boolean, ByVal SessionDelimiter As String, ByVal useSessionQuotes As Boolean, ByVal useAggregations As Boolean, ByRef FilterText As String, ByRef FilterField As String, ByRef Debugger As Framework.Debugger) As Boolean
            If Not DS Is Nothing AndAlso Not DS.Tables Is Nothing AndAlso DS.Tables.Count > 0 AndAlso DS.Tables(0).Rows.Count > 0 Then
                Dim pos As Integer = Formatter.IndexOf(":")
                Dim fparameter As String = Formatter.Substring(pos + 1).TrimEnd(New Char() {"}"c})
                Select Case True
                    Case Formatter.ToUpper.StartsWith("{COUNT:")
                        Dim cnt As Integer = 0
                        Try
                            If fparameter.Length > 0 Then
                                If Not Microsoft.VisualBasic.IsNumeric(Value) Then
                                    Value = "'" & Value & "'"
                                End If
                                cnt = DS.Tables(0).Select(fparameter & "=" & Value).Length
                            End If
                        Catch ex As Exception
                        End Try
                        Source = cnt
                        Return True
                    Case Formatter.ToUpper.StartsWith("{COUNT.REMAINING:")
                        Dim cnt As Integer = 0
                        Try
                            If fparameter.Length > 0 Then
                                If Index < DS.Tables(0).Rows.Count Then
                                    Dim bstop As Boolean = False
                                    Dim i As Integer = Index + 1
                                    While i < DS.Tables(0).Rows.Count AndAlso Not bstop
                                        If Not DS.Tables(0).Rows(i).Item(fparameter) = Value Then
                                            bstop = True
                                        Else
                                            cnt += 1
                                        End If
                                        i += 1
                                    End While
                                End If
                            End If
                        Catch ex As Exception
                        End Try
                        Source = cnt
                        Return True
                    Case Formatter.ToUpper.StartsWith("{COUNT.PRECEDING:")
                        Dim cnt As Integer = 0
                        Try
                            If fparameter.Length > 0 Then
                                If Index > 0 Then
                                    Dim bstop As Boolean = False
                                    Dim i As Integer = Index - 1
                                    While i > 0 AndAlso Not bstop
                                        If Not DS.Tables(0).Rows(i).Item(fparameter) = Value Then
                                            bstop = True
                                        Else
                                            cnt += 1
                                        End If
                                        i -= 1
                                    End While
                                End If
                            End If
                        Catch ex As Exception
                        End Try
                        Source = cnt
                        Return True
                End Select
            End If
            Return False
        End Function
        Public Overrides ReadOnly Property RenderTags() As String()
            Get
                Static str As String() = New String() {"count", "count.remaining", "count.preceding"}
                Return str
            End Get
        End Property

        Public Overrides ReadOnly Property RenderTag() As String
            Get
                Return "count"
            End Get
        End Property
    End Class
End Namespace