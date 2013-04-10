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
Imports r2i.OWS.Framework.Plugins.Actions
Namespace r2i.OWS.Actions.Utilities
    Public Class ThreadedMessageActionProcess
        Public Source As Object
        Public Name As String
        Public SessionID As String
        Public Percentage As Integer
        Public Errors As Integer
        Public Status As String
        Public ThreadAction As MessageActionItem
        Public RenderingEngine As Engine
        Public FilterField As String
        Public FilterText As String
        Public SharedDS As DataSet
        Public Connection As String

        Public WriteOnly Property Completed() As Boolean
            Set(ByVal Value As Boolean)
                If Value = True Then
                    Try
                        RaiseEvent onCompleted(Me)
                    Catch ex As Exception

                    End Try
                End If
            End Set
        End Property
        Public ProcessThread As System.Threading.Thread
        Event onCompleted(ByVal obj As Object)
    End Class

End Namespace

