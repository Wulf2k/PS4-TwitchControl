Public Class frmPS4Twitch

    Dim a As New asm

    Dim lastEmber As Integer = 0
    Dim repeatCount As Integer

    Dim QueuedInput As New List(Of QdInput)


    Private WithEvents refTimerPress As New System.Windows.Forms.Timer()
    Private WithEvents updTimer As New System.Windows.Forms.Timer()
    Private WithEvents refTimerPost As New System.Windows.Forms.Timer()

    Private Declare Function OpenProcess Lib "kernel32.dll" (ByVal dwDesiredAcess As UInt32, ByVal bInheritHandle As Boolean, ByVal dwProcessId As Int32) As IntPtr
    Private Declare Function ReadProcessMemory Lib "kernel32" (ByVal hProcess As IntPtr, ByVal lpBaseAddress As IntPtr, ByVal lpBuffer() As Byte, ByVal iSize As Integer, ByRef lpNumberOfBytesRead As Integer) As Boolean
    Private Declare Function WriteProcessMemory Lib "kernel32" (ByVal hProcess As IntPtr, ByVal lpBaseAddress As IntPtr, ByVal lpBuffer() As Byte, ByVal iSize As Integer, ByVal lpNumberOfBytesWritten As Integer) As Boolean
    Private Declare Function CloseHandle Lib "kernel32.dll" (ByVal hObject As IntPtr) As Boolean
    Private Declare Function VirtualAllocEx Lib "kernel32.dll" (ByVal hProcess As IntPtr, ByVal lpAddress As IntPtr, ByVal dwSize As IntPtr, ByVal flAllocationType As Integer, ByVal flProtect As Integer) As IntPtr
    Private Declare Function VirtualProtectEx Lib "kernel32.dll" (hProcess As IntPtr, lpAddress As IntPtr, ByVal lpSize As IntPtr, ByVal dwNewProtect As UInt32, ByRef dwOldProtect As UInt32) As Boolean
    Private Declare Function VirtualFreeEx Lib "kernel32.dll" (hProcess As IntPtr, lpAddress As IntPtr, ByVal dwSize As Integer, ByVal dwFreeType As Integer) As Boolean
    Private Declare Function CreateRemoteThread Lib "kernel32" (ByVal hProcess As Integer, ByVal lpThreadAttributes As Integer, ByVal dwStackSize As Integer, ByVal lpStartAddress As Integer, ByVal lpParameter As Integer, ByVal dwCreationFlags As Integer, ByRef lpThreadId As Integer) As Integer

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
    Dim ignorelist As New List(Of String)



    Private rpBase As IntPtr = 0
    Private rpCtrlWrap As IntPtr = 0
    Private wow64 As IntPtr = 0

    Dim hookmem As IntPtr
    Dim hookloc As IntPtr


    Private ctrlPtr As IntPtr

    Public Function ScanForProcess(ByVal windowCaption As String, Optional automatic As Boolean = False) As Boolean
        Dim _allProcesses() As Process = Process.GetProcesses
        For Each pp As Process In _allProcesses
            If pp.MainWindowTitle.ToLower.equals(windowCaption.ToLower) Then
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
                MessageBox.Show("Warning: MemoryManager::DetachFromProcess::CloseHandle error " & Environment.NewLine & ex.Message)
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

                Case "wow64.dll" 'Find steam_api.dll for ability to directly add SteamIDs as nodes
                    wow64 = dll.BaseAddress
            End Select
        Next
    End Sub

    Private Sub frmPS4Twitch_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        wb.Navigate("http://www.twitch.tv/wulf2k/chat")
        

        refTimerPress.Interval = 50
        refTimerPress.Enabled = True
        refTimerPress.Start()

        updTimer.Interval = 500
        updTimer.Enabled = True
        updTimer.Start()

        modlist.Add("daydahd")
        modlist.Add("eternalvalley")
        modlist.Add("illusorywall")
        modlist.Add("jesterbo")
        modlist.Add("jesterpatches")
        modlist.Add("seannybee")
        modlist.Add("superwaifubot")
        modlist.Add("tompiet1")
        modlist.Add("wulf2k")
        modlist.Add("wulf2kbot")
        modlist.Add("yuidesu")
        modlist.Add("zephyp")
        

    End Sub
    Private Sub updTimer_Tick() Handles updTimer.Tick
        Dim Elems As HtmlElementCollection
        Dim ember As Integer

        Dim entry(2) As String

        Try
            Elems = wb.Document.GetElementsByTagName("li")
            For Each elem As HtmlElement In Elems
                If elem.GetAttribute("id").Contains("ember") Then
                    ember = parseEmber(elem.GetAttribute("id"))
                    If ember > lastEmber Then
                        lastEmber = ember

                        entry = parseChat(elem.InnerText)

                        ProcessCMD(entry)
                    End If
                End If
            Next

        Catch ex As Exception
            txtChat.Text += ex.Message & Environment.NewLine
        End Try
    End Sub
    Private Sub refTimerPress_Tick() Handles refTimerPress.Tick
        press()
    End Sub
    Private Sub press()
        Dim buttons = 0
        Dim LStickLR As Single = 0
        Dim LStickUD As Single = 0
        Dim RStickLR As Single = 0
        Dim RStickUD As Single = 0
        Dim LTrigger As Single = 0
        Dim RTrigger As Single = 0
        Dim user As String = ""
        Dim cmd As String = ""



        If chkHoldTri.Checked Then buttons = (buttons Or &H1000)
        If chkHoldO.Checked Then buttons = (buttons Or &H2000)
        If chkHoldX.Checked Then buttons = (buttons Or &H4000)
        If chkHoldSq.Checked Then buttons = (buttons Or &H8000)


        'xxxxxxx1	Share
        'xxxxxxx2	L3
        'xxxxxxx4	R3
        'xxxxxxx8	Options
        'xxxxxx1x	Up
        'xxxxxx2x	Right
        'xxxxxx4x	Down
        'xxxxxx8x	Left
        'xxxxx1xx	L2
        'xxxxx2xx	R2
        'xxxxx4xx	L1
        'xxxxx8xx	R1
        'xxxx1xxx	Triangle
        'xxxx2xxx	O
        'xxxx4xxx	X
        'xxxx8xxx	Square
        'xx1xxxxx	Touchscreen push
        Try



        If QueuedInput.Count > 0 Then

            buttons = buttons Or QueuedInput(0).buttons

            If (buttons And &H10000000) then
                buttons -= &H10000000
                buttons = buttons Or &H2000
                chkHoldO.Checked = true
                PopQ
                press
                return
            End If


            If (buttons And &H20000000)
                buttons -= &H20000000
                buttons = buttons And &HFFFFDFFF
                chkHoldO.checked = false
                PopQ
                press
                return
            End If


            LStickLR = QueuedInput(0).LStickLR
            LStickUD = QueuedInput(0).LStickUD
            RStickLR = QueuedInput(0).RStickLR
            RStickUD = QueuedInput(0).RStickUD
            LTrigger = QueuedInput(0).LTrigger
            RTrigger = QueuedInput(0).RTrigger
            user = QueuedInput(0).user
            cmd = QueuedInput(0).cmd
            refTimerPress.Interval = QueuedInput(0).time
            PopQ()
            txtChat.Text = txtChat.Text + Hex(CInt(hookmem)) & " q:" & QueuedInput.Count & Environment.NewLine

        Else
            refTimerPress.Interval = 33
        End if
        
        WBytes(hookmem + &H300, System.Text.Encoding.ASCII.GetBytes(user + Chr(0)))
        WBytes(hookmem + &H310, System.Text.Encoding.ASCII.GetBytes(cmd + Chr(0)))

        WUInt32(hookmem + &H40C, buttons)

        WUInt8(hookmem + &H410, &H7F& + LStickLR * &H7FUI)
        WUInt8(hookmem + &H411, &H7F& - LStickUD * &H7FUI)
        WUInt8(hookmem + &H412, &H7F& + RStickLR * &H7FUI)
        WUInt8(hookmem + &H413, &H7F& - RStickUD * &H7FUI)
        WUInt8(hookmem + &H414, &HFF& * LTrigger)
        WUInt8(hookmem + &H415, &HFF& * RTrigger)

        Catch ex As Exception

        End Try



    End Sub
    Private Sub PushQ(ByRef buttons As Integer, RStickLR As Single, RStickUD As Single, LStickLR As Single, _
                      LStickUD As Single, LTrigger As Single, RTrigger As Single, time As Integer, user As String, _
                      cmd As String)
        
        QueuedInput.Add(New QdInput() With {.buttons = buttons, .RstickLR = RStickLR, .RstickUD = RStickUD, _
                                            .LStickLR = LStickLR, .LStickUD = LStickUD, .LTrigger = LTrigger, _
                                            .RTrigger = RTrigger, .time = time, .user = user, .cmd = cmd})
    End Sub
    Private Sub PopQ()
        QueuedInput.RemoveAt(0)
    End Sub
    Private Function parseEmber(ByVal txt As String) As Integer
        Dim ember = 0
        txt = Microsoft.VisualBasic.Right(txt, txt.Length - 5)
        ember = Val(txt)
        Return ember
    End Function
    Private Function parseChat(ByVal txt As String) As String()
        txt = Microsoft.VisualBasic.Right(txt, txt.Length - InStr(2, txt, Chr(13))).ToLower
        If Asc(txt(0)) = 10 Then txt = Microsoft.VisualBasic.Right(txt, txt.Length - 1)




        If txt.Contains(ChrW(10)) Then
            txt = txt.Split(ChrW(10))(txt.Split(ChrW(10)).Count - 1)
        End If

        Dim username As String
        Dim cmd As string
        username = txt.Split(":")(0).Trim(" ")
        cmd = txt.Split(":")(txt.Split(":").Count-1).Trim(" ")

        txtChat.Text = txtChat.Text & username & "." & cmd & Environment.NewLine
        If txtChat.Text.Length > 100 Then txtChat.Text = ""
        'txtChat.Text += txt & Environment.NewLine



        Return {username, cmd}
    End Function
    Private Sub outputChat(ByVal txt As String)
        Dim Elems As HtmlElementCollection
        Dim elem As HtmlElement
        Try
            Elems = wb.Document.GetElementsByTagName("textarea")
            elem = Elems(0)
            elem.InnerText = txt
        Catch ex As Exception
            txtChat.Text += ex.Message & Environment.NewLine
        End Try

        refTimerPost.Interval = 100
        refTimerPost.Enabled = True
        refTimerPost.Start()
    End Sub
    Private Sub refTimerPost_Tick() Handles refTimerPost.Tick
        Dim Elems As HtmlElementCollection

        Try
            Elems = wb.Document.GetElementsByTagName("button")
            
            Elems(Elems.Count-1).InvokeMember("click")
        Catch ex As Exception
            txtChat.Text += ex.Message & Environment.NewLine
        End Try

        refTimerPost.Stop()
    End Sub

    Private Sub ProcessCMD(entry() As String)

        Dim CllCMDList As String()

        CllCMDList = {"wf", "wl", "wb", "wr", "wfl", "wfr", "wbl", "wbr", "flong", _
                        "hwf", "hwl", "hwr", "hwb", _
                        "hwfl", "hwfr", "hwbl", "hwbr", _
                        "rof", "rol", "rob", "ror", _
                        "rofr", "rofl", "robr", "robl", _
                        "lu", "ll", "lr", "ld", "r3", _
                        "hlu", "hll", "hlr", "hld", _
                        "du", "dd", "dl", "dr", _
                        "share", "options", "sq", "x", "o", "tri", "l3", "ol1", _
                        "l1", "l2", "r1", "r2", "fr1", "fr2", "cr2", "tp", _
                        "takecontrol", "restorecontrol", _
                        "hh", "h", _
                        "holdx", "holdo", "noholdo", "holdsq", "holdtri", _
                        "ho", "nho", _
                        "clearcmds"}
        
        Dim tmpuser = entry(0)
        Dim tmpcmd = entry(1)
        Dim CMDmulti As Integer = 1


        If tmpcmd.Contains(",") Then
            For each cmd in tmpcmd.Split(",")
                ProcessCMD({tmpuser, cmd.Replace(" ","")})
            Next
            Return
        End If

      

        If tmpcmd.Length > 2 Then

            If IsNumeric(tmpcmd(tmpcmd.Length - 1)) And tmpcmd(tmpcmd.Length - 2) = "x" Then
                CMDmulti = Val(tmpcmd(tmpcmd.Length - 1))
                tmpcmd = Microsoft.VisualBasic.Left(tmpcmd, tmpcmd.Length - 2)
            End If
        End If


        Select Case tmpcmd
            Case "options"
                If Not modlist.contains(tmpuser) Then
                    outputChat("Options menu restricted to pre-approved users.")
                    Return
                End If
            Case "tri", "holdtri"
                If Not modlist.Contains(tmpuser) Then
                    outputChat("Consumable use restricted to pre-approved users.")
                    Return
                End If
            Case "clearcmds"
                If Not (modlist.Contains(tmpuser)) Then
                    For i = QueuedInput.Count - 1 To 0 Step -1
                        If QueuedInput(i).user = tmpuser Then
                            QueuedInput.RemoveAt(i)
                        End If
                        outputChat("All commands for " & tmpuser & " removed from queue.")
                    Next
                    Return
                Else
                    QueuedInput.Clear
                    refTimerPress.Interval = 1
                End If
        End Select





        If CllCMDList.Contains(tmpcmd) Or (tmpcmd.Contains("-") And _
            (tmpcmd(0) = "c" Or tmpcmd(0) = "a" Or tmpcmd(0) = "w" Or tmpcmd(0) = "l")) then

            For i = 0 To CMDmulti - 1
                execCMD(tmpuser, tmpcmd)
            Next
            
        End If

    End Sub


    Private Sub execCMD(user As String, cmd As String)

        REM PS3Controller ( buttons, R stick left/right, R stick up/down, L stick left/right, L stick up/down, _
        REM                 hold button length)

        'Math error, left in to maintain existing commands. /5
        If (cmd(0) = "c" And cmd.contains("-")) Then
            Dim duration
            Dim axis(3) as Single
            For i = 0 To 3
                axis(i) = (5 - val(cmd(i+1))) / 5
            Next
            duration = cmd.split("-")(1)
            controller(0, axis(2), axis(3), axis(0), axis(1), 0, 0, duration * 1.9, user, cmd)
        End If



        'replacement for above, /4
        If (cmd(0) = "a" And cmd.contains("-")) Then
            Dim duration
            Dim axis(3) as Single
            For i = 0 To 3
                axis(i) = (5 - val(cmd(i+1))) / 4
            Next
            duration = cmd.split("-")(1)
            controller(0, axis(2), axis(3), axis(0), axis(1), 0, 0, duration, user, cmd)
        End If

        If (cmd(0) = "l" And cmd.contains("-")) Then
            Dim duration
            Dim axis(1) as Single
            For i = 0 To 1
                axis(i) = (5 - val(cmd(i+1))) / 4
            Next
            duration = cmd.split("-")(1)
            controller(0, axis(0), axis(1), 0, 0, 0, 0, duration, user, cmd)
        End If

        If (cmd(0) = "w" And cmd.contains("-")) Then
            Dim duration
            Dim axis(1) as Single
            For i = 0 To 1
                axis(i) = (5 - val(cmd(i+1))) / 4
            Next
            duration = cmd.split("-")(1)
            controller(0, 0, 0, axis(0), axis(1), 0, 0, duration, user, cmd)
        End If



        Select Case cmd
            Case "wf"
                Controller(0, 0, 0, 0, 1, 0, 0, 38, user, cmd)
            Case "wl"
                Controller(0, 0, 0, -1, 0, 0, 0, 38, user, cmd)
            Case "wb"
                Controller(0, 0, 0, 0, -1, 0, 0, 38, user, cmd)
            Case "wr"
                Controller(0, 0, 0, 1, 0, 0, 0, 38, user, cmd)

            Case "wfl"
                Controller(0, 0, 0, -1, 1, 0, 0, 38, user, cmd)
            Case "wfr"
                Controller(0, 0, 0, 1, 1, 0, 0, 38, user, cmd)
            Case "wbl"
                Controller(0, 0, 0, -1, -1, 0, 0, 38, user, cmd)
            Case "wbr"
                Controller(0, 0, 0, 1, -1, 0, 0, 38, user, cmd)

            Case "hwf"
                Controller(0, 0, 0, 0, 0.5, 0, 0, 38, user, cmd)
            Case "hwl"
                Controller(0, 0, 0, -0.5, 0, 0, 0, 38, user, cmd)
            Case "hwb"
                Controller(0, 0, 0, 0, -0.5, 0, 0, 38, user, cmd)
            Case "hwr"
                Controller(0, 0, 0, 0.5, 0, 0, 0, 38, user, cmd)

            Case "hwfl"
                Controller(0, 0, 0, -0.5, 0.5, 0, 0, 38, user, cmd)
            Case "hwfr"
                Controller(0, 0, 0, 0.5, 0.5, 0, 0, 38, user, cmd)
            Case "hwbl"
                Controller(0, 0, 0, -0.5, -0.5, 0, 0, 38, user, cmd)
            Case "hwbr"
                Controller(0, 0, 0, 0.5, -0.5, 0, 0, 38, user, cmd)

            Case "flong"
                Controller(0, 0, 0, 0, 1, 0, 0, 114, user, cmd)

            Case "rof"
                Controller(0, 0, 0, 0, 1, 0, 0, 2, user, cmd)
                Controller(&H2000, 0, 0, 0, 1, 0, 0, 4, user, cmd)
                Controller(0, 0, 0, 0, 1, 0, 0, 18, user, cmd)
                If chkHoldO.Checked Then outputChat("HoldO currently active")

            Case "rofl"
                Controller(0, 0, 0, -1, 1, 0, 0, 2, user, cmd)
                Controller(&H2000, 0, 0, -1, 1, 0, 0, 4, user, cmd)
                Controller(0, 0, 0, -1, 1, 0, 0, 18, user, cmd)
                If chkHoldO.Checked Then outputChat("HoldO currently active")

            Case "rol"
                Controller(0, 0, 0, -1, 0, 0, 0, 2, user, cmd)
                Controller(&H2000, 0, 0, -1, 0, 0, 0, 4, user, cmd)
                Controller(0, 0, 0, -1, 0, 0, 0, 18, user, cmd)
                If chkHoldO.Checked Then outputChat("HoldO currently active")

            Case "robl"
                Controller(0, 0, 0, -1, -1, 0, 0, 2, user, cmd)
                Controller(&H2000, 0, 0, -1, -1, 0, 0, 4, user, cmd)
                Controller(0, 0, 0, -1, -1, 0, 0, 18, user, cmd)
                If chkHoldO.Checked Then outputChat("HoldO currently active")

            Case "rob"
                Controller(0, 0, 0, 0, -1, 0, 0, 2, user, cmd)
                Controller(&H2000, 0, 0, 0, -1, 0, 0, 4, user, cmd)
                Controller(0, 0, 0, 0, -1, 0, 0, 18, user, cmd)
                If chkHoldO.Checked Then outputChat("HoldO currently active")

            Case "robr"
                Controller(0, 0, 0, 1, -1, 0, 0, 2, user, cmd)
                Controller(&H2000, 0, 0, 1, -1, 0, 0, 4, user, cmd)
                Controller(0, 0, 0, 1, -1, 0, 0, 18, user, cmd)
                If chkHoldO.Checked Then outputChat("HoldO currently active")

            Case "ror"
                Controller(0, 0, 0, 1, 0, 0, 0, 2, user, cmd)
                Controller(&H2000, 0, 0, 1, 0, 0, 0, 4, user, cmd)
                Controller(0, 0, 0, 1, 0, 0, 0, 18, user, cmd)
                If chkHoldO.Checked Then outputChat("HoldO currently active")

            Case "rofr"
                Controller(0, 0, 0, 1, 1, 0, 0, 2, user, cmd)
                Controller(&H2000, 0, 0, 1, 1, 0, 0, 4, user, cmd)
                Controller(0, 0, 0, 1, 1, 0, 0, 18, user, cmd)
                If chkHoldO.Checked Then outputChat("HoldO currently active")

            Case "lu"
                Controller(0, 0, 1, 0, 0, 0, 0, 7, user, cmd)
            Case "ll"
                Controller(0, -1, 0, 0, 0, 0, 0, 7, user, cmd)
            Case "lr"
                Controller(0, 1, 0, 0, 0, 0, 0, 7, user, cmd)
            Case "ld"
                Controller(0, 0, -1, 0, 0, 0, 0, 7, user, cmd)

            Case "hlu"
                Controller(0, 0, 0.5, 0, 0, 0, 0, 7, user, cmd)
            Case "hll"
                Controller(0, -0.5, 0, 0, 0, 0, 0, 7, user, cmd)
            Case "hlr"
                Controller(0, 0.5, 0, 0, 0, 0, 0, 7, user, cmd)
            Case "hld"
                Controller(0, 0, -0.5, 0, 0, 0, 0, 7, user, cmd)

            Case "hh"
                Controller(0, 0, 0, 0, 0, 0, 0, 15, user, cmd)
            Case "h"
                Controller(0, 0, 0, 0, 0, 0, 0, 30, user, cmd)

            Case "du"
                Controller(&H10, 0, 0, 0, 0, 0, 0, 4, user, cmd)
                Controller(0, 0, 0, 0, 0, 0, 0, 4, user, cmd)
            Case "dd"
                Controller(&H40, 0, 0, 0, 0, 0, 0, 4, user, cmd)
                Controller(0, 0, 0, 0, 0, 0, 0, 4, user, cmd)
            Case "dl"
                Controller(&H80, 0, 0, 0, 0, 0, 0, 4, user, cmd)
                Controller(0, 0, 0, 0, 0, 0, 0, 4, user, cmd)
            Case "dr"
                Controller(&H20, 0, 0, 0, 0, 0, 0, 4, user, cmd)
                Controller(0, 0, 0, 0, 0, 0, 0, 4, user, cmd)

            Case "share"
                'Controller(&H1, 0, 0, 0, 0, 0, 0, 1)
            Case "options"
                Controller(&H8, 0, 0, 0, 0, 0, 0, 4, user, cmd)
                Controller(0, 0, 0, 0, 0, 0, 0, 4, user, cmd)

            Case "o"
                Controller(&H2000, 0, 0, 0, 0, 0, 0, 16, user, cmd)
                Controller(0, 0, 0, 0, 0, 0, 0, 4, user, cmd)
                If chkHoldO.Checked Then outputChat("HoldO currently active")
            Case "x"
                Controller(&H4000, 0, 0, 0, 0, 0, 0, 16, user, cmd)
                Controller(0, 0, 0, 0, 0, 0, 0, 4, user, cmd)
                If chkHoldX.Checked Then outputChat("HoldX currently active")
            Case "sq"
                Controller(&H8000, 0, 0, 0, 0, 0, 0, 16, user, cmd)
                Controller(0, 0, 0, 0, 0, 0, 0, 4, user, cmd)
                If chkHoldSq.Checked Then outputChat("HoldSq currently active")
            Case "tri"
                Controller(&H1000, 0, 0, 0, 0, 0, 0, 26, user, cmd)
                Controller(0, 0, 0, 0, 0, 0, 0, 4, user, cmd)
                If chkHoldTri.Checked Then outputChat("HoldTri currently active")

            Case "l1"
                Controller(&H400, 0, 0, 0, 0, 0, 0, 4, user, cmd)
                Controller(0, 0, 0, 0, 0, 0, 0, 16, user, cmd)
            Case "l2"
                Controller(&H100, 0, 0, 0, 0, 1, 0, 15, user, cmd)
                Controller(0, 0, 0, 0, 0, 0, 0, 15, user, cmd)
            Case "r1"
                Controller(&H800, 0, 0, 0, 0, 0, 0, 15, user, cmd)
                Controller(0, 0, 0, 0, 0, 0, 0, 15, user, cmd)
            Case "r2"
                Controller(&H200, 0, 0, 0, 0, 0, 1, 10, user, cmd)
                Controller(0, 0, 0, 0, 0, 0, 0, 20, user, cmd)

            Case "ol1"
                 Controller(&H2000, 0, 0, 0, 0, 0, 0, 4, user, cmd)
                Controller(0, 0, 0, 0, 0, 0, 0, 16, user, cmd)
                If chkHoldO.Checked Then outputChat("HoldO currently active")
                Controller(&H400, 0, 0, 0, 0, 0, 0, 4, user, cmd)
                Controller(0, 0, 0, 0, 0, 0, 0, 24, user, cmd)


            Case "cr2"
                Controller(&H200, 0, 0, 0, 0, 0, 1, 90, user, cmd)
                Controller(0, 0, 0, 0, 0, 0, 0, 10, user, cmd)

            Case "fr1"
                Controller(&H800, 0, 0, 0, 1, 0, 0, 20, user, cmd)
                Controller(0, 0, 0, 0, 0, 0, 0, 10, user, cmd)

            Case "fr2"
                Controller(0,0,0,0,0,0,0,02,user,cmd)
                Controller(&H200, 0, 0, 0, 1, 0, 1, 20, user, cmd)
                Controller(0, 0, 0, 0, 0, 0, 0, 38, user, cmd)

            Case "l3"
                Controller(&H2, 0, 0, 0, 0, 0, 0, 4, user, cmd)
                Controller(0, 0, 0, 0, 0, 0, 0, 4, user, cmd)

            Case "r3"
                Controller(&H4, 0, 0, 0, 0, 0, 0, 2, user, cmd)
                Controller(0, 0, 0, 0, 0, 0, 0, 2, user, cmd)

            Case "tp"
                Controller(&H1000000, 0, 0, 0, 0, 0, 0, 5, user, cmd)
                Controller(0, 0, 0, 0, 0, 0, 0, 5, user, cmd)

                
            Case "holdx"
                chkHoldX.Checked = Not chkHoldX.Checked
                outputChat("Hold X = " & chkHoldX.Checked)

            Case "holdo", "ho"
                Controller(&H10000000, 0, 0, 0, 0, 0, 0, 0, user, cmd)
            Case "noholdo", "nho"
                Controller(&H20000000, 0, 0, 0, 0, 0, 0, 0, user, cmd)

            Case "holdtri"
                chkHoldTri.Checked = Not chkHoldTri.Checked
                outputChat("Hold Tri = " & chkHoldTri.Checked)
            Case "holdsq"
                chkHoldSq.Checked = Not chkHoldSq.Checked
                outputChat("Hold Sq = " & chkHoldSq.Checked)


            Case "takecontrol"
                TakeControl()
            Case "restorecontrol"
                RestoreControl()
        End Select
    End Sub
    Private Sub Controller(buttons As Integer, RLR As Single, RUD As Single, LLR As Single, LUD As Single, LT As Single, RT As Single, hold As Integer, user As String, cmd As string)

        hold = hold * 33 'Fake 30fps

        If hold > 60000 Then hold = 60000
        

        PushQ(buttons, RLR, RUD, LLR, LUD, LT, RT, hold, user, cmd)
        
    End Sub

    Private Sub TakeControl
        If ctrlPtr Then



            hookmem = VirtualAllocEx(_targetProcessHandle, 0, &H8000, MEM_COMMIT, PAGE_EXECUTE_READWRITE)
            Dim oldProtectionOut As UInteger
            VirtualProtectEx(_targetProcessHandle, hookmem, &H8000, PAGE_EXECUTE_READWRITE, oldProtectionOut)

            Dim a As New asm

            a.AddVar("hook", rpCtrlWrap + &H1D0980)
            a.AddVar("newmem", hookmem)
            a.AddVar("newctrl", hookmem + &H400)
            a.AddVar("hookreturn", rpCtrlWrap + &H1D0986)

            a.pos = hookmem
            a.Asm("mov edx, newctrl")

            a.Asm("add esp,0x0C") 'Restore overwritten instruction

            a.Asm("jmp hookreturn")

            WriteProcessMemory(_targetProcessHandle, hookmem, a.bytes, a.bytes.length, 0)

            Console.WriteLine("Hook: " & Hex(CInt(hookmem)))

            a.Clear
            a.AddVar("newmem", hookmem)
            a.pos = rpCtrlWrap + &H1D0980
            a.asm("jmp newmem")

            WriteProcessMemory(_targetProcessHandle, rpCtrlWrap + &H1D0980, a.bytes, a.bytes.length, 0)
        End If
    End Sub
    Private Sub RestoreControl
        'Buttons value
        WBytes(rpCtrlWrap + &H1D0980, {&H8B, &H55, &H0C, &H83, &Hc4, &H0c})
    End Sub
    Private Sub chkAttached_CheckedChanged(sender As Object, e As EventArgs) Handles chkAttached.CheckedChanged
        If chkAttached.Checked Then
            chkattached.Checked = ScanForProcess("PS4 Remote Play", True)
            findDllAddresses()

            ctrlPtr = RIntPtr(rpCtrlWrap + &H2AC304)
            If ctrlPtr Then ctrlPtr = RIntPtr(ctrlPtr + &H5C)
            If ctrlPtr Then ctrlPtr = RIntPtr(ctrlPtr + &H58)
            If ctrlPtr Then ctrlPtr = RIntPtr(ctrlPtr)
            Console.WriteLine("ctrlPtr: " & Hex(CINT(ctrlPtr)))

            TakeControl()
        Else
            RestoreControl()
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

    Public sub WInt8(Byval addr As IntPtr, val As SByte)
        WriteProcessMemory(_targetProcessHandle, addr, {val}, 1, Nothing)
    End sub
    Public Sub WInt16(ByVal addr As IntPtr, val As Int16)
        WriteProcessMemory(_targetProcessHandle, addr, BitConverter.GetBytes(val), 2, Nothing)
    End Sub
    Public Sub WInt32(ByVal addr As IntPtr, val As Int32)
        WriteProcessMemory(_targetProcessHandle, addr, BitConverter.GetBytes(val), 4, Nothing)
    End Sub
    Public Sub WUInt8(ByVal addr As IntPtr, val As Byte)
        WriteProcessMemory(_targetProcessHandle, addr, {val}, 1, Nothing)
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




