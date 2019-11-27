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

        private static readonly string AssemblyName = "RoadFlow.Utility"; //����

        [Fact]
        public void ReflectionHelper()
        {
            string className = "RoadFlow.Utility.Randoms.GuidRandomGenerator";
            int runcount = 200000;
            System.Diagnostics.Stopwatch watch = new Stopwatch();

            watch.Start(); // ��ʼ���Ӵ�������ʱ��
            for (int i = 0; i < runcount; i++)
            {
                //Ҫ�ṩ���򼯣�����鷳��ͬʱЧ�ʲ���
                var app = (RoadFlow.Utility.Randoms.GuidRandomGenerator)Assembly.Load(AssemblyName).CreateInstance(className);
                var time = app.Generate();
                //  func();//ִ��ĳ������
            }
            watch.Stop();

            float sec = watch.ElapsedMilliseconds / 1000.0f;
            float freq = sec / runcount;

            var t1 = string.Format("����ִ��ʱ��Ϊ:{0}��,����ִ�д���Ϊ:{1},ƽ��ִ��ʱ��Ϊ:{2}��", sec, runcount, freq);



            System.Diagnostics.Stopwatch watch1 = new Stopwatch();

            watch1.Start(); // ��ʼ���Ӵ�������ʱ��
            for (int i = 0; i < runcount; i++)
            {
                //���ٶȿ�
                var app1 = RoadFlow.Utility.ReflectionHelper.CreateInstance<RoadFlow.Utility.Randoms.GuidRandomGenerator>(typeof(RoadFlow.Utility.Randoms.GuidRandomGenerator));
                var time1 = app1.Generate();
                //  func();//ִ��ĳ������
            }
            watch1.Stop();

            float sec1 = watch1.ElapsedMilliseconds / 1000.0f;
            float freq1 = sec1 / runcount;

            var t2 = string.Format("����ִ��ʱ��Ϊ:{0}��,����ִ�д���Ϊ:{1},ƽ��ִ��ʱ��Ϊ:{2}��", sec1, runcount, freq1);

            System.Diagnostics.Stopwatch watch2 = new Stopwatch();

            watch2.Start(); // ��ʼ���Ӵ�������ʱ��
            for (int i = 0; i < runcount; i++)
            {
                
                var app2 = new RoadFlow.Utility.Randoms.GuidRandomGenerator();
                var time2 = app2.Generate();
                //  func();//ִ��ĳ������
            }
            watch2.Stop();

            float sec2 = watch2.ElapsedMilliseconds / 1000.0f;
            float freq2 = sec2 / runcount;

            var t3= string.Format("����ִ��ʱ��Ϊ:{0}��,����ִ�д���Ϊ:{1},ƽ��ִ��ʱ��Ϊ:{2}��", sec2, runcount, freq2);


            System.Diagnostics.Stopwatch watch3 = new Stopwatch();

            watch3.Start(); // ��ʼ���Ӵ�������ʱ��
            for (int i = 0; i < runcount; i++)
            {

                var app3 = RoadFlow.Utility.DataIocHelper.DataIoc1<RoadFlow.Utility.Randoms.GuidRandomGenerator>();
                var time3 = app3.Generate();
                //  func();//ִ��ĳ������
            }
            watch3.Stop();

            float sec3 = watch3.ElapsedMilliseconds / 1000.0f;
            float freq3 = sec3 / runcount;

            var t4 = string.Format("����ִ��ʱ��Ϊ:{0}��,����ִ�д���Ϊ:{1},ƽ��ִ��ʱ��Ϊ:{2}��", sec3, runcount, freq3);




            var t = 0;

        }

        [Fact]
        public void ReflectionTest()
        {
            int runcount = 500;
            //Stopwatch watch = Stopwatch.StartNew();//����һ��������
            System.Diagnostics.Stopwatch watch = new Stopwatch();

            watch.Start(); // ��ʼ���Ӵ�������ʱ��
            for (int i = 0; i < runcount; i++)
            {

              //  func();//ִ��ĳ������
            }
            watch.Stop();

            float sec = watch.ElapsedMilliseconds / 1000.0f;
            float freq = sec / runcount;

           var t1=string.Format("����ִ��ʱ��Ϊ:{0}��,����ִ�д���Ϊ:{1},ƽ��ִ��ʱ��Ϊ:{2}��", sec, runcount, freq);


            var t99 = 0;

        
        }


      


    }
}
