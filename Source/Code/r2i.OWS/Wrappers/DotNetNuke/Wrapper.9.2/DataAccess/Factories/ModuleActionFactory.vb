Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Actions
Imports DotNetNuke.UI.Utilities
Imports DotNetNuke
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Security
Imports DotNetNuke.Entities.Modules.PortalModuleBase

Namespace DataAccess.Factories
    Public Class ModuleActionFactory
        'Inherits Entities.Modules.PortalModuleBase

        Private Shared _instance As ModuleActionFactory = New ModuleActionFactory()
        Public Shared ReadOnly Property Instance() As ModuleActionFactory
            Get
                Return _instance
            End Get
        End Property

        Public Sub PopulateActionMenu(ByRef Actions As DotNetNuke.Entities.Modules.Actions.ModuleActionCollection, ByVal modulePath As String, ByVal url As String)
            'ADD PACKAGE INSTALLER
            Dim pM As New DotNetNuke.Entities.Modules.PortalModuleBase
            Dim act As New DotNetNuke.Entities.Modules.Actions.ModuleActionCollection
            Dim pGI As New DotNetNuke.Entities.Modules.Actions.ModuleAction(pM.GetNextActionID, "Extended", "", "", "", "", "", False, SecurityAccessLevel.Edit, True)

            'ADD CLEAR MODULE CACHE
            Dim cM As DotNetNuke.Entities.Modules.Actions.ModuleAction
            'ADD CLEAR WEB CACHE
            Dim cW As DotNetNuke.Entities.Modules.Actions.ModuleAction
            Dim cBreak1 As DotNetNuke.Entities.Modules.Actions.ModuleAction
            'ADD TOGGLE DEBUGGING MODE
            Dim cD As DotNetNuke.Entities.Modules.Actions.ModuleAction
            'ADD TOGGLE DEBUGGING MODE
            Dim cR As DotNetNuke.Entities.Modules.Actions.ModuleAction
            Dim cBreak2 As DotNetNuke.Entities.Modules.Actions.ModuleAction
            'ADD BUILDER
            Dim cB As DotNetNuke.Entities.Modules.Actions.ModuleAction
            Dim isLowerVersion As Boolean = False
            Dim major As Integer
            Dim minor As Integer
            GetHostVersion(major, minor)
            If major <= 3 OrElse (major = 4 And minor <= 1) Then
                isLowerVersion = True
            End If

            cM = pGI.Actions.Add(pM.GetNextActionID, "Clear Module Cache", "ClearModCache", "", "restore.gif", Nothing, True, SecurityAccessLevel.Edit, True, False)
            cW = pGI.Actions.Add(pM.GetNextActionID, "Clear Web Cache", "ClearWebCache", "", "restore.gif", Nothing, True, SecurityAccessLevel.Edit, True, False)
            cBreak1 = pGI.Actions.Add(pM.GetNextActionID, "~", "")
            cD = pGI.Actions.Add(pM.GetNextActionID, "Toggle Debugging", "ToggleDebugging", "", "rec.gif", Nothing, True, SecurityAccessLevel.Edit, True, False)
            cR = pGI.Actions.Add(pM.GetNextActionID, "Toggle Redirects", "ToggleRedirects", "", "rev.gif", Nothing, True, SecurityAccessLevel.Edit, True, False)
            cBreak2 = pGI.Actions.Add(pM.GetNextActionID, "~", "")
            cB = pGI.Actions.Add(pM.GetNextActionID, "Quick Builder", "Edit", "", "ratingplus.gif", Url:=Replace(url, "Edit", "Build"), UseActionEvent:=True, Secure:=SecurityAccessLevel.Edit, Visible:=True, ClientScript:=Nothing, NewWindow:=False)
            pGI.Actions.Add(pM.GetNextActionID, "Repository", "Edit", "", "frew.gif", Url:=Replace(url, "Edit", "Repository"), UseActionEvent:=True, Secure:=SecurityAccessLevel.Edit, Visible:=True, ClientScript:=Nothing, NewWindow:=False)
            pGI.Actions.Add(pM.GetNextActionID, "Log", "Edit", "", "icon_viewstats_16px.gif", Url:=Replace(url, "Edit", "Log"), UseActionEvent:=True, Secure:=SecurityAccessLevel.Edit, Visible:=True, ClientScript:=Nothing, NewWindow:=False)

            Dim cBreak3 As DotNetNuke.Entities.Modules.Actions.ModuleAction = act.Add(pM.GetNextActionID, "~", "")

            Dim cH As DotNetNuke.Entities.Modules.Actions.ModuleAction = act.Add(pM.GetNextActionID, "Online Help", "Help", "", "Help.gif", modulePath & "Help.html", "", False, Security.SecurityAccessLevel.Edit, True, True)

            Dim cV As DotNetNuke.Entities.Modules.Actions.ModuleAction = act.Add(pM.GetNextActionID, "View Options", "Edit", "", "Edit.gif", Url:=Replace(url, "Edit", "Edit"), UseActionEvent:=True, Secure:=SecurityAccessLevel.Edit, Visible:=True, ClientScript:=Nothing, NewWindow:=False)

            Actions.Insert(0, cV)
            If isLowerVersion Then
                Actions.AddRange(pGI.Actions)
            Else
                Actions.Insert(0, pGI)
            End If
            Actions.Insert(0, cBreak1)
            Actions.Insert(0, cH)
        End Sub

        Private Shared Sub GetHostVersion(ByRef Major As Integer, ByRef Minor As Integer)
            Static _loaded As Boolean = False
            Static _major As Integer
            Static _minor As Integer
            If Not _loaded Then
                'TOBREAK: Remove the Type for DNN and just get ME.Parent
                Dim ass As System.Reflection.Assembly = System.Reflection.Assembly.GetAssembly(GetType(DotNetNuke.Common.Globals))
                _minor = ass.GetName.Version.Minor
                _major = ass.GetName.Version.Major
                _loaded = True
            End If
            Major = _major
            Minor = _minor
        End Sub
    End Class
End Namespace
