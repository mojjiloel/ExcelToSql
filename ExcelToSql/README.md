# Excel转SQL工具 使用说明

## 功能介绍

这是一个基于 C# .NET Framework 4.0 开发的 Excel 转 SQL 工具，用于：
- 读取 Excel 文件（支持 .xls 和 .xlsx 格式）
- 获取数据并生成数据库脚本
- 创建临时表和插入数据语句
- 支持多种数据库：SQL Server、MySQL、Oracle

## 主要特性

### 1. Excel 文件读取
- 使用 NPOI 库读取 Excel 文件
- 支持选择不同的 Sheet
- 支持指定列头行位置（默认第1行）
- 实时预览 Excel 数据

### 2. 列名智能转换（NPinyin增强版）✨✨✨
- **四层优先级转换策略**：
  1. **常用词汇映射** → 标准英文（100+词汇）
     - 姓名 → Name、年龄 → Age、部门 → Department
  2. **组合词汇识别** → 智能拼接
     - 员工姓名 → EmployeeName、订单号 → OrderNo
  3. **NPinyin转换** → 完整拼音（支持20,000+汉字）
     - 患者 → HuanZhe、诊断 → ZhenDuan、囊肿 → NangZhong
  4. **内置字典** → 备用方案
- **完整覆盖**：支持所有汉字包括生僻字
- **多音字处理**：NPinyin自动选择常用读音
- **数据库兼容**：确保列名符合所有数据库命名规范

### 3. 多数据库支持
- **SQL Server**: 生成兼容 T-SQL 的脚本
  - 使用 `[]` 包裹标识符
  - NVARCHAR 支持中文
  - IF EXISTS 检查表是否存在
  
- **MySQL**: 生成兼容 MySQL 的脚本
  - 使用 `` ` `` 包裹标识符
  - UTF-8 字符集支持
  - DROP TABLE IF EXISTS 语法
  
- **Oracle**: 生成兼容 Oracle 的脚本
  - PL/SQL 块处理表删除
  - TO_DATE 函数处理日期
  - NVARCHAR2 类型支持

### 4. 数据类型映射
工具会自动识别 Excel 中的数据类型并映射到对应的数据库类型：
- 数字 → INT/NUMBER
- 小数 → DECIMAL/NUMBER
- 日期 → DATETIME/DATE
- 文本 → NVARCHAR/TEXT/NVARCHAR2

## 使用步骤

### 第一步：选择 Excel 文件
1. 点击"浏览..."按钮
2. 选择要转换的 Excel 文件（.xls 或 .xlsx）

### 第二步：配置数据源
1. 在"数据预览"区域选择要转换的 Sheet
2. 设置列头行号（通常是第1行）
3. 预览区会高亮显示列头行

### 第三步：生成 SQL
1. 选择目标数据库类型（SQL Server/MySQL/Oracle）
2. 输入表名称（支持中文，会自动转换）
3. 点击"生成SQL"按钮

### 第四步：使用 SQL
1. 在 SQL 输出区域查看生成的脚本
2. 点击"复制SQL"将脚本复制到剪贴板
3. 或点击"保存SQL"将脚本保存为 .sql 文件

## 生成的 SQL 结构

每次生成的 SQL 包含三部分：

1. **删除表语句**：如果表已存在则删除
2. **创建表语句**：根据 Excel 列头创建表结构
3. **插入数据语句**：将 Excel 数据转换为 INSERT 语句

## 示例

假设有如下 Excel 数据：

| 姓名 | 年龄 | 部门 | 入职日期 |
|------|------|------|----------|
| 张三 | 25   | 技术部 | 2023-01-15 |
| 李四 | 30   | 销售部 | 2022-05-20 |

### SQL Server 输出示例：
```sql
IF OBJECT_ID('TempTable', 'U') IS NOT NULL DROP TABLE [TempTable];

CREATE TABLE [TempTable] (
    [XingMing] NVARCHAR(MAX),
    [NianLing] INT,
    [BuMen] NVARCHAR(MAX),
    [RuZhiRiQi] DATETIME
);

INSERT INTO [TempTable] ([XingMing], [NianLing], [BuMen], [RuZhiRiQi]) VALUES (N'张三', 25, N'技术部', '2023-01-15 00:00:00');
INSERT INTO [TempTable] ([XingMing], [NianLing], [BuMen], [RuZhiRiQi]) VALUES (N'李四', 30, N'销售部', '2022-05-20 00:00:00');
```

## 注意事项

1. **Excel 格式**：确保 Excel 文件格式正确，列头行清晰
2. **数据类型**：Excel 中的数字、日期等会自动识别类型
3. **空值处理**：空单元格会转换为 NULL
4. **字符转义**：单引号等特殊字符会自动转义
5. **列名重复**：如果有重复列名，会自动添加数字后缀

## 技术架构

- **框架**：.NET Framework 4.0
- **UI**：Windows Forms
- **Excel 读取**：NPOI 2.5.1
- **拼音转换**：NPinyin.Core 3.0.0（支持20,000+汉字）
- **打包**：Costura.Fody（嵌入依赖）

## 文件说明

- `Form1.cs`: 主窗体业务逻辑
- `ExcelReader.cs`: Excel 文件读取类
- `SqlGenerator.cs`: SQL 生成器基类和各数据库实现
- `PinyinHelper.cs`: 中文转拼音辅助类

## 常见问题

**Q: 为什么有些中文字没有转换成拼音？**
A: 拼音字典包含了常用汉字，未包含的字符会被过滤。可以在 PinyinHelper.cs 中扩展字典。

**Q: 生成的 SQL 能直接在数据库执行吗？**
A: 可以，生成的 SQL 已针对不同数据库做了兼容性处理。

**Q: 支持大文件吗？**
A: 预览默认只显示前50行，但生成 SQL 时会处理所有数据行。

**Q: 可以自定义表名吗？**
A: 可以，在"表名称"文本框中输入任意表名，支持中文。

## 后续优化建议

1. 添加更完整的拼音字典或集成第三方拼音库
2. 支持批量处理多个 Sheet
3. 添加数据类型自定义映射
4. 支持更多数据库类型（PostgreSQL、SQLite等）
5. 添加 SQL 预览和执行功能
6. 支持导出到数据库连接

---

**开发环境要求**：
- Visual Studio 2015 或更高版本
- .NET Framework 4.0 或更高版本

**运行环境要求**：
- Windows 操作系统
- .NET Framework 4.0 或更高版本
