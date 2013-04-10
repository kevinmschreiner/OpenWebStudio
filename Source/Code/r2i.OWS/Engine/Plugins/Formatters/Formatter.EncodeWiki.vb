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
    Public Class EncodeWiki : Inherits FormatterBase

        Public Overrides Function Handle_Render(ByRef Caller as EngineBase, ByVal Index As Integer, ByRef Value As String, ByRef Formatter As String, ByRef Source As String, ByRef DS As System.Data.DataSet, ByRef DR As System.Data.DataRow, ByRef RuntimeMessages As System.Collections.Generic.SortedList(Of String, String), ByVal NullReturn As Boolean, ByVal NullOverride As Boolean, ByVal ProtectSession As Boolean, ByVal SessionDelimiter As String, ByVal useSessionQuotes As Boolean, ByVal useAggregations As Boolean, ByRef FilterText As String, ByRef FilterField As String, ByRef Debugger As Framework.Debugger) As Boolean
            'VERSION 2.0.5 - Added {ENCODEWIKI}
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
            Return True
        End Function

        Public Overrides ReadOnly Property RenderTag() As String
            Get
                Return "encodewiki"
            End Get
        End Property
    End Class
End Namespace