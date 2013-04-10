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

Partial Public Class [Module]
    Inherits PortalModuleBase
    Implements IActionable
    Implements IModuleCommunicator
    Implements IModuleListener
    Implements IPostBackEventHandler
    Protected ctxPortal As HttpContext
    Protected ctxModule As HttpContext
    Protected WithEvents ows As r2i.OWS.Wrapper.DNN.OpenControl

    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Wrapper.DNN.Entities.WrapperFactory.Create()
    End Sub
    Private Sub onModuleCommunicationEvent(ByVal Caller As Object, ByVal Text As String, ByVal Value As String, ByVal Sender As String, ByVal Target As String) Handles ows.ModuleCommunication
        RaiseEvent ModuleCommunication(Caller, New ModuleCommunicationEventArgs(Text, Value, Sender, Target))
    End Sub

    Public Sub New()
        ctxPortal = HttpContext.Current
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
            Return Actions
        End Get
    End Property

#End Region

    Private Sub RecursivePopulateActionMenu(ByVal Actions As DotNetNuke.Entities.Modules.Actions.ModuleActionCollection, ByRef mItems As MenuItemCollection)
        If Not mItems Is Nothing Then
            Dim mnui As MenuItem
            For Each mnui In mItems
                'add the item
                Dim mnuA As New ModuleAction(GetNextActionID)
                If Not mnui.ClientScript Is Nothing Then
                    mnuA.ClientScript = mnui.ClientScript
                End If
                If Not mnui.CommandArgument Is Nothing Then
                    mnuA.CommandArgument = mnui.CommandArgument
                End If
                If Not mnui.CommandName Is Nothing Then
                    mnuA.CommandName = mnui.CommandName
                End If
                If Not mnui.Icon Is Nothing Then
                    mnuA.Icon = mnui.Icon
                End If
                If Not mnui.ID = 0 Then
                    mnuA.ID = mnui.ID
                End If
                mnuA.NewWindow = mnui.NewWindow
                mnuA.Secure = mnui.Secure
                mnuA.UseActionEvent = mnui.UseActionEvent
                mnuA.Visible = mnui.Visible
                If Not mnui.Title Is Nothing Then
                    mnuA.Title = mnui.Title
                End If
                If Not mnui.Url Is Nothing AndAlso mnui.Url.Length > 0 Then
                    mnuA.Url = mnui.Url
                End If
                If Not mnui.ControlKey Is Nothing AndAlso mnui.ControlKey.Length > 0 Then
                    mnuA.Url = EditUrl(mnui.ControlKey)
                End If
                Actions.Add(mnuA)
                If Not mnui.MenuItems Is Nothing AndAlso mnui.MenuItems.Count > 0 Then
                    RecursivePopulateActionMenu(mnuA.Actions, mnui.MenuItems)
                End If
            Next
        End If
    End Sub
    Private Sub PopulateActionMenu(ByVal Actions As DotNetNuke.Entities.Modules.Actions.ModuleActionCollection)
        'POPULATE THIS AND CACHE IT BASED ON THE OLD STYLE METHODS
        If Actions IsNot Nothing Then
            If Not ows Is Nothing Then
                RecursivePopulateActionMenu(Actions, ows.MenuItems)
            End If
        End If
    End Sub

    Public Sub ModuleAction_Click(ByVal sender As Object, ByVal e As DotNetNuke.Entities.Modules.Actions.ActionEventArgs)
        'We could get much fancier here by declaring each ModuleAction with a
        'Command and then using a Select Case statement to handle the various
        'commands.
        Dim handled As Boolean = False
        If Not e.Action.CommandName Is Nothing AndAlso e.Action.CommandName.Length > 0 Then
            Select Case e.Action.CommandName
                Case "RETURN"
                    handled = True
                    Response.Redirect(Me.PortalSettings.ActiveTab.FullUrl, True)
            End Select
        Else
            If e.Action.Url.Length > 0 Then
                handled = True
                Response.Redirect(e.Action.Url, True)
            End If
        End If
        If Not handled Then
            If Not e.Action.CommandArgument Is Nothing AndAlso e.Action.CommandArgument.Length > 0 Then
                ows.Support_RaisePostBackEvent(e.Action.CommandArgument)
            End If
        End If
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
            If Not ows.CapturedMessages.ContainsKey(key) Then
                ows.CapturedMessages.Add(key, value)
            End If
        Catch ex As Exception
        End Try
    End Sub

#End Region

    Public Sub RaisePostBackEvent(ByVal eventArgument As String) Implements IPostBackEventHandler.RaisePostBackEvent
        ows.Support_RaisePostBackEvent(eventArgument)
    End Sub

    Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        'CODEGEN: This method call is required by the Web Form Designer 
        'Do not modify it using the code editor. 
        InitializeComponent()

        'If Utilities.ProviderDefinition.Assembly = "#" OrElse Utilities.ProviderDefinition.Assembly.Contains(".Core.") Then
        '    Dim owsap As New Utilities.AbstractProvider()
        '    owsap.Assembly = "r2i.OWS.Wrapper.DotNetNuke"
        '    owsap.ClassName = "r2i.OWS.Wrapper.DNN.Entities.DnnAbstractFactory"
        '    owsap.ConfigFile = Me.Server.MapPath("~/web.config")
        '    Utilities.ProviderDefinition = owsap
        '    Response.Redirect(Request.Url.ToString())
        'End If
        'LOAD HERE

        'Initialize the Control Properties

        If (MyBase.Actions IsNot Nothing) Then
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
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        ctxModule = HttpContext.Current
        DnnSingleton.GetInstance(ctxModule).CurrentModuleBase = New BaseParentControl(Me)
        Dim slashPath As String = "/"
        If Request.ApplicationPath.EndsWith("/") Then
            slashPath = ""
        End If
        ows.SetOWSPath(Request.ApplicationPath & slashPath & "DesktopModules/OWS/")
    End Sub
End Class
