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
Imports System.Web
Imports System.Web.UI
Imports System.Security
Imports DotNetNuke.Entities.Modules.Communications
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Actions
Imports DotNetNuke.Security
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization

Partial Public Class installerDnn
    Inherits ModuleSettingsBase
    Implements IActionable


#Region " Web Form Designer Generated Code "


    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Wrapper.DNN.Entities.WrapperFactory.Create()
    End Sub

    Protected WithEvents ddlConfigLst As System.Web.UI.WebControls.DropDownList
    Protected WithEvents lblNoConfig As System.Web.UI.WebControls.Label
    Protected WithEvents lblEditLink As System.Web.UI.WebControls.Label

    Protected WithEvents ddlConfigLstSettings As System.Web.UI.WebControls.DropDownList
    Protected WithEvents lblNoConfigSettings As System.Web.UI.WebControls.Label
    Protected WithEvents lblEditLinkSettings As System.Web.UI.WebControls.Label
    Protected WithEvents chkInvisible As System.Web.UI.WebControls.CheckBox
    Protected WithEvents pnlOWS As System.Web.UI.WebControls.Panel
    Protected WithEvents ows As OpenControl

    Protected ctx As HttpContext

    'NOTE: The following placeholder declaration is required by the Web Form Designer.
    'Do not delete or move it.
    Private designerPlaceholderDeclaration As System.Object


    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: This method call is required by the Web Form Designer
        'Do not modify it using the code editor.
        InitializeComponent()
        Wrapper.DNN.Entities.WrapperFactory.Create()
        LoadCustom()
    End Sub

#End Region
    Private Sub LoadCustom()
        If Not Me.Settings("ConfigurationID.Settings") Is Nothing Then
            If Guid.Empty <> New Guid(Me.Settings("ConfigurationID.Settings").ToString()) Then
                ows = New r2i.OWS.Wrapper.DNN.OpenControl
                ows.ConfigurationId = New Guid(Me.Settings("ConfigurationID.Settings").ToString())
                Me.Controls.Add(ows)
            End If
        End If
    End Sub
    Private Function GetConfigList() As Dictionary(Of String, String)
        Dim configCtrl As Controller = New Controller
        Dim cdic As Dictionary(Of String, String) = configCtrl.GetConfigurationDictionaryList()
        Dim dic As New Dictionary(Of String, String)
        Dim enumx As Dictionary(Of String, String).Enumerator
        enumx = cdic.GetEnumerator
        dic.Add("", "<Select a Configuration>")
        While enumx.MoveNext
            dic.Add(enumx.Current.Key, enumx.Current.Value)
        End While
        Return dic
    End Function
    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ctx = HttpContext.Current
        DnnSingleton.GetInstance(ctx).CurrentModuleBase = New BaseParentControl(Me)
        If OptionsVisible() Then
            If Not IsPostBack Then

                Dim configLst As Dictionary(Of String, String) = GetConfigList()

                If configLst.Count > 0 Then
                    With ddlConfigLst
                        .DataSource = configLst
                        .DataTextField = "value"
                        .DataValueField = "key"
                        .DataBind()
                    End With
                    With ddlConfigLstSettings
                        .DataSource = configLst
                        .DataTextField = "value"
                        .DataValueField = "key"
                        .DataBind()
                    End With

                    If Not Me.Settings("ConfigurationID") Is Nothing Then
                        Dim lstItem As System.Web.UI.WebControls.ListItem
                        For Each lstItem In ddlConfigLst.Items
                            If lstItem.Value.ToUpper() = Me.Settings("ConfigurationID").ToString().ToUpper() Then
                                lstItem.Selected = True
                            Else
                                lstItem.Selected = False
                            End If
                        Next
                        lblEditLink.Visible = True
                        lblEditLink.Text = "<a href=""javascript:openconfig('" & ControlPath & "Admin.aspx','" & ddlConfigLst.ClientID & "');"">Edit</a>"
                    End If

                    If Not Me.Settings("ConfigurationID.Settings") Is Nothing Then
                        Dim lstItem As System.Web.UI.WebControls.ListItem
                        For Each lstItem In ddlConfigLstSettings.Items
                            If lstItem.Value.ToUpper() = Me.Settings("ConfigurationID.Settings").ToString().ToUpper() Then
                                lstItem.Selected = True
                            Else
                                lstItem.Selected = False
                            End If
                        Next
                        lblEditLinkSettings.Visible = True
                        lblEditLinkSettings.Text = "<a href=""javascript:openconfig('" & ControlPath & "Admin.aspx','" & ddlConfigLstSettings.ClientID & "');"">Edit</a>"
                    End If
                Else
                    ddlConfigLst.Visible = False
                    ddlConfigLstSettings.Visible = False
                    lblNoConfigSettings.Visible = False
                    lblNoConfig.Visible = True
                    lblNoConfig.Text = "There are no configurations to load. Use the <a href=""" & ControlPath & "Admin.aspx" & """>administration</a> to create a new one."
                End If
            End If
        Else
            pnlOWS.Visible = False
        End If
        Try
            ows.SetParentBase(New BaseParentControl(Me))
            DnnSingleton.GetInstance(ctx).CurrentModuleBase = New BaseParentControl(Me)
            'Initialize the Control Properties
            If Not TemplateSourceDirectory.EndsWith("/") Then
                ows.SetOWSPath(TemplateSourceDirectory & "/")
            Else
                ows.SetOWSPath(TemplateSourceDirectory)
            End If
        Catch ex As Exception
            DotNetNuke.Services.Exceptions.LogException(ex)
        End Try

    End Sub

    Public ReadOnly Property ModuleActions() As DotNetNuke.Entities.Modules.Actions.ModuleActionCollection Implements DotNetNuke.Entities.Modules.IActionable.ModuleActions
        Get
            Throw New NotImplementedException()
        End Get
    End Property

    Private Function OptionsVisible() As Boolean
        If Not Me.Settings("OpenWebStudio.Visible") Is Nothing AndAlso Me.Settings("OpenWebStudio.Visible").ToString.ToUpper = "FALSE" Then
            Return False
        End If
        Return True
    End Function
    Public Overrides Sub UpdateSettings()
        MyBase.UpdateSettings()
        If OptionsVisible() Then
            Try
                AssignConfigurations()
            Catch ex As Exception
                lblNoConfig.Visible = True
                lblNoConfig.Text = ex.Message
            End Try
        End If
        'HANDLE CUSTOM CONFIGURATION
        Try
            Dim sMessageType As String = "Settings"
            Dim sMessageValue As String = "Update"
            If Not ows Is Nothing Then
                If ows.SystemMessage_UpdateSettings <> "" Then
                    If ows.SystemMessage_UpdateSettings.Contains("|") Then
                        sMessageType = ows.SystemMessage_UpdateSettings.Split("|"c)(0)
                        sMessageValue = ows.SystemMessage_UpdateSettings.Split("|"c)(1)
                    Else
                        sMessageType = ows.SystemMessage_UpdateSettings
                        sMessageValue = ""
                    End If
                End If
                If Not ows.CapturedMessages.ContainsKey(sMessageType) Then
                    ows.CapturedMessages.Add(sMessageType, sMessageValue)
                Else
                    ows.CapturedMessages(sMessageType) = sMessageValue
                End If
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub AssignConfigurations()
        Dim mctrl As New ModuleController

        Dim curModId As Integer = Me.ModuleId
        Dim cfgCtrl As IConfigurationController = AbstractFactory.Instance.ConfigurationController
        If Not ddlConfigLst.SelectedValue Is Nothing AndAlso ddlConfigLst.SelectedValue.Length > 0 Then
            Dim configId As Guid = New Guid(ddlConfigLst.SelectedValue)
            Me.Settings("ConfigurationID") = configId.ToString
            cfgCtrl.AssignConfiguration(Convert.ToString(curModId), configId, Nothing)
        Else
            mctrl.DeleteModuleSetting(curModId, "ConfigurationID")
        End If

        If Not ddlConfigLstSettings.SelectedValue Is Nothing AndAlso ddlConfigLstSettings.SelectedValue.Length > 0 Then
            Dim configId_Settings As Guid = New Guid(ddlConfigLstSettings.SelectedValue)
            Me.Settings("ConfigurationID.Settings") = configId_Settings.ToString
            cfgCtrl.AssignConfiguration(Convert.ToString(curModId), configId_Settings, "Settings")
        Else
            mctrl.DeleteModuleSetting(curModId, "ConfigurationID.Settings")
        End If

        If chkInvisible.Checked Then
            mctrl.UpdateModuleSetting(Me.ModuleId, "OpenWebStudio.Visible", "False")

            If Not Me.ModuleSettings.ContainsKey("OpenWebStudio.Visible") Then
                Me.ModuleSettings.Add("OpenWebStudio.Visible", "False")
            Else
                Me.ModuleSettings("OpenWebStudio.Visible") = "False"
            End If
        End If
    End Sub



End Class
