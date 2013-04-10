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
Imports System.Collections.Generic

Namespace r2i.OWS.Framework.Utilities
    Public Class GenericSession
        Implements System.Web.SessionState.IHttpSessionState

        Private _session As Web.SessionState.HttpSessionState
        Private _sessionSLT As SortedList(Of String, Object)
        Default Public Overloads Property Item(ByVal Name As String) As Object Implements System.Web.SessionState.IHttpSessionState.Item
            Get
                If Not _session Is Nothing Then
                    Return _session(Name)
                Else
                    If _sessionSLT.ContainsKey(Name) Then
                        Return _sessionSLT(Name)
                    End If
                End If
                Return Nothing
            End Get
            Set(ByVal Value As Object)
                If Not _session Is Nothing Then
                    _session(Name) = Value
                Else
                    If _sessionSLT.ContainsKey(Name) Then
                        _sessionSLT(Name) = Value
                    Else
                        Try
                            _sessionSLT.Add(Name, Value)
                        Catch ex As Exception
                        End Try
                    End If
                End If
            End Set
        End Property
        Default Public Overloads Property Item(ByVal index As Integer) As Object Implements System.Web.SessionState.IHttpSessionState.Item
            Get
                If Not _session Is Nothing Then
                    Return _session(index)
                Else
                    If _sessionSLT.Keys.Count > index AndAlso index >= 0 Then
                        Return _sessionSLT(_sessionSLT.Keys(index))
                    End If
                End If
                Return Nothing
            End Get
            Set(ByVal Value As Object)
                If Not _session Is Nothing Then
                    _session(index) = Value
                Else
                    If _sessionSLT.Keys.Count > index AndAlso index >= 0 Then
                        _sessionSLT(_sessionSLT.Keys(index)) = Value
                    End If
                End If
            End Set
        End Property
        Public ReadOnly Property SessionID() As String Implements System.Web.SessionState.IHttpSessionState.SessionID
            Get
                If Not _session Is Nothing Then
                    Return _session.SessionID
                End If
                Return String.Empty
            End Get
        End Property
        Public ReadOnly Property Keys() As Collections.Specialized.NameValueCollection.KeysCollection Implements System.Web.SessionState.IHttpSessionState.Keys
            Get
                If Not _session Is Nothing Then
                    Return _session.Keys
                Else
                    Return _sessionSLT.Keys
                End If
            End Get
        End Property

        Public Sub New(ByRef session As Web.SessionState.HttpSessionState)
            If Not session Is Nothing Then
                _session = session
            Else
                _sessionSLT = New SortedList(Of String, Object)
            End If
        End Sub
        Public Sub New()
            _sessionSLT = New SortedList(Of String, Object)
        End Sub
        Public Sub Add(ByVal Name As String, ByVal Value As Object) Implements System.Web.SessionState.IHttpSessionState.Add
            If Not _session Is Nothing Then
                _session.Add(Name, Value)
            Else
                Try
                    _sessionSLT.Add(Name, Value)
                Catch ex As Exception
                End Try
            End If
        End Sub
        Public Sub Remove(ByVal Name As String) Implements System.Web.SessionState.IHttpSessionState.Remove
            If Not _session Is Nothing Then
                _session.Remove(Name)
            Else
                If _sessionSLT.ContainsKey(Name) Then
                    _sessionSLT.Remove(Name)
                End If
            End If
        End Sub

        Public Sub Abandon() Implements System.Web.SessionState.IHttpSessionState.Abandon
            If Not _session Is Nothing Then
                _session.Abandon()
            End If
        End Sub


        Public Sub Clear() Implements System.Web.SessionState.IHttpSessionState.Clear
            If Not _session Is Nothing Then
                _session.Clear()
            Else
                If Not _sessionSLT Is Nothing Then
                    _sessionSLT.Clear()
                End If
            End If
        End Sub

        Public Property CodePage() As Integer Implements System.Web.SessionState.IHttpSessionState.CodePage
            Get
                If Not _session Is Nothing Then
                    Return _session.CodePage
                End If
            End Get
            Set(ByVal value As Integer)
                If Not _session Is Nothing Then
                    _session.CodePage = value
                End If
            End Set
        End Property

        Public ReadOnly Property CookieMode() As System.Web.HttpCookieMode Implements System.Web.SessionState.IHttpSessionState.CookieMode
            Get
                If Not _session Is Nothing Then
                    Return _session.CookieMode
                Else
                    Return Web.HttpCookieMode.AutoDetect
                End If
            End Get
        End Property

        Public Sub CopyTo(ByVal array As System.Array, ByVal index As Integer) Implements System.Web.SessionState.IHttpSessionState.CopyTo
            If Not _session Is Nothing Then
                _session.CopyTo(array, index)
            Else
                'NOT SUPPORTED
            End If
        End Sub

        Public ReadOnly Property Count() As Integer Implements System.Web.SessionState.IHttpSessionState.Count
            Get
                If Not _session Is Nothing Then
                    Return _session.Count
                Else
                    Return _sessionSLT.Count
                End If
            End Get
        End Property

        Public Function GetEnumerator() As System.Collections.IEnumerator Implements System.Web.SessionState.IHttpSessionState.GetEnumerator
            If Not _session Is Nothing Then
                Return _session.GetEnumerator
            Else
                Return _sessionSLT.GetEnumerator
            End If
        End Function

        Public ReadOnly Property IsCookieless() As Boolean Implements System.Web.SessionState.IHttpSessionState.IsCookieless
            Get
                If Not _session Is Nothing Then
                    Return _session.IsCookieless
                Else
                    Return False
                End If
            End Get
        End Property

        Public ReadOnly Property IsNewSession() As Boolean Implements System.Web.SessionState.IHttpSessionState.IsNewSession
            Get
                If Not _session Is Nothing Then
                    Return _session.IsNewSession
                Else
                    Return False
                End If
            End Get
        End Property

        Public ReadOnly Property IsReadOnly() As Boolean Implements System.Web.SessionState.IHttpSessionState.IsReadOnly
            Get
                If Not _session Is Nothing Then
                    Return _session.IsReadOnly
                Else
                    Return False
                End If
            End Get
        End Property

        Public ReadOnly Property IsSynchronized() As Boolean Implements System.Web.SessionState.IHttpSessionState.IsSynchronized
            Get
                If Not _session Is Nothing Then
                    Return _session.IsSynchronized
                Else
                    Return True
                End If
            End Get
        End Property

        Private _lcid As Integer = 0
        Public Property LCID() As Integer Implements System.Web.SessionState.IHttpSessionState.LCID
            Get
                If Not _session Is Nothing Then
                    Return _session.LCID
                Else
                    Return _lcid
                End If
            End Get
            Set(ByVal value As Integer)
                If Not _session Is Nothing Then
                    _session.LCID = value
                Else
                    _lcid = value
                End If
            End Set
        End Property

        Public ReadOnly Property Mode() As System.Web.SessionState.SessionStateMode Implements System.Web.SessionState.IHttpSessionState.Mode
            Get
                If Not _session Is Nothing Then
                    Return _session.Mode
                Else
                    Return Web.SessionState.SessionStateMode.Off
                End If
            End Get
        End Property


        Public Sub RemoveAll() Implements System.Web.SessionState.IHttpSessionState.RemoveAll
            If Not _session Is Nothing Then
                _session.RemoveAll()
            Else
                _sessionSLT.Clear()
            End If
        End Sub

        Public Sub RemoveAt(ByVal index As Integer) Implements System.Web.SessionState.IHttpSessionState.RemoveAt
            If Not _session Is Nothing Then
                _session.RemoveAt(index)
            Else
                _sessionSLT.RemoveAt(index)
            End If
        End Sub

        Public ReadOnly Property StaticObjects() As System.Web.HttpStaticObjectsCollection Implements System.Web.SessionState.IHttpSessionState.StaticObjects
            Get
                If Not _session Is Nothing Then
                    Return _session.StaticObjects
                Else
                    Return Nothing
                End If
            End Get
        End Property

        Public ReadOnly Property SyncRoot() As Object Implements System.Web.SessionState.IHttpSessionState.SyncRoot
            Get
                If Not _session Is Nothing Then
                    Return _session.SyncRoot
                Else
                    Return Nothing
                End If
            End Get
        End Property

        Public Property Timeout() As Integer Implements System.Web.SessionState.IHttpSessionState.Timeout
            Get
                If Not _session Is Nothing Then
                    Return _session.Timeout
                Else
                    Return 0
                End If
            End Get
            Set(ByVal value As Integer)
                If Not _session Is Nothing Then
                    _session.Timeout = value
                Else
                    'NOT SUPPORTED
                End If
            End Set
        End Property
    End Class
End Namespace
