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
Imports System.Runtime.Serialization
Imports r2i.OWS.Framework.DataAccess
Imports r2i.OWS.Wrapper.DNN.DataAccess.Factories
Namespace Entities
    Public Class UserController
        Implements IUserController

         Sub SaveUser(ByVal user As IUser) Implements IUserController.SaveUser
            CType(user, DataAccess.User).Save()
        End Sub

        'Private Sub GetUser(ByVal draftOwnerId As Integer) Implements IUserController.GetUser
        '    'TODO: Get the CONTEXT to have the portalid
        '    Dim portalId As Integer
        '    portalId = DotNetNuke.Entities.Portals.PortalAliasInfo.PortalID
        '    Dim sUserController As New DotNetNuke.Entities.Users.UserController
        '    Dim sUserInfo As DotNetNuke.Entities.Users.UserInfo = sUserController.GetUser(portalId, draftOwnerId)
        'End Sub

        Dim cUser As DataAccess.User
        Public Function CurrentUser() As IUser Implements IUserController.CurrentUser
            If cUser Is Nothing Then
                Dim uinfo As DotNetNuke.Entities.Users.UserInfo = DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo
                If Not uinfo Is Nothing Then
                    cUser = New DataAccess.User(uinfo)
                End If
            End If
            Return cUser
        End Function

        Public Function IsInRoles(ByVal user As IUser, ByVal value As String) As Boolean Implements IUserController.IsInRoles
            Return UserFactory.Instance.IsInRole(user, value)
        End Function

        'Unused function
        Public Function IsInRoleNames_Change(ByVal isSuperUser As Boolean, ByVal value As String) As Boolean Implements IUserController.IsInRoleNames_Change
            Return UserFactory.Instance.IsInRoleNames_Change(isSuperUser, value)
        End Function

        Public Sub SetPassword(ByVal user As IUser, ByVal passValue As String) Implements IUserController.SetPassword

            UserFactory.Instance.SetPassword(CType(user, DataAccess.User).GetUser, passValue)
        End Sub

        Public Function AddUser(ByVal user As IUser, ByVal addToMembershipProvider As Boolean) As Integer Implements IUserController.AddUser
            Return UserFactory.Instance.AddUser(CType(user, DataAccess.User).GetUser, addToMembershipProvider)
        End Function

        Public Function GetUser(ByVal portalId As String, ByVal userId As String) As IUser Implements IUserController.GetUser
            Return CType(UserFactory.Instance.GetUser(CType(portalId, Integer), userId), IUser)
        End Function


        Public Function NewUser(ByVal portalId As String) As IUser Implements IUserController.NewUser
            Return CType(UserFactory.Instance.NewUser(CType(portalId, Integer)), IUser)
        End Function

        Public Function GetUserByUsername(ByVal portalId As String, ByVal userName As String) As IUser Implements IUserController.GetUserByUsername
            Return CType(UserFactory.Instance.GetUserByUsername(CType(portalId, Integer), userName), IUser)
        End Function
    End Class
End Namespace

