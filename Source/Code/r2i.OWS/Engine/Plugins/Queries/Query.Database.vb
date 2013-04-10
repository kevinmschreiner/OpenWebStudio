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
Imports r2i.OWS.Framework.Plugins.Queries
Imports System.Collections.Generic

Namespace r2i.OWS.Queries
    Public Class Database
        Inherits QueryBase

        Public Overrides ReadOnly Property QueryTag() As String
            Get
                Return "<DATABASE>"
            End Get
        End Property

        Public Overrides ReadOnly Property QueryStructure() As String
            Get
                Return "STANDARD ANSI SQL"
            End Get
        End Property

        Public Overrides Function Handle_GetData(ByRef Caller As EngineBase, ByVal isSubQuery As Boolean, ByVal Query As String, ByVal FilterField As String, ByVal FilterText As String, ByRef DebugWriter As Framework.Debugger, ByVal isRendered As Boolean, Optional ByVal timeout As Integer = -1, Optional ByVal CustomConnection As String = Nothing) As OWS.Framework.RuntimeBase.QueryResult
            Dim rslt As New Framework.RuntimeBase.QueryResult(RuntimeBase.ExecutableResultEnum.Executed, New DataSet)

            'SORT THE QUERY VARIABLES
            Try
                If Not Query Is Nothing AndAlso Query.Length > 0 Then
                    If CustomConnection Is Nothing OrElse CustomConnection.Length = 0 Then
                        Dim strCustomConnection As String = Nothing
                        If Not Caller.xls.customConnection Is Nothing AndAlso Caller.xls.customConnection.Length > 0 Then
                            'If isRendered = False Then
                            strCustomConnection = Caller.RenderString(Nothing, Caller.xls.customConnection, Caller.CapturedMessages, False, False, FilterText:=FilterText, FilterField:=FilterField, DebugWriter:=DebugWriter)
                            'Else
                            '   strCustomConnection = xls.customConnection
                            'End If
                        End If
                        Dim notODBCOLEDB As Boolean = False
                        If Not strCustomConnection Is Nothing AndAlso strCustomConnection.ToUpper.IndexOf("DRIVER=") < 0 AndAlso strCustomConnection.ToUpper.IndexOf("PROVIDER=") < 0 Then
                            notODBCOLEDB = True
                        End If
                        If strCustomConnection Is Nothing OrElse strCustomConnection.Length = 0 OrElse notODBCOLEDB Then
                            'VERSION: 1.9.7 - Support for Query Separation for DBMS applications which do not support multiple recordset returns (like ORACLE).
                            Dim xlc As New Controller
                            Dim connectionString As String = Nothing
                            If notODBCOLEDB Then
                                connectionString = strCustomConnection
                            Else
                                connectionString = AbstractFactory.Instance.EngineController.GetConnectionString()
                            End If

                            If Caller.xls.enabledForcedQuerySplit Then
                                'NEW
                                'ROMAIN: Generic replacement - 08/20/2007
                                'Dim queries As ArrayList = xlc.ParseScript(Query, "GO")

                                Dim queries As List(Of String) = xlc.ParseScript(Query, "GO")
                                If queries.Count > 0 Then
                                    Dim i As Integer
                                    For i = 0 To queries.Count - 1
                                        ' Dim ids As DataSet = xlc.GetDataset(DotNetNuke.Common.GetDBConnectionString, queries(i), timeout)
                                        Dim ids As DataSet = xlc.GetDataset(connectionString, queries(i), timeout)
                                        If Not ids Is Nothing AndAlso ids.Tables.Count > 0 Then
                                            Dim dt As DataTable
                                            For Each dt In ids.Tables
                                                dt.TableName = "Table" & rslt.Value.Tables.Count.ToString
                                                rslt.Value.Tables.Add(dt.Copy)
                                            Next
                                        End If
                                    Next
                                    queries = Nothing
                                End If
                            Else
                                'ORIGINAL
                                'ds = xlc.GetDataset(DotNetNuke.Common.GetDBConnectionString, Query, timeout)
                                rslt.Value = xlc.GetDataset(connectionString, Query, timeout)
                            End If
                        Else
                            strCustomConnection = Connection_ParseForCommandTimeout(strCustomConnection, timeout)
                            If strCustomConnection.ToUpper.IndexOf("DRIVER=") >= 0 Then
                                'ODBC
                                'VERSION: 1.9.7 - ODBC Support for Query Separation for DBMS applications which do not support multiple recordset returns (like ORACLE).
                                Dim oda As New System.Data.Odbc.OdbcDataAdapter(Query, strCustomConnection)
                                If Caller.xls.enabledForcedQuerySplit Then
                                    'NEW
                                    Dim xlc As New Controller
                                    'ROMAIN: Generic replacement - 08/20/2007
                                    'Dim queries As ArrayList = xlc.ParseScript(Query, "GO")
                                    Dim queries As List(Of String) = xlc.ParseScript(Query, "GO")
                                    If queries.Count > 0 Then
                                        Dim i As Integer
                                        For i = 0 To queries.Count - 1
                                            Dim ids As New DataSet
                                            oda.SelectCommand.CommandText = queries(i)
                                            If timeout > -1 Then
                                                oda.SelectCommand.CommandTimeout = timeout
                                            End If
                                            oda.Fill(ids)

                                            If Not ids Is Nothing AndAlso ids.Tables.Count > 0 Then
                                                Dim dt As DataTable
                                                For Each dt In ids.Tables
                                                    dt.TableName = "Table" & rslt.Value.Tables.Count.ToString
                                                    rslt.Value.Tables.Add(dt.Copy)
                                                Next
                                            End If
                                        Next
                                        queries = Nothing
                                    End If
                                Else
                                    'ORIGINAL
                                    If timeout > -1 Then
                                        oda.SelectCommand.CommandTimeout = timeout
                                    End If

                                    oda.Fill(rslt.Value)
                                End If
                            ElseIf strCustomConnection.ToUpper.IndexOf("PROVIDER=") >= 0 Then
                                'OLEDB
                                'VERSION: 1.9.7 - OLEDB Support for Query Separation for DBMS applications which do not support multiple recordset returns (like ORACLE).
                                Dim da As New System.Data.OleDb.OleDbDataAdapter(Query, strCustomConnection)
                                If Caller.xls.enabledForcedQuerySplit Then
                                    'NEW
                                    Dim xlc As New Controller
                                    'ROMAIN: Generic replacement - 08/20/2007
                                    'Dim queries As ArrayList = xlc.ParseScript(Query, "GO")
                                    Dim queries As List(Of String) = xlc.ParseScript(Query, "GO")
                                    If queries.Count > 0 Then
                                        Dim i As Integer
                                        For i = 0 To queries.Count - 1
                                            Dim ids As New DataSet
                                            da.SelectCommand.CommandText = queries(i)
                                            If timeout > -1 Then
                                                da.SelectCommand.CommandTimeout = timeout
                                            End If
                                            da.Fill(ids)

                                            If Not ids Is Nothing AndAlso ids.Tables.Count > 0 Then
                                                Dim dt As DataTable
                                                For Each dt In ids.Tables
                                                    dt.TableName = "Table" & rslt.Value.Tables.Count.ToString
                                                    rslt.Value.Tables.Add(dt.Copy)
                                                Next
                                            End If
                                        Next
                                        queries = Nothing
                                    End If
                                Else
                                    'ORIGINAL
                                    If timeout > -1 Then
                                        da.SelectCommand.CommandTimeout = timeout
                                    End If
                                    da.Fill(rslt.Value)
                                End If
                            End If
                        End If
                    Else
                        CustomConnection = Connection_ParseForCommandTimeout(CustomConnection, timeout)
                        If CustomConnection.ToUpper.IndexOf("DRIVER=") >= 0 Then
                            'ODBC
                            'VERSION: 1.9.7 - ODBC Support for Query Separation for DBMS applications which do not support multiple recordset returns (like ORACLE).
                            Dim oda As New System.Data.Odbc.OdbcDataAdapter(Query, CustomConnection)
                            If Caller.xls.enabledForcedQuerySplit Then
                                'NEW
                                Dim xlc As New Controller
                                'ROMAIN: Generic replacement - 08/20/2007
                                'Dim queries As ArrayList = xlc.ParseScript(Query, "GO")
                                Dim queries As List(Of String) = xlc.ParseScript(Query, "GO")
                                If queries.Count > 0 Then
                                    Dim i As Integer
                                    For i = 0 To queries.Count - 1
                                        Dim ids As New DataSet
                                        oda.SelectCommand.CommandText = queries(i)
                                        If timeout > -1 Then
                                            oda.SelectCommand.CommandTimeout = timeout
                                        End If
                                        oda.Fill(ids)

                                        If Not ids Is Nothing AndAlso ids.Tables.Count > 0 Then
                                            Dim dt As DataTable
                                            For Each dt In ids.Tables
                                                dt.TableName = "Table" & rslt.Value.Tables.Count.ToString
                                                rslt.Value.Tables.Add(dt.Copy)
                                            Next
                                        End If
                                    Next
                                    queries = Nothing
                                End If
                            Else
                                'ORIGINAL
                                If timeout > -1 Then
                                    oda.SelectCommand.CommandTimeout = timeout
                                End If

                                oda.Fill(rslt.Value)
                            End If
                        ElseIf CustomConnection.ToUpper.IndexOf("PROVIDER=") >= 0 Then
                            'OLEDB
                            'VERSION: 1.9.7 - OLEDB Support for Query Separation for DBMS applications which do not support multiple recordset returns (like ORACLE).
                            Dim da As New System.Data.OleDb.OleDbDataAdapter(Query, CustomConnection)
                            If Caller.xls.enabledForcedQuerySplit Then
                                'NEW
                                Dim xlc As New Controller
                                'ROMAIN: Generic replacement - 08/20/2007
                                'Dim queries As ArrayList = xlc.ParseScript(Query, "GO")
                                Dim queries As List(Of String) = xlc.ParseScript(Query, "GO")
                                If queries.Count > 0 Then
                                    Dim i As Integer
                                    For i = 0 To queries.Count - 1
                                        Dim ids As New DataSet
                                        da.SelectCommand.CommandText = queries(i)
                                        If timeout > -1 Then
                                            da.SelectCommand.CommandTimeout = timeout
                                        End If
                                        da.Fill(ids)

                                        If Not ids Is Nothing AndAlso ids.Tables.Count > 0 Then
                                            Dim dt As DataTable
                                            For Each dt In ids.Tables
                                                dt.TableName = "Table" & rslt.Value.Tables.Count.ToString
                                                rslt.Value.Tables.Add(dt.Copy)
                                            Next
                                        End If
                                    Next
                                    queries = Nothing
                                End If
                            Else
                                'ORIGINAL
                                If timeout > -1 Then
                                    da.SelectCommand.CommandTimeout = timeout
                                End If
                                da.Fill(rslt.Value)
                            End If
                        Else
                            'VERSION: 1.9.7 - Support for Query Separation for DBMS applications which do not support multiple recordset returns (like ORACLE).
                            Dim xlc As New Controller
                            If Caller.xls.enabledForcedQuerySplit Then
                                'NEW
                                'ROMAIN: Generic replacement - 08/20/2007
                                'Dim queries As ArrayList = xlc.ParseScript(Query, "GO")

                                Dim queries As List(Of String) = xlc.ParseScript(Query, "GO")
                                If queries.Count > 0 Then
                                    Dim i As Integer
                                    For i = 0 To queries.Count - 1
                                        ' Dim ids As DataSet = xlc.GetDataset(DotNetNuke.Common.GetDBConnectionString, queries(i), timeout)
                                        Dim ids As DataSet = xlc.GetDataset(CustomConnection, queries(i), timeout)
                                        If Not ids Is Nothing AndAlso ids.Tables.Count > 0 Then
                                            Dim dt As DataTable
                                            For Each dt In ids.Tables
                                                dt.TableName = "Table" & rslt.Value.Tables.Count.ToString
                                                rslt.Value.Tables.Add(dt.Copy)
                                            Next
                                        End If
                                    Next
                                    queries = Nothing
                                End If
                            Else
                                'ORIGINAL
                                'ds = xlc.GetDataset(DotNetNuke.Common.GetDBConnectionString, Query, timeout)
                                rslt.Value = xlc.GetDataset(CustomConnection, Query, timeout)
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                'sortActionList = Nothing
                rslt.Result = RuntimeBase.ExecutableResultEnum.Failed
                rslt.Error = ex
                Framework.Utilities.Utility.SortStatus(Caller.Session, Caller.ConfigurationID.ToString.Replace("{", "").Replace("}", "").Replace("-", ""), Caller.ModuleID, Caller.UserID) = Nothing
            End Try
            Return rslt
        End Function

        Private Function Connection_ParseForCommandTimeout(ByVal Value As String, ByRef Timeout As Integer) As String
            Dim ctLocation As Integer = Value.ToUpper.IndexOf("COMMANDTIMEOUT=")
            If ctLocation >= 0 Then
                Dim eIndex As Integer = Value.IndexOf(";", ctLocation + 1)
                If eIndex = -1 And ctLocation >= 0 Then
                    Value &= ";"
                    eIndex = Value.IndexOf(";", ctLocation + 1)
                End If
                If eIndex > ctLocation Then
                    Dim ct As String = Value.Substring(ctLocation + 15, eIndex - ctLocation - 15)
                    If IsNumeric(ct) Then
                        If ctLocation >= 0 AndAlso eIndex < Value.Length Then
                            Timeout = CInt(ct)
                            Return Value.Substring(0, ctLocation) & Value.Substring(eIndex + 1)
                        ElseIf ctLocation >= 0 Then
                            Timeout = CInt(ct)
                            Return Value.Substring(0, ctLocation)
                        End If
                    End If
                End If
            End If
            Timeout = -1
            Return Value
        End Function
    End Class
End Namespace
