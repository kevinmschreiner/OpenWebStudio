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

Namespace r2i.OWS.Framework.Plugins.Actions
    Public MustInherit Class ActionBase
        Implements iPlugin


        Public MustOverride Function Handle_Action(ByRef Caller As RuntimeBase, ByRef sharedds As DataSet, ByRef act As MessageActionItem, ByRef PreviousResult As RuntimeBase.ActionExecutionResult, ByRef Debugger As r2i.OWS.Framework.Debugger) As RuntimeBase.ExecutableResult
        Public MustOverride Function Key() As String
        Public MustOverride Function Style() As String
        Public MustOverride Function Title(ByRef act As MessageActionItem) As String
        Public Overridable Function Executable() As Boolean
            Return True
        End Function
        Public MustOverride Function Name() As String
        Public MustOverride Function Description(ByRef act As MessageActionItem) As String

        Public Overridable ReadOnly Property ActionOnLoad_Name() As String
            Get
                Return "onActionLoad_" & Me.Name.Replace(" ", "_")
            End Get
        End Property
        Public Overridable ReadOnly Property ActionOnLoad() As String
            Get
                Dim sJS As String = "function " & Me.ActionOnLoad_Name & "(template,action) {" & vbCrLf
                sJS &= "    return '';" & vbCrLf
                sJS &= "}"
                Return sJS
            End Get
        End Property

        Public Overridable ReadOnly Property ActionOnSave_Name() As String
            Get
                Return "onActionSave_" & Me.Name.Replace(" ", "_")
            End Get
        End Property
        Public Overridable ReadOnly Property ActionOnSave() As String
            Get
                Dim sJS As String = "function " & Me.ActionOnSave_Name & "(template,action) {" & vbCrLf
                sJS &= "    return '';" & vbCrLf
                sJS &= "}"
                Return sJS
            End Get
        End Property

        Public Overridable ReadOnly Property ActionOnDelete_Name() As String
            Get
                Return "onActionDelete"
            End Get
        End Property
        Public Overridable ReadOnly Property ActionOnDelete() As String
            Get
                Dim sJS As String = "function " & Me.ActionOnDelete_Name & "(template,action) {" & vbCrLf
                sJS &= "    return window.confirm('Are you sure you want to delete this action?');" & vbCrLf
                sJS &= "}"
                Return sJS
            End Get
        End Property

        Public Overridable ReadOnly Property ActionOnPrint_Name() As String
            Get
                Return "onActionPrint_" & Me.Name.Replace(" ", "_")
            End Get
        End Property
        Public Overridable ReadOnly Property ActionOnPrint() As String
            Get
                Dim sJS As String = "function " & Me.ActionOnPrint_Name & "(template,action) {" & vbCrLf
                sJS &= "    return '';" & vbCrLf
                sJS &= "}"
                Return sJS
            End Get
        End Property

        Public Overridable ReadOnly Property ActionTemplate() As String
            Get
                Return ""
            End Get
        End Property

        Public ReadOnly Property Plugin() As PluginTag Implements iPlugin.Plugin
            Get
                Return PluginTag.Create(Config.Section.Actions.ToString.ToLower, "", Key)
            End Get
        End Property
    End Class
End Namespace
