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
Imports r2i.OWS.Framework
Imports r2i.OWS.Framework.Utilities
Imports r2i.OWS.Framework.Plugins.Renderers
Imports r2i.OWS.Framework.DataAccess
Imports r2i.OWS.Framework.Entities
Imports System.Web.UI
Imports System.Text.RegularExpressions

Namespace r2i.OWS.Wrapper.DNN.Extensions.Professional.Renderers
    Public Class RenderTextEditor
        Inherits RenderBase

        Public Overrides ReadOnly Property RenderTag() As String
            Get
                Return "TEXTEDITOR"
            End Get
        End Property

        Public Overrides ReadOnly Property RenderType() As RenderTypes
            Get
                Return RenderTypes.Functional
            End Get
        End Property

        Public Overrides Function Handle_Render(ByRef Caller As EngineBase, ByVal Index As Integer, ByRef Source As String, ByRef DS As System.Data.DataSet, ByRef DR As System.Data.DataRow, ByRef RuntimeMessages As System.Collections.Generic.SortedList(Of String, String), ByVal NullReturn As Boolean, ByVal NullOverride As Boolean, ByVal ProtectSession As Boolean, ByVal SessionDelimiter As String, ByVal useSessionQuotes As Boolean, ByVal useAggregations As Boolean, ByRef FilterText As String, ByRef FilterField As String, ByRef Debugger As r2i.OWS.Framework.Debugger) As Boolean
            Dim REPLACED As Boolean = False
            Try
                Dim parameters As String() = ParameterizeString(Source, ","c, """"c, "\"c)
                If Not parameters Is Nothing AndAlso parameters.Length > 2 Then
                    Dim idnameparameter As String = parameters(1)
                    Dim contentparameter As String = Nothing
                    Dim width As String = "100%"
                    Dim height As String = "375"
                    If parameters.Length > 1 Then
                        contentparameter = parameters(2)
                        If parameters.Length > 3 Then
                            If parameters.Length >= 4 Then
                                'Name,Collection split.
                                If parameters(3).Length > 0 Then
                                    contentparameter = String.Format("{0},{1}", parameters(2), parameters(3))
                                Else
                                    contentparameter = parameters(2)
                                End If
                                If parameters.Length > 4 Then
                                    width = parameters(4)
                                    If parameters.Length > 5 Then
                                        height = parameters(5)
                                    End If
                                End If

                                Dim rv As RenderBase = Plugins.Manager.GetPlugin(Plugins.PluginTag.Create(Config.Section.Tokens.ToString.ToLower, RenderTypes.Variable.ToString.ToLower, "")) 'Common.GetRenderer("", RenderTypes.Variable)
                                If Not rv.Handle_Render(Caller, Index, contentparameter, DS, DR, RuntimeMessages, NullReturn, NullReturn, ProtectSession, SessionDelimiter, useSessionQuotes, True, FilterText, FilterField, Debugger) Then
                                    contentparameter = ParameterMerge(CType(parameters, Array), Source, 2)
                                End If
                            Else
                                contentparameter = ParameterMerge(CType(parameters, Array), Source, 2)
                            End If
                        End If
                    End If
                    '<MOD:3>
                    If parameters.Length > 2 Then
                        'Dim sR As New IO.StreamReader(System.Reflection.Assembly.GetExecutingAssembly.GetManifestResourceStream("ChildControls.FTB.Data.Text"))
                        'Source = sR.ReadToEnd()
                        'Source = Source.Replace("[ListX.ChildControls.TextEditor,ID]", idnameparameter)
                        'Source = Source.Replace("[ListX.ChildControls.TextEditor,CONTENT]", contentparameter)
                        'REPLACED = True
                        'MERGE START
                        Try
                            Source = GetRichTextEditor(Caller, Caller.Caller.Page, Caller.ModuleID, idnameparameter, width, height, contentparameter)
                            Firewall.Firewall(Source, True, Firewall.FirewallDirectiveEnum.None, False)
                            DotNetNuke.UI.Utilities.ClientAPI.RegisterClientVariable(Caller.Caller.Page, "editorModuleId", Caller.ModuleID, True)
                            DotNetNuke.UI.Utilities.ClientAPI.RegisterClientVariable(Caller.Caller.Page, "editorTabId", Caller.TabID, True)
                            DotNetNuke.UI.Utilities.ClientAPI.RegisterClientVariable(Caller.Caller.Page, "editorPortalId", Caller.PortalID, True)
                            DotNetNuke.UI.Utilities.ClientAPI.RegisterClientVariable(Caller.Caller.Page, "editorHomeDirectory", Caller.PortalSettings.HomeDirectory, True)
                            DotNetNuke.UI.Utilities.ClientAPI.RegisterClientVariable(Caller.Caller.Page, "editorPortalGuid", Caller.PortalSettings.GUID.ToString, True)
                            DotNetNuke.UI.Utilities.ClientAPI.RegisterClientVariable(Caller.Caller.Page, "editorEnableUrlLanguage", False, True)
#If DEBUG Then
                            IO.File.WriteAllText(Caller.Caller.Page.MapPath(String.Format("~/OWS.TextEditor.{0}.html", Caller.ModuleID)), Source)
#End If
                        Catch es As Exception
                            If Not es Is Nothing Then
                                If Not Debugger Is Nothing AndAlso Not Caller Is Nothing AndAlso Not Caller.ModuleID Is Nothing Then
                                    Debugger.AppendBlock(Caller.ModuleID, "UI Render Error", "UI Render Error", False, es.ToString, True)
                                Else
                                    Throw es
                                End If
                            Else
                                Debugger.AppendBlock(Caller.ModuleID, "UI Render Error", "UI Render Error", False, "Nothing was returned", True)
                            End If
                            Source = String.Format("<h1>{0}</h1>", es)
                        End Try
                        'MERGE STOP
                        REPLACED = True

                    End If
                    '</MOD:3>
                End If
            Catch ex As Exception
                Source = ex.ToString
                REPLACED = True
            End Try
            Return REPLACED
        End Function

        Partial Public Class HybridEditorPage
            Inherits System.Web.UI.Page

            Private _caller As EngineBase
            Private _width As String
            Private _height As String
            Private _value As String
            Private _id As String
            Private _moduleId As String

            Public Sub New(ByRef Caller As EngineBase, ByVal moduleId As String, ByVal id As String, ByVal value As String, ByVal width As String, ByVal height As String)
                _caller = Caller
                _width = width
                _height = height
                _id = id
                _moduleId = moduleId
                _value = value
            End Sub
            Public Shadows ReadOnly Property Request As System.Web.HttpRequest
                Get
                    Return (_caller.Request)
                End Get
            End Property
            Protected Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Init
                Dim frm As New System.Web.UI.HtmlControls.HtmlForm() With
                {
                    .ID = "Form"
                }
                Me.Controls.Add(frm)

                Dim sysCM As New ScriptManager() With
                {
                    .ID = "ScriptManager",
                    .LoadScriptsBeforeUI = True,
                    .EnablePartialRendering = True,
                    .ScriptMode = ScriptMode.Release
                }
                Me.Form.Controls.Add(sysCM)

                DotNetNuke.UI.Utilities.ClientAPI.RegisterClientVariable(Me, "editorModuleId", _caller.ModuleID, True)
                DotNetNuke.UI.Utilities.ClientAPI.RegisterClientVariable(Me, "editorTabId", _caller.TabID, True)
                DotNetNuke.UI.Utilities.ClientAPI.RegisterClientVariable(Me, "editorPortalId", _caller.PortalID, True)
                DotNetNuke.UI.Utilities.ClientAPI.RegisterClientVariable(Me, "editorHomeDirectory", _caller.PortalSettings.HomeDirectory, True)
                DotNetNuke.UI.Utilities.ClientAPI.RegisterClientVariable(Me, "editorPortalGuid", _caller.PortalSettings.GUID.ToString, True)
                DotNetNuke.UI.Utilities.ClientAPI.RegisterClientVariable(Me, "editorEnableUrlLanguage", False, True)
            End Sub

            Protected Sub Page_Load(ByVal Sender As Object, ByVal e As EventArgs) Handles MyBase.Load
                Dim rte As DotNetNuke.Modules.HTMLEditorProvider.HtmlEditorProvider = DotNetNuke.Modules.HTMLEditorProvider.HtmlEditorProvider.Instance()
                rte.ID = "dnn_ctr" + _moduleId + "_" + _id
                rte.ControlID = "dnn_ctr" + _moduleId + "_" + _id

                rte.Page = Page
                rte.Initialize()

                'PAGE LOAD
                If _width.EndsWith("%") Then
                    _width = _width.Remove(_width.Length - 1, 1)
                    rte.Width = System.Web.UI.WebControls.Unit.Percentage(CInt(_width))
                Else
                    rte.Width = System.Web.UI.WebControls.Unit.Pixel(CInt(_width))
                End If
                If _height.EndsWith("%") Then
                    _height = _height.Remove(_height.Length - 1, 1)
                    rte.Height = System.Web.UI.WebControls.Unit.Percentage(CInt(_height))
                Else
                    rte.Height = System.Web.UI.WebControls.Unit.Pixel(CInt(_height))
                End If
                If _value.StartsWith("&lt;") Then
                    _value = Web.HttpUtility.HtmlDecode(_value)
                End If
                If _value.StartsWith("%3c") Then
                    _value = Web.HttpUtility.UrlDecode(_value)
                End If
                rte.Text = _value
                'rte.Initialize()
                Try
                    rte.HtmlEditorControl.GetType().InvokeMember("OnPreRender", Reflection.BindingFlags.InvokeMethod Or Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance, Nothing, rte.HtmlEditorControl, New Object() {Nothing})
                Catch Ex As Exception

                End Try

                Form.Controls.Add(rte.HtmlEditorControl)
            End Sub
        End Class
        Public Function GetRichTextEditor(ByRef Caller As EngineBase, ByRef Page As System.Web.UI.Page, ByVal ModuleID As String, ByVal IdNameParameter As String, ByVal Width As String, ByVal Height As String, ByVal Value As String) As String
            Dim sb As New System.Text.StringBuilder
            Dim tw As New IO.StringWriter(sb)
            Dim twHTML As New System.Web.UI.HtmlTextWriter(tw)
            Dim sval As String
            Dim hp As New HybridEditorPage(Caller, ModuleID, IdNameParameter, Value, Width, Height)

            Try
                Caller.Context.Server.Execute(hp, twHTML, True)
            Catch ex As Exception
                'Ignore this...
#If TESTOUTPUT Then
                DotNetNuke.Services.Exceptions.LogException(ex)
#End If
            End Try

            tw.Flush()
#If TESTOUTPUT Then
            Dim tc As New DotNetNuke.Entities.Tabs.TabController()
            Dim activeTab As DotNetNuke.Entities.Tabs.TabInfo = tc.GetTab(CInt(Caller.PortalSettings.ActiveTab.TabID), Caller.PortalSettings.PortalId, False)

            sb.AppendLine("<!--")
            sb.AppendLine(String.Format("Caller.PortalID = {0}", Caller.PortalID))
            sb.AppendLine(String.Format("Caller.PortalSettings.HTTPAlias = {0}", Caller.PortalSettings.HTTPAlias))
            sb.AppendLine(String.Format("FriendlyUrl = {0}", DotNetNuke.Services.Url.FriendlyUrl.FriendlyUrlProvider.Instance().FriendlyUrl(activeTab, "")))
            sb.AppendLine("-->")
            IO.File.WriteAllText(Caller.Context.Server.MapPath("~/BeforeReplacements.html"), sb.ToString())
#End If
            Dim sPortalPrefix As String = String.Empty
            Dim mtch As Match = Regex.Match(sb.ToString(), """(/[^/]*)?/ScriptResource.axd")

            If mtch.Success AndAlso mtch.Groups.Count > 1 Then
                sPortalPrefix = mtch.Groups(1).Value
            End If
            'sval = Regex.Replace(sb.ToString(), "([""'])~?/DesktopModules/Admin/RadEditorProvider/([^""']*[""'])", String.Format("$1{0}/DesktopModules/Admin/RadEditorProvider/$2", sPortalPrefix), RegexOptions.Multiline Or RegexOptions.IgnoreCase)
            sval = Regex.Replace(sb.ToString(), "([""'])~/([^""']*[""'])", String.Format("$1{0}/$2", sPortalPrefix), RegexOptions.Multiline Or RegexOptions.IgnoreCase)
            sval = Strip(sval, (Caller.Caller.Page.IsPostBack))

#If TESTOUTPUT Then
            IO.File.WriteAllText(Caller.Context.Server.MapPath("~/AfterReplacements.html"), sval)
#End If
            If Page.IsPostBack Then

            End If

            Return sval
        End Function

        Private Function Strip(ByVal value As String, ByVal IsPostback As Boolean) As String
            Dim s As Integer = 0
            Dim e As Integer = 0
            'strip form

            If value.IndexOf("<form") >= 0 Then
                e = value.IndexOf(">", value.IndexOf("<form")) + 1
                value = value.Substring(e)
                e = value.IndexOf("</form>")
                While e > 2
                    value = value.Substring(0, (e - 1))
                    e = value.IndexOf("</form>")
                End While
            End If

            'first div and first script block
            If value.IndexOf("</script>") >= 0 Then
                value = value.Substring(value.IndexOf("</script>") + "</script>".Length)
            End If

            'remove initialization of scriptmanager
            If value.IndexOf("_initialize('ScriptManager'") >= 0 Then
                s = value.Substring(0, value.IndexOf("_initialize('ScriptManager'")).LastIndexOf("<script")
                e = value.IndexOf("</script>", s) + "</script>".Length
                value = value.Substring(0, s) & value.Substring(e)
            End If

            'remove _dnnVariable input
            'If value.IndexOf("<input name=""__dnnVariable") >= 0 Then
            '    s = value.IndexOf("<input name=""__dnnVariable")
            '    e = value.IndexOf(">", s) + 1
            '    value = value.Substring(0, s) & value.Substring(e)
            'End If

            'remove __EVENTVALIDATION input
            value = Regex.Replace(value, "<input\s.*name=\""__EVENTVALIDATION\""[^>]*/?>", String.Empty)
            value = Regex.Replace(value, "<input\s.*name=\""__dnnVariable\""[^>]*/?>", String.Empty)

            'add ClientScripts js after RegisterDialogs
            s = value.IndexOf("RegisterDialogs.js")
            If s >= 0 And value.IndexOf("ClientScripts.js") <= 0 Then
                s = value.Substring(0, s).LastIndexOf("<")
                e = value.IndexOf("</script>", s) + "</script>".Length
                Dim r As String = value.Substring(s, e - s)
                value = value.Substring(0, e) & r.Replace("RegisterDialogs", "ClientScripts") & value.Substring(e)
            End If


            'remove all aspNetHidden divs
            s = value.IndexOf("<div class=""aspNetHidden""")
            While s >= 0
                e = value.IndexOf("</div>", s)
                If s >= 0 And e >= 0 Then
                    value = value.Substring(0, s) & value.Substring(e + "</div>".Length)
                End If
                s = value.IndexOf("<div class=""aspNetHidden""")
            End While
            Return value
        End Function
    End Class
End Namespace