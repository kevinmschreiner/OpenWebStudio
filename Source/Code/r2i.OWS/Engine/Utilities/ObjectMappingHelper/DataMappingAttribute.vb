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

Namespace r2i.OWS.ObjectMappingHelper
    <AttributeUsage(AttributeTargets.[Property])> _
    Public NotInheritable Class DataMappingAttribute
        Inherits System.Attribute
#Region "Private Variables"

        Private _dataFieldName As String
        Private _nullValue As Object

#End Region

#Region "Constructors"

        Public Sub New(ByVal dataFieldName As String, ByVal nullValue As Object)
            MyBase.New()
            _dataFieldName = dataFieldName
            _nullValue = nullValue
        End Sub

        Public Sub New(ByVal nullValue As Object)
            Me.New(String.Empty, nullValue)
        End Sub

#End Region

#Region "Public Properties"

        Public ReadOnly Property DataFieldName() As String
            Get
                Return _dataFieldName
            End Get
        End Property

        Public ReadOnly Property NullValue() As Object
            Get
                Return _nullValue
            End Get
        End Property

#End Region
    End Class
End Namespace
