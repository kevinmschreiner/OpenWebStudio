Imports DotNetNuke.Common
Imports DotNetNuke.Entities
Imports DotNetNuke.Services
Imports r2i.OWS

Namespace DataAccess.Factories
    Public Class EngineFactory
        Private Shared _instance As EngineFactory = New EngineFactory()
        Private Shared TabIdLookup As New Dictionary(Of String, Integer)
        Public Shared ReadOnly Property Instance() As EngineFactory
            Get
                Return _instance
            End Get
        End Property

        Public Function NavigateURL() As String
            Return DotNetNuke.Common.Globals.NavigateURL()
        End Function

        Public Function NavigateURL(ByVal sTab As String, ByVal PortalId As Integer) As String
            Dim tabId As Integer = -1
            If Not TabIdLookup.ContainsKey(PortalId.ToString & "~:!:~" & sTab) Then
                Dim Tab As DotNetNuke.Entities.Tabs.TabInfo = (New DotNetNuke.Entities.Tabs.TabController).GetTabByName(sTab, PortalId)
                If Not Tab Is Nothing Then
                    TabIdLookup.Add(PortalId.ToString & "~:!:~" & sTab, Tab.TabID)
                    tabId = Tab.TabID
                End If
            Else
                tabId = TabIdLookup.Item(PortalId.ToString & "~:!:~" & sTab)
            End If
            If tabId >= 0 Then
                Return NavigateURL(tabId)
            Else
                Return Me.NavigateURL()
            End If
        End Function

        Public Function NavigateURL(ByVal itab As Integer) As String
            Return DotNetNuke.Common.Globals.NavigateURL(itab)
        End Function

        Public Function NavigateURL(ByVal itab As Integer, ByVal sControl As String) As String
            Return DotNetNuke.Common.Globals.NavigateURL(itab, sControl)
        End Function

        Public Function NavigateURL(ByVal itab As Integer, ByVal sControl As String, ByVal strParameters() As String) As String
            Return DotNetNuke.Common.Globals.NavigateURL(itab, sControl, strParameters)
        End Function

        Private Shared s_Urls As SortedList
        Public Function FriendlyUrlByTabID(ByVal tabId As String, ByVal path As String) As String
            'Static Urls As SortedList
            If s_Urls Is Nothing Then
                s_Urls = New SortedList
            End If
            If Not s_Urls.ContainsKey("t" & tabId) Then
                Dim Str As String
                'Dim tinfo As Tabs.TabInfo
                'Dim tc As New Tabs.TabController
                'tinfo = tc.GetTab(CInt(tabId))
                'If Not tinfo Is Nothing Then
                '    Dim str As String
                '    If path.Length = 0 Then
                '        path = tinfo.TabPath
                '    End If
                Str = NavigateURL(CType(tabId, Integer))
                s_Urls("t" & tabId) = Str
                Return Str
                ' End If
            Else
                Return CStr(s_Urls("t" & tabId))
            End If
            Return String.Empty
        End Function

        Public Function PageLookupBy(ByVal value As String, ByVal column As String) As String
            Dim tid As String = ""
            Try
                Dim q As String = DataProvider.Instance().ObjectQualifier
                Dim o As String = DataProvider.Instance().DatabaseOwner
                If Not q Is Nothing AndAlso q.Length > 0 Then
                    If Not q.StartsWith("[") Then
                        q = "[" & q & "Tabs" & "]"
                    Else
                        q = q & "Tabs" & "]"
                    End If
                Else
                    q = "[Tabs]"
                End If
                If Not o Is Nothing AndAlso o.Length > 0 Then
                    If Not o.StartsWith("[") AndAlso Not o.EndsWith(".") Then
                        o = "[" & o & "]"
                    End If
                    If o.EndsWith(".") Then
                        q = o & q
                    Else
                        q = o & "." & q
                    End If
                End If
                If Not column Is Nothing AndAlso Not value Is Nothing AndAlso column.Length > 0 Then
                    Dim rValue As String
                    rValue = TableLookupBy(value, q, "TabID", column)
                    If IsNumeric(rValue) Then
                        tid = rValue
                    End If
                End If
            Catch ex As Exception
            End Try
            Return tid
        End Function

        Public Function TableLookupBy(ByVal Value As String, ByVal Tablename As String, ByVal returnColumn As String, ByVal Column As String) As String
            Dim rvalue As String = ""
            Try
                If Not Column Is Nothing AndAlso Not Value Is Nothing AndAlso Column.Length > 0 Then
                    If Value.IndexOf("'") >= 0 OrElse Not IsNumeric(Value) Then
                        Value = "'" & Value & "'"
                    End If
                    If Not returnColumn.StartsWith("[") Then
                        returnColumn = "[" & returnColumn & "]"
                    End If
                    Dim SQL As String = "SELECT " & returnColumn & " FROM " & Tablename & " WHERE [" & Column & "] = " & Value
                    Dim idr As IDataReader
                    idr = DotNetNuke.Data.DataProvider.Instance.ExecuteSQL(SQL)
                    If Not idr Is Nothing AndAlso idr.Read Then
                        rvalue = CStr(idr(0))
                    End If
                    idr.Close()
                    idr.Dispose()
                    idr = Nothing
                End If
            Catch ex As Exception
            End Try
            Return rvalue
        End Function

        Public Function Dotnetnuke_GetPassword(ByVal UserObj As IUser, ByVal answertext As String) As String
            Dim strPassword As String
            Try
                If Not UserObj Is Nothing AndAlso Not UserObj.Id Is Nothing AndAlso UserObj.Id.Length > 0 AndAlso UserObj.Id <> "-1" Then
                    Dim ctrl As New Users.UserController

                    Dim t As Type = ctrl.GetType
                    Dim miarray As System.Reflection.MemberInfo() = ctrl.GetType().GetMember("GetPassword")

                    strPassword = CStr(t.InvokeMember("GetPassword", Reflection.BindingFlags.InvokeMethod, Nothing, ctrl, New Object() {UserObj, answertext}))
                Else
                    strPassword = ""
                End If
            Catch ex As Exception
                strPassword = "Dotnetnuke no longer supports this capability"
            End Try
            Return strPassword
        End Function

        Public Function ProviderOwner() As String
            Dim ProviderType As String = "data"
            Dim objConfiguration As DotNetNuke.Framework.Providers.ProviderConfiguration = DotNetNuke.Framework.Providers.ProviderConfiguration.GetProviderConfiguration(ProviderType)
            Dim objProvider As DotNetNuke.Framework.Providers.Provider = CType(objConfiguration.Providers(objConfiguration.DefaultProvider), DotNetNuke.Framework.Providers.Provider)
            Dim strOwner As String

            strOwner = objProvider.Attributes("databaseOwner")
            Return strOwner
        End Function

        Public Function GetConnectionString() As String
            Return DotNetNuke.Common.Utilities.Config.GetConnectionString("SiteSqlServer")
        End Function

        Public Function GetApplicationPath() As String
            Return ApplicationPath
        End Function

        Public Function GetApplicationMapPath() As String
            Return ApplicationMapPath
        End Function

        Public Function GetLocalization(ByVal param As String) As String
            Return Localization.Localization.GetString(param)
        End Function

        Public Function GetLocalization(ByVal sKey As String, ByVal fileName As String) As String
            Return Localization.Localization.GetString(sKey, fileName)
        End Function

        Public Sub UpdateModuleSetting(ByVal configurationId As Guid, ByVal name As String, ByVal value As String)
            Dim mc As New DotNetNuke.Entities.Modules.ModuleController
            Dim moduleId As Integer
            Dim tabmodId() As String = DataProvider.Instance().GetConfigNameByConfigurationId(configurationId)
            If Not tabmodId Is Nothing Then
                If Integer.TryParse(tabmodId(1), moduleId) Then
                    mc.UpdateModuleSetting(moduleId, name, value)
                End If
                'Todo: implements Exception Unable to get the moduleId
            Else
                'Todo: implements Exception Unable to get the configName
            End If
            mc = Nothing
        End Sub

        Public Sub UpdateModuleSetting(ByVal moduleId As String, ByVal name As String, ByVal value As String)
            Dim mc As New DotNetNuke.Entities.Modules.ModuleController
            Dim moduleIdConv As Integer
            If Integer.TryParse(moduleId, moduleIdConv) Then
                mc.UpdateModuleSetting(moduleIdConv, name, value)
            End If
            mc = Nothing
        End Sub

        Public Sub UpdatePageModuleSetting(ByVal tabmoduleId As String, ByVal name As String, ByVal value As String)
            Dim mc As New DotNetNuke.Entities.Modules.ModuleController
            Dim moduleIdConv As Integer
            If Integer.TryParse(tabmoduleId, moduleIdConv) Then
                mc.UpdateTabModuleSetting(moduleIdConv, name, value)
            End If
            mc = Nothing
        End Sub

        Public Function GetHostSettings(ByVal parameter As String) As String
            Return DotNetNuke.Entities.Controllers.HostController.Instance().GetSettings()(parameter).Value '(Globals.HostSettings(parameter))
        End Function

        Public Shared Function GetCache(ByVal CacheKey As String) As Object
            Return DotNetNuke.Common.Utilities.DataCache.GetCache(CacheKey)
        End Function

        Public Shared Sub SetCache(ByVal CacheKey As String, ByVal curObject As Object)
            DotNetNuke.Common.Utilities.DataCache.SetCache(CacheKey, curObject)
        End Sub

        Public Function GetRichTextEditor(ByRef Page As System.Web.UI.Page, ByVal IdNameParameter As String, ByVal Width As String, ByVal Height As String, ByVal Value As String) As String
            Dim sb As New System.Text.StringBuilder
            Dim tw As New IO.StringWriter(sb)
            Dim twHTML As New System.Web.UI.HtmlTextWriter(tw)

            Dim ctlRTE As DotNetNuke.Modules.HTMLEditorProvider.HtmlEditorProvider = DotNetNuke.Modules.HTMLEditorProvider.HtmlEditorProvider.Instance()
            ctlRTE.ControlID = IdNameParameter
            ctlRTE.ID = IdNameParameter
            ctlRTE.Initialize()
            ctlRTE.HtmlEditorControl.ID = IdNameParameter

            ctlRTE.Page = Page
            ctlRTE.HtmlEditorControl.Page = Page

            If Width.EndsWith("%") Then
                Width = Width.Remove(Width.Length - 1, 1)
                ctlRTE.Width = System.Web.UI.WebControls.Unit.Percentage(CInt(Width))
            Else
                ctlRTE.Width = System.Web.UI.WebControls.Unit.Pixel(CInt(Width))
            End If
            If Height.EndsWith("%") Then
                Height = Width.Remove(Width.Length - 1, 1)
                ctlRTE.Height = System.Web.UI.WebControls.Unit.Percentage(CInt(Height))
            Else
                ctlRTE.Height = System.Web.UI.WebControls.Unit.Pixel(CInt(Height))
            End If

            ctlRTE.Text = Value

            ctlRTE.HtmlEditorControl.GetType().InvokeMember("OnInit", Reflection.BindingFlags.InvokeMethod Or Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance, Nothing, ctlRTE.HtmlEditorControl, New Object() {Nothing})
            ctlRTE.HtmlEditorControl.GetType().InvokeMember("OnPreRender", Reflection.BindingFlags.InvokeMethod Or Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance, Nothing, ctlRTE.HtmlEditorControl, New Object() {Nothing})

            ctlRTE.HtmlEditorControl.RenderControl(twHTML)

            tw.Flush()
            Return sb.ToString
        End Function

        Public Function GetOpenControlBase(ByRef Page As System.Web.UI.Page, ByVal Id As String, ByVal ModuleID As String, ByVal PageID As String, ByVal ConfigurationID As String, ByVal ResourceFile As String, ByVal ResourceKey As String, ByVal ModulePath As String, ByVal BasePath As String, ByVal ListSource As String, ByVal ControlType As String) As String
            Dim sb As New System.Text.StringBuilder
            Dim tw As New IO.StringWriter(sb)
            Dim twHTML As New System.Web.UI.HtmlTextWriter(tw)

            Dim ctlRTE As New OpenControl()
            ctlRTE.ID = Id
            ctlRTE.ModuleId = ModuleID
            ctlRTE.PageId = PageID
            ctlRTE.ControlTag = ControlType
            ctlRTE.SetOWSPath(BasePath)
            ctlRTE.SetModulePath(ModulePath)
            ctlRTE.SetUniqueID(Id)

            If Not ConfigurationID Is Nothing AndAlso ConfigurationID.Length > 0 Then
                Try
                    ctlRTE.ConfigurationId = New System.Guid(ConfigurationID)
                Catch ex As Exception
                End Try
            End If
            If Not ResourceFile Is Nothing AndAlso ResourceFile.Length > 0 Then
                Try
                    ctlRTE.ResourceFile = ResourceFile
                Catch ex As Exception
                End Try
            End If
            If Not ResourceKey Is Nothing AndAlso ResourceKey.Length > 0 Then
                Try
                    ctlRTE.ResourceKey = ResourceKey
                Catch ex As Exception
                End Try
            End If

            Dim parent As New Web.UI.Page()

            parent.Controls.Add(ctlRTE)
            ctlRTE.Page = Page

            ctlRTE.GetType().InvokeMember("OnInit", Reflection.BindingFlags.InvokeMethod Or Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance, Nothing, ctlRTE, New Object() {Nothing})
            ctlRTE.GetType().InvokeMember("OnLoad", Reflection.BindingFlags.InvokeMethod Or Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance, Nothing, ctlRTE, New Object() {Nothing})
            ctlRTE.GetType().InvokeMember("OnPreRender", Reflection.BindingFlags.InvokeMethod Or Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance, Nothing, ctlRTE, New Object() {Nothing})

            ctlRTE.RenderControl(twHTML)
            ctlRTE.GetType().InvokeMember("OnUnload", Reflection.BindingFlags.InvokeMethod Or Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance, Nothing, ctlRTE, New Object() {Nothing})

            tw.Flush()
            Return sb.ToString
        End Function
    End Class
End Namespace

