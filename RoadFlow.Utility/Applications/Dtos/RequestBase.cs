using RoadFlow.Utility.Exceptions;
using RoadFlow.Utility.Ui.Attributes;
using RoadFlow.Utility.Validations;
using System.Linq;

namespace RoadFlow.Utility.Applications.Dtos
{
    /// <summary>
    /// 请求参数
    /// </summary>
    [Model]
    public abstract class RequestBase : IRequest
    {
        /// <summary>
        /// 验证
        /// </summary>
        public virtual ValidationResultCollection Validate()
        {
            var result = DataAnnotationValidation.Validate(this);
            if (result.IsValid)
                return ValidationResultCollection.Success;
            throw new Warning(result.First().ErrorMessage);
        }
    }
}
