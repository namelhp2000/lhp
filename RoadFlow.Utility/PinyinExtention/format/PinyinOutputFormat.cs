using RoadFlow.Utility;
using System;

namespace RoadFlow.Pinyin.format
{
    public class PinyinOutputFormat
    {
        // Methods
        public PinyinOutputFormat()
        {
            this.GetToneFormat = ToneFormat.WITH_TONE_MARK;
            this.GetCaseFormat = CaseFormat.LOWERCASE;
            this.GetVCharFormat = VCharFormat.WITH_U_UNICODE;
        }

        public PinyinOutputFormat(ToneFormat toneFormat, CaseFormat caseFormat, VCharFormat vCharFormat)
        {
            this.SetFormat(toneFormat, caseFormat, vCharFormat);
        }

        public PinyinOutputFormat(string toneFormat, string caseFormat, string vCharFormat)
        {
            this.SetFormat(toneFormat, caseFormat, vCharFormat);
        }

        public void SetFormat(ToneFormat toneFormat, CaseFormat caseFormat, VCharFormat vCharFormat)
        {
            this.GetToneFormat = toneFormat;
            this.GetCaseFormat = caseFormat;
            this.GetVCharFormat = vCharFormat;
        }

        public void SetFormat(string toneFormat, string caseFormat, string vCharFormat)
        {
            if (!string.IsNullOrEmpty(toneFormat))
            {
                this.GetToneFormat = (ToneFormat)Enum.Parse((Type)typeof(ToneFormat), toneFormat);
            }
            if (!string.IsNullOrEmpty(caseFormat))
            {
                this.GetCaseFormat = (CaseFormat)Enum.Parse((Type)typeof(CaseFormat), caseFormat);
            }
            if (!string.IsNullOrEmpty(vCharFormat))
            {
                this.GetVCharFormat = (VCharFormat)Enum.Parse((Type)typeof(VCharFormat), vCharFormat);
            }
        }

        // Properties
        public CaseFormat GetCaseFormat { get; private set; }

        public ToneFormat GetToneFormat { get; private set; }

        public VCharFormat GetVCharFormat { get; private set; }
    }


}
