﻿Imports System.Windows.Forms
Imports Inventor
Imports Microsoft
Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.ObjectModel
Imports System.IO

Public Class frmAutoPartNumber

    '开始编号
    Private Sub btn开始_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn开始.Click

        Dim strOldFullFileName As String   '旧文件全名
        Dim strNewFullFileName As String   '新文件全名
        Dim oListViewItem As ListViewItem

        Try

            btn开始.Enabled = False
            'ThisApplication.Cursor  = Cursors.WaitCursor

            For i = 0 To lvw文件列表.Items.Count - 1
                oListViewItem = lvw文件列表.Items(i)

                strOldFullFileName = oListViewItem.SubItems(3).Text & "\" & oListViewItem.Text & oListViewItem.SubItems(1).Text

                If oListViewItem.SubItems(2).Text = "" Then
                    GoTo 999
                End If
                strNewFullFileName = oListViewItem.SubItems(3).Text & "\" & oListViewItem.SubItems(2).Text & oListViewItem.SubItems(1).Text

                '打开旧文件,不显示
                Dim oOldInventorDocument As Inventor.Document
                oOldInventorDocument = ThisApplication.Documents.Open(strOldFullFileName, False)

                '另存为新文件
                oOldInventorDocument.SaveAs(strNewFullFileName, False)

                '关闭旧图
                oOldInventorDocument.Close()

                '后台打开文件，修改ipro
                Dim oNewInventorDocument As Inventor.Document
                oNewInventorDocument = ThisApplication.Documents.Open(strNewFullFileName, False)  '打开文件，不显示
                SetDocumentIpropertyFromFileNameSub(oNewInventorDocument, True) '设置Iproperty，打开文件后需关闭

                Dim oComponentOccurrences As Inventor.ComponentOccurrences
                oComponentOccurrences = ThisApplication.ActiveDocument.ComponentDefinition.Occurrences

                '全部替换为新文件
                For Each oComponentOccurrence As ComponentOccurrence In oComponentOccurrences
                    If oComponentOccurrence.ReferencedDocumentDescriptor.FullDocumentName = strOldFullFileName Then
                        oComponentOccurrence.Replace(strNewFullFileName, True)
                        Exit For
                    End If
                Next

                Dim strOldDrawingFullFileName As String
                strOldDrawingFullFileName = GetChangeExtension(strOldFullFileName, IDW)   '旧工程图

                If IsFileExsts(strOldDrawingFullFileName) = True Then

                    Dim strNewDrawingFullFileName As String

                    strNewDrawingFullFileName = GetChangeExtension(strNewFullFileName, IDW)   '新工程图
                    FileSystem.FileCopy(strOldDrawingFullFileName, strNewDrawingFullFileName)             '复制为新工程图

                    ReplaceFileReference(strNewDrawingFullFileName, strOldFullFileName, strNewFullFileName)

                    'With oInventorDrawingDocument
                    '    .ReferencedDocumentDescriptors(1).ReferencedFileDescriptor.ReplaceReference(strNewFullFileName)
                    '    .Save2()
                    '    .Close()
                    'End With

                End If

                ThisApplication.ActiveDocument.Update()

999:
            Next

            Me.TopMost = False
            SetStatusBarText("自动命名图号完成！")
            MsgBox("自动命名图号完成", MsgBoxStyle.Information + MsgBoxStyle.OkOnly, "自动命名图号")
            btn开始.Enabled = True
            'ThisApplication.Cursor  = Cursors.Default

        Catch ex As Exception
            btn开始.Enabled = True
            MsgBox(ex.Message)
        End Try

    End Sub

    '关闭
    Private Sub btn关闭_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn关闭.Click
        lvw文件列表.Items.Clear()
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub btn上移_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn上移.Click
        ListViewUp(lvw文件列表)
    End Sub

    Private Sub btn下移_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn下移.Click
        ListViewDown(lvw文件列表)
    End Sub

    Private Sub lvw文件列表_ColumnClick(ByVal sender As Object, ByVal e As System.Windows.Forms.ColumnClickEventArgs) Handles lvw文件列表.ColumnClick
        If _ListViewSorter = clsListViewSorter.EnumSortOrder.Ascending Then
            Dim Sorter As New clsListViewSorter(e.Column, clsListViewSorter.EnumSortOrder.Descending)
            lvw文件列表.ListViewItemSorter = Sorter
            _ListViewSorter = clsListViewSorter.EnumSortOrder.Descending
        Else
            Dim Sorter As New clsListViewSorter(e.Column, clsListViewSorter.EnumSortOrder.Ascending)
            lvw文件列表.ListViewItemSorter = Sorter
            _ListViewSorter = clsListViewSorter.EnumSortOrder.Ascending
        End If
    End Sub

    'Private Sub ListView1_DragDrop(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles ListView1.DragDrop
    '    '判断是否选择拖放的项，

    '    If ListView1.SelectedItems.Count = 0 Then
    '        Exit Sub
    '    End If

    '    '定义项的坐标点
    '    Dim cp As System.Drawing.Point
    '    cp = ListView1.PointToClient(New System.Drawing.Point(e.X, e.Y))
    '    Dim dragToItem As ListViewItem
    '    dragToItem = ListView1.GetItemAt(cp.X, cp.Y)

    '    If dragToItem Is Nothing Then
    '        Exit Sub
    '    End If

    '    Dim dragIndex As Integer = dragToItem.Index
    '    Dim sel As ListViewItem()
    '    ReDim sel(ListView1.SelectedItems.Count)

    '    For i = 0 To ListView1.SelectedItems.Count
    '        sel(i) = ListView1.SelectedItems(i)
    '    Next

    '    For i = 0 To sel.GetLength(0)
    '        Dim dragItem As ListViewItem
    '        dragItem = sel(i)
    '        Dim itemIndex As Integer = dragIndex

    '        If (itemIndex = dragItem.Index) Then
    '            Exit Sub
    '        End If

    '        If (dragItem.Index < itemIndex) Then
    '            Exit Sub
    '        Else
    '            itemIndex = dragIndex + i
    '            Dim insertItem As ListViewItem = dragItem.Clone()
    '            ListView1.Items.Insert(itemIndex, insertItem)
    '            ListView1.Items.Remove(dragItem)
    '        End If

    '    Next

    'End Sub

    'Private Sub ListView1_DragEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles ListView1.DragEnter
    '    For i = 0 To e.Data.GetFormats().Length - 1
    '        If (e.Data.GetFormats.Equals("System.Windows.Forms.ListView+SelectedListViewItemCollection")) Then
    '            e.Effect = DragDropEffects.Move
    '        End If
    '    Next
    'End Sub

    'Private Sub ListView1_ItemDrag(ByVal sender As Object, ByVal e As System.Windows.Forms.ItemDragEventArgs) Handles ListView1.ItemDrag
    '    ListView1.DoDragDrop(ListView1.SelectedItems, DragDropEffects.Move)
    'End Sub

    '键盘上下键移动
    Private Sub lvw文件列表_KeyDown1(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles lvw文件列表.KeyDown
        Select Case e.KeyCode
            Case Keys.Up
                If e.Control Then
                    ListViewUp(lvw文件列表)
                    e.Handled = True
                End If
            Case Keys.Down
                If e.Control Then
                    ListViewDown(lvw文件列表)
                    e.Handled = True
                End If
            Case Keys.Delete
                ListViewDel(lvw文件列表)
        End Select
    End Sub

    '预览
    Private Sub btn预览_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn预览.Click
        Dim oListViewItem As ListViewItem
        Dim intAssNum As Integer
        Dim intPartNum As Integer
        Dim oStockNumPartName As StockNumPartName = Nothing
        Dim strBasicStockNum As String

        intAssNum = 1
        intPartNum = 1
        strBasicStockNum = txt基准图号.Text

        If (IsNumeric(txt零件变量.Text) = False) Or (IsNumeric(cmb部件变量.Text) = False) Then
            MsgBox("变量非数字！", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, "错误")
            Exit Sub
        End If

        Dim intPartChange As Integer = Val(txt零件变量.Text)
        Dim intAmsChange As Integer = Val(cmb部件变量.Text)

        For i = 0 To lvw文件列表.Items.Count - 1
            oListViewItem = lvw文件列表.Items(i)
            If oListViewItem.SubItems(1).Text = IPT Then
                oStockNumPartName.StockNum = Strings.Left(strBasicStockNum, Strings.Len(strBasicStockNum) - Strings.Len((intPartNum * intPartChange).ToString)) & intPartNum * intPartChange
                oStockNumPartName.PartName = oListViewItem.Text
                oListViewItem.SubItems(2).Text = oStockNumPartName.StockNum & oStockNumPartName.PartName
                intPartNum = intPartNum + 1
            ElseIf oListViewItem.SubItems(1).Text = IAM Then
                oStockNumPartName.StockNum = Strings.Left(strBasicStockNum, Strings.Len(strBasicStockNum) - Strings.Len((intAssNum * intAmsChange).ToString)) & intAssNum * intAmsChange
                oStockNumPartName.PartName = oListViewItem.Text
                oListViewItem.SubItems(2).Text = oStockNumPartName.StockNum & oStockNumPartName.PartName
                intAssNum = intAssNum + 1
            End If
        Next
    End Sub

    Private Sub frmAutoPartNumber_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Dim oInventorAssemblyDocument As Inventor.AssemblyDocument
        oInventorAssemblyDocument = ThisApplication.ActiveDocument
        LoadAssBOM(oInventorAssemblyDocument, lvw文件列表)
    End Sub

    '载入数据函数
    Private Sub LoadAssBOM(ByVal oInventorAssemblyDocument As Inventor.AssemblyDocument, ByVal oListView As ListView)
        On Error Resume Next

        '基于bom结构化数据，可跳过参考的文件
        ' Set a reference to the BOM

        Dim oBOM As BOM

        Dim strInventorAssemblyFullFileName As String

        strInventorAssemblyFullFileName = oInventorAssemblyDocument.FullDocumentName

        Dim oStockNumPartName As StockNumPartName
        oStockNumPartName = GetStockNumPartName(strInventorAssemblyFullFileName)
        txt基准图号.Text = oStockNumPartName.StockNum

        oBOM = oInventorAssemblyDocument.ComponentDefinition.BOM
        oBOM.StructuredViewEnabled = True

        'Set a reference to the "Structured" BOMView

        oListView.Items.Clear()

        oListView.BeginUpdate()

        '获取结构化的bom页面
        For Each oBOMView As BOMView In oBOM.BOMViews
            If oBOMView.ViewType = BOMViewTypeEnum.kStructuredBOMViewType Then

                For i = 1 To oBOMView.BOMRows.Count
                    ' Get the current row.
                    Dim oBOMRow As BOMRow
                    oBOMRow = oBOMView.BOMRows.Item(i)

                    Dim strFullFileName As String

                    strFullFileName = oBOMRow.ReferencedFileDescriptor.FullFileName

                    '测试文件
                    If InStr(strFullFileName, ContentCenterFiles) > 0 Then         '跳过零件库文件

                    Else
                        Debug.Print(strFullFileName)
                        Dim oFileNameInfo As FileNameInfo
                        oFileNameInfo = GetFileNameInfo(strFullFileName)

                        Dim oListViewItem As ListViewItem
                        oListViewItem = oListView.Items.Add(oFileNameInfo.OnlyName)
                        With oListViewItem
                            .SubItems.Add(oFileNameInfo.ExtensionName)
                            .SubItems.Add("")
                            .SubItems.Add(oFileNameInfo.Folder)
                        End With
                    End If

                Next
            End If
        Next

        oListView.EndUpdate()

    End Sub

    '移出项
    Private Sub ListViewDel(ByVal oListView As ListView)
        For i As Integer = oListView.SelectedIndices.Count - 1 To 0 Step -1
            oListView.Items.RemoveAt(oListView.SelectedIndices(i))
        Next
    End Sub

    Private Sub btn移出_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn移出.Click
        ListViewDel(lvw文件列表)
    End Sub

    '重载数据
    Private Sub btn重载_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn重载.Click
        Dim oAssemblyDocument As AssemblyDocument
        oAssemblyDocument = ThisApplication.ActiveDocument
        LoadAssBOM(oAssemblyDocument, lvw文件列表)
    End Sub

    Private Sub btn确定新文件名_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn确定新文件名.Click
        If lvw文件列表.SelectedIndices.Count > 0 Then
            Dim index As Integer = lvw文件列表.SelectedIndices(0)
            lvw文件列表.Items(index).SubItems(2).Text = txt新文件名.Text
        End If

    End Sub

    Private Sub lvw文件列表_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles lvw文件列表.SelectedIndexChanged
        Try
            If lvw文件列表.SelectedIndices.Count > 0 Then
                Dim index As Integer = lvw文件列表.SelectedIndices(0)  '选中行的下一行索引
                If index < lvw文件列表.Items.Count Then
                    txt新文件名.Text = lvw文件列表.Items(index).SubItems(2).Text
                    Dim item As ListViewItem = lvw文件列表.Items(index)
                    Dim strOldFullFileName As String   '旧文件全名
                    strOldFullFileName = item.SubItems(3).Text & "\" & item.Text & item.SubItems(1).Text

                    Dim oInventorAssemblyDocument As Inventor.AssemblyDocument
                    oInventorAssemblyDocument = ThisApplication.ActiveDocument

                    Dim strInventorAssemblyFullFileName As String
                    strInventorAssemblyFullFileName = oInventorAssemblyDocument.FullFileName

                    ' 获取装配定义
                    Dim oAssemblyComponentDefinition As AssemblyComponentDefinition
                    oAssemblyComponentDefinition = oInventorAssemblyDocument.ComponentDefinition

                    ' 获取装配子集
                    Dim oComponentOccurrences As ComponentOccurrences
                    oComponentOccurrences = oAssemblyComponentDefinition.Occurrences

                    '指定要选择的文件()
                    'Dim oDoc As Document
                    'oDoc = ThisApplication.Documents.ItemByName(OldFullFileName)

                    '遍历
                    For Each oComponentOccurrence As ComponentOccurrence In oComponentOccurrences
                        If oComponentOccurrence.ReferencedDocumentDescriptor.FullDocumentName = strOldFullFileName Then
                            ThisApplication.CommandManager.DoSelect(oComponentOccurrence)
                        Else
                            ThisApplication.CommandManager.DoUnSelect(oComponentOccurrence)
                        End If

                    Next

                End If
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

    End Sub

    Private Sub lvw文件列表_DragDrop(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles lvw文件列表.DragDrop
        Dim oDraggedItem As ListViewItem
        oDraggedItem = e.Data.GetData(System.Windows.Forms.DataFormats.Serializable)

        Dim ptScreen As Drawing.Point = New Drawing.Point(e.X, e.Y)
        Dim pt As Drawing.Point = lvw文件列表.PointToClient(ptScreen)
        Dim TargetItem As ListViewItem
        TargetItem = lvw文件列表.GetItemAt(pt.X, pt.Y) '拖动的项将放置于该项之前
        If (TargetItem Is Nothing) Then
            Exit Sub
        End If
        lvw文件列表.Items.Insert(TargetItem.Index, oDraggedItem.Clone())
        lvw文件列表.Items.Remove(oDraggedItem)
    End Sub

    Private Sub lvwFileList_DragEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles lvw文件列表.DragEnter
        e.Effect = DragDropEffects.Move
    End Sub

    Private Sub lvw文件列表_DragLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles lvw文件列表.DragLeave
        lvw文件列表.InsertionMark.Index = -1
    End Sub

    Private Sub lvw文件列表_DragOver(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles lvw文件列表.DragOver
        Dim ptScreen As Drawing.Point = New Drawing.Point(e.X, e.Y)
        Dim pt As Drawing.Point = lvw文件列表.PointToClient(ptScreen)

        Dim index As Integer = lvw文件列表.InsertionMark.NearestIndex(pt)
        lvw文件列表.InsertionMark.Index = index
    End Sub

    Private Sub lvw文件列表_ItemDrag(ByVal sender As Object, ByVal e As System.Windows.Forms.ItemDragEventArgs) Handles lvw文件列表.ItemDrag
        lvw文件列表.InsertionMark.Color = System.Drawing.Color.ForestGreen
        lvw文件列表.DoDragDrop(e.Item, DragDropEffects.Move)

    End Sub

    Private Sub txt零件变量_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txt零件变量.TextChanged
        If IsNumeric(txt零件变量.Text) = False Then
            MsgBox("非数字！", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, "错误")
        End If
    End Sub

    Private Sub txt新文件名_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt新文件名.KeyPress
        If Asc(e.KeyChar) = Keys.Enter Then
            btn确定新文件名.PerformClick()
        End If
    End Sub

End Class