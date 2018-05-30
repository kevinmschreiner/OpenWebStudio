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
Imports System.Web
Imports DotNetNuke.Entities.Modules

Namespace DataAccess
    Public Class ModuleInfo
        Implements IModuleInfo

        Private _obj As DotNetNuke.Entities.Modules.ModuleInfo
        Public Sub New()
            _obj = New DotNetNuke.Entities.Modules.ModuleInfo
        End Sub
        Public Sub New(ByVal obj As DotNetNuke.Entities.Modules.ModuleInfo)
            _obj = obj
            If _obj Is Nothing Then
                _obj = New DotNetNuke.Entities.Modules.ModuleInfo
            End If
        End Sub
        Public Sub New(ByVal ModuleID As String, ByVal TabID As String)
            If IsNumeric(ModuleID) AndAlso IsNumeric(TabID) Then
                _obj = (New DotNetNuke.Entities.Modules.ModuleController).GetModule(CInt(ModuleID), CInt(TabID))
            End If
            If _obj Is Nothing Then
                _obj = New DotNetNuke.Entities.Modules.ModuleInfo
                If IsNumeric(TabID) Then
                    _obj.TabID = CInt(TabID)
                End If
            End If
        End Sub
        Public Property Alignment() As String Implements Framework.DataAccess.IModuleInfo.Alignment
            Get
                Return _obj.Alignment
            End Get
            Set(ByVal value As String)
                _obj.Alignment = value
            End Set
        End Property

        Public Property AllModules() As Boolean Implements Framework.DataAccess.IModuleInfo.AllModules
            Get
                Return _obj.AllModules
            End Get
            Set(ByVal value As Boolean)
                _obj.AllModules = value
            End Set
        End Property

        Public Property AllTabs() As Boolean Implements Framework.DataAccess.IModuleInfo.AllTabs
            Get
                Return _obj.AllTabs
            End Get
            Set(ByVal value As Boolean)
                _obj.AllTabs = value
            End Set
        End Property

        Public Property AuthorizedEditRoles() As String Implements Framework.DataAccess.IModuleInfo.AuthorizedEditRoles
            Get
                Return _obj.AuthorizedEditRoles
            End Get
            Set(ByVal value As String)
                _obj.AuthorizedEditRoles = value
            End Set
        End Property

        Public Property AuthorizedRoles() As String Implements Framework.DataAccess.IModuleInfo.AuthorizedRoles
            Get
                Return _obj.AuthorizedRoles
            End Get
            Set(ByVal value As String)
                _obj.AuthorizedRoles = value
            End Set
        End Property

        Public Property AuthorizedViewRoles() As String Implements Framework.DataAccess.IModuleInfo.AuthorizedViewRoles
            Get
                Return _obj.AuthorizedViewRoles
            End Get
            Set(ByVal value As String)
                _obj.AuthorizedViewRoles = value
            End Set
        End Property

        Public Property Border() As String Implements Framework.DataAccess.IModuleInfo.Border
            Get
                Return _obj.Border
            End Get
            Set(ByVal value As String)
                _obj.Border = value
            End Set
        End Property

        Public Property BusinessControllerClass() As String Implements Framework.DataAccess.IModuleInfo.BusinessControllerClass
            Get
                Return _obj.DesktopModule.BusinessControllerClass
            End Get
            Set(ByVal value As String)
                _obj.DesktopModule.BusinessControllerClass = value
            End Set
        End Property

        Public Property CacheTime() As Integer Implements Framework.DataAccess.IModuleInfo.CacheTime
            Get
                Return _obj.CacheTime
            End Get
            Set(ByVal value As Integer)
                _obj.CacheTime = value
            End Set
        End Property

        Public Property Color() As String Implements Framework.DataAccess.IModuleInfo.Color
            Get
                Return _obj.Color
            End Get
            Set(ByVal value As String)
                _obj.Color = value
            End Set
        End Property

        Public Property ContainerPath() As String Implements Framework.DataAccess.IModuleInfo.ContainerPath
            Get
                Return _obj.ContainerPath
            End Get
            Set(ByVal value As String)
                _obj.ContainerPath = value
            End Set
        End Property

        Public Property ContainerSrc() As String Implements Framework.DataAccess.IModuleInfo.ContainerSrc
            Get
                Return _obj.ContainerSrc
            End Get
            Set(ByVal value As String)
                _obj.ContainerSrc = value
            End Set
        End Property

        Public Property ControlSrc() As String Implements Framework.DataAccess.IModuleInfo.ControlSrc
            Get
                Return _obj.ModuleControl.ControlSrc
            End Get
            Set(ByVal value As String)
                _obj.ModuleControl.ControlSrc = value
            End Set
        End Property

        Public Property ControlTitle() As String Implements Framework.DataAccess.IModuleInfo.ControlTitle
            Get
                Return _obj.ModuleControl.ControlTitle
            End Get
            Set(ByVal value As String)
                _obj.ModuleControl.ControlTitle = value
            End Set
        End Property

        Public Property Description() As String Implements Framework.DataAccess.IModuleInfo.Description
            Get
                Return _obj.DesktopModule.Description
            End Get
            Set(ByVal value As String)
                _obj.DesktopModule.Description = value
            End Set
        End Property

        Public Property DesktopModuleID() As String Implements Framework.DataAccess.IModuleInfo.DesktopModuleID
            Get
                Return CStr(_obj.DesktopModuleID)
            End Get
            Set(ByVal value As String)
                _obj.DesktopModuleID = CInt(value)
            End Set
        End Property

        Public Property DisplayPrint() As Boolean Implements Framework.DataAccess.IModuleInfo.DisplayPrint
            Get
                Return _obj.DisplayPrint
            End Get
            Set(ByVal value As Boolean)
                _obj.DisplayPrint = value
            End Set
        End Property

        Public Property DisplaySyndicate() As Boolean Implements Framework.DataAccess.IModuleInfo.DisplaySyndicate
            Get
                Return _obj.DisplaySyndicate
            End Get
            Set(ByVal value As Boolean)
                _obj.DisplaySyndicate = value
            End Set
        End Property

        Public Property DisplayTitle() As Boolean Implements Framework.DataAccess.IModuleInfo.DisplayTitle
            Get
                Return _obj.DisplayTitle
            End Get
            Set(ByVal value As Boolean)
                _obj.DisplayTitle = value
            End Set
        End Property

        Public Property EndDate() As Date Implements Framework.DataAccess.IModuleInfo.EndDate
            Get
                Return _obj.EndDate
            End Get
            Set(ByVal value As Date)
                _obj.EndDate = value
            End Set
        End Property

        Public Property Footer() As String Implements Framework.DataAccess.IModuleInfo.Footer
            Get
                Return _obj.Footer
            End Get
            Set(ByVal value As String)
                _obj.Footer = value
            End Set
        End Property

        Public Property FriendlyName() As String Implements Framework.DataAccess.IModuleInfo.FriendlyName
            Get
                Return _obj.DesktopModule.FriendlyName
            End Get
            Set(ByVal value As String)
                _obj.DesktopModule.FriendlyName = value
            End Set
        End Property

        Public Property Header() As String Implements Framework.DataAccess.IModuleInfo.Header
            Get
                Return _obj.Header
            End Get
            Set(ByVal value As String)
                _obj.Header = value
            End Set
        End Property

        Public Property HelpUrl() As String Implements Framework.DataAccess.IModuleInfo.HelpUrl
            Get
                Return _obj.ModuleControl.HelpURL
            End Get
            Set(ByVal value As String)
                _obj.ModuleControl.HelpURL = value
            End Set
        End Property

        Public Property IconFile() As String Implements Framework.DataAccess.IModuleInfo.IconFile
            Get
                Return _obj.IconFile
            End Get
            Set(ByVal value As String)
                _obj.IconFile = value

            End Set
        End Property

        Public Property InheritViewPermissions() As Boolean Implements Framework.DataAccess.IModuleInfo.InheritViewPermissions
            Get
                Return _obj.InheritViewPermissions
            End Get
            Set(ByVal value As Boolean)
                _obj.InheritViewPermissions = value
            End Set
        End Property

        Public Property IsAdmin() As Boolean Implements Framework.DataAccess.IModuleInfo.IsAdmin
            Get
                Return _obj.DesktopModule.IsAdmin
            End Get
            Set(ByVal value As Boolean)
                _obj.DesktopModule.IsAdmin = value
            End Set
        End Property

        Public Property IsDefaultModule() As Boolean Implements Framework.DataAccess.IModuleInfo.IsDefaultModule
            Get
                Return _obj.IsDefaultModule
            End Get
            Set(ByVal value As Boolean)
                _obj.IsDefaultModule = value
            End Set
        End Property

        Public Property IsDeleted() As Boolean Implements Framework.DataAccess.IModuleInfo.IsDeleted
            Get
                Return _obj.IsDeleted
            End Get
            Set(ByVal value As Boolean)
                _obj.IsDeleted = value
            End Set
        End Property

        Public ReadOnly Property IsPortable() As Boolean Implements Framework.DataAccess.IModuleInfo.IsPortable
            Get
                Return _obj.IsPortable
            End Get
        End Property

        Public Property IsPremium() As Boolean Implements Framework.DataAccess.IModuleInfo.IsPremium
            Get
                Return _obj.IsPremium
            End Get
            Set(ByVal value As Boolean)
                _obj.IsPremium = value
            End Set
        End Property

        Public ReadOnly Property IsSearchable() As Boolean Implements Framework.DataAccess.IModuleInfo.IsSearchable
            Get
                Return _obj.IsSearchable
            End Get
        End Property

        Public ReadOnly Property IsUpgradeable() As Boolean Implements Framework.DataAccess.IModuleInfo.IsUpgradeable
            Get
                Return _obj.IsUpgradeable
            End Get
        End Property

        Public Property ModuleControlId() As String Implements Framework.DataAccess.IModuleInfo.ModuleControlId
            Get
                Return CStr(_obj.ModuleControlId)
            End Get
            Set(ByVal value As String)
                _obj.ModuleControlId = CInt(value)
            End Set
        End Property

        Public Property ModuleDefID() As String Implements Framework.DataAccess.IModuleInfo.ModuleDefID
            Get
                Return CStr(_obj.ModuleDefID)
            End Get
            Set(ByVal value As String)
                _obj.ModuleDefID = CInt(value)
            End Set
        End Property

        Public Property ModuleID() As String Implements Framework.DataAccess.IModuleInfo.ModuleID
            Get
                Return CStr(_obj.ModuleID)
            End Get
            Set(ByVal value As String)
                _obj.ModuleID = CInt(value)
            End Set
        End Property

        Public Property ModuleOrder() As Integer Implements Framework.DataAccess.IModuleInfo.ModuleOrder
            Get
                Return _obj.ModuleOrder
            End Get
            Set(ByVal value As Integer)
                _obj.ModuleOrder = value
            End Set
        End Property

        Public Property ModulePermissions() As Framework.DataAccess.IModulePermissionCollection Implements Framework.DataAccess.IModuleInfo.ModulePermissions
            Get
                Return (New DataAccess.ModulePermissionCollection(_obj.ModulePermissions))
            End Get
            Set(ByVal value As Framework.DataAccess.IModulePermissionCollection)
                _obj.ModulePermissions = CType(CType(value, DataAccess.ModulePermissionCollection).Save, DotNetNuke.Security.Permissions.ModulePermissionCollection)
            End Set
        End Property

        Public Property ModuleTitle() As String Implements Framework.DataAccess.IModuleInfo.ModuleTitle
            Get
                Return _obj.ModuleTitle
            End Get
            Set(ByVal value As String)
                _obj.ModuleTitle = value
            End Set
        End Property

        Public Property PaneModuleCount() As Integer Implements Framework.DataAccess.IModuleInfo.PaneModuleCount
            Get

            End Get
            Set(ByVal value As Integer)

            End Set
        End Property

        Public Property PaneModuleIndex() As Integer Implements Framework.DataAccess.IModuleInfo.PaneModuleIndex
            Get
                Return _obj.PaneModuleCount
            End Get
            Set(ByVal value As Integer)
                _obj.PaneModuleCount = value
            End Set
        End Property

        Public Property PaneName() As String Implements Framework.DataAccess.IModuleInfo.PaneName
            Get
                Return _obj.PaneName
            End Get
            Set(ByVal value As String)
                _obj.PaneName = value
            End Set
        End Property

        Public Property PortalID() As String Implements Framework.DataAccess.IModuleInfo.PortalID
            Get
                Return CStr(_obj.PortalID)
            End Get
            Set(ByVal value As String)
                _obj.PortalID = CInt(value)
            End Set
        End Property

        Public Property StartDate() As Date Implements Framework.DataAccess.IModuleInfo.StartDate
            Get
                Return _obj.StartDate
            End Get
            Set(ByVal value As Date)
                _obj.StartDate = value
            End Set
        End Property

        Public Property SupportedFeatures() As Integer Implements Framework.DataAccess.IModuleInfo.SupportedFeatures
            Get
                Return _obj.SupportedFeatures
            End Get
            Set(ByVal value As Integer)
                _obj.SupportedFeatures = value
            End Set
        End Property

        Public Property TabID() As String Implements Framework.DataAccess.IModuleInfo.TabID
            Get
                Return CStr(_obj.TabID)
            End Get
            Set(ByVal value As String)
                _obj.TabID = CInt(value)
            End Set
        End Property

        Public Property TabModuleID() As String Implements Framework.DataAccess.IModuleInfo.TabModuleID
            Get
                Return CStr(_obj.TabModuleID)
            End Get
            Set(ByVal value As String)
                _obj.TabModuleID = CInt(value)
            End Set
        End Property

        Public Property Version() As String Implements Framework.DataAccess.IModuleInfo.Version
            Get
                Return _obj.Version
            End Get
            Set(ByVal value As String)
                _obj.Version = value
            End Set
        End Property

        Public Sub Load(ByVal obj As Object) Implements Framework.DataAccess.IModuleInfo.Load
            _obj = CType(obj, DotNetNuke.Entities.Modules.ModuleInfo)
        End Sub

        Public Function Save() As Object Implements Framework.DataAccess.IModuleInfo.Save
            Return _obj
        End Function
    End Class
    'Public Class ModuleInfo
    '    Implements IModuleInfo

    '    Private m_ModuleId As String
    '    Private m_ModuleOrder As Integer
    '    Private m_ModuleTitle As String
    '    Private m_PaneName As String
    '    Private m_FriendlyName As String
    '    Private m_Header As String
    '    Private m_Footer As String
    '    Private m_TabId As String
    '    Private m_IsDeleted As Boolean
    '    Private m_ModuleDefId As String
    '    Private m_PortalId As String
    '    Private m_InheritViewPermissions As Boolean
    '    Private m_Description As String
    '    Private m_AuthorizedViewRoles As String
    '    Private m_ModulePermissions As IModulePermissionCollection


    '    Private moduleInfoTotallyLoaded As Boolean

    '    Private moduleInfo As DotNetNuke.Entities.Modules.ModuleInfo
    '    Private moduleCtl As New DotNetNuke.Entities.Modules.ModuleController

    '    'TODO: Create a common Clas to get the context
    '    Protected ctx As HttpContext = HttpContext.Current
    '    Private currentPortalModuleBase As BaseParentControl

    '    Public Sub New()
    '        currentPortalModuleBase = DnnSingleton.GetInstance(ctx).CurrentModuleBase
    '        LoadModule()
    '    End Sub
    '    Public Sub New(ByVal portalModuleBase As BaseParentControl)
    '        currentPortalModuleBase = portalModuleBase
    '        LoadModule()
    '    End Sub
    '    'Public Sub New(ByVal portalModuleBase As PortalModuleBase)
    '    '    currentPortalModuleBase = PortalModuleBase
    '    '    LoadModule()
    '    'End Sub
    '    Public Sub New(ByVal ModuleID As String, ByVal TabID As String)
    '        m_ModuleId = ModuleID
    '        m_TabId = TabID
    '        LoadModule()
    '    End Sub

    '    Public Property ModuleID() As String Implements IModuleInfo.ModuleID
    '        Get
    '            LoadModule()
    '            Return m_ModuleId
    '            'Return CStr(currentPortalModuleBase.ModuleId)
    '        End Get
    '        Set(ByVal value As String)
    '            m_ModuleId = value
    '        End Set
    '    End Property

    '    Public Property ModuleOrder() As Integer Implements IModuleInfo.ModuleOrder
    '        Get
    '            LoadModule()
    '            Return m_ModuleOrder
    '            'Return currentPortalModuleBase.ModuleConfiguration.ModuleOrder
    '        End Get
    '        Set(ByVal value As Integer)

    '        End Set
    '    End Property

    '    Public Property ModuleTitle() As String Implements IModuleInfo.ModuleTitle
    '        Get
    '            LoadModule()
    '            Return m_ModuleTitle
    '            'Return currentPortalModuleBase.ModuleConfiguration.ModuleTitle
    '        End Get
    '        Set(ByVal value As String)
    '            m_ModuleTitle = value
    '        End Set
    '    End Property

    '    Public Property PaneName() As String Implements IModuleInfo.PaneName
    '        Get
    '            LoadModule()
    '            Return m_PaneName
    '            'Return currentPortalModuleBase.ModuleConfiguration.PaneName
    '        End Get
    '        Set(ByVal value As String)
    '            m_PaneName = value
    '        End Set
    '    End Property

    '    Public Property FriendlyName() As String Implements IModuleInfo.FriendlyName
    '        Get
    '            LoadModule()
    '            Return m_FriendlyName
    '            'Return currentPortalModuleBase.ModuleConfiguration.FriendlyName
    '        End Get
    '        Set(ByVal value As String)
    '            m_FriendlyName = value
    '        End Set
    '    End Property

    '    Public Property Header() As String Implements IModuleInfo.Header
    '        Get
    '            LoadModule()
    '            Return m_Header
    '            'Return currentPortalModuleBase.ModuleConfiguration.Header
    '        End Get
    '        Set(ByVal value As String)
    '            m_Header = value
    '        End Set
    '    End Property

    '    Public Property Footer() As String Implements IModuleInfo.Footer
    '        Get
    '            LoadModule()
    '            Return m_Footer
    '            'Return currentPortalModuleBase.ModuleConfiguration.Footer
    '        End Get
    '        Set(ByVal value As String)
    '            m_Footer = value
    '        End Set
    '    End Property

    '    Public Property TabId() As String Implements IModuleInfo.TabId
    '        Get
    '            LoadModule()
    '            Return m_TabId
    '            'Return CStr(currentPortalModuleBase.ModuleConfiguration.TabID)
    '        End Get
    '        Set(ByVal value As String)
    '            m_TabId = value
    '        End Set
    '    End Property

    '    Public Property IsDeleted() As Boolean Implements IModuleInfo.IsDeleted
    '        Get
    '            LoadModule()
    '            Return m_IsDeleted
    '            'Return currentPortalModuleBase.ModuleConfiguration.IsDeleted
    '        End Get
    '        Set(ByVal value As Boolean)
    '            m_IsDeleted = value
    '        End Set
    '    End Property

    '    Public Property ModuleDefId() As String Implements IModuleInfo.ModuleDefId
    '        Get
    '            LoadModule()
    '            Return m_ModuleDefId
    '            'Return CStr(currentPortalModuleBase.ModuleConfiguration.ModuleDefID)
    '        End Get
    '        Set(ByVal value As String)
    '            m_ModuleDefId = value
    '        End Set
    '    End Property

    '    Public Property PortalId() As String Implements IModuleInfo.PortalId
    '        Get
    '            LoadModule()
    '            Return m_PortalId
    '            'Return CStr(currentPortalModuleBase.PortalId)
    '        End Get
    '        Set(ByVal value As String)
    '            m_PortalId = value
    '        End Set
    '    End Property

    '    Public Property InheritViewPermissions() As Boolean Implements IModuleInfo.InheritViewPermissions
    '        Get
    '            LoadModule()
    '            Return m_InheritViewPermissions
    '            'Return currentPortalModuleBase.ModuleConfiguration.InheritViewPermissions
    '        End Get
    '        Set(ByVal value As Boolean)
    '            m_InheritViewPermissions = value
    '        End Set
    '    End Property

    '    Public Property ModulePermissions() As IModulePermissionCollection Implements IModuleInfo.ModulePermissions
    '        Get
    '            Return m_ModulePermissions
    '            'TODO: GET the Module Permission
    '        End Get
    '        Set(ByVal value As IModulePermissionCollection)
    '            m_ModulePermissions = value
    '        End Set
    '    End Property

    '    Public Property Description() As String Implements IModuleInfo.Description
    '        Get
    '            LoadModule()
    '            Return m_Description
    '            'Return currentPortalModuleBase.ModuleConfiguration.Description
    '        End Get
    '        Set(ByVal value As String)
    '            m_Description = value
    '        End Set
    '    End Property

    '    Public Property AuthorizedViewRoles() As String Implements IModuleInfo.AuthorizedViewRoles
    '        Get
    '            LoadModule()
    '            Return m_AuthorizedViewRoles
    '            'Return currentPortalModuleBase.ModuleConfiguration.AuthorizedViewRoles
    '        End Get
    '        Set(ByVal value As String)
    '            m_AuthorizedViewRoles = value
    '        End Set
    '    End Property


    '    Private Sub LoadModule()
    '        If (Not moduleInfoTotallyLoaded) Then
    '            Dim moduleInfo As DotNetNuke.Entities.Modules.ModuleInfo = Nothing
    '            If Not currentPortalModuleBase Is Nothing Then
    '                moduleInfo = currentPortalModuleBase.ModuleConfiguration
    '            ElseIf Not m_ModuleId Is Nothing AndAlso IsNumeric(m_ModuleId) AndAlso Not m_TabId Is Nothing AndAlso IsNumeric(m_TabId) Then
    '                moduleInfo = (New DotNetNuke.Entities.Modules.ModuleController).GetModule(CInt(m_ModuleId), CInt(m_TabId))
    '            End If
    '            If Not moduleInfo Is Nothing Then
    '                m_ModuleId = CStr(moduleInfo.ModuleID)
    '                m_ModuleOrder = moduleInfo.ModuleOrder
    '                m_ModuleTitle = moduleInfo.ModuleTitle
    '                m_PaneName = moduleInfo.PaneName
    '                m_FriendlyName = moduleInfo.FriendlyName
    '                m_Header = moduleInfo.Header
    '                m_Footer = moduleInfo.Footer
    '                m_TabId = CStr(moduleInfo.TabID)
    '                m_IsDeleted = moduleInfo.IsDeleted
    '                m_ModuleDefId = CStr(moduleInfo.ModuleDefID)
    '                m_PortalId = CStr(moduleInfo.PortalID)
    '                m_InheritViewPermissions = moduleInfo.InheritViewPermissions
    '                m_Description = moduleInfo.Description
    '                m_AuthorizedViewRoles = moduleInfo.AuthorizedViewRoles
    '                Dim curModulePermissions As New ModulePermissionCollection
    '                Dim dnnModulePermissions As DotNetNuke.Security.Permissions.ModulePermissionCollection = moduleInfo.ModulePermissions
    '                If Not dnnModulePermissions Is Nothing Then
    '                    For Each dnnPermissionInfo As DotNetNuke.Security.Permissions.ModulePermissionInfo In dnnModulePermissions
    '                        Dim curModulePermissionInfo As New ModulePermissionInfo
    '                        curModulePermissionInfo.AllowAccess = dnnPermissionInfo.AllowAccess
    '                        curModulePermissionInfo.ModuleId = CStr(dnnPermissionInfo.ModuleID)
    '                        curModulePermissionInfo.PermissionCode = dnnPermissionInfo.PermissionCode
    '                        curModulePermissionInfo.PermissionId = CStr(dnnPermissionInfo.PermissionID)
    '                        curModulePermissionInfo.PermissionKey = dnnPermissionInfo.PermissionKey
    '                        curModulePermissionInfo.PermissionName = dnnPermissionInfo.PermissionName
    '                        curModulePermissionInfo.RoleId = CStr(dnnPermissionInfo.RoleID)
    '                        curModulePermissions.Add(curModulePermissionInfo)
    '                    Next
    '                End If
    '                m_ModulePermissions = curModulePermissions
    '            End If
    '        End If
    '        moduleInfoTotallyLoaded = True
    '    End Sub
    'End Class
End Namespace

