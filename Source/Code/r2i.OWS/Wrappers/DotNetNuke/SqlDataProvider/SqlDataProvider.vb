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
Imports System.Data
Imports System.Data.SqlClient
Imports Microsoft.ApplicationBlocks.Data
Imports System.Collections.Generic
Imports r2i.OWS.Framework.Utilities


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The SqlDataProvider Class is an SQL Server implementation of the DataProvider Abstract
    ''' class that provides the DataLayer for the Toolbar Module.
    ''' </summary>
''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[kschreiner]	5/9/2005	Moved Toolbar to a separate Project
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class SqlDataProvider
    Inherits r2i.OWS.Framework.DataAccess.DataProvider

#Region "Private Members"

    Private Const ProviderType As String = "data"
    Private _providerConfiguration As DotNetNuke.Framework.Providers.ProviderConfiguration = DotNetNuke.Framework.Providers.ProviderConfiguration.GetProviderConfiguration(ProviderType)
    'Private _providerConfiguration As 
    Private _connectionString As String
    Private _providerPath As String
    Private _objectQualifier As String
    Private _databaseOwner As String
    Private Const ProviderAppHeader As String = "OpenWebStudio_"

#End Region

#Region "Constructors"

    Public Sub New()
        ' Read the configuration specific information for this provider
        Dim objProvider As DotNetNuke.Framework.Providers.Provider = CType(_providerConfiguration.Providers(_providerConfiguration.DefaultProvider), DotNetNuke.Framework.Providers.Provider)

        ' Read the attributes for this provider
        If objProvider.Attributes("connectionStringName") <> "" AndAlso Utility.ConfigurationSetting(objProvider.Attributes("connectionStringName")) <> "" Then
            _connectionString = Utility.ConfigurationSetting(objProvider.Attributes("connectionStringName"))
        Else
            _connectionString = objProvider.Attributes("connectionString")
        End If

        _providerPath = objProvider.Attributes("providerPath")

        _objectQualifier = objProvider.Attributes("objectQualifier")
        If _objectQualifier <> "" And _objectQualifier.EndsWith("_") = False Then
            _objectQualifier += "_"
        End If

        _databaseOwner = objProvider.Attributes("databaseOwner")
        If _databaseOwner <> "" And _databaseOwner.EndsWith(".") = False Then
            _databaseOwner += "."
        End If

    End Sub

#End Region

#Region "Properties"

    Public ReadOnly Property ConnectionString() As String
        Get
            Return _connectionString
        End Get
    End Property

    Public ReadOnly Property ProviderPath() As String
        Get
            Return _providerPath
        End Get
    End Property

    Public Overrides ReadOnly Property ObjectQualifier() As String
        Get
            Return _objectQualifier
        End Get
    End Property

    Public Overrides ReadOnly Property DatabaseOwner() As String
        Get
            Return _databaseOwner
        End Get
    End Property

#End Region

#Region "Public Methods"

    Private Function GetNull(ByVal Field As Object) As Object
        Return DotNetNuke.Common.Utilities.Null.GetNull(Field, DBNull.Value)
    End Function
    Public Overrides Function GetSetting(ByVal ConfigurationId As Guid) As String
        Dim ListSettingString As Object
        ListSettingString = SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & ProviderAppHeader & "GetSetting", ConfigurationId)
        If ListSettingString Is System.DBNull.Value Then
            ListSettingString = ""
        End If
        Return CStr(ListSettingString)
    End Function
    Public Overrides Function CheckExistingConfiguration(ByVal ConfigurationId As Guid) As Guid
        Return SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & ProviderAppHeader & "CheckExistingConfiguration", ConfigurationId)
    End Function
    Public Overrides Function GetSettingDate(ByVal ConfigurationId As Guid) As DataSet
        Dim ds As New DataSet
        ds = SqlHelper.ExecuteDataset(ConnectionString, DatabaseOwner & ObjectQualifier & ProviderAppHeader & "GetSettingDate", ConfigurationId)
        Return ds
    End Function
    Public Overrides Sub AddSetting(ByVal ConfigurationId As Guid, ByVal ConfigurationName As String, ByVal SettingValue As String, ByVal CreatedBy As String)
        SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & ProviderAppHeader & "AddSetting", ConfigurationId, ConfigurationName, SettingValue, CreatedBy)
    End Sub
    Public Overrides Sub UpdateSetting(ByVal ConfigurationId As Guid, ByVal ConfigurationName As String, ByVal SettingValue As String, ByVal UpdatedBy As String)
        SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & ProviderAppHeader & "UpdateSetting", ConfigurationId, ConfigurationName, SettingValue, UpdatedBy)
    End Sub
    Public Overrides Function GetSettings(ByVal ConfigurationId As Guid) As IDataReader
        Dim reader As SqlDataReader
        reader = SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & ProviderAppHeader & "GetSettings", ConfigurationId)
        Return reader
    End Function
    Public Overrides Function GetConfigNameByConfigurationId(ByVal ConfigurationId As Guid) As String()
        Dim configName As String = SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & ProviderAppHeader & "GetConfigNameByConfigurationId", ConfigurationId)
        If Not configName Is Nothing Then
            Dim tabmodId() As String = configName.Split(CChar(":"))
            If tabmodId.Length = 2 Then
                Return tabmodId
            Else
                'TODO: IMplements Exception unable to get tabId and moduleId
                Return Nothing
            End If
        Else
            'TODO: IMplements Exception unable to get configName
            Return Nothing
        End If
    End Function
    Public Overrides Function GetConfigurationId(ByVal moduleId As String, ByVal pageId As String) As Guid
        Dim configurationName As String = pageId & ":" & moduleId
        Try
            Return SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & ProviderAppHeader & "GetConfigurationIdByConfigurationName", configurationName)
        Catch ex As Exception
            'Todo: implements exception unable to get the configuration id
            Return Guid.Empty
        End Try
    End Function
    Public Overrides Function GetConfigurationList() As DataSet
        Dim ds As New DataSet
        ds = SqlHelper.ExecuteDataset(ConnectionString, DatabaseOwner & ObjectQualifier & ProviderAppHeader & "GetConfigurationList")
        Return ds
    End Function
#End Region

#Region "Toolbar Methods"
    Public Overloads Overrides Function GetToolbarSetting(ByVal configurationId As Guid) As System.Data.IDataReader
        Return CType(SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "GetToolbarSetting", configurationId), IDataReader)
    End Function
    Public Overloads Overrides Sub AddToolbarSetting(ByVal configurationId As Guid, ByVal SettingValue As String)
        SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "AddToolbarSetting", configurationId, SettingValue)
    End Sub
    Public Overloads Overrides Sub UpdateToolbarSetting(ByVal configurationId As Guid, ByVal SettingValue As String)
        SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & "UpdateToolbarSetting", configurationId, SettingValue)
    End Sub
    Public Overloads Overrides Function GetToolbarSettings(ByVal configurationId As Guid) As System.Data.IDataReader
        Dim reader As SqlDataReader
        reader = SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner & ObjectQualifier & "GetToolbarSettings", configurationId)
        Return reader
    End Function
#End Region

    Public Overrides Function GetProviderVersion() As String
        Dim versionInfo As String = "[PRODUCT] v[MAJOR].[MINOR].[BUILD].[REVISION] [DESCRIPTION]"
        Dim ass As System.Reflection.Assembly = System.Reflection.Assembly.GetExecutingAssembly()
        Dim arr As Object() = ass.GetCustomAttributes(False)
        Dim obj As Object
        For Each obj In arr
            If TypeOf obj Is System.Reflection.AssemblyProductAttribute Then
                versionInfo = versionInfo.Replace("[PRODUCT]", CType(obj, System.Reflection.AssemblyProductAttribute).Product)
            ElseIf TypeOf obj Is System.Reflection.AssemblyDescriptionAttribute Then
                versionInfo = versionInfo.Replace("[DESCRIPTION]", CType(obj, System.Reflection.AssemblyDescriptionAttribute).Description)
            End If
        Next

        versionInfo = versionInfo.Replace("[MAJOR]", ass.GetName.Version.Major)
        versionInfo = versionInfo.Replace("[MINOR]", ass.GetName.Version.Minor)
        versionInfo = versionInfo.Replace("[REVISION]", ass.GetName.Version.Revision)
        versionInfo = versionInfo.Replace("[BUILD]", ass.GetName.Version.Build)

        Return versionInfo
    End Function

#Region "Package Methods"
    Public Overrides Sub DeletePackage(ByVal PackageID As Integer)
        SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & ProviderAppHeader & "DeletePackage", PackageID)
    End Sub

    Public Overrides Function GetPackage(ByVal PortalID As Integer, ByVal PackageID As Integer) As System.Data.DataSet
        Dim ds As New DataSet
        Dim byPackageID As Integer = PackageID
        Dim byName As String = Nothing
        Dim byUniqueID As String = Nothing
        ds = SqlHelper.ExecuteDataset(ConnectionString, DatabaseOwner & ObjectQualifier & ProviderAppHeader & "GetPackage", byPackageID, byName, byUniqueID, PortalID)
        Return ds
    End Function

    Public Overrides Function GetPackage_ByName(ByVal PortalID As Integer, ByVal Name As String) As System.Data.DataSet
        Dim ds As New DataSet
        Dim byPackageID As Integer = -1
        Dim byName As String = Name
        Dim byUniqueID As String = Nothing
        ds = SqlHelper.ExecuteDataset(ConnectionString, DatabaseOwner & ObjectQualifier & ProviderAppHeader & "GetPackage", byPackageID, byName, byUniqueID, PortalID)
        Return ds
    End Function

    Public Overrides Function GetPackage_ByUniqueID(ByVal PortalID As Integer, ByVal UniqueID As String) As System.Data.DataSet
        Dim ds As New DataSet
        Dim byPackageID As Integer = -1
        Dim byName As String = Nothing
        Dim byUniqueID As String = UniqueID
        ds = SqlHelper.ExecuteDataset(ConnectionString, DatabaseOwner & ObjectQualifier & ProviderAppHeader & "GetPackage", byPackageID, byName, byUniqueID, PortalID)
        Return ds
    End Function

    Public Overrides Function GetPackageItem(ByVal PackageID As Integer, ByVal PackageItemID As Integer) As System.Data.DataSet
        Dim ds As New DataSet
        Dim SourceID As Integer = -1
        Dim ItemType As String = Nothing
        ds = SqlHelper.ExecuteDataset(ConnectionString, DatabaseOwner & ObjectQualifier & ProviderAppHeader & "GetPackageItem", PackageID, PackageItemID, SourceID, ItemType)
        Return ds
    End Function

    Public Overrides Function GetPackageItem_BySourceID(ByVal PackageID As Integer, ByVal ItemType As String, ByVal SourceID As Integer) As System.Data.DataSet
        Dim ds As New DataSet
        Dim PackageItemID As Integer = -1
        ds = SqlHelper.ExecuteDataset(ConnectionString, DatabaseOwner & ObjectQualifier & ProviderAppHeader & "GetPackageItem", PackageID, PackageItemID, SourceID, ItemType)
        Return ds
    End Function
    Public Overrides Function GetPackageItems(ByVal PackageID As Integer) As System.Data.DataSet
        Dim ds As New DataSet
        Dim byPackageID As Integer = PackageID
        ds = SqlHelper.ExecuteDataset(ConnectionString, DatabaseOwner & ObjectQualifier & ProviderAppHeader & "GetPackageItems", PackageID)
        Return ds
    End Function
    Public Overrides Function SetPackage(ByVal PackageID As Integer, ByVal Name As String, ByVal Description As String, ByVal Company As String, ByVal Author As String, ByVal Version As String, ByVal UniqueID As String, ByVal PortalID As Integer, ByVal UserID As Integer, ByVal Status As Integer, ByVal StatusDate As Date, ByVal StatusMessage As String) As Integer
        Dim iReturn As Integer = -1
        Dim ds As New DataSet
        Dim byPackageID As Integer = PackageID
        ds = SqlHelper.ExecuteDataset(ConnectionString, DatabaseOwner & ObjectQualifier & ProviderAppHeader & "SetPackage", PackageID, Name, Description, Company, Author, Version, UniqueID, PortalID, UserID, Status, StatusDate, StatusMessage)
        If Not ds Is Nothing AndAlso Not ds.Tables Is Nothing AndAlso ds.Tables.Count > 0 AndAlso ds.Tables(0).Rows.Count > 0 Then
            If IsNumeric(ds.Tables(0).Rows(0).Item(0)) Then
                iReturn = CInt(ds.Tables(0).Rows(0).Item(0))
            End If
            ds.Clear()
            ds.Dispose()
        End If
        ds = Nothing
        Return iReturn
    End Function
    Public Overrides Sub DeletePackageItem(ByVal PackageItemID As Integer)
        SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & ProviderAppHeader & "DeletePackageItem", PackageItemID)
    End Sub
    Public Overrides Function SetPackageItem(ByVal SequenceNumber As Integer, ByVal PackageItemID As Integer, ByVal PackageID As Integer, ByVal ParentPackageItemID As Integer, ByVal ItemType As String, ByVal ItemName As String, ByVal ItemDescription As String, ByVal ItemPath As String, ByVal SourceID As Integer, ByVal DestinationID As Integer, ByVal Status As Integer, ByVal StatusDate As Date, ByVal StatusMessage As String, ByVal Content() As Byte) As Integer
        Dim iReturn As Integer = -1
        Dim ds As New DataSet
        Dim byPackageID As Integer = PackageID
        ds = SqlHelper.ExecuteDataset(ConnectionString, DatabaseOwner & ObjectQualifier & ProviderAppHeader & "SetPackageItem", SequenceNumber, PackageItemID, PackageID, ParentPackageItemID, ItemType, ItemName, ItemDescription, ItemPath, SourceID, DestinationID, Status, StatusDate, StatusMessage, Content)
        If Not ds Is Nothing AndAlso Not ds.Tables Is Nothing AndAlso ds.Tables.Count > 0 AndAlso ds.Tables(0).Rows.Count > 0 Then
            If IsNumeric(ds.Tables(0).Rows(0).Item(0)) Then
                iReturn = CInt(ds.Tables(0).Rows(0).Item(0))
            End If
            ds.Clear()
            ds.Dispose()
        End If
        ds = Nothing
        Return iReturn
    End Function
#End Region
#Region "Draft Methods"
    Public Overrides Sub AddSettingDraft(ByVal UserId As String, ByVal ConfigurationId As Guid, ByVal SettingValue As String)
        SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & ProviderAppHeader & "AddSettingDraft", UserId, ConfigurationId, SettingValue)
    End Sub
    Public Overrides Function GetSettingDraft(ByVal UserId As String, ByVal ConfigurationId As Guid) As String
        Return SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & ProviderAppHeader & "GetSettingDraft", UserId, ConfigurationId)
    End Function
    Public Overrides Function GetSettingDraftOwners(ByVal ConfigurationId As Guid) As System.Data.DataSet
        Dim ds As New DataSet
        ds = SqlHelper.ExecuteDataset(ConnectionString, DatabaseOwner & ObjectQualifier & ProviderAppHeader & "GetSettingDraftOwners", ConfigurationId)
        Return ds
    End Function
    Public Overrides Sub UpdateSettingDraft(ByVal UserId As String, ByVal ConfigurationId As Guid, ByVal SettingValue As String, ByVal isLocked As Boolean)
        SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & ProviderAppHeader & "UpdateSettingDraft", UserId, ConfigurationId, SettingValue, isLocked)
    End Sub
    Public Overrides Sub DeleteSettingDraft(ByVal UserId As String, ByVal ConfigurationId As Guid)
        SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner & ObjectQualifier & ProviderAppHeader & "DeleteSettingDraft", UserId, ConfigurationId)
    End Sub
#End Region

    Public Overrides Function ParseScript(ByVal Query As String, ByVal SplitValue As String) As List(Of String)
        Dim strR As IO.StringReader = Nothing
        Dim result As New List(Of String)
        Try
            strR = New IO.StringReader(Query)

            Dim currentLine As String = strR.ReadLine
            Dim currentStatement As String = ""
            While Not currentLine Is Nothing
                If Not currentLine.TrimStart().ToUpper = SplitValue.ToUpper Then
                    currentStatement &= currentLine & vbCrLf
                Else
                    If currentStatement.Length > 0 Then
                        result.Add(currentStatement)
                        currentStatement = ""
                    End If
                End If
                currentLine = strR.ReadLine
            End While
            If currentStatement.Length > 0 Then
                result.Add(currentStatement)
            End If
            strR.Close()
        Catch ex As Exception
        Finally
            If Not strR Is Nothing Then
                strR.Close()
            End If
        End Try
        strR = Nothing
        Return result
    End Function
    Public Overrides Function ExecuteScript(ByVal Connection As String, ByVal Query As String, Optional ByVal ConnectionTimeout As Integer = -1) As Boolean
        '            Return SqlHelper.ExecuteDataset(Connection, CommandType.Text, Query)
        If ConnectionTimeout >= 0 Then
            Dim rex As Exception = Nothing
            Dim ds As New DataSet
            If Connection Is Nothing Then Connection = ConnectionString
            Dim conn As SqlConnection = New SqlConnection(Connection)
            Try
                conn.Open()
                Try
                    Dim cmd As SqlCommand = New SqlCommand(Query, conn)
                    cmd.CommandTimeout = ConnectionTimeout
                    'ROMAIN: Generic replacement - 08/20/2007
                    'Dim queryArray As ArrayList = ParseScript(Query, "GO")
                    Dim queryArray As List(Of String) = ParseScript(Query, "GO")
                    If Not queryArray Is Nothing AndAlso queryArray.Count > 0 Then
                        Dim i As Integer = 0
                        For i = 0 To queryArray.Count - 1
                            cmd.CommandText = queryArray(i)
                            cmd.ExecuteNonQuery()
                        Next
                    End If

                    cmd.Dispose()
                    cmd = Nothing
                Catch ex As Exception
                    rex = ex
                Finally
                    conn.Close()
                End Try
                conn.Dispose()
                conn = Nothing
                If Not rex Is Nothing Then
                    Throw rex
                End If
                Return True
            Catch ex As Exception
                Try
                    conn.Close()
                Catch ex2 As Exception

                End Try
                Throw ex
            End Try
        Else
            SqlHelper.ExecuteNonQuery(Connection, CommandType.Text, Query)
            Return True
        End If
        Return False
    End Function
    Public Overrides Function GetDataset(ByVal Connection As String, ByVal Query As String, Optional ByVal ConnectionTimeout As Integer = -1) As DataSet
        '            Return SqlHelper.ExecuteDataset(Connection, CommandType.Text, Query)
        If ConnectionTimeout >= 0 Then
            Dim rex As Exception = Nothing
            Dim ds As New DataSet
            Dim conn As SqlConnection = New SqlConnection(Connection)
            Try
                conn.Open()
                Try
                    Dim cmd As SqlCommand = New SqlCommand(Query, conn)

                    cmd.CommandTimeout = ConnectionTimeout


                    Dim da As New SqlClient.SqlDataAdapter(cmd)
                    da.Fill(ds)

                    da.Dispose()
                    cmd.Dispose()
                    da = Nothing
                    cmd = Nothing
                Catch ex As Exception
                    rex = ex
                Finally
                    conn.Close()
                End Try
                conn.Dispose()
                conn = Nothing
            Catch ex As Exception
                rex = ex
            End Try
            If Not rex Is Nothing Then
                Throw rex
            Else
                Return ds
            End If
        Else
            Return SqlHelper.ExecuteDataset(Connection, CommandType.Text, Query)

        End If

    End Function

    

    Public Overloads Overrides Function AddLog(ByVal ConfigurationId As Guid, ByVal SettingName As String, ByVal SettingValue As String, ByVal UserID As String, ByVal SessionID As String) As Integer
        Dim iReturn As Integer = -1
        Dim ds As New DataSet
        ds = SqlHelper.ExecuteDataset(ConnectionString, DatabaseOwner & ObjectQualifier & ProviderAppHeader & "Log_Add", ConfigurationId, SettingName, SettingValue, UserID, SessionID)
        If Not ds Is Nothing AndAlso Not ds.Tables Is Nothing AndAlso ds.Tables.Count > 0 AndAlso ds.Tables(0).Rows.Count > 0 Then
            If IsNumeric(ds.Tables(0).Rows(0).Item(0)) Then
                iReturn = CInt(ds.Tables(0).Rows(0).Item(0))
            End If
            ds.Clear()
            ds.Dispose()
        End If
        ds = Nothing
        Return iReturn
    End Function

    Public Overrides Sub AssignConfiguration(ByVal moduleId As String, ByVal configurationId As Guid, ByVal key As String)
        Dim mc As New DotNetNuke.Entities.Modules.ModuleController
        If key Is Nothing OrElse key.Length = 0 Then
            mc.UpdateModuleSetting(moduleId, "ConfigurationID", configurationId.ToString)
        Else
            mc.UpdateModuleSetting(moduleId, "ConfigurationID." & key, configurationId.ToString)
        End If
    End Sub

    Public Overrides Function UpgradeConfiguration(ByVal pageId As String, ByVal ModuleID As String) As Guid
        Try
            Return SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner & ObjectQualifier & ProviderAppHeader & "UpgradeConfiguration", pageId, ModuleID)
        Catch ex As Exception
            'Todo: implements exception unable to get the configuration id
            Return Guid.Empty
        End Try
    End Function
End Class