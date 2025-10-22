using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace ExcelToSql
{
    /// <summary>
    /// Excel读取辅助类
    /// </summary>
    public class ExcelReader
    {
        private IWorkbook workbook;
        private string filePath;

        public ExcelReader(string filePath)
        {
            this.filePath = filePath;
            LoadWorkbook();
        }

        /// <summary>
        /// 加载工作簿
        /// </summary>
        private void LoadWorkbook()
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                string extension = Path.GetExtension(filePath).ToLower();
                if (extension == ".xls")
                {
                    workbook = new HSSFWorkbook(fs);
                }
                else if (extension == ".xlsx")
                {
                    workbook = new XSSFWorkbook(fs);
                }
                else
                {
                    throw new Exception("不支持的文件格式，仅支持.xls和.xlsx文件");
                }
            }
        }

        /// <summary>
        /// 获取所有Sheet名称
        /// </summary>
        public List<string> GetSheetNames()
        {
            List<string> sheetNames = new List<string>();
            for (int i = 0; i < workbook.NumberOfSheets; i++)
            {
                sheetNames.Add(workbook.GetSheetName(i));
            }
            return sheetNames;
        }

        /// <summary>
        /// 读取指定Sheet到DataTable
        /// </summary>
        /// <param name="sheetIndex">Sheet索引</param>
        /// <param name="headerRowIndex">列头行索引（0-based）</param>
        /// <param name="convertColumnNames">是否转换列名为数据库兼容格式</param>
        public DataTable ReadSheet(int sheetIndex, int headerRowIndex, bool convertColumnNames = true)
        {
            ISheet sheet = workbook.GetSheetAt(sheetIndex);
            return ReadSheetToDataTable(sheet, headerRowIndex, convertColumnNames);
        }

        /// <summary>
        /// 读取指定Sheet到DataTable（通过名称）
        /// </summary>
        public DataTable ReadSheet(string sheetName, int headerRowIndex, bool convertColumnNames = true)
        {
            ISheet sheet = workbook.GetSheet(sheetName);
            if (sheet == null)
                throw new Exception("未找到指定的Sheet: " + sheetName);
            
            return ReadSheetToDataTable(sheet, headerRowIndex, convertColumnNames);
        }

        /// <summary>
        /// 预览Sheet数据（返回原始数据，不做转换）
        /// </summary>
        public DataTable PreviewSheet(int sheetIndex, int maxRows = 100)
        {
            ISheet sheet = workbook.GetSheetAt(sheetIndex);
            DataTable dt = new DataTable();

            if (sheet.PhysicalNumberOfRows == 0)
                return dt;

            // 获取最大列数
            int maxCols = 0;
            for (int i = 0; i <= Math.Min(maxRows, sheet.LastRowNum); i++)
            {
                IRow row = sheet.GetRow(i);
                if (row != null && row.LastCellNum > maxCols)
                    maxCols = row.LastCellNum;
            }

            // 创建列
            for (int i = 0; i < maxCols; i++)
            {
                dt.Columns.Add("Column" + (i + 1), typeof(string));
            }

            // 读取数据
            int rowCount = 0;
            for (int i = 0; i <= sheet.LastRowNum && rowCount < maxRows; i++)
            {
                IRow row = sheet.GetRow(i);
                if (row == null) continue;

                DataRow dr = dt.NewRow();
                for (int j = 0; j < maxCols; j++)
                {
                    ICell cell = row.GetCell(j);
                    dr[j] = GetCellValue(cell);
                }
                dt.Rows.Add(dr);
                rowCount++;
            }

            return dt;
        }

        /// <summary>
        /// 将Sheet转换为DataTable
        /// </summary>
        private DataTable ReadSheetToDataTable(ISheet sheet, int headerRowIndex, bool convertColumnNames)
        {
            DataTable dt = new DataTable();

            if (sheet.PhysicalNumberOfRows == 0)
                return dt;

            // 读取列头
            IRow headerRow = sheet.GetRow(headerRowIndex);
            if (headerRow == null)
                throw new Exception("指定的列头行不存在");

            Dictionary<int, string> columnNames = new Dictionary<int, string>();
            for (int i = 0; i < headerRow.LastCellNum; i++)
            {
                ICell cell = headerRow.GetCell(i);
                string columnName = GetCellValue(cell);
                
                if (string.IsNullOrWhiteSpace(columnName))
                    columnName = "Column" + (i + 1);

                if (convertColumnNames)
                    columnName = PinyinHelper.ConvertToDbColumnName(columnName);
                
                // 确保列名唯一
                string uniqueName = columnName;
                int counter = 1;
                while (columnNames.ContainsValue(uniqueName))
                {
                    uniqueName = columnName + counter;
                    counter++;
                }
                
                columnNames[i] = uniqueName;
                dt.Columns.Add(uniqueName, typeof(string));
            }

            // 读取数据行（从列头的下一行开始）
            for (int i = headerRowIndex + 1; i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                if (row == null) continue;

                // 检查是否为空行
                bool isEmptyRow = true;
                for (int j = 0; j < headerRow.LastCellNum; j++)
                {
                    ICell cell = row.GetCell(j);
                    if (cell != null && cell.CellType != CellType.Blank)
                    {
                        isEmptyRow = false;
                        break;
                    }
                }

                if (isEmptyRow)
                    continue;

                DataRow dr = dt.NewRow();
                for (int j = 0; j < headerRow.LastCellNum; j++)
                {
                    ICell cell = row.GetCell(j);
                    dr[columnNames[j]] = GetCellValue(cell);
                }
                dt.Rows.Add(dr);
            }

            return dt;
        }

        /// <summary>
        /// 获取单元格的值
        /// </summary>
        private string GetCellValue(ICell cell)
        {
            if (cell == null)
                return string.Empty;

            switch (cell.CellType)
            {
                case CellType.Numeric:
                    if (DateUtil.IsCellDateFormatted(cell))
                    {
                        return cell.DateCellValue.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    return cell.NumericCellValue.ToString();

                case CellType.String:
                    return cell.StringCellValue;

                case CellType.Boolean:
                    return cell.BooleanCellValue.ToString();

                case CellType.Formula:
                    try
                    {
                        return cell.NumericCellValue.ToString();
                    }
                    catch
                    {
                        return cell.StringCellValue;
                    }

                case CellType.Blank:
                    return string.Empty;

                default:
                    return cell.ToString();
            }
        }

        /// <summary>
        /// 关闭工作簿
        /// </summary>
        public void Close()
        {
            if (workbook != null)
            {
                workbook.Close();
                workbook = null;
            }
        }
    }
}
