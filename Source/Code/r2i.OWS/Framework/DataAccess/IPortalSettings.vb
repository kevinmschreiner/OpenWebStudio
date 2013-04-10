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
Imports System.Collections
Imports System.Collections.Generic

Namespace r2i.OWS.Framework.DataAccess
    Public Interface IPortalSettings
        ReadOnly Property portalSettingsProperty() As IDictionary(Of String, Object)

        Property PortalId() As String

        Property PortalAliasID() As String

        Property HTTPAlias() As String

        Property HomeDirectory() As String

        Property PortalName() As String

        Property AdministratorRoleName() As String

        Property Version() As String

        Property AdministratorRoleId() As String

        Property GUID() As String
        Property SuperTabId() As String
        Property SplashTabId() As String
        Property UserRegistration() As String
        Property RegisteredRoleId() As String
        Property UserTabId() As String
        Property LogoFile() As String
        Property LoginTabId() As String
        Property KeyWords() As String
        Property TimeZoneOffset() As Integer
        Property HostFee() As String
        Property HomeTabId() As String
        Property Title() As String
        Property ActiveTab() As ITabInfo
        Property RegisteredRoleName() As String
        Property HostSpace() As String
        Property HomeDirectoryMapPath() As String
        Property Email() As String
        Property FooterText() As String
        Property DefaultLanguage() As String
        Property Description() As String
        Property BackgroundFile() As String
        Property AdminTabId() As String
        Property AdministratorId() As String
        Property ExpiryDate() As DateTime


        Function GetModuleSettings(ByVal moduleId As Integer) As Hashtable
        Function GetTabModuleSettings(ByVal tabModuleId As Integer, ByVal settings As Hashtable) As Hashtable

    End Interface
End Namespace