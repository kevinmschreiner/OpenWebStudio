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
Imports System.Reflection
Imports r2i.OWS.Framework.DataAccess

Namespace r2i.OWS.Framework.Entities
    Public Interface IModuleController
        Function CreateNewModuleInfo() As IModuleInfo
        Function GetTabModules(ByVal tabId As String, ByVal portalId As String) As Dictionary(Of Integer, IModuleInfo)
        Function GetDesktopModuleByName(ByVal friendlyName As String) As IDesktopModuleInfo
        Function GetModuleDefinitionByName(ByVal DesktopModuleID As String, ByVal FriendlyName As String) As IModuleDefinitionInfo
        Function GetModuleInfoProperties(ByVal mi As IModuleInfo) As PropertyInfo()
        Function GetModuleInfoProperty(ByVal mi As IModuleInfo, ByVal name As String) As PropertyInfo
        <Obsolete("use GetModule(string configurationId) instead, this should only be called if source provider is attempting an upgrade")> _
        Function GetModule(ByVal moduleId As String, ByVal tabId As String) As IModuleInfo
        Function GetModule(ByVal configurationId As Guid) As IModuleInfo
        Sub UpdateModule(ByVal modInfo As IModuleInfo)
        Function AddModule(ByVal modInfo As IModuleInfo) As String
        'Public Sub UpdateModuleSetting(ByVal ModuleId As Integer, ByVal SettingName As String, ByVal SettingValue As String) 
        Sub UpdateModuleSetting(ByVal moduleId As String, ByVal settingName As String, ByVal settingValue As String)

    End Interface
End Namespace