''' <summary>
''' The abtract of a display in KaraokeShow
''' </summary>
Public Interface IDisplay
    Sub ShowDisplay()
    Sub CloseDisplay()
    Sub OnLyricsFileChanged(LyricsText As List(Of String))
    Sub OnLyricsSentenceChanged(SentenceIndex As Integer)
    Sub OnLyricsWordProgressChanged(WordProgressPercentage As Double)
    ReadOnly Property Visible As Boolean
End Interface
