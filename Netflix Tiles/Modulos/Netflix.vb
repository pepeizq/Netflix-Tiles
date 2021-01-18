Imports Microsoft.Toolkit.Uwp.UI.Controls
Imports Newtonsoft.Json
Imports Windows.UI.Xaml.Media.Animation

Module Netflix

    'https://unogs.com/title/80018294
    'https://unogs.com/api/title/detail?netflixid=80018294
    'https://unogs.com/api/title/bgimages?netflixid=80018294

    'netflix:/app?playVideoId=80002566

    'browseVideoId
    'playVideoId
    'searchTerm

    Public Sub BotonTile_Click(boton As Button)

        Trial.Detectar()
        Interfaz.AñadirTile.ResetearValores()

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim juego As Tile = boton.Tag

        Dim botonAñadirTile As Button = pagina.FindName("botonAñadirTile")
        botonAñadirTile.Tag = juego

        Dim imagenJuegoSeleccionado As ImageEx = pagina.FindName("imagenJuegoSeleccionado")
        imagenJuegoSeleccionado.Source = juego.ImagenAncha

        Dim tbJuegoSeleccionado As TextBlock = pagina.FindName("tbJuegoSeleccionado")
        tbJuegoSeleccionado.Text = juego.Titulo

        Dim gridAñadirTile As Grid = pagina.FindName("gridAñadirTile")
        Interfaz.Pestañas.Visibilidad(gridAñadirTile, juego.Titulo, Nothing)

        '---------------------------------------------

        ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("animacionJuego", boton)
        Dim animacion As ConnectedAnimation = ConnectedAnimationService.GetForCurrentView().GetAnimation("animacionJuego")

        If Not animacion Is Nothing Then
            animacion.Configuration = New BasicConnectedAnimationConfiguration
            animacion.TryStart(gridAñadirTile)
        End If

        '---------------------------------------------

        Dim tbImagenTituloTextoTileAncha As TextBox = pagina.FindName("tbImagenTituloTextoTileAncha")
        tbImagenTituloTextoTileAncha.Text = juego.Titulo
        tbImagenTituloTextoTileAncha.Tag = juego.Titulo

        Dim tbImagenTituloTextoTileGrande As TextBox = pagina.FindName("tbImagenTituloTextoTileGrande")
        tbImagenTituloTextoTileGrande.Text = juego.Titulo
        tbImagenTituloTextoTileGrande.Tag = juego.Titulo

        '---------------------------------------------

        Dim imagenPequeña As ImageEx = pagina.FindName("imagenTilePequeña")
        imagenPequeña.Source = Nothing

        Dim imagenMediana As ImageEx = pagina.FindName("imagenTileMediana")
        imagenMediana.Source = Nothing

        Dim imagenAncha As ImageEx = pagina.FindName("imagenTileAncha")
        imagenAncha.Source = Nothing

        Dim imagenGrande As ImageEx = pagina.FindName("imagenTileGrande")
        imagenGrande.Source = Nothing

        If Not juego.ImagenPequeña = Nothing Then
            imagenPequeña.Source = juego.ImagenPequeña
            imagenPequeña.Tag = juego.ImagenPequeña
        End If

        If Not juego.ImagenMediana = Nothing Then
            imagenMediana.Source = juego.ImagenMediana
            imagenMediana.Tag = juego.ImagenMediana
        End If

        If Not juego.ImagenAncha = Nothing Then
            imagenAncha.Source = juego.ImagenAncha
            imagenAncha.Tag = juego.ImagenAncha
        End If

        If Not juego.ImagenGrande = Nothing Then
            imagenGrande.Source = juego.ImagenGrande
            imagenGrande.Tag = juego.ImagenGrande
        End If

    End Sub

End Module

Public Class NetflixDatos

    <JsonProperty("title")>
    Public Titulo As String

    <JsonProperty("vtype")>
    Public Tipo As String

End Class

Public Class NetflixImagenes

    <JsonProperty("boxart")>
    Public Imagenes_166x233 As List(Of NetflixImagen)

    <JsonProperty("bo342x684")>
    Public Imagenes_284x398 As List(Of NetflixImagen)

    <JsonProperty("bo342x192")>
    Public Imagenes_341x192 As List(Of NetflixImagen)

    <JsonProperty("logo")>
    Public Logo As List(Of NetflixImagen)

    <JsonProperty("bo665x375")>
    Public Imagenes_665x374 As List(Of NetflixImagen)

    <JsonProperty("bg")>
    Public Fondos_848x477 As List(Of NetflixImagen)

    <JsonProperty("bo1280x448")>
    Public Imagenes_1280x720 As List(Of NetflixImagen)

    <JsonProperty("billboard")>
    Public Fondos_1280x720 As List(Of NetflixImagen)

End Class

Public Class NetflixImagen

    <JsonProperty("url")>
    Public Enlace As String

    <JsonProperty("width")>
    Public Ancho As String

End Class