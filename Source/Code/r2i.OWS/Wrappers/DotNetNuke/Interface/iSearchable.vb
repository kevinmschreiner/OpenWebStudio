Imports DotNetNuke.Services.Search.Entities

Public Class iSearchable
    'Implements DotNetNuke.Entities.Modules.ISearchable
    Inherits DotNetNuke.Entities.Modules.ModuleSearchBase

    'Public Function GetSearchItems(ByVal ModInfo As DotNetNuke.Entities.Modules.ModuleInfo) As DotNetNuke.Services.Search.SearchItemInfoCollection Implements DotNetNuke.Entities.Modules.ISearchable.GetSearchItems
    Public Overrides Function GetModifiedSearchDocuments(ModInfo As DotNetNuke.Entities.Modules.ModuleInfo, beginDateUtc As Date) As IList(Of SearchDocument)
        Dim ci As New r2i.OWS.Wrapper.DNN.ControlInterface
        Return ci.GetModifiedSearchDocuments(ModInfo, beginDateUtc)
    End Function
End Class
