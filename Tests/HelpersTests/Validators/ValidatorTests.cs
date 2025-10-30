using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace picpay_challenge.Helpers.Validators
{
    [TestClass]
    public class ValidatorTest
    {
        [TestMethod]
        public void CPFvalidatorTest()
        {
            string validCPF = "";
            string invalidCPF = "";

            var validCPFResult = Validator.IsValidCPF(validCPF);
            var invalidCPFResult = Validator.IsValidCPF(invalidCPF);

            Assert.IsTrue(validCPFResult);
            Assert.IsFalse(invalidCPFResult);
        }
    }
}