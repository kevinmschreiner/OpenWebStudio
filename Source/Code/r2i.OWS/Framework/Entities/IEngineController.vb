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
Imports System.Collections.Generic
Imports System.Data
Imports r2i.OWS.Framework.DataAccess

Namespace r2i.OWS.Framework.Entities
    'misnamed: should be name like PageController 
    Public Interface IEngineController
        Sub Initialize()

        Function PageId(ByVal pageName As String, ByVal portalId As String) As String
        Function PageName(ByVal pageId As String) As String
        Function PageTitle(ByVal pageId As String) As String
        Function PageDescription(ByVal pageId As String) As String


        'Move to Navigation Controller 
        Function NavigateURL() As String
        Function NavigateURL(ByVal tabname As String, ByVal siteId As String) As String
        Function NavigateURL(ByVal tabId As String, ByVal strParameters As List(Of String)) As String
        Function NavigateURL(ByVal itab As Integer) As String
        Function NavigateURL(ByVal itab As Integer, ByVal sControl As String) As String
        Function NavigateURL(ByVal itab As Integer, ByVal sControl As String, ByVal strParameters As String()) As String

        '[Obsolete("FriendlyUrlByTabID")] 
        'string Dotnetnuke_FriendlyUrl_ByTabID(string tabId, string path); 
        '[Obsolete("Use PageLookupBy instead")] 
        'int Dotnetnuke_TabLookupBy(string value, string column); 
        '[Obsolete("Use TableLookupBy instead")] 
        'string Dotnetnuke_TableLookupBy(string value, string tableName, string returnColumn, string column); 
        Function Dotnetnuke_GetPassword(ByVal UserObj As IUser, ByVal answertext As String) As String

        Function FriendlyUrlByPageID(ByVal pageId As String, ByVal path As String) As String
        Function PageLookupBy(ByVal value As String, ByVal column As String) As String
        Function TableLookupBy(ByVal value As String, ByVal tableName As String, ByVal returnColumn As String, ByVal column As String) As String

        Function ProviderOwner() As String

        Function GetConnectionString() As String

        'TODO: move to Application Path 
        Function GetApplicationPath() As String
        Function GetApplicationMapPath() As String

        Function GetLocalization(ByVal param As String) As String
        Function GetLocalization(ByVal sKey As String, ByVal fileName As String) As String

        Sub UpdateModuleSetting(ByVal moduleId As String, ByVal name As String, ByVal value As String)
        Sub UpdateModuleSetting(ByVal configurationId As Guid, ByVal name As String, ByVal value As String)
        Sub UpdatePageModuleSetting(ByVal pagemoduleId As String, ByVal name As String, ByVal value As String)

        Function GetHostSettings(ByVal parameter As String) As String
        Function GetRichTextEditor(ByRef Page As System.Web.UI.Page, ParentID As String, ByVal TabModuleId As String, ByVal ModuleId As String, ByVal IdNameParameter As String, ByVal Width As String, ByVal Height As String, ByVal Value As String) As String
        Function GetOpenControlBase(ByRef Page As System.Web.UI.Page, ByVal Id As String, ByVal ModuleID As String, ByVal PageID As String, ByVal ConfigurationID As String, ByVal ResourceFile As String, ByVal ResourceKey As String, ByVal ListSource As String, ByVal ModulePath As String, ByVal BasePath As String, ByVal ControlType As String) As String

        'Public Shared Function GetCache(ByVal CacheKey As String) As Object 
        'static object GetCache(string cacheKey); 
        'TODO: Use static for the GetCache 
        Function GetCache(ByVal cacheKey As String) As Object
        'Public Shared Sub SetCache(ByVal CacheKey As String, ByVal objObject As Object) 
        'TODO: Use static for the SetCache 
        Sub SetCache(ByVal cacheKey As String, ByVal curObject As Object)
        Sub SetCache(ByVal cacheKey As String, ByVal curObject As Object, ByVal slidingExpiration As TimeSpan)
        Sub RemoveCache(ByVal cacheKey As String)

    End Interface

End Namespace