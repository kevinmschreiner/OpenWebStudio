Namespace DataAccess.Factories
    Public Class ProviderConfigurationFactory
        Private Shared _instance As ProviderConfigurationFactory = New ProviderConfigurationFactory()

        Public Shared ReadOnly Property Instance() As ProviderConfigurationFactory
            Get
                Return _instance
            End Get
        End Property

        Public Shared Function GetProviderConfiguration(ByVal providerType As String) As DotNetNuke.Framework.Providers.ProviderConfiguration
            Return DotNetNuke.Framework.Providers.ProviderConfiguration.GetProviderConfiguration(providerType)
        End Function
    End Class
End Namespace

