using AntdUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ExcelToSql
{
    public partial class ColumnSetting : UserControl
    {
        Form form;

        private BindingList<ColunmnInfo> processingItems = new BindingList<ColunmnInfo>();

        public delegate void ColumnSettingsHandler(Dictionary<string, ExcelToSql.ColumnSettingInfo> settings);
        public event ColumnSettingsHandler OnColumnSettingsSaved;

        public ColumnSetting(Form _form, Dictionary<string, ColumnSettingInfo> colsDict)
        {
            form = _form;
            InitializeComponent();

            Init();
            
            LoadData(colsDict);
        }

        private void LoadData(Dictionary<string, ColumnSettingInfo> colsDict)
        {
            foreach (var item in colsDict)
            {
                if (!processingItems.Any(i => i.ColName == item.Key))
                {
                    var defaulttype = DataTypeMapper.ValidTypes.FirstOrDefault();
                    var colInfo = new ColunmnInfo
                    {
                        ColName = item.Value.DisplayName,
                        ColTypeButton =
                        new AntdUI.CellButton(Guid.NewGuid().ToString(), defaulttype) {
                            Ghost = true,
                            BorderWidth = 1,
                            ShowArrow = true,
                            IsLink = true,
                        },
                        Tag = item.Value,
                    };
                    colInfo.ColTypeButton.DropDownItems = DataTypeMapper.ValidTypes.Select(kv => new AntdUI.SelectItem(kv, kv)).ToArray();

                    colInfo.ColTypeButton.DropDownValueChanged = (s) =>
                    {
                        if (s != null)
                        {
                            colInfo.Tag.ColumnType = s.ToString();
                            colInfo.ColTypeButton.Text = s.ToString();
                        }
                    };
                    processingItems.Add(colInfo);
                }
            }

            tableMain.Binding(processingItems);
        }
        private void Init()
        {
            tableMain.Columns = new ColumnCollection() {
                new Column("ColName", "字段名称"){ Width = "200"},
                new AntdUI.ColumnSwitch("Enable", "启用", AntdUI.ColumnAlign.Center)
                {
                    Call = (value, record, i_row, i_col) => {
                        return value;
                    }
                },
                new Column("ColTypeButton", "字段类型"){ Width = "200"},
                };

            tableMain.SetRowStyle += tableMain_SetRowStyle;
            tableMain.CellPaint += tableMain_CellPaint;
            tableMain.CellButtonClick += tableMain_CellButtonClick;
            tableMain.Invalidate();

            btnSave.Click += BtnSave_Click;
            //btnCancel.Click += BtnCancel_Click;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            // 根据processingItems保存设置 返回给主窗口生成sql使用

            var dictionary = new Dictionary<string, ColumnSettingInfo>();
            foreach (var item in processingItems)
            {
                // 更新Tag中的设置
                item.Tag.IsEnabled = item.Enable;
                dictionary[item.Tag.ColumnName] = item.Tag;
            }
            OnColumnSettingsSaved?.Invoke(dictionary);
            
            // 关闭弹窗
            var popover = this.FindForm();
            if (popover != null)
            {
                popover.Close();
            }
        }

        private void tableMain_CellButtonClick(object sender, TableButtonEventArgs e)
        {
            var buttontext = e.Btn.Text;
            if (e.Record is ColunmnInfo item)
            {
                //switch (buttontext)
                //{
                //    case "查看图片":
                //        Preview.open(new Preview.Config(this, new Bitmap(item.FilePath)));
                //        break;
                //}
            }
        }

        private void tableMain_CellPaint(object sender, TablePaintEventArgs e)
        {
        }

        private Table.CellStyleInfo tableMain_SetRowStyle(object sender, TableSetRowStyleEventArgs e)
        {
            if (e.RowIndex % 2 == 0)
            {
                return new AntdUI.Table.CellStyleInfo
                {
                    BackColor = AntdUI.Style.Db.ErrorBg,
                };
            }
            return null;
        }
    }

    public class ColunmnInfo: INotifyPropertyChanged
    {
        public string ColName { get; set; }

        public bool Enable { get; set; } = true;

        /// <summary>
        /// 下拉按钮
        /// </summary>
        public CellLink ColTypeButton { get; set; }

        public ColumnSettingInfo Tag { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;
    }


}
