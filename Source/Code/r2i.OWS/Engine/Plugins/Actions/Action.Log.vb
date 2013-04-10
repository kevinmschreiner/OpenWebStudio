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
Imports r2i.OWS.Framework.Utilities
Imports r2i.OWS.Framework.Plugins.Actions
Imports r2i.OWS.Framework.Entities
Imports r2i.OWS.Framework.DataAccess
Imports r2i.OWS.Framework.Utilities.Compatibility
Imports r2i.OWS.Actions.Utilities

Namespace r2i.OWS.Actions
    Public Class LogAction
        Inherits ActionBase


#Region "Debugging and Identification: Name,Style,Description"
        Public Overrides Function Description(ByRef act As MessageActionItem) As String
            Dim str As String = ""
            If Not act.Parameters Is Nothing AndAlso act.Parameters.Count > 0 Then
                Dim sValue As String = Utility.GetDictionaryValue(act.Parameters, MessageActionsConstants.ACTIONLOG_VALUE_KEY)
                Dim sName As String = Utility.GetDictionaryValue(act.Parameters, MessageActionsConstants.ACTIONLOG_NAME_KEY)
                If sName.Length > 1 Then
                    str &= "<b>" & Utility.HTMLEncode(" " & sName & ":") & "</b>"
                End If
                str &= Utility.HTMLEncode(" " & sValue & ".")
            Else
                str &= " Nothing."
            End If
            Return str
        End Function
        Public Overrides Function Name() As String
            Return "Log"
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
                Dim strValue As String = ""
                Dim strName As String = ""
                Dim bSkipRendering As Boolean = False
                Dim iAssignmentType As Integer = 0

                If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONLOG_NAME_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONLOG_NAME_KEY)).Length > 0 Then
                    strName = CStr(act.Parameters(MessageActionsConstants.ACTIONLOG_NAME_KEY))
                End If
                If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONLOG_VALUE_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONLOG_VALUE_KEY)).Length > 0 Then
                    strValue = CStr(act.Parameters(MessageActionsConstants.ACTIONLOG_VALUE_KEY))
                End If

                strValue = Caller.Engine.RenderString(sharedds, strValue, Caller.Engine.CapturedMessages, False, isPreRender:=False, DebugWriter:=Debugger)
                strName = Caller.Engine.RenderString(sharedds, strName, Caller.Engine.CapturedMessages, False, isPreRender:=False, DebugWriter:=Debugger)
                Controller.AddLog(Caller.Engine.ConfigurationID, Caller.Engine.UserID, strName, strValue, Caller.Engine.Session.SessionID)
            End If
            Return New Runtime.ExecutableResult(Runtime.ExecutableResultEnum.Executed, Nothing)
        End Function

        Public Overrides Function Key() As String
            Return "Action-Log"
        End Function
    End Class
End Namespace