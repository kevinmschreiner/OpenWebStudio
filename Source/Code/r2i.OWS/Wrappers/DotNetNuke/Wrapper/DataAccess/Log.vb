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
'Imports r2i.OWS.UI.Dnn
'Imports System.Web
'Imports DotNetNuke.Security

'Namespace DataAccess
'    Public Class Log
'        Implements ILog
'        Implements DotNetNuke.Entities.Modules.IActionable

'        Protected ctx As HttpContext = HttpContext.Current
'        Private currentModuleSettingsBase As BaseParentControl = DnnSingleton.GetInstance(ctx).CurrentModuleBase

'        Public Property ActiveTabId() As String Implements ILog.ActiveTabId
'            Get
'                Return CStr(currentModuleSettingsBase.PortalSettings.ActiveTab.TabID)
'            End Get
'            Set(ByVal value As String)

'            End Set
'        End Property

'        Public Property ModuleId() As String Implements ILog.ModuleId
'            Get
'                Return CStr(currentModuleSettingsBase.ModuleId)
'            End Get
'            Set(ByVal value As String)

'            End Set
'        End Property

'        Public ReadOnly Property ModulePath() As String Implements ILog.ModulePath
'            Get
'                Return currentModuleSettingsBase.ModulePath
'            End Get
'        End Property

'        Public ReadOnly Property ClientId() As String Implements ILog.ClientId
'            Get
'                Return currentModuleSettingsBase.ClientID
'            End Get
'        End Property

'        Private Function GetModuleControls(ByVal value As Integer) As List(Of DotNetNuke.Entities.Modules.ModuleControlInfo)
'            Dim ctrl As New DotNetNuke.Entities.Modules.ModuleControlController
'            Dim arr As List(Of DotNetNuke.Entities.Modules.ModuleControlInfo) = New List(Of DotNetNuke.Entities.Modules.ModuleControlInfo)
'            Dim arrModuleControls As ArrayList
'            Dim t As Type = ctrl.GetType
'            'Dim miarray As System.Reflection.MemberInfo() = ctrl.GetType().GetMember("GetModuleConrols")
'            arrModuleControls = CType(t.InvokeMember("GetModuleControls", Reflection.BindingFlags.InvokeMethod, Nothing, ctrl, New Object() {value}), ArrayList)
'            For Each objModuleControl As DotNetNuke.Entities.Modules.ModuleControlInfo In arrModuleControls
'                arr.Add(objModuleControl)
'            Next
'            Return arr
'        End Function
'        Public ReadOnly Property ModuleActions() As DotNetNuke.Entities.Modules.Actions.ModuleActionCollection Implements DotNetNuke.Entities.Modules.IActionable.ModuleActions
'            Get
'                Dim Actions As New DotNetNuke.Entities.Modules.Actions.ModuleActionCollection
'                ' base module properties
'                'Dim ctrl As New DotNetNuke.Entities.Modules.ModuleControlController

'                'ROMAIN: Generic replacement - 08/20/2007              
'                'Dim ctrls As ArrayList = GetModuleControls(Me.ModuleConfiguration.ModuleDefID) 'ctrl.GetModuleControls(CType(Me.ModuleConfiguration.ModuleDefID, Int32))
'                Dim ctrls As List(Of DotNetNuke.Entities.Modules.ModuleControlInfo) = GetModuleControls(currentModuleSettingsBase.ModuleConfiguration.ModuleDefID)

'                If Not ctrls Is Nothing AndAlso ctrls.Count > 0 Then
'                    Dim ctrli As DotNetNuke.Entities.Modules.ModuleControlInfo
'                    For Each ctrli In ctrls
'                        Select Case ctrli.ControlType
'                            Case SecurityAccessLevel.View
'                                Actions.Add(currentModuleSettingsBase.GetNextActionID, "View", ctrli.ControlKey, "", ctrli.IconFile, Url:=DotNetNuke.Common.ApplicationURL(currentModuleSettingsBase.TabId), UseActionEvent:=True, Secure:=ctrli.ControlType, Visible:=True)
'                            Case DotNetNuke.Security.SecurityAccessLevel.Admin, DotNetNuke.Security.SecurityAccessLevel.Edit, DotNetNuke.Security.SecurityAccessLevel.Host
'                                If Not ctrli.ControlKey Is Nothing AndAlso Not ctrli.ControlKey = "Settings" Then
'                                    Dim str As String
'                                    If Not ctrli.ControlTitle Is Nothing Then
'                                        str = ctrli.ControlTitle
'                                    Else
'                                        str = ctrli.ControlKey
'                                    End If

'                                    If str Is Nothing OrElse str.Length = 0 Then
'                                        'DONT ADD THIS TO THE DROP DOWN
'                                    Else
'                                        Actions.Add(currentModuleSettingsBase.GetNextActionID, str, ctrli.ControlKey, "", ctrli.IconFile, Url:=currentModuleSettingsBase.EditUrl(ctrli.ControlKey), UseActionEvent:=True, Secure:=ctrli.ControlType, Visible:=True)
'                                    End If
'                                End If
'                        End Select
'                    Next
'                End If
'                Return Actions
'            End Get
'        End Property
'    End Class
'End Namespace
