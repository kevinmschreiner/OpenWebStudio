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
Namespace DataAccess
    Public Class Skin
        Implements ISkin

        'Public Function GetParentSkin(ByVal portalModuleBase As IPortalModuleBaseUI) As ISkin Implements ISkin.GetParentSkin
        '    Return CType(GetParentSkinDnn(CType(portalModuleBase, DotNetNuke.Entities.Modules.PortalModuleBase)), ISkin)
        'End Function

        'Public Shared Function GetParentSkinDnn(ByVal portalModuleBase As DotNetNuke.Entities.Modules.PortalModuleBase) As DotNetNuke.UI.Skins.Skin
        '    Return DotNetNuke.UI.Skins.Skin.GetParentSkin(portalModuleBase)
        'End Function

        Public Property SkinPath() As String Implements ISkin.SkinPath
            Get
                Return Nothing
            End Get
            Set(ByVal value As String)

            End Set
        End Property

        Public Property SkinId() As String Implements ISkin.SkinId
            Get
                Return Nothing
            End Get
            Set(ByVal value As String)

            End Set
        End Property
    End Class
End Namespace


