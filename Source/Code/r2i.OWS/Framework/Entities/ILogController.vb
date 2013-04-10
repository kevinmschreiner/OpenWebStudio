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
Imports System.Collections
Imports System.Collections.Generic
Imports r2i.OWS.Framework.DataAccess

Namespace r2i.OWS.Framework.Entities
    Public Enum LogSeverity
        ''' <summary>
        ''' Use for logging a message
        ''' </summary>
        NONE

        ''' <summary>
        ''' Minor exception
        ''' </summary>
        Low

        ''' <summary>
        ''' Possibly a problem - worth noting
        ''' </summary>
        Medium

        ''' <summary>
        ''' Could possibly lead to more severe problems in the future
        ''' </summary>        
        High

        ''' <summary>
        ''' Catastrophic. Investigate this immediately.
        ''' </summary>
        Severe

        ''' <summary>
        ''' Generic error. Could be ok - could be really bad.
        ''' </summary>
        [Error]
    End Enum
    Public Interface ILogController
        Sub LogMessage(ByVal sender As Object, ByVal Message As String, ByVal Severity As LogSeverity)
        Sub LogError(ByVal sender As Object, ByVal ex As Exception, ByVal Severity As LogSeverity)
        Sub LogError(ByVal sender As Object, ByVal ex As Exception, ByVal Severity As LogSeverity, ByVal OtherParameters As Dictionary(Of String, String))
    End Interface
End Namespace