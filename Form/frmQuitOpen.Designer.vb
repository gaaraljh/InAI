﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmQuitOpen
    Inherits System.Windows.Forms.Form

    'Form 重写 Dispose，以清理组件列表。
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            if disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End if
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows 窗体设计器所必需的
    Private components As System.ComponentModel.IContainer

    '注意: 以下过程是 Windows 窗体设计器所必需的
    '可以使用 Windows 窗体设计器修改它。
    '不要使用代码编辑器修改它。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.btn关闭 = New System.Windows.Forms.Button()
        Me.lvw文件列表 = New System.Windows.Forms.ListView()
        Me.ColumnHeader文件名 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.btn多选打开 = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'btn关闭
        '
        Me.btn关闭.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btn关闭.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btn关闭.Location = New System.Drawing.Point(679, 224)
        Me.btn关闭.Name = "btn关闭"
        Me.btn关闭.Size = New System.Drawing.Size(75, 28)
        Me.btn关闭.TabIndex = 0
        Me.btn关闭.Text = "关闭"
        '
        'lvw文件列表
        '
        Me.lvw文件列表.AllowColumnReorder = True
        Me.lvw文件列表.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lvw文件列表.AutoArrange = False
        Me.lvw文件列表.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader文件名})
        Me.lvw文件列表.FullRowSelect = True
        Me.lvw文件列表.Location = New System.Drawing.Point(12, 12)
        Me.lvw文件列表.Name = "lvw文件列表"
        Me.lvw文件列表.Size = New System.Drawing.Size(742, 194)
        Me.lvw文件列表.Sorting = System.Windows.Forms.SortOrder.Ascending
        Me.lvw文件列表.TabIndex = 37
        Me.lvw文件列表.UseCompatibleStateImageBehavior = False
        Me.lvw文件列表.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader文件名
        '
        Me.ColumnHeader文件名.Text = "文件名(双击打开)"
        Me.ColumnHeader文件名.Width = 700
        '
        'btn多选打开
        '
        Me.btn多选打开.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btn多选打开.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btn多选打开.Location = New System.Drawing.Point(598, 224)
        Me.btn多选打开.Name = "btn多选打开"
        Me.btn多选打开.Size = New System.Drawing.Size(75, 28)
        Me.btn多选打开.TabIndex = 38
        Me.btn多选打开.Text = "多选打开"
        Me.btn多选打开.Visible = False
        '
        'frmQuitOpen
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.btn关闭
        Me.ClientSize = New System.Drawing.Size(766, 264)
        Me.Controls.Add(Me.btn多选打开)
        Me.Controls.Add(Me.lvw文件列表)
        Me.Controls.Add(Me.btn关闭)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmQuitOpen"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "快速打开"
        Me.TopMost = True
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents btn关闭 As System.Windows.Forms.Button
    Friend WithEvents lvw文件列表 As System.Windows.Forms.ListView
    Friend WithEvents ColumnHeader文件名 As System.Windows.Forms.ColumnHeader
    Friend WithEvents btn多选打开 As System.Windows.Forms.Button

End Class
