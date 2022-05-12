using System.Collections.Generic;

namespace KKday.Web.OCBT.Models.Model.Login
{
    /// <summary>
    /// 依據使用者的登入類型，驗證輸入的使用者資料
    /// </summary>
    public class OAuthLoginRsModel
    {
        public Metadata metadata { get; set; }
        public OAuthLoginData data { get; set; }
    }
    /// <summary>
    /// 取得 auth_user + sub_users 資料 by AuthKey
    /// </summary>
    public class OAuthProfileRsModel
    {
        public Metadata metadata { get; set; }
        public OAuthProfileData data { get; set; }
    }
    public class Metadata
    {
        public string status { get; set; }
        public string desc { get; set; }
        public string pagination { get; set; }
        public string errors { get; set; }
    }
    public class OAuthLoginData
    {
        public string authorizationCode { get; set; }
    }
    public class Common
    {
        public string authOid { get; set; }
        public string createdBy { get; set; }
        public string createdAt { get; set; }
        public string updatedBy { get; set; }
        public string updatedAt { get; set; }
    }
    public class OAuthProfileData : Common
    {
        public string authKey { get; set; }
        public List<SubUsers> subUsers { get; set; }
    }
    public class SubUsers : Common
    {
        public string subAuthOid { get; set; }
        public string platformOid { get; set; }
        public string userStatus { get; set; }
        public UserInfo userDetail { get; set; }
    }
    public class UserInfo
    {
        public string userName { get; set; }
        public string userUuid { get; set; }
    }
}
