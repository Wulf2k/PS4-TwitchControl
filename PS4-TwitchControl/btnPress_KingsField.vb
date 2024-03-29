﻿Imports System.Threading

Partial Public Class frmPS4Twitch
    Private Sub btnPress_KingsField()

        Dim buttons = 0
        Dim LStickLR As Single = 0
        Dim LStickUD As Single = 0
        Dim RStickLR As Single = 0
        Dim RStickUD As Single = 0
        Dim LTrigger As Single = 0
        Dim RTrigger As Single = 0
        Dim user As String = ""
        Dim cmd As String = ""

        SyncLock queuelock
            'If nothing in queue, push a 'nothing'-press onto it for 1 frame
            If QueuedInput.Count = 0 Then
                Controller(0, 0, 0, 0, 0, 0, 0, 1, "", "")
            End If
        End SyncLock

        'Try
        SyncLock queuelock

            'TODO:  Fix up below, randomly seems to be hitting this spot with an empty queue
            If QueuedInput.Count = 0 Then Return


            buttons = QueuedInput(0).buttons

            'Handle hold-toggles
            Select Case QueuedInput(0).cmd

                Case "reconnect1"

                    Dim hndRpWindow As IntPtr
                    Dim hndButtOk As IntPtr
                    hndRpWindow = FindWindowA(vbNullString, "PS4 Remote Play")
                    hndButtOk = FindWindowExA(hndRpWindow, 0, "WindowsForms10.BUTTON.app.0.141b42a_r9_ad1", "OK")
                    SetForegroundWindow(hndButtOk)


                    Dim pos As RECT
                    GetWindowRect(hndButtOk, pos)

                    Dim x = pos.Left + 10
                    Dim y = pos.Top + 10

                    Cursor.Position = New Point(x, y)
                    mouse_event(MOUSEEVENTF_LEFTDOWN, x, y, 0, IntPtr.Zero)
                    mouse_event(MOUSEEVENTF_LEFTUP, x, y, 0, IntPtr.Zero)


                Case "reconnect2"
                    Dim hndRpWindow As IntPtr
                    Dim hndButtOk As IntPtr

                    hndRpWindow = FindWindowA(vbNullString, "PS4 Remote Play")
                    hndButtOk = FindWindowExA(hndRpWindow, 0, "WindowsForms10.STATIC.app.0.141b42a_r9_ad1", vbNullString)
                    Console.WriteLine(hndButtOk)

                    SetForegroundWindow(hndButtOk)

                    Dim pos As RECT
                    GetWindowRect(hndButtOk, pos)

                    Dim x = pos.Left + 620
                    Dim y = pos.Top + 320

                    Cursor.Position = New Point(x, y)
                    mouse_event(MOUSEEVENTF_LEFTDOWN, x, y, 0, IntPtr.Zero)
                    mouse_event(MOUSEEVENTF_LEFTUP, x, y, 0, IntPtr.Zero)


                Case "reconnect3"
                    Dim hndRpWindow As IntPtr
                    hndRpWindow = FindWindowA(vbNullString, "PS4 Remote Play")
                    SetForegroundWindow(hndRpWindow)

                    Dim pos As RECT
                    GetWindowRect(hndRpWindow, pos)

                    Dim x = pos.Right - 50
                    Dim y = pos.Bottom - 50

                    Cursor.Position = New Point(x, y)
                    Thread.Sleep(500)
                    mouse_event(MOUSEEVENTF_LEFTDOWN, x, y, 0, IntPtr.Zero)
                    mouse_event(MOUSEEVENTF_LEFTUP, x, y, 0, IntPtr.Zero)
                    Cursor.Position = New Point(-50, -50)

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



                Case "nha"
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
                    boolHoldOpt = False

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
            If QueuedInput(0).time = 0 Then
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
                presstimer = QueuedInput(0).time
            End SyncLock

            PopQ()




            'check for rolls during holdo
            If boolHoldO Then
                If Strings.Left(cmd, 2) = "ro" Or (cmd = "o") Then
                    outputChat("Evade failed due to HoldO being active.")
                End If
            End If



            'Output queue info and pass to overlay program
            WBytes(hookmem + &H300,
                   System.Text.Encoding.ASCII.GetBytes(user + Chr(0)))

            Dim tmpcmd
            SyncLock presslock
                tmpcmd = cmd & "-" & presstimer / 33
            End SyncLock



            If tmpcmd = "-1" Then tmpcmd = ""

            WBytes(hookmem + &H310,
                   System.Text.Encoding.ASCII.GetBytes(tmpcmd & Chr(0)))

            For i = 0 To 9
                If (QueuedInput.Count) > i Then
                    Dim str As String
                    str = QueuedInput(i).cmd & "-" & Math.Floor(QueuedInput(i).time / 33)

                    'if command too long, shorten it
                    If str.Length > 15 Then str = Strings.Left(str, 15)
                    str = str & Chr(0)

                    WBytes(hookmem + &H320 + i * &H10, System.Text.Encoding.ASCII.GetBytes(str))
                Else
                    WBytes(hookmem + &H320 + i * &H10, {0})
                End If
            Next

            WInt32(hookmem + &H3C0, QueuedInput.Count)

        End SyncLock

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

        Try
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
            Console.WriteLine("WUInt8 stick value overflow? " & ex.Message)
        End Try


        'Catch ex As Exception
        'Console.WriteLine("press exception")
        ' End Try
    End Sub
    Private Sub execCMD_KingsField(user As String, role As String, cmd As String)
        'Console.WriteLine($"{user} = {role}: {cmd}")

        Dim buttons = 0
        Dim axis() As Single = {CSng(0), CSng(0), CSng(0), CSng(0)}
        Dim halfhold As Boolean = False
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
            Case "nha",
                 "holdo", "ho", "noholdo", "nho",
                 "hopt", "nhopt",
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
                 "reconnect1", "reconnect2", "reconnect3",
                 "hidecursor",
                 "killoverlay", "startoverlay"

                Controller(0, 0, 0, 0, 0, 0, 0, duration, user, cmd)


                'If duration specified on hold command, assume there's an implied release at the end
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
                If duration = 0 Then duration = 90
                Controller(0, 0, 0, 0, 1, 0, 0, duration, user, cmd)
                Return



            Case "du"
                If duration = 0 Then duration = 2
                Controller(BTN_DPAD_UP, 0, 0, 0, 0, 0, 0, 2, user, cmd & "(!)")
                Controller(0, 0, 0, 0, 0, 0, 0, duration, user, cmd & "(-)")
                Return
            Case "dd"
                If duration = 0 Then duration = 2
                Controller(BTN_DPAD_DOWN, 0, 0, 0, 0, 0, 0, 2, user, cmd & "(!)")
                Controller(0, 0, 0, 0, 0, 0, 0, duration, user, cmd & "(-)")
                Return
            Case "dl"
                If duration = 0 Then duration = 2
                Controller(BTN_DPAD_LEFT, 0, 0, 0, 0, 0, 0, 2, user, cmd & "(!)")
                Controller(0, 0, 0, 0, 0, 0, 0, duration, user, cmd & "(-)")
                Return
            Case "dr"
                If duration = 0 Then duration = 2
                Controller(BTN_DPAD_RIGHT, 0, 0, 0, 0, 0, 0, 2, user, cmd & "(!)")
                Controller(0, 0, 0, 0, 0, 0, 0, duration, user, cmd & "(-)")
                Return

            Case "share"
                'Controller(BTN_SHARE, 0, 0, 0, 0, 0, 0, 1, user, cmd)
                Return

            Case "options", "opt"
                If duration = 0 Then duration = 2
                Controller(BTN_OPTIONS, 0, 0, 0, 0, 0, 0, 2, user, cmd & "(!)")
                Controller(0, 0, 0, 0, 0, 0, 0, duration, user, cmd & "(-)")
                Return

            Case "o"
                If duration = 0 Then duration = 18
                Controller(BTN_O, 0, 0, 0, 0, 0, 0, 2, user, cmd & "(!)")
                Controller(0, 0, 0, 0, 0, 0, 0, duration, user, cmd & "(-)")
                Return

            Case "x"
                If duration = 0 Then duration = 18
                Controller(BTN_X, 0, 0, 0, 0, 0, 0, 2, user, cmd & "(!)")
                Controller(0, 0, 0, 0, 0, 0, 0, duration, user, cmd & "(-)")
                Return

            Case "sq"
                If duration = 0 Then duration = 18
                Controller(BTN_SQUARE, 0, 0, 0, 0, 0, 0, 2, user, cmd & "(!)")
                Controller(0, 0, 0, 0, 0, 0, 0, duration, user, cmd & "(-)")
                Return

            Case "tri"
                If duration = 0 Then duration = 28
                Controller(BTN_TRIANGLE, 0, 0, 0, 0, 0, 0, 2, user, cmd & "(!)")
                Controller(0, 0, 0, 0, 0, 0, 0, duration, user, cmd & "(-)")
                Return


            Case "l1"
                If duration = 0 Then duration = 18
                Controller(BTN_L1, 0, 0, 0, 0, 0, 0, 2, user, cmd & "(!)")
                Controller(0, 0, 0, 0, 0, 0, 0, duration, user, cmd & "(-)")
                Return
            Case "l2"
                If duration = 0 Then duration = 28
                Controller(BTN_L2, 0, 0, 0, 0, 1, 0, 2, user, cmd & "(!)")
                Controller(0, 0, 0, 0, 0, 0, 0, duration, user, cmd & "(-)")
                Return
            Case "r1"
                If duration = 0 Then duration = 28
                Controller(BTN_R1, 0, 0, 0, 0, 0, 0, 2, user, cmd & "(!)")
                Controller(0, 0, 0, 0, 0, 0, 0, duration, user, cmd & "(-)")
                Return
            Case "r2"
                If duration = 0 Then duration = 28
                Controller(BTN_R2, 0, 0, 0, 0, 0, 1, 2, user, cmd & "(!)")
                Controller(0, 0, 0, 0, 0, 0, 0, duration, user, cmd & "(-)")
                Return

            Case "ol1"
                If duration = 0 Then duration = 26
                Controller(BTN_O, 0, 0, 0, 0, 0, 0, 2, user, cmd & "(!)")
                Controller(0, 0, 0, 0, 0, 0, 0, 18, user, cmd & "(-)")
                Controller(BTN_L1, 0, 0, 0, 0, 0, 0, 2, user, cmd & "(!)")
                Controller(0, 0, 0, 0, 0, 0, 0, duration, user, cmd & "(-)")
                Return


            Case "cr2"
                Controller(BTN_R2, 0, 0, 0, 0, 0, 1, 90, user, cmd & "(!)")
                Controller(0, 0, 0, 0, 0, 0, 0, 10, user, cmd & "(-)")
                Return

            Case "fr1"
                If duration = 0 Then duration = 28
                Controller(0, 0, 0, 0, 0, 0, 0, 2, user, cmd & "(-)")
                Controller(BTN_R1, 0, 0, 0, 1, 0, 0, 2, user, cmd & "(!)")
                Controller(0, 0, 0, 0, 0, 0, 0, duration, user, cmd & "(-)")
                Return

            Case "fr2"
                If duration = 0 Then duration = 56
                Controller(0, 0, 0, 0, 0, 0, 0, 2, user, cmd & "(-)")
                Controller(BTN_R2, 0, 0, 0, 1, 0, 1, 2, user, cmd & "(!)")
                Controller(0, 0, 0, 0, 0, 0, 0, duration, user, cmd & "(-)")
                Return

            Case "l3"
                If duration = 0 Then duration = 2
                Controller(BTN_L3, 0, 0, 0, 0, 0, 0, 2, user, cmd & "(!)")
                Controller(0, 0, 0, 0, 0, 0, 0, duration, user, cmd & "(-)")
                Return

            Case "r3"
                If duration = 0 Then duration = 2
                Controller(BTN_R3, 0, 0, 0, 0, 0, 0, 2, user, cmd & "(!)")
                Controller(0, 0, 0, 0, 0, 0, 0, duration, user, cmd & "(-)")
                Return

            Case "tpl"
                Controller(BTN_TOUCHPAD, 0, 0, 0, 0, 0, 0, 2, user, cmd)
                Controller(0, 0, 0, 0, 0, 0, 0, 2, user, cmd)
                Return
            Case "tpr"
                Controller(BTN_TOUCHPAD, 0, 0, 0, 0, 0, 0, 2, user, cmd)
                Controller(0, 0, 0, 0, 0, 0, 0, 2, user, cmd)
                Return

            Case "pshome"
                Controller(BTN_PSHOME, 0, 0, 0, 0, 0, 0, 2, user, cmd & "(!)")
                Controller(0, 0, 0, 0, 0, 0, 0, 2, user, cmd & "(-)")
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
            (Strings.Left(cmd, 2) = "ro") Then

            Dim axispad = 0
            Dim cmdpad = 0
            Dim roll As Boolean = False


            'Set default walk duration if none specified
            If cmd(0) = "w" Then
                If duration = 0 Then duration = 30
            End If

            If cmd(0) = "a" Then
                'TODO:  Damnit this is ugly.  Redo, with proper parsing.
                cmd = Strings.Left(cmd.Replace(".", "5"), 5)
                If duration = 0 Then duration = 30
                cmdparams = Mid(cmd, 2, 4)
            End If


            'If 'roll', then roll params will be offset 1 character
            If Strings.Left(cmd, 2) = "ro" Then
                cmdpad = 1
                If duration = 0 Then duration = 20
                roll = True
            End If

            'If 'look', then modify right stick's axises
            If cmd(0) = "l" Then
                axispad = 2
                If duration = 0 Then duration = 7
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
            Next

            If halfhold Then cmd = "h" & cmd
            'Remove cmd padding
            cmd = cmd.Replace(".", "")

            If roll Then
                Controller(BTN_O, axis(2), axis(3), axis(0), axis(1), 0, 0, 2, user, cmd & "(!)")
                Controller(0, axis(2), axis(3), axis(0), axis(1), 0, 0, duration, user, cmd & "(-)")
            Else
                Controller(0, axis(2), axis(3), axis(0), axis(1), 0, 0, duration, user, cmd)
            End If


        End If
    End Sub

End Class
