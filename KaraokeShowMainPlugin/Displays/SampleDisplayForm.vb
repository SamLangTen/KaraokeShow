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

        ' Add any initialization after the InitializeComponent() call.
        Me.SetStyle(ControlStyles.AllPaintingInWmPaint, True)
        Me.SetStyle(ControlStyles.DoubleBuffer, True)
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
                               Me.Refresh()
                           End Sub)
        End Set
    End Property



    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)
        Dim g As Graphics = e.Graphics
        g.CompositingQuality = CompositingQuality.HighQuality
        g.SmoothingMode = SmoothingMode.HighQuality
        g.TextRenderingHint = Drawing.Text.TextRenderingHint.SingleBitPerPixel
        g.PixelOffsetMode = PixelOffsetMode.HighQuality
        Dim brushRed As New LinearGradientBrush(New Point(0, 0), New Point(0, 100), Color.Red, Color.DarkRed)
        Dim brushYellow As New LinearGradientBrush(New Point(0, 0), New Point(0, 100), Color.Green, Color.LightGreen)

        If _lyric Is Nothing Then Exit Sub
        Dim gp As New GraphicsPath()
        gp.AddString(_lyric, New FontFamily("微软雅黑"), FontStyle.Bold, 50, New Point(10, 10), StringFormat.GenericDefault)

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
            g.DrawImage(leftBMP, New Point(0, 0))
            leftBMP.Dispose()
            GC.Collect()
        End If

        'g.FillRectangle(brushRed, rect.X, rect.Y, Convert.ToInt32(rect.Width * _percentage), rect.Height)
        'g.FillRectangle(brushYellow, rect.X + Convert.ToInt32(rect.Width * _percentage), rect.Y, rect.Width - Convert.ToInt32(rect.Width * _percentage), rect.Height)


    End Sub
End Class