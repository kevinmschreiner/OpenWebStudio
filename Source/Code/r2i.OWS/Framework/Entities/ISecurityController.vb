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
Imports System

Namespace r2i.OWS.Framework.Entities
    Public Interface ISecurityController
        Function RenderString_Encrypt(ByVal key As String, ByVal value As String) As String
        Function RenderString_Decrypt(ByVal key As String, ByVal value As String) As String
        Function UserLogin(ByVal username As String, ByVal password As String, ByVal portalId As Integer, ByVal portalName As String, ByVal ip As String, ByVal createPersistentCookie As Boolean) As Integer
        Function UserAuthenticate(ByVal UserObject As Framework.DataAccess.IUser, ByVal CreatePersistentCookie As Boolean, ByRef Portal As Framework.DataAccess.IPortalSettings, ByRef Context As Web.HttpContext) As Integer
        Function UserLogoff() As Boolean
        Function HasEditPermissions(ByVal moduleId As String) As Boolean
        Function IsInRole(ByVal role As String) As Boolean
        Function IsInRoles(ByVal roles As String) As Boolean
    End Interface
End Namespace