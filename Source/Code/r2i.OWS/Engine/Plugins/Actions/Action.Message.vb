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
    Public Class ConditionMessageAction
        Inherits ActionBase


#Region "Debugging and Identification: Name,Style,Description"
        Public Overrides Function Description(ByRef act As MessageActionItem) As String
            Dim str As String = ""
            str &= "Awaiting incoming message with "
            If Not act.Parameters Is Nothing AndAlso act.Parameters.Count > 0 Then
                If Not act.Parameters Is Nothing Then
                    Dim i As Integer = 0
                    For Each kvp As KeyValuePair(Of String, Object) In act.Parameters
                        str &= kvp.Key & ":"
                        If TypeOf (kvp.Value) Is String Then
                            str &= kvp.Value.ToString
                        End If
                        i += 1
                        If i < act.Parameters.Count Then
                            str &= " and "
                        End If
                    Next
                End If
            Else
                str &= " undefined message event settings"
            End If
            Return str
        End Function
        Public Overrides Function Name() As String
            Return "Message"
        End Function
        Public Overrides Function Style() As String
            Return ""
        End Function
        Public Overrides Function Title(ByRef act As MessageActionItem) As String
            Return Name()
        End Function
#End Region
        Public Overrides Function Handle_Action(ByRef Caller As RuntimeBase, ByRef sharedds As System.Data.DataSet, ByRef act As MessageActionItem, ByRef previous As Runtime.ActionExecutionResult, ByRef Debugger As Framework.Debugger) As Runtime.ExecutableResult
            Dim sType As String = Nothing
            Dim sValue As String = Nothing
            If act.Parameters.ContainsKey(MessageActionsConstants.MESSAGE_TYPE_KEY) Then
                sType = act.Parameters(MessageActionsConstants.MESSAGE_TYPE_KEY)
            End If
            If act.Parameters.ContainsKey(MessageActionsConstants.MESSAGE_VALUE_KEY) Then
                sValue = act.Parameters(MessageActionsConstants.MESSAGE_VALUE_KEY)
            Else
                sValue = ""
            End If
            If Not Caller.Engine.CapturedMessages Is Nothing AndAlso Not sType Is Nothing AndAlso Caller.Engine.CapturedMessages.ContainsKey(sType) Then
                If sValue.Length > 0 Then
                    If sValue = Caller.Engine.CapturedMessages.Item(sType) Then
                        r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Passed: Handling actions for '" & sType & "' with a value of '" & sValue & "'", True)
                        Return Caller.Execute(act.ChildActions, Debugger, sharedds)
                    Else
                        If Not Debugger Is Nothing Then
                            r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Skip", False)
                        End If
                    End If
                Else
                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Passed: Handling actions for '" & sType & "'", True)
                    Return Caller.Execute(act.ChildActions, Debugger, sharedds)
                End If
            ElseIf sType.Length > 0 AndAlso Not Debugger Is Nothing Then
                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Failed: Skipping actions for '" & sType & "'", True)
            End If
            Return New Runtime.ExecutableResult(Runtime.ExecutableResultEnum.Executed, Nothing)
        End Function

        Public Overrides Function Key() As String
            Return "Message"
        End Function
    End Class
End Namespace