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
    Public MustInherit Class FormatterBase
        Inherits ParserBase
        Implements iPlugin

        Public Overridable Function Handle_Render( _
            ByRef Caller As EngineBase, _
            ByVal Index As Integer, _
            ByRef Source As String, _
            ByRef DS As DataSet, _
            ByRef DR As DataRow, _
            ByRef RuntimeMessages As SortedList(Of String, String), _
            ByVal NullReturn As Boolean, _
            ByVal NullOverride As Boolean, _
            ByVal ProtectSession As Boolean, _
            ByVal SessionDelimiter As String, _
            ByVal useSessionQuotes As Boolean, _
            ByVal useAggregations As Boolean, _
            ByRef FilterText As String, _
            ByRef FilterField As String, _
            ByRef Debugger As r2i.OWS.Framework.Debugger) As Boolean
            Return True
        End Function
        Public Overridable Function Handle_Render( _
            ByRef Caller As EngineBase, _
            ByVal Index As Integer, _
            ByRef Value As String, _
            ByRef Parameters As String, _
            ByRef Source As String, _
            ByRef DS As DataSet, _
            ByRef DR As DataRow, _
            ByRef RuntimeMessages As SortedList(Of String, String), _
            ByVal NullReturn As Boolean, _
            ByVal NullOverride As Boolean, _
            ByVal ProtectSession As Boolean, _
            ByVal SessionDelimiter As String, _
            ByVal useSessionQuotes As Boolean, _
            ByVal useAggregations As Boolean, _
            ByRef FilterText As String, _
            ByRef FilterField As String, _
            ByRef Debugger As r2i.OWS.Framework.Debugger) As Boolean
            Return True
        End Function

        Public MustOverride ReadOnly Property RenderTag() As String
        Public Overridable ReadOnly Property RenderTags() As String()
            Get
                Return Nothing
            End Get
        End Property
        Public Overridable ReadOnly Property StartTag() As String
            Get
                Return "{" & RenderTag
            End Get
        End Property

        Public Overridable ReadOnly Property Plugin() As PluginTag Implements iPlugin.Plugin
            Get
                If Not RenderTags Is Nothing Then
                    Return PluginTag.Create(Config.Section.Formats.ToString.ToLower, "", RenderTags)
                Else
                    Return PluginTag.Create(Config.Section.Formats.ToString.ToLower, "", RenderTag)
                End If
            End Get
        End Property
    End Class
End Namespace