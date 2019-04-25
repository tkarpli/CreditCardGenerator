using System;
using System.Linq;

namespace CreditCardGenerator
{
    public class Luhn
    {
        static public int CalculateChecksum(string cardNumberWithoutLastDig)
        {
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
            int checksum = (10 - sum % 10) % 10;
            return checksum;
        }

        static public bool Validate(string cardNumber)
        {
            string cardNumberWithoutLastDig = cardNumber.Substring(0, cardNumber.Length - 1);
            int lastDigInt = Convert.ToInt32(cardNumber.Substring(cardNumber.Length - 1, 1));
            if (CalculateChecksum(cardNumberWithoutLastDig) == lastDigInt)
                return true;
            else
                return false;
        }
    }
}
