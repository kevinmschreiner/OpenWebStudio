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

        Public Function GetTabPermissionsCollectionByTabID(ByVal tabId As String) As DataAccess.TabPermissionCollection
            Dim tpCtrl As New DotNetNuke.Security.Permissions.TabPermissionController
            Dim tabIdConverted As Integer
            If Integer.TryParse(tabId, tabIdConverted) Then
                Dim tpc As New DataAccess.TabPermissionCollection(tpCtrl.GetTabPermissionsCollectionByTabID(CInt(tabId)))
                Return tpc
            Else
                Return Nothing
            End If
        End Function
    End Class
End Namespace

