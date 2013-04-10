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
Imports System.Net.Mail
Imports System.Collections.Generic
Imports System.Net
Imports System.Reflection
Imports System.Configuration
Imports System.Text
Imports r2i.OWS.Framework.Plugins.Actions
Imports r2i.OWS.Framework.Utilities
Imports r2i.OWS.Framework.Utilities.Compatibility
Imports r2i.OWS.Framework.Utilities.JSON

Namespace r2i.OWS.Framework.Utilities.Engine
    Public Class Utility

#Region "Configuration Utilities"
        Public Class AbstractProvider
            Public ClassName As String
            Public Assembly As String
        End Class
        Public NotInheritable Class WrapperSection
            Inherits System.Configuration.ConfigurationSection
            Private Shared _Properties As ConfigurationPropertyCollection

            ' The FileName property.
            Private Shared _Assembly As New ConfigurationProperty("Assembly", GetType(String), "", ConfigurationPropertyOptions.IsRequired)

            ' The MasUsers property.
            Private Shared _ClassName As New ConfigurationProperty("ClassName", GetType(String), "", ConfigurationPropertyOptions.IsRequired)

            ' CustomSection constructor.
            Public Sub New()
                ' Property initialization
                _Properties = New ConfigurationPropertyCollection()

                _Properties.Add(_Assembly)
                _Properties.Add(_ClassName)
            End Sub 'New
            Public Sub New(ByVal Assembly As String, ByVal ClassName As String)
                Me.New()
                Me.Assembly = Assembly
                Me.ClassName = ClassName
            End Sub

            ' This is a key customization. 
            ' It returns the initialized property bag.
            Protected Overrides ReadOnly Property Properties() As ConfigurationPropertyCollection
                Get
                    Return _Properties
                End Get
            End Property

            Public Property Assembly() As String
                Get
                    Return CStr(Me("Assembly"))
                End Get
                Set(ByVal value As String)

                    Me("Assembly") = value
                End Set
            End Property


            Public Property ClassName() As String
                Get
                    Return CStr(Me("ClassName"))
                End Get
                Set(ByVal value As String)

                    Me("ClassName") = value
                End Set
            End Property
        End Class
#End Region
#Region "Runtime Utilities"
        Public Shared Function SendEMail(ByVal Message As MailMessage, ByVal smtpServerExternal As String, ByVal SMTPAuthentication As String, ByVal SMTPUsername As String, ByVal SMTPPassword As String, Optional ByVal SMTPEnableSSL As Boolean = False) As String

            Dim smtpMailClient As New SmtpClient
            ' external SMTP server
            smtpMailClient.EnableSsl = SMTPEnableSSL
            If smtpServerExternal <> "" Then
                If smtpServerExternal.Contains(":") Then
                    Try
                        smtpMailClient.Port = smtpServerExternal.Split(":"c).GetValue(1)
                        smtpMailClient.Host = smtpServerExternal.Split(":"c).GetValue(0)
                    Catch
                        smtpMailClient.Host = smtpServerExternal
                    End Try
                Else
                    smtpMailClient.Host = smtpServerExternal
                End If
                Select Case SMTPAuthentication
                    Case "", "0" ' anonymous
                    Case "1" ' basic
                        If SMTPUsername <> "" And SMTPPassword <> "" Then
                            smtpMailClient.Credentials = New NetworkCredential(SMTPUsername, SMTPPassword)
                        End If
                    Case "2" ' NTLM
                        smtpMailClient.UseDefaultCredentials = True
                End Select
            End If

            Try
                If Not Message.Attachments Is Nothing Then
                    Dim attachment As System.Net.Mail.Attachment
                    For Each attachment In Message.Attachments
                        Try
                            If Not attachment Is Nothing Then
                                attachment.ContentStream.Position = 0
                            End If
                        Catch ex As Exception
                        End Try
                    Next
                End If
            Catch ex As Exception
            End Try

            Try
                smtpMailClient.Send(Message)
                SendEMail = "True"
            Catch smtpEx As SmtpException
                ' mail configuration problem
                SendEMail = smtpEx.Message
                If Not smtpEx.InnerException Is Nothing Then
                    SendEMail += ": InnerException - " + smtpEx.InnerException.Message()
                End If
                SendEMail += ": StackTrace - " + smtpEx.ToString
            Catch generalEx As Exception
                'Some other problem occurred
                SendEMail = generalEx.Message
            End Try

            If Not Message.Attachments Is Nothing Then
                Dim attachment As System.Net.Mail.Attachment
                For Each attachment In Message.Attachments
                    Try
                        If Not attachment Is Nothing Then
                            attachment.ContentStream.Close()
                            attachment.ContentStream.Dispose()
                            attachment.Dispose()
                        End If
                    Catch ex As Exception
                    End Try
                Next
            End If

        End Function

        ''' <summary>
        ''' Construct a datatable from the objects given
        ''' </summary>
        ''' <param name="Objects">Collection of objects. These provide data for the rows.</param>
        ''' <param name="Properties">Optional list of Property Names in the Objects() to construct the columns. If <i>null</i> then all properties will be used.</param>
        ''' <param name="TableName">Optional name for the table to retrun.</param>
        ''' <returns>DataTable</returns>
        Public Shared Function CollectionToDataTable(ByVal Objects As ICollection, Optional ByVal Properties As List(Of String) = Nothing, Optional ByVal TableName As String = "Table") As DataTable
            Dim dt As New DataTable(TableName)

            If Not Objects Is Nothing AndAlso Objects.Count > 0 Then
                Dim dr As DataRow = Nothing

                For Each obj As Object In Objects
                    Try
                        If dt.Columns.Count = 0 Then
                            Dim props As PropertyInfo() = obj.GetType().GetProperties()

                            For Each prop As PropertyInfo In props
                                If (Not Properties Is Nothing AndAlso Properties.Contains(prop.Name)) OrElse (Properties Is Nothing) Then
                                    If prop.CanRead AndAlso Not dt.Columns.Contains(prop.Name) Then
                                        dt.Columns.Add(prop.Name, prop.PropertyType)
                                    End If
                                End If
                            Next
                        End If

                        Try
                            dr = dt.NewRow()
                            For Each dc As DataColumn In dt.Columns
                                Dim prop As PropertyInfo = obj.GetType().GetProperty(dc.ColumnName, New System.Type() {})
                                If Not prop Is Nothing Then
                                    Try
                                        dr.Item(dc.ColumnName) = prop.GetValue(obj, Nothing)
                                    Catch ex As Exception
                                        dr.Item(dc.ColumnName) = Nothing
                                    End Try
                                End If
                            Next
                            dt.Rows.Add(dr)
                        Catch ex As Exception
                        End Try
                    Catch ex As Exception
                    End Try
                Next
            ElseIf Not Properties Is Nothing Then
                For Each sProp As String In Properties
                    dt.Columns.Add(sProp, GetType(Object))
                Next
            End If

            Return dt
        End Function
        


        Private Const OWS_QS_Key As String = "_OWS_"
        Public Shared Function ParseOWSIncomingParameters(ByVal src As System.Web.HttpRequest) As Dictionary(Of String, String)
            Dim dict As New Dictionary(Of String, String)
            Dim strValue As String = src.QueryString(OWS_QS_Key)
            If Not strValue Is Nothing AndAlso strValue.Length > 0 Then
                Dim strNVP As String() = strValue.Split(",")
                Dim nvp As String
                For Each nvp In strNVP
                    Dim nv As String() = nvp.Split(":")
                    Dim name As String = nv(0)
                    Dim value As String = ""
                    Dim i As Integer
                    If nv.Length > 1 Then
                        For i = 1 To nv.Length - 1
                            If i > 1 Then
                                value &= ":"
                            End If
                            value &= nv(i)
                        Next
                    End If
                    If Not dict.ContainsKey(name) Then
                        dict.Add(name, value)
                    End If
                Next
            End If
            Return dict
        End Function



        Public Shared Function GetRegions(ByVal configId As String) As List(Of r2i.OWS.Framework.Plugins.Actions.MessageActionItem)
            If Not configId Is Nothing Then
                Dim gCID As New Guid(configId)
                Dim sController As New Controller
                Dim strCurrentSource As String = sController.GetSetting(gCID)
                Dim json As New r2i.OWS.Framework.Utilities.JSON.JsonConversion
                Dim xls As r2i.OWS.Framework.Settings = json.GetDeserializedListXConfiguration(strCurrentSource)
                Dim sRegions As String = ""

                If Not xls Is Nothing Then
                    Dim miRegions As List(Of r2i.OWS.Framework.Plugins.Actions.MessageActionItem) = MessageActionItem.FindAll(xls.messageItems, "Action-Region")

                    Return miRegions
                End If

            Else
                'TODO: implements Exception configurationId not available
            End If
            Return Nothing
        End Function

        Public Shared Function GetRegion(ByVal configId As String, ByVal region As String, ByRef current As Settings) As MessageActionItem
            Dim xls As Settings
            If Not configId Is Nothing AndAlso Not configId = Guid.Empty.ToString AndAlso Not configId = current.ConfigurationID Then
                Dim gCID As New Guid(configId)
                Dim sController As New Controller
                Dim strCurrentSource As String = sController.GetSetting(gCID)
                Dim json As New JsonConversion
                xls = json.GetDeserializedListXConfiguration(strCurrentSource)
                Dim sRegions As String = ""
            Else
                xls = current
            End If
            If Not xls Is Nothing Then
                Dim miRegions As List(Of MessageActionItem) = MessageActionItem.FindAll(xls.messageItems, "Action-Region", "Name", region)
                If miRegions.Count > 0 Then
                    Return miRegions.Item(0)
                End If
            End If
            Return Nothing
        End Function

        Public Shared Function GetConfigurationActions(ByVal configId As String, ByRef current As Settings) As List(Of MessageActionItem)
            Dim xls As Settings
            If Not configId Is Nothing AndAlso Not configId = Guid.Empty.ToString AndAlso Not configId = current.ConfigurationID Then
                Dim gCID As New Guid(configId)
                Dim sController As New Controller
                Dim strCurrentSource As String = sController.GetSetting(gCID)
                Dim json As New JsonConversion
                xls = json.GetDeserializedListXConfiguration(strCurrentSource)
                Dim sRegions As String = ""
            Else
                xls = current
            End If
            If Not xls Is Nothing Then
                Return xls.messageItems
            End If
            Return Nothing
        End Function
#End Region

    End Class
End Namespace