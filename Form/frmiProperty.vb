﻿Imports Inventor
Imports System.Windows.Forms

Public Class frmiProperty

    Private Sub btn确定_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn确定.Click
        Dim oInventorDocument As Inventor.Document
        oInventorDocument = ThisApplication.ActiveEditDocument

        SetPropitem(oInventorDocument, Map_DrawingNnumber, txt图号.Text)
        SetPropitem(oInventorDocument, Map_PartName, txt文件名.Text)
        SetPropitem(oInventorDocument, Map_Describe, cbo描述.Text)
        SetPropitem(oInventorDocument, Map_ERPCode, txtERP编码.Text)
        SetPropitem(oInventorDocument, Map_Vendor, cbo供应商.Text)
        SetPropitem(oInventorDocument, Map_Price, Iif(txt价格.Text = "", "0", txt价格.Text))

        if oInventorDocument.DocumentType = Inventor.DocumentTypeEnum.kPartDocumentObject Then
            Dim oPartDocument As Inventor.PartDocument = oInventorDocument

            Dim strMaterialName As String
            strMaterialName = cbo材料.Text.ToString

            Dim oMaterial As Inventor.Material
            oMaterial = oPartDocument.Materials.Item(strMaterialName)

            oPartDocument.ComponentDefinition.Material = oMaterial
            'Else
            ''MsgBox("该功能仅适用于零件！", MsgBoxStyle.OkOnly + MsgBoxStyle.Information, "更改材料")
            'Exit Sub
        End if

        'If cbo描述.Items.Contains(cbo描述.Text) = False Then
        '    cbo描述.Items.Add(cbo描述.Text)
        'End If

        If InStr(oInventorDocument.FullDocumentName, ContentCenterFiles) > 0 Then    '零件库文件自动保存
            oInventorDocument.Save2(True)
        End If

        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub btn取消_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn取消.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Dispose()
    End Sub

    Private Sub frmChangeIpro_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Me.Icon = My.Resources.XHTool48

        '加载自定义描述
        LoadCustomDescription(cbo描述)


        Dim toolTip As New ToolTip()
        toolTip.AutoPopDelay = 0
        toolTip.InitialDelay = 0
        toolTip.ReshowDelay = 500
        toolTip.SetToolTip(btn向上1, "交换 图号-文件名")
        toolTip.SetToolTip(btn向上2, "交换 文件名-描述")
        toolTip.SetToolTip(btn查询, "查询ERP编码")
        toolTip.SetToolTip(btn保存描述, "保存描述到配置文件")

        btn向上1.Image = My.Resources.交换16.ToBitmap
        btn向上2.Image = My.Resources.交换16.ToBitmap
        btn查询.Image = My.Resources.查询16.ToBitmap
        btn保存描述.Image = My.Resources.保存关闭16.ToBitmap


        Dim oInventorDocument As Inventor.Document
        oInventorDocument = ThisApplication.ActiveEditDocument

        Me.Text = "iProperty+  " & GetFileNameInfo(oInventorDocument.FullDocumentName).OnlyName

        txt位置.Text = GetFileNameInfo(oInventorDocument.FullDocumentName).Folder

        Dim oPropertySets As PropertySets
        Dim oPropertySet As PropertySet
        Dim propitem As [Property]

        '=============================================================================
        '采用学徒服务器, 速度更快
        'Dim apprentice As Inventor.ApprenticeServerComponent
        'apprentice = New Inventor.ApprenticeServerComponent

        'Dim apprenticeDoc As Inventor.ApprenticeServerDocument
        'apprenticeDoc = apprentice.Open(oInventorDocument.FullDocumentName)

        'oPropertySets = apprenticeDoc.PropertySets

        '=============================================================================

        oPropertySets = oInventorDocument.PropertySets

        '获取iproperty

        For Each oPropertySet In oPropertySets
            For Each propitem In oPropertySet
                Select Case propitem.DisplayName
                    Case Map_DrawingNnumber
                        txt图号.Text = propitem.Value
                    Case Map_PartName
                        txt文件名.Text = propitem.Value
                    Case Map_Describe
                        cbo描述.Text = propitem.Value
                    Case Map_ERPCode
                        txtERP编码.Text = propitem.Value
                    Case Map_Vendor
                        cbo供应商.Text = propitem.Value
                    Case Map_Price
                        txt价格.Text = propitem.Value
                End Select
            Next
        Next

        if oInventorDocument.DocumentType = Inventor.DocumentTypeEnum.kPartDocumentObject Then
            Dim oInventorPartDocument As Inventor.PartDocument = oInventorDocument
            cbo材料.Items.Clear()

            For Each oMaterial In oInventorPartDocument.Materials
                cbo材料.Items.Add(oMaterial.Name)
            Next
            cbo材料.DropDownStyle = ComboBoxStyle.DropDownList
            cbo材料.Text = oInventorPartDocument.ComponentDefinition.Material.Name.ToString
        Else
            cbo材料.Enabled = False
        End if

    End Sub

    Private Sub btn向上1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn向上1.Click
        Dim strTemp As String
        strTemp = txt图号.Text
        txt图号.Text = txt文件名.Text
        txt文件名.Text = strTemp
    End Sub

    Private Sub btn向上2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn向上2.Click
        Dim strTemp As String
        strTemp = txt文件名.Text
        txt文件名.Text = cbo描述.Text
        cbo描述.Text = strTemp
    End Sub

    Private Sub btn查询_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn查询.Click
        SetStatusBarText()

        if IsInventorOpenDocument() = False Then
            Exit Sub
        End if

        btn查询.Enabled = False

        Dim oInteraction As InteractionEvents = ThisApplication.CommandManager.CreateInteractionEvents
        oInteraction.Start()
        oInteraction.SetCursor(CursorTypeEnum.kCursorTypeWindows, 32514)
        ThisApplication.UserInterfaceManager.DoEvents()


        Dim oInventorDocument As Inventor.Document      '当前文件
        oInventorDocument = ThisApplication.ActiveEditDocument

        Dim strStochNum As String = Nothing
        Dim strPartNum As String = Nothing

        strStochNum = txt图号.Text

        strPartNum = FindSrtingInSheet(BasicExcelFullFileName, strStochNum, SheetName, TableArrays, ColIndexNum, 0)
        if strPartNum <> 0 Then
            'MsgBox("查询到ERP编码：" & strPartNum, MsgBoxStyle.OkOnly, "查询ERP编码")
            'SetPropitem(oInventorDocument, Map_ERPCode, strPartNum)
            Select Case txtERP编码.Text
                Case ""
                    txtERP编码.Text = strPartNum
                Case Else
                    if txtERP编码.Text <> strPartNum Then
                        if MsgBox("查询到不同的ERP编码：" & strPartNum & "，是否更新？", MsgBoxStyle.YesNo + MsgBoxStyle.Question, "") = MsgBoxResult.Yes Then
                            txtERP编码.Text = strPartNum
                        End if
                    End if
            End Select
        Else

            MsgBox("未查询到ERP编码。", MsgBoxStyle.OkOnly, "查询ERP编码")
        End if

        btn查询.Enabled = True

        oInteraction.SetCursor(CursorTypeEnum.kCursorTypeDefault)
        oInteraction.Stop()

    End Sub

    Private Sub txt价格_MouseClick(sender As Object, e As MouseEventArgs) Handles txt价格.MouseClick
        txt价格.SelectAll()
    End Sub

    Private Sub txtERP编码_MouseClick(sender As Object, e As MouseEventArgs) Handles txtERP编码.MouseClick
        txtERP编码.SelectAll()
    End Sub

    Private Sub LoadCustomDescription(oComboBox As ComboBox)
        Dim strDescriptions As String = Nothing
        Dim arrayDescriptions() As String
        Dim strDescription As String

        strDescriptions = GetStrFromINI("自定义描述", "自定义描述", "", IniFile)

        arrayDescriptions = Strings.Split(strDescriptions, "|")

        For Each strDescription In arrayDescriptions
            If oComboBox.Items.Contains(strDescription) = False Then
                oComboBox.Items.Add(strDescription)
            End If
        Next

    End Sub

    Private Sub SaveCustomDescription(oComboBox As ComboBox)
        Dim strDescriptions As String = Nothing
        Dim i As Integer

        For i = 1 To oComboBox.Items.Count - 1
            strDescriptions = strDescriptions & "|" & oComboBox.Items(i)
        Next

        WriteStrINI("自定义描述", "自定义描述", strDescriptions, Inifile)

    End Sub

    Private Sub btn保存描述_Click(sender As Object, e As EventArgs) Handles btn保存描述.Click

        cbo描述.Items.Add(cbo描述.Text)

        SaveCustomDescription(cbo描述)
    End Sub
End Class