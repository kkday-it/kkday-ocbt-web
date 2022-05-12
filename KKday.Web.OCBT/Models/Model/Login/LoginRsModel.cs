using System;
namespace KKday.Web.OCBT.Models.Model.Login
{
    public class UserInfoEx : UserInfo
    {
        public string email { get; set; }
    }
    public class LoginRsModel : RsModel
    {
        public UserInfoEx userInfo { get; set; }
    }
}
