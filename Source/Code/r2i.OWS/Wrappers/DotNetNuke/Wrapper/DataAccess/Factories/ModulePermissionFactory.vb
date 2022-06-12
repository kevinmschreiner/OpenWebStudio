Namespace DataAccess.Factories
    Public Class ModulePermissionFactory

        Private Shared _instance As ModulePermissionFactory = New ModulePermissionFactory()

        Public Shared ReadOnly Property Instance() As ModulePermissionFactory
            Get
                Return _instance
            End Get
        End Property

        Public Function CreateNewModulePermissionInfo() As DataAccess.ModulePermissionInfo
            Return New DataAccess.ModulePermissionInfo
        End Function

        Public Function GetModulePermissionsCollectionByModuleID(ByVal moduleId As String) As DataAccess.ModulePermissionCollection
            Dim tpCtrl As New DotNetNuke.Security.Permissions.ModulePermissionController
            Dim moduleIdConverted As Integer
            If Integer.TryParse(moduleId, moduleIdConverted) Then
                Dim tpc As New DataAccess.ModulePermissionCollection(tpCtrl.GetModulePermissionsCollectionByModuleID(CInt(moduleId)))
                Return tpc
                'DNN9: Dim tb As IList(Of DotNetNuke.Entities.Modules.ModuleInfo) = DotNetNuke.Entities.Modules.ModuleController.Instance.GetTabModulesByModule(moduleIdConverted)
                'If Not tb Is Nothing AndAlso tb.Count > 0 Then
                'Return New DataAccess.ModulePermissionCollection(tb(0).ModulePermissions)
                'End If
            Else
                Return Nothing
            End If
            Return Nothing
        End Function
    End Class
End Namespace


