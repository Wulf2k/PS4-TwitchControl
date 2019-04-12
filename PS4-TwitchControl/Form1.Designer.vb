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
        Me.chkAttached = New System.Windows.Forms.CheckBox()
        Me.txtChat = New System.Windows.Forms.TextBox()
        Me.txtTwitchChat = New System.Windows.Forms.TextBox()
        Me.btnJoinTwitchChat = New System.Windows.Forms.Button()
        Me.SuspendLayout
        '
        'chkAttached
        '
        Me.chkAttached.AutoSize = true
        Me.chkAttached.Location = New System.Drawing.Point(12, 67)
        Me.chkAttached.Name = "chkAttached"
        Me.chkAttached.Size = New System.Drawing.Size(122, 17)
        Me.chkAttached.TabIndex = 66
        Me.chkAttached.Text = "Attached to Process"
        Me.chkAttached.UseVisualStyleBackColor = true
        '
        'txtChat
        '
        Me.txtChat.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
            Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.txtChat.Location = New System.Drawing.Point(12, 90)
        Me.txtChat.Multiline = true
        Me.txtChat.Name = "txtChat"
        Me.txtChat.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtChat.Size = New System.Drawing.Size(422, 316)
        Me.txtChat.TabIndex = 61
        '
        'txtTwitchChat
        '
        Me.txtTwitchChat.Location = New System.Drawing.Point(12, 11)
        Me.txtTwitchChat.Name = "txtTwitchChat"
        Me.txtTwitchChat.Size = New System.Drawing.Size(122, 20)
        Me.txtTwitchChat.TabIndex = 74
        Me.txtTwitchChat.Text = "#wulf2k"
        '
        'btnJoinTwitchChat
        '
        Me.btnJoinTwitchChat.Location = New System.Drawing.Point(12, 38)
        Me.btnJoinTwitchChat.Name = "btnJoinTwitchChat"
        Me.btnJoinTwitchChat.Size = New System.Drawing.Size(122, 23)
        Me.btnJoinTwitchChat.TabIndex = 75
        Me.btnJoinTwitchChat.Text = "Connect Chat"
        Me.btnJoinTwitchChat.UseVisualStyleBackColor = true
        '
        'frmPS4Twitch
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(446, 418)
        Me.Controls.Add(Me.btnJoinTwitchChat)
        Me.Controls.Add(Me.txtTwitchChat)
        Me.Controls.Add(Me.chkAttached)
        Me.Controls.Add(Me.txtChat)
        Me.Name = "frmPS4Twitch"
        Me.Text = "PS4 Remote Play Twitch Control"
        Me.ResumeLayout(false)
        Me.PerformLayout

End Sub
    Friend WithEvents chkAttached As CheckBox
    Friend WithEvents txtChat As TextBox
    Friend WithEvents txtTwitchChat As TextBox
    Friend WithEvents btnJoinTwitchChat As Button
End Class
