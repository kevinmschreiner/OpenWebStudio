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
Imports r2i.OWS.Framework.Utilities
Imports r2i.OWS.Framework
Imports r2i.OWS.Framework.Plugins.Renderers
Namespace r2i.OWS.Renderers
    Public Class RenderColumns
        Inherits RenderBase

        Public Overrides ReadOnly Property RenderTag() As String
            Get
                Return "COLUMNS"
            End Get
        End Property

        Public Overrides ReadOnly Property RenderType() As RenderTypes
            Get
                Return RenderTypes.Functional
            End Get
        End Property

        Public Overrides ReadOnly Property CanPreRender() As Boolean
            Get
                Return False
            End Get
        End Property

        Public Overrides Function Handle_Render(ByRef Caller As EngineBase, ByVal Index As Integer, ByRef Source As String, ByRef DS As System.Data.DataSet, ByRef DR As System.Data.DataRow, ByRef RuntimeMessages As System.Collections.Generic.SortedList(Of String, String), ByVal NullReturn As Boolean, ByVal NullOverride As Boolean, ByVal ProtectSession As Boolean, ByVal SessionDelimiter As String, ByVal useSessionQuotes As Boolean, ByVal useAggregations As Boolean, ByRef FilterText As String, ByRef FilterField As String, ByRef Debugger As r2i.OWS.Framework.Debugger) As Boolean
            'VERSION: 1.9.6 : Added COLUMNS Tag - {COLUMNS,COLUMNTEMPLATE,SEPARATORTEMPLATE,SHOWSEPARATOR,IGNOREDCOLUMNS}
            Dim REPLACED As Boolean = False
            Dim parameters As String() = ParameterizeString(Source, ","c, """"c, "\"c)
            If Not parameters Is Nothing AndAlso parameters.Length >= 2 Then
                Dim VALUE As String = parameters(1)
                Dim SEPARATOR As String = ""
                Dim SHOWSEPARATOR As Boolean = False
                'ROMAIN: Generic replacement - 08/20/2007
                'Dim SkippedColumns As ArrayList
                Dim SkippedColumns As List(Of String) = Nothing
                If parameters.Length >= 3 Then
                    SEPARATOR = parameters(2)
                End If
                If parameters.Length >= 4 Then
                    If parameters(3).ToUpper = "TRUE" Then
                        SHOWSEPARATOR = True
                    End If
                End If
                If parameters.Length >= 5 Then
                    If parameters(4).IndexOf(",") > 0 Then
                        'THIS CONTAINS IGNORED COLUMNS
                        'ROMAIN: Generic replacement - 08/20/2007
                        'SkippedColumns = New ArrayList(parameters(4).ToUpper.Split(","))
                        SkippedColumns = New List(Of String)(parameters(4).ToUpper.Split(","))
                    Else
                        'ROMAIN: Generic replacement - 08/20/2007
                        'SkippedColumns = New ArrayList
                        SkippedColumns = New List(Of String)
                        'EACH ITEM BEYOND 4 is an ignored column
                        Dim icParameter As Integer = 4
                        While icParameter < parameters.Length
                            SkippedColumns.Add(parameters(icParameter).ToUpper)
                            icParameter += 1
                        End While
                    End If
                End If
                Dim col As DataColumn = Nothing
                Dim columnIndex As Integer = 0
                Dim result As String = ""
                Dim cValue As String = Nothing
                Dim cSeparator As String = Nothing
                For Each col In DS.Tables(0).Columns
                    If SkippedColumns Is Nothing OrElse Not SkippedColumns.Contains(col.ColumnName.ToUpper) Then
                        'ADD THE BETWEEN SEPARATOR
                        If columnIndex > 0 Then
                            result &= cSeparator
                        End If

                        cSeparator = SEPARATOR
                        cValue = VALUE


                        RenderString_Columns_Column(col, cValue)


                        RenderString_Columns_Column(col, cSeparator)

                        'ADD THE BEFORE SEPARATOR
                        If columnIndex = 0 AndAlso SHOWSEPARATOR Then
                            result &= cSeparator
                        End If
                        result &= cValue

                        columnIndex += 1
                    End If
                Next
                If columnIndex > 0 AndAlso SHOWSEPARATOR Then
                    result &= cSeparator
                End If

                Source = result
                REPLACED = True
            End If
            Return REPLACED
        End Function
        Private Sub RenderString_Columns_Column(ByRef col As DataColumn, ByRef cValue As String)
            Dim cIndex As Integer
            Dim cLength As Integer
            Dim cRValue As String = Nothing
            'Version: 1.9.8 - [Column.Type,Column.Index,Column.Readonly,Column.AllowNulls]
            Dim colname As String = "[COLUMN.NAME,SYSTEM]"
            Dim coltype As String = "[COLUMN.TYPE,SYSTEM]"
            Dim colindex As String = "[COLUMN.INDEX,SYSTEM]"
            Dim colreadonly As String = "[COLUMN.READONLY,SYSTEM]"
            Dim colnull As String = "[COLUMN.ALLOWNULLS,SYSTEM]"

            cRValue = col.ColumnName
            cIndex = cValue.ToUpper.IndexOf(colname)
            cLength = colname.Length
            If cIndex < 0 Then
                cRValue = col.Ordinal
                cIndex = cValue.ToUpper.IndexOf(colindex)
                cLength = colindex.Length
            End If
            If cIndex < 0 Then
                cRValue = col.DataType.ToString
                cIndex = cValue.ToUpper.IndexOf(coltype)
                cLength = coltype.Length
            End If
            If cIndex < 0 Then
                cRValue = col.ReadOnly.ToString
                cIndex = cValue.ToUpper.IndexOf(colreadonly)
                cLength = colreadonly.Length
            End If
            If cIndex < 0 Then
                cRValue = col.AllowDBNull.ToString
                cIndex = cValue.ToUpper.IndexOf(colnull)
                cLength = colnull.Length
            End If
            While cIndex >= 0
                cValue = cValue.Substring(0, cIndex) & cRValue & cValue.Substring(cIndex + cLength)

                cRValue = col.ColumnName
                cIndex = cValue.ToUpper.IndexOf(colname)
                cLength = colname.Length
                If cIndex < 0 Then
                    cRValue = col.Ordinal
                    cIndex = cValue.ToUpper.IndexOf(colindex)
                    cLength = colindex.Length
                End If
                If cIndex < 0 Then
                    cRValue = col.DataType.ToString
                    cIndex = cValue.ToUpper.IndexOf(coltype)
                    cLength = coltype.Length
                End If
                If cIndex < 0 Then
                    cRValue = col.ReadOnly.ToString
                    cIndex = cValue.ToUpper.IndexOf(colreadonly)
                    cLength = colreadonly.Length
                End If
                If cIndex < 0 Then
                    cRValue = col.AllowDBNull.ToString
                    cIndex = cValue.ToUpper.IndexOf(colnull)
                    cLength = colnull.Length
                End If
            End While
        End Sub
    End Class
End Namespace