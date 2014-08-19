using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarlosAg.ExcelXmlWriter;

namespace GetOAInfomations
{
    public static class Helper
    {
        public static void ToExcel(this IEnumerable list, string userName, string title, string filterflag, string xlsFuulName)
        {
            Workbook excelBook = new Workbook();

            excelBook = WorkbookStyle(excelBook, userName, title);

            Worksheet sheet = excelBook.Worksheets.Add(DateTime.Now.ToString("yyyy-MM-dd"));
            WorksheetRow row;
            WorksheetCell cell;

            IEnumerable<string> hiddenProperties = null;

            WorksheetRow headRow = sheet.Table.Rows.Add();
            headRow.AutoFitHeight = false;
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

            using (FileStream fs = new FileStream(xlsFuulName, FileMode.Create))
            {
                excelBook.Save(fs);
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
