using System;
using System.Collections.Generic;
using System.Linq;

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
        private static readonly Dictionary<VendorName, VendorParams> vendors = new Dictionary<VendorName, VendorParams>
        {
            {
                VendorName.AmericanExpress,
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
                VendorName.Maestro,
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
                VendorName.MasterCard,
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
                VendorName.Visa,
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
                VendorName.JCB,
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

        static private string GetIIN(this string cardNumber)
        {
            VendorName vendor = GetCreditCardVendor(cardNumber);
            int length = 0;
            switch (vendor)
            {
                case VendorName.AmericanExpress:
                    length = 2;
                    break;
                case VendorName.Maestro:
                    length = 2;
                    break;
                case VendorName.MasterCard:
                    length = cardNumber.StartsWith("2") ? 4 : 2;
                    break;
                case VendorName.Visa:
                    length = 1;
                    break;
                case VendorName.JCB:
                    length = 4;
                    break;
                default:
                    break;
            }

            return cardNumber.Substring(0,length);
        }

        static public VendorName GetCreditCardVendor(string cardNumber)
        {
            //American Express: IIN 34, 37; Length 15
            //Maestro: IIN 50, 56-69; Length 12-19
            //MasterCard: IIN 51-55, 2221-2720; Length 16
            //Visa: IIN 4; Length 16
            //JCB: IIN 3528-3589; Length 16-19
            if (!IsCreditCardNumberValid(cardNumber))
            {
                return VendorName.Unknown;
            }

            cardNumber = cardNumber.DeleteAllWhitespaces();

            int IIN1Dig = Convert.ToInt32(cardNumber.Substring(0,1));
            int IIN2Dig = Convert.ToInt32(cardNumber.Substring(0,2));
            int IIN4Dig = Convert.ToInt32(cardNumber.Substring(0,4));

            //Default value is Unknown
            VendorName vendor = vendors.SingleOrDefault(o =>
                cardNumber.Length >= o.Value.minLength &&
                cardNumber.Length <= o.Value.maxLength &&
                (o.Value.IINs.Exists(p => IIN1Dig >= p.minIIN && IIN1Dig <= p.maxIIN) ||
                o.Value.IINs.Exists(p => IIN2Dig >= p.minIIN && IIN2Dig <= p.maxIIN) ||
                o.Value.IINs.Exists(p => IIN4Dig >= p.minIIN && IIN4Dig <= p.maxIIN))
            ).Key;

            return vendor;
        }

        static public bool IsCreditCardNumberValid(string cardNumber)
        {
            bool isFormatValid = Helper.FormatValidate(cardNumber);
            if (!isFormatValid)
            {
                return false;
            }
            cardNumber = cardNumber.DeleteAllWhitespaces();
            return Luhn.Validate(cardNumber);
        }



        static public string GenerateNextCreditCardNumber(string cardNumber)
        {
            if (!IsCreditCardNumberValid(cardNumber))
            {
                return null;
            }

            cardNumber = Helper.DeleteAllWhitespaces(cardNumber);          

            VendorName cardVendor = GetCreditCardVendor(cardNumber);
            var vendorInfo = vendors[cardVendor];

            int defaultLength = cardNumber.Length;
            bool isNeedUpdate = false;

            for (int i = defaultLength; i <= vendorInfo.maxLength; i++)
            {
                foreach (var IIN in vendorInfo.IINs)
                {
                    int cardIIN = Convert.ToInt32(cardNumber.GetIIN());
                    if (Helper.FormatIINTo6Digs(cardIIN) > Helper.FormatIINTo6Digs(IIN.maxIIN) &&
                        i == defaultLength)
                    {
                        continue;
                    }

                    //Default IIN is Unknown
                    var defaultIIN = vendorInfo.IINs.SingleOrDefault(e =>
                        Helper.FormatIINTo6Digs(cardIIN) >= Helper.FormatIINTo6Digs(e.minIIN) &&
                        Helper.FormatIINTo6Digs(cardIIN) <= Helper.FormatIINTo6Digs(e.maxIIN)
                    );

                    int minIIN = (i > defaultLength || IIN != defaultIIN) ? IIN.minIIN : cardIIN;

                    for (int curIIN = minIIN; curIIN <= IIN.maxIIN; curIIN++)
                    {
                        string curIINStr = curIIN.ToString();
                        int curIINLength = curIINStr.Length;
                        string curClientID = cardNumber.Substring(curIINLength, i - curIINLength - 1);

                        if (isNeedUpdate)
                        {
                            cardNumber = curIINStr + new string('0', i - curIINLength);
                            curClientID = cardNumber.Substring(curIINLength, i - curIINLength - 1);
                        }
                        else
                        {
                            string newClientID = (Convert.ToUInt64(curClientID) + 1).ToString();
                            if (newClientID.Length > curClientID.Length)
                            {
                                isNeedUpdate = true;
                                continue;
                            }
                            else
                            {
                                curClientID = newClientID;
                            }
                        }

                        int nextCardLastDig = Luhn.CalculateChecksum(curIINStr + curClientID);
                        string nextCardNumber = curIINStr + curClientID + nextCardLastDig.ToString();

                        return nextCardNumber;
                    }
                }               
            }

            return null;
        }

    }
}
