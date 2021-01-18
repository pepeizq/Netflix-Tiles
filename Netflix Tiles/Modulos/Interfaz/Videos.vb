Imports System.Net
Imports Microsoft.Toolkit.Uwp.UI.Controls
Imports Newtonsoft.Json
Imports Windows.Storage
Imports Windows.UI

Namespace Interfaz

    Module Videos

        Public Sub Cargar()

            Dim config As ApplicationDataContainer = ApplicationData.Current.LocalSettings
            Dim recursos As New Resources.ResourceLoader

            Dim frame As Frame = Window.Current.Content
            Dim pagina As Page = frame.Content

            Dim spBuscadorVideos As StackPanel = pagina.FindName("spBuscadorVideos")
            Dim spBuscadorVideos2 As StackPanel = pagina.FindName("spBuscadorVideos2")

            If config.Values("BuscadorBotones") = Nothing Then
                config.Values("BuscadorBotones") = 0
            End If

            If config.Values("BuscadorBotones") = 0 Then
                spBuscadorVideos.Visibility = Visibility.Visible
                spBuscadorVideos2.Visibility = Visibility.Collapsed
            Else
                spBuscadorVideos.Visibility = Visibility.Collapsed
                spBuscadorVideos2.Visibility = Visibility.Visible
            End If

            '----------------------------------------------

            Dim botonBuscarVideos As Button = pagina.FindName("botonBuscarVideos")
            botonBuscarVideos.IsEnabled = False

            AddHandler botonBuscarVideos.Click, AddressOf BuscarVideosClick
            AddHandler botonBuscarVideos.PointerEntered, AddressOf EfectosHover.Entra_Boton_1_05
            AddHandler botonBuscarVideos.PointerExited, AddressOf EfectosHover.Sale_Boton_1_05

            Dim tbBuscadorVideos As TextBox = pagina.FindName("tbBuscadorVideos")
            tbBuscadorVideos.Tag = botonBuscarVideos
            AddHandler tbBuscadorVideos.TextChanged, AddressOf BuscadorVideosTextoCambia

            '----------------------------------------------

            Dim botonBuscarVideos2 As Button = pagina.FindName("botonBuscarVideos2")
            botonBuscarVideos2.IsEnabled = False

            AddHandler botonBuscarVideos2.Click, AddressOf BuscarVideosClick2
            AddHandler botonBuscarVideos2.PointerEntered, AddressOf EfectosHover.Entra_Boton_1_05
            AddHandler botonBuscarVideos2.PointerExited, AddressOf EfectosHover.Sale_Boton_1_05

            Dim tbBuscadorVideos2 As TextBox = pagina.FindName("tbBuscadorVideos2")
            tbBuscadorVideos2.Tag = botonBuscarVideos2
            AddHandler tbBuscadorVideos2.TextChanged, AddressOf BuscadorVideosTextoCambia

            '----------------------------------------------

            Dim fondoBoton As New SolidColorBrush With {
                .Color = App.Current.Resources("ColorCuarto"),
                .Opacity = 0.8
            }

            Dim spBotonesVideos As StackPanel = pagina.FindName("spBotonesVideos")
            spBotonesVideos.Width = spBuscadorVideos.Width

            Dim botonBuscadorVideos As Button = pagina.FindName("botonBuscadorVideos")
            botonBuscadorVideos.Tag = 0

            If config.Values("BuscadorBotones") = 0 Then
                botonBuscadorVideos.Background = fondoBoton
            Else
                botonBuscadorVideos.Background = New SolidColorBrush(Colors.Transparent)
            End If

            AddHandler botonBuscadorVideos.Click, AddressOf CambiarModo
            AddHandler botonBuscadorVideos.PointerEntered, AddressOf EfectosHover.Entra_Boton_Texto
            AddHandler botonBuscadorVideos.PointerExited, AddressOf EfectosHover.Sale_Boton_Texto

            Dim botonBuscadorVideos2 As Button = pagina.FindName("botonBuscadorVideos2")
            botonBuscadorVideos2.Tag = 1

            If config.Values("BuscadorBotones") = 0 Then
                botonBuscadorVideos2.Background = New SolidColorBrush(Colors.Transparent)
            Else
                botonBuscadorVideos2.Background = fondoBoton
            End If

            AddHandler botonBuscadorVideos2.Click, AddressOf CambiarModo
            AddHandler botonBuscadorVideos2.PointerEntered, AddressOf EfectosHover.Entra_Boton_Texto
            AddHandler botonBuscadorVideos2.PointerExited, AddressOf EfectosHover.Sale_Boton_Texto

        End Sub

        Private Sub BuscadorVideosTextoCambia(sender As Object, e As TextChangedEventArgs)

            Dim tb As TextBox = sender
            Dim boton As Button = tb.Tag

            If tb.Text.Trim.Length > 2 Then
                boton.IsEnabled = True
            Else
                boton.IsEnabled = False
            End If

        End Sub

        Private Async Sub BuscarVideosClick(sender As Object, e As RoutedEventArgs)

            Estado(False, False)

            Dim frame As Frame = Window.Current.Content
            Dim pagina As Page = frame.Content

            Dim pbBuscadorVideos As ProgressBar = pagina.FindName("pbBuscadorVideos")
            pbBuscadorVideos.Visibility = Visibility.Visible

            Dim tbBuscadorVideos As TextBox = pagina.FindName("tbBuscadorVideos")
            tbBuscadorVideos.IsEnabled = False

            Dim buscar As String = tbBuscadorVideos.Text.Trim

            Dim wvBuscador As WebView = pagina.FindName("wvBuscador")
            Await WebView.ClearTemporaryWebDataAsync()

            Dim config As ApplicationDataContainer = ApplicationData.Current.LocalSettings

            If config.Values("Buscador") = Nothing Then
                config.Values("Buscador") = 0
            End If

            If config.Values("Buscador") = 0 Then
                wvBuscador.Navigate(New Uri("https://www.bing.com/search?q=netflix+" + buscar))
            ElseIf config.Values("Buscador") = 1 Then
                wvBuscador.Navigate(New Uri("https://www.google.com/search?q=netflix+" + buscar))
            End If

            wvBuscador.Tag = buscar

            AddHandler wvBuscador.LoadCompleted, AddressOf WvBuscar

        End Sub

        Private Async Sub WvBuscar(sender As Object, e As NavigationEventArgs)

            Dim frame As Frame = Window.Current.Content
            Dim pagina As Page = frame.Content

            Dim botonBuscarVideos As Button = pagina.FindName("botonBuscarVideos")
            Dim pbBuscadorVideos As ProgressBar = pagina.FindName("pbBuscadorVideos")
            Dim tbBuscadorVideos As TextBox = pagina.FindName("tbBuscadorVideos")

            Dim wv As WebView = sender
            Dim enlace As String = wv.Source.AbsoluteUri

            If enlace.Contains(".bing.") Then
                Dim html As String = Await wv.InvokeScriptAsync("eval", New String() {"document.documentElement.outerHTML;"})

                If html.Contains("class=" + ChrW(34) + "b_title" + ChrW(34)) Then
                    Dim int As Integer = html.IndexOf("class=" + ChrW(34) + "b_title" + ChrW(34))
                    Dim temp As String = html.Remove(0, int)

                    Dim int2 As Integer = temp.IndexOf("<a")
                    Dim temp2 As String = temp.Remove(0, int2 + 2)

                    Dim int3 As Integer = temp2.IndexOf("href=")
                    Dim temp3 As String = temp2.Remove(0, int3 + 6)

                    Dim int4 As Integer = temp3.IndexOf(ChrW(34))
                    Dim temp4 As String = temp3.Remove(int4, temp3.Length - int4)

                    If temp4.Trim.Contains("https://www.netflix.com/") And temp4.Trim.Contains("/title/") Then
                        AbrirAPIoNetflix(temp4.Trim, botonBuscarVideos)
                    Else
                        Dim config As ApplicationDataContainer = ApplicationData.Current.LocalSettings
                        config.Values("Buscador") = 1
                        Dim buscar As String = wv.Tag
                        wv.Navigate(New Uri("https://www.google.com/search?q=netflix+" + buscar))
                    End If
                Else
                    Dim config As ApplicationDataContainer = ApplicationData.Current.LocalSettings
                    config.Values("Buscador") = 1
                    Dim buscar As String = wv.Tag
                    wv.Navigate(New Uri("https://www.google.com/search?q=netflix+" + buscar))
                End If
            ElseIf enlace.Contains(".google.") Then
                Dim html As String = Await wv.InvokeScriptAsync("eval", New String() {"document.documentElement.outerHTML;"})

                If html.Contains("query:netflix") Then
                    Dim int As Integer = html.IndexOf("query:netflix")
                    Dim temp As String = html.Remove(0, int)

                    Dim int2 As Integer = temp.IndexOf("<a")
                    Dim temp2 As String = temp.Remove(0, int2 + 2)

                    Dim int3 As Integer = temp2.IndexOf("href=")
                    Dim temp3 As String = temp2.Remove(0, int3 + 6)

                    Dim int4 As Integer = temp3.IndexOf(ChrW(34))
                    Dim temp4 As String = temp3.Remove(int4, temp3.Length - int4)

                    If temp4.Trim.Contains("https://www.netflix.com/") And temp4.Trim.Contains("/title/") Then
                        AbrirAPIoNetflix(temp4.Trim, botonBuscarVideos)
                    Else
                        Estado(True, True)
                    End If
                Else
                    Estado(True, True)
                End If
            ElseIf enlace.Contains("https://www.netflix.com/") Then
                Dim html As String = Await wv.InvokeScriptAsync("eval", New String() {"document.documentElement.outerHTML;"})

                If html.Contains("class=" + ChrW(34) + "logo" + ChrW(34)) Then
                    Dim int As Integer = html.IndexOf("class=" + ChrW(34) + "logo" + ChrW(34))
                    Dim temp As String = html.Remove(0, int)

                    Dim int2 As Integer = temp.IndexOf("src=")
                    Dim temp2 As String = temp.Remove(0, int2 + 5)

                    Dim int3 As Integer = temp2.IndexOf(ChrW(34))
                    Dim temp3 As String = temp2.Remove(int3, temp2.Length - int3)

                    Dim imagenLogo As String = temp3.Trim

                    Dim int4 As Integer = temp.IndexOf("alt=")
                    Dim temp4 As String = temp.Remove(0, int4 + 5)

                    Dim int5 As Integer = temp4.IndexOf(ChrW(34))
                    Dim temp5 As String = temp4.Remove(int5, temp4.Length - int5)

                    Dim titulo As String = temp5.Trim

                    If html.Contains("class=" + ChrW(34) + "hero-image hero-image-desktop" + ChrW(34)) Then
                        Dim int6 As Integer = html.IndexOf("class=" + ChrW(34) + "hero-image hero-image-desktop" + ChrW(34))
                        Dim temp6 As String = html.Remove(0, int6)

                        Dim int7 As Integer = temp6.IndexOf("background-image:")
                        Dim temp7 As String = temp6.Remove(0, int7)

                        Dim int8 As Integer = temp7.IndexOf("url(")
                        Dim temp8 As String = temp7.Remove(0, int8 + 5)

                        Dim int9 As Integer = temp8.IndexOf(ChrW(34))
                        Dim temp9 As String = temp8.Remove(int9, temp8.Length - int9)

                        Dim imagenFondo As String = temp9.Trim

                        Dim id As String = String.Empty

                        If enlace.Contains("/title/") Then
                            Dim int10 As Integer = enlace.IndexOf("/title/")
                            id = enlace.Remove(0, int10 + 7)
                        End If

                        Dim icono As String = String.Empty
                        Dim html2 As String = html

                        Dim i As Integer = 0
                        While i < 10
                            If html2.Contains("<link") Then
                                Dim int11 As Integer = html2.IndexOf("<link")
                                Dim temp11 As String = html2.Remove(0, int11)

                                Dim int12 As Integer = temp11.IndexOf("/>")
                                Dim temp12 As String = temp11.Remove(int12, temp11.Length - int12)

                                html2 = html2.Remove(0, int11 + 2)

                                If temp12.Contains(ChrW(34) + "apple-touch-icon" + ChrW(34)) Then
                                    Dim int13 As Integer = temp12.IndexOf("href=")
                                    Dim temp13 As String = temp12.Remove(0, int13 + 6)

                                    Dim int14 As Integer = temp13.IndexOf(ChrW(34))
                                    Dim temp14 As String = temp13.Remove(int14, temp13.Length - int14)

                                    icono = Await Configuracion.Cache.DescargarImagen(temp14.Trim, id + "icono", "icono")
                                End If
                            End If
                            i += 1
                        End While

                        Dim imagenFondoEx As ImageEx = pagina.FindName("imagenFondo")
                        imagenFondoEx.Source = Await Configuracion.Cache.DescargarImagen(imagenFondo, id + "fondo", "fondo")

                        Dim imagenLogoEx As ImageEx = pagina.FindName("imagenLogo")
                        imagenLogoEx.Source = Await Configuracion.Cache.DescargarImagen(imagenLogo, id + "logo", "logo")

                        Dim gridImagen As Grid = pagina.FindName("gridImagen")

                        Dim fichero As StorageFile = Await ApplicationData.Current.LocalFolder.CreateFileAsync(id + ".png", CreationCollisionOption.ReplaceExisting)
                        Dim imagen As String = fichero.Path
                        Await Task.Delay(5000)
                        Try
                            Await ImagenFichero.Generar(fichero, gridImagen, gridImagen.ActualWidth, gridImagen.ActualHeight)
                        Catch ex As Exception

                        End Try

                        Dim enlace2 As String = String.Empty

                        If html.Contains(ChrW(34) + "@type" + ChrW(34) + ":" + ChrW(34) + "TVSeries" + ChrW(34)) Then
                            enlace2 = "netflix:/app?browseVideoId=" + id
                        ElseIf html.Contains(ChrW(34) + "@type" + ChrW(34) + ":" + ChrW(34) + "Movie" + ChrW(34)) Then
                            enlace2 = "netflix:/app?playVideoId=" + id
                        End If

                        Dim video As New Tile(titulo, id, enlace2, icono, imagenLogo, imagen, imagen)
                        botonBuscarVideos.Tag = video

                        Netflix.BotonTile_Click(botonBuscarVideos)
                    End If
                End If

                Estado(True, True)
            End If

        End Sub

        Private Sub BuscarVideosClick2(sender As Object, e As RoutedEventArgs)

            Estado(False, False)

            Dim frame As Frame = Window.Current.Content
            Dim pagina As Page = frame.Content

            Dim tbBuscadorVideos2 As TextBox = pagina.FindName("tbBuscadorVideos2")
            Dim id As String = tbBuscadorVideos2.Text.Trim

            Dim boton As Button = sender
            AbrirAPIoNetflix(id, boton)

        End Sub

        Private Async Sub AbrirAPIoNetflix(enlace As String, boton As Button)

            Dim frame As Frame = Window.Current.Content
            Dim pagina As Page = frame.Content

            If enlace.Contains("/title/") Then
                Dim int As Integer = enlace.IndexOf("/title/")
                enlace = enlace.Remove(0, int + 7)
            End If

            enlace = enlace.Replace("/", Nothing)

            If enlace.Contains("?") Then
                Dim int2 As Integer = enlace.IndexOf("?")
                enlace = enlace.Remove(int2, enlace.Length - int2)
            End If

            Dim id As String = enlace.Trim
            Dim resultadoDatos As List(Of NetflixDatos) = Nothing
            Dim resultadoImagenes As NetflixImagenes = Nothing
            Dim abrirNetflix As Integer = 0

            Dim htmlDatos As String = Await Decompiladores.HttpClient(New Uri("https://unogs.com/api/title/detail?netflixid=" + id))

            If Not htmlDatos = Nothing Then
                resultadoDatos = JsonConvert.DeserializeObject(Of List(Of NetflixDatos))(htmlDatos)

                If Not resultadoDatos Is Nothing Then
                    abrirNetflix += 1
                End If
            End If

            Dim htmlImagenes As String = Await Decompiladores.HttpClient(New Uri("https://unogs.com/api/title/bgimages?netflixid=" + id))

            If Not htmlImagenes = Nothing Then
                resultadoImagenes = JsonConvert.DeserializeObject(Of NetflixImagenes)(htmlImagenes)

                If Not resultadoImagenes Is Nothing Then
                    abrirNetflix += 1
                End If
            End If

            If abrirNetflix = 2 Then
                Dim titulo As String = WebUtility.HtmlDecode(resultadoDatos(0).Titulo)

                Dim enlace2 As String = String.Empty

                If resultadoDatos(0).Tipo.ToLower = "series" Then
                    enlace2 = "netflix:/app?browseVideoId=" + id
                Else
                    enlace2 = "netflix:/app?playVideoId=" + id
                End If

                Dim icono As String = String.Empty

                If Not resultadoImagenes.Imagenes_166x233 Is Nothing Then
                    icono = resultadoImagenes.Imagenes_166x233(0).Enlace
                End If

                If icono = String.Empty Then
                    If Not resultadoImagenes.Imagenes_665x374 Is Nothing Then
                        icono = resultadoImagenes.Imagenes_665x374(0).Enlace
                    End If
                End If

                Dim logo As String = String.Empty

                If Not resultadoImagenes.Logo Is Nothing Then
                    logo = resultadoImagenes.Logo(0).Enlace
                End If

                If logo = String.Empty Then
                    If Not resultadoImagenes.Imagenes_1280x720 Is Nothing Then
                        logo = resultadoImagenes.Imagenes_1280x720(0).Enlace
                    End If
                End If

                Dim azar As New Random
                Dim ancha As String = resultadoImagenes.Imagenes_1280x720(azar.Next(0, resultadoImagenes.Imagenes_1280x720.Count - 1)).Enlace
                Dim grande As String = resultadoImagenes.Imagenes_284x398(azar.Next(0, resultadoImagenes.Imagenes_284x398.Count - 1)).Enlace

                Dim video As New Tile(titulo, id, enlace2, icono, logo, ancha, grande)
                boton.Tag = video

                Netflix.BotonTile_Click(boton)

                Estado(True, True)
            Else
                Dim wvBuscador As WebView = pagina.FindName("wvBuscador")
                wvBuscador.Navigate(New Uri(enlace))
            End If

        End Sub

        Private Sub CambiarModo(sender As Object, e As RoutedEventArgs)

            Dim frame As Frame = Window.Current.Content
            Dim pagina As Page = frame.Content

            Dim spBuscadorVideos As StackPanel = pagina.FindName("spBuscadorVideos")
            Dim spBuscadorVideos2 As StackPanel = pagina.FindName("spBuscadorVideos2")

            Dim botonBuscadorVideos As Button = pagina.FindName("botonBuscadorVideos")
            Dim botonBuscadorVideos2 As Button = pagina.FindName("botonBuscadorVideos2")

            Dim fondoBoton As New SolidColorBrush With {
                .Color = App.Current.Resources("ColorCuarto"),
                .Opacity = 0.8
            }

            Dim boton As Button = sender
            Dim modo As Integer = boton.Tag

            Dim config As ApplicationDataContainer = ApplicationData.Current.LocalSettings
            config.Values("BuscadorBotones") = modo

            If modo = 0 Then
                spBuscadorVideos.Visibility = Visibility.Visible
                spBuscadorVideos2.Visibility = Visibility.Collapsed

                botonBuscadorVideos.Background = fondoBoton
                botonBuscadorVideos2.Background = New SolidColorBrush(Colors.Transparent)
            Else
                spBuscadorVideos.Visibility = Visibility.Collapsed
                spBuscadorVideos2.Visibility = Visibility.Visible

                botonBuscadorVideos.Background = New SolidColorBrush(Colors.Transparent)
                botonBuscadorVideos2.Background = fondoBoton
            End If

        End Sub

        Private Sub Estado(estado As Boolean, borrarTexto As Boolean)

            Dim frame As Frame = Window.Current.Content
            Dim pagina As Page = frame.Content

            Dim botonBuscadorVideos As Button = pagina.FindName("botonBuscadorVideos")
            botonBuscadorVideos.IsEnabled = estado

            Dim botonBuscadorVideos2 As Button = pagina.FindName("botonBuscadorVideos2")
            botonBuscadorVideos2.IsEnabled = estado

            Dim pbBuscadorVideos As ProgressBar = pagina.FindName("pbBuscadorVideos")

            If estado = False Then
                pbBuscadorVideos.Visibility = Visibility.Visible
            Else
                pbBuscadorVideos.Visibility = Visibility.Collapsed
            End If

            Dim tbBuscadorVideos As TextBox = pagina.FindName("tbBuscadorVideos")
            tbBuscadorVideos.IsEnabled = estado

            If borrarTexto = True Then
                tbBuscadorVideos.Text = String.Empty
            End If

            Dim tbBuscadorVideos2 As TextBox = pagina.FindName("tbBuscadorVideos2")
            tbBuscadorVideos2.IsEnabled = estado

            If borrarTexto = True Then
                tbBuscadorVideos2.Text = String.Empty
            End If

            Dim botonBuscarVideos As Button = pagina.FindName("botonBuscarVideos")
            botonBuscadorVideos.IsEnabled = estado

            Dim botonBuscarVideos2 As Button = pagina.FindName("botonBuscarVideos2")
            botonBuscadorVideos2.IsEnabled = estado
        End Sub

    End Module

End Namespace