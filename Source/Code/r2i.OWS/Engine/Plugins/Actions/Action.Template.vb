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
Imports r2i.OWS.Framework.Utilities.Compatibility, r2i.OWS.Actions.Utilities

Namespace r2i.OWS.Actions
    Public Class TemplateAction
        Inherits ActionBase


#Region "Debugging and Identification: Name,Style,Description"
        Public Overrides Function Description(ByRef act As MessageActionItem) As String
            Dim str As String = ""
            Return str
        End Function
        Public Overrides Function Name() As String
            Return "Template"
        End Function
        Public Overrides Function Title(ByRef act As MessageActionItem) As String
            Dim str As String = Name() & " "
            If Not act.Parameters Is Nothing Then
                Dim type As String = Nothing
                If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONTEMPLATE_TYPE_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONTEMPLATE_TYPE_KEY)).Length > 0 Then
                    type = CStr(act.Parameters(MessageActionsConstants.ACTIONTEMPLATE_TYPE_KEY))
                    Select Case type.ToLower
                        Case "query-query"
                            str &= "Query "
                        Case "group-footer"
                            str &= "Group Footer "
                            Dim groupStatement As String = ""
                            Dim groupIndex As String = Nothing
                            If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONTEMPLATE_GROUPSTATEMENT_KEY) Then
                                groupStatement = CStr(act.Parameters(MessageActionsConstants.ACTIONTEMPLATE_GROUPSTATEMENT_KEY))
                            End If
                            If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONTEMPLATE_GROUPINDEX_KEY) Then
                                groupIndex = CStr(act.Parameters(MessageActionsConstants.ACTIONTEMPLATE_GROUPINDEX_KEY))
                            End If
                            If groupStatement Is Nothing Then groupStatement = ""
                            If groupIndex Is Nothing Then groupIndex = ""
                            str &= "[" & groupIndex & "]"
                            str &= "(" & groupStatement & ")"
                        Case "group-header"
                            str &= "Group Header "
                            Dim groupStatement As String = ""
                            Dim groupIndex As String = Nothing
                            If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONTEMPLATE_GROUPSTATEMENT_KEY) Then
                                groupStatement = CStr(act.Parameters(MessageActionsConstants.ACTIONTEMPLATE_GROUPSTATEMENT_KEY))
                            End If
                            If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONTEMPLATE_GROUPINDEX_KEY) Then
                                groupIndex = CStr(act.Parameters(MessageActionsConstants.ACTIONTEMPLATE_GROUPINDEX_KEY))
                            End If
                            If groupStatement Is Nothing Then groupStatement = ""
                            If groupIndex Is Nothing Then groupIndex = ""
                            str &= "[" & groupIndex & "]"
                            str &= "(" & groupStatement & ")"
                            'End If
                        Case "detail-noquery"
                            str &= "Detail (No Query) "
                        Case "detail-noresults"
                            str &= "Detail (No Results) "
                        Case "detail-detail"
                            str &= "Detail "
                        Case "detail-alternate"
                            str &= "Detail (Alternate) "
                        Case Else
                            'TODO: Exception undefined type
                    End Select
                End If
            End If
            Return str
        End Function
        Public Overrides Function Style() As String
            Return ""
        End Function
#End Region

        Public Overrides Function Handle_Action(ByRef Caller As RuntimeBase, ByRef sharedds As System.Data.DataSet, ByRef act As MessageActionItem, ByRef previous As Runtime.ActionExecutionResult, ByRef Debugger As Framework.Debugger) As Runtime.ExecutableResult
            If Not act.Parameters Is Nothing Then
                Dim type As String
                If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONTEMPLATE_TYPE_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONTEMPLATE_TYPE_KEY)).Length > 0 Then
                    type = CStr(act.Parameters(MessageActionsConstants.ACTIONTEMPLATE_TYPE_KEY))

                    Select Case type.ToLower
                        Case "query-query"
                            If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONTEMPLATE_VALUE_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONTEMPLATE_VALUE_KEY)).Length > 0 Then
                                Caller.Engine.xls.query = CStr(act.Parameters(MessageActionsConstants.ACTIONTEMPLATE_VALUE_KEY))
                            End If
                            If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONTEMPLATE_QUERYCONNECTION_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONTEMPLATE_QUERYCONNECTION_KEY)).Length > 0 Then
                                Caller.Engine.xls.customConnection = CStr(act.Parameters(MessageActionsConstants.ACTIONTEMPLATE_QUERYCONNECTION_KEY))
                            End If
                            If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONTEMPLATE_QUERYFILTER_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONTEMPLATE_QUERYFILTER_KEY)).Length > 0 Then
                                'TODO: implements queryfilter assignement
                                Caller.Engine.xls.filter = CStr(act.Parameters(MessageActionsConstants.ACTIONTEMPLATE_QUERYFILTER_KEY))
                            End If
                            If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONTEMPLATE_CACHE_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONTEMPLATE_CACHE_KEY)).Length > 0 Then
                                Caller.Engine.TemplateCacheName = CStr(act.Parameters(MessageActionsConstants.ACTIONTEMPLATE_CACHE_KEY))
                            End If
                            If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONTEMPLATE_CACHETIME_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONTEMPLATE_CACHETIME_KEY)).Length > 0 Then
                                Caller.Engine.TemplateCacheTime = CStr(act.Parameters(MessageActionsConstants.ACTIONTEMPLATE_CACHETIME_KEY))
                            End If
                            If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONTEMPLATE_CACHETIME_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONTEMPLATE_CACHETIME_KEY)).Length > 0 Then
                                Caller.Engine.TemplateCacheShare = CStr(act.Parameters(MessageActionsConstants.ACTIONTEMPLATE_CACHESHARE_KEY))
                            End If
                            If Not Debugger Is Nothing Then
                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Assigning Query: " & Caller.Engine.xls.query, True)
                            End If
                        Case "group-footer"

                            Dim groupStatement As String = ""
                            Dim groupIndex As String = Nothing
                            If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONTEMPLATE_GROUPSTATEMENT_KEY) Then
                                groupStatement = CStr(act.Parameters(MessageActionsConstants.ACTIONTEMPLATE_GROUPSTATEMENT_KEY))
                            End If
                            If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONTEMPLATE_GROUPINDEX_KEY) Then
                                groupIndex = CStr(act.Parameters(MessageActionsConstants.ACTIONTEMPLATE_GROUPINDEX_KEY))
                            End If
                            If groupStatement Is Nothing Then groupStatement = ""
                            If groupIndex Is Nothing Then groupIndex = ""

                            Dim lstFormatItem As ListFormatItem

                            'If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONTEMPLATE_GROUPSTATEMENT_KEY) Then
                            lstFormatItem = REPLACEMENT_ListItemFormatSearch(Caller, groupIndex)
                            lstFormatItem.GroupStatement = groupStatement
                            lstFormatItem.ListFooter = CStr(act.Parameters(MessageActionsConstants.ACTIONTEMPLATE_VALUE_KEY))
                            If Not Debugger Is Nothing Then
                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Assigning Footer: " & lstFormatItem.ListFooter, True)
                            End If
                            'End If
                        Case "group-header"
                            Dim groupStatement As String = ""
                            Dim groupIndex As String = Nothing
                            If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONTEMPLATE_GROUPSTATEMENT_KEY) Then
                                groupStatement = CStr(act.Parameters(MessageActionsConstants.ACTIONTEMPLATE_GROUPSTATEMENT_KEY))
                            End If
                            If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONTEMPLATE_GROUPINDEX_KEY) Then
                                groupIndex = CStr(act.Parameters(MessageActionsConstants.ACTIONTEMPLATE_GROUPINDEX_KEY))
                            End If
                            If groupStatement Is Nothing Then groupStatement = ""
                            If groupIndex Is Nothing Then groupIndex = ""

                            Dim lstFormatItem As ListFormatItem
                            'If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONTEMPLATE_GROUPSTATEMENT_KEY) Then
                            lstFormatItem = REPLACEMENT_ListItemFormatSearch(Caller, groupIndex)
                            lstFormatItem.GroupStatement = groupStatement
                            lstFormatItem.ListHeader = CStr(act.Parameters(MessageActionsConstants.ACTIONTEMPLATE_VALUE_KEY))
                            If Not Debugger Is Nothing Then
                                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Assigning Header: " & lstFormatItem.ListHeader, True)
                            End If
                            'End If
                        Case "detail-noquery"
                            If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONTEMPLATE_VALUE_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONTEMPLATE_VALUE_KEY)).Length > 0 Then
                                Caller.Engine.xls.noqueryItem = CStr(act.Parameters(MessageActionsConstants.ACTIONTEMPLATE_VALUE_KEY))
                                If Not Debugger Is Nothing Then
                                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Assigning No Query: " & Caller.Engine.xls.noqueryItem, True)
                                End If
                            End If
                        Case "detail-noresults"
                            If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONTEMPLATE_VALUE_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONTEMPLATE_VALUE_KEY)).Length > 0 Then
                                Caller.Engine.xls.defaultItem = CStr(act.Parameters(MessageActionsConstants.ACTIONTEMPLATE_VALUE_KEY))
                                If Not Debugger Is Nothing Then
                                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Assigning No Results: " & Caller.Engine.xls.defaultItem, True)
                                End If
                            End If
                        Case "detail-detail"
                            If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONTEMPLATE_VALUE_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONTEMPLATE_VALUE_KEY)).Length > 0 Then
                                Caller.Engine.xls.listItem = CStr(act.Parameters(MessageActionsConstants.ACTIONTEMPLATE_VALUE_KEY))
                                If Not Debugger Is Nothing Then
                                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Assigning Detail: " & Caller.Engine.xls.listItem, True)
                                End If
                            End If
                        Case "detail-alternate"
                            If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONTEMPLATE_VALUE_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONTEMPLATE_VALUE_KEY)).Length > 0 Then
                                Caller.Engine.xls.listAItem = CStr(act.Parameters(MessageActionsConstants.ACTIONTEMPLATE_VALUE_KEY))
                                If Not Debugger Is Nothing Then
                                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Assigning Detail Alternate: " & Caller.Engine.xls.listAItem, True)
                                End If
                            End If
                        Case Else
                            'TODO: Exception undefined type
                    End Select

                    'Checking the current Engine.xls.listItems list
                    'If Not _Engine.xls.listItems.Contains(lstFormatItem) Then
                    '    _Engine.xls.listItems.Add(lstFormatItem)
                    'Else
                    '    For Each lstFI As ListFormatItem In _Engine.xls.listItems
                    '        If Not lstFI.ListFooter = lstFormatItem.ListFooter AndAlso lstFI.ListHeader = lstFormatItem.ListHeader Then

                    '        End If
                    '    Next
                    'End If

                    'PROCESS THE CHILD ACTIONS FOR FILE ATTACHMENTS
                    Try
                        If Not act.ChildActions Is Nothing AndAlso act.ChildActions.Count > 0 Then
                            ' Caller.SetChildren(act.ChildActions, MessageActionItem.ActionStatusType.DoExecute, Debugger)
                            'Caller.ProcessChildActions(act.ChildActions, Debugger, act.Level + 1, sharedds)
                            Return Caller.Execute(act.ChildActions, Debugger, sharedds)
                        End If
                    Catch ex As Exception
                        If Not Debugger Is Nothing Then
                            r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Unable to process child actions for this template: " & ex.ToString, True)
                        End If
                    End Try
                Else
                    'TODO: implement exception
                End If
            End If
            Return New Runtime.ExecutableResult(Runtime.ExecutableResultEnum.Executed, Nothing)
        End Function

        Private Function REPLACEMENT_ListItemFormatSearch(ByRef Caller As Runtime, ByVal index As String) As ListFormatItem
            Dim i As Integer
            If Caller.Engine.xls.listItems Is Nothing Then
                Caller.Engine.xls.listItems = New Generic.List(Of ListFormatItem)
            End If
            For i = 0 To Caller.Engine.xls.listItems.Count - 1
                If Caller.Engine.xls.listItems(i).Index = index Then
                    Return Caller.Engine.xls.listItems(i)
                End If
            Next
            Dim lsta As New ListFormatItem
            lsta.Index = index
            Caller.Engine.xls.listItems.Add(lsta)
            Return lsta
        End Function

        Public Overrides Function Key() As String
            Return "Template"
        End Function
    End Class
End Namespace