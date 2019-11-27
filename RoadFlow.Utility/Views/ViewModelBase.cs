using RoadFlow.Utility.Exceptions;
using RoadFlow.Utility.Validations;
using System.Linq;
using System.Runtime.Serialization;

namespace RoadFlow.Utility.Views
{
    /// <summary>
    /// 视图模型
    /// </summary>
    [DataContract]
    public abstract class ViewModelBase : IValidation
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
