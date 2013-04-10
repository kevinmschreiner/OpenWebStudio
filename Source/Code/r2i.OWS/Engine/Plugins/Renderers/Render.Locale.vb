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
Imports r2i.OWS.Framework.Entities
Imports r2i.OWS.Framework.Plugins.Renderers
Imports r2i.OWS.Framework.Utilities


Namespace r2i.OWS.Renderers
    Public Class RenderLocale
        Inherits RenderBase

        Public Overrides ReadOnly Property RenderTag() As String
            Get
                Return "LOCALE"
            End Get
        End Property

        Public Overrides ReadOnly Property RenderType() As RenderTypes
            Get
                Return RenderTypes.Variable
            End Get
        End Property

        Public Overrides Function Handle_Render(ByRef Caller as EngineBase, ByVal Index As Integer, ByRef Source As String, ByRef DS As System.Data.DataSet, ByRef DR As System.Data.DataRow, ByRef RuntimeMessages As System.Collections.Generic.SortedList(Of String, String), ByVal NullReturn As Boolean, ByVal NullOverride As Boolean, ByVal ProtectSession As Boolean, ByVal SessionDelimiter As String, ByVal useSessionQuotes As Boolean, ByVal useAggregations As Boolean, ByRef FilterText As String, ByRef FilterField As String, ByRef Debugger As r2i.OWS.Framework.Debugger) As Boolean
            'Build the Localization Tag [LOCALE] or [LOCALE,FileName,Key]
            Dim REPLACED As Boolean = False
            Dim parameters As String() = ParameterizeString(Source, ","c, """"c, "\"c)
            If Not parameters Is Nothing Then
                Select Case parameters.Length
                    Case 1 '[LOCALE]
                        Try
                            Source = Threading.Thread.CurrentThread.CurrentCulture.ToString.ToLower
                            REPLACED = True
                        Catch ex As Exception
                            'ROMAIN 09/19/07
                            'TODO: Change Exceptions
                            'DotNetNuke.Services.Exceptions.LogException(ex)
                        End Try
                    Case 2
                        Try
                            If parameters(1).ToUpper = "SYSTEM" Then
                                Source = Threading.Thread.CurrentThread.CurrentCulture.ToString.ToLower
                                REPLACED = True
                            End If
                        Catch ex As Exception
                            'ROMAIN 09/19/07
                            'TODO: Change Exceptions
                            'DotNetNuke.Services.Exceptions.LogException(ex)
                        End Try
                    Case 3
                        Dim sLocale As String = Threading.Thread.CurrentThread.CurrentCulture.ToString.ToLower
                        Dim sFilename As String = parameters(1)
                        Dim sKey As String = parameters(2)

                        Try
                            'ROMAIN: 09/18/07
                            Source = AbstractFactory.Instance.EngineController.GetLocalization(sKey, sFilename)
                            'Source = DotNetNuke.Services.Localization.Localization.GetString(sKey, sFilename)
                            REPLACED = True
                        Catch ex As Exception
                            'ROMAIN 09/18/07
                            'TODO: Change Exceptions
                            'DotNetNuke.Services.Exceptions.LogException(ex)
                        End Try
                End Select
            End If
            Return REPLACED
        End Function
    End Class
End Namespace