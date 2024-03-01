using System;
using System.Text;

namespace InternationalWaterWebApp.Library.Common
{
    public class EncryptDecryptData
    {
        public string Encryptdata(string decryptItem)
        {
            string strmsg = string.Empty;
            byte[] encode = new byte[decryptItem.Length];
            encode = Encoding.UTF8.GetBytes(decryptItem);
            strmsg = Convert.ToBase64String(encode);
            return strmsg;
        }

        public string Decryptdata(string encryptItem)
        {
            string decryptpwd = string.Empty;
            UTF8Encoding encodepwd = new UTF8Encoding();
            Decoder Decode = encodepwd.GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(encryptItem.Replace(' ', '+'));
            int charCount = Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            decryptpwd = new String(decoded_char);
            return decryptpwd;
        }
    }
}
