'<LICENSE>
'   
'       Open Web Studio - http://www.OpenWebStudio.com
'       Copyright (c) 2007-2009
'       by R2Integrated Inc. http://www.R2integrated.com
'   
'       Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
'       documentation files (the "Software"), to deal in the Software without restriction, including without limitation
'       the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and
'       to permit persons to whom the Software is furnished to do so, subject to the following conditions:
'   
'       The above copyright notice and this permission notice shall be included in all copies or substantial portions of
'       the Software.
'   
'       This Software and associated documentation files are subject to the terms and conditions of the Open Web Studio
'       End User License Agreement and version 2 of the GNU General Public License.
'   
'       THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
'       TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
'       THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
'       CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
'       DEALINGS IN THE SOFTWARE.
'     
'</LICENSE>
Imports System
Imports EnvDTE
Imports EnvDTE80
Imports System.Diagnostics
Imports System.Xml
Imports System.Collections

Public Class SmartConfig
    Private DocPath As String
    Private Doc As XmlDocument
    Public Paths As ArrayList
    Public License As String
    Public Versions As ArrayList
    Public Commands As ArrayList
    Public Structure Version
        Public isReadonly As Boolean
        Public Name As String
        Public Major As Integer
        Public Minor As Integer
        Public Build As Integer
        Public Revision As Integer
        Public Node As Xml.XmlNode
        Public ReadOnly Property ToString() As String
            Get
                Return Major.ToString("0#") & "." & Minor.ToString("0#") & "." & Build.ToString("0#") & "." & Revision.ToString("0#")
            End Get
        End Property
    End Structure
    Public Structure Path
        Public Name As String
        Public From As String
        Public [Select] As String
        Public Where As String
        Public Insert As String
    End Structure
    Sub New(ByVal path As String)
        Try
            Dim fio As New IO.FileInfo(path)
            If fio.Exists Then
                Doc = (New XmlDocument)
                Doc.Load(path)
                If Not Doc Is Nothing Then
                    Me.DocPath = path
                    LoadLicense(Doc)
                    LoadPaths(Doc)
                    LoadVersions(Doc)
                    LoadCommands(Doc)
                End If
            End If
        Catch ex As Exception
        End Try
    End Sub
    Public Sub Increment(ByVal bMajor As Boolean, ByVal bMinor As Boolean, ByVal bBuild As Boolean, ByVal bRevision As Boolean)
        If Not Me.Versions Is Nothing AndAlso Me.Versions.Count > 0 Then
            Dim bDirty As Boolean = False
            Dim v As Version
            For Each v In Me.Versions
                If Not v.isReadonly Then
                    bDirty = True
                    If bMajor Then
                        v.Major += 1
                    End If
                    If bMinor Then
                        v.Minor += 1
                    End If
                    If bBuild Then
                        v.Build += 1
                    End If
                    If bRevision Then
                        v.Revision += 1
                    End If

                    v.Node.SelectSingleNode("major").InnerText = v.Major
                    v.Node.SelectSingleNode("minor").InnerText = v.Minor
                    v.Node.SelectSingleNode("build").InnerText = v.Build
                    v.Node.SelectSingleNode("revision").InnerText = v.Revision

                End If
            Next
            If bDirty Then
                Doc.Save(DocPath)
            End If

        End If
    End Sub
    Private Sub LoadCommands(ByVal xmlD As Xml.XmlDocument)
        Commands = New ArrayList
        Dim nodes As Xml.XmlNodeList = xmlD.SelectNodes("macro/commands")
        If Not nodes Is Nothing AndAlso nodes.Count > 0 Then
            Dim n As XmlNode
            For Each n In nodes
                If Not n.InnerText Is Nothing AndAlso n.InnerText.Length > 0 Then
                    Commands.Add(n.InnerText)
                End If
            Next
        End If
    End Sub
    Private Sub LoadLicense(ByVal xmlD As Xml.XmlDocument)
        Dim nodes As Xml.XmlNodeList = xmlD.SelectNodes("macro/license")
        If Not nodes Is Nothing AndAlso nodes.Count > 0 Then
            Dim n As XmlNode
            For Each n In nodes
                If Not n.InnerText Is Nothing AndAlso n.InnerText.Length > 0 Then
                    Me.License = n.InnerText
                End If
            Next
        End If
    End Sub
    Private Sub LoadPaths(ByVal xmlD As XmlDocument)
        Me.Paths = New ArrayList
        Dim nodes As Xml.XmlNodeList = xmlD.SelectNodes("macro/paths/path")
        If Not nodes Is Nothing AndAlso nodes.Count > 0 Then
            Dim n As XmlNode
            For Each n In nodes
                Dim p As New Path
                Dim fNode As XmlNode = n.SelectSingleNode("from")
                Dim sNode As XmlNode = n.SelectSingleNode("select")
                Dim iNode As XmlNode = n.SelectSingleNode("insert")
                Dim wNode As XmlNode = n.SelectSingleNode("where")
                If Not fNode Is Nothing Then
                    p.From = fNode.InnerText
                End If
                If Not sNode Is Nothing Then
                    p.Select = sNode.InnerText
                End If
                If Not iNode Is Nothing Then
                    p.Insert = iNode.InnerText
                End If
                If Not wNode Is Nothing Then
                    p.Where = wNode.InnerText
                End If
                If Not n.Attributes("name") Is Nothing Then
                    p.Name = n.Attributes("name").Value
                End If
                Me.Paths.Add(p)
            Next
        End If
    End Sub
    Private Sub LoadVersions(ByVal xmlD As XmlDocument)
        Me.Versions = New ArrayList
        Dim nodes As Xml.XmlNodeList = xmlD.SelectNodes("macro/versions/version")
        If Not nodes Is Nothing AndAlso nodes.Count > 0 Then
            Dim n As XmlNode
            For Each n In nodes
                Dim p As New Version
                p.Node = n
                Dim mNode As XmlNode = n.SelectSingleNode("major")
                Dim iNode As XmlNode = n.SelectSingleNode("minor")
                Dim bNode As XmlNode = n.SelectSingleNode("build")
                Dim rNode As XmlNode = n.SelectSingleNode("revision")
                If Not mNode Is Nothing Then
                    p.Major = mNode.InnerText
                End If
                If Not iNode Is Nothing Then
                    p.Minor = iNode.InnerText
                End If
                If Not bNode Is Nothing Then
                    p.Build = bNode.InnerText
                End If
                If Not rNode Is Nothing Then
                    p.Revision = rNode.InnerText
                End If
                If Not n.Attributes("name") Is Nothing Then
                    p.Name = n.Attributes("name").Value
                End If
                If Not n.Attributes("readonly") Is Nothing Then
                    p.isReadonly = CBool(n.Attributes("readonly").Value)
                End If
                Me.Versions.Add(p)
            Next
        End If
    End Sub
End Class




Public Module Module1
    Sub ConfigHandler_License()
        ConfigHandler(True, False, False, False)
    End Sub
    Sub ConfigHandler_increment()
        ConfigHandler(False, True, False, False)
    End Sub
    Sub ConfigHandler_Push()
        ConfigHandler(False, False, True, False)
    End Sub
    Sub ConfigHandler_Build()
        ConfigHandler(False, False, False, True)
    End Sub
    Sub ConfigHandler_IncrementAndBuild()
        ConfigHandler(False, True, False, True)
    End Sub
    Sub ConfigHandler(ByVal license As Boolean, ByVal increment As Boolean, ByVal push As Boolean, ByVal build As Boolean)
        Dim c As New SmartConfig(DTE.Solution.FindProjectItem("Macros.config").FileNames(1))
        If license Then
            If Not c.License Is Nothing Then
                AppendLicenseAll(c.License)
            End If
        End If
        If increment Then
            c.Increment(False, False, False, True)
            VersionSolutionAssemblies(c)
        End If
        If build Then
            Dim cmd As String
            For Each cmd In c.Commands
                DTE.ExecuteCommand(cmd) '"Tools.ExternalCommand7")
            Next
        End If
        If push Then
            Dim p As SmartConfig.Path
            For Each p In c.Paths
                If Not p.From Is Nothing Then
                    Dim target As String = ""
                    If Not p.Insert Is Nothing Then
                        target = p.Insert
                        If Not target.EndsWith("/") AndAlso Not target.EndsWith("\") Then
                            target &= "\"
                        End If
                    End If
                    Dim pF As New IO.DirectoryInfo(p.From)
                    Dim filters As String()
                    If pF.Exists Then
                        If Not p.Select Is Nothing Then
                            filters = p.Select.Split(",")
                        Else
                            filters = "*.*".Split(",")
                        End If
                    End If

                    Dim filter As String = ""
                    For Each filter In filters
                        Dim fS() As IO.FileInfo = Nothing
                        fS = pF.GetFiles(filter)
                        If Not fS Is Nothing AndAlso fS.Length > 0 Then
                            Dim exclude As String() = Nothing
                            If Not p.Where Is Nothing AndAlso p.Where.Length > 0 Then
                                exclude = p.Where.Split(","c)
                            End If
                            Dim f As IO.FileInfo
                            For Each f In fS
                                If checkFile(f, exclude) Then
                                    f.CopyTo(target & f.Name, True)
                                    AppendDebug("Push", "Copied: " & f.Name & " to " & target & vbCrLf)
                                End If
                            Next
                        End If
                    Next
                End If
            Next
        End If
    End Sub
    Private Function checkFile(ByVal f As IO.FileInfo, ByVal s() As String) As Boolean
        If Not s Is Nothing AndAlso s.Length > 0 Then
            Dim v As String
            For Each v In s
                Dim notb As Boolean = False
                If v.StartsWith("!") Then
                    notb = True
                    v = v.Remove(0, 1)
                End If
                If notb Then
                    If v.ToLower = f.Name.ToLower Then
                        Return False
                    End If
                End If
            Next
        End If
        Return True
    End Function


    Sub Push()
        Dim source As New IO.DirectoryInfo("D:\Work\Temporary\Restructure\Build")
        Dim target As New IO.DirectoryInfo("D:\Work\Current\Websites\DNN482\Website\Bin")
        Dim badName As New Collections.ArrayList
        Dim badExt As New Collections.ArrayList
        badName.Add("dotnetnuke.dll")
        badName.Add("newtonsoft.json.dll")
        badName.Add("sharpziplib.dll")
        badExt.Add(".tmp")
        If source.Exists AndAlso target.Exists Then
            Dim sourceFiles As IO.FileInfo() = source.GetFiles("*.*")
            Dim sourceFile As IO.FileInfo
            For Each sourceFile In sourceFiles
                If Not badName.Contains(sourceFile.Name.ToLower) Then
                    If Not badExt.Contains(sourceFile.Extension) Then
                        sourceFile.CopyTo(target.FullName & "\" & sourceFile.Name, True)
                    End If
                End If
            Next
        End If
    End Sub
    Sub AppendDebug(ByVal Name As String, ByVal Value As String, Optional ByVal IsFirst As Boolean = False)
        Dim outputWindow As Window2 = DTE.Windows.Item("Output")
        If outputWindow Is Nothing Then
            DTE.ExecuteCommand("Debug.Output")
            outputWindow = DTE.Windows.Item("Output")
        End If
        If Not outputWindow Is Nothing Then
            outputWindow = DTE.Windows.Item("Output")

            outputWindow.Visible = True
            Dim outputPane As OutputWindowPane
            Dim found As Boolean
            If CType(outputWindow.Object, OutputWindow).ActivePane.Name = Name Then
                found = True
                outputPane = outputWindow.Object.ActivePane
            Else
                For Each outputPane In outputWindow.Object.OutputWindowPanes
                    If outputPane.Name = Name Then
                        found = True
                        Exit For
                    End If
                Next
            End If
            If Not found Then
                outputPane = outputWindow.Object.OutputWindowPanes.Add(Name)
            End If
            If IsFirst Then
                outputPane.Clear()
            End If
            outputPane.OutputString(Value)
        End If
    End Sub
    Sub AppendLicenseProject(ByVal License As String, Optional ByVal prj As Project = Nothing)
        If prj Is Nothing AndAlso DTE.SelectedItems.Count > 0 Then
            prj = DTE.SelectedItems.Item(1).Project
        End If
        If Not prj Is Nothing Then
            Dim prjI As EnvDTE.ProjectItem
            For Each prjI In prj.ProjectItems
                If prjI.Name.EndsWith(".vb") Then
                    prjI.Open()
                    AppendLicense(License, prjI.Document)
                    prjI.Document.Close()
                End If
                If Not prjI.ProjectItems Is Nothing AndAlso prjI.ProjectItems.Count > 0 Then
                    AppendLicenseAll(License, prjI)
                End If
            Next
        End If
    End Sub
    Sub AppendLicenseAll(ByVal License As String, Optional ByVal pPrjItem As ProjectItem = Nothing)
        Dim prj As EnvDTE.Project
        If pPrjItem Is Nothing Then
            For Each prj In DTE.Solution.Projects
                AppendLicenseProject(prj)
            Next
        Else
            Dim prjI As EnvDTE.ProjectItem
            For Each prjI In pPrjItem.ProjectItems
                Try
                    If prjI.Name.EndsWith(".vb") Then
                        prjI.Open()
                        AppendLicense(License, prjI.Document)
                        prjI.Document.Close()
                    End If
                Catch ex As Exception
                    AppendDebug("License", "License Failure (Project Item): " & ex.Message & vbCrLf)
                End Try
            Next
        End If
    End Sub
    Sub AppendLicense(ByVal License As String, Optional ByVal Doc As Document = Nothing)
        Try
            Dim fStr As String
            If Doc Is Nothing Then
                Doc = DTE.ActiveDocument
            End If
            AppendDebug("License", "Added/Updated License for: " & Doc.FullName & vbCrLf)
            Dim fr As New IO.StringReader(License)
            Doc.Selection.GotoLine(1, False)
            Doc.Selection.StartOfLine(vsStartOfLineOptions.vsStartOfLineOptionsFirstColumn)
            'FIRST CHECK TO SEE IF THE FIRST LINE IS '<LICENSE>
            Doc.Selection.SelectLine()
            If Doc.Selection.Text.StartsWith("'<LICENSE>") Then
                While Doc.Selection.Text.StartsWith("'")
                    Dim str As String = Doc.Selection.Text
                    Doc.Selection.Delete()
                    If str.StartsWith("'</LICENSE>") Then
                        Exit While
                    End If
                    Doc.Selection.SelectLine()
                End While
            End If
            Doc.Selection.GotoLine(1, False)
            Doc.Selection.StartOfLine(vsStartOfLineOptions.vsStartOfLineOptionsFirstColumn)
            Doc.Selection.Text = "'<LICENSE>"
            Doc.Selection.NewLine()
            fStr = fr.ReadLine
            While Not fStr Is Nothing
                Doc.Selection.Text = "'" & vbTab & fStr
                Doc.Selection.NewLine()
                fStr = fr.ReadLine
            End While
            Doc.Selection.Text = "'</LICENSE>"
            Doc.Selection.NewLine()
            fStr = fr.ReadToEnd
            fr.Close()
            Doc.Save()
        Catch ex As Exception
            AppendDebug("License", "License Failure: Please verify that the solution contains a License.txt File" & vbCrLf)
        End Try
    End Sub

    Function IncreaseVersion(ByVal Version As String) As String
        If Version Is Nothing OrElse Version.Length = 0 Then
            Version = "1.0.0.0"
        End If
        Dim str() As String = Version.Split(".")
        If str.Length <> 4 Then
            str = New String() {"1", "0", "0", "0"}
        End If
        Dim buildnumber As Integer = CType(str(3), Int32)
        buildnumber += 1
        str(3) = buildnumber
        Return str(0) & "." & str(1) & "." & str(2) & "." & str(3)
    End Function
    Sub UpdateVersionFile(ByVal Fio As IO.FileInfo, ByVal Value As String)
        If Not Fio.IsReadOnly Then
            Dim fs As IO.FileStream = Fio.Open(IO.FileMode.Truncate)
            If fs.CanWrite Then
                Dim fw As New IO.StreamWriter(fs)
                fw.Write(Value)
                fw.Close()
                Try
                    fs.Close()
                    fs = Nothing
                Catch ex As Exception
                End Try
            End If
        End If
    End Sub
    Sub GetVersions(ByVal c As SmartConfig, ByRef assemblyVersion As String, ByRef fileVersion As String)
        Dim fio As IO.FileInfo

        Dim v As SmartConfig.Version
        For Each v In c.Versions
            If v.Name.ToLower = "assembly" Then
                assemblyVersion = v.ToString
            Else
                fileVersion = v.ToString
            End If
        Next
    End Sub
    Sub SetVersions(ByVal c As SmartConfig)
        Dim fio As IO.FileInfo

        Dim assemblyVersion As String
        Dim fileVersion As String

        Dim v As SmartConfig.Version
        For Each v In c.Versions
            If v.Name.ToLower = "assembly" Then
                assemblyVersion = v.ToString
            Else
                fileVersion = v.ToString
            End If
            UpdateVersions(fileVersion, assemblyVersion)
        Next
    End Sub
    Sub VersionSolutionAssemblies(ByVal c As SmartConfig)
        Dim FileVersion As String
        Dim AssemblyVersion As String
        GetVersions(c, FileVersion, AssemblyVersion)
        AppendDebug("Version", "-------------------------------------------------------------------------------------------" & vbCrLf)
        AppendDebug("Version", "Solution Assembly Build Versions for " & Now.ToString & vbCrLf, True)
        AppendDebug("Version", "-------------------------------------------------------------------------------------------" & vbCrLf)
        AppendDebug("Version", "Assembly Version: " & AssemblyVersion & vbCrLf)
        AppendDebug("Version", "File Version: " & FileVersion & vbCrLf)
        AppendDebug("Version", "-------------------------------------------------------------------------------------------" & vbCrLf)
        UpdateVersions(FileVersion, AssemblyVersion)
    End Sub
    Sub UpdateVersions(ByVal FileVersion As String, ByVal AssemblyVersion As String, Optional ByVal pPrjItem As ProjectItem = Nothing)
        Dim fstr As String
        Dim prjI As ProjectItem
        Dim ar As Collections.ArrayList = GetProjectItems("AssemblyInfo.vb")
        For Each prjI In ar
            prjI.Open()
            UpdateVersion(FileVersion, AssemblyVersion, prjI.Document)
            prjI.Document.Close()
        Next
    End Sub
    Private Function GetProjectItems(ByVal Name As String) As Collections.ArrayList
        Dim ar As New Collections.ArrayList
        Dim prj As EnvDTE.Project
        For Each prj In DTE.Solution.Projects
            Dim prjI As EnvDTE.ProjectItem
            Dim pX As ProjectItems
            If prj.ProjectItems Is Nothing Then
                If Not prj.SubProject Is Nothing AndAlso Not prj.SubProject.ProjectItems Is Nothing Then
                    pX = prj.SubProject.ProjectItems
                End If
            Else
                pX = prj.ProjectItems
            End If
            If Not pX Is Nothing AndAlso pX.Count > 0 Then
                For Each prjI In pX
                    Dim pX2 As ProjectItems
                    If prjI.ProjectItems Is Nothing Then
                        If Not prjI.SubProject Is Nothing Then
                            pX2 = prjI.SubProject.ProjectItems
                        End If
                    Else
                        pX2 = prjI.ProjectItems
                    End If
                    If Not pX2 Is Nothing AndAlso pX2.Count > 0 Then
                        GetProjectItem(prjI, Name, ar)
                    Else
                        If prjI.Name.ToLower = Name.ToLower Then
                            ar.Add(prjI)
                        End If
                    End If
                Next
            End If
        Next
        Return ar
    End Function
    Private Sub GetProjectItem(ByVal prj As ProjectItem, ByVal Name As String, ByVal Items As Collections.ArrayList)
        Dim prjI As EnvDTE.ProjectItem
        Dim pX As ProjectItems
        If prj.ProjectItems Is Nothing Then
            If Not prj.SubProject Is Nothing AndAlso Not prj.SubProject.ProjectItems Is Nothing Then
                pX = prj.SubProject.ProjectItems
            End If
        Else
            pX = prj.ProjectItems
        End If

        If Not pX Is Nothing AndAlso pX.Count > 0 Then
            For Each prjI In pX
                Dim pX2 As ProjectItems
                If prjI.ProjectItems Is Nothing Then
                    If Not prjI.SubProject Is Nothing Then
                        pX2 = prjI.SubProject.ProjectItems
                    End If
                Else
                    pX2 = prjI.ProjectItems
                End If
                If Not pX2 Is Nothing AndAlso pX2.Count > 0 Then
                    GetProjectItem(prjI, Name, Items)
                Else
                    If prjI.Name.ToLower = Name.ToLower Then
                        Items.Add(prjI)
                    End If
                End If
            Next
        End If
    End Sub
    Sub UpdateVersionProject(ByVal FileVersion As String, ByVal AssemblyVersion As String, Optional ByVal prj As Project = Nothing)
        If prj Is Nothing AndAlso DTE.SelectedItems.Count > 0 Then
            prj = DTE.SelectedItems.Item(1).Project
        End If
        If Not prj Is Nothing Then
            Dim prjI As EnvDTE.ProjectItem
            For Each prjI In prj.ProjectItems
                If prjI.Name.EndsWith("AssemblyInfo.vb") Then
                    prjI.Open()
                    UpdateVersion(FileVersion, AssemblyVersion, prjI.Document)
                    prjI.Document.Close()
                End If
                If Not prjI.ProjectItems Is Nothing AndAlso prjI.ProjectItems.Count > 0 Then
                    UpdateVersions(FileVersion, AssemblyVersion, prjI)
                End If
            Next
            If Not prj.ProjectItems Is Nothing AndAlso prj.ProjectItems.Count > 0 Then
                UpdateVersionProject(FileVersion, AssemblyVersion, prj)
            End If
        End If
    End Sub
    Sub UpdateVersion(ByVal FileVersion As String, ByVal AssemblyVersion As String, Optional ByVal Doc As Document = Nothing)
        Try
            Dim fStr As String
            If Doc Is Nothing Then
                Doc = DTE.ActiveDocument
            End If
            AppendDebug("Version", "Added/Updated Version for: " & Doc.FullName & vbCrLf)

            'ASSEMBLY VERSION
            '<Assembly: AssemblyVersion("2.0.3.*")> 
            '<Assembly: AssemblyFileVersion("2.0.*.*")> 
            Dim objSel As TextSelection = Doc.Selection

            objSel.StartOfDocument()
            objSel.SelectAll()

            Dim strFull As String = objSel.Text
            objSel.StartOfDocument()
            objSel.SelectAll()
            objSel.Delete()

            Dim strReader As New IO.StringReader(strFull)
            Dim strWriter As New IO.StringWriter()
            Dim strLine As String
            Dim position As Integer = 0
            strLine = strReader.ReadLine
            While Not strLine Is Nothing
                Dim strReplace As String = strLine.ToUpper.Replace(" ", "").Replace(vbTab, "")
                If strReplace.StartsWith("<ASSEMBLY:ASSEMBLYVERSION") = True Then
                    '
                ElseIf strReplace.StartsWith("<ASSEMBLY:ASSEMBLYFILEVERSION") = True Then
                    '
                ElseIf strLine.Length > 2 OrElse position < 2 Then
                    strWriter.WriteLine(strLine)
                End If
                strLine = strReader.ReadLine
                position += 1
            End While
            strWriter.WriteLine("<Assembly: AssemblyVersion(""" & AssemblyVersion & """)>" & vbCrLf)
            strWriter.WriteLine("<Assembly: AssemblyFileVersion(""" & FileVersion & """)>" & vbCrLf)
            strWriter.Flush()
            strWriter.Close()
            strReader.Close()

            objSel.StartOfDocument()
            objSel.Insert(strWriter.GetStringBuilder.ToString)

            Doc.Save()
        Catch ex As Exception
            AppendDebug("Version", "Version Failure: Please verify that the solution contains a Assembly.Version.txt File" & vbCrLf)
        End Try
    End Sub

End Module

