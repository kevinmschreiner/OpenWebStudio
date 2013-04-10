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
Imports r2i.OWS.Framework.Entities
Imports r2i.OWS.Framework.DataAccess
Imports r2i.OWS.Framework.Utilities.Compatibility, r2i.OWS.Actions.Utilities

Namespace r2i.OWS.Actions
    Public Class TemplateVariableAction
        Inherits ActionBase


#Region "Debugging and Identification: Name,Style,Description"
        Public Overrides Function Description(ByRef act As MessageActionItem) As String
            Dim str As String = ""
            If Not act.Parameters Is Nothing AndAlso act.Parameters.Count > 0 Then
                Dim type As String
                If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONTEMPLATE_VARIABLE_TYPE_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONTEMPLATE_VARIABLE_TYPE_KEY)).Length > 0 Then
                    type = CStr(act.Parameters(MessageActionsConstants.ACTIONTEMPLATE_VARIABLE_TYPE_KEY)).Replace("&lt;", "<").Replace("&gt;", ">")
                    Dim qvi As New QueryOptionItem
                    qvi.VariableType = type
                    qvi.QueryTarget = CStr(act.Parameters(MessageActionsConstants.ACTIONTEMPLATE_VARIABLE_QUERYTARGET_KEY))
                    qvi.QuerySource = CStr(act.Parameters(MessageActionsConstants.ACTIONTEMPLATE_VARIABLE_QUERYSOURCE_KEY))
                    str &= "<b>" & qvi.QueryTarget & "</b> as " & qvi.VariableType & " " & qvi.QuerySource
                End If
            Else
                str &= " (no parameters defined)"
            End If
            Return str
        End Function
        Public Overrides Function Name() As String
            Return "Template Query Variable"
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
                Dim type As String
                If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONTEMPLATE_VARIABLE_TYPE_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONTEMPLATE_VARIABLE_TYPE_KEY)).Length > 0 Then
                    type = CStr(act.Parameters(MessageActionsConstants.ACTIONTEMPLATE_VARIABLE_TYPE_KEY))
                    Dim qvi As New QueryOptionItem
                    qvi.VariableType = type
                    Try
                        If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONTEMPLATE_VARIABLE_DATATYPE_KEY) Then
                            qvi.VariableDataType = CStr(act.Parameters(MessageActionsConstants.ACTIONTEMPLATE_VARIABLE_DATATYPE_KEY))
                        End If
                    Catch ex As Exception
                    End Try
                    If qvi.VariableDataType Is Nothing Then
                        qvi.VariableDataType = ""
                    End If
                    Try
                        If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONTEMPLATE_VARIABLE_FORMATTERS_KEY) Then
                            qvi.Formatters = CStr(act.Parameters(MessageActionsConstants.ACTIONTEMPLATE_VARIABLE_FORMATTERS_KEY))
                        End If
                    Catch ex As Exception
                    End Try
                    If qvi.Formatters Is Nothing Then
                        qvi.Formatters = ""
                    End If
                    qvi.QueryTargetRight = Caller.Engine.RenderString(sharedds, CStr(act.Parameters(MessageActionsConstants.ACTIONTEMPLATE_VARIABLE_QUERYTARGETRIGHT_KEY)), Caller.Engine.CapturedMessages, False, isPreRender:=False, DebugWriter:=Debugger)
                    'qvi.QueryTargetRight = CStr(act.Parameters(MessageActionsConstants.ACTIONTEMPLATE_VARIABLE_QUERYTARGETRIGHT_KEY))
                    qvi.QueryTargetLeft = Caller.Engine.RenderString(sharedds, CStr(act.Parameters(MessageActionsConstants.ACTIONTEMPLATE_VARIABLE_QUERYTARGETLEFT_KEY)), Caller.Engine.CapturedMessages, False, isPreRender:=False, DebugWriter:=Debugger)
                    'qvi.QueryTargetLeft = CStr(act.Parameters(MessageActionsConstants.ACTIONTEMPLATE_VARIABLE_QUERYTARGETLEFT_KEY))
                    qvi.QueryTargetEmpty = Caller.Engine.RenderString(sharedds, CStr(act.Parameters(MessageActionsConstants.ACTIONTEMPLATE_VARIABLE_QUERYTARGETEMPTY_KEY)), Caller.Engine.CapturedMessages, False, isPreRender:=False, DebugWriter:=Debugger)
                    'qvi.QueryTargetEmpty = CStr(act.Parameters(MessageActionsConstants.ACTIONTEMPLATE_VARIABLE_QUERYTARGETEMPTY_KEY))
                    qvi.QueryTarget = Caller.Engine.RenderString(sharedds, CStr(act.Parameters(MessageActionsConstants.ACTIONTEMPLATE_VARIABLE_QUERYTARGET_KEY)), Caller.Engine.CapturedMessages, False, isPreRender:=False, DebugWriter:=Debugger)
                    'qvi.QueryTarget = CStr(act.Parameters(MessageActionsConstants.ACTIONTEMPLATE_VARIABLE_QUERYTARGET_KEY))
                    qvi.QuerySource = Caller.Engine.RenderString(sharedds, CStr(act.Parameters(MessageActionsConstants.ACTIONTEMPLATE_VARIABLE_QUERYSOURCE_KEY)), Caller.Engine.CapturedMessages, False, isPreRender:=False, DebugWriter:=Debugger)
                    'qvi.QuerySource = CStr(act.Parameters(MessageActionsConstants.ACTIONTEMPLATE_VARIABLE_QUERYSOURCE_KEY))

                    qvi.Protected = CStr(act.Parameters(MessageActionsConstants.ACTIONTEMPLATE_VARIABLE_ESCAPESQL_KEY))
                    qvi.EscapeListX = CStr(act.Parameters(MessageActionsConstants.ACTIONTEMPLATE_VARIABLE_ESCAPECODE_KEY))
                    qvi.EscapeHTML = CStr(act.Parameters(MessageActionsConstants.ACTIONTEMPLATE_VARIABLE_ESCAPEHTML_KEY))

                    If Not Debugger Is Nothing Then
                        r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Assigning Variable: " & qvi.QueryTarget, True)
                    End If
                    Caller.Engine.xls.QueryItem(qvi.QueryTarget) = qvi
                Else
                    'TODO: implement exception

                    If Not Debugger Is Nothing Then
                        r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Assigning Variable: " & "(Unknown)", True)
                    End If
                End If
            End If
            Return New Runtime.ExecutableResult(Runtime.ExecutableResultEnum.Executed, Nothing)
        End Function

        Public Overrides Function Key() As String
            Return "Template-Variable"
        End Function
    End Class
End Namespace