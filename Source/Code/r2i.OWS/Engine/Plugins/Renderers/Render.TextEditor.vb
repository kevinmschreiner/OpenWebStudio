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

Imports r2i.OWS.Framework.DataAccess
Imports r2i.OWS.Framework.Entities

Namespace r2i.OWS.Renderers
    Public Class RenderTextEditor
        Inherits RenderBase

        Public Overrides ReadOnly Property RenderTag() As String
            Get
                Return "TEXTEDITOR"
            End Get
        End Property

        Public Overrides ReadOnly Property RenderType() As RenderTypes
            Get
                Return RenderTypes.Functional
            End Get
        End Property

        Public Overrides Function Handle_Render(ByRef Caller as EngineBase, ByVal Index As Integer, ByRef Source As String, ByRef DS As System.Data.DataSet, ByRef DR As System.Data.DataRow, ByRef RuntimeMessages As System.Collections.Generic.SortedList(Of String, String), ByVal NullReturn As Boolean, ByVal NullOverride As Boolean, ByVal ProtectSession As Boolean, ByVal SessionDelimiter As String, ByVal useSessionQuotes As Boolean, ByVal useAggregations As Boolean, ByRef FilterText As String, ByRef FilterField As String, ByRef Debugger As r2i.OWS.Framework.Debugger) As Boolean
            Dim REPLACED As Boolean = False
            Dim parameters As String() = ParameterizeString(Source, ","c, """"c, "\"c)
            If Not parameters Is Nothing AndAlso parameters.Length > 2 Then
                Dim idnameparameter As String = parameters(1)
                Dim contentparameter As String = Nothing
                Dim width As String = "100%"
                Dim height As String = "375"
                If parameters.Length > 1 Then
                    contentparameter = parameters(2)
                    If parameters.Length > 3 Then
                        If parameters.Length >= 4 Then
                            'Name,Collection split.
                            If parameters(3).Length > 0 Then
                                contentparameter = parameters(2) & "," & parameters(3)
                            Else
                                contentparameter = parameters(2)
                            End If
                            If parameters.Length > 4 Then
                                width = parameters(4)
                                If parameters.Length > 5 Then
                                    height = parameters(5)
                                End If
                            End If

                            Dim rv As RenderBase = Plugins.Manager.GetPlugin(Plugins.PluginTag.Create(Config.Section.Tokens.ToString.ToLower, RenderTypes.Variable.ToString.ToLower, "")) 'Common.GetRenderer("", RenderTypes.Variable)
                            If Not rv.Handle_Render(Caller, Index, contentparameter, DS, DR, RuntimeMessages, NullReturn, NullReturn, ProtectSession, SessionDelimiter, useSessionQuotes, True, FilterText, FilterField, Debugger) Then
                                contentparameter = ParameterMerge(CType(parameters, Array), Source, 2)
                            End If
                        Else
                            contentparameter = ParameterMerge(CType(parameters, Array), Source, 2)
                        End If
                    End If
                End If
                '<MOD:3>
                If parameters.Length > 2 Then
                    'Dim sR As New IO.StreamReader(System.Reflection.Assembly.GetExecutingAssembly.GetManifestResourceStream("ChildControls.FTB.Data.Text"))
                    'Source = sR.ReadToEnd()
                    'Source = Source.Replace("[ListX.ChildControls.TextEditor,ID]", idnameparameter)
                    'Source = Source.Replace("[ListX.ChildControls.TextEditor,CONTENT]", contentparameter)
                    'REPLACED = True
                    'MERGE START
                    Source = AbstractFactory.Instance.EngineController.GetRichTextEditor(Caller.Caller.Page, idnameparameter, width, height, contentparameter)
                    'MERGE STOP
                    REPLACED = True
                End If
                '</MOD:3>
            End If
            Return REPLACED
        End Function
    End Class
End Namespace