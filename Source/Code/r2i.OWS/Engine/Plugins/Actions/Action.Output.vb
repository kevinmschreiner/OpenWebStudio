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
Imports r2i.OWS.Framework.Utilities.Compatibility
Imports r2i.OWS.Framework.DataAccess
Imports r2i.OWS.Actions.Utilities

Namespace r2i.OWS.Actions
    Public Class OutputAction
        Inherits ActionBase


#Region "Debugging and Identification: Name,Style,Description"
        Public Overrides Function Description(ByRef act As MessageActionItem) As String
            Dim str As String = ""
            If Not act.Parameters Is Nothing AndAlso act.Parameters.Count > 0 Then
                str &= Utility.CNullStr(act.Parameters.Item(MessageActionsConstants.ACTIONOUTPUT_OUTPUTTYPE_KEY)) & " "
                Select Case LCase(Utility.CNullStr(act.Parameters.Item(MessageActionsConstants.ACTIONOUTPUT_OUTPUTTYPE_KEY)))
                    Case "excel", "complete excel", "word", "complete word"
                        str &= " saved as <i>" & Utility.HTMLEncode(Utility.CNullStr(act.Parameters.Item(MessageActionsConstants.ACTIONOUTPUT_FILENAME_KEY))) & "</i>"
                    Case "delimited", "complete delimited"
                        str &= " delimited with <b>" & Utility.HTMLEncode(Utility.CNullStr(act.Parameters.Item(MessageActionsConstants.ACTIONOUTPUT_DELIMITER_KEY))) & "</b> and saved as <i>" & Utility.HTMLEncode(Utility.CNullStr(act.Parameters.Item(MessageActionsConstants.ACTIONOUTPUT_FILENAME_KEY))) & "</i>"
                    Case "report"
                        'TODO: IS THIS USED?
                        str &= " using <b>" & Utility.HTMLEncode(Utility.CNullStr(act.Parameters.Item(MessageActionsConstants.ACTIONOUTPUT_DELIMITER_KEY))) & "</b> as the template into <b>" & Utility.HTMLEncode(Utility.CNullStr(act.Parameters.Item(MessageActionsConstants.ACTIONOUTPUT_DELIMITER_KEY))) & "</b> and saved as <i>" & Utility.HTMLEncode(Utility.CNullStr(act.Parameters.Item(MessageActionsConstants.ACTIONOUTPUT_FILENAME_KEY))) & "</i>"
                End Select
            Else
                str &= " (no parameters defined)"
            End If
            Return str
        End Function
        Public Overrides Function Name() As String
            Return "Output"
        End Function
        Public Overrides Function Title(ByRef act As MessageActionItem) As String
            Return Name()
        End Function
        Public Overrides Function Style() As String
            Return ""
        End Function
#End Region

        Public Overrides Function Handle_Action(ByRef Caller As RuntimeBase, ByRef sharedds As System.Data.DataSet, ByRef act As MessageActionItem, ByRef previous As Runtime.ActionExecutionResult, ByRef Debugger As Framework.Debugger) As Runtime.ExecutableResult
            If Not act.Parameters Is Nothing AndAlso act.Parameters.ContainsKey(MessageActionsConstants.ACTIONOUTPUT_OUTPUTTYPE_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONOUTPUT_OUTPUTTYPE_KEY)).Length > 0 Then
                If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONOUTPUT_FILENAME_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONOUTPUT_FILENAME_KEY)).Length > 0 Then
                    Select Case LCase(CStr(act.Parameters(MessageActionsConstants.ACTIONOUTPUT_OUTPUTTYPE_KEY)))
                        Case "excel"
                            Dim value As String = Caller.Engine.RenderString(sharedds, CStr(act.Parameters(MessageActionsConstants.ACTIONOUTPUT_FILENAME_KEY)), Caller.Engine.CapturedMessages, False, isPreRender:=False, DebugWriter:=Debugger)
                            Caller.Engine.xls.OutputType(Settings.RenderType.Excel, FileName:=value)
                        Case "complete excel"
                            Dim value As String = Caller.Engine.RenderString(sharedds, CStr(act.Parameters(MessageActionsConstants.ACTIONOUTPUT_FILENAME_KEY)), Caller.Engine.CapturedMessages, False, isPreRender:=False, DebugWriter:=Debugger)
                            Caller.Engine.xls.OutputType(Settings.RenderType.Excel_Complete, FileName:=value)
                        Case "word"
                            Dim value As String = Caller.Engine.RenderString(sharedds, CStr(act.Parameters(MessageActionsConstants.ACTIONOUTPUT_FILENAME_KEY)), Caller.Engine.CapturedMessages, False, isPreRender:=False, DebugWriter:=Debugger)
                            Caller.Engine.xls.OutputType(Settings.RenderType.Word, FileName:=value)
                        Case "complete word"
                            Dim value As String = Caller.Engine.RenderString(sharedds, CStr(act.Parameters(MessageActionsConstants.ACTIONOUTPUT_FILENAME_KEY)), Caller.Engine.CapturedMessages, False, isPreRender:=False, DebugWriter:=Debugger)
                            Caller.Engine.xls.OutputType(Settings.RenderType.Word_Complete, FileName:=value)
                        Case "delimited"
                            If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONOUTPUT_DELIMITER_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONOUTPUT_DELIMITER_KEY)).Length > 0 Then
                                Dim delimiter As String = Caller.Engine.RenderString(sharedds, CStr(act.Parameters(MessageActionsConstants.ACTIONOUTPUT_DELIMITER_KEY)), Caller.Engine.CapturedMessages, False, isPreRender:=False, DebugWriter:=Debugger)
                                Dim value As String = Caller.Engine.RenderString(sharedds, CStr(act.Parameters(MessageActionsConstants.ACTIONOUTPUT_FILENAME_KEY)), Caller.Engine.CapturedMessages, False, isPreRender:=False, DebugWriter:=Debugger)
                                Caller.Engine.xls.OutputType(Settings.RenderType.Delimited, Delimiter:=delimiter, FileName:=value)
                            End If
                        Case "complete delimited"
                            If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONOUTPUT_DELIMITER_KEY) AndAlso CStr(act.Parameters(MessageActionsConstants.ACTIONOUTPUT_DELIMITER_KEY)).Length > 0 Then
                                Dim delimiter As String = Caller.Engine.RenderString(sharedds, CStr(act.Parameters(MessageActionsConstants.ACTIONOUTPUT_DELIMITER_KEY)), Caller.Engine.CapturedMessages, False, isPreRender:=False, DebugWriter:=Debugger)
                                Dim value As String = Caller.Engine.RenderString(sharedds, CStr(act.Parameters(MessageActionsConstants.ACTIONOUTPUT_FILENAME_KEY)), Caller.Engine.CapturedMessages, False, isPreRender:=False, DebugWriter:=Debugger)
                                Caller.Engine.xls.OutputType(Settings.RenderType.Delimited_Complete, Delimiter:=delimiter, FileName:=value)
                            End If
                        Case "report"
                            Caller.Engine.xls.OutputType(Settings.RenderType.Report)
                    End Select
                    If Not Debugger Is Nothing Then
                        r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Output: Output Type Set to " & Caller.Engine.xls.OutputType.ToString, True)
                    End If
                End If
            End If
            Return New Runtime.ExecutableResult(Runtime.ExecutableResultEnum.Executed, Nothing)
        End Function

        Public Overrides Function Key() As String
            Return "Action-Output"
        End Function
    End Class
End Namespace