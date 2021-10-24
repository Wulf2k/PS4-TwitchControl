Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports System.Runtime.InteropServices
Imports System.IO

Namespace gcapiTitanOne
    Public Class titanOne

        Public strAPI As String = "gcdapi.dll"

        Public Declare Function LoadLibrary Lib "kernel32.dll" (ByVal dllToLoad As String) As IntPtr
        Public Declare Function GetProcAddress Lib "kernel32.dll" (ByVal hModule As IntPtr, procedureName As String) As IntPtr

        Public Structure GCAPI_CONSTANTS
            Public Const GCAPI_INPUT_TOTAL As Integer = 30
            Public Const GCAPI_OUTPUT_TOTAL As Integer = 36
        End Structure
        Public Structure GCAPI_STATUS
            Public value As Byte 'Current value - Range: [-100 ~ 100] %
            Public prev_value As Byte 'Previous value - Range: [-100 ~ 100] %
            Public press_tv As Integer 'Time marker For the button press Event
        End Structure
        Public Structure GCAPI_REPORT_TITANONE
            Public console As Byte
            Public controller As Byte
            <MarshalAs(UnmanagedType.ByValArray, SizeConst:=4)>
            Public led As Byte()
            <MarshalAs(UnmanagedType.ByValArray, SizeConst:=4)>
            Public rumble As Byte()
            Public battery_level As Byte
            <MarshalAs(UnmanagedType.ByValArray, SizeConst:=GCAPI_CONSTANTS.GCAPI_INPUT_TOTAL, ArraySubType:=UnmanagedType.Struct)>
            Public input As GCAPI_STATUS()
        End Structure

        <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
        Private Delegate Function GCAPI_LOAD() As Byte
        <UnmanagedFunctionPointer(CallingConvention.StdCall)>
        Private Delegate Function GCAPI_LOADDEVICE(ByVal devPID As UShort) As Byte
        <UnmanagedFunctionPointer(CallingConvention.StdCall)>
        Private Delegate Function GCAPI_ISCONNECTED() As Byte
        <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
        Private Delegate Function GCAPI_GETTIMEVAL() As UInteger
        <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
        Private Delegate Function GCAPI_GETFWVER() As UInteger
        <UnmanagedFunctionPointer(CallingConvention.StdCall)>
        Private Delegate Function GCAPI_WRITE(ByVal output As Byte()) As Byte
        <UnmanagedFunctionPointer(CallingConvention.StdCall)>
        Private Delegate Function GCAPI_WRITE_EX(ByVal output As Byte()) As Byte
        <UnmanagedFunctionPointer(CallingConvention.StdCall)>
        Private Delegate Function GCAPI_WRITEREF(ByVal output As Byte()) As Byte
        <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
        Private Delegate Function GCAPI_CALCPRESSTIME(ByVal time As Byte) As Integer
        <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
        Private Delegate Sub GCAPI_UNLOAD()
        <UnmanagedFunctionPointer(CallingConvention.StdCall)>
        Private Delegate Function GCAPI_READ_TO(
        <[In], Out> ByRef gcapi_report As GCAPI_REPORT_TITANONE) As IntPtr


        Private _gcapi_Load As GCAPI_LOAD = Nothing
        Private _gcapi_LoadDevice As GCAPI_LOADDEVICE = Nothing
        Private _gcapi_IsConnected As GCAPI_ISCONNECTED = Nothing
        Private _gcapi_GetTimeVal As GCAPI_GETTIMEVAL = Nothing
        Private _gcapi_GetFwVer As GCAPI_GETFWVER = Nothing
        Private _gcapi_Write As GCAPI_WRITE = Nothing
        Private _gcapi_WriteEx As GCAPI_WRITE_EX = Nothing
        Private _gcapi_WriteRef As GCAPI_WRITEREF = Nothing
        Private _gcapi_Read_TO As GCAPI_READ_TO = Nothing
        Private _gcapi_CalcPressTime As GCAPI_CALCPRESSTIME = Nothing
        Private _gcapi_Unload As GCAPI_UNLOAD = Nothing


        Public Enum DevPID
            Any = &H0
            ControllerMax = &H1
            Cronus = &H2
            TitanOne = &H3
        End Enum


        Enum xbox
            home
            back
            start
            rightShoulder
            rightTrigger
            rightStick
            leftShoulder
            leftTrigger
            leftStick
            rightX
            rightY
            leftX
            leftY
            up
            down
            left
            right
            y
            b
            a
            x
            accX 'rotate X. 90 = -25, 180 = 0, 270 = +25, 360 = 0 
            accY 'shake vertically. +25 (top) To -25 (bottom) 
            accZ 'tilt up
            gyroX
            gyroY
            gyroZ
            touch 'touchpad, 100 = On    (works)
            touchX '-100 To 100   (left To right)
            touchY '-100 To 100   (top To bottom)
        End Enum

        Private _strTODevice As String
        Private _boolGCAPILoaded As Boolean = False
        Private _devId As DevPID = DevPID.Any

        Public Sub setTOInterface(ByVal devID As DevPID)
            _devId = devID
        End Sub

        Public Sub initTitanOne()
            Dim strDir = Directory.GetCurrentDirectory() & "\"
            If File.Exists(strDir + strAPI) = False Then Return
            Dim ptrDll = LoadLibrary(strDir + strAPI)
            If ptrDll = IntPtr.Zero Then Return
            Dim ptrLoad = loadExternalFunction(ptrDll, "gcdapi_Load")
            If ptrLoad = IntPtr.Zero Then Return
            Dim ptrLoadDevice = loadExternalFunction(ptrDll, "gcdapi_LoadDevice")
            If ptrLoadDevice = IntPtr.Zero Then Return
            Dim ptrIsConnected = loadExternalFunction(ptrDll, "gcapi_IsConnected")
            If ptrIsConnected = IntPtr.Zero Then Return
            Dim ptrUnload = loadExternalFunction(ptrDll, "gcdapi_Unload")
            If ptrUnload = IntPtr.Zero Then Return
            Dim ptrGetTimeVal = loadExternalFunction(ptrDll, "gcapi_GetTimeVal")
            If ptrGetTimeVal = IntPtr.Zero Then Return
            Dim ptrGetFwVer = loadExternalFunction(ptrDll, "gcapi_GetFWVer")
            If ptrGetFwVer = IntPtr.Zero Then Return
            Dim ptrWrite = loadExternalFunction(ptrDll, "gcapi_Write")
            If ptrWrite = IntPtr.Zero Then Return
            Dim ptrRead = loadExternalFunction(ptrDll, "gcapi_Read")
            If ptrRead = IntPtr.Zero Then Return
            Dim ptrWriteEx = loadExternalFunction(ptrDll, "gcapi_WriteEX")
            If ptrWriteEx = IntPtr.Zero Then Return
            Dim ptrReadEx = loadExternalFunction(ptrDll, "gcapi_ReadEX")
            If ptrReadEx = IntPtr.Zero Then Return
            Dim ptrCalcPressTime = loadExternalFunction(ptrDll, "gcapi_CalcPressTime")
            If ptrCalcPressTime = IntPtr.Zero Then Return

            Try
                _gcapi_Load = CType(Marshal.GetDelegateForFunctionPointer(ptrLoad, GetType(GCAPI_LOAD)), GCAPI_LOAD)
                _gcapi_LoadDevice = CType(Marshal.GetDelegateForFunctionPointer(ptrLoadDevice, GetType(GCAPI_LOADDEVICE)), GCAPI_LOADDEVICE)
                _gcapi_IsConnected = CType(Marshal.GetDelegateForFunctionPointer(ptrIsConnected, GetType(GCAPI_ISCONNECTED)), GCAPI_ISCONNECTED)
                _gcapi_Unload = CType(Marshal.GetDelegateForFunctionPointer(ptrUnload, GetType(GCAPI_UNLOAD)), GCAPI_UNLOAD)
                _gcapi_GetTimeVal = CType(Marshal.GetDelegateForFunctionPointer(ptrGetTimeVal, GetType(GCAPI_GETTIMEVAL)), GCAPI_GETTIMEVAL)
                _gcapi_GetFwVer = CType(Marshal.GetDelegateForFunctionPointer(ptrGetFwVer, GetType(GCAPI_GETFWVER)), GCAPI_GETFWVER)
                _gcapi_Write = CType(Marshal.GetDelegateForFunctionPointer(ptrWrite, GetType(GCAPI_WRITE)), GCAPI_WRITE)
                _gcapi_CalcPressTime = CType(Marshal.GetDelegateForFunctionPointer(ptrCalcPressTime, GetType(GCAPI_CALCPRESSTIME)), GCAPI_CALCPRESSTIME)
                _gcapi_Write = CType(Marshal.GetDelegateForFunctionPointer(ptrWrite, GetType(GCAPI_WRITE)), GCAPI_WRITE)
                _gcapi_WriteEx = CType(Marshal.GetDelegateForFunctionPointer(ptrWriteEx, GetType(GCAPI_WRITE_EX)), GCAPI_WRITE_EX)
                _gcapi_Read_TO = CType(Marshal.GetDelegateForFunctionPointer(ptrReadEx, GetType(GCAPI_READ_TO)), GCAPI_READ_TO)
            Catch ex As Exception
                Return
            End Try

            If _gcapi_LoadDevice(CUShort(_devId)) <> 1 Then Return
        End Sub


        Private Function loadExternalFunction(ByVal ptrDll As IntPtr, ByVal strFunction As String) As IntPtr
            Dim ptrFunction As IntPtr = IntPtr.Zero
            ptrFunction = GetProcAddress(ptrDll, strFunction)

            If ptrFunction = IntPtr.Zero Then
            Else
            End If

            Return ptrFunction
        End Function

        Public Sub closeTitanOneInterface()
            'RaiseEvent _gcapi_Unload()
            _gcapi_Unload()


            _gcapi_LoadDevice = Nothing
            _gcapi_Load = Nothing
            _gcapi_IsConnected = Nothing
            _gcapi_GetTimeVal = Nothing
            _gcapi_GetFwVer = Nothing
            _gcapi_Write = Nothing
            _gcapi_WriteEx = Nothing
            _gcapi_WriteRef = Nothing
            _gcapi_Read_TO = Nothing
            _gcapi_CalcPressTime = Nothing
            _gcapi_Unload = Nothing
        End Sub

        'Public Sub checkControllerInput()
        '    If Not _boolGCAPILoaded Then Return

        '    If _gcapi_IsConnected() = 1 Then
        '        Dim output As Byte() = New Byte(35) {}

        '        If _controls.DPad.Left Then
        '            output(CInt(xbox.left)) = Convert.ToByte(100)
        '        End If

        '        If _controls.DPad.Right Then
        '            output(CInt(xbox.right)) = Convert.ToByte(100)
        '        End If

        '        If _controls.DPad.Up Then
        '            output(CInt(xbox.up)) = Convert.ToByte(100)
        '        End If

        '        If _controls.DPad.Down Then
        '            output(CInt(xbox.down)) = Convert.ToByte(100)
        '        End If

        '        If _controls.Buttons.A Then
        '            output(CInt(xbox.a)) = Convert.ToByte(100)
        '        End If

        '        If _controls.Buttons.B Then
        '            output(CInt(xbox.b)) = Convert.ToByte(100)
        '        End If

        '        If _controls.Buttons.X Then
        '            output(CInt(xbox.x)) = Convert.ToByte(100)
        '        End If

        '        If _controls.Buttons.Y Then
        '            output(CInt(xbox.y)) = Convert.ToByte(100)
        '        End If

        '        If _controls.Buttons.Start Then
        '            output(CInt(xbox.start)) = Convert.ToByte(100)
        '        End If

        '        If _controls.Buttons.Guide Then
        '            output(CInt(xbox.home)) = Convert.ToByte(100)
        '        End If

        '        If _controls.Buttons.Back Then
        '            output(CInt(xbox.back)) = Convert.ToByte(100)
        '        End If

        '        If _controls.Buttons.LeftShoulder Then
        '            output(CInt(xbox.leftShoulder)) = Convert.ToByte(100)
        '        End If

        '        If _controls.Buttons.RightShoulder Then
        '            output(CInt(xbox.rightShoulder)) = Convert.ToByte(100)
        '        End If

        '        If _controls.Buttons.LeftStick Then
        '            output(CInt(xbox.leftStick)) = Convert.ToByte(100)
        '        End If

        '        If _controls.Buttons.RightStick Then
        '            output(CInt(xbox.rightStick)) = Convert.ToByte(100)
        '        End If

        '        If _controls.Triggers.Left > 0 Then
        '            output(CInt(xbox.leftTrigger)) = Convert.ToByte(_controls.Triggers.Left * 100)
        '        End If

        '        If _controls.Triggers.Right > 0 Then
        '            output(CInt(xbox.rightTrigger)) = Convert.ToByte(_controls.Triggers.Right * 100)
        '        End If

        '        Dim dblLX As Double = _controls.ThumbSticks.Left.X * 100
        '        Dim dblLY As Double = _controls.ThumbSticks.Left.Y * -100
        '        Dim dblRX As Double = _controls.ThumbSticks.Right.X * 100
        '        Dim dblRY As Double = _controls.ThumbSticks.Right.Y * -100

        '        If dblLX <> 0 Then
        '            output(CInt(xbox.leftX)) = CByte(Convert.ToSByte(CInt((dblLX))))
        '        End If

        '        If dblLY <> 0 Then
        '            output(CInt(xbox.leftY)) = CByte(Convert.ToSByte(CInt((dblLY))))
        '        End If

        '        If dblRX <> 0 Then
        '            output(CInt(xbox.rightX)) = CByte(Convert.ToSByte(CInt((dblRX))))
        '        End If

        '        If dblRY <> 0 Then
        '            output(CInt(xbox.rightY)) = CByte(Convert.ToSByte(CInt((dblRY))))
        '        End If

        '        If Ps4Touchpad = True Then output(CInt(xbox.touch)) = Convert.ToByte(100)
        '        _gcapi_Write(output)

        '        If System.UseRumble = True Then
        '            Dim report As GCAPI_REPORT_TITANONE = New GCAPI_REPORT_TITANONE()

        '            If _gcapi_Read_TO(report) <> IntPtr.Zero Then
        '                GamePad.SetState(PlayerIndex.One, report.rumble(0), report.rumble(1))
        '            End If
        '        End If
        '    End If
        'End Sub

    End Class
End Namespace