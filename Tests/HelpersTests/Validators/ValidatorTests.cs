using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace picpay_challenge.Helpers.Validators
{
    [TestClass]
    public class ValidatorTest
    {
        [TestMethod]
        public void CPFvalidatorTest()
        {
            string validCPFWithMask = "000.709.200-87";
            string validCPFWithoutMask = "00070920087";

            string invalidCPFWithMask = "123.456.789-00";
            string invalidCPFWithoutMask = "12345678900";

            Assert.IsTrue(Validator.IsValidCPF(validCPFWithMask));
            Assert.IsTrue(Validator.IsValidCPF(validCPFWithoutMask));


            Assert.IsFalse(Validator.IsValidCPF(invalidCPFWithMask));
            Assert.IsFalse(Validator.IsValidCPF(invalidCPFWithoutMask));
        }

        [TestMethod]
        public void CNPJValidatorTest()
        {
            string validCNPJWithMask = "63.682.686/0001-68";
            string validCNPJWitouthMask = "63682686000168";

            string invalidCNPJWithMask = "12.345.678/0001-00";
            string invalidCNPJWithoutMask = "12345678000100";

            Assert.IsTrue(Validator.IsValidCNPJ(validCNPJWithMask));
            Assert.IsTrue(Validator.IsValidCNPJ(validCNPJWitouthMask));

            Assert.IsFalse(Validator.IsValidCNPJ(invalidCNPJWithMask));
            Assert.IsFalse(Validator.IsValidCNPJ(invalidCNPJWithoutMask));
        }
    }
}