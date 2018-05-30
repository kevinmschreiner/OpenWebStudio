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
    Public Class ModuleActionCollection
        'Inherits CollectionBase
        Implements IModuleActionCollection


        Public Sub CopyTo(ByVal array As System.Array, ByVal index As Integer) Implements System.Collections.ICollection.CopyTo

        End Sub

        Public ReadOnly Property Count() As Integer Implements System.Collections.ICollection.Count
            Get

            End Get
        End Property

        Public ReadOnly Property IsSynchronized() As Boolean Implements System.Collections.ICollection.IsSynchronized
            Get

            End Get
        End Property

        Public ReadOnly Property SyncRoot() As Object Implements System.Collections.ICollection.SyncRoot
            Get
                Return Nothing
            End Get
        End Property

        Public Function GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
            Return Nothing
        End Function

        Public Function Add(ByVal value As Object) As Integer Implements System.Collections.IList.Add
            Return Nothing
        End Function

        Public Sub Clear() Implements System.Collections.IList.Clear

        End Sub

        Public Function Contains(ByVal value As Object) As Boolean Implements System.Collections.IList.Contains

        End Function

        Public Function IndexOf(ByVal value As Object) As Integer Implements System.Collections.IList.IndexOf

        End Function

        Public Sub Insert(ByVal index As Integer, ByVal value As Object) Implements System.Collections.IList.Insert

        End Sub

        Public ReadOnly Property IsFixedSize() As Boolean Implements System.Collections.IList.IsFixedSize
            Get

            End Get
        End Property

        Public ReadOnly Property IsReadOnly() As Boolean Implements System.Collections.IList.IsReadOnly
            Get

            End Get
        End Property

        Default Public Property Item(ByVal index As Integer) As Object Implements System.Collections.IList.Item
            Get
                Return Nothing
            End Get
            Set(ByVal value As Object)

            End Set
        End Property

        Public Sub Remove(ByVal value As Object) Implements System.Collections.IList.Remove

        End Sub

        Public Sub RemoveAt(ByVal index As Integer) Implements System.Collections.IList.RemoveAt

        End Sub
    End Class
End Namespace
