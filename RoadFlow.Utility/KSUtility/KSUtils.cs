using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace RoadFlow.Utility
{
    public static class KSUtils
    {




        /// <summary>
        /// 考试信息配置序列化
        /// </summary>
        /// <param name="configFilePath"></param>
        /// <returns></returns>
        public static ExamConfigInfo Deserialize(string configFilePath)
        {
            return (ExamConfigInfo)Load(typeof(ExamConfigInfo), configFilePath);
        }



        /// <summary>
        /// 加载配置文件
        /// </summary>
        /// <param name="type"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static object Load(Type type, string filename)
        {
            FileStream stream = null;
            object obj2;
            try
            {
                stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                obj2 = new XmlSerializer(type).Deserialize(stream);
            }
            catch (Exception exception1)
            {
                throw exception1;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
            return obj2;
        }

        /// <summary>
        /// 保存配置文件
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="filename"></param>
        public static void Save(object obj, string filename)
        {
            FileStream stream = null;
            try
            {
                stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                new XmlSerializer(obj.GetType()).Serialize((Stream)stream, obj);
            }
            catch (Exception exception1)
            {
                throw exception1;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
        }































    }

    /// <summary>
    /// 考试信息配置文件
    /// </summary>
    [Serializable]
    public class ExamConfigInfo
    {
        // Fields
        [CompilerGenerated]
        private float float_0;
        [CompilerGenerated]
        private float float_1;
        [CompilerGenerated]
        private int int_0;
        [CompilerGenerated]
        private int int_1;
        [CompilerGenerated]
        private int int_10;
        [CompilerGenerated]
        private int int_2;
        [CompilerGenerated]
        private int int_3;
        [CompilerGenerated]
        private int int_4;
        [CompilerGenerated]
        private int int_5;
        [CompilerGenerated]
        private int int_6;
        [CompilerGenerated]
        private int int_7;
        [CompilerGenerated]
        private int int_8;
        [CompilerGenerated]
        private int int_9;
        [CompilerGenerated]
        private string string_0;
        [CompilerGenerated]
        private string string_1;
        [CompilerGenerated]
        private string string_2;
        [CompilerGenerated]
        private string string_3;
        [CompilerGenerated]
        private string string_4;
        [CompilerGenerated]
        private string string_5;
        [CompilerGenerated]
        private string string_6;
        [CompilerGenerated]
        private string string_7;
        [CompilerGenerated]
        private string string_8;
        [CompilerGenerated]
        private string string_9;
        [CompilerGenerated]
        private string vCaOxIoEvt;

        // Properties
        public int AllowVisitorExamTF
        {
            [CompilerGenerated]
            get
            {
                return this.int_8;
            }
            [CompilerGenerated]
            set
            {
                this.int_8 = value;
            }
        }

        public float ChargeExplainMoney
        {
            [CompilerGenerated]
            get
            {
                return this.float_0;
            }
            [CompilerGenerated]
            set
            {
                this.float_0 = value;
            }
        }

        public int ChargeExplainTF
        {
            [CompilerGenerated]
            get
            {
                return this.int_10;
            }
            [CompilerGenerated]
            set
            {
                this.int_10 = value;
            }
        }

        public int ChargesInterval
        {
            [CompilerGenerated]
            get
            {
                return this.int_9;
            }
            [CompilerGenerated]
            set
            {
                this.int_9 = value;
            }
        }

        public float ChargesIntervalExplain
        {
            [CompilerGenerated]
            get
            {
                return this.float_1;
            }
            [CompilerGenerated]
            set
            {
                this.float_1 = value;
            }
        }

        public int ExamCollectTMTF
        {
            [CompilerGenerated]
            get
            {
                return this.int_4;
            }
            [CompilerGenerated]
            set
            {
                this.int_4 = value;
            }
        }

        public string ExamContentTemplate
        {
            [CompilerGenerated]
            get
            {
                return this.string_7;
            }
            [CompilerGenerated]
            set
            {
                this.string_7 = value;
            }
        }

        public string ExamDomain
        {
            [CompilerGenerated]
            get
            {
                return this.string_0;
            }
            [CompilerGenerated]
            set
            {
                this.string_0 = value;
            }
        }

        public string ExamIndexTemplate
        {
            [CompilerGenerated]
            get
            {
                return this.string_5;
            }
            [CompilerGenerated]
            set
            {
                this.string_5 = value;
            }
        }

        public string ExamInstallDir
        {
            [CompilerGenerated]
            get
            {
                return this.string_1;
            }
            [CompilerGenerated]
            set
            {
                this.string_1 = value;
            }
        }

        public int ExamKnowledgeTF
        {
            [CompilerGenerated]
            get
            {
                return this.int_1;
            }
            [CompilerGenerated]
            set
            {
                this.int_1 = value;
            }
        }

        public string ExamListTemplate
        {
            [CompilerGenerated]
            get
            {
                return this.string_6;
            }
            [CompilerGenerated]
            set
            {
                this.string_6 = value;
            }
        }

        public string ExamName
        {
            [CompilerGenerated]
            get
            {
                return this.vCaOxIoEvt;
            }
            [CompilerGenerated]
            set
            {
                this.vCaOxIoEvt = value;
            }
        }

        public int ExamProvinceTF
        {
            [CompilerGenerated]
            get
            {
                return this.int_3;
            }
            [CompilerGenerated]
            set
            {
                this.int_3 = value;
            }
        }

        public int ExamPubVerify
        {
            [CompilerGenerated]
            get
            {
                return this.int_6;
            }
            [CompilerGenerated]
            set
            {
                this.int_6 = value;
            }
        }

        public string ExamSeoDescript
        {
            [CompilerGenerated]
            get
            {
                return this.string_4;
            }
            [CompilerGenerated]
            set
            {
                this.string_4 = value;
            }
        }

        public string ExamSeoKeyWords
        {
            [CompilerGenerated]
            get
            {
                return this.string_3;
            }
            [CompilerGenerated]
            set
            {
                this.string_3 = value;
            }
        }

        public string ExamSeoTitle
        {
            [CompilerGenerated]
            get
            {
                return this.string_2;
            }
            [CompilerGenerated]
            set
            {
                this.string_2 = value;
            }
        }

        public int ExamStatus
        {
            [CompilerGenerated]
            get
            {
                return this.int_0;
            }
            [CompilerGenerated]
            set
            {
                this.int_0 = value;
            }
        }

        public int ExamTypeTF
        {
            [CompilerGenerated]
            get
            {
                return this.int_2;
            }
            [CompilerGenerated]
            set
            {
                this.int_2 = value;
            }
        }

        public int ExamVefiyModify
        {
            [CompilerGenerated]
            get
            {
                return this.int_7;
            }
            [CompilerGenerated]
            set
            {
                this.int_7 = value;
            }
        }

        public string ExportTemplate
        {
            [CompilerGenerated]
            get
            {
                return this.string_9;
            }
            [CompilerGenerated]
            set
            {
                this.string_9 = value;
            }
        }

        public int Int32_0
        {
            [CompilerGenerated]
            get
            {
                return this.int_5;
            }
            [CompilerGenerated]
            set
            {
                this.int_5 = value;
            }
        }

        public string Setting
        {
            [CompilerGenerated]
            get
            {
                return this.string_8;
            }
            [CompilerGenerated]
            set
            {
                this.string_8 = value;
            }
        }








    }









}
