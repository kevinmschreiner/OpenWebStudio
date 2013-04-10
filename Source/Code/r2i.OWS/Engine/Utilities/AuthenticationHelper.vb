Imports System.Net

Public Class AuthenticationHelper
    Public Shared Function GetCrendentials(ByVal strAuthenticationType As String, ByVal targeturl As String, ByVal strAuthenticationUsername As String, _
            ByVal strAuthenticationPassword As String, ByVal strAuthenticationDomain As String) As ICredentials
        'AUTHENTICATE
        Select Case strAuthenticationType.Substring(0, 1).ToLower
            Case "n"
                'none
            Case "b"
                'basic
                Dim secCache As New System.Net.CredentialCache()

                If strAuthenticationDomain <> "" Then
                    secCache.Add(New Uri(targeturl), "Basic", New Net.NetworkCredential(strAuthenticationUsername, strAuthenticationPassword, strAuthenticationDomain))
                Else
                    secCache.Add(New Uri(targeturl), "Basic", New Net.NetworkCredential(strAuthenticationUsername, strAuthenticationPassword))
                End If
                Return secCache
            Case "d"
                If strAuthenticationType.ToLower = "defaultnetworkcredentials" Then
                    ' special case of d: defaultnetworkcredentials
                    Return System.Net.CredentialCache.DefaultNetworkCredentials
                Else
                    ' digest
                    Dim secCache As New System.Net.CredentialCache()

                    If strAuthenticationDomain <> "" Then
                        secCache.Add(New Uri(targeturl), "Digest", New Net.NetworkCredential(strAuthenticationUsername, strAuthenticationPassword, strAuthenticationDomain))
                    Else
                        secCache.Add(New Uri(targeturl), "Digest", New Net.NetworkCredential(strAuthenticationUsername, strAuthenticationPassword))
                    End If
                    Return secCache
                End If
            Case "w"
                'windows
                Return System.Net.CredentialCache.DefaultCredentials
        End Select
        Return Nothing
    End Function
End Class
