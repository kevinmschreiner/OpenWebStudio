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
Imports System.Reflection
Imports r2i.OWS.Framework
Imports r2i.OWS.Framework.Plugins.Formatters
Imports DotNetNuke.Entities.Users

Namespace r2i.OWS.Wrapper.DNN.Extensions.Formatters
    Public Class EncodeDNN : Inherits FormatterBase

        Public Overrides Function Handle_Render(ByRef Caller As EngineBase, ByVal Index As Integer, ByRef Value As String, ByRef Formatter As String, ByRef Source As String, ByRef DS As System.Data.DataSet, ByRef DR As System.Data.DataRow, ByRef RuntimeMessages As System.Collections.Generic.SortedList(Of String, String), ByVal NullReturn As Boolean, ByVal NullOverride As Boolean, ByVal ProtectSession As Boolean, ByVal SessionDelimiter As String, ByVal useSessionQuotes As Boolean, ByVal useAggregations As Boolean, ByRef FilterText As String, ByRef FilterField As String, ByRef Debugger As Framework.Debugger) As Boolean
            Dim tparser As New DotNetNuke.Services.Localization.Locale
            Source = GetSystemMessage(Caller, Value)
            Return True
        End Function

        Private Function GetSystemMessage(ByRef Caller As EngineBase, ByVal strMessageValue As String) As String
            If strMessageValue <> "" Then
                Dim objPortal As DotNetNuke.Entities.Portals.PortalSettings = DotNetNuke.Common.GetPortalSettings
                Dim objUser As DotNetNuke.Entities.Users.UserInfo = DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo
                Dim strKey As String

                ' host values
                If InStr(1, strMessageValue, "Host:", CompareMethod.Text) <> 0 Then
                    Dim objHostSettings As Hashtable = DotNetNuke.Entities.Host.HostSettings.GetSecureHostSettings
                    For Each strKey In objHostSettings.Keys
                        If InStr(1, strMessageValue, "[Host:" & strKey & "]", CompareMethod.Text) <> 0 Then
                            strMessageValue = Replace(strMessageValue, "[Host:" & strKey & "]", objHostSettings(strKey).ToString, , , CompareMethod.Text)
                        End If
                    Next
                End If

                ' get portal values
                If InStr(1, strMessageValue, "Portal:", CompareMethod.Text) <> 0 Then
                    If Not objPortal Is Nothing Then
                        strMessageValue = PersonalizeSystemMessage(strMessageValue, "Portal:", objPortal, GetType(DotNetNuke.Entities.Portals.PortalSettings))
                    End If
                    strMessageValue = Replace(strMessageValue, "[Portal:URL]", objPortal.PortalAlias.HTTPAlias, , , CompareMethod.Text)
                End If

                ' get user values
                If (Not objUser Is Nothing) And (Not objPortal Is Nothing) Then
                    If InStr(1, strMessageValue, "User:", CompareMethod.Text) <> 0 Then
                        strMessageValue = PersonalizeSystemMessage(strMessageValue, "User:", objUser, GetType(UserInfo))
                        strMessageValue = Replace(strMessageValue, "[User:VerificationCode]", objPortal.PortalId.ToString & "-" & objUser.UserID.ToString, , , CompareMethod.Text)
                    End If
                    If InStr(1, strMessageValue, "Membership:", CompareMethod.Text) <> 0 Then
                        If objUser.IsSuperUser Then
                            objUser.Membership.Password = "xxxxxx"
                        End If
                        strMessageValue = PersonalizeSystemMessage(strMessageValue, "Membership:", objUser.Membership, GetType(UserMembership))
                    End If
                    If InStr(1, strMessageValue, "Profile:", CompareMethod.Text) <> 0 Then
                        strMessageValue = PersonalizeSystemMessage(strMessageValue, "Profile:", objUser.Profile, GetType(UserProfile))
                    End If
                End If

                ' constants
                Dim ci As System.Globalization.CultureInfo
                ci = New System.Globalization.CultureInfo(System.Threading.Thread.CurrentThread.CurrentCulture.ToString.ToLower)

                strMessageValue = Replace(strMessageValue, "[Date:Current]", Now().ToString("D", ci), , , CompareMethod.Text)
            End If

            Return strMessageValue

        End Function

        Private Shared Function PersonalizeSystemMessage(ByVal MessageValue As String, ByVal Prefix As String, ByVal objObject As Object, ByVal objType As Type) As String

            Dim intProperty As Integer
            Dim strPropertyName As String = ""
            Dim strPropertyValue As String = ""

            Dim objProperties As ArrayList = DotNetNuke.Common.Utilities.CBO.GetPropertyInfo(objType)

            For intProperty = 0 To objProperties.Count - 1
                strPropertyName = CType(objProperties(intProperty), PropertyInfo).Name
                If InStr(1, MessageValue, "[" & Prefix & strPropertyName & "]", CompareMethod.Text) <> 0 Then
                    Dim propInfo As PropertyInfo = CType(objProperties(intProperty), PropertyInfo)
                    Dim propValue As Object = propInfo.GetValue(objObject, Nothing)

                    If propValue Is Nothing = False Then
                        strPropertyValue = propValue.ToString()
                    End If

                    ' special case for encrypted passwords
                    If (Prefix & strPropertyName = "Membership:Password") And Convert.ToString(DotNetNuke.Common.Globals.HostSettings("EncryptionKey")) <> "" Then
                        Dim objSecurity As New DotNetNuke.Security.PortalSecurity
                        strPropertyValue = objSecurity.Decrypt(DotNetNuke.Common.Globals.HostSettings("EncryptionKey").ToString, strPropertyValue)
                    End If

                    MessageValue = Replace(MessageValue, "[" & Prefix & strPropertyName & "]", strPropertyValue, , , CompareMethod.Text)
                End If
            Next intProperty

            Return MessageValue

        End Function


        Public Overrides ReadOnly Property RenderTag() As String
            Get
                Return "encodednn"
            End Get
        End Property
    End Class
End Namespace