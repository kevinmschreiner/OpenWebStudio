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
Imports r2i.OWS.Framework
Namespace r2i.OWS.Framework.Utilities.Compatibility
    Public Class SearchOptionItem
        Public Index As Integer
        Private m_SearchOption As String
        Public Property SearchOption() As String
            Get
                Return m_SearchOption
            End Get
            Set(ByVal Value As String)
                m_SearchOption = Value
            End Set
        End Property
        Private m_SearchField As String
        Public Property SearchField() As String
            Get
                Return m_SearchField
            End Get
            Set(ByVal Value As String)
                m_SearchField = Value
            End Set
        End Property

        Public Shared Function GetSearchOptionItemFromJson(ByVal propertiesList As Dictionary(Of String, Object)) As SearchOptionItem
            Dim so As New SearchOptionItem
            Dim sValue As String

            Try
                sValue = Utilities.Utility.GetDictionaryValue(propertiesList, "Index")
                If sValue <> "" Then Int32.TryParse(sValue, so.Index)
                sValue = Utilities.Utility.GetDictionaryValue(propertiesList, "SearchOption")
                If sValue <> "" Then so.SearchOption = sValue
                sValue = Utilities.Utility.GetDictionaryValue(propertiesList, "SearchField")
                If sValue <> "" Then so.SearchField = sValue
            Catch ex As Exception
                so = Nothing
            End Try

            Return so
        End Function
    End Class

End Namespace
