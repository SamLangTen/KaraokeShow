Imports System.Globalization
Imports System.Threading
Imports System.Windows.Forms

Public Class KSSettingPanel
    Private Class ComboboxItemPluginTypePair

        Public Property Fullname As String
        Public Property DisplayName As String

        Public Overrides Function ToString() As String
            Return DisplayName
        End Function
    End Class

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
    Public ReadOnly Property Scraper2Musicbee As Boolean
        Get
            Return CheckBox1.Checked
        End Get
    End Property

    Public ReadOnly Property KSInternalLyricsLoader As Boolean
        Get
            Return CheckBox2.Checked
        End Get
    End Property

    Private Sub LoadSettings()
        'Load Plugins List
        PluginManager.GetAllAvailableScrapers().ForEach(Sub(t) ComboBox1.Items.Add(New ComboboxItemPluginTypePair() With {.Fullname = t.FullName, .DisplayName = $"{t.Name}({t.Assembly.FullName})"}))
        PluginManager.GetAllAvailableDisplays().ForEach(Sub(t) ComboBox1.Items.Add(New ComboboxItemPluginTypePair() With {.Fullname = t.FullName, .DisplayName = $"{t.Name}({t.Assembly.FullName})"}))
        'Load Rate
        TextBox1.Text = If(SettingManager.InternalGetValue("synchronization_rate"), "25")
        'Load Timeout
        TextBox2.Text = If(SettingManager.InternalGetValue("lyrics_loading_timeout"), "10000")
        'Scraper to LR
        CheckBox1.Checked = Boolean.Parse(If(SettingManager.InternalGetValue("convert_scraper_to_musicbee_lyrics_provider"), "False"))
        'Internal Scraper
        CheckBox2.Checked = Boolean.Parse(If(SettingManager.InternalGetValue("use_internal_lyrics_scraper"), "False"))
    End Sub

    Private Sub ApplyResource()
        Dim res = New System.ComponentModel.ComponentResourceManager(Me.GetType())
        For Each item As Control In Me.Controls
            res.ApplyResources(item, item.Name)
        Next
        Me.ResumeLayout(False)
        Me.PerformLayout()
        res.ApplyResources(Me, Me.Name)
    End Sub

    Private Sub KSSettingPanel_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadSettings()
        InternationalizationManager.EnableLanguage()
        ApplyResource()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If ComboBox1.SelectedItem Is Nothing Then Exit Sub
        Dim pluginType As Type = If(PluginManager.GetAllAvailableDisplays().FirstOrDefault(Function(s) s.FullName = CType(ComboBox1.SelectedItem, ComboboxItemPluginTypePair).Fullname), PluginManager.GetAllAvailableScrapers().FirstOrDefault(Function(s) s.FullName = CType(ComboBox1.SelectedItem, ComboboxItemPluginTypePair).Fullname))
        If pluginType Is Nothing Then
            MsgBox("Plugin Not Found", MsgBoxStyle.Critical)
            Exit Sub
        End If
        Dim plugin As IKSPlugin = PluginManager.CreateInstance(pluginType)
        plugin.DisplaySetting()
    End Sub
End Class
