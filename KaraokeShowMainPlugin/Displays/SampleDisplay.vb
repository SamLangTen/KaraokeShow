Imports MusicBeePlugin

Public Class SampleDisplay
    Implements IDisplay
    Implements IKSPlugin

    Private _SDF As SampleDisplayForm
    Private LyricsText As New List(Of String)
    Private SentenceIndex As Integer = 0
    Private WordPercentage As Double = 0


    Public Property Description As String Implements IKSPlugin.Description


    Public Property GetSetting As KSPlugin_Setting_GetDelegate Implements IKSPlugin.GetSetting


    Public Property SetSetting As KSPlugin_Setting_SetDelegate Implements IKSPlugin.SetSetting

    Public ReadOnly Property Visible As Boolean Implements IDisplay.Visible
        Get
            Return SDF.Visible
        End Get
    End Property

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
        Me.SentenceIndex = SentenceIndex
        If Not SentenceIndex > Me.LyricsText.Count - 1 Then SDF.Lyric = Me.LyricsText(Me.SentenceIndex)
    End Sub

    Public Sub OnLyricsWordProgressChanged(WordProgressPercentage As Double) Implements IDisplay.OnLyricsWordProgressChanged
        If _SDF Is Nothing OrElse _SDF.IsDisposed Then Exit Sub
        Me.WordPercentage = WordProgressPercentage
        SDF.Percentage = Me.WordPercentage
    End Sub

    Public Sub ShowDisplay() Implements IDisplay.ShowDisplay
        SDF.Show()
        OnLyricsSentenceChanged(Me.SentenceIndex)
        OnLyricsWordProgressChanged(Me.WordPercentage)
    End Sub
End Class
