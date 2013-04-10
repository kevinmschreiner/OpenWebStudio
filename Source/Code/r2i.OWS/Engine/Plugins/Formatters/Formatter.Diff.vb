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
'#define MyDEBUG

' MergeEngine.cs
'
' This file contains the implementation of the functionality to compare
' and merge two Html strings.
'
' The comparison is word-wise, not character wise. Thus, the process
' consists of two steps.
'
' The first step is to parse the html string into collection of English
' words(strong typed collection WordsCollection is defined for this) in
' such a way that:
' 1) anything starts with '<' and ends with '>' is treated as Html
' tag.
' 2) Html tags and whitespaces are treated as prefix or suffix to
' adjacent word and be put in the prefix or suffix fileds of the
' Word object.
' 3) English words separated by space(s), "&nbsp;", "&#xxx",
' tailing punctuation are treated as words and be put in the
' word field of Word class.
' 4) Whitespaces immediately after or before Html tags are ignored.
' ( whitespaces == {' ', '\t', '\n'} )
'
' The second step is to compare and merge the two words collections by
' the algorithm proposed by [1]. The follwoing are the basic steps of
' the algorithm (read [1] for details):
' 1) Find the middle snake of the two sequences by searching from
' both the left-up and right-bottom corners of the edit graph at
' the same time. When the furthest reaching paths of the two
' searches first meet, the snake is reported as middle snake. It
' may be empty sequence(or most likely be?).
' 2) For the sub-sequences before the middle snake and the
' sub-sequences after the middle snake, do recursion on them.
' 3) Some key nomenclature:
' Edit Graph -- for sequences A(N) and B(M), construct graph in
' such a way that there is always edge from (A(i-1), B)
' to (A(i), B) and edge from (A, B(j-1)) to
' (A, B(j)) (vertical or parallel edge). If A(i)
' == B(j) then there is edge from (A(i-1), B(j-1))
' to (A(i), B(j)) (diagonal edge).
' Snake -- not the kind of animal here ..). a sequence of diagonal
' edges surrounded by non-diagonal edges at both ends.
' Furthest Reaching Path -- searching from the left-up corner toward
' the right-bottom corner, the path that goes closest to
' the right-bottom corner(in other words, there are more
' disgonal edges on this path).
' LCS / SES -- Longest Common Sequence and Shortest Edit Script.
' Simple say, the shortest path between left-up and right-bottom
' corners of the edit graph.
'
' [1] Eugene W. Myers, "An O(ND) Difference Algorithm and Its Variations"
' A copy of the file can be found at:
' http://www.xmailserver.org/diff2.pdf
' [2] http://cvs.sourceforge.net/viewcvs.py/*checkout*/cvsgui/cvsgui/cvs-1.10/diff/analyze.c?&rev=1.1.1.3
'
' The file is created to be used inside Rainbow(www.Rainbowportal.net)
' to compare the staging and production contents of HtmlDocument module
' while working in Workflow mode. However, this file can be easily
' modified to be used in other senario.
'
' All of the code in this file are implemented from scratch by the
' author, with reference to the Unix Diff implementation in [2].
'
' This program is free and can be distributed or used for any purpose
' with no restriction.
'
' The author would like to thank Matt Cowan(mcowan@county.oxford.on.ca)
' for pushing this work and undertaking lots of testings.
'
' Author: Hongwei Shen
' Email: hongwei.shen@gmail.com
' Date: June 22, 2005

Imports System
Imports System.Collections
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Diagnostics
Imports r2i.OWS.Framework.Plugins.Formatters
Imports r2i.OWS.Framework
Namespace r2i.OWS.Formatters
    Public Class Diff : Inherits FormatterBase

        Public Overrides Function Handle_Render(ByRef Caller As EngineBase, ByVal Index As Integer, ByRef Value As String, ByRef Formatter As String, ByRef Source As String, ByRef DS As System.Data.DataSet, ByRef DR As System.Data.DataRow, ByRef RuntimeMessages As System.Collections.Generic.SortedList(Of String, String), ByVal NullReturn As Boolean, ByVal NullOverride As Boolean, ByVal ProtectSession As Boolean, ByVal SessionDelimiter As String, ByVal useSessionQuotes As Boolean, ByVal useAggregations As Boolean, ByRef FilterText As String, ByRef FilterField As String, ByRef Debugger As Framework.Debugger) As Boolean
            Dim fParameters As String() = ParameterizeString(Formatter.Substring(6).TrimEnd(New Char() {"}"c}), ","c, """"c, "\"c)
            Dim initialvalue As String = ""
            If fParameters.Length = 2 Then
                If Not Value Is Nothing Then
                    'VALUE WAS THE ORIGINAL SOURCE
                    Dim COLLECTION As String = fParameters(1)
                    initialvalue = fParameters(0)
                    If COLLECTION.Length > 0 Then
                        If Not COLLECTION.ToUpper = "TEXT" Then
                            initialvalue = "[" & initialvalue & "," & COLLECTION & "]"
                        End If
                    Else
                        initialvalue = "[" & initialvalue & "]"
                    End If
                    Dim b As Boolean = Caller.RenderString(Index, initialvalue, New Char() {"["c, "{"c}, New Char() {"]"c, "}"c}, "\"c, DS, DR, RuntimeMessages, False, False, NullReturn:=NullReturn, ProtectSession:=ProtectSession, SessionDelimiter:=SessionDelimiter, useSessionQuotes:=useSessionQuotes)
                    If Not b Then
                        '...
                    End If
                End If

                Dim s As String
                Dim m As New Merger(Value, initialvalue)
                Dim cm As CommentProperties = m.CommentProperties

                s = Caller.ActionVariable("ows.Diff.Added.BeginTag")
                If Not s Is Nothing AndAlso s.Length > 0 Then
                    cm.Added.BeginTag = s
                End If
                s = Caller.ActionVariable("ows.Diff.Added.EndTag")
                If Not s Is Nothing AndAlso s.Length > 0 Then
                    cm.Added.EndTag = s
                End If
                s = Caller.ActionVariable("ows.Diff.Removed.BeginTag")
                If Not s Is Nothing AndAlso s.Length > 0 Then
                    cm.Removed.BeginTag = s
                End If
                s = Caller.ActionVariable("ows.Diff.Removed.EndTag")
                If Not s Is Nothing AndAlso s.Length > 0 Then
                    cm.Removed.EndTag = s
                End If
                m.CommentProperties = cm


                Source = m.merge
                m = Nothing

                Return True
            End If
        End Function

        Public Overrides ReadOnly Property RenderTag() As String
            Get
                Return "diff"
            End Get
        End Property
    End Class
End Namespace

#Region "Data types"

''' <summary>
''' When we compare two files, we say we delete or add
''' some sub sequences in the original file to result
''' in the modified file. This is to define the strong
''' type for identifying the status of a such sequence.
''' </summary>
Enum SequenceStatus
    ''' <summary>
    ''' The sequence is inside the original
    ''' file but not in the modified file
    ''' </summary>
    Deleted = 0

    ''' <summary>
    ''' The sequence is inside the modifed
    ''' file but not in the original file
    ''' </summary>
    Inserted

    ''' <summary>
    ''' The sequence is in both the origianl
    ''' and the modified files
    ''' </summary>
    NoChange
End Enum

Public Structure CommentProperties
    Public Added As CommentTags
    Public Removed As CommentTags
End Structure
Public Structure CommentTags
    Public BeginTag As String
    Public EndTag As String
End Structure

'''' <summary>
'''' The class defines the begining and end html tag
'''' for marking up the deleted words in the merged
'''' file.
'''' </summary>
'Class CommentOff
'    Public Shared BeginTag As String = "<span style=""text-decoration: line-through; color: red"">"
'    Public Shared EndTag As String = "</span>"
'End Class

'''' <summary>
'''' The class defines the begining and end html tag
'''' for marking up the added words in the merged
'''' file.
'''' </summary>
'Class Added
'    Public Shared BeginTag As String = "<span style=""background: SpringGreen"">"
'    Public Shared EndTag As String = "</span>"
'End Class

''' <summary>
''' Data structure for marking start and end indexes of a
''' sequence
''' </summary>
Class Sequence
    ''' <summary>
    ''' Default constructor
    ''' </summary>
    Public Sub New()
    End Sub

    ''' <summary>
    ''' Overloaded Constructor that takes the start
    ''' and end indexes of the sequence. Note that
    ''' the interval is open on right hand side, say,
    ''' it is like [startIndex, endIndex).
    ''' </summary>
    ''' <param name="startIndex">
    ''' The starting index of the sequence
    ''' </param>
    ''' <param name="endIndex">
    ''' The end index of the sequence.
    ''' </param>
    Public Sub New(ByVal startIndex As Integer, ByVal endIndex As Integer)
        Me.StartIndex = startIndex
        Me.EndIndex = endIndex
    End Sub
    ''' <summary>
    ''' The start index of the sequence
    ''' </summary>
    Public StartIndex As Integer

    ''' <summary>
    ''' The end index of the sequence. It is
    ''' open end.
    ''' </summary>
    Public EndIndex As Integer
End Class

''' <summary>
''' This class defines middle common sequence in the original
''' file and the modified file. It is called middle in the
''' sense that it is the common sequence when the furthest
''' forward reaching path in the top-down seaching first overlaps
''' the furthest backward reaching path in the bottom up search.
''' See the listed reference at the top for more details.
''' </summary>
Class MiddleSnake
    Public Sub New()
        Source = New Sequence()
        Destination = New Sequence()
    End Sub
    ''' <summary>
    ''' The indexes of middle snake in source sequence
    ''' </summary>
    Public Source As Sequence

    ''' <summary>
    ''' The indexes of middle snake in the destination
    ''' sequence
    ''' </summary>
    Public Destination As Sequence

    ''' <summary>
    ''' The length of the Shortest Edit Script for the
    ''' path this snake is found.
    ''' </summary>
    Public SES_Length As Integer
End Class


''' <summary>
''' An array indexer class that maps the index of an integer
''' array from -N ~ +N to 0 ~ 2N.
''' </summary>
Class IntVector
    Private data As Integer()
    Private N As Integer

    Public Sub New(ByVal N As Integer)
        data = New Integer(2 * N - 1) {}
        Me.N = N
    End Sub

    Default Public Property Item(ByVal index As Integer) As Integer
        Get
            Return data(N + index)
        End Get
        Set(ByVal value As Integer)
            data(N + index) = value
        End Set
    End Property
End Class


#End Region

#Region "Word and Words Collection"

''' <summary>
''' This class defines the data type for representing a
''' word. The word may have leading or tailing html tags
''' or other special characters. Those prefix or suffix
''' are not compared.
''' </summary>
Friend Class Word
    Implements IComparable
    Private _word As String = String.Empty
    Private _prefix As String = String.Empty
    Private _suffix As String = String.Empty

    ''' <summary>
    ''' Default constructor
    ''' </summary>
    Public Sub New()
        _word = String.Empty
        _prefix = String.Empty
        _suffix = String.Empty
    End Sub

    ''' <summary>
    ''' Overloaded constructor
    ''' </summary>
    ''' <param name="word">
    ''' The word
    ''' </param>
    ''' <param name="prefix">
    ''' The prefix of the word, such as html tags
    ''' </param>
    ''' <param name="suffix">
    ''' The suffix of the word, such as spaces.
    ''' </param>
    Public Sub New(ByVal word As String, ByVal prefix As String, ByVal suffix As String)
        _word = word
        _prefix = prefix
        _suffix = suffix
    End Sub

    ''' <summary>
    ''' The word itself
    ''' </summary>
    Public Property word() As String
        Get
            Return _word
        End Get
        Set(ByVal value As String)
            _word = value
        End Set
    End Property

    ''' <summary>
    ''' The prefix of the word
    ''' </summary>
    Public Property Prefix() As String
        Get
            Return _prefix
        End Get
        Set(ByVal value As String)
            _prefix = value
        End Set
    End Property

    ''' <summary>
    ''' The suffix of the word
    ''' </summary>
    Public Property Suffix() As String
        Get
            Return _suffix
        End Get
        Set(ByVal value As String)
            _suffix = value
        End Set
    End Property

    ''' <summary>
    ''' Reconstruct the text string from the word
    ''' itself without any other decoration.
    ''' </summary>
    ''' <returns>
    ''' Constructed string</returns>
    Public Function reconstruct() As String
        Return _prefix + _word + _suffix
    End Function

    ''' <summary>
    ''' Overloaded function reconstructing the text
    ''' string with additional decoration around the
    ''' _word.
    ''' </summary>
    ''' <param name="beginTag">
    ''' The begining html tag to mark the _word
    ''' </param>
    ''' <param name="endTag">
    ''' The end html tag to mark the _word
    ''' </param>
    ''' <returns>
    ''' The constructed string
    ''' </returns>
    Public Function reconstruct(ByVal beginTag As String, ByVal endTag As String) As String
        Return _prefix + beginTag + _word + endTag + _suffix
    End Function

#Region "IComparable Members"

    ''' <summary>
    ''' Implementation of the CompareTo. It compares
    ''' the _word field.
    ''' </summary>
    ''' <param name="obj"></param>
    ''' <returns></returns>
    Public Function CompareTo(ByVal obj As Object) As Integer Implements IComparable.CompareTo
        If TypeOf obj Is Word Then
            Return _word.CompareTo(DirectCast(obj, Word).word)
        Else
            Throw New ArgumentException("The obj is not a Word", obj.ToString())
        End If
    End Function

#End Region
End Class


''' <summary>
''' Strongly typed collection of Word object
''' </summary>
Friend Class WordsCollection
    Inherits CollectionBase
    ''' <summary>
    ''' Default constructor
    ''' </summary>
    Public Sub New()
    End Sub

    ''' <summary>
    ''' Constructor to populate collection from an ArrayList
    ''' </summary>
    ''' <param name="list" type="ArrayList">
    ''' ArrayList of Words
    ''' </param>
    Public Sub New(ByVal list As ArrayList)
        For Each item As Object In list
            If TypeOf item Is Word Then
                list.Add(item)
            End If
        Next
    End Sub

    ''' <summary>
    ''' Add a Word object to the collection
    ''' </summary>
    ''' <param name="item" type="Word">
    ''' Word object
    ''' </param>
    ''' <returns type="integer">
    ''' Zero based index of the added Word object in
    ''' the colleciton
    ''' </returns>
    Public Function Add(ByVal item As Word) As Integer
        Return List.Add(item)
    End Function

    ''' <summary>
    ''' Add Word object to the collection at specified index
    ''' </summary>
    ''' <param name="index" type="integer">
    ''' Zero based index
    ''' </param>
    ''' <param name="item" type="Word">
    ''' Word object
    ''' </param>
    Public Sub Insert(ByVal index As Integer, ByVal item As Word)
        List.Insert(index, item)
    End Sub

    ''' <summary>
    ''' Remove the Word object from collection
    ''' </summary>
    ''' <param name="item" type="Word">
    ''' Word object to be removed
    ''' </param>
    Public Sub Remove(ByVal item As Word)
        List.Remove(item)
    End Sub

    ''' <summary>
    ''' Check if the Word object is in the collection
    ''' </summary>
    ''' <param name="item" type="Word">
    ''' Word object
    ''' </param>
    ''' <returns type="bool">
    ''' Boolean value of the checking result
    ''' </returns>
    Public Function Contains(ByVal item As Word) As Boolean
        Return List.Contains(item)
    End Function

    ''' <summary>
    ''' Returns zero based index of the Word object in
    ''' the collection
    ''' </summary>
    ''' <param name="item" type="Word">
    ''' Word object to be checked for index
    ''' </param>
    ''' <returns type="integer">
    ''' Zero based index of Word object in the collection
    ''' </returns>
    Public Function IndexOf(ByVal item As Word) As Integer
        Return List.IndexOf(item)
    End Function

    ''' <summary>
    ''' Array indexing operator -- get Word object at
    ''' the index
    ''' </summary>
    Default Public Property Item(ByVal index As Integer) As Word
        Get
            Return DirectCast(List(index), Word)
        End Get
        Set(ByVal value As Word)
            List(index) = value
        End Set
    End Property

    ''' <summary>
    ''' Copy this WordsCollection to another one
    ''' starting at the specified index position
    ''' </summary>
    ''' <param name="col" type="WordsCollection">
    ''' WordsCollection to be copied to
    ''' </param>
    ''' <param name="index" type="integer">
    ''' Starting index to begin copy operations
    ''' </param>
    Public Sub CopyTo(ByVal col As WordsCollection, ByVal index As Integer)
        For i As Integer = index To List.Count - 1
            col.Add(Me(i))
        Next
    End Sub

    ''' <summary>
    ''' Overloaded. Copy this WordsCollection to another one
    ''' starting at the index zero
    ''' </summary>
    ''' <param name="col" type="WordCollection">
    ''' WordsCollection to copy to
    ''' </param>
    Public Sub CopyTo(ByVal col As WordsCollection)
        Me.CopyTo(col, 0)
    End Sub
End Class
#End Region

#Region "Html Text Paser"

''' <summary>
''' The class defines static method that processes html text
''' string in such a way that the text is striped out into
''' separate english words with html tags and some special
''' characters as the prefix or suffix of the words. This way,
''' the original html text string can be reconstructed to
''' retain the original appearance by concating each word
''' object in the collection in such way as word.prefix +
''' word.word + word.suffix.
'''
''' The generated words collection will be used to compare
''' the difference with another html text string in such format.
''' </summary>
Friend Class HtmlTextParser
    ''' <summary>
    ''' Static method that parses the passed-in string into
    ''' Words collection
    ''' </summary>
    ''' <param name="s">
    ''' String
    ''' </param>
    ''' <returns>
    ''' Words Collection
    ''' </returns>
    Public Shared Function parse(ByVal s As String) As WordsCollection
        Dim curPos As Integer = 0
        Dim prevPos As Integer
        Dim prefix As String = String.Empty
        Dim suffix As String = String.Empty
        Dim word As String = String.Empty
        Dim words As New WordsCollection()

        While curPos < s.Length
            ' eat the leading or tailing white spaces
            prevPos = curPos
            While curPos < s.Length AndAlso (Char.IsControl(s(curPos)) OrElse Char.IsWhiteSpace(s(curPos)))
                curPos += 1
            End While
            prefix += s.Substring(prevPos, curPos - prevPos)

            If curPos = s.Length Then
                ' it is possible that there are
                ' something in the prefix
                If prefix <> String.Empty Then
                    ' report a empty word with prefix.
                    words.Add(New Word("", prefix, ""))
                End If
                Exit While
            End If

            ' we have 3 different cases here,
            ' 1) if the string starts with '<', we assume
            ' that it is a html tag which will be put
            ' into prefix.
            ' 2) starts with '&', we need to check if it is
            ' "&nbsp;" or "&#xxx;". If it is the former,
            ' we treat it as prefix and if it is latter,
            ' we treat it as a word.
            ' 3) a string that may be a real word or a set
            ' of words separated by "&nbsp;" or may have
            ' leading special character or tailing
            ' punctuation.
            '
            ' Another possible case that is too complicated
            ' or expensive to handle is that some special
            ' characters are embeded inside the word with
            ' no space separation
            If s(curPos) = "<"c Then
                ' it is a html tag, consume it
                ' as prefix.
                prevPos = curPos
                While curPos < s.Length AndAlso s(curPos) <> ">"c
                    curPos += 1
                End While
                prefix += s.Substring(prevPos, curPos - prevPos + 1)

                If curPos = s.Length Then
                    ' if we come to this point, it means
                    ' the html tag is not closed. Anyway,
                    ' we are not validating html, so just
                    ' report a empty word with prefix.
                    words.Add(New Word("", prefix, ""))
                    Exit While
                End If
                ' curPos is pointing to '>', move
                ' it to next.
                curPos += 1
                If curPos = s.Length Then
                    ' the html tag is closed but nothing more
                    ' behind, so report a empty word with prefix.
                    words.Add(New Word("", prefix, ""))
                    Exit While
                End If
                Continue While
            ElseIf s(curPos) = "&"c Then
                prevPos = curPos

                ' case for html whitespace
                If curPos + 6 < s.Length AndAlso s.Substring(prevPos, 6) = "&nbsp;" Then
                    prefix += "&nbsp;"
                    curPos += 6
                    Continue While
                End If

                ' case for special character like "&#123;" etc
                Dim pattern As String = "&#[0-9]{3};"
                Dim r As New Regex(pattern)

                If curPos + 6 < s.Length AndAlso r.IsMatch(s.Substring(prevPos, 6)) Then
                    words.Add(New Word(s.Substring(prevPos, 6), prefix, ""))
                    prefix = String.Empty
                    curPos += 6
                    Continue While
                End If

                ' case for special character like "&#12;" etc
                pattern = "&#[0-9]{2};"
                r = New Regex(pattern)
                If curPos + 5 < s.Length AndAlso r.IsMatch(s.Substring(prevPos, 5)) Then
                    words.Add(New Word(s.Substring(prevPos, 5), prefix, ""))
                    prefix = String.Empty
                    curPos += 5
                    Continue While
                End If

                ' can't think of anything else that is special,
                ' have to treat it as a '&' leaded word. Hope
                ' it is just single '&' for and in meaning.
                prevPos = curPos
                While curPos < s.Length AndAlso Not Char.IsControl(s(curPos)) AndAlso Not Char.IsWhiteSpace(s(curPos)) AndAlso s(curPos) <> "<"c
                    curPos += 1
                End While
                word = s.Substring(prevPos, curPos - prevPos)

                ' eat the following witespace as suffix
                prevPos = curPos
                While curPos < s.Length AndAlso (Char.IsControl(s(curPos)) OrElse Char.IsWhiteSpace(s(curPos)))
                    curPos += 1
                End While
                suffix += s.Substring(prevPos, curPos - prevPos)

                words.Add(New Word(word, prefix, suffix))
                prefix = String.Empty
                suffix = String.Empty
            Else
                ' eat the word
                prevPos = curPos
                While curPos < s.Length AndAlso Not Char.IsControl(s(curPos)) AndAlso Not Char.IsWhiteSpace(s(curPos)) AndAlso s(curPos) <> "<"c AndAlso s(curPos) <> "&"c
                    curPos += 1
                End While
                word = s.Substring(prevPos, curPos - prevPos)

                ' if there are newlines or spaces follow
                ' the word, consume it as suffix
                prevPos = curPos
                While curPos < s.Length AndAlso (Char.IsControl(s(curPos)) OrElse Char.IsWhiteSpace(s(curPos)))
                    curPos += 1
                End While
                suffix = s.Substring(prevPos, curPos - prevPos)
                processWord(words, prefix, word, suffix)
                prefix = String.Empty
                suffix = String.Empty
            End If
        End While
        Return words
    End Function

    ''' <summary>
    ''' Further processing of a string
    ''' </summary>
    ''' <param name="words">
    ''' Collection that new word(s) will be added in
    ''' </param>
    ''' <param name="prefix">
    ''' prefix come with the string
    ''' </param>
    ''' <param name="word">
    ''' A string that may be a real word or have leading or tailing
    ''' special character
    ''' </param>
    ''' <param name="suffix">
    ''' suffix comes with the string.
    ''' </param>
    Private Shared Sub processWord(ByVal words As WordsCollection, ByVal prefix As String, ByVal word As String, ByVal suffix As String)
        ' the passed in word may have leading special
        ' characters such as '(', '"' etc or tailing
        ' punctuations. We need to sort this out.
        Dim length As Integer = word.Length

        If length = 1 Then
            words.Add(New Word(word, prefix, suffix))
        ElseIf Not Char.IsLetterOrDigit(word(0)) Then
            ' it is some kind of special character in the first place
            ' report it separately
            words.Add(New Word(word(0).ToString(), prefix, ""))
            words.Add(New Word(word.Substring(1), "", suffix))
            Return
        ElseIf Char.IsPunctuation(word(length - 1)) Then
            ' there is a end punctuation
            words.Add(New Word(word.Substring(0, length - 1), prefix, ""))
            words.Add(New Word(word(length - 1).ToString(), "", suffix))
        Else
            ' it is a real word(hope so)
            words.Add(New Word(word, prefix, suffix))
        End If
    End Sub
End Class

#End Region

#Region "Merge Engine"

''' <summary>
''' The class provides functionality to compare two html
''' files and merge them into a new file with differences
''' highlighted
''' </summary>
Public Class Merger
    Private _original As WordsCollection
    Private _modified As WordsCollection
    Private fwdVector As IntVector
    Private bwdVector As IntVector
    Private _CommentProps As CommentProperties

    Public Sub New(ByVal original As String, ByVal modified As String)
        ' parse the passed in string to words
        ' collections
        _original = HtmlTextParser.parse(original)
        _modified = HtmlTextParser.parse(modified)

        ' for hold the forward searching front-line
        ' in previous searching loop
        fwdVector = New IntVector(_original.Count + _modified.Count)

        ' for hold the backward searching front-line
        ' in the previous seaching loop
        bwdVector = New IntVector(_original.Count + _modified.Count)

        ' initialize the comment properties
        _CommentProps = New CommentProperties

        _CommentProps.Added = New CommentTags
        With _CommentProps.Added
            .BeginTag = "<span style=""background: SpringGreen"">"
            .EndTag = "</span>"
        End With

        _CommentProps.Removed = New CommentTags
        With _CommentProps.Removed
            .BeginTag = "<span style=""text-decoration: line-through; color: red"">"
            .EndTag = "</span>"
        End With

    End Sub
    Public Property CommentProperties() As CommentProperties
        Get
            Return _CommentProps
        End Get
        Set(ByVal value As CommentProperties)
            _CommentProps = value
        End Set
    End Property

    ''' <summary>
    ''' Return the number of words in the parsed original file.
    ''' </summary>
    Public ReadOnly Property WordsInOriginalFile() As Integer
        Get
            Return _original.Count
        End Get
    End Property

    ''' <summary>
    ''' Return the number of words in the parsed modified file
    ''' </summary>
    Public ReadOnly Property WordsInModifiedFile() As Integer
        Get
            Return _modified.Count
        End Get
    End Property

    ''' <summary>
    ''' In the edit graph for the sequences src and des, search for the
    ''' optimal(shortest) path from (src.StartIndex, des.StartIndex) to
    ''' (src.EndIndex, des.EndIndex).
    '''
    ''' The searching starts from both ends of the graph and when the
    ''' furthest forward reaching overlaps with the furthest backward
    ''' reaching, the overlapped point is reported as the middle point
    ''' of the shortest path.
    '''
    ''' See the listed reference for the detailed description of the
    ''' algorithm
    ''' </summary>
    ''' <param name="src">
    ''' Represents a (sub)sequence of _original
    ''' </param>
    ''' <returns>
    ''' The found middle snake
    ''' </returns>
    Private Function findMiddleSnake(ByVal src As Sequence, ByVal des As Sequence) As MiddleSnake
        Dim d As Integer, k As Integer
        Dim x As Integer, y As Integer
        Dim midSnake As New MiddleSnake()

        ' the range of diagonal values
        Dim minDiag As Integer = src.StartIndex - des.EndIndex
        Dim maxDiag As Integer = src.EndIndex - des.StartIndex

        ' middle point of forward searching
        Dim fwdMid As Integer = src.StartIndex - des.StartIndex
        ' middle point of backward searching
        Dim bwdMid As Integer = src.EndIndex - des.EndIndex

        ' forward seaching range
        Dim fwdMin As Integer = fwdMid
        Dim fwdMax As Integer = fwdMid

        ' backward seaching range
        Dim bwdMin As Integer = bwdMid
        Dim bwdMax As Integer = bwdMid

        Dim odd As Boolean = ((fwdMin - bwdMid) And 1) = 1

        fwdVector(fwdMid) = src.StartIndex
        bwdVector(bwdMid) = src.EndIndex

#If (MyDEBUG) Then
             Debug.WriteLine("-- Entering Function findMiddleSnake(src, des) --")
#End If
        d = 1
        While True
            ' extend or shrink the search range
            If fwdMin > minDiag Then
                fwdVector(System.Threading.Interlocked.Decrement(fwdMin) - 1) = -1
            Else
                fwdMin += 1
            End If

            If fwdMax < maxDiag Then
                fwdVector(System.Threading.Interlocked.Increment(fwdMax) + 1) = -1
            Else
                fwdMax -= 1
            End If
#If (MyDEBUG) Then
                 Debug.WriteLine(d, " D path")
#End If
            For k = fwdMax To fwdMin Step -2

                ' top-down search
                If fwdVector(k - 1) < fwdVector(k + 1) Then
                    x = fwdVector(k + 1)
                Else
                    x = fwdVector(k - 1) + 1
                End If
                y = x - k
                midSnake.Source.StartIndex = x
                midSnake.Destination.StartIndex = y

                While x < src.EndIndex AndAlso y < des.EndIndex AndAlso _original(x).CompareTo(_modified(y)) = 0
                    x += 1
                    y += 1
                End While

                ' update forward vector
                fwdVector(k) = x
#If (MyDEBUG) Then
                     Debug.WriteLine(" Inside forward loop")
                     Debug.WriteLine(k, " Diagonal value")
                     Debug.WriteLine(x, " X value")
                     Debug.WriteLine(y, " Y value")
#End If
                If odd AndAlso k >= bwdMin AndAlso k <= bwdMax AndAlso x >= bwdVector(k) Then
                    ' this is the snake we are looking for
                    ' and set the end indeses of the snake
                    midSnake.Source.EndIndex = x
                    midSnake.Destination.EndIndex = y
                    midSnake.SES_Length = 2 * d - 1
#If (MyDEBUG) Then
                         Debug.WriteLine("!!!Report snake from forward search")
                         Debug.WriteLine(midSnake.Source.StartIndex, " middle snake source start index")
                         Debug.WriteLine(midSnake.Source.EndIndex, " middle snake source end index")
                         Debug.WriteLine(midSnake.Destination.StartIndex, " middle snake destination start index")
                         Debug.WriteLine(midSnake.Destination.EndIndex, " middle snake destination end index")
#End If
                    Return midSnake
                End If
            Next

            ' extend the search range
            If bwdMin > minDiag Then
                bwdVector(System.Threading.Interlocked.Decrement(bwdMin) - 1) = Integer.MaxValue
            Else
                bwdMin += 1
            End If

            If bwdMax < maxDiag Then
                bwdVector(System.Threading.Interlocked.Increment(bwdMax) + 1) = Integer.MaxValue
            Else
                bwdMax -= 1
            End If
            For k = bwdMax To bwdMin Step -2

                ' bottom-up search
                If bwdVector(k - 1) < bwdVector(k + 1) Then
                    x = bwdVector(k - 1)
                Else
                    x = bwdVector(k + 1) - 1
                End If
                y = x - k
                midSnake.Source.EndIndex = x
                midSnake.Destination.EndIndex = y

                While x > src.StartIndex AndAlso y > des.StartIndex AndAlso _original(x - 1).CompareTo(_modified(y - 1)) = 0
                    x -= 1
                    y -= 1
                End While
                ' update backward Vector
                bwdVector(k) = x

#If (MyDEBUG) Then
                     Debug.WriteLine(" Inside backward loop")
                     Debug.WriteLine(k, " Diagonal value")
                     Debug.WriteLine(x, " X value")
                     Debug.WriteLine(y, " Y value")
#End If
                If Not odd AndAlso k >= fwdMin AndAlso k <= fwdMax AndAlso x <= fwdVector(k) Then
                    ' this is the snake we are looking for
                    ' and set the start indexes of the snake
                    midSnake.Source.StartIndex = x
                    midSnake.Destination.StartIndex = y
                    midSnake.SES_Length = 2 * d
#If (MyDEBUG) Then
                         Debug.WriteLine("!!!Report snake from backward search")
                         Debug.WriteLine(midSnake.Source.StartIndex, " middle snake source start index")
                         Debug.WriteLine(midSnake.Source.EndIndex, " middle snake source end index")
                         Debug.WriteLine(midSnake.Destination.StartIndex, " middle snake destination start index")
                         Debug.WriteLine(midSnake.Destination.EndIndex, " middle snake destination end index")
#End If
                    Return midSnake
                End If
            Next
            d += 1
        End While
        Return Nothing
    End Function

    ''' <summary>
    ''' The function merges the two sequences and returns the merged
    ''' html text string with deleted(exists in source sequence but
    ''' not in destination sequence) and added(exists in destination
    ''' but not in source) decorated extra html tags defined in class
    ''' commentoff and class added.
    ''' </summary>
    ''' <param name="src">
    ''' The source sequence
    ''' </param>
    ''' <returns>
    ''' The merged html string
    ''' </returns>
    Private Function doMerge(ByVal src As Sequence, ByVal des As Sequence) As String
        Dim snake As MiddleSnake
        Dim s As Sequence
        Dim result As New StringBuilder()
        Dim tail As String = String.Empty

        Dim y As Integer = des.StartIndex

        ' strip off the leading common sequence
        While src.StartIndex < src.EndIndex AndAlso des.StartIndex < des.EndIndex AndAlso _original(src.StartIndex).CompareTo(_modified(des.StartIndex)) = 0
            src.StartIndex += 1
            des.StartIndex += 1
        End While

        If des.StartIndex > y Then
            s = New Sequence(y, des.StartIndex)
            result.Append(constructText(s, SequenceStatus.NoChange))
        End If

        y = des.EndIndex

        ' strip off the tailing common sequence
        While src.StartIndex < src.EndIndex AndAlso des.StartIndex < des.EndIndex AndAlso _original(src.EndIndex - 1).CompareTo(_modified(des.EndIndex - 1)) = 0
            src.EndIndex -= 1
            des.EndIndex -= 1
        End While

        If des.EndIndex < y Then
            s = New Sequence(des.EndIndex, y)
            tail = constructText(s, SequenceStatus.NoChange)
        End If

        ' length of the sequences
        Dim N As Integer = src.EndIndex - src.StartIndex
        Dim M As Integer = des.EndIndex - des.StartIndex

        ' Special cases
        If N < 1 AndAlso M < 1 Then
            ' both source and destination are
            ' empty
            Return (result.Append(tail)).ToString()
        ElseIf N < 1 Then
            ' source is already empty, report
            ' destination as added
            result.Append(constructText(des, SequenceStatus.Inserted))
            result.Append(tail)
            Return result.ToString()
        ElseIf M < 1 Then
            ' destination is empty, report source as
            ' deleted
            result.Append(constructText(src, SequenceStatus.Deleted))
            result.Append(tail)
            Return result.ToString()
        ElseIf M = 1 AndAlso N = 1 Then
            ' each of source and destination has only
            ' one word left. At this point, we are sure
            ' that they are not equal.
            result.Append(constructText(src, SequenceStatus.Deleted))
            result.Append(constructText(des, SequenceStatus.Inserted))
            result.Append(tail)
            Return result.ToString()
        Else
            ' find the middle snake
            snake = findMiddleSnake(src, des)

            If snake.SES_Length > 1 Then
                ' prepare the parameters for recursion
                Dim leftSrc As New Sequence(src.StartIndex, snake.Source.StartIndex)
                Dim leftDes As New Sequence(des.StartIndex, snake.Destination.StartIndex)
                Dim rightSrc As New Sequence(snake.Source.EndIndex, src.EndIndex)
                Dim rightDes As New Sequence(snake.Destination.EndIndex, des.EndIndex)

                result.Append(doMerge(leftSrc, leftDes))
                If snake.Source.StartIndex < snake.Source.EndIndex Then
                    ' the snake is not empty, report it as common
                    ' sequence
                    result.Append(constructText(snake.Destination, SequenceStatus.NoChange))
                End If
                result.Append(doMerge(rightSrc, rightDes))
                result.Append(tail)
                Return result.ToString()
            Else
                ' Separating this case out can at least save one
                ' level of recursion.
                '
                ' Only one edit edge suggests the 4 possible cases.
                ' if N > M, it will be either:
                ' - or \
                ' \ (case 1) \ (case 2)
                ' \ -
                ' if N < M, it will be either:
                ' | or \
                ' \ (case 3) \ (case 4)
                ' \ |
                ' N and M can't be equal!
                If N > M Then
                    If src.StartIndex <> snake.Source.StartIndex Then
                        ' case 1
                        Dim leftSrc As New Sequence(src.StartIndex, snake.Source.StartIndex)
                        result.Append(constructText(leftSrc, SequenceStatus.Deleted))
                        result.Append(constructText(snake.Destination, SequenceStatus.NoChange))
                    Else
                        ' case 2
                        Dim rightSrc As New Sequence(snake.Source.StartIndex, src.EndIndex)
                        result.Append(constructText(rightSrc, SequenceStatus.Deleted))
                        result.Append(constructText(snake.Destination, SequenceStatus.NoChange))
                    End If
                Else
                    If des.StartIndex <> snake.Destination.StartIndex Then
                        ' case 3
                        Dim upDes As New Sequence(des.StartIndex, snake.Destination.StartIndex)
                        result.Append(constructText(upDes, SequenceStatus.Inserted))
                        result.Append(constructText(snake.Destination, SequenceStatus.NoChange))
                    Else
                        ' case 4
                        Dim bottomDes As New Sequence(snake.Destination.EndIndex, des.EndIndex)
                        result.Append(constructText(bottomDes, SequenceStatus.Inserted))
                        result.Append(constructText(snake.Destination, SequenceStatus.NoChange))
                    End If
                End If
                result.Append(tail)
                Return result.ToString()
            End If
        End If
    End Function

    ''' <summary>
    ''' The function returns a html text string reconstructed
    ''' from the sub collection of words its starting and ending
    ''' indexes are marked by parameter seq and its collection is
    ''' denoted by parameter status. If the status is "deleted",
    ''' then the _original collection is used, otherwise, _modified
    ''' is used.
    ''' </summary>
    ''' <param name="seq">
    ''' Sequence object that marks the start index and end
    ''' index of the sub sequence
    ''' </param>
    ''' <param name="status">
    ''' Denoting the status of the sequence. When its value is
    ''' Deleted or Added, some extra decoration will be added
    ''' around the word.
    ''' </param>
    ''' <returns>
    ''' The html text string constructed
    ''' </returns>
    Private Function constructText(ByVal seq As Sequence, ByVal status As SequenceStatus) As String
        Dim result As New StringBuilder()

        Select Case status
            Case SequenceStatus.Deleted
                For i As Integer = seq.StartIndex To seq.EndIndex - 1
                    ' the sequence exists in _original and
                    ' will be marked as deleted in the merged
                    ' file.
                    result.Append(_original(i).reconstruct(_CommentProps.Removed.BeginTag, _CommentProps.Removed.EndTag))
                Next
                Exit Select
            Case SequenceStatus.Inserted
                For i As Integer = seq.StartIndex To seq.EndIndex - 1
                    ' the sequence exists in _modified and
                    ' will be marked as added in the merged
                    ' file.
                    result.Append(_modified(i).reconstruct(_CommentProps.Added.BeginTag, _CommentProps.Added.EndTag))
                Next
                Exit Select
            Case SequenceStatus.NoChange
                For i As Integer = seq.StartIndex To seq.EndIndex - 1
                    ' the sequence exists in both _original and
                    ' _modified and will be left as what it is in
                    ' the merged file. We chose to reconstruct from
                    ' _modified collection
                    result.Append(_modified(i).reconstruct())
                Next
                Exit Select
            Case Else
                ' this will not happen (hope)
                Exit Select
        End Select
        Return result.ToString()
    End Function

    ''' <summary>
    ''' The public function merges the two copies of
    ''' files stored inside this class. The html tags
    ''' of the destination file is used in the merged
    ''' file.
    ''' </summary>
    ''' <returns>
    ''' The merged file
    ''' </returns>
    Public Function merge() As String
        Dim src As New Sequence(0, _original.Count)
        Dim des As New Sequence(0, _modified.Count)

        Return doMerge(src, des)
    End Function
End Class
#End Region
