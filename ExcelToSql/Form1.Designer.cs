﻿﻿﻿namespace ExcelToSql
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.panel1 = new AntdUI.Panel();
            this.btnBrowse = new AntdUI.Button();
            this.txtFilePath = new AntdUI.Input();
            this.label1 = new AntdUI.Label();
            this.panel2 = new AntdUI.Panel();
            this.numHeaderRow = new AntdUI.InputNumber();
            this.label3 = new AntdUI.Label();
            this.cmbEncoding = new AntdUI.Select();
            this.label7 = new AntdUI.Label();
            this.cmbSheets = new AntdUI.Select();
            this.label2 = new AntdUI.Label();
            this.dgvPreview = new AntdUI.Table();
            this.panel3 = new AntdUI.Panel();
            this.cmbPinyinMode = new AntdUI.Select();
            this.label6 = new AntdUI.Label();
            this.txtTableName = new AntdUI.Input();
            this.label5 = new AntdUI.Label();
            this.cmbDatabase = new AntdUI.Select();
            this.label4 = new AntdUI.Label();
            this.btnSetColType = new AntdUI.Button();
            this.btnGenerate = new AntdUI.Button();
            this.txtSqlOutput = new AntdUI.Input();
            this.btnCopy = new AntdUI.Button();
            this.btnSave = new AntdUI.Button();
            this.pageHeader1 = new AntdUI.PageHeader();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.btnBrowse);
            this.panel1.Controls.Add(this.txtFilePath);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.panel1.Location = new System.Drawing.Point(9, 42);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(971, 60);
            this.panel1.TabIndex = 0;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnBrowse.Location = new System.Drawing.Point(843, 12);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(125, 45);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.Text = "浏览...";
            this.btnBrowse.Type = AntdUI.TTypeMini.Primary;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtFilePath
            // 
            this.txtFilePath.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtFilePath.Location = new System.Drawing.Point(90, 12);
            this.txtFilePath.Name = "txtFilePath";
            this.txtFilePath.ReadOnly = true;
            this.txtFilePath.Size = new System.Drawing.Size(727, 45);
            this.txtFilePath.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.SystemColors.Window;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(17, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 45);
            this.label1.TabIndex = 0;
            this.label1.Text = "Excel文件：";
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.Controls.Add(this.numHeaderRow);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.cmbEncoding);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.cmbSheets);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.dgvPreview);
            this.panel2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.panel2.Location = new System.Drawing.Point(9, 108);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(971, 280);
            this.panel2.TabIndex = 1;
            // 
            // numHeaderRow
            // 
            this.numHeaderRow.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.numHeaderRow.Location = new System.Drawing.Point(368, 14);
            this.numHeaderRow.Name = "numHeaderRow";
            this.numHeaderRow.Size = new System.Drawing.Size(80, 45);
            this.numHeaderRow.TabIndex = 4;
            this.numHeaderRow.Text = "1";
            this.numHeaderRow.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numHeaderRow.ValueChanged += new AntdUI.DecimalEventHandler(this.numHeaderRow_ValueChanged);
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.SystemColors.Window;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(273, 14);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 45);
            this.label3.TabIndex = 3;
            this.label3.Text = "列头行号(1起)";
            // 
            // cmbEncoding
            // 
            this.cmbEncoding.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbEncoding.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cmbEncoding.Location = new System.Drawing.Point(805, 14);
            this.cmbEncoding.Name = "cmbEncoding";
            this.cmbEncoding.Size = new System.Drawing.Size(150, 45);
            this.cmbEncoding.TabIndex = 2;
            this.cmbEncoding.Visible = false;
            this.cmbEncoding.SelectedIndexChanged += new AntdUI.IntEventHandler(this.cmbSheets_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.BackColor = System.Drawing.SystemColors.Window;
            this.label7.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.Location = new System.Drawing.Point(711, 14);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(88, 45);
            this.label7.TabIndex = 1;
            this.label7.Text = "CSV文件编码：";
            this.label7.Visible = false;
            // 
            // cmbSheets
            // 
            this.cmbSheets.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cmbSheets.Location = new System.Drawing.Point(107, 14);
            this.cmbSheets.Name = "cmbSheets";
            this.cmbSheets.Size = new System.Drawing.Size(150, 45);
            this.cmbSheets.TabIndex = 2;
            this.cmbSheets.SelectedIndexChanged += new AntdUI.IntEventHandler(this.cmbSheets_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.SystemColors.Window;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(18, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 45);
            this.label2.TabIndex = 1;
            this.label2.Text = "选择Sheet：";
            // 
            // dgvPreview
            // 
            this.dgvPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvPreview.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dgvPreview.Gap = 12;
            this.dgvPreview.Location = new System.Drawing.Point(17, 74);
            this.dgvPreview.Name = "dgvPreview";
            this.dgvPreview.Size = new System.Drawing.Size(938, 191);
            this.dgvPreview.TabIndex = 0;
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.Controls.Add(this.cmbPinyinMode);
            this.panel3.Controls.Add(this.label6);
            this.panel3.Controls.Add(this.txtTableName);
            this.panel3.Controls.Add(this.label5);
            this.panel3.Controls.Add(this.cmbDatabase);
            this.panel3.Controls.Add(this.label4);
            this.panel3.Controls.Add(this.btnSetColType);
            this.panel3.Controls.Add(this.btnGenerate);
            this.panel3.Controls.Add(this.txtSqlOutput);
            this.panel3.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.panel3.Location = new System.Drawing.Point(9, 394);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(968, 280);
            this.panel3.TabIndex = 2;
            // 
            // cmbPinyinMode
            // 
            this.cmbPinyinMode.Location = new System.Drawing.Point(552, 13);
            this.cmbPinyinMode.Name = "cmbPinyinMode";
            this.cmbPinyinMode.Size = new System.Drawing.Size(127, 45);
            this.cmbPinyinMode.TabIndex = 7;
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.SystemColors.Window;
            this.label6.Location = new System.Drawing.Point(486, 13);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(100, 45);
            this.label6.TabIndex = 6;
            this.label6.Text = "拼音模式：";
            // 
            // txtTableName
            // 
            this.txtTableName.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtTableName.Location = new System.Drawing.Point(318, 13);
            this.txtTableName.Name = "txtTableName";
            this.txtTableName.Size = new System.Drawing.Size(150, 45);
            this.txtTableName.TabIndex = 5;
            this.txtTableName.Text = "TempTable";
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.SystemColors.Window;
            this.label5.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(259, 13);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 45);
            this.label5.TabIndex = 4;
            this.label5.Text = "表名称：";
            // 
            // cmbDatabase
            // 
            this.cmbDatabase.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cmbDatabase.Location = new System.Drawing.Point(90, 13);
            this.cmbDatabase.Name = "cmbDatabase";
            this.cmbDatabase.Size = new System.Drawing.Size(150, 45);
            this.cmbDatabase.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.SystemColors.Window;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(17, 13);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(98, 45);
            this.label4.TabIndex = 2;
            this.label4.Text = "数据库类型：";
            // 
            // btnSetColType
            // 
            this.btnSetColType.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSetColType.Location = new System.Drawing.Point(712, 13);
            this.btnSetColType.Name = "btnSetColType";
            this.btnSetColType.Size = new System.Drawing.Size(125, 45);
            this.btnSetColType.TabIndex = 1;
            this.btnSetColType.Text = "设置列类型";
            this.btnSetColType.Type = AntdUI.TTypeMini.Primary;
            // 
            // btnGenerate
            // 
            this.btnGenerate.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnGenerate.Location = new System.Drawing.Point(843, 13);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(125, 45);
            this.btnGenerate.TabIndex = 1;
            this.btnGenerate.Text = "生成SQL";
            this.btnGenerate.Type = AntdUI.TTypeMini.Primary;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // txtSqlOutput
            // 
            this.txtSqlOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSqlOutput.AutoScroll = true;
            this.txtSqlOutput.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtSqlOutput.Location = new System.Drawing.Point(17, 55);
            this.txtSqlOutput.Multiline = true;
            this.txtSqlOutput.Name = "txtSqlOutput";
            this.txtSqlOutput.Size = new System.Drawing.Size(938, 210);
            this.txtSqlOutput.TabIndex = 0;
            // 
            // btnCopy
            // 
            this.btnCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCopy.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnCopy.Location = new System.Drawing.Point(721, 680);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(125, 45);
            this.btnCopy.TabIndex = 3;
            this.btnCopy.Text = "复制SQL";
            this.btnCopy.Type = AntdUI.TTypeMini.Primary;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSave.Location = new System.Drawing.Point(852, 680);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(125, 45);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "保存SQL";
            this.btnSave.Type = AntdUI.TTypeMini.Primary;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // pageHeader1
            // 
            this.pageHeader1.Dock = System.Windows.Forms.DockStyle.Top;
            this.pageHeader1.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.pageHeader1.IconSvg = resources.GetString("pageHeader1.IconSvg");
            this.pageHeader1.Location = new System.Drawing.Point(0, 0);
            this.pageHeader1.Name = "pageHeader1";
            this.pageHeader1.ShowButton = true;
            this.pageHeader1.ShowIcon = true;
            this.pageHeader1.Size = new System.Drawing.Size(992, 36);
            this.pageHeader1.TabIndex = 5;
            this.pageHeader1.Text = "Excel转SQL工具";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(992, 737);
            this.Controls.Add(this.pageHeader1);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Excel转SQL工具";
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private AntdUI.Panel panel1;
        private AntdUI.Button btnBrowse;
        private AntdUI.Input txtFilePath;
        private AntdUI.Label label1;
        private AntdUI.Panel panel2;
        private AntdUI.Table dgvPreview;
        private AntdUI.Select cmbSheets;
        private AntdUI.Label label2;
        private AntdUI.InputNumber numHeaderRow;
        private AntdUI.Label label3;
        private AntdUI.Panel panel3;
        private AntdUI.Input txtSqlOutput;
        private AntdUI.Button btnGenerate;
        private AntdUI.Select cmbDatabase;
        private AntdUI.Label label4;
        private AntdUI.Input txtTableName;
        private AntdUI.Label label5;
        private AntdUI.Select cmbPinyinMode;
        private AntdUI.Label label6;
        private AntdUI.Button btnCopy;
        private AntdUI.Button btnSave;
        private AntdUI.PageHeader pageHeader1;
        private AntdUI.Select cmbEncoding;
        private AntdUI.Label label7;
        private AntdUI.Button btnSetColType;
    }
}

