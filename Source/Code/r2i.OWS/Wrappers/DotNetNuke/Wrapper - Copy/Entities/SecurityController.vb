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
Imports r2i.OWS.Framework.DataAccess
Imports r2i.OWS.Wrapper.DNN.DataAccess.Factories

Namespace Entities
    Public Class SecurityController
        Implements ISecurityController

        Public Function RenderString_Decrypt(ByVal key As String, ByVal value As String) As String Implements ISecurityController.RenderString_Decrypt
            Return SecurityFactory.Instance.RenderString_Decrypt(key, value)
        End Function

        Public Function RenderString_Encrypt(ByVal key As String, ByVal value As String) As String Implements ISecurityController.RenderString_Encrypt
            Return SecurityFactory.Instance.RenderString_Encrypt(key, value)
        End Function

        Public Function UserLogin(ByVal username As String, ByVal password As String, ByVal portalId As Integer, ByVal portalName As String, ByVal ip As String, ByVal createPersistentCookie As Boolean) As Integer Implements ISecurityController.UserLogin
            Return SecurityFactory.Instance.UserLogin(username, password, portalId, portalName, ip, createPersistentCookie)
        End Function

        Public Function UserAuthenticate(ByVal UserObject As Framework.DataAccess.IUser, ByVal CreatePersistentCookie As Boolean, ByRef Portal As Framework.DataAccess.IPortalSettings, ByRef Context As Web.HttpContext) As Integer Implements ISecurityController.UserAuthenticate
            Return SecurityFactory.Instance.UserAuthenticate(UserObject, CreatePersistentCookie, Portal, Context)
        End Function

        Public Function UserLogoff() As Boolean Implements ISecurityController.UserLogoff
            Return SecurityFactory.Instance.UserLogoff()
        End Function

        Public Function HasEditPermissionsByConfigurationId(ByVal configurationId As Guid) As Boolean 'Implements ISecurityController.HasEditPermissions
            Dim tabmodId() As String = DataProvider.Instance().GetConfigNameByConfigurationId(configurationId)
            Dim moduleId As Integer
            If Integer.TryParse(tabmodId(1), moduleId) Then
                Return SecurityFactory.HasEditPermissions(moduleId)
            Else
                'Todo: implements Exception Unable to get the moduleId
                Return False
            End If

        End Function
        Public Function HasEditPermissions(ByVal moduleId As String) As Boolean Implements ISecurityController.HasEditPermissions
            Dim moduleIdConv As Integer
            If Integer.TryParse(moduleId, moduleIdConv) Then
                Return SecurityFactory.HasEditPermissions(moduleIdConv)
            Else
                'Todo: implements Exception Unable to get the moduleId
                Return False
            End If

        End Function

        Public Function IsInRole(ByVal role As String) As Boolean Implements ISecurityController.IsInRole
            Return SecurityFactory.IsInRole(role)
        End Function

        Public Function IsInRoles(ByVal roles As String) As Boolean Implements ISecurityController.IsInRoles
            Return SecurityFactory.IsInRoles(roles)
        End Function
    End Class
End Namespace

