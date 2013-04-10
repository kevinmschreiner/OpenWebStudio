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
    Public Class ListFormatItem
        Private m_GroupStatement As String
        Private m_ListHeader As String
        Private m_ListFooter As String
        Private m_Index As String

        Public Property Index() As String
            Get
                Return m_Index
            End Get
            Set(ByVal Value As String)
                m_Index = Value
            End Set
        End Property
        Public Property GroupStatement() As String
            Get
                If Not m_GroupStatement Is Nothing Then
                    Return m_GroupStatement
                Else
                    Return ""
                End If
            End Get
            Set(ByVal Value As String)
                m_GroupStatement = Value
            End Set
        End Property
        Public Property ListHeader() As String
            Get
                If Not m_ListHeader Is Nothing Then
                    Return m_ListHeader
                Else
                    Return ""
                End If
            End Get
            Set(ByVal Value As String)
                m_ListHeader = Value
            End Set
        End Property
        Public Property ListFooter() As String
            Get
                If Not m_ListFooter Is Nothing Then
                    Return m_ListFooter
                Else
                    Return ""
                End If
            End Get
            Set(ByVal Value As String)
                m_ListFooter = Value
            End Set
        End Property

        Public Shared Function GetListFormatItemFromJson(ByVal propertiesList As Dictionary(Of String, Object)) As ListFormatItem
            Dim lf As New ListFormatItem
            Dim sValue As String

            Try
                sValue = Utility.GetDictionaryValue(propertiesList, "Index")
                If sValue <> "" Then lf.Index = sValue
                sValue = Utility.GetDictionaryValue(propertiesList, "GroupStatement")
                If sValue <> "" Then lf.GroupStatement = sValue
                sValue = Utility.GetDictionaryValue(propertiesList, "ListHeader")
                If sValue <> "" Then lf.ListHeader = sValue
                sValue = Utility.GetDictionaryValue(propertiesList, "ListFooter")
                If sValue <> "" Then lf.ListFooter = sValue
            Catch ex As Exception
                lf = Nothing
            End Try

            Return lf
        End Function
    End Class
End Namespace

