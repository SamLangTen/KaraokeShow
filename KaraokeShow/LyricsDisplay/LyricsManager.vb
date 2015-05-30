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
        If File.Exists(New FileInfo(Filename).DirectoryName + "\" + Title + ".lrc") = True Then
            Return New LRCFile(File.ReadAllText(New FileInfo(Filename).DirectoryName + "\" + Title + ".lrc", Text.Encoding.Default))
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
        Return Nothing
    End Function
End Class
