Imports DotNetNuke
Namespace DataAccess.Factories
    Public Class DesignFactory
        Private Shared _instance As DesignFactory = New DesignFactory()

        Public Shared ReadOnly Property Instance() As DesignFactory
            Get
                Return _instance
            End Get
        End Property

        'Public Function DescribeModule(ByRef elmt As KeyValuePair(Of Integer, IModuleInfo)) As String
        '    Dim btype As Integer = 0
        '    Dim Mi As ModuleInfo = elmt.Value
        '    If Mi.FriendlyName.ToUpper.StartsWith("LISTX") Then
        '        'THIS IS LISTX
        '        btype = 1
        '    ElseIf Mi.FriendlyName.ToUpper.StartsWith("TOOLBAR") Then
        '        'THIS IS TOOLBAR
        '        btype = 2
        '    End If
        '    If btype > 0 Then
        '        Dim str As String = vbTab & "<MODULE>" & vbCrLf
        '        Try
        '            If Not Mi.Header Is Nothing AndAlso Mi.Header.Length > 0 Then
        '                Mi.Header = GeneralizeContentSource(Mi.Header, mKeyList)
        '            End If
        '            If Not Mi.Footer Is Nothing AndAlso Mi.Footer.Length > 0 Then
        '                Mi.Footer = GeneralizeContentSource(Mi.Footer, mKeyList)
        '            End If

        '            Dim prop As System.Reflection.PropertyInfo
        '            For Each prop In Mi.GetType().GetProperties()
        '                If CanImport_ModuleProperty(prop.Name) Then
        '                    If prop.CanWrite And prop.CanRead Then
        '                        Dim nstr As String = vbTab & vbTab & "<" & prop.Name & ">"
        '                        Try
        '                            nstr &= XMLFormat(prop.GetValue(Mi, Nothing).ToString)
        '                        Catch ex As Exception
        '                        End Try
        '                        nstr &= "</" & prop.Name & ">" & vbCrLf
        '                        str &= nstr
        '                    End If
        '                End If
        '            Next
        '            ' str &= vbTab & vbTab & "<TabModuleName>" & XMLFormat(NameModule(Mi)) & "</TabModuleName>" & vbCrLf

        '            Dim mc As New DotNetNuke.Entities.Modules.ModuleController
        '            Dim msettings As Hashtable = mc.GetModuleSettings(Mi.ModuleID)
        '            If Not msettings Is Nothing AndAlso msettings.Count > 0 Then
        '                Dim key As String
        '                Dim value As String
        '                For Each key In msettings.Keys
        '                    value = msettings.Item(key)
        '                    If value Is Nothing Then value = ""
        '                    str &= vbTab & vbTab & "<SETTING KEY=""" & key & """>" & XMLFormat(GeneralizeContentSource(value, mKeyList)) & "</SETTING>" & vbCrLf
        '                Next
        '            End If
        '            Dim src As String = ""
        '            If btype = 1 Then
        '                'LISTX CONFIG
        '                Dim LX As New r2i.OWS.Framework.Utilities.Compatibility.Settings
        '                src = LX.GetSetting(Mi.TabId, Mi.ModuleID)
        '                LX = Nothing
        '            ElseIf btype = 2 Then
        '                'TOOLBAR CONFIG
        '                Dim LX As New Bi4ce.Modules.Toolbar.ToolbarController
        '                src = LX.GetToolbarSetting(Mi.TabId, Mi.ModuleID)
        '                LX = Nothing
        '            End If
        '            If src Is Nothing Then src = ""
        '            str &= vbTab & vbTab & "<CONFIGURATION>" & XMLFormat(src) & "</CONFIGURATION>"
        '        Catch ex As Exception
        '            Return False
        '        End Try
        '        str &= vbTab & "</MODULE>" & vbCrLf
        '        Return str
        '    Else
        '        'ROMAIN: 08/22/2007
        '        'NOTE: Replacement Return ""
        '        Return String.Empty
        '    End If
        'End Function

        Public Function DescribeModule(ByRef Mi As IModuleInfo, ByRef mKeyList As SortedList(Of Integer, String)) As String
            Dim btype As Integer = 0


            If Mi.FriendlyName.ToUpper.StartsWith("OWS") Then
                'THIS IS LISTX
                btype = 1
            End If
            If btype >= 0 Then
                Dim str As String = vbTab & "<MODULE>" & vbCrLf
                Dim configId As String = ""
                Try
                    If Not Mi.Header Is Nothing AndAlso Mi.Header.Length > 0 Then
                        Mi.Header = CStr(GeneralizeContentSource(Mi.Header, mKeyList))
                    End If
                    If Not Mi.Footer Is Nothing AndAlso Mi.Footer.Length > 0 Then
                        Mi.Footer = CStr(GeneralizeContentSource(Mi.Footer, mKeyList))
                    End If

                    Dim prop As System.Reflection.PropertyInfo
                    For Each prop In GetType(IModuleInfo).GetProperties()
                        If CBool(CanImport_ModuleProperty(prop.Name)) Then
                            If prop.CanWrite And prop.CanRead Then
                                Dim nstr As String = vbTab & vbTab & "<" & prop.Name & ">"
                                Try
                                    nstr &= XMLFormat(prop.GetValue(Mi, Nothing).ToString)
                                Catch ex As Exception
                                End Try
                                nstr &= "</" & prop.Name & ">" & vbCrLf
                                str &= nstr
                            End If
                        End If
                    Next
                    ' str &= vbTab & vbTab & "<TabModuleName>" & XMLFormat(NameModule(Mi)) & "</TabModuleName>" & vbCrLf

                    Dim mc As New DotNetNuke.Entities.Modules.ModuleController
                    Dim msettings As Hashtable = mc.GetModuleSettings(CInt(Mi.ModuleID))
                    If Not msettings Is Nothing AndAlso msettings.Count > 0 Then
                        Dim key As String
                        Dim value As String
                        For Each key In msettings.Keys
                            value = CStr(msettings.Item(key))
                            If value Is Nothing Then value = ""
                            str &= vbTab & vbTab & "<SETTING KEY=""" & key & """>" & XMLFormat(CStr(GeneralizeContentSource(value, mKeyList))) & "</SETTING>" & vbCrLf
                        Next
                    End If
                    Dim src As String = ""
                    'Dim configurationId As String = CStr(Mi.TabID) & ":" & CStr(Mi.ModuleID)
                    'If btype = 1 Then
                    '    'LISTX CONFIG

                    '    Dim ConfigCtrl As New Controller
                    '    src = ConfigCtrl.GetSetting(New Guid(configId))
                    'ElseIf btype = 2 Then
                    '    'TOOLBAR CONFIG
                    '    'TODO: Add Toolbar Support??
                    '    'Dim LX As New Modules.Toolbar.ToolbarController
                    '    'NOTE: If necessary pass the configId Guid As a parameter or get the configId By Name
                    '    'src = LX.GetToolbarSetting(configurationId)
                    '    'LX = Nothing
                    'End If
                    src = ExportModule(Mi)
                    If src Is Nothing Then src = ""
                    str &= vbTab & vbTab & "<CONFIGURATION>" & XMLFormat(src) & "</CONFIGURATION>"
                Catch ex As Exception
                    Return CStr(False)
                End Try
                str &= vbTab & "</MODULE>" & vbCrLf
                Return str
            Else
                'ROMAIN: 08/22/2007
                'NOTE: Replacement Return ""
                Return String.Empty
            End If
        End Function

        Private Function ExportModule(ByRef Mi As IModuleInfo) As String

            Dim strMessage As String = ""

            Dim objModules As New DotNetNuke.Entities.Modules.ModuleController
            Dim objModule As DotNetNuke.Entities.Modules.ModuleInfo = objModules.GetModule(CInt(Mi.ModuleID), CInt(Mi.TabId))
            If Not objModule Is Nothing Then
                If objModule.DesktopModule.BusinessControllerClass <> "" And objModule.DesktopModule.IsPortable Then
                    Try
                        Dim objObject As Object = DotNetNuke.Framework.Reflection.CreateObject(objModule.DesktopModule.BusinessControllerClass, objModule.DesktopModule.BusinessControllerClass)

                        'Double-check
                        If TypeOf objObject Is DotNetNuke.Entities.Modules.IPortable Then

                            Dim Content As String = CType(CType(objObject, DotNetNuke.Entities.Modules.IPortable).ExportModule(CInt(objModule.ModuleID)), String)

                            If Content <> "" Then
                                ' add attributes to XML document
                                Content = "<?xml version=""1.0"" encoding=""utf-8"" ?>" & _
                                  "<content type=""" & CleanName(objModule.DesktopModule.FriendlyName) & """ version=""" & objModule.DesktopModule.Version & """>" & _
                                  Content & _
                                  "</content>"
                            End If
                            Return Content
                        End If
                    Catch
                    End Try
                End If
            End If
            Return ""
        End Function
        Public Function ImportModule(ByRef Mi As IModuleInfo, ByVal UserID As String, ByVal Content As String) As String
            Dim mc As New DotNetNuke.Entities.Modules.ModuleController
            Dim objModule As DotNetNuke.Entities.Modules.ModuleInfo = mc.GetModule(CInt(Mi.ModuleID), CInt(Mi.TabID)) 'CType(CType(Mi, DataAccess.ModuleInfo).Save, DotNetNuke.Entities.Modules.ModuleInfo)
            If Not objModule Is Nothing Then
                If objModule.DesktopModule.BusinessControllerClass <> "" And objModule.DesktopModule.IsPortable Then
                    Try
                        Dim objObject As Object = DotNetNuke.Framework.Reflection.CreateObject(objModule.DesktopModule.BusinessControllerClass, objModule.DesktopModule.BusinessControllerClass)

                        If TypeOf objObject Is DotNetNuke.Entities.Modules.IPortable Then

                            Dim xmlDoc As New System.Xml.XmlDocument
                            Try
                                xmlDoc.LoadXml(Content)
                            Catch
                                Return "Invalid XML Document"
                            End Try


                            Dim strType As String = xmlDoc.DocumentElement.GetAttribute("type").ToString
                            If strType = CleanName(objModule.DesktopModule.FriendlyName) Or strType = CleanName(objModule.DesktopModule.FriendlyName) Then
                                Dim strVersion As String = xmlDoc.DocumentElement.GetAttribute("version").ToString

                                CType(objObject, DotNetNuke.Entities.Modules.IPortable).ImportModule(CInt(Mi.ModuleID), xmlDoc.DocumentElement.InnerXml, strVersion, CInt(UserID))
                                Return "Success"
                            Else
                                Return "Incorrect Type"
                            End If
                        Else
                            Return "Not Supported"
                        End If
                    Catch
                        Return "Runtime Error"
                    End Try
                Else
                    Return "Not Supported"
                End If
            Else
                Return "Incorrect Type"
            End If

            Return "Aborted"
        End Function

        Private Function CleanName(ByVal Name As String) As String

            Dim strName As String = Name
            Dim strBadChars As String = ". ~`!@#$%^&*()-_+={[}]|\:;<,>?/" & Chr(34) & Chr(39)

            Dim intCounter As Integer
            For intCounter = 0 To Len(strBadChars) - 1
                strName = strName.Replace(strBadChars.Substring(intCounter, 1), "")
            Next intCounter

            Return strName

        End Function


        Private Function GeneralizeContentSource(ByVal Source As Object, ByVal slK As SortedList(Of Integer, String)) As Object
            If TypeOf (Source) Is String Then
                Dim str As String = CStr(Source)
                Dim mID As Integer
                For Each mID In slK.Keys
                    str = str.Replace(mID.ToString, "|!" & slK(mID) & "!|")
                Next
                Return str
            Else
                Return Source
            End If
        End Function


        Private Function CanImport_ModuleProperty(ByVal Name As String) As String
            Name = Name.ToUpper
            Select Case Name
                Case "PORTALID"
                Case "TABID"
                Case "TABMODULEID"
                    'Case "MODULEID"
                Case "MODULEDEFID"
                Case "STARTDATE"
                Case "ENDDATE"
                Case "MODULEPERMISSIONS"
                Case "DESKTOPMODULEID"
                Case "DESCRIPTION"
                Case "VERSION"
                Case "ISPREMIUM"
                Case "ISADMIN"
                Case "BUSINESSCONTROLLERCLASS"
                Case "MODULECONTROLID"
                Case "CONTROLSRC"
                Case "CONTROLTYPE"
                Case "CONTROLTITLE"
                Case "HELPURL"
                Case Else
                    Return CStr(True)
            End Select
            Return CStr(False)
        End Function

        Private Function XMLFormat(ByVal Value As String) As String
            Return Value.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;")
        End Function
    End Class
End Namespace
