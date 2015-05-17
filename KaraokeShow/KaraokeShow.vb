Imports System.Runtime.InteropServices
Imports System.Drawing
Imports System.Windows.Forms

Public Class Plugin
    Private mbApiInterface As New MusicBeeApiInterface
    Private about As New PluginInfo

    Private LF As LyricForm

    Public Function Initialise(ByVal apiInterfacePtr As IntPtr) As PluginInfo
        CopyMemory(mbApiInterface, apiInterfacePtr, 4)
        If mbApiInterface.MusicBeeVersion = MusicBeeVersion.v2_0 Then
            ' MusicBee version 2.0 - Api methods > revision 25 are not available
            CopyMemory(mbApiInterface, apiInterfacePtr, 456)
        ElseIf mbApiInterface.MusicBeeVersion = MusicBeeVersion.v2_1 Then
            CopyMemory(mbApiInterface, apiInterfacePtr, 516)
        ElseIf mbApiInterface.MusicBeeVersion = MusicBeeVersion.v2_2 Then
            CopyMemory(mbApiInterface, apiInterfacePtr, 584)
        ElseIf mbApiInterface.MusicBeeVersion = MusicBeeVersion.v2_3 Then
            CopyMemory(mbApiInterface, apiInterfacePtr, 596)
        ElseIf mbApiInterface.MusicBeeVersion = MusicBeeVersion.v2_4 Then
            CopyMemory(mbApiInterface, apiInterfacePtr, 604)
        Else
            CopyMemory(mbApiInterface, apiInterfacePtr, Marshal.SizeOf(mbApiInterface))
        End If
        about.PluginInfoVersion = PluginInfoVersion
        about.Name = "KaraokeShow"
        about.Description = "A lyric searching and displaying plugin for MusicBee"
        about.Author = "Samersions"
        'about.TargetApplication = "None"  ' current only applies to artwork, lyrics or instant messenger name that appears in the provider drop down selector or target Instant Messenger
        about.Type = PluginType.General
        about.VersionMajor = 1  ' your plugin version
        about.VersionMinor = 0
        about.Revision = 1
        'about.MinInterfaceVersion = MinInterfaceVersion
        'about.MinApiRevision = MinApiRevision
        about.ReceiveNotifications = ReceiveNotificationFlags.PlayerEvents
        about.ConfigurationPanelHeight = 200  ' height in pixels that musicbee should reserve in a panel for config settings. When set, a handle to an empty panel will be passed to the Configure function

        Return about
    End Function

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
            Dim button As New Button
            button.Text = "来个测试"
            button.Location = New Point(10, 10)
            AddHandler button.Click, Sub()
                                         MsgBox("MusicBee插件测试")
                                     End Sub
            configPanel.Controls.Add(button)
        End If
        Return True
    End Function

    ' called by MusicBee when the user clicks Apply or Save in the MusicBee Preferences screen.
    ' its up to you to figure out whether anything has changed and needs updating
    Public Sub SaveSettings()
        ' save any persistent settings in a sub-folder of this path
        Dim dataPath As String = mbApiInterface.Setting_GetPersistentStoragePath()
    End Sub

    ' MusicBee is closing the plugin (plugin is being disabled by user or MusicBee is shutting down)
    Public Sub Close(ByVal reason As PluginCloseReason)
        LF.StopAllBackgroundThread()
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
                ' perform startup initialisation
                LF = New LyricForm(mbApiInterface)
                mbApiInterface.MB_AddMenuItem("mnuTools/Karaoke Show", "", Sub()
                                                                               LF.Show()
                                                                           End Sub)
            Case NotificationType.PlayStateChanged
                If Not (mbApiInterface.Player_GetPlayState() = PlayState.Playing Or mbApiInterface.Player_GetPlayState() = PlayState.Paused) Then
                    LF.StopAllBackgroundThread()
                End If
        End Select
    End Sub
End Class
