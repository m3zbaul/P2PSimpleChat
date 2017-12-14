using System;
using System.IO;
using System.Linq;
using System.Net;

namespace P2PSimpleChat.BLL
{
    public class Utility
    {
        public static bool IsValidNickname(string s)
        {
            return s.All(c => Char.IsLetterOrDigit(c) || c.Equals('_'));
        }
        public static bool IsValidPassword(string s)
        {
            return s.All(
                c => Char.IsLetterOrDigit(c) 
                || c.Equals('_')
                || c.Equals('-')
                || c.Equals('$')
            );
        }
        public static bool IsValidHint(string s)
        {
            return s.All(
                c => Char.IsLetterOrDigit(c)
                || c.Equals('_')
                || c.Equals('-')
                || c.Equals(' ')
            );
        }
        public static bool IsNumeric(string s)
        {
            int n;
            bool isNumeric = int.TryParse(s, out n);
            return isNumeric;
        }
        public static bool IsValidIPv4(string s)
        {
            // verify that IP consists of 4 parts
            if (s.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries).Length == 4)
            {
                IPAddress ipAddr;
                if (IPAddress.TryParse(s, out ipAddr))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static void SaveFile(string path, byte[] bytes)
        {
            try
            {
                File.WriteAllBytes(path, bytes);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
