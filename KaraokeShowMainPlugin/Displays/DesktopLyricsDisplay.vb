Imports MusicBeePlugin
Imports System.Windows.Forms
Imports System.Drawing
Imports System.Drawing.Drawing2D
''' <summary>
''' A lyrics panel displayed on desktop with words only
''' </summary>
Public Class DesktopLyricsDisplay
    Implements IDisplay
    Implements IKSPlugin

    'Settings Variables
    Private colorB1 As Color = Color.Green
    Private colorB2 As Color = Color.LightGreen
    Private colorA1 As Color = Color.Red
    Private colorA2 As Color = Color.DarkRed
    Private outerColorA As Color = Color.PaleVioletRed
    Private outerColorB As Color = Color.GreenYellow
    Private colorRect As Color = Color.FromArgb(100, 0, 0, 0)
    Private fontName As String = "微软雅黑"
    Private fontSize As Single = 60
    Private fStyle As FontStyle = FontStyle.Bold
    Private windowWidth As Single = 1000

    'Fields Used by Paint Pipelines
    Private IsMouseHoverForm As Boolean = False
    Private NowText As String = ""
    Private NextText As String = ""
    Private lyrics As List(Of String) = Nothing
    Private Index As Integer = 0
    Private Percentage As Double = 0
    Private IsLastLyricsUpper As Boolean = False
    Private bmpNowWidth As Single
    Private bmpNowHeight As Single

    Private PaintingCaches As New Dictionary(Of String, BitmapCache)
    Private ValuesCaches As New Dictionary(Of String, Object)
    Private _LyricsForm As DesktopLyricsForm
    ''' <summary>
    ''' Represent the display form entity
    ''' </summary>
    ''' <returns>an instence of a desktop lyrics form</returns>
    Private ReadOnly Property LyricsForm() As DesktopLyricsForm
        Get
            If _LyricsForm Is Nothing OrElse _LyricsForm.IsDisposed = True Then _LyricsForm = New DesktopLyricsForm()
            _LyricsForm.DeliverMessageHandler = AddressOf Me.FormMessageDeliveryHandler
            Return _LyricsForm
        End Get
    End Property


#Region "Paint Pipelines"

    Private Function PP_DrawBeforeText(g As Graphics) As Graphics
        Dim fontSize As SizeF
        'Extract fontsize from ValuesCaches
        fontSize = If(ValuesCaches.Keys.Contains(NowText), ValuesCaches(NowText), Nothing)
        If fontSize = Nothing Then
            Dim font As New Font(fontName, Me.fontSize, fStyle)
            fontSize = GetCorrectFontSize(NowText, font)
        End If
        'Paint basic background
        'extract from cache if possible
        Dim lyricsBMPCache = If(PaintingCaches.Keys.Contains("DrawBeforeText"), PaintingCaches("DrawBeforeText"), Nothing)
        If (lyricsBMPCache Is Nothing) OrElse lyricsBMPCache.Tag <> NowText Then
            'Add brush
            Dim bshBefore As New LinearGradientBrush(New PointF(0, 0), New PointF(0, 100), colorB1, colorB2)
            'Add Outer pen
            Dim outerPen As New Pen(outerColorB, 2)
            'Get FullsizeTextBitmap
            Dim lrcBMP = DrawOriginalFullsizeTextBitmap(NowText, bshBefore, outerPen, fontSize)
            'save caches
            lyricsBMPCache = New BitmapCache()
            lyricsBMPCache.Image = lrcBMP
            lyricsBMPCache.Tag = NowText
            PaintingCaches.Item("DrawBeforeText") = lyricsBMPCache
        End If
        'Point on upper or down
        Try
            If IsLastLyricsUpper Then
                g.DrawImage(lyricsBMPCache.Image, New PointF(0, 0))
            Else
                g.DrawImage(lyricsBMPCache.Image, New PointF(0, bmpNowHeight / 2))
            End If
        Catch ex As Exception

        End Try
        Return g
    End Function

    Private Function PP_DrawAfterText(g As Graphics) As Graphics
        Dim fontSize As SizeF
        'Extract fontsize from ValuesCaches
        fontSize = If(ValuesCaches.Keys.Contains(NowText), ValuesCaches(NowText), Nothing)
        If fontSize = Nothing Then
            Dim font As New Font(fontName, Me.fontSize, fStyle)
            fontSize = GetCorrectFontSize(NowText, font)
        End If
        'Calc Percentage
        Dim percentageA = If(Percentage <= 1, Percentage, 1)
        Dim textWidth As Integer = Convert.ToInt32(fontSize.Width * percentageA)
        'Get Fullsize After Bitmap
        'paint fullsize foreground bmp or extract from cache
        Dim bmpAfterAll As Bitmap = Nothing
        Dim lyricsBMPCache = If(PaintingCaches.Keys.Contains("DrawAfterText"), PaintingCaches("DrawAfterText"), Nothing)
        If lyricsBMPCache IsNot Nothing AndAlso lyricsBMPCache.Tag = NowText Then
            bmpAfterAll = lyricsBMPCache.Image
        Else
            'Add brush
            Dim bshAfter As New LinearGradientBrush(New PointF(0, 0), New PointF(0, 100), colorA1, colorA2)
            'Add pen
            Dim outerPen As New Pen(outerColorA, 2)
            'Get FullsizeTextBitmap
            bmpAfterAll = DrawOriginalFullsizeTextBitmap(NowText, bshAfter, outerPen, fontSize)
            lyricsBMPCache = New BitmapCache()
            lyricsBMPCache.Image = bmpAfterAll
            lyricsBMPCache.Tag = NowText
            PaintingCaches.Item("DrawAfterText") = lyricsBMPCache
        End If
        'curtail and paint on pipeline g
        If textWidth > 0 And Percentage <= 1 Then
            Try
                Dim BMPAfter = bmpAfterAll.Clone(New Rectangle(0, 0, textWidth, Convert.ToInt32(fontSize.Height)), Imaging.PixelFormat.DontCare)
                If IsLastLyricsUpper Then
                    g.DrawImage(BMPAfter, New PointF(0, 0))
                Else
                    g.DrawImage(BMPAfter, New PointF(0, bmpNowHeight / 2))
                End If
            Catch ex As Exception

            End Try
        End If
        Return g
    End Function

    Private Function PP_DrawMouseHover(g As Graphics) As Graphics
        'If cusor on the form, paint background
        If (Me.IsMouseHoverForm) Then
            Dim backBrush As New SolidBrush(colorRect)
            g.FillRectangle(backBrush, 0, 0, g.ClipBounds.Width, g.ClipBounds.Height)
        End If
        Return g
    End Function

    Private Function PP_DrawNextLyrics(g As Graphics) As Graphics
        Dim fontSize As SizeF
        'Extract fontsize from ValuesCaches
        fontSize = If(ValuesCaches.Keys.Contains(NextText), ValuesCaches(NextText), Nothing)
        If fontSize = Nothing Then
            Dim font As New Font(fontName, Me.fontSize, fStyle)
            fontSize = GetCorrectFontSize(NextText, font)
        End If
        'Paint basic background
        'extract from cache if possible
        Dim lyricsBMPCache = If(PaintingCaches.Keys.Contains("DrawNextLyrics"), PaintingCaches("DrawNextLyrics"), Nothing)
        If (lyricsBMPCache Is Nothing) OrElse lyricsBMPCache.Tag <> NextText Then
            'Add brush
            Dim bshBefore As New LinearGradientBrush(New PointF(0, 0), New PointF(0, 100), colorB1, colorB2)
            'Add Outer pen
            Dim outerPen As New Pen(outerColorB, 2)
            'Get FullsizeTextBitmap
            Dim lrcBMP = DrawOriginalFullsizeTextBitmap(NextText, bshBefore, outerPen, fontSize)
            'save caches
            lyricsBMPCache = New BitmapCache()
            lyricsBMPCache.Image = lrcBMP
            lyricsBMPCache.Tag = NextText
            PaintingCaches.Item("DrawNextLyrics") = lyricsBMPCache
        End If
        'Point on upper or down
        Try
            If Not IsLastLyricsUpper Then
                g.DrawImage(lyricsBMPCache.Image, New PointF(0, 0))
            Else
                g.DrawImage(lyricsBMPCache.Image, New PointF(0, bmpNowHeight / 2))
            End If
        Catch ex As Exception

        End Try
        Return g
    End Function

#End Region

#Region "Painting Methods"

    Private Function DrawOriginalFullsizeTextBitmap(Text As String, Brush As Brush, OuterPen As Pen, Size As SizeF) As Bitmap
        'Add Font
        Dim font As New Font(fontName, Me.fontSize, fStyle)
        'Add text path
        Dim gPath As New GraphicsPath()
        gPath.AddString(Text, font.FontFamily, font.Style, font.Size, New Drawing.Point(10, 10), StringFormat.GenericDefault)
        'Create bitmap and graphic
        Dim lrcBMP As New Bitmap(Convert.ToInt32(Size.Width), Convert.ToInt32(Size.Height))
        Dim specialGraphics As Graphics = Graphics.FromImage(lrcBMP)
        'paint text
        specialGraphics.CompositingQuality = CompositingQuality.HighQuality
        specialGraphics.SmoothingMode = SmoothingMode.HighQuality
        specialGraphics.TextRenderingHint = Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit
        specialGraphics.PixelOffsetMode = PixelOffsetMode.HighQuality
        specialGraphics.FillPath(Brush, gPath)
        specialGraphics.DrawPath(OuterPen, gPath)
        Return lrcBMP
    End Function

    Private Function ConvertPxToPt(px As Single) As Single
        Dim preGraphics As Graphics = Graphics.FromImage(New Bitmap(10, 10))
        Return px * preGraphics.DpiY / 72
    End Function
    Private Function GetCorrectFontSize(Text As String, Font As Font) As SizeF
        Dim preGraphics As Graphics = Graphics.FromImage(New Bitmap(10, 10))
        Dim fontSize As SizeF = preGraphics.MeasureString(Text, Font)
        Dim scalePercentage As Double = (preGraphics.DpiX / 0.96) / 100
        Dim y = scalePercentage / 3 * 4
        fontSize = New SizeF(fontSize.Width / y + 10, fontSize.Height / y + 10)
        preGraphics.Dispose()
        Return fontSize
    End Function

#End Region


    Private Sub LoadSettings()
        'Get colors
        Dim cb1, cb2, ca1, ca2, bca, bcb As String
        cb1 = colorB1.ToArgb().ToString()
        cb2 = colorB2.ToArgb().ToString()
        ca1 = colorA1.ToArgb().ToString()
        ca2 = colorA2.ToArgb().ToString()
        bca = outerColorA.ToArgb().ToString()
        bcb = outerColorB.ToArgb().ToString()
        GetSetting.Invoke(Me, "ColorBefore1", cb1)
        GetSetting.Invoke(Me, "ColorBefore2", cb2)
        GetSetting.Invoke(Me, "ColorAfter1", ca1)
        GetSetting.Invoke(Me, "ColorAfter2", ca2)
        GetSetting.Invoke(Me, "BorderColorAfter", bca)
        GetSetting.Invoke(Me, "BorderColorBefore", bcb)
        colorB1 = Color.FromArgb(Integer.Parse(cb1))
        colorB2 = Color.FromArgb(Integer.Parse(cb2))
        colorA1 = Color.FromArgb(Integer.Parse(ca1))
        colorA2 = Color.FromArgb(Integer.Parse(ca2))
        outerColorA = Color.FromArgb(Integer.Parse(bca))
        outerColorB = Color.FromArgb(Integer.Parse(bcb))
        'Get Font
        GetSetting.Invoke(Me, "FontName", fontName)
        Dim fSizeS As String = fontSize.ToString()
        GetSetting.Invoke(Me, "FontSize", fSizeS)
        fontSize = Single.Parse(fSizeS)
        Dim fStyleS As String = fStyle.ToString()
        GetSetting.Invoke(Me, "FontStyle", fStyleS)
        fStyle = [Enum].Parse(GetType(FontStyle), fStyleS)
    End Sub

    ''' <summary>
    ''' Handling message from lyrics form
    ''' </summary>
    Private Sub FormMessageDeliveryHandler(param As String)
        Select Case param.ToLower()
            Case "mouse_hover"
                Me.IsMouseHoverForm = True
            Case "mouse_leave"
                Me.IsMouseHoverForm = False
        End Select
        Me.RefreshWindow()
    End Sub

    ''' <summary>
    ''' Notify window to refresh
    ''' </summary>
    Private Sub RefreshWindow()
        'Exit if LyricsForm is disabled
        If Me.Visible = False Then Exit Sub
        'Get Font
        Dim font As New Font(fontName, fontSize, fStyle)
        'Get text
        If Me.lyrics IsNot Nothing Then
            If Me.Index < lyrics.Count Then Me.NowText = Me.lyrics(Me.Index)
            Me.NextText = If(Me.lyrics.Skip(Me.Index + 1).FirstOrDefault(Function(s) s?.Trim() <> ""), "")
        Else
            Exit Sub
        End If
        If Me.NowText Is Nothing Then Exit Sub
        'Get bmp
        'Dim bmp As Bitmap = GetLyricsBMP(Me.NowText, Percentage, font, colorB1, colorB2, colorA1, colorA2)
        Dim fs = GetCorrectFontSize(NowText, font)
        Dim nextfs = If((Me.Index + 1) < lyrics.Count, GetCorrectFontSize(NextText, font), New SizeF(0, 0))
        'bmpNowWidth = If(Me.Index Mod 2 = 0, If(fs.Width / 2 + nextfs.Width > fs.Width, fs.Width / 2 + nextfs.Width, fs.Width), If(nextfs.Width / 2 + fs.Width > nextfs.Width, nextfs.Width / 2 + fs.Width, nextfs.Width))
        bmpNowWidth = If(fs.Width > nextfs.Width, fs.Width, nextfs.Width)
        bmpNowHeight = fs.Height * 2

        'bmpLastHalfX = bmpHalfX
        'bmpHalfX = If(Me.Index Mod 2 = 0, fs.Width / 2, nextfs.Width / 2)
        Dim bmp As New Bitmap(Convert.ToInt32(bmpNowWidth), Convert.ToInt32(bmpNowHeight))
        Dim pipelineGraphics As Graphics = Graphics.FromImage(bmp)
        pipelineGraphics.CompositingQuality = CompositingQuality.HighQuality
        pipelineGraphics.SmoothingMode = SmoothingMode.HighQuality
        pipelineGraphics.TextRenderingHint = Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit
        pipelineGraphics.PixelOffsetMode = PixelOffsetMode.HighQuality
        'Set Pipeline
        pipelineGraphics = PP_DrawAfterText(PP_DrawBeforeText(PP_DrawNextLyrics(PP_DrawMouseHover(pipelineGraphics))))
        'Enable to window
        Me.LyricsForm.BeginInvoke(Sub()
                                      Me.LyricsForm.UpdateLayeredWindow(bmp)
                                      Me.LyricsForm.Refresh()
                                  End Sub)

    End Sub

#Region "Interface Implements"
    Public ReadOnly Property Visible As Boolean Implements IDisplay.Visible
        Get
            Return Me.LyricsForm.Visible
        End Get
    End Property

    Public Property Description As String = "A lyrics panel displayed on desktop with words only" Implements IKSPlugin.Description

    Public Property GetSetting As KSPlugin_Setting_GetDelegate Implements IKSPlugin.GetSetting

    Public Property SetSetting As KSPlugin_Setting_SetDelegate Implements IKSPlugin.SetSetting

    Public Sub CloseDisplay() Implements IDisplay.CloseDisplay
        Me.LyricsForm.Close()
    End Sub

    Public Sub OnLyricsFileChanged(LyricsText As List(Of String)) Implements IDisplay.OnLyricsFileChanged
        Me.lyrics = LyricsText
        Me.RefreshWindow()
    End Sub

    Public Sub OnLyricsSentenceChanged(SentenceIndex As Integer) Implements IDisplay.OnLyricsSentenceChanged
        Me.Index = SentenceIndex
        'Change Upper Down
        If Me.lyrics IsNot Nothing AndAlso Me.lyrics(SentenceIndex) IsNot Nothing AndAlso Me.lyrics(SentenceIndex).Trim() <> "" Then
            IsLastLyricsUpper = Not IsLastLyricsUpper
        End If
        Me.RefreshWindow()
    End Sub

    Public Sub OnLyricsWordProgressChanged(WordProgressPercentage As Double) Implements IDisplay.OnLyricsWordProgressChanged
        Me.Percentage = WordProgressPercentage
        Me.RefreshWindow()
    End Sub

    Public Sub ShowDisplay() Implements IDisplay.ShowDisplay
        Me.LyricsForm.Show()
    End Sub

    Public Sub OnLoaded() Implements IKSPlugin.OnLoaded
        Me.LoadSettings()
    End Sub

    Public Sub DisplaySetting() Implements IKSPlugin.DisplaySetting
        Dim sf As New DesktopLyricsSetting()
        sf.FontSet = New Font(fontName, fontSize, fStyle)
        sf.ColorA1 = colorA1
        sf.ColorA2 = colorA2
        sf.ColorB1 = colorB1
        sf.ColorB2 = colorB2
        sf.BorderColorA = outerColorA
        sf.BorderColorB = outerColorB
        If sf.ShowDialog() = DialogResult.OK Then
            SetSetting.Invoke(Me, "FontName", sf.FontSet.Name)
            SetSetting.Invoke(Me, "FontStyle", sf.FontSet.Style.ToString())
            SetSetting.Invoke(Me, "FontSize", sf.FontSet.Size.ToString())
            SetSetting.Invoke(Me, "ColorBefore1", sf.ColorB1.ToArgb().ToString())
            SetSetting.Invoke(Me, "ColorBefore2", sf.ColorB2.ToArgb().ToString())
            SetSetting.Invoke(Me, "ColorAfter1", sf.ColorA1.ToArgb().ToString())
            SetSetting.Invoke(Me, "ColorAfter2", sf.ColorA2.ToArgb().ToString())
            SetSetting.Invoke(Me, "BorderColorAfter", sf.BorderColorA.ToArgb().ToString())
            SetSetting.Invoke(Me, "BorderColorBefore", sf.BorderColorB.ToArgb().ToString())
        End If
    End Sub

    Public Sub OnSettingReset() Implements IKSPlugin.OnSettingReset
        LoadSettings()
        'Reset Cache
        Me.ValuesCaches.Clear()
        Me.PaintingCaches.Clear()
    End Sub

#End Region

End Class

''' <summary>
''' A bitmap cache
''' </summary>
Friend Class BitmapCache
    Public Property Image As Bitmap
    Public Property Tag As String
End Class