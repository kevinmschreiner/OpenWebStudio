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
Imports r2i.OWS.Framework.Utilities
Imports r2i.OWS.Framework.Utilities.Compatibility, r2i.OWS.Framework.Plugins.Actions

Namespace r2i.OWS.Actions
    Public Class InputAction
        Inherits ActionBase



#Region "Debugging and Identification: Name,Style,Description"
        Public Overrides Function Description(ByRef act As MessageActionItem) As String
            Dim str As String = ""
            If Not act.Parameters Is Nothing AndAlso act.Parameters.Count > 0 Then
                str &= Utility.CNullStr(act.Parameters.Item(MessageActionsConstants.ACTIONINPUT_URL_KEY)) & " via method "
                str &= Utility.CNullStr(act.Parameters.Item(MessageActionsConstants.ACTIONINPUT_METHOD_KEY)) & ""
                If Utility.GetDictionaryValue(act.Parameters, MessageActionsConstants.ACTIONINPUT_AUTHENTICATIONTYPE_KEY, "0") <> "0" Then
                    str &= " authenticating as " & Utility.GetDictionaryValue(act.Parameters, MessageActionsConstants.ACTIONINPUT_USERNAME_KEY, "Undefined used")
                End If
            Else
                str &= " (no parameters defined)"
            End If
            Return str
        End Function
        Public Overrides Function Name() As String
            Return "Input"
        End Function
        Public Overrides Function Title(ByRef act As MessageActionItem) As String
            Return Name()
        End Function
        Public Overrides Function Style() As String
            Return ""
        End Function
#End Region

        Public Overrides Function Handle_Action(ByRef Caller As RuntimeBase, ByRef sharedds As System.Data.DataSet, ByRef act As MessageActionItem, ByRef previous As Runtime.ActionExecutionResult, ByRef Debugger As Framework.Debugger) As Runtime.ExecutableResult
            'Dim splitter As New Utility.SmartSplitter
            'splitter.Split(act.ActionInformation)
            If Not act.Parameters Is Nothing Then
                Dim Item_URL As String = Nothing
                Dim Item_QueryString As String = Nothing
                Dim Item_Data As String = Nothing
                Dim Item_Headers As String = Nothing
                Dim Item_ContentType As String = Nothing
                Dim Item_Method As String = Nothing
                Dim Item_VariableType As String = Nothing
                Dim Item_VariableName As String = Nothing
                Dim Item_InputFormat As String = Nothing
                Dim Item_XPATH As String = Nothing
                Dim Item_AuthType As String = Nothing
                Dim Item_Domain As String = Nothing
                Dim Item_UserName As String = Nothing
                Dim Item_Password As String = Nothing
                Dim Item_SoapAction As String = Nothing
                Dim Item_StripEnvelopeTag As String = Nothing
                Dim bDoIt As Boolean = False

                If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONINPUT_URL_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONINPUT_URL_KEY)).Length > 0 Then
                    Item_URL = CStr(act.Parameters(MessageActionsConstants.ACTIONINPUT_URL_KEY))
                End If
                If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONINPUT_QUERYSTRING_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONINPUT_QUERYSTRING_KEY)).Length > 0 Then
                    Item_QueryString = CStr(act.Parameters(MessageActionsConstants.ACTIONINPUT_QUERYSTRING_KEY))
                End If
                If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONINPUT_DATA_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONINPUT_DATA_KEY)).Length > 0 Then
                    Item_Data = CStr(act.Parameters(MessageActionsConstants.ACTIONINPUT_DATA_KEY))
                End If
                If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONINPUT_HEADERS_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONINPUT_HEADERS_KEY)).Length > 0 Then
                    Item_Headers = CStr(act.Parameters(MessageActionsConstants.ACTIONINPUT_HEADERS_KEY))
                End If
                If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONINPUT_CONTENTTYPE_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONINPUT_CONTENTTYPE_KEY)).Length > 0 Then
                    Item_ContentType = CStr(act.Parameters(MessageActionsConstants.ACTIONINPUT_CONTENTTYPE_KEY))
                End If
                If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONINPUT_METHOD_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONINPUT_METHOD_KEY)).Length > 0 Then
                    Item_Method = CStr(act.Parameters(MessageActionsConstants.ACTIONINPUT_METHOD_KEY))
                End If
                If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONINPUT_VARIABLETYPE_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONINPUT_VARIABLETYPE_KEY)).Length > 0 Then
                    Item_VariableType = CStr(act.Parameters(MessageActionsConstants.ACTIONINPUT_VARIABLETYPE_KEY)).Replace("&lt;", "<").Replace("&gt;", ">")
                End If
                If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONINPUT_VARIABLENAME_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONINPUT_VARIABLENAME_KEY)).Length > 0 Then
                    Item_VariableName = CStr(act.Parameters(MessageActionsConstants.ACTIONINPUT_VARIABLENAME_KEY))
                End If
                If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONINPUT_INPUTFORMAT_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONINPUT_INPUTFORMAT_KEY)).Length > 0 Then
                    Item_InputFormat = CStr(act.Parameters(MessageActionsConstants.ACTIONINPUT_INPUTFORMAT_KEY))
                End If
                If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONINPUT_XPATH_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONINPUT_XPATH_KEY)).Length > 0 Then
                    Item_XPATH = CStr(act.Parameters(MessageActionsConstants.ACTIONINPUT_XPATH_KEY))
                End If
                If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONINPUT_AUTHENTICATIONTYPE_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONINPUT_AUTHENTICATIONTYPE_KEY)).Length > 0 Then
                    Item_AuthType = CStr(act.Parameters(MessageActionsConstants.ACTIONINPUT_AUTHENTICATIONTYPE_KEY))
                Else
                    Item_AuthType = "0"
                End If
                If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONINPUT_DOMAIN_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONINPUT_DOMAIN_KEY)).Length > 0 Then
                    Item_Domain = CStr(act.Parameters(MessageActionsConstants.ACTIONINPUT_DOMAIN_KEY))
                Else
                    Item_Domain = ""
                End If
                If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONINPUT_USERNAME_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONINPUT_USERNAME_KEY)).Length > 0 Then
                    Item_UserName = CStr(act.Parameters(MessageActionsConstants.ACTIONINPUT_USERNAME_KEY))
                Else
                    Item_UserName = ""
                End If
                If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONINPUT_PASSWORD_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONINPUT_PASSWORD_KEY)).Length > 0 Then
                    Item_Password = CStr(act.Parameters(MessageActionsConstants.ACTIONINPUT_PASSWORD_KEY))
                Else
                    Item_Password = ""
                End If
                If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONINPUT_SOAPACTION_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONINPUT_SOAPACTION_KEY)).Length > 0 Then
                    Item_SoapAction = CStr(act.Parameters(MessageActionsConstants.ACTIONINPUT_SOAPACTION_KEY))
                End If
                If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONINPUT_SOAPRESULT_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONINPUT_SOAPRESULT_KEY)).Length > 0 Then
                    Item_StripEnvelopeTag = CStr(act.Parameters(MessageActionsConstants.ACTIONINPUT_SOAPRESULT_KEY))
                End If


                Dim URL As String = Caller.Engine.RenderString(sharedds, Item_URL, Caller.Engine.CapturedMessages, False, isPreRender:=False, DebugWriter:=Debugger)
                If Not URL Is Nothing Then
                    Try
                        If URL.ToLower().StartsWith("https") Then
                            System.Net.ServicePointManager.SecurityProtocol = Net.SecurityProtocolType.Tls12
                        End If
                    Catch ExSP As Exception
                    End Try

                    Dim webclientCall As New Net.WebClient

                    'ASSIGN THE URL
                    'VERSION: 1.7.9 - Base Address for Input Action is now rendered properly.
                    If Not Debugger Is Nothing Then
                        r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Initiating Request to external address: " & URL, True)
                    End If
                    Dim strFullURL As String = URL
                    webclientCall.BaseAddress = URL

                    'ASSIGN THE QUERYSTRING
                    Dim QueryString As String = Nothing
                    If Not Item_QueryString Is Nothing Then
                        QueryString = Caller.Engine.RenderString(sharedds, Item_QueryString, Caller.Engine.CapturedMessages, False, isPreRender:=False, DebugWriter:=Debugger)
                    End If

                    If Not QueryString Is Nothing AndAlso QueryString.Length > 0 Then
                        Dim nvpc As New Collections.Specialized.NameValueCollection
                        Dim svcr As String() = QueryString.Split("&"c)
                        If Not svcr Is Nothing AndAlso svcr.Length > 0 Then
                            Dim str As String
                            For Each str In svcr
                                Dim eqs As String() = str.Split("="c)
                                If Not eqs Is Nothing Then
                                    If eqs.Length = 1 Then
                                        nvpc.Add(eqs(0), "")
                                    ElseIf eqs.Length = 2 Then
                                        nvpc.Add(eqs(0), eqs(1))
                                    End If
                                End If
                            Next
                        End If
                        webclientCall.QueryString = nvpc
                        If Not strFullURL.Contains("?") AndAlso Not QueryString.Contains("?") Then
                            strFullURL &= "?"
                        ElseIf Not QueryString.Contains("?") AndAlso strFullURL.Contains("?") Then
                            strFullURL &= "&"
                        End If
                        strFullURL &= QueryString
                    End If


                    'ASSIGN THE DATA BYTE ARRAY
                    Dim data As Byte() = Nothing
                    If Not Item_Data Is Nothing AndAlso Item_Data.Length > 0 Then
                        data = System.Text.UTF8Encoding.UTF8.GetBytes(Caller.Engine.RenderString(sharedds, Item_Data, Caller.Engine.CapturedMessages, False, isPreRender:=False, DebugWriter:=Debugger))
                    End If

                    'ASSIGN THE CONTENT TYPE
                    If Not Item_ContentType Is Nothing AndAlso Item_ContentType.Length > 0 Then
                        Item_ContentType = Caller.Engine.RenderString(sharedds, Item_ContentType, Caller.Engine.CapturedMessages, False, isPreRender:=False, DebugWriter:=Debugger)
                        If Not Item_ContentType Is Nothing AndAlso Item_ContentType.Length > 0 Then
                            webclientCall.Headers.Add("Content-Type", Item_ContentType)
                        End If
                    End If

                    'VERSION 1.9 - Authentication
                    Dim Credentials As System.Net.ICredentials = Nothing
                    If Not Item_AuthType Is Nothing AndAlso Item_AuthType.Length > 0 AndAlso Item_AuthType <> "0" Then
                        Dim strAuthenticationType As String = ""
                        Item_UserName = Caller.Engine.RenderString(sharedds, Item_UserName, Caller.Engine.CapturedMessages, False, isPreRender:=False, DebugWriter:=Debugger)
                        Item_Password = Caller.Engine.RenderString(sharedds, Item_Password, Caller.Engine.CapturedMessages, False, isPreRender:=False, DebugWriter:=Debugger)
                        Item_Domain = Caller.Engine.RenderString(sharedds, Item_Domain, Caller.Engine.CapturedMessages, False, isPreRender:=False, DebugWriter:=Debugger)
                        Select Case Item_AuthType
                            Case "1"
                                strAuthenticationType = "Basic"
                            Case "2"
                                strAuthenticationType = "Windows"
                            Case "3"
                                If Item_Domain <> "" Then
                                    strAuthenticationType = "Digest"
                                End If
                        End Select
                        Credentials = AuthenticationHelper.GetCrendentials(strAuthenticationType, URL, Item_UserName, Item_Password, Item_Domain)
                        If Credentials Is Nothing Then
                            Credentials = System.Net.CredentialCache.DefaultCredentials
                        End If
                        webclientCall.Credentials = Credentials
                    End If

                    'ASSIGN THE REQUEST METHOD
                    Dim requestMethod As String = "GET"
                    Select Case Item_Method.ToUpper
                        Case "POST"
                            requestMethod = "POST"
                        Case "PUT"
                            requestMethod = "PUT"
                        Case "DELETE"
                            requestMethod = "DELETE"
                        Case "SOAP"
                            requestMethod = "POST"
                            Item_SoapAction = Caller.Engine.RenderString(sharedds, Item_SoapAction, Caller.Engine.CapturedMessages, False, isPreRender:=False, DebugWriter:=Debugger)
                            webclientCall.Headers.Add("SOAPAction", Item_SoapAction)
                    End Select

                    'ADD THE HEADERS
                    If Not Item_Headers Is Nothing AndAlso Item_Headers.Length > 0 Then
                        Item_Headers = Caller.Engine.RenderString(sharedds, Item_Headers, Caller.Engine.CapturedMessages, False, isPreRender:=False, DebugWriter:=Debugger)
                        Utility.SendHeaders(Item_Headers, webclientCall)
                    End If

                    If Not Debugger Is Nothing Then
                        Dim sMsg As String
                        If requestMethod = "GET" Then
                            sMsg = "GET data from: "
                        ElseIf requestMethod = "PUT" Then
                            sMsg = "PUT data to: "
                        ElseIf requestMethod = "DELETE" Then
                            sMsg = "DELETE data from: "
                        Else
                            If Item_Method.ToUpper = "SOAP" Then
                                sMsg = "POST/SOAP data to: "
                            Else
                                sMsg = "POST data to: "
                            End If
                        End If
                        sMsg &= webclientCall.BaseAddress
                        If Not QueryString Is Nothing AndAlso QueryString.Length > 0 Then
                            sMsg &= "?" & QueryString
                        End If
                        r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, sMsg, True)
                    End If

                    'GET THE DATA
                    Dim isBinary As Boolean = False
                    Dim responseData() As Byte = Nothing
                    Dim responseString As String = Nothing
                    Try
                        If Not (requestMethod = "PUT" OrElse requestMethod = "DELETE") AndAlso Not data Is Nothing AndAlso data.Length > 0 Then
                            responseData = webclientCall.UploadData("", requestMethod, data)
                        ElseIf requestMethod = "PUT" OrElse requestMethod = "DELETE" Then
                            Dim response As System.Net.HttpWebResponse = Utility.SendHTTPRequest(Credentials, strFullURL, requestMethod, Item_ContentType, data, Item_Headers)
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
                            responseData = webclientCall.DownloadData("")
                        End If
                        If Not Item_InputFormat Is Nothing AndAlso Item_InputFormat = "binary" Then
                            responseString = Nothing
                            isBinary = True
                        Else
                            responseString = Text.UTF8Encoding.UTF8.GetString(responseData)
                        End If
                    Catch ex As Exception
                        responseString = ex.ToString
                    Finally
                        If Not webclientCall Is Nothing Then
                            'webclientCall.Dispose()
                            ' EJW : 03.21.2008 changed to assign to nothing
                            webclientCall = Nothing
                        End If
                    End Try

                    If Item_Method.ToUpper = "SOAP" AndAlso Not Item_StripEnvelopeTag Is Nothing AndAlso Item_StripEnvelopeTag.Length > 0 Then
                        Dim startTag As String = "<" & Item_StripEnvelopeTag & ">"
                        Dim emptyTag As String = "<" & Item_StripEnvelopeTag & "/>"
                        Dim endTag As String = "</" & Item_StripEnvelopeTag & ">"
                        r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Opening SOAP Envelope for tag " & startTag & "..." & endTag, True)
                        Dim startPosition As Integer = responseString.ToUpper.IndexOf(startTag.ToUpper)
                        Dim endPosition As Integer = responseString.ToUpper.LastIndexOf(endTag.ToUpper)
                        If startPosition < 0 Then
                            startPosition = responseString.ToUpper.IndexOf(emptyTag.ToUpper)
                            If startPosition >= 0 Then
                                responseString = ""
                            End If
                        Else
                            startPosition = startPosition + startTag.Length
                            If endPosition > startPosition Then
                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Extracting SOAP Content", True)
                                responseString = responseString.Substring(startPosition, endPosition - startPosition)
                                responseString = Utility.HTMLDecode(responseString)
                            Else
                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "SOAP Extract Failure.", True)
                                responseString = "Invalid Response Structure: Please provide a different Action Soap Target, Actual response was: " & responseString
                            End If
                        End If
                    End If

                    Dim NVP As SortedList(Of String, Object) = Nothing
                    If Item_InputFormat.ToString.ToUpper = "XML" AndAlso Item_XPATH.ToString.Length > 0 Then
                        Try
                            Dim xmlReader As New Xml.XmlTextReader(responseString, Xml.XmlNodeType.Document, Nothing)
                            Dim xmlPDoc As New Xml.XPath.XPathDocument(xmlReader)

                            Dim xmlNav As Xml.XPath.XPathNavigator = xmlPDoc.CreateNavigator
                            Dim xmlNM As Xml.XmlNamespaceManager = BuildNamespaceManager(xmlNav)
                            Dim xmlExp As Xml.XPath.XPathExpression = xmlNav.Compile(Item_XPATH)
                            xmlExp.SetContext(xmlNM)
                            Dim xpIt As Xml.XPath.XPathNodeIterator = xmlNav.Select(xmlExp)
                            If xpIt.Count > 0 Then
                                If xpIt.Count > 0 Then
                                    'Dim xmlNode As Xml.XmlNode
                                    If Item_VariableName.ToUpper.IndexOf("[*INDEX]") >= 0 Then
                                        'ROMAIN: Generic replacement - 08/22/2007
                                        NVP = New SortedList(Of String, Object)
                                        Do While xpIt.MoveNext
                                            Dim Keyname As String = xpIt.CurrentPosition.ToString
                                            If Not NVP.ContainsKey(Keyname) Then
                                                If Item_VariableName.ToUpper.IndexOf("[*CHILD]") > 0 Then
                                                    'WE ARE LOOPING THROUGH THE CHILDREN OF THIS
                                                    Dim NVP2 As New SortedList(Of String, String)
                                                    If xpIt.Current.HasChildren Then
                                                        Dim xpItC As Xml.XPath.XPathNodeIterator
                                                        xpItC = xpIt.Current.SelectChildren(Xml.XPath.XPathNodeType.All)
                                                        Do While xpItC.MoveNext
                                                            Dim cKeyName As String = xpItC.Current.Name
                                                            If Not NVP2.ContainsKey(cKeyName) Then
                                                                If Not xpItC.Current Is Nothing Then
                                                                    If xpItC.Current.Value Is Nothing Then
                                                                        'NVP2.Add(cKeyName, xmlNode.InnerText)
                                                                    ElseIf Not xpItC.Current.Value Is Nothing Then
                                                                        NVP2.Add(cKeyName, xpItC.Current.Value)
                                                                    End If
                                                                End If
                                                            End If
                                                        Loop
                                                    End If
                                                    NVP.Add(Keyname, NVP2)
                                                Else
                                                    'WE ARE LOOPING ONLY THROUGH THE INDEXES
                                                    If Not xpIt.Current Is Nothing Then
                                                        If xpIt.Current.Value Is Nothing Then
                                                            'NVP.Add(Keyname, xmlNode.InnerText)
                                                        ElseIf Not xpIt.Current.Value Is Nothing Then
                                                            NVP.Add(Keyname, xpIt.Current.Value)
                                                        End If
                                                    End If
                                                End If
                                            End If
                                        Loop
                                    ElseIf Item_VariableName.ToUpper.IndexOf("[*CHILD]") >= 0 Then
                                        'WE ARE LOOPING THROUGH THE CHILDREN OF THIS
                                        NVP = New SortedList(Of String, Object)
                                        Dim NVP2 As New SortedList(Of String, String)
                                        xpIt.MoveNext()
                                        If Not xpIt.Current Is Nothing AndAlso xpIt.Current.HasChildren() Then
                                            'Dim xmlNodeC As Xml.XmlNode
                                            Dim xpItC As Xml.XPath.XPathNodeIterator
                                            xpItC = xpIt.Current.SelectChildren(Xml.XPath.XPathNodeType.All)
                                            Do While xpItC.MoveNext
                                                Dim cKeyName As String = xpItC.Current.Name
                                                If Not NVP2.ContainsKey(cKeyName) Then
                                                    If Not xpItC.Current Is Nothing Then
                                                        If xpItC.Current.Value Is Nothing Then
                                                            'NVP2.Add(cKeyName, xmlNode.InnerText)
                                                        ElseIf Not xpItC.Current.Value Is Nothing Then
                                                            NVP2.Add(cKeyName, xpItC.Current.Value)
                                                        End If
                                                    End If
                                                End If
                                            Loop
                                        End If
                                        NVP.Add("LISTX_ROOT_KEY", NVP2)
                                    Else
                                        xpIt.MoveNext()
                                        'xmlNode = CType(xpIt.Current, Xml.IHasXmlNode).GetNode()
                                        If Not xpIt.Current Is Nothing Then
                                            If xpIt.Current.Value Is Nothing Then
                                                'responseString = xmlNode.InnerText
                                            ElseIf Not xpIt.Current.Value Is Nothing Then
                                                responseString = xpIt.Current.Value
                                            End If
                                        End If
                                        xpIt = Nothing
                                    End If
                                End If
                            End If
                            xmlReader.Close()
                        Catch ex As Exception
                            If Not Debugger Is Nothing Then
                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Failed to translate incoming data: " & ex.ToString, True)
                            End If
                        End Try

                    End If
                    Select Case Item_VariableType.ToString.ToUpper
                        Case "<SESSION>"
                            Dim sourcekeyName As String = Caller.Engine.RenderString(sharedds, Item_VariableName, Caller.Engine.CapturedMessages, False, isPreRender:=False)
                            If sourcekeyName.Length > 0 Then
                                If isBinary = True Then
                                    Caller.Engine.Session.Item(sourcekeyName) = responseData
                                    If Not Debugger Is Nothing Then
                                        r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Input: Session[" & sourcekeyName & "] = URL Binary Result of " & responseData.Length & " bytes.", True)
                                    End If
                                Else
                                    Dim SL As SortedList(Of String, String) = XMLAssignment(NVP, responseString, sourcekeyName)
                                    Dim key As String
                                    For Each key In SL.Keys
                                        Dim value As String = SL.Item(key)
                                        Caller.Engine.Session.Item(key) = value
                                    Next
                                    SL = Nothing
                                    If Not Debugger Is Nothing Then
                                        r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Input: Session[" & sourcekeyName & "] = URL Result of " & responseString.Length & " bytes.", True)
                                    End If
                                End If
                            End If
                        Case "<COOKIE>"
                            Dim sourcekeyName As String = Caller.Engine.RenderString(sharedds, Item_VariableName, Caller.Engine.CapturedMessages, False, isPreRender:=False, DebugWriter:=Debugger)
                            If sourcekeyName.Length > 0 Then
                                If isBinary = True Then
                                    Caller.Engine.Response.SetCookie(New Web.HttpCookie(sourcekeyName, Convert.ToBase64String(responseData)))
                                    If Not Debugger Is Nothing Then
                                        r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Input: Cookie[" & sourcekeyName & "] = URL Binary Result of " & responseData.Length & " bytes.", True)
                                    End If
                                Else
                                    Dim SL As SortedList(Of String, String) = XMLAssignment(NVP, responseString, sourcekeyName)
                                    Dim key As String
                                    For Each key In SL.Keys
                                        Dim value As String = SL.Item(key)
                                        Caller.Engine.Response.SetCookie(New Web.HttpCookie(key, value))
                                    Next
                                    SL = Nothing
                                    If Not Debugger Is Nothing Then
                                        r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Input: Cookie[" & sourcekeyName & "] = URL Result of " & responseString.Length & " bytes.", True)
                                    End If
                                End If
                            End If
                        Case "<ACTION>"
                            Dim sourcekeyName As String = Caller.Engine.RenderString(sharedds, Item_VariableName, Caller.Engine.CapturedMessages, False, isPreRender:=False, DebugWriter:=Debugger)
                            If sourcekeyName.Length > 0 Then
                                If isBinary = True Then
                                    Caller.Engine.ActionVariable(sourcekeyName) = responseData
                                    If Not Debugger Is Nothing Then
                                        r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Input: Action[" & sourcekeyName & "] = URL Binary Result of " & responseData.Length & " bytes.", True)
                                    End If
                                Else
                                    Dim SL As SortedList(Of String, String) = XMLAssignment(NVP, responseString, sourcekeyName)
                                    Dim key As String
                                    For Each key In SL.Keys
                                        Dim value As String = SL.Item(key)
                                        Caller.Engine.ActionVariable(key) = value
                                    Next
                                    SL = Nothing
                                    If Not Debugger Is Nothing Then
                                        r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Input: Action[" & sourcekeyName & "] = URL Result of " & responseString.Length & " bytes.", True)
                                    End If
                                End If
                            End If
                        Case "<DATASOURCE>"
                            'TODO: ALLOW SETTING OVERRIDE OF THE DATASOURCE
                            If Not Debugger Is Nothing Then
                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Input: Datasource = URL Result of " & responseString.Length & " bytes.", True)
                            End If
                        Case "<QUERY>"
                            'TODO: ALLOW SETTING OVERRIDE OF THE QUERY
                            If Not Debugger Is Nothing Then
                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Input: Query = URL Result of " & responseString.Length & " bytes.", True)
                            End If
                    End Select
                End If
            End If
            Return New Runtime.ExecutableResult(Runtime.ExecutableResultEnum.Executed, Nothing)
        End Function


        Private Function BuildNamespaceManager(ByVal Doc As Xml.XPath.XPathNavigator) As Xml.XmlNamespaceManager
            Dim xmlNS As New Xml.XmlNamespaceManager(Doc.NameTable)
            Dim xmlNL As Xml.XPath.XPathNodeIterator = Doc.Select("*") 'Doc.GetElementsByTagName("*")
            'ROMAIN: Generic replacement - 08/21/2007
            'Dim sl As New ArrayList
            Dim sl As New List(Of String)

            If Not xmlNL Is Nothing Then
                Do While xmlNL.MoveNext
                    If xmlNL.Current.Prefix Is Nothing OrElse xmlNL.Current.Prefix.Length = 0 Then
                        If Not sl.Contains("default") Then
                            sl.Add("default")
                            xmlNS.AddNamespace("default", xmlNL.Current.NamespaceURI)
                        End If
                    Else
                        If Not sl.Contains(xmlNL.Current.Prefix) Then
                            sl.Add(xmlNL.Current.Prefix)
                            xmlNS.AddNamespace(xmlNL.Current.Prefix, xmlNL.Current.NamespaceURI)
                        End If
                    End If
                Loop
            End If
            Return xmlNS
        End Function

        Private Function XMLAssignmentName(ByVal TargetName As String, ByVal Index As String, ByVal Child As String) As String
            Dim result As String = Replace(TargetName, "[*Index]", Index, 1, -1, CompareMethod.Text)
            result = Replace(result, "[*Child]", Child, 1, -1, CompareMethod.Text)
            Return result
        End Function
        Private Function XMLAssignment(ByRef Multivalue As SortedList(Of String, Object), ByVal SingleValue As String, ByVal TargetName As String) As SortedList(Of String, String)
            Dim SL As New SortedList(Of String, String)
            Dim TargetKey As String
            If Not Multivalue Is Nothing AndAlso Multivalue.Count > 0 Then
                Dim obj As Object
                Dim key As String
                For Each key In Multivalue.Keys
                    'INDEX
                    obj = Multivalue(key)
                    If TypeOf obj Is SortedList(Of String, String) Then
                        'CHILD
                        Dim Multivalue2 As SortedList(Of String, String) = obj
                        Dim ChildKey As String
                        For Each ChildKey In Multivalue2.Keys
                            Dim objC As String = Multivalue2(ChildKey)
                            TargetKey = XMLAssignmentName(TargetName, key, ChildKey)
                            If Not SL.ContainsKey(TargetKey) Then
                                If Not objC Is Nothing Then
                                    SL.Add(TargetKey, objC)
                                Else
                                    SL.Add(TargetKey, "")
                                End If
                            End If
                        Next
                    Else
                        'VALUE
                        TargetKey = XMLAssignmentName(TargetName, key, "")
                        If Not SL.ContainsKey(TargetKey) Then
                            If Not obj Is Nothing Then
                                SL.Add(TargetKey, obj)
                            Else
                                SL.Add(TargetKey, "")
                            End If
                        End If
                    End If
                Next
            Else
                SL.Add(XMLAssignmentName(TargetName, "", ""), SingleValue)
            End If
            Return SL
        End Function

        Public Overrides Function Key() As String
            Return "Action-Input"
        End Function
    End Class
End Namespace