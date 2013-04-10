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
Imports r2i.OWS.Framework.Plugins
Imports r2i.OWS.Framework.Plugins.Renderers
Imports r2i.OWS.Framework.Utilities

Namespace r2i.OWS.Renderers
    Public Class RenderCast
        Inherits RenderBase

        Public Overrides ReadOnly Property RenderTag() As String
            Get
                Return "CAST"
            End Get
        End Property

        Public Overrides ReadOnly Property RenderType() As RenderTypes
            Get
                Return RenderTypes.Variable
            End Get
        End Property

        Public Overrides ReadOnly Property CanPreRender() As Boolean
            Get
                Return True
            End Get
        End Property

        Public Overrides ReadOnly Property PreRenderTag() As String
            Get
                Return "#"
            End Get
        End Property

        Public Overrides Function Handle_Render(ByRef Caller As EngineBase, ByVal Index As Integer, ByRef Source As String, ByRef DS As System.Data.DataSet, ByRef DR As System.Data.DataRow, ByRef RuntimeMessages As System.Collections.Generic.SortedList(Of String, String), ByVal NullReturn As Boolean, ByVal NullOverride As Boolean, ByVal ProtectSession As Boolean, ByVal SessionDelimiter As String, ByVal useSessionQuotes As Boolean, ByVal useAggregations As Boolean, ByRef FilterText As String, ByRef FilterField As String, ByRef Debugger As r2i.OWS.Framework.Debugger) As Boolean
            Dim REPLACED As Boolean = False
            Dim VALUE As String = Nothing
            Dim variabletypeparameter As String = Nothing
            Dim VariableName As String = Nothing
            Dim parameters As String() = ParameterizeString(Source, ","c, """"c, "\"c)
            ' Cast,VariableName,variabletypeparameter,CastingMethod
            ' Initially, we support ToJson
            If Not parameters Is Nothing AndAlso parameters.Length = 4 Then
                VariableName = parameters(1)
                variabletypeparameter = parameters(2)
                Select Case parameters(3).ToLower
                    Case "tojson"
                        Dim o As Object = Nothing
                        Select Case variabletypeparameter.ToLower
                            Case "a", "action"
                                o = Caller.ActionVariable(VariableName)
                            Case "s", "session"
                                o = Caller.Session(VariableName)
                        End Select
                        If Not o Is Nothing Then
                            Source = Newtonsoft.Json.JavaScriptConvert.SerializeObject(o)
                            REPLACED = True
                        End If
                    Case "jsontoobject"
                        Dim o As Object = Nothing
                        Select Case variabletypeparameter.ToLower
                            Case "a", "action"
                                o = Caller.ActionVariable(VariableName)
                                Caller.ActionVariable(VariableName) = Newtonsoft.Json.JavaScriptConvert.DeserializeObject(o)
                                REPLACED = True
                            Case "s", "session"
                                o = Caller.Session(VariableName)
                                Caller.Session(VariableName) = Newtonsoft.Json.JavaScriptConvert.DeserializeObject(o)
                                REPLACED = True
                        End Select
                End Select
            End If
            Return REPLACED
        End Function
    End Class
End Namespace