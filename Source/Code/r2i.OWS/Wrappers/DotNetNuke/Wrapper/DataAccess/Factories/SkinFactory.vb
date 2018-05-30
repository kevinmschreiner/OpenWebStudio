Imports DotNetNuke
Imports System.Web

Namespace DataAccess.Factories
    Public Class SkinFactory
        Private Shared _instance As SkinFactory = New SkinFactory()

        Public Shared ReadOnly Property Instance() As SkinFactory
            Get
                Return _instance
            End Get
        End Property



        Public Shared Function GetParentSkinDnn() As ISkin
            'Dim ctx As HttpContext = HttpContext.Current
            'Dim currentPortalModuleBase As DotNetNuke.Entities.Modules.PortalModuleBase = DnnSingleton.GetInstance(ctx).CurrentPortalModuleBase
            'Dim _skin As New Skin
            'Dim ParentSkin As DotNetNuke.UI.Skins.Skin = DotNetNuke.UI.Skins.Skin.GetParentSkin(currentPortalModuleBase)
            '_skin.SkinId = ParentSkin.SkinID
            '_skin.SkinPath = ParentSkin.SkinPath
            'Return _skin
            Return Nothing
        End Function

        Public Sub ModuleAction_Click(ByVal sender As Object, ByVal e As DotNetNuke.Entities.Modules.Actions.ActionEventArgs)

            'We could get much fancier here by declaring each ModuleAction with a
            'Command and then using a Select Case statement to handle the various
            'commands.
            If e.Action.Url.Length > 0 Then
                'TODO: ToFix
                'Redirect(e.Action.Url, True)
            End If

        End Sub

        Public Function SkinType(ByVal type As String) As Integer
            If type = "Admin" Then
                Return DotNetNuke.UI.Skins.SkinType.Admin
            ElseIf type = "Portal" Then
                Return DotNetNuke.UI.Skins.SkinType.Portal
            End If
        End Function

        Public Shared Function GetSkin(ByVal SkinRoot As String, ByVal PortalId As String, ByVal SkinType As DotNetNuke.UI.Skins.SkinType) As ISkinInfo
            Dim dnnSkin As DotNetNuke.UI.Skins.SkinPackageInfo
            Dim iskin As New SkinInfo
            dnnSkin = DotNetNuke.UI.Skins.SkinController.GetSkinPackage(CInt(PortalId), SkinRoot, SkinType.ToString())
            If Not dnnSkin Is Nothing Then
                iskin.Id = CStr(dnnSkin.SkinPackageID)
                iskin.SkinSrc = dnnSkin.SkinName
            End If
            Return iskin
        End Function

        Public Function GetSkinFieldInfos() As System.Reflection.FieldInfo()
            Dim sp As Reflection.FieldInfo()
            sp = GetType(DotNetNuke.UI.Skins.Skin).GetFields(System.Reflection.BindingFlags.NonPublic Or _
                System.Reflection.BindingFlags.Instance Or _
                System.Reflection.BindingFlags.Public Or _
                System.Reflection.BindingFlags.IgnoreCase)
            Return sp
        End Function

        'Public Property ModuleConfiguration() As IModuleInfo Implements IOWS.ModuleConfiguration
        '    Get
        '        'Return _ModuleConfiguration
        '        Dim modInfo As New ModuleInfo
        '        modInfo.Description = currentPortalModuleBase.ModuleConfiguration.Description
        '        modInfo.Footer = currentPortalModuleBase.ModuleConfiguration.Footer
        '        modInfo.FriendlyName = currentPortalModuleBase.ModuleConfiguration.FriendlyName
        '        modInfo.Header = currentPortalModuleBase.ModuleConfiguration.Header
        '        modInfo.InheritViewPermissions = currentPortalModuleBase.ModuleConfiguration.InheritViewPermissions
        '        modInfo.IsDeleted = currentPortalModuleBase.ModuleConfiguration.IsDeleted
        '        modInfo.ModuleDefId = CStr(currentPortalModuleBase.ModuleConfiguration.ModuleDefID)
        '        modInfo.ModuleID = CStr(currentPortalModuleBase.ModuleConfiguration.ModuleID)
        '        modInfo.ModuleOrder = currentPortalModuleBase.ModuleConfiguration.ModuleOrder
        '        'TODO: Get ModulePermission
        '        'modInfo.ModulePermissions = CType(currentPortalModuleBase.ModuleConfiguration.ModulePermissions, IModulePermissionCollection)
        '        modInfo.ModuleTitle = currentPortalModuleBase.ModuleConfiguration.ModuleTitle
        '        modInfo.PaneName = currentPortalModuleBase.ModuleConfiguration.PaneName
        '        modInfo.PortalId = CStr(currentPortalModuleBase.ModuleConfiguration.PortalID)
        '        modInfo.TabId = CStr(currentPortalModuleBase.ModuleConfiguration.TabID)
        '        Return modInfo
        '    End Get
        '    Set(ByVal value As IModuleInfo)
        '        _ModuleConfiguration = value
        '    End Set
        'End Property
    End Class
End Namespace

