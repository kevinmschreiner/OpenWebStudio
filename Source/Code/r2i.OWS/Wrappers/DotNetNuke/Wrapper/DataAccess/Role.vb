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
    Public Class Role
        Implements IRole

        Private Shared ReadOnly _Instance As Role = New Role()
        Public Shared ReadOnly Property Instance() As Role
            Get
                Return _Instance
            End Get
        End Property

        Private _id As String


        Private roleTotallyLoaded As Boolean
        Private roleInfo As DotNetNuke.Security.Roles.RoleInfo
        Public Sub New()
        End Sub
        Public Sub New(ByVal RoleData As DotNetNuke.Security.Roles.RoleInfo)
            roleTotallyLoaded = True
            roleInfo = RoleData
            _id = roleInfo.RoleID.ToString
        End Sub

        Public Property Id() As String Implements IRole.Id
            Get
                LoadRole()
                Return _id
            End Get
            Set(ByVal value As String)
                LoadRole()
                _id = value
            End Set
        End Property
        Public Property Name() As String Implements IRole.RoleName
            Get
                LoadRole()
                Return roleInfo.RoleName
            End Get
            Set(ByVal value As String)
                LoadRole()
                roleInfo.RoleName = value
            End Set
        End Property
        Public Property GroupId() As String Implements IRole.GroupId
            Get
                LoadRole()
                Return CStr(roleInfo.RoleGroupID)
            End Get
            Set(ByVal value As String)
                LoadRole()
                roleInfo.RoleGroupID = CInt(value)
            End Set
        End Property
        Public Property GroupName() As String Implements IRole.GroupName
            Get
                LoadRole()
                Return ""
            End Get
            Set(ByVal value As String)
                'LoadRole()
                '
            End Set
        End Property
        Public Property SiteId() As String Implements IRole.SiteId
            Get
                LoadRole()
                Return CStr(roleInfo.PortalID)
            End Get
            Set(ByVal value As String)
                LoadRole()
                roleInfo.PortalID = CInt(value)
            End Set
        End Property

        Private Sub LoadRole()
            If (Not roleTotallyLoaded AndAlso Not _id Is Nothing) Then
                Dim roleController As New DotNetNuke.Security.Roles.RoleController

                roleInfo = roleController.GetRole(CInt(_id), -1)

                roleTotallyLoaded = True
            Else
                roleInfo = New DotNetNuke.Security.Roles.RoleInfo
                roleTotallyLoaded = True
            End If
        End Sub
    End Class
End Namespace

