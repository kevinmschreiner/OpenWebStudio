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
Imports r2i.OWS.Wrapper.DNN.DataAccess.Factories
Namespace Entities
    Public Class RoleController
        Implements IRoleController

        Public Function GetRolesByUser(ByVal userId As String, ByVal portalId As String) As String() Implements IRoleController.GetRolesByUser
            Return RoleFactory.Instance.GetRolesByUser(userId, portalId)
        End Function

        Public Function GetRole(ByVal roleId As String, ByVal portalId As String) As IRole Implements IRoleController.GetRole
            Return CType(RoleFactory.Instance.GetRole(roleId, portalId), IRole)
        End Function

        Public Function GetRoleByName(ByVal portalId As String, ByVal roleName As String) As IRole Implements IRoleController.GetRoleByName
            Return CType(RoleFactory.Instance.GetRoleByName(portalId, roleName), IRole)
        End Function

        Public Function DeleteUserRole(ByVal portalId As String, ByVal userId As String, ByVal roleId As String) As Boolean Implements IRoleController.DeleteUserRole
            Return RoleFactory.Instance.DeleteUserRole(portalId, CInt(userId), CInt(roleId))
        End Function

        Public Sub AddUserRole(ByVal portalId As String, ByVal userId As String, ByVal roleId As String, ByVal ExpiryDate As Date) Implements IRoleController.AddUserRole
            RoleFactory.Instance.AddUserRole(portalId, CInt(userId), CInt(roleId), ExpiryDate)
        End Sub

        Public Function GetPortalRoles(ByVal PortalId As String) As System.Collections.ArrayList Implements IRoleController.GetPortalRoles
            Return RoleFactory.Instance.GetPortalRoles(PortalId)
        End Function


        Public Function GetCurrentRole(ByVal sRole As String, ByVal pRoles As System.Collections.Generic.SortedList(Of String, String)) As String Implements IRoleController.GetCurrentRole
            Return RoleFactory.Instance.GetCurrentRole(sRole, pRoles)
        End Function
    End Class
End Namespace
