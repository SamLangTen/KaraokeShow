Imports System.Xml
Imports System.Xml.Serialization
Imports System.Xml.Linq

''' <summary>
''' Represents main setting file of settings
''' </summary>
Public Class SettingObject
    Public Property InternalSettings As New List(Of InternalSettingItem)
    Public Property PluginSettings As New List(Of PluginSettingItem)
End Class


