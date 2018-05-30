Namespace DataAccess.Factories
    Public Class TabPermissionFactory
        Private Shared _instance As TabPermissionFactory = New TabPermissionFactory()

        Public Shared ReadOnly Property Instance() As TabPermissionFactory
            Get
                Return _instance
            End Get
        End Property

        Public Function CreateNewTabPermissionInfo() As DataAccess.TabPermissionInfo
            Return New DataAccess.TabPermissionInfo
        End Function

        Public Function GetTabPermissionsCollectionByTabID(ByVal portalId As String, ByVal tabId As String) As DataAccess.TabPermissionCollection
            Dim tpCtrl As New DotNetNuke.Security.Permissions.TabPermissionController
            Dim tabIdConverted As Integer
            If Integer.TryParse(tabId, tabIdConverted) Then
                Return New DataAccess.TabPermissionCollection(DotNetNuke.Security.Permissions.TabPermissionController.GetTabPermissions(tabIdConverted, CInt(portalId)))
            Else
                Return Nothing
            End If
        End Function
    End Class
End Namespace

