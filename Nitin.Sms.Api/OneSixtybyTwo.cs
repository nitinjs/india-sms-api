using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using CsQuery;

namespace Nitin.Sms.Api
{
    public class OneSixtybyTwo
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        private CookieContainer CookieJar { get; set; }
        private SmsWebClient Client { get; set; }

        private string base_url = "http://www.160by2.com/";
        private bool IsLoggedIn = false;

        public OneSixtybyTwo(string username, string password)
        {
            UserName = username;
            Password = password;
            CookieJar = new CookieContainer();
            Client = new SmsWebClient(CookieJar, false);
        }

        public bool Login()
        {
            string loginPage = base_url + "re-login";
            NameValueCollection data = new NameValueCollection();
            data.Add("rssData", "");
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

            CQ sendSmsPage = Client.DownloadString(base_url + "SendSMS?id=" + cookieVal);
            NameValueCollection data = new NameValueCollection();
            //all inputs
            CQ form = sendSmsPage.Find("form[id=frm_sendsms]");
            CQ inputs = form.Find("input[type=hidden]");
            foreach (var input in inputs)
            {
                CQ inp = input.Cq();
                data.Add(inp.Attr("name"),inp.Attr("value"));
            }

            //sms input
            CQ mobileNumberBox = form.Find("input[placeholder='Enter Mobile Number or Name']")[0].Cq();
            data.Add(mobileNumberBox.Attr("name"), recipient);

            //textarea
            data.Add("sendSMSMsg", message);
            string sendSmsPost = base_url + data["fkapps"];

            data["hid_exists"] = "no";
            data["maxwellapps"] = cookieVal;

            //additional vsls
            data.Add("messid_0", "");
            data.Add("messid_1", "");
            data.Add("messid_2", "");
            data.Add("messid_3", "");
            data.Add("messid_4", "");
            data.Add("newsExtnUrl", "");
            data.Add("reminderDate", DateTime.Now.ToString("dd-MM-yyyy"));
            data.Add("sel_hour", "");
            data.Add("sel_minute", "");
            data.Add("ulCategories", "29");

            Client.UploadValues(sendSmsPost, data);

            return true;
        }
    }
}
