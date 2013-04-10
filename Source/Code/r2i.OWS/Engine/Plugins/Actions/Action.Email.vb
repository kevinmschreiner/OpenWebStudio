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
    Public Class EmailAction
        Inherits ActionBase


#Region "Debugging and Identification: Name,Style,Description"
        Public Overrides Function Description(ByRef act As MessageActionItem) As String
            Dim str As String = ""
            If Not act.Parameters Is Nothing AndAlso act.Parameters.Count > 0 Then
                Dim sFrom As String = Utility.GetDictionaryValue(act.Parameters, MessageActionsConstants.ACTIONEMAIL_FROM_KEY)
                Dim sTo As String = Utility.GetDictionaryValue(act.Parameters, MessageActionsConstants.ACTIONEMAIL_TO_KEY)
                Dim sCC As String = Utility.GetDictionaryValue(act.Parameters, MessageActionsConstants.ACTIONEMAIL_CC_KEY)
                Dim sBCC As String = Utility.GetDictionaryValue(act.Parameters, MessageActionsConstants.ACTIONEMAIL_BCC_KEY)
                Dim sSubject As String = Utility.GetDictionaryValue(act.Parameters, MessageActionsConstants.ACTIONEMAIL_SUBJECT_KEY)
                Dim sResultType As String = Utility.GetDictionaryValue(act.Parameters, MessageActionsConstants.ACTIONEMAIL_RESULTVARIABLETYPE_KEY).Replace("&lt;", "<").Replace("&gt;", ">")
                Dim sResultName As String = Utility.GetDictionaryValue(act.Parameters, MessageActionsConstants.ACTIONEMAIL_RESULTVARIABLENAME_KEY)
                Dim hasTargets As Boolean = False

                If sFrom.Length > 0 Then
                    str &= "From: " & Utility.HTMLEncode(sFrom) & ". "
                Else
                    str &= "From: Not Assigned, no mail will be sent. "
                End If
                If sTo.Length > 0 Then
                    hasTargets = True
                    str &= "To: " & Utility.HTMLEncode(sTo) & ". "
                End If
                If sCC.Length > 0 Then
                    hasTargets = True
                    str &= "Cc'd to " & Utility.HTMLEncode(sCC) & ". "
                End If
                If sBCC.Length > 0 Then
                    hasTargets = True
                    str &= "Bcc'd to " & Utility.HTMLEncode(sBCC) & ". "
                End If
                If Not hasTargets Then
                    str &= "No target email addresses are provided, no mail will be sent. "
                End If
                If sSubject.Length > 0 Then
                    str &= "With a subject of """ & Utility.HTMLEncode(sSubject) & """."
                End If
                If sResultType.Length > 0 And sResultName.Length > 0 Then
                    str &= "Email result assigned to " & Utility.HTMLEncode(sResultType) & ":" & Utility.HTMLEncode(sResultName) & "."
                End If
            Else
                str &= " not sent due to incomplete parameter assignments"
            End If
            Return str
        End Function
        Public Overrides Function Name() As String
            Return "Email"
        End Function
        Public Overrides Function Title(ByRef act As MessageActionItem) As String
            Return "Send Email"
        End Function
        Public Overrides Function Style() As String
            Return ""
        End Function
#End Region
        Private Function SplitEmailAddresses(ByVal Value As String) As String()
            If Not Value Is Nothing AndAlso Value.Length > 0 Then
                Return Value.Replace(" "c, ";"c).Replace(","c, ";"c).Split(";"c)
            Else
                Return Nothing
            End If
        End Function
        Public Overrides Function Handle_Action(ByRef Caller As RuntimeBase, ByRef sharedds As System.Data.DataSet, ByRef act As MessageActionItem, ByRef previous As Runtime.ActionExecutionResult, ByRef Debugger As Framework.Debugger) As Runtime.ExecutableResult
            If Not act.Parameters Is Nothing Then
                'Dim splitter As New Utility.SmartSplitter
                'splitter.Split(act.ActionInformation)
                'If splitter.Length >= 7 Then
                'If act.Parameters.Count >= 7 Then
                'If Not splitter.Item(0) Is Nothing AndAlso splitter.Item(0).ToString.Length > 0 Then

                If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONEMAIL_FROM_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONEMAIL_FROM_KEY)).Length > 0 Then

                    Dim emailmsg As New MailMessage
                    Dim hasTargets As Boolean = False
                    Dim resultCollection As String = Nothing
                    Dim resultName As String = Nothing

                    'Dim strFrom As String = Caller.Engine.RenderString(sharedds, splitter.Item(0), Caller.Engine.CapturedMessages, False, isPreRender:=False)
                    Dim strFrom As String = Caller.Engine.RenderString(sharedds, CStr(act.Parameters(MessageActionsConstants.ACTIONEMAIL_FROM_KEY)), Caller.Engine.CapturedMessages, False, isPreRender:=False, DebugWriter:=Debugger)
                    If Not strFrom Is Nothing AndAlso strFrom.Length > 0 Then
                        Try
                            If Not Debugger Is Nothing Then
                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Parsing [From]: " & strFrom, True)
                            End If
                            emailmsg.From = New MailAddress(strFrom)
                        Catch ex As Exception
                            If Not Debugger Is Nothing Then
                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Failed to parse [From]: " & strFrom & " : " & ex.ToString, True)
                            End If
                        End Try
                    End If
                    'If Not splitter.Item(1) Is Nothing AndAlso splitter.Item(1).ToString().Length > 0 Then
                    If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONEMAIL_TO_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONEMAIL_TO_KEY)).Length > 0 Then
                        Dim strValue As String = Caller.Engine.RenderString(sharedds, CStr(act.Parameters(MessageActionsConstants.ACTIONEMAIL_TO_KEY)), Caller.Engine.CapturedMessages, False, isPreRender:=False, DebugWriter:=Debugger)
                        If Not strValue Is Nothing AndAlso strValue.Length > 0 Then
                            If Not Debugger Is Nothing Then
                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Parsing [To]: " & strValue, True)
                            End If
                            Dim strEmails() As String = SplitEmailAddresses(strValue)
                            If Not strEmails Is Nothing Then
                                Dim strEmail As String
                                For Each strEmail In strEmails
                                    hasTargets = True
                                    Try
                                        emailmsg.To.Add(New MailAddress(strEmail))
                                    Catch ex As Exception
                                        If Not Debugger Is Nothing Then
                                            r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Failed to parse [To]: " & strEmail & " : " & ex.ToString, True)
                                        End If
                                    End Try
                                Next
                            End If
                        End If
                    End If
                    'If Not splitter.Item(2) Is Nothing AndAlso splitter.Item(2).ToString.Length > 0 Then
                    If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONEMAIL_CC_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONEMAIL_CC_KEY)).Length > 0 Then
                        Dim strValue As String = Caller.Engine.RenderString(sharedds, CStr(act.Parameters(MessageActionsConstants.ACTIONEMAIL_CC_KEY)), Caller.Engine.CapturedMessages, False, isPreRender:=False, DebugWriter:=Debugger)
                        If Not strValue Is Nothing AndAlso strValue.Length > 0 Then
                            If Not Debugger Is Nothing Then
                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Parsing [Cc]: " & strValue, True)
                            End If
                            Dim strEmails() As String = SplitEmailAddresses(strValue)
                            If Not strEmails Is Nothing Then
                                Dim strEmail As String
                                For Each strEmail In strEmails
                                    hasTargets = True
                                    Try
                                        emailmsg.CC.Add(New MailAddress(strEmail))
                                    Catch ex As Exception
                                        If Not Debugger Is Nothing Then
                                            r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Failed to parse [Cc]: " & strEmail & " : " & ex.ToString, True)
                                        End If
                                    End Try
                                Next
                            End If
                        End If
                    End If

                    'If Not splitter.Item(3) Is Nothing AndAlso splitter.Item(3).ToString.Length > 0 Then
                    If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONEMAIL_BCC_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONEMAIL_BCC_KEY)).Length > 0 Then
                        Dim strValue As String = Caller.Engine.RenderString(sharedds, CStr(act.Parameters(MessageActionsConstants.ACTIONEMAIL_BCC_KEY)), Caller.Engine.CapturedMessages, False, isPreRender:=False, DebugWriter:=Debugger)
                        If Not strValue Is Nothing AndAlso strValue.Length > 0 Then
                            If Not Debugger Is Nothing Then
                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Parsing [Bcc]: " & strValue, True)
                            End If
                            Dim strEmails() As String = SplitEmailAddresses(strValue)
                            If Not strEmails Is Nothing Then
                                Dim strEmail As String
                                For Each strEmail In strEmails
                                    hasTargets = True
                                    Try
                                        emailmsg.Bcc.Add(New MailAddress(strEmail))
                                    Catch ex As Exception
                                        If Not Debugger Is Nothing Then
                                            r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Failed to parse [Bcc]: " & strEmail & " : " & ex.ToString, True)
                                        End If
                                    End Try
                                Next
                            End If
                        End If
                    End If

                    'If Not splitter.Item(4) Is Nothing AndAlso splitter.Item(4).ToString.Length > 0 Then
                    If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONEMAIL_FORMAT_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONEMAIL_FORMAT_KEY)).Length > 0 Then
                        If CStr(act.Parameters(MessageActionsConstants.ACTIONEMAIL_FORMAT_KEY)).ToLower = "html" Then
                            emailmsg.IsBodyHtml = True
                        Else
                            emailmsg.BodyEncoding = Encoding.Default
                        End If
                    End If

                    'If Not splitter.Item(5) Is Nothing AndAlso splitter.Item(5).ToString.Length > 0 Then
                    If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONEMAIL_SUBJECT_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONEMAIL_SUBJECT_KEY)).Length > 0 Then
                        emailmsg.Subject = Caller.Engine.RenderString(sharedds, CStr(act.Parameters(MessageActionsConstants.ACTIONEMAIL_SUBJECT_KEY)), Caller.Engine.CapturedMessages, False, isPreRender:=False, DebugWriter:=Debugger)
                    End If

                    'If Not splitter.Item(6) Is Nothing AndAlso splitter.Item(6).ToString.Length > 0 Then
                    If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONEMAIL_BODY_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONEMAIL_BODY_KEY)).Length > 0 Then
                        emailmsg.Body = Caller.Engine.RenderString(sharedds, CStr(act.Parameters(MessageActionsConstants.ACTIONEMAIL_BODY_KEY)), Caller.Engine.CapturedMessages, False, isPreRender:=False, DebugWriter:=Debugger)
                    End If


                    'If Not splitter.Item(7) Is Nothing AndAlso splitter.Item(7).ToString.Length > 0 Then
                    If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONEMAIL_RESULTVARIABLETYPE_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONEMAIL_RESULTVARIABLETYPE_KEY)).Length > 0 Then
                        resultCollection = CStr(act.Parameters(MessageActionsConstants.ACTIONEMAIL_RESULTVARIABLETYPE_KEY)).Replace("&lt;", "<").Replace("&gt;", ">")
                    End If
                    'If Not splitter.Item(8) Is Nothing AndAlso splitter.Item(8).ToString.Length > 0 Then
                    If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONEMAIL_RESULTVARIABLENAME_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONEMAIL_RESULTVARIABLENAME_KEY)).Length > 0 Then
                        resultName = Caller.Engine.RenderString(sharedds, CStr(act.Parameters(MessageActionsConstants.ACTIONEMAIL_RESULTVARIABLENAME_KEY)), Caller.Engine.CapturedMessages, False, isPreRender:=False, DebugWriter:=Debugger)
                    End If

                    If hasTargets Then
                        ' SMTP server configuration
                        Dim smtpserver As String = Nothing
                        Dim smtpauthentication As String = Nothing
                        Dim smtpusername As String = Nothing
                        Dim smtppassword As String = Nothing
                        Dim smtpsslval As String = Nothing
                        Dim smtpenablessl As Boolean = False
                        Dim smtpuseconfig As Boolean = False

                        If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONEMAIL_SMTPSERVER_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONEMAIL_SMTPSERVER_KEY)).Length > 0 Then
                            smtpserver = Caller.Engine.RenderString(sharedds, CStr(act.Parameters(MessageActionsConstants.ACTIONEMAIL_SMTPSERVER_KEY)), Caller.Engine.CapturedMessages, False, isPreRender:=False, DebugWriter:=Debugger)
                        End If
                        If smtpserver Is Nothing OrElse smtpserver = "" Then
                            smtpserver = AbstractFactory.Instance.EngineController.GetHostSettings("SMTPServer")
                        Else
                            smtpuseconfig = True
                        End If

                        If smtpuseconfig AndAlso act.Parameters.ContainsKey(MessageActionsConstants.ACTIONEMAIL_SMTPUSERNAME_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONEMAIL_SMTPUSERNAME_KEY)).Length > 0 Then
                            smtpusername = Caller.Engine.RenderString(sharedds, CStr(act.Parameters(MessageActionsConstants.ACTIONEMAIL_SMTPUSERNAME_KEY)), Caller.Engine.CapturedMessages, False, isPreRender:=False, DebugWriter:=Debugger)
                        End If
                        If smtpusername Is Nothing OrElse smtpusername = "" Then
                            smtpusername = AbstractFactory.Instance.EngineController.GetHostSettings("SMTPUsername")
                        End If

                        If smtpuseconfig AndAlso act.Parameters.ContainsKey(MessageActionsConstants.ACTIONEMAIL_SMTPPASSWORD_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONEMAIL_SMTPPASSWORD_KEY)).Length > 0 Then
                            smtppassword = Caller.Engine.RenderString(sharedds, CStr(act.Parameters(MessageActionsConstants.ACTIONEMAIL_SMTPPASSWORD_KEY)), Caller.Engine.CapturedMessages, False, isPreRender:=False, DebugWriter:=Debugger)
                        End If
                        If smtppassword Is Nothing OrElse smtppassword = "" Then
                            smtppassword = AbstractFactory.Instance.EngineController.GetHostSettings("SMTPPassword")
                        End If


                        If smtpuseconfig AndAlso act.Parameters.ContainsKey(MessageActionsConstants.ACTIONEMAIL_SMTPAUTHTYPE_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONEMAIL_SMTPAUTHTYPE_KEY)).Length > 0 Then
                            smtpauthentication = Caller.Engine.RenderString(sharedds, CStr(act.Parameters(MessageActionsConstants.ACTIONEMAIL_SMTPAUTHTYPE_KEY)), Caller.Engine.CapturedMessages, False, isPreRender:=False, DebugWriter:=Debugger)
                        End If
                        If smtpauthentication Is Nothing OrElse smtpauthentication = "" Then
                            smtpauthentication = AbstractFactory.Instance.EngineController.GetHostSettings("SMTPAuthentication")
                        End If

                        If smtpuseconfig AndAlso act.Parameters.ContainsKey(MessageActionsConstants.ACTIONEMAIL_SMTPSSL_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONEMAIL_SMTPSSL_KEY)).Length > 0 Then
                            smtpsslval = Caller.Engine.RenderString(sharedds, CStr(act.Parameters(MessageActionsConstants.ACTIONEMAIL_SMTPSSL_KEY)), Caller.Engine.CapturedMessages, False, isPreRender:=False, DebugWriter:=Debugger)
                        End If
                        If smtpsslval Is Nothing OrElse smtpsslval = "" Then
                            smtpsslval = AbstractFactory.Instance.EngineController.GetHostSettings("SMTPEnableSSL")
                        End If

                        If Not smtpsslval Is Nothing Then
                            If smtpsslval.Length = 0 OrElse smtpsslval.ToLower.Substring(0, 1) = "f" OrElse smtpsslval.ToLower.Substring(0, 1) = "n" Then
                                smtpenablessl = False
                            Else
                                smtpenablessl = True
                            End If
                        End If

                        Dim bresult As Boolean = False
                        Dim b(15) As Byte
                        Dim thisMailMessageFolder As String

                        Utility.Randomizer.NextBytes(b)
                        thisMailMessageFolder = New Guid(b).ToString

                        Try
                            'PROCESS THE CHILD ACTIONS FOR FILE ATTACHMENTS
                            Try
                                Caller.CurrentMailMessage = emailmsg
                                Caller.CurrentMailMessageFolder = thisMailMessageFolder
                                Caller.Execute(act.ChildActions, Debugger, sharedds)
                                'Caller.ProcessChildActions(act.ChildActions, Debugger, act.Level + 1, sharedds)
                            Catch ex As Exception
                                If Not Debugger Is Nothing Then
                                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Unable to process child actions for this file: " & ex.ToString, True)
                                End If
                            End Try

                            If Not Debugger Is Nothing Then
                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Send Mail: " & "Sending '" & nZ(emailmsg.Subject) & "', To '" & nZ(emailmsg.To) & "', cc '" & nZ(emailmsg.CC) & "', bcc '" & nZ(emailmsg.Bcc) & "'.", True)
                            End If

                            If Not Debugger Is Nothing Then
                                Try
                                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Server: " & smtpserver, True)
                                Catch ex As Exception
                                End Try
                                Try
                                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "From: " & emailmsg.From.ToString, True)
                                Catch ex As Exception
                                End Try
                                Try
                                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "To: " & emailmsg.To.ToString, True)
                                Catch ex As Exception
                                End Try
                                Try
                                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Cc: " & emailmsg.CC.ToString, True)
                                Catch ex As Exception
                                End Try
                                Try
                                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Bcc: " & emailmsg.Bcc.ToString, True)
                                Catch ex As Exception
                                End Try
                                Try
                                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Subject: " & emailmsg.Subject, True)
                                Catch ex As Exception
                                End Try
                            End If


                            Dim emailResult As String = ""
                            emailResult = r2i.OWS.Framework.Utilities.Engine.Utility.SendEMail(emailmsg, smtpserver, smtpauthentication, smtpusername, smtppassword, smtpenablessl)


                            Caller.CurrentMailMessage = Nothing
                            Caller.CurrentMailMessageFolder = Nothing

                            If emailResult.ToUpper = "TRUE" Then
                                bresult = True
                                If Not Debugger Is Nothing Then
                                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Send Mail: " & "Send Completed.", True)
                                End If
                                If Not resultName Is Nothing AndAlso resultName.Length > 0 Then
                                    Caller.Handle_Assignment(resultCollection, resultName, emailResult, 0, Debugger)
                                End If
                            Else
                                If Not Debugger Is Nothing Then
                                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Send Mail: " & "Send Failure: " & emailResult & ".", True)
                                End If
                                bresult = False
                                If Not resultName Is Nothing AndAlso resultName.Length > 0 Then
                                    Caller.Handle_Assignment(resultCollection, resultName, emailResult, 0, Debugger)
                                End If
                            End If
                        Catch ex As Exception
                            If bresult = False Then
                                If Not resultName Is Nothing AndAlso resultName.Length > 0 Then
                                    Caller.Handle_Assignment(resultCollection, resultName, ex.Message, 0, Debugger)
                                End If
                            End If
                        End Try

                        'DeleteEmailFolder(Caller, thisMailMessageFolder)

                    ElseIf Not Debugger Is Nothing Then
                        r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Send Mail: Operation had no targets.", True)
                    End If
                End If
                'End If
            End If
            Return New Runtime.ExecutableResult(Runtime.ExecutableResultEnum.Executed, Nothing)
        End Function
        'Private Sub DeleteEmailFolder(ByRef Caller As MessageActions, ByVal CurrentFolderName As String)
        '    Dim strBasePath As String
        '    If Caller.Engine.ModulePath.EndsWith("/") Or Caller.Engine.ModulePath.EndsWith("\") Then
        '        strBasePath = Caller.Engine.ModulePath
        '    Else
        '        strBasePath = Caller.Engine.ModulePath & "/"
        '    End If
        '    Dim srcConfigDirectory As String = strBasePath & "Mail/"
        '    If Not System.Configuration.ConfigurationSettings.AppSettings("ListX.Mail.Temporary.Directory") Is Nothing Then
        '        srcConfigDirectory = System.Configuration.ConfigurationSettings.AppSettings("ListX.Mail.Temporary.Directory")
        '        If srcConfigDirectory.Length = 0 Then
        '            srcConfigDirectory = strBasePath & "Mail/"
        '        End If
        '    End If
        '    If Not srcConfigDirectory.EndsWith("/") AndAlso Not srcConfigDirectory.EndsWith("\") Then
        '        srcConfigDirectory &= "/"
        '    End If

        '    Dim tmpEmailTarget As String = srcConfigDirectory & CurrentFolderName

        '    Try
        '        tmpEmailTarget = Caller.Engine.Request.MapPath(tmpEmailTarget)
        '    Catch ex As Exception
        '    End Try
        '    Try
        '        Dim dio As New IO.DirectoryInfo(tmpEmailTarget)
        '        If dio.Exists Then
        '            dio.Delete(True)
        '        End If
        '    Catch ex As Exception
        '    End Try
        'End Sub
        Public Function nZ(ByVal value As Object) As String
            If value Is Nothing Then
                'ROMAIN: 08/22/2007
                'NOTE: Replacement Return ""
                Return String.Empty
            Else
                Dim sValue As String = ""

                sValue = value.ToString()
                If sValue = value.GetType().ToString() Then
                    ' ToString() was not overridden
                    sValue = Convert.ToString(value)
                End If
                Return sValue
            End If
        End Function

        Public Overrides Function Key() As String
            Return "Action-Email"
        End Function
    End Class
End Namespace