namespace RoadFlow.Utility.EnumExtention
{
    /// <summary>
    /// 异常枚举扩展
    /// </summary>
    public enum EnumException1
    {
        /// <summary>
        /// 删除成功
        /// </summary>
        // DeleteSuccess,
        删除成功,
        /// <summary>
        /// 其他用户正在执行该操作,请稍后再试
        /// </summary>
       // GlobalDuplicateRequest,
        其他用户正在执行该操作__请稍后再试,
        /// <summary>
        /// 操作成功
        /// </summary>
        //Success,
        操作成功,
        /// <summary>
        /// 系统忙，请稍后再试
        /// </summary>
        // SystemError,
        系统忙__请稍后再试,
        /// <summary>
        /// 请不要重复提交
        /// </summary>
        //  UserDuplicateRequest
        请不要重复提交,
        IdCard验证为空
    }
}
