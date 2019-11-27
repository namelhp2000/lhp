using System;
using System.Text;
using System.Threading;

namespace RoadFlow.Utility
{
    /// <summary>
    /// 工具进度条
    /// </summary>
    public class UtilsProgressBar
    {
        // Fields
        private static DateTime dateTime_0 = DateTime.Now;
        private static int int_0 = 0;
        private static string string_0 = "记录";
        private static string string_1 = "条";

        /// <summary>
        /// 进度条结束
        /// </summary>
        /// <param name="isDie"> 是否关掉</param>
        /// <param name="jsAction">js运行脚本</param>
        public static void End(bool isDie = true, string jsAction = "history.back()")
        {
            TimeSpan span = (TimeSpan)(DateTime.Now - dateTime_0);
            W("<script>");
            W("img2.width=400;");
            W("txt2.innerHTML='更新进度:100%';");
            object[] args = new object[] { int_0, span.TotalSeconds.ToString("f2"), string_1 + string_0, jsAction };
            W(string.Format("txt3.innerHTML=\"总共更新了 <font color=red><b>{0}</b></font> {2},总费时:<font color=red>{1}</font> 秒<br><br><input name='button1' type='button' class='button' onclick='javascript:{3};' class='button' value=' 返 回 '>\";", args));
            W("</script>");
            if (isDie)
            {
                //结束流
                Die("");
            }
            else
            {
                Thread.Sleep(0xbb8);
            }
        }

        /// <summary>
        /// 进度过程
        /// </summary>
        /// <param name="NowNum">当前数量</param>
        public static void Go(int NowNum)
        {
            if ((NowNum % 100) == 0)
            {
                Thread.Sleep(200);
            }
            W("<script>");
            W(string.Format("img2.width={0};", (((double)NowNum) / ((double)int_0)) * 400.0));
            double num = (((double)NowNum) / ((double)int_0)) * 100.0;
            W(string.Format("txt2.innerHTML=\"更新进度:{0}\";", num.ToString("f2")));
            W(string.Format("txt3.innerHTML=\"总共需要更新 <font color=red><b>{0}</b></font> {2},<font color=red><b>在此过程中请勿刷新此页面！！！</b></font> 系统正在更新第 <font color=red><b>{1}</b></font> {2}\";", int_0, NowNum, string_1 + string_0));
            W("</script>");
            Flush();
        }

        /// <summary>
        /// 进度条开始
        /// </summary>
        /// <param name="tips"></param>
        /// <param name="totalNum"></param>
        /// <param name="itemName"></param>
        /// <param name="itemUnit"></param>
        /// <param name="isFirstTask"></param>
        public static void Start(string tips, int totalNum = 0, string itemName = "记录", string itemUnit = "条", bool isFirstTask = true)
        {
            int_0 = totalNum;
            dateTime_0 = DateTime.Now;
            string_0 = itemName;
            string_1 = itemUnit;
            if (isFirstTask)
            {
                W(string.Format("\r\n                    <!DOCTYPE html><html>\r\n                    <head>\r\n<link href='/admin/images/style.CSS' rel='stylesheet' type='text/css' />\r\n                    </head>\r\n                  <body scroll=no bgcolor='transparent'>\r\n                   <div id='manage_top' class='toptitle menu_top_fixed'>{0}</div><div class=\"menu_top_fixed_height\"></div> \r\n", tips) + "<div class=\"content-area\"><br><br>" + "<table id=\"BarShowArea\" width=\"400\" border=\"0\" align=\"center\" cellspacing=\"1\" cellpadding=\"1\">\r\n\t\t\t\t    <tr>\r\n\t\t\t\t    <td bgcolor=000000>\r\n\t\t\t\t     <table width=\"400\" border=\"0\" cellspacing=\"0\" cellpadding=\"1\">\r\n\t\t\t\t    <tr>\r\n\t\t\t\t    <td bgcolor=ffffff height=9 style=\"text-align:left\"><img src=\"/sysimg/default/bar.gif\" width=0 height=10 id=img2 name=img2 align=absmiddle></td></tr></table>\r\n\t\t\t\t    <td bgcolor=ffffff height=9><span width=0 height=16 id=Span1 name=img2 align=absmiddle bgcolor='#000000'></span></td></tr></table>\r\n\t\t\t\t    </td></tr></table>\r\n\t\t\t\t    <table width=\"550\" border=\"0\" align=\"center\" cellspacing=\"1\" cellpadding=\"1\" style=\"margin:20px auto;\"><tr>\r\n\t\t\t\t    <td align=center> <span id=txt2 name=txt2 style=\"font-size:9pt\">0</span><span id=txt4 style=\"font-size:9pt\">%</span></td></tr>\r\n\t\t\t\t    <tr><td align=center><span id=txt3 name=txt3 style=\"font-size:9pt\">0</span></td></tr>\r\n\t\t\t\t    </table>\r\n\r\n           </body>\r\n         </htmL>\r\n              ");
            }
        }



        /// <summary>
        /// 写入流到前端
        /// </summary>
        /// <param name="str"></param>
        public static void W(string str)
        {
            str = replaceuid(str);
            Tools.HttpContext.Response.Body.Write(Encoding.ASCII.GetBytes(str), 0, Encoding.ASCII.GetBytes(str).Length);
        }

        /// <summary>
        /// 释放流
        /// </summary>
        public static void Flush()
        {
            Tools.HttpContext.Response.Body.Flush();
        }


        public static void Die(string str)
        {
            W(str);
            E();
        }


        /// <summary>
        /// 结束流
        /// </summary>
        public static void E()
        {
            Tools.HttpContext.Response.Body.Close();
        }














        /// <summary>
        /// uid替换到对应的值
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string replaceuid(string content)
        {
            if (content.IndexOf("{uid}") != -1)
            {
                //  return content.Replace("{uid}", new TemporaryVar().AgentUID.ToString());
            }
            return content;
        }








    }
}
