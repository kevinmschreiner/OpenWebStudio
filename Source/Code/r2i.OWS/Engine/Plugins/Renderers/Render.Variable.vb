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
Imports r2i.OWS.Framework.Utilities, r2i.OWS.Framework.Utilities.Compatibility
Imports r2i.OWS.Framework.Plugins.Renderers
Imports r2i.OWS.Framework.Entities
Imports r2i.OWS.Framework.DataAccess
Imports System.Collections.Generic
Imports System.Reflection


Namespace r2i.OWS.Renderers
    Public Class RenderVariable
        Inherits RenderBase

        Private Enum VariableTypeEnum
            Runtime
            Session
            QueryString
            ViewState
            Cache
            Context
            Cookie
            ModuleSetting
            Message
            Form
            Table
            Action
        End Enum

        Public Overrides ReadOnly Property RenderTag() As String
            Get
                Return ""
            End Get
        End Property

        Public Overrides ReadOnly Property RenderType() As RenderTypes
            Get
                Return RenderTypes.Variable
            End Get
        End Property
        Private Shared Sub IdentifyFirewallDirectives(ByRef Value As String, ByRef Directive As Utilities.Firewall.FirewallDirectiveEnum, ByRef isList As Boolean)
            If Value.Contains(":") Then
                Dim valLeft As String = Value.Substring(0, Value.IndexOf(":"c))
                Dim valRight As String = Value.Substring(Value.IndexOf(":"c) + 1).ToUpper
                If valRight.EndsWith("LIST") Then
                    isList = True
                End If

                If valRight.StartsWith("NONE") Then
                    Directive = Utilities.Firewall.FirewallDirectiveEnum.None
                ElseIf valRight.StartsWith("ANY") Then
                    Directive = Utilities.Firewall.FirewallDirectiveEnum.Any
                ElseIf valRight.StartsWith("BOOLEAN") Then
                    Directive = Utilities.Firewall.FirewallDirectiveEnum.Boolean
                ElseIf valRight.StartsWith("DATE") Then
                    Directive = Utilities.Firewall.FirewallDirectiveEnum.Date
                ElseIf valRight.StartsWith("GUID") Then
                    Directive = Utilities.Firewall.FirewallDirectiveEnum.Guid
                ElseIf valRight.StartsWith("NUMBER") Then
                    Directive = Utilities.Firewall.FirewallDirectiveEnum.Number
                ElseIf valRight.StartsWith("STRING") Then
                    Directive = Utilities.Firewall.FirewallDirectiveEnum.String
                End If
                Value = valLeft
            Else
                If Value.ToUpper = "Q" Or Value.ToUpper = "QUERYSTRING" Then
                    isList = False
                    Directive = Utilities.Firewall.FirewallDirectiveEnum.Any
                Else
                    isList = False
                    Directive = Utilities.Firewall.FirewallDirectiveEnum.None
                End If
            End If
        End Sub
        Private Class VariableResult
            Public Name As String
            Public Type As String
            Public Source As String
        End Class
        Private Shared Function _HandleVariable(ByRef vr As VariableResult, ByVal bSkipRendering As Boolean, ByRef Caller As EngineBase, ByVal Index As Integer, ByRef DS As System.Data.DataSet, ByRef DR As System.Data.DataRow, ByRef RuntimeMessages As System.Collections.Generic.SortedList(Of String, String), ByVal NullReturn As Boolean, ByVal NullOverride As Boolean, ByVal ProtectSession As Boolean, ByVal SessionDelimiter As String, ByVal useSessionQuotes As Boolean, ByVal useAggregations As Boolean, ByRef FilterText As String, ByRef FilterField As String, ByRef Debugger As r2i.OWS.Framework.Debugger) As Boolean
            'STRUCTURE: [COLUMN/VARIABLENAME,OPTIONAL SOURCE (TABLE OR ENTITY)]
            Dim b As Boolean = False
            Dim REPLACED As Boolean = False
            Dim bSystemParse As Boolean = False

            If Not vr Is Nothing AndAlso Not vr.Name Is Nothing AndAlso Not vr.Type Is Nothing AndAlso vr.Type.Length > 0 Then
                Dim rhv As String = vr.Type
                Dim lhv As String = vr.Name
                Dim firewallDirective As Utilities.Firewall.FirewallDirectiveEnum
                Dim firewallList As Boolean = False
                IdentifyFirewallDirectives(rhv, firewallDirective, firewallList)


                Select Case rhv.ToUpper
                    Case "MO", "MODULESETTING", "MODULESETTINGS"
                        If Caller.Settings.ContainsKey(lhv) Then
                            Dim val As String = Utility.CNullStr(Caller.Settings.Item(lhv))
                            If Not val Is Nothing Then
                                vr.Source = val
                                Firewall.Firewall(vr.Source, False, firewallDirective, firewallList)
                                b = True
                            ElseIf NullReturn Then
                                vr.Source = ""
                                b = True
                            End If
                        Else
                            'NOTE: ADDED MODULESETTING DEFAULT TO EMPTY STRING WHEN MISSING - MAY CAUSE AN ISSUE!!!
                            If NullOverride Then
                                vr.Source = Nothing
                                b = True
                            Else
                                vr.Source = ""
                                b = True
                            End If
                        End If
                    Case "SY", "SYSTEM"
                        bSystemParse = True
                    Case "TEXT"
                        vr.Source = lhv
                        b = True
                    Case "M", "ME", "MESSAGE"
                        If Not RuntimeMessages Is Nothing AndAlso RuntimeMessages.ContainsKey(lhv) Then
                            Dim val As String = RuntimeMessages.Item(lhv)
                            If Not val Is Nothing Then
                                vr.Source = val
                                Firewall.Firewall(vr.Source, True, firewallDirective, firewallList)
                                b = True
                            ElseIf NullOverride Then
                                vr.Source = Nothing
                                b = True
                            ElseIf NullReturn Then
                                vr.Source = ""
                                b = True
                            End If
                        End If
                    Case "S", "SESSION"
                        Dim val As Object = SessionVariableToString(Caller, lhv, ProtectSession, useSessionQuotes, SessionDelimiter)
                        If Not val Is Nothing Then
                            b = True
                            vr.Source = val
                            Firewall.Firewall(vr.Source, False, firewallDirective, firewallList)
                        ElseIf NullOverride Then
                            vr.Source = Nothing
                            b = True
                        ElseIf NullReturn Then
                            b = True
                            vr.Source = ""
                        End If
                    Case "VA", "VARIABLE"
                        Dim val As String = ReplaceCommonVariables(Caller, RuntimeMessages, lhv, FilterText, FilterField, Debugger, True)
                        If Not val Is Nothing Then
                            vr.Source = val
                            'NOTE: NO FIREWALL HERE AS REPLACECOMMON DOES IT FOR US
                            b = True
                        ElseIf NullOverride Then
                            vr.Source = Nothing
                            b = True
                        ElseIf NullReturn Then
                            vr.Source = ""
                            b = True
                        End If
                    Case "V", "VIEWSTATE"
                        Dim val As String = Utility.CNullStr(Caller.ViewState.Item(lhv))
                        If Not val Is Nothing Then
                            vr.Source = val
                            Firewall.Firewall(vr.Source, False, firewallDirective, firewallList)
                            b = True
                        ElseIf NullOverride Then
                            vr.Source = Nothing
                            b = True
                        ElseIf NullReturn Then
                            vr.Source = ""
                            b = True
                        End If
                    Case "CACHE"
                        Dim val As String = Utility.CNullStr(Caller.Context.Cache.Item(lhv))
                        If Not val Is Nothing Then
                            vr.Source = val
                            Firewall.Firewall(vr.Source, False, firewallDirective, firewallList)
                            b = True
                        ElseIf NullOverride Then
                            vr.Source = Nothing
                            b = True
                        ElseIf NullReturn Then
                            vr.Source = ""
                            b = True
                        End If
                    Case "SHAREDCACHE"
                        Dim val As String = Utility.CNullStr(Engine.SharedCache_Get(lhv))
                        If Not val Is Nothing Then
                            vr.Source = val
                            Firewall.Firewall(vr.Source, False, firewallDirective, firewallList)
                            b = True
                        ElseIf NullOverride Then
                            vr.Source = Nothing
                            b = True
                        ElseIf NullReturn Then
                            vr.Source = ""
                            b = True
                        End If
                    Case "CONTEXT"
                        Dim val As String
                        If Caller.Context.Items.Contains(lhv) Then
                            val = Utility.CNullStr(Caller.Context.Items.Item(lhv))
                        Else
                            val = Nothing
                        End If

                        If Not val Is Nothing Then
                            vr.Source = val
                            Firewall.Firewall(vr.Source, False, firewallDirective, firewallList)
                            b = True
                        ElseIf NullOverride Then
                            vr.Source = Nothing
                            b = True
                        ElseIf NullReturn Then
                            vr.Source = ""
                            b = True
                        End If

                    Case "F", "FORM"
                        If lhv.Length > 0 Then
                            Dim val As String = Caller.Request.Form.Item(lhv)
                            If val Is Nothing Then
                                If lhv.IndexOf(".") > 0 Then
                                    Dim strs() As String = lhv.Split(".")
                                    If strs.Length = 2 Then
                                        Dim propertyname As String = strs(1).ToUpper
                                        lhv = strs(0)
                                        Try
                                            Dim fi As System.Web.HttpPostedFile = Caller.Request.Files.Get(lhv)
                                            'Added the following to block Multiple Extension Security Flaw
                                            If fi.FileName.ToLower.Contains(".asp;") Then
                                                fi = Nothing
                                                Caller.Request.Files(lhv).InputStream.Close()
                                            End If

                                            If Not fi Is Nothing Then
                                                'Dim fio As New IO.FileInfo(fi.FileName)
                                                'VERSION: 1.9.7 - Add another tag which lets you check the following values for files.
                                                Select Case propertyname
                                                    Case "PATH"
                                                        'val = fio.FullName.Substring(0, fio.FullName.Length - fio.Name.Length)
                                                        val = IO.Path.GetDirectoryName(fi.FileName)
                                                    Case "NAME"
                                                        'val = fio.Name
                                                        val = IO.Path.GetFileName(fi.FileName)
                                                    Case "NAMEONLY"
                                                        val = IO.Path.GetFileNameWithoutExtension(fi.FileName)
                                                    Case "EXTENSION"
                                                        val = IO.Path.GetExtension(fi.FileName)
                                                    Case "TYPE"
                                                        val = fi.ContentType
                                                    Case "LENGTH", "SIZE"
                                                        val = fi.ContentLength
                                                    Case "UTF32", "UTF8", "UTF7", "HEX", "ASCII"
                                                        Try
                                                            Dim fs(fi.ContentLength - 1) As Byte
                                                            fi.InputStream.Read(fs, 0, fi.ContentLength)
                                                            If propertyname = "UTF32" Then
                                                                val = System.Text.Encoding.UTF32.GetString(fs)
                                                            ElseIf propertyname = "UTF8" Then
                                                                val = System.Text.Encoding.UTF8.GetString(fs)
                                                            ElseIf propertyname = "UTF7" Then
                                                                val = System.Text.Encoding.UTF7.GetString(fs)
                                                            ElseIf propertyname = "HEX" Then
                                                                val = r2i.OWS.Framework.Utilities.Utility.GetHexString(fs)
                                                            ElseIf propertyname = "ASCII" Then
                                                                val = System.Text.ASCIIEncoding.ASCII.GetString(fs)
                                                            End If
                                                        Catch ex As Exception
                                                        End Try
                                                    Case "WIDTH", "HEIGHT", "RAWFORMAT", "PIXELFORMAT", "DIMENSIONS", "VERTICALRESOLUTION", "HORIZONTALRESOLUTION"
                                                        Try
                                                            Dim img As System.Drawing.Image
                                                            img = System.Drawing.Image.FromStream(fi.InputStream)
                                                            If Not img Is Nothing Then
                                                                Select Case propertyname
                                                                    Case "WIDTH"
                                                                        val = img.Width
                                                                    Case "HEIGHT"
                                                                        val = img.Height
                                                                    Case "RAWFORMAT"
                                                                        val = img.RawFormat.ToString
                                                                    Case "PIXELFORMAT"
                                                                        val = img.PixelFormat.ToString
                                                                    Case "DIMENSIONS"
                                                                        val = img.Width & "x" & img.Height
                                                                    Case "VERTICALRESOLUTION"
                                                                        val = img.VerticalResolution
                                                                    Case "HORIZONTALRESOLUTION"
                                                                        val = img.HorizontalResolution
                                                                End Select
                                                            Else
                                                                val = "Undefined"
                                                            End If
                                                            img = Nothing
                                                        Catch ex As Exception
                                                            val = "Undefined"
                                                        End Try
                                                End Select
                                            Else
                                                val = ""
                                            End If
                                        Catch ex As Exception
                                            val = ""
                                        End Try
                                    Else
                                        val = ""
                                    End If
                                End If
                            Else
                                If Not val Is Nothing Then
                                    Firewall.Firewall(val, True, firewallDirective, firewallList)
                                End If
                            End If
                            If Not val Is Nothing Then
                                'VERSION: 1.9.9 - AUTOESCAPE FORM VALUES
                                'Source = AddEscapes(val)
                                vr.Source = val

                                b = True
                            ElseIf NullOverride Then
                                vr.Source = Nothing
                                b = True
                            ElseIf NullReturn Then
                                vr.Source = ""
                                b = True
                            End If
                        Else
                            vr.Source = GetRequestForm(Caller)
                            b = True
                        End If
                    Case "Q", "QUERYSTRING"
                        Dim val As String = Nothing
                        If Not Caller.Request.QueryString Is Nothing AndAlso Not Caller.Request.QueryString(lhv) Is Nothing Then
                            val = Caller.Request.QueryString.Item(lhv)
                        End If
                        If Not val Is Nothing Then
                            'VERSION: 1.9.9 - AUTOESCAPE QUERYSTRING VALUES
                            'Source = AddEscapes(val)
                            vr.Source = val
                            Firewall.Firewall(vr.Source, True, firewallDirective, firewallList)
                            b = True
                        ElseIf NullOverride Then
                            vr.Source = Nothing
                            b = True
                        ElseIf NullReturn Then
                            vr.Source = ""
                            b = True
                        End If
                    Case "C", "COOKIE"
                        Dim val As String = Nothing
                        If Not Caller.Request.Cookies(lhv) Is Nothing Then
                            val = Caller.Request.Cookies(lhv).Value
                        End If
                        If Not val Is Nothing Then
                            'VERSION: 1.9.9 - AUTOESCAPE COOKIE VALUES
                            'Source = AddEscapes(val)
                            vr.Source = val
                            Firewall.Firewall(vr.Source, True, firewallDirective, firewallList)
                            b = True
                        ElseIf NullOverride Then
                            vr.Source = Nothing
                            b = True
                        ElseIf NullReturn Then
                            vr.Source = ""
                            b = True
                        End If
                    Case "CON", "CFG", "CONFIGURATION", "APPSETTINGS"
                        Dim val As String
                        val = Utility.ConfigurationSetting(lhv)
                        If Not val Is Nothing Then
                            vr.Source = val
                            Firewall.Firewall(vr.Source, False, firewallDirective, firewallList)
                            b = True
                        ElseIf NullOverride Then
                            vr.Source = Nothing
                            b = True
                        ElseIf NullReturn Then
                            vr.Source = ""
                            b = True
                        End If
                    Case "A", "ACTION"
                        Dim val As String = Utility.CNullStr(Caller.ActionVariable(lhv))
                        If Not val Is Nothing Then
                            vr.Source = val
                            Firewall.Firewall(vr.Source, False, firewallDirective, firewallList)
                            b = True
                        ElseIf NullOverride Then
                            vr.Source = Nothing
                            b = True
                        ElseIf NullReturn Then
                            vr.Source = ""
                            b = True
                        End If
                    Case "TRUE", "FALSE"
                        bSkipRendering = CBool(rhv)
                        If Not DS Is Nothing AndAlso DS.Tables.Count > 0 AndAlso Not DR Is Nothing Then
                            If ConvertDataValueToString(firewallDirective, firewallList, DR, vr.Source, vr.Source) Then
                                b = True
                            End If
                        End If
                        bSystemParse = True
                    Case Else
                        If IsNumeric(rhv) Then
                            Dim idx As Integer = Convert.ToInt32(rhv)
                            Dim colName As String = lhv
                            If Not DS Is Nothing Then
                                If DS.Tables.Count > idx AndAlso idx >= 0 Then
                                    If DS.Tables(idx).Rows.Count > 0 Then
                                        If Not DR Is Nothing AndAlso idx = 0 Then

                                            If ConvertDataValueToString(firewallDirective, firewallList, DR, colName, vr.Source, NullOverride) Then
                                                b = True
                                            End If
                                        Else
                                            If ConvertDataValueToString(firewallDirective, firewallList, DS.Tables(idx).Rows(Caller.CachedTableRowIndexCollection("_lx_" & idx.ToString)), colName, vr.Source, NullOverride) Then
                                                b = True
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                            If b = False AndAlso Not Caller.TableVariables Is Nothing Then
                                If Not Caller.TableVariables Is Nothing Then
                                    If Caller.TableVariables.Tables.Count > idx AndAlso idx >= 0 Then
                                        If Caller.TableVariables.Tables(idx).Rows.Count > 0 Then
                                            If ConvertDataValueToString(firewallDirective, firewallList, Caller.TableVariables.Tables(idx).Rows(0), colName, vr.Source, NullOverride) Then
                                                b = True
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        ElseIf Not rhv Is Nothing AndAlso rhv.Length > 0 Then
                            Dim colName As String = lhv
                            If rhv.IndexOf(":") >= 0 Then   'This is from a subquery
                                Dim sSQName As String = rhv.Split(":").GetValue(0)
                                Dim sSQTable As String = rhv.Split(":").GetValue(1)
                                Dim sQuery As String = Caller.Cache("xLSQ_Q" & sSQName, False)

                                If Not sQuery Is Nothing AndAlso sQuery <> "" Then
                                    Dim dsQ As DataSet = Caller.Cache("xLSQ_DS" & sQuery, False)
                                    If Not dsQ Is Nothing Then
                                        If dsQ.Tables.Contains(sSQTable) Then
                                            If dsQ.Tables(sSQTable).Rows.Count > 0 Then
                                                If ConvertDataValueToString(firewallDirective, firewallList, dsQ.Tables(sSQTable).Rows(Caller.CachedTableRowIndexCollection(sSQTable)), colName, vr.Source, NullOverride) Then
                                                    b = True
                                                End If
                                            End If
                                        End If
                                    End If
                                End If
                            Else
                                If Not DS Is Nothing Then
                                    If DS.Tables.Contains(rhv) Then
                                        If DS.Tables(rhv).Rows.Count > 0 Then
                                            If ConvertDataValueToString(firewallDirective, firewallList, DS.Tables(rhv).Rows(Caller.CachedTableRowIndexCollection(rhv)), colName, vr.Source, NullOverride) Then
                                                b = True
                                            End If
                                        End If
                                    End If
                                End If
                                If b = False Then
                                    If Not Caller.TableVariables Is Nothing Then
                                        If Caller.TableVariables.Tables.Contains(rhv) Then
                                            If Caller.TableVariables.Tables(rhv).Rows.Count > 0 Then
                                                If ConvertDataValueToString(firewallDirective, firewallList, Caller.TableVariables.Tables(rhv).Rows(0), colName, vr.Source, NullOverride) Then
                                                    b = True
                                                End If
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        End If
                End Select
            Else
                Select Case vr.Source '.ToUpper
                    Case "SORTTAG", "FILTERTAG", "FILTER"
                        Dim r As RenderBase = Plugins.Manager.GetPlugin(Plugins.PluginTag.Create(Config.Section.Tokens.ToString.ToLower, RenderTypes.Variable.ToString.ToLower, vr.Source)) 'Common.GetRenderer(Source, RenderTypes.Variable)
                        If Not r Is Nothing Then
                            r.Handle_Render(Caller, Index, vr.Source, DS, DR, RuntimeMessages, NullReturn, True, ProtectSession, SessionDelimiter, useSessionQuotes, useAggregations, FilterText, FilterField, Debugger)
                            b = True
                            bSystemParse = True
                        End If
                    Case Else
                        If Not DR Is Nothing Then
                            If ConvertDataValueToString(Utilities.Firewall.FirewallDirectiveEnum.None, False, DR, vr.Source, vr.Source, NullOverride) Then
                                b = True
                            End If
                        End If
                        bSystemParse = True
                End Select
            End If
            If b = False And bSystemParse = True Then
                Try
                    If Not Caller.xls.useExplicitSystemVariables OrElse (vr.Source.ToUpper.StartsWith("*") OrElse vr.Source.ToUpper.EndsWith(",SYSTEM")) Then
                        If Not vr.Source.ToUpper.StartsWith("CRYPTOGRAPHY.") AndAlso Not vr.Source.ToUpper.StartsWith("*CRYPTOGRAPHY.") AndAlso vr.Source.IndexOf("."c) >= 0 Then

                            b = GetMemberValue(Caller, vr.Source)
                        Else
                            Select Case vr.Source.ToUpper
                                'Create a List with all the runtime values
                                Case "*CRYPTOGRAPHY.KEY.RIJNDAEL", "CRYPTOGRAPHY.KEY.RIJNDAEL", "CRYPTOGRAPHY.KEY.RIJNDAEL,SYSTEM"
                                    Dim crypto As New r2i.OWS.Framework.Utilities.Security.Cryptography.Rijndael
                                    crypto.GenerateKey()
                                    vr.Source = Convert.ToBase64String(crypto.Key)
                                    crypto = Nothing
                                    b = True
                                Case "*CRYPTOGRAPHY.KEY.RC2", "CRYPTOGRAPHY.KEY.RC2", "CRYPTOGRAPHY.KEY.RC2,SYSTEM"
                                    Dim crypto As New r2i.OWS.Framework.Utilities.Security.Cryptography.RC2
                                    crypto.GenerateKey()
                                    vr.Source = Convert.ToBase64String(crypto.Key)
                                    crypto = Nothing
                                    b = True
                                Case "*CRYPTOGRAPHY.KEY.TRIPLEDES", "CRYPTOGRAPHY.KEY.TRIPLEDES", "CRYPTOGRAPHY.KEY.TRIPLEDES,SYSTEM"
                                    Dim crypto As New r2i.OWS.Framework.Utilities.Security.Cryptography.TripleDES
                                    crypto.GenerateKey()
                                    vr.Source = Convert.ToBase64String(crypto.Key)
                                    crypto = Nothing
                                    b = True
                                Case "*CRYPTOGRAPHY.KEY.DES", "CRYPTOGRAPHY.KEY.DES", "CRYPTOGRAPHY.KEY.DES,SYSTEM"
                                    Dim crypto As New r2i.OWS.Framework.Utilities.Security.Cryptography.DES
                                    crypto.GenerateKey()
                                    vr.Source = Convert.ToBase64String(crypto.Key)
                                    crypto = Nothing
                                    b = True
                                Case "*LOCALHOSTALIAS", "LOCALHOSTALIAS", "LOCALHOSTALIAS,SYSTEM"
                                    'If PortalSettings.PortalAlias.HTTPAlias.ToString.ToLower.StartsWith("localhost") Then
                                    '    Source = PortalSettings.PortalAlias.HTTPAlias.Replace("localhost", "")
                                    'Else
                                    If Caller.PortalSettings.HTTPAlias.ToString.ToLower.StartsWith("localhost") Then
                                        vr.Source = Caller.PortalSettings.HTTPAlias.Replace("localhost", "")
                                    Else
                                        vr.Source = ""
                                    End If
                                    b = True
                                Case "*LANGUAGE", "LANGUAGE", "LANGUAGE,SYSTEM"
                                    vr.Source = System.Threading.Thread.CurrentThread.CurrentUICulture.Name
                                    b = True
                                Case "*ALIAS", "ALIAS", "ALIAS,SYSTEM"
                                    'Source = PortalSettings.PortalAlias.HTTPAlias.ToString
                                    vr.Source = Caller.PortalSettings.HTTPAlias.ToString
                                    b = True
                                Case "*APPLICATIONPATH", "APPLICATIONPATH", "APPLICATIONPATH,SYSTEM"
                                    'ROMAIN: 09/18/07
                                    'Source = DotNetNuke.Common.ApplicationPath                                    
                                    vr.Source = AbstractFactory.Instance.EngineController.GetApplicationPath()
                                    b = True
                                Case "*PAGETITLE", "PAGETITLE,SYSTEM"
                                    vr.Source = Caller.Caller.Page.Title
                                    b = True
                                Case "*MODULETITLE", "MODULETITLE", "MODULETITLE,SYSTEM"
                                    vr.Source = Caller.xls.Title
                                    b = True
                                Case "*MODULEID", "MODULEID", "MODULEID,SYSTEM"
                                    vr.Source = Caller.ModuleID.ToString
                                    b = True
                                Case "*PORTALID", "PORTALID", "PORTALID,SYSTEM"
                                    vr.Source = Caller.PortalID.ToString
                                    b = True
                                Case "*TABID", "TABID", "TABID,SYSTEM"
                                    vr.Source = Caller.TabID.ToString
                                    b = True
                                Case "*TABMODULEID", "TABMODULEID", "TABMODULEID,SYSTEM"
                                    vr.Source = Caller.TabModuleID.ToString
                                    b = True
                                Case "*CLIENTID", "CLIENTID", "CLIENTID,SYSTEM"
                                    vr.Source = Caller.ClientID.ToString
                                    b = True
                                Case "*ID", "ID", "ID,SYSTEM"
                                    vr.Source = Caller.ClientID.ToString
                                    b = True
                                Case "*MODULEPATH", "MODULEPATH", "MODULEPATH,SYSTEM"
                                    vr.Source = Caller.ModulePath.ToString
                                    b = True
                                Case "*PORTALPATH", "PORTALPATH", "PORTALPATH,SYSTEM"
                                    vr.Source = Caller.PortalSettings.HomeDirectory
                                    b = True
                                Case "*PAGENUMBER", "PAGENUMBER", "PAGENUMBER,SYSTEM"
                                    vr.Source = (Caller.PageCurrent + 1).ToString
                                    b = True
                                Case "*PAGESIZE", "PAGESIZE", "PAGESIZE,SYSTEM"
                                    Try
                                        'If Session("RPP" & "m" & ModuleID.ToString & "t" & TabID.ToString) Is Nothing Then
                                        '    Source = xls.recordsPerPage.ToString
                                        'Else
                                        '    Source = Session("RPP" & "m" & ModuleID.ToString & "t" & TabID.ToString)
                                        'End If
                                        vr.Source = Caller.RecordsPerPage
                                        b = True
                                    Catch ex As Exception
                                    End Try
                                Case "*ROWNUMBER", "ROWNUMBER", "ROWNUMBER,SYSTEM", "INDEX,SYSTEM"
                                    vr.Source = Index.ToString
                                    b = True
                                Case "*STARTINDEX", "STARTINDEX", "STARTINDEX,SYSTEM", "STARTINDEX,SYSTEM"
                                    Dim si As Integer
                                    si = (Caller.PageCurrent + 1)
                                    If (si > 1) Then
                                        vr.Source = (Caller.PageCurrent * Caller.RecordsPerPage) + 1
                                    Else
                                        vr.Source = 1
                                    End If
                                    b = True
                                Case "*ENDINDEX", "ENDINDEX", "ENDINDEX,SYSTEM", "ENDINDEX,SYSTEM"
                                    Dim si As Integer
                                    si = (Caller.PageCurrent + 1)
                                    If (si > 1) Then
                                        si = (Caller.PageCurrent * Caller.RecordsPerPage) + 1
                                    Else
                                        si = 1
                                    End If
                                    Dim ei As Integer = si + (Caller.RecordsPerPage - 1)
                                    If ei > Caller.TotalRecords Then
                                        ei = Caller.TotalRecords
                                    End If
                                    vr.Source = ei
                                    b = True
                                Case "*ROWS", "ROWS", "ROWS,SYSTEM"
                                    If Not DR Is Nothing Then
                                        vr.Source = DR.Table.Rows.Count
                                        b = True
                                    End If
                                Case "*TOTALPAGES", "TOTALPAGES", "TOTALPAGES,SYSTEM"
                                    vr.Source = Caller.TotalPages.ToString
                                    b = True
                                Case "*TOTALRECORDS", "TOTALRECORDS", "TOTALRECORDS,SYSTEM"
                                    vr.Source = Caller.TotalRecords.ToString
                                    b = True
                                Case "*TOTALCOLUMNS", "TOTALCOLUMNS", "TOTALCOLUMNS,SYSTEM"
                                    If Not DS Is Nothing AndAlso DS.Tables.Count > 0 Then
                                        vr.Source = DS.Tables(0).Columns.Count
                                    Else
                                        vr.Source = 0
                                    End If
                                    b = True
                                Case "*USERNAME", "USERNAME", "USERNAME,SYSTEM"
                                    If Not Caller.UserInfo Is Nothing Then
                                        If Not Caller.UserInfo.UserName Is Nothing Then
                                            vr.Source = Caller.UserInfo.UserName.ToString
                                        Else
                                            vr.Source = ""
                                        End If
                                    Else
                                        vr.Source = ""
                                    End If
                                    b = True
                                Case "*FIRSTNAME", "FIRSTNAME", "FIRSTNAME,SYSTEM"
                                    If Not Caller.UserInfo Is Nothing Then
                                        If Not Caller.UserInfo.FirstName Is Nothing Then
                                            vr.Source = Caller.UserInfo.FirstName.ToString
                                        Else
                                            vr.Source = ""
                                        End If
                                    Else
                                        vr.Source = ""
                                    End If
                                    b = True
                                Case "*LASTNAME", "LASTNAME", "LASTNAME,SYSTEM"
                                    If Not Caller.UserInfo Is Nothing Then
                                        If Not Caller.UserInfo.LastName Is Nothing Then
                                            vr.Source = Caller.UserInfo.LastName.ToString
                                        Else
                                            vr.Source = ""
                                        End If
                                    Else
                                        vr.Source = ""
                                    End If
                                    b = True
                                Case "*EMAIL", "EMAIL,SYSTEM"
                                    If Not Caller.UserInfo Is Nothing Then
                                        If Not Caller.UserInfo.Email Is Nothing Then
                                            vr.Source = Caller.UserInfo.Email.ToString
                                        Else
                                            vr.Source = ""
                                        End If
                                    Else
                                        vr.Source = ""
                                    End If
                                    b = True
                                Case "*FULLNAME", "FULLNAME", "FULLNAME,SYSTEM"
                                    If Not Caller.UserInfo Is Nothing Then
                                        If Not Caller.UserInfo.DisplayName Is Nothing Then
                                            vr.Source = Caller.UserInfo.DisplayName.ToString
                                        Else
                                            vr.Source = ""
                                        End If
                                    Else
                                        vr.Source = ""
                                    End If
                                    b = True
                                Case "*USERID", "USERID", "USERID,SYSTEM"
                                    If Not Caller.UserInfo Is Nothing Then
                                        vr.Source = Caller.UserInfo.UserId.ToString
                                    Else
                                        vr.Source = Caller.UserID.ToString
                                    End If
                                    b = True
                                Case "*DATE", "DATE", "DATE,SYSTEM"
                                    vr.Source = Now.ToString
                                    b = True
                                Case "*DATESTAMP", "DATESTAMP,SYSTEM"
                                    Dim timespanTime As TimeSpan = DateTime.UtcNow - New DateTime(1970, 1, 1)
                                    Dim arraytime() As String = timespanTime.TotalMilliseconds.ToString().Split(".")
                                    vr.Source = arraytime(0)
                                    b = True
                                Case "*URLREFERRER", "URLREFERRER", "URLREFERRER,SYSTEM"
                                    If Caller.Request Is Nothing OrElse Caller.Request.UrlReferrer Is Nothing OrElse Caller.Request.UrlReferrer.PathAndQuery Is Nothing Then
                                        vr.Source = ""
                                    Else
                                        vr.Source = Caller.Request.UrlReferrer.OriginalString
                                    End If
                                    b = True
                                Case "*FORM", "FORM", "FORM,SYSTEM"
                                    vr.Source = GetRequestForm(Caller)
                                    b = True
                                Case "*GUID", "GUID", "GUID,SYSTEM", "*UNIQUEIDENTIFIER", "UNIQUEIDENTIFIER", "UNIQUEIDENTIFIER,SYSTEM"
                                    vr.Source = Guid.NewGuid.ToString
                                    b = True
                                Case "REQUEST,SYSTEM"
                                    vr.Source = GetRequest(Caller)
                                    b = True
                                Case "*RESPONSESTATUS", "RESPONSESTATUS", "RESPONSESTATUS,SYSTEM"
                                    vr.Source = Caller.Response.Status
                                    b = True
                                Case "*RESOURCEKEY", "RESOURCEKEY", "RESOURCEKEY,SYSTEM"
                                    vr.Source = Caller.xls.ResourceKey
                                    b = True
                                Case "*RESPONSETEXT", "RESPONSETEXT", "RESPONSETEXT,SYSTEM"
                                    vr.Source = Caller.Response.StatusDescription
                                    b = True
                                Case "*OBJECTQUALIFIER", "OBJECTQUALIFIER", "OBJECTQUALIFIER,SYSTEM"
                                    vr.Source = DataProvider.Instance().ObjectQualifier
                                    b = True
                                Case "*DATABASEOWNER", "DATABASEOWNER", "DATABASEOWNER,SYSTEM"
                                    vr.Source = DataProvider.Instance().DatabaseOwner
                                    b = True
                                Case "CLEARCACHE,SYSTEM"
                                    vr.Source = ""
                                    Caller.ClearCache()
                                    b = True
                                Case "CLEARPAGECACHE,SYSTEM"
                                    vr.Source = ""
                                    Caller.ClearPageCache()
                                    b = True
                                Case "CLEARSITECACHE,SYSTEM"
                                    vr.Source = ""
                                    Caller.ClearSiteCache()
                                    b = True
                                Case "*OWSVERSION", "OWSVERSION", "OWSVERSION,SYSTEM"
                                    vr.Source = getVersion(True)
                                    b = True
                            End Select
                        End If
                    End If
                Catch ex As Exception
                    If Caller.xls.enableAdvancedParsing Then
                        b = False
                    Else
                        b = True
                    End If
                End Try
            End If

            If bSkipRendering = True Then
                Caller.AbortRendering = True
            End If
            Return b
        End Function
        Private Shared Function getVersion(ByVal withdecimals As Boolean) As String
            Static Dim sVersion As String
            Static Dim sVersionDecimals As String
            If withdecimals Then
                If sVersionDecimals Is Nothing Then
                    Try
                        Dim asb As Reflection.Assembly = Reflection.Assembly.GetExecutingAssembly
                        If Not asb Is Nothing Then
                            With asb.GetName
                                If .Version.Revision > 0 Then
                                    sVersionDecimals = .Version.Major & "." & .Version.Minor.ToString & "." & .Version.Build.ToString & "." & .Version.Revision.ToString
                                Else
                                    sVersionDecimals = .Version.Major & "." & .Version.Minor.ToString & "." & .Version.Build.ToString
                                End If
                            End With
                        Else
                            sVersionDecimals = "0"
                        End If
                    Catch ex As Exception
                        sVersionDecimals = "0"
                    End Try
                End If
                Return sVersionDecimals
            Else
                If sVersion Is Nothing Then
                    Try
                        Dim asb As Reflection.Assembly = Reflection.Assembly.GetExecutingAssembly
                        If Not asb Is Nothing Then
                            With asb.GetName
                                sVersion = .Version.Major & .Version.Minor.ToString.PadLeft(2, "0") & .Version.Build.ToString.PadLeft(2, "0") & .Version.Revision.ToString.PadLeft(2, "0")
                            End With
                        Else
                            sVersion = "0"
                        End If
                    Catch ex As Exception
                        sVersion = "0"
                    End Try
                End If
                Return sVersion
            End If
        End Function
        Public Overrides Function Handle_Render(ByRef Caller As EngineBase, ByVal Index As Integer, ByRef Source As String, ByRef DS As System.Data.DataSet, ByRef DR As System.Data.DataRow, ByRef RuntimeMessages As System.Collections.Generic.SortedList(Of String, String), ByVal NullReturn As Boolean, ByVal NullOverride As Boolean, ByVal ProtectSession As Boolean, ByVal SessionDelimiter As String, ByVal useSessionQuotes As Boolean, ByVal useAggregations As Boolean, ByRef FilterText As String, ByRef FilterField As String, ByRef Debugger As r2i.OWS.Framework.Debugger) As Boolean
            'STRUCTURE: [COLUMN/VARIABLENAME,OPTIONAL SOURCE (TABLE OR ENTITY)]
            Dim b As Boolean = False
            Dim REPLACED As Boolean = False
            Dim parameters As String() = ParameterizeString(Source, ","c, """"c, "\"c)
            Dim bSystemParse As Boolean = False
            Dim bSkipRendering As Boolean = False

            Dim vr As New VariableResult
            If Not parameters Is Nothing AndAlso parameters.Length > 1 Then
                vr.Source = Source
                vr.Name = parameters(0)
                vr.Type = parameters(1)
                If parameters.Length > 2 Then
                    Try
                        bSkipRendering = CBool(parameters(2))
                    Catch ex As Exception
                    End Try
                End If
            Else
                vr.Source = Source
                vr.Name = Source
            End If
            'KMS 1/21/09: Removed variable passage of NullReturn and NullOverride. Session returned nothing and failed. Changed to True,False. Orginal: NullReturn, NullOverride
            b = _HandleVariable(vr, bSkipRendering, Caller, Index, DS, DR, RuntimeMessages, True, False, ProtectSession, SessionDelimiter, useSessionQuotes, useAggregations, FilterText, FilterField, Debugger)
            Source = vr.Source
            Return b
        End Function

        Private Shared Function ConvertDataValueToString(ByVal FirewallDirective As Utilities.Firewall.FirewallDirectiveEnum, ByVal FirewallList As Boolean, ByRef DR As DataRow, ByRef ColumnName As String, ByRef Source As String, Optional ByVal NullOverride As Boolean = False) As Boolean
            If ColumnName.Contains(".") Then
                Dim sParts() As String = ColumnName.Split(".")

                If DR.Table.Columns.Contains(sParts(0)) Then
                    Dim obj As Object = DR.Item(sParts(0))
                    Dim sSource As String = ""

                    For idx As Integer = 1 To sParts.Length - 1
                        If sSource.Length > 0 Then sSource &= "."
                        sSource &= sParts(idx)
                    Next
                    Dim b As Boolean = GetMemberValue(obj, sSource)
                    Source = sSource
                    Firewall.Firewall(Source, False, FirewallDirective, FirewallList)
                    Return b
                End If
            Else
                If DR.Table.Columns.Contains(ColumnName) Then
                    If IsDBNull(DR.Item(ColumnName)) Then
                        If NullOverride Then
                            Source = Nothing
                            Return True
                        Else
                            Source = ""
                            Return True
                        End If
                    Else
                        If TypeOf DR.Item(ColumnName) Is System.Guid Then
                            Source = CType(DR.Item(ColumnName), System.Guid).ToString
                            Firewall.Firewall(Source, False, FirewallDirective, FirewallList)
                            Return True
                        Else
                            Source = DR.Item(ColumnName)
                            Firewall.Firewall(Source, False, FirewallDirective, FirewallList)
                            Return True
                        End If
                    End If
                End If
            End If
            Return False
        End Function

        Private Shared Function SessionVariableToString(ByVal Caller As Engine, ByVal SourceName As String, ByVal Protect As Boolean, Optional ByVal useQuotes As Boolean = True, Optional ByVal Delimiter As String = ",") As String
            Dim src As Object = Caller.Session.Item(SourceName)
            Dim output As String = ""
            If Not src Is Nothing Then
                If TypeOf src Is ArrayList OrElse TypeOf src Is Generic.List(Of String) Then
                    Dim strobj As Object
                    Dim i As Integer = 0
                    'ROMAIN:08/23/2007
                    'NOTE: Remove cast
                    'For Each strobj In CType(src, ArrayList)
                    For Each strobj In src
                        If i > 0 Then
                            output &= Delimiter
                        End If
                        If IsNumeric(strobj.ToString) Then
                            output &= strobj.ToString
                        Else
                            If Protect And useQuotes Then
                                output &= "'" & strobj.ToString.Replace("'", "''") & "'"
                            ElseIf useQuotes Then
                                output &= "'" & strobj.ToString & "'"
                            Else
                                output &= strobj.ToString
                            End If
                        End If
                        i += 1
                    Next
                Else
                    output = Utility.CNullStr(src, "Unable to Convert value to string")
                End If
            End If
            src = Nothing
            Return output
        End Function
        Private Shared Function ActionVariableToString(ByVal Caller As Engine, ByVal SourceName As String, ByVal Protect As Boolean, Optional ByVal useQuotes As Boolean = True, Optional ByVal Delimiter As String = ",") As String
            'ROMAIN: 08/22/2007
            'Dim src As Object = ActionVariable(SourceName)
            Dim src As Object = Caller.ActionVariable(SourceName)
            Dim output As String = ""
            'ROMAIN: 08/21/2007
            'Note: The ActionVariable_Get never returns an arraylist
            'If Not src Is Nothing Then
            '    If TypeOf src Is ArrayList Then
            '        Dim strobj As Object
            '        Dim i As Integer = 0
            '        For Each strobj In CType(src, ArrayList)
            '            If i > 0 Then
            '                output &= Delimiter
            '            End If
            '            If IsNumeric(strobj.ToString) Then
            '                output &= strobj.ToString
            '            Else
            '                If Protect And useQuotes Then
            '                    output &= "'" & strobj.ToString.Replace("'", "''") & "'"
            '                ElseIf useQuotes Then
            '                    output &= "'" & strobj.ToString & "'"
            '                Else
            '                    output &= strobj.ToString
            '                End If
            '            End If
            '            i += 1
            '        Next
            '    Else
            '        output = StringConvert(src)
            '    End If
            'End If

            'ROMAIN: 08/22/2007
            If Not src Is Nothing Then
                output = Utility.CNullStr(src, "Unable to Convert value to string")
            End If
            src = Nothing
            'output = src
            'src = String.Empty
            Return output
        End Function

        Private Shared Function isOfType(ByVal Value As String, ByVal DataType As String) As Boolean
            If Value Is Nothing OrElse Value.Length = 0 Then
                Return False
            Else
                If Not DataType Is Nothing AndAlso Not DataType.Length = 0 Then
                    Select Case DataType.ToUpper
                        Case "BOOLEAN"
                            If Value.ToUpper = "FALSE" Or Value.ToUpper = "TRUE" OrElse Value.ToUpper = "F" OrElse Value.ToUpper = "T" OrElse Value.ToUpper = Boolean.TrueString.ToUpper OrElse Value.ToUpper = Boolean.FalseString.ToUpper OrElse IsNumeric(Value) Then
                                Return True
                            Else
                                Return False
                            End If
                        Case "DATE"
                            Return IsDate(Value)
                        Case "GUID"
                            Try
                                Dim g As New Guid(Value)
                                If Not g.ToString = System.Guid.Empty.ToString Then
                                    Return True
                                Else
                                    Return False
                                End If

                            Catch ex As Exception
                                Return False
                            End Try
                        Case "NUMBER"
                            Return IsNumeric(Value)
                    End Select
                End If
            End If
            Return True
        End Function

        Private Shared Function ReplaceCommonVariable_Value(ByVal Value As String, ByVal DataType As String, ByVal Left As String, ByVal Right As String, ByVal Empty As String) As String
            If Not isOfType(Value, DataType) Then
                'EMPTY
                If Not Empty Is Nothing Then
                    Value = Empty
                End If
            Else
                If Not Left Is Nothing Then
                    Value = Left & Value
                End If
                If Not Right Is Nothing Then
                    Value &= Right
                End If
            End If
            Return Value
        End Function
        Public Shared Function ReplaceCommonVariables(ByVal Caller As Engine, ByRef CapturedMessages As System.Collections.Generic.SortedList(Of String, String), ByVal Source As String, ByVal FilterText As String, ByVal FilterField As String, ByRef DebugWriter As Debugger, Optional ByVal isSpecific As Boolean = False) As String
            'Replace Common Variables
            Dim Name As String = Nothing
            If isSpecific Then
                Name = Source
            End If
            Dim fparams As New StandardVariables
            With fparams
                .bSkipRendering = False
                .Caller = Caller
                .Debugger = DebugWriter
                .DR = Nothing
                .DS = Nothing
                .FilterField = FilterField
                .FilterText = FilterText
                .Index = 0
                .NullOverride = True
                .NullReturn = True
                .ProtectSession = False
                .RuntimeMessages = CapturedMessages
                .SessionDelimiter = ","
                .useAggregations = False
                .useSessionQuotes = False
            End With


            Dim qOptionItem As QueryOptionItem
            Dim keys As String() = Caller.xls.QueryItemsKeys
            'REG: 02/17/08 - xls.queryItems initializes to NULL
            If Not keys Is Nothing Then
                Dim strKey As String = Nothing
                Dim replacements As New Specialized.NameValueCollection
                For Each strKey In keys
                    Dim bname As Boolean = False
                    If Not Name Is Nothing Then
                        If strKey = Name Then
                            bname = True
                        End If
                    End If
                    If Name Is Nothing OrElse bname Then
                        If Source.Contains(strKey) Then
                            qOptionItem = Caller.xls.QueryItem(strKey)
                            Dim strvariabletarget As String = qOptionItem.QueryTarget
                            Dim strvariablename As String = qOptionItem.QuerySource
                            Dim strvariablevalue As String = ""
                            If Not strvariablename Is Nothing Then
                                strvariablename = Caller.RenderString(Nothing, strvariablename, CapturedMessages, False, False, , , , , FilterText, FilterField)
                            End If

                            Dim escapeqoutes As Boolean = False
                            Dim escapetags As Integer = 0
                            Dim escapehtml As Boolean = False
                            If strvariabletarget.ToUpper.StartsWith("ESCAPE(") Then
                                escapeqoutes = True
                                If strvariabletarget.Length > 8 Then
                                    strvariabletarget = strvariabletarget.Substring(7, strvariabletarget.Length - 8)
                                End If
                            ElseIf strvariabletarget.ToUpper.StartsWith("IGNORE(") Then
                                escapeqoutes = False
                                escapetags = 1
                                If strvariabletarget.Length > 8 Then
                                    strvariabletarget = strvariabletarget.Substring(7, strvariabletarget.Length - 8)
                                End If
                            ElseIf strvariabletarget.ToUpper.StartsWith("ESCAPEIGNORE(") Then
                                escapetags = 1
                                escapeqoutes = True
                                If strvariabletarget.Length > 14 Then
                                    strvariabletarget = strvariabletarget.Substring(13, strvariabletarget.Length - 14)
                                End If
                            End If
                            If qOptionItem.Protected Then
                                escapeqoutes = True
                            End If
                            If qOptionItem.EscapeListX > 0 Then
                                escapetags = qOptionItem.EscapeListX
                            End If
                            If qOptionItem.EscapeHTML = True Then
                                escapehtml = True
                            End If

                            If qOptionItem.VariableType.IndexOf("Session") > -1 Then
                                Dim vr As New VariableResult
                                vr.Name = strvariablename
                                vr.Source = ""
                                vr.Type = "Session"
                                _HandleVariable(vr, False, Caller, 0, Nothing, Nothing, CapturedMessages, True, False, False, ",", False, True, FilterText, FilterField, Nothing)
                                'This is a Session Variable
                                'strvariablevalue = FormatQueryVariable(SessionVariableToString(Caller, strvariablename, False), escapetags, escapeqoutes, escapehtml, qOptionItem.QueryTargetLeft, qOptionItem.QueryTargetRight, qOptionItem.QueryTargetEmpty, qOptionItem.VariableDataType)
                                strvariablevalue = FormatQueryVariable(fparams, vr.Source, escapetags, escapeqoutes, escapehtml, qOptionItem.QueryTargetLeft, qOptionItem.QueryTargetRight, qOptionItem.QueryTargetEmpty, qOptionItem.VariableDataType, qOptionItem.Formatters)
                                vr = Nothing
                            ElseIf qOptionItem.VariableType.IndexOf("Action") > -1 Then
                                Dim vr As New VariableResult
                                vr.Name = strvariablename
                                vr.Source = ""
                                vr.Type = "Action"
                                _HandleVariable(vr, False, Caller, 0, Nothing, Nothing, CapturedMessages, True, False, False, ",", False, True, FilterText, FilterField, Nothing)

                                'Source = Source.Replace(strvariabletarget, FormatQueryVariable(ActionVariableToString(Caller, strvariablename, False), escapetags, escapeqoutes, escapehtml, qOptionItem.QueryTargetLeft, qOptionItem.QueryTargetRight, qOptionItem.QueryTargetEmpty))
                                'strvariablevalue = FormatQueryVariable(ActionVariableToString(Caller, strvariablename, False), escapetags, escapeqoutes, escapehtml, qOptionItem.QueryTargetLeft, qOptionItem.QueryTargetRight, qOptionItem.QueryTargetEmpty, qOptionItem.VariableDataType)
                                strvariablevalue = FormatQueryVariable(fparams, vr.Source, escapetags, escapeqoutes, escapehtml, qOptionItem.QueryTargetLeft, qOptionItem.QueryTargetRight, qOptionItem.QueryTargetEmpty, qOptionItem.VariableDataType, qOptionItem.Formatters)
                                vr = Nothing
                            ElseIf qOptionItem.VariableType.IndexOf("QueryString") > -1 Then
                                Dim vr As New VariableResult
                                vr.Name = strvariablename
                                vr.Source = ""
                                vr.Type = "Querystring"
                                _HandleVariable(vr, False, Caller, 0, Nothing, Nothing, CapturedMessages, True, False, False, ",", False, True, FilterText, FilterField, Nothing)

                                'Source = Source.Replace(strvariabletarget, FormatQueryVariable(FormatQueryVariable(Request.QueryString.Item(strvariablename), escapetags, qOptionItem.Protected, escapehtml, "", "", ""), False, escapeqoutes, False, qOptionItem.QueryTargetLeft, qOptionItem.QueryTargetRight, qOptionItem.QueryTargetEmpty))
                                'Source = Source.Replace(strvariabletarget, FormatQueryVariable(FormatQueryVariable(Caller.Request.QueryString.Item(strvariablename), 0, False, False, "", "", ""), escapetags, escapeqoutes, escapehtml, qOptionItem.QueryTargetLeft, qOptionItem.QueryTargetRight, qOptionItem.QueryTargetEmpty))
                                'strvariablevalue = FormatQueryVariable(FormatQueryVariable(Caller.Request.QueryString.Item(strvariablename), 0, False, False, "", "", "", "Any"), escapetags, escapeqoutes, escapehtml, qOptionItem.QueryTargetLeft, qOptionItem.QueryTargetRight, qOptionItem.QueryTargetEmpty, qOptionItem.VariableDataType)
                                strvariablevalue = FormatQueryVariable(fparams, FormatQueryVariable(fparams, vr.Source, 0, False, False, "", "", "", "Any", ""), escapetags, escapeqoutes, escapehtml, qOptionItem.QueryTargetLeft, qOptionItem.QueryTargetRight, qOptionItem.QueryTargetEmpty, qOptionItem.VariableDataType, qOptionItem.Formatters)
                                vr = Nothing
                            ElseIf qOptionItem.VariableType.IndexOf("Form") > -1 Then
                                Dim vr As New VariableResult
                                vr.Name = strvariablename
                                vr.Source = ""
                                vr.Type = "Form"
                                _HandleVariable(vr, False, Caller, 0, Nothing, Nothing, CapturedMessages, True, False, False, ",", False, True, FilterText, FilterField, Nothing)

                                'Source = Source.Replace(strvariabletarget, FormatQueryVariable(FormatQueryVariable(Request.Form.Item(strvariablename), escapetags, qOptionItem.Protected, escapehtml, "", "", ""), False, escapeqoutes, False, qOptionItem.QueryTargetLeft, qOptionItem.QueryTargetRight, qOptionItem.QueryTargetEmpty))
                                'Source = Source.Replace(strvariabletarget, FormatQueryVariable(FormatQueryVariable(Caller.Request.Form.Item(strvariablename), 0, False, False, "", "", ""), escapetags, escapeqoutes, escapehtml, qOptionItem.QueryTargetLeft, qOptionItem.QueryTargetRight, qOptionItem.QueryTargetEmpty))
                                'strvariablevalue = FormatQueryVariable(FormatQueryVariable(Caller.Request.Form.Item(strvariablename), 0, False, False, "", "", "", "Any"), escapetags, escapeqoutes, escapehtml, qOptionItem.QueryTargetLeft, qOptionItem.QueryTargetRight, qOptionItem.QueryTargetEmpty, qOptionItem.VariableDataType)
                                strvariablevalue = FormatQueryVariable(fparams, FormatQueryVariable(fparams, vr.Source, 0, False, False, "", "", "", "Any", ""), escapetags, escapeqoutes, escapehtml, qOptionItem.QueryTargetLeft, qOptionItem.QueryTargetRight, qOptionItem.QueryTargetEmpty, qOptionItem.VariableDataType, qOptionItem.Formatters)
                                vr = Nothing
                            ElseIf qOptionItem.VariableType.IndexOf("Cookie") > -1 Then
                                Dim vr As New VariableResult
                                vr.Name = strvariablename
                                vr.Source = ""
                                vr.Type = "Cookie"
                                _HandleVariable(vr, False, Caller, 0, Nothing, Nothing, CapturedMessages, True, False, False, ",", False, True, FilterText, FilterField, Nothing)

                                'Dim ck As Web.HttpCookie = Caller.Request.Cookies(strvariablename)
                                'Dim ckstr As String = ""
                                'If Not ck Is Nothing Then
                                '    ckstr = ck.Value
                                'End If
                                'Source = Source.Replace(strvariabletarget, FormatQueryVariable(FormatQueryVariable(ckstr, 0, False, False, "", "", ""), escapetags, escapeqoutes, escapehtml, qOptionItem.QueryTargetLeft, qOptionItem.QueryTargetRight, qOptionItem.QueryTargetEmpty))
                                'strvariablevalue = FormatQueryVariable(FormatQueryVariable(ckstr, 0, False, False, "", "", "", "Any"), escapetags, escapeqoutes, escapehtml, qOptionItem.QueryTargetLeft, qOptionItem.QueryTargetRight, qOptionItem.QueryTargetEmpty, qOptionItem.VariableDataType)
                                strvariablevalue = FormatQueryVariable(fparams, FormatQueryVariable(fparams, vr.Source, 0, False, False, "", "", "", "Any", ""), escapetags, escapeqoutes, escapehtml, qOptionItem.QueryTargetLeft, qOptionItem.QueryTargetRight, qOptionItem.QueryTargetEmpty, qOptionItem.VariableDataType, qOptionItem.Formatters)
                                vr = Nothing
                            ElseIf qOptionItem.VariableType.IndexOf("Cache") > -1 Then
                                Dim vr As New VariableResult
                                vr.Name = strvariablename
                                vr.Source = ""
                                vr.Type = "Cache"
                                _HandleVariable(vr, False, Caller, 0, Nothing, Nothing, CapturedMessages, True, False, False, ",", False, True, FilterText, FilterField, Nothing)
                                strvariablevalue = FormatQueryVariable(fparams, FormatQueryVariable(fparams, vr.Source, 0, False, False, "", "", "", "Any", ""), escapetags, escapeqoutes, escapehtml, qOptionItem.QueryTargetLeft, qOptionItem.QueryTargetRight, qOptionItem.QueryTargetEmpty, qOptionItem.VariableDataType, qOptionItem.Formatters)

                                'strvariablevalue = FormatQueryVariable(FormatQueryVariable(Caller.Context.Cache.Item(strvariablename), 0, False, False, "", "", "", "Any"), escapetags, escapeqoutes, escapehtml, qOptionItem.QueryTargetLeft, qOptionItem.QueryTargetRight, qOptionItem.QueryTargetEmpty, qOptionItem.VariableDataType)
                            ElseIf qOptionItem.VariableType.IndexOf("Context") > -1 Then
                                Dim vr As New VariableResult
                                vr.Name = strvariablename
                                vr.Source = ""
                                vr.Type = "Context"
                                _HandleVariable(vr, False, Caller, 0, Nothing, Nothing, CapturedMessages, True, False, False, ",", False, True, FilterText, FilterField, Nothing)
                                strvariablevalue = FormatQueryVariable(fparams, FormatQueryVariable(fparams, vr.Source, 0, False, False, "", "", "", "Any", ""), escapetags, escapeqoutes, escapehtml, qOptionItem.QueryTargetLeft, qOptionItem.QueryTargetRight, qOptionItem.QueryTargetEmpty, qOptionItem.VariableDataType, qOptionItem.Formatters)

                                'strvariablevalue = FormatQueryVariable(FormatQueryVariable(Caller.Context.Items.Item(strvariablename), 0, False, False, "", "", "", "Any"), escapetags, escapeqoutes, escapehtml, qOptionItem.QueryTargetLeft, qOptionItem.QueryTargetRight, qOptionItem.QueryTargetEmpty, qOptionItem.VariableDataType)
                            ElseIf qOptionItem.VariableType.IndexOf("ViewState") > -1 Then
                                Dim vr As New VariableResult
                                vr.Name = strvariablename
                                vr.Source = ""
                                vr.Type = "Viewstate"
                                _HandleVariable(vr, False, Caller, 0, Nothing, Nothing, CapturedMessages, True, False, False, ",", False, True, FilterText, FilterField, Nothing)
                                strvariablevalue = FormatQueryVariable(fparams, FormatQueryVariable(fparams, vr.Source, 0, False, False, "", "", "", "Any", ""), escapetags, escapeqoutes, escapehtml, qOptionItem.QueryTargetLeft, qOptionItem.QueryTargetRight, qOptionItem.QueryTargetEmpty, qOptionItem.VariableDataType, qOptionItem.Formatters)

                                'strvariablevalue = FormatQueryVariable(FormatQueryVariable(Caller.ViewState.Item(strvariablename), 0, False, False, "", "", "", "Any"), escapetags, escapeqoutes, escapehtml, qOptionItem.QueryTargetLeft, qOptionItem.QueryTargetRight, qOptionItem.QueryTargetEmpty, qOptionItem.VariableDataType)
                            ElseIf qOptionItem.VariableType.IndexOf("Custom") > -1 Then
                                'Source = Source.Replace(strvariabletarget, FormatQueryVariable(FormatQueryVariable(strvariablename, escapetags, qOptionItem.Protected, escapehtml, "", "", ""), False, escapeqoutes, False, qOptionItem.QueryTargetLeft, qOptionItem.QueryTargetRight, qOptionItem.QueryTargetEmpty))
                                'Source = Source.Replace(strvariabletarget, FormatQueryVariable(FormatQueryVariable(strvariablename, 0, False, False, "", "", ""), escapetags, escapeqoutes, escapehtml, qOptionItem.QueryTargetLeft, qOptionItem.QueryTargetRight, qOptionItem.QueryTargetEmpty))
                                strvariablevalue = FormatQueryVariable(fparams, FormatQueryVariable(fparams, strvariablename, 0, False, False, "", "", "", "", ""), escapetags, escapeqoutes, escapehtml, qOptionItem.QueryTargetLeft, qOptionItem.QueryTargetRight, qOptionItem.QueryTargetEmpty, qOptionItem.VariableDataType, qOptionItem.Formatters)
                            ElseIf qOptionItem.VariableType.IndexOf("Message") > -1 Then
                                Dim vr As New VariableResult
                                vr.Name = strvariablename
                                vr.Source = ""
                                vr.Type = "Message"
                                _HandleVariable(vr, False, Caller, 0, Nothing, Nothing, CapturedMessages, True, False, False, ",", False, True, FilterText, FilterField, Nothing)
                                strvariablevalue = FormatQueryVariable(fparams, FormatQueryVariable(fparams, vr.Source, 0, False, False, "", "", "", "Any", ""), escapetags, escapeqoutes, escapehtml, qOptionItem.QueryTargetLeft, qOptionItem.QueryTargetRight, qOptionItem.QueryTargetEmpty, qOptionItem.VariableDataType, qOptionItem.Formatters)

                                'Dim val As String = ""
                                'If Not CapturedMessages Is Nothing AndAlso CapturedMessages.ContainsKey(qOptionItem.QuerySource) Then
                                '    val = CapturedMessages.Item(qOptionItem.QuerySource)
                                'End If
                                ''Source = Source.Replace(strvariabletarget, FormatQueryVariable(FormatQueryVariable(val, escapetags, qOptionItem.Protected, escapehtml, "", "", ""), False, escapeqoutes, False, qOptionItem.QueryTargetLeft, qOptionItem.QueryTargetRight, qOptionItem.QueryTargetEmpty))
                                ''Source = Source.Replace(strvariabletarget, FormatQueryVariable(FormatQueryVariable(val, 0, False, False, "", "", ""), escapetags, escapeqoutes, escapehtml, qOptionItem.QueryTargetLeft, qOptionItem.QueryTargetRight, qOptionItem.QueryTargetEmpty))
                                'strvariablevalue = FormatQueryVariable(FormatQueryVariable(val, 0, False, False, "", "", "", "Any"), escapetags, escapeqoutes, escapehtml, qOptionItem.QueryTargetLeft, qOptionItem.QueryTargetRight, qOptionItem.QueryTargetEmpty, qOptionItem.VariableDataType)
                            Else
                                'This is a module variable
                                Select Case qOptionItem.VariableType.ToUpper
                                    Case "MODULEID"
                                        strvariablevalue = FormatQueryVariable(fparams, Caller.ModuleID, escapetags, False, escapehtml, qOptionItem.QueryTargetLeft, qOptionItem.QueryTargetRight, qOptionItem.QueryTargetEmpty, qOptionItem.VariableDataType, qOptionItem.Formatters)
                                    Case "PORTALID"
                                        strvariablevalue = FormatQueryVariable(fparams, Caller.PortalID, escapetags, False, escapehtml, qOptionItem.QueryTargetLeft, qOptionItem.QueryTargetRight, qOptionItem.QueryTargetEmpty, qOptionItem.VariableDataType, qOptionItem.Formatters)
                                    Case "TABID"
                                        strvariablevalue = FormatQueryVariable(fparams, Caller.TabID, escapetags, False, escapehtml, qOptionItem.QueryTargetLeft, qOptionItem.QueryTargetRight, qOptionItem.QueryTargetEmpty, qOptionItem.VariableDataType, qOptionItem.Formatters)
                                    Case "USERID"
                                        strvariablevalue = FormatQueryVariable(fparams, Caller.UserID, escapetags, False, escapehtml, qOptionItem.QueryTargetLeft, qOptionItem.QueryTargetRight, qOptionItem.QueryTargetEmpty, qOptionItem.VariableDataType, qOptionItem.Formatters)
                                    Case "PORTALALIAS"
                                        'Source = Source.Replace(strvariabletarget, FormatQueryVariable(PortalSettings.PortalAlias.HTTPAlias, escapetags, escapeqoutes, escapehtml, qOptionItem.QueryTargetLeft, qOptionItem.QueryTargetRight, qOptionItem.QueryTargetEmpty))
                                        strvariablevalue = FormatQueryVariable(fparams, Caller.PortalSettings.HTTPAlias, escapetags, escapeqoutes, escapehtml, qOptionItem.QueryTargetLeft, qOptionItem.QueryTargetRight, qOptionItem.QueryTargetEmpty, qOptionItem.VariableDataType, qOptionItem.Formatters)
                                    Case "PAGENUMBER"
                                        strvariablevalue = FormatQueryVariable(fparams, (Caller.PageCurrent + 1).ToString, escapetags, False, escapehtml, qOptionItem.QueryTargetLeft, qOptionItem.QueryTargetRight, qOptionItem.QueryTargetEmpty, qOptionItem.VariableDataType, qOptionItem.Formatters)
                                    Case "PAGESIZE"
                                        strvariablevalue = FormatQueryVariable(fparams, Caller.RecordsPerPage, escapetags, False, escapehtml, qOptionItem.QueryTargetLeft, qOptionItem.QueryTargetRight, qOptionItem.QueryTargetEmpty, qOptionItem.VariableDataType, qOptionItem.Formatters)
                                    Case "OWNER"
                                        'ROMAIN: 09/18/07
                                        'Dim ProviderType As String = "data"
                                        'Dim objConfiguration As DotNetNuke.Framework.Providers.ProviderConfiguration = DotNetNuke.Framework.Providers.ProviderConfiguration.GetProviderConfiguration(ProviderType)
                                        'Dim objProvider As DotNetNuke.Framework.Providers.Provider = CType(objConfiguration.Providers(objConfiguration.DefaultProvider), DotNetNuke.Framework.Providers.Provider)
                                        'Dim strOwner As String

                                        'strOwner = objProvider.Attributes("databaseOwner")
                                        Dim strOwner As String = AbstractFactory.Instance.EngineController.ProviderOwner()
                                        If strOwner <> "" And strOwner.EndsWith(".") = False Then
                                            strOwner += "."
                                        End If

                                        strvariablevalue = FormatQueryVariable(fparams, strOwner, escapetags, False, escapehtml, qOptionItem.QueryTargetLeft, qOptionItem.QueryTargetRight, qOptionItem.QueryTargetEmpty, qOptionItem.VariableDataType, qOptionItem.Formatters)
                                    Case "QUALIFIER"
                                        Dim ProviderType As String = "data"
                                        'Dim objConfiguration As DotNetNuke.Framework.Providers.ProviderConfiguration = DotNetNuke.Framework.Providers.ProviderConfiguration.GetProviderConfiguration(ProviderType)
                                        Dim objConfiguration As IProviderConfiguration = AbstractFactory.Instance.ProviderConfigurationController.GetProviderConfiguration(ProviderType)
                                        'Dim objProvider As DotNetNuke.Framework.Providers.Provider = CType(objConfiguration.Providers(objConfiguration.DefaultProvider), DotNetNuke.Framework.Providers.Provider)
                                        Dim objProvider As IProvider = CType(objConfiguration.Providers(objConfiguration.DefaultProvider), IProvider)
                                        Dim strQualifier As String


                                        strQualifier = objProvider.Attributes("objectQualifier")
                                        If strQualifier <> "" And strQualifier.EndsWith("_") = False Then
                                            strQualifier += "_"
                                        End If

                                        strvariablevalue = FormatQueryVariable(fparams, strQualifier, escapetags, False, escapehtml, qOptionItem.QueryTargetLeft, qOptionItem.QueryTargetRight, qOptionItem.QueryTargetEmpty, qOptionItem.VariableDataType, qOptionItem.Formatters)
                                End Select
                            End If
                            replacements.Add(strvariabletarget, strvariablevalue)
                        End If
                    End If
                    If bname Then
                        Exit For
                    End If
                Next
                Source = RenderVariable.Replace(replacements, Source, 0)
                replacements.Clear()
                replacements = Nothing
            End If
            'If Not DebugWriter Is Nothing Then
            '    Try
            '        Dim ticks As Integer = DebugWriter.getCounter
            '        DebugWriter.Write("<div style=""margin-top: 2px; border: 1px dotted black; background: #EFFFEF; color: black; font-family: arial; font-size: 11px;""><span style=""font-weight: bold;"">After Variable Replacement:</span><a href=""javascript:xdbg" & Me.ModuleID & "('" & ticks & "');"">Expand/Collapse</a><br><div id=""xDbg" & Me.ModuleID & "x" & ticks & """ style=""display: none; width: 100%;""><pre>" & Source.Replace("<", "&lt;").Replace(">", "&gt;") & "</pre></div></div>")
            '    Catch ex As Exception
            '    End Try
            'End If
            Return Source
        End Function

        Private Shared Function Replace(ByRef Parameters As Specialized.NameValueCollection, ByVal Source As String, ByVal i As Integer) As String
            Dim str As String = ""
            While i < Parameters.Count
                Dim key As String = Parameters.Keys(i)
                Dim value As String = Parameters(key)
                Dim skey(0) As String
                skey(0) = key
                i += 1
                Dim s() As String = Source.Split(skey, StringSplitOptions.None)
                If i < Parameters.Count Then
                    Dim k As Integer
                    For k = 0 To s.Length - 1
                        s(k) = Replace(Parameters, s(k), i)
                    Next
                End If
                If s.Length > 0 Then
                    Return String.Join(value, s)
                Else
                    Return Source
                End If
            End While
            Return Source
        End Function

        Private Shared Function GetRequestForm(ByVal Caller As Engine) As String
            Dim sForm As String = ""

            Try
                If Not Caller.Request.Form Is Nothing Then
                    'ROMAIN: Generic replacement - 08/20/2007
                    'Dim arrKeys As New ArrayList(Me.Request.Form.AllKeys)
                    Dim arrKeys As New List(Of String)(Caller.Request.Form.AllKeys)
                    Dim sKey As String
                    Dim sValue As String

                    For Each sKey In arrKeys
                        If Not Utility.CNullStr(sKey) = "" AndAlso sKey.ToUpper <> "__EVENTTARGET" AndAlso sKey.ToUpper <> "__EVENTARGUMENT" AndAlso sKey.ToUpper <> "__VIEWSTATE" Then
                            sValue = Utility.URLEncode(Utility.CNullStr(Caller.Request.Form.Item(sKey)))
                            If sForm <> "" Then sForm &= "&"
                            sForm &= (sKey & "=" & sValue)
                        End If
                    Next
                End If
            Catch ex As Exception
                'TODO: Change Exception 
                'DotNetNuke.Services.Exceptions.LogException(ex)
            End Try
            Return sForm
        End Function


        Private Shared Function GetRequest(ByVal Caller As Engine) As String
            Dim sValue As String = ""

            Try
                If Not Caller.Request.InputStream Is Nothing AndAlso Caller.Request.InputStream.CanRead Then
                    Dim sReader As New IO.StreamReader(Caller.Request.InputStream)
                    Caller.Request.InputStream.Position = 0
                    sValue = sReader.ReadToEnd()
                    Caller.Request.InputStream.Position = 0
                    'ROMAIN: Generic replacement - 08/20/2007
                    'Dim arrKeys As New ArrayList(Me.Request.Form.AllKeys)
                End If
            Catch ex As Exception
                sValue = ex.ToString
                'TODO: Change Exception 
                'DotNetNuke.Services.Exceptions.LogException(ex)
            End Try
            Return sValue
        End Function


        Private Shared Function GetMemberValue(ByVal Caller As Engine, ByRef Source As String) As Boolean
            Dim b As Boolean = False
            Dim ObjV As Object = Caller
            Dim srcString As String = Source
            'STRIP THE LISTX FORMATTING
            If srcString.StartsWith("*") Then
                srcString = srcString.Remove(0, 1)
            End If
            If srcString.ToUpper.EndsWith(",SYSTEM") Then
                srcString = srcString.Remove(Source.Length - 7, 7)
            End If

            Dim isUser As Boolean = False
            Dim userKey As String = ""
            Dim strSource() As String = srcString.Split(".")
            Dim strV As String
            Dim iV As Integer = 0
            Dim abort As Boolean = False
            If strSource.Length > 1 And strSource(iV).ToUpper = "ME" Then
                iV += 1
            End If
            If strSource.Length > 0 Then
                Select Case strSource(iV).ToUpper
                    Case "USER", "USERINFO"
                        If Not Caller.UserInfo Is Nothing Then
                            isUser = True
                            ObjV = Caller.UserInfo
                        Else
                            isUser = False
                            Source = ""
                            b = True
                            abort = True
                        End If
                        iV += 1
                    Case "EDITUSER", "EDITUSERINFO"
                        isUser = True
                        ObjV = Caller.EditUserInfo
                        iV += 1
                    Case "ACTION"
                        srcString = srcString.Remove(0, 7)
                        Dim colI As Integer = srcString.IndexOf(".")
                        If (colI > 0) Then
                            Dim coll As String = srcString.Substring(0, colI)
                            ObjV = Caller.ActionVariable(coll)
                            iV += 2
                        End If
                    Case "PAGE"
                            srcString = srcString.Remove(0, 5)
                            Dim coll As String = srcString.Substring(0, srcString.IndexOf("."))
                            Select Case coll.ToUpper
                                Case "HEADER"
                                    srcString = srcString.Substring(srcString.IndexOf(".") + 1, srcString.Length - 1 - srcString.IndexOf("."))
                                    Dim val As String = Nothing
                                    Try
                                        val = Caller.Request.Headers.Get(srcString)
                                    Catch ex As Exception

                                    End Try
                                    If val Is Nothing Then
                                        val = ""
                                    End If
                                    Source = val
                                    b = True
                                Case "META"
                                    srcString = srcString.Substring(srcString.IndexOf(".") + 1, srcString.Length - 1 - srcString.IndexOf("."))
                                    Select Case srcString.ToUpper
                                        Case "DESCRIPTION"
                                            Source = AbstractFactory.Instance.PageInfoController.GetPageInfo(Caller.Caller.Page).Description
                                            b = True
                                        Case "KEYWORDS"
                                            Source = AbstractFactory.Instance.PageInfoController.GetPageInfo(Caller.Caller.Page).Keywords
                                            b = True
                                        Case "AUTHOR"
                                            Source = AbstractFactory.Instance.PageInfoController.GetPageInfo(Caller.Caller.Page).Author
                                            b = True
                                        Case "COPYRIGHT"
                                            Source = AbstractFactory.Instance.PageInfoController.GetPageInfo(Caller.Caller.Page).Copyright
                                            b = True
                                        Case "GENERATOR"
                                            Source = AbstractFactory.Instance.PageInfoController.GetPageInfo(Caller.Caller.Page).Generator
                                            b = True
                                        Case Else
                                            Source = AbstractFactory.Instance.PageInfoController.GetPageInfo(Caller.Caller.Page).GetMetaProperty(srcString)
                                            b = True
                                    End Select
                            End Select
                            abort = True
                End Select

                While iV < strSource.Length And Not abort
                    Dim key As String = Nothing
                    strV = strSource(iV)
                    If isUser Then
                        userKey &= strV
                        If iV + 1 < strSource.Length Then
                            userKey &= "."
                        End If
                    Else
                        If iV + 1 < strSource.Length AndAlso strSource(iV + 1).StartsWith("(") Then
                            iV += 1
                            key = strSource(iV).Substring(1, strSource(iV).Length - 2)
                        End If
                    End If
                    Dim mInfos As System.Reflection.MemberInfo() = ObjV.GetType.GetMember(strV)
                    'SINCE NOT ALL USERS WILL GET THE REFLECTION CORRECT, OFFER A LESS OPTIMIZED LOOKUP
                    If mInfos Is Nothing OrElse mInfos.Length = 0 Then
                        mInfos = ObjV.GetType.GetMember(strV, Reflection.BindingFlags.IgnoreCase Or Reflection.BindingFlags.Instance Or Reflection.BindingFlags.Public)
                    End If
                    abort = True
                    If Not mInfos Is Nothing AndAlso mInfos.Length > 0 Then
                        Dim mInfo As System.Reflection.MemberInfo = mInfos(0)
                        Select Case mInfo.MemberType
                            Case Reflection.MemberTypes.Constructor
                                'DO NOTHING
                            Case Reflection.MemberTypes.Custom
                                'DO NOTHING
                            Case Reflection.MemberTypes.Event
                                'Dim mTobj As System.Reflection.EventInfo
                                'mTobj = objV.GetType.GetEvent(strV)
                                'DO NOTHING
                            Case Reflection.MemberTypes.Field
                                Dim mTobj As System.Reflection.FieldInfo
                                mTobj = ObjV.GetType.GetField(mInfos(0).Name)
                                ObjV = mTobj.GetValue(ObjV)
                                abort = False
                            Case Reflection.MemberTypes.Method
                                abort = False
                                Dim mTobj As System.Reflection.MethodInfo
                                mTobj = ObjV.GetType.GetMethod(mInfos(0).Name, New Type() {})
                                'Dim param As System.Reflection.ParameterInfo
                                'For Each param In mtObj.GetParameters()
                                '    If param.IsIn Then
                                '        abort = True
                                '    End If
                                'Next
                                If Not abort Then
                                    ObjV = mTobj.Invoke(ObjV, Nothing)
                                End If
                            Case Reflection.MemberTypes.NestedType
                                'DO NOTHING
                            Case Reflection.MemberTypes.Property
                                abort = False
                                Dim mTobj As System.Reflection.PropertyInfo
                                mTobj = ObjV.GetType.GetProperty(mInfos(0).Name, New Type() {})

                                If Not abort Then
                                    If mTobj Is Nothing Then
                                        Dim nextValue As String = Nothing
                                        If iV < strSource.Length Then
                                            Dim keyObj As Object = Nothing
                                            Dim identifiedType As Boolean = False

                                            If Not identifiedType AndAlso IsNumeric(key) AndAlso Not key.Contains(".") Then
                                                If mTobj Is Nothing Then
                                                    mTobj = ObjV.GetType.GetProperty(mInfos(0).Name, New Type() {GetType(Integer)})
                                                    If Not mTobj Is Nothing Then
                                                        keyObj = CType(key, Integer)
                                                    End If
                                                End If
                                                If mTobj Is Nothing Then
                                                    mTobj = ObjV.GetType.GetProperty(mInfos(0).Name, New Type() {GetType(Int16)})
                                                    If Not mTobj Is Nothing Then
                                                        keyObj = CType(key, Int16)
                                                    End If
                                                End If
                                                If mTobj Is Nothing Then
                                                    mTobj = ObjV.GetType.GetProperty(mInfos(0).Name, New Type() {GetType(Int32)})
                                                    If Not mTobj Is Nothing Then
                                                        keyObj = CType(key, Int32)
                                                    End If
                                                End If
                                                If mTobj Is Nothing Then
                                                    mTobj = ObjV.GetType.GetProperty(mInfos(0).Name, New Type() {GetType(Int64)})
                                                    If Not mTobj Is Nothing Then
                                                        keyObj = CType(key, Int64)
                                                    End If
                                                End If
                                                If mTobj Is Nothing Then
                                                    mTobj = ObjV.GetType.GetProperty(mInfos(0).Name, New Type() {GetType(Single)})
                                                    If Not mTobj Is Nothing Then
                                                        keyObj = CType(key, Single)
                                                    End If
                                                End If
                                                If Not mTobj Is Nothing Then
                                                    identifiedType = True
                                                End If
                                            End If
                                            If Not identifiedType AndAlso IsNumeric(key) AndAlso key.Contains(".") Then
                                                If mTobj Is Nothing Then
                                                    mTobj = ObjV.GetType.GetProperty(mInfos(0).Name, New Type() {GetType(Double)})
                                                    If Not mTobj Is Nothing Then
                                                        keyObj = CType(key, Double)
                                                    End If
                                                End If
                                                If Not mTobj Is Nothing Then
                                                    identifiedType = True
                                                End If
                                            End If
                                            If Not identifiedType AndAlso IsDate(key) Then
                                                If mTobj Is Nothing Then
                                                    mTobj = ObjV.GetType.GetProperty(mInfos(0).Name, New Type() {GetType(Date)})
                                                    If Not mTobj Is Nothing Then
                                                        keyObj = CType(key, Date)
                                                    End If
                                                End If
                                                If Not mTobj Is Nothing Then
                                                    identifiedType = True
                                                End If
                                            End If
                                            If Not identifiedType Then
                                                If mTobj Is Nothing Then
                                                    mTobj = ObjV.GetType.GetProperty(mInfos(0).Name, New Type() {GetType(String)})
                                                    If Not mTobj Is Nothing Then
                                                        keyObj = CType(key, String)
                                                    End If
                                                End If
                                                If Not mTobj Is Nothing Then
                                                    identifiedType = True
                                                End If
                                            End If
                                            If Not mTobj Is Nothing Then
                                                'abort = True
                                                ObjV = mTobj.GetValue(ObjV, New Object() {keyObj})
                                            End If
                                        End If
                                    Else
                                        ObjV = mTobj.GetValue(ObjV, Nothing)
                                    End If
                                End If
                            Case Reflection.MemberTypes.TypeInfo
                                'DO NOTHING
                        End Select
                    ElseIf isUser Then
                        abort = True
                        If TypeOf ObjV Is IUser Then

                            iV += 1
                            Dim besc As Boolean = False
                            Dim objU As Object = ObjV

                            While Not besc And iV < strSource.Length
                                userKey &= strSource(iV)
                                iV += 1

                                If CType(objU, IUser).Properties.ContainsKey(userKey) Then
                                    abort = False
                                    ObjV = CType(objU, IUser).Property(userKey)
                                    If ObjV Is Nothing Then
                                        ObjV = ""
                                        besc = True
                                    End If
                                Else

                                    Dim valid As Boolean = False
                                    If iV < strSource.Length Then
                                        'make sure the entire key isnt present
                                        Dim fullkey As String = userKey
                                        Dim iFV As Integer = iV
                                        While iFV < strSource.Length
                                            fullkey &= "." & strSource(iFV)
                                            iFV += 1
                                        End While
                                        If CType(objU, IUser).Properties.ContainsKey(fullkey) Then
                                            iV = iFV
                                            ObjV = CType(objU, IUser).Property(fullkey)
                                            valid = True
                                            besc = True
                                            abort = False
                                        End If
                                    End If

                                    If Not valid Then
                                        If Not abort Then
                                            iV -= 2
                                        End If
                                        besc = True
                                    End If
                                End If


                                If Not besc AndAlso iV < strSource.Length Then
                                    userKey &= "."
                                End If


                            End While
                        End If
                    ElseIf TypeOf (ObjV) Is r2i.OWS.Newtonsoft.Json.JavaScriptArray Then
                        Dim pi As r2i.OWS.Newtonsoft.Json.JavaScriptArray = ObjV
                        Dim position As Integer = -1
                        If strSource.Length > iV Then
                            If IsNumeric(strV) Then
                                position = CInt(strV)
                            Else
                                iV -= 1
                                position = 0
                            End If
                            If (pi.Count > 0 AndAlso position >= 0 AndAlso position < pi.Count) Then
                                ObjV = pi.Item(position)
                                abort = False
                            Else
                                abort = True
                            End If
                        Else
                            ObjV = pi.Count
                            abort = False
                        End If
                    ElseIf TypeOf (ObjV) Is r2i.OWS.Newtonsoft.Json.JavaScriptObject Then
                        Dim pi As r2i.OWS.Newtonsoft.Json.JavaScriptObject = ObjV
                        If pi.ContainsKey(strV) Then
                            ObjV = pi.Item(strV)
                            If Not ObjV Is Nothing AndAlso TypeOf (ObjV) Is r2i.OWS.Newtonsoft.Json.JavaScriptArray Then
                                If iV + 1 >= strSource.Length Then
                                    ObjV = CType(ObjV, r2i.OWS.Newtonsoft.Json.JavaScriptArray).Count
                                End If
                            End If
                            abort = False
                        End If
                    Else
                        'This may reference a key in a dictionary, or something
                        For Each pi As PropertyInfo In ObjV.GetType().GetProperties()
                            If pi.PropertyType Is GetType(IDictionary) Then
                                Dim dict As IDictionary = pi.GetValue(ObjV, Nothing)
                                If dict.Contains(strV) Then
                                    ObjV = dict.Item(strV)
                                    abort = False
                                    Exit For
                                End If
                            ElseIf pi.PropertyType Is GetType(IDictionary(Of String, Object)) Then
                                Dim dict As IDictionary(Of String, Object) = pi.GetValue(ObjV, Nothing)
                                If dict.ContainsKey(strV) Then
                                    ObjV = dict.Item(strV)
                                    abort = False
                                    Exit For
                                End If
                            End If
                        Next
                    End If
                    iV += 1
                End While
                If Not abort And Not ObjV Is Nothing AndAlso Not ObjV.GetType.Name = Caller.GetType.Name Then
                    Source = Utility.CNullStr(ObjV, "")
                    b = True
                End If
            End If


            Return b
        End Function

        Public Structure StandardVariables
            Public bSkipRendering As Boolean
            Public Caller As EngineBase
            Public Index As Integer
            Public DS As System.Data.DataSet
            Public DR As System.Data.DataRow
            Public RuntimeMessages As System.Collections.Generic.SortedList(Of String, String)
            Public NullReturn As Boolean
            Public NullOverride As Boolean
            Public ProtectSession As Boolean
            Public SessionDelimiter As String
            Public useSessionQuotes As Boolean
            Public useAggregations As Boolean
            Public FilterText As String
            Public FilterField As String
            Public Debugger As r2i.OWS.Framework.Debugger
        End Structure

        Private Shared Function FormatQueryVariable(ByRef fParams As StandardVariables, ByVal Value As String, ByVal EscapeListX As Integer, ByVal EscapeQuotes As Boolean, ByVal EscapeHTML As Boolean, ByVal Left As String, ByVal Right As String, ByVal Empty As String, ByVal DataType As String, ByVal sFormatters As String) As String
            If Not Value Is Nothing Then
                'HANDLE FORMATTERS
                Firewall.Unfirewall(Value)
                If Not sFormatters Is Nothing AndAlso sFormatters.Length > 0 Then
                    Dim Formatters As Integer
                    Dim saFormatters() As String = ParameterizeString(sFormatters, ","c, """"c, "\"c)
                    Formatters = saFormatters.Length
                    If Formatters > 0 Then
                        Dim i As Integer = 0
                        While Formatters > 0
                            Formatters -= 1

                            Dim bval As Boolean = False
                            'bval = RenderString_Format_Value(Index, VALUE, parameters(maxi), VALUE, DS, DR, RuntimeMessages, NullReturn, ProtectSession, SessionDelimiter, useSessionQuotes, FilterText, FilterField)
                            Dim rf As New Renderers.RenderFormat
                            bval = rf.RenderString_Format_Value(fParams.Caller, fParams.Index, Value, saFormatters(i), Value, fParams.DS, fParams.DR, fParams.RuntimeMessages, fParams.NullReturn, fParams.ProtectSession, fParams.SessionDelimiter, fParams.useSessionQuotes, fParams.FilterText, fParams.FilterField)
                            i += 1
                        End While
                    End If
                End If
                If EscapeListX > 0 Then
                    Dim charEscape As String = New String("\"c, EscapeListX)
                    If Not charEscape Is Nothing Then
                        Value = Value.Replace("[", charEscape & "[").Replace("{", charEscape & "{").Replace("]", charEscape & "]").Replace("}", charEscape & "}")
                    End If
                End If
                If EscapeHTML Then
                    Value = Utility.HTMLEncode(Value)
                End If
                If EscapeQuotes Then
                    Value = Value.Replace("'", "''")
                    If (Left Is Nothing OrElse Left.Length = 0) AndAlso (Right Is Nothing OrElse Right.Length = 0) Then
                        'NO LEFT AND RIGHT ARE PROVIDED, BUT SQL INJECTION IS CHECKED
                        Firewall.Firewall(Value, False, Utilities.Firewall.FirewallDirectiveEnum.Any, False)
                    End If
                End If
            Else
                Value = ""
            End If

            Value = ReplaceCommonVariable_Value(Value, DataType, Left, Right, Empty)
            Return Value
        End Function

        Public Sub New()

        End Sub
    End Class
End Namespace