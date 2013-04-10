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
    Public Class SearchAction
        Inherits ActionBase


#Region "Debugging and Identification: Name,Style,Description"
        Public Overrides Function Description(ByRef act As MessageActionItem) As String
            Dim str As String = ""
            If Not act.Parameters Is Nothing AndAlso act.Parameters.Count > 0 Then
                str &= Utility.HTMLEncode(" Setup and initialization of the Search Integration")
            Else
                str &= " Failure to setup and initialize the Search Integration."
            End If
            Return str
        End Function
        Public Overrides Function Name() As String
            Return "Search"
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
                'action.Parameters.Query = sysGetText(v1);
                'action.Parameters.Title = sysGetText(v2);
                'action.Parameters.Author = sysGetText(v3);
                'action.Parameters.Date = sysGetText(v4);
                'action.Parameters.Querystring = sysGetText(v5);
                'action.Parameters.Key = sysGetText(v6);
                'action.Parameters.Description = sysGetText(v7);
                'action.Parameters.Content = sysGetText(v8);
                Dim strQuery As String = ""
                Dim strTitle As String = ""
                Dim strAuthor As String = ""
                Dim strDate As String = ""
                Dim strQuerystring As String = ""
                Dim strKey As String = ""
                Dim strDescription As String = ""
                Dim strContent As String = ""

                If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONSEARCH_AUTHOR_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONSEARCH_AUTHOR_KEY)).Length > 0 Then
                    strAuthor = CStr(act.Parameters(MessageActionsConstants.ACTIONSEARCH_AUTHOR_KEY))
                End If
                If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONSEARCH_CONTENT_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONSEARCH_CONTENT_KEY)).Length > 0 Then
                    strContent = CStr(act.Parameters(MessageActionsConstants.ACTIONSEARCH_CONTENT_KEY))
                End If
                If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONSEARCH_DATE_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONSEARCH_DATE_KEY)).Length > 0 Then
                    strDate = CStr(act.Parameters(MessageActionsConstants.ACTIONSEARCH_DATE_KEY))
                End If
                If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONSEARCH_DESCRIPTION_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONSEARCH_DESCRIPTION_KEY)).Length > 0 Then
                    strDescription = CStr(act.Parameters(MessageActionsConstants.ACTIONSEARCH_DESCRIPTION_KEY))
                End If
                If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONSEARCH_KEY_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONSEARCH_KEY_KEY)).Length > 0 Then
                    strKey = CStr(act.Parameters(MessageActionsConstants.ACTIONSEARCH_KEY_KEY))
                End If
                If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONSEARCH_QUERY_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONSEARCH_QUERY_KEY)).Length > 0 Then
                    strQuery = CStr(act.Parameters(MessageActionsConstants.ACTIONSEARCH_QUERY_KEY))
                End If
                If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONSEARCH_QUERYSTRING_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONSEARCH_QUERYSTRING_KEY)).Length > 0 Then
                    strQuerystring = CStr(act.Parameters(MessageActionsConstants.ACTIONSEARCH_QUERYSTRING_KEY))
                End If
                If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONSEARCH_TITLE_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONSEARCH_TITLE_KEY)).Length > 0 Then
                    strTitle = CStr(act.Parameters(MessageActionsConstants.ACTIONSEARCH_TITLE_KEY))
                End If

                strAuthor = Caller.Engine.RenderString(sharedds, strAuthor, Caller.Engine.CapturedMessages, False, isPreRender:=False, DebugWriter:=Debugger)
                strContent = Caller.Engine.RenderString(sharedds, strContent, Caller.Engine.CapturedMessages, False, isPreRender:=False, DebugWriter:=Debugger)
                strDate = Caller.Engine.RenderString(sharedds, strDate, Caller.Engine.CapturedMessages, False, isPreRender:=False, DebugWriter:=Debugger)
                strDescription = Caller.Engine.RenderString(sharedds, strDescription, Caller.Engine.CapturedMessages, False, isPreRender:=False, DebugWriter:=Debugger)
                strKey = Caller.Engine.RenderString(sharedds, strKey, Caller.Engine.CapturedMessages, False, isPreRender:=False, DebugWriter:=Debugger)
                strQuery = Caller.Engine.RenderString(sharedds, strQuery, Caller.Engine.CapturedMessages, False, isPreRender:=False, DebugWriter:=Debugger)
                strQuerystring = Caller.Engine.RenderString(sharedds, strQuerystring, Caller.Engine.CapturedMessages, False, isPreRender:=False, DebugWriter:=Debugger)
                strTitle = Caller.Engine.RenderString(sharedds, strTitle, Caller.Engine.CapturedMessages, False, isPreRender:=False, DebugWriter:=Debugger)
                With Caller.Engine.xls
                    .SearchAuthor = strAuthor
                    .SearchContent = strContent
                    .SearchDate = strDate
                    .SearchDescription = strDescription
                    .SearchKey = strKey
                    .SearchLink = strQuerystring
                    .SearchQuery = strQuery
                    .SearchTitle = strTitle
                End With
            End If
            Return New Runtime.ExecutableResult(Runtime.ExecutableResultEnum.Executed, Nothing)
        End Function

        Public Overrides Function Key() As String
            Return "Action-Search"
        End Function
    End Class
End Namespace