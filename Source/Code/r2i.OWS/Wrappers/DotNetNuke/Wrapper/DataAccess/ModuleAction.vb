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
Namespace DataAccess
    Public Class ModuleAction
        Implements IModuleAction
        ' Fields
        'Private _actions As ModuleActionCollection
        'Private _clientScript As String
        'Private _commandArgument As String
        'Private _commandName As String
        'Private _icon As String
        'Private _id As Integer
        'Private _newwindow As Boolean
        'Private _secure As SecurityAccessLevel
        'Private _title As String
        'Private _url As String
        'Private _useActionEvent As Boolean
        'Private _visible As Boolean

        ' Methods
        'Public Sub New(ByVal ID As Integer)
        '    Me.New(ID, "", "", "", "", "", _
        '     "", False, SecurityAccessLevel.Anonymous, True, False)
        'End Sub

        'Public Sub New(ByVal ID As Integer, ByVal Title As String, ByVal CmdName As String)
        '    Me.New(ID, Title, CmdName, "", "", "", _
        '     "", False, SecurityAccessLevel.Anonymous, True, False)
        'End Sub

        'Public Sub New(ByVal ID As Integer, ByVal Title As String, ByVal CmdName As String, ByVal CmdArg As String)
        '    Me.New(ID, Title, CmdName, CmdArg, "", "", _
        '     "", False, SecurityAccessLevel.Anonymous, True, False)
        'End Sub

        'Public Sub New(ByVal ID As Integer, ByVal Title As String, ByVal CmdName As String, ByVal CmdArg As String, ByVal Icon As String)
        '    Me.New(ID, Title, CmdName, CmdArg, Icon, "", _
        '     "", False, SecurityAccessLevel.Anonymous, True, False)
        'End Sub

        'Public Sub New(ByVal ID As Integer, ByVal Title As String, ByVal CmdName As String, ByVal CmdArg As String, ByVal Icon As String, ByVal Url As String)
        '    Me.New(ID, Title, CmdName, CmdArg, Icon, Url, _
        '     "", False, SecurityAccessLevel.Anonymous, True, False)
        'End Sub

        'Public Sub New(ByVal ID As Integer, ByVal Title As String, ByVal CmdName As String, ByVal CmdArg As String, ByVal Icon As String, ByVal Url As String, _
        ' ByVal ClientScript As String)
        '    Me.New(ID, Title, CmdName, CmdArg, Icon, Url, _
        '     ClientScript, False, SecurityAccessLevel.Anonymous, True, False)
        'End Sub

        'Public Sub New(ByVal ID As Integer, ByVal Title As String, ByVal CmdName As String, ByVal CmdArg As String, ByVal Icon As String, ByVal Url As String, _
        ' ByVal ClientScript As String, ByVal UseActionEvent As Boolean)
        '    Me.New(ID, Title, CmdName, CmdArg, Icon, Url, _
        '     ClientScript, UseActionEvent, SecurityAccessLevel.Anonymous, True, False)
        'End Sub

        'Public Sub New(ByVal ID As Integer, ByVal Title As String, ByVal CmdName As String, ByVal CmdArg As String, ByVal Icon As String, ByVal Url As String, _
        ' ByVal ClientScript As String, ByVal UseActionEvent As Boolean, ByVal Secure As SecurityAccessLevel)
        '    Me.New(ID, Title, CmdName, CmdArg, Icon, Url, _
        '     ClientScript, UseActionEvent, Secure, True, False)
        'End Sub

        'Public Sub New(ByVal ID As Integer, ByVal Title As String, ByVal CmdName As String, ByVal CmdArg As String, ByVal Icon As String, ByVal Url As String, _
        ' ByVal ClientScript As String, ByVal UseActionEvent As Boolean, ByVal Secure As SecurityAccessLevel, ByVal Visible As Boolean)
        '    Me.New(ID, Title, CmdName, CmdArg, Icon, Url, _
        '     ClientScript, UseActionEvent, Secure, Visible, False)
        'End Sub

        'Public Sub New(ByVal ID As Integer, ByVal Title As String, ByVal CmdName As String, ByVal CmdArg As String, ByVal Icon As String, ByVal Url As String, _
        ' ByVal ClientScript As String, ByVal UseActionEvent As Boolean, ByVal Secure As SecurityAccessLevel, ByVal Visible As Boolean, ByVal NewWindow As Boolean)
        '    Me._id = ID
        '    Me._title = Title
        '    Me._commandName = CmdName
        '    Me._commandArgument = CmdArg
        '    Me._icon = Icon
        '    Me._url = Url
        '    Me._clientScript = ClientScript
        '    Me._useActionEvent = UseActionEvent
        '    Me._secure = Secure
        '    Me._visible = Visible
        '    Me._newwindow = NewWindow
        'End Sub

        'Public Function HasChildren() As Boolean
        '    Return (Me.Actions.Count > 0)
        'End Function

        ' Properties
        ''Public Property Actions() As ModuleActionCollection
        ''    Get
        ''        If Me._actions Is Nothing Then
        ''            Me._actions = New ModuleActionCollection()
        ''        End If
        ''        Return Me._actions
        ''    End Get
        ''    Set(ByVal value As ModuleActionCollection)
        ''        Me._actions = value
        ''    End Set
        ''End Property

        'Public Property ClientScript() As String
        '    Get
        '        Return Me._clientScript
        '    End Get
        '    Set(ByVal value As String)
        '        Me._clientScript = value
        '    End Set
        'End Property

        'Public Property CommandArgument() As String
        '    Get
        '        Return Me._commandArgument
        '    End Get
        '    Set(ByVal value As String)
        '        Me._commandArgument = value
        '    End Set
        'End Property

        'Public Property CommandName() As String
        '    Get
        '        Return Me._commandName
        '    End Get
        '    Set(ByVal value As String)
        '        Me._commandName = value
        '    End Set
        'End Property

        'Public Property Icon() As String
        '    Get
        '        Return Me._icon
        '    End Get
        '    Set(ByVal value As String)
        '        Me._icon = value
        '    End Set
        'End Property

        'Public Property ID() As Integer
        '    Get
        '        Return Me._id
        '    End Get
        '    Set(ByVal value As Integer)
        '        Me._id = value
        '    End Set
        'End Property

        'Public Property NewWindow() As Boolean
        '    Get
        '        Return Me._newwindow
        '    End Get
        '    Set(ByVal value As Boolean)
        '        Me._newwindow = value
        '    End Set
        'End Property

        'Public Property Secure() As SecurityAccessLevel
        '    Get
        '        Return Me._secure
        '    End Get
        '    Set(ByVal value As SecurityAccessLevel)
        '        Me._secure = value
        '    End Set
        'End Property

        'Public Property Title() As String
        '    Get
        '        Return Me._title
        '    End Get
        '    Set(ByVal value As String)
        '        Me._title = value
        '    End Set
        'End Property

        'Public Property Url() As String
        '    Get
        '        Return Me._url
        '    End Get
        '    Set(ByVal value As String)
        '        Me._url = value
        '    End Set
        'End Property

        'Public Property UseActionEvent() As Boolean
        '    Get
        '        Return Me._useActionEvent
        '    End Get
        '    Set(ByVal value As Boolean)
        '        Me._useActionEvent = value
        '    End Set
        'End Property

        'Public Property Visible() As Boolean
        '    Get
        '        Return Me._visible
        '    End Get
        '    Set(ByVal value As Boolean)
        '        Me._visible = value
        '    End Set
        'End Property

        Public Property Visible() As Boolean Implements IModuleAction.Visible
            Get

            End Get
            Set(ByVal value As Boolean)

            End Set
        End Property
    End Class
End Namespace
