Imports Microsoft.Toolkit.Uwp.UI.Controls
Imports Windows.Storage

Namespace Interfaz

    Module Videos

        Public Sub Cargar()

            Dim recursos As New Resources.ResourceLoader

            Dim frame As Frame = Window.Current.Content
            Dim pagina As Page = frame.Content

            Dim tbBuscadorVideos As TextBox = pagina.FindName("tbBuscadorVideos")

            AddHandler tbBuscadorVideos.TextChanged, AddressOf BuscadorVideosTextoCambia

            Dim botonBuscarVideos As Button = pagina.FindName("botonBuscarVideos")
            botonBuscarVideos.IsEnabled = False

            AddHandler botonBuscarVideos.Click, AddressOf BuscarVideosClick
            AddHandler botonBuscarVideos.PointerEntered, AddressOf EfectosHover.Entra_Boton_IconoTexto
            AddHandler botonBuscarVideos.PointerExited, AddressOf EfectosHover.Sale_Boton_IconoTexto

        End Sub

        Private Sub BuscadorVideosTextoCambia(sender As Object, e As TextChangedEventArgs)

            Dim frame As Frame = Window.Current.Content
            Dim pagina As Page = frame.Content

            Dim botonBuscarVideos As Button = pagina.FindName("botonBuscarVideos")

            Dim tb As TextBox = sender

            If tb.Text.Trim.Length > 2 Then
                botonBuscarVideos.IsEnabled = True
            Else
                botonBuscarVideos.IsEnabled = False
            End If

        End Sub

        Private Async Sub BuscarVideosClick(sender As Object, e As RoutedEventArgs)

            Dim boton As Button = sender
            boton.IsEnabled = False

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
                        wv.Navigate(New Uri(temp4.Trim))
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
                        wv.Navigate(New Uri(temp4.Trim))
                    Else
                        botonBuscarVideos.IsEnabled = True
                        pbBuscadorVideos.Visibility = Visibility.Collapsed
                        tbBuscadorVideos.IsEnabled = True
                        tbBuscadorVideos.Text = String.Empty
                    End If
                Else
                    botonBuscarVideos.IsEnabled = True
                    pbBuscadorVideos.Visibility = Visibility.Collapsed
                    tbBuscadorVideos.IsEnabled = True
                    tbBuscadorVideos.Text = String.Empty
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

                        Netflix.BotonTile_Click()
                    End If
                End If

                botonBuscarVideos.IsEnabled = True
                pbBuscadorVideos.Visibility = Visibility.Collapsed
                tbBuscadorVideos.IsEnabled = True
                tbBuscadorVideos.Text = String.Empty

            End If

        End Sub

    End Module

End Namespace