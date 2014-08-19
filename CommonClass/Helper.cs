using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CarlosAg.ExcelXmlWriter;

namespace GetOAInfomations
{
    public static class Helper
    {

        #region 验证域用户

        [DllImport("Advapi32.dll")]
        public static extern int LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, ref int phToken);
        [DllImport("Advapi32.dll")]
        public static extern int ImpersonateLoggedOnUser(int phToken);
        [DllImport("Advapi32.dll")]
        public static extern int RevertToSelf();
        /// <summary>
        /// 使用域用户登录
        /// </summary>
        /// <param name="lpszUsername">域用户名</param>
        /// <param name="lpszDomain">域名</param>
        /// <param name="lpszPassword">登录密码</param>
        /// <returns>true登录成功, fasle登录失败</returns>
        public static bool VirtualLogOn(string lpszUsername = "OaAdmin", string lpszDomain = "NLIS", string lpszPassword = "qazwsx")
        {
            bool result = false;
            int hToken = 0;
            int intRet = LogonUser(lpszUsername, lpszDomain, lpszPassword, 2, 0, ref hToken);
            intRet = ImpersonateLoggedOnUser(hToken);
            if (intRet == 0)
            {
                VirtualLogOff();
                return result;
            }
            result = true;
            return result;
        }
        /// <summary>
        /// 登出域用户
        /// </summary>
        public static void VirtualLogOff()
        {
            RevertToSelf();
        }

        #endregion


        #region 文件占用检查

        /// <summary>
        /// 判断文件是否被占用
        /// </summary>
        /// <param name="lpPathName">文件路径</param>
        /// <param name="iReadWriter"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern IntPtr _lopen(string lpPathName, int iReadWriter);

        public static bool IsFileUsing(string fileFullPath)
        {
            const int OF_READWRITE = 2;
            const int OP_SHARE_DENY_NONE = 0X40;
            IntPtr hfileError = new IntPtr(-1);

            IntPtr vHandle = _lopen(fileFullPath, OF_READWRITE | OP_SHARE_DENY_NONE);

            return vHandle == hfileError;
        }

        #endregion
        public static void ToExcel(this IEnumerable list, string userName, string title, string filterflag, string fileFullName)
        {
            Workbook excelBook = new Workbook();

            excelBook = WorkbookStyle(excelBook, userName, title);

            Worksheet sheet = excelBook.Worksheets.Add(DateTime.Now.ToString("yyyy-MM-dd"));
            WorksheetRow row;
            WorksheetCell cell;

            IEnumerable<string> hiddenProperties = null;

            WorksheetRow headRow = sheet.Table.Rows.Add();
            headRow.AutoFitHeight = false;
            headRow.Height = 30;
            WorksheetCell headCell = headRow.Cells.Add(title);
            headCell.StyleID = "TitleStyle";

            int rowCount = 1;
            int cellCount = 0;

            foreach (var item in list)
            {
                row = sheet.Table.Rows.Add();
                row.AutoFitHeight = false;

                var properties = item.GetType().GetProperties().ToList();
                if (rowCount == 1)
                {
                    hiddenProperties = properties.Where(p => p.GetValue(item, null).ToString() == filterflag).Select(p => p.Name);
                    cellCount = properties.Count - hiddenProperties.Count() - 1;
                }

                foreach (var p in properties)
                {
                    if (hiddenProperties != null && hiddenProperties.Contains(p.Name))
                    {
                        continue;
                    }
                    var value = p.GetValue(item, null);
                    cell = row.Cells.Add(value == null ? string.Empty : value.ToString().Replace("<b>", "").Replace("</b>", ""), DataType.String, "DefaultStyle");

                    if (value != null && value.ToString().IndexOf("<b>") >= 0) cell.StyleID = "HeaderStyle"; //如果列表内容有加粗标志<b> 则导出也相应加粗

                    if (rowCount == 1) cell.StyleID = "HeaderStyle";//第一行 表头字体加粗
                }
                rowCount++;
            }

            headCell.MergeAcross = cellCount;

            using (FileStream ms = new FileStream(fileFullName, FileMode.Create))
            {
                excelBook.Save(ms);
            }
        }
        static Workbook WorkbookStyle(Workbook book, string UserName, string fileName)
        {
            book.ExcelWorkbook.ActiveSheetIndex = 1;
            book.Properties.Author = UserName;
            book.Properties.Title = fileName;

            book.Properties.Created = DateTime.Now;

            WorksheetStyle style = book.Styles.Add("TitleStyle");
            style.Font.FontName = "黑体";
            style.Font.Size = 22;
            style.Font.Bold = true;
            style.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            style.Alignment.Vertical = StyleVerticalAlignment.Center;

            style = book.Styles.Add("HeaderStyle");
            style.Font.FontName = "宋体";
            style.Font.Size = 10;
            style.Font.Bold = true;
            style.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            style.Alignment.Vertical = StyleVerticalAlignment.Center;
            style.Alignment.WrapText = true;
            style.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1);
            style.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 1);
            style.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1);
            style.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1);

            style = book.Styles.Add("DefaultStyle");
            style.Font.FontName = "宋体";
            style.Font.Size = 10;
            style.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            style.Alignment.Vertical = StyleVerticalAlignment.Center;
            style.Alignment.WrapText = true;
            style.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1);
            style.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 1);
            style.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1);
            style.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1);

            style = book.Styles.Add("LeftStyle");
            style.Font.FontName = "宋体";
            style.Font.Size = 10;
            style.Alignment.Horizontal = StyleHorizontalAlignment.Left;
            style.Alignment.Vertical = StyleVerticalAlignment.Center;
            style.Alignment.WrapText = true;
            style.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1);
            style.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 1);
            style.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1);
            style.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1);

            style = book.Styles.Add("RightStyle");
            style.Font.FontName = "宋体";
            style.Font.Size = 10;
            style.Alignment.Horizontal = StyleHorizontalAlignment.Right;
            style.Alignment.Vertical = StyleVerticalAlignment.Center;
            style.Alignment.WrapText = true;
            style.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1);
            style.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 1);
            style.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1);
            style.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1);

            style = book.Styles.Add("LeftStyleNoBorder");
            style.Font.FontName = "宋体";
            style.Font.Size = 10;
            style.Alignment.Horizontal = StyleHorizontalAlignment.Left;
            style.Alignment.Vertical = StyleVerticalAlignment.Center;
            style.Alignment.WrapText = true;

            style = book.Styles.Add("RightStyleNoBorder");
            style.Font.FontName = "宋体";
            style.Font.Size = 10;
            style.Alignment.Horizontal = StyleHorizontalAlignment.Right;
            style.Alignment.Vertical = StyleVerticalAlignment.Center;
            style.Alignment.WrapText = true;

            return book;
        }


    }
}
