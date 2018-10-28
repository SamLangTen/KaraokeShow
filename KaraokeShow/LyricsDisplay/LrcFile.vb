Imports System.IO
Imports System.Text.RegularExpressions
''' <summary>
''' Represent a lrc file object
'''</summary>
Friend Class LRCFile
    Public Enum LRCFilemode
        Accurate
        Normal
    End Enum
    Sub New(LRCString As String)
        Try
            Me.LoadFileInNormalMode(LRCString)
        Catch ex As Exception
            Me.IsLRCFormat = False
        End Try
        If (Me.TimeLines.Count = 0) Then Me.IsLRCFormat = False
    End Sub

    Public Property Artist As String
    Public Property Album As String
    Public Property FileMaker As String
    Public Property Title As String
    Public Property Offset As TimeSpan
    Private _fm As LRCFilemode
    Public ReadOnly Property FileMode As LRCFilemode
        Get
            Return _fm
        End Get
    End Property
    Public Property TimeLines As New List(Of LRCTimeline)

    Public Property IsLRCFormat() As Boolean = True

    Private Sub LoadFileInNormalMode(ByVal lrcfilestr As String)
        'Regex expressions to analyze timeline And other info：\[[\d\.:\]\[]*\][^(\[)]*
        'Timeline：\[[\d\.:]*\]
        'Title：\[ti:.[^(\[)]*]
        'Artist：\[ar:.[^(\[)]*]
        'Album：\[al:.[^(\[)]*]
        'Lyrics Creator：\[by:.[^(\[)]*]
        'Offset：\[offset:.[^(\[)]*]

        'Load Lrc file
        'Dim lrcfilestr As String = File.ReadAllText(loadfile, System.Text.Encoding.Default)
        'Analyze info
        Dim FileRegex As New Regex("\[ti:.[^(\[)]*]")
        Me.Title = FileRegex.Match(lrcfilestr).Value.Replace("[ti:", "").Replace("]", "")
        FileRegex = New Regex("\[ar:.[^(\[)]*]")
        Me.Artist = FileRegex.Match(lrcfilestr).Value.Replace("[ar:", "").Replace("]", "")
        FileRegex = New Regex("\[al:.[^(\[)]*]")
        Me.Album = FileRegex.Match(lrcfilestr).Value.Replace("[al:", "").Replace("]", "")
        FileRegex = New Regex("\[by:.[^(\[)]*]")
        Me.FileMaker = FileRegex.Match(lrcfilestr).Value.Replace("[by:", "").Replace("]", "")
        FileRegex = New Regex("\[offset:.[^(\[)]*]")
        Dim offsetsrt As String = FileRegex.Match(lrcfilestr).Value.Replace("[offset:", "").Replace("]", "")
        If Not offsetsrt = "" Then
            Dim offsetint As Int64 = CLng(offsetsrt)
            Me.Offset = New TimeSpan(offsetint)
        Else
            Me.Offset = New TimeSpan(0)
        End If

        'Load timeline
        'Note：
        'Each lyric can be used by more than one timeline
        FileRegex = New Regex("\[[\d\.:\]\[]*\][^\[]*")
        Dim timelineregex As New Regex("\[[\d\.:]*\]")
        For Each item As Match In FileRegex.Matches(lrcfilestr)
            'Read lyrics
            Dim lrcword As String = If(timelineregex.Replace(item.Value, "") Is Nothing, "", timelineregex.Replace(item.Value, ""))
            'Read timeline
            For Each item2 As Match In timelineregex.Matches(item.Value)
                Dim lrctl As New LRCTimeline
                Dim startp As String = item2.Value
                startp = startp.Replace("[", "").Replace("]", "")
                Dim min As Int32 = CInt(startp.Split(":")(0))
                Dim sec As Int32 = CInt(startp.Split(":")(1).Split(".")(0))
                Dim msec As Int32 = CInt(startp.Split(":")(1).Split(".")(1))
                lrctl.StartPoint = New DateTime(1, 1, 1, 0, min, sec, msec)
                lrctl.Lyric = lrcword
                Me.TimeLines.Add(lrctl)
            Next
        Next
        'Adjust timeline order
        Me.TimeLines = From i In Me.TimeLines Order By i.StartPoint
    End Sub
End Class
''' <summary>
''' Represent a timeline of lrc file
''' </summary>
Public Structure LRCTimeline
    Public Property StartPoint As DateTime
    Public Property Lyric As String
End Structure