Namespace DataAccess.Factories
    Public Class DeliveryFactory
        Private Shared _instance As DeliveryFactory = New DeliveryFactory()

        Public Shared ReadOnly Property Instance() As DeliveryFactory
            Get
                Return _instance
            End Get
        End Property

        Public Sub Script_ReplaceOwner(ByRef Source As String)
            Dim ProviderType As String = "data"
            Dim objConfiguration As DotNetNuke.Framework.Providers.ProviderConfiguration = DotNetNuke.Framework.Providers.ProviderConfiguration.GetProviderConfiguration(ProviderType)
            Dim objProvider As DotNetNuke.Framework.Providers.Provider = CType(objConfiguration.Providers(objConfiguration.DefaultProvider), DotNetNuke.Framework.Providers.Provider)
            Dim strOwner As String

            strOwner = objProvider.Attributes("databaseOwner")
            If strOwner <> "" And strOwner.EndsWith(".") = False Then
                strOwner += "."
            End If
            Replacer(Source, "{databaseOwner}", strOwner)
        End Sub
        Public Sub Script_ReplaceQualifier(ByRef Source As String)
            Dim ProviderType As String = "data"
            Dim objConfiguration As DotNetNuke.Framework.Providers.ProviderConfiguration = DotNetNuke.Framework.Providers.ProviderConfiguration.GetProviderConfiguration(ProviderType)
            Dim objProvider As DotNetNuke.Framework.Providers.Provider = CType(objConfiguration.Providers(objConfiguration.DefaultProvider), DotNetNuke.Framework.Providers.Provider)
            Dim strQualifier As String

            strQualifier = objProvider.Attributes("objectQualifier")
            If strQualifier <> "" And strQualifier.EndsWith("_") = False Then
                strQualifier += "_"
            End If

            Replacer(Source, "{objectQualifier}", strQualifier)
        End Sub
        Private Sub Replacer(ByRef source As String, ByVal replacing As String, ByVal replacement As String)
            Dim istart As Integer
            Dim starter As String = replacing
            istart = source.ToUpper.IndexOf(starter.ToUpper)

            While istart >= 0
                Dim xlength As Integer = (starter).Length
                Dim fvalue As String
                If Not replacement Is Nothing Then
                    fvalue = replacement
                Else
                    fvalue = ""
                End If
                source = source.Substring(0, istart) & fvalue & source.Substring(istart + xlength)
                If istart + 1 < source.Length Then
                    istart = source.ToUpper.IndexOf(starter.ToUpper, istart + 1)
                Else
                    istart = -1
                End If
            End While
        End Sub
    End Class
End Namespace
