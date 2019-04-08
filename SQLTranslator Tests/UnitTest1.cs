using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SQLTranslator_Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void RemoveDefaultValueForNumericTypes_WithDefaultValueAndNotNull_ReturnsDefaultValue()
        {
            string entry = "\"ID_1\" INTEGER NOT NULL DEFAULT '0'";
            string sorty = "\"ID_1\" INTEGER NOT NULL DEFAULT 0";
           
        }
    }
}
