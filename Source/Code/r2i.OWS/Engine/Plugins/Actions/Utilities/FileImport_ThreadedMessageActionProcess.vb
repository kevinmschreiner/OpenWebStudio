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
Imports r2i.OWS.Framework.Utilities.Compatibility, r2i.OWS.Actions.Utilities

Namespace r2i.OWS.Actions.Utilities
    Public Class FileImport_ThreadedMessageActionProcess
        Inherits ThreadedMessageActionProcess

        Public Delegate Sub FileImport_ThreadedMessageActionProcess_Address(ByVal Source As FileImport_ThreadedMessageActionProcess)
        Public Debugger As Text.StringBuilder
        Public SourceStream As IO.Stream
        Public DestinationIncludeColumnName As Boolean
        Public DestinationColumnMappings As r2i.OWS.Framework.Plugins.Actions.MessageAction_File_ColumnMappings
        Public Delimiter As String
        Public DestinationTarget As String
        Public StartPosition As FileImport_ThreadedMessageActionProcess_Address
        Public ThreadObj As System.Threading.Thread

        Public Sub Start()
            'Threading.Thread.GetDomain.SetPrincipalPolicy(System.Security.Principal.PrincipalPolicy.WindowsPrincipal)
            StartPosition.Invoke(Me)
        End Sub
    End Class
End Namespace

