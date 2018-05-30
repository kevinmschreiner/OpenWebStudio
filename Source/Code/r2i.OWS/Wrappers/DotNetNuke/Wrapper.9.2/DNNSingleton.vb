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
Imports DotNetNuke.Entities.Modules

''
'[Obsolete("This is available in the r2i.OWS.Core.DNN project", true)] 
Public NotInheritable Class DnnSingleton
    Private ReadOnly context As HttpContext
    'Private ReadOnly CURRENTDNNPAGE_SESSION_KEY As String = "CurrentDnnPage"
    'Private ReadOnly CURRENTPORTALMODULEBASE_SESSION_KEY As String = "CurrentPortalModuleBase"
    'Private ReadOnly CURRENTMODULESETTINGSBASE_SESSION_KEY As String = "CurrentModuleSettingsBase"
    Private _currentUser As String
    Private _currentPortalModuleBase As BaseParentControl

    ''' <summary> 
    ''' Initializes a new instance of the <see cref="DnnSingleton"/> class. 
    ''' </summary> 
    ''' <param name="ctx">The CTX.</param> 
    Private Sub New(ByVal ctx As HttpContext)
        context = ctx
    End Sub

    Public Property CurrentUser() As String
        Get
            'Return DirectCast(context.Items(CURRENTDNNPAGE_SESSION_KEY), String)
            Return _currentUser
        End Get
        Set(ByVal value As String)
            _currentUser = value
            'context.Items(CURRENTDNNPAGE_SESSION_KEY) = value
        End Set
    End Property

    'Public Property CurrentPortalModuleBase() As PortalModuleBase
    '    Get
    '        Return _currentPortalModuleBase
    '        'Return DirectCast(context.Items(CURRENTPORTALMODULEBASE_SESSION_KEY), PortalModuleBase)
    '    End Get
    '    Set(ByVal value As PortalModuleBase)
    '        _currentPortalModuleBase = value
    '        'context.Items(CURRENTPORTALMODULEBASE_SESSION_KEY) = value
    '    End Set
    'End Property

    'Public Property CurrentModuleSettingsBase() As ModuleSettingsBase
    '    Get
    '        'Return DirectCast(context.Items(CURRENTMODULESETTINGSBASE_SESSION_KEY), ModuleSettingsBase)
    '        Return _currentModuleSettingsBase
    '    End Get
    '    Set(ByVal value As ModuleSettingsBase)
    '        'context.Items(CURRENTMODULESETTINGSBASE_SESSION_KEY) = value
    '        _currentModuleSettingsBase = value
    '    End Set
    'End Property

    Public Property CurrentModuleBase() As BaseParentControl
        Get
            Return _currentPortalModuleBase
        End Get
        Set(ByVal value As BaseParentControl)
            _currentPortalModuleBase = value
        End Set
    End Property


    Public Shared Function GetInstance(ByVal ctx As HttpContext) As DnnSingleton
        Dim singleton As DnnSingleton = TryCast(ctx.Items("DnnSingleton"), DnnSingleton)
        If singleton Is Nothing Then
            singleton = New DnnSingleton(ctx)
            ctx.Items("DnnSingleton") = singleton
        End If
        Return singleton
    End Function
End Class