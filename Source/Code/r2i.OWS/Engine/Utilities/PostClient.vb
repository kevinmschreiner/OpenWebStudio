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
Imports System
Imports System.Text
Imports System.IO
Imports System.Web
Imports System.Net
Imports System.Collections.Specialized

Namespace r2i.OWS.Framework.Utilities
    Public Class PostClient
        Public Enum PostTypeEnum
            [Get]
            Post
        End Enum

        Private m_url As String = String.Empty
        Private m_values As New NameValueCollection()
        Private m_type As PostTypeEnum = PostTypeEnum.[Get]

        Public Sub New()
        End Sub

        Public Sub New(ByVal url As String)
            m_url = url
        End Sub

        Public Sub New(ByVal url As String, ByVal values As NameValueCollection)
            Me.New(url)
            m_values = values
        End Sub

        Public Property Url() As String
            Get
                Return m_url
            End Get
            Set(ByVal value As String)
                m_url = value
            End Set
        End Property

        Public Property PostItems() As NameValueCollection
            Get
                Return m_values
            End Get
            Set(ByVal value As NameValueCollection)
                m_values = value
            End Set
        End Property

        Public Property Type() As PostTypeEnum
            Get
                Return m_type
            End Get
            Set(ByVal value As PostTypeEnum)
                m_type = value
            End Set
        End Property

        Public Function Post() As String
            Dim parameters As New StringBuilder()
            For i As Integer = 0 To m_values.Count - 1
                EncodeAndAddItem(parameters, m_values.GetKey(i), m_values(i))
            Next
            Dim result As String = PostData(m_url, parameters.ToString())
            Return result
        End Function

        Public Function Post(ByVal url As String) As String
            m_url = url
            Return Me.Post()
        End Function

        Public Function Post(ByVal url As String, ByVal values As NameValueCollection) As String
            m_values = values
            Return Me.Post(url)
        End Function

        Private Function PostData(ByVal url As String, ByVal postParameter As String) As String
            Dim request As HttpWebRequest = Nothing
            If m_type = PostTypeEnum.Post Then
                Dim uri As New Uri(url)
                request = DirectCast(WebRequest.Create(uri), HttpWebRequest)
                request.Method = "POST"
                request.ContentType = "application/x-www-form-urlencoded"
                Dim encoding As New UTF8Encoding()
                Dim bytes As Byte() = encoding.GetBytes(postParameter)
                request.ContentLength = bytes.Length
                Using writeStream As Stream = request.GetRequestStream()
                    writeStream.Write(bytes, 0, bytes.Length)
                End Using
            Else
                Dim uri As New Uri(url + "?" + postParameter)
                request = DirectCast(WebRequest.Create(uri), HttpWebRequest)
                request.Method = "GET"
            End If
            Dim result As String = String.Empty
            Using response As HttpWebResponse = DirectCast(request.GetResponse(), HttpWebResponse)
                Using responseStream As Stream = response.GetResponseStream()
                    Using readStream As New StreamReader(responseStream, Encoding.UTF8)
                        result = readStream.ReadToEnd()
                    End Using
                End Using
            End Using
            Return result
        End Function

        Private Sub EncodeAndAddItem(ByRef baseRequest As StringBuilder, ByVal key As String, ByVal dataItem As String)
            If baseRequest Is Nothing Then
                baseRequest = New StringBuilder()
            End If
            If baseRequest.Length <> 0 Then
                baseRequest.Append("&")
            End If
            If key.Length > 0 Then
                baseRequest.Append(key)
                baseRequest.Append("=")
            End If
            baseRequest.Append(System.Web.HttpUtility.UrlEncode(dataItem))
        End Sub
    End Class
End Namespace
