using RoadFlow.Utility;
using RoadFlow.Utility.Properties;
using RoadFlow.Utility.Validations.Validators;
using System.Globalization;
using System.Text.RegularExpressions;

namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    /// 身份证验证
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IdCardAttribute : ValidationAttribute
    {
        /// <summary>
        /// 格式化错误消息
        /// </summary>
        public override string FormatErrorMessage(string name)
        {
            if (ErrorMessage == null && ErrorMessageResourceName == null)
                //判断IdCard验证码
                ErrorMessage = LibraryResource.InvalidIdCard;
            return String.Format(CultureInfo.CurrentCulture, ErrorMessageString);
        }

        /// <summary>
        /// 是否验证通过
        /// </summary>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value.SafeString().IsNullOrWhiteSpace())
                return ValidationResult.Success;
            if (Regex.IsMatch(value.SafeString(), ValidatePattern.IdCardPattern))
                return ValidationResult.Success;
            return new ValidationResult(FormatErrorMessage(string.Empty));
        }
    }
}