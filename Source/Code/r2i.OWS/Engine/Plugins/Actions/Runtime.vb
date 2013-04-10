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
    Public Class Runtime
        Inherits r2i.OWS.Framework.RuntimeBase
        Private _Engine As Engine
        Private _Actions As List(Of MessageActionItem)
        Private _FilterField As String
        Private _FilterText As String

        Public Overrides ReadOnly Property FilterField() As String
            Get
                Return _FilterField
            End Get
        End Property
        Public Overrides ReadOnly Property FilterText() As String
            Get
                Return _FilterText
            End Get
        End Property
        Public Overrides ReadOnly Property Engine() As EngineBase
            Get
                Return _Engine
            End Get
        End Property

        Public Overrides ReadOnly Property Actions() As List(Of MessageActionItem)
            Get
                Return _Actions
            End Get
        End Property


        Public Sub New(ByRef RenderingEngine As Engine, Optional ByVal FilterField As String = Nothing, Optional ByVal FilterText As String = Nothing)
            _Engine = RenderingEngine
            _FilterField = FilterField
            _FilterText = FilterText

            'Initialize_Callers()
        End Sub


       

        Public Overrides Sub ExecuteRoot(ByRef Actions As List(Of MessageActionItem), ByRef dbg As Debugger, ByRef DS As DataSet)
            Dim rslt As Runtime.ExecutableResult
            rslt = Me.Execute(Actions, dbg, DS)
            If Not rslt.Value Is Nothing AndAlso rslt.Result = ExecutableResultEnum.Redirected Then
                If Not dbg Is Nothing Then
                    Debugger.ContinueDebugMessage(dbg, "Redirecting to " & rslt.Value & ".", True)
                End If
                If _Engine.xls.skipRedirectActions = False Then
                    Try
                        If _Engine.RequestType = EngineBase.RequestTypeEnum.Ajax Then
                            Utility.ConvertStringToJavascript(rslt.Value, False)
                            _Engine.Response.Write("0".PadLeft(20, " ") & "<script type=""text/javascript"">window.location.href='" & rslt.Value & "';</script>")
                        Else
                        _Engine.Response.AddHeader("P3P", "CP=""CAO PSA OUR""")
                        _Engine.Response.Redirect(rslt.Value)
                        End If
                    Catch exA As Threading.ThreadAbortException
                        'DO NOTHING, WE WANT TO REDIRECT
                    Catch ex As Exception
                        'DO NOTHING, WE WANT TO REDIRECT
                    End Try
                End If
            End If
        End Sub
        Public Overrides Function Execute(ByRef Actions As List(Of MessageActionItem), ByRef dbg As Debugger, ByRef DS As DataSet) As ExecutableResult
            If Not Actions Is Nothing AndAlso Actions.Count > 0 Then
                Dim act As MessageActionItem = Nothing
                Dim lastResult As New ActionExecutionResult
                For Each act In Actions
                    Debugger.StartDebugMessage(act, dbg)
                    Dim rslt As ExecutableResult = ExecuteAction(DS, act, lastResult, dbg)

                    If rslt.Result = ExecutableResultEnum.Redirected Then
                        Return rslt
                    ElseIf rslt.Result = ExecutableResultEnum.Executed Then
                        lastResult.Action = act
                        lastResult.Result = rslt
                    End If

                    Debugger.EndDebugMessage(act, dbg, "")
                Next
            End If
            Return New Runtime.ExecutableResult(ExecutableResultEnum.Executed, Nothing)
        End Function
        Private Function ExecuteAction(ByRef sharedds As DataSet, ByVal act As MessageActionItem, ByRef PreviousAction As ActionExecutionResult, ByRef Debugger As Debugger) As ExecutableResult
            Dim r As ActionBase = Plugins.Manager.GetPlugin(Plugins.PluginTag.Create(Config.Section.Actions.ToString.ToLower, "", act.ActionType))  'Common.GetActionHandler(act.ActionType)
            If Not r Is Nothing Then
                Try
                    Return r.Handle_Action(Me, sharedds, act, PreviousAction, Debugger)
                Catch ex As Exception
                    If Not Debugger Is Nothing Then
                        r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "The Action failed to terminate gracefully: " & ex.ToString, True)
                    End If
                    Return New Runtime.ExecutableResult(ExecutableResultEnum.NotExecuted, Nothing)
                End Try
            Else
                Return New Runtime.ExecutableResult(ExecutableResultEnum.NotExecuted, Nothing)
            End If
        End Function


        ' Recursively copy all files and subdirectories from the
        ' specified source to the specified destination.

        Public Overrides Function Handle_Assignment(ByVal strAction As String, ByVal strName As String, ByVal strValue As String, ByVal iAssignmentType As Integer, ByRef Debugger As r2i.OWS.Framework.Debugger) As Runtime.ExecutableResult
            Dim r As ActionBase = Plugins.Manager.GetPlugin(Plugins.PluginTag.Create(Config.Section.Actions.ToString.ToLower, "", "Action-Assignment"))  'Common.GetActionHandler("Action-Assignment")
            If Not r Is Nothing Then
                Return CType(r, Actions.AssignmentAction).Handle_Assignment(Me, strAction, strName, strValue, iAssignmentType, Debugger)
            Else
                Return Nothing
            End If
        End Function

        Public Overrides Function Handler_Render(ByRef sharedds As DataSet, ByVal act As MessageActionItem, ByRef Debugger As Debugger) As String
            Dim Query As String = _Engine.RenderQuery(sharedds, _FilterField, _FilterText, _Engine.RecordsPerPage, Nothing, Debugger)
            Dim customConnection As String = ""
            Dim tCacheName As String = Nothing
            Dim tCacheTime As String = Nothing
            Dim bCacheShare As Boolean = False
            If Not _Engine.TemplateCacheTime Is Nothing AndAlso _Engine.TemplateCacheTime.Length > 0 Then
                tCacheTime = _Engine.RenderString(sharedds, _Engine.TemplateCacheTime, Nothing, False, False, FilterText:=_FilterText, FilterField:=_FilterField, DebugWriter:=Debugger)
            End If
            If Not _Engine.TemplateCacheName Is Nothing AndAlso _Engine.TemplateCacheName.Length > 0 Then
                tCacheName = _Engine.RenderString(sharedds, _Engine.TemplateCacheName, Nothing, False, False, FilterText:=_FilterText, FilterField:=_FilterField, DebugWriter:=Debugger)
            End If
            If Not _Engine.TemplateCacheShare Is Nothing AndAlso _Engine.TemplateCacheShare.length > 0 Then
                bCacheShare = Utility.CNullBool(_Engine.RenderString(sharedds, _Engine.TemplateCacheShare, Nothing, False, False, FilterText:=_FilterText, FilterField:=_FilterField, DebugWriter:=Debugger))
            End If
            If Not _Engine.xls.customConnection Is Nothing AndAlso _Engine.xls.customConnection.Length > 0 Then
                customConnection = _Engine.RenderString(sharedds, _Engine.xls.customConnection, Nothing, False, False, FilterText:=_FilterText, FilterField:=_FilterField, DebugWriter:=Debugger)
            End If
            Dim ds As DataSet = _Engine.GetData(False, Query, _FilterField, _FilterText, Debugger, True, tCacheName, tCacheTime, bCacheShare, CustomConnection:=customConnection)

            Dim ql As Integer
            If Not Query Is Nothing Then
                ql = Query.Length
            End If

            Dim Writer As New System.IO.StringWriter
            _Engine.TableVariables = sharedds
            If _Engine.OverridePaging Then
                _Engine.Render(ds, ql, _Engine.xls.recordsPerPage, _Engine.PageCurrent, _Engine.xls.enableCustomPaging, Writer, Nothing, , Debugger)
            Else
                _Engine.Render(ds, ql, _Engine.xls.recordsPerPage, 1, _Engine.xls.enableCustomPaging, Writer, Nothing, , Debugger)
            End If


            Query = Nothing

            If Not ds Is Nothing Then
                If Not ds.ExtendedProperties.ContainsKey("isCached") Then
                    ds.Clear()
                    ds.Dispose()
                    ds = Nothing
                End If
            End If

            Handler_Render = Writer.ToString
            Writer.Close()
            Writer = Nothing
        End Function
    End Class
End Namespace