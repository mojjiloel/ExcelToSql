using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExcelToSql
{
    public class ColumnSettingInfo
    {
        public string ColumnName { get; set; }
        public string DisplayName { get; set; }

        public string ColumnType { get; set; }
        public bool IsEnabled { get; set; }
    }
}
