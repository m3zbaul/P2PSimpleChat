using System;
using System.Collections.Generic;
using System.Linq;

namespace P2PSimpleChat.DAL
{
    public class DataAccess
    {
        //private static P2PSCDBDataContext dbc = new P2PSCDBDataContext(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\P2PSC.mdf;Integrated Security=True;Connect Timeout=30");
        private static P2PSCDBDataContext dbc = new P2PSCDBDataContext(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\rocketeer\Documents\Visual Studio 2015\Projects\P2PSimpleChat\PresentationLayer\P2PSC.mdf;Integrated Security=True");

        public static string GetPassword()
        {
            var x = dbc.Settings.First(i => i.Key == "Password");
            string result = x.Value;
            return result;
        }
        public static string GetHint()
        {
            var x = dbc.Settings.First(i => i.Key == "Password");
            string result = x.Extra;
            return result;
        }
        public static string SetPassword(string p, string h)
        {
            try
            {
                Setting s = (from a in dbc.Settings
                             where a.Key == "Password"
                             select a).SingleOrDefault();
                s.Value = p;
                s.Extra = h;
                dbc.SubmitChanges();
                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public static List<string> GetHelpTopicList()
        {
            var x = from a in dbc.Helps
                    select a;
            return x.Select(a => a.Topic).ToList();
        }
        public static string GetDiscussionByTopic(string topic)
        {
            var x = dbc.Helps.First(i => i.Topic==topic);
            return x.Discussion;
        }
    }
}
