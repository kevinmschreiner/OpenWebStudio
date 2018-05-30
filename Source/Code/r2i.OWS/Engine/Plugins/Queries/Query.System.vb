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
Imports r2i.OWS.Framework.Plugins.Queries
Imports r2i.OWS.Framework
Imports r2i.OWS.Framework.Utilities


Namespace r2i.OWS.Queries
    Public Class SystemProperties
        Inherits QueryBase

        Public Overrides ReadOnly Property QueryTag() As String
            Get
                Return "<SYSTEM>"
            End Get
        End Property

        Public Overrides ReadOnly Property QueryStructure() As String
            Get
                Dim s As String = ""

                s &= "<SYSTEM>" & vbCrLf
                s &= "   <PATH></PATH>" & vbCrLf
                s &= "   <QUERY></QUERY>" & vbCrLf
                s &= "   <SOURCE></SOURCE>" & vbCrLf
                s &= "   <PIVOT></PIVOT>" & vbCrLf
                s &= "</SYSTEM>" & vbCrLf

                Return s
            End Get
        End Property

        Public Overrides Function Handle_GetData(ByRef Caller As EngineBase, ByVal isSubQuery As Boolean, ByVal Query As String, ByVal FilterField As String, ByVal FilterText As String, ByRef DebugWriter As Framework.Debugger, ByVal isRendered As Boolean, Optional ByVal timeout As Integer = -1, Optional ByVal CustomConnection As String = Nothing) As Framework.RuntimeBase.QueryResult
            Dim rslt As New Framework.RuntimeBase.QueryResult(RuntimeBase.ExecutableResultEnum.Executed, New DataSet)
            Try
                Dim output As DataTable = GetTableStructure("System")
                Dim strPivot As String = Nothing
                If Not Query Is Nothing AndAlso Query.Length > 0 Then
                    Dim strPath As String
                    Dim strSource As String
                    Dim strQuery As String
                    strPath = Utility.XMLPropertyParse_Quick(Query, "path")
                    strQuery = Utility.XMLPropertyParse_Quick(Query, "query")
                    strSource = Utility.XMLPropertyParse_Quick(Query, "source")
                    strPivot = Utility.XMLPropertyParse_Quick(Query, "pivot")
                    Select Case strPath.ToUpper
                        Case "REFLECTION"
                            GetRows_Reflection(Caller, strQuery, output)
                        Case "CACHE"
                            GetRows_Cache(Caller, strQuery, output)
                        Case "FORM"
                            GetRows_Form(Caller, strQuery, output)
                        Case "FORM.FILES"
                            GetRows_Files(Caller, strQuery, output)
                        Case "QUERYSTRING"
                            If Not strSource Is Nothing AndAlso strSource.Length > 0 Then
                                GetRows_Querystring_Parameters(Caller, strSource, strQuery, output)
                            Else
                                GetRows_Querystring(Caller, strQuery, output)
                            End If
                        Case "SESSION"
                            GetRows_Session(Caller, strQuery, output)
                        Case "COOKIE"
                            GetRows_Cookie(Caller, strQuery, output)
                        Case "USER"
                            GetRows_User(Caller, strQuery, output)
                        Case "PORTAL"
                            GetRows_Portal(Caller, strQuery, output)
                        Case "CONTEXT"
                            GetRows_Context(Caller, strQuery, output)
                        Case "VIEWSTATE"
                            GetRows_ViewState(Caller, strQuery, output)
                        Case "ACTION"
                            GetRows_Actions(Caller, strQuery, output)
                        Case "MESSAGES"
                            GetRows_Messages(Caller, strQuery, output)
                        Case "SETTINGS"
                            GetRows_Settings(Caller, strQuery, output)
                        Case "HEADERS"
                            GetRows_Headers(Caller, strQuery, output)
                    End Select
                End If
                If Not strPivot Is Nothing AndAlso strPivot.Length > 0 AndAlso strPivot.ToUpper.StartsWith("T") Then
                    Dim pivot As DataTable = GetPivot(output, "System")
                    rslt.Value.Tables.Add(pivot)
                Else
                    rslt.Value.Tables.Add(output)
                End If

            Catch ex As Exception
                Framework.Utilities.Utility.SortStatus(Caller.Session, Caller.ConfigurationID.ToString.Replace("{", "").Replace("}", "").Replace("-", ""), Caller.ModuleID, Caller.UserID) = Nothing
                rslt.Result = RuntimeBase.ExecutableResultEnum.Failed
                rslt.Error = ex
            End Try

            Return rslt
        End Function

        Private Sub GetRows_Cache(ByRef Caller As EngineBase, ByVal Query As String, ByRef dt As DataTable)
            Try
                Dim CacheEnum As IDictionaryEnumerator = Caller.Context.Cache.GetEnumerator()
                While CacheEnum.MoveNext()
                    Try
                        Dim dr As DataRow = dt.NewRow
                        dr(0) = (CacheEnum.Current.Key.ToString())
                        dr(1) = (CacheEnum.Current.Value.ToString())
                        dt.Rows.Add(dr)
                    Catch ex As Exception
                    End Try
                End While
                If Not Query Is Nothing AndAlso Query.Length > 0 Then
                    Dim drf() As System.Data.DataRow = dt.Select(Query)
                    Dim dtr As DataTable = dt.Clone()
                    Dim drfr As DataRow
                    For Each drfr In drf
                        Dim drx As System.Data.DataRow = dtr.NewRow()
                        drx(0) = drfr(0)
                        drx(1) = drfr(1)
                        dtr.Rows.Add(drx)
                    Next
                    dt.Clear()
                    dt = dtr
                End If
            Catch ex As Exception
            End Try
        End Sub
        Private Sub GetRows_Reflection(ByRef Caller As EngineBase, ByVal Name As String, ByRef dt As DataTable)
            Try
                Dim dict As System.Collections.Generic.IDictionary(Of String, String) = ReflectionReader.Reflect(Name, Caller)
                If Not dict Is Nothing AndAlso dict.Count > 0 Then
                    Dim str As String
                    For Each str In dict.Keys
                        Dim dr As DataRow = dt.NewRow
                        dr(0) = (str)
                        dr(1) = (dict(str))
                        dt.Rows.Add(dr)
                    Next
                End If
            Catch ex As Exception
            End Try
        End Sub
        Private Sub GetRows_Form(ByRef Caller As EngineBase, ByVal Query As String, ByRef dt As DataTable)
            Try
                Dim keys() As String = Caller.Context.Request.Form.AllKeys
                Dim i As Integer = 0
                For i = 0 To keys.Length - 1
                    Try
                        Dim dr As DataRow = dt.NewRow
                        dr(0) = (keys(i))
                        dr(1) = Caller.Context.Request.Form(keys(i))
                        dt.Rows.Add(dr)
                    Catch ex As Exception
                    End Try
                Next
                If Not Query Is Nothing AndAlso Query.Length > 0 Then
                    Dim drf() As System.Data.DataRow = dt.Select(Query)
                    Dim dtr As DataTable = dt.Clone()
                    Dim drfr As DataRow
                    For Each drfr In drf
                        Dim drx As System.Data.DataRow = dtr.NewRow()
                        drx(0) = drfr(0)
                        drx(1) = drfr(1)
                        dtr.Rows.Add(drx)
                    Next
                    dt.Clear()
                    dt = dtr
                End If
            Catch ex As Exception
            End Try
        End Sub
        Private Sub GetRows_Files(ByRef Caller As EngineBase, ByVal Query As String, ByRef dt As DataTable)
            Try
                Dim keys() As String = Caller.Context.Request.Files.AllKeys
                Dim i As Integer = 0
                For i = 0 To keys.Length - 1
                    Try
                        Dim dr As DataRow = dt.NewRow
                        dr(0) = (keys(i))
                        dr(1) = Caller.Context.Request.Files(keys(i)).FileName
                        dt.Rows.Add(dr)
                    Catch ex As Exception
                    End Try
                Next
                If Not Query Is Nothing AndAlso Query.Length > 0 Then
                    Dim drf() As System.Data.DataRow = dt.Select(Query)
                    Dim dtr As DataTable = dt.Clone()
                    Dim drfr As DataRow
                    For Each drfr In drf
                        Dim drx As System.Data.DataRow = dtr.NewRow()
                        drx(0) = drfr(0)
                        drx(1) = drfr(1)
                        dtr.Rows.Add(drx)
                    Next
                    dt.Clear()
                    dt = dtr
                End If
            Catch ex As Exception
            End Try
        End Sub
        Private Sub GetRows_Querystring_parameters(ByRef Caller As EngineBase, ByVal Source As String, ByVal Query As String, ByRef dt As DataTable)
            Try
                Dim qscoll As System.Collections.Specialized.NameValueCollection = System.Web.HttpUtility.ParseQueryString(Source)
                Dim CacheEnum As IEnumerator = qscoll.GetEnumerator()
                While CacheEnum.MoveNext()
                    Try
                        Dim dr As DataRow = dt.NewRow
                        dr(0) = (CacheEnum.Current)
                        dr(1) = qscoll(CacheEnum.Current)
                        dt.Rows.Add(dr)
                    Catch ex As Exception
                    End Try
                End While
                If Not Query Is Nothing AndAlso Query.Length > 0 Then
                    Dim drf() As System.Data.DataRow = dt.Select(Query)
                    Dim dtr As DataTable = dt.Clone()
                    Dim drfr As DataRow
                    For Each drfr In drf
                        Dim drx As System.Data.DataRow = dtr.NewRow()
                        drx(0) = drfr(0)
                        drx(1) = drfr(1)
                        dtr.Rows.Add(drx)
                    Next
                    dt.Clear()
                    dt = dtr
                End If
            Catch ex As Exception
            End Try
        End Sub
        Private Sub GetRows_Querystring(ByRef Caller As EngineBase, ByVal Query As String, ByRef dt As DataTable)
            Try
                Dim CacheEnum As IEnumerator = Caller.Context.Request.QueryString.GetEnumerator()
                While CacheEnum.MoveNext()
                    Try
                        Dim dr As DataRow = dt.NewRow
                        dr(0) = (CacheEnum.Current)
                        dr(1) = Caller.Context.Request.QueryString(CacheEnum.Current)
                        dt.Rows.Add(dr)
                    Catch ex As Exception
                    End Try
                End While
                If Not Query Is Nothing AndAlso Query.Length > 0 Then
                    Dim drf() As System.Data.DataRow = dt.Select(Query)
                    Dim dtr As DataTable = dt.Clone()
                    Dim drfr As DataRow
                    For Each drfr In drf
                        Dim drx As System.Data.DataRow = dtr.NewRow()
                        drx(0) = drfr(0)
                        drx(1) = drfr(1)
                        dtr.Rows.Add(drx)
                    Next
                    dt.Clear()
                    dt = dtr
                End If
            Catch ex As Exception
            End Try
        End Sub
        Private Sub GetRows_Session(ByRef Caller As EngineBase, ByVal Query As String, ByRef dt As DataTable)
            Try
                Dim CacheEnum As IEnumerator = Caller.Context.Session.GetEnumerator()
                While CacheEnum.MoveNext()
                    Try
                        Dim dr As DataRow = dt.NewRow
                        dr(0) = (CacheEnum.Current)
                        dr(1) = Caller.Context.Session(CacheEnum.Current)
                        dt.Rows.Add(dr)
                    Catch ex As Exception
                    End Try
                End While
                If Not Query Is Nothing AndAlso Query.Length > 0 Then
                    Dim drf() As System.Data.DataRow = dt.Select(Query)
                    Dim dtr As DataTable = dt.Clone()
                    Dim drfr As DataRow
                    For Each drfr In drf
                        Dim drx As System.Data.DataRow = dtr.NewRow()
                        drx(0) = drfr(0)
                        drx(1) = drfr(1)
                        dtr.Rows.Add(drx)
                    Next
                    dt.Clear()
                    dt = dtr
                End If
            Catch ex As Exception
            End Try
        End Sub
        Private Sub GetRows_Cookie(ByRef Caller As EngineBase, ByVal Query As String, ByRef dt As DataTable)
            Try
                Dim CacheEnum As IEnumerator = Caller.Context.Request.Cookies.GetEnumerator()
                While CacheEnum.MoveNext()
                    Try
                        Dim httpc As System.Web.HttpCookie = Caller.Context.Request.Cookies(CacheEnum.Current)
                        Try
                            Dim dr As DataRow = dt.NewRow
                            dr(0) = httpc.Name & ".Domain"
                            dr(1) = httpc.Domain
                            dt.Rows.Add(dr)
                        Catch exx As Exception
                        End Try
                        Try
                            Dim dr As DataRow = dt.NewRow
                            dr(0) = httpc.Name & ".Expires"
                            dr(1) = httpc.Expires
                            dt.Rows.Add(dr)
                        Catch exx As Exception
                        End Try
                        Try
                            Dim dr As DataRow = dt.NewRow
                            dr(0) = httpc.Name & ".HasKeys"
                            dr(1) = httpc.HasKeys
                            dt.Rows.Add(dr)
                        Catch exx As Exception
                        End Try
                        Try
                            Dim dr As DataRow = dt.NewRow
                            dr(0) = httpc.Name & ".HttpOnly"
                            dr(1) = httpc.HttpOnly
                            dt.Rows.Add(dr)
                        Catch exx As Exception
                        End Try
                        Try
                            Dim dr As DataRow = dt.NewRow
                            dr(0) = httpc.Name & ".Path"
                            dr(1) = httpc.Path
                            dt.Rows.Add(dr)
                        Catch exx As Exception
                        End Try
                        Try
                            Dim dr As DataRow = dt.NewRow
                            dr(0) = httpc.Name & ".Secure"
                            dr(1) = httpc.Secure
                            dt.Rows.Add(dr)
                        Catch exx As Exception
                        End Try
                        Try
                            Dim dr As DataRow = dt.NewRow
                            dr(0) = httpc.Name & ".Value"
                            dr(1) = httpc.Value
                            dt.Rows.Add(dr)
                        Catch exx As Exception
                        End Try
                        If httpc.HasKeys Then
                            Try
                                Dim CCacheEnum As IEnumerator = httpc.Values.GetEnumerator
                                While CCacheEnum.MoveNext()
                                    Try
                                        Dim dr As DataRow = dt.NewRow
                                        dr(0) = httpc.Name & ".Values." & (CCacheEnum.Current.Key.ToString())
                                        dr(1) = (CCacheEnum.Current.Value.ToString())
                                        dt.Rows.Add(dr)
                                    Catch exx As Exception
                                    End Try
                                End While
                            Catch ex2 As Exception
                            End Try
                        End If
                    Catch ex As Exception
                    End Try
                End While
                If Not Query Is Nothing AndAlso Query.Length > 0 Then
                    Dim drf() As System.Data.DataRow = dt.Select(Query)
                    Dim dtr As DataTable = dt.Clone()
                    Dim drfr As DataRow
                    For Each drfr In drf
                        Dim drx As System.Data.DataRow = dtr.NewRow()
                        drx(0) = drfr(0)
                        drx(1) = drfr(1)
                        dtr.Rows.Add(drx)
                    Next
                    dt.Clear()
                    dt = dtr
                End If
            Catch ex As Exception
            End Try
        End Sub
        Private Sub GetRows_Context(ByRef Caller As EngineBase, ByVal Query As String, ByRef dt As DataTable)
            Try
                Dim CacheEnum As IDictionaryEnumerator = Caller.Context.Items.GetEnumerator()
                While CacheEnum.MoveNext()
                    Try
                        Dim dr As DataRow = dt.NewRow
                        dr(0) = (CacheEnum.Current.Key.ToString())
                        dr(1) = (CacheEnum.Current.Value.ToString())
                        dt.Rows.Add(dr)
                    Catch ex As Exception
                    End Try
                End While
                If Not Query Is Nothing AndAlso Query.Length > 0 Then
                    Dim drf() As System.Data.DataRow = dt.Select(Query)
                    Dim dtr As DataTable = dt.Clone()
                    Dim drfr As DataRow
                    For Each drfr In drf
                        Dim drx As System.Data.DataRow = dtr.NewRow()
                        drx(0) = drfr(0)
                        drx(1) = drfr(1)
                        dtr.Rows.Add(drx)
                    Next
                    dt.Clear()
                    dt = dtr
                End If
            Catch ex As Exception
            End Try
        End Sub
        Private Sub GetRows_ViewState(ByRef Caller As EngineBase, ByVal Query As String, ByRef dt As DataTable)
            Try
                Dim CacheEnum As IDictionaryEnumerator = Caller.ViewState.GetEnumerator()
                While CacheEnum.MoveNext()
                    Try
                        Dim dr As DataRow = dt.NewRow
                        dr(0) = (CacheEnum.Current.Key.ToString())
                        dr(1) = (CacheEnum.Current.Value.ToString())
                        dt.Rows.Add(dr)
                    Catch ex As Exception
                    End Try
                End While
                If Not Query Is Nothing AndAlso Query.Length > 0 Then
                    Dim drf() As System.Data.DataRow = dt.Select(Query)
                    Dim dtr As DataTable = dt.Clone()
                    Dim drfr As DataRow
                    For Each drfr In drf
                        Dim drx As System.Data.DataRow = dtr.NewRow()
                        drx(0) = drfr(0)
                        drx(1) = drfr(1)
                        dtr.Rows.Add(drx)
                    Next
                    dt.Clear()
                    dt = dtr
                End If
            Catch ex As Exception
            End Try
        End Sub
        Private Sub GetRows_Actions(ByRef Caller As EngineBase, ByVal Query As String, ByRef dt As DataTable)
            Try
                Dim CacheEnum As System.Collections.Generic.IEnumerator(Of Collections.Generic.KeyValuePair(Of String, Object)) = Caller.ActionVariables.GetEnumerator()
                While CacheEnum.MoveNext()
                    Try
                        Dim dr As DataRow = dt.NewRow
                        dr(0) = (CacheEnum.Current.Key.ToString())
                        dr(1) = (CacheEnum.Current.Value.ToString())
                        dt.Rows.Add(dr)
                    Catch ex As Exception
                    End Try
                End While
                If Not Query Is Nothing AndAlso Query.Length > 0 Then
                    Dim drf() As System.Data.DataRow = dt.Select(Query)
                    Dim dtr As DataTable = dt.Clone()
                    Dim drfr As DataRow
                    For Each drfr In drf
                        Dim drx As System.Data.DataRow = dtr.NewRow()
                        drx(0) = drfr(0)
                        drx(1) = drfr(1)
                        dtr.Rows.Add(drx)
                    Next
                    dt.Clear()
                    dt = dtr
                End If
            Catch ex As Exception
            End Try
        End Sub
        Private Sub GetRows_Messages(ByRef Caller As EngineBase, ByVal Query As String, ByRef dt As DataTable)
            Try
                Dim CacheEnum As System.Collections.Generic.IEnumerator(Of Collections.Generic.KeyValuePair(Of String, String)) = Caller.CapturedMessages.GetEnumerator()
                While CacheEnum.MoveNext()
                    Try
                        Dim dr As DataRow = dt.NewRow
                        dr(0) = (CacheEnum.Current.Key.ToString())
                        dr(1) = (CacheEnum.Current.Value.ToString())
                        dt.Rows.Add(dr)
                    Catch ex As Exception
                    End Try
                End While
                If Not Query Is Nothing AndAlso Query.Length > 0 Then
                    Dim drf() As System.Data.DataRow = dt.Select(Query)
                    Dim dtr As DataTable = dt.Clone()
                    Dim drfr As DataRow
                    For Each drfr In drf
                        Dim drx As System.Data.DataRow = dtr.NewRow()
                        drx(0) = drfr(0)
                        drx(1) = drfr(1)
                        dtr.Rows.Add(drx)
                    Next
                    dt.Clear()
                    dt = dtr
                End If
            Catch ex As Exception
            End Try
        End Sub
        Private Sub GetRows_Settings(ByRef Caller As EngineBase, ByVal Query As String, ByRef dt As DataTable)
            Try
                Dim CacheEnum As IDictionaryEnumerator = Caller.Settings.GetEnumerator()
                While CacheEnum.MoveNext()
                    Try
                        Dim dr As DataRow = dt.NewRow
                        dr(0) = (CacheEnum.Current.Key.ToString())
                        dr(1) = (CacheEnum.Current.Value.ToString())
                        dt.Rows.Add(dr)
                    Catch ex As Exception
                    End Try
                End While
                If Not Query Is Nothing AndAlso Query.Length > 0 Then
                    Dim drf() As System.Data.DataRow = dt.Select(Query)
                    Dim dtr As DataTable = dt.Clone()
                    Dim drfr As DataRow
                    For Each drfr In drf
                        Dim drx As System.Data.DataRow = dtr.NewRow()
                        drx(0) = drfr(0)
                        drx(1) = drfr(1)
                        dtr.Rows.Add(drx)
                    Next
                    dt.Clear()
                    dt = dtr
                End If
            Catch ex As Exception
            End Try
        End Sub
        Private Sub GetRows_Headers(ByRef Caller As EngineBase, ByVal Query As String, ByRef dt As DataTable)
            Try
                Dim CacheEnum As IEnumerator = Caller.Context.Request.Headers.GetEnumerator()
                While CacheEnum.MoveNext()
                    Try
                        Dim dr As DataRow = dt.NewRow
                        dr(0) = "Request." & (CacheEnum.Current)
                        dr(1) = Caller.Context.Request.Headers(CacheEnum.Current)
                        dt.Rows.Add(dr)
                    Catch ex As Exception
                    End Try
                End While
            Catch ex As Exception
            End Try
            Try
                Dim CacheEnum As IEnumerator = Caller.Context.Response.Headers.GetEnumerator()
                While CacheEnum.MoveNext()
                    Try
                        Dim dr As DataRow = dt.NewRow
                        dr(0) = "Response." & (CacheEnum.Current)
                        dr(1) = Caller.Context.Request.Headers(CacheEnum.Current)
                        dt.Rows.Add(dr)
                    Catch ex As Exception
                    End Try
                End While
            Catch ex As Exception
            End Try
            Try
                If Not Query Is Nothing AndAlso Query.Length > 0 Then
                    Dim drf() As System.Data.DataRow = dt.Select(Query)
                    Dim dtr As DataTable = dt.Clone()
                    Dim drfr As DataRow
                    For Each drfr In drf
                        Dim drx As System.Data.DataRow = dtr.NewRow()
                        drx(0) = drfr(0)
                        drx(1) = drfr(1)
                        dtr.Rows.Add(drx)
                    Next
                    dt.Clear()
                    dt = dtr
                End If
            Catch ex As Exception
            End Try
        End Sub
        Private Sub GetRows_User(ByRef Caller As EngineBase, ByVal Query As String, ByRef dt As DataTable)
            Try
                Try
                    Dim dr As DataRow = dt.NewRow
                    dr(0) = "Approved"
                    dr(1) = Caller.UserInfo.Approved
                    dt.Rows.Add(dr)
                Catch ex As Exception
                End Try
                Try
                    Dim dr As DataRow = dt.NewRow
                    dr(0) = "DisplayName"
                    dr(1) = Caller.UserInfo.DisplayName
                    dt.Rows.Add(dr)
                Catch ex As Exception
                End Try
                Try
                    Dim dr As DataRow = dt.NewRow
                    dr(0) = "Email"
                    dr(1) = Caller.UserInfo.Email
                    dt.Rows.Add(dr)
                Catch ex As Exception
                End Try
                Try
                    Dim dr As DataRow = dt.NewRow
                    dr(0) = "FirstName"
                    dr(1) = Caller.UserInfo.FirstName
                    dt.Rows.Add(dr)
                Catch ex As Exception
                End Try
                Try
                    Dim dr As DataRow = dt.NewRow
                    dr(0) = "Id"
                    dr(1) = Caller.UserInfo.Id
                    dt.Rows.Add(dr)
                Catch ex As Exception
                End Try
                Try
                    Dim dr As DataRow = dt.NewRow
                    dr(0) = "IsAdministrator"
                    dr(1) = Caller.UserInfo.IsAdministrator
                    dt.Rows.Add(dr)
                Catch ex As Exception
                End Try
                Try
                    Dim dr As DataRow = dt.NewRow
                    dr(0) = "IsSuperUser"
                    dr(1) = Caller.UserInfo.IsSuperUser
                    dt.Rows.Add(dr)
                Catch ex As Exception
                End Try
                Try
                    Dim dr As DataRow = dt.NewRow
                    dr(0) = "LastName"
                    dr(1) = Caller.UserInfo.LastName
                    dt.Rows.Add(dr)
                Catch ex As Exception
                End Try
                Try
                    Dim dr As DataRow = dt.NewRow
                    dr(0) = "LockedOut"
                    dr(1) = Caller.UserInfo.LockedOut
                    dt.Rows.Add(dr)
                Catch ex As Exception
                End Try
                Try
                    Dim dr As DataRow = dt.NewRow
                    dr(0) = "Login"
                    dr(1) = Caller.UserInfo.Login
                    dt.Rows.Add(dr)
                Catch ex As Exception
                End Try
                Try
                    Dim dr As DataRow = dt.NewRow
                    dr(0) = "Password"
                    dr(1) = Caller.UserInfo.Password
                    dt.Rows.Add(dr)
                Catch ex As Exception
                End Try
                Try
                    Dim dr As DataRow = dt.NewRow
                    dr(0) = "SiteId"
                    dr(1) = Caller.UserInfo.SiteId
                    dt.Rows.Add(dr)
                Catch ex As Exception
                End Try
                Try
                    Dim dr As DataRow = dt.NewRow
                    dr(0) = "UserId"
                    dr(1) = Caller.UserInfo.UserId
                    dt.Rows.Add(dr)
                Catch ex As Exception
                End Try
                Try
                    Dim dr As DataRow = dt.NewRow
                    dr(0) = "UserName"
                    dr(1) = Caller.UserInfo.UserName
                    dt.Rows.Add(dr)
                Catch ex As Exception
                End Try
                Try
                    Dim CacheEnum As System.Collections.Generic.IEnumerator(Of Collections.Generic.KeyValuePair(Of String, Object)) = Caller.UserInfo.Properties.GetEnumerator()
                    While CacheEnum.MoveNext()
                        Try
                            Dim dr As DataRow = dt.NewRow
                            dr(0) = (CacheEnum.Current.Key.ToString())
                            dr(1) = (CacheEnum.Current.Value.ToString())
                            dt.Rows.Add(dr)
                        Catch ex As Exception
                        End Try
                    End While

                    If Not Query Is Nothing AndAlso Query.Length > 0 Then
                        Dim drf() As System.Data.DataRow = dt.Select(Query)
                        Dim dtr As DataTable = dt.Clone()
                        Dim drfr As DataRow
                        For Each drfr In drf
                            Dim drx As System.Data.DataRow = dtr.NewRow()
                            drx(0) = drfr(0)
                            drx(1) = drfr(1)
                            dtr.Rows.Add(drx)
                        Next
                        dt.Clear()
                        dt = dtr
                    End If
                Catch ex As Exception
                End Try
            Catch ex As Exception
            End Try
        End Sub
        Private Sub GetRows_Portal(ByRef Caller As EngineBase, ByVal Query As String, ByRef dt As DataTable)
            Try
                Try
                    Dim dr As DataRow = dt.NewRow
                    dr(0) = "ActiveTab.AdministratorRoles"
                    dr(1) = Caller.PortalSettings.ActiveTab.AdministratorRoles
                    dt.Rows.Add(dr)
                Catch ex As Exception
                End Try
                Try
                    Dim dr As DataRow = dt.NewRow
                    dr(0) = "ActiveTab.AuthorizedRoles"
                    dr(1) = Caller.PortalSettings.ActiveTab.AuthorizedRoles
                    dt.Rows.Add(dr)
                Catch ex As Exception
                End Try
                Try
                    Dim dr As DataRow = dt.NewRow
                    dr(0) = "ActiveTab.ContainerPath"
                    dr(1) = Caller.PortalSettings.ActiveTab.ContainerPath
                    dt.Rows.Add(dr)
                Catch ex As Exception
                End Try
                Try
                    Dim dr As DataRow = dt.NewRow
                    dr(0) = "ActiveTab.ContainerSrc"
                    dr(1) = Caller.PortalSettings.ActiveTab.ContainerSrc
                    dt.Rows.Add(dr)
                Catch ex As Exception
                End Try
                Try
                    Dim CacheEnum As IEnumerator = Caller.PortalSettings.ActiveTab.BreadCrumbs.GetEnumerator()
                    While CacheEnum.MoveNext()
                        Try
                            Dim dr As DataRow = dt.NewRow
                            dr(0) = "ActiveTab.BreadCrumbs." & (CacheEnum.Current.Key.ToString())
                            dr(1) = (CacheEnum.Current.Value.ToString())
                            dt.Rows.Add(dr)
                        Catch ex As Exception
                        End Try
                    End While
                Catch ex As Exception
                End Try
                Try
                    Dim dr As DataRow = dt.NewRow
                    dr(0) = "ActiveTab.Description"
                    dr(1) = Caller.PortalSettings.ActiveTab.Description
                    dt.Rows.Add(dr)
                Catch ex As Exception
                End Try
                Try
                    Dim dr As DataRow = dt.NewRow
                    dr(0) = "ActiveTab.DisableLink"
                    dr(1) = Caller.PortalSettings.ActiveTab.DisableLink
                    dt.Rows.Add(dr)
                Catch ex As Exception
                End Try
                Try
                    Dim dr As DataRow = dt.NewRow
                    dr(0) = "ActiveTab.EndDate"
                    dr(1) = Caller.PortalSettings.ActiveTab.EndDate
                    dt.Rows.Add(dr)
                Catch ex As Exception
                End Try
                Try
                    Dim dr As DataRow = dt.NewRow
                    dr(0) = "AdministratorId"
                    dr(1) = Caller.PortalSettings.AdministratorId
                    dt.Rows.Add(dr)
                Catch ex As Exception
                End Try

                If Not Query Is Nothing AndAlso Query.Length > 0 Then
                    Dim drf() As System.Data.DataRow = dt.Select(Query)
                    Dim dtr As DataTable = dt.Clone()
                    Dim drfr As DataRow
                    For Each drfr In drf
                        Dim drx As System.Data.DataRow = dtr.NewRow()
                        drx(0) = drfr(0)
                        drx(1) = drfr(1)
                        dtr.Rows.Add(drx)
                    Next
                    dt.Clear()
                    dt = dtr
                End If
            Catch ex As Exception
            End Try
        End Sub

        Private Function GetPivot(ByRef dt As DataTable, ByVal Name As String) As DataTable
            Dim dr As DataRow
            Dim out As New DataTable(Name)
            For Each dr In dt.Rows
                Dim strName As String
                If Not IsDBNull(dr(0)) Then
                    strName = dr(0)
                Else
                    strName = Nothing
                End If
                If Not strName Is Nothing Then
                    out.Columns.Add(New DataColumn(strName, GetType(String)))
                End If
            Next
            Dim nr As DataRow = out.NewRow()
            For Each dr In dt.Rows
                Dim strName As String
                Dim strValue As String
                If Not IsDBNull(dr(0)) Then
                    strName = dr(0)
                Else
                    strName = Nothing
                End If
                If Not IsDBNull(dr(1)) Then
                    strValue = dr(1)
                Else
                    strValue = Nothing
                End If
                If Not strName Is Nothing Then
                    nr(strName) = strValue
                End If
            Next
            out.Rows.Add(nr)
            Return out
        End Function
        Private Shared Function GetTableStructure(ByVal Name As String) As DataTable
            Dim dt As New DataTable(Name)
            dt.Columns.Add(New DataColumn("Name", GetType(String)))
            dt.Columns.Add(New DataColumn("Value", GetType(String)))
            Return dt
        End Function


    End Class
End Namespace
