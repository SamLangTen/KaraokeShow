Imports System.Xml.Linq
Imports System.Xml
''' <summary>
''' A setting manager in charge of setting writing and reading
''' </summary>
Public Class SettingManager

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
        Return True
    End Function


End Class
