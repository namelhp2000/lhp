using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadFlow.Model
{
    [Serializable, Table("RF_User")]
    public class User : IEqualityComparer<User>
    {
        // Methods
        public User Clone()
        {
            return (User)base.MemberwiseClone();
        }

        public bool Equals(User u1, User u2)
        {
            return (u1.Id == u2.Id);
        }

        public int GetHashCode(User u)
        {
            return u.Id.GetHashCode();
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
        [DisplayName("Id"), Key]
        public Guid Id { get; set; }

        [DisplayName("姓名"), Required(ErrorMessage = "姓名不能为空")]
        public string Name { get; set; }

        // Properties
        [DisplayName("帐号"), Required(ErrorMessage = "帐号不能为空")]
        public string Account { get; set; }

        [DisplayName("密码"), Required(ErrorMessage = "密码不能为空")]
        public string Password { get; set; }
        [DisplayName("性别 0男 1女")]
        public int? Sex { get; set; }
        [DisplayName("状态 0 正常 1 冻结")]
        public int Status { get; set; }
        [DisplayName("职务")]
        public string Job { get; set; }

        [DisplayName("备注")]
        public string Note { get; set; }

        [DisplayName("手机")]
        public string Mobile { get; set; }
        [DisplayName("办公电话")]
        public string Tel { get; set; }

        [DisplayName("其它联系方式")]
        public string OtherTel { get; set; }
        [DisplayName("传真")]
        public string Fax { get; set; }





        [DisplayName("邮箱")]
        public string Email { get; set; }
        [DisplayName("QQ")]
        public string QQ { get; set; }


        [DisplayName("头像")]
        public string HeadImg { get; set; }






        [DisplayName("微信号")]
        public string WeiXin { get; set; }




        [DisplayName("人员兼职的机构ID")]
        public Guid? PartTimeId { get; set; }





        [DisplayName("科室")]
        public string Office { get; set; }



        [DisplayName("微信openid")]
        public string WeiXinOpenId { get; set; }



    }


}
