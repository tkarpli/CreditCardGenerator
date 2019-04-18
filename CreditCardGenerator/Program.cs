using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CreditCardGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Write("Card Number: ");
                string cardNumber = Console.ReadLine();
                bool isValid = CreditCard.IsCreditCardNumberValid(cardNumber);
                string vendor = CreditCard.GetCreditCardVendor(cardNumber);
                Console.WriteLine($"Your card number: {cardNumber}");
                Console.WriteLine($"Vendor of your card: {vendor}");
                Console.WriteLine($"Is your card valid? {isValid}");
                if ( vendor != "Unknown" )
                {
                    string nextCardNumber = CreditCard.GenerateNextCreditCardNumber(cardNumber);
                    if (nextCardNumber != null)
                        Console.WriteLine($"Next card number: {nextCardNumber}");
                    else
                        Console.WriteLine($"Can't generate a new card");
                }
                Console.WriteLine("-----------------");
            }
        }
    }
}
