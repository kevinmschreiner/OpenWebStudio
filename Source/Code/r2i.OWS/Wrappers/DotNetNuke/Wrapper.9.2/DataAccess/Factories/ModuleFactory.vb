Imports r2i.OWS

Namespace DataAccess.Factories
    Public Class ModuleFactory
        Private Shared _instance As ModuleFactory = New ModuleFactory()

        Public Shared ReadOnly Property Instance() As ModuleFactory
            Get
                Return _instance
            End Get
        End Property

        Public Function CreateNewModuleInfo() As ModuleInfo
            Return New ModuleInfo
        End Function

        Public Function GetModuleInfoProperties(ByVal mi As IModuleInfo) As System.Reflection.PropertyInfo()
            Return GetType(IModuleInfo).GetProperties()
        End Function

        Public Function GetModuleInfoProperty(ByVal mi As IModuleInfo, ByVal name As String) As System.Reflection.PropertyInfo
            Return GetType(IModuleInfo).GetProperty(name, Reflection.BindingFlags.IgnoreCase Or Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance Or Reflection.BindingFlags.Static)
        End Function
        Public Function GetTabModules(ByVal tabId As String, ByVal PortalId As String) As System.Collections.Generic.Dictionary(Of Integer, DotNetNuke.Entities.Modules.ModuleInfo)
            Dim tc As New DotNetNuke.Entities.Modules.ModuleController
            Dim tms As New System.Collections.Generic.Dictionary(Of Integer, DotNetNuke.Entities.Modules.ModuleInfo)
            Dim arr As Dictionary(Of Integer, DotNetNuke.Entities.Modules.ModuleInfo) = tc.GetTabModules(CInt(tabId))

            Dim mi As DotNetNuke.Entities.Modules.ModuleInfo
            For Each mi In arr.Values
                tms.Add(mi.TabModuleID, mi)
            Next
            Return tms
        End Function
        Public Function GetDesktopModuleByName(ByVal friendlyName As String) As DesktopModuleInfo
            Dim mdC As New DotNetNuke.Entities.Modules.DesktopModuleController
            'Dim dnnDeskModInfo As DotNetNuke.Entities.Modules.DesktopModuleInfo = mdC.GetDesktopModuleByName(friendlyName)
            Dim dnnDeskModInfo As DotNetNuke.Entities.Modules.DesktopModuleInfo = DotNetNuke.Entities.Modules.DesktopModuleController.GetDesktopModuleByFriendlyName(friendlyName)
            Dim deskModInfo As New DesktopModuleInfo
            deskModInfo.DesktopModuleID = CStr(dnnDeskModInfo.DesktopModuleID)
            deskModInfo.FriendlyName = dnnDeskModInfo.FriendlyName
            Return deskModInfo
        End Function

        Public Function GetModuleDefinitionByName(ByVal DesktopModuleID As String, ByVal FriendlyName As String) As IModuleDefinitionInfo
            Dim dmC As New DotNetNuke.Entities.Modules.Definitions.ModuleDefinitionController
            Dim desktopModuleIDConverted As Integer
            If Integer.TryParse(DesktopModuleID, desktopModuleIDConverted) Then
                'Dim dnnModDefInfo As DotNetNuke.Entities.Modules.Definitions.ModuleDefinitionInfo = dmC.GetModuleDefinitionByName(desktopModuleIDConverted, FriendlyName)
                Dim dnnModDefInfo As DotNetNuke.Entities.Modules.Definitions.ModuleDefinitionInfo = DotNetNuke.Entities.Modules.DesktopModuleController.GetDesktopModuleByFriendlyName(FriendlyName).ModuleDefinitions.Values.GetEnumerator().Current
                Dim modDefInfo As New ModuleDefinitionInfo
                modDefInfo.FriendlyName = dnnModDefInfo.FriendlyName
                modDefInfo.ModuleDefID = CStr(dnnModDefInfo.ModuleDefID)
                Return modDefInfo
            Else
                Return Nothing
            End If
        End Function
        Public Function GetModule(ByVal moduleId As String, ByVal tabId As String) As IModuleInfo
            Dim mdC As New DotNetNuke.Entities.Modules.ModuleController
            Dim moduleIdConverted As Integer
            Dim tabIdConverted As Integer
            If Integer.TryParse(moduleId, moduleIdConverted) AndAlso (Integer.TryParse(tabId, tabIdConverted)) Then
                Dim dnnModuleInfo As DotNetNuke.Entities.Modules.ModuleInfo = mdC.GetModule(moduleIdConverted, tabIdConverted)
                Dim modInfo As New ModuleInfo
                modInfo.AuthorizedViewRoles = dnnModuleInfo.ModulePermissions.ToString("EDIT")
                modInfo.Description = dnnModuleInfo.DesktopModule.Description
                modInfo.Footer = dnnModuleInfo.Footer
                modInfo.FriendlyName = dnnModuleInfo.DesktopModule.FriendlyName
                modInfo.Header = dnnModuleInfo.Header
                modInfo.InheritViewPermissions = dnnModuleInfo.InheritViewPermissions
                modInfo.IsDeleted = dnnModuleInfo.IsDeleted
                modInfo.ModuleDefId = CStr(dnnModuleInfo.ModuleDefID)
                modInfo.ModuleID = CStr(dnnModuleInfo.ModuleID)
                modInfo.ModuleOrder = dnnModuleInfo.ModuleOrder
                'modInfo.ModulePermissions = dnnModuleInfo.ModulePermissions
                modInfo.ModuleTitle = dnnModuleInfo.ModuleTitle
                modInfo.PaneName = dnnModuleInfo.PaneName
                modInfo.PortalId = CStr(dnnModuleInfo.PortalID)
                modInfo.TabId = CStr(dnnModuleInfo.TabID)
                Return modInfo
            Else
                Return Nothing
            End If
        End Function

        Public Function GetModule(ByVal configurationId As Guid) As IModuleInfo
            Dim mdC As New DotNetNuke.Entities.Modules.ModuleController
            Dim moduleIdConverted As Integer
            Dim tabIdConverted As Integer
            Dim tabmodId() As String = DataProvider.Instance().GetConfigNameByConfigurationId(configurationId)
            If Not tabmodId Is Nothing Then
                If (Integer.TryParse(tabmodId(0), tabIdConverted)) AndAlso Integer.TryParse(tabmodId(1), moduleIdConverted) Then
                    Dim dnnModuleInfo As DotNetNuke.Entities.Modules.ModuleInfo = mdC.GetModule(moduleIdConverted, tabIdConverted)
                    Dim modInfo As New ModuleInfo
                    modInfo.AuthorizedViewRoles = dnnModuleInfo.ModulePermissions.ToString("EDIT")
                    modInfo.Description = dnnModuleInfo.DesktopModule.Description
                    modInfo.Footer = dnnModuleInfo.Footer
                    modInfo.FriendlyName = dnnModuleInfo.DesktopModule.FriendlyName
                    modInfo.Header = dnnModuleInfo.Header
                    modInfo.InheritViewPermissions = dnnModuleInfo.InheritViewPermissions
                    modInfo.IsDeleted = dnnModuleInfo.IsDeleted
                    modInfo.ModuleDefId = CStr(dnnModuleInfo.ModuleDefID)
                    modInfo.ModuleID = CStr(dnnModuleInfo.ModuleID)
                    modInfo.ModuleOrder = dnnModuleInfo.ModuleOrder
                    'modInfo.ModulePermissions = dnnModuleInfo.ModulePermissions
                    modInfo.ModuleTitle = dnnModuleInfo.ModuleTitle
                    modInfo.PaneName = dnnModuleInfo.PaneName
                    modInfo.PortalId = CStr(dnnModuleInfo.PortalID)
                    modInfo.TabId = CStr(dnnModuleInfo.TabID)
                    Return modInfo
                Else
                    'TODO: Implements Exception configurationId wrong format
                    Return Nothing
                End If
            Else
                'TODO: Implements Exception unable to get the configname
                Return Nothing
            End If
        End Function

        Public Sub UpdateModule(ByVal modInfo As DataAccess.ModuleInfo)
            Dim mdC As New DotNetNuke.Entities.Modules.ModuleController
            mdC.UpdateModule(CType(modInfo.Save, DotNetNuke.Entities.Modules.ModuleInfo))
        End Sub
        Public Function AddModule(ByVal modInfo As DataAccess.ModuleInfo) As String
            Dim mdC As New DotNetNuke.Entities.Modules.ModuleController
            Return CType(mdC.AddModule(CType(modInfo.Save, DotNetNuke.Entities.Modules.ModuleInfo)), String)
        End Function

        Public Sub UpdateModuleSetting(ByVal moduleId As Integer, ByVal settingName As String, ByVal settingValue As String)
            Dim mdC As New DotNetNuke.Entities.Modules.ModuleController
            Dim moduleIdConverted As Integer
            If Integer.TryParse(CStr(moduleId), moduleIdConverted) Then
                mdC.UpdateModuleSetting(moduleId, settingName, settingValue)
            Else
                'TODO: Exception: Unable to convert
            End If
        End Sub
    End Class
End Namespace

