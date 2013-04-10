Public Class iSearchable
    Implements DotNetNuke.Entities.Modules.ISearchable

    Public Function GetSearchItems(ByVal ModInfo As DotNetNuke.Entities.Modules.ModuleInfo) As DotNetNuke.Services.Search.SearchItemInfoCollection Implements DotNetNuke.Entities.Modules.ISearchable.GetSearchItems
        Dim ci As New r2i.OWS.Wrapper.DNN.ControlInterface
        Return ci.GetSearchItems(ModInfo)
    End Function
End Class
