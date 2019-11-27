using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Xunit;

namespace UnitTestUtilty
{
   
    public class ReflectionHelperTest
    {

        private static readonly string AssemblyName = "RoadFlow.Utility"; //程序集

        [Fact]
        public void ReflectionHelper()
        {
            string className = "RoadFlow.Utility.Randoms.GuidRandomGenerator";
            int runcount = 200000;
            System.Diagnostics.Stopwatch watch = new Stopwatch();

            watch.Start(); // 开始监视代码运行时间
            for (int i = 0; i < runcount; i++)
            {
                //要提供程序集，相对麻烦，同时效率不高
                var app = (RoadFlow.Utility.Randoms.GuidRandomGenerator)Assembly.Load(AssemblyName).CreateInstance(className);
                var time = app.Generate();
                //  func();//执行某个方法
            }
            watch.Stop();

            float sec = watch.ElapsedMilliseconds / 1000.0f;
            float freq = sec / runcount;

            var t1 = string.Format("总体执行时间为:{0}秒,总体执行次数为:{1},平均执行时间为:{2}秒", sec, runcount, freq);



            System.Diagnostics.Stopwatch watch1 = new Stopwatch();

            watch1.Start(); // 开始监视代码运行时间
            for (int i = 0; i < runcount; i++)
            {
                //简单速度快
                var app1 = RoadFlow.Utility.ReflectionHelper.CreateInstance<RoadFlow.Utility.Randoms.GuidRandomGenerator>(typeof(RoadFlow.Utility.Randoms.GuidRandomGenerator));
                var time1 = app1.Generate();
                //  func();//执行某个方法
            }
            watch1.Stop();

            float sec1 = watch1.ElapsedMilliseconds / 1000.0f;
            float freq1 = sec1 / runcount;

            var t2 = string.Format("总体执行时间为:{0}秒,总体执行次数为:{1},平均执行时间为:{2}秒", sec1, runcount, freq1);

            System.Diagnostics.Stopwatch watch2 = new Stopwatch();

            watch2.Start(); // 开始监视代码运行时间
            for (int i = 0; i < runcount; i++)
            {
                
                var app2 = new RoadFlow.Utility.Randoms.GuidRandomGenerator();
                var time2 = app2.Generate();
                //  func();//执行某个方法
            }
            watch2.Stop();

            float sec2 = watch2.ElapsedMilliseconds / 1000.0f;
            float freq2 = sec2 / runcount;

            var t3= string.Format("总体执行时间为:{0}秒,总体执行次数为:{1},平均执行时间为:{2}秒", sec2, runcount, freq2);


            System.Diagnostics.Stopwatch watch3 = new Stopwatch();

            watch3.Start(); // 开始监视代码运行时间
            for (int i = 0; i < runcount; i++)
            {

                var app3 = RoadFlow.Utility.DataIocHelper.DataIoc1<RoadFlow.Utility.Randoms.GuidRandomGenerator>();
                var time3 = app3.Generate();
                //  func();//执行某个方法
            }
            watch3.Stop();

            float sec3 = watch3.ElapsedMilliseconds / 1000.0f;
            float freq3 = sec3 / runcount;

            var t4 = string.Format("总体执行时间为:{0}秒,总体执行次数为:{1},平均执行时间为:{2}秒", sec3, runcount, freq3);




            var t = 0;

        }

        [Fact]
        public void ReflectionTest()
        {
            int runcount = 500;
            //Stopwatch watch = Stopwatch.StartNew();//创建一个监听器
            System.Diagnostics.Stopwatch watch = new Stopwatch();

            watch.Start(); // 开始监视代码运行时间
            for (int i = 0; i < runcount; i++)
            {

              //  func();//执行某个方法
            }
            watch.Stop();

            float sec = watch.ElapsedMilliseconds / 1000.0f;
            float freq = sec / runcount;

           var t1=string.Format("总体执行时间为:{0}秒,总体执行次数为:{1},平均执行时间为:{2}秒", sec, runcount, freq);


            var t99 = 0;

        
        }


      


    }
}
