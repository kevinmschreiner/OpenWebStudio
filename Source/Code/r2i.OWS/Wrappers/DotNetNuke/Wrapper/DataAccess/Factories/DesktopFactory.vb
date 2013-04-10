Namespace DataAccess.Factories
    Public Class DesktopFactory
        Private Shared _instance As DesktopFactory = New DesktopFactory()

        Public Shared ReadOnly Property Instance() As DesktopFactory
            Get
                Return _instance
            End Get
        End Property

        Public Function GetDesktopModuleByName(ByVal FriendlyName As String) As DotNetNuke.Entities.Modules.DesktopModuleInfo
            Dim mdC As New DotNetNuke.Entities.Modules.DesktopModuleController
            Return mdC.GetDesktopModuleByName(FriendlyName)
        End Function
    End Class
End Namespace

