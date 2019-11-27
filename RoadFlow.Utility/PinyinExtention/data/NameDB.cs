using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace RoadFlow.Pinyin.data
{
    internal class NameDB
    {
        // Fields
        private const string DATA = "艾=ai4\r\n安=an1\r\n敖=ao2\r\n巴=ba1\r\n白=bai2\r\n柏=bai3\r\n班=ban1\r\n包=bao1\r\n暴=bao4\r\n鲍=bao4\r\n贝=bei4\r\n贲=ben1\r\n毕=bi4\r\n边=bian1\r\n卞=bian4\r\n别=bie2\r\n邴=bing3\r\n薄=bo2\r\n卜=bu3\r\n步=bu4\r\n蔡=cai4\r\n苍=cang1\r\n曹=cao2\r\n岑=cen2\r\n柴=chai2\r\n单于=chan2-yu2\r\n昌=chang1\r\n常=chang2\r\n巢=chao2\r\n晁=chao2\r\n车=che1\r\n陈=chen2\r\n成=cheng2\r\n程=cheng2\r\n池=chi2\r\n充=chong1\r\n储=chu3\r\n褚=chu3\r\n淳于=chun2-yu2\r\n从=cong2\r\n崔=cui1\r\n戴=dai4\r\n党=dang3\r\n邓=deng4\r\n狄=di2\r\n第五=di4-wu3\r\n刁=diao1\r\n丁=ding1\r\n东方=dong1-fang1\r\n东=dong1\r\n董=dong3\r\n窦=dou4\r\n都=du1\r\n堵=du3\r\n杜=du4\r\n段=duan4\r\n鄂=e4\r\n樊=fan2\r\n范=fan4\r\n方=fang1\r\n房=fang2\r\n费=fei4\r\n丰=feng1\r\n封=feng1\r\n酆=feng1\r\n冯=feng2\r\n凤=feng4\r\n伏=fu2\r\n扶=fu2\r\n福=fu2\r\n符=fu2\r\n傅=fu4\r\n富=fu4\r\n干=gan1\r\n甘=gan1\r\n高=gao1\r\n郜=gao4\r\n戈=ge1\r\n盖=ge3\r\n葛=ge3\r\n耿=geng3\r\n公孙=gong1-sun1\r\n公羊=gong1-yang2\r\n公冶=gong1-ye3\r\n宗政=gong1-ye3\r\n公=gong1\r\n宫=gong1\r\n弓=gong1\r\n龚=gong1\r\n巩=gong3\r\n贡=gong4\r\n勾=gou1\r\n古=gu3\r\n谷=gu3\r\n顾=gu4\r\n关=guan1\r\n管=guan3\r\n广=guang3\r\n桂=gui4\r\n郭=guo1\r\n国=guo2\r\n韩=han2\r\n杭=hang2\r\n郝=hao3\r\n何=he2\r\n和=he2\r\n赫连=he4-lian2\r\n贺=he4\r\n衡=heng2\r\n弘=hong2\r\n洪=hong2\r\n红=hong2\r\n侯=hou2\r\n後=hou4\r\n胡=hu2\r\n扈=hu4\r\n花=hua1\r\n滑=hua2\r\n华=hua4\r\n怀=huai2\r\n桓=huan2\r\n宦=huan4\r\n皇甫=huang2-fu3\r\n黄=huang2\r\n惠=hui4\r\n霍=huo4\r\n姬=ji1\r\n嵇=ji1\r\n吉=ji2\r\n汲=ji2\r\n纪=ji3\r\n冀=ji4\r\n季=ji4\r\n暨=ji4\r\n蓟=ji4\r\n计=ji4\r\n家=jia1\r\n郏=jia2\r\n贾=jia3\r\n简=jian3\r\n姜=jiang1\r\n江=jiang1\r\n蒋=jiang3\r\n焦=jiao1\r\n金=jin1\r\n靳=jin4\r\n经=jing1\r\n荆=jing1\r\n井=jing3\r\n景=jing3\r\n居=ju1\r\n鞠=ju1\r\n阚=kan4\r\n康=kang1\r\n柯=ke1\r\n空=kong1\r\n孔=kong3\r\n寇=kou4\r\n蒯=kuai3\r\n匡=kuang1\r\n夔=kui2\r\n隗=kui2\r\n赖=lai4\r\n蓝=lan2\r\n郎=lang2\r\n劳=lao2\r\n雷=lei2\r\n冷=leng3\r\n黎=li2\r\n李=li3\r\n利=li4\r\n厉=li4\r\n郦=li4\r\n廉=lian2\r\n连=lian2\r\n梁=liang2\r\n廖=liao4\r\n林=lin2\r\n蔺=lin4\r\n令狐=ling2-hu2\r\n凌=ling2\r\n刘=liu2\r\n柳=liu3\r\n隆=long2\r\n龙=long2\r\n娄=lou2\r\n闾丘=lu2-qiu1\r\n卢=lu2\r\n吕=lu3\r\n鲁=lu3\r\n禄=lu4\r\n路=lu4\r\n逯=lu4\r\n陆=lu4\r\n栾=luan2\r\n罗=luo2\r\n骆=luo4\r\n麻=ma2\r\n马=ma3\r\n满=man3\r\n毛=mao2\r\n茅=mao2\r\n梅=mei2\r\n蒙=meng2\r\n孟=meng4\r\n糜=mi2\r\n米=mi3\r\n宓=mi4\r\n苗=miao2\r\n缪=miao4\r\n闵=min3\r\n明=ming2\r\n万俟=mo4-qi2\r\n莫=mo4\r\n慕容=mu4-rong2\r\n慕=mu4\r\n牧=mu4\r\n穆=mu4\r\n那=na1\r\n能=nai4\r\n倪=ni2\r\n乜=nie4\r\n聂=nie4\r\n宁=ning4\r\n牛=niu2\r\n钮=niu3\r\n农=nong2\r\n欧阳=ou1-yang2\r\n欧=ou1\r\n潘=pan1\r\n庞=pang2\r\n逄=pang2\r\n裴=pei2\r\n彭=peng2\r\n蓬=peng2\r\n皮=pi2\r\n平=ping2\r\n濮阳=pu2-yang2\r\n濮=pu2\r\n蒲=pu2\r\n浦=pu3\r\n戚=qi1\r\n祁=qi2\r\n齐=qi2\r\n钱=qian2\r\n强=qiang2\r\n乔=qiao2\r\n秦=qin2\r\n秋=qiu1\r\n邱=qiu1\r\n仇=qiu2\r\n裘=qiu2\r\n屈=qu1\r\n麴=qu1\r\n璩=qu2\r\n瞿=qu2\r\n全=quan2\r\n权=quan2\r\n阙=que1\r\n冉=ran3\r\n饶=rao2\r\n任=ren2\r\n容=rong2\r\n戎=rong2\r\n荣=rong2\r\n融=rong2\r\n茹=ru2\r\n阮=ruan3\r\n芮=rui4\r\n桑=sang1\r\n沙=sha1\r\n山=shan1\r\n单=shan4\r\n上官=shang4-guan1\r\n尚=shang4\r\n韶=shao2\r\n邵=shao4\r\n厍=she4\r\n申屠=shen1-tu2\r\n申=shen1\r\n莘=shen1\r\n沈=shen3\r\n慎=shen4\r\n盛=sheng\r\n师=shi1\r\n施=shi1\r\n时=shi2\r\n石=shi2\r\n史=shi3\r\n寿=shou4\r\n殳=shu1\r\n舒=shu1\r\n束=shu4\r\n双=shuang1\r\n水=shui3\r\n司空=si1-kong1\r\n司马=si1-ma3\r\n司徒=si1-tu2\r\n司=si1\r\n松=song1\r\n宋=song4\r\n苏=su1\r\n宿=su4\r\n孙=sun1\r\n索=suo3\r\n邰=tai2\r\n太叔=tai4-shu1\r\n澹台=tan2-tai2\r\n谈=tan2\r\n谭=tan2\r\n汤=tang1\r\n唐=tang2\r\n陶=tao2\r\n滕=teng2\r\n田=tian2\r\n通=tong1\r\n佟=tong2\r\n童=tong2\r\n钭=tou3\r\n屠=tu2\r\n万=wan4\r\n汪=wang1\r\n王=wang2\r\n危=wei1\r\n韦=wei2\r\n卫=wei4\r\n蔚=wei4\r\n魏=wei4\r\n温=wen1\r\n闻人=wen2-ren2\r\n文=wen2\r\n闻=wen2\r\n翁=weng1\r\n沃=wo4\r\n乌=wu1\r\n巫=wu1\r\n邬=wu1\r\n吴=wu2\r\n毋=wu2\r\n伍=wu3\r\n武=wu3\r\n奚=xi1\r\n郗=xi1\r\n习=xi2\r\n席=xi2\r\n郤=xi4\r\n夏侯=xia4-hou2\r\n夏=xia4\r\n鲜于=xian1-yu2\r\n咸=xian2\r\n向=xiang4\r\n相=xiang4\r\n项=xiang4\r\n萧=xiao1\r\n解=xie4\r\n谢=xie4\r\n辛=xin1\r\n邢=xing2\r\n幸=xing4\r\n熊=xiong2\r\n胥=xu1\r\n须=xu1\r\n徐=xu2\r\n许=xu3\r\n轩辕=xuan1-yuan2\r\n宣=xuan1\r\n薛=xue1\r\n荀=xun2\r\n燕=yan1\r\n严=yan2\r\n言=yan2\r\n阎=yan2\r\n颜=yan2\r\n晏=yan4\r\n杨=yang2\r\n羊=yang2\r\n仰=yang3\r\n养=yang3\r\n姚=yao2\r\n叶=ye4\r\n伊=yi1\r\n易=yi4\r\n益=yi4\r\n羿=yi4\r\n殷=yin1\r\n阴=yin1\r\n尹=yin3\r\n印=yin4\r\n应=ying1\r\n雍=yong1\r\n尤=you2\r\n游=you2\r\n於=yu1\r\n于=yu2\r\n余=yu2\r\n俞=yu2\r\n虞=yu2\r\n鱼=yu2\r\n宇文=yu3-wen2\r\n庾=yu3\r\n禹=yu3\r\n尉迟=yu4-chi2\r\n喻=yu4\r\n郁=yu4\r\n元=yuan2\r\n袁=yuan2\r\n乐=yue4\r\n越=yue4\r\n云=yun2\r\n宰=zai3\r\n昝=zan3\r\n臧=zang1\r\n曾=zeng1\r\n查=zha1\r\n翟=zhai2\r\n詹=zhan1\r\n湛=zhan4\r\n张=zhang1\r\n章=zhang1\r\n长孙=zhang3-sun1\r\n赵=zhao4\r\n甄=zhen1\r\n郑=zheng4\r\n支=zhi1\r\n钟离=zhong1-li2\r\n仲孙=zhong1-sun1\r\n终=zhong1\r\n钟=zhong1\r\n仲=zhong4\r\n周=zhou1\r\n诸葛=zhu1-ge3\r\n朱=zhu1\r\n诸=zhu1\r\n竺=zhu2\r\n祝=zhu4\r\n庄=zhuang1\r\n卓=zhuo2\r\n訾=zi3\r\n宗=zong1\r\n邹=zou1\r\n祖=zu3\r\n左=zuo3";
        private static NameDB instance;
        private readonly Dictionary<string, string> map = new Dictionary<string, string>();

        // Methods
        private NameDB()
        {
            this.loadResource();
        }

        public string[] GetHanzi(string pinyin, bool matchAll)
        {
            Regex reg = new Regex("[0-9]");
            if (matchAll)
            {
                return Enumerable.ToArray<string>(Enumerable.Select<KeyValuePair<string, string>, string>(from item in (IEnumerable<KeyValuePair<string, string>>)this.map select item, s_c.s_9__7_1 ?? (s_c.s_9__7_1 = new Func<KeyValuePair<string, string>, string>(s_c.s_9.GetHanzib__7_1))));
            }
            return Enumerable.ToArray<string>(Enumerable.Select<KeyValuePair<string, string>, string>(from item in (IEnumerable<KeyValuePair<string, string>>)this.map select item, s_c.s_9__7_3 ?? (s_c.s_9__7_3 = new Func<KeyValuePair<string, string>, string>(s_c.s_9.GetHanzib__7_3))));
        }

        public string GetPinyin(string hanzi)
        {
            if (!this.map.ContainsKey(hanzi))
            {
                return null;
            }
            // this.map[hanzi];
            return this.map[hanzi];
            //  this.map.get_Item(hanzi);
        }

        private void loadResource()
        {
            char[] separator = new char[] { '\n' };
            using (IEnumerator<string> enumerator = Enumerable.Where<string>("艾=ai4\r\n安=an1\r\n敖=ao2\r\n巴=ba1\r\n白=bai2\r\n柏=bai3\r\n班=ban1\r\n包=bao1\r\n暴=bao4\r\n鲍=bao4\r\n贝=bei4\r\n贲=ben1\r\n毕=bi4\r\n边=bian1\r\n卞=bian4\r\n别=bie2\r\n邴=bing3\r\n薄=bo2\r\n卜=bu3\r\n步=bu4\r\n蔡=cai4\r\n苍=cang1\r\n曹=cao2\r\n岑=cen2\r\n柴=chai2\r\n单于=chan2-yu2\r\n昌=chang1\r\n常=chang2\r\n巢=chao2\r\n晁=chao2\r\n车=che1\r\n陈=chen2\r\n成=cheng2\r\n程=cheng2\r\n池=chi2\r\n充=chong1\r\n储=chu3\r\n褚=chu3\r\n淳于=chun2-yu2\r\n从=cong2\r\n崔=cui1\r\n戴=dai4\r\n党=dang3\r\n邓=deng4\r\n狄=di2\r\n第五=di4-wu3\r\n刁=diao1\r\n丁=ding1\r\n东方=dong1-fang1\r\n东=dong1\r\n董=dong3\r\n窦=dou4\r\n都=du1\r\n堵=du3\r\n杜=du4\r\n段=duan4\r\n鄂=e4\r\n樊=fan2\r\n范=fan4\r\n方=fang1\r\n房=fang2\r\n费=fei4\r\n丰=feng1\r\n封=feng1\r\n酆=feng1\r\n冯=feng2\r\n凤=feng4\r\n伏=fu2\r\n扶=fu2\r\n福=fu2\r\n符=fu2\r\n傅=fu4\r\n富=fu4\r\n干=gan1\r\n甘=gan1\r\n高=gao1\r\n郜=gao4\r\n戈=ge1\r\n盖=ge3\r\n葛=ge3\r\n耿=geng3\r\n公孙=gong1-sun1\r\n公羊=gong1-yang2\r\n公冶=gong1-ye3\r\n宗政=gong1-ye3\r\n公=gong1\r\n宫=gong1\r\n弓=gong1\r\n龚=gong1\r\n巩=gong3\r\n贡=gong4\r\n勾=gou1\r\n古=gu3\r\n谷=gu3\r\n顾=gu4\r\n关=guan1\r\n管=guan3\r\n广=guang3\r\n桂=gui4\r\n郭=guo1\r\n国=guo2\r\n韩=han2\r\n杭=hang2\r\n郝=hao3\r\n何=he2\r\n和=he2\r\n赫连=he4-lian2\r\n贺=he4\r\n衡=heng2\r\n弘=hong2\r\n洪=hong2\r\n红=hong2\r\n侯=hou2\r\n後=hou4\r\n胡=hu2\r\n扈=hu4\r\n花=hua1\r\n滑=hua2\r\n华=hua4\r\n怀=huai2\r\n桓=huan2\r\n宦=huan4\r\n皇甫=huang2-fu3\r\n黄=huang2\r\n惠=hui4\r\n霍=huo4\r\n姬=ji1\r\n嵇=ji1\r\n吉=ji2\r\n汲=ji2\r\n纪=ji3\r\n冀=ji4\r\n季=ji4\r\n暨=ji4\r\n蓟=ji4\r\n计=ji4\r\n家=jia1\r\n郏=jia2\r\n贾=jia3\r\n简=jian3\r\n姜=jiang1\r\n江=jiang1\r\n蒋=jiang3\r\n焦=jiao1\r\n金=jin1\r\n靳=jin4\r\n经=jing1\r\n荆=jing1\r\n井=jing3\r\n景=jing3\r\n居=ju1\r\n鞠=ju1\r\n阚=kan4\r\n康=kang1\r\n柯=ke1\r\n空=kong1\r\n孔=kong3\r\n寇=kou4\r\n蒯=kuai3\r\n匡=kuang1\r\n夔=kui2\r\n隗=kui2\r\n赖=lai4\r\n蓝=lan2\r\n郎=lang2\r\n劳=lao2\r\n雷=lei2\r\n冷=leng3\r\n黎=li2\r\n李=li3\r\n利=li4\r\n厉=li4\r\n郦=li4\r\n廉=lian2\r\n连=lian2\r\n梁=liang2\r\n廖=liao4\r\n林=lin2\r\n蔺=lin4\r\n令狐=ling2-hu2\r\n凌=ling2\r\n刘=liu2\r\n柳=liu3\r\n隆=long2\r\n龙=long2\r\n娄=lou2\r\n闾丘=lu2-qiu1\r\n卢=lu2\r\n吕=lu3\r\n鲁=lu3\r\n禄=lu4\r\n路=lu4\r\n逯=lu4\r\n陆=lu4\r\n栾=luan2\r\n罗=luo2\r\n骆=luo4\r\n麻=ma2\r\n马=ma3\r\n满=man3\r\n毛=mao2\r\n茅=mao2\r\n梅=mei2\r\n蒙=meng2\r\n孟=meng4\r\n糜=mi2\r\n米=mi3\r\n宓=mi4\r\n苗=miao2\r\n缪=miao4\r\n闵=min3\r\n明=ming2\r\n万俟=mo4-qi2\r\n莫=mo4\r\n慕容=mu4-rong2\r\n慕=mu4\r\n牧=mu4\r\n穆=mu4\r\n那=na1\r\n能=nai4\r\n倪=ni2\r\n乜=nie4\r\n聂=nie4\r\n宁=ning4\r\n牛=niu2\r\n钮=niu3\r\n农=nong2\r\n欧阳=ou1-yang2\r\n欧=ou1\r\n潘=pan1\r\n庞=pang2\r\n逄=pang2\r\n裴=pei2\r\n彭=peng2\r\n蓬=peng2\r\n皮=pi2\r\n平=ping2\r\n濮阳=pu2-yang2\r\n濮=pu2\r\n蒲=pu2\r\n浦=pu3\r\n戚=qi1\r\n祁=qi2\r\n齐=qi2\r\n钱=qian2\r\n强=qiang2\r\n乔=qiao2\r\n秦=qin2\r\n秋=qiu1\r\n邱=qiu1\r\n仇=qiu2\r\n裘=qiu2\r\n屈=qu1\r\n麴=qu1\r\n璩=qu2\r\n瞿=qu2\r\n全=quan2\r\n权=quan2\r\n阙=que1\r\n冉=ran3\r\n饶=rao2\r\n任=ren2\r\n容=rong2\r\n戎=rong2\r\n荣=rong2\r\n融=rong2\r\n茹=ru2\r\n阮=ruan3\r\n芮=rui4\r\n桑=sang1\r\n沙=sha1\r\n山=shan1\r\n单=shan4\r\n上官=shang4-guan1\r\n尚=shang4\r\n韶=shao2\r\n邵=shao4\r\n厍=she4\r\n申屠=shen1-tu2\r\n申=shen1\r\n莘=shen1\r\n沈=shen3\r\n慎=shen4\r\n盛=sheng\r\n师=shi1\r\n施=shi1\r\n时=shi2\r\n石=shi2\r\n史=shi3\r\n寿=shou4\r\n殳=shu1\r\n舒=shu1\r\n束=shu4\r\n双=shuang1\r\n水=shui3\r\n司空=si1-kong1\r\n司马=si1-ma3\r\n司徒=si1-tu2\r\n司=si1\r\n松=song1\r\n宋=song4\r\n苏=su1\r\n宿=su4\r\n孙=sun1\r\n索=suo3\r\n邰=tai2\r\n太叔=tai4-shu1\r\n澹台=tan2-tai2\r\n谈=tan2\r\n谭=tan2\r\n汤=tang1\r\n唐=tang2\r\n陶=tao2\r\n滕=teng2\r\n田=tian2\r\n通=tong1\r\n佟=tong2\r\n童=tong2\r\n钭=tou3\r\n屠=tu2\r\n万=wan4\r\n汪=wang1\r\n王=wang2\r\n危=wei1\r\n韦=wei2\r\n卫=wei4\r\n蔚=wei4\r\n魏=wei4\r\n温=wen1\r\n闻人=wen2-ren2\r\n文=wen2\r\n闻=wen2\r\n翁=weng1\r\n沃=wo4\r\n乌=wu1\r\n巫=wu1\r\n邬=wu1\r\n吴=wu2\r\n毋=wu2\r\n伍=wu3\r\n武=wu3\r\n奚=xi1\r\n郗=xi1\r\n习=xi2\r\n席=xi2\r\n郤=xi4\r\n夏侯=xia4-hou2\r\n夏=xia4\r\n鲜于=xian1-yu2\r\n咸=xian2\r\n向=xiang4\r\n相=xiang4\r\n项=xiang4\r\n萧=xiao1\r\n解=xie4\r\n谢=xie4\r\n辛=xin1\r\n邢=xing2\r\n幸=xing4\r\n熊=xiong2\r\n胥=xu1\r\n须=xu1\r\n徐=xu2\r\n许=xu3\r\n轩辕=xuan1-yuan2\r\n宣=xuan1\r\n薛=xue1\r\n荀=xun2\r\n燕=yan1\r\n严=yan2\r\n言=yan2\r\n阎=yan2\r\n颜=yan2\r\n晏=yan4\r\n杨=yang2\r\n羊=yang2\r\n仰=yang3\r\n养=yang3\r\n姚=yao2\r\n叶=ye4\r\n伊=yi1\r\n易=yi4\r\n益=yi4\r\n羿=yi4\r\n殷=yin1\r\n阴=yin1\r\n尹=yin3\r\n印=yin4\r\n应=ying1\r\n雍=yong1\r\n尤=you2\r\n游=you2\r\n於=yu1\r\n于=yu2\r\n余=yu2\r\n俞=yu2\r\n虞=yu2\r\n鱼=yu2\r\n宇文=yu3-wen2\r\n庾=yu3\r\n禹=yu3\r\n尉迟=yu4-chi2\r\n喻=yu4\r\n郁=yu4\r\n元=yuan2\r\n袁=yuan2\r\n乐=yue4\r\n越=yue4\r\n云=yun2\r\n宰=zai3\r\n昝=zan3\r\n臧=zang1\r\n曾=zeng1\r\n查=zha1\r\n翟=zhai2\r\n詹=zhan1\r\n湛=zhan4\r\n张=zhang1\r\n章=zhang1\r\n长孙=zhang3-sun1\r\n赵=zhao4\r\n甄=zhen1\r\n郑=zheng4\r\n支=zhi1\r\n钟离=zhong1-li2\r\n仲孙=zhong1-sun1\r\n终=zhong1\r\n钟=zhong1\r\n仲=zhong4\r\n周=zhou1\r\n诸葛=zhu1-ge3\r\n朱=zhu1\r\n诸=zhu1\r\n竺=zhu2\r\n祝=zhu4\r\n庄=zhuang1\r\n卓=zhuo2\r\n訾=zi3\r\n宗=zong1\r\n邹=zou1\r\n祖=zu3\r\n左=zuo3".Split(separator), s_c.s_9__5_0 ?? (s_c.s_9__5_0 = new Func<string, bool>(s_c.s_9.loadResourceb__5_0))).GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    char[] chArray2 = new char[] { '=' };
                    string[] textArray1 = enumerator.Current.Split(chArray2);
                    string str = textArray1[0];
                    string str2 = textArray1[1].Trim();
                    this.map.Add(str, str2.Replace('-', ' '));
                }
            }
        }

        // Properties
        public static NameDB Instance
        {
            get
            {
                return (instance ?? (instance = new NameDB()));
            }
        }

        // Nested Types
        [Serializable, CompilerGenerated]
        private sealed class s_c
        {
            // Fields
            public static readonly NameDB.s_c s_9 = new NameDB.s_c();
            public static Func<string, bool> s_9__5_0;
            public static Func<KeyValuePair<string, string>, string> s_9__7_1;
            public static Func<KeyValuePair<string, string>, string> s_9__7_3;

            // Methods
            internal string GetHanzib__7_1(KeyValuePair<string, string> item)
            {
                return item.Key;
            }

            internal string GetHanzib__7_3(KeyValuePair<string, string> item)
            {
                return item.Key;
            }

            internal bool loadResourceb__5_0(string buf)
            {
                return !string.IsNullOrEmpty(buf);
            }
        }
    }


}
