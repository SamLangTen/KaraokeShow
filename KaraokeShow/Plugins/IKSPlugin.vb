''' <summary>
''' The abtract of A KaraokeShow Plugin
''' </summary>
Public Interface IKSPlugin

    Property Description As String
    Property GetSetting As KSPlugin_Setting_GetDelegate
    Property SetSetting As KSPlugin_Setting_SetDelegate
    Sub DisplaySetting()
    Sub OnLoaded()
End Interface
Public Delegate Sub KSPlugin_Setting_SetDelegate(Caller As Object, Key As String, Value As String)
Public Delegate Sub KSPlugin_Setting_GetDelegate(Caller As Object, Key As String, ByRef Value As String)
