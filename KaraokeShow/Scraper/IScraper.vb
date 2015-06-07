''' <summary>
''' The abtract of a scraper
''' </summary>
Public Interface IScraper

    Function SearchLyrics(Title As String, Artist As String) As ScraperLyricInfo()
    Function DownloadLyrics(Index As ScraperLyricInfo) As String

End Interface
Public Class ScraperLyricInfo

    Public Property Scraper As IScraper
    Public Property ID As String
    Public Property Title As String
    Public Property Artist As String

End Class