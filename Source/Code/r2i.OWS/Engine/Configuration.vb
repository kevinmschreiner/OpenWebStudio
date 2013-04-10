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
'Imports r2i.OWS.Framework.Entities

'Namespace r2i.OWS.Framework.DataAccess
'    Public Class Configuration
'        Inherits System.Web.UI.Control
'        Implements IConfiguration
'        Private _ResourceFile As String
'        Private _ResourceKey As String
'        Private _HeaderKey As String
'        Private _FooterKey As String
'        Private _TabID As String
'        Private _ModuleID As String
'        Private _ClientId As String
'        Private _ForceAnchorTag As Boolean
'        Private _SystemMessage_UpdateSettings As String
'        Public Property ResourceFile() As String Implements IConfiguration.ResourceFile
'            Get
'                Return _ResourceFile
'            End Get
'            Set(ByVal Value As String)
'                _ResourceFile = Value
'            End Set
'        End Property
'        Public Property ForceAnchorTag() As Boolean Implements IConfiguration.ForceAnchorTag
'            Get
'                Return _ForceAnchorTag
'            End Get
'            Set(ByVal Value As Boolean)
'                _ForceAnchorTag = Value
'            End Set
'        End Property
'        Public Property ResourceKey() As String Implements IConfiguration.ResourceKey
'            Get
'                Return _ResourceKey
'            End Get
'            Set(ByVal Value As String)
'                _ResourceKey = Value
'            End Set
'        End Property
'        Public Property HeaderKey() As String Implements IConfiguration.HeaderKey
'            Get
'                Return _HeaderKey
'            End Get
'            Set(ByVal Value As String)
'                _HeaderKey = Value
'            End Set
'        End Property
'        Public Property FooterKey() As String Implements IConfiguration.FooterKey
'            Get
'                Return _FooterKey
'            End Get
'            Set(ByVal Value As String)
'                _FooterKey = Value
'            End Set
'        End Property
'        Public Property TabID() As String Implements IConfiguration.TabId
'            Get
'                Return CStr(_TabID)
'            End Get
'            Set(ByVal Value As String)
'                _TabID = Value
'            End Set
'        End Property
'        Public Property ModuleID() As String Implements IConfiguration.ModuleId
'            Get
'                Return _ModuleID
'            End Get
'            Set(ByVal Value As String)
'                _ModuleID = Value
'            End Set
'        End Property
'        Public Property SystemMessage_UpdateSettings() As String Implements IConfiguration.SystemMessage_UpdateSettings
'            Get
'                Return _SystemMessage_UpdateSettings
'            End Get
'            Set(ByVal Value As String)
'                _SystemMessage_UpdateSettings = Value
'            End Set
'        End Property
'        Public ReadOnly Property ID1() As String Implements IConfiguration.ID
'            Get
'                Return ID
'            End Get
'        End Property
'    End Class
'End Namespace