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
Imports r2i.OWS.Framework.Plugins.Actions
Imports r2i.OWS.Framework
Imports r2i.OWS.Framework.Utilities

Imports r2i.OWS.Framework.Entities
Imports r2i.OWS.Framework.DataAccess
Imports r2i.OWS.Framework.Utilities.Compatibility, r2i.OWS.Actions.Utilities

Namespace r2i.OWS.Actions
    Public Class AssignmentAction
        Inherits ActionBase
#Region "Debugging and Identification: Name,Style,Description"
        Public Overrides Function Description(ByRef act As MessageActionItem) As String
            Dim str As String = ""
            If Not act.Parameters Is Nothing AndAlso act.Parameters.Count > 0 Then
                Dim action As String = Nothing
                Dim name As String = Nothing
                Dim value As String = Nothing
                Dim skipRendering As Boolean = False
                Dim assignmentType As Integer = 0

                If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONASSIGNEMENT_TYPE_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONASSIGNEMENT_TYPE_KEY)).Length > 0 Then
                    action = CStr(act.Parameters(MessageActionsConstants.ACTIONASSIGNEMENT_TYPE_KEY)).Replace("&lt;", "<").Replace("&gt;", ">")
                End If
                If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONASSIGNEMENT_NAME_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONASSIGNEMENT_NAME_KEY)).Length > 0 Then
                    name = CStr(act.Parameters(MessageActionsConstants.ACTIONASSIGNEMENT_NAME_KEY))
                Else
                    name = ""
                End If
                If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONASSIGNEMENT_VALUE_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONASSIGNEMENT_VALUE_KEY)).Length > 0 Then
                    value = CStr(act.Parameters(MessageActionsConstants.ACTIONASSIGNEMENT_VALUE_KEY))
                Else
                    value = ""
                End If
                If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONASSIGNEMENT_SKIPPROCESSING_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONASSIGNEMENT_SKIPPROCESSING_KEY)).Length > 0 Then
                    skipRendering = CBool(act.Parameters(MessageActionsConstants.ACTIONASSIGNEMENT_SKIPPROCESSING_KEY))
                End If
                If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONASSIGNEMENT_ASSIGNEMENTTYPE_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONASSIGNEMENT_ASSIGNEMENTTYPE_KEY)).Length > 0 Then
                    assignmentType = CInt(act.Parameters(MessageActionsConstants.ACTIONASSIGNEMENT_ASSIGNEMENTTYPE_KEY))
                End If


                str &= Utility.HTMLEncode(" " & action & " variable ")

                If name.Length > 0 Then
                    str &= Utility.HTMLEncode(" '" & name & "'")
                End If
                If value.Length > 0 Then
                    str &= Utility.HTMLEncode(" to '" & value & "'.")
                Else
                    str &= " to Nothing."
                End If
            Else
                str &= " no defined parameters."
            End If
            Return str
        End Function
        Public Overrides Function Name() As String
            Return "Assignment"
        End Function
        Public Overrides Function Key() As String
            Return "Action-Assignment"
        End Function
        Public Overrides Function Title(ByRef act As MessageActionItem) As String
            Return Name()
        End Function
        Public Overrides Function Style() As String
            Return ""
        End Function
#End Region

        Public Overrides Function Handle_Action(ByRef Caller As RuntimeBase, ByRef sharedds As System.Data.DataSet, ByRef act As MessageActionItem, ByRef previous As Runtime.ActionExecutionResult, ByRef Debugger As Framework.Debugger) As Runtime.ExecutableResult

            'dim strName as String = Dotnetnuke.Entities.Users.UserInfo
            If Not act.Parameters Is Nothing Then
                Dim action As String = Nothing
                Dim name As String = Nothing
                Dim value As String = Nothing
                Dim skipRendering As Boolean = False
                Dim assignmentType As Integer = 0

                'Dim splitter As New Utility.SmartSplitter
                'splitter.Split(act.ActionInformation)

                'If splitter.Length >= 3 Then
                If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONASSIGNEMENT_TYPE_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONASSIGNEMENT_TYPE_KEY)).Length > 0 Then
                    action = CStr(act.Parameters(MessageActionsConstants.ACTIONASSIGNEMENT_TYPE_KEY)).Replace("&lt;", "<").Replace("&gt;", ">")
                End If
                If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONASSIGNEMENT_NAME_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONASSIGNEMENT_NAME_KEY)).Length > 0 Then
                    name = CStr(act.Parameters(MessageActionsConstants.ACTIONASSIGNEMENT_NAME_KEY))
                End If
                If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONASSIGNEMENT_VALUE_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONASSIGNEMENT_VALUE_KEY)).Length > 0 Then
                    value = CStr(act.Parameters(MessageActionsConstants.ACTIONASSIGNEMENT_VALUE_KEY))
                End If
                If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONASSIGNEMENT_SKIPPROCESSING_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONASSIGNEMENT_SKIPPROCESSING_KEY)).Length > 0 Then
                    skipRendering = CBool(act.Parameters(MessageActionsConstants.ACTIONASSIGNEMENT_SKIPPROCESSING_KEY))
                End If
                If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONASSIGNEMENT_ASSIGNEMENTTYPE_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONASSIGNEMENT_ASSIGNEMENTTYPE_KEY)).Length > 0 Then
                    assignmentType = CInt(act.Parameters(MessageActionsConstants.ACTIONASSIGNEMENT_ASSIGNEMENTTYPE_KEY))
                End If

                If Not name Is Nothing Then
                    name = Caller.Engine.RenderString(sharedds, name, Caller.Engine.CapturedMessages, False, isPreRender:=False, DebugWriter:=Debugger)
                Else
                    name = ""
                End If
                If Not skipRendering AndAlso Not value Is Nothing Then
                    value = Caller.Engine.RenderString(sharedds, value, Caller.Engine.CapturedMessages, False, isPreRender:=False, DebugWriter:=Debugger)
                End If

                If Not value Is Nothing AndAlso (value.ToUpper = "[RENDER,SYSTEM]" OrElse value.ToUpper = "[|RENDER,SYSTEM]") Then
                    value = Caller.Handler_Render(sharedds, act, Debugger)
                End If

                Return Handle_Assignment(Caller, action, name, value, assignmentType, Debugger)
            End If
            Return New Runtime.ExecutableResult(Runtime.ExecutableResultEnum.Executed, Nothing)
        End Function
        Public Function Handle_Assignment(ByRef Caller As Runtime, ByVal strAction As String, ByVal strName As String, ByVal strValue As String, ByVal iAssignmentType As Integer, ByRef Debugger As r2i.OWS.Framework.Debugger) As Runtime.ExecutableResult
            Try
                strAction = strAction.Replace("&lt;", "<").Replace("&gt;", ">")
                Select Case strAction
                    Case "<Session>"
                        Dim cValue As Object = Nothing
                        Dim xValue As Object = Nothing
                        Select Case iAssignmentType
                            Case 0
                                If strValue Is Nothing OrElse strValue.Length = 0 Then
                                    '_Engine.Session.Item(strName) = Nothing
                                    Caller.Engine.Session.Remove(strName)
                                    If Not Debugger Is Nothing Then
                                        r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Assignment: Session[" & strName & "] = Nothing", True)
                                    End If
                                Else
                                    xValue = strValue
                                End If
                            Case 1
                                If Caller.Engine.Session.Item(strName) Is Nothing Then
                                    cValue = ""
                                Else
                                    cValue = Caller.Engine.Session.Item(strName)
                                End If
                                xValue = cValue & strValue
                            Case 3
                                If Caller.Engine.Session.Item(strName) Is Nothing Then
                                    cValue = ""
                                Else
                                    cValue = Caller.Engine.Session.Item(strName)
                                End If
                                xValue = strValue & cValue
                            Case 2
                                If Caller.Engine.Session.Item(strName) Is Nothing Then
                                    cValue = 0.0
                                Else
                                    cValue = Caller.Engine.Session.Item(strName)
                                End If
                                xValue = CType(cValue, Double) + CType(strValue, Double)
                        End Select
                        Caller.Engine.Session.Item(strName) = xValue
                        If Not Debugger Is Nothing Then
                            r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Assignment: Session[" & strName & "] = '" & Caller.Engine.Session.Item(strName) & "'", True)
                        End If
                    Case "<ViewState>"
                        Dim cValue As Object = Nothing
                        Dim xValue As Object = Nothing
                        Select Case iAssignmentType
                            Case 0
                                If strValue Is Nothing OrElse strValue.Length = 0 Then
                                    '_Engine.ViewState.Item(strName) = Nothing
                                    Caller.Engine.ViewState.Remove(strName)
                                    If Not Debugger Is Nothing Then
                                        r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Assignment: ViewState[" & strName & "] = Nothing", True)
                                    End If
                                Else
                                    xValue = strValue
                                End If
                            Case 1
                                If Caller.Engine.ViewState.Item(strName) Is Nothing Then
                                    cValue = ""
                                Else
                                    cValue = Caller.Engine.ViewState.Item(strName)
                                End If
                                xValue = cValue & strValue
                            Case 2
                                If Caller.Engine.ViewState.Item(strName) Is Nothing Then
                                    cValue = 0.0
                                Else
                                    cValue = Caller.Engine.ViewState.Item(strName)
                                End If
                                xValue = CType(cValue, Double) + CType(strValue, Double)
                            Case 3
                                If Caller.Engine.ViewState.Item(strName) Is Nothing Then
                                    cValue = ""
                                Else
                                    cValue = Caller.Engine.ViewState.Item(strName)
                                End If
                                xValue = strValue & cValue
                        End Select
                        Caller.Engine.ViewState.Item(strName) = xValue

                        If Not Debugger Is Nothing Then
                            r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Assignment: ViewState[" & strName & "] = '" & Caller.Engine.ViewState.Item(strName) & "'", True)
                        End If
                    Case "<SharedCache>"
                        Dim cValue As Object = Nothing
                        Dim xValue As Object = Nothing
                        Select Case iAssignmentType
                            Case 0
                                If strValue Is Nothing OrElse strValue.Length = 0 Then
                                    '_Engine.ViewState.Item(strName) = Nothing
                                    Engine.SharedCache_Remove(strName)

                                    If Not Debugger Is Nothing Then
                                        r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Assignment: SharedCache[" & strName & "] = Nothing", True)
                                    End If
                                Else
                                    xValue = strValue
                                End If
                            Case 1
                                cValue = Engine.SharedCache_Get(strName)
                                If (cValue Is Nothing) Then cValue = ""
                                xValue = cValue & strValue
                            Case 3
                                cValue = Engine.SharedCache_Get(strName)
                                If (cValue Is Nothing) Then cValue = ""
                                xValue = strValue & cValue
                            Case 2
                                cValue = Engine.SharedCache_Get(strName)
                                If (cValue Is Nothing) Then cValue = 0.0
                                xValue = CType(cValue, Double) + CType(strValue, Double)
                        End Select
                        Engine.SharedCache_Set(strName, xValue)


                        If Not Debugger Is Nothing Then
                            r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Assignment: SharedCache[" & strName & "] = '" & Engine.SharedCache_Get(strName) & "'", True)
                        End If
                    Case "<Cache>"
                        Dim cValue As Object = Nothing
                        Dim xValue As Object = Nothing
                        Select Case iAssignmentType
                            Case 0
                                If strValue Is Nothing OrElse strValue.Length = 0 Then
                                    '_Engine.ViewState.Item(strName) = Nothing
                                    Caller.Engine.Context.Cache.Remove(strName)

                                    If Not Debugger Is Nothing Then
                                        r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Assignment: Cache[" & strName & "] = Nothing", True)
                                    End If
                                Else
                                    xValue = strValue
                                End If
                            Case 1
                                If Caller.Engine.Context.Cache.Item(strName) Is Nothing Then
                                    cValue = ""
                                Else
                                    cValue = Caller.Engine.Context.Cache.Item(strName)
                                End If
                                xValue = cValue & strValue
                            Case 3
                                If Caller.Engine.Context.Cache.Item(strName) Is Nothing Then
                                    cValue = ""
                                Else
                                    cValue = Caller.Engine.Context.Cache.Item(strName)
                                End If
                                xValue = strValue & cValue
                            Case 2
                                If Caller.Engine.Context.Cache.Item(strName) Is Nothing Then
                                    cValue = 0.0
                                Else
                                    cValue = Caller.Engine.Context.Cache.Item(strName)
                                End If
                                xValue = CType(cValue, Double) + CType(strValue, Double)
                        End Select
                        Caller.Engine.Context.Cache.Item(strName) = xValue


                        If Not Debugger Is Nothing Then
                            r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Assignment: Cache[" & strName & "] = '" & Caller.Engine.Context.Cache.Item(strName) & "'", True)
                        End If
                    Case "<Context>"
                        Dim cValue As Object = Nothing
                        Dim xValue As Object = Nothing
                        Select Case iAssignmentType
                            Case 0
                                If strValue Is Nothing OrElse strValue.Length = 0 Then
                                    If Caller.Engine.Context.Items.Contains(strName) Then
                                        Caller.Engine.Context.Items.Remove(strName)
                                    End If

                                    If Not Debugger Is Nothing Then
                                        r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Assignment: Context[" & strName & "] = Nothing", True)
                                    End If
                                Else
                                    xValue = strValue
                                End If
                            Case 1
                                If Caller.Engine.Context.Items.Item(strName) Is Nothing Then
                                    cValue = ""
                                Else
                                    cValue = Caller.Engine.Context.Items.Item(strName)
                                End If
                                xValue = cValue & strValue
                            Case 3
                                If Caller.Engine.Context.Items.Item(strName) Is Nothing Then
                                    cValue = ""
                                Else
                                    cValue = Caller.Engine.Context.Items.Item(strName)
                                End If
                                xValue = strValue & cValue
                            Case 2
                                If Caller.Engine.Context.Items.Item(strName) Is Nothing Then
                                    cValue = 0.0
                                Else
                                    cValue = Caller.Engine.Context.Items.Item(strName)
                                End If
                                xValue = CType(cValue, Double) + CType(strValue, Double)
                        End Select
                        Caller.Engine.Context.Items.Item(strName) = xValue

                        If Not Debugger Is Nothing Then
                            r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Assignment: Context[" & strName & "] = '" & Caller.Engine.Context.Items.Item(strName) & "'", True)
                        End If
                    Case "<Cookie>"
                        If strName.Length > 0 Then
                            Dim cValue As Object = Nothing
                            Dim xValue As Object = Nothing
                            Dim setExpires As Boolean = False
                            Dim httpOnly As Boolean = False
                            'ADD Cookie Name.expires to Documentation
                            If strName.ToLower.EndsWith(".expires") Then
                                strName = strName.Substring(0, strName.Length - 8)
                                setExpires = True
                            End If
                            If strName.ToLower.EndsWith(".httponly") Then
                                strName = strName.Substring(0, strName.Length - 9)
                                httpOnly = True
                            End If
                            Select Case iAssignmentType
                                Case 0
                                    If strValue Is Nothing OrElse strValue.Length = 0 Then
                                        If strName.Length > 0 Then
                                            If Not Caller.Engine.Response.Cookies(strName) Is Nothing Then
                                                Caller.Engine.Response.Cookies(strName).HttpOnly = httpOnly
                                                Caller.Engine.Response.Cookies(strName).Value = ""
                                                Caller.Engine.Response.Cookies(strName).Expires = Now.Subtract(New TimeSpan(24, 0, 0))
                                            End If
                                            If Not Debugger Is Nothing Then
                                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Assignment: Cookie[" & strName & "] = Nothing", True)
                                            End If
                                        End If
                                    Else
                                        xValue = strValue
                                    End If
                                Case 1
                                    If Caller.Engine.Response.Cookies(strName) Is Nothing Then
                                        cValue = ""
                                    Else
                                        cValue = Caller.Engine.Response.Cookies(strName).Value
                                    End If
                                    xValue = cValue & strValue
                                Case 3
                                    If Caller.Engine.Response.Cookies(strName) Is Nothing Then
                                        cValue = ""
                                    Else
                                        cValue = Caller.Engine.Response.Cookies(strName).Value
                                    End If
                                    xValue = strValue & cValue
                                Case 2
                                    If Caller.Engine.Response.Cookies(strName) Is Nothing Then
                                        cValue = 0.0
                                    Else
                                        cValue = Caller.Engine.Response.Cookies(strName).Value
                                    End If
                                    xValue = CType(cValue, Double) + CType(strValue, Double)
                            End Select

                            'VERSION: 2.0.1
                            'ASSIGN TO BOTH THE REQUEST AND RESPONSE FOR PROPER CONSUMPTION
                            If Not Caller.Engine.Request.Cookies(strName) Is Nothing Then
                                If setExpires AndAlso IsDate(xValue) Then
                                    Caller.Engine.Request.Cookies(strName).Expires = CDate(xValue)
                                Else
                                    Caller.Engine.Request.Cookies(strName).HttpOnly = httpOnly
                                    Caller.Engine.Request.Cookies(strName).Value = xValue
                                End If
                            Else
                                ' _Engine.Response.AppendCookie(New Web.HttpCookie(strName, xvalue))
                                Dim ck As New Web.HttpCookie(strName, xValue)
                                ck.HttpOnly = httpOnly
                                Caller.Engine.Request.Cookies.Add(ck)
                            End If

                            If Not Caller.Engine.Response.Cookies(strName) Is Nothing Then
                                If setExpires AndAlso IsDate(xValue) Then
                                    Caller.Engine.Response.Cookies(strName).Expires = CDate(xValue)
                                Else
                                    Caller.Engine.Response.Cookies(strName).HttpOnly = httpOnly
                                    Caller.Engine.Response.Cookies(strName).Value = xValue
                                End If
                            Else
                                Dim ck As New Web.HttpCookie(strName, xValue)
                                ck.HttpOnly = httpOnly
                                Caller.Engine.Response.AppendCookie(ck)
                            End If

                            If Not Debugger Is Nothing Then
                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Assignment: Cookie[" & strName & "] = '" & xValue & "'", True)
                            End If
                        End If
                    Case "<Action>"
                        Dim cValue As Object = Nothing
                        Dim xValue As Object = Nothing
                        Select Case iAssignmentType
                            Case 0
                                If strValue Is Nothing OrElse strValue.Length = 0 Then
                                    If strName.ToLower.EndsWith(".complete") Then
                                        ThreadedMessageHandler.ResetProcess(Left(strName, strName.Length - 9), Caller.Engine.Session.SessionID)
                                    Else
                                        Caller.Engine.ActionVariable(strName) = Nothing
                                    End If
                                    If Not Debugger Is Nothing Then
                                        r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Assignment: Action[" & strName & "] = Nothing", True)
                                    End If
                                Else
                                    xValue = strValue
                                End If
                            Case 1
                                If Caller.Engine.ActionVariable(strName) Is Nothing Then
                                    cValue = ""
                                Else
                                    cValue = Caller.Engine.ActionVariable(strName)
                                End If
                                xValue = cValue & strValue
                            Case 3
                                If Caller.Engine.ActionVariable(strName) Is Nothing Then
                                    cValue = ""
                                Else
                                    cValue = Caller.Engine.ActionVariable(strName)
                                End If
                                xValue = strValue & cValue
                            Case 2
                                If Caller.Engine.ActionVariable(strName) Is Nothing Then
                                    cValue = 0.0
                                Else
                                    cValue = Caller.Engine.ActionVariable(strName)
                                End If
                                xValue = CType(cValue, Double) + CType(strValue, Double)
                        End Select
                        Caller.Engine.ActionVariable(strName) = xValue

                        If Not Debugger Is Nothing Then
                            r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Assignment: Action[" & strName & "] = '" & Caller.Engine.ActionVariable(strName) & "'", True)
                        End If
                    Case "<Module Setting>"
                        Try
                            If strValue Is Nothing OrElse strValue.Length = 0 Then
                                strValue = ""
                            End If

                            Dim cValue As Object = Nothing
                            Dim xValue As Object = Nothing
                            Select Case iAssignmentType
                                Case 0
                                    xValue = strValue
                                Case 1
                                    If Caller.Engine.Settings Is Nothing OrElse Not Caller.Engine.Settings.ContainsKey(strName) Then
                                        cValue = ""
                                    Else
                                        cValue = Caller.Engine.Settings(strName)
                                    End If
                                    xValue = cValue & strValue
                                Case 3
                                    If Caller.Engine.Settings Is Nothing OrElse Not Caller.Engine.Settings.ContainsKey(strName) Then
                                        cValue = ""
                                    Else
                                        cValue = Caller.Engine.Settings(strName)
                                    End If
                                    xValue = strValue & cValue
                                Case 2
                                    If Caller.Engine.Settings Is Nothing OrElse Not Caller.Engine.Settings.ContainsKey(strName) Then
                                        cValue = 0.0
                                    Else
                                        cValue = Caller.Engine.Settings(strName)
                                    End If
                                    xValue = CType(cValue, Double) + CType(strValue, Double)
                            End Select

                            'ROMAIN: 09/19/07
                            'Dim mc As New DotNetNuke.Entities.Modules.ModuleController
                            'mc.UpdateModuleSetting(Caller.Engine.ModuleID, strName, xvalue)

                            AbstractFactory.Instance.EngineController.UpdateModuleSetting(Caller.Engine.ModuleID, strName, xValue)

                            If Not Caller.Engine.Settings Is Nothing Then
                                If Caller.Engine.Settings.ContainsKey(strName) Then
                                    Caller.Engine.Settings(strName) = xValue
                                    If Not Debugger Is Nothing Then
                                        r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Assignment: Module Setting[" & strName & "] = '" & xValue & "'", True)
                                    End If
                                Else
                                    Caller.Engine.Settings.Add(strName, xValue)
                                    If Not Debugger Is Nothing Then
                                        r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Assignment: Adding Module Setting[" & strName & "] = '" & xValue & "'", True)
                                    End If
                                End If
                            End If
                        Catch ex As Exception
                        End Try
                    Case "<Tab Module Setting>"
                        Try
                            If strValue Is Nothing OrElse strValue.Length = 0 Then
                                strValue = ""
                            End If

                            Dim cValue As Object = Nothing
                            Dim xValue As Object = Nothing
                            Select Case iAssignmentType
                                Case 0
                                    xValue = strValue
                                Case 1
                                    If Caller.Engine.Settings Is Nothing OrElse Not Caller.Engine.Settings.ContainsKey(strName) Then
                                        cValue = ""
                                    Else
                                        cValue = Caller.Engine.Settings(strName)
                                    End If
                                    xValue = cValue & strValue
                                Case 1
                                    If Caller.Engine.Settings Is Nothing OrElse Not Caller.Engine.Settings.ContainsKey(strName) Then
                                        cValue = ""
                                    Else
                                        cValue = Caller.Engine.Settings(strName)
                                    End If
                                    xValue = strValue & cValue
                                Case 2
                                    If Caller.Engine.Settings Is Nothing OrElse Not Caller.Engine.Settings.ContainsKey(strName) Then
                                        cValue = 0.0
                                    Else
                                        cValue = Caller.Engine.Settings(strName)
                                    End If
                                    xValue = CType(cValue, Double) + CType(strValue, Double)
                            End Select

                            'ROMAIN: 09/19/07
                            'Dim mc As New DotNetNuke.Entities.Modules.ModuleController
                            'mc.UpdateTabModuleSetting(Caller.Engine.ModuleID, strName, xValue)
                            'AbstractFactory.Instance.EngineController.UpdateModuleSetting(Caller.Engine.ConfigurationID, strName, xValue)
                            AbstractFactory.Instance.EngineController.UpdatePageModuleSetting(Caller.Engine.TabModuleID, strName, xValue)


                            If Not Caller.Engine.Settings Is Nothing Then
                                If Caller.Engine.Settings.ContainsKey(strName) Then
                                    Caller.Engine.Settings(strName) = xValue
                                    If Not Debugger Is Nothing Then
                                        r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Assignment: Tab Module Setting[" & strName & "] = '" & strValue & "'", True)
                                    End If
                                Else
                                    Caller.Engine.Settings.Add(strName, xValue)
                                    If Not Debugger Is Nothing Then
                                        r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Assignment: Adding Tab Module Setting[" & strName & "] = '" & strValue & "'", True)
                                    End If
                                End If
                            End If
                        Catch ex As Exception
                        End Try
                    Case "<UserInfo>"
                        'VERSION: 1.7.9 Added User Info Assignment Capabilities
                        Dim strErrorPosition As String = ""
                        Try
                            Dim UserObject As IUser
                            Dim UserControl As IUserController = AbstractFactory.Instance.UserController

                            If strName Is Nothing OrElse strName.Length = 0 Then
                                'LOAD OR SAVE THE USER INFORMATION
                                If strValue.ToUpper.StartsWith("LOGIN") Then
                                    Dim strparams() As String = strValue.Split(",")
                                    Dim result As Integer = -3
                                    Dim resultstr As String = Nothing

                                    UserObject = Caller.Engine.ActionVariable("~!UserInformation!~")

                                    If strparams.Length >= 3 Then
                                        If Not UserObject Is Nothing Then
                                            Dim ipaddress As String = Nothing
                                            If Not Caller.Engine.Request.UserHostAddress Is Nothing Then
                                                ipaddress = Caller.Engine.Request.UserHostAddress
                                            End If
                                            Dim bCreatePersistentCookie As Boolean = False
                                            Dim strPasswordVariable As String = Caller.Engine.ActionVariable(strparams(1))
                                            'Dim strPasswordResult As String = Nothing
                                            Dim SecurityControl As ISecurityController = AbstractFactory.Instance.SecurityController
                                            If strparams.Length > 3 Then
                                                If strparams(3).ToUpper = "TRUE" Then
                                                    bCreatePersistentCookie = True
                                                End If
                                            End If
                                            Try
                                                'result = objSecurity.UserLogin(UserObject.Username, strPasswordVariable, Caller.Engine.PortalID, Caller.Engine.PortalSettings.PortalName, ipaddress, bCreatePersistentCookie)
                                                result = SecurityControl.UserLogin(UserObject.UserName, strPasswordVariable, Caller.Engine.PortalID, Caller.Engine.PortalSettings.PortalName, ipaddress, bCreatePersistentCookie)
                                            Catch ex As Exception
                                                result = -3
                                            End Try
                                            If result = -1 Then
                                                'ROMAIN: 09/19/07
                                                'FAILURE
                                                'If UserObject.Membership.LockedOut Then
                                                '    result = 0
                                                'ElseIf UserObject.Membership.Approved = False Then
                                                '    result = -1
                                                'Else
                                                '    result = -2 'FAILED PASSWORD
                                                'End If
                                                If UserObject.LockedOut Then
                                                    result = 0
                                                ElseIf UserObject.Approved = False Then
                                                    result = -1
                                                Else
                                                    result = -2 'FAILED PASSWORD
                                                End If
                                            Else
                                                '                                            CType(Page, PageBase).SetLanguage(objUserInfo.Profile.PreferredLocale)
                                                UserObject = UserControl.GetUser(Caller.Engine.PortalSettings.PortalId, result)
                                                Caller.Engine.ActionVariable("~!UserInformation!~") = UserObject
                                                Caller.Engine.CurrentUser = UserObject
                                                Caller.Engine.UserID = UserObject.UserId
                                                result = 1
                                            End If
                                        Else
                                            result = -3
                                        End If
                                        Select Case result
                                            Case -3
                                                resultstr = "Unknown User"
                                            Case -2
                                                resultstr = "Bad Password"
                                            Case -1
                                                resultstr = "User Not Approved"
                                            Case 0
                                                resultstr = "User Locked Out"
                                            Case 1
                                                resultstr = ""
                                        End Select
                                        Caller.Engine.ActionVariable(strparams(2)) = result
                                        Caller.Engine.ActionVariable(strparams(2) & ".Error") = resultstr
                                    End If
                                ElseIf strValue.ToUpper.StartsWith("LOGOFF") Then
                                    Dim SecurityControl As ISecurityController = AbstractFactory.Instance.SecurityController
                                    SecurityControl.UserLogoff()
                                    Caller.Engine.CurrentUser = Nothing
                                    Caller.Engine.UserID = "-1"
                                    Caller.Engine.UserInfo = Nothing
                                ElseIf strValue.ToUpper.StartsWith("AUTHENTICATE") Then
                                    'FORCE THE LOGIN PROCESS AS DNN HANDLES IT.
                                    'Version: 1.9.8 - Authentication and Login for User via Message Assignment. Result -3 (Not Loaded), -2 (Unknown), -1 (Unapproved), 0 (Locked Out), 1 (Success)
                                    UserObject = Caller.Engine.ActionVariable("~!UserInformation!~")
                                    Dim result As Integer = -3
                                    Dim resultstr As String = Nothing
                                    If Not UserObject Is Nothing Then
                                        Dim bCreatePersistent As Boolean = False
                                        If strValue.ToUpper.StartsWith("AUTHENTICATE,TRUE,") Then
                                            bCreatePersistent = True
                                        End If
                                        Dim SecurityControl As ISecurityController = AbstractFactory.Instance.SecurityController
                                        result = SecurityControl.UserAuthenticate(UserObject, bCreatePersistent, Caller.Engine.PortalSettings, Caller.Engine.Context)
                                    End If
                                    Dim variabletargetname As String = "~!UserInformation!~"
                                    If strValue.ToUpper.StartsWith("AUTHENTICATE,TRUE,") Then
                                        variabletargetname = strValue.Substring(18)
                                    ElseIf strValue.ToUpper.StartsWith("AUTHENTICATE,") Then
                                        variabletargetname = strValue.Substring(13)
                                    End If
                                    Select Case result
                                        Case -3
                                            resultstr = "No User Loaded"
                                        Case -2
                                            resultstr = "User Unknown"
                                        Case -1
                                            resultstr = "User Not Approved"
                                        Case 0
                                            resultstr = "User Locked Out"
                                        Case 1
                                            Caller.Engine.CurrentUser = UserObject
                                            Caller.Engine.UserID = UserObject.UserId
                                            resultstr = ""
                                    End Select
                                    Caller.Engine.ActionVariable(variabletargetname) = result
                                    Caller.Engine.ActionVariable(variabletargetname & ".Error") = resultstr
                                ElseIf strValue.ToUpper.StartsWith("SAVE") Then
                                    strErrorPosition = "Save User"
                                    Dim variabletargetname As String = "~!UserInformation!~"
                                    If strValue.ToUpper.StartsWith("SAVE,") Then
                                        variabletargetname = strValue.Substring(5)
                                    End If
                                    'ROMAIN: 09/19/07
                                    'UserControl = New DotNetNuke.Entities.Users.UserController
                                    UserObject = Caller.Engine.ActionVariable("~!UserInformation!~")
                                    If Not UserObject Is Nothing Then
                                        'ROMAIN: 09/19/07
                                        'If UserObject.UserID > 0 Then
                                        If UserObject.Id > 0 Then
                                            If Not Debugger Is Nothing Then
                                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Updating User", False)
                                            End If
                                            'ROMAIN: 09/19/07
                                            'UserControl.UpdateUser(UserObject)

                                            UserControl.SaveUser(UserObject)
                                            If UserObject.Id = Caller.Engine.UserID Then
                                                If UserObject.Id <> "-1" Then
                                                    Caller.Engine.UserInfo = UserObject
                                                    Caller.Engine.UserID = UserObject.UserId
                                                End If
                                            End If
                                            Caller.Engine.ActionVariable("~!UserInformation!~") = ""
                                            Dim strPassValue As String = Caller.Engine.ActionVariable("~!UserInformation-Password!~")
                                            If Not strPassValue Is Nothing AndAlso strPassValue.Length > 0 Then
                                                Caller.Engine.ActionVariable("~!UserInformation-Password!~") = ""
                                                Try
                                                    UserControl.SetPassword(UserObject, strPassValue)
                                                Catch ex As Exception
                                                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Password assignment failed: " & ex.ToString, False)
                                                    Caller.Engine.ActionVariable(variabletargetname & ".Error") = "Password is not long enough for updating this user."
                                                End Try
                                            End If
                                        Else
                                            If Not Debugger Is Nothing Then
                                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Creating User", False)
                                            End If
                                            Dim newuserid As Integer = UserControl.AddUser(UserObject, True)
                                            Caller.Engine.ActionVariable("~!UserInformation!~") = ""
                                            If newuserid > 0 Then
                                                Dim strPassValue As String = Caller.Engine.ActionVariable("~!UserInformation-Password!~")
                                                If Not strPassValue Is Nothing AndAlso strPassValue.Length > 0 Then
                                                    Caller.Engine.ActionVariable("~!UserInformation-Password!~") = ""
                                                    'UserObject.UserID = newuserid
                                                    'ROMAIN: 09/19/07
                                                    UserObject.Id = newuserid
                                                    Try
                                                        UserControl.SetPassword(UserObject, strPassValue)
                                                    Catch ex As Exception
                                                        r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Password assignment failed: " & ex.ToString, False)
                                                        Caller.Engine.ActionVariable(variabletargetname & ".Error") = "Password is not long enough for creating this user."
                                                    End Try
                                                End If
                                            End If
                                            Caller.Engine.ActionVariable("~!UserInformation-Password!~") = ""
                                            Caller.Engine.ActionVariable(variabletargetname) = newuserid
                                            If newuserid < 0 Then
                                                'Dim UserRegistrationStatus As Microsoft.ScalableHosting.Security.MembershipCreateStatus
                                                'UserRegistrationStatus = CType(newuserid * -1, Microsoft.ScalableHosting.Security.MembershipCreateStatus)
                                                Caller.Engine.ActionVariable(variabletargetname & ".Error") = "Username already exists, or password is not long enough for creating a new user."
                                            End If
                                            If Not Debugger Is Nothing Then
                                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Created User " & newuserid.ToString, False)
                                            End If
                                        End If
                                    Else
                                        Throw New Exception("UserInfo was never created, or loaded. To create a User, set the Name to an empty value, and the value to -1.")
                                    End If
                                Else
                                    If IsNumeric(strValue) Then
                                        strErrorPosition = "Load User"
                                        'LOAD THE USER
                                        Dim targetUserID As Integer = CType(strValue, Integer)
                                        If targetUserID > 0 Then
                                            'ROMAIN: 09/19/07
                                            'UserControl = New DotNetNuke.Entities.Users.UserController
                                            If Not Debugger Is Nothing Then
                                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Loading User", False)
                                            End If
                                            UserObject = UserControl.GetUser(Caller.Engine.PortalSettings.PortalId, targetUserID)

                                            Caller.Engine.ActionVariable("~!UserInformation!~") = UserObject
                                        ElseIf targetUserID = -1 Then
                                            strErrorPosition = "Create User"
                                            'ROMAIN: 09/19/07
                                            UserObject = UserControl.NewUser(Caller.Engine.PortalSettings.PortalId)
                                            If Not Debugger Is Nothing Then
                                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Initiating New User", False)
                                            End If

                                            Caller.Engine.ActionVariable("~!UserInformation!~") = UserObject
                                        End If
                                    Else
                                        'TRY LOADING BY USERNAME
                                        strErrorPosition = "Load User"
                                        'LOAD THE USER
                                        'ROMAIN: 09/19/07
                                        'UserControl = New DotNetNuke.Entities.Users.UserController
                                        If Not Debugger Is Nothing Then
                                            r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Loading User", False)
                                        End If
                                        UserObject = UserControl.GetUserByUsername(Caller.Engine.PortalSettings.PortalId, strValue)
                                        Caller.Engine.ActionVariable("~!UserInformation!~") = UserObject
                                    End If
                                End If
                            Else
                                'ASSIGN AS SPECIFIC PROFILE/USER VALUE
                                strErrorPosition = "Profile Assignment"
                                'ROMAIN: 09/19/07
                                'UserControl = New DotNetNuke.Entities.Users.UserController
                                UserObject = Caller.Engine.ActionVariable("~!UserInformation!~")
                                If strName.ToUpper = "PASSWORD" Then
                                    Caller.Engine.ActionVariable("~!UserInformation-Password!~") = strValue
                                End If
                                If Not UserObject Is Nothing Then
                                    'USERINFO
                                    strErrorPosition = "UserInfo Assignment"
                                    'ROLES
                                    strErrorPosition = "User Role Assignment"
                                    If (strName.ToUpper = "ROLEID" OrElse strName.ToUpper = "ROLE") AndAlso strValue.Length > 0 Then
                                        strErrorPosition = "Role Assignment"
                                        'ROMAIN: 09/19/07
                                        'Dim roleInfo As DotNetNuke.Security.Roles.RoleInfo
                                        'Dim roleControl As New DotNetNuke.Security.Roles.RoleController
                                        Dim roleInfo As IRole = Nothing
                                        Dim roleControl As IRoleController = AbstractFactory.Instance.RoleController
                                        Dim isRemoved As Boolean = False
                                        If strValue.StartsWith("-") Then
                                            isRemoved = True
                                            strValue = strValue.Substring(1)
                                        End If
                                        If IsNumeric(strValue) Then
                                            'WE HAVE THE ROLEID
                                            Try
                                                roleInfo = roleControl.GetRole(CInt(strValue), Caller.Engine.PortalID)
                                            Catch ex As Exception
                                                If Not Debugger Is Nothing Then
                                                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Could not identify role for " & strValue & ": " & strErrorPosition & " (" & strName & ")", True)
                                                End If
                                            End Try
                                        Else
                                            'WE HAVE A ROLE NAME
                                            Try
                                                roleInfo = roleControl.GetRoleByName(Caller.Engine.PortalID, strValue)
                                            Catch ex As Exception
                                                If Not Debugger Is Nothing Then
                                                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Could not identify role for " & strValue & ": " & strErrorPosition & " (" & strName & ")", True)
                                                End If
                                            End Try
                                        End If
                                        If Not roleInfo Is Nothing Then
                                            Try
                                                'VERSION: 1.9.8 - User Role - UserID must be greater than -1 so therefore, save the user before attempting this.
                                                'If UserObject.UserID > -1 Then
                                                'ROMAIN: 09/19/07
                                                If UserObject.Id > -1 Then
                                                    If isRemoved Then
                                                        'roleControl.DeleteUserRole(Caller.Engine.PortalID, UserObject.UserID, roleInfo.RoleID)
                                                        roleControl.DeleteUserRole(Caller.Engine.PortalID, UserObject.Id, roleInfo.Id)
                                                        If Not Debugger Is Nothing Then
                                                            r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Removed Role " & strValue & " for User.", True)
                                                        End If
                                                    Else
                                                        'roleControl.AddUserRole(Caller.Engine.PortalID, UserObject.UserID, roleInfo.RoleID, Nothing)
                                                        roleControl.AddUserRole(Caller.Engine.PortalID, UserObject.Id, roleInfo.Id, Nothing)
                                                        If Not Debugger Is Nothing Then
                                                            r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Added Role " & strValue & " for User.", True)
                                                        End If
                                                    End If
                                                Else
                                                    If Not Debugger Is Nothing Then
                                                        r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Could not save role for " & strValue & " because UserID is -1, save user before attempting Role assignment: " & strErrorPosition & " (" & strName & ")", True)
                                                    End If
                                                End If
                                            Catch ex As Exception

                                            End Try
                                        End If
                                        roleControl = Nothing
                                    Else
                                        'Dim prop As System.Reflection.PropertyInfo
                                        'TODO: Change UserInfo Assignment
                                        'prop = UserObject.GetType.GetProperty(strName)
                                        UserObject.Property(strName) = strValue
                                    End If
                                End If
                            End If
                        Catch ex As Exception
                            If Not Debugger Is Nothing Then
                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Unabled to assign User Info Property (" & strName & ") " & strErrorPosition & ":" & ex.ToString, True)
                            End If
                        End Try
                    Case "<Configuration>"
                        'VERSION: 1.9.7 Added Configuration Assignment Capabilities
                        Dim strErrorPosition As String = ""
                        Try
                            Dim cwriter As Actions.Utilities.ConfigurationWriter
                            If Not strName Is Nothing AndAlso strName.Length > 0 Then
                                Select Case strName.ToUpper
                                    Case "NEW"
                                        If strValue.ToUpper = "CURRENT" Then
                                            'MODIFY THE CURRENT CONFIG
                                            cwriter = New Actions.Utilities.ConfigurationWriter(Caller.Engine.xls)
                                        Else
                                            If strValue.Length = 0 Then
                                                'CREATE A NEW CONFIG
                                                cwriter = New Actions.Utilities.ConfigurationWriter
                                            Else
                                                'Dim lstrs As String() = strValue.Split("|")
                                                ' '' REG - Change to ConfigurationID
                                                'If lstrs.Length = 2 AndAlso IsNumeric(lstrs(0)) AndAlso IsNumeric(lstrs(1)) Then
                                                '    'LOAD CONFIG
                                                '    Dim cloader As New Settings
                                                '    'cwriter = New ConfigurationWriter(cloader.GetSetting(CInt(lstrs(0)), CInt(lstrs(1))))
                                                '    cwriter = New ConfigurationWriter(cloader.GetSetting(lstrs(0)) & ":" & lstrs(1))
                                                'ElseIf lstrs.Length = 1 Then
                                                '    Dim cloader As New Settings
                                                '    cwriter = New ConfigurationWriter(cloader.GetSetting(lstrs(0)))
                                                'End If
                                                Dim cloader As New Controller
                                                cwriter = New Actions.Utilities.ConfigurationWriter(cloader.GetSetting(Caller.Engine.ConfigurationID))
                                            End If
                                        End If
                                        If Not cwriter Is Nothing Then
                                            Caller.Engine.ActionVariable("~!Configuration!~") = cwriter
                                        End If
                                    Case "SAVE"
                                        cwriter = Caller.Engine.ActionVariable("~!Configuration!~")
                                        If Not cwriter Is Nothing Then
                                            If strValue.ToUpper = "CURRENT" Then
                                                Caller.Engine.xls = cwriter.Configuration
                                            Else
                                                'Dim lstrs As String() = strValue.Split("|")
                                                ' '' REG - Change to ConfigurationID
                                                'If lstrs.Length = 2 AndAlso IsNumeric(lstrs(0)) AndAlso IsNumeric(lstrs(1)) Then
                                                '    'LOAD CONFIG
                                                '    Dim cloader As New Settings
                                                '    cloader.UpdateSetting(lstrs(0), lstrs(1), cwriter.Configuration.Serialize)
                                                'ElseIf lstrs.Length = 1 Then
                                                '    Dim cloader As New Settings
                                                '    cloader.UpdateSetting(lstrs(0), cwriter.Configuration.Serialize)
                                                'End If
                                                Dim cloader As New Controller
                                                cloader.UpdateSetting(Caller.Engine.ConfigurationID, cwriter.Configuration.Serialize, Caller.Engine.UserInfo.UserName)
                                                'cloader.UpdateSetting(Caller.Engine.ConfigurationID, Caller.Engine.ModuleID, Caller.Engine.TabID, cwriter.Configuration.Serialize)

                                            End If
                                        End If
                                        Caller.Engine.ActionVariable("~!Configuration!~") = Nothing
                                    Case Else
                                        cwriter = Caller.Engine.ActionVariable("~!Configuration!~")
                                        If Not cwriter Is Nothing Then
                                            cwriter.Assign(strName, strValue)
                                        End If
                                End Select
                            End If
                        Catch ex As Exception
                            If Not Debugger Is Nothing Then
                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Unabled to assign User Info Property (" & strName & ") " & strErrorPosition & ":" & ex.ToString, True)
                            End If
                        End Try

                    Case "PAGESIZE"
                        Try
                            Dim strResult As String = strValue
                            Dim cValue As Object = Nothing
                            Dim xValue As Object = Nothing
                            Select Case iAssignmentType
                                Case 0
                                    xValue = strValue
                                Case 1
                                    cValue = Caller.Engine.RecordsPerPage
                                    xValue = cValue & strValue
                                Case 3
                                    cValue = Caller.Engine.RecordsPerPage
                                    xValue = strValue & cValue
                                Case 2
                                    cValue = Caller.Engine.RecordsPerPage
                                    xValue = CType(cValue, Double) + CType(strValue, Double)
                            End Select

                            If IsNumeric(xValue) Then
                                Caller.Engine.RecordsPerPage = xValue
                                If Not Debugger Is Nothing Then
                                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Assignment: PageSize = '" & xValue & "'", True)
                                End If
                            Else
                                If Not Debugger Is Nothing Then
                                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Failed Assignment: PageSize = '" & xValue & "' not numeric.", True)
                                End If
                            End If
                        Catch ex As Exception
                        End Try
                    Case "PAGENUMBER"
                        Try
                            Dim strResult As String = strValue

                            Dim cValue As Object = Nothing
                            Dim xValue As Object = Nothing
                            Select Case iAssignmentType
                                Case 0
                                    xValue = strValue
                                Case 1
                                    cValue = Caller.Engine.PageCurrent 'Caller.Engine.ViewState("CurrentPage" & Caller.Engine.TabModuleID.ToString)
                                    xValue = cValue & strValue
                                Case 3
                                    cValue = Caller.Engine.PageCurrent 'Caller.Engine.ViewState("CurrentPage" & Caller.Engine.TabModuleID.ToString)
                                    xValue = strValue & cValue
                                Case 2
                                    cValue = Caller.Engine.PageCurrent 'Caller.Engine.ViewState("CurrentPage" & Caller.Engine.TabModuleID.ToString)
                                    xValue = CType(cValue, Double) + CType(strValue, Double)
                            End Select

                            If IsNumeric(xValue) Then
                                strResult = CInt(strResult) - 1 'We're now 0-based
                                'Caller.Engine.ViewState("CurrentPage" & Caller.Engine.TabModuleID.ToString) = xValue
                                Caller.Engine.PageCurrent = xValue
                                If Not Debugger Is Nothing Then
                                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Assignment: PageNumber = '" & xValue & "'", True)
                                End If
                            Else
                                If Not Debugger Is Nothing Then
                                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Failed Assignment: PageNumber = '" & xValue & "' not numeric.", True)
                                End If
                            End If
                        Catch ex As Exception
                        End Try
                    Case "FILTER"
                        Try
                            Dim strResult As String = strValue

                            Dim cValue As Object = Nothing
                            Dim xValue As Object = Nothing
                            Select Case iAssignmentType
                                Case 0
                                    xValue = strValue
                                Case 1
                                    cValue = Caller.Engine.Session(Caller.Engine.ModuleID.ToString() & Caller.Engine.UserID.ToString() & "FILTERTEXT")
                                    xValue = cValue & strValue
                                Case 3
                                    cValue = Caller.Engine.Session(Caller.Engine.ModuleID.ToString() & Caller.Engine.UserID.ToString() & "FILTERTEXT")
                                    xValue = strValue & cValue
                                Case 2
                                    cValue = Caller.Engine.Session(Caller.Engine.ModuleID.ToString() & Caller.Engine.UserID.ToString() & "FILTERTEXT")
                                    xValue = CType(cValue, Double) + CType(strValue, Double)
                            End Select

                            Caller.Engine.Session(Caller.Engine.ModuleID.ToString() & Caller.Engine.UserID.ToString() & "FILTERTEXT") = xValue
                            If Not Debugger Is Nothing Then
                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Assignment: Filter = '" & xValue & "'", True)
                            End If
                        Catch ex As Exception
                        End Try
                    Case "SEARCH"
                        Try
                            Dim strResult As String = strValue

                            Dim cValue As Object = Nothing
                            Dim xValue As Object = Nothing
                            Select Case iAssignmentType
                                Case 0
                                    xValue = strValue
                                Case 1
                                    cValue = Caller.Engine.Session(Caller.Engine.ModuleID.ToString() & Caller.Engine.UserID.ToString() & "FILTERFIELD")
                                    xValue = cValue & strValue
                                Case 3
                                    cValue = Caller.Engine.Session(Caller.Engine.ModuleID.ToString() & Caller.Engine.UserID.ToString() & "FILTERFIELD")
                                    xValue = strValue & cValue
                                Case 2
                                    cValue = Caller.Engine.Session(Caller.Engine.ModuleID.ToString() & Caller.Engine.UserID.ToString() & "FILTERFIELD")
                                    xValue = CType(cValue, Double) + CType(strValue, Double)
                            End Select

                            Caller.Engine.Session(Caller.Engine.ModuleID.ToString() & Caller.Engine.UserID.ToString() & "FILTERFIELD") = xValue
                            If Not Debugger Is Nothing Then
                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Assignment: Search = '" & xValue & "'", True)
                            End If
                        Catch ex As Exception
                        End Try
                    Case "SORT"
                        Try
                            Dim strResult As String = strValue
                            Caller.Engine.Session("OWS.sort" & Caller.Engine.ModuleID.ToString() & Caller.Engine.UserID.ToString()) = strResult
                            If Not Debugger Is Nothing Then
                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Assignment: Sort = '" & strResult & "'", True)
                            End If
                        Catch ex As Exception
                        End Try
                    Case "<System>"
                        Dim strResult As String = strValue
                        If strName.ToLower.StartsWith("configuration.") Then
                            strName = strName.Substring(14, strName.Length - 14)
                            Select Case strName.ToUpper
                                Case "AUTOREFRESHINTERVAL"
                                    Caller.Engine.xls.autoRefreshInterval = strValue
                                Case "ENABLEAJAX"
                                    If strValue.ToUpper.StartsWith("T") OrElse strValue = "1" Then
                                        Caller.Engine.xls.enableAJAX = True
                                    Else
                                        Caller.Engine.xls.enableAJAX = False
                                    End If
                                Case "ENABLEAJAXCUSTOMPAGING"
                                    If strValue.ToUpper.StartsWith("T") OrElse strValue = "1" Then
                                        Caller.Engine.xls.enableAJAXCustomPaging = True
                                    Else
                                        Caller.Engine.xls.enableAJAXCustomPaging = False
                                    End If
                                Case "ENABLEAJAXCUSTOMSTATUS"
                                    If strValue.ToUpper.StartsWith("T") OrElse strValue = "1" Then
                                        Caller.Engine.xls.enableAJAXCustomStatus = True
                                    Else
                                        Caller.Engine.xls.enableAJAXCustomStatus = False
                                    End If
                                Case "ENABLEAJAXMANUAL"
                                    If strValue.ToUpper.StartsWith("T") OrElse strValue = "1" Then
                                        Caller.Engine.xls.enableAJAXManual = True
                                    Else
                                        Caller.Engine.xls.enableAJAXManual = False
                                    End If
                                Case "ENABLEAJAXPAGEHISTORY"
                                    If strValue.ToUpper.StartsWith("T") OrElse strValue = "1" Then
                                        Caller.Engine.xls.enableAJAXPageHistory = True
                                    Else
                                        Caller.Engine.xls.enableAJAXPageHistory = False
                                    End If
                                Case "ENABLEAJAXPAGING"
                                    If strValue.ToUpper.StartsWith("T") OrElse strValue = "1" Then
                                        Caller.Engine.xls.enableAJAXPaging = True
                                    Else
                                        Caller.Engine.xls.enableAJAXPaging = False
                                    End If
                                Case "NOOWSCREATE"
                                    If strValue.ToUpper.StartsWith("T") OrElse strValue = "1" Then
                                        Caller.Engine.xls.noOWSCreate = True
                                    Else
                                        Caller.Engine.xls.noOWSCreate = False
                                    End If
                                Case "ENABLEALPHAFILTER"
                                    If strValue.ToUpper.StartsWith("T") OrElse strValue = "1" Then
                                        Caller.Engine.xls.enableAlphaFilter = True
                                    Else
                                        Caller.Engine.xls.enableAlphaFilter = False
                                    End If
                                Case "ENABLECUSTOMPAGING"
                                    If strValue.ToUpper.StartsWith("T") OrElse strValue = "1" Then
                                        Caller.Engine.xls.enableCustomPaging = True
                                    Else
                                        Caller.Engine.xls.enableCustomPaging = False
                                    End If
                                Case "ENABLEFORCEDQUERYSPLIT"
                                    If strValue.ToUpper.StartsWith("T") OrElse strValue = "1" Then
                                        Caller.Engine.xls.enabledForcedQuerySplit = True
                                    Else
                                        Caller.Engine.xls.enabledForcedQuerySplit = False
                                    End If
                                Case "RECORDSPERPAGE"
                                    If IsNumeric(strValue) Then
                                        Caller.Engine.xls.recordsPerPage = CInt(strValue)
                                    End If
                                Case "SHOWALL"
                                    If strValue.ToUpper.StartsWith("T") OrElse strValue = "1" Then
                                        Caller.Engine.xls.showAll = True
                                    Else
                                        Caller.Engine.xls.showAll = False
                                    End If
                                Case "OVERRIDEPAGING"
                                    If strValue.ToUpper.StartsWith("T") OrElse strValue = "1" Then
                                        Caller.Engine.OverridePaging = True
                                    Else
                                        Caller.Engine.OverridePaging = False
                                    End If
                                Case "PAGE", "CURRENTPAGE", "PAGENUMBER"
                                    Try
                                        If IsNumeric(strValue) Then
                                            strValue = CInt(strResult) - 1 'We're now 0-based
                                            Caller.Engine.PageCurrent = strValue
                                            Caller.Engine.OverridePaging = True
                                            If Not Debugger Is Nothing Then
                                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Assignment: PageNumber = '" & strValue & "'", True)
                                            End If
                                        Else
                                            If Not Debugger Is Nothing Then
                                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Failed Assignment: PageNumber = '" & strValue & "' not numeric.", True)
                                            End If
                                        End If
                                    Catch ex As Exception
                                    End Try
                                Case "ENABLEHIDEONNOQUERY"
                                    If strValue.ToUpper.StartsWith("T") OrElse strValue = "1" Then
                                        Caller.Engine.xls.enableHide_OnNoQuery = True
                                    Else
                                        Caller.Engine.xls.enableHide_OnNoQuery = False
                                    End If
                                Case "ENABLEHIDEONNORESULTS"
                                    If strValue.ToUpper.StartsWith("T") OrElse strValue = "1" Then
                                        Caller.Engine.xls.enableHide_OnNoResults = True
                                    Else
                                        Caller.Engine.xls.enableHide_OnNoResults = False
                                    End If
                                Case "ENABLEMULTIPLECOLUMNSORTING"
                                    If strValue.ToUpper.StartsWith("T") OrElse strValue = "1" Then
                                        Caller.Engine.xls.enableMultipleColumnSorting = True
                                    Else
                                        Caller.Engine.xls.enableMultipleColumnSorting = False
                                    End If
                                Case "ENABLEPAGESELECTION"
                                    If strValue.ToUpper.StartsWith("T") OrElse strValue = "1" Then
                                        Caller.Engine.xls.enablePageSelection = True
                                    Else
                                        Caller.Engine.xls.enablePageSelection = False
                                    End If
                                Case "ENABLEQUERYDEBUG"
                                    If strValue.ToUpper.StartsWith("T") OrElse strValue = "1" Then
                                        Caller.Engine.xls.enableQueryDebug = True
                                        Caller.Engine.xls.enableQueryDebug_Admin = True
                                        Caller.Engine.xls.enableQueryDebug_Edit = True
                                        Caller.Engine.xls.enableQueryDebug_ErrorLog = True
                                        Caller.Engine.xls.enableQueryDebug_Log = True
                                        Caller.Engine.xls.enableQueryDebug_Super = True
                                        If Debugger Is Nothing Then
                                            Debugger = New OWS.Framework.Debugger()
                                        End If
                                    Else
                                        Caller.Engine.xls.enableQueryDebug = False
                                        Caller.Engine.xls.enableQueryDebug_Admin = False
                                        Caller.Engine.xls.enableQueryDebug_Edit = False
                                        Caller.Engine.xls.enableQueryDebug_ErrorLog = False
                                        Caller.Engine.xls.enableQueryDebug_Log = False
                                        Caller.Engine.xls.enableQueryDebug_Super = False
                                        If Not Debugger Is Nothing Then
                                            Debugger = Nothing
                                        End If
                                    End If
                                Case "ENABLEQUERYDEBUGADMIN"
                                    If strValue.ToUpper.StartsWith("T") OrElse strValue = "1" Then
                                        Caller.Engine.xls.enableQueryDebug_Admin = True
                                        If Debugger Is Nothing AndAlso Caller.Engine.xls.canDebug(Caller.Engine.UserInfo, Caller.Engine.PortalSettings, Caller.Engine.ModuleIsEditable) Then
                                            Debugger = New OWS.Framework.Debugger()
                                        End If
                                    Else
                                        Caller.Engine.xls.enableQueryDebug_Admin = False
                                        If Not Debugger Is Nothing AndAlso Not Caller.Engine.xls.canDebug(Caller.Engine.UserInfo, Caller.Engine.PortalSettings, Caller.Engine.ModuleIsEditable) Then
                                            Debugger = Nothing
                                        End If
                                    End If
                                Case "ENABLEQUERYDEBUGEDIT"
                                    If strValue.ToUpper.StartsWith("T") OrElse strValue = "1" Then
                                        Caller.Engine.xls.enableQueryDebug_Edit = True
                                        If Debugger Is Nothing AndAlso Caller.Engine.xls.canDebug(Caller.Engine.UserInfo, Caller.Engine.PortalSettings, Caller.Engine.ModuleIsEditable) Then
                                            Debugger = New OWS.Framework.Debugger()
                                        End If

                                    Else
                                        Caller.Engine.xls.enableQueryDebug_Edit = False
                                        If Not Debugger Is Nothing AndAlso Not Caller.Engine.xls.canDebug(Caller.Engine.UserInfo, Caller.Engine.PortalSettings, Caller.Engine.ModuleIsEditable) Then
                                            Debugger = Nothing
                                        End If

                                    End If
                                Case "ENABLEQUERYDEBUGERRORLOG"
                                    If strValue.ToUpper.StartsWith("T") OrElse strValue = "1" Then
                                        Caller.Engine.xls.enableQueryDebug_ErrorLog = True
                                        If Debugger Is Nothing AndAlso Caller.Engine.xls.canDebug(Caller.Engine.UserInfo, Caller.Engine.PortalSettings, Caller.Engine.ModuleIsEditable) Then
                                            Debugger = New OWS.Framework.Debugger()
                                        End If

                                    Else
                                        Caller.Engine.xls.enableQueryDebug_ErrorLog = False
                                        If Not Debugger Is Nothing AndAlso Not Caller.Engine.xls.canDebug(Caller.Engine.UserInfo, Caller.Engine.PortalSettings, Caller.Engine.ModuleIsEditable) Then
                                            Debugger = Nothing
                                        End If

                                    End If
                                Case "ENABLEQUERYDEBUGLOG"
                                    If strValue.ToUpper.StartsWith("T") OrElse strValue = "1" Then
                                        Caller.Engine.xls.enableQueryDebug_Log = True
                                        If Debugger Is Nothing AndAlso Caller.Engine.xls.canDebug(Caller.Engine.UserInfo, Caller.Engine.PortalSettings, Caller.Engine.ModuleIsEditable) Then
                                            Debugger = New OWS.Framework.Debugger()
                                        End If

                                    Else
                                        Caller.Engine.xls.enableQueryDebug_Log = False
                                        If Not Debugger Is Nothing AndAlso Not Caller.Engine.xls.canDebug(Caller.Engine.UserInfo, Caller.Engine.PortalSettings, Caller.Engine.ModuleIsEditable) Then
                                            Debugger = Nothing
                                        End If

                                    End If
                                Case "ENABLEQUERYDEBUGSUPER"
                                    If strValue.ToUpper.StartsWith("T") OrElse strValue = "1" Then
                                        Caller.Engine.xls.enableQueryDebug_Super = True
                                        If Debugger Is Nothing AndAlso Caller.Engine.xls.canDebug(Caller.Engine.UserInfo, Caller.Engine.PortalSettings, Caller.Engine.ModuleIsEditable) Then
                                            Debugger = New OWS.Framework.Debugger()
                                        End If

                                    Else
                                        Caller.Engine.xls.enableQueryDebug_Super = False
                                        If Not Debugger Is Nothing AndAlso Not Caller.Engine.xls.canDebug(Caller.Engine.UserInfo, Caller.Engine.PortalSettings, Caller.Engine.ModuleIsEditable) Then
                                            Debugger = Nothing
                                        End If

                                    End If
                                Case "ENABLERECORDSPERPAGE"
                                    If strValue.ToUpper.StartsWith("T") OrElse strValue = "1" Then
                                        Caller.Engine.xls.enableRecordsPerPage = True
                                    Else
                                        Caller.Engine.xls.enableRecordsPerPage = False
                                    End If
                                Case "TEMPLATE"
                                    If strValue Is Nothing OrElse strValue.Length <= 0 Then
                                        Caller.Engine.xls.ClearTemplate()
                                    End If
                            End Select
                            If Not Debugger Is Nothing Then
                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Assignment: SYSTEM.CONFIGURATION[" & strName & "] = '" & strResult & "'", True)
                            End If
                        ElseIf strName.ToLower.StartsWith("page.") Then
                            Dim strPageProp As String = strName.Substring(5, strName.Length - 5)
                            Dim coll As String = strPageProp.Substring(0, strPageProp.IndexOf("."))
                            Select Case coll.ToUpper
                                Case "HEAD"
                                    strPageProp = strPageProp.Substring(5, strPageProp.Length - 5)
                                    Dim ipic As IPageInfoController = AbstractFactory.Instance.PageInfoController
                                    Dim pInfo As AbstractPageInfo = ipic.GetPageInfo(Caller.Engine.Caller.Page)
                                    pInfo.SetPageHeader(strValue, strPageProp)
                                Case "META"
                                    strPageProp = strPageProp.Substring(5, strPageProp.Length - 5)
                                    Dim ipic As IPageInfoController = AbstractFactory.Instance.PageInfoController
                                    Dim pInfo As AbstractPageInfo = ipic.GetPageInfo(Caller.Engine.Caller.Page)
                                    Select Case strPageProp.ToUpper
                                        Case "DESCRIPTION"
                                            pInfo.Description = strValue
                                        Case "KEYWORDS"
                                            pInfo.Keywords = strValue
                                        Case "AUTHOR"
                                            pInfo.Author = strValue
                                        Case "COPYRIGHT"
                                            pInfo.Copyright = strValue
                                        Case "GENERATOR"
                                            pInfo.Generator = strValue
                                        Case Else
                                            If strPageProp.StartsWith("{") Then
                                                Dim attr As Specialized.NameValueCollection = r2i.OWS.Framework.Utilities.JSON.JsonConversion.toNameValuePairs(strPageProp)
                                                Dim name As String = Nothing
                                                Try
                                                    Dim k As String = Nothing
                                                    Dim key As String = Nothing
                                                    For Each k In attr.Keys
                                                        If k.ToUpper = "NAME" Then
                                                            name = attr(k)
                                                            key = k
                                                        End If
                                                    Next
                                                    If Not key Is Nothing Then
                                                        attr.Remove(key)
                                                    End If
                                                Catch ex As Exception

                                                End Try
                                                If name Is Nothing Then
                                                    name = ""
                                                End If
                                                pInfo.SetMetaProperty(name, attr, strValue)
                                            Else
                                                pInfo.SetMetaProperty(strPageProp, strValue)
                                            End If
                                    End Select
                                Case "RESPONSE"
                                    strPageProp = strPageProp.Substring(9, strPageProp.Length - 9)
                                    Select Case strPageProp.ToUpper
                                        Case "HEADERS.CLEAR"
                                            If Not strValue Is Nothing AndAlso strValue.ToUpper.StartsWith("T") Then
                                                Caller.Engine.ClearHeaders()
                                            Else
                                                Caller.Engine.ClearResponse(False)
                                            End If
                                        Case "CLEAR"
                                            If Not strValue Is Nothing AndAlso strValue.ToUpper.StartsWith("T") Then
                                                Caller.Engine.ClearResponse(True)
                                            Else
                                                Caller.Engine.ClearResponse(False)
                                            End If
                                        Case "HEADER"
                                            If (strValue.Contains(":")) Then
                                                Dim n As String = strValue.Substring(0, strValue.IndexOf(":"))
                                                Dim v As String = strValue.Substring(strValue.IndexOf(":") + 1)
                                                Caller.Engine.Response.AddHeader(n, v)
                                            End If
                                        Case "FLUSH"
                                            Try
                                                Caller.Engine.Context.Response.Flush()
                                                'Context.Response.Close()
                                            Catch exh As Threading.ThreadAbortException
                                                'Do Nothing - This was anticipated
                                            Catch ex As Exception
                                                'Do Nothing - This was anticipated
                                            End Try
                                        Case "END"
                                            Caller.Engine.EndResponse = True
                                            Try
                                                Caller.Engine.Context.Response.End()
                                            Catch exh As Threading.ThreadAbortException
                                                'Do Nothing - This was anticipated
                                            Catch ex As Exception
                                                'Do Nothing - This was anticipated
                                            End Try
                                        Case ("OUTPUT")
                                            Dim dta() As Byte = System.Text.UTF8Encoding.UTF8.GetBytes(strValue)
                                            Caller.Engine.Response.OutputStream.Write(dta, 0, dta.Length)
                                            'Caller.Engine.Response.Write(strValue)
                                    End Select
                            End Select
                            If Not Debugger Is Nothing Then
                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Assignment: SYSTEM[" & strName & "] = '" & strResult & "'", True)
                            End If
                        End If
                    Case "PAGETITLE"
                        Try
                            Dim strResult As String = strValue
                            'ROMAIN: 09/21/07
                            'Dim p As DotNetNuke.Framework.CDefault = Caller.Engine.Caller.Page
                            'Dim p As ICDefault = CType(pObj, ICDefault)

                            ' do we need to abstract the page object?
                            Dim cValue As Object = Caller.Engine.Caller.Page.Title
                            If cValue Is Nothing Then
                                cValue = ""
                            End If
                            Dim xValue As Object = Nothing
                            Select Case iAssignmentType
                                Case 0
                                    xValue = strValue
                                Case 1
                                    xValue = cValue & strValue
                                Case 3
                                    xValue = strValue & cValue
                                Case 2
                                    xValue = CType(cValue, Double) + CType(strValue, Double)
                            End Select

                            Caller.Engine.Caller.Page.Title = xValue

                            '_Engine.Session("sortActionList" & Caller.Engine.ModuleID.ToString() & Caller.Engine.UserID.ToString()) = strResult
                            If Not Debugger Is Nothing Then
                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Assignment: Page Title = '" & strResult & "'", True)
                            End If
                        Catch ex As Exception
                        End Try
                    Case "STATUSCODE"
                        'VERSION: 2.0 Added StatusCode assignment
                        Try
                            Dim strResult As String = strValue
                            If IsNumeric(strValue) Then
                                Caller.Engine.ClearResponse()
                                Caller.Engine.Response.StatusCode = CInt(strValue)
                            End If
                            If Not Debugger Is Nothing Then
                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Assignment: Response.StatusCode = '" & strResult & "'", True)
                            End If
                        Catch ex As Exception

                        End Try
                    Case "STATUS"
                        'VERSION: 2.0 Added Status assignment
                        Try
                            Dim strResult As String = strValue
                            Caller.Engine.ClearResponse()
                            Caller.Engine.Response.Status = strValue
                            If Not Debugger Is Nothing Then
                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Assignment: Response.Status = '" & strResult & "'", True)
                            End If
                        Catch ex As Exception

                        End Try
                    Case "STATUSDESCRIPTION"
                        'VERSION: 2.0 Added StatusDescription assignment
                        Try
                            Dim strResult As String = strValue
                            Caller.Engine.ClearResponse()
                            Caller.Engine.Response.StatusDescription = strValue
                            If Not Debugger Is Nothing Then
                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Assignment: Response.StatusDescription = '" & strResult & "'", True)
                            End If
                        Catch ex As Exception

                        End Try
                    Case "REDIRECTLOCATION"
                        'VERSION: 2.0 Added StatusLocation assignment
                        Try
                            Dim strResult As String = strValue
                            Caller.Engine.ClearResponse()
                            Caller.Engine.Response.AddHeader("Location", strValue)
                            If Not Debugger Is Nothing Then
                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Assignment: Response.RedirectLocation = '" & strResult & "'", True)
                            End If
                        Catch ex As Exception

                        End Try
                    Case "CONTENTDISPOSITION"
                        'VERSION: 2.0 Added StatusDescription assignment
                        Try
                            Dim strResult As String = strValue
                            Caller.Engine.ClearResponse()
                            Caller.Engine.Response.AddHeader("Content-Disposition", strResult)

                            If Not Debugger Is Nothing Then
                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Assignment: Response.ContentDisposition = '" & strResult & "'", True)
                            End If
                        Catch ex As Exception

                        End Try
                    Case "CONTENTTYPE"
                        'VERSION: 2.0 Added StatusDescription assignment
                        Try
                            Dim strResult As String = strValue
                            Caller.Engine.ClearResponse()
                            Caller.Engine.Response.ContentType = strResult

                            'DUE to an issue with DOTNETNUKE's compression algorithm, we must abort compression
                            'because we are forcing it to abort the compression.

                            If Not Caller.Engine.Context.Items.Contains("httpcompress.attemptedinstall") Then
                                Caller.Engine.Context.Items.Add("httpcompress.attemptedinstall", "true")
                            End If

                            If Not Debugger Is Nothing Then
                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Assignment: Response.ContentType = '" & strResult & "'", True)
                            End If
                        Catch ex As Exception

                        End Try
                    Case "CACHECONTROL"
                        'VERSION: 2.0 Added StatusDescription assignment
                        Try
                            Dim strResult As String = strValue
                            Caller.Engine.ClearResponse()
                            Caller.Engine.Response.CacheControl = strResult

                            If Not Debugger Is Nothing Then
                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Assignment: Response.CacheControl = '" & strResult & "'", True)
                            End If
                        Catch ex As Exception

                        End Try
                    Case "PRAGMA"
                        'VERSION: 2.0 Added StatusDescription assignment
                        Try
                            Dim strResult As String = strValue
                            Caller.Engine.ClearResponse()
                            Caller.Engine.Response.AddHeader("Pragma", strResult)

                            If Not Debugger Is Nothing Then
                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Assignment: Response.Pragma = '" & strResult & "'", True)
                            End If
                        Catch ex As Exception

                        End Try
                    Case "CONTROL.HEADER"
                        Dim cValue As Object = Caller.Engine.xls.Header
                        If cValue Is Nothing Then
                            cValue = ""
                        End If
                        Dim xValue As Object = Nothing
                        Select Case iAssignmentType
                            Case 0
                                xValue = strValue
                            Case 1
                                If strValue Is Nothing Then
                                    cValue = ""
                                End If
                                xValue = cValue & strValue
                            Case 3
                                If strValue Is Nothing Then
                                    cValue = ""
                                End If
                                xValue = strValue & cValue
                            Case 2
                                If strValue Is Nothing Then
                                    cValue = 0.0
                                End If
                                xValue = CType(cValue, Double) + CType(strValue, Double)
                        End Select

                        Caller.Engine.xls.Header = xValue
                    Case "CONTROL.FOOTER"
                        Dim cValue As Object = Caller.Engine.xls.Footer
                        If cValue Is Nothing Then
                            cValue = ""
                        End If
                        Dim xValue As Object = Nothing
                        Select Case iAssignmentType
                            Case 0
                                xValue = strValue
                            Case 1
                                If strValue Is Nothing Then
                                    cValue = ""
                                End If
                                xValue = cValue & strValue
                            Case 3
                                If strValue Is Nothing Then
                                    cValue = ""
                                End If
                                xValue = strValue & cValue
                            Case 2
                                If strValue Is Nothing Then
                                    cValue = 0.0
                                End If
                                xValue = CType(cValue, Double) + CType(strValue, Double)
                        End Select

                        Caller.Engine.xls.Footer = xValue
                    Case "MODULETITLE"

                        Dim cValue As Object = Caller.Engine.xls.Title
                        If cValue Is Nothing Then
                            cValue = ""
                        End If
                        Dim xValue As Object = Nothing
                        Select Case iAssignmentType
                            Case 0
                                xValue = strValue
                            Case 1
                                If strValue Is Nothing Then
                                    cValue = ""
                                End If
                                xValue = cValue & strValue
                            Case 3
                                If strValue Is Nothing Then
                                    cValue = ""
                                End If
                                xValue = strValue & cValue
                            Case 2
                                If strValue Is Nothing Then
                                    cValue = 0.0
                                End If
                                xValue = CType(cValue, Double) + CType(strValue, Double)
                        End Select

                        Caller.Engine.xls.Title = xValue
                End Select
            Catch ex As Exception
                If Not Debugger Is Nothing Then
                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Failed Assignment: " & ex.ToString, True)
                End If
            End Try
            Return New Runtime.ExecutableResult(Runtime.ExecutableResultEnum.Executed, Nothing)
        End Function
    End Class
End Namespace
