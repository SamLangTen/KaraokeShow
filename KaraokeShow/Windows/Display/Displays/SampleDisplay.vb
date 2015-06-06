Imports MusicBeePlugin

Public Class SampleDisplay
    Implements IDisplay

    Private _SDF As SampleDisplayForm
    Private LyricsText As New List(Of String)
    Private ReadOnly Property SDF() As SampleDisplayForm
        Get
            If _SDF Is Nothing OrElse (_SDF.IsDisposed = True) Then _SDF = New SampleDisplayForm
            Return _SDF
        End Get
    End Property


    Public Sub CloseDisplay() Implements IDisplay.CloseDisplay
        SDF.Close()
    End Sub

    Public Sub OnLyricsFileChanged(LyricsText As List(Of String)) Implements IDisplay.OnLyricsFileChanged
        Me.LyricsText = LyricsText
    End Sub

    Public Sub OnLyricsSentenceChanged(SentenceIndex As Integer) Implements IDisplay.OnLyricsSentenceChanged
        If _SDF Is Nothing OrElse _SDF.IsDisposed Then Exit Sub
        SDF.Lyric = Me.LyricsText(SentenceIndex)
    End Sub

    Public Sub OnLyricsWordProgressChanged(WordProgressPercentage As Double) Implements IDisplay.OnLyricsWordProgressChanged
        If _SDF Is Nothing OrElse _SDF.IsDisposed Then Exit Sub
        SDF.Percentage = WordProgressPercentage
    End Sub

    Public Sub ShowDisplay() Implements IDisplay.ShowDisplay
        SDF.Show()
    End Sub
End Class
