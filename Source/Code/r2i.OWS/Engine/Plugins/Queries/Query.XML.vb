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
Imports System.Xml
Imports System.Xml.XPath
Imports r2i.OWS.Framework.Utilities
Imports r2i.OWS.Framework
Imports r2i.OWS.Framework.Plugins.Queries

Namespace r2i.OWS.Queries
    Public Class XML
        Inherits QueryBase

        Public Overrides ReadOnly Property QueryTag() As String
            Get
                Return "<XML>"
            End Get
        End Property

        Public Overrides ReadOnly Property QueryStructure() As String
            Get
                Dim s As String = ""

                s &= "<XML>" & vbCrLf
                s &= "   <PATH></PATH>" & vbCrLf
                s &= "   <!---USE ONE OF THE FOLLOWING: VALUE,POST,GET,PUT,DELETE,SOAP-->" & vbCrLf
                s &= "   <VALUE>" & vbCrLf
                s &= "      <CONTENTTYPE></CONTENTTYPE>" & vbCrLf
                s &= "      <NAME></NAME>" & vbCrLf
                s &= "   </VALUE>" & vbCrLf
                s &= "   <POST>" & vbCrLf
                s &= "      <CONTENTTYPE></CONTENTTYPE>" & vbCrLf
                s &= "      <QUERY></QUERY>" & vbCrLf
                s &= "      <HEADERS></HEADERS>" & vbCrLf
                s &= "      <BODY></BODY>" & vbCrLf
                s &= "   </POST>" & vbCrLf
                s &= "   <GET>" & vbCrLf
                s &= "      <CONTENTTYPE></CONTENTTYPE>" & vbCrLf
                s &= "      <QUERY></QUERY>" & vbCrLf
                s &= "      <HEADERS></HEADERS>" & vbCrLf
                s &= "      <BODY></BODY>" & vbCrLf
                s &= "   </GET>" & vbCrLf
                s &= "   <PUT>" & vbCrLf
                s &= "      <CONTENTTYPE></CONTENTTYPE>" & vbCrLf
                s &= "      <QUERY></QUERY>" & vbCrLf
                s &= "      <HEADERS></HEADERS>" & vbCrLf
                s &= "      <BODY></BODY>" & vbCrLf
                s &= "   </PUT>" & vbCrLf
                s &= "   <DELETE>" & vbCrLf
                s &= "      <CONTENTTYPE></CONTENTTYPE>" & vbCrLf
                s &= "      <QUERY></QUERY>" & vbCrLf
                s &= "      <HEADERS></HEADERS>" & vbCrLf
                s &= "      <BODY></BODY>" & vbCrLf
                s &= "   </DELETE>" & vbCrLf
                s &= "   <SOAP>" & vbCrLf
                s &= "      <CONTENTTYPE></CONTENTTYPE>" & vbCrLf
                s &= "      <ACTION></ACTION>" & vbCrLf
                s &= "      <HEADERS></HEADERS>" & vbCrLf
                s &= "      <BODY></BODY>" & vbCrLf
                s &= "   </SOAP>" & vbCrLf
                s &= "   <AUTHENTICATION>" & vbCrLf
                s &= "      <TYPE></TYPE>" & vbCrLf
                s &= "      <USERNAME></USERNAME>" & vbCrLf
                s &= "      <PASSWORD></PASSWORD>" & vbCrLf
                s &= "      <DOMAIN></DOMAIN>" & vbCrLf
                s &= "   </AUTHENTICATION>" & vbCrLf
                s &= "   <TRANSFORM>" & vbCrLf
                s &= "      <SOURCE></SOURCE>" & vbCrLf
                s &= "      <TARGET></TARGET>" & vbCrLf
                s &= "      <DECODE></DECODE>" & vbCrLf
                s &= "   </TRANSFORM>" & vbCrLf
                s &= "   <ROWS></ROWS>" & vbCrLf
                s &= "   <NAMESPACES>" & vbCrLf
                s &= "      <NAMESPACE>" & vbCrLf
                s &= "          <PREFIX></PREFIX>" & vbCrLf
                s &= "          <URI></URI>" & vbCrLf
                s &= "      </NAMESPACE>" & vbCrLf
                s &= "   </NAMESPACE>" & vbCrLf
                s &= "   <COLUMNS>" & vbCrLf
                s &= "      <COLUMN>" & vbCrLf
                s &= "          <NAME></NAME>" & vbCrLf
                s &= "          <XPATH></XPATH>" & vbCrLf
                s &= "      </COLUMN>" & vbCrLf
                s &= "      <COLUMN>" & vbCrLf
                s &= "          <NAME></NAME>" & vbCrLf
                s &= "          <XPATH></XPATH>" & vbCrLf
                s &= "      </COLUMN>" & vbCrLf
                s &= "      <COLUMN>" & vbCrLf
                s &= "          <NAME></NAME>" & vbCrLf
                s &= "          <XPATH></XPATH>" & vbCrLf
                s &= "      </COLUMN>" & vbCrLf
                s &= "   </COLUMNS>" & vbCrLf
                s &= "</XML>"

                Return s
            End Get
        End Property

        Public Overrides Function Handle_GetData(ByRef Caller As EngineBase, ByVal isSubQuery As Boolean, ByVal Query As String, ByVal FilterField As String, ByVal FilterText As String, ByRef DebugWriter As Framework.Debugger, ByVal isRendered As Boolean, Optional ByVal timeout As Integer = -1, Optional ByVal CustomConnection As String = Nothing) As Framework.RuntimeBase.QueryResult
            Dim rslt As New Framework.RuntimeBase.QueryResult(RuntimeBase.ExecutableResultEnum.Executed, New DataSet)
            Dim strResult As String = ""
            Dim debug As Boolean = False
            Dim traceDebug As Boolean = Caller.xls.enableQueryDebug_Log
            If Not DebugWriter Is Nothing Then
                debug = True
            Else
                If traceDebug Then
                    If DebugWriter Is Nothing Then
                        DebugWriter = New r2i.OWS.Framework.Debugger
                    End If
                End If
            End If

            Try
                If Not Query Is Nothing AndAlso Query.Length > 0 Then
                    Dim strDebug As String = ""
                    Dim strPath As String = ""
                    Dim strAuthentication As String = ""
                    Dim strNamespaces As String = ""
                    Dim strMethod_Get As String = ""
                    Dim strMethod_Soap As String = ""
                    Dim strMethod_Post As String = ""
                    Dim strMethod_Put As String = ""
                    Dim strMethod_Delete As String = ""
                    Dim strMethod_Value As String = ""

                    'Dim strBody As String = ""
                    Dim strRows As String = ""
                    Dim strColumns As String = ""
                    Dim strVariables As String = ""

                    Dim strtransform As String = ""
                    Dim strforcetrim As String = ""
                    Dim forceTrim As Boolean = True
                    Dim isValue As Boolean = False

                    strPath = Utility.XMLPropertyParse_Quick(Query, "path")
                    strAuthentication = Utility.XMLPropertyParse_Quick(Query, "authentication")
                    strMethod_Soap = Utility.XMLPropertyParse_Quick(Query, "soap")
                    strMethod_Post = Utility.XMLPropertyParse_Quick(Query, "post")
                    strMethod_Get = Utility.XMLPropertyParse_Quick(Query, "get")
                    strMethod_Put = Utility.XMLPropertyParse_Quick(Query, "put")
                    strMethod_Delete = Utility.XMLPropertyParse_Quick(Query, "delete")
                    strMethod_Value = Utility.XMLPropertyParse_Quick(Query, "value")
                    strNamespaces = Utility.XMLPropertyParse_Quick(Query, "namespaces")
                    strVariables = Utility.XMLPropertyParse_Quick(Query, "variables")
                    strColumns = Utility.XMLPropertyParse_Quick(Query, "columns")
                    strRows = Utility.XMLPropertyParse_Quick(Query, "rows")
                    strtransform = Utility.XMLPropertyParse_Quick(Query, "transform")
                    strforcetrim = Utility.XMLPropertyParse_Quick(Query, "trimall")
                    If Not strforcetrim Is Nothing AndAlso strforcetrim.ToLower.StartsWith("f") Then
                        forceTrim = False
                    End If
                    If Not strMethod_Value Is Nothing AndAlso strMethod_Value.Length > 0 AndAlso (strPath Is Nothing OrElse strPath.Length = 0) Then
                        isValue = True
                    End If

                    'CONDITION PATH
                    If (Not strPath Is Nothing AndAlso strPath.Length > 0) OrElse isValue Then
                        If Not isValue Then
                            If strPath.IndexOf(":") > 0 Then
                                'URI
                                Dim URL As New Uri(strPath)
                                Select Case URL.Scheme
                                    Case Uri.UriSchemeFile
                                        'PATH
                                        Try
                                            Dim sIO As New IO.StreamReader(strPath)
                                            strResult = sIO.ReadToEnd
                                            sIO.Close()
                                            sIO = Nothing
                                        Catch ex As Exception
                                            'THROW SQL EXCEPTION!!!
                                            Throw ex
                                        End Try
                                    Case Else
                                        'Case URL.UriSchemeFtp
                                        'Case URL.UriSchemeGopher
                                        'Case URL.UriSchemeHttp
                                        'Case URL.UriSchemeHttps
                                        'Case URL.UriSchemeMailto
                                        'Case URL.UriSchemeNews
                                        'Case URL.UriSchemeNntp
                                        'URL

                                        'DEEP PARSE AUTHETICATION
                                        ''   <AUTHENTICATION>
                                        ''      <TYPE>None,Basic,Windows</TYPE>
                                        ''      <USERNAME></USERNAME>
                                        ''      <PASSWORD></PASSWORD>
                                        ''      <DOMAIN></DOMAIN>
                                        ''   </AUTHENTICATION>
                                        Dim strAuthenticationType As String = "none"
                                        Dim strAuthenticationUsername As String = ""
                                        Dim strAuthenticationPassword As String = ""
                                        Dim strAuthenticationDomain As String = ""
                                        If Not strAuthentication Is Nothing AndAlso strAuthentication.Length > 0 Then
                                            strAuthenticationType = Utility.XMLPropertyParse_Quick(strAuthentication, "type")
                                            strAuthenticationUsername = Utility.XMLPropertyParse_Quick(strAuthentication, "username")
                                            strAuthenticationPassword = Utility.XMLPropertyParse_Quick(strAuthentication, "password")
                                            strAuthenticationDomain = Utility.XMLPropertyParse_Quick(strAuthentication, "domain")
                                        End If

                                        Dim settings As SortedList = Nothing
                                        Dim strMethod As String = "GET"
                                        Dim isSoapResult As Boolean = False
                                        If Not strMethod_Soap Is Nothing AndAlso strMethod_Soap.Length > 0 Then
                                            'SOAP
                                            strMethod = "POST"
                                            settings = GetData_XML_Method(Nothing, "SOAP", strMethod_Soap, forceTrim)
                                            isSoapResult = True
                                        ElseIf Not strMethod_Post Is Nothing AndAlso strMethod_Post.Length > 0 Then
                                            'POST
                                            strMethod = "POST"
                                            settings = GetData_XML_Method(Nothing, "POST", strMethod_Post, forceTrim)
                                        ElseIf Not strMethod_Put Is Nothing AndAlso strMethod_Put.Length > 0 Then
                                            'POST
                                            strMethod = "PUT"
                                            settings = GetData_XML_Method(Nothing, "PUT", strMethod_Put, forceTrim)
                                        ElseIf Not strMethod_Delete Is Nothing AndAlso strMethod_Delete.Length > 0 Then
                                            'GET
                                            strMethod = "DELETE"
                                            settings = GetData_XML_Method(Nothing, "DELETE", strMethod_Delete, forceTrim)
                                        ElseIf Not strMethod_Get Is Nothing AndAlso strMethod_Get.Length > 0 Then
                                            'GET
                                            settings = GetData_XML_Method(Nothing, "GET", strMethod_Get, forceTrim)
                                        End If

                                        'ASSIGN THE QUERYSTRING
                                        Dim targeturl As String
                                        If settings Is Nothing Then
                                            settings = New SortedList()
                                        End If
                                        If settings.ContainsKey("query") Then
                                            Dim strQuery As String = CType(settings("query"), String)
                                            If strQuery Is Nothing Then strQuery = ""
                                            If strQuery.Length > 0 AndAlso Not strQuery.StartsWith("?") Then
                                                strQuery = "?" & strQuery
                                            End If
                                            targeturl = strPath & strQuery
                                        Else
                                            targeturl = strPath
                                        End If

                                        Try
                                            If targeturl.ToLower().StartsWith("https") Then
                                                System.Net.ServicePointManager.SecurityProtocol = Net.SecurityProtocolType.Tls12
                                                System.Net.ServicePointManager.ServerCertificateValidationCallback =
  Function(se As Object,
  cert As System.Security.Cryptography.X509Certificates.X509Certificate,
  chain As System.Security.Cryptography.X509Certificates.X509Chain,
  sslerror As System.Net.Security.SslPolicyErrors) True

                                            End If
                                        Catch ExSP As Exception
                                        End Try
                                        Dim wc As New Net.WebClient
                                        'wc = Net.HttpWebRequest.Create(targeturl)
                                        'wc.AllowAutoRedirect = True
                                        'wc.Timeout = 10000
                                        wc = New Net.WebClient
                                        wc.BaseAddress = targeturl
                                        wc.Headers = New Net.WebHeaderCollection

                                        'ASSIGN THE CONTENT TYPE
                                        If settings.ContainsKey("contenttype") Then
                                            Dim strcontenttype As String = CType(settings("contenttype"), String)
                                            If Not strcontenttype Is Nothing AndAlso strcontenttype.Length > 0 Then
                                                'wc.ContentType = strcontenttype
                                                wc.Headers.Add("Content-Type", strcontenttype)
                                            End If
                                        End If

                                        If settings.ContainsKey("headers") Then
                                            'ADD THE HEADERS
                                            Utility.SendHeaders(settings("headers"), wc)
                                        End If

                                        'AUTHENTICATION
                                        Dim Credentials As System.Net.ICredentials = AuthenticationHelper.GetCrendentials(strAuthenticationType, targeturl, strAuthenticationUsername, strAuthenticationPassword, strAuthenticationDomain)
                                        If Credentials Is Nothing Then
                                            Credentials = System.Net.CredentialCache.DefaultCredentials
                                        End If
                                        wc.Credentials = Credentials

                                        'DEBUG
                                        If debug Then
                                            If isSoapResult Then
                                                strDebug = "POST/SOAP data to: "
                                            Else
                                                strDebug = strMethod & " data to: "
                                            End If
                                            strDebug &= wc.BaseAddress
                                        End If

                                        'METHOD
                                        'Select Case strMethod.ToLower
                                        '    Case "soap"
                                        '        strMethod = "post"
                                        '        wc.Headers.Add("SOAPAction", strAction)
                                        '        strBody = strBody.Trim
                                        '        isSoapResult = True
                                        'End Select

                                        'ASSIGN THE DATA BYTE ARRAY
                                        Dim data As Byte() = Nothing
                                        If settings.ContainsKey("body") Then
                                            Dim strbody As String = CType(settings("body"), String)
                                            If Not strbody Is Nothing AndAlso strbody.Length > 0 Then
                                                data = System.Text.UTF8Encoding.UTF8.GetBytes(strbody)
                                            End If
                                        End If

                                        'SOAP ACTION
                                        If settings.ContainsKey("action") Then
                                            Dim strAction As String = CType(settings("action"), String)
                                            If Not strAction Is Nothing AndAlso strAction.Length > 0 Then
                                                If Not strAction.StartsWith("""") Then
                                                    wc.Headers.Add("SOAPAction", """" & strAction & """")
                                                Else
                                                    wc.Headers.Add("SOAPAction", strAction)
                                                End If
                                            End If
                                        End If

                                        'GET THE DATA
                                        Dim responseData() As Byte = Nothing
                                        Try
                                            If Not (strMethod = "GET" OrElse strMethod = "DELETE" OrElse strMethod = "PUT") AndAlso Not data Is Nothing AndAlso data.Length > 0 Then
                                                ' EJW: 2009-07-06: Do not explicitly set content length, should be set for you.
                                                ' see: http://msdn.microsoft.com/en-us/library/system.net.webclient.headers.aspx
                                                responseData = wc.UploadData("", strMethod, data)
                                            ElseIf strMethod = "DELETE" Or strMethod = "PUT" Then
                                                Dim strcontenttype As String = Nothing
                                                Dim strheaders As String = Nothing
                                                If settings.ContainsKey("headers") Then
                                                    'ADD THE HEADERS
                                                    strheaders = settings("headers")
                                                End If
                                                If settings.ContainsKey("contenttype") Then
                                                    strcontenttype = CType(settings("contenttype"), String)
                                                End If
                                                Dim response As System.Net.HttpWebResponse = Utility.SendHTTPRequest(Credentials, targeturl, strMethod, strcontenttype, data, strheaders)
                                                If Not response Is Nothing Then
                                                    Dim mx As New IO.MemoryStream
                                                    Dim b As Integer = Nothing
                                                    Try
                                                        Utility.StreamTransfer(response.GetResponseStream, CType(mx, IO.Stream))
                                                    Catch ex As Exception

                                                    End Try
                                                    Dim mreader As New IO.BinaryReader(mx)
                                                    mx.Position = 0
                                                    responseData = mreader.ReadBytes(mx.Length)
                                                    Try
                                                        mx.Close()
                                                    Catch ex As Exception

                                                    End Try
                                                    Try
                                                        response.Close()
                                                    Catch ex As Exception
                                                    End Try
                                                ElseIf response Is Nothing Then
                                                    Throw New Exception("No result was provided from the response, there was no response from the server")
                                                Else
                                                    responseData = New Byte() {}
                                                End If
                                            Else
                                                responseData = (wc.DownloadData(""))
                                            End If

                                            strResult = System.Text.UTF8Encoding.UTF8.GetString(responseData)
                                            Try
                                                wc.Dispose()
                                            Catch ex As Exception
                                            End Try
                                            wc = Nothing

                                        Catch ex As Exception
                                            If Not ex.InnerException Is Nothing Then
                                                Throw New Exception(ex.Message & ":" & ex.InnerException.Message & "<br /> Result: " & strResult)
                                            Else
                                                Throw ex
                                            End If
                                        End Try
                                        If Not strtransform Is Nothing AndAlso Not strtransform.Length = 0 Then
                                            Dim strEnvelope As String = Utility.XMLPropertyParse_Quick(strtransform, "source")
                                            Dim strBaseElement As String = Utility.XMLPropertyParse_Quick(strtransform, "target")
                                            Dim strEnvolopeDecode As String = Utility.XMLPropertyParse_Quick(strtransform, "decode")

                                            If Not strEnvelope Is Nothing AndAlso strEnvelope.Length > 0 Then
                                                Dim startTag As String = "<" & strEnvelope & ">"
                                                Dim emptyTag As String = "<" & strEnvelope & "/>"
                                                Dim endTag As String = "</" & strEnvelope & ">"
                                                If debug Then
                                                    strDebug &= "Opening SOAP Envelope for tag " & startTag & "..." & endTag & "<br>"
                                                End If
                                                Dim startPosition As Integer = strResult.ToUpper.IndexOf(startTag.ToUpper)
                                                Dim endPosition As Integer = strResult.ToUpper.LastIndexOf(endTag.ToUpper)
                                                If startPosition < 0 Then
                                                    startPosition = strResult.ToUpper.IndexOf(emptyTag.ToUpper)
                                                    If startPosition >= 0 Then
                                                        startPosition = -1
                                                        strResult = ""
                                                    Else
                                                        startPosition = strResult.ToUpper.IndexOf(startTag.ToUpper.Substring(0, startTag.Length - 1))
                                                    End If
                                                End If
                                                If startPosition >= 0 Then
                                                    startPosition = startPosition + startTag.Length
                                                    If endPosition > startPosition Then
                                                        If debug Then
                                                            strDebug &= "Extracting SOAP Content" & "<br>"
                                                        End If

                                                        strResult = strResult.Substring(startPosition, endPosition - startPosition)
                                                        If Not strEnvolopeDecode Is Nothing AndAlso strEnvolopeDecode.ToLower.StartsWith("t") Then
                                                            strResult = Utility.HTMLDecode(strResult)
                                                        End If
                                                    Else
                                                        If debug Then
                                                            strDebug &= "SOAP Extract Failure." & "<br>"
                                                        End If
                                                        strResult = "Invalid Envelope Structure: Please provide a correct soap envelope, Actual response was: " & strResult
                                                    End If
                                                End If
                                            End If

                                            If Not strBaseElement Is Nothing AndAlso strBaseElement.Length > 0 Then
                                                strResult = "<" & strBaseElement & ">" & strResult & "</" & strBaseElement & ">"
                                            End If
                                        End If
                                End Select
                            Else
                                'RELATIVE PATH
                                Try
                                    Dim sIO As New IO.StreamReader(Caller.Request.MapPath(strPath))
                                    strResult = sIO.ReadToEnd
                                    sIO.Close()
                                    sIO = Nothing
                                Catch ex As Exception
                                    'THROW SQL EXCEPTION!!!
                                    Throw ex
                                End Try
                            End If
                        Else
                            If Not strMethod_Value Is Nothing AndAlso strMethod_Value.Length > 0 Then
                                Dim settings As SortedList = Nothing
                                Dim rc As New MiniRenderer(Caller, Caller.TableVariables, Caller.CapturedMessages, True, isRendered, , , , , FilterText, FilterField, DebugWriter)
                                settings = GetData_XML_Method(rc, "VALUE", strMethod_Value, forceTrim)
                                If Not settings Is Nothing AndAlso settings.ContainsKey("body") Then
                                    strResult = settings("body")
                                End If
                            End If
                        End If
                        'CHECK THE ROWS
                        If strRows Is Nothing OrElse strRows.Length = 0 Then
                            'ATTEMPT DATASET
                            Dim myDataDocument As System.Xml.XmlDataDocument = New System.Xml.XmlDataDocument
                            'File strResult
                            myDataDocument.DataSet.ReadXml(New IO.StringReader(strResult.TrimStart()), XmlReadMode.Auto)
                            rslt.Value = myDataDocument.DataSet

                            If Not rslt.Value Is Nothing AndAlso rslt.Value.Tables.Count > 1 Then
                                If rslt.Value.Tables(0).TableName.ToUpper = "NEWDATASET" Then
                                    Dim ds2 As New DataSet
                                    ds2.Tables.Add(rslt.Value.Tables(1).Copy())
                                    rslt.Value = ds2
                                End If
                            End If

                        Else
                            'CHECK THE COLUMNS
                            Dim dt As New DataTable
                            Dim dcolumns As New Collections.Specialized.NameValueCollection
                            If Not strColumns Is Nothing AndAlso strColumns.Length > 0 Then
                                Dim strSplit As String() = Utilities.Utility.StringSplit(strColumns, "</COLUMN>")
                                Dim strValue As String
                                For Each strValue In strSplit
                                    If strValue.Length > 0 Then
                                        Dim strName As String = Utility.XMLPropertyParse_Quick(strValue, "name")
                                        Dim strXPath As String = Utility.XMLPropertyParse_Quick(strValue, "xpath")
                                        If Not strName Is Nothing AndAlso strName.Length > 0 AndAlso Not strXPath Is Nothing Then
                                            dt.Columns.Add(strName)
                                            dcolumns.Add(strName, strXPath)
                                        End If
                                    End If
                                Next
                            End If

                            'CHECK THE NAMESPACES
                            Dim dschema As New Collections.Specialized.NameValueCollection
                            If Not strNamespaces Is Nothing AndAlso strNamespaces.Length > 0 Then
                                Dim strSplit As String() = Utilities.Utility.StringSplit(strNamespaces, "</NAMESPACE>")
                                Dim strValue As String
                                For Each strValue In strSplit
                                    If strValue.Length > 0 Then
                                        Dim strPrefix As String = Utility.XMLPropertyParse_Quick(strValue, "prefix")
                                        Dim strUri As String = Utility.XMLPropertyParse_Quick(strValue, "uri")
                                        If Not strPrefix Is Nothing AndAlso strPrefix.Length > 0 AndAlso Not strUri Is Nothing Then
                                            dschema.Add(strPrefix, strUri)
                                        End If
                                    End If
                                Next
                            Else
                                dschema.Add(Nothing, System.Xml.Schema.XmlSchema.Namespace)
                            End If
                            rslt.Value.Tables.Add(dt)
                            GetData_XMLBind(Caller, dt, strRows, strVariables, dschema, dcolumns, strResult)
                        End If
                    End If
                End If
            Catch ex As Exception
                Framework.Utilities.Utility.SortStatus(Caller.Session, Caller.ConfigurationID.ToString.Replace("{", "").Replace("}", "").Replace("-", ""), Caller.ModuleID, Caller.UserID) = Nothing
                rslt.Result = RuntimeBase.ExecutableResultEnum.Failed
                rslt.Error = ex
                '1.9.9.x - ejw: Added extra debugging for XML response. 
                If Not strResult Is Nothing AndAlso strResult.Length > 0 Then
                    rslt.Error = New Exception(ex.Message & " XML Result:" & System.Web.HttpUtility.HtmlEncode(strResult))
                End If
            End Try

            Return rslt
        End Function

        Private Function GetData_XML_Method(ByRef CallRuntime As MiniRenderer, ByVal MethodType As String, ByRef Source As String, ByVal ForceTrim As Boolean) As SortedList
            ''   <VALUE>
            ''      <CONTENTTYPE></CONTENTTYPE>
            ''      <NAME></NAME>
            ''   </VALUE>
            ''   <POST>
            ''      <CONTENTTYPE></CONTENTTYPE>
            ''      <QUERY></QUERY>
            ''      <BODY></BODY>
            ''   </POST>
            ''   <GET>
            ''      <CONTENTTYPE></CONTENTTYPE>
            ''      <QUERY></QUERY>
            ''      <BODY></BODY>
            ''   </GET>
            ''   <SOAP>
            ''      <CONTENTTYPE></CONTENTTYPE>
            ''      <ACTION></ACTION>
            ''      <BODY></BODY>
            ''   </SOAP>
            Dim srt As New SortedList
            Dim strContentType As String = Utility.XMLPropertyParse_Quick(Source, "contenttype")
            Dim strHeaders As String = Utility.XMLPropertyParse_Quick(Source, "headers")
            If Not strContentType Is Nothing Then
                If ForceTrim Then strContentType = strContentType.Trim
                srt.Add("contenttype", strContentType)
            End If
            If Not strHeaders Is Nothing Then
                srt.Add("headers", strHeaders)
            End If
            Select Case MethodType.ToUpper
                Case "SOAP"
                    Dim strAction As String = Utility.XMLPropertyParse_Quick(Source, "action")
                    If Not strAction Is Nothing Then
                        If ForceTrim Then strAction = strAction.Trim
                        srt.Add("action", strAction)
                    End If
                    Dim strBody As String = Utility.XMLPropertyParse_Quick(Source, "body")
                    If Not strBody Is Nothing Then
                        If ForceTrim Then strBody = strBody.Trim
                        srt.Add("body", strBody)
                    End If
                Case "VALUE"
                    Dim strBody As String = Utility.XMLPropertyParse_Quick(Source, "name")
                    If Not strBody Is Nothing Then
                        If ForceTrim Then strBody = strBody.Trim

                        strBody = CallRuntime.RenderString("[" & strBody & "," & strContentType & "]")
                        srt.Add("body", strBody)
                    End If
                Case "POST", "GET", "PUT", "DELETE"
                    Dim strAction As String = Utility.XMLPropertyParse_Quick(Source, "query")
                    If Not strAction Is Nothing Then
                        If ForceTrim Then
                            strAction = strAction.Trim
                        End If
                        srt.Add("query", strAction)
                    End If
                    Dim strBody As String = Utility.XMLPropertyParse_Quick(Source, "body")
                    If Not strBody Is Nothing AndAlso strBody.Length > 0 Then
                        If ForceTrim Then strBody = strBody.Trim
                        srt.Add("body", strBody)
                    End If
            End Select
            Return srt
        End Function

        
        Private Sub GetData_XMLBind(ByRef Caller As r2i.OWS.Engine, ByRef dt As DataTable, ByRef RowPath As String, ByRef strVariables As String, ByRef dSchema As Specialized.NameValueCollection, ByRef dcolumns As Specialized.NameValueCollection, ByVal strResult As String)
            Dim xmlNs As New System.Xml.XmlNamespaceManager(New System.Xml.NameTable)
            Dim strNKey As String
            'Dim xmlSch As New System.Xml.Schema.XmlSchema()
            'xmlSch.TargetNamespace = "http://www.w3.org/1999/XSL/Transform"

            Dim namespaces As String = ""
            For Each strNKey In dSchema.Keys
                xmlNs.AddNamespace(strNKey, dSchema(strNKey))
                'xmlSch.Namespaces.Add(strNKey, dSchema(strNKey))
                namespaces &= " xmlns:" & strNKey & "=""" & dSchema(strNKey) & """"
            Next

            Dim xmlC As New XmlParserContext(Nothing, xmlNs, Nothing, XmlSpace.None)

            Dim xmlSet As New XmlReaderSettings()
            xmlSet.ConformanceLevel = ConformanceLevel.Fragment

            If Not strResult.ToLower.Trim.StartsWith("<?xml") Then
                strResult = "<root" & namespaces & ">" & strResult & "</root>"
            End If

            Dim xmlR As XmlReader = XmlReader.Create(New IO.StringReader(strResult), xmlSet)

            'Dim xmlDoc As New System.Xml.XmlDocument(xmlNs.NameTable)


            ''xmlDoc.LoadXml(strResult)

            Dim xmlD As New XPathDocument(xmlR)

            Dim xmlN As XPathNavigator = xmlD.CreateNavigator()
            xmlR.Close()

            ''CREATE EXPRESSION
            Dim expr As XPathExpression
            expr = xmlN.Compile(RowPath)
            expr.SetContext(xmlNs)



            If Not dt Is Nothing Then
                Dim xmlNI As XPathNodeIterator = xmlN.Select(expr)
                If dt.Columns.Count = 0 Then
                    'LOOP THROUGH THE ROWS
                    While xmlNI.MoveNext
                        Dim dr As DataRow = dt.NewRow()
                        'LOOP THROUGH THE COLUMNS
                        Dim xPcN As XPathNavigator = xmlNI.Current
                        Dim xmlNIc As XPathNodeIterator = xPcN.SelectChildren(XPathNodeType.All)
                        If dt.Columns.Count = 0 Then
                            While xmlNIc.MoveNext
                                dt.Columns.Add(xmlNIc.Current.Name)
                            End While
                            xmlNIc = xPcN.SelectChildren(XPathNodeType.All)
                        End If
                        While xmlNIc.MoveNext
                            dr.Item(xmlNIc.Current.Name) = xmlNIc.Current.Value
                        End While
                        dt.Rows.Add(dr)
                    End While
                Else
                    'LOOP THROUGH THE ROWS
                    While xmlNI.MoveNext
                        Dim dr As DataRow = dt.NewRow()
                        'LOOP THROUGH THE COLUMNS
                        Dim strName As String
                        For Each strName In dcolumns.Keys
                            Dim strPath As String = dcolumns.Item(strName)

                            Dim xPcN As XPathNavigator = xmlNI.Current.Clone

                            Dim exprC As XPathExpression = xPcN.Compile(strPath)
                            exprC.SetContext(xmlNs)
                            Dim xmlNIc As XPathNodeIterator = xPcN.Select(exprC)

                            If xmlNIc.MoveNext Then
                                dr.Item(strName) = xmlNIc.Current.Value
                            End If
                        Next
                        dt.Rows.Add(dr)
                    End While
                End If
            End If

            'CHECK THE VARIABLES
            'VERSION: 2.0 - GetDate_XMLBind - Adjusted to include VARIABLE assignment.
            If Not strVariables Is Nothing AndAlso strVariables.Length > 0 Then
                Dim strSplit As String() = Utility.StringSplit(strVariables, "</VARIABLE>")
                Dim strValue As String
                For Each strValue In strSplit
                    If strValue.Length > 0 Then
                        Dim strName As String = Utility.XMLPropertyParse_Quick(strValue, "name")
                        Dim strCollection As String = Utility.XMLPropertyParse_Quick(strValue, "collection")
                        Dim strXPath As String = Utility.XMLPropertyParse_Quick(strValue, "xpath")
                        If Not strName Is Nothing AndAlso strName.Length > 0 AndAlso Not strXPath Is Nothing Then
                            Dim xmlNI As XPathNodeIterator = xmlN.Select(strXPath)
                            Dim strNodeValue As String
                            If Not xmlNI Is Nothing AndAlso xmlNI.Count > 0 Then
                                xmlNI.MoveNext()
                                strNodeValue = xmlNI.Current.Value
                                If strCollection Is Nothing OrElse strCollection.Length = 0 Then strCollection = "ACTION"
                                If Not dt Is Nothing AndAlso (strCollection.ToUpper = "SYSTEM" OrElse strCollection.ToUpper = "S") AndAlso strName.ToUpper = "TOTALRECORDS" Then
                                    Dim dtRC As New DataTable("TotalRecords")
                                    dtRC.Columns.Add(New DataColumn("TotalRecords", GetType(System.Int32)))
                                    Dim drRCr As DataRow = dtRC.NewRow
                                    drRCr(0) = strNodeValue
                                    dtRC.Rows.Add(drRCr)
                                    dt.DataSet.Tables.Add(dtRC)
                                Else
                                    Caller.RenderString_Assignment_Assign(strName, strNodeValue, strCollection, "", False)
                                End If
                            End If
                        End If
                    End If
                Next
            End If
        End Sub
    End Class
End Namespace
