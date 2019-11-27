using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RoadFlow.Business;
using RoadFlow.Utility;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebCore.Areas.RoadFlowCore.Controllers
{




  
    [Area("RoadFlowCore")]
    public class WorkDateController : Controller
    {
        // Methods
        [Validate]
        public IActionResult Index()
        {
            DateTime time = DateTimeExtensions.Now;
            int num = base.Request.Method.EqualsIgnoreCase("post") ? base.Request.Forms("DropDownList1").ToInt(time.Year) : time.Year;
            WorkDate date = new WorkDate();
            List<RoadFlow.Model.WorkDate> yearList = date.GetYearList(num);
            StringBuilder builder = new StringBuilder();
            for (int i = date.GetMinYear(); i <= (time.Year + 1); i++)
            {
                object[] objArray1 = new object[] { "<option value='", (int)i, "'", (i == num) ? "selected='selected'" : "", ">", (int)i, "</option>" };
                builder.Append(string.Concat((object[])objArray1));
            }
            base.ViewData["year"]= (int)num;
            base.ViewData["yearOptions"]= builder.ToString();
            base.ViewData["queryString"]= base.Request.UrlQuery();
            return this.View(yearList.FindAll(x=>x.IsWork==1));
        }

        public string SaveWorkDate()
        {
            int num;
            string str = base.Request.Forms("year1");
            string str2 = "," + base.Request.Forms("workdate") + ",";
            string str3 = base.Request.Forms("daydate");
            if (!str.IsInt(out num))
            {
                return "年份错误!";
            }
            List<RoadFlow.Model.WorkDate> list = new List<RoadFlow.Model.WorkDate>();
            foreach (string str4 in str3.Split(',', (StringSplitOptions)StringSplitOptions.None))
            {
                DateTime dt;
                if (StringExtensions.IsDateTime(str4, out dt) && (dt.Year == num))
                {
                    int num3 = str2.Contains("," + str4 + ",") ? 1 : 0;
                    if (!list.Exists(delegate (RoadFlow.Model.WorkDate p) {
                        return p.WorkDay == dt;
                    }))
                    {
                        RoadFlow.Model.WorkDate date1 = new RoadFlow.Model.WorkDate();
                        date1.WorkDay=dt;
                        date1.IsWork = num3;
                        list.Add(date1);
                    }
                }
            }
            Log.Add("设置了工作日", JsonConvert.SerializeObject(list), LogType.系统管理, "", "", "", "", "", "", "", "");
            new WorkDate().Add(list.ToArray(), num);
            return "保存成功!";
        }
    }



  

}
