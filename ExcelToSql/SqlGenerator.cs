using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ExcelToSql
{
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

        public SqlGenerator(string tableName, DataTable dataTable)
        {
            this.tableName = SanitizeTableName(tableName);
            this.dataTable = dataTable;
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

        public override string GenerateDropTableSql()
        {
            return string.Format("IF OBJECT_ID('{0}', 'U') IS NOT NULL DROP TABLE [{0}];", tableName);
        }

        public override string GenerateCreateTableSql()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("CREATE TABLE [{0}] (\r\n", tableName);

            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                DataColumn col = dataTable.Columns[i];
                sb.AppendFormat("    [{0}] {1}", col.ColumnName, GetDbType(col.DataType));
                
                if (i < dataTable.Columns.Count - 1)
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

            foreach (DataRow row in dataTable.Rows)
            {
                sb.AppendFormat("INSERT INTO [{0}] (", tableName);
                
                // 列名
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    sb.AppendFormat("[{0}]", dataTable.Columns[i].ColumnName);
                    if (i < dataTable.Columns.Count - 1)
                        sb.Append(", ");
                }
                
                sb.Append(") VALUES (");
                
                // 值
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    sb.Append(FormatValue(row[i], dataTable.Columns[i].DataType));
                    if (i < dataTable.Columns.Count - 1)
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

        public override string GenerateDropTableSql()
        {
            return string.Format("DROP TABLE IF EXISTS `{0}`;", tableName);
        }

        public override string GenerateCreateTableSql()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("CREATE TABLE `{0}` (\r\n", tableName);

            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                DataColumn col = dataTable.Columns[i];
                sb.AppendFormat("    `{0}` {1}", col.ColumnName, GetDbType(col.DataType));
                
                if (i < dataTable.Columns.Count - 1)
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

            foreach (DataRow row in dataTable.Rows)
            {
                sb.AppendFormat("INSERT INTO `{0}` (", tableName);
                
                // 列名
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    sb.AppendFormat("`{0}`", dataTable.Columns[i].ColumnName);
                    if (i < dataTable.Columns.Count - 1)
                        sb.Append(", ");
                }
                
                sb.Append(") VALUES (");
                
                // 值
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    sb.Append(FormatValue(row[i], dataTable.Columns[i].DataType));
                    if (i < dataTable.Columns.Count - 1)
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

            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                DataColumn col = dataTable.Columns[i];
                sb.AppendFormat("    {0} {1}", col.ColumnName, GetDbType(col.DataType));
                
                if (i < dataTable.Columns.Count - 1)
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

            foreach (DataRow row in dataTable.Rows)
            {
                sb.AppendFormat("INSERT INTO {0} (", tableName);
                
                // 列名
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    sb.Append(dataTable.Columns[i].ColumnName);
                    if (i < dataTable.Columns.Count - 1)
                        sb.Append(", ");
                }
                
                sb.Append(") VALUES (");
                
                // 值
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    sb.Append(FormatValue(row[i], dataTable.Columns[i].DataType));
                    if (i < dataTable.Columns.Count - 1)
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
    }
}
