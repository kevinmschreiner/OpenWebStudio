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
    Public Class TabPermissionInfo
        Implements ITabPermissionInfo

        Private _obj As DotNetNuke.Security.Permissions.TabPermissionInfo

        Public Sub New()
            _obj = New DotNetNuke.Security.Permissions.TabPermissionInfo
        End Sub
        Public Sub New(ByVal obj As DotNetNuke.Security.Permissions.TabPermissionInfo)
            _obj = obj
            If _obj Is Nothing Then
                _obj = New DotNetNuke.Security.Permissions.TabPermissionInfo
            End If
        End Sub

        Public Property AllowAccess() As Boolean Implements Framework.DataAccess.ITabPermissionInfo.AllowAccess
            Get
                Return _obj.AllowAccess
            End Get
            Set(ByVal value As Boolean)
                _obj.AllowAccess = value
            End Set
        End Property

        Public Sub Load(ByVal obj As Object) Implements Framework.DataAccess.ITabPermissionInfo.Load
            _obj = CType(obj, DotNetNuke.Security.Permissions.TabPermissionInfo)
        End Sub

        Public Property PermissionCode() As String Implements Framework.DataAccess.ITabPermissionInfo.PermissionCode
            Get
                Return _obj.PermissionCode
            End Get
            Set(ByVal value As String)
                _obj.PermissionCode = value
            End Set
        End Property

        Public Property PermissionID() As String Implements Framework.DataAccess.ITabPermissionInfo.PermissionID
            Get
                Return CStr(_obj.PermissionID)
            End Get
            Set(ByVal value As String)
                _obj.PermissionID = CInt(value)
            End Set
        End Property

        Public Property PermissionKey() As String Implements Framework.DataAccess.ITabPermissionInfo.PermissionKey
            Get
                Return _obj.PermissionKey
            End Get
            Set(ByVal value As String)
                _obj.PermissionKey = value
            End Set
        End Property

        Public Property PermissionName() As String Implements Framework.DataAccess.ITabPermissionInfo.PermissionName
            Get
                Return _obj.PermissionName
            End Get
            Set(ByVal value As String)
                _obj.PermissionName = value
            End Set
        End Property

        Public Property RoleID() As String Implements Framework.DataAccess.ITabPermissionInfo.RoleID
            Get
                Return CStr(_obj.RoleID)
            End Get
            Set(ByVal value As String)
                _obj.RoleID = CInt(value)
            End Set
        End Property

        Public Property RoleName() As String Implements Framework.DataAccess.ITabPermissionInfo.RoleName
            Get
                Return _obj.RoleName
            End Get
            Set(ByVal value As String)
                _obj.RoleName = value
            End Set
        End Property

        Public Function Save() As Object Implements Framework.DataAccess.ITabPermissionInfo.Save
            Return _obj
        End Function

        Public Property TabID() As String Implements Framework.DataAccess.ITabPermissionInfo.TabID
            Get
                Return CStr(_obj.TabID)
            End Get
            Set(ByVal value As String)
                _obj.TabID = CInt(value)
            End Set
        End Property

        Public Property TabPermissionID() As String Implements Framework.DataAccess.ITabPermissionInfo.TabPermissionID
            Get
                Return CStr(_obj.TabPermissionID)
            End Get
            Set(ByVal value As String)
                _obj.TabPermissionID = CInt(value)
            End Set
        End Property
    End Class
End Namespace
