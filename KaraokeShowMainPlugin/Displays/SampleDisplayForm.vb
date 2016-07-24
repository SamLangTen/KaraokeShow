Imports System.Windows.Forms
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Threading.Tasks
Imports System.Threading
Imports MusicBeePlugin.Plugin
Imports System.IO

Public Class SampleDisplayForm
    Private _paintWhat As String = ""
    Private _lyric As String
    Private _percentage As Double


    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call.
        Me.SetStyle(ControlStyles.AllPaintingInWmPaint, True)
        'Me.SetStyle(ControlStyles.DoubleBuffer, True)
        Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
        Me.SetStyle(ControlStyles.ResizeRedraw, True)
        Me.SetStyle(ControlStyles.Selectable, True)
        Me.SetStyle(ControlStyles.SupportsTransparentBackColor, True)
        Me.SetStyle(ControlStyles.UserPaint, True)

        Me.UpdateStyles()

    End Sub

    Public WriteOnly Property Lyric As String
        Set(value As String)
            _lyric = value
            Me.BeginInvoke(Sub()
                               Me.Refresh()
                           End Sub)
        End Set
    End Property
    Public WriteOnly Property Percentage As Double
        Set(value As Double)
            _percentage = value
            Me.BeginInvoke(Sub()

                               UpdateWindowsStyle()
                               Me.Refresh()
                           End Sub)
        End Set
    End Property
    Sub UpdateWindowsStyle()
        Dim bmp = New Bitmap(Me.ClientRectangle.Width, Me.ClientRectangle.Height)
        Dim g As Graphics = Graphics.FromImage(bmp)
        PaintToBMP(g)
        Dim screenDC As IntPtr = Win32.GetDC(IntPtr.Zero)
        Dim hBitmap As IntPtr = IntPtr.Zero
        Dim memDC As IntPtr = Win32.CreateCompatibleDC(screenDC)
        Dim oldBits As IntPtr = IntPtr.Zero
        Try
            Dim topLoc As New Win32.Point(Me.Left, Me.Top)
            Dim bitmapSize As New Win32.Size(bmp.Width, bmp.Height)
            Dim srcLoc As New Win32.Point(0, 0)
            hBitmap = bmp.GetHbitmap(Color.FromArgb(0))
            oldBits = Win32.SelectObject(memDC, hBitmap)
            Dim bfunc As New Win32.BLENDFUNCTION() With {
             .BlendOp = Win32.AC_SRC_OVER,
             .AlphaFormat = Win32.AC_SRC_ALPHA,
             .SourceConstantAlpha = 255,
             .BlendFlags = 0
            }
            Win32.UpdateLayeredWindow(Me.Handle, screenDC, topLoc, bitmapSize, memDC, srcLoc, Color.FromArgb(255, 255, 255).ToArgb(), bfunc, Win32.ULW_ALPHA)

        Catch ex As Exception

        Finally
            If (hBitmap <> IntPtr.Zero) Then
                Win32.SelectObject(memDC, oldBits)
                Win32.DeleteObject(hBitmap)
            End If
            Win32.ReleaseDC(IntPtr.Zero, screenDC)
            Win32.DeleteDC(memDC)
        End Try
        bmp.Dispose()
        GC.Collect()
    End Sub
    Sub PaintToBMP(g As Graphics)

        g.CompositingQuality = CompositingQuality.HighQuality
        g.SmoothingMode = SmoothingMode.HighQuality
        g.TextRenderingHint = Drawing.Text.TextRenderingHint.SingleBitPerPixel
        g.PixelOffsetMode = PixelOffsetMode.HighQuality
        Dim brushRed As New LinearGradientBrush(New Drawing.Point(0, 0), New Drawing.Point(0, 100), Color.Red, Color.DarkRed)
        Dim brushYellow As New LinearGradientBrush(New Drawing.Point(0, 0), New Drawing.Point(0, 100), Color.Green, Color.LightGreen)

        If _lyric Is Nothing Then Exit Sub
        Dim gp As New GraphicsPath()
        gp.AddString(_lyric, New FontFamily("微软雅黑"), FontStyle.Bold, 50, New Drawing.Point(10, 10), StringFormat.GenericDefault)

        Dim fontSize As SizeF = g.MeasureString(_lyric, New Font(New FontFamily("微软雅黑"), 50, FontStyle.Bold))
        Dim textWidth As Integer = Convert.ToInt32(fontSize.Width * _percentage)
        Dim leftBMP As Bitmap = Nothing
        If textWidth > 0 Then
            leftBMP = New Bitmap(textWidth, Convert.ToInt32(fontSize.Height))
            Dim leftGraphic As Graphics = Graphics.FromImage(leftBMP)
            leftGraphic.CompositingQuality = CompositingQuality.HighQuality
            leftGraphic.SmoothingMode = SmoothingMode.HighQuality
            leftGraphic.TextRenderingHint = Drawing.Text.TextRenderingHint.SingleBitPerPixel
            leftGraphic.PixelOffsetMode = PixelOffsetMode.HighQuality
            leftGraphic.FillPath(brushRed, gp)
        End If



        'Dim regionLeft As New Region(gp)
        'Dim regionRight As New Region(gp)
        'g.Clip = regionLeft
        'Dim rect As RectangleF = regionLeft.GetBounds(g)
        'Dim rectLeft As New RectangleF(rect.Location, New SizeF(rect.Width * _percentage, rect.Height))
        'regionLeft.Intersect(rectLeft)
        'Dim rectRight As New RectangleF(New PointF(rect.X + rectLeft.Width, rect.Y), New SizeF(rect.Width - rectLeft.Width, rect.Height))
        'regionRight.Intersect(rectRight)
        'g.FillRegion(brushRed, regionLeft)
        'g.FillRegion(brushYellow, regionRight)
        g.FillPath(brushYellow, gp)
        If textWidth > 0 Then
            g.DrawImage(leftBMP, New Drawing.Point(0, 0))
            leftBMP.Dispose()
            GC.Collect()
        End If
    End Sub


    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)
        'Dim g As Graphics = e.Graphics
        'g.CompositingQuality = CompositingQuality.HighQuality
        'g.SmoothingMode = SmoothingMode.HighQuality
        'g.TextRenderingHint = Drawing.Text.TextRenderingHint.SingleBitPerPixel
        'g.PixelOffsetMode = PixelOffsetMode.HighQuality
        'Dim brushRed As New LinearGradientBrush(New Drawing.Point(0, 0), New Drawing.Point(0, 100), Color.Red, Color.DarkRed)
        'Dim brushYellow As New LinearGradientBrush(New Drawing.Point(0, 0), New Drawing.Point(0, 100), Color.Green, Color.LightGreen)

        'If _lyric Is Nothing Then Exit Sub
        'Dim gp As New GraphicsPath()
        'gp.AddString(_lyric, New FontFamily("微软雅黑"), FontStyle.Bold, 50, New Drawing.Point(10, 10), StringFormat.GenericDefault)

        'Dim fontSize As SizeF = g.MeasureString(_lyric, New Font(New FontFamily("微软雅黑"), 50, FontStyle.Bold))
        'Dim textWidth As Integer = Convert.ToInt32(fontSize.Width * _percentage)
        'Dim leftBMP As Bitmap = Nothing
        'If textWidth > 0 Then
        '    leftBMP = New Bitmap(textWidth, Convert.ToInt32(fontSize.Height))
        '    Dim leftGraphic As Graphics = Graphics.FromImage(leftBMP)
        '    leftGraphic.CompositingQuality = CompositingQuality.HighQuality
        '    leftGraphic.SmoothingMode = SmoothingMode.HighQuality
        '    leftGraphic.TextRenderingHint = Drawing.Text.TextRenderingHint.SingleBitPerPixel
        '    leftGraphic.PixelOffsetMode = PixelOffsetMode.HighQuality
        '    leftGraphic.FillPath(brushRed, gp)
        'End If



        'Dim regionLeft As New Region(gp)
        'Dim regionRight As New Region(gp)
        'g.Clip = regionLeft
        'Dim rect As RectangleF = regionLeft.GetBounds(g)
        'Dim rectLeft As New RectangleF(rect.Location, New SizeF(rect.Width * _percentage, rect.Height))
        'regionLeft.Intersect(rectLeft)
        'Dim rectRight As New RectangleF(New PointF(rect.X + rectLeft.Width, rect.Y), New SizeF(rect.Width - rectLeft.Width, rect.Height))
        'regionRight.Intersect(rectRight)
        'g.FillRegion(brushRed, regionLeft)
        'g.FillRegion(brushYellow, regionRight)
        'g.FillPath(brushYellow, gp)
        'If textWidth > 0 Then
        '    g.DrawImage(leftBMP, New Drawing.Point(0, 0))
        '    leftBMP.Dispose()
        '    GC.Collect()
        'End If

        'g.FillRectangle(brushRed, rect.X, rect.Y, Convert.ToInt32(rect.Width * _percentage), rect.Height)
        'g.FillRectangle(brushYellow, rect.X + Convert.ToInt32(rect.Width * _percentage), rect.Y, rect.Width - Convert.ToInt32(rect.Width * _percentage), rect.Height)


    End Sub

    Private Sub SampleDisplayForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Win32.SetWindowLong(Me.Handle, Win32.GWL_EXSTYLE, Win32.GetWindowLong(Me.Handle, Win32.GWL_EXSTYLE) Or Win32.WS_EX_LAYERED)
        Me.Width = Screen.PrimaryScreen.WorkingArea.Width
        Me.Height = 100
        Me.Top = Screen.PrimaryScreen.WorkingArea.Height - Me.Height - 50
        Me.Left = 0
    End Sub


    Private IsMoving As Boolean = False
    Private StartX, StartY, MouseX, MouseY As Integer
    Private Sub SampleDisplayForm_MouseDown(sender As Object, e As MouseEventArgs) Handles MyBase.MouseDown
        MouseX = e.X
        MouseY = e.Y
        IsMoving = True
    End Sub

    Private Sub SampleDisplayForm_MouseMove(sender As Object, e As MouseEventArgs) Handles MyBase.MouseMove
        If IsMoving = True Then
            Me.Location = New Drawing.Point((Me.Location.X - MouseX) + e.X, (Me.Location.Y - MouseY + e.Y))
        End If
    End Sub

    Private Sub SampleDisplayForm_MouseUp(sender As Object, e As MouseEventArgs) Handles MyBase.MouseUp
        IsMoving = False
    End Sub
End Class