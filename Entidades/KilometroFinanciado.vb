Public Class KilometroFinanciado

    Public Property Organizacion As String
    Public Property Euros As Decimal
    Public Sub New(numKm As Integer, Direccion As String, localidad As String, provincia As String)
        MyBase.New(numKm, Direccion, localidad, provincia)
    End Sub
    Public Sub New(organizacion As String, euros As Decimal, numKm As Integer, Direccion As String, localidad As String, provincia As String)
        Me.New(numKm, Direccion, localidad, provincia)
        Me.Organizacion = organizacion
        Me.Euros = euros
    End Sub
End Class
