﻿Imports System.Windows.Forms
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Threading.Tasks
Imports System.Threading
Imports MusicBeePlugin.Plugin
Imports System.IO

Public Class SampleDisplayForm
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
            Me.Refresh()
        End Set
    End Property
    Public WriteOnly Property Percentage As Double
        Set(value As Double)
            _percentage = value
            Me.Refresh()
        End Set
    End Property

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)
        e.Graphics.CompositingQuality = CompositingQuality.HighSpeed
        e.Graphics.SmoothingMode = SmoothingMode.HighSpeed

        If _lyric Is Nothing Then Exit Sub
        Dim gp As New GraphicsPath()
        gp.AddString(_lyric, New FontFamily("微软雅黑"), 0, 50, New Point(10, 10), StringFormat.GenericTypographic)

        Dim regionLeft As New Region(gp)
        Dim regionRight As New Region(gp)
        Dim rect As RectangleF = regionLeft.GetBounds(e.Graphics)
        Dim rectLeft As New RectangleF(rect.Location, New SizeF(rect.Width * _percentage, rect.Height))
        regionLeft.Intersect(rectLeft)
        Dim rectRight As New RectangleF(New PointF(rect.X + rectLeft.Width, rect.Y), New SizeF(rect.Width - rectLeft.Width, rect.Height))
        regionRight.Intersect(rectRight)
        Dim brushRed As New LinearGradientBrush(New Point(0, 0), New Point(10, 10), Color.Red, Color.OrangeRed)
        Dim brushYellow As New LinearGradientBrush(New Point(0, 0), New Point(10, 10), Color.Yellow, Color.GreenYellow)
        e.Graphics.FillRegion(brushRed, regionLeft)
        e.Graphics.FillRegion(brushYellow, regionRight)
    End Sub
End Class