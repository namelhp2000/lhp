using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using NPOI.XWPF.UserModel;
using System;
using System.Data;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace RoadFlow.Utility
{



    #region  新的方法

    public class NPOIHelper
    {
        // Methods
        /// <summary>
        /// 导出功能返回工作簿
        /// </summary>
        /// <param name="dtSource">数据源表</param>
        /// <param name="strHeaderText">标题文本</param>
        /// <param name="templateFile">模板文件</param>
        /// <param name="fileName">文件名称</param>
        /// <returns></returns>
        public static IWorkbook Export(DataTable dtSource, string strHeaderText, string templateFile = "", string fileName = "")
        {
            IWorkbook workbook = null;
            ISheet sheetAt = null;
            //工作表标识
            bool flag = false;
            int num = 0;
            //模板标识
            bool flag2 = false;
            //模板路径不为空同时存在
            if (!templateFile.IsNullOrEmpty() && File.Exists(templateFile))
            {
                //判断后缀名称是否xlsx
                flag2 = templateFile.EndsWith("xlsx", (StringComparison)StringComparison.CurrentCultureIgnoreCase);
                using (FileStream stream = new FileStream(templateFile, (FileMode)FileMode.Open, (FileAccess)FileAccess.Read))
                {
                    if (flag2)
                    {
                        workbook = new XSSFWorkbook((Stream)stream);
                    }
                    else
                    {
                        workbook = new HSSFWorkbook((Stream)stream);
                    }
                }
                if (workbook != null) //工作簿不为空
                {
                    //获取工作表
                    sheetAt = workbook.GetSheetAt(0);
                    if (sheetAt != null)
                    {
                        flag = true;
                        //获取表最后一行
                        num = sheetAt.LastRowNum + 1;
                    }
                }
            }
            else
            {
                flag2 = fileName.EndsWith("xlsx", (StringComparison)StringComparison.CurrentCultureIgnoreCase);
            }
            if (workbook == null)
            {
                if (flag2)
                {
                    workbook = new XSSFWorkbook();
                }
                else
                {
                    workbook = new HSSFWorkbook();
                }
            }
            if (sheetAt == null) //表为空，新建表格
            {
                sheetAt = workbook.CreateSheet("Sheet1");
            }
            int[] numArray = new int[0];
            string[] strArray = new string[0];
            if (!flag)
            {
                //数据源表行数
                numArray = new int[dtSource.Columns.Count];
                //数据源表字符串
                strArray = new string[dtSource.Columns.Count];
                foreach (DataColumn column in dtSource.Columns)
                {
                    if (column.Caption.IsInt())
                    {
                        numArray[column.Ordinal] = column.Caption.ToInt(-2147483648);
                    }
                    else
                    {
                        numArray[column.Ordinal] = Encoding.Default.GetBytes(column.ColumnName.ToString()).Length;
                    }
                    strArray[column.Ordinal] = column.DataType.ToString();
                }
                if (dtSource.Rows.Count > 0)
                {
                    for (int i = 0; i < dtSource.Columns.Count; i++)
                    {
                        if (!dtSource.Columns[i].Caption.IsInt())
                        {
                            int length = Encoding.Default.GetBytes(dtSource.Rows[0][i].ToString()).Length;
                            if (length > numArray[i])
                            {
                                numArray[i] = length;
                            }
                        }
                    }
                }
            }
            //标题文本标识
            bool flag3 = !strHeaderText.IsNullOrEmpty();
            if (!flag && ((num == 0xffff) || (num == 0)))
            {
                if (flag3)
                {
                    IRow row1 = sheetAt.CreateRow(0);
                    row1.HeightInPoints = 25f;
                    row1.CreateCell(0).SetCellValue(strHeaderText);
                    ICellStyle style2 = workbook.CreateCellStyle();
                    style2.Alignment = HorizontalAlignment.Center;
                    IFont font = workbook.CreateFont();
                    font.FontHeightInPoints = 20;
                    font.Boldweight = 700;
                    style2.SetFont(font);
                    style2.BorderLeft = BorderStyle.Thin;
                    style2.BorderRight = BorderStyle.Thin;
                    style2.BorderTop = BorderStyle.Thin;
                    style2.BorderBottom = BorderStyle.Thin;
                    row1.GetCell(0).CellStyle = style2;
                    sheetAt.AddMergedRegion(new CellRangeAddress(0, 0, 0, dtSource.Columns.Count - 1));
                }
                IRow row = sheetAt.CreateRow(flag3 ? 1 : 0);
                ICellStyle style3 = workbook.CreateCellStyle();
                style3.Alignment = HorizontalAlignment.Center;
                IFont font2 = workbook.CreateFont();
                font2.FontHeightInPoints = 10;
                font2.Boldweight = 700;
                style3.BorderLeft = BorderStyle.Thin;
                style3.BorderRight = BorderStyle.Thin;
                style3.BorderTop = BorderStyle.Thin;
                style3.BorderBottom = BorderStyle.Thin;
                style3.IsLocked = true;
                style3.SetFont(font2);
                workbook.CreateCellStyle();
                foreach (DataColumn column2 in dtSource.Columns)
                {
                    row.CreateCell(column2.Ordinal).SetCellValue(column2.ColumnName);
                    row.GetCell(column2.Ordinal).CellStyle = style3;
                    if (numArray.Length != 0)
                    {
                        sheetAt.SetColumnWidth(column2.Ordinal, (numArray[column2.Ordinal] + 1) * 0x100);
                    }
                }
                num = flag3 ? 2 : 1;
            }
            int count = dtSource.Columns.Count;
            ICellStyle style = workbook.CreateCellStyle();
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderTop = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.Thin;
            foreach (DataRow row2 in dtSource.Rows)
            {
                IRow row3 = sheetAt.CreateRow(num);
                for (int j = 0; j < count; j++)
                {
                    DateTime time;
                    bool flag4;
                    int num7;
                    double num8;
                    NPOI.SS.UserModel.ICell cell = row3.CreateCell(j);
                    cell.CellStyle = style;
                    string str = row2[j].ToString();
                    string s = (strArray.Length > j) ? strArray[j] : "System.String";
                    uint num6 = PrivateImplementationDetails.ComputeStringHash(s);
                    if (num6 <= 0x6532296c)
                    {
                        switch (num6)
                        {
                            case 0x328ee55b:
                                if (s == "System.Double")
                                {
                                    goto Label_0630;
                                }
                                break;

                            case 0x5be1dd63:
                                if (s == "System.DateTime")
                                {
                                    goto Label_05EA;
                                }
                                break;

                            case 0x6532296c:
                                if (s == "System.Int16")
                                {
                                    goto Label_0617;
                                }
                                break;

                            case 0x14b01c5e:
                                goto Label_0538;

                            case 0x1faaa7d9:
                                goto Label_05C9;
                        }
                    }
                    else if (num6 <= 0x692563c5)
                    {
                        switch (num6)
                        {
                            case 0x67c7c205:
                                goto Label_05A3;

                            case 0x692563c5:
                                goto Label_057A;
                        }
                    }
                    else
                    {
                        switch (num6)
                        {
                            case 0xb79438bc:
                                if (s == "System.Byte")
                                {
                                    goto Label_0617;
                                }
                                goto Label_065E;

                            case 0xf92d023a:
                                if (s == "System.Int32")
                                {
                                    goto Label_0617;
                                }
                                goto Label_065E;
                        }
                        if ((num6 == 0xfa6bbba7) && (s == "System.String"))
                        {
                            goto Label_05DC;
                        }
                    }
                    goto Label_065E;
                Label_0538:
                    if (s == "System.Boolean")
                    {
                        goto Label_05FF;
                    }
                    goto Label_065E;
                Label_057A:
                    if (s == "System.Int64")
                    {
                        goto Label_0617;
                    }
                    goto Label_065E;
                Label_05A3:
                    if (s == "System.Decimal")
                    {
                        goto Label_0630;
                    }
                    goto Label_065E;
                Label_05C9:
                    if (s == "System.DBNull")
                    {
                        goto Label_0650;
                    }
                    goto Label_065E;
                Label_05DC:
                    cell.SetCellValue(str);
                    continue;
                Label_05EA:
                    DateTime.TryParse(str, out time);
                    cell.SetCellValue(time);
                    continue;
                Label_05FF:
                    flag4 = false;
                    bool.TryParse(str, out flag4);
                    cell.SetCellValue(flag4);
                    continue;
                Label_0617:
                    num7 = 0;
                    int.TryParse(str, out num7);
                    cell.SetCellValue((double)num7);
                    continue;
                Label_0630:
                    num8 = 0.0;
                    double.TryParse(str, out num8);
                    cell.SetCellValue(num8);
                    continue;
                Label_0650:
                    cell.SetCellValue("");
                    continue;
                Label_065E:
                    cell.SetCellValue("");
                }
                num++;
            }
            return workbook;
        }


        /// <summary>
        /// 通过web导出
        /// </summary>
        /// <param name="dtSource">数据源表</param>
        /// <param name="strHeaderText">标题文本</param>
        /// <param name="strFileName">文件名称</param>
        /// <param name="response">网页响应</param>
        /// <param name="templateFile">模板文件路径</param>
        public static void ExportByWeb(DataTable dtSource, string strHeaderText, string strFileName, HttpResponse response = null, string templateFile = "")
        {
            string str = strFileName.UrlEncode();
            HttpResponse response2 = response ?? Tools.HttpContext.Response;
            ResponseExtensions.Clear(response2);
            response2.Headers.Add("Server-FileName", (StringValues)str);
            response2.ContentType = "application/octet-stream";
            response2.Headers.Add("Content-Disposition", (StringValues)("attachment;filename=" + str));
            IWorkbook workbook1 = Export(dtSource, strHeaderText, templateFile, strFileName);
            workbook1.Write(response2.Body);
            workbook1.Close();
            response2.Body.Flush();
            response2.Body.Close();
        }


        /// <summary>
        /// 导出word文档
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="content"></param>
        public static void ExportToWord(string filename, string content, HttpResponse response = null)
        {
            string str = filename.UrlEncode();
            byte[] bytes = Encoding.UTF8.GetBytes(content);
            HttpResponse response2 = response ?? Tools.HttpContext.Response;
            ResponseExtensions.Clear(response2);
            response2.Headers.Add("Server-FileName", (StringValues)str);
            response2.ContentType = "application/octet-stream";
            response2.Headers.Add("Content-Disposition", (StringValues)("attachment; filename=" + str));
            int length = bytes.Length;
            response2.Headers.Add("Content-Length", (StringValues)((int)length).ToString());
            response2.Body.Write(bytes, 0, length);
            response2.Body.Flush();
            response2.Body.Close();

        }











        /// <summary>
        /// 导出CSV文件
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="strFileName">文件名</param>
        public static void ExportCSV(DataTable dt, string strFileName)
        {
            StringBuilder builder = new StringBuilder();
            //标题循环获取
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                builder.Append(dt.Columns[i].ColumnName);
                if (i < (dt.Columns.Count - 1))
                {
                    builder.Append(",");
                }
            }
            builder.Append("\n");
            //数据行循环获取
            foreach (DataRow row in dt.Rows)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    builder.Append(row[j]);
                    if (j < (dt.Columns.Count - 1))
                    {
                        builder.Append(",");
                    }
                }
                builder.Append("\n");
            }
            builder.Append("\n");
            using (FileStream stream = new FileStream(strFileName, (FileMode)FileMode.Create, (FileAccess)FileAccess.Write))
            {
                //字符串转成流
                byte[] bytes = Encoding.Default.GetBytes(builder.ToString());
                stream.Write(bytes, 0, bytes.Length);
                stream.Close();
            }
        }

        /// <summary>
        /// 导出文件
        /// </summary>
        /// <param name="dtSource">数据源表</param>
        /// <param name="strHeaderText">标题文本</param>
        /// <param name="strFileName">文件名</param>
        /// <param name="templateFile">模板文件路径</param>
        public static void ExportToFile(DataTable dtSource, string strHeaderText, string strFileName, string templateFile = "")
        {
            using (FileStream stream = new FileStream(strFileName, (FileMode)FileMode.Create, (FileAccess)FileAccess.Write))
            {
                IWorkbook workbook1 = Export(dtSource, strHeaderText, templateFile, strFileName);
                workbook1.Write((Stream)stream);
                workbook1.Close();
                stream.Flush();
            }
        }

        /// <summary>
        /// Excel转DataTable表
        /// </summary>
        /// <param name="strFileName">文件名称</param>
        /// <param name="headerRows">标题行</param>
        /// <returns></returns>
        public static DataTable ReadToDataTable(string strFileName, int headerRows = 1)
        {
            IWorkbook workbook;
            DataTable table = new DataTable();
            using (FileStream stream = new FileStream(strFileName, (FileMode)FileMode.Open, (FileAccess)FileAccess.Read))
            {
                if (Path.GetExtension(strFileName).Equals(".xlsx", (StringComparison)StringComparison.CurrentCultureIgnoreCase))
                {
                    workbook = new XSSFWorkbook((Stream)stream);
                }
                else
                {
                    workbook = new HSSFWorkbook((Stream)stream);
                }
            }
            //获取表
            ISheet sheetAt = workbook.GetSheetAt(0);
            sheetAt.GetRowEnumerator();
            IRow row = sheetAt.GetRow(0);
            int num = row.LastCellNum;
            for (int i = 0; i < num; i++)  //标题装入表中
            {
                NPOI.SS.UserModel.ICell cell = row.GetCell(i);
                table.Columns.Add(cell.ToString());
            }
            for (int j = sheetAt.FirstRowNum + headerRows; j <= sheetAt.LastRowNum; j++) //数据表装入表
            {
                IRow row2 = sheetAt.GetRow(j);
                DataRow row3 = table.NewRow();
                for (int k = row2.FirstCellNum; k < num; k++)
                {
                    if (row2.GetCell(k) != null)
                    {
                        row3[k] = row2.GetCell(k).ToString();
                    }
                }
                table.Rows.Add(row3);
            }
            return table;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="strFileName">路径</param>
        /// <param name="coloumncount">列数</param>
        /// <param name="tablenamehead">指定标题行数</param>
        /// <returns></returns>
        public static DataTable ReadToDataTable(string strFileName, int coloumncount, int headerRows)
        {
            IWorkbook workbook;
            DataTable table = new DataTable();
            using (FileStream stream = new FileStream(strFileName, (FileMode)FileMode.Open, (FileAccess)FileAccess.Read))
            {
                if (Path.GetExtension(strFileName).Equals(".xlsx", (StringComparison)StringComparison.CurrentCultureIgnoreCase))
                {
                    workbook = new XSSFWorkbook((Stream)stream);
                }
                else
                {
                    workbook = new HSSFWorkbook((Stream)stream);
                }
            }
            //获取第一表
            ISheet sheetAt = workbook.GetSheetAt(0);
            sheetAt.GetRowEnumerator();
            IRow row = sheetAt.GetRow(headerRows - 1);

            for (int i = 0; i < coloumncount; i++)  //标题装入表中
            {
                NPOI.SS.UserModel.ICell cell = row.GetCell(i);
                table.Columns.Add(cell.ToString());
            }
            for (int j = sheetAt.FirstRowNum + headerRows; j <= sheetAt.LastRowNum; j++) //数据表装入表
            {
                IRow row2 = sheetAt.GetRow(j);
                DataRow row3 = table.NewRow();
                for (int k = row2.FirstCellNum; k < coloumncount; k++)
                {
                    if (row2.GetCell(k) != null)
                    {
                        row3[k] = row2.GetCell(k).ToString();
                    }
                }
                table.Rows.Add(row3);
            }
            return table;
        }
        #region 手动设置Word模板的段落与表格段落

        /// <summary>
        /// 创建word文档中的段落对象和设置段落文本的基本样式（字体大小，字体，字体颜色，字体对齐位置）
        /// </summary>
        /// <param name="document">document文档对象</param>
        /// <param name="fillContent">段落第一个文本对象填充的内容</param>
        /// <param name="isBold">是否加粗</param>
        /// <param name="fontSize">字体大小</param>
        /// <param name="fontFamily">字体</param>
        /// <param name="paragraphAlign">段落排列（左对齐，居中，右对齐）</param>
        /// <param name="isStatement">是否在同一段落创建第二个文本对象（解决同一段落里面需要填充两个或者多个文本值的情况，多个文本需要自己拓展，现在最多支持两个）</param>
        /// <param name="secondFillContent">第二次声明的文本对象填充的内容，样式与第一次的一致</param>
        /// <returns></returns>
        public static XWPFParagraph ParagraphInstanceSetting(XWPFDocument document, string fillContent, bool isBold, int fontSize, string fontFamily, ParagraphAlignment paragraphAlign, bool isStatement = false, string secondFillContent = "")
        {
            XWPFParagraph paragraph = document.CreateParagraph();//创建段落对象
            paragraph.Alignment = paragraphAlign;//文字显示位置,段落排列（左对齐，居中，右对齐）

            XWPFRun xwpfRun = paragraph.CreateRun();//创建段落文本对象
            xwpfRun.IsBold = isBold;//文字加粗
            xwpfRun.SetText(fillContent);//填充内容
            xwpfRun.FontSize = fontSize;//设置文字大小
            xwpfRun.SetFontFamily(fontFamily, FontCharRange.None); //设置标题样式如：（微软雅黑，隶书，楷体）根据自己的需求而定

            if (isStatement)
            {
                XWPFRun secondxwpfRun = paragraph.CreateRun();//创建段落文本对象
                secondxwpfRun.IsBold = isBold;//文字加粗
                secondxwpfRun.SetText(secondFillContent);//填充内容
                secondxwpfRun.FontSize = fontSize;//设置文字大小
                secondxwpfRun.SetFontFamily(fontFamily, FontCharRange.None); //设置标题样式如：（微软雅黑，隶书，楷体）根据自己的需求而定
            }


            return paragraph;
        }

        /// <summary>  
        /// 创建Word文档中表格段落实例和设置表格段落文本的基本样式（字体大小，字体，字体颜色，字体对齐位置）
        /// </summary>  
        /// <param name="document">document文档对象</param>  
        /// <param name="table">表格对象</param>  
        /// <param name="fillContent">要填充的文字</param>  
        /// <param name="paragraphAlign">段落排列（左对齐，居中，右对齐）</param>
        /// <param name="rowsHeight">设置文本位置（设置两行之间的行间），从而实现table的高度设置效果  </param>
        /// <param name="isBold">是否加粗（true加粗，false不加粗）</param>
        /// <param name="fontSize">字体大小</param>
        /// <returns></returns>  
        public static NPOI.XWPF.UserModel.XWPFParagraph SetTableParagraphInstanceSetting(XWPFDocument document, XWPFTable table, string fillContent, ParagraphAlignment paragraphAlign, int rowsHeight, bool isBold, int fontSize = 10)
        {
            var para = new NPOI.OpenXmlFormats.Wordprocessing.CT_P();
            XWPFParagraph paragraph = new XWPFParagraph(para, table.Body);//创建表格中的段落对象
            paragraph.Alignment = paragraphAlign;//文字显示位置,段落排列（左对齐，居中，右对齐）

            XWPFRun xwpfRun = paragraph.CreateRun();//创建段落文本对象


            //xwpfRun.IsBold = isBold;//文字加粗
            //xwpfRun.SetText(fillContent);//填充内容
            //xwpfRun.FontSize = fontSize;//设置文字大小    
            //xwpfRun.SetFontFamily("fontFamily", FontCharRange.None); //设置标题样式如：（微软雅黑，隶书，楷体）根据自己的需求而定
            //xwpfRun.SetColor("BED4F1");//设置字体颜色--十六进制
            //xwpfRun.IsDoubleStrikeThrough = true;//是否显示双删除线
            //xwpfRun.IsStrikeThrough = true;//是否显示单删除线
            //xwpfRun.SetUnderline(UnderlinePatterns.Dash);//设置下划线，枚举类型
            //xwpfRun.SetTextPosition(20);//设置文本位置（设置两行之间的行间）
            //xwpfRun.AddBreak();//设置换行（</br>）
            //xwpfRun.AddTab();//添加tab键
            //xwpfRun.AddCarriageReturn();//添加回车键
            //xwpfRun.IsImprinted = true;//印迹（悬浮阴影）,效果和浮雕类似
            //xwpfRun.IsItalic = true;//是否设置斜体（字体倾斜）
            //xwpfRun.Subscript = VerticalAlign.SUBSCRIPT;//设置下标，枚举类型


            xwpfRun.SetText(fillContent);
            xwpfRun.FontSize = fontSize;//字体大小
            xwpfRun.IsBold = isBold;//是否加粗
            xwpfRun.SetFontFamily("宋体", FontCharRange.None);//设置字体（如：微软雅黑,华文楷体,宋体）
            xwpfRun.SetTextPosition(rowsHeight);//设置文本位置（设置两行之间的行间），从而实现table的高度设置效果 
            return paragraph;
        }


        #endregion

    }


    [CompilerGenerated]
    internal sealed class PrivateImplementationDetails
    {
        // Methods
        internal static uint ComputeStringHash(string s)
        {
            uint num = 0x811c9dc5;
            if (s != null)
            {
                num = 0x811c9dc5;
                for (int i = 0; i < s.Length; i++)
                {
                    num = (s[i] ^ num) * 0x1000193;
                }
            }
            return num;
        }
    }







    #endregion


}
