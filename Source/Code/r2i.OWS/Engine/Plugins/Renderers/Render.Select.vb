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
Imports r2i.OWS.Framework.Plugins.Renderers
Imports r2i.OWS.Framework
Imports r2i.OWS.Framework.Utilities


Namespace r2i.OWS.Renderers
    Public Class RenderSelect
        Inherits RenderBase

        Public Overrides ReadOnly Property RenderTag() As String
            Get
                Return "SELECT"
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

            '{SELECT,left,
            '               value,
            '               (conditional),
            '               default,
            '                               result
            '               ...repeat...
            '}


            If Not parameters Is Nothing AndAlso parameters.Length > 4 AndAlso parameters.Length Mod 0 Then
                Dim leftportion As String = parameters(1)
                Dim i As Integer = 2
                Dim defaultValue As String = ""
                Dim isFound As Boolean = False
                For i = 2 To parameters.Length Step 2
                    If parameters(i).ToUpper = "DEFAULT" Then
                        defaultValue = parameters(i + 1)
                    Else
                        Dim sCondition As String = leftportion & parameters(i)
                        Dim rm As New RenderMath
                        rm.RenderMath(sCondition)
                        If sCondition.ToUpper = "TRUE" Then
                            Source = parameters(i + 1)
                            isFound = True
                            Exit For
                        End If
                    End If
                Next
                If Not isFound Then
                    Source = defaultValue
                End If
                REPLACED = True
            End If

            Return REPLACED
        End Function
    End Class
End Namespace