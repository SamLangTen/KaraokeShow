Imports System.IO
Public Class PluginManager

    ''' <summary>
    ''' To tell plugin manager where the plugins can be found
    ''' </summary>
    Public Shared Property KSPluginStorageFolder As String

    ''' <summary>
    ''' Get all available loaded assemblies
    ''' </summary>
    Public Shared Property AvailablePlugins As New List(Of PluginAssembly)

    ''' <summary>
    ''' Get all available scrapers in all loaded assemblies
    ''' </summary>
    Public Shared Function GetAllAvailableScrapers() As List(Of Type)
        If PluginManager.AvailablePlugins Is Nothing Then Return Nothing
        Dim availableScrapers As New List(Of Type)
        PluginManager.AvailablePlugins.ForEach(Sub(e)
                                                   availableScrapers.AddRange(e.AvailableScrapers)
                                               End Sub)
        Return availableScrapers
    End Function

    ''' <summary>
    ''' Get all available displays in all loaded assemblies
    ''' </summary>
    Public Shared Function GetAllAvailableDisplays() As List(Of Type)
        If PluginManager.AvailablePlugins Is Nothing Then Return Nothing
        Dim availableDisplays As New List(Of Type)
        PluginManager.AvailablePlugins.ForEach(Sub(e)
                                                   availableDisplays.AddRange(e.AvailableDisplays)
                                               End Sub)
        Return availableDisplays
    End Function

    ''' <summary>
    ''' Create instance by typename
    ''' </summary>
    ''' <param name="TypeName">Full typename</param>
    Public Shared Function CreateInstance(TypeName As Type) As Object
        Return Activator.CreateInstance(TypeName)
    End Function

    ''' <summary>
    ''' Load KS plugin storaged in StorageFolder(default in MBPluginFolder\KaraokeShowPlugins\)
    ''' </summary>
    Public Shared Sub InitializePluginInStorageFolder()
        'Get all ksplg files in folder
        If Directory.Exists(KSPluginStorageFolder) = False Then Exit Sub
        PluginManager.AvailablePlugins = (From f In Directory.GetFiles(KSPluginStorageFolder, "*.ksplg") Select New PluginAssembly(f))
        PluginManager.AvailablePlugins.ForEach(Sub(e)
                                                   e.Load()
                                               End Sub)
    End Sub
End Class
