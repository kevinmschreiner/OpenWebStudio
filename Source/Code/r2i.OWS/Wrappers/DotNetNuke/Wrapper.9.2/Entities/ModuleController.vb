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
Imports r2i.OWS.Framework.DataAccess
Namespace Entities
    Public Class ModuleController
        Implements IModuleController

        Public Function CreateNewModuleInfo() As IModuleInfo Implements IModuleController.CreateNewModuleInfo
            Return ModuleFactory.Instance.CreateNewModuleInfo
        End Function

        Public Function GetTabModules(ByVal tabId As String, ByVal portalId As String) As System.Collections.Generic.Dictionary(Of Integer, IModuleInfo) Implements IModuleController.GetTabModules
            Dim dic As New Dictionary(Of Integer, DotNetNuke.Entities.Modules.ModuleInfo)
            Dim dicInterface As Dictionary(Of Integer, IModuleInfo) = New Dictionary(Of Integer, IModuleInfo)
            dic = ModuleFactory.Instance.GetTabModules(tabId, portalId)
            For Each kvp As KeyValuePair(Of Integer, DotNetNuke.Entities.Modules.ModuleInfo) In dic
                dicInterface.Add(kvp.Key, New DataAccess.ModuleInfo(CType(kvp.Value.ModuleID, String), CType(kvp.Value.TabID, String)))
            Next
            Return dicInterface

            'Return ModuleFactory.Instance.GetTabModules(tabId)
        End Function

        Public Function GetDesktopModuleByName(ByVal friendlyName As String) As IDesktopModuleInfo Implements IModuleController.GetDesktopModuleByName
            Return ModuleFactory.Instance.GetDesktopModuleByName(friendlyName)
        End Function

        Public Function GetModuleDefinitionByName(ByVal desktopModuleId As String, ByVal friendlyName As String) As IModuleDefinitionInfo Implements IModuleController.GetModuleDefinitionByName
            Return ModuleFactory.Instance.GetModuleDefinitionByName(desktopModuleId, friendlyName)
        End Function

        Public Function GetModuleInfoProperties(ByVal mi As IModuleInfo) As System.Reflection.PropertyInfo() Implements IModuleController.GetModuleInfoProperties
            Return ModuleFactory.Instance.GetModuleInfoProperties(mi)
        End Function

        Public Function GetModuleInfoProperty(ByVal mi As IModuleInfo, ByVal name As String) As System.Reflection.PropertyInfo Implements IModuleController.GetModuleInfoProperty
            Return ModuleFactory.Instance.GetModuleInfoProperty(mi, name)
        End Function

        Public Function GetModule(ByVal moduleId As String, ByVal tabId As String) As IModuleInfo Implements IModuleController.GetModule
            Return ModuleFactory.Instance.GetModule(moduleId, tabId)
        End Function

        Public Function GetModule1(ByVal configurationId As Guid) As IModuleInfo Implements IModuleController.GetModule
            Return ModuleFactory.Instance.GetModule(configurationId)
        End Function

        Public Sub UpdateModule(ByVal modInfo As IModuleInfo) Implements IModuleController.UpdateModule
            ModuleFactory.Instance.UpdateModule(CType(modInfo, DataAccess.ModuleInfo))
        End Sub

        Public Function AddModule(ByVal modInfo As IModuleInfo) As String Implements IModuleController.AddModule
            Return ModuleFactory.Instance.AddModule(CType(modInfo, DataAccess.ModuleInfo))
        End Function

        Public Sub UpdateModuleSetting(ByVal moduleId As String, ByVal settingName As String, ByVal settingValue As String) Implements IModuleController.UpdateModuleSetting
            ModuleFactory.Instance.UpdateModuleSetting(CInt(moduleId), settingName, settingValue)
        End Sub


    End Class

End Namespace
