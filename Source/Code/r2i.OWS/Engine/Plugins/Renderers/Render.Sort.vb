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
Imports r2i.OWS.Framework.Utilities
Imports r2i.OWS.Framework.Plugins.Renderers
Imports r2i.OWS.Framework.Utilities.Compatibility, r2i.OWS.Actions

Namespace r2i.OWS.Renderers
    Public Class RenderSort
        Inherits RenderBase

        Public Overrides ReadOnly Property RenderTag() As String
            Get
                Return "SORT"
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
            If Not parameters Is Nothing AndAlso parameters.Length >= 7 Then
                Dim sa As SortAction
                sa = New SortAction(parameters)

                Dim sac As SortAction = FindSortAction(Caller, sa)

                If sac Is Nothing Then
                    'Dim ar As ArrayList = sortActionList
                    'If ar Is Nothing Then ar = New ArrayList
                    'ar.Add(sa)
                    'ar.Sort(New SortAction)
                    'sortActionList = ar
                    'Dim ar As List(Of SortAction) = sortActionList
                    Dim ar As List(Of SortAction) = Framework.Utilities.Utility.SortStatus(Caller.Session, Caller.ConfigurationID.ToString.Replace("{", "").Replace("}", "").Replace("-", ""), Caller.ModuleID, Caller.UserID)
                    If ar Is Nothing Then ar = New List(Of SortAction)
                    ar.Add(sa)
                    ar.Sort()
                    'sortActionList = ar
                    Framework.Utilities.Utility.SortStatus(Caller.Session, Caller.ConfigurationID.ToString.Replace("{", "").Replace("}", "").Replace("-", ""), Caller.ModuleID, Caller.UserID) = ar
                Else
                    sa = sac
                End If
                If parameters.Length > 7 Then
                    sa.SortQuery = parameters(7)
                End If
                If parameters.Length > 8 Then
                    sa.SortTarget = parameters(8)
                End If
                Dim pber As String
                If Caller.xls.enableAJAX OrElse Caller.xls.enableAJAXPaging Then
                    If Not sa.SortQuery Is Nothing Then
                        If Not sa.SortTarget Is Nothing Then
                            pber = "ows.Sort('" & Caller.ModuleID & "',null," & sa.SortOrder.ToString & ",'" & sa.SortQuery & "','" & sa.SortTarget & "');"
                        Else
                            pber = "ows.Sort('" & Caller.ModuleID & "',null," & sa.SortOrder.ToString & ",'" & sa.SortQuery & "');"
                        End If
                    Else
                        pber = "ows.Sort('" & Caller.ModuleID & "',null," & sa.SortOrder.ToString & ");"
                    End If
                Else
                    pber = Caller.GetPostBackEventReference("SORTCOMMAND," + sa.SortOrder.ToString)    'postback to the same tab
                End If

                Source = "href=""javascript:" & pber & """"
                REPLACED = True
            End If
            Return REPLACED
        End Function
        Private Function FindSortAction(ByVal Caller As Engine, ByVal source As SortAction) As SortAction
            Dim sac As SortAction = Nothing
            'ROMAIN: Generic replacement - 08/20/2007
            'If Not sortActionList Is Nothing Then

            '    Dim sList As New ArrayList
            '    sList = sortActionList.Clone
            '    Dim sEnumerator As IEnumerator = sList.GetEnumerator
            '    While sEnumerator.MoveNext
            '        Dim sa2 As SortAction = CType(sEnumerator.Current, SortAction)
            '        If sa2.SortOrder = source.SortOrder And source.ColumnName = sa2.ColumnName Then
            '            sac = sa2
            '            Exit While
            '        End If
            '    End While
            'End If
            Dim sortActionList As List(Of SortAction) = Framework.Utilities.Utility.SortStatus(Caller.Session, Caller.ConfigurationID.ToString.Replace("{", "").Replace("}", "").Replace("-", ""), Caller.ModuleID, Caller.UserID)
            If Not sortActionList Is Nothing AndAlso sortActionList.Count > 0 Then

                'Dim sList As New List(Of SortAction)
                'sList = sortActionList.GetRange(0, sortActionList.Count - 1)
                Dim sEnumerator As IEnumerator(Of SortAction) = sortActionList.GetEnumerator
                While sEnumerator.MoveNext
                    Dim sa2 As SortAction = sEnumerator.Current
                    If sa2.SortOrder = source.SortOrder And source.ColumnName = sa2.ColumnName Then
                        sac = sa2
                        Exit While
                    End If
                End While
            End If
            If Not sac Is Nothing Then
                Return sac
            Else
                Return Nothing
            End If
        End Function
    End Class
End Namespace