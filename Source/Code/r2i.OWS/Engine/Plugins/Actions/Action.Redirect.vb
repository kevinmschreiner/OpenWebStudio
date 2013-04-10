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
Imports r2i.OWS.Framework.Plugins.Actions
Imports r2i.OWS.Framework.Utilities
Imports r2i.OWS.Framework.Entities
Imports r2i.OWS.Framework.DataAccess
Imports r2i.OWS.Framework.Utilities.Compatibility, r2i.OWS.Actions.Utilities

Namespace r2i.OWS.Actions
    Public Class RedirectAction
        Inherits ActionBase


#Region "Debugging and Identification: Name,Style,Description"
        Public Overrides Function Description(ByRef act As MessageActionItem) As String
            Dim str As String = ""
            If Not act.Parameters Is Nothing AndAlso act.Parameters.Count > 0 Then
                Dim sType As String = Utility.GetDictionaryValue(act.Parameters, MessageActionsConstants.ACTIONREDIRECT_TYPE_KEY)
                If sType.Length > 0 Then
                    str &= Utility.HTMLEncode(sType) & " "
                    Select Case sType.ToLower()
                        Case "link"
                            str &= Utility.HTMLEncode(Utility.GetDictionaryValue(act.Parameters, MessageActionsConstants.ACTIONREDIRECT_LINK_KEY))
                        Case "tab", "page"
                            Dim pageId As String = Utility.HTMLEncode(Utility.GetDictionaryValue(act.Parameters, MessageActionsConstants.ACTIONREDIRECT_PAGEID_KEY))
                            If pageId Is Nothing OrElse pageId.Length = 0 Then
                                pageId = Utility.HTMLEncode(Utility.GetDictionaryValue(act.Parameters, MessageActionsConstants.ACTIONREDIRECT_LINK_KEY))
                            End If
                            str &= pageId
                    End Select
                Else
                    str &= "an undefined location."
                End If
            Else
                str &= " (no parameters defined)"
            End If
            Return str
        End Function
        Public Overrides Function Name() As String
            Return "Redirect"
        End Function
        Public Overrides Function Title(ByRef act As MessageActionItem) As String
            Return Name()
        End Function
        Public Overrides Function Style() As String
            Return ""
        End Function
#End Region

        Public Overrides Function Handle_Action(ByRef Caller As RuntimeBase, ByRef sharedds As System.Data.DataSet, ByRef act As MessageActionItem, ByRef previous As Runtime.ActionExecutionResult, ByRef Debugger As Framework.Debugger) As Runtime.ExecutableResult
            If Not act.Parameters Is Nothing Then

                If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONREDIRECT_TYPE_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONREDIRECT_TYPE_KEY)).Length > 0 Then
                    Dim redirectType As String = CStr(act.Parameters(MessageActionsConstants.ACTIONREDIRECT_TYPE_KEY))


                    If redirectType = "Link" Then
                        If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONREDIRECT_LINK_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONREDIRECT_LINK_KEY)).Length > 0 Then
                            Dim sLink As String = Caller.Engine.RenderString(sharedds, CStr(act.Parameters(MessageActionsConstants.ACTIONREDIRECT_LINK_KEY)), Caller.Engine.CapturedMessages, False, isPreRender:=False, DebugWriter:=Debugger)
                            r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Navigate to Url Link " & CStr(act.Parameters(MessageActionsConstants.ACTIONREDIRECT_LINK_KEY)) & " (" & sLink & ")", True)
                            If IsNumeric(sLink) Then
                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Navigate to Page " & sLink, True)
                                Return New Runtime.ExecutableResult(Runtime.ExecutableResultEnum.Redirected, AbstractFactory.Instance.EngineController.NavigateURL(Convert.ToInt32(sLink)))
                            Else
                                Return New Runtime.ExecutableResult(Runtime.ExecutableResultEnum.Redirected, sLink)
                            End If
                        End If
                    ElseIf redirectType = "Tab" OrElse redirectType = "Page" Then
                        Dim pageId As String = Utility.HTMLEncode(Utility.GetDictionaryValue(act.Parameters, MessageActionsConstants.ACTIONREDIRECT_PAGEID_KEY))
                        If pageId Is Nothing OrElse pageId.Length = 0 Then
                            pageId = Utility.HTMLEncode(Utility.GetDictionaryValue(act.Parameters, MessageActionsConstants.ACTIONREDIRECT_LINK_KEY))
                        End If
                        If Not pageId Is Nothing AndAlso pageId.Length > 0 Then
                            Dim value As String = Caller.Engine.RenderString(sharedds, pageId, Caller.Engine.CapturedMessages, False, isPreRender:=False, DebugWriter:=Debugger)
                            If Not value Is Nothing AndAlso value.Length > 0 Then
                                If IsNumeric(value) Then
                                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Navigate to Page (id) '" & value & "'", True)
                                    Return New Runtime.ExecutableResult(Runtime.ExecutableResultEnum.Redirected, AbstractFactory.Instance.EngineController.NavigateURL(Convert.ToInt32(value)))
                                Else
                                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Navigate to Page (name) '" & value & "'", True)
                                    Return New Runtime.ExecutableResult(Runtime.ExecutableResultEnum.Redirected, AbstractFactory.Instance.EngineController.NavigateURL(value, Caller.Engine.PortalSettings.PortalId))
                                End If
                            Else
                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Navigate to Page failed for '" & pageId & "'", True)
                            End If
                        End If
                    End If
                End If
            End If
            Return New Runtime.ExecutableResult(Runtime.ExecutableResultEnum.Executed, Nothing)
        End Function

        Public Overrides Function Key() As String
            Return "Action-Redirect"
        End Function
    End Class
End Namespace