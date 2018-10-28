Imports System.IO
Imports System.Text
Imports System.Xml.Serialization
''' <summary>
''' A setting manager in charge of setting writing and reading
''' </summary>
Friend Class SettingManager

#Region "Private Members"

    ''' <summary>
    ''' A class that will store all setting
    ''' </summary>
    Private Shared settingObject As SettingObject

#End Region

    Public Shared Property SettingStoragePath As String = ""

    ''' <summary>
    ''' Write value to a setting item of plugin
    ''' </summary>
    ''' <param name="AssemblyName">assembly name of plugin</param>
    ''' <param name="TypeName">typename of the type that calls setting saving</param>
    ''' <param name="PluginType">plugin type</param>
    ''' <param name="Key">setting key</param>
    ''' <param name="Value">setting value</param>
    Public Shared Sub PluginSetValue(AssemblyName As String, TypeName As String, PluginType As PluginType, Key As String, Value As String)
        If settingObject Is Nothing OrElse settingObject.PluginSettings Is Nothing Then Exit Sub
        Dim settingItem = (From s In settingObject.PluginSettings Where s.AssemblyName = AssemblyName And s.TypeName = TypeName And s.PluginType = PluginType And s.SettingKey = Key).FirstOrDefault()
        If settingItem IsNot Nothing Then
            settingItem.SettingValue = Value
        Else
            settingObject.PluginSettings.Add(New PluginSettingItem() With {.AssemblyName = AssemblyName,
                .PluginType = PluginType,
                .TypeName = TypeName,
                .SettingKey = Key,
                .SettingValue = Value})
        End If
    End Sub

    ''' <summary>
    ''' Read value from a setting item of plugin
    ''' </summary>
    ''' <param name="AssemblyName">assembly name of plugin</param>
    ''' <param name="TypeName">typename of the type that calls setting saving</param>
    ''' <param name="PluginType">plugin type</param>
    ''' <param name="Key">setting key</param>
    ''' <returns></returns>
    Public Shared Function PluginGetValue(AssemblyName As String, TypeName As String, PluginType As PluginType, Key As String) As String
        If settingObject Is Nothing OrElse settingObject.PluginSettings Is Nothing Then Return ""
        Dim settingItem = (From s In settingObject.PluginSettings Where s.AssemblyName = AssemblyName And s.TypeName = TypeName And s.PluginType = PluginType And s.SettingKey = Key).FirstOrDefault()
        If settingItem IsNot Nothing Then
            Return If(settingItem.SettingValue, "")
        Else
            Return Nothing
        End If
    End Function

    ''' <summary>
    ''' Write value to setting object
    ''' </summary>
    ''' <param name="Key">setting key</param>
    ''' <param name="Value">setting value</param>
    Public Shared Sub InternalSetValue(Key As String, Value As String)
        If settingObject Is Nothing OrElse settingObject.InternalSettings Is Nothing Then Exit Sub
        Dim settingItem = (From s In settingObject.InternalSettings Where s.Key = Key).FirstOrDefault()
        If settingItem IsNot Nothing Then
            settingItem.Value = Value
        Else
            settingObject.InternalSettings.Add(New InternalSettingItem() With {.Key = Key, .Value = Value})
        End If
    End Sub

    ''' <summary>
    ''' Read value from setting object
    ''' </summary>
    ''' <param name="Key">setting key</param>
    Public Shared Function InternalGetValue(Key As String) As String
        If settingObject Is Nothing OrElse settingObject.InternalSettings Is Nothing Then Return Nothing
        Dim settingItem = (From s In settingObject.InternalSettings Where s.Key = Key).FirstOrDefault()
        If settingItem IsNot Nothing Then
            Return If(settingItem.Value, "")
        Else
            Return Nothing
        End If
    End Function

    ''' <summary>
    ''' Save all settings to xml file
    ''' </summary>
    Public Shared Sub Save()
        If settingObject Is Nothing Or Directory.Exists(SettingStoragePath) = False Then Exit Sub
        Dim xms As New XmlSerializer(GetType(SettingObject))
        Dim writer As New StringWriter()
        xms.Serialize(writer, settingObject)
        File.WriteAllText(SettingStoragePath + "\KaraokeShowSettings.xml", writer.ToString(), Encoding.UTF8)
    End Sub

    ''' <summary>
    ''' Load all settings from xml file
    ''' </summary>
    Public Shared Sub Load()
        If File.Exists(SettingStoragePath + "\KaraokeShowSettings.xml") = False Then
            settingObject = New SettingObject()
        Else
            Dim xmlText As String = File.ReadAllText(SettingStoragePath + "\KaraokeShowSettings.xml", Encoding.UTF8)
            Dim sr As New StringReader(xmlText)
            Dim xms As New XmlSerializer(GetType(SettingObject))
            settingObject = xms.Deserialize(sr)
        End If
    End Sub

    ''' <summary>
    ''' To delete setting file on disk
    ''' </summary>
    Public Shared Sub Uninstall()
        If File.Exists(SettingStoragePath + "\KaraokeShowSettings.xml") Then
            File.Delete(SettingStoragePath + "\KaraokeShowSettings.xml")
        End If
    End Sub
End Class
