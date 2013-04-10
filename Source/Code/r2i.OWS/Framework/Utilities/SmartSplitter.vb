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
    Public Class SmartSplitter
        Private m_Items As List(Of String)

        Public Sub Add(ByVal Value As String)
            m_Items.Add(Value)
        End Sub
        Default Public Property Item(ByVal Index As Integer) As String
            Get
                If Not m_Items Is Nothing AndAlso m_Items.Count > Index AndAlso Index >= 0 Then
                    Return m_Items(Index)
                Else
                    Return String.Empty
                End If
            End Get
            Set(ByVal Value As String)
                If Not m_Items Is Nothing AndAlso m_Items.Count > Index AndAlso Index >= 0 Then
                    m_Items(Index) = Value
                End If
            End Set
        End Property
        Public Sub New()
            m_Items = New List(Of String)
        End Sub
        Public Sub Split(ByVal Source As String)
            Dim position As Integer
            If Not Source Is Nothing AndAlso Source.Length > 0 Then
                position = Source.IndexOf(":")
                If position > 0 Then
                    If IsNumeric(Source.Substring(0, position)) Then
                        Dim headLength As Integer = Convert.ToInt32(Source.Substring(0, position))
                        If Source.Length > headLength + position Then
                            Dim header As String() = Source.Substring(position + 1, headLength).Split(";")
                            If Not header Is Nothing AndAlso header.Length > 0 Then
                                position = position + headLength + 1
                                Dim str As String
                                For Each str In header
                                    If IsNumeric(str) Then
                                        Dim itemLength As Integer = Convert.ToInt32(str)
                                        If Source.Length >= position + itemLength Then
                                            If itemLength > 0 Then
                                                m_Items.Add(Source.Substring(position, itemLength))
                                                position = position + itemLength
                                            Else
                                                m_Items.Add("")
                                            End If
                                        End If
                                    End If
                                Next
                            End If
                        End If
                    End If
                End If
            End If
        End Sub
        Public Function Length() As Integer
            If Not m_Items Is Nothing Then
                Return m_Items.Count
            Else
                Return 0
            End If
        End Function
        Public Function Blend() As String
            Dim header As String = ""
            Dim trailer As String = ""
            Dim str As String
            For Each str In m_Items
                str = str.Replace(vbCrLf, Chr(10))
                header = header & str.Length.ToString & ";"
                trailer = trailer & str
            Next
            header = header.Length.ToString & ":" & header
            Return header & trailer
        End Function
    End Class


End Namespace
