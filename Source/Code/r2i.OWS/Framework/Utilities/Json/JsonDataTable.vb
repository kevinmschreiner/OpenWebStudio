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
    ''' Builds a JSON string based on a DataTable in the following format: 
    ''' {"Columns":[ 
    '''8 {"Name":"ContactID"}, 
    '''9 {"Name":"FirstName"}, 
    '''10 {"Name":"LastName"}, 
    '''11 {"Name":"Company"}, 
    '''12 {"Name":"Department"}, 
    '''13 {"Name":"Title"}, 
    '''14 {"Name":"Email"}, 
    '''15 {"Name":"Phone"}, 
    '''16 {"Name":"Address"}, 
    '''17 {"Name":"City"}, 
    '''18 {"Name":"Region"}, 
    '''19 {"Name":"PostalCode"}, 
    '''20 {"Name":"Country"}, 
    '''21 {"Name":"UserName"}], 
    '''22 "Rows":[ 
    '''23 {"Columns":["1","Paul","Anderson","Corporate America","Sales","Director","panderson@camerica.com","420-232-4231","123 Oakdale Ave.","Columbia","MD","21046","USA","panders"]}, 
    '''24 {"Columns":["2","Steve","Bronson","Windows Unlimited","Sales","Director","sabronson@wutld.com","420-232-4231","123 Oakdale Ave.","Columbia","MD","21046","USA","panders"]}, 
    '''25 {"Columns":["3","Debbie","Charles","Doors to Tomorrow","Sales","Director","dcharles@d2t.com","420-232-4231","123 Oakdale Ave.","Columbia","MD","21046","USA","panders"]}, 
    '''26 {"Columns":["4","John","Dobb","Stocks And Bonds","Sales","Director","jdobb@standBonds.com","420-232-4231","123 Oakdale Ave.","Columbia","MD","21046","USA","panders"]}, 
    '''27 {"Columns":["5","Wayne","Earl","Clients Corporation","Sales","Director","wearl@ccorporate.com","420-232-4231","123 Oakdale Ave.","Columbia","MD","21046","USA","panders"]} 
    '''28 ] 
    '''29} 
    ''' </summary> 
    Public Class JsonDataTable
        ''' <summary> 
        ''' Conv 
        ''' </summary> 
        ''' <param name="dt"></param> 
        ''' <returns></returns> 
        Public Shared Function ConvertTableToJson(ByVal dt As DataTable) As String
            Return ConvertTableToJson(dt, dt.Rows.Count)
        End Function
        <CLSCompliant(False)> Public Shared Sub GenerateRow(ByVal rowJ As JavaScriptObject, ByVal cols As String, ByVal dt As DataTable)
            Dim colList As String() = GetColumns(cols)
            Dim vals As Object() = New Object(colList.Length - 1) {}
            Dim i As Integer = 0
            For Each col As String In colList
                vals(i) = getJsonNode(rowJ, col)
                i += 1
            Next
            dt.Rows.Add(vals)
        End Sub
        Public Shared Function GetColumns(ByVal input As String) As String()
            Return input.Split(","c)
        End Function
        ''' <summary>
        ''' Fetches a JSON node from the path.
        ''' </summary>
        ''' <param name="o">JSON object to traverse</param>
        ''' <param name="path">JSON object path ie</param>
        ''' <returns>The best effort at find the new node</returns>
        Public Shared Function GetJsonNode(ByVal o As Object, ByVal path As String) As Object
            ' this was a copy and paste, repointed to original function
            Return JsonToDataset.getJsonNode(o, path)
        End Function
        Public Shared Function ConvertTableToJson(ByVal dt As DataTable, ByVal maxRowCount As Integer) As String
            Dim jsonTable As New JavaScriptObject()
            jsonTable.Add("Columns", getJsonColumns(dt))
            jsonTable.Add("Rows", getJsonRows(dt, maxRowCount))
            Dim jsonString As String = JavaScriptConvert.SerializeObject(jsonTable)
            Return jsonString
        End Function

        Private Shared Function getJsonColumns(ByVal dt As DataTable) As JavaScriptArray
            Dim jsonColumns As New JavaScriptArray()
            For Each col As DataColumn In dt.Columns
                jsonColumns.Add(getColumnObject(col))
            Next
            Return jsonColumns
        End Function

        Private Shared Function getJsonRows(ByVal dr As DataTable, ByVal maxRowCount As Integer) As JavaScriptArray
            Dim jsonRows As New JavaScriptArray()
            Dim row As DataRow
            Dim i As Integer = 0
            While i < dr.Rows.Count AndAlso i < maxRowCount
                row = dr.Rows(i)
                jsonRows.Add(getJsonRow(row))
                i += 1
            End While
            Return jsonRows
        End Function

        Private Shared Function getJsonRow(ByVal dr As DataRow) As JavaScriptObject
            Dim rowContainer As New JavaScriptObject()
            Dim row As New JavaScriptArray()
            For Each item As Object In dr.ItemArray
                row.Add(item.ToString())
            Next
            rowContainer.Add("Columns", row)
            Return rowContainer
        End Function

        Private Shared Function getColumnObject(ByVal col As DataColumn) As JavaScriptObject
            Return getColumnObject(col.ColumnName, col.DataType.Name, col.[ReadOnly], col.MaxLength)
        End Function

        Private Shared Function getColumnObject(ByVal Name As String, ByVal Type As String, ByVal [ReadOnly] As Boolean, ByVal MaxLength As Integer) As JavaScriptObject
            Dim jso As New JavaScriptObject()
            jso.Add("Name", Name)
            jso.Add("Type", Type)
            jso.Add("ReadOnly", [ReadOnly].ToString())
            jso.Add("Length", MaxLength.ToString())
            Return jso
        End Function

    End Class

End Namespace
