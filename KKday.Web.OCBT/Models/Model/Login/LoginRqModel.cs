using System;
namespace KKday.Web.OCBT.Models.Model.Login
{
    [Serializable]
    public class LoginRqModel : RqModel
    {
        public string email { get; set; }
        public string password { get; set; }
        public string timezone { get; set; }
        public bool rememberme { get; set; }
    }
}