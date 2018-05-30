Public Class OpenPage
    Inherits DotNetNuke.Framework.CDefault

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        EnsureChildControls()
    End Sub
End Class
