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
Imports r2i.OWS.Framework.Utilities
Imports r2i.OWS.Framework
Imports r2i.OWS.Framework.Plugins.Renderers


Namespace r2i.OWS.Renderers
    Public Class RenderCheckItem
        Inherits RenderBase

        Public Overrides ReadOnly Property RenderTag() As String
            Get
                Return "CHECKITEM"
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
            If Not parameters Is Nothing AndAlso parameters.Length >= 3 Then
                Dim skipped As Boolean = False
                'Try
                Dim checkname As String = parameters(1)
                Dim checkvalue As String = parameters(2)
                'ADDED 
                Dim checkdefault As String = ""
                If parameters.Length > 3 Then
                    checkdefault = parameters(3)
                End If

                'If checkvalue.IndexOf("[") > 0 OrElse checkdefault.IndexOf("[") > 0 Then
                '    skipped = True
                'End If

                'We cannot handle this until the value of the column in the session has already been identified
                If Not skipped Then
                    Caller.Session.Item("xLm" & Caller.ModuleID & "s" & checkname) = checkname
                    'ROMAIN: Generic replacement - 08/21/2007
                    'Dim checkedItems As ArrayList = Me.CheckedItems(Me.ModuleID, checkname, Session)
                    'Dim uncheckedItems As ArrayList = Me.UnCheckedItems(Me.ModuleID, checkname, Session)
                    Dim checkedItems As List(Of String) = Utilities.Utility.CheckedItems(Caller.ModuleID, checkname, Caller.Session)
                    Dim uncheckedItems As List(Of String) = Utilities.Utility.UnCheckedItems(Caller.ModuleID, checkname, Caller.Session)
                    Dim ischecked As Boolean = False
                    'Determine if there is a default for this item
                    If checkdefault.Length > 0 AndAlso (checkdefault.ToUpper = "TRUE" OrElse checkdefault = "1" OrElse checkdefault = "-1") Then
                        ischecked = True
                    End If
                    'Determine if the item has been Checked within the session
                    If Not checkedItems Is Nothing Then
                        If checkedItems.Contains(checkvalue) Then
                            ischecked = True
                        End If
                    End If
                    'Determine if the item has been Unchecked within the session
                    If Not uncheckedItems Is Nothing Then
                        If uncheckedItems.Contains(checkvalue) Then
                            ischecked = False
                        End If
                    End If
                    Source = "<input runat=server type=checkbox id=" & checkname & "|!HEADERINDEX!|i" & Index & " name=""" & checkname & "|!HEADERINDEX!|i" & Index & """ "
                    If ischecked Then
                        Source &= " checked "
                    End If
                    'Source &= " onClick=""m" & Me.ModuleID & "_cc('" & checkname & "','" & checkvalue & "',this);"" >"
                    Source &= " onClick=""ows.onCheck('" & Caller.ModuleID & "','" & checkname & "','" & checkvalue & "',this);"" >"

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