Imports System
Imports System.IO.MemoryMappedFiles
Imports System.Threading
Imports Nefarius.ViGEm.Client
Imports Nefarius.ViGEm.Client.Targets
Imports Nefarius.ViGEm.Client.Targets.DualShock4

Partial Public Class frmPS4Twitch

    Dim gh As IntPtr = IntPtr.Zero
    Dim rph As IntPtr = IntPtr.Zero
    Dim fcAddr As IntPtr = IntPtr.Zero

    Dim mmf As MemoryMappedFile
    Dim mmfa As MemoryMappedViewAccessor

    Dim timerfixer = 1
    Dim frametime = 50


    Private Sub TimerPress()
        'TimerPress_Celeste()
        'TimerPress_DarkSoulsRemastered()
        TimerPress_ZeldaOOT()
    End Sub
    Private Sub press()
        'btnPress_Standard()
        'btnPress_Celeste()
        'btnPress_DarkSoulsRemastered()
        btnPress_ZeldaOOT()
    End Sub

    Private Sub execCMD(user As String, role As String, cmd As String)
        'execCMD_Celeste(user, role, cmd)
        'execCMD_DarkSoulsRemastered(user, role, cmd)
        execCMD_ZeldaOOT(user, role, cmd)
    End Sub

    Private Sub ProcessCMD(user As String, role As String, cmd As String)
        'ProcessCMD_Celeste(user, role, cmd)
        'ProcessCMD_DarkSoulsRemastered(user, role, cmd)
        ProcessCMD_ZeldaOOT(user, role, cmd)
    End Sub



    'TODO:  Add held-analog commands

    'TODO:  Change cmd portion of each entry to blank, then process every message's command
    'TODO:  This is to handle multiple new messages in a single check

    Dim client As New ViGEmClient()

    Dim DS4ctrl As IDualShock4Controller
    Dim XBctrl As IXbox360Controller



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

    Private Declare Function CloseHandle Lib "kernel32.dll" (ByVal hObject As IntPtr) As Boolean
    Private Declare Function CreateRemoteThread Lib "kernel32" (ByVal hProcess As Integer, ByVal lpThreadAttributes As Integer, ByVal dwStackSize As Integer, ByVal lpStartAddress As Integer, ByVal lpParameter As Integer, ByVal dwCreationFlags As Integer, ByRef lpThreadId As Integer) As Integer
    Private Declare Function FindWindowA Lib "user32.dll" (ByVal lpClassName As String, ByVal lpWindowName As String) As IntPtr
    Private Declare Function FindWindowExA Lib "user32.dll" (ByVal hWnd1 As Integer, ByVal hWnd2 As Integer, ByVal lpsz1 As String, ByVal lpsz2 As String) As Integer
    Private Declare Function GetWindowRect Lib "user32" (ByVal hWnd As IntPtr, ByRef lpRect As RECT) As Long
    Private Declare Function OpenProcess Lib "kernel32.dll" (ByVal dwDesiredAcess As UInt32, ByVal bInheritHandle As Boolean, ByVal dwProcessId As Int32) As IntPtr
    Private Declare Function ReadProcessMemory Lib "kernel32" (ByVal hProcess As IntPtr, ByVal lpBaseAddress As IntPtr, ByVal lpBuffer() As Byte, ByVal iSize As Integer, ByRef lpNumberOfBytesRead As Integer) As Boolean
    Private Declare Function SetForegroundWindow Lib "user32.dll" (ByVal hwnd As Integer) As Integer
    Private Declare Function VirtualAllocEx Lib "kernel32.dll" (ByVal hProcess As IntPtr, ByVal lpAddress As IntPtr, ByVal dwSize As IntPtr, ByVal flAllocationType As Integer, ByVal flProtect As Integer) As IntPtr
    Private Declare Function VirtualProtectEx Lib "kernel32.dll" (hProcess As IntPtr, lpAddress As IntPtr, ByVal lpSize As IntPtr, ByVal dwNewProtect As UInt32, ByRef dwOldProtect As UInt32) As Boolean
    Private Declare Function VirtualFreeEx Lib "kernel32.dll" (hProcess As IntPtr, lpAddress As IntPtr, ByVal dwSize As Integer, ByVal dwFreeType As Integer) As Boolean
    Private Declare Function WriteProcessMemory Lib "kernel32" (ByVal hProcess As IntPtr, ByVal lpBaseAddress As IntPtr, ByVal lpBuffer() As Byte, ByVal iSize As Integer, ByVal lpNumberOfBytesWritten As Integer) As Boolean




    Public Const PROCESS_VM_READ = &H10
    Public Const TH32CS_SNAPPROCESS = &H2
    Public Const MEM_COMMIT = 4096
    Public Const MEM_RELEASE = &H8000
    Public Const PAGE_READWRITE = 4
    Public Const PAGE_EXECUTE_READWRITE = &H40
    Public Const PROCESS_CREATE_THREAD = (&H2)
    Public Const PROCESS_VM_OPERATION = (&H8)
    Public Const PROCESS_VM_WRITE = (&H20)
    Public Const PROCESS_ALL_ACCESS = &H1F0FFF

    Private _targetProcess As Process = Nothing 'to keep track of it. not used yet.
    Private _targetProcessHandle As IntPtr = IntPtr.Zero 'Used for ReadProcessMemory

    Dim modlist As New List(Of String)
    Dim trilist As New List(Of String)
    Dim ignorelist As New List(Of String)


    'RemotePlay app addresses
    Private rpBase As IntPtr = 0
    Private rpCtrlWrap As IntPtr = 0
    Private wow64 As IntPtr = 0

    'Our newly created memory
    Dim hookmem As IntPtr

    'Hook in RemotePlay's code
    Dim hookloc As IntPtr


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
    Shared boolHoldOpt = False


    Dim boolHoldAxis() As Boolean = {False, False, False, False}
    Dim boolHoldAxisVal() As Single = {0, 0, 0, 0}

    Private ctrlPtr As IntPtr

    Dim IRC As New IrcCon



    Public Function ScanForProcess(ByVal procName As String, Optional automatic As Boolean = False) As Boolean

        Dim _allProcesses() As Process = Process.GetProcesses
        For Each pp As Process In _allProcesses
            'If pp.MainWindowTitle.ToLower.Equals(procName.ToLower) Then
            If pp.ProcessName = procName Then
                'found it! proceed.
                Return TryAttachToProcess(pp, automatic)
            End If
        Next
        Return False
    End Function
    Public Function TryAttachToProcess(ByVal proc As Process, Optional automatic As Boolean = False) As Boolean
        If Not (_targetProcessHandle = IntPtr.Zero) Then
            DetachFromProcess()
        End If

        _targetProcess = proc
        _targetProcessHandle = OpenProcess(PROCESS_ALL_ACCESS, False, _targetProcess.Id)
        If _targetProcessHandle = 0 Then
            If Not automatic Then 'Showing 2 message boxes as soon as you start the program is too annoying.
                MessageBox.Show("Failed to attach to process.")
            End If

            Return False
        Else
            'if we get here, all connected and ready to use ReadProcessMemory()

            Return True
            'MessageBox.Show("OpenProcess() OK")
        End If

    End Function
    Public Sub DetachFromProcess()
        If Not (_targetProcessHandle = IntPtr.Zero) Then
            _targetProcess = Nothing
            Try
                CloseHandle(_targetProcessHandle)
                _targetProcessHandle = IntPtr.Zero
                'MessageBox.Show("MemReader::Detach() OK")
            Catch ex As Exception
                'MessageBox.Show("Warning: MemoryManager::DetachFromProcess::CloseHandle error " & Environment.NewLine & ex.Message)
            End Try
        End If
    End Sub


    Private Sub findDllAddresses()
        For Each dll As ProcessModule In _targetProcess.Modules
            Select Case dll.ModuleName.ToLower
                Case "remoteplay.exe"
                    rpBase = dll.BaseAddress

                Case "rpctrlwrapper.dll"
                    rpCtrlWrap = dll.BaseAddress

                Case "wow64.dll"
                    wow64 = dll.BaseAddress
            End Select
        Next
    End Sub

    Private Sub frmPS4Twitch_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        'Control which chat users can execute mod commands
        modlist.Add("emonkreg")
        modlist.Add("nightbot")
        modlist.Add("seannyb")
        modlist.Add("schattentod")
        modlist.Add("tompiet1")
        modlist.Add("wea000")
        modlist.Add("wulf2k")
        modlist.Add("yuidesu")
    End Sub

    Private Sub btnJoinTwitchChat_Click(sender As Object, e As EventArgs) Handles btnJoinTwitchChat.Click
        IRC.server = "irc.chat.twitch.tv"
        IRC.port = 6667
        IRC.nick = "Wulf2kbot"
        IRC.user = "Wulf2kbot"
        IRC.pwd = My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\Wulf", "TwitchOAuth", Nothing)
        IRC.realname = ""
        IRC.hostname = "wulf2k.ca"

        IRC.Connect()
        IRC.Join(txtTwitchChat.Text)





        'SyncLock wblock
        'wb.Navigate(txtTwitchChat.Text)
        'End SyncLock

        'Below reg setting must be set for webbrowser control to properly load chat
        'Computer\HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION
        'PS4-TwitchControl.exe = 11000

        'Timer to press buttons at
        'Initial value fairly irrelevant
        'refTimerPress.Interval = 33
        'refTimerPress.Enabled = True
        'refTimerPress.Start()


        'Timer to check chat messages
        Threading.Thread.Sleep(5000)
        updTimer.Interval = 25
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
            'line = line.ToLower
            'fuck it, user lowercase


            If line.Length < 1000 Then
                txtChat.AppendText($"-> {line}")
                txtChat.AppendText(Environment.NewLine)
            End If

            If line.IndexOf("PING") = 0 Then
                IRC.Send("PONG")
                txtChat.AppendText($"<- PONG :tmi.twitch.tv")
                txtChat.AppendText(Environment.NewLine)
            End If

            If line.IndexOf("PRIVMSG") > -1 Then
                user = line.Split("!")(0)
                user = user.split(":")(1)

                cmd = line.Split(":")(2)

                ProcessCMD(user, role, cmd)
            End If

        Next

    End Sub

    Private Sub PushQ(ByRef buttons As Integer, RStickLR As Single, RStickUD As Single, LStickLR As Single,
                      LStickUD As Single, LTrigger As Single, RTrigger As Single, time As Integer, user As String,
                      cmd As String)
        SyncLock queuelock
            QueuedInput.Add(New QdInput() With {.buttons = buttons, .RstickLR = RStickLR, .RstickUD = RStickUD,
                                                .LStickLR = LStickLR, .LStickUD = LStickUD, .LTrigger = LTrigger,
                                                .RTrigger = RTrigger, .time = time, .user = user, .cmd = cmd})
        End SyncLock
    End Sub
    Private Sub PopQ()
        SyncLock queuelock
            QueuedInput.RemoveAt(0)
        End SyncLock
    End Sub
    Private Function parseEmber(ByVal txt As String) As Integer
        Dim ember = 0
        txt = Microsoft.VisualBasic.Right(txt, txt.Length - 5)
        ember = Val(txt)
        Return ember
    End Function
    Private Function parseChat(ByVal txt As String) As String()
        txt = Microsoft.VisualBasic.Right(txt, txt.Length - InStr(2, txt, Chr(13)))

        'LUL is a dumb BTTV emoticon people say a fair bit
        'lul is also look up left.  Ignore the caps version.
        If txt.Contains("LUL") Or txt.Contains(",,") Or txt.Contains(".") Or
            txt.Contains("!") Or txt.Contains("?") Then
            Return {"", ""}
        End If

        txt = txt.ToLower
        txt = txt.Replace(" ", "")

        'MsgBox(txt)

        If Asc(txt(0)) = 10 Then txt = Microsoft.VisualBasic.Right(txt, txt.Length - 1)


        If txt.Contains(ChrW(10)) Then
            txt = txt.Split(ChrW(10))(txt.Split(ChrW(10)).Count - 1)
        End If

        Dim username As String
        Dim cmd As String
        username = txt.Split(":")(0).Trim(" ")
        cmd = txt.Split(":")(txt.Split(":").Count - 1).Trim(" ")

        'yes, wulf and hard turn out to be valid commands.
        'ignore them.
        If cmd.Contains("wulf") Or cmd.Contains("hard") Then Return {"", ""}

        Return {username, cmd}
    End Function
    Private Sub outputChat(ByVal txt As String)


    End Sub





    Private Sub Controller(buttons As Integer, RLR As Single, RUD As Single, LLR As Single, LUD As Single, LT As Single, RT As Single, hold As Integer, user As String, cmd As String)

        hold = hold * frametime

        If hold > 66000 Then hold = 66000
        'SyncLock queuelock
        'If QueuedInput.Count > 0 Then
        'If QueuedInput(QueuedInput.Count - 1).cmd = cmd Then
        'Stop combining identical inputs
        'QueuedInput(QueuedInput.Count - 1).time = QueuedInput(QueuedInput.Count - 1).time + hold
        'Return
        'End If
        'End If
        'End SyncLock

        PushQ(buttons, RLR, RUD, LLR, LUD, LT, RT, hold, user, cmd)
    End Sub

    Private Sub TakeControl()


        DS4ctrl = client.CreateDualShock4Controller()
        XBctrl = client.CreateXbox360Controller()

        'DS4ctrl.Connect()
        XBctrl.Connect()

        XBctrl.AutoSubmitReport = False

        'ctrl.SetButtonState(DualShock4Button.Circle, True)
        'ctrl.SetAxisValue(DualShock4Axis.LeftThumbX, 40)


        'If ctrlPtr Then
        'If False Then
        hookmem = VirtualAllocEx(_targetProcessHandle, 0, &H8000, MEM_COMMIT, PAGE_EXECUTE_READWRITE)
        Dim oldProtectionOut As UInteger
        VirtualProtectEx(_targetProcessHandle, hookmem, &H8000, PAGE_EXECUTE_READWRITE, oldProtectionOut)

        'Dim a As New asm


        'a.AddVar("hook", rpCtrlWrap + &H1BEB90)
        'a.AddVar("newmem", hookmem)
        'a.AddVar("newctrl", hookmem + &H400)
        '
        '           a.AddVar("hookreturn", rpCtrlWrap + &H1BEB96)

        'a.pos = hookmem
        'a.Asm("mov edx, newctrl")

        'a.Asm("add esp,0x0C") 'Restore overwritten instruction

        'a.Asm("jmp hookreturn")

        'WriteProcessMemory(_targetProcessHandle, hookmem, a.bytes, a.bytes.Length, 0)

        'Console.WriteLine("Hook: " & Hex(CInt(hookmem)))

        'a.Clear()
        'a.AddVar("newmem", hookmem)
        'a.pos = rpCtrlWrap + &H1BEB90
        'a.Asm("jmp newmem")

        'WriteProcessMemory(_targetProcessHandle, rpCtrlWrap + &H1BEB90, a.bytes, a.bytes.Length, 0)



        pressthread = New Thread(AddressOf TimerPress)
        pressthread.IsBackground = True
        pressthread.Start()




        'End If
    End Sub
    Private Sub RestoreControl()
        ''WBytes(rpCtrlWrap + &H1D0980, {&H8B, &H55, &HC, &H83, &HC4, &HC})  'Old ver
        'WBytes(rpCtrlWrap + &H1BFD10, {&H8B, &H55, &HC, &H83, &HC4, &HC})
        WBytes(rpCtrlWrap + &H1BEB90, {&H8B, &H55, &HC, &H83, &HC4, &HC})

        'pressthread.Abort
    End Sub
    Private Sub chkAttached_CheckedChanged(sender As Object, e As EventArgs) Handles chkAttached.CheckedChanged
        If chkAttached.Checked Then
            Dim found As Boolean

            found = ScanForProcess("Celeste", True)
            'chkAttached.Checked = found
            'If found Then findDllAddresses()


            mmf = MemoryMappedFile.CreateOrOpen("TwitchControl", &H1000)
            mmfa = mmf.CreateViewAccessor()

            TakeControl()
        Else
            'RestoreControl()
        End If
    End Sub


    Public Function RInt8(ByVal addr As IntPtr) As SByte
        Dim _rtnBytes(0) As Byte
        ReadProcessMemory(_targetProcessHandle, addr, _rtnBytes, 1, vbNull)
        Return _rtnBytes(0)
    End Function
    Public Function RInt16(ByVal addr As IntPtr) As Int16
        Dim _rtnBytes(1) As Byte
        ReadProcessMemory(_targetProcessHandle, addr, _rtnBytes, 2, vbNull)
        Return BitConverter.ToInt16(_rtnBytes, 0)
    End Function
    Public Function RInt32(ByVal addr As IntPtr) As Int32
        Dim _rtnBytes(3) As Byte
        ReadProcessMemory(_targetProcessHandle, addr, _rtnBytes, 4, vbNull)

        Return BitConverter.ToInt32(_rtnBytes, 0)
    End Function
    Public Function RInt64(ByVal addr As IntPtr) As Int64
        Dim _rtnBytes(7) As Byte
        ReadProcessMemory(_targetProcessHandle, addr, _rtnBytes, 8, vbNull)
        Return BitConverter.ToInt64(_rtnBytes, 0)
    End Function
    Public Function RUInt16(ByVal addr As IntPtr) As UInt16
        Dim _rtnBytes(1) As Byte
        ReadProcessMemory(_targetProcessHandle, addr, _rtnBytes, 2, vbNull)
        Return BitConverter.ToUInt16(_rtnBytes, 0)
    End Function
    Public Function RUInt32(ByVal addr As IntPtr) As UInt32
        Dim _rtnBytes(3) As Byte
        ReadProcessMemory(_targetProcessHandle, addr, _rtnBytes, 4, vbNull)
        Return BitConverter.ToUInt32(_rtnBytes, 0)
    End Function
    Public Function RUInt64(ByVal addr As IntPtr) As UInt64
        Dim _rtnBytes(7) As Byte
        ReadProcessMemory(_targetProcessHandle, addr, _rtnBytes, 8, vbNull)
        Return BitConverter.ToUInt64(_rtnBytes, 0)
    End Function
    Public Function RSingle(ByVal addr As IntPtr) As Single
        Dim _rtnBytes(3) As Byte
        ReadProcessMemory(_targetProcessHandle, addr, _rtnBytes, 4, vbNull)
        Return BitConverter.ToSingle(_rtnBytes, 0)
    End Function
    Public Function RDouble(ByVal addr As IntPtr) As Double
        Dim _rtnBytes(7) As Byte
        ReadProcessMemory(_targetProcessHandle, addr, _rtnBytes, 8, vbNull)
        Return BitConverter.ToDouble(_rtnBytes, 0)
    End Function
    Public Function RIntPtr(ByVal addr As IntPtr) As IntPtr
        Dim _rtnBytes(IntPtr.Size - 1) As Byte
        ReadProcessMemory(_targetProcessHandle, addr, _rtnBytes, IntPtr.Size, Nothing)
        If IntPtr.Size = 4 Then
            Return New IntPtr(BitConverter.ToUInt32(_rtnBytes, 0))
        Else
            Return New IntPtr(BitConverter.ToInt64(_rtnBytes, 0))
        End If
    End Function
    Public Function RBytes(ByVal addr As IntPtr, ByVal size As Int32) As Byte()
        Dim _rtnBytes(size - 1) As Byte
        ReadProcessMemory(_targetProcessHandle, addr, _rtnBytes, size, vbNull)
        Return _rtnBytes
    End Function
    Private Function RAscStr(ByVal addr As UInteger) As String
        Dim Str As String = ""
        Dim cont As Boolean = True
        Dim loc As Integer = 0

        Dim bytes(&H10) As Byte

        ReadProcessMemory(_targetProcessHandle, addr, bytes, &H10, vbNull)

        While (cont And loc < &H10)
            If bytes(loc) > 0 Then

                Str = Str + Convert.ToChar(bytes(loc))

                loc += 1
            Else
                cont = False
            End If
        End While

        Return Str
    End Function
    Private Function RUniStr(ByVal addr As UInteger) As String
        Dim Str As String = ""
        Dim cont As Boolean = True
        Dim loc As Integer = 0

        Dim bytes(&H20) As Byte


        ReadProcessMemory(_targetProcessHandle, addr, bytes, &H20, vbNull)

        While (cont And loc < &H20)
            If bytes(loc) > 0 Then

                Str = Str + Convert.ToChar(bytes(loc))

                loc += 2
            Else
                cont = False
            End If
        End While

        Return Str
    End Function

    Public Sub WInt8(ByVal addr As IntPtr, val As SByte)
        WriteProcessMemory(_targetProcessHandle, addr, {val}, 1, Nothing)
    End Sub
    Public Sub WInt16(ByVal addr As IntPtr, val As Int16)
        WriteProcessMemory(_targetProcessHandle, addr, BitConverter.GetBytes(val), 2, Nothing)
    End Sub
    Public Sub WInt32(ByVal addr As IntPtr, val As Int32)
        WriteProcessMemory(_targetProcessHandle, addr, BitConverter.GetBytes(val), 4, Nothing)
    End Sub
    Public Sub WUInt8(ByVal addr As IntPtr, val As Byte)
        WriteProcessMemory(_targetProcessHandle, addr, {val}, 1, Nothing)
    End Sub
    Public Sub WUInt16(ByVal addr As IntPtr, val As UInt16)
        WriteProcessMemory(_targetProcessHandle, addr, BitConverter.GetBytes(val), 2, Nothing)
    End Sub
    Public Sub WUInt32(ByVal addr As IntPtr, val As UInt32)
        WriteProcessMemory(_targetProcessHandle, addr, BitConverter.GetBytes(val), 4, Nothing)
    End Sub
    Public Sub WSingle(ByVal addr As IntPtr, val As Single)
        WriteProcessMemory(_targetProcessHandle, addr, BitConverter.GetBytes(val), 4, Nothing)
    End Sub
    Public Sub WBytes(ByVal addr As IntPtr, val As Byte())
        WriteProcessMemory(_targetProcessHandle, addr, val, val.Length, Nothing)
    End Sub
    Public Sub WAscStr(addr As UInteger, str As String)
        WriteProcessMemory(_targetProcessHandle, addr, System.Text.Encoding.ASCII.GetBytes(str), str.Length, Nothing)
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



