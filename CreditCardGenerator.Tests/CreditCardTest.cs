using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreditCardGenerator.Tests
{
    [TestClass]
    public class CreditCardTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetCreditCardVendor_NullArgument_ThrowsArgumentNullException()
        {
            CreditCard.GetCreditCardVendor(null);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void IsCreditCardNumberValid_NullArgument_ThrowsArgumentNullException()
        {
            CreditCard.IsCreditCardNumberValid(null);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GenerateNextCreditCardNumber_NullArgument_ThrowsArgumentNullException()
        {
            CreditCard.GenerateNextCreditCardNumber(null);
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetCreditCardVendor_InvalidCardNumberWithWorngSymbold_ThrowsArgumentException()
        {
            CreditCard.GetCreditCardVendor("3433 1111 2222asd 333?");
        }

        [TestMethod]
        public void GetCreditCardVendor_Valid15DigitsWithSpacesAmericanExpress_ReturnsAmericanExpressVendor()
        {
            Assert.AreEqual(VendorName.AmericanExpress, CreditCard.GetCreditCardVendor("3433 1111 2222 333"));
        }

        [TestMethod]
        public void GetCreditCardVendor_Valid13DigitsWithoutSpacesVisa_ReturnsVisa()
        {
            Assert.AreEqual(VendorName.Visa, CreditCard.GetCreditCardVendor("4567123478693"));
        }

        [TestMethod]
        public void GetCreditCardVendor_Invalid17DigitsWithoutSpacesJcb_ReturnsUnknow()
        {
            Assert.AreEqual(VendorName.Unknown, CreditCard.GetCreditCardVendor("35301113333000001"));
        }

        [TestMethod]
        public void IsCreditCardNumberValid_Valid16DigitsWithoutSpaces_ReturnsTrue()
        {
            Assert.IsTrue(CreditCard.IsCreditCardNumberValid("5555555555554444"));
        }

        [TestMethod]
        public void IsCreditCardNumberValid_Invalid16DigitsWithoutSpaces_ReturnsFalse()
        {
            Assert.IsFalse(CreditCard.IsCreditCardNumberValid("3530111333300001"));
        }

        [TestMethod]
        public void IsCreditCardNumberValid_Valid16DigitsWithSpaces_ReturnsTrue()
        {
            Assert.IsTrue(CreditCard.IsCreditCardNumberValid("4012 8888 8888 1881"));
        }

        [TestMethod]
        public void IsCreditCardNumberValid_Invalid16DigitsWithSpaces_ReturnsTrue()
        {
            Assert.IsFalse(CreditCard.IsCreditCardNumberValid("4111 1111 1111 1110"));
        }

        [TestMethod]
        public void GenerateNextCreditCardNumber_WithoutSpaces_ReturnsValidCreditCard()
        {
            Assert.IsTrue(CreditCard.IsCreditCardNumberValid(CreditCard.GenerateNextCreditCardNumber("5555555555554444")));
        }

        [TestMethod]
        public void GenerateNextCreditCardNumber_WithSpaces_ReturnsValidCreditCard()
        {
            Assert.IsTrue(CreditCard.IsCreditCardNumberValid(CreditCard.GenerateNextCreditCardNumber("4012 8888 8888 1881")));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GenerateNextCreditCardNumber_MaximumCardNumber_ThrowsArgumentException()
        {
            CreditCard.GenerateNextCreditCardNumber("4999999999999999993");
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GenerateNextCreditCardNumber_UnknowVendor_ThrowsArgumentException()
        {
            CreditCard.GenerateNextCreditCardNumber("0000999999999999985");
            Assert.Fail();
        }

        [TestMethod]
        public void GenerateNextCreditCardNumber_19DigitsWithoutSpaces_IncrementCardNumber()
        {
            Assert.AreEqual("4999999999999999993", CreditCard.GenerateNextCreditCardNumber("4999999999999999985"));
        }

        [TestMethod]
        public void GenerateNextCreditCardNumber_16DigitsWithoutSpaces_IncrementBinNumber()
        {
            Assert.AreEqual("5100000000000008", CreditCard.GenerateNextCreditCardNumber("2720999999999996"));
        }

        [TestMethod]
        public void GenerateNextCreditCardNumber_12DigitsWithSpaces_IncrementLengthTo13()
        {
            Assert.AreEqual("5000000000005", CreditCard.GenerateNextCreditCardNumber("6999 9999 9997"));
        }

        [TestMethod]
        public void GenerateNextCreditCardNumber_16DigitsWithSpaces_IncrementLengthTo19()
        {
            Assert.AreEqual("4000000000000000006", CreditCard.GenerateNextCreditCardNumber("4999 9999 9999 9996"));
        }
    }
}
