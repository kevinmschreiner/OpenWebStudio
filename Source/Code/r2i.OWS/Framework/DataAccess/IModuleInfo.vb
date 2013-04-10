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
Namespace r2i.OWS.Framework.DataAccess
    Public Interface IModuleInfo
        Property Alignment() As String
        Property AllModules() As Boolean
        Property AllTabs() As Boolean
        Property AuthorizedEditRoles() As String
        Property AuthorizedRoles() As String
        Property AuthorizedViewRoles() As String
        Property Border() As String
        Property BusinessControllerClass() As String
        Property CacheTime() As Integer
        Property Color() As String
        Property ContainerPath() As String
        Property ContainerSrc() As String
        Property ControlSrc() As String
        Property ControlTitle() As String
        Property Description() As String
        Property DesktopModuleID() As String
        Property DisplayPrint() As Boolean
        Property DisplaySyndicate() As Boolean
        Property DisplayTitle() As Boolean
        Property EndDate() As Date
        Property Footer() As String
        Property FriendlyName() As String
        Property Header() As String
        Property HelpUrl() As String
        Property IconFile() As String
        Property InheritViewPermissions() As Boolean
        Property IsAdmin() As Boolean
        Property IsDefaultModule() As Boolean
        Property IsDeleted() As Boolean
        ReadOnly Property IsPortable() As Boolean
        Property IsPremium() As Boolean
        ReadOnly Property IsSearchable() As Boolean
        ReadOnly Property IsUpgradeable() As Boolean
        Property ModuleControlId() As String
        Property ModuleDefID() As String
        Property ModuleID() As String
        Property ModuleOrder() As Integer
        Property ModulePermissions() As IModulePermissionCollection
        Property ModuleTitle() As String
        Property PaneModuleCount() As Integer
        Property PaneModuleIndex() As Integer
        Property PaneName() As String
        Property PortalID() As String
        Property StartDate() As Date
        Property SupportedFeatures() As Integer
        Property TabID() As String
        Property TabModuleID() As String
        Property Version() As String

        Sub Load(ByVal obj As Object)
        Function Save() As Object
    End Interface
End Namespace