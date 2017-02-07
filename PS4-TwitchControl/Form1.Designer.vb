<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmPS4Twitch
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.chkHoldSq = New System.Windows.Forms.CheckBox()
        Me.chkAttached = New System.Windows.Forms.CheckBox()
        Me.chkHoldO = New System.Windows.Forms.CheckBox()
        Me.chkHoldTri = New System.Windows.Forms.CheckBox()
        Me.chkHoldX = New System.Windows.Forms.CheckBox()
        Me.wb = New System.Windows.Forms.WebBrowser()
        Me.txtChat = New System.Windows.Forms.TextBox()
        Me.chkHoldR1 = New System.Windows.Forms.CheckBox()
        Me.chkHoldL2 = New System.Windows.Forms.CheckBox()
        Me.chkHoldR2 = New System.Windows.Forms.CheckBox()
        Me.chkHoldL1 = New System.Windows.Forms.CheckBox()
        Me.chkHoldR3 = New System.Windows.Forms.CheckBox()
        Me.chkHoldL3 = New System.Windows.Forms.CheckBox()
        Me.txtTwitchChat = New System.Windows.Forms.TextBox()
        Me.btnJoinTwitchChat = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'chkHoldSq
        '
        Me.chkHoldSq.AutoSize = True
        Me.chkHoldSq.Location = New System.Drawing.Point(16, 77)
        Me.chkHoldSq.Name = "chkHoldSq"
        Me.chkHoldSq.Size = New System.Drawing.Size(64, 17)
        Me.chkHoldSq.TabIndex = 67
        Me.chkHoldSq.Text = "Hold Sq"
        Me.chkHoldSq.UseVisualStyleBackColor = True
        '
        'chkAttached
        '
        Me.chkAttached.AutoSize = True
        Me.chkAttached.Location = New System.Drawing.Point(235, 100)
        Me.chkAttached.Name = "chkAttached"
        Me.chkAttached.Size = New System.Drawing.Size(122, 17)
        Me.chkAttached.TabIndex = 66
        Me.chkAttached.Text = "Attached to Process"
        Me.chkAttached.UseVisualStyleBackColor = True
        '
        'chkHoldO
        '
        Me.chkHoldO.AutoSize = True
        Me.chkHoldO.Location = New System.Drawing.Point(16, 54)
        Me.chkHoldO.Name = "chkHoldO"
        Me.chkHoldO.Size = New System.Drawing.Size(59, 17)
        Me.chkHoldO.TabIndex = 65
        Me.chkHoldO.Text = "Hold O"
        Me.chkHoldO.UseVisualStyleBackColor = True
        '
        'chkHoldTri
        '
        Me.chkHoldTri.AutoSize = True
        Me.chkHoldTri.Location = New System.Drawing.Point(16, 100)
        Me.chkHoldTri.Name = "chkHoldTri"
        Me.chkHoldTri.Size = New System.Drawing.Size(63, 17)
        Me.chkHoldTri.TabIndex = 64
        Me.chkHoldTri.Text = "Hold Tri"
        Me.chkHoldTri.UseVisualStyleBackColor = True
        '
        'chkHoldX
        '
        Me.chkHoldX.AutoSize = True
        Me.chkHoldX.Location = New System.Drawing.Point(16, 31)
        Me.chkHoldX.Name = "chkHoldX"
        Me.chkHoldX.Size = New System.Drawing.Size(58, 17)
        Me.chkHoldX.TabIndex = 63
        Me.chkHoldX.Text = "Hold X"
        Me.chkHoldX.UseVisualStyleBackColor = True
        '
        'wb
        '
        Me.wb.Location = New System.Drawing.Point(235, 132)
        Me.wb.MinimumSize = New System.Drawing.Size(20, 20)
        Me.wb.Name = "wb"
        Me.wb.ScriptErrorsSuppressed = True
        Me.wb.Size = New System.Drawing.Size(397, 448)
        Me.wb.TabIndex = 62
        '
        'txtChat
        '
        Me.txtChat.Location = New System.Drawing.Point(16, 132)
        Me.txtChat.Multiline = True
        Me.txtChat.Name = "txtChat"
        Me.txtChat.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtChat.Size = New System.Drawing.Size(205, 448)
        Me.txtChat.TabIndex = 61
        '
        'chkHoldR1
        '
        Me.chkHoldR1.AutoSize = True
        Me.chkHoldR1.Location = New System.Drawing.Point(80, 77)
        Me.chkHoldR1.Name = "chkHoldR1"
        Me.chkHoldR1.Size = New System.Drawing.Size(65, 17)
        Me.chkHoldR1.TabIndex = 71
        Me.chkHoldR1.Text = "Hold R1"
        Me.chkHoldR1.UseVisualStyleBackColor = True
        '
        'chkHoldL2
        '
        Me.chkHoldL2.AutoSize = True
        Me.chkHoldL2.Location = New System.Drawing.Point(80, 54)
        Me.chkHoldL2.Name = "chkHoldL2"
        Me.chkHoldL2.Size = New System.Drawing.Size(63, 17)
        Me.chkHoldL2.TabIndex = 70
        Me.chkHoldL2.Text = "Hold L2"
        Me.chkHoldL2.UseVisualStyleBackColor = True
        '
        'chkHoldR2
        '
        Me.chkHoldR2.AutoSize = True
        Me.chkHoldR2.Location = New System.Drawing.Point(80, 100)
        Me.chkHoldR2.Name = "chkHoldR2"
        Me.chkHoldR2.Size = New System.Drawing.Size(65, 17)
        Me.chkHoldR2.TabIndex = 69
        Me.chkHoldR2.Text = "Hold R2"
        Me.chkHoldR2.UseVisualStyleBackColor = True
        '
        'chkHoldL1
        '
        Me.chkHoldL1.AutoSize = True
        Me.chkHoldL1.Location = New System.Drawing.Point(80, 31)
        Me.chkHoldL1.Name = "chkHoldL1"
        Me.chkHoldL1.Size = New System.Drawing.Size(63, 17)
        Me.chkHoldL1.TabIndex = 68
        Me.chkHoldL1.Text = "Hold L1"
        Me.chkHoldL1.UseVisualStyleBackColor = True
        '
        'chkHoldR3
        '
        Me.chkHoldR3.AutoSize = True
        Me.chkHoldR3.Location = New System.Drawing.Point(149, 54)
        Me.chkHoldR3.Name = "chkHoldR3"
        Me.chkHoldR3.Size = New System.Drawing.Size(65, 17)
        Me.chkHoldR3.TabIndex = 73
        Me.chkHoldR3.Text = "Hold R3"
        Me.chkHoldR3.UseVisualStyleBackColor = True
        '
        'chkHoldL3
        '
        Me.chkHoldL3.AutoSize = True
        Me.chkHoldL3.Location = New System.Drawing.Point(149, 31)
        Me.chkHoldL3.Name = "chkHoldL3"
        Me.chkHoldL3.Size = New System.Drawing.Size(63, 17)
        Me.chkHoldL3.TabIndex = 72
        Me.chkHoldL3.Text = "Hold L3"
        Me.chkHoldL3.UseVisualStyleBackColor = True
        '
        'txtTwitchChat
        '
        Me.txtTwitchChat.Location = New System.Drawing.Point(235, 31)
        Me.txtTwitchChat.Name = "txtTwitchChat"
        Me.txtTwitchChat.Size = New System.Drawing.Size(397, 20)
        Me.txtTwitchChat.TabIndex = 74
        Me.txtTwitchChat.Text = "http://www.twitch.tv/wulf2k/chat"
        '
        'btnJoinTwitchChat
        '
        Me.btnJoinTwitchChat.Location = New System.Drawing.Point(235, 58)
        Me.btnJoinTwitchChat.Name = "btnJoinTwitchChat"
        Me.btnJoinTwitchChat.Size = New System.Drawing.Size(122, 23)
        Me.btnJoinTwitchChat.TabIndex = 75
        Me.btnJoinTwitchChat.Text = "Connect Chat"
        Me.btnJoinTwitchChat.UseVisualStyleBackColor = True
        '
        'frmPS4Twitch
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(642, 589)
        Me.Controls.Add(Me.btnJoinTwitchChat)
        Me.Controls.Add(Me.txtTwitchChat)
        Me.Controls.Add(Me.chkHoldR3)
        Me.Controls.Add(Me.chkHoldL3)
        Me.Controls.Add(Me.chkHoldR1)
        Me.Controls.Add(Me.chkHoldL2)
        Me.Controls.Add(Me.chkHoldR2)
        Me.Controls.Add(Me.chkHoldL1)
        Me.Controls.Add(Me.chkHoldSq)
        Me.Controls.Add(Me.chkAttached)
        Me.Controls.Add(Me.chkHoldO)
        Me.Controls.Add(Me.chkHoldTri)
        Me.Controls.Add(Me.chkHoldX)
        Me.Controls.Add(Me.wb)
        Me.Controls.Add(Me.txtChat)
        Me.Name = "frmPS4Twitch"
        Me.Text = "PS4 Remote Play Twitch Control"
        Me.ResumeLayout(false)
        Me.PerformLayout

End Sub

    Friend WithEvents chkHoldSq As CheckBox
    Friend WithEvents chkAttached As CheckBox
    Friend WithEvents chkHoldO As CheckBox
    Friend WithEvents chkHoldTri As CheckBox
    Friend WithEvents chkHoldX As CheckBox
    Friend WithEvents wb As WebBrowser
    Friend WithEvents txtChat As TextBox
    Friend WithEvents chkHoldR1 As CheckBox
    Friend WithEvents chkHoldL2 As CheckBox
    Friend WithEvents chkHoldR2 As CheckBox
    Friend WithEvents chkHoldL1 As CheckBox
    Friend WithEvents chkHoldR3 As CheckBox
    Friend WithEvents chkHoldL3 As CheckBox
    Friend WithEvents txtTwitchChat As TextBox
    Friend WithEvents btnJoinTwitchChat As Button
End Class
