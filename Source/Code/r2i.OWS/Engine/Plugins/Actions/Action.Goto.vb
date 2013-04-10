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
Imports r2i.OWS.Framework.Utilities.Compatibility, r2i.OWS.Actions.Utilities
Imports r2i.OWS.Framework.Plugins.Actions
Imports r2i.OWS.Framework
Imports r2i.OWS.Framework.Utilities

Namespace r2i.OWS.Actions
    Public Class GotoAction
        Inherits ActionBase



#Region "Debugging and Identification: Name,Style,Description"
        Public Overrides Function Description(ByRef act As MessageActionItem) As String
            Dim str As String = ""
            If Not act.Parameters Is Nothing AndAlso act.Parameters.Count > 0 Then
                Dim sConfigurationName As String = ""
                If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONGOTO_CONFIGURATIONNAME_KEY) Then sConfigurationName = act.Parameters(MessageActionsConstants.ACTIONGOTO_CONFIGURATIONNAME_KEY)
                Dim sRegionName As String = ""
                If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONGOTO_REGION_KEY) Then sRegionName = act.Parameters(MessageActionsConstants.ACTIONGOTO_REGION_KEY)
                If sConfigurationName.Length > 0 Then
                    str &= " Configuration: " & sConfigurationName
                End If
                If Not sConfigurationName.Length > 0 AndAlso Not sRegionName.Length > 0 Then
                    str &= " Region: " & sRegionName
                End If
            Else
                str &= " No Goto Definition is provided."
            End If
            Return str
        End Function
        Public Overrides Function Name() As String
            Return "Goto"
        End Function
        Public Overrides Function Title(ByRef act As MessageActionItem) As String
            Return "Goto"
        End Function
        Public Overrides Function Style() As String
            Return ""
        End Function
#End Region

        Public Overrides Function Handle_Action(ByRef Caller As RuntimeBase, ByRef sharedds As System.Data.DataSet, ByRef act As MessageActionItem, ByRef previous As Runtime.ActionExecutionResult, ByRef Debugger As Framework.Debugger) As Runtime.ExecutableResult
            Dim rslt As New Runtime.ExecutableResult(Runtime.ExecutableResultEnum.Executed, Nothing)
            If Not act.Parameters Is Nothing Then
                Dim sConfigurationName As String = ""
                If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONGOTO_CONFIGURATIONNAME_KEY) Then sConfigurationName = act.Parameters(MessageActionsConstants.ACTIONGOTO_CONFIGURATIONNAME_KEY)
                Dim sRegionName As String = ""
                If act.Parameters.ContainsKey(MessageActionsConstants.ACTIONGOTO_REGION_KEY) Then sRegionName = act.Parameters(MessageActionsConstants.ACTIONGOTO_REGION_KEY)

                If sRegionName.Length > 0 Then
                    Dim rgn As MessageActionItem = r2i.OWS.Framework.Utilities.Engine.Utility.GetRegion(sConfigurationName, sRegionName, Caller.Engine.xls)
                    If Not rgn Is Nothing Then
                        Dim temporaryList As New Generic.List(Of MessageActionItem)
                        temporaryList.Insert(0, rgn)
                        'MessageActions.SetChildren(act.ChildActions, MessageActionItem.ActionStatusType.DoExecute, Debugger)
                        'Caller.ProcessChildActions(act.ChildActions, Debugger, act.Level + 1, sharedds)
                        rslt = Caller.Execute(temporaryList, Debugger, sharedds)
                    End If
                Else
                    'ALL REGIONS IN CONFIG
                    Dim rgn As Generic.List(Of MessageActionItem) = r2i.OWS.Framework.Utilities.Engine.Utility.GetConfigurationActions(sConfigurationName, Caller.Engine.xls)
                    If Not rgn Is Nothing Then
                        Dim temporaryList As New Generic.List(Of MessageActionItem)
                        temporaryList.InsertRange(0, rgn)
                        'MessageActions.SetChildren(act.ChildActions, MessageActionItem.ActionStatusType.DoExecute, Debugger)
                        'Caller.ProcessChildActions(act.ChildActions, Debugger, act.Level + 1, sharedds)
                        rslt = Caller.Execute(temporaryList, Debugger, sharedds)
                    End If
                End If
            End If
            Return rslt
        End Function

        Public Overrides Function Key() As String
            Return "Action-Goto"
        End Function
    End Class
End Namespace