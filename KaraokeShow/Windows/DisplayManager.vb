Public Class DisplayManager

    Private Shared DisplayList As New List(Of IDisplay)
    Private Shared LyricsText As New List(Of String)
    Private Shared NowLyric As String = ""

    Public Shared Sub ResetAllLyricTicking()
        DisplayManager.LyricsText = Nothing
        DisplayManager.NowLyric = Nothing
    End Sub

    Public Shared Sub SendLyricsFileChanged(LyricsText As List(Of String))
        DisplayManager.LyricsText = LyricsText
        For Each item In DisplayManager.DisplayList
            item.OnLyricsFileChanged(LyricsText)
        Next
    End Sub
    Public Shared Sub SendLyricsSentenceChanged(SentenceIndex As Integer)
        If (LyricsText Is Nothing) OrElse (LyricsText.Count < SentenceIndex + 1) Then Exit Sub
        NowLyric = LyricsText(SentenceIndex)
        For Each item In DisplayManager.DisplayList
            item.OnLyricsSentenceChanged(SentenceIndex)
        Next

    End Sub
    Public Shared Sub SendLyricsWordProgressChanged(WordIndex As Integer, WordProgressPercentage As Double)
        If (NowLyric Is Nothing) OrElse (NowLyric.Length < WordIndex + 1) Then Exit Sub
        For Each item In DisplayManager.DisplayList
            item.OnLyricsWordProgressChanged(WordIndex, WordProgressPercentage)
        Next
    End Sub

    Public Shared Sub LoadDisplayPlugin()
        DisplayManager.DisplayList.Add(New SampleDisplay)
    End Sub

    Public Shared Function GetDisplayNames() As String()
        Return (From i In DisplayManager.DisplayList Select i.GetType().Name)
    End Function

    Public Shared Sub SetDisplayVisibility(DisplayName As String, IsVisible As Boolean)
        Dim display = (From i In DisplayManager.DisplayList Where i.GetType().Name = DisplayName).FirstOrDefault()
        If display IsNot Nothing Then
            If IsVisible = True Then display.ShowDisplay() Else display.CloseDisplay()
        End If
    End Sub

End Class
