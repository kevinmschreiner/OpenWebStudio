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
Imports r2i.OWS.Framework.Utilities.Compatibility, r2i.OWS.Actions.Utilities
Imports r2i.OWS.Framework.Utilities
Imports r2i.OWS.Framework.Plugins.Actions

Namespace r2i.OWS.Actions
    Public Class RegionAction
        Inherits ActionBase


#Region "Debugging and Identification: Name,Style,Description"
        Public Overrides Function Description(ByRef act As MessageActionItem) As String
            Dim str As String = ""
            If Not act.Parameters Is Nothing Then
                Dim rname As String = ""
                Dim typevalue As String = "(Unknown)"
                If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONREGION_NAME_KEY) Then
                    rname = act.Parameters(MessageActionsConstants.ACTIONREGION_NAME_KEY)
                End If
                If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONREGION_RENDERTYPE_KEY) Then
                    If act.Parameters(MessageActionsConstants.ACTIONREGION_RENDERTYPE_KEY) = 0 Then
                        typevalue = "(Ajax and Page Load)"
                    ElseIf act.Parameters(MessageActionsConstants.ACTIONREGION_RENDERTYPE_KEY) = 2 Then
                        typevalue = "(Ajax Only)"
                    ElseIf act.Parameters(MessageActionsConstants.ACTIONREGION_RENDERTYPE_KEY) = 1 Then
                        typevalue = "(Page Load Only)"
                    End If
                End If
                Dim canDebug As Boolean = True
                If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONREGION_DEBUG_KEY) Then
                    If act.Parameters(MessageActionsConstants.ACTIONREGION_DEBUG_KEY) = True Then
                        canDebug = False
                    End If
                End If
                If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONREGION_SEARCH_KEY) Then
                    If act.Parameters(MessageActionsConstants.ACTIONREGION_SEARCH_KEY) = True Then
                        typevalue &= " (Searchable)"
                    End If
                End If
                If canDebug Then
                    Return typevalue & ": " & rname
                Else
                    Return typevalue & "(Debug Disabled): " & rname
                End If
            End If
            Return str
        End Function
        Public Overrides Function Name() As String
            Return "Region"
        End Function
        Public Overrides Function Style() As String
            Return ""
        End Function
        Public Overrides Function Title(ByRef act As MessageActionItem) As String
            Return Name()
        End Function
#End Region
        Public Overrides Function Handle_Action(ByRef Caller As RuntimeBase, ByRef sharedds As System.Data.DataSet, ByRef act As MessageActionItem, ByRef previous As Runtime.ActionExecutionResult, ByRef Debugger As Framework.Debugger) As Runtime.ExecutableResult
            If Not act.Parameters Is Nothing Then
                Dim rname As String = ""
                If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONREGION_NAME_KEY) Then
                    rname = act.Parameters(MessageActionsConstants.ACTIONREGION_NAME_KEY)
                End If
                If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONREGION_RENDERTYPE_KEY) Then
                    Dim conditionPassed As Boolean = False
                    Dim typevalue As String = "(Unknown)"
                    Dim canDebug As Boolean = True

                    'STANDARD PAGE REQUESTS
                    Select Case Caller.Engine.RequestType
                        Case EngineBase.RequestTypeEnum.Ajax, EngineBase.RequestTypeEnum.Page, EngineBase.RequestTypeEnum.Postback
                            Dim cannotExecute As Boolean = False
                            If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONREGION_SEARCH_KEY) AndAlso act.Parameters(MessageActionsConstants.ACTIONREGION_SEARCH_KEY) = True Then
                                cannotExecute = True
                            ElseIf act.Parameters.ContainsKey(MessageActionsConstants.ACTIONREGION_IMPORT_KEY) AndAlso act.Parameters(MessageActionsConstants.ACTIONREGION_IMPORT_KEY) = True Then
                                cannotExecute = True
                            ElseIf act.Parameters.ContainsKey(MessageActionsConstants.ACTIONREGION_EXPORT_KEY) AndAlso act.Parameters(MessageActionsConstants.ACTIONREGION_EXPORT_KEY) = True Then
                                cannotExecute = True
                            End If
                            If Not cannotExecute Then
                                Select Case CType(act.Parameters(MessageActionsConstants.ACTIONREGION_RENDERTYPE_KEY), Integer)
                                    Case 0
                                        conditionPassed = True
                                        typevalue = "(Ajax and Page Load)"
                                    Case 1
                                        If Caller.Engine.RequestType = EngineBase.RequestTypeEnum.Page OrElse Caller.Engine.RequestType = EngineBase.RequestTypeEnum.Postback Then
                                            conditionPassed = True
                                        End If
                                        typevalue = "(Page Load Only)"
                                    Case 2
                                        If Caller.Engine.RequestType = EngineBase.RequestTypeEnum.Ajax Then
                                            conditionPassed = True
                                        End If
                                        typevalue = "(Ajax Only)"
                                End Select
                            Else
                                typevalue = "(Not Import/Export/Search)"
                                conditionPassed = False
                            End If
                        Case EngineBase.RequestTypeEnum.Search
                            If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONREGION_SEARCH_KEY) AndAlso act.Parameters(MessageActionsConstants.ACTIONREGION_SEARCH_KEY) = True Then
                                'THIS ONLY MATTERS IF SEARCH IS TRUE AND THIS IS A SEARCH REQUEST
                                typevalue = "(Searchable)"
                                conditionPassed = True
                            Else
                                typevalue = "(Not Searchable)"
                                conditionPassed = False
                            End If
                        Case EngineBase.RequestTypeEnum.Import
                            If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONREGION_IMPORT_KEY) AndAlso act.Parameters(MessageActionsConstants.ACTIONREGION_IMPORT_KEY) = True Then
                                'THIS ONLY MATTERS IF SEARCH IS TRUE AND THIS IS A SEARCH REQUEST
                                typevalue = "(Importable)"
                                conditionPassed = True
                            Else
                                typevalue = "(Not Importable)"
                                conditionPassed = False
                            End If
                        Case EngineBase.RequestTypeEnum.Export
                            If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONREGION_EXPORT_KEY) AndAlso act.Parameters(MessageActionsConstants.ACTIONREGION_EXPORT_KEY) = True Then
                                'THIS ONLY MATTERS IF SEARCH IS TRUE AND THIS IS A SEARCH REQUEST
                                typevalue = "(Exportable)"
                                conditionPassed = True
                            Else
                                typevalue = "(Not Exportable)"
                                conditionPassed = False
                            End If
                    End Select


                    If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONREGION_DEBUG_KEY) Then
                        If act.Parameters(MessageActionsConstants.ACTIONREGION_DEBUG_KEY) = True Then
                            canDebug = False
                        End If
                    End If
                    If conditionPassed Then
                        If canDebug Then
                            r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Executing Region " & typevalue & ": " & rname, True)
                            Return Caller.Execute(act.ChildActions, Debugger, sharedds)
                        Else
                            r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Executing Region " & typevalue & "(Debug Disabled): " & rname, True)
                            Return Caller.Execute(act.ChildActions, Nothing, sharedds)
                        End If
                    Else
                        If canDebug Then
                            r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Skipping Region " & typevalue & ": " & rname, True)
                        Else
                            r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Skipping Region " & typevalue & "(Debug Disabled): " & rname, True)
                        End If
                    End If
                End If
            End If
            Return New Runtime.ExecutableResult(Runtime.ExecutableResultEnum.Executed, Nothing)
        End Function

        Public Overrides Function Key() As String
            Return "Action-Region"
        End Function
    End Class
End Namespace