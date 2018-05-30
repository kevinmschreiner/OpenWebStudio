Namespace r2i.OWS.Framework.UI
    Public Class Control
        Inherits System.Web.UI.Control
        Public Shared Function Create(ByVal source As System.Web.UI.Control) As r2i.OWS.Framework.UI.Control
            Dim e As New r2i.OWS.Framework.UI.Control
            e.Page = source.Page
            e.ID = source.ID
            Return e
        End Function



        Public Overridable Sub ClearCache()

        End Sub
        Public Overridable Sub ClearSiteCache()

        End Sub
        Public Overridable Sub ClearPageCache()

        End Sub
    End Class
End Namespace
