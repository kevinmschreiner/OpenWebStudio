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
Imports r2i.OWS.Framework
Imports r2i.OWS.Framework.Plugins.Formatters

Namespace r2i.OWS.Formatters
    Public Class Encrypt : Inherits FormatterBase

        Public Overrides Function Handle_Render(ByRef Caller as EngineBase, ByVal Index As Integer, ByRef Value As String, ByRef Formatter As String, ByRef Source As String, ByRef DS As System.Data.DataSet, ByRef DR As System.Data.DataRow, ByRef RuntimeMessages As System.Collections.Generic.SortedList(Of String, String), ByVal NullReturn As Boolean, ByVal NullOverride As Boolean, ByVal ProtectSession As Boolean, ByVal SessionDelimiter As String, ByVal useSessionQuotes As Boolean, ByVal useAggregations As Boolean, ByRef FilterText As String, ByRef FilterField As String, ByRef Debugger As Framework.Debugger) As Boolean
            Dim type As String = ""
            Dim key As String = ""
            Dim vector As String = ""
            Dim size As Integer = 10
            If Formatter.ToLower.StartsWith("{encrypt_") Then
                If Formatter.ToLower.StartsWith("{encrypt_md5") Then
                    key = Formatter.Substring(size + 3, Formatter.Length - (size + 4))
                ElseIf Formatter.ToLower.StartsWith("{encrypt_sha1") Then
                    type = "sha1"
                    key = Formatter.Substring(size + 4, Formatter.Length - (size + 5))
                ElseIf Formatter.ToLower.StartsWith("{encrypt_formsauthentication") Then
                ElseIf Formatter.ToLower.StartsWith("{encrypt_rijndael") Then
                    type = "rijndael"
                    key = Formatter.Substring(size + type.Length, Formatter.Length - (size + type.Length + 1))
                    vector = "x/M2XUUrKxzkif0BWr7Yxw=="
                ElseIf Formatter.ToLower.StartsWith("{encrypt_hmacsha256") Then
                    type = "hmacsha256"
                    key = Formatter.Substring(size + 10, Formatter.Length - (size + 11))
                ElseIf Formatter.ToLower.StartsWith("{encrypt_hmacsha256b") Then
                    type = "hmacsha256b"
                    key = Formatter.Substring(size + 11, Formatter.Length - (size + 12))
                ElseIf Formatter.ToLower.StartsWith("{encrypt_rc2") Then
                    type = "rc2"
                    key = Formatter.Substring(size + type.Length, Formatter.Length - (size + type.Length + 1))
                    vector = "89y8JfFPUBk="
                ElseIf Formatter.ToLower.StartsWith("{encrypt_tripledes") Then
                    type = "tripledes"
                    key = Formatter.Substring(size + type.Length, Formatter.Length - (size + type.Length + 1))
                    vector = "32wsQ19jbpk="
                ElseIf Formatter.ToLower.StartsWith("{encrypt_des") Then
                    type = "des"
                    key = Formatter.Substring(size + type.Length, Formatter.Length - (size + type.Length + 1))
                    vector = "mfYTqWn80cc="
                ElseIf Formatter.ToLower.StartsWith("{encrypt_aes") Then
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
                Source = HandleEncrypt(type, Value, vector, key)
            Else
                Source = Value
            End If
            Return True
        End Function
        Private Function HandleEncrypt(ByVal encType As String, ByVal value As String, ByVal vector As String, ByVal key As String) As String
            'Dim fParameter As String = Formatter.Substring(9, Formatter.Length - 10)
            'If fParameter.Length > 0 Then
            '    'ROMAIN: 09/18/07
            '    'Source = RenderString_Encrypt(fParameter, Value)
            If encType = "sha1" Then
                Return EncryptSHA1(key, value)
            ElseIf encType = "hmacsha256" Then
                'Dim crypto As New OWS.Framework.Utilities.Security.Cryptography.HMACSHA256
                'crypto.Key = Convert.FromBase64String(key)
                'Return crypto.Encrypt(value)
                Return EncryptSHA256(key, value)
            ElseIf encType = "hmacsha256b" Then
                Dim crypto As New OWS.Framework.Utilities.Security.Cryptography.HMACSHA256
                crypto.Key = Convert.FromBase64String(key)
                Return crypto.Encrypt(value)
            ElseIf encType = "rijndael" Then
                Dim crypto As New OWS.Framework.Utilities.Security.Cryptography.Rijndael
                crypto.Key = Convert.FromBase64String(key)
                crypto.Vector = Convert.FromBase64String(vector)
                Return crypto.Encrypt(value)
            ElseIf encType = "des" Then
                Dim crypto As New OWS.Framework.Utilities.Security.Cryptography.DES
                crypto.Key = Convert.FromBase64String(key)
                crypto.Vector = Convert.FromBase64String(vector)
                Return crypto.Encrypt(value)
            ElseIf encType = "tripledes" Then
                Dim crypto As New OWS.Framework.Utilities.Security.Cryptography.TripleDES
                crypto.Key = Convert.FromBase64String(key)
                crypto.Vector = Convert.FromBase64String(vector)
                Return crypto.Encrypt(value)
            ElseIf encType = "rc2" Then
                Dim crypto As New OWS.Framework.Utilities.Security.Cryptography.RC2
                crypto.Key = Convert.FromBase64String(key)
                crypto.Vector = Convert.FromBase64String(vector)
                Return crypto.Encrypt(value)
            Else
                Return AbstractFactory.Instance.SecurityController.RenderString_Encrypt(key, value)
            End If
            'Else
            'Source = value
            'End If
        End Function
        Private Function EncryptSHA1(ByVal Key As String, ByVal Value As String) As String
            Dim hmac As System.Security.Cryptography.HMACSHA1 = New System.Security.Cryptography.HMACSHA1(System.Text.Encoding.UTF8.GetBytes(Key))
            Return Convert.ToBase64String(hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(Value)))
        End Function
        Private Function EncryptSHA256(ByVal Key As String, ByVal Value As String) As String
            Dim hmac As System.Security.Cryptography.HMACSHA256 = New System.Security.Cryptography.HMACSHA256(System.Text.Encoding.UTF8.GetBytes(Key))
            Return Convert.ToBase64String(hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(Value)))
        End Function

        Public Overrides ReadOnly Property RenderTag() As String
            Get
                Return "encrypt"
            End Get
        End Property
        Public Overrides ReadOnly Property RenderTags() As String()
            Get
                Static str As String() = New String() {"encrypt", "encrypt_formsauthentication", "encrypt_sha1", "encrypt_md5", "encrypt_rc2", "encrypt_rijndael", "encrypt_tripledes", "encrypt_des", "encrypt_hmacsha256", "encrypt_hmacsha256b"}
                Return str
            End Get
        End Property
    End Class
End Namespace