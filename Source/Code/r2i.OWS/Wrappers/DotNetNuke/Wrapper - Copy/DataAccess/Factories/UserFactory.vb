Imports r2i.OWS.Framework.DataAccess
Imports DotNetNuke.Entities.Users
Namespace DataAccess.Factories
    Public Class UserFactory
        Private Shared _instance As UserFactory = New UserFactory()

        Public Shared ReadOnly Property Instance() As UserFactory
            Get
                Return _instance
            End Get
        End Property

        Public Function IsInRole(ByVal user As IUser, ByVal value As String) As Boolean
            If Not user Is Nothing Then
                If user.IsSuperUser Then
                    Return True
                Else
                    value = value.Replace(",", ";")
                    Return DotNetNuke.Security.PortalSecurity.IsInRoles(value)
                End If
            End If
        End Function

        Public Function IsInRoleNames_Change(ByRef IsSuperUser As Boolean, ByVal VALUE As String) As Boolean
            If IsSuperUser Then
                Return True
            Else
                VALUE = VALUE.Replace(",", ";")
                Return DotNetNuke.Security.PortalSecurity.IsInRoles(VALUE)
            End If
        End Function

        Public Sub SetPassword(ByVal user As UserInfo, ByVal passValue As String)
            'Dim UserControl As UserController
            Dim UserControl As New DotNetNuke.Entities.Users.UserController
            UserControl.SetPassword(user, passValue)
        End Sub

        Public Function AddUser(ByVal user As UserInfo, ByVal AddToMembershipProvider As Boolean) As Integer
            Dim UserControl As New DotNetNuke.Entities.Users.UserController
            Dim userId As Integer
            'userId = UserControl.AddUser(user, AddToMembershipProvider)
            Dim result As DotNetNuke.Security.Membership.UserCreateStatus

            result = DotNetNuke.Entities.Users.UserController.CreateUser(user)
            userId = -1
            'VALID RESULT VALUES
            'USERID > 0  - USERID
            'USERID < 0  - FAILURE
            'USERID = -1 - UNIDENTIFIED ERROR
            'USERID = -2 - Duplicate UserName / UserKey / Already Exists / Already Registered
            'USERID = -3 - USER REJECTED
            'USERID = -4 - INVALID USERNAME
            'USERID = -5 - UNEXPECTED / CAUGHT ERROR
            'USERID = -6 - BAD/INVALID PASSWORD
            'USERID = -7 - INVALID ANSWER/QUESTION
            Select Case result
                Case DotNetNuke.Security.Membership.UserCreateStatus.AddUser
                    userId = user.UserID
                Case DotNetNuke.Security.Membership.UserCreateStatus.AddUserToPortal
                    userId = user.UserID
                Case DotNetNuke.Security.Membership.UserCreateStatus.DuplicateEmail
                    userId = -2
                Case DotNetNuke.Security.Membership.UserCreateStatus.DuplicateProviderUserKey
                    userId = -2
                Case DotNetNuke.Security.Membership.UserCreateStatus.DuplicateUserName
                    userId = -2
                Case DotNetNuke.Security.Membership.UserCreateStatus.InvalidAnswer
                    userId = -7
                Case DotNetNuke.Security.Membership.UserCreateStatus.InvalidEmail
                    userId = -2
                Case DotNetNuke.Security.Membership.UserCreateStatus.InvalidPassword
                    userId = -6
                Case DotNetNuke.Security.Membership.UserCreateStatus.InvalidProviderUserKey
                    userId = -2
                Case DotNetNuke.Security.Membership.UserCreateStatus.InvalidQuestion
                    userId = -7
                Case DotNetNuke.Security.Membership.UserCreateStatus.InvalidUserName
                    userId = -4
                Case DotNetNuke.Security.Membership.UserCreateStatus.PasswordMismatch
                    userId = -6
                Case DotNetNuke.Security.Membership.UserCreateStatus.ProviderError
                    userId = -5
                Case DotNetNuke.Security.Membership.UserCreateStatus.Success
                    userId = user.UserID
                Case DotNetNuke.Security.Membership.UserCreateStatus.UnexpectedError
                    userId = -5
                Case DotNetNuke.Security.Membership.UserCreateStatus.UserAlreadyRegistered
                    userId = -2
                Case DotNetNuke.Security.Membership.UserCreateStatus.UsernameAlreadyExists
                    userId = -2
                Case DotNetNuke.Security.Membership.UserCreateStatus.UserRejected
                    userId = -3
            End Select
            Return userId
        End Function

        Public Function GetUser(ByVal portalId As Integer, ByVal userId As String) As DataAccess.User
            Dim UserControl As New DotNetNuke.Entities.Users.UserController
            Dim userIdConvert As Integer
            If Integer.TryParse(userId, userIdConvert) Then
                Return New DataAccess.User(UserControl.GetUser(portalId, userIdConvert))
            Else
                Return Nothing
            End If
        End Function

        Public Function NewUser(ByVal portalId As Integer) As DataAccess.User
            Dim UserControl As New DotNetNuke.Entities.Users.UserController
            Dim userInfoObj As New UserInfo()
            Return New DataAccess.User(userInfoObj)
        End Function

        Public Function GetUserByUsername(ByVal portalID As Integer, ByVal username As String) As DataAccess.User
            Dim UserControl As New DotNetNuke.Entities.Users.UserController
            Dim uinfo As DotNetNuke.Entities.Users.UserInfo = UserControl.GetUserByUsername(portalID, username)
            If Not uinfo Is Nothing Then
                Return New DataAccess.User(uinfo)
            Else
                Return Nothing
            End If
        End Function

    End Class
End Namespace

