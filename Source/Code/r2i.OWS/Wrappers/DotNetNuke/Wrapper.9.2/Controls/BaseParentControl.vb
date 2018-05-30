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

Public Class BaseParentControl
    Inherits r2i.OWS.Framework.UI.Control

    Private _Parent As Object
    Private _LocalResourceFile As String
    Private _LocalResourceKey As String
    Private _Path As String
    Private _ModuleID As String

    Public Sub New(ByVal ModSettings As ModuleSettingsBase)
        _Parent = ModSettings
    End Sub
    Public Sub New(ByVal SkinObject As DotNetNuke.UI.Skins.SkinObjectBase, ByVal ResourceFile As String, ByVal ResourceKey As String, ByVal Path As String, ByVal ModuleID As String)
        _Parent = SkinObject
        _LocalResourceFile = ResourceFile
        _LocalResourceKey = ResourceKey
        _ModuleID = ModuleID
        _Path = Path
    End Sub
    Public Sub New(ByVal schedulerObject As DotNetNuke.Services.Scheduling.SchedulerClient, ByVal ConfigurationID As String, ByVal Path As String, ByVal ModuleID As String)
        _Parent = schedulerObject
        _LocalResourceFile = ""
        _LocalResourceKey = ""
        _ModuleID = ModuleID
        _Path = Path
    End Sub
    Public Sub New(ByVal ResourceFile As String, ByVal ResourceKey As String, ByVal Path As String, ByVal ModuleID As String)
        _LocalResourceFile = ResourceFile
        _LocalResourceKey = ResourceKey
        _ModuleID = ModuleID
        _Path = Path
    End Sub
    Public Sub New(ByVal PortalMod As PortalModuleBase)
        _Parent = PortalMod
    End Sub
    Public Property Parent() As Object
        Get
            Return _Parent
        End Get
        Set(ByVal value As Object)
            _Parent = value
        End Set
    End Property

    Public Function GetNextActionID() As Integer
        If Not _Parent Is Nothing Then
            If TypeOf _Parent Is ModuleSettingsBase Then
                Return CType(_Parent, ModuleSettingsBase).GetNextActionID
            ElseIf TypeOf _Parent Is PortalModuleBase Then
                Return CType(_Parent, PortalModuleBase).GetNextActionID
            End If
        End If
        Return 0
    End Function
    Public Function MapPath(ByVal Path As String) As String
        If Not _Parent Is Nothing Then
            If TypeOf _Parent Is ModuleSettingsBase Then
                Return CType(_Parent, ModuleSettingsBase).MapPath(Path)
            ElseIf TypeOf _Parent Is PortalModuleBase Then
                Try
                    Return CType(_Parent, PortalModuleBase).MapPath(Path)
                Catch
                    'Control throws an exception when MapPath is called without a fully qualified loader context.
                    Return System.Web.HttpContext.Current.Server.MapPath(Path)
                End Try
            ElseIf TypeOf _Parent Is DotNetNuke.UI.Skins.SkinObjectBase Then
                Return CType(_Parent, DotNetNuke.UI.Skins.SkinObjectBase).MapPath(Path)
            ElseIf TypeOf _Parent Is DotNetNuke.Services.Scheduling.SchedulerClient Then
                Return System.Web.HttpContext.Current.Server.MapPath(Path)
            ElseIf TypeOf _Parent Is System.Web.UI.Page Then
                Return System.Web.HttpContext.Current.Server.MapPath(Path)
            End If
        End If
        Return ""
    End Function
    Public ReadOnly Property ModuleConfiguration() As ModuleInfo
        Get
            If Not _Parent Is Nothing Then
                LoadModuleConfiguration()
                If TypeOf _Parent Is ModuleSettingsBase Then
                    Return _moduleConfiguration
                ElseIf TypeOf _Parent Is PortalModuleBase Then
                    Return _moduleConfiguration
                ElseIf TypeOf _Parent Is DotNetNuke.UI.Skins.SkinObjectBase Then
                    Return _moduleConfiguration
                ElseIf TypeOf _Parent Is DotNetNuke.Services.Scheduling.SchedulerClient Then
                    Return _moduleConfiguration
                ElseIf TypeOf _Parent Is System.Web.UI.Page Then
                    Return _moduleConfiguration
                End If
            End If
            Return Nothing
        End Get
    End Property
    Public ReadOnly Property LocalResourceFile() As String
        Get
            If Not _Parent Is Nothing Then
                If TypeOf _Parent Is ModuleSettingsBase Then
                    Return CType(_Parent, ModuleSettingsBase).LocalResourceFile
                ElseIf TypeOf _Parent Is PortalModuleBase Then
                    Return CType(_Parent, PortalModuleBase).LocalResourceFile
                ElseIf TypeOf _Parent Is DotNetNuke.UI.Skins.SkinObjectBase Then
                    Return _LocalResourceFile
                ElseIf TypeOf _Parent Is DotNetNuke.Services.Scheduling.SchedulerClient Then
                    Return _LocalResourceFile
                ElseIf TypeOf _Parent Is System.Web.UI.Page Then
                    Return _LocalResourceFile
                End If
            End If
            Return ""
        End Get
    End Property
    Public ReadOnly Property ModulePath() As String
        Get
            If Not _Parent Is Nothing Then
                If TypeOf _Parent Is ModuleSettingsBase Then
                    Return CType(_Parent, ModuleSettingsBase).ControlPath 'was modulepath
                ElseIf TypeOf _Parent Is PortalModuleBase Then
                    Return CType(_Parent, PortalModuleBase).ControlPath 'was modulepath
                ElseIf TypeOf _Parent Is DotNetNuke.UI.Skins.SkinObjectBase Then
                    Return _Path
                ElseIf TypeOf _Parent Is DotNetNuke.Services.Scheduling.SchedulerClient Then
                    Return _Path
                ElseIf TypeOf _Parent Is System.Web.UI.Page Then
                    Return _Path
                End If
            End If
            Return ""
        End Get
    End Property
    Public ReadOnly Property ClientID() As String
        Get
            If Not _Parent Is Nothing Then
                If TypeOf _Parent Is ModuleSettingsBase Then
                    Return CType(_Parent, ModuleSettingsBase).ClientID
                ElseIf TypeOf _Parent Is PortalModuleBase Then
                    Return CType(_Parent, PortalModuleBase).ClientID
                ElseIf TypeOf _Parent Is DotNetNuke.UI.Skins.SkinObjectBase Then
                    Return CType(_Parent, DotNetNuke.UI.Skins.SkinObjectBase).ClientID
                ElseIf TypeOf _Parent Is DotNetNuke.Services.Scheduling.SchedulerClient Then
                    Return Me.ID
                ElseIf TypeOf _Parent Is System.Web.UI.Page Then
                    Return Me.ID
                End If
            End If
            Return ""
        End Get
    End Property
    Public ReadOnly Property IsEditable() As Boolean
        Get
            If Not _Parent Is Nothing Then
                If TypeOf _Parent Is ModuleSettingsBase Then
                    Return CType(_Parent, ModuleSettingsBase).IsEditable
                ElseIf TypeOf _Parent Is PortalModuleBase Then
                    Return CType(_Parent, PortalModuleBase).IsEditable
                ElseIf TypeOf _Parent Is DotNetNuke.UI.Skins.SkinObjectBase Then
                    Return False
                ElseIf TypeOf _Parent Is DotNetNuke.Services.Scheduling.SchedulerClient Then
                    Return False
                End If
            End If
            Return False
        End Get
    End Property

    Public ReadOnly Property UserInfo() As DotNetNuke.Entities.Users.UserInfo
        Get
            If Not _Parent Is Nothing Then
                If TypeOf _Parent Is ModuleSettingsBase Then
                    Return CType(_Parent, ModuleSettingsBase).UserInfo
                ElseIf TypeOf _Parent Is PortalModuleBase Then
                    Return CType(_Parent, PortalModuleBase).UserInfo
                ElseIf TypeOf _Parent Is DotNetNuke.UI.Skins.SkinObjectBase Then
                    Return DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo
                ElseIf TypeOf _Parent Is DotNetNuke.Services.Scheduling.SchedulerClient Then
                    Return DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo
                ElseIf TypeOf _Parent Is System.Web.UI.Page Then
                    Return DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo
                End If
            End If
            Return Nothing
        End Get
    End Property
    Public ReadOnly Property PortalSettings() As DotNetNuke.Entities.Portals.PortalSettings
        Get
            If Not _Parent Is Nothing Then
                If TypeOf _Parent Is ModuleSettingsBase Then
                    Return CType(_Parent, ModuleSettingsBase).PortalSettings
                ElseIf TypeOf _Parent Is PortalModuleBase Then
                    Return CType(_Parent, PortalModuleBase).PortalSettings
                ElseIf TypeOf _Parent Is DotNetNuke.UI.Skins.SkinObjectBase Then
                    If Not CType(_Parent, DotNetNuke.UI.Skins.SkinObjectBase).Page.Items Is Nothing AndAlso CType(_Parent, DotNetNuke.UI.Skins.SkinObjectBase).Page.Items.Contains("OWS_SCHEDULER") Then
                        Return DotNetNuke.Common.GetHostPortalSettings()
                    End If
                    Return CType(_Parent, DotNetNuke.UI.Skins.SkinObjectBase).PortalSettings

                ElseIf TypeOf _Parent Is DotNetNuke.Services.Scheduling.SchedulerClient Then
                    Return DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings()
                ElseIf TypeOf _Parent Is System.Web.UI.Page Then
                    Return DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings()
                End If
            End If
            Return Nothing
        End Get
    End Property
    Public ReadOnly Property UserId() As Integer
        Get
            If Not _Parent Is Nothing Then
                If TypeOf _Parent Is ModuleSettingsBase Then
                    Return CType(_Parent, ModuleSettingsBase).UserId
                ElseIf TypeOf _Parent Is PortalModuleBase Then
                    Return CType(_Parent, PortalModuleBase).UserId
                ElseIf TypeOf _Parent Is DotNetNuke.UI.Skins.SkinObjectBase Then
                    Return DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo.UserID
                ElseIf TypeOf _Parent Is DotNetNuke.Services.Scheduling.SchedulerClient Then
                    Return -1
                End If
            End If
            Return -1
        End Get
    End Property
    Public ReadOnly Property PortalId() As Integer
        Get
            If Not _Parent Is Nothing Then
                If TypeOf _Parent Is ModuleSettingsBase Then
                    Return CType(_Parent, ModuleSettingsBase).PortalId
                ElseIf TypeOf _Parent Is PortalModuleBase Then
                    Return CType(_Parent, PortalModuleBase).PortalId
                ElseIf TypeOf _Parent Is DotNetNuke.UI.Skins.SkinObjectBase Then
                    Return CType(_Parent, DotNetNuke.UI.Skins.SkinObjectBase).PortalSettings.PortalId
                ElseIf TypeOf _Parent Is DotNetNuke.Services.Scheduling.SchedulerClient Then
                    Return 0
                End If
            End If
            Return -1
        End Get
    End Property
    Public ReadOnly Property TabModuleId() As Integer
        Get
            If Not _Parent Is Nothing Then
                LoadModuleConfiguration()
                If TypeOf _Parent Is ModuleSettingsBase Then
                    Return CType(_Parent, ModuleSettingsBase).TabModuleId
                ElseIf TypeOf _Parent Is PortalModuleBase Then
                    Return CType(_Parent, PortalModuleBase).TabModuleId
                ElseIf TypeOf _Parent Is DotNetNuke.UI.Skins.SkinObjectBase Then
                    Return -1
                ElseIf TypeOf _Parent Is DotNetNuke.Services.Scheduling.SchedulerClient Then
                    Return -1
                End If
            End If
            Return -1
        End Get
    End Property
    Public ReadOnly Property ModuleId() As Integer
        Get
            If Not _Parent Is Nothing Then
                LoadModuleConfiguration()
                If TypeOf _Parent Is ModuleSettingsBase Then
                    Return _moduleConfiguration.ModuleID
                ElseIf TypeOf _Parent Is PortalModuleBase Then
                    Return CType(_Parent, PortalModuleBase).ModuleId
                ElseIf TypeOf _Parent Is DotNetNuke.UI.Skins.SkinObjectBase Then
                    If IsNumeric(_ModuleID) Then
                        Return CInt(ModuleId)
                    Else
                        Return -1
                    End If
                ElseIf TypeOf _Parent Is DotNetNuke.Services.Scheduling.SchedulerClient Then
                    Return -1
                End If
            End If
            Return -1
        End Get
    End Property
    Public ReadOnly Property TabId() As Integer
        Get
            If Not _Parent Is Nothing Then
                LoadModuleConfiguration()
                If TypeOf _Parent Is ModuleSettingsBase Then
                    Return _moduleConfiguration.TabID
                ElseIf TypeOf _Parent Is PortalModuleBase Then
                    Return CType(_Parent, PortalModuleBase).TabId
                ElseIf TypeOf _Parent Is DotNetNuke.UI.Skins.SkinObjectBase Then
                    Return CType(_Parent, DotNetNuke.UI.Skins.SkinObjectBase).PortalSettings.ActiveTab.TabID
                ElseIf TypeOf _Parent Is DotNetNuke.Services.Scheduling.SchedulerClient Then
                    Return -1
                End If
            End If
            Return -1
        End Get
    End Property

    Public Overrides Sub ClearCache()
        Try
            DotNetNuke.Common.Utilities.DataCache.ClearHostCache(True)
        Catch ex As Exception
        End Try
    End Sub
    Public Overrides Sub ClearSiteCache()
        Try
            DotNetNuke.Common.Utilities.DataCache.ClearPortalCache(PortalId, True)
        Catch ex As Exception
        End Try
    End Sub
    Public Overrides Sub ClearPageCache()
        Try
            DotNetNuke.Common.Utilities.DataCache.ClearModuleCache(TabId) 'was ClearTabCache
        Catch ex As Exception
        End Try
    End Sub

    Public ReadOnly Property Cache() As System.Web.Caching.Cache
        Get
            If Not _Parent Is Nothing Then
                If TypeOf _Parent Is ModuleSettingsBase Then
                    Return CType(_Parent, ModuleSettingsBase).Cache
                ElseIf TypeOf _Parent Is PortalModuleBase Then
                    Try
                        If Not CType(_Parent, PortalModuleBase).Cache Is Nothing Then
                            Return CType(_Parent, PortalModuleBase).Cache
                        Else
                            Dim c As New System.Web.Caching.Cache()
                            c.Item(".") = ""
                            Return c
                        End If
                    Catch ex As Exception
                        Return Nothing
                    End Try
                ElseIf TypeOf _Parent Is DotNetNuke.UI.Skins.SkinObjectBase Then
                    Return CType(_Parent, DotNetNuke.UI.Skins.SkinObjectBase).Cache
                End If
            End If
            If System.Web.HttpContext.Current Is Nothing OrElse System.Web.HttpContext.Current.Cache Is Nothing Then
                Return New Caching.Cache
            Else
                Return System.Web.HttpContext.Current.Cache
            End If
        End Get
    End Property
    Private _loadedModuleConfiguration As Boolean
    Private _moduleConfiguration As ModuleInfo
    Private _moduleSettings As Hashtable
    Private _settings As Hashtable
    Private _tabmodulesettings As Hashtable
    Private Sub LoadModuleConfiguration()
        If _loadedModuleConfiguration = False Then
            _loadedModuleConfiguration = True
            If Not _Parent Is Nothing Then
                If TypeOf _Parent Is ModuleSettingsBase Then
                    _moduleConfiguration = CType(_Parent, ModuleSettingsBase).ModuleConfiguration
                ElseIf TypeOf _Parent Is PortalModuleBase Then
                    _moduleConfiguration = CType(_Parent, PortalModuleBase).ModuleConfiguration
                ElseIf TypeOf _Parent Is DotNetNuke.UI.Skins.SkinObjectBase Then
                    _moduleConfiguration = New ModuleInfo
                ElseIf TypeOf _Parent Is DotNetNuke.Services.Scheduling.SchedulerClient Then
                    _moduleConfiguration = New ModuleInfo
                End If
            End If
            If _moduleConfiguration Is Nothing Then
                Dim mId As String = CType(_Parent, ModuleSettingsBase).Request.QueryString.Item("ModuleId")
                Dim tId As Integer = CType(_Parent, ModuleSettingsBase).PortalSettings.ActiveTab.TabID
                If Not mId Is Nothing AndAlso IsNumeric(mId) Then
                    Dim mcTM As New DotNetNuke.Entities.Modules.ModuleController
                    _moduleConfiguration = mcTM.GetModule(CInt(mId), tId)
                    'OTHER ITEMS HERE.
                End If
            End If
        End If
    End Sub



    Public ReadOnly Property UniqueId() As String
        Get
            If Not _Parent Is Nothing Then
                If TypeOf _Parent Is ModuleSettingsBase Then
                    Return CType(_Parent, ModuleSettingsBase).UniqueID
                ElseIf TypeOf _Parent Is PortalModuleBase Then
                    Return CType(_Parent, PortalModuleBase).UniqueID
                ElseIf TypeOf _Parent Is DotNetNuke.UI.Skins.SkinObjectBase Then
                    Return CType(_Parent, DotNetNuke.UI.Skins.SkinObjectBase).UniqueID
                ElseIf TypeOf _Parent Is DotNetNuke.Services.Scheduling.SchedulerClient Then
                    Return Me.ID
                ElseIf TypeOf _Parent Is System.Web.UI.Page Then
                    Return Me.ID
                End If
            End If
            Return "OWS1"
        End Get
    End Property
    Public ReadOnly Property Settings() As Hashtable
        Get
            If Not _Parent Is Nothing Then
                If TypeOf _Parent Is ModuleSettingsBase Then
                    Return CType(_Parent, ModuleSettingsBase).Settings
                ElseIf TypeOf _Parent Is PortalModuleBase Then
                    If _settings Is Nothing AndAlso Not PortalSettings Is Nothing Then
                        'Merge the TabModuleSettings and ModuleSettings
                        For Each strKey As String In TabModuleSettings.Keys
                            ModuleSettings(strKey) = TabModuleSettings(strKey)
                        Next
                        _settings = ModuleSettings
                    End If
                    Return _settings
                    '                    Return CType(_Parent, PortalModuleBase).Settings
                ElseIf TypeOf _Parent Is DotNetNuke.UI.Skins.SkinObjectBase Then
                    'CHECK TO SEE IF A MODULEID IS PRESENT!
                    'Return CType(_Parent, DotNetNuke.UI.Skins.SkinObjectBase)
                    Return New Hashtable
                ElseIf TypeOf _Parent Is DotNetNuke.Services.Scheduling.SchedulerClient Then
                    Return New Hashtable
                End If
            End If
            Return New Hashtable
        End Get
    End Property
    Private ReadOnly Property ModuleSettings() As Hashtable
        Get
            If _moduleSettings Is Nothing AndAlso Not PortalSettings Is Nothing AndAlso ModuleId >= 0 Then
                'Get ModuleSettings
                _moduleSettings = ModuleController.Instance().GetModule(ModuleId, TabId, False).ModuleSettings
                '_moduleSettings = PortalSettings.GetModuleSettings(ModuleId)
            End If
            Return _moduleSettings
        End Get
    End Property

    Private ReadOnly Property TabModuleSettings() As Hashtable
        Get
            If _tabmodulesettings Is Nothing AndAlso Not PortalSettings Is Nothing AndAlso TabModuleId >= 0 Then
                'Get TabModuleSettings
                '_tabmodulesettings = PortalSettings.GetTabModuleSettings(TabModuleId)
                _tabmodulesettings = ModuleController.Instance().GetTabModule(TabModuleId).TabModuleSettings
            End If
            Return _tabmodulesettings
        End Get
    End Property

    Public ReadOnly Property Request() As HttpRequest
        Get
            If Not _Parent Is Nothing Then
                If TypeOf _Parent Is ModuleSettingsBase Then
                    Return CType(_Parent, ModuleSettingsBase).Request
                ElseIf TypeOf _Parent Is PortalModuleBase Then
                    Return CType(_Parent, PortalModuleBase).Request
                ElseIf TypeOf _Parent Is DotNetNuke.UI.Skins.SkinObjectBase Then
                    Return CType(_Parent, DotNetNuke.UI.Skins.SkinObjectBase).Request
                ElseIf TypeOf _Parent Is DotNetNuke.Services.Scheduling.SchedulerClient Then
                    Return System.Web.HttpContext.Current.Request
                End If
            End If
            Return System.Web.HttpContext.Current.Request
        End Get
    End Property

    Public ReadOnly Property Session() As Utilities.GenericSession
        Get
            Dim _session As SessionState.HttpSessionState = Nothing
            Try
                If Not _Parent Is Nothing Then
                    If TypeOf _Parent Is ModuleSettingsBase Then
                        _session = CType(_Parent, ModuleSettingsBase).Session
                    ElseIf TypeOf _Parent Is PortalModuleBase Then
                        _session = CType(_Parent, PortalModuleBase).Session
                    ElseIf TypeOf _Parent Is DotNetNuke.UI.Skins.SkinObjectBase Then
                        _session = CType(_Parent, DotNetNuke.UI.Skins.SkinObjectBase).Session
                    End If
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

End Class