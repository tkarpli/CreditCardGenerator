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

        static private bool FormatValidate(string cardNumber)
        {
            bool isFormatValid = Regex.IsMatch(cardNumber, @"^(?:((\d{4} ){2,3}\d{1,4})|((\d{4} ){4}\d{1,3})|(\d{12,19}))$");
            return isFormatValid;
        }

        static private int CalculateChecksum(string cardNumberWithoutLastDig)
        {
            cardNumberWithoutLastDig = DeleteAllWhitespaces(cardNumberWithoutLastDig);
            int sum = 0;
            int n;
            bool alternate = true;
            char[] nx = cardNumberWithoutLastDig.ToArray();
            for (int i = cardNumberWithoutLastDig.Length - 1; i >= 0; i--)
            {
                n = int.Parse(nx[i].ToString());

                if (alternate)
                {
                    n *= 2;

                    if (n > 9)
                    {
                        n -= 9;
                    }
                }
                sum += n;
                alternate = !alternate;
            }
            int checksum = (10 - sum % 10)%10;
            return checksum;
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
            bool isFormatValid = FormatValidate(cardNumber);
            if (!isFormatValid)
                return false;
            cardNumber = DeleteAllWhitespaces(cardNumber);
            string cardNumberWithoutLastDig = cardNumber.Substring(0,cardNumber.Length - 1);
            int lastDigInt = Convert.ToInt32(cardNumber.Substring(cardNumber.Length - 1, 1));
            #region Luhn algorithm
            if (CalculateChecksum(cardNumberWithoutLastDig) == lastDigInt)
                return true;
            else
                return false;
            #endregion
        }

        //TODO: Resolve conflicts between string and int; Convert somewhere Int to String or another way;

        static public string GenerateNextCreditCardNumber(string cardNumber)
        {
            cardNumber = DeleteAllWhitespaces(cardNumber);
            ulong cardNumberInt = Convert.ToUInt64(cardNumber);
            ulong nextCardNumber = cardNumberInt;
            ulong nextCardNumberIntWithoutLastDig = Convert.ToUInt64(cardNumber.Substring(0, cardNumber.Length - 1));
            string cardVendor = GetCreditCardVendor(cardNumber);
            do
            {
                if (IsCreditCardNumberValid(nextCardNumber.ToString()))
                    nextCardNumberIntWithoutLastDig++;
                int nextCardLastDig = CalculateChecksum(nextCardNumberIntWithoutLastDig.ToString());
                nextCardNumber = Convert.ToUInt64(nextCardNumberIntWithoutLastDig.ToString() + nextCardLastDig.ToString());

                if (IsCreditCardNumberValid(nextCardNumber.ToString()) && GetCreditCardVendor(nextCardNumber.ToString()) == cardVendor)
                    return nextCardNumber.ToString();
            }
            while (GetCreditCardVendor(nextCardNumber.ToString()) == cardVendor);

            return null;
        }

    }
}
