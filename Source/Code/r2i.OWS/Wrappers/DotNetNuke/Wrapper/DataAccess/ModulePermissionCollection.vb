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
    Public Class ModulePermissionCollection
        Implements IModulePermissionCollection

        Private _obj As DotNetNuke.Security.Permissions.ModulePermissionCollection
        Public Sub New()
            _obj = New DotNetNuke.Security.Permissions.ModulePermissionCollection
        End Sub
        Public Sub New(ByVal obj As DotNetNuke.Security.Permissions.ModulePermissionCollection)
            _obj = obj
            If _obj Is Nothing Then
                _obj = New DotNetNuke.Security.Permissions.ModulePermissionCollection
            End If
        End Sub

        Public Function Add(ByVal value As Framework.DataAccess.IModulePermissionInfo) As Integer Implements Framework.DataAccess.IModulePermissionCollection.Add
            _obj.Add(CType(value.Save(), DotNetNuke.Security.Permissions.ModulePermissionInfo))
        End Function

        Public Property Capacity() As Integer Implements Framework.DataAccess.IModulePermissionCollection.Capacity
            Get
                Return _obj.Capacity
            End Get
            Set(ByVal value As Integer)
                _obj.Capacity = value
            End Set
        End Property

        Public Sub Clear() Implements Framework.DataAccess.IModulePermissionCollection.Clear
            _obj.Clear()
        End Sub

        Public Function CompareTo(ByVal value As Framework.DataAccess.IModulePermissionCollection) As Boolean Implements Framework.DataAccess.IModulePermissionCollection.CompareTo
            Return _obj.CompareTo(CType(value.Save(), DotNetNuke.Security.Permissions.ModulePermissionCollection))
        End Function

        Public Function Contains(ByVal value As Framework.DataAccess.IModulePermissionInfo) As Boolean Implements Framework.DataAccess.IModulePermissionCollection.Contains
            Return _obj.Contains(CType(value.Save(), DotNetNuke.Security.Permissions.ModulePermissionInfo))
        End Function

        Public ReadOnly Property Count() As Integer Implements Framework.DataAccess.IModulePermissionCollection.Count
            Get
                Return _obj.Count
            End Get
        End Property

        Public Function GetEnumerator() As System.Collections.IEnumerator Implements Framework.DataAccess.IModulePermissionCollection.GetEnumerator
            Return _obj.GetEnumerator
        End Function


        Public Function IndexOf(ByVal value As Framework.DataAccess.IModulePermissionInfo) As Integer Implements Framework.DataAccess.IModulePermissionCollection.IndexOf
            Return _obj.IndexOf(CType(value.Save(), DotNetNuke.Security.Permissions.ModulePermissionInfo))
        End Function

        Public Sub Insert(ByVal index As Integer, ByVal value As Framework.DataAccess.IModulePermissionInfo) Implements Framework.DataAccess.IModulePermissionCollection.Insert
            _obj.Insert(index, CType(value.Save(), DotNetNuke.Security.Permissions.ModulePermissionInfo))
        End Sub

        Default Public Property Item(ByVal index As Integer) As Framework.DataAccess.IModulePermissionInfo Implements Framework.DataAccess.IModulePermissionCollection.Item
            Get
                Return New ModulePermissionInfo(_obj.Item(index))
            End Get
            Set(ByVal value As Framework.DataAccess.IModulePermissionInfo)
                _obj.Item(index) = CType(value.Save(), DotNetNuke.Security.Permissions.ModulePermissionInfo)
            End Set
        End Property

        Public Sub Load(ByVal obj As Object) Implements Framework.DataAccess.IModulePermissionCollection.Load
            _obj = CType(obj, DotNetNuke.Security.Permissions.ModulePermissionCollection)
        End Sub

        Public Sub Remove(ByVal value As Framework.DataAccess.IModulePermissionInfo) Implements Framework.DataAccess.IModulePermissionCollection.Remove
            _obj.Remove(CType(value.Save(), DotNetNuke.Security.Permissions.ModulePermissionInfo))
        End Sub

        Public Sub RemoveAt(ByVal index As Integer) Implements Framework.DataAccess.IModulePermissionCollection.RemoveAt
            _obj.RemoveAt(index)
        End Sub

        Public Function Save() As Object Implements Framework.DataAccess.IModulePermissionCollection.Save
            Return _obj
        End Function
    End Class

End Namespace
