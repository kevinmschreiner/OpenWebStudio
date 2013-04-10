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
Imports r2i.OWS.Renderers
Imports r2i.OWS.Framework.Utilities
Imports r2i.OWS.Framework
Imports r2i.OWS.Framework.Plugins.Actions
Imports r2i.OWS.Framework.Entities
Imports r2i.OWS.Framework.DataAccess
Imports r2i.OWS.Framework.Utilities.Compatibility
Imports r2i.OWS.Actions

Imports r2i.OWS.Actions.Utilities
Imports System.Web

Namespace r2i.OWS.Actions
    Public Class FilterAction
        Inherits ActionBase



#Region "Debugging and Identification: Name,Style,Description"
        Public Overrides Function Description(ByRef act As MessageActionItem) As String
            Dim str As String = "Options"
            Return str
        End Function
        Public Overrides Function Name() As String
            Return "Filter"
        End Function
        Public Overrides Function Title(ByRef act As MessageActionItem) As String
            Return "Filter"
        End Function
        Public Overrides Function Style() As String
            Return ""
        End Function
#End Region

        Public Overrides Function Handle_Action(ByRef Caller As RuntimeBase, ByRef sharedds As System.Data.DataSet, ByRef act As MessageActionItem, ByRef previous As Runtime.ActionExecutionResult, ByRef Debugger As Framework.Debugger) As Runtime.ExecutableResult
            Try
                If Not act.Parameters Is Nothing Then
                    Dim parms As SerializableDictionary(Of String, Object) = act.Parameters

                    Dim Options As MessageAction_Filter_Options
                    Dim mappings As Object = Utility.GetDictionaryObject(parms, MessageActionsConstants.ACTIONFILTER_OPTIONS_KEY)
                    If TypeOf mappings Is Newtonsoft.Json.JavaScriptArray Then
                        Options = New MessageAction_Filter_Options(CType(mappings, Newtonsoft.Json.JavaScriptArray))
                    Else
                        Options = New MessageAction_Filter_Options(mappings.ToString)
                    End If

                    Caller.Engine.xls.searchItems = New Generic.List(Of SearchOptionItem)
                    If Not Options.Options Is Nothing AndAlso Options.Options.Count > 0 Then

                        Dim i As Integer = 0
                        Dim mo As MessageAction_Filter_Options.MessageAction_Filter_Option
                        For Each mo In Options.Options
                            Dim si As New SearchOptionItem
                            si.SearchField = mo.Field
                            si.SearchOption = mo.Option
                            si.Index = i
                            Caller.Engine.xls.searchItems.Add(si)
                            i += 1
                        Next
                    Else
                        Caller.Engine.xls.searchItems = Nothing
                    End If

                End If
            Catch ex As Exception
                If Not Debugger Is Nothing Then
                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Unabled to handle filter action: " & ex.ToString(), True)
                End If
            End Try
            Return New Runtime.ExecutableResult(Runtime.ExecutableResultEnum.Executed, Nothing)
        End Function

        Public Overrides Function Key() As String
            Return "Action-Filter"
        End Function
    End Class
End Namespace