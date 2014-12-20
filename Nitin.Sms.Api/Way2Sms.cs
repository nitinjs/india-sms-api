using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using CsQuery;
 
namespace Nitin.Sms.Api
{ 
    public class Way2Sms
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        private CookieContainer CookieJar { get; set; }
        private SmsWebClient Client { get; set; }

        private string base_url = "http://site23.way2sms.com/";
        private bool IsLoggedIn = false;

        public Way2Sms(string username, string password)
        {
            UserName = username;
            Password = password;
            CookieJar = new CookieContainer();
            Client = new SmsWebClient(CookieJar, false);
        }

        public bool Login()
        {
            string loginPage = base_url + "Login1.action";
            NameValueCollection data = new NameValueCollection();
            data.Add("username", UserName);
            data.Add("password", Password);

            byte[] loginResponseBytes = Client.UploadValues(loginPage, "POST", data);
            CQ loginResponse = System.Text.Encoding.UTF8.GetString(loginResponseBytes);
            IsLoggedIn = loginResponse.Find("[type=password]").Count() == 0;
            return IsLoggedIn;
        }

        public bool SendSms(string recipient, string message)
        {
            if (IsLoggedIn == false)
                throw new Exception("Not logged in");

            string cookieVal = CookieJar.GetCookies(new Uri(base_url))["JSESSIONID"].Value;
            cookieVal = cookieVal.Substring(cookieVal.IndexOf('~') + 1);

            string sendSmsPost = base_url + "smstoss.action";

            NameValueCollection data = new NameValueCollection();
            data.Add("ssaction", "ss");
            data.Add("Token", cookieVal);
            data.Add("mobile", recipient);
            data.Add("message", message);
            data.Add("msgLen", (140-message.Length).ToString());

            Client.UploadValues(sendSmsPost, data);
            return true;
        }
    }
}
