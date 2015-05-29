''' <summary>
''' A controller to sync lyric and position
''' </summary>
Public Class LyricSynchronizationController

    ''' <summary>
    ''' Create a new LyricSynchronizationController instance with LyricFile instance
    ''' </summary>
    ''' <param name="LyricFile">Lyric file</param>
    Sub New(LyricFile As LRCFile)
        Me.LRC = LyricFile
    End Sub

    Private LRC As LRCFile

    ''' <summary>
    ''' Get lyric of position
    ''' </summary>
    ''' <param name="MilesecondPosition">position</param>
    Public Function GetLyric(MilesecondPosition As Integer) As String
        Dim lyricIndex As Integer = GetLyricIndex(MilesecondPosition)
        Return If(lyricIndex <> -1, LRC.TimeLines(lyricIndex).Lyric, "")
    End Function
    ''' <summary>
    ''' Get lyrics index of position
    ''' </summary>
    ''' <param name="MilesecondPosition">position</param>
    ''' <returns></returns>
    Public Function GetLyricIndex(MilesecondPosition As Integer) As Integer
        If LRC Is Nothing Then Return ""
        For i As Integer = 0 To Me.LRC.TimeLines.Count - 1
            Dim item As LRCTimeline = Me.LRC.TimeLines(i)
            If Not i + 2 > LRC.TimeLines.Count Then 'check if the position is between two timeline
                Dim previousTimePoint, nextTimePoint As Integer
                previousTimePoint = item.StartPoint.Millisecond + item.StartPoint.Second * 1000 + item.StartPoint.Minute * 60000 + item.StartPoint.Hour * 3600000
                Dim nextItem As LRCTimeline = Me.LRC.TimeLines(i + 1)
                nextTimePoint = nextItem.StartPoint.Millisecond + nextItem.StartPoint.Second * 1000 + nextItem.StartPoint.Minute * 60000 + nextItem.StartPoint.Hour * 3600000
                If MilesecondPosition >= previousTimePoint And MilesecondPosition < nextTimePoint Then Return i
            Else 'if not,means the position is later than the last timeline
                Dim previousTimePoint As Integer
                previousTimePoint = item.StartPoint.Millisecond + item.StartPoint.Second * 1000 + item.StartPoint.Minute * 60000 + item.StartPoint.Hour * 3600000
                If MilesecondPosition >= previousTimePoint Then Return i
            End If
        Next
        Return -1
    End Function
    ''' <summary>
    ''' Get percentage of word
    ''' </summary>
    ''' <param name="SentenceIndex">Lyric sentence index</param>
    ''' <returns></returns>
    Public Function GetWordPercentage(SentenceIndex As Integer, MilesecondPosition As Integer) As WordPercentageResult
        Dim nowTimeLine = Me.LRC.TimeLines(SentenceIndex)
        Dim sentenceTimeSpan As TimeSpan = If(SentenceIndex = Me.LRC.TimeLines.Count - 1, New TimeSpan(0, 0, 10), Me.LRC.TimeLines(SentenceIndex + 1).StartPoint - Me.LRC.TimeLines(SentenceIndex).StartPoint)
        Dim wordCount As Integer = nowTimeLine.Lyric.Length
        Dim timespanPerWord As Integer = sentenceTimeSpan.TotalMilliseconds / wordCount
        Dim thisSentenceProgress As Integer = MilesecondPosition - (nowTimeLine.StartPoint.Millisecond + nowTimeLine.StartPoint.Second * 1000 + nowTimeLine.StartPoint.Minute * 60000 + nowTimeLine.StartPoint.Hour * 3600000)
        Dim wordIndex As Integer = 0
        Dim outTime As Integer = 0
        wordIndex = Math.DivRem(thisSentenceProgress, timespanPerWord, outTime)
        If outTime > 0 Then wordIndex += 1
        Return New WordPercentageResult() With {.Percentage = (outTime / timespanPerWord), .WordIndex = wordIndex}
    End Function

End Class
Public Class WordPercentageResult
    Public Property WordIndex As Integer
    Public Property Percentage As Double
End Class