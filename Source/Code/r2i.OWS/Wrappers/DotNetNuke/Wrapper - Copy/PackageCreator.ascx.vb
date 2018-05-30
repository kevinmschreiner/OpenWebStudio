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
Imports System.Web
Imports System.Web.UI
Imports System.Security
Imports DotNetNuke.Entities.Modules.Communications
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Actions
Imports DotNetNuke.Security
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization

Partial Public Class [PackageCreator]
    Inherits [Module]

    Implements DotNetNuke.Entities.Modules.Communications.IModuleListener
    Private Function CleanName(ByVal Name As String) As String

        Dim strName As String = Name
        Dim strBadChars As String = " ~`!@#$%^&*()-_+={[}]|\:;<,>?/" & Chr(34) & Chr(39)

        Dim intCounter As Integer
        For intCounter = 0 To Len(strBadChars) - 1
            strName = strName.Replace(strBadChars.Substring(intCounter, 1), "")
        Next intCounter

        Return strName

    End Function
    Public Shadows Sub OnModuleCommunication(ByVal s As Object, ByVal e As DotNetNuke.Entities.Modules.Communications.ModuleCommunicationEventArgs) Implements DotNetNuke.Entities.Modules.Communications.IModuleListener.OnModuleCommunication
        If e.Type = "Package" AndAlso e.Value.ToString = "Export" Then
            Dim fi As System.Web.HttpPostedFile = Request.Files.Get("frmInstallFile")
            If fi Is Nothing OrElse fi.FileName Is Nothing OrElse fi.FileName.Length = 0 Then
                BuildPackage()
            Else
                InstallPackage(fi)
            End If
        ElseIf e.Type = "Package" AndAlso e.Value.ToString = "Cancel" Then
            Response.Redirect(DotNetNuke.Common.NavigateURL(Me.TabId))
        End If
    End Sub
    Private Sub InstallPackage(ByVal flUpload As System.Web.HttpPostedFile)
        Dim result As String = Nothing
        Dim userObj As New DataAccess.User(UserInfo)
        Dim portalObj As New DataAccess.PortalSettings(PortalSettings)
        Dim bt(15) As Byte
        Dim rnd As New Random
        rnd.NextBytes(bt)
        Dim configId As New Guid(bt)
        Dim iSess As SessionState.IHttpSessionState = Nothing
        Try
            iSess = CType(Session, SessionState.IHttpSessionState)
        Catch ex As Exception
        End Try
        If iSess Is Nothing Then iSess = New Utilities.GenericSession
        Dim rEngine As Engine = New Engine(Context, iSess, r2i.OWS.Framework.UI.Control.Create(CType(Me, System.Web.UI.Control)), False, userObj, ViewState, Settings, CType(portalObj, IPortalSettings), Nothing, CStr(ModuleId), CStr(TabId), CStr(TabModuleId), configId, New r2i.OWS.Framework.Settings, Me.ClientID, ModulePath, Nothing, True)
        Dim handler As Packaging.Wrapper
        Try
            handler = New Packaging.Wrapper(rEngine, CStr(Me.ModuleId), CStr(Me.PortalSettings.ActiveTab.TabID), CStr(Me.PortalId), Me.ModulePath, portalObj)
            Select Case handler.Unwrap(flUpload)
                Case Packaging.Common.PackageType.FailureReadOnly
                    result = "<i class=NormalRed>The Open Web Studio directory is read-only, and cannot extract temporary files. Please set the directory to allow read/write access for the Open Web Studio engine by turning off the ReadOnly flag.</i>"
                Case Packaging.Common.PackageType.ConfigurationOnly
                    result = "<i class=Normaled>Standalone Configurations are not yet supported</i>"
                Case Packaging.Common.PackageType.Failure
                    result = "<i class=NormalRed>The format of this file does not appear to be an OWS Package, or a configuration of any kind. Please try another file.</i>"
                Case Packaging.Common.PackageType.StandardPackage
                    Dim pkg As New Packaging.Package(CStr(Me.PortalId), handler.FullPath)
                    pkg.PathMapper = AddressOf MapPath
                    pkg.Engine = rEngine
                    pkg.Save(handler.Result)

                    Dim fpath As String = Nothing
                    If Not handler Is Nothing Then
                        fpath = handler.FullPath
                    End If

                    Packaging.Courier.Deliver(pkg.PackageID, rEngine, configId, CStr(Me.ModuleId), CStr(Me.PortalSettings.ActiveTab.TabID), Me.ClientID, CStr(Me.PortalId), ModulePath, Me.PortalSettings.AdministratorRoleName, CStr(Me.PortalSettings.AdministratorRoleId), DotNetNuke.Common.ApplicationMapPath, fpath)
                    result = "<script type=""text/javascript"">function watchInstall() {ows.Fetch(" & CStr(Me.ModuleId) & ",-1,'Package=" & pkg.PackageID.ToString & "','owsInstaller');} watchInstall(); window.setInterval(watchInstall,10000);</script>"
                Case Packaging.Common.PackageType.Unknown
                    result = "<i class=NormalRed>The unknown format of this file does not appear to be a ListX Package, or a configuration of any kind. Please try another file.</i>"
            End Select
        Catch ex As Exception
            result = "<i class=NormalRed>An exception was thrown while attempting to install this package. Please provide the following error to Technical Support: " & ex.ToString & "</i>"
        End Try
        Dim ltl As New LiteralControl(result)
        Me.Controls.Add(ltl)
        Me.EnsureChildControls()
    End Sub
    Private Sub BuildPackage()
        'EXPORT OF THE SITE IS ATTEMPTED - GET THE TABS AND MODULES
        Dim userObj As New DataAccess.User(UserInfo)
        Dim portalObj As New DataAccess.PortalSettings(PortalSettings)
        Dim bt(15) As Byte
        Dim rnd As New Random
        rnd.NextBytes(bt)
        Dim configId As New Guid(bt)

        Dim packName As String = Request.Form.Item("frmInstallFileName")
        Dim packfileName As String = CleanName(packName)
        Dim packfileVersion As String = Request.Form.Item("frmInstallFileVersion")
        Dim packfileversionS() As String = packfileVersion.Split("."c)
        Dim pversion(4) As String
        Dim i As Integer = 0
        For i = 0 To 3
            pversion(i) = "00"
            If packfileversionS.Length > i Then
                If IsNumeric(packfileversionS(i)) Then
                    pversion(i) = packfileversionS(i).PadLeft(2, "0"c)
                End If
            End If
        Next
        packfileVersion = String.Join(".", pversion)
        If packfileName.Length = 0 Then
            packfileName = "Portal." & Me.PortalId & ".Package." & Now.ToString("MM.dd.yyyy.hh.mm.ss")
            packName = packfileName
        Else
            packfileName &= "." & packfileVersion
        End If
        Dim iSess As SessionState.HttpSessionState = Nothing
        Try
            iSess = Session
        Catch ex As Exception
        End Try
        Dim rEngine As Engine = New Engine(Context, New Utilities.GenericSession(iSess), r2i.OWS.Framework.UI.Control.Create(CType(Me, System.Web.UI.Control)), False, userObj, ViewState, Settings, CType(portalObj, IPortalSettings), Nothing, CStr(ModuleId), CStr(TabId), CStr(TabModuleId), configId, New r2i.OWS.Framework.Settings, Me.ClientID, ModulePath, Nothing, True)
        Dim myBuilder As New Packaging.Designer(rEngine, CStr(Me.ModuleId), CStr(Me.TabId), CStr(Me.PortalId), Me.ModulePath, AddressOf MyBase.MapPath, CType(portalObj, IPortalSettings))
        Dim tC As New DotNetNuke.Entities.Tabs.TabController
        Dim sC As New DotNetNuke.UI.Skins.SkinController


        Dim packageName As String = packfileName & ".zip"
        Dim fullPath As String = myBuilder.File_CreateDirectory(PortalSettings.HomeDirectory & packageName)

        Dim fileIO As IO.FileStream

        Dim b As Boolean = False
        Try

            fileIO = New IO.FileStream(fullPath, IO.FileMode.Create)
            Dim key As String
            Dim pages As String() = Me.Request.Form.Item("pages").Split(","c)
            Dim pageList As New Generic.List(Of String)
            For Each key In pages
                pageList.Add(key)
            Next
            Dim modules As String() = Me.Request.Form.Item("modules").Split(","c)
            Dim moduleList As New Generic.List(Of String)
            For Each key In modules
                moduleList.Add(key)
            Next
            If pageList.Count > 0 Then
                b = myBuilder.BuildPortalPackage(AbstractFactory.Instance.SkinController.GetSkin("Skins", CStr(Me.PortalId), DotNetNuke.UI.Skins.SkinType.Portal), AbstractFactory.Instance.SkinController.GetSkin("Containers", CStr(Me.PortalId), DotNetNuke.UI.Skins.SkinType.Portal), fileIO, pageList, moduleList, packName, packfileVersion, configId.ToString)
            End If
            Try
                fileIO.Close()
            Catch ex As Exception
            End Try
        Catch ex As Exception
            Console.WriteLine(ex.ToString)
        End Try

        'Dim tabResult As String = myInstaller.DescribeTab(tC.GetTab(Me.PortalSettings.ActiveTab.TabID), sC.GetSkin("Skins", Me.PortalId, UI.Skins.SkinType.Portal), sC.GetSkin("Containers", Me.PortalId, UI.Skins.SkinType.Portal))
        If b Then
            Response.Clear()
            Response.ClearHeaders()
            'ADDED TO SKIP COMPRESSION
            If Not Context.Items.Contains("httpcompress.attemptedinstall") Then
                Context.Items.Add("httpcompress.attemptedinstall", "true")
            End If
            Response.ContentType = "application/octet-stream"
            Response.AddHeader("Content-Disposition", "attachment; filename=" & packageName)
            Response.WriteFile(fullPath)

            Try
                Response.Flush()
                Response.Close()
            Catch ex As Exception
            End Try
        End If

        Dim xfileio As New IO.FileInfo(fullPath)
        If xfileio.Exists Then
            System.IO.File.Delete(fullPath)
        End If

        myBuilder = Nothing
    End Sub
End Class
