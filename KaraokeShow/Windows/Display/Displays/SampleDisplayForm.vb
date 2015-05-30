﻿Imports System.Windows.Forms
Imports System.Threading.Tasks
Imports System.Threading
Imports MusicBeePlugin.Plugin
Imports System.IO

Public Class SampleDisplayForm

    Public WriteOnly Property Lyric As String
        Set(value As String)
            Me.Label1.Text = value
        End Set
    End Property
    Public WriteOnly Property Percentage As Double
        Set(value As Double)
            Me.Label3.Text = (value * 100).ToString() + "%"
        End Set
    End Property
End Class