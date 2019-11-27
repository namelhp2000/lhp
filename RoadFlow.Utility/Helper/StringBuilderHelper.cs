using System.Text;


namespace RoadFlow.Utility
{
    /// <summary>
    /// 字符串操作 - 工具
    /// </summary>
    public partial class StringHelper
    {

        /// <summary>
        /// 初始化字符串操作
        /// </summary>
        public StringHelper()
        {
            Builder = new StringBuilder();
        }

        /// <summary>
        /// 字符串生成器
        /// </summary>
        private StringBuilder Builder { get; set; }

        /// <summary>
        /// 追加内容
        /// </summary>
        /// <typeparam name="T">值的类型</typeparam>
        /// <param name="value">值</param>
        public StringHelper Append<T>(T value)
        {
            Builder.Append(value);
            return this;
        }

        /// <summary>
        /// 追加内容
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="args">参数</param>
        public StringHelper Append(string value, params object[] args)
        {
            if (args == null)
                args = new object[] { string.Empty };
            if (args.Length == 0)
                Builder.Append(value);
            else
                Builder.AppendFormat(value, args);
            return this;
        }

        /// <summary>
        /// 追加内容并换行
        /// </summary>
        public StringHelper AppendLine()
        {
            Builder.AppendLine();
            return this;
        }

        /// <summary>
        /// 追加内容并换行
        /// </summary>
        /// <typeparam name="T">值的类型</typeparam>
        /// <param name="value">值</param>
        public StringHelper AppendLine<T>(T value)
        {
            Append(value);
            Builder.AppendLine();
            return this;
        }

        /// <summary>
        /// 追加内容并换行
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="args">参数</param>
        public StringHelper AppendLine(string value, params object[] args)
        {
            Append(value, args);
            Builder.AppendLine();
            return this;
        }

        /// <summary>
        /// 替换内容
        /// </summary>
        /// <param name="value">值</param>
        public StringHelper Replace(string value)
        {
            Builder.Clear();
            Builder.Append(value);
            return this;
        }

        /// <summary>
        /// 移除末尾字符串
        /// </summary>
        /// <param name="end">末尾字符串</param>
        public StringHelper RemoveEnd(string end)
        {
            string result = Builder.ToString();
            if (!result.EndsWith(end))
                return this;
            Builder = new StringBuilder(result.TrimEnd(end.ToCharArray()));
            return this;
        }

        /// <summary>
        /// 清空字符串
        /// </summary>
        public StringHelper Clear()
        {
            Builder = Builder.Clear();
            return this;
        }

        /// <summary>
        /// 字符串长度
        /// </summary>
        public int Length => Builder.Length;

        /// <summary>
        /// 空字符串
        /// </summary>
        public string Empty => string.Empty;

        /// <summary>
        /// 转换为字符串
        /// </summary>
        public override string ToString()
        {
            return Builder.ToString();
        }
    }
}
