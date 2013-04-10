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
Imports r2i.OWS.Framework
Imports System.Collections.Generic
Imports r2i.OWS.Framework.Utilities.Compatibility, r2i.OWS.Framework.Plugins.Renderers

Namespace r2i.OWS.Renderers
    Public Class RenderAlternate
        Inherits RenderBase

        'ALL SOURCE VARIABLES HAVE ALREADY BEEN STRIPPED OF THEIR BRACING

        Private Class ItemAlternator
            Public Name As String
            Public LastIndex As Integer
            Public Maximum As Integer
            Public Alternator As Integer
        End Class
        Public Overrides ReadOnly Property RenderTag() As String
            Get
                Return "ALTERNATE"
            End Get
        End Property

        Public Overrides ReadOnly Property RenderType() As RenderTypes
            Get
                Return RenderTypes.Functional
            End Get
        End Property

        Public Overrides ReadOnly Property CanPreRender() As Boolean
            Get
                Return False
            End Get
        End Property

        Public Overrides Function Handle_Render(ByRef Caller as EngineBase, ByVal Index As Integer, ByRef Source As String, ByRef DS As System.Data.DataSet, ByRef DR As System.Data.DataRow, ByRef RuntimeMessages As System.Collections.Generic.SortedList(Of String, String), ByVal NullReturn As Boolean, ByVal NullOverride As Boolean, ByVal ProtectSession As Boolean, ByVal SessionDelimiter As String, ByVal useSessionQuotes As Boolean, ByVal useAggregations As Boolean, ByRef FilterText As String, ByRef FilterField As String, ByRef Debugger As r2i.OWS.Framework.Debugger) As Boolean
            Dim REPLACED As Boolean = False
            Dim parameters As String() = ParameterizeString(Source, ","c, """"c, "\"c)
            If Not parameters Is Nothing AndAlso parameters.Length > 1 Then
                Dim NAME As String
                Dim IGNOREINDEX As Boolean = False
                'ROMAIN: Generic replacement - 08/20/2007
                'Dim ALTERNATES As ArrayList
                Dim ALTERNATES As List(Of String)
                Dim VALUE As String
                Dim isValid As Boolean = False
                If parameters.Length > 2 Then
                    NAME = parameters(1)
                    'ROMAIN: Generic replacement - 08/20/2007
                    'ALTERNATES = New ArrayList
                    ALTERNATES = New List(Of String)
                    Dim i As Integer = 2
                    If NAME.ToUpper = "TRUE" Then
                        IGNOREINDEX = True
                        NAME = parameters(2)
                        i = 3
                    ElseIf NAME.ToUpper = "FALSE" Then
                        NAME = parameters(2)
                        i = 3
                    End If

                    For i = i To parameters.Length - 1
                        ALTERNATES.Add(parameters(i))
                    Next
                    If ALTERNATES.Count > 0 Then
                        VALUE = Render_Alternator(Caller, Index, IGNOREINDEX, NAME, ALTERNATES.ToArray)
                        If VALUE.Length > 0 Then
                            Source = VALUE
                        Else
                            Source = ""
                        End If
                        REPLACED = True
                    End If
                End If
            End If
            Return REPLACED
        End Function
        Private Function Render_Alternator(ByRef Caller As EngineBase, ByVal Index As Integer, ByVal IgnoreIndex As Boolean, ByVal Alternator As String, ByVal Values() As String) As String
            Dim Alternators As SortedList(Of String, ItemAlternator) = Nothing
            Try
                If Not Caller Is Nothing Then
                    If Caller.Context.Items.Contains("Render.Alternate") Then
                        Alternators = CType(Caller.Context.Items("Render.Alternate"), SortedList(Of String, ItemAlternator))
                    End If
                End If
            Catch ex As Exception
            End Try
            'ROMAIN: Generic replacement - 08/22/2007
            'NOTE: moved closer from the usage
            If Alternators Is Nothing Then
                Alternators = New SortedList(Of String, ItemAlternator)
            End If
            Dim thisAlternator As ItemAlternator = Nothing
            Dim altvalue As Integer = 0
            If Alternators.ContainsKey(Alternator) Then
                thisAlternator = Alternators(Alternator)
            End If
            If thisAlternator Is Nothing Then
                thisAlternator = New ItemAlternator
                thisAlternator.LastIndex = Index
                thisAlternator.Name = Alternator
                Alternators.Add(Alternator, thisAlternator)
            End If
            If Not Values Is Nothing Then
                thisAlternator.Maximum = Values.Length
            End If
            If thisAlternator.LastIndex <> Index OrElse IgnoreIndex Then
                thisAlternator.LastIndex = Index
                thisAlternator.Alternator += 1
                If thisAlternator.Alternator >= thisAlternator.Maximum Then
                    thisAlternator.Alternator = 0
                End If
            End If
            Alternators(Alternator) = thisAlternator
            altvalue = thisAlternator.Alternator

            Caller.Context.Items("Render.Alternate") = Alternators

            Return Values(altvalue)
        End Function
    End Class
End Namespace