Imports System.Drawing
Imports System.Windows.Forms
Imports MusicBeePlugin
Public Class DesktopLyricsSetting
#Region "Publish Properties"
    Public Property FontSet As Font
        Set(value As Font)
            Button1.Font = value
            Label5.Text = value.Name + "," + value.Size.ToString() + "," + value.Style.ToString()
        End Set
        Get
            Return Button1.Font
        End Get
    End Property

    Public Property BorderColorA As Color
        Set(value As Color)
            PictureBox1.BackColor = value
        End Set
        Get
            Return PictureBox1.BackColor
        End Get
    End Property
    Public Property BorderColorB As Color
        Set(value As Color)
            PictureBox6.BackColor = value
        End Set
        Get
            Return PictureBox6.BackColor
        End Get
    End Property

    Public Property ColorA1 As Color
        Set(value As Color)
            PictureBox2.BackColor = value
        End Set
        Get
            Return PictureBox2.BackColor
        End Get
    End Property

    Public Property ColorA2 As Color
        Set(value As Color)
            PictureBox3.BackColor = value
        End Set
        Get
            Return PictureBox3.BackColor
        End Get
    End Property

    Public Property ColorB1 As Color
        Set(value As Color)
            PictureBox4.BackColor = value
        End Set
        Get
            Return PictureBox4.BackColor
        End Get
    End Property


    Public Property ColorB2 As Color
        Set(value As Color)
            PictureBox5.BackColor = value
        End Set
        Get
            Return PictureBox5.BackColor
        End Get
    End Property

#End Region

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim fontd As New FontDialog()
        fontd.Font = Button1.Font
        If fontd.ShowDialog() = DialogResult.OK Then
            Button1.Font = fontd.Font
            Label5.Text = fontd.Font.Name + "," + fontd.Font.Size.ToString() + "," + fontd.Font.Style.ToString()
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim color As New ColorDialog()
        color.Color = PictureBox1.BackColor
        If color.ShowDialog() = DialogResult.OK Then
            PictureBox1.BackColor = color.Color
        End If
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim color As New ColorDialog()
        color.Color = PictureBox2.BackColor
        If color.ShowDialog() = DialogResult.OK Then
            PictureBox2.BackColor = color.Color
        End If
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Dim color As New ColorDialog()
        color.Color = PictureBox3.BackColor
        If color.ShowDialog() = DialogResult.OK Then
            PictureBox3.BackColor = color.Color
        End If
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Dim color As New ColorDialog()
        color.Color = PictureBox4.BackColor
        If color.ShowDialog() = DialogResult.OK Then
            PictureBox4.BackColor = color.Color
        End If
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        Dim color As New ColorDialog()
        color.Color = PictureBox5.BackColor
        If color.ShowDialog() = DialogResult.OK Then
            PictureBox5.BackColor = color.Color
        End If
    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        Dim color As New ColorDialog()
        color.Color = PictureBox6.BackColor
        If color.ShowDialog() = DialogResult.OK Then
            PictureBox6.BackColor = color.Color
        End If
    End Sub
End Class
