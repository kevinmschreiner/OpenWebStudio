Imports System.Web.UI.WebControls
Imports DotNetNuke.Entities.Modules
Imports System.Web

Namespace DataAccess.Factories
    Public Class PortalModuleBaseUIFactory

        Private Shared _instance As PortalModuleBaseUIFactory = New PortalModuleBaseUIFactory()

        Public Shared ReadOnly Property Instance() As PortalModuleBaseUIFactory
            Get
                Return _instance
            End Get
        End Property

        Public Function GetContainerControl(ByVal portalModBase As DotNetNuke.Entities.Modules.PortalModuleBase, ByVal moduleId As String) As System.Web.UI.Control
            Return DotNetNuke.Common.Globals.FindControlRecursive(portalModBase, "ctr" & portalModBase.ModuleId.ToString())
        End Function

        Public Function EditUrl() As String
            Dim pC As New DotNetNuke.Entities.Modules.PortalModuleBase
            Return pC.EditUrl
        End Function

        Public Function GetNextActionID() As Integer
            Dim pC As New DotNetNuke.Entities.Modules.PortalModuleBase
            Return pC.GetNextActionID()
        End Function


        Public Shared Function GetPortalModuleBase(ByVal objControl As System.Web.UI.UserControl) As IPortalModuleBaseUI

            'Dim objPortalModuleBase As New DotNetNuke.Entities.Modules.PortalModuleBase
            'objPortalModuleBase = DotNetNuke.UI.Containers.Container.GetPortalModuleBase(objControl)
            'objPortalModuleBase = DotNetNuke.UI.Containers.Container.GetPortalModuleBase(DnnSingleton.GetInstance(HttpContext.Current).CurrentPortalModuleBase)


            Dim PortalModuleBaseUI As New PortalModuleBaseUI

            'Dim _moduleActionColl As DotNetNuke.Entities.Modules.Actions.ModuleActionCollection = objPortalModuleBase.Actions
            'Dim listXModActionColl As New ModuleActionCollection
            'listXModActionColl = CType(_moduleActionColl, ModuleActionCollection)
            'PortalModuleBaseUI.Actions = CType(objPortalModuleBase.Actions, IModuleActionCollection)
            'TODO: RAISE AN EXCEPTION - TO FIX
            'Try
            '    PortalModuleBaseUI.Actions = PortalModuleBaseUIFactory.Instance.FillModuleAction(objPortalModuleBase.Actions)
            'Catch ex As Exception
            '    PortalModuleBaseUI.Actions = New ModuleActionCollection
            'End Try
            'PortalModuleBaseUI.ContainerControl = objPortalModuleBase.ContainerControl
            PortalModuleBaseUI.moduleId = CStr(DnnSingleton.GetInstance(HttpContext.Current).CurrentModuleBase.ModuleId)
            PortalModuleBaseUI.modulePath = DnnSingleton.GetInstance(HttpContext.Current).CurrentModuleBase.ModulePath

            Return PortalModuleBaseUI

        End Function

        Private Function FillModuleAction(ByVal initialCollection As DotNetNuke.Entities.Modules.Actions.ModuleActionCollection) As ModuleActionCollection
            Dim modActColl As New ModuleActionCollection
            If Not initialCollection Is Nothing Then
                For Each item As DotNetNuke.Entities.Modules.Actions.ModuleAction In initialCollection
                    Dim modAct As New ModuleAction
                    modAct.Visible = item.Visible
                    modActColl.Add(modAct)
                Next
            End If
            Return modActColl
        End Function
    End Class
End Namespace

