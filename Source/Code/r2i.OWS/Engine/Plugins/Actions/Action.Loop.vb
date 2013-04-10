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
Imports r2i.OWS.Framework.Utilities
Imports r2i.OWS.Framework
Imports r2i.OWS.Framework.Plugins.Actions
Imports r2i.OWS.Framework.Entities
Imports r2i.OWS.Framework.DataAccess
Imports r2i.OWS.Framework.Utilities.Compatibility, r2i.OWS.Actions.Utilities

Namespace r2i.OWS.Actions
    Public Class LoopAction
        Inherits ActionBase


#Region "Debugging and Identification: Name,Style,Description"
        Public Overrides Function Description(ByRef act As MessageActionItem) As String
            Dim str As String = ""
            If Not act.Parameters Is Nothing AndAlso act.Parameters.Count > 0 Then
                If Utility.CNullStr(act.Parameters.Item(MessageActionsConstants.ACTIONCONDITIONIF_ISADVANCED_KEY)).ToUpper = "TRUE" Then
                    str &= Utility.HTMLEncode(Utility.CNullStr(act.Parameters.Item(MessageActionsConstants.ACTIONCONDITIONIF_LEFTCONDITION_KEY)))
                Else
                    str &= Utility.HTMLEncode(Utility.CNullStr(act.Parameters.Item(MessageActionsConstants.ACTIONCONDITIONIF_LEFTCONDITION_KEY)))
                    str &= Utility.HTMLEncode(Utility.CNullStr(act.Parameters.Item(MessageActionsConstants.ACTIONCONDITIONIF_OPERATOR_KEY)))
                    str &= Utility.HTMLEncode(Utility.CNullStr(act.Parameters.Item(MessageActionsConstants.ACTIONCONDITIONIF_RIGHTCONDITION_KEY)))
                End If
            Else
                str &= " (no parameters defined)"
            End If

            Return str
        End Function
        Public Overrides Function Name() As String
            Return "Loop"
        End Function
        Public Overrides Function Style() As String
            Return ""
        End Function
        Public Overrides Function Title(ByRef act As MessageActionItem) As String
            Return Name() & " Until"
        End Function
#End Region
        Public Overrides Function Handle_Action(ByRef Caller As RuntimeBase, ByRef sharedds As System.Data.DataSet, ByRef act As MessageActionItem, ByRef previous As Runtime.ActionExecutionResult, ByRef Debugger As Framework.Debugger) As Runtime.ExecutableResult
            Dim rslt As Runtime.ExecutableResult = New Runtime.ExecutableResult(Runtime.ExecutableResultEnum.Executed, Nothing)
            'If Not act.ActionInformation Is Nothing Then
            If Not act.Parameters Is Nothing Then
                Dim condescription As String = ""
                If Not act.Parameters Is Nothing Then 'AndAlso mi.Parameters.Count > 0 Then
                    Dim sLeftHand As String = Nothing
                    If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONCONDITIONIF_LEFTCONDITION_KEY) Then
                        sLeftHand = act.Parameters(MessageActionsConstants.ACTIONCONDITIONIF_LEFTCONDITION_KEY)
                    End If
                    Dim sOperator As String = Nothing
                    If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONCONDITIONIF_OPERATOR_KEY) Then
                        sOperator = act.Parameters(MessageActionsConstants.ACTIONCONDITIONIF_OPERATOR_KEY)
                    End If
                    Dim sRightHand As String = Nothing
                    If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONCONDITIONIF_RIGHTCONDITION_KEY) Then
                        sRightHand = act.Parameters(MessageActionsConstants.ACTIONCONDITIONIF_RIGHTCONDITION_KEY)
                    End If
                    Dim bAdvanced As Boolean = False
                    If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONCONDITIONIF_ISADVANCED_KEY) Then
                        bAdvanced = act.Parameters(MessageActionsConstants.ACTIONCONDITIONIF_ISADVANCED_KEY)
                    End If

                    If bAdvanced Then
                        sOperator = ""
                        sRightHand = ""
                    End If

                    If Not sLeftHand Is Nothing Then    'If LeftHand isn't nothing, neither is anything else
                        Dim rm As New Renderers.RenderMath
                        Dim failed As Boolean = False
                        condescription = ""
                        If Not Debugger Is Nothing Then
                            r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Looping Starting", True)
                        End If
                        While Not rm.EvaluateCondition(Caller.Engine, sharedds, Caller.Engine.CapturedMessages, sLeftHand, sOperator, sRightHand, condescription, Debugger)
                            'WE CANNOT EXIT...
                            If Not Debugger Is Nothing Then
                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Looping: " & condescription & " = False", True)
                            End If

                            'PROCESS THE CHILD ACTIONS
                            Try
                                rslt = Caller.Execute(act.ChildActions, Debugger, sharedds)
                                If rslt.Result = Runtime.ExecutableResultEnum.Redirected Then
                                    Exit While
                                End If
                            Catch ex As Exception
                                failed = True
                                If Not Debugger Is Nothing Then
                                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Unable to process child actions for this loop instance: " & ex.ToString, True)
                                End If
                                Exit While
                            End Try
                            condescription = ""
                        End While

                        If Not failed Then
                            condescription = condescription & " = True"
                        Else
                            condescription = condescription & " = False (failure)"
                        End If

                        If Not Debugger Is Nothing Then
                            r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Looping Complete: " & condescription, True)
                        End If
                    End If
                End If
            End If
            Return rslt
        End Function

        Public Overrides Function Key() As String
            Return "Action-Loop"
        End Function
    End Class
End Namespace