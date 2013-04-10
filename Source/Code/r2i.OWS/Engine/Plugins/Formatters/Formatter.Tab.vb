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
Imports System
Imports System.Collections
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Diagnostics
Imports System.Collections.Generic
Imports r2i.OWS.Framework.Entities
Imports r2i.OWS.Framework, r2i.OWS.Framework.Utilities, r2i.OWS.Framework.Plugins.Formatters

Namespace r2i.OWS.Formatters
    Public Class Tab : Inherits FormatterBase
        Public Overrides Function Handle_Render(ByRef Caller as EngineBase, ByVal Index As Integer, ByRef Value As String, ByRef Formatter As String, ByRef Source As String, ByRef DS As System.Data.DataSet, ByRef DR As System.Data.DataRow, ByRef RuntimeMessages As System.Collections.Generic.SortedList(Of String, String), ByVal NullReturn As Boolean, ByVal NullOverride As Boolean, ByVal ProtectSession As Boolean, ByVal SessionDelimiter As String, ByVal useSessionQuotes As Boolean, ByVal useAggregations As Boolean, ByRef FilterText As String, ByRef FilterField As String, ByRef Debugger As Framework.Debugger) As Boolean
            Select Case True
                Case Formatter.ToUpper = "{TABID}"
                    Dim tID As String = ConvertPageNameToPageID(Caller, Value)
                    If Microsoft.VisualBasic.IsNumeric(tID) AndAlso tID >= 0 Then
                        Source = tID
                        Return True
                    ElseIf Not Microsoft.VisualBasic.IsNumeric(tID) AndAlso tID <> "" Then
                        Source = tID
                        Return True
                    End If
                    'Version: 1.9.1 - Added TabID:Column lookup formatter
                Case Formatter.ToUpper.StartsWith("{TABID:")
                    Dim tiD As String = "-1"
                    Dim fParameters As String() = ParameterizeString(Formatter.Substring(7).TrimEnd(New Char() {"}"c}), ","c, """"c, "\"c)
                    If fParameters.Length = 1 Then
                        If Not Value Is Nothing Then
                            Dim columnName As String = fParameters(0)
                            If columnName.IndexOf("'") < 0 AndAlso columnName.IndexOf(" ") < 0 AndAlso columnName.IndexOf("(") < 0 AndAlso columnName.IndexOf(")") < 0 Then
                                'tiD = Dotnetnuke_TabLookupBy(Value, columnName)
                                'tiD = AbstractFactory.Instance.EngineController.Dotnetnuke_TabLookupBy(Value, columnName)
                                tiD = AbstractFactory.Instance.EngineController.PageLookupBy(Value, columnName)
                            End If
                        End If
                    End If
                    Source = tiD
                    Return True
                Case Formatter.ToUpper = "{TABNAME}"
                    'If IsNumeric(Value) Then
                    Dim tName As String = ConvertPageIDToPageName(Value)
                    If Not tName Is Nothing Then
                        Source = tName
                        Return True
                    End If
                    'End If
                Case Formatter.ToUpper = "{TABTITLE}"
                    Try

                        Dim pageTitle As String = AbstractFactory.Instance.EngineController.PageTitle(Value)
                        If Not pageTitle Is Nothing Then
                            Source = pageTitle
                            Return True
                        End If

                    Catch ex As Exception
                    End Try
                Case Formatter.ToUpper = "{TABDESCRIPTION}"
                    Try

                        Dim pageDescription As String = AbstractFactory.Instance.EngineController.PageDescription(Value)
                        If Not pageDescription Is Nothing Then
                            Source = pageDescription
                            Return True
                        End If
                    Catch ex As Exception
                    End Try
                    'VERSION: 1.9.9 - Formatter {MAPPATH}
            End Select
            Return False
        End Function
        Public Overrides ReadOnly Property RenderTags() As String()
            Get
                Static str As String() = New String() {"tab", "tabid", "tabname", "tabtitle", "tabdescription"}
                Return str
            End Get
        End Property

        Private pageConversion_PageID As SortedList(Of String, String)
        Private pageConversion_PageName As SortedList(Of String, String)

        Private Function ConvertPageIDToPageName(ByVal pageId As String) As String
            If pageConversion_PageID Is Nothing Then
                'ROMAIN: Generic replacement - 08/22/2007
                'tabConversion_TabID = New SortedList
                pageConversion_PageID = New SortedList(Of String, String)
            End If
            If Not pageConversion_PageID.ContainsKey(pageId) Then
                Try
                    Dim pageName As String = AbstractFactory.Instance.EngineController.PageName(pageId)
                    If Not pageName Is Nothing Then
                        pageConversion_PageID.Item(pageId) = pageName
                    End If
                    Return pageName
                Catch ex As Exception
                End Try
            Else
                Return pageConversion_PageID.Item(pageId)
            End If
            Return Nothing
        End Function
        Private Function ConvertPageNameToPageID(ByRef Caller As Engine, ByVal pageName As String) As String
            If pageConversion_PageName Is Nothing Then
                pageConversion_PageName = New SortedList(Of String, String)
            End If
            If Not pageConversion_PageName.ContainsKey(Caller.PortalSettings.PortalId.ToString & ":" & pageName) Then
                Try
                    Dim pageId As String = AbstractFactory.Instance.EngineController.PageId(pageName, Caller.PortalID)
                    If (Microsoft.VisualBasic.IsNumeric(pageId) AndAlso pageId > 0) OrElse (Not Microsoft.VisualBasic.IsNumeric(pageId) AndAlso pageId <> "") Then
                        pageConversion_PageName.Item(Caller.PortalSettings.PortalId.ToString & ":" & pageName) = pageId
                        Return pageId
                    End If
                Catch ex As Exception
                End Try
            Else
                Return pageConversion_PageName.Item(Caller.PortalSettings.PortalId.ToString & ":" & pageName)
            End If
            Return "-1"
        End Function

        Public Overrides ReadOnly Property RenderTag() As String
            Get
                Return "tab"
            End Get
        End Property
    End Class
End Namespace