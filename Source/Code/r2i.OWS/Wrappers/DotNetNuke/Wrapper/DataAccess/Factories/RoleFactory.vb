Namespace DataAccess.Factories
    Public Class RoleFactory
        Private Shared _instance As RoleFactory = New RoleFactory()

        Public Shared ReadOnly Property Instance() As RoleFactory
            Get
                Return _instance
            End Get
        End Property

        Public Function GetRolesByUser(ByVal UserId As String, ByVal PortalId As String) As String()
            Dim userIdConvert As Integer
            If Integer.TryParse(UserId, userIdConvert) Then
                Return DotNetNuke.Entities.Users.UserController.GetUserById(CInt(PortalId), CInt(UserId)).Roles
            Else : Return Nothing
            End If
        End Function

        Public Function GetRole(ByVal RoleID As String, ByVal PortalID As String) As DataAccess.Role
            Dim roleIdConvert As Integer
            If Integer.TryParse(RoleID, roleIdConvert) Then
                Dim roleControl As New DotNetNuke.Security.Roles.RoleController
                Return New DataAccess.Role(roleControl.GetRole(CInt(RoleID), CInt(PortalID)))
            Else
                Return Nothing
            End If
        End Function

        Public Function GetRoleByName(ByVal PortalId As String, ByVal RoleName As String) As DataAccess.Role
            Dim roleControl As New DotNetNuke.Security.Roles.RoleController
            Dim rolei As DotNetNuke.Security.Roles.RoleInfo = roleControl.GetRoleByName(CInt(PortalId), RoleName)
            If Not rolei Is Nothing Then
                Return New DataAccess.Role(rolei)
            Else
                Return Nothing
            End If
        End Function

        Public Function DeleteUserRole(ByVal PortalId As String, ByVal UserId As Integer, ByVal RoleId As Integer) As Boolean
            Dim UserIdConvert As Integer
            Dim RoleIdConvert As Integer
            If Integer.TryParse(CStr(UserId), UserIdConvert) AndAlso Integer.TryParse(CStr(RoleId), RoleIdConvert) Then
                Dim roleControl As New DotNetNuke.Security.Roles.RoleController
                RoleControllerInstance().UpdateUserRole(CInt(PortalId), UserIdConvert, RoleIdConvert, DotNetNuke.Security.Roles.RoleStatus.Disabled, False, False)
            Else
                Return False
            End If
        End Function

        Public Sub AddUserRole(ByVal PortalID As String, ByVal UserId As Integer, ByVal RoleId As Integer, ByVal ExpiryDate As Date)
            Dim UserIdConvert As Integer
            Dim RoleIdConvert As Integer
            If Integer.TryParse(CStr(UserId), UserIdConvert) AndAlso Integer.TryParse(CStr(RoleId), RoleIdConvert) Then
                Dim roleControl As New DotNetNuke.Security.Roles.RoleController
                roleControl.AddUserRole(CInt(PortalID), UserIdConvert, RoleIdConvert, ExpiryDate)
            End If
        End Sub

        Public Function GetPortalRoles(ByVal PortalId As String) As System.Collections.ArrayList
            Dim rC As New DotNetNuke.Security.Roles.RoleController
            Dim arr As ArrayList = rC.GetPortalRoles(CInt(PortalId))
            If Not arr Is Nothing AndAlso arr.Count > 0 Then
                Dim i As Integer = 0
                For i = 0 To arr.Count - 1
                    Dim rolei As DotNetNuke.Security.Roles.RoleInfo = CType(arr(i), DotNetNuke.Security.Roles.RoleInfo)
                    If Not rolei Is Nothing Then
                        arr(i) = New Role(rolei)
                    End If
                Next
            End If
            Return arr
        End Function

        Public Function GetCurrentRole(ByVal sRole As String, ByVal pRoles As SortedList(Of String, String)) As String
            If Not sRole Is Nothing AndAlso sRole.Length > 0 AndAlso (pRoles.ContainsKey(sRole.ToUpper) OrElse sRole = DotNetNuke.Common.glbRoleAllUsersName OrElse sRole = DotNetNuke.Common.glbRoleUnauthUserName) Then
                Select Case sRole
                    Case DotNetNuke.Common.glbRoleAllUsersName
                        Return DotNetNuke.Common.glbRoleAllUsers
                    Case DotNetNuke.Common.glbRoleUnauthUserName
                        Return DotNetNuke.Common.glbRoleUnauthUser
                    Case Else
                        Return pRoles(sRole.ToUpper)
                End Select
            Else
                Return Nothing
            End If
        End Function

        Private Function RoleControllerInstance() As DotNetNuke.Security.Roles.RoleController
            'Return DotNetNuke.Entities.Users.UserController.Instance()
            Dim temporary As New DotNetNuke.Security.Roles.RoleController()
            Return temporary
        End Function
    End Class
End Namespace

