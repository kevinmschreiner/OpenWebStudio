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
Namespace r2i.OWS.Framework.Utilities
    Public Class Firewall
#Region "Firewall Support"
        Public Enum FirewallDirectiveEnum
            None
            Any
            [Boolean]
            [Date]
            [Guid]
            [Number]
            [String]
        End Enum
        Private Shared Sub Firewall_Item(ByRef Source As String, ByVal Directive As FirewallDirectiveEnum)
            Select Case Directive
                Case FirewallDirectiveEnum.None
                    'DO NOTHING
                Case FirewallDirectiveEnum.Any, FirewallDirectiveEnum.String
                    Firewall_SQLInjection(Source)
                Case FirewallDirectiveEnum.Boolean
                    Dim success As Boolean
                    System.Boolean.TryParse(Source, success)
                    If Not success Then
                        Source = ""
                    End If
                Case FirewallDirectiveEnum.Date
                    If Not IsDate(Source) Then
                        Source = ""
                    End If
                Case FirewallDirectiveEnum.Guid
                    Try
                        Dim g As Guid = New System.Guid(Source)
                    Catch ex As Exception
                        Source = ""
                    End Try
                Case FirewallDirectiveEnum.Number
                    If Not IsNumeric(Source) Then
                        Source = ""
                    End If
            End Select
        End Sub
        Public Shared Sub Firewall(ByRef Source As String, ByVal Escape As Boolean, ByVal Directive As FirewallDirectiveEnum, ByVal isList As Boolean)
            If Not Source Is Nothing AndAlso Source.Length > 0 Then
                If Directive = FirewallDirectiveEnum.None Then
                ElseIf isList Then
                    'CHECKING ALL ITEMS
                    If Source.Contains(",") Then
                        Dim strArray As String() = Source.Split(",")
                        Dim arrI As New Generic.List(Of String)
                        Dim i As Integer
                        For i = 0 To strArray.Length - 1
                            Firewall_Item(strArray(i), Directive)
                            If strArray(i).Length > 0 Then
                                arrI.Add(strArray(i))
                            End If
                        Next
                        Source = String.Join(",", arrI.ToArray)
                    Else
                        Firewall_Item(Source, Directive)
                    End If
                Else
                    'CHECKING THIS ITEM
                    Firewall_Item(Source, Directive)
                End If
                If Escape Then
                    Source = Source.Replace("{"c, Chr(2)).Replace("}"c, Chr(3))
                    Source = Source.Replace("["c, Chr(1)).Replace("]"c, Chr(4))
                End If
            End If
        End Sub
        Private Shared Sub Firewall_SQLInjection(ByRef Source As String)
            If Not Source Is Nothing AndAlso Source.Length > 0 Then
                'FIRST CHECK TO SEE IF THE SOURCE CONTAINS ANY KNOWN SQL FUNCTIONAL STATEMENTS
                Static Dim matchExpression As New System.Text.RegularExpressions.Regex("insert|delete|select|union|where|declare|exec", Text.RegularExpressions.RegexOptions.IgnoreCase Or Text.RegularExpressions.RegexOptions.Multiline Or Text.RegularExpressions.RegexOptions.IgnorePatternWhitespace Or Text.RegularExpressions.RegexOptions.Compiled)
                If matchExpression.IsMatch(Source) Then
                    'THE EXPRESSION MATCHED ON THE FIRST CHECK, ELIMINATE ALL DELIMITERS THAT COULD FORCE THIS TO EXECUTE
                    Static Dim replaceExpression As New System.Text.RegularExpressions.Regex("[(""*^';&<>)]", Text.RegularExpressions.RegexOptions.IgnoreCase Or Text.RegularExpressions.RegexOptions.Multiline Or Text.RegularExpressions.RegexOptions.IgnorePatternWhitespace Or Text.RegularExpressions.RegexOptions.Compiled)
                    'REASSIGN THE SOURCE TO THE SAFE REPLACER
                    Source = replaceExpression.Replace(Source, "")
                End If
            End If
        End Sub

        Public Shared Sub Unfirewall(ByRef Source As String)
            If Not Source Is Nothing AndAlso Source.Length > 0 Then
                Source = Source.Replace(Chr(2), "{"c).Replace(Chr(3), "}"c)
                Source = Source.Replace(Chr(1), "["c).Replace(Chr(4), "]"c)
            End If
        End Sub
#End Region
    End Class
End Namespace
