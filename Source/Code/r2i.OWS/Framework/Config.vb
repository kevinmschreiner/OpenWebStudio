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
Imports System.IO
Imports System.Text
Imports System.Xml
Imports System.Xml.Serialization
Imports System.Reflection

Namespace r2i.OWS.Framework
    Public Class Config
        Public Enum Section
            Formats
            Queryies
            Tokens
            Actions
            Wrapper
            General
        End Enum
        Public Enum SectionType
            Administration
            Runtime
            UI
        End Enum
        Public Class SectionItem
            Private _name As String
            Private _type As String
            Private _required As String
            Private _title As String
            Private _mime As String
            Private _path As String
            Private _sectiontype As SectionType
            Private _section As Section

            ''' <summary>
            ''' If SectionType is Administration, then "Type" means the ScriptPath
            ''' </summary>
            ''' <param name="Name"></param>
            ''' <param name="Type"></param>
            ''' <param name="Section"></param>
            ''' <param name="SectionType"></param>
            ''' <remarks></remarks>
            Public Sub New(ByVal Name As String, ByVal Type As String, ByVal Section As Section, ByVal SectionType As SectionType)
                _name = Name
                _type = Type
                _section = Section
                _sectiontype = SectionType
            End Sub
            ''' <summary>
            ''' If SectionType is Administration, then "Type" means the ScriptPath
            ''' </summary>
            ''' <param name="Name"></param>
            ''' <param name="Type"></param>
            ''' <param name="Section"></param>
            ''' <param name="SectionType"></param>
            ''' <param name="Title"></param>
            ''' <remarks></remarks>
            Public Sub New(ByVal Name As String, ByVal Type As String, ByVal Section As Section, ByVal SectionType As SectionType, ByVal Title As String)
                _name = Name
                _type = Type
                _section = Section
                _sectiontype = SectionType
                _title = Title
            End Sub
            ''' <summary>
            ''' If SectionType is Administration, then "Type" means the ScriptPath
            ''' </summary>
            ''' <param name="Name"></param>
            ''' <param name="Type"></param>
            ''' <param name="Section"></param>
            ''' <param name="SectionType"></param>
            ''' <param name="Title"></param>
            ''' <param name="Required"></param>
            ''' <remarks></remarks>
            Public Sub New(ByVal Name As String, ByVal Type As String, ByVal Section As Section, ByVal SectionType As SectionType, ByVal Title As String, ByVal Required As String)
                _name = Name
                _type = Type
                _section = Section
                _sectiontype = SectionType
                _title = Title
                _required = Required
            End Sub
            ''' <summary>
            ''' If SectionType is Administration, then "Type" means the ScriptPath
            ''' </summary>
            ''' <param name="Name"></param>
            ''' <param name="Type"></param>
            ''' <param name="Section"></param>
            ''' <param name="SectionType"></param>
            ''' <param name="Title"></param>
            ''' <param name="Required"></param>
            ''' <remarks></remarks>
            Public Sub New(ByVal Name As String, ByVal Type As String, ByVal Section As Section, ByVal SectionType As SectionType, ByVal Title As String, ByVal Required As String, ByVal Mime As String, ByVal Path As String)
                _name = Name
                _type = Type
                _section = Section
                _sectiontype = SectionType
                _title = Title
                _required = Required
                _mime = Mime
                _path = Path
            End Sub
            Public Property Name() As String
                Get
                    Return _name
                End Get
                Set(ByVal value As String)
                    _name = value
                End Set
            End Property
            Public Property Type() As String
                Get
                    Return _type
                End Get
                Set(ByVal value As String)
                    _type = value
                End Set
            End Property
            Public Property Title() As String
                Get
                    If _title Is Nothing OrElse _title.Length = 0 Then
                        _title = _name
                    End If
                    Return _title
                End Get
                Set(ByVal value As String)
                    _title = value
                End Set
            End Property
            Public Property Mime() As String
                Get
                    Return _mime
                End Get
                Set(ByVal value As String)
                    _mime = value
                End Set
            End Property
            Public Property Path() As String
                Get
                    Return _path
                End Get
                Set(ByVal value As String)
                    _path = value
                End Set
            End Property
            Public Property Required() As String
                Get
                    If _required Is Nothing OrElse _required.ToLower <> "true" Then
                        Return "false"
                    Else
                        Return "true"
                    End If
                End Get
                Set(ByVal value As String)
                    _required = value
                End Set
            End Property
            Public Property SectionType() As SectionType
                Get
                    Return _sectiontype
                End Get
                Set(ByVal value As SectionType)
                    _sectiontype = value
                End Set
            End Property
            Public Property Section() As Section
                Get
                    Return _section
                End Get
                Set(ByVal value As Section)
                    _section = value
                End Set
            End Property
            Public Function Load() As Object
                If Not _type Is Nothing Then
                    'System.Reflection.Assembly.LoadFrom
                    Dim s() As String = _type.Split(",")
                    If s.Length > 1 Then
                        Dim asmb As System.Reflection.Assembly = System.Reflection.Assembly.Load(s(1))
                        Return asmb.CreateInstance(s(0))
                    End If
                End If
                Return Nothing
            End Function

            Public Function ToXMLNode(ByVal Doc As XmlDocument) As XmlNode
                Dim nd As XmlNode = Doc.CreateNode(XmlNodeType.Element, getSectionType(Me.SectionType), "")

                Dim xaName As XmlAttribute = Doc.CreateAttribute("name")
                xaName.Value = Me.Name
                nd.Attributes.Append(xaName)

                Dim xaType As XmlAttribute = Doc.CreateAttribute("type")
                xaType.Value = Me.Type
                nd.Attributes.Append(xaType)

                If Not Me.Title Is Nothing AndAlso Me.Title.Length > 0 Then
                    Dim xaTitle As XmlAttribute = Doc.CreateAttribute("title")
                    xaTitle.Value = Me.Title
                    nd.Attributes.Append(xaTitle)
                End If

                If Not Me.Mime Is Nothing AndAlso Me.Mime.Length > 0 Then
                    Dim xaMime As XmlAttribute = Doc.CreateAttribute("mime")
                    xaMime.Value = Me.Mime
                    nd.Attributes.Append(xaMime)
                End If

                If Not Me.Path Is Nothing AndAlso Me.Path.Length > 0 Then
                    Dim xaPath As XmlAttribute = Doc.CreateAttribute("path")
                    xaPath.Value = Me.Path
                    nd.Attributes.Append(xaPath)
                End If

                Dim xaRequired As XmlAttribute = Doc.CreateAttribute("required")
                xaRequired.Value = Me.Required
                nd.Attributes.Append(xaRequired)


                Return nd
            End Function
        End Class

        Private Shared Function getSection(ByRef Group As Section) As String
            Select Case Group
                Case Section.Actions
                    Return "actions"
                Case Section.Formats
                    Return "formats"
                Case Section.Queryies
                    Return "queries"
                Case Section.Tokens
                    Return "tokens"
                Case Section.Wrapper
                    Return "wrapper"
                Case Section.General
                    Return "general"
            End Select
            Return Nothing
        End Function
        Private Shared Function getSectionType(ByRef Type As SectionType) As String
            Select Case Type
                Case SectionType.Administration
                    Return "admin"
                Case SectionType.Runtime
                    Return "run"
                Case SectionType.UI
                    Return "ui"
            End Select
            Return Nothing
        End Function
        Private Shared Function getItems(ByRef group As Hashtable, ByRef Type As SectionType) As List(Of SectionItem)
            If Not group Is Nothing Then
                Return group.Item(getSectionType(Type))
            End If
            Return Nothing
        End Function
        Private Shared _config As Hashtable
        Private Shared _configWriterMutex As New System.Threading.Mutex

        Public Shared ReadOnly Property Items(ByVal Group As Section, ByVal Type As SectionType) As List(Of SectionItem)
            Get
                If Not _config Is Nothing Then
                    Return getItems(_config(getSection(Group)), Type)
                Else
                    Throw New Exception("The Configuration has not yet been loaded, please verify that the configuration loader is applied during the Initialize of the Abstract Factory.")
                End If
                Return Nothing
            End Get
        End Property
        Public Shared Sub Initialize(ByRef ConfigFilePath As String, ByRef BadConfigFilePath As String, Optional ByVal AdditionalItems As List(Of SectionItem) = Nothing)
            Dim haveMutex As Boolean = False
            Try
                haveMutex = _configWriterMutex.WaitOne()
                If haveMutex Then
                    Try
                        _config = New Hashtable
                        'LoadConfig(ConfigFilePath, BadConfigFilePath, AdditionalItems)
                        If Not LoadConfigs(ConfigFilePath, BadConfigFilePath, AdditionalItems) Then
                            GenerateConfig(ConfigFilePath, AdditionalItems)
                        End If

                        If _config.Keys.Count <= 1 Then
                            _config = Nothing
                        End If
                    Catch ex As Exception
                        Throw ex
                    End Try
                End If
            Catch ex As Exception
            Finally
                If haveMutex Then
                    Try
                        _configWriterMutex.ReleaseMutex()
                    Catch ex As Exception
                    End Try
                End If
            End Try
        End Sub
        Private Shared Function LoadConfigs(ByVal ConfigFilePath As String, ByVal BadFilePath As String, Optional ByVal AdditionalItems As List(Of SectionItem) = Nothing) As Boolean
            Dim breturn As Boolean = False
            Try
                Dim ds As New IO.DirectoryInfo(ConfigFilePath)
                If Not ds Is Nothing AndAlso ds.Exists Then
                    Dim fios As IO.FileSystemInfo() = ds.GetFiles("openwebstudio*.config", SearchOption.TopDirectoryOnly)
                    If Not fios Is Nothing AndAlso fios.Length > 0 Then
                        Dim fio As IO.FileSystemInfo
                        For Each fio In fios
                            Dim bval As Boolean = LoadConfig(GetConfig(fio.FullName))
                            If Not breturn And bval Then
                                breturn = bval
                            End If
                        Next
                    End If
                End If
            Catch ex As Exception

            End Try
            Return breturn
        End Function
        Private Shared Function GetConfig(ByVal Path As String) As String
            Dim fstream As IO.FileStream = Nothing
            Dim sreader As IO.StreamReader = Nothing
            Dim str As String = ""
            Try
                fstream = New IO.FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
                sreader = New IO.StreamReader(fstream)
                str = sreader.ReadToEnd()
                sreader.Close()
                sreader.Dispose()
            Catch ex As Exception
            Finally
                If Not fstream Is Nothing AndAlso fstream.CanRead Then
                    fstream.Close()
                End If
            End Try
            Return str
        End Function

        Private Shared Sub PrepareConfig_Section(ByVal xd As XmlDocument, ByVal Section As Section, ByVal SectionItems As List(Of SectionItem), ByRef Added As Boolean)
            Dim ndRoot As XmlElement = xd.SelectSingleNode(".//openwebstudio")

            Dim ndType As XmlElement = ndRoot.SelectSingleNode(getSection(Section))
            If ndType Is Nothing Then
                ndType = xd.CreateElement(getSection(Section))
                ndRoot.AppendChild(ndType)
            End If
            If Not SectionItems Is Nothing Then
                For Each si As SectionItem In SectionItems
                    If si.Section = Section Then
                        If ndType.SelectSingleNode(getSectionType(si.SectionType) & "[@name=""" & si.Name & """]") Is Nothing Then
                            ndType.AppendChild(si.ToXMLNode(xd))
                            Added = True
                        End If
                    End If
                Next
            End If
        End Sub
        Private Shared Sub GenerateConfig(ByVal BasePath As String, ByVal AdditionalSectionItems As List(Of SectionItem))
            Dim dio As New IO.DirectoryInfo(BasePath)
            If dio.Exists Then
                Dim fio As New IO.FileInfo(dio.FullName & "openwebstudio.default.config")
                If fio.Exists Then
                    fio.Delete()
                End If

                Dim xd As New XmlDocument()

                Dim ndRoot As XmlElement

                ndRoot = xd.CreateElement("openwebstudio")
                xd.AppendChild(ndRoot)

                Dim bAdded As Boolean = False

                PrepareConfig_Section(xd, Section.Wrapper, AdditionalSectionItems, bAdded)

                PrepareConfig_Section(xd, Section.Actions, AdditionalSectionItems, bAdded)
                PrepareConfig_Section(xd, Section.Actions, CoreActionItems, bAdded)

                PrepareConfig_Section(xd, Section.Formats, AdditionalSectionItems, bAdded)
                PrepareConfig_Section(xd, Section.Formats, CoreFormatItems, bAdded)

                PrepareConfig_Section(xd, Section.Queryies, AdditionalSectionItems, bAdded)
                PrepareConfig_Section(xd, Section.Queryies, CoreQueryItems, bAdded)

                PrepareConfig_Section(xd, Section.Tokens, AdditionalSectionItems, bAdded)
                PrepareConfig_Section(xd, Section.Tokens, CoreTokenItems, bAdded)

                PrepareConfig_Section(xd, Section.General, AdditionalSectionItems, bAdded)
                PrepareConfig_Section(xd, Section.General, CoreGeneralItems, bAdded)

                If bAdded Then
                    xd.Save(fio.FullName)
                End If

                Try
                    _config = New Hashtable
                    Dim nav As System.Xml.XPath.XPathNavigator = xd.CreateNavigator
                    nav.MoveToRoot()
                    nav.MoveToFirstChild()
                    If nav.Name.ToLower = "openwebstudio" Then
                        _config.Add(getSection(Section.Actions), LoadGroup(nav.CreateNavigator, Section.Actions))
                        _config.Add(getSection(Section.Formats), LoadGroup(nav.CreateNavigator, Section.Formats))
                        _config.Add(getSection(Section.Queryies), LoadGroup(nav.CreateNavigator, Section.Queryies))
                        _config.Add(getSection(Section.Tokens), LoadGroup(nav.CreateNavigator, Section.Tokens))
                        _config.Add(getSection(Section.Wrapper), LoadGroup(nav.CreateNavigator, Section.Wrapper))
                        _config.Add(getSection(Section.General), LoadGroup(nav.CreateNavigator, Section.General))
                    End If
                Catch ex As Exception
                Finally

                End Try
                If _config.Keys.Count <= 1 Then
                    _config = Nothing
                End If

            End If
        End Sub
        Private Shared Function LoadConfig(ByVal xml As String) As Boolean
            Dim breturn As Boolean = False
            If Not _config Is Nothing Then
                Dim xd As New XmlDocument()
                Try
                    xd.LoadXml(xml)
                Catch exDNE As Xml.XmlException
                End Try
                Dim ndRoot As XmlElement = xd.SelectSingleNode(".//openwebstudio")

                If Not ndRoot Is Nothing Then
                    Try
                        Dim nav As System.Xml.XPath.XPathNavigator = xd.CreateNavigator
                        nav.MoveToRoot()
                        nav.MoveToFirstChild()
                        If nav.Name.ToLower = "openwebstudio" Then
                            MergeGroup(getSection(Section.Actions), LoadGroup(nav.CreateNavigator, Section.Actions))
                            MergeGroup(getSection(Section.Formats), LoadGroup(nav.CreateNavigator, Section.Formats))
                            MergeGroup(getSection(Section.Queryies), LoadGroup(nav.CreateNavigator, Section.Queryies))
                            MergeGroup(getSection(Section.Tokens), LoadGroup(nav.CreateNavigator, Section.Tokens))
                            MergeGroup(getSection(Section.Wrapper), LoadGroup(nav.CreateNavigator, Section.Wrapper))
                            MergeGroup(getSection(Section.General), LoadGroup(nav.CreateNavigator, Section.General))
                        End If
                        breturn = True
                    Catch ex As Exception
                    End Try
                End If
            End If
            Return breturn
        End Function
        Private Shared Sub MergeGroup(ByVal Section As String, ByVal Contents As Hashtable)
            If Not _config.ContainsKey(Section) Then
                _config.Add(Section, Contents)
            Else
                Dim merge As Hashtable = _config.Item(Section)
                If Not merge Is Nothing Then
                    Dim itemkey As Object
                    For Each itemkey In Contents.Keys
                        If Not merge.ContainsKey(itemkey) Then
                            merge.Add(itemkey, Contents.Item(itemkey))
                        Else
                            Dim mlst As List(Of SectionItem) = merge.Item(itemkey)
                            Dim nlst As List(Of SectionItem) = Contents.Item(itemkey)
                            Dim item As SectionItem
                            For Each item In nlst
                                If IndexOf(mlst, item.Name, item.Section) = -1 Then
                                    mlst.Add(item)
                                End If
                            Next
                            merge.Item(itemkey) = mlst
                        End If
                    Next
                End If
                _config.Item(Section) = merge
            End If
        End Sub
        Private Shared Function IndexOf(ByRef Items As List(Of SectionItem), ByVal Name As String, ByVal Type As Section) As Integer
            Dim i As Integer = -1
            If Not Items Is Nothing AndAlso Items.Count > 0 Then
                For i = 0 To Items.Count - 1
                    If Items(i).SectionType = Type AndAlso Items(i).Name = Name Then
                        Return i
                    End If
                Next
            End If
            Return -1
        End Function
        Private Shared Function LoadGroup(ByRef nv As Xml.XPath.XPathNavigator, ByVal type As Section) As Hashtable
            Dim hsh As New Hashtable

            Dim arrRun As New List(Of SectionItem)
            Dim xpni As System.Xml.XPath.XPathNodeIterator = nv.Select("/openwebstudio/" & getSection(type) & "/" & getSectionType(SectionType.Runtime))
            While xpni.MoveNext
                arrRun.Add(New SectionItem(xpni.Current.GetAttribute("name", ""), xpni.Current.GetAttribute("type", ""), type, SectionType.Runtime))
            End While
            hsh.Add(getSectionType(SectionType.Runtime), arrRun)

            Dim arrAdmin As New List(Of SectionItem)
            xpni = nv.Select("/openwebstudio/" & getSection(type) & "/" & getSectionType(SectionType.Administration))
            While xpni.MoveNext
                arrAdmin.Add(New SectionItem(xpni.Current.GetAttribute("name", ""), xpni.Current.GetAttribute("type", ""), type, SectionType.Administration))
            End While
            hsh.Add(getSectionType(SectionType.Administration), arrAdmin)

            Dim arrUI As New List(Of SectionItem)
            xpni = nv.Select("/openwebstudio/" & getSection(type) & "/" & getSectionType(SectionType.UI))
            While xpni.MoveNext
                If Not xpni.Current.Value Is Nothing AndAlso Not xpni.Current.Value.Length = 0 Then
                    arrUI.Add(New SectionItem(xpni.Current.GetAttribute("name", ""), xpni.Current.Value, type, SectionType.UI, xpni.Current.GetAttribute("title", ""), xpni.Current.GetAttribute("required", ""), xpni.Current.GetAttribute("mime", ""), xpni.Current.GetAttribute("path", "")))
                Else
                    arrUI.Add(New SectionItem(xpni.Current.GetAttribute("name", ""), xpni.Current.GetAttribute("type", ""), type, SectionType.UI, xpni.Current.GetAttribute("title", ""), xpni.Current.GetAttribute("required", ""), xpni.Current.GetAttribute("mime", ""), xpni.Current.GetAttribute("path", "")))
                End If
            End While
            hsh.Add(getSectionType(SectionType.UI), arrUI)

            Return hsh
        End Function

        Private Shared Function CoreActionItems() As List(Of SectionItem)
            Dim l As New List(Of SectionItem)

            l.Add(New SectionItem("assignment", "r2i.OWS.Actions.AssignmentAction, r2i.OWS.Engine", Section.Actions, SectionType.Runtime))
            l.Add(New SectionItem("comment", "r2i.OWS.Actions.CommentAction, r2i.OWS.Engine", Section.Actions, SectionType.Runtime))
            l.Add(New SectionItem("condition.else", "r2i.OWS.Actions.ConditionElseAction, r2i.OWS.Engine", Section.Actions, SectionType.Runtime))
            l.Add(New SectionItem("condition.if.else", "r2i.OWS.Actions.ConditionIfElseAction, r2i.OWS.Engine", Section.Actions, SectionType.Runtime))
            l.Add(New SectionItem("condition.if", "r2i.OWS.Actions.ConditionIfAction, r2i.OWS.Engine", Section.Actions, SectionType.Runtime))
            l.Add(New SectionItem("delay", "r2i.OWS.Actions.DelayAction, r2i.OWS.Engine", Section.Actions, SectionType.Runtime))
            l.Add(New SectionItem("email", "r2i.OWS.Actions.EmailAction, r2i.OWS.Engine", Section.Actions, SectionType.Runtime))
            l.Add(New SectionItem("query", "r2i.OWS.Actions.ExecuteAction, r2i.OWS.Engine", Section.Actions, SectionType.Runtime))
            l.Add(New SectionItem("file", "r2i.OWS.Actions.FileAction, r2i.OWS.Engine", Section.Actions, SectionType.Runtime))
            l.Add(New SectionItem("filter", "r2i.OWS.Actions.FilterAction, r2i.OWS.Engine", Section.Actions, SectionType.Runtime))
            l.Add(New SectionItem("goto", "r2i.OWS.Actions.GotoAction, r2i.OWS.Engine", Section.Actions, SectionType.Runtime))
            l.Add(New SectionItem("input", "r2i.OWS.Actions.InputAction, r2i.OWS.Engine", Section.Actions, SectionType.Runtime))
            l.Add(New SectionItem("log", "r2i.OWS.Actions.LogAction, r2i.OWS.Engine", Section.Actions, SectionType.Runtime))
            l.Add(New SectionItem("loop", "r2i.OWS.Actions.LoopAction, r2i.OWS.Engine", Section.Actions, SectionType.Runtime))
            l.Add(New SectionItem("message", "r2i.OWS.Actions.ConditionMessageAction, r2i.OWS.Engine", Section.Actions, SectionType.Runtime))
            l.Add(New SectionItem("output", "r2i.OWS.Actions.OutputAction, r2i.OWS.Engine", Section.Actions, SectionType.Runtime))
            l.Add(New SectionItem("redirect", "r2i.OWS.Actions.RedirectAction, r2i.OWS.Engine", Section.Actions, SectionType.Runtime))
            l.Add(New SectionItem("region", "r2i.OWS.Actions.RegionAction, r2i.OWS.Engine", Section.Actions, SectionType.Runtime))
            l.Add(New SectionItem("search", "r2i.OWS.Actions.SearchAction, r2i.OWS.Engine", Section.Actions, SectionType.Runtime))
            l.Add(New SectionItem("template.variable", "r2i.OWS.Actions.TemplateVariableAction, r2i.OWS.Engine", Section.Actions, SectionType.Runtime))
            l.Add(New SectionItem("template", "r2i.OWS.Actions.TemplateAction, r2i.OWS.Engine", Section.Actions, SectionType.Runtime))
            l.Add(New SectionItem("tools.repository", "Scripts/Tools.Repository.js", Section.Actions, SectionType.Administration))
            l.Add(New SectionItem("tools.events", "Scripts/Tools.Debug.js", Section.Actions, SectionType.Administration))

            l.Add(New SectionItem("admin.actions", "Scripts/admin_Actions.js", Section.Actions, SectionType.Administration))
            l.Add(New SectionItem("admin.config", "Scripts/config.js", Section.Actions, SectionType.Administration))
            l.Add(New SectionItem("ows.render", "Scripts/admin_onRender.js", Section.Actions, SectionType.Administration))
            l.Add(New SectionItem("admin.quickbuilder", "Scripts/admin_QuickBuilder.js", Section.Actions, SectionType.Administration))

            Return l
        End Function
        Private Shared Function CoreQueryItems() As List(Of SectionItem)
            Dim l As New List(Of SectionItem)

            l.Add(New SectionItem("database", "r2i.OWS.Queries.Database, r2i.OWS.Engine", Section.Queryies, SectionType.Runtime))
            l.Add(New SectionItem("directory", "r2i.OWS.Queries.Directory, r2i.OWS.Engine", Section.Queryies, SectionType.Runtime))
            l.Add(New SectionItem("json", "r2i.OWS.Queries.JSON, r2i.OWS.Engine", Section.Queryies, SectionType.Runtime))
            l.Add(New SectionItem("xml", "r2i.OWS.Queries.XML, r2i.OWS.Engine", Section.Queryies, SectionType.Runtime))
            'l.Add(New SectionItem("r2i.OWS.Engine", "Scripts/r2i.OWS.Queries.js", Section.Queryies, SectionType.Administration))
            Return l
        End Function
        Private Shared Function CoreTokenItems() As List(Of SectionItem)
            Dim l As New List(Of SectionItem)

            'l.Add(New SectionItem("r2i.OWS.Engine", "r2i.OWS.Renderers.js", Section.Tokens, SectionType.Administration))
            l.Add(New SectionItem("action", "r2i.OWS.Renderers.RenderAction, r2i.OWS.Engine", Section.Tokens, SectionType.Runtime))
            l.Add(New SectionItem("actions", "r2i.OWS.Renderers.RenderActions, r2i.OWS.Engine", Section.Tokens, SectionType.Runtime))
            l.Add(New SectionItem("alternate", "r2i.OWS.Renderers.RenderAlternate, r2i.OWS.Engine", Section.Tokens, SectionType.Runtime))
            l.Add(New SectionItem("checkitem", "r2i.OWS.Renderers.RenderCheckItem, r2i.OWS.Engine", Section.Tokens, SectionType.Runtime))
            l.Add(New SectionItem("checklist", "r2i.OWS.Renderers.RenderCheckList, r2i.OWS.Engine", Section.Tokens, SectionType.Runtime))
            l.Add(New SectionItem("checklistitem", "r2i.OWS.Renderers.RenderCheckListItem, r2i.OWS.Engine", Section.Tokens, SectionType.Runtime))
            l.Add(New SectionItem("coalesce", "r2i.OWS.Renderers.RenderCoalesce, r2i.OWS.Engine", Section.Tokens, SectionType.Runtime))
            l.Add(New SectionItem("columns", "r2i.OWS.Renderers.RenderColumns, r2i.OWS.Engine", Section.Tokens, SectionType.Runtime))
            l.Add(New SectionItem("count", "r2i.OWS.Renderers.RenderCount, r2i.OWS.Engine", Section.Tokens, SectionType.Runtime))
            l.Add(New SectionItem("filter", "r2i.OWS.Renderers.RenderFilter, r2i.OWS.Engine", Section.Tokens, SectionType.Runtime))
            l.Add(New SectionItem("filtertag", "r2i.OWS.Renderers.RenderFilterTag, r2i.OWS.Engine", Section.Tokens, SectionType.Runtime))
            l.Add(New SectionItem("format", "r2i.OWS.Renderers.RenderFormat, r2i.OWS.Engine", Section.Tokens, SectionType.Runtime))
            l.Add(New SectionItem("iif", "r2i.OWS.Renderers.RenderIIF, r2i.OWS.Engine", Section.Tokens, SectionType.Runtime))
            l.Add(New SectionItem("locale", "r2i.OWS.Renderers.RenderLocale, r2i.OWS.Engine", Section.Tokens, SectionType.Runtime))
            l.Add(New SectionItem("math", "r2i.OWS.Renderers.RenderMath, r2i.OWS.Engine", Section.Tokens, SectionType.Runtime))
            l.Add(New SectionItem("radio", "r2i.OWS.Renderers.RenderRadio, r2i.OWS.Engine", Section.Tokens, SectionType.Runtime))
            l.Add(New SectionItem("select", "r2i.OWS.Renderers.RenderSort, r2i.OWS.Engine", Section.Tokens, SectionType.Runtime))
            l.Add(New SectionItem("set", "r2i.OWS.Renderers.RenderSet, r2i.OWS.Engine", Section.Tokens, SectionType.Runtime))
            l.Add(New SectionItem("sort", "r2i.OWS.Renderers.RenderSort, r2i.OWS.Engine", Section.Tokens, SectionType.Runtime))
            l.Add(New SectionItem("sortheader", "r2i.OWS.Renderers.RenderSortHeader, r2i.OWS.Engine", Section.Tokens, SectionType.Runtime))
            l.Add(New SectionItem("sorttag", "r2i.OWS.Renderers.RenderSortTag, r2i.OWS.Engine", Section.Tokens, SectionType.Runtime))
            l.Add(New SectionItem("subquery", "r2i.OWS.Renderers.RenderSubQuery, r2i.OWS.Engine", Section.Tokens, SectionType.Runtime))
            l.Add(New SectionItem("sum", "r2i.OWS.Renderers.RenderSum, r2i.OWS.Engine", Section.Tokens, SectionType.Runtime))
            l.Add(New SectionItem("texteditor", "r2i.OWS.Renderers.RenderTextEditor, r2i.OWS.Engine", Section.Tokens, SectionType.Runtime))
            l.Add(New SectionItem("variable", "r2i.OWS.Renderers.RenderVariable, r2i.OWS.Engine", Section.Tokens, SectionType.Runtime))
            Return l
        End Function
        Private Shared Function CoreGeneralItems() As List(Of SectionItem)
            Dim l As New List(Of SectionItem)
            l.Add(New SectionItem("jquery", "Scripts/jquery.js", Section.General, SectionType.Administration))
            l.Add(New SectionItem("jquery.noconflict", "Scripts/jquery.noconflict.js", Section.General, SectionType.Administration))
            l.Add(New SectionItem("jribbon", "Scripts/jRibbon.js", Section.General, SectionType.Administration))
            l.Add(New SectionItem("ows", "Scripts/OWS.js", Section.General, SectionType.Administration))
            l.Add(New SectionItem("ows.debug", "Scripts/OWS.debug.js", Section.General, SectionType.Administration))
            l.Add(New SectionItem("json", "Scripts/json.js", Section.General, SectionType.Administration))
            l.Add(New SectionItem("admin.core", "Scripts/admin_core.js", Section.General, SectionType.Administration))
            l.Add(New SectionItem("admin.load", "Scripts/admin_onload.js", Section.General, SectionType.Administration))
            l.Add(New SectionItem("ows.utilities", "Scripts/ows.utilities.js", Section.General, SectionType.Administration))
            l.Add(New SectionItem("admin.ribbon", "Scripts/admin_ribbon.js", Section.General, SectionType.Administration))
            l.Add(New SectionItem("ows.community", "Scripts/OWS.Community.js", Section.General, SectionType.Administration))
            l.Add(New SectionItem("admin.selector", "Scripts/admin_selector.js", Section.General, SectionType.Administration))

            l.Add(New SectionItem("ows.general", "Scripts/OWS.js", Section.General, SectionType.UI, "OWS Core Runtime", "true"))
            l.Add(New SectionItem("ows.utilities", "Scripts/OWS.Utilities.js", Section.General, SectionType.UI, "OWS Utility Library"))
            l.Add(New SectionItem("ows.validation", "Scripts/OWS.Validation.js", Section.General, SectionType.UI, "OWS Validation Library"))
            l.Add(New SectionItem("jquery", "Scripts/jquery.js", Section.General, SectionType.UI, "JQuery"))
            l.Add(New SectionItem("jquery.noconflict", "Scripts/jquery.noconflict.js", Section.General, SectionType.UI, "JQuery NoConflict Override ($jq)"))
            l.Add(New SectionItem("jquery.noconflict", "Scripts/jquery.thickbox.js", Section.General, SectionType.UI, "JQuery Thickbox"))
            l.Add(New SectionItem("jquery.noconflict.css", "Scripts/jquery.thickbox.css", Section.General, SectionType.UI, "JQuery Thickbox Stylesheet", "false", "text/css", Nothing))
            Return l
        End Function
        Private Shared Function CoreFormatItems() As List(Of SectionItem)
            Dim l As New List(Of SectionItem)

            'l.Add(New SectionItem("r2i.OWS.Engine", "r2i.OWS.Formatters.js", Section.Formats, SectionType.Administration))
            l.Add(New SectionItem("0", "r2i.OWS.Formatters.Default, r2i.OWS.Engine", Section.Formats, SectionType.Runtime))
            l.Add(New SectionItem("breakword", "r2i.OWS.Formatters.BreakWord, r2i.OWS.Engine", Section.Formats, SectionType.Runtime))
            l.Add(New SectionItem("canedit", "r2i.OWS.Formatters.CanEdit, r2i.OWS.Engine", Section.Formats, SectionType.Runtime))
            l.Add(New SectionItem("contains", "r2i.OWS.Formatters.Contains, r2i.OWS.Engine", Section.Formats, SectionType.Runtime))
            l.Add(New SectionItem("decodehtml", "r2i.OWS.Formatters.DecodeHtml, r2i.OWS.Engine", Section.Formats, SectionType.Runtime))
            l.Add(New SectionItem("decodeuri", "r2i.OWS.Formatters.DecodeUri, r2i.OWS.Engine", Section.Formats, SectionType.Runtime))
            l.Add(New SectionItem("decrypt", "r2i.OWS.Formatters.Decrypt, r2i.OWS.Engine", Section.Formats, SectionType.Runtime))
            l.Add(New SectionItem("diff", "r2i.OWS.Formatters.Diff, r2i.OWS.Engine", Section.Formats, SectionType.Runtime))
            l.Add(New SectionItem("encodehtml", "r2i.OWS.Formatters.EncodeHtml, r2i.OWS.Engine", Section.Formats, SectionType.Runtime))
            l.Add(New SectionItem("encodeuri", "r2i.OWS.Formatters.EncodeUri, r2i.OWS.Engine", Section.Formats, SectionType.Runtime))
            l.Add(New SectionItem("encodewiki", "r2i.OWS.Formatters.EncodeWiki, r2i.OWS.Engine", Section.Formats, SectionType.Runtime))
            l.Add(New SectionItem("encrypt", "r2i.OWS.Formatters.Encrypt, r2i.OWS.Engine", Section.Formats, SectionType.Runtime))
            l.Add(New SectionItem("endswith", "r2i.OWS.Formatters.EndsWith, r2i.OWS.Engine", Section.Formats, SectionType.Runtime))
            l.Add(New SectionItem("escape", "r2i.OWS.Formatters.Escape, r2i.OWS.Engine", Section.Formats, SectionType.Runtime))
            l.Add(New SectionItem("exists", "r2i.OWS.Formatters.Exists, r2i.OWS.Engine", Section.Formats, SectionType.Runtime))
            l.Add(New SectionItem("file", "r2i.OWS.Formatters.File, r2i.OWS.Engine", Section.Formats, SectionType.Runtime))
            l.Add(New SectionItem("firewall", "r2i.OWS.Formatters.Firewall, r2i.OWS.Engine", Section.Formats, SectionType.Runtime))
            l.Add(New SectionItem("friendlyurl", "r2i.OWS.Formatters.FriendlyUrl, r2i.OWS.Engine", Section.Formats, SectionType.Runtime))
            l.Add(New SectionItem("indexof", "r2i.OWS.Formatters.IndexOf, r2i.OWS.Engine", Section.Formats, SectionType.Runtime))
            l.Add(New SectionItem("isdate", "r2i.OWS.Formatters.IsDate, r2i.OWS.Engine", Section.Formats, SectionType.Runtime))
            l.Add(New SectionItem("isempty", "r2i.OWS.Formatters.IsEmpty, r2i.OWS.Engine", Section.Formats, SectionType.Runtime))
            l.Add(New SectionItem("isinrole", "r2i.OWS.Formatters.IsInRole, r2i.OWS.Engine", Section.Formats, SectionType.Runtime))
            l.Add(New SectionItem("isnumeric", "r2i.OWS.Formatters.IsNumeric, r2i.OWS.Engine", Section.Formats, SectionType.Runtime))
            l.Add(New SectionItem("issuperuser", "r2i.OWS.Formatters.IsSuperUser, r2i.OWS.Engine", Section.Formats, SectionType.Runtime))
            l.Add(New SectionItem("lastindexof", "r2i.OWS.Formatters.LastIndexOf, r2i.OWS.Engine", Section.Formats, SectionType.Runtime))
            l.Add(New SectionItem("left", "r2i.OWS.Formatters.Left, r2i.OWS.Engine", Section.Formats, SectionType.Runtime))
            l.Add(New SectionItem("length", "r2i.OWS.Formatters.Length, r2i.OWS.Engine", Section.Formats, SectionType.Runtime))
            l.Add(New SectionItem("list", "r2i.OWS.Formatters.List, r2i.OWS.Engine", Section.Formats, SectionType.Runtime))
            l.Add(New SectionItem("lower", "r2i.OWS.Formatters.Lower, r2i.OWS.Engine", Section.Formats, SectionType.Runtime))
            l.Add(New SectionItem("mapparenturl", "r2i.OWS.Formatters.MapParentUrl, r2i.OWS.Engine", Section.Formats, SectionType.Runtime))
            l.Add(New SectionItem("mappath", "r2i.OWS.Formatters.MapPath, r2i.OWS.Engine", Section.Formats, SectionType.Runtime))
            l.Add(New SectionItem("mapurl", "r2i.OWS.Formatters.MapUrl, r2i.OWS.Engine", Section.Formats, SectionType.Runtime))
            l.Add(New SectionItem("md5hash", "r2i.OWS.Formatters.MD5Hash, r2i.OWS.Engine", Section.Formats, SectionType.Runtime))
            l.Add(New SectionItem("mid", "r2i.OWS.Formatters.Mid, r2i.OWS.Engine", Section.Formats, SectionType.Runtime))
            l.Add(New SectionItem("pad", "r2i.OWS.Formatters.Pad, r2i.OWS.Engine", Section.Formats, SectionType.Runtime))
            l.Add(New SectionItem("parentpath", "r2i.OWS.Formatters.ParentPath, r2i.OWS.Engine", Section.Formats, SectionType.Runtime))
            l.Add(New SectionItem("replace", "r2i.OWS.Formatters.Replace, r2i.OWS.Engine", Section.Formats, SectionType.Runtime))
            l.Add(New SectionItem("reverseparentpath", "r2i.OWS.Formatters.ReverseParentPath, r2i.OWS.Engine", Section.Formats, SectionType.Runtime))
            l.Add(New SectionItem("reversepath", "r2i.OWS.Formatters.ReversePath, r2i.OWS.Engine", Section.Formats, SectionType.Runtime))
            l.Add(New SectionItem("right", "r2i.OWS.Formatters.Right, r2i.OWS.Engine", Section.Formats, SectionType.Runtime))
            l.Add(New SectionItem("sqlfind", "r2i.OWS.Formatters.SqlFind, r2i.OWS.Engine", Section.Formats, SectionType.Runtime))
            l.Add(New SectionItem("startswith", "r2i.OWS.Formatters.StartsWith, r2i.OWS.Engine", Section.Formats, SectionType.Runtime))
            l.Add(New SectionItem("striphtml", "r2i.OWS.Formatters.StripHtml, r2i.OWS.Engine", Section.Formats, SectionType.Runtime))
            l.Add(New SectionItem("tab", "r2i.OWS.Formatters.Tab, r2i.OWS.Engine", Section.Formats, SectionType.Runtime))
            l.Add(New SectionItem("trim", "r2i.OWS.Formatters.Trim, r2i.OWS.Engine", Section.Formats, SectionType.Runtime))
            l.Add(New SectionItem("unescape", "r2i.OWS.Formatters.Unescape, r2i.OWS.Engine", Section.Formats, SectionType.Runtime))
            l.Add(New SectionItem("upper", "r2i.OWS.Formatters.Upper, r2i.OWS.Engine", Section.Formats, SectionType.Runtime))
            l.Add(New SectionItem("url", "r2i.OWS.Formatters.Url, r2i.OWS.Engine", Section.Formats, SectionType.Runtime))
            Return l
        End Function
    End Class
End Namespace