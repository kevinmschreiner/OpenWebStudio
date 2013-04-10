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
'
' Bi4ce Toolbar -  http://dnn.bi4ce.com
' Copyright (c) 2005
' by Kevin M Schreiner ( sales@bi4ce.com ) of Business Intelligence Force, Inc. ( http://www.bi4ce.com )
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
Imports r2i.OWS
Imports r2i.OWS.Framework.DataAccess
' DEALINGS IN THE SOFTWARE.
Namespace Bi4ce.Modules.Toolbar
    Public Class ToolbarController
        Public Function GetToolbarSetting(ByVal ConfigurationId As Guid) As String
            Dim dr As IDataReader = DataProvider.Instance().GetToolbarSetting(ConfigurationId)
            Dim str As String = Nothing
            While dr.Read()

                If Not dr.IsDBNull(0) Then
                    str = dr.GetString(0)
                Else
                    str = String.Empty
                End If

            End While

            dr.Close()

            Return str

        End Function
        Public Function GetToolbarSettings(ByVal ConfigurationID As Guid) As IDataReader
            Try
                Dim reader As IDataReader = DataProvider.Instance().GetToolbarSettings(ConfigurationID)
                Return reader
            Catch ex As Exception
                'ROMAIN: 09/18/07
                'TODO: CHANGE EXCEPTIONS
                'DotNetNuke.Services.Exceptions.LogException(ex)
            End Try
            Return Nothing
        End Function
        Public Sub UpdateToolbarSetting(ByVal ConfigurationID As Guid, ByVal SettingValue As String)

            Dim dr As IDataReader = DataProvider.Instance().GetToolbarSetting(ConfigurationID)

            If dr.Read Then
                DataProvider.Instance().UpdateToolbarSetting(ConfigurationID, SettingValue)
            Else
                DataProvider.Instance().AddToolbarSetting(ConfigurationID, SettingValue)
            End If
            dr.Close()

        End Sub
    End Class
End Namespace