Namespace DataAccess.Factories
    Public Class CDefaultFactory
        Private Shared _instance As CDefaultFactory = New CDefaultFactory()

        Public Shared ReadOnly Property Instance() As CDefaultFactory
            Get
                Return _instance
            End Get
        End Property


        Public Function GetCDefaultProperties(ByVal cd As DotNetNuke.Framework.CDefault) As System.Reflection.PropertyInfo()
            Return cd.GetType().GetProperties()
        End Function

        Public Function GetCDefaultProperty(ByVal cd As DotNetNuke.Framework.CDefault, ByVal name As String) As System.Reflection.PropertyInfo
            Return cd.GetType().GetProperty(name)
        End Function
    End Class
End Namespace
