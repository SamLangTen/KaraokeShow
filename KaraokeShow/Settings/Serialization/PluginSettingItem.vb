''' <summary>
''' Represent a setting item of KaraokeShow Plugin
''' </summary>
Friend Class PluginSettingItem
    Public Property AssemblyName As String
    Public Property TypeName As String
    Public Property PluginType As PluginType
    Public Property SettingKey As String
    Public Property SettingValue As String
End Class
Friend Enum PluginType
    Scraper
    Display
End Enum
