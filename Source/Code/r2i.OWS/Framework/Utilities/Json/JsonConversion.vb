'<LICENSE>
'   Open Web Studio - http://www.openwebstudio.com
'   Copyright (c) 2006-2008
'   by R2 Integrated Inc. ( http://www.r2integrated.com )
'   
'   Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
'   documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
'   the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
'   to permit persons to whom the Software is furnished to do so, subject to the following conditions:
'   
'   The above copyright notice and this permission notice shall be included in all copies or substantial portions 
'   of the Software.
'   
'   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
'   TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
'   THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
'   CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
'   DEALINGS IN THE SOFTWARE.
'</LICENSE>
Imports System.Xml
Imports r2i.OWS.Framework.Plugins.Actions
Imports r2i.OWS.Framework.Utilities.Compatibility
Imports r2i.OWS.Framework.Utilities
Imports r2i.OWS.Newtonsoft.Json
Imports System.Collections.Generic
Imports r2i.OWS
Imports System.Xml.Serialization
Imports System.IO
Imports System.Reflection
Imports System.ComponentModel



Namespace r2i.OWS.Framework.Utilities.JSON
    Public Class JsonConversion
        Private Const PROPERTIESLIST_COLUMN_MAPPING As String = "Position,Name,Target,Type,DefaultValue,NullValue,Format,Index,FileType,StartColumn,EndColumn"
        Private Const PROPERTIESLIST_ACTION_INPUT As String = "URL,Querystring,Data,ContentType,Method,VariableType,VariableName,InputFormat,XPath,AuthenticationType,Domain,Username,Password,SoapAction,SoapResult"
        Private Const UNDEFINED_PARAMETER As String = "no defined parameters."


        Private Shared depth As Integer = 0
        Private m_messageActionItem_File_ColumnMappings As MessageAction_File_ColumnMappings

        Public Context As Web.HttpContext

        Public Function GetJsonStructure(ByRef currentListXConfig As String) As String
            Return GetJsonStructure(System.Guid.Empty, currentListXConfig)
        End Function
        Public Shared Function Encode(ByVal Value As String) As String
            Return Newtonsoft.Json.JavaScriptConvert.ToString(Value)
        End Function
        Public Shared Function toNameValuePairs(ByVal value As String) As Specialized.NameValueCollection
            Dim attr As New Specialized.NameValueCollection
            Dim r As New IO.StringReader(value)
            Try
                Dim jsonreader As New Newtonsoft.Json.JsonReader(r)
                Dim currentName As String = Nothing
                Dim currentValue As String = Nothing
                jsonreader.Read()
                If jsonreader.TokenType = Newtonsoft.Json.JsonToken.StartObject Then
                    While jsonreader.Read()
                        Select Case jsonreader.TokenType
                            Case Newtonsoft.Json.JsonToken.PropertyName
                                currentName = jsonreader.Value
                            Case Newtonsoft.Json.JsonToken.String
                                currentValue = jsonreader.Value
                            Case Newtonsoft.Json.JsonToken.Boolean
                                currentValue = jsonreader.Value
                            Case Newtonsoft.Json.JsonToken.Date
                                currentValue = jsonreader.Value
                            Case Newtonsoft.Json.JsonToken.Float
                                currentValue = jsonreader.Value
                            Case Newtonsoft.Json.JsonToken.Integer
                                currentValue = jsonreader.Value
                        End Select
                        If Not currentName Is Nothing AndAlso Not currentValue Is Nothing Then
                            attr.Add(currentName, currentValue)
                            currentName = Nothing
                            currentValue = Nothing
                        End If
                    End While
                End If
            Catch ex As Exception

            End Try
            Try
                r.Close()
                r.Dispose()
            Catch ex As Exception
            End Try
            r = Nothing
            Return attr
        End Function
        Public Function GetJsonStructure(ByRef configID As System.Guid, ByRef currentListXConfig As String) As String
            Dim jsonString As String
            If currentListXConfig.TrimStart().StartsWith("<") Then
                'KEVIN: 1/14/08
                'CONVERT TO Settings Instead of xListSettings
                Try
                    'TODO: REPLACE WITH XML READER NOT THIS...
                    Dim start As Integer = currentListXConfig.IndexOf("<xListSettings")
                    If start >= 0 Then
                        currentListXConfig = currentListXConfig.Remove(start, "<xListSettings".Length)
                        currentListXConfig = currentListXConfig.Insert(start, "<Settings")
                        start = currentListXConfig.LastIndexOf("</xListSettings")
                        If start >= 0 Then
                            currentListXConfig = currentListXConfig.Remove(start, "</xListSettings".Length)
                            currentListXConfig = currentListXConfig.Insert(start, "</Settings")
                        End If
                    End If
                Catch ex As Exception
                    'TODO: Exception: not correct xml format and return an exception message
                End Try

                'BOB: 10/13/2007
                'Old untyped arraylists won't deserialize as new strong-typed generics lists 
                currentListXConfig = FixGenericLists(currentListXConfig)

                'ROMAIN: 10/22/07
                ' ROMAIN: Add New propert to support the Json Notation
                currentListXConfig = AddNodeParameters(currentListXConfig)

                Dim xmlDoc As New XmlDocument()
                Try
                    xmlDoc.LoadXml(currentListXConfig)
                Catch ex As Exception
                    'TODO: Exception: not correct xml format and return an exception message
                End Try

                ' this strategy moves the nodes in the xml doc
                xmlDoc = ConvertXMLNodesToTree(xmlDoc)

                Dim xs As New XmlSerializer(GetType(Settings))
                Dim reader As New XmlNodeReader(xmlDoc.DocumentElement)

                ' now we deserialize the modified object
                Dim SettingsFromXml As Settings = DirectCast(xs.Deserialize(reader), Settings)

                'note: we could have move the nodes on the object itself.  Could be a useful update.  -ejw and rg

                RemoveSmartSplitting(SettingsFromXml)

                'SET THE VERSION TO 19
                SettingsFromXml.Version = 19

                jsonString = JavaScriptConvert.SerializeObject(DirectCast(SettingsFromXml, Object))
            Else
                'Check if the string is correct and can be deseriliazed
                Try
                    Dim SettingsFromJson As Settings = GetDeserializedListXConfiguration(currentListXConfig) ' TryCast(JavaScriptConvert.DeserializeObject(currentListXConfig, GetType(Settings)), Settings)
                    'Ensure that the configId in the database and in the Settings object match
                    If Not configID = System.Guid.Empty AndAlso SettingsFromJson.ConfigurationID <> configID.ToString Then
                        SettingsFromJson.ConfigurationID = configID.ToString
                        currentListXConfig = JavaScriptConvert.SerializeObject(SettingsFromJson)
                    End If
                Catch ex As Exception
                    'TODO: configuration stored in the database or passed through the user interface has been corrupt
                    'TODO: Implement Exception
                    'Return "Json Structure corrupted"
                    Return String.Empty
                    Exit Function
                End Try
                jsonString = currentListXConfig
            End If
            Return jsonString
        End Function

        Public Function GetDeserializedListXConfiguration(ByVal jsonConfig As String) As Settings
            Dim xls As New Settings
            Dim jso As JavaScriptObject
            Dim value As Object

            Try
                jso = JavaScriptConvert.DeserializeObject(jsonConfig)
                If Not jso Is Nothing Then
                    For Each sFieldName As String In jso.Keys
                        Dim fi As FieldInfo = xls.GetType().GetField(sFieldName)

                        If Not fi Is Nothing Then
                            value = jso.Item(sFieldName)

                            If TypeOf value Is JavaScriptArray Then
                                If sFieldName = "searchItems" Then
                                    xls.searchItems = New List(Of SearchOptionItem)
                                    For Each jsoChild As JavaScriptObject In CType(value, JavaScriptArray)
                                        xls.searchItems.Add(SearchOptionItem.GetSearchOptionItemFromJson(jsoChild))
                                    Next
                                ElseIf sFieldName = "queryItems" Then
                                    xls.queryItems = New List(Of QueryOptionItem)
                                    For Each jsoChild As JavaScriptObject In CType(value, JavaScriptArray)
                                        xls.queryItems.Add(QueryOptionItem.GetQueryOptionItemFromJson(jsoChild))
                                    Next
                                ElseIf sFieldName = "listItems" Then
                                    xls.listItems = New List(Of ListFormatItem)
                                    For Each jsoChild As JavaScriptObject In CType(value, JavaScriptArray)
                                        xls.listItems.Add(ListFormatItem.GetListFormatItemFromJson(jsoChild))
                                    Next
                                ElseIf sFieldName = "messageItems" Then
                                    xls.messageItems = New List(Of MessageActionItem)
                                    For Each jsoChild As JavaScriptObject In CType(value, JavaScriptArray)
                                        xls.messageItems.Add(MessageActionItem.GetMessageActionItemFromJson(jsoChild))
                                    Next
                                ElseIf sFieldName = "javascriptInclude" Then
                                    If (CType(value, JavaScriptArray).Count > 0) Then
                                        Dim jsarr(CType(value, JavaScriptArray).Count - 1) As String
                                        Dim jsi As Integer = 0
                                        For Each jsoChild As String In CType(value, JavaScriptArray)
                                            jsarr(jsi) = jsoChild
                                            jsi += 1
                                        Next
                                        xls.javascriptInclude = jsarr
                                    End If
                                End If
                            Else

                                Try
                                    fi.SetValue(xls, Convert.ChangeType(value, fi.FieldType))
                                Catch ex As Exception
                                End Try
                            End If
                        End If
                    Next
                End If
            Catch ex As Exception
                Return Nothing
                'TODO: IMplements Exception
            End Try

            Return xls
        End Function


        Public Function GetJsonFromDataset(ByVal ds As Dictionary(Of String, String)) As String
            Dim str As String = JavaScriptConvert.SerializeObject(ds)
            Return str
        End Function

        Public Function getSettingsValues(ByVal dicProperties As Dictionary(Of String, Object)) As Settings
            Dim xls As New Settings
            Dim mbInfos As MemberInfo() = xls.GetType().GetMembers
            Dim lstProperties As New Dictionary(Of String, String)
            For Each mbInfo As MemberInfo In mbInfos
                Select Case mbInfo.MemberType
                    Case System.Reflection.MemberTypes.Field
                        lstProperties.Add(mbInfo.Name, (DirectCast(mbInfo, FieldInfo)).GetValue(xls))
                    Case System.Reflection.MemberTypes.Property
                        lstProperties.Add(mbInfo.Name, (DirectCast(mbInfo, PropertyInfo)).GetValue(xls, Nothing))
                End Select
            Next

            Return xls
        End Function

        Private Class TreeActionItem
            Public ChildActions As XmlNode
            Public ParentItem As TreeActionItem
            Public isRoot As Boolean = False
            'Public Children As ArrayList
        End Class
        Private Function CreateCommentNode(ByVal Index As Integer, ByVal Level As Integer, ByVal xmlDoc As Xml.XmlDocument) As Xml.XmlNode
            Dim baseNode As Xml.XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "", "MessageActionItem", "")
            baseNode.InnerXml = "<Index>" & Index & "</Index><ActionType>Action-Comment</ActionType><ActionInformation>3:62;The following action or actions are disabled due to commenting</ActionInformation><Level>" & Level & "</Level>"

            Return baseNode
        End Function
        Public Function ConvertXMLNodesToTree(ByVal xmlDoc As XmlDocument) As XmlDocument
            Dim fieldNode As XmlNode = xmlDoc.SelectSingleNode("//messageItems")
            Dim childActionsRootNode As XmlNode = xmlDoc.CreateNode(XmlNodeType.Element, "", "messageItems", "")

            If fieldNode.HasChildNodes Then
                Dim levelItems As New SortedList()
                Dim rootItem As New TreeActionItem
                Dim currentLevel As Integer = -1
                Dim currentIndex As Integer = 0
                rootItem.isRoot = True
                rootItem.ChildActions = childActionsRootNode

                levelItems("level-1") = rootItem

                For Each maItem As XmlNode In fieldNode.ChildNodes
                    Dim cmaItem As XmlNode = maItem.Clone

                    currentIndex += 1
                    Dim index As Integer
                    Dim level As Integer
                    Dim TreeItemObj As New TreeActionItem

                    index = Convert.ToInt32(cmaItem.SelectSingleNode("//Index").InnerXml)
                    level = Convert.ToInt32(cmaItem.SelectSingleNode("//Level").InnerXml)


                    Dim nodeChildActions As XmlNode
                    nodeChildActions = cmaItem.SelectSingleNode("//ChildActions")
                    If nodeChildActions Is Nothing Then
                        nodeChildActions = xmlDoc.CreateNode(XmlNodeType.Element, "", "ChildActions", "")
                        cmaItem.AppendChild(nodeChildActions)
                    End If


                    TreeItemObj.ChildActions = nodeChildActions

                    While level > currentLevel + 1
                        currentLevel += 1
                        Dim ctreeActionItem As New TreeActionItem
                        Dim ctreeNodexNode As XmlNode = CreateCommentNode(currentIndex, currentLevel, xmlDoc)

                        Dim cnodeChildActions As XmlNode
                        cnodeChildActions = ctreeNodexNode.SelectSingleNode("//ChildActions")
                        If cnodeChildActions Is Nothing Then
                            cnodeChildActions = xmlDoc.CreateNode(XmlNodeType.Element, "", "ChildActions", "")
                            ctreeNodexNode.AppendChild(cnodeChildActions)
                        End If

                        ctreeActionItem.ChildActions = cnodeChildActions

                        ctreeActionItem.ParentItem = levelItems("level" & currentLevel - 1)
                        ctreeActionItem.ParentItem.ChildActions.AppendChild(ctreeNodexNode)
                        levelItems("level" & currentLevel) = ctreeActionItem

                        currentIndex += 1
                    End While

                    While level < currentLevel - 1
                        currentLevel -= 1
                        levelItems.Remove("level" & currentLevel.ToString)
                    End While

                    currentLevel = level
                    cmaItem.SelectSingleNode("//Index").InnerXml = currentIndex
                    TreeItemObj.ParentItem = levelItems("level" & currentLevel - 1)
                    TreeItemObj.ParentItem.ChildActions.AppendChild(cmaItem)
                    levelItems("level" & level.ToString) = TreeItemObj
                Next
            End If

            fieldNode.ParentNode.ReplaceChild(childActionsRootNode, fieldNode)
            Return xmlDoc

        End Function
        Public Sub RemoveSmartSplitting(ByRef xls As Settings)
            RemoveSmartSplitting(xls.messageItems)
        End Sub
        Public Sub RemoveSmartSplitting(ByRef messageItems As System.Collections.Generic.List(Of r2i.OWS.Framework.Plugins.Actions.MessageActionItem))
            For Each ma As r2i.OWS.Framework.Plugins.Actions.MessageActionItem In messageItems
                ' first act on children
                RemoveSmartSplitting(ma.ChildActions)
                ' then act on parent
                ma.Parameters = ActionInformationsList(ma)
            Next

        End Sub

        Private Function ActionInformationsList(ByRef ma As r2i.OWS.Framework.Plugins.Actions.MessageActionItem) As SerializableDictionary(Of String, Object)
            Dim valuesList As New SerializableDictionary(Of String, Object)

            Select Case ma.ActionType

                Case "Message"
                    If Not ma.ActionInformation Is Nothing Then
                        Dim strs As New SmartSplitter
                        strs.Split(ma.ActionInformation)
                        If Not strs Is Nothing AndAlso strs.Length > 0 Then
                            valuesList.Add("Type", strs(0))
                        Else
                            valuesList.Add("Type", UNDEFINED_PARAMETER)
                        End If
                        If Not strs Is Nothing AndAlso strs.Length > 1 Then
                            If Not strs(1).Length = 0 Then
                                valuesList.Add("Value", strs(1))
                            Else
                                valuesList.Add("Value", "")
                            End If
                        Else
                            valuesList.Add("Value", "")
                        End If
                    Else
                        valuesList.Add("Type", UNDEFINED_PARAMETER)
                        valuesList.Add("Value", UNDEFINED_PARAMETER)
                    End If
                Case "Action-Comment"
                    If Not ma.ActionInformation Is Nothing Then
                        Dim strs As New SmartSplitter
                        strs.Split(ma.ActionInformation)
                        If Not strs Is Nothing AndAlso strs.Length > 0 Then
                            valuesList.Add("Value", ShortenString(strs(0), 80) & "...")
                        Else
                            valuesList.Add("Value", "(No Comment Provided)")
                        End If
                    Else
                        valuesList.Add("Value", "(No Comment Provided)")
                    End If
                    'NOTE: Action-Delay Removed Useful?
                Case "Action-Redirect"
                    If Not ma.ActionInformation Is Nothing Then
                        Dim strs As New SmartSplitter
                        strs.Split(ma.ActionInformation)
                        If Not strs Is Nothing Then
                            If strs.Length > 0 Then
                                valuesList.Add("Type", strs(0))
                                If strs.Length > 1 Then
                                    valuesList.Add("PageID", "")
                                    If Not strs(0).ToUpper = "LINK" AndAlso strs.Length > 2 AndAlso Not strs(2).ToUpper = "-1" Then
                                        valuesList.Add("Link", strs(2))
                                    Else
                                        valuesList.Add("Link", strs(1))
                                    End If
                                End If
                            Else
                                valuesList.Add("Link", UNDEFINED_PARAMETER)
                            End If
                        Else
                            valuesList.Add("Type", UNDEFINED_PARAMETER)
                        End If
                    End If
                Case "Action-File"
                    If Not ma.ActionInformation Is Nothing Then
                        Dim strs As New SmartSplitter
                        strs.Split(ma.ActionInformation)
                        Dim i As Integer = 0
                        If strs.Length > 0 Then
                            Dim sourcetype As String = strs(i)
                            valuesList.Add(MessageActionsConstants.ACTIONFILE_SOURCETYPE_KEY, sourcetype)
                            i += 1
                            Select Case sourcetype
                                Case "Path"
                                    valuesList.Add(MessageActionsConstants.ACTIONFILE_SOURCE_KEY, strs(i))
                                Case "SQL"
                                    valuesList.Add(MessageActionsConstants.ACTIONFILE_SOURCEQUERY_KEY, strs(i))
                                Case "Variable"
                                    valuesList.Add(MessageActionsConstants.ACTIONFILE_SOURCEVARIABLETYPE_KEY, strs(i))
                                    i += 1
                                    valuesList.Add(MessageActionsConstants.ACTIONFILE_SOURCE_KEY, strs(i))
                            End Select
                            i += 1

                            Dim destinationType As String = strs(i)
                            valuesList.Add(MessageActionsConstants.ACTIONFILE_DESTINATIONTYPE_KEY, destinationType)
                            i += 1
                            Select Case destinationType
                                Case "Response", "EmailAttachment"
                                    valuesList.Add(MessageActionsConstants.ACTIONFILE_DESTINATIONRESPONSETYPE_KEY, strs(i))
                                    i += 1
                                Case "SQL"
                                    valuesList.Add(MessageActionsConstants.ACTIONFILE_SQLFIRSTROW_KEY, strs(i))
                                    i += 1
                                    'TODO: Get the Column mapping --> Create a specific function
                                    valuesList.Add(MessageActionsConstants.ACTIONFILE_COLUMNMAPPING_KEY, ColumnMappingList(strs(i)))
                                    i += 1
                                    valuesList.Add(MessageActionsConstants.ACTIONFILE_DESTINATIONQUERY_KEY, strs(i))
                                    i += 1
                                    valuesList.Add(MessageActionsConstants.ACTIONFILE_RUNASPROCESS_KEY, strs(i))
                                    i += 1
                                    valuesList.Add(MessageActionsConstants.ACTIONFILE_PROCESSNAME_KEY, strs(i))
                            End Select
                            If Not destinationType = "SQL" Then
                                If Not sourcetype = "Response" Then
                                    valuesList.Add(MessageActionsConstants.ACTIONFILE_DESTINATION_KEY, strs(i))
                                End If
                                i += 1

                                Dim sTransformType As String = strs(i)
                                valuesList.Add(MessageActionsConstants.ACTIONFILE_TRANSFORMTYPE_KEY, sTransformType)
                                i += 1

                                Select Case sTransformType
                                    Case "Image"
                                        valuesList.Add(MessageActionsConstants.ACTIONFILE_IMAGEWIDTH_KEY, strs(i))
                                        i += 1
                                        valuesList.Add(MessageActionsConstants.ACTIONFILE_IMAGEWIDTHTYPE_KEY, strs(i))
                                        i += 1
                                        valuesList.Add(MessageActionsConstants.ACTIONFILE_IMAGEHEIGHT_KEY, strs(i))
                                        i += 1
                                        valuesList.Add(MessageActionsConstants.ACTIONFILE_IMAGEHEIGHTYPE_KEY, strs(i))
                                        i += 1
                                        valuesList.Add(MessageActionsConstants.ACTIONFILE_IMAGEQUALITY_KEY, strs(i))
                                    Case "XML"
                                        valuesList.Add(MessageActionsConstants.ACTIONFILE_XMLREADPATH_KEY, strs(i))
                                        i += 1
                                        valuesList.Add(MessageActionsConstants.ACTIONFILE_XMLWRITEPATH_KEY, strs(i))
                                    Case "File"
                                        Dim fp As String = strs(i)
                                        i += 1
                                        If fp.Length > 0 Then
                                            valuesList.Add(MessageActionsConstants.ACTIONFILE_FILETASK_KEY, fp)
                                        End If
                                End Select
                            Else
                                'valuesList.Add("Destination", "Undefined")
                            End If
                        End If
                    End If
                Case "Action-Log"
                    'TODO: CHECK THE ACTION-LOG
                    If Not ma.ActionInformation Is Nothing Then
                        Dim strs As New SmartSplitter
                        strs.Split(ma.ActionInformation)
                        If Not strs Is Nothing AndAlso strs.Length > 0 Then
                            If strs.Length > 1 Then
                                valuesList.Add("Value", strs(1))
                            End If
                            valuesList.Add("Name", strs(0))
                        Else
                            valuesList.Add("Name", "")
                        End If
                    Else
                        valuesList.Add("Name", "")
                    End If
                Case "Action-Assignment"
                    If Not ma.ActionInformation Is Nothing Then
                        Dim strs As New SmartSplitter
                        strs.Split(ma.ActionInformation)
                        If Not strs Is Nothing Then
                            If strs.Length > 0 Then
                                valuesList.Add("Type", strs(0))
                            Else
                                valuesList.Add("Type", "")
                            End If
                            If strs.Length > 1 Then
                                valuesList.Add("Name", strs(1))
                            Else
                                valuesList.Add("Name", "")
                            End If
                            If strs.Length > 2 Then
                                valuesList.Add("Value", strs(2))
                            Else
                                valuesList.Add("Value", "")
                            End If
                            If strs.Length > 3 Then
                                valuesList.Add("SkipProcessing", strs(3))
                            Else
                                valuesList.Add("SkipProcessing", "False")
                            End If
                            If strs.Length > 4 Then
                                valuesList.Add("AssignmentType", strs(4))
                            Else
                                valuesList.Add("AssignmentType", "0")
                            End If
                        End If
                    Else
                        valuesList.Add("Type", "")
                    End If
                Case "Action-Execute"
                    If Not ma.ActionInformation Is Nothing AndAlso ma.ActionInformation.Length > 0 Then
                        Dim strs As New SmartSplitter
                        strs.Split(ma.ActionInformation)
                        valuesList.Add("Name", strs(0))
                        valuesList.Add("Query", strs(1))
                        If strs(2).Length > 0 AndAlso strs(2).Length < 6 Then
                            valuesList.Add("IsProcess", strs(2))
                        Else
                            valuesList.Add("IsProcess", "False")
                        End If
                        valuesList.Add("Connection", strs(3))
                    Else
                        valuesList.Add("Name", "")
                    End If
                Case "Action-Email"
                    Dim splitter As New SmartSplitter
                    splitter.Split(ma.ActionInformation)
                    If splitter.Length >= 7 Then
                        Dim hasTargets As Boolean = False
                        If Not splitter.Item(0) Is Nothing AndAlso splitter.Item(0).ToString.Length > 0 Then
                            valuesList.Add("From", splitter.Item(0))
                        Else
                            valuesList.Add("From", "Not Assigned, no mail will be sent. ")
                        End If
                        If Not splitter.Item(1) Is Nothing AndAlso splitter.Item(1).ToString().Length > 0 Then
                            hasTargets = True
                            valuesList.Add("To", splitter.Item(1))
                        End If
                        If Not splitter.Item(2) Is Nothing AndAlso splitter.Item(2).ToString.Length > 0 Then
                            hasTargets = True
                            valuesList.Add("Cc", splitter.Item(2))
                        End If
                        If Not splitter.Item(3) Is Nothing AndAlso splitter.Item(3).ToString.Length > 0 Then
                            hasTargets = True
                            valuesList.Add("Bcc", splitter.Item(3))
                        End If
                        If Not splitter.Item(4) Is Nothing AndAlso splitter.Item(4).ToString.Length > 0 Then
                            valuesList.Add("Format", splitter.Item(4))
                        End If
                        If Not hasTargets Then
                            valuesList.Add("To", "No target email addresses are provided, no mail will be sent. ")
                            valuesList.Add("Cc", "No target email addresses are provided, no mail will be sent. ")
                            valuesList.Add("Bcc", "No target email addresses are provided, no mail will be sent. ")
                        End If
                        If Not splitter.Item(5) Is Nothing AndAlso splitter.Item(5).ToString.Length > 0 Then
                            valuesList.Add("Subject", splitter.Item(5))
                        End If
                        If Not splitter.Item(6) Is Nothing AndAlso splitter.Item(6).ToString.Length > 0 Then
                            valuesList.Add("Body", splitter.Item(6))
                        End If
                        If Not splitter.Item(7) Is Nothing AndAlso splitter.Item(7).ToString.Length > 0 Then
                            valuesList.Add("ResultVariableType", splitter.Item(7))
                        Else
                            valuesList.Add("ResultVariableType", "")
                        End If
                        If Not splitter.Item(8) Is Nothing AndAlso splitter.Item(8).ToString.Length > 0 Then
                            valuesList.Add("ResultVariableName", splitter.Item(8))
                        Else
                            valuesList.Add("ResultVariableName", "")
                        End If

                    End If
                Case "Action-Output"
                    If Not ma.ActionInformation Is Nothing Then
                        Dim strs As New SmartSplitter
                        strs.Split(ma.ActionInformation)
                        If Not strs Is Nothing AndAlso strs.Length >= 1 Then
                            valuesList.Add("OutputType", strs(0))
                            Select Case LCase(strs(0))
                                Case "excel", "complete excel", "word", "complete word"
                                    valuesList.Add("Filename", strs(1))
                                Case "delimited", "complete delimited"
                                    valuesList.Add("Filename", strs(1))
                                    valuesList.Add("Delimiter", strs(2))
                                Case "report"
                                    valuesList.Add("Filename", strs(1))
                                    valuesList.Add("Delimiter", strs(2))
                            End Select
                        End If
                    Else
                        valuesList.Add("OutputType", UNDEFINED_PARAMETER)
                    End If
                Case "Action-Input"
                    If Not ma.ActionInformation Is Nothing Then
                        Dim strs As New SmartSplitter
                        strs.Split(ma.ActionInformation)
                        Dim KeyList As New List(Of String)(GetListElements(PROPERTIESLIST_ACTION_INPUT))
                        Dim kList As New List(Of String)(GetListElements(PROPERTIESLIST_ACTION_INPUT))
                        If Not strs Is Nothing Then
                            Dim i As Integer
                            For i = 0 To strs.Length - 1 Step 1
                                If KeyList(i) = "VariableType" Then strs(i) = strs(i).Replace("<", "&lt;").Replace(">", "&gt;")
                                valuesList.Add(KeyList(i), strs(i))
                                kList.Remove(KeyList(i))
                            Next
                            If kList.Count > 0 Then
                                For Each name As String In kList
                                    valuesList.Add(name, "")
                                Next
                            End If
                        Else
                            valuesList.Add("Type", "")
                        End If
                    End If
                Case "Condition-If", "Condition-ElseIf"
                    If Not ma.ActionInformation Is Nothing Then
                        Dim strs As New SmartSplitter
                        strs.Split(ma.ActionInformation)
                        If Not strs Is Nothing Then
                            If strs.Length = 4 Then
                                valuesList.Add("LeftCondition", strs(0))
                                valuesList.Add("Operator", strs(1))
                                valuesList.Add("RightCondition", strs(2))
                                valuesList.Add("IsAdvanced", strs(3))
                            ElseIf strs.Length = 3 Then
                                valuesList.Add("LeftCondition", strs(0))
                                valuesList.Add("Operator", strs(1))
                                valuesList.Add("RightCondition", strs(2))
                                valuesList.Add("IsAdvanced", "False")
                            Else
                                valuesList.Add("LeftCondition", "")
                                valuesList.Add("RightCondition", "")
                                valuesList.Add("Operator", "")
                                valuesList.Add("IsAdvanced", "False")
                            End If
                        End If
                    Else
                        valuesList.Add("LeftCondition", "")
                    End If
                Case "Condition-Else"
                    Exit Select
                Case Else
                    valuesList.Add("Type", "This Action has not been configured.")
            End Select
            Return valuesList
        End Function

        Private Function ColumnMappingList(ByVal colMap As String) As List(Of SerializableDictionary(Of String, String))
            LoadMCFList(colMap)
            Dim colMapList As New List(Of SerializableDictionary(Of String, String))

            Dim i As Integer
            For i = 0 To m_messageActionItem_File_ColumnMappings.ColumnMappings.Count - 1
                Dim columnMappingsDic As New SerializableDictionary(Of String, String)
                Dim maFileColumnMapItem As MessageAction_File_ColumnMappings.MessageAction_File_ColumnMappingItem
                maFileColumnMapItem = m_messageActionItem_File_ColumnMappings.ColumnMappings(i)
                columnMappingsDic = ColumnMappings(maFileColumnMapItem)
                colMapList.Add(columnMappingsDic)
            Next
            'jsonString = JavaScriptConvert.SerializeObject(DirectCast(SettingsFromXml, Object))

            'Dim jsonStr As String = JavaScriptConvert.SerializeObject(colMapList)
            Return colMapList
        End Function
        Private Function ColumnMappings(ByVal maFileColumnMapItem As MessageAction_File_ColumnMappings.MessageAction_File_ColumnMappingItem) As SerializableDictionary(Of String, String)
            Dim columnMappingsDic As New SerializableDictionary(Of String, String)
            Dim parametersList As New Dictionary(Of String, String)(GetListElementsDictionary(PROPERTIESLIST_COLUMN_MAPPING))
            Dim lstPropertiesMaFileColumnMapItem As Dictionary(Of String, String) = ListProperties(maFileColumnMapItem)
            For Each kvp As KeyValuePair(Of String, String) In lstPropertiesMaFileColumnMapItem
                If parametersList.ContainsKey(kvp.Key) Then
                    columnMappingsDic.Add(kvp.Key, kvp.Value)
                End If
            Next
            Return columnMappingsDic
        End Function

        'Private Function ColumnMappings(ByVal maFileColumnMapItem As MessageAction_File_ColumnMappings.MessageAction_File_ColumnMappingItem) As String
        '    Dim columnMappingsElt As String = ""
        '    Dim parametersList As New Dictionary(Of String, String)(GetListElementsDictionary(PROPERTIESLIST_COLUMN_MAPPING))
        '    Dim lstPropertiesMaFileColumnMapItem As Dictionary(Of String, String) = ListProperties(maFileColumnMapItem)
        '    For Each kvp As KeyValuePair(Of String, String) In lstPropertiesMaFileColumnMapItem
        '        If parametersList.ContainsKey(kvp.Key) Then
        '            Dim strToAdd = kvp.Key + ":" + kvp.Value + ";"
        '            columnMappingsElt += strToAdd
        '        End If
        '    Next
        '    Return columnMappingsElt
        'End Function

        Private Function ListProperties(ByVal ma As MessageAction_File_ColumnMappings.MessageAction_File_ColumnMappingItem) As Dictionary(Of String, String)
            Dim mbInfos As MemberInfo() = ma.GetType().GetMembers
            Dim lstProperties As New Dictionary(Of String, String)
            For Each mbInfo As MemberInfo In mbInfos
                Select Case mbInfo.MemberType
                    Case System.Reflection.MemberTypes.Field
                        lstProperties.Add(mbInfo.Name, (DirectCast(mbInfo, FieldInfo)).GetValue(ma))
                    Case System.Reflection.MemberTypes.Property
                        lstProperties.Add(mbInfo.Name, (DirectCast(mbInfo, PropertyInfo)).GetValue(ma, Nothing))
                End Select
            Next
            Return lstProperties
        End Function
        Private Sub LoadMCFList(ByVal colMap As String)
            m_messageActionItem_File_ColumnMappings = New MessageAction_File_ColumnMappings(colMap)
        End Sub
        Private Function ShortenString(ByRef Value As String, ByRef Length As Integer) As String
            If Not Value Is Nothing AndAlso Value.Length > Length Then
                Return Value.Substring(0, Length)
            Else
                Return Value
            End If
        End Function
        Private Function GetListElements(ByVal elmntList As String) As List(Of String)
            Dim eltGenericList As New List(Of String)
            Dim elmts() As String = elmntList.Split(",")
            For Each name As String In elmts
                eltGenericList.Add(name)
            Next
            Return eltGenericList
        End Function
        Private Function GetListElementsDictionary(ByVal elmntList As String) As Dictionary(Of String, String)
            Dim eltGenericDictionary As New Dictionary(Of String, String)
            Dim elmts() As String = elmntList.Split(",")
            For Each name As String In elmts
                eltGenericDictionary.Add(name, "")
            Next
            Return eltGenericDictionary
        End Function
        Private Shared Function FixGenericLists(ByVal Value As String) As String
            Dim sRet As String = Value

            If Value.ToLower().Contains("<anytype xsi:type=") Then
                Static xsListItemSerializer As XmlSerializer
                If xsListItemSerializer Is Nothing Then
                    xsListItemSerializer = New XmlSerializer(GetType(GenericsConverter))
                End If

                Dim xd As New XmlDocument

                xd.LoadXml(Value)
                If Value.Contains("<anyType xsi:type=""SearchOptionItem"">") Then
                    Dim xnSearchItems As XmlNode = xd.SelectSingleNode("//Settings/searchItems")
                    If xnSearchItems.HasChildNodes Then
                        Dim xs As New GenericsConverter(xnSearchItems.OuterXml, xsListItemSerializer)

                        If Not xs Is Nothing AndAlso Not xs.searchItems Is Nothing AndAlso xs.searchItems.Count > 0 Then
                            For Each si As SearchOptionItem In xs.searchItems
                                xs.newSearchItems.Add(si)
                            Next
                            sRet = xs.FixXML(sRet, xsListItemSerializer, GenericsConverter.GenericType.Search)
                        End If
                    End If
                End If

                If Value.Contains("<anyType xsi:type=""QueryOptionItem"">") Then
                    Dim xnQueryItems As XmlNode = xd.SelectSingleNode("//Settings/queryItems")
                    If xnQueryItems.HasChildNodes Then
                        Dim xs As New GenericsConverter(xnQueryItems.OuterXml, xsListItemSerializer)

                        If Not xs Is Nothing AndAlso Not xs.queryItems Is Nothing AndAlso xs.queryItems.Count > 0 Then
                            For Each qi As QueryOptionItem In xs.queryItems
                                xs.newQueryItems.Add(qi)
                            Next
                            sRet = xs.FixXML(sRet, xsListItemSerializer, GenericsConverter.GenericType.Query)
                        End If
                    End If
                End If

                If Value.Contains("<anyType xsi:type=""ListFormatItem"">") Then
                    Dim xnListItems As XmlNode = xd.SelectSingleNode("//Settings/listItems")
                    If xnListItems.HasChildNodes Then
                        Dim xs As New GenericsConverter(xnListItems.OuterXml, xsListItemSerializer)

                        If Not xs Is Nothing AndAlso Not xs.listItems Is Nothing AndAlso xs.listItems.Count > 0 Then
                            For Each lfi As ListFormatItem In xs.listItems
                                xs.newListItems.Add(lfi)
                            Next
                            sRet = xs.FixXML(sRet, xsListItemSerializer, GenericsConverter.GenericType.List)
                        End If
                    End If
                End If

                If Value.Contains("<anyType xsi:type=""MessageActionItem"">") Then
                    Dim xnMessageItems As XmlNode = xd.SelectSingleNode("//Settings/messageItems")
                    If xnMessageItems.HasChildNodes Then
                        Dim xs As New GenericsConverter(xnMessageItems.OuterXml, xsListItemSerializer)

                        If Not xs Is Nothing AndAlso Not xs.messageItems Is Nothing AndAlso xs.messageItems.Count > 0 Then
                            For Each mi As MessageActionItem In xs.messageItems
                                xs.newMessageItems.Add(mi)
                            Next
                            sRet = xs.FixXML(sRet, xsListItemSerializer, GenericsConverter.GenericType.Message)
                        End If
                    End If
                End If
            End If

            Return sRet
        End Function
        Private Shared Function AddNodeParameters(ByVal Value As String) As String
            Dim sRet As String = Value
            Dim xDoc As New XmlDocument
            xDoc.LoadXml(Value)
            Dim xnMessageItems As XmlNode = xDoc.SelectSingleNode("//Settings/messageItems")
            Dim tw As TextWriter
            If Not xnMessageItems Is Nothing AndAlso xnMessageItems.HasChildNodes Then
                If Not Value.ToLower().Contains("<Parameters") Then
                    Dim xnMessageActionItemsList As XmlNodeList = xDoc.GetElementsByTagName("MessageActionItem")
                    Dim newNode As XmlNode = xDoc.CreateNode(XmlNodeType.Element, "Parameters", "")
                    For Each xn As XmlNode In xnMessageActionItemsList
                        Try
                            xn.AppendChild(newNode)
                        Catch ex As Exception
                            Throw ex
                        End Try

                    Next
                    Try
                        tw = New StringWriter()
                        xDoc.Save(tw)
                    Catch ex As Exception
                    End Try
                    sRet = xDoc.OuterXml
                    'sRet = tw.
                End If
            End If
            Return sRet
        End Function

        <Serializable(), XmlInclude(GetType(SearchOptionItem)), XmlInclude(GetType(QueryOptionItem)), XmlInclude(GetType(ListFormatItem)), XmlInclude(GetType(MessageActionItem))> _
  <Browsable(False)> _
  <EditorBrowsable(EditorBrowsableState.Never)> _
  <Description("Internal class not intended for use in your code")> _
  Public Class GenericsConverter
            Public Enum GenericType
                Search
                Query
                List
                Message
            End Enum
            Public searchItems As ArrayList
            Public queryItems As ArrayList
            Public listItems As ArrayList
            Public messageItems As ArrayList

            Public newSearchItems As New List(Of SearchOptionItem)
            Public newQueryItems As New List(Of QueryOptionItem)
            Public newListItems As New List(Of ListFormatItem)
            Public newMessageItems As New List(Of MessageActionItem)

            Public Sub New()
            End Sub
            Friend Sub New(ByVal XML As String, ByVal XS As XmlSerializer)
                Dim sXML As String = "<GenericsConverter>" & XML & "</GenericsConverter>"
                Dim tr As New IO.StringReader(sXML)
                Dim obj As GenericsConverter = XS.Deserialize(tr)

                Me.searchItems = obj.searchItems
                Me.queryItems = obj.queryItems
                Me.listItems = obj.listItems
                Me.messageItems = obj.messageItems
                Me.newSearchItems = obj.newSearchItems
                Me.newQueryItems = obj.newQueryItems
                Me.newListItems = obj.newListItems
                Me.newMessageItems = obj.newMessageItems
            End Sub

            Private Sub ClearArrays(ByVal GenType As GenericType)
                searchItems = Nothing
                queryItems = Nothing
                listItems = Nothing
                messageItems = Nothing
                Select Case GenType
                    Case GenericType.Search
                        newQueryItems = Nothing
                        newListItems = Nothing
                        newMessageItems = Nothing
                    Case GenericType.Query
                        newSearchItems = Nothing
                        newListItems = Nothing
                        newMessageItems = Nothing
                    Case GenericType.List
                        newSearchItems = Nothing
                        newQueryItems = Nothing
                        newMessageItems = Nothing
                    Case GenericType.Message
                        newSearchItems = Nothing
                        newQueryItems = Nothing
                        newListItems = Nothing
                End Select
            End Sub
            Public Function FixXML(ByVal CurrentXML As String, ByVal XS As XmlSerializer, ByVal GenType As GenericType) As String
                Me.ClearArrays(GenType)
                Dim sStart As String = Nothing, sEnd As String = Nothing, sStartOrig As String = Nothing, sEndOrig As String = Nothing

                Select Case GenType
                    Case GenericType.List
                        sStart = "<newListItems>"
                        sEnd = "</newListItems>"
                        sStartOrig = "<listItems>"
                        sEndOrig = "</listItems>"
                    Case GenericType.Message
                        sStart = "<newMessageItems>"
                        sEnd = "</newMessageItems>"
                        sStartOrig = "<messageItems>"
                        sEndOrig = "</messageItems>"
                    Case GenericType.Query
                        sStart = "<newQueryItems>"
                        sEnd = "</newQueryItems>"
                        sStartOrig = "<queryItems>"
                        sEndOrig = "</queryItems>"
                    Case GenericType.Search
                        sStart = "<newSearchItems>"
                        sEnd = "</newSearchItems>"
                        sStartOrig = "<searchItems>"
                        sEndOrig = "</searchItems>"
                End Select
                Dim tw As New IO.StringWriter()
                XS.Serialize(tw, Me)
                Dim sXML As String = Utility.GetBetween(tw.ToString(), sStart, sEnd)
                sXML = sXML.Replace(sStart, sStartOrig).Replace(sEnd, sEndOrig)
                Dim sRet As String = Utility.ReplaceBetween(CurrentXML, sStartOrig, sEndOrig, sXML)

                Return sRet
            End Function
        End Class
        Public Class SmartSplitter
            Private m_Items As List(Of String)

            Public Sub Add(ByVal Value As String)
                m_Items.Add(Value)
            End Sub
            Default Public Property Item(ByVal Index As Integer) As String
                Get
                    If Not m_Items Is Nothing AndAlso m_Items.Count > Index AndAlso Index >= 0 Then
                        Return m_Items(Index)
                    Else
                        Return String.Empty
                    End If
                End Get
                Set(ByVal Value As String)
                    If Not m_Items Is Nothing AndAlso m_Items.Count > Index AndAlso Index >= 0 Then
                        m_Items(Index) = Value
                    End If
                End Set
            End Property
            Public Sub New()
                m_Items = New List(Of String)
            End Sub
            Public Sub Split(ByVal Source As String)
                Dim position As Integer
                If Not Source Is Nothing AndAlso Source.Length > 0 Then
                    position = Source.IndexOf(":")
                    If position > 0 Then
                        If IsNumeric(Source.Substring(0, position)) Then
                            Dim headLength As Integer = Convert.ToInt32(Source.Substring(0, position))
                            If Source.Length > headLength + position Then
                                Dim header As String() = Source.Substring(position + 1, headLength).Split(";")
                                If Not header Is Nothing AndAlso header.Length > 0 Then
                                    position = position + headLength + 1
                                    Dim str As String
                                    For Each str In header
                                        If IsNumeric(str) Then
                                            Dim itemLength As Integer = Convert.ToInt32(str)
                                            If Source.Length >= position + itemLength Then
                                                If itemLength > 0 Then
                                                    m_Items.Add(Source.Substring(position, itemLength))
                                                    position = position + itemLength
                                                Else
                                                    m_Items.Add("")
                                                End If
                                            End If
                                        End If
                                    Next
                                End If
                            End If
                        End If
                    End If
                End If
            End Sub
            Public Function Length() As Integer
                If Not m_Items Is Nothing Then
                    Return m_Items.Count
                Else
                    Return 0
                End If
            End Function
            Public Function Blend() As String
                Dim header As String = ""
                Dim trailer As String = ""
                Dim str As String
                For Each str In m_Items
                    str = str.Replace(vbCrLf, Chr(10))
                    header = header & str.Length.ToString & ";"
                    trailer = trailer & str
                Next
                header = header.Length.ToString & ":" & header
                Return header & trailer
            End Function
        End Class
    End Class
End Namespace
