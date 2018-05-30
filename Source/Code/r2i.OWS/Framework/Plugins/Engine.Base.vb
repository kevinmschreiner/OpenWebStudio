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
Imports System.Xml.Serialization
Imports System.Collections.Generic
Imports System.Net.Mail
Imports System.Text
Imports r2i.OWS.Framework
Imports r2i.OWS.Framework.Entities
Imports r2i.OWS.Framework.DataAccess
Imports r2i.OWS.Framework.Plugins
Imports r2i.OWS.Framework.Plugins.Renderers
Imports r2i.OWS.Framework.Utilities.Compatibility
Imports r2i.OWS.Framework.Utilities
Imports System.Reflection
Imports System.Web
Imports System.Web.UI
Imports r2i.OWS.Newtonsoft.Json

Namespace r2i.OWS.Framework
    Public MustInherit Class EngineBase
        Implements IDisposable
        Public Request As HttpRequest
        Public Response As HttpResponse
        Public Context As HttpContext
        Public ViewState As StateBag

        'Public JSONState As Dictionary(Of String, String)
        Public PortalSettings As IPortalSettings
        Public ModuleID As String
        Public TemplateCacheName As String
        Public TemplateCacheShare As String
        Public TemplateCacheTime As String
        Public ModuleIsEditable As Boolean
        Public TabModuleID As String
        Public TabID As String
        Public ClientID As String
        Public UserInfo As IUser
        Public UserID As String = ""
        Public PortalID As String
        Public Settings As Hashtable
        Public Caller As Framework.UI.Control
        Public TableVariables As DataSet
        Public EventReferenceControl As System.Web.UI.Control
        Public ConfigurationID As Guid
        Public xls As Settings

        Public ReadOnly Property Culture() As System.Globalization.CultureInfo
            Get
                Return System.Threading.Thread.CurrentThread.CurrentUICulture
            End Get
        End Property
        Public Session As GenericSession
        Public ModulePath As String
        Public CapturedMessages As SortedList(Of String, String)
        Public mDebugWriter As Debugger

        Public Enum RequestTypeEnum
            Page
            Postback
            Ajax
            Search
            Import
            Export
        End Enum

        '<Serializable()> _
        'Public Structure JSStateNameValuePair
        '    Public name As String
        '    Public value As String

        '    Public Function Serialize() As String
        '        Try
        '            Serialize = JavaScriptConvert.SerializeObject(DirectCast(Me, Object))
        '        Catch ex As Exception
        '            Serialize = Nothing
        '        End Try
        '    End Function

        '    Public Shared Function Deserialize(ByVal str As String) As JSStateNameValuePair
        '        Dim nvp As JSStateNameValuePair = Nothing
        '        Try
        '            If Not str Is Nothing AndAlso str.Length > 0 Then
        '                Dim xls As New JSStateNameValuePair
        '                Dim jso As JavaScriptObject
        '                Try
        '                    jso = JavaScriptConvert.DeserializeObject(str)
        '                    nvp = New JSStateNameValuePair
        '                    nvp.name = jso.Item("name").ToString
        '                    nvp.value = jso.Item("value").ToString
        '                Catch ex As Exception
        '                    Return Nothing
        '                    'TODO: IMplements Exception
        '                End Try

        '                Return xls
        '            Else
        '                'TODO: Implements exception json unvalid 
        '            End If
        '        Catch ex As Exception

        '        End Try
        '        Return nvp
        '    End Function
        'End Structure

        'Public Sub State_Load(ByVal name As String)
        '    'Name is "__OWS_PAGESTATE__"
        '    If Not Me.Request Is Nothing AndAlso Not Me.Request.Form Is Nothing AndAlso Not Me.Request.Form.Item(name) Is Nothing Then
        '        Try
        '            Dim xls As New JSStateNameValuePair
        '            Dim jso As JavaScriptObject
        '            Me.JSONState = New Dictionary(Of String, String)
        '            Try

        '                jso = JavaScriptConvert.DeserializeObject(Me.Request.Form.Item(name))

        '            Catch ex As Exception
        '                'TODO: IMplements Exception
        '            End Try
        '        Catch ex As Exception

        '        End Try
        '    End If
        'End Sub

        Public MustOverride Property RequestType() As RequestTypeEnum

        'Extended to support iPortable
        'Public MustOverride Property isAjaxRequest() As Boolean
        'Public MustOverride Property isSearchRequest() As Boolean
        'Public MustOverride Property isImportRequest() As Boolean
        'Public MustOverride Property isExportRequest() As Boolean

        Public MustOverride WriteOnly Property AbortRendering() As Boolean
        Public MustOverride Property EndResponse() As Boolean

        Public MustOverride Property CachedTableRowIndexCollection(ByVal TableName As String) As Integer
        Public MustOverride Property TotalRecords() As Integer
        Public MustOverride WriteOnly Property CurrentPage() As Integer
        Public MustOverride Property PageCurrent() As Integer
        Public MustOverride Property TotalPages() As Integer
        Public MustOverride Property RecordsPerPage() As Integer
        Public MustOverride Property OverridePaging() As Boolean
        Public MustOverride Property CurrentUser() As IUser


#Region "Internal - Single Usage Variables"
        Public MustOverride Function ActionVariableSearch(ByVal Query As String) As String()
        Public MustOverride Property ActionVariable(ByVal Name As String) As Object
        Public MustOverride ReadOnly Property ActionVariables() As SortedList(Of String, Object)
        Private Shared _SharedCache As New Dictionary(Of String, Object)
        Private Shared _SharedCache_event As New System.Threading.Mutex
        Private Enum SharedCacheAction
            Read
            Write
            Contains
        End Enum
        Private Shared Function SharedCache_SetGet(ByVal Name As String, ByVal Value As Object, ByVal Action As SharedCacheAction) As Object
            Dim o As Object = Nothing
            _SharedCache_event.WaitOne()
            Try
                If (Action = SharedCacheAction.Read) Then
                    If (_SharedCache.ContainsKey(Name)) Then
                        o = _SharedCache.Item(Name)
                    End If
                ElseIf (Action = SharedCacheAction.Write) Then
                    If (Value Is Nothing) Then
                        If (_SharedCache.ContainsKey(Name)) Then
                            _SharedCache.Remove(Name)
                        End If
                    Else
                        If (_SharedCache.ContainsKey(Name)) Then
                            _SharedCache.Item(Name) = Value
                        Else
                            _SharedCache.Add(Name, Value)
                        End If
                    End If
                Else
                    If (_SharedCache.ContainsKey(Name)) Then
                        o = True
                    Else
                        o = False
                    End If
                End If
            Catch ex As Exception
            Finally
                _SharedCache_event.ReleaseMutex()
            End Try
            Return o
        End Function
        Public Shared Function SharedCache_Contains(ByVal Name As String) As Boolean
            Dim result As Boolean = SharedCache_SetGet(Name, Nothing, SharedCacheAction.Contains)
            Return result
        End Function
        Public Shared Sub SharedCache_Remove(ByVal Name As String)
            SharedCache_SetGet(Name, Nothing, SharedCacheAction.Write)
        End Sub
        Public Shared Sub SharedCache_Set(ByVal Name As String, ByVal Value As Object)
            SharedCache_SetGet(Name, Value, SharedCacheAction.Write)
        End Sub
        Public Shared Function SharedCache_Get(ByVal Name As String) As Object
            Return SharedCache_SetGet(Name, Nothing, SharedCacheAction.Read)
        End Function
#End Region
#Region "Protected Members"
        '        Protected excelData As Text.StringBuilder
#End Region
        Public MustOverride Function GetPostBackEventReference(ByVal Argument As String) As String

 
#Region "Runtime Rendering Methods"
        Public MustOverride Sub ExportData(ByRef DS As DataSet, ByVal QueryLength As Integer, ByVal PageSize As Integer, ByVal CurrentPage As Integer, ByVal CustomPaging As Boolean, ByVal Output As IO.TextWriter, ByRef RuntimeMessages As SortedList(Of String, String), Optional ByVal forceShowAll As Boolean = False, Optional ByVal Delimiter As String = ",")
        Public MustOverride Sub Initialize()
        Public MustOverride Sub Render(ByRef DS As DataSet, ByVal QueryLength As Integer, ByVal PageSize As Integer, ByVal CurrentPage As Integer, ByVal CustomPaging As Boolean, ByVal Output As IO.TextWriter, ByRef RuntimeMessages As SortedList(Of String, String), Optional ByVal forceShowAll As Boolean = False, Optional ByVal DebugWriter As Debugger = Nothing)
        Public MustOverride Property Cache(ByVal Key As String, ByVal useCache As Boolean, Optional ByVal CacheTime As Integer = 1200) As Object


#Region "String Rendering"
        Public MustOverride Function RenderString(ByRef DS As DataSet, ByVal Source As String, ByRef Messages As SortedList(Of String, String), ByVal useAggregations As Boolean, ByVal isPreRender As Boolean, Optional ByVal NullReturn As Boolean = True, Optional ByVal ProtectSession As Boolean = False, Optional ByVal SessionDelimiter As String = ",", Optional ByVal useSessionQuotes As Boolean = True, Optional ByRef FilterText As String = Nothing, Optional ByRef FilterField As String = Nothing, Optional ByRef DebugWriter As r2i.OWS.Framework.Debugger = Nothing, Optional ByVal Row As DataRow = Nothing) As String

        Public MustOverride Function RenderString(ByVal Index As Integer, ByRef Source As String, ByRef StartValues As Char(), ByRef EndValues As Char(), ByRef EscapeChar As Char, ByRef DS As DataSet, ByRef DR As DataRow, ByRef RuntimeMessages As SortedList(Of String, String), ByVal useAggregations As Boolean, ByVal isPreRender As Boolean, Optional ByVal NullReturn As Boolean = False, Optional ByVal ProtectSession As Boolean = False, Optional ByVal SessionDelimiter As String = ",", Optional ByVal useSessionQuotes As Boolean = True, Optional ByRef FilterText As String = Nothing, Optional ByRef FilterField As String = Nothing, Optional ByRef DebugWriter As Debugger = Nothing) As Boolean
        Protected MustOverride Function RenderString(ByVal Index As Integer, ByRef Source As String, ByVal Position As RenderBase.Positional, ByRef DS As DataSet, ByRef DR As DataRow, ByRef RuntimeMessages As SortedList(Of String, String), ByVal useAggregations As Boolean, ByVal isPreRender As Boolean, Optional ByVal NullReturn As Boolean = False, Optional ByVal ProtectSession As Boolean = False, Optional ByVal SessionDelimiter As String = ",", Optional ByVal useSessionQuotes As Boolean = True, Optional ByRef FilterText As String = Nothing, Optional ByRef FilterField As String = Nothing, Optional ByRef DebugWriter As Debugger = Nothing) As Boolean

        Public MustOverride Function RenderString_Assignment_Assign(ByVal Name As String, ByVal Value As String, ByVal Location As String, ByRef Source As String, ByRef bSystemParse As Boolean) As Boolean
#End Region

        Public MustOverride Function ModuleImageURL(ByVal Src As String) As String
        Public MustOverride Function ClearCache() As Boolean
        Public MustOverride Function ClearSiteCache() As Boolean
        Public MustOverride Function ClearPageCache() As Boolean
        Public MustOverride Sub ClearResponse()
        Public MustOverride Sub ClearResponse(ByVal resetTo As Boolean)
        Public MustOverride Sub ClearHeaders()
        Public MustOverride ReadOnly Property EditUserInfo() As IUser
#End Region

#Region "Data Engine"
        Public MustOverride Function RenderQuery(ByRef SharedDS As DataSet, ByVal FilterField As String, ByVal FilterText As String, ByVal RecordsPerPage As Integer, ByVal CapturedMessages As SortedList(Of String, String), ByRef DebugWriter As r2i.OWS.Framework.Debugger, Optional ByVal Query As String = Nothing) As String
        Public MustOverride Function GetData(ByVal isSubQuery As Boolean, ByVal Query As String, ByVal FilterField As String, ByVal FilterText As String, ByRef DebugWriter As Debugger, ByVal isRendered As Boolean, ByVal CacheName As String, ByVal CacheTime As String, ByVal CacheShare As Boolean, Optional ByVal timeout As Integer = -1, Optional ByVal CustomConnection As String = Nothing, Optional ByRef IsSuccessful As Boolean = True, Optional ByRef FailureMessage As String = Nothing) As DataSet
        Public MustOverride Sub renderJsonObject(ByVal o As Object, ByVal ds As DataSet, ByVal columnList As String)
#End Region
#Region "Runtime Debugging"
        Public MustOverride Function ShowRuntimeItems(ByVal header As String, ByVal footer As String) As String
        Public MustOverride Function ShowQuerystring(ByVal header As String, ByVal footer As String) As String
        Public MustOverride Function ShowHeaders(ByVal header As String, ByVal footer As String) As String
        Public MustOverride Function ShowForm(ByVal header As String, ByVal footer As String) As String
        Public MustOverride Function ShowViewState(ByVal header As String, ByVal footer As String) As String
        Public MustOverride Function ShowCookies(ByVal header As String, ByVal footer As String) As String
        Public MustOverride Function ShowFile(ByRef Value As System.Web.HttpPostedFile) As String
        Public MustOverride Function ShowSession(ByVal header As String, ByVal footer As String) As String
#End Region
#Region "Runtime Message Actions"
        Public MustOverride Sub ExecuteActions(ByRef outgoingDS As DataSet, ByVal FilterField As String, ByVal FilterText As String, ByRef debugger As Debugger)
#End Region

        Public Shared Sub Load()
            'Any global initialization can be done here
        End Sub

        'ROMAIN: 08/23/07
        'NOTE: Idisposable Implementation
        Protected Overridable Overloads Sub Dispose(ByVal disposing As Boolean)
            If disposing Then
            End If
            ' free native resources
        End Sub 'Dispose

        Public Overloads Sub Dispose() Implements IDisposable.Dispose
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
    End Class
End Namespace
