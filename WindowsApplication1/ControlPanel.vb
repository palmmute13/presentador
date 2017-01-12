﻿Public Class ControlPanel
    'Valor fijo
    Dim df_CarpetaRecursos = Microsoft.VisualBasic.FileIO.SpecialDirectories.MyDocuments + "\Presentaciones Recursos"
    Dim df_CarpetaCache As String = df_CarpetaRecursos + "\Cache"
    ' POR DEFECTO
    Dim df_videoFilters As String = "*.mp4;*.webm;*.3gp".ToUpper
    Dim df_imageFilters As String = "*.bmp;*.jpg;*.png;*.gif;*.jpeg".ToUpper
    Dim df_audioFilters As String = "*.mp3;*.m4a".ToUpper
    Dim df_urlToWOLSearch As String = "http://wol.jw.org/es/wol/l/r4/lp-s?q="
    Dim df_sitioOficial As String = "http://jw.org/es/"
    Dim df_sitioWOL As String = "http://wol.jw.org/es/"
    Dim df_sitioBroadcasting As String = "http://tv.jw.org/#es"
    Dim df_sitioCanciones As String = "http://wol.jw.org/es/wol/publication/r4/lp-s/sn/S/2009"
    Dim df_sitioCancionesNuevas As String = "http://wol.jw.org/es/wol/publication/r4/lp-s/snnw/S/2016"
    Dim df_fileImageBackgroundDefault As String = "Por defecto"
    Dim df_CarpetaCanciones As String = df_CarpetaRecursos + "\Canciones\"
    Dim df_CarpetaGrabaciones As String = df_CarpetaRecursos + "\Grabaciones\"
    Dim df_NombreArchivoCancionesAntes = "iasn_S_"
    Dim df_NombreArchivoCancionesDespues = ".mp3"
    Dim df_LanguageID = "ES_AR"
    Dim df_Melodias = vbTrue
    ' Valores que se usarán
    Dim videoFilters As String = df_videoFilters
    Dim imageFilters As String = df_imageFilters
    Dim audioFilters As String = df_audioFilters
    Dim urlToWOLSearch As String = df_urlToWOLSearch
    Dim sitioOficial As String = df_sitioOficial
    Dim sitioWOL As String = df_sitioWOL
    Dim sitioBroadcasting As String = df_sitioBroadcasting
    Dim sitioCanciones As String = df_sitioCanciones
    Dim sitioCancionesNuevas As String = df_sitioCancionesNuevas
    Dim fileImageBackgroundDefault As String = df_fileImageBackgroundDefault
    Dim CarpetaCanciones As String = df_CarpetaCanciones
    Dim CarpetaGrabaciones As String = df_CarpetaGrabaciones
    Dim NombreArchivoCancionesAntes = df_NombreArchivoCancionesAntes
    Dim NombreArchivoCancionesDespues = df_NombreArchivoCancionesDespues
    Dim LanguageID = df_LanguageID
    Dim Melodias = df_Melodias
    ' Variables de entorno
    Private WithEvents webClient As System.Net.WebClient
    Dim Presentacion As Presentacion
    Dim panelsTop As Integer = 351
    Dim panelLeft As Integer = 12
    Dim panelControlsTop As Integer = 92
    Dim panelControlsLeft As Integer = 694
    Public nombreGlobalArchivo As String = ""
    Public cModo As String = ""
    Private Declare Function mciSendString Lib "winmm.dll" Alias "mciSendStringA" (ByVal lpstrCommand As String, ByVal lpstrReturnString As String, ByVal uReturnLength As Integer, ByVal hwndCallback As Integer) As Integer
    Dim grabando As Boolean = False
    Dim grabacionEnProgreso As Boolean = False
    Dim grabacionInteligentePausada As Boolean = False
    Dim PointerOverAudioTrack As Boolean = False
    Dim PointerOverVideoTrack As Boolean = False
    Dim versionEnDescarga As String = ""
    Public Sub btnIniciaPresentador_Click(sender As Object, e As EventArgs) Handles btnIniciaPresentador.Click
        tooglePresentacion(Nothing)
    End Sub
    ''' <summary>
    ''' Inicia o cierra el Presentador, según sea el caso, verificando si hay un segundo monitor, de lo contrario se mostrará en el monitor principal, previa confirmación.
    ''' </summary>
    ''' <returns>Devuelve true/false en caso de que se haya completado la tarea o no.</returns>
    Public Function tooglePresentacion(senderForm As Form)
        Dim monitorPrincipal As Boolean = False
        If Not PresentadorDisponible() Then
            Dim screen As Screen
            Try
                screen = Screen.AllScreens(1)
                MostrarStatus(devuelveResourceString("status", "default_en_presentacion"))
            Catch
                screen = Screen.AllScreens(0)
                monitorPrincipal = True

                If MsgBox(devuelveResourceString("msgb", "no_segundo_monitor"), MsgBoxStyle.YesNo, devuelveResourceString("msgb_title", "atencion")) = MsgBoxResult.No Then
                    btnReestablecePresentador.Enabled = False
                    btnReestablecePresentador.BackColor = Color.FromArgb(64, 64, 64)
                    btnReestablecePresentadorANegro.Enabled = False
                    btnReestablecePresentadorANegro.BackColor = Color.FromArgb(64, 64, 64)
                    Me.btnIniciaPresentador.BackgroundImage = My.Resources.Resources.IniciarPresentador
                    MostrarStatus(devuelveResourceString("status", "presentacion_cancelado"))
                    Return False
                End If
                MostrarStatus(devuelveResourceString("status", "default_en_presentacion_principal"))
            End Try
            Presentacion = New Presentacion()
            Presentacion.TopMost = Not monitorPrincipal
            Presentacion.StartPosition = FormStartPosition.Manual
            Presentacion.Location = screen.Bounds.Location + New Point(0, 0)
            Presentacion.FormBorderStyle = FormBorderStyle.None
            Presentacion.WindowState = FormWindowState.Maximized
            Presentacion.Show()
            ActivarContenido(-1)
            Me.btnIniciaPresentador.BackgroundImage = My.Resources.Resources.CerrarPresentador
            btnReestablecePresentador.Enabled = True
            btnReestablecePresentador.BackColor = Color.FromArgb(64, 0, 64)
            btnReestablecePresentadorANegro.Enabled = True
            btnReestablecePresentadorANegro.BackColor = Color.FromArgb(0, 0, 0)
            Return True
        Else
            Try
                If senderForm IsNot Nothing Then
                    Presentacion = senderForm
                End If
                ActivarContenido(-1)
                Presentacion.Close()
                Presentacion.Dispose()
                Presentacion = Nothing
                MostrarStatus(devuelveResourceString("status", "presentacion_cerrado"))
            Catch ex As Exception
                Presentacion = Nothing
                MostrarStatus(devuelveResourceString("status", "presentacion_cerrado_de_antes"))
            End Try
            Me.btnIniciaPresentador.BackgroundImage = My.Resources.Resources.IniciarPresentador
            btnReestablecePresentador.BackColor = Color.FromArgb(64, 64, 64)
            btnReestablecePresentador.Enabled = False
            btnReestablecePresentadorANegro.Enabled = False
            btnReestablecePresentadorANegro.BackColor = Color.FromArgb(64, 64, 64)
            Return True
        End If
    End Function
    ''' <summary>
    ''' Verifica que la ventana de Presentación esté encendida.
    ''' </summary>
    ''' <returns>True si está encendida, False si no.</returns>
    Public Function PresentadorDisponible()
        Dim frmCollection = Application.OpenForms()
        For Each form As Form In frmCollection
            If form.Name = "Presentacion" Then
                Return True
            End If
        Next
        Return False
    End Function
    ''' <summary>
    ''' Activa el contenido en la pantalla de Presentación y los controles para dicho contenido en el Panel de Control.
    ''' </summary>
    ''' <param name="objeto">
    ''' -1. Muestra el fondo negro
    ''' 0. Muestra la imagen por defecto
    ''' 1. Habilita el navegador web
    ''' 2. Habilita el reproductor de video
    ''' 3. Habilita el Adobe PDF Reader
    ''' 4. Habilita la galería de imágenes
    ''' 5. Habilita el reproductor de música
    ''' </param>
    ''' <returns></returns>
    Function ActivarContenido(objeto As Integer)
        If Not objeto = 5 Then
            If PresentadorDisponibleContinuar() Then
                Presentacion.WebBrowser1.Visible = False
                Presentacion.AxAcroPDF1.Visible = False
                Presentacion.AxWindowsMediaPlayer1.Visible = False
                Presentacion.AxWindowsMediaPlayer1.settings.volume = 100
                If (Presentacion.AxWindowsMediaPlayer1.playState = WMPLib.WMPPlayState.wmppsPlaying) Then
                    Presentacion.AxWindowsMediaPlayer1.Ctlcontrols.pause()
                    If chkGrabacionInteligente.Checked And grabacionEnProgreso And grabacionInteligentePausada Then
                        btnGrabarToogle.PerformClick()
                    End If
                End If
                Try
                    Presentacion.AxWindowsMediaPlayer1.fullScreen = False
                Catch ex As Exception
                End Try
                If Presentacion.AxWindowsMediaPlayer1.settings.mute Then
                    Presentacion.AxWindowsMediaPlayer1.settings.mute = False
                End If
                Presentacion.PictureBox1.Visible = False
                Presentacion.BackColor = Color.White
                Presentacion.PictureBox1.BackColor = Color.White
            End If
        Else

            If PresentadorDisponible() Then
                Presentacion.AxWindowsMediaPlayer1.Visible = False
                Presentacion.AxWindowsMediaPlayer1.settings.volume = 100
                If (Presentacion.AxWindowsMediaPlayer1.playState = WMPLib.WMPPlayState.wmppsPlaying) Then
                    Presentacion.AxWindowsMediaPlayer1.Ctlcontrols.pause()
                    If chkGrabacionInteligente.Checked And grabacionEnProgreso And grabacionInteligentePausada Then
                        btnGrabarToogle.PerformClick()
                    End If
                End If
                Try
                    Presentacion.AxWindowsMediaPlayer1.fullScreen = False
                Catch ex As Exception
                End Try
                If Presentacion.AxWindowsMediaPlayer1.settings.mute Then
                    Presentacion.AxWindowsMediaPlayer1.settings.mute = False
                End If
            End If
        End If
        timAudioPosicionActual.Enabled = False
        timVideoPosicionActual.Enabled = False
        AxWindowsMediaPlayer1.Visible = False
        AxWindowsMediaPlayer1.settings.volume = 100
        If (AxWindowsMediaPlayer1.playState = WMPLib.WMPPlayState.wmppsPlaying) Then
            AxWindowsMediaPlayer1.Ctlcontrols.pause()
            If chkGrabacionInteligente.Checked And grabacionEnProgreso And grabacionInteligentePausada Then
                btnGrabarToogle.PerformClick()
            End If
        End If
        If AxWindowsMediaPlayer1.settings.mute Then
            AxWindowsMediaPlayer1.settings.mute = False
        End If
        esconderPanelesControl()
        Select Case objeto
            Case -1 'Fondo negro
                Presentacion.BackColor = Color.Black
            Case 0 'Imagen por defecto
                Presentacion.PictureBox1.Top = 0
                Presentacion.PictureBox1.Left = 0
                Presentacion.PictureBox1.Height = Presentacion.Height
                Presentacion.PictureBox1.Width = Presentacion.Width
                Presentacion.PictureBox1.Visible = True
                If fileImageBackgroundDefault = devuelveResourceString("str", "por_defecto") Then
                    Presentacion.PictureBox1.BackgroundImage = My.Resources.BackgroundLogo
                Else
                    Presentacion.PictureBox1.BackgroundImage = Image.FromFile(fileImageBackgroundDefault)
                End If

                Presentacion.PictureBox1.BackgroundImageLayout = ImageLayout.Zoom
            Case 1 'WebBrowser
                Presentacion.WebBrowser1.Top = 0
                Presentacion.WebBrowser1.Left = 0
                Presentacion.WebBrowser1.Height = Presentacion.Height
                Presentacion.WebBrowser1.Width = Presentacion.Width
                Presentacion.WebBrowser1.Visible = True
            Case 2 'MediaPlayer//Video
                Presentacion.AxWindowsMediaPlayer1.Top = 0
                Presentacion.AxWindowsMediaPlayer1.Left = 0
                Presentacion.AxWindowsMediaPlayer1.Height = Presentacion.Height
                Presentacion.AxWindowsMediaPlayer1.Width = Presentacion.Width
                Presentacion.AxWindowsMediaPlayer1.uiMode = "None"
                Presentacion.AxWindowsMediaPlayer1.Visible = True
                panelControlVideo.Visible = True
                panelControlVideo.Top = panelControlsTop
                panelControlVideo.Left = panelControlsLeft
                timVideoPosicionActual.Enabled = True
            Case 3 'PDF Reader
                Presentacion.AxAcroPDF1.Top = 0
                Presentacion.AxAcroPDF1.Left = 0
                Presentacion.AxAcroPDF1.Height = Presentacion.Height
                Presentacion.AxAcroPDF1.Width = Presentacion.Width
                Presentacion.AxAcroPDF1.Visible = True
                panelControlPDF.Visible = True
                panelControlPDF.Top = panelControlsTop
                panelControlPDF.Left = panelControlsLeft
                Presentacion.AxAcroPDF1.setZoom(100)
            Case 4 'Imagenes
                Presentacion.PictureBox1.Top = 0
                Presentacion.PictureBox1.Left = 0
                Presentacion.PictureBox1.Height = Presentacion.Height
                Presentacion.PictureBox1.Width = Presentacion.Width
                Presentacion.PictureBox1.Visible = True

                panelControlImagen.Visible = vbTrue
                panelControlImagen.Top = panelControlsTop
                panelControlImagen.Left = panelControlsLeft

            Case 5 'Musica
                panelControlAudio.Visible = True
                panelControlAudio.Top = panelControlsTop
                panelControlAudio.Left = panelControlsLeft
                timAudioPosicionActual.Enabled = True



                'Case 6 'Musica // Con video de Fondo
                'TODO crear otro reproductor para la opcion Música + Video de fondo. Rara.
            Case Else
                Return False
        End Select
        Return True
    End Function
    ''' <summary>
    ''' Esconde los paneles de control de contenido en Presentación
    ''' </summary>
    Private Sub esconderPanelesControl()
        Dim control As Control = New Control()
        For Each control In Me.Controls
            If control.Name.Contains("panelControl") Then
                control.Visible = False
            End If
        Next
        control = Nothing
    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles btnCerrar.Click
        If grabando Or grabacionEnProgreso Then
            MsgBox(devuelveResourceString("msgb", "finalizar_grabacion") + lblTitle.Text.Trim())
            Exit Sub
        End If
        If (nombreGlobalArchivo = "" And lstContenidos.Items.Count > 0) Or hayCambios() Then
            Dim respuestaCerrar As MsgBoxResult = MsgBox(devuelveResourceString("msgb", "guardar_cambios") + IIf(nombreGlobalArchivo = "", devuelveResourceString("str", "archivo_default_titulo"), nombreGlobalArchivo) + "?", MsgBoxStyle.YesNoCancel, devuelveResourceString("msgb_title", "confirmacion"))
            If respuestaCerrar = MsgBoxResult.Yes Or respuestaCerrar = MsgBoxResult.No Then
                If respuestaCerrar = MsgBoxResult.Yes Then
                    btnGuardarContenidos.PerformClick()
                End If
                Me.Dispose()
            End If
        Else
            Me.Dispose()
        End If

    End Sub
    ''' <summary>
    ''' Verifica que el archivo abierto haya sido editado o que no haya ningún archivo guardado pero la lista no esté vacía
    ''' </summary>
    ''' <returns>Si hay cambios retorna true sino false</returns>
    Function hayCambios()
        If nombreGlobalArchivo = "" Then
            Return False
        End If
        Dim resultado As String = ""
        For Each item In lstContenidos.Items
            resultado += item.Text + "|" + item.SubItems(1).Text + ";"
        Next
        If My.Computer.FileSystem.ReadAllText(nombreGlobalArchivo) = resultado Then
            Return False
        Else
            Return True
        End If
    End Function
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles btnMinimizar.Click
        Me.WindowState = FormWindowState.Minimized
    End Sub
    ''' <summary>
    ''' Cuando no se guardan las configuraciones reestablece los campos y muestra lo que está guardado en este momento.
    ''' </summary>
    Private Sub refrescaConfiguraciones()
        txtFormatoImagenes.Text = imageFilters
        txtFormatoVideos.Text = videoFilters
        txtFormatoAudio.Text = audioFilters
        txtURLBuscador.Text = urlToWOLSearch
        txtSitioOficial.Text = sitioOficial
        txtURLWOL.Text = sitioWOL
        txtURLBroadcasting.Text = sitioBroadcasting
        txtURLCanciones.Text = sitioCanciones
        txtURLNuevasCanciones.Text = sitioCancionesNuevas
        txtBackgroudImage.Text = fileImageBackgroundDefault
        txtCarpetaCanciones.Text = CarpetaCanciones
        txtCarpetaGrabaciones.Text = CarpetaGrabaciones
        txtPatronAntesCanciones.Text = NombreArchivoCancionesAntes
        txtPatronDespuesCanciones.Text = NombreArchivoCancionesDespues
        chkMelodias.Checked = Melodias
        cmbIdioma.Text = LanguageID
    End Sub
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles btnConfiguracion.Click
        refrescaConfiguraciones()
        panConfigs.Visible = True
        panConfigs.BringToFront()
    End Sub
    ''' <summary>
    ''' En el label de status establece el texto que se mostrará.
    ''' </summary>
    ''' <param name="Status"></param>
    Public Sub MostrarStatus(Status As String)
        lblStatus.Text = devuelveResourceString("str", "lblStatus") + Status
    End Sub
    Private Sub btnReestablecePresentador_Click(sender As Object, e As EventArgs) Handles btnReestablecePresentador.Click
        If PresentadorDisponibleContinuar() Then
            ActivarContenido(0)
            MostrarStatus(devuelveResourceString("status", "presentacion_imagen_por_defecto"))
        End If
    End Sub
    ''' <summary>
    ''' Cuando el presentador se encuentra apagado y se intenta reproducir un contenido solicita la información y luego la confirmación para encenderlo.
    ''' </summary>
    ''' <returns>El resultado de tooglePresentador o true si ya estaba encendido</returns>
    Public Function PresentadorDisponibleContinuar()
        If Not PresentadorDisponible() Then
            If MsgBox(devuelveResourceString("msgb", "presentacion_no_disponible_encender"), MsgBoxStyle.YesNo, devuelveResourceString("msgb_title", "atencion")) = MsgBoxResult.Yes Then
                Return tooglePresentacion(Nothing)
            Else
                Return False
            End If
        End If
        Return True
    End Function
    Private Sub btnAgregaCancion_Click(sender As Object, e As EventArgs) Handles btnAgregaCancion.Click
        esconderPaneles()
        'Dim counter As System.Collections.ObjectModel.ReadOnlyCollection(Of String)
        'counter = My.Computer.FileSystem.GetFiles(CarpetaCanciones)
        'numCanciones.Maximum = counter.Count
        pnlCanciones.Left = panelLeft
        pnlCanciones.Top = panelsTop
        pnlCanciones.Visible = True
        numCanciones.Select(0, numCanciones.Value.ToString.Length)
        numCanciones.Focus()
    End Sub
    Private Sub btnAbreJWorg_Click(sender As Object, e As EventArgs) Handles btnAbreJWorg.Click
        esconderPaneles()
        agregaContenidoALista(devuelveResourceString("text", btnAbreJWorg.Name), sitioOficial, sender)
        If chkAgregaAutomaticamente.Checked Then
            abreWeb(sitioOficial)
            chkAgregaAutomaticamente.Checked = False
        End If
    End Sub
    ''' <summary>
    ''' Inicia el presentador y muestra un sitio web en el navegador del mismo
    ''' </summary>
    ''' <param name="sitio">url del sitio a presentar</param>
    Private Sub abreWeb(sitio As String)
        If Not PresentadorDisponible() Then
            If MsgBox(devuelveResourceString("msgb", "presentacion_no_disponible_encender"), MsgBoxStyle.YesNo, devuelveResourceString("msgb_title", "atencion")) = MsgBoxResult.Yes Then
                If Not tooglePresentacion(Nothing) Then
                    Return
                End If
            Else
                Return
            End If
        End If
        If Not sitio.StartsWith("http://") Or sitio.StartsWith("https://") Then
            sitio = "http://" + sitio
        End If
        Try
            Presentacion.WebBrowser1.Navigate(New Uri(sitio))
            ActivarContenido(1)
            MostrarStatus(devuelveResourceString("status", "presentacion_sitio") + sitio + devuelveResourceString("status", "presentacion_sitio_final"))
        Catch ex As Exception
            MostrarStatus(devuelveResourceString("status", "error_presentacion_sitio") + sitio + ". Error: " + ex.Message)
        End Try

    End Sub
    Private Sub btnAbreWOL_Click(sender As Object, e As EventArgs) Handles btnAbreWOL.Click
        esconderPaneles()
        agregaContenidoALista(devuelveResourceString("text", btnAbreWOL.Name), sitioWOL, sender)
        If chkAgregaAutomaticamente.Checked Then
            abreWeb(sitioWOL)
            chkAgregaAutomaticamente.Checked = False
        End If
    End Sub
    Private Sub btnAbreTV_Click(sender As Object, e As EventArgs) Handles btnAbreTV.Click
        esconderPaneles()
        agregaContenidoALista(devuelveResourceString("text", btnAbreTV.Name), sitioBroadcasting, sender)
        If chkAgregaAutomaticamente.Checked Then
            abreWeb(sitioBroadcasting)
            chkAgregaAutomaticamente.Checked = False
        End If
    End Sub
    ''' <summary>
    ''' Agrega el contenido seleccionado a la lista
    ''' </summary>
    ''' <param name="tipo">Determina el tipo de contenido que se va a presentar</param>
    ''' <param name="archivo">Dirección del contenido. Número de canción, URL o ubicación de archivo.</param>
    ''' <param name="sender">Determina con precisión el tipo de contenido y el color a utilizar</param>
    Private Sub agregaContenidoALista(tipo As String, archivo As String, sender As Object)
        Dim lstItem = New ListViewItem()
        lstItem.Text = tipo
        lstItem.Checked = Me.chkAgregaAutomaticamente.CheckState
        lstItem.BackColor = sender.BackColor
        lstItem.ForeColor = Color.White
        lstItem.ToolTipText = archivo
        lstItem.SubItems.Add(archivo)
        lstContenidos.Items.Add(lstItem)
    End Sub
    Private Sub btnAgregarCancionLista_Click(sender As Object, e As EventArgs) Handles btnAgregarCancionLista.Click
        agregarCancion(sender)

    End Sub
    ''' <summary>
    ''' Agrega el contenido tipo Canción a la lista y la reproduce si así se ha indicado.
    ''' </summary>
    ''' <param name="sender">El botón de canción se envía como parametro para establecer el color en la lista</param>
    Private Sub agregarCancion(sender As Object)
        agregaContenidoALista(devuelveResourceString("str", "cancion"), numCanciones.Value.ToString, sender)
        If chkAgregaAutomaticamente.Checked Then
            suenaMusica()
            chkAgregaAutomaticamente.Checked = False
        End If
    End Sub

    ''' <summary>
    ''' Activa los controles de música, envía el archivo a reproducir y detiene la grabación
    ''' </summary>
    Private Sub suenaMusica()
        Dim cancion As String = buscaArchivoCancion(numCanciones.Value)
        If My.Computer.FileSystem.FileExists(cancion) Then
            ActivarContenido(5)
            If chkGrabacionInteligente.Checked And grabacionEnProgreso Then
                grabacionInteligentePausada = True
                btnGrabarToogle.PerformClick()
            End If
            AxWindowsMediaPlayer1.URL = cancion
            AxWindowsMediaPlayer1.Ctlcontrols.play()
            Try
                trackAudioPosition.Maximum = Integer.Parse(AxWindowsMediaPlayer1.currentMedia.duration + 1)
            Catch ex As Exception
                trackAudioPosition.Maximum = trackAudioPosition.Maximum
            End Try
            btnAudioPlayPausa.BackgroundImage = My.Resources.Pausa

        Else
            MsgBox(devuelveResourceString("msgb", "no_hay_cancion"), MsgBoxStyle.Information, devuelveResourceString("msgb_title", "atencion"))
        End If

    End Sub
    ''' <summary>
    ''' Busca el archivo de audio correspondiente al número ingresado de canción. Utiliza los parametros establecidos en el panel de configuración para ubicar la carpeta de recursos y el nombre de los archivos.
    ''' </summary>
    ''' <param name="numero">Número de canción a reproducir.</param>
    ''' <returns>Una cadena de texto con el nombre completo de la ruta al archivo</returns>
    Function buscaArchivoCancion(numero As Integer)
        Dim auxCarpetaCanciones As String = CarpetaCanciones
        If Not auxCarpetaCanciones.EndsWith("\") Then
            auxCarpetaCanciones += "\"
        End If
        If numero > 135 Then
            If Not My.Computer.FileSystem.FileExists(auxCarpetaCanciones + NombreArchivoCancionesAntes + numero.ToString.Trim().PadLeft(3, "0") + NombreArchivoCancionesDespues) Then
                If My.Computer.FileSystem.FileExists(auxCarpetaCanciones + "snnw_" + devuelveResourceString("str", "patron_idioma_jw") + "_" + numero.ToString.Trim().PadLeft(3, "0") + NombreArchivoCancionesDespues) Then
                    Return auxCarpetaCanciones + "snnw_" + devuelveResourceString("str", "patron_idioma_jw") + "_" + numero.ToString.Trim().PadLeft(3, "0") + NombreArchivoCancionesDespues
                End If
            End If
        End If
        If chkMelodias.Checked Then
            If My.Computer.FileSystem.FileExists(auxCarpetaCanciones + "iasnm_" + devuelveResourceString("str", "patron_idioma_jw") + "_" + numero.ToString.Trim().PadLeft(3, "0") + NombreArchivoCancionesDespues) Then
                Return auxCarpetaCanciones + "iasnm_" + devuelveResourceString("str", "patron_idioma_jw") + "_" + numero.ToString.Trim().PadLeft(3, "0") + NombreArchivoCancionesDespues
            End If
        End If
        Return auxCarpetaCanciones + NombreArchivoCancionesAntes + numero.ToString.Trim().PadLeft(3, "0") + NombreArchivoCancionesDespues
    End Function
    ''' <summary>
    ''' Devuelve una cadena de texto especificada en el archivo de recursos del proyecto, esto permite usar una nomenclatura para las cadenas en distintos idiomas. El nombre de dichas cadenas se forma con el siguiente formato {prefijo}_{nombre}_{codigo_idioma}
    ''' </summary>
    ''' <param name="prefijo">El prefijo se usa para separar las cadenas de caracteres en grupos como msgb, para contenidos de los mensajes, msgb_title, para los títulos de los mensajes, text y ttip para los textos y los tips de los controles y str para cualquier otro string. Por ejemplo.</param>
    ''' <param name="nombre">El nombre del control o de la cadena para identificar</param>
    ''' <returns>Devuelve el contenido del recurso especificado en el idioma deseado.</returns>
    Function devuelveResourceString(prefijo As String, nombre As String)
        If Not prefijo.EndsWith("_") Then
            prefijo += "_"
        End If
        If My.Resources.ResourceManager.GetObject(prefijo + nombre + "_" + LanguageID) = "" Then
            MsgBox(devuelveResourceString("msgb", "obtener_clave_error") + prefijo + nombre + "_" + LanguageID + devuelveResourceString("msgb", "obtener_clave_error_fin"), vbOKOnly, devuelveResourceString("msgb_title", "error"))
        End If
        Return My.Resources.ResourceManager.GetObject(prefijo + nombre + "_" + LanguageID)
    End Function
    ''' <summary>
    ''' Verifica el tipo de contenido seleccionado y lanza la función que lo reproduce o muestra
    ''' </summary>
    Private Sub ejecutarContenido()
        If lstContenidos.SelectedItems.Count = 1 Then
            If (AxWindowsMediaPlayer1.playState = WMPLib.WMPPlayState.wmppsPlaying) Then
                If (MsgBox(devuelveResourceString("str", "cortar_reproduccion"), vbYesNo, devuelveResourceString("msgb_title", "confirmacion")) = MsgBoxResult.No) Then
                    Return
                End If
            End If
            Dim item As ListViewItem = New ListViewItem()
            For Each item In lstContenidos.SelectedItems
                If item.Text.Contains(devuelveResourceString("str", "cancion")) Then
                    numCanciones.Value = Integer.Parse(item.SubItems(1).Text.Trim())
                    suenaMusica()
                End If
                If item.Text.Contains(devuelveResourceString("text", btnAgregaCita.Name)) Then
                    ComboBox1.Text = item.SubItems(1).Text
                    muestraCita()
                End If
                If item.Text.Contains(devuelveResourceString("text", btnAbreJWorg.Name)) Then
                    abreWeb(sitioOficial)
                End If
                If item.Text.Contains(devuelveResourceString("text", btnAbreWOL.Name)) Then
                    abreWeb(sitioWOL)
                End If
                If item.Text.Contains(devuelveResourceString("text", btnAbreTV.Name)) Then
                    abreWeb(sitioBroadcasting)
                End If
                If item.Text.Contains(devuelveResourceString("text", btnAbreCancionero.Name)) Then
                    abreWeb(sitioCanciones)
                End If
                If item.Text.Contains(devuelveResourceString("text", btnAbreCancioneroNuevo.Name)) Then
                    abreWeb(sitioCancionesNuevas)
                End If
                If item.Text.Contains(devuelveResourceString("text", btnAbreSitioWeb.Name)) Then
                    abreWeb(item.SubItems(1).Text.Trim())
                End If
                If item.Text.Contains(devuelveResourceString("str", "audio")) Then
                    suenaOtraMusica(item.SubItems(1).Text.Trim())
                End If
                If item.Text.Contains(devuelveResourceString("str", "video")) Then
                    muestraVideo(item.SubItems(1).Text.Trim())
                End If
                If item.Text.Contains(devuelveResourceString("str", "imagen")) Then
                    muestraImagen(item.SubItems(1).Text.Trim())
                End If
                If item.Text.Contains(devuelveResourceString("str", "pdf")) Then
                    muestraPDF(item.SubItems(1).Text.Trim())
                End If
            Next
        Else
            If lstContenidos.SelectedItems.Count > 1 Then
                MsgBox(devuelveResourceString("msgb", "error_contenidos_a_presentacion"), MsgBoxStyle.OkOnly, devuelveResourceString("msgb_title", "atencion"))
            End If
        End If
    End Sub
    Private Sub lstContenidos_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lstContenidos.DoubleClick
        ejecutarContenido()
    End Sub
    ''' <summary>
    ''' Habilita el Presentador para mostrar PDFs, activa el contenido, abre el archivo en el visor y establece el status correspondiente.
    ''' </summary>
    ''' <param name="archivo">Ubicación del archivo a mostrar</param>
    Private Sub muestraPDF(archivo As String)
        If PresentadorDisponibleContinuar() Then
            Presentacion.AxAcroPDF1.LoadFile(archivo)
            Presentacion.AxAcroPDF1.setShowToolbar(False)
            ActivarContenido(3)
            MostrarStatus(devuelveResourceString("status", "pdf_en_presentacion") + archivo)
        End If
    End Sub
    Private Sub Button1_Click_1(sender As Object, e As EventArgs) Handles btnAgregaCita.Click
        esconderPaneles()
        pnlCitas.Left = panelLeft
        pnlCitas.Top = panelsTop
        pnlCitas.Visible = True
        ComboBox1.SelectAll()
        ComboBox1.Select(0, ComboBox1.Text.Length)
        ComboBox1.Focus()
    End Sub
    ''' <summary>
    ''' Oculta los paneles de controles de contenido en Presentación
    ''' </summary>
    Private Sub esconderPaneles()
        Dim control As Control = New Control()
        For Each control In Me.Controls
            If control.Name.Contains("pnl") Then
                control.Visible = False
            End If
        Next
        control = Nothing
    End Sub
    Private Sub btnAgregaCitaLista_Click(sender As Object, e As EventArgs) Handles btnAgregaCitaLista.Click
        agregarCita(sender)
    End Sub
    ''' <summary>
    ''' Verifica que haya algo escrito en el campo de cita bíblica y lo agrega a la lista, de lo contrario envía un error.
    ''' </summary>
    ''' <param name="sender">Envía el botón como parámetro para establecer el color en la lista</param>
    Private Sub agregarCita(sender As Object)
        If Not ComboBox1.Text.Equals("") Then
            agregaContenidoALista(devuelveResourceString("text", btnAgregaCita.Name), ComboBox1.Text, sender)
            cachearCita(ComboBox1.Text)
            If chkAgregaAutomaticamente.Checked Then
                muestraCita()
                chkAgregaAutomaticamente.Checked = False
            End If
        Else
            MsgBox(devuelveResourceString("msgb", "no_versiculo"), MsgBoxStyle.OkOnly, devuelveResourceString("msgb_title", "atencion"))
        End If
    End Sub
    ''' <summary>
    ''' Verifica que df_CarpetaCache exista, sino, la crea
    ''' </summary>
    Private Sub verificarCarpetaCache()
        If Not My.Computer.FileSystem.DirectoryExists(df_CarpetaCache) Then
            My.Computer.FileSystem.CreateDirectory(df_CarpetaCache)
        End If
    End Sub

    Private Function StringSeguroArchivo(valor As String) As String
        For Each typo In IO.Path.GetInvalidFileNameChars
            valor = valor.Replace(typo, "_")
        Next
        Return valor
    End Function
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="cita"></param>
    Private Sub cachearCita(cita)
        verificarCarpetaCache()
        Dim cita_archivo = StringSeguroArchivo(cita)
        Dim resultado = obtenerTextoDeCita(cita, False)
        If resultado.StartsWith("Error: ") Then
            Return
        End If
        My.Computer.FileSystem.WriteAllText(df_CarpetaCache + "\" + cita_archivo + "_" + LanguageID + ".jwpcache", resultado, False)
    End Sub
    Function obtenerTextoDeCita(cita As String, status As Boolean) As String
        Dim presuntoArchivo = df_CarpetaCache + "\" + StringSeguroArchivo(cita) + "_" + LanguageID + ".jwpcache"
        If My.Computer.FileSystem.FileExists(presuntoArchivo) Then
            If status Then
                MostrarStatus(devuelveResourceString("status", "cita_en_presentacion_de_cache") + ComboBox1.Text + "' (" + presuntoArchivo + ")")
            End If
            Return My.Computer.FileSystem.ReadAllText(presuntoArchivo)
        End If
        Try
            Dim webClient As System.Net.WebClient = New System.Net.WebClient()
            webClient.Encoding = System.Text.Encoding.UTF8
            Dim result = webClient.DownloadString(urlToWOLSearch + cita)
            webClient = Nothing
            If status Then
                MostrarStatus(devuelveResourceString("status", "cita_en_presentacion") + ComboBox1.Text + "' (" + urlToWOLSearch + cita + ")")
            End If
            Return result
        Catch ex As Exception
            If status Then
                MostrarStatus(devuelveResourceString("status", "versiculo_no_encontrado") + ex.Message)
            End If
            Return "Error: " + ex.Message
        End Try
    End Function
    ''' <summary>
    ''' Habilita Presentación para mostrar citas bíblicas, solicita la cita desde la ubicación que esté configurada y la muestra o coloca el cartel de error.
    ''' </summary>
    Private Sub muestraCita()
        If Not PresentadorDisponibleContinuar() Then
            Return
        End If
        Dim cita = ComboBox1.Text
        Dim sourceString = obtenerTextoDeCita(cita, True)
        Presentacion.WebBrowser1.ScriptErrorsSuppressed = True
        Presentacion.WebBrowser1.DocumentText = sourceString
        Presentacion.WebBrowserBiblia = True
        ActivarContenido(1)

        cita = Nothing

        sourceString = Nothing
    End Sub
    Private Sub btnAgregaImagen_Click(sender As Object, e As EventArgs) Handles btnAgregaImagen.Click
        esconderPaneles()
        Dim files As String() = buscarArchivos(sender, devuelveResourceString("str", "imagen"), devuelveResourceString("str", "archivos_imagen") + " (" + imageFilters + ")|" + imageFilters)
        If chkAgregaAutomaticamente.Checked And files.Length > 0 Then
            muestraImagen(files(0))
            chkAgregaAutomaticamente.Checked = False
        End If
    End Sub
    ''' <summary>
    ''' Habilita Presentación para mostrar imágenes, muestra los controles de imagen y muestra la imagen.
    ''' </summary>
    ''' <param name="imagen">Ruta completa a la ubicación del archivo de imagen a mostrar</param>
    Private Sub muestraImagen(imagen As String)
        If PresentadorDisponibleContinuar() Then
            Presentacion.PictureBox1.BackgroundImage = Image.FromFile(imagen)
            If Presentacion.PictureBox1.BackgroundImage.Height > Presentacion.PictureBox1.Height Then
                Presentacion.PictureBox1.BackgroundImageLayout = ImageLayout.Zoom
                btnImagenPosicion.BackgroundImage = My.Resources.CentrarImagen
            Else
                Presentacion.PictureBox1.BackgroundImageLayout = ImageLayout.Center
                btnImagenPosicion.BackgroundImage = My.Resources.AjustarImagen
            End If
            ActivarContenido(4)
            MostrarStatus(devuelveResourceString("status", "imagen_en_presentacion") + imagen)
        End If
    End Sub
    ''' <summary>
    ''' Permite seleccionar archivos del sistema de archivos de la PC para luego agregarlos a la lista.
    ''' </summary>
    ''' <param name="sender">Envía el control que ha realizado la llamada a esta función para luego establecer el estilo en la lista</param>
    ''' <param name="tipo">Coloca el tipo en la primer columna de la lista</param>
    ''' <param name="filtro">El filtro de extensiones disponibles de archivo a seleccionar en base al tipo de contenido</param>
    ''' <returns>Arreglo de strings con las rutas a los archivos seleccionados</returns>
    Function buscarArchivos(sender As Object, tipo As String, filtro As String)

        Dim openFileDialog1 As New OpenFileDialog()
        Dim mostrando As Boolean = False
        openFileDialog1.Multiselect = True
        openFileDialog1.Filter = filtro
        If openFileDialog1.ShowDialog = DialogResult.OK Then
            For Each file In openFileDialog1.FileNames

                Dim lstItem = New ListViewItem()
                lstItem.Checked = Me.chkAgregaAutomaticamente.CheckState And Not mostrando
                lstItem.Text = tipo
                lstItem.BackColor = sender.BackColor
                lstItem.ForeColor = Color.White
                lstItem.ToolTipText = file
                lstItem.SubItems.Add(file)
                lstContenidos.Items.Add(lstItem)
                If chkAgregaAutomaticamente.Checked Then
                    mostrando = True
                End If
                lstItem = Nothing
            Next
            Return openFileDialog1.FileNames
        End If
        Dim retorno As String() = {}
        Return retorno
    End Function
    Private Sub btnAgregaVideo_Click(sender As Object, e As EventArgs) Handles btnAgregaVideo.Click
        esconderPaneles()
        Dim files As String() = buscarArchivos(sender, devuelveResourceString("str", "video"), devuelveResourceString("str", "archivos_video") + " (" + videoFilters + ")|" + videoFilters)
        If chkAgregaAutomaticamente.Checked And files.Length > 0 Then
            muestraVideo(files(0))
            chkAgregaAutomaticamente.Checked = False
        End If
    End Sub
    Private Sub btnAgregaAudio_Click(sender As Object, e As EventArgs) Handles btnAgregaAudio.Click
        esconderPaneles()
        Dim files As String() = buscarArchivos(sender, devuelveResourceString("str", "audio"), devuelveResourceString("str", "archivos_audio") + " (" + audioFilters + ")|" + audioFilters)
        If chkAgregaAutomaticamente.Checked And files.Length > 0 Then
            suenaOtraMusica(files(0))
            chkAgregaAutomaticamente.Checked = False
        End If
    End Sub
    ''' <summary>
    ''' Prepara los controles y comienza a reproducir la música 
    ''' </summary>
    ''' <param name="musica"></param>
    Private Sub suenaOtraMusica(musica As String)
        AxWindowsMediaPlayer1.URL = musica
        ActivarContenido(5)
        If chkGrabacionInteligente.Checked And grabacionEnProgreso Then
            grabacionInteligentePausada = True
            btnGrabarToogle.PerformClick()
        End If
        AxWindowsMediaPlayer1.Ctlcontrols.play()
        trackAudioPosition.Maximum = Integer.Parse(AxWindowsMediaPlayer1.currentMedia.duration) + 1
    End Sub
    Private Sub muestraVideo(video As String)
        If PresentadorDisponibleContinuar() Then
            Presentacion.AxWindowsMediaPlayer1.URL = video
            ActivarContenido(2)
            If chkGrabacionInteligente.Checked And grabacionEnProgreso Then
                grabacionInteligentePausada = True
                btnGrabarToogle.PerformClick()
            End If
            Presentacion.AxWindowsMediaPlayer1.Ctlcontrols.play()
            MostrarStatus(devuelveResourceString("status", "video_en_presentacion") + Presentacion.AxWindowsMediaPlayer1.URL)

        End If
    End Sub
    Private Sub btnAgregaPDF_Click(sender As Object, e As EventArgs) Handles btnAgregaPDF.Click
        esconderPaneles()
        Dim files As String() = buscarArchivos(sender, devuelveResourceString("str", "pdf"), devuelveResourceString("str", "archivos_pdf") + " |" + ("*.PDF").ToUpper)
        If chkAgregaAutomaticamente.Checked And files.Length > 0 Then
            muestraPDF(files(0))
            chkAgregaAutomaticamente.Checked = False
        End If
    End Sub
    Private Sub numCanciones_KeyDown(sender As Object, e As KeyEventArgs) Handles numCanciones.KeyDown
        If e.KeyCode = 13 Then

            btnAgregarCancionLista.PerformClick()
            e.Handled = True
            e.SuppressKeyPress = True
        End If
    End Sub
    Private Sub TrackBar1_Scroll(sender As Object, e As EventArgs) Handles trackPDFZoom.ValueChanged
        Try
            Presentacion.AxAcroPDF1.setZoom(trackPDFZoom.Value)

            If trackPDFZoom.Value = trackPDFZoom.Minimum Then
                btnPDFZoomOut.Enabled = False
            Else
                btnPDFZoomOut.Enabled = True
            End If
            If trackPDFZoom.Value = trackPDFZoom.Maximum Then
                btnPDFZoomIn.Enabled = False
            Else
                btnPDFZoomIn.Enabled = True
            End If
        Catch
        End Try
    End Sub
    Private Sub btnPDFZoomIn_Click(sender As Object, e As EventArgs) Handles btnPDFZoomIn.Click
        trackPDFZoom.Value += 20
    End Sub
    Private Sub btnPDFZoomOut_Click(sender As Object, e As EventArgs) Handles btnPDFZoomOut.Click
        trackPDFZoom.Value -= 20
    End Sub
    Private Sub btnPDFUltimo_Click(sender As Object, e As EventArgs) Handles btnPDFUltimo.Click
        Presentacion.AxAcroPDF1.gotoLastPage()
    End Sub
    Private Sub btnPDFSiguiente_Click(sender As Object, e As EventArgs) Handles btnPDFSiguiente.Click
        Presentacion.AxAcroPDF1.gotoNextPage()
    End Sub
    Private Sub btnPDFAnterior_Click(sender As Object, e As EventArgs) Handles btnPDFAnterior.Click
        Presentacion.AxAcroPDF1.gotoPreviousPage()
    End Sub
    Private Sub btnPDFPrimero_Click(sender As Object, e As EventArgs) Handles btnPDFPrimero.Click
        Presentacion.AxAcroPDF1.gotoFirstPage()
    End Sub
    Private Sub btnReestablecePresentadorANegro_Click(sender As Object, e As EventArgs) Handles btnReestablecePresentadorANegro.Click
        If PresentadorDisponibleContinuar() Then
            ActivarContenido(-1)
            MostrarStatus(devuelveResourceString("status", "negro_en_presentacion"))
        End If
    End Sub
    Private Sub btnAudioPlayPausa_Click(sender As Object, e As EventArgs) Handles btnAudioPlayPausa.Click
        If (AxWindowsMediaPlayer1.playState = WMPLib.WMPPlayState.wmppsPlaying) Then
            timAudioPosicionActual.Enabled = False
            AxWindowsMediaPlayer1.Ctlcontrols.pause()
            If chkGrabacionInteligente.Checked And grabacionEnProgreso And grabacionInteligentePausada Then
                btnGrabarToogle.PerformClick()
            End If
            btnAudioPlayPausa.BackgroundImage = My.Resources.Play
        Else
            timAudioPosicionActual.Enabled = True
            If chkGrabacionInteligente.Checked And grabacionEnProgreso Then
                grabacionInteligentePausada = True
                btnGrabarToogle.PerformClick()
            End If
            AxWindowsMediaPlayer1.Ctlcontrols.play()
            btnAudioPlayPausa.BackgroundImage = My.Resources.Pausa
        End If
    End Sub
    Private Sub Button3_Click_1(sender As Object, e As EventArgs) Handles btnAudioMute.Click
        If AxWindowsMediaPlayer1.settings.mute Then
            AxWindowsMediaPlayer1.settings.mute = False
            btnAudioMute.BackgroundImage = My.Resources.Silencio
        Else
            AxWindowsMediaPlayer1.settings.mute = True
            btnAudioMute.BackgroundImage = My.Resources.Volumen
        End If

    End Sub
    Private Sub TrackBar1_Scroll_1(sender As Object, e As EventArgs) Handles TrackBar1.ValueChanged
        Try
            AxWindowsMediaPlayer1.settings.volume = TrackBar1.Value
        Catch

        End Try
    End Sub
    Private Sub Button3_Click_2(sender As Object, e As EventArgs) Handles btnVideoPlayPausa.Click
        ' If the player is playing, switch to full screen. 
        If (Presentacion.AxWindowsMediaPlayer1.playState = WMPLib.WMPPlayState.wmppsPlaying) Then
            Presentacion.AxWindowsMediaPlayer1.Ctlcontrols.pause()
            If chkGrabacionInteligente.Checked And grabando And grabacionInteligentePausada Then
                btnGrabarToogle.PerformClick()
            End If
            btnVideoPlayPausa.BackgroundImage = My.Resources.Play
        Else
            If chkGrabacionInteligente.Checked And grabando Then
                grabacionInteligentePausada = True
                btnGrabarToogle.PerformClick()
            End If
            Presentacion.AxWindowsMediaPlayer1.Ctlcontrols.play()
            btnVideoPlayPausa.BackgroundImage = My.Resources.Pausa
        End If
    End Sub
    Private Sub btnVideoMute_Click(sender As Object, e As EventArgs) Handles btnVideoMute.Click
        If Presentacion.AxWindowsMediaPlayer1.settings.mute Then
            Presentacion.AxWindowsMediaPlayer1.settings.mute = False
            btnAudioMute.BackgroundImage = My.Resources.Silencio
        Else
            Presentacion.AxWindowsMediaPlayer1.settings.mute = True
            btnAudioMute.BackgroundImage = My.Resources.Volumen
        End If
    End Sub
    Private Sub trackVideoVolume_ValueChanged(sender As Object, e As EventArgs) Handles trackVideoVolume.ValueChanged
        Try
            Presentacion.AxWindowsMediaPlayer1.settings.volume = trackVideoVolume.Value
        Catch

        End Try
    End Sub
    Private Sub btnVideoFullscreen_Click(sender As Object, e As EventArgs) Handles btnVideoFullscreen.Click
        If Presentacion.AxWindowsMediaPlayer1.fullScreen Then
            Presentacion.AxWindowsMediaPlayer1.fullScreen = False
            btnVideoFullscreen.BackgroundImage = My.Resources.Fullscreen
        Else
            Presentacion.AxWindowsMediaPlayer1.fullScreen = True
            btnVideoFullscreen.BackgroundImage = My.Resources.FullscreenExit
        End If
    End Sub
    Private Sub btnLimpiarContenido_Click(sender As Object, e As EventArgs) Handles btnLimpiarContenido.Click
        If lstContenidos.Items.Count > 0 Then
            If MsgBox(devuelveResourceString("msgb", "eliminar_contenido"), MsgBoxStyle.YesNo, devuelveResourceString("msgb_title", "confirmacion")) = MsgBoxResult.Yes Then
                lstContenidos.Items.Clear()
            End If
        End If
    End Sub
    Private Sub btnValidarContenido_Click(sender As Object, e As EventArgs) Handles btnValidarContenido.Click
        validarLista(False)
    End Sub
    Private Sub validarLista(eliminando As Boolean)
        If lstContenidos.Items.Count > 0 Then
            Dim item As ListViewItem
            Dim archivo As String = ""
            Dim archivosInexistentes As String = ""
            Dim cantidadArchivosInexistentes As Integer = 0
            For Each item In lstContenidos.Items
                'TODO: revisar el funcionamiento
                If (devuelveResourceString("str", "audio") + "," + devuelveResourceString("str", "video") + "," + devuelveResourceString("str", "imagen") + "," + devuelveResourceString("str", "pdf")).Contains(item.Text) Then
                    archivo = item.SubItems(1).Text

                End If
                If item.Text = devuelveResourceString("str", "cancion") Then
                    archivo = buscaArchivoCancion(Integer.Parse(item.SubItems(1).Text))
                End If
                If (Not archivo = "") And (Not My.Computer.FileSystem.FileExists(archivo)) Then
                    archivosInexistentes += archivo + Chr(13)
                    cantidadArchivosInexistentes += 1
                    item.BackColor = Color.Red
                    If eliminando Then
                        item.Remove()
                    End If
                End If
                archivo = ""
            Next
            If cantidadArchivosInexistentes > 0 Then
                If cantidadArchivosInexistentes > 1 Then
                    If eliminando Then
                        MsgBox(devuelveResourceString("msgb", "eliminar_contenidos_validados_principio") + cantidadArchivosInexistentes.ToString + devuelveResourceString("msgb", "eliminar_contenidos_validados_final") + Chr(13) + archivosInexistentes, MsgBoxStyle.Exclamation + MsgBoxStyle.OkOnly, devuelveResourceString("msgb_title", "atencion"))
                    Else
                        If MsgBox(devuelveResourceString("msgb", "contenidos_validados_principio") + cantidadArchivosInexistentes.ToString + devuelveResourceString("msgb", "contenidos_validados_medio") + Chr(13) + archivosInexistentes + devuelveResourceString("msgb", "contenido_validado_final"), MsgBoxStyle.Exclamation + MsgBoxStyle.YesNo, devuelveResourceString("msgb_title", "atencion")) = MsgBoxResult.Yes Then
                            validarLista(True)
                        End If

                    End If

                Else
                    If eliminando Then
                        MsgBox(devuelveResourceString("msgb", "eliminar_contenido_validado") + archivosInexistentes, MsgBoxStyle.Exclamation + MsgBoxStyle.OkOnly, devuelveResourceString("msgb_title", "atencion"))
                    Else
                        If MsgBox(devuelveResourceString("msgb", "contenido_validado_principio") + archivosInexistentes + devuelveResourceString("msgb", "contenido_validado_final"), MsgBoxStyle.Exclamation + MsgBoxStyle.YesNo, devuelveResourceString("msgb_title", "atencion")) = MsgBoxResult.Yes Then
                            validarLista(True)
                        End If
                    End If

                End If
            Else
                MsgBox(devuelveResourceString("msgb", "contenido_encontrado"), MsgBoxStyle.OkOnly, devuelveResourceString("msgb_title", "atencion"))
            End If
        End If
    End Sub
    Private Sub Button2_Click_1(sender As Object, e As EventArgs) Handles btnLimpiaItem.Click
        If lstContenidos.SelectedItems.Count > 0 Then
            If MsgBox(devuelveResourceString("msgb", "quitar_contenido"), MsgBoxStyle.YesNo, devuelveResourceString("msgb_title", "confirmacion")) = MsgBoxResult.Yes Then
                For Each item In lstContenidos.SelectedItems
                    item.Remove()
                Next
            End If
        End If
    End Sub
    Private Sub btnSubirItem_Click(sender As Object, e As EventArgs) Handles btnSubirItem.Click
        Dim item As ListViewItem
        For Each item In lstContenidos.SelectedItems
            If item.Index > 0 Then
                Dim itemClone As ListViewItem = item.Clone()
                Dim itemFinal As ListViewItem = lstContenidos.Items.Insert(item.Index - 1, itemClone)
                item.Remove()
                itemFinal.Selected = True
                itemClone = Nothing
            End If
        Next
        item = Nothing
    End Sub
    Private Sub btnBajarItem_Click(sender As Object, e As EventArgs) Handles btnBajarItem.Click
        Dim item As ListViewItem
        For Each item In lstContenidos.SelectedItems
            If item.Index < lstContenidos.Items.Count - 1 Then
                Dim itemClone As ListViewItem = item.Clone()
                Dim indice As Integer = item.Index + 1
                item.Remove()
                Dim itemFinal As ListViewItem = lstContenidos.Items.Insert(indice, itemClone)
                itemFinal.Selected = True
                itemClone = Nothing
                indice = Nothing
            End If
        Next
        item = Nothing
    End Sub
    Private Sub btnGuardarContenidos_Click(sender As Object, e As EventArgs) Handles btnGuardarContenidos.Click
        If lstContenidos.Items.Count > 0 Then
            Dim item As ListViewItem
            Dim resultado As String = ""
            For Each item In lstContenidos.Items
                resultado += item.Text + "|" + item.SubItems(1).Text + ";"
            Next
            If nombreGlobalArchivo = "" Then
                Dim saveFileDialog1 As SaveFileDialog = New SaveFileDialog()
                saveFileDialog1.Filter = devuelveResourceString("str", "archivo_presentacion") + " (.jwp)|*.jwp"
                saveFileDialog1.FileName = devuelveResourceString("str", "archivo_default_titulo")

                If saveFileDialog1.ShowDialog() = DialogResult.OK Then
                    If Not saveFileDialog1.FileName.EndsWith(".jwp") Then
                        saveFileDialog1.FileName += ".jwp"
                    End If
                Else
                    Exit Sub
                End If
                nombreGlobalArchivo = saveFileDialog1.FileName
                lblNombreArchivo.Text = saveFileDialog1.FileName
            End If

            My.Computer.FileSystem.WriteAllText(nombreGlobalArchivo, resultado, False)
            If My.Computer.FileSystem.FileExists(nombreGlobalArchivo) Then
                If My.Computer.FileSystem.ReadAllText(nombreGlobalArchivo) = resultado Then
                    MsgBox(devuelveResourceString("msgb", "archivo_presentacion_exito"), MsgBoxStyle.Information, devuelveResourceString("msgb_title", "atencion"))
                Else
                    MsgBox(devuelveResourceString("msgb", "archivo_presentacion_error_integridad"), MsgBoxStyle.Critical, devuelveResourceString("msgb_title", "error"))
                End If
            Else
                MsgBox(devuelveResourceString("msgb", "archivo_presentacion_error_existencia"), MsgBoxStyle.Critical, devuelveResourceString("msgb_title", "error"))
            End If
        Else
            MsgBox(devuelveResourceString("msgb", "archivo_presentacion_vacio"), MsgBoxStyle.OkOnly, devuelveResourceString("msgb_title", "atencion"))
        End If
    End Sub
    Private Sub btnAbrirContenidos_Click(sender As Object, e As EventArgs) Handles btnAbrirContenidos.Click
        Dim openFileDialog1 As New OpenFileDialog()

        openFileDialog1.Multiselect = True
        'TODO: Verificar funcionamiento
        openFileDialog1.Filter = devuelveResourceString("str", "archivo_presentacion") + " (.jwp)|*.jwp"
        openFileDialog1.Multiselect = False
        If openFileDialog1.ShowDialog = DialogResult.OK Then

            If lstContenidos.Items.Count > 0 Then
                If MsgBox(devuelveResourceString("msgb", "archivo_presentacion_abrir_importar"), MsgBoxStyle.YesNo, devuelveResourceString("msgb_title", "confirmacion")) = MsgBoxResult.Yes Then
                    btnLimpiarContenido.PerformClick()
                    nombreGlobalArchivo = openFileDialog1.FileName
                    lblNombreArchivo.Text = openFileDialog1.FileName
                End If
            Else
                nombreGlobalArchivo = openFileDialog1.FileName
                lblNombreArchivo.Text = openFileDialog1.FileName
            End If
            For Each line In My.Computer.FileSystem.ReadAllText(openFileDialog1.FileName).Split(";")
                If line = "" Then
                    Exit For
                End If
                Dim lstItem = New ListViewItem()
                lstItem.Checked = False
                lstItem.Text = line.Split("|")(0)
                lstItem.BackColor = coloreaItem(line.Split("|")(0).ToString())
                lstItem.ForeColor = Color.White
                lstItem.ToolTipText = line.Split("|")(1)
                lstItem.SubItems.Add(line.Split("|")(1))
                lstContenidos.Items.Add(lstItem)
                lstItem = Nothing
            Next

        End If
    End Sub
    Function coloreaItem(tipoItem As String)
        'Verificar funcionamiento
        If tipoItem = devuelveResourceString("str", "audio") Or tipoItem = devuelveResourceString("str", "cancion") Then
            Return btnAgregaAudio.BackColor
        End If
        'Verificar funcionamiento
        If tipoItem = devuelveResourceString("str", "video") Then
            Return btnAgregaVideo.BackColor
        End If

        If tipoItem = devuelveResourceString("text", btnAgregaCita.Name) Then
            Return btnAgregaCita.BackColor
        End If
        'Verificar funcionamiento
        If tipoItem = devuelveResourceString("str", "pdf") Then
            Return btnAgregaPDF.BackColor
        End If
        'Verificar funcionamiento
        If tipoItem = devuelveResourceString("str", "imagen") Then
            Return btnAgregaImagen.BackColor
        End If
        'Verificar funcionamiento
        If tipoItem = devuelveResourceString("text", btnAbreWOL.Name) Or tipoItem = devuelveResourceString("text", btnAbreJWorg.Name) Or tipoItem = devuelveResourceString("text", btnAbreTV.Name) Then
            Return btnAbreJWorg.BackColor
        End If
        Return Color.Black

    End Function
    Private Sub btnCerrarArchivo_Click(sender As Object, e As EventArgs) Handles btnCerrarArchivo.Click
        nombreGlobalArchivo = ""
        lblNombreArchivo.Text = ""
    End Sub
    Private Sub revisarVersion()
        Dim webClient = New System.Net.WebClient()
        webClient.Encoding = System.Text.Encoding.UTF8
        versionEnDescarga = ""
        Try
            versionEnDescarga = webClient.DownloadString("http://rusticit.com/presentador/version/")

            If Not devuelveResourceString("str", "version") = versionEnDescarga Then
                If My.Computer.FileSystem.FileExists(df_CarpetaRecursos + "\Instalar Presentador_" + versionEnDescarga + ".exe") Then
                    If MsgBox(devuelveResourceString("msgb", "instalacion_ya_lista"), MsgBoxStyle.YesNo, devuelveResourceString("msgb_title", "atencion")) = MsgBoxResult.Yes Then
                        Process.Start(df_CarpetaRecursos + "\Instalar Presentador_" + versionEnDescarga + ".exe")
                    Else
                        lblInstalarNuevaVersion.Visible = True
                    End If
                    Exit Sub
                End If

                If MsgBox(devuelveResourceString("str", "actualizar_principio") + versionEnDescarga + devuelveResourceString("str", "actualizar_medio") + vbCrLf + devuelveResourceString("str", "actualizar_final"), MsgBoxStyle.YesNo, devuelveResourceString("msgb_title", "confirmacion")) = MsgBoxResult.Yes Then
                    Dim remoteFileID As String = webClient.DownloadString("http://rusticit.com/presentador/archivo/")
                    Dim UpdateFileDownload As Uri = New Uri("https://drive.google.com/uc?export=download&id=" + remoteFileID)
                    AddHandler webClient.DownloadFileCompleted, AddressOf Downloaded
                    DownloadingUpdateProgressBar.Visible = True
                    webClient.DownloadFileAsync(UpdateFileDownload, df_CarpetaRecursos + "\Instalar Presentador_" + versionEnDescarga + ".exe")

                    'openURL("https://drive.google.com/uc?export=download&id=" + remoteFileID)
                End If
            End If
        Catch ex As Exception
            'MsgBox(ex.Message, MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, devuelveResourceString("msgb_title", "error"))
        End Try
    End Sub
    Private Sub dpc(ByVal sender As Object, ByVal e As System.Net.DownloadProgressChangedEventArgs) Handles webClient.DownloadProgressChanged
        DownloadingUpdateProgressBar.Value = e.ProgressPercentage
    End Sub

    Private Sub Downloaded()
        lblInstalarNuevaVersion.Visible = True
        DownloadingUpdateProgressBar.Visible = False
    End Sub
    Private Sub ControlPanel_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ajustarAPantalla()
        'Verifica si los archivos .jwp se abren con esta aplicación. En esta ubicación.
        Dim keyExistente = False
        Dim keyNecesitaActualizar = False
        Try
            For Each subkey In My.Computer.Registry.ClassesRoot.GetSubKeyNames
                If subkey = ".jwp" Then
                    keyExistente = True
                    If My.Computer.Registry.ClassesRoot.GetValue("JWP\shell\open\command\") = Application.ExecutablePath & " ""%l"" " Then
                        keyNecesitaActualizar = True
                    End If
                End If
            Next
            If Not keyExistente Then
                My.Computer.Registry.ClassesRoot.CreateSubKey(".jwp").SetValue("", "JWP", Microsoft.Win32.RegistryValueKind.String)
                My.Computer.Registry.ClassesRoot.CreateSubKey("JWP\shell\open\command").SetValue("", Application.ExecutablePath & " ""%l"" ", Microsoft.Win32.RegistryValueKind.String)
                My.Computer.Registry.ClassesRoot.CreateSubKey("JWP\DefaultIcon").SetValue("", df_CarpetaRecursos + "\VistaPrevia.ico", Microsoft.Win32.RegistryValueKind.ExpandString)
            Else
                If keyNecesitaActualizar Then
                    My.Computer.Registry.ClassesRoot.DeleteSubKeyTree("JWP\shell")
                    My.Computer.Registry.ClassesRoot.CreateSubKey("JWP\shell\open\command").SetValue("", Application.ExecutablePath & " ""%l"" ", Microsoft.Win32.RegistryValueKind.String)
                End If
            End If
        Catch ex As Exception
            MsgBox(devuelveResourceString("msgb", "ejecutar_como_admin_principio") + devuelveResourceString("str", "programa_nombre") + devuelveResourceString("msgb", "ejecutar_como_admin_medio") + vbCrLf + devuelveResourceString("msgb", "ejecutar_como_admin_final"), MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, devuelveResourceString("msgb_title", "atencion"))
        End Try

        'Si no encuentro el archivo de configuración lo creo con los valores por defecto.
        If My.Computer.FileSystem.FileExists(df_CarpetaRecursos + "\config.jwpc") Then
            Dim result = My.Computer.FileSystem.ReadAllText(df_CarpetaRecursos + "\config.jwpc")
            Dim count = 1
            For Each line In result.Split(vbCr)
                Select Case count
                    Case 1
                        videoFilters = line
                    Case 2
                        imageFilters = line
                    Case 3
                        audioFilters = line
                    Case 4
                        urlToWOLSearch = line
                    Case 5
                        sitioOficial = line
                    Case 6
                        sitioWOL = line
                    Case 7
                        sitioBroadcasting = line
                    Case 8
                        fileImageBackgroundDefault = line
                    Case 9
                        CarpetaCanciones = line
                    Case 10
                        NombreArchivoCancionesAntes = line
                    Case 11
                        NombreArchivoCancionesDespues = line
                    Case 12
                        CarpetaGrabaciones = line
                    Case 13
                        LanguageID = line
                        cmbIdioma.SelectedValue = LanguageID
                    Case 14
                        Melodias = (line = "True")
                        chkMelodias.Checked = Melodias
                    Case 15
                        sitioCanciones = line
                    Case 16
                        sitioCancionesNuevas = line
                End Select
                count += 1
            Next
        Else
            restableceValoresPorDefecto()
        End If
        ' Si se abrió la aplicación por medio de un archivo .jwp lo lee desde acá
        For Each file In My.Application.CommandLineArgs
            nombreGlobalArchivo = file
            lblNombreArchivo.Text = file

            For Each line In My.Computer.FileSystem.ReadAllText(file).Split(";")
                If line = "" Then
                    Exit For
                End If
                Dim lstItem = New ListViewItem()
                lstItem.Checked = False
                lstItem.Text = line.Split("|")(0)
                lstItem.BackColor = coloreaItem(line.Split("|")(0).ToString())
                lstItem.ForeColor = Color.White
                lstItem.ToolTipText = line.Split("|")(1)
                lstItem.SubItems.Add(line.Split("|")(1))
                lstContenidos.Items.Add(lstItem)
                lstItem = Nothing
            Next
        Next
        establecerIdioma(LanguageID)

        'verifica que no haya una versión superior
        revisarVersion()
    End Sub
    Private Sub establecerIdioma(languageID As String)
        For Each currentControl As Object In Me.Controls
            Try
                If currentControl.GetType.ToString.Contains("Panel") Then
                    For Each miniControl As Object In currentControl.Controls
                        miniControl.Text = My.Resources.ResourceManager.GetObject("text_" + miniControl.Name + "_" + languageID)
                    Next
                Else
                    currentControl.Text = My.Resources.ResourceManager.GetObject("text_" + currentControl.Name + "_" + languageID)
                End If
            Catch ex As Exception
                MsgBox(ex.Message)
            End Try
        Next
        df_fileImageBackgroundDefault = devuelveResourceString("str", "por_defecto")
        ComboBox1.Items.Clear()
        Dim libros As String = devuelveResourceString("df", "libros_biblia")
        For Each libro In libros.Split("|")
            ComboBox1.Items.Add(libro)
        Next
        'Actualiza el nombre de la ventana
        lblTitle.Text = "            " + devuelveResourceString("str", "programa_nombre") + " " + devuelveResourceString("str", "version") + " | " + devuelveResourceString("str", "ventana_panel_control_nombre")
        Me.Text = devuelveResourceString("str", "programa_nombre") + " " + devuelveResourceString("str", "version") + " | " + devuelveResourceString("str", "ventana_panel_control_nombre")
    End Sub
    Private Sub customTooltipShow(sender As Object, e As EventArgs) Handles _
        btnBug.MouseEnter,
        btnConfiguracion.MouseEnter,
        btnCerrarArchivo.MouseEnter,
        btnLimpiaItem.MouseEnter,
        btnValidarContenido.MouseEnter,
        btnBajarItem.MouseEnter,
        btnAbrirContenidos.MouseEnter,
        btnGuardarContenidos.MouseEnter,
        btnSubirItem.MouseEnter,
        btnLimpiarContenido.MouseEnter,
        btnReestablecePresentadorANegro.MouseEnter,
        btnAbreTV.MouseEnter,
        btnAbreWOL.MouseEnter,
        btnAbreJWorg.MouseEnter,
        btnAgregaCita.MouseEnter,
        btnAgregaPDF.MouseEnter,
        btnAgregaCancion.MouseEnter,
        btnAgregaVideo.MouseEnter,
        btnAgregaImagen.MouseEnter,
        btnAgregaAudio.MouseEnter,
        chkAgregaAutomaticamente.MouseEnter,
        btnReestablecePresentador.MouseEnter,
        btnIniciaPresentador.MouseEnter,
        lblStatus.MouseEnter,
        btnMinimizar.MouseEnter,
        btnCerrar.MouseEnter,
        lstContenidos.MouseEnter,
        chkGrabacionInteligente.MouseEnter,
        btnGrabarDetener.MouseEnter,
        btnGrabarCompartir.MouseEnter,
        btnGrabarToogle.MouseEnter,
        btnCancionCancelar.MouseEnter,
        btnAgregarCancionLista.MouseEnter,
        numCanciones.MouseEnter,
        btnAudioMute.MouseEnter,
        TrackBar1.MouseEnter,
        btnAudioPlayPausa.MouseEnter,
        btnPDFZoomIn.MouseEnter,
        btnPDFZoomOut.MouseEnter,
        trackPDFZoom.MouseEnter,
        btnPDFUltimo.MouseEnter,
        btnPDFSiguiente.MouseEnter,
        btnPDFAnterior.MouseEnter,
        btnPDFPrimero.MouseEnter,
        btnImagenPosicion.MouseEnter,
        btnImagenLuz.MouseEnter,
        btnBuscaCarpetaGrabaciones.MouseEnter,
        btnReestablecer.MouseEnter,
        btnGuardarConfig.MouseEnter,
        btnCancelarConfig.MouseEnter,
        btnBuscaCarpetaCanciones.MouseEnter,
        btnBuscaImagenFondo.MouseEnter,
        btnAbreBroadcastingTest.MouseEnter,
        btnAbreWOLTest.MouseEnter,
        btnAbreSitioOficialTest.MouseEnter,
        btnAbreURLBuscador.MouseEnter,
        btnCancelarCita.MouseEnter,
        ComboBox1.MouseEnter,
        btnAgregaCitaLista.MouseEnter,
        chkMelodias.MouseEnter,
        btnReacomodar.MouseEnter,
        btnPreview.MouseEnter,
        btnAbreCancionero.MouseEnter,
        btnAbreCancioneroNuevo.MouseEnter,
        btnAbreSitioWeb.MouseEnter,
        btnAgregaSitioLista.MouseEnter,
        txtCustomURL.MouseEnter,
        btnCancelaSitio.MouseEnter,
        txtURLCanciones.MouseEnter,
        txtURLNuevasCanciones.MouseEnter,
        btnAbreCanciones.MouseEnter,
        btnAbreNuevasCanciones.MouseEnter
        Dim texto As String = My.Resources.ResourceManager.GetObject("ttip_" + sender.Name + "_" + LanguageID)
        If Not texto = "" Then
            ToolTipDestination.Text = texto
        End If
    End Sub
    Private Sub customTooltipHide(sender As Object, e As EventArgs) Handles _
        btnBug.MouseLeave,
        btnConfiguracion.MouseLeave,
        btnCerrarArchivo.MouseLeave,
        btnLimpiaItem.MouseLeave,
        btnValidarContenido.MouseLeave,
        btnBajarItem.MouseLeave,
        btnAbrirContenidos.MouseLeave,
        btnGuardarContenidos.MouseLeave,
        btnSubirItem.MouseLeave,
        btnLimpiarContenido.MouseLeave,
        btnReestablecePresentadorANegro.MouseLeave,
        btnAbreTV.MouseLeave,
        btnAbreWOL.MouseLeave,
        btnAbreJWorg.MouseLeave,
        btnAgregaCita.MouseLeave,
        btnAgregaPDF.MouseLeave,
        btnAgregaCancion.MouseLeave,
        btnAgregaVideo.MouseLeave,
        btnAgregaImagen.MouseLeave,
        btnAgregaAudio.MouseLeave,
        chkAgregaAutomaticamente.MouseLeave,
        btnReestablecePresentador.MouseLeave,
        btnIniciaPresentador.MouseLeave,
        lblStatus.MouseLeave,
        btnMinimizar.MouseLeave,
        btnCerrar.MouseLeave,
        lstContenidos.MouseLeave,
        chkGrabacionInteligente.MouseLeave,
        btnGrabarDetener.MouseLeave,
        btnGrabarCompartir.MouseLeave,
        btnGrabarToogle.MouseLeave,
        btnCancionCancelar.MouseLeave,
        btnAgregarCancionLista.MouseLeave,
        numCanciones.MouseLeave,
        btnAudioMute.MouseLeave,
        TrackBar1.MouseLeave,
        btnAudioPlayPausa.MouseLeave,
        btnPDFZoomIn.MouseLeave,
        btnPDFZoomOut.MouseLeave,
        trackPDFZoom.MouseLeave,
        btnPDFUltimo.MouseLeave,
        btnPDFSiguiente.MouseLeave,
        btnPDFAnterior.MouseLeave,
        btnPDFPrimero.MouseLeave,
        btnImagenPosicion.MouseLeave,
        btnImagenLuz.MouseLeave,
        btnBuscaCarpetaGrabaciones.MouseLeave,
        btnReestablecer.MouseLeave,
        btnGuardarConfig.MouseLeave,
        btnCancelarConfig.MouseLeave,
        btnBuscaCarpetaCanciones.MouseLeave,
        btnBuscaImagenFondo.MouseLeave,
        btnAbreBroadcastingTest.MouseLeave,
        btnAbreWOLTest.MouseLeave,
        btnAbreSitioOficialTest.MouseLeave,
        btnAbreURLBuscador.MouseLeave,
        btnCancelarCita.MouseLeave,
        ComboBox1.MouseLeave,
        btnAgregaCitaLista.MouseLeave,
        chkMelodias.MouseLeave,
        btnReacomodar.MouseLeave,
        btnPreview.MouseLeave,
        btnAbreCancionero.MouseLeave,
        btnAbreCancioneroNuevo.MouseLeave,
        btnAbreSitioWeb.MouseLeave,
        btnAgregaSitioLista.MouseLeave,
        txtCustomURL.MouseLeave,
        btnCancelaSitio.MouseLeave,
        txtURLCanciones.MouseLeave,
        txtURLNuevasCanciones.MouseLeave,
        btnAbreCanciones.MouseLeave,
        btnAbreNuevasCanciones.MouseLeave
        ToolTipDestination.Text = ""
    End Sub
    Private Sub restableceValoresPorDefecto()
        ' Valores que se usarán
        videoFilters = df_videoFilters
        imageFilters = df_imageFilters
        audioFilters = df_audioFilters
        urlToWOLSearch = df_urlToWOLSearch
        sitioOficial = df_sitioOficial
        sitioWOL = df_sitioWOL
        sitioBroadcasting = df_sitioBroadcasting
        sitioCanciones = df_sitioCanciones
        df_sitioCancionesNuevas = df_sitioCancionesNuevas
        fileImageBackgroundDefault = df_fileImageBackgroundDefault
        CarpetaCanciones = df_CarpetaCanciones
        CarpetaGrabaciones = df_CarpetaGrabaciones
        NombreArchivoCancionesAntes = df_NombreArchivoCancionesAntes
        NombreArchivoCancionesDespues = df_NombreArchivoCancionesDespues
        LanguageID = df_LanguageID
        Melodias = df_Melodias
        crearArchivoConfig()


    End Sub
    Private Sub crearArchivoConfig()
        Dim result As String = videoFilters + vbCr + imageFilters + vbCr + audioFilters + vbCr + urlToWOLSearch _
                                + vbCr + sitioOficial + vbCr + sitioWOL + vbCr + sitioBroadcasting + vbCr +
                                fileImageBackgroundDefault + vbCr + CarpetaCanciones + vbCr + NombreArchivoCancionesAntes +
                                vbCr + NombreArchivoCancionesDespues + vbCr + CarpetaGrabaciones + vbCr + LanguageID + vbCr + Melodias.ToString +
                                vbCr + sitioCanciones + vbCr + sitioCancionesNuevas
        If Not My.Computer.FileSystem.DirectoryExists(df_CarpetaRecursos) Then
            My.Computer.FileSystem.CreateDirectory(df_CarpetaRecursos)
        End If
        My.Computer.FileSystem.WriteAllText(df_CarpetaRecursos + "\config.jwpc", result, False)
        'Creo las carpetas por defecto del sistema.
        If Not My.Computer.FileSystem.DirectoryExists(df_CarpetaCanciones) Then
            My.Computer.FileSystem.CreateDirectory(df_CarpetaCanciones)
        End If
        If Not My.Computer.FileSystem.DirectoryExists(df_CarpetaGrabaciones) Then
            My.Computer.FileSystem.CreateDirectory(df_CarpetaGrabaciones)
        End If
        'Copio el ícono
        If Not My.Computer.FileSystem.FileExists(df_CarpetaRecursos + "\VistaPrevia.ico") Then
            Dim file As System.IO.FileInfo
            file = My.Computer.FileSystem.GetFileInfo(Application.ExecutablePath)
            FileSystem.FileCopy(file.DirectoryName + "\VistaPrevia.ico", df_CarpetaRecursos + "\VistaPrevia.ico")
        End If
    End Sub
    Function presionoEnter(e As KeyPressEventArgs)
        Return e.KeyChar = Microsoft.VisualBasic.ChrW(Keys.Enter)
    End Function
    Private Sub lstContenidos_KeyPress(sender As Object, e As KeyPressEventArgs) Handles lstContenidos.KeyPress
        If presionoEnter(e) Then
            ejecutarContenido()
        End If

    End Sub
    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles btnCancelarConfig.Click
        If MsgBox(devuelveResourceString("msgb", "configuracion_cancelar"), MsgBoxStyle.YesNo, devuelveResourceString("msgb_title", "confirmacion")) = MsgBoxResult.Yes Then
            txtFormatoImagenes.Text = imageFilters
            txtFormatoVideos.Text = videoFilters
            txtFormatoAudio.Text = audioFilters
            txtURLBuscador.Text = urlToWOLSearch
            txtSitioOficial.Text = sitioOficial
            txtURLWOL.Text = sitioWOL
            txtURLBroadcasting.Text = sitioBroadcasting
            txtBackgroudImage.Text = fileImageBackgroundDefault
            txtCarpetaCanciones.Text = CarpetaCanciones
            txtPatronAntesCanciones.Text = NombreArchivoCancionesAntes
            txtPatronDespuesCanciones.Text = NombreArchivoCancionesDespues
            panConfigs.Visible = False
            btnAgregaCancion.Focus()
        End If
    End Sub
    Private Sub openURL(url As String)
        If Not url = "" Then
            If Not (url.StartsWith("http://") Or url.StartsWith("https://")) Then
                url = "http://" + url
            End If
            Process.Start("cmd", "/c start " + url)
        Else
            MsgBox(devuelveResourceString("msgb", "url_vacia"), MsgBoxStyle.OkOnly, devuelveResourceString("msgb_title", "atencion"))
        End If

    End Sub
    Private Sub Button1_Click_2(sender As Object, e As EventArgs) Handles btnAbreURLBuscador.Click
        openURL(txtURLBuscador.Text + devuelveResourceString("str", "cita_por_defecto"))
    End Sub
    Private Sub btnAbreSitioOficialTest_Click(sender As Object, e As EventArgs) Handles btnAbreSitioOficialTest.Click
        openURL(txtSitioOficial.Text)
    End Sub
    Private Sub btnAbreWOLTest_Click(sender As Object, e As EventArgs) Handles btnAbreWOLTest.Click
        openURL(txtURLWOL.Text)
    End Sub
    Private Sub btnAbreBroadcastingTest_Click(sender As Object, e As EventArgs) Handles btnAbreBroadcastingTest.Click
        openURL(txtURLBroadcasting.Text)
    End Sub
    Private Sub btnBuscaImagenFondo_Click(sender As Object, e As EventArgs) Handles btnBuscaImagenFondo.Click
        Dim openFileDialog1 As New OpenFileDialog()
        openFileDialog1.Multiselect = False
        'TODO: Verificar funcionamiento
        openFileDialog1.Filter = devuelveResourceString("str", "archivos_imagen") + " (" + imageFilters + ")|" + imageFilters
        If openFileDialog1.ShowDialog = DialogResult.OK Then
            txtBackgroudImage.Text = openFileDialog1.FileName
        End If
    End Sub
    Private Sub btnBuscaCarpetaCanciones_Click(sender As Object, e As EventArgs) Handles btnBuscaCarpetaCanciones.Click
        Dim openFolder As New FolderBrowserDialog()
        If openFolder.ShowDialog = DialogResult.OK Then
            txtCarpetaCanciones.Text = openFolder.SelectedPath
            'TODO: verificar funcionamiento de formato en distintos idiomas
            verificaCancionesCarpeta(openFolder.SelectedPath)
        End If
    End Sub
    Private Sub verificaCancionesCarpeta(path As String)
        If My.Computer.FileSystem.FileExists(path + "\iasn_" + devuelveResourceString("str", "patron_idioma_jw") + "_001.mp3") Or My.Computer.FileSystem.FileExists(path + "\iasn_" + devuelveResourceString("str", "patron_idioma_jw") + "_001.m4a") Then
            If My.Computer.FileSystem.FileExists(path + "\iasn_" + devuelveResourceString("str", "patron_idioma_jw") + "_001.mp3") Then
                txtPatronAntesCanciones.Text = "iasn_" + devuelveResourceString("str", "patron_idioma_jw") + "_"
                txtPatronDespuesCanciones.Text = ".mp3"
            End If
            If My.Computer.FileSystem.FileExists(path + "\iasn_" + devuelveResourceString("str", "patron_idioma_jw") + "_001.m4a") Then
                txtPatronAntesCanciones.Text = "iasn_" + devuelveResourceString("str", "patron_idioma_jw") + "_"
                txtPatronDespuesCanciones.Text = ".m4a"
            End If
        Else
            txtPatronAntesCanciones.Text = ""
            txtPatronDespuesCanciones.Text = ""
            MsgBox(devuelveResourceString("msgb", "no_hay_canciones_en_carpeta") + path, MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, devuelveResourceString("msgb_title", "atencion"))
        End If
    End Sub
    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles btnGuardarConfig.Click
        If MsgBox(devuelveResourceString("msgb", "configuracion_guardar"), MsgBoxStyle.YesNo, devuelveResourceString("msgb_title", "confirmacion")) = MsgBoxResult.Yes Then
            Dim old_LanguageID = LanguageID
            imageFilters = txtFormatoImagenes.Text
            videoFilters = txtFormatoVideos.Text
            audioFilters = txtFormatoAudio.Text
            urlToWOLSearch = txtURLBuscador.Text
            sitioOficial = txtSitioOficial.Text
            sitioWOL = txtURLWOL.Text
            sitioBroadcasting = txtURLBroadcasting.Text
            sitioCanciones = txtURLCanciones.Text
            sitioCancionesNuevas = txtURLNuevasCanciones.Text
            CarpetaCanciones = txtCarpetaCanciones.Text
            NombreArchivoCancionesAntes = txtPatronAntesCanciones.Text
            NombreArchivoCancionesDespues = txtPatronDespuesCanciones.Text
            CarpetaGrabaciones = txtCarpetaGrabaciones.Text
            LanguageID = cmbIdioma.Text
            If My.Resources.ResourceManager.GetObject("str_por_defecto_" + old_LanguageID) = fileImageBackgroundDefault Then
                fileImageBackgroundDefault = devuelveResourceString("str", "por_defecto")
            Else
                fileImageBackgroundDefault = txtBackgroudImage.Text
            End If
            verificaCancionesCarpeta(CarpetaCanciones)
            crearArchivoConfig()
            panConfigs.Visible = False
            btnAgregaCancion.Focus()
            establecerIdioma(LanguageID)
        End If
    End Sub
    Private Sub Button1_Click_3(sender As Object, e As EventArgs) Handles btnReestablecer.Click
        If MsgBox(devuelveResourceString("msgb", "configuracion_restablecer"), MsgBoxStyle.YesNo, devuelveResourceString("msgb_title", "confirmacion")) = MsgBoxResult.Yes Then
            restableceValoresPorDefecto()
            refrescaConfiguraciones()
        End If
    End Sub
    Private Sub btnImagenLuz_Click(sender As Object, e As EventArgs) Handles btnImagenLuz.Click
        Try
            If Presentacion.PictureBox1.BackColor = Color.Black Then
                Presentacion.PictureBox1.BackColor = Color.White
                btnImagenLuz.BackgroundImage = My.Resources.Luz
            Else
                Presentacion.PictureBox1.BackColor = Color.Black
                btnImagenLuz.BackgroundImage = My.Resources.LuzPrendida
            End If
        Catch ex As Exception

        End Try
    End Sub
    Private Sub btnImagenPosicion_Click(sender As Object, e As EventArgs) Handles btnImagenPosicion.Click
        If Presentacion.PictureBox1.BackgroundImageLayout = ImageLayout.Center Then
            Presentacion.PictureBox1.BackgroundImageLayout = ImageLayout.Zoom
            btnImagenPosicion.BackgroundImage = My.Resources.CentrarImagen
        Else
            Presentacion.PictureBox1.BackgroundImageLayout = ImageLayout.Center
            btnImagenPosicion.BackgroundImage = My.Resources.AjustarImagen
        End If
    End Sub
    Private Sub btnGrabarToogle_Click(sender As Object, e As EventArgs) Handles btnGrabarToogle.Click
        If Not grabando Then
            If Not grabacionEnProgreso Then
                mciSendString("open new Type waveaudio Alias recsound", "", 0, 0)
            End If
            mciSendString("record recsound", "", 0, 0)
            btnGrabarToogle.BackgroundImage = My.Resources.Pausa
            lblGrabarFilename.Text = ""
            lblGrabarStatus.Text = devuelveResourceString("str", lblGrabarStatus.Name + "_grabando")
            If chkGrabacionInteligente.Checked Then
                lblGrabarStatus.Text += devuelveResourceString("str", lblGrabarStatus.Name + "_Inteligente")
            End If
            grabando = True
            grabacionEnProgreso = True
            grabacionInteligentePausada = False
            btnGrabarDetener.Enabled = True
            btnGrabarDetener.BackColor = Color.LightCoral
        Else
            mciSendString("pause recsound", "", 0, 0)
            btnGrabarToogle.BackgroundImage = My.Resources.Grabar
            lblGrabarStatus.Text = devuelveResourceString("str", lblGrabarStatus.Name + "_pausada")
            If grabacionInteligentePausada Then
                lblGrabarStatus.Text += devuelveResourceString("str", lblGrabarStatus.Name + "_Inteligente")
            End If
            grabando = False
        End If

    End Sub
    Private Sub btnGrabarCompartir_Click(sender As Object, e As EventArgs) Handles btnGrabarCompartir.Click
        If lblGrabarFilename.Text = "" Then
            MsgBox(devuelveResourceString("msgb", "grabacion_compartir_no"), MsgBoxStyle.OkOnly, devuelveResourceString("msgb_title", "atencion"))
            Exit Sub
        End If

        Dim ShareAudio As SaveFileDialog = New SaveFileDialog()
        Dim archivo As String() = lblGrabarFilename.Text.Split("\")
        ShareAudio.FileName = archivo(archivo.Length - 1)
        ShareAudio.Filter = devuelveResourceString("str", "archivo_grabar_mp3") + " |*.mp3"
        If ShareAudio.ShowDialog = DialogResult.OK Then
            Try
                FileIO.FileSystem.CopyFile(lblGrabarFilename.Text, ShareAudio.FileName, True)
                If FileIO.FileSystem.FileExists(ShareAudio.FileName) Then
                    MsgBox(devuelveResourceString("msgb", "grabacion_compartir_exito"), MsgBoxStyle.OkOnly, devuelveResourceString("msgb_title", "atencion"))
                Else
                    MsgBox(devuelveResourceString("msgb", "grabacion_compartir_error"), MsgBoxStyle.OkOnly, devuelveResourceString("msgb_title", "error"))
                End If
            Catch ex As Exception
                MsgBox(ex.Message + vbCrLf + devuelveResourceString("msgb", "grabacion_compartir_error_desconocido"), MsgBoxStyle.Critical, devuelveResourceString("msgb_title", "error"))
            End Try
        End If

    End Sub
    Private Sub btnBuscaCarpetaGrabaciones_Click(sender As Object, e As EventArgs) Handles btnBuscaCarpetaGrabaciones.Click
        Dim openFolder As New FolderBrowserDialog()
        If openFolder.ShowDialog = DialogResult.OK Then
            txtCarpetaGrabaciones.Text = openFolder.SelectedPath
        End If
    End Sub
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles timAudioPosicionActual.Tick
        If AxWindowsMediaPlayer1.playState = WMPLib.WMPPlayState.wmppsStopped Then
            If chkGrabacionInteligente.Checked And grabacionEnProgreso And grabacionInteligentePausada Then
                btnGrabarToogle.PerformClick()
            End If
            lblAudioPosition.Text = "00: 00 / 00:00"
            trackAudioPosition.Value = 0
            btnAudioPlayPausa.BackgroundImage = My.Resources.Play
            timAudioPosicionActual.Enabled = False
        Else
            trackAudioPosition.Maximum = Convert.ToInt64(AxWindowsMediaPlayer1.currentMedia.duration)
            lblAudioPosition.Text = AxWindowsMediaPlayer1.Ctlcontrols.currentPositionString + " / " + AxWindowsMediaPlayer1.currentMedia.durationString
            If Not PointerOverAudioTrack Then
                Try
                    trackAudioPosition.Value = AxWindowsMediaPlayer1.Ctlcontrols.currentPosition
                Catch
                    trackAudioPosition.Maximum = AxWindowsMediaPlayer1.Ctlcontrols.currentPosition
                    trackAudioPosition.Value = AxWindowsMediaPlayer1.Ctlcontrols.currentPosition
                End Try
            End If
        End If
    End Sub
    Private Sub trackAudioPosition_MouseEnter(sender As Object, e As EventArgs) Handles trackAudioPosition.MouseEnter
        PointerOverAudioTrack = True
    End Sub
    Private Sub trackAudioPosition_MouseLeave(sender As Object, e As EventArgs) Handles trackAudioPosition.MouseLeave
        PointerOverAudioTrack = False
    End Sub
    Private Sub trackVideoPosition_MouseEnter(sender As Object, e As EventArgs) Handles trackVideoPosicionActual.MouseEnter
        PointerOverVideoTrack = True
    End Sub
    Private Sub trackVideoPosition_MouseLeave(sender As Object, e As EventArgs) Handles trackVideoPosicionActual.MouseLeave
        PointerOverVideoTrack = False
    End Sub
    Private Sub trackAudioPosition_Scroll(sender As Object, e As EventArgs) Handles trackAudioPosition.Scroll
        AxWindowsMediaPlayer1.Ctlcontrols.currentPosition = trackAudioPosition.Value
        lblAudioPosition.Text = AxWindowsMediaPlayer1.Ctlcontrols.currentPositionString
    End Sub
    Private Sub trackVideoPosition_Scroll(sender As Object, e As EventArgs) Handles trackVideoPosicionActual.Scroll
        Presentacion.AxWindowsMediaPlayer1.Ctlcontrols.currentPosition = trackVideoPosicionActual.Value
        lblVideoPosition.Text = Presentacion.AxWindowsMediaPlayer1.Ctlcontrols.currentPositionString
    End Sub
    Private Sub timVideoPosicionActual_Tick(sender As Object, e As EventArgs) Handles timVideoPosicionActual.Tick
        If Presentacion.AxWindowsMediaPlayer1.playState = WMPLib.WMPPlayState.wmppsStopped Then
            If chkGrabacionInteligente.Checked And grabacionEnProgreso And grabacionInteligentePausada Then
                btnGrabarToogle.PerformClick()
            End If
            lblVideoPosition.Text = "00:00 / 00:00"
            trackVideoPosicionActual.Value = 0
            btnVideoPlayPausa.BackgroundImage = My.Resources.Play
            timVideoPosicionActual.Enabled = False
            ActivarContenido(-1)
        Else
            trackVideoPosicionActual.Maximum = Convert.ToInt64(Presentacion.AxWindowsMediaPlayer1.currentMedia.duration) + 1
            lblVideoPosition.Text = Presentacion.AxWindowsMediaPlayer1.Ctlcontrols.currentPositionString + " / " + Presentacion.AxWindowsMediaPlayer1.currentMedia.durationString
            If Not PointerOverVideoTrack Then
                Try
                    trackVideoPosicionActual.Value = Presentacion.AxWindowsMediaPlayer1.Ctlcontrols.currentPosition
                Catch
                    trackVideoPosicionActual.Maximum = Presentacion.AxWindowsMediaPlayer1.Ctlcontrols.currentPosition
                    trackVideoPosicionActual.Value = Presentacion.AxWindowsMediaPlayer1.Ctlcontrols.currentPosition
                End Try
            End If
        End If
    End Sub
    Private Sub btnBug_Click(sender As Object, e As EventArgs) Handles btnBug.Click
        openURL("https://docs.google.com/forms/d/1FR3D6GkVley2wNg4oy5MLKIsINsR3Km5Cjq6FYr7jHs/viewform?entry.1588766469=" + devuelveResourceString("str", "version"))
    End Sub
    Private Sub btnGrabarDetener_Click(sender As Object, e As EventArgs) Handles btnGrabarDetener.Click
        mciSendString("pause recsound", "", 0, 0)
        btnGrabarToogle.BackgroundImage = My.Resources.Grabar
        lblGrabarStatus.Text = devuelveResourceString("str", lblGrabarStatus.Name + "_detenida")
        grabando = False
        grabacionInteligentePausada = False
        Dim SaveAudio As SaveFileDialog = New SaveFileDialog()
        SaveAudio.InitialDirectory = CarpetaGrabaciones
        Dim sFecha As String = DateAndTime.DateString
        Dim sHora As String = DateAndTime.TimeString.Replace(":", ".")
        Dim sMes As String = sFecha.Split("-")(0)
        Dim sDia As String = sFecha.Split("-")(1)
        Dim sAnio As String = sFecha.Split("-")(2)
        SaveAudio.FileName = devuelveResourceString("str", "archivo_grabar_titulo_por_defecto") + sAnio + "-" + sMes + "-" + sDia + "_" + sHora + ".mp3"
        SaveAudio.Filter = devuelveResourceString("str", "archivo_grabar_mp3") + " |*.mp3"
        If SaveAudio.ShowDialog = DialogResult.OK Then
            mciSendString("save recsound " + Chr(34) + SaveAudio.FileName + Chr(34), "", 0, 0)
            mciSendString("close recsound", "", 0, 0)
            If FileIO.FileSystem.FileExists(SaveAudio.FileName) Then
                lblGrabarStatus.Text = devuelveResourceString("str", lblGrabarStatus.Name + "_detenida_guardado")
                lblGrabarFilename.Text = SaveAudio.FileName
                btnGrabarCompartir.Enabled = True
                btnGrabarCompartir.BackColor = Color.LightCoral
            Else
                lblGrabarStatus.Text = devuelveResourceString("str", lblGrabarStatus.Name + "_detenida_error")
                lblGrabarFilename.Text = ""
                btnGrabarCompartir.Enabled = False
                btnGrabarCompartir.BackColor = Color.FromArgb(64, 64, 64)
            End If
            grabacionEnProgreso = False
            btnGrabarDetener.BackColor = Color.FromArgb(64, 64, 64)
            btnGrabarDetener.Enabled = False
        Else
            Dim result As Integer = MsgBox(devuelveResourceString("msgb", "grabacion_cerrar_agregar"), MsgBoxStyle.YesNoCancel, devuelveResourceString("msgb_title", "confirmacion"))
            If result = MsgBoxResult.Yes Then
                lblGrabarStatus.Text = devuelveResourceString("str", lblGrabarStatus.Name + "_pausada_no_guardada")
                lblGrabarFilename.Text = ""
                btnGrabarCompartir.Enabled = False
                btnGrabarCompartir.BackColor = Color.FromArgb(64, 64, 64)
            End If
            If result = MsgBoxResult.No Then
                lblGrabarStatus.Text = devuelveResourceString("str", lblGrabarStatus.Name + "_detenida_no_guardada")
                lblGrabarFilename.Text = ""
                btnGrabarCompartir.Enabled = False
                btnGrabarCompartir.BackColor = Color.FromArgb(64, 64, 64)
                grabacionEnProgreso = False
                btnGrabarDetener.BackColor = Color.FromArgb(64, 64, 64)
                btnGrabarDetener.Enabled = False
            End If
        End If
    End Sub

    Private Sub btnCancionCancelar_Click(sender As Object, e As EventArgs) Handles btnCancionCancelar.Click
        pnlCanciones.Visible = False
    End Sub

    Private Sub btnCancelarCita_Click(sender As Object, e As EventArgs) Handles btnCancelarCita.Click
        pnlCitas.Visible = False
    End Sub

    Private Sub Ajustar(sender As Object, e As EventArgs) Handles lblTitle.DoubleClick, btnReacomodar.Click
        ajustarAPantalla()
    End Sub
    Private Sub ajustarAPantalla()
        Me.StartPosition = FormStartPosition.Manual
        Me.Location = Screen.PrimaryScreen.Bounds.Location + New Point(Screen.PrimaryScreen.WorkingArea.Left, Screen.PrimaryScreen.WorkingArea.Top)
        Me.Size = New Drawing.Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height)
        btnCerrar.Left = Me.Width - btnCerrar.Width
        btnReacomodar.Left = btnCerrar.Left - btnReacomodar.Width
        btnMinimizar.Left = btnReacomodar.Left - btnMinimizar.Width
        btnConfiguracion.Left = btnMinimizar.Left - btnConfiguracion.Width
        panConfigs.Top = 28
        panConfigs.Left = btnConfiguracion.Left + btnConfiguracion.Width - panConfigs.Width
        btnBug.Left = btnConfiguracion.Left - btnBug.Width
        'StatusBar.Width = Me.Width
        'StatusBar.Left = 0
        Me.Refresh()
    End Sub
    Private Sub lblInstalarNuevaVersion_Click(sender As Object, e As EventArgs) Handles lblInstalarNuevaVersion.Click
        If MsgBox(devuelveResourceString("msgb", "instalar"), MsgBoxStyle.YesNo, devuelveResourceString("msgb_title", "confirmacion")) = MsgBoxResult.No Then
            Exit Sub
        End If

        If System.IO.File.Exists(df_CarpetaRecursos + "\Instalar Presentador_" + versionEnDescarga + ".exe") = True Then
            Process.Start(df_CarpetaRecursos + "\Instalar Presentador_" + versionEnDescarga + ".exe")
        Else
            MsgBox(devuelveResourceString("msgb", "instalar_no_archivo"), MsgBoxStyle.Critical, devuelveResourceString("msgb_title", "error"))
        End If
    End Sub

    Private Sub ControlPanel_Scroll(sender As Object, e As ScrollEventArgs) Handles MyBase.Scroll
        If e.ScrollOrientation = ScrollOrientation.HorizontalScroll Then
            ajustarAPantalla()
        End If
    End Sub
    Private Sub ControlPanel_MouseWheel(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseWheel
        ajustarAPantalla()
    End Sub

    Private Sub btnPreview_Click(sender As Object, e As EventArgs) Handles btnPreview.Click, itemPresentar.Click
        ejecutarContenido()
    End Sub

    Private Sub ComboBox1_KeyDown(sender As Object, e As KeyEventArgs) Handles ComboBox1.KeyDown
        If e.KeyCode = 13 Then
            btnAgregaCitaLista.PerformClick()
        End If
    End Sub

    Private Sub btnAgregaPPT_Click(sender As Object, e As EventArgs)
        esconderPaneles()
        Dim files As String() = buscarArchivos(sender, devuelveResourceString("str", "ppt"), devuelveResourceString("str", "archivos_ppt") + " |" + "*.ppt;*.pptx;*.ppsx".ToUpper)
        If chkAgregaAutomaticamente.Checked And files.Length > 0 Then
            muestraPPT(files(0))
            chkAgregaAutomaticamente.Checked = False
        End If
    End Sub
    Private Sub muestraPPT(archivo As String)
        If PresentadorDisponibleContinuar() Then
            ActivarContenido(0)
            MostrarStatus(devuelveResourceString("status", "ppt_en_presentacion") + archivo)
        End If
    End Sub

    Private Sub btnAbreCancionero_Click(sender As Object, e As EventArgs) Handles btnAbreCancionero.Click
        esconderPaneles()
        agregaContenidoALista(devuelveResourceString("text", btnAbreCancionero.Name), sitioCanciones, sender)
        If chkAgregaAutomaticamente.Checked Then
            abreWeb(sitioCanciones)
            chkAgregaAutomaticamente.Checked = False
        End If
    End Sub

    Private Sub btnAbreCanciones_Click(sender As Object, e As EventArgs) Handles btnAbreCanciones.Click
        openURL(txtURLCanciones.Text)
    End Sub

    Private Sub btnAbreNuevasCanciones_Click(sender As Object, e As EventArgs) Handles btnAbreNuevasCanciones.Click
        openURL(txtURLNuevasCanciones.Text)
    End Sub

    Private Sub btnAbreCancioneroNuevo_Click(sender As Object, e As EventArgs) Handles btnAbreCancioneroNuevo.Click
        esconderPaneles()
        agregaContenidoALista(devuelveResourceString("text", btnAbreCancioneroNuevo.Name), sitioCancionesNuevas, sender)
        If chkAgregaAutomaticamente.Checked Then
            abreWeb(sitioCancionesNuevas)
            chkAgregaAutomaticamente.Checked = False
        End If
    End Sub

    Private Sub btnAgregaSitioLista_Click(sender As Object, e As EventArgs) Handles btnAgregaSitioLista.Click
        If Not txtCustomURL.Text.Equals("") Then
            esconderPaneles()
            agregaContenidoALista(devuelveResourceString("text", btnAbreSitioWeb.Name), txtCustomURL.Text, sender)
            If chkAgregaAutomaticamente.Checked Then
                abreWeb(txtCustomURL.Text)
                chkAgregaAutomaticamente.Checked = False
            End If
        Else
            MsgBox(devuelveResourceString("msgb", "noURL"), MsgBoxStyle.OkOnly, devuelveResourceString("msgb_title", "atencion"))
        End If

    End Sub

    Private Sub btnCancelaSitio_Click(sender As Object, e As EventArgs) Handles btnCancelaSitio.Click
        esconderPaneles()
    End Sub

    Private Sub btnAbreSitioWeb_Click(sender As Object, e As EventArgs) Handles btnAbreSitioWeb.Click
        esconderPaneles()
        pnlSitio.Left = panelLeft
        pnlSitio.Top = panelsTop
        pnlSitio.Visible = True
        txtCustomURL.SelectAll()
        txtCustomURL.Select(0, txtCustomURL.Text.Length)
        txtCustomURL.Focus()
    End Sub
End Class