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
Imports r2i.OWS.Framework.Utilities.Compatibility
Imports r2i.OWS.Framework.Plugins.Renderers

Namespace r2i.OWS.Renderers
    Public Class RenderSortHeader
        Inherits RenderBase

        Public Overrides ReadOnly Property RenderTag() As String
            Get
                Return "SORTHEADER"
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
            If Not parameters Is Nothing AndAlso parameters.Length >= 2 Then
                Dim SA As Utilities.Compatibility.SortAction = Nothing
                If IsNumeric(parameters(1)) Then
                    SA = FindSortAction(Caller, Convert.ToInt32(parameters(1)))
                End If
                If Not SA Is Nothing Then
                    Dim sortHeader As String
                    If SA.SortDirection Is Nothing Then
                        SA.SortDirection = ""
                    End If
                    Select Case SA.SortDirection.ToUpper
                        Case "ASC"
                            sortHeader = SA.AscendingText
                        Case "DESC"
                            sortHeader = SA.DescendingText
                        Case Else
                            sortHeader = SA.NormalText
                    End Select

                    Source = sortHeader
                    REPLACED = True

                    SA = Nothing
                End If
            End If
            Return REPLACED
        End Function

        Private Function FindSortAction(ByVal Caller As Engine, ByVal Index As Integer) As SortAction
            Dim sac As SortAction = Nothing
            'ROMAIN: Generic replacement - 08/20/2007
            'If Not sortActionList Is Nothing Then
            '    Dim sList As New ArrayList
            '    sList = sortActionList.Clone
            '    Dim sEnumerator As IEnumerator = sList.GetEnumerator
            '    While sEnumerator.MoveNext
            '        Dim sa2 As SortAction = CType(sEnumerator.Current, SortAction)
            '        If sa2.SortOrder = Index Then
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
                    If sa2.SortOrder = Index Then
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