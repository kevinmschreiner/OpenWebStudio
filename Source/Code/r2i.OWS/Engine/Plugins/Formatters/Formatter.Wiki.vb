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
Imports System.Text.RegularExpressions
Imports System.Collections.Generic
Imports r2i.OWS.Framework.Plugins.Formatters
Imports r2i.OWS.Framework
Imports r2i.OWS.Framework.Utilities
Imports r2i.OWS.Framework.Utilities.Compatibility

Public Class WikiTokenType
    Public Class WikiTokenManager
        Public Class Section
            Public TokenType As WikiTokenTypeEnum
            Public Name As String
            Public Number As Integer
            Public ChildSections As New ArrayList
            Public Parent As Section
        End Class
        Public Class Sidebar
            Public Name As String
            Public LayoutFormat As String
            Public LayoutItemFormat As String
            Public Items As String
        End Class
        Public LinkFormat As String = "<a #CLASS# href=""#LINK#"">#VALUE#</a>"
        Public LinkValueFormat As String = "#VALUE##SECTION#"
        Public LinkSectionValueFormat As String = " (#SECTION#)"
        Public LinkClassFormat As String = "class=""#CLASS#"""
        Public LinkExternalFormat As String = "default.aspx?topic=#NAME#"
        Public LinkInternalFormat As String = "#NAME#"

        Public CodeFormat As String = "<pre>#CODE#</pre>"
        Public TOCFormat As String = "<div>#TOC#</div>"
        Public TOCBlockFormat As String = "<ul>#TOC#</ul>"
        Public TOC0Format As String = "<li><span>#NUMBER#</span> <a href=""##NAME#"">#SECTION#</a>#TOC#</li>"
        Public TOC1Format As String = "<li><span>#NUMBER#</span> <a href=""##NAME#"">#SECTION#</a>#TOC#</li>"
        Public TOC2Format As String = "<li><span>#NUMBER#</span> <a href=""##NAME#"">#SECTION#</a>#TOC#</li>"
        Public TOC3Format As String = "<li><span>#NUMBER#</span> <a href=""##NAME#"">#SECTION#</a>#TOC#</li>"

        Public Section0Format As String = "<h1><a name=""#NAME#"">#SECTION#</a></h1>"
        Public Section1Format As String = "<h2><a name=""#NAME#"">#SECTION#</a></h2>"
        Public Section2Format As String = "<h3><a name=""#NAME#"">#SECTION#</a></h3>"
        Public Section3Format As String = "<h4><a name=""#NAME#"">#SECTION#</a></h4>"

        Public SidebarFormat As String = "<div>#HEADLINE#<br/><ol>#ITEMS#</ol></div>"
        Public SidebarItemFormat As String = "<li><a href=""#LINK#"">#TEXT#</a></li>"

        Public QuoteFormat As String = "<quote>#QUOTE#</quote>"
        '"""This is a pull quote"""

        '<sidebar>
        '<layout>...</layout>
        '<category>...</category>
        '<headline>...</headline>
        '<items>...</items> --CALL TO RETRIEVE ITEMS IF NOT KNOWN.
        '<item>...</item>
        '<item>...</item>
        '<item>...</item>
        '<item>...</item>
        '<item>...</item>
        '<item>...</item>
        '<item>...</item>
        '<item>...</item>
        '</sidebar>

        Public Sections As ArrayList
        Public SidebarLayouts As SortedDictionary(Of String, Sidebar)

        Private _sections As SortedList
        Public Sub New()
            Sections = New ArrayList
            _sections = New SortedList
            SidebarLayouts = New SortedDictionary(Of String, Sidebar)
        End Sub

        Public Function Render(ByVal Value As String, ByRef Caller As r2i.OWS.Engine, ByRef DS As DataSet, ByRef Messages As SortedList(Of String, String), ByVal useAggregations As Boolean, ByVal isPrerender As Boolean, ByRef DebugWriter As r2i.OWS.Framework.Debugger) As String
            Dim Tokens As New System.Collections.Generic.Stack(Of WikiTokenType)
            Dim i As Integer = 0
            While i < Value.Length AndAlso i >= 0
                Dim tki As Integer = WikiTokenType.Next(Value, i)
                If tki >= 0 Then
                    i = tki
                    Dim tk As WikiTokenType = WikiTokenType.Identify(Value, i)
                    i = tk.CloseIndex
                    If Not tk.TokenType = WikiTokenTypeEnum.UNKNOWN Then
                        Select Case tk.TokenType
                            Case WikiTokenTypeEnum.SECTION_TITLE, WikiTokenTypeEnum.SECTION_HEAD_1, WikiTokenTypeEnum.SECTION_HEAD_2, WikiTokenTypeEnum.SECTION_HEAD_3
                                AddSection(tk)
                        End Select
                        Tokens.Push(tk)
                    Else
                        i = i + 1
                    End If
                Else
                    i = -1
                End If
            End While
            While Tokens.Count > 0
                Dim tk As WikiTokenType = Tokens.Pop
                Dim str As String = WikiTokenType.Replace(tk, Me, Caller, DS, Messages, useAggregations, isPrerender, DebugWriter)
                Value = Value.Substring(0, tk.OpenIndex) & str & Value.Substring(tk.CloseIndex)
            End While
            Return Value
        End Function
        Public Sub AddSection(ByVal SectionToken As WikiTokenType)
            If Not _sections.ContainsKey(SectionToken.Value) Then
                Dim s As New Section
                s.TokenType = SectionToken.TokenType
                s.Name = SectionToken.Value
                _sections.Add(SectionToken.Value, s)
                WalkSection(s)
            End If
        End Sub
        Public Property SidebarLayout(ByVal Key As String) As String
            Get
                Dim isItem As Boolean = False
                Dim isItems As Boolean = False
                If Key.ToUpper.EndsWith(".ITEM") Then
                    isItem = True
                    Key = Key.Remove(Key.Length - 5)
                End If
                If Key.ToUpper.EndsWith(".ITEMS") Then
                    isItems = True
                    Key = Key.Remove(Key.Length - 6)
                End If

                If SidebarLayouts.ContainsKey(Key.ToUpper) Then
                    Dim sb As Sidebar = SidebarLayouts(Key.ToUpper)
                    If isItem Then
                        Return sb.LayoutItemFormat
                    ElseIf isItems Then
                        Return sb.Items
                    Else
                        Return sb.LayoutFormat
                    End If
                Else
                    Return ""
                End If
            End Get
            Set(ByVal value As String)
                Dim isItem As Boolean = False
                Dim isItems As Boolean = False
                If Key.ToUpper.EndsWith(".ITEM") Then
                    isItem = True
                    Key = Key.Remove(Key.Length - 5)
                End If
                If Key.ToUpper.EndsWith(".ITEMS") Then
                    isItems = True
                    Key = Key.Remove(Key.Length - 6)
                End If
                If SidebarLayouts.ContainsKey(Key.ToUpper) Then
                    Dim sb As Sidebar = SidebarLayouts(Key.ToUpper)
                    If isItem Then
                        sb.LayoutItemFormat = value
                    ElseIf isItems Then
                        sb.Items = value
                    Else
                        sb.LayoutFormat = value
                    End If
                    SidebarLayouts.Item(Key.ToUpper) = sb
                Else
                    Dim sb As Sidebar = New Sidebar
                    If isItem Then
                        sb.LayoutItemFormat = value
                    ElseIf isItems Then
                        sb.Items = value
                    Else
                        sb.LayoutFormat = value
                    End If
                    SidebarLayouts.Add(Key.ToUpper, sb)
                End If
            End Set
        End Property


        Private _walk As Section
        Private Sub WalkSection(ByVal s As Section)
            If _walk Is Nothing Then
                If Sections.Count = 0 Then
                    _walk = s
                    Sections.Add(s)
                    s.Number = Sections.Count
                Else
                    _walk = Sections(Sections.Count - 1)
                End If
            Else
                Dim depth As Integer = SectionDepth(s.TokenType)
                Dim pdepth As Integer = SectionDepth(_walk.TokenType)
                If depth > pdepth Then
                    s.TokenType = SectionDepthValue(pdepth + 1)
                    s.Parent = _walk
                    _walk.ChildSections.Add(s)
                    s.Number = _walk.ChildSections.Count
                    _walk = s
                ElseIf depth < pdepth Then
                    If depth > 0 Then
                        While depth <= pdepth
                            _walk = _walk.Parent
                            pdepth -= 1
                        End While
                        _walk.ChildSections.Add(s)
                        s.Number = _walk.ChildSections.Count
                        s.Parent = _walk
                        _walk = s
                    Else
                        _walk = s
                        Sections.Add(s)
                        s.Number = Sections.Count
                    End If
                Else
                    'SAME
                    If _walk.Parent Is Nothing Then
                        Sections.Add(s)
                        s.Number = Sections.Count
                        _walk = s
                    Else
                        s.Parent = _walk.Parent
                        s.Parent.ChildSections.Add(s)
                        s.Number = s.Parent.ChildSections.Count
                        _walk = s
                    End If
                End If
            End If
        End Sub
        Public Shared Function SectionDepthValue(ByVal depth As Integer) As WikiTokenTypeEnum
            Select Case depth
                Case 0
                    Return WikiTokenTypeEnum.SECTION_TITLE
                Case 1
                    Return WikiTokenTypeEnum.SECTION_HEAD_1
                Case 2
                    Return WikiTokenTypeEnum.SECTION_HEAD_2
                Case 3
                    Return WikiTokenTypeEnum.SECTION_HEAD_3
            End Select
        End Function
        Public Shared Function SectionDepth(ByVal tokenType As WikiTokenTypeEnum) As Integer
            Select Case tokenType
                Case WikiTokenTypeEnum.SECTION_TITLE
                    Return 0
                Case WikiTokenTypeEnum.SECTION_HEAD_1
                    Return 1
                Case WikiTokenTypeEnum.SECTION_HEAD_2
                    Return 2
                Case WikiTokenTypeEnum.SECTION_HEAD_3
                    Return 3
            End Select
            Return 4
        End Function
    End Class
    Public Enum WikiTokenTypeEnum
        LINK
        SECTION_TITLE
        SECTION_HEAD_1
        SECTION_HEAD_2
        SECTION_HEAD_3
        NOWIKI
        TOC
        CODE
        REFERENCES
        SIDEBAR
        QUOTE
        UNKNOWN
    End Enum
    Public Open As String
    Public Close As String
    Public OpenIndex As Integer
    Public CloseIndex As Integer
    Public Value As String
    Public TokenType As WikiTokenTypeEnum
    Private Shared TokenValues As String() = {"[[", "==", "<nowiki>", "&lt;nowiki&gt;", "<code>", "&lt;code&gt;", "<toc>", "&lt;toc&gt;", "<toc/>", "&lt;toc/&gt;", "<references/>", "&lt;references/&gt;", "<sidebar>", "&lt;sidebar&gt;", """""""", "&quot;&quot;&quot;"}
    Public Shared Function CloseFromOpen(ByVal Open As String) As WikiTokenType
        Dim wikit As New WikiTokenType
        wikit.Open = Open
        With wikit
            Select Case Open.ToUpper
                Case "[["
                    wikit.Close = "]]"
                    wikit.TokenType = WikiTokenTypeEnum.LINK
                Case """"""""
                    wikit.Close = """"""""
                    wikit.TokenType = WikiTokenTypeEnum.QUOTE
                Case "&QUOT;&QUOT;&QUOT;"
                    wikit.Close = "&quot;&quot;&quot;"
                    wikit.TokenType = WikiTokenTypeEnum.QUOTE
                Case "=="
                    wikit.Close = "=="
                    wikit.TokenType = WikiTokenTypeEnum.SECTION_TITLE
                Case "==="
                    wikit.Close = "==="
                    wikit.TokenType = WikiTokenTypeEnum.SECTION_HEAD_1
                Case "===="
                    wikit.Close = "===="
                    wikit.TokenType = WikiTokenTypeEnum.SECTION_HEAD_2
                Case "====="
                    wikit.Close = "====="
                    wikit.TokenType = WikiTokenTypeEnum.SECTION_HEAD_3
                Case "<NOWIKI>"
                    wikit.Close = "</nowiki>"
                    wikit.TokenType = WikiTokenTypeEnum.NOWIKI
                Case "&LT;NOWIKI&GT;"
                    wikit.Close = "&lt;/nowiki&gt;"
                    wikit.TokenType = WikiTokenTypeEnum.NOWIKI
                Case "<CODE>"
                    wikit.Close = "</code>"
                    wikit.TokenType = WikiTokenTypeEnum.CODE
                Case "&LT;CODE&GT;"
                    wikit.Close = "&lt;/code&gt;"
                    wikit.TokenType = WikiTokenTypeEnum.CODE
                Case "<TOC/>"
                    wikit.Close = ""
                    wikit.TokenType = WikiTokenTypeEnum.TOC
                Case "&LT;TOC/&GT;"
                    wikit.Close = ""
                    wikit.TokenType = WikiTokenTypeEnum.TOC
                Case "<TOC>"
                    wikit.Close = "</toc>"
                    wikit.TokenType = WikiTokenTypeEnum.TOC
                Case "&LT;TOC&GT;"
                    wikit.Close = "&lt;/toc&gt;"
                    wikit.TokenType = WikiTokenTypeEnum.TOC
                Case "<REFERENCES/>"
                    wikit.Close = ""
                    wikit.TokenType = WikiTokenTypeEnum.REFERENCES
                Case "&LT;REFERENCES/&GT;"
                    wikit.Close = ""
                    wikit.TokenType = WikiTokenTypeEnum.REFERENCES
                Case "<SIDEBAR>"
                    wikit.Close = "</sidebar>"
                    wikit.TokenType = WikiTokenTypeEnum.SIDEBAR
                Case "&LT;SIDEBAR&GT;"
                    wikit.Close = "&lt;/sidebar&gt;"
                    wikit.TokenType = WikiTokenTypeEnum.SIDEBAR
            End Select
        End With
        Return wikit
    End Function
    Public Shared Function Replace(ByVal Token As WikiTokenType, ByRef SectionManager As WikiTokenManager, ByRef Caller As r2i.OWS.Engine, ByRef DS As DataSet, ByRef Messages As SortedList(Of String, String), ByVal useAggregations As Boolean, ByVal isPrerender As Boolean, ByRef DebugWriter As r2i.OWS.Framework.Debugger) As String
        'THIS ASSUMES THAT THE WIKITOKEN HAS THE FOLLOWING:
        'TOKEN TYPE IS THE TYPE OF THE TOKEN
        'VALUE IS THE BODY OF THE TOKEN
        'OPEN INDEX AND CLOSE INDEX, WHILE OPTIONAL, AID THE CALLER IN REPLACEMENT
        'OPEN AND CLOSE MAY OR MAY NOT BE POPULATED
        Select Case Token.TokenType
            Case WikiTokenTypeEnum.CODE
                Return SectionManager.CodeFormat.Replace("#CODE#", System.Web.HttpUtility.HtmlEncode(Token.Value))
            Case WikiTokenTypeEnum.LINK
                Return FormatLink(Token.Value, SectionManager, Caller, DS, messages, useaggregations, isprerender, debugwriter)
            Case WikiTokenTypeEnum.QUOTE
                Return SectionManager.QuoteFormat.Replace("#QUOTE#", Token.Value)
            Case WikiTokenTypeEnum.NOWIKI
                Return Token.Value
            Case WikiTokenTypeEnum.REFERENCES
                'NOT HANDLED YET?
            Case WikiTokenTypeEnum.SECTION_HEAD_1
                Return FormatSection(Token.Value, 1, SectionManager)
            Case WikiTokenTypeEnum.SECTION_HEAD_2
                Return FormatSection(Token.Value, 2, SectionManager)
            Case WikiTokenTypeEnum.SECTION_HEAD_3
                Return FormatSection(Token.Value, 3, SectionManager)
            Case WikiTokenTypeEnum.SECTION_TITLE
                Return FormatSection(Token.Value, 0, SectionManager)
            Case WikiTokenTypeEnum.TOC
                Return FormatTOC(SectionManager)
            Case WikiTokenTypeEnum.SIDEBAR
                Return FormatSidebar(Token.Value, SectionManager, Caller, DS, Messages, useAggregations, isPrerender, DebugWriter)
            Case WikiTokenTypeEnum.UNKNOWN
                Return Token.Value
        End Select
        Return Nothing
    End Function
    Private Shared Function FormatSection(ByVal Value As String, ByVal Level As Integer, ByRef SectionManager As WikiTokenManager) As String
        Dim formatValue As String = "<a name=""#NAME#""/><span>#SECTION#</span>"
        Select Case Level
            Case 0
                formatValue = SectionManager.Section0Format
            Case 1
                formatValue = SectionManager.Section1Format
            Case 2
                formatValue = SectionManager.Section2Format
            Case 3
                formatValue = SectionManager.Section3Format
        End Select
        Return formatValue.Replace("#NAME#", FormatSectionLink(Value)).Replace("#SECTION#", Value)
    End Function
    Private Shared Function FormatSidebar(ByVal Value As String, ByVal SidebarManager As WikiTokenManager, ByRef Caller As r2i.OWS.Engine, ByRef DS As DataSet, ByRef Messages As SortedList(Of String, String), ByVal useAggregations As Boolean, ByVal isPrerender As Boolean, ByRef DebugWriter As r2i.OWS.Framework.Debugger) As String
        'Public SidebarFormat As String = "<div>#TITLE#<br/><ol>#ITEMS#</ol></div>"
        'Public SidebarItemFormat As String = "<li><a href=""#LINK#"">#TEXT#</a></li>"
        Dim str As String = ""
        Dim itemArray As String() = Nothing
        Dim layout As String = ""
        Dim title As String = ""
        Dim category As String = ""
        Dim sb As New WikiTokenManager.Sidebar
        sb.LayoutFormat = SidebarManager.SidebarFormat
        sb.LayoutItemFormat = SidebarManager.SidebarItemFormat
        'GET THE PARAMETERS
        Dim s() As String = Nothing

        Dim svalue As String = Value
        If svalue.ToUpper.Contains("&LT;LAYOUT") Then
            'DECODE THE HTML
            svalue = System.Web.HttpUtility.HtmlDecode(svalue)
        End If
        s = Nothing : s = GetShortValue("layout", svalue) : If (s.Length > 0) Then layout = s(0)
        s = Nothing : s = GetShortValue("category", svalue) : If (s.Length > 0) Then category = s(0)
        s = Nothing : s = GetShortValue("headline", svalue) : If (s.Length > 0) Then title = s(0)
        s = Nothing : s = GetShortValue("item", svalue) : If (s.Length > 0) Then itemArray = s

        If Not layout Is Nothing AndAlso layout.Length > 0 Then
            'GET THE LAYOUT
            Dim sbL As String = SidebarManager.SidebarLayout(layout)
            Dim sbLi As String = SidebarManager.SidebarLayout(layout & ".item")
            Dim sbLis As String = SidebarManager.SidebarLayout(layout & ".items")
            If Not sbL Is Nothing Then sb.LayoutFormat = sbL
            If Not sbLi Is Nothing Then sb.LayoutItemFormat = sbLi
            If Not sbLis Is Nothing Then sb.Items = sbLis
        End If
        Dim strTEXT As String = ""
        Dim strITEMS As String = ""
        Dim iIndex As Integer = 1
        sb.LayoutFormat = sb.LayoutFormat.Replace("#HEADLINE#", title).Replace("#CATEGORY#", category)
        If Not itemArray Is Nothing AndAlso itemArray.Length > 0 Then
            Dim strItem As String
            For Each strItem In itemArray
                Dim strPindex As Integer = strItem.IndexOf("|")
                Dim strLink As String = ""
                Dim strLinkText As String = ""
                If strPindex >= 0 Then
                    If strPindex > 0 Then
                        strLink = strItem.Substring(0, strPindex)
                    Else
                        strLink = ""
                    End If
                    If strPindex + 1 < strItem.Length Then
                        strLinkText = strItem.Substring(strPindex + 1)
                    End If
                Else
                    strLink = strItem
                End If

                strLink = SidebarManager.Render(strLink, Caller, ds, messages, useaggregations, isprerender, debugwriter)
                strLinkText = SidebarManager.Render(strLinkText, Caller, ds, messages, useaggregations, isprerender, debugwriter)

                strTEXT &= sb.LayoutItemFormat.Replace("#LINK#", strLink).Replace("#TEXT#", strLinkText).Replace("#CATEGORY#", category).Replace("#INDEX#", iIndex)
                iIndex += 1
            Next
        End If
        Return sb.LayoutFormat.Replace("#ITEMS#", strTEXT)
    End Function
    Private Shared Function GetShortValue(ByVal tag As String, ByRef Value As String) As String()
        Dim strResult As Stack(Of String) = New Stack(Of String)
        Dim startPos As Integer = Value.ToUpper.IndexOf("<" & tag.ToUpper & ">")
        While startPos >= 0
            Dim endPos As Integer
            endPos = Value.ToUpper.IndexOf("</" & tag.ToUpper & ">", startPos)
            If endPos >= 0 Then
                strResult.Push(Value.Substring(startPos + 2 + tag.Length, endPos - (startPos + 2 + tag.Length)))
                startPos = Value.ToUpper.IndexOf("<" & tag.ToUpper & ">", endPos + 3 + tag.Length)
            Else
                startPos = -1
            End If
        End While
        Dim s As String() = strResult.ToArray
        Array.Reverse(s)
        Return s
    End Function

    Private Shared LinkRegEx As Regex = New Regex("[^a-zA-Z0-9_#]+", RegexOptions.IgnoreCase Or RegexOptions.Multiline Or RegexOptions.IgnorePatternWhitespace Or RegexOptions.Compiled)
    Private Shared Function FormatLink(ByVal Value As String, ByRef SectionManager As WikiTokenManager, ByRef Caller As r2i.OWS.Engine, ByRef DS As DataSet, ByRef Messages As SortedList(Of String, String), ByVal useAggregations As Boolean, ByVal isPrerender As Boolean, ByRef DebugWriter As r2i.OWS.Framework.Debugger) As String
        Dim LinkFormat As String = "<a #CLASS# href=""#LINK#"">#VALUE#</a>"
        Dim LinkExternalFormat As String = "default.aspx?topic=#NAME#"
        'IF THE LINKEXTERNALFORMAT CONTAINS #@NAME#, THE RENDERSTRING MUST BE PERFORMED ON THE VALUE
        Dim RenderExternalLink As Boolean = False
        Dim LinkInternalFormat As String = "#NAME#"
        Dim LinkValueFormat As String = "#VALUE##SECTION#"
        Dim LinkSectionValueFormat As String = " (#SECTION#)"
        Dim LinkClassFormat As String = "class=""#CLASS#"""
        If Not SectionManager.LinkFormat Is Nothing AndAlso SectionManager.LinkFormat.Length > 0 Then
            LinkFormat = SectionManager.LinkFormat
        End If
        If Not SectionManager.LinkValueFormat Is Nothing AndAlso SectionManager.LinkValueFormat.Length > 0 Then
            LinkValueFormat = SectionManager.LinkValueFormat
        End If
        If Not SectionManager.LinkSectionValueFormat Is Nothing AndAlso SectionManager.LinkSectionValueFormat.Length > 0 Then
            LinkSectionValueFormat = SectionManager.LinkSectionValueFormat
        End If
        If Not SectionManager.LinkClassFormat Is Nothing AndAlso SectionManager.LinkClassFormat.Length > 0 Then
            LinkClassFormat = SectionManager.LinkClassFormat
        End If
        If Not SectionManager.LinkInternalFormat Is Nothing AndAlso SectionManager.LinkInternalFormat.Length > 0 Then
            LinkInternalFormat = SectionManager.LinkInternalFormat
        End If
        If Not SectionManager.LinkExternalFormat Is Nothing AndAlso SectionManager.LinkExternalFormat.Length > 0 Then
            LinkExternalFormat = SectionManager.LinkExternalFormat
            If LinkExternalFormat.Contains("#@NAME#") Then RenderExternalLink = True
        End If
        Dim noValue As Boolean = True
        Dim linkFmt As String = LinkExternalFormat
        Dim sNAME As String = ""
        Dim sSECTION As String = ""
        Dim sVALUE As String = ""
        Dim sCLASS As String = ""
        Dim pipeIndex As Integer = Value.IndexOf("|"c)
        Dim colonIndex As Integer = Value.IndexOf(":"c)
        Dim poundIndex As Integer = Value.IndexOf("#"c)
        If pipeIndex >= 0 Then
            sNAME = Value.Substring(0, pipeIndex)
            sVALUE = Value.Substring(pipeIndex + 1)
            noValue = False
        Else
            sNAME = Value
            pipeIndex = Value.Length
        End If
        If colonIndex >= 0 AndAlso colonIndex < pipeIndex Then
            sCLASS = sNAME.Substring(0, colonIndex)
            sNAME = sNAME.Substring(colonIndex + 1)
        End If
        If sVALUE Is Nothing OrElse sVALUE.Length = 0 Then
            sVALUE = sNAME
        End If
        If poundIndex >= 0 AndAlso poundIndex > colonIndex AndAlso poundIndex < pipeIndex Then
            poundIndex = sNAME.IndexOf("#")
            If poundIndex = 0 Then
                linkFmt = LinkInternalFormat
                RenderExternalLink = False
            End If
            If poundIndex >= 0 Then
                sSECTION = sNAME.Substring(poundIndex + 1)
            End If
            If Not sSECTION Is Nothing AndAlso sSECTION.Length > 0 Then
                If noValue Then
                    sVALUE = sNAME.Substring(0, poundIndex)
                    If poundIndex = 0 Then
                        sVALUE = sSECTION
                        sSECTION = ""
                    Else
                        sSECTION = LinkSectionValueFormat.Replace("#SECTION#", sSECTION)
                    End If
                Else
                    sSECTION = ""
                End If
            End If
        End If

        If Not sCLASS Is Nothing AndAlso sCLASS.Length > 0 Then
            sCLASS = LinkClassFormat.Replace("#CLASS#", sCLASS)
        End If
        sVALUE = LinkValueFormat.Replace("#VALUE#", sVALUE).Replace("#SECTION#", sSECTION)
        If RenderExternalLink Then
            Dim sNameValue As String = FormatSectionLink(sNAME)
            Dim qv As New QueryOptionItem
            qv.EscapeListX = 2
            qv.QuerySource = sNameValue
            qv.Protected = True
            qv.QueryTarget = "#@NAME#"
            qv.QueryTargetEmpty = "''"
            qv.QueryTargetLeft = "'"
            qv.QueryTargetRight = "'"
            qv.VariableType = "Custom"

            Caller.xls.QueryItem(qv.QueryTarget) = qv

            linkFmt = Caller.RenderString(DS, linkFmt, Messages, useAggregations, isPrerender, DebugWriter:=DebugWriter)
            Return LinkFormat.Replace("#LINK#", linkFmt).Replace("#NAME#", sNameValue).Replace("#VALUE#", sVALUE).Replace("#CLASS#", sCLASS)
        Else
            Return LinkFormat.Replace("#LINK#", linkFmt).Replace("#NAME#", FormatSectionLink(sNAME)).Replace("#VALUE#", sVALUE).Replace("#CLASS#", sCLASS)
        End If
    End Function
    Private Shared Function FormatNumber(ByVal s As WikiTokenManager.Section) As String
        Dim str As String = s.Number
        Dim sparent As WikiTokenManager.Section
        sparent = s.Parent
        While Not sparent Is Nothing
            str = sparent.Number & "." & str
            sparent = sparent.Parent
        End While
        Return str
    End Function
    Private Shared Function FormatSectionLink(ByVal Text As String) As String
        Return LinkRegEx.Replace(Text, "_")
    End Function
    Private Shared Function FormatTOCItem(ByVal Manager As WikiTokenManager, ByVal s As WikiTokenManager.Section, ByVal TokenFormat As String, ByVal Tkf0 As String, ByVal Tkf1 As String, ByVal Tkf2 As String, ByVal Tkf3 As String) As String
        Dim strToken As String = ""
        Dim strTokens As String = ""
        Dim format As String = ""
        Select Case s.TokenType
            Case WikiTokenTypeEnum.SECTION_HEAD_1
                format = Tkf1
            Case WikiTokenTypeEnum.SECTION_HEAD_2
                format = Tkf2
            Case WikiTokenTypeEnum.SECTION_HEAD_3
                format = Tkf3
            Case Else
                format = Tkf0
        End Select

        If s.ChildSections.Count > 0 Then
            Dim tkS As WikiTokenManager.Section
            For Each tkS In s.ChildSections
                strTokens &= WikiTokenType.FormatTOCItem(Manager, tkS, TokenFormat, Tkf0, Tkf1, Tkf2, Tkf3)
            Next
        End If
        strToken = format.Replace("#NUMBER#", FormatNumber(s)).Replace("#NAME#", FormatSectionLink(s.Name)).Replace("#SECTION#", s.Name)
        If s.ChildSections.Count > 0 Then
            strTokens = TokenFormat.Replace("#TOC#", strTokens)
        End If
        Return strToken.Replace("#TOC#", strTokens)
    End Function
    Private Shared Function FormatTOC(ByVal Manager As WikiTokenManager) As String
        'THIS SHOULD ONLY BE HANDLED AT THE END
        Dim TOCFormat As String = "<div>#TOC#</div>"
        Dim TOCBlockFormat As String = "<ul>#TOC#</ul>"
        Dim TOC0Format As String = "<li><span>#NUMBER#</span> <a href=""##NAME#"">#SECTION#</a>#TOC#</li>"
        Dim TOC1Format As String = "<li><span>#NUMBER#</span> <a href=""##NAME#"">#SECTION#</a>#TOC#</li>"
        Dim TOC2Format As String = "<li><span>#NUMBER#</span> <a href=""##NAME#"">#SECTION#</a>#TOC#</li>"
        Dim TOC3Format As String = "<li><span>#NUMBER#</span> <a href=""##NAME#"">#SECTION#</a>#TOC#</li>"
        If Not Manager.TOCFormat Is Nothing AndAlso Manager.TOCFormat.Length > 0 Then
            TOCFormat = Manager.TOCFormat
        End If
        If Not Manager.TOCBlockFormat Is Nothing AndAlso Manager.TOCBlockFormat.Length > 0 Then
            TOCBlockFormat = Manager.TOCBlockFormat
        End If
        If Not Manager.TOC0Format Is Nothing AndAlso Manager.TOC0Format.Length > 0 Then
            TOC0Format = Manager.TOC0Format
        End If
        If Not Manager.TOC1Format Is Nothing AndAlso Manager.TOC1Format.Length > 0 Then
            TOC1Format = Manager.TOC1Format
        End If
        If Not Manager.TOC2Format Is Nothing AndAlso Manager.TOC2Format.Length > 0 Then
            TOC2Format = Manager.TOC2Format
        End If
        If Not Manager.TOC3Format Is Nothing AndAlso Manager.TOC3Format.Length > 0 Then
            TOC3Format = Manager.TOC3Format
        End If
        Dim str As String = ""
        Dim s As WikiTokenManager.Section
        For Each s In Manager.Sections
            str &= WikiTokenType.FormatTOCItem(Manager, s, TOCBlockFormat, TOC0Format, TOC1Format, TOC2Format, TOC3Format)
        Next
        Return TOCFormat.Replace("#TOC#", str)
    End Function
    Public Shared Function Identify(ByRef Source As String, ByRef Position As Integer) As WikiTokenType
        Dim charStart As Char = Source.Chars(Position)
        Dim maxLength As Integer = 0
        Dim tk As WikiTokenType = Nothing
        Select Case charStart
            Case """"c
                'QUOTE
                tk = WikiTokenType.CloseFromOpen(Source.Substring(Position, 3))
            Case "["c
                'LINK
                tk = WikiTokenType.CloseFromOpen(Source.Substring(Position, 2))
            Case "<"c, "&"c
                Dim offset As Integer = 0
                If charStart = "&" Then
                    Select Case Source.Chars(Position + 1)
                        Case "l"c, "L"c
                            offset = 3
                        Case "q"c, "Q"c
                            offset = -1
                            'QUOTE
                            tk = WikiTokenType.CloseFromOpen(Source.Substring(Position, 18))
                    End Select
                End If
                If offset >= 0 Then
                    Select Case Source.Chars(Position + offset + 1)
                        Case "N"c, "n"c
                            'NOWIKI
                            tk = WikiTokenType.CloseFromOpen(Source.Substring(Position, 8 + ((offset) * 2)))
                        Case "T"c, "t"c
                            'TOC
                            If Source.Substring(Position, 5 + offset).EndsWith("/") Then
                                tk = WikiTokenType.CloseFromOpen(Source.Substring(Position, 6 + ((offset) * 2)))
                            Else
                                tk = WikiTokenType.CloseFromOpen(Source.Substring(Position, 5 + ((offset) * 2)))
                            End If
                        Case "S"c, "s"c
                            'SIDEBAR
                            tk = WikiTokenType.CloseFromOpen(Source.Substring(Position, 9 + ((offset) * 2)))
                        Case "R"c, "r"c
                            'REFERENCE
                            tk = WikiTokenType.CloseFromOpen(Source.Substring(Position, 12 + ((offset) * 2)))
                        Case "C"c, "c"c
                            'CODE
                            tk = WikiTokenType.CloseFromOpen(Source.Substring(Position, 6 + ((offset) * 2)))
                    End Select
                End If
            Case "="c
                Dim depth As Integer = -1
                Dim i As Integer = Position
                Dim t As Boolean = True
                While i < Source.Length AndAlso t
                    If Source.Chars(i) = "="c Then
                        depth += 1
                    Else
                        t = False
                    End If
                    i += 1
                End While
                Select Case depth
                    Case 1
                        tk = WikiTokenType.CloseFromOpen(Source.Substring(Position, 2))
                    Case 2
                        tk = WikiTokenType.CloseFromOpen(Source.Substring(Position, 3))
                    Case 3
                        tk = WikiTokenType.CloseFromOpen(Source.Substring(Position, 4))
                    Case 4
                        tk = WikiTokenType.CloseFromOpen(Source.Substring(Position, 5))
                    Case Else
                        tk = New WikiTokenType
                        tk.TokenType = WikiTokenTypeEnum.UNKNOWN
                        'NOT A TOKEN
                End Select
        End Select
        tk.OpenIndex = Position
        tk.CloseIndex = Position
        If Not tk.TokenType = WikiTokenTypeEnum.UNKNOWN Then
            Dim nextI As Integer = Source.IndexOf(tk.Close, tk.OpenIndex + tk.Open.Length)
            If maxLength > 0 Then
                If nextI > tk.OpenIndex + maxLength Then
                    nextI = -1
                End If
            End If
            If nextI > -1 Then
                tk.CloseIndex = nextI + tk.Close.Length
                tk.Value = Source.Substring(tk.OpenIndex + tk.Open.Length, tk.CloseIndex - tk.OpenIndex - tk.Close.Length - tk.Open.Length)
            End If
            If nextI = -1 OrElse ((tk.TokenType = WikiTokenTypeEnum.SECTION_TITLE OrElse tk.TokenType = WikiTokenTypeEnum.SECTION_HEAD_1 OrElse tk.TokenType = WikiTokenTypeEnum.SECTION_HEAD_2 OrElse tk.TokenType = WikiTokenTypeEnum.SECTION_HEAD_3) AndAlso (tk.Value Is Nothing OrElse tk.Value.Length = 0)) Then
                tk.TokenType = WikiTokenTypeEnum.UNKNOWN
            End If
        End If
        Return tk
    End Function
    Public Shared Function [Next](ByRef Source As String, ByRef Position As Integer) As Integer
        Dim pIndex As Integer = -1
        Dim strK As String
        For Each strK In TokenValues
            Dim i As Integer = Source.IndexOf(strK, Position)
            If i >= 0 AndAlso (i < pIndex Or pIndex = -1) Then
                pIndex = i
            End If
        Next
        'pIndex contains the closest next index
        Return pIndex
    End Function
End Class