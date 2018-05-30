
Namespace DataAccess.Factories
    Public Class ModuleDefinitionFactory
        Private Shared _instance As ModuleDefinitionFactory = New ModuleDefinitionFactory()

        Public Shared ReadOnly Property Instance() As ModuleDefinitionFactory
            Get
                Return _instance
            End Get
        End Property

        Public Function GetModuleDefinitionByName(ByVal desktopModuleId As Integer, ByVal friendlyName As String) As DotNetNuke.Entities.Modules.Definitions.ModuleDefinitionInfo
            Dim desktopModuleIdConverted As Integer
            If Integer.TryParse(CStr(desktopModuleId), desktopModuleIdConverted) Then
                Return DotNetNuke.Entities.Modules.DesktopModuleController.GetDesktopModuleByFriendlyName(friendlyName).ModuleDefinitions.Values.GetEnumerator().Current
            Else
                'TODO: Exception
                Return Nothing
            End If


        End Function
    End Class
End Namespace