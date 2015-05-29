Imports System.Threading
''' <summary>
''' Represent main interaction class of KaraokeShow
''' </summary>
Public Class KaraokeShow

#Region "Private Members"
    Private LRCCtrl As LyricSynchronizationController
    Private CanBackgroundMethodRunning As Boolean = False

    ''' <summary>
    ''' A background method for lyrics synchronization
    ''' </summary>
    Private Sub BackgroundSynchronization()
        Dim previousLyricIndex As Integer = -1
        While True
            'Whether to exit background method
            If CanBackgroundMethodRunning = False Then Exit Sub
            If LRCCtrl Is Nothing Then Exit Sub
            'Synchronization Code
            Dim nowLyricIndex As Integer = LRCCtrl.GetLyricIndex(Me.GetNowPosition().Invoke())
            If Not nowLyricIndex = previousLyricIndex Then 'prevent raise too many times
                DisplayManager.OnLyricsSentenceChanged(Me, New LyricsSentenceChangedEventArgs() With {.SentenceIndex = nowLyricIndex})
                nowLyricIndex = previousLyricIndex
            End If
            'To refresh word displaing progress
            Dim wordPercentage = LRCCtrl.GetWordPercentage(previousLyricIndex, Me.GetNowPosition().Invoke())
            DisplayManager.OnLyricsWordProgressChanged(Me, New LyricsWordProgressChangedEventArgs() With {.WordIndex = wordPercentage.WordIndex, .WordProgressPercentage = wordPercentage.Percentage})
            Thread.Sleep(100)
        End While
    End Sub
#End Region

    ''' <summary>
    ''' Get playing position of MusicBee
    ''' </summary>
    ''' <returns>Position(Mileseconds)</returns>
    Public Property GetNowPosition As Func(Of Integer)


    ''' <summary>
    ''' Stop all background method and others
    ''' </summary>
    Public Sub ResetPlayback()

    End Sub
    ''' <summary>
    ''' Start a new music playback
    ''' </summary>
    ''' <param name="Filename">The filename of music</param>
    ''' <param name="TrackTitle">The title of music</param>
    ''' <param name="Artist">The artist of music</param>
    Public Sub StartNewPlayback(Filename As String, TrackTitle As String, Artist As String)

    End Sub

    Public Event LyricsDownloadFinished(sender As Object, e As LyricsDownloadFinishedEventArgs)


End Class
