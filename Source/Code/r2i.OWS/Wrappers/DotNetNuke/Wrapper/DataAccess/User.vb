'<LICENSE>
'   Open Web Studio - http://www.OpenWebStudio.com
'   Copyright (c) 2007-2008
'   by R2Integrated Inc. http://www.R2integrated.com
'      
'   Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
'   documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
'   the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
'   to permit persons to whom the Software is furnished to do so, subject to the following conditions:
'    
'   The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
'   the Software.
'   
'   This Software and associated documentation files are subject to the terms and conditions of the Open Web Studio 
'   End User License Agreement and version 2 of the GNU General Public License.
'    
'   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
'   TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
'   THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
'   CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
'   DEALINGS IN THE SOFTWARE.
'</LICENSE>
Imports System.Reflection
Imports r2i.OWS.Framework.Utilities.Compatibility, r2i.OWS.Framework.Utilities

Namespace DataAccess
    Public Class User
        'Inherits DotNetNuke.Entities.Users.UserInfo
        Implements IUser

        'TODO: Move GetUserInformations to UserController and remove the instance declaration
        Private Shared ReadOnly _Instance As User = New User()
        Public Shared ReadOnly Property Instance() As User
            Get
                Return _Instance
            End Get
        End Property

        Private _UserProperties As Dictionary(Of String, Object)
        Private _Id As String

        Private userTotallyLoaded As Boolean
        Private loading As Boolean
        Private userInfo As DotNetNuke.Entities.Users.UserInfo
        Private sUserController As New DotNetNuke.Entities.Users.UserController

        Public Sub New()

        End Sub
        Public Sub New(ByVal UserData As DotNetNuke.Entities.Users.UserInfo)
            userInfo = UserData
            _Id = userInfo.UserID.ToString
            LoadUser()
        End Sub
        Public ReadOnly Property Properties() As System.Collections.Generic.IDictionary(Of String, Object) Implements IUser.Properties
            Get
                'CType(FillDictionaryProperies(), Global.System.Collections.Generic.Dictionary(Of String, Object))
                LoadUser()
                Return _UserProperties
            End Get
        End Property
        Private Function GetProperty(ByVal Name As String, Optional ByVal [Default] As String = "") As String
            If Properties.ContainsKey(Name) Then
                If Not Properties(Name) Is Nothing Then
                    Return Properties(Name).ToString
                Else
                    Return [Default]
                End If
            Else
                Return [Default]
            End If
            Return [Default]
        End Function

        Public Property Id() As String Implements IUser.Id
            Get
                LoadUser()
                Return _Id
            End Get
            Set(ByVal value As String)
                LoadUser()
                _Id = value
            End Set
        End Property


        Public Property UserId() As String Implements IUser.UserId
            Get
                Return Id
            End Get
            Set(ByVal value As String)
                Id = value
            End Set
        End Property

        Public Property SiteId() As String Implements IUser.SiteId
            Get
                LoadUser()
                Return GetProperty("PortalID")
            End Get
            Set(ByVal value As String)
                LoadUser()
                Properties("PortalID") = value
            End Set
        End Property
        Public Property Email() As String Implements IUser.Email
            Get
                LoadUser()
                Return GetProperty("Email")
            End Get
            Set(ByVal value As String)
                LoadUser()
                Properties("Email") = value
            End Set
        End Property
        Public Property Login() As String Implements IUser.Login
            Get
                LoadUser()
                Return GetProperty("Username")
            End Get
            Set(ByVal value As String)
                LoadUser()
                Properties("Username") = value
            End Set
        End Property
        Public Property Password() As String Implements IUser.Password
            Get
                LoadUser()
                Return GetProperty("Password")
            End Get
            Set(ByVal value As String)
                LoadUser()
                Properties("Password") = value
            End Set
        End Property
        Public Property FirstName() As String Implements IUser.FirstName
            Get
                LoadUser()
                Return GetProperty("FirstName")
            End Get
            Set(ByVal value As String)
                LoadUser()
                Properties("FirstName") = value
            End Set
        End Property

        Public Property LastName() As String Implements IUser.LastName
            Get
                LoadUser()
                Return GetProperty("LastName")
            End Get
            Set(ByVal value As String)
                LoadUser()
                Properties("LastName") = value
            End Set
        End Property

        Public Property UserName() As String Implements IUser.UserName
            Get
                LoadUser()
                Return GetProperty("Username")
            End Get
            Set(ByVal value As String)
                LoadUser()
                Properties("Username") = value
            End Set
        End Property

        Public Property IsSuperUser() As Boolean Implements IUser.IsSuperUser
            Get
                LoadUser()
                Return CBool(GetProperty("IsSuperUser"))

            End Get
            Set(ByVal value As Boolean)
                LoadUser()
                Properties("IsSuperUser") = value
            End Set
        End Property

        Public ReadOnly Property IsAdministrator() As Boolean Implements IUser.IsAdministrator
            Get
                LoadUser()
                Dim VALUE As String = ""
                If Not userInfo Is Nothing AndAlso Not userInfo.Roles Is Nothing Then
                    Dim r As String = ""
                    Dim ri As String = ""
                    For Each ri In userInfo.Roles
                        r &= ";" & ri
                    Next
                    If r.Length > 0 Then
                        VALUE = r.Remove(0, 1)
                    Else
                        VALUE = ""
                    End If
                Else
                    VALUE = ""
                End If
                    Return DotNetNuke.Security.PortalSecurity.IsInRoles(VALUE)
            End Get
        End Property

        Public Property DisplayName() As String Implements IUser.DisplayName
            Get
                LoadUser()
                Return GetProperty("DisplayName")
            End Get
            Set(ByVal value As String)
                LoadUser()
                Properties("DisplayName") = value
            End Set
        End Property

        Public Property Approved() As Boolean Implements IUser.Approved
            Get
                LoadUser()
                Return CBool(GetProperty("Approved", "False"))
            End Get
            Set(ByVal value As Boolean)
                LoadUser()
                Properties("Approved") = value
            End Set
        End Property

        Public Property LockedOut() As Boolean Implements IUser.LockedOut
            Get
                LoadUser()
                Return CBool(GetProperty("LockedOut", "False"))
            End Get
            Set(ByVal value As Boolean)
                LoadUser()
                Properties("LockedOut") = value
            End Set
        End Property

        Public Property [Property](ByVal Name As String) As Object Implements IUser.Property
            Get
                LoadUser()
                If Properties.ContainsKey(Name) Then
                    Return Properties(Name)
                Else
                    Return Nothing
                End If
            End Get
            Set(ByVal value As Object)
                LoadUser()
                If Properties.ContainsKey(Name) Then
                    Properties(Name) = value
                Else
                    Properties.Add(Name, value)
                End If
            End Set
        End Property

        Private Sub LoadUser()
            If Not loading Then
                If (Not userTotallyLoaded AndAlso Not userInfo Is Nothing) Then
                    Hydrate()
                    userTotallyLoaded = True
                ElseIf (Not userTotallyLoaded AndAlso Not _Id Is Nothing) Then
                    userInfo = DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo
                    Hydrate()
                    userTotallyLoaded = True
                ElseIf Not userTotallyLoaded Then
                    'NEW USER
                    userInfo = New DotNetNuke.Entities.Users.UserInfo
                    Hydrate()
                    userTotallyLoaded = True
                End If
            End If
        End Sub

        Public Function GetUserObject() As DotNetNuke.Entities.Users.UserInfo
            Return userInfo
        End Function
        Private Function GetPassword(ByVal UserObj As DotNetNuke.Entities.Users.UserInfo, ByVal answertext As String) As String
            Dim strPassword As String
            Try
                If UserObj.UserID >= 0 Then
                    Dim ctrl As New DotNetNuke.Entities.Users.UserController

                    Dim t As Type = ctrl.GetType
                    Dim miarray As System.Reflection.MemberInfo() = ctrl.GetType().GetMember("GetPassword")

                    strPassword = CStr(t.InvokeMember("GetPassword", Reflection.BindingFlags.InvokeMethod, Nothing, ctrl, New Object() {UserObj, answertext}))
                Else
                    strPassword = Nothing
                End If
            Catch ex As Exception
                strPassword = "Dotnetnuke no longer supports this capability"
            End Try
            Return strPassword
        End Function
        Private Sub Hydrate()
            Try
                loading = True

                _UserProperties = New Dictionary(Of String, Object)
                Me.Id = CStr(userInfo.UserID)
                Me.Email = userInfo.Email
                Me.Login = userInfo.Username

                Me.Password = userInfo.Membership.Password
                Try
                    If userInfo.UserID >= 0 AndAlso (Me.Password Is Nothing OrElse Me.Password.Length = 0) Then
                        Me.Password = GetPassword(userInfo, Nothing)
                    End If
                Catch ex As Exception

                End Try
                Me.FirstName = userInfo.FirstName
                Me.LastName = userInfo.LastName
                Me.UserName = userInfo.Username
                Me.DisplayName = userInfo.DisplayName

                If Me.Id <> "-1" Then
                    Me.Approved = userInfo.Membership.Approved
                    Me.LockedOut = userInfo.Membership.LockedOut
                    Me.IsSuperUser = userInfo.IsSuperUser
                Else
                    Me.IsSuperUser = False
                    Me.LockedOut = False
                    Me.Approved = False
                End If
                Me.SiteId = CStr(userInfo.PortalID)

                'Extra Properties
                Dim bindingAttr As BindingFlags = BindingFlags.[Public] Or BindingFlags.[Static] Or BindingFlags.Instance
                'Dim xproperties As PropertyInfo() = DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo.[GetType]().GetProperties(bindingAttr)

                'Dim i As Integer
                'Dim curType As Object
                'Dim listProfile As Dictionary(Of String, Object) = New Dictionary(Of String, Object)

                'Dim objValue As String
                'For i = 0 To xproperties.Length - 1
                '    curType = DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo.[GetType]().GetProperties()(i).Name
                '    If Not Properties.ContainsKey(CStr(curType)) Then
                '        Dim objectType As Type = DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo.[GetType]().GetProperties()(i).[GetType]()
                '        'objValue = CType(DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo.[GetType]().GetProperties()(3), Reflection.PropertyInfo).GetValue(UserInfo, Nothing)
                '        [Property](CStr(curType)) = DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo.[GetType]().GetProperties()(i)
                '        'listProfile.Add(CStr(curType), DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo.[GetType]().GetProperties()(i))
                '    End If
                'Next

                Dim prop As Reflection.PropertyInfo
                For Each prop In userInfo.Membership.GetType().GetProperties(bindingAttr)
                    Try
                        If Not Properties.ContainsKey("Membership." & prop.Name) Then
                            [Property]("Membership." & prop.Name) = prop.GetValue(userInfo.Membership, Nothing)
                        End If
                    Catch ex As Exception
                    End Try
                Next

                For Each prop In userInfo.Profile.GetType().GetProperties(bindingAttr)
                    Try
                        If prop.Name <> "Item" Then
                            If Not Properties.ContainsKey("Profile." & prop.Name) Then
                                [Property]("Profile." & prop.Name) = prop.GetValue(userInfo.Profile, Nothing)
                            End If
                        End If
                    Catch ex As Exception
                    End Try
                Next


                Dim pp As DotNetNuke.Entities.Profile.ProfilePropertyDefinition
                For Each pp In userInfo.Profile.ProfileProperties
                    Try
                        If Not Properties.ContainsKey("Profile." & pp.PropertyName) Then
                            [Property]("Profile." & pp.PropertyName) = pp.PropertyValue
                        End If
                    Catch ex As Exception
                    End Try
                Next
            Catch ex As Exception
            Finally
                loading = False
            End Try
        End Sub

        Public Function GetUser() As DotNetNuke.Entities.Users.UserInfo
            Dim SavedProperties As New ArrayList()
            'GET THE EXISTING USER
            Dim userObj As DotNetNuke.Entities.Users.UserInfo
            If CInt(Me.Id) > 0 Then
                'userObj = DotNetNuke.Entities.Users.UserController.Instance.GetUser(CInt(Me.SiteId), CInt(Me.Id))
                userObj = UserControllerInstance().GetUser(CInt(Me.SiteId), CInt(Me.Id))
            Else
                userObj = New DotNetNuke.Entities.Users.UserInfo
                userObj.UserID = -1
            End If

            If Utility.CNullStr(userObj.DisplayName) <> Utility.CNullStr(Me.DisplayName) Then
                userObj.DisplayName = Me.DisplayName
            End If
            SavedProperties.Add("DisplayName")
            SavedProperties.Add("Profile.FullName")
            If Utility.CNullStr(userObj.Email) <> Utility.CNullStr(Me.Email) Then
                userObj.Email = Me.Email
                'userObj.Membership.Email = Me.Email
            End If
            SavedProperties.Add("Email")
            SavedProperties.Add("Membership.Email")
            If Me.SiteId Is Nothing Then
                Me.SiteId = "0"
            End If
            If userObj.PortalID.ToString <> Me.SiteId Then
                userObj.PortalID = CType(Me.SiteId, Integer)
            End If
            SavedProperties.Add("PortalID")
            If Utility.CNullStr(userObj.FirstName) <> Utility.CNullStr(Me.FirstName) Then
                userObj.FirstName = Me.FirstName
                userObj.Profile.FirstName = Me.FirstName
            End If
            SavedProperties.Add("FirstName")
            SavedProperties.Add("Profile.FirstName")
            If Utility.CNullStr(userObj.LastName) <> Utility.CNullStr(Me.LastName) Then
                userObj.LastName = Me.LastName
                userObj.Profile.LastName = Me.LastName
            End If
            SavedProperties.Add("LastName")
            SavedProperties.Add("Profile.LastName")
            If userObj.IsSuperUser <> Me.IsSuperUser Then
                userObj.IsSuperUser = Me.IsSuperUser
            End If
            SavedProperties.Add("IsSuperUser")
            If userObj.Username <> Me.UserName Then
                userObj.Username = Me.UserName
                'userObj.Membership = Me.UserName
            End If
            SavedProperties.Add("Username")
            SavedProperties.Add("Membership.Username")
            If Not Me.Password Is Nothing AndAlso Utility.CNullStr(userObj.Membership.Password, "") <> Me.Password Then
                userObj.Membership.Password = Me.Password
                'userObj.Membership.UpdatePassword = True
            End If
            SavedProperties.Add("Password")
            SavedProperties.Add("UpdatePassword")
            SavedProperties.Add("Membership.UpdatePassword")
            SavedProperties.Add("Membership.Password")

            ''OTHER PROPERTIES
            Dim bindingAttr As BindingFlags = BindingFlags.[Public] Or BindingFlags.[Static] Or BindingFlags.Instance
            Dim prop As Reflection.PropertyInfo
            For Each prop In userInfo.Membership.GetType().GetProperties(bindingAttr)
                Try
                    Dim propertyName As String = "Membership." & prop.Name
                    If Properties.ContainsKey(propertyName) AndAlso Not (SavedProperties.Contains(propertyName)) Then
                        SavedProperties.Add(propertyName)

                        Dim strValue As String = ""
                        Try
                            If Not Me.Property("Membership." & prop.Name) Is Nothing Then
                                strValue = Me.Property("Membership." & prop.Name).ToString
                            End If
                        Catch ex As Exception
                        End Try

                        Try
                            If (prop.GetValue(userObj.Membership, Nothing) Is Nothing AndAlso Not strValue = "") OrElse (prop.GetValue(userObj.Membership, Nothing) Is Nothing OrElse Not prop.GetValue(userObj.Membership, Nothing).ToString = strValue) Then
                                'If Not prop.GetValue(userObj.Membership, Nothing).ToString = strValue Then
                                If prop.GetGetMethod.ReturnType.Name = GetType(Integer).Name Then
                                    prop.SetValue(userObj.Membership, CType(strValue, Integer), Nothing)
                                ElseIf prop.GetGetMethod.ReturnType.Name = GetType(Int16).Name Then
                                    prop.SetValue(userObj.Membership, CType(strValue, Int16), Nothing)
                                ElseIf prop.GetGetMethod.ReturnType.Name = GetType(Int32).Name Then
                                    prop.SetValue(userObj.Membership, CType(strValue, Int32), Nothing)
                                ElseIf prop.GetGetMethod.ReturnType.Name = GetType(Boolean).Name Then
                                    prop.SetValue(userObj.Membership, CType(strValue, Boolean), Nothing)
                                Else
                                    prop.SetValue(userObj.Membership, strValue, Nothing)
                                End If
                            End If
                        Catch ex As Exception
                        End Try
                    End If
                Catch ex As Exception
                End Try
            Next

            For Each prop In userInfo.Profile.GetType().GetProperties(bindingAttr)
                Try
                    Dim propertyName As String = "Profile." & prop.Name
                    If Properties.ContainsKey(propertyName) AndAlso Not (SavedProperties.Contains(propertyName)) Then
                        SavedProperties.Add(propertyName)

                        Dim strValue As String = ""
                        Try
                            If Not Me.Property("Profile." & prop.Name) Is Nothing Then
                                strValue = Me.Property("Profile." & prop.Name).ToString
                            Else
                                strValue = ""
                            End If
                        Catch ex As Exception
                        End Try

                        Try
                            If (prop.GetValue(userObj.Profile, Nothing) Is Nothing AndAlso Not strValue = "") OrElse (prop.GetValue(userObj.Profile, Nothing) Is Nothing OrElse Not prop.GetValue(userObj.Profile, Nothing).ToString = strValue) Then
                                If prop.GetGetMethod.ReturnType.Name = GetType(Integer).Name Then
                                    prop.SetValue(userObj.Profile, CType(strValue, Integer), Nothing)
                                ElseIf prop.GetGetMethod.ReturnType.Name = GetType(Int16).Name Then
                                    prop.SetValue(userObj.Profile, CType(strValue, Int16), Nothing)
                                ElseIf prop.GetGetMethod.ReturnType.Name = GetType(Int32).Name Then
                                    prop.SetValue(userObj.Profile, CType(strValue, Int32), Nothing)
                                ElseIf prop.GetGetMethod.ReturnType.Name = GetType(Boolean).Name Then
                                    prop.SetValue(userObj.Profile, CType(strValue, Boolean), Nothing)
                                Else
                                    prop.SetValue(userObj.Profile, strValue, Nothing)
                                End If
                            End If
                        Catch ex As Exception
                        End Try
                    End If
                Catch ex As Exception
                End Try
            Next
            Dim key As String
            Dim pkey As String = Nothing
            For Each key In Properties.Keys
                Try
                    If Not SavedProperties.Contains(key) Then
                        SavedProperties.Add(key)
                        If key.ToUpper.StartsWith("PROFILE.") Then
                            pkey = key.Substring(8)
                        Else
                            pkey = key
                        End If
                        If Not [Property](key) Is Nothing Then
                            userObj.Profile.SetProfileProperty(pkey, [Property](key).ToString)
                        Else
                            userObj.Profile.SetProfileProperty(pkey, Nothing)
                        End If
                    End If
                Catch ex As Exception
                End Try
            Next
            Return userObj
        End Function
        Public Sub Save()
            Dim userObj As DotNetNuke.Entities.Users.UserInfo = GetUser()

            DotNetNuke.Entities.Users.UserController.UpdateUser(userObj.PortalID, userObj)

        End Sub

        Private Function UserControllerInstance() As DotNetNuke.Entities.Users.UserController
            'Return DotNetNuke.Entities.Users.UserController.Instance()
            Dim temporary As New DotNetNuke.Entities.Users.UserController()
            Return temporary
        End Function
    End Class
End Namespace


