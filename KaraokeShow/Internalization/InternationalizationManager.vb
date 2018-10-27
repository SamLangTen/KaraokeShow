Public Class InternationalizationManager
    Public Shared MB_GetLocalizationAPI As Func(Of String, String, String)
    Public Shared Function GetCurrentMusicBeeLanguage() As String
        'Initialize langDict that convert MusicBee setting to .Net Culture Text
        Dim langDict As New Dictionary(Of String, String)
        langDict("Language") = "en"
        langDict("语言") = "zh-Hans"
        langDict("言語") = "ja"
        'Get culture text
        Dim mbLang = MB_GetLocalizationAPI?.Invoke("Main.field.173", "Language")
        Dim cultureText = If(langDict.ContainsKey(mbLang), langDict(mbLang), "en")
        Return cultureText
    End Function
    Public Shared Sub EnableLanguage()
        Dim cultureText = InternationalizationManager.GetCurrentMusicBeeLanguage()
        Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(cultureText)
    End Sub
End Class
