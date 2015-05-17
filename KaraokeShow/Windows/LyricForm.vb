Imports System.Windows.Forms
Imports System.Threading.Tasks
Imports System.Threading
Imports MusicBeePlugin.Plugin
Imports System.IO

Public Class LyricForm
    Private mbi As MusicBeeApiInterface
    Private LRCFI As LRCFile
    Private LRCSyncCtrl As LyricSynchronizationController
    Sub New(mbi As MusicBeeApiInterface)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.mbi = mbi
    End Sub
    Sub StopAllBackgroundThread()
        Me.IsBackgroundThreadStopped = True
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim open As New OpenFileDialog()
        If open.ShowDialog = DialogResult.OK Then
            Me.IsBackgroundThreadStopped = True
            Me.LRCFI = New LRCFile(File.ReadAllText(open.FileName, System.Text.Encoding.Default))
            Me.LRCSyncCtrl = New LyricSynchronizationController(Me.LRCFI)
            Dim t As New Thread(Sub()
                                    Dim previousLyric As String = ""
                                    While True
                                        If Me.IsBackgroundThreadStopped = True Then Exit Sub 'Exit if this flag is set true
                                        Dim nowLyric As String = Me.LRCSyncCtrl.GetLyric(mbi.Player_GetPosition())
                                        If Not previousLyric = nowLyric Then 'prevent "flash"
                                            Me.Label1.BeginInvoke(Sub()
                                                                      Me.Label1.Text = nowLyric 'Update lyric
                                            End Sub)
                                        End If
                                        Thread.Sleep(100)
                                    End While
                                End Sub)
            Me.IsBackgroundThreadStopped = False 'set to this value to make background threading start
            t.Start()
        End If
    End Sub

    Private IsBackgroundThreadStopped As Boolean = True
End Class