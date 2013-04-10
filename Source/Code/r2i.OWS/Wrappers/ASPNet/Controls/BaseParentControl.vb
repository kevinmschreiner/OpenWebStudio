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
'Imports r2i.OWS.Wrapper.ASPNET.DataAccess

'Public Class BaseParentControl
'    Inherits r2i.OWS.Framework.UI.Control

'    Private _Parent As Object
'    Private _LocalResourceFile As String
'    Private _LocalResourceKey As String
'    Private _Path As String
'    Private _ModuleID As String

'    Public Sub New(ByVal ParentObject As System.Web.UI.Control, ByVal ResourceFile As String, ByVal ResourceKey As String, ByVal Path As String, ByVal ModuleID As String)
'        _Parent = ParentObject
'        _LocalResourceFile = ResourceFile
'        _LocalResourceKey = ResourceKey
'        _ModuleID = ModuleID
'        _Path = Path
'    End Sub
'    Public Sub New(ByVal ResourceFile As String, ByVal ResourceKey As String, ByVal Path As String, ByVal ModuleID As String)
'        _LocalResourceFile = ResourceFile
'        _LocalResourceKey = ResourceKey
'        _ModuleID = ModuleID
'        _Path = Path
'    End Sub
'    Public Property Parent() As Object
'        Get
'            Return _Parent
'        End Get
'        Set(ByVal value As Object)
'            _Parent = value
'        End Set
'    End Property

'    Public Function MapPath(ByVal Path As String) As String
'        If Not _Parent Is Nothing Then
'            If TypeOf _Parent Is System.Web.UI.Control Then
'                Return System.Web.HttpContext.Current.Server.MapPath(Path)
'            ElseIf TypeOf _Parent Is System.Web.UI.Page Then
'                Return System.Web.HttpContext.Current.Server.MapPath(Path)
'            End If
'        End If
'        Return ""
'    End Function
'    Public ReadOnly Property ModuleConfiguration() As ModuleInfo
'        Get
'            If Not _Parent Is Nothing Then
'                LoadModuleConfiguration()
'                If TypeOf _Parent Is System.Web.UI.Control Then
'                    Return _moduleConfiguration
'                ElseIf TypeOf _Parent Is System.Web.UI.Page Then
'                    Return _moduleConfiguration
'                End If
'            End If
'            Return Nothing
'        End Get
'    End Property
'    Public ReadOnly Property LocalResourceFile() As String
'        Get
'            If Not _Parent Is Nothing Then
'                If TypeOf _Parent Is System.Web.UI.Control Then
'                    Return _LocalResourceFile
'                ElseIf TypeOf _Parent Is System.Web.UI.Page Then
'                    Return _LocalResourceFile
'                End If
'            End If
'            Return ""
'        End Get
'    End Property
'    Public ReadOnly Property ModulePath() As String
'        Get
'            If Not _Parent Is Nothing Then
'               If TypeOf _Parent Is System.Web.UI.Control Then
'                    Return _Path
'                ElseIf TypeOf _Parent Is System.Web.UI.Page Then
'                    Return _Path
'                End If
'            End If
'            Return ""
'        End Get
'    End Property
'    Public ReadOnly Property ClientID() As String
'        Get
'            If Not _Parent Is Nothing Then
'                If TypeOf _Parent Is System.Web.UI.Control Then
'                    Return Me.ID
'                ElseIf TypeOf _Parent Is System.Web.UI.Page Then
'                    Return Me.ID
'                End If
'            End If
'            Return ""
'        End Get
'    End Property
'    Public ReadOnly Property IsEditable() As Boolean
'        Get
'            If Not _Parent Is Nothing Then
'                If TypeOf _Parent Is System.Web.UI.Control Then
'                    Return False
'                ElseIf TypeOf _Parent Is System.Web.UI.Page Then
'                    Return False
'                End If
'            End If
'            Return False
'        End Get
'    End Property

'    'Public ReadOnly Property UserInfo() As DotNetNuke.Entities.Users.UserInfo
'    '    Get
'    '        If Not _Parent Is Nothing Then
'    '            If TypeOf _Parent Is System.Web.UI.Control Then
'    '                Return DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo
'    '            ElseIf TypeOf _Parent Is System.Web.UI.Page Then
'    '                Return DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo
'    '            End If
'    '        End If
'    '        Return Nothing
'    '    End Get
'    'End Property
'    'Public ReadOnly Property PortalSettings() As DotNetNuke.Entities.Portals.PortalSettings
'    '    Get
'    '        If Not _Parent Is Nothing Then
'    '            If TypeOf _Parent Is ModuleSettingsBase Then
'    '                Return CType(_Parent, ModuleSettingsBase).PortalSettings
'    '            ElseIf TypeOf _Parent Is PortalModuleBase Then
'    '                Return CType(_Parent, PortalModuleBase).PortalSettings
'    '            ElseIf TypeOf _Parent Is DotNetNuke.UI.Skins.SkinObjectBase Then
'    '                If Not CType(_Parent, DotNetNuke.UI.Skins.SkinObjectBase).Page.Items Is Nothing AndAlso CType(_Parent, DotNetNuke.UI.Skins.SkinObjectBase).Page.Items.Contains("OWS_SCHEDULER") Then
'    '                    Return DotNetNuke.Common.GetHostPortalSettings()
'    '                End If
'    '                Return CType(_Parent, DotNetNuke.UI.Skins.SkinObjectBase).PortalSettings

'    '            ElseIf TypeOf _Parent Is DotNetNuke.Services.Scheduling.SchedulerClient Then
'    '                Return DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings()
'    '            ElseIf TypeOf _Parent Is System.Web.UI.Page Then
'    '                Return DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings()
'    '            End If
'    '        End If
'    '        Return Nothing
'    '    End Get
'    'End Property
'    'Public ReadOnly Property UserId() As Integer
'    '    Get
'    '        If Not _Parent Is Nothing Then
'    '            If TypeOf _Parent Is ModuleSettingsBase Then
'    '                Return CType(_Parent, ModuleSettingsBase).UserId
'    '            ElseIf TypeOf _Parent Is PortalModuleBase Then
'    '                Return CType(_Parent, PortalModuleBase).UserId
'    '            ElseIf TypeOf _Parent Is DotNetNuke.UI.Skins.SkinObjectBase Then
'    '                Return DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo.UserID
'    '            ElseIf TypeOf _Parent Is DotNetNuke.Services.Scheduling.SchedulerClient Then
'    '                Return -1
'    '            End If
'    '        End If
'    '        Return -1
'    '    End Get
'    'End Property
'    'Public ReadOnly Property PortalId() As Integer
'    '    Get
'    '        If Not _Parent Is Nothing Then
'    '            If TypeOf _Parent Is ModuleSettingsBase Then
'    '                Return CType(_Parent, ModuleSettingsBase).PortalId
'    '            ElseIf TypeOf _Parent Is PortalModuleBase Then
'    '                Return CType(_Parent, PortalModuleBase).PortalId
'    '            ElseIf TypeOf _Parent Is DotNetNuke.UI.Skins.SkinObjectBase Then
'    '                Return CType(_Parent, DotNetNuke.UI.Skins.SkinObjectBase).PortalSettings.PortalId
'    '            ElseIf TypeOf _Parent Is DotNetNuke.Services.Scheduling.SchedulerClient Then
'    '                Return 0
'    '            End If
'    '        End If
'    '        Return -1
'    '    End Get
'    'End Property
'    'Public ReadOnly Property TabModuleId() As Integer
'    '    Get
'    '        If Not _Parent Is Nothing Then
'    '            LoadModuleConfiguration()
'    '            If TypeOf _Parent Is ModuleSettingsBase Then
'    '                Return CType(_Parent, ModuleSettingsBase).TabModuleId
'    '            ElseIf TypeOf _Parent Is PortalModuleBase Then
'    '                Return CType(_Parent, PortalModuleBase).TabModuleId
'    '            ElseIf TypeOf _Parent Is DotNetNuke.UI.Skins.SkinObjectBase Then
'    '                Return -1
'    '            ElseIf TypeOf _Parent Is DotNetNuke.Services.Scheduling.SchedulerClient Then
'    '                Return -1
'    '            End If
'    '        End If
'    '        Return -1
'    '    End Get
'    'End Property
'    'Public ReadOnly Property ModuleId() As Integer
'    '    Get
'    '        If Not _Parent Is Nothing Then
'    '            LoadModuleConfiguration()
'    '            If TypeOf _Parent Is ModuleSettingsBase Then
'    '                Return _moduleConfiguration.ModuleID
'    '            ElseIf TypeOf _Parent Is PortalModuleBase Then
'    '                Return CType(_Parent, PortalModuleBase).ModuleId
'    '            ElseIf TypeOf _Parent Is DotNetNuke.UI.Skins.SkinObjectBase Then
'    '                If IsNumeric(_ModuleID) Then
'    '                    Return CInt(ModuleId)
'    '                Else
'    '                    Return -1
'    '                End If
'    '            ElseIf TypeOf _Parent Is DotNetNuke.Services.Scheduling.SchedulerClient Then
'    '                Return -1
'    '            End If
'    '        End If
'    '        Return -1
'    '    End Get
'    'End Property
'    'Public ReadOnly Property TabId() As Integer
'    '    Get
'    '        If Not _Parent Is Nothing Then
'    '            LoadModuleConfiguration()
'    '            If TypeOf _Parent Is ModuleSettingsBase Then
'    '                Return _moduleConfiguration.TabID
'    '            ElseIf TypeOf _Parent Is PortalModuleBase Then
'    '                Return CType(_Parent, PortalModuleBase).TabId
'    '            ElseIf TypeOf _Parent Is DotNetNuke.UI.Skins.SkinObjectBase Then
'    '                Return CType(_Parent, DotNetNuke.UI.Skins.SkinObjectBase).PortalSettings.ActiveTab.TabID
'    '            ElseIf TypeOf _Parent Is DotNetNuke.Services.Scheduling.SchedulerClient Then
'    '                Return -1
'    '            End If
'    '        End If
'    '        Return -1
'    '    End Get
'    'End Property

'    'Public Overrides Sub ClearCache()
'    '    Try
'    '        DotNetNuke.Common.Utilities.DataCache.ClearHostCache(True)
'    '    Catch ex As Exception
'    '    End Try
'    'End Sub
'    'Public Overrides Sub ClearSiteCache()
'    '    Try
'    '        DotNetNuke.Common.Utilities.DataCache.ClearPortalCache(PortalId, True)
'    '    Catch ex As Exception
'    '    End Try
'    'End Sub
'    'Public Overrides Sub ClearPageCache()
'    '    Try
'    '        DotNetNuke.Common.Utilities.DataCache.ClearTabCache(TabId, PortalId)
'    '    Catch ex As Exception
'    '    End Try
'    'End Sub

'    Public ReadOnly Property Cache() As System.Web.Caching.Cache
'        Get
'            If System.Web.HttpContext.Current Is Nothing OrElse System.Web.HttpContext.Current.Cache Is Nothing Then
'                Return New Caching.Cache
'            Else
'                Return System.Web.HttpContext.Current.Cache
'            End If
'        End Get
'    End Property
'    Private _loadedModuleConfiguration As Boolean
'    Private _moduleConfiguration As DataAccess.ModuleInfo
'    Private _moduleSettings As Hashtable
'    Private _settings As Hashtable
'    Private _tabmodulesettings As Hashtable
'    Private Sub LoadModuleConfiguration()
'        If _loadedModuleConfiguration = False Then
'            _loadedModuleConfiguration = True
'            'If Not _Parent Is Nothing Then
'            '    If TypeOf _Parent Is ModuleSettingsBase Then
'            '        _moduleConfiguration = CType(_Parent, ModuleSettingsBase).ModuleConfiguration
'            '    ElseIf TypeOf _Parent Is PortalModuleBase Then
'            '        _moduleConfiguration = CType(_Parent, PortalModuleBase).ModuleConfiguration
'            '    ElseIf TypeOf _Parent Is DotNetNuke.UI.Skins.SkinObjectBase Then
'            '        _moduleConfiguration = New ModuleInfo
'            '    ElseIf TypeOf _Parent Is DotNetNuke.Services.Scheduling.SchedulerClient Then
'            '        _moduleConfiguration = New ModuleInfo
'            '    End If
'            'End If
'            'If _moduleConfiguration Is Nothing Then
'            '    Dim mId As String = CType(_Parent, ModuleSettingsBase).Request.QueryString.Item("ModuleId")
'            '    Dim tId As Integer = CType(_Parent, ModuleSettingsBase).PortalSettings.ActiveTab.TabID
'            '    If Not mId Is Nothing AndAlso IsNumeric(mId) Then
'            '        Dim mcTM As New DotNetNuke.Entities.Modules.ModuleController
'            '        _moduleConfiguration = mcTM.GetModule(CInt(mId), tId)
'            '        'OTHER ITEMS HERE.
'            '    End If
'            'End If
'        End If
'    End Sub



'    Public ReadOnly Property UniqueId() As String
'        Get
'            If Not _Parent Is Nothing Then
'                If TypeOf _Parent Is System.Web.UI.Control Then
'                    Return Me.ID
'                ElseIf TypeOf _Parent Is System.Web.UI.Page Then
'                    Return Me.ID
'                End If
'            End If
'            Return "OWS1"
'        End Get
'    End Property
'    Public ReadOnly Property Settings() As Hashtable
'        Get
'            If Not _Parent Is Nothing Then
'                Return New Hashtable
'            End If
'            Return New Hashtable
'        End Get
'    End Property
'    Private ReadOnly Property ModuleSettings() As Hashtable
'        Get
'            'If _moduleSettings Is Nothing AndAlso Not PortalSettings Is Nothing AndAlso ModuleId >= 0 Then
'            '    'Get ModuleSettings
'            '    _moduleSettings = PortalSettings.GetModuleSettings(ModuleId)
'            'End If
'            Return _moduleSettings
'        End Get
'    End Property

'    Private ReadOnly Property TabModuleSettings() As Hashtable
'        Get
'            'If _tabmodulesettings Is Nothing AndAlso Not PortalSettings Is Nothing AndAlso TabModuleId >= 0 Then
'            '    'Get TabModuleSettings
'            '    _tabmodulesettings = PortalSettings.GetTabModuleSettings(TabModuleId)
'            'End If
'            Return _tabmodulesettings
'        End Get
'    End Property

'    Public ReadOnly Property Request() As HttpRequest
'        Get
'            Return System.Web.HttpContext.Current.Request
'        End Get
'    End Property

'    Public ReadOnly Property Session() As Utilities.GenericSession
'        Get
'            Dim _session As SessionState.HttpSessionState = Nothing
'            Try
'                If Not _Parent Is Nothing Then
'                    If TypeOf _Parent Is System.Web.UI.Page Then
'                        _session = CType(_Parent, System.Web.UI.Page).Session
'                    ElseIf TypeOf _Parent Is System.Web.UI.Control Then
'                        _session = CType(_Parent, System.Web.UI.Control).Page.Session
'                    End If
'                End If
'                If _session Is Nothing Then
'                    _session = System.Web.HttpContext.Current.Session
'                End If

'            Catch ex As Exception

'            End Try
'            If _session Is Nothing Then
'                If Not System.Web.HttpContext.Current Is Nothing Then
'                    Dim s As Utilities.GenericSession = Nothing
'                    If System.Web.HttpContext.Current.Items.Item("OWS.EMPTYSESSION") Is Nothing Then
'                        s = New Utilities.GenericSession()
'                        System.Web.HttpContext.Current.Items.Item("OWS.EMPTYSESSION") = s
'                    Else
'                        s = CType(System.Web.HttpContext.Current.Items.Item("OWS.EMPTYSESSION"), Utilities.GenericSession)
'                    End If
'                    Return s
'                End If
'            Else
'                Return New Utilities.GenericSession(_session)
'            End If
'            Return Nothing
'        End Get
'    End Property

'End Class