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

Namespace r2i.OWS.Actions.Utilities
    <Serializable()> _
      Public Class MessageAction_Filter_Options

        <Serializable()> _
        Public Class MessageAction_Filter_Option
            Public [Option] As String
            Public Field As String
        End Class

        Public Options As New List(Of MessageAction_Filter_Option)
        Public Sub New()
            Options = New List(Of MessageAction_Filter_Option)
        End Sub
        <CLSCompliant(False)> Public Sub New(ByVal Source As Newtonsoft.Json.JavaScriptArray)
            Dim maFilterOptions As New MessageAction_Filter_Options
            For Each jo As JavaScriptObject In Source
                Dim mafcm As New MessageAction_Filter_Option
                mafcm.Option = jo("Option")
                mafcm.Field = jo("Field")
                Options.Add(mafcm)
            Next
        End Sub
        Public Sub New(ByVal Source As String)
            ' take values from source and fill ColumnMappings
            '2Cases: 
            '- old one for the xml ToJson Converison
            '- new one using Json

            If Source.StartsWith("{") Then
                Dim maFilterOptions As New MessageAction_Filter_Options
                Dim lstmaFilterCM As List(Of MessageAction_Filter_Option) = JavaScriptConvert.DeserializeObject(Source)

                For Each mafcm As MessageAction_Filter_Option In lstmaFilterCM
                    Options.Add(mafcm)
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
                            If strSplitter.Length >= 1 Then
                                Dim mafcm As New MessageAction_Filter_Option
                                mafcm.Option = strSplitter(0)
                                mafcm.Field = strSplitter(1)
                                Options.Add(mafcm)
                            End If
                        End If
                    Next
                End If
            End If
        End Sub
        Public Overrides Function ToString() As String
            Dim splitter As New JsonConversion.SmartSplitter

            Dim mafcm As MessageAction_Filter_Option
            For Each mafcm In Options
                Dim strsplitter As New JsonConversion.SmartSplitter
                strsplitter.Add(mafcm.Option)
                strsplitter.Add(mafcm.Field)
                If Not mafcm Is Nothing Then
                    strsplitter.Add(mafcm.Option)
                Else
                    strsplitter.Add("")
                End If
                splitter.Add(strsplitter.Blend)
            Next
            Return splitter.Blend
        End Function

    End Class
End Namespace
