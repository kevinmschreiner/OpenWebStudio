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
    Public Class Installer
        Implements IInstaller



        Protected ctx As HttpContext = HttpContext.Current
        Private currentModuleSettingsBase As DotNetNuke.Entities.Modules.ModuleSettingsBase ' = DnnSingleton.GetInstance(ctx).CurrentModuleSettingsBase

        Public Property ConfigurationId() As System.Guid Implements IInstaller.ConfigurationId
            Get

            End Get
            Set(ByVal value As System.Guid)

            End Set
        End Property

        Public Property ModuleId() As String Implements IInstaller.ModuleId
            Get
                Return CStr(currentModuleSettingsBase.ModuleId)
            End Get
            Set(ByVal value As String)

            End Set
        End Property

        Public ReadOnly Property ModulePath() As String Implements IInstaller.ModulePath
            Get
                Return currentModuleSettingsBase.ModulePath
            End Get
        End Property

        Public Property TabId() As String Implements IInstaller.PageId
            Get
                Return CStr(currentModuleSettingsBase.TabId)
            End Get
            Set(ByVal value As String)

            End Set
        End Property

        Public ReadOnly Property ClientId() As String Implements IInstaller.ClientId
            Get
                Return currentModuleSettingsBase.ClientID
            End Get
        End Property

        Public Property ActiveTabId() As String Implements IInstaller.ActiveTabId
            Get
                Return CStr(currentModuleSettingsBase.PortalSettings.ActiveTab.TabID)
            End Get
            Set(ByVal value As String)

            End Set
        End Property

        Public Property PortalId() As String Implements IInstaller.PortalId
            Get
                Return CStr(currentModuleSettingsBase.PortalId)
            End Get
            Set(ByVal value As String)

            End Set
        End Property

        Public Property PortalSettings() As IPortalSettings Implements IInstaller.PortalSettings
            Get
                Dim iPortSet As New PortalSettings
                Dim dnnPortSet As DotNetNuke.Entities.Portals.PortalSettings = currentModuleSettingsBase.PortalSettings
                If Not dnnPortSet Is Nothing Then
                    iPortSet.ActiveTab = New TabInfo(dnnPortSet.ActiveTab)
                    iPortSet.AdministratorRoleId = CStr(dnnPortSet.AdministratorRoleId)
                    iPortSet.AdministratorRoleName = dnnPortSet.AdministratorRoleName
                    iPortSet.HomeDirectory = dnnPortSet.HomeDirectory
                    iPortSet.HTTPAlias = dnnPortSet.PortalAlias.HTTPAlias
                    iPortSet.PortalAliasID = CStr(dnnPortSet.PortalAlias.PortalAliasID)
                    iPortSet.PortalId = CStr(dnnPortSet.PortalId)
                    iPortSet.PortalName = dnnPortSet.PortalName
                    'iPortSet.portalSettingsProperty dictionary with all the properties
                    iPortSet.Version = dnnPortSet.Version
                End If
                Return iPortSet
            End Get
            Set(ByVal value As IPortalSettings)

            End Set
        End Property

        Public Property Settings() As System.Collections.Hashtable Implements IInstaller.Settings
            Get
                Return currentModuleSettingsBase.Settings
            End Get
            Set(ByVal value As System.Collections.Hashtable)

            End Set
        End Property
    End Class
End Namespace
