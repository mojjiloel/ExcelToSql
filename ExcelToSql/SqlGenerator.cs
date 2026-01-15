using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ExcelToSql
{
    /// <summary>
    /// 集中的数据类型映射管理器
    /// </summary>
    public static class DataTypeMapper
    {
        /// <summary>
        /// 通用数据类型枚举
        /// </summary>
        public static readonly HashSet<string> ValidTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "string",
            "int",
            "double",
            "decimal",
            "datetime",
            "bool"
        };
        
        /// <summary>
        /// 将通用类型转换为对应的.NET类型
        /// </summary>
        public static Type GetNetType(string typeSetting)
        {
            switch (typeSetting.ToLower())
            {
                case "int":
                    return typeof(int);
                case "double":
                case "decimal":
                    return typeof(double);
                case "datetime":
                    return typeof(DateTime);
                case "bool":
                    return typeof(bool);
                default:
                    return typeof(string);
            }
        }
        
        /// <summary>
        /// 获取SQL Server对应的数据库类型
        /// </summary>
        public static string GetSqlServerDbType(string typeSetting)
        {
            switch (typeSetting.ToLower())
            {
                case "int":
                    return "INT";
                case "double":
                case "decimal":
                    return "DECIMAL(18, 2)";
                case "datetime":
                    return "DATETIME";
                case "bool":
                    return "BIT";
                default:
                    return "NVARCHAR(MAX)";
            }
        }
        
        /// <summary>
        /// 获取MySQL对应的数据库类型
        /// </summary>
        public static string GetMySqlDbType(string typeSetting)
        {
            switch (typeSetting.ToLower())
            {
                case "int":
                    return "INT";
                case "double":
                case "decimal":
                    return "DECIMAL(18, 2)";
                case "datetime":
                    return "DATETIME";
                case "bool":
                    return "TINYINT(1)";
                default:
                    return "TEXT";
            }
        }
        
        /// <summary>
        /// 获取Oracle对应的数据库类型
        /// </summary>
        public static string GetOracleDbType(string typeSetting)
        {
            switch (typeSetting.ToLower())
            {
                case "int":
                    return "NUMBER(10)";
                case "double":
                case "decimal":
                    return "NUMBER(18, 2)";
                case "datetime":
                    return "DATE";
                case "bool":
                    return "NUMBER(1)";
                default:
                    return "NVARCHAR2(2000)";
            }
        }
    }


    /// <summary>
    /// 数据库类型枚举
    /// </summary>
    public enum DatabaseType
    {
        SqlServer,
        MySql,
        Oracle
    }

    /// <summary>
    /// SQL生成器基类
    /// </summary>
    public abstract class SqlGenerator
    {
        protected string tableName;
        protected DataTable dataTable;
        protected Dictionary<string, ExcelToSql.ColumnSettingInfo> columnSettings;

        public SqlGenerator(string tableName, DataTable dataTable)
        {
            this.tableName = SanitizeTableName(tableName);
            this.dataTable = dataTable;
        }

        public SqlGenerator(string tableName, DataTable dataTable, Dictionary<string, ExcelToSql.ColumnSettingInfo> columnSettings)
        {
            this.tableName = SanitizeTableName(tableName);
            this.dataTable = dataTable;
            this.columnSettings = columnSettings;
        }

        /// <summary>
        /// 生成完整的SQL脚本
        /// </summary>
        public string GenerateSql()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(GenerateDropTableSql());
            sb.AppendLine();
            sb.AppendLine(GenerateCreateTableSql());
            sb.AppendLine();
            sb.AppendLine(GenerateInsertSql());
            return sb.ToString();
        }

        /// <summary>
        /// 生成删除表的SQL
        /// </summary>
        public abstract string GenerateDropTableSql();

        /// <summary>
        /// 生成建表SQL
        /// </summary>
        public abstract string GenerateCreateTableSql();

        /// <summary>
        /// 生成插入数据SQL
        /// </summary>
        public abstract string GenerateInsertSql();

        /// <summary>
        /// 获取数据库类型映射
        /// </summary>
        protected abstract string GetDbType(Type dataType);

        /// <summary>
        /// 格式化值
        /// </summary>
        protected abstract string FormatValue(object value, Type dataType);

        /// <summary>
        /// 清理表名
        /// </summary>
        protected virtual string SanitizeTableName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return "TempTable";
            
            return PinyinHelper.ConvertToDbColumnName(name);
        }

        /// <summary>
        /// 转义字符串中的单引号
        /// </summary>
        protected string EscapeString(string value)
        {
            if (value == null)
                return string.Empty;
            return value.Replace("'", "''");
        }

        /// <summary>
        /// 工厂方法：创建SQL生成器
        /// </summary>
        public static SqlGenerator Create(DatabaseType dbType, string tableName, DataTable dataTable)
        {
            switch (dbType)
            {
                case DatabaseType.SqlServer:
                    return new SqlServerGenerator(tableName, dataTable);
                case DatabaseType.MySql:
                    return new MySqlGenerator(tableName, dataTable);
                case DatabaseType.Oracle:
                    return new OracleGenerator(tableName, dataTable);
                default:
                    throw new NotSupportedException("不支持的数据库类型");
            }
        }

        /// <summary>
        /// 工厂方法：创建SQL生成器（带列设置）
        /// </summary>
        public static SqlGenerator Create(DatabaseType dbType, string tableName, DataTable dataTable, Dictionary<string, ExcelToSql.ColumnSettingInfo> columnSettings)
        {
            switch (dbType)
            {
                case DatabaseType.SqlServer:
                    return new SqlServerGenerator(tableName, dataTable, columnSettings);
                case DatabaseType.MySql:
                    return new MySqlGenerator(tableName, dataTable, columnSettings);
                case DatabaseType.Oracle:
                    return new OracleGenerator(tableName, dataTable, columnSettings);
                default:
                    throw new NotSupportedException("不支持的数据库类型");
            }
        }


        /// <summary>
        /// 获取需要处理的列（根据启用状态过滤）
        /// </summary>
        protected List<DataColumn> GetColumnsToProcess()
        {
            var columnsToProcess = new List<DataColumn>();

            if (columnSettings != null)
            {
                // 如果有列设置，只处理启用的列
                foreach (DataColumn col in dataTable.Columns)
                {
                    if (columnSettings.ContainsKey(col.ColumnName) && columnSettings[col.ColumnName].IsEnabled)
                    {
                        columnsToProcess.Add(col);
                    }
                    else if (!columnSettings.ContainsKey(col.ColumnName))
                    {
                        // 如果列不在设置中，默认包含它
                        columnsToProcess.Add(col);
                    }
                }
            }
            else
            {
                // 如果没有列设置，处理所有列
                foreach (DataColumn col in dataTable.Columns)
                {
                    columnsToProcess.Add(col);
                }
            }

            return columnsToProcess;
        }
    }

    /// <summary>
    /// SQL Server SQL生成器
    /// </summary>
    public class SqlServerGenerator : SqlGenerator
    {
        public SqlServerGenerator(string tableName, DataTable dataTable)
            : base(tableName, dataTable)
        {
        }

        public SqlServerGenerator(string tableName, DataTable dataTable, Dictionary<string, ExcelToSql.ColumnSettingInfo> columnSettings)
            : base(tableName, dataTable, columnSettings)
        {
        }

        public override string GenerateDropTableSql()
        {
            return string.Format("IF OBJECT_ID('{0}', 'U') IS NOT NULL DROP TABLE [{0}];", tableName);
        }

        public override string GenerateCreateTableSql()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("CREATE TABLE [{0}] (\r\n", tableName);

            // 确定要处理的列
            var columnsToProcess = GetColumnsToProcess();

            for (int i = 0; i < columnsToProcess.Count; i++)
            {
                var col = columnsToProcess[i];
                string dbType;
                
                // 如果有列设置，使用设置的类型，否则使用默认的数据类型
                if (columnSettings != null && columnSettings.ContainsKey(col.ColumnName) && !string.IsNullOrEmpty(columnSettings[col.ColumnName].ColumnType))
                {
                    dbType = GetDbTypeFromSetting(columnSettings[col.ColumnName].ColumnType);
                }
                else
                {
                    dbType = GetDbType(col.DataType);
                }
                
                sb.AppendFormat("    [{0}] {1}", col.ColumnName, dbType);
                
                if (i < columnsToProcess.Count - 1)
                    sb.AppendLine(",");
                else
                    sb.AppendLine();
            }

            sb.Append(");");
            return sb.ToString();
        }

        public override string GenerateInsertSql()
        {
            StringBuilder sb = new StringBuilder();

            // 确定要处理的列
            var columnsToProcess = GetColumnsToProcess();

            foreach (DataRow row in dataTable.Rows)
            {
                sb.AppendFormat("INSERT INTO [{0}] (", tableName);
                
                // 列名
                for (int i = 0; i < columnsToProcess.Count; i++)
                {
                    sb.AppendFormat("[{0}]", columnsToProcess[i].ColumnName);
                    if (i < columnsToProcess.Count - 1)
                        sb.Append(", ");
                }
                
                sb.Append(") VALUES (");
                
                // 值
                for (int i = 0; i < columnsToProcess.Count; i++)
                {
                    var col = columnsToProcess[i];
                    Type dataType;
                    
                    // 如果有列设置，使用设置的类型，否则使用默认的数据类型
                    if (columnSettings != null && columnSettings.ContainsKey(col.ColumnName) && !string.IsNullOrEmpty(columnSettings[col.ColumnName].ColumnType))
                    {
                        dataType = GetSqlServerDataTypeFromSetting(columnSettings[col.ColumnName].ColumnType);
                    }
                    else
                    {
                        dataType = col.DataType;
                    }
                    
                    sb.Append(FormatValue(row[col.ColumnName], dataType));
                    if (i < columnsToProcess.Count - 1)
                        sb.Append(", ");
                }
                
                sb.AppendLine(");");
            }

            return sb.ToString();
        }

        protected override string GetDbType(Type dataType)
        {
            if (dataType == typeof(int) || dataType == typeof(long))
                return "INT";
            if (dataType == typeof(decimal) || dataType == typeof(double) || dataType == typeof(float))
                return "DECIMAL(18, 2)";
            if (dataType == typeof(DateTime))
                return "DATETIME";
            if (dataType == typeof(bool))
                return "BIT";
            
            return "NVARCHAR(MAX)";
        }

        protected override string FormatValue(object value, Type dataType)
        {
            if (value == null || value == DBNull.Value)
                return "NULL";

            if (dataType == typeof(int) || dataType == typeof(long) || 
                dataType == typeof(decimal) || dataType == typeof(double) || dataType == typeof(float))
                return value.ToString();

            if (dataType == typeof(bool))
                return ((bool)value) ? "1" : "0";

            if (dataType == typeof(DateTime))
                return string.Format("'{0:yyyy-MM-dd HH:mm:ss}'", value);

            return string.Format("N'{0}'", EscapeString(value.ToString()));
        }

        /// <summary>
        /// 根据设置获取数据库类型
        /// </summary>
        protected string GetDbTypeFromSetting(string typeSetting)
        {
            return DataTypeMapper.GetSqlServerDbType(typeSetting);
        }

        /// <summary>
        /// 根据设置获取数据类型
        /// </summary>
        protected Type GetSqlServerDataTypeFromSetting(string typeSetting)
        {
            return DataTypeMapper.GetNetType(typeSetting);
        }
    }

    /// <summary>
    /// MySQL SQL生成器
    /// </summary>
    public class MySqlGenerator : SqlGenerator
    {
        public MySqlGenerator(string tableName, DataTable dataTable)
            : base(tableName, dataTable)
        {
        }

        public MySqlGenerator(string tableName, DataTable dataTable, Dictionary<string, ExcelToSql.ColumnSettingInfo> columnSettings)
            : base(tableName, dataTable, columnSettings)
        {
        }

        public override string GenerateDropTableSql()
        {
            return string.Format("DROP TABLE IF EXISTS `{0}`;", tableName);
        }

        public override string GenerateCreateTableSql()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("CREATE TABLE `{0}` (\r\n", tableName);

            // 确定要处理的列
            var columnsToProcess = GetColumnsToProcess();

            for (int i = 0; i < columnsToProcess.Count; i++)
            {
                var col = columnsToProcess[i];
                string dbType;
                
                // 如果有列设置，使用设置的类型，否则使用默认的数据类型
                if (columnSettings != null && columnSettings.ContainsKey(col.ColumnName) && !string.IsNullOrEmpty(columnSettings[col.ColumnName].ColumnType))
                {
                    dbType = GetMySqlDbTypeFromSetting(columnSettings[col.ColumnName].ColumnType);
                }
                else
                {
                    dbType = GetDbType(col.DataType);
                }
                
                sb.AppendFormat("    `{0}` {1}", col.ColumnName, dbType);
                
                if (i < columnsToProcess.Count - 1)
                    sb.AppendLine(",");
                else
                    sb.AppendLine();
            }

            sb.Append(") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;");
            return sb.ToString();
        }

        public override string GenerateInsertSql()
        {
            StringBuilder sb = new StringBuilder();

            // 确定要处理的列
            var columnsToProcess = GetColumnsToProcess();

            foreach (DataRow row in dataTable.Rows)
            {
                sb.AppendFormat("INSERT INTO `{0}` (", tableName);
                
                // 列名
                for (int i = 0; i < columnsToProcess.Count; i++)
                {
                    sb.AppendFormat("`{0}`", columnsToProcess[i].ColumnName);
                    if (i < columnsToProcess.Count - 1)
                        sb.Append(", ");
                }
                
                sb.Append(") VALUES (");
                
                // 值
                for (int i = 0; i < columnsToProcess.Count; i++)
                {
                    var col = columnsToProcess[i];
                    Type dataType;
                    
                    // 如果有列设置，使用设置的类型，否则使用默认的数据类型
                    if (columnSettings != null && columnSettings.ContainsKey(col.ColumnName) && !string.IsNullOrEmpty(columnSettings[col.ColumnName].ColumnType))
                    {
                        dataType = GetMySqlDataTypeFromSetting(columnSettings[col.ColumnName].ColumnType);
                    }
                    else
                    {
                        dataType = col.DataType;
                    }
                    
                    sb.Append(FormatValue(row[col.ColumnName], dataType));
                    if (i < columnsToProcess.Count - 1)
                        sb.Append(", ");
                }
                
                sb.AppendLine(");");
            }

            return sb.ToString();
        }

        protected override string GetDbType(Type dataType)
        {
            if (dataType == typeof(int) || dataType == typeof(long))
                return "INT";
            if (dataType == typeof(decimal) || dataType == typeof(double) || dataType == typeof(float))
                return "DECIMAL(18, 2)";
            if (dataType == typeof(DateTime))
                return "DATETIME";
            if (dataType == typeof(bool))
                return "TINYINT(1)";
            
            return "TEXT";
        }

        protected override string FormatValue(object value, Type dataType)
        {
            if (value == null || value == DBNull.Value)
                return "NULL";

            if (dataType == typeof(int) || dataType == typeof(long) || 
                dataType == typeof(decimal) || dataType == typeof(double) || dataType == typeof(float))
                return value.ToString();

            if (dataType == typeof(bool))
                return ((bool)value) ? "1" : "0";

            if (dataType == typeof(DateTime))
                return string.Format("'{0:yyyy-MM-dd HH:mm:ss}'", value);

            return string.Format("'{0}'", EscapeString(value.ToString()));
        }

        /// <summary>
        /// 根据设置获取MySQL数据库类型
        /// </summary>
        protected string GetMySqlDbTypeFromSetting(string typeSetting)
        {
            return DataTypeMapper.GetMySqlDbType(typeSetting);
        }

        /// <summary>
        /// 根据设置获取数据类型
        /// </summary>
        protected Type GetMySqlDataTypeFromSetting(string typeSetting)
        {
            return DataTypeMapper.GetNetType(typeSetting);
        }
    }

    /// <summary>
    /// Oracle SQL生成器
    /// </summary>
    public class OracleGenerator : SqlGenerator
    {
        public OracleGenerator(string tableName, DataTable dataTable)
            : base(tableName, dataTable)
        {
        }

        public OracleGenerator(string tableName, DataTable dataTable, Dictionary<string, ExcelToSql.ColumnSettingInfo> columnSettings)
            : base(tableName, dataTable, columnSettings)
        {
        }

        public override string GenerateDropTableSql()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("BEGIN");
            sb.AppendFormat("   EXECUTE IMMEDIATE 'DROP TABLE {0}';\r\n", tableName);
            sb.AppendLine("EXCEPTION");
            sb.AppendLine("   WHEN OTHERS THEN NULL;");
            sb.Append("END;");
            return sb.ToString();
        }

        public override string GenerateCreateTableSql()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("CREATE TABLE {0} (\r\n", tableName);

            // 确定要处理的列
            var columnsToProcess = GetColumnsToProcess();

            for (int i = 0; i < columnsToProcess.Count; i++)
            {
                var col = columnsToProcess[i];
                string dbType;
                
                // 如果有列设置，使用设置的类型，否则使用默认的数据类型
                if (columnSettings != null && columnSettings.ContainsKey(col.ColumnName) && !string.IsNullOrEmpty(columnSettings[col.ColumnName].ColumnType))
                {
                    dbType = GetOracleDbTypeFromSetting(columnSettings[col.ColumnName].ColumnType);
                }
                else
                {
                    dbType = GetDbType(col.DataType);
                }
                
                sb.AppendFormat("    {0} {1}", col.ColumnName, dbType);
                
                if (i < columnsToProcess.Count - 1)
                    sb.AppendLine(",");
                else
                    sb.AppendLine();
            }

            sb.Append(");");
            return sb.ToString();
        }

        public override string GenerateInsertSql()
        {
            StringBuilder sb = new StringBuilder();

            // 确定要处理的列
            var columnsToProcess = GetColumnsToProcess();

            foreach (DataRow row in dataTable.Rows)
            {
                sb.AppendFormat("INSERT INTO {0} (", tableName);
                
                // 列名
                for (int i = 0; i < columnsToProcess.Count; i++)
                {
                    sb.Append(columnsToProcess[i].ColumnName);
                    if (i < columnsToProcess.Count - 1)
                        sb.Append(", ");
                }
                
                sb.Append(") VALUES (");
                
                // 值
                for (int i = 0; i < columnsToProcess.Count; i++)
                {
                    var col = columnsToProcess[i];
                    Type dataType;
                    
                    // 如果有列设置，使用设置的类型，否则使用默认的数据类型
                    if (columnSettings != null && columnSettings.ContainsKey(col.ColumnName) && !string.IsNullOrEmpty(columnSettings[col.ColumnName].ColumnType))
                    {
                        dataType = GetOracleDataTypeFromSetting(columnSettings[col.ColumnName].ColumnType);
                    }
                    else
                    {
                        dataType = col.DataType;
                    }
                    
                    sb.Append(FormatValue(row[col.ColumnName], dataType));
                    if (i < columnsToProcess.Count - 1)
                        sb.Append(", ");
                }
                
                sb.AppendLine(");");
            }

            return sb.ToString();
        }

        protected override string GetDbType(Type dataType)
        {
            if (dataType == typeof(int) || dataType == typeof(long))
                return "NUMBER(10)";
            if (dataType == typeof(decimal) || dataType == typeof(double) || dataType == typeof(float))
                return "NUMBER(18, 2)";
            if (dataType == typeof(DateTime))
                return "DATE";
            if (dataType == typeof(bool))
                return "NUMBER(1)";
            
            return "NVARCHAR2(2000)";
        }

        protected override string FormatValue(object value, Type dataType)
        {
            if (value == null || value == DBNull.Value)
                return "NULL";

            if (dataType == typeof(int) || dataType == typeof(long) || 
                dataType == typeof(decimal) || dataType == typeof(double) || dataType == typeof(float))
                return value.ToString();

            if (dataType == typeof(bool))
                return ((bool)value) ? "1" : "0";

            if (dataType == typeof(DateTime))
                return string.Format("TO_DATE('{0:yyyy-MM-dd HH:mm:ss}', 'YYYY-MM-DD HH24:MI:SS')", value);

            return string.Format("'{0}'", EscapeString(value.ToString()));
        }

        /// <summary>
        /// 根据设置获取Oracle数据库类型
        /// </summary>
        protected string GetOracleDbTypeFromSetting(string typeSetting)
        {
            return DataTypeMapper.GetOracleDbType(typeSetting);
        }

        /// <summary>
        /// 根据设置获取数据类型
        /// </summary>
        protected Type GetOracleDataTypeFromSetting(string typeSetting)
        {
            return DataTypeMapper.GetNetType(typeSetting);
        }
    }
}
