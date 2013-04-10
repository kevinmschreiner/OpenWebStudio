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
    Public Class RenderOpenControl
        Inherits RenderBase

        Public Overrides ReadOnly Property RenderTag() As String
            Get
                Return "OPENCONTROL"
            End Get
        End Property

        Public Overrides ReadOnly Property RenderType() As RenderTypes
            Get
                Return RenderTypes.Functional
            End Get
        End Property

        Public Overrides Function Handle_Render(ByRef Caller As EngineBase, ByVal Index As Integer, ByRef Source As String, ByRef DS As System.Data.DataSet, ByRef DR As System.Data.DataRow, ByRef RuntimeMessages As System.Collections.Generic.SortedList(Of String, String), ByVal NullReturn As Boolean, ByVal NullOverride As Boolean, ByVal ProtectSession As Boolean, ByVal SessionDelimiter As String, ByVal useSessionQuotes As Boolean, ByVal useAggregations As Boolean, ByRef FilterText As String, ByRef FilterField As String, ByRef Debugger As r2i.OWS.Framework.Debugger) As Boolean
            Dim REPLACED As Boolean = False
            Dim parameters As String() = ParameterizeString(Source, ","c, """"c, "\"c)
            If Not parameters Is Nothing AndAlso parameters.Length > 2 Then
                Dim idnameparameter As String = parameters(1)
                Dim srctype As String = parameters(2)
                Dim parameterID As Integer = 3
                Dim configId As String = Nothing
                Dim resourcekey As String = Nothing
                Dim controltype As String = "div"
                Dim resourcefile As String = Nothing
                Select Case srctype.ToUpper
                    Case "CONFIGURATIONID"
                        configId = parameters(parameterID)
                        parameterID += 1
                    Case "RESOURCEFILE"
                        resourcefile = parameters(parameterID)
                        parameterID += 1
                        resourcekey = parameters(parameterID)
                        parameterID += 1
                    Case "RESOURCEKEY"
                        resourcekey = parameters(parameterID)
                        parameterID += 1
                End Select
                If parameters.Length > parameterID Then
                    controltype = parameters(parameterID)
                End If

                '<MOD:3>
                If (Not configId Is Nothing AndAlso configId.Length > 0) OrElse (Not resourcekey Is Nothing AndAlso resourcekey.Length > 0) Then
                    'Dim sR As New IO.StreamReader(System.Reflection.Assembly.GetExecutingAssembly.GetManifestResourceStream("ChildControls.FTB.Data.Text"))
                    'Source = sR.ReadToEnd()
                    'Source = Source.Replace("[ListX.ChildControls.TextEditor,ID]", idnameparameter)
                    'Source = Source.Replace("[ListX.ChildControls.TextEditor,CONTENT]", contentparameter)
                    'REPLACED = True
                    'MERGE START
                    Source = AbstractFactory.Instance.EngineController.GetOpenControlBase(Caller.Caller.Page, idnameparameter, idnameparameter, Caller.TabID, configId, resourcefile, resourcekey, idnameparameter, Caller.ModulePath, Caller.ModulePath, controltype)
                    'MERGE STOP
                    REPLACED = True
                End If
                '</MOD:3>
            End If
            Return REPLACED
        End Function
    End Class
End Namespace