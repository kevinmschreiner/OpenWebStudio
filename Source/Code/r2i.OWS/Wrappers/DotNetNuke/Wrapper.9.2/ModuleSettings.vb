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
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Actions
Imports DotNetNuke.Entities.Modules.Communications
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls

Public Class ModuleSettings
    Inherits ModuleSettingsBase
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

    

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ctxModule = HttpContext.Current

        ows.SetParentBase(New BaseParentControl(Me))
        'DnnSingleton.GetInstance(ctxModule).CurrentPortalModuleBase = Me

        Dim slashPath As String = "/"
        If Request.ApplicationPath.EndsWith("/") Then
            slashPath = ""
        End If

        ows.SetOWSPath(Request.ApplicationPath & slashPath & "DesktopModules/OWS/")
        'THERE ARE NO HEADERS AND FOOTERS ON THE MODULE SETTINGS INTERFACE, SET THEM TO NOTHING SO THAT NO ATTEMPT IS MADE TO RETRIEVE THE H/F. 
        'INSTEAD, OWS WILL RENDER ABOVE.
        ows.SetHeaderFooter(Nothing, Nothing)
    End Sub

    Public Overrides Sub UpdateSettings()
        MyBase.UpdateSettings()
        Try
            Dim sMessageType As String = "Settings"
            Dim sMessageValue As String = "Update"

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
        Catch ex As Exception

        End Try
    End Sub
End Class
