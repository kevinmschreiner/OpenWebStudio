
Namespace DataAccess.Factories
    Public Class ModuleControlInfoFactory
        Private Shared _instance As ModuleControlInfoFactory = New ModuleControlInfoFactory()

        Public Shared ReadOnly Property Instance() As ModuleControlInfoFactory
            Get
                Return _instance
            End Get
        End Property

        Public Function GetModuleControls(ByVal value As Integer) As List(Of ModuleControlInfo)
            Dim ctrl As New DotNetNuke.Entities.Modules.ModuleControlController
            Dim arr As List(Of ModuleControlInfo) = New List(Of ModuleControlInfo)
            Dim arrModuleControls As ArrayList
            Dim t As Type = ctrl.GetType
            'Dim miarray As System.Reflection.MemberInfo() = ctrl.GetType().GetMember("GetModuleConrols")
            arrModuleControls = CType(t.InvokeMember("GetModuleControls", Reflection.BindingFlags.InvokeMethod, Nothing, ctrl, New Object() {value}), ArrayList)
            For Each objModuleControl As ModuleControlInfo In arrModuleControls
                arr.Add(objModuleControl)
            Next
            Return arr
        End Function

    End Class
End Namespace

