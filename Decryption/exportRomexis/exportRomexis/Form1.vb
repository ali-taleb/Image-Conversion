
Imports Leadtools
Imports Leadtools.Codecs
Imports Leadtools.Codecs.CodecsPcdOptions
Imports System.IO
Imports System.Reflection
Imports System.Threading
Public Class Form1
    Dim dir = ""
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If Not File.Exists(My.Computer.FileSystem.SpecialDirectories.Desktop & "\ImageInfo.csv") Then
            MsgBox("please add ImageInfo file (\ImageInfo.csv) on the desktop ")
            Exit Sub
        End If
        If FolderBrowserDialog1.ShowDialog = DialogResult.OK Then
            Label1.Text = "Wait ..."

            dir = FolderBrowserDialog1.SelectedPath
            Dim th As New Thread(AddressOf exec)
            th.SetApartmentState(ApartmentState.STA)
            th.Start()
        Else
            Label1.Text = "0"
            Exit Sub
        End If

    End Sub

    Private Sub exec()
        Dim s = File.ReadAllLines(My.Computer.FileSystem.SpecialDirectories.Desktop & "\ImageInfo.csv")
        Dim ff = My.Computer.FileSystem.GetFiles(dir, FileIO.SearchOption.SearchAllSubDirectories, "*")
        'aa
        SetLicense()

        Label1.Invoke(Sub()
                          Label1.Text = ff.Count
                      End Sub)
        Dim notFound = 0
        For j = 0 To ff.Count - 1

            Dim ffn As New FileInfo(ff(j))


            For i = 0 To s.Count - 1
                Try
                    If s(i).Contains("|") Then

                        Dim ext = Split(s(i), "|")(10) ' Notes column
                        Dim rotation = Split(s(i), "|")(8) ' rotation column
                        Dim flip = Split(s(i), "|")(9) ' flip column
                        'Dim f = dir & "\" & Split(s(i), "|")(0).Replace(".jpg", ".img")

                        'If File.Exists(f) Then
                        If ffn.Name = Split(s(i), "|")(0).Replace(".jpg", ".img") Then
                            Dim fn As New FileInfo(ff(j))
                            If ext = ".tif" Then
                                Dim ra As New RasterCodecs
                                Dim im = ra.Load(ff(j))
                                ra.Save(im, ff(j).Replace(".img", ".jpg"), RasterImageFormat.Jpeg, 24)
                                Try
                                    My.Computer.FileSystem.DeleteFile(ff(j), FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                                Catch ex As Exception
                                    log("Error in Deleting ")
                                End Try
                            Else
                                My.Computer.FileSystem.RenameFile(ff(j), fn.Name.Replace(".img", ".jpg"))
                            End If
                            If rotation = "0" Then
                            Else
                                Try
                                    Dim impath = fn.Directory.FullName & "\" & fn.Name.Replace(".img", ".jpg")
                                    Dim bmp = New Bitmap(impath)
                                    If rotation = "90" Then
                                        bmp.RotateFlip(RotateFlipType.Rotate270FlipNone)
                                        bmp.Save(impath)
                                    ElseIf rotation = "180" Then
                                        bmp.RotateFlip(RotateFlipType.Rotate180FlipNone)
                                        bmp.Save(impath)
                                    ElseIf rotation = "270" Then
                                        bmp.RotateFlip(RotateFlipType.Rotate90FlipNone)
                                        bmp.Save(impath)
                                    End If
                                Catch ex As Exception
                                    MsgBox("Error in rotating")
                                    MsgBox(ex.ToString)
                                End Try
                            End If

                            If flip = "0" Then
                            Else
                                Try
                                    Dim impath = fn.Directory.FullName & "\" & fn.Name.Replace(".img", ".jpg")
                                    Dim bmp = New Bitmap(impath)
                                    If flip = "1" Then
                                        bmp.RotateFlip(RotateFlipType.RotateNoneFlipX)
                                        bmp.Save(impath)
                                    ElseIf flip = "2" Then
                                        bmp.RotateFlip(RotateFlipType.RotateNoneFlipY)
                                        bmp.Save(impath)
                                    End If
                                Catch ex As Exception
                                    MsgBox("Error in flipping")
                                    MsgBox(ex.ToString)
                                End Try
                            End If
                            Exit For
                        End If
                    End If
                Catch ex As Exception
                    MsgBox("Error in reading line " & i)
                    MsgBox("The Error is : " & ex.ToString)
                    Exit Sub
                End Try

                If i = s.Count - 1 Then
                    notFound = notFound + 1
                    log("File does not exist " & ff(j))
                End If

            Next
            Label4.Invoke(Sub()
                              Label4.Text = j + 1
                          End Sub)
        Next

        MsgBox("From the ImageInfo there were " & notFound & " Files were not in the slected directory. Check logFile")
    End Sub

    Public Shared Function SetLicense(ByVal silent As Boolean) As Boolean
        Try
            Dim licenseFilePath As String = Application.StartupPath & "\License\LEADTOOLS.LIC"
            Dim developerKey As String = File.ReadAllText(Application.StartupPath & "\License\LEADTOOLS.LIC.key")
            RasterSupport.SetLicense(licenseFilePath, developerKey)
        Catch ex As Exception

            MsgBox("error code ; 666126 lciense leadtools" & ex.Message)
        End Try

        If RasterSupport.KernelExpired Then
            Dim licenseName = "LEADTOOLS.LIC"
            Dim keyName = "LEADTOOLS.LIC.KEY"
            Dim _assembly As Assembly
            Dim developerKey As String = Nothing
            Dim licenseFile As Byte() = Nothing

            Try
                _assembly = Assembly.GetExecutingAssembly()

                Using _imageStream As Stream = _assembly.GetManifestResourceStream(keyName)

                    If _imageStream IsNot Nothing Then

                        Using reader As StreamReader = New StreamReader(_imageStream)

                            If reader IsNot Nothing Then
                                developerKey = reader.ReadToEnd()
                            End If
                        End Using
                    End If
                End Using

                Using _imageStream As Stream = _assembly.GetManifestResourceStream(licenseName)

                    If _imageStream IsNot Nothing Then
                        Dim br As BinaryReader = New BinaryReader(_imageStream)
                        licenseFile = New Byte(_imageStream.Length - 1) {}
                        _imageStream.Read(licenseFile, 0, licenseFile.Length)
                        br.Close()
                        _imageStream.Close()
                    End If
                End Using

            Catch ex As Exception
                licenseFile = Nothing

                MsgBox("error code ; 666127 lciense leadtools" & ex.Message)
            End Try

            If (developerKey IsNot Nothing) AndAlso (licenseFile IsNot Nothing) Then

                Try
                    RasterSupport.SetLicense(licenseFile, developerKey)
                Catch ex As Exception
                    MsgBox("error code ; 666128 lciense leadtools" & ex.Message)
                End Try
            End If
        End If

        If RasterSupport.KernelExpired Then

            If silent = False Then
                Dim msg As String = "Your license file is missing, invalid or expired. LEADTOOLS will not function. Please contact LEAD Sales for information on obtaining a valid license."
                Dim logmsg As String = String.Format("*** NOTE: {0} ***{1}", msg, Environment.NewLine)
                System.Diagnostics.Debugger.Log(0, Nothing, "*******************************************************************************" & Environment.NewLine)
                System.Diagnostics.Debugger.Log(0, Nothing, logmsg)
                System.Diagnostics.Debugger.Log(0, Nothing, "*******************************************************************************" & Environment.NewLine)
                MessageBox.Show(Nothing, msg, "No LEADTOOLS License", MessageBoxButtons.OK, MessageBoxIcon.[Stop])
                System.Diagnostics.Process.Start("https://www.leadtools.com/downloads/evaluation-form.asp?evallicenseonly=true")
            End If

            Return False
        End If

        Return True
    End Function

    Public Shared Function SetLicense() As Boolean
        Return SetLicense(False)
    End Function

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim s = "Be careful if this is running for the first couple of time we still need to make sure that the template matching in the sql query below does actullay match , I mean the ones in the column PhotoPos" &
            "First download the sql database Romexis and run the query " & vbNewLine &
                "select 
                concat('I',a.image_id,'.jpg') as [filename],
                a.patient_id as patientId, 
                isnull(cast(b.study_id as varchar(16)),concat(cast(a.updated_date as varchar(8)),cast(a.patient_id as varchar(8)))) as seriesId,
                isnull(e.[description],'') as seriesDescription,

	                case
		                when (c.template_id = 1) Then 2
		                when (c.template_id = 2) Then 1 
		                when (c.template_id = 3) Then 3 
						when (c.template_id = 6) Then 2 
						when (c.template_id = 7) Then 2 
						when (c.template_id = 8) Then 2 
		                else 0 
	                end as seriesTemplate,

                    concat(SUBSTRING(cast(a.updated_date as varchar(8)), 5,2),'/', SUBSTRING(cast(a.updated_date as varchar(8)), 7,2),'/', SUBSTRING(cast(a.updated_date as varchar(8)), 1,4)) as seriesDate,
                a.image_id as PhotoId,
                case
		                when (d.template_image_id = 1) Then 2001
		                when (d.template_image_id = 2) Then 2002 
		                when (d.template_image_id = 3) Then 2003 
		                when (d.template_image_id = 4) Then 2004
		                when (d.template_image_id = 5) Then 1008 
		                when (d.template_image_id = 6) Then 1007
		                when (d.template_image_id = 7) Then 1001
		                when (d.template_image_id = 8) Then 1002 
		                when (d.template_image_id = 9) Then 1003 
		                when (d.template_image_id = 10) Then 1011
		                when (d.template_image_id = 11) Then 1012 
		                when (d.template_image_id = 12) Then 1015 
		                when (d.template_image_id = 13) Then 1016
		                when (d.template_image_id = 14) Then 1010 
		                when (d.template_image_id = 15) Then 1018
		                when (d.template_image_id = 16) Then 1018
		                when (d.template_image_id = 17) Then 1017 
		                when (d.template_image_id = 18) Then 1018 
		                when (d.template_image_id = 19) Then 1014
		                when (d.template_image_id = 20) Then 1010 
		                when (d.template_image_id = 21) Then 1006 
		                when (d.template_image_id = 22) Then 1005
		                when (d.template_image_id = 23) Then 1004 
		                when (d.template_image_id = 24) Then 1013 
		                when (d.template_image_id = 25) Then 1010
		                when (d.template_image_id = 26) Then 3001 
		                when (d.template_image_id = 27) Then 3001
		                when (d.template_image_id = 28) Then 3001 
		                when (d.template_image_id = 29) Then 0 
		                when (d.template_image_id = 30) Then 0
		                when (d.template_image_id = 31) Then 0 
		                when (d.template_image_id = 32) Then 0 
		                when (d.template_image_id = 33) Then 0
		                when (d.template_image_id = 34) Then 0 
		                when (d.template_image_id = 35) Then 0 
		                when (d.template_image_id = 36) Then 2002
		                when (d.template_image_id = 37) Then 2003 
		                when (d.template_image_id = 38) Then 2002 
		                when (d.template_image_id = 39) Then 2003
		                when (d.template_image_id = 40) Then 0 
		                when (d.template_image_id = 41) Then 0 
		                when (d.template_image_id = 42) Then 0
		                when (d.template_image_id = 43) Then 0 
		                when (d.template_image_id = 44) Then 0 		
		                else 0 
	                end as photoPos,
                    a.rotation_angle as photoRotation,
                    a.is_mirrored as photoFlip,
	                concat('.',a.image_format) as note /* this is file extension*/
                /*d.exposure_order as photoPos*/
                from [RIM_Image_Info]  a 
                left join  [RIM_Study_Image] as b on b.image_id = a.image_id 
                left join [RIM_Study]  c on c.study_id = b.study_id
                left join [RIM_Template_Image] d on d.template_image_id = b.template_image_id 
                left join [RIM_Template] e on  e.template_id = d.template_id

                    " & vbNewLine &
                "then run this software , this software will replace the file extensions to jpg and if tif we use raster codecs" & vbNewLine &
                "Image info file :" &
       " '0 Filename
        '1 PatientID
        '2 SeriesId
        '3 SeriesDescription
        '4 SeriesTemplate
        '5 SeriesDate
        '6 PhotoId
        '7 PhotoPOS
        '8 PhotoRotation values 0 90 180 270
        '9 PhotoFlip 0, 1 for horizantal, 2 for vertical
        '10 Notes"
        MsgBox(s)
    End Sub

    Function log(s As String) As Boolean
        My.Computer.FileSystem.WriteAllText(My.Computer.FileSystem.SpecialDirectories.MyDocuments & "/logRomexisExport.txt", s & "(" & DateTime.Now & ") " & vbNewLine, True)
        Return True
    End Function

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'SetLicense()
        'Dim ra As New RasterCodecs
        'Dim im = ra.Load("C:\Users\developer\Documents\Migration\Running Offices\26 Smyl Dr. Grant\tmp\I2230.img")
        'ra.Save(im, "C:\Users\developer\Documents\Migration\Running Offices\26 Smyl Dr. Grant\tmp\I2230.jpg", RasterImageFormat.Jpeg, 24)
        'Dim bmp = New Bitmap("C:\Users\developer\Documents\Migration\Running Offices\26 Smyl Dr. Grant\tmp\I2230.jpg")
        'bmp.RotateFlip(RotateFlipType.Rotate270FlipNone)
        'bmp.Save("C:\Users\developer\Documents\Migration\Running Offices\26 Smyl Dr. Grant\tmp\I2230-rot270.jpg")

        'Dim bmp1 = New Bitmap("C:\Users\developer\Documents\Migration\Running Offices\26 Smyl Dr. Grant\tmp\I2230.jpg")
        'bmp1.RotateFlip(RotateFlipType.Rotate90FlipNone)
        'bmp1.Save("C:\Users\developer\Documents\Migration\Running Offices\26 Smyl Dr. Grant\tmp\I2230-rot90.jpg")

        'Dim bmp2 = New Bitmap("C:\Users\developer\Documents\Migration\Running Offices\26 Smyl Dr. Grant\tmp\I2230.jpg")
        'bmp2.RotateFlip(RotateFlipType.Rotate270FlipNone)
        'bmp2.Save("C:\Users\developer\Documents\Migration\Running Offices\26 Smyl Dr. Grant\tmp\I2230-rot270.jpg")

        'Dim bmp3 = New Bitmap("C:\Users\developer\Documents\Migration\Running Offices\26 Smyl Dr. Grant\tmp\I2230.jpg")
        'bmp3.RotateFlip(RotateFlipType.Rotate90FlipX)
        'bmp3.Save("C:\Users\developer\Documents\Migration\Running Offices\26 Smyl Dr. Grant\tmp\I2230-rot90flipx.jpg")
    End Sub
End Class
