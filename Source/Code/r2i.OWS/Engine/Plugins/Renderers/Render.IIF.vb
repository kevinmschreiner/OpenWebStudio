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

Namespace r2i.OWS.Renderers
    Public Class RenderIIF
        Inherits RenderBase

        Public Overrides ReadOnly Property RenderTag() As String
            Get
                Return "IIF"
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

            If Not parameters Is Nothing AndAlso parameters.Length = 4 Then
                Dim sCondition As String = parameters(1)
                Dim sIfTrue As String = parameters(2)
                Dim sIfFalse As String = parameters(3)
                Dim cOp As Char
                Dim sOp As String = ""
                Dim idx As Integer
                Dim sLeft As String
                Dim sRight As String
                Dim rm As New RenderMath()

                'TODO: Add an Override for the Conditions
                If Caller.xls.enableCompoundIIFConditions = False Then
                    ExtractOperator(sCondition, "\"c, sOp, idx)
                    If sOp <> "" Then
                        cOp = sOp.Substring(0, 1)
                        Dim sConditions() As String = ParameterizeString(sCondition, cOp, """"c, "\"c)
                        sLeft = sConditions(0).Trim
                        sRight = sConditions(1).Substring(sOp.Length - 1).Trim
                    Else
                        sLeft = sCondition.Trim
                        sOp = "="
                        sRight = "True"
                    End If
                    If rm.EvaluateCondition(Caller, sLeft, sOp, sRight, Debugger) Then
                        Source = sIfTrue
                    Else
                        Source = sIfFalse
                    End If
                Else
                    'Caller.RenderMath(sCondition)
                    rm.RenderMath(sCondition)
                    If sCondition.ToUpper = "TRUE" Then
                        Source = sIfTrue
                    Else
                        Source = sIfFalse
                    End If
                End If

                'StripEscapes(Source)
                REPLACED = True
            End If

            Return REPLACED
        End Function

        Private Sub ExtractOperator(ByVal Condition As String, ByVal Escape As Char, ByRef [Operator] As String, ByRef Index As Integer)
            Dim idx As Integer
            Dim s As String

            For idx = 1 To Condition.Length - 1
                s = Condition.Substring(idx, 1)
                If s = "!" Or s = "=" Or s = "<" Or s = ">" Then
                    If Condition.Substring(idx - 1, 1) <> Escape Then
                        [Operator] = s
                        Index = idx
                        If idx < (Condition.Length - 1) Then
                            Dim s2 As String = Condition.Substring(idx + 1, 1)
                            If s2 = "!" Or s2 = "=" Or s2 = "<" Or s2 = ">" Then
                                [Operator] &= s2
                            End If
                        End If
                        Exit For
                    End If
                End If
            Next
        End Sub
    End Class
End Namespace