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
Imports System
Imports System.Collections
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Diagnostics
Imports r2i.OWS.Framework.Utilities
Imports r2i.OWS.Framework
Imports r2i.OWS.Framework.Plugins.Formatters
Namespace r2i.OWS.Formatters
    Public Class SHA1Hash : Inherits FormatterBase

        Public Overrides Function Handle_Render(ByRef Caller As EngineBase, ByVal Index As Integer, ByRef Value As String, ByRef Formatter As String, ByRef Source As String, ByRef DS As System.Data.DataSet, ByRef DR As System.Data.DataRow, ByRef RuntimeMessages As System.Collections.Generic.SortedList(Of String, String), ByVal NullReturn As Boolean, ByVal NullOverride As Boolean, ByVal ProtectSession As Boolean, ByVal SessionDelimiter As String, ByVal useSessionQuotes As Boolean, ByVal useAggregations As Boolean, ByRef FilterText As String, ByRef FilterField As String, ByRef Debugger As Framework.Debugger) As Boolean
            Source = RenderString_Format_SHA1Hash(Value)
            Return True
        End Function

        Public Overrides ReadOnly Property RenderTag() As String
            Get
                Return "sha1hash"
            End Get
        End Property

        Dim _sha1 As System.Security.Cryptography.SHA1
        Private Function RenderString_Format_SHA1Hash(ByVal Value As String) As String
            If _sha1 Is Nothing Then
                _sha1 = System.Security.Cryptography.SHA1.Create()
            End If
            'oFirewall.Clean(Key)
            Dim hash() As Byte
            hash = _sha1.ComputeHash(System.Text.Encoding.UTF8.GetBytes(Value))
            Dim sb As System.Text.StringBuilder = New System.Text.StringBuilder
            For Each b As Byte In hash
                sb.Append(b.ToString("x2", System.Globalization.CultureInfo.InvariantCulture))
            Next
            Return sb.ToString
        End Function
    End Class
End Namespace