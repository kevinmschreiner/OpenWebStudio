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
Imports System
Imports System.Collections.Generic
Imports System.Reflection

Namespace r2i.OWS.ObjectMappingHelper
    Public Class ObjectMapper
        Private Shared Function GetProperties(ByVal objType As Type) As List(Of PropertyMappingInfo)
            Dim info As List(Of PropertyMappingInfo) = MappingInfoCache.GetCache(objType.Name)

            If info Is Nothing Then
                info = LoadPropertyMappingInfo(objType)
                MappingInfoCache.SetCache(objType.Name, info)
            End If
            Return info
        End Function

        Private Shared Function LoadPropertyMappingInfo(ByVal objType As Type) As List(Of PropertyMappingInfo)
            Dim mapInfoList As New List(Of PropertyMappingInfo)()

            For Each info As PropertyInfo In objType.GetProperties()
                Dim mapAttr As DataMappingAttribute = DirectCast(Attribute.GetCustomAttribute(info, GetType(DataMappingAttribute)), DataMappingAttribute)

                If mapAttr IsNot Nothing Then
                    Dim mapInfo As New PropertyMappingInfo(mapAttr.DataFieldName, mapAttr.NullValue, info)
                    mapInfoList.Add(mapInfo)
                End If
            Next
            Return mapInfoList
        End Function

        Public Shared Function FillObject(Of T As {Class, New})(ByVal objType As Type, ByVal dr As Dictionary(Of String, Object)) As T
            Dim obj As T = Nothing

            'Try
            '    Dim mapInfo As List(Of PropertyMappingInfo) = GetProperties(objType)
            '    Dim ordinals As Integer() = GetOrdinals(mapInfo, dr)
            '    If dr.Read() Then
            '        obj = CreateObject(Of T)(dr, mapInfo, ordinals)
            '    End If
            'Finally
            '    If dr.IsClosed = False Then
            '        dr.Close()
            '    End If
            'End Try

            Return obj
        End Function


        Private Shared Function CreateObject(Of T As {Class, New})(ByVal dr As Dictionary(Of String, Object), ByVal propInfoList As List(Of PropertyMappingInfo), ByVal ordinals As Integer()) As T
            Dim obj As New T()
            For i As Integer = 0 To propInfoList.Count - 1

                ' iterate through the PropertyMappingInfo objects for this type.
                If propInfoList(i).PropertyInfo.CanWrite Then
                    Dim type As Type = propInfoList(i).PropertyInfo.PropertyType
                    Dim value As Object = propInfoList(i).DefaultValue

                    'If ordinals(i) <> -1 AndAlso dr.IsDBNull(ordinals(i)) = False Then
                    '    value = dr.GetValue(ordinals(i))
                    'End If

                    Try
                        ' try implicit conversion first
                        propInfoList(i).PropertyInfo.SetValue(obj, value, Nothing)
                    Catch
                        ' data types do not match

                        Try

                            ' need to handle enumeration types differently than other base types.
                            If type.BaseType.Equals(GetType(System.Enum)) Then
                                propInfoList(i).PropertyInfo.SetValue(obj, System.[Enum].ToObject(type, value), Nothing)
                            Else
                                ' try explicit conversion
                                propInfoList(i).PropertyInfo.SetValue(obj, Convert.ChangeType(value, type), Nothing)
                            End If
                            ' error assigning the datareader value to a property
                        Catch
                        End Try
                    End Try
                End If
            Next

            Return obj
        End Function


    End Class
End Namespace

