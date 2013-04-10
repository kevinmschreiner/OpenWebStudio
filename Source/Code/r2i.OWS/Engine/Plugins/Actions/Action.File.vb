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
Imports System.Xml.Serialization
Imports System.Collections.Generic
Imports System.Net.Mail
Imports System.Text
Imports r2i.OWS.Renderers
Imports r2i.OWS.Framework
Imports r2i.OWS.Framework.Utilities
Imports r2i.OWS.Framework.Plugins.Renderers
Imports r2i.OWS.Framework.Plugins.Actions
Imports r2i.OWS.Framework.Entities
Imports r2i.OWS.Framework.Utilities.Compatibility, r2i.OWS.Actions.Utilities
Imports r2i.OWS.Framework.DataAccess

Imports System.Web

Namespace r2i.OWS.Actions
    Public Class FileAction
        Inherits ActionBase



#Region "Debugging and Identification: Name,Style,Description"
        Public Overrides Function Description(ByRef act As MessageActionItem) As String
            Dim str As String = ""
            If Not act.Parameters Is Nothing AndAlso act.Parameters.Count > 0 Then
                Dim sourcetype As String = act.Parameters(MessageActionsConstants.ACTIONFILE_SOURCETYPE_KEY)
                str &= Utility.HTMLEncode("Source " & sourcetype & ": ")
                Select Case sourcetype
                    Case "Variable"
                        str &= " " & Utility.HTMLEncode(CStr(act.Parameters(MessageActionsConstants.ACTIONFILE_SOURCEVARIABLETYPE_KEY)).Replace("&lt;", "<").Replace("&gt;", ">")) & " "
                    Case "Response"
                        str &= " Response."
                    Case "EmailAttachment"
                        str &= " Email Attachment."
                    Case "SQL"
                        str &= ": " & act.Parameters(MessageActionsConstants.ACTIONFILE_SOURCEQUERY_KEY)
                End Select
                If Not sourcetype = "SQL" Then
                    If Not sourcetype = "Response" Then
                        str &= Utility.HTMLEncode(act.Parameters(MessageActionsConstants.ACTIONFILE_SOURCE_KEY))
                    End If
                    sourcetype = act.Parameters(MessageActionsConstants.ACTIONFILE_TRANSFORMTYPE_KEY)
                    Select Case sourcetype
                        Case "Image"
                            str &= " Transform Image "
                            str &= Utility.HTMLEncode(act.Parameters(MessageActionsConstants.ACTIONFILE_IMAGEWIDTH_KEY))
                            str &= Utility.HTMLEncode(act.Parameters(MessageActionsConstants.ACTIONFILE_IMAGEWIDTHTYPE_KEY) & " by ")
                            str &= Utility.HTMLEncode(act.Parameters(MessageActionsConstants.ACTIONFILE_IMAGEHEIGHT_KEY))
                            str &= Utility.HTMLEncode(act.Parameters(MessageActionsConstants.ACTIONFILE_IMAGEHEIGHTYPE_KEY))
                        Case "XML"
                            str &= " Transform XML."
                        Case "File"
                            Dim fp As String = act.Parameters(MessageActionsConstants.ACTIONFILE_FILETASK_KEY)
                            If fp.Length > 0 Then
                                str &= Utility.HTMLEncode(" File " & fp & ".")
                            End If
                    End Select
                Else
                    str &= "Undefined"
                End If
            Else
                str &= " No query or parameters defined."
            End If
            Return str
        End Function
        Public Overrides Function Name() As String
            Return "File"
        End Function
        Public Overrides Function Title(ByRef act As MessageActionItem) As String
            Return "File"
        End Function
        Public Overrides Function Style() As String
            Return ""
        End Function
#End Region

        Private Class FileItem
            Public VirtualPath As String
            Public LocalPath As String
            Public RelativePath As String
            Public RelativeLocalPath As String
            Public FileName As String
            Public Source As System.IO.Stream
            Public Data As Object
            Public AutoClose As Boolean = True
            Public Abort As Boolean = False
            Private mExtension As String = Nothing
            Private mContentType As String = Nothing
            Private mWidth As Integer = 0
            Private mHeight As Integer = 0

            Public Sub New(ByVal FileName As String)
                Me.FileName = FileName
                Me.Source = New IO.MemoryStream
                If Not Me.Source Is Nothing Then
                    Me.Source.Position = 0
                End If
            End Sub
            Public Sub New(ByVal FileName As String, ByVal Source As IO.Stream)
                Me.FileName = FileName
                Me.Source = Source
                If Not Me.Source Is Nothing Then
                    Me.Source.Position = 0
                End If
            End Sub

            Public Property ContentType() As String
                Get
                    If mContentType Is Nothing OrElse mContentType.Length = 0 Then
                        mContentType = Utility.GetContentType(Me.Extension)
                    End If
                    Return mContentType
                End Get
                Set(ByVal value As String)
                    mContentType = value
                End Set
            End Property

            Public Property Extension() As String
                Get
                    If mExtension Is Nothing OrElse mExtension.Length = 0 Then
                        mExtension = IO.Path.GetExtension(Me.FileName)
                    End If
                    Return mExtension
                End Get
                Set(ByVal value As String)
                    mExtension = value
                End Set
            End Property

            Public Property Width() As Integer
                Get
                    Return mWidth
                End Get
                Set(ByVal value As Integer)
                    mWidth = value
                End Set
            End Property

            Public Property Height() As Integer
                Get
                    Return mHeight
                End Get
                Set(ByVal value As Integer)
                    mHeight = value
                End Set
            End Property

            Public Shared Function GetVirtualPath(ByVal FullPath As String, ByVal Root As String) As String
                Dim sPath As String = ""

                If FullPath.ToLower().StartsWith(Root.ToLower()) Then
                    sPath = FullPath.Substring(Root.Length)
                    sPath = sPath.Replace("\", "/")
                    If Not sPath.StartsWith("/") Then sPath = "/" & sPath
                    sPath = "~" & sPath
                End If

                Return sPath
            End Function
            Public Shared Function GetRelativePath(ByVal FullPath As String, ByVal Root As String) As String
                Dim sPath As String = ""

                If FullPath.ToLower().StartsWith(Root.ToLower()) Then
                    sPath = FullPath.Substring(Root.Length)
                    If sPath = "/" Then sPath = ""
                End If

                Return sPath
            End Function

            Public Sub SetSourceStream()
                If IO.File.Exists(Me.LocalPath) Then
                    Dim fio As New IO.FileInfo(Me.LocalPath)
                    Utility.StreamTransfer(fio.OpenRead(), Source)
                End If
            End Sub
        End Class

        Private Function BuildSourceStreams(ByRef Caller As Runtime, ByRef sharedds As System.Data.DataSet, ByVal act As MessageActionItem, ByRef Debugger As Framework.Debugger) As List(Of FileItem)
            Dim parms As SerializableDictionary(Of String, Object) = act.Parameters
            Dim SourceType As String = Utility.GetDictionaryValue(parms, MessageActionsConstants.ACTIONFILE_SOURCETYPE_KEY).Replace("&lt;", "<").Replace("&gt;", ">")
            Dim SourceStreams As New List(Of FileItem)
            Dim DestinationType As String = Utility.GetDictionaryValue(parms, MessageActionsConstants.ACTIONFILE_DESTINATIONTYPE_KEY).Replace("&lt;", "<").Replace("&gt;", ">")
            Dim TransformationType As String = Utility.GetDictionaryValue(parms, MessageActionsConstants.ACTIONFILE_TRANSFORMTYPE_KEY)
            Dim TransformationTypeTask As String = ""
            Try
                TransformationTypeTask = Utility.GetDictionaryValue(parms, MessageActionsConstants.ACTIONFILE_FILETASK_KEY)
            Catch ex As Exception
                TransformationTypeTask = ""
            End Try

            Try
                '' IDENTIFY AND BUILD THE SOURCE STREAMS
                Select Case SourceType
                    Case "SQL"
                        Dim Query As String = Utility.GetDictionaryValue(parms, MessageActionsConstants.ACTIONFILE_SOURCEQUERY_KEY)
                        Query = Caller.Engine.RenderString(sharedds, Query, Caller.Engine.CapturedMessages, False, isPreRender:=False)
                        'EXECUTE THE QUERY                    
                        Query = Caller.Engine.RenderQuery(sharedds, Caller.FilterField, Caller.FilterText, Caller.Engine.RecordsPerPage, Caller.Engine.CapturedMessages, Debugger, Query)
                        Dim Connection As String = Utility.GetDictionaryValue(parms, MessageActionsConstants.ACTIONFILE_SOURCEQUERY_CONNECTION_KEY)
                        If Not Connection Is Nothing AndAlso Connection.Length > 0 Then
                            Connection = Caller.Engine.RenderString(sharedds, Connection, Caller.Engine.CapturedMessages, False, isPreRender:=False)
                            If Not Connection Is Nothing AndAlso Connection.Length = 0 Then
                                Connection = Nothing
                            End If
                        End If
                        Dim ds As DataSet = Caller.Engine.GetData(False, Query, Caller.FilterField, Caller.FilterText, Debugger, False, Nothing, Nothing, False, -1, Connection)
                        If Not ds Is Nothing AndAlso ds.Tables.Count > 0 Then
                            If Queries.Directory.IsSystemFileDatatable(ds.Tables(0)) Then
                                For Each drFile As DataRow In ds.Tables(0).Rows
                                    Dim fi As New FileItem(drFile.Item("Name"))
                                    Dim sFullName As String = Caller.Engine.RenderString(sharedds, drFile.Item("FullName"), Caller.Engine.CapturedMessages, False, isPreRender:=False)
                                    Dim sRootPath As String = Caller.Engine.RenderString(sharedds, drFile.Item("RootPath"), Caller.Engine.CapturedMessages, False, isPreRender:=False)

                                    fi.ContentType = drFile.Item("ContentType")

                                    If IO.File.Exists(sFullName) Then
                                        fi.LocalPath = sFullName
                                        fi.RelativePath = FileItem.GetVirtualPath(sFullName, Caller.Engine.Request.MapPath(Caller.Engine.Request.ApplicationPath))
                                        fi.VirtualPath = FileItem.GetVirtualPath(sFullName, Utility.XMLPropertyParse_Quick(Query, "DIRECTORY"))
                                    Else
                                        If sFullName.StartsWith("~") Then
                                            sFullName = Caller.Engine.Request.ApplicationPath & sFullName.Substring(1)
                                        End If
                                        If sRootPath.StartsWith("~") Then
                                            sRootPath = Caller.Engine.Request.ApplicationPath & sRootPath.Substring(1)
                                        End If
                                        sRootPath = Caller.Engine.Request.MapPath(sRootPath)
                                        fi.LocalPath = Caller.Engine.Request.MapPath(sFullName)
                                        fi.RelativePath = FileItem.GetRelativePath(fi.LocalPath, sRootPath).Replace("\", "/")
                                        fi.RelativeLocalPath = FileItem.GetRelativePath(fi.LocalPath, sRootPath).Replace("/", "\")
                                        fi.VirtualPath = sFullName
                                    End If
                                    'TODO: MAKE SURE that everything calls SetSourceStream() at the right time. fi.SetSourceStream()

                                    If Not Debugger Is Nothing Then
                                        r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "SQL File Added: " & fi.VirtualPath, True)
                                    End If

                                    SourceStreams.Add(fi)
                                Next
                            Else
                                ''VERSION 2.0.4.76 - Improved to allow for getting raw data directly from data (like a BLOB field)

                                If TransformationType.ToUpper() <> "" AndAlso TransformationTypeTask.ToUpper() <> "CSV" AndAlso TransformationTypeTask.ToUpper() <> "TAB" Then
                                    If Not ds Is Nothing AndAlso ds.Tables.Count > 0 Then
                                        Dim sourceColumn As Integer = -1
                                        Dim extensionColumn As Integer = -1
                                        Dim filenamecolumn As Integer = -1
                                        Dim contenttypecolumn As Integer = -1

                                        Dim dc As DataColumn
                                        For Each dc In ds.Tables(0).Columns
                                            Select Case dc.ColumnName.ToUpper
                                                Case "FILECONTENT", "CONTENT", "IMAGE", "SOURCE", "DATA"
                                                    sourceColumn = dc.Ordinal
                                                Case "NAME", "FILENAME"
                                                    filenamecolumn = dc.Ordinal
                                                Case "CONTENTTYPE", "MIME", "TYPE", "MIMETYPE"
                                                    contenttypecolumn = dc.Ordinal
                                                Case "EXT", "EXTENSION"
                                                    extensionColumn = dc.Ordinal
                                            End Select
                                        Next
                                        If sourceColumn + extensionColumn + filenamecolumn + contenttypecolumn = -4 Then
                                            'NO COLUMN NAMES FOUND
                                            sourceColumn = 0
                                            If ds.Tables(0).Columns.Count > 1 Then
                                                filenamecolumn = 1
                                            End If
                                            If ds.Tables(0).Columns.Count > 2 Then
                                                extensionColumn = 2
                                            End If
                                            If ds.Tables(0).Columns.Count > 3 Then
                                                contenttypecolumn = 3
                                            End If
                                        End If
                                        If sourceColumn = -1 Then
                                            Dim gap As Integer = 0
                                            Dim cols As New List(Of Integer)
                                            If filenamecolumn > -1 Then
                                                cols.Add(filenamecolumn)
                                            End If
                                            If extensionColumn > -1 Then
                                                cols.Add(extensionColumn)
                                            End If
                                            If contenttypecolumn > -1 Then
                                                cols.Add(contenttypecolumn)
                                            End If
                                            If cols.Count > 0 Then
                                                cols.Sort()
                                                Dim i As Integer
                                                For Each i In cols
                                                    If i > gap Then
                                                        Exit For
                                                    Else
                                                        gap = i + 1
                                                    End If
                                                Next
                                            End If
                                            If gap > ds.Tables(0).Columns.Count OrElse gap = -1 Then
                                                gap = 0
                                            End If
                                            sourceColumn = gap
                                        End If

                                        For Each dr As DataRow In ds.Tables(0).Rows

                                            Dim fi As New FileItem("")
                                            Dim bData() As Byte = dr(sourceColumn)
                                            If filenamecolumn > -1 Then
                                                Try
                                                    fi.FileName = dr(filenamecolumn)
                                                Catch ex As Exception
                                                End Try
                                            End If
                                            If extensionColumn > -1 Then
                                                Try
                                                    fi.Extension = dr(extensionColumn)
                                                Catch ex As Exception
                                                End Try
                                            End If
                                            If contenttypecolumn > -1 Then
                                                Try
                                                    fi.ContentType = dr(contenttypecolumn)
                                                Catch ex As Exception
                                                End Try
                                            End If

                                            fi.Source = New IO.MemoryStream(bData)
                                            If Not Debugger Is Nothing Then
                                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "SQL Data Written: " & fi.Source.Length.ToString("#,##0") & " bytes", True)
                                            End If
                                            SourceStreams.Add(fi)
                                        Next
                                    Else
                                        If Not Debugger Is Nothing Then
                                            r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "SQL Data Written: NO RECORDS RETURNED", True)
                                        End If
                                    End If
                                Else
                                    Dim fi As New FileItem("")
                                    fi.Data = ds
                                    If Not Debugger Is Nothing Then
                                        Dim iRecords As Integer = 0
                                        If Not ds Is Nothing AndAlso ds.Tables.Count > 0 AndAlso ds.Tables(0).Rows.Count > 0 Then
                                            iRecords = ds.Tables(0).Rows.Count
                                        End If
                                        r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "SQL Data Written: " & iRecords.ToString("#,##0") & " records", True)
                                    End If
                                    SourceStreams.Add(fi)
                                End If
                            End If
                        End If
                    Case "Path"
                        Dim bRecurseSubDirectories As Boolean = False
                        Dim sFileFilter As String = "*"
                        Dim sDirectoryFilter As String = "*"
                        Dim bRetry As Boolean = False
                        Dim sPath As String = Utility.GetDictionaryValue(parms, MessageActionsConstants.ACTIONFILE_SOURCE_KEY)
                        sPath = Caller.Engine.RenderString(sharedds, sPath, Caller.Engine.CapturedMessages, False, isPreRender:=False)
                        If Not Debugger Is Nothing Then
                            r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Path File (Initial): " & sPath, True)
                        End If

                        If sPath.Contains("**") Then
                            bRecurseSubDirectories = True
                            sDirectoryFilter = sPath.Substring(sPath.IndexOf("**") + 2)
                            If sDirectoryFilter.LastIndexOf("/") >= 0 Then
                                Dim sCheck As String = sDirectoryFilter.Substring(sDirectoryFilter.LastIndexOf("/") + 1)
                                If sCheck.Length > 0 Then
                                    sFileFilter = sCheck
                                End If
                                sDirectoryFilter = sDirectoryFilter.Substring(0, sDirectoryFilter.LastIndexOf("/"))
                            End If
                            If sDirectoryFilter = "" Then
                                sDirectoryFilter = "*"
                            End If
                            sPath = sPath.Substring(0, sPath.IndexOf("**"))

                            Try
                                If sPath.Length > 2 AndAlso (sPath.StartsWith("//") OrElse sPath.StartsWith("\\")) Then
                                    'sPath is a UNC path
                                    sPath = sPath
                                Else
                                    sPath = Caller.Engine.Request.MapPath(sPath)
                                End If
                            Catch
                                bRetry = True
                                'sFileFilter = IO.Path.GetFileName(sPath)
                                sPath = IO.Path.GetDirectoryName(sPath)
                            End Try
                            If bRetry Then
                                Try
                                    sPath = Caller.Engine.Request.MapPath(sPath)
                                Catch
                                End Try
                            End If

                        Else


                            Try
                                If sPath.Length > 2 AndAlso (sPath.StartsWith("//") OrElse sPath.StartsWith("\\")) Then
                                    'sPath is a UNC path
                                    sPath = sPath
                                Else
                                    sPath = Caller.Engine.Request.MapPath(sPath)
                                End If
                            Catch
                                bRetry = True
                                sFileFilter = IO.Path.GetFileName(sPath)
                                sPath = IO.Path.GetDirectoryName(sPath)
                            End Try
                            If bRetry Then
                                Try
                                    sPath = Caller.Engine.Request.MapPath(sPath)
                                Catch
                                End Try
                            End If
                            If Not IO.Directory.Exists(sPath) Then 'this must be a file
                                Dim sDir As String = IO.Path.GetDirectoryName(sPath)
                                sFileFilter = IO.Path.GetFileName(sPath)
                                sPath = sDir
                            ElseIf IO.Directory.Exists(sPath) And Not sFileFilter Is Nothing AndAlso Not sFileFilter.Length = 0 Then
                                '02/22/2010 [KMS]: If the sFileFilter is meant for MULTIPLE files, this logic clears the filter.
                                'If Not IO.File.Exists(IO.Path.Combine(sPath, sFileFilter)) Then
                                '    sFileFilter = ""
                                'End If
                            Else
                                sFileFilter = ""
                            End If
                        End If
                        If Not Debugger Is Nothing Then
                            r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Path File (Mapped): " & sPath & " using filter '" & sFileFilter & "'", True)
                        End If

                        RecurseGetFiles(SourceStreams, sPath, Caller.Engine.Request.MapPath(Caller.Engine.Request.ApplicationPath), sPath, Debugger, bRecurseSubDirectories, sFileFilter, sDirectoryFilter)
                    Case "Variable"
                        Dim Source_VariableType As String = Utility.GetDictionaryValue(parms, MessageActionsConstants.ACTIONFILE_SOURCEVARIABLETYPE_KEY).Replace("&lt;", "<").Replace("&gt;", ">")
                        Dim Source_VariableName As String = Utility.GetDictionaryValue(parms, MessageActionsConstants.ACTIONFILE_SOURCE_KEY)

                        Source_VariableName = Caller.Engine.RenderString(sharedds, Source_VariableName, Caller.Engine.CapturedMessages, False, isPreRender:=False)
                        Select Case Source_VariableType.Replace("&lt;", "<").Replace("&gt;", ">").ToUpper
                            Case "<FORM>"
                                'THIS FORM IS THE FILE FORM, NOT A FORM PARAMETERS
                                Dim FileInput As System.Web.HttpPostedFile = Nothing
                                Try
                                    FileInput = Caller.Engine.Request.Files.Get(Source_VariableName)
                                Catch ex As Exception
                                    'THERE WAS NO FILE INPUT WITH THAT NAME, CHECK TO MAKE SURE IT WASNT STANDARD TEXT
                                End Try
                                If Not FileInput Is Nothing Then
                                    FileInput.InputStream.Position = 0
                                    Dim fi As New FileItem(FileInput.FileName, FileInput.InputStream)
                                    fi.RelativeLocalPath = FileInput.FileName
                                    fi.ContentType = FileInput.ContentType
                                    fi.AutoClose = False

                                    SourceStreams.Add(fi)
                                    If Not Debugger Is Nothing Then
                                        r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Form File: " & FileInput.FileName, True)
                                    End If

                                    'SourceFileName = FileInput.FileName
                                    'SourceStream = FileInput.InputStream
                                    'SourceStream.Position = 0
                                    'SourceExtension = FileInput.FileName.Substring(FileInput.FileName.LastIndexOf("."))
                                    'SourceContentType = FileInput.ContentType
                                Else
                                    Try

                                        If Not Caller.Engine.Request.Form(Source_VariableName) Is Nothing Then
                                            Dim frmContent As String = Caller.Engine.Request.Form(Source_VariableName)
                                            Dim ms As New IO.MemoryStream
                                            Dim sw As New IO.StreamWriter(ms)
                                            sw.Write(frmContent)
                                            sw.Flush()
                                            sw = Nothing
                                            Dim fi As New FileItem("", ms)
                                            fi.ContentType = "text/plain"
                                            fi.AutoClose = True
                                            SourceStreams.Add(fi)
                                            If Not Debugger Is Nothing Then
                                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Form File (from Text): " & Source_VariableName, True)
                                            End If
                                        End If
                                    Catch ex As Exception

                                    End Try
                                End If
                            Case "<QUERYSTRING>"
                                If Not Debugger Is Nothing Then
                                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Added File from Querystring", True)
                                End If
                                SourceStreams.Add(New FileItem(Source_VariableName, New IO.MemoryStream(System.Text.UTF8Encoding.UTF8.GetBytes(Caller.Engine.Request.QueryString.Item(Source_VariableName)))))
                            Case "<COOKIE>"
                                If Not Debugger Is Nothing Then
                                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Added File from Cookie", True)
                                End If
                                SourceStreams.Add(New FileItem(Source_VariableName, New IO.MemoryStream(System.Text.UTF8Encoding.UTF8.GetBytes(Caller.Engine.Request.Cookies.Item(Source_VariableName).Value))))
                            Case "<SESSION>"
                                Dim sourceObj As Object = Caller.Engine.Session(Source_VariableName)
                                Dim ms As New IO.MemoryStream
                                If Not TypeOf sourceObj Is Byte() Then
                                    'sourceObj = Convert.FromBase64String(sourceObj)
                                    If TypeOf sourceObj Is String Then
                                        Dim mwriter As New IO.StreamWriter(ms)
                                        mwriter.Write(sourceObj)
                                        mwriter.Flush()
                                    End If
                                Else
                                    ms = New IO.MemoryStream(CType(sourceObj, Byte()))
                                End If
                                If Not Debugger Is Nothing Then
                                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Added File from Session", True)
                                End If
                                'SourceStream = New IO.MemoryStream(CType(sourceObj, Byte()))
                                SourceStreams.Add(New FileItem(Source_VariableName, ms))
                            Case "<VIEWSTATE>"
                                If Not Debugger Is Nothing Then
                                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Added File from Viewstate", True)
                                End If
                                'SourceStream = New IO.MemoryStream(CType(Caller.Engine.ViewState(Source_VariableName), Byte()))
                                SourceStreams.Add(New FileItem(Source_VariableName, New IO.MemoryStream(CType(Caller.Engine.ViewState(Source_VariableName), Byte()))))
                            Case "<CONTEXT>"
                                If Not Debugger Is Nothing Then
                                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Added File from Context", True)
                                End If
                                SourceStreams.Add(New FileItem(Source_VariableName, CType(Caller.Engine.Context.Items(Source_VariableName), System.IO.MemoryStream)))
                            Case "<ACTION>"
                                'Caller.Engine.ActionVariable("Encoding.Session." & key) = "UTF8"
                                If Not Debugger Is Nothing Then
                                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Added File from Action: " & Source_VariableName, True)
                                End If
                                Dim sourceObj As Object = Caller.Engine.ActionVariable(Source_VariableName)
                                Dim ms As New IO.MemoryStream
                                If Not TypeOf sourceObj Is Byte() Then
                                    'sourceObj = Convert.FromBase64String(sourceObj)
                                    If TypeOf sourceObj Is String Then
                                        Dim mwriter As New IO.StreamWriter(ms)
                                        mwriter.Write(sourceObj)
                                        mwriter.Flush()
                                    End If
                                Else
                                    ms = New IO.MemoryStream(CType(sourceObj, Byte()))
                                End If
                                SourceStreams.Add(New FileItem(Source_VariableName, ms))
                            Case "<CUSTOM>"
                                If Not Debugger Is Nothing Then
                                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Added File from 'Custom'", True)
                                End If
                                'SourceStream = New IO.MemoryStream(Source_VariableName)
                                SourceStreams.Add(New FileItem(Source_VariableName, New IO.MemoryStream()))
                        End Select
                End Select
            Catch ex As Exception
                If Not Debugger Is Nothing Then
                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Fatal Source Failure: " & ex.ToString, True)
                End If
            End Try
            If Not Debugger Is Nothing Then
                If Not SourceStreams Is Nothing Then
                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, SourceStreams.Count & " source(s) were identified", True)
                Else
                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "No sources were identified", True)
                End If
            End If
            Return SourceStreams
        End Function
        Private Sub ApplyTransforms(ByRef Caller As Runtime, ByRef sharedds As System.Data.DataSet, ByVal act As MessageActionItem, ByRef SourceStreams As List(Of FileItem), ByVal DestinationTarget As String, ByVal DestinationType As String, ByVal DestinationTargetMimeType As String, ByRef Debugger As Framework.Debugger)
            Dim parms As SerializableDictionary(Of String, Object) = act.Parameters
            Dim TransformationType As String = Utility.GetDictionaryValue(parms, MessageActionsConstants.ACTIONFILE_TRANSFORMTYPE_KEY)

            Try
                Select Case TransformationType.ToUpper
                    Case "IMAGE"
                        If Not Debugger Is Nothing Then
                            r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Processing Image Transformation", True)
                        End If

                        Dim ImageTransformType As String = Utility.GetDictionaryValue(parms, MessageActionsConstants.ACTIONFILE_IMAGETRANSFORMTYPE_KEY)
                        Dim ImageFont As String = Utility.GetDictionaryValue(parms, MessageActionsConstants.ACTIONFILE_IMAGEFONT_KEY)
                        Dim ImageSize As String = Utility.GetDictionaryValue(parms, MessageActionsConstants.ACTIONFILE_IMAGESIZE_KEY)
                        Dim ImageSizeType As String = Utility.GetDictionaryValue(parms, MessageActionsConstants.ACTIONFILE_IMAGESIZETYPE_KEY)
                        Dim ImageColor As String = Utility.GetDictionaryValue(parms, MessageActionsConstants.ACTIONFILE_IMAGECOLOR_KEY)
                        Dim ImageBGColor As String = Utility.GetDictionaryValue(parms, MessageActionsConstants.ACTIONFILE_IMAGEBGCOLOR_KEY)
                        Dim ImageWarp As String = Utility.GetDictionaryValue(parms, MessageActionsConstants.ACTIONFILE_IMAGEWARP_KEY)
                        If ImageWarp Is Nothing OrElse ImageWarp.Length = 0 Then
                            ImageWarp = ""
                        End If
                        Dim ImageText As String = Utility.GetDictionaryValue(parms, MessageActionsConstants.ACTIONFILE_IMAGETEXT_KEY)
                        Dim ImageX As String = Utility.GetDictionaryValue(parms, MessageActionsConstants.ACTIONFILE_IMAGEX_KEY)
                        Dim ImageY As String = Utility.GetDictionaryValue(parms, MessageActionsConstants.ACTIONFILE_IMAGEY_KEY)
                        If ImageX Is Nothing OrElse ImageX.Length = 0 OrElse Not IsNumeric(ImageX) Then
                            ImageX = "0"
                        End If
                        If ImageY Is Nothing OrElse ImageY.Length = 0 OrElse Not IsNumeric(ImageY) Then
                            ImageY = "0"
                        End If
                        If ImageText Is Nothing OrElse ImageText.Length = 0 Then
                            ImageText = ""
                        End If
                        If Not parms.ContainsKey(MessageActionsConstants.ACTIONFILE_IMAGETEXT_KEY) Then
                            ImageText = Nothing
                        End If
                        Dim Rotation As String = Utility.GetDictionaryValue(parms, MessageActionsConstants.ACTIONFILE_IMAGEROTATION_KEY)
                        Dim Width As String = Utility.GetDictionaryValue(parms, MessageActionsConstants.ACTIONFILE_IMAGEWIDTH_KEY)
                        Dim WidthUnit As String = Utility.GetDictionaryValue(parms, MessageActionsConstants.ACTIONFILE_IMAGEWIDTHTYPE_KEY)
                        Dim Height As String = Utility.GetDictionaryValue(parms, MessageActionsConstants.ACTIONFILE_IMAGEHEIGHT_KEY)
                        Dim HeightUnit As String = Utility.GetDictionaryValue(parms, MessageActionsConstants.ACTIONFILE_IMAGEHEIGHTYPE_KEY)
                        Dim Quality As String = Utility.GetDictionaryValue(parms, MessageActionsConstants.ACTIONFILE_IMAGEQUALITY_KEY)

                        If ImageTransformType Is Nothing OrElse ImageTransformType.Length = 0 Then
                            ImageTransformType = "Size"
                        End If

                        Select Case ImageTransformType.ToLower
                            Case "crop"
                                Width = Caller.Engine.RenderString(sharedds, Width, Caller.Engine.CapturedMessages, False, isPreRender:=False)
                                Height = Caller.Engine.RenderString(sharedds, Height, Caller.Engine.CapturedMessages, False, isPreRender:=False)
                                ImageX = Caller.Engine.RenderString(sharedds, ImageX, Caller.Engine.CapturedMessages, False, isPreRender:=False)
                                ImageY = Caller.Engine.RenderString(sharedds, ImageY, Caller.Engine.CapturedMessages, False, isPreRender:=False)
                                Quality = Caller.Engine.RenderString(sharedds, Quality, Caller.Engine.CapturedMessages, False, isPreRender:=False)

                                If Quality Is Nothing OrElse Quality.Length = 0 Then
                                    Quality = "-1"
                                End If

                                Dim aWidth As Integer
                                Dim aHeight As Integer
                                Dim aX As Integer
                                Dim aY As Integer
                                Dim aQuality As Long
                                If IsNumeric(Width) Then
                                    aWidth = CInt(Width)
                                End If
                                If IsNumeric(Height) Then
                                    aHeight = CInt(Height)
                                End If
                                If IsNumeric(Quality) Then
                                    aQuality = CLng(Quality)
                                Else
                                    aQuality = -1
                                End If
                                If IsNumeric(ImageX) Then
                                    aX = CInt(ImageX)
                                Else
                                    aX = 0
                                End If
                                If IsNumeric(ImageY) Then
                                    aY = CInt(ImageY)
                                Else
                                    aY = 0
                                End If


                                If aWidth > 0 Or aHeight > 0 Then
                                    Dim BMP As System.Drawing.Bitmap = Nothing
                                    For Each fi As FileItem In SourceStreams
                                        Try
                                            If fi.Source.Length = 0 Then fi.SetSourceStream()
                                            BMP = New System.Drawing.Bitmap(fi.Source)

                                            If aWidth = BMP.Width And aHeight = BMP.Height And aQuality = -1 Then
                                                'DO NOTHING, LEAVING THE STREAM INTACT
                                            Else
                                                If aQuality <= 0 Then
                                                    aQuality = 100
                                                End If
                                                If Not fi.Source Is Nothing Then
                                                    If fi.AutoClose Then
                                                        fi.Source.Close()
                                                    End If
                                                    fi.Source = Nothing
                                                End If
                                                'fi.Source = r2i.OWS.Framework.Utilities.Engine.Graphics.ResizeImage(BMP, DestinationTarget, aWidth, aHeight, aQuality, Debugger)
                                                fi.Source = r2i.OWS.Framework.Utilities.Engine.Graphics.Crop(BMP, DestinationTarget, 7, aWidth, aHeight, aX, aY, aQuality, Debugger)
                                            End If
                                            fi.Width = aWidth
                                            fi.Height = aHeight
                                            If Not Debugger Is Nothing AndAlso Not fi.FileName Is Nothing Then
                                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, fi.FileName & " cropped to " & fi.Width & "x" & fi.Height, True)
                                            End If
                                        Catch ex As Exception
                                            If Not Debugger Is Nothing Then
                                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Failed to resize image: " & ex.ToString(), True)
                                            End If
                                        Finally
                                            'VERSION: 2.0 Fixed Image Closing Behavuour
                                            If Not BMP Is Nothing Then
                                                BMP.Dispose()
                                            End If
                                            BMP = Nothing
                                        End Try
                                    Next
                                End If
                            Case "smartcrop"
                                Width = Caller.Engine.RenderString(sharedds, Width, Caller.Engine.CapturedMessages, False, isPreRender:=False)
                                Height = Caller.Engine.RenderString(sharedds, Height, Caller.Engine.CapturedMessages, False, isPreRender:=False)
                                Quality = Caller.Engine.RenderString(sharedds, Quality, Caller.Engine.CapturedMessages, False, isPreRender:=False)

                                If Quality Is Nothing OrElse Quality.Length = 0 Then
                                    Quality = "-1"
                                End If

                                Dim aWidth As Integer
                                Dim aHeight As Integer
                                Dim aQuality As Long
                                If IsNumeric(Width) Then
                                    aWidth = CInt(Width)
                                End If
                                If IsNumeric(Height) Then
                                    aHeight = CInt(Height)
                                End If
                                If IsNumeric(Quality) Then
                                    aQuality = CLng(Quality)
                                Else
                                    aQuality = -1
                                End If

                                If aWidth > 0 Or aHeight > 0 Then
                                    Dim BMP As System.Drawing.Bitmap = Nothing
                                    For Each fi As FileItem In SourceStreams
                                        Try
                                            If fi.Source.Length = 0 Then fi.SetSourceStream()
                                            BMP = New System.Drawing.Bitmap(fi.Source)

                                            If aWidth = BMP.Width And aHeight = BMP.Height And aQuality = -1 Then
                                                'DO NOTHING, LEAVING THE STREAM INTACT
                                            Else
                                                If aQuality <= 0 Then
                                                    aQuality = 100
                                                End If
                                                If Not fi.Source Is Nothing Then
                                                    If fi.AutoClose Then
                                                        fi.Source.Close()
                                                    End If
                                                    fi.Source = Nothing
                                                End If
                                                'fi.Source = r2i.OWS.Framework.Utilities.Engine.Graphics.ResizeImage(BMP, DestinationTarget, aWidth, aHeight, aQuality, Debugger)
                                                fi.Source = r2i.OWS.Framework.Utilities.Engine.Graphics.SmartCropAndScale(BMP, DestinationTarget, 7, aWidth, aHeight, WidthUnit, HeightUnit, aQuality, Debugger)
                                            End If
                                            fi.Width = aWidth
                                            fi.Height = aHeight
                                            If Not Debugger Is Nothing AndAlso Not fi.FileName Is Nothing Then
                                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, fi.FileName & " resized to " & fi.Width & "x" & fi.Height, True)
                                            End If
                                        Catch ex As Exception
                                            If Not Debugger Is Nothing Then
                                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Failed to resize image: " & ex.ToString(), True)
                                            End If
                                        Finally
                                            'VERSION: 2.0 Fixed Image Closing Behavuour
                                            If Not BMP Is Nothing Then
                                                BMP.Dispose()
                                            End If
                                            BMP = Nothing
                                        End Try
                                    Next
                                End If
                            Case "rotate"
                                Rotation = Caller.Engine.RenderString(sharedds, Rotation, Caller.Engine.CapturedMessages, False, isPreRender:=False)

                                Dim BMP As System.Drawing.Bitmap = Nothing
                                For Each fi As FileItem In SourceStreams
                                    Try
                                        If fi.Source.Length = 0 Then fi.SetSourceStream()
                                        BMP = New System.Drawing.Bitmap(fi.Source)


                                        If Not fi.Source Is Nothing Then
                                            If fi.AutoClose Then
                                                fi.Source.Close()
                                            End If
                                            fi.Source = Nothing
                                        End If
                                        fi.Source = r2i.OWS.Framework.Utilities.Engine.Graphics.RotateImage(BMP, DestinationTarget, Rotation, 100, Debugger)

                                        If Not Debugger Is Nothing AndAlso Not fi.FileName Is Nothing Then
                                            r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, fi.FileName & " rotated to " & Rotation, True)
                                        End If
                                    Catch ex As Exception
                                        If Not Debugger Is Nothing Then
                                            r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Failed to rotate image: " & ex.ToString(), True)
                                        End If
                                    Finally
                                        'VERSION: 2.0 Fixed Image Closing Behavuour
                                        If Not BMP Is Nothing Then
                                            BMP.Dispose()
                                        End If
                                        BMP = Nothing
                                    End Try
                                Next
                            Case "size"
                                Width = Caller.Engine.RenderString(sharedds, Width, Caller.Engine.CapturedMessages, False, isPreRender:=False)
                                Height = Caller.Engine.RenderString(sharedds, Height, Caller.Engine.CapturedMessages, False, isPreRender:=False)
                                Quality = Caller.Engine.RenderString(sharedds, Quality, Caller.Engine.CapturedMessages, False, isPreRender:=False)

                                If Quality Is Nothing OrElse Quality.Length = 0 Then
                                    Quality = "-1"
                                End If

                                Dim aWidth As Integer
                                Dim aHeight As Integer
                                Dim aQuality As Long
                                If IsNumeric(Width) Then
                                    aWidth = CInt(Width)
                                End If
                                If IsNumeric(Height) Then
                                    aHeight = CInt(Height)
                                End If
                                If IsNumeric(Quality) Then
                                    aQuality = CLng(Quality)
                                Else
                                    aQuality = -1
                                End If

                                If aWidth > 0 Or aHeight > 0 Then
                                    Dim BMP As System.Drawing.Bitmap = Nothing
                                    For Each fi As FileItem In SourceStreams
                                        Try
                                            If fi.Source.Length = 0 Then fi.SetSourceStream()
                                            BMP = New System.Drawing.Bitmap(fi.Source)
                                            Dim isFixedWidth As Boolean = False
                                            Dim isFixedHeight As Boolean = False
                                            If Not WidthUnit = "%" And aWidth > 0 Then
                                                isFixedWidth = True
                                            End If
                                            If Not HeightUnit = "%" And aHeight > 0 Then
                                                isFixedHeight = True
                                            End If
                                            If WidthUnit = "%" And aWidth > 0 Then
                                                aWidth = BMP.Width * (aWidth / 100.0)
                                            End If
                                            If HeightUnit = "%" And aHeight > 0 Then
                                                aHeight = BMP.Height * (aHeight / 100.0)
                                            End If
                                            If aWidth > 0 And aHeight <= 0 Then
                                                aHeight = BMP.Height * (aWidth / BMP.Width)
                                            ElseIf aWidth <= 0 And aHeight > 0 Then
                                                aWidth = BMP.Width * (aHeight / BMP.Height)
                                            End If
                                            If isFixedHeight Or isFixedWidth Then
                                                If isFixedHeight And Not isFixedWidth Then
                                                    If aHeight > BMP.Height Then
                                                        aHeight = BMP.Height
                                                        aWidth = BMP.Width
                                                    End If
                                                End If
                                                If isFixedWidth And Not isFixedHeight Then
                                                    If aWidth > BMP.Width Then
                                                        aHeight = BMP.Height
                                                        aWidth = BMP.Width
                                                    End If
                                                End If
                                            End If
                                            If aWidth > 0 And aHeight > 0 Then
                                                If aWidth = BMP.Width And aHeight = BMP.Height And aQuality = -1 Then
                                                    'DO NOTHING, LEAVING THE STREAM INTACT
                                                Else
                                                    If aQuality <= 0 Then
                                                        aQuality = 100
                                                    End If
                                                    If Not fi.Source Is Nothing Then
                                                        If fi.AutoClose Then
                                                            fi.Source.Close()
                                                        End If
                                                        fi.Source = Nothing
                                                    End If
                                                    fi.Source = r2i.OWS.Framework.Utilities.Engine.Graphics.ResizeImage(BMP, DestinationTarget, aWidth, aHeight, aQuality, Debugger)
                                                End If
                                            End If
                                            fi.Width = aWidth
                                            fi.Height = aHeight
                                            If Not Debugger Is Nothing AndAlso Not fi.FileName Is Nothing Then
                                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, fi.FileName & " resized to " & fi.Width & "x" & fi.Height, True)
                                            End If
                                        Catch ex As Exception
                                            If Not Debugger Is Nothing Then
                                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Failed to resize image: " & ex.ToString(), True)
                                            End If
                                        Finally
                                            'VERSION: 2.0 Fixed Image Closing Behavuour
                                            If Not BMP Is Nothing Then
                                                BMP.Dispose()
                                            End If
                                            BMP = Nothing
                                        End Try
                                    Next
                                End If
                            Case "draw.text"
                                ImageSize = Caller.Engine.RenderString(sharedds, ImageSize, Caller.Engine.CapturedMessages, False, isPreRender:=False)
                                ImageSizeType = Caller.Engine.RenderString(sharedds, ImageSizeType, Caller.Engine.CapturedMessages, False, isPreRender:=False)
                                ImageColor = Caller.Engine.RenderString(sharedds, ImageColor, Caller.Engine.CapturedMessages, False, isPreRender:=False)
                                ImageBGColor = Caller.Engine.RenderString(sharedds, ImageBGColor, Caller.Engine.CapturedMessages, False, isPreRender:=False)
                                ImageFont = Caller.Engine.RenderString(sharedds, ImageFont, Caller.Engine.CapturedMessages, False, isPreRender:=False)
                                If Not ImageText Is Nothing Then
                                    ImageText = Caller.Engine.RenderString(sharedds, ImageText, Caller.Engine.CapturedMessages, False, isPreRender:=False)
                                End If
                                ImageX = Caller.Engine.RenderString(sharedds, ImageX, Caller.Engine.CapturedMessages, False, isPreRender:=False)
                                ImageY = Caller.Engine.RenderString(sharedds, ImageY, Caller.Engine.CapturedMessages, False, isPreRender:=False)
                                Try
                                    Dim BMP As System.Drawing.Bitmap = Nothing
                                    For Each fi As FileItem In SourceStreams
                                        Try
                                            Dim text As String = ImageText
                                            If ImageText Is Nothing Then
                                                text = fi.FileName
                                            Else
                                                If fi.Source.Length = 0 Then
                                                    BMP = Nothing
                                                Else
                                                    BMP = New System.Drawing.Bitmap(fi.Source)
                                                End If
                                            End If
                                            'If fi.Source.Length = 0 Then fi.SetSourceStream()

                                            'If Not fi.Source Is Nothing Then
                                            '    If fi.AutoClose Then
                                            '        fi.Source.Close()
                                            '    End If
                                            '    fi.Source = Nothing
                                            'End If

                                            If ImageSizeType Is Nothing Then
                                                ImageSizeType = ""
                                            End If
                                            If ImageSize Is Nothing Then
                                                ImageSize = ""
                                            End If

                                            Dim rFont As System.Drawing.Font = r2i.OWS.Framework.Utilities.Engine.Graphics.GetFont(ImageFont, ImageSize & ImageSizeType)
                                            'Dim rColor As System.Drawing.Brush = Drawing.Brushes.Black
                                            'Dim rbgColor As System.Drawing.Brush = Drawing.Brushes.White
                                            Dim rColor As System.Drawing.Color = Drawing.Color.Black
                                            Dim rbgColor As System.Drawing.Color = Drawing.Color.White
                                            If Not ImageColor Is Nothing AndAlso ImageColor.Length > 0 Then
                                                Try
                                                    rColor = System.Drawing.ColorTranslator.FromHtml(ImageColor)
                                                Catch ex As Exception

                                                End Try
                                            End If
                                            If Not ImageBGColor Is Nothing AndAlso ImageBGColor.Length > 0 Then
                                                Try
                                                    rbgColor = System.Drawing.ColorTranslator.FromHtml(ImageBGColor)
                                                Catch ex As Exception

                                                End Try
                                            End If

                                            If Not fi.Source Is Nothing Then
                                                If fi.AutoClose Then
                                                    fi.Source.Close()
                                                End If
                                                fi.Source = Nothing
                                            End If
                                            fi.Source = r2i.OWS.Framework.Utilities.Engine.Graphics.RenderString(BMP, ImageX, ImageY, rFont, rColor, rbgColor, text, DestinationTargetMimeType, ImageWarp)

                                            If Not Debugger Is Nothing AndAlso Not fi.FileName Is Nothing Then
                                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Generating Image with Text " & text, True)
                                            End If
                                        Catch ex As Exception
                                            If Not Debugger Is Nothing Then
                                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Failed to create image: " & ex.ToString(), True)
                                            End If
                                        Finally
                                            'VERSION: 2.0 Fixed Image Closing Behavuour
                                        End Try
                                    Next
                                Catch ex As Exception
                                    If Not Debugger Is Nothing Then
                                        r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Failed to resize image: " & ex.ToString(), True)
                                    End If
                                Finally

                                End Try
                        End Select
                    Case "XML"
                        Dim SourceXMLPath As String = Utility.GetDictionaryValue(parms, MessageActionsConstants.ACTIONFILE_XMLREADPATH_KEY)
                        Dim DestinationXMLPath As String = Utility.GetDictionaryValue(parms, MessageActionsConstants.ACTIONFILE_XMLWRITEPATH_KEY)

                        'TODO: Actually do something 
                        If Not Debugger Is Nothing Then
                            r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "XML Tranformation is currently not supported", True)
                        End If
                    Case "FILE"
                        Dim ActionType As String = Utility.GetDictionaryValue(parms, MessageActionsConstants.ACTIONFILE_FILETASK_KEY)

                        Select Case ActionType.ToUpper
                            Case "COPY"     'DETERMINE IF THE SOURCE IS A FOLDER - IF SO, WE NEED TO MIGRATE AN ENTIRE FOLDER TO THE NEW POSITION
                                If Not Debugger Is Nothing Then
                                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Attempting to copy the file", True)
                                End If
                                For Each fi As FileItem In SourceStreams
                                    If fi.LocalPath <> "" Then
                                        Try
                                            Dim sourcePath As String = fi.LocalPath
                                            Dim dio As New IO.DirectoryInfo(sourcePath) 'SOURCEPATH
                                            If dio.Exists Then
                                                If Not Debugger Is Nothing Then
                                                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Source is a Directory", False)
                                                End If
                                                'copy the directory to the new location

                                                If DestinationType.ToUpper() = "<FILE>" And Not DestinationTarget Is Nothing Then
                                                    fi.Abort = True
                                                    Dim tdio As New IO.DirectoryInfo(DestinationTarget)
                                                    If Not tdio.Exists AndAlso tdio.Parent.Exists Then
                                                        If Not Debugger Is Nothing Then
                                                            r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Create Destination Directory", False)
                                                        End If
                                                        'tdio.Parent.CreateSubdirectory(DestinationTarget)
                                                        Try
                                                            tdio.Create()
                                                        Catch ex As Exception
                                                            If Not Debugger Is Nothing Then
                                                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Failed to create destination directory: " & ex.ToString, True)
                                                            End If
                                                        End Try
                                                    End If
                                                    tdio = New IO.DirectoryInfo(DestinationTarget)
                                                    If tdio.Exists Then
                                                        If Not Debugger Is Nothing Then
                                                            r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Copying Files and Sub-Directories", False)
                                                        End If
                                                        RecursiveCopyFiles(Caller, sourcePath, DestinationTarget, True, True, Debugger)
                                                    Else
                                                        If Not Debugger Is Nothing Then
                                                            r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Cannot create destination directory", False)
                                                        End If
                                                    End If
                                                End If
                                            End If
                                        Catch ex As Exception
                                            If Not Debugger Is Nothing Then
                                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Cannot handle copy request: " & ex.ToString, True)
                                            End If
                                        End Try
                                    End If
                                Next
                            Case "MOVE"     'MOVING REQUIRES THAT WE DELETE THE SOURCE AFTER WE MOVE THE 
                                If Not Debugger Is Nothing Then
                                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Attempting to move the file", True)
                                End If
                                For Each fi As FileItem In SourceStreams
                                    If fi.LocalPath <> "" Then
                                        Try
                                            Dim sourcePath As String = fi.LocalPath
                                            'fi.Delete = True
                                            Dim dio As New IO.DirectoryInfo(sourcePath)
                                            If dio.Exists Then
                                                'copy the directory to the new location
                                                If DestinationType.ToUpper() = "<FILE>" And Not DestinationTarget Is Nothing Then
                                                    fi.Abort = True
                                                    dio.MoveTo(DestinationTarget) 'RecursiveCopyFiles(SourcePath, DestinationTarget, True, True, Debugger)
                                                    'End If
                                                    'fi.Delete = False
                                                End If
                                            Else
                                                'THIS IS A FILE?
                                                Dim fio As New IO.FileInfo(sourcePath)
                                                If fio.Exists Then
                                                    fi.Source.Close()
                                                    fi.Source = Nothing
                                                    fi.Abort = True
                                                    fio.MoveTo(DestinationTarget)
                                                End If
                                            End If
                                        Catch ex As Exception
                                            If Not Debugger Is Nothing Then
                                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Cannot handle move request: " & ex.ToString, True)
                                            End If
                                        End Try
                                    End If
                                Next
                            Case "DELETE"   'DELETE REQUIRES THAT WE DELETE THE SOURCE
                                If Not Debugger Is Nothing Then
                                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Attempting to Delete the file", True)
                                End If
                                For Each fi As FileItem In SourceStreams
                                    If fi.LocalPath <> "" Then
                                        Dim dio As New IO.DirectoryInfo(fi.LocalPath)
                                        If dio.Exists Then
                                            If Not fi.Source Is Nothing Then
                                                fi.Source.Close()
                                                fi.Source = Nothing
                                                fi.Abort = True
                                            End If
                                            If Not fi.LocalPath = Caller.Engine.Request.PhysicalApplicationPath Then
                                                If Not Debugger Is Nothing Then
                                                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Deleting Folder: " & fi.LocalPath, False)
                                                End If
                                                Directory_Delete(dio, True, True)
                                            Else
                                                If Not Debugger Is Nothing Then
                                                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Cannot Delete Root Folder: " & fi.LocalPath, False)
                                                End If
                                            End If
                                        Else
                                            'THIS IS A FILE?
                                            Try
                                                Dim fio As New IO.FileInfo(fi.LocalPath)
                                                If fio.Exists Then
                                                    If Not Debugger Is Nothing Then
                                                        r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Deleting File: " & fio.FullName, True)
                                                    End If
                                                    fi.Source.Close()
                                                    fi.Source = Nothing
                                                    fi.Abort = True
                                                    File_Delete(fio)
                                                End If
                                            Catch ex As Exception
                                                If Not Debugger Is Nothing Then
                                                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Cannot delete " & fi.VirtualPath & ": " & ex.ToString, True)
                                                End If
                                            End Try

                                        End If
                                    Else
                                        If Not Debugger Is Nothing Then
                                            r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Unable to delete", True)
                                        End If
                                    End If
                                Next
                            Case "EXTRACT"
                                Dim zunzip As Object = Nothing
                                Dim zentry As Object = Nothing

                                For Each fi As FileItem In SourceStreams
                                    Try
                                        If Not fi.Source Is Nothing Then
                                            If fi.Source.Length = 0 Then fi.SetSourceStream()
                                            fi.Source.Position = 0
                                            Select Case fi.Extension.ToUpper.Replace(".", "")
                                                Case "GZIP"
                                                    'SINGLE FILE
                                                    zunzip = New ICSharpCode.SharpZipLib.GZip.GZipInputStream(fi.Source)
                                                Case "BZIP2"
                                                    'SINGLE FILE
                                                    zunzip = New ICSharpCode.SharpZipLib.BZip2.BZip2InputStream(fi.Source)
                                                Case "TAR"
                                                    zunzip = New ICSharpCode.SharpZipLib.Tar.TarInputStream(fi.Source)
                                                    zentry = CType(zunzip, ICSharpCode.SharpZipLib.Tar.TarInputStream).GetNextEntry
                                                Case Else  'ZIP
                                                    zunzip = New ICSharpCode.SharpZipLib.Zip.ZipInputStream(fi.Source)
                                                    zentry = CType(zunzip, ICSharpCode.SharpZipLib.Zip.ZipInputStream).GetNextEntry
                                            End Select
                                            While Not zentry Is Nothing
                                                Select Case fi.Extension.ToUpper.Replace(".", "")
                                                    Case "GZIP"
                                                    Case "BZIP2"
                                                    Case "TAR"
                                                        'Dim tentry As ICSharpCode.SharpZipLib.Tar.TarEntry = CType(zentry, ICSharpCode.SharpZipLib.Tar.TarEntry)
                                                        'tentry.
                                                    Case "ZIP"
                                                        Try
                                                            Dim zoentry As ICSharpCode.SharpZipLib.Zip.ZipEntry = CType(zentry, ICSharpCode.SharpZipLib.Zip.ZipEntry)
                                                            Dim zipper As ICSharpCode.SharpZipLib.Zip.ZipInputStream = CType(zunzip, ICSharpCode.SharpZipLib.Zip.ZipInputStream)
                                                            If zoentry.IsDirectory = False Then

                                                                Dim strTarget As String = DestinationTarget & zoentry.Name
                                                                Try
                                                                    If Handle_File_Path(Caller, strTarget) Then

                                                                        Dim ms As New IO.FileStream(strTarget, IO.FileMode.Create)
                                                                        Try
                                                                            Dim intSize As Integer = 2048
                                                                            Dim arrData(2048) As Byte

                                                                            intSize = zipper.Read(arrData, 0, arrData.Length)
                                                                            While intSize > 0
                                                                                ms.Write(arrData, 0, intSize)
                                                                                intSize = zipper.Read(arrData, 0, arrData.Length)
                                                                            End While
                                                                        Catch ex As Exception
                                                                        End Try
                                                                        ms.Close()

                                                                        'DestinationTarget = Caller.Engine.Request.MapPath(strTarget)
                                                                    Else
                                                                    End If
                                                                Catch ex As Exception
                                                                    DestinationTarget = strTarget
                                                                End Try

                                                            End If
                                                            zentry = zipper.GetNextEntry
                                                        Catch exf As Exception
                                                            zentry = Nothing
                                                        End Try
                                                End Select
                                            End While
                                            If Not zunzip Is Nothing Then
                                                Try
                                                    Dim zipper As ICSharpCode.SharpZipLib.Zip.ZipInputStream = CType(zunzip, ICSharpCode.SharpZipLib.Zip.ZipInputStream)
                                                    zipper.Close()
                                                    zipper = Nothing
                                                    zunzip = Nothing
                                                Catch ex As Exception
                                                End Try
                                            End If
                                        End If
                                    Catch ex As Exception
                                        If Not Debugger Is Nothing Then
                                            r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Error Decompressing a File: " & ex.ToString(), True)
                                        End If
                                    End Try
                                Next
                            Case "COMPRESS"
                                Dim zzip As Object
                                Dim msZipped As New IO.MemoryStream
                                Dim objCrc32 As New ICSharpCode.SharpZipLib.Checksums.Crc32()
                                Dim sExtension As String

                                Select Case True
                                    Case DestinationTarget.ToUpper().EndsWith(".GZIP")
                                        'SINGLE FILE
                                        zzip = New ICSharpCode.SharpZipLib.GZip.GZipOutputStream(msZipped)
                                        sExtension = ".gzip"
                                    Case DestinationTarget.ToUpper().EndsWith(".BZIP2")
                                        'SINGLE FILE
                                        zzip = New ICSharpCode.SharpZipLib.BZip2.BZip2OutputStream(msZipped)
                                        sExtension = ".bzip2"
                                    Case DestinationTarget.ToUpper().EndsWith(".TAR")
                                        zzip = New ICSharpCode.SharpZipLib.Tar.TarOutputStream(msZipped)
                                        sExtension = ".tar"
                                    Case Else  'ZIP
                                        zzip = New ICSharpCode.SharpZipLib.Zip.ZipOutputStream(msZipped)
                                        CType(zzip, ICSharpCode.SharpZipLib.Zip.ZipOutputStream).SetLevel(6)
                                        sExtension = ".zip"
                                End Select
                                For Each fi As FileItem In SourceStreams
                                    Try
                                        If Not fi.Source Is Nothing Then
                                            If fi.Source.Length = 0 Then fi.SetSourceStream()
                                            fi.Source.Position = 0
                                            Select Case True
                                                Case DestinationTarget.ToUpper().EndsWith(".GZIP")
                                                Case DestinationTarget.ToUpper().EndsWith(".BZIP2")
                                                Case DestinationTarget.ToUpper().EndsWith(".TAR")
                                                    'Dim tentry As ICSharpCode.SharpZipLib.Tar.TarEntry = CType(zentry, ICSharpCode.SharpZipLib.Tar.TarEntry)
                                                    'tentry.
                                                Case Else
                                                    Try
                                                        Dim abyBuffer(fi.Source.Length - 1) As Byte

                                                        fi.Source.Read(abyBuffer, 0, abyBuffer.Length)
                                                        Dim localPath As String = fi.RelativeLocalPath
                                                        If localPath.StartsWith("\") OrElse localPath.StartsWith("/") Then
                                                            localPath = localPath.Remove(0, 1)
                                                        End If
                                                        Dim objZipEntry As New ICSharpCode.SharpZipLib.Zip.ZipEntry(localPath)

                                                        objZipEntry.DateTime = DateTime.Now
                                                        objZipEntry.Size = fi.Source.Length
                                                        fi.Source.Close()
                                                        objCrc32.Reset()
                                                        objCrc32.Update(abyBuffer)
                                                        objZipEntry.Crc = objCrc32.Value
                                                        CType(zzip, ICSharpCode.SharpZipLib.Zip.ZipOutputStream).PutNextEntry(objZipEntry)
                                                        CType(zzip, ICSharpCode.SharpZipLib.Zip.ZipOutputStream).Write(abyBuffer, 0, abyBuffer.Length)
                                                    Catch exf As Exception

                                                    End Try
                                            End Select
                                        End If
                                    Catch ex As Exception
                                        If Not Debugger Is Nothing Then
                                            r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Error Compressing a File: " & ex.ToString(), True)
                                        End If
                                    End Try
                                Next
                                CType(zzip, ICSharpCode.SharpZipLib.Zip.ZipOutputStream).Finish()

                                SourceStreams = New List(Of FileItem)
                                Dim fiZip As New FileItem(DestinationTarget, msZipped)
                                SourceStreams.Add(fiZip)
                        End Select
                    Case Else
                        'TODO: Implements exceptions unexpected parameter
                End Select
            Catch ex As Exception
                Debug.WriteLine(ex.ToString())
                If Not Debugger Is Nothing Then
                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Error During Transform: " & ex.ToString(), True)
                End If
            End Try
        End Sub
        Private Sub ApplyTransforms_SQL_To_CSV(ByRef Caller As Runtime, ByRef sharedds As System.Data.DataSet, ByVal act As MessageActionItem, ByRef Debugger As Framework.Debugger, ByRef DestinationTargetType As String, ByRef DestinationTarget As String, ByRef SourceStreams As List(Of FileItem), ByRef DestinationTargetMimeType As String, ByRef isDestinationProcess As Boolean, ByRef DestinationIncludeColumnName As Boolean, ByVal DestinationColumnMappings As Object, ByRef DestinationProcessName As String, ByVal Delimiter As String)
            If isDestinationProcess Then
                Dim p As New FileImport_ThreadedMessageActionProcess
                Dim targetms As New IO.MemoryStream
                'MessageActions.SetChildren(act.ChildActions, MessageActionItem.ActionStatusType.DontExecute, Debugger)

                p.SharedDS = sharedds
                p.ThreadAction = act
                p.Debugger = Nothing
                p.Source = CType(SourceStreams.Item(0).Data, DataSet).Tables(0)
                If DestinationTarget Is Nothing OrElse DestinationTarget.Length = 0 Then
                    DestinationTarget = "SQL"
                End If
                SourceStreams.Item(0) = New r2i.OWS.Actions.FileAction.FileItem(DestinationTarget, targetms)
                p.SourceStream = targetms
                p.DestinationIncludeColumnName = DestinationIncludeColumnName
                p.DestinationColumnMappings = DestinationColumnMappings
                p.DestinationTarget = DestinationTarget
                p.Delimiter = Delimiter
                p.RenderingEngine = Caller.Engine
                p.FilterField = Caller.FilterField
                p.FilterText = Caller.FilterText

                Dim d As New FileImport_ThreadedMessageActionProcess.FileImport_ThreadedMessageActionProcess_Address(AddressOf Handle_File_SQL_to_CSV)
                p.StartPosition = d
                Dim t As New Threading.Thread(AddressOf p.Start)
                p.ThreadObj = t
                ThreadedMessageHandler.StartProcess(p, DestinationProcessName, Caller.Engine.Session.SessionID, t)
                t.Start()
            Else
                Dim targetms As New IO.MemoryStream
                Handle_File_SQL_to_CSV(Caller, sharedds, act, Debugger, CType(SourceStreams.Item(0).Data, DataSet).Tables(0), targetms, DestinationIncludeColumnName, DestinationColumnMappings, DestinationTarget, Delimiter)
                Dim fi As FileItem = SourceStreams.Item(0)
                If Not fi.Source Is Nothing Then
                    fi.Source.Close()
                    fi.Source = Nothing
                End If
                If DestinationTarget Is Nothing OrElse DestinationTarget.Length = 0 Then
                    DestinationTarget = "SQL"
                End If
                SourceStreams.Item(0) = New r2i.OWS.Actions.FileAction.FileItem(DestinationTarget, targetms)
            End If
        End Sub
        Private Sub HandleStoreFileNames(ByRef fi As FileItem, ByRef DestinationTarget As String, ByRef DestinationTargetMimeType As String)
            If Not fi Is Nothing AndAlso Not DestinationTarget Is Nothing Then
                HandleReplaceFileAttributes(fi, DestinationTarget)
            End If
            If Not fi Is Nothing AndAlso Not DestinationTargetMimeType Is Nothing Then
                HandleReplaceFileAttributes(fi, DestinationTargetMimeType)
            End If
        End Sub
        Private Sub HandleReplaceFileAttributes(ByRef fi As FileItem, ByRef Value As String)
            Dim Attribute As String = ""

            Attribute = ""
            If Not fi Is Nothing AndAlso Not fi.FileName Is Nothing Then
                Attribute = fi.FileName
            End If
            If Not Value Is Nothing Then
                Dim startIndex As Integer = -1
                startIndex = Value.ToUpper.IndexOf("[FILENAME,FILE]")
                If startIndex > -1 Then
                    Value = Value.Insert(startIndex + "[FILENAME,FILE]".Length, Attribute)
                    Value = Value.Remove(startIndex, "[FILENAME,FILE]".Length)
                End If
            End If

            Attribute = ""
            If Not fi Is Nothing AndAlso Not fi.ContentType Is Nothing Then
                Attribute = fi.ContentType
            End If
            If Not Value Is Nothing Then
                Dim startIndex As Integer = -1
                startIndex = Value.ToUpper.IndexOf("[CONTENTTYPE,FILE]")
                If startIndex > -1 Then
                    Value = Value.Insert(startIndex + "[CONTENTTYPE,FILE]".Length, Attribute)
                    Value = Value.Remove(startIndex, "[CONTENTTYPE,FILE]".Length)
                End If
            End If

            Attribute = ""
            If Not fi Is Nothing AndAlso Not fi.Extension Is Nothing Then
                Attribute = fi.Extension
            End If
            If Not Value Is Nothing Then
                Dim startIndex As Integer = -1
                startIndex = Value.ToUpper.IndexOf("[EXTENSION,FILE]")
                If startIndex > -1 Then
                    Value = Value.Insert(startIndex + "[EXTENSION,FILE]".Length, Attribute)
                    Value = Value.Remove(startIndex, "[EXTENSION,FILE]".Length)
                End If
            End If
        End Sub
        Private Sub Store(ByRef Caller As Runtime, ByRef sharedds As System.Data.DataSet, ByVal act As MessageActionItem, ByRef Debugger As Framework.Debugger, ByRef DestinationTargetType As String, ByRef DestinationTarget As String, ByRef SourceStreams As List(Of FileItem), ByRef DestinationTargetMimeType As String, ByRef isDestinationProcess As Boolean, ByRef DestinationIncludeColumnName As Boolean, ByVal DestinationColumnMappings As Object, ByRef DestinationProcessName As String)
            Dim rslt As Runtime.ExecutableResult
            '' POINTER MAY BE AT THE END OF THE FILE, RESET POINTERS
            For Each fi As FileItem In SourceStreams
                If Not fi.Abort Then
                    If Not fi.Source Is Nothing Then
                        Try
                            If Not fi Is Nothing AndAlso Not fi.Source Is Nothing AndAlso fi.Source.CanRead Then
                                fi.Source.Position = 0
                            End If
                        Catch ex As Exception

                        End Try
                    End If
                End If
            Next
            '' SOURCE WAS NOT SQL DATA
            Select Case DestinationTargetType.Replace("&lt;", "<").Replace("&gt;", ">").ToUpper
                Case "<FILE>"
                    Try
                        If Not Debugger Is Nothing Then
                            r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Saving to Path: " & DestinationTarget, False)
                        End If


                        Dim fio As IO.FileInfo = Nothing
                        If Not DestinationTarget Is Nothing AndAlso DestinationTarget.Length > 0 Then
                            fio = New IO.FileInfo(DestinationTarget)
                        End If
                        If Not fio Is Nothing AndAlso fio.Directory.Exists Then
                            For Each fi As FileItem In SourceStreams
                                If Not fi.Abort Then
                                    Dim dStream As IO.FileStream = Nothing

                                    Try
                                        If Not fio Is Nothing AndAlso (fio.Attributes <> IO.FileAttributes.Directory) Then
                                            If fio.Exists Then
                                                dStream = fio.Open(IO.FileMode.Truncate, IO.FileAccess.Write)
                                            Else
                                                dStream = fio.Open(IO.FileMode.OpenOrCreate, IO.FileAccess.Write)
                                            End If
                                            Utility.StreamTransfer(fi.Source, CType(dStream, System.IO.Stream))
                                        End If
                                    Catch ex As Exception
                                        If Not Debugger Is Nothing Then
                                            r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "ERROR saving to " & DestinationTarget & ": " & ex.ToString(), False)
                                        End If
                                    Finally
                                        If Not dStream Is Nothing AndAlso (dStream.CanRead OrElse dStream.CanWrite OrElse dStream.CanSeek) Then
                                            dStream.Close()
                                        End If
                                        dStream = Nothing
                                    End Try
                                End If
                            Next
                        Else
                            If Not Debugger Is Nothing Then
                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Directory does not exist for path: " & DestinationTarget, False)
                            End If
                        End If
                    Catch ex As Exception
                        If Not Debugger Is Nothing Then
                            r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Unabled to save target file path: " & ex.ToString(), True)
                        End If
                    End Try
                Case "<RESPONSE>"
                    Try
                        ' Can only attach one file at a time
                        Dim fi As FileItem = SourceStreams.Item(0)
                        HandleStoreFileNames(fi, DestinationTarget, DestinationTargetMimeType)
                        Dim noattachment As Boolean = False
                        If DestinationTargetMimeType.ToUpper.IndexOf("-NOATTACH") >= 0 Then
                            noattachment = True
                        End If
                        Caller.Engine.ClearResponse()
                        Handle_File_Set_Headers(Caller.Engine.Context)

                        'Caller.Engine.Response.AddHeader("Content-Type", DestinationTargetMimeType.ToLower.Replace("-noattach", ""))
                        If Not DestinationTargetMimeType Is Nothing AndAlso DestinationTargetMimeType.Length > 0 Then
                            Caller.Engine.Response.ContentType = DestinationTargetMimeType.ToLower.Replace("-noattach", "")
                        End If
                        If Not DestinationTarget Is Nothing AndAlso DestinationTarget.Length > 0 Then
                            If Not noattachment Then
                                Caller.Engine.Response.AddHeader("Content-Disposition", "attachment; filename=" & DestinationTarget & ";")
                            Else
                                Caller.Engine.Response.AddHeader("Content-Disposition", "inline; filename=" & DestinationTarget & ";")
                            End If
                        End If
                        'VERSION: 1.9.8.6 - Added File Action "Download" support when Chunk Encoding is employed for Safari
                        Caller.Engine.Response.AddHeader("Content-Length", fi.Source.Length)

                        'Caller.Engine.Response.AddHeader("Cache-Control", "private")

                        Utility.StreamTransfer(fi.Source, Caller.Engine.Response.OutputStream)

                        Handle_File_Complete_Transfer(Caller.Engine.Context)
                    Catch ex As Exception
                        If Not Debugger Is Nothing Then
                            r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Failed to add file to response: " & ex.ToString, True)
                        End If
                    End Try
                Case "<EMAILATTACHMENT>"
                    Try
                        Dim noattachment As Boolean = False
                        If DestinationTargetMimeType.ToUpper.IndexOf("-NOATTACH") >= 0 Then
                            noattachment = True
                        End If

                        For Each fi As FileItem In SourceStreams
                            'Dim ms As New IO.MemoryStream
                            'Utility.StreamTransfer(fi.Source, ms)
                            Dim thisFileName As String = fi.FileName
                            Dim thisFileType As String = fi.ContentType
                            If Not DestinationTarget Is Nothing AndAlso DestinationTarget.Length > 0 Then
                                thisFileName = DestinationTarget
                            End If
                            If Not DestinationTargetMimeType Is Nothing AndAlso DestinationTargetMimeType.Length > 0 Then
                                thisFileType = DestinationTargetMimeType.ToLower.Replace("-noattach", "")
                            End If

                            Dim ma As New Attachment(fi.Source, thisFileName, thisFileType)
                            Caller.CurrentMailMessage.Attachments.Add(ma)
                            fi.AutoClose = False
                        Next
                    Catch ex As Exception
                        If Not Debugger Is Nothing Then
                            r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Failed to add file to email attachment: " & ex.ToString, True)
                        End If
                    End Try
                Case "<SESSION>"
                    Try
                        For Each fi As FileItem In SourceStreams
                            Dim ms As New IO.MemoryStream
                            Utility.StreamTransfer(fi.Source, CType(ms, System.IO.Stream))
                            Caller.Engine.Session.Item(DestinationTarget) = ms.ToArray
                            ms.Close()
                            ms = Nothing
                        Next
                    Catch ex As Exception
                        If Not Debugger Is Nothing Then
                            r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Failed to add file to session: " & ex.ToString, True)
                        End If
                    End Try
                Case "<COOKIE>"
                    Try
                        For Each fi As FileItem In SourceStreams
                            Dim ms As New IO.MemoryStream
                            Utility.StreamTransfer(fi.Source, CType(ms, System.IO.Stream))
                            Dim ck As New HttpCookie(DestinationTarget, System.Text.UTF8Encoding.UTF8.GetString(ms.ToArray()))
                            Try
                                Caller.Engine.Response.Cookies.Set(ck)
                            Catch ex As Exception
                                Caller.Engine.Response.Cookies.Add(ck)
                            End Try
                            ms.Close()
                            ms = Nothing
                        Next
                    Catch ex As Exception
                        If Not Debugger Is Nothing Then
                            r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Failed to add file to cookie: " & ex.ToString, True)
                        End If
                    End Try
                Case "<VIEWSTATE>"
                    Try
                        For Each fi As FileItem In SourceStreams
                            Try
                                Dim ms As New IO.MemoryStream
                                If fi.Source.Length = 0 Then fi.SetSourceStream()
                                Utility.StreamTransfer(fi.Source, CType(ms, System.IO.Stream))
                                Caller.Engine.ViewState.Add(DestinationTarget, ms.ToArray())
                                ms.Close()
                                ms = Nothing
                            Catch ex As Exception
                                If Not Debugger Is Nothing Then
                                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Error assigning ViewState to a File: " & ex.ToString(), True)
                                End If
                            End Try
                        Next
                    Catch ex As Exception
                        If Not Debugger Is Nothing Then
                            r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Failed to add file to viewstate: " & ex.ToString, True)
                        End If
                    End Try
                Case "<CONTEXT>"
                    Try
                        For Each fi As FileItem In SourceStreams
                            Try
                                Dim ms As New IO.MemoryStream
                                If fi.Source.Length = 0 Then fi.SetSourceStream()
                                Utility.StreamTransfer(fi.Source, CType(ms, System.IO.Stream))
                                'Caller.Engine.ViewState.Add(DestinationTarget, ms.ToArray())
                                ms.Position = 0
                                Caller.Engine.Context.Items.Add(DestinationTarget, ms)
                                'DONT CLOSE THE STREAM!!!
                            Catch ex As Exception
                                If Not Debugger Is Nothing Then
                                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Error assigning Context to a File: " & ex.ToString(), True)
                                End If
                            End Try
                        Next
                    Catch ex As Exception
                        If Not Debugger Is Nothing Then
                            r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Failed to add file to viewstate: " & ex.ToString, True)
                        End If
                    End Try
                Case "<ACTION>"
                    Try
                        For Each fi As FileItem In SourceStreams
                            Try
                                Dim ms As New IO.MemoryStream
                                If fi.Source.Length = 0 Then fi.SetSourceStream()
                                Utility.StreamTransfer(fi.Source, CType(ms, System.IO.Stream))

                                If Not ms Is Nothing AndAlso ms.Length > 0 Then
                                    If DestinationTarget.ToUpper.EndsWith(".BINARY") Then
                                        Caller.Engine.ActionVariable(DestinationTarget) = ms.ToArray
                                        DestinationTarget = DestinationTarget.Remove(DestinationTarget.Length - 7)
                                    Else
                                        Caller.Engine.ActionVariable(DestinationTarget) = System.Text.UTF8Encoding.UTF8.GetString(ms.ToArray)
                                    End If
                                    Caller.Engine.ActionVariable(DestinationTarget & ".Path") = IO.Path.GetDirectoryName(fi.LocalPath)
                                    Caller.Engine.ActionVariable(DestinationTarget & ".VirtualPath") = IO.Path.GetDirectoryName(fi.VirtualPath)
                                    Caller.Engine.ActionVariable(DestinationTarget & ".RelativePath") = IO.Path.GetDirectoryName(fi.RelativePath)
                                    Caller.Engine.ActionVariable(DestinationTarget & ".Name") = fi.FileName
                                    Caller.Engine.ActionVariable(DestinationTarget & ".NameOnly") = IO.Path.GetFileNameWithoutExtension(fi.FileName)
                                    Caller.Engine.ActionVariable(DestinationTarget & ".Extension") = fi.Extension
                                    Caller.Engine.ActionVariable(DestinationTarget & ".Type") = fi.ContentType
                                    Caller.Engine.ActionVariable(DestinationTarget & ".Length") = ms.Length
                                    Caller.Engine.ActionVariable(DestinationTarget & ".Size") = ms.Length
                                    Caller.Engine.ActionVariable(DestinationTarget & ".SourceHex") = Utility.GetHexString(ms.ToArray())
                                    Caller.Engine.ActionVariable(DestinationTarget & ".Width") = fi.Width
                                    Caller.Engine.ActionVariable(DestinationTarget & ".Height") = fi.Height

                                    'HANDLE THE CHILD ACTIONS
                                    Try
                                        'Caller.SetChildren(act.ChildActions, MessageActionItem.ActionStatusType.DoExecute, Debugger)
                                        'Caller.ProcessChildActions(act.ChildActions, Debugger, act.Level + 1, sharedds)
                                        rslt = Caller.Execute(act.ChildActions, Debugger, sharedds)
                                    Catch ex As Exception
                                        Caller.Engine.ActionVariable(DestinationTarget) = Nothing
                                        Caller.Engine.ActionVariable(DestinationTarget & ".Name") = Nothing
                                        Caller.Engine.ActionVariable(DestinationTarget & ".NameOnly") = Nothing
                                        Caller.Engine.ActionVariable(DestinationTarget & ".Path") = Nothing
                                        Caller.Engine.ActionVariable(DestinationTarget & ".Extension") = Nothing
                                        Caller.Engine.ActionVariable(DestinationTarget & ".Type") = Nothing
                                        Caller.Engine.ActionVariable(DestinationTarget & ".Length") = Nothing
                                        Caller.Engine.ActionVariable(DestinationTarget & ".Size") = Nothing
                                        Caller.Engine.ActionVariable(DestinationTarget & ".ERROR") = ex.ToString()
                                    End Try
                                    ms.Close()
                                End If
                                ms = Nothing
                            Catch ex As Exception
                                If Not Debugger Is Nothing Then
                                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Error Assigning an Action to a File: " & ex.ToString(), True)
                                End If
                            End Try
                        Next
                    Catch ex As Exception
                        If Not Debugger Is Nothing Then
                            r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Failed to add file to action: " & ex.ToString, True)
                        End If
                    End Try
                Case "<SQL>"
                    Handle_File_Source_To_SQL(Caller, sharedds, act, Debugger, DestinationTargetType, DestinationTarget, SourceStreams, DestinationTargetMimeType, isDestinationProcess, DestinationIncludeColumnName, DestinationColumnMappings, DestinationProcessName)
            End Select
        End Sub
        Private Sub Handle_File_Source_To_SQL(ByRef Caller As Runtime, ByRef sharedds As System.Data.DataSet, ByVal act As MessageActionItem, ByRef Debugger As Framework.Debugger, ByRef DestinationTargetType As String, ByRef DestinationTarget As String, ByRef SourceStreams As List(Of FileItem), ByRef DestinationTargetMimeType As String, ByRef isDestinationProcess As Boolean, ByRef DestinationIncludeColumnName As Boolean, ByVal DestinationColumnMappings As Object, ByRef DestinationProcessName As String)
            If Not SourceStreams Is Nothing AndAlso SourceStreams.Count > 0 AndAlso SourceStreams.Item(0).Data Is Nothing Then
                For Each fi As FileItem In SourceStreams
                    'IMPORTING FROM CSV TO SQL
                    If isDestinationProcess Then
                        Dim p As New FileImport_ThreadedMessageActionProcess
                        'Caller.SetChildren(act.ChildActions, MessageActionItem.ActionStatusType.DontExecute, Debugger)

                        p.SharedDS = sharedds
                        p.ThreadAction = act
                        p.Debugger = Nothing
                        p.SourceStream = fi.Source
                        p.DestinationIncludeColumnName = DestinationIncludeColumnName
                        p.DestinationColumnMappings = DestinationColumnMappings
                        p.DestinationTarget = DestinationTarget
                        p.RenderingEngine = Caller.Engine
                        p.FilterField = Caller.FilterField
                        p.FilterText = Caller.FilterText

                        Dim d As New FileImport_ThreadedMessageActionProcess.FileImport_ThreadedMessageActionProcess_Address(AddressOf Handle_File_CSV_to_SQL)
                        p.StartPosition = d
                        Dim t As New Threading.Thread(AddressOf p.Start)
                        p.ThreadObj = t
                        ThreadedMessageHandler.StartProcess(p, DestinationProcessName, Caller.Engine.Session.SessionID, t)
                        t.Start()
                    Else
                        Handle_File_CSV_to_SQL(Caller, sharedds, act, Debugger, DestinationProcessName, fi.Source, DestinationIncludeColumnName, DestinationColumnMappings, DestinationTarget)
                    End If
                Next
            ElseIf Not SourceStreams Is Nothing AndAlso SourceStreams.Count > 0 AndAlso Not SourceStreams.Item(0).Data Is Nothing Then
                'TRANSPORTING FROM SQL TO SQL
                If isDestinationProcess Then
                    Dim p As New FileImport_ThreadedMessageActionProcess
                    Dim targetms As New IO.MemoryStream
                    'Caller.SetChildren(act.ChildActions, MessageActionItem.ActionStatusType.DontExecute, Debugger)

                    p.SharedDS = sharedds
                    p.ThreadAction = act
                    p.Debugger = Nothing
                    p.Source = CType(SourceStreams.Item(0).Data, DataSet).Tables(0)
                    p.SourceStream = targetms
                    p.DestinationIncludeColumnName = DestinationIncludeColumnName
                    p.DestinationColumnMappings = DestinationColumnMappings
                    p.DestinationTarget = DestinationTarget
                    p.RenderingEngine = Caller.Engine
                    p.FilterField = Caller.FilterField
                    p.FilterText = Caller.FilterText

                    Dim d As New FileImport_ThreadedMessageActionProcess.FileImport_ThreadedMessageActionProcess_Address(AddressOf Handle_File_SQL_to_CSV)
                    p.StartPosition = d
                    Dim t As New Threading.Thread(AddressOf p.Start)
                    p.ThreadObj = t
                    ThreadedMessageHandler.StartProcess(p, DestinationProcessName, Caller.Engine.Session.SessionID, t)
                    t.Start()
                Else
                    Handle_File_SQL_to_SQL(Caller, sharedds, act, Debugger, CType(SourceStreams.Item(0).Data, DataSet).Tables(0), DestinationIncludeColumnName, DestinationColumnMappings, DestinationTarget)
                End If
            End If
        End Sub
        Public Overrides Function Handle_Action(ByRef Caller As RuntimeBase, ByRef sharedds As System.Data.DataSet, ByRef act As MessageActionItem, ByRef previous As Runtime.ActionExecutionResult, ByRef Debugger As Framework.Debugger) As Runtime.ExecutableResult
            Try
                If Not act.Parameters Is Nothing Then
                    'Dim splitter As New Utility.SmartSplitter
                    'Dim i As Integer = 0
                    'splitter.Split(act.ActionInformation)
                    Dim parms As SerializableDictionary(Of String, Object) = act.Parameters
                    'Dim SourcePath As String = ""
                    Dim SourceType As String = Utility.GetDictionaryValue(parms, MessageActionsConstants.ACTIONFILE_SOURCETYPE_KEY)
                    Dim SourceStreams As List(Of FileItem) = Me.BuildSourceStreams(Caller, sharedds, act, Debugger)
                    'Dim SourceTable As DataTable
                    'Dim SourceFileName As String = ""
                    'Dim SourceExtension As String = ""
                    'Dim DeleteSource As Boolean = False
                    'Dim UnzipSource As Boolean = False
                    'Dim ZipSource As Boolean = False
                    'Dim SourceContentType As String = ""
                    Dim DestinationType As String = Utility.GetDictionaryValue(parms, MessageActionsConstants.ACTIONFILE_DESTINATIONTYPE_KEY)
                    Dim DestinationAction As String = ""
                    Dim DestinationTargetType As String = ""
                    Dim DestinationTargetMimeType As String = ""
                    Dim DestinationTarget As String = ""
                    Dim DestinationIncludeColumnName As Boolean = False
                    Dim DestinationColumnMappings As MessageAction_File_ColumnMappings = Nothing
                    Dim DestinationStream As IO.Stream = Nothing

                    'WE NOW HAVE THE FILE INFORMATION LOADED - IDENTIFY THE DESTINATION                   
                    Dim DestinationProcessName As String = Nothing
                    Dim isDestinationProcess As Boolean
                    Select Case DestinationType.ToUpper
                        Case "PATH"
                            DestinationTargetType = "<File>"
                            DestinationTarget = Utility.GetDictionaryValue(parms, MessageActionsConstants.ACTIONFILE_DESTINATION_KEY)
                            DestinationTarget = Caller.Engine.RenderString(sharedds, DestinationTarget, Caller.Engine.CapturedMessages, False, isPreRender:=False)
                            Try
                                If Handle_File_Path(Caller, DestinationTarget) Then
                                    If DestinationTarget.StartsWith("\\") OrElse DestinationTarget.StartsWith("//") Then
                                    Else
                                        DestinationTarget = Caller.Engine.Request.MapPath(DestinationTarget)
                                    End If
                                Else
                                End If
                            Catch ex As Exception
                                If Not Debugger Is Nothing Then
                                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Failed to add file to Path: " & ex.ToString, True)
                                End If
                            End Try
                        Case "VARIABLE"
                            DestinationTargetType = Utility.GetDictionaryValue(parms, MessageActionsConstants.ACTIONFILE_DESTINATIONVARIABLETYPE_KEY)
                            DestinationTargetType = Caller.Engine.RenderString(sharedds, DestinationTargetType, Caller.Engine.CapturedMessages, False, isPreRender:=False)
                            DestinationTarget = Utility.GetDictionaryValue(parms, MessageActionsConstants.ACTIONFILE_DESTINATION_KEY)
                            DestinationTarget = Caller.Engine.RenderString(sharedds, DestinationTarget, Caller.Engine.CapturedMessages, False, isPreRender:=False)
                        Case "RESPONSE"
                            DestinationTargetType = "<Response>"
                            DestinationTargetMimeType = Utility.GetDictionaryValue(parms, MessageActionsConstants.ACTIONFILE_DESTINATIONRESPONSETYPE_KEY)
                            DestinationTargetMimeType = Caller.Engine.RenderString(sharedds, DestinationTargetMimeType, Caller.Engine.CapturedMessages, False, isPreRender:=False)
                            DestinationTarget = Utility.GetDictionaryValue(parms, MessageActionsConstants.ACTIONFILE_DESTINATION_KEY)
                            DestinationTarget = Caller.Engine.RenderString(sharedds, DestinationTarget, Caller.Engine.CapturedMessages, False, isPreRender:=False)
                        Case "EMAILATTACHMENT"
                            If Not Caller.CurrentMailMessage Is Nothing Then
                                DestinationTargetType = "<EmailAttachment>"
                                DestinationTargetMimeType = Utility.GetDictionaryValue(parms, MessageActionsConstants.ACTIONFILE_DESTINATIONRESPONSETYPE_KEY)
                                DestinationTargetMimeType = Caller.Engine.RenderString(sharedds, DestinationTargetMimeType, Caller.Engine.CapturedMessages, False, isPreRender:=False)
                                DestinationTarget = Utility.GetDictionaryValue(parms, MessageActionsConstants.ACTIONFILE_DESTINATION_KEY)
                                DestinationTarget = Caller.Engine.RenderString(sharedds, DestinationTarget, Caller.Engine.CapturedMessages, False, isPreRender:=False)
                            Else
                                Throw New Exception("You cannot attach an email message unless the action appears as a child of a Mail action.")
                            End If
                        Case "SQL"
                            DestinationTargetType = "<SQL>"
                            DestinationIncludeColumnName = Utility.CNullBool(Utility.GetDictionaryValue(parms, MessageActionsConstants.ACTIONFILE_SQLFIRSTROW_KEY), False)
                            Dim mappings As Object = Utility.GetDictionaryObject(parms, MessageActionsConstants.ACTIONFILE_COLUMNMAPPING_KEY)
                            If TypeOf mappings Is Newtonsoft.Json.JavaScriptArray Then
                                DestinationColumnMappings = New MessageAction_File_ColumnMappings(CType(mappings, Newtonsoft.Json.JavaScriptArray))
                            Else
                                DestinationColumnMappings = New MessageAction_File_ColumnMappings(mappings.ToString)
                            End If


                            DestinationTarget = Utility.GetDictionaryValue(parms, MessageActionsConstants.ACTIONFILE_DESTINATIONQUERY_KEY)
                            isDestinationProcess = Utility.CNullBool(Utility.GetDictionaryValue(parms, MessageActionsConstants.ACTIONFILE_RUNASPROCESS_KEY), False)

                            DestinationProcessName = Utility.GetDictionaryValue(parms, MessageActionsConstants.ACTIONFILE_PROCESSNAME_KEY)
                            DestinationProcessName = Caller.Engine.RenderString(sharedds, DestinationProcessName, Caller.Engine.CapturedMessages, False, isPreRender:=False)
                        Case Else
                    End Select

                    '' APPLY TRANSFORMED SOURCES TO THE DESTINATION TARGETS
                    If Not SourceStreams Is Nothing AndAlso SourceStreams.Count > 0 Then
                        If SourceStreams.Item(0).Data Is Nothing Then
                            Try
                                Me.ApplyTransforms(Caller, sharedds, act, SourceStreams, DestinationTarget, DestinationTargetType, DestinationTargetMimeType, Debugger)
                            Catch ex As Exception
                                If Not Debugger Is Nothing Then
                                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Unabled to complete transformation: " & ex.ToString(), True)
                                End If
                            End Try
                        Else
                            'TRANSFORM IS CURRENTLY SQL TO CSV
                            DestinationIncludeColumnName = Utility.CNullBool(Utility.GetDictionaryValue(parms, MessageActionsConstants.ACTIONFILE_SQLFIRSTROW_KEY), False)
                            Dim mappings As Object = Utility.GetDictionaryObject(parms, MessageActionsConstants.ACTIONFILE_COLUMNMAPPING_KEY)
                            If TypeOf mappings Is Newtonsoft.Json.JavaScriptArray Then
                                DestinationColumnMappings = New MessageAction_File_ColumnMappings(CType(mappings, Newtonsoft.Json.JavaScriptArray))
                            Else
                                DestinationColumnMappings = New MessageAction_File_ColumnMappings(mappings.ToString)
                            End If

                            'DestinationTarget = Utility.GetDictionaryValue(parms, MessageActionsConstants.ACTIONFILE_DESTINATIONQUERY_KEY)
                            isDestinationProcess = Utility.CNullBool(Utility.GetDictionaryValue(parms, MessageActionsConstants.ACTIONFILE_RUNASPROCESS_KEY), False)

                            DestinationProcessName = Utility.GetDictionaryValue(parms, MessageActionsConstants.ACTIONFILE_PROCESSNAME_KEY)
                            DestinationProcessName = Caller.Engine.RenderString(sharedds, DestinationProcessName, Caller.Engine.CapturedMessages, False, isPreRender:=False)
                            If DestinationTargetType <> "<SQL>" Then
                                Dim TransformationTypeTask As String = ""
                                Try
                                    TransformationTypeTask = Utility.GetDictionaryValue(parms, MessageActionsConstants.ACTIONFILE_FILETASK_KEY)
                                Catch ex As Exception
                                    TransformationTypeTask = ""
                                End Try
                                Dim delimiter As String = ","
                                Try
                                    Select Case TransformationTypeTask.ToUpper
                                        Case "TAB"
                                            delimiter = vbTab
                                        Case Else
                                            delimiter = ","

                                    End Select
                                Catch ex As Exception
                                End Try

                                Me.ApplyTransforms_SQL_To_CSV(Caller, sharedds, act, Debugger, DestinationTargetType, DestinationTarget, SourceStreams, DestinationTargetMimeType, isDestinationProcess, DestinationIncludeColumnName, DestinationColumnMappings, DestinationProcessName, delimiter)
                            End If
                        End If

                        Me.Store(Caller, sharedds, act, Debugger, DestinationTargetType, DestinationTarget, SourceStreams, DestinationTargetMimeType, isDestinationProcess, DestinationIncludeColumnName, DestinationColumnMappings, DestinationProcessName)
                    End If





                    For Each fi As FileItem In SourceStreams
                        Try
                            If fi.AutoClose Then
                                Try
                                    If Not fi Is Nothing AndAlso Not fi.Source Is Nothing Then
                                        If fi.Source.CanRead OrElse fi.Source.CanSeek OrElse fi.Source.CanWrite Then
                                            fi.Source.Close()
                                        End If
                                    End If
                                Catch
                                End Try
                                fi.Source = Nothing
                            End If
                        Catch ex As Exception
                            If Not Debugger Is Nothing Then
                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Unabled to complete file action: " & ex.ToString(), True)
                            End If
                        End Try
                    Next
                    '' REG - 4/23/2007
                    '' If SourcePath is a folder, delete it and all child elements
                    'If DeleteSource Then
                    '    If IO.Directory.Exists(SourcePath) Then
                    '        If Not Debugger Is Nothing Then
                    '            r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Deleting Folder: " & SourcePath, False)
                    '        End If
                    '        Directory_Delete(New IO.DirectoryInfo(SourcePath), True)
                    '    ElseIf IO.File.Exists(SourcePath) Then
                    '        If Not Debugger Is Nothing Then
                    '            r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Deleting File: " & SourcePath, False)
                    '        End If
                    '        File_Delete(New IO.FileInfo(SourcePath))
                    '    End If
                    'End If

                End If
            Catch ex As Exception
                If Not Debugger Is Nothing Then
                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Unabled to handle file action: " & ex.ToString(), True)
                End If
            End Try
            Return Nothing
        End Function

        Private Function Handle_File_Path(ByRef Caller As Runtime, ByVal Path As String) As Boolean
            'MAP THE PATH
            If Not Path Is Nothing AndAlso Path.Length > 0 Then
                Try

                    Dim fio As New IO.FileInfo(Path)
                    If Not fio.Exists AndAlso (Not IO.Path.IsPathRooted(Path) OrElse Path.Length < 2 OrElse (Path.StartsWith("/") OrElse Path.StartsWith("\") AndAlso (Not Path.StartsWith("//") AndAlso Not Path.StartsWith("\\")))) Then
                        Path = Caller.Engine.Request.MapPath(Path)
                    End If
                Catch ex As Exception
                End Try

                Try
                    Dim tio As New IO.FileInfo(Path)
                    Dim dio As IO.DirectoryInfo = tio.Directory
                    'Dim dstack As New Stack
                    'ROMAIN: Generic replacement - 08/20/2007
                    Dim dstack As New Stack(Of String)
                    Dim IX As Integer = 0
                    While Not dio.Exists And IX < 10000
                        dstack.Push(dio.Name)
                        If dio.Parent Is Nothing Then
                            Throw New Exception("Unable to build directory path")
                        Else
                            dio = dio.Parent
                        End If
                        IX += 1
                    End While
                    While dio.Exists And dstack.Count > 0 And IX < 10000
                        Dim foldername As String = dstack.Pop
                        dio = dio.CreateSubdirectory(foldername)
                        IX += 1
                    End While
                    Return True
                Catch ex As Exception
                    Return False
                End Try
                Return False
            End If
        End Function
        Private Sub Handle_File_Set_Headers(ByRef Context As Web.HttpContext)

            'Context.Response.ClearHeaders()
            'Context.Response.Clear()
            'ADDED TO SKIP COMPRESSION
            If Not Context.Items.Contains("httpcompress.attemptedinstall") Then
                Context.Items.Add("httpcompress.attemptedinstall", "true")
            End If

            'VERSION: 1.9.7 - Changed Pragma and Cache-Control for Compression and PDF issues.
            Context.Response.Cache.SetCacheability(Web.HttpCacheability.Public)
            Context.Response.AddHeader("Cache-Control", "max-age=0")
            Context.Response.CacheControl = "Public"
            Context.Response.AddHeader("Pragma", "public")
        End Sub
        Private Sub Handle_File_Complete_Transfer(ByRef Context As Web.HttpContext)
            Try
                Context.Response.Flush()
                'Context.Response.Close()
            Catch exh As Threading.ThreadAbortException
                'Do Nothing - This was anticipated
            Catch ex As Exception
                'Do Nothing - This was anticipated
            End Try
            Try
                Context.Response.End()
            Catch exh As Threading.ThreadAbortException
                'Do Nothing - This was anticipated
            Catch ex As Exception
                'Do Nothing - This was anticipated
            End Try
        End Sub
        Private Sub RecursiveCopyFiles(ByRef Caller As Runtime, _
                ByVal sourceDir As String, _
                ByVal destDir As String, _
                ByVal fRecursive As Boolean, ByVal overWrite As Boolean, ByRef Debugger As Debugger)

            Dim i As Integer
            Dim posSep As Integer
            Dim sDir As String = Nothing
            Dim aDirs() As String
            Dim sFile As String = Nothing
            Dim aFiles() As String

            sourceDir = sourceDir.Replace(System.IO.Path.AltDirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar)
            destDir = destDir.Replace(System.IO.Path.AltDirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar)

            ' Add trailing separators to the supplied paths if they don't exist.
            If Not sourceDir.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()) Then
                sourceDir &= System.IO.Path.DirectorySeparatorChar
            End If

            If Not destDir.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()) Then
                destDir &= System.IO.Path.DirectorySeparatorChar
            End If

            ' Recursive switch to continue drilling down into dir structure.
            If fRecursive Then

                ' Get a list of directories from the current parent.
                aDirs = System.IO.Directory.GetDirectories(sourceDir)

                For i = 0 To aDirs.GetUpperBound(0)
                    Try
                        ' Get the position of the last separator in the current path.
                        posSep = aDirs(i).LastIndexOf(System.IO.Path.DirectorySeparatorChar)

                        ' Get the path of the source directory.
                        sDir = aDirs(i).Substring((posSep + 1), aDirs(i).Length - (posSep + 1))

                        ' Create the new directory in the destination directory.
                        System.IO.Directory.CreateDirectory(destDir + sDir)

                        ' Since we are in recursive mode, copy the children also
                        RecursiveCopyFiles(Caller, aDirs(i), (destDir + sDir), fRecursive, overWrite, Debugger)
                    Catch ex As Exception
                        If Not Debugger Is Nothing Then
                            r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Failed to complete directory copy of " & destDir & sDir & ": " & ex.Message, True)
                        End If
                    End Try
                Next

            End If

            ' Get the files from the current parent.
            aFiles = System.IO.Directory.GetFiles(sourceDir)


            ' Copy all files.
            For i = 0 To aFiles.GetUpperBound(0)

                ' Get the position of the trailing separator.
                posSep = aFiles(i).LastIndexOf(System.IO.Path.DirectorySeparatorChar)

                ' Get the full path of the source file.
                sFile = aFiles(i).Substring((posSep + 1), aFiles(i).Length - (posSep + 1))
                Try


                    ' Copy the file.
                    System.IO.File.Copy(aFiles(i), destDir + sFile, False)
                    If Not Debugger Is Nothing Then
                        r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Copied " & sFile & " to " & destDir, True)
                    End If
                Catch ex As Exception
                    If overWrite = False Then
                        If Not Debugger Is Nothing Then
                            r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Skipping..." & ex.Message, True)
                        End If
                    Else
                        Try
                            System.IO.File.Copy(aFiles(i), destDir + sFile, True)
                            If Not Debugger Is Nothing Then
                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Overwriting old " & sFile & " in " & destDir, True)
                            End If
                        Catch ex2 As Exception
                            If Not Debugger Is Nothing Then
                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Skipping Overwrite..." & ex2.Message, True)
                            End If
                        End Try
                    End If
                End Try
            Next i
        End Sub

        Private Sub File_Delete(ByRef FileObj As IO.FileInfo)
            File_RemoveReadonly(FileObj)
            FileObj.Delete()
        End Sub

        Private Sub File_RemoveReadonly(ByRef FileObj As IO.FileInfo)
            If CDbl(FileObj.Attributes And IO.FileAttributes.ReadOnly) = IO.FileAttributes.ReadOnly Then
                FileObj.Attributes = FileObj.Attributes Xor IO.FileAttributes.ReadOnly
            End If
        End Sub

        Private Sub Directory_RemoveReadonly(ByRef DirectoryObj As IO.DirectoryInfo)
            If CDbl(DirectoryObj.Attributes And IO.FileAttributes.ReadOnly) = IO.FileAttributes.ReadOnly Then
                DirectoryObj.Attributes = DirectoryObj.Attributes Xor IO.FileAttributes.ReadOnly
            End If
        End Sub
        Private Sub Directory_Delete(ByRef DirectoryObj As IO.DirectoryInfo, ByVal fRecursive As Boolean, Optional ByVal isRoot As Boolean = True)
            Directory_RemoveReadonly(DirectoryObj)
            Dim files As IO.FileInfo() = DirectoryObj.GetFiles
            Dim i As Integer
            For i = 0 To files.Length - 1
                Dim fio As IO.FileInfo = files(i)
                File_RemoveReadonly(fio)
            Next
            If fRecursive Then
                Dim directories As IO.DirectoryInfo() = DirectoryObj.GetDirectories
                If Not directories Is Nothing Then
                    For i = 0 To directories.Length - 1
                        Directory_Delete(directories(i), fRecursive, False)
                    Next
                End If
            End If
            Try
                If isRoot Then
                    DirectoryObj.Delete(True)
                End If
            Catch ex As Exception
                'ROMAIN: 09/18/07
                'TODO: Change Exceptions
                'DotNetNuke.Services.Exceptions.LogException(ex)
            End Try
        End Sub

        Private Sub RecurseGetFiles(ByRef Files As List(Of FileItem), ByVal MappedRoot As String, ByVal ApplicationRoot As String, ByVal Path As String, ByRef Debugger As Debugger, Optional ByVal RecurseDirectories As Boolean = False, Optional ByVal FileFilter As String = "*", Optional ByVal DirectoryFilter As String = "*")
            If IO.Directory.Exists(Path) Then
                If Not FileFilter Is Nothing AndAlso FileFilter.Length > 0 Then
                    For Each sFile As String In IO.Directory.GetFiles(Path, FileFilter)
                        Try
                            Dim fio As New IO.FileInfo(sFile)
                            Dim f As New FileItem(IO.Path.GetFileName(sFile))

                            f.Extension = IO.Path.GetExtension(sFile)
                            f.LocalPath = IO.Path.GetFullPath(sFile)
                            f.VirtualPath = FileItem.GetVirtualPath(f.LocalPath, ApplicationRoot)
                            f.RelativePath = FileItem.GetVirtualPath(f.LocalPath, MappedRoot)
                            f.RelativeLocalPath = FileItem.GetRelativePath(f.LocalPath, MappedRoot)
                            'Utility.StreamTransfer(fio.OpenRead(), f.Source)
                            f.Source = fio.Open(IO.FileMode.Open, IO.FileAccess.Read)
                            Files.Add(f)
                        Catch ex As Exception
                            If Not Debugger Is Nothing Then
                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Unable to Open File: " & ex.Message, True)
                            End If
                        End Try
                    Next

                    If RecurseDirectories Then
                        For Each sDir As String In IO.Directory.GetDirectories(Path, DirectoryFilter)
                            Try
                                RecurseGetFiles(Files, MappedRoot, ApplicationRoot, sDir, Debugger, RecurseDirectories, FileFilter, DirectoryFilter)
                                Dim d As New FileItem(IO.Path.GetDirectoryName(sDir))
                                d.Extension = ""
                                d.LocalPath = IO.Path.GetFullPath(sDir)
                                d.VirtualPath = FileItem.GetVirtualPath(d.LocalPath, ApplicationRoot)
                                d.RelativePath = FileItem.GetVirtualPath(d.LocalPath, MappedRoot)
                                d.RelativeLocalPath = FileItem.GetRelativePath(d.LocalPath, MappedRoot)
                                Files.Add(d)

                            Catch ex As Exception
                                If Not Debugger Is Nothing Then
                                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Unable to read directories: " & ex.Message, True)
                                End If
                            End Try
                        Next
                    End If
                ElseIf Not FileFilter Is Nothing AndAlso FileFilter.Length = 0 Then
                    Try
                        Dim dio As New IO.DirectoryInfo(Path)
                        Dim f As New FileItem(dio.FullName)
                        f.Extension = dio.Extension
                        f.LocalPath = IO.Path.GetFullPath(Path)
                        f.VirtualPath = FileItem.GetVirtualPath(f.LocalPath, ApplicationRoot)
                        f.RelativePath = FileItem.GetVirtualPath(f.LocalPath, MappedRoot)
                        f.RelativeLocalPath = FileItem.GetRelativePath(f.LocalPath, MappedRoot)
                        f.Source = Nothing
                        f.Data = Nothing
                        f.Abort = True
                        Files.Add(f)
                    Catch ex As Exception
                        If Not Debugger Is Nothing Then
                            r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Enable to Add File:" & ex.Message, True)
                        End If
                    End Try
                End If
            End If
        End Sub

#Region "Extended File Actions for SQL and CSV Import/Export"
#Region "CSV To SQL"
        Private Sub Handle_File_CSV_to_SQL(ByVal Obj As FileImport_ThreadedMessageActionProcess)
            Try
                Handle_File_CSV_to_SQL(Nothing, Obj.SharedDS, Obj.ThreadAction, Nothing, Obj.Name, Obj.SourceStream, Obj.DestinationIncludeColumnName, Obj.DestinationColumnMappings, Obj.DestinationTarget, Obj)
            Catch ex As Exception
                'ROMAIN: 09/18/07
                'TODO: Change Exceptions
                'DotNetNuke.Services.Exceptions.LogException(New Exception("The Process (" & Obj.Name & ") CSV file import failed to complete the process: " & ex.ToString))
            End Try
            Obj.Completed = True
        End Sub
        Private Function Handle_File_CSV_to_SQL(ByRef Caller As Runtime, ByRef sharedds As DataSet, ByVal act As MessageActionItem, ByRef Debugger As Debugger, ByVal ImportName As String, ByRef Source As IO.Stream, ByVal FirstRowIncludesColumnNames As Boolean, ByRef Mappings As MessageAction_File_ColumnMappings, ByVal RecordQuery As String, Optional ByVal threadObj As ThreadedMessageActionProcess = Nothing) As Boolean
            Try
                Dim SortNames As New SortedList(Of String, Integer)
                Dim Reader As New IO.StreamReader(Source)
                Dim Line As String = Nothing
                Dim lineCounter As Integer = 1
                Dim iColumns As Integer = 0
                Dim i As Integer = 1
                Dim cnObject As System.Data.IDbConnection
                Dim errorCounter As Integer = 0


                Dim _Engine As Engine = Nothing
                If Not Caller Is Nothing Then
                    _Engine = Caller.Engine
                ElseIf Not threadObj Is Nothing Then
                    _Engine = threadObj.RenderingEngine
                End If

                RecordQuery = _Engine.RenderString(sharedds, RecordQuery, _Engine.CapturedMessages, False, isPreRender:=False)
                If Not Caller Is Nothing AndAlso threadObj Is Nothing Then
                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "CSV Import (Query): " & RecordQuery, True)
                End If

                If _Engine.xls.customConnection Is Nothing OrElse _Engine.xls.customConnection.Length = 0 Then
                    'ROMAIN: 09/19/07
                    'cnObject = New System.Data.SqlClient.SqlConnection(DotNetNuke.Common.GetDBConnectionString)
                    cnObject = New System.Data.SqlClient.SqlConnection(AbstractFactory.Instance.EngineController.GetConnectionString())
                    cnObject.Open()
                Else
                    cnObject = New System.Data.Odbc.OdbcConnection(_Engine.xls.customConnection)
                    cnObject.Open()
                End If

                Reader.BaseStream.Position = 0
                Line = Reader.ReadLine
                If Line Is Nothing Then
                    If Not Caller Is Nothing AndAlso threadObj Is Nothing Then
                        r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "CSV Import Failure - Unable to open or read from file. Check that the file exists and that data is contained within.", True)
                    End If
                End If
                While Not Line Is Nothing
                    If Not threadObj Is Nothing Then
                        'WE NEED TO ASSIGN THE STATUS
                        If Reader.BaseStream.Length > 0 Then
                            threadObj.Percentage = (Reader.BaseStream.Position / Reader.BaseStream.Length) * 100
                        Else
                            threadObj.Percentage = 50
                        End If
                        threadObj.Status = lineCounter.ToString
                    End If
                    lineCounter += 1
                    If Not Line Is Nothing Then 'there is data here, so continue
                        Try
                            If Not Caller Is Nothing AndAlso threadObj Is Nothing Then
                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Importing Line - """ & Line & """", True)
                            End If
                            Dim parameters() As String = RenderBase.ParameterizeString(Line, ","c, """"c, "\"c, True)
                            If i = 1 Then
                                If FirstRowIncludesColumnNames Then
                                    If parameters.Length > 0 Then
                                        Dim strName As String
                                        For Each strName In parameters
                                            If Not SortNames.ContainsKey(strName) Then
                                                SortNames.Add(strName, iColumns)
                                            End If
                                            iColumns += 1
                                        Next
                                    End If
                                Else
                                    If parameters.Length > 0 Then
                                        iColumns = parameters.Length
                                    End If
                                End If
                            End If

                            If iColumns > 0 AndAlso ((FirstRowIncludesColumnNames And i > 1) Or Not FirstRowIncludesColumnNames) Then
                                'BUILD THE STATEMENT
                                Dim mapper As MessageAction_File_ColumnMappings.MessageAction_File_ColumnMappingItem
                                Dim Command As String = RecordQuery

                                For Each mapper In Mappings.ColumnMappings
                                    Dim Position As Integer = -2
                                    Dim strValue As String = ""
                                    'If mapper.Position.Length > 0 AndAlso IsNumeric(mapper.Position) Then
                                    '    If CType(mapper.Position, Integer) >= 0 AndAlso CType(mapper.Position, Integer) <= parameters.Length Then
                                    '        Position = CType(mapper.Position, Integer)
                                    '    End If
                                    'ElseIf mapper.Name.Length > 0 Then
                                    '    If SortNames.ContainsKey(mapper.Name) Then
                                    '        'ROMAIN: Generic replacement - 08/22/2007
                                    '        'Position = CType(SortNames(mapper.Name), Integer)
                                    '        Position = SortNames(mapper.Name)
                                    '    Else
                                    '        Position = -1
                                    '    End If
                                    'End If
                                    If mapper.FileType = "CSV" Then
                                        If mapper.Position.Length > 0 AndAlso IsNumeric(mapper.Position) Then
                                            If CType(mapper.Position, Integer) >= 0 AndAlso CType(mapper.Position, Integer) <= parameters.Length Then
                                                Position = CType(mapper.Position, Integer)
                                            End If
                                        ElseIf mapper.Name.Length > 0 Then
                                            If SortNames.ContainsKey(mapper.Name) Then
                                                Position = SortNames(mapper.Name)
                                            Else
                                                Position = -1
                                            End If
                                        End If
                                        'VERSION: 1.9.9.5 - Fixed for CSV Import of varying row column lengths
                                        If Position > -1 AndAlso Position < parameters.Length Then
                                            strValue = parameters(Position)
                                        Else
                                            strValue = ""
                                        End If
                                    ElseIf mapper.FileType = "FIXED" Then
                                        If mapper.StartColumn.Length > 0 AndAlso IsNumeric(mapper.StartColumn) AndAlso mapper.EndColumn.Length > 0 AndAlso IsNumeric(mapper.EndColumn) Then
                                            If CInt(mapper.StartColumn) <= CInt(mapper.EndColumn) Then
                                                Position = 0
                                                Try
                                                    strValue = Line.Substring((mapper.StartColumn - 1), (mapper.EndColumn - mapper.StartColumn + 1)).Trim()
                                                Catch
                                                    strValue = ""
                                                End Try
                                            End If
                                        End If
                                    End If


                                    If Position >= -1 Then
                                        'If Position > -1 Then
                                        '    strValue = parameters(Position)
                                        'Else
                                        '    strValue = ""
                                        'End If

                                        'HANDLE FORMAT
                                        If Not mapper.Format Is Nothing AndAlso mapper.Format.Length > 0 Then
                                            strValue = _Engine.RenderString(sharedds, strValue, _Engine.CapturedMessages, False, isPreRender:=False)
                                        End If

                                        'HANDLE TYPE
                                        Dim includeQuote As Boolean = True
                                        Select Case mapper.Type.ToUpper
                                            Case "NUMBER"
                                                Try
                                                    includeQuote = False
                                                    If strValue.Length = 0 Then
                                                        If Not mapper.DefaultValue Is Nothing Then
                                                            strValue = mapper.DefaultValue
                                                        Else
                                                            strValue = Nothing
                                                        End If
                                                    Else
                                                        strValue = Integer.Parse(strValue).ToString
                                                    End If
                                                Catch ex As Exception
                                                    If Not mapper.DefaultValue Is Nothing Then
                                                        strValue = mapper.DefaultValue
                                                    Else
                                                        strValue = Nothing
                                                    End If
                                                End Try
                                            Case "DECIMAL"
                                                Try
                                                    includeQuote = False
                                                    If strValue.Length = 0 Then
                                                        If Not mapper.DefaultValue Is Nothing Then
                                                            strValue = mapper.DefaultValue
                                                        Else
                                                            strValue = Nothing
                                                        End If
                                                    Else
                                                        strValue = Decimal.Parse(strValue).ToString
                                                    End If
                                                Catch ex As Exception
                                                    If Not mapper.DefaultValue Is Nothing Then
                                                        strValue = mapper.DefaultValue
                                                    Else
                                                        strValue = Nothing
                                                    End If
                                                End Try
                                            Case "TEXT"
                                                If strValue.Length = 0 Then
                                                    If Not mapper.DefaultValue Is Nothing Then
                                                        strValue = mapper.DefaultValue
                                                    Else
                                                        strValue = Nothing
                                                    End If
                                                End If
                                            Case "DATE"
                                                Try
                                                    Try
                                                        If strValue.Length = 0 Then
                                                            If Not mapper.DefaultValue Is Nothing Then
                                                                strValue = mapper.DefaultValue
                                                            Else
                                                                strValue = Nothing
                                                            End If
                                                        Else
                                                            strValue = DateTime.Parse(strValue).ToString("MM/dd/yyyy")
                                                        End If
                                                    Catch ex As Exception
                                                        If Not mapper.DefaultValue Is Nothing Then
                                                            strValue = mapper.DefaultValue
                                                        Else
                                                            strValue = Nothing
                                                        End If
                                                    End Try
                                                Catch ex As Exception
                                                End Try
                                            Case "TIME"
                                                Try
                                                    Try
                                                        If strValue.Length = 0 Then
                                                            If Not mapper.DefaultValue Is Nothing Then
                                                                strValue = mapper.DefaultValue
                                                            Else
                                                                strValue = Nothing
                                                            End If
                                                        Else
                                                            strValue = DateTime.Parse(strValue).ToString("hh:mm:ss tt")
                                                        End If
                                                    Catch ex As Exception
                                                        If Not mapper.DefaultValue Is Nothing Then
                                                            strValue = mapper.DefaultValue
                                                        Else
                                                            strValue = Nothing
                                                        End If
                                                    End Try
                                                Catch ex As Exception
                                                End Try
                                            Case "DATETIME"
                                                Try
                                                    Try
                                                        If strValue.Length = 0 Then
                                                            If Not mapper.DefaultValue Is Nothing Then
                                                                strValue = mapper.DefaultValue
                                                            Else
                                                                strValue = Nothing
                                                            End If
                                                        Else
                                                            strValue = DateTime.Parse(strValue).ToString("MM/dd/yyyy hh:mm:ss tt")
                                                        End If
                                                    Catch ex As Exception
                                                        If Not mapper.DefaultValue Is Nothing Then
                                                            strValue = mapper.DefaultValue
                                                        Else
                                                            strValue = Nothing
                                                        End If
                                                    End Try
                                                Catch ex As Exception
                                                End Try
                                        End Select
                                        'HANDLE NULL
                                        If strValue Is Nothing OrElse strValue = mapper.NullValue Then
                                            strValue = "NULL"
                                        Else
                                            If includeQuote Then
                                                strValue = "'" & strValue.Replace("'", "''") & "'"
                                            End If
                                        End If
                                        'THE VALUE IS NOW RENDERED - ASSIGN WITHIN QUERY
                                        Command = Command.Replace(mapper.Target, strValue)
                                    End If
                                Next

                                'EXECUTE THE COMMAND
                                If Not Command Is Nothing AndAlso Command.Length > 0 Then
                                    If threadObj Is Nothing Then
                                        r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Importing Line - """ & Line & """", True)
                                    End If
                                    Try
                                        Dim cmd As System.Data.IDbCommand = cnObject.CreateCommand()
                                        cmd.CommandText = Command
                                        If Not Caller Is Nothing AndAlso threadObj Is Nothing Then
                                            r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Executing: " & cmd.CommandText, True)
                                        End If
                                        cmd.ExecuteNonQuery()
                                        cmd = Nothing
                                    Catch ex As Exception
                                        If Not Caller Is Nothing AndAlso threadObj Is Nothing Then
                                            r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "CSV Import Failure - " & ex.ToString, True)
                                        Else
                                            threadObj.Errors += 1
                                        End If
                                        errorCounter += 1
                                    End Try
                                Else
                                    If Not Caller Is Nothing AndAlso threadObj Is Nothing Then
                                        r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "CSV Import Failure: No Query was available after processing the line.", True)
                                    End If
                                End If
                            End If
                        Catch ex As Exception
                            If Not threadObj Is Nothing Then
                                threadObj.Errors += 1
                            End If
                            errorCounter += 1
                        End Try
                        'VERSION: 1.8.0 - Corrected CSV Import failure issue and extended debugging. Documentation must reflect the extended documentaton and Process recommentdation.
                        i += 1
                        'GET THE NEXT ROW
                        Line = Reader.ReadLine
                    End If
                End While
                cnObject.Close()
                cnObject = Nothing

                If threadObj Is Nothing Then
                    _Engine.ActionVariable(ImportName & ".Percentage") = 100
                    _Engine.ActionVariable(ImportName & ".Status") = lineCounter
                    _Engine.ActionVariable(ImportName & ".Errors") = errorCounter
                    If errorCounter = 0 Then
                        _Engine.ActionVariable(ImportName & ".isSuccessful") = "True"
                    Else
                        _Engine.ActionVariable(ImportName & ".isSuccessful") = "False"
                    End If
                End If

                If threadObj Is Nothing Then
                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Completed Import - Closing File and Executing Child Operations.", True)
                End If
                If Not threadObj Is Nothing AndAlso Not threadObj.RenderingEngine Is Nothing AndAlso (Not threadObj.ThreadAction.ChildActions Is Nothing OrElse threadObj.ThreadAction.ChildActions.Count > 0) Then
                    threadObj.Percentage = 100
                    'Dim mah As New MessageActions(threadObj.RenderingEngine, threadObj.FilterField, threadObj.FilterText)
                    'mah.HandleProcessMessages(threadObj.ThreadAction.ChildActions, Debugger, 0, threadObj.SharedDS, _Engine.Session.SessionID & ":::" & threadObj.Name)
                    Dim exe As New Runtime(threadObj.RenderingEngine, threadObj.FilterField, threadObj.FilterText)
                    exe.Execute(threadObj.ThreadAction.ChildActions, Debugger, threadObj.SharedDS)
                ElseIf (threadObj Is Nothing AndAlso Not act Is Nothing AndAlso Not act.ChildActions Is Nothing AndAlso act.ChildActions.Count > 0) Then
                    If Not Caller Is Nothing Then
                        'Dim mah As New MessageActions(_Engine, Caller.FilterField, Caller.FilterText)
                        'mah.HandleProcessMessages(act.ChildActions, Debugger, 0, sharedds, _Engine.Session.SessionID & ":::Runtime")
                        Dim exe As New Runtime(_Engine, Caller.FilterField, Caller.FilterText)
                        exe.Execute(act.ChildActions, Debugger, sharedds)
                    End If
                End If

            Catch ex As Exception
                'ROMAIN: 09/18/07
                'TODO: Change Exceptions
                'DotNetNuke.Services.Exceptions.LogException(New Exception("CSV Import Failure: " & ex.ToString))
                If Not Caller Is Nothing AndAlso threadObj Is Nothing Then
                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "CSV Import Failure - " & ex.ToString, True)
                End If
            End Try
        End Function
#End Region
#Region "SQL to SQL"
        Private Sub Handle_File_SQL_to_SQL(ByVal Obj As FileImport_ThreadedMessageActionProcess)
            Try
                Handle_File_SQL_to_SQL(Nothing, Obj.SharedDS, Obj.ThreadAction, Nothing, Obj.Source, Obj.DestinationIncludeColumnName, Obj.DestinationColumnMappings, Obj.DestinationTarget, Obj)
            Catch ex As Exception
                'ROMAIN: 09/18/07
                'TODO: Change Exceptions
                'DotNetNuke.Services.Exceptions.LogException(New Exception("The Process (" & Obj.Name & ") CSV file import failed to complete the process: " & ex.ToString))
            End Try
            Obj.Completed = True
        End Sub
        Private Function Handle_File_SQL_to_SQL(ByRef Caller As Runtime, ByRef sharedds As DataSet, ByVal act As MessageActionItem, ByRef Debugger As Debugger, ByRef SourceTable As DataTable, ByVal FirstRowIncludesColumnNames As Boolean, ByRef Mappings As MessageAction_File_ColumnMappings, ByVal RecordQuery As String, Optional ByVal threadObj As ThreadedMessageActionProcess = Nothing) As Boolean
            Try
                'ROMAIN: Generic replacement - 08/22/2007
                'Dim SortNames As New SortedList
                Dim SortNames As New SortedList(Of String, Integer)
                Dim Source As DataTable

                If SourceTable Is Nothing Then
                    Source = New DataTable
                Else
                    Source = SourceTable
                End If

                Dim row As DataRow = Nothing
                Dim rowposition As Integer = 0
                Dim maxrow As Integer = Source.Rows.Count
                Dim lineCounter As Integer = 1
                Dim iColumns As Integer = 0
                Dim i As Integer = 1
                Dim cnObject As System.Data.IDbConnection


                Dim _Engine As Engine = Nothing
                If Not Caller Is Nothing Then
                    _Engine = Caller.Engine
                ElseIf Not threadObj Is Nothing Then
                    _Engine = threadObj.RenderingEngine
                End If

                RecordQuery = _Engine.RenderString(sharedds, RecordQuery, _Engine.CapturedMessages, False, isPreRender:=False)
                If Not Caller Is Nothing AndAlso threadObj Is Nothing Then
                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "CSV Import (Query): " & RecordQuery, True)
                End If

                If _Engine.xls.customConnection Is Nothing OrElse _Engine.xls.customConnection.Length = 0 Then
                    'ROMAIN: 09/19/07
                    'cnObject = New System.Data.SqlClient.SqlConnection(DotNetNuke.Common.GetDBConnectionString)
                    cnObject = New System.Data.SqlClient.SqlConnection(AbstractFactory.Instance.EngineController.GetConnectionString())
                    cnObject.Open()
                Else
                    cnObject = New System.Data.Odbc.OdbcConnection(_Engine.xls.customConnection)
                    cnObject.Open()
                End If

                If maxrow > 0 Then
                    row = Source.Rows(0)
                End If
                rowposition = 0
                If row Is Nothing Then
                    If Not Caller Is Nothing AndAlso threadObj Is Nothing Then
                        r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "SQL Import Failure - Unable to open or read from file. Check that the file exists and that data is contained within.", True)
                    End If
                End If
                While Not row Is Nothing
                    If Not threadObj Is Nothing Then
                        'WE NEED TO ASSIGN THE STATUS
                        If maxrow > 0 Then
                            threadObj.Percentage = (rowposition / maxrow) * 100
                        Else
                            threadObj.Percentage = 50
                        End If
                        threadObj.Status = lineCounter.ToString
                        lineCounter += 1
                    End If
                    If Not row Is Nothing Then 'there is data here, so continue
                        Try
                            If Not Caller Is Nothing AndAlso threadObj Is Nothing Then
                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Importing Row - """ & rowposition & """", True)
                            End If
                            'Dim parameters() As String = _Engine.ParameterizeString(Line, ","c, """"c, "\"c, True)
                            If i = 1 Then
                                If Source.Columns.Count > 0 Then
                                    Dim col As System.Data.DataColumn
                                    For Each col In Source.Columns
                                        If Not SortNames.ContainsKey(col.ColumnName) Then
                                            SortNames.Add(col.ColumnName, iColumns)
                                        End If
                                        iColumns += 1
                                    Next
                                End If
                            End If

                            If iColumns > 0 Then
                                'BUILD THE STATEMENT
                                Dim mapper As MessageAction_File_ColumnMappings.MessageAction_File_ColumnMappingItem
                                Dim Command As String = RecordQuery

                                For Each mapper In Mappings.ColumnMappings
                                    Dim Position As Integer = -1
                                    If mapper.Position.Length > 0 AndAlso IsNumeric(mapper.Position) Then
                                        If CType(mapper.Position, Integer) >= 0 AndAlso CType(mapper.Position, Integer) <= Source.Columns.Count Then
                                            Position = CType(mapper.Position, Integer)
                                        End If
                                    ElseIf mapper.Name.Length > 0 Then
                                        If SortNames.ContainsKey(mapper.Name) Then
                                            'ROMAIN: Generic replacement - 08/22/2007
                                            'Position = CType(SortNames(mapper.Name), Integer)
                                            Position = SortNames(mapper.Name)
                                        End If
                                    End If
                                    If Position > -1 Then
                                        Dim strValue As String = Nothing
                                        If Not IsDBNull(row(Position)) Then
                                            Try
                                                strValue = row(Position)
                                            Catch ex As Exception
                                            End Try
                                        Else
                                            strValue = ""
                                        End If

                                        'HANDLE FORMAT
                                        If Not mapper.Format Is Nothing AndAlso mapper.Format.Length > 0 Then
                                            strValue = _Engine.RenderString(sharedds, strValue, _Engine.CapturedMessages, False, isPreRender:=False)
                                        End If

                                        'HANDLE TYPE
                                        Select Case mapper.Type.ToUpper
                                            Case "NUMBER"
                                                Try
                                                    If strValue.Length = 0 Then
                                                        If Not mapper.DefaultValue Is Nothing Then
                                                            strValue = mapper.DefaultValue
                                                        Else
                                                            strValue = Nothing
                                                        End If
                                                    Else
                                                        strValue = Integer.Parse(strValue).ToString
                                                    End If
                                                Catch ex As Exception
                                                    If Not mapper.DefaultValue Is Nothing Then
                                                        strValue = mapper.DefaultValue
                                                    Else
                                                        strValue = Nothing
                                                    End If
                                                End Try
                                            Case "DECIMAL"
                                                Try
                                                    If strValue.Length = 0 Then
                                                        If Not mapper.DefaultValue Is Nothing Then
                                                            strValue = mapper.DefaultValue
                                                        Else
                                                            strValue = Nothing
                                                        End If
                                                    Else
                                                        strValue = Decimal.Parse(strValue).ToString
                                                    End If
                                                Catch ex As Exception
                                                    If Not mapper.DefaultValue Is Nothing Then
                                                        strValue = mapper.DefaultValue
                                                    Else
                                                        strValue = Nothing
                                                    End If
                                                End Try
                                            Case "TEXT"
                                                If strValue.Length = 0 Then
                                                    If Not mapper.DefaultValue Is Nothing Then
                                                        strValue = mapper.DefaultValue
                                                    Else
                                                        strValue = Nothing
                                                    End If
                                                End If
                                            Case "DATE"
                                                Try
                                                    Try
                                                        If strValue.Length = 0 Then
                                                            If Not mapper.DefaultValue Is Nothing Then
                                                                strValue = mapper.DefaultValue
                                                            Else
                                                                strValue = Nothing
                                                            End If
                                                        Else
                                                            strValue = DateTime.Parse(strValue).ToString("MM/dd/yyyy")
                                                        End If
                                                    Catch ex As Exception
                                                        If Not mapper.DefaultValue Is Nothing Then
                                                            strValue = mapper.DefaultValue
                                                        Else
                                                            strValue = Nothing
                                                        End If
                                                    End Try
                                                Catch ex As Exception
                                                End Try
                                            Case "TIME"
                                                Try
                                                    Try
                                                        If strValue.Length = 0 Then
                                                            If Not mapper.DefaultValue Is Nothing Then
                                                                strValue = mapper.DefaultValue
                                                            Else
                                                                strValue = Nothing
                                                            End If
                                                        Else
                                                            strValue = DateTime.Parse(strValue).ToString("hh:mm:ss tt")
                                                        End If
                                                    Catch ex As Exception
                                                        If Not mapper.DefaultValue Is Nothing Then
                                                            strValue = mapper.DefaultValue
                                                        Else
                                                            strValue = Nothing
                                                        End If
                                                    End Try
                                                Catch ex As Exception
                                                End Try
                                            Case "DATETIME"
                                                Try
                                                    Try
                                                        If strValue.Length = 0 Then
                                                            If Not mapper.DefaultValue Is Nothing Then
                                                                strValue = mapper.DefaultValue
                                                            Else
                                                                strValue = Nothing
                                                            End If
                                                        Else
                                                            strValue = DateTime.Parse(strValue).ToString("MM/dd/yyyy hh:mm:ss tt")
                                                        End If
                                                    Catch ex As Exception
                                                        If Not mapper.DefaultValue Is Nothing Then
                                                            strValue = mapper.DefaultValue
                                                        Else
                                                            strValue = Nothing
                                                        End If
                                                    End Try
                                                Catch ex As Exception
                                                End Try
                                        End Select
                                        'HANDLE NULL
                                        If strValue Is Nothing OrElse strValue = mapper.NullValue Then
                                            strValue = "NULL"
                                        Else
                                            strValue = "'" & strValue.Replace("'", "''") & "'"
                                        End If

                                        'THE VALUE IS NOW RENDERED - ASSIGN WITHIN QUERY
                                        Command = Command.Replace(mapper.Target, strValue)
                                    End If
                                Next

                                'EXECUTE THE COMMAND
                                If Not Command Is Nothing AndAlso Command.Length > 0 Then
                                    Try
                                        Dim cmd As System.Data.IDbCommand = cnObject.CreateCommand()
                                        cmd.CommandText = Command
                                        If Not Caller Is Nothing AndAlso threadObj Is Nothing Then
                                            r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Executing: " & cmd.CommandText, True)
                                        End If
                                        cmd.ExecuteNonQuery()
                                        cmd = Nothing
                                    Catch ex As Exception
                                        If Not Caller Is Nothing AndAlso threadObj Is Nothing Then
                                            r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "CSV Import Failure - " & ex.ToString, True)
                                        Else
                                            threadObj.Errors += 1
                                        End If
                                    End Try
                                Else
                                    If Not Caller Is Nothing AndAlso threadObj Is Nothing Then
                                        r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "CSV Import Failure: No Query was available after processing the line.", True)
                                    End If
                                End If
                            End If
                        Catch ex As Exception
                            If Not threadObj Is Nothing Then
                                threadObj.Errors += 1
                            End If
                        End Try
                        'VERSION: 1.8.0 - Corrected CSV Import failure issue and extended debugging. Documentation must reflect the extended documentaton and Process recommentdation.
                        i += 1
                        'GET THE NEXT ROW
                        rowposition += 1
                        If rowposition < maxrow Then
                            row = Source.Rows(rowposition)
                        Else
                            row = Nothing
                        End If
                    End If
                End While
                cnObject.Close()
                cnObject = Nothing

                If Not threadObj Is Nothing AndAlso Not threadObj.RenderingEngine Is Nothing AndAlso (Not threadObj.ThreadAction.ChildActions Is Nothing OrElse threadObj.ThreadAction.ChildActions.Count > 0) Then
                    threadObj.Percentage = 100
                    'Dim mah As New MessageActions(threadObj.RenderingEngine, threadObj.FilterField, threadObj.FilterText)
                    'mah.HandleProcessMessages(threadObj.ThreadAction.ChildActions, Nothing, 0, threadObj.SharedDS, _Engine.Session.SessionID & ":::" & threadObj.Name)
                    Dim exe As New Runtime(threadObj.RenderingEngine, threadObj.FilterField, threadObj.FilterText)
                    exe.Execute(threadObj.ThreadAction.ChildActions, Nothing, threadObj.SharedDS)
                End If

            Catch ex As Exception
                'ROMAIN: 09/18/07
                'TODO: Change Exceptions
                'DotNetNuke.Services.Exceptions.LogException(New Exception("SQL Import Failure: " & ex.ToString))
                If Not Caller Is Nothing AndAlso threadObj Is Nothing Then
                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "SQL Import Failure - " & ex.ToString, True)
                End If
            End Try
        End Function
#End Region
#Region "SQL to CSV"
        Private Sub Handle_File_SQL_to_CSV(ByVal Obj As FileImport_ThreadedMessageActionProcess)
            Try
                Handle_File_SQL_to_CSV(Nothing, Obj.SharedDS, Obj.ThreadAction, Nothing, Obj.Source, Obj.SourceStream, Obj.DestinationIncludeColumnName, Obj.DestinationColumnMappings, Obj.DestinationTarget, Obj.Delimiter, Obj)
            Catch ex As Exception
                'ROMAIN: 09/18/07
                'TODO: Change Exceptions
                'DotNetNuke.Services.Exceptions.LogException(New Exception("The Process (" & Obj.Name & ") CSV file import failed to complete the process: " & ex.ToString))
            End Try
            Obj.Completed = True
        End Sub
        Private Function Handle_File_SQL_to_CSV(ByRef Caller As Runtime, ByRef sharedds As DataSet, ByVal act As MessageActionItem, ByRef Debugger As Debugger, ByRef SourceTable As DataTable, ByVal TargetStream As IO.Stream, ByVal FirstRowIncludesColumnNames As Boolean, ByRef Mappings As MessageAction_File_ColumnMappings, ByVal RecordQuery As String, ByVal Delimiter As String, Optional ByVal threadObj As ThreadedMessageActionProcess = Nothing) As Boolean
            Try
                FirstRowIncludesColumnNames = True
                Dim sWriter As New IO.StreamWriter(TargetStream, System.Text.Encoding.UTF8)
                'ROMAIN: Generic replacement - 08/22/2007
                'Dim SortNames As New SortedList
                Dim SortNames As New SortedList(Of String, Integer)
                Dim Source As DataTable

                If SourceTable Is Nothing Then
                    Source = New DataTable
                Else
                    Source = SourceTable
                End If

                Dim _Engine As Engine = Nothing
                If Not Caller Is Nothing Then
                    _Engine = Caller.Engine
                ElseIf Not threadObj Is Nothing Then
                    _Engine = threadObj.RenderingEngine
                End If

                Dim row As DataRow = Nothing
                Dim rowposition As Integer = 0
                Dim maxrow As Integer = Source.Rows.Count
                Dim lineCounter As Integer = 1
                Dim iColumns As Integer = 0
                Dim i As Integer = 1

                If maxrow > 0 Then
                    row = Source.Rows(0)
                End If
                rowposition = 0
                If row Is Nothing Then
                    If Not Caller Is Nothing AndAlso threadObj Is Nothing Then
                        r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "SQL Import Failure - Unable to open or read from file. Check that the file exists and that data is contained within.", True)
                    End If
                End If
                While Not row Is Nothing
                    If Not threadObj Is Nothing Then
                        'WE NEED TO ASSIGN THE STATUS
                        If maxrow > 0 Then
                            threadObj.Percentage = (rowposition / maxrow) * 100
                        Else
                            threadObj.Percentage = 50
                        End If
                        threadObj.Status = lineCounter.ToString
                        lineCounter += 1
                    End If
                    If Not row Is Nothing Then 'there is data here, so continue
                        Try
                            If Not Caller Is Nothing AndAlso threadObj Is Nothing Then
                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Importing Row - """ & rowposition & """", True)
                            End If
                            'Dim parameters() As String = _Engine.ParameterizeString(Line, ","c, """"c, "\"c, True)
                            If i = 1 Then
                                If Source.Columns.Count > 0 Then
                                    Dim col As System.Data.DataColumn
                                    For Each col In Source.Columns
                                        If Not SortNames.ContainsKey(col.ColumnName) Then
                                            SortNames.Add(col.ColumnName, iColumns)
                                        End If
                                        If FirstRowIncludesColumnNames Then
                                            If iColumns > 0 Then
                                                sWriter.Write(Delimiter)
                                            End If
                                            sWriter.Write(col.ColumnName)
                                        End If
                                        iColumns += 1
                                    Next
                                    If FirstRowIncludesColumnNames Then
                                        sWriter.Write(vbCrLf)
                                    End If
                                End If
                            End If


                            If iColumns > 0 Then
                                'BUILD THE STATEMENT
                                Dim mapper As MessageAction_File_ColumnMappings.MessageAction_File_ColumnMappingItem
                                Dim Command As String = RecordQuery

                                If Not Mappings Is Nothing AndAlso Mappings.ColumnMappings.Count > 0 Then
                                    For Each mapper In Mappings.ColumnMappings
                                        Dim Position As Integer = -1
                                        If mapper.Position.Length > 0 AndAlso IsNumeric(mapper.Position) Then
                                            If CType(mapper.Position, Integer) > 0 AndAlso CType(mapper.Position, Integer) <= Source.Columns.Count Then
                                                Position = CType(mapper.Position, Integer)
                                            End If
                                        ElseIf mapper.Name.Length > 0 Then
                                            If SortNames.ContainsKey(mapper.Name) Then
                                                'ROMAIN: Generic replacement - 08/22/2007
                                                'Position = CType(SortNames(mapper.Name), Integer)
                                                Position = SortNames(mapper.Name)
                                            End If
                                        End If
                                        If Position > -1 Then
                                            Dim strValue As String = Nothing
                                            If Not IsDBNull(row(Position)) Then
                                                Try
                                                    strValue = row(Position)
                                                Catch ex As Exception
                                                End Try
                                            Else
                                                strValue = ""
                                            End If

                                            'HANDLE FORMAT
                                            If Not mapper.Format Is Nothing AndAlso mapper.Format.Length > 0 Then
                                                strValue = _Engine.RenderString(sharedds, strValue, _Engine.CapturedMessages, False, isPreRender:=False)
                                            End If

                                            'HANDLE TYPE
                                            Select Case mapper.Type.ToUpper
                                                Case "NUMBER"
                                                    Try
                                                        If strValue.Length = 0 Then
                                                            If Not mapper.DefaultValue Is Nothing Then
                                                                strValue = mapper.DefaultValue
                                                            Else
                                                                strValue = Nothing
                                                            End If
                                                        Else
                                                            strValue = Integer.Parse(strValue).ToString
                                                        End If
                                                    Catch ex As Exception
                                                        If Not mapper.DefaultValue Is Nothing Then
                                                            strValue = mapper.DefaultValue
                                                        Else
                                                            strValue = Nothing
                                                        End If
                                                    End Try
                                                Case "DECIMAL"
                                                    Try
                                                        If strValue.Length = 0 Then
                                                            If Not mapper.DefaultValue Is Nothing Then
                                                                strValue = mapper.DefaultValue
                                                            Else
                                                                strValue = Nothing
                                                            End If
                                                        Else
                                                            strValue = Decimal.Parse(strValue).ToString
                                                        End If
                                                    Catch ex As Exception
                                                        If Not mapper.DefaultValue Is Nothing Then
                                                            strValue = mapper.DefaultValue
                                                        Else
                                                            strValue = Nothing
                                                        End If
                                                    End Try
                                                Case "TEXT"
                                                    If strValue.Length = 0 Then
                                                        If Not mapper.DefaultValue Is Nothing Then
                                                            strValue = mapper.DefaultValue
                                                        Else
                                                            strValue = Nothing
                                                        End If
                                                    End If
                                                Case "DATE"
                                                    Try
                                                        Try
                                                            If strValue.Length = 0 Then
                                                                If Not mapper.DefaultValue Is Nothing Then
                                                                    strValue = mapper.DefaultValue
                                                                Else
                                                                    strValue = Nothing
                                                                End If
                                                            Else
                                                                strValue = DateTime.Parse(strValue).ToString("MM/dd/yyyy")
                                                            End If
                                                        Catch ex As Exception
                                                            If Not mapper.DefaultValue Is Nothing Then
                                                                strValue = mapper.DefaultValue
                                                            Else
                                                                strValue = Nothing
                                                            End If
                                                        End Try
                                                    Catch ex As Exception
                                                    End Try
                                                Case "TIME"
                                                    Try
                                                        Try
                                                            If strValue.Length = 0 Then
                                                                If Not mapper.DefaultValue Is Nothing Then
                                                                    strValue = mapper.DefaultValue
                                                                Else
                                                                    strValue = Nothing
                                                                End If
                                                            Else
                                                                strValue = DateTime.Parse(strValue).ToString("hh:mm:ss tt")
                                                            End If
                                                        Catch ex As Exception
                                                            If Not mapper.DefaultValue Is Nothing Then
                                                                strValue = mapper.DefaultValue
                                                            Else
                                                                strValue = Nothing
                                                            End If
                                                        End Try
                                                    Catch ex As Exception
                                                    End Try
                                                Case "DATETIME"
                                                    Try
                                                        Try
                                                            If strValue.Length = 0 Then
                                                                If Not mapper.DefaultValue Is Nothing Then
                                                                    strValue = mapper.DefaultValue
                                                                Else
                                                                    strValue = Nothing
                                                                End If
                                                            Else
                                                                strValue = DateTime.Parse(strValue).ToString("MM/dd/yyyy hh:mm:ss tt")
                                                            End If
                                                        Catch ex As Exception
                                                            If Not mapper.DefaultValue Is Nothing Then
                                                                strValue = mapper.DefaultValue
                                                            Else
                                                                strValue = Nothing
                                                            End If
                                                        End Try
                                                    Catch ex As Exception
                                                    End Try
                                            End Select
                                            'HANDLE NULL
                                            If strValue Is Nothing OrElse strValue = mapper.NullValue Then
                                                strValue = "NULL"
                                            Else
                                                strValue = "'" & strValue.Replace("'", "''") & "'"
                                            End If

                                            'THE VALUE IS NOW RENDERED - ASSIGN WITHIN QUERY
                                            Command = Command.Replace(mapper.Target, strValue)
                                        End If
                                    Next
                                Else
                                    Dim strValue As String
                                    Dim ci As Integer
                                    Command = ""
                                    For ci = 0 To row.Table.Columns.Count - 1
                                        Try
                                            If Not IsDBNull(row(ci)) Then
                                                strValue = row(ci)
                                            Else
                                                strValue = ""
                                            End If
                                        Catch ex As Exception
                                            strValue = "#ERROR"
                                        End Try
                                        If ci > 0 Then
                                            Command &= Delimiter
                                        End If
                                        If Delimiter = "," Then
                                            If strValue.Contains(Delimiter) OrElse Not IsNumeric(strValue) Then
                                                Command &= """" & strValue.Replace("""", """""") & """"
                                            Else
                                                Command &= strValue
                                            End If
                                        Else
                                            Command &= strValue.Replace(vbCr, "").Replace(vbLf, " ").Replace(vbTab, " ")
                                        End If
                                    Next
                                End If

                                'ATTACH THE COMMAND VALUE

                                If Not Command Is Nothing AndAlso Command.Length > 0 Then
                                    Try
                                        sWriter.WriteLine(Command)
                                    Catch ex As Exception
                                        If Not Caller Is Nothing AndAlso threadObj Is Nothing Then
                                            r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "CSV Export Failure - " & ex.ToString, True)
                                        Else
                                            threadObj.Errors += 1
                                        End If
                                    End Try
                                Else
                                    If Not Caller Is Nothing AndAlso threadObj Is Nothing Then
                                        r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "CSV Export Failure: No line was available after processing the row.", True)
                                    End If
                                End If
                            End If
                        Catch ex As Exception
                            If Not threadObj Is Nothing Then
                                threadObj.Errors += 1
                            End If
                        End Try
                        'VERSION: 1.8.0 - Corrected CSV Import failure issue and extended debugging. Documentation must reflect the extended documentaton and Process recommentdation.
                        i += 1
                        'GET THE NEXT ROW
                        rowposition += 1
                        If rowposition < maxrow Then
                            row = Source.Rows(rowposition)
                        Else
                            row = Nothing
                        End If
                    End If
                End While
                sWriter.Flush()
                sWriter = Nothing

                If Not threadObj Is Nothing AndAlso Not threadObj.RenderingEngine Is Nothing AndAlso (Not threadObj.ThreadAction.ChildActions Is Nothing OrElse threadObj.ThreadAction.ChildActions.Count > 0) Then
                    threadObj.Percentage = 100
                    'Dim mah As New MessageActions(threadObj.RenderingEngine, threadObj.FilterField, threadObj.FilterText)
                    'mah.HandleProcessMessages(threadObj.ThreadAction.ChildActions, Nothing, 0, threadObj.SharedDS, _Engine.Session.SessionID & ":::" & threadObj.Name)
                    Dim exe As New Runtime(threadObj.RenderingEngine, threadObj.FilterField, threadObj.FilterText)
                    exe.Execute(threadObj.ThreadAction.ChildActions, Nothing, threadObj.SharedDS)
                End If

            Catch ex As Exception
                'ROMAIN: 09/18/07
                'TODO: Change Exceptions
                'DotNetNuke.Services.Exceptions.LogException(New Exception("SQL Export Failure: " & ex.ToString))
                If Not Caller Is Nothing AndAlso threadObj Is Nothing Then
                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "SQL Export Failure - " & ex.ToString, True)
                End If
            End Try
        End Function
#End Region
#End Region

        Public Overrides Function Key() As String
            Return "Action-File"
        End Function
    End Class
End Namespace