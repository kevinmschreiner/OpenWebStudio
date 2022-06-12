'----------------------------------------'
'-------UNUSED FOR THE DNN WRAPPER-------'
'----------------------------------------'
Imports System.Xml
Imports System.ComponentModel

Namespace DataAccess.Factories
    '<Browsable(False), EditorBrowsable(EditorBrowsableState.Never)> _
    Public Class ProviderFactory
        Inherits System.Configuration.Provider.ProviderBase

        Private mAttributes As New Hashtable

        Public ReadOnly Property Attirubtes() As Hashtable
            Get
                Return mAttributes
            End Get
        End Property

        Public ReadOnly Property Type() As String
            Get
                If Me.Attirubtes.ContainsKey("type") Then
                    Return CStr(Me.Attirubtes.Item("type"))
                Else
                    Return ""
                End If
            End Get
        End Property

        Public Sub New(ByVal Attributes As System.Xml.XmlAttributeCollection)
            MyBase.New()

            For Each xa As XmlAttribute In Attributes
                If Not mAttributes.ContainsKey(xa.Name) Then mAttributes.Add(xa.Name, xa.Value)
            Next
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Creates an object
        ''' </summary>
        ''' <param name="ObjectProviderType">The type of Object to create (data/navigation)</param>
        ''' <param name="ObjectProviderName">The name of the Provider</param>
        ''' <param name="ObjectNamespace">The namespace of the object to create.</param>
        ''' <param name="ObjectAssemblyName">The assembly of the object to create.</param>
        ''' <param name="UseCache">Caching switch</param>
        ''' <returns>The created Object</returns>
        ''' <remarks>Overload for creating an object from a Provider including NameSpace, 
        ''' AssemblyName and ProviderName</remarks>
        ''' <history>
        ''' 	[cnurse]	    10/13/2005	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function CreateObject(ByVal ObjectProviderType As String, ByVal ObjectProviderName As String, ByVal ObjectNamespace As String, ByVal ObjectAssemblyName As String, ByVal UseCache As Boolean, ByVal ForEmailReading As Boolean) As Object

            Dim TypeName As String = ""
            Dim sDefProvider As String

            ' get the provider configuration based on the type
            Dim objProviderConfiguration As ProviderConfiguration = ProviderConfiguration.GetProviderConfiguration(ObjectProviderType)

            If ForEmailReading Then
                sDefProvider = objProviderConfiguration.ReadProvider
            Else
                sDefProvider = objProviderConfiguration.SendProvider
            End If
            Dim p As ProviderFactory = CType(objProviderConfiguration.Providers(sDefProvider), ProviderFactory)

            If p.Type = "" Then
                ' if both the Namespace and AssemblyName are provided then we will construct an "assembly qualified typename" - ie. "NameSpace.ClassName, AssemblyName" 
                If ObjectNamespace <> "" AndAlso ObjectAssemblyName <> "" Then
                    If ObjectProviderName = "" Then
                        ' dynamically create the typename from the constants ( this enables private assemblies to share the same configuration as the base provider ) 
                        TypeName = ObjectNamespace & "." & sDefProvider & ", " & ObjectAssemblyName & "." & sDefProvider
                    Else
                        ' dynamically create the typename from the constants ( this enables private assemblies to share the same configuration as the base provider ) 
                        TypeName = ObjectNamespace & "." & ObjectProviderName & ", " & ObjectAssemblyName & "." & ObjectProviderName
                    End If
                Else
                    ' if only the Namespace is provided then we will construct an "full typename" - ie. "NameSpace.ClassName" 
                    If ObjectNamespace <> "" Then
                        If ObjectProviderName = "" Then
                            ' dynamically create the typename from the constants ( this enables private assemblies to share the same configuration as the base provider ) 
                            TypeName = ObjectNamespace & "." & sDefProvider
                        Else
                            ' dynamically create the typename from the constants ( this enables private assemblies to share the same configuration as the base provider ) 
                            TypeName = ObjectNamespace & "." & ObjectProviderName
                        End If
                    Else
                        '' if neither Namespace or AssemblyName are provided then we will get the typename from the default provider 
                        'If ObjectProviderName = "" Then
                        '    ' get the typename of the default Provider from web.config
                        '    TypeName = CType(objProviderConfiguration.Providers(objProviderConfiguration.DefaultProvider), Provider).Type
                        'Else
                        '    ' get the typename of the specified ProviderName from web.config 
                        '    TypeName = CType(objProviderConfiguration.Providers(ObjectProviderName), Provider).Type
                        'End If
                    End If
                End If

                Return CreateObject(TypeName, TypeName, UseCache)
            Else
                Return CreateObject(p.Type, p.Type, UseCache)
            End If
        End Function

        Private Shared ReadOnly Property DataCache() As DotNetNuke.Services.Cache.CachingProvider
            Get
                Return DotNetNuke.Services.Cache.CachingProvider.Instance()
            End Get
        End Property
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Creates an object
        ''' </summary>
        ''' <param name="TypeName">The fully qualified TypeName</param>
        ''' <param name="CacheKey">The Cache Key</param>
        ''' <param name="UseCache">Caching switch</param>
        ''' <returns>The created Object</returns>
        ''' <remarks>Overload that takes a fully-qualified typename and a Cache Key</remarks>
        ''' <history>
        ''' 	[cnurse]	    10/13/2005	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function CreateObject(ByVal TypeName As String, ByVal CacheKey As String, ByVal UseCache As Boolean) As Object

            If CacheKey = "" Then
                CacheKey = TypeName
            End If

            Dim objType As Type = Nothing

            ' use the cache for performance
            If UseCache Then
                objType = CType(DataCache.GetItem(CacheKey), Type)
            End If

            ' is the type in the cache?
            If objType Is Nothing Then
                Try
                    ' use reflection to get the type of the class
                    objType = System.Web.Compilation.BuildManager.GetType(TypeName, True, True)

                    If UseCache Then
                        ' insert the type into the cache
                        'DNN9: DataCache.Insert(CacheKey, objType, Nothing, Date.MaxValue, Nothing, System.Web.Caching.CacheItemPriority.Normal, Nothing)
                        DataCache.Insert(CacheKey, objType)
                    End If
                Catch exc As Exception

                End Try
            End If

            Try
                ' dynamically create the object
                Dim obj As Object = Activator.CreateInstance(objType)

                Return obj
            Catch ex As Exception
                Debug.WriteLine(ex.ToString())
                Return Nothing
            End Try

        End Function

    End Class

    <Browsable(False), EditorBrowsable(EditorBrowsableState.Never)> _
    Friend Class ProviderConfiguration
        Inherits System.Configuration.ConfigurationSection

        Private _Providers As New Hashtable
        Private _ReadProvider As String
        Private _SendProvider As String
        Private Const _SectionLocation As String = "dotnetnuke/"

        Public Overloads Shared Function GetProviderConfiguration(ByVal strProvider As String) As ProviderConfiguration
            'Return CType(DotNetNuke.Common.Utilities.Config.GetSection(_SectionLocation & strProvider), ProviderConfiguration)
            'Return CType(System.Configuration.ConfigurationManager.GetSection(_SectionLocation & strProvider), ProviderConfiguration)
            Dim obj As ProviderConfiguration
            obj = CType(System.Configuration.ConfigurationManager.GetSection(_SectionLocation & strProvider), ProviderConfiguration)
            If obj Is Nothing Then
                Dim test As String
                test = ""
            End If
            Return obj
        End Function

        Friend Sub LoadValuesFromConfigurationXml(ByVal node As XmlNode)
            Dim attributeCollection As XmlAttributeCollection = node.Attributes

            ' Get the default provider
            _ReadProvider = attributeCollection("readProvider").Value
            _SendProvider = attributeCollection("sendProvider").Value

            ' Read child nodes
            Dim child As XmlNode
            For Each child In node.ChildNodes
                If child.Name = "providers" Then
                    GetProviders(child)
                End If
            Next child
        End Sub

        Friend Sub GetProviders(ByVal node As XmlNode)

            Dim Provider As XmlNode
            For Each Provider In node.ChildNodes

                Select Case Provider.Name
                    Case "add"
                        Providers.Add(Provider.Attributes("name").Value, New ProviderFactory(Provider.Attributes))

                    Case "remove"
                        Providers.Remove(Provider.Attributes("name").Value)

                    Case "clear"
                        Providers.Clear()
                End Select
            Next Provider
        End Sub

        Public Overloads ReadOnly Property ReadProvider() As String
            Get
                Return _ReadProvider
            End Get
        End Property
        Public Overloads ReadOnly Property SendProvider() As String
            Get
                Return _SendProvider
            End Get
        End Property

        Public Overloads ReadOnly Property Providers() As Hashtable
            Get
                Return _Providers
            End Get
        End Property

    End Class

    <Browsable(False), EditorBrowsable(EditorBrowsableState.Never)> _
    Friend Class ProviderConfigurationHandler
        Implements System.Configuration.IConfigurationSectionHandler

        Public Overridable Overloads Function Create(ByVal parent As Object, ByVal context As Object, ByVal node As System.Xml.XmlNode) As Object Implements System.Configuration.IConfigurationSectionHandler.Create
            Dim objProviderConfiguration As New ProviderConfiguration
            objProviderConfiguration.LoadValuesFromConfigurationXml(node)
            Return objProviderConfiguration
        End Function
    End Class
End Namespace