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
    Public Class TabController
        Implements ITabController

        'TODO: FINISH TABINFO IMPLEMENTATION
        Public Function CreateNewTabInfo() As ITabInfo Implements ITabController.CreateNewTabInfo
            Return CType(TabFactory.Instance.CreateNewTabInfo, ITabInfo)
        End Function

        Public Function GetTabs(ByVal portalId As String) As ArrayList Implements ITabController.GetTabs
            Return TabFactory.Instance.GetTabs(portalId)
        End Function

        Public Function GetTabInfoProperties(ByVal ti As ITabInfo) As System.Reflection.PropertyInfo() Implements ITabController.GetTabInfoProperties
            Dim tbInfo As DataAccess.TabInfo = CType(ti, DataAccess.TabInfo)
            Return TabFactory.Instance.GetTabInfoProperties(CType(tbInfo.Save, DotNetNuke.Entities.Tabs.TabInfo))
        End Function

        Public Function GetTabInfoProperty(ByVal ti As ITabInfo, ByVal name As String) As System.Reflection.PropertyInfo Implements ITabController.GetTabInfoProperty
            Return TabFactory.Instance.GetTabInfoProperty(ti, name)
        End Function

        Public Function GetTab(ByVal moduleParentId As String) As ITabInfo Implements ITabController.GetTab
            Return TabFactory.Instance.GetTab(moduleParentId)
        End Function

        Public Function GetTabByName(ByVal tabName As String, ByVal portalId As String) As ITabInfo Implements ITabController.GetTabByName
            Return CType(TabFactory.Instance.GetTabByName(tabName, portalId), ITabInfo)
        End Function

        Public Function GetTabByName(ByVal tabName As String, ByVal portalId As String, ByVal parentId As String) As ITabInfo Implements ITabController.GetTabByName
            Return CType(TabFactory.Instance.GetTabByName(tabName, portalId, parentId), ITabInfo)
        End Function

        Public Function AddTab(ByVal tabInfo As ITabInfo) As Integer Implements ITabController.AddTab
            Return TabFactory.Instance.AddTab(CType(CType(tabInfo, DataAccess.TabInfo).Save(), DotNetNuke.Entities.Tabs.TabInfo))
        End Function

        Public Sub UpdateTab(ByVal tabInfo As ITabInfo) Implements ITabController.UpdateTab

        End Sub

        Public Function GetPortalTabs() As System.Collections.ArrayList Implements ITabController.GetPortalTabs
            Return TabFactory.Instance.GetPortalTabs()
        End Function
    End Class
End Namespace

