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
Imports r2i.OWS.Framework.Plugins.Actions

Namespace r2i.OWS.Framework
    Public MustInherit Class RuntimeBase
        Public Enum ExecutableResultEnum
            Executed
            NotExecuted
            Redirected
            Failed
            Aborted
            Terminated
        End Enum
        Public Structure ExecutableResult
            Public Sub New(ByVal result As ExecutableResultEnum, ByVal value As String)
                Me.Result = result
                Me.Value = value
                Me.Error = Nothing
            End Sub
            Public Sub New(ByVal result As ExecutableResultEnum, ByVal value As String, ByVal Err As Exception)
                Me.Result = result
                Me.Value = value
                Me.Error = Err
            End Sub
            Public Result As ExecutableResultEnum
            Public Value As String
            Public [Error] As Exception
        End Structure
        Public Structure QueryResult
            Public Sub New(ByVal result As ExecutableResultEnum, ByVal value As DataSet)
                Me.Result = result
                Me.Value = value
                Me.Error = Nothing
            End Sub
            Public Sub New(ByVal result As ExecutableResultEnum, ByVal value As DataSet, ByVal Err As Exception)
                Me.Result = result
                Me.Value = value
                Me.Error = Err
            End Sub
            Public Result As ExecutableResultEnum
            Public Value As DataSet
            Public [Error] As Exception
        End Structure
        Public Structure ActionExecutionResult
            Public Action As MessageActionItem
            Public Result As ExecutableResult
        End Structure
       
        Public ThreadName As String = Nothing
        Public CurrentMailMessage As MailMessage = Nothing
        Public CurrentMailMessageFolder As String = Nothing

        Public MustOverride ReadOnly Property FilterField() As String
        Public MustOverride ReadOnly Property FilterText() As String
        Public MustOverride ReadOnly Property Engine() As EngineBase
        Public MustOverride ReadOnly Property Actions() As List(Of MessageActionItem)


        'Public Sub New(ByRef RenderingEngine As Engine, Optional ByVal FilterField As String = Nothing, Optional ByVal FilterText As String = Nothing)
        '    _Engine = RenderingEngine
        '    _FilterField = FilterField
        '    _FilterText = FilterText

        '    'Initialize_Callers()
        'End Sub


        Public MustOverride Sub ExecuteRoot(ByRef Actions As List(Of MessageActionItem), ByRef debugger As Debugger, ByRef DS As DataSet)
        Public MustOverride Function Execute(ByRef Actions As List(Of MessageActionItem), ByRef debugger As Debugger, ByRef DS As DataSet) As ExecutableResult
        Public MustOverride Function Handle_Assignment(ByVal strAction As String, ByVal strName As String, ByVal strValue As String, ByVal iAssignmentType As Integer, ByRef Debugger As r2i.OWS.Framework.Debugger) As ExecutableResult
        Public MustOverride Function Handler_Render(ByRef sharedds As DataSet, ByVal act As MessageActionItem, ByRef Debugger As Debugger) As String
    End Class
End Namespace
