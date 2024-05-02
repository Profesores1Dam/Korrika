Imports System.Diagnostics.Eventing.Reader
Imports System.IO
Imports Entidades
' todo Commits: No hay de Ivan y no son significativos

Public Class Korrika
' TODO: LA PRIMERA PARTE COMPARTIDA POR VARIOS GRUPOS EN CLASE, CON ERRORES 
    Public Property datosGenerales As DatosGeneralesKorrika ' todo Si es una propiedad debe comenzar en Mayúsculas
    Private Property _Provincias As New List(Of String) From {"araba", "gipuzkoa", "nafarroa", "bizkaia", "zuberoa", "nafarra behera", "lapurdi"}
    Public ReadOnly Property Provincias ' todo Las propiedades deben tener Tipo Y en vuestro caso no los tienen
        Get
            Return _Provincias.AsReadOnly
        End Get
    End Property
    Private Property _Kilometros As New List(Of Kilometro) ' todo Esto no puede ser propiedad
    Public ReadOnly Property Kilometros
        Get
            Return _Kilometros.AsReadOnly
        End Get
    End Property
    Private Property _TotalRecaudado As Decimal
    Public ReadOnly Property TotalRecaudado ' todo Hay que hacer aquí el cálculo
        Get
            Return _TotalRecaudado
        End Get
    End Property

    Private Sub TotalRecaudadoCalculo(euros As Decimal) ' todo Esto no funciona, ya que el _TotalRecaudado hay que calcularlo en función de todos los kms financiados.ESTABA CORREGIDO
        _TotalRecaudado += euros
    End Sub
    Public Sub New(nKorrika As Byte, anyo As Integer, eslogan As String, fechaInicio As Date, fechaFin As Date, cantKms As Integer)
        Me.New(New DatosGeneralesKorrika(nKorrika, anyo, eslogan, fechaInicio, fechaFin, cantKms))
    End Sub

    Public Sub New(datosGenerales As DatosGeneralesKorrika) ' todo Este no debía existir
        Me.datosGenerales = datosGenerales
        ' TODO ¡¡¡Tenía que crear los kms!!!
    End Sub

    Public Sub New(nKorrika As Byte) ' todo ¿Control de errores y devolución en parámetro por referencia?
        LeerFichero(nKorrika)
    End Sub

    Public Sub New(datosG As DatosGeneralesKorrika, ByRef msgError As String)
        ' todo Y si recibe Nothing?
        Dim numKorrika As Byte = datosG.NKorrika
        Dim fichero As String = $"Korrika{numKorrika}.txt"
        If File.Exists(fichero) Then
            msgError = $"Ya existe el fichero {fichero}"
        End If
        ' todo ¡¡¡Si existe no debe hacer esto!!!
        CrearKilometros(datosG.CantKms)
        ' todo El CambioGuardar no hace lo que debía
        CambiosGuardar(datosG.NKorrika, datosG.CantKms, datosG.Eslogan, datosG.Anyo, datosG.FechaFin, datosG.FechaInicio)
    End Sub
    Private Sub CrearKilometros(cantKm) ' todo cantKm no tiene declaración de tipo
        For i = 1 To cantKm
            _Kilometros.Add(New Kilometro(i))
        Next
    End Sub
    Public Overrides Function ToString() As String
        Return datosGenerales.ToString
    End Function

    Public Function DefinirKm(numKm As Integer, direccion As String, localidad As String, provincia As String) As String
        Dim msg As String = DefinirKm(New Kilometro(numKm, direccion, localidad, provincia))
        Return msg
    End Function
    Public Function DefinirKm(kilometro As Kilometro) As String
        If kilometro Is Nothing Then
            Return "El kilometro no existe"
        End If
        If String.IsNullOrWhiteSpace(kilometro.Direccion) Then
            Return "La dirección no puede quedar vacia"
        End If
        If String.IsNullOrWhiteSpace(kilometro.Localidad) Then
            Return "La localidad no puede quedar vacia"
        End If
        If String.IsNullOrWhiteSpace(kilometro.Provincia) Then
            Return "La provincia no puede quedar vacia"
        End If
        If Not _Provincias.Contains(kilometro.Provincia.ToLower) Then
            Return $"No existe la provincia {kilometro.Provincia}"
        End If
        Dim posKm As Integer = _Kilometros.IndexOf(kilometro)
        If posKm = -1 Then
            Return $"No existe el kilometro {kilometro.NumKm}"
        End If
        For Each km In _Kilometros
            If kilometro.Direccion = km.Direccion AndAlso kilometro.Localidad = km.Localidad Then
                Return $"El kilómetro número {km.NumKm} ya c comienza en la dirección {km.Direccion} de {km.Provincia}"
            End If
        Next
        _Kilometros(posKm) = kilometro
        Return ""
    End Function

    Public Function PatrocinarKilometro(numKm As Integer, organizacion As String, euros As Decimal) As String
        If euros <= 0 Then
            Return "Para patrocinar el kilometro tienes que aportar dinero"
        End If
        Dim posKm As Integer = _Kilometros.IndexOf(New Kilometro(numKm))
        If posKm = -1 Then
            Return $"No existe el kilometro {numKm}"
        End If
        Dim kmAux As KilometroFinanciado = TryCast(_Kilometros(posKm), KilometroFinanciado)
        If kmAux IsNot Nothing Then
            Return $"El kilómetro número {numKm} ya está financiado por { kmAux.Organizacion}"
        End If
        If String.IsNullOrWhiteSpace(organizacion) Then
            Return $"Tiene que haber una organizacion patrocinadora"
        End If
        Dim organizacionYaEstaba As Boolean = False
        Dim kmFinanciadosOrg As Integer
        For Each km In _Kilometros
            Dim kmKmFinanciado As KilometroFinanciado = TryCast(km, KilometroFinanciado)
            If kmKmFinanciado IsNot Nothing Then
                If kmKmFinanciado.Organizacion.ToLower = organizacion.ToLower Then
                    organizacionYaEstaba = True
                    kmFinanciadosOrg += 1
                End If
            End If
        Next
        _Kilometros(posKm) = New KilometroFinanciado(_Kilometros(posKm), organizacion, euros)
        TotalRecaudadoCalculo(euros)
        If organizacionYaEstaba Then
            Return $"La organización {organizacion} financia el kilómetro {numKm}, aunque ya había financiado otros {kmFinanciadosOrg} kilómetros"
        End If
        Return $"La organización {organizacion} financia el kilómetro {numKm}"
    End Function
    Public Function KilometrosLibreProvincia(provincia As String) As List(Of Kilometro)
        If Not _Provincias.Contains(provincia) Then
            Return Nothing
        End If
        Dim kmLibres As New List(Of Kilometro)
        For Each km In _Kilometros
            If km.Provincia.ToLower = provincia.ToLower Then
                If TypeOf km IsNot KilometroFinanciado Then
                    kmLibres.Add(km)
                End If
            End If
        Next
        Return kmLibres
    End Function

    Private Function LeerFichero(numKorrika As Integer) As String
        Dim rutaFichero As String = $"./Ficheros/Korrika{numKorrika}.txt"
        Dim existeFichero As Boolean = File.Exists(rutaFichero)
        If Not existeFichero Then Return $"El fichero {rutaFichero} no existe"
        Dim lineas() As String = File.ReadAllLines(rutaFichero)

        Dim datosG As String() = lineas(0).Split("*")
        Dim newGestion As New DatosGeneralesKorrika(datosG(0), datosG(1), datosG(2), datosG(3), datosG(4), datosG(5))
        ' todo Debería haber 3 tipos de línea, con el km, con datos que indican por donde va y con más si está financiado
        For i = 1 To lineas.Length - 1
            Dim datos As String() = lineas(i).Split("*")
            Dim Km As Kilometro = New Kilometro(datos(0), datos(1), datos(2), datos(3))
            If datos.Count = 5 Then
                Dim kmFinanciados As KilometroFinanciado = New KilometroFinanciado(Km, datos(3), datos(4)) ' todo Nombre ilógico si es solo 1
                _Kilometros.Add(kmFinanciados)
            Else
                _Kilometros.Add(Km)
            End If
        Next
        ' todo ¿Qué devuelve?
    End Function
    Public Sub CambiosGuardar(NKorrika As Byte, CantKms As Integer, Eslogan As String, Anyo As Integer, FechaFin As Date, FechaInicio As Date) ' NO tiene sentido este método. Necesitábamos uno para guardar TODA la información de la Korrika, datos generales en 1ª línea y una línea por kilómetro
        Dim ruta As String = $"./Ficheros/Korrika{NKorrika}.txt"
        Dim rutaExiste As Boolean = File.Exists(ruta) ' todo ¿Para qué?
        Dim Informacion As String = $"{NKorrika}{CantKms}{Eslogan}{Anyo}{FechaFin}*{FechaInicio}" ' todo ¿Cómo? esto es 1 String y WriteAllLInes necesita un array.
        ' Las variables nunca comienzan en minúsculas
        File.WriteAllLines(ruta, Informacion) ' todo El 2º parámetro no puede ser un String sino un array de string (o IEnumerable)
    End Sub
End Class

