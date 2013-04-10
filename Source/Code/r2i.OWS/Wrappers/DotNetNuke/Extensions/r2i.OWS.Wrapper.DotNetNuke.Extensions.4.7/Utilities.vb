Namespace r2i.OWS.Wrapper.DNN.Extensions
    Public Class Utilities
        Public Shared Function Dotnetnuke_CreateNewPortal(ByVal UserID As Integer, ByRef PortalSettings As r2i.OWS.Framework.DataAccess.IPortalSettings, ByVal SelectedTemplate As String, ByVal PortalName As String, ByVal [Alias] As String, ByVal isChild As Boolean, ByVal HomeDirectory As String, ByVal FirstName As String, ByVal LastName As String, ByVal UserName As String, ByVal Password As String, ByVal Email As String, ByVal Description As String, ByVal Keywords As String) As Integer
            Dim exception As String = ""
            Try
                Dim blnChild As Boolean
                Dim strPortalAlias As String = Nothing
                Dim intCounter As Integer
                Dim intPortalId As Integer
                Dim strServerPath As String
                Dim strChildPath As String = String.Empty

                Dim objPortalController As New DotNetNuke.Entities.Portals.PortalController

                ' check template validity
                Dim messages As New ArrayList
                Dim schemaFilename As String = System.Web.HttpContext.Current.Server.MapPath("admin/Portal/portal.template.xsd")
                Dim xmlFilename As String = ""
                If SelectedTemplate.ToUpper.EndsWith(".TEMPLATE") Then
                    xmlFilename = DotNetNuke.Common.Globals.HostMapPath & SelectedTemplate
                Else
                    xmlFilename = DotNetNuke.Common.Globals.HostMapPath & SelectedTemplate & ".template"
                End If


                'Dim xval As New DotNetNuke.Entities.Portals.PortalTemplateValidator
                'If Not xval.Validate(xmlFilename, schemaFilename) Then
                'Throw New Exception("Invalid Template File")
                'Exit Function
                'End If

                'Set Portal Name
                Dim Title As String = Replace(PortalName, "http://", "")
                PortalName = LCase(Title)

                blnChild = isChild '(optType.SelectedValue = "C")
                If Not [Alias] Is Nothing AndAlso [Alias].Length > 0 Then
                    exception &= "The alias was supplied directly when this occurs, the Portal Name is not used to aid in the naming of the destinaton portal. If this is a child portal, this may cause a problem. "
                    strPortalAlias = [Alias]
                Else
                    exception &= "No alias was supplied, the alias will be inferred based on the incoming Portal Name. "
                End If
                If strPortalAlias Is Nothing Then
                    If blnChild Then
                        exception &= "The new portal will be a Child portal, and its Alias will be ParentAlias/PortalName with the PortalName value set to: "
                        strPortalAlias = Mid(PortalName, InStrRev(PortalName, "/") + 1)
                        exception &= strPortalAlias & ". "
                    Else
                        exception &= "The new portal will be a Parent portal, and its Alias will be set to the incoming PortalName: "
                        strPortalAlias = PortalName
                        exception &= strPortalAlias & ". "
                    End If
                End If

                Dim strValidChars As String = "abcdefghijklmnopqrstuvwxyz0123456789-"
                If Not blnChild Then
                    strValidChars += "./:"
                End If

                For intCounter = 1 To strPortalAlias.Length
                    If InStr(1, strValidChars, Mid(strPortalAlias, intCounter, 1)) = 0 Then
                        Throw New Exception("The Portal Alias is invalid as it contains characters which are not allowed. Allowed characters are contained within the following: " & strValidChars & ". ")
                    End If
                Next intCounter

                strServerPath = DotNetNuke.Common.GetAbsoluteServerPath(System.Web.HttpContext.Current.Request)

                'Set Portal Alias for Child Portals
                If blnChild Then
                    strChildPath = strServerPath & strPortalAlias
                    exception &= "When a child portal is created, a directory matching the path for the child alias is created in the root of the website. The provided path in this case will be: " & strChildPath & ". "
                    If System.IO.Directory.Exists(strChildPath) Then
                        Throw New Exception(exception & "This fails because the directory itself already exists. You will need to either manually remove that directory or use a different portal path to create the portal. This is done by either using a unique portal name and/or alias.")
                    Else
                        If PortalSettings.ActiveTab.ParentId <> PortalSettings.SuperTabId Then
                            strPortalAlias = DotNetNuke.Common.GetDomainName(System.Web.HttpContext.Current.Request) & "/" & strPortalAlias
                            exception &= "The current Parent Tab is not the Super Tab, therefore the Alias is now reset to contain the incoming requests Domain Name, followed by the current Portal Alias value: " & strPortalAlias & "."
                        Else
                            'strPortalAlias = PortalName
                            exception &= "At this point, the folder exists, and the Portal Alias will be based specifically on the incoming value."
                        End If
                    End If
                End If

                'Get Home Directory
                Dim HomeDir As String
                If Not HomeDirectory Is Nothing AndAlso HomeDirectory.ToUpper <> "PORTALS/[PORTALID]" Then
                    HomeDir = HomeDirectory
                    exception &= "The home directory was manually assigned, and will be set to: " & HomeDir & ". "
                Else
                    HomeDir = ""
                    exception &= "The home directory was not provided, it will be automatically set. "
                End If

                'Validate Home Folder
                If Not HomeDir Is Nothing AndAlso HomeDir.Length > 0 Then
                    Dim objFolderController As New DotNetNuke.Services.FileSystem.FolderController
                    If Not objFolderController.GetMappedDirectory(DotNetNuke.Common.Globals.ApplicationPath + "/" + HomeDir + "/") Is Nothing Then
                        Throw New Exception("This home directory directory path is not a valid path. ")
                    End If
                End If

                'Validate Portal Alias
                If Not strPortalAlias Is Nothing AndAlso strPortalAlias.Length > 0 Then
                    Dim mc As New DotNetNuke.Entities.Portals.PortalAliasController

                    Dim PortalAlias As DotNetNuke.Entities.Portals.PortalAliasInfo = mc.GetPortalAlias(strPortalAlias, -1)
                    If Not PortalAlias Is Nothing Then
                        Throw New Exception("The provided Portal Alias """ & strPortalAlias & """ already exists, please change the alias and try again. ")
                    End If
                End If


                'Create Portal
                Dim strTemplateFile As String = ""
                If SelectedTemplate.ToUpper.EndsWith(".TEMPLATE") Then
                    strTemplateFile = SelectedTemplate
                Else
                    strTemplateFile = SelectedTemplate & ".template"
                End If

                'Attempt to create the portal
                Try
                    intPortalId = objPortalController.CreatePortal(Title, FirstName, LastName, UserName, Password, Email, Description, Keywords, DotNetNuke.Common.Globals.HostMapPath, strTemplateFile, HomeDir, strPortalAlias, strServerPath, strChildPath, blnChild)
                Catch ex As Exception
                    intPortalId = DotNetNuke.Common.Utilities.Null.NullInteger
                    Throw New Exception("DotNetNuke has thown an exeption while attempting to create the portal, its message is: " & ex.ToString)
                End Try

                If intPortalId <> -1 Then
                    exception &= "The portal appears to have been created successfully. Any futher errors beyond this point are due to sending notifications. "
                    ' notification
                    Dim objUserC As New DotNetNuke.Entities.Users.UserController
                    Dim objUser As DotNetNuke.Entities.Users.UserInfo = objUserC.GetUserByUsername(intPortalId, UserName, False)

                    'Create a Portal Settings object for the new Portal
                    Dim objPortal As DotNetNuke.Entities.Portals.PortalInfo = objPortalController.GetPortal(intPortalId)
                    Dim newSettings As New DotNetNuke.Entities.Portals.PortalSettings
                    newSettings.PortalAlias = New DotNetNuke.Entities.Portals.PortalAliasInfo
                    newSettings.PortalAlias.HTTPAlias = strPortalAlias
                    newSettings.PortalId = intPortalId
                    newSettings.DefaultLanguage = objPortal.DefaultLanguage
                    Dim webUrl As String = DotNetNuke.Common.AddHTTP(strPortalAlias)

                    Dim objEventLog As New DotNetNuke.Services.Log.EventLog.EventLogController
                    objEventLog.AddLog(objPortalController.GetPortal(intPortalId), newSettings, UserID, "", DotNetNuke.Services.Log.EventLog.EventLogController.EventLogType.PORTAL_CREATED)

                    ' Redirect to this new site
                    Return intPortalId 'Response.Redirect(webUrl, True)
                End If

                Return -1
            Catch exc As Exception    'Module failed to load
                Throw New Exception(exception & exc.Message)
            End Try
            Return -1
        End Function
    End Class
End Namespace