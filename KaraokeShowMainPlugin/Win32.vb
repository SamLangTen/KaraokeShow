Imports System.Runtime.InteropServices
Module Win32

    <StructLayout(LayoutKind.Sequential)>
    Public Structure Point
        Public x As Int32
        Public y As Int32
        Sub New(x As Int32, y As Int32)
            Me.x = x
            Me.y = y
        End Sub
    End Structure
    <StructLayout(LayoutKind.Sequential)>
    Public Structure Size
        Public cx As Int32
        Public cy As Int32
        Sub New(cx As Int32, cy As Int32)
            Me.cx = cx
            Me.cy = cy
        End Sub
    End Structure

    <StructLayout(LayoutKind.Sequential, Pack:=1)>
    Public Structure ARGB
        Public Blue As Byte
        Public Green As Byte
        Public Red As Byte
        Public Alpha As Byte
    End Structure

    <StructLayout(LayoutKind.Sequential, Pack:=1)>
    Public Structure BLENDFUNCTION
        Public BlendOp As Byte
        Public BlendFlags As Byte
        Public SourceConstantAlpha As Byte
        Public AlphaFormat As Byte
    End Structure

    Public Const ULW_COLORKEY As Int32 = &H1
    Public Const ULW_ALPHA As Int32 = &H2
    Public Const ULW_OPAQUE As Int32 = &H4
    Public Const AC_SRC_OVER As Byte = &H0
    Public Const AC_SRC_ALPHA As Byte = &H1

    <DllImport("user32.dll", EntryPoint:="SetWindowLong")>
    Public Function SetWindowLong(hWnd As IntPtr, nIndex As Int32, dwNewLong As Int64) As Int64
    End Function

    <DllImport("user32.dll")>
    Public Function UpdateLayeredWindow(hWnd As IntPtr, hdcDst As IntPtr, ByRef pptDst As Win32.Point, ByRef psize As Win32.Size, hdcSrc As IntPtr, ByRef pptSrc As Win32.Point, crKey As Int32, ByRef pblend As BLENDFUNCTION, dwFlags As Int32) As Boolean
    End Function

    <DllImport("user32.dll")>
    Public Function GetDC(hWnd As IntPtr) As IntPtr
    End Function

    <DllImport("user32.dll")>
    Public Function ReleaseDC(hWnd As IntPtr, hDC As IntPtr) As Int32
    End Function

    <DllImport("user32.dll")>
    Public Function CreateCompatitbleDC(hDC As IntPtr) As IntPtr
    End Function
End Module
