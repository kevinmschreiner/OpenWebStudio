Imports System
Imports System.Security.Cryptography
Namespace r2i.OWS.Framework.Utilities.Security.Cryptography
    Public Interface iCryptography
        Property Key As Byte()
        Property Vector As Byte()
        Sub GenerateKey()
        Sub GenerateVector()
        Function Encrypt(ByVal Value As String) As String
        Function Decrypt(ByVal Value As String) As String
    End Interface
    Public Class Rijndael
        Implements iCryptography
        Public Sub New()

        End Sub

        Private savedKey As Byte() = Nothing
        Private savedIV As Byte() = Nothing
        Private rdProvider As New RijndaelManaged()

        Public Property Key() As Byte() Implements iCryptography.Key
            Get
                Return savedKey
            End Get
            Set(ByVal value As Byte())
                savedKey = value
            End Set
        End Property

        Public Property Vector As Byte() Implements iCryptography.Vector
            Get
                Return savedIV
            End Get
            Set(ByVal value As Byte())
                savedIV = value
            End Set
        End Property

        Public Sub GenerateKey() Implements iCryptography.GenerateKey
            If (savedKey Is Nothing) Then
                rdProvider.KeySize = 256
                rdProvider.GenerateKey()
                savedKey = rdProvider.Key
            End If
        End Sub

        Private Sub GenerateVector() Implements iCryptography.GenerateVector
            If (savedIV Is Nothing) Then
                rdProvider.GenerateIV()
                savedIV = rdProvider.IV
            End If
        End Sub

        Public Function Encrypt(ByVal originalStr As String) As String Implements iCryptography.Encrypt
            Dim originalStrAsBytes() As Byte = System.Text.Encoding.UTF8.GetBytes(originalStr)
            Dim originalBytes() As Byte

            Using memStream As New System.IO.MemoryStream(originalStrAsBytes.Length)
                Using rdProvider
                    GenerateKey()
                    GenerateVector()
                    If (savedKey Is Nothing Or savedIV Is Nothing) Then
                        Throw New NullReferenceException("The Key and Initial Vector cannot be null!")
                    End If
                    Using rdTransform As ICryptoTransform = rdProvider.CreateEncryptor(CType(savedKey.Clone(), Byte()), CType(savedIV.Clone(), Byte()))
                        Using cryptostream As New CryptoStream(memStream, rdTransform, CryptoStreamMode.Write)
                            cryptostream.Write(originalStrAsBytes, 0, originalStrAsBytes.Length)
                            cryptostream.FlushFinalBlock()
                            originalBytes = memStream.ToArray()
                        End Using
                    End Using
                End Using
            End Using

            Return Convert.ToBase64String(originalBytes)
        End Function

        Public Function Decrypt(ByVal encryptedStr As String) As String Implements iCryptography.Decrypt
            Dim encryptedStrAsBytes() As Byte = Convert.FromBase64String(encryptedStr)
            Dim originalBytes(encryptedStrAsBytes.Length) As Byte

            Using rdProvider
                Using memStream As New System.IO.MemoryStream(encryptedStrAsBytes)
                    If (savedKey Is Nothing Or savedIV Is Nothing) Then
                        Throw New NullReferenceException("The Key and Initial Vector cannot be null!")
                    End If
                    Using rdTransform As ICryptoTransform = rdProvider.CreateDecryptor(CType(savedKey.Clone(), Byte()), CType(savedIV.Clone(), Byte()))
                        Using cryptostream As New CryptoStream(memStream, rdTransform, CryptoStreamMode.Read)
                            cryptostream.Read(originalBytes, 0, originalBytes.Length)
                        End Using
                    End Using
                End Using
            End Using

            Dim i As Integer = originalBytes.Length - 1
            While originalBytes(i) = 0 AndAlso i >= 0
                i -= 1
            End While
            If i >= 0 Then
                i += 1
            End If
            Return System.Text.Encoding.UTF8.GetString(originalBytes, 0, i)
        End Function
    End Class
    Public Class RC2
        Implements iCryptography
        Public Sub New()

        End Sub

        Private savedKey As Byte() = Nothing
        Private savedIV As Byte() = Nothing
        Private rdProvider As New RC2CryptoServiceProvider()

        Public Property Key() As Byte() Implements iCryptography.Key
            Get
                Return savedKey
            End Get
            Set(ByVal value As Byte())
                savedKey = value
            End Set
        End Property

        Public Property Vector As Byte() Implements iCryptography.Vector
            Get
                Return savedIV
            End Get
            Set(ByVal value As Byte())
                savedIV = value
            End Set
        End Property

        Public Sub GenerateKey() Implements iCryptography.GenerateKey
            If (savedKey Is Nothing) Then
                rdProvider.KeySize = 128
                rdProvider.GenerateKey()
                savedKey = rdProvider.Key
            End If
        End Sub

        Private Sub GenerateVector() Implements iCryptography.GenerateVector
            If (savedIV Is Nothing) Then
                rdProvider.GenerateIV()
                savedIV = rdProvider.IV
            End If
        End Sub

        Public Function Encrypt(ByVal originalStr As String) As String Implements iCryptography.Encrypt
            Dim originalStrAsBytes() As Byte = System.Text.Encoding.UTF8.GetBytes(originalStr)
            Dim originalBytes() As Byte

            Using memStream As New System.IO.MemoryStream(originalStrAsBytes.Length)
                Using rdProvider
                    GenerateKey()
                    GenerateVector()
                    If (savedKey Is Nothing Or savedIV Is Nothing) Then
                        Throw New NullReferenceException("The Key and Initial Vector cannot be null!")
                    End If
                    Using rdTransform As ICryptoTransform = rdProvider.CreateEncryptor(CType(savedKey.Clone(), Byte()), CType(savedIV.Clone(), Byte()))
                        Using cryptostream As New CryptoStream(memStream, rdTransform, CryptoStreamMode.Write)
                            cryptostream.Write(originalStrAsBytes, 0, originalStrAsBytes.Length)
                            cryptostream.FlushFinalBlock()
                            originalBytes = memStream.ToArray()
                        End Using
                    End Using
                End Using
            End Using

            Return Convert.ToBase64String(originalBytes)
        End Function

        Public Function Decrypt(ByVal encryptedStr As String) As String Implements iCryptography.Decrypt
            Dim encryptedStrAsBytes() As Byte = Convert.FromBase64String(encryptedStr)
            Dim originalBytes(encryptedStrAsBytes.Length) As Byte

            Using rdProvider
                Using memStream As New System.IO.MemoryStream(encryptedStrAsBytes)
                    If (savedKey Is Nothing Or savedIV Is Nothing) Then
                        Throw New NullReferenceException("The Key and Initial Vector cannot be null!")
                    End If
                    Using rdTransform As ICryptoTransform = rdProvider.CreateDecryptor(CType(savedKey.Clone(), Byte()), CType(savedIV.Clone(), Byte()))
                        Using cryptostream As New CryptoStream(memStream, rdTransform, CryptoStreamMode.Read)
                            cryptostream.Read(originalBytes, 0, originalBytes.Length)
                        End Using
                    End Using
                End Using
            End Using

            Dim i As Integer = originalBytes.Length - 1
            While originalBytes(i) = 0 AndAlso i >= 0
                i -= 1
            End While
            If i >= 0 Then
                i += 1
            End If
            Return System.Text.Encoding.UTF8.GetString(originalBytes, 0, i)
        End Function
    End Class
    Public Class HMACSHA256
        Implements iCryptography
        Public Sub New()

        End Sub

        Private savedKey As Byte() = Nothing
        Private savedIV As Byte() = Nothing

        Public Property Key() As Byte() Implements iCryptography.Key
            Get
                Return savedKey
            End Get
            Set(ByVal value As Byte())
                savedKey = value
            End Set
        End Property

        Public Property Vector As Byte() Implements iCryptography.Vector
            Get
                Return savedIV
            End Get
            Set(ByVal value As Byte())
                savedIV = value
            End Set
        End Property

        Public Sub GenerateKey() Implements iCryptography.GenerateKey
            If (savedKey Is Nothing) Then
                Dim rdProvider As New HMACSHA256()
                rdProvider.GenerateKey()
                savedKey = rdProvider.Key
            End If
        End Sub

        Private Sub GenerateVector() Implements iCryptography.GenerateVector
            If (savedIV Is Nothing) Then
                'rdProvider.GenerateIV()
                'savedIV = rdProvider.IV
            End If
        End Sub

        Public Function Encrypt(ByVal originalStr As String) As String Implements iCryptography.Encrypt
            Dim originalStrAsBytes() As Byte = System.Text.Encoding.UTF8.GetBytes(originalStr)
            Dim originalBytes() As Byte

            Using memStream As New System.IO.MemoryStream(originalStrAsBytes.Length)

                GenerateKey()
                GenerateVector()
                If (savedKey Is Nothing Or savedIV Is Nothing) Then
                    Throw New NullReferenceException("The Key and Initial Vector cannot be null!")
                End If
                Dim rdProvider As New HMACSHA256 '(savedKey)
                rdProvider.Key = savedKey

                'Using rdTransform As ICryptoTransform = rdProvider.CreateEncryptor(CType(savedKey.Clone(), Byte()), CType(savedIV.Clone(), Byte()))
                '    Using cryptostream As New CryptoStream(memStream, rdTransform, CryptoStreamMode.Write)
                '        cryptostream.Write(originalStrAsBytes, 0, originalStrAsBytes.Length)
                '        cryptostream.FlushFinalBlock()
                '        originalBytes = memStream.ToArray()
                '    End Using
                'End Using

                Using cryptostream As New CryptoStream(memStream, rdProvider, CryptoStreamMode.Write)
                    cryptostream.Write(originalStrAsBytes, 0, originalStrAsBytes.Length)
                    cryptostream.FlushFinalBlock()
                    originalBytes = memStream.ToArray()
                End Using

            End Using

            Return Convert.ToBase64String(originalBytes)
        End Function

        Public Function Decrypt(ByVal encryptedStr As String) As String Implements iCryptography.Decrypt
            Dim encryptedStrAsBytes() As Byte = Convert.FromBase64String(encryptedStr)
            Dim originalBytes(encryptedStrAsBytes.Length) As Byte

            Dim rdProvider As New HMACSHA256 '(savedKey)
            rdProvider.Key = savedKey

            Using memStream As New System.IO.MemoryStream(encryptedStrAsBytes)
                If (savedKey Is Nothing Or savedIV Is Nothing) Then
                    Throw New NullReferenceException("The Key and Initial Vector cannot be null!")
                End If

                Using cryptostream As New CryptoStream(memStream, rdProvider, CryptoStreamMode.Read)
                    cryptostream.Read(originalBytes, 0, originalBytes.Length)
                End Using
            End Using


            Dim i As Integer = originalBytes.Length - 1
            While originalBytes(i) = 0 AndAlso i >= 0
                i -= 1
            End While
            If i >= 0 Then
                i += 1
            End If
            Return System.Text.Encoding.UTF8.GetString(originalBytes, 0, i)
        End Function
    End Class
    Public Class TripleDES
        Implements iCryptography
        Public Sub New()

        End Sub

        Private savedKey As Byte() = Nothing
        Private savedIV As Byte() = Nothing
        Private rdProvider As New TripleDESCryptoServiceProvider()

        Public Property Key() As Byte() Implements iCryptography.Key
            Get
                Return savedKey
            End Get
            Set(ByVal value As Byte())
                savedKey = value
            End Set
        End Property

        Public Property Vector As Byte() Implements iCryptography.Vector
            Get
                Return savedIV
            End Get
            Set(ByVal value As Byte())
                savedIV = value
            End Set
        End Property

        Public Sub GenerateKey() Implements iCryptography.GenerateKey
            If (savedKey Is Nothing) Then
                rdProvider.KeySize = 192
                rdProvider.GenerateKey()
                savedKey = rdProvider.Key
            End If
        End Sub

        Private Sub GenerateVector() Implements iCryptography.GenerateVector
            If (savedIV Is Nothing) Then
                rdProvider.GenerateIV()
                savedIV = rdProvider.IV
            End If
        End Sub

        Public Function Encrypt(ByVal originalStr As String) As String Implements iCryptography.Encrypt
            Dim originalStrAsBytes() As Byte = System.Text.Encoding.UTF8.GetBytes(originalStr)
            Dim originalBytes() As Byte

            Using memStream As New System.IO.MemoryStream(originalStrAsBytes.Length)
                Using rdProvider
                    GenerateKey()
                    GenerateVector()
                    If (savedKey Is Nothing Or savedIV Is Nothing) Then
                        Throw New NullReferenceException("The Key and Initial Vector cannot be null!")
                    End If
                    Using rdTransform As ICryptoTransform = rdProvider.CreateEncryptor(CType(savedKey.Clone(), Byte()), CType(savedIV.Clone(), Byte()))
                        Using cryptostream As New CryptoStream(memStream, rdTransform, CryptoStreamMode.Write)
                            cryptostream.Write(originalStrAsBytes, 0, originalStrAsBytes.Length)
                            cryptostream.FlushFinalBlock()
                            originalBytes = memStream.ToArray()
                        End Using
                    End Using
                End Using
            End Using

            Return Convert.ToBase64String(originalBytes)
        End Function

        Public Function Decrypt(ByVal encryptedStr As String) As String Implements iCryptography.Decrypt
            Dim encryptedStrAsBytes() As Byte = Convert.FromBase64String(encryptedStr)
            Dim originalBytes(encryptedStrAsBytes.Length) As Byte

            Using rdProvider
                Using memStream As New System.IO.MemoryStream(encryptedStrAsBytes)
                    If (savedKey Is Nothing Or savedIV Is Nothing) Then
                        Throw New NullReferenceException("The Key and Initial Vector cannot be null!")
                    End If
                    Using rdTransform As ICryptoTransform = rdProvider.CreateDecryptor(CType(savedKey.Clone(), Byte()), CType(savedIV.Clone(), Byte()))
                        Using cryptostream As New CryptoStream(memStream, rdTransform, CryptoStreamMode.Read)
                            cryptostream.Read(originalBytes, 0, originalBytes.Length)
                        End Using
                    End Using
                End Using
            End Using

            Dim i As Integer = originalBytes.Length - 1
            While originalBytes(i) = 0 AndAlso i >= 0
                i -= 1
            End While
            If i >= 0 Then
                i += 1
            End If
            Return System.Text.Encoding.UTF8.GetString(originalBytes, 0, i)
        End Function
    End Class
    Public Class DES
        Implements iCryptography
        Public Sub New()

        End Sub

        Private savedKey As Byte() = Nothing
        Private savedIV As Byte() = Nothing
        Private rdProvider As New DESCryptoServiceProvider()

        Public Property Key() As Byte() Implements iCryptography.Key
            Get
                Return savedKey
            End Get
            Set(ByVal value As Byte())
                savedKey = value
            End Set
        End Property

        Public Property Vector As Byte() Implements iCryptography.Vector
            Get
                Return savedIV
            End Get
            Set(ByVal value As Byte())
                savedIV = value
            End Set
        End Property

        Public Sub GenerateKey() Implements iCryptography.GenerateKey
            If (savedKey Is Nothing) Then
                rdProvider.KeySize = 64
                rdProvider.GenerateKey()
                savedKey = rdProvider.Key
            End If
        End Sub

        Private Sub GenerateVector() Implements iCryptography.GenerateVector
            If (savedIV Is Nothing) Then
                rdProvider.GenerateIV()
                savedIV = rdProvider.IV
            End If
        End Sub

        Public Function Encrypt(ByVal originalStr As String) As String Implements iCryptography.Encrypt
            Dim originalStrAsBytes() As Byte = System.Text.Encoding.UTF8.GetBytes(originalStr)
            Dim originalBytes() As Byte

            Using memStream As New System.IO.MemoryStream(originalStrAsBytes.Length)
                Using rdProvider
                    GenerateKey()
                    GenerateVector()
                    If (savedKey Is Nothing Or savedIV Is Nothing) Then
                        Throw New NullReferenceException("The Key and Initial Vector cannot be null!")
                    End If
                    Using rdTransform As ICryptoTransform = rdProvider.CreateEncryptor(CType(savedKey.Clone(), Byte()), CType(savedIV.Clone(), Byte()))
                        Using cryptostream As New CryptoStream(memStream, rdTransform, CryptoStreamMode.Write)
                            cryptostream.Write(originalStrAsBytes, 0, originalStrAsBytes.Length)
                            cryptostream.FlushFinalBlock()
                            originalBytes = memStream.ToArray()
                        End Using
                    End Using
                End Using
            End Using

            Return Convert.ToBase64String(originalBytes)
        End Function

        Public Function Decrypt(ByVal encryptedStr As String) As String Implements iCryptography.Decrypt
            Dim encryptedStrAsBytes() As Byte = Convert.FromBase64String(encryptedStr)
            Dim originalBytes(encryptedStrAsBytes.Length) As Byte

            Using rdProvider
                Using memStream As New System.IO.MemoryStream(encryptedStrAsBytes)
                    If (savedKey Is Nothing Or savedIV Is Nothing) Then
                        Throw New NullReferenceException("The Key and Initial Vector cannot be null!")
                    End If
                    Using rdTransform As ICryptoTransform = rdProvider.CreateDecryptor(CType(savedKey.Clone(), Byte()), CType(savedIV.Clone(), Byte()))
                        Using cryptostream As New CryptoStream(memStream, rdTransform, CryptoStreamMode.Read)
                            cryptostream.Read(originalBytes, 0, originalBytes.Length)
                        End Using
                    End Using
                End Using
            End Using

            Dim i As Integer = originalBytes.Length - 1
            While originalBytes(i) = 0 AndAlso i >= 0
                i -= 1
            End While
            If i >= 0 Then
                i += 1
            End If
            Return System.Text.Encoding.UTF8.GetString(originalBytes, 0, i)
        End Function
    End Class
    'Public Class AES
    '    Implements iCryptography
    '    Public Sub New()

    '    End Sub

    '    Private savedKey As Byte() = Nothing
    '    Private savedIV As Byte() = Nothing
    '    Private rdProvider As New System.Security.Cryptography.AesManaged

    '    Public Property Key() As Byte() Implements iCryptography.Key
    '        Get
    '            Return savedKey
    '        End Get
    '        Set(ByVal value As Byte())
    '            savedKey = value
    '        End Set
    '    End Property

    '    Public Property Vector As Byte() Implements iCryptography.Vector
    '        Get
    '            Return savedIV
    '        End Get
    '        Set(ByVal value As Byte())
    '            savedIV = value
    '        End Set
    '    End Property

    '    Public Sub GenerateKey() Implements iCryptography.GenerateKey
    '        If (savedKey Is Nothing) Then
    '            rdProvider.KeySize = rdProvider.LegalKeySizes.Max.MaxSize
    '            rdProvider.GenerateKey()
    '            savedKey = rdProvider.Key
    '        End If
    '    End Sub

    '    Private Sub GenerateVector() Implements iCryptography.GenerateVector
    '        If (savedIV Is Nothing) Then
    '            rdProvider.GenerateIV()
    '            savedIV = rdProvider.IV
    '        End If
    '    End Sub

    '    Public Function Encrypt(ByVal originalStr As String) As String Implements iCryptography.Encrypt
    '        Dim originalStrAsBytes() As Byte = System.Text.Encoding.UTF8.GetBytes(originalStr)
    '        Dim originalBytes() As Byte

    '        Using memStream As New System.IO.MemoryStream(originalStrAsBytes.Length)
    '            Using rdProvider
    '                GenerateKey()
    '                GenerateVector()
    '                If (savedKey Is Nothing Or savedIV Is Nothing) Then
    '                    Throw New NullReferenceException("The Key and Initial Vector cannot be null!")
    '                End If
    '                Using rdTransform As ICryptoTransform = rdProvider.CreateEncryptor(CType(savedKey.Clone(), Byte()), CType(savedIV.Clone(), Byte()))
    '                    Using cryptostream As New CryptoStream(memStream, rdTransform, CryptoStreamMode.Write)
    '                        cryptostream.Write(originalStrAsBytes, 0, originalStrAsBytes.Length)
    '                        cryptostream.FlushFinalBlock()
    '                        originalBytes = memStream.ToArray()
    '                    End Using
    '                End Using
    '            End Using
    '        End Using

    '        Return Convert.ToBase64String(originalBytes)
    '    End Function

    '    Public Function Decrypt(ByVal encryptedStr As String) As String Implements iCryptography.Decrypt
    '        Dim encryptedStrAsBytes() As Byte = Convert.FromBase64String(encryptedStr)
    '        Dim originalBytes(encryptedStrAsBytes.Length) As Byte

    '        Using rdProvider
    '            Using memStream As New System.IO.MemoryStream(encryptedStrAsBytes)
    '                If (savedKey Is Nothing Or savedIV Is Nothing) Then
    '                    Throw New NullReferenceException("The Key and Initial Vector cannot be null!")
    '                End If
    '                Using rdTransform As ICryptoTransform = rdProvider.CreateDecryptor(CType(savedKey.Clone(), Byte()), CType(savedIV.Clone(), Byte()))
    '                    Using cryptostream As New CryptoStream(memStream, rdTransform, CryptoStreamMode.Read)
    '                        cryptostream.Read(originalBytes, 0, originalBytes.Length)
    '                    End Using
    '                End Using
    '            End Using
    '        End Using

    '        Dim i As Integer = originalBytes.Length - 1
    '        While originalBytes(i) = 0 AndAlso i >= 0
    '            i -= 1
    '        End While
    '        If i >= 0 Then
    '            i += 1
    '        End If
    '        Return System.Text.Encoding.UTF8.GetString(originalBytes, 0, i)
    '    End Function
    'End Class
End Namespace