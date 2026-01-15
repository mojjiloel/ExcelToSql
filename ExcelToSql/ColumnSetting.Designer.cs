namespace ExcelToSql
{
    partial class ColumnSetting
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

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.tableMain = new AntdUI.Table();
            this.panel1 = new AntdUI.Panel();
            this.panel2 = new AntdUI.Panel();
            this.btnSave = new AntdUI.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableMain
            // 
            this.tableMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableMain.EmptyHeader = true;
            this.tableMain.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tableMain.Gap = 12;
            this.tableMain.Location = new System.Drawing.Point(0, 0);
            this.tableMain.Name = "tableMain";
            this.tableMain.Size = new System.Drawing.Size(297, 206);
            this.tableMain.TabIndex = 0;
            this.tableMain.Text = "table1";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tableMain);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(297, 206);
            this.panel1.TabIndex = 1;
            this.panel1.Text = "panel1";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnSave);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 206);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(297, 49);
            this.panel2.TabIndex = 2;
            this.panel2.Text = "panel2";
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSave.Location = new System.Drawing.Point(204, 3);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(90, 40);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "保存";
            this.btnSave.Type = AntdUI.TTypeMini.Success;
            // 
            // ColumnSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Name = "ColumnSetting";
            this.Size = new System.Drawing.Size(297, 255);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private AntdUI.Table tableMain;
        private AntdUI.Panel panel1;
        private AntdUI.Panel panel2;
        private AntdUI.Button btnSave;
    }
}
