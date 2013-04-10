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
Imports System
Imports System.IO
Imports System.Text
Imports r2i.OWS.Framework.Plugins.Actions
Namespace r2i.OWS.Framework
    Public Class Debugger
        Implements IDisposable
        Private ReadOnly dbgWriter As StringWriter = Nothing
        Private Shared rnd As New Random

        Public Sub New()
            dbgWriter = New StringWriter()
        End Sub
        Public Sub Dispose() Implements IDisposable.Dispose
            GC.SuppressFinalize(Me)
        End Sub

        Public Sub AppendLine(ByVal Value As String)
            dbgWriter.WriteLine(Value)
        End Sub
        Public Sub AppendHeader(ByVal ModuleID As String, ByVal Title As String, ByVal Group As String, ByVal isPre As Boolean)
            Dim ticks As String = Me.Counter
            dbgWriter.WriteLine("<div class=""owsdebug_" & Group.ToLower & """><span>" & Title & ":</span><a href=""javascript:OWSDebug.Toggle('" & ModuleID.ToString & "','" & ticks & "');"">Expand/Collapse</a><br><div id=""xDbg" & ModuleID.ToString & "x" & ticks & """ class=""owsdebug_" & Group.ToLower & "_content"">")
            If isPre Then dbgWriter.WriteLine("<pre>")
        End Sub
        Public Sub AppendStamp(ByVal AdditionalMessage As String)
            If Not AdditionalMessage Is Nothing Then
                AdditionalMessage = " - " & AdditionalMessage
            Else
                AdditionalMessage = ""
            End If
            dbgWriter.WriteLine("<div class=""owsdebug_stamp"">" & Now.ToShortDateString & " " & Now.ToString("h:mm:ss tt") & AdditionalMessage & "</div>")
        End Sub
        Public Sub AppendBlock(ByVal ModuleID As String, ByVal Title As String, ByVal Group As String, ByVal isPre As Boolean, ByVal Content As String, ByVal encode As Boolean)
            AppendHeader(ModuleID, Title, Group, isPre)
            If encode Then
                dbgWriter.WriteLine(Content.Replace("<", "&lt;").Replace(">", "&gt;"))
            Else
                dbgWriter.WriteLine(Content)
            End If
            AppendFooter(isPre)
        End Sub
        Public Sub AppendFooter(ByVal isPre As Boolean)
            If isPre Then dbgWriter.WriteLine("</pre>")
            dbgWriter.WriteLine("</div></div>")
        End Sub
        Public Sub Append(ByVal Value As String)
            dbgWriter.Write(Value)
        End Sub
        Public Sub Close()
            dbgWriter.Close()
        End Sub

        Public Function GetStringBuilder() As Text.StringBuilder
            Return dbgWriter.GetStringBuilder
        End Function
        Public Overloads Overrides Function ToString() As String
            Return dbgWriter.ToString()
        End Function

        Private Function Counter() As String
            Return rnd.Next(0, Integer.MaxValue).ToString.PadLeft(10, "0"c) & rnd.Next(0, Integer.MaxValue).ToString().PadLeft(10, "0"c)
        End Function

        'GLOBAL SHARED FUNCTIONS
        Public Shared Function DescribeActionMessage(ByVal act As MessageActionItem) As String
            Dim r As r2i.OWS.Framework.Plugins.Actions.ActionBase = Plugins.Manager.GetPlugin(Plugins.PluginTag.Create(Config.Section.Actions.ToString.ToLower, "", act.ActionType)) 'r2i.OWS.Framework.Utilities.Utility.GetActionHandler(act.ActionType)
            If Not r Is Nothing Then
                Return r.Description(act)
            End If
            Return ""
        End Function
        Public Shared Function TitleActionMessage(ByVal act As MessageActionItem) As String
            Dim r As r2i.OWS.Framework.Plugins.Actions.ActionBase = Plugins.Manager.GetPlugin(Plugins.PluginTag.Create(Config.Section.Actions.ToString.ToLower, "", act.ActionType)) 'r2i.OWS.Framework.Utilities.Utility.GetActionHandler(act.ActionType)
            If Not r Is Nothing Then
                Return r.Title(act)
            End If
            Return ""
        End Function
        Public Shared Sub StartDebugMessage(ByVal act As MessageActionItem, ByRef debugger As Debugger)
            If Not debugger Is Nothing Then
                Try
                    If Not act Is Nothing AndAlso Not act.ActionType Is Nothing AndAlso Not act.ActionType.ToUpper = "ACTION-COMMENT" Then
                        Dim sLevel As String = ""
                        sLevel = "<div style=""margin-left:15px;"">"
                        Dim message As String = DescribeActionMessage(act)
                        Dim title As String = TitleActionMessage(act)
                        debugger.AppendLine(sLevel & "<a href=""#"" onclick=""OWSDebug.Toggle('0','" & act.Index & "');return false;"">" & act.Index & ". (Expand/Collapse)</a>" & "<b>" & title & "</b> " & "<i>" & message & "</i>" & "<div id=""xDbg0x" & act.Index & """ style=""display:block;"">")
                    End If
                Catch ex As Exception
                End Try
            End If
        End Sub
        'Public Shared Sub StartDebugMessage(ByVal Header As String, ByVal message As String, ByVal index As String, ByRef debugger As Debugger)
        '    If Not debugger Is Nothing Then
        '        If Not Header Is Nothing Then
        '            Dim sLevel As String = ""
        '            sLevel = "<div style=""margin-left:15px;"">"
        '            debugger.AppendLine(sLevel & "<a href=""#"" onclick=""OWSDebug.Toggle('0','" & index & "');return false;"">" & index & ". (Expand/Collapse)</a>" & "<b>" & Header & "</b> " & "<i>" & message & "</i>" & "<div id=""xDbg0x" & index & """ style=""display:block;"">")
        '        End If
        '    End If
        'End Sub
        Public Shared Sub ContinueDebugMessage(ByRef debugger As Debugger, ByVal message As String, ByVal reformat As Boolean)
            If Not debugger Is Nothing Then
                Try
                    If reformat Then
                        debugger.AppendLine("<LI>" & r2i.OWS.Framework.Utilities.Utility.HTMLEncode(message) & "</LI>")
                    Else
                        debugger.AppendLine("<LI>" & message & "</LI>")
                    End If
                Catch ex As Exception
                End Try
            End If
        End Sub
        Public Shared Sub EndDebugMessage(ByVal act As MessageActionItem, ByRef debugger As Debugger, ByVal message As String)
            If Not debugger Is Nothing Then
                Try
                    If Not act Is Nothing AndAlso Not act.ActionType Is Nothing AndAlso Not act.ActionType.ToUpper = "ACTION-COMMENT" Then
                        debugger.AppendLine("</div></div>" & vbCrLf)
                    End If
                Catch ex As Exception
                End Try
            End If
        End Sub
        'Public Shared Sub EndDebugMessage(ByRef debugger As Debugger)
        '    If Not debugger Is Nothing Then
        '        debugger.AppendLine("</div></div>" & vbCrLf)
        '    End If
        'End Sub
    End Class
End Namespace