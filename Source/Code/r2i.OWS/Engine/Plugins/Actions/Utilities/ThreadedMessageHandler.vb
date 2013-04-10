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
Imports System.Collections.Generic

Namespace r2i.OWS.Actions.Utilities
    Public Class ThreadedMessageHandler
        Private Shared _mtx_ As New System.Threading.Mutex
        Private Shared ThreadedMessages As SortedList(Of String, ThreadedMessageActionProcess)
        Private Shared FinalStatus As SortedList(Of String, String)
        Private Shared FinalErrors As SortedList(Of String, Integer)
        Private Shared FinalPercent As SortedList(Of String, Integer)
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
        Public Shared Sub ResetProcess(ByVal Name As String, ByVal SessionID As String)
            If GetMutex() Then
                Try
                    If FinalPercent Is Nothing Then
                        FinalPercent = New SortedList(Of String, Integer)
                    End If
                    If FinalStatus Is Nothing Then
                        FinalStatus = New SortedList(Of String, String)
                    End If
                    If FinalErrors Is Nothing Then
                        FinalErrors = New SortedList(Of String, Integer)
                    End If
                    If FinalPercent.ContainsKey(SessionID & ":::" & Name) Then
                        FinalPercent.Remove(SessionID & ":::" & Name)
                    End If
                    If FinalStatus.ContainsKey(SessionID & ":::" & Name) Then
                        FinalStatus.Remove(SessionID & ":::" & Name)
                    End If
                    If FinalErrors.ContainsKey(SessionID & ":::" & Name) Then
                        FinalErrors.Remove(SessionID & ":::" & Name)
                    End If
                    If ThreadedMessages.ContainsKey(SessionID & ":::" & Name) Then
                        ThreadedMessages.Remove(SessionID & ":::" & Name)
                    End If
                Catch ex As Exception
                Finally
                    ReleaseMutex()
                End Try
            End If
        End Sub
        Public Shared Function GetProcess_Complete(ByVal Name As String, ByVal SessionID As String) As Boolean
            Dim returnValue As Boolean = False
            If GetMutex() Then
                Try
                    If ThreadedMessages Is Nothing Then
                        ThreadedMessages = New SortedList(Of String, ThreadedMessageActionProcess)
                    End If
                    If FinalPercent Is Nothing Then
                        FinalPercent = New SortedList(Of String, Integer)
                    End If
                    If Not ThreadedMessages.ContainsKey(SessionID & ":::" & Name) Then
                        'CHECK FOR FINAL CHECK
                        If FinalPercent.ContainsKey(SessionID & ":::" & Name) Or FinalStatus.ContainsKey(SessionID & ":::" & Name) Then
                            returnValue = True
                        End If
                    End If
                Catch ex As Exception
                Finally
                    ReleaseMutex()
                End Try
            End If
            Return returnValue
        End Function
        Public Shared Function GetProcess_Percentage(ByVal Name As String, ByVal SessionID As String) As Integer
            Dim returnValue As Integer = 0
            If GetMutex() Then
                Try
                    If ThreadedMessages Is Nothing Then
                        ThreadedMessages = New SortedList(Of String, ThreadedMessageActionProcess)
                    End If
                    If FinalPercent Is Nothing Then
                        FinalPercent = New SortedList(Of String, Integer)
                    End If
                    If ThreadedMessages.ContainsKey(SessionID & ":::" & Name) Then
                        Dim tmap As ThreadedMessageActionProcess = ThreadedMessages(SessionID & ":::" & Name)
                        If Not tmap Is Nothing Then
                            returnValue = tmap.Percentage
                        End If
                    Else
                        'CHECK FOR FINAL CHECK
                        If FinalPercent.ContainsKey(SessionID & ":::" & Name) Then
                            returnValue = FinalPercent(SessionID & ":::" & Name)
                        End If
                    End If
                Catch ex As Exception
                Finally
                    ReleaseMutex()
                End Try
            End If
            Return returnValue
        End Function
        Public Shared Function GetProcess_Status(ByVal Name As String, ByVal SessionID As String) As String
            Dim returnValue As String = ""
            If GetMutex() Then
                Try
                    If ThreadedMessages Is Nothing Then
                        ThreadedMessages = New SortedList(Of String, ThreadedMessageActionProcess)
                    End If
                    If FinalStatus Is Nothing Then
                        FinalStatus = New SortedList(Of String, String)
                    End If
                    If ThreadedMessages.ContainsKey(SessionID & ":::" & Name) Then
                        Dim tmap As ThreadedMessageActionProcess = ThreadedMessages(SessionID & ":::" & Name)
                        If Not tmap Is Nothing Then
                            returnValue = tmap.Status
                        End If
                    Else
                        'CHECK FOR FINAL CHECK
                        If FinalStatus.ContainsKey(SessionID & ":::" & Name) Then
                            returnValue = FinalStatus(SessionID & ":::" & Name)
                        End If
                    End If
                Catch ex As Exception
                Finally
                    ReleaseMutex()
                End Try
            End If
            Return returnValue
        End Function
        Public Shared Function GetProcess_Errors(ByVal Name As String, ByVal SessionID As String) As Integer
            Dim returnValue As Integer = 0
            If GetMutex() Then
                Try
                    If ThreadedMessages Is Nothing Then
                        ThreadedMessages = New SortedList(Of String, ThreadedMessageActionProcess)
                    End If
                    If FinalErrors Is Nothing Then
                        FinalErrors = New SortedList(Of String, Integer)
                    End If
                    If ThreadedMessages.ContainsKey(SessionID & ":::" & Name) Then
                        Dim tmap As ThreadedMessageActionProcess = ThreadedMessages(SessionID & ":::" & Name)
                        If Not tmap Is Nothing Then
                            returnValue = tmap.Errors
                        End If
                    Else
                        'CHECK FOR FINAL CHECK
                        If FinalErrors.ContainsKey(SessionID & ":::" & Name) Then
                            returnValue = FinalErrors(SessionID & ":::" & Name)
                        End If
                    End If
                Catch ex As Exception
                Finally
                    ReleaseMutex()
                End Try
            End If
            Return returnValue
        End Function
        Public Shared Function StartProcess(ByVal ThreadedProcess As ThreadedMessageActionProcess, ByVal Name As String, ByVal SessionID As String, ByVal messageThread As System.Threading.Thread) As Object
            Dim returnValue As Integer = 0
            If GetMutex() Then
                Try
                    If ThreadedMessages Is Nothing Then
                        ThreadedMessages = New SortedList(Of String, ThreadedMessageActionProcess)
                    End If
                    If FinalPercent Is Nothing Then
                        FinalPercent = New SortedList(Of String, Integer)
                    End If
                    If FinalStatus Is Nothing Then
                        FinalStatus = New SortedList(Of String, String)
                    End If
                    If FinalErrors Is Nothing Then
                        FinalErrors = New SortedList(Of String, Integer)
                    End If
                    If Not ThreadedMessages.ContainsKey(SessionID & ":::" & Name) Then
                        messageThread.Name = SessionID & ":::" & Name
                        ThreadedProcess.Name = Name
                        ThreadedProcess.SessionID = SessionID
                        ThreadedMessages.Add(SessionID & ":::" & Name, ThreadedProcess)
                        AddHandler ThreadedProcess.onCompleted, AddressOf onMessageCompleted

                        If FinalPercent.ContainsKey(SessionID & ":::" & Name) Then
                            FinalPercent.Remove(SessionID & ":::" & Name)
                        End If
                        If FinalStatus.ContainsKey(SessionID & ":::" & Name) Then
                            FinalStatus.Remove(SessionID & ":::" & Name)
                        End If
                        If FinalErrors.ContainsKey(SessionID & ":::" & Name) Then
                            FinalErrors.Remove(SessionID & ":::" & Name)
                        End If
                    Else
                        'ROMAIN: 09/19/07
                        'TODO: CHANGE EXCEPTIONS
                        'DotNetNuke.Services.Exceptions.LogException(New Exception("The ListX Process (" & SessionID & ":::" & Name & ") already exists. Please check your configuration and be more unique with your process naming conventions."))
                    End If
                Catch ex As Exception
                Finally
                    ReleaseMutex()
                End Try
            End If
            Return returnValue
        End Function
        Private Shared Sub onMessageCompleted(ByVal obj As Object)
            If GetMutex() Then
                Try
                    If FinalPercent Is Nothing Then
                        FinalPercent = New SortedList(Of String, Integer)
                    End If
                    If FinalStatus Is Nothing Then
                        FinalStatus = New SortedList(Of String, String)
                    End If
                    Dim tmap As ThreadedMessageActionProcess = CType(obj, ThreadedMessageActionProcess)
                    Dim name As String = Nothing
                    Dim sessionid As String = Nothing
                    Try
                        name = tmap.Name
                        sessionid = tmap.SessionID

                        If Not FinalStatus.ContainsKey(sessionid & ":::" & name) Then
                            FinalStatus.Add(sessionid & ":::" & name, tmap.Status)
                        Else
                            FinalStatus(sessionid & ":::" & name) = tmap.Status
                        End If
                        If Not FinalPercent.ContainsKey(sessionid & ":::" & name) Then
                            FinalPercent.Add(sessionid & ":::" & name, tmap.Percentage)
                        Else
                            FinalPercent(sessionid & ":::" & name) = tmap.Percentage
                        End If
                        If Not FinalErrors.ContainsKey(sessionid & ":::" & name) Then
                            FinalErrors.Add(sessionid & ":::" & name, tmap.Errors)
                        Else
                            FinalErrors(sessionid & ":::" & name) = tmap.Errors
                        End If
                        If Not tmap.SharedDS Is Nothing Then
                            tmap.SharedDS.Clear()
                        End If
                    Catch ex As Exception

                    End Try
                    tmap.SharedDS = Nothing
                    tmap.Source = Nothing
                    tmap.ThreadAction = Nothing
                    tmap = Nothing
                    obj = Nothing
                    Try
                        ThreadedMessages.Remove(sessionid & ":::" & name)
                    Catch ex As Exception
                    End Try
                Catch ex As Exception
                Finally
                    ReleaseMutex()
                End Try
            End If
        End Sub
    End Class
End Namespace

