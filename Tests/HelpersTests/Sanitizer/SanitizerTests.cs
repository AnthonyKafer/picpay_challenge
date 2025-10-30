using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace picpay_challenge.Helpers
{
    [TestClass]
    public class SanitizerTests
    {
        [TestMethod]
        public void SanitizerOnlyDigitsTests()
        {
            string dirtyString = "$.14520-cfafkahfaj/12:42152fafavvbas";

            Assert.AreEqual(Sanitizer.OnlyDigits(dirtyString), "145201242152");
        }
    }

}
