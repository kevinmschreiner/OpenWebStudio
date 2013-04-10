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
            Else
                Return Nothing
            End If
        End Function
    End Class
End Namespace

