Imports r2i.OWS.Framework.DataAccess
Imports System.Web.UI.HtmlControls

Namespace DataAccess.Factories
    Public Class ConfigurationFactory
        Private Shared _instance As ConfigurationFactory = New ConfigurationFactory()

        Public Shared ReadOnly Property Instance() As ConfigurationFactory
            Get
                Return _instance
            End Get
        End Property

        Public Sub AssignConfiguration(ByVal moduleId As String, ByVal configurationId As System.Guid, ByVal Key As String)
            DataProvider.Instance().AssignConfiguration(moduleId, configurationId, Key)
        End Sub

        Public Sub LoadConfigurations_Environment(ByVal parentObject As System.Web.UI.HtmlControls.HtmlControl)
            'Dim ds As DataSet = SqlDataProvider.ExecuteDataSet("SELECT ConfigurationID, ConfigurationName FROM Settings")
            'Dim cbo As HtmlSelect = CType(parentObject, HtmlSelect)

            'If Not ds Is Nothing AndAlso ds.Tables.Count > 0 Then
            '    For Each dr As DataRow In ds.Tables(0).Rows
            '        cbo.Items.Add(New System.Web.UI.WebControls.ListItem(CStr(dr.Item("ConfigurationName")), CType(dr.Item("ConfigurationID"), Guid).ToString()))
            '    Next
            'End If
        End Sub
    End Class
End Namespace


