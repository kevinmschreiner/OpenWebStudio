''<LICENSE>
''   Open Web Studio - http://www.OpenWebStudio.com
''   Copyright (c) 2007-2008
''   by R2Integrated Inc. http://www.R2integrated.com
''      
''   Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
''   documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
''   the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
''   to permit persons to whom the Software is furnished to do so, subject to the following conditions:
''    
''   The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
''   the Software.
''   
''   This Software and associated documentation files are subject to the terms and conditions of the Open Web Studio 
''   End User License Agreement and version 2 of the GNU General Public License.
''    
''   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
''   TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
''   THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
''   CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
''   DEALINGS IN THE SOFTWARE.
''</LICENSE>
'Imports System.Web
'Imports System.Collections.Generic
'Imports r2i.OWS.Framework
'Imports r2i.OWS.Framework.Utilities
'Imports r2i.OWS.Framework.Utilities.Compatibility


'Namespace r2i.OWS
'    Public NotInheritable Class EngineSingleton

'        Private ReadOnly context As HttpContext
'        Private Const CONFIGURATIONNAME_SESSION_KEY As String = "OWS.name"
'        Private Const CONFIGURATIONID_SESSION_KEY As String = "OWS.id"

'        Private Sub New()
'        End Sub

'        Private Sub New(ByRef ctx As HttpContext)
'            context = ctx
'        End Sub


'        'Public Property CurrentConfigurationId() As Guid
'        '    Get
'        '        If Not context.Session Is Nothing Then
'        '            Return DirectCast(context.Session(CONFIGURATIONID_SESSION_KEY), Guid)
'        '        Else
'        '            Return Nothing
'        '        End If
'        '    End Get
'        '    Set(ByVal value As Guid)
'        '        If Not context.Session Is Nothing Then
'        '            context.Session(CONFIGURATIONID_SESSION_KEY) = value
'        '        End If
'        '    End Set
'        'End Property


'        Public Shared Function Instance(ByVal ctx As HttpContext) As EngineSingleton
'            Dim singleton As EngineSingleton = TryCast(ctx.Items("listXSingleton"), EngineSingleton)
'            If singleton Is Nothing Then
'                singleton = New EngineSingleton(ctx)
'                ctx.Items("listXSingleton") = singleton
'            End If
'            Return singleton
'        End Function


'        'Public Shared ReadOnly Property Instance() As EngineSingleton
'        '    Get
'        '        Return Nested.instance
'        '    End Get
'        'End Property

'        'Private Class Nested
'        '    Shared Sub New()
'        '    End Sub

'        '    Friend Shared ReadOnly instance As New EngineSingleton()
'        'End Class
'    End Class
'End Namespace


