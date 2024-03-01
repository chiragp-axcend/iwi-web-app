using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternationalWaterWebApp.Library.Common
{
    public class RandomPasswordGenerator
    {
        const string LowerCaseChars = "abcdefghijklmnopqursuvwxyz";
        const string UpperCasesChares = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string Numbers = "123456789";
        const string SpecialChars = @"!@#$%^&*()";

        public static string GeneratePassword(int UpperCaseLength = 3, int SpecialsCharLenght = 1, int LowerCaseLength = 3, int NumsLength = 3)
        {
            string Password = "";
            Random random = new Random();

            while (UpperCaseLength > 0)
            {
                Password += UpperCasesChares[random.Next(UpperCasesChares.Length - 1)].ToString();
                UpperCaseLength--;
            }
            while (SpecialsCharLenght > 0)
            {
                Password += SpecialChars[random.Next(SpecialChars.Length - 1)].ToString();
                SpecialsCharLenght--;
            }
            while (LowerCaseLength > 0)
            {
                Password += LowerCaseChars[random.Next(LowerCaseChars.Length - 1)].ToString();
                LowerCaseLength--;
            }
            while (NumsLength > 0)
            {
                Password += Numbers[random.Next(Numbers.Length - 1)].ToString();
                NumsLength--;
            }
            return Password;
        }
    }
}
