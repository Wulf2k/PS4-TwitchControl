Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports System.Runtime.InteropServices
Imports System.IO

Namespace gcapiTitanOne
    Public Class TitanOne
        Private Declare Function LoadLibraryA Lib "kernel32.dll" (ByVal dllToLoad As String) As IntPtr
        Private Declare Function GetProcAddress Lib "kernel32.dll" (ByVal hModule As IntPtr, ByVal procedureName As String) As IntPtr

        'Public Declare Function gcdapi_Load Lib "gcdapi.dll" () As Byte

        Public Structure GcmapiConstants
            Public Const GcapiInputTotal As Integer = 30
            Public Const GcapiOutputTotal As Integer = 36
        End Structure

        Public Structure GcmapiStatus
            Public Value As Byte
            Public PrevValue As Byte
            Public PressTv As Integer
        End Structure

        Public Structure Report
            Public Console As Byte
            Public Controller As Byte
            <MarshalAs(UnmanagedType.ByValArray, SizeConst:=4)>
            Public Led As Byte()
            <MarshalAs(UnmanagedType.ByValArray, SizeConst:=4)>
            Public Rumble As Byte()
            Public BatteryLevel As Byte
            <MarshalAs(UnmanagedType.ByValArray, SizeConst:=GcmapiConstants.GcapiInputTotal, ArraySubType:=UnmanagedType.Struct)>
            Public Input As GcmapiStatus()
        End Structure

        Public Enum Xbox
            Home = 0
            Back = 1
            Start = 2
            RightShoulder = 3
            RightTrigger = 4
            RightStick = 5
            LeftShoulder = 6
            LeftTrigger = 7
            LeftStick = 8
            RightX = 9
            RightY = 10
            LeftX = 11
            LeftY = 12
            Up = 13
            Down = 14
            Left = 15
            Right = 16
            Y = 17
            B = 18
            A = 19
            X = 20
            AccX = 21      'rotate X. 90 = -25, 180 = 0, 270 = +25, 360 = 0 (ng)
            AccY = 22      'shake vertically. +25 (top) to -25 (bottom) (ng)
            AccZ = 23      'tilt up
            GyroX = 24
            GyroY = 25
            GyroZ = 26
            Touch = 27            'touchpad, 100 = on    (works)
            TouchX = 28           '-100 to 100   (left to right)
            TouchY = 29             '-100 to 100   (top to bottom)
        End Enum

        <UnmanagedFunctionPointer(CallingConvention.StdCall)>
        Public Delegate Function GcmapiLoad() As Byte
        <UnmanagedFunctionPointer(CallingConvention.Cdecl)>
        Public Delegate Sub GcmapiUnload()
        <UnmanagedFunctionPointer(CallingConvention.StdCall)>
        Public Delegate Function GcmapiConnect(ByVal devPid As UShort) As Integer
        <UnmanagedFunctionPointer(CallingConvention.StdCall)>
        Public Delegate Function GcmapiGetserialnumber(ByVal devId As Integer) As IntPtr
        <UnmanagedFunctionPointer(CallingConvention.StdCall)>
        Public Delegate Function GcmapiIsconnected(ByVal m As Integer) As Integer
        <UnmanagedFunctionPointer(CallingConvention.StdCall)>
        Public Delegate Function GcmapiWrite(ByVal device As Integer, ByVal output As Byte()) As Integer
        <UnmanagedFunctionPointer(CallingConvention.StdCall)>
        Public Delegate Function GcmapiRead(ByVal device As Integer,
        <[In], Out> ByRef report As Report) As IntPtr
        Public Shared Load As GcmapiLoad
        Public Shared Unload As GcmapiUnload
        Public Shared Connect As GcmapiConnect
        Public Shared Serial As GcmapiGetserialnumber
        Public Shared Connected As GcmapiIsconnected
        Public Shared Write As GcmapiWrite
        Public Shared Read As GcmapiRead
        Private Shared _notConnected As List(Of Integer)
        Private Shared _connected As List(Of Integer)
        Public Shared DeviceList As List(Of TitanDevices)

        Public Class TitanDevices
            Public Id As Integer
            Public SerialNo As String
        End Class


        Sub Open()
            Dim file = "gcdapi.dll"

            If String.IsNullOrEmpty(file) Then
                Console.WriteLine("Error loading TitanOne gcdapi.dll")
                Return
            End If

            Dim dll = LoadLibraryA(file)

            If dll = IntPtr.Zero Then
                Console.WriteLine("[FAIL] Unable to allocate Device API")
                Return
            End If

            Dim _load = LoadExternalFunction(dll, "gcmapi_Load")

            If _load = IntPtr.Zero Then
                Console.WriteLine("[FAIL] gcMapi_Load")
                Return
            End If

            Dim _unload = LoadExternalFunction(dll, "gcmapi_Unload")

            If _unload = IntPtr.Zero Then
                Console.WriteLine("[FAIL] gcMapi_Unload")
                Return
            End If

            Dim _connect = LoadExternalFunction(dll, "gcmapi_Connect")

            If _connect = IntPtr.Zero Then
                Console.WriteLine("[FAIL] gcmapi_Connect")
                Return
            End If

            Dim _connected = LoadExternalFunction(dll, "gcmapi_IsConnected")

            If _connected = IntPtr.Zero Then
                Console.WriteLine("[FAIL] gcmapi_IsConnected")
                Return
            End If

            Dim _serial = LoadExternalFunction(dll, "gcmapi_GetSerialNumber")

            If _serial = IntPtr.Zero Then
                Console.WriteLine("[FAIL] gcmapi_GetSerialNumber")
                Return
            End If

            Dim _write = LoadExternalFunction(dll, "gcmapi_Write")

            If _write = IntPtr.Zero Then
                Console.WriteLine("[FAIL] gcmapi_Write")
                Return
            End If

            Dim _read = LoadExternalFunction(dll, "gcmapi_Read")

            If _read = IntPtr.Zero Then
                Console.WriteLine("[FAIL] gcmapi_Read")
                Return
            End If

            Try
                Load = CType(Marshal.GetDelegateForFunctionPointer(_load, GetType(GcmapiLoad)), GcmapiLoad)
                Unload = CType(Marshal.GetDelegateForFunctionPointer(_unload, GetType(GcmapiUnload)), GcmapiUnload)
                Connect = CType(Marshal.GetDelegateForFunctionPointer(_connect, GetType(GcmapiConnect)), GcmapiConnect)
                Serial = CType(Marshal.GetDelegateForFunctionPointer(_serial, GetType(GcmapiGetserialnumber)), GcmapiGetserialnumber)
                Write = CType(Marshal.GetDelegateForFunctionPointer(_write, GetType(GcmapiWrite)), GcmapiWrite)
                Connected = CType(Marshal.GetDelegateForFunctionPointer(_connected, GetType(GcmapiIsconnected)), GcmapiIsconnected)
                Read = CType(Marshal.GetDelegateForFunctionPointer(_read, GetType(GcmapiRead)), GcmapiRead)
            Catch ex As Exception
                Console.WriteLine("Fail -> " & ex.Message)
                Console.WriteLine("[ERR] Critical failure loading TitanOne API.")
                Return
            End Try

            Console.WriteLine("TitanOne API initialised ok")
        End Sub

        Private Function LoadExternalFunction(ByVal dll As IntPtr, ByVal [function] As String) As IntPtr
            Dim ptr = GetProcAddress(dll, [function])
            Console.WriteLine(If(ptr = IntPtr.Zero, $"[NG] {[function]} alloc fail", $"[OK] {[function]}"))
            Return ptr
        End Function

        Public Sub FindDevices()
            DeviceList = New List(Of TitanDevices)()
            If Connect Is Nothing Then Return
            Dim deviceCount = Load()
            Console.WriteLine($"Number of devices found: {deviceCount}")
            Connect(&H3)

            Threading.Thread.Sleep(33)

            For count = 0 To deviceCount
                If Connected(count) = 0 Then Continue For
                Dim _serial = ReadSerial(count)
                Console.WriteLine($"Device found: [ID]{count} [SERIAL]{_serial}")
                DeviceList.Add(New TitanDevices() With {
                    .Id = count,
                    .SerialNo = _serial
                })
            Next
        End Sub

        Public Function ReadSerial(ByVal devId As Integer) As String
            Dim _serial = New Byte(19) {}
            Dim ret = Serial(devId)
            Marshal.Copy(ret, _serial, 0, 20)
            Dim serialNo = ""

            For Each item In _serial
                serialNo += $"{item}"
            Next

            Return serialNo
        End Function

        Public Sub Send(pid As Integer, output As Object)
            Write(pid, output)
        End Sub

        'Function Send(ByVal player As Gamepad.GamepadOutput) As GcmapiStatus()
        '    If _notConnected Is Nothing Then _notConnected = New List(Of Integer)()
        '    If _connected Is Nothing Then _connected = New List(Of Integer)()

        '    If Connected(player.Index) <> 1 Then
        '        If _notConnected.IndexOf(player.Index) > -1 Then Return Nothing
        '        If _connected.IndexOf(player.Index) > -1 Then _connected.Remove(player.Index)
        '        _notConnected.Add(player.Index)
        '        Console.WriteLine($"TitanOne device {player.Index} not connected")
        '        Return Nothing
        '    End If

        '    If _connected.IndexOf(player.Index) = -1 Then
        '        _connected.Add(player.Index)
        '        If _notConnected.IndexOf(player.Index) > -1 Then _notConnected.Remove(player.Index)
        '        Console.WriteLine($"TitanOne device {player.Index} connected")
        '    End If

        '    Write(player.Index, player.Output)
        '    Dim report = New Report()
        '    If Read(player.Index, report) = IntPtr.Zero Then Return Nothing
        '    If AppSettings.AllowPassthrough Then Gamepad.ReturnOutput(player.Index - 1) = report.Input
        '    If AppSettings.AllowRumble(player.Index) Then Gamepad.SetState(player.Index, report.Rumble(0), report.Rumble(1))
        '    Return report.Input
        'End Function
    End Class

End Namespace