﻿Imports System.Drawing
Imports System.Windows.Forms

Public Class DesktopLyricsForm

    ''' <summary>
    ''' Refresh Layered Window
    ''' </summary>
    Public Sub UpdateLayeredWindow(bmp As Bitmap)
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
    End Sub

    Private Sub DesktopLyricsForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Win32.SetWindowLong(Me.Handle, Win32.GWL_EXSTYLE, Win32.GetWindowLong(Me.Handle, Win32.GWL_EXSTYLE) Or Win32.WS_EX_LAYERED)
        Me.Width = Screen.PrimaryScreen.WorkingArea.Width
        Me.Height = 100
        Me.Top = Screen.PrimaryScreen.WorkingArea.Height - Me.Height - 50
        Me.Left = 0
    End Sub

#Region "Window Movement"
    Private mouseX, mouseY As Integer
    Private Sub DesktopLyricsForm_MouseDown(sender As Object, e As MouseEventArgs) Handles Me.MouseDown
        mouseX = e.X
        mouseY = e.Y
    End Sub

    Private Sub DesktopLyricsForm_MouseMove(sender As Object, e As MouseEventArgs) Handles Me.MouseMove
        If e.Button = MouseButtons.Left Then
            Me.Location = New Drawing.Point((Me.Location.X - MouseX) + e.X, (Me.Location.Y - MouseY + e.Y))
        End If
    End Sub
#End Region

End Class