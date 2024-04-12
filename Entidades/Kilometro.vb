Public Class Kilometro
    Public Property NumKm As Integer
    Public Property Direccion As String
    Public Property Localidad As String
    Public Property Provincia As String

    Public Sub New(numKm As Integer)
        Me.NumKm = numKm
        Direccion = ""
        Localidad = ""
        Provincia = ""
    End Sub

    Public Sub New(numKm As Integer, direccion As String, localidad As String, provincia As String)
        Me.New(numKm)
        Me.Direccion = direccion
        Me.Localidad = localidad
        Me.Provincia = provincia
    End Sub
    Public Overrides Function ToString() As String
        Dim toStr = $"{NumKm}"
        If Not String.IsNullOrWhiteSpace( Then
            Return MyBase.ToString()
    End Function
End Class
