Imports System.Reflection
''' <summary>
''' Represent an assembly of plugin
''' </summary>
Public Class PluginAssembly

    Private plgAssmbly As Assembly
    Private assemblyFilename As String

    ''' <summary>
    ''' Create a PluginAssembly instance by AssemblyFilename
    ''' </summary>
    ''' <param name="AssemblyFilename">Filename of assembly</param>
    Sub New(AssemblyFilename As String)
        Me.assemblyFilename = AssemblyFilename
    End Sub

    ''' <summary>
    ''' Load assembly to KS
    ''' </summary>
    Public Sub Load()
        plgAssmbly = Assembly.LoadFrom(assemblyFilename)
        'Get all scrapers (the class that implements IScraper)
        Me.AvailableScrapers = (From t In plgAssmbly.GetTypes() Where t.GetInterfaces.Contains(GetType(IScraper)) And t.GetInterfaces().Contains(GetType(IKSPlugin))).ToList()
        'Get all displays (the class that implements IDisplay)
        Me.AvailableDisplays = (From t In plgAssmbly.GetTypes() Where t.GetInterfaces.Contains(GetType(IDisplay)) And t.GetInterfaces().Contains(GetType(IKSPlugin))).ToList()
    End Sub

    ''' <summary>
    ''' Get wheather the assembly has been loaded to this appdomain
    ''' </summary>
    Public ReadOnly Property IsAssemblyLoaded As Boolean
        Get
            Return (plgAssmbly IsNot Nothing) OrElse (AppDomain.CurrentDomain.GetAssemblies().Contains(plgAssmbly))
        End Get
    End Property


    ''' <summary>
    ''' Contain all available scrapers in this plugin.
    ''' </summary>
    Public Property AvailableScrapers As New List(Of Type)

    ''' <summary>
    ''' Contain all available displays in this plugin.
    ''' </summary>
    Public Property AvailableDisplays As New List(Of Type)
End Class
