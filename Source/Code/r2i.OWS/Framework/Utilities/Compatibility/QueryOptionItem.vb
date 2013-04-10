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
Imports r2i.OWS.Framework.Utilities

Namespace r2i.OWS.Framework.Utilities.Compatibility
    Public Class QueryOptionItem
        Implements IComparable(Of QueryOptionItem)

        Public Index As Integer
        Private m_VariableType As String
        Private m_Protected As Boolean
        Private m_escapeListX As Integer
        Private m_escapeHTML As Boolean
        Private m_QueryTargetLeft As String
        Private m_QueryTargetRight As String
        Private m_QueryTargetEmpty As String
        Private m_QuerySource As String
        Private m_QueryTarget As String
        Private m_VariableDataType As String
        Private m_Formatters As String

        Public Property Formatters() As String
            Get
                Return m_Formatters
            End Get
            Set(ByVal Value As String)
                m_Formatters = Value
            End Set
        End Property

        Public Property VariableType() As String
            Get
                Return m_VariableType
            End Get
            Set(ByVal Value As String)
                m_VariableType = Value
            End Set
        End Property
        Public Property VariableDataType() As String
            Get
                If m_VariableDataType Is Nothing OrElse m_VariableDataType.Length = 0 Then
                    m_VariableDataType = "Any"
                End If
                Return m_VariableDataType
            End Get
            Set(ByVal Value As String)
                m_VariableDataType = Value
            End Set
        End Property
        Public Property QuerySource() As String
            Get
                Return m_QuerySource
            End Get
            Set(ByVal Value As String)
                m_QuerySource = Value
            End Set
        End Property
        Public Property QueryTarget() As String
            Get
                Return m_QueryTarget
            End Get
            Set(ByVal Value As String)
                m_QueryTarget = Value
            End Set
        End Property
        Public Property QueryTargetLeft() As String
            Get
                Return m_QueryTargetLeft
            End Get
            Set(ByVal Value As String)
                m_QueryTargetLeft = Value
            End Set
        End Property
        Public Property QueryTargetRight() As String
            Get
                Return m_QueryTargetRight
            End Get
            Set(ByVal Value As String)
                m_QueryTargetRight = Value
            End Set
        End Property
        Public Property QueryTargetEmpty() As String
            Get
                Return m_QueryTargetEmpty
            End Get
            Set(ByVal Value As String)
                m_QueryTargetEmpty = Value
            End Set
        End Property
        Public Property [Protected]() As Boolean
            Get
                Return m_Protected
            End Get
            Set(ByVal Value As Boolean)
                m_Protected = Value
            End Set
        End Property
        Public Property EscapeHTML() As Boolean
            Get
                Return m_escapeHTML
            End Get
            Set(ByVal Value As Boolean)
                m_escapeHTML = Value
            End Set
        End Property
        Public Property EscapeListX() As Integer
            Get
                Return m_escapeListX
            End Get
            Set(ByVal Value As Integer)
                m_escapeListX = Value
            End Set
        End Property

        Public Shared Function GetQueryOptionItemFromJson(ByVal propertiesList As Dictionary(Of String, Object)) As QueryOptionItem
            Dim qo As New QueryOptionItem
            Dim sValue As String

            Try
                sValue = Utility.GetDictionaryValue(propertiesList, "Index")
                If sValue <> "" Then Int32.TryParse(sValue, qo.Index)
                sValue = Utility.GetDictionaryValue(propertiesList, "VariableType")
                If sValue <> "" Then qo.VariableType = sValue
                sValue = Utility.GetDictionaryValue(propertiesList, "Formatters")
                If sValue <> "" Then qo.Formatters = sValue
                sValue = Utility.GetDictionaryValue(propertiesList, "VariableDataType")
                If sValue <> "" Then qo.VariableDataType = sValue
                sValue = Utility.GetDictionaryValue(propertiesList, "QuerySource")
                If sValue <> "" Then qo.QuerySource = sValue
                sValue = Utility.GetDictionaryValue(propertiesList, "QueryTarget")
                If sValue <> "" Then qo.QueryTarget = sValue
                sValue = Utility.GetDictionaryValue(propertiesList, "QueryTargetLeft")
                If sValue <> "" Then qo.QueryTargetLeft = sValue
                sValue = Utility.GetDictionaryValue(propertiesList, "QueryTargetRight")
                If sValue <> "" Then qo.QueryTargetRight = sValue
                sValue = Utility.GetDictionaryValue(propertiesList, "QueryTargetEmpty")
                If sValue <> "" Then qo.QueryTargetEmpty = sValue
                sValue = Utility.GetDictionaryValue(propertiesList, "Protected")
                If sValue <> "" Then Boolean.TryParse(sValue, qo.Protected)
                sValue = Utility.GetDictionaryValue(propertiesList, "EscapeHTML")
                If sValue <> "" Then Boolean.TryParse(sValue, qo.EscapeHTML)
                sValue = Utility.GetDictionaryValue(propertiesList, "EscapeListX")
                If sValue <> "" Then Int32.TryParse(sValue, qo.EscapeListX)
            Catch ex As Exception
                qo = Nothing
            End Try

            Return qo
        End Function

        Public Function CompareTo(ByVal other As QueryOptionItem) As Integer Implements System.IComparable(Of QueryOptionItem).CompareTo
            If other.QueryTarget.Length = Me.QueryTarget.Length Then
                Return 0
            ElseIf other.QueryTarget.Length > Me.QueryTarget.Length Then
                Return -1
            Else
                Return 1
            End If
        End Function
        Public Class QueryOptionItemKeyComparer
            Implements IComparer(Of String)
            Implements IComparer


            Public Function Compare(ByVal x As String, ByVal y As String) As Integer Implements System.Collections.Generic.IComparer(Of String).Compare
                If x.Length = y.Length Then
                    Return 0
                ElseIf x.Length > y.Length Then
                    Return -1
                Else
                    Return 1
                End If
            End Function

            Public Function Compare1(ByVal x As Object, ByVal y As Object) As Integer Implements System.Collections.IComparer.Compare
                If TypeOf x Is String AndAlso TypeOf y Is String Then
                    Return Compare(CStr(x), CStr(y))
                Else
                    Return 0
                End If
            End Function
        End Class
    End Class
End Namespace
