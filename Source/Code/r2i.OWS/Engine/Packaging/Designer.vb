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
Imports System.Xml
Imports System.Collections.Generic
Imports ICSharpCode.SharpZipLib.Zip
Imports ICSharpCode.SharpZipLib.Checksums
'Imports DotNetNuke.Entities.Modules
Imports r2i.OWS.Framework.DataAccess
Imports r2i.OWS.Framework.Entities


Namespace r2i.OWS.Packaging
    Public Class Designer
        Private TabID As String
        Private ModuleID As String
        ''ROMAIN: 08/21/2007
        'NOTE: UNUSED Results field
        'Public Results As New ArrayList
        Public Configured As Boolean '= False
        Public Scripted As Boolean '= False
        Private PortalID As String
        Private BasePath As String
        'ROMAIN: 09/21/07
        'Private PortalSettings As DotNetNuke.Entities.Portals.PortalSettings
        Private PortalSettings As IPortalSettings
        Private _renderer As Engine

        Private _MapPath As MapPath
        Public Delegate Function MapPath(ByVal Path As String) As String
        'ROMAIN: 09/21/07
        'Public Sub New(ByVal RenderingEngine As Engine, ByVal ModuleID As Integer, ByVal TabID As Integer, ByVal PortalID As Integer, ByVal BasePath As String, ByVal PathMapper As MapPath, ByVal PortalSettings As DotNetNuke.Entities.Portals.PortalSettings)
        '    Me.ModuleID = ModuleID
        '    Me.PortalID = PortalID
        '    Me.TabID = TabID
        '    Me.BasePath = BasePath
        '    Me.PortalSettings = PortalSettings
        '    _MapPath = PathMapper
        '    _renderer = RenderingEngine
        'End Sub
        Public Sub New(ByVal RenderingEngine As Engine, ByVal ModuleID As String, ByVal TabID As String, ByVal PortalID As String, ByVal BasePath As String, ByVal PathMapper As MapPath, ByVal PortalSettings As IPortalSettings)
            Me.ModuleID = ModuleID
            Me.PortalID = PortalID
            Me.TabID = TabID
            Me.BasePath = BasePath
            Me.PortalSettings = PortalSettings
            _MapPath = PathMapper
            _renderer = RenderingEngine
        End Sub
        Public Function File_CreateDirectory(ByVal Target As String) As String
            Dim strTarget As String = _MapPath(Target)
            Dim tio As New IO.FileInfo(strTarget)
            Dim dio As IO.DirectoryInfo = tio.Directory
            Dim dstack As New Stack
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
            Return strTarget
        End Function
        Private Function XMLFormat(ByVal Value As String) As String
            Return Value.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;")
        End Function
        Private Function XMLUnFormat(ByVal Value As String) As String
            Return Value.Replace("&gt;", ">").Replace("&lt;", "<").Replace("&amp;", "&")
        End Function
        'ROMAIN: Generic replacement - 08/21/2007
        'Public Function BuildPortalPackage(ByRef Si As DotNetNuke.UI.Skins.SkinInfo, ByRef Ci As DotNetNuke.UI.Skins.SkinInfo, ByRef TargetStream As IO.Stream, ByVal SelectedTabs As ArrayList) As Boolean
        '    Dim InstallSrc As String = ""
        '    InstallSrc &= "<installer name=""" & XMLFormat(Me.PortalSettings.PortalName) & """ version=""" & XMLFormat(Me.PortalSettings.Version) & """ generated=""" & XMLFormat(Now.ToString("MM/dd/yyyy hh:mm:ss")) & """ creator=""" & XMLFormat(_renderer.UserInfo.FullName) & """>" & vbCrLf
        '    InstallSrc &= "<!--" & vbCrLf
        '    InstallSrc &= "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~" & vbCrLf
        '    InstallSrc &= "NUKEDK - PACKAGE CONFIGURATION INSTRUCTIONS:" & vbCrLf
        '    InstallSrc &= "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~" & vbCrLf
        '    InstallSrc &= "The ListX Installation Package supported the following types of tags:" & vbCrLf
        '    InstallSrc &= "SCRIPT:  Executes provided SQL Scripts against the target site database. These" & vbCrLf
        '    InstallSrc &= "         Files should be placed in the order which they will be executed. Like" & vbCrLf
        '    InstallSrc &= "         standard DotNetNuke Scripts, the {databaseOwner} and {objectQualifier}" & vbCrLf
        '    InstallSrc &= "         tags are supported, as are standard ListX variables like [PORTALID]." & vbCrLf
        '    InstallSrc &= "EXAMPLE:" & vbCrLf
        '    InstallSrc &= "         <script name=""Tables.sql""/>" & vbCrLf
        '    InstallSrc &= "         <script name=""UserDefinedFunctions.sql""/>" & vbCrLf
        '    InstallSrc &= "         <script name=""StoredProcedures.sql""/>" & vbCrLf
        '    InstallSrc &= "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~" & vbCrLf
        '    InstallSrc &= "FILE:    Takes the incoming file by Name within the Package and stores it at the" & vbCrLf
        '    InstallSrc &= "         target location within the Dotnetnuke instance. All ListX tags are" & vbCrLf
        '    InstallSrc &= "         supported within the path attribute, so you can place simple things" & vbCrLf
        '    InstallSrc &= "         within the target path such as [PORTALID]." & vbCrLf
        '    InstallSrc &= "EXAMPLE:" & vbCrLf
        '    InstallSrc &= "         <file name=""paypal.html"" path=""~/items/paypal.html""/>" & vbCrLf
        '    InstallSrc &= "         <file name=""banner.gif"" path=""~/Portals/[PORTALID]/banner.gif""/>" & vbCrLf
        '    InstallSrc &= "         <file name=""spacer.gif"" path=""~/Portals/[PORTALID]/spacer.gif""/>" & vbCrLf
        '    InstallSrc &= "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~" & vbCrLf
        '    InstallSrc &= "TAB:     The heart of the ListX Package installation routines, the Tab element" & vbCrLf
        '    InstallSrc &= "         identifies the tab file which will be used to building the target" & vbCrLf
        '    InstallSrc &= "         pages and modules. ONLY NUKEDK MODULES ARE SUPPORTED HERE!!! So other" & vbCrLf
        '    InstallSrc &= "         third party modules will be completely ignored on the export. No indent" & vbCrLf
        '    InstallSrc &= "         is required, but it does aid in visual identification of parent child" & vbCrLf
        '    InstallSrc &= "         tab relationships between the target entities." & vbCrLf
        '    InstallSrc &= "EXAMPLE:" & vbCrLf
        '    InstallSrc &= "         <tab name=""Shipping Methods.tab""/>" & vbCrLf
        '    InstallSrc &= "             <tab name=""Edit Ship Methods.tab""/>" & vbCrLf
        '    InstallSrc &= "         <tab name=""PPP.tab""/>" & vbCrLf
        '    InstallSrc &= "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~" & vbCrLf
        '    InstallSrc &= "CONFIG:  When the Package is meant to only overwrite the current modules ListX" & vbCrLf
        '    InstallSrc &= "         Configuration, the Configuration tag lets you identify the source file" & vbCrLf
        '    InstallSrc &= "         containing the actual XML settings." & vbCrLf
        '    InstallSrc &= "         This is generally not used when additional tabs are created, and is not" & vbCrLf
        '    InstallSrc &= "         required." & vbCrLf
        '    InstallSrc &= "EXAMPLE:" & vbCrLf
        '    InstallSrc &= "         <configuration name=""Repository.config""/>" & vbCrLf
        '    InstallSrc &= "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~" & vbCrLf
        '    InstallSrc &= "-->" & vbCrLf & vbCrLf
        '    Dim ZIP As ZipOutputStream = CreateZipFile(TargetStream)
        '    Dim tC As New DotNetNuke.Entities.Tabs.TabController
        '    Dim tabs As ArrayList = tC.GetTabs(Me.PortalID)
        '    Dim tI As DotNetNuke.Entities.Tabs.TabInfo
        '    For Each tI In tabs
        '        Try
        '            If SelectedTabs.Contains(tI.TabID) Then
        '                If Not tI.IsAdminTab And Not tI.IsSuperTab And Not tI.IsDeleted Then
        '                    Dim tabSrc As String = DescribeTab(tI, Si, Ci)
        '                    If Not tabSrc Is Nothing AndAlso tabSrc.Length > 0 Then
        '                        InstallSrc &= vbTab
        '                        Dim tIndent As Integer = 0
        '                        While tIndent < tI.Level And tI.Level > 0
        '                            InstallSrc &= vbTab
        '                            tIndent += 1
        '                        End While
        '                        InstallSrc &= "<tab name=""" & XMLFormat(tI.TabName & ".tab") & """/>" & vbCrLf
        '                        'Dim fileIO As New IO.FileStream(SpawnDirectory("~/Transcribe/Portal/" & Me.PortalID & "/" & tI.TabName & ".txt"), IO.FileMode.Create)
        '                        AddFileToZip(ZIP, tabSrc, tI.TabName & ".tab", "")
        '                    End If
        '                End If
        '            End If
        '        Catch ex As Exception
        '        End Try
        '    Next
        '    InstallSrc &= "</installer>"
        '    AddFileToZip(ZIP, InstallSrc, "Package" & ".install", "")
        '    ZIP.Finish()
        '    ZIP.Close()
        '    Return True
        'End Function
        'TODO: parameters non CLS-Compliant!
        Public Function BuildPortalPackage(ByRef si As ISkinInfo, ByRef ci As ISkinInfo, ByRef targetStream As IO.FileStream, ByVal selectedTabs As List(Of String), ByVal selectedModules As List(Of String), ByVal Name As String, ByVal Version As String, ByVal UniqueId As String) As Boolean
            Dim InstallSrc As String = ""
            InstallSrc &= "<installer name=""" & XMLFormat(Name) & """ version=""" & XMLFormat(Version) & """ generated=""" & XMLFormat(Now.ToString("MM/dd/yyyy hh:mm:ss")) & """ creator=""" & XMLFormat(_renderer.UserInfo.DisplayName) & """ uniqueid=""" & XMLFormat(UniqueId) & """>" & vbCrLf
            InstallSrc &= "<!--" & vbCrLf
            InstallSrc &= "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~" & vbCrLf
            InstallSrc &= "OPEN WEB STUDIO - PACKAGE CONFIGURATION INSTRUCTIONS:" & vbCrLf
            InstallSrc &= "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~" & vbCrLf
            InstallSrc &= "The OWS Installation Package supported the following types of tags:" & vbCrLf
            InstallSrc &= "SCRIPT:  Executes provided SQL Scripts against the target site database. These" & vbCrLf
            InstallSrc &= "         Files should be placed in the order which they will be executed. Like" & vbCrLf
            InstallSrc &= "         other standard Scripts, the {databaseOwner} and {objectQualifier}" & vbCrLf
            InstallSrc &= "         tags are supported, as are standard OWS variables like [PORTALID]." & vbCrLf
            InstallSrc &= "EXAMPLE:" & vbCrLf
            InstallSrc &= "         <script name=""Tables.sql""/>" & vbCrLf
            InstallSrc &= "         <script name=""UserDefinedFunctions.sql""/>" & vbCrLf
            InstallSrc &= "         <script name=""StoredProcedures.sql""/>" & vbCrLf
            InstallSrc &= "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~" & vbCrLf
            InstallSrc &= "FILE:    Takes the incoming file by Name within the Package and stores it at the" & vbCrLf
            InstallSrc &= "         target location within the application instance. All OWS tags are" & vbCrLf
            InstallSrc &= "         supported within the path attribute, so you can place simple things" & vbCrLf
            InstallSrc &= "         within the target path such as [PORTALID]." & vbCrLf
            InstallSrc &= "EXAMPLE:" & vbCrLf
            InstallSrc &= "         <file name=""paypal.html"" path=""~/items/paypal.html""/>" & vbCrLf
            InstallSrc &= "         <file name=""banner.gif"" path=""~/Portals/[PORTALID]/banner.gif""/>" & vbCrLf
            InstallSrc &= "         <file name=""spacer.gif"" path=""~/Portals/[PORTALID]/spacer.gif""/>" & vbCrLf
            InstallSrc &= "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~" & vbCrLf
            InstallSrc &= "TAB:     The heart of the OWS Package installation routines, the Tab element" & vbCrLf
            InstallSrc &= "         identifies the tab file which will be used to building the target" & vbCrLf
            InstallSrc &= "         pages and modules. All portable modules ae supported. No indent" & vbCrLf
            InstallSrc &= "         is required, but it does aid in visual identification of parent child" & vbCrLf
            InstallSrc &= "         page relationships between the target entities." & vbCrLf
            InstallSrc &= "EXAMPLE:" & vbCrLf
            InstallSrc &= "         <tab name=""Shipping Methods.tab""/>" & vbCrLf
            InstallSrc &= "             <tab name=""Edit Ship Methods.tab""/>" & vbCrLf
            InstallSrc &= "         <tab name=""PPP.tab""/>" & vbCrLf
            InstallSrc &= "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~" & vbCrLf
            InstallSrc &= "CONFIG:  When the Package is meant to only overwrite the current modules OWS" & vbCrLf
            InstallSrc &= "         Configuration, the Configuration tag lets you identify the source file" & vbCrLf
            InstallSrc &= "         containing the actual settings." & vbCrLf
            InstallSrc &= "         This is generally not used when additional pages are created, and is not" & vbCrLf
            InstallSrc &= "         required." & vbCrLf
            InstallSrc &= "EXAMPLE:" & vbCrLf
            InstallSrc &= "         <configuration name=""Repository.config""/>" & vbCrLf
            InstallSrc &= "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~" & vbCrLf
            InstallSrc &= "-->" & vbCrLf & vbCrLf
            Dim ZIP As ZipOutputStream = CreateZipFile(targetStream)
            'ROMAIN: 09/21/07
            'Dim tC As New DotNetNuke.Entities.Tabs.TabController
            'Dim tabs As ArrayList = tC.GetTabs(Me.PortalID)
            'Dim tI As DotNetNuke.Entities.Tabs.TabInfo
            Dim tabs As ArrayList = AbstractFactory.Instance.TabController.GetTabs(Me.PortalID)
            Dim tI As ITabInfo
            For Each tI In tabs
                Try
                    If selectedTabs.Contains(tI.TabId) Then
                        If Not tI.IsAdminTab And Not tI.IsSuperTab And Not tI.IsDeleted Then
                            Dim tabSrc As String = DescribeTab(tI, si, ci, selectedModules)
                            If Not tabSrc Is Nothing AndAlso tabSrc.Length > 0 Then
                                InstallSrc &= vbTab
                                Dim tIndent As Integer = 0
                                While tIndent < tI.Level And tI.Level > 0
                                    InstallSrc &= vbTab
                                    tIndent += 1
                                End While
                                InstallSrc &= "<tab name=""" & XMLFormat(tI.TabName & ".tab") & """/>" & vbCrLf
                                'Dim fileIO As New IO.FileStream(SpawnDirectory("~/Transcribe/Portal/" & Me.PortalID & "/" & tI.TabName & ".txt"), IO.FileMode.Create)
                                AddFileToZip(ZIP, tabSrc, tI.TabName & ".tab", "")
                            End If
                        End If
                    End If
                Catch ex As Exception
                End Try
            Next
            InstallSrc &= "</installer>"
            AddFileToZip(ZIP, InstallSrc, "Package" & ".install", "")
            ZIP.Finish()
            ZIP.Close()
            Return True
        End Function
        ''ROMAIN: Generic replacement - 08/21/2007
        ''NOTE: OBSOLETE METHODE
        'Private Function BuildModuleKeyArray(ByVal TabName As String, ByVal Modules As ArrayList) As SortedList(Of Integer, String)
        '    Dim mI As DotNetNuke.Entities.Modules.ModuleInfo
        '    Dim sl As New SortedList(Of Integer, String)
        '    For Each mI In Modules
        '        Dim key As String = TabName & ":" & Common.NameModule(mI)
        '        Dim index As Integer = mI.ModuleID
        '        sl.Add(index, key)
        '    Next
        '    Return sl
        'End Function
        'ROMAIN: 09/20/07
        'NOTE: REPLACE BY THE public Dictionary<int, ModuleInfo> GetTabModules(int TabId) function
        'Private Function BuildModuleKeyArray(ByVal TabName As String, ByVal Modules As Dictionary(Of Integer, IModuleInfo)) As SortedList(Of Integer, String)
        '    Dim mI As IModuleInfo
        '    Dim sl As New SortedList(Of Integer, String)
        '    For Each mI In Modules
        '        Dim key As String = TabName & ":" & Common.NameModule(mI)
        '        Dim index As Integer = mI.ModuleID
        '        sl.Add(index, key)
        '    Next
        '    Return sl
        'End Function
        Private Function BuildModuleKeyArray(ByVal TabName As String, ByVal ModulesList As Dictionary(Of Integer, IModuleInfo)) As SortedList(Of Integer, String)
            Dim sl As New SortedList(Of Integer, String)
            For Each elmt As KeyValuePair(Of Integer, IModuleInfo) In ModulesList
                Dim key As String = TabName & ":" & Common.NameModule(elmt.Value)
                Dim index As Integer = elmt.Value.ModuleID
                sl.Add(index, key)
            Next
            Return sl
        End Function
        'ROMAIN: Generic replacement - 08/22/2007
        'Private Function GeneralizeContentSource(ByVal Source As Object, ByVal slK As SortedList) As Object
        'Private Function GeneralizeContentSource(ByVal Source As Object, ByVal slK As SortedList(Of Integer, String)) As Object
        '    If TypeOf (Source) Is String Then
        '        Dim str As String = Source
        '        Dim mID As Integer
        '        For Each mID In slK.Keys
        '            str = str.Replace(mID.ToString, "|!" & slK(mID) & "!|")
        '        Next
        '        Return str
        '    Else
        '        Return Source
        '    End If
        'End Function
        'ROMAIN: 09/21/07
        'Public Function DescribeTab(ByRef Ti As DotNetNuke.Entities.Tabs.TabInfo, ByRef Si As DotNetNuke.UI.Skins.SkinInfo, ByRef Ci As DotNetNuke.UI.Skins.SkinInfo) As String
        '    If Ti.SkinSrc Is Nothing OrElse Ti.SkinSrc.Length = 0 Then
        '        If Not Si Is Nothing Then
        '            Ti.SkinSrc = Si.SkinSrc
        '        End If
        '    End If
        '    If Ti.ContainerSrc Is Nothing OrElse Ti.ContainerSrc.Length = 0 Then
        '        If Not Ci Is Nothing Then
        '            Ti.ContainerSrc = Ci.SkinSrc
        '        End If
        '    End If
        '    Dim str As String = "<TAB>" & vbCrLf
        '    Try
        '        'Dim mc As New DotNetNuke.Entities.Modules.ModuleController
        '        Dim mc As IModuleController
        '        'ROMAIN: 08/21/07
        '        'NOTE: OBSOLETE METHOD 
        '        'ROMAIN: 09/20/07
        '        'Dim tMods As ArrayList = mc.GetPortalTabModules(Me.PortalID, Ti.TabID)
        '        'Dim tMods As List(Of IModuleInfo) = mc.GetPortalTabModules(Me.PortalID, Ti.TabID)
        '        Dim tMods As Dictionary(Of Integer, IModuleInfo) = mc.GetTabModules(Ti.TabID)
        '        'Dim tMods As Dictionary(Of Integer, ModuleInfo) = mc.GetTabModules(Ti.TabID)
        '        'ROMAIN: Generic replacement - 08/22/2007
        '        'Dim kSL As SortedList = BuildModuleKeyArray(Ti.TabName & ".txt", tMods)
        '        Dim kSL As SortedList(Of Integer, String) = BuildModuleKeyArray(Ti.TabName & ".txt", tMods)

        '        Dim prop As System.Reflection.PropertyInfo
        '        For Each prop In Ti.GetType().GetProperties()
        '            If Common.canImport_TabProperty(prop.Name) Then
        '                If prop.CanWrite And prop.CanRead Then
        '                    Dim nstr As String = vbTab & "<" & prop.Name & ">"
        '                    Try
        '                        nstr &= XMLFormat(prop.GetValue(Ti, Nothing).ToString)
        '                    Catch ex As Exception
        '                    End Try
        '                    nstr &= "</" & prop.Name & ">" & vbCrLf
        '                    str &= nstr
        '                End If
        '            End If
        '        Next
        '        If Ti.ParentId > 0 Then
        '            Dim tiP As DotNetNuke.Entities.Tabs.TabInfo
        '            Dim tiC As New DotNetNuke.Entities.Tabs.TabController
        '            tiP = tiC.GetTab(Ti.ParentId)
        '            If Not tiP Is Nothing AndAlso Not tiP.TabName Is Nothing AndAlso tiP.TabName.Length > 0 Then
        '                str &= vbTab & "<ParentTabName>" & XMLFormat(tiP.TabName) & "</ParentTabName>" & vbCrLf
        '            End If
        '        End If
        '        If Not tMods Is Nothing AndAlso tMods.Count > 0 Then
        '            Dim tmI As DotNetNuke.Entities.Modules.ModuleInfo
        '            'ROMAIN: 09/20/07
        '            Dim dc As IDesignerController = AbstractFactory.Instance.DesignController
        '            For Each elmt As KeyValuePair(Of Integer, IModuleInfo) In tMods
        '                'Function moved to the Core
        '                'str &= dc.DescribeModule(elmt)
        '                str &= dc.DescribeModule(elmt.Value, kSL)
        '            Next
        '            'For Each tmI In tMods
        '            '    str &= dc.DescribeModule(tmI, kSL)
        '            'Next
        '        End If
        '    Catch ex As Exception
        '        Return False
        '    End Try
        '    str &= "</TAB>" & vbCrLf
        '    Return str
        'End Function
        Public Function DescribeTab(ByRef ti As ITabInfo, ByRef Si As ISkinInfo, ByRef Ci As ISkinInfo, ByVal selectedModules As List(Of String)) As String
            If ti.SkinSrc Is Nothing OrElse ti.SkinSrc.Length = 0 Then
                If Not Si Is Nothing Then
                    ti.SkinSrc = Si.SkinSrc
                End If
            End If
            If ti.ContainerSrc Is Nothing OrElse ti.ContainerSrc.Length = 0 Then
                If Not Ci Is Nothing Then
                    ti.ContainerSrc = Ci.SkinSrc
                End If
            End If
            Dim str As String = "<TAB>" & vbCrLf
            Try
                'Dim mc As New DotNetNuke.Entities.Modules.ModuleController
                'ROMAIN: 08/21/07
                'NOTE: OBSOLETE METHOD 
                'ROMAIN: 09/20/07
                'Dim tMods As ArrayList = mc.GetPortalTabModules(Me.PortalID, Ti.TabID)
                'Dim tMods As List(Of IModuleInfo) = mc.GetPortalTabModules(Me.PortalID, Ti.TabID)
                Dim tMods As Dictionary(Of Integer, IModuleInfo) = AbstractFactory.Instance.ModuleController.GetTabModules(ti.TabId, ti.PortalId)
                'Dim tMods As Dictionary(Of Integer, ModuleInfo) = mc.GetTabModules(Ti.TabID)
                'ROMAIN: Generic replacement - 08/22/2007
                'Dim kSL As SortedList = BuildModuleKeyArray(Ti.TabName & ".txt", tMods)
                Dim kSL As SortedList(Of Integer, String) = BuildModuleKeyArray(ti.TabName & ".txt", tMods)

                'ROMAIN: 09/21/07
                Dim prop As System.Reflection.PropertyInfo
                'For Each prop In Ti.GetType().GetProperties()


                For Each prop In GetType(ITabInfo).GetProperties()
                    If Common.canImport_TabProperty(prop.Name) Then
                        If prop.CanWrite And prop.CanRead Then
                            Dim nstr As String = vbTab & "<" & prop.Name & ">"
                            Try
                                nstr &= XMLFormat(prop.GetValue(ti, Nothing).ToString)
                            Catch ex As Exception
                            End Try
                            nstr &= "</" & prop.Name & ">" & vbCrLf
                            str &= nstr
                        End If
                    End If
                Next
                If ti.ParentId > 0 Then
                    'ROMAIN: 09/21/07
                    'Dim tiP As DotNetNuke.Entities.Tabs.TabInfo
                    'Dim tiC As New DotNetNuke.Entities.Tabs.TabController
                    'tiP = tiC.GetTab(Ti.ParentId)
                    Dim tiP As ITabInfo = AbstractFactory.Instance.TabController.GetTab(ti.ParentId)
                    If Not tiP Is Nothing AndAlso Not tiP.TabName Is Nothing AndAlso tiP.TabName.Length > 0 Then
                        str &= vbTab & "<ParentTabName>" & XMLFormat(tiP.TabName) & "</ParentTabName>" & vbCrLf
                    End If
                End If
                If Not tMods Is Nothing AndAlso tMods.Count > 0 Then
                    'Dim tmI As DotNetNuke.Entities.Modules.ModuleInfo
                    'ROMAIN: 09/20/07
                    Dim dc As IDesignerController = AbstractFactory.Instance.DesignController
                    For Each elmt As KeyValuePair(Of Integer, IModuleInfo) In tMods
                        'Function moved to the Core
                        'str &= dc.DescribeModule(elmt)
                        If selectedModules.Contains(elmt.Value.ModuleID) Then
                            str &= dc.DescribeModule(elmt.Value, kSL)
                        End If
                    Next
                    'For Each tmI In tMods
                    '    str &= dc.DescribeModule(tmI, kSL)
                    'Next
                End If
            Catch ex As Exception
                Return False
            End Try
            str &= "</TAB>" & vbCrLf
            Return str
        End Function
        'Public Function DescribeModule(ByRef Mi As DotNetNuke.Entities.Modules.ModuleInfo, ByRef mKeyList As SortedList(Of Integer, String)) As String
        '    Dim btype As Integer = 0
        '    If Mi.FriendlyName.ToUpper.StartsWith("LISTX") Then
        '        'THIS IS LISTX
        '        btype = 1
        '    ElseIf Mi.FriendlyName.ToUpper.StartsWith("TOOLBAR") Then
        '        'THIS IS TOOLBAR
        '        btype = 2
        '    End If
        '    If btype > 0 Then
        '        Dim str As String = vbTab & "<MODULE>" & vbCrLf
        '        Try
        '            If Not Mi.Header Is Nothing AndAlso Mi.Header.Length > 0 Then
        '                Mi.Header = GeneralizeContentSource(Mi.Header, mKeyList)
        '            End If
        '            If Not Mi.Footer Is Nothing AndAlso Mi.Footer.Length > 0 Then
        '                Mi.Footer = GeneralizeContentSource(Mi.Footer, mKeyList)
        '            End If

        '            Dim prop As System.Reflection.PropertyInfo
        '            For Each prop In Mi.GetType().GetProperties()
        '                If Common.canImport_ModuleProperty(prop.Name) Then
        '                    If prop.CanWrite And prop.CanRead Then
        '                        Dim nstr As String = vbTab & vbTab & "<" & prop.Name & ">"
        '                        Try
        '                            nstr &= XMLFormat(prop.GetValue(Mi, Nothing).ToString)
        '                        Catch ex As Exception
        '                        End Try
        '                        nstr &= "</" & prop.Name & ">" & vbCrLf
        '                        str &= nstr
        '                    End If
        '                End If
        '            Next
        '            ' str &= vbTab & vbTab & "<TabModuleName>" & XMLFormat(NameModule(Mi)) & "</TabModuleName>" & vbCrLf

        '            Dim mc As New DotNetNuke.Entities.Modules.ModuleController
        '            Dim msettings As Hashtable = mc.GetModuleSettings(Mi.ModuleID)
        '            If Not msettings Is Nothing AndAlso msettings.Count > 0 Then
        '                Dim key As String
        '                Dim value As String
        '                For Each key In msettings.Keys
        '                    value = msettings.Item(key)
        '                    If value Is Nothing Then value = ""
        '                    str &= vbTab & vbTab & "<SETTING KEY=""" & key & """>" & XMLFormat(GeneralizeContentSource(value, mKeyList)) & "</SETTING>" & vbCrLf
        '                Next
        '            End If
        '            Dim src As String = ""
        '            If btype = 1 Then
        '                'LISTX CONFIG
        '                Dim LX As New r2i.OWS.Framework.Utilities.Compatibility.Settings
        '                src = LX.GetSetting(Mi.TabID, Mi.ModuleID)
        '                LX = Nothing
        '            ElseIf btype = 2 Then
        '                'TOOLBAR CONFIG
        '                Dim LX As New Bi4ce.Modules.Toolbar.ToolbarController
        '                src = LX.GetToolbarSetting(Mi.TabID, Mi.ModuleID)
        '                LX = Nothing
        '            End If
        '            If src Is Nothing Then src = ""
        '            str &= vbTab & vbTab & "<CONFIGURATION>" & XMLFormat(src) & "</CONFIGURATION>"
        '        Catch ex As Exception
        '            Return False
        '        End Try
        '        str &= vbTab & "</MODULE>" & vbCrLf
        '        Return str
        '    Else
        '        'ROMAIN: 08/22/2007
        '        'NOTE: Replacement Return ""
        '        Return String.Empty
        '    End If
        'End Function
        'Public Function DescribeModule(ByRef Mi As DotNetNuke.Entities.Modules.ModuleInfo, ByRef mKeyList As SortedList(Of Integer, String)) As String
        '    Dim btype As Integer = 0
        '    If Mi.FriendlyName.ToUpper.StartsWith("LISTX") Then
        '        'THIS IS LISTX
        '        btype = 1
        '    ElseIf Mi.FriendlyName.ToUpper.StartsWith("TOOLBAR") Then
        '        'THIS IS TOOLBAR
        '        btype = 2
        '    End If
        '    If btype > 0 Then
        '        Dim str As String = vbTab & "<MODULE>" & vbCrLf
        '        Try
        '            If Not Mi.Header Is Nothing AndAlso Mi.Header.Length > 0 Then
        '                Mi.Header = GeneralizeContentSource(Mi.Header, mKeyList)
        '            End If
        '            If Not Mi.Footer Is Nothing AndAlso Mi.Footer.Length > 0 Then
        '                Mi.Footer = GeneralizeContentSource(Mi.Footer, mKeyList)
        '            End If

        '            Dim prop As System.Reflection.PropertyInfo
        '            For Each prop In Mi.GetType().GetProperties()
        '                If Common.canImport_ModuleProperty(prop.Name) Then
        '                    If prop.CanWrite And prop.CanRead Then
        '                        Dim nstr As String = vbTab & vbTab & "<" & prop.Name & ">"
        '                        Try
        '                            nstr &= XMLFormat(prop.GetValue(Mi, Nothing).ToString)
        '                        Catch ex As Exception
        '                        End Try
        '                        nstr &= "</" & prop.Name & ">" & vbCrLf
        '                        str &= nstr
        '                    End If
        '                End If
        '            Next
        '            ' str &= vbTab & vbTab & "<TabModuleName>" & XMLFormat(NameModule(Mi)) & "</TabModuleName>" & vbCrLf

        '            Dim mc As New DotNetNuke.Entities.Modules.ModuleController
        '            Dim msettings As Hashtable = mc.GetModuleSettings(Mi.ModuleID)
        '            If Not msettings Is Nothing AndAlso msettings.Count > 0 Then
        '                Dim key As String
        '                Dim value As String
        '                For Each key In msettings.Keys
        '                    value = msettings.Item(key)
        '                    If value Is Nothing Then value = ""
        '                    str &= vbTab & vbTab & "<SETTING KEY=""" & key & """>" & XMLFormat(GeneralizeContentSource(value, mKeyList)) & "</SETTING>" & vbCrLf
        '                Next
        '            End If
        '            Dim src As String = ""
        '            If btype = 1 Then
        '                'LISTX CONFIG
        '                Dim LX As New r2i.OWS.Framework.Utilities.Compatibility.Settings
        '                src = LX.GetSetting(Mi.TabID, Mi.ModuleID)
        '                LX = Nothing
        '            ElseIf btype = 2 Then
        '                'TOOLBAR CONFIG
        '                Dim LX As New Bi4ce.Modules.Toolbar.ToolbarController
        '                src = LX.GetToolbarSetting(Mi.TabID, Mi.ModuleID)
        '                LX = Nothing
        '            End If
        '            If src Is Nothing Then src = ""
        '            str &= vbTab & vbTab & "<CONFIGURATION>" & XMLFormat(src) & "</CONFIGURATION>"
        '        Catch ex As Exception
        '            Return False
        '        End Try
        '        str &= vbTab & "</MODULE>" & vbCrLf
        '        Return str
        '    Else
        '        'ROMAIN: 08/22/2007
        '        'NOTE: Replacement Return ""
        '        Return String.Empty
        '    End If
        'End Function
        'Public Function DescribeModule(ByRef elmt As KeyValuePair(Of Integer, IModuleInfo)) As String
        '    Dim btype As Integer = 0
        '    Dim Mi As IModuleInfo = elmt.Value
        '    If Mi.FriendlyName.ToUpper.StartsWith("LISTX") Then
        '        'THIS IS LISTX
        '        btype = 1
        '    ElseIf Mi.FriendlyName.ToUpper.StartsWith("TOOLBAR") Then
        '        'THIS IS TOOLBAR
        '        btype = 2
        '    End If
        '    If btype > 0 Then
        '        Dim str As String = vbTab & "<MODULE>" & vbCrLf
        '        Try
        '            If Not Mi.Header Is Nothing AndAlso Mi.Header.Length > 0 Then
        '                Mi.Header = GeneralizeContentSource(Mi.Header, mKeyList)
        '            End If
        '            If Not Mi.Footer Is Nothing AndAlso Mi.Footer.Length > 0 Then
        '                Mi.Footer = GeneralizeContentSource(Mi.Footer, mKeyList)
        '            End If

        '            Dim prop As System.Reflection.PropertyInfo
        '            For Each prop In Mi.GetType().GetProperties()
        '           If Common.canImport_ModuleProperty(prop.Name) Then
        '                    If prop.CanWrite And prop.CanRead Then
        '                        Dim nstr As String = vbTab & vbTab & "<" & prop.Name & ">"
        '                        Try
        '                            nstr &= XMLFormat(prop.GetValue(Mi, Nothing).ToString)
        '                        Catch ex As Exception
        '                        End Try
        '                        nstr &= "</" & prop.Name & ">" & vbCrLf
        '                        str &= nstr
        '                    End If
        '                End If
        '            Next
        '            ' str &= vbTab & vbTab & "<TabModuleName>" & XMLFormat(NameModule(Mi)) & "</TabModuleName>" & vbCrLf

        '            Dim mc As New DotNetNuke.Entities.Modules.ModuleController
        '            Dim msettings As Hashtable = mc.GetModuleSettings(Mi.ModuleID)
        '            If Not msettings Is Nothing AndAlso msettings.Count > 0 Then
        '                Dim key As String
        '                Dim value As String
        '                For Each key In msettings.Keys
        '                    value = msettings.Item(key)
        '                    If value Is Nothing Then value = ""
        '                    str &= vbTab & vbTab & "<SETTING KEY=""" & key & """>" & XMLFormat(GeneralizeContentSource(value, mKeyList)) & "</SETTING>" & vbCrLf
        '                Next
        '            End If
        '            Dim src As String = ""
        '            If btype = 1 Then
        '                'LISTX CONFIG
        '                Dim LX As New r2i.OWS.Framework.Utilities.Compatibility.Settings
        '                src = LX.GetSetting(Mi.TabID, Mi.ModuleID)
        '                LX = Nothing
        '            ElseIf btype = 2 Then
        '                'TOOLBAR CONFIG
        '                Dim LX As New Bi4ce.Modules.Toolbar.ToolbarController
        '                src = LX.GetToolbarSetting(Mi.TabID, Mi.ModuleID)
        '                LX = Nothing
        '            End If
        '            If src Is Nothing Then src = ""
        '            str &= vbTab & vbTab & "<CONFIGURATION>" & XMLFormat(src) & "</CONFIGURATION>"
        '        Catch ex As Exception
        '            Return False
        '        End Try
        '        str &= vbTab & "</MODULE>" & vbCrLf
        '        Return str
        '    Else
        '        'ROMAIN: 08/22/2007
        '        'NOTE: Replacement Return ""
        '        Return String.Empty
        '    End If
        'End Function
#Region "Package Compression"
        Private Function CreateZipFile(ByVal Destination As IO.Stream) As ZipOutputStream
            Dim zp As New ZipOutputStream(Destination)
            zp.SetLevel(9)

            Return zp
        End Function
        Private Function AddFileToZip(ByRef ZipFile As ZipOutputStream, ByVal Source As IO.Stream, ByVal fileName As String, ByVal folderName As String) As Boolean
            Dim crc As Crc32 = New Crc32

            'Read file into byte array buffer
            Dim buffer As Byte()
            ReDim buffer(Convert.ToInt32(Source.Length) - 1)
            Source.Position = 0
            Source.Read(buffer, 0, buffer.Length)

            'Create Zip Entry
            Dim entry As ZipEntry = New ZipEntry(folderName & fileName)
            entry.DateTime = DateTime.Now
            entry.Size = Source.Length

            crc.Reset()
            crc.Update(buffer)
            entry.Crc = crc.Value

            'Compress file and add to Zip file
            ZipFile.PutNextEntry(entry)
            ZipFile.Write(buffer, 0, buffer.Length)
        End Function
        Private Function AddFileToZip(ByRef ZipFile As ZipOutputStream, ByVal Text As String, ByVal fileName As String, ByVal folderName As String) As Boolean
            Dim crc As Crc32 = New Crc32

            'Read file into byte array buffer
            Dim buffer As Byte()
            'ReDim buffer(Convert.ToInt32(Source.Length) - 1)
            buffer = System.Text.UTF8Encoding.UTF8.GetBytes(Text)

            'Create Zip Entry
            Dim entry As ZipEntry = New ZipEntry(folderName & fileName)
            entry.DateTime = DateTime.Now
            entry.Size = buffer.Length

            crc.Reset()
            crc.Update(buffer)
            entry.Crc = crc.Value

            'Compress file and add to Zip file
            ZipFile.PutNextEntry(entry)
            ZipFile.Write(buffer, 0, buffer.Length)
        End Function
#End Region
    End Class
End Namespace
