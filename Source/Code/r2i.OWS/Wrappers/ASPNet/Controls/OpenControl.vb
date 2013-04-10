''<LICENSE>
''   Open Web Studio - http://www.OpenWebStudio.com
''   Copyright (c) 2007-2008
''   by R2Integrated Inc. http://www.R2integrated.com
''      
''   Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
''   documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
''   the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
''   to permit persons to whom the Software is furnished to do so, subject to the following conditions:
''    
''   The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
''   the Software.
''   
''   This Software and associated documentation files are subject to the terms and conditions of the Open Web Studio 
''   End User License Agreement and version 2 of the GNU General Public License.
''    
''   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
''   TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
''   THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
''   CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
''   DEALINGS IN THE SOFTWARE.
''</LICENSE>
'Imports System
'Imports System.Web
'Imports System.Web.UI
'Imports System.Security

'Imports System.ComponentModel
'Imports r2i.OWS.Framework.Utilities

'<ParseChildren(True), PersistChildren(False), ToolboxData("<{0}:MenuItemCollectionControl runat=server></{0}:MenuItemCollectionControl>")> _
'Public Class OpenControl
'    Inherits r2i.OWS.UI.OpenControlBase

'    Private pItems As New MenuItemCollection
'    <NotifyParentProperty(True), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)> _
'    Public ReadOnly Property MenuItems() As MenuItemCollection
'        Get
'            Return pItems
'        End Get
'    End Property

'    Private _currentmoduleInfo As IModuleInfo
'    Private _portalSettings As IPortalSettings
'    Private _parent As BaseParentControl
'    Private _messages As SortedList(Of String, String)
'    Private _userInfo As IUser
'    Private _moduleID As String
'    Private _pagemoduleID As String
'    Private _oModulePath As String
'    Private _pageID As String
'    Private _configurationID As Guid
'    Private _resourceKey As String
'    Private _usepagedefaults As Boolean = False
'    Private _resourceFile As String
'    Private _configLoaded As Boolean
'    Private _configLocked As Boolean = False
'    Private _basePath As String = ""
'    'Private Shared _basePath As String = ""
'    Private _ModuleSettingsListener As String = ""

'    Public Sub SetParentBase(ByVal base As BaseParentControl)
'        _parent = base
'    End Sub

'    Public Overrides Property Cache() As System.Web.Caching.Cache
'        Get
'            If _parent Is Nothing OrElse _parent.Cache Is Nothing Then
'                Return Nothing
'            Else
'                Return _parent.Cache
'            End If
'        End Get
'        Set(ByVal value As System.Web.Caching.Cache)
'            'CANNOT SET - READONLY
'        End Set
'    End Property


'    Public Overrides Sub ClearCache()
'        Try
'            _parent.ClearCache()
'        Catch ex As Exception
'        End Try
'    End Sub
'    Public Overrides Sub ClearSiteCache()
'        Try
'            _parent.ClearSiteCache()
'        Catch ex As Exception
'        End Try
'    End Sub
'    Public Overrides Sub ClearPageCache()
'        Try
'            _parent.ClearPageCache()
'        Catch ex As Exception
'        End Try
'    End Sub

'    Public Overrides ReadOnly Property CapturedMessages() As System.Collections.Generic.SortedList(Of String, String)
'        Get
'            If _messages Is Nothing Then
'                _messages = New SortedList(Of String, String)
'            End If
'            Return _messages
'        End Get
'    End Property
'    Private _overrideUniqueID As String
'    Public Sub SetUniqueID(ByVal value As String)
'        _overrideUniqueID = value
'    End Sub
'    Public Overrides ReadOnly Property ClientID() As String
'        Get
'            If Not _overrideUniqueID Is Nothing Then
'                Return _overrideUniqueID
'            Else
'                Return _parent.UniqueId
'            End If
'        End Get
'    End Property
'    Public Overrides Property UsePageDefaults() As Boolean
'        Get
'            Return _usepagedefaults
'        End Get
'        Set(ByVal value As Boolean)
'            _usepagedefaults = value
'        End Set
'    End Property
'    Public Sub SetOWSPath(ByVal Path As String)
'        _basePath = Path
'    End Sub
'    Public Sub SetModulePath(ByVal Path As String)
'        _oModulePath = Path
'    End Sub
'    Public Overrides ReadOnly Property BasePath() As String
'        Get
'            Return _basePath
'        End Get
'    End Property

'    Public Overrides Property ConfigurationId() As System.Guid
'        Get
'            If Not _configLoaded Then
'                If _parent.Settings.ContainsKey("ConfigurationID") Then
'                    _configurationID = New Guid(_parent.Settings.Item("ConfigurationID").ToString)
'                    _configLoaded = True
'                End If
'            End If
'            Return _configurationID
'        End Get
'        Set(ByVal value As System.Guid)
'            _configurationID = value
'            _configLoaded = True
'        End Set
'    End Property


'    Public Overrides Property ConfigurationLocked() As Boolean
'        Get
'            Return _configLocked
'        End Get
'        Set(ByVal value As Boolean)
'            _configLocked = value
'        End Set
'    End Property

'    Public Overrides Property FilterField() As String
'        Get
'            If Not _parent.Session.Item(Me.ConfigurationId.ToString & Me.UserId & "." & Me.ModuleId & "FILTERFIELD") Is Nothing Then
'                Return _parent.Session.Item(Me.ConfigurationId.ToString & Me.UserId & "." & Me.ModuleId & "FILTERFIELD").ToString
'            Else
'                Return ""
'            End If
'        End Get
'        Set(ByVal value As String)
'            _parent.Session.Item(Me.ConfigurationId.ToString & Me.UserId & "." & Me.ModuleId & "FILTERFIELD") = value
'        End Set
'    End Property

'    Public Overrides Property FilterText() As String
'        Get
'            If Not _parent.Session.Item(Me.ConfigurationId.ToString & Me.UserId & "." & Me.ModuleId & "FILTERTEXT") Is Nothing Then
'                Return _parent.Session.Item(Me.ConfigurationId.ToString & Me.UserId & "." & Me.ModuleId & "FILTERTEXT").ToString
'            Else
'                Return ""
'            End If
'        End Get
'        Set(ByVal value As String)
'            _parent.Session.Item(Me.ConfigurationId.ToString & Me.UserId & "." & Me.ModuleId & "FILTERTEXT") = value
'        End Set
'    End Property

'    Overrides ReadOnly Property Request() As HttpRequest
'        Get
'            If Not _parent Is Nothing Then
'                Return _parent.Request
'            End If
'            Return Nothing
'        End Get
'    End Property
'    Overrides ReadOnly Property Session() As GenericSession
'        Get
'            If Not _parent Is Nothing Then
'                Return _parent.Session
'            End If
'            Return Nothing
'        End Get
'    End Property


'    Public Overrides ReadOnly Property Footer() As System.Web.UI.Control
'        Get
'            If Not _loadedHeaderFooter Then
'                GetHeaderFooter()
'            End If
'            Return _footer
'        End Get
'    End Property
'    Public Overrides ReadOnly Property Title() As System.Web.UI.Control
'        Get
'            If Not _loadedTitle Then
'                GetTitle()
'            End If
'            Return _title
'        End Get
'    End Property
'    Public Overrides ReadOnly Property Header() As System.Web.UI.Control
'        Get
'            If Not _loadedHeaderFooter Then
'                GetHeaderFooter()
'            End If
'            Return _header
'        End Get
'    End Property

'    Public Sub SetHeaderFooter(ByRef Header As System.Web.UI.Control, ByRef Footer As System.Web.UI.Control)
'        _loadedHeaderFooter = True
'        _header = Header
'        _footer = Footer
'    End Sub

'    Public Overrides ReadOnly Property IsEditable() As Boolean
'        Get
'            Return _parent.IsEditable
'        End Get
'    End Property
'    Public Overrides ReadOnly Property isViewable() As Boolean
'        Get
'            Return True
'        End Get
'    End Property

'    Public Overrides ReadOnly Property ListSource() As String
'        Get
'            Dim src As New r2i.OWS.Wrapper.ASPNET.DataAccess.AjaxMsgParams
'            src.moduleId = Me.ModuleId
'            src.pageId = Me.PageId
'            src.pageModuleId = Me.PageModuleId
'            src.configurationId = Me.ConfigurationId
'            src.Source = Me.ClientID
'            src.siteId = Me.SiteId
'            If Not Me.ResourceKey Is Nothing AndAlso Not Me.ResourceKey.Length = 0 Then
'                src.ResourceKey = Me.ResourceKey
'                If Not Me.ResourceFile Is Nothing AndAlso Me.ResourceFile.Length > 0 Then
'                    src.ResourceFile = Me.ResourceFile
'                    If Not Me.ResourceFile.EndsWith(".ascx.resx") Then
'                        src.ResourceFile &= ".ascx.resx"
'                    End If
'                End If
'            End If
'            Dim strValue As String = src.toString
'            'If Not QueryVariables Is Nothing AndAlso QueryVariables.Length > 0 Then
'            '    If strValue.Length > 0 Then strValue &= "&"
'            '    strValue &= QueryVariables
'            'End If
'            Return strValue
'        End Get
'    End Property

'    Public Overrides Function MapPath(ByVal value As String) As String
'        Return _parent.MapPath(value)
'    End Function

'    Public Overrides Property ModuleConfiguration() As Framework.DataAccess.IModuleInfo
'        Get

'            Return _currentmoduleInfo
'        End Get
'        Set(ByVal value As Framework.DataAccess.IModuleInfo)
'            _currentmoduleInfo = value
'        End Set
'    End Property

'    Public Overrides Property ModuleId() As String
'        Get
'            If _moduleID Is Nothing AndAlso Not _currentmoduleInfo Is Nothing Then
'                _moduleID = _currentmoduleInfo.ModuleID
'                Return _moduleID
'            Else
'                Return _moduleID
'            End If
'        End Get
'        Set(ByVal value As String)
'            _moduleID = value
'        End Set
'    End Property

'    Public Overrides Property PageModuleId() As String
'        Get
'            If _pagemoduleID Is Nothing AndAlso Not _parent Is Nothing Then
'                _pagemoduleID = CType(_parent.TabModuleId, String)
'                Return _pagemoduleID
'            Else
'                Return _pagemoduleID
'            End If
'        End Get
'        Set(ByVal value As String)
'            _pagemoduleID = value
'        End Set
'    End Property

'    Public Overrides ReadOnly Property ModulePath() As String
'        Get
'            If Not _oModulePath Is Nothing AndAlso _oModulePath.Length > 0 Then
'                Return _oModulePath
'            Else
'                If Not _parent Is Nothing Then
'                    Return _parent.ModulePath
'                Else
'                    Return ""
'                End If
'            End If
'        End Get
'    End Property

'    Public Overrides ReadOnly Property NoAjax() As Boolean
'        Get
'            'TODO: ignore ajax?
'            Return False
'        End Get
'    End Property

'    Public Overrides Property PageId() As String
'        Get
'            If _pageID Is Nothing Then
'                Return _parent.TabId.ToString()
'            Else
'                Return _pageID
'            End If
'        End Get
'        Set(ByVal value As String)
'            _pageID = value
'        End Set
'    End Property

'    Public Overrides Property PageNumber() As Integer
'        Get
'            If ViewState(Me.UniqueID & "PAGE") Is Nothing Then
'                Return 0
'            Else
'                Return CInt(ViewState(Me.UniqueID & "PAGE"))
'            End If
'        End Get
'        Set(ByVal Value As Integer)
'            ViewState(Me.UniqueID & "PAGE") = Value
'        End Set
'    End Property



'    Public Overrides Property RecordsPerPage() As String
'        Get
'            If ViewState(Me.UniqueID & "PERPAGE") Is Nothing Then
'                Return Nothing
'            Else
'                Return ViewState(Me.UniqueID & "PERPAGE").ToString
'            End If
'        End Get
'        Set(ByVal Value As String)
'            ViewState(Me.UniqueID & "PERPAGE") = Value
'        End Set
'    End Property

'    Public Overrides Property ResourceKey() As String
'        Get
'            Return _resourceKey
'        End Get
'        Set(ByVal Value As String)
'            _resourceKey = Value
'        End Set
'    End Property
'    Public Overrides Property ResourceFile() As String
'        Get
'            Dim rslt As String = ""
'            If Not _resourceKey Is Nothing AndAlso Not _resourceKey.Length = 0 Then
'                If Not _resourceFile Is Nothing Then
'                    If _resourceFile.Length = 0 Then
'                        rslt = _parent.LocalResourceFile
'                    ElseIf _resourceFile.StartsWith("*") Then
'                        rslt = Utility.Join("/", Me.ModulePath, _resourceFile)
'                    Else
'                        rslt = _resourceFile
'                    End If
'                Else
'                    If _parent Is Nothing Then
'                        rslt = DotNetNuke.Services.Localization.Localization.GetResourceFile(Me, System.IO.Path.GetFileName(Parent.TemplateControl.AppRelativeVirtualPath) & ".resx")
'                    Else
'                        rslt = _parent.LocalResourceFile
'                    End If
'                End If
'                _resourceFile = rslt
'            End If
'            If _parent Is Nothing Then
'                _parent = New BaseParentControl(_resourceFile, Me.ResourceKey, Me.TemplateSourceDirectory, Me.ModuleId)
'                _parent.Parent = Me.Parent
'            End If
'            Return _resourceFile
'        End Get
'        Set(ByVal Value As String)
'            _resourceFile = Value
'        End Set
'    End Property

'    Public Overrides ReadOnly Property Settings() As System.Collections.Hashtable
'        Get
'            Return _parent.Settings
'        End Get
'    End Property

'    Private _portalID As String = Nothing
'    Public Overrides Property SiteId() As String
'        Get
'            If _portalID Is Nothing Then
'                _portalID = _parent.PortalSettings.PortalId.ToString
'            End If
'            Return _portalID
'        End Get
'        Set(ByVal Value As String)
'            _portalID = Value
'            CheckPortalSettings(Value)
'        End Set
'    End Property
'    Private Sub CheckPortalSettings(ByVal PortalID As String)
'        If _portalSettings Is Nothing OrElse _portalSettings.PortalId <> PortalID Then
'            If Not PortalID Is Nothing AndAlso IsNumeric(PortalID) Then
'                _portalSettings = Wrapper.DNN.Entities.PortalSettingsController.GetPortalSettings(PortalID)
'            End If
'        End If
'    End Sub
'    Public Overrides ReadOnly Property SiteInfo() As Framework.DataAccess.IPortalSettings
'        Get
'            If _portalSettings Is Nothing Then
'                _portalSettings = New r2i.OWS.Wrapper.DNN.DataAccess.PortalSettings(_parent.PortalSettings)
'            End If
'            If Not _portalID Is Nothing AndAlso IsNumeric(_portalID) Then
'                CheckPortalSettings(_portalID)
'            End If
'            Return _portalSettings
'        End Get
'    End Property

'    Public Overrides ReadOnly Property UserId() As String
'        Get
'            Return _parent.UserId.ToString
'        End Get
'    End Property

'    Public Overrides ReadOnly Property UserInfo() As Framework.DataAccess.IUser
'        Get
'            Return _userInfo
'        End Get
'    End Property

'    Public Property SystemMessage_UpdateSettings() As String
'        Get
'            Return _ModuleSettingsListener
'        End Get
'        Set(ByVal value As String)
'            _ModuleSettingsListener = value
'        End Set
'    End Property

'    Private _loadedHeaderFooter As Boolean = False
'    Private _loadedTitle As Boolean = False
'    Private _title As System.Web.UI.Control
'    Private _header As System.Web.UI.Control
'    Private _footer As System.Web.UI.Control
'    Private Sub GetHeaderFooter()
'        _loadedHeaderFooter = True
'        Dim startParent As Control = Nothing
'        If Not Me.Parent Is Nothing Then
'            startParent = Me.Parent '_parent
'        End If
'        If Not startParent Is Nothing AndAlso Not startParent.Parent Is Nothing Then
'            'THE PARENT OF A DNN MODULE CONTAINER IS A TABLE CELL, BUT LETS JUST IGNORE THE TYPES
'            'IF WE ENCOUNTER A LABEL OR A LITERAL - WE WILL RENDER THEIR CONTENT
'            Dim showHeader As Boolean = False
'            Dim showFooter As Boolean = False
'            If Not _parent.ModuleConfiguration.Header Is Nothing AndAlso _parent.ModuleConfiguration.Header.Length > 0 Then
'                showHeader = True
'            End If
'            If Not _parent.ModuleConfiguration.Footer Is Nothing AndAlso _parent.ModuleConfiguration.Footer.Length > 0 Then
'                showFooter = True
'            End If

'            If showHeader Or showFooter Then
'                If Not startParent.Parent.Parent.Controls Is Nothing AndAlso startParent.Parent.Parent.Controls.Count > 0 Then
'                    Dim i As Integer
'                    For i = 0 To startParent.Parent.Parent.Controls.Count - 1
'                        If TypeOf startParent.Parent.Parent.Controls(i) Is System.Web.UI.WebControls.Label Then
'                            Dim lbl As System.Web.UI.WebControls.Label = CType(startParent.Parent.Parent.Controls(i), System.Web.UI.WebControls.Label)
'                            If showHeader Then
'                                _header = lbl
'                                showHeader = False
'                            Else
'                                _footer = lbl
'                            End If
'                        ElseIf TypeOf startParent.Parent.Parent.Controls(i) Is System.Web.UI.WebControls.Literal Then
'                            Dim ltl As System.Web.UI.WebControls.Literal = CType(startParent.Parent.Parent.Controls(i), System.Web.UI.WebControls.Literal)
'                            Dim thisstr As String = ltl.Text
'                            If showHeader Then
'                                _header = ltl
'                                showHeader = False
'                            Else
'                                _footer = ltl
'                            End If
'                        End If
'                    Next
'                End If
'            End If
'        End If
'    End Sub
'    Public Sub GetTitle()
'        _loadedTitle = True
'        Try
'            'ROMAIN: 09/19/07
'            'If TypeOf Caller.Engine.Caller Is PortalModuleBase Then
'            'Dim mpmb As PortalModuleBase = CType(Caller.Engine.Caller, PortalModuleBase)
'            'Dim mpmb As IPortalModuleBaseUI = CType(Caller.Engine.Caller, IPortalModuleBaseUI)
'            Dim startParent As Control = Nothing
'            If Not Me.Parent Is Nothing Then
'                startParent = Me.Parent '_parent
'            End If
'            If Not startParent Is Nothing AndAlso Not startParent.Parent Is Nothing Then
'                Dim title As System.Web.UI.Control
'                Dim titleLabel As System.Web.UI.Control
'                Dim replaced As Boolean = False
'                Dim ttlObject As Object = Nothing
'                Dim ttlLabelObject As Object = Nothing

'                'If Not startParent.Parent.Parent.Parent.Controls Is Nothing AndAlso startParent.Parent.Parent.Parent.Controls.Count > 0 Then
'                '    Dim i As Integer
'                '    For i = 0 To startParent.Parent.Parent.Parent.Controls.Count - 1
'                '        If startParent.Parent.Parent.Parent.Controls(i).ID.ToUpper.EndsWith("TITLE") Then
'                '            title = startParent.Parent.Parent.Parent.Controls(i)
'                '            If Not title.GetType().GetProperty("Text") Is Nothing Then
'                '                ttlObject = title
'                '            End If
'                '            For Each titleLabel In title.Controls
'                '                If Not titleLabel Is Nothing AndAlso Not titleLabel.ID Is Nothing AndAlso titleLabel.ID.ToUpper.EndsWith("TITLE") Then
'                '                    ttlLabelObject = titleLabel
'                '                    Exit For
'                '                End If
'                '            Next
'                '            Exit For
'                '        End If
'                '    Next
'                'End If

'                ttlLabelObject = startParent.Page.FindControl(startParent.ClientID.Replace("_", "$") & "TITLE$lblTitle")

'                If Not ttlLabelObject Is Nothing Then
'                    _title = CType(ttlLabelObject, System.Web.UI.Control)
'                ElseIf Not ttlObject Is Nothing Then
'                    _title = CType(ttlObject, System.Web.UI.Control)
'                End If
'            End If
'        Catch ex As Exception
'        End Try
'    End Sub

'    Private Sub OpenControl_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
'        Wrapper.DNN.Entities.WrapperFactory.Create()

'        If TypeOf Me.Parent Is PortalModuleBase Then
'            _parent = New BaseParentControl(CType(Me.Parent, DotNetNuke.Entities.Modules.PortalModuleBase))
'            _currentmoduleInfo = New r2i.OWS.Wrapper.DNN.DataAccess.ModuleInfo(_parent.ModuleConfiguration)
'        ElseIf TypeOf Me.Parent Is ModuleSettingsBase Then
'            _parent = New BaseParentControl(CType(Me.Parent, DotNetNuke.Entities.Modules.ModuleSettingsBase))
'            _currentmoduleInfo = New r2i.OWS.Wrapper.DNN.DataAccess.ModuleInfo(CStr(_parent.ModuleId), CStr(_parent.TabId))
'        ElseIf TypeOf Me.Parent Is DotNetNuke.UI.Skins.SkinObjectBase Then
'            If Not ModuleId Is Nothing AndAlso IsNumeric(ModuleId) Then
'                _parent = New BaseParentControl(CType(Me.Parent, DotNetNuke.UI.Skins.SkinObjectBase), ResourceFile, ResourceKey, ModulePath, ModuleId)
'            Else
'                _parent = New BaseParentControl(CType(Me.Parent, DotNetNuke.UI.Skins.SkinObjectBase), ResourceFile, ResourceKey, ModulePath, "-1")
'            End If
'            _currentmoduleInfo = New r2i.OWS.Wrapper.DNN.DataAccess.ModuleInfo(CStr(_parent.ModuleId), CStr(_parent.TabId))
'        ElseIf TypeOf Me.Parent Is Page Then
'            Dim base As DotNetNuke.UI.Skins.SkinObjectBase = New DotNetNuke.UI.Skins.SkinObjectBase()
'            base.Page = DirectCast(Me.Parent, System.Web.UI.Page)
'            _parent = New BaseParentControl(base, ResourceFile, ResourceKey, ModulePath, "-1")
'        End If
'        _portalSettings = New r2i.OWS.Wrapper.DNN.DataAccess.PortalSettings(_parent.PortalSettings)
'        If Not _parent.UserInfo Is Nothing Then
'            _userInfo = New r2i.OWS.Wrapper.DNN.DataAccess.User(_parent.UserInfo)
'        Else
'            'UNAUTHENTICATED USER
'            _userInfo = New r2i.OWS.Wrapper.DNN.DataAccess.User(New DotNetNuke.Entities.Users.UserInfo())
'        End If

'    End Sub

'    Private _scriptblocks As Dictionary(Of String, String)
'    Public Overrides Sub RegisterScriptBlock(ByVal Key As String, ByVal Value As String)
'        If Not Me.ModuleConfiguration Is Nothing AndAlso Me.ModuleConfiguration.CacheTime > 0 Then
'            If _scriptblocks Is Nothing Then
'                _scriptblocks = New Dictionary(Of String, String)
'            End If
'            If Not _scriptblocks.ContainsKey(Key) Then
'                _scriptblocks.Add(Key, Value)
'            End If
'        Else
'            If (Not Parent.Page.ClientScript.IsClientScriptBlockRegistered(Key)) Then
'                Page.ClientScript.RegisterClientScriptBlock(Me.GetType, Key, Value)
'            End If
'        End If
'    End Sub
'    Public Overrides Function CachedScriptBlocks() As String
'        If Not _scriptblocks Is Nothing Then
'            Dim sb As New Text.StringBuilder()
'            Dim str As String
'            For Each str In _scriptblocks.Values
'                sb.Append(str)
'            Next
'            _scriptblocks = Nothing
'            Return sb.ToString
'        End If
'        Return Nothing
'    End Function

'    Public Overrides ReadOnly Property isCallback() As Boolean
'        Get
'            Return False
'        End Get
'    End Property

'    Private Sub OpenControl_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
'        'MyBase.OnLoad(e)
'    End Sub

'    Private Sub OpenControl_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
'        'MyBase.OnPreRender(e)
'    End Sub


'    'ADDED IN 2.1.7.1 - this may have compatibility issues outside of the constraints of this change whenever threading is enabled.
'    Private Sub OpenCallbackControl_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Unload
'        'If Not _messages Is Nothing Then
'        '    _messages = Nothing
'        'End If
'        '_parent = Nothing
'        '_portalSettings = Nothing
'        '_pagemoduleID = Nothing
'        '_resourceFile = Nothing
'        '_resourceKey = Nothing
'        '_userInfo = Nothing
'        'MyBase.Dispose()
'        'Me.Dispose()
'    End Sub
'End Class
