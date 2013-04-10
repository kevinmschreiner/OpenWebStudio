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
Imports r2i.OWS.Framework
Imports r2i.OWS.Framework.Plugins
Imports r2i.OWS.Framework.Plugins.Renderers
Imports r2i.OWS.Framework.Utilities

Namespace r2i.OWS.Renderers
    Public Class RenderCoalesce
        Inherits RenderBase

        Public Overrides ReadOnly Property RenderTag() As String
            Get
                Return "COALESCE"
            End Get
        End Property

        Public Overrides ReadOnly Property RenderType() As RenderTypes
            Get
                Return RenderTypes.Variable
            End Get
        End Property

        Public Overrides ReadOnly Property CanPreRender() As Boolean
            Get
                Return True
            End Get
        End Property

        Public Overrides ReadOnly Property PreRenderTag() As String
            Get
                Return "#"
            End Get
        End Property

        Public Overrides Function Handle_Render(ByRef Caller as EngineBase, ByVal Index As Integer, ByRef Source As String, ByRef DS As System.Data.DataSet, ByRef DR As System.Data.DataRow, ByRef RuntimeMessages As System.Collections.Generic.SortedList(Of String, String), ByVal NullReturn As Boolean, ByVal NullOverride As Boolean, ByVal ProtectSession As Boolean, ByVal SessionDelimiter As String, ByVal useSessionQuotes As Boolean, ByVal useAggregations As Boolean, ByRef FilterText As String, ByRef FilterField As String, ByRef Debugger As r2i.OWS.Framework.Debugger) As Boolean
            Dim REPLACED As Boolean = False
            Dim VALUE As String = Nothing
            Dim parameters As String() = ParameterizeString(Source, ","c, """"c, "\"c)
            If Not parameters Is Nothing AndAlso parameters.Length > 1 Then
                Dim formatters As Integer = 0
                Dim isValid As Boolean = False
                If parameters.Length > 1 Then
                    Dim i As Integer = 1
                    Dim maxi As Integer = parameters.Length - 1
                    If parameters.Length > 2 Then
                        While parameters(maxi).StartsWith("{")
                            formatters += 1
                            maxi -= 1
                        End While
                        'If parameters(maxi).StartsWith("{") Then
                        '    FORMATTER = parameters(maxi)
                        '    maxi -= 1
                        'End If
                    End If
                    If maxi Mod 2 = 0 Then
                        i = 1

                        While i < maxi
                            isValid = False
                            Dim isNullAllowed As Boolean = False
                            Dim n As String = parameters(i)
                            Dim v As String = parameters(i + 1)

                            If Not v Is Nothing AndAlso v.Length > 0 AndAlso Not v.ToUpper = "TEXT" Then
                                If v.ToUpper.StartsWith("EMPTY") = True Then
                                    isNullAllowed = True
                                    If v.ToUpper.StartsWith("EMPTY ") Then
                                        v = v.Substring(6)
                                    Else
                                        v = v.Substring(5)
                                    End If
                                End If

                                VALUE = n & "," & v
                            ElseIf v Is Nothing OrElse v.Length = 0 Then
                                VALUE = n
                            Else
                                VALUE = n
                                isValid = True
                            End If

                            If Not isValid Then
                                '''''isValid = RenderString_Variable(VALUE, DS, DR, RuntimeMessages, NullReturn, ProtectSession, SessionDelimiter, useSessionQuotes, FilterText, FilterField, isNullAllowed)
                                'Dim rv As RenderBase = Common.GetRenderer("", RenderTypes.Variable)
                                Dim rv As RenderBase = Plugins.Manager.GetPlugin(Plugins.PluginTag.Create(Config.Section.Tokens.ToString.ToLower, RenderTypes.Variable.ToString.ToLower, ""))
                                Dim bFirewall As Boolean = False
                                If Not VALUE Is Nothing AndAlso VALUE.Length > 0 AndAlso VALUE(0) = "|"c Then
                                    bFirewall = True
                                    VALUE = VALUE.Remove(0, 1)
                                End If
                                isValid = rv.Handle_Render(Caller, Index, VALUE, DS, DR, RuntimeMessages, NullReturn, isNullAllowed, ProtectSession, SessionDelimiter, useSessionQuotes, True, FilterText, FilterField, Debugger)
                                If bFirewall AndAlso isValid Then
                                    'NOTE: THIS IS A BIT UNCLEAR AS V SHOULD BE LOADED BY THE RENDERER
                                    Firewall.Firewall(VALUE, True, Utilities.Firewall.FirewallDirectiveEnum.None, False)
                                End If


                                If VALUE Is Nothing Then
                                    isNullAllowed = False
                                    VALUE = ""
                                ElseIf VALUE.Length = 0 And isNullAllowed Then
                                    isNullAllowed = True
                                Else
                                    isNullAllowed = False
                                End If
                            End If

                            If isValid AndAlso (VALUE.Length > 0 OrElse isNullAllowed) Then
                                isValid = True
                                Exit While
                            Else
                                isValid = False
                            End If


                            i += 2
                        End While

                        If formatters > 0 AndAlso isValid Then
                            While formatters > 0
                                maxi += 1
                                formatters -= 1

                                Dim bval As Boolean = False
                                'bval = RenderString_Format_Value(Index, VALUE, parameters(maxi), VALUE, DS, DR, RuntimeMessages, NullReturn, ProtectSession, SessionDelimiter, useSessionQuotes, FilterText, FilterField)
                                Dim rf As New Renderers.RenderFormat
                                bval = rf.RenderString_Format_Value(Caller, Index, VALUE, parameters(maxi), VALUE, DS, DR, RuntimeMessages, NullReturn, ProtectSession, SessionDelimiter, useSessionQuotes, FilterText, FilterField)
                            End While
                        End If
                    End If
                    REPLACED = isValid
                End If
            End If
            If Not REPLACED Then
                VALUE = ""
                REPLACED = True
            End If
            Source = VALUE
            Return REPLACED
        End Function
    End Class
End Namespace