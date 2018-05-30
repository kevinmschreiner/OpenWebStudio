
Namespace DataAccess.Factories
    Public Class ModuleDefinitionFactory
        Private Shared _instance As ModuleDefinitionFactory = New ModuleDefinitionFactory()

        Public Shared ReadOnly Property Instance() As ModuleDefinitionFactory
            Get
                Return _instance
            End Get
        End Property

        Public Function GetModuleDefinitionByName(ByVal desktopModuleId As Integer, ByVal friendlyName As String) As DotNetNuke.Entities.Modules.Definitions.ModuleDefinitionInfo
            Dim dmC As New DotNetNuke.Entities.Modules.Definitions.ModuleDefinitionController
            Dim desktopModuleIdConverted As Integer
            If Integer.TryParse(CStr(desktopModuleId), desktopModuleIdConverted) Then
                Return dmC.GetModuleDefinitionByName(desktopModuleId, friendlyName)
            Else
                'TODO: Exception
                Return Nothing
            End If


        End Function
    End Class
End Namespace