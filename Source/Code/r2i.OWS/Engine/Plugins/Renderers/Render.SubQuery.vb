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
Imports System.Collections.Generic
Imports r2i.OWS.Framework
Imports r2i.OWS.Framework.Utilities
Imports r2i.OWS.Framework.Plugins.Renderers
Namespace r2i.OWS.Renderers
    Public Class RenderSubQuery
        Inherits RenderBase

        Public Overrides ReadOnly Property RenderTag() As String
            Get
                Return "SUBQUERY"
            End Get
        End Property

        Public Overrides ReadOnly Property RenderType() As RenderTypes
            Get
                Return RenderTypes.Functional
            End Get
        End Property

        Private Function Get_Cached_Query(ByRef Caller As Engine, ByVal name As String, ByVal isPublic As Boolean) As String
            Return Caller.Cache("ows.subquery.name." & name, isPublic)
        End Function
        Private Sub Set_Cached_Query(ByRef Caller As Engine, ByVal name As String, ByVal isPublic As Boolean, ByVal Value As String)
            Caller.Cache("ows.subquery.name." & name, isPublic) = Value
        End Sub

        Private Function Get_Cached_Data(ByRef Caller As Engine, ByVal query As String, ByVal isPublic As Boolean) As DataSet
            Return CType(Caller.Cache("ows.subquery.query." & query, isPublic), DataSet)
            'If Not ds Is Nothing Then
            '    Return ds.Clone
            'End If
            'Return Nothing
        End Function
        Private Sub Set_Cached_Data(ByRef Caller As Engine, ByVal query As String, ByVal isPublic As Boolean, ByVal Value As DataSet)
            'If isPublic Then
            '    Caller.Cache("ows.subquery.query." & query, isPublic) = Value.Clone
            'Else
            Caller.Cache("ows.subquery.query." & query, isPublic) = Value
            'End If
        End Sub

        Public Overrides Function Handle_Render(ByRef Caller as EngineBase, ByVal Index As Integer, ByRef Source As String, ByRef SharedDS As System.Data.DataSet, ByRef DR As System.Data.DataRow, ByRef RuntimeMessages As System.Collections.Generic.SortedList(Of String, String), ByVal NullReturn As Boolean, ByVal NullOverride As Boolean, ByVal ProtectSession As Boolean, ByVal SessionDelimiter As String, ByVal useSessionQuotes As Boolean, ByVal useAggregations As Boolean, ByRef FilterText As String, ByRef FilterField As String, ByRef Debugger As r2i.OWS.Framework.Debugger) As Boolean
            Dim REPLACED As Boolean = False
            Dim parameters As String() = ParameterizeString(Source, ","c, """"c, "\"c)
            Dim bDebug As Boolean

            If Debugger Is Nothing Or Caller.xls.skipSubqueryDebugging Then
                bDebug = False
            Else
                bDebug = True
            End If

            If SharedDS Is Nothing Then
                SharedDS = New DataSet
            End If

            If Not parameters Is Nothing AndAlso parameters.Length > 1 Then
                ''    'REG - 11/28/2005: Added new
                ''    '{SUBQUERY, Name, [Query], [Format], [AlternateFormat], [SelectedFormat], [SelectedField], [SelectedItems]}
                Dim sbvalue As New System.Text.StringBuilder
                Try
                    Dim DS As DataSet = Nothing
                    Dim bcachedQuery As Boolean = False
                    Dim bcachedData As Boolean = False
                    Dim sName As String = ""
                    Dim sQuery As String = ""
                    Dim sFormat As String = ""
                    Dim sHeader As String = ""
                    Dim sFooter As String = ""
                    Dim sConnection As String = ""
                    Dim sNoResultFormat As String = ""
                    Dim sNoQueryFormat As String = ""
                    Dim sSelFormat As String = ""
                    Dim sJSON As String = ""
                    Dim sAltFormat As String = ""
                    Dim sSelItems As String = ""
                    Dim sSelField As String = ""
                    'ROMAIN: Generic replacement - 08/20/2007
                    'Dim arrSelItems As New ArrayList
                    Dim arrSelItems As New List(Of String)
                    Dim bUseCache As Boolean = False
                    Dim sCacheTime As String = Nothing

                    Dim str As String = Nothing
                    Dim i As Integer = 0
                    For Each str In parameters
                        Select Case True
                            Case str.ToUpper.StartsWith("QUERY=")
                                sQuery = str.Remove(0, 7).TrimEnd(New Char() {""""c})
                            Case str.ToUpper.StartsWith("FORMAT=")
                                sFormat = str.Remove(0, 8).TrimEnd(New Char() {""""c})
                            Case str.ToUpper.StartsWith("HEADER=")
                                sHeader = str.Remove(0, 8).TrimEnd(New Char() {""""c})
                            Case str.ToUpper.StartsWith("FOOTER=")
                                sFooter = str.Remove(0, 8).TrimEnd(New Char() {""""c})
                            Case str.ToUpper.StartsWith("NOQUERYFORMAT=")
                                sNoQueryFormat = str.Remove(0, 15).TrimEnd(New Char() {""""c})
                            Case str.ToUpper.StartsWith("NORESULTFORMAT=")
                                sNoResultFormat = str.Remove(0, 16).TrimEnd(New Char() {""""c})
                            Case str.ToUpper.StartsWith("SELECTEDFORMAT=")
                                sSelFormat = str.Remove(0, 16).TrimEnd(New Char() {""""c})
                            Case str.ToUpper.StartsWith("TOJSON=")
                                sJSON = str.Remove(0, 8).TrimEnd(New Char() {""""c}).ToUpper()
                            Case str.ToUpper.StartsWith("ALTERNATEFORMAT=")
                                sAltFormat = str.Remove(0, 17).TrimEnd(New Char() {""""c})
                            Case str.ToUpper.StartsWith("SELECTEDITEMS=")
                                sSelItems = str.Remove(0, 15).TrimEnd(New Char() {""""c})
                                Dim sSels() As String = sSelItems.Split(",")
                                If Not sSels Is Nothing AndAlso sSels.Length > 0 Then
                                    arrSelItems.AddRange(sSels)
                                End If
                            Case str.ToUpper.StartsWith("SELECTEDFIELD=")
                                sSelField = str.Remove(0, 15).TrimEnd(New Char() {""""c})
                            Case str.ToUpper.StartsWith("NAME=")
                                sName = str.Remove(0, 6).TrimEnd(New Char() {""""c})
                            Case str.ToUpper.StartsWith("USECACHE=")
                                If str.Remove(0, 10).TrimEnd(New Char() {""""c}).Trim.ToUpper = "TRUE" Then
                                    bUseCache = True
                                ElseIf IsNumeric(str.Remove(0, 10).TrimEnd(New Char() {""""c}).Trim) Then
                                    bUseCache = True
                                    sCacheTime = CInt(str.Remove(0, 10).TrimEnd(New Char() {""""c}).Trim)
                                End If
                            Case str.ToUpper.StartsWith("CONNECTION=")
                                sConnection = str.Remove(0, 12).TrimEnd(New Char() {""""c})
                        End Select
                    Next

                    'EXIT IMMEDIATELY IF THIS IS NOT YET DEFINED, AND DOES NOT PROVIDE THE DEFINITION
                    If sQuery.Length > 0 Then
                        If sName.Length > 0 Then
                            DefineSubquery(Caller, sName)
                        End If
                    ElseIf sName.Length > 0 Then
                        If Not isSubqueryDefined(Caller, sName) AndAlso Not bUseCache Then
                            Return False
                        ElseIf bUseCache Then
                            DefineSubquery(Caller, sName)
                        End If
                    End If

                    If bDebug Then
                        Debugger.AppendHeader(Caller.ModuleID, "SubQuery " & CountOfSubqueries(Caller) & "[" & sName & "]", "query_sub", False)
                        Debugger.AppendLine("<br>")
                        Debugger.AppendLine("<ul><li><b>Original: </b>" & Utility.HTMLEncode(Source) & "</li><br>")
                        If Not sConnection Is Nothing AndAlso sConnection.Length > 0 Then
                            Debugger.AppendLine("<li><b>Connection: </b>" & Utility.HTMLEncode(sConnection) & "</li><br>")
                        End If
                    End If

                    'IF THE QUERY WAS NOT PROVIDED, ATTEMPT TO GRAB IT FROM THE CACHE
                    If sQuery Is Nothing OrElse sQuery = "" Then
                        sQuery = Get_Cached_Query(Caller, sName, bUseCache)
                        If Not sQuery Is Nothing AndAlso sQuery.Length > 0 Then
                            Debugger.AppendLine("<li><b>Query: </b>" & "Query was pulled from the cache</li><br>")
                            bcachedQuery = True
                        End If
                    End If

                    'IF THERE IS A QUERY, CONTINUE
                    If Not sQuery Is Nothing AndAlso sQuery.Length > 0 Then
                        If Not bcachedQuery Then
                            'THIS QUERY WAS NOT LOADED FROM THE CACHE, RENDER THE QUERY.
                            sQuery = Caller.RenderQuery(SharedDS, FilterField, FilterText, Caller.RecordsPerPage, RuntimeMessages, IIf(bDebug = True, Debugger, Nothing), sQuery)
                        End If

                        If Not sQuery Is Nothing AndAlso sQuery.Length > 0 Then
                            If bDebug Then
                                Debugger.AppendLine("<li><b>Query: </b>" & Utility.HTMLEncode(sQuery) & "</li><br>")
                            End If

                            Dim cachedData As Object = Nothing
                            'GET THE CACHED DATA, IF IT EXISTS FOR THIS QUERY
                            cachedData = Get_Cached_Data(Caller, sQuery, bUseCache)
                            If Not cachedData Is Nothing Then
                                If bDebug Then
                                    Debugger.AppendLine("<li><b>Data: </b> Data was pulled from the cache</li><br>")
                                End If
                                DS = CType(cachedData, DataSet)
                                bcachedData = True
                            End If
                        Else
                            If bDebug Then
                                Debugger.AppendLine("<li><b>Query: </b>" & "The Resulting Query was Empty" & "</li><br>")
                            End If
                        End If
                    End If

                    If DS Is Nothing Then
                        'GET THE DATA
                        If Not sQuery Is Nothing AndAlso sQuery.Length > 0 Then
                            If sCacheTime Is Nothing Then
                                If sConnection Is Nothing OrElse sConnection.Length = 0 Then
                                    DS = Caller.GetData(True, sQuery, FilterField, FilterText, IIf(bDebug = True, Debugger, Nothing), False, Nothing, Nothing, False)
                                Else
                                    DS = Caller.GetData(True, sQuery, FilterField, FilterText, IIf(bDebug = True, Debugger, Nothing), True, Nothing, Nothing, False, CustomConnection:=sConnection)
                                End If
                            Else
                                If sConnection Is Nothing OrElse sConnection.Length = 0 Then
                                    DS = Caller.GetData(True, sQuery, FilterField, FilterText, IIf(bDebug = True, Debugger, Nothing), False, sName, sCacheTime, True)
                                Else
                                    DS = Caller.GetData(True, sQuery, FilterField, FilterText, IIf(bDebug = True, Debugger, Nothing), True, sName, sCacheTime, True, CustomConnection:=sConnection)
                                End If
                            End If
                            If sCacheTime Is Nothing Then
                                Set_Cached_Query(Caller, sName, bUseCache, sQuery)
                                Set_Cached_Data(Caller, sQuery, bUseCache, DS)
                            End If
                            If bDebug Then
                                If bUseCache Then
                                    Debugger.AppendLine("<li><b>Cache: </b> Data stored in the web cache</li><br>")
                                Else
                                    Debugger.AppendLine("<li><b>Cache: </b> Data stored in the request cache</li><br>")
                                End If
                            End If
                        End If
                    End If

                    If bDebug Then
                        Debugger.AppendLine("<li><b>Query: </b>" & Utility.HTMLEncode(sQuery) & "</li><br>")
                    End If

                    If bDebug Then
                        Debugger.AppendLine("<li><b>Header: </b>" & Utility.HTMLEncode(sHeader) & "</li><br>")
                        Debugger.AppendLine("<li><b>Format: </b>" & Utility.HTMLEncode(sFormat) & "</li><br>")
                        Debugger.AppendLine("<li><b>AltFormat: </b>" & Utility.HTMLEncode(sAltFormat) & "</li><br>")
                        Debugger.AppendLine("<li><b>Footer: </b>" & Utility.HTMLEncode(sFooter) & "</li><br>")
                        Debugger.AppendLine("<li><b>NoQueryFormat: </b>" & Utility.HTMLEncode(sNoQueryFormat) & "</li><br>")
                        Debugger.AppendLine("<li><b>NoResultFormat: </b>" & Utility.HTMLEncode(sNoResultFormat) & "</li><br>")
                        Debugger.AppendLine("<li><b>SelFormat: </b>" & Utility.HTMLEncode(sSelFormat) & "</li><br>")
                        Debugger.AppendLine("<li><b>SelField: </b>" & Utility.HTMLEncode(sSelField) & "</li><br>")
                        Debugger.AppendLine("<li><b>SelItems: </b>" & Utility.HTMLEncode(sSelItems) & "</li><br>")
                        Debugger.AppendLine("<li><b>ToJSON: </b>" & Utility.HTMLEncode(sJSON) & "</li><br>")
                    End If


                    If sQuery Is Nothing OrElse sQuery.Length = 0 Then
                        sbvalue.Append(sNoQueryFormat)
                    ElseIf Not sJSON Is Nothing AndAlso sJSON.Length > 0 Then
                        Dim jsonNames As String() = sJSON.Split(",")
                        If Not DS Is Nothing AndAlso DS.Tables.Count > 0 Then
                            Dim xi As Integer
                            For xi = 0 To DS.Tables.Count - 1
                                If (xi > 0) Then
                                    sbvalue.Append(",")
                                End If
                                Dim dname As String = jsonNames(0)
                                If (xi < jsonNames.Length) Then
                                    dname = jsonNames(xi)
                                End If
                                sbvalue.Append("{""" + dname + """:")
                                sbvalue.Append(JSON.JsonDataTable.ConvertTableToJson(DS.Tables(xi)))
                                sbvalue.Append("}")
                            Next
                        End If
                    ElseIf Not sFormat Is Nothing AndAlso sFormat.Length > 0 Then
                        Dim dt As DataTable = Nothing, iTable As Integer = 0  'Use to iterate through tables
                        Dim drSub As DataRow = Nothing, iIndex As Integer = -1
                        Dim isAlternate As Boolean = False
                        Dim dc As DataColumn = Nothing

                        If Not DS Is Nothing AndAlso DS.Tables.Count > 0 AndAlso DS.Tables(0).Rows.Count > 0 Then
                            dt = DS.Tables(iTable)
                            'RENDER HEADER
                            'WE MAY HAVE TO ADD THE DATA TABLE INTO THE SHAREDDS.. BUT THIS IS NOT OPTIMAL
                            'If SharedDS.Tables.Contains(sName) Then
                            '    SharedDS.Tables.Remove(sName)
                            'End If
                            'dt.TableName = sName
                            'SharedDS.Tables.Add(dt.Clone)

                            Dim sRep As String
                            If Not sHeader Is Nothing AndAlso sHeader.Length > 0 Then
                                sRep = sHeader
                                Caller.RenderString(-2, sRep, startvalues, endvalues, escapechar, SharedDS, drSub, RuntimeMessages, False, False, NullReturn:=NullReturn, ProtectSession:=ProtectSession, SessionDelimiter:=SessionDelimiter, useSessionQuotes:=useSessionQuotes, DebugWriter:=Debugger)
                                sbvalue.Append(sRep)
                            End If

                            For Each drSub In dt.Rows
                                sRep = ""

                                If Not isAlternate Or (sAltFormat = "") Then
                                    sRep = sFormat
                                ElseIf isAlternate And (sAltFormat <> "") Then
                                    sRep = sAltFormat
                                End If
                                If sSelField <> "" AndAlso sSelFormat <> "" AndAlso arrSelItems.Count > 0 Then
                                    Dim sSelValue As String = Utility.CNullData(drSub, sSelField)
                                    If arrSelItems.Contains(sSelValue) Then
                                        sRep = sSelFormat
                                    End If
                                End If

                                'RenderString(0, sRep, startvalues, endvalues, escapechar, DSSub, dr, RuntimeMessages, False, False, NullReturn:=NullReturn, ProtectSession:=ProtectSession, SessionDelimiter:=SessionDelimiter, useSessionQuotes:=useSessionQuotes, Debugger:=Debugger)
                                'VERSION: 1.9.9 - Changed Subquery Rendering to include Header, Footer, NoQuery, NoResult and modified to include the Index (negative) to force alternate to work as prescribed.
                                Caller.RenderString(iIndex, sRep, startvalues, endvalues, escapechar, SharedDS, drSub, RuntimeMessages, False, False, NullReturn:=NullReturn, ProtectSession:=ProtectSession, SessionDelimiter:=SessionDelimiter, useSessionQuotes:=useSessionQuotes, DebugWriter:=Debugger)

                                sbvalue.Append(sRep)
                                iIndex -= 1
                                isAlternate = Not isAlternate
                            Next

                            'RENDER FOOT
                            If Not sFooter Is Nothing AndAlso sFooter.Length > 0 Then
                                sRep = sFooter
                                Caller.RenderString(iIndex, sRep, startvalues, endvalues, escapechar, SharedDS, drSub, RuntimeMessages, False, False, NullReturn:=NullReturn, ProtectSession:=ProtectSession, SessionDelimiter:=SessionDelimiter, useSessionQuotes:=useSessionQuotes, DebugWriter:=Debugger)
                                sbvalue.Append(sRep)
                            End If
                            'SharedDS.Tables.Remove(sName)
                        Else
                            'NO RESULT
                            sbvalue.Append(sNoResultFormat)
                        End If
                    End If
                Catch ex As Exception
                    If bDebug Then
                        Debugger.AppendLine("<span style=""color: #FF0000; font-weight: bold;"">Error: </span>" & Utility.HTMLEncode(ex.Message) & "<br>" & Utility.HTMLEncode(ex.StackTrace) & "<br>")
                    End If
                End Try

                If bDebug Then
                    Debugger.AppendFooter(False)
                End If

                REPLACED = True
                Source = sbvalue.ToString
            End If
            Return REPLACED
        End Function

        Private Function CountOfSubqueries(ByRef caller As Engine) As Integer
            Return getSubqueries(caller).Count
        End Function
        Private Function getSubqueries(ByRef Caller As Engine) As List(Of String)
            If Not Caller.Context.Items.Contains("ows.subqueries") Then
                Caller.Context.Items("ows.subqueries") = New List(Of String)
            End If
            Return CType(Caller.Context.Items("ows.subqueries"), List(Of String))
        End Function
        Private Sub setSubqueries(ByRef Caller As Engine, ByVal content As List(Of String))
            Caller.Context.Items("ows.subqueries") = content
        End Sub
        Private Function isSubqueryDefined(ByRef Caller As Engine, ByVal name As String) As Boolean
            If Not name Is Nothing AndAlso name.Length > 0 Then
                If getSubqueries(Caller).Contains(name) Then
                    Return True
                End If
            End If
            Return False
        End Function
        Private Sub DefineSubquery(ByRef Caller As Engine, ByVal name As String)
            If Not name Is Nothing AndAlso name.Length > 0 Then
                If Not isSubqueryDefined(Caller, name) Then
                    Dim content As List(Of String) = getSubqueries(Caller)
                    content.Add(name)
                    setSubqueries(Caller, content)
                End If
            End If
        End Sub
    End Class
End Namespace