using System;
using System.Text.RegularExpressions;

namespace CreditCardGenerator
{
    public static class Helper
    {
        static public string DeleteAllWhitespaces(this string str)
        {
            return str.Replace(" ", "");
        }

        static public bool FormatValidate(this string cardNumber)
        {
            bool isFormatValid = Regex.IsMatch(cardNumber, @"^(?:((\d{4} ){2,3}\d{1,4})|((\d{4} ){4}\d{1,3})|(\d{12,19}))$");
            return isFormatValid;
        }

        static public int FormatIINTo6Digs(int IIN)
        {
            return Convert.ToInt32(IIN.ToString().PadRight(6, '0'));
        }
    }
}
