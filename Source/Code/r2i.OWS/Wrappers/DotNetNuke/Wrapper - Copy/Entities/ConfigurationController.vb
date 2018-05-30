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
Imports System.Web.UI.HtmlControls
Imports r2i.OWS.Wrapper.DNN.DataAccess.Factories

Namespace Entities
    Class ConfigurationController
        Implements Framework.Entities.IConfigurationController

        'Priate _Modules As List(Of x-ListingSkin)

        Public Sub LoadConfiguration_Environment() Implements Framework.Entities.IConfigurationController.LoadConfiguration_Environment
            Throw New NotImplementedException()
        End Sub

        Public Sub LoadConfigurations_Environment(ByVal parentObject As System.Web.UI.HtmlControls.HtmlControl) Implements Framework.Entities.IConfigurationController.LoadConfigurations_Environment
            'Dim ds As DataSet = SqlDataProvider.ExecuteDataSet("SELECT ConfigurationID, ConfigurationName FROM Settings")
            'Dim cbo As HtmlSelect = CType(parentObject, HtmlSelect)

            'If Not ds Is Nothing AndAlso ds.Tables.Count > 0 Then
            '    For Each dr As DataRow In ds.Tables(0).Rows
            '        cbo.Items.Add(New System.Web.UI.WebControls.ListItem(CStr(dr.Item("ConfigurationName")), CType(dr.Item("ConfigurationID"), Guid).ToString()))
            '    Next
            'End If
        End Sub

        Public Sub LoadConfigurations_Environment(ByVal ctx As System.Web.HttpContext, ByVal parentObject As System.Web.UI.HtmlControls.HtmlControl) Implements Framework.Entities.IConfigurationController.LoadConfigurations_Environment

            Throw New NotImplementedException()
        End Sub

        Public Sub LoadConfigurations_Environment1(ByVal page As System.Web.UI.Page, ByVal ctx As System.Web.HttpContext, ByVal parentObject As System.Web.UI.HtmlControls.HtmlControl) Implements Framework.Entities.IConfigurationController.LoadConfigurations_Environment
            'Dim ctl As Object
            'Dim itemIndex As Integer = 0
            'For Each ctl In page.Controls
            '    If TypeOf ctl Is Configuration Then
            '        'ROMAIN:08/23/2007
            '        'NOTE: Remove cast
            '        'Dim cfgModule As Configuration = CType(ctl, Configuration)
            '        Dim cfgModule As Configuration = ctl
            '        Dim skObj As xList.x-ListingSkin = LoadModuleConfiguration(cfgModule, itemIndex)
            '        If Not skObj Is Nothing Then
            '            _Modules.Add(skObj)
            '        End If
            '        itemIndex += 1
            '    End If
            'Next
            'Dim skObjt As x-List.x-ListingSkin
            'For Each skObjt In _Modules
            '    skObjt.DataBind()
            '    If Not _m_RaisePostBackEvent Is Nothing Then
            '        Dim mcea As New DotNetNuke.Entities.Modules.Communications.ModuleCommunicationEventArgs
            '        mcea.Sender = Request.Form.Item("__EVENTTARGET")
            '        mcea.Target = Request.Form.Item("__EVENTTARGET")
            '        Dim str() As String = _m_RaisePostBackEvent.Split(","c)
            '        If str.Length > 1 Then
            '            mcea.Type = str(1)
            '            mcea.Value = str(0)
            '        End If

            '        skObjt.Add_OnModuleCommunication(mcea)
            '    End If
            '    parentObject.Controls.Add(skObjt)
            'Next
        End Sub

        Public Sub SaveConfiguration() Implements Framework.Entities.IConfigurationController.SaveConfiguration
            'If Not _Modules Is Nothing Then
            '    Dim obj As Object
            '    Dim objIndex As Integer = 0
            '    For Each obj In _Modules
            '        If TypeOf obj Is x-List.x-ListingSkin Then
            '            Dim skObj As x-List.x-ListingSkin = obj
            '            Dim sl As New SortedList(Of String, String)
            '            Dim mtype As String = "Settings"
            '            Dim mvalue As String = "Update"
            '            If Not skObj.SystemMessage_Update Is Nothing AndAlso skObj.SystemMessage_Update.Length > 0 Then
            '                mtype = ""
            '                mvalue = ""
            '                Dim lindex As Integer = skObj.SystemMessage_Update.IndexOf("|")
            '                If lindex > 0 Then
            '                    mtype = skObj.SystemMessage_Update.Substring(0, lindex)
            '                Else
            '                    mtype = skObj.SystemMessage_Update
            '                End If
            '                If lindex < skObj.SystemMessage_Update.Length Then
            '                    mvalue = skObj.SystemMessage_Update.Substring(lindex + 1)
            '                End If
            '            End If
            '            sl.Add(mtype, mvalue)
            '            skObj.ExecuteMessages(sl)
            '            objIndex += 1
            '        End If
            '    Next
            'End If
        End Sub

        Public Sub AssignConfiguration(ByVal moduleId As String, ByVal configurationId As System.Guid, ByVal Key As String) Implements IConfigurationController.AssignConfiguration
            ConfigurationFactory.Instance.AssignConfiguration(moduleId, configurationId, Key)
        End Sub
    End Class
End Namespace

