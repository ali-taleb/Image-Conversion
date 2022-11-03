Imports System.IO
Imports Ionic.Zip
Public Class Form2
    Dim files As String()
    Dim pathToExtraction = "D:\"
    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'For i = 46761 To 62512
        '    If i <> 48 Then


        '        Try
        '            If i Mod 10000 = 0 Then
        '                MsgBox("reached" & i)
        '            End If
        '            Dim extract = unzip("C:\Users\developer\Desktop\ddddd\DC000105C.zip", i, My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\DentrixDocExtraction")
        '            MsgBox("found " & i)
        '        Catch ex As Exception

        '        End Try
        '    End If
        'Next
        'MsgBox("none")
        'Dim PASS_KEYS = {"A trickling ", "noise and a ", "strong chemi", "cal smell pr", "ompted them", "to look arou", "nd.  Goodboy", " Bindle Feat", "herstone was", " squatting ", "with an air ", "of sheepish ", "innocence al", "ongside what", " was not so", "much a stain",
        '    " on the carp", "et as a hole", " in the floo", "r.  A few w", "isps of smok", "e were curli", "ng up from t", "he edges."}

        'Dim j = 0
        'While j < 24
        '    If j Mod 1000 = 999 Then
        '        MsgBox(j)
        '    End If
        '    Try
        '        For i = 0 To 10

        '            Try





        '                Dim zip1 As ZipFile = ZipFile.Read("C:\Users\developer\Desktop\3\3~2.zip")
        '                zip1.Password = PASS_KEYS(j) & i

        '                'AddHandler zip1.ExtractProgress, AddressOf MyExtractProgress
        '                Dim ee As ZipEntry
        '                ' here, we extract every entry, but we could extract    
        '                ' based on entry name, size, date, etc.   

        '                For Each ee In zip1
        '                    ee.Extract("C:\Users\developer\Desktop\3", ExtractExistingFileAction.OverwriteSilently)
        '                    MsgBox("found " & j)
        '                Next
        '            Catch ex As Exception

        '            End Try
        '        Next

        '    Catch ex As Exception
        '        j = j + 1
        '    End Try


        'End While

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click


        If (FolderBrowserDialog1.ShowDialog() = DialogResult.OK) Then
            Label2.Text = FolderBrowserDialog1.SelectedPath
        Else
            Exit Sub
        End If
        If Not Directory.Exists(Label2.Text) Then
            MsgBox("Please select a valid Directory")
            files = Nothing
            Exit Sub
        End If
        files = My.Computer.FileSystem.GetFiles(Label2.Text, FileIO.SearchOption.SearchAllSubDirectories).ToArray
        Label1.Text = files.Count
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If files Is Nothing Then
            Exit Sub
        End If
        If files.Count < 1 Then
            Exit Sub
        End If
        BackgroundWorker1.RunWorkerAsync()
    End Sub

    Private Sub BackgroundWorker1_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        If Not File.Exists(My.Computer.FileSystem.SpecialDirectories.Desktop & "\nameID.csv") Then
            MsgBox("the file that contains docid , name and IDs of patients does not exist on desktop")
            Exit Sub
        End If
        Dim arr = File.ReadAllLines(My.Computer.FileSystem.SpecialDirectories.Desktop & "\nameID.csv")
        Dim coch = 0
        Dim coche = "The following rows in nameID.csv have erros"
        For ch = 0 To arr.Count - 1

            If Split(arr(ch), "|").Count < 6 Then
                coch = coch + 1
                coche = coche & "  : " & ch + 1
            End If
        Next
        If coch > 0 Then
            MsgBox(coche)
            Exit Sub
        End If
        'Label3.Invoke(Sub()
        '                  Label3.Text = "renaming"
        '              End Sub)
        'For i = 0 To files.Count - 1
        '    Label4.Invoke(Sub()
        '                      Label4.Text = i + 1
        '                  End Sub)
        '    Try
        '        Dim fi As FileInfo = New FileInfo(files(i))
        '        My.Computer.FileSystem.RenameFile(files(i), files(i).Replace(fi.DirectoryName & "\", "").Replace(".ZED", ".zip").Replace(".ZENI", ".zip"))
        '    Catch ex As Exception
        '        MsgBox(ex.ToString)
        '    End Try



        'Next

        Label3.Invoke(Sub()
                          Label3.Text = "Renaming"
                      End Sub)
        For i = 0 To files.Count - 1
            Label4.Invoke(Sub()
                              Label4.Text = i + 1
                          End Sub)

            'My.Computer.FileSystem.RenameFile(files(i), files(i).Replace(".ZED", ".zip"))
            'Dim extract = True
            'Try

            '    extract = unzip(files(i).Replace(".ZED", ".zip").Replace(".ZENI", ".zip"), getID(files(i)), pathToExtraction & "\DentrixDocExtraction")

            'Catch ex As Exception
            '    Label8.Invoke(Sub()
            '                      Label8.Text = Val(Label8.Text) + 1
            '                  End Sub)
            '    log("error 65866 in extracting " & files(i) & " - " & ex.ToString)
            'End Try


            Dim id = ""
            Dim extensionn = ""
            Dim fileid = ""
            Try
                Dim fiInfo As New FileInfo(files(i))
                Dim lastin = fiInfo.DirectoryName().LastIndexOf("\")
                fileid = fiInfo.DirectoryName().Substring(lastin + 1, fiInfo.DirectoryName().Length - lastin - 1)

                id = getIDFromarr(fileid, arr)

                extensionn = fiInfo.Extension()

            Catch ex As Exception
                MsgBox(ex.ToString)
            End Try

            If id = "" Then
                Label6.Invoke(Sub()
                                  Label6.Text = Val(Label6.Text) + 1
                              End Sub)
                log("error 57958 doc id was not found in file " & fileid)
            Else
                'Dim dir = pathToExtraction & "\DentrixDocExtraction" & "\" & getID(files(i))

                'Dim f = My.Computer.FileSystem.GetFiles(dir, FileIO.SearchOption.SearchAllSubDirectories).Where(Function(saa) Path.GetExtension(saa).ToLower = ".dcp").ToArray

                My.Computer.FileSystem.RenameFile(files(i), id & "-" & fileid & "-" & i & extensionn)
                'For j = 0 To f.Count - 1
                '    Dim fii As New FileInfo(f(j))
                '    Dim d = My.Computer.FileSystem.ReadAllText(fii.DirectoryName & "\MetaData")

                '    My.Computer.FileSystem.RenameFile(f(j), id & "-" & getID(files(i)) & "-" & j & Strings.Left(metadata(d)(1), 35) & Strings.Left(metadata(d)(0), 35))
                'Next

            End If



        Next


    End Sub
    Function getID(s As String) As Integer
        Dim code = s.Substring(s.Length - 8, 4)
        'MsgBox(code)
        Dim answer(4) As Integer
        For i = 0 To code.Length - 1
            If IsNumeric(code.ToCharArray()(i)) Then
                answer(i) = Val(code.ToCharArray()(i))
            Else
                answer(i) = Asc(code.ToCharArray()(i).ToString.ToUpper) - 55
            End If
        Next
        Return answer(0) * 36 * 36 * 36 + answer(1) * 36 * 36 + answer(2) * 36 + answer(3)
    End Function
    Function unzip(s As String, id As Integer, TargetDir As String) As Boolean
        My.Computer.FileSystem.CreateDirectory(TargetDir & "\" & id)
        Dim zip1 As ZipFile = ZipFile.Read(s)
        zip1.Password = getPass(Val(id))

        'AddHandler zip1.ExtractProgress, AddressOf MyExtractProgress
        Dim e As ZipEntry
        ' here, we extract every entry, but we could extract    
        ' based on entry name, size, date, etc.   

        For Each e In zip1
            e.Extract(TargetDir & "\" & id, ExtractExistingFileAction.OverwriteSilently)

        Next

        Return True
    End Function
    Function getPass(id As Integer) As String
        Dim PASS_KEYS = {"A trickling ", "noise and a ", "strong chemi", "cal smell pr", "ompted them", "to look arou", "nd.  Goodboy", " Bindle Feat", "herstone was", " squatting ", "with an air ", "of sheepish ", "innocence al", "ongside what", " was not so", "much a stain",
            " on the carp", "et as a hole", " in the floo", "r.  A few w", "isps of smok", "e were curli", "ng up from t", "he edges."}
        Dim num As Integer = id Mod 10
        Dim str = PASS_KEYS(id Mod 24)
        Return str & num
    End Function
    Function getIDFromarr(docID As Integer, arr As String()) As String
        For i = 0 To arr.Length - 1
            If Val(arr(i).Split("|")(0).Trim) = docID.ToString Then
                Return arr(i).Split("|")(1).Trim
            End If
        Next
        Return ""
    End Function
    Function log(s As String) As Boolean
        My.Computer.FileSystem.WriteAllText(My.Computer.FileSystem.SpecialDirectories.MyDocuments & "/logDentrix.txt", s & "(" & DateTime.Now & ") " & vbNewLine, True)
        Return True
    End Function
    Function metadata(s As String) As String()
        Dim answer(2) As String
        answer(0) = ""
        answer(1) = ""

        If s.Contains("<TY V=") Then

            Dim t = s.Substring(s.IndexOf("<TY V=")).Split("""")(1)

            If t = "1" Then
                answer(0) = ".png"
            ElseIf t = "2" Then
                answer(0) = ".pdf"
            ElseIf t = "3" Then
                answer(0) = ".doc"
            ElseIf t = "4" Then
                answer(0) = ".xls"
            ElseIf t = "10" Then
                answer(0) = ".html"
            Else
                answer(0) = ".unknown"
            End If

            If s.Contains("<OFN V=") Then
                answer(1) = s.Substring(s.IndexOf("<OFN V=")).Split("""")(1).Replace(answer(0), "")
            End If
            If s.Contains("<OFN V =") Then
                answer(1) = s.Substring(s.IndexOf("<OFN V =")).Split("""")(1).Replace(answer(0), "")
            End If
        ElseIf s.Contains("<TY V =") Then
            Dim t = s.Substring(s.IndexOf("<TY V =")).Split("""")(1)

            If t = "1" Then
                answer(0) = ".png"
            ElseIf t = "2" Then
                answer(0) = ".pdf"
            ElseIf t = "3" Then
                answer(0) = ".doc"
            ElseIf t = "4" Then
                answer(0) = ".xls"
            ElseIf t = "10" Then
                answer(0) = ".html"
            Else
                answer(0) = ".unknown"
            End If

            If s.Contains("<OFN V=") Then
                answer(1) = s.Substring(s.IndexOf("<OFN V=")).Split("""")(1).Replace(answer(0), "")
            End If
            If s.Contains("<OFN V =") Then
                answer(1) = s.Substring(s.IndexOf("<OFN V =")).Split("""")(1).Replace(answer(0), "")
            End If
        Else

            answer(0) = ".unknown"
            answer(1) = ""

        End If
        Return answer
    End Function

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim s = "-----in the migration folder / code there is a folder named dentrix. Launch the server by starting the file faircom.exe in the server folder." & vbNewLine &
            "Open c-tree SQL Explorer  and create new database in the name of office then refresh" & vbNewLine &
            "in the folder office.DBS , copy paste the following 6 files DCAttach,DCDoc,DCDocTyp" & vbNewLine &
            "now open sqlisamexplorer and right click attach the following tables DCAttach,DCDoc,DCDocTyp" & vbNewLine &
            "open c-treeAce SQL Explorer select the Scripts tab run the command : " & vbNewLine &
            "select a.docid , a.attachtoentityid , c.automodifiedtimestamp ,b.name as category, c.descript as name  from dcdoc c inner join dcattach a on a.docid=c.docid left join dcdoctyp b on b.dcdoctypeid = c.doctypeid " & vbNewLine &
            "* be carefull to set Max Rows to all (to view all rows)" & vbNewLine &
            "copy the text viewed in the result to a file" & vbNewLine &
            "VI: replace in notepad++ the double space ('  ') by nothing , to remove spaces between words and the comma by dot" & vbNewLine &
            "save the file as nameID.csv" & vbNewLine &
            "be carefull the date is in the format mm/dd/yyyy" & vbNewLine &
            "create a folder in the name of office.DBS , then copy paste the following 6 files DCAttach,DCDoc,DCDocTyp" & vbNewLine &
            "---------other version of instructions: download the folder of database DentrixSQL and put it in the folder C:\FairCom\V11.0.0\winX64\bin\ace\sql\data " & vbNewLine &
            "Do not change the files FSD and FCS" & vbNewLine &
            "go to ctree sql explorer and attach a new database , its name is DentrixSQL , then refresh and close sql explorer" & vbNewLine &
            "now open Ctreeace ISAM explorer and then for the table : Document,DocCat and DocAttach and the preview tabe records " & vbNewLine &
            "select all and then copy and paste in a text file . Save . Change file extension to xls . open using Microsoft excel and save as Excel workbook with file extension (xlsx)" & vbNewLine &
            "now open SQL create database and then import the files Document.xlsx , DocAttach.xlsx , DocCat.xlsx " & vbNewLine &
            " when importing make sure you choose excel in the source data and then Excel 2003. Also DeSelect the option of titles in first row" & vbNewLine &
            "run the command :   
            select a.F3 as  docid, a.F5 as attachtoentityid , FORMAT(a.F1, 'MM/dd/yyyy hh:mm:ss') as automodifiedtimestamp,Replace( b.F3,',','_') as category, Replace( c.F4,',','_') as [name] ,'' from document$ c inner join docattach$ a on a.F3=c.F2 left join doccat$ b on b.F2 = c.F3   
            VI: open in notepad++ and replace comma by |  and then add this to the first row 
            docid| attachtoentityid| automodifiedtimestamp| category|  name |
            save the file as nameID.csv
            be carefull the date is in the format mm/dd/yyyy" & vbNewLine &
            "--------Old instructions:
            download the dentrix database
            place the database in the path C:\FairCom\V11.5.0\winX64\bin\ace\sql\database
            open ctree , Tools , c-treeAce SQL Explorer
            open DentrixSQL database in the tree , open the tables go to  the table u want (document , doccat , docattach)
            select the Scripts tab run the command :   
            select a.docid , a.attachtoentityid , c.automodifiedtimestamp ,b.name as category, c.descript as name  from document c inner join docattach a on a.docid=c.docid left join doccat b on b.dcdoctypeid = c.doctypeid 
            * be carefull to set Max Rows to all (to view all rows)
            copy the text viewed in the result to a file 
            VI: replace in notepad++ the double space ('  ') by nothing , to remove spaces between words and the comma by dot
            save the file as nameID.csv
            be carefull the date is in the format mm/dd/yyyy
            "

        MsgBox(s)

    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        MsgBox("please in the following browser dialog that will appear select the folder of extraction named DentrixDocExtraction")
        Dim fil As String()
        If (FolderBrowserDialog1.ShowDialog() = DialogResult.OK) Then
            fil = My.Computer.FileSystem.GetFiles(FolderBrowserDialog1.SelectedPath, FileIO.SearchOption.SearchAllSubDirectories).Where(Function(saa) Path.GetExtension(saa).ToLower = ".dcp").ToArray
        Else
            Exit Sub
        End If
        If Not Directory.Exists(FolderBrowserDialog1.SelectedPath) Then
            MsgBox("Please select a valid Directory")
            files = Nothing
            Exit Sub
        End If
        Dim dest = My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\Oryx Dentrix Unknown"
        Directory.CreateDirectory(dest)
        For i = 0 To fil.Count - 1

            Dim finf As New FileInfo(fil(i))
            Dim d = My.Computer.FileSystem.ReadAllText(finf.DirectoryName & "\MetaData")
            Dim dir = finf.DirectoryName
            My.Computer.FileSystem.CopyFile(fil(i), dest & "\" & i & metadata(d)(0))


        Next
        MsgBox("done")
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        If (FolderBrowserDialog2.ShowDialog() = DialogResult.OK) Then
            pathToExtraction = FolderBrowserDialog2.SelectedPath
        Else
        End If
    End Sub
End Class
