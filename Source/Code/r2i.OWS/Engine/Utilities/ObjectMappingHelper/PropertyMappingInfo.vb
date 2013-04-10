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
Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Reflection

Namespace r2i.OWS.ObjectMappingHelper
    Friend NotInheritable Class PropertyMappingInfo
#Region "Private Variables"

        Private _dataFieldName As String
        Private _defaultValue As Object
        Private _propInfo As PropertyInfo

#End Region

#Region "Constructors"

        Friend Sub New()
            Me.New(String.Empty, Nothing, Nothing)
        End Sub

        Friend Sub New(ByVal dataFieldName As String, ByVal defaultValue As Object, ByVal info As PropertyInfo)
            _dataFieldName = dataFieldName
            _defaultValue = defaultValue
            _propInfo = info
        End Sub

#End Region

#Region "Public Properties"

        Public ReadOnly Property DataFieldName() As String
            Get
                If String.IsNullOrEmpty(_dataFieldName) Then
                    _dataFieldName = _propInfo.Name
                End If
                Return _dataFieldName
            End Get
        End Property

        Public ReadOnly Property DefaultValue() As Object
            Get
                Return _defaultValue
            End Get
        End Property

        Public ReadOnly Property PropertyInfo() As PropertyInfo
            Get
                Return _propInfo
            End Get
        End Property

#End Region
    End Class
End Namespace
