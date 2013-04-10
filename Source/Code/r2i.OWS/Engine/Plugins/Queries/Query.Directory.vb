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
Imports r2i.OWS.Framework.Plugins.Queries
Imports r2i.OWS.Framework
Imports r2i.OWS.Framework.Utilities


Namespace r2i.OWS.Queries
    Public Class Directory
        Inherits QueryBase

        Public Overrides ReadOnly Property QueryTag() As String
            Get
                Return "<DIRECTORY>"
            End Get
        End Property

        Public Overrides ReadOnly Property QueryStructure() As String
            Get
                Dim s As String = ""

                s &= "<DIRECTORY>" & vbCrLf
                s &= "   <PATH></PATH>" & vbCrLf
                s &= "   <DIRECTORYSEARCH></DIRECTORYSEARCH>" & vbCrLf
                s &= "   <FILESEARCH></FILESEARCH>" & vbCrLf
                s &= "   <FLAGS></FLAGS>" & vbCrLf
                s &= "   <QUERY></QUERY>" & vbCrLf
                s &= "   <SORT></SORT>" & vbCrLf
                s &= "</DIRECTORY>" & vbCrLf

                Return s
            End Get
        End Property

        Public Overrides Function Handle_GetData(ByRef Caller As EngineBase, ByVal isSubQuery As Boolean, ByVal Query As String, ByVal FilterField As String, ByVal FilterText As String, ByRef DebugWriter As Framework.Debugger, ByVal isRendered As Boolean, Optional ByVal timeout As Integer = -1, Optional ByVal CustomConnection As String = Nothing) As Framework.RuntimeBase.QueryResult
            Dim rslt As New Framework.RuntimeBase.QueryResult(RuntimeBase.ExecutableResultEnum.Executed, New DataSet)
            Try
                If Not Query Is Nothing AndAlso Query.Length > 0 Then
                    Dim strPath As String
                    Dim strDirectoryFilter As String
                    Dim strFileFilter As String
                    Dim strFlags As String
                    Dim strQuery As String
                    Dim strSort As String
                    Dim strAppBasePath As String
                    Try
                        strAppBasePath = Caller.Context.Server.MapPath("")
                    Catch ex As Exception
                        strAppBasePath = Caller.Context.Server.MapPath("/")
                    End Try



                    strPath = Utility.XMLPropertyParse_Quick(Query, "path")
                    Dim pathpos As Integer
                    If Not strPath Is Nothing Then
                        pathpos = strPath.IndexOf(":")
                        If pathpos < 0 OrElse pathpos > 10 Then
                            Try
                                strPath = Caller.Context.Server.MapPath(strPath)
                            Catch ex As Exception
                            End Try
                        End If
                    End If

                    strDirectoryFilter = Utility.XMLPropertyParse_Quick(Query, "directorysearch")
                    strFileFilter = Utility.XMLPropertyParse_Quick(Query, "filesearch")
                    strFlags = Utility.XMLPropertyParse_Quick(Query, "flags")
                    strQuery = Utility.XMLPropertyParse_Quick(Query, "query")
                    strSort = Utility.XMLPropertyParse_Quick(Query, "sort")

                    Dim dt As DataTable = GetFileObjects(strAppBasePath, strPath, strDirectoryFilter, strFileFilter, strFlags)
                    If Not dt Is Nothing Then
                        Dim dtR As DataRow() = Nothing
                        Dim isFiltered As Boolean = False
                        If Not strQuery Is Nothing AndAlso strQuery.Length > 0 Then
                            If Not strSort Is Nothing AndAlso strSort.Length > 0 Then
                                dtR = dt.Select(strQuery, strSort)
                            Else
                                dtR = dt.Select(strQuery)
                            End If
                            isFiltered = True
                        ElseIf Not strSort Is Nothing AndAlso strSort.Length > 0 Then
                            dtR = dt.Select("1=1", strSort)
                            isFiltered = True
                        End If
                        If isFiltered Then
                            Dim dtx As DataTable = dt.Clone
                            Dim dr As DataRow = Nothing
                            For Each dr In dtR
                                dtx.ImportRow(dr)
                            Next
                            rslt.Value.Tables.Add(dtx)
                        Else
                            rslt.Value.Tables.Add(dt)
                        End If
                    End If
                End If
            Catch ex As Exception
                Framework.Utilities.Utility.SortStatus(Caller.Session, Caller.ConfigurationID.ToString.Replace("{", "").Replace("}", "").Replace("-", ""), Caller.ModuleID, Caller.UserID) = Nothing
                rslt.Result = RuntimeBase.ExecutableResultEnum.Failed
                rslt.Error = ex
            End Try

            Return rslt
        End Function

        Private Function GetFileObjects(ByVal BasePath As String, ByVal StartPath As String, ByVal Filter As String, ByVal FileFilter As String, ByVal Attributes As String) As System.Data.DataTable
            Dim dt As Data.DataTable = GetSystemFileDatatable(StartPath)
            Dim rootDirectory As New System.IO.DirectoryInfo(StartPath)

            If Attributes Is Nothing Then Attributes = ""

            If Attributes.IndexOf("D") >= 0 AndAlso Attributes.IndexOf("F") < 0 Then
                'DIRECTORIES ONLY 
                GetDirectories_Recursive(rootDirectory, Filter, FileFilter, Attributes, dt, False, True, BasePath)
            ElseIf Attributes.IndexOf("F") >= 0 AndAlso Attributes.IndexOf("D") < 0 Then
                'FILES ONLY
                Attributes.Replace("F", "")
                GetDirectories_Recursive(rootDirectory, Filter, FileFilter, Attributes, dt, True, True, BasePath)
            Else
                'FILES AND DIRECTORIES
                Attributes.Replace("F", "")
                Attributes.Replace("D", "")
                GetDirectories_Recursive(rootDirectory, Filter, FileFilter, Attributes, dt, True, True, BasePath)
            End If
            Return dt
        End Function
        Private Sub GetDirectories_Recursive(ByVal CurrentDirectory As IO.DirectoryInfo, ByVal Filter As String, ByVal FileFilter As String, ByVal Attributes As String, ByRef table As System.Data.DataTable, ByVal IncludeFiles As Boolean, ByVal isRootFolder As Boolean, ByVal BasePath As String)
            'VERSION: 2.0 Added Multi-Filter for Get Directory
            Try
                Dim includeSubdirectories As Boolean = False
                If Attributes Is Nothing Then Attributes = ""

                If Attributes.IndexOf("I") >= 0 Then
                    Attributes = Attributes.Replace("I", "")
                    includeSubdirectories = True
                End If

                If Not CurrentDirectory Is Nothing AndAlso CurrentDirectory.Exists Then
                    Dim items() As System.IO.DirectoryInfo
                    If Not Filter Is Nothing AndAlso Filter.Length > 0 Then
                        items = CurrentDirectory.GetDirectories(Filter)
                    Else
                        items = CurrentDirectory.GetDirectories
                    End If

                    PopulateDirectoryTable(CType(items, Array), table, Attributes, BasePath)

                    If IncludeFiles Then
                        Dim files As New ArrayList
                        If Not FileFilter Is Nothing AndAlso FileFilter.Length > 0 Then
                            Dim ffilters As String() = FileFilter.Split(";")
                            Dim ffilter As String

                            For Each ffilter In ffilters
                                files.AddRange(CurrentDirectory.GetFiles(ffilter))
                            Next
                        Else
                            files.AddRange(CurrentDirectory.GetFiles("*.*"))
                        End If
                        If files.Count > 0 Then
                            Try
                                PopulateFileTable(files, table, Attributes, BasePath)
                            Catch ex As Exception

                            End Try
                        End If
                    End If


                    If items.Length > 0 AndAlso includeSubdirectories Then
                        Dim di As IO.DirectoryInfo
                        For Each di In items
                            Try
                                GetDirectories_Recursive(di, Filter, FileFilter, Attributes, table, IncludeFiles, False, BasePath)
                            Catch ex As Exception
                            End Try
                        Next
                    End If
                End If
            Catch ex As Exception
                Throw ex
            End Try
        End Sub
        Private Sub PopulateDirectoryTable(ByRef x As Array, ByRef Table As Data.DataTable, ByVal Filter As String, ByVal BasePath As String)
            If Not x Is Nothing AndAlso x.Length > 0 Then
                Dim di As IO.DirectoryInfo
                For Each di In x
                    Dim canLoad As Boolean = True
                    If Not Filter Is Nothing AndAlso Filter.Length > 0 Then
                        Dim filterCount As Integer = 0
                        'BE SELECTIVE ABOUT THE ITEMS WE LOAD
                        canLoad = False
                        If Filter.ToUpper.IndexOf("A") >= 0 AndAlso ((CDbl(di.Attributes And IO.FileAttributes.Archive)) = IO.FileAttributes.Archive) Then
                            filterCount += 1
                        End If
                        If Filter.ToUpper.IndexOf("C") >= 0 AndAlso ((CDbl(di.Attributes And IO.FileAttributes.Compressed)) = IO.FileAttributes.Compressed) Then
                            filterCount += 1
                        End If
                        If Filter.ToUpper.IndexOf("D") >= 0 AndAlso ((CDbl(di.Attributes And IO.FileAttributes.Directory)) = IO.FileAttributes.Directory) Then
                            filterCount += 1
                        End If
                        If Filter.ToUpper.IndexOf("E") >= 0 AndAlso ((CDbl(di.Attributes And IO.FileAttributes.Encrypted)) = IO.FileAttributes.Encrypted) Then
                            filterCount += 1
                        End If
                        If Filter.ToUpper.IndexOf("H") >= 0 AndAlso ((CDbl(di.Attributes And IO.FileAttributes.Hidden)) = IO.FileAttributes.Hidden) Then
                            filterCount += 1
                        End If
                        If Filter.ToUpper.IndexOf("O") >= 0 AndAlso ((CDbl(di.Attributes And IO.FileAttributes.Offline)) = IO.FileAttributes.Offline) Then
                            filterCount += 1
                        End If
                        If Filter.ToUpper.IndexOf("R") >= 0 AndAlso ((CDbl(di.Attributes And IO.FileAttributes.ReadOnly)) = IO.FileAttributes.ReadOnly) Then
                            filterCount += 1
                        End If
                        If Filter.ToUpper.IndexOf("S") >= 0 AndAlso ((CDbl(di.Attributes And IO.FileAttributes.System)) = IO.FileAttributes.System) Then
                            filterCount += 1
                        End If
                        If Filter.ToUpper.IndexOf("T") >= 0 AndAlso ((CDbl(di.Attributes And IO.FileAttributes.Temporary)) = IO.FileAttributes.Temporary) Then
                            filterCount += 1
                        End If
                        If filterCount = Filter.Length Then
                            canLoad = True
                        End If
                    End If
                    If canLoad Then
                        Dim dr As Data.DataRow = Table.NewRow
                        dr("CreationTime") = di.CreationTime
                        dr("CreationTimeUTC") = di.CreationTimeUtc
                        dr("Exists") = di.Exists
                        dr("Extension") = di.Extension
                        dr("FullName") = di.FullName
                        dr("Name") = di.Name
                        If Not di.Extension Is Nothing AndAlso di.Extension.Length > 0 AndAlso Not di.Name Is Nothing Then
                            dr("NameOnly") = di.Name.Substring(0, di.Name.Length - di.Extension.Length)
                        Else
                            dr("NameOnly") = di.Name
                        End If
                        If Not di.Parent Is Nothing Then
                            dr("ParentPath") = di.Parent.FullName
                            dr("ParentName") = di.Parent.Name
                        End If
                        If Not di.Root Is Nothing Then
                            dr("RootPath") = di.Root.FullName
                            dr("RootName") = di.Root.Name
                        End If
                        If Not BasePath Is Nothing AndAlso di.FullName.StartsWith(BasePath) Then
                            dr("BasePath") = di.FullName.Substring(BasePath.Length)
                        End If
                        dr("LastAccessTime") = di.LastAccessTime
                        dr("LastAccessTimeUTC") = di.LastAccessTimeUtc
                        dr("LastWriteTime") = di.LastWriteTime
                        dr("LastWriteTimeUTC") = di.LastWriteTimeUtc

                        If ((CDbl(di.Attributes And IO.FileAttributes.Archive)) = IO.FileAttributes.Archive) Then
                            dr("isArchived") = True
                        Else
                            dr("isArchived") = False
                        End If
                        If ((CDbl(di.Attributes And IO.FileAttributes.Compressed)) = IO.FileAttributes.Compressed) Then
                            dr("isCompressed") = True
                        Else
                            dr("isCompressed") = False
                        End If
                        If ((CDbl(di.Attributes And IO.FileAttributes.Device)) = IO.FileAttributes.Device) Then
                            dr("isDevice") = True
                        Else
                            dr("isDevice") = False
                        End If
                        If ((CDbl(di.Attributes And IO.FileAttributes.Directory)) = IO.FileAttributes.Directory) Then
                            dr("isDirectory") = True
                        Else
                            dr("isDirectory") = False
                        End If
                        If ((CDbl(di.Attributes And IO.FileAttributes.Encrypted)) = IO.FileAttributes.Encrypted) Then
                            dr("isEncrypted") = True
                        Else
                            dr("isEncrypted") = False
                        End If
                        If ((CDbl(di.Attributes And IO.FileAttributes.Hidden)) = IO.FileAttributes.Hidden) Then
                            dr("isHidden") = True
                        Else
                            dr("isHidden") = False
                        End If
                        If ((CDbl(di.Attributes And IO.FileAttributes.Normal)) = IO.FileAttributes.Normal) Then
                            dr("isNormal") = True
                        Else
                            dr("isNormal") = False
                        End If
                        If ((CDbl(di.Attributes And IO.FileAttributes.NotContentIndexed)) = IO.FileAttributes.NotContentIndexed) Then
                            dr("isNotContentIndexed") = True
                        Else
                            dr("isNotContentIndexed") = False
                        End If
                        If ((CDbl(di.Attributes And IO.FileAttributes.Offline)) = IO.FileAttributes.Offline) Then
                            dr("isOffline") = True
                        Else
                            dr("isOffline") = False
                        End If
                        If ((CDbl(di.Attributes And IO.FileAttributes.ReadOnly)) = IO.FileAttributes.ReadOnly) Then
                            dr("isReadOnly") = True
                        Else
                            dr("isReadOnly") = False
                        End If
                        If ((CDbl(di.Attributes And IO.FileAttributes.System)) = IO.FileAttributes.System) Then
                            dr("isSystem") = True
                        Else
                            dr("isSystem") = False
                        End If
                        If ((CDbl(di.Attributes And IO.FileAttributes.Temporary)) = IO.FileAttributes.Temporary) Then
                            dr("isTemporary") = True
                        Else
                            dr("isTemporary") = False
                        End If
                        Table.Rows.Add(dr)
                    End If
                Next
            End If
        End Sub
        Private Sub PopulateFileTable(ByRef files As ArrayList, ByRef Table As Data.DataTable, ByVal Filter As String, ByVal BasePath As String)
            'VERSION: 2.0 Added Multi-Filter for Get Directory
            'VERSION: 2.0 Added Size for columns
            Filter = Filter.Replace("D", "") 'STRIP OUT DIRECTORY
            Filter = Filter.Replace("F", "") 'STRIP OUT FILE
            If Not files Is Nothing AndAlso files.Count > 0 Then
                Dim di As IO.FileInfo
                For Each di In files
                    Dim canLoad As Boolean = True
                    If Not Filter Is Nothing AndAlso Filter.Length > 0 Then
                        Dim filterCount As Integer = 0
                        'BE SELECTIVE ABOUT THE ITEMS WE LOAD
                        canLoad = False
                        If Filter.ToUpper.IndexOf("A") >= 0 AndAlso ((CDbl(di.Attributes And IO.FileAttributes.Archive)) = IO.FileAttributes.Archive) Then
                            filterCount += 1
                        End If
                        If Filter.ToUpper.IndexOf("C") >= 0 AndAlso ((CDbl(di.Attributes And IO.FileAttributes.Compressed)) = IO.FileAttributes.Compressed) Then
                            filterCount += 1
                        End If
                        If Filter.ToUpper.IndexOf("E") >= 0 AndAlso ((CDbl(di.Attributes And IO.FileAttributes.Encrypted)) = IO.FileAttributes.Encrypted) Then
                            filterCount += 1
                        End If
                        If Filter.ToUpper.IndexOf("H") >= 0 AndAlso ((CDbl(di.Attributes And IO.FileAttributes.Hidden)) = IO.FileAttributes.Hidden) Then
                            filterCount += 1
                        End If
                        If Filter.ToUpper.IndexOf("O") >= 0 AndAlso ((CDbl(di.Attributes And IO.FileAttributes.Offline)) = IO.FileAttributes.Offline) Then
                            filterCount += 1
                        End If
                        If Filter.ToUpper.IndexOf("R") >= 0 AndAlso ((CDbl(di.Attributes And IO.FileAttributes.ReadOnly)) = IO.FileAttributes.ReadOnly) Then
                            filterCount += 1
                        End If
                        If Filter.ToUpper.IndexOf("S") >= 0 AndAlso ((CDbl(di.Attributes And IO.FileAttributes.System)) = IO.FileAttributes.System) Then
                            filterCount += 1
                        End If
                        If Filter.ToUpper.IndexOf("T") >= 0 AndAlso ((CDbl(di.Attributes And IO.FileAttributes.Temporary)) = IO.FileAttributes.Temporary) Then
                            filterCount += 1
                        End If
                        If filterCount = Filter.Length Then
                            canLoad = True
                        End If
                    End If
                    If canLoad Then
                        Dim dr As Data.DataRow = Table.NewRow
                        dr("CreationTime") = di.CreationTime
                        dr("CreationTimeUTC") = di.CreationTimeUtc
                        dr("Exists") = di.Exists
                        dr("Extension") = di.Extension
                        dr("FullName") = di.FullName
                        dr("Name") = di.Name
                        If Not di.Extension Is Nothing AndAlso di.Extension.Length > 0 AndAlso Not di.Name Is Nothing Then
                            dr("NameOnly") = di.Name.Substring(0, di.Name.Length - di.Extension.Length)
                        Else
                            dr("NameOnly") = di.Name
                        End If
                        If Not di.Directory Is Nothing Then
                            dr("ParentPath") = di.Directory.FullName
                            dr("ParentName") = di.Directory.Name
                            If Not di.Directory.Parent Is Nothing Then
                                dr("ParentParentPath") = di.Directory.Parent.FullName
                                dr("ParentParentName") = di.Directory.Parent.Name
                            End If
                        End If
                        If Not di.Directory Is Nothing AndAlso Not di.Directory.Root Is Nothing Then
                            dr("RootPath") = di.Directory.Root.FullName
                            dr("RootName") = di.Directory.Root.Name
                        End If
                        If Not BasePath Is Nothing AndAlso di.FullName.StartsWith(BasePath) Then
                            dr("BasePath") = di.FullName.Substring(BasePath.Length)
                        End If
                        dr("LastAccessTime") = di.LastAccessTime
                        dr("LastAccessTimeUTC") = di.LastAccessTimeUtc
                        dr("LastWriteTime") = di.LastWriteTime
                        dr("LastWriteTimeUTC") = di.LastWriteTimeUtc
                        dr("Size") = di.Length

                        If ((CDbl(di.Attributes And IO.FileAttributes.Archive)) = IO.FileAttributes.Archive) Then
                            dr("isArchived") = True
                        Else
                            dr("isArchived") = False
                        End If
                        If ((CDbl(di.Attributes And IO.FileAttributes.Compressed)) = IO.FileAttributes.Compressed) Then
                            dr("isCompressed") = True
                        Else
                            dr("isCompressed") = False
                        End If
                        If ((CDbl(di.Attributes And IO.FileAttributes.Device)) = IO.FileAttributes.Device) Then
                            dr("isDevice") = True
                        Else
                            dr("isDevice") = False
                        End If

                        dr("isDirectory") = False

                        If ((CDbl(di.Attributes And IO.FileAttributes.Encrypted)) = IO.FileAttributes.Encrypted) Then
                            dr("isEncrypted") = True
                        Else
                            dr("isEncrypted") = False
                        End If
                        If ((CDbl(di.Attributes And IO.FileAttributes.Hidden)) = IO.FileAttributes.Hidden) Then
                            dr("isHidden") = True
                        Else
                            dr("isHidden") = False
                        End If
                        If ((CDbl(di.Attributes And IO.FileAttributes.Normal)) = IO.FileAttributes.Normal) Then
                            dr("isNormal") = True
                        Else
                            dr("isNormal") = False
                        End If
                        If ((CDbl(di.Attributes And IO.FileAttributes.NotContentIndexed)) = IO.FileAttributes.NotContentIndexed) Then
                            dr("isNotContentIndexed") = True
                        Else
                            dr("isNotContentIndexed") = False
                        End If
                        If ((CDbl(di.Attributes And IO.FileAttributes.Offline)) = IO.FileAttributes.Offline) Then
                            dr("isOffline") = True
                        Else
                            dr("isOffline") = False
                        End If
                        If ((CDbl(di.Attributes And IO.FileAttributes.ReadOnly)) = IO.FileAttributes.ReadOnly) Then
                            dr("isReadOnly") = True
                        Else
                            dr("isReadOnly") = False
                        End If
                        If ((CDbl(di.Attributes And IO.FileAttributes.System)) = IO.FileAttributes.System) Then
                            dr("isSystem") = True
                        Else
                            dr("isSystem") = False
                        End If
                        If ((CDbl(di.Attributes And IO.FileAttributes.Temporary)) = IO.FileAttributes.Temporary) Then
                            dr("isTemporary") = True
                        Else
                            dr("isTemporary") = False
                        End If

                        Table.Rows.Add(dr)
                    End If
                Next
            End If
        End Sub
        Private Shared Function GetSystemFileDatatable(ByVal Name As String) As DataTable
            Dim dt As New DataTable(Name)
            dt.Columns.Add(New DataColumn("CreationTime", GetType(System.DateTime)))
            dt.Columns.Add(New DataColumn("CreationTimeUTC", GetType(System.DateTime)))

            dt.Columns.Add(New DataColumn("isArchived", GetType(System.Boolean)))
            dt.Columns.Add(New DataColumn("isCompressed", GetType(System.Boolean)))
            dt.Columns.Add(New DataColumn("isDevice", GetType(System.Boolean)))
            dt.Columns.Add(New DataColumn("isDirectory", GetType(System.Boolean)))
            dt.Columns.Add(New DataColumn("isEncrypted", GetType(System.Boolean)))
            dt.Columns.Add(New DataColumn("isHidden", GetType(System.Boolean)))
            dt.Columns.Add(New DataColumn("isNormal", GetType(System.Boolean)))
            dt.Columns.Add(New DataColumn("isNotContentIndexed", GetType(System.Boolean)))
            dt.Columns.Add(New DataColumn("isOffline", GetType(System.Boolean)))
            dt.Columns.Add(New DataColumn("isReadOnly", GetType(System.Boolean)))
            dt.Columns.Add(New DataColumn("isSystem", GetType(System.Boolean)))
            dt.Columns.Add(New DataColumn("isTemporary", GetType(System.Boolean)))

            dt.Columns.Add(New DataColumn("Exists", GetType(System.Boolean)))
            dt.Columns.Add(New DataColumn("Extension", GetType(System.String)))
            dt.Columns.Add(New DataColumn("FullName", GetType(System.String)))
            dt.Columns.Add(New DataColumn("Name", GetType(System.String)))
            dt.Columns.Add(New DataColumn("NameOnly", GetType(System.String)))
            dt.Columns.Add(New DataColumn("ParentPath", GetType(System.String)))
            dt.Columns.Add(New DataColumn("ParentName", GetType(System.String)))
            dt.Columns.Add(New DataColumn("ParentParentPath", GetType(System.String)))
            dt.Columns.Add(New DataColumn("ParentParentName", GetType(System.String)))
            dt.Columns.Add(New DataColumn("RootPath", GetType(System.String)))
            dt.Columns.Add(New DataColumn("RootName", GetType(System.String)))

            dt.Columns.Add(New DataColumn("BasePath", GetType(System.String)))

            dt.Columns.Add(New DataColumn("LastAccessTime", GetType(System.DateTime)))
            dt.Columns.Add(New DataColumn("LastAccessTimeUTC", GetType(System.DateTime)))
            dt.Columns.Add(New DataColumn("LastWriteTime", GetType(System.DateTime)))
            dt.Columns.Add(New DataColumn("LastWriteTimeUTC", GetType(System.DateTime)))

            'VERSION: 2.0 Added Size for columns
            dt.Columns.Add(New DataColumn("Size", GetType(System.Int64)))

            Return dt
        End Function

        Public Shared Function IsSystemFileDatatable(ByVal DT As DataTable) As Boolean
            Dim dtFiles As DataTable = GetSystemFileDatatable("CHECK")
            Dim isFiles As Boolean = True

            If DT.Columns.Count <> dtFiles.Columns.Count Then
                isFiles = False
            Else
                For Each dcFiles As DataColumn In dtFiles.Columns
                    If Not DT.Columns.Contains(dcFiles.ColumnName) Then
                        isFiles = False
                        Exit For
                    End If
                Next
            End If

            Return isFiles
        End Function
    End Class
End Namespace
