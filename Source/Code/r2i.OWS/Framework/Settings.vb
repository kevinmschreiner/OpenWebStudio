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
Imports System.ComponentModel
Imports r2i.OWS.Framework.Entities
Imports r2i.OWS.Framework.DataAccess
Imports r2i.OWS.Framework.Plugins.Actions
Imports System.Xml
Imports System.IO
Imports r2i.OWS.Framework.Utilities
Imports r2i.OWS.Framework.Utilities.Compatibility
Imports r2i.OWS.Framework.Utilities.JSON
Imports r2i.OWS.Newtonsoft.Json


Namespace r2i.OWS.Framework
    '<Serializable(), XmlInclude(GetType(SearchOptionItem)), XmlInclude(GetType(QueryOptionItem)), XmlInclude(GetType(ListFormatItem)), XmlInclude(GetType(MessageActionItem))> _
    <Serializable()> _
    Public Class Settings
        Public Name As String
        Public ConfigurationID As String

        Public searchItems As List(Of SearchOptionItem)
        Public queryItems As List(Of QueryOptionItem) 'THIS IS ONLY IN PLACE FOR UPGRADES
        Public listItems As List(Of ListFormatItem)
        Public messageItems As List(Of MessageActionItem)

        Private m_name As String
        Public recordsPerPage As Integer
        Public enableAlphaFilter As Boolean
        Public enablePageSelection As Boolean
        Public enableRecordsPerPage As Boolean
        Public enableCustomPaging As Boolean
        Public enableExcelExport As Boolean
        Public enableHide_OnNoQuery As Boolean
        Public enableHide_OnNoResults As Boolean
        Public enableAdvancedParsing As Boolean
        Public enableCompoundIIFConditions As Boolean
        Public enableQueryDebug As Boolean  'ALL USERS
        Public enableQueryDebug_Edit As Boolean
        Public enableQueryDebug_Admin As Boolean
        Public enableQueryDebug_Super As Boolean
        Public enableQueryDebug_Log As Boolean
        Public enableQueryDebug_ErrorLog As Boolean
        Public autoRefreshInterval As String
        Public skipRedirectActions As Boolean
        Public skipSubqueryDebugging As Boolean
        Public enableAdmin_Edit As Boolean
        Public enableAdmin_Admin As Boolean
        Public enableAdmin_Super As Boolean

        Public BotRecordCount As Integer
        Public BotPageVariableName As String
        Public enableBotShowAllRecords As Boolean
        Public enableBotDetection As Boolean
        Public BotNonAjaxText As String

        Public disableOpenScript As Boolean
        Public enableSilverlight As Boolean

        Public enableAJAX As Boolean
        Public enableAJAXPaging As Boolean
        Public enableAJAXCustomPaging As Boolean
        Public enableAJAXCustomStatus As Boolean
        Public enableAJAXPageHistory As Boolean
        Public customAJAXPageHistory As String
        Public enableAJAXManual As Boolean
        Public includeJavascriptUtilities As Boolean
        Public includeJavascriptValidation As Boolean
        Public javascriptInclude As String()
        Public javascriptOnComplete As String
        Public enableMultipleColumnSorting As Boolean
        Public ModuleCommunicationMessageType As String
        Public showAll As Boolean
        Public useExplicitSystemVariables As Boolean
        Public enabledForcedQuerySplit As Boolean
        Public query As String
        Public filter As String
        Public customConnection As String
        Public listItem As String
        Public listAItem As String
        Public defaultItem As String
        Public noqueryItem As String
        Public SearchQuery As String
        Public SearchTitle As String
        Public SearchLink As String
        Public SearchAuthor As String
        Public SearchDate As String
        Public SearchKey As String
        Public SearchContent As String
        Public SearchDescription As String
        Public Header As String
        Public Footer As String
        Public Title As String
        Public Version As Integer
        <NonSerialized(), JsonIgnore()> _
        Public ResourceKey As String
        'Private _ResourceKey As String
        'Public Sub ResourceKey(ByVal Value As String)
        '    _ResourceKey = Value
        'End Sub
        'Public Function ResourceKey() As String
        '    If Not _ResourceKey Is Nothing Then
        '        Return _ResourceKey
        '    Else
        '        Return String.Empty
        '    End If
        'End Function



        'Externally Control how the Data will render
        Private _RenderType As RenderType = RenderType.Default
        Private _DelimiterChar As String = Nothing
        Private _FileName As String '= ""
        Public Enum RenderType
            Undefined
            [Default]
            Excel
            Excel_Complete
            Report
            Report_Complete
            Word
            Word_Complete
            Delimited
            Delimited_Complete
        End Enum
        Public Function OutputType(Optional ByVal SetType As RenderType = RenderType.Undefined, Optional ByVal Delimiter As String = Nothing, Optional ByVal FileName As String = "") As RenderType
            If Not SetType = RenderType.Undefined Then
                Me._RenderType = SetType
                Me._DelimiterChar = Delimiter
                Me._FileName = FileName
            End If
            Return Me._RenderType
        End Function
        Public ReadOnly Property OutputFilename(Optional ByVal Extension As String = "") As String
            Get
                If Me._FileName = "" Then
                    Return "Output." & IIf(Extension = "", "htm", Extension)
                Else
                    Return Me._FileName
                End If
            End Get
        End Property
        Public Function isEditable(ByRef UserInfo As IUser, ByRef PortalSettings As IPortalSettings, ByVal ModuleIsEditable As Boolean) As Boolean
            Dim canEdit As Boolean = False
            If enableAdmin_Edit = False And enableAdmin_Admin = False And enableAdmin_Super = False Then
                enableAdmin_Edit = True
            End If
            If enableAdmin_Edit AndAlso ModuleIsEditable Then
                canEdit = True
            ElseIf enableAdmin_Admin AndAlso AbstractFactory.Instance.UserController.IsInRoles(UserInfo, PortalSettings.AdministratorRoleName) Then
                canEdit = True
            ElseIf enableAdmin_Super AndAlso UserInfo.IsSuperUser Then
                canEdit = True
            End If
            If Not Utilities.Utility.ConfigurationSetting("ListX.Maximum.Security") Is Nothing AndAlso Utility.ConfigurationSetting("ListX.Maximum.Security").ToLower = "true" Then
                If Not UserInfo.IsSuperUser Then
                    canEdit = False
                End If
            End If
            Return canEdit
        End Function
        Public Function canDebug(ByRef UserInfo As IUser, ByRef PortalSettings As IPortalSettings, ByVal ModuleIsEditable As Boolean) As Boolean
            If enableQueryDebug Then
                Return True
            ElseIf enableQueryDebug_Edit AndAlso ModuleIsEditable Then
                Return True
            ElseIf enableQueryDebug_Admin AndAlso AbstractFactory.Instance.UserController.IsInRoles(UserInfo, PortalSettings.AdministratorRoleName) Then
                'Engine.IsInRoleNames(UserInfo, PortalSettings.AdministratorRoleName) Then
                Return True
            ElseIf enableQueryDebug_Super AndAlso UserInfo.IsSuperUser Then
                Return True
            End If
            Return False
        End Function
        Public Function canDebug_Change(ByRef IsSuperUser As Boolean, ByRef IsAdministrator As Boolean, ByVal ModuleIsEditable As Boolean) As Boolean
            If enableQueryDebug Then
                Return True
            ElseIf enableQueryDebug_Edit AndAlso ModuleIsEditable Then
                Return True
            ElseIf enableQueryDebug_Admin AndAlso IsAdministrator OrElse IsSuperUser Then
                Return True
            ElseIf enableQueryDebug_Super AndAlso IsSuperUser Then
                Return True
            End If
            Return False
        End Function


#Region "Query Items"
        Private _QueryItems_sd As Generic.Dictionary(Of String, QueryOptionItem)
        Public Property QueryItem(ByVal key As String) As QueryOptionItem
            Get
                If _QueryItems_sd.ContainsKey(key) Then
                    Return _QueryItems_sd(key)
                Else
                    Return Nothing
                End If
            End Get
            Set(ByVal value As QueryOptionItem)
                If _QueryItems_sd Is Nothing Then
                    _QueryItems_sd = New Generic.Dictionary(Of String, QueryOptionItem)
                End If
                If _QueryItems_sd.ContainsKey(key) Then
                    _QueryItems_sd(key) = value
                Else
                    _QueryItems_sd.Add(key, value)
                End If
            End Set
        End Property
        Public Function QueryItemsKeys() As String()
            Static comparer As QueryOptionItem.QueryOptionItemKeyComparer
            If comparer Is Nothing Then
                comparer = New QueryOptionItem.QueryOptionItemKeyComparer
            End If

            If Not _QueryItems_sd Is Nothing Then
                Dim s As ArrayList = Nothing
                s = New ArrayList(_QueryItems_sd.Keys)
                s.Sort(comparer)
                Return s.ToArray(GetType(String))
            End If
            Return Nothing
        End Function
        Public Sub QueryItemsClear()
            _QueryItems_sd = Nothing
        End Sub
#End Region

        Public Function Serialize() As String
            Try
                If searchItems Is Nothing Then
                    searchItems = New List(Of SearchOptionItem)
                End If
                If queryItems Is Nothing Then
                    queryItems = New List(Of QueryOptionItem)
                End If
                If listItems Is Nothing Then
                    listItems = New List(Of ListFormatItem)
                End If
                If messageItems Is Nothing Then
                    messageItems = New List(Of MessageActionItem)
                End If

                Dim sItem As SearchOptionItem
                Dim i As Integer = 0
                For Each sItem In searchItems
                    sItem.Index = i
                    i += 1
                Next

                Dim lItem As ListFormatItem
                i = 0
                For Each lItem In listItems
                    lItem.Index = i
                    i += 1
                Next

                Serialize = JavaScriptConvert.SerializeObject(DirectCast(Me, Object))
            Catch ex As Exception
#If DEBUG Then
                Try
                    'ROMAIN: 09/18/07
                    'TODO: CHANGE EXCEPTIONS
                    'DotNetNuke.Services.Exceptions.LogException(ex)
                Catch exD As Exception
                End Try
#End If
                Serialize = Nothing
            End Try
        End Function
        Protected Sub ReIndex_Actions(ByRef root As MessageActionItem, ByRef StartIndex As Integer)
            Dim lMessage As MessageActionItem
            If root Is Nothing Then
                For Each lMessage In messageItems
                    lMessage.Index = StartIndex
                    StartIndex += 1
                    If Not lMessage.ChildActions Is Nothing AndAlso lMessage.ChildActions.Count > 0 Then
                        ReIndex_Actions(lMessage, StartIndex)
                    End If
                Next
            Else
                For Each lMessage In root.ChildActions
                    lMessage.Index = StartIndex
                    StartIndex += 1
                    If Not lMessage.ChildActions Is Nothing AndAlso lMessage.ChildActions.Count > 0 Then
                        ReIndex_Actions(lMessage, StartIndex)
                    End If
                Next
            End If
        End Sub

        Private Shared ReadOnly Property _Serializer() As Xml.Serialization.XmlSerializer
            Get
                Static xmlSerializer As Xml.Serialization.XmlSerializer
                'Try
                '    obj = DotNetNuke.Services.Cache.CachingProvider.Instance().GetItem("LISTX16-SERIALIZER")
                'Catch ex As Exception
                '    DotNetNuke.Services.Exceptions.LogException(ex)
                'End Try
                'If obj Is Nothing Then
                If xmlSerializer Is Nothing Then
                    Try
                        xmlSerializer = New Xml.Serialization.XmlSerializer(GetType(Settings))
                    Catch ex As Exception
                        Dim s As String = ex.InnerException.ToString
                        Console.WriteLine(s)
                    End Try

                End If
                '    Try
                '        DotNetNuke.Services.Cache.CachingProvider.Instance().Add("LISTX16-SERIALIZER", obj, Nothing, DateTime.MaxValue, New TimeSpan(4, 27, 0), Web.Caching.CacheItemPriority.Normal, Nothing)
                '    Catch ex As Exception
                '        DotNetNuke.Services.Exceptions.LogException(ex)
                '    End Try
                'End If
                Return xmlSerializer
            End Get
        End Property

        Private Shared Function Copy(ByRef Source As Settings) As Settings
            Dim str As String = Source.Serialize
            Return Settings.Deserialize(str)
        End Function
        Public Shared Function Clone(ByRef src As Settings) As Settings
            Dim c As New Settings
            c.searchItems = New List(Of SearchOptionItem)
            c.queryItems = New List(Of QueryOptionItem)
            c.listItems = New List(Of ListFormatItem)
            'c.messageItems = New List(Of MessageActionItem)
            c.messageItems = src.messageItems

            c.Name = src.Name
            c.ConfigurationID = src.ConfigurationID

            c.recordsPerPage = src.recordsPerPage
            c.enableAlphaFilter = src.enableAlphaFilter
            c.enablePageSelection = src.enablePageSelection
            c.enableRecordsPerPage = src.enableRecordsPerPage
            c.enableCustomPaging = src.enableCustomPaging
            c.enableExcelExport = src.enableExcelExport
            c.enableHide_OnNoQuery = src.enableHide_OnNoQuery
            c.enableHide_OnNoResults = src.enableHide_OnNoResults
            c.enableAdvancedParsing = src.enableAdvancedParsing
            c.enableCompoundIIFConditions = src.enableCompoundIIFConditions
            c.enableQueryDebug = src.enableQueryDebug
            c.enableQueryDebug_Edit = src.enableQueryDebug_Edit
            c.enableQueryDebug_Admin = src.enableQueryDebug_Admin
            c.enableQueryDebug_Super = src.enableQueryDebug_Super
            c.enableQueryDebug_Log = src.enableQueryDebug_Log
            c.enableQueryDebug_ErrorLog = src.enableQueryDebug_ErrorLog
            c.autoRefreshInterval = src.autoRefreshInterval
            c.skipRedirectActions = src.skipRedirectActions
            c.skipSubqueryDebugging = src.skipSubqueryDebugging
            c.enableAdmin_Edit = src.enableAdmin_Edit
            c.enableAdmin_Admin = src.enableAdmin_Admin
            c.enableAdmin_Super = src.enableAdmin_Super

            c.BotRecordCount = src.BotRecordCount
            c.BotPageVariableName = src.BotPageVariableName
            c.enableBotShowAllRecords = src.enableBotShowAllRecords
            c.enableBotDetection = src.enableBotDetection
            c.BotNonAjaxText = src.BotNonAjaxText
            c.javascriptInclude = src.javascriptInclude

            c.disableOpenScript = src.disableOpenScript
            c.enableSilverlight = src.enableSilverlight

            c.enableAJAX = src.enableAJAX
            c.enableAJAXPaging = src.enableAJAXPaging
            c.enableAJAXCustomPaging = src.enableAJAXCustomPaging
            c.enableAJAXCustomStatus = src.enableAJAXCustomStatus
            c.enableAJAXPageHistory = src.enableAJAXPageHistory
            c.customAJAXPageHistory = src.customAJAXPageHistory
            c.enableAJAXManual = src.enableAJAXManual
            c.includeJavascriptUtilities = src.includeJavascriptUtilities
            c.includeJavascriptValidation = src.includeJavascriptValidation
            c.javascriptOnComplete = src.javascriptOnComplete
            c.enableMultipleColumnSorting = src.enableMultipleColumnSorting
            c.ModuleCommunicationMessageType = src.ModuleCommunicationMessageType
            c.showAll = src.showAll
            c.useExplicitSystemVariables = src.useExplicitSystemVariables
            c.enabledForcedQuerySplit = src.enabledForcedQuerySplit
            c.query = src.query
            c.filter = src.filter
            c.customConnection = src.customConnection
            c.listItem = src.listItem
            c.listAItem = src.listAItem
            c.defaultItem = src.defaultItem
            c.noqueryItem = src.noqueryItem
            c.SearchQuery = src.SearchQuery
            c.SearchTitle = src.SearchTitle
            c.SearchLink = src.SearchLink
            c.SearchAuthor = src.SearchAuthor
            c.SearchDate = src.SearchDate
            c.SearchKey = src.SearchKey
            c.SearchContent = src.SearchContent
            c.SearchDescription = src.SearchDescription
            c.Header = src.Header
            c.Footer = src.Footer
            c.Title = src.Title
            c.Version = src.Version
            c.ResourceKey = src.ResourceKey
            'Me._RenderType As RenderType = RenderType.Default
            'Me._DelimiterChar = x
            'Me._FileName = x

            'c = Copy(Source)
            'c.Reset()
            Return c
        End Function
        Public Sub ClearTemplate()
            Me.query = String.Empty
            Me.listAItem = String.Empty
            Me.listItems = New List(Of ListFormatItem)
            Me.listItem = String.Empty
            Me.noqueryItem = String.Empty
            Me.defaultItem = String.Empty
        End Sub
        Public Sub Reset()
            defaultItem = String.Empty
            filter = String.Empty
            listAItem = String.Empty
            listItem = String.Empty
            listItems = Nothing
            noqueryItem = String.Empty
            query = String.Empty
            queryItems = Nothing
            recordsPerPage = 0
            SearchAuthor = String.Empty
            SearchContent = String.Empty
            SearchDate = String.Empty
            SearchDescription = String.Empty
            searchItems = Nothing
            SearchKey = String.Empty
            SearchLink = String.Empty
            SearchQuery = String.Empty
            SearchTitle = String.Empty
        End Sub

        Public Shared Function Deserialize(ByVal Value As String) As Settings
            Deserialize = Nothing
            Try
                If Not Value Is Nothing AndAlso Value.Length > 0 Then
                    Dim jsonString As String = Value
                    If jsonString.TrimStart.StartsWith("<") Then
                        Dim xmlToJsonConv As New JsonConversion
                        Dim jsonText As String = xmlToJsonConv.GetJsonStructure(jsonString)
                        jsonString = jsonText
                    End If
                    If jsonString.TrimStart.StartsWith("{") Then
                        Dim jsonConv As New JsonConversion
                        Dim jsonText As String = jsonConv.GetJsonStructure(Value)
                        If jsonText.Length > 0 Then
                            Deserialize = jsonConv.GetDeserializedListXConfiguration(jsonText)
                        Else
                            'TODO: Implements exception json unvalid 
                        End If
                    Else
                        'TODO: Implements exception neither xml or json
                        Deserialize = Nothing
                        Exit Function
                    End If
                End If
            Catch ex As Exception
#If DEBUG Then
                Try
                    'ROMAIN: 09/18/07
                    'TODO: CHANGE EXCEPTIONS
                    'DotNetNuke.Services.Exceptions.LogException(ex)
                Catch exD As Exception
                End Try
#End If
                Deserialize = Nothing
            End Try
            'REG - Added for adding Level property to Actions
            'RepairActionLevels(Deserialize)
            If Not Deserialize Is Nothing Then
                RepairVersion(Deserialize)
            End If

            If Deserialize.listItems Is Nothing Then
                Deserialize.listItems = New List(Of ListFormatItem)
            End If


        End Function

        Private Shared Sub RepairVersion(ByRef XSet As Settings)
            'Just need to check the version here
            If XSet.Version < 16 Then
                FixActionItems(XSet)
                FixItemFormatting(XSet)
                XSet.enableAdmin_Edit = True 'THIS IS THE DEFAULT
                XSet.includeJavascriptUtilities = True
                XSet.Version = 16
            ElseIf XSet.Version < 17 Then
                XSet.enableAdmin_Edit = True 'THIS IS THE DEFAULT
                XSet.Version = 17
            ElseIf XSet.Version < 20 Then
                XSet.Version = 20
            End If
        End Sub
        Private Shared Sub FixActionItems(ByRef XSet As Settings)
            If Not XSet Is Nothing AndAlso Not XSet.messageItems Is Nothing AndAlso XSet.messageItems.Count > 0 Then
                Dim messageAction As MessageActionItem
                For Each messageAction In XSet.messageItems
                    messageAction.Repair(XSet.Version)
                Next
            End If
        End Sub
        Private Shared Sub FixItemFormatting(ByRef XSet As Settings)
            If Not XSet Is Nothing Then
                If Not XSet.listItems Is Nothing Then
                    Dim listitem As ListFormatItem
                    For Each listitem In XSet.listItems
                        If Not listitem.ListHeader Is Nothing Then
                            FixSortItem(listitem.ListHeader)
                        End If
                        If Not listitem.ListFooter Is Nothing Then
                            FixSortItem(listitem.ListFooter)
                        End If
                    Next
                End If
                If Not XSet.listItem Is Nothing Then
                    FixSortItem(XSet.listItem)
                End If
                If Not XSet.listAItem Is Nothing Then
                    FixSortItem(XSet.listAItem)
                End If
            End If
        End Sub
        Private Shared Sub FixSortItem(ByRef thisContent As String)
            'Since this configuration doesn't provide the capability to handle the more
            'robust string parameterization, all item formatting for Sorting must be corrected.

            Dim istart As Integer
            Dim starter As String = "{SORT,"
            istart = thisContent.ToUpper.IndexOf(starter)
            While istart >= 0
                Dim iend As Integer = thisContent.IndexOf("}", istart)
                If iend > istart Then
                    'WE FOUND THE ENDPOINT
                    Dim xlength As Integer = (starter).Length
                    Dim fvalue As String = thisContent.Substring(istart + xlength, iend - istart - xlength)
                    Dim sa As SortAction = Nothing
                    Try
                        Dim sortParameters() As String = fvalue.Split(",")
                        If sortParameters.Length = 6 Then
                            'REFORMAT HERE
                            sortParameters(1) = """" & sortParameters(1).Replace("""", "\""") & """"
                            sortParameters(2) = """" & sortParameters(2).Replace("""", "\""") & """"
                            sortParameters(3) = """" & sortParameters(3).Replace("""", "\""") & """"

                            fvalue = "{SORT," & String.Join(",", sortParameters) & "}"
                        Else
                            fvalue = "{SORT," & fvalue & "}"
                        End If
                    Catch ex As Exception
                        fvalue = "{SORT," & fvalue & "}"
                    End Try

                    thisContent = thisContent.Substring(0, istart) & fvalue & thisContent.Substring(iend + 1)
                End If
                istart = thisContent.IndexOf(starter, istart + 1)
            End While
        End Sub

    End Class

End Namespace
