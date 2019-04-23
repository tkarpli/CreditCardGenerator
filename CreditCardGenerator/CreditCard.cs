using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CreditCardGenerator
{
    class IINStruct
    {
        public int minIIN;
        public int maxIIN;
    }

    class VendorParams
    {
        public List<IINStruct> IINs;
        public int minLength;
        public int maxLength;
    }

    public static class CreditCard
    {
        private static readonly Dictionary<string, VendorParams> vendors = new Dictionary<string, VendorParams>
        {
            {
                "American Express",
                new VendorParams()
                {
                    IINs = new List<IINStruct>()
                    {
                        new IINStruct() { minIIN = 37, maxIIN = 37 },
                        new IINStruct() { minIIN = 34, maxIIN = 34 }
                    },
                    minLength = 15,
                    maxLength = 15
                }
            },

            {
                "Maestro",
                 new VendorParams()
                 {
                    IINs = new List<IINStruct>()
                    {
                        new IINStruct() { minIIN = 50, maxIIN = 50 },
                        new IINStruct() { minIIN = 56, maxIIN = 69 }
                    },
                    minLength = 12,
                    maxLength = 19
                 }
            },

            {
                "MasterCard",
                 new VendorParams()
                 {
                    IINs = new List<IINStruct>()
                    {
                        new IINStruct() { minIIN = 2221, maxIIN = 2720 },
                        new IINStruct() { minIIN = 51, maxIIN = 55 }
                    },
                    minLength = 16,
                    maxLength = 16
                 }
            },


            {
                "Visa",
                new VendorParams()
                {
                    IINs = new List<IINStruct>()
                    {
                        new IINStruct() { minIIN = 4, maxIIN = 4 }
                    },
                    minLength = 16,
                    maxLength = 16
                }
            },

            {
                "JCB",
                new VendorParams()
                {
                    IINs = new List<IINStruct>()
                    {
                        new IINStruct() { minIIN = 3528, maxIIN = 3589 }
                    },
                    minLength = 16,
                    maxLength = 19
                }
            }
        };

        static private string DeleteAllWhitespaces(this string str)
        {
            return str.Replace(" ", "");
        }

        static private bool FormatValidate(this string cardNumber)
        {
            bool isFormatValid = Regex.IsMatch(cardNumber, @"^(?:((\d{4} ){2,3}\d{1,4})|((\d{4} ){4}\d{1,3})|(\d{12,19}))$");
            return isFormatValid;
        }

        static private string GetIIN(this string cardNumber)
        {
            string vendor = GetCreditCardVendor(cardNumber);
            int length = 0;
            switch (vendor)
            {
                case "American Express":
                    length = 2;
                    break;
                case "Maestro":
                    length = 2;
                    break;
                case "MasterCard":
                    length = cardNumber.StartsWith("2") ? 4 : 2;
                    break;
                case "Visa":
                    length = 1;
                    break;
                case "JCB":
                    length = 4;
                    break;
                default:
                    break;
            }

            return cardNumber.Substring(0,length);
        }

        static private int CalculateChecksum(string cardNumberWithoutLastDig)
        {
            cardNumberWithoutLastDig = cardNumberWithoutLastDig.DeleteAllWhitespaces();
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
            //American Express: IIN 34, 37; Length 15
            //Maestro: IIN 50, 56-69; Length 12-19
            //MasterCard: IIN 51-55, 2221-2720; Length 16
            //Visa: IIN 4; Length 16
            //JCB: IIN 3528-3589; Length 16-19
            if (!IsCreditCardNumberValid(cardNumber))
                return "Unknown";

            cardNumber = cardNumber.DeleteAllWhitespaces();

            int IIN1Dig = Convert.ToInt32(cardNumber.Substring(0,1));
            int IIN2Dig = Convert.ToInt32(cardNumber.Substring(0,2));
            int IIN4Dig = Convert.ToInt32(cardNumber.Substring(0,4));

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
            cardNumber = cardNumber.DeleteAllWhitespaces();
            string cardNumberWithoutLastDig = cardNumber.Substring(0,cardNumber.Length - 1);
            int lastDigInt = Convert.ToInt32(cardNumber.Substring(cardNumber.Length - 1, 1));
            #region Luhn algorithm
            if (CalculateChecksum(cardNumberWithoutLastDig) == lastDigInt)
                return true;
            else
                return false;
            #endregion
        }

        static public string GenerateNextCreditCardNumber(string cardNumber)
        {
            if (!IsCreditCardNumberValid(cardNumber))
                return null;

            cardNumber = DeleteAllWhitespaces(cardNumber);
            ulong cardNumberInt = Convert.ToUInt64(cardNumber);
            string curCardNumber = cardNumber;
            string cardVendor = GetCreditCardVendor(cardNumber);
            var vendorInfo = vendors[cardVendor];
            bool isNeedUpdate = false;

            for (int i = cardNumber.Length; i <= vendorInfo.maxLength; i++)
            {
                foreach (var IIN in vendorInfo.IINs)
                {
                    int cardIIN = Convert.ToInt32(curCardNumber.GetIIN());
                    if (Convert.ToInt32(cardIIN.ToString().PadRight(6,'0')) > Convert.ToInt32(IIN.maxIIN.ToString().PadRight(6,'0')) && i == cardNumber.Length)
                        continue;

                    var defaultIIN = vendorInfo.IINs.SingleOrDefault(e => (Convert.ToInt32(cardIIN.ToString().PadRight(6, '0')) >= Convert.ToInt32(e.minIIN.ToString().PadRight(6, '0'))) &&
                       (Convert.ToInt32(cardIIN.ToString().PadRight(6, '0')) <= Convert.ToInt32(e.maxIIN.ToString().PadRight(6, '0'))));

                    int minIIN = (i>cardNumber.Length || IIN != defaultIIN) ? IIN.minIIN : cardIIN;

                    for (int curIIN = minIIN; curIIN <= IIN.maxIIN; curIIN++)
                    {
                        int curIINLength = curIIN.ToString().Length;
                        if (isNeedUpdate)
                            curCardNumber = curIIN.ToString() + new string('0', i - curIINLength);

                        string curClientID = curCardNumber.Substring(curIINLength, i - curIINLength - 1);
                        ulong curClientIDInt = Convert.ToUInt64(curClientID);

                        if (IsCreditCardNumberValid(curCardNumber))
                        {
                            string newClientID = (curClientIDInt + 1).ToString();

                            if (newClientID.Length > curClientID.Length)
                            {
                                isNeedUpdate = true;
                                continue;
                            }
                            else
                                curClientID = newClientID;
                        }

                        string nextCardWithoutLastDig = curIIN.ToString() + curClientID;
                        int nextCardLastDig = CalculateChecksum(nextCardWithoutLastDig);
                        string nextCardNumber = nextCardWithoutLastDig + nextCardLastDig.ToString();

                        return nextCardNumber;
                    }
                }               
            }

            return null;
        }

    }
}
