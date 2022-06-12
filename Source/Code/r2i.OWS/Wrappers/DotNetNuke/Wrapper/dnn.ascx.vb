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

Partial Public Class Dnn
    Inherits PortalModuleBase
    Implements IActionable
    Implements IModuleCommunicator
    Implements IModuleListener
    Implements IPostBackEventHandler
    Protected ctxPortal As HttpContext
    Protected ctxModule As HttpContext
    Protected WithEvents xdnn As r2i.OWS.Wrapper.DNN.OpenControl

    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Wrapper.DNN.Entities.WrapperFactory.Create()
    End Sub
    Private Sub onModuleCommunicationEvent(ByVal Caller As Object, ByVal Text As String, ByVal Value As String, ByVal Sender As String, ByVal Target As String) Handles xdnn.ModuleCommunication
        RaiseEvent ModuleCommunication(Caller, New ModuleCommunicationEventArgs(Text, Value, Sender, Target))
    End Sub

    Public Sub New()
        Try

            ctxPortal = HttpContext.Current
        Catch ex As Exception
            DotNetNuke.Services.Exceptions.LogException(ex)
        End Try
    End Sub

#Region "Optional Interfaces"

    ''' ----------------------------------------------------------------------------- 
    ''' <summary> 
    ''' Registers the module actions required for interfacing with the portal framework 
    ''' </summary> 
    ''' <value></value> 
    ''' <returns></returns> 
    ''' <remarks></remarks> 
    ''' <history> 
    ''' </history> 
    ''' ----------------------------------------------------------------------------- 
    Public ReadOnly Property ModuleActions() As ModuleActionCollection Implements IActionable.ModuleActions
        Get
            Dim Actions As New ModuleActionCollection()
            'Actions.Add(GetNextActionID(), Localization.GetString(ModuleActionType.AddContent, LocalResourceFile), ModuleActionType.AddContent, "", "", EditUrl(), false, SecurityAccessLevel.Edit, true, false); 
            Dim major As Integer = 0
            Dim minor As Integer = 0

            GetHostVersion(major, minor)
            If (major >= 7) Then
                PopulateActionMenu(Actions, True)
            End If

            Return Actions
        End Get
    End Property

#End Region

    Private Sub PopulateActionMenu(ByVal Actions As DotNetNuke.Entities.Modules.Actions.ModuleActionCollection, Optional ByVal override As Boolean = False)
        Dim major As Integer = 0
        Dim minor As Integer = 0

        GetHostVersion(major, minor)

        If (major < 7 OrElse override) Then
            If (Actions IsNot Nothing) AndAlso Me.IsEditable Then

                Dim act As New DotNetNuke.Entities.Modules.Actions.ModuleActionCollection()
                Dim eurl As String = EditUrl().Replace("Edit", "settings")
                If eurl.IndexOf("?") >= 0 Then
                    eurl &= "&ModuleId=" & Me.ModuleId
                Else
                    eurl &= "?ModuleId=" & Me.ModuleId
                End If

                Dim pGI As New ModuleAction(GetNextActionID(), "Extended", "", "", "", "", "", False, SecurityAccessLevel.Edit, True)

                Dim cM As ModuleAction = Nothing
                'ADD CLEAR WEB CACHE 
                Dim cW As ModuleAction = Nothing
                Dim cBreak1 As ModuleAction = Nothing

                Dim lowerVersion As Boolean = override

                If major <= 3 OrElse (major = 4 AndAlso minor <= 1) Then
                    lowerVersion = True
                End If

                cM = pGI.Actions.Add(GetNextActionID(), "Clear Module Cache", "ClearModCache", "", "restore.gif", "", True, SecurityAccessLevel.Edit, True, False)
                cW = pGI.Actions.Add(GetNextActionID(), "Clear Web Cache", "ClearWebCache", "", "restore.gif", "", True, SecurityAccessLevel.Edit, True, False)
                pGI.Actions.Add(GetNextActionID(), "Package", ModuleActionType.AddContent, "", "save.gif", EditUrl("Package"), False, SecurityAccessLevel.Edit, True, True)

                If major < 7 Then
                    cBreak1 = pGI.Actions.Add(GetNextActionID(), "~", "Break1", "", "", "", False, SecurityAccessLevel.Anonymous, True, False)
                    Dim cBreak3 As ModuleAction = act.Add(GetNextActionID(), "~", "Break2", "", "", "", False, SecurityAccessLevel.Anonymous, True, False)
                End If

                Dim cH As ModuleAction = act.Add(GetNextActionID(), "Online Help", "Help", "", "Help.gif", Me.ControlPath + "Help.html", "", False, SecurityAccessLevel.Edit, True, True)
                'Dim cadmin As ModuleAction = act.Add(GetNextActionID(), "Administration", ModuleActionType.AddContent, "", "~" & Me.ModulePath + "images/publish.gif", "", Me.GetAdminUrl(), False, SecurityAccessLevel.Edit, True, True)
                'Dim cadmin As ModuleAction = act.Add(GetNextActionID(), "Administration", ModuleActionType.AddContent, "", "settings.gif", "", Me.GetAdminUrl(), False, SecurityAccessLevel.Edit, True, True)
                Dim cadmin As ModuleAction
                If major < 4 Or major > 7 Then
                    cadmin = act.Add(GetNextActionID(), "Administration", ModuleActionType.AddContent, "", "edit.gif", Me.GetAdminUrl(False), False, SecurityAccessLevel.Edit, True, True)
                Else
                    cadmin = act.Add(GetNextActionID(), "Administration", ModuleActionType.AddContent, "", "edit.gif", "javascript:" & Me.GetAdminUrl(True), False, SecurityAccessLevel.Edit, True, True)
                End If


                If lowerVersion Then
                    Actions.AddRange(pGI.Actions)
                Else
                    Actions.Insert(0, pGI)
                End If

                If major < 6 Then
                    Actions.Insert(0, cBreak1)
                End If

                Actions.Insert(0, cH)

                Actions.Insert(0, cadmin)
            End If
        End If
    End Sub

    Private Function GetAdminUrl(useJS As Boolean) As String
        Dim adminUrl As String = ControlPath & "Admin.aspx"
        ' add a direct link to edit this configuration, if one has been selected
        If Me.Settings("ConfigurationID") IsNot Nothing Then
            adminUrl = adminUrl & "#config/" & Me.Settings("ConfigurationID").ToString
        End If

        If (useJS) Then
            adminUrl = "window.open('" & adminUrl & "')==false"
        End If
        Return adminUrl
    End Function

    Public Sub ModuleAction_Click(ByVal sender As Object, ByVal e As DotNetNuke.Entities.Modules.Actions.ActionEventArgs)
        If e.Action.CommandName = "ClearModCache" Then
            ClearModuleCache()
        ElseIf e.Action.CommandName = "ClearWebCache" Then
            ClearWebCache()
        End If
    End Sub

    Private Sub ClearModuleCache()
        If (Me.Cache IsNot Nothing) Then
            If Me.Settings("ConfigurationID") IsNot Nothing Then
                Me.Cache.Remove("OWS" & Me.Settings("ConfigurationID").ToString)
            End If
        End If
        Try
            'DotNetNuke.Entities.Tabs.TabController.Instance.ClearCache(PortalId)
            TabControllerInstance.ClearCache(PortalId)
            DotNetNuke.Common.Utilities.DataCache.ClearModuleCache(TabId)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub ClearWebCache()
        If Not Me.Cache Is Nothing Then
            If Me.Cache.Count > 0 Then
                Dim enumerator As System.Collections.IDictionaryEnumerator = Me.Cache.GetEnumerator
                If Not enumerator Is Nothing Then
                    While enumerator.MoveNext()
                        Me.Cache.Remove(enumerator.Key.ToString)
                    End While
                End If
            End If
        End If
        Try
            DotNetNuke.Common.Utilities.DataCache.ClearPortalCache(PortalId, True)
        Catch ex As Exception
        End Try
    End Sub

    Private Shared Sub GetHostVersion(ByRef major As Integer, ByRef minor As Integer)
        Dim ass As System.Reflection.Assembly = System.Reflection.Assembly.GetAssembly(GetType(DotNetNuke.Common.Globals))
        minor = ass.GetName().Version.Minor
        major = ass.GetName().Version.Major
    End Sub

#Region "IModuleCommunicator Members"

    Public Event ModuleCommunication As ModuleCommunicationEventHandler Implements IModuleCommunicator.ModuleCommunication

#End Region

#Region "IModuleListener Members"

    Public Sub OnModuleCommunication(ByVal s As Object, ByVal e As ModuleCommunicationEventArgs) Implements IModuleListener.OnModuleCommunication
        Try
            Dim key As String = ""
            Dim value As String = ""
            If Not e.Type Is Nothing Then
                key = e.Type
            End If
            If Not e.Value Is Nothing Then
                Try
                    value = e.Value.ToString
                Catch ex As Exception
                End Try
            End If
            If Not xdnn.CapturedMessages.ContainsKey(key) Then
                xdnn.CapturedMessages.Add(key, value)
            End If
        Catch ex As Exception
        End Try
    End Sub

#End Region

    Public Sub RaisePostBackEvent(ByVal eventArgument As String) Implements IPostBackEventHandler.RaisePostBackEvent
        xdnn.Support_RaisePostBackEvent(eventArgument)
    End Sub
    Private Function OptionsVisible() As Boolean
        If Not Me.Settings("OpenWebStudio.Visible") Is Nothing AndAlso Me.Settings("OpenWebStudio.Visible").ToString.ToUpper = "FALSE" Then
            Return False
        End If
        Return True
    End Function
    Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Try
            'CODEGEN: This method call is required by the Web Form Designer 
            'Do not modify it using the code editor. 
            InitializeComponent()

            'LOAD HERE
            'If Utilities.ProviderDefinition.Assembly = "#" OrElse Utilities.ProviderDefinition.Assembly.Contains(".Core.") Then
            '    Dim owsap As New Utilities.AbstractProvider()
            '    owsap.Assembly = "r2i.OWS.Wrapper.DotNetNuke"
            '    owsap.ClassName = "r2i.OWS.Wrapper.DNN.Entities.DnnAbstractFactory"
            '    owsap.ConfigFile = Me.Server.MapPath("~/web.config")
            '    Utilities.ProviderDefinition = owsap
            '    Response.Redirect(Request.Url.ToString())
            'End If

            If (MyBase.Actions IsNot Nothing) AndAlso OptionsVisible() Then
                PopulateActionMenu(MyBase.Actions)
            End If

            'Put user code to initialize the page here 
            '------------------------------------------------------------------------------------ 
            '- Menu Action Handler Registration - 
            '------------------------------------------------------------------------------------ 
            'This finds a reference to the containing skin 
            Dim ParentSkin As DotNetNuke.UI.Skins.Skin = DotNetNuke.UI.Skins.Skin.GetParentSkin(Me)

            'We should always have a ParentSkin, but need to make sure 
            If (ParentSkin IsNot Nothing) Then
                'Register our EventHandler as a listener on the ParentSkin so that it may tell us 
                'when a menu has been clicked. 
                ParentSkin.RegisterModuleActionEvent(Me.ModuleId, AddressOf ModuleAction_Click)
            End If
            ' ------------------------------------------------------------------------------------- 
        Catch ex As Exception
            DotNetNuke.Services.Exceptions.LogException(ex)
        End Try
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Try
            ctxModule = HttpContext.Current
            xdnn.SetParentBase(New BaseParentControl(Me))
            DnnSingleton.GetInstance(ctxModule).CurrentModuleBase = New BaseParentControl(Me)
            'Initialize the Control Properties
            If Not TemplateSourceDirectory.EndsWith("/") Then
                xdnn.SetOWSPath(TemplateSourceDirectory & "/")
            Else
                xdnn.SetOWSPath(TemplateSourceDirectory)
            End If
        Catch ex As Exception
            DotNetNuke.Services.Exceptions.LogException(ex)
        End Try
    End Sub

    Private Function ModuleControllerInstance() As DotNetNuke.Entities.Modules.ModuleController
        'Return DotNetNuke.Entities.Modules.ModuleController.Instance()
        Dim temporary As New DotNetNuke.Entities.Modules.ModuleController()
        Return temporary
    End Function
    Private Function TabControllerInstance() As DotNetNuke.Entities.Tabs.TabController
        'Return DotNetNuke.Entities.Modules.ModuleController.Instance()
        Dim temporary As New DotNetNuke.Entities.Tabs.TabController()
        Return temporary
    End Function
End Class
