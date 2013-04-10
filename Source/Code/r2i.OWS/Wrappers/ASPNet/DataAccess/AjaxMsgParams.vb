' ''<LICENSE>
' ''   Open Web Studio - http://www.OpenWebStudio.com
' ''   Copyright (c) 2007-2008
' ''   by R2Integrated Inc. http://www.R2integrated.com
' ''      
' ''   Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
' ''   documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
' ''   the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
' ''   to permit persons to whom the Software is furnished to do so, subject to the following conditions:
' ''    
' ''   The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
' ''   the Software.
' ''   
' ''   This Software and associated documentation files are subject to the terms and conditions of the Open Web Studio 
' ''   End User License Agreement and version 2 of the GNU General Public License.
' ''    
' ''   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
' ''   TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
' ''   THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
' ''   CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
' ''   DEALINGS IN THE SOFTWARE.
' ''</LICENSE>
'Namespace DataAccess
'    Public Class AjaxMsgParams
'        Public configurationId As Guid
'        Public pageId As String
'        Public siteId As String
'        Public moduleId As String
'        Public pageModuleId As String
'        Public Source As String
'        Public ResourceKey As String
'        Public ResourceFile As String
'        Public Sub New()
'        End Sub
'        Public Sub New(ByVal configId As Guid, ByVal srcModuleId As String, ByVal srcPageId As String, ByVal srcPageModuleId As String, ByVal srcSource As String, ByVal srcResourceKey As String, ByVal srcResourceFile As String)
'            configurationId = configId
'            moduleId = srcModuleId
'            pageId = srcPageId
'            pageModuleId = srcPageModuleId
'            Source = srcSource
'            ResourceKey = srcResourceKey
'            ResourceFile = srcResourceFile
'        End Sub
'        Public Overrides Function toString() As String
'            Dim configId As String
'            If Not configurationId = Guid.Empty Then
'                configId = "," & UI.OpenControlBase.qConfigurationID & ":" & configurationId.ToString
'            Else
'                configId = ""

'            End If
'            Dim value As String = "s:1x," & UI.OpenControlBase.qModuleId & ":" & moduleId & "," & UI.OpenControlBase.qPageModuleId & ":" & pageModuleId & "," & UI.OpenControlBase.qPageId & ":" & pageId & configId & "," & UI.OpenControlBase.qSource & ":" & Source
'            If Not ResourceKey Is Nothing AndAlso ResourceKey.Length > 0 Then
'                value &= "," & UI.OpenControlBase.qResourceKey & ":" & ResourceKey
'            End If
'            If Not ResourceFile Is Nothing Then
'                value &= "," & UI.OpenControlBase.qResourceFile & ":" & ResourceFile
'            End If
'            Return value
'        End Function
'    End Class
'End Namespace