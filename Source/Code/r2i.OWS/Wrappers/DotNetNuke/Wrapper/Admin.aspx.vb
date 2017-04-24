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
Imports r2i.OWS.Framework.Plugins.Renderers
Imports r2i.OWS.Framework.Plugins.Actions
Imports r2i.OWS.Framework.Utilities

Partial Public Class Admin
    Inherits System.Web.UI.Page

    Public Function Version(Optional ByVal showDecimals As Boolean = True) As String
        Return r2i.OWS.UI.OpenControlBase._JAVASCRIPTVERSION(showDecimals)
    End Function
    Public Function CSSLibrary(ByVal isHeader As Boolean) As String
        Return "<link rel=""stylesheet"" type=""text/css"" href=""admin.css?v=" & Version(False) & """/>"
    End Function
    Public Function CodeMirrorCss() As String
        Dim cmcss As String
        cmcss = ""
        cmcss += "<link rel=""stylesheet"" type=""text/css"" href=""Scripts/CodeMirror/lib/codemirror.css?v=" & Version(False) & """/> \n"
        cmcss += "<link rel=""stylesheet"" type=""text/css"" href=""Scripts/CodeMirror/theme/dracula.css?v=" & Version(False) & """/> \n"
        cmcss += "<link rel=""stylesheet"" type=""text/css"" href=""Scripts/CodeMirror/addon/hint/show-hint.css?v=" & Version(False) & """/> \n"
        cmcss += "<link rel=""stylesheet"" type=""text/css"" href=""Scripts/CodeMirror/addon/dialog/dialog.css?v=" & Version(False) & """/>"
        Return cmcss
    End Function
    Public Function PageTitle(ByVal isHeader As Boolean) As String
        Return "<title>Open Web Studio " & Version(True) & "</title>"
    End Function
    Public Function CodeMirrorConfig() As String
        Dim str As String = ""
            str += "<script type=""text/javascript"" src=""/Scripts/Codemirror/codeMirrorConfig.js?v=" & Version(False) & """></script>"
        Return str
    End Function
    Public Function JavascriptLibrary(ByVal isHeader As Boolean) As String
        Dim sectionInclude As r2i.OWS.Framework.Config.SectionItem
        Dim sectionInclusions As List(Of r2i.OWS.Framework.Config.SectionItem)

        Dim str As String = ""
        If isHeader Then

            sectionInclusions = r2i.OWS.Framework.Config.Items(Config.Section.Wrapper, Config.SectionType.Administration)
            If Not sectionInclusions Is Nothing AndAlso sectionInclusions.Count > 0 Then
                For Each sectionInclude In sectionInclusions
                    str &= "<script type=""text/javascript"" src=""" & sectionInclude.Type & "?v=" & r2i.OWS.UI.OpenControlBase._JAVASCRIPTVERSION & """></script>"
                Next
            End If
            sectionInclusions = r2i.OWS.Framework.Config.Items(Config.Section.General, Config.SectionType.Administration)
            If Not sectionInclusions Is Nothing AndAlso sectionInclusions.Count > 0 Then
                For Each sectionInclude In sectionInclusions
                    str &= "<script type=""text/javascript"" src=""" & sectionInclude.Type & "?v=" & r2i.OWS.UI.OpenControlBase._JAVASCRIPTVERSION & """></script>"
                Next
            End If

        Else

              Try

                sectionInclusions = r2i.OWS.Framework.Config.Items(Config.Section.Actions, Config.SectionType.Administration)
                If Not sectionInclusions Is Nothing AndAlso sectionInclusions.Count > 0 Then
                    For Each sectionInclude In sectionInclusions
                        str &= "<script type=""text/javascript"" src=""" & sectionInclude.Type & "?v=" & r2i.OWS.UI.OpenControlBase._JAVASCRIPTVERSION & """></script>"
                    Next
                End If
                sectionInclusions = r2i.OWS.Framework.Config.Items(Config.Section.Formats, Config.SectionType.Administration)
                If Not sectionInclusions Is Nothing AndAlso sectionInclusions.Count > 0 Then
                    For Each sectionInclude In sectionInclusions
                        str &= "<script type=""text/javascript"" src=""" & sectionInclude.Type & "?v=" & r2i.OWS.UI.OpenControlBase._JAVASCRIPTVERSION & """></script>"
                    Next
                End If
                sectionInclusions = r2i.OWS.Framework.Config.Items(Config.Section.Tokens, Config.SectionType.Administration)
                If Not sectionInclusions Is Nothing AndAlso sectionInclusions.Count > 0 Then
                    For Each sectionInclude In sectionInclusions
                        str &= "<script type=""text/javascript"" src=""" & sectionInclude.Type & "?v=" & r2i.OWS.UI.OpenControlBase._JAVASCRIPTVERSION & """></script>"
                    Next
                End If
                sectionInclusions = r2i.OWS.Framework.Config.Items(Config.Section.Queryies, Config.SectionType.Administration)
                If Not sectionInclusions Is Nothing AndAlso sectionInclusions.Count > 0 Then
                    For Each sectionInclude In sectionInclusions
                        str &= "<script type=""text/javascript"" src=""" & sectionInclude.Type & "?v=" & r2i.OWS.UI.OpenControlBase._JAVASCRIPTVERSION & """></script>"
                    Next
                End If

                str &= "<script type=""text/javascript"">adminAbout=" & GenAbout() & ";</script>"

            Catch ex As Exception
                AssignError(2, ex)
            End Try
        End If
        Return str
    End Function

    Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Wrapper.DNN.Entities.WrapperFactory.Create()
    End Sub

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo.IsSuperUser Then
            Dim msg As String = ""
            If DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo Is Nothing OrElse DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo.UserID = -1 Then
                msg = "<li>Unauthenticated User"
            Else
                msg = "<li>Username: " & DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo.Username
                msg &= "<li>Permission: "
                If DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo.IsSuperUser Then
                    msg &= "Super User"
                Else
                    msg &= "Not a Super User"
                End If
            End If
            AssignError(1, New Exception(msg))
        End If
    End Sub
    Private Sub GetAboutAssembly(ByRef assemList As Dictionary(Of String, String), ByVal Value As String)
        Try
            'Dim canGetVersion As String = System.Configuration.ConfigurationSettings.AppSettings("OpenWebStudio.Skip.Assembly.Version")


            'If canGetVersion Is Nothing OrElse canGetVersion.Length = 0 Then
            Dim typeLib As String = Value.Split(","c)(1).Trim
            If Not assemList.ContainsKey(typeLib) Then
                If Utilities.Utility.Security_FilePermission AndAlso Utilities.Utility.Security_ReflectPermission Then
                    'GET THE VERSION
                    Dim assembl As Reflection.Assembly = Nothing
                    assembl = System.Reflection.Assembly.Load(typeLib)
                    If Not assembl Is Nothing Then
                        With System.Diagnostics.FileVersionInfo.GetVersionInfo(assembl.Location)
                            With assembl.GetName()
                                assemList.Add(typeLib, .Version.ToString(4))
                            End With
                        End With
                    End If
                End If
            End If
            'End If
        Catch ex As Exception
        End Try
    End Sub

    Private Function GenAbout() As String
        Dim str As String = "{"
        Try

            'CUSTOM INCLUSIONS
            Dim sectionInclude As r2i.OWS.Framework.Config.SectionItem
            Dim assemList As New Dictionary(Of String, String)
            Dim jitems As List(Of String)
            Dim status As String = ""

            'ACTIONS

            str &= """Actions"":["
            jitems = New List(Of String)
            For Each sectionInclude In r2i.OWS.Framework.Config.Items(Config.Section.Actions, Config.SectionType.Runtime)
                status = "Invalid Permissions"
                If Utilities.Utility.Security_FilePermission AndAlso Utilities.Utility.Security_ReflectPermission Then
                    GetAboutAssembly(assemList, sectionInclude.Type)
                    status = ""
                    'ATTEMPT ISSUES
                    Try
                        Dim cR As ActionBase = DirectCast(sectionInclude.Load(), ActionBase)
                        Dim ptag As Plugins.PluginTag = Plugins.PluginTag.Create(Config.Section.Actions.ToString.ToLower, "", cR.Key)
                        If Not cR Is Nothing Then
                            Dim r As ActionBase = CType(Plugins.Manager.GetPlugin(ptag), ActionBase)  ' Common.GetActionHandler(sectionInclude.Type)
                            If r Is Nothing Then
                                status = "FAILED TO LOAD"
                            Else
                                status = ""
                            End If
                        Else
                            status = "FAILED TO INIT"
                        End If
                    Catch ex As Exception
                        status = ex.Message
                    End Try
                End If
                If Not Plugins.Manager.PluginException(sectionInclude.Type, sectionInclude.Name) Is Nothing Then
                    status &= " Error: " & Plugins.Manager.PluginException(sectionInclude.Type, sectionInclude.Name)
                End If

                jitems.Add("{""Name"":""" & sectionInclude.Name & """,""Version"":" & JSON.JsonConversion.Encode(status & " " & sectionInclude.Type) & "}")
            Next
            str &= String.Join(",", jitems.ToArray()) & "]"
            str &= ","

            str &= """Tokens"":["
            jitems = New List(Of String)
            For Each sectionInclude In r2i.OWS.Framework.Config.Items(Config.Section.Tokens, Config.SectionType.Runtime)
                status = "Invalid Permissions"
                If Utilities.Utility.Security_FilePermission AndAlso Utilities.Utility.Security_ReflectPermission Then
                    GetAboutAssembly(assemList, sectionInclude.Type)
                    status = ""
                    'ATTEMPT ISSUES
                    Try
                        Dim cR As RenderBase = DirectCast(sectionInclude.Load(), RenderBase)
                        If Not cR Is Nothing Then
                            Dim r As RenderBase = CType(Plugins.Manager.GetPlugin(Plugins.PluginTag.Create(Config.Section.Tokens.ToString.ToLower, cR.RenderType.ToString.ToLower, cR.RenderTag.ToLower)), RenderBase)  ' Common.GetActionHandler(sectionInclude.Type)
                            If r Is Nothing Then
                                status = "FAILED TO LOAD"
                            Else
                                status = ""
                            End If
                        Else
                            status = "FAILED TO INIT"
                        End If

                    Catch ex As Exception
                        status = ex.Message
                    End Try
                End If
                If Not Plugins.Manager.PluginException(sectionInclude.Type, sectionInclude.Name) Is Nothing Then
                    status &= " Error: " & Plugins.Manager.PluginException(sectionInclude.Type, sectionInclude.Name)
                End If
                jitems.Add("{""Name"":""" & sectionInclude.Name & """,""Version"":" & JSON.JsonConversion.Encode(status & " " & sectionInclude.Type) & "}")
            Next
            str &= String.Join(",", jitems.ToArray()) & "]"
            str &= ","


            str &= """Formats"":["
            jitems = New List(Of String)
            For Each sectionInclude In r2i.OWS.Framework.Config.Items(Config.Section.Formats, Config.SectionType.Runtime)
                status = "Invalid Permissions"
                If Utilities.Utility.Security_FilePermission AndAlso Utilities.Utility.Security_ReflectPermission Then
                    GetAboutAssembly(assemList, sectionInclude.Type)
                    status = ""
                    'ATTEMPT ISSUES
                    Try
                        Dim r As Plugins.Formatters.FormatterBase = CType(Plugins.Manager.GetPlugin(Plugins.PluginTag.Create(Config.Section.Formats.ToString.ToLower, "", sectionInclude.Name)), Plugins.Formatters.FormatterBase)  ' Common.GetActionHandler(sectionInclude.Type)
                        If r Is Nothing Then
                            status = "FAILED TO LOAD"
                        Else
                            status = ""
                        End If
                    Catch ex As Exception
                        status = ex.Message
                    End Try
                End If
                If Not Plugins.Manager.PluginException(sectionInclude.Type, sectionInclude.Name) Is Nothing Then
                    status &= " Error: " & Plugins.Manager.PluginException(sectionInclude.Type, sectionInclude.Name)
                End If
                jitems.Add("{""Name"":""" & sectionInclude.Name & """,""Version"":" & JSON.JsonConversion.Encode(status & " " & sectionInclude.Type) & "}")
            Next
            str &= String.Join(",", jitems.ToArray()) & "]"
            str &= ","

            str &= """Queries"":["
            jitems = New List(Of String)
            For Each sectionInclude In r2i.OWS.Framework.Config.Items(Config.Section.Queryies, Config.SectionType.Runtime)
                status = "Invalid Permissions"
                If Utilities.Utility.Security_FilePermission AndAlso Utilities.Utility.Security_ReflectPermission Then
                    GetAboutAssembly(assemList, sectionInclude.Type)
                    status = ""
                    'ATTEMPT ISSUES
                    Try
                        Dim cR As Plugins.Queries.QueryBase = DirectCast(sectionInclude.Load(), Plugins.Queries.QueryBase)
                        If Not cR Is Nothing Then
                            Dim r As Plugins.Queries.QueryBase = CType(Plugins.Manager.GetPlugin(Plugins.PluginTag.Create(Config.Section.Queryies.ToString.ToLower, "", cR.QueryTag)), Plugins.Queries.QueryBase)  ' Common.GetActionHandler(sectionInclude.Type)
                            If r Is Nothing Then
                                status = "FAILED TO LOAD"
                            Else
                                status = ""
                            End If
                        Else
                            status = "FAILED TO INIT"
                        End If
                    Catch ex As Exception
                        status = ex.Message
                    End Try
                End If
                If Not Plugins.Manager.PluginException(sectionInclude.Type, sectionInclude.Name) Is Nothing Then
                    status &= " Error: " & Plugins.Manager.PluginException(sectionInclude.Type, sectionInclude.Name)
                End If
                jitems.Add("{""Name"":""" & sectionInclude.Name & """,""Version"":" & JSON.JsonConversion.Encode(status & " " & sectionInclude.Type) & "}")
            Next
            str &= String.Join(",", jitems.ToArray()) & "]"
            str &= ","

            str &= """Admin"":["
            jitems = New List(Of String)
            For Each sectionInclude In r2i.OWS.Framework.Config.Items(Config.Section.General, Config.SectionType.Administration)
                jitems.Add("{""Name"":""" & sectionInclude.Name & """,""Version"":" & JSON.JsonConversion.Encode(sectionInclude.Type) & "}")
            Next
            For Each sectionInclude In r2i.OWS.Framework.Config.Items(Config.Section.Wrapper, Config.SectionType.Administration)
                jitems.Add("{""Name"":""" & sectionInclude.Name & """,""Version"":" & JSON.JsonConversion.Encode(sectionInclude.Type) & "}")
            Next
            For Each sectionInclude In r2i.OWS.Framework.Config.Items(Config.Section.Actions, Config.SectionType.Administration)
                jitems.Add("{""Name"":""" & sectionInclude.Name & """,""Version"":" & JSON.JsonConversion.Encode(sectionInclude.Type) & "}")
            Next
            For Each sectionInclude In r2i.OWS.Framework.Config.Items(Config.Section.Queryies, Config.SectionType.Administration)
                jitems.Add("{""Name"":""" & sectionInclude.Name & """,""Version"":" & JSON.JsonConversion.Encode(sectionInclude.Type) & "}")
            Next
            For Each sectionInclude In r2i.OWS.Framework.Config.Items(Config.Section.Tokens, Config.SectionType.Administration)
                jitems.Add("{""Name"":""" & sectionInclude.Name & """,""Version"":" & JSON.JsonConversion.Encode(sectionInclude.Type) & "}")
            Next
            For Each sectionInclude In r2i.OWS.Framework.Config.Items(Config.Section.Formats, Config.SectionType.Administration)
                jitems.Add("{""Name"":""" & sectionInclude.Name & """,""Version"":" & JSON.JsonConversion.Encode(sectionInclude.Type) & "}")
            Next
            str &= String.Join(",", jitems.ToArray()) & "]"
            str &= ","

            str &= """UI"":["
            jitems = New List(Of String)
            For Each sectionInclude In r2i.OWS.Framework.Config.Items(Config.Section.General, Config.SectionType.UI)
                jitems.Add("{""Name"":""" & sectionInclude.Name & """,""Version"":" & JSON.JsonConversion.Encode(sectionInclude.Type) & ",""Required"":" & JSON.JsonConversion.Encode(sectionInclude.Required) & ",""Title"":""" & sectionInclude.Title & """}")
            Next
            For Each sectionInclude In r2i.OWS.Framework.Config.Items(Config.Section.Wrapper, Config.SectionType.UI)
                jitems.Add("{""Name"":""" & sectionInclude.Name & """,""Version"":" & JSON.JsonConversion.Encode(sectionInclude.Type) & ",""Required"":" & JSON.JsonConversion.Encode(sectionInclude.Required) & ",""Title"":""" & sectionInclude.Title & """}")
            Next
            For Each sectionInclude In r2i.OWS.Framework.Config.Items(Config.Section.Actions, Config.SectionType.UI)
                jitems.Add("{""Name"":""" & sectionInclude.Name & """,""Version"":" & JSON.JsonConversion.Encode(sectionInclude.Type) & ",""Required"":" & JSON.JsonConversion.Encode(sectionInclude.Required) & ",""Title"":""" & sectionInclude.Title & """}")
            Next
            For Each sectionInclude In r2i.OWS.Framework.Config.Items(Config.Section.Queryies, Config.SectionType.UI)
                jitems.Add("{""Name"":""" & sectionInclude.Name & """,""Version"":" & JSON.JsonConversion.Encode(sectionInclude.Type) & ",""Required"":" & JSON.JsonConversion.Encode(sectionInclude.Required) & ",""Title"":""" & sectionInclude.Title & """}")
            Next
            For Each sectionInclude In r2i.OWS.Framework.Config.Items(Config.Section.Tokens, Config.SectionType.UI)
                jitems.Add("{""Name"":""" & sectionInclude.Name & """,""Version"":" & JSON.JsonConversion.Encode(sectionInclude.Type) & ",""Required"":" & JSON.JsonConversion.Encode(sectionInclude.Required) & ",""Title"":""" & sectionInclude.Title & """}")
            Next
            For Each sectionInclude In r2i.OWS.Framework.Config.Items(Config.Section.Formats, Config.SectionType.UI)
                jitems.Add("{""Name"":""" & sectionInclude.Name & """,""Version"":" & JSON.JsonConversion.Encode(sectionInclude.Type) & ",""Required"":" & JSON.JsonConversion.Encode(sectionInclude.Required) & ",""Title"":""" & sectionInclude.Title & """}")
            Next
            str &= String.Join(",", jitems.ToArray()) & "]"
            str &= ","

            'ACTIONS
            str &= """Versions"":["
            jitems = New List(Of String)
            Dim strKey As String
            For Each strKey In assemList.Keys
                jitems.Add("{""Name"":""" & strKey & """,""Version"":""" & assemList(strKey) & """}")
            Next
            str &= String.Join(",", jitems.ToArray()) & "]"
            str &= ","

            'ISSUES
            str &= """Issues"":["
            jitems = New List(Of String)

            If Not Utilities.Utility.Security_FilePermission Then
                jitems.Add("{""Name"":""" & "FileIO Permissions" & """,""Value"":""" & "You do not have adequate permissions to provide all File interactions. Please check with your host and try again. Open Web Studio operates at its peak under Full Trust." & """}")
            Else
                jitems.Add("{""Name"":""" & "FileIO Permissions" & """,""Value"":""" & "Full Access" & """}")
            End If
            If Not Utilities.Utility.Security_ReflectPermission Then
                jitems.Add("{""Name"":""" & "Reflection Permissions" & """,""Value"":""" & "You do not have adequate reflection permissions. Your application will probably suffer severe limitiations or unexpected behavior. Please check with your host and try again. Open Web Studio operates at its peak under Full Trust." & """}")
            Else
                jitems.Add("{""Name"":""" & "Reflection Permissions" & """,""Value"":""" & "Full Access" & """}")
            End If
            str &= String.Join(",", jitems.ToArray()) & "]"


            str &= "}"
        Catch ex As Exception
            Try
                DotNetNuke.Services.Exceptions.LogException(New Exception("OpenWebStudio was unable to provide access to the administrator request: " & ex.ToString))
            Catch ex2 As Exception
            End Try
            AssignError(2, ex)
        End Try
        Return str
    End Function
    Private __error As String = Nothing
    Private Property [ErrorMsg]() As String
        Get
            Return __error
        End Get
        Set(ByVal value As String)
            __error = value
        End Set
    End Property
    Private Sub ShowError()
        Response.Clear()
        Response.Write(ErrorMsg)
        Response.End()
    End Sub
    Private Sub AssignError(ByVal code As Integer, ByVal ex As Exception)
        If ErrorMsg Is Nothing Then
            Dim str As String = ""
            If code = 1 Then
                str = ("<html><body><style>div.Illegal { padding: 30px; font-family: arial; font-size: 14px; color: #bb0000; border: 1px solid #bb0000; background: #FF9999; margin: 30% 30%;}</style><div class=""Illegal"">")
                Str &= ("You have attempted to initiate the Open Web Studio administration interface without the proper credentials. You must be granted ""Host Access"" to this system to be granted passage into the IDE. Please contact your administrator and try your attempt again. <a href=""#"" onclick=""document.getElementById('divExc').style.display='block';return false;"">View the exception</a>.")
                Str &= ("<div id=""divExc"" style=""display:none;"">")
                Str &= (Framework.Utilities.Utility.HTMLEncode(ex.ToString))
                Str &= ("</div>")
                Str &= ("</div></body></html>")
            ElseIf code = 2 Then
                DotNetNuke.Services.Exceptions.LogException(ex)

                str = ("<html><body><style>div.Info { padding: 30px; font-family: arial; font-size: 14px; color: #000000; border: 1px solid #bbbb00; background: #FFFF99; margin: 30% 30%;}</style><div class=""Info"">")
                str &= ("You have attempted to execute the administrative window without first loading the wrapper. You must load the wrapper in the environment first. In most cases you simply need to refresh the application in the browser and resume your operations here. Please try again. <a href=""#"" onclick=""document.getElementById('divExc').style.display='block';return false;"">View the exception</a>.")
                str &= ("<div id=""divExc"" style=""display:none;"">")
                str &= (Framework.Utilities.Utility.HTMLEncode(ex.ToString))
                str &= ("</div>")
                str &= ("</div></body></html>")

            End If
            ErrorMsg = str
        End If
    End Sub

    Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete
        If Not ErrorMsg Is Nothing Then
            ShowError()
        End If
    End Sub
End Class