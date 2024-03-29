﻿Imports System
Imports System.IO.MemoryMappedFiles
Imports System.Threading


Partial Public Class frmPS4Twitch

    Dim lastping As DateTime

    Dim gh As IntPtr = IntPtr.Zero
    Dim rph As IntPtr = IntPtr.Zero
    Dim fcAddr As IntPtr = IntPtr.Zero

    Dim game As String = "sunblaze"

    Dim ctrlStyle As String = ""
    Dim ctrlType As String = ""


    Dim mmf As MemoryMappedFile
    Dim mmfa As MemoryMappedViewAccessor

    Dim timerfixer = 1
    'Dim frametime = 33333 'in microseconds
    Dim frametime = 16667

    Dim microTimer As MicroLibrary.MicroTimer = New MicroLibrary.MicroTimer()

    Dim recurse = 0
    Dim outspamcheck = 0


    Private Sub TimerPress()

        AddHandler microTimer.MicroTimerElapsed, AddressOf press
        microTimer.Interval = frametime
        microTimer.Enabled = True


    End Sub
    Private Sub press()
        Dim repeat = 0
        Do
            Select Case game
                Case "gamecube"
                    btnPress__Gamecube()
                Case "ps4"
                    btnPress__PS4()
                Case "xb1"
                    repeat = btnPress__XB1()

                Case "bloodbornedemake"
                    btnPress_BloodborneDemake()
                Case "celeste"
                    btnPress_Celeste()
                Case "crashbandicootwoc"
                    btnPress_CrashBandicootWoC()
                Case "celeste"
                    btnPress_Celeste()
                Case "eldenring"
                    repeat = btnPress_EldenRing()
                Case "endisnigh"
                    repeat = btnPress_EndIsNigh()
                Case "jumpking"
                    btnPress_JumpKing()
                Case "pikuniku"
                    repeat = btnPress_Pikuniku()
                Case "sunblaze"
                    repeat = btnPress_Sunblaze()
                Case "terraria"
                    btnPress_Terraria()
            End Select
        Loop While repeat = 1
    End Sub

    Private Sub execCMD(user As String, role As String, cmd As String)

        Select Case game
            Case "gamecube"
                execCMD__Gamecube(user, role, cmd)
            Case "ps4"
                execCMD__PS4(user, role, cmd)
            Case "xb1"
                execCMD__XB1(user, role, cmd)

            Case "bloodbornedemake"
                execCMD_BloodborneDemake(user, role, cmd)
            Case "celeste"
                execCMD_Celeste(user, role, cmd)
            Case "crashbandicootwoc"
                execCMD_CrashBandicootWoC(user, role, cmd)
            Case "celeste"
                execCMD_Celeste(user, role, cmd)
            Case "eldenring"
                execCMD_EldenRing(user, role, cmd)
            Case "endisnigh"
                execCMD_EndIsNigh(user, role, cmd)
            Case "jumpking"
                execCMD_JumpKing(user, role, cmd)
            Case "pikuniku"
                execCMD_Pikuniku(user, role, cmd)
            Case "terraria"
                execCMD_Terraria(user, role, cmd)
            Case "sunblaze"
                execCMD_Sunblaze(user, role, cmd)
        End Select
    End Sub

    Private Sub ProcessCMD(user As String, role As String, cmd As String)


        If user = "wulf2kbot" Then Return


        Dim tmpuser = user
        Dim tmpcmd = cmd
        Dim CMDmulti As Integer = 1


        tmpcmd = tmpcmd.ToLower


        If tmpcmd.Contains("wulf") Then
            Return
        End If

        If tmpcmd = "hello" Then
            outputChat("Hello.")
            Return
        End If






        'authlist
        If tmpcmd = "#authlist" Then
            Dim txt As String = ""
            For Each usr In authlist
                txt += usr + ", "
            Next
            outputChat(txt)
            Return
        End If

        'authadd
        If tmpcmd.IndexOf("#authadd") = 0 Then
            'Is user issuing the command authed already?
            If Not authlist.Contains(tmpuser) Or tmpuser = "nightbot" Then
                outputChat("Only authed users can auth users.")
                Return
            End If

            'Was a user specified?
            Dim usr As String
            If tmpcmd.Contains(" ") Then
                usr = tmpcmd.Split(" ")(1)
            Else
                Return
            End If

            'Is the usr alphanumeric?
            For Each c In usr
                If Not ((Asc(c) >= 48 And Asc(c) <= 57) Or (Asc(c) >= 97 And Asc(c) <= 122)) Then
                    outputChat("Invalid char in #authadd.")
                    Return
                End If
            Next

            'Does the usr already exist?
            If authlist.Contains(usr) Then
                outputChat("User already authed.")
                Return
            End If

            authlist.Add(usr)
            authlist.Sort()
            IO.File.WriteAllLines("authlist.txt", authlist.ToArray())
            outputChat($"{usr} added to authlist.")

            Return
        End If

        'authdel
        If tmpcmd.IndexOf("#authdel") = 0 Then
            'Is user issuing the command authed already?
            If Not authlist.Contains(tmpuser) Or tmpuser = "nightbot" Then
                outputChat("Only authed users can deauth users.")
                Return
            End If

            'Was a user specified?
            Dim usr As String
            If tmpcmd.Contains(" ") Then
                usr = tmpcmd.Split(" ")(1)
            Else
                Return
            End If

            'Is the usr alphanumeric?
            For Each c In usr
                If Not ((Asc(c) >= 48 And Asc(c) <= 57) Or (Asc(c) >= 97 And Asc(c) <= 122)) Then
                    outputChat("Invalid char in #authdel.")
                    Return
                End If
            Next

            'Does the usr already exist?
            If Not authlist.Contains(usr) Then
                outputChat("User is already deauthed.")
                Return
            End If

            authlist.Remove(usr)
            IO.File.WriteAllLines("authlist.txt", authlist.ToArray())
            outputChat($"{usr} removed from authlist.")

            Return
        End If







        Try
            Dim restricted As New List(Of String) From
                {"#authlist", "#authadd", "#authdel",
                "#game",
                "#macrolist", "#macroshow", "#macroadd", "#macrodel", "#macroedit"}


            'game
            If tmpcmd.IndexOf("#game") = 0 Then
                outputChat("Functionality temporarily neutered.")
                'Debug macros failing to load from txt file when using this cmd before re-enabling
                Return

                If tmpcmd.Contains(" ") Then
                    If authlist.Contains(tmpuser) Then
                        game = tmpcmd.Split(" ")(1).ToLower()
                        game = game.Replace(".", "")
                        game = game.Replace("\", "")
                        game = game.Replace("/", "")

                        For Each c In game
                            If Not ((Asc(c) >= 48 And Asc(c) <= 57) Or (Asc(c) >= 97 And Asc(c) <= 122)) Then
                                outputChat("Invalid char in game name.")
                                Return
                            End If
                        Next

                        If gamelist.Contains(game) Then
                            macros.Clear()

                            Dim lines As New List(Of String)
                            'Load macros
                            Try
                                lines = IO.File.ReadLines($"{game}-macros.txt")
                                For Each line In lines
                                    macros.Add(New KeyValuePair(Of String, String)(line.Split("~")(0), line.Split("~")(1)))
                                Next
                            Catch ex As Exception

                            End Try
                            outputChat($"Controls now set to {game}")
                            Return
                        Else 'if game not in gamlist
                            outputChat($"Game {game} not recognized.")
                        End If 'end if game in gamelist

                    Else 'if authlist not contain tmpuser
                        outputChat($"{tmpuser} not authorized to change controls.")
                        Return
                    End If 'end if authlist contains tmpuser
                Else 'if tmpcmd had no space
                    outputChat($"Current game is {game}.")
                End If 'end if tmpcmd has space
                Return
            End If

            'macrolist
            If tmpcmd = "#macrolist" Then
                If macros.Count > 0 Then
                    Dim txt As String = ""
                    For Each pair As KeyValuePair(Of String, String) In macros
                        txt += pair.Key + ", "
                    Next
                    outputChat(txt)
                Else
                    outputChat($"No macros found for {game}.")
                End If

                Return
            End If

            'macroshow
            If tmpcmd.IndexOf("#macroshow") = 0 Then
                For Each pair As KeyValuePair(Of String, String) In macros
                    If tmpcmd.Contains(" ") AndAlso pair.Key = tmpcmd.Split(" ")(1) Then
                        outputChat(pair.Value)
                        Return
                    End If
                Next
                outputChat("Macro not found.")
                Return
            End If

            'macroadd
            If tmpcmd.IndexOf("#macroadd") = 0 Then
                Dim macro As String = ""
                Dim commands As String = ""

                If tmpcmd.Contains(" ") Then
                    macro = tmpcmd.Split(" ")(1)
                    commands = tmpcmd.Replace($"#macroadd {macro}", "")
                    'commands = commands.Replace(" ", "")
                End If

                If Not macro.IndexOf("#") = 0 Then
                    outputChat("Macros must begin with #.")
                    Return
                End If

                If macro.Length > 20 Then
                    outputChat("Macro names must be < 20 characters.")
                    Return
                End If

                For Each c In macro
                    If Not ((Asc(c) >= 48 And Asc(c) <= 57) Or (Asc(c) >= 97 And Asc(c) <= 122) Or (Asc(c) = 35)) Then
                        outputChat("Invalid char in macro name.")
                        Return
                    End If
                Next

                For Each pair As KeyValuePair(Of String, String) In macros
                    If pair.Key = macro Then
                        outputChat($"{pair.Key} already exists.")
                        Return
                    End If
                Next


                For Each cmd In restricted
                    If commands.Contains(cmd) Then
                        outputChat("Control macros cannot be macro'd.")
                        Return
                    End If
                Next

                macros.Add(New KeyValuePair(Of String, String)(macro, commands))

                'Save macros
                Dim lines As New List(Of String)
                For Each pair As KeyValuePair(Of String, String) In macros
                    lines.Add($"{pair.Key}~{pair.Value}")
                Next
                lines.Sort()

                IO.File.WriteAllLines($"{game}-macros.txt", lines)

                outputChat($"{macro} added.")
                Return
            End If

            'macroedit
            If tmpcmd.IndexOf("#macroedit") = 0 Then
                Dim macro As String = ""
                Dim commands As String = ""

                If tmpcmd.Contains(" ") Then
                    macro = tmpcmd.Split(" ")(1)
                    commands = tmpcmd.Replace($"#macroedit {macro}", "")
                    'commands = commands.Replace(" ", "")
                End If

                For Each cmd In restricted
                    If commands.Contains(cmd) Then
                        outputChat("Control macros cannot be macro'd.")
                        Return
                    End If
                Next

                For Each pair As KeyValuePair(Of String, String) In macros
                    If pair.Key = macro Then
                        macros.Remove(pair)
                        macros.Add(New KeyValuePair(Of String, String)(macro, commands))
                        'Save macros
                        Try
                            Dim lines As New List(Of String)
                            For Each subpair As KeyValuePair(Of String, String) In macros
                                lines.Add($"{subpair.Key}~{subpair.Value}")
                            Next
                            lines.Sort()
                            IO.File.WriteAllLines($"{game}-macros.txt", lines)

                            outputChat($"{macro} updated.")
                        Catch ex As Exception

                        End Try
                        Return
                    End If
                Next

                outputChat($"{macro} not found.")
                Return
            End If

            'macrodel
            If tmpcmd.IndexOf("#macrodel") = 0 Then
                Dim macro As String = ""

                If tmpcmd.Contains(" ") Then
                    macro = tmpcmd.Split(" ")(1)
                End If

                For Each pair As KeyValuePair(Of String, String) In macros
                    If pair.Key = macro Then
                        macros.Remove(pair)

                        'Save macros
                        Try
                            Dim lines As New List(Of String)
                            For Each subpair As KeyValuePair(Of String, String) In macros
                                lines.Add($"{subpair.Key}~{subpair.Value}")
                            Next
                            lines.Sort()
                            IO.File.WriteAllLines($"{game}-macros.txt", lines)

                            outputChat($"{macro} removed.")
                        Catch ex As Exception

                        End Try
                        Return
                    End If
                Next

                outputChat($"{macro} not found.")
                Return
            End If
        Catch ex As Exception
            outputChat($"Something macro related just tried to crash. {ex.Message}")
        End Try





        If tmpcmd.Contains("*") Then
            CMDmulti = Val(tmpcmd.Split("*")(1))
            If CMDmulti > 999 Then CMDmulti = 999
            If CMDmulti < 1 Then CMDmulti = 1
            For i = 1 To CMDmulti
                ProcessCMD(tmpuser, role, tmpcmd.Split("*")(0))
            Next
            Return
        End If


        'Allow multiple strings per line, with a multiplier on each
        If tmpcmd.Contains("\") Then
            tmpcmd = tmpcmd.Replace(" ", "")
            For Each part In tmpcmd.Split("\")
                ProcessCMD(tmpuser, role, part)
            Next
            Return
        End If



        'Loop entire string
        If tmpcmd.Contains("|") Then
            CMDmulti = Val(tmpcmd.Split("|")(1))

            If CMDmulti > 999 Then CMDmulti = 999
            If CMDmulti < 1 Then CMDmulti = 1
            For i = 0 To CMDmulti - 1
                ProcessCMD(tmpuser, role, tmpcmd.Split("|")(0))
            Next
            Return
        End If

        'Handle multi-command entries
        If tmpcmd.Contains(",") Then
            For Each part In tmpcmd.Split(",")
                'Prevent buffer overflow in RemotePlay memory
                tmpuser = Strings.Left(tmpuser, 15)
                part = part.Replace(" ", "")
                part = Strings.Left(part, 15)

                ProcessCMD(tmpuser, role, part)
            Next
            Return
        End If


        tmpcmd = tmpcmd.Replace(" ", "")


        'Handle command multipliers
        If tmpcmd.Length > 2 Then
            If IsNumeric(Strings.Right(tmpcmd, tmpcmd.Length - 1 - tmpcmd.LastIndexOf("x"))) And (tmpcmd.LastIndexOf("x") > tmpcmd.LastIndexOf("-")) Then

                CMDmulti = Val(Strings.Right(tmpcmd, tmpcmd.Length - 1 - tmpcmd.LastIndexOf("x")))
                If CMDmulti > 999 Then CMDmulti = 999
                If CMDmulti < 1 Then CMDmulti = 1
                tmpcmd = Microsoft.VisualBasic.Left(tmpcmd, tmpcmd.LastIndexOf("x"))
            End If
        End If



        'TODO: Improve this handling
        Dim shorttmpcmd As String
        If tmpcmd.Contains("-") Then
            shorttmpcmd = tmpcmd.Split("-")(0)
        Else
            shorttmpcmd = tmpcmd
        End If



        Select Case shorttmpcmd
            Case "reconnect1", "ss", "ls", "rs"
                If Not authlist.Contains(user) Then
                    outputChat("Command restricted.")
                    Return
                End If
            Case "options", "opt", "hopt", "start", "hstart"
                If Not authlist.Contains(user) Then
                    outputChat("Options menu restricted to pre-approved users.")
                    Return
                End If
            Case "home", "hhome", "pshome", "hpshome"
                If Not authlist.Contains(user) Then
                    outputChat("Uhh....  No.")
                    Return
                End If
            Case "dl", "hdl", "dr", "hdr"
                If Not authlist.Contains(user) Then
                    outputChat("DR/DL buttons restricted to pre-approved users.")
                    Return
                End If
            Case "tri", "htri"
                If Not authlist.Contains(user) Then

                End If
            Case "ca"
                SyncLock queuelock
                    QueuedInput.Clear()
                End SyncLock

                ProcessCMD(tmpuser, role, "nh")

                SyncLock presslock
                    microTimer.Enabled = False
                End SyncLock


            Case "c"
                For i = QueuedInput.Count - 1 To 0 Step -1
                    If QueuedInput(i).user = tmpuser Then
                        SyncLock queuelock
                            QueuedInput.RemoveAt(i)
                        End SyncLock
                    End If
                Next

                execCMD(tmpuser, role, "h-1")
                outputChat("All commands for " & tmpuser & " removed from queue.")
                Return
        End Select


        If tmpcmd.IndexOf("#") = 0 Then
            For Each pair As KeyValuePair(Of String, String) In macros
                If pair.Key = tmpcmd Then
                    tmpcmd = pair.Value
                    If recurse > 420 Then
                        Return
                    End If
                    If QueuedInput.Count < 42069 Then
                        recurse += 1
                        For i = 0 To CMDmulti - 1
                            ProcessCMD(tmpuser, role, tmpcmd)
                        Next
                    End If
                End If
            Next
            Return
        End If


        For i = 0 To CMDmulti - 1
            execCMD(tmpuser, role, tmpcmd)
        Next
    End Sub





    'TODO:  Change cmd portion of each entry to blank, then process every message's command
    'TODO:  This is to handle multiple new messages in a single check



    Dim tOne As New gcapiTitanOne.TitanOne
    Public P1output(36) As Byte


    Dim a As New asm

    Dim repeatCount As Integer

    Dim firstMessage As String = ""
    Dim lastMessage As String = ""


    Shared QueuedInput As New List(Of QdInput)




    Private WithEvents updTimer As New System.Windows.Forms.Timer()

    Private Declare Sub mouse_event Lib "user32.dll" (ByVal dwFlags As Integer, ByVal dx As Integer, ByVal dy As Integer, ByVal cButtons As Integer, ByVal dwExtraInfo As IntPtr)

    Const MOUSEEVENTF_LEFTDOWN As Integer = 2
    Const MOUSEEVENTF_LEFTUP As Integer = 4

    Public Structure RECT
        Public Left, Top, Right, Bottom As Integer
        Public Function ToRectangle() As Rectangle
            Return New Rectangle(Left, Top, Right - Left, Bottom - Top)
        End Function
    End Structure



    Dim authlist As New List(Of String)
    Dim trilist As New List(Of String)
    Dim ignorelist As New List(Of String)
    Dim gamelist As New List(Of String)

    Dim macros As List(Of KeyValuePair(Of String, String)) = New List(Of KeyValuePair(Of String, String))


    'Button values
    Public Const BTN_SHARE As UInt32 = &H1
    Public Const BTN_L3 As UInt32 = &H2
    Public Const BTN_R3 As UInt32 = &H4
    Public Const BTN_OPTIONS As UInt32 = &H8

    Public Const BTN_DPAD_UP As UInt32 = &H10
    Public Const BTN_DPAD_RIGHT As UInt32 = &H20
    Public Const BTN_DPAD_DOWN As UInt32 = &H40
    Public Const BTN_DPAD_LEFT As UInt32 = &H80

    Public Const BTN_L2 As UInt32 = &H100
    Public Const BTN_R2 As UInt32 = &H200
    Public Const BTN_L1 As UInt32 = &H400
    Public Const BTN_R1 As UInt32 = &H800

    Public Const BTN_TRIANGLE As UInt32 = &H1000
    Public Const BTN_O As UInt32 = &H2000
    Public Const BTN_X As UInt32 = &H4000
    Public Const BTN_SQUARE As UInt32 = &H8000

    Public Const BTN_PSHOME As UInt32 = &H10000
    Public Const BTN_TOUCHPAD As UInt32 = &H100000


    Dim dbgtime As DateTime = Now
    Dim presstimer As Integer = frametime
    Private pressthread As Thread

    Dim queuelock As New Object
    Dim presslock As New Object

    Dim wblock As New Object

    Shared boolHoldL1 = False
    Shared boolHoldL2 = False
    Shared boolHoldL3 = False
    Shared boolHoldR1 = False
    Shared boolHoldR2 = False
    Shared boolHoldR3 = False
    Shared boolHoldO = False
    Shared boolHoldSq = False
    Shared boolHoldTri = False
    Shared boolHoldX = False
    Shared boolHoldDU = False
    Shared boolHoldDD = False
    Shared boolHoldDL = False
    Shared boolHoldDR = False
    Shared boolHoldShare = False
    Shared boolHoldOpt = False

    Dim boolInvertLX = False
    Dim boolInvertLY = False
    Dim boolInvertRX = False
    Dim boolInvertRY = False

    Dim boolHoldAxis() As Boolean = {False, False, False, False}
    Dim boolHoldAxisVal() As Single = {0, 0, 0, 0}

    Private ctrlPtr As IntPtr

    Dim IRC As New IrcCon





    Private Sub frmPS4Twitch_Load(sender As Object, e As EventArgs) Handles MyBase.Load


        gamelist.Add("celeste")
        gamelist.Add("jumpking")




        'Control which chat users can execute privileged commands
        Dim lines
        Try
            lines = IO.File.ReadLines("authlist.txt")
            For Each line In lines
                authlist.Add(line)
            Next
        Catch ex As Exception

        End Try



        'Load macros
        Try
            lines = IO.File.ReadLines($"{game}-macros.txt")
            For Each line In lines
                macros.Add(New KeyValuePair(Of String, String)(line.Split("~")(0), line.Split("~")(1)))
            Next
        Catch ex As Exception

        End Try




        'macros.Add(New KeyValuePair(Of String, String)("#test1", "hx10,wb,wr,hx10"))
        'macros.Add(New KeyValuePair(Of String, String)("#test2", "hx5,wf,wl,hx5"))


    End Sub

    Private Sub frmPS4Twitch_FormClose(sender As Object, e As EventArgs) Handles MyBase.FormClosed
        microTimer.Enabled = False

    End Sub

    Private Sub btnJoinTwitchChat_Click(sender As Object, e As EventArgs) Handles btnJoinTwitchChat.Click
        IRC.server = "irc.chat.twitch.tv"
        IRC.port = 6667
        IRC.nick = "Wulf2kbot"
        IRC.user = "Wulf2kbot"
        IRC.pwd = My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\Wulf", "TwitchOAuth", Nothing)
        IRC.realname = ""
        IRC.hostname = "wulf2k.ca"

        lastping = DateTime.Now
        IRC.Connect()
        IRC.Join(txtTwitchChat.Text)




        'Timer to check chat messages
        Threading.Thread.Sleep(500)

        updTimer.Interval = 250
        updTimer.Enabled = True
        updTimer.Start()

    End Sub

    Private Sub updTimer_Tick() Handles updTimer.Tick

        If txtChat.Lines.Count > 20 Then
            txtChat.Lines = txtChat.Lines.Skip(1).Take(txtChat.Lines.Length - 1).ToArray()
        End If

        If outspamcheck > 0 Then outspamcheck -= 1
        If recurse > 0 Then recurse -= 1

        Dim msg() As String
        msg = IRC.Read

        Dim user
        Dim role = ""
        Dim cmd



        For Each line In msg
            line = line.TrimEnd

            If line.Length < 1000 Then
                txtChat.AppendText($"-> {line}")
                txtChat.AppendText(Environment.NewLine)
            End If

            If line.IndexOf("PING") = 0 Then
                IRC.Send("PONG")
                txtChat.AppendText($"<- PONG :tmi.twitch.tv")
                txtChat.AppendText(Environment.NewLine)
                lastping = DateTime.Now
            End If

            If line.IndexOf("PRIVMSG") > -1 Then
                user = line.Split("!")(0)
                user = user.split(":")(1)

                cmd = line.Split(":")(2)

                ProcessCMD(user, role, cmd)
            End If

        Next

        SyncLock queuelock
            If QueuedInput.Count > 0 Then
                microTimer.Enabled = True
            Else
                microTimer.Enabled = False
            End If
        End SyncLock


        If (DateTime.Now - lastping).TotalMinutes > 6 Then
            lastping = DateTime.Now
            IRC.Connect()
        End If

    End Sub

    Private Sub PushQ(ByRef buttons As Integer, RStickLR As Single, RStickUD As Single, LStickLR As Single,
                      LStickUD As Single, LTrigger As Single, RTrigger As Single, time As Integer, user As String,
                      cmd As String)
        SyncLock queuelock
            Try
                QueuedInput.Add(New QdInput() With {.buttons = buttons, .RstickLR = RStickLR, .RstickUD = RStickUD,
                                                .LStickLR = LStickLR, .LStickUD = LStickUD, .LTrigger = LTrigger,
                                                .RTrigger = RTrigger, .time = time, .user = user, .cmd = cmd})
            Catch ex As Exception

            End Try

        End SyncLock
    End Sub
    Private Sub PopQ()
        SyncLock queuelock
            QueuedInput.RemoveAt(0)
        End SyncLock
    End Sub

    Private Sub outputChat(ByVal txt As String)
        If txt(0) = "/" Or txt(0) = "." Then Return

        Dim maxChat As Int32 = 500
        Do
            If outspamcheck > 100 Then Return
            outspamcheck += 20

            If txt.Length > maxChat Then
                IRC.Send($"PRIVMSG {txtTwitchChat.Text} :{txt.Substring(0, maxChat)}")
                txt = txt.Replace(txt.Substring(0, maxChat), "")
            Else
                IRC.Send($"PRIVMSG {txtTwitchChat.Text} :{txt}")
                Exit Do
            End If
        Loop

    End Sub





    Private Sub Controller(buttons As Integer, RLR As Single, RUD As Single, LLR As Single, LUD As Single, LT As Single, RT As Single, hold As Integer, user As String, cmd As String)
        PushQ(buttons, RLR, RUD, LLR, LUD, LT, RT, hold, user, cmd)
    End Sub

    Private Sub TakeControl()


        tOne.Open()
        tOne.FindDevices()

        pressthread = New Thread(AddressOf TimerPress)
        pressthread.IsBackground = True
        pressthread.Start()

    End Sub

    Private Sub chkAttached_CheckedChanged(sender As Object, e As EventArgs) Handles chkAttached.CheckedChanged
        If chkAttached.Checked Then


            mmf = MemoryMappedFile.CreateOrOpen("TwitchControl", &H1000)
            mmfa = mmf.CreateViewAccessor()

            TakeControl()
        Else
            'RestoreControl()
        End If
    End Sub
End Class




Public Class QdInput
    Public buttons As Integer
    Public RstickLR As Single
    Public RstickUD As Single
    Public LStickLR As Single
    Public LStickUD As Single
    Public LTrigger As Single
    Public RTrigger As Single
    Public time As Integer
    Public user As String
    Public cmd As string
End Class


Public Class IrcCon
    Public server As String
    Public port As Integer
    Public nick As String
    Public user As String
    Public pwd As String
    Public realname As String
    Public hostname As String

    Private client = New Net.Sockets.TcpClient
    Private netstream As Net.Sockets.NetworkStream
    Private writer As IO.StreamWriter
    Private active As Boolean

    Public Sub Connect()
        client = New Net.Sockets.TcpClient
        client.connect(server, port)
        netstream = client.GetStream()
        writer = New IO.StreamWriter(netstream, Text.Encoding.ASCII)
        active = True

        Send($"PASS {pwd}")
        Send($"NICK {nick}")
        Send($"USER {user}")
    End Sub

    Public Sub Join(chan As String)
        
        Send($"JOIN {chan}")

    End Sub

    Public Sub Send(str As String)
        writer.WriteLine(str)
        writer.Flush
    End Sub
    Public Function Read() As String()

        If (netstream.DataAvailable) Then
            Dim b(client.ReceiveBufferSize) As Byte
            netstream.Read(b, 0, client.ReceiveBufferSize)
            Return System.Text.Encoding.ASCII.GetString(b).Split(Chr(&HA))
        End If

        Return {}
    End Function

    'Private function processLine


End Class



