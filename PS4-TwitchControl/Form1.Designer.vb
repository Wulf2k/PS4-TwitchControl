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
        Me.SuspendLayout
        '
        'chkHoldSq
        '
        Me.chkHoldSq.AutoSize = true
        Me.chkHoldSq.Location = New System.Drawing.Point(16, 77)
        Me.chkHoldSq.Name = "chkHoldSq"
        Me.chkHoldSq.Size = New System.Drawing.Size(64, 17)
        Me.chkHoldSq.TabIndex = 67
        Me.chkHoldSq.Text = "Hold Sq"
        Me.chkHoldSq.UseVisualStyleBackColor = true
        '
        'chkAttached
        '
        Me.chkAttached.AutoSize = true
        Me.chkAttached.Location = New System.Drawing.Point(235, 100)
        Me.chkAttached.Name = "chkAttached"
        Me.chkAttached.Size = New System.Drawing.Size(122, 17)
        Me.chkAttached.TabIndex = 66
        Me.chkAttached.Text = "Attached to Process"
        Me.chkAttached.UseVisualStyleBackColor = true
        '
        'chkHoldO
        '
        Me.chkHoldO.AutoSize = true
        Me.chkHoldO.Location = New System.Drawing.Point(16, 54)
        Me.chkHoldO.Name = "chkHoldO"
        Me.chkHoldO.Size = New System.Drawing.Size(59, 17)
        Me.chkHoldO.TabIndex = 65
        Me.chkHoldO.Text = "Hold O"
        Me.chkHoldO.UseVisualStyleBackColor = true
        '
        'chkHoldTri
        '
        Me.chkHoldTri.AutoSize = true
        Me.chkHoldTri.Location = New System.Drawing.Point(16, 100)
        Me.chkHoldTri.Name = "chkHoldTri"
        Me.chkHoldTri.Size = New System.Drawing.Size(63, 17)
        Me.chkHoldTri.TabIndex = 64
        Me.chkHoldTri.Text = "Hold Tri"
        Me.chkHoldTri.UseVisualStyleBackColor = true
        '
        'chkHoldX
        '
        Me.chkHoldX.AutoSize = true
        Me.chkHoldX.Location = New System.Drawing.Point(16, 31)
        Me.chkHoldX.Name = "chkHoldX"
        Me.chkHoldX.Size = New System.Drawing.Size(58, 17)
        Me.chkHoldX.TabIndex = 63
        Me.chkHoldX.Text = "Hold X"
        Me.chkHoldX.UseVisualStyleBackColor = true
        '
        'wb
        '
        Me.wb.Location = New System.Drawing.Point(235, 132)
        Me.wb.MinimumSize = New System.Drawing.Size(20, 20)
        Me.wb.Name = "wb"
        Me.wb.ScriptErrorsSuppressed = true
        Me.wb.Size = New System.Drawing.Size(397, 448)
        Me.wb.TabIndex = 62
        '
        'txtChat
        '
        Me.txtChat.Location = New System.Drawing.Point(16, 132)
        Me.txtChat.Multiline = true
        Me.txtChat.Name = "txtChat"
        Me.txtChat.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtChat.Size = New System.Drawing.Size(205, 448)
        Me.txtChat.TabIndex = 61
        '
        'frmPS4Twitch
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(642, 589)
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
End Class
