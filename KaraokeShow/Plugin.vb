Imports System.Runtime.InteropServices
Imports System.Drawing
Imports System.Windows.Forms
Imports System.Reflection
Imports System.IO

Public Class Plugin
    Private mbApiInterface As New MusicBeeApiInterface
    Private about As New PluginInfo
    Private KaraokeShowInterface As KaraokeShow
    Private displayManager As DisplayManager

    Public Function Initialise(ByVal apiInterfacePtr As IntPtr) As PluginInfo
        MusicBeeApiInterface.Initialise(mbApiInterface, apiInterfacePtr)
        'initialize setting manager
        SettingManager.SettingStoragePath = mbApiInterface.Setting_GetPersistentStoragePath.Invoke()
        SettingManager.Load()
        'Bind I18nManager API
        InternationalizationManager.MB_GetLocalizationAPI = Function(id, d)
                                                                Return mbApiInterface.MB_GetLocalisation(id, d)
                                                            End Function
        'optize plugin manager
        PluginManager.KSPluginStorageFolder = New FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName + "\KaraokeShowPlugins\"
        PluginManager.InitializePluginInStorageFolder()
        'Load scraper manager
        ScraperManager.LoadScrapers()
        'Load KaraokeShow Mode
        Dim lrMode As Boolean = Boolean.Parse(If(SettingManager.InternalGetValue("convert_scraper_to_musicbee_lyrics_provider"), "False"))

        about.PluginInfoVersion = PluginInfoVersion
        about.Name = "KaraokeShow"
        about.Description = "A lyric searching and displaying plugin for MusicBee"
        about.Author = "Samersions"
        'about.TargetApplication = "None"  ' current only applies to artwork, lyrics or instant messenger name that appears in the provider drop down selector or target Instant Messenger
        about.Type = If(lrMode = True, PluginType.LyricsRetrieval, PluginType.General)
        about.VersionMajor = 1  ' your plugin version
        about.VersionMinor = 0
        about.Revision = 1
        'about.MinInterfaceVersion = MinInterfaceVersion
        'about.MinApiRevision = MinApiRevision
        about.ReceiveNotifications = ReceiveNotificationFlags.PlayerEvents
        about.ConfigurationPanelHeight = 200  ' height in pixels that musicbee should reserve in a panel for config settings. When set, a handle to an empty panel will be passed to the Configure function

        Return about
    End Function
    Private SettingPanel As KSSettingPanel
    Public Function Configure(ByVal panelHandle As IntPtr) As Boolean
        ' save any persistent settings in a sub-folder of this path
        Dim dataPath As String = mbApiInterface.Setting_GetPersistentStoragePath()
        ' panelHandle will only be set if you set about.ConfigurationPanelHeight to a non-zero value
        ' keep in mind the panel width is scaled according to the font the user has selected
        ' if about.ConfigurationPanelHeight is set to 0, you can display your own popup window
        If panelHandle <> IntPtr.Zero Then
            Dim configPanel As Panel = DirectCast(Panel.FromHandle(panelHandle), Panel)
            'Dim prompt As New Label
            'prompt.AutoSize = True
            'prompt.Location = New Point(0, 0)
            'prompt.Text = "prompt:"
            'Dim textBox As New TextBox
            'textBox.Bounds = New Rectangle(60, 0, 100, textBox.Height)
            'configPanel.Controls.AddRange(New Control() {prompt, textBox})
            SettingPanel = New KSSettingPanel()
            configPanel.Controls.Add(SettingPanel)
        End If
        Return True
    End Function

    ' called by MusicBee when the user clicks Apply or Save in the MusicBee Preferences screen.
    ' its up to you to figure out whether anything has changed and needs updating
    Public Sub SaveSettings()
        ' save any persistent settings in a sub-folder of this path
        Dim dataPath As String = mbApiInterface.Setting_GetPersistentStoragePath()
        If SettingPanel IsNot Nothing Then
            SettingManager.InternalSetValue("synchronization_rate", SettingPanel.SyncRate)
            SettingManager.InternalSetValue("lyrics_loading_timeout", SettingPanel.LyricsTimeout)
            SettingManager.InternalSetValue("convert_scraper_to_musicbee_lyrics_provider", SettingPanel.Scraper2Musicbee.ToString())
            SettingManager.InternalSetValue("use_internal_lyrics_scraper", SettingPanel.KSInternalLyricsLoader.ToString())
            SettingManager.Save()
            PluginManager.NotifyAllPluginResetSetting()
        End If
    End Sub

    ' MusicBee is closing the plugin (plugin is being disabled by user or MusicBee is shutting down)
    Public Sub Close(ByVal reason As PluginCloseReason)
        'Unload all plugins
        Me.displayManager.UnloadDisplayPlugin()
        ScraperManager.UnloadScrapers()
        'Reset playback
        Me.KaraokeShowInterface.ResetPlayback()
        'Write settings
        SettingManager.Save()
    End Sub

    ' uninstall this plugin - clean up any persisted files
    Public Sub Uninstall()
    End Sub

    ' receive event notifications from MusicBee
    ' you need to set about.ReceiveNotificationFlags = PlayerEvents to receive all notifications, and not just the startup event
    Public Sub ReceiveNotification(ByVal sourceFileUrl As String, ByVal type As NotificationType)
        ' perform some action depending on the notification type
        Select Case type
            Case NotificationType.PluginStartup
                'init all class
                displayManager = New DisplayManager()
                KaraokeShowInterface = New KaraokeShow(displayManager)
                KaraokeShowInterface.GetNowPosition = Function()
                                                          Return mbApiInterface.Player_GetPosition()
                                                      End Function
                KaraokeShowInterface.GetLrycisFromMusicbee = Function()
                                                                 Return mbApiInterface.NowPlaying_GetLyrics()
                                                             End Function
                'Load basic manager
                displayManager.LoadDisplayPlugin()
                'Add all display
                mbApiInterface.MB_AddMenuItem("mnuView/Karaoke Show", "", Sub()
                                                                          End Sub).Visible = True
                displayManager.GetDisplays().ToList().ForEach(Sub(d)
                                                                  mbApiInterface.MB_AddMenuItem("mnuView/Karaoke Show/" + d.GetType().Name, "", Sub()
                                                                                                                                                    If d.Visible = True Then displayManager.SetDisplayVisibility(d.GetType().FullName, False) Else displayManager.SetDisplayVisibility(d.GetType().FullName, True)
                                                                                                                                                End Sub).Visible = True
                                                              End Sub)
            ' perform startup initialisation
            Case NotificationType.TrackChanged
                Me.KaraokeShowInterface.ResetPlayback()

                Threading.Thread.Sleep(500)
                Me.KaraokeShowInterface.StartNewPlayback(mbApiInterface.NowPlaying_GetFileProperty(FilePropertyType.Url), mbApiInterface.NowPlaying_GetFileTag(MetaDataType.TrackTitle), mbApiInterface.NowPlaying_GetFileTag(MetaDataType.Artist))

                'If Not (mbApiInterface.Player_GetPlayState() = PlayState.Playing Or mbApiInterface.Player_GetPlayState() = PlayState.Paused) Then
                '    Me.KaraokeShowInterface.ResetPlayback()
                'End If
                'If mbApiInterface.Player_GetPlayState() = PlayState.Playing Then
                '    If Me.KaraokeShowInterface.IsPlaybackRunning = False Then
                '        Me.KaraokeShowInterface.ResetPlayback()
                '        Me.KaraokeShowInterface.StartNewPlayback(mbApiInterface.NowPlaying_GetFileProperty(FilePropertyType.Url), mbApiInterface.NowPlaying_GetFileTag(MetaDataType.TrackTitle), mbApiInterface.NowPlaying_GetFileTag(MetaDataType.Artist))
                '    End If
                'End If
            Case NotificationType.PlayStateChanged
                If mbApiInterface.Player_GetPlayState() = PlayState.Stopped Then
                    Me.KaraokeShowInterface.ResetPlayback()
                End If
        End Select
    End Sub

    ' return an array of lyric or artwork provider names this plugin supports
    ' the providers will be iterated through one by one and passed to the RetrieveLyrics/ RetrieveArtwork function in order set by the user in the MusicBee Tags(2) preferences screen until a match is found
    Public Function GetProviders() As String()
        'If LyricsRetrieval mode is enabled, return provider
        Dim lrMode As Boolean = Boolean.Parse(If(SettingManager.InternalGetValue("convert_scraper_to_musicbee_lyrics_provider"), "False"))
        If lrMode = True Then
            Return PluginManager.GetAllAvailableScrapers().Select(Function(t) $"{t.Name}(KaraokeShow)").ToArray()
        Else
            Return New String() {}
        End If
    End Function

    ' return lyrics for the requested artist/title from the requested provider
    ' only required if PluginType = LyricsRetrieval
    ' return Nothing if no lyrics are found
    Public Function RetrieveLyrics(ByVal sourceFileUrl As String, ByVal artist As String, ByVal trackTitle As String, ByVal album As String, ByVal synchronisedPreferred As Boolean, ByVal provider As String) As String
        If Not provider.Contains("(KaraokeShow)") Then Return Nothing
        'Get scraper instance
        Dim scraper As IScraper = ScraperManager.GetScraper(provider.Replace("(KaraokeShow)", ""))
        'Search lyrics
        Dim lyricsInfo() As ScraperLyricInfo = ScraperManager.SearchLyricsFrom(scraper, trackTitle, artist)
        If lyricsInfo.Length = 0 Then Return Nothing
        'Download Lyrics
        Dim lyricsText As String = ScraperManager.DownloadLyricsFrom(scraper, lyricsInfo(0))
        If lyricsText IsNot Nothing AndAlso lyricsText <> "" Then Return lyricsText Else Return Nothing
    End Function

    ' return Base64 string representation of the artwork binary data from the requested provider
    ' only required if PluginType = ArtworkRetrieval
    ' return Nothing if no artwork is found
    Public Function RetrieveArtwork(ByVal sourceFileUrl As String, ByVal albumArtist As String, ByVal album As String, ByVal provider As String) As String
        'Return Convert.ToBase64String(artworkBinaryData)
        Return Nothing
    End Function
End Class
