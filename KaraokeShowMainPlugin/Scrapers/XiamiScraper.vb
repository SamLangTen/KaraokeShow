Imports System.Net
Imports MusicBeePlugin
''' <summary>
''' A scraper for Xiami Music
''' </summary>
Public Class XiamiScraper
    Implements IKSPlugin, IScraper

    Public Property Description As String = "Xiami Music is one of music sharing websites in China." Implements IKSPlugin.Description

    Public Property GetSetting As KSPlugin_Setting_GetDelegate Implements IKSPlugin.GetSetting

    Public Property SetSetting As KSPlugin_Setting_SetDelegate Implements IKSPlugin.SetSetting


    Public Function DownloadLyrics(Index As ScraperLyricInfo) As String Implements IScraper.DownloadLyrics
        Throw New NotImplementedException()
    End Function

    Public Function SearchLyrics(Title As String, Artist As String) As ScraperLyricInfo() Implements IScraper.SearchLyrics
        Throw New NotImplementedException()
    End Function
End Class
