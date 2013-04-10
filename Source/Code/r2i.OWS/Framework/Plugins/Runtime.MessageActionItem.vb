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
Imports System.Xml.Serialization
Imports System.Collections.Generic
Imports System.Net.Mail
Imports System.Text
Imports r2i.OWS.Framework
Imports r2i.OWS.Framework.Entities
Imports r2i.OWS.Framework.DataAccess
Imports r2i.OWS.Framework.Utilities
Imports r2i.OWS.Framework.Utilities.JSON
Imports r2i.OWS.Newtonsoft.Json

Namespace r2i.OWS.Framework.Plugins.Actions
    Public Class MessageActionItem
        Private m_Parameters As SerializableDictionary(Of String, Object)
        Private m_index As Integer
        Private m_actionType As String
        Private m_actionInformation As String
        Private m_level As Integer = 0
        Private m_DELETE As Boolean
        Private m_ignore As Boolean
        Private m_Children As New List(Of MessageActionItem)
        Private m_parentAction As MessageActionItem
        Private m_actionStatus As ActionStatusType = ActionStatusType.NotExecuted
        Private m_allowedThreadName As String


        Public Property Parameters() As SerializableDictionary(Of String, Object)
            Get
                Return m_Parameters
            End Get
            Set(ByVal value As SerializableDictionary(Of String, Object))
                m_Parameters = value
            End Set
        End Property
        Public Property Index() As Integer
            Get
                Return m_index
            End Get
            Set(ByVal value As Integer)
                m_index = value
            End Set
        End Property
        Public Property ActionType() As String
            Get
                Return m_actionType
            End Get
            Set(ByVal value As String)
                m_actionType = value
            End Set
        End Property
        Public Property ActionInformation() As String
            Get
                Return m_actionInformation
            End Get
            Set(ByVal value As String)
                m_actionInformation = value
            End Set
        End Property
        Public Property Level() As Integer
            Get
                Return m_level
            End Get
            Set(ByVal value As Integer)
                m_level = value
            End Set
        End Property
        Public Property ChildActions() As List(Of MessageActionItem)
            Get
                Return m_Children
            End Get
            Set(ByVal value As List(Of MessageActionItem))
                m_Children = value
            End Set
        End Property
        Public Property Ignore() As Boolean
            Get
                Return m_ignore
            End Get
            Set(ByVal value As Boolean)
                m_ignore = value
            End Set
        End Property

        <System.Xml.Serialization.XmlIgnore(), JsonIgnore()> _
        Public Property ParentAction() As MessageActionItem
            Get
                Return m_parentAction
            End Get
            Set(ByVal value As MessageActionItem)
                m_parentAction = value
            End Set
        End Property

        Public Property AllowedThreadName() As String
            Get
                Return m_allowedThreadName
            End Get
            Set(ByVal value As String)
                m_allowedThreadName = value
            End Set
        End Property
        'Public Property __DELETE() As Boolean
        '    Get
        '        Return m_DELETE
        '    End Get
        '    Set(ByVal value As Boolean)
        '        m_DELETE = value
        '    End Set
        'End Property

        Public Shared Function GetMessageActionItemFromJson(ByVal propertiesList As Dictionary(Of String, Object)) As MessageActionItem
            Dim ma As New MessageActionItem
            Dim sValue As String

            Try
                sValue = Utilities.Utility.GetDictionaryValue(propertiesList, "Index")
                If sValue <> "" Then Int32.TryParse(sValue, ma.Index)
                sValue = Utilities.Utility.GetDictionaryValue(propertiesList, "Ignore")
                If sValue <> "" Then Boolean.TryParse(sValue, ma.Ignore)
                sValue = Utilities.Utility.GetDictionaryValue(propertiesList, "Level")
                If sValue <> "" Then Int32.TryParse(sValue, ma.Level)
                sValue = Utilities.Utility.GetDictionaryValue(propertiesList, "ActionType")
                If sValue <> "" Then ma.ActionType = sValue
                sValue = Utilities.Utility.GetDictionaryValue(propertiesList, "AllowedThreadName")
                If sValue <> "" Then ma.AllowedThreadName = sValue
                sValue = Utilities.Utility.GetDictionaryValue(propertiesList, "ActionInformation")
                If sValue <> "" Then ma.ActionInformation = sValue
                'sValue = Utility.GetDictionaryValue(propertiesList, "ActionStatus")
                'If sValue <> "" Then ma.ActionStatus = [Enum].Parse(GetType(ActionStatusType), sValue, True)
                'sValue = Utility.GetDictionaryValue(propertiesList, "__DELETE")
                'If sValue <> "" Then Boolean.TryParse(sValue, ma.__DELETE)
                If propertiesList.ContainsKey("Parameters") Then
                    Dim dictParms As Dictionary(Of String, Object) = CType(propertiesList("Parameters"), Dictionary(Of String, Object))

                    ma.Parameters = New SerializableDictionary(Of String, Object)
                    For Each sKey As String In dictParms.Keys
                        ma.Parameters.Add(sKey, dictParms.Item(sKey))
                    Next
                End If
                If propertiesList.ContainsKey("ChildActions") Then
                    Dim children As JavaScriptArray = CType(propertiesList("ChildActions"), JavaScriptArray)

                    ma.ChildActions = New List(Of MessageActionItem)
                    For Each dictChild As Dictionary(Of String, Object) In children
                        ma.ChildActions.Add(MessageActionItem.GetMessageActionItemFromJson(dictChild))
                    Next
                End If
                If propertiesList.ContainsKey("ParentAction") Then
                    ma.ParentAction = MessageActionItem.GetMessageActionItemFromJson(propertiesList.Item("ParentAction"))
                End If
            Catch ex As Exception
                ma = Nothing
            End Try

            Return ma
        End Function

        Public Enum ActionStatusType
            DontExecute = -1
            NotExecuted = 0
            DoExecute = 1
        End Enum

        Public Sub Repair(ByVal Version As Integer)
            If Version < 16 Then
                Select Case ActionType
                    Case "Message", "Condition-If", "Condition-ElseIf", "Action-Assignment", "Action-Redirect", "Action-Output"
                        'NON
                        If Not ActionInformation Is Nothing Then
                            Dim strs() As String = ActionInformation.Split("|")
                            Dim splitter As New Utilities.SmartSplitter
                            Dim str As String
                            For Each str In strs
                                splitter.Add(str)
                            Next
                            ActionInformation = splitter.Blend
                        End If
                    Case "Action-Execute"
                        If Not ActionInformation Is Nothing Then
                            Dim strs() As String = ActionInformation.Split("|")
                            Dim splitter As New Utilities.SmartSplitter
                            Dim str As String
                            If strs.Length = 1 Then
                                splitter.Add("")
                            End If
                            If strs.Length = 0 Then
                                splitter.Add("")
                                splitter.Add("")
                            End If

                            For Each str In strs
                                splitter.Add(str)
                            Next
                            ActionInformation = splitter.Blend
                        End If
                        'Case "Action-Email", "Action-Input"
                End Select
            End If
        End Sub

        'GLOBAL SHARED FUNCTIONS
        Public Shared Function FindAll(ByVal ActionsList As List(Of MessageActionItem), ByVal ActionType As String, Optional ByVal ParameterName As String = Nothing, Optional ByVal ParameterValue As String = Nothing) As List(Of MessageActionItem)
            Dim maAll As New List(Of MessageActionItem)

            If Not ActionsList Is Nothing Then
                _FindAll(maAll, ActionsList, ActionType, ParameterName, ParameterValue)
            Else
                maAll = Nothing
            End If

            Return maAll
        End Function
        Private Shared Sub _FindAll(ByRef Found As List(Of MessageActionItem), ByVal ActionsList As List(Of MessageActionItem), ByVal ActionType As String, Optional ByVal ParameterName As String = Nothing, Optional ByVal ParameterValue As String = Nothing)
            For Each mi As MessageActionItem In ActionsList
                If Not ParameterName Is Nothing AndAlso Not ParameterValue Is Nothing AndAlso Found.Count = 1 Then
                    Exit Sub
                End If
                If mi.ActionType = ActionType Then
                    If Not ParameterName Is Nothing Then
                        If mi.Parameters.ContainsKey(ParameterName) Then
                            If Not ParameterValue Is Nothing Then
                                If mi.Parameters(ParameterName) = ParameterValue Then
                                    Found.Add(mi)
                                    Exit Sub
                                End If
                            End If
                        End If
                    Else
                        Found.Add(mi)
                    End If
                End If
                If Not mi.ChildActions Is Nothing AndAlso mi.ChildActions.Count > 0 Then
                    _FindAll(Found, mi.ChildActions, ActionType, ParameterName, ParameterValue)
                End If
            Next
        End Sub
    End Class
End Namespace