Public Class KSSettingPanel

    Public ReadOnly Property SyncRate As String
        Get
            Return TextBox1.Text
        End Get
    End Property
    Public ReadOnly Property LyricsTimeout As String
        Get
            Return TextBox2.Text
        End Get
    End Property

    Private Sub LoadSettings()
        'Load Plugins List
        PluginManager.GetAllAvailableScrapers().Select(Function(s) s.FullName).ToList().ForEach(Sub(t) ComboBox1.Items.Add(t))
        PluginManager.GetAllAvailableDisplays().Select(Function(d) d.FullName).ToList().ForEach(Sub(t) ComboBox1.Items.Add(t))
        'Load Rate
        TextBox1.Text = If(SettingManager.InternalGetValue("synchronization_rate"), "25")
        'Load Timeout
        TextBox2.Text = If(SettingManager.InternalGetValue("lyrics_loading_timeout"), "10000")
    End Sub

    Private Sub KSSettingPanel_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadSettings()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim pluginType As Type = If(PluginManager.GetAllAvailableDisplays().FirstOrDefault(Function(s) s.FullName = ComboBox1.SelectedItem), PluginManager.GetAllAvailableScrapers().FirstOrDefault(Function(s) s.FullName = ComboBox1.SelectedItem))
        If pluginType Is Nothing Then
            MsgBox("Plugin Not Found", MsgBoxStyle.Critical)
            Exit Sub
        End If
        Dim plugin As IKSPlugin = Activator.CreateInstance(pluginType)
        plugin.DisplaySetting()
    End Sub
End Class
