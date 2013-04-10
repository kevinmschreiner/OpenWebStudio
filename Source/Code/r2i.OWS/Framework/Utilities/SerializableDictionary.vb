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
Imports System.Text
Imports System.Xml.Serialization
Namespace r2i.OWS.Framework.Utilities
    <XmlRoot("dictionary")> _
    Public Class SerializableDictionary(Of TKey, TValue)
        Inherits Dictionary(Of TKey, TValue)
        Implements IXmlSerializable


#Region "IXmlSerializable Members"
        Public Function GetSchema() As System.Xml.Schema.XmlSchema Implements System.Xml.Serialization.IXmlSerializable.GetSchema
            Return Nothing
        End Function

        Public Sub ReadXml(ByVal reader As System.Xml.XmlReader) Implements System.Xml.Serialization.IXmlSerializable.ReadXml
            Dim keySerializer As New XmlSerializer(GetType(TKey))
            Dim valueSerializer As New XmlSerializer(GetType(TValue))

            Dim wasEmpty As Boolean = reader.IsEmptyElement
            reader.Read()

            If wasEmpty Then
                Return
            End If

            While reader.NodeType <> System.Xml.XmlNodeType.EndElement
                reader.ReadStartElement("item")
                reader.ReadStartElement("key")
                Dim key As TKey = DirectCast(keySerializer.Deserialize(reader), TKey)
                reader.ReadEndElement()
                reader.ReadStartElement("value")
                Dim value As TValue = DirectCast(valueSerializer.Deserialize(reader), TValue)
                reader.ReadEndElement()
                Me.Add(key, value)
                reader.ReadEndElement()
                reader.MoveToContent()
            End While

            reader.ReadEndElement()
        End Sub


        Public Sub WriteXml(ByVal writer As System.Xml.XmlWriter) Implements System.Xml.Serialization.IXmlSerializable.WriteXml
            Dim keySerializer As New XmlSerializer(GetType(TKey))
            Dim valueSerializer As New XmlSerializer(GetType(TValue))
            For Each key As TKey In Me.Keys
                writer.WriteStartElement("item")
                writer.WriteStartElement("key")
                keySerializer.Serialize(writer, key)
                writer.WriteEndElement()
                writer.WriteStartElement("value")
                Dim value As TValue = Me(key)
                valueSerializer.Serialize(writer, value)
                writer.WriteEndElement()
                writer.WriteEndElement()
            Next
        End Sub

#End Region


    End Class
End Namespace