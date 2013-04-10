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

Imports System.Collections

Namespace r2i.OWS.Framework.DataAccess
    Public Interface IModulePermissionCollection
        Function Add(ByVal value As IModulePermissionInfo) As Integer
        Property Capacity() As Integer
        Sub Clear()
        Function CompareTo(ByVal objModulePermissionCollection As IModulePermissionCollection) As Boolean
        Function Contains(ByVal value As IModulePermissionInfo) As Boolean
        ReadOnly Property Count() As Integer
        Function GetEnumerator() As System.Collections.IEnumerator
        Function IndexOf(ByVal value As IModulePermissionInfo) As Integer
        Sub Insert(ByVal index As Integer, ByVal value As IModulePermissionInfo)
        Default Property Item(ByVal index As Integer) As IModulePermissionInfo
        Sub Remove(ByVal value As IModulePermissionInfo)
        Sub RemoveAt(ByVal index As Integer)

        Sub Load(ByVal obj As Object)
        Function Save() As Object
    End Interface
End Namespace