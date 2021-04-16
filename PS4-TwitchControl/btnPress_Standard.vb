Imports System.Threading

Partial Public Class frmPS4Twitch
    Private Sub btnPress_Standard()

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

End Class
