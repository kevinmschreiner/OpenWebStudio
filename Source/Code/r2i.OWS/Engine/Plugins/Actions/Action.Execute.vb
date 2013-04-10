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
Imports r2i.OWS.Framework.Utilities.Compatibility
Imports r2i.OWS.Framework.Plugins
Imports r2i.OWS.Framework.Entities
Imports r2i.OWS.Framework.DataAccess
Imports r2i.OWS.Framework.Plugins.Actions
Imports r2i.OWS.Actions.Utilities

Namespace r2i.OWS.Actions
    Public Class ExecuteAction
        Inherits ActionBase


#Region "Debugging and Identification: Name,Style,Description"
        Public Overrides Function Description(ByRef act As MessageActionItem) As String
            Dim str As String = ""
            If Not act.Parameters Is Nothing AndAlso act.Parameters.Count > 0 Then
                Dim sQuery As String = Utility.GetDictionaryValue(act.Parameters, MessageActionsConstants.ACTIONEXECUTE_QUERY_KEY)
                str &= Utility.HTMLEncode(sQuery)
            Else
                str &= " No query or parameters defined."
            End If
            Return str
        End Function
        Public Overrides Function Name() As String
            Return "Query"
        End Function
        Public Overrides Function Title(ByRef act As MessageActionItem) As String
            Dim str As String = ""
            If Not act.Parameters Is Nothing AndAlso act.Parameters.Count > 0 Then
                Dim sName As String = Utility.GetDictionaryValue(act.Parameters, MessageActionsConstants.ACTIONEXECUTE_NAME_KEY)
                str &= "Execute Query [" & Utility.HTMLEncode(sName) & "]"
            Else
                str &= "Execute Query [Undefined]"
            End If
            Return str
        End Function
        Public Overrides Function Style() As String
            Return ""
        End Function
#End Region

        Public Overrides Function Handle_Action(ByRef Caller As RuntimeBase, ByRef sharedds As System.Data.DataSet, ByRef act As r2i.OWS.Framework.Plugins.Actions.MessageActionItem, ByRef previous As Runtime.ActionExecutionResult, ByRef Debugger As Framework.Debugger) As Runtime.ExecutableResult

            'If Not act.ActionInformation Is Nothing Then
            If Not act.Parameters Is Nothing Then
                Dim parms As SerializableDictionary(Of String, Object) = act.Parameters
                Dim strName As String = Utility.GetDictionaryValue(parms, MessageActionsConstants.ACTIONEXECUTE_NAME_KEY)
                Dim strValue As String = Utility.GetDictionaryValue(parms, MessageActionsConstants.ACTIONEXECUTE_QUERY_KEY)
                Dim strConnection As String = Utility.GetDictionaryValue(parms, MessageActionsConstants.ACTIONEXECUTE_CONNECTION_KEY)
                Dim runasprocess As Boolean = Utility.CNullBool(Utility.GetDictionaryValue(parms, MessageActionsConstants.ACTIONEXECUTE_ISPROCESS_KEY))
                Dim strCacheName As String = Utility.GetDictionaryValue(parms, MessageActionsConstants.ACTIONEXECUTE_CACHENAME_KEY)
                Dim strCacheTime As String = Utility.GetDictionaryValue(parms, MessageActionsConstants.ACTIONEXECUTE_CACHETIME_KEY)
                Dim bCacheShare As Boolean = Utility.CNullBool(Utility.GetDictionaryValue(parms, MessageActionsConstants.ACTIONEXECUTE_CACHESHARE_KEY))

                If Not Debugger Is Nothing Then
                    If Not runasprocess Then
                        r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Executing Query: " & strValue, True)
                    Else
                        r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Executing Query (Process): " & strValue, True)
                    End If
                End If

                strName = Caller.Engine.RenderString(sharedds, strName, Caller.Engine.CapturedMessages, False, isPreRender:=False, DebugWriter:=Debugger)
                strCacheName = Caller.Engine.RenderString(sharedds, strCacheName, Caller.Engine.CapturedMessages, False, isPreRender:=False, DebugWriter:=Debugger)
                strCacheTime = Caller.Engine.RenderString(sharedds, strCacheTime, Caller.Engine.CapturedMessages, False, isPreRender:=False, DebugWriter:=Debugger)
                strConnection = Caller.Engine.RenderString(sharedds, strConnection, Caller.Engine.CapturedMessages, False, isPreRender:=False, DebugWriter:=Debugger)

                Dim Query As String = Caller.Engine.RenderQuery(sharedds, Caller.FilterField, Caller.FilterText, Caller.Engine.RecordsPerPage, Caller.Engine.CapturedMessages, Debugger, strValue)

                If Not runasprocess Then
                    'KMS: Immediatly return child results (this was originally not returned)
                    Return Handle_ExecuteQuery_Processing(Caller, strName, Query, strConnection, sharedds, act, Debugger, Nothing, False, strCacheName, strCacheTime, bCacheShare)
                Else
                    Dim p As New Execute_ThreadedMessageActionProcess

                    p.Name = strName
                    p.SharedDS = sharedds
                    p.ThreadAction = act
                    p.Debugger = Nothing
                    p.Script = Query
                    p.CacheName = strCacheName
                    p.CacheTime = strCacheTime
                    p.CacheShare = bCacheShare
                    p.RenderingEngine = Caller.Engine
                    p.FilterField = Caller.FilterField
                    p.FilterText = Caller.FilterText
                    p.Connection = strConnection

                    Dim d As New Execute_ThreadedMessageActionProcess.Execute_ThreadedMessageActionProcess_Address(AddressOf Handle_ExecuteQuery)
                    p.StartPosition = d
                    Dim t As New Threading.Thread(AddressOf p.Start)
                    p.ThreadObj = t
                    ThreadedMessageHandler.StartProcess(p, strName, Caller.Engine.Session.SessionID, t)

                    System.Threading.ThreadPool.QueueUserWorkItem(New Threading.WaitCallback(AddressOf _Start_), p)
                    't.Start()
                End If
            End If
            Return New Runtime.ExecutableResult(Runtime.ExecutableResultEnum.Executed, Nothing)
        End Function
        Private Sub _Start_(ByVal state As Object)
            CType(state, Execute_ThreadedMessageActionProcess).Start()
        End Sub

        Private Sub Handle_ExecuteQuery(ByVal Obj As Execute_ThreadedMessageActionProcess)
            Try
                Handle_ExecuteQuery_Processing(Nothing, Obj.Name, Obj.Script, Obj.Connection, Obj.SharedDS, Obj.ThreadAction, Nothing, CType(Obj, Execute_ThreadedMessageActionProcess), True, Obj.CacheName, Obj.CacheTime, Obj.CacheShare)
            Catch ex As Exception
                Dim name As String = "Unknown"
                Dim processname As String = "Default"
                If Not Obj Is Nothing Then
                    If Not Obj Is Nothing AndAlso Not Obj.Name Is Nothing AndAlso Obj.Name.Length > 0 Then
                        name = Obj.Name
                    ElseIf Not Obj Is Nothing AndAlso Not Obj.Script Is Nothing AndAlso Obj.Script.Length > 0 Then
                        name = Obj.Script
                    End If
                    If Not Obj.ThreadObj Is Nothing AndAlso Not Obj.ThreadObj.Name Is Nothing Then
                        processname = Obj.ThreadObj.Name
                    End If
                End If
                'ROMAIN: 09/19/07
                'TODO: Change Exceptions
                'DotNetNuke.Services.Exceptions.LogException(New Exception("The Process (" & processname & ") failed during execution of the SQL statement: " & name & " . The contents of the error were: " & ex.ToString))
            End Try
            Obj.Completed = True
        End Sub

        Private Function Handle_ExecuteQuery_Processing(ByRef Caller As Runtime, ByVal Name As String, ByVal Query As String, ByVal Connection As String, ByRef sharedds As DataSet, ByVal act As MessageActionItem, ByRef Debugger As Debugger, ByRef threadObj As ThreadedMessageActionProcess, ByVal noTimeout As Boolean, ByVal CacheName As String, ByVal CacheTime As String, ByVal CacheShare As Boolean) As Runtime.ExecutableResult
            'Dim rv As New Renderers.RenderVariable

            If Not threadObj Is Nothing Then
                'WE NEED TO ASSIGN THE STATUS
                'SET THE START TIME
                threadObj.Status = Now.ToString
            End If

            Dim _FilterField As String = Nothing
            Dim _FilterText As String = Nothing
            Dim _Engine As Engine = Nothing
            If Not Caller Is Nothing Then
                _FilterField = Caller.FilterField
                _FilterText = Caller.FilterText
                _Engine = Caller.Engine
            ElseIf Not threadObj Is Nothing Then
                _Engine = threadObj.RenderingEngine
                _FilterField = threadObj.FilterField
                _FilterText = threadObj.FilterText
            End If

            Dim ds As DataSet = Nothing
            Dim errorMessage As String = ""
            Dim isSuccessful As Boolean = False

            If (Query Is Nothing OrElse Query.Length = 0) AndAlso Not Name Is Nothing AndAlso Name.Length > 0 AndAlso Not sharedds Is Nothing AndAlso sharedds.Tables.Contains(Name) Then
                Try
                    ds = New DataSet
                    ds.Tables.Add(sharedds.Tables(Name).Copy)
                    isSuccessful = True
                Catch ex As Exception
                End Try
            End If

            If ds Is Nothing Then
                If System.Threading.Thread.CurrentThread.Name Is Nothing OrElse System.Threading.Thread.CurrentThread.Name.IndexOf(_Engine.Session.SessionID) < 0 Then
                    If Not Connection Is Nothing AndAlso Connection.Length > 0 Then
                        If noTimeout Then
                            ds = _Engine.GetData(False, Query, _FilterField, _FilterText, Debugger, True, CacheName, CacheTime, CacheShare, timeout:=9000, CustomConnection:=Connection, FailureMessage:=errorMessage, IsSuccessful:=isSuccessful)
                        Else
                            ds = _Engine.GetData(False, Query, _FilterField, _FilterText, Debugger, True, CacheName, CacheTime, CacheShare, CustomConnection:=Connection, FailureMessage:=errorMessage, IsSuccessful:=isSuccessful)
                        End If
                    Else
                        If noTimeout Then
                            ds = _Engine.GetData(False, Query, _FilterField, _FilterText, Debugger, False, CacheName, CacheTime, CacheShare, timeout:=9000, FailureMessage:=errorMessage, IsSuccessful:=isSuccessful)
                        Else
                            ds = _Engine.GetData(False, Query, _FilterField, _FilterText, Debugger, False, CacheName, CacheTime, CacheShare, FailureMessage:=errorMessage, IsSuccessful:=isSuccessful)
                        End If
                    End If
                Else
                    If Not Connection Is Nothing AndAlso Connection.Length > 0 Then
                        ds = _Engine.GetData(False, Query, _FilterField, _FilterText, Debugger, True, CacheName, CacheTime, CacheShare, timeout:=9000, CustomConnection:=Connection, FailureMessage:=errorMessage, IsSuccessful:=isSuccessful)
                    Else
                        ds = _Engine.GetData(False, Query, _FilterField, _FilterText, Debugger, True, CacheName, CacheTime, CacheShare, timeout:=9000, FailureMessage:=errorMessage, IsSuccessful:=isSuccessful)
                    End If
                End If
            End If

            If Not Name Is Nothing AndAlso Name.Length > 0 Then
                'VERSION: 1.9.8 - [ExecuteName.isSuccessful,Action] (TRUE OR FALSE)
                'VERSION: 1.9.8 - [ExecuteName.Error,Action] (Error Message)
                'VERSION: 1.9.8 - Executing more than one query with the same name simply replaces the results in the result set.
                'VERSION: 2.0.7 - Added Tables, Rows, and Table.#.Rows
                Dim strRowLength As Integer = 0
                If isSuccessful = False Then
                    _Engine.ActionVariable(Name & ".isSuccessful") = "False"
                    _Engine.ActionVariable(Name & ".Error") = errorMessage
                    _Engine.ActionVariable(Name & ".Query") = Query
                    _Engine.ActionVariable(Name & ".Tables") = "0"
                Else
                    _Engine.ActionVariable(Name & ".isSuccessful") = "True"
                    If Not ds Is Nothing AndAlso ds.Tables.Count > 0 Then
                        _Engine.ActionVariable(Name & ".Tables") = ds.Tables.Count.ToString
                        Dim dI As Integer = 0
                        For dI = 0 To ds.Tables.Count - 1
                            strRowLength += ds.Tables(dI).Rows.Count
                            _Engine.ActionVariable(Name & ".Tables." & dI.ToString & ".Rows") = ds.Tables(dI).Rows.Count.ToString
                        Next
                    Else
                        _Engine.ActionVariable(Name & ".Tables") = "0"
                    End If
                End If
                _Engine.ActionVariable(Name & ".Rows") = strRowLength
            End If
            If Not ds Is Nothing AndAlso Not ds.Tables Is Nothing AndAlso ds.Tables.Count > 0 Then
                'TODO: Change this to use Action variables
                Dim dt As DataTable = ds.Tables(0).Copy
                If Name.Length > 0 Then
                    If ds.Tables.Contains(Name) Then
                        ds.Tables.Remove(Name)
                    End If
                    dt.TableName = Name
                Else
                    dt.TableName = "Table" & sharedds.Tables.Count
                End If
                If Not threadObj Is Nothing Then
                    If sharedds.Tables.Contains(dt.TableName) Then
                        sharedds.Tables.Remove(dt.TableName)
                    End If
                    threadObj.SharedDS.Tables.Add(dt)
                Else
                    If sharedds.Tables.Contains(dt.TableName) Then
                        sharedds.Tables.Remove(dt.TableName)
                    End If
                    sharedds.Tables.Add(dt)
                End If
                If Not Caller Is Nothing AndAlso Not Debugger Is Nothing Then
                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Resulting table contained " & dt.Columns.Count & " columns and " & dt.Rows.Count & " rows.", True)
                End If

                If Not act.ChildActions Is Nothing AndAlso act.ChildActions.Count > 0 AndAlso Not dt Is Nothing AndAlso Not dt.Rows Is Nothing AndAlso dt.Rows.Count > 0 Then
                    'LOOP THROUGH THESE RECORDS
                    Dim dr As DataRow
                    Dim index As Integer = 0
                    For Each dr In dt.Rows
                        Try
                            'Caller.Engine.CachedTableRowIndexCollection(dt.TableName) = index
                            _Engine.CachedTableRowIndexCollection(dt.TableName) = index

                            'EXECUTE THE MESSAGE ACTIONS FOR THIS ITEM
                            'VERSION: 1.7.9 - Modified Execution Action Script when query is run in an external process
                            If Not threadObj Is Nothing AndAlso Not threadObj.RenderingEngine Is Nothing AndAlso (Not threadObj.ThreadAction.ChildActions Is Nothing OrElse threadObj.ThreadAction.ChildActions.Count > 0) Then
                                'Dim mah As New MessageActions(threadObj.RenderingEngine, threadObj.FilterField, threadObj.FilterText)
                                'mah.HandleProcessMessages(threadObj.ThreadAction.ChildActions, Nothing, 0, threadObj.SharedDS, _Engine.Session.SessionID & ":::" & threadObj.Name)
                                Dim exe As New Runtime(threadObj.RenderingEngine, threadObj.FilterField, threadObj.FilterText)
                                exe.Execute(threadObj.ThreadAction.ChildActions, Nothing, threadObj.SharedDS)
                                index += 1
                                If Not threadObj Is Nothing Then
                                    'WE NEED TO ASSIGN THE STATUS
                                    If dt.Rows.Count > 0 Then
                                        threadObj.Percentage = (index / dt.Rows.Count) * 100
                                    End If
                                End If
                            ElseIf Not Caller Is Nothing Then
                                'MessageActions.SetChildren(act.ChildActions, MessageActionItem.ActionStatusType.DoExecute, Debugger)
                                'Caller.ProcessChildActions(act.ChildActions, Debugger, act.Level + 1, sharedds)
                                Dim result As Runtime.ExecutableResult = Caller.Execute(act.ChildActions, Debugger, sharedds)
                                If result.Result = Runtime.ExecutableResultEnum.Redirected Then
                                    Return result
                                End If
                                index += 1
                                If Not threadObj Is Nothing Then
                                    'WE NEED TO ASSIGN THE STATUS
                                    If dt.Rows.Count > 0 Then
                                        threadObj.Percentage = (index / dt.Rows.Count) * 100
                                    End If
                                End If
                            End If
                        Catch ex As Exception
                            r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "An exception was thrown while processing this rows actions: " & ex.ToString, True)
                        End Try
                    Next
                    'Caller.Engine.CachedTableRowIndexCollection(dt.TableName) = 0
                    _Engine.CachedTableRowIndexCollection(dt.TableName) = 0
                End If
            ElseIf Not Caller Is Nothing AndAlso Not Debugger Is Nothing Then
                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "No table resulted from this execution.", True)
            End If
            Return New Runtime.ExecutableResult(Runtime.ExecutableResultEnum.Executed, Nothing)
        End Function

        Public Overrides Function Key() As String
            Return "Action-Execute"
        End Function
    End Class
End Namespace