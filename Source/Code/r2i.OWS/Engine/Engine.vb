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
Imports r2i.OWS.Framework.DataAccess
Imports r2i.OWS.Framework.Entities
Imports r2i.OWS.Framework.Utilities
Imports r2i.OWS.Framework.Utilities.JSON
Imports r2i.OWS.Framework.Plugins.Queries
Imports r2i.OWS.Framework.Plugins.Renderers
Imports r2i.OWS.Framework.Plugins.Formatters
Imports r2i.OWS.Framework.Utilities.Compatibility, r2i.OWS.Actions.Utilities
Imports r2i.OWS.Renderers
Imports System.Reflection
Imports System.Web
Imports System.Web.UI
Imports r2i.OWS.Newtonsoft.Json


Namespace r2i.OWS
    Public Class Engine
        Inherits r2i.OWS.Framework.EngineBase

        Private _abortRendering As Boolean
        Private _isResponseCleared As Boolean = False
        Private _isResponseEnding As Boolean = False
        Private _OverridePaging As Boolean = False

        Private _requestType As Framework.EngineBase.RequestTypeEnum

        Public Overrides Property RequestType() As Framework.EngineBase.RequestTypeEnum
            Get
                Return _requestType
            End Get
            Set(ByVal value As Framework.EngineBase.RequestTypeEnum)
                _requestType = value
            End Set
        End Property

        Public Overrides WriteOnly Property AbortRendering() As Boolean
            Set(ByVal value As Boolean)
                _abortRendering = value
            End Set
        End Property
        Public Overrides Property EndResponse() As Boolean
            Get
                Return _isResponseEnding
            End Get
            Set(ByVal value As Boolean)
                _isResponseEnding = value
            End Set
        End Property



        Private _CachedTableRowIndexCollection As SortedList(Of String, Integer)
        Public Overrides Property CachedTableRowIndexCollection(ByVal TableName As String) As Integer
            Get
                If _CachedTableRowIndexCollection Is Nothing OrElse Not _CachedTableRowIndexCollection.ContainsKey(TableName) Then
                    Return 0
                Else
                    Return _CachedTableRowIndexCollection(TableName)
                End If
            End Get
            Set(ByVal Index As Integer)
                If _CachedTableRowIndexCollection Is Nothing Then
                    'ROMAIN: Generic replacement - 08/22/2007
                    '_ATRIndexCollection = New SortedList
                    _CachedTableRowIndexCollection = New SortedList(Of String, Integer)
                End If
                If Not _CachedTableRowIndexCollection.ContainsKey(TableName) Then
                    _CachedTableRowIndexCollection.Add(TableName, Index)
                Else
                    _CachedTableRowIndexCollection(TableName) = Index
                End If
            End Set
        End Property
        Public Overrides Property TotalRecords() As Integer
            Get
                If ViewState("TotalRecords" + Me.ClientID) Is Nothing Then
                    Return 0
                Else
                    Return CInt(ViewState("TotalRecords" + Me.ClientID))
                End If
            End Get
            Set(ByVal value As Integer)
                ViewState("TotalRecords" + Me.ClientID) = value
            End Set
        End Property
        Public Overrides WriteOnly Property CurrentPage() As Integer
            Set(ByVal value As Integer)
                PageCurrent = value
            End Set
        End Property
        Public Overrides Property OverridePaging() As Boolean
            Get
                Return _OverridePaging
            End Get
            Set(ByVal value As Boolean)
                _OverridePaging = value
            End Set
        End Property
        Public Overrides Property PageCurrent() As Integer
            Get
                If ViewState("CurrentPage" + Me.ClientID) Is Nothing Then
                    Return 0
                Else
                    Return CInt(ViewState("CurrentPage" + Me.ClientID))
                End If
            End Get
            Set(ByVal value As Integer)
                ViewState("CurrentPage" + Me.ClientID) = value
            End Set
        End Property
        Public Overrides Property TotalPages() As Integer
            Get
                If ViewState("TotalPages" + Me.ClientID) Is Nothing Then
                    Return -1
                Else
                    Return CInt(ViewState("TotalPages" + Me.ClientID))
                End If
            End Get
            Set(ByVal value As Integer)
                ViewState("TotalPages" + Me.ClientID) = value
            End Set
        End Property
        Public Overrides Property RecordsPerPage() As Integer
            Get
                If ViewState("RecordsPerPage" + Me.ClientID) Is Nothing Then
                    Return 0
                Else
                    Return CInt(ViewState("RecordsPerPage" + Me.ClientID))
                End If
            End Get
            Set(ByVal value As Integer)
                ViewState("RecordsPerPage" + Me.ClientID) = value
            End Set
        End Property


#Region "Internal - Single Usage Variables"
        Private _ActionVariables As SortedList(Of String, Object)

        Public Overrides Function ActionVariableSearch(ByVal Query As String) As String()
            Dim stck As New Stack(Of String)
            If Not _ActionVariables Is Nothing Then
                Dim strK As String
                For Each strK In _ActionVariables.Keys
                    If strK.ToUpper.StartsWith(Query.ToUpper) Then stck.Push(strK)
                Next
            End If
            Return stck.ToArray
        End Function

        Public Overrides ReadOnly Property ActionVariables() As SortedList(Of String, Object)
            Get
                Return _ActionVariables
            End Get
        End Property
        Public Overrides Property ActionVariable(ByVal Name As String) As Object
            Get
                If _ActionVariables Is Nothing OrElse _ActionVariables.ContainsKey(Name) = False Then
                    If Name.ToLower.EndsWith(".percent") Then
                        'Thread Process Action
                        Return ThreadedMessageHandler.GetProcess_Percentage(Left(Name, Name.Length - 8), Session.SessionID)
                    ElseIf Name.ToLower.EndsWith(".status") Then
                        Return ThreadedMessageHandler.GetProcess_Status(Left(Name, Name.Length - 7), Session.SessionID)
                    ElseIf Name.ToLower.EndsWith(".complete") Then
                        Return ThreadedMessageHandler.GetProcess_Complete(Left(Name, Name.Length - 9), Session.SessionID)
                    ElseIf Name.ToLower.EndsWith(".errors") Then
                        Return ThreadedMessageHandler.GetProcess_Errors(Left(Name, Name.Length - 7), Session.SessionID)
                    End If
                    Return Nothing
                Else
                    Return _ActionVariables.Item(Name)
                End If
            End Get
            Set(ByVal value As Object)
                If _ActionVariables Is Nothing Then
                    _ActionVariables = New SortedList(Of String, Object)
                End If

                If Not _ActionVariables.ContainsKey(Name) Then
                    _ActionVariables.Add(Name, value)
                Else
                    _ActionVariables.Item(Name) = value
                End If
            End Set
        End Property
#End Region
#Region "Protected Members"
        'Protected excelData As Text.StringBuilder
#End Region
        Public Overrides Function GetPostBackEventReference(ByVal Argument As String) As String
            If (EventReferenceControl Is Nothing) Then

                EventReferenceControl = New Web.UI.Control
                EventReferenceControl.ID = ClientID '.Replace("_", ":")
            End If
            Return (New Web.UI.Page).ClientScript.GetPostBackEventReference(EventReferenceControl, Argument)
        End Function

        ''' <summary>
        ''' Creates a new instance of the rendering engine
        ''' </summary>
        ''' <param name="Context"></param>
        ''' <param name="Caller"></param>
        ''' <param name="curUserInfo"></param>
        ''' <param name="View"></param>
        ''' <param name="ModuleSettingsHash"></param>
        ''' <param name="PortalSettingsObj"></param>
        ''' <param name="CapturedMessages"></param>
        ''' <param name="ModuleID"></param>
        ''' <param name="TabID"></param>
        ''' <param name="Config"></param>
        ''' <param name="ClientID">Unique ID for calling control. NOTE: if the platform requires different delimiters, you should set those BEFORE passing in this value</param>
        ''' <param name="ModulePath"></param>
        ''' <param name="DebugWriter"></param>
        ''' <param name="canEdit"></param>
        ''' <remarks></remarks>
        Public Sub New(ByRef Context As Web.HttpContext, ByVal iSession As Object, ByRef Caller As Framework.UI.Control, ByVal isAjax As Boolean, ByVal curUserInfo As IUser, ByRef View As System.Web.UI.StateBag, ByRef ModuleSettingsHash As Hashtable, ByRef PortalSettingsObj As IPortalSettings, ByRef CapturedMessages As SortedList(Of String, String), ByVal ModuleID As String, ByVal TabID As String, ByVal TabModuleID As String, ByVal ConfigurationId As Guid, ByRef Config As Settings, ByVal ClientID As String, ByVal ModulePath As String, ByRef DebugWriter As r2i.OWS.Framework.Debugger, ByVal canEdit As Boolean)
            xls = Config
            If TypeOf iSession Is Web.SessionState.HttpSessionState Then
                Session = New GenericSession(iSession)
            Else
                Session = iSession
            End If
            If Not Context Is Nothing Then
                Request = Context.Request
                Response = Context.Response
            End If

            Me.Context = Context
            ViewState = View
            Settings = ModuleSettingsHash
            PortalSettings = PortalSettingsObj
            Me.TabID = TabID
            Me.ModuleID = ModuleID
            Me.TabModuleID = TabModuleID
            Me.ClientID = ClientID
            Me.ConfigurationID = ConfigurationId
            CurrentUser = curUserInfo
            Me.ModuleIsEditable = canEdit

            If isAjax Then
                Me.RequestType = RequestTypeEnum.Ajax
            Else
                Me.RequestType = RequestTypeEnum.Page
            End If

            Me.Caller = Caller

            Me.CapturedMessages = CapturedMessages
            Me.mDebugWriter = DebugWriter

            If Not PortalSettings Is Nothing Then
                Me.PortalID = PortalSettings.PortalId
            End If
            Me.ModulePath = ModulePath

            Initialize()
        End Sub
        Public Overrides Property CurrentUser As Framework.DataAccess.IUser
            Get
                Return MyBase.UserInfo
            End Get
            Set(ByVal value As Framework.DataAccess.IUser)
                If Not value Is Nothing Then
                    MyBase.UserInfo = value
                    MyBase.UserID = MyBase.UserInfo.UserId
                Else
                    MyBase.UserInfo = Nothing
                    MyBase.UserID = -1
                End If
            End Set
        End Property
        'KMS - Added to reduce the memory utilization after disposal. This may cause problems with threaded OWS modules.
        Public Overloads Sub Dispose()
            _ActionVariables = Nothing
            _temporaryCache = Nothing
            Me.xls = Nothing
            Me.Context = Nothing
            Me.Request = Nothing
            Me.Response = Nothing
            Me.Session = Nothing
            Me.Settings = Nothing
            Me.TableVariables = Nothing
            Me.UserInfo = Nothing
            Me.ViewState = Nothing
            MyBase.Dispose()
        End Sub
        Private Sub ClearArray(ByRef Arr As Array)
            If Not Arr Is Nothing AndAlso Arr.Length > 0 Then
                Array.Clear(Arr, 0, Arr.Length)
            End If
        End Sub


#Region "Runtime Rendering Methods"
        Public Overrides Sub Initialize()
            ' State_Load("__OWS_PAGESTATE__")
            If Framework.Utilities.Utility.SortStatus(Me.Session, Me.ConfigurationID.ToString.Replace("{", "").Replace("}", "").Replace("-", ""), Me.ModuleID, Me.UserID) Is Nothing Then
                Prerender()
                If Framework.Utilities.Utility.SortStatus(Me.Session, Me.ConfigurationID.ToString.Replace("{", "").Replace("}", "").Replace("-", ""), Me.ModuleID, Me.UserID) Is Nothing Then
                    Framework.Utilities.Utility.SortStatus(Me.Session, Me.ConfigurationID.ToString.Replace("{", "").Replace("}", "").Replace("-", ""), Me.ModuleID, Me.UserID) = New List(Of SortAction)
                End If
            End If
            r2i.OWS.Framework.Entities.AbstractFactory.Instance.EngineController.Initialize()
        End Sub

        Private Sub Prerender()
            If Not xls.listItems Is Nothing AndAlso xls.listItems.Count > 0 Then
                Dim showHeaders(xls.listItems.Count - 1) As Boolean
                Dim contentHeaders(xls.listItems.Count - 1) As String
                Dim showFooters(xls.listItems.Count - 1) As Boolean
                Dim contentFooters(xls.listItems.Count - 1) As String
                Dim baseHeaderIndex As Integer = 1

                Dim stkHeaderIndex As New System.Collections.Generic.Stack(Of String)

                Dim startvalues As Char() = New Char() {"["c, "{"c}
                Dim endvalues As Char() = New Char() {"]"c, "}"c}
                Dim escapechar As Char = "\"c

                Dim i As Integer
                Dim content As String = ""
                Dim acontent As String = ""
                If Not xls.listItem Is Nothing Then
                    content = xls.listItem
                End If
                If Not xls.listAItem Is Nothing Then
                    acontent = xls.listAItem
                End If


                Dim ds As New DataSet
                Dim dt As New DataTable
                Dim dr As DataRow = dt.NewRow
                ds.Tables.Add(dt)

                Dim thisContent As String = content

                Render_ITEM(0, i, startvalues, endvalues, escapechar, thisContent, ds, dr, Nothing, False, True, Prerender:=True)

                Dim lFormatItem As ListFormatItem
                For Each lFormatItem In xls.listItems
                    Dim thisHeader As String = lFormatItem.ListHeader
                    Dim thisFooter As String = lFormatItem.ListFooter

                    Render_ITEM("", i, startvalues, endvalues, escapechar, thisHeader, ds, dr, Nothing, False, True, Prerender:=True)
                    Render_ITEM("", i, startvalues, endvalues, escapechar, thisFooter, ds, dr, Nothing, False, True, Prerender:=True)
                Next

                content = Nothing
                acontent = Nothing

                ClearArray(CType(showHeaders, Array))
                ClearArray(CType(showFooters, Array))
                ClearArray(CType(contentFooters, Array))

                stkHeaderIndex.Clear()
            End If
        End Sub
        Public Overrides Sub ExportData(ByRef DS As DataSet, ByVal QueryLength As Integer, ByVal PageSize As Integer, ByVal CurrentPage As Integer, ByVal CustomPaging As Boolean, ByVal Output As IO.TextWriter, ByRef RuntimeMessages As SortedList(Of String, String), Optional ByVal forceShowAll As Boolean = False, Optional ByVal Delimiter As String = ",")
            Dim dt As DataTable = Nothing
            If Not DS Is Nothing AndAlso DS.Tables.Count > 0 AndAlso Not DS.Tables(0) Is Nothing AndAlso Not DS.Tables(0).Rows Is Nothing AndAlso DS.Tables(0).Rows.Count > 0 Then
                dt = DS.Tables(0)

                Dim colindex As Integer = 0
                Dim i As Integer
                Dim startp As Integer
                Dim endp As Integer
                If xls.showAll OrElse forceShowAll Then
                    startp = 0
                    endp = dt.Rows.Count - 1
                ElseIf CustomPaging Then
                    startp = 0
                    endp = dt.Rows.Count - 1
                Else
                    startp = (CurrentPage - 1) * (PageSize)
                    endp = (CurrentPage * PageSize) - 1
                End If

                'Generate the Header Row with Column Names
                Dim col As DataColumn
                For colindex = 0 To dt.Columns.Count - 1
                    If colindex > 0 Then
                        Output.Write(Delimiter)
                    End If
                    col = dt.Columns(colindex)
                    Output.Write(col.ColumnName)
                Next
                Output.Write(Output.NewLine)

                'Generate the Record values
                For i = startp To endp
                    Dim dr As DataRow
                    If i >= dt.Rows.Count Then
                        Exit For
                    Else
                        dr = dt.Rows(i)
                    End If

                    For colindex = 0 To dt.Columns.Count - 1
                        If colindex > 0 Then
                            Output.Write(Delimiter)
                        End If
                        If Not IsDBNull(dr.Item(colindex)) Then
                            Output.Write("""" & CType(dr.Item(colindex), String).Replace(""""c, """""") & """")
                        End If
                    Next
                    Output.Write(Output.NewLine)
                Next
            End If

            'Clear Everything Up
            If Not dt Is Nothing Then
                dt.Dispose()
                dt = Nothing
            End If
        End Sub

        Private Class ListItemFormatRecall
            Public Formatter As ListFormatItem
            Public Statement As String
            Public Row As DataRow
            Public Index As Integer
        End Class
        Private Class ListItemFormatComparer
            Implements IComparer(Of ListFormatItem)

            Public Function Compare(ByVal x As ListFormatItem, ByVal y As ListFormatItem) As Integer Implements System.Collections.Generic.IComparer(Of ListFormatItem).Compare
                If IsNumeric(x.Index) AndAlso IsNumeric(y.Index) Then
                    Return CType(x.Index, Int32).CompareTo(CType(y.Index, Int32))
                Else
                    Return x.Index.CompareTo(y.Index)
                End If
            End Function
        End Class

        Public Overrides Sub Render(ByRef DS As DataSet, ByVal QueryLength As Integer, ByVal PageSize As Integer, ByVal CurrentPage As Integer, ByVal CustomPaging As Boolean, ByVal Output As IO.TextWriter, ByRef RuntimeMessages As SortedList(Of String, String), Optional ByVal forceShowAll As Boolean = False, Optional ByVal DebugWriter As Debugger = Nothing)
            Dim dt As DataTable = Nothing
            Static HeaderFooterSort As New ListItemFormatComparer
            Dim startvalues As Char() = New Char() {"["c, "{"c}
            Dim endvalues As Char() = New Char() {"]"c, "}"c}
            Dim escapechar As Char = "\"c

            If Not DS Is Nothing AndAlso DS.Tables.Count > 0 AndAlso Not DS.Tables(0) Is Nothing AndAlso Not DS.Tables(0).Rows Is Nothing AndAlso DS.Tables(0).Rows.Count > 0 Then
                dt = DS.Tables(0)
                Dim i As Integer
                Dim alternate As Boolean
                Dim content As String = ""
                Dim acontent As String = ""
                If Not xls.listItem Is Nothing Then
                    content = xls.listItem
                End If
                If Not xls.listAItem Is Nothing Then
                    acontent = xls.listAItem
                End If

                If CurrentPage < 0 Then CurrentPage = 0

                Dim startp As Integer
                Dim endp As Integer
                If xls.showAll OrElse forceShowAll OrElse PageSize = -1 Then
                    startp = 0
                    endp = dt.Rows.Count - 1
                ElseIf CustomPaging Then
                    startp = 0
                    endp = dt.Rows.Count - 1

                Else
                    startp = (CurrentPage * PageSize)
                    endp = ((CurrentPage + 1) * PageSize) - 1
                End If

                'BUILD THE GROUP RECALL (HEADER/FOOTER) ARRAY
                Dim GroupRecall As New List(Of ListItemFormatRecall)
                Dim objListItemFormat As ListFormatItem
                Dim GroupIndexGlobal As Integer = 0
                xls.listItems.Sort(HeaderFooterSort)
                For Each objListItemFormat In xls.listItems
                    Dim gr As New ListItemFormatRecall
                    gr.Formatter = objListItemFormat
                    gr.Row = Nothing
                    gr.Statement = Nothing
                    gr.Index = GroupIndexGlobal

                    GroupRecall.Add(gr)
                Next


                For i = startp To endp

                    Dim dr As DataRow
                    Dim thisContent As String = content

                    If alternate AndAlso Not acontent Is Nothing AndAlso acontent.Length > 0 Then
                        thisContent = acontent
                    End If
                    alternate = Not alternate
                    If i >= dt.Rows.Count Then
                        Exit For
                    Else
                        dr = dt.Rows(i)
                    End If

                    'GENERATE THE RENDERED OUTPUT FOR THE CURRENT ITEM
                    '                    Render_ITEM("", i, startvalues, endvalues, escapechar, thiscontent, DS, dr, RuntimeMessages, True)

                    'CALCULATE AND RENDER THE HEADER/FOOTERS
                    Dim oGroupRecallItem As ListItemFormatRecall
                    Dim oGroupRecallFooters As New List(Of String)
                    Dim oGroupRecallHeaders As New List(Of String)

                    'FIRST IDENTIFY THE FIRST DISTINCT LEVEL
                    Dim GroupRecallIndex As Integer
                    Dim bGroupRecallIdentified As Boolean = False
                    Dim StartGroupRecallIndex As Integer = GroupRecall.Count - 1
                    For GroupRecallIndex = 0 To GroupRecall.Count - 1
                        oGroupRecallItem = GroupRecall(GroupRecallIndex)
                        Dim thisGroupStatement As String = oGroupRecallItem.Formatter.GroupStatement
                        Render_ITEM("", i, startvalues, endvalues, escapechar, thisGroupStatement, DS, dr, RuntimeMessages, True, True, DebugWriter:=DebugWriter)

                        If oGroupRecallItem.Statement Is Nothing OrElse Not oGroupRecallItem.Statement = thisGroupStatement Then
                            'THIS IS UNIQUE
                            StartGroupRecallIndex = GroupRecallIndex
                            bGroupRecallIdentified = True
                        End If
                        If bGroupRecallIdentified Then
                            'ADD THE OLD FOOTER TO THE STACK
                            If Not oGroupRecallItem.Row Is Nothing Then
                                Dim oGroupFooterValue As String = oGroupRecallItem.Formatter.ListFooter
                                Render_ITEM("x" & oGroupRecallItem.Index, i, startvalues, endvalues, escapechar, oGroupFooterValue, DS, oGroupRecallItem.Row, RuntimeMessages, True, True, DebugWriter:=DebugWriter)
                                oGroupRecallFooters.Add(oGroupFooterValue)
                            End If

                            'RECALIBRATE THE NEW ITEM
                            GroupIndexGlobal += 1
                            oGroupRecallItem.Statement = thisGroupStatement
                            oGroupRecallItem.Row = dr
                            oGroupRecallItem.Index = GroupIndexGlobal

                            'GENERATE THE NEW HEADER
                            Dim oGroupHeaderValue As String = oGroupRecallItem.Formatter.ListHeader
                            Render_ITEM("x" & oGroupRecallItem.Index, i, startvalues, endvalues, escapechar, oGroupHeaderValue, DS, oGroupRecallItem.Row, RuntimeMessages, True, True, DebugWriter:=DebugWriter)
                            oGroupRecallHeaders.Add(oGroupHeaderValue)
                        End If
                    Next

                    'NOW ALL THE FOOTERS ARE AVAILABLE IN THEIR SPECIFIC AREAS - RENDER THEM
                    Dim strGroupFormat As String
                    oGroupRecallFooters.Reverse()
                    For Each strGroupFormat In oGroupRecallFooters
                        Output.Write(strGroupFormat)
                    Next
                    oGroupRecallFooters.Clear()

                    'NOW WRITE THE HEADERS
                    For Each strGroupFormat In oGroupRecallHeaders
                        Output.Write(strGroupFormat)
                    Next
                    oGroupRecallHeaders.Clear()


                    'NOW WRITE THE ITEM
                    Render_ITEM("x" & GroupIndexGlobal, i, startvalues, endvalues, escapechar, thisContent, DS, dr, RuntimeMessages, True, True, DebugWriter:=DebugWriter)
                    Output.Write(thisContent)
                    thisContent = Nothing
                Next


                'show all footers
                GroupRecall.Reverse()
                Dim oGroupRecallItemFooter As ListItemFormatRecall
                For Each oGroupRecallItemFooter In GroupRecall
                    Dim strFooterOutput As String = oGroupRecallItemFooter.Formatter.ListFooter
                    Render_ITEM("x" & oGroupRecallItemFooter.Index, i, startvalues, endvalues, escapechar, strFooterOutput, DS, oGroupRecallItemFooter.Row, RuntimeMessages, True, True, DebugWriter:=DebugWriter)
                    Output.Write(strFooterOutput)
                    strFooterOutput = Nothing
                Next

                content = Nothing
                acontent = Nothing
            Else
                'show the default item
                Dim thisContent As String = ""
                If Not xls.defaultItem Is Nothing AndAlso QueryLength > 0 Then
                    thisContent = xls.defaultItem
                ElseIf QueryLength = 0 AndAlso Not xls.noqueryItem Is Nothing Then
                    thisContent = xls.noqueryItem
                End If
                'Format_ItemActions(0, 0, thisContent, RuntimeMessages)
                Try
                    Render_ITEM(0, 0, startvalues, endvalues, escapechar, thisContent, DS, Nothing, RuntimeMessages, True, NullReturn:=True, DebugWriter:=DebugWriter)
                Catch ex As Exception
                    If Not DebugWriter Is Nothing Then
                        DebugWriter.AppendBlock(ModuleID, "Render Error", "Error", False, ex.ToString, True)
                    End If
                End Try

                Output.Write(thisContent)
                thisContent = Nothing
            End If

            If Not DebugWriter Is Nothing Then
                Try
                    'If Debug OrElse (errorDebug AndAlso thrownError) OrElse traceDebug Then
                    DebugWriter.AppendHeader(ModuleID, "Statistics", "statistics", False)

                    DebugWriter.AppendHeader(ModuleID, "Runtime", "statisticsitem", False)
                    DebugWriter.Append(ShowRuntimeItems("<li>", "</li>"))
                    DebugWriter.AppendFooter(False)

                    DebugWriter.AppendHeader(ModuleID, "Headers", "statisticsitem", False)
                    DebugWriter.Append(ShowHeaders("<li>", "</li>"))
                    DebugWriter.AppendFooter(False)

                    DebugWriter.AppendHeader(ModuleID, "Querystring", "statisticsitem", False)
                    DebugWriter.Append(ShowQuerystring("<li>", "</li>"))
                    DebugWriter.AppendFooter(False)

                    DebugWriter.AppendHeader(ModuleID, "Form", "statisticsitem", False)
                    DebugWriter.Append(ShowForm("<li>", "</li>"))
                    DebugWriter.AppendFooter(False)

                    DebugWriter.AppendHeader(ModuleID, "Session", "statisticsitem", False)
                    DebugWriter.Append(ShowSession("<li>", "</li>"))
                    DebugWriter.AppendFooter(False)

                    DebugWriter.AppendHeader(ModuleID, "Cookies", "statisticsitem", False)
                    DebugWriter.Append(ShowCookies("<li>", "</li>"))
                    DebugWriter.AppendFooter(False)

                    DebugWriter.AppendHeader(ModuleID, "ViewState", "statisticsitem", False)
                    DebugWriter.Append(ShowViewState("<li>", "</li>"))
                    DebugWriter.AppendFooter(False)

                    DebugWriter.AppendFooter(False)
                    'End If
                Catch ex As Exception
                    If Not DebugWriter Is Nothing Then
                        DebugWriter.AppendBlock(ModuleID, "Statistics Error", "Error", False, ex.ToString, True)
                    End If
                End Try
            End If

            'Clear alot of other stuff up...
            ClearTemporaryCache()

            'Clear Everything Up
            If Not dt Is Nothing Then
                If Not DS Is Nothing Then
                    If Not DS.ExtendedProperties.ContainsKey("isCached") Then
                        DS.Clear()
                        DS.Dispose()
                        DS = Nothing
                    End If
                End If
            End If
        End Sub
        'ROMAIN: Generic replacement - 08/22/2007
        'Private Sub Render_ITEM(ByVal HeaderIndex As String, ByVal Index As Integer, ByVal StartValues As Char(), ByVal EndValues As Char(), ByVal Escape As Char, ByRef Source As String, ByRef DS As DataSet, ByRef DR As DataRow, ByRef RuntimeMessages As SortedList, ByVal useAggregations As Boolean, Optional ByVal NullReturn As Boolean = False, Optional ByVal ProtectSession As Boolean = False, Optional ByVal SessionDelimiter As String = ",", Optional ByVal useSessionQuotes As Boolean = True, Optional ByRef Prerender As Boolean = False, Optional ByRef DebugWriter As Debugger = Nothing)
        Private Sub Render_ITEM(ByVal HeaderIndex As String, ByVal Index As Integer, ByVal StartValues As Char(), ByVal EndValues As Char(), ByVal Escape As Char, ByRef Source As String, ByRef DS As DataSet, ByRef DR As DataRow, ByRef RuntimeMessages As SortedList(Of String, String), ByVal useAggregations As Boolean, Optional ByVal NullReturn As Boolean = False, Optional ByVal ProtectSession As Boolean = False, Optional ByVal SessionDelimiter As String = ",", Optional ByVal useSessionQuotes As Boolean = True, Optional ByRef Prerender As Boolean = False, Optional ByRef DebugWriter As Debugger = Nothing)
            RenderString(Index, Source, StartValues, EndValues, Escape, DS, DR, RuntimeMessages, useAggregations, Prerender, NullReturn:=NullReturn, ProtectSession:=ProtectSession, SessionDelimiter:=SessionDelimiter, useSessionQuotes:=useSessionQuotes, DebugWriter:=DebugWriter)

            If Not HeaderIndex Is Nothing Then
                Source = Source.Replace("|!HEADERINDEX!|", HeaderIndex)
            End If
            'End If
        End Sub


        'Private Enum VariableTypeEnum
        '    Runtime
        '    Session
        '    QueryString
        '    ViewState
        '    Cookie
        '    ModuleSetting
        '    Message
        '    Form
        '    Table
        '    Action
        'End Enum

        'ROMAIN: Generic replacement - 08/22/2007
        'Private _temporaryCache As SortedList
        Private _temporaryCache As SortedList
        Private _cacheproperty As PropertyInfo
        Private _cacheExists As Boolean = True
        Public Overrides Property Cache(ByVal Key As String, ByVal useCache As Boolean, Optional ByVal CacheTime As Integer = 1200) As Object
            Get
                'CHECK TEMPORARY CACHE
                If Not useCache AndAlso Not _temporaryCache Is Nothing Then
                    If _temporaryCache.ContainsKey(Key) Then
                        Return _temporaryCache(Key)
                    End If
                End If
                useCache = True
                If useCache Then
                    Return Me.Context.Cache.Get(Key)
                End If
                Return Nothing
            End Get
            Set(ByVal Value As Object)
                If useCache = False Then
                    If _temporaryCache Is Nothing Then
                        'ROMAIN: Generic replacement - 08/22/2007
                        '_temporaryCache = New SortedList
                        _temporaryCache = New SortedList()
                    End If
                    If Not _temporaryCache.ContainsKey(Key) Then
                        _temporaryCache.Add(Key, Value)
                    Else
                        _temporaryCache.Item(Key) = Value
                    End If
                Else
                    If Me.Context.Cache.Get(Key) Is Nothing Then
                        If CacheTime < 0 Then
                            CacheTime = CacheTime * -1
                            Me.Context.Cache.Add(Key, Value, Nothing, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromSeconds(CacheTime), Caching.CacheItemPriority.BelowNormal, Nothing)
                        Else
                            Me.Context.Cache.Add(Key, Value, Nothing, DateTime.Now.AddSeconds(CacheTime), System.Web.Caching.Cache.NoSlidingExpiration, Caching.CacheItemPriority.BelowNormal, Nothing)
                        End If
                    Else
                        Me.Context.Cache.Item(Key) = Value
                    End If
                End If
            End Set
        End Property
        Private Sub ClearTemporaryCache()
            If Not _temporaryCache Is Nothing Then
                _temporaryCache.Clear()
                _temporaryCache = Nothing
            End If
            'TODO: This should also clear cache for any actions or renderers that need it 
            'If Not _subqueries Is Nothing Then
            '    _subqueries.Clear()
            '    _subqueries = Nothing
            'End If
        End Sub
        Public Overrides Sub ClearResponse()
            If Not _isResponseCleared Then
                _isResponseCleared = True
                _isResponseEnding = True
                Response.ClearHeaders()
                Response.Clear()
            End If
        End Sub
        Public Overrides Sub ClearResponse(ByVal resetTo As Boolean)
            If resetTo Then
                _isResponseCleared = False
                _isResponseEnding = False
                ClearResponse()
            Else
                _isResponseEnding = False
                _isResponseCleared = True
            End If
        End Sub
        Public Overrides Sub ClearHeaders()
            Response.ClearHeaders()
        End Sub
#Region "String Rendering"

        Public Overrides Function RenderString(ByRef DS As DataSet, ByVal Source As String, ByRef Messages As SortedList(Of String, String), ByVal useAggregations As Boolean, ByVal isPreRender As Boolean, Optional ByVal NullReturn As Boolean = True, Optional ByVal ProtectSession As Boolean = False, Optional ByVal SessionDelimiter As String = ",", Optional ByVal useSessionQuotes As Boolean = True, Optional ByRef FilterText As String = Nothing, Optional ByRef FilterField As String = Nothing, Optional ByRef DebugWriter As r2i.OWS.Framework.Debugger = Nothing, Optional ByVal Row As DataRow = Nothing) As String
            _abortRendering = False
            RenderString(0, Source, RenderBase.startvalues, RenderBase.endvalues, RenderBase.escapechar, DS, Row, Messages, useAggregations, isPreRender, NullReturn:=NullReturn, ProtectSession:=ProtectSession, SessionDelimiter:=SessionDelimiter, useSessionQuotes:=useSessionQuotes, FilterText:=FilterText, FilterField:=FilterField, DebugWriter:=DebugWriter)

            'STRIP ESCAPES
            Dim tArray(RenderBase.startvalues.Length + RenderBase.endvalues.Length) As Char
            Array.Copy(RenderBase.startvalues, tArray, RenderBase.startvalues.Length)
            Array.Copy(RenderBase.endvalues, 0, tArray, RenderBase.startvalues.Length, RenderBase.endvalues.Length)

            'TODO: Add Backwards Compatibility'
            If Not xls.enableAdvancedParsing Then
                RenderBase.StripEscapes(Source, tArray, RenderBase.escapechar)
            End If

            Firewall.Unfirewall(Source)
            Return Source
        End Function

        Public Overrides Function RenderString(ByVal Index As Integer, ByRef Source As String, ByRef StartValues As Char(), ByRef EndValues As Char(), ByRef EscapeChar As Char, ByRef DS As DataSet, ByRef DR As DataRow, ByRef RuntimeMessages As SortedList(Of String, String), ByVal useAggregations As Boolean, ByVal isPreRender As Boolean, Optional ByVal NullReturn As Boolean = False, Optional ByVal ProtectSession As Boolean = False, Optional ByVal SessionDelimiter As String = ",", Optional ByVal useSessionQuotes As Boolean = True, Optional ByRef FilterText As String = Nothing, Optional ByRef FilterField As String = Nothing, Optional ByRef DebugWriter As Debugger = Nothing) As Boolean
            _abortRendering = False
            'HANDLES THE FORMATTING OF A STRING
            Dim thisPosition As ParserBase.Positional = RenderBase.Match(Source, Nothing, StartValues, EndValues, EscapeChar)

            Dim b As Boolean = False
            While Not thisPosition Is Nothing And Not _abortRendering
                If RenderString(Index, Source, thisPosition, DS, DR, RuntimeMessages, useAggregations, isPreRender, NullReturn:=NullReturn, ProtectSession:=ProtectSession, SessionDelimiter:=SessionDelimiter, useSessionQuotes:=useSessionQuotes, FilterText:=FilterText, FilterField:=FilterField, DebugWriter:=DebugWriter) Then
                    b = True
                    thisPosition = Nothing
                End If
                thisPosition = RenderBase.Match(Source, thisPosition, StartValues, EndValues, EscapeChar)
            End While

            RenderBase.StripEscapes(Source)
            Firewall.Unfirewall(Source)
            Return b
        End Function
        Protected Overrides Function RenderString(ByVal Index As Integer, ByRef Source As String, ByVal Position As FormatterBase.Positional, ByRef DS As DataSet, ByRef DR As DataRow, ByRef RuntimeMessages As SortedList(Of String, String), ByVal useAggregations As Boolean, ByVal isPreRender As Boolean, Optional ByVal NullReturn As Boolean = False, Optional ByVal ProtectSession As Boolean = False, Optional ByVal SessionDelimiter As String = ",", Optional ByVal useSessionQuotes As Boolean = True, Optional ByRef FilterText As String = Nothing, Optional ByRef FilterField As String = Nothing, Optional ByRef DebugWriter As Debugger = Nothing) As Boolean
            _abortRendering = False
            Dim b As Boolean = False
            Dim bFirewall As Boolean = False
            Dim rValue As String = ""
            If Position.Starting + 1 < Position.Ending Then rValue = Source.Substring(Position.Starting + 1, Position.Ending - 1 - Position.Starting)
            If Not rValue Is Nothing AndAlso rValue.Length > 0 AndAlso rValue(0) = "|"c Then
                If rValue.Length > 1 Then
                    bFirewall = True
                    rValue = rValue.Remove(0, 1)
                End If
            End If
            Dim sTag As String = ""
            If rValue.IndexOf(",") > 0 Then sTag = rValue.Substring(0, rValue.IndexOf(","))
            Dim r As RenderBase
            Select Case Position.StartChar
                Case "["c
                    ' r = Common.GetRenderer(sTag.ToUpper, RenderTypes.Variable)
                    r = Plugins.Manager.GetPlugin(Plugins.PluginTag.Create(Config.Section.Tokens.ToString.ToLower, RenderTypes.Variable.ToString.ToLower, sTag.ToUpper))
                    If r Is Nothing Then
                        'r = Common.GetRenderer("", RenderTypes.Variable)
                        r = Plugins.Manager.GetPlugin(Plugins.PluginTag.Create(Config.Section.Tokens.ToString.ToLower, RenderTypes.Variable.ToString.ToLower, ""))
                    End If
                Case Else
                    ' r = Common.GetRenderer(sTag.ToUpper, RenderTypes.Functional)
                    r = Plugins.Manager.GetPlugin(Plugins.PluginTag.Create(Config.Section.Tokens.ToString.ToLower, RenderTypes.Functional.ToString.ToLower, sTag.ToUpper))
            End Select

            If Not r Is Nothing Then
                Try
                    b = r.Handle_Render(Me, Index, rValue, DS, DR, RuntimeMessages, NullReturn, True, ProtectSession, SessionDelimiter, useSessionQuotes, useAggregations, FilterText, FilterField, DebugWriter)
                Catch ex As Exception
                    If Not DebugWriter Is Nothing Then
                        DebugWriter.AppendHeader(Me.ModuleID, "Rendering Error", "error", False)
                        DebugWriter.AppendLine("<li><b>Tag Not Found Exception:</b> Cannot Handle Token '" & rValue & "' properly. " & ex.ToString)
                        DebugWriter.AppendFooter(False)
                    End If
                End Try
            End If

            If bFirewall Then
                Firewall.Firewall(rValue, True, Firewall.FirewallDirectiveEnum.None, False)
            End If

            If b Then
                Source = Source.Substring(0, Position.Starting) + rValue + Source.Substring(Position.Ending + 1)
            End If
            Return b
        End Function
        'TODO: Call this a
        Public Overrides Function RenderString_Assignment_Assign(ByVal Name As String, ByVal Value As String, ByVal Location As String, ByRef Source As String, ByRef bSystemParse As Boolean) As Boolean
            'VERSION: 2.0 - RenderString - Separate Assignment for use elsewhere - NEW FUNCTION.
            Dim b As Boolean
            Select Case Location.ToUpper
                Case "MO", "MODULESETTING"
                    If Settings.ContainsKey(Name) Then
                        Settings(Name) = Value
                    Else
                        Settings.Add(Name, Value)
                    End If
                    Try
                        'Dim mc As New DotNetNuke.Entities.Modules.ModuleController
                        'AbstractFactory.Instance.EngineController.UpdateModuleSetting(Me.ModuleID, name, value)
                        Dim mdC As IModuleController = AbstractFactory.Instance.ModuleController
                        mdC.UpdateModuleSetting(Me.ModuleID, Name, Value)
                        mdC = Nothing
                    Catch ex As Exception
                    End Try
                    Source = ""
                    b = True
                Case "SY", "SYSTEM"
                    bSystemParse = True
                Case "M", "ME", "MESSAGE"
                    'CANNOT ASSIGN TO A MESSAGE - YET.
                    b = False
                Case "S", "SESSION"
                    If Session(Name) Is Nothing Then
                        Session.Add(Name, Value)
                    Else
                        Session(Name) = Value
                    End If
                    Source = ""
                    b = True
                Case "V", "VIEWSTATE"
                    If ViewState.Item(Name) Is Nothing Then
                        ViewState.Add(Name, Value)
                    Else
                        ViewState(Name) = Value
                    End If
                    Source = ""
                    b = True
                Case "F", "FORM"
                    b = False
                Case "Q", "QUERYSTRING"
                    Me.Response.RedirectLocation = Value
                    Source = ""
                    b = True
                Case "C", "COOKIE"
                    If Not Response.Cookies.Item(Name) Is Nothing Then
                        Response.Cookies.Item(Name).Value = Value
                    Else
                        Response.Cookies.Add(New Web.HttpCookie(Name, Value))
                    End If
                    Source = ""
                    b = True
                Case "CON", "CFG", "CONFIGURATION", "APPSETTINGS"
                    'NOT SUPPORTED
                    Source = ""
                    b = True
                Case "A", "ACTION"
                    ActionVariable(Name) = Value
                    Source = ""
                    b = True
            End Select
            Return b
        End Function
#End Region



        Private Sub Replacer(ByRef source As String, ByVal replacing As String, ByVal replacement As String)
            'CHECK FOR FORMATS
            Dim istart As Integer
            Dim starter As String = replacing
            istart = source.ToUpper.IndexOf(starter)

            While istart >= 0
                Dim xlength As Integer = (starter).Length
                Dim fvalue As String
                If Not replacement Is Nothing Then
                    fvalue = replacement
                Else
                    fvalue = ""
                End If
                source = source.Substring(0, istart) & fvalue & source.Substring(istart + xlength)
                If istart + 1 < source.Length Then
                    istart = source.ToUpper.IndexOf(starter, istart + 1)
                Else
                    istart = -1
                End If
            End While
        End Sub


        Public Overrides Function ModuleImageURL(ByVal Src As String) As String
            If Me.ModulePath.EndsWith("/") Or Me.ModulePath.EndsWith("\") Then
                Return Me.ModulePath & Src
            Else
                Return Me.ModulePath & "/" & Src
            End If
        End Function

        Public Overrides Function ClearCache() As Boolean
            If Not Context.Cache Is Nothing Then
                If Context.Cache.Count > 0 Then
                    Dim enumerator As System.Collections.IDictionaryEnumerator = Context.Cache.GetEnumerator
                    If Not enumerator Is Nothing Then
                        While enumerator.MoveNext()
                            Context.Cache.Remove(enumerator.Key)
                        End While
                    End If
                End If
            End If
            Try
                Caller.ClearCache()
            Catch ex As Exception

            End Try
        End Function
        Public Overrides Function ClearSiteCache() As Boolean
            Try
                Caller.ClearSiteCache()
            Catch ex As Exception
            End Try
        End Function
        Public Overrides Function ClearPageCache() As Boolean
            Try
                Caller.ClearPageCache()
            Catch ex As Exception

            End Try
        End Function
        Public Overrides ReadOnly Property EditUserInfo() As IUser
            Get
                Dim eUser As IUser = ActionVariable("~!UserInformation!~")
                Return eUser
            End Get
        End Property
#End Region

        'TODO: STRIP ESCAPED VALUE 

#Region "Data Engine"
        Private Function CheckSum(ByVal Value As String) As Double
            Dim i As Integer = 0
            Dim sum As Double = 0
            If Not Value Is Nothing AndAlso Value.Length > 0 Then
                For i = 0 To Value.Length - 1
                    sum += Char.GetNumericValue(Value, i)
                Next
            End If
            Return sum
        End Function
        Private Function Verify_CheckSum(ByVal Value As String, ByVal Verification As Double) As Boolean
            Dim i As Integer = 0
            Dim sum As Double = 0
            If Not Value Is Nothing AndAlso Value.Length > 0 Then
                For i = 0 To Value.Length - 1
                    sum += Char.GetNumericValue(Value, i)
                    If sum > Verification Then
                        Exit For
                    End If
                Next
            End If
            If sum = Verification Then
                Return True
            End If
            Return False
        End Function

        Public Overrides Function RenderQuery(ByRef SharedDS As DataSet, ByVal FilterField As String, ByVal FilterText As String, ByVal RecordsPerPage As Integer, ByVal CapturedMessages As SortedList(Of String, String), ByRef DebugWriter As r2i.OWS.Framework.Debugger, Optional ByVal Query As String = Nothing) As String
            Dim debug As Boolean = False
            If Not DebugWriter Is Nothing Then
                debug = True
            End If

            If FilterField Is Nothing Then
                FilterField = ""
            End If
            If FilterText Is Nothing Then
                FilterText = ""
            End If
            Me.RecordsPerPage = RecordsPerPage
            Try
                If Not xls Is Nothing AndAlso (Not xls.query Is Nothing OrElse Not Query Is Nothing) Then
                    Dim sqlStatement As String = xls.query
                    If Not Query Is Nothing Then
                        sqlStatement = Query
                    End If
                    If debug Then
                        DebugWriter.AppendBlock(Me.ModuleID, "Original", "query_original", True, sqlStatement, True)
                    End If

                    If xls.enableAdvancedParsing Then
                        'THIS HAS BEEN MODIFIED BECAUSE YOU CANNOT RERENDER THE CONTENTS OF A VARIABLE - THIS DEFEATS THE PURPOSE OF THE RENDERING SETTINGS AND ESCAPING...
                        'NEW
                        'FIRST RENDER EVERYTHING THAT IS A STANDARD SYNTAX
                        Dim initialValue As String = sqlStatement
                        Dim initialLength As Integer = sqlStatement.Length
                        sqlStatement = RenderString(SharedDS, sqlStatement, CapturedMessages, False, False, , , , , FilterText, FilterField)
                        sqlStatement = Renderers.RenderVariable.ReplaceCommonVariables(Me, CapturedMessages, sqlStatement, FilterText, FilterField, DebugWriter)

                        'FINALLY RENDER THE VARIABLES
                    Else
                        sqlStatement = Renderers.RenderVariable.ReplaceCommonVariables(Me, CapturedMessages, sqlStatement, FilterText, FilterField, DebugWriter)
                        sqlStatement = RenderString(SharedDS, sqlStatement, CapturedMessages, False, False, , , , , FilterText, FilterField)
                    End If

                    Dim errstr As String = Nothing
                    Try
                        If debug Then
                            DebugWriter.AppendBlock(Me.ModuleID, "Actual", "query_actual", True, sqlStatement, True)
                        End If
                    Catch ex As Exception
                    End Try
                    Return sqlStatement
                End If
            Catch ex As Exception
                'ROMAIN: 09/18/07
                'TODO: CHANGE EXCEPTIONS
                'DotNetNuke.Services.Exceptions.LogException(ex)
            End Try
            Return Nothing
        End Function

        Private Function Get_Cached_Data(ByVal name As String, ByVal isShared As Boolean) As DataSet
            If Not isShared Then
                Return CType(Cache("ows." & ConfigurationID.ToString & ".query." & name, True), DataSet)
            Else
                Return CType(Cache("ows.shared.query." & name, True), DataSet)
            End If
        End Function
        Private Sub Set_Cached_Data(ByVal name As String, ByVal isShared As Boolean, ByVal Value As DataSet, Optional ByVal cachetime As Integer = 0)
            If Not isShared Then
                Cache("ows." & ConfigurationID.ToString & ".query." & name, True, cachetime) = Value
            Else
                Cache("ows.shared.query." & name, True, cachetime) = Value
            End If
        End Sub

        Public Overrides Function GetData(ByVal isSubQuery As Boolean, ByVal Query As String, ByVal FilterField As String, ByVal FilterText As String, ByRef DebugWriter As Debugger, ByVal isRendered As Boolean, ByVal CacheName As String, ByVal CacheTime As String, ByVal isCacheShared As Boolean, Optional ByVal timeout As Integer = -1, Optional ByVal CustomConnection As String = Nothing, Optional ByRef IsSuccessful As Boolean = True, Optional ByRef FailureMessage As String = Nothing) As DataSet
            Dim queryType As String = "<DATABASE>"
            Dim rslt As Framework.RuntimeBase.QueryResult
            Dim cached As Boolean = False
            If Not Query Is Nothing AndAlso Query.Length > 1 Then
                If Query.Substring(0, 1) = "<" Then
                    'get first block
                    Dim firstBlock As Integer = 50
                    If Query.Length > 1 AndAlso Query.Length < 50 Then
                        firstBlock = Query.Length - 1
                    End If
                    Dim i As Integer = Query.IndexOf(">", 0, firstBlock)
                    If i > 0 Then
                        queryType = Query.Substring(0, i + 1)
                    End If
                End If
                Dim qb As QueryBase = Plugins.Manager.GetPlugin(Plugins.PluginTag.Create(Config.Section.Queryies.ToString.ToLower, "", queryType)) 'Common.GetQuery(queryType)
                If Not qb Is Nothing Then
                    If Not CacheName Is Nothing AndAlso Not CacheTime Is Nothing AndAlso IsNumeric(CacheTime) AndAlso CInt(CacheTime) > 0 Then
                        rslt = New Framework.RuntimeBase.QueryResult(RuntimeBase.ExecutableResultEnum.NotExecuted, Get_Cached_Data(CacheName, isCacheShared))
                        If rslt.Value Is Nothing Then
                            rslt = qb.Handle_GetData(Me, isSubQuery, Query, FilterField, FilterText, DebugWriter, isRendered, timeout, CustomConnection)
                            If rslt.Result = RuntimeBase.ExecutableResultEnum.Executed Then
                                IsSuccessful = True
                            Else
                                IsSuccessful = False
                                If Not rslt.Error Is Nothing Then
                                    FailureMessage = rslt.Error.Message
                                End If
                            End If
                            If Not rslt.Value Is Nothing Then
                                cached = True
                                If Not rslt.Value.ExtendedProperties.ContainsKey("isCached") Then
                                    rslt.Value.ExtendedProperties.Add("isCached", True)
                                End If
                                Set_Cached_Data(CacheName, isCacheShared, rslt.Value, CacheTime)
                            End If
                        Else
                            IsSuccessful = True
                        End If
                    Else
                        rslt = qb.Handle_GetData(Me, isSubQuery, Query, FilterField, FilterText, DebugWriter, isRendered, timeout, CustomConnection)
                        If rslt.Result = RuntimeBase.ExecutableResultEnum.Executed Then
                            IsSuccessful = True
                        Else
                            IsSuccessful = False
                            If Not rslt.Error Is Nothing Then
                                FailureMessage = rslt.Error.Message
                            End If
                        End If
                    End If
                Else
                    If Not DebugWriter Is Nothing Then
                        DebugWriter.AppendHeader(Me.ModuleID, "Query Error", "Query", False)
                        DebugWriter.AppendLine("Cannot Handle " & Query.Substring(0, 50))
                        DebugWriter.AppendFooter(False)
                    End If
                    rslt = New Framework.RuntimeBase.QueryResult(RuntimeBase.ExecutableResultEnum.Aborted, Nothing, New Exception("No plugin was located with the required query type: " & queryType))
                End If
            Else
                rslt = New Framework.RuntimeBase.QueryResult(RuntimeBase.ExecutableResultEnum.Aborted, Nothing, New Exception("No query was provided"))
            End If
            HandleDataResult(rslt, cached, FilterField, FilterText, isSubQuery, DebugWriter)
            If Not rslt.Value Is Nothing Then
                Return rslt.Value
            Else
                Return Nothing
            End If
        End Function
        Private Sub HandleDataResult(ByRef rslt As r2i.OWS.Framework.RuntimeBase.QueryResult, ByVal cached As Boolean, ByVal FilterField As String, ByVal FilterText As String, ByVal isSubQuery As Boolean, ByRef DebugWriter As Debugger)
            Dim debug As Boolean = False
            Dim traceDebug As Boolean = xls.enableQueryDebug_Log
            Dim errorDebug As Boolean = xls.enableQueryDebug_ErrorLog
            Dim thrownError As Boolean = False
            Dim isSuccessful As Boolean = True
            If Not DebugWriter Is Nothing Then
                debug = True
            Else
                If traceDebug Then
                    If DebugWriter Is Nothing Then
                        DebugWriter = New r2i.OWS.Framework.Debugger
                    End If
                End If
            End If
            If rslt.Result = RuntimeBase.ExecutableResultEnum.Failed Then
                isSuccessful = False
            End If
            If rslt.Result = RuntimeBase.ExecutableResultEnum.Failed AndAlso Not rslt.Error Is Nothing Then
                thrownError = True
                isSuccessful = False
                If thrownError AndAlso (debug OrElse (errorDebug AndAlso thrownError) OrElse traceDebug) AndAlso DebugWriter Is Nothing Then
                    DebugWriter = New r2i.OWS.Framework.Debugger
                End If

                Try
                    If debug OrElse (errorDebug AndAlso thrownError) OrElse traceDebug Then
                        DebugWriter.AppendBlock(ModuleID, "Error", "error", False, rslt.Error.Message, True)
                    End If
                    'TODO: ROMAIN IMPLEMENT ERROR LOGGING
                    'DotNetNuke.Services.Exceptions.LogException((New Exception("ListX Execute Action Error: " & errorStr)))
                Catch ex As Exception
                End Try
            Else
                Try
                    If debug OrElse (errorDebug AndAlso thrownError) OrElse traceDebug Then
                        DebugWriter.AppendHeader(ModuleID, "Tables", "tables", False)
                        Try
                            Dim dbgX As New Text.StringBuilder
                            If rslt.Result = RuntimeBase.ExecutableResultEnum.NotExecuted Then
                                dbgX.Append("<b>Loaded from Cache</b>")
                            End If
                            If cached Then
                                dbgX.Append("<b>Stored in Cache</b>")
                            End If

                            Dim tbl As DataTable
                            For Each tbl In rslt.Value.Tables
                                dbgX.Append(tbl.TableName & " - <i>&nbsp;Rows:</i>&nbsp;<b>" & tbl.Rows.Count & "</b>&nbsp;<i>Columns:</i>&nbsp;<b>" & tbl.Columns.Count & "</b><br><ul>")
                                Dim col As DataColumn
                                For Each col In tbl.Columns
                                    dbgX.Append("<li><b>" & col.ColumnName & "</b>&nbsp;<i>" & col.DataType.ToString & "</i></li>")
                                Next
                                dbgX.Append("</ul><br>")
                            Next
                            DebugWriter.Append(dbgX.ToString)
                        Catch ex As Exception
                        End Try
                        DebugWriter.AppendFooter(False)
                    End If
                Catch ex As Exception
                End Try
            End If

            If Not isSubQuery Then
                Try
                    'Handle custom paging if required here
                    If Not rslt.Value Is Nothing AndAlso rslt.Value.Tables.Count > 0 Then
                        If xls.enableCustomPaging = True Then
                            'CHECK FOR TABLE 1 COLUMN 0 ROW 0
                            If rslt.Value.Tables.Count > 1 Then
                                'WE HAVE OUR TABLE
                                If rslt.Value.Tables(1).Rows.Count > 0 Then
                                    'WE HAVE OUR ROW
                                    Try
                                        If Not IsDBNull(rslt.Value.Tables(1).Rows(0).Item(0)) Then
                                            TotalRecords = rslt.Value.Tables(1).Rows(0).Item(0)
                                        Else
                                            TotalRecords = rslt.Value.Tables(0).Rows.Count
                                        End If
                                    Catch ex As Exception
                                    End Try
                                Else
                                    TotalRecords = rslt.Value.Tables(0).Rows.Count
                                End If
                            Else
                                TotalRecords = rslt.Value.Tables(0).Rows.Count
                            End If
                        Else
                            TotalRecords = rslt.Value.Tables(0).Rows.Count
                        End If
                        'VERSION: 1.8.1 - Now sets the TotalPages value
                        If TotalRecords > 0 Then
                            If RecordsPerPage > 0 Then
                                TotalPages = Math.Ceiling(CDbl(TotalRecords) / CDbl(RecordsPerPage))
                            Else
                                TotalPages = 1
                            End If
                        Else
                            TotalPages = 0
                        End If
                    Else
                        TotalRecords = 0
                        TotalPages = 0
                    End If

                Catch ex As Exception
                    thrownError = True
                    If thrownError AndAlso (debug OrElse (errorDebug AndAlso thrownError) OrElse traceDebug) AndAlso DebugWriter Is Nothing Then
                        DebugWriter = New r2i.OWS.Framework.Debugger
                    End If

                    If debug OrElse (errorDebug AndAlso thrownError) OrElse traceDebug Then
                        DebugWriter.AppendBlock(ModuleID, "Exception", "exception", False, ex.ToString, True)
                    End If
                End Try
            End If

            Try
                If (errorDebug AndAlso thrownError) OrElse traceDebug Then
                    If Not debug Then
                        DebugWriter.AppendHeader(ModuleID, "Statistics", "statistics", False)

                        DebugWriter.AppendHeader(ModuleID, "Runtime", "statisticsitem", False)
                        DebugWriter.Append(ShowRuntimeItems("<li>", "</li>"))
                        DebugWriter.AppendFooter(False)

                        DebugWriter.AppendHeader(ModuleID, "Headers", "statisticsitem", False)
                        DebugWriter.Append(ShowHeaders("<li>", "</li>"))
                        DebugWriter.AppendFooter(False)

                        DebugWriter.AppendHeader(ModuleID, "Querystring", "statisticsitem", False)
                        DebugWriter.Append(ShowQuerystring("<li>", "</li>"))
                        DebugWriter.AppendFooter(False)

                        DebugWriter.AppendHeader(ModuleID, "Form", "statisticsitem", False)
                        DebugWriter.Append(ShowForm("<li>", "</li>"))
                        DebugWriter.AppendFooter(False)

                        DebugWriter.AppendHeader(ModuleID, "Session", "statisticsitem", False)
                        DebugWriter.Append(ShowSession("<li>", "</li>"))
                        DebugWriter.AppendFooter(False)

                        DebugWriter.AppendHeader(ModuleID, "Cookies", "statisticsitem", False)
                        DebugWriter.Append(ShowCookies("<li>", "</li>"))
                        DebugWriter.AppendFooter(False)

                        DebugWriter.AppendHeader(ModuleID, "ViewState", "statisticsitem", False)
                        DebugWriter.Append(ShowViewState("<li>", "</li>"))
                        DebugWriter.AppendFooter(False)

                        DebugWriter.AppendFooter(False)

                        If (errorDebug AndAlso thrownError) OrElse traceDebug Then
                            If traceDebug Then
                                Controller.AddLog(ConfigurationID, UserID, "Automated Debug - Trace", DebugWriter.ToString, Session.SessionID)
                            ElseIf (errorDebug AndAlso thrownError) Then
                                Controller.AddLog(ConfigurationID, UserID, "Automated Debug - Query Error", DebugWriter.ToString, Session.SessionID)
                            End If

                            If Not debug Then
                                DebugWriter.Close()
                                DebugWriter = Nothing
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception

            End Try
        End Sub

        Public Overrides Sub RenderJsonObject(ByVal o As Object, ByVal ds As DataSet, ByVal columnList As String)
            Dim dt As DataTable = ds.Tables(0)
            For Each col As String In r2i.OWS.Framework.Utilities.JSON.JsonDataTable.GetColumns(columnList)
                dt.Columns.Add(col)
            Next
            If o.[GetType]() Is GetType(JavaScriptArray) Then
                For Each row As Object In DirectCast(o, JavaScriptArray)
                    Dim rowJ As JavaScriptObject = DirectCast(row, JavaScriptObject)
                    r2i.OWS.Framework.Utilities.JSON.JsonDataTable.GenerateRow(rowJ, columnList, dt)
                Next
            ElseIf o.[GetType]() Is GetType(JavaScriptObject) Then
                Dim rowJ As JavaScriptObject = DirectCast(o, JavaScriptObject)
                r2i.OWS.Framework.Utilities.JSON.JsonDataTable.GenerateRow(rowJ, columnList, dt)
            End If
        End Sub
#End Region
#Region "Runtime Debugging"
        Public Overrides Function ShowRuntimeItems(ByVal header As String, ByVal footer As String) As String
            Dim str As String = ""
            Try

                str &= header & "<b>PortalId</b>&nbsp;" & PortalID & footer
                str &= header & "<b>TabId</b>&nbsp;" & TabID & footer
                str &= header & "<b>ModuleId</b>&nbsp;" & ModuleID & footer
                str &= header & "<b>TabModuleId</b>&nbsp;" & TabModuleID & footer
                str &= header & "<b>Filter</b>&nbsp;" & xls.filter & footer
                str &= header & "<b>Current  Page</b>&nbsp;" & (PageCurrent + 1) & footer
                str &= header & "<b>Page Size</b>&nbsp;" & RecordsPerPage & footer
                str &= header & "<b>Total Records</b>&nbsp;" & TotalRecords & footer


            Catch ex As Exception
                str &= header & "<b>Runtime Error</b>:&nbsp; <span style=""color: red;"">Failed to populate general statistics.</span>" & footer
            End Try
            Return str
        End Function
        Public Overrides Function ShowQuerystring(ByVal header As String, ByVal footer As String) As String
            Dim str As String = ""
            Dim key As String
            For Each key In Request.QueryString.AllKeys
                Try
                    str &= header & "<b>" & key & "</b>:&nbsp;" & Utility.HTMLEncode(Utility.CleanControlCharacters(Request.QueryString.Item(key))) & footer
                Catch ex As Exception
                    str &= header & "<b>" & key & "</b>:&nbsp; <span style=""color: red;"">Error</span>" & footer
                End Try
            Next
            Return str
        End Function
        Public Overrides Function ShowForm(ByVal header As String, ByVal footer As String) As String
            Dim str As String = ""
            Dim key As String
            For Each key In Request.Form.AllKeys
                Try
                    str &= header & "<b>" & key & "</b>:&nbsp;" & Utility.HTMLEncode(Utility.CleanControlCharacters(Request.Form.Item(key))) & footer
                Catch ex As Exception
                    str &= header & "<b>" & key & "</b>:&nbsp; <span style=""color: red;"">Error</span>" & footer
                End Try
            Next
            If Not Request.Files Is Nothing AndAlso Not Request.Files.Keys Is Nothing AndAlso Request.Files.Keys.Count > 0 Then
                Dim i As Integer
                For i = 0 To Request.Files.Count - 1
                    Try
                        str &= header & "<b>File[" & i.ToString & "] " & Request.Files.GetKey(i) & "</b>:&nbsp;" & ShowFile(Request.Files(i)) & footer
                    Catch ex As Exception
                        str &= header & "<b>File[" & i.ToString & "]</b>:&nbsp; <span style=""color: red;"">Error: " & ex.ToString & "</span>" & footer
                    End Try
                    i += 1
                Next
            End If
            Return str
        End Function
        Public Overrides Function ShowViewState(ByVal header As String, ByVal footer As String) As String
            Dim str As String = ""
            Dim key As String
            For Each key In ViewState.Keys
                Try
                    str &= header & "<b>" & key & "</b>:&nbsp;" & Utility.HTMLEncode(ViewState.Item(key)) & footer
                Catch ex As Exception
                    str &= header & "<b>" & key & "</b>:&nbsp; <span style=""color: red;"">Unable to parse value.</span>" & footer
                End Try
            Next
            Return str
        End Function
        Public Overrides Function ShowHeaders(ByVal header As String, ByVal footer As String) As String
            Dim str As String = ""
            Dim key As String
            For Each key In Context.Request.Headers
                Try
                    str &= header & "<b>" & key & "</b>:&nbsp;" & Utility.HTMLEncode(Context.Request.Headers.Item(key)) & footer
                Catch ex As Exception
                    str &= header & "<b>" & key & "</b>:&nbsp; <span style=""color: red;"">Unable to parse value.</span>" & footer
                End Try
            Next
            Return str
        End Function
        Public Overrides Function ShowCookies(ByVal header As String, ByVal footer As String) As String
            Dim str As String = ""
            Dim key As String
            For Each key In Context.Request.Cookies.AllKeys
                Try
                    str &= header & "<b>" & key & "</b>:&nbsp;" & Utility.HTMLEncode(Context.Request.Cookies.Item(key).Value) & footer
                Catch ex As Exception
                    str &= header & "<b>" & key & "</b>:&nbsp; <span style=""color: red;"">Unable to parse value.</span>" & footer
                End Try
            Next
            Return str
        End Function
        Public Overrides Function ShowFile(ByRef Value As System.Web.HttpPostedFile) As String
            Dim str As String = "<ul><li>Type:[TYPE]</li><li>Name:[NAME]</li><li>Path:[PATH]</li><li>Size:[SIZE]</li></ul>"
            Try
                str = str.Replace("[NAME]", Value.FileName).Replace("[SIZE]", Value.ContentLength.ToString).Replace("[TYPE]", Value.ContentType).Replace("[PATH]", IO.Path.GetDirectoryName(Value.FileName))
            Catch ex As Exception
                str = "Unable to extract file information: " & ex.ToString()
            End Try
            Return str
        End Function
        Public Overrides Function ShowSession(ByVal header As String, ByVal footer As String) As String
            Dim str As String = ""
            Dim key As String
            'TODO: Remove Keys used for the singleton?CurrentListXPortalModuleBase + 
            For Each key In Session.Keys
                Try
                    'If TypeOf Session.Item(key) Is ArrayList Then

                    If TypeOf Session.Item(key) Is List(Of String) Then
                        'ROMAIN: Generic replacement - 08/21/2007
                        'TODO: Check how does it work
                        'Dim arr As ArrayList = Session.Item(key)
                        Dim arr As List(Of String) = Session.Item(key)
                        str &= header & "<b>" & key & "</b>:&nbsp; ArrayList[" & arr.Count & "]<br>"
                        str &= "<ul>"
                        'ROMAIN: Generic replacement - 08/21/2007
                        'Dim sList As New ArrayList
                        'sList = arr.Clone
                        Dim sList As New List(Of String)
                        sList = arr.GetRange(0, arr.Count - 1)
                        'ROMAIN: Generic replacement - 08/22/2007
                        'Dim sEnumerator As IEnumerator = sList.GetEnumerator
                        Dim sEnumerator As IEnumerator(Of String) = sList.GetEnumerator
                        Dim sindex As Integer = 0
                        While sEnumerator.MoveNext AndAlso sindex < 20
                            str &= "<li>[<b>" & sindex & "</b>]:&nbsp;"
                            Try
                                'ROMAIN: Generic replacement - 08/22/2007
                                'Dim val As String = sEnumerator.Current.ToString()
                                Dim val As String = sEnumerator.Current
                                If Not val Is Nothing Then
                                    str &= Utility.HTMLEncode(val)
                                End If
                            Catch ex2 As Exception
                                str &= "<span style=""color: red;"">Unable to parse value.</span>"
                            End Try
                            str &= "</li>"
                            sindex += 1
                        End While

                        If sEnumerator.MoveNext Then
                            str &= "<li>&lt; More... (The first 20 of " & sList.Count & " items were displayed.) &gt;</li>"
                        End If
                        str &= "</ul>"
                        str &= footer
                        'ROMAIN: Generic replacement - 08/20/2007
                        'NOTE: If T is a value type there is additional cost assocated with creating a specialized version of List<T> for that value type
                        'http://msdn2.microsoft.com/en-us/library/6sh2ey19.aspx
                        'ElseIf TypeOf Session.Item(key) Is Stack Then
                    ElseIf TypeOf Session.Item(key) Is Stack(Of String) Then
                        'Dim stk As Stack = Session.Item(key)
                        Dim stk As Stack(Of String) = Session.Item(key)
                        str &= header & "<b>" & key & "</b>:&nbsp; Stack[" & stk.Count & "]<br>"
                        str &= "<ul>"
                        'ROMAIN: Generic replacement - 08/21/2007
                        'Dim sList As New ArrayList
                        'sList = New ArrayList(stk.ToArray)
                        Dim sList As New List(Of String)
                        sList = New List(Of String)(stk.ToArray)
                        'ROMAIN: Generic replacement - 08/22/2007
                        'Dim sEnumerator As IEnumerator = sList.GetEnumerator
                        Dim sEnumerator As IEnumerator(Of String) = sList.GetEnumerator
                        Dim sindex As Integer = 0
                        While sEnumerator.MoveNext AndAlso sindex < 20
                            str &= "<li>[<b>" & sindex & "</b>]:&nbsp;"
                            Try
                                'ROMAIN: Generic replacement - 08/22/2007
                                'Dim val As String = sEnumerator.Current.ToString()
                                Dim val As String = sEnumerator.Current
                                If Not val Is Nothing Then
                                    str &= Utility.HTMLEncode(val)
                                End If
                            Catch ex2 As Exception
                                str &= "<span style=""color: red;"">Unable to parse value.</span>"
                            End Try
                            str &= "</li>"
                            sindex += 1
                        End While

                        If sEnumerator.MoveNext Then
                            str &= "<li>&lt; More... (The first 20 of " & sList.Count & " items were displayed.) &gt;</li>"
                        End If
                        str &= "</ul>"
                        str &= footer
                    ElseIf TypeOf Session.Item(key) Is IList Then
                        Dim lst As IList = Session.Item(key)
                        str &= header & "<b>" & key & "</b>:&nbsp; List[" & lst.Count & "]<br>"
                        str &= "<ul>"
                        Dim sEnumerator As IEnumerator = lst.GetEnumerator
                        Dim sindex As Integer = 0
                        While sEnumerator.MoveNext AndAlso sindex < 20
                            str &= "<li>[<b>" & sindex & "</b>]:&nbsp;"
                            Try
                                Dim val As String = sEnumerator.Current.ToString()
                                If Not val Is Nothing Then
                                    str &= Utility.HTMLEncode(val)
                                End If
                            Catch ex2 As Exception
                                str &= "<span style=""color: red;"">Unable to parse value.</span>"
                            End Try
                            str &= "</li>"
                            sindex += 1
                        End While
                        If sEnumerator.MoveNext Then
                            str &= "<li>&lt; More... (The first 20 of " & lst.Count & " items were displayed.) &gt;</li>"
                        End If
                        str &= "</ul>"
                        str &= footer
                    Else
                        Dim value As String = Convert.ToString(Session.Item(key))
                        'string s=Convert.ToString(val)
                        If Not value Is Nothing Then
                            str &= header & "<b>" & key & "</b>:&nbsp;" & Utility.HTMLEncode(value) & footer
                        Else
                            str &= header & "<b>" & key & "</b>:&nbsp; <span style=""color: red;"">This value is not parsable.</span>" & footer
                        End If
                        'str &= header & "<b>" & key & "</b>:&nbsp;" & HTMLEncode(CStr(Session.Item(key))) & footer
                    End If
                Catch ex As Exception
                    str &= header & "<b>" & key & "</b>:&nbsp; <span style=""color: red;"">Unable to parse value.</span>" & footer
                End Try
            Next
            Return str
        End Function
#End Region
#Region "Runtime Message Actions"
        Public Overrides Sub ExecuteActions(ByRef outgoingDS As DataSet, ByVal FilterField As String, ByVal FilterText As String, ByRef debugger As Debugger)
            Dim handler As New r2i.OWS.Actions.Runtime(Me, FilterField, FilterText)
            handler.ExecuteRoot(Me.xls.messageItems, debugger, outgoingDS)
            handler = Nothing
        End Sub
#End Region
    End Class
End Namespace
