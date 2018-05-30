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
Imports r2i.OWS.Framework.Entities
Imports r2i.OWS.Framework.Plugins.Formatters
Imports r2i.OWS.Framework
Imports r2i.OWS.Framework.Utilities

Namespace r2i.OWS.Formatters
    Public Class Decrypt : Inherits FormatterBase

        Public Overrides Function Handle_Render(ByRef Caller As EngineBase, ByVal Index As Integer, ByRef Value As String, ByRef Formatter As String, ByRef Source As String, ByRef DS As System.Data.DataSet, ByRef DR As System.Data.DataRow, ByRef RuntimeMessages As System.Collections.Generic.SortedList(Of String, String), ByVal NullReturn As Boolean, ByVal NullOverride As Boolean, ByVal ProtectSession As Boolean, ByVal SessionDelimiter As String, ByVal useSessionQuotes As Boolean, ByVal useAggregations As Boolean, ByRef FilterText As String, ByRef FilterField As String, ByRef Debugger As Framework.Debugger) As Boolean
            Dim type As String = ""
            Dim key As String = ""
            Dim vector As String = ""
            Dim size As Integer = 10
            If Formatter.ToLower.StartsWith("{decrypt_") Then
                If Formatter.ToLower.StartsWith("{decrypt_md5") Then
                    key = Formatter.Substring(size + 3, Formatter.Length - (size + 4))
                ElseIf Formatter.ToLower.StartsWith("{decrypt_sha1") Then
                    type = "sha1"
                    key = Formatter.Substring(size + 4, Formatter.Length - (size + 5))
                ElseIf Formatter.ToLower.StartsWith("{decrypt_hmacsha256") Then
                    type = "hmacsha256"
                    key = Formatter.Substring(size + 10, Formatter.Length - (size + 11))
                ElseIf Formatter.ToLower.StartsWith("{decrypt_formsauthentication") Then
                ElseIf Formatter.ToLower.StartsWith("{decrypt_rijndael") Then
                    type = "rijndael"
                    key = Formatter.Substring(size + type.Length, Formatter.Length - (size + type.Length + 1))
                    vector = "x/M2XUUrKxzkif0BWr7Yxw=="
                ElseIf Formatter.ToLower.StartsWith("{decrypt_rc2") Then
                    type = "rc2"
                    key = Formatter.Substring(size + type.Length, Formatter.Length - (size + type.Length + 1))
                    vector = "89y8JfFPUBk="
                ElseIf Formatter.ToLower.StartsWith("{decrypt_tripledes") Then
                    type = "tripledes"
                    key = Formatter.Substring(size + type.Length, Formatter.Length - (size + type.Length + 1))
                    vector = "32wsQ19jbpk="
                ElseIf Formatter.ToLower.StartsWith("{decrypt_des") Then
                    type = "des"
                    key = Formatter.Substring(size + type.Length, Formatter.Length - (size + type.Length + 1))
                    vector = "mfYTqWn80cc="
                ElseIf Formatter.ToLower.StartsWith("{decrypt_aes") Then
                    type = "aes"
                    key = Formatter.Substring(size + type.Length, Formatter.Length - (size + type.Length + 1))
                    vector = "xD5pbqNzrMFk0mu42Hcmtg=="
                End If
            Else
                Dim fParameter As String = Formatter.Substring(9, Formatter.Length - 10)
                If fParameter.Length > 0 Then
                    key = fParameter
                    type = ""
                End If
            End If
            If Value.Length > 0 Then
                Source = HandleDecrypt(type, Value, vector, key)
            Else
                Source = Value
            End If
            Return True
        End Function
        Private Function HandleDecrypt(ByVal encType As String, ByVal value As String, ByVal vector As String, ByVal key As String) As String
            'Dim fParameter As String = Formatter.Substring(9, Formatter.Length - 10)
            'If fParameter.Length > 0 Then
            '    'ROMAIN: 09/18/07
            '    'Source = RenderString_Encrypt(fParameter, Value)
            If encType = "rijndael" Then
                Dim crypto As New OWS.Framework.Utilities.Security.Cryptography.Rijndael
                crypto.Key = Convert.FromBase64String(key)
                crypto.Vector = Convert.FromBase64String(vector)
                Return crypto.Decrypt(value)
            ElseIf encType = "des" Then
                Dim crypto As New OWS.Framework.Utilities.Security.Cryptography.DES
                crypto.Key = Convert.FromBase64String(key)
                crypto.Vector = Convert.FromBase64String(vector)
                Return crypto.Decrypt(value)
            ElseIf encType = "tripledes" Then
                Dim crypto As New OWS.Framework.Utilities.Security.Cryptography.TripleDES
                crypto.Key = Convert.FromBase64String(key)
                crypto.Vector = Convert.FromBase64String(vector)
                Return crypto.Decrypt(value)
            ElseIf encType = "rc2" Then
                Dim crypto As New OWS.Framework.Utilities.Security.Cryptography.RC2
                crypto.Key = Convert.FromBase64String(key)
                crypto.Vector = Convert.FromBase64String(vector)
                Return crypto.Decrypt(value)
            ElseIf encType = "hmacsha256" Then
                Dim crypto As New OWS.Framework.Utilities.Security.Cryptography.HMACSHA256
                crypto.Key = Convert.FromBase64String(key)
                crypto.Vector = Convert.FromBase64String(vector)
                Return crypto.Decrypt(value)
            Else
                Return AbstractFactory.Instance.SecurityController.RenderString_Decrypt(key, value)
            End If
            'Else
            'Source = value
            'End If
        End Function

        Public Overrides ReadOnly Property RenderTag() As String
            Get
                Return "decrypt"
            End Get
        End Property
        Public Overrides ReadOnly Property RenderTags() As String()
            Get
                Static str As String() = New String() {"decrypt", "decrypt_formsauthentication", "decrypt_md5", "decrypt_rc2", "decrypt_rijndael", "decrypt_tripledes", "decrypt_des", "decrypt_hmacsha256"}
                Return str
            End Get
        End Property
    End Class
End Namespace