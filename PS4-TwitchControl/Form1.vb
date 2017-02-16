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

                Case "wow64.dll" 'Find steam_api.dll for ability to directly add SteamIDs as nodes
                    wow64 = dll.BaseAddress
            End Select
        Next
    End Sub

    Private Sub frmPS4Twitch_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        modlist.Add("byrdshot")
        modlist.Add("daydahd")
        modlist.Add("eternalvalley")
        modlist.Add("illusorywall")
        modlist.Add("jesterbo")
        modlist.Add("jesterpatches")
        modlist.Add("seannybee")
        modlist.Add("shippo62")
        modlist.Add("superwaifubot")
        modlist.Add("tompiet1")
        modlist.Add("wea000")
        modlist.Add("wulf2k")
        modlist.Add("wulf2kbot")
        modlist.Add("yuidesu")
        modlist.Add("zephyp")


    End Sub

    Private Sub btnJoinTwitchChat_Click(sender As Object, e As EventArgs) Handles btnJoinTwitchChat.Click
        wb.Navigate(txtTwitchChat.Text)


        refTimerPress.Interval = 50
        refTimerPress.Enabled = True
        refTimerPress.Start()

        updTimer.Interval = 500
        updTimer.Enabled = True
        updTimer.Start()

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


        Dim buttons = 0
        Dim LStickLR As Single = 0
        Dim LStickUD As Single = 0
        Dim RStickLR As Single = 0
        Dim RStickUD As Single = 0
        Dim LTrigger As Single = 0
        Dim RTrigger As Single = 0
        Dim user As String = ""
        Dim cmd As String = ""




        'If nothing in queue, push a 'nothing'-press onto it for 1 frame
        If QueuedInput.Count = 0 Then
            Controller(0, 0, 0, 0, 0, 0, 0, 1, "", "")
        End If





        Try
            buttons = QueuedInput(0).buttons

            'Handle hold-toggles
            Select Case QueuedInput(0).cmd
                Case "nha"
                    chkHoldL1.Checked = False
                    chkHoldL2.Checked = False
                    chkHoldL3.Checked = False
                    chkHoldR1.Checked = False
                    chkHoldR2.Checked = False
                    chkHoldR3.Checked = False
                    chkHoldO.Checked = False
                    chkHoldSq.Checked = False
                    chkHoldTri.Checked = False
                    chkHoldX.Checked = False

                Case "hl1"
                    chkHoldL1.Checked = True
                Case "nhl1"
                    chkHoldL1.Checked = False

                Case "hl2"
                    chkHoldL2.Checked = True
                Case "nhl2"
                    chkHoldL2.Checked = False

                Case "hl3"
                    chkHoldL3.Checked = True
                Case "nhl3"
                    chkHoldL3.Checked = False


                Case "hr1"
                    chkHoldR1.Checked = True
                Case "nhr1"
                    chkHoldR1.Checked = False

                Case "hr2"
                    chkHoldR2.Checked = True
                Case "nhr2"
                    chkHoldR2.Checked = False

                Case "hr3"
                    chkHoldR3.Checked = True
                Case "nhr3"
                    chkHoldR3.Checked = False


                Case "ho", "holdo"
                    chkHoldO.Checked = True
                Case "nho", "noholdo"
                    chkHoldO.Checked = False

                Case "hsq"
                    chkHoldSq.Checked = True
                Case "nhsq"
                    chkHoldSq.Checked = False

                Case "htri"
                    chkHoldTri.Checked = True
                Case "nhtri"
                    chkHoldTri.Checked = False

                Case "hx"
                    chkHoldX.Checked = True
                Case "nhx"
                    chkHoldX.Checked = False
            End Select



            'If command has no duration, skip to next command.
            If QueuedInput(0).time = 0 Then
                PopQ()
                press()
                Return
            End If




            'Combine held inputs with specified presses
            If chkHoldL3.Checked Then buttons = (buttons Or &H2)
            If chkHoldR3.Checked Then buttons = (buttons Or &H4)

            If chkHoldL2.Checked Then
                buttons = (buttons Or &H100)
                QueuedInput(0).LTrigger = 1
            End If
            If chkHoldR2.Checked Then
                buttons = (buttons Or &H200)
                QueuedInput(0).RTrigger = 1
            End If
            If chkHoldL1.Checked Then buttons = (buttons Or &H400)
            If chkHoldR1.Checked Then buttons = (buttons Or &H800)

            If chkHoldTri.Checked Then buttons = (buttons Or &H1000)
            If chkHoldO.Checked Then buttons = (buttons Or &H2000)
            If chkHoldX.Checked Then buttons = (buttons Or &H4000)
            If chkHoldSq.Checked Then buttons = (buttons Or &H8000)




            'Process specified axises
            LStickLR = QueuedInput(0).LStickLR
            LStickUD = QueuedInput(0).LStickUD
            RStickLR = QueuedInput(0).RstickLR
            RStickUD = QueuedInput(0).RstickUD
            LTrigger = QueuedInput(0).LTrigger
            RTrigger = QueuedInput(0).RTrigger
            user = QueuedInput(0).user
            cmd = QueuedInput(0).cmd
            refTimerPress.Interval = QueuedInput(0).time
            PopQ()




            'check for rolls during holdo
            If cmd.length >= 2 And chkHoldO.Checked Then
                If (cmd(0) = "r" And cmd(1) = "o") Or (cmd(0) = "o") Then
                    outputChat("Evade failed due to HoldO being active.")
                End If
            End If



            'Output queue info and pass to overlay program
            WBytes(hookmem + &H300,
                   System.Text.Encoding.ASCII.GetBytes(user + Chr(0)))

            Dim tmpcmd
            tmpcmd = cmd & "-" & refTimerPress.Interval / 33

            
            If tmpcmd = "-1" Then tmpcmd = ""

            WBytes(hookmem + &H310,
                   System.Text.Encoding.ASCII.GetBytes(tmpcmd & Chr(0)))

            For i = 0 To 9
                If (QueuedInput.Count) > i Then
                    Dim str As String
                    str = QueuedInput(i).cmd & "-" & Math.floor(QueuedInput(i).time / 33)

                    'if command too long, shorten it
                    If str.Length > 15 Then str = Strings.Left(str, 15)
                    str = str & Chr(0)

                    WBytes(hookmem + &H320 + i * &H10, System.Text.Encoding.ASCII.GetBytes(str))
                Else
                    WBytes(hookmem + &H320 + i * &H10, {0})
                End If
            Next

            WInt32(hookmem + &H3C0, QueuedInput.Count)

            'TODO:  Pass tpad values as part of controller queued input
            Select Case cmd
                Case "tpl"
                    WUInt8(hookmem + &H427, &H70)
                    WUInt16(hookmem + &H42A, &H100)
                    WUInt16(hookmem + &H42C, &H100)

                Case "tpr"
                    WUInt8(hookmem + &H427, &H70)
                    WUInt16(hookmem + &H42A, &H400)
                    WUInt16(hookmem + &H42C, &H100)

                Case Else
                    WUInt8(hookmem + &H427, &H80)
                    WUInt16(hookmem + &H42A, 0)
                    WUInt16(hookmem + &H42C, 0)
            End Select



            WUInt32(hookmem + &H40C,
                    buttons)

            WUInt8(hookmem + &H410,
                   &H7FUI + LStickLR * &H7FUI)
            WUInt8(hookmem + &H411,
                   &H7FUI - LStickUD * &H7FUI)
            WUInt8(hookmem + &H412,
                   &H7FUI + RStickLR * &H7FUI)
            WUInt8(hookmem + &H413,
                   &H7FUI - RStickUD * &H7FUI)

            WUInt8(hookmem + &H414,
                   &HFFUI * LTrigger)
            WUInt8(hookmem + &H415,
                   &HFFUI * RTrigger)

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
        txt = Microsoft.VisualBasic.Right(txt, txt.Length - InStr(2, txt, Chr(13)))

        If txt.Contains("LUL") Or txt.Contains(",,") Or txt.Contains(".") Or 
            txt.Contains ("!") or txt.Contains("?") Then 
            return {"", ""}
        End If

        txt = txt.ToLower




        If Asc(txt(0)) = 10 Then txt = Microsoft.VisualBasic.Right(txt, txt.Length - 1)


        If txt.Contains(ChrW(10)) Then
            txt = txt.Split(ChrW(10))(txt.Split(ChrW(10)).Count - 1)
        End If

        Dim username As String
        Dim cmd As string
        username = txt.Split(":")(0).Trim(" ")
        cmd = txt.Split(":")(txt.Split(":").Count-1).Trim(" ")

        If cmd.Contains("wulf") Then Return {"", ""}

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




        Dim tmpuser = entry(0)
        Dim tmpcmd = entry(1)
        Dim CMDmulti As Integer = 1


        'Handle multi-command entries
        If tmpcmd.Contains(",") Then
            For Each cmd In tmpcmd.Split(",")
                'Prevent buffer overflow in RemotePlay memory
                tmpuser = Strings.Left(tmpuser, 15)
                cmd = cmd.Replace(" ", "")
                cmd = Strings.Left(cmd, 15)

                ProcessCMD({tmpuser, cmd})
            Next
            Return
        End If




        'Handle command multipliers
        If tmpcmd.Length > 2 Then
            If IsNumeric(tmpcmd(tmpcmd.Length - 1)) And tmpcmd(tmpcmd.Length - 2) = "x" Then
                CMDmulti = Val(tmpcmd(tmpcmd.Length - 1))
                tmpcmd = Microsoft.VisualBasic.Left(tmpcmd, tmpcmd.Length - 2)
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
            Case "options"
                If Not modlist.Contains(tmpuser) Then
                    outputChat("Options menu restricted to pre-approved users.")
                    Return
                End If
            Case "tri", "htri"
                If Not modlist.Contains(tmpuser) Then
                    outputChat("Consumable use restricted to pre-approved users.")
                    Return
                End If
            Case "clearallcmds", "ca"
                If Not (modlist.Contains(tmpuser)) Then
                    outputChat("Clearing all commands restricted to pre-approved users.")
                Else
                    QueuedInput.Clear()
                    refTimerPress.Interval = 1
                End If
            Case "clearcmds", "c"
                For i = QueuedInput.Count - 1 To 0 Step -1
                    If QueuedInput(i).user = tmpuser Then
                        QueuedInput.RemoveAt(i)
                    End If
                    outputChat("All commands for " & tmpuser & " removed from queue.")
                Next
                Return
            Case "csx"
                ProcessCMD({tmpuser, "clearcmds"})
                ProcessCMD({tmpuser, "nha"})
                ProcessCMD({tmpuser, "sq"})
                ProcessCMD({tmpuser, "x"})
                ProcessCMD({tmpuser, "sq"})
                ProcessCMD({tmpuser, "x"})
                ProcessCMD({tmpuser, "sq"})
                ProcessCMD({tmpuser, "x"})
            Case "casx"
                ProcessCMD({tmpuser, "clearallcmds"})
                ProcessCMD({tmpuser, "nha"})
                ProcessCMD({tmpuser, "sq"})
                ProcessCMD({tmpuser, "x"})
                ProcessCMD({tmpuser, "sq"})
                ProcessCMD({tmpuser, "x"})
                ProcessCMD({tmpuser, "sq"})
                ProcessCMD({tmpuser, "x"})
        End Select




        'For direct analog stick inputs
        For i = 0 To CMDmulti - 1
            execCMD(tmpuser, tmpcmd)
        Next
    End Sub


    Private Sub execCMD(user As String, cmd As String)

        Dim buttons = 0
        Dim axis() As Single = {CSng(0), CSng(0), CSng(0), CSng(0)}
        Dim halfhold As Boolean = False
        Dim duration As Integer = 0

        Dim partcmd As String = ""
        Dim cmdparams As String = "5555"


        If cmd.Contains("-") Then
            duration = cmd.Split("-")(1)
            cmd = cmd.Split("-")(0)
        Else
            duration = 0
        End If



        Select Case cmd
            'Hold toggles
            Case "nha",
                 "holdo", "ho", "noholdo", "nho",
                 "hl1", "nhl1",
                 "hl2", "nhl2",
                 "hl3", "nhl3",
                 "hr1", "nhr1",
                 "hr2", "nhr2",
                 "hr3", "nhr3",
                 "hsq", "nhsq",
                 "htri", "nhtri",
                 "hx", "nhx"

                Controller(0, 0, 0, 0, 0, 0, 0, duration, user, cmd)

                If duration > 0 Then
                    Controller(0, 0, 0, 0, 0, 0, 0, 0, user, "n" & cmd)
                End If
                Return

            'Half halt
            Case "hh"
                If duration = 0 Then duration = 15
                Controller(0, 0, 0, 0, 0, 0, 0, duration, user, cmd)
                Return

            'Halt
            Case "h"
                If duration = 0 Then duration = 30
                Controller(0, 0, 0, 0, 0, 0, 0, duration, user, cmd)
                Return

            'Our old, archaic friend 'flong', "forward long"
            Case "flong"
                If duration = 0 Then duration = 114
                Controller(0, 0, 0, 0, 1, 0, 0, duration, user, cmd)
                Return



            Case "du"
                Controller(&H10, 0, 0, 0, 0, 0, 0, 4, user, cmd & "(!)")
                Controller(0, 0, 0, 0, 0, 0, 0, 4, user, cmd & "(-)")
                Return
            Case "dd"
                Controller(&H40, 0, 0, 0, 0, 0, 0, 4, user, cmd & "(!)")
                Controller(0, 0, 0, 0, 0, 0, 0, 4, user, cmd & "(-)")
                Return
            Case "dl"
                Controller(&H80, 0, 0, 0, 0, 0, 0, 4, user, cmd & "(!)")
                Controller(0, 0, 0, 0, 0, 0, 0, 4, user, cmd & "(-)")
                Return
            Case "dr"
                Controller(&H20, 0, 0, 0, 0, 0, 0, 4, user, cmd & "(!)")
                Controller(0, 0, 0, 0, 0, 0, 0, 4, user, cmd & "(-)")
                Return

            Case "share"
                'Controller(&H1, 0, 0, 0, 0, 0, 0, 1)
                Return

            Case "options"
                Controller(&H8, 0, 0, 0, 0, 0, 0, 4, user, cmd & "(!)")
                Controller(0, 0, 0, 0, 0, 0, 0, 4, user, cmd & "(-)")
                Return

            Case "o"
                Controller(&H2000, 0, 0, 0, 0, 0, 0, 16, user, cmd & "(!)")
                Controller(0, 0, 0, 0, 0, 0, 0, 4, user, cmd & "(-)")
                Return

            Case "x"
                Controller(&H4000, 0, 0, 0, 0, 0, 0, 16, user, cmd & "(!)")
                Controller(0, 0, 0, 0, 0, 0, 0, 4, user, cmd & "(-)")
                Return

            Case "sq"
                Controller(&H8000, 0, 0, 0, 0, 0, 0, 16, user, cmd & "(!)")
                Controller(0, 0, 0, 0, 0, 0, 0, 4, user, cmd & "(-)")
                Return

            Case "tri"
                Controller(&H1000, 0, 0, 0, 0, 0, 0, 26, user, cmd & "(!)")
                Controller(0, 0, 0, 0, 0, 0, 0, 4, user, cmd)
                Return


            Case "l1"
                Controller(&H400, 0, 0, 0, 0, 0, 0, 4, user, cmd & "(!)")
                Controller(0, 0, 0, 0, 0, 0, 0, 16, user, cmd & "(-)")
                Return
            Case "l2"
                Controller(&H100, 0, 0, 0, 0, 1, 0, 15, user, cmd & "(!)")
                Controller(0, 0, 0, 0, 0, 0, 0, 15, user, cmd & "(-)")
                Return
            Case "r1"
                Controller(&H800, 0, 0, 0, 0, 0, 0, 15, user, cmd & "(!)")
                Controller(0, 0, 0, 0, 0, 0, 0, 15, user, cmd & "(-)")
                Return
            Case "r2"
                Controller(&H200, 0, 0, 0, 0, 0, 1, 10, user, cmd & "(!)")
                Controller(0, 0, 0, 0, 0, 0, 0, 20, user, cmd & "(-)")
                Return

            Case "ol1"
                Controller(&H2000, 0, 0, 0, 0, 0, 0, 4, user, cmd & "(!)")
                Controller(0, 0, 0, 0, 0, 0, 0, 16, user, cmd & "(-)")
                Controller(&H400, 0, 0, 0, 0, 0, 0, 4, user, cmd & "(!)")
                Controller(0, 0, 0, 0, 0, 0, 0, 24, user, cmd & "(-)")
                Return


            Case "cr2"
                Controller(&H200, 0, 0, 0, 0, 0, 1, 90, user, cmd & "(!)")
                Controller(0, 0, 0, 0, 0, 0, 0, 10, user, cmd & "(-)")
                Return

            Case "fr1"
                Controller(&H800, 0, 0, 0, 1, 0, 0, 20, user, cmd & "(!)")
                Controller(0, 0, 0, 0, 0, 0, 0, 10, user, cmd & "(-)")
                Return

            Case "fr2"
                Controller(0, 0, 0, 0, 0, 0, 0, 2, user, cmd & "(-)")
                Controller(&H200, 0, 0, 0, 1, 0, 1, 20, user, cmd & "(!)")
                Controller(0, 0, 0, 0, 0, 0, 0, 38, user, cmd & "(-)")
                Return

            Case "l3"
                Controller(&H2, 0, 0, 0, 0, 0, 0, 4, user, cmd & "(!)")
                Controller(0, 0, 0, 0, 0, 0, 0, 4, user, cmd & "(-)")
                Return

            Case "r3"
                Controller(&H4, 0, 0, 0, 0, 0, 0, 2, user, cmd & "(!)")
                Controller(0, 0, 0, 0, 0, 0, 0, 2, user, cmd & "(-)")
                Return

            Case "tpl"
                Controller(&H100000, 0, 0, 0, 0, 0, 0, 5, user, cmd)
                Return
            Case "tpr"
                Controller(&H100000, 0, 0, 0, 0, 0, 0, 5, user, cmd)
                Return
        End Select




        'If not handled above, proceed below.
        'pad to at least 5 characters
        If cmd.Length < 6 Then cmd = cmd & "....."



        'parse out half-hold
        If cmd(0) = "h" Then
            halfhold = True
            cmd = Strings.Right(cmd, cmd.Length - 1)
        End If





        'parse 'walks', 'looks', 'analog's, and 'rolls'
        If ((cmd(0) = "w") Or (cmd(0) = "l") Or (cmd(0) = "a")) Or
            (cmd(0) = "r" And cmd(1) = "o") Then

            Dim axispad = 0
            Dim cmdpad = 0
            Dim roll As Boolean = False


            'Set default walk duration if none specified
            If cmd(0) = "w" Then
                If duration = 0 Then duration = 38
            End If

            if cmd(0) = "a" then
                If duration = 0 Then duration = 38
                cmdparams = Mid(cmd, 2,4)
            End If

            
            'If 'roll', then roll params will be offset 1 character
            If cmd(0) = "r" And cmd(1) = "o" Then
                cmdpad = 1
                duration = 20
                roll = True
            End If

            'If 'look', then modify right stick's axises
            If cmd(0) = "l" Then
                axispad = 2
                If duration = 0 Then duration = 7
            End If



            'Return if garbage data
            For i = 1 To 5
                If Not {"f", "u", "b", "d", "l", "r", "1", "2", "3", "4", "5", "6", "7", "8", "9", "."
                        }.Contains(cmd(cmdpad + i)) Then
                    Return
                End If
            Next






            For i = 1 To 2
                'Handle lettered values
                Select Case cmd(cmdpad + i)
                    Case "f", "u"
                        mid(cmdparams, axispad + 2) = "1"
                    Case "b", "d"
                        mid(cmdparams, axispad + 2) = "9"
                    Case "l"
                        mid(cmdparams, axispad + 1) = "9"
                    Case "r"
                        mid(cmdparams, axispad + 1) = "1"
                End Select
                If cmd(cmdpad + i) >= "1" And cmd(cmdpad + i) <= "9" Then
                    Mid(cmdparams, axispad + i) = cmd(cmdpad + i)
                End If
            Next


            'Convert to stick values
            For i = 0 To 3
                axis(i) = (5 - Val(cmdparams(i))) / 4
                If halfhold Then
                    axis(i) = axis(i) / 2
                End If
            Next

            If halfhold Then cmd = "h" & cmd
            'Remove cmd padding
            cmd = cmd.Replace(".", "")

            If roll Then Controller(&H2000, axis(2), axis(3), axis(0), axis(1), 0, 0, 2, user, cmd & "(!)")
            Controller(0, axis(2), axis(3), axis(0), axis(1), 0, 0, duration, user, cmd)

        End If


    End Sub
    Private Sub Controller(buttons As Integer, RLR As Single, RUD As Single, LLR As Single, LUD As Single, LT As Single, RT As Single, hold As Integer, user As String, cmd As String)

        hold = hold * 33 'Fake 30fps

        If hold > 60000 Then hold = 60000

        If QueuedInput.Count > 0 Then
            If QueuedInput(QueuedInput.Count - 1).cmd = cmd Then
                QueuedInput(QueuedInput.Count - 1).time = QueuedInput(QueuedInput.Count - 1).time + hold
                Return
            End If
        End If
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
        WBytes(rpCtrlWrap + &H1D0980, {&H8B, &H55, &HC, &H83, &HC4, &HC})
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




