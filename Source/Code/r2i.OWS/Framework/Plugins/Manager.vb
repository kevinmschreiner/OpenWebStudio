'<LICENSE>
'   Open Web Studio - http://www.openwebstudio.com
'   Copyright (c) 2006-2008
'   by R2 Integrated Inc. ( http://www.r2integrated.com )
'   
'   Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
'   documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
'   the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
'   to permit persons to whom the Software is furnished to do so, subject to the following conditions:
'   
'   The above copyright notice and this permission notice shall be included in all copies or substantial portions 
'   of the Software.
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
Imports r2i.OWS.Framework.Plugins
Imports r2i.OWS.Framework.Utilities.JSON
Imports r2i.OWS.Framework.Plugins.Renderers
Imports r2i.OWS.Framework.Utilities.Compatibility
Imports r2i.OWS.Framework.Utilities
Imports System.Reflection
Imports System.Web
Imports System.Web.UI
Imports r2i.OWS.Newtonsoft.Json
Namespace r2i.OWS.Framework.Plugins
    Public Class Manager

        Private Shared Function LoadInstance(ByVal name As String, ByVal className As String) As Object
            Dim asmb As Assembly = Assembly.Load(name)
            Return asmb.CreateInstance(className)
        End Function


        Private Shared s_Plugins As Generic.Dictionary(Of String, iPlugin)
        Private Shared s_PluginErrors As Generic.Dictionary(Of String, String)
        Private Shared s_PluginMutex As New Threading.Mutex
        Private Shared ReadOnly Property Items() As Generic.Dictionary(Of String, iPlugin)
            Get
                If s_Plugins Is Nothing Then
                    Dim b As Boolean = False
                    Try
                        s_PluginMutex.WaitOne(-1, False)
                        b = True
                    Catch ex As Exception

                    End Try
                    If b AndAlso s_Plugins Is Nothing Then
                        Try
                            s_Plugins = New Generic.Dictionary(Of String, iPlugin)
                            Dim cObject As r2i.OWS.Framework.Config.SectionItem
                            For Each cObject In r2i.OWS.Framework.Config.Items(Config.Section.Actions, Config.SectionType.Runtime)
                                Try
                                    AddPlugin(DirectCast(cObject.Load(), Actions.ActionBase))
                                Catch ex As Exception
                                    AddPluginException(cObject.Type & ", " & cObject.Name, ex)
                                End Try
                            Next
                            For Each cObject In r2i.OWS.Framework.Config.Items(Config.Section.Formats, Config.SectionType.Runtime)
                                Try
                                    AddPlugin(DirectCast(cObject.Load(), Formatters.FormatterBase))
                                Catch ex As Exception
                                    AddPluginException(cObject.Type & ", " & cObject.Name, ex)
                                End Try
                            Next
                            For Each cObject In r2i.OWS.Framework.Config.Items(Config.Section.Tokens, Config.SectionType.Runtime)
                                Try
                                    AddPlugin(DirectCast(cObject.Load(), Renderers.RenderBase))
                                Catch ex As Exception
                                    AddPluginException(cObject.Type & ", " & cObject.Name, ex)
                                End Try
                            Next
                            For Each cObject In r2i.OWS.Framework.Config.Items(Config.Section.Queryies, Config.SectionType.Runtime)
                                Try
                                    AddPlugin(DirectCast(cObject.Load(), Queries.QueryBase))
                                Catch ex As Exception
                                    AddPluginException(cObject.Type & ", " & cObject.Name, ex)
                                End Try
                            Next
                        Catch ex As Exception
                        End Try
                        Try
                            s_PluginMutex.ReleaseMutex()
                            b = True
                        Catch ex As Exception

                        End Try
                    End If
                End If
                Return s_Plugins
            End Get
        End Property
        Private Shared Sub AddPluginException(ByVal Source As String, ByVal ex As Exception)
            If s_PluginErrors Is Nothing Then
                s_PluginErrors = New Dictionary(Of String, String)
            End If
            If Not s_PluginErrors.ContainsKey(Source) Then
                s_PluginErrors.Add(Source, ex.ToString)
            End If
        End Sub
        Public Shared ReadOnly Property PluginException(ByVal Type As String, ByVal Name As String) As String
            Get
                If Not s_PluginErrors Is Nothing AndAlso s_PluginErrors.ContainsKey(Type & ", " & Name) Then
                    Return s_PluginErrors(Type & ", " & Name)
                End If
                Return Nothing
            End Get
        End Property
        Public Shared ReadOnly Property PluginExceptions() As Dictionary(Of String, String)
            Get
                Return s_PluginErrors
            End Get
        End Property


        Private Shared Sub AddPlugin(ByVal Plugin As iPlugin)
            Dim i As Integer
            If Plugin.Plugin.Length > 0 Then
                For i = 0 To Plugin.Plugin.Length - 1
                    If s_Plugins.ContainsKey(Plugin.Plugin.Key(i).ToLower) Then
                        s_Plugins.Remove(Plugin.Plugin.Key(i).ToLower)
                    End If
                    s_Plugins.Add(Plugin.Plugin.Key(i).ToLower, Plugin)
                Next
            End If
        End Sub
        Public Shared Function GetPlugin(ByVal tag As PluginTag) As iPlugin
            If tag.Length > 0 Then
                'ASSUME WE ARE ALWAYS LOOKING FOR THE FIRST KEY BECAUSE A LIST OF KEYS WOULD BE UNKNOWN TO THE RENDERER
                If Items.ContainsKey(tag.Key(0).ToLower) Then
                    Return Items(tag.Key(0).ToLower)
                Else
                    If tag.TypeKey = Config.Section.Tokens.ToString.ToLower AndAlso tag.Name(0).Length > 0 AndAlso Char.IsSymbol(tag.Name(0), 0) AndAlso Items.ContainsKey(tag.AssembleKey(tag.Name(0)(0)).ToLower()) Then
                        Return Items(tag.AssembleKey(tag.Name(0)(0)).ToLower())
                    End If
                End If
            End If
            Return Nothing

            'If Items.ContainsKey(key.ToString(True).ToLower) Then
            '    Return Items(key.ToString(True).ToLower)
            'Else
            '    If key.TypeKey = Config.Section.Tokens.ToString.ToLower AndAlso key.PrimaryKey.Length > 1 AndAlso Char.IsSymbol(key.PrimaryKey, 0) Then
            '        Dim k As New PluginTag(key.TypeKey, key.SubTypeKey, key.PrimaryKey(0))
            '        If Items.ContainsKey(k.ToString(True).ToLower) Then
            '            Return Items(k.ToString(True).ToLower)
            '        End If
            '    End If
            '    Return Nothing
            'End If
        End Function
    End Class
End Namespace
