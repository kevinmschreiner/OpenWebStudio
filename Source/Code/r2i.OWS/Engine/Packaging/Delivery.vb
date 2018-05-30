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
Imports r2i.OWS.Framework.Entities
Imports r2i.OWS.Framework.DataAccess


Namespace r2i.OWS.Packaging
    Public Class Delivery
        Private Const AUTHORIZEDROLES As String = "AUTHORIZEDROLES"
        Private Const ADMINISTRATORROLES As String = "ADMINISTRATORROLES"
        'ROMAIN: Generic replacement - 08/21/2007
        'Private Errors As ArrayList

        Private Errors As List(Of String)
        Public SourcePackage As Package
        'Private _renderer As Engine
        Private _Engine As Engine
        Public ThreadObject As Threading.Thread
        Private configurationId As Guid
        Private pageId As String
        Private moduleId As String
        Public Configured As Boolean '= False
        Public Scripted As Boolean '= False
        Private PortalID As Integer
        Private BasePath As String
        Private _PackagePath As String
        Private _WebPath As String
        'ROMAIN: Generic replacement - 08/21/2007
        'Private _InstalledTabs_PackageItems As ArrayList
        'Private _InstalledModules_PackageItems As ArrayList
        Private _InstalledTabs_PackageItems As List(Of InstallController.PackageItem)
        Private _InstalledModules_PackageItems As List(Of InstallController.PackageItem)

        'Private PortalSettings As DotNetNuke.Entities.Portals.PortalSettings
        Private PortalSettings_AdminRoleName As String
        Private PortalSettings_AdminRoleId As Integer

        Public Sub New(ByVal PackageID As Integer, ByVal RenderingEngine As Engine, ByVal configurationId As Guid, ByVal moduleId As String, ByVal pageId As String, ByVal ClientID As String, ByVal PortalID As String, ByVal BasePath As String, ByVal AdminRoleName As String, ByVal AdminRoleID As Integer, ByVal RootPath As String, ByVal PackagePath As String)
            Me.moduleId = moduleId
            Me.PortalID = PortalID
            _PackagePath = PackagePath
            Me.pageId = pageId
            Me.configurationId = configurationId
            Me.BasePath = BasePath
            _WebPath = BasePath
            PortalSettings_AdminRoleName = AdminRoleName
            PortalSettings_AdminRoleId = AdminRoleID
            '_Engine = New Engine(Nothing, Nothing, RenderingEngine.UserInfo, Nothing, Nothing, RenderingEngine.PortalSettings, Nothing, ModuleId, TabID, TabModuleId, Nothing, ClientID, BasePath, Nothing, True)
            _Engine = RenderingEngine

            Dim pkg As New Packaging.Package
            pkg.Load(PortalID, PackageID, Nothing, Nothing)
            Me.SourcePackage = pkg
        End Sub

        Public Sub Deliver(ByVal Threaded As Boolean)
            If Threaded Then
                ThreadObject = New System.Threading.Thread(AddressOf _Deliver)
            End If
            If Not ThreadObject Is Nothing Then
                ThreadObject.Start()
            Else
                _Deliver()
            End If
        End Sub
        Private Sub _Deliver()
            Try
                'INSTALL PACKAGE
                'ROMAIN: Generic replacement - 08/21/2007
                '_InstalledTabs_PackageItems = New ArrayList
                '_InstalledModules_PackageItems = New ArrayList
                _InstalledTabs_PackageItems = New List(Of InstallController.PackageItem)
                _InstalledModules_PackageItems = New List(Of InstallController.PackageItem)
                InstallPackage()
            Catch ex As Exception
                If Not Errors Is Nothing Then
                    Errors.Add(ex.ToString)
                End If
            Finally
                If Not ThreadObject Is Nothing Then
                    _End()
                End If
                If Not _PackagePath Is Nothing AndAlso _PackagePath.Length > 0 Then
                    RemovePackageFolder()
                End If
            End Try
        End Sub
        Public Sub RemovePackageFolder()
            Dim sPath As String = _PackagePath
            Dim dinfo As New IO.DirectoryInfo(sPath)
            If dinfo.Exists Then
                dinfo.Delete(True)
            End If
        End Sub
        Private Sub _End()
            Dim identifier As String = SourcePackage.toString
            SourcePackage = Nothing
            Courier.Delivered(identifier)
        End Sub


        Private Sub InstallPackage()
            'READ THE CONTENTS OF THE PACKAGE
            'INSTALL THE PACKAGE ITEMS
            Dim pkC As New InstallController
            SourcePackage.PackageInfo.Status = 1
            SourcePackage.PackageInfo.StatusMessage = Nothing
            SourcePackage.PackageInfo.StatusDate = Now
            pkC.SetPackage(SourcePackage.PackageInfo)
            'ROMAIN: Generic replacement - 08/21/2007
            'Dim PackageErrors As New ArrayList
            Dim PackageErrors As New List(Of String)
            Dim PackageErrorCounter As Integer = 0
            Dim PackageItems As InstallController.PackageItem()

            PackageItems = pkC.GetPackageItems(SourcePackage.PackageID)
            Dim packItem As InstallController.PackageItem
            For Each packItem In PackageItems
                Try
                    Select Case packItem.ItemType.ToUpper
                        Case Common.ItemTypeResult.ResultTypeEnumerator.Configuration.ToString().ToUpper
                            If Config_Install(packItem, Me._Engine.UserInfo.UserName) = False Then
                                PackageErrorCounter += 1
                            End If
                        Case Common.ItemTypeResult.ResultTypeEnumerator.Install.ToString().ToUpper
                        Case Common.ItemTypeResult.ResultTypeEnumerator.Package.ToString().ToUpper
                        Case Common.ItemTypeResult.ResultTypeEnumerator.Resource.ToString().ToUpper
                            If File_Install(packItem) = False Then
                                PackageErrorCounter += 1
                            End If
                        Case Common.ItemTypeResult.ResultTypeEnumerator.Script.ToString().ToUpper
                            If Script_Install(packItem) = False Then
                                PackageErrorCounter += 1
                            End If
                        Case Common.ItemTypeResult.ResultTypeEnumerator.Tab.ToString().ToUpper
                            If Tab_Install(packItem) = False Then
                                PackageErrorCounter += 1
                            End If
                        Case Common.ItemTypeResult.ResultTypeEnumerator.Module.ToString().ToUpper
                            If Module_Install(packItem, _Engine.UserInfo.UserName) = False Then
                                PackageErrorCounter += 1
                            End If
                        Case Common.ItemTypeResult.ResultTypeEnumerator.Module.ToString().ToUpper & "-UNKNOWN"
                            PackageErrorCounter += 1
                    End Select
                Catch ex As Exception
                    PackageErrors.Add(ex.ToString)
                End Try
            Next
            If PackageErrors.Count = 0 And PackageErrorCounter = 0 Then
                'SUCCESS
                SourcePackage.PackageInfo.StatusMessage = "Success"
                SourcePackage.PackageInfo.Status = 2
                SourcePackage.PackageInfo.StatusDate = Now
                pkC.SetPackage(SourcePackage.PackageInfo)
            Else
                'FAILURE
                Dim result As String = ""
                If Not PackageErrors Is Nothing AndAlso PackageErrors.Count > 0 Then
                    Dim str As String
                    For Each str In PackageErrors
                        result &= "<li>" & str & vbCrLf
                    Next
                    Errors.Clear()
                    Errors = Nothing
                End If
                If PackageErrorCounter > 0 Then
                    If PackageErrorCounter > 1 Then
                        result &= "Multiple (" & PackageErrorCounter & ") Package Items failed to install properly." & vbCrLf
                    Else
                        result &= "One Package Item failed to install properly." & vbCrLf
                    End If
                End If
                SourcePackage.PackageInfo.StatusMessage = result
                SourcePackage.PackageInfo.Status = 3
                SourcePackage.PackageInfo.StatusDate = Now
                pkC.SetPackage(SourcePackage.PackageInfo)
            End If
        End Sub

        Private Sub StartPackageItem(ByRef Item As InstallController.PackageItem)
            Item.StatusMessage = Nothing
            Item.Status = 0
            'ROMAIN: Generic replacement - 08/21/2007
            'Errors = New ArrayList
            Errors = New List(Of String)
        End Sub
        Private Function EndPackageItem(ByRef Item As InstallController.PackageItem, ByVal DestinationID As Integer) As Boolean
            If Item.SourceID <= 0 Then
                Item.SourceID = DestinationID
            End If
            Item.DestinationID = DestinationID
            Item.StatusMessage = StatusMessage()
            If Item.StatusMessage Is Nothing Then
                Item.Status = 2
                Item.StatusMessage = "Success"
            Else
                Item.Status = 3
            End If
            Item.StatusDate = Now
            Dim xdc As New InstallController
            xdc.SetPackageItem(Item)

            If Item.Status = 2 Then
                Return True
            Else
                Return False
            End If
        End Function
        Private Function StatusMessage() As String
            Dim result As String = ""
            If Not Errors Is Nothing AndAlso Errors.Count > 0 Then
                Dim str As String
                For Each str In Errors
                    result &= "<li>" & str & vbCrLf
                Next
                Errors.Clear()
                Errors = Nothing
                Return result
            End If
            Return Nothing
        End Function
#Region "SQL Script Installation"
        Private Function Script_Load(ByRef Source As String) As Boolean
            Try
                If Not Source Is Nothing Then
                    'ROMAIN: 08/21/07
                    'Script_ReplaceOwner(Source)
                    AbstractFactory.Instance.DeliveryController.Script_ReplaceOwner(Source)
                    'Script_ReplaceQualifier(Source)
                    AbstractFactory.Instance.DeliveryController.Script_ReplaceQualifier(Source)
                    Source = _Engine.RenderString(Nothing, Source, Nothing, False, isPreRender:=False)
                    Return True
                Else
                    'execResult.Succeeded = False
                    'execResult.Result = "The Script File - " & Filename & " had no content."
                    'WriteDebugLine(execResult.Result)
                    Return False
                End If
            Catch ex As Exception
            End Try
            Return False
        End Function
        Private Function Script_Install(ByVal Item As InstallController.PackageItem) As Boolean
            StartPackageItem(Item)
            Try
                Dim Script As String = Common.ByteToString(Item.Content)
                'This check needs to be included because the unicode Byte Order mark results in an extra character at the start of the file
                'The extra character - '?' - causes an error with the database.
                If Script.StartsWith("?") Then
                    Script = Script.Substring(1)
                End If

                Dim canExecute As Boolean = Script_Load(Script)
                If canExecute Then
                    Dim xldc As New Controller

                    Dim strSQLExceptions As String = xldc.ExecuteScript(Nothing, Script, 9000)

                    If strSQLExceptions <> "" Then
                        Errors.Add(strSQLExceptions)
                    Else
                        Scripted = True
                    End If
                Else
                    Throw New Exception("Unabled to execute script, the script is poorly formatted or contains no data.")
                End If
            Catch ex As Exception
                Errors.Add(ex.ToString)
            End Try

            Return EndPackageItem(Item, Item.SequenceNumber * -1)
        End Function
        'ROMAIN: 09/21/07
        'NOTE: Moved to DNN CORE
        'Private Function Script_ReplaceOwner(ByRef Source As String)
        '    Dim ProviderType As String = "data"
        '    Dim objConfiguration As DotNetNuke.Framework.Providers.ProviderConfiguration = DotNetNuke.Framework.Providers.ProviderConfiguration.GetProviderConfiguration(ProviderType)
        '    Dim objProvider As DotNetNuke.Framework.Providers.Provider = CType(objConfiguration.Providers(objConfiguration.DefaultProvider), DotNetNuke.Framework.Providers.Provider)
        '    Dim strOwner As String

        '    strOwner = objProvider.Attributes("databaseOwner")
        '    If strOwner <> "" And strOwner.EndsWith(".") = False Then
        '        strOwner += "."
        '    End If

        '    Replacer(Source, "{databaseOwner}", strOwner)
        'End Function
        'Private Function Script_ReplaceQualifier(ByRef Source As String)
        '    Dim ProviderType As String = "data"
        '    Dim objConfiguration As DotNetNuke.Framework.Providers.ProviderConfiguration = DotNetNuke.Framework.Providers.ProviderConfiguration.GetProviderConfiguration(ProviderType)
        '    Dim objProvider As DotNetNuke.Framework.Providers.Provider = CType(objConfiguration.Providers(objConfiguration.DefaultProvider), DotNetNuke.Framework.Providers.Provider)
        '    Dim strQualifier As String

        '    strQualifier = objProvider.Attributes("objectQualifier")
        '    If strQualifier <> "" And strQualifier.EndsWith("_") = False Then
        '        strQualifier += "_"
        '    End If

        '    Replacer(Source, "{objectQualifier}", strQualifier)
        'End Function
        'Private Sub Replacer(ByRef source As String, ByVal replacing As String, ByVal replacement As String)
        '    Dim istart As Integer
        '    Dim starter As String = replacing
        '    istart = source.ToUpper.IndexOf(starter.ToUpper)

        '    While istart >= 0
        '        Dim xlength As Integer = (starter).Length
        '        Dim fvalue As String
        '        If Not replacement Is Nothing Then
        '            fvalue = replacement
        '        Else
        '            fvalue = ""
        '        End If
        '        source = source.Substring(0, istart) & fvalue & source.Substring(istart + xlength)
        '        If istart + 1 < source.Length Then
        '            istart = source.ToUpper.IndexOf(starter.ToUpper, istart + 1)
        '        Else
        '            istart = -1
        '        End If
        '    End While
        'End Sub
#End Region
#Region "Support File Installation"
        Public Function File_CreateDirectory(ByVal Target As String) As String
            'Dim strTarget As String = _Engine.Context.Server.MapPath(Target)
            Dim strTarget As String = System.IO.Path.Combine(_WebPath, Target)
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
        'Private Function File_Install(ByVal Filename As String, ByVal Target As String) As Boolean
        Private Function File_Install(ByVal Item As InstallController.PackageItem) As Boolean
            StartPackageItem(Item)

            Dim Target As String = Item.ItemPath
            'Target = _renderer.RenderString(Nothing, Target, Nothing, False, isPreRender:=False)

            Try
                If Not Item.Content Is Nothing AndAlso Item.Content.Length > 0 Then
                    'MOVE THE FILE
                    Dim tio As New IO.FileInfo(Target)
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
                    If tio.Exists Then
                        tio.Delete()
                    End If
                    Dim fileS As New IO.MemoryStream(Item.Content)
                    Dim destS As IO.FileStream = tio.Open(IO.FileMode.OpenOrCreate, IO.FileAccess.Write)
                    Common.StreamTransfer(CType(fileS, IO.Stream), CType(destS, IO.Stream))
                    Try
                        fileS.Close()
                    Catch ex As Exception
                    End Try
                    Try
                        destS.Close()
                    Catch ex As Exception
                    End Try
                    fileS = Nothing
                    destS = Nothing


                Else
                    Throw New Exception("The content is missing from the database, please try uploading the material again.")
                End If

            Catch ex As Exception
                Errors.Add(ex.ToString)
            End Try

            Return EndPackageItem(Item, Item.SequenceNumber * -1)
        End Function
#End Region
#Region "Configuration File Installation"
        Private Function Config_Install(ByVal Item As InstallController.PackageItem, ByVal Username As String) As Boolean
            StartPackageItem(Item)
            Try
                Dim Source As String = Common.ByteToString(Item.Content)
                Dim xset As New r2i.OWS.Controller
                'TODO: Needs to be updated for pageID, ModuleId
                xset.UpdateSetting(configurationId, Source, Username)
                Configured = True
            Catch ex As Exception
                Errors.Add(ex.ToString)
            End Try

            Return EndPackageItem(Item, Me.moduleId)
        End Function
        'Public Function Config_Install(ByVal Value As String, ByVal ModuleID As String, ByVal pageId As String) As Boolean
        Public Shared Function Config_Install(ByVal configurationId As Guid, ByVal Value As String, ByVal moduleID As String, ByVal pageId As String, ByVal Username As String) As Boolean
            Try
                Dim xset As New r2i.OWS.Controller
                'TODO: Needs to be updated for ConfigurationID
                'NOTE: If necessary pass the configId Guid As a parameter or get the configId By Name
                xset.UpdateSetting(configurationId, Value, Username)
                'xset.UpdateSetting(configurationId, moduleID, pageId, Value)
                Return True
            Catch ex As Exception
                Return False
            End Try
            Return False
        End Function
#End Region
#Region "Tab Installation"
        'ROMAIN: 09/21/07
        'Private Function ParseTab(ByVal SourceXML As System.Xml.XmlNode, ByRef Ti As DotNetNuke.Entities.Tabs.TabInfo, ByRef ViewRoles As ArrayList, ByRef EditRoles As ArrayList) As Boolean
        Private Function ParseTab(ByVal sourceXML As System.Xml.XmlNode, ByRef ti As ITabInfo, ByRef viewRoles As ArrayList, ByRef editRoles As ArrayList) As Boolean
            Try
                Dim prop As System.Reflection.PropertyInfo

                'For Each prop In ti.GetType().GetProperties()
                For Each prop In AbstractFactory.Instance.TabController.GetTabInfoProperties(ti)
                    Dim xmlnode As System.Xml.XmlNode = Common.CaseLess_SelectSingleNode(sourceXML, prop.Name)
                    If Not xmlnode Is Nothing Then
                        Dim tp As System.Type = prop.PropertyType
                        Dim currentProperty As System.Reflection.PropertyInfo
                        currentProperty = AbstractFactory.Instance.TabController.GetTabInfoProperty(ti, prop.Name)
                        If prop.CanWrite Then
                            Try

                                If currentProperty.PropertyType Is GetType(Integer) Then
                                    currentProperty.SetValue(ti, CType(xmlnode.InnerText, Int32), Nothing)
                                ElseIf currentProperty.PropertyType Is GetType(DateTime) Then
                                    currentProperty.SetValue(ti, CType(xmlnode.InnerText, DateTime), Nothing)
                                ElseIf currentProperty.PropertyType Is GetType(Boolean) Then
                                    currentProperty.SetValue(ti, CType(xmlnode.InnerText, Boolean), Nothing)
                                Else
                                    currentProperty.SetValue(ti, xmlnode.InnerText, Nothing)
                                End If
                            Catch ex As Exception
                            End Try
                        End If
                        Dim roleValue As String
                        Select Case prop.Name.ToUpper
                            Case AUTHORIZEDROLES
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
                                            viewRoles.Add(roleValue)
                                        End If
                                        'If Not sRole Is Nothing AndAlso sRole.Length > 0 AndAlso (pRoles.ContainsKey(sRole.ToUpper) OrElse sRole = DotNetNuke.Common.glbRoleAllUsersName OrElse sRole = DotNetNuke.Common.glbRoleUnauthUserName) Then
                                        '    Select Case sRole
                                        '        Case DotNetNuke.Common.glbRoleAllUsersName
                                        '            viewRoles.Add(DotNetNuke.Common.glbRoleAllUsers)
                                        '        Case DotNetNuke.Common.glbRoleUnauthUserName
                                        '            viewRoles.Add(DotNetNuke.Common.glbRoleUnauthUser)
                                        '        Case Else
                                        '            viewRoles.Add(pRoles(sRole.ToUpper))
                                        '    End Select
                                        'End If
                                    Next
                                End If
                            Case ADMINISTRATORROLES
                                Dim sRoles() As String = xmlnode.InnerText.Split(";"c)
                                If Not sRoles Is Nothing AndAlso sRoles.Length > 0 Then
                                    Dim sRole As String
                                    'ROMAIN: Generic replacement - 08/22/2007
                                    'Dim pRoles As SortedList = Common.GetRoles(PortalID)
                                    Dim pRoles As SortedList(Of String, String) = Common.GetRoles(PortalID)
                                    For Each sRole In sRoles
                                        roleValue = AbstractFactory.Instance.RoleController.GetCurrentRole(sRole, pRoles)
                                        If Not roleValue Is Nothing Then
                                            viewRoles.Add(roleValue)
                                        End If
                                        'If Not sRole Is Nothing AndAlso sRole.Length > 0 AndAlso (pRoles.ContainsKey(sRole.ToUpper) OrElse sRole = DotNetNuke.Common.glbRoleAllUsersName OrElse sRole = DotNetNuke.Common.glbRoleUnauthUserName) Then
                                        '    Select Case sRole
                                        '        Case DotNetNuke.Common.glbRoleAllUsersName
                                        '            editRoles.Add(DotNetNuke.Common.glbRoleAllUsers)
                                        '        Case DotNetNuke.Common.glbRoleUnauthUserName
                                        '            editRoles.Add(DotNetNuke.Common.glbRoleUnauthUser)
                                        '        Case Else
                                        '            editRoles.Add(pRoles(sRole.ToUpper))
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
        Private Function Tab_Install(ByVal Item As InstallController.PackageItem) As Boolean
            StartPackageItem(Item)
            Dim targetID As Integer = 0
            Try
                'ROMAIN: 09/21/07
                'Dim tbi As DotNetNuke.Entities.Tabs.TabInfo
                Dim tbi As ITabInfo
                Dim roles_View As New ArrayList
                Dim roles_Edit As New ArrayList
                'ROMAIN: 09/21/07
                'Dim tbc As New DotNetNuke.Entities.Tabs.TabController
                Dim tbc As ITabController = AbstractFactory.Instance.TabController
                If Item.DestinationID > 0 Then
                    'THIS ITEM ALREADY EXISTS - UPDATE
                    tbi = tbc.GetTab(Item.DestinationID)
                    If tbi Is Nothing OrElse tbi.PortalId <> Me.PortalID OrElse tbi.IsDeleted = True Then
                        'ROMAIN: 09/21/07
                        'tbi = New DotNetNuke.Entities.Tabs.TabInfo
                        tbi = AbstractFactory.Instance.TabController.CreateNewTabInfo()
                        tbi.TabId = -1
                    End If
                    Dim xTabID As Integer = tbi.TabId
                    ParseTab(Common.ByteToXMLDocument(Item.Content), tbi, roles_View, roles_Edit)
                    tbi.TabId = xTabID
                Else
                    'tbi = New DotNetNuke.Entities.Tabs.TabInfo
                    tbi = AbstractFactory.Instance.TabController.CreateNewTabInfo()
                    ParseTab(Common.ByteToXMLDocument(Item.Content), tbi, roles_View, roles_Edit)
                    tbi.TabId = -1
                End If
                If Not tbi Is Nothing Then
                    tbi.PortalId = Me.PortalID

                    Dim bexisted As Boolean = True
                    If tbi.TabId > 0 Then
                        bexisted = True
                    Else
                        bexisted = False
                        Dim parentNode As XmlNode = Common.CaseLess_SelectSingleNode(Common.ByteToXMLDocument(Item.Content), "ParentTabName")
                        If Not parentNode Is Nothing Then
                            'ROMAIN: 09/21/07
                            'Dim tbParent As DotNetNuke.Entities.Tabs.TabInfo
                            Dim tbParent As ITabInfo
                            tbParent = tbc.GetTabByName(parentNode.InnerXml, Me.PortalID)
                            If Not tbParent Is Nothing Then
                                If Me.SourcePackage.PackageInfo.MatchTabName Then
                                    'Dim tbi_found As DotNetNuke.Entities.Tabs.TabInfo = tbc.GetTabByName(tbi.TabName, Me.PortalID, tbParent.TabID)
                                    Dim tbi_found As ITabInfo = tbc.GetTabByName(tbi.TabName, Me.PortalID, tbParent.TabId)
                                    If Not tbi_found Is Nothing AndAlso tbi_found.TabId > 0 Then
                                        bexisted = True
                                        tbi.TabId = tbi_found.TabId
                                    End If
                                End If
                                tbi.ParentId = tbParent.TabId
                            End If
                        Else
                            If Me.SourcePackage.PackageInfo.MatchTabName Then
                                'Dim tbi_found As DotNetNuke.Entities.Tabs.TabInfo = tbc.GetTabByName(tbi.TabName, Me.PortalID, -1)
                                Dim tbi_found As ITabInfo = tbc.GetTabByName(tbi.TabName, Me.PortalID, -1)
                                If Not tbi_found Is Nothing AndAlso tbi_found.TabId > 0 Then
                                    bexisted = True
                                    tbi.TabId = tbi_found.TabId
                                End If
                            End If
                        End If
                    End If

                    ' REG - 1.9.8+ 
                    ' - First, we insert the new tab.  Disregard permissions here.
                    If Not bexisted Then
                        tbi.TabId = tbc.AddTab(tbi)
                        bexisted = True
                    End If

                    'SET PERMISSIONS

                    'ROMAIN: 09/21/07
                    'Dim tpI As New DotNetNuke.Security.Permissions.TabPermissionCollection
                    Dim tpI As ITabPermissionCollection
                    'If Not bexisted Then
                    'If tbi.TabID > 0 Then
                    'Dim tS As New DotNetNuke.Security.Permissions.TabPermissionController
                    Dim tpCtrl As ITabPermissionController = AbstractFactory.Instance.TabPermissionController
                    '    tS.DeleteTabPermissionsByTabID(tbi.TabID)
                    'End If
                    ' REG - 1.9.8+
                    ' - Then, we populate the collection, and check before each add.
                    'tpI = tpCtrl.GetTabPermissionsCollectionByTabID(tbi.TabID)
                    tpI = tpCtrl.GetTabPermissionsCollectionByTabID(tbi.PortalID, tbi.TabID)

                    If (Not roles_View Is Nothing AndAlso roles_View.Count > 0) OrElse (Not roles_Edit Is Nothing AndAlso roles_Edit.Count > 0) Then
                        If Not roles_View Is Nothing AndAlso roles_View.Count > 0 Then
                            Dim roleID As Integer
                            For Each roleID In roles_View
                                'ROMAIN: 09/21/07
                                'Dim mP As New DotNetNuke.Security.Permissions.TabPermissionInfo
                                Dim mP As ITabPermissionInfo = AbstractFactory.Instance.TabPermissionController.CreateNewTabPermissionInfo()
                                mP.PermissionID = 3
                                mP.PermissionKey = "VIEW"
                                mP.PermissionName = "View Tab"
                                mP.RoleID = roleID
                                'mP.RoleName = rolename
                                mP.AllowAccess = True
                                mP.TabID = tbi.TabId
                                'mP.ModuleDefID = -1
                                mP.PermissionCode = "SYSTEM_TAB"
                                '       tS.AddTabPermission(mp)
                                If Not HasPermission(tpI, roleID) Then tpI.Add(mP)
                            Next
                        End If
                        If Not roles_Edit Is Nothing AndAlso roles_Edit.Count > 0 Then
                            Dim roleID As Integer
                            For Each roleID In roles_Edit
                                'Dim mP As New DotNetNuke.Security.Permissions.TabPermissionInfo
                                Dim mP As ITabPermissionInfo = AbstractFactory.Instance.TabPermissionController.CreateNewTabPermissionInfo()
                                mP.PermissionID = 4
                                mP.PermissionKey = "EDIT"
                                mP.PermissionName = "Edit Tab"
                                mP.RoleID = roleID
                                'mP.RoleName = rolename
                                mP.AllowAccess = True
                                mP.TabID = tbi.TabId
                                'mP.ModuleDefID = -1
                                mP.PermissionCode = "SYSTEM_TAB"
                                'tS.AddTabPermission(mp)
                                If Not HasPermission(tpI, roleID) Then tpI.Add(mP)
                            Next
                        End If
                    End If
                    If roles_View Is Nothing OrElse Not roles_View.Contains(PortalSettings_AdminRoleName) Then
                        'Dim mP As New DotNetNuke.Security.Permissions.TabPermissionInfo
                        Dim mP As ITabPermissionInfo = AbstractFactory.Instance.TabPermissionController.CreateNewTabPermissionInfo()
                        mP.PermissionID = 3
                        mP.PermissionKey = "VIEW"
                        mP.PermissionName = "View Tab"
                        mP.RoleID = PortalSettings_AdminRoleId
                        'mP.RoleName = PortalSettings.AdministratorRoleName
                        mP.AllowAccess = True
                        mP.TabID = tbi.TabId
                        'mP.ModuleDefID = -1
                        mP.PermissionCode = "SYSTEM_TAB"
                        '       tS.AddTabPermission(mp)
                        If Not HasPermission(tpI, PortalSettings_AdminRoleId) Then tpI.Add(mP)
                    End If
                    'End If
                    tbi.TabPermissions = tpI

                    If bexisted Then
                        tbc.UpdateTab(tbi)
                    Else
                        tbi.TabId = tbc.AddTab(tbi)
                    End If

                    targetID = tbi.TabId
                Else
                    Throw New Exception("Tab definition does not appear to be a properly formatted structure.")
                End If
            Catch ex As Exception
                Errors.Add(ex.ToString)
            End Try

            Return EndPackageItem(Item, targetID)
        End Function
#End Region
#Region "Module Installation"
        'Private Function ParseModule(ByVal SourceXML As System.Xml.XmlNode, ByRef Mi As DotNetNuke.Entities.Modules.ModuleInfo, ByRef ViewRoles As ArrayList, ByRef EditRoles As ArrayList, ByRef Settings As Hashtable, ByRef Source As String) As Boolean
        Private Function ParseModule(ByVal SourceXML As System.Xml.XmlNode, ByRef mi As IModuleInfo, ByRef ViewRoles As ArrayList, ByRef EditRoles As ArrayList, ByRef Settings As Hashtable, ByRef Source As String) As Boolean
            Try
                Dim prop As System.Reflection.PropertyInfo
                'For Each prop In Mi.GetType().GetProperties()
                For Each prop In AbstractFactory.Instance.ModuleController.GetModuleInfoProperties(mi)
                    Dim xmlnode As System.Xml.XmlNode = Common.CaseLess_SelectSingleNode(SourceXML, prop.Name)
                    If Not xmlnode Is Nothing Then
                        Dim tp As System.Type = prop.PropertyType
                        Dim currentProperty As System.Reflection.PropertyInfo
                        currentProperty = AbstractFactory.Instance.ModuleController.GetModuleInfoProperty(mi, prop.Name)
                        If prop.CanWrite Then
                            Try

                                If currentProperty.PropertyType Is GetType(Integer) Then
                                    currentProperty.SetValue(mi, CType(xmlnode.InnerText, Int32), Nothing)
                                ElseIf currentProperty.PropertyType Is GetType(DateTime) Then
                                    currentProperty.SetValue(mi, CType(xmlnode.InnerText, DateTime), Nothing)
                                ElseIf currentProperty.PropertyType Is GetType(Boolean) Then
                                    currentProperty.SetValue(mi, CType(xmlnode.InnerText, Boolean), Nothing)
                                Else
                                    currentProperty.SetValue(mi, xmlnode.InnerText, Nothing)
                                End If
                            Catch ex As Exception
                            End Try
                        End If
                        Dim roleValue As String
                        Select Case prop.Name.ToUpper
                            Case "AUTHORIZEDVIEWROLES"
                                If xmlnode.InnerText.Length > 1 Then
                                    Dim sRoles() As String = xmlnode.InnerText.Split(";"c)
                                    If Not sRoles Is Nothing AndAlso sRoles.Length > 0 Then
                                        Dim sRole As String
                                        'ROMAIN: Generic replacement - 08/22/2007
                                        'Dim pRoles As SortedList = Common.GetRoles(PortalID)
                                        Dim pRoles As SortedList(Of String, String) = Common.GetRoles(PortalID)
                                        'ROMAIN: 08/22/07

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
                                End If
                            Case "AUTHORIZEDEDITROLES"
                                If xmlnode.InnerText.Length > 1 Then
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
        Private Function Module_Install(ByVal Item As InstallController.PackageItem, ByVal Username As String) As Boolean
            StartPackageItem(Item)
            Dim targetID As Integer = 0
            Try
                'ROMAIN: Generic replacement - 08/21/2007
                Dim roles_View As New ArrayList
                Dim roles_Edit As New ArrayList
                'NOTE: roles_View and roles_Edit contains both integer and string
                'Dim roles_View As New List(Of Integer) 
                'Dim roles_Edit As New List(Of Integer)
                Dim parentTabID As Integer = -1
                'ROMAIN: 09/21/07
                'Dim mdi As New DotNetNuke.Entities.Modules.ModuleInfo
                Dim mdi As IModuleInfo = AbstractFactory.Instance.ModuleController.CreateNewModuleInfo()
                Dim Settings As New Hashtable
                Dim contentsource As String = Nothing
                Dim sourceXML As Xml.XmlNode = Common.ByteToXMLDocument(Item.Content)
                Dim useExisting As Boolean = False

                Dim xtabpackage As New InstallController
                Dim ptabpackage As InstallController.PackageItem = xtabpackage.GetPackageItem(Item.PackageID, Item.ParentPackageItemID, -1, Nothing) ' Common.ItemTypeResult.ResultTypeEnumerator.Tab.ToString)
                If Not ptabpackage Is Nothing Then
                    parentTabID = ptabpackage.DestinationID
                End If

                'Dim mdC As New DotNetNuke.Entities.Modules.ModuleController
                Dim mdC As IModuleController = AbstractFactory.Instance.ModuleController
                If Item.DestinationID > 0 Then
                    'THIS ITEM ALREADY EXISTS - UPDATE
                    mdi = mdC.GetModule(Item.DestinationID, parentTabID)
                    If Not mdi Is Nothing AndAlso mdi.PortalId = Me.PortalID AndAlso mdi.IsDeleted = False Then
                        useExisting = True
                    Else
                        Item.DestinationID = 0
                        'mdi = New DotNetNuke.Entities.Modules.ModuleInfo
                        mdi = AbstractFactory.Instance.ModuleController.CreateNewModuleInfo()
                    End If
                Else
                    'THIS IS A NEW ITEM - CREATE
                    'mdi = New DotNetNuke.Entities.Modules.ModuleInfo
                    mdi = AbstractFactory.Instance.ModuleController.CreateNewModuleInfo()
                End If

                'PARSE THE MODULE INFORMATION
                Dim xModuleID As Integer = mdi.ModuleID
                Dim xTabID As Integer = mdi.TabId
                ParseModule(sourceXML, mdi, roles_View, roles_Edit, Settings, contentsource)

                If Not useExisting Then
                    mdi.ModuleID = -1
                Else
                    mdi.ModuleID = xModuleID
                    mdi.TabId = xTabID
                End If


                'UPDATE THE MODULE INFO

                Dim TargetDefType As String = Nothing
                Dim TargetDefID As Integer = Common.GetModuleDefinition(sourceXML, TargetDefType)
                Dim moduleInstallation As Integer = 0

                If TargetDefID > 0 Then
                    If mdi.ModuleID <= 0 Then
                        mdi.ModuleDefId = TargetDefID
                        mdi.TabId = parentTabID
                        mdi.PortalId = Me.PortalID
                        Item.DestinationID = mdi.ModuleID
                        moduleInstallation = 2
                    Else
                        Item.DestinationID = mdi.ModuleID
                        moduleInstallation = 1
                    End If
                End If


                'UPDATE HEADER/FOOTER MODULE SETTINGS
                Dim strHeader As String = mdi.Header
                Dim strFooter As String = mdi.Footer
                Dim HFChanged As Boolean = False
                Common.ReplaceSourceValues(strHeader, _InstalledTabs_PackageItems, _InstalledModules_PackageItems, Nothing)

                If Not strHeader = mdi.Header Then
                    HFChanged = True
                    mdi.Header = strHeader
                End If
                If Not strFooter = mdi.Footer Then
                    HFChanged = True
                    mdi.Footer = strFooter
                End If

                If HFChanged Then
                    mdC.UpdateModule(mdi)
                End If

                'UPDATE THE MODULE PERMISSIONS
                If moduleInstallation > 0 Then
                    ' REG - 1.9.8+ 
                    ' - First, we insert the new module.  Screw permissions here.
                    If moduleInstallation = 2 Then
                        mdi.ModuleID = mdC.AddModule(mdi)
                        moduleInstallation = 1
                    End If

                    'ROMAIN: 09/21/07
                    'Dim mPI As New DotNetNuke.Security.Permissions.ModulePermissionCollection
                    'Dim mP As DotNetNuke.Security.Permissions.ModulePermissionInfo
                    'TODO: Create Class!!
                    'Dim mPI As New DotNetNuke.Security.Permissions.ModulePermissionCollection
                    'ROMAIN: 09/27/07
                    'Dim mP As DotNetNuke.Security.Permissions.ModulePermissionInfo
                    Dim mP As IModulePermissionInfo

                    'If mdi.ModuleId > 0 Then
                    'Dim mS As New DotNetNuke.Security.Permissions.ModulePermissionController
                    Dim mS As IModulePermissionController = AbstractFactory.Instance.ModulePermissionController
                    ' REG - 1.9.8+
                    ' - Then, we populate the collection, and check before each add.
                    Dim mPI As IModulePermissionCollection
                    mPI = mS.GetModulePermissionsCollectionByModuleID(mdi.ModuleID)

                    'mS.DeleteModulePermissionsByModuleID(mdi.ModuleId)
                    'End If
                    If (Not roles_View Is Nothing AndAlso roles_View.Count > 0) OrElse (Not roles_Edit Is Nothing AndAlso roles_Edit.Count > 0) Then
                        'Dim mS As New DotNetNuke.Security.Permissions.ModulePermissionController
                        'mS.DeleteModulePermissionsByModuleID(mdi.ModuleId)
                        If Not roles_View Is Nothing AndAlso roles_View.Count > 0 Then
                            Dim roleid As Integer
                            For Each roleid In roles_View
                                'mP = New DotNetNuke.Security.Permissions.ModulePermissionInfo
                                mP = AbstractFactory.Instance.ModulePermissionController.CreateNewModulePermissionInfo()
                                mP.PermissionId = 1
                                mP.PermissionKey = "VIEW"
                                mP.PermissionName = "View"

                                mP.RoleId = roleid
                                'mP.RoleName = rolename
                                mP.AllowAccess = True
                                mP.ModuleId = mdi.ModuleID
                                'mP.ModuleDefID = -1
                                mP.PermissionCode = "SYSTEM_MODULE_DEFINITION"
                                '       mS.AddModulePermission(mp)

                                If Not HasPermission(mPI, roleid) Then mPI.Add(mP)
                            Next
                        End If
                        If Not roles_Edit Is Nothing AndAlso roles_Edit.Count > 0 Then
                            Dim roleid As Integer
                            'Dim rolename As String
                            For Each roleid In roles_Edit
                                'mP = New DotNetNuke.Security.Permissions.ModulePermissionInfo
                                mP = AbstractFactory.Instance.ModulePermissionController.CreateNewModulePermissionInfo()
                                mP.PermissionId = 2
                                mP.PermissionKey = "EDIT"
                                mP.PermissionName = "Edit"

                                mP.RoleId = roleid
                                'mP.RoleName = rolename

                                mP.AllowAccess = True
                                mP.ModuleId = mdi.ModuleID
                                'mP.ModuleDefID = -1
                                mP.PermissionCode = "SYSTEM_MODULE_DEFINITION"
                                'mS.AddModulePermission(mp)
                                If Not HasPermission(mPI, roleid) Then mPI.Add(mP)
                            Next
                        End If
                    End If
                    If Not roles_View.Contains(PortalSettings_AdminRoleName) AndAlso mdi.InheritViewPermissions Then
                        'ROMAIN: 09/21/07
                        'mP = New DotNetNuke.Security.Permissions.ModulePermissionInfo
                        mP = AbstractFactory.Instance.ModulePermissionController.CreateNewModulePermissionInfo()
                        mP.PermissionId = 1
                        mP.PermissionKey = "VIEW"
                        mP.PermissionName = "View"

                        mP.RoleId = PortalSettings_AdminRoleId
                        'mP.RoleName = PortalSettings.AdministratorRoleName
                        mP.AllowAccess = True
                        mP.ModuleId = mdi.ModuleID
                        'mP.ModuleDefID = -1
                        mP.PermissionCode = "SYSTEM_MODULE_DEFINITION"
                        '       mS.AddModulePermission(mp)
                        If Not HasPermission(mPI, PortalSettings_AdminRoleId) Then mPI.Add(mP)
                    End If
                    mdi.ModulePermissions = mPI
                End If

                If moduleInstallation = 2 Then
                    mdi.ModuleID = mdC.AddModule(mdi)
                Else
                    mdC.UpdateModule(mdi)
                End If
                targetID = mdi.ModuleID


                'UPDATE THE MODULE SETTINGS
                If mdi.ModuleID > 0 Then
                    Dim key As String
                    For Each key In Settings.Keys
                        If Not Settings(key) Is Nothing Then
                            Try
                                mdC.UpdateModuleSetting(mdi.ModuleID, key.ToString, Settings(key))
                            Catch ex As Exception
                            End Try
                        End If
                    Next
                End If


                'INSTALL THE MODULE CONFIGURATION
                If mdi.ModuleID > 0 Then
                    Dim rslt As String = AbstractFactory.Instance.DesignController.ImportModule(mdi, 0, contentsource)
                    If Not rslt Is Nothing AndAlso Not rslt.ToUpper = "SUCCESS" Then
                        Throw New Exception(rslt)
                    End If
                End If

            Catch ex As Exception
                Errors.Add(ex.ToString)
            End Try

            Return EndPackageItem(Item, targetID)
        End Function
        'ROMAIN: 09/22/07
        'Private Function HasPermission(ByVal PermissionCollection As DotNetNuke.Security.Permissions.ModulePermissionCollection, ByVal RoleID As Integer) As Boolean
        '    Dim mP As DotNetNuke.Security.Permissions.ModulePermissionInfo
        '    Dim bFound As Boolean = False

        '    If Not PermissionCollection Is Nothing Then
        '        For Each mP In PermissionCollection
        '            If mP.RoleID = RoleID Then
        '                bFound = True
        '                Exit For
        '            End If
        '        Next
        '    End If

        '    Return bFound
        'End Function
        Private Function HasPermission(ByVal PermissionCollection As IModulePermissionCollection, ByVal RoleID As Integer) As Boolean
            Dim mP As IModulePermissionInfo
            Dim bFound As Boolean = False

            If Not PermissionCollection Is Nothing Then
                Dim i As Integer = 0
                For i = 0 To PermissionCollection.Count() - 1
                    mP = PermissionCollection(i)
                    If mP.RoleID = RoleID Then
                        bFound = True
                        Exit For
                    End If
                Next
            End If

            Return bFound
        End Function
        'Private Function HasPermission(ByVal PermissionCollection As DotNetNuke.Security.Permissions.TabPermissionCollection, ByVal RoleID As Integer) As Boolean
        '    Dim tP As DotNetNuke.Security.Permissions.TabPermissionInfo
        '    Dim bFound As Boolean = False

        '    If Not PermissionCollection Is Nothing Then
        '        For Each tP In PermissionCollection
        '            If tP.RoleID = RoleID Then
        '                bFound = True
        '                Exit For
        '            End If
        '        Next
        '    End If

        '    Return bFound
        'End Function
        Private Function HasPermission(ByVal PermissionCollection As ITabPermissionCollection, ByVal RoleID As Integer) As Boolean
            Dim tP As ITabPermissionInfo
            Dim bFound As Boolean = False

            If Not PermissionCollection Is Nothing Then
                Dim i As Integer = 0
                For i = 0 To PermissionCollection.Count() - 1
                    tP = PermissionCollection(i)
                    If tP.RoleID = RoleID Then
                        bFound = True
                        Exit For
                    End If
                Next
            End If

            Return bFound
        End Function
#End Region
    End Class
End Namespace
