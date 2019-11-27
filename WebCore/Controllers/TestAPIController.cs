using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RoadFlow.Business;
using RoadFlow.Utility.Cache;
using RoadFlow.Mapper;
using RoadFlow.Model.FlowRunModel;
using RoadFlow.Utility;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebCore.Controllers
{




    #region 新的方法 2.8.3

    public class TestAPIController : Controller
    {


        public string ConnType = new DbConnection().Get(new Guid("a7b0e502-7a72-4720-bccc-2c17d2ddf1dd")).ConnType;
        public string ConnString = new DbConnection().Get(new Guid("a7b0e502-7a72-4720-bccc-2c17d2ddf1dd")).ConnString;





        
        [HttpPost, HttpGet]
        public IActionResult QuartzIndex()
        {
            return Json(new { status = true, msg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") });
        }

        public string QuartzIndex1()
        {
            return  DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }



        public IActionResult Index()
        {

      
            return this.View();
        }


        public IActionResult Decrypt()
        {
            

          

            return this.View();
        }


        public IActionResult SqlView()
        {
            string ic = "FJ059510070000000024";
            StringBuilder sbs = new StringBuilder();
            RoadFlow.Model.Program program_ZDXX = new RoadFlow.Business.Program() .Get("bbc06e0f-90ec-42ab-85d5-d992d936b601".ToGuid());
            DataTable dataTable_ZDXX = new DataTable();

            
            string sql_ZDXX = program_ZDXX.SqlString.ToString();
            using (DataContext context = new DataContext(ConnType, ConnString, true))
            {
                //添加参数
                object[] objArray1 = new object[] { ic };
                dataTable_ZDXX = context.GetDataTable(sql_ZDXX, objArray1);
            }
            sbs.Append(DataTableToXML(dataTable_ZDXX, program_ZDXX.ExportHeaderText));
            base.ViewData["sql"] = sbs.ToString();

            return this.View();
        }



        public string DataTableToXML(DataTable dataTable, string table)
        {
            StringBuilder sb = new StringBuilder();
            if (dataTable.Rows.Count > 0)
            {

                foreach (DataRow row in dataTable.Rows)
                {
                    sb.Append("<" + table + ">");
                    for (int i = 0; i < dataTable.Columns.Count; i++)
                    {
                        sb.Append("<" + dataTable.Columns[i].ColumnName + ">" + row[dataTable.Columns[i].ColumnName] + "</" + dataTable.Columns[i].ColumnName + ">");
                    }
                    sb.Append("</" + table + ">");
                }

            }
            return sb.ToString();
        }



        #region 查询语句测试

        /// <summary>
        /// 患者列表
        /// </summary>
        /// <returns></returns>
        [Validate]
        public IActionResult FindSql()
        {


            //Url查询获取
            base.ViewData["appId"] = base.Request.Querys("appid");
            base.ViewData["tabId"] = base.Request.Querys("tabid");
            string[] textArray1 = new string[] { "appid=", base.Request.Querys("appid"), "&tabid=", base.Request.Querys("tabid") };
            base.ViewData["query"] = string.Concat((string[])textArray1);
            return View();
        }

        /// <summary>
        /// 查询方法
        /// </summary>
        /// <returns></returns>
        [Validate]
        public string Query()
        {
            //**************查询返回总条数***************
            int num3;

            //************排序设置方法*******************

            //---需修改的地方---
            string orderstr = "d_input_time";   //当排序值为空，按照该值进行排序


            //排序字段抓取
            string str1 = base.Request.Forms("sidx");
            //排序方式抓取
            string str2 = base.Request.Forms("sord");
            int pageSize = Tools.GetPageSize();
            int pageNumber = Tools.GetPageNumber();

            bool flag = "asc".EqualsIgnoreCase(str2);
            //排序字符串结果
            string order = (str1.IsNullOrEmpty() ? orderstr : str1) + " " + (str2.IsNullOrEmpty() ? "DESC" : str2);


            //*************字段抓取从str3开始*****************************
            Dictionary<ValueTuple<string, string>, datatype> dics = new Dictionary<ValueTuple<string, string>, datatype>();

            //---需修改的地方---
            //前端获取字段的值，最好方式是字段名称与值一样的名称
            //卡号
            string KH = base.Request.Forms("KH");
            //姓名
            string XM = base.Request.Forms("XM");
            string YS = base.Request.Forms("YS");
            string StartDate = base.Request.Forms("StartDate");
            string EndDate = base.Request.Forms("EndDate");



            //  string s_name = base.Request.Forms("s_name");

            //---需修改的地方---
            //对应字段放置于字典中

            dics.Add(new ValueTuple<string, string>("s_ic_no", KH), datatype.stringType);
            dics.Add(new ValueTuple<string, string>("s_name", XM), datatype.stringType);
            dics.Add(new ValueTuple<string, string>("s_doctor_input", YS), datatype.stringType);
            dics.Add(new ValueTuple<string, string>("d_input_time", StartDate), datatype.dataStartType);
            dics.Add(new ValueTuple<string, string>("ttt.d_input_time", EndDate), datatype.dataEndType);
            // dics.Add(new ValueTuple<string, string>("s_name", s_name), datatype.stringType);


            //******************表所有的sql语句*************************
            //数据表名称
            //---需修改的地方---
            string tablename = $@" select * from 

(select  top 100  a.s_ic_no ,a.s_name,a.s_sex , a.n_age , CONVERT(varchar(100), a.d_birthday, 23) d_birthday , a.s_address_resident, b.d_input_time,b.s_doctor_input   from HIS_patient_account a  inner join   HIS_patient_out_case b on a.s_ic_no=b.s_ic_no
)
ttt  ";


            // string allsql = " SELECT* FROM  " + tablename;

            //**********************数据连接类型与连接字符串***************************


            //**************抓取数据转tableTemplate该方法的转变*******************
            DataTable table = GetPagerTemplate.GetWherePagerList(out num3, pageSize, pageNumber, dics, tablename, order, ConnType, ConnString);


            //*******************************前端处理的对象*******************************
            JArray array = new JArray();
            foreach (DataRow row in table.Rows)
            {
                //****************标题********************


                //---需修改的地方---
                //对应的数据添加到对应的对象中
                JObject obj1 = new JObject();


                obj1.Add("id", (JToken)row["s_ic_no"].ToString());

                string KH5 = "<a class=\"list\" href=\"javascript:void(0);\" onclick=\"edit('" + row["s_ic_no"].ToString() + "');return false;\">" + row["s_ic_no"].ToString() + "</a>";

                obj1.Add("s_ic_no", (JToken)KH5);



                obj1.Add("s_name", (JToken)row["s_name"].ToString());

                obj1.Add("d_input_time", (JToken)DateTimeExtensions.ToDateTimeString(StringExtensions.ToDateTime(row["d_input_time"].ToString())));

                obj1.Add("s_sex", (JToken)row["s_sex"].ToString());
                obj1.Add("n_age", (JToken)row["n_age"].ToString());




                obj1.Add("d_birthday", (JToken)DateTimeExtensions.ToDateTimeString(StringExtensions.ToDateTime(row["d_birthday"].ToString())));

                obj1.Add("s_doctor_input", (JToken)row["s_doctor_input"].ToString());
                obj1.Add("s_address_resident", (JToken)row["s_address_resident"].ToString());



                //Opation的值最好是主键，方便通过该值调用相关数据
                obj1.Add("Opation", (JToken)("<a class=\"list\" href=\"javascript:void(0);\" onclick=\"edit('" + row["s_ic_no"].ToString() + "');return false;\"><i class=\"fa fa-edit (alias)\"></i>查看</a>"));
                JObject obj2 = obj1;
                array.Add(obj2);
            }

            //***************************返回结果传递给前端******************************
            object[] objArray1 = new object[] { "{\"userdata\":{\"total\":", (int)num3, ",\"pagesize\":", (int)pageSize, ",\"pagenumber\":", (int)pageNumber, "},\"rows\":", array.ToString(), "}" };
            return string.Concat((object[])objArray1);
        }




        #endregion

        public IActionResult print()
        {
            return this.View();
        }





        /// <summary>
        /// 加密
        /// </summary>
        /// <returns></returns>
        public string DesEncrypt1()
        {
            string str3 = base.Request.Forms("s_name");
             return  Encryption.DesEncrypt(str3);
           // return RSACrypt.Encrypt(str3);
        }


        /// <summary>
        /// 解密
        /// </summary>
        /// <returns></returns>
        public string DesDecrypt1()
        {
            string str3 = base.Request.Forms("s_glide");
           return Encryption.DesDecrypt(str3);
          //  return RSACrypt.Decrypt(str3);
        }

        public string FuncTry()
        {
            Func<string, string> FuncTry1 = (str)=>
            {
                str += "str5";
                return str;
            };
            string str1 = FuncTry1("我是中国！");
            return str1 + "不是";
        }




    }



    #endregion


    #region 加解密

    /// <summary>
    ///Encryption 的摘要说明
    /// </summary>
    public class Encryption
    {
        public Encryption()
        {
            //
            //TODO: 在此处添加构造函数逻辑
            //
        }
        private static string encryptKey = "xd!@E$fGs$i#d3E5%)8-5(5*er34/WaR";
        //默认密钥向量
        private static byte[] Keys = { 0x41, 0x72, 0x65, 0x79, 0x6F, 0x75, 0x6D, 0x79, 0x53, 0x6E, 0x6F, 0x77, 0x6D, 0x61, 0x6E, 0x3F };
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="encryptString"></param>
        /// <returns></returns>
        public static string DesEncrypt(string encryptString)
        {
            if (string.IsNullOrEmpty(encryptString))
                return string.Empty;
            RijndaelManaged rijndaelProvider = new RijndaelManaged();
            rijndaelProvider.Key = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 32));
            rijndaelProvider.IV = Keys;
            ICryptoTransform rijndaelEncrypt = rijndaelProvider.CreateEncryptor();

            byte[] inputData = Encoding.UTF8.GetBytes(encryptString);
            byte[] encryptedData = rijndaelEncrypt.TransformFinalBlock(inputData, 0, inputData.Length);

            return Convert.ToBase64String(encryptedData);
        }
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="decryptString"></param>
        /// <returns></returns>
        public static string DesDecrypt(string decryptString)
        {
            if (string.IsNullOrEmpty(decryptString))
                return string.Empty;
            try
            {
                RijndaelManaged rijndaelProvider = new RijndaelManaged();
                rijndaelProvider.Key = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 32));
                rijndaelProvider.IV = Keys;
                ICryptoTransform rijndaelDecrypt = rijndaelProvider.CreateDecryptor();

                byte[] inputData = Convert.FromBase64String(decryptString);
                byte[] decryptedData = rijndaelDecrypt.TransformFinalBlock(inputData, 0, inputData.Length);

                return Encoding.UTF8.GetString(decryptedData);
            }
            catch
            {
                return "";
            }

        }
    }

    #endregion


    #region 移动护理加解密工具
    public class RSACrypt
    {
        // Fields
        private int _ciphertextSize = 0x80;
        private Encoding _currEncoding = Encoding.UTF8;
        private RSACryptoServiceProvider _myRSA = null;
        private int _plaintextSize = 0x75;

        // Methods
        public RSACrypt(string keystring)
        {
            RSACryptoServiceProvider.UseMachineKeyStore = true;
            CspParameters parameters = new CspParameters
            {
                Flags = CspProviderFlags.UseMachineKeyStore
            };
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider(parameters);
            this._myRSA = new RSACryptoServiceProvider();
            if (keystring.StartsWith("<"))
            {
                this._myRSA.FromXmlString(keystring);
            }
            else
            {
                this._myRSA.ImportCspBlob(Convert.FromBase64String(keystring));
            }
            this._ciphertextSize = this._myRSA.KeySize / 8;
            this._plaintextSize = this._ciphertextSize - 11;
        }

        public  string Decrypt(string input)
        {
            if (this._myRSA.PublicOnly)
            {
                throw new Exception("输入的Key是公钥，无法进行解密");
            }
            byte[] buffer = Convert.FromBase64String(input);
            byte[] buffer2 = null;
            if (buffer.Length > this._ciphertextSize)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    using (MemoryStream stream2 = new MemoryStream(buffer))
                    {
                        byte[] buffer3 = new byte[this._ciphertextSize];
                        int count = ((stream2.ReadByte() | (stream2.ReadByte() << 8)) | (stream2.ReadByte() << 0x10)) | (stream2.ReadByte() << 0x18);
                        while (stream2.Position < stream2.Length)
                        {
                            stream2.Read(buffer3, 0, this._ciphertextSize);
                            byte[] buffer4 = this._myRSA.Decrypt(buffer3, false);
                            stream.Write(buffer4, 0, buffer4.Length);
                        }
                        buffer2 = new byte[count];
                        stream.Seek(0L, SeekOrigin.Begin);
                        stream.Read(buffer2, 0, count);
                    }
                }
            }
            else
            {
                buffer2 = this._myRSA.Decrypt(buffer, false);
            }
            return this._currEncoding.GetString(buffer2);
        }

        public  string Encrypt(string input)
        {
            byte[] bytes = this._currEncoding.GetBytes(input);
            byte[] inArray = null;
            if (input.Length > this._plaintextSize)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    using (MemoryStream stream2 = new MemoryStream(bytes))
                    {
                        stream.WriteByte((byte)input.Length);
                        stream.WriteByte((byte)(input.Length >> 8));
                        stream.WriteByte((byte)(input.Length >> 0x10));
                        stream.WriteByte((byte)(input.Length >> 0x18));
                        byte[] buffer = new byte[this._plaintextSize];
                        while (stream2.Position < stream2.Length)
                        {
                            stream2.Read(buffer, 0, this._plaintextSize);
                            byte[] buffer4 = this._myRSA.Encrypt(buffer, false);
                            stream.Write(buffer4, 0, buffer4.Length);
                        }
                        inArray = stream.ToArray();
                    }
                }
            }
            else
            {
                inArray = this._myRSA.Encrypt(bytes, false);
            }
            return Convert.ToBase64String(inArray);
        }

        // Properties
        public Encoding CurrentEncoding
        {
            get
            {
                return this._currEncoding;
            }
            set
            {
                this._currEncoding = value;
            }
        }

        public bool PublicOnly
        {
            get
            {
                return this._myRSA.PublicOnly;
            }
        }
    }





    #endregion
}
