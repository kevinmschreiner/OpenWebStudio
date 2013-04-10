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
Imports System.Reflection

Namespace r2i.OWS.Framework.Entities
    Public MustInherit Class AbstractFactory
        'FROM Initialize - Call the assignment of the Factory from the Wrapper
        Protected Shared s_instance As AbstractFactory
        Public MustOverride Sub Initialize()
        Public Shared Function Initialized() As Boolean
            If s_instance Is Nothing Then
                Return False
            End If
            Return True
        End Function
        Public Shared Property Instance() As AbstractFactory
            Get
                If s_instance Is Nothing Then
                    Throw New Exception("You must assign the Shared Factory from the wrapper via the write-only Factory property.")
                Else
                    Return s_instance
                End If
            End Get
            Set(ByVal value As AbstractFactory)
                s_instance = value
            End Set
        End Property

        Public MustOverride ReadOnly Property ConfigurationController() As IConfigurationController

        Public MustOverride ReadOnly Property UserController() As IUserController

        Public MustOverride ReadOnly Property PortalModuleBaseUIController() As IPortalModuleBaseUIController

        Public MustOverride ReadOnly Property PortalSettingsController() As IPortalSettingsController

        Public MustOverride ReadOnly Property EngineController() As IEngineController

        Public MustOverride ReadOnly Property SecurityController() As ISecurityController

        Public MustOverride ReadOnly Property DataController() As IDataController

        Public MustOverride ReadOnly Property RoleController() As IRoleController

        Public MustOverride ReadOnly Property ModuleController() As IModuleController


        Public MustOverride ReadOnly Property DesignController() As IDesignerController

        Public MustOverride ReadOnly Property TabController() As ITabController

        Public MustOverride ReadOnly Property DeliveryController() As IDeliveryController

        Public MustOverride ReadOnly Property TabPermissionController() As ITabPermissionController

        Public MustOverride ReadOnly Property ModulePermissionController() As IModulePermissionController

        Public MustOverride ReadOnly Property DesktopModuleController() As IDesktopModuleController

        Public MustOverride ReadOnly Property ModuleDefinitionController() As IModuleDefinitionController

        Public MustOverride ReadOnly Property ProviderConfigurationController() As IProviderConfigurationController

        'Public MustOverride ReadOnly Property CDefaultController() As ICDefaultController
        Public MustOverride ReadOnly Property PageInfoController() As IPageInfoController

        Public MustOverride ReadOnly Property ProviderController() As IProviderController

        Public MustOverride ReadOnly Property ModuleActionController() As IModuleActionController

        Public MustOverride ReadOnly Property SkinController() As ISkinController

        Public MustOverride ReadOnly Property ModuleControlInfoController() As IModuleControlInfoController

        Public MustOverride ReadOnly Property OWSModuleController() As IOWSModuleController

        Public MustOverride ReadOnly Property InstallerController() As IInstallerController

        Public MustOverride ReadOnly Property LogController() As ILogController

    End Class
End Namespace