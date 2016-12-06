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

    Private lyrics As List(Of String) = Nothing
    Private Index As Integer = 0
    Private Percentage As Double = 0

    Private colorB1 As Color = Color.Green
    Private colorB2 As Color = Color.LightGreen
    Private colorA1 As Color = Color.Red
    Private colorA2 As Color = Color.DarkRed
    Private colorRect As Color = Color.FromArgb(100, 0, 0, 0)
    Private fontName As String = "微软雅黑"
    Private fontSize As Single = 50
    Private fStyle As FontStyle = FontStyle.Bold

    Private IsMouseHoverForm As Boolean = False

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

    ''' <summary>
    ''' Get Lyrcis Bitmap
    ''' </summary>
    Private Function GetLyricsBMP(Text As String, Percentage As Double, Font As Font, ColorBefore1 As Color, ColorBefore2 As Color, ColorAfter1 As Color, ColorAfter2 As Color) As Bitmap
        'Get font size to create bitmap object
        Dim preGraphics As Graphics = Graphics.FromImage(New Bitmap(10, 10))
        Dim fontSize As SizeF = preGraphics.MeasureString(Text, Font)
        Dim scalePercentage As Double = (preGraphics.DpiX / 0.96) / 100
        Dim y = scalePercentage / 3 * 4
        fontSize = New SizeF(fontSize.Width / y + 10, fontSize.Height / y + 10)
        preGraphics.Dispose()
        'Create bitmap and graphic
        Dim lrcBMP As New Bitmap(Convert.ToInt32(fontSize.Width), Convert.ToInt32(fontSize.Height))
        Dim paintGraphics As Graphics = Graphics.FromImage(lrcBMP)
        'Set painting mode
        paintGraphics.CompositingQuality = CompositingQuality.HighQuality
        paintGraphics.SmoothingMode = SmoothingMode.HighQuality
        paintGraphics.TextRenderingHint = Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit
        paintGraphics.PixelOffsetMode = PixelOffsetMode.HighQuality
        'create gradient brushes
        Dim bshBefore As New LinearGradientBrush(New PointF(0, 0), New PointF(0, 100), ColorBefore1, ColorBefore2)
        Dim bshAfter As New LinearGradientBrush(New PointF(0, 0), New PointF(0, 100), ColorAfter1, ColorAfter2)
        'Add text path
        Dim gPath As New GraphicsPath()
        gPath.AddString(Text, Font.FontFamily, Font.Style, Font.Size, New Drawing.Point(10, 10), StringFormat.GenericDefault)
        'scale text size
        Dim textWidth As Integer = Convert.ToInt32(fontSize.Width * Percentage)
        'create bitmap after time
        Dim BMPAfter As Bitmap = Nothing
        If textWidth > 0 Then
            BMPAfter = New Bitmap(textWidth, Convert.ToInt32(fontSize.Height))
            Dim graphicAfter As Graphics = Graphics.FromImage(BMPAfter)
            graphicAfter.CompositingQuality = CompositingQuality.HighQuality
            graphicAfter.SmoothingMode = SmoothingMode.HighQuality
            graphicAfter.TextRenderingHint = Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit
            graphicAfter.PixelOffsetMode = PixelOffsetMode.HighQuality
            graphicAfter.FillPath(bshAfter, gPath)
        End If
        'If cusor on the form, paint background
        If (Me.IsMouseHoverForm) Then
            Dim backBrush As New SolidBrush(colorRect)
            paintGraphics.FillRectangle(backBrush, 0, 0, lrcBMP.Width, lrcBMP.Height)
        End If
        'Paint basic background
        paintGraphics.FillPath(bshBefore, gPath)
        If textWidth > 0 Then
            paintGraphics.DrawImage(BMPAfter, New Drawing.Point(0, 0))
            BMPAfter.Dispose()
        End If
        Return lrcBMP
    End Function

    Private Sub LoadSettings()
        'Get colors
        Dim cb1, cb2, ca1, ca2 As String
        cb1 = colorB1.ToArgb().ToString()
        cb2 = colorB2.ToArgb().ToString()
        ca1 = colorA1.ToArgb().ToString()
        ca2 = colorA2.ToArgb().ToString()
        GetSetting.Invoke(Me, "ColorBefore1", cb1)
        GetSetting.Invoke(Me, "ColorBefore2", cb2)
        GetSetting.Invoke(Me, "ColorAfter1", ca1)
        GetSetting.Invoke(Me, "ColorAfter2", ca2)
        colorB1 = Color.FromArgb(Integer.Parse(cb1))
        colorB2 = Color.FromArgb(Integer.Parse(cb2))
        colorA1 = Color.FromArgb(Integer.Parse(ca1))
        colorA2 = Color.FromArgb(Integer.Parse(ca2))
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
        Dim text As String = ""
        If Me.lyrics IsNot Nothing AndAlso Me.Index < lyrics.Count Then text = Me.lyrics(Me.Index) Else Exit Sub
        If text Is Nothing Then Exit Sub
        'Get bmp
        Dim bmp As Bitmap = GetLyricsBMP(text, Percentage, font, colorB1, colorB2, colorA1, colorA2)
        'Enable to window
        Me.LyricsForm.BeginInvoke(Sub()
                                      Me.LyricsForm.UpdateLayeredWindow(bmp)
                                      Me.LyricsForm.Refresh()
                                  End Sub)

    End Sub

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
End Class
