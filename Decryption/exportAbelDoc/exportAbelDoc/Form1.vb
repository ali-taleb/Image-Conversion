
Imports System.IO
Imports System.Threading
Imports System.Reflection

Public Class Form1
    Dim files As String()
    Dim th1 As Thread
    Dim output = My.Computer.FileSystem.SpecialDirectories.MyDocuments



    Function log(s As String) As Boolean
        My.Computer.FileSystem.WriteAllText(My.Computer.FileSystem.SpecialDirectories.MyDocuments & "/logAbel.txt", s & "(" & DateTime.Now & ") " & vbNewLine, True)
        Return True
    End Function

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim s = "DO NOT USE ORGIGINAL FILES.Delete thumb files from folder of data. Download sql database its name is Abel" & vbNewLine &
            "run this command    select Document.ID,replace(replace(concat(DocumentType.FriendlyName,' - ', Description),',','!@#$%'),'  ','') from Document left join DocumentType on Document.DocTypeID = DocumentType.ID; " &
            "in notepad replace , by | then replace !@#$% by , then replace double quotations by nothing , save the result to a file named   abelDocumentDescription.csv " &
            " select folder of Images run this software and upload using eaglesoft doc upload"

        MsgBox(s)
    End Sub


    Private Sub loadFiles()

        Label11.Invoke(Sub()
                           Label11.Text = "WAIT ..."
                       End Sub)
        Try
            files = My.Computer.FileSystem.GetFiles(Label2.Text, FileIO.SearchOption.SearchAllSubDirectories).ToArray
            Label4.Invoke(Sub()
                              Label4.Text = files.Count
                          End Sub)
        Catch ex As Exception
            MsgBox(ex, ToString)
        End Try
        Label11.Invoke(Sub()
                           Label11.Text = "READY ..."
                       End Sub)


    End Sub

    Private Sub Button2_Click_1(sender As Object, e As EventArgs) Handles Button2.Click
        th1 = New Thread(AddressOf exec1)
        Dim stateThread As New Thread(AddressOf state)
        th1.Start()
        stateThread.Start()

    End Sub

    Private Sub exec1()
        Try

            Dim Processed = 0
            Dim errorFiles = 0
            Dim Ignored = 0
            Dim NotFound = 0

            Label2.Invoke(Sub()
                              Label2.Text = Processed
                          End Sub)
            Label3.Invoke(Sub()
                              Label3.Text = Ignored
                          End Sub)
            Label4.Invoke(Sub()
                              Label4.Text = errorFiles
                          End Sub)
            Label9.Invoke(Sub()
                              Label9.Text = NotFound
                          End Sub)



            If Not File.Exists(My.Computer.FileSystem.SpecialDirectories.Desktop & "\abelDocumentDescription.csv") Then
                MsgBox("File abelDocumentDescription.csv on desktop was not found")

                Exit Sub
            End If
            Dim arrDesc = File.ReadAllLines(My.Computer.FileSystem.SpecialDirectories.Desktop & "\abelDocumentDescription.csv")


            Label11.Invoke(Sub()
                               Label11.Text = "exporting ..."
                           End Sub)
            For i = 0 To files.Count - 1



                Dim fn As New FileInfo(files(i))
                If files(i).Contains("-") And files(i).IndexOf("-") <> files(i).LastIndexOf("-") Or (files(i).Contains("_") And files(i).IndexOf("_") <> files(i).LastIndexOf("_")) Then

                    Try
                        Dim fname = fn.Name
                        Dim Patientid = getIDFromPath(fn.Directory.FullName)
                        Dim description = getDescription(fn.Name.Replace(Path.GetExtension(files(i)), ""), arrDesc)
                        If description = "Not Found" Or Patientid = "" Then ' btw you can remove this and upload docs that were not found in database too
                            NotFound = NotFound + 1
                            log("The file has no description: " & files(i))
                            My.Computer.FileSystem.RenameFile(files(i), fn.Name.Replace("-", "@")) 'to ignore thefile when uploading
                        Else
                            Dim dt = fn.LastWriteTime
                            Dim docDate = dt.Month & "/" & dt.Day & "/" & dt.Year
                            Dim ImageID = fname(2) 'not used
                            My.Computer.FileSystem.RenameFile(files(i), Patientid & "-" & i & "-" & i & Path.GetExtension(files(i)))
                            My.Computer.FileSystem.WriteAllText(files(i).Replace(fn.Name, Patientid & "-" & i & ".txt"), ";;;" & description & ";;;" & docDate, False)
                            Processed = Processed + 1
                        End If
                    Catch ex As Exception
                        log("error in file " & files(i) & " " & ex.ToString)
                        errorFiles = errorFiles + 1

                    End Try


                Else
                    log("The file was ignored: " & files(i))
                    Ignored = Ignored + 1
                End If

                Label2.Invoke(Sub()
                                  Label2.Text = Processed
                              End Sub)
                Label3.Invoke(Sub()
                                  Label3.Text = Ignored
                              End Sub)
                Label4.Invoke(Sub()
                                  Label4.Text = errorFiles
                              End Sub)
                Label9.Invoke(Sub()
                                  Label9.Text = NotFound
                              End Sub)
            Next
            MsgBox("Done")
            Label11.Invoke(Sub()
                               Label11.Text = "Done"
                           End Sub)
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub

    Private Sub state()
        Dim i = 1

        While th1.ThreadState = ThreadState.Running Or th1.ThreadState = ThreadState.Suspended Or th1.ThreadState = ThreadState.WaitSleepJoin
            'MsgBox("pr")
            Dim tex = "processing"
            For j = 0 To i
                tex = tex & " ."
            Next
            Label11.Invoke(Sub()
                               Label11.Text = tex
                           End Sub)
            Thread.Sleep(500)
            i = (i + 1) Mod 5
        End While
        Label11.Invoke(Sub()
                           Label11.Text = "stopped"
                       End Sub)
    End Sub

    Function getIDFromPath(pathDirec As String) As String
        Dim s = pathDirec
        Dim id = ""
        If s.Contains("\") Then
            id = s.Substring(s.LastIndexOf("PN")).Replace("PN", "").Replace("\", "")
        Else
            id = s.Substring(s.LastIndexOf("PN")).Replace("PN", "").Replace("/", "")
        End If
        If IsNumeric(id) Then
            Return Val(id)
        Else
            Return ""
        End If
    End Function

    Function getDescription(path As String, arr As String()) As String
        'path here is the filname only and without extension
        For i = 0 To arr.Count - 1
            If arr(i).Contains("|") Then
                Dim name = arr(i).Split("|")(0).ToLower

                If name = Strings.Right(path, 36).ToLower Or name = Strings.Right(path, 36).Replace("-", "_").ToLower Then
                    Return arr(i).Split("|")(1)
                End If
            End If
        Next
        Return "Not Found"
    End Function

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If (FolderBrowserDialog1.ShowDialog() = DialogResult.OK) Then
            files = My.Computer.FileSystem.GetFiles(FolderBrowserDialog1.SelectedPath, FileIO.SearchOption.SearchAllSubDirectories).ToArray
        Else
            Exit Sub
        End If
        Label1.Text = files.Count
    End Sub

End Class
