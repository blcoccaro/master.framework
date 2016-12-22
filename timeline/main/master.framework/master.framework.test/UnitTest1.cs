using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using master.framework;

namespace master.framework.test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var v1 = ((double)13.4).ToMoney();
            var v2 = ((double)44487.33).ToMoney();
            var v3 = ((double)7382.0122).ToMoney();
            var v4 = ((double)3.1).ToMoney();
            var v5 = ((double)32321321.42312).ToMoney();
        }
    }
}
