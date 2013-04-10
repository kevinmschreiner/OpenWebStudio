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
Imports r2i.OWS.Framework.DataAccess
Imports r2i.OWS.Wrapper.DNN.DataAccess.Factories

Namespace Entities
    Public Class SkinController
        Implements ISkinController

        Public Function GetParentSkin() As ISkin Implements ISkinController.GetParentSkin
            Return SkinFactory.GetParentSkinDnn()
        End Function

        Public Function SkinType(ByVal name As String) As Integer Implements ISkinController.SkinType
            Return SkinFactory.Instance.SkinType(name)
        End Function

        Public Function GetSkin(ByVal skinRoot As String, ByVal portalId As String, ByVal skinType As Integer) As ISkinInfo Implements ISkinController.GetSkin
            Return SkinFactory.GetSkin(skinRoot, portalId, CType(skinType, DotNetNuke.UI.Skins.SkinType))
        End Function

        Public Function GetSkinFieldInfos() As System.Reflection.FieldInfo() Implements ISkinController.GetSkinFieldInfos
            Return SkinFactory.Instance.GetSkinFieldInfos()
        End Function
    End Class
End Namespace
