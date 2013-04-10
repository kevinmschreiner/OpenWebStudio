Namespace DataAccess.Factories
    Public Class TabFactory
        Private Shared _instance As TabFactory = New TabFactory()

        Public Shared ReadOnly Property Instance() As TabFactory
            Get
                Return _instance
            End Get
        End Property

        Public Function CreateNewTabInfo() As DataAccess.TabInfo
            Return New DataAccess.TabInfo(New DotNetNuke.Entities.Tabs.TabInfo)
        End Function

        Public Function GetTabs(ByVal portalId As String) As ArrayList
            Dim tC As New DotNetNuke.Entities.Tabs.TabController
            Dim portalIdConverted As Integer
            If Integer.TryParse(portalId, portalIdConverted) Then
                Dim tabs As ArrayList = tC.GetTabs(CInt(portalId))
                Dim i As Integer = 0
                For i = 0 To tabs.Count - 1
                    tabs(i) = New DataAccess.TabInfo(CType(tabs(i), DotNetNuke.Entities.Tabs.TabInfo))
                Next
                Return tabs
            Else : Return Nothing
            End If
        End Function

        Public Function GetTabInfoProperties(ByVal ti As DotNetNuke.Entities.Tabs.TabInfo) As System.Reflection.PropertyInfo()
            Return ti.GetType().GetProperties()
        End Function

        Public Function GetTabInfoProperty(ByVal ti As ITabInfo, ByVal name As String) As System.Reflection.PropertyInfo
            Return GetType(ITabInfo).GetProperty(name, Reflection.BindingFlags.IgnoreCase Or Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance Or Reflection.BindingFlags.Static)
        End Function

        Public Function GetTab(ByVal moduleParentId As String) As ITabInfo
            Dim tiC As New DotNetNuke.Entities.Tabs.TabController
            Dim moduleParentIdConvert As Integer
            If Integer.TryParse(moduleParentId, moduleParentIdConvert) Then
                Dim dnnTab As DotNetNuke.Entities.Tabs.TabInfo = tiC.GetTab(moduleParentIdConvert)
                Dim itab As New TabInfo(dnnTab)
                'itab.TabId = dnnTab.TabID.ToString
                'itab.AdministratorRoles = dnnTab.AdministratorRoles
                'itab.ContainerSrc = dnnTab.ContainerSrc
                'itab.FullUrl = dnnTab.FullUrl
                'itab.IsAdminTab = dnnTab.IsAdminTab
                'itab.IsDeleted = dnnTab.IsDeleted
                'itab.IsSuperTab = dnnTab.IsSuperTab
                'itab.Level = dnnTab.Level
                'itab.ParentId = dnnTab.ParentId.ToString
                'itab.PortalId = dnnTab.PortalID.ToString
                'itab.SkinSrc = dnnTab.SkinSrc
                'itab.TabName = dnnTab.TabName
                'itab.TabPermissions = dnnTab.TabPermissions
                'itab.IsTotallyLoaded = True
                Return itab
            Else
                Return Nothing
            End If

        End Function

        Public Function GetTabByName(ByVal tabName As String, ByVal portalId As String) As DataAccess.TabInfo
            Dim tiC As New DotNetNuke.Entities.Tabs.TabController
            Dim portalIdConverted As Integer
            If Integer.TryParse(portalId, portalIdConverted) Then
                Return New DataAccess.TabInfo(tiC.GetTabByName(tabName, portalIdConverted))
            Else : Return Nothing
            End If
        End Function

        Public Function GetTabByName(ByVal tabName As String, ByVal portalId As String, ByVal parentId As String) As DataAccess.TabInfo
            Dim tiC As New DotNetNuke.Entities.Tabs.TabController
            Dim portalIdConverted As Integer
            Dim parentIdConverted As Integer
            If Integer.TryParse(portalId, portalIdConverted) AndAlso Integer.TryParse(parentId, parentIdConverted) Then
                Return New DataAccess.TabInfo(tiC.GetTabByName(tabName, portalIdConverted, parentIdConverted))
            Else : Return Nothing
            End If
        End Function

        Public Function AddTab(ByVal tabInfo As DotNetNuke.Entities.Tabs.TabInfo) As Integer
            Dim tiC As New DotNetNuke.Entities.Tabs.TabController
            Return tiC.AddTab(tabInfo)
        End Function

        Public Sub UpdateTab(ByVal tabInfo As DotNetNuke.Entities.Tabs.TabInfo)
            Dim tiC As New DotNetNuke.Entities.Tabs.TabController
            tiC.UpdateTab(tabInfo)
        End Sub

        Public Function GetPortalTabs() As System.Collections.ArrayList
            Dim dnnPortalSettings As DotNetNuke.Entities.Portals.PortalSettings = CType(System.Web.HttpContext.Current.Items("PortalSettings"), DotNetNuke.Entities.Portals.PortalSettings)
            Return DotNetNuke.Common.GetPortalTabs(dnnPortalSettings.DesktopTabs, True, True)
        End Function
    End Class
End Namespace

