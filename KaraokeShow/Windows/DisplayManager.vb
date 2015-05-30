''' <summary>
''' The manager of all display,used to load display from PluginManager and be as a hub to resend message coming from Karaokeshow to all display
''' </summary>
Public Class DisplayManager
    ''' <summary>
    ''' Contains all displays loading from PluginManager
    ''' </summary>
    Private DisplayList As New List(Of IDisplay)
    ''' <summary>
    ''' a cache to prevent overflow
    ''' </summary>
    Private LyricsText As New List(Of String)
    ''' <summary>
    ''' a cache to prevent overflow
    ''' </summary>
    Private NowLyric As String = ""

    ''' <summary>
    ''' Reset all display state
    ''' </summary>
    Public Sub ResetAllLyricTicking()
        Me.LyricsText = Nothing
        Me.NowLyric = Nothing
    End Sub
    ''' <summary>
    ''' Notify all display that a new lyric file is coming
    ''' </summary>
    Public Sub SendLyricsFileChanged(LyricsText As List(Of String))
        Me.LyricsText = LyricsText
        For Each item In Me.DisplayList
            item.OnLyricsFileChanged(LyricsText)
        Next
    End Sub
    ''' <summary>
    ''' Notify all displays that sentence has changed
    ''' </summary>
    Public Sub SendLyricsSentenceChanged(SentenceIndex As Integer)
        If (LyricsText Is Nothing) OrElse (LyricsText.Count < SentenceIndex + 1) Then Exit Sub
        NowLyric = LyricsText(SentenceIndex)
        For Each item In Me.DisplayList
            item.OnLyricsSentenceChanged(SentenceIndex)
        Next
    End Sub
    ''' <summary>
    ''' Notify all displays that word percentage has changed
    ''' </summary>
    Public Sub SendLyricsWordProgressChanged(WordIndex As Integer, WordProgressPercentage As Double)
        If (NowLyric Is Nothing) OrElse (NowLyric.Length < WordIndex + 1) Then Exit Sub
        For Each item In Me.DisplayList
            item.OnLyricsWordProgressChanged(WordIndex, WordProgressPercentage)
        Next
    End Sub

    ''' <summary>
    ''' Load all displays from PluginManager
    ''' </summary>
    Public Sub LoadDisplayPlugin()
        Me.DisplayList.Add(New SampleDisplay)
    End Sub
    ''' <summary>
    ''' Get name of all displays
    ''' </summary>
    Public Function GetDisplayNames() As String()
        Return (From i In Me.DisplayList Select i.GetType().Name)
    End Function

    ''' <summary>
    ''' Set a display's visibility
    ''' </summary>
    Public Sub SetDisplayVisibility(DisplayName As String, IsVisible As Boolean)
        Dim display = (From i In Me.DisplayList Where i.GetType().Name = DisplayName).FirstOrDefault()
        If display IsNot Nothing Then
            If IsVisible = True Then display.ShowDisplay() Else display.CloseDisplay()
        End If
    End Sub

End Class
