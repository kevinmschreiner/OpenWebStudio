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
Imports r2i.OWS.Framework.Entities
Imports r2i.OWS.Framework
Imports System.Collections.Generic
Imports r2i.OWS.Framework.Utilities
Imports r2i.OWS.Framework.Plugins.Renderers
Imports r2i.OWS.Framework.Plugins.Formatters

Namespace r2i.OWS.Renderers
    Public Class RenderFormat
        Inherits RenderBase

        Public Overrides ReadOnly Property RenderTag() As String
            Get
                Return "FORMAT"
            End Get
        End Property

        Public Overrides ReadOnly Property RenderType() As RenderTypes
            Get
                Return RenderTypes.Variable
            End Get
        End Property

        Public Overrides ReadOnly Property PreRenderTag() As String
            Get
                Return "$"
            End Get
        End Property

        Public Overrides Function Handle_Render(ByRef Caller as EngineBase, ByVal Index As Integer, ByRef Source As String, ByRef DS As System.Data.DataSet, ByRef DR As System.Data.DataRow, ByRef RuntimeMessages As System.Collections.Generic.SortedList(Of String, String), ByVal NullReturn As Boolean, ByVal NullOverride As Boolean, ByVal ProtectSession As Boolean, ByVal SessionDelimiter As String, ByVal useSessionQuotes As Boolean, ByVal useAggregations As Boolean, ByRef FilterText As String, ByRef FilterField As String, ByRef Debugger As r2i.OWS.Framework.Debugger) As Boolean
            'THIS NEEDS TO BE CHANGED TO CHECK FOR EMBEDDED BRACES.
            'CHECK FOR FORMATS
            'Try
            Dim REPLACED As Boolean = False
            Dim parameters As String() = ParameterizeString(Source, ","c, """"c, "\"c)
            If Not parameters Is Nothing AndAlso parameters.Length > 1 Then
                Dim VALUE As String = Nothing
                Dim FORMATTER As String = Nothing
                Dim isValid As Boolean = False
                If parameters(0).Length > 0 Then
                    If parameters(0) = "$" OrElse parameters(0).ToUpper = "FORMAT" Then
                        'THIS STRING IS IN THE FORMAT $,VALUE,FORMATTER OR FORMAT,VALUE,FORMATTER
                        If parameters.Length > 1 Then
                            VALUE = parameters(1)
                            If parameters.Length > 2 Then
                                If parameters(2).StartsWith("{") Then
                                    FORMATTER = ParameterMerge(CType(parameters, Array), Source, 2)
                                Else
                                    'THIS IS A FORMAT/$,Name,Collection,Format
                                    Dim COLLECTION As String = parameters(2)
                                    Dim initialValue As String = VALUE
                                    If COLLECTION.Length > 0 Then
                                        If Not COLLECTION.ToUpper = "TEXT" Then
                                            VALUE = "[" & VALUE & "," & COLLECTION & "]"
                                        End If
                                    Else
                                        VALUE = "[" & VALUE & "]"
                                    End If
                                    Dim b As Boolean = Caller.RenderString(Index, VALUE, New Char() {"["c, "{"c}, New Char() {"]"c, "}"c}, "\"c, DS, DR, RuntimeMessages, False, False, NullReturn:=NullReturn, ProtectSession:=ProtectSession, SessionDelimiter:=SessionDelimiter, useSessionQuotes:=useSessionQuotes)
                                    If Not b Then
                                        VALUE = initialValue
                                    End If
                                    FORMATTER = ParameterMerge(CType(parameters, Array), Source, 3)

                                    If COLLECTION.ToUpper = "SESSION" OrElse COLLECTION.ToUpper = "S" AndAlso FORMATTER.ToUpper = "{LENGTH}" Then
                                        If Not Caller.Session(VALUE) Is Nothing AndAlso TypeOf (Caller.Session(VALUE)) Is ArrayList Then
                                            Source = CType(Caller.Session(VALUE), ArrayList).Count.ToString
                                            REPLACED = True
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    Else
                        'THIS STRING IS IN THE FORMAT $COLUMN,FORMATTER (OLD STYLE)
                        Dim addBraces As Boolean = False
                        Dim COLLECTION As String
                        VALUE = parameters(0).Remove(0, 1)
                        If Not VALUE.StartsWith("[") Then
                            addBraces = True
                            If VALUE.IndexOf(":") > -1 Then
                                COLLECTION = VALUE.Substring(VALUE.IndexOf(":") + 1)
                            End If
                            VALUE = "[" & VALUE.Replace(":", ",") & "]"
                        End If

                        If parameters.Length > 1 Then
                            FORMATTER = ParameterMerge(CType(parameters, Array), Source, 1)
                        End If
                        If FORMATTER.ToUpper.StartsWith("{ESCAPE:") Then
                            Dim v As RenderBase = Plugins.Manager.GetPlugin(Plugins.PluginTag.Create(Config.Section.Tokens.ToString.ToLower, RenderTypes.Variable.ToString.ToLower, "")) 'Common.GetRenderer("", RenderTypes.Variable)
                            'RenderString_Variable(VALUE, DS, DR, RuntimeMessages, NullReturn, ProtectSession, SessionDelimiter, useSessionQuotes, FilterText, FilterField)
                            v.Handle_Render(Caller, Index, VALUE, DS, DR, RuntimeMessages, NullReturn, True, ProtectSession, SessionDelimiter, useSessionQuotes, useAggregations, FilterText, FilterField, Debugger)
                        Else
                            Dim b As Boolean = Caller.RenderString(Index, VALUE, New Char() {"["c, "{"c}, New Char() {"]"c, "}"c}, "\"c, DS, DR, RuntimeMessages, False, False, NullReturn:=NullReturn, ProtectSession:=ProtectSession, SessionDelimiter:=SessionDelimiter, useSessionQuotes:=useSessionQuotes)
                            If Not b AndAlso addBraces Then
                                VALUE = VALUE.Remove(0, 1).Remove(VALUE.Length - 2, 1)
                            End If
                        End If


                    End If
                End If
                If Not VALUE Is Nothing AndAlso Not FORMATTER Is Nothing Then
                    Source = ""
                    REPLACED = RenderString_Format_Value(Caller, Index, VALUE, FORMATTER, Source, DS, DR, RuntimeMessages, NullReturn, ProtectSession, SessionDelimiter, useSessionQuotes, FilterText, FilterField, Debugger)
                End If
            End If
            Return REPLACED
        End Function

        Public Function RenderString_Format_Value(ByRef Caller As EngineBase, ByVal Index As Integer, ByVal Value As String, ByVal Formatter As String, ByRef Source As String, ByRef DS As DataSet, ByRef DR As DataRow, ByRef RuntimeMessages As SortedList(Of String, String), Optional ByVal NullReturn As Boolean = False, Optional ByVal ProtectSession As Boolean = False, Optional ByVal SessionDelimeter As String = ",", Optional ByVal useSessionQuotes As Boolean = True, Optional ByRef FilterText As String = Nothing, Optional ByRef FilterField As String = Nothing, Optional ByVal DebugWriter As r2i.OWS.Framework.Debugger = Nothing) As Boolean
            Dim REPLACED As Boolean = False
            If Not Value Is Nothing AndAlso Not Formatter Is Nothing Then
                Dim rpos As Integer = Formatter.IndexOfAny(New Char() {"}"c, ":"c, " "c})
                If rpos > 0 AndAlso Formatter.Chars(0) = "{"c Then
                    Dim r As FormatterBase = Plugins.Manager.GetPlugin(Plugins.PluginTag.Create(Config.Section.Formats.ToString.ToLower, "", Formatter.Substring(1, rpos - 1))) 'Common.GetFormatter(Formatter.Substring(1, rpos - 1))
                    If Not r Is Nothing Then
                        Try
                            REPLACED = r.Handle_Render(Caller, Index, Value, Formatter, Source, DS, DR, RuntimeMessages, NullReturn, True, ProtectSession, SessionDelimeter, useSessionQuotes, False, FilterText, FilterField, DebugWriter)
                        Catch ex As Exception
                            If Not DebugWriter Is Nothing Then
                                DebugWriter.AppendHeader(Caller.ModuleID, "Formatter Error", "error", False)
                                DebugWriter.AppendLine("<li><b>Formatter Not Found Exception:</b> Cannot Handle Token '" & Value & "' properly. " & ex.ToString)
                                DebugWriter.AppendFooter(False)
                            End If
                        End Try
                    End If
                End If
            End If
            Return REPLACED
        End Function

        'Public Function OLD_RenderString_Format_Value(ByRef Caller As Engine, ByVal Index As Integer, ByVal Value As String, ByVal Formatter As String, ByRef Source As String, ByRef DS As DataSet, ByRef DR As DataRow, ByRef RuntimeMessages As SortedList(Of String, String), Optional ByVal NullReturn As Boolean = False, Optional ByVal ProtectSession As Boolean = False, Optional ByVal SessionDelimiter As String = ",", Optional ByVal useSessionQuotes As Boolean = True, Optional ByRef FilterText As String = Nothing, Optional ByRef FilterField As String = Nothing, Optional ByVal DebugWriter As r2i.OWS.Framework.Debugger = Nothing) As Boolean
        '    Dim REPLACED As Boolean = False
        '    If Not Value Is Nothing AndAlso Not Formatter Is Nothing Then
        '        '' REG - Not ready for primetime
        '        'If FormatProvider.AnyCanHandle(FORMATTER) Then
        '        '    Source = FormatProvider.Instance(FORMATTER).FormatString(VALUE, FORMATTER, REPLACED)
        '        'Else
        '        Select Case True
        '            Case Formatter.ToUpper.StartsWith("{?}")
        '                Source = Value
        '                REPLACED = True
        '            Case Formatter.ToUpper.StartsWith("{LEFT:")
        '                Dim fParameters As String() = ParameterizeString(Formatter.Substring(6).TrimEnd(New Char() {"}"c}), ","c, """"c, "\"c)
        '                If fParameters.Length = 1 Then
        '                    If IsNumeric(fParameters(0)) Then
        '                        Dim Length As Integer = Convert.ToInt32(fParameters(0))
        '                        If Length > Value.Length Then
        '                            Length = Value.Length
        '                        End If
        '                        If Value.Length > 0 Then
        '                            Source = Value.Substring(0, Length)
        '                        Else
        '                            Source = ""
        '                        End If
        '                        REPLACED = True
        '                    End If
        '                End If
        '            Case Formatter.ToUpper = ("{TRIM}")
        '                If Value.Length > 0 Then
        '                    Source = Value.Trim
        '                Else
        '                    Source = ""
        '                End If
        '                REPLACED = True
        '            Case Formatter.ToUpper = ("{TRIMLEFT}")
        '                If Value.Length > 0 Then
        '                    Source = Value.TrimStart(" ")
        '                Else
        '                    Source = ""
        '                End If
        '                REPLACED = True
        '            Case Formatter.ToUpper = ("{TRIMRIGHT}")
        '                If Value.Length > 0 Then
        '                    Source = Value.TrimEnd(" ")
        '                Else
        '                    Source = ""
        '                End If
        '                REPLACED = True
        '            Case Formatter.ToUpper = ("{FRIENDLYURL}")
        '                'If IsNumeric(Value) Then
        '                '	'Source = Utilities.Dotnetnuke_FriendlyUrl_ByTabID(Value, Me.Request.Path)
        '                'Source = AbstractFactory.Instance.EngineController.FriendlyUrlByPageID(Value, Caller.Request.Path)
        '                Source = AbstractFactory.Instance.EngineController.FriendlyUrlByPageID(Value, "")
        '                REPLACED = True
        '                'Else
        '                '	Source = Value
        '                '	REPLACED = False
        '                'End If
        '            Case Formatter.ToUpper.StartsWith("{RIGHT:")
        '                Dim fParameters As String() = ParameterizeString(Formatter.Substring(7).TrimEnd(New Char() {"}"c}), ","c, """"c, "\"c)
        '                If fParameters.Length = 1 Then
        '                    If IsNumeric(fParameters(0)) Then
        '                        Dim Length As Integer = Convert.ToInt32(fParameters(0))
        '                        If Length > Value.Length Then
        '                            Length = Value.Length
        '                        End If
        '                        If Value.Length > 0 Then
        '                            Source = Value.Substring(Value.Length - Length, Length)
        '                        Else
        '                            Source = ""
        '                        End If
        '                        REPLACED = True
        '                    End If
        '                End If
        '            Case Formatter.ToUpper.StartsWith("{MID:")
        '                Dim fParameters As String() = ParameterizeString(Formatter.Substring(5).TrimEnd(New Char() {"}"c}), ","c, """"c, "\"c)
        '                If fParameters.Length = 2 Then
        '                    If IsNumeric(fParameters(0)) AndAlso IsNumeric(fParameters(1)) Then
        '                        Dim from As Integer = Convert.ToInt32(fParameters(0))
        '                        Dim [to] As Integer = Convert.ToInt32(fParameters(1))
        '                        If from < 0 Then
        '                            from = Value.Length + from
        '                        End If
        '                        If [to] < 0 Then
        '                            [to] = Value.Length + [to]
        '                        End If

        '                        If from < 0 Then from = 0
        '                        If [to] < 0 Then [to] = 0

        '                        If from > Value.Length Then
        '                            from = Value.Length
        '                        End If
        '                        If [to] > Value.Length Then
        '                            [to] = Value.Length
        '                        End If
        '                        If [to] > from AndAlso Value.Length > 0 Then
        '                            Source = Value.Substring(from, [to] - from)
        '                        Else
        '                            Source = Value
        '                        End If
        '                        REPLACED = True
        '                    End If
        '                End If
        '            Case Formatter.ToUpper.StartsWith("{UPPER}")
        '                Source = Value.ToUpper
        '                REPLACED = True
        '            Case Formatter.ToUpper.StartsWith("{LOWER}")
        '                Source = Value.ToLower
        '                REPLACED = True
        '            Case Formatter.ToUpper.StartsWith("{LENGTH}")
        '                If Not Value Is Nothing Then
        '                    Source = Value.Length
        '                Else
        '                    Source = 0
        '                End If
        '                REPLACED = True
        '            Case Formatter.ToUpper.StartsWith("{REPLACE:") 'WHAT, WITH
        '                Dim fParameters As String() = ParameterizeString(Formatter.Substring(9).TrimEnd(New Char() {"}"c}), ","c, """"c, "\"c)
        '                If fParameters.Length = 2 Then
        '                    If Not Value Is Nothing Then
        '                        Source = Value.Replace(fParameters(0), fParameters(1))
        '                    Else
        '                        Source = Value
        '                    End If
        '                    REPLACED = True
        '                End If
        '                'VERSION: 1.9.7 - Added DECRYPT:KEY
        '            Case Formatter.ToUpper.StartsWith("{LIST:") 'PREFIX,POSTFIX,DELIMITER,ORIGINALDELIMITER
        '                'VERSION: 1.9.7 - Added List Tag
        '                Dim fParameters As String() = ParameterizeString(Formatter.Substring(6).TrimEnd(New Char() {"}"c}), ","c, """"c, "\"c)
        '                If fParameters.Length >= 3 Then
        '                    If Not Value Is Nothing Then
        '                        Dim delimiter As String = ","
        '                        If fParameters.Length > 3 Then
        '                            delimiter = fParameters(3)
        '                        End If
        '                        Dim strs() As String = Value.Split(delimiter)
        '                        Dim str As String
        '                        Dim comma As Boolean = False
        '                        Source = ""
        '                        For Each str In strs
        '                            If comma Then
        '                                Source &= fParameters(2)
        '                            Else
        '                                comma = True
        '                            End If
        '                            Source &= fParameters(0) & str & fParameters(1)
        '                        Next
        '                    Else
        '                        Source = Value
        '                    End If
        '                    REPLACED = True
        '                End If
        '            Case Formatter.ToUpper.StartsWith("{PADLEFT:") 'TARGETLENGTH, PADDINGCHAR
        '                Dim fParameters As String() = ParameterizeString(Formatter.Substring(9).TrimEnd(New Char() {"}"c}), ","c, """"c, "\"c)
        '                If fParameters.Length = 2 Then
        '                    If Not Value Is Nothing AndAlso IsNumeric(fParameters(0)) Then
        '                        If fParameters(1).Length <= 0 Then
        '                            Source = Value.PadLeft(Convert.ToInt32(fParameters(0)))
        '                        Else
        '                            Source = Value.PadLeft(Convert.ToInt32(fParameters(0)), fParameters(1).Chars(0))
        '                        End If
        '                        REPLACED = True
        '                    End If
        '                End If
        '            Case Formatter.ToUpper.StartsWith("{PADRIGHT:") 'TARGETLENGTH, PADDINGCHAR
        '                Dim fParameters As String() = ParameterizeString(Formatter.Substring(10).TrimEnd(New Char() {"}"c}), ","c, """"c, "\"c)
        '                If fParameters.Length = 2 Then
        '                    If Not Value Is Nothing AndAlso IsNumeric(fParameters(0)) Then
        '                        If fParameters(1).Length <= 0 Then
        '                            Source = Value.PadRight(Convert.ToInt32(fParameters(0)))
        '                        Else
        '                            Source = Value.PadRight(Convert.ToInt32(fParameters(0)), fParameters(1).Chars(0))
        '                        End If
        '                        REPLACED = True
        '                    End If
        '                End If
        '            Case Formatter.ToUpper = "{TABID}"
        '                Dim tID As String = ConvertPageNameToPageID(Caller, Value)
        '                If IsNumeric(tID) AndAlso tID >= 0 Then
        '                    Source = tID
        '                    REPLACED = True
        '                ElseIf Not IsNumeric(tID) AndAlso tID <> "" Then
        '                    Source = tID
        '                    REPLACED = True
        '                End If
        '                'Version: 1.9.1 - Added TabID:Column lookup formatter
        '            Case Formatter.ToUpper.StartsWith("{TABID:")
        '                Dim tiD As String = "-1"
        '                Dim fParameters As String() = ParameterizeString(Formatter.Substring(7).TrimEnd(New Char() {"}"c}), ","c, """"c, "\"c)
        '                If fParameters.Length = 1 Then
        '                    If Not Value Is Nothing Then
        '                        Dim columnName As String = fParameters(0)
        '                        If columnName.IndexOf("'") < 0 AndAlso columnName.IndexOf(" ") < 0 AndAlso columnName.IndexOf("(") < 0 AndAlso columnName.IndexOf(")") < 0 Then
        '                            'tiD = Dotnetnuke_TabLookupBy(Value, columnName)
        '                            'tiD = AbstractFactory.Instance.EngineController.Dotnetnuke_TabLookupBy(Value, columnName)
        '                            tiD = AbstractFactory.Instance.EngineController.PageLookupBy(Value, columnName)
        '                        End If
        '                    End If
        '                End If
        '                Source = tiD
        '                REPLACED = True
        '            Case Formatter.ToUpper = "{TABNAME}"
        '                'If IsNumeric(Value) Then
        '                Dim tName As String = ConvertPageIDToPageName(Value)
        '                If Not tName Is Nothing Then
        '                    Source = tName
        '                    REPLACED = True
        '                End If
        '                'End If
        '            Case Formatter.ToUpper = "{TABTITLE}"
        '                Try

        '                    Dim pageTitle As String = AbstractFactory.Instance.EngineController.PageTitle(Value)
        '                    If Not pageTitle Is Nothing Then
        '                        Source = pageTitle
        '                        REPLACED = True
        '                    End If

        '                Catch ex As Exception
        '                End Try
        '            Case Formatter.ToUpper = "{TABDESCRIPTION}"
        '                Try

        '                    Dim pageDescription As String = AbstractFactory.Instance.EngineController.PageDescription(Value)
        '                    If Not pageDescription Is Nothing Then
        '                        Source = pageDescription
        '                        REPLACED = True
        '                    End If
        '                Catch ex As Exception
        '                End Try
        '                'VERSION: 1.9.9 - Formatter {MAPPATH}
        '            Case Formatter.ToUpper = "{MAPPATH}"
        '                Try
        '                    Dim Path As String = Value
        '                    Try
        '                        Path = Caller.Request.MapPath(Path)
        '                    Catch ex As Exception
        '                        Source = "Unable to Map Path: " & ex.ToString
        '                    End Try
        '                    Source = Path
        '                Catch ex As Exception
        '                    Source = Boolean.FalseString
        '                End Try
        '                REPLACED = True
        '            Case Formatter.ToUpper = "{PARENTPATH}"
        '                Try
        '                    Dim Path As String = Value
        '                    Try
        '                        Path = Caller.Request.MapPath(Path)
        '                        Dim fio As IO.DirectoryInfo = New System.IO.DirectoryInfo(Path)
        '                        If fio.Exists AndAlso Not fio.Parent Is Nothing Then
        '                            Source = fio.Parent.FullName.Replace("\", "/")
        '                        Else
        '                            Source = ""
        '                        End If
        '                    Catch ex As Exception
        '                        Source = "Unable to Map Parent Path: " & ex.ToString
        '                    End Try
        '                Catch ex As Exception
        '                    Source = ""
        '                End Try
        '                REPLACED = True
        '            Case Formatter.ToUpper = "{REVERSEPARENTPATH}" Or Formatter.ToUpper = "{MAPPARENTURL}"
        '                Try
        '                    Dim Path As String = Value
        '                    Try
        '                        Path = Caller.Request.MapPath(Path)
        '                        Dim fio As IO.DirectoryInfo = New System.IO.DirectoryInfo(Path)
        '                        If fio.Exists AndAlso Not fio.Parent Is Nothing Then
        '                            Path = fio.Parent.FullName.Replace("\", "/")

        '                            If Path = "" Then
        '                                'Path = Context.Current.Server.MapPath("~")
        '                                Path = Web.HttpContext.Current.Server.MapPath("~")
        '                            End If

        '                            'Dim AppPath As String = Context.Current.Server.MapPath("~").Replace("\", "/")
        '                            Dim AppPath As String = Web.HttpContext.Current.Server.MapPath("~").Replace("\", "/")
        '                            Path = Path.Replace("\", "/")

        '                            While AppPath.EndsWith("/")
        '                                AppPath = AppPath.Remove(AppPath.Length - 1, 1)
        '                            End While
        '                            While Path.EndsWith("/")
        '                                Path = Path.Remove(Path.Length - 1, 1)
        '                            End While
        '                            While AppPath.StartsWith("/")
        '                                AppPath = AppPath.Remove(0, 1)
        '                            End While
        '                            While Path.StartsWith("/")
        '                                Path = Path.Remove(0, 1)
        '                            End While

        '                            Source = String.Format("~/{0}", Path.Replace(AppPath, ""))
        '                        Else
        '                            Source = ""
        '                        End If
        '                    Catch ex As Exception
        '                        Source = "Unable to Map Parent Path: " & ex.ToString
        '                    End Try
        '                Catch ex As Exception
        '                    Source = ""
        '                End Try
        '                REPLACED = True
        '            Case Formatter.ToUpper = "{REVERSEPATH}" Or Formatter.ToUpper = "{MAPURL}"
        '                'VERSION: 2.0 Added REVERSEPATH
        '                Try
        '                    Dim Path As String
        '                    Try
        '                        If Value.StartsWith("~") OrElse Value.StartsWith("/") OrElse Value.StartsWith("\") Then
        '                            'Path = Context.Current.Server.MapPath(Value)
        '                            Path = Web.HttpContext.Current.Server.MapPath(Value)
        '                        Else
        '                            Path = Value
        '                        End If
        '                    Catch ex As Exception
        '                        'Path = Context.Current.Server.MapPath("~")
        '                        Path = Web.HttpContext.Current.Server.MapPath("~")
        '                    End Try


        '                    'Dim AppPath As String = Context.Current.Server.MapPath("~").Replace("\", "/")
        '                    Dim AppPath As String = Web.HttpContext.Current.Server.MapPath("~").Replace("\", "/")

        '                    Path = Path.Replace("\", "/")

        '                    While AppPath.EndsWith("/")
        '                        AppPath = AppPath.Remove(AppPath.Length - 1, 1)
        '                    End While
        '                    While Path.EndsWith("/")
        '                        Path = Path.Remove(Path.Length - 1, 1)
        '                    End While
        '                    While AppPath.StartsWith("/")
        '                        AppPath = AppPath.Remove(0, 1)
        '                    End While
        '                    While Path.StartsWith("/")
        '                        Path = Path.Remove(0, 1)
        '                    End While

        '                    If Not Path.ToUpper.StartsWith(AppPath.ToUpper) Then
        '                        Source = AppPath
        '                    Else
        '                        Source = String.Format("~/{0}", Path.Replace(AppPath, ""))
        '                    End If
        '                Catch ex As Exception
        '                    Source = "Unabled to reverse path"
        '                End Try
        '                REPLACED = True
        '            Case Formatter.ToUpper = "{EXISTS}"
        '                Try
        '                    Dim Path As String = Value
        '                    Try
        '                        Path = Caller.Request.MapPath(Path)
        '                    Catch ex As Exception
        '                    End Try
        '                    If New System.IO.FileInfo(Path).Exists Then
        '                        Source = Boolean.TrueString
        '                    Else
        '                        Source = Boolean.FalseString
        '                    End If
        '                Catch ex As Exception
        '                    Source = Boolean.FalseString
        '                End Try
        '                REPLACED = True
        '                'Version: 1.9.1 - Added SQLFIND:TABLE,FINDCOLUMN,RETURNCOLUMN Column lookup formatter
        '            Case Formatter.ToUpper.StartsWith("{SQLFIND:")
        '                Dim fParameters As String() = ParameterizeString(Formatter.Substring(9).TrimEnd(New Char() {"}"c}), ","c, """"c, "\"c)
        '                If fParameters.Length = 3 Then
        '                    If Not Value Is Nothing Then
        '                        Dim tablename As String = fParameters(0)
        '                        Dim findcolumn As String = fParameters(1)
        '                        Dim returncolumn As String = fParameters(2)
        '                        If Not tablename Is Nothing AndAlso tablename.IndexOf("'") < 0 AndAlso tablename.IndexOf(" ") < 0 AndAlso tablename.IndexOf("(") < 0 AndAlso tablename.IndexOf(")") < 0 Then
        '                        Else
        '                            tablename = Nothing
        '                        End If
        '                        If Not findcolumn Is Nothing AndAlso findcolumn.IndexOf("'") < 0 AndAlso findcolumn.IndexOf(" ") < 0 AndAlso findcolumn.IndexOf("(") < 0 AndAlso findcolumn.IndexOf(")") < 0 Then
        '                        Else
        '                            findcolumn = Nothing
        '                        End If
        '                        If Not returncolumn Is Nothing AndAlso returncolumn.IndexOf("'") < 0 AndAlso returncolumn.IndexOf(" ") < 0 AndAlso returncolumn.IndexOf("(") < 0 AndAlso returncolumn.IndexOf(")") < 0 Then
        '                        Else
        '                            returncolumn = Nothing
        '                        End If
        '                        If Not tablename Is Nothing AndAlso Not findcolumn Is Nothing AndAlso Not returncolumn Is Nothing AndAlso Not Value Is Nothing Then
        '                            Dim srcR As String = ""
        '                            Try
        '                                'srcR = Dotnetnuke_TableLookupBy(Value, tablename, returncolumn, findcolumn)
        '                                'srcR = AbstractFactory.Instance.EngineController.Dotnetnuke_TableLookupBy(Value, tablename, returncolumn, findcolumn)
        '                                srcR = AbstractFactory.Instance.EngineController.TableLookupBy(Value, tablename, returncolumn, findcolumn)
        '                            Catch ex As Exception

        '                            End Try
        '                            Source = srcR
        '                            REPLACED = True
        '                        End If
        '                    End If
        '                End If
        '            Case Formatter.ToUpper.StartsWith("{URL}")
        '                If Value Is Nothing OrElse Value.Length = 0 Then
        '                    Source = ""
        '                Else
        '                    'Dim sTab As String
        '                    'sTab = Value
        '                    'If Not IsNumeric(sTab) Then
        '                    '	Dim tID As Integer = ConvertPageNameToPageID(Value)
        '                    '	If tID >= 0 Then
        '                    '		sTab = tID
        '                    '	End If
        '                    'End If
        '                    'If IsNumeric(sTab) Then
        '                    '	'ROMAIN: 09/
        '                    '	'Source = DotNetNuke.Common.NavigateURL(sTab)
        '                    '	Source = AbstractFactory.Instance.EngineController.NavigateURL(sTab)
        '                    'End If
        '                    Source = AbstractFactory.Instance.EngineController.NavigateURL(Value)
        '                End If
        '                REPLACED = True
        '            Case Formatter.ToUpper.StartsWith("{URL:")
        '                'VERSION: 1.9.8 - Added URL Formatter
        '                If Value Is Nothing OrElse Value.Length = 0 Then
        '                    Source = ""
        '                Else
        '                    'Dim sTab As String
        '                    'Dim sControl As String
        '                    'ROMAIN: Generic replacement - 08/20/2007
        '                    'Dim sParameters As New ArrayList
        '                    Dim fParameters As String() = ParameterizeString(Formatter.Substring(5).TrimEnd(New Char() {"}"c}), ","c, """"c, "\"c)
        '                    Dim sParameters As New List(Of String)
        '                    sParameters.AddRange(fParameters)
        '                    'sTab = Value
        '                    ''If Not IsNumeric(sTab) Then
        '                    ''	Dim tID As Integer = ConvertPageNameToPageID(Value)
        '                    ''	If tID >= 0 Then
        '                    ''		sTab = tID
        '                    ''	End If
        '                    ''End If
        '                    'If IsNumeric(sTab) Then
        '                    '	Dim itab As Integer = CInt(sTab)
        '                    '	Try
        '                    '		Dim fParameters As String() = Formatter.Substring(5).TrimEnd(New Char() {"}"c}).Split("&"c)
        '                    '		If fParameters.Length > 0 Then
        '                    '			Dim fparameter As String
        '                    '			For Each fparameter In fParameters
        '                    '				Dim iparameters As String() = fparameter.Split("=")
        '                    '				If iparameters.Length > 1 Then
        '                    '					If iparameters(0).ToUpper = "CTL" OrElse iparameters(0).ToUpper = "ControlKey" Then
        '                    '						sControl = iparameters(1)
        '                    '					Else
        '                    '						sParameters.Add(fparameter)
        '                    '					End If
        '                    '				End If
        '                    '			Next
        '                    '			If sParameters.Count > 0 Then
        '                    '				'ROMAIN: Generic replacement - 08/20/2007
        '                    '				'Dim strParameters As String() = sParameters.ToArray(GetType(String))
        '                    '				Dim strParameters As String() = sParameters.ToArray
        '                    '				'Source = DotNetNuke.Common.NavigateURL(itab, sControl, strParameters)
        '                    '				Source = AbstractFactory.Instance.EngineController.NavigateURL(itab, sControl, strParameters)
        '                    '			ElseIf Not sControl Is Nothing Then
        '                    '				'Source = DotNetNuke.Common.NavigateURL(itab, sControl)
        '                    '				Source = AbstractFactory.Instance.EngineController.NavigateURL(itab, sControl)
        '                    '			Else
        '                    '				'Source = DotNetNuke.Common.NavigateURL(itab)
        '                    '				Source = AbstractFactory.Instance.EngineController.NavigateURL(itab)
        '                    '			End If
        '                    '		Else
        '                    '			If Value Is Nothing OrElse Value.Length = 0 Then
        '                    '				Source = ""
        '                    '			Else
        '                    '				Source = Value
        '                    '			End If
        '                    '			REPLACED = True
        '                    '		End If
        '                    '	Catch ex As Exception

        '                    '	End Try
        '                    'End If
        '                    Source = AbstractFactory.Instance.EngineController.NavigateURL(Value, sParameters)
        '                End If
        '                REPLACED = True
        '            Case Formatter.ToUpper.StartsWith("{ISEMPTY:")
        '                Dim fParameters As String() = ParameterizeString(Formatter.Substring(9).TrimEnd(New Char() {"}"c}), ","c, """"c, "\"c)
        '                If fParameters.Length = 1 Then
        '                    If Value Is Nothing OrElse Value.Length = 0 Then
        '                        Source = fParameters(0)
        '                    Else
        '                        Source = Value
        '                    End If
        '                    REPLACED = True
        '                Else
        '                    If Value Is Nothing OrElse Value.Length = 0 Then
        '                        Source = ""
        '                    Else
        '                        Source = Value
        '                    End If
        '                    REPLACED = True
        '                End If
        '            Case Formatter.ToUpper.StartsWith("{STARTSWITH:")
        '                Dim fParameter As String = Formatter.Substring(12, Formatter.Length - 13)
        '                If fParameter.Length > 0 Then
        '                    If Value.StartsWith(fParameter) Then
        '                        Source = Boolean.TrueString
        '                    Else
        '                        Source = Boolean.FalseString
        '                    End If
        '                Else
        '                    Source = Boolean.FalseString
        '                End If
        '                REPLACED = True
        '            Case Formatter.ToUpper.StartsWith("{ENDSWITH:")
        '                Dim fParameter As String = Formatter.Substring(10, Formatter.Length - 11)
        '                If fParameter.Length > 0 Then
        '                    If Value.EndsWith(fParameter) Then
        '                        Source = Boolean.TrueString
        '                    Else
        '                        Source = Boolean.FalseString
        '                    End If
        '                Else
        '                    Source = Boolean.FalseString
        '                End If
        '                REPLACED = True
        '                'VERSION: Added INDEXOF in 1.9.6
        '            Case Formatter.ToUpper.StartsWith("{INDEXOF:")
        '                Dim fParameter As String = Formatter.Substring(9, Formatter.Length - 10)
        '                If fParameter.Length > 0 Then
        '                    Dim indexi As Integer = Value.IndexOf(fParameter)
        '                    Source = indexi
        '                Else
        '                    Source = -1
        '                End If
        '                REPLACED = True
        '                'VERSION: Added LASTINDEXOF in 1.9.6
        '            Case Formatter.ToUpper.StartsWith("{LASTINDEXOF:")
        '                Dim fParameter As String = Formatter.Substring(13, Formatter.Length - 14)
        '                If fParameter.Length > 0 Then
        '                    Dim indexi As Integer = Value.LastIndexOf(fParameter)
        '                    Source = indexi
        '                Else
        '                    Source = -1
        '                End If
        '                REPLACED = True
        '            Case Formatter.ToUpper.StartsWith("{CONTAINS:")
        '                Dim fParameter As String = Formatter.Substring(10, Formatter.Length - 11)
        '                If fParameter.Length > 0 Then
        '                    If Value.IndexOf(fParameter) >= 0 Then
        '                        Source = Boolean.TrueString
        '                    Else
        '                        Source = Boolean.FalseString
        '                    End If
        '                Else
        '                    Source = Boolean.FalseString
        '                End If
        '                REPLACED = True
        '            Case Formatter.ToUpper.StartsWith("{ISSUPERUSER}")

        '                If Not Caller.UserInfo Is Nothing Then
        '                    Source = Caller.UserInfo.IsSuperUser
        '                End If
        '                'Source = "USER: " & Me.UserInfo.UserID & " PORTAL: " & Me.PortalID & " ROLES: " & Source
        '                REPLACED = True
        '            Case Formatter.ToUpper.StartsWith("{ISINROLE}")
        '                If Not Value Is Nothing AndAlso Value.Length > 0 Then
        '                    Try
        '                        Dim bInRole As Boolean = False
        '                        'bInRole = IsInRoleNames(Me.UserInfo, Value)
        '                        bInRole = AbstractFactory.Instance.UserController.IsInRoles(Caller.UserInfo, Value)
        '                        Source = bInRole.ToString
        '                    Catch ex As Exception
        '                        Source = ex.ToString
        '                    End Try
        '                Else
        '                    'Caller.UserInfo()
        '                    Source = Boolean.FalseString
        '                End If
        '                'Source = "USER: " & Me.UserInfo.UserID & " PORTAL: " & Me.PortalID & " ROLES: " & Source
        '                REPLACED = True
        '            Case Formatter.ToUpper.StartsWith("{ISNUMERIC}")
        '                If IsNumeric(Value) Then
        '                    Source = Boolean.TrueString
        '                Else
        '                    Source = Boolean.FalseString
        '                End If
        '                REPLACED = True
        '            Case Formatter.ToUpper.StartsWith("{ISDATE}")
        '                If IsDate(Value) Then
        '                    Source = Boolean.TrueString
        '                Else
        '                    Source = Boolean.FalseString
        '                End If
        '                REPLACED = True
        '            Case Formatter.ToUpper.StartsWith("{CANEDIT}")
        '                If Caller.ModuleIsEditable Then
        '                    Source = Boolean.TrueString
        '                Else
        '                    Source = Boolean.FalseString
        '                End If
        '                REPLACED = True
        '            Case Formatter.ToUpper.StartsWith("{ENCODEWIKI}")
        '                'VERSION 2.0.5 - Added {ENCODEWIKI}
        '                Dim wikiM As New WikiTokenType.WikiTokenManager
        '                With wikiM
        '                    Dim s As String
        '                    s = Caller.ActionVariable("ows.Wiki.CodeFormat")
        '                    If Not s Is Nothing AndAlso s.Length > 0 Then
        '                        .CodeFormat = s
        '                    End If
        '                    s = Caller.ActionVariable("ows.Wiki.LinkClassFormat")
        '                    If Not s Is Nothing AndAlso s.Length > 0 Then
        '                        .LinkClassFormat = s
        '                    End If
        '                    s = Caller.ActionVariable("ows.Wiki.LinkExternalFormat")
        '                    If Not s Is Nothing AndAlso s.Length > 0 Then
        '                        .LinkExternalFormat = s
        '                    End If
        '                    s = Caller.ActionVariable("ows.Wiki.LinkFormat")
        '                    If Not s Is Nothing AndAlso s.Length > 0 Then
        '                        .LinkFormat = s
        '                    End If
        '                    s = Caller.ActionVariable("ows.Wiki.LinkInternalFormat")
        '                    If Not s Is Nothing AndAlso s.Length > 0 Then
        '                        .LinkInternalFormat = s
        '                    End If
        '                    s = Caller.ActionVariable("ows.Wiki.LinkSectionValueFormat")
        '                    If Not s Is Nothing AndAlso s.Length > 0 Then
        '                        .LinkSectionValueFormat = s
        '                    End If
        '                    s = Caller.ActionVariable("ows.Wiki.LinkValueFormat")
        '                    If Not s Is Nothing AndAlso s.Length > 0 Then
        '                        .LinkValueFormat = s
        '                    End If
        '                    s = Caller.ActionVariable("ows.Wiki.Section0Format")
        '                    If Not s Is Nothing AndAlso s.Length > 0 Then
        '                        .Section0Format = s
        '                    End If
        '                    s = Caller.ActionVariable("ows.Wiki.Section1Format")
        '                    If Not s Is Nothing AndAlso s.Length > 0 Then
        '                        .Section1Format = s
        '                    End If
        '                    s = Caller.ActionVariable("ows.Wiki.Section2Format")
        '                    If Not s Is Nothing AndAlso s.Length > 0 Then
        '                        .Section2Format = s
        '                    End If
        '                    s = Caller.ActionVariable("ows.Wiki.Section3Format")
        '                    If Not s Is Nothing AndAlso s.Length > 0 Then
        '                        .Section3Format = s
        '                    End If
        '                    s = Caller.ActionVariable("ows.Wiki.TOC0Format")
        '                    If Not s Is Nothing AndAlso s.Length > 0 Then
        '                        .TOC0Format = s
        '                    End If
        '                    s = Caller.ActionVariable("ows.Wiki.TOC1Format")
        '                    If Not s Is Nothing AndAlso s.Length > 0 Then
        '                        .TOC1Format = s
        '                    End If
        '                    s = Caller.ActionVariable("ows.Wiki.TOC2Format")
        '                    If Not s Is Nothing AndAlso s.Length > 0 Then
        '                        .TOC2Format = s
        '                    End If
        '                    s = Caller.ActionVariable("ows.Wiki.TOCFormat3")
        '                    If Not s Is Nothing AndAlso s.Length > 0 Then
        '                        .TOC3Format = s
        '                    End If
        '                    s = Caller.ActionVariable("ows.Wiki.TOCFormat")
        '                    If Not s Is Nothing AndAlso s.Length > 0 Then
        '                        .TOCFormat = s
        '                    End If
        '                    s = Caller.ActionVariable("ows.Wiki.TOCBlockFormat")
        '                    If Not s Is Nothing AndAlso s.Length > 0 Then
        '                        .TOCBlockFormat = s
        '                    End If
        '                    s = Caller.ActionVariable("ows.Wiki.Quote")
        '                    If Not s Is Nothing AndAlso s.Length > 0 Then
        '                        .QuoteFormat = s
        '                    End If
        '                    Dim sidebarKeys As String() = Caller.ActionVariableSearch("ows.Wiki.Sidebar.")
        '                    If Not sidebarKeys Is Nothing AndAlso sidebarKeys.Length > 0 Then
        '                        Dim strKey As String
        '                        For Each strKey In sidebarKeys
        '                            s = Caller.ActionVariable(strKey)
        '                            If Not s Is Nothing AndAlso s.Length > 0 Then
        '                                strKey = strKey.Substring(17)
        '                                .SidebarLayout(strKey) = s
        '                            End If
        '                        Next
        '                    End If
        '                End With
        '                Source = wikiM.Render(Value, Caller, DS, RuntimeMessages, False, False, DebugWriter)
        '                wikiM = Nothing
        '                REPLACED = True
        '            Case Formatter.ToUpper.StartsWith("{ENCODEURI}")
        '                Source = Web.HttpUtility.UrlEncode(Value)
        '                REPLACED = True
        '            Case Formatter.ToUpper.StartsWith("{DECODEURI}")
        '                Source = Web.HttpUtility.UrlDecode(Value)
        '                REPLACED = True
        '                'VERSION: 1.9 - Added {ENCODEHTML} AND {DECODEHTML} to the FORMAT
        '            Case Formatter.ToUpper.StartsWith("{ENCODEHTML}")
        '                Source = Web.HttpUtility.HtmlEncode(Value)
        '                REPLACED = True
        '            Case Formatter.ToUpper.StartsWith("{DECODEHTML}")
        '                Source = Web.HttpUtility.HtmlDecode(Value)
        '                REPLACED = True
        '                'VERSION: 1.9.7 - Added ENCRYPT:KEY
        '            Case Formatter.ToUpper.StartsWith("{ESCAPE}")
        '                'VERSION: 1.9.9 - Added ESCAPE
        '                Try
        '                    Source = AddEscapes(Value)
        '                Catch ex As Exception

        '                End Try
        '                REPLACED = True
        '            Case Formatter.ToUpper.StartsWith("{ESCAPE:")
        '                Dim fParameters As String() = ParameterizeString(Formatter.Substring(7).TrimEnd(New Char() {"}"c}), ","c, """"c, "\"c)
        '                Dim repeat As Integer = 1
        '                If fParameters.Length = 1 Then
        '                    If IsNumeric(fParameters(0)) Then
        '                        repeat = CInt(fParameters(0))
        '                        If repeat < 1 Then repeat = 1
        '                    End If
        '                End If
        '                Dim strEscape As String = New String("\"c, repeat)
        '                Source = Value.Replace("{", strEscape & "{").Replace("}", strEscape & "}").Replace("[", strEscape & "[").Replace("]", strEscape & "]")
        '                REPLACED = True
        '            Case Formatter.ToUpper.StartsWith("{UNESCAPE}")
        '                'VERSION: 1.9.9 - Added UNESCAPE
        '                Try
        '                    StripEscapes(Value)
        '                Catch ex As Exception

        '                End Try
        '                Source = Value
        '                REPLACED = True
        '                'VERSION: 1.9.7 - Added ENCRYPT:KEY
        '            Case Formatter.ToUpper.StartsWith("{ENCRYPT:")
        '                Dim fParameter As String = Formatter.Substring(9, Formatter.Length - 10)
        '                If fParameter.Length > 0 Then
        '                    'ROMAIN: 09/18/07
        '                    'Source = RenderString_Encrypt(fParameter, Value)
        '                    Source = AbstractFactory.Instance.SecurityController.RenderString_Encrypt(fParameter, Value)
        '                Else
        '                    Source = Value
        '                End If
        '                REPLACED = True
        '                'VERSION: 2.0 - Added MD5HASH
        '            Case Formatter.ToUpper.StartsWith("{MD5HASH}")
        '                Source = RenderString_Format_MD5Hash(Value)
        '                REPLACED = True
        '                'VERSION: 1.9.7 - Added DECRYPT:KEY
        '            Case Formatter.ToUpper.StartsWith("{DECRYPT:")
        '                Dim fParameter As String = Formatter.Substring(9, Formatter.Length - 10)
        '                If fParameter.Length > 0 Then
        '                    'ROMAIN: 09/18/07
        '                    'Source = RenderString_Decrypt(fParameter, Value)
        '                    Source = AbstractFactory.Instance.SecurityController.RenderString_Decrypt(fParameter, Value)
        '                Else
        '                    Source = Value
        '                End If
        '                REPLACED = True
        '            Case Else
        '                If Not Value Is Nothing AndAlso Formatter.ToUpper.StartsWith("{0:") Then
        '                    If Value.Length > 0 Then
        '                        If IsNumeric(Value) Then
        '                            'ITS NUMERIC
        '                            Try
        '                                If Formatter.ToUpper.StartsWith("{0:X") Then
        '                                    Source = System.String.Format(Formatter, CType(Value, Int64))
        '                                Else
        '                                    Source = System.String.Format(Formatter, CType(Value, Decimal))
        '                                End If
        '                                REPLACED = True
        '                            Catch ex As Exception
        '                            End Try
        '                        Else
        '                            'ITS A DATE
        '                            Try
        '                                Source = System.String.Format(Formatter, CType(Value, Date))
        '                                REPLACED = True
        '                            Catch ex As Exception
        '                            End Try
        '                        End If
        '                    Else
        '                        If Formatter.IndexOf("#"c) >= 0 Then
        '                            'ITS NUMERIC
        '                            Try
        '                                Source = System.String.Format(Formatter, CType(Value, Decimal))
        '                                REPLACED = True
        '                            Catch ex As Exception
        '                            End Try
        '                        Else
        '                            'Don't Know
        '                            Try
        '                                Source = System.String.Format(Formatter, Value)
        '                                REPLACED = True
        '                            Catch ex As Exception
        '                            End Try
        '                        End If
        '                    End If
        '                    'ElseIf Not Value Is Nothing Then
        '                    '    REPLACED = RenderString_CustomFormat(Index, Value, Formatter, Source, DS, DR, RuntimeMessages, NullReturn, ProtectSession, SessionDelimiter, useSessionQuotes, FilterText, FilterField, False, __DebugWriter)
        '                End If
        '        End Select
        '        'End If
        '    End If
        '    Return REPLACED
        'End Function

        'Dim _md5 As System.Security.Cryptography.MD5
        'Private Function RenderString_Format_MD5Hash(ByVal Value As String) As String
        '    If _md5 Is Nothing Then
        '        _md5 = System.Security.Cryptography.MD5.Create()
        '    End If
        '    'oFirewall.Clean(Key)
        '    Dim hash() As Byte
        '    hash = _md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(Value))
        '    Dim sb As System.Text.StringBuilder = New System.Text.StringBuilder
        '    For Each b As Byte In hash
        '        sb.Append(b.ToString("x2", System.Globalization.CultureInfo.InvariantCulture))
        '    Next
        '    Return sb.ToString
        'End Function

        'Private pageConversion_PageID As SortedList(Of String, String)
        'Private pageConversion_PageName As SortedList(Of String, String)

        'Private Function ConvertPageIDToPageName(ByVal pageId As String) As String
        '    If pageConversion_PageID Is Nothing Then
        '        'ROMAIN: Generic replacement - 08/22/2007
        '        'tabConversion_TabID = New SortedList
        '        pageConversion_PageID = New SortedList(Of String, String)
        '    End If
        '    If Not pageConversion_PageID.ContainsKey(pageId) Then
        '        Try
        '            Dim pageName As String = AbstractFactory.Instance.EngineController.PageName(pageId)
        '            If Not pageName Is Nothing Then
        '                pageConversion_PageID.Item(pageId) = pageName
        '            End If
        '            Return pageName
        '        Catch ex As Exception
        '        End Try
        '    Else
        '        Return pageConversion_PageID.Item(pageId)
        '    End If
        '    Return Nothing
        'End Function
        'Private Function ConvertPageNameToPageID(ByRef Caller As Engine, ByVal pageName As String) As String
        '    If pageConversion_PageName Is Nothing Then
        '        pageConversion_PageName = New SortedList(Of String, String)
        '    End If
        '    If Not pageConversion_PageName.ContainsKey(Caller.PortalSettings.PortalId.ToString & ":" & pageName) Then
        '        Try
        '            Dim pageId As String = AbstractFactory.Instance.EngineController.PageId(pageName, Caller.PortalID)
        '            If (IsNumeric(pageId) AndAlso pageId > 0) OrElse (Not IsNumeric(pageId) AndAlso pageId <> "") Then
        '                pageConversion_PageName.Item(Caller.PortalSettings.PortalId.ToString & ":" & pageName) = pageId
        '                Return pageId
        '            End If
        '        Catch ex As Exception
        '        End Try
        '    Else
        '        Return pageConversion_PageName.Item(Caller.PortalSettings.PortalId.ToString & ":" & pageName)
        '    End If
        '    Return "-1"
        'End Function
    End Class
End Namespace