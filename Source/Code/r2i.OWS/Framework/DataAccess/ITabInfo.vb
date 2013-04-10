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
Imports System.Collections.Generic

Namespace r2i.OWS.Framework.DataAccess
    Public Interface ITabInfo
        Property AdministratorRoles() As String
        Property AuthorizedRoles() As String
        Property BreadCrumbs() As System.Collections.ArrayList
        Property ContainerPath() As String
        Property ContainerSrc() As String
        Property Description() As String
        Property DisableLink() As Boolean
        Property EndDate() As Date
        ReadOnly Property FullUrl() As String
        Property HasChildren() As Boolean
        Property IconFile() As String
        ReadOnly Property IsAdminTab() As Boolean
        Property IsDeleted() As Boolean
        Property IsSuperTab() As Boolean
        Property IsVisible() As Boolean
        Property KeyWords() As String
        Property Level() As Integer
        Property Modules() As System.Collections.ArrayList
        Property PageHeadText() As String
        Property Panes() As System.Collections.ArrayList
        Property ParentId() As String
        Property PortalID() As String
        Property RefreshInterval() As Integer
        Property SkinPath() As String
        Property SkinSrc() As String
        Property StartDate() As Date
        Property TabID() As String
        Property TabOrder() As Integer
        Property TabPath() As String
        Property TabName() As String
        Property TabPermissions() As ITabPermissionCollection
        Property Title() As String
        Property Url() As String

        Sub Load(ByVal obj As Object)
        Function Save() As Object
    End Interface
End Namespace