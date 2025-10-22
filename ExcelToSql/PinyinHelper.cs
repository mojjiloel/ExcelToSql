using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using NPinyin;

namespace ExcelToSql
{
    /// <summary>
    /// 拼音转换模式
    /// </summary>
    public enum PinyinMode
    {
        /// <summary>
        /// 全拼模式（如：PingZhengRiQi）
        /// </summary>
        FullPinyin,
        
        /// <summary>
        /// 首字母模式（如：PZRQ）
        /// </summary>
        FirstLetter
    }

    /// <summary>
    /// 列名转换辅助类（集成NPinyin库 + 智能词汇映射）
    /// </summary>
    public class PinyinHelper
    {
        private static Dictionary<string, string> commonNameDict = new Dictionary<string, string>();
        private static Dictionary<char, string> pinyinDict = new Dictionary<char, string>();
        
        /// <summary>
        /// 当前拼音转换模式（默认全拼）
        /// </summary>
        public static PinyinMode CurrentMode { get; set; } = PinyinMode.FullPinyin;

        static PinyinHelper()
        {
            InitializeCommonNameDict();
            InitializePinyinDict();
        }

        /// <summary>
        /// 将中文字符串转换为拼音（优先使用常用词汇映射，备用NPinyin）
        /// </summary>
        /// <param name="chinese">中文字符串</param>
        /// <param name="mode">拼音模式（可选，默认使用全局设置）</param>
        public static string ConvertToPinyin(string chinese, PinyinMode? mode = null)
        {
            if (string.IsNullOrEmpty(chinese))
                return "Col";

            PinyinMode useMode = mode ?? CurrentMode;
            string trimmed = chinese.Trim();
            
            // 优先级1：完全匹配常用词汇（标准英文命名）
            //if (commonNameDict.ContainsKey(trimmed))
            //{
            //    return commonNameDict[trimmed];
            //}

            // 优先级2：尝试匹配部分常用词汇（如："员工姓名"中包含"姓名"）
            foreach (var kvp in commonNameDict)
            {
                if (trimmed.Contains(kvp.Key))
                {
                    // 替换已知部分，处理剩余部分
                    string remaining = trimmed.Replace(kvp.Key, "");
                    if (string.IsNullOrEmpty(remaining))
                        return kvp.Value;
                    
                    // 对剩余部分使用NPinyin转换
                    string remainingConverted = ConvertByNPinyin(remaining, useMode);
                    if (!string.IsNullOrEmpty(remainingConverted))
                        return remainingConverted + kvp.Value;
                }
            }

            // 优先级3：使用NPinyin库转换（支持20000+汉字）
            string result = ConvertByNPinyin(trimmed, useMode);
            if (!string.IsNullOrEmpty(result) && result != "Col")
                return result;

            // 优先级4：降级到内置拼音字典
            return ConvertByPinyin(trimmed, useMode);
        }

        /// <summary>
        /// 使用NPinyin库转换（完整汉字支持）
        /// </summary>
        private static string ConvertByNPinyin(string text, PinyinMode mode)
        {
            if (string.IsNullOrEmpty(text))
                return "Col";

            try
            {
                string result;
                
                if (mode == PinyinMode.FirstLetter)
                {
                    // 首字母模式
                    result = Pinyin.GetInitials(text);
                }
                else
                {
                    // 全拼模式
                    result = Pinyin.GetPinyin(text);
                    // 移除空格
                    result = result.Replace(" ", "");
                }
                
                if (string.IsNullOrEmpty(result))
                    return "Col";
                
                // 移除非字母数字字符
                StringBuilder sb = new StringBuilder();
                foreach (char c in result)
                {
                    if (char.IsLetterOrDigit(c) || c == '_')
                        sb.Append(c);
                }

                result = sb.ToString();
                if (string.IsNullOrEmpty(result))
                    return "Col";

                // 格式化：首字母大写或全大写
                if (mode == PinyinMode.FirstLetter)
                {
                    // 首字母模式：全部大写（如：PZRQ）
                    result = result.ToUpper();
                }
                else
                {
                    // 全拼模式：驼峰命名（如：PingZhengRiQi）
                    result = char.ToUpper(result[0]) + (result.Length > 1 ? result.Substring(1) : "");
                }
                
                return result;
            }
            catch (Exception)
            {
                // NPinyin转换失败，返回默认值
                return "Col";
            }
        }

        /// <summary>
        /// 使用内置拼音字典转换（备用方案）
        /// </summary>
        private static string ConvertByPinyin(string text, PinyinMode mode)
        {
            if (string.IsNullOrEmpty(text))
                return "Col";

            StringBuilder sb = new StringBuilder();
            
            if (mode == PinyinMode.FirstLetter)
            {
                // 首字母模式：只取每个字的首字母
                foreach (char c in text)
                {
                    if (pinyinDict.ContainsKey(c))
                    {
                        string pinyin = pinyinDict[c];
                        if (!string.IsNullOrEmpty(pinyin))
                            sb.Append(char.ToUpper(pinyin[0]));
                    }
                    else if (IsEnglishLetter(c))
                    {
                        sb.Append(char.ToUpper(c));
                    }
                    else if (char.IsDigit(c))
                    {
                        sb.Append(c);
                    }
                }
            }
            else
            {
                // 全拼模式：完整拼音
                foreach (char c in text)
                {
                    if (pinyinDict.ContainsKey(c))
                    {
                        sb.Append(pinyinDict[c]);
                    }
                    else if (IsEnglishLetter(c) || char.IsDigit(c))
                    {
                        sb.Append(c);
                    }
                    else if (c == '_')
                    {
                        sb.Append(c);
                    }
                }
            }

            string result = sb.ToString();
            if (string.IsNullOrEmpty(result))
                result = "Col";
            
            // 格式化
            if (mode == PinyinMode.FirstLetter)
            {
                // 首字母模式：全部大写
                result = result.ToUpper();
            }
            else if (result.Length > 0)
            {
                // 全拼模式：驼峰命名（首字母大写）
                result = char.ToUpper(result[0]) + (result.Length > 1 ? result.Substring(1) : "");
            }

            return result;
        }

        /// <summary>
        /// 转换列名为数据库兼容格式
        /// </summary>
        public static string ConvertToDbColumnName(string columnName)
        {
            if (string.IsNullOrEmpty(columnName))
                return "Column1";

            string result = ConvertToPinyin(columnName.Trim());
            
            // 确保以字母开头
            if (result.Length > 0 && !char.IsLetter(result[0]))
                result = "C_" + result;

            // 替换非法字符为下划线
            StringBuilder sb = new StringBuilder();
            foreach (char c in result)
            {
                if (char.IsLetterOrDigit(c) || c == '_')
                    sb.Append(c);
                else
                    sb.Append('_');
            }

            // 移除连续的下划线
            result = Regex.Replace(sb.ToString(), "_{2,}", "_");
            
            // 移除首尾下划线
            result = result.Trim('_');
            
            return string.IsNullOrEmpty(result) ? "Column1" : result;
        }

        /// <summary>
        /// 初始化常用词汇映射字典（中文 -> 英文/拼音）
        /// </summary>
        private static void InitializeCommonNameDict()
        {
            // 常用字段名映射（优先使用英文标准命名）
            commonNameDict["编号"] = "ID";
            commonNameDict["序号"] = "SeqNo";
            commonNameDict["标识"] = "ID";
            commonNameDict["主键"] = "ID";
            
            // 人员相关
            commonNameDict["姓名"] = "Name";
            commonNameDict["名称"] = "Name";
            commonNameDict["用户名"] = "Username";
            commonNameDict["昵称"] = "Nickname";
            commonNameDict["年龄"] = "Age";
            commonNameDict["性别"] = "Gender";
            commonNameDict["生日"] = "Birthday";
            commonNameDict["出生日期"] = "Birthday";
            
            // 联系方式
            commonNameDict["电话"] = "Phone";
            commonNameDict["手机"] = "Mobile";
            commonNameDict["手机号"] = "Mobile";
            commonNameDict["电话号码"] = "PhoneNumber";
            commonNameDict["邮箱"] = "Email";
            commonNameDict["邮件"] = "Email";
            commonNameDict["地址"] = "Address";
            commonNameDict["邮编"] = "ZipCode";
            commonNameDict["邮政编码"] = "PostalCode";
            
            // 时间相关
            commonNameDict["日期"] = "Date";
            commonNameDict["时间"] = "Time";
            commonNameDict["创建时间"] = "CreateTime";
            commonNameDict["创建日期"] = "CreateDate";
            commonNameDict["修改时间"] = "UpdateTime";
            commonNameDict["更新时间"] = "UpdateTime";
            commonNameDict["删除时间"] = "DeleteTime";
            commonNameDict["开始时间"] = "StartTime";
            commonNameDict["结束时间"] = "EndTime";
            commonNameDict["入职日期"] = "HireDate";
            commonNameDict["离职日期"] = "LeaveDate";
            
            // 组织架构
            commonNameDict["公司"] = "Company";
            commonNameDict["部门"] = "Department";
            commonNameDict["职位"] = "Position";
            commonNameDict["岗位"] = "Position";
            commonNameDict["职务"] = "Title";
            commonNameDict["职称"] = "Title";
            commonNameDict["级别"] = "Level";
            commonNameDict["等级"] = "Grade";
            
            // 财务相关
            commonNameDict["金额"] = "Amount";
            commonNameDict["价格"] = "Price";
            commonNameDict["单价"] = "UnitPrice";
            commonNameDict["总价"] = "TotalPrice";
            commonNameDict["费用"] = "Fee";
            commonNameDict["工资"] = "Salary";
            commonNameDict["薪资"] = "Salary";
            commonNameDict["奖金"] = "Bonus";
            commonNameDict["收入"] = "Income";
            commonNameDict["支出"] = "Expense";
            commonNameDict["余额"] = "Balance";
            
            // 数量相关
            commonNameDict["数量"] = "Quantity";
            commonNameDict["数目"] = "Count";
            commonNameDict["总数"] = "Total";
            commonNameDict["库存"] = "Stock";
            commonNameDict["重量"] = "Weight";
            
            // 状态相关
            commonNameDict["状态"] = "Status";
            commonNameDict["类型"] = "Type";
            commonNameDict["类别"] = "Category";
            commonNameDict["分类"] = "Category";
            commonNameDict["标签"] = "Tag";
            commonNameDict["标记"] = "Flag";
            
            // 描述相关
            commonNameDict["备注"] = "Remark";
            commonNameDict["说明"] = "Description";
            commonNameDict["描述"] = "Description";
            commonNameDict["注释"] = "Comment";
            commonNameDict["内容"] = "Content";
            commonNameDict["详情"] = "Detail";
            
            // 业务相关
            commonNameDict["客户"] = "Customer";
            commonNameDict["供应商"] = "Supplier";
            commonNameDict["产品"] = "Product";
            commonNameDict["商品"] = "Product";
            commonNameDict["订单"] = "Order";
            commonNameDict["订单号"] = "OrderNo";
            commonNameDict["合同"] = "Contract";
            commonNameDict["项目"] = "Project";
            
            // 账号相关
            commonNameDict["账号"] = "Account";
            commonNameDict["密码"] = "Password";
            commonNameDict["令牌"] = "Token";
            
            // 其他常用
            commonNameDict["图片"] = "Image";
            commonNameDict["照片"] = "Photo";
            commonNameDict["文件"] = "File";
            commonNameDict["链接"] = "Link";
            commonNameDict["网址"] = "URL";
            commonNameDict["代码"] = "Code";
            commonNameDict["编码"] = "Code";
            commonNameDict["排序"] = "Sort";
            commonNameDict["排序号"] = "SortOrder";
            commonNameDict["是否"] = "IsFlag";
            commonNameDict["启用"] = "Enabled";
            commonNameDict["禁用"] = "Disabled";
            commonNameDict["删除"] = "Deleted";
        }

        private static bool IsEnglishLetter(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
        }

        /// <summary>
        /// 初始化拼音字典（常用汉字拼音映射）
        /// </summary>
        private static void InitializePinyinDict()
        {
            // 常用汉字拼音映射
            pinyinDict['姓'] = "Xing";
            pinyinDict['名'] = "Ming";
            pinyinDict['年'] = "Nian";
            pinyinDict['龄'] = "Ling";
            pinyinDict['性'] = "Xing";
            pinyinDict['别'] = "Bie";
            pinyinDict['地'] = "Di";
            pinyinDict['址'] = "Zhi";
            pinyinDict['电'] = "Dian";
            pinyinDict['话'] = "Hua";
            pinyinDict['邮'] = "You";
            pinyinDict['箱'] = "Xiang";
            pinyinDict['编'] = "Bian";
            pinyinDict['号'] = "Hao";
            pinyinDict['日'] = "Ri";
            pinyinDict['期'] = "Qi";
            pinyinDict['时'] = "Shi";
            pinyinDict['间'] = "Jian";
            pinyinDict['金'] = "Jin";
            pinyinDict['额'] = "E";
            pinyinDict['数'] = "Shu";
            pinyinDict['量'] = "Liang";
            pinyinDict['价'] = "Jia";
            pinyinDict['格'] = "Ge";
            pinyinDict['单'] = "Dan";
            pinyinDict['位'] = "Wei";
            pinyinDict['部'] = "Bu";
            pinyinDict['门'] = "Men";
            pinyinDict['职'] = "Zhi";
            pinyinDict['务'] = "Wu";
            pinyinDict['工'] = "Gong";
            pinyinDict['资'] = "Zi";
            pinyinDict['产'] = "Chan";
            pinyinDict['品'] = "Pin";
            pinyinDict['订'] = "Ding";
            pinyinDict['项'] = "Xiang";
            pinyinDict['目'] = "Mu";
            pinyinDict['客'] = "Ke";
            pinyinDict['户'] = "Hu";
            pinyinDict['供'] = "Gong";
            pinyinDict['应'] = "Ying";
            pinyinDict['商'] = "Shang";
            pinyinDict['备'] = "Bei";
            pinyinDict['注'] = "Zhu";
            pinyinDict['描'] = "Miao";
            pinyinDict['述'] = "Shu";
            pinyinDict['说'] = "Shuo";
            pinyinDict['明'] = "Ming";
            pinyinDict['类'] = "Lei";
            pinyinDict['型'] = "Xing";
            pinyinDict['状'] = "Zhuang";
            pinyinDict['态'] = "Tai";
            pinyinDict['码'] = "Ma";
            pinyinDict['员'] = "Yuan";
            pinyinDict['人'] = "Ren";
            pinyinDict['成'] = "Cheng";
            pinyinDict['本'] = "Ben";
            pinyinDict['利'] = "Li";
            pinyinDict['润'] = "Run";
            pinyinDict['收'] = "Shou";
            pinyinDict['入'] = "Ru";
            pinyinDict['支'] = "Zhi";
            pinyinDict['出'] = "Chu";
            pinyinDict['余'] = "Yu";
            pinyinDict['月'] = "Yue";
            pinyinDict['省'] = "Sheng";
            pinyinDict['市'] = "Shi";
            pinyinDict['区'] = "Qu";
            pinyinDict['县'] = "Xian";
            pinyinDict['镇'] = "Zhen";
            pinyinDict['街'] = "Jie";
            pinyinDict['道'] = "Dao";
            pinyinDict['公'] = "Gong";
            pinyinDict['司'] = "Si";
            pinyinDict['企'] = "Qi";
            pinyinDict['业'] = "Ye";
            pinyinDict['行'] = "Hang";
            pinyinDict['银'] = "Yin";
            pinyinDict['账'] = "Zhang";
            pinyinDict['卡'] = "Ka";
            pinyinDict['信'] = "Xin";
            pinyinDict['用'] = "Yong";
            pinyinDict['积'] = "Ji";
            pinyinDict['分'] = "Fen";
            pinyinDict['等'] = "Deng";
            pinyinDict['级'] = "Ji";
            pinyinDict['会'] = "Hui";
            pinyinDict['费'] = "Fei";
            pinyinDict['发'] = "Fa";
            pinyinDict['票'] = "Piao";
            pinyinDict['税'] = "Shui";
            pinyinDict['率'] = "Lv";
            pinyinDict['折'] = "Zhe";
            pinyinDict['扣'] = "Kou";
            pinyinDict['优'] = "You";
            pinyinDict['惠'] = "Hui";
            pinyinDict['活'] = "Huo";
            pinyinDict['动'] = "Dong";
            pinyinDict['始'] = "Shi";
            pinyinDict['结'] = "Jie";
            pinyinDict['束'] = "Shu";
            pinyinDict['开'] = "Kai";
            pinyinDict['关'] = "Guan";
            pinyinDict['创'] = "Chuang";
            pinyinDict['建'] = "Jian";
            pinyinDict['更'] = "Geng";
            pinyinDict['新'] = "Xin";
            pinyinDict['修'] = "Xiu";
            pinyinDict['改'] = "Gai";
            pinyinDict['删'] = "Shan";
            pinyinDict['除'] = "Chu";
            pinyinDict['增'] = "Zeng";
            pinyinDict['加'] = "Jia";
            pinyinDict['减'] = "Jian";
            pinyinDict['少'] = "Shao";
            pinyinDict['总'] = "Zong";
            pinyinDict['计'] = "Ji";
            pinyinDict['平'] = "Ping";
            pinyinDict['均'] = "Jun";
            pinyinDict['最'] = "Zui";
            pinyinDict['大'] = "Da";
            pinyinDict['小'] = "Xiao";
            pinyinDict['高'] = "Gao";
            pinyinDict['低'] = "Di";
            pinyinDict['长'] = "Chang";
            pinyinDict['短'] = "Duan";
            pinyinDict['宽'] = "Kuan";
            pinyinDict['窄'] = "Zhai";
            pinyinDict['厚'] = "Hou";
            pinyinDict['薄'] = "Bao";
            pinyinDict['重'] = "Zhong";
            pinyinDict['轻'] = "Qing";
            pinyinDict['快'] = "Kuai";
            pinyinDict['慢'] = "Man";
            pinyinDict['多'] = "Duo";
            pinyinDict['好'] = "Hao";
            pinyinDict['坏'] = "Huai";
            pinyinDict['正'] = "Zheng";
            pinyinDict['常'] = "Chang";
            pinyinDict['异'] = "Yi";
            pinyinDict['有'] = "You";
            pinyinDict['无'] = "Wu";
            pinyinDict['是'] = "Shi";
            pinyinDict['否'] = "Fou";
            pinyinDict['真'] = "Zhen";
            pinyinDict['假'] = "Jia";
            pinyinDict['男'] = "Nan";
            pinyinDict['女'] = "Nv";
            pinyinDict['老'] = "Lao";
            pinyinDict['幼'] = "You";
            pinyinDict['中'] = "Zhong";
            pinyinDict['外'] = "Wai";
            pinyinDict['内'] = "Nei";
            pinyinDict['上'] = "Shang";
            pinyinDict['下'] = "Xia";
            pinyinDict['左'] = "Zuo";
            pinyinDict['右'] = "You";
            pinyinDict['前'] = "Qian";
            pinyinDict['后'] = "Hou";
            pinyinDict['东'] = "Dong";
            pinyinDict['西'] = "Xi";
            pinyinDict['南'] = "Nan";
            pinyinDict['北'] = "Bei";
        }
    }
}
