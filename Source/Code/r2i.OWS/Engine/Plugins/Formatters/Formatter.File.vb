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
Imports System.Collections
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Diagnostics
Imports r2i.OWS.Framework.Utilities
Imports r2i.OWS.Framework
Imports r2i.OWS.Framework.Plugins.Formatters
Namespace r2i.OWS.Formatters
    Public Class File : Inherits FormatterBase

        Public Overrides Function Handle_Render(ByRef Caller As EngineBase, ByVal Index As Integer, ByRef Value As String, ByRef Formatter As String, ByRef Source As String, ByRef DS As System.Data.DataSet, ByRef DR As System.Data.DataRow, ByRef RuntimeMessages As System.Collections.Generic.SortedList(Of String, String), ByVal Nullreturn As Boolean, ByVal NullOverride As Boolean, ByVal ProtectSession As Boolean, ByVal SessionDelimiter As String, ByVal useSessionQuotes As Boolean, ByVal useAggregations As Boolean, ByRef FilterText As String, ByRef FilterField As String, ByRef Debugger As Framework.Debugger) As Boolean
            Try
                Select Case True
                    Case Formatter.ToUpper.StartsWith("{FILE.VERSION")
                        If System.IO.File.Exists(Value) AndAlso (System.IO.File.GetAttributes(Value) And IO.FileAttributes.Directory) = 0 AndAlso Utilities.Utility.Security_FilePermission AndAlso Utilities.Utility.Security_ReflectPermission Then
                            'GET THE VERSION
                            With System.Diagnostics.FileVersionInfo.GetVersionInfo(Value)
                                Select Case Formatter.ToUpper
                                    Case "{FILE.VERSION}"
                                        Source = .FileVersion
                                    Case "{FILE.VERSION.MAJOR}"
                                        Source = .FileMajorPart.ToString()
                                    Case "{FILE.VERSION.MINOR}"
                                        Source = .FileMinorPart.ToString()
                                    Case "{FILE.VERSION.BUILD}"
                                        Source = .FileBuildPart.ToString()
                                    Case "{FILE.VERSION.REVISION}"
                                        Source = .FilePrivatePart.ToString()
                                    Case "{FILE.VERSION.PRODUCT}"
                                        Source = .ProductVersion
                                    Case "{FILE.VERSION.PRODUCT.MAJOR}"
                                        Source = .ProductMajorPart.ToString()
                                    Case "{FILE.VERSION.PRODUCT.MINOR}"
                                        Source = .ProductMinorPart.ToString()
                                    Case "{FILE.VERSION.PRODUCT.BUILD}"
                                        Source = .ProductBuildPart.ToString()
                                    Case "{FILE.VERSION.PRODUCT.REVISION}"
                                        Source = .ProductPrivatePart.ToString()
                                    Case "{FILE.VERSION.PRODUCT.NAME}"
                                        Source = .ProductName.ToString()
                                    Case "{FILE.VERSION.NAME}"
                                        Source = .InternalName.ToString()
                                    Case "{FILE.VERSION.COMMENTS}"
                                        Source = .Comments.ToString()
                                    Case "{FILE.VERSION.COMPANY}"
                                        Source = .CompanyName.ToString()
                                End Select
                            End With
                        End If
                    Case Formatter.ToUpper.StartsWith("{FILE.IMAGE")
                        Dim val As String = ""
                        If System.IO.File.Exists(Value) Then
                            Try
                                Dim img As System.Drawing.Image
                                img = System.Drawing.Image.FromFile(Value)
                                Try
                                    Select Case Formatter.ToUpper
                                        Case "{FILE.IMAGE}"
                                            val = "True"
                                        Case "{FILE.IMAGE.WIDTH}"
                                            val = img.Width.ToString
                                        Case "{FILE.IMAGE.HEIGHT}"
                                            val = img.Height.ToString
                                        Case "{FILE.IMAGE.RAWFORMAT}"
                                            val = img.RawFormat.ToString
                                        Case "{FILE.IMAGE.PIXELFORMAT}"
                                            val = img.PixelFormat.ToString
                                        Case "{FILE.IMAGE.HORIZONTALRESOLUTION}"
                                            val = img.HorizontalResolution.ToString
                                        Case "{FILE.IMAGE.VERTICALRESOLUTION}"
                                            val = img.VerticalResolution.ToString
                                        Case "{FILE.IMAGE.DIMENSIONS}"
                                            val = img.Width.ToString & "x" & img.Height.ToString
                                        Case "{FILE.IMAGE.ORIENTATION}"
                                            Dim orientation_index As Integer = Array.IndexOf(img.PropertyIdList, &H112)
                                            If (orientation_index < 0) Then
                                                orientation_index = 0
                                            End If
                                            Select Case orientation_index
                                                Case 0
                                                    val = "unknown"
                                                Case 1
                                                    val = "topleft"
                                                Case 2
                                                    val = "topright"
                                                Case 3
                                                    val = "bottomright"
                                                Case 4
                                                    val = "bottomleft"
                                                Case 5
                                                    val = "lefttop"
                                                Case 6
                                                    val = "righttop"
                                                Case 7
                                                    val = "rightbottom"
                                                Case 8
                                                    val = "leftbottom"
                                            End Select
                                        Case "{FILE.IMAGE.ROTATION}"
                                            Dim orientation_index As Integer = Array.IndexOf(img.PropertyIdList, &H112)
                                            If (orientation_index < 0) Then
                                                orientation_index = 0
                                            End If
                                            Select Case orientation_index
                                                Case 0
                                                    val = 0
                                                Case 1
                                                    val = 0
                                                Case 2
                                                    val = 0
                                                Case 3
                                                    val = 180
                                                Case 4
                                                    val = 180
                                                Case 5
                                                    val = 90
                                                Case 6
                                                    val = 90
                                                Case 7
                                                    val = 270
                                                Case 8
                                                    val = 270
                                            End Select
                                    End Select
                                Catch ex As Exception
                                    If Formatter.ToUpper = "{FILE.IMAGE}" Then
                                        val = "False"
                                    End If
                                End Try
                                If Not img Is Nothing Then
                                    img.Dispose()
                                    img = Nothing
                                End If
                            Catch ex As Exception
                            End Try
                        End If
                        Source = val
                    Case Else
                        Select Case Formatter.ToUpper
                            Case "{FILE.EXISTS}"
                                Try
                                    If (System.IO.File.GetAttributes(Value) And IO.FileAttributes.Directory) = 0 Then
                                        Source = IO.File.Exists(Value).ToString
                                    Else
                                        Source = IO.Directory.Exists(Value).ToString
                                    End If
                                Catch ex As Exception
                                    Source = "False"
                                End Try
                            Case "{FILE.FOLDER}"
                                Try
                                    If (System.IO.File.GetAttributes(Value) And IO.FileAttributes.Directory) = 0 Then
                                        Source = "False"
                                    Else
                                        Source = "True"
                                    End If
                                Catch ex As Exception
                                    Source = "False"
                                End Try
                            Case "{FILE.PATH}"
                                Source = IO.Path.GetDirectoryName(Value)
                            Case "{FILE.NAME}"
                                Source = IO.Path.GetFileName(Value)
                            Case "{FILE.NAMEONLY}"
                                Source = IO.Path.GetFileNameWithoutExtension(Value)
                            Case "{FILE.EXTENSION}"
                                Source = IO.Path.GetExtension(Value)
                            Case "{FILE}"
                                If IO.File.Exists(Value) AndAlso (System.IO.File.GetAttributes(Value) And IO.FileAttributes.Directory) = 0 Then
                                    Source = IO.File.ReadAllText(Value)
                                End If
                            Case "{FILE.BYTES}"
                                If IO.File.Exists(Value) AndAlso (System.IO.File.GetAttributes(Value) And IO.FileAttributes.Directory) = 0 Then
                                    Source = System.Text.Encoding.UTF32.GetString(IO.File.ReadAllBytes(Value))
                                End If
                            Case "{FILE.BASE64}"
                                If IO.File.Exists(Value) AndAlso (System.IO.File.GetAttributes(Value) And IO.FileAttributes.Directory) = 0 Then
                                    Source = Convert.ToBase64String(IO.File.ReadAllBytes(Value))
                                End If
                            Case "{FILE.LENGTH}", "{FILE.SIZE}"
                                If IO.File.Exists(Value) AndAlso (System.IO.File.GetAttributes(Value) And IO.FileAttributes.Directory) = 0 Then
                                    Dim fio As New IO.FileInfo(Value)
                                    Source = fio.Length.ToString
                                End If
                            Case "{FILE.HASSIZE}", "{FILE.HASLENGTH}"
                                Try
                                    If IO.File.Exists(Value) AndAlso (System.IO.File.GetAttributes(Value) And IO.FileAttributes.Directory) = 0 Then
                                        Dim fio As New IO.FileInfo(Value)
                                        If fio.Length > 0 Then
                                            Source = "True"
                                        Else
                                            Source = "False"
                                        End If
                                    Else
                                        Source = "False"
                                    End If
                                Catch ex As Exception
                                    Source = "False"
                                End Try
                            Case "{FILE.CREATED}"
                                If IO.File.Exists(Value) Then
                                    Source = IO.File.GetCreationTime(Value).ToString
                                End If
                            Case "{FILE.UPDATED}"
                                If IO.File.Exists(Value) Then
                                    Source = IO.File.GetLastWriteTime(Value).ToString
                                End If
                            Case "{FILE.ACCESSED}"
                                If IO.File.Exists(Value) Then
                                    Source = IO.File.GetLastAccessTime(Value).ToString
                                End If
                        End Select
                End Select

            Catch ex As Exception
                Source = ""
            End Try
            Return True
        End Function

        Public Overrides ReadOnly Property RenderTags() As String()
            Get
                Static str As String() = New String() {"file", "file.folder", "file.version", "file.version.major", "file.version.minor", "file.version.build", "file.version.revision", _
               "file.version.product", "file.version.product.major", "file.version.product.minor", "file.version.product.build", "file.version.product.revision", "file.version.product.name", _
               "file.version.name", "file.version.comments", "file.version.company", "file.image", "file.image.width", "file.image.height", "file.image.rawformat", "file.image.horizontalresolution", _
               "file.image.verticalresolution", "file.image.dimensions", "file.exists", "file.path", "file.name", "file.nameonly", "file.extension", "file.bytes", "file.length", "file.size", "file.created", "file.updated", "file.accessed", "file.image.orientation", "file.image.rotation", "file.haslength", "file.hassize", "file.base64"}
                Return str
            End Get
        End Property
        Public Overrides ReadOnly Property RenderTag() As String
            Get
                Return "file"
            End Get
        End Property
    End Class
End Namespace
