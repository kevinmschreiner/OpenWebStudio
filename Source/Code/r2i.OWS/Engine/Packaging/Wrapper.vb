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
'ROMAIN: UNUSED Imports - 08/21/2007
'Imports System.Xml
'Imports ICSharpCode.SharpZipLib.Checksums
Imports ICSharpCode.SharpZipLib.Zip
Imports r2i.OWS.Framework.DataAccess

Namespace r2i.OWS.Packaging
    Public Class Wrapper
        Public Result As String
        Private TabID As String
        Private ModuleID As String
        'ROMAIN: 08/21/07
        'NOTE: Unused field
        'Public Results As New ArrayList
        Public Configured As Boolean '= False
        Public Scripted As Boolean '= False
        Private PortalID As Integer
        Private BasePath As String
        'ROMAIN: 09/21/07
        'Private PortalSettings As DotNetNuke.Entities.Portals.PortalSettings
        Private PortalSettings As IPortalSettings
        Private _renderer As Engine


        'Public Sub New(ByVal RenderingEngine As Engine, ByVal ModuleID As Integer, ByVal TabID As Integer, ByVal PortalID As Integer, ByVal BasePath As String, ByVal PortalSettings As DotNetNuke.Entities.Portals.PortalSettings)
        '    Me.ModuleID = ModuleID
        '    Me.PortalID = PortalID
        '    Me.TabID = TabID
        '    Me.BasePath = BasePath
        '    Me.PortalSettings = PortalSettings
        '    _renderer = RenderingEngine
        'End Sub
        Public Sub New(ByVal RenderingEngine As Engine, ByVal ModuleID As String, ByVal TabID As String, ByVal PortalID As String, ByVal BasePath As String, ByVal PortalSettings As IPortalSettings)
            Me.ModuleID = ModuleID
            Me.PortalID = PortalID
            Me.TabID = TabID
            Me.BasePath = BasePath
            Me.PortalSettings = PortalSettings
            _renderer = RenderingEngine
        End Sub

        Public Function Unwrap(ByVal File As Web.HttpPostedFile) As Common.PackageType
            'SAVE THE FILE INTO THE PACKAGE AREA
            'ANALYZE THE FILE TO DETERMINE WHAT THE CURRENT TASK SHOULD BE:
            '   .ZIP        -  EXTRACT TO P[RANDOMNUMBER]
            '   .INSTALL    -  IDENTIFIES THE FILENAMES AND TASKS FOR EACH
            '   .CONFIG     -  UPLOAD ONLY ALLOWS .ZIP and .CONFIG files. .ZIP is a full package. .CONFIG is a XML Configuration.


            'Dim execResult As New Common.ItemTypeResult(Common.ItemTypeResult.ResultTypeEnumerator.Install)
            'execResult.Initiated = Now
            'execResult.Name = "Installing File " & File.FileName
            'Me.Results.Add(execResult)
            Try
                Select Case True
                    Case File.FileName.ToLower.EndsWith(".zip")
                        'PACKAGE UPLOAD
                        Return Decompress(File)
                    Case File.FileName.ToLower.EndsWith(".config"), File.FileName.ToLower.EndsWith(".xml")
                        'XLIST SETTINGS FILE
                        'JUST A CONFIGURATION - DO WHAT?

                        Dim fstream As New IO.StreamReader(File.InputStream)
                        Dim fsrc As String = fstream.ReadToEnd
                        fstream.Close()

                        Result = fsrc
                        Return Common.PackageType.ConfigurationOnly
                    Case Else
                        Result = Nothing
                        Return Common.PackageType.Unknown
                End Select
            Catch ex As Exception
                Result = ex.ToString
                Return Common.PackageType.Failure
            End Try
        End Function
        Public ReadOnly Property FullPath() As String
            Get
                Return _FullPath
            End Get
        End Property
        Private _FullPath As String
        Public Sub RemovePackageFolder()
            Dim sPath As String = _FullPath
            Dim dinfo As New IO.DirectoryInfo(sPath)
            If dinfo.Exists Then
                dinfo.Delete(True)
            End If
        End Sub
        Private Function CheckReadWrite(ByVal Path As String) As Boolean
            Try
                Dim dir As New IO.DirectoryInfo(Path)
                If dir.Exists Then
                    If CDbl(dir.Attributes And IO.FileAttributes.ReadOnly) = IO.FileAttributes.ReadOnly Then
                        'READONLY
                        System.IO.File.SetAttributes(Path, CDbl(dir.Attributes Or IO.FileAttributes.ReadOnly))
                    End If
                    dir = New IO.DirectoryInfo(Path)
                    If CDbl(dir.Attributes And IO.FileAttributes.ReadOnly) = IO.FileAttributes.ReadOnly Then
                        'FAILED
                        Return False
                    Else
                        Return True
                    End If
                End If
                Return True
            Catch ex As Exception
                Return False
            End Try
        End Function
        Private Function Decompress(ByVal File As Web.HttpPostedFile) As Common.PackageType
            Dim rsltType As Common.PackageType = Common.PackageType.Failure
            Try
                'CREATE FOLDER - UNPACK
                Dim folderName As String = "Pkg" & (New Random).Next(0, 9000000)
                'Dim sPath As String = _MapPath(BasePath & folderName)
                Dim canReadWrite As Object = CheckReadWrite(_renderer.Context.Server.MapPath(BasePath))
                If Not canReadWrite Then
                    Return Common.PackageType.FailureReadOnly
                End If
                Dim sPath As String = _renderer.Context.Server.MapPath(BasePath & folderName)
                canReadWrite = CheckReadWrite(sPath)
                If Not canReadWrite Then
                    Return Common.PackageType.FailureReadOnly
                End If
                If Not IO.Directory.Exists(sPath) Then
                    IO.Directory.CreateDirectory(sPath)
                End If
                canReadWrite = CheckReadWrite(sPath)
                If Not canReadWrite Then
                    Return Common.PackageType.FailureReadOnly
                End If
                If IO.Directory.Exists(sPath) Then
                    _FullPath = sPath
                    File.SaveAs(sPath & "/" & folderName & ".zip")
                    If Decompress_File(sPath & "/" & folderName & ".zip", sPath & "/") Then
                        'LOCATE THE INSTALL FILE
                        Dim fios() As IO.FileInfo
                        Dim dinfo As New IO.DirectoryInfo(sPath)
                        fios = dinfo.GetFiles("*.install")
                        If Not fios Is Nothing AndAlso fios.Length = 1 Then
                            'INSTALL EXISTS - 
                            Dim fstream As IO.FileStream = fios(0).OpenRead
                            Dim treader As New IO.StreamReader(fstream)
                            Dim installScript As String = treader.ReadToEnd()
                            treader.Close()

                            Result = installScript

                            rsltType = Common.PackageType.StandardPackage
                        End If
                    End If
                Else
                    rsltType = Common.PackageType.Unknown
                End If
            Catch ex As Exception
                rsltType = Common.PackageType.Failure
            End Try

            Return rsltType
        End Function

#Region "Package Compression"
        Private Function Decompress_File(ByVal FileLoc As String, ByVal destPath As String) As Boolean
            Dim errorCounter As Integer = 0
            Try
                Dim objZipEntry As ZipEntry
                Dim strFileName As String = ""
                Dim objZipInputStream As ZipInputStream
                Try
                    objZipInputStream = New ZipInputStream(System.IO.File.OpenRead(FileLoc))
                Catch ex As Exception

                    Return ex.Message
                End Try

                Dim LocalFileName, RelativeDir, FileNamePath As String

                objZipEntry = objZipInputStream.GetNextEntry
                While Not objZipEntry Is Nothing
                    ' This gets the Zipped FileName (including the path)
                    LocalFileName = objZipEntry.Name

                    ' This creates the necessary directories if they don't 
                    ' already exist.


                    RelativeDir = System.IO.Path.GetDirectoryName(objZipEntry.Name)
                    If (RelativeDir <> String.Empty) AndAlso (Not System.IO.Directory.Exists(System.IO.Path.Combine(destPath, RelativeDir))) Then
                        System.IO.Directory.CreateDirectory(System.IO.Path.Combine(destPath, RelativeDir))
                    End If

                    ' This block creates the file using buffered reads from the zipfile
                    If (Not objZipEntry.IsDirectory) AndAlso (LocalFileName <> "") Then
                        FileNamePath = System.IO.Path.Combine(destPath, LocalFileName).Replace("/", "\")
                        Try
                            ' delete the file if it already exists
                            If System.IO.File.Exists(FileNamePath) Then
                                System.IO.File.SetAttributes(FileNamePath, System.IO.FileAttributes.Normal)
                                System.IO.File.Delete(FileNamePath)
                            End If

                            ' create the file
                            Dim objFileStream As System.IO.FileStream = System.IO.File.Create(FileNamePath)

                            Dim intSize As Integer = 2048
                            Dim arrData(2048) As Byte

                            intSize = objZipInputStream.Read(arrData, 0, arrData.Length)
                            While intSize > 0
                                objFileStream.Write(arrData, 0, intSize)
                                intSize = objZipInputStream.Read(arrData, 0, arrData.Length)
                            End While

                            objFileStream.Close()
                        Catch exZ As Exception
                            ' an error occurred saving a file in the resource file
                            errorCounter += 1
                        End Try
                    End If

                    objZipEntry = objZipInputStream.GetNextEntry
                End While
                If Not objZipInputStream Is Nothing Then
                    objZipInputStream.Close()
                End If

                objZipInputStream.Close()
            Catch ex As Exception
                Return False
            End Try
            If errorCounter = 0 Then
                Return True
            Else
                Return False
            End If
        End Function
#End Region
    End Class
End Namespace
