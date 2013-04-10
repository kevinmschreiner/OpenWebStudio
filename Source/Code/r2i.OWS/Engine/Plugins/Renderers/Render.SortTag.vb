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
Imports System.Collections.Generic
Imports r2i.OWS.Framework.Plugins.Renderers
Imports r2i.OWS.Framework.Utilities.Compatibility

Namespace r2i.OWS.Renderers
    Public Class RenderSortTag
        Inherits RenderBase

        Public Overrides ReadOnly Property RenderTag() As String
            Get
                Return "SORTTAG"
            End Get
        End Property

        Public Overrides ReadOnly Property RenderType() As RenderTypes
            Get
                Return RenderTypes.Variable
            End Get
        End Property

        Public Overrides Function Handle_Render(ByRef Caller as EngineBase, ByVal Index As Integer, ByRef Source As String, ByRef DS As System.Data.DataSet, ByRef DR As System.Data.DataRow, ByRef RuntimeMessages As System.Collections.Generic.SortedList(Of String, String), ByVal NullReturn As Boolean, ByVal NullOverride As Boolean, ByVal ProtectSession As Boolean, ByVal SessionDelimiter As String, ByVal useSessionQuotes As Boolean, ByVal useAggregations As Boolean, ByRef FilterText As String, ByRef FilterField As String, ByRef Debugger As r2i.OWS.Framework.Debugger) As Boolean
            'Build the Sort Query Tag [SORTTAG]
            Dim parameters As String() = ParameterizeString(Source, ","c, """"c, "\"c)
            Dim REPLACED As Boolean = False
            Dim isEmpty As Boolean = False
            Dim sqlSortStr As String = ""

            Dim sortActionList As List(Of SortAction) = Framework.Utilities.Utility.SortStatus(Caller.Session, Caller.ConfigurationID.ToString.Replace("{", "").Replace("}", "").Replace("-", ""), Caller.ModuleID, Caller.UserID)
            If Not sortActionList Is Nothing AndAlso sortActionList.Count > 0 Then
                'ROMAIN: Generic replacement - 08/20/2007
                'Try
                '    Dim sList As New ArrayList
                '    Dim sSorting As New ArrayList
                '    sList = sortActionList.Clone
                '    Dim sEnumerator As IEnumerator = sList.GetEnumerator
                '    While sEnumerator.MoveNext
                '        Dim sa As SortAction = CType(sEnumerator.Current, SortAction)
                '        If Not sa Is Nothing AndAlso Not sa.SortDirection Is Nothing AndAlso (sa.SortDirection.ToUpper = "ASC" OrElse sa.SortDirection.ToUpper = "DESC") Then
                '            If Not sSorting.Contains(sa.ColumnName) Then    'Issue with doubling-up on columns
                '                sSorting.Add(sa.ColumnName)
                '                sqlSortStr += sa.ColumnName + " " + sa.SortDirection + ", "
                '            End If
                '        End If
                '    End While

                '    Source = sqlSortStr.Trim.TrimEnd(",")
                '    REPLACED = True
                Try
                    Dim sList As New List(Of SortAction)
                    Dim sSorting As New List(Of String)
                    sList = sortActionList.GetRange(0, sortActionList.Count)
                    Dim sEnumerator As IEnumerator(Of SortAction) = sList.GetEnumerator
                    While sEnumerator.MoveNext
                        Dim sa As SortAction = sEnumerator.Current
                        If Not sa Is Nothing AndAlso Not sa.SortDirection Is Nothing AndAlso (sa.SortDirection.ToUpper = "ASC" OrElse sa.SortDirection.ToUpper = "DESC") Then
                            If Not sSorting.Contains(sa.ColumnName) Then    'Issue with doubling-up on columns
                                sSorting.Add(sa.ColumnName)
                                sqlSortStr += sa.ColumnName + " " + sa.SortDirection + ", "
                            End If
                        End If
                    End While

                    Source = sqlSortStr.Trim.TrimEnd(",")
                    REPLACED = True
                Catch ex As Exception
                    'ROMAIN 09/19/07
                    'TODO: Change Exceptions
                    'DotNetNuke.Services.Exceptions.LogException(ex)
                End Try
            Else
                isEmpty = True
                REPLACED = True
            End If
            If sqlSortStr Is Nothing OrElse sqlSortStr.Length = 0 Then
                isEmpty = True
                REPLACED = True
            End If
            If isEmpty Then
                If Not parameters Is Nothing Then
                    If parameters.Length = 1 Then
                        '[SORTTAG]
                        Source = "1"
                    Else
                        '[SORTTAG,DEFAULT]
                        'Version: 1.9.7 - SortTag provides Default value (rather than 1) when a default sort is required and cannot be an index value (SQL 2005)
                        Source = parameters(1)
                    End If
                End If
            End If
            Return REPLACED
        End Function
    End Class
End Namespace