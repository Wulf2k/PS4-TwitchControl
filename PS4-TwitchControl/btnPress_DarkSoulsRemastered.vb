Imports System.Threading
Imports Nefarius.ViGEm.Client.Targets.DualShock4
Imports Nefarius.ViGEm.Client.Targets.Xbox360

Partial Public Class frmPS4Twitch



    Private Sub TimerPress_DarkSoulsRemastered()
        Dim timer = 16
        Do
            press()
            Do
                If timerfixer = -1 Then timerfixer = 1
                SyncLock presslock
                    presstimer -= (16 + Math.Abs(timerfixer))

                    timer = presstimer
                End SyncLock

                Thread.Sleep(16 + Math.Abs(timerfixer))
                timerfixer -= 1
            Loop While timer > 0
        Loop
    End Sub

    Private Sub btnPress_DarkSoulsRemastered()

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
                    Shell("cmd.exe /c taskkill /f /im DarkSoulsRemastered.*")
                    Thread.Sleep(1000)
                    Dim currDir = Microsoft.Win32.Registry.GetValue("HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 570940", "InstallLocation", Nothing)
                    Dim exe = $"{currDir}\DarkSoulsRemastered.exe"

                    Try
                        IO.File.WriteAllText($"{currDir}\steam_appid.txt", "570940")
                    Catch
                    End Try

                    Dim ProcessProperties As New ProcessStartInfo
                    ProcessProperties.FileName = exe
                    ProcessProperties.WorkingDirectory = currDir
                    Dim myProcess As Process = Process.Start(ProcessProperties)

                Case "savebackup"
                    Try
                        Dim fileloc = "C:\Users\dontw\OneDrive\Documents\NBGI\DARK SOULS REMASTERED\10279151\DRAKS0005.sl2"
                        Dim dst1 = "C:\temp\DRAKS0005.sl2"
                        Dim dst2 = $"C:\temp\DRAKS0005-{DateTime.Now.Year}{DateTime.Now.Month.ToString("D2")}{DateTime.Now.Day.ToString("D2")}{DateTime.Now.Hour.ToString("D2")}{DateTime.Now.Minute.ToString("D2")}{DateTime.Now.Second.ToString("D2")}.sl2"
                        IO.File.Copy(fileloc, dst1, True)
                        IO.File.Copy(fileloc, dst2, True)
                    Catch ex As Exception

                    End Try

                Case "saverestore"
                    Try

                    Catch ex As Exception
                        Dim fileloc = "C:\Users\dontw\OneDrive\Documents\NBGI\DARK SOULS REMASTERED\10279151\DRAKS0005.sl2"
                        Dim src = "C:\temp\DRAKS0005.sl2"

                        IO.File.Copy(src, fileloc, True)
                    End Try

                Case "hidecursor"
                    Dim x = 1600
                    Dim y = 1

                    Cursor.Position = New Point(x, y)

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



            Dim b(&H20) As Byte



            'Output queue info and pass to overlay program
            b = System.Text.Encoding.ASCII.GetBytes(user + Chr(0))
            mmfa.WriteArray(&H300, b, 0, b.Length)

            Dim tmpcmd
            SyncLock presslock
                tmpcmd = cmd & "-" & CInt(presstimer / 16.667)
            End SyncLock



            If tmpcmd(0) = "-" Then tmpcmd = ""

            b = System.Text.Encoding.ASCII.GetBytes(tmpcmd & Chr(0))
            mmfa.WriteArray(&H310, b, 0, b.Length)

            For i = 0 To 9
                If (QueuedInput.Count) > i Then
                    Dim str As String
                    str = QueuedInput(i).cmd & "-" & Math.Floor(QueuedInput(i).time / 16.667)

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
            'WUInt32(hookmem + &H40C, buttons)




            'DS4ctrl.SetButtonState(DualShock4Button.Circle, buttons And BTN_O)
            'DS4ctrl.SetButtonState(DualShock4Button.Cross, buttons And BTN_X)
            'DS4ctrl.SetButtonState(DualShock4Button.Square, buttons And BTN_SQUARE)
            'DS4ctrl.SetButtonState(DualShock4Button.Triangle, buttons And BTN_TRIANGLE)
            'DS4ctrl.SetButtonState(DualShock4Button.Share, buttons And BTN_SHARE)
            'DS4ctrl.SetButtonState(DualShock4Button.Options, buttons And BTN_OPTIONS)
            'DS4ctrl.SetButtonState(DualShock4Button.ShoulderLeft, buttons And BTN_L1)
            'DS4ctrl.SetButtonState(DualShock4Button.TriggerLeft, buttons And BTN_L2)
            'DS4ctrl.SetButtonState(DualShock4Button.ThumbLeft, buttons And BTN_L3)
            'DS4ctrl.SetButtonState(DualShock4Button.ShoulderRight, buttons And BTN_R1)
            'DS4ctrl.SetButtonState(DualShock4Button.TriggerRight, buttons And BTN_R2)
            'DS4ctrl.SetButtonState(DualShock4Button.ThumbRight, buttons And BTN_R3)

            'DS4ctrl.SetAxisValue(DualShock4Axis.LeftThumbX, &H7FUI + LStickLR * &H7FUI)
            'DS4ctrl.SetAxisValue(DualShock4Axis.LeftThumbY, &H7FUI + LStickUD * &H7FUI)
            'DS4ctrl.SetAxisValue(DualShock4Axis.RightThumbX, &H7FUI + RStickLR * &H7FUI)
            'DS4ctrl.SetAxisValue(DualShock4Axis.RightThumbY, &H7FUI + RStickUD * &H7FUI)

            'DS4ctrl.SetSliderValue(DualShock4Slider.LeftTrigger, &HFFUI * LTrigger)
            'DS4ctrl.SetSliderValue(DualShock4Slider.RightTrigger, &HFFUI * RTrigger)



            XBctrl.SetButtonState(Xbox360Button.B, buttons And BTN_O)
            XBctrl.SetButtonState(Xbox360Button.A, buttons And BTN_X)
            XBctrl.SetButtonState(Xbox360Button.X, buttons And BTN_SQUARE)
            XBctrl.SetButtonState(Xbox360Button.Y, buttons And BTN_TRIANGLE)
            XBctrl.SetButtonState(Xbox360Button.Back, buttons And BTN_SHARE)
            XBctrl.SetButtonState(Xbox360Button.Start, buttons And BTN_OPTIONS)
            XBctrl.SetButtonState(Xbox360Button.LeftShoulder, buttons And BTN_L1)
            XBctrl.SetButtonState(Xbox360Button.LeftThumb, buttons And BTN_L3)
            XBctrl.SetButtonState(Xbox360Button.RightShoulder, buttons And BTN_R1)
            XBctrl.SetButtonState(Xbox360Button.RightThumb, buttons And BTN_R3)


            XBctrl.SetAxisValue(Xbox360Axis.LeftThumbX, LStickLR * &H7FFFUI)
            XBctrl.SetAxisValue(Xbox360Axis.LeftThumbY, LStickUD * &H7FFFUI)
            XBctrl.SetAxisValue(Xbox360Axis.RightThumbX, RStickLR * &H7FFFUI)
            XBctrl.SetAxisValue(Xbox360Axis.RightThumbY, RStickUD * &H7FFFUI)

            XBctrl.SetSliderValue(Xbox360Slider.LeftTrigger, &HFFUI * LTrigger)
            XBctrl.SetSliderValue(Xbox360Slider.RightTrigger, &HFFUI * RTrigger)

            XBctrl.SetButtonState(Xbox360Button.Up, buttons And BTN_DPAD_UP)
            XBctrl.SetButtonState(Xbox360Button.Right, buttons And BTN_DPAD_RIGHT)
            XBctrl.SetButtonState(Xbox360Button.Down, buttons And BTN_DPAD_DOWN)
            XBctrl.SetButtonState(Xbox360Button.Left, buttons And BTN_DPAD_LEFT)

            XBctrl.SubmitReport()


            'Do DPad properly
            'Someday
            'Fuck you, Future-Wulf, you deal with this shit

            'If (buttons And BTN_DPAD_UP) Then DS4ctrl.SetDPadDirection(DualShock4DPadDirection.North)
            'If (buttons And BTN_DPAD_RIGHT) Then DS4ctrl.SetDPadDirection(DualShock4DPadDirection.East)
            'If (buttons And BTN_DPAD_DOWN) Then DS4ctrl.SetDPadDirection(DualShock4DPadDirection.South)
            'If (buttons And BTN_DPAD_LEFT) Then DS4ctrl.SetDPadDirection(DualShock4DPadDirection.West)
            'If (buttons And (BTN_DPAD_UP + BTN_DPAD_LEFT)) Then DS4ctrl.SetDPadDirection(DualShock4DPadDirection.Northwest)
            'If (buttons And (BTN_DPAD_UP + BTN_DPAD_RIGHT)) Then DS4ctrl.SetDPadDirection(DualShock4DPadDirection.Northeast)
            'If (buttons And (BTN_DPAD_DOWN + BTN_DPAD_LEFT)) Then DS4ctrl.SetDPadDirection(DualShock4DPadDirection.Southwest)
            'If (buttons And (BTN_DPAD_DOWN + BTN_DPAD_RIGHT)) Then DS4ctrl.SetDPadDirection(DualShock4DPadDirection.Southeast)




            'WUInt8(hookmem + &H410, &H7FUI + LStickLR * &H7FUI)
            'WUInt8(hookmem + &H411, &H7FUI - LStickUD * &H7FUI)
            'WUInt8(hookmem + &H412, &H7FUI + RStickLR * &H7FUI)
            'WUInt8(hookmem + &H413, &H7FUI - RStickUD * &H7FUI)

            'WUInt8(hookmem + &H414, &HFFUI * LTrigger)
            'WUInt8(hookmem + &H415, &HFFUI * RTrigger)
        Catch ex As Exception
            Console.WriteLine("WUInt8 stick value overflow? " & ex.Message)
        End Try


        'Catch ex As Exception
        'Console.WriteLine("press exception")
        ' End Try
    End Sub

    Private Sub execCMD_DarkSoulsRemastered(user As String, role As String, cmd As String)

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
                 "reconnect1",
                 "savebackup", "saverestore",
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
                'Controller(BTN_SHARE, 0, 0, 0, 0, 0, 0, 1, user, cmd)
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

            Case "fr1"
                If duration = 0 Then duration = 56
                Controller(0, 0, 0, 0, 0, 0, 0, 4, user, cmd & "(-)")
                Controller(BTN_R1, 0, 0, 0, 1, 0, 0, 4, user, cmd & "(!)")
                Controller(0, 0, 0, 0, 0, 0, 0, duration, user, cmd & "(-)")
                Return

            Case "fr2"
                If duration = 0 Then duration = 112
                Controller(0, 0, 0, 0, 0, 0, 0, 4, user, cmd & "(-)")
                Controller(BTN_R2, 0, 0, 0, 1, 0, 1, 4, user, cmd & "(!)")
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
            Next

            If halfhold Then cmd = "h" & cmd
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

    Private Sub ProcessCMD_DarkSoulsRemastered(user As String, role As String, cmd As String)
        Dim tmpuser = user
        Dim tmpcmd = cmd
        Dim CMDmulti As Integer = 1

        'allow loop of entire string, with inner loops



        If tmpcmd.Contains("*") Then
            CMDmulti = Val(tmpcmd.Split("*")(1))
            If CMDmulti > 20 Then CMDmulti = 20
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

            'Fine, roll over ints, see if I care.
            If CMDmulti < 0 Then CMDmulti = 0

            'Allow a maximum of 1000 loops
            If CMDmulti > 1000 Then CMDmulti = 1000
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

        tmpcmd = tmpcmd.ToLower
        tmpcmd = tmpcmd.Replace(" ", "")


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
            'Case "tpr", "tpl"      'Free for all on the tpwhatevs
            '   If Not (tmpuser = "wulf2k" Or tmpuser = "seannyb") Then
            'outputChat("Personal items restricted to pre-approved users.")
            '       Return
            'End If
            Case "reconnect1", "savebackup", "saverestore"
                If Not modlist.Contains(user) Then
                    Return
                End If
            Case "options", "opt", "hopt"
                If Not modlist.Contains(user) Then
                    outputChat("Options menu restricted to pre-approved users.")
                    Return
                End If
            Case "pshome"
                If Not (tmpuser = "wulf2k" Or tmpuser = "seannyb" Or tmpuser = "tompiet1") Then
                    outputChat("Uhh....  No.")
                    Return
                End If
            Case "tri", "htri"
                If Not modlist.Contains(user) Then
                    'outputChat("Consumable use restricted to pre-approved users.")
                    'Return
                End If
            Case "clearallcmds", "ca"
                'Testing removal of mod restriction on clear all commands


                'If Not (modlist.Contains(tmpuser)) Then
                'outputChat("Clearing all commands restricted to pre-approved users.")
                'ProcessCMD({tmpuser, "clearcmds"})
                'Else
                outputChat(tmpuser & " has cleared the command queue.")

                SyncLock queuelock
                    QueuedInput.Clear()
                End SyncLock

                ProcessCMD(tmpuser, role, "nha")

                SyncLock presslock
                    presstimer = 0
                End SyncLock


                'End If
            Case "clearcmds", "c"
                SyncLock queuelock
                    For i = QueuedInput.Count - 1 To 0 Step -1
                        If QueuedInput(i).user = tmpuser Then
                            QueuedInput.RemoveAt(i)
                        End If
                    Next
                End SyncLock
                ProcessCMD(tmpuser, role, "nha")
                outputChat("All commands for " & tmpuser & " removed from queue.")
                Return

        End Select




        'For direct analog stick inputs
        For i = 0 To CMDmulti - 1
            execCMD(tmpuser, role, tmpcmd)
        Next
    End Sub


End Class
