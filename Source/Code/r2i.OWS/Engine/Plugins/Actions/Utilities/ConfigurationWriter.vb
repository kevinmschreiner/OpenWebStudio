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
Imports System.Xml.Serialization
Imports System.Collections.Generic
Imports System.Net.Mail
Imports System.Text
Imports r2i.OWS.Framework.Plugins.Actions
Imports r2i.OWS.Framework
Imports r2i.OWS.Framework.Utilities

Imports r2i.OWS.Framework.Entities
Imports r2i.OWS.Framework.DataAccess
Imports r2i.OWS.Framework.Utilities.Compatibility, r2i.OWS.Actions.Utilities

Namespace r2i.OWS.Actions.Utilities
    Public Class ConfigurationWriter
        Private config As Settings
        Public ReadOnly Property Configuration() As Settings
            Get
                Return config
            End Get
        End Property

        Public Sub New()
            config = New Settings
        End Sub
        Public Sub New(ByVal Source As String)
            config = Settings.Deserialize(Source)
        End Sub
        Public Sub New(ByRef Source As Settings)
            config = Source
        End Sub
        Private Function Cast_Boolean(ByVal Value As String) As Boolean
            If Not Value Is Nothing AndAlso Value.ToUpper = "TRUE" Then
                Return True
            Else
                Return False
            End If
        End Function
        Private Function Cast_Integer(ByVal Value As String) As Integer
            If Not Value Is Nothing AndAlso IsNumeric(Value) Then
                Return CInt(Value)
            Else
                Return 0
            End If
        End Function
        Private Function Cast_String(ByVal Value As String) As String
            If Value Is Nothing Then
                'ROMAIN: 08/22/2007
                'NOTE: Replacement Return ""
                Return String.Empty
            Else
                Return Value
            End If
        End Function
        Private Sub Assign_Configuration_Item(ByRef item As Object, ByRef Value As String)
            Try
                Dim tr As New System.IO.StringReader(Value)
                Dim xmld As New Xml.XPath.XPathDocument(tr)
                Dim xmlNav As Xml.XPath.XPathNavigator
                Dim b As Boolean = True
                xmlNav = xmld.CreateNavigator()
                b = xmlNav.MoveToFirstChild()
                If TypeOf item Is QueryOptionItem Then
                    Dim qitem As QueryOptionItem = item
                    While b
                        Select Case xmlNav.Name.ToUpper
                            Case "EscapeHTML".ToUpper
                                qitem.EscapeHTML = Cast_Boolean(xmlNav.Value)
                            Case "EscapeListX".ToUpper
                                qitem.EscapeListX = Cast_Boolean(xmlNav.Value)
                            Case "Protected".ToUpper
                                qitem.Protected = Cast_Boolean(xmlNav.Value)
                            Case "QuerySource".ToUpper
                                qitem.QuerySource = Cast_String(xmlNav.Value)
                            Case "QueryTarget".ToUpper
                                qitem.QueryTarget = Cast_String(xmlNav.Value)
                            Case "QueryTargetEmpty".ToUpper
                                qitem.QueryTargetEmpty = Cast_String(xmlNav.Value)
                            Case "QueryTargetLeft".ToUpper
                                qitem.QueryTargetLeft = Cast_String(xmlNav.Value)
                            Case "QueryTargetRight".ToUpper
                                qitem.QueryTargetRight = Cast_String(xmlNav.Value)
                            Case "VariableType".ToUpper
                                qitem.VariableType = Cast_String(xmlNav.Value)
                        End Select
                        b = xmlNav.MoveToNext
                    End While
                ElseIf TypeOf item Is MessageActionItem Then
                    Dim mitem As MessageActionItem = item
                    While b
                        Select Case xmlNav.Name.ToUpper
                            Case "ActionType".ToUpper
                                mitem.ActionType = Cast_String(Value)
                            Case "Level".ToUpper
                                mitem.Level = Cast_Integer(Value)
                            Case "ActionInformation".ToUpper
                                b = xmlNav.MoveToFirstChild
                                If b Then
                                    'TODO: Check this
                                    'Dim splitter As New SmartSplitter
                                    'splitter.Split(item.ActionInformation)
                                    'While b
                                    '    If xmlNav.Name.ToUpper = "CLEAR" Then
                                    '        splitter = New SmartSplitter
                                    '    ElseIf xmlNav.Name.ToUpper = "ADD" Then
                                    '        splitter.Add(xmlNav.Value)
                                    '    ElseIf IsNumeric(xmlNav.Name) Then
                                    '        splitter.Item(xmlNav.Name) = xmlNav.Value
                                    '    End If

                                    '    b = xmlNav.MoveToNext
                                    'End While
                                    'item.ActionInformation = splitter.Blend

                                    'b = xmlNav.MoveToParent
                                    'If b Then
                                    '    xmlNav.MoveToNext()
                                    'End If
                                End If
                        End Select
                        b = xmlNav.MoveToNext
                    End While
                ElseIf TypeOf item Is SearchOptionItem Then
                    Dim sitem As SearchOptionItem = item
                    While b
                        Select Case xmlNav.Name.ToUpper
                            Case "SearchField".ToUpper
                                sitem.SearchField = Cast_String(Value)
                            Case "SearchOption".ToUpper
                                sitem.SearchOption = Cast_String(Value)
                        End Select
                        b = xmlNav.MoveToNext
                    End While
                End If
                tr.Close()
            Catch ex As Exception
            End Try
        End Sub
        Public Sub Assign(ByVal Name As String, ByVal Value As String)
            If Not Name Is Nothing AndAlso Name.Length > 0 Then
                Name = Name.Replace("\", "/")
                Dim params As String() = Name.Split("/")
                If params.Length = 1 Then
                    Select Case params(0).ToUpper
                        Case "autoRefreshInterval".ToUpper
                            config.autoRefreshInterval = Cast_String(Value)
                        Case "customConnection".ToUpper
                            config.customConnection = Cast_String(Value)
                        Case "defaultItem".ToUpper
                            config.defaultItem = Cast_String(Value)
                        Case "enableAdmin_Admin".ToUpper
                            config.enableAdmin_Admin = Cast_Boolean(Value)
                        Case "enableAdmin_Edit".ToUpper
                            config.enableAdmin_Edit = Cast_Boolean(Value)
                        Case "enableAdmin_Super".ToUpper
                            config.enableAdmin_Super = Cast_Boolean(Value)
                        Case "enableAdvancedParsing".ToUpper
                            config.enableAdvancedParsing = Cast_Boolean(Value)
                        Case "enableAJAX".ToUpper
                            config.enableAJAX = Cast_Boolean(Value)
                        Case "enableAJAXPaging".ToUpper
                            config.enableAJAXPaging = Cast_Boolean(Value)
                        Case "enableAJAXCustomPaging".ToUpper
                            config.enableAJAXCustomPaging = Cast_Boolean(Value)
                        Case "enableAJAXCustomStatus".ToUpper
                            config.enableAJAXCustomStatus = Cast_Boolean(Value)
                        Case "enableAJAXManual".ToUpper
                            config.enableAJAXManual = Cast_Boolean(Value)
                        Case "enableAJAXPageHistory".ToUpper
                            config.enableAJAXPageHistory = Cast_Boolean(Value)
                        Case "customAJAXPageHistory".ToUpper
                            config.customAJAXPageHistory = Cast_String(Value)
                        Case "enableAlphaFilter".ToUpper
                            config.enableAlphaFilter = Cast_Boolean(Value)
                        Case "enableCompoundIIFConditions".ToUpper
                            config.enableCompoundIIFConditions = Cast_Boolean(Value)
                        Case "enableCustomPaging".ToUpper
                            config.enableCustomPaging = Cast_Boolean(Value)
                        Case "enabledForcedQuerySplit".ToUpper
                            config.enabledForcedQuerySplit = Cast_Boolean(Value)
                        Case "enableExcelExport".ToUpper
                            config.enableExcelExport = Cast_Boolean(Value)
                        Case "enableHide_OnNoQuery".ToUpper
                            config.enableHide_OnNoQuery = Cast_Boolean(Value)
                        Case "enableHide_OnNoResults".ToUpper
                            config.enableHide_OnNoResults = Cast_Boolean(Value)
                        Case "enableMultipleColumnSorting".ToUpper
                            config.enableMultipleColumnSorting = Cast_Boolean(Value)
                        Case "enablePageSelection".ToUpper
                            config.enablePageSelection = Cast_Boolean(Value)
                        Case "enableQueryDebug".ToUpper
                            config.enableQueryDebug = Cast_Boolean(Value)
                        Case "enableQueryDebug_Admin".ToUpper
                            config.enableQueryDebug_Admin = Cast_Boolean(Value)
                        Case "enableQueryDebug_Edit".ToUpper
                            config.enableQueryDebug_Edit = Cast_Boolean(Value)
                        Case "enableQueryDebug_Super".ToUpper
                            config.enableQueryDebug_Super = Cast_Boolean(Value)
                        Case "disableOpenScript".ToUpper
                            config.disableOpenScript = Cast_Boolean(Value)
                        Case "enableSilverlight".ToUpper
                            config.enableSilverlight = Cast_Boolean(Value)
                        Case "filter".ToUpper
                            config.filter = Cast_String(Value)
                        Case "includeJavascriptUtilities".ToUpper
                            config.includeJavascriptUtilities = Cast_Boolean(Value)
                        Case "includeJavascriptValidation".ToUpper
                            config.includeJavascriptValidation = Cast_Boolean(Value)
                        Case "javascriptOnComplete".ToUpper
                            config.javascriptOnComplete = Cast_String(Value)
                        Case "listAItem".ToUpper
                            config.listAItem = Cast_String(Value)
                        Case "listItem".ToUpper
                            config.listItem = Cast_String(Value)
                        Case "ModuleCommunicationMessageType".ToUpper
                            config.ModuleCommunicationMessageType = Cast_String(Value)
                        Case "noqueryItem".ToUpper
                            config.noqueryItem = Cast_String(Value)
                        Case "query".ToUpper
                            config.query = Cast_String(Value)
                        Case "recordsPerPage".ToUpper
                            config.recordsPerPage = Cast_Integer(Value)
                        Case "SearchAuthor".ToUpper
                            config.SearchAuthor = Cast_String(Value)
                        Case "SearchContent".ToUpper
                            config.SearchContent = Cast_String(Value)
                        Case "SearchDate".ToUpper
                            config.SearchDate = Cast_String(Value)
                        Case "SearchDescription".ToUpper
                            config.SearchDescription = Cast_String(Value)
                        Case "SearchKey".ToUpper
                            config.SearchKey = Cast_String(Value)
                        Case "SearchLink".ToUpper
                            config.SearchLink = Cast_String(Value)
                        Case "SearchQuery".ToUpper
                            config.SearchQuery = Cast_String(Value)
                        Case "SearchTitle".ToUpper
                            config.SearchTitle = Cast_String(Value)
                        Case "showAll".ToUpper
                            config.showAll = Cast_Boolean(Value)
                        Case "skipRedirectActions".ToUpper
                            config.skipRedirectActions = Cast_Boolean(Value)
                        Case "skipSubqueryDebugging".ToUpper
                            config.skipSubqueryDebugging = Cast_Boolean(Value)
                        Case "useExplicitSystemVariables".ToUpper
                            config.useExplicitSystemVariables = Cast_Boolean(Value)
                        Case "Version".ToUpper
                            config.Version = Cast_Integer(Value)
                        Case "listItems".ToUpper
                            If Value Is Nothing OrElse Value.Length = 0 Then
                                'ROMAIN: Generic replacement - 08/21/2007
                                'config.listItems = New ArrayList
                                config.listItems = New List(Of ListFormatItem)
                            End If
                        Case "messageItems".ToUpper
                            If Value Is Nothing OrElse Value.Length = 0 Then
                                'ROMAIN: Generic replacement - 08/21/2007
                                'config.messageItems = New ArrayList
                                config.messageItems = New List(Of MessageActionItem)
                            End If
                        Case "searchItems".ToUpper
                            If Value Is Nothing OrElse Value.Length = 0 Then
                                'ROMAIN: Generic replacement - 08/21/2007
                                'config.searchItems = New ArrayList
                                config.searchItems = New List(Of SearchOptionItem)
                            End If
                        Case "queryItems".ToUpper
                            If Value Is Nothing OrElse Value.Length = 0 Then
                                'ROMAIN: Generic replacement - 08/21/2007
                                'config.queryItems = New ArrayList
                                config.queryItems = New List(Of QueryOptionItem)
                            End If
                    End Select
                ElseIf params.Length > 1 Then
                    Select Case params(0).ToUpper
                        Case "listItems".ToUpper 'MULTI
                            Dim index As Integer = Cast_Integer(params(1))
                            If config.listItems Is Nothing Then
                                'ROMAIN: Generic replacement - 08/21/2007
                                'config.listItems = New ArrayList
                                config.listItems = New List(Of ListFormatItem)
                            End If
                            While config.listItems.Count <= index
                                Dim item As New ListFormatItem
                                item.GroupStatement = ""
                                item.ListFooter = ""
                                item.ListHeader = ""
                                item.Index = config.listItems.Count
                                config.listItems.Add(item)
                            End While
                            If config.listItems.Count >= index Then
                                Dim item As ListFormatItem
                                item = config.listItems(index)
                                If Not item Is Nothing Then
                                    If params.Length > 2 Then
                                        Select Case params(2).ToUpper
                                            Case "GroupStatement".ToUpper
                                                item.GroupStatement = Cast_String(Value)
                                            Case "ListFooter".ToUpper
                                                item.ListFooter = Cast_String(Value)
                                            Case "ListHeader".ToUpper
                                                item.ListHeader = Cast_String(Value)
                                        End Select
                                        config.listItems(index) = item
                                    Else
                                        If Value Is Nothing OrElse Value.Length = 0 Then
                                            config.listItems.RemoveAt(index)
                                        End If
                                    End If
                                End If
                            End If
                        Case "messageItems".ToUpper 'MULTI
                            Dim index As Integer = Cast_Integer(params(1))
                            If config.messageItems Is Nothing Then
                                'ROMAIN: Generic replacement - 08/21/2007
                                'config.messageItems = New ArrayList
                                config.messageItems = New List(Of MessageActionItem)
                            End If
                            While config.messageItems.Count <= index
                                Dim item As New MessageActionItem
                                item.ActionType = ""
                                item.ActionInformation = ""
                                item.Index = config.messageItems.Count
                                item.Level = 0
                                config.messageItems.Add(item)
                            End While
                            If config.messageItems.Count >= index Then
                                Dim item As MessageActionItem
                                item = config.messageItems(index)
                                If Not item Is Nothing Then
                                    If params.Length > 2 Then
                                        Select Case params(2).ToUpper
                                            Case "ActionType".ToUpper
                                                item.ActionType = Cast_String(Value)
                                            Case "Level".ToUpper
                                                item.Level = Cast_Integer(Value)
                                            Case "ActionInformation".ToUpper
                                                If params.Length > 3 Then
                                                    'TODO: Check This
                                                    'Dim splitter As New SmartSplitter
                                                    'splitter.Split(item.ActionInformation)
                                                    'If params(3) = "+" Then
                                                    '    'APPEND
                                                    '    splitter.Add(Value)
                                                    'Else
                                                    '    Dim si As Integer = Cast_Integer(params(3))
                                                    '    While splitter.Length <= si
                                                    '        splitter.Add(Value)
                                                    '    End While
                                                    '    splitter(si) = Value
                                                    'End If
                                                    'item.ActionInformation = splitter.Blend
                                                Else
                                                    item.ActionInformation = ""
                                                End If
                                        End Select
                                        config.messageItems(index) = item
                                    Else
                                        If Value Is Nothing OrElse Value.Length = 0 Then
                                            config.messageItems.RemoveAt(index)
                                        End If
                                    End If
                                End If
                            End If
                        Case "queryItems".ToUpper 'MULTI
                            Dim index As Integer = Cast_Integer(params(1))
                            If config.queryItems Is Nothing Then
                                'ROMAIN: Generic replacement - 08/21/2007
                                'config.queryItems = New ArrayList
                                config.queryItems = New List(Of QueryOptionItem)
                            End If
                            While config.queryItems.Count <= index
                                Dim item As New QueryOptionItem
                                item.QuerySource = ""
                                item.QueryTarget = ""
                                item.QueryTargetEmpty = ""
                                item.QueryTargetLeft = ""
                                item.QueryTargetRight = ""
                                item.VariableType = ""
                                item.Index = config.queryItems.Count
                                config.queryItems.Add(item)
                            End While
                            If config.queryItems.Count >= index Then
                                Dim item As QueryOptionItem = Nothing
                                item = config.queryItems(index)
                                If Not item Is Nothing Then
                                    If params.Length > 2 Then
                                        Select Case params(2).ToUpper
                                            Case "Source".ToUpper
                                                Assign_Configuration_Item(item, Value)
                                            Case "EscapeHTML".ToUpper
                                                item.EscapeHTML = Cast_Boolean(Value)
                                            Case "EscapeListX".ToUpper
                                                item.EscapeListX = Cast_Boolean(Value)
                                            Case "Protected".ToUpper
                                                item.Protected = Cast_Boolean(Value)
                                            Case "QuerySource".ToUpper
                                                item.QuerySource = Cast_String(Value)
                                            Case "QueryTarget".ToUpper
                                                item.QueryTarget = Cast_String(Value)
                                            Case "QueryTargetEmpty".ToUpper
                                                item.QueryTargetEmpty = Cast_String(Value)
                                            Case "QueryTargetLeft".ToUpper
                                                item.QueryTargetLeft = Cast_String(Value)
                                            Case "QueryTargetRight".ToUpper
                                                item.QueryTargetRight = Cast_String(Value)
                                            Case "VariableType".ToUpper
                                                item.VariableType = Cast_String(Value)
                                        End Select
                                        config.queryItems(index) = item
                                    Else
                                        If Value Is Nothing OrElse Value.Length = 0 Then
                                            config.queryItems.RemoveAt(index)
                                        End If
                                    End If
                                End If
                            End If
                        Case "searchItems".ToUpper
                            Dim index As Integer = Cast_Integer(params(1))
                            If config.searchItems Is Nothing Then
                                'ROMAIN: Generic replacement - 08/21/2007
                                'config.searchItems = New ArrayList
                                config.searchItems = New List(Of SearchOptionItem)
                            End If
                            While config.searchItems.Count <= index
                                Dim item As New SearchOptionItem
                                item.SearchField = ""
                                item.SearchOption = ""
                                item.Index = config.searchItems.Count
                                config.searchItems.Add(item)
                            End While
                            If config.searchItems.Count >= index Then
                                Dim item As SearchOptionItem
                                item = config.searchItems(index)
                                If Not item Is Nothing Then
                                    If params.Length > 2 Then
                                        Select Case params(2).ToUpper
                                            Case "SearchField".ToUpper
                                                item.SearchField = Cast_String(Value)
                                            Case "SearchOption".ToUpper
                                                item.SearchOption = Cast_String(Value)
                                        End Select
                                        config.searchItems(index) = item
                                    Else
                                        If Value Is Nothing OrElse Value.Length = 0 Then
                                            config.searchItems.RemoveAt(index)
                                        End If
                                    End If
                                End If
                            End If
                    End Select
                End If
            End If
        End Sub
    End Class
End Namespace