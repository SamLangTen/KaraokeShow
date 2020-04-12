''' <summary>
''' The manager of all display,used to load display from PluginManager and be as a hub to resend message coming from Karaokeshow to all display
''' </summary>
Friend Class DisplayManager
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

    Private DisplayVisibilityMap As Dictionary(Of String, Boolean)

    Private Function GetDisplayTypeIdentification(displayType As Type) As String
        Return $"{displayType.Assembly.FullName.Split(",")(0)}/{displayType.FullName}"
    End Function


    ''' <summary>
    ''' Reset all display state
    ''' </summary>
    Public Sub ResetAllLyricTicking()
        Me.LyricsText = Nothing
        Me.NowLyric = Nothing
        Me.SendLyricsFileChanged(Nothing)
        Me.SendLyricsSentenceChanged(0)
        Me.SendLyricsWordProgressChanged(0)
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
    Public Sub SendLyricsWordProgressChanged(WordProgressPercentage As Double)
        For Each item In Me.DisplayList
            item.OnLyricsWordProgressChanged(WordProgressPercentage)
        Next
    End Sub

    ''' <summary>
    ''' Load all displays from PluginManager
    ''' </summary>
    Public Sub LoadDisplayPlugin()
        'Load Visibility setting
        Dim text As String = SettingManager.InternalGetValue("DisplayVisibilities")
        If text = "" Then
            DisplayVisibilityMap = New Dictionary(Of String, Boolean)
        Else
            DisplayVisibilityMap = text.Split("|").ToDictionary(Of String, Boolean)(Function(x) x.Split("=")(0), Function(x) Boolean.Parse(x.Split("=")(1)))
        End If
        Me.DisplayList.Clear()
        PluginManager.GetAllAvailableDisplays().ForEach(Sub(t)
                                                            Dim display = DirectCast(PluginManager.CreateInstance(t), IDisplay)
                                                            Me.DisplayList.Add(display)
                                                            If DisplayVisibilityMap.ContainsKey(GetDisplayTypeIdentification(display.GetType())) Then
                                                                Dim isVisible = DisplayVisibilityMap.Item(GetDisplayTypeIdentification(display.GetType()))
                                                                If isVisible = True Then display.ShowDisplay() Else display.CloseDisplay()
                                                            End If

                                                        End Sub)
    End Sub

    ''' <summary>
    ''' Unload all displays 
    ''' </summary>
    Public Sub UnloadDisplayPlugin()
        Me.DisplayList.ForEach(Sub(d)
                                   CType(d, IKSPlugin).OnUnloaded()
                               End Sub)
        Me.DisplayList.Clear()
        'Save visibility setting
        Dim visibilityText As String = String.Join("|", (From i In Me.DisplayVisibilityMap Select $"{i.Key}={i.Value.ToString()}").ToList())
        SettingManager.InternalSetValue("DisplayVisibilities", visibilityText)
    End Sub

    ''' <summary>
    ''' Get name of all displays
    ''' </summary>
    Public Function GetDisplayNames() As String()
        Return (From i In Me.DisplayList Select i.GetType().Name)
    End Function

    Public Function GetDisplays() As IDisplay()
        Return Me.DisplayList.ToArray()
    End Function

    ''' <summary>
    ''' Set a display's visibility
    ''' </summary>
    Public Sub SetDisplayVisibility(DisplayName As String, IsVisible As Boolean)
        Dim display = (From i In Me.DisplayList Where i.GetType().FullName = DisplayName).FirstOrDefault()
        If display IsNot Nothing Then
            If IsVisible = True Then display.ShowDisplay() Else display.CloseDisplay()
            DisplayVisibilityMap.Item(GetDisplayTypeIdentification(display.GetType())) = IsVisible
        End If
    End Sub

End Class
