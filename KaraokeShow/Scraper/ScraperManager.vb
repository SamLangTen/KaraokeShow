''' <summary>
''' A manager of scrapers
''' </summary>
Public Class ScraperManager

    ''' <summary>
    ''' Contains all available scrapers creating by scraper types that loading from plugin manager.
    ''' </summary>
    Private Shared availableScraperInstance As New List(Of IScraper)

    ''' <summary>
    ''' Initialize all scrapers from plugin manager
    ''' </summary>
    Public Shared Sub LoadScrapers()
        availableScraperInstance.Clear()
        PluginManager.GetAllAvailableScrapers().ForEach(Sub(t)
                                                            availableScraperInstance.Add(PluginManager.CreateInstance(t))
                                                        End Sub)
    End Sub

    ''' <summary>
    ''' Search and download lyric file automatically
    ''' </summary>
    ''' <param name="Title">music title</param>
    ''' <param name="Artist">music artist</param>
    ''' <returns>Text with lrc file structure</returns>
    Public Shared Function AutoSearchLyrics(Title As String, Artist As String) As String
        Dim lyricText As String = ""
        availableScraperInstance.ForEach(Sub(s)
                                             Dim lyrics = s.SearchLyrics(Title, Artist)
                                             If lyrics.Length > 0 Then
                                                 lyricText = s.DownloadLyrics(lyrics(0))
                                                 Exit Sub
                                             End If
                                         End Sub)
        Return lyricText
    End Function

    ''' <summary>
    ''' Search lyric file from a scraper
    ''' </summary>
    ''' <param name="Scraper">scraper to be used</param>
    ''' <param name="Title">music title</param>
    ''' <param name="Artist">music artist</param>
    Public Shared Function SearchLyricsFrom(Scraper As IScraper, Title As String, Artist As String) As ScraperLyricInfo()
        Return Scraper.SearchLyrics(Title, Artist)
    End Function

    Public Shared Function SearchLyricsFrom(ScraperName As String, Title As String, Artist As String) As ScraperLyricInfo()
        Dim scraper = availableScraperInstance.FirstOrDefault(Function(r) r.GetType().Name = ScraperName)
        Return SearchLyricsFrom(scraper, Title, Artist)
    End Function

    ''' <summary>
    ''' Download lyric file from a scraper
    ''' </summary>
    ''' <param name="Scraper">scraper to be used</param>
    ''' <param name="DownloadInfo">download info of scraper</param>
    Public Shared Function DownloadLyricsFrom(Scraper As IScraper, DownloadInfo As ScraperLyricInfo) As String
        Return Scraper.DownloadLyrics(DownloadInfo)
    End Function

End Class
