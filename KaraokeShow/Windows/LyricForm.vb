Imports System.Windows.Forms
Imports System.Threading
Imports MusicBeePlugin.Plugin

Public Class LyricForm
    Private mbi As MusicBeeApiInterface
    Private LRCFI As LrcFile
    Sub New(mbi As MusicBeeApiInterface)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.mbi = mbi
        Control.CheckForIllegalCrossThreadCalls = False
    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim open As New OpenFileDialog()
        If open.ShowDialog = DialogResult.OK Then
            Me.LRCFI = New LrcFile()
            LRCFI.LoadFileInNormalMode(open.FileName)
            Dim thread As New Thread(AddressOf LyricsControl)
            thread.Start()
        End If
    End Sub
    Private LastTL As New LrcTimeline
    Private Sub LyricsControl()
        While True
            If mbi.Player_GetPlayState() = PlayState.Playing Then
                For i As Integer = 0 To LRCFI.TimeLines.Count - 1
                    Dim item As LrcTimeline = LRCFI.TimeLines(i)
                    If Not item.StartPoint = LastTL.StartPoint Then
                        If Not i + 2 > LRCFI.TimeLines.Count Then

                            Dim biggers, smallers As Integer
                            smallers = item.StartPoint.Millisecond + item.StartPoint.Second * 1000 + item.StartPoint.Minute * 60000 + item.StartPoint.Hour * 3600000
                            Dim biggitem As LrcTimeline = LRCFI.TimeLines(i + 1)
                            biggers = biggitem.StartPoint.Millisecond + biggitem.StartPoint.Second * 1000 + biggitem.StartPoint.Minute * 60000 + biggitem.StartPoint.Hour * 3600000
                            If mbi.Player_GetPosition() >= smallers And mbi.Player_GetPosition() < biggers Then
                                Me.Label1.Text = item.Lyric
                                LastTL = item
                            End If
                        Else
                            Dim smallers As Integer
                            smallers = item.StartPoint.Millisecond + item.StartPoint.Second * 1000 + item.StartPoint.Minute * 60000 + item.StartPoint.Hour * 3600000
                            If mbi.Player_GetPosition() >= smallers Then
                                Me.Label1.Text = item.Lyric
                            End If
                        End If
                    End If
                Next
            End If
            Thread.Sleep(100)
        End While

    End Sub
End Class