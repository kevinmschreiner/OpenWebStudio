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
Imports r2i.OWS.Framework.Plugins.Queries
Imports r2i.OWS.Framework
Imports r2i.OWS.Framework.Utilities
Imports r2i.OWS.Queries
Imports r2i.OWS
Namespace r2i.OWS.Wrapper.DNN.Extensions.Queries
    Public Class Portals
        Inherits QueryBase


        Public Overrides ReadOnly Property QueryTag() As String
            Get
                'The name of the token as it will appear at the top of the query
                Return "<DOTNETNUKE.PORTALS>"
            End Get
        End Property


        Public Overrides ReadOnly Property QueryStructure() As String
            Get
                Dim s As String = ""

                s &= "<DOTNETNUKE.PORTALS>" & vbCrLf
                s &= "   <METHOD>Create,Alias</METHOD>" & vbCrLf
                'PLACE ALL PARAMETERS THAT COULD BE PROVIDED WITHIN THIS BLOCK IN ORDER TO DISPLAY HOW THEY WILL BE UTILIZED
                s &= "   <PARAMETERS>" & vbCrLf
                'THE FOLLOWING ARE USED FOR CREATE:
                s &= "      <PORTALNAME></PORTALNAME>" & vbCrLf
                s &= "      <PORTALALIAS></PORTALALIAS>" & vbCrLf
                s &= "      <ISCHILD></ISCHILD>" & vbCrLf
                s &= "      <HOMEDIRECTORY></HOMEDIRECTORY>" & vbCrLf
                s &= "      <KEYWORDS></KEYWORDS>" & vbCrLf
                s &= "      <DESCRIPTION></DESCRIPTION>" & vbCrLf
                s &= "      <FIRSTNAME></FIRSTNAME>" & vbCrLf
                s &= "      <LASTNAME></LASTNAME>" & vbCrLf
                s &= "      <PASSWORD></PASSWORD>" & vbCrLf
                s &= "      <EMAIL></EMAIL>" & vbCrLf
                s &= "      <TEMPLATE></TEMPLATE>" & vbCrLf
                s &= "      <USERNAME></USERNAME>" & vbCrLf
                'THE FOLLOWING ARE USED FOR CREATE:
                s &= "      <ALIAS></ALIAS>" & vbCrLf 'THE VALUE CAN BE COMMA DELIMITED
                s &= "   </PARAMETERS>" & vbCrLf
                s &= "</DOTNETNUKE.PORTALS>" & vbCrLf

                Return s
            End Get
        End Property

        Public Overrides Function Handle_GetData(ByRef Caller As Framework.EngineBase, ByVal isSubQuery As Boolean, ByVal Query As String, ByVal FilterField As String, ByVal FilterText As String, ByRef DebugWriter As Framework.Debugger, ByVal isRendered As Boolean, Optional ByVal timeout As Integer = -1, Optional ByVal CustomConnection As String = Nothing) As Framework.RuntimeBase.QueryResult
            Dim rslt As New Framework.RuntimeBase.QueryResult(RuntimeBase.ExecutableResultEnum.Executed, New DataSet)
            Try
                Dim output As DataTable = Nothing
                If Not Query Is Nothing AndAlso Query.Length > 0 Then
                    Dim strMETHOD As String
                    Dim strPARAMETERS As String
                    strMETHOD = Utility.XMLPropertyParse_Quick(Query, "method")
                    strPARAMETERS = Utility.XMLPropertyParse_Quick(Query, "parameters")
                    Select Case strMETHOD.ToUpper
                        Case "CREATE"
                            output = DotNetNuke_Portals_Create(Caller, DebugWriter, strPARAMETERS)
                        Case "ALIAS"
                            output = DotNetNuke_Portals_Alias(Caller, DebugWriter, strPARAMETERS)
                    End Select
                    rslt.Value.Tables.Add(output)
                End If
            Catch ex As Exception
                rslt.Result = RuntimeBase.ExecutableResultEnum.Failed
                rslt.Error = ex
            End Try

            Return rslt
        End Function

        'PRIVATE METHODS
        Private Function DotNetNuke_Portals_Create(ByRef Caller As Framework.EngineBase, ByRef DebugWriter As Framework.Debugger, ByVal strParameters As String) As DataTable
            'CREATE THE RESULT TABLE
            Dim returnI As Int32 = -1
            Dim dt As New DataTable
            dt.Columns.Add("PortalId", GetType(Int32))
            dt.Columns.Add("Error", GetType(String))

            'POPULATE THE RESULT TABLE
            Dim dr As DataRow = dt.NewRow
            Try
                returnI = r2i.OWS.Wrapper.DNN.Extensions.Utilities.Dotnetnuke_CreateNewPortal(CType(Caller.UserID, Int32), Caller.PortalSettings, GetParameter("TEMPLATE", strParameters), GetParameter("PORTALNAME", strParameters), GetParameter("PORTALALIAS", strParameters), CType(GetParameter("ISCHILD", strParameters), Boolean), GetParameter("HOMEDIRECTORY", strParameters), GetParameter("FIRSTNAME", strParameters), GetParameter("LASTNAME", strParameters), GetParameter("USERNAME", strParameters), GetParameter("PASSWORD", strParameters), GetParameter("EMAIL", strParameters), GetParameter("DESCRIPTION", strParameters), GetParameter("KEYWORDS", strParameters))
                dr("PortalId") = returnI
                If returnI < 0 Then
                    dr("Error") = "The portal creation failed due to an unidentified complication."
                End If
            Catch ex As Exception
                dr("PortalId") = -1
                dr("Error") = ex.ToString
            End Try

            'RETURN THE RESULT TABLE
            dt.Rows.Add(dr)
            Return dt
        End Function
        Private Function DotNetNuke_Portals_Alias(ByRef Caller As Framework.EngineBase, ByRef DebugWriter As Framework.Debugger, ByVal strParameters As String) As DataTable
            'CREATE THE RESULT TABLE
            Dim returnI As Int32 = -1
            Dim dt As New DataTable
            dt.Columns.Add("PortalAliasId", GetType(Int32))
            dt.Columns.Add("Error", GetType(String))

            'POPULATE THE RESULT TABLE
            Try
                Dim strValues As String()
                strValues = GetParameter(strParameters, "ALIAS").Split(",")
                If strValues.Length > 1 AndAlso IsNumeric(strValues(0)) Then
                    Dim tPortalId As Integer = CInt(strValues(0))
                    Dim xi As Integer = 1
                    For xi = 1 To strValues.Length - 1
                        Dim dr As DataRow = dt.NewRow
                        Try
                            dr("PortalAliasId") = DotNetNuke.Data.DataProvider.Instance().AddPortalAlias(tPortalId, strValues(xi).ToLower)
                        Catch ex As Exception
                            dr("PortalAliasId") = -1
                            dr("Error") = ex.ToString
                        End Try
                        dt.Rows.Add(dr)
                    Next

                    'SET THE NEW CACHE VALUE
                    Dim objPortalAliasCollection As DotNetNuke.Entities.Portals.PortalAliasCollection
                    Dim objPortalAliasController As New DotNetNuke.Entities.Portals.PortalAliasController
                    objPortalAliasCollection = objPortalAliasController.GetPortalAliases()
                    DotNetNuke.Common.Utilities.DataCache.SetCache("GetPortalByAlias", objPortalAliasCollection, Nothing, DateTime.MaxValue, TimeSpan.Zero)
                End If
            Catch ex As Exception
                Dim dr As DataRow = dt.NewRow
                dr("PortalAliasId") = -1
                dr("Error") = ex.ToString
                dt.Rows.Add(dr)
            End Try

            'RETURN THE RESULT TABLE
            Return dt
        End Function
        Private Function GetParameter(ByVal Name As String, ByRef Parameters As String) As String
            Return Utility.XMLPropertyParse_Quick(Parameters, Name)
        End Function

    End Class
End Namespace