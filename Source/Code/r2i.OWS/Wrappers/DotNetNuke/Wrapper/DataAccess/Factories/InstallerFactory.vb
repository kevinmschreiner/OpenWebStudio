Namespace DataAccess.Factories
    Public Class InstallerFactory
        Private Shared _instance As InstallerFactory = New InstallerFactory()

        Public Shared ReadOnly Property Instance() As InstallerFactory
            Get
                Return _instance
            End Get
        End Property

        Public Function LoadInstaller() As Installer
            Return New Installer
        End Function
    End Class
End Namespace

