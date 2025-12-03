using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using AntdUI;

namespace ExcelToSql
{
    public partial class Form1 : Window
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
            cmbDatabase.Items.Add(new AntdUI.SelectItem("SQL Server"));
            cmbDatabase.Items.Add(new AntdUI.SelectItem("MySQL"));
            cmbDatabase.Items.Add(new AntdUI.SelectItem("Oracle"));
            cmbDatabase.SelectedIndex = 0;

            // 初始化拼音模式下拉框
            cmbPinyinMode.Items.Add(new AntdUI.SelectItem("全拼（如：PingZhengRiQi）"));
            cmbPinyinMode.Items.Add(new AntdUI.SelectItem("首字母（如：PZRQ）"));
            cmbPinyinMode.SelectedIndex = 1; // 默认全拼

            // 设置默认列头行
            numHeaderRow.Value = 1;

            // 列头行 高亮
            dgvPreview.SetRowStyle += dgvPreview_SetRowStyle; ; 
        }

        private Table.CellStyleInfo dgvPreview_SetRowStyle(object sender, TableSetRowStyleEventArgs e)
        {
            if (e.RowIndex == numHeaderRow.Value)
            {
                return new AntdUI.Table.CellStyleInfo
                {
                    BackColor = Color.LightYellow,                    
                };
            }
            return null;
        }

        /// <summary>
        /// 浏览文件
        /// </summary>
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Excel或CSV文件|*.xls;*.xlsx;*.csv";
                ofd.Title = "选择Excel或CSV文件";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        txtFilePath.Text = ofd.FileName;
                        LoadExcelFile(ofd.FileName);
                    }
                    catch (Exception ex)
                    {
                        AntdUI.Modal.open(new AntdUI.Modal.Config(this,
                          "错误",
                          $"加载文件失败: {ex.Message}")
                        {
                            Icon = TType.Error,
                            //内边距
                            Padding = new Size(24, 20),
                            Font = new Font("微软雅黑", 12),
                        });
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
                cmbSheets.Items.Add(new AntdUI.SelectItem(sheet));
            }

            if (cmbSheets.Items.Count > 0)
            {
                cmbSheets.SelectedIndex = 0;
            }
        }

        private void cmbSheets_SelectedIndexChanged(object sender, IntEventArgs e)
        {
            // AntdUI的Select控件使用SelectedIndex属性
            if (cmbSheets.SelectedIndex >= 0)
            {
                LoadPreview();
            }
        }

        /// <summary>
        /// 列头行改变
        /// </summary>
        private void numHeaderRow_ValueChanged(object sender, DecimalEventArgs e)
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
                // 对于AntdUI的Select控件，我们需要获取实际的Sheet名称
                // 假设我们可以通过某种方式获取到选中的值
                DataTable dt = excelReader.PreviewSheet(cmbSheets.SelectedIndex, 50);
                dgvPreview.DataSource = dt;

            }
            catch (Exception ex)
            {
                AntdUI.Modal.open(new AntdUI.Modal.Config(this,
                 "错误",
                 $"预览失败: {ex.Message}")
                {
                    Icon = TType.Error,
                    //内边距
                    Padding = new Size(24, 20),
                });
            }
        }

        /// <summary>
        /// 生成SQL
        /// </summary>
        private void btnGenerate_Click(object sender, EventArgs e)
        {
            if (excelReader == null || cmbSheets.SelectedIndex < 0)
            {
                Alert("请先选择Excel文件和Sheet", "提示", TType.Info);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtTableName.Text))
            {
                Alert("请输入表名称", "提示", TType.Info);
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
                // 对于AntdUI的Select控件，我们需要获取实际的Sheet索引
                currentData = excelReader.ReadSheet(cmbSheets.SelectedIndex, headerRowIndex, true);

                if (currentData.Rows.Count == 0)
                {
                    Alert("没有数据可以生成", "提示",TType.Info);
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
                Alert(string.Format("SQL生成成功！\r\n\r\n表名：{0}\r\n列数：{1}\r\n行数：{2}\r\n拼音模式：{3}",
                        txtTableName.Text, currentData.Columns.Count, currentData.Rows.Count, modeText),
                    "成功", TType.Success);
            }
            catch (Exception ex)
            {
                Alert("生成SQL失败：" + ex.Message, "错误");
            }
        }

        /// <summary>
        /// 复制SQL到剪贴板
        /// </summary>
        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSqlOutput.Text))
            {
                Alert("没有可复制的SQL", "提示",TType.Success);
                return;
            }

            try
            {
                Clipboard.SetText(txtSqlOutput.Text);
                Alert("已复制到剪贴板", "成功",TType.Success);
            }
            catch (Exception ex)
            {
                Alert("复制失败：" + ex.Message, "错误");
            }
        }

        /// <summary>
        /// 保存SQL到文件
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSqlOutput.Text))
            {
                Alert("没有可保存的SQL", "提示",TType.Info);
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
                        Alert("保存成功", "成功",
                            TType.Success);
                    }
                    catch (Exception ex)
                    {
                        Alert("保存失败：" + ex.Message, "错误");
                    }
                }
            }
        }

        private void Alert(string message, string title = "提示", AntdUI.TType icon = AntdUI.TType.Error)
        {
            AntdUI.Modal.open(new AntdUI.Modal.Config(this,
                          title,
                          message)
            {
                Icon = icon,
                //内边距
                Padding = new Size(24, 20),
                Font = new Font("微软雅黑", 12),
            });
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
            //base.OnFormClosing(e);
        }

    }
}
