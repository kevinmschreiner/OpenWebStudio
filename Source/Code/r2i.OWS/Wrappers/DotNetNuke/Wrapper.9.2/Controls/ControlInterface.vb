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
Imports System.Web
Imports r2i.OWS.Wrapper.DNN.Entities
Imports r2i.OWS.Framework.DataAccess
Imports r2i.OWS.Framework.Entities
Imports r2i.OWS.Framework.Utilities
Imports r2i.OWS.Framework.Utilities.Compatibility

Public Class ControlInterface
    Implements DotNetNuke.Entities.Modules.ISearchable
    Implements DotNetNuke.Entities.Modules.IPortable    
    Private Function BuildPortalSettings(ByVal PortalID As Integer, ByVal TabID As Integer, ByVal ModuleID As Integer) As DotNetNuke.Entities.Portals.PortalSettings
        Dim psettings As New DotNetNuke.Entities.Portals.PortalSettings
        Dim pinfo As DotNetNuke.Entities.Portals.PortalInfo
        Dim tinfo As DotNetNuke.Entities.Tabs.TabInfo
        Dim pctrl As New DotNetNuke.Entities.Portals.PortalController
        Dim pactrl As New DotNetNuke.Entities.Portals.PortalAliasController
        Dim tctrl As New DotNetNuke.Entities.Tabs.TabController
        tinfo = tctrl.GetTab(TabID)
        pinfo = pctrl.GetPortal(PortalID)
        With psettings
            .ActiveTab = tinfo
            .AdministratorId = pinfo.AdministratorId
            .AdministratorRoleId = pinfo.AdministratorRoleId
            .AdministratorRoleName = pinfo.AdministratorRoleName
            .AdminTabId = pinfo.AdminTabId
            .BackgroundFile = pinfo.BackgroundFile
            .BannerAdvertising = pinfo.BannerAdvertising
            .Currency = pinfo.Currency
            .DefaultLanguage = pinfo.DefaultLanguage
            .Description = pinfo.DefaultLanguage
            '.DesktopTabs = New ArrayList
            .Email = pinfo.Email
            .ExpiryDate = pinfo.ExpiryDate
            .FooterText = pinfo.FooterText
            .HomeDirectory = pinfo.HomeDirectory
            .HomeTabId = pinfo.HomeTabId
            .HostFee = pinfo.HostFee
            .HostSpace = pinfo.HostSpace
            .KeyWords = pinfo.KeyWords
            .LoginTabId = pinfo.LoginTabId
            .LogoFile = pinfo.LogoFile
            .PortalAlias = pactrl.GetPortalAliasByPortalID(PortalID).Item(CStr(0))
            .PortalId = pinfo.PortalID
            .PortalName = pinfo.PortalName
            .RegisteredRoleId = pinfo.RegisteredRoleId
            .RegisteredRoleName = pinfo.RegisteredRoleName
            .SiteLogHistory = pinfo.SiteLogHistory
            .SplashTabId = pinfo.SplashTabId
            .SuperTabId = pinfo.SuperTabId
            '.TimeZoneOffset = pinfo.TimeZoneOffset
            .UserRegistration = pinfo.UserRegistration
            .UserTabId = pinfo.UserTabId
            '.Version = pinfo.Version
        End With
        Return psettings
    End Function


    Private _debugOutput As r2i.OWS.Framework.Debugger
    Public Sub Debugger_Open(ByRef Configuration As Settings, ByVal ModuleID As String, ByVal Title As String, ByRef userInfo As IUser, ByRef portalSettings As IPortalSettings, ByVal isEditable As Boolean, ByVal createDebug As Boolean, ByVal forceDebug As Boolean)
        If forceDebug OrElse Configuration.canDebug(userInfo, portalSettings, isEditable) Then
            If createDebug Then
                _debugOutput = New r2i.OWS.Framework.Debugger
                _debugOutput.AppendStamp(Title)
            End If
        End If
        If Not _debugOutput Is Nothing Then
            _debugOutput.AppendHeader(ModuleID, "Actions", "actions", False)
        End If
    End Sub
    Public Sub Debugger_Close(ByRef Configuration As Settings, ByVal Name As String, ByRef _engine As Engine, ByVal isEditable As Boolean, ByVal ForceDebug As Boolean)
        Try
            If Not _debugOutput Is Nothing Then
                _debugOutput.AppendFooter(True)
                If ForceDebug OrElse Configuration.canDebug(_engine.UserInfo, _engine.PortalSettings, isEditable) Then
                    Controller.AddLog(Configuration.ConfigurationID, _engine.UserInfo.UserId, "Debug - " & Name, _debugOutput.ToString, _engine.Session.SessionID)
                End If
                _debugOutput.Close()
                _debugOutput.Dispose()
            End If
        Catch ex As Exception
        End Try
    End Sub

    Public Sub GetImport(ByVal ModuleID As Integer, ByVal Content As String, ByVal Version As String, ByVal UserID As Integer)
        Dim sb As New Text.StringBuilder
        Dim tw As New IO.StringWriter(sb)
        Dim renderingEngine As r2i.OWS.Engine = Nothing
        Dim conf As Settings = Nothing
        Dim isEditable As Boolean = True
        Try
            'LOAD THE CONFIGURATION
            Dim xmlDoc As New Xml.XmlDocument
            xmlDoc.LoadXml("<root>" & Content & "</root>")
            Dim xmlCustom As System.Xml.XmlNode = xmlDoc.SelectSingleNode("/root/custom")
            If Not xmlCustom Is Nothing Then
                Dim extractCustom As String = "<root>" & xmlCustom.InnerText & "</root>"
                xmlDoc.LoadXml(extractCustom)
            End If
            Dim xmlData As System.Xml.XmlNode = xmlDoc.SelectSingleNode("/root/data")
            Dim xmlVariables As System.Xml.XmlNode = xmlDoc.SelectSingleNode("/root/variables")
            Dim modInfo As DotNetNuke.Entities.Modules.ModuleInfo = Nothing
            Try
                Dim mC As New DotNetNuke.Entities.Modules.ModuleController
                modInfo = mC.GetModule(ModuleID, -1)

                conf = GetConfiguration(modInfo)
            Catch ex As Exception

            End Try
            If Not conf Is Nothing Then
                If Not conf Is Nothing Then
                    Dim mctrl As New DotNetNuke.Entities.Modules.ModuleController
                    Dim pctrl As New DotNetNuke.Entities.Portals.PortalController
                    Dim pactrl As New DotNetNuke.Entities.Portals.PortalAliasController
                    Dim piCol As DotNetNuke.Entities.Portals.PortalAliasCollection = pactrl.GetPortalAliasByPortalID(modInfo.PortalID)
                    Dim pi As DotNetNuke.Entities.Portals.PortalAliasInfo = Nothing
                    Try
                        If piCol.HasKeys Then
                            pi = piCol.Item(CType(New ArrayList(piCol.Keys).Item(0), String))
                        End If
                        If pi Is Nothing Then
                            pi = New DotNetNuke.Entities.Portals.PortalAliasInfo
                            pi.PortalID = modInfo.PortalID
                        End If
                    Catch ex As Exception

                    End Try
                    Dim pinfo As DotNetNuke.Entities.Portals.PortalInfo
                    Dim psettings As DotNetNuke.Entities.Portals.PortalSettings
                    pinfo = pctrl.GetPortal(modInfo.PortalID)

                    Try
                        If pinfo.HomeTabId <= 0 And pinfo.SplashTabId <= 0 Then
                            psettings = New DotNetNuke.Entities.Portals.PortalSettings(modInfo.TabID, pi)
                        ElseIf pinfo.HomeTabId <= 0 Then
                            psettings = New DotNetNuke.Entities.Portals.PortalSettings(pinfo.SplashTabId, pi)
                        Else
                            psettings = New DotNetNuke.Entities.Portals.PortalSettings(pinfo.HomeTabId, pi)
                        End If
                    Catch ex As Exception
                        Try
                            psettings = BuildPortalSettings(modInfo.PortalID, modInfo.TabID, modInfo.ModuleID)
                        Catch ex2 As Exception
                            psettings = New DotNetNuke.Entities.Portals.PortalSettings
                            psettings.PortalId = modInfo.PortalID
                        End Try
                    End Try
                    Dim ipsettings As New DataAccess.PortalSettings(psettings)

                    Dim httpS As New HttpContext(New HttpRequest("IMPORT", "http://www.openwebstudio.com", ""), New HttpResponse(New System.IO.StringWriter))
                    Dim esession As New GenericSession()
                    Dim ctl As New Framework.UI.Control
                    ctl.ID = "Import"

                    Dim settings As Hashtable
                    settings = mctrl.GetModuleSettings(modInfo.ModuleID)

                    'TRIGGER THE DEBUG START
                    If conf.enableQueryDebug OrElse (conf.canDebug(AbstractFactory.Instance.UserController.CurrentUser(), CType(ipsettings, IPortalSettings), isEditable) And conf.OutputType = r2i.OWS.Framework.Settings.RenderType.Default) Then
                        Debugger_Open(conf, modInfo.ModuleID.ToString, "iPortable - Import", AbstractFactory.Instance.UserController.CurrentUser(), CType(ipsettings, IPortalSettings), isEditable, True, conf.enableQueryDebug)
                    End If
                    If (conf.enableQueryDebug OrElse conf.enableQueryDebug_Log) AndAlso _debugOutput Is Nothing Then
                        Debugger_Open(conf, modInfo.ModuleID.ToString, "iPortable - Import", AbstractFactory.Instance.UserController.CurrentUser(), CType(ipsettings, IPortalSettings), isEditable, False, conf.enableQueryDebug)
                    End If

                    'CREATE THE ENGINE
                    renderingEngine = New r2i.OWS.Engine(httpS, esession, ctl, False, AbstractFactory.Instance.UserController.CurrentUser(), New System.Web.UI.StateBag, settings, CType(ipsettings, IPortalSettings), Nothing, modInfo.ModuleID.ToString(), modInfo.TabID.ToString(), modInfo.TabModuleID.ToString(), ConfigurationID(modInfo.ModuleID), conf, "", "", _debugOutput, True)
                    renderingEngine.RequestType = EngineBase.RequestTypeEnum.Import

                    If Not xmlVariables Is Nothing Then
                        Dim dt As New DataTable("Action.Variables")
                        dt.Columns.Add("key")
                        dt.Columns.Add("value")

                        Dim tr As New IO.StringReader(xmlVariables.InnerXml)
                        dt.ReadXml(tr)
                        If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
                            Dim dr As DataRow
                            For Each dr In dt.Rows
                                renderingEngine.ActionVariable(CType(dr("key"), String)) = dr("value")
                            Next
                        End If
                    End If
                    Dim sharedds As DataSet = Nothing
                    If Not xmlData Is Nothing Then
                        Try
                            Dim ds As DataSet = New DataSet()
                            Dim tr As New IO.StringReader(xmlData.InnerXml)
                            'ds.ReadXml(tr)
                            Dim ser As New Xml.Serialization.XmlSerializer(ds.GetType)
                            Dim obj As Object = ser.Deserialize(tr)
                            If Not obj Is Nothing Then
                                ds = CType(obj, DataSet)
                            End If
                            sharedds = ds
                        Catch ex As Exception
                        End Try

                    End If

                    renderingEngine.ExecuteActions(sharedds, "", "", _debugOutput)
                End If
            End If
        Catch ex As Exception
            'DotNetNuke.Services.Exceptions.LogException(New Exception("Error Occured in Module " & ModInfo.ModuleID & " while attempting to handle iSearchable: " & ex.ToString))
        Finally
            'CLOSE THE DEBUGGER
            If Not _debugOutput Is Nothing Then
                Try
                    Debugger_Close(conf, "IMPORT", renderingEngine, isEditable, conf.enableQueryDebug)
                Catch ex As Exception
                End Try
            End If
        End Try
    End Sub
    Public Function GetExport(ByVal ModuleId As Integer) As String
        Dim sb As New Text.StringBuilder
        Dim tw As New IO.StringWriter(sb)
        Dim renderingEngine As r2i.OWS.Engine = Nothing
        Dim conf As Settings = Nothing
        Dim isEditable As Boolean = True
        Try
            Dim modInfo As DotNetNuke.Entities.Modules.ModuleInfo = Nothing
            Try
                Dim mC As New DotNetNuke.Entities.Modules.ModuleController
                modInfo = mC.GetModule(ModuleId, -1)

                conf = GetConfiguration(modInfo)
            Catch ex As Exception

            End Try

            If Not conf Is Nothing Then
                If Not conf Is Nothing Then
                    Dim mctrl As New DotNetNuke.Entities.Modules.ModuleController
                    Dim pctrl As New DotNetNuke.Entities.Portals.PortalController
                    Dim pactrl As New DotNetNuke.Entities.Portals.PortalAliasController
                    Dim piCol As DotNetNuke.Entities.Portals.PortalAliasCollection = pactrl.GetPortalAliasByPortalID(ModInfo.PortalID)
                    Dim pi As DotNetNuke.Entities.Portals.PortalAliasInfo = piCol.Item(CType(New ArrayList(piCol.Keys).Item(0), String))
                    Dim pinfo As DotNetNuke.Entities.Portals.PortalInfo
                    Dim psettings As DotNetNuke.Entities.Portals.PortalSettings
                    pinfo = pctrl.GetPortal(ModInfo.PortalID)

                    Try
                        If pinfo.HomeTabId <= 0 And pinfo.SplashTabId <= 0 Then
                            psettings = New DotNetNuke.Entities.Portals.PortalSettings(ModInfo.TabID, pi)
                        ElseIf pinfo.HomeTabId <= 0 Then
                            psettings = New DotNetNuke.Entities.Portals.PortalSettings(pinfo.SplashTabId, pi)
                        Else
                            psettings = New DotNetNuke.Entities.Portals.PortalSettings(pinfo.HomeTabId, pi)
                        End If
                    Catch ex As Exception
                        Try
                            psettings = BuildPortalSettings(ModInfo.PortalID, ModInfo.TabID, ModInfo.ModuleID)
                        Catch ex2 As Exception
                            psettings = New DotNetNuke.Entities.Portals.PortalSettings
                            psettings.PortalId = ModInfo.PortalID
                        End Try
                    End Try
                    Dim ipsettings As New DataAccess.PortalSettings(psettings)

                    Dim ds As New DataSet
                    Dim httpS As New HttpContext(New HttpRequest("EXPORT", "http://www.openwebstudio.com", ""), New HttpResponse(New System.IO.StringWriter))
                    Dim esession As New GenericSession()
                    Dim ctl As New Framework.UI.Control
                    ctl.ID = "Export"

                    Dim settings As Hashtable
                    settings = mctrl.GetModuleSettings(ModInfo.ModuleID)


                    'TRIGGER THE DEBUG START
                    If conf.canDebug(AbstractFactory.Instance.UserController.CurrentUser(), CType(ipsettings, IPortalSettings), isEditable) And conf.OutputType = r2i.OWS.Framework.Settings.RenderType.Default Then
                        Debugger_Open(conf, "iPortable - Export", modInfo.ModuleID.ToString, AbstractFactory.Instance.UserController.CurrentUser(), CType(ipsettings, IPortalSettings), isEditable, True, conf.enableQueryDebug)
                    End If
                    If conf.enableQueryDebug_Log AndAlso _debugOutput Is Nothing Then
                        Debugger_Open(conf, "iPortable - Export", modInfo.ModuleID.ToString, AbstractFactory.Instance.UserController.CurrentUser(), CType(ipsettings, IPortalSettings), isEditable, False, conf.enableQueryDebug)
                    End If

                    'CREATE THE ENGINE
                    renderingEngine = New r2i.OWS.Engine(httpS, esession, ctl, False, AbstractFactory.Instance.UserController.CurrentUser(), New System.Web.UI.StateBag, settings, CType(ipsettings, IPortalSettings), Nothing, modInfo.ModuleID.ToString(), modInfo.TabID.ToString(), modInfo.TabModuleID.ToString(), ConfigurationID(modInfo.ModuleID), conf, "", "", _debugOutput, True)
                    renderingEngine.RequestType = EngineBase.RequestTypeEnum.Export
                    renderingEngine.ExecuteActions(ds, "", "", _debugOutput)

                    If Not ds Is Nothing Then
                        sb.Append("<data>")
                        Dim ser As New Xml.Serialization.XmlSerializer(ds.GetType)
                        Dim sbx As New Text.StringBuilder
                        Dim sbw As New IO.StringWriter(sbx)
                        ser.Serialize(sbw, ds)
                        Dim charI As Integer = 0
                        Dim foundCharI As Integer = -1
                        Dim b As Boolean = False
                        While Not b
                            If sbx.Length > 0 AndAlso sbx.Length > charI Then
                                If sbx(charI) = ">"c Then
                                    b = True
                                    foundCharI = charI + 1
                                End If
                            Else
                                b = True
                            End If
                            charI += 1
                        End While
                        If foundCharI > 0 Then
                            sbx.Remove(0, foundCharI)
                        End If
                        sb.Append(sbx.ToString)
                        sbx = Nothing
                        sb.Append("</data>")
                    End If
                    If Not renderingEngine.ActionVariables Is Nothing AndAlso renderingEngine.ActionVariables.Count > 0 Then
                        sb.Append("<variables>")
                        Dim dt As New DataTable("Action.Variables")
                        dt.Columns.Add("key")
                        dt.Columns.Add("value")
                        Dim strKey As String
                        For Each strKey In renderingEngine.ActionVariables.Keys
                            Try
                                Dim dr As DataRow = dt.NewRow()
                                dr("key") = strKey
                                dr("value") = renderingEngine.ActionVariables.Item(strKey)
                                dt.Rows.Add(dr)
                            Catch ex As Exception
                            End Try
                        Next
                        dt.WriteXml(tw)
                        sb.Append("</variables>")
                    End If
                End If
            End If
        Catch ex As Exception
            'DotNetNuke.Services.Exceptions.LogException(New Exception("Error Occured in Module " & ModInfo.ModuleID & " while attempting to handle iSearchable: " & ex.ToString))
        Finally
            'CLOSE THE DEBUGGER
            If Not _debugOutput Is Nothing Then
                Try
                    Debugger_Close(conf, "EXPORT", renderingEngine, isEditable, conf.enableQueryDebug)
                Catch ex As Exception
                End Try
            End If
        End Try
        tw.Flush()
        Return DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(sb.ToString)
    End Function

    Public Function GetSearchItems(ByVal ModInfo As DotNetNuke.Entities.Modules.ModuleInfo) As DotNetNuke.Services.Search.SearchItemInfoCollection Implements DotNetNuke.Entities.Modules.ISearchable.GetSearchItems
        Dim seC As New DotNetNuke.Services.Search.SearchItemInfoCollection
        Dim renderingEngine As r2i.OWS.Engine = Nothing
        Dim conf As Settings = Nothing
        Dim isEditable As Boolean = False
        Try
            'LOAD THE CONFIGURATION
            'Dim xctrl As New r2i.OWS.Controller


            conf = GetConfiguration(ModInfo)
            If Not conf Is Nothing Then
                If Not conf Is Nothing Then
                    Dim mctrl As New DotNetNuke.Entities.Modules.ModuleController
                    Dim pctrl As New DotNetNuke.Entities.Portals.PortalController
                    Dim pactrl As New DotNetNuke.Entities.Portals.PortalAliasController
                    Dim piCol As DotNetNuke.Entities.Portals.PortalAliasCollection = pactrl.GetPortalAliasByPortalID(ModInfo.PortalID)
                    Dim pi As DotNetNuke.Entities.Portals.PortalAliasInfo = piCol.Item(CType(New ArrayList(piCol.Keys).Item(0), String))
                    Dim pinfo As DotNetNuke.Entities.Portals.PortalInfo
                    Dim psettings As DotNetNuke.Entities.Portals.PortalSettings
                    pinfo = pctrl.GetPortal(ModInfo.PortalID)

                    Try
                        If pinfo.HomeTabId <= 0 And pinfo.SplashTabId <= 0 Then
                            psettings = New DotNetNuke.Entities.Portals.PortalSettings(ModInfo.TabID, pi)
                        ElseIf pinfo.HomeTabId <= 0 Then
                            psettings = New DotNetNuke.Entities.Portals.PortalSettings(pinfo.SplashTabId, pi)
                        Else
                            psettings = New DotNetNuke.Entities.Portals.PortalSettings(pinfo.HomeTabId, pi)
                        End If
                    Catch ex As Exception
                        Try
                            psettings = BuildPortalSettings(ModInfo.PortalID, ModInfo.TabID, ModInfo.ModuleID)
                        Catch ex2 As Exception
                            psettings = New DotNetNuke.Entities.Portals.PortalSettings
                            psettings.PortalId = ModInfo.PortalID
                        End Try
                    End Try
                    Dim ipsettings As New DataAccess.PortalSettings(psettings)

                    Dim ds As DataSet = Nothing
                    Dim httpS As New HttpContext(New HttpRequest("SEARCH", "http://www.openwebstudio.com", ""), New HttpResponse(New System.IO.StringWriter))
                    Dim esession As New GenericSession()
                    Dim ctl As New Framework.UI.Control
                    ctl.ID = "SearchEngine"

                    Dim settings As Hashtable
                    settings = mctrl.GetModuleSettings(ModInfo.ModuleID)

                    'TRIGGER THE DEBUG START
                    If conf.canDebug(AbstractFactory.Instance.UserController.CurrentUser(), CType(ipsettings, IPortalSettings), isEditable) And conf.OutputType = r2i.OWS.Framework.Settings.RenderType.Default Then
                        'Debugger_Open(conf, "iSearchable - GetSearchItems", ModInfo.ModuleID.ToString, AbstractFactory.Instance.UserController.CurrentUser(), CType(ipsettings, IPortalSettings), isEditable, True, conf.enableQueryDebug)
                        _debugOutput = Nothing
                    End If
                    If conf.enableQueryDebug_Log AndAlso _debugOutput Is Nothing Then
                        'Debugger_Open(conf, "iSearchable - GetSearchItems", ModInfo.ModuleID.ToString, AbstractFactory.Instance.UserController.CurrentUser(), CType(ipsettings, IPortalSettings), isEditable, False, conf.enableQueryDebug)
                        _debugOutput = Nothing
                    End If

                    'CREATE THE ENGINE
                    renderingEngine = New r2i.OWS.Engine(httpS, esession, ctl, False, AbstractFactory.Instance.UserController.CurrentUser(), New System.Web.UI.StateBag, settings, CType(ipsettings, IPortalSettings), Nothing, ModInfo.ModuleID.ToString(), ModInfo.TabID.ToString(), ModInfo.TabModuleID.ToString(), ConfigurationID(ModInfo.ModuleID), conf, "", "", _debugOutput, True)
                    renderingEngine.RequestType = EngineBase.RequestTypeEnum.Search
                    renderingEngine.ExecuteActions(ds, "", "", _debugOutput)

                    Dim strQuery As String = conf.SearchQuery
                    If Not strQuery Is Nothing AndAlso strQuery.Length > 0 Then

                        'Dim renderingEngine As Engine = New Engine(httpS, ctl, New DotNetNuke.Entities.Users.UserInfo, New System.Web.UI.StateBag, settings, psettings, Nothing, ModInfo.ModuleID, ModInfo.TabID, ModInfo.TabModuleID, conf, "", "", Nothing, True)

                        'strQuery = renderingEngine.RenderString(Nothing, strQuery, Nothing, False, False)
                        strQuery = renderingEngine.RenderQuery(ds, Nothing, Nothing, renderingEngine.RecordsPerPage, Nothing, _debugOutput, strQuery)

                        'EXECUTE THE QUERY
                        If Not strQuery Is Nothing AndAlso strQuery.Length > 0 Then
                            Try
                                Dim customConnection As String = Nothing
                                If Not conf.customConnection Is Nothing AndAlso conf.customConnection.Length > 0 Then
                                    customConnection = renderingEngine.RenderString(Nothing, conf.customConnection, Nothing, False, False, DebugWriter:=_debugOutput)
                                End If

                                Dim sCacheName As String = Nothing
                                Dim sCacheTime As String = Nothing
                                Dim bCacheShared As Boolean = False
                                If Not renderingEngine.TemplateCacheName Is Nothing Then
                                    sCacheName = renderingEngine.RenderString(Nothing, renderingEngine.TemplateCacheName, Nothing, False, False, DebugWriter:=_debugOutput)
                                End If
                                If Not renderingEngine.TemplateCacheTime Is Nothing Then
                                    sCacheTime = renderingEngine.RenderString(Nothing, renderingEngine.TemplateCacheTime, Nothing, False, False, DebugWriter:=_debugOutput)
                                End If
                                If Not renderingEngine.TemplateCacheShare Is Nothing Then
                                    bCacheShared = Utility.CNullBool(renderingEngine.RenderString(Nothing, renderingEngine.TemplateCacheShare, Nothing, False, False, DebugWriter:=_debugOutput))
                                End If

                                ds = renderingEngine.GetData(False, strQuery, "", "", _debugOutput, True, sCacheName, sCacheTime, bCacheShared, CustomConnection:=customConnection)
                            Catch ex As Exception
                                DotNetNuke.Services.Exceptions.LogException(New Exception("An Error Occured in Module " & ModInfo.ModuleID & " while attempting to execute the iSearchable Query: " & ex.ToString))
                            End Try
                        End If
                        If Not ds Is Nothing AndAlso Not ds.Tables Is Nothing AndAlso ds.Tables.Count > 0 AndAlso ds.Tables(0).Rows.Count > 0 Then
                            'LOOP THROUGH ITEMS
                            Dim dr As DataRow = Nothing
                            For Each dr In ds.Tables(0).Rows
                                Dim strTitle As String = conf.SearchTitle
                                Dim strContent As String = conf.SearchContent
                                Dim strDescription As String = conf.SearchDescription
                                Dim strAuthor As String = conf.SearchAuthor
                                Dim strLink As String = conf.SearchLink
                                Dim strDate As String = conf.SearchDate
                                Dim strSearchKey As String = conf.SearchKey
                                If Not strTitle Is Nothing Then
                                    strTitle = renderingEngine.RenderString(ds, strTitle, Nothing, False, False, Row:=dr, DebugWriter:=_debugOutput)
                                End If
                                If Not strContent Is Nothing Then
                                    strContent = renderingEngine.RenderString(ds, strContent, Nothing, False, False, Row:=dr, DebugWriter:=_debugOutput)
                                End If
                                If Not strDescription Is Nothing Then
                                    strDescription = renderingEngine.RenderString(ds, strDescription, Nothing, False, False, Row:=dr, DebugWriter:=_debugOutput)
                                End If
                                If Not strAuthor Is Nothing Then
                                    strAuthor = renderingEngine.RenderString(ds, strAuthor, Nothing, False, False, Row:=dr, DebugWriter:=_debugOutput)
                                    If Not IsNumeric(strAuthor) Then
                                        strAuthor = Nothing
                                    End If
                                End If
                                If Not strLink Is Nothing Then
                                    strLink = renderingEngine.RenderString(ds, strLink, Nothing, False, False, Row:=dr, DebugWriter:=_debugOutput)
                                End If
                                If Not strSearchKey Is Nothing Then
                                    strSearchKey = renderingEngine.RenderString(ds, strSearchKey, Nothing, False, False, Row:=dr, DebugWriter:=_debugOutput)
                                Else
                                    strSearchKey = ""
                                End If
                                If Not strDate Is Nothing Then
                                    strDate = renderingEngine.RenderString(ds, strDate, Nothing, False, False, Row:=dr, DebugWriter:=_debugOutput)
                                    If Not IsDate(strDate) Then
                                        strDate = Nothing
                                    End If
                                End If
                                If Not strTitle Is Nothing AndAlso strTitle.Length > 0 Then
                                    Dim sei As New DotNetNuke.Services.Search.SearchItemInfo
                                    sei.Title = strTitle.Trim()
                                    If Not strAuthor Is Nothing Then
                                        sei.Author = CInt(strAuthor)
                                    End If
                                    If Not strContent Is Nothing Then
                                        sei.Content = strContent.Trim()
                                    End If
                                    If Not strDescription Is Nothing Then
                                        sei.Description = DotNetNuke.Common.Utilities.HtmlUtils.Shorten(DotNetNuke.Common.Utilities.HtmlUtils.Clean(strDescription.Trim(), False), 100, "...")
                                    End If
                                    If Not strLink Is Nothing Then
                                        sei.GUID = strLink
                                    End If
                                    If Not strDate Is Nothing Then
                                        sei.PubDate = CDate(strDate)
                                    Else
                                        sei.PubDate = Now
                                    End If
                                    sei.SearchKey = strSearchKey
                                    sei.ModuleId = ModInfo.ModuleID
                                    seC.Add(sei)
                                End If
                            Next
                            'ADD EACH ITEM TO SEARCH
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            'DotNetNuke.Services.Exceptions.LogException(New Exception("Error Occured in Module " & ModInfo.ModuleID & " while attempting to handle iSearchable: " & ex.ToString))
        Finally
            'CLOSE THE DEBUGGER
            If Not _debugOutput Is Nothing Then
                Try
                    Debugger_Close(conf, "SEARCH", renderingEngine, isEditable, conf.enableQueryDebug)
                Catch ex As Exception
                End Try
            End If
        End Try
        Return seC
    End Function
    Private Function GetConfiguration(ByVal modInfo As DotNetNuke.Entities.Modules.ModuleInfo) As Settings
        Dim conf As Settings = Nothing
        If modInfo.ModuleDefinition.FriendlyName.ToUpper = "OWS" Then
            conf = LoadConfiguration(modInfo.ModuleID)
        Else
            Dim mscrl As System.Web.UI.Control = (New System.Web.UI.UserControl).LoadControl(modInfo.ModuleControl.ControlSrc)
            Dim mscrlx As System.Web.UI.Control
            For Each mscrlx In mscrl.Controls
                If mscrlx.GetType().Name.ToUpper.Contains("OPENCONTROL") Then
                    Dim oc As OpenControl = CType(mscrlx, OpenControl)
                    oc.Load_Configuration()
                    conf = oc.Configuration
                    Exit For
                End If
            Next
            'If mscrl.Controls(0).ID.ToUpper = "OWS" Then
            '    Dim oc As OpenControl = CType(mscrl.Controls(0), OpenControl)
            '    oc.Load_Configuration()

            '    conf = oc.Configuration
            'End If
        End If
        Return conf
    End Function
    Private ReadOnly Property ConfigurationID(ByVal ModuleID As Integer) As Guid
        Get
            Dim idr As IDataReader = DotNetNuke.Data.DataProvider.Instance().GetModuleSetting(ModuleID, "ConfigurationID")
            Dim sConfigID As String = Guid.Empty.ToString()

            If Not idr Is Nothing AndAlso idr.Read() Then
                sConfigID = CStr(idr.Item("SettingValue"))
            End If
            Try
                idr.Close()
            Catch ex As Exception

            End Try
            Try
                idr.Dispose()
            Catch ex As Exception

            End Try

            Try
                Return New Guid(sConfigID)
            Catch
                Return Guid.Empty
            End Try
        End Get
    End Property
    Private Function LoadConfiguration(ByVal ModuleID As Integer) As Settings        
        Try
            Return LoadConfiguration(ConfigurationID(ModuleID))
        Catch
            Return Nothing
        End Try
    End Function
    Private Function LoadConfiguration(ByVal ConfigurationID As Guid) As Settings
        Dim ConfigCtrl As New Controller

        Return r2i.OWS.Framework.Settings.Deserialize(ConfigCtrl.GetSetting(ConfigurationID))
    End Function


    Public Function ExportModule(ByVal ModuleID As Integer) As String Implements DotNetNuke.Entities.Modules.IPortable.ExportModule
        Dim configurationId As String = Nothing
        Dim configurationStr As String = Nothing
        Dim settingStr As String = Nothing
        Dim items As String = ""
        Try
            Dim msc As New DotNetNuke.Entities.Modules.ModuleController
            Dim hsh As Hashtable = msc.GetModuleSettings(ModuleID)
            If hsh.ContainsKey("ConfigurationID") Then
                configurationId = CStr(hsh.Item("ConfigurationID"))
            End If
            Dim k As String
            For Each k In hsh.Keys
                Try
                    Dim strMS As String = CType(hsh(k), String)
                    items &= "<setting key=""" & Utilities.Utility.HTMLEncode(k) & """>" & Utilities.Utility.HTMLEncode(strMS) & "</setting>"
                Catch ex As Exception
                End Try
            Next
        Catch ex As Exception
        End Try

        If Not configurationId Is Nothing AndAlso configurationId.Length > 0 Then
            Dim conf As String = ""
            Dim portability As String = Utility.ConfigurationSetting("OpenWebStudio.iPortable.Configuration")
            If portability Is Nothing OrElse portability.Length = 0 OrElse portability.ToUpper = "INCLUDE" Then
                conf = DataProvider.Instance().GetSetting(New Guid(configurationId))
            Else
                conf = ""
            End If
            items &= "<configurationId>" & DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(configurationId) & "</configurationId>"
            items &= "<configuration>" & DotNetNuke.Common.Utilities.XmlUtils.XMLEncode(conf) & "</configuration>"
        End If

        Dim strXML As String = ""
        strXML &= "<openwebstudio>"
        strXML &= items
        strXML &= "</openwebstudio>"
        strXML &= "<custom>"
        strXML &= GetExport(ModuleID)
        strXML &= "</custom>"
        Return strXML
    End Function
    Public Sub ImportModule(ByVal ModuleID As Integer, ByVal Content As String, ByVal Version As String, ByVal UserID As Integer) Implements DotNetNuke.Entities.Modules.IPortable.ImportModule
        Try
            Dim configurationId As String = Nothing
            Dim configurationStr As String = Nothing

            Dim xmlDoc As New Xml.XmlDocument
            xmlDoc.LoadXml("<root>" & Content & "</root>")
            Dim xmlSettings As System.Xml.XmlNode = xmlDoc.SelectSingleNode("/root/openwebstudio")

            Dim mctrl As New DotNetNuke.Entities.Modules.ModuleController
            Dim minfo As DotNetNuke.Entities.Modules.ModuleInfo = mctrl.GetModule(ModuleID, -1)
            Try
                configurationId = xmlSettings.SelectSingleNode("configurationId").InnerText

                If Not configurationId Is Nothing Then
                    Try
                        configurationStr = xmlSettings.SelectSingleNode("configuration").InnerText
                        If Not configurationStr Is Nothing AndAlso configurationStr.Length > 0 Then
                            'ONLY INSERT THE CONFIGURATION, DO NOT SIMPLY OVERRIDE!
                            Dim objSettings As New r2i.OWS.Controller
                            If Not objSettings.CheckExists(configurationId) Then
                                objSettings.UpdateSetting(New Guid(configurationId), configurationStr, CStr(UserID))
                            End If
                        End If
                    Catch ex As Exception
                    End Try
                    mctrl.UpdateModuleSetting(ModuleID, "ConfigurationID", configurationId)
                End If
            Catch ex As Exception
            End Try
            Try
                'NOW HANDLE THE MODULESETTINGS BECAUSE APPARENTLY DNN IGNORES THEM NOW.
                Dim nl As System.Xml.XmlNodeList = xmlSettings.SelectNodes("setting")
                If Not nl Is Nothing AndAlso nl.Count > 0 Then
                    Dim n As System.Xml.XmlNode
                    For Each n In nl
                        Try
                            Dim key As String = n.Attributes("key").Value
                            Dim value As String = n.InnerText
                            key = Utility.HTMLDecode(key)
                            value = Utility.HTMLDecode(value)
                            If Not key.ToUpper = "CONFIGURATIONID" Then
                                mctrl.UpdateModuleSetting(ModuleID, key, value)
                            End If
                        Catch ex As Exception
                        End Try
                    Next
                End If
            Catch ex As Exception

            End Try
            GetImport(ModuleID, Content, Version, UserID)
        Catch ex As Exception

        End Try

    End Sub

    Private Sub ClearCache()
        If Not HttpContext.Current.Cache Is Nothing Then
            While HttpContext.Current.Cache.Count > 0
                Dim enumerator As System.Collections.IDictionaryEnumerator = HttpContext.Current.Cache.GetEnumerator
                If Not enumerator Is Nothing Then
                    enumerator.MoveNext()
                    HttpContext.Current.Cache.Remove(CStr(enumerator.Key))
                End If
            End While
        End If
    End Sub

    Public Sub New()
        Wrapper.DNN.Entities.WrapperFactory.Create()
    End Sub
End Class