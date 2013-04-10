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
Imports r2i.OWS.Framework.Utilities.JSON
Imports r2i.OWS.Newtonsoft.Json

Namespace r2i.OWS.Framework.Plugins.Actions
    <Serializable()> _
      Public Class MessageAction_File_ColumnMappings

        <Serializable()> _
        Public Class MessageAction_File_ColumnMappingItem

            Public Position As String
            Public Name As String
            Public Target As String
            Public Type As String
            Public DefaultValue As String
            Public NullValue As String
            Public Format As String
            Public Index As Integer
            Public StartColumn As String
            Public EndColumn As String
            Public FileType As String


        End Class

        Public ColumnMappings As New List(Of MessageAction_File_ColumnMappingItem)
        Public Sub New()
            ColumnMappings = New List(Of MessageAction_File_ColumnMappingItem)
        End Sub
        <CLSCompliant(False)> Public Sub New(ByVal Source As Newtonsoft.Json.JavaScriptArray)
            Dim maFileColumnMappings As New MessageAction_File_ColumnMappings
            'Dim lstmaFileCM As List(Of MessageAction_File_ColumnMappingItem) = JavaScriptConvert.DeserializeObject(Source)
            Dim i As Integer = 0
            For Each jo As JavaScriptObject In Source
                Dim mafcm As New MessageAction_File_ColumnMappingItem
                mafcm.DefaultValue = jo("DefaultValue")
                mafcm.Format = jo("Format")
                mafcm.Index = i
                mafcm.Name = jo("Name")
                mafcm.NullValue = jo("NullValue")
                mafcm.StartColumn = jo("StartColumn")
                mafcm.EndColumn = jo("EndColumn")
                mafcm.FileType = jo("FileType")
                mafcm.Position = jo("Position")
                mafcm.Target = jo("Target")
                mafcm.Type = jo("Type")
                ColumnMappings.Add(mafcm)
                i += 1
            Next
        End Sub
        Public Sub New(ByVal Source As String)
            ' take values from source and fill ColumnMappings
            '2Cases: 
            '- old one for the xml ToJson Converison
            '- new one using Json

            If Source.StartsWith("{") Then
                Dim maFileColumnMappings As New MessageAction_File_ColumnMappings
                Dim lstmaFileCM As List(Of MessageAction_File_ColumnMappingItem) = JavaScriptConvert.DeserializeObject(Source)

                For Each mafcm As MessageAction_File_ColumnMappingItem In lstmaFileCM
                    ColumnMappings.Add(mafcm)
                Next
            Else
                Dim splitter As New JsonConversion.SmartSplitter
                splitter.Split(Source)
                If splitter.Length > 0 Then
                    Dim str As String
                    Dim i As Integer
                    For i = 0 To splitter.Length - 1
                        str = splitter(i)
                        If Not str Is Nothing Then
                            Dim strSplitter As New JsonConversion.SmartSplitter
                            strSplitter.Split(str)
                            If strSplitter.Length >= 6 Then
                                Dim mafcm As New MessageAction_File_ColumnMappingItem
                                mafcm.Index = i
                                mafcm.Position = strSplitter(0)
                                mafcm.Name = strSplitter(1)
                                mafcm.Target = strSplitter(2)
                                mafcm.Type = strSplitter(3)
                                mafcm.DefaultValue = strSplitter(4)
                                mafcm.NullValue = strSplitter(5)
                                If strSplitter.Length > 6 Then
                                    mafcm.Format = strSplitter(6)
                                End If
                                ColumnMappings.Add(mafcm)
                            End If
                        End If
                    Next
                End If
            End If
        End Sub
        Public Overrides Function ToString() As String
            Dim splitter As New JsonConversion.SmartSplitter

            Dim mafcm As MessageAction_File_ColumnMappingItem
            For Each mafcm In ColumnMappings
                Dim strsplitter As New JsonConversion.SmartSplitter
                strsplitter.Add(mafcm.Position)
                strsplitter.Add(mafcm.Name)
                strsplitter.Add(mafcm.Target)
                strsplitter.Add(mafcm.Type)
                strsplitter.Add(mafcm.DefaultValue)
                strsplitter.Add(mafcm.NullValue)
                If mafcm Is Nothing Then
                    strsplitter.Add(mafcm.Format)
                Else
                    strsplitter.Add("")
                End If
                splitter.Add(strsplitter.Blend)
            Next
            Return splitter.Blend
        End Function

    End Class
End Namespace
