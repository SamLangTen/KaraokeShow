Imports System.ComponentModel
Imports System.Globalization
Imports System.IO
Imports System.Resources

Public Class EmbedResourceManager
    Inherits ComponentResourceManager

    Private _contextTypeInfo As Type
    Private _neutralResourcesCulture As CultureInfo

    Sub New(t As Type)
        MyBase.New(t)
        _contextTypeInfo = t
    End Sub

    Protected Overrides Function InternalGetResourceSet(culture As CultureInfo, createIfNotExists As Boolean, tryParents As Boolean) As ResourceSet
        Dim rs As ResourceSet = CType(Me.ResourceSets(culture), ResourceSet)
        If rs Is Nothing Then
            Dim store As Stream = Nothing
            Dim resourceFilename As String = Nothing
            If Me._neutralResourcesCulture Is Nothing Then
                Me._neutralResourcesCulture = GetNeutralResourcesLanguage(Me.MainAssembly)
            End If
            If _neutralResourcesCulture.Equals(culture) Then culture = CultureInfo.InvariantCulture
            resourceFilename = GetResourceFileName(culture)
            store = Me.MainAssembly.GetManifestResourceStream(Me._contextTypeInfo, resourceFilename)
            If store IsNot Nothing Then
                rs = New ResourceSet(store)
                AddResourceSet(Me.ResourceSets, culture, rs)
            Else
                rs = MyBase.InternalGetResourceSet(culture, createIfNotExists, tryParents)
            End If
        End If
        Return rs
    End Function
    Private Shared Sub AddResourceSet(localResourcesSets As Hashtable, culture As CultureInfo, ByRef rs As ResourceSet)
        SyncLock localResourcesSets
            Dim objA As ResourceSet = CType(localResourcesSets(culture), ResourceSet)
            If objA IsNot Nothing Then
                If Not Object.Equals(objA, rs) Then
                    rs.Dispose()
                    rs = objA
                End If
            Else
                localResourcesSets.Add(culture, rs)
            End If
        End SyncLock
    End Sub
End Class
