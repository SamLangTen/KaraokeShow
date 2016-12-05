Imports System.IO
''' <summary>
''' A manager which provides lyrics searching and loading functions
''' </summary>
Public Class LyricsManager

    ''' <summary>
    ''' Search from music containing folder
    ''' </summary>
    ''' <param name="Filename">music filename</param>
    ''' <param name="Title">music title</param>
    ''' <param name="Artist">music artist</param>
    ''' <returns>LRCFile instance</returns>
    Public Shared Function SearchFromContainingFolder(Filename As String, Title As String, Artist As String) As LRCFile
        'Check whether the lrc file exists
        Dim vailTitle As String = Title
        For Each item As Char In Path.GetInvalidFileNameChars
            vailTitle = vailTitle.Replace(item, "_")
        Next
        If File.Exists(New FileInfo(Filename).DirectoryName + "\" + vailTitle + ".lrc") = True Then
            Return New LRCFile(File.ReadAllText(New FileInfo(Filename).DirectoryName + "\" + vailTitle + ".lrc"))

        Else
            Return Nothing
        End If
    End Function
    ''' <summary>
    ''' Search from lyric scraper
    ''' </summary>
    ''' <param name="Title">music title</param>
    ''' <param name="Artist">music artist</param>
    ''' <returns>LRCFile instance</returns>
    Public Shared Function SearchFromScraper(Title As String, Artist As String) As LRCFile
        Dim lyricsText As String = ScraperManager.AutoSearchLyrics(Title, Artist)
        If lyricsText IsNot Nothing AndAlso lyricsText <> "" Then Return New LRCFile(lyricsText) Else Return Nothing
    End Function
End Class
