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
Imports r2i.OWS.Framework.DataAccess
Imports r2i.OWS.Framework.Entities

Namespace r2i.OWS.Packaging
    Public Class Package
        Public PortalID As String
        Public PackageID As Integer
        Public Path As String
        Public ParseError As String
        Private _MapPath As MapPath
        Private _renderer As Engine
        Public Property Engine() As Engine
            Get
                Return _renderer
            End Get
            Set(ByVal Value As Engine)
                _renderer = Value
            End Set
        End Property
        Public Delegate Function MapPath(ByVal Path As String) As String
        Public Property PathMapper() As MapPath
            Get
                Return _MapPath
            End Get
            Set(ByVal Value As MapPath)
                _MapPath = Value
            End Set
        End Property
        Public PackageInfo As InstallController.Package
        Public Overrides Function toString() As String
            Return "P" & PortalID & "p" & PackageID
        End Function
        Public Sub New(ByVal PortalID As String, ByVal Path As String)
            Me.PortalID = PortalID
            Me.Path = Path
        End Sub
        Public Sub New(ByVal PortalID As Integer)
            Me.PortalID = PortalID
        End Sub
        Public Sub New()
        End Sub
#Region "Package Parsing"
        Private Sub ParsePackageItem(ByRef PackageItemInfo As InstallController.PackageItem, ByRef xmln As System.Xml.XmlNode)
            If Not xmln Is Nothing Then
                Dim xmlattr As XmlAttribute
                For Each xmlattr In xmln.Attributes
                    Select Case xmlattr.Name.ToLower
                        Case "description"
                            PackageItemInfo.ItemDescription = xmlattr.Value
                        Case "name"
                            PackageItemInfo.ItemName = xmlattr.Value
                        Case "path"
                            PackageItemInfo.ItemPath = xmlattr.Value
                        Case "sourceid"
                            PackageItemInfo.SourceID = xmlattr.Value
                    End Select
                Next
            End If
        End Sub
        Private Sub Parse_Installer(ByRef sourcexml As System.Xml.XmlDocument)
            Try
                Dim xmln As System.Xml.XmlNode = Common.CaseLess_SelectSingleNode(CType(sourcexml, System.Xml.XmlNode), "installer")
                If Not xmln Is Nothing Then
                    Dim xmlattr As XmlAttribute
                    For Each xmlattr In xmln.Attributes
                        Select Case xmlattr.Name.ToLower
                            Case "author"
                                PackageInfo.Author = xmlattr.Value
                            Case "company"
                                PackageInfo.Company = xmlattr.Value
                            Case "description"
                                PackageInfo.Description = xmlattr.Value
                            Case "name"
                                PackageInfo.Name = xmlattr.Value
                            Case "uniqueid"
                                PackageInfo.UniqueID = xmlattr.Value
                            Case "version"
                                PackageInfo.Version = xmlattr.Value
                        End Select
                    Next
                End If
            Catch ex As Exception
            End Try
        End Sub
        Private Sub Parse_Configuration(ByRef sourcexml As System.Xml.XmlDocument, ByRef dbI As InstallController, ByRef SequenceNumber As Integer)
            Dim xmln As System.Xml.XmlNode = Common.CaseLess_SelectSingleNode(CType(sourcexml, System.Xml.XmlNode), "installer/configuration")
            If Not xmln Is Nothing Then
                Try
                    Dim PackageItemInfo As New InstallController.PackageItem
                    ParsePackageItem(PackageItemInfo, xmln)

                    Dim configfilename As String = Path & "\" & PackageItemInfo.ItemName
                    If (New System.IO.FileInfo(configfilename)).Exists Then
                        Dim fstream As New IO.StreamReader(configfilename)
                        Dim fsrc As String = fstream.ReadToEnd
                        fstream.Close()
                        With PackageItemInfo
                            .SequenceNumber = SequenceNumber
                            .PackageID = PackageInfo.PackageID
                            .Content = System.Text.ASCIIEncoding.ASCII.GetBytes(fsrc)
                            .ItemType = Common.ItemTypeResult.ResultTypeEnumerator.Configuration.ToString
                            .Status = 1
                            .StatusDate = Now
                        End With
                        SequenceNumber += 1
                        PackageItemInfo.SequenceNumber = SequenceNumber
                        dbI.SetPackageItem(PackageItemInfo)
                    Else
                        'THE CONFIG FILE DOESNT EXIST
                    End If
                Catch ex As Exception
                    Throw New Exception("A failure occured while attempting to parse the configuration script: " & ex.ToString)
                End Try
            End If
        End Sub
        Private Sub Parse_Script(ByRef sourcexml As System.Xml.XmlDocument, ByVal dbI As InstallController, ByRef SequenceNumber As Integer)
            Try
                Dim xmlnl As System.Xml.XmlNodeList = Common.CaseLess_SelectNodes(CType(sourcexml, System.Xml.XmlNode), "installer/script")
                Dim xmlnp As System.Xml.XmlNode
                For Each xmlnp In xmlnl
                    Dim PackageItemInfo As New InstallController.PackageItem
                    ParsePackageItem(PackageItemInfo, xmlnp)

                    Dim filename As String = Path & "\" & PackageItemInfo.ItemName

                    Dim fstream As New IO.StreamReader(filename)
                    Dim fsrc As String = fstream.ReadToEnd
                    fstream.Close()

                    With PackageItemInfo
                        .SequenceNumber = SequenceNumber
                        .PackageID = PackageInfo.PackageID
                        .Content = System.Text.ASCIIEncoding.ASCII.GetBytes(fsrc)
                        .ItemType = Common.ItemTypeResult.ResultTypeEnumerator.Script.ToString
                        .Status = 1
                        .StatusDate = Now
                    End With
                    SequenceNumber += 1
                    PackageItemInfo.SequenceNumber = SequenceNumber
                    dbI.SetPackageItem(PackageItemInfo)

                    'If Not LoadScript(filename) Then
                    '    execResult.Succeeded = False
                    'End If
                Next
            Catch ex As Exception
                Throw New Exception("A failure occured while attempting to parse the package script: " & ex.ToString)
            End Try
        End Sub
        Private Sub Parse_Files(ByRef sourcexml As System.Xml.XmlDocument, ByVal dbI As InstallController, ByRef SequenceNumber As Integer)
            Try
                Dim xmlnl As System.Xml.XmlNodeList = Common.CaseLess_SelectNodes(CType(sourcexml, System.Xml.XmlNode), "installer/file")
                Dim xmlnp As System.Xml.XmlNode
                For Each xmlnp In xmlnl
                    Dim PackageItemInfo As New InstallController.PackageItem
                    ParsePackageItem(PackageItemInfo, xmlnp)

                    Dim filename As String = Path & "\" & PackageItemInfo.ItemName

                    Dim fstream As New IO.FileStream(filename, IO.FileMode.Open, IO.FileAccess.Read)
                    Dim b(fstream.Length) As Byte
                    If fstream.Length > 0 Then
                        fstream.Read(b, 0, fstream.Length)
                        fstream.Close()
                    End If

                    With PackageItemInfo
                        .ItemPath = _renderer.RenderString(Nothing, .ItemPath, Nothing, False, isPreRender:=False)
                        Try
                            If Not _MapPath Is Nothing Then
                                .ItemPath = _MapPath(.ItemPath)
                            End If
                        Catch ex As Exception

                        End Try
                        .PackageID = PackageInfo.PackageID
                        .Content = b
                        .ItemType = Common.ItemTypeResult.ResultTypeEnumerator.Resource.ToString
                        .Status = 1
                        .StatusDate = Now
                    End With
                    SequenceNumber += 1
                    PackageItemInfo.SequenceNumber = SequenceNumber
                    dbI.SetPackageItem(PackageItemInfo)
                Next
            Catch ex As Exception
                Throw New Exception("A failure occured while attempting to parse the package resources: " & ex.ToString)
            End Try
        End Sub
        'Private Function Parse_Tabs(ByRef sourcexml As System.Xml.XmlDocument, ByVal dbI As InstallController, ByRef SequenceNumber As Integer) As ArrayList
        '    'ROMAIN: Generic replacement - 08/21/2007
        '    Dim tabs As New ArrayList
        '    'Dim tabs As New List(Of InstallController.PackageItem)
        '    Try
        '        Dim xmlnl As System.Xml.XmlNodeList = Common.CaseLess_SelectNodes(sourcexml, "installer/tab")
        '        Dim xmlnp As System.Xml.XmlNode
        '        For Each xmlnp In xmlnl
        '            'this contains the name of the file which contains the tab information.
        '            'open the file, and presume to load the information below.

        '            Dim PackageItemInfo As New InstallController.PackageItem
        '            ParsePackageItem(PackageItemInfo, xmlnp)

        '            Dim filename As String = Path & "\" & PackageItemInfo.ItemName

        '            Dim fstream As New IO.StreamReader(filename)
        '            Dim fstr As String = fstream.ReadToEnd()
        '            fstream.Close()
        '            fstream = Nothing

        '            With PackageItemInfo
        '                .PackageID = PackageInfo.PackageID
        '                .Content = System.Text.ASCIIEncoding.ASCII.GetBytes(fstr)
        '                .ItemType = Common.ItemTypeResult.ResultTypeEnumerator.Tab.ToString
        '                .Status = 1
        '                .StatusDate = Now
        '            End With

        '            Dim tbi As DotNetNuke.Entities.Tabs.TabInfo = LoadTab_ParseContent(fstr, PackageItemInfo.ItemName)
        '            If Not tbi Is Nothing Then
        '                If tbi.TabID > 0 Then
        '                    PackageItemInfo.SourceID = tbi.TabID
        '                End If
        '            End If

        '            'CHECK FOR EXISTING
        '            Dim ipi As InstallController.PackageItem
        '            If PackageItemInfo.SourceID > 0 Then
        '                ipi = dbI.GetPackageItem(Me.PackageID, -1, PackageItemInfo.SourceID, Common.ItemTypeResult.ResultTypeEnumerator.Tab.ToString)
        '                If Not ipi Is Nothing Then
        '                    PackageItemInfo.PackageItemID = ipi.PackageItemID
        '                    PackageItemInfo.DestinationID = ipi.DestinationID
        '                End If
        '            End If

        '            SequenceNumber += 1
        '            PackageItemInfo.SequenceNumber = SequenceNumber

        '            dbI.SetPackageItem(PackageItemInfo)

        '            tabs.Add(PackageItemInfo)
        '        Next
        '    Catch ex As Exception

        '    End Try
        '    Return tabs
        'End Function
        Private Function Parse_Tabs(ByRef sourcexml As System.Xml.XmlDocument, ByVal dbI As InstallController, ByRef SequenceNumber As Integer) As List(Of InstallController.PackageItem)
            'ROMAIN: Generic replacement - 08/21/2007
            'Dim tabs As New ArrayList
            Dim tabs As New List(Of InstallController.PackageItem)
            Try
                Dim xmlnl As System.Xml.XmlNodeList = Common.CaseLess_SelectNodes(CType(sourcexml, System.Xml.XmlNode), "installer/tab")
                Dim xmlnp As System.Xml.XmlNode
                For Each xmlnp In xmlnl
                    'this contains the name of the file which contains the tab information.
                    'open the file, and presume to load the information below.

                    Dim PackageItemInfo As New InstallController.PackageItem
                    ParsePackageItem(PackageItemInfo, xmlnp)

                    Dim filename As String = Path & "\" & PackageItemInfo.ItemName

                    Dim fstream As New IO.StreamReader(filename)
                    Dim fstr As String = fstream.ReadToEnd()
                    fstream.Close()
                    fstream = Nothing

                    With PackageItemInfo
                        .PackageID = PackageInfo.PackageID
                        .Content = System.Text.ASCIIEncoding.ASCII.GetBytes(fstr)
                        .ItemType = Common.ItemTypeResult.ResultTypeEnumerator.Tab.ToString
                        .Status = 1
                        .StatusDate = Now
                    End With

                    'ROMAIN: 09/22/07
                    'Dim tbi As DotNetNuke.Entities.Tabs.TabInfo = LoadTab_ParseContent(fstr, PackageItemInfo.ItemName)
                    Dim tbi As ITabInfo = LoadTab_ParseContent(fstr, PackageItemInfo.ItemName)
                    If Not tbi Is Nothing Then
                        If tbi.TabID > 0 Then
                            PackageItemInfo.SourceID = tbi.TabID
                        End If
                    End If

                    'CHECK FOR EXISTING
                    Dim ipi As InstallController.PackageItem
                    If PackageItemInfo.SourceID > 0 Then
                        ipi = dbI.GetPackageItem(Me.PackageID, -1, PackageItemInfo.SourceID, Common.ItemTypeResult.ResultTypeEnumerator.Tab.ToString)
                        If Not ipi Is Nothing Then
                            PackageItemInfo.PackageItemID = ipi.PackageItemID
                            PackageItemInfo.DestinationID = ipi.DestinationID
                        End If
                    End If

                    SequenceNumber += 1
                    PackageItemInfo.SequenceNumber = SequenceNumber

                    dbI.SetPackageItem(PackageItemInfo)

                    tabs.Add(PackageItemInfo)
                Next
            Catch ex As Exception

            End Try
            Return tabs
        End Function

        'ROMAIN: Generic replacement - 08/21/2007
        'Private Sub Parse_Modules(ByRef sourcexml As System.Xml.XmlDocument, ByVal dbI As InstallController, ByRef Tabs As ArrayList, ByRef SequenceNumber As Integer)
        Private Sub Parse_Modules(ByRef sourcexml As System.Xml.XmlDocument, ByVal dbI As InstallController, ByRef Tabs As List(Of InstallController.PackageItem), ByRef SequenceNumber As Integer)
            If Tabs.Count > 0 Then
                Try
                    Dim pckTabInfo As InstallController.PackageItem = Nothing
                    For Each pckTabInfo In Tabs
                        Dim xmlDoc As New System.Xml.XmlDocument
                        Try
                            xmlDoc.LoadXml(System.Text.ASCIIEncoding.ASCII.GetString(pckTabInfo.Content))
                        Catch ex As Exception

                        End Try
                        Dim tabXML As XmlNode = Nothing
                        If Not xmlDoc Is Nothing Then
                            tabXML = Common.CaseLess_SelectSingleNode(CType(xmlDoc, Xml.XmlNode), "tab")
                        End If

                        Dim xmlnl As System.Xml.XmlNodeList = Common.CaseLess_SelectNodes(tabXML, "tab/module")
                        Dim xmlnp As System.Xml.XmlNode = Nothing
                        For Each xmlnp In xmlnl
                            Dim PackageItemInfo As InstallController.PackageItem = LoadModule_ParseContent(xmlnp, pckTabInfo)
                            If Not PackageItemInfo Is Nothing Then
                                SequenceNumber += 1
                                PackageItemInfo.SequenceNumber = SequenceNumber
                                dbI.SetPackageItem(PackageItemInfo)
                            End If
                        Next
                    Next
                Catch ex As Exception

                End Try
            End If
        End Sub
        Public Sub Load(ByVal PortalID As Integer, ByVal PackageID As Integer, ByVal Name As String, ByVal UniqueID As String)
            Me.PackageID = PackageID
            Me.PortalID = PortalID

            If Not Name Is Nothing AndAlso Name.Length = 0 Then
                Name = Nothing
            End If
            If Not UniqueID Is Nothing AndAlso UniqueID.Length = 0 Then
                UniqueID = Nothing
            End If
            If PackageID <= 0 AndAlso Not UniqueID Is Nothing Then
                Load_ByUniqueID(UniqueID)
            End If
            If PackageID <= 0 AndAlso Not Name Is Nothing AndAlso Me.PackageInfo Is Nothing Then
                Load_ByName(Name)
            End If
            If Me.PackageInfo Is Nothing Then
                Load()
            End If
        End Sub
        Public Sub Load_ByUniqueID(ByVal UniqueID As String)
            Dim pgC As New InstallController
            Me.PackageInfo = pgC.GetPackage(Me.PortalID, -1, Nothing, UniqueID)
            If Not Me.PackageInfo Is Nothing Then
                Me.PackageID = Me.PackageInfo.PackageID
            End If
        End Sub
        Public Sub Load_ByName(ByVal Name As String)
            Dim pgC As New InstallController
            Me.PackageInfo = pgC.GetPackage(Me.PortalID, -1, Name, Nothing)
            If Not Me.PackageInfo Is Nothing Then
                Me.PackageID = Me.PackageInfo.PackageID
            End If
        End Sub
        Public Sub Load()
            'Me.PackageInfo = New InstallController.Package
            If PackageInfo Is Nothing Then
                Dim pgC As New InstallController
                Me.PackageInfo = pgC.GetPackage(Me.PortalID, Me.PackageID, Nothing, Nothing)
            End If
        End Sub
        Public Function Save(ByVal Source As String) As Boolean
            Dim xmlDoc As New Xml.XmlDocument
            xmlDoc.LoadXml(Source)
            Return Save(xmlDoc)
        End Function
        Public Function Save(ByVal Installer As System.Xml.XmlDocument) As Boolean
            Dim dbI As New InstallController

            PackageInfo = New InstallController.Package

            Parse_Installer(Installer)

            'LOOK FOR AN EXISTING CONFIGURATION
            Dim existingPackage As InstallController.Package = Nothing
            If Not PackageInfo.UniqueID Is Nothing AndAlso PackageInfo.UniqueID.Length > 0 Then
                existingPackage = dbI.GetPackage(Me.PortalID, -1, Nothing, PackageInfo.UniqueID)
            End If
            If existingPackage Is Nothing AndAlso Not PackageInfo.Name Is Nothing AndAlso PackageInfo.Name.Length > 0 Then
                existingPackage = dbI.GetPackage(Me.PortalID, -1, PackageInfo.Name, Nothing)
            End If

            If Not existingPackage Is Nothing Then
                'EXISTING PACKAGE
                PackageInfo = existingPackage

                'REMOVE ALL ITEMS WHICH WILL NOT HAVE A SOURCEID
                Dim pgkI() As InstallController.PackageItem
                pgkI = dbI.GetPackageItems(PackageInfo.PackageID)
                Dim pgkItem As InstallController.PackageItem
                If Not pgkI Is Nothing AndAlso pgkI.Length > 0 Then
                    For Each pgkItem In pgkI
                        If pgkItem.ItemType.ToUpper = Common.ItemTypeResult.ResultTypeEnumerator.Resource.ToString.ToUpper Then
                            dbI.DeletePackageItem(pgkItem)
                        ElseIf pgkItem.ItemType.ToUpper = Common.ItemTypeResult.ResultTypeEnumerator.Configuration.ToString.ToUpper Then
                            dbI.DeletePackageItem(pgkItem)
                        ElseIf pgkItem.ItemType.ToUpper = Common.ItemTypeResult.ResultTypeEnumerator.Script.ToString.ToUpper Then
                            dbI.DeletePackageItem(pgkItem)
                        End If
                    Next
                End If
            Else
                existingPackage = PackageInfo
                PackageInfo.PortalID = Me.PortalID
            End If
            PackageInfo.StatusDate = Now
            PackageInfo.Status = 0
            PackageInfo.PackageID = dbI.SetPackage(PackageInfo)

            PackageID = PackageInfo.PackageID
            Dim seqNo As Integer = 0
            Try
                Parse_Configuration(Installer, dbI, seqNo)
                Parse_Script(Installer, dbI, seqNo)
                Parse_Files(Installer, dbI, seqNo)
                Parse_Modules(Installer, dbI, Parse_Tabs(Installer, dbI, seqNo), seqNo)
            Catch ex As Exception
                ParseError = ex.ToString()
                Return False
            End Try

            Return True
        End Function
        Private Function GetModuleDefinition(ByVal sourceXML As System.Xml.XmlNode, ByRef TargetType As String) As Integer
            If Not sourceXML Is Nothing Then
                Dim xnode As XmlNode = Common.CaseLess_SelectSingleNode(sourceXML, "FriendlyName")
                If Not xnode Is Nothing Then
                    'Dim mdC As New DotNetNuke.Entities.Modules.DesktopModuleController
                    Dim mdC As IDesktopModuleController = AbstractFactory.Instance.DesktopModuleController
                    'Dim mdI As DotNetNuke.Entities.Modules.DesktopModuleInfo = mdC.GetDesktopModuleByName(xnode.InnerXml)
                    Dim mdI As IDesktopModuleInfo = mdC.GetDesktopModuleByName(xnode.InnerXml)
                    If Not mdI Is Nothing Then
                        'Dim dmC As New DotNetNuke.Entities.Modules.Definitions.ModuleDefinitionController
                        'Dim dmI As DotNetNuke.Entities.Modules.Definitions.ModuleDefinitionInfo = dmC.GetModuleDefinitionByName(mdI.DesktopModuleID, mdI.FriendlyName)
                        Dim dmC As IModuleDefinitionController = AbstractFactory.Instance.ModuleDefinitionController
                        Dim dmI As IModuleDefinitionInfo = dmC.GetModuleDefinitionByName(mdI.DesktopModuleID, mdI.FriendlyName)
                        If Not dmI Is Nothing Then
                            If dmI.FriendlyName.ToUpper.StartsWith("LISTX") Then
                                TargetType = "LISTX"
                            ElseIf dmI.FriendlyName.ToUpper.StartsWith("TOOLBAR") Then
                                TargetType = "TOOLBAR"
                            End If
                            Return dmI.ModuleDefID
                        End If
                    End If
                End If
            End If
            Return -1
        End Function
        Private Function LoadModule_ParseContent(ByVal sourceXML As System.Xml.XmlNode, ByVal TabPackageItem As InstallController.PackageItem) As InstallController.PackageItem
            'Dim MInstaller As New ModuleInstaller
            'ROMAIN: Generic replacement - 08/21/2007
            'NOTE: roles_View and roles_Edit contains both integer and string
            'Dim roles_View As New List(Of Integer) 
            'Dim roles_Edit As New List(Of Integer)
            Dim roles_View As New ArrayList
            Dim roles_Edit As New ArrayList
            'ROMAIN: 09/21/07
            'Dim mi As DotNetNuke.Entities.Modules.ModuleInfo
            Dim mi As IModuleInfo = Nothing
            Dim installType As String = Nothing
            Dim settings As New Hashtable
            Dim source As String = Nothing

            'ROMAIN: 09/21/07
            'Dim mdC As New DotNetNuke.Entities.Modules.ModuleController
            Try
                mi = AbstractFactory.Instance.ModuleController.CreateNewModuleInfo()
                Dim useExisting As Boolean = False
                If ParseModule(sourceXML, mi, roles_View, roles_Edit, settings, source) Then
                    Dim TargetDefType As String = Nothing
                    Dim TargetDefID As Integer = GetModuleDefinition(sourceXML, TargetDefType)

                    If TargetDefID > 0 Then
                        If Not useExisting Then
                            mi.TabID = TabPackageItem.SourceID
                        End If
                        installType = Common.ItemTypeResult.ResultTypeEnumerator.Module.ToString
                    End If
                Else
                    'MInstaller = Nothing
                End If
            Catch ex As Exception
                'MInstaller = Nothing
            End Try
            If Not mi Is Nothing Then
                Dim ipi As InstallController.PackageItem = Nothing
                Dim pkc As New InstallController
                If mi.ModuleID > 0 Then
                    ipi = pkc.GetPackageItem(Me.PackageID, -1, mi.ModuleID, installType)
                End If
                If ipi Is Nothing Then
                    ipi = New InstallController.PackageItem
                End If
                ipi.Content = System.Text.ASCIIEncoding.ASCII.GetBytes(sourceXML.OuterXml)
                ipi.ItemDescription = mi.Description
                ipi.ItemName = mi.ModuleTitle
                ipi.ItemType = installType
                ipi.PackageID = TabPackageItem.PackageID
                ipi.ParentPackageItemID = TabPackageItem.PackageItemID
                ipi.SourceID = mi.ModuleID
                ipi.Status = 1
                ipi.StatusDate = Now
                Return ipi
            End If
            Return Nothing
        End Function
        'Private Function LoadTab_ParseContent(ByVal Source As String, ByVal srcName As String) As DotNetNuke.Entities.Tabs.TabInfo
        Private Function LoadTab_ParseContent(ByVal Source As String, ByVal srcName As String) As ITabInfo
            Dim tbI As ITabInfo = Nothing
            'ROMAIN: Generic replacement - 08/21/2007
            'NOTE: roles_View and roles_Edit contains both integer and string
            'Dim roles_View As New List(Of Integer) 
            'Dim roles_Edit As New List(Of Integer)
            Dim roles_View As New ArrayList
            Dim roles_Edit As New ArrayList

            Dim sourceXML As XmlNode = Nothing
            If Not Source Is Nothing Then
                Dim xmlDoc As New System.Xml.XmlDocument
                Try
                    xmlDoc.LoadXml(Source)
                Catch ex As Exception
                    xmlDoc = Nothing
                End Try
                If Not xmlDoc Is Nothing Then
                    sourceXML = Common.CaseLess_SelectSingleNode(CType(xmlDoc, System.Xml.XmlNode), "tab")
                End If
            End If

            If Not sourceXML Is Nothing Then
                'Dim tbC As New DotNetNuke.Entities.Tabs.TabController
                Dim tbC As ITabController = AbstractFactory.Instance.TabController
                Try
                    'tbI = New DotNetNuke.Entities.Tabs.TabInfo
                    tbI = tbC.CreateNewTabInfo()
                    If ParseTab(sourceXML, tbI, roles_View, roles_Edit) Then
                        tbI.PortalId = Me.PortalID
                    End If
                Catch ex As Exception
                End Try
            End If
            Return tbI
        End Function
        'ROMAIN: 09/21/07
        'Private Function ParseTab(ByVal SourceXML As System.Xml.XmlNode, ByRef Ti As DotNetNuke.Entities.Tabs.TabInfo, ByRef ViewRoles As ArrayList, ByRef EditRoles As ArrayList) As Boolean
        Private Function ParseTab(ByVal SourceXML As System.Xml.XmlNode, ByRef Ti As ITabInfo, ByRef ViewRoles As ArrayList, ByRef EditRoles As ArrayList) As Boolean
            Try
                Dim prop As System.Reflection.PropertyInfo
                'ROMAIN: 09/21/07
                'For Each prop In Ti.GetType().GetProperties()
                For Each prop In AbstractFactory.Instance.TabController.GetTabInfoProperties(Ti)
                    Dim xmlnode As System.Xml.XmlNode = Common.CaseLess_SelectSingleNode(SourceXML, prop.Name)
                    If Not xmlnode Is Nothing Then
                        Dim tp As System.Type = prop.PropertyType
                        Dim currentProperty As System.Reflection.PropertyInfo
                        currentProperty = AbstractFactory.Instance.TabController.GetTabInfoProperty(Ti, prop.Name)
                        If prop.CanWrite Then
                            Try
                                If currentProperty.PropertyType Is GetType(DateTime) Then
                                    currentProperty.SetValue(Ti, CType(xmlnode.InnerText, DateTime), Nothing)
                                ElseIf currentProperty.PropertyType Is GetType(Integer) Then
                                    currentProperty.SetValue(Ti, CType(xmlnode.InnerText, Int32), Nothing)
                                ElseIf currentProperty.PropertyType Is GetType(Boolean) Then
                                    currentProperty.SetValue(Ti, CType(xmlnode.InnerText, Boolean), Nothing)
                                ElseIf currentProperty.PropertyType Is GetType(ITabPermissionCollection) Then
                                    currentProperty.SetValue(Ti, CType(xmlnode.InnerText, ITabPermissionCollection), Nothing)
                                Else
                                    currentProperty.SetValue(Ti, xmlnode.InnerText, Nothing)
                                End If
                            Catch ex As Exception
                            End Try
                        End If
                        Dim roleValue As String
                        Select Case prop.Name.ToUpper
                            Case "AUTHORIZEDROLES"
                                Dim sRoles() As String = xmlnode.InnerText.Split(";"c)
                                If Not sRoles Is Nothing AndAlso sRoles.Length > 0 Then
                                    Dim sRole As String
                                    'ROMAIN: Generic replacement - 08/22/2007
                                    'Dim pRoles As SortedList = Common.GetRoles(PortalID)
                                    Dim pRoles As SortedList(Of String, String) = Common.GetRoles(PortalID)
                                    For Each sRole In sRoles
                                        roleValue = AbstractFactory.Instance.RoleController.GetCurrentRole(sRole, pRoles)
                                        If Not roleValue Is Nothing Then
                                            ViewRoles.Add(roleValue)
                                        End If
                                        'If Not sRole Is Nothing AndAlso sRole.Length > 0 AndAlso (pRoles.ContainsKey(sRole.ToUpper) OrElse sRole = DotNetNuke.Common.glbRoleAllUsersName OrElse sRole = DotNetNuke.Common.glbRoleUnauthUserName) Then
                                        '    Select Case sRole
                                        '        Case DotNetNuke.Common.glbRoleAllUsersName
                                        '            ViewRoles.Add(DotNetNuke.Common.glbRoleAllUsers)
                                        '        Case DotNetNuke.Common.glbRoleUnauthUserName
                                        '            ViewRoles.Add(DotNetNuke.Common.glbRoleUnauthUser)
                                        '        Case Else
                                        '            ViewRoles.Add(pRoles(sRole.ToUpper))
                                        '    End Select
                                        'End If
                                    Next
                                End If
                            Case "ADMINISTRATORROLES"
                                Dim sRoles() As String = xmlnode.InnerText.Split(";"c)
                                If Not sRoles Is Nothing AndAlso sRoles.Length > 0 Then
                                    Dim sRole As String
                                    'ROMAIN: Generic replacement - 08/22/2007
                                    'Dim pRoles As SortedList = Common.GetRoles(PortalID)
                                    Dim pRoles As SortedList(Of String, String) = Common.GetRoles(PortalID)
                                    For Each sRole In sRoles
                                        roleValue = AbstractFactory.Instance.RoleController.GetCurrentRole(sRole, pRoles)
                                        If Not roleValue Is Nothing Then
                                            ViewRoles.Add(roleValue)
                                        End If
                                        'If Not sRole Is Nothing AndAlso sRole.Length > 0 AndAlso (pRoles.ContainsKey(sRole.ToUpper) OrElse sRole = DotNetNuke.Common.glbRoleAllUsersName OrElse sRole = DotNetNuke.Common.glbRoleUnauthUserName) Then
                                        '    Select Case sRole
                                        '        Case DotNetNuke.Common.glbRoleAllUsersName
                                        '            EditRoles.Add(DotNetNuke.Common.glbRoleAllUsers)
                                        '        Case DotNetNuke.Common.glbRoleUnauthUserName
                                        '            EditRoles.Add(DotNetNuke.Common.glbRoleUnauthUser)
                                        '        Case Else
                                        '            EditRoles.Add(pRoles(sRole.ToUpper))
                                        '    End Select
                                        'End If
                                    Next
                                End If
                        End Select
                    End If
                Next
            Catch ex As Exception
                Return False
            End Try
            Return True
        End Function
        'Private Function ParseModule(ByVal SourceXML As System.Xml.XmlNode, ByRef Mi As DotNetNuke.Entities.Modules.ModuleInfo, ByRef ViewRoles As ArrayList, ByRef EditRoles As ArrayList, ByRef Settings As Hashtable, ByRef Source As String) As Boolean
        Private Function ParseModule(ByVal SourceXML As System.Xml.XmlNode, ByRef Mi As IModuleInfo, ByRef ViewRoles As ArrayList, ByRef EditRoles As ArrayList, ByRef Settings As Hashtable, ByRef Source As String) As Boolean
            Try
                Dim prop As System.Reflection.PropertyInfo
                'For Each prop In Mi.GetType().GetProperties()
                For Each prop In AbstractFactory.Instance.ModuleController.GetModuleInfoProperties(Mi)
                    Dim xmlnode As System.Xml.XmlNode = Common.CaseLess_SelectSingleNode(SourceXML, prop.Name)
                    If Not xmlnode Is Nothing Then
                        Dim tp As System.Type = prop.PropertyType
                        Dim currentProperty As System.Reflection.PropertyInfo
                        currentProperty = AbstractFactory.Instance.ModuleController.GetModuleInfoProperty(Mi, prop.Name)
                        If prop.CanWrite Then
                            Try
                                Try
                                    If currentProperty.PropertyType Is GetType(DateTime) Then
                                        currentProperty.SetValue(Mi, CType(xmlnode.InnerText, DateTime), Nothing)
                                    ElseIf currentProperty.PropertyType Is GetType(Integer) Then
                                        currentProperty.SetValue(Mi, CType(xmlnode.InnerText, Int32), Nothing)
                                    ElseIf currentProperty.PropertyType Is GetType(Boolean) Then
                                        currentProperty.SetValue(Mi, CType(xmlnode.InnerText, Boolean), Nothing)
                                    Else
                                        currentProperty.SetValue(Mi, xmlnode.InnerText, Nothing)
                                    End If
                                Catch ex As Exception
                                End Try

                            Catch ex As Exception
                            End Try
                        End If
                        Dim roleValue As String
                        Select Case prop.Name.ToUpper
                            Case "AUTHORIZEDVIEWROLES"
                                Dim sRoles() As String = xmlnode.InnerText.Split(";"c)
                                If Not sRoles Is Nothing AndAlso sRoles.Length > 0 Then
                                    Dim sRole As String
                                    'ROMAIN: Generic replacement - 08/22/2007
                                    'Dim pRoles As SortedList = Common.GetRoles(PortalID)
                                    Dim pRoles As SortedList(Of String, String) = Common.GetRoles(PortalID)
                                    For Each sRole In sRoles
                                        'ROMAIN: 09/21/07
                                        roleValue = AbstractFactory.Instance.RoleController.GetCurrentRole(sRole, pRoles)
                                        If Not roleValue Is Nothing Then
                                            ViewRoles.Add(roleValue)
                                        End If
                                        'For Each sRole In sRoles
                                        '    If Not sRole Is Nothing AndAlso sRole.Length > 0 AndAlso (pRoles.ContainsKey(sRole.ToUpper) OrElse sRole = DotNetNuke.Common.glbRoleAllUsersName OrElse sRole = DotNetNuke.Common.glbRoleUnauthUserName) Then
                                        '        Select Case sRole
                                        '            Case DotNetNuke.Common.glbRoleAllUsersName
                                        '                ViewRoles.Add(DotNetNuke.Common.glbRoleAllUsers)
                                        '            Case DotNetNuke.Common.glbRoleUnauthUserName
                                        '                ViewRoles.Add(DotNetNuke.Common.glbRoleUnauthUser)
                                        '            Case Else
                                        '                'ViewRoles.Add(pRoles(sRole.ToUpper))
                                        '                ViewRoles.Add(pRoles(sRole.ToUpper))
                                        '        End Select
                                        '    End If
                                    Next
                                End If
                            Case "AUTHORIZEDEDITROLES"
                                Dim sRoles() As String = xmlnode.InnerText.Split(";"c)
                                If Not sRoles Is Nothing AndAlso sRoles.Length > 0 Then
                                    Dim sRole As String
                                    'ROMAIN: Generic replacement - 08/22/2007
                                    'Dim pRoles As SortedList = Common.GetRoles(PortalID)
                                    Dim pRoles As SortedList(Of String, String) = Common.GetRoles(PortalID)
                                    For Each sRole In sRoles
                                        roleValue = AbstractFactory.Instance.RoleController.GetCurrentRole(sRole, pRoles)
                                        If Not roleValue Is Nothing Then
                                            ViewRoles.Add(roleValue)
                                        End If
                                        'If Not sRole Is Nothing AndAlso sRole.Length > 0 AndAlso (pRoles.ContainsKey(sRole.ToUpper) OrElse sRole = DotNetNuke.Common.glbRoleAllUsersName OrElse sRole = DotNetNuke.Common.glbRoleUnauthUserName) Then
                                        '    Select Case sRole
                                        '        Case DotNetNuke.Common.glbRoleAllUsersName
                                        '            EditRoles.Add(DotNetNuke.Common.glbRoleAllUsers)
                                        '        Case DotNetNuke.Common.glbRoleUnauthUserName
                                        '            EditRoles.Add(DotNetNuke.Common.glbRoleUnauthUser)
                                        '        Case Else
                                        '            EditRoles.Add(pRoles(sRole.ToUpper))
                                        '    End Select
                                        'End If
                                    Next
                                End If
                        End Select
                    End If
                Next
                Dim xmlSettings As XmlNodeList = Common.CaseLess_SelectNodes(SourceXML, "Setting")
                If Not xmlSettings Is Nothing AndAlso xmlSettings.Count > 0 Then
                    Dim xmlSNode As System.Xml.XmlNode
                    For Each xmlSNode In xmlSettings
                        Dim attr As XmlAttribute = xmlSNode.Attributes("key")
                        If attr Is Nothing Then
                            attr = xmlSNode.Attributes("KEY")
                        End If
                        If Not attr Is Nothing Then
                            Dim key As String = attr.Value
                            Dim value As String = xmlSNode.InnerText
                            If Not key Is Nothing AndAlso key.Length > 0 AndAlso Not Settings.ContainsKey(key) Then
                                Settings.Add(key, value)
                            End If
                        End If
                    Next
                End If
                Dim xmlSourceConfig As XmlNode = Common.CaseLess_SelectSingleNode(SourceXML, "Configuration")
                If Not xmlSourceConfig Is Nothing Then
                    Source = xmlSourceConfig.InnerText
                End If
            Catch ex As Exception
                Return False
            End Try
            Return True
        End Function
#End Region
    End Class
End Namespace
