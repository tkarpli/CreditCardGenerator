using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CreditCardGenerator
{
    class CreditCard
    {
        static private string DeleteAllWhitespaces(string str)
        {
            return str.Replace(" ", "");
        }

        static public string GetCreditCardVendor(string cardNumber)
        {
            //American Express: IIN 34,37; Length 15
            //Maestro: IIN 50,56-69; Length 12-19
            //Mastercard: IIN 2221-2720, 51-55; Length 16
            //Visa: IIN 4; Length 16
            //JCB: IIN 3528-3589; Length 16-19
            cardNumber = DeleteAllWhitespaces(cardNumber);
            int IIN1Dig = Convert.ToInt32(cardNumber.Substring(0, 1));
            int IIN2Dig = Convert.ToInt32(cardNumber.Substring(0, 2));
            int IIN4Dig = Convert.ToInt32(cardNumber.Substring(0, 4));

            string vendor = null;
            if (cardNumber.Length == 15 && (IIN2Dig == 34 || IIN2Dig == 37))
                vendor = "American Express";
            else if (cardNumber.Length >= 12 && cardNumber.Length <= 19 && (IIN2Dig == 50 || (IIN2Dig >= 56 && IIN2Dig <= 69)))
                vendor = "Maestro";
            else if (cardNumber.Length == 16 && ((IIN4Dig >= 2221 && IIN4Dig <= 2720) || (IIN2Dig >= 51 && IIN2Dig <= 55)))
                vendor = "MasterCard";
            else if (cardNumber.Length == 16 && IIN1Dig == 4)
                vendor = "Visa";
            else if (cardNumber.Length >= 16 && cardNumber.Length <= 19 && IIN4Dig >= 3528 && IIN4Dig <= 3589)
                vendor = "JCB";
            else
                vendor = "Unknown";

            return vendor;
        }

        static public bool IsCreditCardNumberValid(string cardNumber)
        {
            bool isFormatValid = Regex.IsMatch(cardNumber, @"^(?:((\d{4} ){2,3}\d{1,4})|((\d{4} ){4}\d{1,3})|(\d{12,19}))$");
            if (!isFormatValid)
                return false;
            cardNumber = DeleteAllWhitespaces(cardNumber);
            #region Luhn algorithm
            int sum = 0;
            int n;
            bool alternate = false;
            char[] nx = cardNumber.ToArray();
            for (int i = cardNumber.Length - 1; i >= 0; i--)
            {
                n = int.Parse(nx[i].ToString());

                if (alternate)
                {
                    n *= 2;

                    if (n > 9)
                    {
                        n = (n % 10) + 1;
                    }
                }
                sum += n;
                alternate = !alternate;
            }
            return sum % 10 == 0;
            #endregion
        }

        static public string GenerateNextCreditCardNumber(string cardNumber)
        {
            cardNumber = DeleteAllWhitespaces(cardNumber);
            long cardNumberInt = Convert.ToInt64(cardNumber);
            string cardVendor = GetCreditCardVendor(cardNumber);
            do
            {
                cardNumberInt++;
                if (IsCreditCardNumberValid(cardNumberInt.ToString()) && GetCreditCardVendor(cardNumberInt.ToString()) == cardVendor)
                    return cardNumberInt.ToString();
            }
            while (cardNumberInt.ToString().Length == cardNumber.Length);

            return null;
        }

    }
}
