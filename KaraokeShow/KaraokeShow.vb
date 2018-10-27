Imports System.Threading
Imports System.Threading.Tasks
''' <summary>
''' Represent main interaction class of KaraokeShow
''' </summary>
Public Class KaraokeShow

#Region "Private Members"

#Region "Settings"
    Private ReadOnly Property set_RefreshRate As Integer
        Get
            Return Integer.Parse(If(SettingManager.InternalGetValue("synchronization_rate"), "25"))
        End Get
    End Property
    Private ReadOnly Property set_LyricsLoadTimeout As Integer
        Get
            Return Integer.Parse(If(SettingManager.InternalGetValue("lyrics_loading_timeout"), "10000"))

        End Get
    End Property
    Private ReadOnly Property set_InternalLyricsScraper As Boolean
        Get
            Return Boolean.Parse(If(SettingManager.InternalGetValue("use_internal_lyrics_scraper"), "False"))
        End Get
    End Property

#End Region

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
        Dim previousPercentage As Double = 0
        While True
            'Whether to exit background method
            If canBackgroundMethodRunning = False Then Exit Sub
            If lrcCtrl Is Nothing Then Continue While
            'Synchronization Code
            If (previousLyricIndex > (lrcCtrl.LRC.TimeLines.Count - 1）) Then Continue While
            Dim nowLyricIndex As Integer = lrcCtrl.GetLyricIndex(Me.GetNowPosition().Invoke())
            If Not nowLyricIndex = previousLyricIndex Then 'prevent raise too many times
                displayManager.SendLyricsSentenceChanged(nowLyricIndex)
                previousLyricIndex = nowLyricIndex
            End If
            'To refresh word displaing progress
            If (previousLyricIndex < 0) OrElse lrcCtrl Is Nothing OrElse (previousLyricIndex > (lrcCtrl.LRC.TimeLines.Count - 1）) Then Continue While
            Dim wordPercentage = lrcCtrl.GetWordPercentage(previousLyricIndex, Me.GetNowPosition().Invoke())
            If Not previousPercentage = wordPercentage Then displayManager.SendLyricsWordProgressChanged(wordPercentage)
            previousPercentage = wordPercentage
            Thread.Sleep(set_RefreshRate)
        End While
    End Sub
    ''' <summary>
    ''' A background method for lyrics loading
    ''' </summary>
    Private Sub BackgroundLyricLoading()
        If set_InternalLyricsScraper Then
            'TODO:User can change the order.
            Dim lrcf As LRCFile
            If (Me.GetLrycisFromMusicbee.Invoke <> "") Then
                lrcf = New LRCFile(Me.GetLrycisFromMusicbee.Invoke())
            Else
                lrcf = LyricsManager.SearchFromContainingFolder(Me.filename, Me.trackTitle, Me.artist)
            End If
            If lrcf Is Nothing Then lrcf = LyricsManager.SearchFromScraper(Me.trackTitle, Me.artist)
            If lrcf IsNot Nothing Then
                RaiseEvent LyricsDownloadFinished(Me, New LyricsFetchFinishedEventArgs() With {.Lyrics = lrcf})
            Else
                'If no lyrics can be found,KaraokeShow will be reset
                Me.ResetPlayback()
            End If
        Else
            'Load MusicBee Lyrics until timeout
            While True
                If Me.GetLrycisFromMusicbee.Invoke() <> "" Then
                    Dim lrcf As LRCFile
                    lrcf = New LRCFile(Me.GetLrycisFromMusicbee.Invoke())
                    RaiseEvent LyricsDownloadFinished(Me, New LyricsFetchFinishedEventArgs() With {.Lyrics = lrcf})
                    Exit While
                End If
            End While
        End If

    End Sub

    ''' <summary>
    ''' Handler of event lyricsloadfinished
    ''' </summary>
    Private Sub LyricsLoadFinishedHandler(sender As Object, e As LyricsFetchFinishedEventArgs) Handles Me.LyricsDownloadFinished
        Me.lrcCtrl = New LyricSynchronizationController(e.Lyrics)
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
    ''' Get lyrcis from MusicBee
    ''' </summary>
    Public Property GetLrycisFromMusicbee As Func(Of String)

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
        Me.lrcCtrl = Nothing
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
        Dim threadSync As New Task(AddressOf BackgroundSynchronization)
        Me.canBackgroundMethodRunning = True

        threadSync.Start()
        'Start background loading
        Dim threadLoad As New Task(Sub()
                                       Dim cts As New CancellationTokenSource()
                                       Dim threadKidLoad As New Task(AddressOf BackgroundLyricLoading, cts)
                                       threadKidLoad.Start()
                                       Thread.Sleep(set_LyricsLoadTimeout) 'Set timeout
                                       If Not threadKidLoad.IsCompleted = True Then
                                           Me.ResetPlayback()
                                       End If
                                       cts.Cancel()
                                   End Sub)
        threadLoad.Start()

    End Sub

    Public Event LyricsDownloadFinished(sender As Object, e As LyricsFetchFinishedEventArgs)


End Class
