Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Text.RegularExpressions
Imports MusicBeePlugin
''' <summary>
''' A scraper for Xiami Music
''' </summary>
Public Class XiamiScraper
    Implements IKSPlugin, IScraper

#Region "Private Members"

    Private Function tagRemover(originalText As String) As String
        If originalText.Contains("</b>") Then
            Dim regex As New Regex("(?<=<b class=""key_red"">)[^\<]*(?=</b>)")
            Return regex.Match(originalText).Value
        Else
            Return originalText
        End If
    End Function


#End Region

    Public Property Description As String = "Xiami Music is one of music sharing websites in China." Implements IKSPlugin.Description

    Public Property GetSetting As KSPlugin_Setting_GetDelegate Implements IKSPlugin.GetSetting

    Public Property SetSetting As KSPlugin_Setting_SetDelegate Implements IKSPlugin.SetSetting


    Public Function DownloadLyrics(Index As ScraperLyricInfo) As String Implements IScraper.DownloadLyrics
        'Request this url:http://www.xiami.com/song/playlist/id/songid
        'Regex:\<lyric_url\>\s*(.*)\s*\</lyric_url\>
        Dim r As HttpWebRequest = HttpWebRequest.Create("http://www.xiami.com/song/playlist/id/" + Index.ID)
        r.Host = "www.xiami.com"
        r.Accept = "*/*"
        Dim sr As New StreamReader(r.GetResponse().GetResponseStream())
        Dim regex As New Regex("\<lyric_url\>\s*(.*)\s*\</lyric_url\>")
        Dim url As String = regex.Match(sr.ReadToEnd).Groups(1).Value
        Dim dataBytes As Byte() = New WebClient().DownloadData(url)
        Return Encoding.UTF8.GetString(dataBytes)
    End Function

    Public Function SearchLyrics(Title As String, Artist As String) As ScraperLyricInfo() Implements IScraper.SearchLyrics
        'Request this url:www.xiami.com/search/song-lyric?key=songname
        'Regex:\<a\starget=\"_blank\"\shref=\"http://www.xiami.com/song/(\d*)\"\stitle=\"\"\>(.*)<\/a\>\s*\<\/td\>\s*\<td\sclass=\"song_artist\"\>\s*\<a\starget=\"_blank\"\shref=\"http://www.xiami.com/artist/\d*\"\stitle=\".*\"\>(.*)\</a>
        Dim r As HttpWebRequest = HttpWebRequest.Create("http://www.xiami.com/search/song-lyric?key=" + Title)
        r.Host = "www.xiami.com"
        r.Accept = "*/*"
        Dim sr As New StreamReader(r.GetResponse().GetResponseStream(), Encoding.UTF8)
        Dim regex As New Regex("\<a\starget=\""_blank\""\shref=\""http://www.xiami.com/song/(\d*)\""\stitle=\""\""\>(.*)<\/a\>\s*\<\/td\>\s*\<td\sclass=\""song_artist\""\>\s*\<a\starget=\""_blank\""\shref=\""http://www.xiami.com/artist/\d*\""\stitle=\"".*\""\>(.*)\</a>")
        Dim returnText As String = sr.ReadToEnd()
        Dim returnLyricInfo As New List(Of ScraperLyricInfo)
        For Each m As Match In regex.Matches(returnText)
            returnLyricInfo.Add(New ScraperLyricInfo() With {.Title = tagRemover(m.Groups(2).Value),
                                                                                    .Artist = tagRemover(m.Groups(3).Value),
                                                                                    .ID = tagRemover(m.Groups(1).Value),
                                                                                    .Scraper = Me})
        Next
        Return returnLyricInfo.ToArray()
    End Function
End Class
