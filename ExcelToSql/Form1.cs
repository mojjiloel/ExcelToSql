﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace ExcelToSql
{
    public partial class Form1 : Form
    {
        private ExcelReader excelReader;
        private DataTable currentData;

        public Form1()
        {
            InitializeComponent();
            InitializeForm();
        }

        /// <summary>
        /// 初始化窗体
        /// </summary>
        private void InitializeForm()
        {
            // 初始化数据库类型下拉框
            cmbDatabase.Items.Add("SQL Server");
            cmbDatabase.Items.Add("MySQL");
            cmbDatabase.Items.Add("Oracle");
            cmbDatabase.SelectedIndex = 0;

            // 初始化拼音模式下拉框
            cmbPinyinMode.Items.Add("全拼（如：PingZhengRiQi）");
            cmbPinyinMode.Items.Add("首字母（如：PZRQ）");
            cmbPinyinMode.SelectedIndex = 0; // 默认全拼

            // 设置默认列头行
            numHeaderRow.Value = 1;
        }

        /// <summary>
        /// 浏览文件
        /// </summary>
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Excel文件|*.xls;*.xlsx";
                ofd.Title = "选择Excel文件";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        txtFilePath.Text = ofd.FileName;
                        LoadExcelFile(ofd.FileName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("加载文件失败：" + ex.Message, "错误", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        /// 加载Excel文件
        /// </summary>
        private void LoadExcelFile(string filePath)
        {
            // 关闭之前的文件
            if (excelReader != null)
            {
                excelReader.Close();
            }

            // 加载新文件
            excelReader = new ExcelReader(filePath);
            
            // 获取Sheet列表
            List<string> sheets = excelReader.GetSheetNames();
            cmbSheets.Items.Clear();
            foreach (string sheet in sheets)
            {
                cmbSheets.Items.Add(sheet);
            }

            if (cmbSheets.Items.Count > 0)
            {
                cmbSheets.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Sheet选择改变
        /// </summary>
        private void cmbSheets_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSheets.SelectedIndex >= 0)
            {
                LoadPreview();
            }
        }

        /// <summary>
        /// 列头行改变
        /// </summary>
        private void numHeaderRow_ValueChanged(object sender, EventArgs e)
        {
            if (cmbSheets.SelectedIndex >= 0)
            {
                LoadPreview();
            }
        }

        /// <summary>
        /// 加载预览数据
        /// </summary>
        private void LoadPreview()
        {
            if (excelReader == null || cmbSheets.SelectedIndex < 0)
                return;

            try
            {
                DataTable dt = excelReader.PreviewSheet(cmbSheets.SelectedIndex, 50);
                dgvPreview.DataSource = dt;

                // 高亮列头行
                if (dt.Rows.Count > 0 && numHeaderRow.Value > 0 && numHeaderRow.Value <= dt.Rows.Count)
                {
                    int headerRowIndex = (int)numHeaderRow.Value - 1;
                    dgvPreview.Rows[headerRowIndex].DefaultCellStyle.BackColor = Color.LightYellow;
                    dgvPreview.Rows[headerRowIndex].DefaultCellStyle.Font = 
                        new Font(dgvPreview.Font, FontStyle.Bold);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("预览失败：" + ex.Message, "错误", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 生成SQL
        /// </summary>
        private void btnGenerate_Click(object sender, EventArgs e)
        {
            if (excelReader == null || cmbSheets.SelectedIndex < 0)
            {
                MessageBox.Show("请先选择Excel文件和Sheet", "提示", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtTableName.Text))
            {
                MessageBox.Show("请输入表名称", "提示", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                // 设置拼音模式
                PinyinHelper.CurrentMode = cmbPinyinMode.SelectedIndex == 0 
                    ? PinyinMode.FullPinyin 
                    : PinyinMode.FirstLetter;

                // 读取数据
                int headerRowIndex = (int)numHeaderRow.Value - 1;
                currentData = excelReader.ReadSheet(cmbSheets.SelectedIndex, headerRowIndex, true);

                if (currentData.Rows.Count == 0)
                {
                    MessageBox.Show("没有数据可以生成", "提示", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // 获取数据库类型
                DatabaseType dbType = DatabaseType.SqlServer;
                switch (cmbDatabase.SelectedIndex)
                {
                    case 0:
                        dbType = DatabaseType.SqlServer;
                        break;
                    case 1:
                        dbType = DatabaseType.MySql;
                        break;
                    case 2:
                        dbType = DatabaseType.Oracle;
                        break;
                }

                // 生成SQL
                SqlGenerator generator = SqlGenerator.Create(dbType, txtTableName.Text, currentData);
                string sql = generator.GenerateSql();
                txtSqlOutput.Text = sql;

                string modeText = cmbPinyinMode.SelectedIndex == 0 ? "全拼" : "首字母";
                MessageBox.Show(
                    string.Format("SQL生成成功！\r\n\r\n表名：{0}\r\n列数：{1}\r\n行数：{2}\r\n拼音模式：{3}", 
                        txtTableName.Text, currentData.Columns.Count, currentData.Rows.Count, modeText),
                    "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("生成SQL失败：" + ex.Message, "错误", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 复制SQL到剪贴板
        /// </summary>
        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSqlOutput.Text))
            {
                MessageBox.Show("没有可复制的SQL", "提示", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                Clipboard.SetText(txtSqlOutput.Text);
                MessageBox.Show("已复制到剪贴板", "成功", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("复制失败：" + ex.Message, "错误", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 保存SQL到文件
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSqlOutput.Text))
            {
                MessageBox.Show("没有可保存的SQL", "提示", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "SQL文件|*.sql|所有文件|*.*";
                sfd.Title = "保存SQL文件";
                sfd.FileName = txtTableName.Text + ".sql";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        File.WriteAllText(sfd.FileName, txtSqlOutput.Text, Encoding.UTF8);
                        MessageBox.Show("保存成功", "成功", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("保存失败：" + ex.Message, "错误", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        /// 窗体关闭时清理资源
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (excelReader != null)
            {
                excelReader.Close();
            }
            base.OnFormClosing(e);
        }
    }
}
