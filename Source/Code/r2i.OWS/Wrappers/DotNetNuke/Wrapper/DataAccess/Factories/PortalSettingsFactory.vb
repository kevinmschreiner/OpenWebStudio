Namespace DataAccess.Factories
    Public Class PortalSettingsFactory
        Private Shared _instance As PortalSettingsFactory = New PortalSettingsFactory()

        Public Shared ReadOnly Property Instance() As PortalSettingsFactory
            Get
                Return _instance
            End Get
        End Property

        Public Function GetPortalSettings() As IPortalSettings
            Return PortalSettings.Instance.GetPortalSettings()
        End Function

        Public Function LocalizationGetString(ByVal key As String, ByVal resourceFile As String) As String
            Return DotNetNuke.Services.Localization.Localization.GetString(key, resourceFile)
        End Function
    End Class
End Namespace

