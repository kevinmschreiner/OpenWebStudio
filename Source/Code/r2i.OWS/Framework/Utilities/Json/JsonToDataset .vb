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
Imports System
Imports r2i.OWS.Newtonsoft.Json
Imports System.Collections.Generic
Imports System.Data

Namespace r2i.OWS.Framework.Utilities.JSON
    ''' <summary> 
    ''' Builds a System.Data.DataTable based on a json object and a series of columns. 
    ''' </summary> 
    Public Class JsonToDataset

        ''' <summary> 
        ''' Fetches a DataTable from a JSON object. 
        ''' </summary> 
        ''' <param name="o">JSON object to iterate over.</param> 
        ''' <param name="TableName">Name of the System.Data.DataTable to create</param> 
        ''' <param name="columns">Array of columns to map.</param> 
        ''' <param name="columnMappings">key, value pairs with the values being the paths to the data to extract.</param> 
        ''' <returns></returns> 
        Public Shared Function renderJsonObject(ByVal o As Object, ByVal TableName As String, ByVal columns As String(), ByVal columnMappings As Dictionary(Of String, String)) As DataTable
            Dim dt As New DataTable(TableName)
            For Each col As String In columns
                dt.Columns.Add(col)
            Next
            If o IsNot Nothing AndAlso o.[GetType]() Is GetType(JavaScriptArray) Then
                For Each row As Object In DirectCast(o, JavaScriptArray)
                    Dim rowJ As JavaScriptObject = DirectCast(row, JavaScriptObject)
                    buildRow(rowJ, dt, columns, columnMappings)
                Next
            ElseIf o IsNot Nothing AndAlso o.[GetType]() Is GetType(JavaScriptObject) Then
                Dim rowJ As JavaScriptObject = DirectCast(o, JavaScriptObject)
                buildRow(rowJ, dt, columns, columnMappings)
            End If
            Return dt
        End Function

        ''' <summary> 
        ''' Create a DataTable with the default name of Table1. 
        ''' </summary> 
        ''' <param name="o"></param> 
        ''' <param name="columns"></param> 
        ''' <param name="columnMappings"></param> 
        ''' <returns></returns> 
        Public Shared Function renderJsonObject(ByVal o As Object, ByVal columns As String(), ByVal columnMappings As Dictionary(Of String, String)) As DataTable
            Return renderJsonObject(o, "Table1", columns, columnMappings)
        End Function


        Private Shared Sub buildRow(ByVal rowJ As JavaScriptObject, ByVal dt As DataTable, ByVal columns As String(), ByVal columnMappings As Dictionary(Of String, String))
            Dim vals As Object() = New Object(columns.Length - 1) {}
            Dim i As Integer = 0
            For Each col As String In columns
                vals(i) = getJsonNode(rowJ, columnMappings(col))
                i += 1
            Next
            dt.Rows.Add(vals)
        End Sub


        ''' <summary> 
        ''' Fetches a JSON node from the path. 
        ''' </summary> 
        ''' <param name="o">JSON object to traverse</param> 
        ''' <param name="path">JSON object path ie</param> 
        ''' <returns>The best effort at find the new node</returns> 
        Public Shared Function getJsonNode(ByVal o As Object, ByVal path As String) As Object
            For Each item As String In path.Split("."c)
                If item.Length = 0 Then
                    Exit For
                End If
                If o IsNot Nothing Then
                    If item.IndexOf("["c) >= 0 Then
                        If item.IndexOf("["c) = 0 Then
                            If o.[GetType]() Is GetType(JavaScriptArray) Then
                                Dim itemX As Integer = Integer.Parse(item.Substring(1, item.Length - 2))
                                o = DirectCast(o, JavaScriptArray)(itemX)
                            End If
                        Else
                            Dim itemRoot As String = item.Substring(0, item.IndexOf("["c))
                            Dim bracketIndex As Integer = item.IndexOf("["c)
                            If o.[GetType]() Is GetType(JavaScriptObject) Then
                                o = DirectCast(o, JavaScriptObject)(itemRoot)
                            End If
                            If o.[GetType]() Is GetType(JavaScriptArray) Then
                                Dim itemX As Integer = Integer.Parse(item.Substring(bracketIndex + 1, item.Length - 2 - bracketIndex))
                                If DirectCast(o, JavaScriptArray).Count > itemX Then
                                    o = DirectCast(o, JavaScriptArray)(itemX)
                                Else
                                    o = Nothing
                                End If
                            End If
                        End If
                    Else
                        If o.[GetType]() Is GetType(JavaScriptObject) AndAlso DirectCast(o, JavaScriptObject).ContainsKey(item) Then
                            o = DirectCast(o, JavaScriptObject)(item)
                        Else
                            o = Nothing
                        End If
                    End If
                End If
            Next
            Return o
        End Function
    End Class
End Namespace
