namespace RoadFlow.Utility
{

    /// <summary>
    /// 查询字段类型
    /// </summary>
    public enum datatype
    {
        //字符串类型
        stringType,
        //开始时间类型
        dataStartType,
        //结束时间类型
        dataEndType,
        //guid类型
        guidType,

        //类型数值类型
        typenumType,
        //类型包含
        stringTypeIn,
        intTypeIn
    }



    // Nested Types
    /// <summary>
    /// 类型枚举
    /// </summary>
    public enum OperationType
    {
        /// <summary>
        /// 提交
        /// </summary>
        Submit,
        /// <summary>
        /// 自动提交
        /// </summary>
        FreeSubmit,
        /// <summary>
        /// 保存
        /// </summary>
        Save,
        /// <summary>
        /// 后退
        /// </summary>
        Back,
        /// <summary>
        /// 完成
        /// </summary>
        Completed,
        /// <summary>
        /// 重定向
        /// </summary>
        Redirect,
        /// <summary>
        /// 添加写
        /// </summary>
        AddWrite,
        /// <summary>
        /// 复制完成
        /// </summary>
        CopyforCompleted,
        /// <summary>
        /// 任务结束
        /// </summary>
        TaskEnd
    }


    /// <summary>
    /// 数据库类型
    /// </summary>
    public enum DatabaseType
    {
        /// <summary>
        /// SqlServer数据库
        /// </summary>
        SqlServer,

        /// <summary>
        /// MySql数据库
        /// </summary>
        MySql,

        /// <summary>
        /// Oracle数据库
        /// </summary>
        Oracle,

        /// <summary>
        /// PostgreSql数据库
        /// </summary>
        PostgreSql,
        /// <summary>
        ///  Sqlite数据库
        /// </summary>
        Sqlite
    }




    /// <summary>
    /// 过期类型
    /// </summary>
    public enum ExpireType
    {
        /// <summary>
        /// 绝对过期
        /// 注：即自创建一段时间后就过期
        /// </summary>
        Absolute,

        /// <summary>
        /// 相对过期
        /// 注：即该键未被访问后一段时间后过期，若此键一直被访问则过期时间自动延长
        /// </summary>
        Relative,
    }


    /// <summary>
    /// 英文大小格式
    /// </summary>
    public enum CaseFormat
    {
        /// <summary>
        /// 第一个字母大写
        /// </summary>
        CAPITALIZE_FIRST_LETTER,
        /// <summary>
        /// 小写
        /// </summary>
        LOWERCASE,
        /// <summary>
        /// 大写字母
        /// </summary>
        UPPERCASE
    }

    /// <summary>
    /// 语气格式
    /// </summary>
    public enum ToneFormat
    {

        WITH_TONE_MARK,
        WITHOUT_TONE,
        WITH_TONE_NUMBER
    }

    public enum VCharFormat
    {
        WITH_U_AND_COLON,
        WITH_V,
        WITH_U_UNICODE
    }

    /// <summary>
    /// 考试题目类型
    /// </summary>
    public enum TmType
    {
        /// <summary>
        /// 填空题
        /// </summary>
        fillblank = 5,
        /// <summary>
        /// 多选题
        /// </summary>
        multiple = 3,
        /// <summary>
        /// 单选题
        /// </summary>
        single = 2,
        /// <summary>
        /// 主观题
        /// </summary>
        subject = 1,
        /// <summary>
        /// 判断题
        /// </summary>
        trueorfalse = 4
    }




    /// <summary>
    /// 字典值字段
    /// </summary>
    public enum ValueField
    {
        /// <summary>
        /// 字典Id
        /// </summary>
        Id,
        /// <summary>
        /// 标题
        /// </summary>
        Title,
        /// <summary>
        /// 唯一代码
        /// </summary>
        Code,
        /// <summary>
        /// 值
        /// </summary>
        Value,
        /// <summary>
        /// 其它信息
        /// </summary>
        Other,
        /// <summary>
        /// 备注
        /// </summary>
        Note
    }


    // Nested Types
    public enum LogType
    {
        用户登录,
        系统管理,
        流程管理,
        表单管理,
        流程运行,
        数据连接,
        系统异常,
        其它
    }



    #region 类型定义

    /// <summary>
    /// Http请求方法定义
    /// </summary>
    public enum HttpMethod
    {
        Get,
        Post,
        Put,
        Delete,
        Head,
        Options,
        Trace,
        Connect
    }

    public enum ContentType
    {
        /// <summary>
        /// 传统Form表单,即application/x-www-form-urlencoded
        /// </summary>
        Form,
        /// <summary>
        /// 使用Json,即application/json
        /// </summary>
        Json
    }

    #endregion

}
