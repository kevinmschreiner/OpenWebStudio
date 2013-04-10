'<LICENSE>
'   Open Web Studio - http://www.OpenWebStudio.com
'   Copyright (c) 2007-2008
'   by R2Integrated Inc. http://www.R2integrated.com
'      
'   Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
'   documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
'   the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
'   to permit persons to whom the Software is furnished to do so, subject to the following conditions:
'    
'   The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
'   the Software.
'   
'   This Software and associated documentation files are subject to the terms and conditions of the Open Web Studio 
'   End User License Agreement and version 2 of the GNU General Public License.
'    
'   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
'   TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
'   THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
'   CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
'   DEALINGS IN THE SOFTWARE.
'</LICENSE>
'
' Bi4ce ListX 1.5 -  http://dnn.bi4ce.com
' Copyright (c) 2005
' by Kevin M Schreiner ( sales@bi4ce.com ) of Business Intelligence Force, Inc. ( http://www.bi4ce.com )
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
' DEALINGS IN THE SOFTWARE.
Imports System.Collections.Generic
Imports r2i.OWS.Newtonsoft.Json
Imports r2i.OWS.Framework.Utilities
Imports r2i.OWS.Framework.DataAccess
Namespace r2i.OWS
    Public Class InstallController
        Public Class Package
            Public PackageID As Integer
            Public Name As String
            Public Description As String
            Public Company As String
            Public Author As String
            Public MatchTabName As Boolean
            Public Version As String
            Public UniqueID As String
            Public PortalID As Integer
            Public UserID As Integer
            Public Status As Integer
            Public StatusDate As DateTime
            Public StatusMessage As String
        End Class
        Public Class PackageItem
            Public PackageItemID As Integer
            Public PackageID As Integer
            Public ParentPackageItemID As Integer
            Public ItemType As String
            Public ItemName As String
            Public ItemDescription As String
            Public ItemPath As String
            Public SourceID As Integer
            Public DestinationID As Integer
            Public SequenceNumber As Integer
            Public Status As Integer
            Public StatusDate As DateTime
            Public StatusMessage As String
            Public Content As Byte()
        End Class
        Public Function GetPackage(ByVal PortalID As Integer, ByVal PackageID As Integer, ByVal Name As String, ByVal UniqueID As String) As Package
            Dim ds As DataSet = Nothing
            If PackageID > 0 Then
                ds = DataProvider.Instance().GetPackage(PortalID, PackageID)
            ElseIf Not Name Is Nothing Then
                ds = DataProvider.Instance().GetPackage_ByName(PortalID, Name)
            ElseIf Not UniqueID Is Nothing Then
                ds = DataProvider.Instance().GetPackage_ByUniqueID(PortalID, UniqueID)
            End If
            Dim Value As Package = Nothing
            If Not ds Is Nothing AndAlso ds.Tables.Count > 0 AndAlso ds.Tables(0).Rows.Count > 0 Then
                Value = New Package
                If Not IsDBNull(ds.Tables(0).Rows(0).Item("PackageID")) Then
                    Value.PackageID = ds.Tables(0).Rows(0).Item("PackageID")
                End If
                If Not IsDBNull(ds.Tables(0).Rows(0).Item("Author")) Then
                    Value.Author = ds.Tables(0).Rows(0).Item("Author")
                End If
                If Not IsDBNull(ds.Tables(0).Rows(0).Item("Company")) Then
                    Value.Company = ds.Tables(0).Rows(0).Item("Company")
                End If
                If Not IsDBNull(ds.Tables(0).Rows(0).Item("Name")) Then
                    Value.Name = ds.Tables(0).Rows(0).Item("Name")
                End If
                If Not IsDBNull(ds.Tables(0).Rows(0).Item("PortalID")) Then
                    Value.PortalID = ds.Tables(0).Rows(0).Item("PortalID")
                End If
                If Not IsDBNull(ds.Tables(0).Rows(0).Item("Status")) Then
                    Value.Status = ds.Tables(0).Rows(0).Item("Status")
                End If
                If Not IsDBNull(ds.Tables(0).Rows(0).Item("StatusDate")) Then
                    Value.StatusDate = ds.Tables(0).Rows(0).Item("StatusDate")
                End If
                If Not IsDBNull(ds.Tables(0).Rows(0).Item("StatusMessage")) Then
                    Value.StatusMessage = ds.Tables(0).Rows(0).Item("StatusMessage")
                End If
                If Not IsDBNull(ds.Tables(0).Rows(0).Item("UniqueID")) Then
                    Value.UniqueID = ds.Tables(0).Rows(0).Item("UniqueID")
                End If
                If Not IsDBNull(ds.Tables(0).Rows(0).Item("UserID")) Then
                    Value.UserID = ds.Tables(0).Rows(0).Item("UserID")
                End If
                If Not IsDBNull(ds.Tables(0).Rows(0).Item("Version")) Then
                    Value.Version = ds.Tables(0).Rows(0).Item("Version")
                End If
                If Not IsDBNull(ds.Tables(0).Rows(0).Item("Description")) Then
                    Value.Description = ds.Tables(0).Rows(0).Item("Description")
                    If Not Value.Description Is Nothing AndAlso Value.Description.ToUpper.EndsWith("MATCHTAB") Then
                        Value.MatchTabName = True
                    End If
                End If
            End If
            Return Value
        End Function
        Public Function GetPackageItem(ByVal PackageID As Integer, ByVal PackageItemID As Integer, ByVal SourceID As Integer, ByVal ItemType As String) As PackageItem
            Dim ds As DataSet = Nothing
            If PackageItemID > 0 Then
                ds = DataProvider.Instance().GetPackageItem(PackageID, PackageItemID)
            ElseIf SourceID > 0 Then
                ds = DataProvider.Instance().GetPackageItem_BySourceID(PackageID, ItemType, SourceID)
            End If
            Dim Value As PackageItem = Nothing
            If Not ds Is Nothing AndAlso ds.Tables.Count > 0 AndAlso ds.Tables(0).Rows.Count > 0 Then
                Value = New PackageItem
                If Not IsDBNull(ds.Tables(0).Rows(0).Item("Content")) Then
                    Value.Content = ds.Tables(0).Rows(0).Item("Content")
                End If
                If Not IsDBNull(ds.Tables(0).Rows(0).Item("DestinationID")) Then
                    Value.DestinationID = ds.Tables(0).Rows(0).Item("DestinationID")
                End If
                If Not IsDBNull(ds.Tables(0).Rows(0).Item("ItemDescription")) Then
                    Value.ItemDescription = ds.Tables(0).Rows(0).Item("ItemDescription")
                End If
                If Not IsDBNull(ds.Tables(0).Rows(0).Item("ItemName")) Then
                    Value.ItemName = ds.Tables(0).Rows(0).Item("ItemName")
                End If
                If Not IsDBNull(ds.Tables(0).Rows(0).Item("ItemType")) Then
                    Value.ItemType = ds.Tables(0).Rows(0).Item("ItemType")
                End If
                If Not IsDBNull(ds.Tables(0).Rows(0).Item("ItemPath")) Then
                    Value.ItemPath = ds.Tables(0).Rows(0).Item("ItemPath")
                End If
                If Not IsDBNull(ds.Tables(0).Rows(0).Item("PackageID")) Then
                    Value.PackageID = ds.Tables(0).Rows(0).Item("PackageID")
                End If
                If Not IsDBNull(ds.Tables(0).Rows(0).Item("PackageItemID")) Then
                    Value.PackageItemID = ds.Tables(0).Rows(0).Item("PackageItemID")
                End If
                If Not IsDBNull(ds.Tables(0).Rows(0).Item("ParentPackageItemID")) Then
                    Value.ParentPackageItemID = ds.Tables(0).Rows(0).Item("ParentPackageItemID")
                End If
                If Not IsDBNull(ds.Tables(0).Rows(0).Item("SourceID")) Then
                    Value.SourceID = ds.Tables(0).Rows(0).Item("SourceID")
                End If
                If Not IsDBNull(ds.Tables(0).Rows(0).Item("Status")) Then
                    Value.Status = ds.Tables(0).Rows(0).Item("Status")
                End If
                If Not IsDBNull(ds.Tables(0).Rows(0).Item("StatusDate")) Then
                    Value.StatusDate = ds.Tables(0).Rows(0).Item("StatusDate")
                End If
                If Not IsDBNull(ds.Tables(0).Rows(0).Item("StatusMessage")) Then
                    Value.StatusMessage = ds.Tables(0).Rows(0).Item("StatusMessage")
                End If
                If Not IsDBNull(ds.Tables(0).Rows(0).Item("SequenceNumber")) Then
                    Value.SequenceNumber = ds.Tables(0).Rows(0).Item("SequenceNumber")
                End If
            End If
            Return Value
        End Function
        Private Sub FillPackageItem(ByRef Value As PackageItem, ByRef Row As DataRow)
            If Not Row Is Nothing Then
                If Not IsDBNull(Row.Item("Content")) Then
                    Value.Content = Row.Item("Content")
                End If
                If Not IsDBNull(Row.Item("DestinationID")) Then
                    Value.DestinationID = Row.Item("DestinationID")
                End If
                If Not IsDBNull(Row.Item("ItemDescription")) Then
                    Value.ItemDescription = Row.Item("ItemDescription")
                End If
                If Not IsDBNull(Row.Item("ItemName")) Then
                    Value.ItemName = Row.Item("ItemName")
                End If
                If Not IsDBNull(Row.Item("ItemType")) Then
                    Value.ItemType = Row.Item("ItemType")
                End If
                If Not IsDBNull(Row.Item("ItemPath")) Then
                    Value.ItemPath = Row.Item("ItemPath")
                End If
                If Not IsDBNull(Row.Item("PackageID")) Then
                    Value.PackageID = Row.Item("PackageID")
                End If
                If Not IsDBNull(Row.Item("PackageItemID")) Then
                    Value.PackageItemID = Row.Item("PackageItemID")
                End If
                If Not IsDBNull(Row.Item("ParentPackageItemID")) Then
                    Value.ParentPackageItemID = Row.Item("ParentPackageItemID")
                End If
                If Not IsDBNull(Row.Item("SourceID")) Then
                    Value.SourceID = Row.Item("SourceID")
                End If
                If Not IsDBNull(Row.Item("Status")) Then
                    Value.Status = Row.Item("Status")
                End If
                If Not IsDBNull(Row.Item("StatusDate")) Then
                    Value.StatusDate = Row.Item("StatusDate")
                End If
                If Not IsDBNull(Row.Item("StatusMessage")) Then
                    Value.StatusMessage = Row.Item("StatusMessage")
                End If
                If Not IsDBNull(Row.Item("SequenceNumber")) Then
                    Value.SequenceNumber = Row.Item("SequenceNumber")
                End If
            End If
        End Sub
        Public Function SetPackage(ByVal Value As Package) As Integer
            Dim i As Integer = DataProvider.Instance().SetPackage(Value.PackageID, Value.Name, Value.Description, Value.Company, Value.Author, Value.Version, Value.UniqueID, Value.PortalID, Value.UserID, Value.Status, Value.StatusDate, Value.StatusMessage)
            Value.PackageID = i
            Return i
        End Function
        Public Function SetPackageItem(ByVal Value As PackageItem) As Integer
            Dim i As Integer = DataProvider.Instance().SetPackageItem(Value.SequenceNumber, Value.PackageItemID, Value.PackageID, Value.ParentPackageItemID, Value.ItemType, Value.ItemName, Value.ItemDescription, Value.ItemPath, Value.SourceID, Value.DestinationID, Value.Status, Value.StatusDate, Value.StatusMessage, Value.Content)
            Value.PackageItemID = i
            Return i
        End Function
        Public Function GetPackageItems(ByVal PackageID As Integer) As PackageItem()
            Dim ds As DataSet
            Dim arr As New List(Of PackageItem)
            If PackageID > 0 Then
                ds = DataProvider.Instance().GetPackageItems(PackageID)
                If Not ds Is Nothing AndAlso ds.Tables.Count > 0 Then
                    Dim dr As DataRow
                    For Each dr In ds.Tables(0).Rows
                        Dim obj As New PackageItem
                        FillPackageItem(obj, dr)
                        arr.Add(obj)
                    Next
                End If
            End If
            If arr.Count = 0 Then
                Return Nothing
            Else
                Return arr.ToArray
            End If
        End Function
        Public Function SetPackageItems(ByVal Value As PackageItem()) As Boolean
            Dim pi As PackageItem
            For Each pi In Value
                pi.PackageItemID = SetPackageItem(pi)
            Next
            Return True
        End Function
        Public Sub DeletePackage(ByVal Value As Package)
            DataProvider.Instance().DeletePackageItem(Value.PackageID)
        End Sub
        Public Sub DeletePackageItem(ByVal Value As PackageItem)
            DataProvider.Instance().DeletePackageItem(Value.PackageItemID)
        End Sub
    End Class
    Public Class Controller
#Region "Drafts Mode"
        <Obsolete("This was never called, so don't call it now.")> _
        Public Function GetSettingDraft(ByVal UserId As String, ByVal ConfigurationID As Guid, ByRef DraftDate As DateTime, ByRef CreatedDate As DateTime, ByRef RuntimeDate As DateTime) As String
            'Dim str As String
            'Try
            '	Dim ds As DataSet = DataProvider.Instance().GetSettingDraft(UserId, ConfigurationID)
            '	If Not ds Is Nothing AndAlso ds.Tables.Count > 0 AndAlso ds.Tables(0).Rows.Count > 0 Then
            '		If Not IsDBNull(ds.Tables(0).Rows(0).Item(0)) Then
            '			str = ds.Tables(0).Rows(0).Item(0)
            '		Else
            '			str = ""
            '		End If
            '		If Not IsDBNull(ds.Tables(0).Rows(0).Item(1)) Then
            '			DraftDate = ds.Tables(0).Rows(0).Item(1)
            '		End If
            '		If Not IsDBNull(ds.Tables(0).Rows(0).Item(2)) Then
            '			CreatedDate = ds.Tables(0).Rows(0).Item(2)
            '		End If
            '	End If
            'Catch ex As Exception
            '	'ROMAIN: 09/18/07
            '	'TODO: CHANGE EXCEPTIONS
            '	'DotNetNuke.Services.Exceptions.LogException(ex)
            'End Try
            'RuntimeDate = GetSettingDate(ConfigurationID)
            'Return str
            Return Nothing
        End Function
        Public Sub DeleteSettingDraft(ByVal UserId As String, ByVal ConfigurationId As Guid)
            DataProvider.Instance().DeleteSettingDraft(UserId, ConfigurationId)
        End Sub
        Public Function GetSettingDraftOwnership(ByVal ConfigurationID As String) As Integer
            Return GetSettingDraftOwnership(New Guid(ConfigurationID))
        End Function
        Public Function GetSettingDraftOwnership(ByVal ConfigurationID As Guid) As String
            Dim ds As DataSet = DataProvider.Instance().GetSettingDraftOwners(ConfigurationID)
            Dim UserId As Integer = -1
            If Not ds Is Nothing AndAlso ds.Tables.Count > 0 AndAlso ds.Tables(0).Rows.Count > 0 Then
                Dim dr As DataRow = ds.Tables(0).Rows(0)

                If Not IsDBNull(dr(0)) Then
                    UserId = CType(dr(0), Integer)
                End If
            End If
            Return UserId
        End Function
        Public Sub SetSettingDraftOwnership(ByVal ConfigurationID As String, ByVal UserId As String, ByVal isLocked As Boolean)
            SetSettingDraftOwnership(New Guid(ConfigurationID), UserId, isLocked)
        End Sub
        Public Sub SetSettingDraftOwnership(ByVal ConfigurationID As Guid, ByVal UserId As String, ByVal isLocked As Boolean)
            Dim owner As Integer = GetSettingDraftOwnership(ConfigurationID)
            If owner = UserId Or owner = -1 Then
                Dim SettingValue As String = DataProvider.Instance().GetSettingDraft(UserId, ConfigurationID)
                If Not SettingValue Is Nothing AndAlso SettingValue.Length > 0 Then
                    DataProvider.Instance().UpdateSettingDraft(UserId, ConfigurationID, SettingValue, isLocked)
                End If
            End If
        End Sub
        Public Function UpdateSettingDraft(ByVal UserId As String, ByVal ConfigurationID As Guid, ByVal SettingValue As String) As String
            Dim sCheckDraft As String = DataProvider.Instance().GetSettingDraft(UserId, ConfigurationID)
            Dim owner As Integer = GetSettingDraftOwnership(ConfigurationID)
            Dim canEdit As Boolean = False
            If UserId = owner OrElse owner = -1 Then
                canEdit = True
            End If
            If canEdit Then
                If owner > 0 Then
                    canEdit = True
                Else
                    canEdit = False
                End If
                If Not sCheckDraft Is Nothing AndAlso sCheckDraft.Length > 0 Then
                    DataProvider.Instance().UpdateSettingDraft(UserId, ConfigurationID, SettingValue, canEdit)
                Else
                    DataProvider.Instance().AddSettingDraft(UserId, ConfigurationID, SettingValue)
                End If
            End If
            Return Nothing
        End Function
#End Region
        Public Shared Function AddLog(ByVal ConfigurationID As String, ByVal UserID As String, ByVal SettingName As String, ByVal SettingValue As String, ByVal SessionID As String) As Integer
            Return AddLog(New Guid(ConfigurationID), UserID, SettingName, SettingValue, SessionID)
        End Function
        Public Shared Function AddLog(ByVal ConfigurationID As Guid, ByVal UserID As String, ByVal SettingName As String, ByVal SettingValue As String, ByVal SessionID As String) As Integer
            Return DataProvider.Instance.AddLog(ConfigurationID, SettingName, SettingValue, UserID, SessionID)
        End Function
        Public Function GetDataset(ByVal Connection As String, ByVal Query As String, Optional ByVal Timeout As Integer = -1) As DataSet
            Return DataProvider.Instance().GetDataset(Connection, Query, Timeout)
        End Function
        Public Function ParseScript(ByVal Query As String, ByVal Splitter As String) As List(Of String)
            Return DataProvider.Instance().ParseScript(Query, Splitter)
        End Function
        Public Function ExecuteScript(ByVal Connection As String, ByVal Query As String, Optional ByVal Timeout As Integer = -1) As String
            Try
                DataProvider.Instance().ExecuteScript(Connection, Query, Timeout)
                Return String.Empty
            Catch ex As Exception
                Return ex.ToString
            End Try
        End Function
        Public Function GetSetting(ByVal ConfigurationID As Guid) As String
            Dim str As String
            Try
                str = DataProvider.Instance().GetSetting(ConfigurationID)
                'Dim ds As DataSet = DataProvider.Instance().GetSetting(TabId, ModuleId)
                'If Not ds Is Nothing AndAlso ds.Tables.Count > 0 AndAlso ds.Tables(0).Rows.Count > 0 Then
                '    If Not IsDBNull(ds.Tables(0).Rows(0).Item(0)) Then
                '        str = ds.Tables(0).Rows(0).Item(0)
                '    Else
                '        str = ""
                '    End If
                'End If
            Catch ex As Exception
                str = Nothing
                'ROMAIN: 09/18/07
                'TODO: CHANGE EXCEPTIONS
                'DotNetNuke.Services.Exceptions.LogException(ex)
            End Try
            Return str
        End Function
        Public Function GetSettingDate(ByVal ConfigurationID As String) As DateTime
            Return GetSettingDate(New Guid(ConfigurationID))
        End Function
        Public Function GetSettingDate(ByVal ConfigurationID As Guid) As DateTime
            Dim dt As DateTime
            Try
                Dim ds As DataSet = DataProvider.Instance().GetSettingDate(ConfigurationID)
                If Not ds Is Nothing AndAlso ds.Tables.Count > 0 AndAlso ds.Tables(0).Rows.Count > 0 Then
                    If Not IsDBNull(ds.Tables(0).Rows(0).Item(0)) Then
                        dt = ds.Tables(0).Rows(0).Item(0)
                    Else
                        dt = Nothing
                    End If
                End If
            Catch ex As Exception
                'ROMAIN: 09/18/07
                'TODO: CHANGE EXCEPTIONS
                'DotNetNuke.Services.Exceptions.LogException(ex)
            End Try
            Return dt
        End Function
        Public Function CheckExists(ByVal ConfigurationID As String) As Boolean
            Return CheckExists(New Guid(ConfigurationID))
        End Function
        Public Function CheckExists(ByVal ConfigurationID As Guid) As Boolean
            Dim exists As Boolean = False
            Try
                Dim ds As DataSet = DataProvider.Instance().GetSettingDate(ConfigurationID)
                If Not ds Is Nothing AndAlso ds.Tables.Count > 0 AndAlso ds.Tables(0).Rows.Count > 0 Then
                    exists = True
                End If
            Catch ex As Exception
                'ROMAIN: 09/18/07
                'TODO: CHANGE EXCEPTIONS
                'DotNetNuke.Services.Exceptions.LogException(ex)
            End Try
            Return exists
        End Function
        Public Function GetSettings(ByVal ConfigurationID As String) As IDataReader
            Return GetSettings(New Guid(ConfigurationID))
        End Function
        Public Function GetSettings(ByVal ConfigurationID As Guid) As IDataReader
            Try
                Dim reader As IDataReader = DataProvider.Instance().GetSettings(ConfigurationID)
                Return reader
            Catch ex As Exception
                'ROMAIN: 09/18/07
                'TODO: CHANGE EXCEPTIONS
                'DotNetNuke.Services.Exceptions.LogException(ex)
            End Try
            Return Nothing
        End Function
        Public Sub UpdateSetting(ByVal ConfigurationID As Guid, ByVal jsonValue As String, ByVal Username As String)
            'Public Sub UpdateSetting(ByVal ConfigurationID As Guid, ByVal moduleId As String, ByVal pageId As String, ByVal jsonValue As String)
            Dim str As Guid = DataProvider.Instance().CheckExistingConfiguration(ConfigurationID)
            Dim o As Object = JavaScriptConvert.DeserializeObject(jsonValue)
            o = Json.JsonToDataset.getJsonNode(o, "Name")
            Dim configurationName As String

            If Not o Is Nothing AndAlso o.GetType Is GetType(String) Then
                configurationName = o
            Else
                configurationName = "undefined configuration name"
            End If

            If Not str = Guid.Empty Then
                DataProvider.Instance().UpdateSetting(ConfigurationID, configurationName, jsonValue, Username)
            Else
                'Dim configurationName As String = pageId & ":" & moduleId    
                DataProvider.Instance().AddSetting(ConfigurationID, configurationName, jsonValue, Username)
                'DataProvider.Instance().AddSetting(ConfigurationID, jsonValue)
            End If
        End Sub
        Public Function UpgradeConfiguration(ByVal pageId As String, ByVal ModuleID As String) As Guid
            Return DataProvider.Instance().UpgradeConfiguration(pageId, ModuleID)
        End Function
        Public Function GetConfigurationDictionaryList() As Dictionary(Of String, String)
            Dim ds As DataSet = DataProvider.Instance().GetConfigurationList
            Dim dic As New Dictionary(Of String, String)
            Dim i As Integer
            For i = 0 To ds.Tables(0).Rows.Count - 1 Step 1
                dic.Add(ds.Tables(0).Rows(i).Item(0).ToString, ds.Tables(0).Rows(i).Item(1).ToString)
            Next
            Return dic
        End Function

        Public Function GetConfigurationList() As DataSet
            Return DataProvider.Instance().GetConfigurationList
        End Function
    End Class
End Namespace