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
Namespace r2i.OWS.Framework.Utilities.Compatibility
    <Serializable()> _
      Public Class SortAction
        Inherits Object
        Implements IComparer
        Implements IComparable, IComparable(Of SortAction)

        Private _ColumnName As String
        Private _NormalText As String
        Private _AscendingText As String
        Private _DescendingText As String
        Private _SortDirection As String
        Private _SortToggle As Boolean
        Private _SortOrder As Integer
        Private _SortQuery As String
        Private _SortTarget As String

        Public Sub New()
        End Sub

        Public Sub New(ByVal sParam() As String)
            ColumnName = sParam(1)
            NormalText = sParam(2)
            AscendingText = sParam(3)
            DescendingText = sParam(4)
            SortDirection = sParam(5)
            SortOrder = sParam(6)
            If sParam.Length > 7 Then
                SortQuery = sParam(7)
            End If
            If sParam.Length > 8 Then
                SortTarget = sParam(8)
            End If
        End Sub

        Public Overrides Function ToString() As String
            Dim str As String = "<UL>" & Me.GetType().ToString()

            str &= "<li><b>SortOrder</b>:&nbsp;" & SortOrder & "</li>"

            If Not ColumnName Is Nothing Then
                str &= "<li><b>ColumnName</b>:&nbsp;" & ColumnName & "</li>"
            Else
                str &= "<li><b>ColumnName</b>:&nbsp; Nothing</li>"
            End If
            If Not SortQuery Is Nothing Then
                str &= "<li><b>SortQuery</b>:&nbsp;" & SortQuery & "</li>"
            Else
                str &= "<li><b>SortQuery</b>:&nbsp; Nothing</li>"
            End If
            If Not SortTarget Is Nothing Then
                str &= "<li><b>SortTarget</b>:&nbsp;" & SortTarget & "</li>"
            Else
                str &= "<li><b>SortTarget</b>:&nbsp; Nothing</li>"
            End If
            If Not SortDirection Is Nothing Then
                str &= "<li><b>SortDirection</b>:&nbsp;" & SortDirection & "</li>"
            Else
                str &= "<li><b>SortDirection</b>:&nbsp; Nothing</li>"
            End If
            If Not NormalText Is Nothing Then
                str &= "<li><b>NormalText</b>:&nbsp;" & NormalText & "</li>"
            Else
                str &= "<li><b>NormalText</b>:&nbsp; Nothing</li>"
            End If
            If Not AscendingText Is Nothing Then
                str &= "<li><b>AscendingText</b>:&nbsp;" & AscendingText & "</li>"
            Else
                str &= "<li><b>AscendingText</b>:&nbsp; Nothing</li>"
            End If
            If Not DescendingText Is Nothing Then
                str &= "<li><b>DescendingText</b>:&nbsp;" & DescendingText & "</li>"
            Else
                str &= "<li><b>DescendingText</b>:&nbsp; Nothing</li>"
            End If
            str &= "</UL>"

            Return str
        End Function

        'Protected Property sortActionList() As List(Of SortAction)
        '    Get
        '        If Session("sortActionList" + ModuleId.ToString + UserId.ToString) Is Nothing Then
        '            Return Nothing
        '        Else
        '            Return Session("sortActionList" + ModuleId.ToString + UserId.ToString)
        '        End If
        '    End Get
        '    Set(ByVal Value As List(Of SortAction))
        '        Session("sortActionList" + ModuleId.ToString + UserId.ToString) = Value
        '    End Set
        'End Property

        Public Property ColumnName() As String
            Get
                Return _ColumnName
            End Get
            Set(ByVal Value As String)
                _ColumnName = Value
            End Set
        End Property

        Public Property SortQuery() As String
            Get
                Return _SortQuery
            End Get
            Set(ByVal Value As String)
                _SortQuery = Value
            End Set
        End Property

        Public Property SortTarget() As String
            Get
                Return _SortTarget
            End Get
            Set(ByVal Value As String)
                _SortTarget = Value
            End Set
        End Property

        Public Property Toggle() As Boolean
            Get
                Return _SortToggle
            End Get
            Set(ByVal Value As Boolean)
                _SortToggle = Value
            End Set
        End Property

        Public Property NormalText() As String
            Get
                Return _NormalText
            End Get
            Set(ByVal Value As String)
                _NormalText = Value
            End Set
        End Property

        Public Property AscendingText() As String
            Get
                Return _AscendingText
            End Get
            Set(ByVal Value As String)
                _AscendingText = Value
            End Set
        End Property

        Public Property DescendingText() As String
            Get
                Return _DescendingText
            End Get
            Set(ByVal Value As String)
                _DescendingText = Value
            End Set
        End Property

        Public Property SortDirection() As String
            Get
                Return _SortDirection
            End Get
            Set(ByVal Value As String)
                If Value.ToUpper.EndsWith(" TOGGLE") Then
                    _SortDirection = Value.Substring(0, Value.Length - 7)
                    _SortToggle = True
                Else
                    _SortDirection = Value
                End If
            End Set
        End Property

        Public Property SortOrder() As String
            Get
                Return _SortOrder
            End Get
            Set(ByVal Value As String)
                _SortOrder = Value
            End Set
        End Property

        Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Nothing Then
                Return False
            End If
            If Not Me.GetType Is obj.GetType Then
                Return False
            End If
            Dim sa As SortAction = CType(obj, SortAction)
            If Me.SortOrder = sa.SortOrder Then
                Return True
            End If

            Return False
        End Function

        Public Function SACompare(ByVal x As Object, ByVal y As Object) As Integer Implements System.Collections.IComparer.Compare
            Dim xSA As SortAction = CType(x, SortAction)
            Dim ySA As SortAction = CType(y, SortAction)

            If (xSA.SortOrder < ySA.SortOrder) Then
                Return -1
            ElseIf (xSA.SortOrder > ySA.SortOrder) Then
                Return 1
            Else
                Return 0
            End If
        End Function

        Public Function CompareTo(ByVal obj As Object) As Integer Implements System.IComparable.CompareTo
            Return SACompare(Me, obj)
        End Function

        Public Function CompareTo_T(ByVal other As SortAction) As Integer Implements System.IComparable(Of SortAction).CompareTo
            Return CompareTo(other)
        End Function
    End Class
End Namespace
