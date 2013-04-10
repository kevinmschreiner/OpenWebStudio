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
Imports System.IO
Imports System.Web
Imports System.Web.Hosting
Imports System.Threading
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Scheduling

Public Class Scheduler
    Inherits DotNetNuke.Services.Scheduling.SchedulerClient
    Public Sub New(ByVal objScheduleHistoryItem As DotNetNuke.Services.Scheduling.ScheduleHistoryItem)
        MyBase.new()

        Me.ScheduleHistoryItem = objScheduleHistoryItem

    End Sub
    Public Overrides Sub DoWork()
        Try
            Me.Progressing()

            SchedulerHttpContext.SetHttpContextWithSimulatedRequest()
            HttpContext.Current.Items.Add("OWS_SCHEDULER", True)
            Me.ScheduleHistoryItem.Succeeded = False

            Run()

            Me.ScheduleHistoryItem.Succeeded = True
            Me.ScheduleHistoryItem.AddLogNote("OpenWebStudio Scheduled Configuration completed.")
        Catch exc As Exception
            Me.ScheduleHistoryItem.Succeeded = False
            Me.ScheduleHistoryItem.AddLogNote("OpenWebStudio Scheduled Configuration failed. " + exc.ToString)
            Me.Errored(exc)
            LogException(exc)
        End Try
    End Sub
    Public Sub Run()
        Dim strConfig As String = Me.ScheduleHistoryItem.ObjectDependencies
        If Not strConfig Is Nothing AndAlso strConfig.Length > 0 Then
            Dim configId As Guid
            Try
                configId = New Guid(strConfig)
                If configId.ToString = Guid.Empty.ToString Then
                    Throw New Exception("The ConfigurationID was not in the correct format.")
                End If
            Catch ex As Exception
                Throw New Exception("The ConfigurationID was not in the correct format.")
            End Try

            Dim sb As New System.Text.StringBuilder
            Dim tw As New IO.StringWriter(sb)
            Dim twHTML As New System.Web.UI.HtmlTextWriter(tw)

            Dim xdnn As New OpenControl
            'Initialize the Control Properties
            Dim spath As String = "/"

            xdnn.SetParentBase(New BaseParentControl(Me, "", spath, "Scheduler"))
            DnnSingleton.GetInstance(HttpContext.Current).CurrentModuleBase = New BaseParentControl(Me, "", spath & "DesktopModules/OWS/", "Scheduler")
            xdnn.SetOWSPath(spath & "DesktopModules/OWS/")

            Dim p As New System.Web.UI.Page()
            p.Items.Add("OWS_SCHEDULER", True)
            xdnn.ID = "Scheduler"
            xdnn.ConfigurationId = configId
            xdnn.Page = p
            xdnn.ModuleId = -1
            xdnn.PageId = -1
            xdnn.PageModuleId = -1
            p.Controls.Add(xdnn)

            xdnn.GetType().InvokeMember("OnInit", Reflection.BindingFlags.InvokeMethod Or Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance, Nothing, xdnn, New Object() {Nothing})
            xdnn.GetType().InvokeMember("OnLoad", Reflection.BindingFlags.InvokeMethod Or Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance, Nothing, xdnn, New Object() {Nothing})
            xdnn.GetType().InvokeMember("OnPreRender", Reflection.BindingFlags.InvokeMethod Or Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance, Nothing, xdnn, New Object() {Nothing})
            xdnn.RenderControl(twHTML)
            tw.Flush()
            xdnn.GetType().InvokeMember("OnUnload", Reflection.BindingFlags.InvokeMethod Or Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance, Nothing, xdnn, New Object() {Nothing})

            Me.ScheduleHistoryItem.AddLogNote(sb.ToString)

        Else
            Throw New Exception("Unable to execute configuration: No configuration GUID was supplied within the Object Dependencies property.")
        End If
    End Sub
End Class
Public Class SchedulerHttpContext
    Private Shared appPhysicalDir As String

    Public Shared Sub SetHttpContextWithSimulatedRequest()

        Dim page As String = (HttpRuntime.AppDomainAppVirtualPath + "/default.aspx").TrimStart("/")
        Dim query As String = ""
        Dim output As StringWriter = New StringWriter

        Thread.GetDomain.SetData(".hostingInstallDir", HttpRuntime.AspInstallDirectory)
        Thread.GetDomain.SetData(".hostingVirtualPath", HttpRuntime.AppDomainAppVirtualPath)
        Dim workerRequest As SimpleWorkerRequest = New SimpleWorkerRequest(page, query, output)
        HttpContext.Current = New HttpContext(workerRequest)

    End Sub

End Class
