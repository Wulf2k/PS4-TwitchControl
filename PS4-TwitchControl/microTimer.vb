Imports System

Namespace MicroLibrary
    Public Class MicroStopwatch
        Inherits System.Diagnostics.Stopwatch

        ReadOnly _microSecPerTick As Double = 1000000.0R / System.Diagnostics.Stopwatch.Frequency

        Public Sub New()
            If Not System.Diagnostics.Stopwatch.IsHighResolution Then
                Throw New Exception("On this system the high-resolution " & "performance counter is not available")
            End If
        End Sub

        Public ReadOnly Property ElapsedMicroseconds As Long
            Get
                Return CLng((ElapsedTicks * _microSecPerTick))
            End Get
        End Property
    End Class

    Public Class MicroTimer
        Public Delegate Sub MicroTimerElapsedEventHandler(ByVal sender As Object, ByVal timerEventArgs As MicroTimerEventArgs)
        Public Event MicroTimerElapsed As MicroTimerElapsedEventHandler
        Private _threadTimer As System.Threading.Thread = Nothing
        Private _ignoreEventIfLateBy As Long = Long.MaxValue
        Private _timerIntervalInMicroSec As Long = 0
        Private _stopTimer As Boolean = True

        Public Sub New()
        End Sub

        Public Sub New(ByVal timerIntervalInMicroseconds As Long)
            Interval = timerIntervalInMicroseconds
        End Sub

        Public Property Interval As Long
            Get
                Return System.Threading.Interlocked.Read(_timerIntervalInMicroSec)
            End Get
            Set(ByVal value As Long)
                System.Threading.Interlocked.Exchange(_timerIntervalInMicroSec, value)
            End Set
        End Property

        Public Property IgnoreEventIfLateBy As Long
            Get
                Return System.Threading.Interlocked.Read(_ignoreEventIfLateBy)
            End Get
            Set(ByVal value As Long)
                System.Threading.Interlocked.Exchange(_ignoreEventIfLateBy, If(value <= 0, Long.MaxValue, value))
            End Set
        End Property

        Public Property Enabled As Boolean
            Set(ByVal value As Boolean)

                If value Then
                    Start()
                Else
                    [Stop]()
                End If
            End Set
            Get
                Return (_threadTimer IsNot Nothing AndAlso _threadTimer.IsAlive)
            End Get
        End Property

        Public Sub Start()
            If Enabled OrElse Interval <= 0 Then
                Return
            End If

            _stopTimer = False
            Dim threadStart As System.Threading.ThreadStart = Sub()
                                                                  NotificationTimer(_timerIntervalInMicroSec, _ignoreEventIfLateBy, _stopTimer)
                                                              End Sub

            _threadTimer = New System.Threading.Thread(threadStart)
            _threadTimer.Priority = System.Threading.ThreadPriority.Highest
            _threadTimer.Start()
        End Sub

        Public Sub [Stop]()
            _stopTimer = True
        End Sub

        Public Sub StopAndWait()
            StopAndWait(System.Threading.Timeout.Infinite)
        End Sub

        Public Function StopAndWait(ByVal timeoutInMilliSec As Integer) As Boolean
            _stopTimer = True

            If Not Enabled OrElse _threadTimer.ManagedThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId Then
                Return True
            End If

            Return _threadTimer.Join(timeoutInMilliSec)
        End Function

        Public Sub Abort()
            _stopTimer = True

            If Enabled Then
                _threadTimer.Abort()
            End If
        End Sub

        Private Sub NotificationTimer(ByRef timerIntervalInMicroSec As Long, ByRef ignoreEventIfLateBy As Long, ByRef stopTimer As Boolean)
            Dim timerCount As Integer = 0
            Dim nextNotification As Long = 0
            Dim microStopwatch As MicroStopwatch = New MicroStopwatch()
            microStopwatch.Start()

            While Not stopTimer
                Dim callbackFunctionExecutionTime As Long = microStopwatch.ElapsedMicroseconds - nextNotification
                Dim timerIntervalInMicroSecCurrent As Long = System.Threading.Interlocked.Read(timerIntervalInMicroSec)
                Dim ignoreEventIfLateByCurrent As Long = System.Threading.Interlocked.Read(ignoreEventIfLateBy)
                nextNotification += timerIntervalInMicroSecCurrent
                timerCount += 1
                Dim elapsedMicroseconds As Long = 0

                While (CSharpImpl.__Assign(elapsedMicroseconds, microStopwatch.ElapsedMicroseconds)) < nextNotification
                    System.Threading.Thread.SpinWait(10)
                End While

                Dim timerLateBy As Long = elapsedMicroseconds - nextNotification

                If timerLateBy >= ignoreEventIfLateByCurrent Then
                    Continue While
                End If

                Dim microTimerEventArgs As MicroTimerEventArgs = New MicroTimerEventArgs(timerCount, elapsedMicroseconds, timerLateBy, callbackFunctionExecutionTime)
                RaiseEvent MicroTimerElapsed(Me, microTimerEventArgs)
            End While

            microStopwatch.[Stop]()
        End Sub

        Private Class CSharpImpl
            <Obsolete("Please refactor calling code to use normal Visual Basic assignment")>
            Shared Function __Assign(Of T)(ByRef target As T, value As T) As T
                target = value
                Return value
            End Function
        End Class
    End Class

    Public Class MicroTimerEventArgs
        Inherits EventArgs

        Public Property TimerCount As Integer
        Public Property ElapsedMicroseconds As Long
        Public Property TimerLateBy As Long
        Public Property CallbackFunctionExecutionTime As Long

        Public Sub New(ByVal timerCount As Integer, ByVal elapsedMicroseconds As Long, ByVal timerLateBy As Long, ByVal callbackFunctionExecutionTime As Long)
            timerCount = timerCount
            elapsedMicroseconds = elapsedMicroseconds
            timerLateBy = timerLateBy
            callbackFunctionExecutionTime = callbackFunctionExecutionTime
        End Sub

        Private Class CSharpImpl
            <Obsolete("Please refactor calling code to use normal Visual Basic assignment")>
            Shared Function __Assign(Of T)(ByRef target As T, value As T) As T
                target = value
                Return value
            End Function
        End Class
    End Class
End Namespace
