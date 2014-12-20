using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nitin.Sms.Api;

namespace Api.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            OneSixtybyTwo o160 = new OneSixtybyTwo("username", "pass");
            o160.Login();
            o160.SendSms("10 digit phone", "Test Message");

            Way2Sms oway2 = new Way2Sms("username", "pass");
            oway2.Login();
            oway2.SendSms("10 digit phone", "Test Message");

            Console.WriteLine("Press any key to exit");
            Console.ReadLine();
        }
    }
}
