Public Interface IScraper

    Property Description As String
    Function SearchLyrics(Title As String, Artist As String) As ScraperLyricInfo()
    Function DownloadLyrics(Index As ScraperLyricInfo) As String

End Interface
Public Class ScraperLyricInfo

    Public Property Scraper As IScraper
    Public Property Index As Integer
    Public Property Title As String
    Public Property Artist As String

End Class