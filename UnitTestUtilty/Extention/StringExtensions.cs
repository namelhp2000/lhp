using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;
using RoadFlow.Utility;
using System.Diagnostics;

namespace UnitTestUtilty
{
   
    public class StringExtensions
    {
       



        [Fact]
        public void ToChineseSpell()
        {
            var t1 = RoadFlow.Utility.StringExtensions.ToChineseSpell("��8 wo suan shi 5");
            var t2 = RoadFlow.Utility.StringExtensions.ToPinYing("�Ҵ���");
            var t3 = 0;
        }
       




    }
}
