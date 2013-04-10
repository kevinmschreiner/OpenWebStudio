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
Imports System.Net.Mail
Imports System.Collections.Generic
Imports System.Net
Imports System.Reflection
Imports System.Configuration
Imports System.Text
Imports System.Drawing
Imports System.Math

Namespace r2i.OWS.Framework.Utilities.Engine
    Public Class Graphics
        Public Shared Function GetImageCodec(ByVal mimeType As String) As Drawing.Imaging.ImageCodecInfo
            Dim codec As Drawing.Imaging.ImageCodecInfo
            For Each codec In Drawing.Imaging.ImageCodecInfo.GetImageEncoders()
                If String.Compare(codec.MimeType, mimeType, True) = 0 Then
                    Return codec
                End If
            Next
            Return Nothing
        End Function
        Public Shared Function RotateImage(ByRef BMP As System.Drawing.Bitmap, ByRef DestinationTarget As String, ByVal Angle As String, ByVal Quality As Long, Optional ByRef Debugger As Debugger = Nothing) As IO.Stream
            Dim sStream As New IO.MemoryStream
            Dim bitmap As System.Drawing.Bitmap
            'Dim aWidth As Integer = BMP.Width
            'Dim aHeight As Integer = BMP.Height
            Dim flip As String = ""
            If Angle.Contains("X") Then
                flip &= "X"
                Angle = Angle.Replace("X", "")
            End If
            If Angle.Contains("Y") Then
                flip &= "Y"
                Angle = Angle.Replace("Y", "")
            End If
            If Not Debugger Is Nothing Then
                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Rotating Image " & Angle.ToString(), False)
            End If
            Dim aAngle As Integer = CInt(Angle)
            If aAngle Mod 90 = 0 Then
                bitmap = New Bitmap(BMP)
                Select Case aAngle
                    Case 0
                        Select Case flip
                            Case ""
                                bitmap.RotateFlip(RotateFlipType.RotateNoneFlipNone)
                            Case "X"
                                bitmap.RotateFlip(RotateFlipType.RotateNoneFlipX)
                            Case "Y"
                                bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY)
                            Case "XY"
                                bitmap.RotateFlip(RotateFlipType.RotateNoneFlipXY)
                        End Select
                    Case 90
                        Select Case flip
                            Case ""
                                bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone)
                            Case "X"
                                bitmap.RotateFlip(RotateFlipType.Rotate90FlipX)
                            Case "Y"
                                bitmap.RotateFlip(RotateFlipType.Rotate90FlipY)
                            Case "XY"
                                bitmap.RotateFlip(RotateFlipType.Rotate90FlipXY)
                        End Select
                    Case 180
                        Select Case flip
                            Case ""
                                bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone)
                            Case "X"
                                bitmap.RotateFlip(RotateFlipType.Rotate180FlipX)
                            Case "Y"
                                bitmap.RotateFlip(RotateFlipType.Rotate180FlipY)
                            Case "XY"
                                bitmap.RotateFlip(RotateFlipType.Rotate180FlipXY)
                        End Select
                    Case 270
                        Select Case flip
                            Case ""
                                bitmap.RotateFlip(RotateFlipType.Rotate270FlipNone)
                            Case "X"
                                bitmap.RotateFlip(RotateFlipType.Rotate270FlipX)
                            Case "Y"
                                bitmap.RotateFlip(RotateFlipType.Rotate270FlipY)
                            Case "XY"
                                bitmap.RotateFlip(RotateFlipType.Rotate270FlipXY)
                        End Select
                End Select
            Else
                ' Make an array of points defining the
                ' image's corners.
                Dim wid As Single = BMP.Width
                Dim hgt As Single = BMP.Height
                Dim corners As Point() = { _
                    New Point(0, 0), _
                    New Point(wid, 0), _
                    New Point(0, hgt), _
                    New Point(wid, hgt)}

                ' Translate to center the bounding box at the origin.
                Dim cx As Single = wid / 2
                Dim cy As Single = hgt / 2
                Dim i As Long
                For i = 0 To 3
                    corners(i).X -= cx
                    corners(i).Y -= cy
                Next i

                ' Rotate.
                Dim theta As Single = Single.Parse(Angle) * PI _
                    / 180.0
                Dim sin_theta As Single = Sin(theta)
                Dim cos_theta As Single = Cos(theta)
                Dim X As Single
                Dim Y As Single
                For i = 0 To 3
                    X = corners(i).X
                    Y = corners(i).Y
                    corners(i).X = X * cos_theta + Y * sin_theta
                    corners(i).Y = -X * sin_theta + Y * cos_theta
                Next i

                ' Translate so X >= 0 and Y >=0 for all corners.
                Dim xmin As Single = corners(0).X
                Dim ymin As Single = corners(0).Y
                For i = 1 To 3
                    If xmin > corners(i).X Then xmin = corners(i).X
                    If ymin > corners(i).Y Then ymin = corners(i).Y
                Next i
                For i = 0 To 3
                    corners(i).X -= xmin
                    corners(i).Y -= ymin
                Next i

                ' Create an output Bitmap and Graphics object.
                bitmap = New Bitmap(CInt(-2 * xmin), CInt(-2 * _
                    ymin))
                Dim g As System.Drawing.Graphics = System.Drawing.Graphics.FromImage(bitmap)
                g.SmoothingMode = Drawing.Drawing2D.SmoothingMode.HighQuality
                g.CompositingQuality = Drawing.Drawing2D.CompositingQuality.HighQuality
                g.InterpolationMode = Drawing.Drawing2D.InterpolationMode.HighQualityBicubic

                ' Drop the last corner lest we confuse DrawImage, 
                ' which expects an array of three corners.
                ReDim Preserve corners(2)

                ' Draw the result onto the output Bitmap.
                g.DrawImage(BMP, corners)
            End If

            If DestinationTarget.ToUpper.EndsWith("JPEG") Or DestinationTarget.ToUpper.EndsWith("JPG") Then
                Dim imgCodec As Drawing.Imaging.ImageCodecInfo = GetImageCodec("image/jpeg")
                If Not imgCodec Is Nothing Then
                    'Determine required number of parameters
                    Dim paramSize As Integer = 2

                    Dim encoderParams As New Drawing.Imaging.EncoderParameters(paramSize)
                    encoderParams.Param(0) = New Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, Quality)
                    encoderParams.Param(1) = New Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.ColorDepth, 24L)

                    bitmap.Save(sStream, imgCodec, encoderParams)
                Else
                    bitmap.Save(sStream, System.Drawing.Imaging.ImageFormat.Jpeg)
                End If
            ElseIf DestinationTarget.ToUpper.EndsWith("PNG") Then
                bitmap.Save(sStream, System.Drawing.Imaging.ImageFormat.Png)
            Else
                bitmap.Save(sStream, System.Drawing.Imaging.ImageFormat.Gif)
            End If
            Return sStream
        End Function
        Public Shared Function SmartSize(ByRef BMP As System.Drawing.Bitmap, ByVal allowedChange As Integer) As System.Drawing.Rectangle
            Dim r As New System.Drawing.Rectangle
            Dim c As System.Drawing.Color = FrequentBorderColor(BMP)
            Dim cMinR As Integer = c.R - allowedChange
            Dim cMaxR As Integer = c.R + allowedChange
            Dim cMinG As Integer = c.G - allowedChange
            Dim cMaxG As Integer = c.G + allowedChange
            Dim cMinB As Integer = c.B - allowedChange
            Dim cMaxB As Integer = c.B + allowedChange

            Dim current As System.Drawing.Color
            Dim cR As Integer = 0
            Dim cG As Integer = 0
            Dim cB As Integer = 0
            r.X = BMP.Width
            r.Y = BMP.Height
            r.Width = 0
            r.Height = 0
            For y As Integer = 0 To BMP.Height - 1
                For x As Integer = 0 To BMP.Width - 1
                    current = BMP.GetPixel(x, y)
                    cR = current.R
                    cG = current.G
                    cB = current.B
                    If ((cR < cMinR OrElse cR > cMaxR) OrElse (cG < cMinG OrElse cG > cMaxG) OrElse (cB < cMinB OrElse cB > cMaxB)) Then
                        If (x < r.X) Then
                            r.X = x
                        End If
                        If (y < r.Y) Then
                            r.Y = y
                        End If
                        If (x > r.Width) Then
                            r.Width = x
                        End If
                        If (y > r.Height) Then
                            r.Height = y
                        End If
                    End If
                Next
            Next
            Return r
        End Function
        Public Class ColorLimits
            Public min_Red As Integer
            Public max_Red As Integer
            Public min_Green As Integer
            Public max_Green As Integer
            Public min_Blue As Integer
            Public max_Blue As Integer
            Public Sub New(ByVal initialColor As System.Drawing.Color, ByVal allowedChange As Integer)
                min_Red = initialColor.R - allowedChange
                max_Red = initialColor.R + allowedChange
                min_Green = initialColor.G - allowedChange
                max_Green = initialColor.G + allowedChange
                min_Blue = initialColor.B - allowedChange
                max_Blue = initialColor.B + allowedChange
            End Sub
            Public Function Check(ByVal checkColor As System.Drawing.Color) As Boolean
                If ((checkColor.R < min_Red OrElse checkColor.R > max_Red) OrElse (checkColor.G < min_Green OrElse checkColor.G > max_Green) OrElse (checkColor.B < min_Blue OrElse checkColor.B > max_Blue)) Then
                    Return True
                End If
                Return False
            End Function
        End Class
        Public Shared Function FindPointOfChange(ByRef BMP As System.Drawing.Bitmap, ByVal p As GPoint, ByVal DirectionX As Integer, ByVal directionY As Integer, ByVal limitP As GPoint, ByVal baseColor As System.Drawing.Color, ByVal AllowedChange As Integer, ByVal TopToBottom As Boolean) As GPoint
            Dim limitColor As New ColorLimits(baseColor, AllowedChange)

            Dim found As Boolean = False
            Dim x As Int32
            Dim y As Int32
            If TopToBottom Then
                For x = p.X To limitP.X Step DirectionX
                    For y = p.Y To limitP.Y Step directionY
                        found = limitColor.Check(BMP.GetPixel(x, y))
                        If found Then
                            Exit For
                        End If
                    Next
                    If found Then
                        Exit For
                    End If
                Next
            Else
                For y = p.Y To limitP.Y Step directionY
                    For x = p.X To limitP.X Step DirectionX
                        found = limitColor.Check(BMP.GetPixel(x, y))
                        If found Then
                            Exit For
                        End If
                    Next
                    If found Then
                        Exit For
                    End If
                Next
            End If
            If found Then
                Return New GPoint(x, y)
            End If
            Return Nothing
        End Function
        Public Class GPoint
            Public X As Int32
            Public Y As Int32
            Public Sub New(ByVal x As Int32, ByVal y As Int32)
                Me.X = x
                Me.Y = y
            End Sub
            Public Sub New(ByVal x As Int32, ByVal y As Int32, ByRef BMP As Bitmap)
                If (x >= BMP.Width) Then
                    x = BMP.Width - 1
                End If
                If (y >= BMP.Height) Then
                    y = BMP.Height - 1
                End If
                Me.X = x
                Me.Y = y
            End Sub
            Public Function toPoint() As System.Drawing.Point
                Return New System.Drawing.Point(X, Y)
            End Function
            Public Function Compare(ByVal comparison As GPoint) As Boolean
                If comparison.X = Me.X AndAlso comparison.Y = Me.Y Then
                    Return True
                End If
                Return False
            End Function
        End Class
        Public Shared Function SmartSize_Fast(ByRef BMP As System.Drawing.Bitmap, ByVal allowedChange As Integer) As System.Drawing.Rectangle
            Dim c As System.Drawing.Color = FrequentBorderColor(BMP)
            Dim r As New System.Drawing.Rectangle

            Dim p1 As GPoint
            Dim p2 As GPoint
            Dim p3 As GPoint
            Dim p4 As GPoint

            p1 = FindPointOfChange(BMP, New GPoint(0, 0), 1, 1, New GPoint(BMP.Width - 1, BMP.Height - 1), c, allowedChange, False)
            If Not p1 Is Nothing Then
                p2 = FindPointOfChange(BMP, New GPoint(BMP.Width - 1, BMP.Height - 1), -1, -1, New GPoint(0, p1.Y), c, allowedChange, False) 'slight overlap...
                If p2 Is Nothing Then
                    p2 = p1
                End If
                If Not p1.Compare(p2) Then
                    p3 = FindPointOfChange(BMP, New GPoint(BMP.Width - 1, p1.Y), -1, 1, New GPoint(p2.X + 1, p2.Y - 1, BMP), c, allowedChange, True)
                    p4 = FindPointOfChange(BMP, New GPoint(0, p2.Y), 1, -1, New GPoint(p1.X - 1, p1.Y + 1, BMP), c, allowedChange, True)
                    If p3 Is Nothing Then
                        p3 = p2
                    End If
                    If p4 Is Nothing Then
                        p4 = p1
                    End If
                    Dim minP As New GPoint(BMP.Width, BMP.Height)
                    Dim maxP As New GPoint(0, 0)
                    If p1.X > maxP.X Then maxP.X = p1.X
                    If p1.X < minP.X Then minP.X = p1.X
                    If p2.X > maxP.X Then maxP.X = p2.X
                    If p2.X < minP.X Then minP.X = p2.X
                    If p3.X > maxP.X Then maxP.X = p3.X
                    If p3.X < minP.X Then minP.X = p3.X
                    If p4.X > maxP.X Then maxP.X = p4.X
                    If p4.X < minP.X Then minP.X = p4.X

                    If p1.Y > maxP.Y Then maxP.Y = p1.Y
                    If p1.Y < minP.Y Then minP.Y = p1.Y
                    If p2.Y > maxP.Y Then maxP.Y = p2.Y
                    If p2.Y < minP.Y Then minP.Y = p2.Y
                    If p3.Y > maxP.Y Then maxP.Y = p3.Y
                    If p3.Y < minP.Y Then minP.Y = p3.Y
                    If p4.Y > maxP.Y Then maxP.Y = p4.Y
                    If p4.Y < minP.Y Then minP.Y = p4.Y

                    If Not minP.Compare(maxP) Then
                        r.X = minP.X
                        r.Y = minP.Y
                        r.Width = maxP.X
                        r.Height = maxP.Y
                    Else
                        r.X = minP.X
                        r.Y = minP.Y
                        r.Width = 1
                        r.Height = 1
                    End If
                Else
                    'DONE - p1 is the only point
                    r.X = p1.X
                    r.Y = p1.Y
                    r.Width = 1
                    r.Height = 1
                End If
            Else
                'all blank
                r.X = 0
                r.Y = 0
                r.Width = 0
                r.Height = 0
            End If

            Return r
        End Function
        Public Shared Function FrequentBorderColor(ByRef BMP As System.Drawing.Bitmap) As System.Drawing.Color
            Dim colors As System.Collections.Generic.Dictionary(Of String, Long) = New Dictionary(Of String, Long)
            Dim maxKey As String = Nothing
            Dim result As System.Drawing.Color = BMP.GetPixel(0, 0)
            Dim maxValue As Long = 0
            Dim y As Integer
            While y < BMP.Height
                If (y = 0 Or y = BMP.Height - 1) Then
                    Dim x As Integer = 0
                    While (x < BMP.Width)
                        Dim c As System.Drawing.Color = BMP.GetPixel(x, y)
                        Dim argb As Integer = c.ToArgb()
                        Dim key As String = "k" & argb.ToString()
                        Dim val As Long = 1
                        If colors.ContainsKey(key) Then
                            val = colors.Item(key) + 1
                            colors.Item(key) = val
                        Else
                            colors.Add(key, 1)
                        End If
                        If (maxKey Is Nothing OrElse val > maxValue) Then
                            maxKey = key
                            maxValue = val
                            result = c
                        End If
                        x += 1
                    End While
                Else
                    Dim x As Integer = 0
                    While (x < 2)
                        If (x = 1) Then
                            x = BMP.Width - 1
                        End If
                        Dim c As System.Drawing.Color = BMP.GetPixel(x, y)
                        Dim argb As Integer = c.ToArgb()
                        Dim key As String = "k" & argb.ToString()
                        Dim val As Long = 1
                        If (colors.ContainsKey(key)) Then
                            val = colors.Item(key) + 1
                            colors.Item(key) = colors.Item(key) + 1
                        Else
                            colors.Add(key, 1)
                        End If
                        If (maxKey Is Nothing OrElse val > maxValue) Then
                            maxKey = key
                            maxValue = val
                            result = c
                        End If
                        x += 1
                    End While
                End If
                y += 1
            End While
            colors = Nothing
            Return result
        End Function
        Public Shared Function SmartCropAndScale(ByRef BMP As System.Drawing.Bitmap, ByRef DestinationTarget As String, ByVal allowedChange As Int16, ByVal width As Double, ByVal height As Double, ByVal widthunit As String, ByVal heightunit As String, ByVal quality As Long, Optional ByRef Debugger As Debugger = Nothing) As IO.Stream
            Dim croprect As System.Drawing.Rectangle = SmartSize_Fast(BMP, allowedChange) 'SmartSize(BMP, allowedChange)
            croprect.Width = (croprect.Width - croprect.X) + 1
            croprect.Height = (croprect.Height - croprect.Y) + 1
            Dim cropS As System.IO.Stream = ResizeImage(BMP, DestinationTarget, croprect.Width, croprect.Height, 100, Debugger, croprect.Width, croprect.Height, croprect.X, croprect.Y)

            Dim cropBMP As System.Drawing.Bitmap = New System.Drawing.Bitmap(cropS)
            Dim crop As System.Drawing.Size = New System.Drawing.Size(cropBMP.Width, cropBMP.Height)
            crop = Scale(crop, width, height, widthunit, heightunit)
            cropS = ResizeImage(cropBMP, DestinationTarget + ".jpg", crop.Width, crop.Height, quality)
            cropBMP.Dispose()
            Return cropS
        End Function
        Public Shared Function Crop(ByRef BMP As System.Drawing.Bitmap, ByRef DestinationTarget As String, ByVal allowedChange As Int16, ByVal width As Double, ByVal height As Double, ByVal X As Long, ByVal Y As Long, ByVal quality As Long, Optional ByRef Debugger As Debugger = Nothing) As IO.Stream
            Dim newWidth As Integer = (width - X) + 1
            Dim newHeight As Integer = (height - Y) + 1
            Dim cropS As System.IO.Stream = ResizeImage(BMP, DestinationTarget, newWidth, newHeight, 100, Debugger, newWidth, newHeight, X, Y)

            Dim cropBMP As System.Drawing.Bitmap = New System.Drawing.Bitmap(cropS)
            cropS = ResizeImage(cropBMP, DestinationTarget + ".jpg", width, height, quality)
            cropBMP.Dispose()
            Return cropS
        End Function
        Public Shared Function Scale(ByVal originalSize As System.Drawing.SizeF, ByVal width As Double, ByVal height As Double, ByVal widthUnit As String, ByVal heightUnit As String) As System.Drawing.Size
            Dim f As System.Drawing.Size = New System.Drawing.Size()

            Dim fixedWidth As Boolean = False
            Dim fixedHeight As Boolean = False
            If (widthUnit <> "%" AndAlso width > 0) Then
                fixedWidth = True
            End If
            If (heightUnit <> "%" AndAlso height > 0) Then
                fixedHeight = True
            End If
            If (widthUnit = "%" AndAlso width > 0) Then
                width = originalSize.Width * (width / 100.0)
            End If
            If (heightUnit = "%" AndAlso height > 0) Then
                height = originalSize.Height * (height / 100.0)
            End If
            If (width > 0 AndAlso height <= 0) Then
                height = originalSize.Height * (width / originalSize.Width)
            End If
            If (height > 0 AndAlso width <= 0) Then
                width = originalSize.Width * (height / originalSize.Height)
            End If
            If (fixedHeight OrElse fixedWidth) Then
                If (fixedHeight AndAlso Not fixedWidth) Then
                    If (height > originalSize.Height) Then
                        height = originalSize.Height
                        width = originalSize.Width
                    End If
                End If
                If (fixedWidth AndAlso Not fixedHeight) Then
                    If (width > originalSize.Width) Then
                        height = originalSize.Height
                        width = originalSize.Width
                    End If
                End If
            End If
            f.Width = width
            f.Height = height
            Return f
        End Function

        Public Shared Function ResizeImage(ByRef BMP As System.Drawing.Bitmap, ByRef DestinationTarget As String, ByRef aWidth As Integer, ByRef aHeight As Integer, ByVal Quality As Long, Optional ByRef Debugger As Debugger = Nothing, Optional ByVal rectWidth As Int32 = -1, Optional ByVal rectHeight As Int32 = -1, Optional ByVal rectX As Int32 = -1, Optional ByVal rectY As Int32 = -1) As IO.Stream
            Dim sStream As New IO.MemoryStream
            If DestinationTarget.ToUpper.EndsWith("JPEG") Or DestinationTarget.ToUpper.EndsWith("JPG") Then
                If Not Debugger Is Nothing Then
                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Transforming to JPEG " & aWidth.ToString() & " x " & aHeight.ToString(), False)
                End If
                Dim bitmap As System.Drawing.Bitmap
                If rectWidth <= 0 Then
                    bitmap = New System.Drawing.Bitmap(aWidth, aHeight, System.Drawing.Imaging.PixelFormat.Format32bppRgb) 'BMP.PixelFormat)
                Else
                    bitmap = New System.Drawing.Bitmap(rectWidth, rectHeight, System.Drawing.Imaging.PixelFormat.Format32bppRgb) 'BMP.PixelFormat)
                End If
                Dim g As System.Drawing.Graphics = System.Drawing.Graphics.FromImage(bitmap)
                g.SmoothingMode = Drawing.Drawing2D.SmoothingMode.HighQuality
                g.CompositingQuality = Drawing.Drawing2D.CompositingQuality.HighQuality
                g.InterpolationMode = Drawing.Drawing2D.InterpolationMode.HighQualityBicubic

                Dim Drect As System.Drawing.Rectangle
                If rectWidth <= 0 Then
                    Drect = New System.Drawing.Rectangle(0, 0, aWidth, aHeight)
                Else
                    Drect = New System.Drawing.Rectangle(0, 0, rectWidth, rectHeight)
                End If

                Dim Srect As New System.Drawing.Rectangle(0, 0, BMP.Width, BMP.Height)
                Dim wrapMode As New System.Drawing.Imaging.ImageAttributes

                'fill?
                Dim blackandwhite As Boolean = False
                If Quality < 0 Then
                    blackandwhite = True
                    Quality = Abs(Quality)
                End If
                If blackandwhite Then
                    Dim cm As System.Drawing.Imaging.ColorMatrix = New System.Drawing.Imaging.ColorMatrix(New Single()() _
                       {New Single() {0.3, 0.3, 0.3, 0, 0}, _
                      New Single() {0.59, 0.59, 0.59, 0, 0}, _
                      New Single() {0.11, 0.11, 0.11, 0, 0}, _
                      New Single() {0, 0, 0, 1, 0}, _
                      New Single() {0, 0, 0, 0, 1}})

                    wrapMode.SetColorMatrix(cm)
                    wrapMode.SetWrapMode(Drawing.Drawing2D.WrapMode.TileFlipXY)
                Else
                    wrapMode.SetWrapMode(Drawing.Drawing2D.WrapMode.TileFlipXY)
                End If

                If rectWidth <= 0 Then
                    g.DrawImage(BMP, Drect, 0, 0, BMP.Width, BMP.Height, Drawing.GraphicsUnit.Pixel, wrapMode)
                Else
                    g.DrawImage(BMP, Drect, rectX, rectY, aWidth, aHeight, Drawing.GraphicsUnit.Pixel, wrapMode)
                End If

                Dim imgCodec As Drawing.Imaging.ImageCodecInfo = GetImageCodec("image/jpeg")
                If Not imgCodec Is Nothing Then
                    'Determine required number of parameters
                    Dim paramSize As Integer = 2

                    Dim encoderParams As New Drawing.Imaging.EncoderParameters(paramSize)
                    encoderParams.Param(0) = New Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, Quality)
                    encoderParams.Param(1) = New Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.ColorDepth, 24L)

                    bitmap.Save(sStream, imgCodec, encoderParams)
                Else
                    bitmap.Save(sStream, System.Drawing.Imaging.ImageFormat.Jpeg)
                End If
            ElseIf DestinationTarget.ToUpper.EndsWith("PNG") Then
                If Not Debugger Is Nothing Then
                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Transforming to PNG " & aWidth.ToString() & " x " & aHeight.ToString(), False)
                End If

                Dim bitmap As System.Drawing.Bitmap
                Dim nwidth As Int32
                Dim nheight As Int32
                If rectWidth <= 0 Then
                    nwidth = aWidth
                    nheight = aHeight
                Else
                    nwidth = rectWidth
                    nheight = rectHeight
                End If

                Dim blackandwhite As Boolean = False
                Select Case Quality
                    Case 0
                        bitmap = New System.Drawing.Bitmap(nwidth, nheight, System.Drawing.Imaging.PixelFormat.Indexed)
                    Case 1
                        bitmap = New System.Drawing.Bitmap(nwidth, nheight, System.Drawing.Imaging.PixelFormat.Format1bppIndexed)
                    Case 4
                        bitmap = New System.Drawing.Bitmap(nwidth, nheight, System.Drawing.Imaging.PixelFormat.Format4bppIndexed)
                    Case 8
                        bitmap = New System.Drawing.Bitmap(nwidth, nheight, System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
                    Case 15
                        blackandwhite = True
                        bitmap = New System.Drawing.Bitmap(nwidth, nheight, System.Drawing.Imaging.PixelFormat.Format16bppRgb555)
                    Case 16
                        bitmap = New System.Drawing.Bitmap(nwidth, nheight, System.Drawing.Imaging.PixelFormat.Format16bppRgb555)
                    Case 24
                        bitmap = New System.Drawing.Bitmap(nwidth, nheight, System.Drawing.Imaging.PixelFormat.Format24bppRgb)
                    Case 48
                        bitmap = New System.Drawing.Bitmap(nwidth, nheight, System.Drawing.Imaging.PixelFormat.Format48bppRgb)
                    Case 64
                        bitmap = New System.Drawing.Bitmap(nwidth, nheight, System.Drawing.Imaging.PixelFormat.Format64bppArgb)
                    Case Else
                        bitmap = New System.Drawing.Bitmap(nwidth, nheight, System.Drawing.Imaging.PixelFormat.Format32bppRgb)
                End Select

                Dim g As System.Drawing.Graphics = System.Drawing.Graphics.FromImage(bitmap)
                g.SmoothingMode = Drawing.Drawing2D.SmoothingMode.HighQuality
                g.CompositingQuality = Drawing.Drawing2D.CompositingQuality.HighQuality
                g.InterpolationMode = Drawing.Drawing2D.InterpolationMode.HighQualityBicubic
                'g.DrawImage(BMP, 0, 0, aWidth, aHeight)
                Dim Drect As System.Drawing.Rectangle
                If rectWidth <= 0 Then
                    Drect = New System.Drawing.Rectangle(0, 0, aWidth, aHeight)
                Else
                    Drect = New System.Drawing.Rectangle(0, 0, rectWidth, rectHeight)
                End If
                Dim Srect As New System.Drawing.Rectangle(0, 0, BMP.Width, BMP.Height)
                Dim wrapMode As New System.Drawing.Imaging.ImageAttributes

                'fill?
                If blackandwhite Then
                    Dim cm As System.Drawing.Imaging.ColorMatrix = New System.Drawing.Imaging.ColorMatrix(New Single()() _
                       {New Single() {0.3, 0.3, 0.3, 0, 0}, _
                      New Single() {0.59, 0.59, 0.59, 0, 0}, _
                      New Single() {0.11, 0.11, 0.11, 0, 0}, _
                      New Single() {0, 0, 0, 1, 0}, _
                      New Single() {0, 0, 0, 0, 1}})

                    wrapMode.SetColorMatrix(cm)
                    wrapMode.SetWrapMode(Drawing.Drawing2D.WrapMode.TileFlipXY)
                Else
                    wrapMode.SetWrapMode(Drawing.Drawing2D.WrapMode.TileFlipXY)
                End If

                If rectWidth <= 0 Then
                    g.DrawImage(BMP, Drect, 0, 0, BMP.Width, BMP.Height, Drawing.GraphicsUnit.Pixel, wrapMode)
                Else
                    g.DrawImage(BMP, Drect, rectX, rectY, aWidth, aHeight, Drawing.GraphicsUnit.Pixel, wrapMode)
                End If
                bitmap.Save(sStream, System.Drawing.Imaging.ImageFormat.Png)
            Else
                If Not Debugger Is Nothing Then
                    r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Transforming to GIF " & aWidth.ToString() & " x " & aHeight.ToString(), False)
                End If
                Dim bitmap As System.Drawing.Bitmap
                Dim nwidth As Int32
                Dim nheight As Int32
                If rectWidth <= 0 Then
                    nwidth = aWidth
                    nheight = aHeight
                Else
                    nwidth = rectWidth
                    nheight = rectHeight
                End If

                Dim blackandwhite As Boolean = False
                Select Case Quality
                    Case 0
                        bitmap = New System.Drawing.Bitmap(nwidth, nheight, System.Drawing.Imaging.PixelFormat.Indexed)
                    Case 1
                        bitmap = New System.Drawing.Bitmap(nwidth, nheight, System.Drawing.Imaging.PixelFormat.Format1bppIndexed)
                    Case 4
                        bitmap = New System.Drawing.Bitmap(nwidth, nheight, System.Drawing.Imaging.PixelFormat.Format4bppIndexed)
                    Case 8
                        bitmap = New System.Drawing.Bitmap(nwidth, nheight, System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
                    Case 15
                        blackandwhite = True
                        bitmap = New System.Drawing.Bitmap(nwidth, nheight, System.Drawing.Imaging.PixelFormat.Format16bppRgb555)
                    Case 16
                        bitmap = New System.Drawing.Bitmap(nwidth, nheight, System.Drawing.Imaging.PixelFormat.Format16bppRgb555)
                    Case 24
                        bitmap = New System.Drawing.Bitmap(nwidth, nheight, System.Drawing.Imaging.PixelFormat.Format24bppRgb)
                    Case 48
                        bitmap = New System.Drawing.Bitmap(nwidth, nheight, System.Drawing.Imaging.PixelFormat.Format48bppRgb)
                    Case 64
                        bitmap = New System.Drawing.Bitmap(nwidth, nheight, System.Drawing.Imaging.PixelFormat.Format64bppArgb)
                    Case Else
                        bitmap = New System.Drawing.Bitmap(nwidth, nheight, System.Drawing.Imaging.PixelFormat.Format32bppRgb)
                End Select

                Dim g As System.Drawing.Graphics = System.Drawing.Graphics.FromImage(bitmap)
                g.SmoothingMode = Drawing.Drawing2D.SmoothingMode.HighQuality
                g.CompositingQuality = Drawing.Drawing2D.CompositingQuality.HighQuality
                g.InterpolationMode = Drawing.Drawing2D.InterpolationMode.HighQualityBicubic
                'g.DrawImage(BMP, 0, 0, aWidth, aHeight)
                Dim Drect As System.Drawing.Rectangle
                If rectWidth <= 0 Then
                    Drect = New System.Drawing.Rectangle(0, 0, aWidth, aHeight)
                Else
                    Drect = New System.Drawing.Rectangle(0, 0, rectWidth, rectHeight)
                End If
                Dim wrapMode As New System.Drawing.Imaging.ImageAttributes

                'fill?
                If blackandwhite Then
                    Dim cm As System.Drawing.Imaging.ColorMatrix = New System.Drawing.Imaging.ColorMatrix(New Single()() _
                       {New Single() {0.5, 0.5, 0.5, 0, 0}, _
                      New Single() {0.5, 0.5, 0.5, 0, 0}, _
                      New Single() {0.5, 0.5, 0.5, 0, 0}, _
                      New Single() {0, 0, 0, 1, 0}, _
                      New Single() {0, 0, 0, 0, 1}})

                    wrapMode.SetColorMatrix(cm)
                    wrapMode.SetWrapMode(Drawing.Drawing2D.WrapMode.TileFlipXY)
                Else
                    wrapMode.SetWrapMode(Drawing.Drawing2D.WrapMode.TileFlipXY)
                End If

                'fill?
                If rectWidth <= 0 Then
                    g.DrawImage(BMP, Drect, 0, 0, BMP.Width, BMP.Height, Drawing.GraphicsUnit.Pixel, wrapMode)
                Else
                    g.DrawImage(BMP, Drect, rectX, rectY, aWidth, aHeight, Drawing.GraphicsUnit.Pixel, wrapMode)
                End If
                bitmap.Save(sStream, System.Drawing.Imaging.ImageFormat.Gif)
                End If
                Return sStream
        End Function
        Public Shared Function MeasureString(ByVal FontName As String, ByVal Size As String, ByVal Value As String) As Drawing.SizeF
            Dim bx As New System.Drawing.Bitmap(1, 1)
            Dim g As System.Drawing.Graphics = System.Drawing.Graphics.FromImage(bx)
            Return g.MeasureString(Value, GetFont(FontName, Size))
        End Function

        Public Shared Function GetFont(ByVal FontName As String, ByVal Size As String) As System.Drawing.Font
            Dim rSize As Single = 12
            Dim rSizeType As System.Drawing.GraphicsUnit = Drawing.GraphicsUnit.Point
            If Size Is Nothing OrElse Size.Length = 0 Then
                Size = "12"
            End If
            Dim imagesizetype As String = "px"
            If Not Size Is Nothing Then
                If Size.EndsWith("pt") Then
                    imagesizetype = "pt"
                    Size = Size.Substring(0, Size.Length - 2)
                End If
                If Size.EndsWith("px") Then
                    imagesizetype = "px"
                    Size = Size.Substring(0, Size.Length - 2)
                End If
            End If
            If imagesizetype.Length > 0 AndAlso imagesizetype.ToUpper = "PX" Then
                rSizeType = Drawing.GraphicsUnit.Pixel
            End If
            If IsNumeric(Size) Then
                rSize = Size
            End If
            Dim rFName As System.Drawing.FontFamily = System.Drawing.FontFamily.GenericSansSerif
            If Not FontName Is Nothing AndAlso FontName.Length > 0 Then
                Dim fc As New Drawing.FontConverter
                Try
                    Dim fcc As Drawing.Font = fc.ConvertFromString(FontName)
                    If Not fcc Is Nothing Then
                        rFName = fcc.FontFamily
                    End If
                Catch ex As Exception

                End Try
            End If
            Dim rFont As System.Drawing.Font = New System.Drawing.Font(rFName, rSize, rSizeType)
            Return rFont
        End Function
        Public Shared Function RenderString(ByRef originalBMP As System.Drawing.Bitmap, ByVal X As Integer, ByVal Y As Integer, ByVal Font As System.Drawing.Font, ByVal foreColor As System.Drawing.Color, ByVal backColor As System.Drawing.Color, ByVal Value As String, ByRef format As String, ByVal ImageWarp As String) As IO.Stream
            Dim foreBrush As New System.Drawing.SolidBrush(foreColor)
            Dim backBrush As New System.Drawing.SolidBrush(backColor)
            ImageWarp = "," & ImageWarp & ","
            Dim sStream As New IO.MemoryStream
            Dim autoSize As Boolean = True
            Dim warpBackground As Boolean = False
            If Not originalBMP Is Nothing Then
                autoSize = False
                'ONLY WARP THE BACKGROUND IF OPTION IS SELECTED
                If ImageWarp.Contains(",5,") Then
                    warpBackground = True
                End If
            End If


            Dim bmp As New System.Drawing.Bitmap(1, 1, System.Drawing.Imaging.PixelFormat.Format64bppPArgb)
            Dim g As System.Drawing.Graphics = System.Drawing.Graphics.FromImage(bmp)
            Dim size As Drawing.SizeF = g.MeasureString(Value, Font)
            If autoSize Then
                bmp = New System.Drawing.Bitmap(size.Width, size.Height, System.Drawing.Imaging.PixelFormat.Format64bppPArgb)
            Else
                ' bmp = New System.Drawing.Bitmap(originalBMP.Width, originalBMP.Height, System.Drawing.Imaging.PixelFormat.Format64bppPArgb)
                If Not warpBackground Then
                    bmp = New System.Drawing.Bitmap(originalBMP.Width, originalBMP.Height, System.Drawing.Imaging.PixelFormat.Format64bppPArgb)
                Else
                    bmp = New System.Drawing.Bitmap(originalBMP, originalBMP.Width, originalBMP.Height)
                End If
                'g.SmoothingMode = Drawing.Drawing2D.SmoothingMode.HighQuality
                'g.CompositingQuality = Drawing.Drawing2D.CompositingQuality.HighQuality
                'g.InterpolationMode = Drawing.Drawing2D.InterpolationMode.HighQualityBicubic

                'Dim Drect As New System.Drawing.Rectangle(0, 0, originalBMP.Width, originalBMP.Height)
                'Dim Srect As New System.Drawing.Rectangle(0, 0, originalBMP.Width, originalBMP.Height)
                'Dim wrapMode As New System.Drawing.Imaging.ImageAttributes
                'wrapMode.SetWrapMode(Drawing.Drawing2D.WrapMode.TileFlipXY)
                'g.DrawImage(originalBMP, Drect, 0, 0, originalBMP.Width, originalBMP.Height, Drawing.GraphicsUnit.Pixel, wrapMode)
                'g.Save()
            End If
            g = System.Drawing.Graphics.FromImage(bmp)

            Dim rect As New System.Drawing.RectangleF(X, Y, size.Width, size.Height)
            If format.ToUpper.EndsWith("JPEG") Or format.ToUpper.EndsWith("JPG") Then
                g.SmoothingMode = Drawing.Drawing2D.SmoothingMode.HighQuality
                g.CompositingQuality = Drawing.Drawing2D.CompositingQuality.HighQuality
                g.InterpolationMode = Drawing.Drawing2D.InterpolationMode.HighQualityBicubic
            ElseIf format.ToUpper.EndsWith("PNG") Then
                g.SmoothingMode = Drawing.Drawing2D.SmoothingMode.HighQuality
                g.CompositingQuality = Drawing.Drawing2D.CompositingQuality.HighQuality
                g.InterpolationMode = Drawing.Drawing2D.InterpolationMode.HighQualityBicubic
            Else
                g.SmoothingMode = Drawing.Drawing2D.SmoothingMode.HighQuality
                g.CompositingQuality = Drawing.Drawing2D.CompositingQuality.HighQuality
                g.InterpolationMode = Drawing.Drawing2D.InterpolationMode.HighQualityBicubic
            End If

            If Not warpBackground OrElse originalBMP Is Nothing Then
                g.FillRectangle(backBrush, bmp.GetBounds(System.Drawing.GraphicsUnit.Pixel))
            End If

            Dim path As New Drawing.Drawing2D.GraphicsPath
            Dim sformat As New Drawing.StringFormat
            sformat.Alignment = Drawing.StringAlignment.Near
            sformat.LineAlignment = Drawing.StringAlignment.Near

            path.AddString(Value, Font.FontFamily, CType(Font.Style, Integer), Font.Size, rect, sformat)
            Dim r As New Random()
            If ImageWarp.Contains(",1,") Then
                'Skew
                Dim v As Double = 4.0
                Dim wPoints As Drawing.PointF() = {New Drawing.PointF(r.Next(size.Width) / v, r.Next(size.Height) / v), New Drawing.PointF(size.Width - r.Next(size.Width) / v, r.Next(size.Height) / v), New Drawing.PointF(r.Next(size.Width) / v, size.Height - r.Next(size.Height) / v), New Drawing.PointF(size.Width - r.Next(size.Width) / v, size.Height - r.Next(size.Height) / v)}
                Dim m As New Drawing.Drawing2D.Matrix
                'm.Translate(0, 0)
                m.Translate(X, Y)
                path.Warp(wPoints, rect, m, Drawing.Drawing2D.WarpMode.Perspective, 0.0F)
            End If
            g.FillPath(foreBrush, path)
            g.Save()

            If ImageWarp.Contains(",2,") Then
                DistortImage_Water(bmp, r.Next(2, 20))
            End If
            If ImageWarp.Contains(",3,") Then
                DistortImage_Jitter(bmp, 4, 4)
            End If
            If ImageWarp.Contains(",4,") Then
                DistortImage_Thrash(bmp, 4, 8)
            End If

            If Not warpBackground AndAlso Not originalBMP Is Nothing Then
                MergeTextLayer(bmp, originalBMP, backColor)
            End If

            If format.ToUpper.EndsWith("JPEG") Or format.ToUpper.EndsWith("JPG") Then
                g.SmoothingMode = Drawing.Drawing2D.SmoothingMode.HighQuality
                g.CompositingQuality = Drawing.Drawing2D.CompositingQuality.HighQuality
                g.InterpolationMode = Drawing.Drawing2D.InterpolationMode.HighQualityBicubic

                Dim imgCodec As Drawing.Imaging.ImageCodecInfo = GetImageCodec("image/jpeg")
                If Not imgCodec Is Nothing Then
                    'Determine required number of parameters
                    Dim paramSize As Integer = 2

                    Dim encoderParams As New Drawing.Imaging.EncoderParameters(paramSize)
                    encoderParams.Param(0) = New Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100)
                    encoderParams.Param(1) = New Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.ColorDepth, 24L)

                    bmp.Save(sStream, imgCodec, encoderParams)
                Else
                    bmp.Save(sStream, System.Drawing.Imaging.ImageFormat.Jpeg)
                End If
            ElseIf format.ToUpper.EndsWith("PNG") Then
                g.SmoothingMode = Drawing.Drawing2D.SmoothingMode.HighQuality
                g.CompositingQuality = Drawing.Drawing2D.CompositingQuality.HighQuality
                g.InterpolationMode = Drawing.Drawing2D.InterpolationMode.HighQualityBicubic
                bmp.Save(sStream, System.Drawing.Imaging.ImageFormat.Png)
            Else
                g.SmoothingMode = Drawing.Drawing2D.SmoothingMode.HighQuality
                g.CompositingQuality = Drawing.Drawing2D.CompositingQuality.HighQuality
                g.InterpolationMode = Drawing.Drawing2D.InterpolationMode.HighQualityBicubic
                bmp.Save(sStream, System.Drawing.Imaging.ImageFormat.Gif)
            End If

            g = Nothing
            bmp = Nothing

            size = Nothing
            Return sStream
        End Function
        Public Shared Sub MergeTextLayer(ByRef textB As Drawing.Bitmap, ByRef imgB As Drawing.Bitmap, ByVal bgColor As Drawing.Color)
            Dim width As Integer = imgB.Width
            Dim height As Integer = imgB.Height
            Dim r As New Random
            Dim y As Integer = 0
            For y = 0 To height - 1
                Dim x As Integer = 0
                For x = 0 To width - 1
                    Dim pColor As Drawing.Color = textB.GetPixel(x, y)
                    If bgColor.ToArgb = pColor.ToArgb Then
                        textB.SetPixel(x, y, imgB.GetPixel(x, y))
                    End If
                Next
            Next
        End Sub
        Public Shared Sub DistortImage_Water(ByRef b As Drawing.Bitmap, ByVal distortion As Double)
            Dim width As Integer = b.Width
            Dim height As Integer = b.Height
            Dim r As New Random
            Dim copy As Drawing.Bitmap = b.Clone()
            Dim y As Integer = 0
            For y = 0 To height - 1
                Dim x As Integer = 0
                For x = 0 To width - 1
                    Dim newX As Integer = Convert.ToInt32(x + (distortion * Math.Sin(Math.PI * y / 64.0)))
                    Dim newY As Integer = Convert.ToInt32(y + (distortion * Math.Cos(Math.PI * x / 64.0)))

                    'If r.Next(chance) = 2 Then
                    '    If r.Next(2) = 2 Then
                    '        newX = newX - r.Next(depth)
                    '    Else
                    '        newX = newX + r.Next(depth)
                    '    End If
                    '    If r.Next(2) = 2 Then
                    '        newY = newY - r.Next(depth)
                    '    Else
                    '        newY = newY + r.Next(depth)
                    '    End If
                    'End If

                    If newX < 0 OrElse newX >= width Then
                        newX = 0
                    End If
                    If newY < 0 OrElse newY >= height Then
                        newY = 0
                    End If
                    b.SetPixel(x, y, copy.GetPixel(newX, newY))
                Next
            Next
        End Sub

        Public Shared Sub DistortImage_Jitter(ByRef b As Drawing.Bitmap, ByVal chance As Integer, ByVal depth As Integer)
            Dim width As Integer = b.Width
            Dim height As Integer = b.Height
            Dim r As New Random
            Dim copy As Drawing.Bitmap = b.Clone()
            Dim y As Integer = 0
            For y = 0 To height - 1
                Dim x As Integer = 0
                For x = 0 To width - 1
                    Dim newX As Integer = x
                    Dim newY As Integer = y
                    If r.Next(chance) = 2 Then
                        If r.Next(2) = 2 Then
                            newX = newX - r.Next(depth)
                        Else
                            newX = newX + r.Next(depth)
                        End If
                        If r.Next(2) = 2 Then
                            newY = newY - r.Next(depth)
                        Else
                            newY = newY + r.Next(depth)
                        End If
                    End If
                    If newX < 0 OrElse newX >= width Then
                        newX = 0
                    End If
                    If newY < 0 OrElse newY >= height Then
                        newY = 0
                    End If
                    b.SetPixel(x, y, copy.GetPixel(newX, newY))
                Next
            Next
        End Sub

        Public Shared Sub DistortImage_Thrash(ByRef b As Drawing.Bitmap, ByVal chance As Integer, ByVal depth As Integer)
            Dim width As Integer = b.Width
            Dim height As Integer = b.Height
            Dim r As New Random
            Dim copy As Drawing.Bitmap = b.Clone()
            Dim y As Integer = 0
            If height <= 2 Or width <= 2 Then
                Return
            End If
            For y = 1 To height - 2
                Dim x As Integer = 0
                For x = 1 To width - 2
                    Dim thrashLength As Integer = 0
                    Dim thrashXDirection As Integer = 1
                    Dim thrashYDirection As Integer = 1
                    If r.Next(chance) = 2 Then
                        thrashLength = r.Next(depth)
                        If r.Next(2) = 2 Then
                            thrashXDirection = -1
                        End If
                        If r.Next(2) = 2 Then
                            thrashYDirection = -1
                        End If
                    End If
                    Dim clr As System.Drawing.Color = copy.GetPixel(x, y)
                    Dim j As Integer = 0
                    If thrashLength > 0 Then
                        Dim jX As Integer = x
                        Dim jY As Integer = y
                        For j = 0 To thrashLength - 1
                            jX = jX + thrashXDirection
                            jY = jY + thrashYDirection
                            If Not (jX < 0 OrElse jX >= width OrElse jY < 0 OrElse jY >= height) Then
                                b.SetPixel(jX, jY, clr)
                            End If
                        Next
                    End If
                    'b.SetPixel(x, y, clr)
                Next
            Next
        End Sub
    End Class
End Namespace
