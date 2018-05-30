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
Imports System.Web
Imports System.Web.UI
Imports System.Security
Imports DotNetNuke.Entities.Modules.Communications
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Actions
Imports DotNetNuke.Security
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization

Public Class OpenCallbackControl
    Inherits r2i.OWS.UI.OpenControlBase


    Private _moduleInfo As IModuleInfo
    Private _portalSettings As IPortalSettings
    Private _parent As DotNetNuke.Framework.CDefault
    Private _messages As SortedList(Of String, String)
    Private _userInfo As IUser
    Private _moduleID As String
    Private _pagemoduleID As String
    Private _pageID As String
    Private _configurationID As Guid
    Private _resourceKey As String
    Private _resourceFile As String
    Private _usepagedefaults As Boolean = False
    Private _configLoaded As Boolean
    Private _configLocked As Boolean = False


    Public Overrides Property Cache() As System.Web.Caching.Cache
        Get
            Return _parent.Cache
        End Get
        Set(ByVal value As System.Web.Caching.Cache)
            'CANNOT SET - READONLY
        End Set
    End Property

    Public Overrides Sub ClearCache()
        Try
            DotNetNuke.Common.Utilities.DataCache.ClearHostCache(True)
        Catch ex As Exception
        End Try
    End Sub
    Public Overrides Sub ClearSiteCache()
        Try
            DotNetNuke.Common.Utilities.DataCache.ClearPortalCache(CInt(_portalSettings.PortalId), True)
        Catch ex As Exception
        End Try
    End Sub
    Public Overrides Sub ClearPageCache()
        Try
            'DotNetNuke.Common.Utilities.DataCache.ClearTabCache(CInt(_pageID), CInt(_portalSettings.PortalId))
            DotNetNuke.Common.Utilities.DataCache.ClearModuleCache(CInt(_pageID))
        Catch ex As Exception
        End Try
    End Sub

    Public Overrides ReadOnly Property CapturedMessages() As System.Collections.Generic.SortedList(Of String, String)
        Get
            If _messages Is Nothing Then
                _messages = New SortedList(Of String, String)
            End If
            Return _messages
        End Get
    End Property


    Public Overrides ReadOnly Property BasePath() As String
        Get
            If _parent.TemplateSourceDirectory.EndsWith("/") Or Me.TemplateSourceDirectory.EndsWith("\") Then
                Return _parent.TemplateSourceDirectory
            Else
                Return _parent.TemplateSourceDirectory & "/"
            End If
        End Get
    End Property

    Public Overrides Property ConfigurationId() As System.Guid
        Get
            Return _configurationID
        End Get
        Set(ByVal value As System.Guid)
            _configurationID = value
        End Set
    End Property


    Public Overrides Property ConfigurationLocked() As Boolean
        Get
            Return _configLocked
        End Get
        Set(ByVal value As Boolean)
            _configLocked = value
        End Set
    End Property

    Overrides ReadOnly Property Request() As HttpRequest
        Get
            If Not Page Is Nothing Then
                Return Page.Request
            End If
            Return Nothing
        End Get
    End Property
    Overrides ReadOnly Property Session() As Utilities.GenericSession
        Get
            Dim _session As SessionState.HttpSessionState = Nothing
            Try
                If Not Page Is Nothing Then
                    _session = Page.Session
                End If
                If _session Is Nothing Then
                    _session = System.Web.HttpContext.Current.Session
                End If
            Catch ex As Exception

            End Try
            If _session Is Nothing Then
                If Not System.Web.HttpContext.Current Is Nothing Then
                    Dim s As Utilities.GenericSession = Nothing
                    If System.Web.HttpContext.Current.Items.Item("OWS.EMPTYSESSION") Is Nothing Then
                        s = New Utilities.GenericSession()
                        System.Web.HttpContext.Current.Items.Item("OWS.EMPTYSESSION") = s
                    Else
                        s = CType(System.Web.HttpContext.Current.Items.Item("OWS.EMPTYSESSION"), Utilities.GenericSession)
                    End If
                    Return s
                End If
            Else
                Return New Utilities.GenericSession(_session)
            End If
            Return Nothing
        End Get
    End Property

    Public Overrides Property FilterField() As String
        Get
            'VIEWSTATE WONT WORK IN THIS CONTEXT
            If Not Session.Item(Me.ConfigurationId.ToString & Me.UserId & "." & Me.ModuleId & "FILTERFIELD") Is Nothing Then
                Return Session.Item(Me.ConfigurationId.ToString & Me.UserId & "." & Me.ModuleId & "FILTERFIELD").ToString
            Else
                Return ""
            End If
        End Get
        Set(ByVal value As String)
            Session.Item(Me.ConfigurationId.ToString & Me.UserId & "." & Me.ModuleId & "FILTERFIELD") = value
        End Set
    End Property

    Public Overrides Property FilterText() As String
        Get
            'VIEWSTATE WONT WORK IN THIS CONTEXT
            If Not Session.Item(Me.ConfigurationId.ToString & Me.UserId & "." & Me.ModuleId & "FILTERTEXT") Is Nothing Then
                Return Session.Item(Me.ConfigurationId.ToString & Me.UserId & "." & Me.ModuleId & "FILTERTEXT").ToString
            Else
                Return ""
            End If
        End Get
        Set(ByVal value As String)
            Session.Item(Me.ConfigurationId.ToString & Me.UserId & "." & Me.ModuleId & "FILTERTEXT") = value
        End Set
    End Property

    Public Overrides ReadOnly Property Footer() As System.Web.UI.Control
        Get
            Return Nothing
        End Get
    End Property

    Public Overrides ReadOnly Property Title() As System.Web.UI.Control
        Get
            Return Nothing
        End Get
    End Property

    Public Overrides ReadOnly Property Header() As System.Web.UI.Control
        Get
            Return Nothing
        End Get
    End Property

    Public Overrides ReadOnly Property IsEditable() As Boolean
        Get
            If DotNetNuke.Security.PortalSecurity.IsInRole(_parent.PortalSettings.AdministratorRoleName) = True Then
                Return True
            Else
                Dim tbInfo As DotNetNuke.Entities.Tabs.TabInfo = Nothing
                If Not _parent.PortalSettings.ActiveTab Is Nothing Then
                    tbInfo = _parent.PortalSettings.ActiveTab
                Else
                    If Not PageId Is Nothing AndAlso IsNumeric(PageId) Then
                        tbInfo = (New DotNetNuke.Entities.Tabs.TabController).GetTab(CInt(PageId))
                    End If
                End If

                If Not tbInfo Is Nothing AndAlso DotNetNuke.Security.PortalSecurity.IsInRoles(tbInfo.TabPermissions.ToString("EDIT")) Then
                    Return True
                Else
                    If Not ModuleId Is Nothing AndAlso IsNumeric(ModuleId) Then
                        Dim mInfo As DotNetNuke.Entities.Modules.ModuleInfo
                        mInfo = (New DotNetNuke.Entities.Modules.ModuleController).GetModule(CInt(Me.ModuleId), CInt(Me.PageId))
                        If Not mInfo Is Nothing AndAlso DotNetNuke.Security.PortalSecurity.IsInRoles(mInfo.ModulePermissions.ToString("EDIT")) Then 'mInfo.AuthorizedEditRoles
                            Return True
                        End If
                    End If
                End If
            End If
            Return False
        End Get
    End Property
    Public Overrides ReadOnly Property isViewable() As Boolean
        Get
            If DotNetNuke.Security.PortalSecurity.IsInRole(_parent.PortalSettings.AdministratorRoleName) = True Then
                Return True
            Else
                Dim tbInfo As DotNetNuke.Entities.Tabs.TabInfo = Nothing
                If Not _parent.PortalSettings.ActiveTab Is Nothing Then
                    tbInfo = _parent.PortalSettings.ActiveTab
                Else
                    If Not PageId Is Nothing AndAlso IsNumeric(PageId) Then
                        tbInfo = (New DotNetNuke.Entities.Tabs.TabController).GetTab(CInt(PageId))
                    End If
                End If
                If Not tbInfo Is Nothing AndAlso DotNetNuke.Security.PortalSecurity.IsInRoles(tbInfo.TabPermissions.ToString("EDIT")) Then
                    Return True
                Else
                    If Not ModuleId Is Nothing AndAlso IsNumeric(ModuleId) Then
                        Dim mInfo As DotNetNuke.Entities.Modules.ModuleInfo
                        mInfo = (New DotNetNuke.Entities.Modules.ModuleController).GetModule(CInt(Me.ModuleId), CInt(Me.PageId))
                        If Not mInfo Is Nothing AndAlso DotNetNuke.Security.PortalSecurity.IsInRoles(mInfo.ModulePermissions.ToString("EDIT")) Then
                            Return True
                        End If
                    End If
                End If
            End If
            Return False
        End Get
    End Property

    Public Overrides ReadOnly Property ListSource() As String
        Get
            Dim src As New r2i.OWS.Wrapper.DNN.DataAccess.AjaxMsgParams
            src.moduleId = Me.ModuleId
            src.pageId = Me.PageId
            src.configurationId = Me.ConfigurationId
            src.Source = Me.ClientID
            src.ResourceKey = Me.ResourceKey
            src.PortalID = Me.SiteId
            Dim strValue As String = src.toString
            'If Not QueryVariables Is Nothing AndAlso QueryVariables.Length > 0 Then
            '    If strValue.Length > 0 Then strValue &= "&"
            '    strValue &= QueryVariables
            'End If
            Return strValue
        End Get
    End Property

    Public Overrides Function MapPath(ByVal value As String) As String
        Return _parent.MapPath(value)
    End Function

    Public Overrides Property ModuleConfiguration() As Framework.DataAccess.IModuleInfo
        Get
            If _moduleInfo Is Nothing AndAlso Not _moduleID Is Nothing Then
                _moduleInfo = New r2i.OWS.Wrapper.DNN.DataAccess.ModuleInfo(ModuleId, PageId)
            End If
            Return _moduleInfo
        End Get
        Set(ByVal value As Framework.DataAccess.IModuleInfo)
            _moduleInfo = value
        End Set
    End Property

    Public Overrides Property PageModuleId() As String
        Get
            If _pagemoduleID Is Nothing AndAlso Not _moduleInfo Is Nothing Then
                Return ModuleConfiguration.TabModuleID
            Else
                Return _pagemoduleID
            End If
        End Get
        Set(ByVal value As String)
            _pagemoduleID = value
        End Set
    End Property

    Public Overrides Property ModuleId() As String
        Get
            If _moduleID Is Nothing AndAlso Not _moduleInfo Is Nothing Then
                Return ModuleConfiguration.ModuleID
            Else
                Return _moduleID
            End If
        End Get
        Set(ByVal value As String)
            _moduleID = value
            _moduleInfo = Nothing
        End Set
    End Property

    Public Overrides ReadOnly Property ModulePath() As String
        Get
            Return Me.TemplateSourceDirectory & "/"
        End Get
    End Property

    Public Overrides ReadOnly Property NoAjax() As Boolean
        Get
            'TODO: ignore ajax?
            Return False
        End Get
    End Property

    Public Overrides Property PageId() As String
        Get
            If _pageID Is Nothing AndAlso Not _parent.PortalSettings.ActiveTab Is Nothing Then
                Return _parent.PortalSettings.ActiveTab.TabID.ToString()
            Else
                Return _pageID
            End If
        End Get
        Set(ByVal value As String)
            _pageID = value
        End Set
    End Property

    Public Overrides Property PageNumber() As Integer
        Get
            If ViewState(Me.UniqueID & "PAGE") Is Nothing Then
                Return 0
            Else
                Return CInt(ViewState(Me.UniqueID & "PAGE"))
            End If
        End Get
        Set(ByVal Value As Integer)
            ViewState(Me.UniqueID & "PAGE") = Value
        End Set
    End Property

    Public Overrides Property RecordsPerPage() As String
        Get
            If ViewState(Me.UniqueID & "PERPAGE") Is Nothing Then
                Return Nothing
            Else
                Return ViewState(Me.UniqueID & "PERPAGE").ToString
            End If
        End Get
        Set(ByVal Value As String)
            ViewState(Me.UniqueID & "PERPAGE") = Value
        End Set
    End Property

    Public Overrides Property ResourceKey() As String
        Get
            Return _resourceKey
        End Get
        Set(ByVal Value As String)
            _resourceKey = Value
        End Set
    End Property

    Public Overrides Property ResourceFile() As String
        Get
            Return _resourceFile
        End Get
        Set(ByVal Value As String)
            _resourceFile = Value
        End Set
    End Property
    Public Overrides Property UsePageDefaults() As Boolean
        Get
            Return _usepagedefaults
        End Get
        Set(ByVal value As Boolean)
            _usepagedefaults = value
        End Set
    End Property
    Public Overrides ReadOnly Property Settings() As System.Collections.Hashtable
        Get
            If Not ModuleId Is Nothing AndAlso IsNumeric(ModuleId) Then
                Return (New DotNetNuke.Entities.Modules.ModuleController).GetModuleSettings(CInt(ModuleId))
            End If
            Return New Hashtable
        End Get
    End Property

    Private _portalID As String = Nothing
    Public Overrides Property SiteId() As String
        Get
            If _portalID Is Nothing Then
                _portalID = _parent.PortalSettings.PortalId.ToString
                CheckPortalSettings(_portalID)
                _portalID = _portalSettings.PortalId
            End If
            Return _portalID
        End Get
        Set(ByVal Value As String)
            _portalID = Value
            CheckPortalSettings(Value)
        End Set
    End Property
    Private Sub CheckPortalSettings(ByVal PortalID As String)
        If _portalSettings Is Nothing OrElse _portalSettings.PortalId <> PortalID Then
            If Not PortalID Is Nothing AndAlso IsNumeric(PortalID) Then
                _portalSettings = Wrapper.DNN.Entities.PortalSettingsController.GetPortalSettings(PortalID)
            End If
        End If
    End Sub
    Public Overrides ReadOnly Property SiteInfo() As Framework.DataAccess.IPortalSettings
        Get
            If _portalSettings Is Nothing Then
                _portalSettings = New r2i.OWS.Wrapper.DNN.DataAccess.PortalSettings(_parent.PortalSettings)
            End If
            If Not _portalID Is Nothing AndAlso IsNumeric(_portalID) Then
                CheckPortalSettings(_portalID)
            End If
            Return _portalSettings
        End Get
    End Property

    Public Overrides ReadOnly Property UserId() As String
        Get
            If Not UserInfo Is Nothing Then
                Return UserInfo.Id
            Else
                Return "-1"
            End If
        End Get
    End Property

    Public Overrides ReadOnly Property UserInfo() As Framework.DataAccess.IUser
        Get
            If _userInfo Is Nothing Then
                _userInfo = New r2i.OWS.Wrapper.DNN.DataAccess.User(DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo)
            End If
            Return _userInfo
        End Get
    End Property

    Public Overrides ReadOnly Property ClientID() As String
        Get
            'If Not Me.Page.Request.QueryString.Item("lxSrc") Is Nothing AndAlso Me.Page.Request.QueryString.Item("lxSrc").Length > 0 Then
            '    Return Me.Page.Request.QueryString.Item("lxSrc")
            'Else
            '    Return MyBase.ClientID
            'End If

            If IncomingParameters.ContainsKey(qSource) AndAlso IncomingParameters(qSource).Length > 0 Then
                Return IncomingParameters(qSource)
            Else
                Return MyBase.ClientID
            End If
        End Get
    End Property

    Private Sub OpenControl_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Wrapper.DNN.Entities.WrapperFactory.Create()
        _parent = CType(Me.Parent, DotNetNuke.Framework.CDefault)
    End Sub

    Private _scriptblocks As Dictionary(Of String, String)
    Public Overrides Sub RegisterScriptBlock(ByVal Key As String, ByVal Value As String)
        If Me.ModuleConfiguration.CacheTime > 0 Then
            If _scriptblocks Is Nothing Then
                _scriptblocks = New Dictionary(Of String, String)
            End If
            If Not _scriptblocks.ContainsKey(Key) Then
                _scriptblocks.Add(Key, Value)
            End If
        Else
            If (Not Parent.Page.ClientScript.IsClientScriptBlockRegistered(Key)) Then
                Page.ClientScript.RegisterClientScriptBlock(Me.GetType, Key, Value)
            End If
        End If
    End Sub

    Public Overrides ReadOnly Property isCallback() As Boolean
        Get
            Return True
        End Get
    End Property

    'ADDED IN 2.1.7.1 - this may have compatibility issues outside of the constraints of this change whenever threading is enabled.
    Private Sub OpenCallbackControl_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Unload
        'If Not _messages Is Nothing Then
        '    _messages = Nothing
        'End If
        '_parent = Nothing
        '_moduleInfo = Nothing
        '_portalSettings = Nothing
        '_pagemoduleID = Nothing
        '_resourceFile = Nothing
        '_resourceKey = Nothing
        '_userInfo = Nothing
        'MyBase.Dispose()
        'Me.Dispose()
    End Sub
End Class
