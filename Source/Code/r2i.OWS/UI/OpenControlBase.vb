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
Imports System.Web.UI.WebControls
Imports System.Collections.Generic
Imports System.Web.UI
Imports r2i.OWS.Framework.Utilities.Compatibility, r2i.OWS.Actions.Utilities
Imports r2i.OWS
Imports r2i.OWS.Framework
Imports r2i.OWS.Framework.Utilities
Imports r2i.OWS.Framework.Utilities.JSON
Imports r2i.OWS.Framework.Entities
Imports r2i.OWS.Framework.DataAccess
Imports r2i.OWS.Framework.Plugins.Actions

Imports Microsoft.VisualBasic.CompilerServices
Imports System.Web.UI.ClientScriptManager
Namespace r2i.OWS.UI
    Public MustInherit Class OpenControlBase
        Inherits Framework.UI.Control
        Implements System.Web.UI.IPostBackEventHandler
        Implements System.Web.UI.IPostBackDataHandler

        Private _skipfordebug_ As Boolean = False

#Region "Public Events"
        Public Event ModuleCommunication(ByVal Caller As Object, ByVal Text As String, ByVal Value As String, ByVal Sender As String, ByVal Target As String)
#End Region
#Region "Public Enumerators"
        Public Enum MessageResponseEnum
            ACK
            ERR
        End Enum
        Public Enum MessageTypeEnum
            Page
            ItemCheck
            ItemRCheck
            SessionVariable
            Config
            Draft
            Actions
            Import
            Download
            Unknown
            Upgrade
            Downgrade
            Custom
        End Enum
        Public Enum EncodingTypeEnum
            GENERAL
            XML
            HTML
            TEXT
            JSON
        End Enum
#End Region
#Region "Required Operations"
        MustOverride ReadOnly Property Title() As Control
        MustOverride ReadOnly Property Header() As Control
        MustOverride ReadOnly Property Footer() As Control
        MustOverride Property ConfigurationId() As Guid
        MustOverride Property ModuleId() As String
        MustOverride Property PageModuleId() As String
        MustOverride Property PageId() As String
        MustOverride Property SiteId() As String
        MustOverride Property PageNumber() As Integer
        MustOverride Property UsePageDefaults() As Boolean
        MustOverride Property RecordsPerPage() As String
        MustOverride Property ResourceKey() As String
        MustOverride Property ResourceFile() As String
        MustOverride ReadOnly Property isEditable() As Boolean
        MustOverride ReadOnly Property isViewable() As Boolean
        MustOverride ReadOnly Property ListSource() As String
        MustOverride ReadOnly Property Settings() As Hashtable
        ''' <summary>
        ''' Root path for currently executing control
        ''' </summary>
        ''' <remarks>Don't confuse this with <seealso cref="OpenControlBase.BasePath">BasePath</seealso></remarks>
        MustOverride ReadOnly Property ModulePath() As String
        MustOverride Function MapPath(ByVal value As String) As String
        MustOverride ReadOnly Property CapturedMessages() As SortedList(Of String, String)
        MustOverride ReadOnly Property UserId() As String
        MustOverride ReadOnly Property UserInfo() As IUser
        MustOverride ReadOnly Property SiteInfo() As IPortalSettings
        MustOverride ReadOnly Property NoAjax() As Boolean
        ''' <summary>
        ''' Root path for OWS install (location of IM.aspx)
        ''' </summary>
        ''' <remarks>Don't confuse this with <seealso cref="OpenControlBase.ModulePath">ModulePath</seealso></remarks>
        MustOverride ReadOnly Property BasePath() As String
        MustOverride Property Cache() As System.Web.Caching.Cache
        MustOverride Overrides Sub ClearCache()
        MustOverride Overrides Sub ClearSiteCache()
        MustOverride Overrides Sub ClearPageCache()
        MustOverride Property FilterText() As String
        MustOverride Property FilterField() As String
        MustOverride Property ModuleConfiguration() As IModuleInfo
        MustOverride Property ConfigurationLocked() As Boolean
        MustOverride ReadOnly Property Session() As GenericSession
        MustOverride ReadOnly Property Request() As HttpRequest
        MustOverride ReadOnly Property isCallback() As Boolean
        MustOverride Sub RegisterScriptBlock(ByVal Key As String, ByVal Value As String)
        Public Overridable Function CachedScriptBlocks() As String
            Return Nothing
        End Function



        Property SuppressAJAX() As Boolean
            Get
                Return _suppressAJAX
            End Get
            Set(ByVal value As Boolean)
                _suppressAJAX = value
            End Set
        End Property
        Private _encodingType As EncodingTypeEnum
        Overridable Property EncodingType() As EncodingTypeEnum
            Get
                Return _encodingType
            End Get
            Set(ByVal value As EncodingTypeEnum)
                _encodingType = value
            End Set
        End Property
        Private _controlTag As String = "div"
        Overridable Property ControlTag() As String
            Get
                Return _controlTag
            End Get
            Set(ByVal value As String)
                _controlTag = value
            End Set
        End Property
#End Region
#Region "Private Properties"
        Public Const _ERRORVERSION As String = "02010020"
        Public Shared ReadOnly Property _JAVASCRIPTVERSION(Optional ByVal withDecimals As Boolean = False) As String
            Get
                Static Dim sVersion As String
                Static Dim sVersionDecimals As String
                If withDecimals Then
                    If sVersionDecimals Is Nothing Then
                        Try
                            Dim asb As Reflection.Assembly = Reflection.Assembly.GetExecutingAssembly
                            If Not asb Is Nothing Then
                                With asb.GetName
                                    If .Version.Revision > 0 Then
                                        sVersionDecimals = .Version.Major & "." & .Version.Minor.ToString & "." & .Version.Build.ToString & "." & .Version.Revision.ToString
                                    Else
                                        sVersionDecimals = .Version.Major & "." & .Version.Minor.ToString & "." & .Version.Build.ToString
                                    End If
                                End With
                            Else
                                sVersionDecimals = _ERRORVERSION.Substring(0, 2)
                            End If
                        Catch ex As Exception
                            sVersionDecimals = _ERRORVERSION
                        End Try
                    End If
                    Return sVersionDecimals
                Else
                    If sVersion Is Nothing Then
                        Try
                            Dim asb As Reflection.Assembly = Reflection.Assembly.GetExecutingAssembly
                            If Not asb Is Nothing Then
                                With asb.GetName
                                    sVersion = .Version.Major & .Version.Minor.ToString.PadLeft(2, "0") & .Version.Build.ToString.PadLeft(2, "0") & .Version.Revision.ToString.PadLeft(2, "0")
                                End With
                            Else
                                sVersion = _ERRORVERSION
                            End If
                        Catch ex As Exception
                            sVersion = _ERRORVERSION
                        End Try
                    End If
                    Return sVersion
                End If
            End Get
        End Property
        Private _configuration As Settings
        Private _engine As Engine
        Private _debugOutput As r2i.OWS.Framework.Debugger
        Private _totalRecords As Integer
        Private _requestType As String = "Page"
        Private _defaults As Defaults
        Private _output As Text.StringBuilder
        Private _GlobalPostBack As String
        Private _incomingParameters As Dictionary(Of String, String)
        Private _emptySession As GenericSession
        Private _suppressAJAX As Boolean = False
        Private Shared _s_returnImage As System.Drawing.Bitmap

        Public Const qConfigurationID = "id"
        Public Const qSource = "lxSrc"
        Public Const qResourceKey = "key"
        Public Const qResourceFile = "file"
        Public Const qPageId = "p"
        Public Const qPageModuleId = "pm"
        Public Const qModuleId = "m"
        Public Const qDownload = "download"
        Public Const qCPageId = "cp"
        Public Const qCPortalId = "pp"
        Public Const qCModuleId = "cm"
        Public Const qType = "type"
        Public Const qSort = "sort"
        Public Const qSortState = "sortstate"
        Public Const qAction = "lxA"
        Public Const qActions = "xActions"
        Public Const qUpgradeModule = "uM"
        Public Const qUpgradePage = "uT"
        Public Const qFilename = "filename"
        Public Const qDelete = "delete"
        Public Const qCheckIndex = "lxIx"
        Public Const qCheckModuleID = "lxM"
        Public Const qCheckGroup = "lxG"
        Public Const qCheckItem = "lxI"
        Public Const qCheckValue = "lxV"
        Public Const qCheckRemove = "lxR"
        Public Const qName = "lxN"
        Public Const qPageNumber = "lxP"
        Public Const qRecordsPerPage = "lxC"

        Public ReadOnly Property IncomingParameters() As Dictionary(Of String, String)
            Get
                If _incomingParameters Is Nothing Then
                    _incomingParameters = r2i.OWS.Framework.Utilities.Engine.Utility.ParseOWSIncomingParameters(Request)
                End If
                Return _incomingParameters
            End Get
        End Property
        Public ReadOnly Property Configuration() As Settings
            Get
                Return _configuration
            End Get
        End Property
        Public ReadOnly Property TotalRecords() As Integer
            Get
                Return _totalRecords
            End Get
        End Property
        Private Function Support_PageVariableName() As String
            If Not Configuration Is Nothing Then
                If Not Configuration.BotPageVariableName Is Nothing AndAlso Configuration.BotPageVariableName.Length > 0 Then
                    Return Configuration.BotPageVariableName
                End If
            End If
            Return "Page" & IIf(Not Me.ModuleId Is Nothing, Me.ModuleId, "").ToString
        End Function
        'Private Property sortActionList(ByVal ModuleID As String, ByVal UserID As String) As List(Of SortAction)
        '    Get
        '        Return EngineSingleton.Instance(Context).SortStatus(ModuleID, UserID)
        '    End Get
        '    Set(ByVal Value As List(Of SortAction))
        '        EngineSingleton.Instance(Context).SortStatus(ModuleID, UserID) = Value
        '    End Set
        'End Property
        Private Property sortActionList(ByVal ConfigurationID As String, ByVal ModuleID As String, ByVal UserID As String) As List(Of SortAction)
            Get
                Dim sac As Global.System.Collections.Generic.List(Of SortAction) = Nothing
                Try
                    sac = Framework.Utilities.Utility.SortStatus(Session, ConfigurationID.ToString.Replace("{", "").Replace("}", "").Replace("-", ""), ModuleID, UserID)
                Catch ex As Exception
                End Try
                If sac Is Nothing Then
                    Return Nothing
                Else
                    Return sac
                End If
            End Get
            Set(ByVal Value As List(Of SortAction))
                Try
                    Framework.Utilities.Utility.SortStatus(Session, ConfigurationID.ToString.Replace("{", "").Replace("}", "").Replace("-", ""), ModuleID, UserID) = Value
                Catch ex As Exception
                End Try
            End Set
        End Property
#End Region
#Region "Processing"
        Public Sub Control_Init()
            'LOAD HERE
            'Dim apc As Utilities.AbstractProvider = Utilities.ProviderDefinition
            'AbstractFactory.AbstractFactoryAssemblyName = apc.Assembly
            'AbstractFactory.AbstractFactoryClassName = apc.ClassName
            'r2i.OWS.Framework.Provider.Load()
        End Sub
        Public Sub Control_Load()
            If Not isCallback Then
                _requestType = "PAGE"
                Load_Output()
                Load_Configuration()
                Load_Defaults()
                Load_Engine(isCallback)
                Load_Request()
            Else
                _requestType = "AJAX"
                Callback()
            End If
        End Sub
        Public Sub Callback()
            Try
                Page.Response.Clear()
                'If Not Session Is Nothing AndAlso _emptySession Is Nothing AndAlso Not TypeOf (Session) Is Utilities.GenericSession Then
                '    _emptySession = New GenericSession(Session)
                'ElseIf Not Session Is Nothing AndAlso _emptySession Is Nothing AndAlso TypeOf (Session) Is Utilities.GenericSession Then
                '    _emptySession = Session
                'End If
                _emptySession = Session

                Dim msgType As MessageTypeEnum
                If Page.Request.QueryString Is Nothing Then
                    msgType = MessageTypeEnum.Unknown
                Else
                    Dim action As String
                    'If Not Page.Request.QueryString("lxA") Is Nothing Then
                    If IncomingParameters.ContainsKey(qAction) Then
                        action = IncomingParameters(qAction).ToUpper.Trim
                    Else
                        action = ""
                    End If

                    Select Case action
                        Case "D"
                            msgType = MessageTypeEnum.Download
                        Case "C"
                            msgType = MessageTypeEnum.ItemCheck
                        Case "U"
                            msgType = MessageTypeEnum.ItemRCheck
                        Case "V"
                            msgType = MessageTypeEnum.SessionVariable
                            'ROMAIN: 10/08/07
                            'TODO: CHECK SECURITY
                        Case "LISTXCONFIG"
                            'If AuthorizedUser Then
                            msgType = MessageTypeEnum.Config
                            'Else
                            'msgType = MessageTypeEnum.x-ListingPage
                            'End If
                        Case "LISTXDRAFT"
                            'If AuthorizedUser Then
                            msgType = MessageTypeEnum.Draft
                            'Else
                            'msgType = MessageTypeEnum.x-ListingPage
                            'End If
                        Case "LXACTIONS"
                            'If AuthorizedUser Then
                            msgType = MessageTypeEnum.Actions
                            'Else
                            'msgType = MessageTypeEnum.x-ListingPage
                            'End If
                        Case "LXIMPORT"
                            msgType = MessageTypeEnum.Import
                        Case "UPGRADE"
                            msgType = MessageTypeEnum.Upgrade
                        Case "CUSTOM"
                            msgType = MessageTypeEnum.Custom
                        Case Else
                            'STANDARD
                            msgType = MessageTypeEnum.Page
                    End Select
                End If
                Select Case msgType
                    Case MessageTypeEnum.ItemCheck
                        Callback_ItemCheck()
                    Case MessageTypeEnum.ItemRCheck
                        Callback_ItemUncheck()
                    Case MessageTypeEnum.Page
                        Callback_Page()
                    Case MessageTypeEnum.SessionVariable
                        Callback_SessionVariable()
                    Case MessageTypeEnum.Download
                        Callback_Download()
                    Case MessageTypeEnum.Config
                        Callback_Configuration()
                    Case MessageTypeEnum.Draft
                        Callback_Draft()
                    Case MessageTypeEnum.Actions
                        Callback_Actions()
                    Case MessageTypeEnum.Import
                        Callback_Import()
                    Case MessageTypeEnum.Upgrade
                        Callback_Upgrade()
                    Case MessageTypeEnum.Custom
                        Callback_Custom()
                End Select
            Catch ex As Exception
            End Try
        End Sub
        Public Sub Load_Configuration()
            Try
                'BETA RUNTIME CHANGE - CONFIGURATION IS CACHED AS THE SERIALIZED CONTENT.
                Dim obj As Object = Nothing
                If ResourceKey Is Nothing OrElse ResourceKey.Length = 0 Then
                    'Only load the serialized object if the current object is not contained within the cache

                    If Not Me.ConfigurationId = Guid.Empty Then
                        'If Not EngineSingleton.Instance(Context) Is Nothing Then
                        '    EngineSingleton.Instance(Context).CurrentConfigurationId = Me.ConfigurationId
                        'End If
                        'CHECK THE IOWS.CONFIGURED LOGIC IF IT IS REQUIRED\
                        Try
                            obj = Me.Cache.Item("OWS" & Me.ConfigurationId.ToString)
                        Catch ex As Exception
                        End Try
                        If Not obj Is Nothing Then
                            'THIS CONFIGURATION IS ALREADY CACHED
                            _configuration = r2i.OWS.Framework.Settings.Clone(CType(obj, r2i.OWS.Framework.Settings))
                        Else
                            'THIS CONFIGURATION IS NOT ALREADY CACHED
                            Dim ConfigCtrl As New Controller
                            Dim strSerialized As String = ConfigCtrl.GetSetting(Me.ConfigurationId)
                            If Not strSerialized Is Nothing Then
                                _configuration = r2i.OWS.Framework.Settings.Deserialize(strSerialized)
                            End If
                            If Not _configuration Is Nothing Then
                                'SUCCESS
                                Try
                                    Me.Cache.Item("OWS" & Me.ConfigurationId.ToString) = r2i.OWS.Framework.Settings.Clone(_configuration) 'strSerialized '_configuration
                                Catch ex As Exception
                                End Try
                            Else
                                'FAILED TO DESERIALIZE
                            End If
                        End If
                    Else
                        'NOT ASSIGNED A CONFIGURATION
                    End If

                Else
                    Dim rkCache As String = ""
                    If Not ResourceFile Is Nothing Then
                        rkCache = ResourceFile
                    End If
                    If Not Me.Cache Is Nothing Then
                        obj = Me.Cache.Item("OWS-" & ResourceKey & rkCache)
                    End If
                    If obj Is Nothing Then
                        'NOT YET CACHED
                        Dim value As String
                        'Dim sR As New IO.StreamReader(System.Reflection.Assembly.GetExecutingAssembly.GetManifestResourceStream(ResourceKey & ".Text"))
                        'value = sR.ReadToEnd()
                        If ResourceFile Is Nothing Then
                            Dim ext As String = ""
                            If Not ResourceKey.ToUpper.EndsWith(".TEXT") Then
                                ext = ".Text"
                            End If
                            Dim sR As New IO.StreamReader(System.Reflection.Assembly.GetExecutingAssembly.GetManifestResourceStream(ResourceKey & ext))
                            value = sR.ReadToEnd()
                        Else
                            value = Support_GetResourceValue(ResourceFile, ResourceKey) '
                        End If

                        If value Is Nothing OrElse value.Length = 0 Then
                            Throw New Exception("New Control")
                        End If
                        If Not value Is Nothing Then
                            _configuration = r2i.OWS.Framework.Settings.Deserialize(value)
                        End If
                        If Not _configuration Is Nothing AndAlso Not Me.Cache Is Nothing Then
                            Me.Cache.Item("OWS-" & ResourceKey & rkCache) = r2i.OWS.Framework.Settings.Clone(_configuration) 'value
                        End If
                        value = Nothing
                    Else
                        _configuration = r2i.OWS.Framework.Settings.Clone(CType(obj, r2i.OWS.Framework.Settings)) ' r2i.OWS.Framework.Utilities.Compatibility.Settings.Deserialize(CType(obj, String)) 
                    End If
                End If

                If _configuration Is Nothing Then
                    Throw New Exception("No Configuration")
                End If
            Catch ex As Exception
                _configuration = New Settings
                _configuration.showAll = True
                _configuration.query = ""
                _configuration.defaultItem = ""
                If (ResourceKey Is Nothing OrElse ResourceKey.Length = 0) Then
                    Dim value As String
                    Dim sR As New IO.StreamReader(System.Reflection.Assembly.GetExecutingAssembly.GetManifestResourceStream("Configuration.New" & ".Text"))
                    value = sR.ReadToEnd()
                    sR.Close()
                    sR = Nothing

                    Dim engineCtrl As IEngineController = AbstractFactory.Instance.EngineController
                    value = value.Replace("[LINK:VIEW]", engineCtrl.NavigateURL(Me.PageId) & "?mid=" & Me.ModuleId & "&ctl=Edit")
                    value = value.Replace("[LINK:SETTINGS]", engineCtrl.NavigateURL(Me.PageId) & "?ModuleId=" & Me.ModuleId & "&ctl=Module")
                    value = value.Replace("[LINK:WORD]", Me.BasePath & "r2i.OWS.Documentation.doc")
                    value = value.Replace("[LINK:PDF]", Me.BasePath & "r2i.OWS.Documentation.pdf")
                    value = value.Replace("[DIRECTORY]", Me.BasePath)


                    'ROMAIN: 09/24/07
                    'TODO: Centralize declaration

                    If Me.UserId.Length > 0 AndAlso Me.isEditable Then
                        _configuration.noqueryItem = value
                    End If
                Else
                    _configuration.noqueryItem = ""
                End If
                _configuration.listItem = ""
                _configuration.listAItem = ""
                _configuration.searchItems = New List(Of SearchOptionItem)
                _configuration.QueryItemsClear()
                _configuration.listItems = New List(Of ListFormatItem)
            End Try
        End Sub
        Public Sub Load_Output()
            Try
                _GlobalPostBack = "javascript:" & Parent.Page.ClientScript.GetPostBackEventReference(Me, "{0}={1}")
            Catch ex As Exception
                _GlobalPostBack = ""
            End Try
            _output = New Text.StringBuilder
        End Sub
        Public Sub Load_Defaults()
            _defaults = New Defaults
            _defaults.Data = New DataSet
            If Me.UsePageDefaults OrElse Me.RecordsPerPage Is Nothing OrElse Me.RecordsPerPage.Length = 0 Then
                Me.RecordsPerPage = Configuration.recordsPerPage
            End If
            Load_HeaderFooter()
            '_defaults.PageNumber = Me.PageNumber
        End Sub
        Private Sub Load_HeaderFooter()
            If Not Me.Header Is Nothing Then
                If TypeOf Header Is Label Then
                    Dim lbl As Label = CType(Header, Label)
                    Me.Configuration.Header = lbl.Text
                ElseIf TypeOf Header Is Literal Then
                    Dim ltl As Literal = CType(Header, Literal)
                    Me.Configuration.Header = ltl.Text
                End If
            Else
                Me.Configuration.Header = ""
            End If
            If Not Me.Footer Is Nothing Then
                If TypeOf Footer Is Label Then
                    Dim lbl As Label = CType(Footer, Label)
                    Me.Configuration.Footer = lbl.Text
                ElseIf TypeOf Footer Is Literal Then
                    Dim ltl As Literal = CType(Footer, Literal)
                    Me.Configuration.Footer = ltl.Text
                End If
            Else
                Me.Configuration.Footer = ""
            End If
            Try
                If Not Me.Title Is Nothing Then
                    If TypeOf Me.Title Is System.Web.UI.WebControls.Label Then
                        Dim lbl As Label = CType(Me.Title, Label)
                        Me.Configuration.Title = lbl.Text

                    Else
                        Me.Configuration.Title = Me.Title.GetType().GetProperty("Text").GetValue(Me.Title, Nothing)
                    End If
                End If
            Catch ex As Exception
            End Try

        End Sub
        Private Sub Load_Sort(ByVal SortValue As String, ByVal SortState As String)
            If Not SortValue Is Nothing Then
                Dim saTemp As SortAction = New SortAction
                saTemp.SortOrder = SortValue

                Dim SortArr As List(Of SortAction) = sortActionList(ConfigurationId.ToString, ModuleId, UserInfo.Id)
                If Not SortArr Is Nothing Then
                    Dim i As Integer = SortArr.IndexOf(saTemp)
                    If i >= 0 Then
                        Dim sa As SortAction = SortArr.Item(SortArr.IndexOf(saTemp))

                        If SortState Is Nothing Then
                            If sa.SortDirection = "DESC" Then
                                If Not sa.Toggle Then
                                    sa.SortDirection = ""
                                Else
                                    sa.SortDirection = "ASC"
                                End If
                            ElseIf sa.SortDirection = "ASC" Then
                                sa.SortDirection = "DESC"
                            Else
                                sa.SortDirection = "ASC"
                            End If
                        Else
                            sa.SortDirection = SortState
                        End If

                        SortArr.Remove(sa)
                        If Not Configuration.enableMultipleColumnSorting Then
                            For Each saTemp In SortArr
                                saTemp.SortDirection = ""
                            Next
                        End If
                        SortArr.Insert(0, sa)
                        sortActionList(ConfigurationId.ToString, ModuleId, UserInfo.Id) = SortArr
                    End If
                End If
            End If
        End Sub
        Public Sub Load_Engine(ByVal isAjax As Boolean)
            If Configuration.canDebug(Me.UserInfo, Me.SiteInfo, Me.isEditable) And Configuration.OutputType = r2i.OWS.Framework.Settings.RenderType.Default Then
                Debugger_Open(True)
            End If
            If _configuration.enableQueryDebug_Log AndAlso _debugOutput Is Nothing Then
                Debugger_Open(False)
            End If
            _engine = New Engine(Context, Session, Me, isAjax, Me.UserInfo, Me.ViewState, Me.Settings, Me.SiteInfo, Me.CapturedMessages, Me.ModuleId, Me.PageId, Me.PageModuleId, Me.ConfigurationId, Me.Configuration, Me.ClientID, Me.ModulePath, _debugOutput, Me.isEditable)
            _engine.RecordsPerPage = Me.Configuration.recordsPerPage
        End Sub
        Public Sub Load_Request()
            'HANDLE THE INCOMING PAGE VARIABLE (AUTOBOT)
            If Not isCallback AndAlso Not Me.Page Is Nothing Then
                Try
                    If Not Me.Page.Request.QueryString.Item(Support_PageVariableName) Is Nothing AndAlso IsNumeric(Me.Page.Request.QueryString.Item(Support_PageVariableName)) Then
                        If Not IncomingParameters.ContainsKey(qPageNumber) OrElse IncomingParameters(qPageNumber).Length = 0 Then
                            Me.PageNumber = CInt(Me.Page.Request.QueryString.Item(Support_PageVariableName)) - 1
                        End If
                    End If
                Catch ex As Exception
                    _engine.CurrentPage = PageNumber
                End Try
            Else
                _engine.CurrentPage = PageNumber
            End If
        End Sub

        Public Sub Load_Incoming(ByVal eventargument As String)
            Try
                Select Case True
                    Case eventargument.ToUpper.StartsWith("ALPHA=")
                        Load_Incoming_Alphabet(eventargument)
                    Case eventargument.ToUpper.StartsWith("PAGE=")
                        Load_Incoming_Page(eventargument)
                    Case eventargument.ToUpper.StartsWith("SEARCH=")
                        Load_Incoming_Search(eventargument)
                    Case eventargument.ToUpper.StartsWith("PERPAGE=")
                        Load_Incoming_PerPage(eventargument)
                    Case eventargument Like "#*:#*;*"
                        Load_Incoming_Actions(eventargument)
                    Case Else
                        Load_Incoming_Action(eventargument)
                End Select
            Catch taex As Threading.ThreadAbortException
                'IGNORE THIS EXCEPTION
            Catch ex As Exception
                'ROMAIN: 09/26/07D
                'TODO:CHANGE EXCEPTIONS
                'DotNetNuke.Services.Exceptions.LogException(ex)
            End Try
        End Sub
        Private Sub Load_Incoming_Page(ByVal eventargument As String)
            Dim eventInfo As String = eventargument.Substring(5)
            If eventInfo.Length > 0 AndAlso IsNumeric(eventInfo) Then
                Try
                    Me.PageNumber = CInt(eventInfo)
                    If Not Configuration.ModuleCommunicationMessageType Is Nothing AndAlso Configuration.ModuleCommunicationMessageType.Length > 0 Then
                        Try
                            'ROMAIN: 09/24/07
                            'TODO:RAISE EVENT
                            RaiseEvent ModuleCommunication(Me, Configuration.ModuleCommunicationMessageType, eventInfo, Me.GetType().Name, "")
                        Catch ex As Exception

                        End Try
                    End If
                Catch ex As Exception

                End Try
            End If
        End Sub
        Private Sub Load_Incoming_Search(ByVal eventargument As String)
            Dim eventInfo As String = eventargument.Substring(7)
            If eventInfo.Length > 0 Then
                Me.PageNumber = 0
                Me.FilterField = Me.Page.Request.Form.Item(Me.UniqueID & "_ddlSearch")
                Me.FilterText = Me.Page.Request.Form.Item(Me.UniqueID & "_txtSearch")
                If Not Configuration.ModuleCommunicationMessageType Is Nothing AndAlso Configuration.ModuleCommunicationMessageType.Length > 0 Then
                    Try
                        'ROMAIN: 09/24/07
                        'TODO:RAISE EVENT
                        RaiseEvent ModuleCommunication(Me, Configuration.ModuleCommunicationMessageType, "Filter", Me.GetType.Name, "")
                    Catch ex As Exception

                    End Try
                End If
            End If
        End Sub
        Private Sub Load_Incoming_Alphabet(ByVal eventargument As String)
            Dim eventInfo As String = eventargument.Substring(6)
            If eventInfo.Length > 0 Then
                Try
                    Me.PageNumber = 0
                    Me.FilterField = Nothing
                    Me.FilterText = eventInfo
                    If Not Configuration.ModuleCommunicationMessageType Is Nothing AndAlso Configuration.ModuleCommunicationMessageType.Length > 0 Then
                        Try
                            'ROMAIN: 09/24/07
                            'TODO:RAISE EVENT
                            RaiseEvent ModuleCommunication(Me, Configuration.ModuleCommunicationMessageType, "Filter", Me.GetType.Name, "")
                        Catch ex As Exception

                        End Try
                    End If
                Catch ex As Exception

                End Try
            End If
        End Sub
        Private Sub Load_Incoming_PerPage(ByVal eventargument As String)
            Dim eventInfo As String = eventargument.Substring(8)
            If eventInfo.Length > 0 AndAlso eventInfo = "PERPAGE" Then
                If Not Me.Page.Request.Form(Me.UniqueID & "selRPP") Is Nothing Then
                    eventInfo = Me.Page.Request.Form(Me.UniqueID & "selRPP")
                End If
                If IsNumeric(eventInfo) AndAlso Not CInt(eventInfo) = Me.RecordsPerPage Then
                    Me.PageNumber = 0
                    Me.RecordsPerPage = eventInfo
                    If Not Configuration.ModuleCommunicationMessageType Is Nothing AndAlso Configuration.ModuleCommunicationMessageType.Length > 0 Then
                        Try
                            RaiseEvent ModuleCommunication(Me, Configuration.ModuleCommunicationMessageType, "PageCount", Me.GetType().Name, "")
                        Catch ex As Exception

                        End Try
                    End If
                End If
            End If
        End Sub
        Private Sub Load_Incoming_Actions(ByVal eventArgument As String)
            Dim spltSS As New SmartSplitter
            Dim idxSplitter As Integer
            Dim sQuery As String = ""

            spltSS.Split(eventArgument)
            For idxSplitter = 0 To spltSS.Length - 1
                Dim sAction As String = spltSS.Item(idxSplitter)

                If sAction Like "#*:#*;*" Then
                    Dim spltAct As New SmartSplitter

                    spltAct.Split(sAction)
                    If spltAct.Length = 3 Then
                        Dim strColumnValue As String = spltAct(0)
                        Dim strVariableName As String = spltAct(1)
                        Dim strVariableType As String = spltAct(2)

                        Load_Incoming_Action(strColumnValue, strVariableName, strVariableType, Nothing, sQuery)
                    ElseIf spltAct.Length = 4 Then
                        Dim strColumnValue As String = spltAct(0)
                        Dim strVariableName As String = spltAct(1)
                        Dim strVariableType As String = spltAct(3)
                        Dim strTargetURL As String = spltAct(2)

                        Load_Incoming_Action(strColumnValue, strVariableName, strVariableType, strTargetURL, sQuery)
                    End If
                Else
                    Load_Incoming_Action(Nothing, Nothing, Nothing, sAction, sQuery)
                End If
            Next
        End Sub
        Private Sub Load_Incoming_Action(ByVal eventargument As String)
            Dim strArray() As String = eventargument.Split(CChar(","))
            If strArray.Length = 4 Then
                Dim strColumnValue As String = strArray(0)
                Dim strVariableName As String = strArray(1)
                Dim strTargetURL As String = strArray(2)
                Dim strVariableType As String = strArray(3)

                Load_Incoming_Action(strColumnValue, strVariableName, strVariableType, strTargetURL)
            End If
            If strArray.Length = 2 AndAlso strArray(0) = "SORTCOMMAND" Then
                Dim sortActionOrder As Integer = CInt(strArray(1))
                Dim saTemp As SortAction = New SortAction
                saTemp.SortOrder = CStr(sortActionOrder)

                'ROMAIN: Generic replacement - 08/20/2007
                'Dim SortArr As ArrayList = sortActionList
                'Dim SortArr As List(Of SortAction) = sortActionList
                Dim SortArr As List(Of SortAction) = Framework.Utilities.Utility.SortStatus(Session, Me.ConfigurationId.ToString.Replace("{", "").Replace("}", "").Replace("-", ""), Me.ModuleId, Me.UserId)


                Dim sa As SortAction = SortArr.Item(SortArr.IndexOf(saTemp))

                If sa.SortDirection = "DESC" Then
                    If Not sa.Toggle Then
                        sa.SortDirection = ""
                    Else
                        sa.SortDirection = "ASC"
                    End If
                ElseIf sa.SortDirection = "ASC" Then
                    sa.SortDirection = "DESC"
                Else
                    sa.SortDirection = "ASC"
                End If

                SortArr.Remove(sa)
                If Not Configuration.enableMultipleColumnSorting Then
                    For Each saTemp In SortArr
                        saTemp.SortDirection = ""
                    Next
                End If
                'sortActionList.Insert(sortActionListIndex, sa)
                SortArr.Insert(0, sa)

                'BindData("", "")
                'sortActionList = SortArr
                Framework.Utilities.Utility.SortStatus(Session, Me.ConfigurationId.ToString.Replace("{", "").Replace("}", "").Replace("-", ""), Me.ModuleId, Me.UserId) = SortArr

                If Not Configuration.ModuleCommunicationMessageType Is Nothing AndAlso Configuration.ModuleCommunicationMessageType.Length > 0 Then
                    Try
                        'ROMAIN: 09/24/07
                        'TODO:RAISE EVENT
                        RaiseEvent ModuleCommunication(Me, Configuration.ModuleCommunicationMessageType, "Sort", Me.GetType.Name, "")
                    Catch taex As Threading.ThreadAbortException
                    Catch ex As Exception
                    End Try
                End If
            End If
        End Sub

        Private Sub Load_Incoming_Action(ByVal ColumnValue As String, ByVal VariableName As String, ByVal VariableType As String, ByVal TargetURL As String, Optional ByRef QueryString As String = "")
            Try
                If Not VariableName Is Nothing And Not VariableType Is Nothing Then
                    Select Case VariableType
                        Case "S"
                            If Not Me.Page.Session Is Nothing Then
                                If Not ColumnValue Is Nothing Then
                                    Me.Page.Session(VariableName) = ColumnValue
                                Else
                                    Me.Page.Session.Remove(VariableName)
                                End If
                            End If
                        Case "Q"
                            If QueryString Is Nothing Then
                                QueryString = ""
                            End If
                            If QueryString <> "" Then
                                QueryString &= "&"
                            End If
                            If Not ColumnValue Is Nothing Then
                                QueryString &= VariableName & "=" & ColumnValue
                            Else
                                QueryString &= VariableName & "="
                            End If
                        Case "V"
                            If Not ViewState Is Nothing Then
                                If Not ColumnValue Is Nothing Then
                                    ViewState.Item(VariableName) = ColumnValue
                                Else
                                    ViewState.Remove(VariableName)
                                End If
                            End If
                        Case "C"
                            If Not Me.Page.Response Is Nothing AndAlso Not Me.Page.Response.Cookies Is Nothing AndAlso VariableName.Length > 0 Then
                                Dim ck As Web.HttpCookie = Nothing
                                Try
                                    ck = Me.Page.Response.Cookies(VariableName)
                                Catch ex As Exception
                                End Try
                                If ck Is Nothing Then
                                    ck = New Web.HttpCookie(VariableName)
                                    Me.Page.Response.Cookies.Add(ck)
                                End If
                                ck.Name = VariableName
                                If Not ColumnValue Is Nothing Then
                                    ck.Value = ColumnValue
                                Else
                                    ck.Value = ""
                                End If
                            End If
                        Case "M"
                            Try
                                'ROMAIN: 09/24/07
                                'TODO:RAISE EVENT
                                RaiseEvent ModuleCommunication(Me, VariableName, ColumnValue, Me.GetType.Name, "")
                            Catch taex As Threading.ThreadAbortException
                            Catch ex As Exception

                            End Try
                    End Select
                End If
                'VERSION: 1.7.9 - Corrected TabName Lookup when ACTION target url is empty
                If Not TargetURL Is Nothing AndAlso TargetURL.Length > 0 Then
                    Dim tbinfo As ITabInfo
                    Dim tabCtrl As ITabController = AbstractFactory.Instance.TabController
                    If IsNumeric(TargetURL) Then
                        'Go to the Tab
                        Try
                            'ROMAIN: 09/26/07
                            'Dim tbinfo As DotNetNuke.Entities.Tabs.TabInfo
                            'Dim tbinfo As ITabInfo
                            'Dim tabCtrl As ITabController = AbstractFactory.Instance.TabController
                            'tbinfo = (New DotNetNuke.Entities.Tabs.TabController).GetTab(CType(TargetURL, Integer))
                            tbinfo = tabCtrl.GetTab(TargetURL)
                            If tbinfo Is Nothing Then
                                Throw New Exception("No Tab with the ID '" & TargetURL & "' was discovered.")
                            End If
                            TargetURL = tbinfo.FullUrl
                        Catch taex As Threading.ThreadAbortException
                        Catch ex As Exception
                            Throw New Exception("Unable to parse tab information - " & ex.ToString)
                        End Try
                    Else
                        If TargetURL.IndexOf(".") >= 0 Or TargetURL.IndexOf("/") >= 0 Or TargetURL.IndexOf("\") >= 0 Or TargetURL.IndexOf(":") >= 0 Then
                            'This is a URL - Leave it alone
                        Else
                            'This is a tab name

                            'Dim tbinfo As DotNetNuke.Entities.Tabs.TabInfo
                            'tbinfo = (New DotNetNuke.Entities.Tabs.TabController).GetTabByName(TargetURL, Me.PortalId)
                            tbinfo = tabCtrl.GetTabByName(TargetURL, Me.SiteId)
                            If tbinfo Is Nothing Then
                                Throw New Exception("No Tab with the Name '" & TargetURL & "' was discovered.")
                            End If
                            TargetURL = tbinfo.FullUrl
                        End If
                    End If
                    If QueryString Is Nothing Then
                        QueryString = ""
                    End If
                    If QueryString.Length > 0 Then
                        If TargetURL.IndexOf("?") < 0 Then
                            TargetURL &= "?" & QueryString
                        Else
                            TargetURL &= "&" & QueryString
                        End If
                    End If
                    If TargetURL.Length > 0 Then
                        Try
                            Me.Page.Response.Redirect(TargetURL, True)
                        Catch taex As Threading.ThreadAbortException
                        Catch ex As Exception
                        End Try
                    End If
                End If
            Catch taex As Threading.ThreadAbortException
            Catch ex As Exception
                If Not _debugOutput Is Nothing Then
                    Me._debugOutput.AppendLine(Utility.HTMLEncode("An exception occurred trying to handle the Execute Incoming Action process, the contents of the exception are: " & ex.ToString))
                Else
                    'ROMAIN: 09/26/07
                    'TODO: Change Exceptions
                    'DotNetNuke.Services.Exceptions.LogException(New Exception("An exception occurred trying to handle the Execute Incoming Action process, the contents of the exception are: " & ex.ToString))
                End If
            End Try
        End Sub
#End Region
#Region "Support"
        Private Function Support_GetJavascriptOnComplete() As String
            If Utility.CNullStr(Configuration.javascriptOnComplete) <> "" Then
                Dim sScript As String = _engine.RenderString(Nothing, Configuration.javascriptOnComplete, Me.CapturedMessages, False, isPreRender:=False, NullReturn:=False, ProtectSession:=True, DebugWriter:=_debugOutput)
                Return Utility.CNullStr(sScript, "")
            Else
                Return String.Empty
            End If
        End Function
        Private Function Support_GetResourceValue(ByVal ResourceFile As String, ByVal Key As String) As String
            Dim value As String
            If ResourceFile Is Nothing Then
                Dim sR As New IO.StreamReader(System.Reflection.Assembly.GetExecutingAssembly.GetManifestResourceStream(Key & ".Text"))
                value = sR.ReadToEnd()
            Else
                If ResourceFile.Length = 0 Then
                    Dim portalSettingsCrtl As IPortalSettingsController = AbstractFactory.Instance.PortalSettingsController
                    value = portalSettingsCrtl.LocalizationGetString(Key, ResourceFile)
                Else
                    value = Support_GetResourceFromFile(Key, ResourceFile)
                End If
            End If
            If value Is Nothing Then
                value = ""
            End If
            Return value
        End Function
        Private Function Support_GetResourceFromFile(ByVal Key As String, ByVal File As String) As String
            Dim fstr As String = File
            If Not fstr.ToLower.EndsWith(".resx") Then
                If fstr.ToLower.EndsWith(".aspx") OrElse fstr.ToLower.EndsWith(".ascx") Then
                    fstr &= ".resx"
                Else
                    fstr &= ".ascx.resx"
                End If
            End If
            If Key.IndexOf(".") < 0 Then
                Key &= ".Text"
            End If
            Dim target As Hashtable = Nothing
            If Not Cache Is Nothing Then
                target = Cache.Item(fstr)
            End If
            If target Is Nothing Then
                Dim xmlLoaded As Boolean = False
                Dim d As New System.Xml.XmlDocument
                Try
                    d.Load(MapPath(fstr))
                    xmlLoaded = True
                Catch exc As Exception
                    xmlLoaded = False
                End Try
                If xmlLoaded Then
                    Dim n As System.Xml.XmlNode
                    target = New Hashtable
                    For Each n In d.SelectNodes("root/data")
                        If n.NodeType <> System.Xml.XmlNodeType.Comment Then
                            Dim val As String = n.SelectSingleNode("value").InnerText
                            If Not n.Attributes("type") Is Nothing AndAlso n.Attributes("type").Value.ToUpper.StartsWith("SYSTEM.BYTE[]") Then
                                val = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(val))
                            End If
                            If target(n.Attributes("name").Value) Is Nothing Then
                                target.Add(n.Attributes("name").Value, val)
                            Else
                                target(n.Attributes("name").Value) = val
                            End If
                        End If
                    Next n
                    If Not Cache Is Nothing Then
                        Cache(fstr) = target
                    End If
                Else
                    target = New Hashtable
                End If
            End If
            If target.ContainsKey(Key) Then
                Support_GetResourceFromFile = target(Key)
                target = Nothing
            Else
                'ROMAIN: 08/22/2007
                'NOTE: Replacement Return ""
                Return String.Empty
            End If
        End Function
#End Region
#Region "Page Processing"
        Private Sub Callback_ItemCheck()
            Try

                Dim moduleId As String = IncomingParameters(qCheckModuleID)
                Dim GroupName As String = IncomingParameters(qCheckGroup)
                Dim Item As String = IncomingParameters(qCheckItem)
                Dim Value As String = IncomingParameters(qCheckValue)

                If Not moduleId Is Nothing AndAlso moduleId.Length > 0 Then
                    Dim checks As List(Of String) = Utilities.Utility.CheckedItems(moduleId, GroupName, _emptySession)
                    Dim unchecks As List(Of String) = Utilities.Utility.UnCheckedItems(moduleId, GroupName, _emptySession)
                    If Value = "1" Then
                        If Not checks.Contains(Item) Then
                            checks.Add(Item)
                        End If
                        If unchecks.Contains(Item) Then
                            unchecks.Remove(Item)
                        End If
                    Else
                        If checks.Contains(Item) Then
                            checks.Remove(Item)
                        End If
                        If Not unchecks.Contains(Item) Then
                            unchecks.Add(Item)
                        End If
                    End If

                    Utilities.Utility.CheckedItems(moduleId, GroupName, _emptySession) = checks
                    Utilities.Utility.UnCheckedItems(moduleId, GroupName, _emptySession) = unchecks
                End If
            Catch ex As Exception
                Log_Exception(ex)
            End Try
            Try
                Render_Callback_ReturnMessage()
            Catch ex As Exception
                Log_Exception(ex)
            End Try
        End Sub
        Private Sub Callback_ItemUncheck()
            Try
                Dim moduleId As String = IncomingParameters(qCheckModuleID)
                Dim GroupName As String = IncomingParameters(qCheckGroup)
                Dim Item As String = IncomingParameters(qCheckItem)
                Dim Value As String = IncomingParameters(qCheckValue)

                If Not moduleId Is Nothing AndAlso moduleId.Length > 0 Then
                    Dim arr As List(Of String) = New List(Of String)
                    Dim unarr As List(Of String) = Utilities.Utility.UnCheckedItems(moduleId, GroupName, _emptySession)
                    If Value = "1" Then
                        If Not arr.Contains(Item) Then
                            arr.Add(Item)
                        End If
                        If unarr.Contains(Item) Then
                            unarr.Remove(Item)
                        End If
                    Else
                        If Not unarr.Contains(Item) Then
                            unarr.Add(Item)
                        End If
                    End If

                    Utilities.Utility.CheckedItems(moduleId, GroupName, _emptySession) = arr
                    Utilities.Utility.UnCheckedItems(moduleId, GroupName, _emptySession) = unarr
                End If
            Catch ex As Exception
                Log_Exception(ex)
            End Try
            Try
                Render_Callback_ReturnMessage()
            Catch ex As Exception
                Log_Exception(ex)
            End Try
        End Sub
        Private Sub Callback_SessionVariable()
            Try
                Dim SessionName As String = IncomingParameters(qName)
                Dim moduleId As String = IncomingParameters(qCheckModuleID)
                Dim Item As String = IncomingParameters(qCheckItem)
                Dim Value As String = IncomingParameters(qCheckValue)


                If moduleId > 0 AndAlso isViewable Then
                    Session.Item(SessionName) = Value
                End If
            Catch ex As Exception
                Log_Exception(ex)
            End Try
            Try
                Render_Callback_ReturnMessage()
            Catch ex As Exception
                Log_Exception(ex)
            End Try
        End Sub

        Private Sub Callback_Page()
            Try
                If Not IncomingParameters.ContainsKey(qPageNumber) OrElse IncomingParameters(qPageNumber).Length = 0 Then
                    PageNumber = 0
                Else
                    PageNumber = CType(IncomingParameters(qPageNumber), Integer)
                End If
                If Not IncomingParameters.ContainsKey(qRecordsPerPage) OrElse IncomingParameters(qRecordsPerPage).Length = 0 Then
                    UsePageDefaults = True
                Else
                    RecordsPerPage = CType(IncomingParameters(qRecordsPerPage), Integer)
                End If


                Dim source As String
                If IncomingParameters.ContainsKey(qSource) Then
                    source = IncomingParameters(qSource)
                Else
                    source = ""
                End If
                If IncomingParameters.ContainsKey(qPageId) Then
                    PageId = IncomingParameters(qPageId)
                End If
                If IncomingParameters.ContainsKey(qModuleId) Then
                    ModuleId = IncomingParameters(qModuleId)
                End If

                If IncomingParameters.ContainsKey(qCModuleId) Then
                    ModuleId = IncomingParameters(qCModuleId)
                End If
                If IncomingParameters.ContainsKey(qCPageId) Then
                    PageId = IncomingParameters(qCPageId)
                End If
                If IncomingParameters.ContainsKey(qCPortalId) Then
                    SiteId = IncomingParameters(qCPortalId)
                End If
                If IncomingParameters.ContainsKey(qPageModuleId) Then
                    PageModuleId = IncomingParameters(qPageModuleId)
                End If

                If IncomingParameters.ContainsKey(qResourceKey) Then
                    If IncomingParameters(qResourceKey).Length > 0 Then
                        ResourceKey = IncomingParameters(qResourceKey)
                    End If
                    If IncomingParameters.ContainsKey(qResourceFile) Then
                        ResourceFile = IncomingParameters(qResourceFile)
                    End If
                End If
                If ResourceKey Is Nothing Then
                    Dim configId As String = Nothing
                    If IncomingParameters.ContainsKey(qConfigurationID) Then
                        configId = IncomingParameters(qConfigurationID)
                    End If
                    If Not configId Is Nothing AndAlso configId.Length > 0 Then
                        Try
                            ConfigurationId = New Guid(configId)
                        Catch ex As Exception
                            Throw New Exception("configurationid not correct")
                        End Try
                    Else
                        'Throw New Exception("configurationid not provided")
                    End If
                End If

                Dim sortValue As String = Nothing
                Dim sortState As String = Nothing
                If IncomingParameters.ContainsKey(qSort) Then
                    'HANDLE THE SORT

                    Dim sortActionOrder As Integer = CInt(IncomingParameters(qSort))
                    sortValue = sortActionOrder
                    If IncomingParameters.ContainsKey(qSortState) Then
                        Dim sortActionState As Integer = CInt(IncomingParameters(qSortState))
                        If sortActionState > 0 Then
                            sortState = "ASC"
                        Else
                            sortState = "DESC"
                        End If
                    End If
                End If

                If IncomingParameters.ContainsKey(qType) AndAlso IncomingParameters(qType).Length > 0 Then
                    Select Case IncomingParameters(qType).ToUpper
                        Case "XML"
                            EncodingType = EncodingTypeEnum.XML
                        Case "TEXT"
                            EncodingType = EncodingTypeEnum.TEXT
                        Case "HTML"
                            EncodingType = EncodingTypeEnum.HTML
                        Case "JSON"
                            EncodingType = EncodingTypeEnum.JSON
                    End Select
                End If

                Render_Callback_Content(sortValue, sortState)

                'Debugger_End("Page")
            Catch ex As Exception
                Log_Exception(ex)
            End Try
        End Sub
        Private Sub Callback_Download()
            Dim sName As String = IncomingParameters(qDownload)
            Dim sFileName As String = MapPath(Me.BasePath & "dl" & "/" & sName)
            Dim sContentType As String = IncomingParameters(qType)
            Dim sRename As String = IncomingParameters(qFilename)
            Dim sDelete As String = IncomingParameters(qDelete)

            If sRename Is Nothing OrElse sRename.Length <= 0 Then
                sRename = sName
            End If
            If sDelete Is Nothing OrElse sDelete.Length <= 0 Then
                sDelete = "True"
            End If
            If IO.File.Exists(sFileName) Then
                Page.Response.Clear()
                Page.Response.Charset = ""
                Page.Response.ContentType = sContentType
                Page.Response.ClearHeaders()
                Page.Response.AddHeader("Content-Disposition", "attachment; filename=""" & sRename & """")
                Dim fs As New IO.FileStream(sFileName, IO.FileMode.Open, IO.FileAccess.Read)
                Dim sreader As New IO.BinaryReader(fs)
                While sreader.BaseStream.Position < sreader.BaseStream.Length - 1
                    Dim bsize As Integer = 32767
                    If bsize + sreader.BaseStream.Position >= sreader.BaseStream.Length Then
                        bsize = (sreader.BaseStream.Length - sreader.BaseStream.Position) - 1
                    End If
                    If bsize > 0 Then
                        Dim buffer() As Byte = sreader.ReadBytes(bsize)
                        Page.Response.BinaryWrite(buffer)
                        buffer = Nothing
                    Else
                        Exit While
                    End If
                End While

                sreader.Close()
                sreader = Nothing

                If sDelete.ToLower = "true" Then
                    Try
                        IO.File.Delete(sFileName)
                    Catch ex As Exception
                        'DotNetNuke.Services.Exceptions.LogException(ex)
                    End Try
                End If
            Else
                'ROMAIN: 02/10/07
                'TODO: implement Exception
                'DotNetNuke.Services.Exceptions.LogException(New IO.FileNotFoundException("Could not export file with type " & sContentType, sFileName))
            End If
        End Sub
        Private Sub Callback_Configuration()
            Dim configId As String
            If IncomingParameters.ContainsKey(qActions) And Me.isEditable Then
                If IncomingParameters(qActions).StartsWith("Set") Then
                    configId = IncomingParameters(qConfigurationID)
                    If Not configId Is Nothing Then
                        Try
                            Dim postValue As String = Request.Form(configId)
                            ConfigurationId = New Guid(configId)
                            Dim objSettings As New r2i.OWS.Controller
                            If IncomingParameters(qActions) = "Set" Then
                                objSettings.UpdateSetting(ConfigurationId, postValue, Me.UserInfo.UserName)
                                Dim x As Settings = r2i.OWS.Framework.Settings.Deserialize(postValue)
                                If Not x Is Nothing Then
                                    Me.Cache.Item("OWS" & ConfigurationId.ToString) = r2i.OWS.Framework.Settings.Clone(x)
                                Else
                                    Me.Cache.Item("OWS" & ConfigurationId.ToString) = Nothing
                                End If
                            ElseIf IncomingParameters(qActions) = "SetDraft" Then
                                Dim UserInfo As IUser = AbstractFactory.Instance.UserController.CurrentUser
                                objSettings.UpdateSettingDraft(UserInfo.Id, ConfigurationId, postValue)
                            End If
                            Page.Response.Write("{""Code"":""" & MessageResponseEnum.ACK.ToString & """,""Value"":""""}")
                        Catch ex As Exception
                            Page.Response.Write("{""Code"":""" & MessageResponseEnum.ERR.ToString & """,""Value"":""" & Utility.HTMLEncode(ex.ToString) & """}")
                        End Try
                    Else
                        'TODO: implements Exception
                    End If
                End If
                If IncomingParameters(qActions) = "Get" Then
                    configId = IncomingParameters(qConfigurationID)
                    If Not configId Is Nothing Then
                        ConfigurationId = New Guid(configId)
                        Dim sController As New Controller
                        Dim strCurrentSource As String = sController.GetSetting(ConfigurationId)
                        Dim xmlToJsonConv As New JsonConversion
                        Dim jsonText As String = xmlToJsonConv.GetJsonStructure(ConfigurationId, strCurrentSource)
                        Page.Response.Write(jsonText)
                    Else
                        'TODO: implements Exception configurationId not available
                    End If
                End If

                If IncomingParameters(qActions) = "GetConfigList" Then
                    Dim sController As New Controller
                    Dim dict As Dictionary(Of String, String) = sController.GetConfigurationDictionaryList()
                    Dim jsConv As New JsonConversion
                    Dim json As String = jsConv.GetJsonFromDataset(dict)
                    Page.Response.Write(json)
                End If


                If IncomingParameters(qActions) = "GetRegions" Then
                    configId = IncomingParameters(qConfigurationID)
                    If Not configId Is Nothing Then
                        Dim sRegions As String = ""

                        Dim miRegions As List(Of MessageActionItem) = r2i.OWS.Framework.Utilities.Engine.Utility.GetRegions(configId)

                        If Not miRegions Is Nothing Then
                            For Each miRegion As MessageActionItem In miRegions
                                If sRegions.Length > 0 Then sRegions &= ","
                                sRegions &= """" & miRegion.Parameters("Name") & """"
                            Next
                        End If
                        sRegions = "{""Regions"":[" & sRegions & "]}"
                        Page.Response.Write(sRegions)
                    Else
                        'TODO: implements Exception configurationId not available
                    End If
                End If

                If IncomingParameters(qActions) = "Exec" AndAlso Not Me.UserInfo Is Nothing AndAlso Me.UserInfo.IsSuperUser Then
                    Dim q As String = Request.QueryString("q")
                    Dim cnn As String = Nothing
                    Try
                        cnn = Request.QueryString.Item("c")
                    Catch ex As Exception
                        cnn = Nothing
                    End Try
                    If Not q Is Nothing AndAlso q.Length > 0 Then
                        Dim sController As New Controller

                        Dim connectionString As String = AbstractFactory.Instance.EngineController.GetConnectionString()
                        If Not cnn Is Nothing Then
                            connectionString = cnn
                        End If
                        Dim ds As DataSet = sController.GetDataset(connectionString, q)

                        If Not ds Is Nothing AndAlso ds.Tables.Count > 0 Then
                            Page.Response.Write(JSON.JsonDataTable.ConvertTableToJson(ds.Tables(0), 5))
                        End If
                    Else
                        'TODO: implements Exception configurationId not available
                    End If
                End If
            End If
        End Sub


        Private Sub Callback_Draft()
            If IncomingParameters.ContainsKey(qActions) AndAlso Me.isEditable Then
                If IncomingParameters(qActions) = "Delete" Then
                    Dim configId As String = IncomingParameters(qConfigurationID)
                    If Not configId Is Nothing AndAlso configId.Length > 0 Then
                        Dim configurationId As New Guid(configId)
                        Dim objSettings As New r2i.OWS.Controller
                        objSettings.DeleteSettingDraft(Me.UserId, configurationId)
                    Else
                        'TODO: implements Exception configurationId not available
                    End If
                End If
            End If
        End Sub
        Private Sub Callback_Actions()
            'Get the Actions List
            'Serialize as a jason Object
            ' Dim actionsList As ArrayList
            Dim requestOK As Boolean = False
            'If Not Request.QueryString("lxConfigId") Is Nothing Then
            '    Dim lxConfigId As Integer
            '    If Integer.TryParse(Request.QueryString("lxConfigId"), lxConfigId) Then
            requestOK = True

            ' Dim maItem As MessageActionsComponents.MessageActionItem
            Dim name As String = ""

            'Get the listXConfig
            ' Prepare object
            Dim sController As New Controller
            'Dim tabid As Integer
            'Dim moduleid As Integer
            Dim config As String = IncomingParameters(qConfigurationID)
            If Not config Is Nothing AndAlso config.Length > 0 AndAlso Me.isEditable Then
                Dim configurationId As New Guid(config)
                Dim strCurrentSource As String = sController.GetSetting(configurationId)

                Dim xmlDoc As New System.Xml.XmlDocument()
                xmlDoc.LoadXml(strCurrentSource)

                Dim jsonText As String = Newtonsoft.Json.JavaScriptConvert.SerializeXmlNode(xmlDoc)
                Page.Response.Clear()
                Page.Response.Write(jsonText)
            Else
                'TODO: Implements exceptions configurationId not provided
            End If
        End Sub
        Private Sub Callback_Import()
            Dim postValue As String = Request.Form("Import")
            If Not postValue Is Nothing AndAlso postValue.Length > 0 Then
                'postValue = System.Web.HttpUtility.HtmlDecode(postValue)
                Dim xmlToJsonConv As New JsonConversion
                Dim jsonText As String = xmlToJsonConv.GetJsonStructure(postValue)
                Page.Response.Write(jsonText)
            Else
                'TODO: no import xml value
            End If
        End Sub

        Private Sub Support_ClearWebCache()
            If Not Me.Cache Is Nothing Then
                If Me.Cache.Count > 0 Then
                    Dim enumerator As System.Collections.IDictionaryEnumerator = Me.Cache.GetEnumerator
                    If Not enumerator Is Nothing Then
                        While enumerator.MoveNext()
                            Me.Cache.Remove(enumerator.Key)
                        End While
                    End If
                End If
            End If
        End Sub
        Private Sub Callback_Upgrade()
            Try
                ModuleId = IncomingParameters(qUpgradeModule)
                PageId = IncomingParameters(qUpgradePage)
                If Me.isEditable Then
                    Dim c As New Controller
                    Dim newConfigId As Guid = c.UpgradeConfiguration(PageId, ModuleId)
                    Dim jsonString As String = c.GetSetting(newConfigId)
                    If Not jsonString Is Nothing Then
                        If jsonString.TrimStart.StartsWith("<") Then
                            Dim xmlToJsonConv As New JsonConversion
                            Dim jsonText As String = xmlToJsonConv.GetJsonStructure(jsonString)
                            jsonString = jsonText
                            'Dim s As Settings = Settings.Deserialize(jsonText)
                            Dim s As Settings = r2i.OWS.Framework.Settings.Deserialize(jsonText)
                            If Not s Is Nothing Then
                                s.ConfigurationID = newConfigId.ToString
                                s.Name = "Upgrade: Module-" & ModuleId & " Tab-" & PageId
                                s.Version = 19
                                c.UpdateSetting(newConfigId, s.Serialize, Me.UserInfo.UserName)
                                Try
                                    Support_ClearWebCache()
                                Catch ex As Exception

                                End Try
                                Page.Response.Redirect("Admin.aspx?configurationId=" & newConfigId.ToString, True)
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception

            End Try
        End Sub
        Private Sub Render_Callback_Content(ByVal SortValue As String, ByVal SortState As String)
            If Not Page.Request.IsAuthenticated Then
                If Not UserInfo Is Nothing Then
                    UserInfo.Id = -1
                End If
            End If

            Page.Response.Clear()
            Select Case EncodingType
                Case EncodingTypeEnum.GENERAL, EncodingTypeEnum.HTML
                    Page.Response.ContentType = "text/html"
                Case EncodingTypeEnum.TEXT
                    Page.Response.ContentType = "text/plain"
                Case EncodingTypeEnum.XML
                    Page.Response.ContentType = "text/xml"
                Case EncodingTypeEnum.JSON
                    Page.Response.ContentType = "text/json"
            End Select

            If SiteInfo Is Nothing Then
                Throw New Exception("Site Information failed to load at this time")
            ElseIf Me.UserInfo Is Nothing Then
                Throw New Exception("User Information failed to load at this time")
            ElseIf Me.ModuleConfiguration Is Nothing AndAlso ResourceKey Is Nothing Then
                Throw New Exception("Module Information failed to load at this time")
            End If

            If Me.isViewable Then
                Load_Configuration()
                Load_Defaults()
                If Not Configuration Is Nothing Then
                    Dim msg As New SortedList(Of String, String)
                    'CHECK FOR SORT ACTION
                    Load_Sort(SortValue, SortState)

                    Me.Load_Engine(True)
                    Me.Load_Request()

                    Me.Control_ExecuteActions()

                    Dim sWriter As New IO.StreamWriter(Page.Response.OutputStream)
                    Render_Callback(sWriter)
                    sWriter.Close()

                    If Not _defaults.Data Is Nothing Then
                        _defaults.Data.Clear()
                        _defaults.Data.Dispose()
                    End If
                    _defaults.Data = Nothing
                End If
            End If
        End Sub
        Private Sub Render_Callback(ByRef Writer As IO.StreamWriter)
            Dim Query As String = _engine.RenderQuery(_defaults.Data, FilterField, FilterText, RecordsPerPage, CapturedMessages, _debugOutput)
            Dim customConnection As String = ""
            Dim tCacheName As String = Nothing
            Dim tCacheTime As String = Nothing
            Dim bCacheShared As Boolean = False
            If Not _engine.TemplateCacheTime Is Nothing AndAlso _engine.TemplateCacheTime.Length > 0 Then
                tCacheTime = _engine.RenderString(Nothing, _engine.TemplateCacheTime, Nothing, False, False, FilterText:=FilterText, FilterField:=FilterField, DebugWriter:=_debugOutput)
            End If
            If Not _engine.TemplateCacheName Is Nothing AndAlso _engine.TemplateCacheName.Length > 0 Then
                tCacheName = _engine.RenderString(Nothing, _engine.TemplateCacheName, Nothing, False, False, FilterText:=FilterText, FilterField:=FilterField, DebugWriter:=_debugOutput)
            End If
            If Not _engine.TemplateCacheShare Is Nothing AndAlso _engine.TemplateCacheShare.Length > 0 Then
                bCacheShared = Utility.CNullBool(_engine.RenderString(Nothing, _engine.TemplateCacheShare, Nothing, False, False, FilterText:=FilterText, FilterField:=FilterField, DebugWriter:=_debugOutput))
            End If
            If Not Configuration.customConnection Is Nothing AndAlso Configuration.customConnection.Length > 0 Then
                customConnection = _engine.RenderString(Nothing, Configuration.customConnection, Nothing, False, False, FilterText:=FilterText, FilterField:=FilterField, DebugWriter:=_debugOutput)
            End If
            Dim ds As DataSet = _engine.GetData(False, Query, Me.FilterField, Me.FilterText, _debugOutput, True, tCacheName, tCacheTime, bCacheShared, CustomConnection:=customConnection)
            If EncodingType = EncodingTypeEnum.GENERAL Then
                If Not _engine Is Nothing AndAlso _engine.TotalRecords > 0 Then
                    'WRITE THE RECORD COUNT
                    Writer.Write(_engine.TotalRecords.ToString.PadLeft(20, " "))
                Else
                    Writer.Write("0".PadLeft(20, " "))
                End If
                'RENDER IFRAME SUPPORT
                If _engine.Request.Form("lxiAJAXRESPONSE") = "1" Then
                    'THIS IS AN IFRAME REQUEST
                    Writer.Write("<AJAX><NOSCRIPT>")
                End If
            End If

            Dim ql As Integer
            If Not Query Is Nothing Then
                ql = Query.Length
            End If
            _engine.TableVariables = _defaults.Data
            _engine.Render(ds, ql, RecordsPerPage, PageNumber, Configuration.enableCustomPaging, Writer, Nothing, , _debugOutput)

            'DEBUG USED TO END HERE

            'RENDER IFRAME SUPPORT
            If EncodingType = EncodingTypeEnum.GENERAL AndAlso _engine.Request.Form("lxiAJAXRESPONSE") = "1" Then
                'THIS IS AN IFRAME REQUEST
                Writer.Write("</NOSCRIPT></AJAX>")
            End If
            Query = Nothing
        End Sub

        Private Sub Render_Callback_ReturnMessage()
            If _s_returnImage Is Nothing Then
                Dim bmp2 As New System.Drawing.Bitmap(1, 1)
                Dim g As System.Drawing.Graphics
                g = System.Drawing.Graphics.FromImage(bmp2)
                g.FillRectangle(System.Drawing.Brushes.White, 0, 0, 1, 1)
                _s_returnImage = bmp2
            End If

            Page.Response.ContentType = "image/jpeg"
            _s_returnImage.Save(Page.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Jpeg)
            Page.Response.End()
        End Sub
        Public Sub Log_Exception(ByVal Ex As Exception)
            Debug.WriteLine("ERROR: " & Ex.ToString())
            Page.Response.Write("<div style='border: 1px solid red; font-family: arial; font-size: 11px; color: #FFFFFF; background: #FF0000;'>" & "<b>Open Web Studio Fatal Exception:</b><br>" & Ex.ToString & "</div>")
        End Sub

        ''' <summary>
        ''' Use this to perform custom callback actions (like, for loading lists, etc.)
        ''' </summary>
        ''' <remarks></remarks>
        Protected Overridable Sub Callback_Custom()
        End Sub
#End Region
#Region "Rendering"
        Public Sub Control_PreRender(ByRef sb As Text.StringBuilder)
            If Not _skipfordebug_ Then
                Try
                    _engine.CurrentPage = Me.PageNumber
                    _engine.RecordsPerPage = Me.RecordsPerPage
                    Render_Scripts(sb)
                    If Not Configuration Is Nothing Then
                        'Generate the Javascript References
                        '//****BuildJavascript()
                        Control_ExecuteActions()
                        Me.PageNumber = _engine.PageCurrent
                        Me.RecordsPerPage = _engine.RecordsPerPage
                    End If
                Catch ex As Exception
                    'ProcessModuleLoadException(Me, ex)
                End Try
            End If
        End Sub
        Public Sub Control_Render(ByRef sb As Text.StringBuilder)
            If Not _skipfordebug_ Then
                Try
                    Render_Title()
                    Render_Header(sb)
                    Render_Functional_Header(sb)
                    Render_ContentStart(sb)
                    Render_Content(sb)
                    Render_PageNumbers(sb)
                    Render_ContentEnd(sb)
                    Render_Scripts_Callback(sb)
                    Render_Footer(sb)
                Catch ex As Exception
                    'ProcessModuleLoadException(Me, ex)
                End Try
                'Debugger_End("Control")
            End If
        End Sub

        Private Function Load_PageNumbers() As DataTable
            If Configuration.enablePageSelection AndAlso Not Configuration.enableAJAX Then
                Dim returnvalue As String = ""
                Dim PageLinksPerPage As Integer = 10
                Dim TotalPages As Integer = _engine.TotalPages
                'Dim PageCurrent As Integer = PageNumber + 1

                If _engine.RecordsPerPage <= 0 Then
                    TotalPages = 1
                Else
                    TotalPages = Convert.ToInt32(Math.Ceiling(CType(_engine.TotalRecords / _engine.RecordsPerPage, Double))) - 1
                End If

                Dim ht As New DataTable
                ht.Columns.Add("Name")
                ht.Columns.Add("Number")
                ht.Columns.Add("Enabled")
                Dim tmpRow As DataRow

                Dim LowNum As Integer = 0
                Dim HighNum As Integer = CType(TotalPages, Integer)
                Dim MinNum As Integer = 1
                Dim MaxNum As Integer = HighNum

                Dim tmpNum As Double
                tmpNum = _engine.PageCurrent - PageLinksPerPage / 2
                'If tmpNum < 1 Then tmpNum = 1

                If _engine.PageCurrent > (PageLinksPerPage / 2) Then
                    LowNum = CType(Math.Floor(tmpNum), Integer)
                End If

                If CType(TotalPages, Integer) <= PageLinksPerPage Then
                    HighNum = CType(TotalPages, Integer)
                Else
                    HighNum = LowNum + PageLinksPerPage - 1
                End If

                If HighNum > CType(TotalPages, Integer) Then
                    HighNum = CType(TotalPages, Integer)
                    If HighNum - LowNum < PageLinksPerPage Then
                        LowNum = HighNum - PageLinksPerPage + 1
                    End If
                End If

                If HighNum > CType(TotalPages, Integer) Then HighNum = CType(TotalPages, Integer)
                'If LowNum < 1 Then LowNum = 1

                Dim i As Integer
                If HighNum > LowNum Then

                    tmpRow = ht.NewRow
                    tmpRow("Name") = "Previous"
                    'tmpRow("Number") = _engine.PageCurrent - 1
                    tmpRow("Number") = _engine.PageCurrent
                    If _engine.PageCurrent > 0 Then
                        tmpRow("Enabled") = True
                    Else
                        tmpRow("Enabled") = False
                    End If
                    ht.Rows.Add(tmpRow)



                    tmpRow = ht.NewRow
                    tmpRow("Name") = "First"
                    tmpRow("Number") = MinNum
                    If _engine.PageCurrent > 0 Then
                        tmpRow("Enabled") = True
                    Else
                        tmpRow("Enabled") = False
                    End If
                    ht.Rows.Add(tmpRow)

                    For i = LowNum To HighNum
                        tmpRow = ht.NewRow
                        tmpRow("Name") = (i + 1)
                        tmpRow("Number") = i + 1
                        If i = _engine.PageCurrent Then
                            tmpRow("Enabled") = False
                        Else
                            tmpRow("Enabled") = True
                        End If
                        ht.Rows.Add(tmpRow)
                    Next


                    tmpRow = ht.NewRow
                    tmpRow("Name") = "Last"
                    tmpRow("Number") = MaxNum + 1
                    If _engine.PageCurrent < MaxNum Then
                        tmpRow("Enabled") = True
                    Else
                        tmpRow("Enabled") = False
                    End If
                    ht.Rows.Add(tmpRow)


                    tmpRow = ht.NewRow
                    tmpRow("Name") = "Next"
                    tmpRow("Number") = _engine.PageCurrent + 1 + 1
                    If _engine.PageCurrent < MaxNum Then
                        tmpRow("Enabled") = True
                    Else
                        tmpRow("Enabled") = False
                    End If
                    ht.Rows.Add(tmpRow)

                End If


                If HighNum < 1 Then
                    Return Nothing
                Else
                    Return ht
                End If
            End If
            Return Nothing
        End Function
        Public Sub Render_Functional_Header(ByRef sb As Text.StringBuilder)
            Static baseHeader As String = "<table width=""100%"" border=""0"">"
            Static spRow As String = "<tr><td class=""Normal"" align=""left"">{0}</td><td class=""Normal"" align=""right"">{1}</td></tr>"
            Static alphaRow As String = "<tr><td class=""Normal"" colspan=""2"" align=""center"">{0}</td></tr>"
            Static baseFooter As String = "</table>"

            Dim ssb As New Text.StringBuilder
            Render_Search(ssb)
            Dim rsb As New Text.StringBuilder
            Render_RecordsPerPage(rsb)
            Dim asb As New Text.StringBuilder
            Render_Alphabet(asb)
            If ssb.Length > 0 OrElse rsb.Length > 0 OrElse asb.Length > 0 Then
                sb.Append(baseHeader)
            End If
            If ssb.Length > 0 OrElse rsb.Length > 0 Then
                If ssb.Length = 0 Then
                    ssb.Append("&nbsp;")
                End If
                If rsb.Length = 0 Then
                    rsb.Append("&nbsp;")
                End If
                sb.AppendFormat(spRow, ssb.ToString, rsb.ToString)
            End If
            If asb.Length > 0 Then
                sb.AppendFormat(alphaRow, asb.ToString)
            End If
            If ssb.Length > 0 OrElse rsb.Length > 0 OrElse asb.Length > 0 Then
                sb.AppendFormat(baseFooter)
            End If
            ssb.Length = 0
            asb.Length = 0
            rsb.Length = 0
        End Sub

        Public Sub Debugger_Open(ByVal createDebug As Boolean)
            If Configuration.canDebug(UserInfo, SiteInfo, Me.isEditable) Then
                If createDebug Then
                    _debugOutput = New r2i.OWS.Framework.Debugger
                    If isCallback Then
                        _debugOutput.AppendStamp("Ajax Callback Request")
                    Else
                        _debugOutput.AppendStamp("Page Load Request")
                    End If
                End If
            End If
        End Sub
        Public Sub Debugger_Close(ByVal Name As String)
            Try
                If Not _debugOutput Is Nothing Then
                    If Configuration.canDebug(_engine.UserInfo, _engine.PortalSettings, Me.isEditable) Then
                        'Dim rndNumbers As New Random
                        'Dim rndNumber As Integer = rndNumbers.Next

                        'If Not isCallback Then
                        '    sb.Append("<div id=""owsDebug" & Me.ModuleId & "_" & rndNumber & "_Attach"" style=""display:none; width: 100%; border-top: 1px dotted #cccccc; font-family: arial; font-size: 11px; color: #cccccc;text-align:center;""><a href=""#"" onclick=""return OWSDebug.Open('" & Me.ModuleId & "','" & Me.BasePath & "Debug.html');"">&lt;&lt;&nbsp;Show Debug Window&nbsp;&gt;&gt;</a></div>")
                        'End If
                        'sb.Append("<script type=""text/javascript"">OWSDebug.Write('" & Me.ModuleId & "','owsDebug" & Me.ModuleId & "_" & rndNumber & "','" & _debugOutput.ToString & "');</script>")

                        Controller.AddLog(Configuration.ConfigurationID, Me.UserId, "Debug - " & Name, _debugOutput.ToString, Me.Session.SessionID)
                    End If
                    _debugOutput.Close()
                    _debugOutput.Dispose()
                End If
            Catch ex As Exception

            End Try
        End Sub
        Public Sub Control_ExecuteActions()
            If Not _debugOutput Is Nothing Then
                _debugOutput.AppendHeader(Me.ModuleId, "Actions", "actions", False)
            End If
            _engine.ExecuteActions(_defaults.Data, Me.FilterField, Me.FilterText, _debugOutput)
            If Not _debugOutput Is Nothing Then
                _debugOutput.AppendFooter(True)
            End If
        End Sub

        Public Sub Render_Scripts(ByRef sb As Text.StringBuilder)
            ''GENERAL SCRIPT
            'Me.RegisterScriptBlock("OWS_General", "<script type=""text/javascript"" src=""" & Me.BasePath & "Scripts/ows.js?v=" & _JAVASCRIPTVERSION & """></script>")
            ''UTILITY SCRIPT
            'If Configuration.includeJavascriptUtilities Then
            '    Me.RegisterScriptBlock("OWS_Utilities", "<script type=""text/javascript"" src=""" & BasePath & "Scripts/OWS.Utilities.js?v=" & _JAVASCRIPTVERSION & """></script>")
            'End If
            ''VALIDATION SCRIPT
            'If Configuration.includeJavascriptValidation Then
            '    Me.RegisterScriptBlock("OWS_Validation", "<script type=""text/javascript"" src=""" & BasePath & "Scripts/OWS.Validation.js?v=" & _JAVASCRIPTVERSION & """></script>")
            'End If
            ''DEBUG SCRIPT
            'If Configuration.canDebug(_engine.UserInfo, _engine.PortalSettings, Me.isEditable) Then
            '    If Not Page.ClientScript.IsClientScriptBlockRegistered("OWSDEBUG") Then
            '        Page.ClientScript.RegisterClientScriptBlock(GetType(String), "OWSDEBUG", "<script type=""text/javascript"" src=""" & BasePath & "Scripts/OWS.Debug.js?v=" & _JAVASCRIPTVERSION & """></script>")
            '    End If
            'End If
            Dim sectionInclude As r2i.OWS.Framework.Config.SectionItem
            Dim sectionInclusions As List(Of r2i.OWS.Framework.Config.SectionItem)

            sectionInclusions = r2i.OWS.Framework.Config.Items(Config.Section.Wrapper, Config.SectionType.UI)
            If Not sectionInclusions Is Nothing AndAlso sectionInclusions.Count > 0 Then
                For Each sectionInclude In sectionInclusions
                    If Not Page.ClientScript.IsClientScriptBlockRegistered("OWS." & sectionInclude.Name) Then
                        If sectionInclude.Name.ToUpper = "OWS.UTILITIES" Then
                            If Configuration.includeJavascriptUtilities Then
                                Me.RegisterScriptBlock("OWS." & sectionInclude.Name, "<script type=""text/javascript"" src=""" & Me.BasePath & sectionInclude.Type & "?v=" & r2i.OWS.UI.OpenControlBase._JAVASCRIPTVERSION & """></script>")
                            End If
                        ElseIf sectionInclude.Name.ToUpper = "OWS.VALIDATION" Then
                            If Configuration.includeJavascriptValidation Then
                                Me.RegisterScriptBlock("OWS." & sectionInclude.Name, "<script type=""text/javascript"" src=""" & Me.BasePath & sectionInclude.Type & "?v=" & r2i.OWS.UI.OpenControlBase._JAVASCRIPTVERSION & """></script>")
                            End If
                        Else
                            If (Not sectionInclude.Required Is Nothing AndAlso sectionInclude.Required.ToLower = "true") OrElse (Not Configuration.javascriptInclude Is Nothing AndAlso Array.IndexOf(Configuration.javascriptInclude, sectionInclude.Name) >= 0) Then
                                If sectionInclude.Path Is Nothing OrElse sectionInclude.Path.Length = 0 Then
                                    sectionInclude.Path = Me.BasePath
                                End If
                                If sectionInclude.Mime Is Nothing OrElse sectionInclude.Mime.Length = 0 OrElse sectionInclude.Mime = "text/javascript" Then
                                    Me.RegisterScriptBlock("OWS." & sectionInclude.Name, "<script type=""text/javascript"" src=""" & sectionInclude.Path & sectionInclude.Type & "?v=" & r2i.OWS.UI.OpenControlBase._JAVASCRIPTVERSION & """></script>")
                                Else
                                    Select Case sectionInclude.Mime.ToLower
                                        Case "text/css"
                                            Me.RegisterScriptBlock("OWS." & sectionInclude.Name, "<link rel=""stylesheet"" type=""text/css"" href=""" & sectionInclude.Path & sectionInclude.Type & "?v=" & r2i.OWS.UI.OpenControlBase._JAVASCRIPTVERSION & """/>")
                                        Case "text/plain"
                                            Me.RegisterScriptBlock("OWS." & sectionInclude.Name, sectionInclude.Type)
                                    End Select
                                End If
                            End If
                        End If
                    End If
                Next
            End If
            sectionInclusions = r2i.OWS.Framework.Config.Items(Config.Section.General, Config.SectionType.UI)
            If Not sectionInclusions Is Nothing AndAlso sectionInclusions.Count > 0 Then
                For Each sectionInclude In sectionInclusions
                    If Not Page.ClientScript.IsClientScriptBlockRegistered("OWS." & sectionInclude.Name) Then
                        If sectionInclude.Name.ToUpper = "OWS.UTILITIES" Then
                            If Configuration.includeJavascriptUtilities Then
                                Me.RegisterScriptBlock("OWS." & sectionInclude.Name, "<script type=""text/javascript"" src=""" & Me.BasePath & sectionInclude.Type & "?v=" & r2i.OWS.UI.OpenControlBase._JAVASCRIPTVERSION & """></script>")
                            End If
                        ElseIf sectionInclude.Name.ToUpper = "OWS.VALIDATION" Then
                            If Configuration.includeJavascriptValidation Then
                                Me.RegisterScriptBlock("OWS." & sectionInclude.Name, "<script type=""text/javascript"" src=""" & Me.BasePath & sectionInclude.Type & "?v=" & r2i.OWS.UI.OpenControlBase._JAVASCRIPTVERSION & """></script>")
                            End If
                        Else
                            If (Not sectionInclude.Required Is Nothing AndAlso sectionInclude.Required.ToLower = "true") OrElse (Not Configuration.javascriptInclude Is Nothing AndAlso Array.IndexOf(Configuration.javascriptInclude, sectionInclude.Name) >= 0) Then
                                If sectionInclude.Path Is Nothing OrElse sectionInclude.Path.Length = 0 Then
                                    sectionInclude.Path = Me.BasePath
                                End If
                                If sectionInclude.Mime Is Nothing OrElse sectionInclude.Mime.Length = 0 OrElse sectionInclude.Mime = "text/javascript" Then
                                    Me.RegisterScriptBlock("OWS." & sectionInclude.Name, "<script type=""text/javascript"" src=""" & sectionInclude.Path & sectionInclude.Type & "?v=" & r2i.OWS.UI.OpenControlBase._JAVASCRIPTVERSION & """></script>")
                                Else
                                    Select Case sectionInclude.Mime.ToLower
                                        Case "text/css"
                                            Me.RegisterScriptBlock("OWS." & sectionInclude.Name, "<link rel=""stylesheet"" type=""text/css"" href=""" & sectionInclude.Path & sectionInclude.Type & "?v=" & r2i.OWS.UI.OpenControlBase._JAVASCRIPTVERSION & """/>")
                                        Case "text/plain"
                                            Me.RegisterScriptBlock("OWS." & sectionInclude.Name, sectionInclude.Type)
                                    End Select
                                End If
                            End If
                        End If
                    End If
                Next
            End If
        End Sub
        Public Sub Render_Scripts_Callback(ByRef sb As Text.StringBuilder)
            If Not SuppressAJAX() Then
                sb.Append("<script type=""text/javascript"">" & vbCrLf)
                sb.Append("/* <![CDATA[ */" & vbCrLf)
                Dim enable As String = "false"
                Dim historyPager As String = ""
                Dim refresh As String = "-1"
                Dim ajaxPager As String = ""
                If Not (Configuration.enableAJAX And Not Configuration.enableAJAXManual) Then
                    enable = "true"
                End If
                If Configuration.enableAJAXPageHistory Then
                    If Not Configuration.customAJAXPageHistory Is Nothing AndAlso Configuration.customAJAXPageHistory.Length > 0 Then
                        historyPager = Configuration.customAJAXPageHistory
                    Else
                        historyPager = "Page" & Me.ModuleId
                    End If
                    historyPager = historyPager.Replace("'", "")
                End If
                If Not Configuration.autoRefreshInterval Is Nothing AndAlso IsNumeric(Configuration.autoRefreshInterval) Then
                    refresh = Configuration.autoRefreshInterval.ToString
                End If
                If Configuration.enableAJAXPaging AndAlso Not Configuration.enableAJAX AndAlso Not Configuration.enableAJAXManual Then
                    enable = _engine.TotalRecords
                End If
                Dim onload As String = "null"
                If Not Configuration.javascriptOnComplete Is Nothing AndAlso Configuration.javascriptOnComplete.Length > 0 Then
                    onload = "'" & Configuration.javascriptOnComplete.Replace("'", "\'") & "'"
                End If
                'TODO: Pass Onload
                sb.Append("ows.Create('" & Me.ModuleId & "'," & _engine.PageCurrent - 1 & "," & _engine.RecordsPerPage & ",'" & Me.ListSource & "','" & ModulePath & "','" & BasePath & "'," & enable & "," & refresh & ",'" & historyPager & "',null,null,null," & onload & ");" & vbCrLf)
                sb.Append("/* ]]> */" & vbCrLf)
                sb.Append("</script>" & vbCrLf)
                If Me.Load_Ajax Then
                    'REMOVED - No longer required thanks to the logic from the Progressing Enhancement

                    'If Not Configuration.BotPageVariableName Is Nothing AndAlso Configuration.BotPageVariableName.Length > 0 Then
                    '    If Not Configuration.BotNonAjaxText Is Nothing AndAlso Configuration.BotNonAjaxText.Length > 0 Then
                    '        sb.Append("<noscript>" & "<a href=""" & Render_PageNumber(1, False) & """>" & Configuration.BotNonAjaxText & "</a></noscript>")
                    '    Else
                    '        sb.Append("<noscript>" & "<a href=""" & Render_PageNumber(1, False) & """>" & "Click Here for the Non-AJAX Content" & "</a></noscript>")
                    '    End If
                    'End If
                End If
            End If
        End Sub
        Private Function Load_Ajax() As Boolean
            'TODO: ADD INGORE AJAX
          return True
        End Function
        Public Function Render_PageNumber(ByVal Name As Object, ByVal PostBack As Boolean) As String
            If PostBack Then
                Return Page.ClientScript.GetPostBackEventReference(Me, "page=" & Name.ToString) & ";return false;"
            Else
                If IsNumeric(Name) Then
                    Dim URL As String = ""
                    If Me.Page.Request.QueryString.Item(Support_PageVariableName) Is Nothing Then
                        If Me.Page.Request.Url.ToString.IndexOf("?"c) >= 0 Then
                            URL = Me.Page.Request.Url.ToString & "&" & Support_PageVariableName() & "=" & Name.ToString
                        Else
                            URL = Me.Page.Request.Url.ToString & "?" & Support_PageVariableName() & "=" & Name.ToString
                        End If
                    Else
                        URL = Me.Page.Request.Url.ToString.Replace(Support_PageVariableName() & "=" & Me.Page.Request.QueryString(Support_PageVariableName), Support_PageVariableName() & "=" & Name.ToString)
                    End If
                    Return URL
                Else
                    Return "#"
                End If
            End If
        End Function
        Public Sub Render_Title()
            Try
                If Not Me.Configuration.Title Is Nothing AndAlso Me.Configuration.Title.Length > 0 AndAlso Not Me.Title Is Nothing Then
                    If TypeOf Me.Title Is System.Web.UI.WebControls.Label Then
                        Dim lbl As Label = CType(Me.Title, Label)
                        lbl.Text = _engine.RenderString(_defaults.Data, Me.Configuration.Title, Me.CapturedMessages, False, False, FilterText:=Me.FilterText, FilterField:=Me.FilterField, DebugWriter:=_debugOutput)
                    Else
                        Dim strt As String = _engine.RenderString(_defaults.Data, Me.Configuration.Title, Me.CapturedMessages, False, False, FilterText:=Me.FilterText, FilterField:=Me.FilterField, DebugWriter:=_debugOutput)
                        Me.Title.GetType().GetProperty("Text").SetValue(Me.Title, strt, Nothing)
                    End If
                End If
            Catch ex As Exception
            End Try
        End Sub
        Public Sub Render_Header(ByRef sb As Text.StringBuilder)
            Dim found As Boolean = False
            If Not Me.Header Is Nothing Then
                If TypeOf Header Is Label Then
                    Dim lbl As Label = CType(Header, Label)
                    lbl.Text = _engine.RenderString(_defaults.Data, Me.Configuration.Header, Me.CapturedMessages, False, False, FilterText:=Me.FilterText, FilterField:=Me.FilterField, DebugWriter:=_debugOutput)
                    found = True
                ElseIf TypeOf Header Is Literal Then
                    Dim ltl As Literal = CType(Header, Literal)
                    ltl.Text = _engine.RenderString(_defaults.Data, Me.Configuration.Header, Me.CapturedMessages, False, False, FilterText:=Me.FilterText, FilterField:=Me.FilterField, DebugWriter:=_debugOutput)
                    found = True
                End If
            End If
            If Not found Then
                sb.Append(_engine.RenderString(_defaults.Data, Me.Configuration.Header, Me.CapturedMessages, False, False, FilterText:=Me.FilterText, FilterField:=Me.FilterField, DebugWriter:=_debugOutput))
            End If

            'KMS - This was added due to the Module Caching of DotNetNuke. Whenever the module is cached, the register script blocks is not.
            Dim cachedBlocks As String = Me.CachedScriptBlocks
            If Not cachedBlocks Is Nothing Then
                sb.Append(cachedBlocks)
            End If
        End Sub
        Public Sub Render_Footer(ByRef sb As Text.StringBuilder)
            Dim found As Boolean = False
            If Not Me.Footer Is Nothing Then
                If TypeOf Footer Is Label Then
                    Dim lbl As Label = CType(Footer, Label)
                    lbl.Text = _engine.RenderString(_defaults.Data, Me.Configuration.Footer, Me.CapturedMessages, False, False, FilterText:=Me.FilterText, FilterField:=Me.FilterField, DebugWriter:=_debugOutput)
                    found = True
                ElseIf TypeOf Footer Is Literal Then
                    Dim ltl As Literal = CType(Footer, Literal)
                    ltl.Text = _engine.RenderString(_defaults.Data, Me.Configuration.Footer, Me.CapturedMessages, False, False, FilterText:=Me.FilterText, FilterField:=Me.FilterField, DebugWriter:=_debugOutput)
                    found = True
                End If
            End If
            If Not found Then
                sb.Append(_engine.RenderString(_defaults.Data, Me.Configuration.Footer, Me.CapturedMessages, False, False, FilterText:=Me.FilterText, FilterField:=Me.FilterField, DebugWriter:=_debugOutput))
            End If
        End Sub
        Public Sub Render_Search(ByRef sb As Text.StringBuilder)
            If Configuration.searchItems.Count > 0 Then
                Dim strSep As String = ""
                Dim strStart As String = "<table width=""100%"" border=""0""><tr><td style=""text-align:left;""><span class=""SubHead"">Search</span><br/><input name=""{0}"" value=""{2}""/><select name=""{1}"">"
                '0-txtName,1-selectName
                '0-buttonName,1-imageUrl
                'Dim strStop As String = "</select><input type=""image"" name=""{0}"" src=""{1}"" border=""0"" /></td></table>"
                Dim strFormat As String = "<option value=""{0}"" {1}>{2}</option>"
                Static strItemCommand As String = "Search"

                Dim strImage As String = "<img src=""" & Me.BasePath & "/images/search.gif" & """ border=""0"" />"
                Dim strStop As String = "</select>" & "<a href=""" & _GlobalPostBack & """>{2}</a>&nbsp;" & "</td></table>"

                Dim strItem As SearchOptionItem

                Dim b As Boolean = False
                sb.AppendFormat(strStart, Me.UniqueID & "_txtSearch", Me.UniqueID & "_ddlSearch", _defaults.SearchText)
                For Each strItem In Configuration.searchItems
                    If b Then
                        sb.Append(strSep)
                    End If
                    Dim isSelected As String = ""
                    If _defaults.SearchOption = strItem.SearchOption Then
                        isSelected = "selected"
                    End If
                    sb.AppendFormat(strFormat, strItem.SearchField, isSelected, strItem.SearchOption)
                Next
                sb.AppendFormat(strStop, "Search", "Search", strImage)
                'sb.AppendFormat(strStop, "btnSearch", Me.BasePath & "/images/search.gif")
            End If
        End Sub
        Public Sub Render_RecordsPerPage(ByRef sb As Text.StringBuilder)
            If Configuration.enableRecordsPerPage Then
                Dim strSep As String = ""
                Dim strStart As String = "<span class=""SubHead"">Records Per Page:</span><br/><select name=""{2}"" onchange=""" & _GlobalPostBack & """ language=""javascript"">"
                Dim strStop As String = "</select>"
                Dim strFormat As String = "<option {0} value=""{1}"">{2}</option>"
                Static strItemCommand As String = "PERPAGE"
                Dim strPageSelector As String = Me.UniqueID & "selRPP"
                Static strItems As String() = {"All", "10", "25", "50", "100", "250"}

                Dim strItem As String
                Dim b As Boolean = False
                sb.AppendFormat(strStart, strItemCommand, strItemCommand, strPageSelector)
                For Each strItem In strItems
                    If b Then
                        sb.Append(strSep)
                    End If
                    Dim iItem As Integer = -1
                    Dim sSelected As String = ""
                    If IsNumeric(strItem) Then
                        iItem = CInt(strItem)
                    End If
                    If iItem = Me.RecordsPerPage Then
                        sSelected = "selected"
                    End If
                    sb.AppendFormat(strFormat, sSelected, iItem.ToString, strItem)
                Next
                sb.Append(strStop)
            End If
        End Sub
        Public Sub Render_Alphabet(ByRef sb As Text.StringBuilder)
            If Configuration.enableAlphaFilter Then
                Dim strSep As String = "&nbsp;"
                Dim strStart As String = "<div style=""text-align:center;"">"
                Dim strStop As String = "</div>"
                Dim strFormat As String = "<a href=""" & _GlobalPostBack & """>{2}</a>&nbsp;"
                Static strItemCommand As String = "Alpha"
                Static strItems As String() = {"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "All"}

                Dim strItem As String
                Dim b As Boolean = False
                sb.Append(strStart)
                For Each strItem In strItems
                    If b Then
                        sb.Append(strSep)
                    End If
                    sb.AppendFormat(strFormat, strItemCommand, strItem, strItem)
                Next
                sb.Append(strStop)
            End If

        End Sub
        Public Function Render_ContentStart(ByRef sb As Text.StringBuilder) As Boolean
            '<div id="lxT<%=ModuleId%>" style="WIDTH: 100%">
            If Not SuppressAJAX() Then
                sb.AppendFormat("<" & _controlTag & " id=""lxT{0}"">", Me.ModuleId)
            End If
        End Function
        Public Function Render_ContentEnd(ByRef sb As Text.StringBuilder) As Boolean
            If Not SuppressAJAX() Then
                sb.Append("</" & _controlTag & ">")
                'AJAX SCRIPT
                If (Configuration.enableAJAXPaging OrElse Configuration.enableAJAX) AndAlso Not Configuration.enableAJAXCustomPaging Then
                    sb.Append("<span Class=""CommandButton"" id=""lxP" & Me.ModuleId & """></span>" & vbCrLf)
                End If
                If (Configuration.enableAJAXPaging OrElse Configuration.enableAJAX) AndAlso Not Configuration.enableAJAXCustomStatus Then
                    sb.Append("<span Class=""CommandButton"" id=""lxS" & Me.ModuleId & """ style=""width:100%; text-align: center;""></span>" & vbCrLf)
                End If
            End If
        End Function
        Public Function Render_Content(ByRef sb As Text.StringBuilder) As Boolean
            If isViewable AndAlso (Not Load_Ajax() OrElse Not Configuration.enableAJAX) OrElse Configuration.OutputType <> r2i.OWS.Framework.Settings.RenderType.Default Then
                Dim Query As String = _engine.RenderQuery(_defaults.Data, Me.FilterField, Me.FilterText, Me.RecordsPerPage, Me.CapturedMessages, _debugOutput)
                Dim customConnection As String = Nothing
                Dim tCacheName As String = Nothing
                Dim tCacheTime As String = Nothing
                Dim bCacheShared As Boolean = False
                If Not _engine.TemplateCacheTime Is Nothing AndAlso _engine.TemplateCacheTime.Length > 0 Then
                    tCacheTime = _engine.RenderString(_defaults.Data, _engine.TemplateCacheTime, Me.CapturedMessages, False, False, FilterText:=Me.FilterText, FilterField:=Me.FilterField, DebugWriter:=_debugOutput)
                End If
                If Not _engine.TemplateCacheName Is Nothing AndAlso _engine.TemplateCacheName.Length > 0 Then
                    tCacheName = _engine.RenderString(_defaults.Data, _engine.TemplateCacheName, Me.CapturedMessages, False, False, FilterText:=Me.FilterText, FilterField:=Me.FilterField, DebugWriter:=_debugOutput)
                End If
                If Not _engine.TemplateCacheShare Is Nothing AndAlso _engine.TemplateCacheShare.Length > 0 Then
                    bCacheShared = Utility.CNullBool(_engine.RenderString(_defaults.Data, _engine.TemplateCacheShare, Me.CapturedMessages, False, False, FilterText:=Me.FilterText, FilterField:=Me.FilterField, DebugWriter:=_debugOutput))
                End If
                If Not Configuration.customConnection Is Nothing AndAlso Configuration.customConnection.Length > 0 Then
                    customConnection = _engine.RenderString(_defaults.Data, Configuration.customConnection, Me.CapturedMessages, False, False, FilterText:=Me.FilterText, FilterField:=Me.FilterField, DebugWriter:=_debugOutput)
                End If

                Dim ds As DataSet = _engine.GetData(False, Query, Me.FilterField, Me.FilterText, _debugOutput, True, tCacheName, tCacheTime, bCacheShared, CustomConnection:=customConnection)

                Dim fileType As String = Nothing
                Dim fileExtension As String = Nothing
                Dim fileComplete As Boolean = False
                Dim fileName As String = Nothing
                Select Case Configuration.OutputType
                    Case r2i.OWS.Framework.Settings.RenderType.Excel
                        fileType = "application/vnd.ms-excel"
                        fileExtension = "xls"
                        fileName = Configuration.OutputFilename(fileExtension)
                    Case r2i.OWS.Framework.Settings.RenderType.Excel_Complete
                        fileType = "application/vnd.ms-excel"
                        fileExtension = "xls"
                        fileName = Configuration.OutputFilename(fileExtension)
                        fileComplete = True
                    Case r2i.OWS.Framework.Settings.RenderType.Delimited
                        fileType = "text/plain"
                        fileExtension = "csv"
                        fileName = Configuration.OutputFilename(fileExtension)
                        fileComplete = False
                    Case r2i.OWS.Framework.Settings.RenderType.Delimited_Complete
                        fileType = "text/plain"
                        fileExtension = "csv"
                        fileName = Configuration.OutputFilename(fileExtension)
                        fileComplete = True
                    Case r2i.OWS.Framework.Settings.RenderType.Report
                    Case r2i.OWS.Framework.Settings.RenderType.Report_Complete
                    Case r2i.OWS.Framework.Settings.RenderType.Word
                        fileType = "application/vnd.ms-word"
                        fileExtension = "doc"
                        fileName = Configuration.OutputFilename(fileExtension)
                    Case r2i.OWS.Framework.Settings.RenderType.Word_Complete
                        fileType = "application/vnd.ms-word"
                        fileExtension = "doc"
                        fileName = Configuration.OutputFilename(fileExtension)
                        fileComplete = True
                End Select
                _engine.TableVariables = _defaults.Data
                If Not fileType Is Nothing And Not ds Is Nothing Then
                    Dim ql As Integer
                    If Not Query Is Nothing Then
                        ql = Query.Length
                    End If
                    Dim rnd As New Random
                    Dim sPath As String = Me.MapPath(BasePath & "dl")
                    If Not IO.Directory.Exists(sPath) Then
                        IO.Directory.CreateDirectory(sPath)
                    End If

                    Dim sName As String = "OWS" & Me.ClientID & "_" & rnd.Next(342098, Integer.MaxValue) & "." & fileExtension
                    Dim sType As String = fileType

                    Dim fs As IO.FileStream = Nothing
                    Dim bGood As Boolean

                    Try
                        fs = New IO.FileStream(sPath & "/" & sName, IO.FileMode.Create)
                        Dim tw As New IO.StreamWriter(fs)
                        If Configuration.OutputType = r2i.OWS.Framework.Settings.RenderType.Delimited Or Configuration.OutputType = r2i.OWS.Framework.Settings.RenderType.Delimited_Complete Then
                            _engine.ExportData(ds, ql, _engine.RecordsPerPage, _engine.PageCurrent, Configuration.enableCustomPaging, tw, Me.CapturedMessages, fileComplete)
                        Else
                            _engine.Render(ds, ql, _engine.RecordsPerPage, _engine.PageCurrent, Configuration.enableCustomPaging, tw, Me.CapturedMessages, fileComplete, DebugWriter:=_debugOutput)
                        End If
                        tw.Flush()
                        bGood = True
                    Catch ex As Exception
                        'ROMAIN: 09/27/07
                        'TODO:Change exceptions
                        'DotNetNuke.Services.Exceptions.LogException(ex)
                        bGood = False
                    Finally
                        If Not fs Is Nothing Then
                            fs.Flush()
                            fs.Close()
                        End If
                        Configuration.OutputType(r2i.OWS.Framework.Settings.RenderType.Default)
                    End Try
                    If bGood Then
                        'Page.Response.Redirect(BasePath & "IM.aspx?lxA=D&tabid=" & Me.PageId & "&mid=" & Me.ModuleId & "&Download=" & Web.HttpUtility.UrlEncode(sName) & "&Type=" & Web.HttpUtility.UrlEncode(sType) & "&FileName=" & Web.HttpUtility.UrlEncode(fileName))
                        Page.Response.Redirect(BasePath & "IM.aspx?_OWS_=" & qAction & ":D," & qPageId & ":" & Me.PageId & "," & qModuleId & ":" & Me.ModuleId & "," & qPageModuleId & ":" & Me.PageModuleId & "," & qDownload & ":" & Web.HttpUtility.UrlEncode(sName) & "," & qType & ":" & Web.HttpUtility.UrlEncode(sType) & "," & qFilename & ":" & Web.HttpUtility.UrlEncode(fileName))
                    End If
                ElseIf isViewable Then
                    Dim output As New Text.StringBuilder
                    Dim writer As New IO.StringWriter(output)
                    Dim ql As Integer
                    If Not Query Is Nothing Then
                        ql = Query.Length
                    End If

                    If _engine.TotalRecords > 0 AndAlso _engine.RecordsPerPage * (_engine.PageCurrent) - _engine.RecordsPerPage >= _engine.TotalRecords Then
                        _engine.PageCurrent = 0
                    End If

                    _engine.Render(ds, ql, _engine.RecordsPerPage, _engine.PageCurrent, Configuration.enableCustomPaging, writer, Me.CapturedMessages, DebugWriter:=_debugOutput)
                    writer.Close()

                    sb.Append(output.ToString)

                    'lstItems.EnableViewState = False
                    'lstItems.Text = output.ToString
                    output = Nothing

                    'Me.ContainerControl.Visible = True
                    'If (xls.enableAJAX = False OrElse _ignoreAJAX) AndAlso xls.enableHide_OnNoQuery = True AndAlso (Query Is Nothing OrElse Query.Length = 0) AndAlso Not xls.isEditable(Me.UserInfo, Me.PortalSettings, Me.IsEditable) Then
                    '    Me.ContainerControl.Visible = False
                    'ElseIf (xls.enableAJAX = False OrElse _ignoreAJAX) AndAlso xls.enableHide_OnNoResults = True AndAlso Not Query Is Nothing AndAlso Query.Length > 0 AndAlso Not xls.isEditable(Me.UserInfo, Me.PortalSettings, Me.IsEditable) AndAlso _renderer.TotalRecords = 0 Then
                    '    Me.ContainerControl.Visible = False
                    'End If

                    Configuration.OutputType(r2i.OWS.Framework.Settings.RenderType.Default)
                End If
                If Not ds Is Nothing Then
                    If Not ds.ExtendedProperties.ContainsKey("isCached") Then
                        ds.Clear()
                        ds.Dispose()
                        ds = Nothing
                    End If
                End If
                Return True
            Else
                _engine.RecordsPerPage = RecordsPerPage
                Return False
                'PageNumbers.Visible = False
            End If
            If Configuration.enablePageSelection = False Then
                Return False
                'pnlPageNum.Visible = False
            End If
        End Function
        Public Sub Render_PageNumbers(ByRef sb As Text.StringBuilder)
            Dim dt As DataTable = Load_PageNumbers()
            If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
                Static ajaxHeader As String = "<noscript>"
                Static baseHeader As String = "<div class=""Normal OWSPager"" align=""center"">"
                Static enabledItem As String = "<a class=""OWSPage"" href=""{0}"" onclick=""{1}"">{2}</a>"
                Static disabledItem As String = "<a class=""OWSPageDisabled"">{0}</a>"
                Static baseSep As String = "&nbsp;"
                Static baseFooter As String = "</div>"
                Static ajaxFooter As String = "</noscript>"
                
                If Configuration.enableAJAXPaging Then
                    sb.Append(ajaxHeader)
                End If

                sb.Append(baseHeader)
                Dim dr As DataRow
                Dim sep As Boolean = False
                For Each dr In dt.Rows
                    If sep Then
                        sb.Append(baseSep)
                    End If
                    sep = True

                    Dim ub As Utilities.UrlBuilder
                    Dim uriobj As Uri = Nothing
                    If Uri.TryCreate(Request.RawUrl, UriKind.Absolute, uriobj) Then
                        ub = New Utilities.UrlBuilder(uriobj)
                    Else
                        ub = New Utilities.UrlBuilder(Request.Url)
                        If Request.RawUrl.Contains("?") Then
                            ub.Path = Request.RawUrl.Substring(0, Request.RawUrl.IndexOf("?"c))
                        Else
                            ub.Path = Request.RawUrl
                        End If
                    End If
                    If ub.QueryString.ContainsKey("TabID") Then
                        ub.QueryString.Remove("TabID")
                    End If
                    ub.QueryString(Me.Support_PageVariableName) = dr.Item("Number")
                    Dim baseLink As String = ub.ToString

                    Dim postLink As String = String.Format(_GlobalPostBack, "Page", (dr.Item("Number") - 1).ToString)
                    postLink = "window.setTimeout('" & postLink.Replace("'", "\'") & "',0);return false;"
                    If Not CType(dr.Item("Enabled"), Boolean) = True Then
                        sb.AppendFormat(disabledItem, dr.Item("Name").ToString)
                    Else
                        sb.AppendFormat(enabledItem, baseLink, postLink, dr.Item("Name").ToString)
                    End If
                Next
                sb.Append(baseFooter)
                If Configuration.enableAJAXPaging Then
                    sb.Append(ajaxFooter)
                End If
            End If
        End Sub
#End Region
#Region "Implementations"
        Public Function Load_ViewState(ByVal postDataKey As String, ByVal postCollection As System.Collections.Specialized.NameValueCollection) As Boolean Implements System.Web.UI.IPostBackDataHandler.LoadPostData

        End Function

        Public Sub Support_RaisePostDataChangedEvent() Implements System.Web.UI.IPostBackDataHandler.RaisePostDataChangedEvent

        End Sub

        Public Sub Support_RaisePostBackEvent(ByVal eventArgument As String) Implements System.Web.UI.IPostBackEventHandler.RaisePostBackEvent
            Load_Incoming(eventArgument)
        End Sub
#End Region
        Public Structure Defaults
            Public RecordsPerPage As Integer
            Public SearchOption As String
            Public SearchText As String
            Public Data As DataSet
            Public PageNumber As Integer
        End Structure

        Private Sub UI_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            If Not _skipfordebug_ Then
                Me.Control_Init()
            End If
        End Sub

        Private Sub UI_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not _skipfordebug_ Then
                Me.Control_Load()
            End If
        End Sub
        Private Sub UI_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Unload
            If Not _skipfordebug_ Then
                Debugger_Close(_requestType)
            End If
            Control_Unload()
        End Sub
        Private Sub Control_Unload()
            If Not _configuration Is Nothing Then
                _configuration = Nothing
            End If
            If Not _engine Is Nothing Then
                _engine.Dispose()
                _engine = Nothing
            End If
            If Not _debugOutput Is Nothing Then
                _debugOutput.Dispose()
                _debugOutput = Nothing
            End If
            If Not _incomingParameters Is Nothing Then
                _incomingParameters = Nothing
            End If
            If Not _output Is Nothing Then
                _output = Nothing
            End If
            _defaults = Nothing
        End Sub

        Private Sub UI_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            If Not _skipfordebug_ Then
                If Not isCallback Then
                    Me.Control_PreRender(_output)
                    If _engine.EndResponse Then
                        Try
                            'WRITE DIRECTLY TO THE RESPONSE INSTEAD
                            Dim swriter As New System.IO.StringWriter(_output)
                            Dim outputwriter As New System.Web.UI.HtmlTextWriter(swriter)
                            'Don't write things from the Platform's page request/response. Just our response.
                            'MyBase.Render(outputwriter)
                            Me.Render_Content(_output)
                            swriter.Flush()
                            Context.Response.Write(_output.ToString)
                            Try
                                outputwriter.Close()
                                outputwriter.Dispose()
                                swriter.Close()
                                swriter.Dispose()
                            Catch ex As Exception
                            End Try
                            outputwriter = Nothing
                            swriter = Nothing
                            _output = Nothing
                            Context.Response.Flush()
                        Catch ex As Exception

                        End Try
                        Try
                            Context.Response.End()
                        Catch tae As Threading.ThreadAbortException
                            'DO NOTHING  - EXPECTED
                        Catch ex As Exception
                            'DO NOTHING
                        End Try
                    Else

                        Me.Control_Render(_output)
                    End If
                End If
            End If
        End Sub
        Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
            If Not _skipfordebug_ Then
                If Not isCallback Then
                    If Not _engine.EndResponse Then
                        'MyBase.Render(writer)
                        writer.Write(_output.ToString)
                        _output = Nothing
                    End If
                End If
            End If
        End Sub

        Public Sub New()
            ' r2i.OWS.Engine.Load()
        End Sub
    End Class
End Namespace