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
Imports DotNetNuke
Imports DotNetNuke.UI
Imports System.Web

Namespace DataAccess
    Public Class PortalModuleBaseUI
        Inherits System.Web.UI.UserControl
        Implements IPortalModuleBaseUI

        Private Shared ReadOnly _Instance As PortalModuleBaseUI = New PortalModuleBaseUI()
        Public Shared ReadOnly Property Instance() As PortalModuleBaseUI
            Get
                Return _Instance
            End Get
        End Property

        Private _moduleId As String
        Private _Actions As IModuleActionCollection

        Private portalModuleBaseTotallyLoaded As Boolean

        Public Sub LoadSkin(ByRef objPortalModuleBase As DotNetNuke.Entities.Modules.PortalModuleBase)
            Dim ParentSkin As Skins.Skin = Skins.Skin.GetParentSkin(objPortalModuleBase)
            'We should always have a ParentSkin, but need to make sure

            'ROMAIN: 09/26/07
            'TODO: Manage the RegisterModuleActionEvent to the parent skin
            'If Not ParentSkin Is Nothing Then
            '    'Register our EventHandler as a listener on the ParentSkin so that it may tell us 
            '    'when a menu has been clicked.
            '    ParentSkin.RegisterModuleActionEvent(objPortalModuleBase.ModuleId, AddressOf ModuleAction_Click)
            'End If
            '-------------------------------------------------------------------------------------
            Try
                objPortalModuleBase.Page.GetPostBackEventReference(objPortalModuleBase)
            Catch ex As Exception
            End Try
        End Sub

        Public Sub ModuleAction_Click(ByVal sender As Object, ByVal e As DotNetNuke.Entities.Modules.Actions.ActionEventArgs)

            'We could get much fancier here by declaring each ModuleAction with a
            'Command and then using a Select Case statement to handle the various
            'commands.

            If e.Action.Url.Length > 0 Then
                'Response.Redirect(e.Action.Url, True)
            End If
        End Sub

        Public Property ContainerControl() As System.Web.UI.Control Implements IPortalModuleBaseUI.ContainerControl
            Get
                Return DotNetNuke.Common.Globals.FindControlRecursive(Me, "ctr" & Me.moduleId.ToString())
            End Get
            Set(ByVal value As System.Web.UI.Control)

            End Set
        End Property

        Public Property moduleId() As String Implements IPortalModuleBaseUI.moduleId
            Get
                LoadUser()
                Return _moduleId
            End Get
            Set(ByVal value As String)
                LoadUser()
                _moduleId = value
            End Set
        End Property


        Private Sub LoadUser()
            If (Not portalModuleBaseTotallyLoaded) Then
                'userInfo = DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo
                portalModuleBaseTotallyLoaded = True
            End If
        End Sub

        Public Property modulePath() As String Implements IPortalModuleBaseUI.modulePath
            Get
                Return Nothing
            End Get
            Set(ByVal value As String)

            End Set
        End Property

        Public Property Actions() As IModuleActionCollection Implements IPortalModuleBaseUI.Actions
            Get
                Return _Actions
            End Get
            Set(ByVal value As IModuleActionCollection)
                _Actions = Actions
            End Set
        End Property


    End Class
End Namespace
