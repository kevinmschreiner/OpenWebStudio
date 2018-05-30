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

Namespace DataAccess
    Public Class TabInfo
        Implements ITabInfo
        Private _obj As DotNetNuke.Entities.Tabs.TabInfo
        Public Sub New()
            _obj = New DotNetNuke.Entities.Tabs.TabInfo()

        End Sub
        Public Sub New(ByVal src As DotNetNuke.Entities.Tabs.TabInfo)
            _obj = src
            If _obj Is Nothing Then
                _obj = New DotNetNuke.Entities.Tabs.TabInfo()
            End If
        End Sub
        Public Property AdministratorRoles() As String Implements Framework.DataAccess.ITabInfo.AdministratorRoles
            Get
                Return _obj.TabPermissions.ToString("EDIT")
            End Get
            Set(ByVal value As String)
                '_obj.TabPermissions = value
                _obj.TabPermissions.Clear()
                For Each role As String In value.Split(","c)
                    Dim t As DotNetNuke.Security.Permissions.TabPermissionInfo = New DotNetNuke.Security.Permissions.TabPermissionInfo()
                    t.AllowAccess = True
                    t.RoleName = value
                    t.PermissionKey = "EDIT"
                    t.TabID = Integer.Parse(Me.TabID)
                    t.PermissionID = 4
                    t.RoleID = 0
                    _obj.TabPermissions.Add(t)
                Next
            End Set
            Set(ByVal value As String)
                _obj.AdministratorRoles = value
            End Set
        End Property

        Public Property AuthorizedRoles() As String Implements Framework.DataAccess.ITabInfo.AuthorizedRoles
            Get
                Return _obj.AuthorizedRoles
            End Get
            Set(ByVal value As String)
                _obj.AuthorizedRoles = value
            End Set
        End Property

        Public Property BreadCrumbs() As System.Collections.ArrayList Implements Framework.DataAccess.ITabInfo.BreadCrumbs
            Get
                Return _obj.BreadCrumbs
            End Get
            Set(ByVal value As System.Collections.ArrayList)
                _obj.BreadCrumbs = value
            End Set
        End Property

        Public Property ContainerPath() As String Implements Framework.DataAccess.ITabInfo.ContainerPath
            Get
                Return _obj.ContainerPath
            End Get
            Set(ByVal value As String)
                _obj.ContainerPath = value
            End Set
        End Property

        Public Property ContainerSrc() As String Implements Framework.DataAccess.ITabInfo.ContainerSrc
            Get
                Return _obj.ContainerSrc
            End Get
            Set(ByVal value As String)
                _obj.ContainerSrc = value
            End Set
        End Property

        Public Property Description() As String Implements Framework.DataAccess.ITabInfo.Description
            Get
                Return _obj.Description
            End Get
            Set(ByVal value As String)
                _obj.Description = value
            End Set
        End Property

        Public Property DisableLink() As Boolean Implements Framework.DataAccess.ITabInfo.DisableLink
            Get
                Return _obj.DisableLink
            End Get
            Set(ByVal value As Boolean)
                _obj.DisableLink = value
            End Set
        End Property

        Public Property EndDate() As Date Implements Framework.DataAccess.ITabInfo.EndDate
            Get
                Return _obj.EndDate
            End Get
            Set(ByVal value As Date)
                _obj.EndDate = value
            End Set
        End Property

        Public ReadOnly Property FullUrl() As String Implements Framework.DataAccess.ITabInfo.FullUrl
            Get
                Return _obj.FullUrl
            End Get
        End Property

        Public Property HasChildren() As Boolean Implements Framework.DataAccess.ITabInfo.HasChildren
            Get
                Return _obj.HasChildren
            End Get
            Set(ByVal value As Boolean)
                _obj.HasChildren = value
            End Set
        End Property

        Public Property IconFile() As String Implements Framework.DataAccess.ITabInfo.IconFile
            Get
                Return _obj.IconFile
            End Get
            Set(ByVal value As String)
                _obj.IconFile = value
            End Set
        End Property

        Public ReadOnly Property IsAdminTab() As Boolean Implements Framework.DataAccess.ITabInfo.IsAdminTab
            Get
                Return _obj.IsSuperTab
            End Get
        End Property

        Public Property IsDeleted() As Boolean Implements Framework.DataAccess.ITabInfo.IsDeleted
            Get
                Return _obj.IsDeleted
            End Get
            Set(ByVal value As Boolean)
                _obj.IsDeleted = value
            End Set
        End Property

        Public Property IsSuperTab() As Boolean Implements Framework.DataAccess.ITabInfo.IsSuperTab
            Get
                Return _obj.IsSuperTab
            End Get
            Set(ByVal value As Boolean)
                _obj.IsSuperTab = value
            End Set
        End Property

        Public Property IsVisible() As Boolean Implements Framework.DataAccess.ITabInfo.IsVisible
            Get
                Return _obj.IsVisible
            End Get
            Set(ByVal value As Boolean)
                _obj.IsVisible = value
            End Set
        End Property

        Public Property KeyWords() As String Implements Framework.DataAccess.ITabInfo.KeyWords
            Get
                Return _obj.KeyWords
            End Get
            Set(ByVal value As String)
                _obj.KeyWords = value
            End Set
        End Property

        Public Property Level() As Integer Implements Framework.DataAccess.ITabInfo.Level
            Get
                Return _obj.Level
            End Get
            Set(ByVal value As Integer)
                _obj.Level = value
            End Set
        End Property

        Public Sub Load(ByVal obj As Object) Implements Framework.DataAccess.ITabInfo.Load
            _obj = CType(obj, DotNetNuke.Entities.Tabs.TabInfo)
        End Sub

        Public Function Save() As Object Implements Framework.DataAccess.ITabInfo.Save
            Return _obj
        End Function

        Public Property Modules() As System.Collections.ArrayList Implements Framework.DataAccess.ITabInfo.Modules
            Get
                Return _obj.Modules
            End Get
            Set(ByVal value As System.Collections.ArrayList)
                _obj.Modules = value
            End Set
        End Property

        Public Property PageHeadText() As String Implements Framework.DataAccess.ITabInfo.PageHeadText
            Get
                Return _obj.PageHeadText
            End Get
            Set(ByVal value As String)
                _obj.PageHeadText = value
            End Set
        End Property

        Public Property Panes() As System.Collections.ArrayList Implements Framework.DataAccess.ITabInfo.Panes
            Get
                Return _obj.Panes
            End Get
            Set(ByVal value As System.Collections.ArrayList)
                _obj.Panes = value
            End Set
        End Property

        Public Property ParentId() As String Implements Framework.DataAccess.ITabInfo.ParentId
            Get
                Return CStr(_obj.ParentId)
            End Get
            Set(ByVal value As String)
                _obj.ParentId = CInt(value)
            End Set
        End Property

        Public Property PortalID() As String Implements Framework.DataAccess.ITabInfo.PortalID
            Get
                Return CStr(_obj.PortalID)
            End Get
            Set(ByVal value As String)
                _obj.PortalID = CInt(value)
            End Set
        End Property

        Public Property RefreshInterval() As Integer Implements Framework.DataAccess.ITabInfo.RefreshInterval
            Get
                Return _obj.RefreshInterval
            End Get
            Set(ByVal value As Integer)
                _obj.RefreshInterval = value
            End Set
        End Property

        Public Property SkinPath() As String Implements Framework.DataAccess.ITabInfo.SkinPath
            Get
                Return _obj.SkinPath
            End Get
            Set(ByVal value As String)
                _obj.SkinPath = value
            End Set
        End Property

        Public Property SkinSrc() As String Implements Framework.DataAccess.ITabInfo.SkinSrc
            Get
                Return _obj.SkinSrc
            End Get
            Set(ByVal value As String)
                _obj.SkinSrc = value
            End Set
        End Property

        Public Property StartDate() As Date Implements Framework.DataAccess.ITabInfo.StartDate
            Get
                Return _obj.StartDate
            End Get
            Set(ByVal value As Date)
                _obj.StartDate = value
            End Set
        End Property

        Public Property TabID() As String Implements Framework.DataAccess.ITabInfo.TabID
            Get
                Return CStr(_obj.TabID)
            End Get
            Set(ByVal value As String)
                If Not value Is Nothing AndAlso value.Length > 0 Then
                    _obj.TabID = CInt(value)
                Else
                    _obj.TabID = -1
                End If
            End Set
        End Property

        Public Property TabOrder() As Integer Implements Framework.DataAccess.ITabInfo.TabOrder
            Get
                Return _obj.TabOrder
            End Get
            Set(ByVal value As Integer)
                _obj.TabOrder = value
            End Set
        End Property

        Public Property TabPath() As String Implements Framework.DataAccess.ITabInfo.TabPath
            Get
                Return _obj.TabPath
            End Get
            Set(ByVal value As String)
                _obj.TabPath = value
            End Set
        End Property


        Public Property TabName() As String Implements Framework.DataAccess.ITabInfo.TabName
            Get
                Return _obj.TabName
            End Get
            Set(ByVal value As String)
                _obj.TabName = value
            End Set
        End Property


        Public Property TabPermissions() As Framework.DataAccess.ITabPermissionCollection Implements Framework.DataAccess.ITabInfo.TabPermissions
            Get
                Return (New DataAccess.TabPermissionCollection(_obj.TabPermissions))
            End Get
            Set(ByVal value As Framework.DataAccess.ITabPermissionCollection)
                _obj.TabPermissions = CType(CType(value, DataAccess.TabPermissionCollection).Save, DotNetNuke.Security.Permissions.TabPermissionCollection)
            End Set
        End Property

        Public Property Title() As String Implements Framework.DataAccess.ITabInfo.Title
            Get
                Return _obj.Title
            End Get
            Set(ByVal value As String)
                _obj.Title = value
            End Set
        End Property

        Public Property Url() As String Implements Framework.DataAccess.ITabInfo.Url
            Get
                Return _obj.Url
            End Get
            Set(ByVal value As String)
                _obj.Url = value
            End Set
        End Property

    End Class
    'Public Class TabInfo
    '    Implements ITabInfo
    '    Implements IPersistent

    '    Private m_IsAdminTab As Boolean
    '    Private m_IsDeleted As Boolean
    '    Private m_IsSuperTab As Boolean
    '    Private m_Level As Integer
    '    Private m_TabId As String
    '    Private m_TabName As String
    '    Private m_ContainerSrc As String
    '    Private m_SkinSrc As String
    '    Private m_ParentId As String
    '    Private m_PortalID As String
    '    Private m_AdministratorRoles As String
    '    'Private _TabPermissions As TabPermissionInfo
    '    Private m_FullUrl As String
    '    Private m_Description As String
    '    Private m_TabTitle As String

    '    Private tabInfoTotallyLoaded As Boolean
    '    Private dnnTabInfo As DotNetNuke.Entities.Tabs.TabInfo
    '    'Protected ctx As HttpContext = HttpContext.Current
    '    'Private currentPortalModuleBase As BaseParentControl


    '    Public Sub New()
    '        AbstractFactory.Instance.PortalSettingsController.GetPortalSettings()
    '        'Try
    '        '    currentPortalModuleBase = DnnSingleton.GetInstance(ctx).CurrentModuleBase
    '        'Catch ex As Exception

    '        'End Try
    '    End Sub
    '    Public Sub New(ByVal source As DotNetNuke.Entities.Tabs.TabInfo)
    '        dnnTabInfo = source
    '        'Try
    '        '    currentPortalModuleBase = DnnSingleton.GetInstance(ctx).CurrentModuleBase
    '        'Catch ex As Exception

    '        'End Try
    '        LoadTabInfo()
    '    End Sub

    '    Public Property BaseObject() As DotNetNuke.Entities.Tabs.TabInfo
    '        Get
    '            Return dnnTabInfo
    '        End Get
    '        Set(ByVal value As DotNetNuke.Entities.Tabs.TabInfo)
    '            dnnTabInfo = value
    '        End Set
    '    End Property
    '    Public Property IsAdminTab() As Boolean Implements ITabInfo.IsAdminTab
    '        Get
    '            LoadTabInfo()
    '            Return m_IsAdminTab
    '        End Get
    '        Set(ByVal value As Boolean)
    '            m_IsAdminTab = value
    '        End Set
    '    End Property

    '    Public Property IsDeleted() As Boolean Implements ITabInfo.IsDeleted
    '        Get
    '            LoadTabInfo()
    '            Return m_IsDeleted
    '        End Get
    '        Set(ByVal value As Boolean)
    '            m_IsDeleted = value
    '        End Set
    '    End Property

    '    Public Property IsSuperTab() As Boolean Implements ITabInfo.IsSuperTab
    '        Get
    '            LoadTabInfo()
    '            Return m_IsSuperTab
    '        End Get
    '        Set(ByVal value As Boolean)
    '            m_IsSuperTab = value
    '        End Set
    '    End Property

    '    Public Property Level() As Integer Implements ITabInfo.Level
    '        Get
    '            LoadTabInfo()
    '            Return m_Level
    '        End Get
    '        Set(ByVal value As Integer)
    '            m_Level = value
    '        End Set
    '    End Property

    '    Public Property TabId() As String Implements ITabInfo.TabId, IPersistent.Id
    '        Get
    '            LoadTabInfo()
    '            Return m_TabId
    '        End Get
    '        Set(ByVal value As String)
    '            m_TabId = value
    '        End Set
    '    End Property

    '    Public Property TabName() As String Implements ITabInfo.TabName
    '        Get
    '            LoadTabInfo()
    '            Return m_TabName
    '        End Get
    '        Set(ByVal value As String)
    '            m_TabName = value
    '        End Set
    '    End Property

    '    Public Property ContainerSrc() As String Implements ITabInfo.ContainerSrc
    '        Get
    '            LoadTabInfo()
    '            m_ContainerSrc = dnnTabInfo.ContainerSrc
    '            Return m_ContainerSrc
    '        End Get
    '        Set(ByVal value As String)
    '            m_ContainerSrc = value
    '        End Set
    '    End Property

    '    Public Property SkinSrc() As String Implements ITabInfo.SkinSrc
    '        Get
    '            LoadTabInfo()
    '            m_SkinSrc = dnnTabInfo.SkinSrc
    '            Return m_SkinSrc
    '        End Get
    '        Set(ByVal value As String)
    '            m_SkinSrc = value
    '        End Set
    '    End Property

    '    Public Property ParentId() As String Implements ITabInfo.ParentId
    '        Get
    '            LoadTabInfo()
    '            m_ParentId = CStr(dnnTabInfo.ParentId)
    '            Return m_ParentId
    '        End Get
    '        Set(ByVal value As String)
    '            m_ParentId = value
    '        End Set
    '    End Property

    '    Public Property PortalId() As String Implements ITabInfo.PortalId
    '        Get
    '            LoadTabInfo()
    '            If m_PortalID Is Nothing OrElse m_PortalID = "-1" Then
    '                m_PortalID = CStr(dnnTabInfo.PortalID)
    '            End If
    '            Return m_PortalID
    '        End Get
    '        Set(ByVal value As String)
    '            m_PortalID = value
    '        End Set
    '    End Property

    '    Public Property TabPermissions() As ITabPermissionCollection Implements ITabInfo.TabPermissions
    '        Get
    '            LoadTabInfo()
    '            'TODO: Implements TabPermissions
    '            'Return _TabPermissions
    '            Return Nothing
    '        End Get
    '        Set(ByVal value As ITabPermissionCollection)

    '        End Set
    '    End Property

    '    Public Property FullUrl() As String Implements ITabInfo.FullUrl
    '        Get
    '            LoadTabInfo()
    '            Return m_FullUrl
    '        End Get
    '        Set(ByVal value As String)
    '            m_FullUrl = value
    '        End Set
    '    End Property

    '    Public Property AdministratorRoles() As String Implements ITabInfo.AdministratorRoles
    '        Get
    '            LoadTabInfo()
    '            Return m_AdministratorRoles
    '        End Get
    '        Set(ByVal value As String)
    '            m_AdministratorRoles = value
    '        End Set
    '    End Property
    '    Public Function GetDNNTab() As DotNetNuke.Entities.Tabs.TabInfo
    '        Dim ti As New DotNetNuke.Entities.Tabs.TabInfo
    '        ti.AdministratorRoles = Me.AdministratorRoles
    '        ti.ContainerSrc = Me.ContainerSrc
    '        ti.Description = Me.Description
    '        ti.IsDeleted = Me.IsDeleted
    '        ti.IsSuperTab = Me.IsSuperTab
    '        ti.Level = Me.Level
    '        ti.ParentId = CInt(Me.ParentId)
    '        ti.PortalID = CInt(Me.PortalId)
    '        ti.SkinSrc = Me.SkinSrc
    '        If Me.TabId Is Nothing Then
    '            ti.TabID = -1
    '        Else
    '            ti.TabID = CInt(Me.TabId)
    '        End If
    '        ti.TabName = Me.TabName
    '        ti.Title = Me.Title
    '        Return ti
    '    End Function
    '    Private Sub LoadTabInfo()
    '        If (Not tabInfoTotallyLoaded AndAlso m_TabId Is Nothing) Then

    '            If dnnTabInfo Is Nothing Then
    '                Dim dnnTabInfoCtrl As New DotNetNuke.Entities.Tabs.TabController
    '                Dim currentTabId As Integer
    '                Dim currentPortalId As Integer

    '                'If Not currentPortalModuleBase Is Nothing Then
    '                '    currentTabId = currentPortalModuleBase.TabId
    '                '    currentPortalId = currentPortalModuleBase.PortalId
    '                'Else
    '                Dim dnnPortalSettings As DotNetNuke.Entities.Portals.PortalSettings = CType(System.Web.HttpContext.Current.Items("PortalSettings"), DotNetNuke.Entities.Portals.PortalSettings)
    '                currentTabId = dnnPortalSettings.ActiveTab.TabID
    '                currentPortalId = dnnPortalSettings.PortalId
    '                'End If

    '                dnnTabInfo = dnnTabInfoCtrl.GetTab(currentTabId) ', currentPortalId, False)
    '            End If
    '            'dnnTabInfo = dnnTabInfoCtrl.GetTab(CInt(_TabId))
    '            m_IsAdminTab = dnnTabInfo.IsAdminTab
    '            m_IsDeleted = dnnTabInfo.IsDeleted
    '            m_IsSuperTab = dnnTabInfo.IsSuperTab
    '            m_Level = dnnTabInfo.Level
    '            m_TabId = CStr(dnnTabInfo.TabID)
    '            m_TabName = dnnTabInfo.TabName
    '            Try
    '                If Not dnnTabInfo Is Nothing AndAlso Not dnnTabInfo.FullUrl Is Nothing Then
    '                    m_FullUrl = dnnTabInfo.FullUrl
    '                End If
    '            Catch ex As Exception
    '            End Try
    '            m_AdministratorRoles = dnnTabInfo.AdministratorRoles
    '            '_TabPermissions = dnnTabInfo.TabPermissions
    '            m_Description = dnnTabInfo.Description
    '            m_TabTitle = dnnTabInfo.Title

    '            tabInfoTotallyLoaded = True
    '        End If
    '    End Sub


    '    Public Property IsTotallyLoaded() As Boolean Implements IPersistent.IsTotallyLoaded
    '        Get
    '            Return tabInfoTotallyLoaded
    '        End Get
    '        Set(ByVal value As Boolean)
    '            tabInfoTotallyLoaded = value
    '        End Set
    '    End Property

    '    Public Property Description() As String Implements ITabInfo.Description
    '        Get
    '            LoadTabInfo()
    '            Return m_Description
    '        End Get
    '        Set(ByVal value As String)
    '            m_Description = value
    '        End Set
    '    End Property

    '    Public Property Title() As String Implements ITabInfo.Title
    '        Get
    '            LoadTabInfo()
    '            Return m_TabTitle
    '        End Get
    '        Set(ByVal value As String)
    '            m_TabTitle = value
    '        End Set
    '    End Property

    '    Public ReadOnly Property PageProperties() As System.Collections.Generic.IDictionary(Of String, Object) Implements ITabInfo.PageProperties
    '        Get

    '        End Get
    '    End Property
    'End Class
End Namespace
