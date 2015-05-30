Imports System.Threading
''' <summary>
''' Represent main interaction class of KaraokeShow
''' </summary>
Public Class KaraokeShow

#Region "Private Members"
    Private lrcCtrl As LyricSynchronizationController
    Private canBackgroundMethodRunning As Boolean = False
    Private filename As String
    Private trackTitle As String
    Private artist As String
    Private displayManager As DisplayManager

    ''' <summary>
    ''' A background method for lyrics synchronization
    ''' </summary>
    Private Sub BackgroundSynchronization()
        Dim previousLyricIndex As Integer = -1
        While True
            'Whether to exit background method
            If canBackgroundMethodRunning = False Then Exit Sub
            If LRCCtrl Is Nothing Then Continue While
            'Synchronization Code
            If (previousLyricIndex > (LRCCtrl.LRC.TimeLines.Count - 1）) Then Continue While
            Dim nowLyricIndex As Integer = LRCCtrl.GetLyricIndex(Me.GetNowPosition().Invoke())
            If Not nowLyricIndex = previousLyricIndex Then 'prevent raise too many times
                displayManager.SendLyricsSentenceChanged(nowLyricIndex)
                previousLyricIndex = nowLyricIndex
            End If
            'To refresh word displaing progress
            If (previousLyricIndex < 0) OrElse (previousLyricIndex > (LRCCtrl.LRC.TimeLines.Count - 1）) Then Continue While
            Dim wordPercentage = LRCCtrl.GetWordPercentage(previousLyricIndex, Me.GetNowPosition().Invoke())
            displayManager.SendLyricsWordProgressChanged(wordPercentage)
            Thread.Sleep(10)
        End While
    End Sub
    ''' <summary>
    ''' A background method for lyrics loading
    ''' </summary>
    Private Sub BackgroundLyricLoading()
        Dim lrcf = LyricsManager.SearchFromContainingFolder(Me.filename, Me.trackTitle, Me.artist)
        If lrcf Is Nothing Then lrcf = LyricsManager.SearchFromScraper(Me.trackTitle, Me.artist)
        If lrcf IsNot Nothing Then
            RaiseEvent LyricsDownloadFinished(Me, New LyricsFetchFinishedEventArgs() With {.Lyrics = lrcf})
        Else
            'If no lyrics can be found,KaraokeShow will be reset
            Me.ResetPlayback()
        End If
    End Sub

    ''' <summary>
    ''' Handler of event lyricsloadfinished
    ''' </summary>
    Private Sub LyricsLoadFinishedHandler(sender As Object, e As LyricsFetchFinishedEventArgs) Handles Me.LyricsDownloadFinished
        Me.LRCCtrl = New LyricSynchronizationController(e.Lyrics)
        'Tell DisplayManager to change the file
        displayManager.SendLyricsFileChanged((From i In Me.lrcCtrl.LRC.TimeLines Select i.Lyric).ToList())
    End Sub
#End Region

    Sub New(DisplayManager As DisplayManager)
        Me.displayManager = DisplayManager
    End Sub

    ''' <summary>
    ''' Get playing position of MusicBee
    ''' </summary>
    ''' <returns>Position(Mileseconds)</returns>
    Public Property GetNowPosition As Func(Of Integer)

    ''' <summary>
    ''' Get wether the playback of KaraokeShow is running
    ''' </summary>
    Public ReadOnly Property IsPlaybackRunning As Boolean
        Get
            Return Me.canBackgroundMethodRunning
        End Get
    End Property

    ''' <summary>
    ''' Stop all background method and others
    ''' </summary>
    Public Sub ResetPlayback()
        Me.canBackgroundMethodRunning = False
        displayManager.ResetAllLyricTicking()
        Me.LRCCtrl = Nothing
    End Sub
    ''' <summary>
    ''' Start a new music playback
    ''' </summary>
    ''' <param name="Filename">The filename of music</param>
    ''' <param name="TrackTitle">The title of music</param>
    ''' <param name="Artist">The artist of music</param>
    Public Sub StartNewPlayback(Filename As String, TrackTitle As String, Artist As String)
        Me.filename = Filename
        Me.trackTitle = TrackTitle
        Me.artist = Artist
        'Start Background synchronization
        Dim threadSync As New Thread(AddressOf BackgroundSynchronization)
        Me.canBackgroundMethodRunning = True

        threadSync.Start()
        'Start background loading
        Dim threadLoad As New Thread(Sub()
                                         Dim threadKidLoad As New Thread(AddressOf BackgroundLyricLoading)
                                         threadKidLoad.Start()
                                         Thread.Sleep(10000) 'Set timeout
                                         threadKidLoad.Abort()
                                     End Sub)
        threadLoad.Start()

    End Sub

    Public Event LyricsDownloadFinished(sender As Object, e As LyricsFetchFinishedEventArgs)


End Class
