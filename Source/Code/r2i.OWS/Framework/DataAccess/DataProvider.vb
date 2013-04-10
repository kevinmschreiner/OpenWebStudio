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

Imports System
Imports System.Collections.Generic
Imports r2i.OWS.Framework.Entities

Namespace r2i.OWS.Framework.DataAccess
    Public MustInherit Class DataProvider

        ' singleton reference to the instantiated object 
        Private Shared objProvider As DataProvider = Nothing

        ' constructor
        Shared Sub New()
            'TODO: Important move this elsewhere

            'Dim apc As Utilities.AbstractProvider = Utilities.ProviderDefinition
            'AbstractFactory.AbstractFactoryAssemblyName = apc.Assembly
            'AbstractFactory.AbstractFactoryClassName = apc.ClassName
            'Engine.Load()

            CreateProvider()
        End Sub

        ' dynamically create provider
        Private Shared Sub CreateProvider()
            Dim obj As Object = AbstractFactory.Instance.DataController.GetProvider("data", "r2i.OWS", "r2i.OWS")
            objProvider = CType(obj, DataProvider)
        End Sub

        ' return the provider
        Public Shared Shadows Function Instance() As DataProvider
            Return objProvider
        End Function

        Public MustOverride Function GetProviderVersion() As String

        Public MustOverride ReadOnly Property ObjectQualifier() As String
        Public MustOverride ReadOnly Property DatabaseOwner() As String

        Public MustOverride Function ExecuteScript(ByVal Connection As String, ByVal Query As String, Optional ByVal ConnectionTimeout As Integer = -1) As Boolean
        Public MustOverride Function ParseScript(ByVal Query As String, ByVal SplitValue As String) As List(Of String)

        Public MustOverride Function GetConfigurationId(ByVal ModuleId As String, ByVal pageId As String) As Guid
        Public MustOverride Function GetConfigurationList() As DataSet
        Public MustOverride Sub AssignConfiguration(ByVal moduleId As String, ByVal configurationId As Guid, ByVal Key As String)

        Public MustOverride Function GetSetting(ByVal ConfigurationID As Guid) As String
        Public MustOverride Function GetSettingDate(ByVal ConfigurationID As Guid) As DataSet
        Public MustOverride Sub AddSetting(ByVal ConfigurationID As Guid, ByVal configurationName As String, ByVal SettingValue As String, ByVal CreatedBy As String)
        Public MustOverride Sub UpdateSetting(ByVal ConfigurationID As Guid, ByVal configurationName As String, ByVal SettingValue As String, ByVal UpdatedBy As String)
        Public MustOverride Function GetSettings(ByVal ConfigurationID As Guid) As IDataReader
        Public MustOverride Function GetConfigNameByConfigurationId(ByVal ConfigurationId As Guid) As String()
        Public MustOverride Function CheckExistingConfiguration(ByVal ConfigurationId As Guid) As Guid
        Public MustOverride Function UpgradeConfiguration(ByVal pageId As String, ByVal ModuleID As String) As Guid

        Public MustOverride Sub AddSettingDraft(ByVal UserId As String, ByVal ConfigurationID As Guid, ByVal SettingValue As String)
        Public MustOverride Function GetSettingDraft(ByVal UserId As String, ByVal ConfigurationID As Guid) As String
        Public MustOverride Function GetSettingDraftOwners(ByVal ConfigurationID As Guid) As DataSet
        Public MustOverride Sub UpdateSettingDraft(ByVal UserId As String, ByVal ConfigurationID As Guid, ByVal SettingValue As String, ByVal isLocked As Boolean)
        Public MustOverride Sub DeleteSettingDraft(ByVal UserId As String, ByVal ConfigurationID As Guid)

        Public MustOverride Function AddLog(ByVal ConfigurationID As Guid, ByVal SettingName As String, ByVal SettingValue As String, ByVal UserID As String, ByVal SessionID As String) As Integer

        Public MustOverride Function GetDataset(ByVal Connection As String, ByVal Query As String, Optional ByVal ConnectionTimeout As Integer = -1) As DataSet

        Public MustOverride Function GetPackage(ByVal PortalID As Integer, ByVal PackageID As Integer) As DataSet
        Public MustOverride Function GetPackage_ByName(ByVal PortalID As Integer, ByVal Name As String) As DataSet
        Public MustOverride Function GetPackage_ByUniqueID(ByVal PortalID As Integer, ByVal UniqueID As String) As DataSet
        Public MustOverride Function SetPackage(ByVal PackageID As Integer, ByVal Name As String, ByVal Description As String, ByVal Company As String, ByVal Author As String, ByVal Version As String, ByVal UniqueID As String, ByVal PortalID As Integer, ByVal UserID As Integer, ByVal Status As Integer, ByVal StatusDate As DateTime, ByVal StatusMessage As String) As Integer
        Public MustOverride Sub DeletePackage(ByVal PackageID As Integer)
        Public MustOverride Sub DeletePackageItem(ByVal PackageItemID As Integer)
        Public MustOverride Function GetPackageItems(ByVal PackageID As Integer) As DataSet
        Public MustOverride Function GetPackageItem(ByVal PackageID As Integer, ByVal PackageItemID As Integer) As DataSet
        Public MustOverride Function GetPackageItem_BySourceID(ByVal PackageID As Integer, ByVal SourceType As String, ByVal SourceID As Integer) As DataSet
        Public MustOverride Function SetPackageItem(ByVal SequenceNumber As Integer, ByVal PackageItemID As Integer, ByVal PackageID As Integer, ByVal ParentPackageItemID As Integer, ByVal ItemType As String, ByVal ItemName As String, ByVal ItemDescription As String, ByVal ItemPath As String, ByVal SourceID As Integer, ByVal DestinationID As Integer, ByVal Status As Integer, ByVal StatusDate As DateTime, ByVal StatusMessage As String, ByVal Content As Byte()) As Integer

        Public MustOverride Function GetToolbarSetting(ByVal ConfigurationID As Guid) As IDataReader
        Public MustOverride Sub AddToolbarSetting(ByVal ConfigurationID As Guid, ByVal SettingValue As String)
        Public MustOverride Sub UpdateToolbarSetting(ByVal ConfigurationID As Guid, ByVal SettingValue As String)
        Public MustOverride Function GetToolbarSettings(ByVal ConfigurationID As Guid) As IDataReader


    End Class
End Namespace
