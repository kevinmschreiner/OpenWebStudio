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
Imports r2i.OWS.Framework.Utilities.Compatibility, r2i.OWS.Framework.Utilities, r2i.OWS.Framework.Plugins.Renderers

Namespace r2i.OWS.Renderers
    Public Class RenderRadio
        Inherits RenderBase

        Public Overrides ReadOnly Property RenderTag() As String
            Get
                Return "RADIO"
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
            If Not parameters Is Nothing AndAlso parameters.Length >= 4 Then
                'Try
                Dim skipped As Boolean = False
                Dim radioname As String = parameters(1)
                Dim radiovalue As String = parameters(2)
                Dim rSessionValue As String = parameters(3)
                Dim checkdefault As String = ""
                If parameters.Length > 4 Then
                    checkdefault = parameters(4)
                End If
                Caller.Session.Item("xLm" & Caller.ModuleID & "s" & radioname) = rSessionValue

                'If radiovalue.IndexOf("[") > 0 OrElse checkdefault.IndexOf("[") > 0 Then
                '    skipped = True
                'End If

                'We cannot handle this until the value of the column in the session has already been identified
                If Not skipped Then
                    'ROMAIN: Generic replacement - 08/21/2007
                    'Dim CheckedItems As ArrayList = Me.CheckedItems(Me.ModuleID, radioname, Session)
                    'Dim uncheckeditems As ArrayList = Me.UnCheckedItems(Me.ModuleID, radioname, Session)
                    Dim CheckedItems As List(Of String) = Utilities.Utility.CheckedItems(Caller.ModuleID, radioname, Caller.Session)
                    Dim UnCheckedItems As List(Of String) = Utilities.Utility.UnCheckedItems(Caller.ModuleID, radioname, Caller.Session)
                    Dim ischecked As Boolean = False
                    'Determine if item is to be selected by default
                    If checkdefault.Length > 0 AndAlso (checkdefault.ToUpper = "TRUE" OrElse checkdefault = "1" OrElse checkdefault = "-1") Then
                        ischecked = True
                    End If

                    'Determine if the item has been checked within the session
                    If Not CheckedItems Is Nothing Then
                        If CheckedItems.Contains(radiovalue) Then
                            ischecked = True
                        End If
                    End If

                    'Determine if the item has been unchecked within the session
                    If Not UnCheckedItems Is Nothing Then
                        If UnCheckedItems.Contains(radiovalue) Then
                            ischecked = False
                        End If
                    End If

                    Source = "<input runat=server type=radio id=" & radioname & "|!HEADERINDEX!|i" & Index & " name=""" & radioname & """ "
                    If ischecked Then
                        Source &= " checked "
                    End If
                    'Source &= " onClick=""m" & Me.ModuleID & "_rc('" & radioname & "','" & radiovalue & "',this);"" >"
                    Source &= " onClick=""ows.onUncheck(" & Caller.ModuleID & ",'" & radioname & "','" & radiovalue & "',this);"" >"
                    REPLACED = True
                End If

                'Catch ex As Exception
                '    fvalue = ""
                'End Try
            End If
            Return REPLACED
        End Function
    End Class
End Namespace