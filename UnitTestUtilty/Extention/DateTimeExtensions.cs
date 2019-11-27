using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;


namespace UnitTestUtilty
{
    [TestClass]
    public class DateTimeExtensions
    {
        [TestMethod]
        public void WhatDayWeek()
        {
           var t1= RoadFlow.Utility.DateTimeExtensions.WhatDayWeek(DateTime.Now,3);
            Assert.AreEqual(DateTime.Now.AddDays(-1).ToShortDateString(), t1.ToShortDateString());
        }



     


    }
}
