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
Namespace Entities
    Public Class WrapperFactory
        Inherits r2i.OWS.Framework.Entities.AbstractFactory

        Private ReadOnly configCtrl As IConfigurationController
        Private ReadOnly userCtrl As IUserController
        Private ReadOnly portalModuleBaseUICtrl As IPortalModuleBaseUIController
        Private ReadOnly portalSettingsCtrl As IPortalSettingsController
        Private ReadOnly engineCtrl As IEngineController
        Private ReadOnly securityCtrl As ISecurityController
        Private ReadOnly dataCtrl As IDataController
        Private ReadOnly roleCtrl As IRoleController
        Private ReadOnly moduleCtrl As IModuleController
        Private ReadOnly designCtrl As IDesignerController
        Private ReadOnly tabCtrl As ITabController
        Private ReadOnly deliveryCtrl As IDeliveryController
        Private ReadOnly tabPermissionCtrl As ITabPermissionController
        Private ReadOnly modulePermissionCtrl As IModulePermissionController
        Private ReadOnly desktopModuleCtrl As IDesktopModuleController
        Private ReadOnly moduleDefinitionCtrl As IModuleDefinitionController
        Private ReadOnly providerConfigurationCtrl As IProviderConfigurationController
        'Private ReadOnly cDefaultCtrl As ICDefaultController
        Private ReadOnly pageInfoCtrl As IPageInfoController
        Private ReadOnly providerCtrl As IProviderController
        Private ReadOnly moduleActionCtrl As IModuleActionController
        Private ReadOnly skinCtrl As ISkinController
        Private ReadOnly moduleControlInfoCtrl As IModuleControlInfoController
        Private ReadOnly owsModuleCtrl As IOWSModuleController 'IListXModuleController
        Private ReadOnly installerCtrl As IInstallerController
        Private ReadOnly logCtrl As ILogController

        Public Sub New()
            Try
                configCtrl = New ConfigurationController()
                userCtrl = New UserController()
                portalModuleBaseUICtrl = New PortalModuleBaseUIController()
                portalSettingsCtrl = New PortalSettingsController()
                engineCtrl = New EngineController()
                securityCtrl = New SecurityController()
                dataCtrl = New DataController()
                roleCtrl = New RoleController()
                moduleCtrl = New ModuleController()
                designCtrl = New DesignController()
                tabCtrl = New TabController()
                deliveryCtrl = New DeliveryController()
                tabPermissionCtrl = New TabPermissionController()
                modulePermissionCtrl = New ModulePermissionController()
                desktopModuleCtrl = New DesktopModuleController()
                moduleDefinitionCtrl = New ModuleDefinitionController()
                providerConfigurationCtrl = New ProviderConfigurationController()
                'cDefaultCtrl = New CDefaultController()
                pageInfoCtrl = New PageInfoController()
                providerCtrl = New ProviderController()
                moduleActionCtrl = New ModuleActionController()
                skinCtrl = New SkinController()
                moduleControlInfoCtrl = New ModuleControlInfoController()
                installerCtrl = New InstallerController()
                logCtrl = New LogController()
            Catch ex As Exception
                Throw New Exception("There was a fatal error loading the wrapper: ", ex)
            End Try
        End Sub
        Public Overloads Overrides ReadOnly Property ConfigurationController() As IConfigurationController
            Get
                Return configCtrl
            End Get
        End Property

        Public Overrides ReadOnly Property UserController() As IUserController
            Get
                Return userCtrl
            End Get
        End Property

        Public Overrides ReadOnly Property PortalModuleBaseUIController() As IPortalModuleBaseUIController
            Get
                Return portalModuleBaseUICtrl
            End Get
        End Property

        Public Overrides ReadOnly Property PortalSettingsController() As IPortalSettingsController
            Get
                Return portalSettingsCtrl
            End Get
        End Property

        Public Overrides ReadOnly Property EngineController() As IEngineController
            Get
                Return engineCtrl
            End Get
        End Property

        Public Overrides ReadOnly Property SecurityController() As ISecurityController
            Get
                Return securityCtrl
            End Get
        End Property

        Public Overrides ReadOnly Property DataController() As IDataController
            Get
                Return dataCtrl
            End Get
        End Property

        Public Overrides ReadOnly Property RoleController() As IRoleController
            Get
                Return roleCtrl
            End Get
        End Property

        Public Overrides ReadOnly Property DesignController() As IDesignerController
            Get
                Return designCtrl
            End Get
        End Property

        Public Overrides ReadOnly Property ModuleController() As IModuleController
            Get
                Return moduleCtrl
            End Get
        End Property

        Public Overrides ReadOnly Property TabController() As ITabController
            Get
                Return tabCtrl
            End Get
        End Property

        Public Overrides ReadOnly Property DeliveryController() As IDeliveryController
            Get
                Return deliveryCtrl
            End Get
        End Property

        Public Overrides ReadOnly Property TabPermissionController() As ITabPermissionController
            Get
                Return tabPermissionCtrl
            End Get
        End Property

        Public Overrides ReadOnly Property ModulePermissionController() As IModulePermissionController
            Get
                Return modulePermissionCtrl
            End Get
        End Property

        Public Overrides ReadOnly Property DesktopModuleController() As IDesktopModuleController
            Get
                Return desktopModuleCtrl
            End Get
        End Property

        Public Overrides ReadOnly Property ModuleDefinitionController() As IModuleDefinitionController
            Get
                Return moduleDefinitionCtrl
            End Get
        End Property

        Public Overrides ReadOnly Property ProviderConfigurationController() As IProviderConfigurationController
            Get
                Return providerConfigurationCtrl
            End Get
        End Property

        Public Overrides ReadOnly Property PageInfoController() As Framework.Entities.IPageInfoController
            Get
                Return pageInfoCtrl
            End Get
        End Property

        Public Overrides ReadOnly Property ProviderController() As IProviderController
            Get
                Return providerCtrl
            End Get
        End Property

        Public Overrides ReadOnly Property ModuleActionController() As IModuleActionController
            Get
                Return moduleActionCtrl
            End Get
        End Property

        Public Overrides ReadOnly Property SkinController() As ISkinController
            Get
                Return skinCtrl
            End Get
        End Property


        Public Overrides ReadOnly Property ModuleControlInfoController() As IModuleControlInfoController
            Get
                Return moduleControlInfoCtrl
            End Get
        End Property

        Public Overrides ReadOnly Property OWSModuleController() As IOWSModuleController
            Get
                Return owsModuleCtrl
            End Get
        End Property

        Public Overrides ReadOnly Property InstallerController() As IInstallerController
            Get
                Return installerCtrl
            End Get
        End Property

        Public Overrides ReadOnly Property LogController() As ILogController
            Get
                Return logCtrl
            End Get
        End Property

        Public Shared Sub Create()
            If Not Initialized() Then
                Dim f As New WrapperFactory
                f.Initialize()
            End If
        End Sub

        Public Overrides Sub Initialize()
            If Not Initialized() Then
                Dim rootpath As String = DotNetNuke.Common.ApplicationMapPath
                If Not rootpath.EndsWith("/") AndAlso Not rootpath.EndsWith("\") Then
                    rootpath &= "\"
                End If
                r2i.OWS.Framework.Config.Initialize(rootpath, rootpath)
                AbstractFactory.Instance = New WrapperFactory
            End If
        End Sub
    End Class
End Namespace

