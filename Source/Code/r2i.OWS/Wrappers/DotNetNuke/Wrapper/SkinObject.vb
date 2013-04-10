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

Partial Public Class SkinObject
    Inherits DotNetNuke.UI.Skins.SkinObjectBase
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
    Private Sub onModuleCommunicationEvent(ByVal Caller As Object, ByVal Text As String, ByVal Value As String, ByVal Sender As String, ByVal Target As String) Handles OWS.ModuleCommunication
        RaiseEvent ModuleCommunication(Caller, New ModuleCommunicationEventArgs(Text, Value, Sender, Target))
    End Sub

    Public Sub New()
        ctxPortal = HttpContext.Current
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

    End Sub

    Private _resourceFile As String = Nothing
    Private _resourceKey As String = Nothing
    Private _configurationId As System.Guid = System.Guid.Empty
    Private _moduleID As String = Nothing
    Private _controlType As String = "div"
    Public Property ResourceFile() As String
        Get
            Return _resourceFile
        End Get
        Set(ByVal value As String)
            _resourceFile = value
        End Set
    End Property
    Public Property ResourceKey() As String
        Get
            Return _resourceKey
        End Get
        Set(ByVal value As String)
            _resourceKey = value
        End Set
    End Property
    Public Property ConfigurationID() As System.Guid
        Get
            Return _configurationId
        End Get
        Set(ByVal value As System.Guid)
            _configurationId = value
        End Set
    End Property
    Public Property ControlType() As String
        Get
            Return _controlType
        End Get
        Set(ByVal value As String)
            _controlType = value
        End Set
    End Property
    Public Property ModuleID() As String
        Get
            If _moduleID Is Nothing Then
                Return Me.ClientID
            End If
            Return _moduleID
        End Get
        Set(ByVal value As String)
            _moduleID = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        ows = New r2i.OWS.Wrapper.DNN.OpenControl()

        ows.ID = "ows"
        Me.Controls.Add(ows)

        ctxModule = HttpContext.Current
        ows.ControlTag = ControlType
        If Not ResourceFile Is Nothing AndAlso ResourceFile.Length > 0 Then
            ows.ResourceFile = ResourceFile
        End If
        If Not ResourceKey Is Nothing AndAlso ResourceKey.Length > 0 Then
            ows.ResourceKey = ResourceKey
        End If
        If Not ModuleID Is Nothing AndAlso ModuleID.Length > 0 Then ' AndAlso IsNumeric(ModuleID) Then
            ows.ModuleId = ModuleID
            'GET THE CONFIGURATIONID FROM THE SETTINGS
            If ConfigurationID = System.Guid.Empty AndAlso IsNumeric(ModuleID) Then
                Dim hsh As Hashtable = (New DotNetNuke.Entities.Modules.ModuleController).GetModuleSettings(CInt(ModuleID))
                If hsh.ContainsKey("ConfigurationID") Then
                    ConfigurationID = New System.Guid(hsh("ConfigurationID").ToString)
                End If
            End If
        End If
        If Not ConfigurationID = System.Guid.Empty Then
            ows.ConfigurationId = ConfigurationID
        End If

        Dim slashPath As String = "/"
        If Request.ApplicationPath.EndsWith("/") Then
            slashPath = ""
        End If

        Dim bs As New BaseParentControl(Me, ResourceFile, ResourceKey, Request.ApplicationPath & slashPath & "DesktopModules/OWS/", ModuleID)
        ows.SetParentBase(bs)
        DnnSingleton.GetInstance(ctxModule).CurrentModuleBase = bs 'New BaseParentControl(Me, ResourceFile, ResourceKey, Request.ApplicationPath & slashPath & "DesktopModules/OWS/", ModuleID)

        ows.SetOWSPath(Request.ApplicationPath & slashPath & "DesktopModules/OWS/")

        LoadCommunicator()
    End Sub

    Private Sub LoadCommunicator()
        Dim MyParent As System.Web.UI.Control = Me
        Dim FoundSkin As Boolean = False
        Dim pSkin As DotNetNuke.UI.Skins.Skin = Nothing
        While Not MyParent Is Nothing
            If TypeOf MyParent Is DotNetNuke.UI.Skins.Skin Then
                FoundSkin = True
                Exit While
            End If
            MyParent = MyParent.Parent
        End While
        If FoundSkin Then
            pSkin = DirectCast(MyParent, DotNetNuke.UI.Skins.Skin)
        End If
        If Not pSkin Is Nothing Then
            Dim objComm As DotNetNuke.Entities.Modules.Communications.ModuleCommunicate
            Dim sp As Reflection.FieldInfo()
            sp = GetType(DotNetNuke.UI.Skins.Skin).GetFields(System.Reflection.BindingFlags.NonPublic Or _
                System.Reflection.BindingFlags.Instance Or _
                System.Reflection.BindingFlags.Public Or _
                System.Reflection.BindingFlags.IgnoreCase)
            If Not sp Is Nothing Then
                Dim p As Reflection.FieldInfo = Nothing
                Dim found As Boolean = False
                For Each p In sp
                    If p.FieldType.FullName = GetType(DotNetNuke.Entities.Modules.Communications.ModuleCommunicate).FullName Then
                        found = True
                        Exit For
                    End If
                Next
                If found = True Then
                    objComm = CType(p.GetValue(pSkin), DotNetNuke.Entities.Modules.Communications.ModuleCommunicate)
                    If Not objComm Is Nothing Then
                        objComm.LoadCommunicator(Me)
                    End If
                End If
            End If
        End If
    End Sub
End Class
