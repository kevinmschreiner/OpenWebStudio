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
Imports System.Net.Mail
Imports System.Collections.Generic
Imports System.Net
Imports System.Reflection
Imports System.Configuration
Imports System.Text

Namespace r2i.OWS.Framework.Utilities
    Public Class Utility
#Region "String Utilities"
        Public Shared Sub ConvertStringToJavascript(ByRef Source As String, ByVal escapeDouble As Boolean)
            If Not Source Is Nothing Then
                If escapeDouble Then
                    Source = Source.Replace("""", "\""").Replace(vbLf, "").Replace(vbCr, "\n")
                Else
                    Source = Source.Replace("'", "\'").Replace(vbLf, "").Replace(vbCr, "\n")
                End If
            Else
                Source = ""
            End If
        End Sub
        Public Shared Sub ConvertStringToJavascript(ByRef Source As Text.StringBuilder, ByVal escapeDouble As Boolean)
            If Not Source Is Nothing Then
                If escapeDouble Then
                    Source.Replace("""", "\""")
                Else
                    Source.Replace("'", "\'")
                End If
                Source.Replace(vbCrLf, "\n")
                Source.Replace(vbLf, "\n")
                Source.Replace(vbCr, "\n")
            Else
                Source = New StringBuilder
            End If
        End Sub
        Public Shared Function CNullStr(ByVal Value As Object, Optional ByVal DefaultValue As String = "") As String
            Try
                If Value Is Nothing OrElse Value Is DBNull.Value Then
                    Return DefaultValue
                ElseIf TypeOf Value Is Byte() Then
                    Return Convert.ToBase64String(CType(Value, Byte()))
                Else
                    Return Convert.ToString(Value)
                End If
            Catch
                Return DefaultValue
            End Try
        End Function
        Public Shared Function CNullInt(ByVal Value As Object, Optional ByVal DefaultValue As Integer = -1) As Integer
            Try
                If Value Is Nothing OrElse Value Is DBNull.Value Then
                    Return DefaultValue
                Else
                    Return Convert.ToInt32(Value)
                End If
            Catch
                Return DefaultValue
            End Try
        End Function
        Public Shared Function CNullBool(ByVal Value As Object, Optional ByVal DefaultValue As Boolean = False) As Boolean
            Try
                If Value Is Nothing OrElse Value Is DBNull.Value Then
                    Return DefaultValue
                Else
                    Return Convert.ToBoolean(Value)
                End If
            Catch
                Return DefaultValue
            End Try
        End Function
        Public Shared Function CNullData(ByVal Row As DataRow, ByVal Column As String, Optional ByVal DefaultValue As Object = "") As Object
            If Not Row Is Nothing AndAlso Not Column Is Nothing AndAlso Row.Table.Columns.Contains(Column) Then
                Dim value As Object = Row.Item(Column)

                If Not value Is DBNull.Value Then
                    Return value.ToString
                Else
                    Return DefaultValue
                End If
            Else
                Return DefaultValue
            End If
        End Function
        ''' <summary>
        ''' Replaces text between two tags
        ''' </summary>
        ''' <param name="Original">Incoming string</param>
        ''' <param name="StartTag">First instance of this tag marks the beginning of the replacement</param>
        ''' <param name="EndTag">First instance of this tag marks the end of the replacement</param>
        ''' <param name="ReplaceWith">Replace the text with this</param>
        ''' <returns>Replaced string</returns>
        Public Shared Function ReplaceBetween(ByVal Original As String, ByVal StartTag As String, ByVal EndTag As String, ByVal ReplaceWith As String) As String
            Dim sRet As String = Original

            If sRet.Contains(StartTag) And sRet.Contains(EndTag) Then
                Dim iStart As Integer = sRet.IndexOf(StartTag)
                Dim iEnd As Integer = sRet.IndexOf(EndTag)

                If iEnd > iStart Then
                    iEnd += EndTag.Length
                    sRet = Original.Substring(0, iStart - 1) & ReplaceWith & Original.Substring(iEnd + 1)
                End If
            End If

            Return sRet
        End Function
        Public Shared Function ShortenString(ByRef Value As String, ByRef Length As Integer) As String
            If Not Value Is Nothing AndAlso Value.Length > Length Then
                Return Value.Substring(0, Length)
            Else
                Return Value
            End If
        End Function

        ''' <summary>
        ''' Returns all text between two given tags
        ''' </summary>
        ''' <param name="Original">Incoming string</param>
        ''' <param name="StartTag">First instance of this tag marks the beginning of the inner text</param>
        ''' <param name="EndTag">First instance of this tag marks the end of the inner text</param>
        ''' <returns>String in between</returns>
        Public Shared Function GetBetween(ByVal Original As String, ByVal StartTag As String, ByVal EndTag As String) As String
            Dim sRet As String = Original

            If Original.Contains(StartTag) And sRet.Contains(EndTag) Then
                Dim iStart As Integer = sRet.IndexOf(StartTag)
                Dim iEnd As Integer = sRet.IndexOf(EndTag)

                If iEnd > iStart Then
                    sRet = sRet.Substring(iStart, iEnd - iStart + (EndTag.Length))
                End If
            End If

            Return sRet
        End Function
        ''' <summary>
        ''' Returns a single string of all Items joined together
        ''' </summary>
        ''' <param name="Items">Strings to join</param>
        ''' <param name="JoinCharacter">String to insert between items. <i>Default is ,</i></param>
        ''' <returns>String</returns>
        Public Shared Function Join(ByVal Items As List(Of String), Optional ByVal JoinCharacter As String = ",") As String
            Dim sRet As String = ""

            For Each s As String In Items
                If sRet <> "" Then sRet &= JoinCharacter
                sRet &= s
            Next

            Return sRet
        End Function
        ''' <summary>
        ''' Returns a concatenated string for all JoinValues, joined with JoinChar.  Checks if each part ends with
        ''' JoinChar first.  If not, then it appends JoinChar prior to JoinValues(i).
        ''' </summary>
        ''' <param name="JoinChar">String to appear between JoinValues</param>
        ''' <param name="JoinValues">Array of strings</param>
        ''' <returns>Single string</returns>
        ''' <remarks></remarks>
        Public Shared Function Join(ByVal JoinChar As String, ByVal ParamArray JoinValues() As String) As String
            Dim s As String = ""

            For Each j As String In JoinValues
                If s <> "" AndAlso Not s.EndsWith(JoinChar) Then
                    s &= JoinChar
                End If
                s &= j
            Next

            Return s
        End Function
        Public Shared Function StringSplit(ByRef Source As String, ByRef Value As String) As String()
            Dim arr As New ArrayList
            Dim position As Integer = 0
            Dim length As Integer = Value.Length
            If Not Source Is Nothing Then
                While position < Source.Length
                    Dim nextposition As Integer
                    nextposition = Source.IndexOf(Value, position)
                    If nextposition >= 0 AndAlso nextposition > position Then
                        Dim substr As String
                        substr = Source.Substring(position, nextposition - position)
                        arr.Add(substr)
                        position = nextposition + 1
                    Else
                        If nextposition < 0 Then
                            Dim substr As String
                            substr = Source.Substring(position, Source.Length - position)
                            arr.Add(substr)
                        End If
                        position = Source.Length
                    End If
                End While
            End If
            Return arr.ToArray(GetType(String))
        End Function
        ''' <summary>
        ''' Returns a valid HEX representation of the incoming byte array
        ''' </summary>
        ''' <param name="Bytes"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetHexString(ByVal Bytes As Byte()) As String
            Dim iLen As Integer = 0
            If Not Bytes Is Nothing AndAlso Bytes.Length > 0 Then
                iLen = Bytes.Length
            End If
            Dim sbBLOB As New StringBuilder(iLen + 2)
            sbBLOB.Append("0x")
            For idx As Integer = 0 To (iLen - 1)
                sbBLOB.Append(String.Format("{0:x2}", Bytes(idx)))
            Next

            Return sbBLOB.ToString()
        End Function
        Public Shared Function GetDictionaryValue(ByVal Dict As IDictionary(Of String, Object), ByVal Key As String, Optional ByVal DefaultValue As String = "") As String
            If Dict.ContainsKey(Key) Then
                If TypeOf Dict(Key) Is Newtonsoft.Json.JavaScriptArray Then
                    Return CNullStr(CType(Dict(Key), Newtonsoft.Json.JavaScriptArray).ToString)
                Else
                    Return CNullStr(Dict(Key))
                End If
            Else
                Return DefaultValue
            End If
        End Function
        Public Shared Function GetDictionaryObject(ByVal Dict As IDictionary(Of String, Object), ByVal Key As String, Optional ByVal DefaultValue As String = "") As Object
            If Dict.ContainsKey(Key) Then
                Return Dict(Key)
            Else
                Return DefaultValue
            End If
        End Function
        Public Shared Function TrimJson(ByVal sStart As String, ByVal sEnd As String, ByVal sJson As String) As String
            Dim output As String = sJson
            If output.Contains(sStart) Then
                output = output.Substring(output.IndexOf(sStart) + sStart.Length)
            End If
            If output.Contains(sEnd) Then
                output = output.Remove(output.LastIndexOf(sEnd))
            End If
            Return output
        End Function

        Public Shared Function XMLPropertyParse_Quick(ByRef Source As String, ByVal Name As String) As String
            Dim iS_s As Integer
            Dim iS_e As Integer
            Dim iS_l As Integer
            iS_s = Source.ToUpper.IndexOf("<" & Name.ToUpper & ">")
            If iS_s > 0 Then
                iS_l = Name.Length + 2
                iS_e = Source.ToUpper.IndexOf("</" & Name.ToUpper & ">", iS_s)
                If iS_e > iS_s + iS_l Then
                    Return Source.Substring(iS_s + iS_l, iS_e - iS_s - iS_l)
                End If
            End If
            Return Nothing
        End Function

#End Region
#Region "Permission Utilities"
        Public Shared Function Security_WebPermission(ByVal url As String) As Boolean
            Dim b As Boolean = False
            Try
                If System.Security.SecurityManager.IsGranted(New System.Net.WebPermission(System.Net.NetworkAccess.Connect, url)) Then
                    Return True
                End If
            Catch ex As Exception
                Return False
            End Try
            Return False
        End Function
        Public Shared Function Security_FilePermission() As Boolean
            Dim b As Boolean = False
            Try
                If System.Security.SecurityManager.IsGranted(New System.Security.Permissions.FileIOPermission(System.Security.Permissions.FileIOPermissionAccess.Read)) Then
                    Return True
                End If
            Catch ex As Exception
                Return False
            End Try
            Return False
        End Function
        Public Shared Function Security_ReflectPermission() As Boolean
            Dim b As Boolean = False
            Try
                If System.Security.SecurityManager.IsGranted(New System.Security.Permissions.ReflectionPermission(System.Security.Permissions.PermissionState.Unrestricted)) Then
                    Return True
                End If
            Catch ex As Exception
                Return False
            End Try
            Return False
        End Function
#End Region
#Region "Configuration Utilities"
        Public Shared Function ConfigurationSetting(ByVal Name As String) As String
            Return System.Configuration.ConfigurationManager.AppSettings.Get(Name)
        End Function
#End Region
#Region "File Utilities"
        Public Shared Sub StreamTransfer(ByRef Source As IO.Stream, ByRef Destination As IO.Stream)
            Dim sreader As New IO.BinaryReader(Source)
            Dim swriter As New IO.BinaryWriter(Destination)
            If Source.CanSeek Then
                sreader.BaseStream.Position = 0

                While sreader.BaseStream.Position < sreader.BaseStream.Length
                    swriter.Write(sreader.ReadByte())
                End While
            Else
                Dim buffer(4096) As Byte
                Dim bytesRead As Integer = 1
                While bytesRead > 0
                    bytesRead = Source.Read(buffer, 0, 4096)
                    swriter.Write(buffer, 0, bytesRead)
                End While
            End If
        End Sub

        Public Shared Function GetContentType(ByVal Extension As String) As String
            Dim rm As Resources.ResourceManager = My.Resources.MIMETypes.ResourceManager
            Dim sMimeType As String = ""

            Extension = Extension.Replace("-", "_")
            If Extension.StartsWith(".") Then Extension = Extension.Substring(1)
            sMimeType = rm.GetString(Extension)
            If sMimeType Is Nothing Then
                sMimeType = ""
            End If
            If sMimeType.Length = 0 Then sMimeType = rm.GetString("_" & Extension)
            If sMimeType Is Nothing OrElse sMimeType.Length = 0 Then sMimeType = "application/octet-stream"

            Return sMimeType
        End Function
#End Region
        Private Shared _Randomizer As New Random
        Public Shared ReadOnly Property Randomizer() As Random
            Get
                Return _Randomizer
            End Get
        End Property

        Private Const ACTIONLIST_SESSION_KEY As String = "OWS.sort" '+ CONFIGURATIONNAME_SESSION_KEY
        Public Shared Property SortStatus(ByVal Session As Web.SessionState.IHttpSessionState, ByVal ConfigurationID As String, ByVal ModuleID As String, ByVal UserID As String) As List(Of Compatibility.SortAction)
            Get
                If Session Is Nothing OrElse Session(ACTIONLIST_SESSION_KEY + ConfigurationID + ModuleID + UserID.ToString()) Is Nothing Then
                    Return Nothing
                Else
                    Return Session(ACTIONLIST_SESSION_KEY + ConfigurationID + ModuleID + UserID.ToString())
                End If
            End Get
            Set(ByVal Value As List(Of Compatibility.SortAction))
                If Not Session Is Nothing Then
                    Session(ACTIONLIST_SESSION_KEY + ConfigurationID + ModuleID + UserID.ToString()) = Value
                End If
            End Set
        End Property

        Public Shared Property CheckedItems(ByVal ModuleID As String, ByVal GroupName As String, ByVal Session As GenericSession) As List(Of String)
            Get
                Dim rst As List(Of String) = Nothing
                Try
                    Dim SessionItemName As String = "xLm" & ModuleID & "s" & GroupName
                    'From the SessionItem - get the name of the Session Variable we are using for list storage
                    Dim SessionDirective As String = Session(SessionItemName)
                    Dim sessionValue As List(Of String) = Nothing
                    If Not SessionDirective Is Nothing Then
                        If Session.Item(SessionDirective) Is Nothing Then
                            sessionValue = New List(Of String)
                        Else
                            sessionValue = Session.Item(SessionDirective)
                        End If
                    End If
                    rst = sessionValue
                Catch ex As Exception
                End Try
                If rst Is Nothing Then
                    rst = New List(Of String)
                End If
                Return rst
            End Get
            Set(ByVal Value As List(Of String))
                Try
                    Dim SessionItemName As String = "xLm" & ModuleID & "s" & GroupName
                    'From the SessionItem - get the name of the Session Variable we are using for list storage
                    Dim SessionDirective As String = Session(SessionItemName)
                    'ROMAIN: 08/21/2007
                    'NOTE: UNUSED variable
                    'Dim sessionValue As ArrayList
                    If Not SessionDirective Is Nothing Then
                        Session.Item(SessionDirective) = Value
                    End If
                Catch ex As Exception

                End Try
            End Set
        End Property
        Public Shared Property UnCheckedItems(ByVal ModuleID As String, ByVal GroupName As String, ByVal Session As GenericSession) As List(Of String)
            Get
                Dim rst As List(Of String) = Nothing
                Try
                    Dim SessionItemName As String = "xLm" & ModuleID & "s" & GroupName
                    'From the SessionItem - get the name of the Session Variable we are using for list storage
                    Dim SessionDirective As String = Session(SessionItemName)
                    Dim sessionValue As List(Of String) = Nothing
                    If Not SessionDirective Is Nothing Then
                        SessionDirective = "Un" & SessionDirective
                        If Session.Item(SessionDirective) Is Nothing Then
                            sessionValue = New List(Of String)
                        Else
                            sessionValue = Session.Item(SessionDirective)
                        End If
                    End If
                    rst = sessionValue
                Catch ex As Exception
                End Try
                If rst Is Nothing Then
                    rst = New List(Of String)
                End If
                Return rst
            End Get
            Set(ByVal Value As List(Of String))
                Try
                    Dim SessionItemName As String = "xLm" & ModuleID & "s" & GroupName
                    'From the SessionItem - get the name of the Session Variable we are using for list storage
                    Dim SessionDirective As String = Session(SessionItemName)
                    If Not SessionDirective Is Nothing Then
                        SessionDirective = "Un" & SessionDirective
                        Session.Item(SessionDirective) = Value
                    End If
                Catch ex As Exception

                End Try
            End Set
        End Property

        Public Shared Sub SendHeaders(ByRef Headers As String, ByRef Client As Net.WebClient)
            Try
                Dim sreader As New IO.StringReader(Headers)
                Dim sLine As String = sreader.ReadLine
                While Not sLine Is Nothing
                    If sLine.Length > 0 Then
                        Dim colonPos As Integer = sLine.IndexOf(":"c)
                        If colonPos > 0 Then
                            Dim key As String = sLine.Substring(0, colonPos).Trim()
                            Dim value As String = ""
                            If colonPos < sLine.Length Then
                                value = sLine.Substring(colonPos + 1).Trim()
                            End If
                            Client.Headers.Add(key, value)
                        End If
                    End If
                    sLine = sreader.ReadLine
                End While

            Catch ex As Exception

            End Try
        End Sub
        Public Shared Sub SendHeaders(ByRef Headers As String, ByRef Client As Net.HttpWebRequest)
            Try
                Dim sreader As New IO.StringReader(Headers)
                Dim sLine As String = sreader.ReadLine
                While Not sLine Is Nothing
                    If sLine.Length > 0 Then
                        Dim colonPos As Integer = sLine.IndexOf(":"c)
                        If colonPos > 0 Then
                            Dim key As String = sLine.Substring(0, colonPos).Trim()
                            Dim value As String = ""
                            If colonPos < sLine.Length Then
                                value = sLine.Substring(colonPos + 1).Trim()
                            End If
                            Client.Headers.Add(key, value)
                        End If
                    End If
                    sLine = sreader.ReadLine
                End While

            Catch ex As Exception

            End Try
        End Sub
        Public Shared Function SendHTTPRequest(ByVal Credentials As System.Net.ICredentials, ByVal Path As String, ByVal Method As String, ByVal ContentType As String, ByVal Data As Byte(), ByVal Headers As String) As System.Net.HttpWebResponse
            Try
                Dim request As System.Net.HttpWebRequest
                request = CType(System.Net.WebRequest.Create(Path), System.Net.HttpWebRequest)
                'Add the credentials if they are supplied
                If Not Credentials Is Nothing Then
                    request.Credentials = Credentials
                End If
                'Set the Request Method
                request.Method = Method

                'Sends the Headers if they are supplied
                If (Not ContentType Is Nothing AndAlso ContentType.Length > 0) OrElse (Not Headers Is Nothing AndAlso Headers.Length > 0) Then
                    request.Headers = New Net.WebHeaderCollection

                    'ASSIGN THE CONTENT TYPE
                    If Not ContentType Is Nothing AndAlso ContentType.Length > 0 Then
                        'wc.ContentType = strcontenttype
                        'request.Headers.Add("Content-Type", ContentType)
                        request.ContentType = ContentType
                    End If


                    If Not Headers Is Nothing AndAlso Headers.Length > 0 Then
                        'ADD THE HEADERS
                        Utility.SendHeaders(Headers, request)
                    End If
                End If

                'WRITE THE DATA IF PROVIDED
                If Not Data Is Nothing AndAlso Data.Length > 0 Then
                    Dim bwriter As New IO.BinaryWriter(request.GetRequestStream)
                    bwriter.Write(Data)
                    bwriter.Close()
                End If

                'GET THE RESPONSE
                Return CType(request.GetResponse(), System.Net.HttpWebResponse)
            Catch ex As Exception
                Throw ex
            End Try
            Return Nothing
        End Function
        Public Shared Function CleanControlCharacters(ByVal value As String) As String
            If Not value Is Nothing Then
                'ASCII character codes 17 through 20 are causing problems in IE - need to remove these 
                Dim i As Integer
                For i = 17 To 20
                    value = value.Replace(Chr(i), "&#" & i.ToString & ";")
                Next
            End If
            Return value
        End Function
        Public Shared Function HTMLEncode(ByVal value As String) As String
            If Not value Is Nothing Then
                Return System.Web.HttpUtility.HtmlEncode(value)
            Else
                'ROMAIN: 08/22/2007
                'NOTE: Replacement Return ""
                Return String.Empty
            End If
        End Function
        Public Shared Function HTMLDecode(ByVal value As String) As String
            If Not value Is Nothing Then
                Return System.Web.HttpUtility.HtmlDecode(value)
            Else
                'ROMAIN: 08/22/2007
                'NOTE: Replacement Return ""
                Return String.Empty
            End If
        End Function
        Public Shared Function URLEncode(ByVal value As String) As String
            If Not value Is Nothing Then
                Return System.Web.HttpUtility.UrlEncode(value)
            Else
                'ROMAIN: 08/22/2007
                'NOTE: Replacement Return ""
                Return String.Empty
            End If
        End Function
        Public Shared Function URLDecode(ByVal value As String) As String
            If Not value Is Nothing Then
                Return System.Web.HttpUtility.UrlDecode(value)
            Else
                'ROMAIN: 08/22/2007
                'NOTE: Replacement Return ""
                Return String.Empty
            End If
        End Function
    End Class

    '    using System;
    'using System.Web;
    'using System.Collections.Specialized;

    'namespace CraigLabs.Common {
    Public Class UrlBuilder
        Inherits UriBuilder
        Private _querystring As System.Collections.Specialized.StringDictionary
#Region "properties"
        Public ReadOnly Property QueryString() As System.Collections.Specialized.StringDictionary
            Get
                If _querystring Is Nothing Then
                    _querystring = New System.Collections.Specialized.StringDictionary
                End If
                Return _querystring
            End Get
        End Property
        Public Property PageName() As String
            Get
                Dim path As String = MyBase.Path
                Return path.Substring(path.LastIndexOf("/") + 1)
            End Get
            Set(ByVal value As String)
                Dim path As String = MyBase.Path
                path = path.Substring(0, path.LastIndexOf("/"))
                MyBase.Path = String.Concat(path, "/", value)
            End Set
        End Property
#End Region
#Region "Constructor"
        Public Sub New()
            MyBase.New()
        End Sub
        Public Sub New(ByVal uri As String)
            MyBase.New(uri)
            PopulateQueryString()
        End Sub
        Public Sub New(ByVal uri As Uri)
            MyBase.New(uri)
            PopulateQueryString()
        End Sub
        Public Sub New(ByVal page As System.Web.UI.Page)
            MyBase.New(page.Request.Url.AbsoluteUri)
            PopulateQueryString()
        End Sub
#End Region
#Region "Public Methods"
        Public Overrides Function ToString() As String
            GetQueryString()
            If Not MyBase.Query Is Nothing AndAlso MyBase.Query.Length > 0 Then
                Return String.Concat(MyBase.Path, MyBase.Query)
            Else
                Return MyBase.Path
            End If
            Return ""
        End Function
        Public Sub Navigate()
            _Navigate(True)
        End Sub
        Public Sub Navigate(ByVal endResponse As Boolean)
            _Navigate(endResponse)
        End Sub
        Private Sub _Navigate(ByVal endResponse As Boolean)
            Dim uri As String = Me.ToString
            System.Web.HttpContext.Current.Response.Redirect(uri, endResponse)
        End Sub
#End Region
#Region "Private Methods"
        Private Sub PopulateQueryString()
            Dim query As String = MyBase.Query
            If (query Is Nothing OrElse query = String.Empty) Then
                Return
            End If
            If (_querystring Is Nothing) Then
                _querystring = New System.Collections.Specialized.StringDictionary()
            End If
            _querystring.Clear()
            query = query.Substring(1) 'remote the ?

            Dim pairs() As String = query.Split(New Char() {"&"c})
            Dim s As String
            For Each s In pairs
                Dim pair() As String = s.Split(New Char() {"="c})
                If pair.Length > 1 Then
                    _querystring(pair(0)) = pair(1)
                Else
                    _querystring(pair(0)) = String.Empty
                End If
            Next
        End Sub

        Public Sub GetQueryString()
            Dim count As Integer = _querystring.Count
            If (count = 0) Then
                MyBase.Query = String.Empty
                Return
            End If
            Dim keys(count - 1) As String
            Dim values(count - 1) As String
            Dim pairs(count - 1) As String
            _querystring.Keys.CopyTo(keys, 0)
            _querystring.Values.CopyTo(values, 0)

            Dim i As Integer
            For i = 0 To count - 1
                pairs(i) = String.Concat(keys(i), "=", values(i))
            Next
            MyBase.Query = String.Join("&", pairs)
        End Sub
#End Region
    End Class
End Namespace