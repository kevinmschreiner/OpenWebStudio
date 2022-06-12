Imports DotNetNuke.Security

Namespace DataAccess.Factories
    Public Class SecurityFactory
        Private Shared _instance As SecurityFactory = New SecurityFactory()
        Public Shared ReadOnly Property Instance() As SecurityFactory
            Get
                Return _instance
            End Get
        End Property

        Private _cryption As DotNetNuke.Security.PortalSecurity

        Public Function RenderString_Encrypt(ByVal Key As String, ByVal Value As String) As String
            If _cryption Is Nothing Then
                _cryption = New DotNetNuke.Security.PortalSecurity
            End If
            Return _cryption.Encrypt(Key, Value)
        End Function

        Public Function RenderString_Decrypt(ByVal Key As String, ByVal Value As String) As String
            If _cryption Is Nothing Then
                _cryption = New DotNetNuke.Security.PortalSecurity
            End If
            Return _cryption.Decrypt(Key, Value)
        End Function

        Public Function UserLogin(ByVal Username As String, ByVal Password As String, ByVal PortalID As Integer, ByVal PortalName As String, ByVal IP As String, ByVal CreatePersistentCookie As Boolean) As Integer
            Dim portalSecurity As New DotNetNuke.Security.PortalSecurity
            'DotNetNuke.Security.Membership.MembershipProvider.Instance.UserLogin
            'DotNetNuke.Entities.Users.UserController.UserLogin(PortalID, Username, Password,)
            'Dim uservalue As DotNetNuke.Entities.Users.UserInfo = DotNetNuke.Entities.Users.UserController.GetUserByName(Username)
            'If (Not uservalue Is Nothing AndAlso uservalue.UserID >= 0) Then
            'Return portalSecurity.UserLogin(Username, Password, PortalID, PortalName, IP, CreatePersistentCookie)
            'DotNetNuke.Entities.Users.UserController.UserLogin(PortalID, uservalue, PortalName, IP, CreatePersistentCookie)
            Dim loginStatus As Membership.UserLoginStatus = New Membership.UserLoginStatus()
            Dim uservalue As DotNetNuke.Entities.Users.UserInfo = DotNetNuke.Entities.Users.UserController.UserLogin(PortalID, Username, Password, "", PortalName, IP, loginStatus, CreatePersistentCookie)

            Select Case loginStatus
                Case Membership.UserLoginStatus.LOGIN_SUCCESS
                    Return uservalue.UserID
                Case Membership.UserLoginStatus.LOGIN_FAILURE,
                 Membership.UserLoginStatus.LOGIN_INSECUREADMINPASSWORD,
                    Membership.UserLoginStatus.LOGIN_INSECUREHOSTPASSWORD,
                    Membership.UserLoginStatus.LOGIN_USERLOCKEDOUT,
                    Membership.UserLoginStatus.LOGIN_USERNOTAPPROVED
                    uservalue = DotNetNuke.Entities.Users.UserController.GetUserByName(Username)
                    If (uservalue Is Nothing OrElse uservalue.UserID < 0) Then Return -4
                    Return -1
                Case Membership.UserLoginStatus.LOGIN_SUPERUSER
                    uservalue = DotNetNuke.Entities.Users.UserController.GetUserByName(Username)
                    Return uservalue.UserID
            End Select

            'End If
            Return -1
        End Function

        Public Function UserLogoff() As Boolean
            Dim portalSecurity As New DotNetNuke.Security.PortalSecurity
            portalSecurity.SignOut()
            Return True
        End Function

        Public Function UserAuthenticate(ByVal UserObject As IUser, ByVal CreatePersistentCookie As Boolean, ByRef Portal As IPortalSettings, ByRef Context As Web.HttpContext) As Integer
            '-2 = Unknown User
            '-1 = Unapproved User
            '0 = User Locked Out
            '1 = Success
            Dim result As Integer = -2
            '            If Not _Engine.Request.Cookies("portalaliasid") Is Nothing Then
            If UserObject.Approved Then
                If Not UserObject.LockedOut Then
                    Web.Security.FormsAuthentication.SetAuthCookie(UserObject.UserName, False)
                    If Not Portal Is Nothing Then
                        Dim PortalCookie As Web.Security.FormsAuthenticationTicket = Nothing
                        If Not Context.Request.Cookies("portalaliasid") Is Nothing Then
                            PortalCookie = Web.Security.FormsAuthentication.Decrypt(Context.Request.Cookies("portalaliasid").Value)
                        End If
                        ' check if user has switched portals
                        'If _Engine.PortalSettings.PortalAlias.PortalAliasID <> Int32.Parse(PortalCookie.UserData) Then
                        If PortalCookie Is Nothing OrElse Int32.Parse(Portal.PortalAliasID) <> Int32.Parse(PortalCookie.UserData) Then
                            ' expire cookies if portal has changed
                            Context.Response.Cookies("portalaliasid").Value = Nothing
                            Context.Response.Cookies("portalaliasid").Path = "/"
                            Context.Response.Cookies("portalaliasid").Expires = DateTime.Now.AddYears(-30)

                            Context.Response.Cookies("portalroles").Value = Nothing
                            Context.Response.Cookies("portalroles").Path = "/"
                            Context.Response.Cookies("portalroles").Expires = DateTime.Now.AddYears(-30)
                        End If

                        ' create cookies if they do not exist yet for this session.
                        If Context.Request.Cookies("portalroles") Is Nothing Then
                            ' keep cookies in sync
                            Dim CurrentDateTime As Date = DateTime.Now

                            ' create a cookie authentication ticket ( version, user name, issue time, expires every hour, don't persist cookie, roles )
                            'Dim PortalTicket As New System.Web.Security.FormsAuthenticationTicket(1, UserObject.Username, CurrentDateTime, CurrentDateTime.AddHours(1), CreatePersistentCookie, _Engine.PortalSettings.PortalAlias.PortalAliasID.ToString)
                            Dim PortalTicket As New System.Web.Security.FormsAuthenticationTicket(1, UserObject.UserName, CurrentDateTime, CurrentDateTime.AddHours(1), CreatePersistentCookie, Portal.PortalAliasID.ToString)

                            ' encrypt the ticket
                            Dim strPortalAliasID As String = System.Web.Security.FormsAuthentication.Encrypt(PortalTicket)

                            ' send portal cookie to client
                            Context.Response.Cookies("portalaliasid").Value = strPortalAliasID
                            Context.Response.Cookies("portalaliasid").Path = "/"
                            Context.Response.Cookies("portalaliasid").Expires = CurrentDateTime.AddMinutes(1)

                            ' get roles from UserRoles table
                            Dim arrPortalRoles As String()

                            'ROMAIN: 09/19/07
                            Dim objRoleController As IRoleController = AbstractFactory.Instance.RoleController
                            arrPortalRoles = objRoleController.GetRolesByUser(UserObject.Id, Portal.PortalId)

                            ' create a string to persist the roles
                            Dim strPortalRoles As String = Join(arrPortalRoles, New Char() {";"c})

                            ' create a cookie authentication ticket ( version, user name, issue time, expires every hour, don't persist cookie, roles )
                            Dim RolesTicket As New System.Web.Security.FormsAuthenticationTicket(1, UserObject.UserName, CurrentDateTime, CurrentDateTime.AddHours(1), CreatePersistentCookie, strPortalRoles)
                            ' encrypt the ticket
                            Dim strRoles As String = System.Web.Security.FormsAuthentication.Encrypt(RolesTicket)

                            ' send roles cookie to client
                            Context.Response.Cookies("portalroles").Value = strRoles
                            Context.Response.Cookies("portalroles").Path = "/"
                            Context.Response.Cookies("portalroles").Expires = CurrentDateTime.AddMinutes(1)
                        End If

                        If Not Context.Request.Cookies("portalroles") Is Nothing Then
                            ' get roles from roles cookie
                            If Context.Request.Cookies("portalroles").Value <> "" Then
                                Dim RoleTicket As System.Web.Security.FormsAuthenticationTicket = System.Web.Security.FormsAuthentication.Decrypt(Context.Request.Cookies("portalroles").Value)

                                ' convert the string representation of the role data into a string array
                                ' and store it in the Roles Property of the User
                                'UserObject.Roles = RoleTicket.UserData.Split(";"c)
                            End If
                            If Not Context.Items.Contains("UserInfo") Then
                                Context.Items.Add("UserInfo", CType(UserObject, Wrapper.DNN.DataAccess.User).GetUserObject)
                            Else
                                Context.Items("UserInfo") = CType(UserObject, Wrapper.DNN.DataAccess.User).GetUserObject
                            End If
                            
                            'Localization.SetLanguage(objUser.Profile.PreferredLocale)
                        End If
                    End If
                    result = 1
                Else
                    result = 0
                End If
            Else
                result = -1
            End If
            'End If
            Return result
        End Function

        Public Shared Function HasEditPermissions(ByVal ModuleId As Integer) As Boolean

            'Dim tb As IList(Of DotNetNuke.Entities.Modules.ModuleInfo) = ModuleControllerInstance().GetTabModulesByModule(CInt(ModuleId))
            'If Not tb Is Nothing AndAlso tb.Count > 0 Then
            '    Return DotNetNuke.Security.Permissions.ModulePermissionController.CanEditModuleContent(tb(0))
            'End If
            Dim info As DotNetNuke.Entities.Modules.ModuleInfo = ModuleControllerInstance().GetModule(CInt(ModuleId))
            If Not info Is Nothing Then
                Return DotNetNuke.Security.Permissions.ModulePermissionController.CanEditModuleContent(info)
            End If
        End Function

        Public Shared Function IsInRoles(ByVal roles As String) As Boolean
            Return DotNetNuke.Security.PortalSecurity.IsInRoles(roles)
        End Function

        Public Shared Function IsInRole(ByVal role As String) As Boolean
            Return DotNetNuke.Security.PortalSecurity.IsInRole(role)
        End Function

        Private Shared Function UserControllerInstance() As DotNetNuke.Entities.Users.UserController
            'Return DotNetNuke.Entities.Users.UserController.Instance()
            Dim temporary As New DotNetNuke.Entities.Users.UserController()
            Return temporary
        End Function
        Private Shared Function ModuleControllerInstance() As DotNetNuke.Entities.Modules.ModuleController
            'Return DotNetNuke.Entities.Modules.ModuleController.Instance()
            Dim temporary As New DotNetNuke.Entities.Modules.ModuleController()
            Return temporary
        End Function
        Private Shared Function TabControllerInstance() As DotNetNuke.Entities.Tabs.TabController
            'Return DotNetNuke.Entities.Tabs.TabController.Instance()
            Dim temporary As New DotNetNuke.Entities.Tabs.TabController()
            Return temporary
        End Function
    End Class
End Namespace

