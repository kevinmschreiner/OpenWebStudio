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
Imports DotNetNuke

Namespace DataAccess
    Public Class PortalSettings
        Implements IPortalSettings

        Private Shared ReadOnly _Instance As PortalSettings = New PortalSettings()
        Public Shared ReadOnly Property Instance() As PortalSettings
            Get
                Return _Instance
            End Get
        End Property

        Public Sub New()
        End Sub
        Public Sub New(ByVal portalSettings As DotNetNuke.Entities.Portals.PortalSettings)
            portalSettingsTotallyLoaded = True
            _portalSettings = portalSettings
        End Sub

        'Private _PortalId As String
        'Private _PortalAliasID As String
        'Private _HttpAlias As String
        'Private _HomeDirectory As String
        'Private _PortalName As String
        Private _ActiveTab As ITabInfo
        'Private _AdministratorRoleName As String
        'Private _Version As String

        Private portalSettingsTotallyLoaded As Boolean
        Private _portalSettings As DotNetNuke.Entities.Portals.PortalSettings


        Public Property AdministratorId() As String Implements IPortalSettings.AdministratorId
            Get
                Return _portalSettings.AdministratorId.ToString
            End Get
            Set(ByVal value As String)

            End Set
        End Property
        Public Property AdminTabId() As String Implements IPortalSettings.AdminTabId
            Get
                Return _portalSettings.AdminTabId.ToString
            End Get
            Set(ByVal value As String)

            End Set
        End Property
        Public Property BackgroundFile() As String Implements IPortalSettings.BackgroundFile
            Get
                Return _portalSettings.BackgroundFile.ToString
            End Get
            Set(ByVal value As String)

            End Set
        End Property
        Public Property DefaultLanguage() As String Implements IPortalSettings.DefaultLanguage
            Get
                Return _portalSettings.DefaultLanguage.ToString
            End Get
            Set(ByVal value As String)

            End Set
        End Property
        Public Property Description() As String Implements IPortalSettings.Description
            Get
                Return _portalSettings.Description.ToString
            End Get
            Set(ByVal value As String)

            End Set
        End Property
        Public Property Email() As String Implements IPortalSettings.Email
            Get
                Return _portalSettings.Email.ToString
            End Get
            Set(ByVal value As String)

            End Set
        End Property
        Public Property ExpiryDate() As DateTime Implements IPortalSettings.ExpiryDate
            Get
                Return _portalSettings.ExpiryDate
            End Get
            Set(ByVal value As DateTime)

            End Set
        End Property
        Public Property FooterText() As String Implements IPortalSettings.FooterText
            Get
                Return _portalSettings.FooterText
            End Get
            Set(ByVal value As String)

            End Set
        End Property
        Public Property GUID() As String Implements IPortalSettings.GUID
            Get
                Return _portalSettings.GUID.ToString
            End Get
            Set(ByVal value As String)

            End Set
        End Property
        Public Property HomeDirectoryMapPath() As String Implements IPortalSettings.HomeDirectoryMapPath
            Get
                Return _portalSettings.HomeDirectoryMapPath
            End Get
            Set(ByVal value As String)

            End Set
        End Property
        Public Property HomeTabId() As String Implements IPortalSettings.HomeTabId
            Get
                Return _portalSettings.HomeTabId.ToString
            End Get
            Set(ByVal value As String)

            End Set
        End Property
        Public Property HostFee() As String Implements IPortalSettings.HostFee
            Get
                Return _portalSettings.HostFee.ToString
            End Get
            Set(ByVal value As String)

            End Set
        End Property
        Public Property HostSpace() As String Implements IPortalSettings.HostSpace
            Get
                Return _portalSettings.HostSpace.ToString
            End Get
            Set(ByVal value As String)

            End Set
        End Property
        Public Property Title() As String Implements IPortalSettings.Title
            Get
                Return _portalSettings.PortalName
            End Get
            Set(ByVal value As String)

            End Set
        End Property
        Public Property KeyWords() As String Implements IPortalSettings.KeyWords
            Get
                Return _portalSettings.KeyWords
            End Get
            Set(ByVal value As String)

            End Set
        End Property
        Public Property LoginTabId() As String Implements IPortalSettings.LoginTabId
            Get
                Return _portalSettings.LoginTabId.ToString
            End Get
            Set(ByVal value As String)

            End Set
        End Property
        Public Property LogoFile() As String Implements IPortalSettings.LogoFile
            Get
                Return _portalSettings.LogoFile
            End Get
            Set(ByVal value As String)

            End Set
        End Property
        Public Property UserTabId() As String Implements IPortalSettings.UserTabId
            Get
                Return _portalSettings.UserTabId.ToString
            End Get
            Set(ByVal value As String)

            End Set
        End Property
        Public Property RegisteredRoleId() As String Implements IPortalSettings.RegisteredRoleId
            Get
                Return _portalSettings.RegisteredRoleId.ToString
            End Get
            Set(ByVal value As String)

            End Set
        End Property
        Public Property UserRegistration() As String Implements IPortalSettings.UserRegistration
            Get
                Return _portalSettings.UserRegistration.ToString
            End Get
            Set(ByVal value As String)

            End Set
        End Property
        Public Property TimeZoneOffset() As Integer Implements IPortalSettings.TimeZoneOffset
            Get
                Return CType(_portalSettings.TimeZone.BaseUtcOffset.TotalHours, Integer)
            End Get
            Set(ByVal value As Integer)

            End Set
        End Property
        Public Property SuperTabId() As String Implements IPortalSettings.SuperTabId
            Get
                Return _portalSettings.SuperTabId.ToString
            End Get
            Set(ByVal value As String)

            End Set
        End Property
        Public Property SplashTabId() As String Implements IPortalSettings.SplashTabId
            Get
                Return _portalSettings.SplashTabId.ToString
            End Get
            Set(ByVal value As String)

            End Set
        End Property
        Public Property RegisteredRoleName() As String Implements IPortalSettings.RegisteredRoleName
            Get
                Return _portalSettings.RegisteredRoleName
            End Get
            Set(ByVal value As String)

            End Set
        End Property
        Public Property HomeDirectory() As String Implements IPortalSettings.HomeDirectory
            Get
                LoadPortalSettings()
                Return _portalSettings.HomeDirectory
            End Get
            Set(ByVal value As String)

            End Set
        End Property

        Public Property HTTPAlias() As String Implements IPortalSettings.HTTPAlias
            Get
                LoadPortalSettings()
                Return _portalSettings.PortalAlias.HTTPAlias
            End Get
            Set(ByVal value As String)

            End Set
        End Property

        Public Property PortalAliasID() As String Implements IPortalSettings.PortalAliasID
            Get
                LoadPortalSettings()
                Return CStr(_portalSettings.PortalAlias.PortalAliasID)
            End Get
            Set(ByVal value As String)

            End Set
        End Property

        Public Property PortalId() As String Implements IPortalSettings.PortalId
            Get
                LoadPortalSettings()
                Return CStr(_portalSettings.PortalId)
            End Get
            Set(ByVal value As String)

            End Set
        End Property

        Private Sub LoadPortalSettings()
            If (Not portalSettingsTotallyLoaded) Then
                _portalSettings = DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings()
                portalSettingsTotallyLoaded = True
            End If
        End Sub

        Public Property PortalName() As String Implements IPortalSettings.PortalName
            Get
                LoadPortalSettings()
                Return _portalSettings.PortalName
            End Get
            Set(ByVal value As String)

            End Set
        End Property

        Public ReadOnly Property portalSettingsProperty() As System.Collections.Generic.IDictionary(Of String, Object) Implements IPortalSettings.portalSettingsProperty
            Get
                Return Nothing
            End Get
        End Property


        Public Property ActiveTab() As ITabInfo Implements IPortalSettings.ActiveTab
            Get
                LoadPortalSettings()
                If _ActiveTab Is Nothing OrElse _ActiveTab.TabId <> _portalSettings.ActiveTab.TabID.ToString Then
                    _ActiveTab = New TabInfo(_portalSettings.ActiveTab)
                End If
                Return _ActiveTab
            End Get
            Set(ByVal value As ITabInfo)
                LoadPortalSettings()
                _ActiveTab = value
            End Set
        End Property


        Public Function GetPortalSettings() As PortalSettings
            Dim dnnPortalSettings As DotNetNuke.Entities.Portals.PortalSettings = CType(System.Web.HttpContext.Current.Items("PortalSettings"), DotNetNuke.Entities.Portals.PortalSettings)
            Dim currentPortalSettings As New PortalSettings(dnnPortalSettings)
            Return currentPortalSettings
        End Function

        Public Property AdministratorRoleName() As String Implements IPortalSettings.AdministratorRoleName
            Get
                LoadPortalSettings()
                Return _portalSettings.AdministratorRoleName
            End Get
            Set(ByVal value As String)

            End Set
        End Property

        Public Property Version() As String Implements IPortalSettings.Version
            Get
                LoadPortalSettings()
                Return _portalSettings.CdfVersion.ToString()
            End Get
            Set(ByVal value As String)

            End Set
        End Property

        Public Property AdministratorRoleId() As String Implements IPortalSettings.AdministratorRoleId
            Get
                LoadPortalSettings()
                Return CStr(_portalSettings.AdministratorRoleId)
            End Get
            Set(ByVal value As String)

            End Set
        End Property

        Public Function GetModuleSettings(ByVal moduleId As Integer) As System.Collections.Hashtable Implements IPortalSettings.GetModuleSettings
            'LoadPortalSettings()
            Dim lst As IList(Of DotNetNuke.Entities.Modules.ModuleInfo) = DotNetNuke.Entities.Modules.ModuleController.Instance.GetTabModulesByModule(moduleId)
            If Not lst Is Nothing AndAlso lst.Count > 0 Then
                Return lst(0).ModuleSettings
            End If
            Return Nothing
        End Function

        Public Function GetTabModuleSettings(ByVal tabModuleId As Integer, ByVal settings As System.Collections.Hashtable) As System.Collections.Hashtable Implements IPortalSettings.GetTabModuleSettings
            'Return DotNetNuke.Entities.Portals.PortalSettings.GetTabModuleSettings(tabModuleId, settings)
            Return DotNetNuke.Entities.Modules.ModuleController.Instance.GetTabModule(tabModuleId).TabModuleSettings
        End Function
    End Class
End Namespace

