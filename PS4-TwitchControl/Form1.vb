Imports System
Imports System.IO.MemoryMappedFiles
Imports System.Threading


Partial Public Class frmPS4Twitch

    Dim lastping As DateTime

    Dim gh As IntPtr = IntPtr.Zero
    Dim rph As IntPtr = IntPtr.Zero
    Dim fcAddr As IntPtr = IntPtr.Zero

    Dim ctrlStyle As String = ""
    Dim ctrlType As String = ""


    Dim mmf As MemoryMappedFile
    Dim mmfa As MemoryMappedViewAccessor

    Dim timerfixer = 1
    'Dim frametime = 33333 'in microseconds
    Dim frametime = 16667

    Dim microTimer As MicroLibrary.MicroTimer = New MicroLibrary.MicroTimer()





    Private Sub TimerPress()

        AddHandler microTimer.MicroTimerElapsed, AddressOf press
        microTimer.Interval = frametime
        microTimer.Enabled = True


    End Sub
    Private Sub press()
        'btnPress__Switch()
        'btnPress__XB1()
        'btnPress_Standard()
        btnPress_Celeste()
        'btnPress_DarkSoulsRemastered()
        'btnPress_PokemonFireRed()
        'btnPress_PokemonPlatinum()
        'btnPress_SilentHill2()
        'btnPress_SuperMeatBoy()
        'btnPress_ZeldaMM()
        'btnPress_ZeldaOOT()
        'btnPress_ZeldaTP()
    End Sub

    Private Sub execCMD(user As String, role As String, cmd As String)
        'execCMD__Switch(user, role, cmd)
        'execCMD__XB1(user, role, cmd)
        execCMD_Celeste(user, role, cmd)
        'execCMD_DarkSoulsRemastered(user, role, cmd)
        'execCMD_PokemonFireRed(user, role, cmd)
        'execCMD_PokemonPlatinum(user, role, cmd)
        'execCMD_SilentHill2(user, role, cmd)
        'execCMD_SuperMeatBoy(user, role, cmd)
        'execCMD_ZeldaMM(user, role, cmd)
        'execCMD_ZeldaOOT(user, role, cmd)
        'execCMD_ZeldaTP(user, role, cmd)
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



        If tmpcmd = "#authlist" Then
            Dim txt As String = ""
            For Each usr In modlist
                txt += usr + ", "
            Next
            outputChat(txt)
            Return
        End If

        If tmpcmd = "#macrolist" Then
            Dim txt As String = ""
            For Each pair As KeyValuePair(Of String, String) In macros
                txt += pair.Key + ", "
            Next
            outputChat(txt)
            Return
        End If

        If tmpcmd.IndexOf("#macroshow") = 0 Then
            For Each pair As KeyValuePair(Of String, String) In macros
                If tmpcmd.Contains(" ") AndAlso pair.Key = tmpcmd.Split(" ")(1) Then
                    outputChat(pair.Value)
                    Return
                End If
                outputChat("Macro not found.")
                Return
            Next
        End If

        'If tmpcmd.IndexOf("#authadd") = 0 Then
        'outputChat("Authadd triggered.")
        'End If





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
            Case "reconnect1"
                If Not modlist.Contains(user) Then
                    outputChat("Command restricted.")
                    Return
                End If
            Case "options", "opt", "hopt"
                If Not modlist.Contains(user) Then
                    'outputChat("Options menu restricted to pre-approved users.")
                    'Return
                End If
            Case "pshome"
                If Not (tmpuser = "wulf2k" Or tmpuser = "seannyb" Or tmpuser = "tompiet1") Then
                    outputChat("Uhh....  No.")
                    Return
                End If
            Case "tri", "htri"
                If Not modlist.Contains(user) Then

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
                If pair.Key = tmpcmd Then tmpcmd = pair.Value
            Next
            ProcessCMD(tmpuser, role, tmpcmd)
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



    Dim modlist As New List(Of String)
    Dim trilist As New List(Of String)
    Dim ignorelist As New List(Of String)

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

        'Control which chat users can execute mod commands
        modlist.Add("7kmarkus")
        modlist.Add("emonkreg")
        modlist.Add("knoll24")
        modlist.Add("nick666101")
        modlist.Add("nightbot")
        modlist.Add("seannyb")
        modlist.Add("schattentod")
        modlist.Add("tompiet1")
        modlist.Add("wea000")
        modlist.Add("wulf2k")
        modlist.Add("yuidesu")



        macros.Add(New KeyValuePair(Of String, String)("#test1", "hx10,wb,wr,hx10"))
        macros.Add(New KeyValuePair(Of String, String)("#test2", "hx5,wf,wl,hx5"))


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


        IRC.Send($"PRIVMSG {txtTwitchChat.Text} :{txt}")

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



