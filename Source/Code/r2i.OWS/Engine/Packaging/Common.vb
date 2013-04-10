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
Imports System.Xml
Imports System.Collections.Generic
Imports r2i.OWS.Framework.DataAccess
Imports r2i.OWS.Framework.Entities


Namespace r2i.OWS.Packaging
    Public Class Common
        'Friend Shared Function NameModule(ByVal mdI As DotNetNuke.Entities.Modules.ModuleInfo) As String
        '    Dim str As String = ""
        '    If Not mdI Is Nothing Then
        '        If Not mdI.PaneName Is Nothing AndAlso mdI.PaneName.Length > 0 Then
        '            str = mdI.PaneName
        '        End If
        '        If Not mdI.ModuleTitle Is Nothing AndAlso mdI.ModuleTitle.Length > 0 Then
        '            If Not str Is Nothing AndAlso str.Length > 0 Then
        '                str &= "-"
        '            End If
        '            str &= mdI.ModuleTitle
        '        End If
        '        str &= mdI.ModuleOrder
        '    End If
        '    Return str
        'End Function
        Friend Shared Function NameModule(ByVal mdI As IModuleInfo) As String
            Dim str As String = ""
            If Not mdI Is Nothing Then
                If Not mdI.PaneName Is Nothing AndAlso mdI.PaneName.Length > 0 Then
                    str = mdI.PaneName
                End If
                If Not mdI.ModuleTitle Is Nothing AndAlso mdI.ModuleTitle.Length > 0 Then
                    If Not str Is Nothing AndAlso str.Length > 0 Then
                        str &= "-"
                    End If
                    str &= mdI.ModuleTitle
                End If
                str &= mdI.ModuleOrder
            End If
            Return str
        End Function
        Friend Shared Function StreamTransfer(ByRef Source As IO.Stream, ByRef Destination As IO.Stream) As Boolean
            Dim sreader As New IO.BinaryReader(Source)
            Dim swriter As New IO.BinaryWriter(Destination)

            sreader.BaseStream.Position = 0

            While sreader.BaseStream.Position < sreader.BaseStream.Length
                swriter.Write(sreader.ReadByte())
            End While
        End Function
        Friend Shared Function canImport_TabProperty(ByVal Name As String) As Boolean
            Name = Name.ToUpper
            Select Case Name
                'Case "TABID"
                Case "TABORDER"
                Case "PORTALID"
                Case "PARENTID"
                Case "STARTDATE"
                Case "ENDDATE"
                Case "PANES"
                Case "MODULES"
                Case Else
                    Return True
            End Select
            Return False
        End Function
        'Friend Shared Function canImport_ModuleProperty(ByVal Name As String)
        '    Name = Name.ToUpper
        '    Select Case Name
        '        Case "PORTALID"
        '        Case "TABID"
        '        Case "TABMODULEID"
        '            'Case "MODULEID"
        '        Case "MODULEDEFID"
        '        Case "STARTDATE"
        '        Case "ENDDATE"
        '        Case "MODULEPERMISSIONS"
        '        Case "DESKTOPMODULEID"
        '        Case "DESCRIPTION"
        '        Case "VERSION"
        '        Case "ISPREMIUM"
        '        Case "ISADMIN"
        '        Case "BUSINESSCONTROLLERCLASS"
        '        Case "MODULECONTROLID"
        '        Case "CONTROLSRC"
        '        Case "CONTROLTYPE"
        '        Case "CONTROLTITLE"
        '        Case "HELPURL"
        '        Case Else
        '            Return True
        '    End Select
        '    Return False
        'End Function
        Friend Shared Function CaseLess_SelectSingleNode(ByRef SourceNode As XmlNode, ByVal Path As String) As XmlNode
            Dim paths() As String
            paths = Path.Split("/"c)
            Dim pathvalue As String
            Path = ""
            For Each pathvalue In paths
                Path &= "//*[translate(local-name(),'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') = '" & pathvalue.ToLower & "']"
            Next
            Dim xmld As New XmlDocument
            xmld.LoadXml(SourceNode.OuterXml)
            Return xmld.SelectSingleNode(Path)
        End Function
        Friend Shared Function CaseLess_SelectNodes(ByRef SourceNode As XmlNode, ByVal Path As String) As XmlNodeList
            Dim paths() As String
            paths = Path.Split("/"c)
            Dim pathvalue As String
            Path = ""
            For Each pathvalue In paths
                Path &= "//*[translate(local-name(),'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz') = '" & pathvalue.ToLower & "']"
            Next
            Dim xmld As New XmlDocument
            xmld.LoadXml(SourceNode.OuterXml)
            Return xmld.SelectNodes(Path)
        End Function
        'ROMAIN: Generic replacement - 08/21/2007
        'Friend Shared Sub ReplaceSourceValues(ByRef Source As String, ByRef Tabs As ArrayList, ByRef Modules As ArrayList, ByRef Settings As Hashtable)
        '    Dim ptabItem As InstallController.PackageItem
        '    For Each ptabItem In Tabs
        '        Source = Source.Replace("|!" & ptabItem.ItemName & "!|", ptabItem.DestinationID)
        '        If Not Settings Is Nothing Then
        '            Dim key As String
        '            Dim keys As New ArrayList(Settings.Keys)
        '            For Each key In keys
        '                Dim strItemValue = Settings.Item(key)
        '                strItemValue = strItemValue.Replace("|!" & ptabItem.ItemName & "!|", ptabItem.DestinationID)
        '                Settings.Item(key) = strItemValue
        '            Next
        '        End If
        '    Next

        '    Dim pmodItem As InstallController.PackageItem
        '    Dim ptabLookup As New InstallController
        '    For Each pmodItem In Modules
        '        ptabItem = ptabLookup.GetPackageItem(pmodItem.PackageID, pmodItem.ParentPackageItemID, -1, Nothing)
        '        If Not ptabItem Is Nothing Then
        '            Source = Source.Replace("|!" & ptabItem.ItemName & ":" & pmodItem.SourceID & "!|", pmodItem.DestinationID)
        '            If Not Settings Is Nothing Then
        '                Dim key As String
        '                Dim keys As New ArrayList(Settings.Keys)
        '                For Each key In keys
        '                    Dim strItemValue = Settings.Item(key)
        '                    strItemValue = strItemValue.Replace("|!" & ptabItem.ItemName & ":" & pmodItem.SourceID & "!|", pmodItem.DestinationID)
        '                    Settings.Item(key) = strItemValue
        '                Next
        '            End If
        '        End If
        '    Next
        'End Sub
        Friend Shared Sub ReplaceSourceValues(ByRef Source As String, ByRef Tabs As List(Of InstallController.PackageItem), ByRef Modules As List(Of InstallController.PackageItem), ByRef Settings As Hashtable)
            Dim ptabItem As InstallController.PackageItem
            For Each ptabItem In Tabs
                Source = Source.Replace("|!" & ptabItem.ItemName & "!|", ptabItem.DestinationID)
                If Not Settings Is Nothing Then
                    Dim key As String
                    'ROMAIN: Generic replacement - 08/21/2007
                    'Dim keys As New ArrayList(Settings.Keys)
                    Dim keys As New List(Of String)(Settings.Keys)
                    For Each key In keys
                        Dim strItemValue As Object = Settings.Item(key)
                        strItemValue = strItemValue.Replace("|!" & ptabItem.ItemName & "!|", ptabItem.DestinationID)
                        Settings.Item(key) = strItemValue
                    Next
                End If
            Next

            Dim pmodItem As InstallController.PackageItem
            Dim ptabLookup As New InstallController
            For Each pmodItem In Modules
                ptabItem = ptabLookup.GetPackageItem(pmodItem.PackageID, pmodItem.ParentPackageItemID, -1, Nothing)
                If Not ptabItem Is Nothing Then
                    Source = Source.Replace("|!" & ptabItem.ItemName & ":" & pmodItem.SourceID & "!|", pmodItem.DestinationID)
                    If Not Settings Is Nothing Then
                        Dim key As String
                        'ROMAIN: Generic replacement - 08/21/2007
                        'Dim keys As New ArrayList(Settings.Keys)
                        Dim keys As New List(Of String)(Settings.Keys)
                        For Each key In keys
                            Dim strItemValue As Object = Settings.Item(key)
                            strItemValue = strItemValue.Replace("|!" & ptabItem.ItemName & ":" & pmodItem.SourceID & "!|", pmodItem.DestinationID)
                            Settings.Item(key) = strItemValue
                        Next
                    End If
                End If
            Next
        End Sub
        'ROMAIN: Generic replacement - 08/22/2007
        'Private Shared _portalRoles As SortedList
        Private Shared _portalRoles As SortedList(Of String, String)
        'ROMAIN: Generic replacement - 08/22/2007
        'Friend Shared Function GetRoles(ByVal PortalID As Integer) As SortedList
        Friend Shared Function GetRoles(ByVal PortalID As Integer) As SortedList(Of String, String)
            If _portalRoles Is Nothing Then
                'Dim rC As New DotNetNuke.Security.Roles.RoleController
                'Dim rI As DotNetNuke.Security.Roles.RoleInfo
                Dim rI As IRole
                'ROMAIN: 09/20/07
                'Dim arr As ArrayList = rC.GetPortalRoles(PortalID)
                Dim arr As ArrayList = AbstractFactory.Instance.RoleController.GetPortalRoles(PortalID)
                'ROMAIN: Generic replacement - 08/22/2007
                '_portalRoles = New SortedList
                _portalRoles = New SortedList(Of String, String)
                If Not arr Is Nothing Then
                    For Each rI In arr
                        _portalRoles.Add(rI.RoleName.ToUpper, rI.Id)
                    Next
                End If
            End If
            Return _portalRoles
        End Function
        Friend Shared Function GetModuleDefinition(ByVal sourceXML As System.Xml.XmlNode, ByRef TargetType As String) As Integer
            If Not sourceXML Is Nothing Then
                Dim xnode As XmlNode = CaseLess_SelectSingleNode(sourceXML, "FriendlyName")
                If Not xnode Is Nothing Then
                    'ROMAIN: 09/20/07
                    'Dim mdC As New DotNetNuke.Entities.Modules.DesktopModuleController
                    'Dim mdI As DotNetNuke.Entities.Modules.DesktopModuleInfo = mdC.GetDesktopModuleByName(xnode.InnerXml)
                    Dim mdI As IDesktopModuleInfo
                    mdI = AbstractFactory.Instance.ModuleController.GetDesktopModuleByName(xnode.InnerXml)
                    If Not mdI Is Nothing Then
                        'ROMAIN: 09/20/07
                        'Dim dmC As New DotNetNuke.Entities.Modules.Definitions.ModuleDefinitionController
                        'Dim dmI As DotNetNuke.Entities.Modules.Definitions.ModuleDefinitionInfo = dmC.GetModuleDefinitionByName(mdI.DesktopModuleID, mdI.FriendlyName)
                        Dim dmI As IModuleDefinitionInfo
                        dmI = AbstractFactory.Instance.ModuleController.GetModuleDefinitionByName(mdI.DesktopModuleID, mdI.FriendlyName)
                        If Not dmI Is Nothing Then
                            If dmI.FriendlyName.ToUpper.StartsWith("LISTX") Then
                                TargetType = "LISTX"
                            ElseIf dmI.FriendlyName.ToUpper.StartsWith("TOOLBAR") Then
                                TargetType = "TOOLBAR"
                            End If
                            Return dmI.ModuleDefID
                        End If
                    End If
                End If
            End If
            Return -1
        End Function
        Friend Shared Function ByteToXMLDocument(ByVal source() As Byte) As System.Xml.XmlDocument
            Try
                If Not source Is Nothing AndAlso source.Length > 0 Then
                    Dim xml As New System.Xml.XmlDocument
                    xml.LoadXml(System.Text.UTF8Encoding.UTF8.GetString(source))
                    Return xml
                End If
            Catch ex As Exception
            End Try
            Return Nothing
        End Function
        Friend Shared Function ByteToString(ByVal source() As Byte) As String
            Try
                If Not source Is Nothing AndAlso source.Length > 0 Then
                    Return System.Text.UTF8Encoding.UTF8.GetString(source)
                End If
            Catch ex As Exception
            End Try
            Return Nothing
        End Function
        Public Class ItemTypeResult
            Public Enum ResultTypeEnumerator
                Install
                Package
                Configuration
                Resource
                Script
                Tab
                [Module]
            End Enum

            Public Succeeded As Boolean '= False
            Public Result As String
            Public Name As String
            Public Initiated As DateTime
            Public Completed As DateTime
            Private _resultType As ResultTypeEnumerator
            Public Sub New(ByVal [Type] As ResultTypeEnumerator)
                _resultType = [Type]
            End Sub
            Public ReadOnly Property ResultType() As ResultTypeEnumerator
                Get
                    Return _resultType
                End Get
            End Property
        End Class
        Public Enum PackageType
            StandardPackage
            ConfigurationOnly
            Unknown
            Failure
            FailureReadOnly
        End Enum
    End Class
End Namespace
