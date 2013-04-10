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
Namespace r2i.OWS.Framework.Plugins.Queries
    Public MustInherit Class QueryBase
        Implements iPlugin

        Public MustOverride ReadOnly Property QueryTag() As String
        Public MustOverride ReadOnly Property QueryStructure() As String

        Public MustOverride Function Handle_GetData( _
            ByRef Caller As EngineBase, _
            ByVal isSubQuery As Boolean, _
            ByVal Query As String, _
            ByVal FilterField As String, _
            ByVal FilterText As String, _
            ByRef DebugWriter As r2i.OWS.Framework.Debugger, _
            ByVal isRendered As Boolean, _
            Optional ByVal timeout As Integer = -1, _
            Optional ByVal CustomConnection As String = Nothing) As OWS.Framework.RuntimeBase.QueryResult

        Protected Class MiniRenderer
            Private _Ds As DataSet
            Private _Caller As EngineBase
            Private _Messages As System.Collections.Generic.SortedList(Of String, String)
            Private _UseAggregations As Boolean
            Private _isPreRender As Boolean
            Private _NullReturn As Boolean
            Private _ProtectSession As Boolean
            Private _SessionDelimiter As String
            Private _UseSessionQuotes As Boolean
            Private _FilterField As String
            Private _FilterText As String
            Private _DebugWriter As r2i.OWS.Framework.Debugger
            Private _dr As DataRow

            Public Sub New(ByRef Caller As EngineBase, ByRef DS As DataSet, ByRef Messages As System.Collections.Generic.SortedList(Of String, String), ByVal useAggregations As Boolean, ByVal isPreRender As Boolean, Optional ByVal NullReturn As Boolean = True, Optional ByVal ProtectSession As Boolean = False, Optional ByVal SessionDelimiter As String = ",", Optional ByVal useSessionQuotes As Boolean = True, Optional ByRef FilterText As String = Nothing, Optional ByRef FilterField As String = Nothing, Optional ByRef DebugWriter As r2i.OWS.Framework.Debugger = Nothing, Optional ByVal Row As DataRow = Nothing)
                _Caller = Caller
                _Ds = DS
                _Messages = Messages
                _UseAggregations = useAggregations
                _isPreRender = isPreRender
                _NullReturn = NullReturn
                _ProtectSession = ProtectSession
                _SessionDelimiter = SessionDelimiter
                _UseSessionQuotes = useSessionQuotes
                _FilterField = FilterField
                _FilterText = FilterText
                _DebugWriter = DebugWriter
                _dr = Row
            End Sub

            Public Function RenderString(ByVal Value As String) As String
                Return _Caller.RenderString(Me._Ds, Value, Me._Messages, Me._UseAggregations, Me._isPreRender, Me._NullReturn, Me._ProtectSession, Me._SessionDelimiter, Me._UseSessionQuotes, Me._FilterText, Me._FilterField, Me._DebugWriter, Me._dr)
            End Function
        End Class

        Public ReadOnly Property Plugin() As PluginTag Implements iPlugin.Plugin
            Get
                Return PluginTag.Create(Config.Section.Queryies.ToString.ToLower, "", QueryTag)
            End Get
        End Property
    End Class

End Namespace