using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Data;

namespace RoadFlow.Business
{


    #region 2.8.7方法
    public class Wildcard
    {
        // Fields
        private static readonly string[] wildcardList;

        // Methods
        static Wildcard()
        {
            string[] textArray1 = new string[] {
            "{<UserId>}", "{<UserName>}", "{<UserDeptId>}", "{<UserDeptName>}", "{<UserStationId>}", "{<UserStationName>}", "{<UserWorkGroupId>}", "{<UserWorkGroupName>}", "{<UserDeptLeaderId>}", "{<UserDeptLeaderName>}", "{<UserCharegLeaderId>}", "{<UserCharegLeaderName>}", "{<UserUnitId>}", "{<UserUnitName>}", "{<InitiatorId>}", "{<InitiatorName>}",
            "{<InitiatorDeptId>}", "{<InitiatorDeptName>}", "{<InitiatorStationId>}", "{<InitiatorStationName>}", "{<InitiatorRoleId>}", "{<InitiatorRoleName>}", "{<InitiatorUnitId>}", "{<InitiatorUnitName>}", "{<InitiatorLeaderId>}", "{<InitiatorLeaderName>}", "{<InitiatorCharegId>}", "{<InitiatorCharegName>}", "{<ShortDate>}", "{<LongDate>}", "{<ShortDateTime>}", "{<LongDateTime>}",
            "{<ShortDateTimeSecond>}", "{<LongDateTimeSecond>}", "{<FlowId>}", "{<FlowName>}", "{<StepId>}", "{<StepName>}", "{<TaskId>}", "{<InstanceId>}", "{<GroupId>}", "{<PrevInstanceId>}", "{<PrevFlowTitle>}", "{<Guid>}", "{<EmptyGuid>}", "{Query<", "{Form<", "{DataRow<",
            "{Date<", "{Method<", "{JArray<", "{JObject<"
         };
            wildcardList = textArray1;
        }

        public static string Filter(string sqlstr, RoadFlow.Model. User user = null, object obj = null)
        {
            if (sqlstr.IsNullOrWhiteSpace())
            {
                return "";
            }
            HttpRequest request = Tools.HttpContext.Request;
            foreach (string wildcardstr in wildcardList)
            {
                while (sqlstr.ContainsIgnoreCase(wildcardstr))
                {
                    string str4 = string.Empty;
                    string oldStr = wildcardstr;
                    if ("{Query<".EqualsIgnoreCase(wildcardstr))
                    {
                        string str6 = sqlstr.Substring(sqlstr.IndexOf("{Query<") + 7);
                        string str7 = str6.Substring(0, str6.IndexOf(">}"));
                        if (!str7.IsNullOrWhiteSpace())
                        {
                            oldStr = wildcardstr + str7 + ">}";
                            str4 = request.Querys(str7);
                        }
                    }
                    else if ("{Form<".EqualsIgnoreCase(wildcardstr))
                    {
                        string str8 = sqlstr.Substring(sqlstr.IndexOf("{Form<") + 6);
                        string str9 = str8.Substring(0, str8.IndexOf(">}"));
                        if (!str9.IsNullOrWhiteSpace())
                        {
                            oldStr = wildcardstr + str9 + ">}";
                            str4 = request.Forms(str9);
                        }
                    }
                    else if ("{DataRow<".EqualsIgnoreCase(wildcardstr))
                    {
                        string str10 = sqlstr.Substring(sqlstr.IndexOf("{DataRow<") + 9);
                        string str11 = str10.Substring(0, str10.IndexOf(">}"));
                        if (!str11.IsNullOrWhiteSpace())
                        {
                            oldStr = wildcardstr + str11 + ">}";
                            DataRow row = (DataRow)obj;
                            try
                            {
                                str4 = row[str11].ToString();
                            }
                            catch
                            {
                                str4 = "";
                            }
                        }
                    }
                    else if ("{Method<".EqualsIgnoreCase(wildcardstr))
                    {
                        string str12 = sqlstr.Substring(sqlstr.IndexOf("{Method<") + 8);
                        string str13 = str12.Substring(0, str12.IndexOf(">}"));
                        if (!str13.IsNullOrWhiteSpace())
                        {
                            oldStr = wildcardstr + str13 + ">}";
                            ValueTuple<object, Exception> tuple1 = (obj == null) ? Tools.ExecuteMethod(str13, Array.Empty<object>()) : Tools.ExecuteMethod(str13, new object[] { obj });
                            object obj2 = tuple1.Item1;
                            Exception exception = tuple1.Item2;
                            str4 = (obj2 == null) ? "" : obj2.ToString();
                        }
                    }
                    else if ("{Date<".EqualsIgnoreCase(wildcardstr))
                    {
                        string str14 = sqlstr.Substring(sqlstr.IndexOf("{Date<") + 6);
                        string str15 = str14.Substring(0, str14.IndexOf(">}"));
                        if (!str15.IsNullOrWhiteSpace())
                        {
                            oldStr = wildcardstr + str15 + ">}";
                            str4 = DateTimeExtensions.Now.ToString(str15);
                        }
                    }
                    else if ("{Object<".EqualsIgnoreCase(wildcardstr))
                    {
                        string str16 = sqlstr.Substring(sqlstr.IndexOf("{Object<") + 8);
                        string str17 = str16.Substring(0, str16.IndexOf(">}"));
                        if (!str17.IsNullOrWhiteSpace())
                        {
                            oldStr = wildcardstr + str17 + ">}";
                            str4 = (obj == null) ? "" : obj.ToString();
                        }
                    }
                    else if ("{JArray<".EqualsIgnoreCase(wildcardstr))
                    {
                        string str18 = sqlstr.Substring(sqlstr.IndexOf("{JArray<") + 8);
                        string str19 = str18.Substring(0, str18.IndexOf(">}"));
                        if (!str19.IsNullOrWhiteSpace())
                        {
                            oldStr = wildcardstr + str19 + ">}";
                            JArray array = (JArray)obj;
                            if (array != null)
                            {
                                foreach (JObject obj3 in array)
                                {
                                    if (obj3.ContainsKey(str19))
                                    {
                                        str4 = obj3.Value<string>(str19);
                                        break;
                                    }
                                }
                                if (str4.IsNullOrEmpty())
                                {
                                    foreach (JObject obj4 in array)
                                    {
                                        if (obj4.ContainsKey("name") && obj4.Value<string>("name").Equals(str19))
                                        {
                                            str4 = obj4.Value<string>("value");
                                            break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                str4 = "";
                            }
                        }
                    }
                    else if ("{JObject<".EqualsIgnoreCase(wildcardstr))
                    {
                        string str20 = sqlstr.Substring(sqlstr.IndexOf("{JObject<") + 9);
                        string str21 = str20.Substring(0, str20.IndexOf(">}"));
                        if (!str21.IsNullOrWhiteSpace())
                        {
                            oldStr = wildcardstr + str21 + ">}";
                            JObject obj5 = (JObject)obj;
                            if ((obj5 != null) && obj5.ContainsKey(str21))
                            {
                                str4 = obj5.Value<string>(str21);
                            }
                            else
                            {
                                str4 = "";
                            }
                        }
                    }
                    else
                    {
                        str4 = GetWildcardValue(wildcardstr, user, obj);
                    }
                    sqlstr = sqlstr.ReplaceIgnoreCase(oldStr, str4);
                }
            }
            return sqlstr;
        }

        public static string GetWildcardValue(string wildcard, RoadFlow.Model.User userModel, object obj)
        {
            Organize organize = new Organize();
            User user = new User();
            HttpRequest request = Tools.HttpContext.Request;
            string s = wildcard.ToLower();
            switch (PrivateImplementationDetails.ComputeStringHash(s))
            {
                case 0xfb50680:
                    if (s == "{<shortdatetimesecond>}")
                    {
                        return DateTimeExtensions.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    break;

                case 0x10bea57c:
                    if (s == "{<stepid>}")
                    {
                        return request.Querys("stepid");
                    }
                    break;

                case 0x10cb3b9b:
                    if (s == "{<taskid>}")
                    {
                        return request.Querys("taskid");
                    }
                    break;

                case 0x3e5075b:
                    if (s == "{<initiatorworkgroupname>}")
                    {
                        Guid firstSenderId = new FlowTask().GetFirstSenderId(request.Querys("groupid").ToString().ToGuid());
                        userModel = userModel ?? User.CurrentUser;
                        if (firstSenderId.IsEmptyGuid() && (userModel != null))
                        {
                            firstSenderId = userModel.Id;
                        }
                        return user.GetWorkGroupsName(firstSenderId);
                    }
                    break;

                case 0x8f780d9:
                    if (s == "{<initiatorunitid>}")
                    {
                        Guid id = new FlowTask().GetFirstSenderId(request.Querys("groupid").ToGuid());
                        userModel = userModel ?? User.CurrentUser;
                        if (id.IsEmptyGuid() && (userModel != null))
                        {
                            id = userModel.Id;
                        }
                       RoadFlow.Model. Organize unit = user.GetUnit(id.ToString());
                        return ((unit == null) ? "" : unit.Id.ToString());
                    }
                    break;

                case 0x2d475495:
                    if (s == "{<userid>}")
                    {
                        userModel = userModel ?? User.CurrentUser;
                        return ((userModel == null) ? "" : userModel.Id.ToUpperString());
                    }
                    break;

                case 0x2dd6a823:
                    if (s == "{<initiatorunitname>}")
                    {
                        Guid guid10 = new FlowTask().GetFirstSenderId(request.Querys("groupid").ToGuid());
                        userModel = userModel ?? User.CurrentUser;
                        if (guid10.IsEmptyGuid() && (userModel != null))
                        {
                            guid10 = userModel.Id;
                        }
                        RoadFlow.Model.Organize organize13 = user.GetUnit(guid10.ToString());
                        return ((organize13 == null) ? "" : organize13.Name);
                    }
                    break;

                case 0x2fcf8199:
                    if (s == "{<initiatorstationid>}")
                    {
                        userModel = userModel ?? User.CurrentUser;
                        Guid guid5 = new FlowTask().GetFirstSenderId(request.Querys("groupid").ToGuid());
                        if (guid5.IsEmptyGuid() && (userModel != null))
                        {
                            guid5 = userModel.Id;
                        }
                        RoadFlow.Model.Organize station = user.GetStation(guid5.ToString());
                        return ((station == null) ? "" : station.Id.ToString());
                    }
                    break;

                case 0x350fffa7:
                    if (s == "{<userworkgroupid>}")
                    {
                        userModel = userModel ?? User.CurrentUser;
                        return ((userModel == null) ? "" : user.GetWorkGroupsId(userModel.Id));
                    }
                    break;

                case 0x3cafd1be:
                    if (s == "{<flowid>}")
                    {
                        return request.Querys("flowid");
                    }
                    break;

                case 0x46d4c591:
                    if (s == "{<initiatorcharegname>}")
                    {
                        Guid guid14 = new FlowTask().GetFirstSenderId(request.Querys("groupid").ToGuid());
                        userModel = userModel ?? User.CurrentUser;
                        if (guid14.IsEmptyGuid() && (userModel != null))
                        {
                            guid14 = userModel.Id;
                        }
                        return user.GetNames(user.GetLeader(guid14.ToString()).Item2);
                    }
                    break;

                case 0x624c18fd:
                    if (s == "{<userworkgroupname>}")
                    {
                        userModel = userModel ?? User.CurrentUser;
                        return ((userModel == null) ? "" : user.GetWorkGroupsName(userModel.Id));
                    }
                    break;

                case 0x62d9f6a2:
                    if (s == "{<usercharegleadername>}")
                    {
                        userModel = userModel ?? User.CurrentUser;
                        return ((userModel == null) ? "" : user.GetNames(user.GetLeader(userModel.Id.ToString()).Item2));
                    }
                    break;

                case 0x71c444eb:
                    if (s == "{<initiatorcharegid>}")
                    {
                        Guid guid13 = new FlowTask().GetFirstSenderId(request.Querys("groupid").ToGuid());
                        userModel = userModel ?? User.CurrentUser;
                        if (guid13.IsEmptyGuid() && (userModel != null))
                        {
                            guid13 = userModel.Id;
                        }
                        return user.GetLeader(guid13.ToString()).Item2;
                    }
                    break;

                case 0x497577fb:
                    if (s == "{<emptyguid>}")
                    {
                        return Guid.Empty.ToString();
                    }
                    break;

                case 0x5fecd9e0:
                    if (s == "{<usercharegleaderid>}")
                    {
                        userModel = userModel ?? User.CurrentUser;
                        return ((userModel == null) ? "" : user.GetLeader(userModel.Id.ToString()).Item2);
                    }
                    break;

                case 0x75f55b19:
                    if (s == "{<userunitname>}")
                    {
                        userModel = userModel ?? User.CurrentUser;
                        if (userModel == null)
                        {
                            return "";
                        }
                        RoadFlow.Model.Organize organize7 = user.GetUnit(userModel.Id.ToString());
                        return ((organize7 == null) ? "" : organize7.Name);
                    }
                    break;

                case 0x781f1b7e:
                    if (s == "{<stepname>}")
                    {
                        Guid guid16;
                        Guid guid17;
                        string str = request.Querys("flowid");
                        string str4 = request.Querys("stepid");
                        return ((str.IsGuid(out guid16) && str4.IsGuid(out guid17)) ? new Flow().GetStepName(guid16, guid17) : string.Empty);
                    }
                    break;

                case 0x78ea68bd:
                    if (s == "{<instanceid>}")
                    {
                        return request.Querys("instanceid");
                    }
                    break;

                case 0x79f7ca5d:
                    if (s == "{<longdate>}")
                    {
                        return DateTimeExtensions.Now.ToString("yyyy年MM月dd日");
                    }
                    break;

                case 0x8c3c5146:
                    if (s == "{<initiatorleaderid>}")
                    {
                        Guid guid11 = new FlowTask().GetFirstSenderId(request.Querys("groupid").ToGuid());
                        userModel = userModel ?? User.CurrentUser;
                        if (guid11.IsEmptyGuid() && (userModel !=null))
                        {
                            guid11 = userModel.Id;
                        }
                        return user.GetLeader(guid11.ToString()).Item1;
                    }
                    break;

                case 0x8e04e184:
                    if (s == "{<userdeptname>}")
                    {
                        userModel = userModel ?? User.CurrentUser;
                        if (userModel == null)
                        {
                            return "";
                        }
                        RoadFlow.Model.Organize dept = user.GetDept(userModel.Id.ToString());
                        return ((dept == null) ? "" : dept.Name);
                    }
                    break;

                case 0xa1ce3883:
                    if (s == "{<userunitid>}")
                    {
                        userModel = userModel ?? User.CurrentUser;
                        if (userModel == null)
                        {
                            return "";
                        }
                        RoadFlow.Model.Organize organize6 = user.GetUnit(userModel.Id.ToString());
                        return ((organize6 == null) ? "" : organize6.Id.ToUpperString());
                    }
                    break;

                case 0xa2e41977:
                    if (s == "{<userstationid>}")
                    {
                        userModel = userModel ?? User.CurrentUser;
                        if (userModel == null)
                        {
                            return "";
                        }
                        RoadFlow.Model.Organize organize4 = user.GetStation(userModel.Id.ToString());
                        return ((organize4 == null) ? "" : organize4.Id.ToUpperString());
                    }
                    break;

                case 0xa41dc6cd:
                    if (s == "{<userstationname>}")
                    {
                        userModel = userModel ?? User.CurrentUser;
                        if (userModel == null)
                        {
                            return "";
                        }
                        RoadFlow.Model.Organize organize5 = user.GetStation(userModel.Id.ToString());
                        return ((organize5 == null) ? "" : organize5.Name);
                    }
                    break;

                case 0x9039b495:
                    if (s == "{<userdeptleadername>}")
                    {
                        userModel = userModel ?? User.CurrentUser;
                        return ((userModel == null) ? "" : user.GetNames(user.GetLeader(userModel.Id.ToString()).Item1));
                    }
                    break;

                case 0x9a27108f:
                    if (s == "{<userdeptleaderid>}")
                    {
                        userModel = userModel ?? User.CurrentUser;
                        return ((userModel == null) ? "" : user.GetLeader(userModel.Id.ToString()).Item1);
                    }
                    break;

                case 0xb0a85295:
                    if (s == "{<initiatorname>}")
                    {
                        userModel = userModel ?? User.CurrentUser;
                        Guid guid2 = new FlowTask().GetFirstSenderId(request.Querys("groupid").ToGuid());
                        return ((guid2.IsEmptyGuid() && (userModel != null)) ? userModel.Name : user.GetName(guid2));
                    }
                    break;

                case 0xb4f7e74e:
                    if (s == "{<prevflowtitle>}")
                    {
                        return new FlowTask().GetPrevTitle(request.Querys("taskid"));
                    }
                    break;

                case 0xbb99aaa8:
                    if (s == "{<longdatetimesecond>}")
                    {
                        return DateTimeExtensions.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒");
                    }
                    break;

                case 0xbeac803e:
                    if (s == "{<guid>}")
                    {
                        return Guid.NewGuid().ToString();
                    }
                    break;

                case 0xc0f6be40:
                    if (s == "{<longdatetime>}")
                    {
                        return DateTimeExtensions.Now.ToString("yyyy年MM月dd日 HH时mm分");
                    }
                    break;

                case 0xc1319961:
                    if (s == "{<initiatorworkgroupid>}")
                    {
                        Guid guid7 = new FlowTask().GetFirstSenderId(request.Querys("groupid").ToGuid());
                        userModel = userModel ?? User.CurrentUser;
                        if (guid7.IsEmptyGuid() && (userModel!= null))
                        {
                            guid7 = userModel.Id;
                        }
                        return user.GetWorkGroupsId(guid7);
                    }
                    break;

                case 0xda0f959c:
                    if (s == "{<flowname>}")
                    {
                        Guid guid15;
                        return (!request.Querys("flowid").IsGuid(out guid15) ? "" : new Flow().GetName(guid15));
                    }
                    break;

                case 0xdedf02e3:
                    if (s == "{<initiatorstationname>}")
                    {
                        Guid guid6 = new FlowTask().GetFirstSenderId(request.Querys("groupid").ToGuid());
                        userModel = userModel ?? User.CurrentUser;
                        if (guid6.IsEmptyGuid() && (userModel!= null))
                        {
                            guid6 = userModel.Id;
                        }
                        RoadFlow.Model.Organize organize11 = user.GetStation(guid6.ToString());
                        return ((organize11 == null) ? "" : organize11.Name);
                    }
                    break;

                case 0xe67b4eba:
                    if (s == "{<initiatordeptname>}")
                    {
                        userModel = userModel ?? User.CurrentUser;
                        Guid guid4 = new FlowTask().GetFirstSenderId(request.Querys("groupid").ToGuid());
                        if (guid4.IsEmptyGuid() && (userModel != null))
                        {
                            guid4 = userModel.Id;
                        }
                        RoadFlow.Model.Organize organize9 = user.GetDept(guid4.ToString());
                        return ((organize9 == null) ? "" : organize9.Name);
                    }
                    break;

                case 0xce9f8408:
                    if (s == "{<initiatordeptid>}")
                    {
                        userModel = userModel ?? User.CurrentUser;
                        Guid guid3 = new FlowTask().GetFirstSenderId(request.Querys("groupid").ToGuid());
                        if (guid3.IsEmptyGuid() && (userModel != null))
                        {
                            guid3 = userModel.Id;
                        }
                        RoadFlow.Model.Organize organize8 = user.GetDept(guid3.ToString());
                        return ((organize8 == null) ? "" : organize8.Id.ToString());
                    }
                    break;

                case 0xd96c4676:
                    if (s == "{<userdeptid>}")
                    {
                        userModel = userModel ?? User.CurrentUser;
                        if (userModel == null)
                        {
                            return "";
                        }
                        RoadFlow.Model.Organize organize2 = user.GetDept(userModel.Id.ToString());
                        return ((organize2 == null) ? "" : organize2.Id.ToUpperString());
                    }
                    break;

                case 0xd9f30c18:
                    if (s == "{<shortdatetime>}")
                    {
                        return DateTimeExtensions.Now.ToString("yyyy-MM-dd HH:mm");
                    }
                    break;

                case 0xed4ce8d4:
                    if (s == "{<initiatorleadername>}")
                    {
                        Guid guid12 = new FlowTask().GetFirstSenderId(request.Querys("groupid").ToGuid());
                        userModel = userModel ?? User.CurrentUser;
                        if (guid12.IsEmptyGuid() && (userModel != null))
                        {
                            guid12 = userModel.Id;
                        }
                        return user.GetNames(user.GetLeader(guid12.ToString()).Item1);
                    }
                    break;

                case 0xee48237f:
                    if (s == "{<username>}")
                    {
                        userModel = userModel ?? User.CurrentUser;
                        return ((userModel == null) ? "" : userModel.Name);
                    }
                    break;

                case 0xf340dd75:
                    if (s == "{<shortdate>}")
                    {
                        return DateTimeExtensions.Now.ToString("yyyy-MM-dd");
                    }
                    break;

                case 0xf90386b8:
                    if (s == "{<previnstanceid>}")
                    {
                        return new FlowTask().GetPrevInstanceID(request.Querys("taskid"));
                    }
                    break;

                case 0xf9a0fe8f:
                    if (s == "{<initiatorid>}")
                    {
                        userModel = userModel ?? User.CurrentUser;
                        Guid guid = new FlowTask().GetFirstSenderId(request.Querys("groupid").ToGuid());
                        return ((guid.IsEmptyGuid() && (userModel != null)) ? userModel.Id.ToString() : guid.ToString());
                    }
                    break;

                case 0xfa78dfa5:
                    if (s == "{<groupid>}")
                    {
                        return request.Querys("groupid");
                    }
                    break;
            }
            return "";
        }
    }

    #endregion

}