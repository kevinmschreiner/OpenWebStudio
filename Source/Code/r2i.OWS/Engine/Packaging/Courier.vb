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
Namespace r2i.OWS.Packaging
    Public Class Courier
        Private Shared _mtx_ As New System.Threading.Mutex
        'ROMAIN: Generic replacement - 08/22/2007
        'Private Shared CurrentPackages As SortedList
        Private Shared CurrentPackages As SortedList(Of String, Delivery)
        Private Shared Function GetMutex() As Boolean
            Try
                _mtx_.WaitOne()
                Return True
            Catch ex As Exception
                Return False
            End Try
        End Function
        Private Shared Function ReleaseMutex() As Boolean
            Try
                _mtx_.ReleaseMutex()
                Return True
            Catch ex As Exception
                Return False
            End Try
        End Function
        Public Shared Sub Deliver(ByVal PackageID As Integer, ByVal RenderingEngine As Engine, ByVal configurationId As Guid, ByVal ModuleId As String, ByVal pageId As String, ByVal ClientID As String, ByVal PortalId As String, ByVal BasePath As String, ByVal AdminRoleName As String, ByVal AdminRoleID As String, ByVal WebPath As String, ByVal PackagePath As String)
            'Dim inst As New xList.Packaging.Delivery(rEngine, Me.ModuleId, Me.PortalSettings.ActiveTab.TabID, Me.PortalId, BasePath, Me.PortalSettings)
            'inst.SourcePackage = pkg
            Dim dlv As New Packaging.Delivery(PackageID, RenderingEngine, configurationId, ModuleId, pageId, ClientID, PortalId, BasePath, AdminRoleName, AdminRoleID, WebPath, PackagePath)
            Deliver(dlv)
        End Sub
        Public Shared Sub Deliver(ByRef Dlv As Delivery)
            Dim gotm As Boolean = False
            Try
                If (GetMutex()) Then
                    gotm = True
                    If CurrentPackages Is Nothing Then
                        'ROMAIN: Generic replacement - 08/22/2007
                        'CurrentPackages = New SortedList
                        CurrentPackages = New SortedList(Of String, Delivery)
                    End If
                    If CurrentPackages.ContainsKey(Dlv.SourcePackage.ToString) Then
                        CurrentPackages.Remove(Dlv.SourcePackage.ToString)
                    End If
                    If Not CurrentPackages.ContainsKey(Dlv.SourcePackage.ToString) Then
                        CurrentPackages.Add(Dlv.SourcePackage.ToString, Dlv)
                        Dlv.Deliver(True)
                    End If
                End If
                ReleaseMutex()
            Catch ex As Exception
                If gotm Then
                    Try
                        ReleaseMutex()
                    Catch ex2 As Exception

                    End Try
                End If
                Throw New Exception(ex.ToString)
            End Try
        End Sub
        Public Shared Sub Delivered(ByRef Identifier As String)
            Dim gotm As Boolean = False
            Try
                If GetMutex() Then
                    gotm = True
                    If CurrentPackages Is Nothing Then
                        'ROMAIN: Generic replacement - 08/22/2007
                        'CurrentPackages = New SortedList
                        CurrentPackages = New SortedList(Of String, Delivery)
                    End If
                    If CurrentPackages.ContainsKey(Identifier) Then
                        CurrentPackages.Remove(Identifier)
                    End If
                End If
                ReleaseMutex()
            Catch ex As Exception
                If gotm Then
                    Try
                        ReleaseMutex()
                    Catch ex2 As Exception

                    End Try
                End If
            End Try
        End Sub
    End Class
End Namespace
