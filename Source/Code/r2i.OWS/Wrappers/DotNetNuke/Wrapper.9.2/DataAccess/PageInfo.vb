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
Namespace DataAccess
    Public Class PageInfo
        Inherits AbstractPageInfo


        Private Page As DotNetNuke.Framework.CDefault
        Public Sub New(ByVal Page As DotNetNuke.Framework.CDefault)
            Me.Page = Page
        End Sub

        Public Overrides Function GetMetaProperty(ByVal field As String) As String
            Dim hMeta As System.Web.UI.HtmlControls.HtmlMeta = CType(Me.Page.Header.FindControl(field), System.Web.UI.HtmlControls.HtmlMeta)
            If Not hMeta Is Nothing Then
                Return hMeta.Content
            End If
            Return ""
        End Function

        Public Overrides Sub SetPageHeader(ByVal value As String, ByVal key As String)
            Dim l As System.Web.UI.WebControls.Literal
            Dim c As System.Web.UI.Control = Me.Page.Header.FindControl(key)

            If (Not c Is Nothing) Then
                l = CType(c, System.Web.UI.WebControls.Literal)
                l.Text = value
            Else
                l = New System.Web.UI.WebControls.Literal()
                l.ID = key
                l.Text = value
                Me.Page.Header.Controls.Add(l)
            End If
        End Sub
        Public Overrides Sub SetMetaProperty(ByVal field As String, ByVal val As String)
            Dim hMeta As System.Web.UI.HtmlControls.HtmlMeta = CType(Me.Page.Header.FindControl(field), System.Web.UI.HtmlControls.HtmlMeta)
            If Not hMeta Is Nothing Then
                hMeta.Content = val
            Else
                ' allow user to assign a custom val.
                hMeta = New System.Web.UI.HtmlControls.HtmlMeta()
                hMeta.Name = field
                hMeta.Content = val
                Me.Page.Header.Controls.Add(hMeta)
            End If
        End Sub
        Public Overloads Overrides Sub SetMetaProperty(ByVal field As String, ByVal attributes As System.Collections.Specialized.NameValueCollection, ByVal val As String)
            Dim hMeta As System.Web.UI.HtmlControls.HtmlMeta = Nothing
            If field.Length > 0 Then
                hMeta = CType(Me.Page.Header.FindControl(field), System.Web.UI.HtmlControls.HtmlMeta)
            End If
            If Not hMeta Is Nothing Then
                hMeta.Content = val
            Else
                ' allow user to assign a custom val.
                hMeta = New System.Web.UI.HtmlControls.HtmlMeta()
                If field.Length > 0 Then
                    hMeta.Name = field
                End If
                Try
                    If Not attributes Is Nothing AndAlso attributes.Count > 0 Then
                        Dim k As String
                        For Each k In attributes.Keys
                            Try
                                Dim v As String = attributes(k)
                                hMeta.Attributes(k) = v
                            Catch exc As Exception
                            End Try
                        Next
                    End If
                Catch ex As Exception
                End Try
                hMeta.Content = val
                Me.Page.Header.Controls.Add(hMeta)
                End If
        End Sub

        Public Overrides Property Author() As String
            Get
                Return GetMetaProperty("MetaAuthor")
            End Get
            Set(ByVal value As String)
                SetMetaProperty("MetaAuthor", value)
            End Set
        End Property

        Public Overrides Property Copyright() As String
            Get
                Return GetMetaProperty("MetaCopyright")
            End Get
            Set(ByVal value As String)
                SetMetaProperty("MetaCopyright", value)
            End Set
        End Property

        Public Overrides Property Description() As String
            Get
                Return GetMetaProperty("MetaDescription")
            End Get
            Set(ByVal value As String)
                SetMetaProperty("MetaDescription", value)
            End Set
        End Property

        Public Overrides Property Generator() As String
            Get
                Return GetMetaProperty("MetaGenerator")
            End Get
            Set(ByVal value As String)
                SetMetaProperty("MetaGenerator", value)
            End Set
        End Property

        Public Overrides Property Keywords() As String
            Get
                Return GetMetaProperty("MetaKeywords")
            End Get
            Set(ByVal value As String)
                SetMetaProperty("MetaKeywords", value)
            End Set
        End Property


    End Class

End Namespace
