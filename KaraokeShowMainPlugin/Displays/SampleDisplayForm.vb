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
    Private Function DrawOnBMP() As Bitmap
        Dim bmp As New Bitmap(Me.Width, Me.Height)
        Dim g As Graphics = Graphics.FromImage(bmp)
        g.CompositingQuality = CompositingQuality.HighSpeed
        g.SmoothingMode = SmoothingMode.HighSpeed
        If _lyric Is Nothing Then Return Nothing
        Dim gp As New GraphicsPath()
        gp.AddString(_lyric, New FontFamily("微软雅黑"), 0, 50, New Point(10, 10), StringFormat.GenericTypographic)

        Dim regionLeft As New Region(gp)
        'Dim regionRight As New Region(gp)
        g.Clip = regionLeft
        Dim rect As RectangleF = regionLeft.GetBounds(g)
        'Dim rectLeft As New RectangleF(rect.Location, New SizeF(rect.Width * _percentage, rect.Height))
        'regionLeft.Intersect(rectLeft)
        'Dim rectRight As New RectangleF(New PointF(rect.X + rectLeft.Width, rect.Y), New SizeF(rect.Width - rectLeft.Width, rect.Height))
        'regionRight.Intersect(rectRight)
        Dim brushRed As New LinearGradientBrush(New Point(0, 0), New Point(30, 30), Color.Red, Color.OrangeRed)
        Dim brushYellow As New LinearGradientBrush(New Point(0, 0), New Point(30, 30), Color.Yellow, Color.GreenYellow)
        'e.Graphics.FillRegion(brushRed, regionLeft)
        'e.Graphics.FillRegion(brushYellow, regionRight)
        g.FillRectangle(brushRed, rect.X, rect.Y, Convert.ToInt32(rect.Width * _percentage), rect.Height)
        g.FillRectangle(brushYellow, rect.X + Convert.ToInt32(rect.Width * _percentage), rect.Y, rect.Width - Convert.ToInt32(rect.Width * _percentage), rect.Height)
        Return bmp
    End Function
    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)
        Dim bmp = DrawOnBMP()
        If Not bmp Is Nothing Then
            e.Graphics.DrawImage(bmp, New Point(0, 0))
            bmp.Dispose()
        End If

    End Sub
End Class