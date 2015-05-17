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
        If LRC Is Nothing Then Return ""
        For i As Integer = 0 To Me.LRC.TimeLines.Count - 1
            Dim item As LRCTimeline = Me.LRC.TimeLines(i)
            If Not i + 2 > LRC.TimeLines.Count Then 'check if the position is between two timeline
                Dim previousTimePoint, nextTimePoint As Integer
                previousTimePoint = item.StartPoint.Millisecond + item.StartPoint.Second * 1000 + item.StartPoint.Minute * 60000 + item.StartPoint.Hour * 3600000
                Dim nextItem As LRCTimeline = Me.LRC.TimeLines(i + 1)
                nextTimePoint = nextItem.StartPoint.Millisecond + nextItem.StartPoint.Second * 1000 + nextItem.StartPoint.Minute * 60000 + nextItem.StartPoint.Hour * 3600000
                If MilesecondPosition >= previousTimePoint And MilesecondPosition < nextTimePoint Then Return item.Lyric
            Else 'if not,means the position is later than the last timeline
                Dim previousTimePoint As Integer
                previousTimePoint = item.StartPoint.Millisecond + item.StartPoint.Second * 1000 + item.StartPoint.Minute * 60000 + item.StartPoint.Hour * 3600000
                If MilesecondPosition >= previousTimePoint Then Return item.Lyric
            End If
        Next
        Return ""
    End Function

End Class
