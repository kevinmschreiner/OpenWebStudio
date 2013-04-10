'<LICENSE>
'   Open Web Studio - http://www.openwebstudio.com
'   Copyright (c) 2006-2008
'   by R2 Integrated Inc. ( http://www.r2integrated.com )
'   
'   Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
'   documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
'   the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
'   to permit persons to whom the Software is furnished to do so, subject to the following conditions:
'   
'   The above copyright notice and this permission notice shall be included in all copies or substantial portions 
'   of the Software.
'   
'   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
'   TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
'   THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
'   CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
'   DEALINGS IN THE SOFTWARE.
'</LICENSE>
Namespace r2i.OWS.Framework.Plugins
    Public Class PluginTag
        Public TypeKey As String
        Public SubTypeKey As String
        'Public PrimaryKey As String
        'Public SecondaryKey As String
        Private m_Keys() As String
        Private m_FullKeys() As String
        Public Function AssembleKey(ByVal Value As String)
            Dim s As String = ""
            If Not TypeKey Is Nothing Then
                s &= TypeKey & "."
            End If
            If Not SubTypeKey Is Nothing AndAlso SubTypeKey.Length > 0 Then
                s &= SubTypeKey & "."
            End If
            Return s & Value
        End Function
        Private Sub Load()
            If Not m_Keys Is Nothing AndAlso m_Keys.Length > 0 Then
                Dim strMFK(m_Keys.Length - 1) As String
                Dim i As Integer
                For i = 0 To m_Keys.Length - 1
                    strMFK(i) = AssembleKey(m_Keys(i))
                Next
                m_FullKeys = strMFK
            End If
        End Sub
        Public Sub New(ByVal [type] As String, ByVal subtype As String, ByVal keyarray() As String)
            TypeKey = [type]
            SubTypeKey = subtype
            m_Keys = keyarray
            Load()
        End Sub
        Public Sub New(ByVal [type] As String, ByVal subtype As String, ByVal primarykey As String)
            TypeKey = [type]
            SubTypeKey = subtype
            m_Keys = New String() {primarykey}
            Load()
        End Sub

        Public Sub New(ByVal [type] As String, ByVal subtype As String, ByVal primarykey As String, ByVal subkey As String)
            TypeKey = [type]
            SubTypeKey = subtype
            m_Keys = New String() {primarykey, subkey}
            Load()
        End Sub
        'Public Sub New(ByVal [type] As String, ByVal subtype As String, ByVal primary As String, ByVal secondary As String)
        '    TypeKey = [type]
        '    SubTypeKey = subtype
        '    PrimaryKey = primary
        '    SecondaryKey = secondary
        'End Sub
        'Public Sub New(ByVal [type] As String, ByVal subtype As String, ByVal primary As String)
        '    TypeKey = [type]
        '    SubTypeKey = subtype
        '    PrimaryKey = primary
        'End Sub
        Shared Function Create(ByVal [type] As String, ByVal subtype As String, ByVal primarykey As String, ByVal subkey As String) As PluginTag
            Return New PluginTag([type], subtype, primarykey, subkey)
        End Function
        Shared Function Create(ByVal [type] As String, ByVal subtype As String, ByVal primarykey As String) As PluginTag
            Return New PluginTag([type], subtype, primarykey)
        End Function
        Shared Function Create(ByVal [type] As String, ByVal subtype As String, ByVal keyarray() As String) As PluginTag
            Return New PluginTag([type], subtype, keyarray)
        End Function
        Public ReadOnly Property Names() As String()
            Get
                Return m_Keys
            End Get
        End Property
        Public ReadOnly Property Length() As Integer
            Get
                If Not m_FullKeys Is Nothing Then
                    Return m_FullKeys.Length
                Else
                    Return 0
                End If
            End Get
        End Property
        Public ReadOnly Property Name(ByVal index As Integer) As String
            Get
                If Not m_Keys Is Nothing AndAlso index < m_Keys.Length AndAlso index >= 0 Then
                    Return m_Keys(index)
                End If
                Return Nothing
            End Get
        End Property
        Public ReadOnly Property Key(ByVal index As Integer) As String
            Get
                If Not m_FullKeys Is Nothing AndAlso index < m_FullKeys.Length AndAlso index >= 0 Then
                    Return m_FullKeys(index)
                End If
                Return Nothing
            End Get
        End Property
        'Public Overloads Function ToString(ByVal isPrimary As Boolean) As String
        '    Dim s As String = ""
        '    If Not TypeKey Is Nothing Then
        '        s &= TypeKey & "."
        '    End If
        '    If Not SubTypeKey Is Nothing AndAlso SubTypeKey.Length > 0 Then
        '        s &= SubTypeKey & "."
        '    End If
        '    If isPrimary Then
        '        If Not PrimaryKey Is Nothing Then
        '            s &= PrimaryKey
        '        End If
        '    Else
        '        If SecondaryKey Is Nothing Then
        '            Return Nothing
        '        Else
        '            s &= SecondaryKey
        '        End If
        '    End If
        '    Return s
        'End Function
    End Class
    Public Interface iPlugin
        'Provides the specific key used by the plugin
        ReadOnly Property Plugin() As PluginTag
    End Interface
End Namespace