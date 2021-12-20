﻿Imports System.Threading


Partial Public Class frmPS4Twitch
    Dim which = "ms"

    Private Sub btnPress_Celeste()
        'frametime = 33333  '30fps
        frametime = 16667   '60fps

        'ctrlStyle = "switch"
        ctrlStyle = "xbox"

        ctrlType = "tt"
        'ctrlType = "vg"





        Dim buttons = 0
        Dim LStickLR As Single = 0
        Dim LStickUD As Single = 0
        Dim RStickLR As Single = 0
        Dim RStickUD As Single = 0
        Dim LTrigger As Single = 0
        Dim RTrigger As Single = 0
        Dim user As String = ""
        Dim cmd As String = ""



        If QueuedInput.Count = 1 AndAlso Not QueuedInput(0).cmd = "idle" Then
            Controller(0, 0, 0, 0, 0, 0, 0, 1, "", "idle")
        End If


        'Try
        SyncLock queuelock
            'TODO:  Fix up below, randomly seems to be hitting this spot with an empty queue
            If QueuedInput.Count < 1 Then
                Return

            End If


            buttons = QueuedInput(0).buttons

            'Handle hold-toggles
            Select Case QueuedInput(0).cmd
                Case "idle"
                    microTimer.Enabled = False
                    QueuedInput(0).cmd = ""


                Case "reconnect1"
                    Shell("cmd.exe /c taskkill /f /im celeste.*")
                    Thread.Sleep(5000)
                    Select Case which
                        Case "ms"
                            Shell("cmd.exe /c start shell:AppsFolder\MattMakesGamesInc.Celeste_79daxvg0dq3v6!App")

                        Case "nonms"
                            Shell("cmd.exe /c start c:\\temp\\celeste\\celeste.exe")
                    End Select


                Case "focus"
                    Dim hwnd As IntPtr
                    hwnd = FindWindowA(Nothing, "Celeste")
                    If Not hwnd.Equals(IntPtr.Zero) Then
                        ShowWindow(hwnd, 3)
                        SetForegroundWindow(hwnd)
                        outputChat("Celeste focused.")
                    Else
                        outputChat($"'Celeste' window not found.")
                    End If

                Case "hidecursor"
                    Dim x = 1600
                    Dim y = 1

                    Cursor.Position = New Point(x, y)
                    'mouse_event(MOUSEEVENTF_LEFTDOWN, x, y, 0, IntPtr.Zero)
                    'mouse_event(MOUSEEVENTF_LEFTUP, x, y, 0, IntPtr.Zero)

                Case "killoverlay"
                    Shell("taskkill /f /im ps4-twitchhelper.exe")

                Case "startoverlay"
                    'Shell("")
                    Dim ProcessProperties As New ProcessStartInfo
                    ProcessProperties.FileName = "C:\Users\Lane\Documents\GitHub\PS4-TwitchHelper\PS4-TwitchHelper\bin\Debug\PS4-TwitchHelper.exe"
                    'Dim myProcess As Process = Process.Start(ProcessProperties)



                Case "nh"
                    boolHoldL1 = False
                    boolHoldL2 = False
                    boolHoldL3 = False
                    boolHoldR1 = False
                    boolHoldR2 = False
                    boolHoldR3 = False
                    boolHoldO = False
                    boolHoldSq = False
                    boolHoldTri = False
                    boolHoldX = False
                    boolHoldDU = False
                    boolHoldDD = False
                    boolHoldDL = False
                    boolHoldDR = False
                    boolHoldShare = False
                    boolHoldOpt = False

                    For i = 0 To 3
                        boolHoldAxis(i) = False
                        boolHoldAxisVal(i) = 0
                    Next

                Case "nhw"
                    For i = 0 To 3
                        boolHoldAxis(i) = False
                        boolHoldAxisVal(i) = 0
                    Next

                Case "share", "hselect"
                    boolHoldShare = True
                Case "nhshare", "nhselect"
                    boolHoldShare = False

                Case "hopt"
                    boolHoldOpt = True
                Case "nhopt"
                    boolHoldOpt = False

                Case "hl1"
                    boolHoldL1 = True
                Case "nhl1"
                    boolHoldL1 = False

                Case "hl2"
                    boolHoldL2 = True
                Case "nhl2"
                    boolHoldL2 = False

                Case "hl3"
                    boolHoldL3 = True
                Case "nhl3"
                    boolHoldL3 = False


                Case "hr1"
                    boolHoldR1 = True
                Case "nhr1"
                    boolHoldR1 = False

                Case "hr2"
                    boolHoldR2 = True
                Case "nhr2"
                    boolHoldR2 = False

                Case "hr3"
                    boolHoldR3 = True
                Case "nhr3"
                    boolHoldR3 = False


                Case "ho", "holdo"
                    boolHoldO = True
                Case "nho", "noholdo"
                    boolHoldO = False

                Case "hsq"
                    boolHoldSq = True
                Case "nhsq"
                    boolHoldSq = False

                Case "htri"
                    boolHoldTri = True
                Case "nhtri"
                    boolHoldTri = False

                Case "hx"
                    boolHoldX = True
                Case "nhx"
                    boolHoldX = False

                Case "hdu"
                    boolHoldDU = True
                Case "nhdu"
                    boolHoldDU = False
                Case "hdd"
                    boolHoldDD = True
                Case "nhdd"
                    boolHoldDD = False
                Case "hdl"
                    boolHoldDL = True
                Case "nhdl"
                    boolHoldDL = False
                Case "hdr"
                    boolHoldDR = True
                Case "nhdr"
                    boolHoldDR = False
            End Select



            'If command has no duration, skip to next command.
            If QueuedInput(0).time < 1 Then
                PopQ()
                press()
                Return
            End If




            'Combine held inputs with specified presses
            If boolHoldL3 Then buttons = (buttons Or BTN_L3)
            If boolHoldR3 Then buttons = (buttons Or BTN_R3)
            If boolHoldOpt Then buttons = (buttons Or BTN_OPTIONS)

            If boolHoldDU Then buttons = (buttons Or BTN_DPAD_UP)
            If boolHoldDD Then buttons = (buttons Or BTN_DPAD_DOWN)
            If boolHoldDL Then buttons = (buttons Or BTN_DPAD_LEFT)
            If boolHoldDR Then buttons = (buttons Or BTN_DPAD_RIGHT)

            If boolHoldL2 Then
                buttons = (buttons Or BTN_L2)
                QueuedInput(0).LTrigger = 1
            End If
            If boolHoldR2 Then
                buttons = (buttons Or BTN_R2)
                QueuedInput(0).RTrigger = 1
            End If
            If boolHoldL1 Then buttons = (buttons Or BTN_L1)
            If boolHoldR1 Then buttons = (buttons Or BTN_R1)

            If boolHoldTri Then buttons = (buttons Or BTN_TRIANGLE)
            If boolHoldO Then buttons = (buttons Or BTN_O)
            If boolHoldX Then buttons = (buttons Or BTN_X)
            If boolHoldSq Then buttons = (buttons Or BTN_SQUARE)

            'Apply held analogs
            If boolHoldAxis(0) Then QueuedInput(0).LStickLR = boolHoldAxisVal(0)
            If boolHoldAxis(1) Then QueuedInput(0).LStickUD = boolHoldAxisVal(1)
            If boolHoldAxis(2) Then QueuedInput(0).RstickLR = boolHoldAxisVal(2)
            If boolHoldAxis(3) Then QueuedInput(0).RstickUD = boolHoldAxisVal(3)


            'Process specified axises
            LStickLR = QueuedInput(0).LStickLR
            LStickUD = QueuedInput(0).LStickUD
            RStickLR = QueuedInput(0).RstickLR
            RStickUD = QueuedInput(0).RstickUD
            LTrigger = QueuedInput(0).LTrigger
            RTrigger = QueuedInput(0).RTrigger
            user = QueuedInput(0).user
            cmd = QueuedInput(0).cmd
            'refTimerPress.Interval = QueuedInput(0).time



            SyncLock presslock
                QueuedInput(0).time -= 1
                presstimer = QueuedInput(0).time
            End SyncLock

            If presstimer = 0 Then PopQ()


            Dim b(&H20) As Byte



            'Output queue info and pass to overlay program
            b = System.Text.Encoding.ASCII.GetBytes(user + Chr(0))
            mmfa.WriteArray(&H300, b, 0, b.Length)

            Dim tmpcmd
            SyncLock presslock
                tmpcmd = cmd & "-" & presstimer
            End SyncLock



            If tmpcmd(0) = "-" Then tmpcmd = ""

            b = System.Text.Encoding.ASCII.GetBytes(tmpcmd & Chr(0))
            mmfa.WriteArray(&H310, b, 0, b.Length)

            For i = 1 To 9
                If (QueuedInput.Count) > i Then
                    Dim str As String
                    str = QueuedInput(i).cmd & "-" & QueuedInput(i).time

                    'if command too long, shorten it
                    If str.Length > 15 Then str = Strings.Left(str, 15)
                    str = str & Chr(0)

                    b = System.Text.Encoding.ASCII.GetBytes(str + Chr(0))
                    mmfa.WriteArray(&H320 + i * &H10, b, 0, b.Length)
                Else
                    mmfa.WriteArray(&H320 + i * &H10, {0}, 0, 1)
                End If
            Next

            mmfa.Write(&H3C0, QueuedInput.Count)

        End SyncLock

        'TODO:  Pass tpad values as part of controller queued input
        Select Case cmd
            Case "tpl"
                'WUInt8(hookmem + &H427, &H70)
                'WUInt16(hookmem + &H42A, &H100)
                'WUInt16(hookmem + &H42C, &H100)

            Case "tpr"
                'WUInt8(hookmem + &H427, &H70)
                'WUInt16(hookmem + &H42A, &H400)
                'WUInt16(hookmem + &H42C, &H100)

            Case Else
                'WUInt8(hookmem + &H427, &H80)
                'WUInt16(hookmem + &H42A, 0)
                'WUInt16(hookmem + &H42C, 0)
        End Select

        Try
            Select Case ctrlStyle
                Case "switch"
                    Select Case ctrlType
                        Case "tt"
                            P1output(gcapiTitanOne.TitanOne.Xbox.B) = IIf((buttons And BTN_O) >= 1, 100, 0)
                            P1output(gcapiTitanOne.TitanOne.Xbox.A) = IIf((buttons And BTN_X) >= 1, 100, 0)
                            P1output(gcapiTitanOne.TitanOne.Xbox.X) = IIf((buttons And BTN_SQUARE) >= 1, 100, 0)
                            P1output(gcapiTitanOne.TitanOne.Xbox.Y) = IIf((buttons And BTN_TRIANGLE) >= 1, 100, 0)

                            P1output(gcapiTitanOne.TitanOne.Xbox.Back) = IIf((buttons And BTN_SHARE) >= 1, 100, 0)
                            P1output(gcapiTitanOne.TitanOne.Xbox.Start) = IIf((buttons And BTN_OPTIONS) >= 1, 100, 0)
                            P1output(gcapiTitanOne.TitanOne.Xbox.Home) = IIf((buttons And BTN_PSHOME) >= 1, 100, 0)


                            P1output(gcapiTitanOne.TitanOne.Xbox.LeftShoulder) = IIf((buttons And BTN_L1) >= 1, 100, 0)
                            P1output(gcapiTitanOne.TitanOne.Xbox.LeftStick) = IIf((buttons And BTN_L3) >= 1, 100, 0)
                            P1output(gcapiTitanOne.TitanOne.Xbox.RightShoulder) = IIf((buttons And BTN_R1) >= 1, 100, 0)
                            P1output(gcapiTitanOne.TitanOne.Xbox.RightStick) = IIf((buttons And BTN_R3) >= 1, 100, 0)

                            P1output(gcapiTitanOne.TitanOne.Xbox.LeftX) = LStickLR * 100
                            P1output(gcapiTitanOne.TitanOne.Xbox.LeftY) = LStickUD * -100
                            P1output(gcapiTitanOne.TitanOne.Xbox.RightX) = RStickLR * 100
                            P1output(gcapiTitanOne.TitanOne.Xbox.RightY) = RStickUD * 100

                            P1output(gcapiTitanOne.TitanOne.Xbox.LeftTrigger) = LTrigger * 100
                            P1output(gcapiTitanOne.TitanOne.Xbox.RightTrigger) = RTrigger * 100

                            P1output(gcapiTitanOne.TitanOne.Xbox.Down) = IIf((buttons And BTN_DPAD_DOWN) >= 1, 100, 0)
                            P1output(gcapiTitanOne.TitanOne.Xbox.Right) = IIf((buttons And BTN_DPAD_RIGHT) >= 1, 100, 0)
                            P1output(gcapiTitanOne.TitanOne.Xbox.Left) = IIf((buttons And BTN_DPAD_LEFT) >= 1, 100, 0)
                            P1output(gcapiTitanOne.TitanOne.Xbox.Up) = IIf((buttons And BTN_DPAD_UP) >= 1, 100, 0)

                            tOne.Send(0, P1output)
                            'end case switch tt
                        Case "vg"

                            'end case switch vg
                    End Select
                    'end case switch

                Case "xbox"
                    Select Case ctrlType
                        Case "tt"
                            P1output(gcapiTitanOne.TitanOne.Xbox.B) = IIf((buttons And BTN_O) >= 1, 100, 0)
                            P1output(gcapiTitanOne.TitanOne.Xbox.A) = IIf((buttons And BTN_X) >= 1, 100, 0)
                            P1output(gcapiTitanOne.TitanOne.Xbox.X) = IIf((buttons And BTN_SQUARE) >= 1, 100, 0)
                            P1output(gcapiTitanOne.TitanOne.Xbox.Y) = IIf((buttons And BTN_TRIANGLE) >= 1, 100, 0)

                            P1output(gcapiTitanOne.TitanOne.Xbox.Back) = IIf((buttons And BTN_SHARE) >= 1, 100, 0)
                            P1output(gcapiTitanOne.TitanOne.Xbox.Start) = IIf((buttons And BTN_OPTIONS) >= 1, 100, 0)
                            P1output(gcapiTitanOne.TitanOne.Xbox.Home) = IIf((buttons And BTN_PSHOME) >= 1, 100, 0)


                            P1output(gcapiTitanOne.TitanOne.Xbox.LeftShoulder) = IIf((buttons And BTN_L1) >= 1, 100, 0)
                            P1output(gcapiTitanOne.TitanOne.Xbox.LeftStick) = IIf((buttons And BTN_L3) >= 1, 100, 0)
                            P1output(gcapiTitanOne.TitanOne.Xbox.RightShoulder) = IIf((buttons And BTN_R1) >= 1, 100, 0)
                            P1output(gcapiTitanOne.TitanOne.Xbox.RightStick) = IIf((buttons And BTN_R3) >= 1, 100, 0)

                            P1output(gcapiTitanOne.TitanOne.Xbox.LeftX) = LStickLR * 100
                            P1output(gcapiTitanOne.TitanOne.Xbox.LeftY) = LStickUD * -100
                            P1output(gcapiTitanOne.TitanOne.Xbox.RightX) = RStickLR * 100
                            P1output(gcapiTitanOne.TitanOne.Xbox.RightY) = RStickUD * 100

                            P1output(gcapiTitanOne.TitanOne.Xbox.LeftTrigger) = LTrigger * 100
                            P1output(gcapiTitanOne.TitanOne.Xbox.RightTrigger) = RTrigger * 100

                            P1output(gcapiTitanOne.TitanOne.Xbox.Down) = IIf((buttons And BTN_DPAD_DOWN) >= 1, 100, 0)
                            P1output(gcapiTitanOne.TitanOne.Xbox.Right) = IIf((buttons And BTN_DPAD_RIGHT) >= 1, 100, 0)
                            P1output(gcapiTitanOne.TitanOne.Xbox.Left) = IIf((buttons And BTN_DPAD_LEFT) >= 1, 100, 0)
                            P1output(gcapiTitanOne.TitanOne.Xbox.Up) = IIf((buttons And BTN_DPAD_UP) >= 1, 100, 0)

                            tOne.Send(0, P1output)

                            'end case xbox tt
                        Case "vg"
                            'XBctrl.SetButtonState(Xbox360Button.B, buttons And BTN_O)
                            'XBctrl.SetButtonState(Xbox360Button.A, buttons And BTN_X)
                            'XBctrl.SetButtonState(Xbox360Button.X, buttons And BTN_SQUARE)
                            'XBctrl.SetButtonState(Xbox360Button.Y, buttons And BTN_TRIANGLE)
                            'XBctrl.SetButtonState(Xbox360Button.Back, buttons And BTN_SHARE)
                            'XBctrl.SetButtonState(Xbox360Button.Start, buttons And BTN_OPTIONS)
                            'XBctrl.SetButtonState(Xbox360Button.LeftShoulder, buttons And BTN_L1)
                            'XBctrl.SetButtonState(Xbox360Button.LeftThumb, buttons And BTN_L3)
                            'XBctrl.SetButtonState(Xbox360Button.RightShoulder, buttons And BTN_R1)
                            'XBctrl.SetButtonState(Xbox360Button.RightThumb, buttons And BTN_R3)


                            'XBctrl.SetAxisValue(Xbox360Axis.LeftThumbX, LStickLR * &H7FFFUI)
                            'XBctrl.SetAxisValue(Xbox360Axis.LeftThumbY, LStickUD * &H7FFFUI)
                            'XBctrl.SetAxisValue(Xbox360Axis.RightThumbX, RStickLR * &H7FFFUI)
                            'XBctrl.SetAxisValue(Xbox360Axis.RightThumbY, RStickUD * &H7FFFUI)

                            'XBctrl.SetSliderValue(Xbox360Slider.LeftTrigger, &HFFUI * LTrigger)
                            'XBctrl.SetSliderValue(Xbox360Slider.RightTrigger, &HFFUI * RTrigger)

                            'XBctrl.SetButtonState(Xbox360Button.Up, buttons And BTN_DPAD_UP)
                            'XBctrl.SetButtonState(Xbox360Button.Right, buttons And BTN_DPAD_RIGHT)
                            'XBctrl.SetButtonState(Xbox360Button.Down, buttons And BTN_DPAD_DOWN)
                            'XBctrl.SetButtonState(Xbox360Button.Left, buttons And BTN_DPAD_LEFT)

                            'XBctrl.SubmitReport()
                            'end case xbox vg
                    End Select
                    'end case xbox
            End Select




        Catch ex As Exception
            Console.WriteLine("WUInt8 stick value overflow? " & ex.Message)
        End Try


        'Catch ex As Exception
        'Console.WriteLine("press exception")
        ' End Try
    End Sub

    Private Sub execCMD_Celeste(user As String, role As String, cmd As String)

        Dim buttons = 0
        Dim axis() As Single = {CSng(0), CSng(0), CSng(0), CSng(0)}
        Dim halfhold As Boolean = False
        Dim analoghold As Boolean = False
        Dim analogholdrelease As Boolean = False
        Dim duration As Integer = 0

        Dim partcmd As String = ""

        'Expect neutral stick values by default
        Dim cmdparams As String = "5555"


        If cmd.Contains("-") Then
            If IsNumeric(cmd.Split("-")(1)) Then duration = cmd.Split("-")(1)
            cmd = cmd.Split("-")(0)
        Else
            duration = 0
        End If



        Select Case cmd
            'Hold toggles
            Case "nh",
                 "hw", "nhw",
                 "holdo", "ho", "noholdo", "nho",
                 "hopt", "nhopt",
                 "hshare", "nhshare",
                 "hl1", "nhl1",
                 "hl2", "nhl2",
                 "hl3", "nhl3",
                 "hr1", "nhr1",
                 "hr2", "nhr2",
                 "hr3", "nhr3",
                 "hsq", "nhsq",
                 "htri", "nhtri",
                 "hx", "nhx",
                 "hdu", "nhdu",
                 "hdd", "nhdd",
                 "hdl", "nhdl",
                 "hdr", "nhdr",
                 "reconnect1",
                 "focus",
                 "hidecursor"


                Controller(0, 0, 0, 0, 0, 0, 0, duration, user, cmd)


                'If duration specified on hold command, assume there's an implied release at the end
                If duration > 0 Then
                    Controller(0, 0, 0, 0, 0, 0, 0, 0, user, "n" & cmd)
                End If
                Return

            'Half halt
            Case "hh"
                If duration = 0 Then duration = 30
                Controller(0, 0, 0, 0, 0, 0, 0, duration, user, cmd)
                Return

            'Halt
            Case "h"
                If duration = 0 Then duration = 60
                Controller(0, 0, 0, 0, 0, 0, 0, duration, user, cmd)
                Return

            'Our old, archaic friend 'flong', "forward long"
            Case "flong"
                If duration = 0 Then duration = 180
                Controller(0, 0, 0, 0, 1, 0, 0, duration, user, cmd)
                Return


            Case "du"
                If duration = 0 Then duration = 4
                Controller(BTN_DPAD_UP, 0, 0, 0, 0, 0, 0, 4, user, cmd & "(!)")
                Controller(0, 0, 0, 0, 0, 0, 0, duration, user, cmd & "(-)")
                Return
            Case "dd"
                If duration = 0 Then duration = 4
                Controller(BTN_DPAD_DOWN, 0, 0, 0, 0, 0, 0, 4, user, cmd & "(!)")
                Controller(0, 0, 0, 0, 0, 0, 0, duration, user, cmd & "(-)")
                Return
            Case "dl"
                If duration = 0 Then duration = 4
                Controller(BTN_DPAD_LEFT, 0, 0, 0, 0, 0, 0, 4, user, cmd & "(!)")
                Controller(0, 0, 0, 0, 0, 0, 0, duration, user, cmd & "(-)")
                Return
            Case "dr"
                If duration = 0 Then duration = 4
                Controller(BTN_DPAD_RIGHT, 0, 0, 0, 0, 0, 0, 4, user, cmd & "(!)")
                Controller(0, 0, 0, 0, 0, 0, 0, duration, user, cmd & "(-)")
                Return

            Case "share"
                If duration = 0 Then duration = 2
                Controller(BTN_SHARE, 0, 0, 0, 0, 0, 0, 2, user, cmd & "(!)")
                Controller(0, 0, 0, 0, 0, 0, 0, duration, user, cmd & "(-)")
                Return

            Case "options", "opt"
                If duration = 0 Then duration = 4
                Controller(BTN_OPTIONS, 0, 0, 0, 0, 0, 0, 4, user, cmd & "(!)")
                Controller(0, 0, 0, 0, 0, 0, 0, duration, user, cmd & "(-)")
                Return

            Case "o"
                If duration = 0 Then duration = 36
                Controller(BTN_O, 0, 0, 0, 0, 0, 0, 4, user, cmd & "(!)")
                Controller(0, 0, 0, 0, 0, 0, 0, duration, user, cmd & "(-)")
                Return

            Case "x"
                If duration = 0 Then duration = 36
                Controller(BTN_X, 0, 0, 0, 0, 0, 0, 4, user, cmd & "(!)")
                Controller(0, 0, 0, 0, 0, 0, 0, duration, user, cmd & "(-)")
                Return

            Case "sq"
                If duration = 0 Then duration = 36
                Controller(BTN_SQUARE, 0, 0, 0, 0, 0, 0, 4, user, cmd & "(!)")
                Controller(0, 0, 0, 0, 0, 0, 0, duration, user, cmd & "(-)")
                Return

            Case "tri"
                If duration = 0 Then duration = 56
                Controller(BTN_TRIANGLE, 0, 0, 0, 0, 0, 0, 4, user, cmd & "(!)")
                Controller(0, 0, 0, 0, 0, 0, 0, duration, user, cmd & "(-)")
                Return


            Case "l1"
                If duration = 0 Then duration = 36
                Controller(BTN_L1, 0, 0, 0, 0, 0, 0, 4, user, cmd & "(!)")
                Controller(0, 0, 0, 0, 0, 0, 0, duration, user, cmd & "(-)")
                Return
            Case "l2"
                If duration = 0 Then duration = 56
                Controller(BTN_L2, 0, 0, 0, 0, 1, 0, 4, user, cmd & "(!)")
                Controller(0, 0, 0, 0, 0, 0, 0, duration, user, cmd & "(-)")
                Return
            Case "r1"
                If duration = 0 Then duration = 56
                Controller(BTN_R1, 0, 0, 0, 0, 0, 0, 4, user, cmd & "(!)")
                Controller(0, 0, 0, 0, 0, 0, 0, duration, user, cmd & "(-)")
                Return
            Case "r2"
                If duration = 0 Then duration = 56
                Controller(BTN_R2, 0, 0, 0, 0, 0, 1, 4, user, cmd & "(!)")
                Controller(0, 0, 0, 0, 0, 0, 0, duration, user, cmd & "(-)")
                Return



            Case "l3"
                If duration = 0 Then duration = 4
                Controller(BTN_L3, 0, 0, 0, 0, 0, 0, 4, user, cmd & "(!)")
                Controller(0, 0, 0, 0, 0, 0, 0, duration, user, cmd & "(-)")
                Return

            Case "r3"
                If duration = 0 Then duration = 4
                Controller(BTN_R3, 0, 0, 0, 0, 0, 0, 4, user, cmd & "(!)")
                Controller(0, 0, 0, 0, 0, 0, 0, duration, user, cmd & "(-)")
                Return



            Case "pshome"
                Controller(BTN_PSHOME, 0, 0, 0, 0, 0, 0, 2, user, cmd & "(!)")
                Controller(0, 0, 0, 0, 0, 0, 0, 2, user, cmd & "(-)")
                Return



            Case "retry"
                If duration = 0 Then duration = 110
                ProcessCMD(user, role, $"nh-2,opt-2,dd-2,x-{duration},")
                Return
        End Select







        'If not handled above, proceed below.
        'pad to at least 5 characters
        If cmd.Length < 6 Then cmd = cmd & "....."



        'parse out no-hold
        If cmd(0) = "n" Then
            analogholdrelease = True
            cmd = Strings.Right(cmd, cmd.Length - 1)
        End If

        'parse out half-hold
        If cmd(0) = "h" Then
            If duration > 0 Then analoghold = True
            cmd = Strings.Right(cmd, cmd.Length - 1)
        End If

        'parse out short-walk
        If cmd(0) = "s" Then
            halfhold = True
            cmd = Strings.Right(cmd, cmd.Length - 1)
        End If



        'parse 'walks', 'looks', 'analog's, and 'rolls'
        If ((cmd(0) = "w") Or (cmd(0) = "l") Or (cmd(0) = "a") Or (cmd(0) = "j")) Or
            (Strings.Left(cmd, 2) = "da") Then

            Dim axispad = 0
            Dim cmdpad = 0
            Dim jump As Boolean = False
            Dim dash As Boolean = False


            'Set default walk duration if none specified
            If cmd(0) = "w" Then
                If duration = 0 Then duration = 60
            End If

            If cmd(0) = "a" Then
                'TODO:  Damnit this is ugly.  Redo, with proper parsing.
                cmd = Strings.Left(cmd.Replace(".", "5"), 5)
                If duration = 0 Then duration = 60
                cmdparams = Mid(cmd, 2, 4)
            End If



            If Strings.Left(cmd, 2) = "da" Then
                cmdpad = 1
                If duration = 0 Then duration = 12
                dash = True
            End If

            If cmd(0) = "j" Then
                If duration = 0 Then duration = 12
                jump = True
            End If

            'If 'look', then modify right stick's axises
            If cmd(0) = "l" Then
                axispad = 2
                If duration = 0 Then duration = 14
            End If



            'Return if garbage data
            For i = 1 To 4
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
                If analoghold And Not axis(i) = 0 Then
                    boolHoldAxis(i) = Not analogholdrelease
                    boolHoldAxisVal(i) = axis(i)
                End If
            Next



            If halfhold Then cmd = "s" & cmd
            If analoghold Then cmd = "h" & cmd
            If analogholdrelease Then cmd = "n" & cmd

            'Remove cmd padding
            cmd = cmd.Replace(".", "")

            If jump Then
                Controller(BTN_X, axis(2), axis(3), axis(0), axis(1), 0, 0, duration, user, cmd & "(!)")
            ElseIf dash Then
                Controller(BTN_SQUARE, axis(2), axis(3), axis(0), axis(1), 0, 0, 2, user, cmd & "(!)")
                Controller(0, axis(2), axis(3), axis(0), axis(1), 0, 0, duration - 2, user, cmd & "(-)")
            Else
                Controller(0, axis(2), axis(3), axis(0), axis(1), 0, 0, duration, user, cmd)
            End If


        End If
    End Sub

End Class
