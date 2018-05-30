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
Imports DotNetNuke.Common
Imports DotNetNuke.Entities
Imports DotNetNuke.Services
Imports r2i.OWS.Wrapper.DNN.DataAccess.Factories
Imports System.Text
Imports r2i.OWS
Imports r2i.OWS.Engine

Namespace Entities
    Public Class EngineController
        Implements IEngineController

        Private ModuleID As String = Guid.Empty.ToString().Replace("-", "")
        Private ClientID As String = ""

        'The logic should be put in a factory!
        Public Function PageId(ByVal pageName As String, ByVal portalId As String) As String Implements IEngineController.PageId
            Dim tc As New Tabs.TabController
            Dim tinfo As Tabs.TabInfo = tc.GetTabByName(pageName, CInt(portalId))
            If Not tinfo Is Nothing Then
                'pageConversion_PageName.Item(PortalSettings.PortalId.ToString & ":" & pageName) = tinfo.TabID
                tc = Nothing
                Return CStr(tinfo.TabID)
            Else : Return Nothing
            End If
        End Function
        Public Function PageName(ByVal pageId As String) As String Implements IEngineController.PageName
            Dim tc As New Tabs.TabController

            Dim tinfo As Tabs.TabInfo = tc.GetTab(CInt(pageId))
            If Not tinfo Is Nothing Then
                'pageConversion_PageID.Item(pageId) = tinfo.TabName
                tc = Nothing
                Return tinfo.TabName
            Else : Return Nothing
            End If
        End Function
        Public Function PageTitle(ByVal pageId As String) As String Implements IEngineController.PageTitle
            Dim tc As New Tabs.TabController

            Dim tinfo As Tabs.TabInfo = tc.GetTab(CInt(pageId))
            If Not tinfo Is Nothing Then
                'pageConversion_PageID.Item(pageId) = tinfo.TabName
                tc = Nothing
                Return tinfo.Title
            Else : Return Nothing
            End If
        End Function
        Public Function PageDescription(ByVal pageId As String) As String Implements IEngineController.PageDescription
            Dim tc As New Tabs.TabController

            Dim tinfo As Tabs.TabInfo = tc.GetTab(CInt(pageId))
            If Not tinfo Is Nothing Then
                'pageConversion_PageID.Item(pageId) = tinfo.TabName
                tc = Nothing
                Return tinfo.Description
            Else : Return Nothing
            End If
        End Function
        Public Function NavigateURL() As String Implements IEngineController.NavigateURL
            'Return NavigateURL(sTab)
            Return EngineFactory.Instance.NavigateURL()
        End Function
        Public Function NavigateURL(ByVal sTab As String, ByVal siteName As String) As String Implements IEngineController.NavigateURL
            'Return NavigateURL(sTab)
            Return EngineFactory.Instance.NavigateURL(sTab, CInt(siteName))
        End Function
        Public Function NavigateURL(ByVal itab As Integer) As String Implements IEngineController.NavigateURL
            'Return NavigateURL(itab)
            Return EngineFactory.Instance.NavigateURL(itab)
        End Function
        Public Function NavigateURL(ByVal itab As Integer, ByVal sControl As String) As String Implements IEngineController.NavigateURL
            ' Return NavigateURL(itab, sControl)
            Return EngineFactory.Instance.NavigateURL(itab, sControl)
        End Function
        Public Function NavigateURL(ByVal itab As Integer, ByVal sControl As String, ByVal strParameters() As String) As String Implements IEngineController.NavigateURL
            ' Return NavigateURL(itab, sControl, strParameters)
            Return EngineFactory.Instance.NavigateURL(itab, sControl, strParameters)
        End Function
        Public Function NavigateURL1(ByVal tabId As String, ByVal strParameters As System.Collections.Generic.List(Of String)) As String Implements IEngineController.NavigateURL
            Return EngineFactory.Instance.NavigateURL(CInt(tabId), "", strParameters.ToArray())
        End Function
        Public Function Dotnetnuke_GetPassword(ByVal UserObj As r2i.OWS.Framework.DataAccess.IUser, ByVal answertext As String) As String Implements IEngineController.Dotnetnuke_GetPassword
            Return EngineFactory.Instance.Dotnetnuke_GetPassword(UserObj, answertext)
        End Function
        Public Function ProviderOwner() As String Implements IEngineController.ProviderOwner
            Return EngineFactory.Instance.ProviderOwner()
        End Function
        Public Function GetConnectionString() As String Implements IEngineController.GetConnectionString
            Return EngineFactory.Instance.GetConnectionString()
        End Function
        Public Function GetApplicationPath() As String Implements IEngineController.GetApplicationPath
            Return EngineFactory.Instance.GetApplicationPath()
        End Function
        Public Function GetApplicationMapPath() As String Implements IEngineController.GetApplicationMapPath
            Return EngineFactory.Instance.GetApplicationMapPath()
        End Function
        Public Function GetLocalization(ByVal param As String) As String Implements IEngineController.GetLocalization
            Return EngineFactory.Instance.GetLocalization(param)
        End Function
        Public Function GetLocalization(ByVal sKey As String, ByVal fileName As String) As String Implements IEngineController.GetLocalization
            Return EngineFactory.Instance.GetLocalization(sKey, fileName)
        End Function
        Public Function GetRichTextEditor(ByRef Page As System.Web.UI.Page, ByVal ParentId As String, ByVal TabModuleId As String, ByVal ModuleId As String, ByVal IdNameParameter As String, ByVal Width As String, ByVal Height As String, ByVal Value As String) As String Implements IEngineController.GetRichTextEditor
            DotNetNuke.Entities.Modules.ModuleController.Instance().GetTabModule(CInt(TabModuleId))
            Dim placeholder As New DotNetNuke.Entities.Modules.PortalModuleBase()
            'Dim modInfo As New Modules.ModuleInfo()
            'modInfo.ModuleID = CInt(ModuleId)
            'modInfo.TabModuleID = CInt(TabModuleId)

            placeholder.ModuleContext.Configuration = DotNetNuke.Entities.Modules.ModuleController.Instance().GetTabModule(CInt(TabModuleId))
            placeholder.ID = ParentId

            Return EngineFactory.Instance.GetRichTextEditor(Page, placeholder, TabModuleId, ModuleId, IdNameParameter, Width, Height, Value)
        End Function
        Public Function GetOpenControlBase(ByRef Page As System.Web.UI.Page, ByVal Id As String, ByVal ModuleID As String, ByVal PageID As String, ByVal ConfigurationID As String, ByVal ResourceFile As String, ByVal ResourceKey As String, ByVal ListSource As String, ByVal ModulePath As String, ByVal BasePath As String, ByVal ControlType As String) As String Implements IEngineController.GetOpenControlBase
            Return EngineFactory.Instance.GetOpenControlBase(Page, Id, ModuleID, PageID, ConfigurationID, ResourceFile, ResourceKey, ModulePath, BasePath, ListSource, ControlType)
        End Function
        Public Sub UpdateModuleSetting(ByVal configurationId As Guid, ByVal name As String, ByVal value As String) Implements IEngineController.UpdateModuleSetting
            EngineFactory.Instance.UpdateModuleSetting(configurationId, name, value)
        End Sub
        Public Sub UpdateModuleSetting(ByVal moduleId As String, ByVal name As String, ByVal value As String) Implements IEngineController.UpdateModuleSetting
            EngineFactory.Instance.UpdateModuleSetting(moduleId, name, value)
        End Sub

        Public Sub UpdatePageModuleSetting(ByVal pagemoduleId As String, ByVal name As String, ByVal value As String) Implements IEngineController.UpdatePageModuleSetting
            EngineFactory.Instance.UpdatePageModuleSetting(pagemoduleId, name, value)
        End Sub

        Public Function GetHostSettings(ByVal parameter As String) As String Implements IEngineController.GetHostSettings
            If Not parameter Is Nothing AndAlso parameter.ToLower() = "smtppassword" Then
                Return DotNetNuke.Entities.Host.Host.SMTPPassword
            Else
                Return EngineFactory.Instance.GetHostSettings(parameter)
            End If
        End Function
        Public Function GetCache(ByVal cacheKey As String) As Object Implements IEngineController.GetCache
            Return EngineFactory.GetCache(cacheKey)
        End Function
        Public Sub SetCache(ByVal cacheKey As String, ByVal curObject As Object) Implements IEngineController.SetCache
            EngineFactory.SetCache(cacheKey, curObject)
        End Sub
        Public Function PageLookupBy(ByVal value As String, ByVal column As String) As String Implements IEngineController.PageLookupBy
            Return EngineFactory.Instance.PageLookupBy(value, column)
        End Function
        Public Function TableLookupBy(ByVal value As String, ByVal tableName As String, ByVal returnColumn As String, ByVal column As String) As String Implements IEngineController.TableLookupBy
            Return EngineFactory.Instance.TableLookupBy(value, tableName, returnColumn, column)
        End Function
        Public Sub SetCache1(ByVal cacheKey As String, ByVal curObject As Object, ByVal slidingExpiration As System.TimeSpan) Implements IEngineController.SetCache
        End Sub
        Public Function FriendlyUrlByPageID(ByVal pageId As String, ByVal path As String) As String Implements IEngineController.FriendlyUrlByPageID
            Return EngineFactory.Instance.FriendlyUrlByTabID(pageId, path)
        End Function
        Public Sub Initialize() Implements IEngineController.Initialize

        End Sub        
        Public Sub RemoveCache(ByVal cacheKey As String) Implements IEngineController.RemoveCache

        End Sub
    End Class
End Namespace

