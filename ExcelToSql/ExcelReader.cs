using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
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
        public bool isCsvFile;
        private List<string[]> csvData;
        // 用于存储当前编码
        private Encoding currentEncoding = Encoding.UTF8;

        public ExcelReader(string filePath)
        {
            this.filePath = filePath;
            string extension = Path.GetExtension(filePath).ToLower();
            isCsvFile = extension == ".csv";
            
            if (isCsvFile)
            {
                LoadCsvData(currentEncoding);
            }
            else
            {
                LoadWorkbook();
            }
        }

        /// <summary>
        /// 加载CSV数据
        /// </summary>
        private void LoadCsvData(Encoding encoding)
        {
            // 保存当前编码
            currentEncoding = encoding;
            
            csvData = new List<string[]>();
            using (var reader = new StreamReader(filePath, encoding))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] values = ParseCsvLine(line);
                    csvData.Add(values);
                }
            }
        }

        /// <summary>
        /// 解析CSV行数据
        /// </summary>
        private string[] ParseCsvLine(string line)
        {
            List<string> values = new List<string>();
            StringBuilder currentValue = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"' && !inQuotes)
                {
                    inQuotes = true;
                }
                else if (c == '"' && inQuotes)
                {
                    // 检查下一个字符是否也是引号（转义引号）
                    if (i + 1 < line.Length && line[i + 1] == '"')
                    {
                        currentValue.Append('"');
                        i++; // 跳过下一个引号
                    }
                    else
                    {
                        inQuotes = false;
                    }
                }
                else if (c == ',' && !inQuotes)
                {
                    values.Add(currentValue.ToString());
                    currentValue.Clear();
                }
                else
                {
                    currentValue.Append(c);
                }
            }

            // 添加最后一个值
            values.Add(currentValue.ToString());

            return values.ToArray();
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
                    throw new Exception("不支持的文件格式，仅支持.xls、.xlsx和.csv文件");
                }
            }
        }

        /// <summary>
        /// 获取所有Sheet名称
        /// </summary>
        public List<string> GetSheetNames()
        {
            if (isCsvFile)
            {
                // CSV文件只有一个"Sheet"
                return new List<string> { "Sheet1" };
            }
            
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
            if (isCsvFile)
            {
                return ReadCsvToDataTable(headerRowIndex, convertColumnNames);
            }
            
            ISheet sheet = workbook.GetSheetAt(sheetIndex);
            return ReadSheetToDataTable(sheet, headerRowIndex, convertColumnNames);
        }

        /// <summary>
        /// 读取指定Sheet到DataTable（通过名称）
        /// </summary>
        public DataTable ReadSheet(string sheetName, int headerRowIndex, bool convertColumnNames = true)
        {
            if (isCsvFile)
            {
                return ReadCsvToDataTable(headerRowIndex, convertColumnNames);
            }
            
            ISheet sheet = workbook.GetSheet(sheetName);
            if (sheet == null)
                throw new Exception("未找到指定的Sheet: " + sheetName);
            
            return ReadSheetToDataTable(sheet, headerRowIndex, convertColumnNames);
        }

        /// <summary>
        /// 读取指定Sheet到DataTable（支持编码参数）
        /// </summary>
        /// <param name="sheetIndex">Sheet索引</param>
        /// <param name="headerRowIndex">列头行索引（0-based）</param>
        /// <param name="convertColumnNames">是否转换列名为数据库兼容格式</param>
        /// <param name="encoding">CSV文件编码</param>
        public DataTable ReadSheet(int sheetIndex, int headerRowIndex, bool convertColumnNames = true, Encoding encoding = null)
        {
            if (isCsvFile)
            {
                // 如果是CSV文件且指定了编码，则重新加载数据
                if (encoding != null)
                {
                    LoadCsvData(encoding);
                }
                return ReadCsvToDataTable(headerRowIndex, convertColumnNames);
            }
            
            ISheet sheet = workbook.GetSheetAt(sheetIndex);
            return ReadSheetToDataTable(sheet, headerRowIndex, convertColumnNames);
        }

        /// <summary>
        /// 预览Sheet数据（返回原始数据，不做转换）
        /// </summary>
        public DataTable PreviewSheet(int sheetIndex, int maxRows = 100)
        {
            if (isCsvFile)
            {
                return PreviewCsvData(maxRows);
            }
            
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
        /// 预览CSV数据
        /// </summary>
        private DataTable PreviewCsvData(int maxRows = 100)
        {
            // 使用当前编码重新加载数据
            LoadCsvData(GetCurrentEncoding());
            
            DataTable dt = new DataTable();

            if (csvData.Count == 0)
                return dt;

            // 获取最大列数
            int maxCols = 0;
            for (int i = 0; i < Math.Min(maxRows, csvData.Count); i++)
            {
                if (csvData[i].Length > maxCols)
                    maxCols = csvData[i].Length;
            }

            // 创建列
            for (int i = 0; i < maxCols; i++)
            {
                dt.Columns.Add("Column" + (i + 1), typeof(string));
            }

            // 读取数据
            int rowCount = 0;
            for (int i = 0; i < csvData.Count && rowCount < maxRows; i++)
            {
                DataRow dr = dt.NewRow();
                string[] rowData = csvData[i];
                
                for (int j = 0; j < maxCols; j++)
                {
                    if (j < rowData.Length)
                    {
                        dr[j] = rowData[j];
                    }
                    else
                    {
                        dr[j] = string.Empty;
                    }
                }
                dt.Rows.Add(dr);
                rowCount++;
            }

            return dt;
        }

        /// <summary>
        /// 获取当前编码（用于重新加载CSV数据）
        /// </summary>
        /// <returns></returns>
        private Encoding GetCurrentEncoding()
        {
            return currentEncoding;
        }

        /// <summary>
        /// 预览Sheet数据（支持编码参数）
        /// </summary>
        public DataTable PreviewSheet(int sheetIndex, int maxRows = 100, Encoding encoding = null)
        {
            if (isCsvFile)
            {
                // 如果是CSV文件且指定了编码，则重新加载数据
                if (encoding != null)
                {
                    LoadCsvData(encoding);
                }
                return PreviewCsvData(maxRows);
            }
            
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
        /// 将CSV数据转换为DataTable
        /// </summary>
        private DataTable ReadCsvToDataTable(int headerRowIndex, bool convertColumnNames)
        {
            DataTable dt = new DataTable();

            if (csvData.Count == 0)
                return dt;

            // 检查列头行是否存在
            if (headerRowIndex >= csvData.Count)
                throw new Exception("指定的列头行不存在");

            // 读取列头
            string[] headerRow = csvData[headerRowIndex];
            Dictionary<int, string> columnNames = new Dictionary<int, string>();
            
            for (int i = 0; i < headerRow.Length; i++)
            {
                string columnName = headerRow[i];
                
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
            for (int i = headerRowIndex + 1; i < csvData.Count; i++)
            {
                string[] rowData = csvData[i];
                
                // 检查是否为空行
                bool isEmptyRow = true;
                for (int j = 0; j < headerRow.Length; j++)
                {
                    if (j < rowData.Length && !string.IsNullOrWhiteSpace(rowData[j]))
                    {
                        isEmptyRow = false;
                        break;
                    }
                }

                if (isEmptyRow)
                    continue;

                DataRow dr = dt.NewRow();
                for (int j = 0; j < headerRow.Length; j++)
                {
                    if (j < rowData.Length)
                    {
                        dr[columnNames[j]] = rowData[j];
                    }
                    else
                    {
                        dr[columnNames[j]] = string.Empty;
                    }
                }
                dt.Rows.Add(dr);
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