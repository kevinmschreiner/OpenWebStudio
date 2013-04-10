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
'Imports System.Xml
'Imports ICSharpCode.SharpZipLib.Zip
'Imports ICSharpCode.SharpZipLib.Checksums
'Namespace r2i.OWS.Packaging
'    'Public Class PackageInstaller




'    '    'Private _packName As String = "Install"

'    '    'Private Function WriteDebugLine(ByVal value As String)
'    '    '    Dim iox As New IO.FileInfo(SpawnDirectory("~/DesktopModules/OWS/Debug/" & _packName & ".log"))
'    '    '    Dim fs As IO.FileStream = iox.OpenWrite()
'    '    '    Dim fw As New IO.StreamWriter(fs)
'    '    '    If fs.Length > 0 Then
'    '    '        fs.Position = fs.Length - 1
'    '    '    End If
'    '    '    fw.WriteLine(Now.ToShortTimeString & "-" & value)
'    '    '    fw.Flush()
'    '    '    fw.Close()
'    '    '    iox = Nothing
'    '    '    fs = Nothing
'    '    '    fw = Nothing
'    '    'End Function

'    '    Private Function HandleInstaller(ByVal Installer As System.Xml.XmlDocument, ByVal Path As String) As Boolean 'DEPRICATED
'    '        HandleInstaller_ParseContent(Installer, Path)

'    '        Dim execResult As New InstallResult(InstallResult.ResultTypeEnumerator.Script)
'    '        execResult.Initiated = Now
'    '        Dim installName As String = ""
'    '        Dim installVersion As String = ""
'    '        Try
'    '            Dim xmln As System.Xml.XmlNode = CaseLess_SelectSingleNode(Installer, "installer")
'    '            If Not xmln Is Nothing Then
'    '                Try
'    '                    Dim attr As XmlAttribute = xmln.Attributes("name")
'    '                    If attr Is Nothing Then
'    '                        attr = xmln.Attributes("NAME")
'    '                    End If
'    '                    installName = attr.Value
'    '                Catch ex As Exception
'    '                End Try
'    '                Try
'    '                    Dim attr As XmlAttribute = xmln.Attributes("version")
'    '                    If attr Is Nothing Then
'    '                        attr = xmln.Attributes("VERSION")
'    '                    End If
'    '                    installVersion = attr.Value
'    '                Catch ex As Exception
'    '                End Try
'    '            End If
'    '        Catch ex As Exception
'    '        End Try
'    '        If installName Is Nothing Then
'    '            installName = ""
'    '        End If
'    '        If installVersion Is Nothing Then
'    '            installVersion = ""
'    '        ElseIf installVersion.Length > 0 Then
'    '            installVersion = " (v" & installVersion & ")"
'    '        End If

'    '        execResult.Name = "Installing " & installName & installVersion & "..."
'    '        execResult.Succeeded = True
'    '        execResult.Result = ""
'    '        Me.Results.Add(execResult)
'    '        WriteDebugLine(execResult.Name)
'    '        Try
'    '            Dim xmln As System.Xml.XmlNode = CaseLess_SelectSingleNode(Installer, "installer/configuration")
'    '            If Not xmln Is Nothing Then
'    '                Try
'    '                    Dim attr As XmlAttribute = xmln.Attributes("name")
'    '                    If attr Is Nothing Then
'    '                        attr = xmln.Attributes("NAME")
'    '                    End If

'    '                    Dim configfilename As String = Path & "\" & attr.Value
'    '                    If (New System.IO.FileInfo(configfilename)).Exists Then
'    '                        Dim fstream As New IO.StreamReader(configfilename)
'    '                        Dim fsrc As String = fstream.ReadToEnd
'    '                        fstream.Close()
'    '                        If Not LoadConfig(fsrc) Then
'    '                            execResult.Succeeded = False
'    '                        End If
'    '                    Else
'    '                        'THE CONFIG FILE DOESNT EXIST
'    '                    End If
'    '                Catch ex As Exception
'    '                    Throw New Exception("A failure occured while attempting to parse the configuration script: " & ex.ToString)
'    '                End Try
'    '            End If


'    '            Try
'    '                Dim xmlnl As System.Xml.XmlNodeList = CaseLess_SelectNodes(Installer, "installer/script")
'    '                Dim xmlnp As System.Xml.XmlNode
'    '                For Each xmlnp In xmlnl
'    '                    Dim attr As XmlAttribute = xmlnp.Attributes("name")
'    '                    If attr Is Nothing Then
'    '                        attr = xmlnp.Attributes("NAME")
'    '                    End If
'    '                    Dim filename As String = Path & "\" & attr.Value
'    '                    If Not LoadScript(filename) Then
'    '                        execResult.Succeeded = False
'    '                    End If
'    '                Next
'    '            Catch ex As Exception
'    '                Throw New Exception("A failure occured while attempting to parse the package script: " & ex.ToString)
'    '            End Try

'    '            Try
'    '                Dim xmlnl As System.Xml.XmlNodeList = CaseLess_SelectNodes(Installer, "installer/file")
'    '                Dim xmlnp As System.Xml.XmlNode
'    '                For Each xmlnp In xmlnl
'    '                    Dim attr As XmlAttribute = xmlnp.Attributes("name")
'    '                    If attr Is Nothing Then
'    '                        attr = xmlnp.Attributes("NAME")
'    '                    End If

'    '                    Dim filename As String = Path & "\" & attr.Value
'    '                    Dim target As String

'    '                    Dim attrp As XmlAttribute = xmlnp.Attributes("path")
'    '                    If attrp Is Nothing Then
'    '                        attrp = xmlnp.Attributes("PATH")
'    '                    End If

'    '                    If Not attrp Is Nothing Then
'    '                        target = attrp.Value
'    '                    End If

'    '                    If Not LoadFile(filename, target) Then
'    '                        execResult.Succeeded = False
'    '                    End If
'    '                Next
'    '            Catch ex As Exception
'    '                Throw New Exception("A failure occured while attempting to parse the package resources: " & ex.ToString)
'    '            End Try

'    '            Dim Tabs As New ArrayList
'    '            Dim Modules As New ArrayList
'    '            Try
'    '                Dim xmlnl As System.Xml.XmlNodeList = CaseLess_SelectNodes(Installer, "installer/tab")
'    '                Dim xmlnp As System.Xml.XmlNode
'    '                For Each xmlnp In xmlnl
'    '                    'this contains the name of the file which contains the tab information.
'    '                    'open the file, and presume to load the information below.
'    '                    Dim attr As XmlAttribute = xmlnp.Attributes("name")
'    '                    If attr Is Nothing Then
'    '                        attr = xmlnp.Attributes("NAME")
'    '                    End If

'    '                    Dim filename As String = Path & "\" & attr.Value

'    '                    Dim tinstall As TabInstaller
'    '                    WriteDebugLine("Installing Tab File: " & filename)
'    '                    tinstall = LoadTab(filename, attr.Value)
'    '                    If Not tinstall Is Nothing Then
'    '                        execResult.Succeeded = True
'    '                        Tabs.Add(tinstall)
'    '                    End If
'    '                Next
'    '            Catch ex As Exception
'    '                WriteDebugLine("Tab Creation Error:" & ex.ToString)
'    '            End Try
'    '            WriteDebugLine("Deciphering Modules")
'    '            If Tabs.Count > 0 Then
'    '                Try
'    '                    Dim tinstall As TabInstaller
'    '                    For Each tinstall In Tabs
'    '                        Try
'    '                            Dim xmlnl As System.Xml.XmlNodeList = CaseLess_SelectNodes(tinstall.Source, "tab/module")
'    '                            Dim xmlnp As System.Xml.XmlNode
'    '                            For Each xmlnp In xmlnl
'    '                                WriteDebugLine("Loading Modules for Tab: " & tinstall.TabInfo.TabName)
'    '                                Dim minstaller As ModuleInstaller = LoadModule(xmlnp, tinstall.TabInfo)
'    '                                If Not minstaller Is Nothing Then
'    '                                    execResult.Succeeded = True
'    '                                    minstaller.TabIntall = tinstall
'    '                                    Modules.Add(minstaller)
'    '                                End If
'    '                            Next
'    '                        Catch ex As Exception
'    '                        End Try
'    '                    Next
'    '                Catch ex As Exception

'    '                End Try
'    '                WriteDebugLine("Installing Configurations")

'    '                Try
'    '                    Dim minstall As ModuleInstaller
'    '                    Dim mc As New DotNetNuke.Entities.Modules.ModuleController

'    '                    For Each minstall In Modules
'    '                        Try
'    '                            ReplaceSourceValues(minstall.Source, Tabs, Modules, minstall.Settings)
'    '                            Select Case minstall.TypeName.ToUpper
'    '                                Case "LISTX"
'    '                                    WriteDebugLine("Assigning Configuration: " & minstall.ModuleInfo.ModuleId & " LISTX")
'    '                                    If minstall.ModuleInfo.ModuleId > 0 Then
'    '                                        'SAVE THE LISTX
'    '                                        Dim LX As New r2i.OWS.Framework.Utilities.Compatibility.Settings
'    '                                        LX.UpdateSetting(minstall.TabIntall.TabInfo.TabID, minstall.ModuleInfo.ModuleId, minstall.Source)
'    '                                    End If
'    '                                Case "TOOLBAR"
'    '                                    WriteDebugLine("Assigning Configuration: " & minstall.ModuleInfo.ModuleId & " TOOLBAR")
'    '                                    If minstall.ModuleInfo.ModuleId > 0 Then
'    '                                        'SAVE THE TOOLBAR
'    '                                        Dim LX As New Bi4ce.Modules.Toolbar.ToolbarController
'    '                                        LX.UpdateToolbarSetting(minstall.TabIntall.TabInfo.TabID, minstall.ModuleInfo.ModuleId, minstall.Source)
'    '                                    End If
'    '                                Case Else
'    '                                    WriteDebugLine("Skipping Configuration: " & minstall.ModuleInfo.ModuleId & " " & minstall.TypeName.ToUpper)
'    '                            End Select
'    '                            Dim key As String
'    '                            For Each key In minstall.Settings.Keys
'    '                                mc.UpdateModuleSetting(minstall.ModuleInfo.ModuleId, key.ToString, minstall.Settings(key))
'    '                            Next

'    '                            'HEADER/FOOTER Handling
'    '                            Dim strHeader As String = minstall.ModuleInfo.Header
'    '                            Dim strFooter As String = minstall.ModuleInfo.Footer
'    '                            Dim HFChanged As Boolean = False
'    '                            ReplaceSourceValues(strHeader, Tabs, Modules, Nothing)
'    '                            ReplaceSourceValues(strFooter, Tabs, Modules, Nothing)
'    '                            If Not strHeader = minstall.ModuleInfo.Header Then
'    '                                HFChanged = True
'    '                                minstall.ModuleInfo.Header = strHeader
'    '                                WriteDebugLine("Module header changed for " & minstall.ModuleInfo.ModuleId)
'    '                            End If
'    '                            If Not strFooter = minstall.ModuleInfo.Footer Then
'    '                                HFChanged = True
'    '                                minstall.ModuleInfo.Footer = strFooter
'    '                                WriteDebugLine("Module footer changed for " & minstall.ModuleInfo.ModuleId)
'    '                            End If
'    '                            If HFChanged Then
'    '                                mc.UpdateModule(minstall.ModuleInfo)
'    '                            End If
'    '                        Catch ex As Exception
'    '                            WriteDebugLine("Unable to install module (" & minstall.ModuleInfo.ModuleId & "): " & ex.ToString)
'    '                        End Try
'    '                    Next
'    '                Catch ex As Exception
'    '                End Try
'    '            End If
'    '        Catch ex As Exception
'    '            execResult.Result = ex.ToString
'    '            execResult.Succeeded = False
'    '        End Try
'    '        execResult.Completed = Now
'    '        Return execResult.Succeeded
'    '    End Function



'    'End Class
'    'Public Class FileInstallerResults
'    '    Public Overloads Shared Function toString(ByVal Results As ArrayList) As String
'    '        Dim sb As New Text.StringBuilder
'    '        If Not Results Is Nothing Then
'    '            Dim result As xList.PackageInstaller.InstallResult
'    '            Dim i As Integer = 1
'    '            For Each result In Results
'    '                Dim tdiff As TimeSpan = result.Completed.Subtract(result.Initiated)
'    '                Dim tdiffValue As String = "(" & tdiff.TotalSeconds & " seconds)"
'    '                If Not result.Succeeded Then
'    '                    sb.Append("<span class='NormalRed'>")
'    '                Else
'    '                    sb.Append("<span class='Normal'>")
'    '                End If
'    '                sb.Append(i.ToString & ".")
'    '                sb.Append("<b>")
'    '                Select Case result.ResultType
'    '                    Case xList.PackageInstaller.InstallResult.ResultTypeEnumerator.Configuration
'    '                        sb.Append("CONFIGURE")
'    '                    Case xList.PackageInstaller.InstallResult.ResultTypeEnumerator.Install
'    '                        sb.Append("INSTALL")
'    '                    Case xList.PackageInstaller.InstallResult.ResultTypeEnumerator.Package
'    '                        sb.Append("PACKAGE")
'    '                    Case xList.PackageInstaller.InstallResult.ResultTypeEnumerator.Resource
'    '                        sb.Append("RESOURCE")
'    '                    Case xList.PackageInstaller.InstallResult.ResultTypeEnumerator.Script
'    '                        sb.Append("SCRIPT")
'    '                    Case xList.PackageInstaller.InstallResult.ResultTypeEnumerator.Module
'    '                        sb.Append("MODULE")
'    '                    Case xList.PackageInstaller.InstallResult.ResultTypeEnumerator.Tab
'    '                        sb.Append("TAB")
'    '                End Select
'    '                sb.Append("</b>:&nbsp;" & result.Name)
'    '                If Not result.Result Is Nothing Then
'    '                    sb.Append("<br><p>" & result.Result & "</p>")
'    '                Else
'    '                    sb.Append(" <i>" & tdiffValue & "</i>")
'    '                End If
'    '                sb.Append("</span><br>")
'    '                i += 1
'    '            Next
'    '        End If
'    '        Return sb.ToString
'    '    End Function
'    'End Class
'    'Public Class TabInstaller
'    '    Public Source As XmlNode
'    '    Public TabInfo As DotNetNuke.Entities.Tabs.TabInfo
'    '    Public ViewRoles As ArrayList
'    '    Public EditRoles As ArrayList
'    '    Public Name As String
'    'End Class
'    'Public Class ModuleInstaller
'    '    Public Source As String
'    '    Public ModuleInfo As DotNetNuke.Entities.Modules.ModuleInfo
'    '    Public TabIntall As TabInstaller
'    '    Public TypeName As String
'    '    Public ViewRoles As ArrayList
'    '    Public EditRoles As ArrayList
'    '    Public Settings As Hashtable
'    'End Class
'End Namespace