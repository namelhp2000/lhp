using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace RoadFlow.Business
{

   
    public class Menu
    {
        // Fields
        private readonly RoadFlow.Data.Menu menuData = new RoadFlow.Data.Menu();

        // Methods
        public int Add(RoadFlow.Model.Menu menu)
        {
            return this.menuData.Add(menu);
        }

        public int Delete(RoadFlow.Model.Menu[] menus)
        {
            return this.menuData.Delete(menus);
        }

        public RoadFlow.Model.Menu Get(Guid id)
        {
            return this.menuData.Get(id);
        }

        private string GetAddress(DataRow dr, string paramsMenuUsers = "")
        {
            string address = dr["Address"].ToString().Trim();
            string str2 = dr["Params"].ToString().Trim();
            return this.GetAddress(address, str2, paramsMenuUsers);
        }

        public string GetAddress(string address, string params1, string paramsMenuUsers = "")
        {
            StringBuilder builder = new StringBuilder();
            if (params1.IsNullOrWhiteSpace() && paramsMenuUsers.IsNullOrWhiteSpace())
            {
                return address;
            }
            if (!params1.IsNullOrWhiteSpace())
            {
                if (builder.Length > 0)
                {
                    builder.Append('&');
                }
                char[] trimChars = new char[] { '?' };
                char[] chArray2 = new char[] { '&' };
                char[] chArray3 = new char[] { '&' };
                char[] chArray4 = new char[] { '?' };
                builder.Append(params1.TrimStart(trimChars).TrimStart(chArray2).TrimEnd(chArray3).TrimEnd(chArray4));
            }
            if (!paramsMenuUsers.IsNullOrWhiteSpace())
            {
                if (builder.Length > 0)
                {
                    builder.Append('&');
                }
                char[] chArray5 = new char[] { '?' };
                char[] chArray6 = new char[] { '&' };
                char[] chArray7 = new char[] { '&' };
                char[] chArray8 = new char[] { '?' };
                builder.Append(paramsMenuUsers.TrimStart(chArray5).TrimStart(chArray6).TrimEnd(chArray7).TrimEnd(chArray8));
            }
            if (!address.Contains("?"))
            {
                return (address + "?" + builder.ToString());
            }
            return (address + "&" + builder.ToString());
        }

        public List<RoadFlow.Model.Menu> GetAll()
        {
            return this.menuData.GetAll();
        }

        public List<RoadFlow.Model.Menu> GetChilds(Guid id)
        {
            List<RoadFlow.Model.Menu> childs = this.menuData.GetChilds(id);
            if (!Enumerable.Any<RoadFlow.Model.Menu>((IEnumerable<RoadFlow.Model.Menu>)childs))
            {
                return childs;
            }
            return Enumerable.ToList<RoadFlow.Model.Menu>((IEnumerable<RoadFlow.Model.Menu>)Enumerable.OrderBy<RoadFlow.Model.Menu, int>((IEnumerable<RoadFlow.Model.Menu>)childs, key=>key.Sort));
        }

        public int GetMaxSort(Guid id)
        {
            List<RoadFlow.Model.Menu> childs = this.GetChilds(id);
            if (childs.Count == 0)
            {
                return 5;
            }
            return (Enumerable.Max<RoadFlow.Model.Menu>((IEnumerable<RoadFlow.Model.Menu>)childs,key=>key.Sort) + 5);
        }

        /// <summary>
        /// 获取菜单App数据表
        /// </summary>
        /// <returns></returns>
        public DataTable GetMenuAppDataTable()
        {
            return this.menuData.GetMenuAppDataTable();
        }

        public string GetMenuTreeTableHtml(string orgID, Guid? userId = new Guid?())
        {
            DataTable menuAppDataTable = this.GetMenuAppDataTable();
            List<RoadFlow.Model.MenuUser> all = new MenuUser().GetAll();
            StringBuilder sb = new StringBuilder();
            RoadFlow.Model.Menu root = this.GetRoot();
            if (root == null)
            {
                return string.Empty;
            }
            this.GetMenuTreeTableHtml(sb, menuAppDataTable, root.Id, all, orgID, userId);
            return sb.ToString();
        }

        private void GetMenuTreeTableHtml(StringBuilder sb, DataTable appDt, Guid parentID, List<RoadFlow.Model.MenuUser> menuUsers, string orgId, Guid? userId = new Guid?())
        {
            DataRow[] rowArray = appDt.Select("ParentID='" + parentID.ToString() + "'");
            for (int i = 0; i < rowArray.Length; i++)
            {
                string str2;
                string str3;
                DataRow dr = rowArray[i];
                if ((!userId.HasValue || !userId.HasValue) || (userId.Value.IsEmptyGuid() || this.HasUse(dr["Id"].ToString().ToGuid(), userId.Value, menuUsers, out str2, out str3, false)))
                {
                    RoadFlow.Model.MenuUser user = menuUsers.Find(delegate (RoadFlow.Model.MenuUser p) {
                        return (p.MenuId == dr["Id"].ToString().ToGuid()) && p.Organizes.EqualsIgnoreCase(orgId);
                    });
                    string str = (user != null) ? " checked=\"checked\"" : "";
                    string[] textArray1 = new string[] { "<tr id=\"", dr["Id"].ToString().ToUpper(), "\" data-pid=\"", dr["ParentId"].ToString().ToUpper(), "\">" };
                    sb.Append(string.Concat((string[])textArray1));
                    sb.Append("<td>" + dr["Title"] + "</td>");
                    if (!dr["Ico"].ToString().IsNullOrEmpty())
                    {
                        sb.Append("<td><input type=\"hidden\" name=\"apptype\" value=\"0\"/>" + (dr["Ico"].ToString().IsFontIco() ? ("<i class=\"fa " + dr["Ico"].ToString() + "\" style=\"font-size:14px;\"></i>") : ("<img src=\"" + dr["Ico"] + "\" style=\"vertical-align:middle;\"/>")) + "</td>");
                    }
                    else
                    {
                        sb.Append("<td></td>");
                    }
                    string[] textArray2 = new string[] { "<td style=\"text-align:center\"><input type=\"checkbox\" ", str, " onclick=\"appboxclick(this);\" name=\"menuid\" value=\"", dr["Id"].ToString().ToUpper(), "\"/></td>" };
                    sb.Append(string.Concat((string[])textArray2));
                    sb.Append("<td>");
                    bool flag = dr["AppLibraryId"].ToString().IsGuid();
                    if (flag)
                    {
                        foreach (RoadFlow.Model.AppLibraryButton button in Enumerable.ThenBy<RoadFlow.Model.AppLibraryButton, int>(Enumerable.OrderBy<RoadFlow.Model.AppLibraryButton, int>((IEnumerable<RoadFlow.Model.AppLibraryButton>)new AppLibraryButton().GetListByApplibraryId(dr["AppLibraryId"].ToString().ToGuid()),key=>key.ShowType), key=>key.Sort))
                        {
                            str = ((user != null) && user.Buttons.ContainsIgnoreCase(button.Id.ToString())) ? " checked=\"checked\"" : "";
                            object[] objArray1 = new object[] { "<input type=\"checkbox\" ", str, " onclick=\"buttonboxclick(this);\" style=\"vertical-align:middle;\" id=\"button_", dr["Id"].ToString().ToUpper(), "_", button.Id, "\" name=\"button_", dr["Id"].ToString().ToUpper(), "\" value=\"", button.Id.ToUpperString(), "\"/>" };
                            sb.Append(string.Concat((object[])objArray1));
                            string[] textArray3 = new string[] { "<label style=\"vertical-align:middle;\" for=\"button_", dr["Id"].ToString().ToUpper(), "_", button.Id.ToUpperString(), "\">", button.Name, "</label>" };
                            sb.Append(string.Concat((string[])textArray3));
                        }
                    }
                    sb.Append("</td>");
                    if (flag)
                    {
                        string[] textArray4 = new string[] { "<td><input type=\"text\" class=\"mytext\" style=\"width:95%\" value=\"", (user != null) ? ((string)user.Params) : "", "\" name=\"params_", dr["Id"].ToString().ToUpper(), "\"/></td>" };
                        sb.Append(string.Concat((string[])textArray4));
                    }
                    else
                    {
                        sb.Append("<td>&nbsp;</td>");
                    }
                    sb.Append("</tr>");
                    this.GetMenuTreeTableHtml(sb, appDt, dr["Id"].ToString().ToGuid(), menuUsers, orgId, userId);
                }
            }
        }

        public RoadFlow.Model.Menu GetRoot()
        {
            return this.GetAll().Find(key=>key.ParentId == Guid.Empty);
        }

        public string GetUserMenuChilds(Guid userId, Guid refreshId, string rootDir = "", string isrefresh1 = "0")
        {
            StringBuilder builder = new StringBuilder();
            DataTable menuAppDataTable = this.menuData.GetMenuAppDataTable();
            DataView view1 = menuAppDataTable.DefaultView;
            view1.RowFilter = string.Format("ParentID='{0}'", refreshId);
            view1.Sort = "Sort";
            DataTable table2 = view1.ToTable();
            if (table2.Rows.Count == 0)
            {
                return "[]";
            }
            int count = table2.Rows.Count;
            new StringBuilder("[", count * 100);
            List<RoadFlow.Model.MenuUser> all = new MenuUser().GetAll();
            string source = string.Empty;
            foreach (DataRow row in table2.Rows)
            {
                string str2 = string.Empty;
                if (this.HasUse(row["Id"].ToString().ToGuid(), userId, all, out source, out str2, false))
                {
                    bool flag = false;
                    foreach (DataRow row2 in menuAppDataTable.Select(string.Format("ParentID='{0}'", row["id"].ToString())))
                    {
                        if (this.HasUse(row2["ID"].ToString().ToGuid(), userId, all, out source, out str2, false))
                        {
                            flag = true;
                            break;
                        }
                    }
                    string str = row["IcoColor"].ToString();
                    string[] textArray1 = new string[] {
                    "<div class=\"menulistdiv\" ", ("1" == isrefresh1) ? "data-isrefresh1=\"1\"" : "", " onclick=\"", ("1" == isrefresh1) ? "menuClick(this, 1);" : "menuClick(this, 0);", "\" data-id=\"", row["Id"].ToString().ToUpper(), "\" data-title=\"", row["Title"].ToString().Trim(), "\" data-model=\"", row["OpenMode"].ToString(), "\" data-width=\"", row["Width"].ToString(), "\" data-height=\"", row["Height"].ToString(), "\" data-childs=\"", flag ? "1" : "0",
                    "\" data-url=\"", this.GetAddress(row, str2), "\" data-parent=\"", refreshId.ToString().ToUpper(), "\" style=\"", str.IsNullOrEmpty() ? "" : ((string) ("color:" + str.Trim1() + ";")), "\">"
                 };
                    builder.Append(string.Concat((string[])textArray1));
                    builder.Append("<div class=\"menulistdiv1\">");
                    string str4 = row["Ico"].ToString();
                    if (str4.IsNullOrEmpty())
                    {
                        str4 = row["Ico"].ToString();
                    }
                    if (str4.IsNullOrEmpty())
                    {
                        builder.Append("<i class=\"fa fa-file-text-o\"></i>");
                    }
                    else if (str4.IsFontIco())
                    {
                        builder.Append("<i class=\"fa " + str4 + "\"></i>");
                    }
                    else
                    {
                        builder.Append("<img src=\"" + str4 + "\" style=\"vertical-align:middle\" alt=\"\"/>");
                    }
                    builder.Append("<span style=\"vertical-align:middle\">" + row["Title"].ToString().Trim1() + "</span>");
                    builder.Append("</div>");
                    if (flag)
                    {
                        builder.Append("<div class=\"menulistdiv2\"><i class=\"fa fa-angle-left\"></i></div>");
                    }
                    builder.Append("</div>");
                }
            }
            return builder.ToString();
        }

        public string GetUserMenuHtml(Guid userId)
        {
            DataTable menuAppDataTable = this.menuData.GetMenuAppDataTable();
            if (menuAppDataTable.Rows.Count == 0)
            {
                return "";
            }
            List<RoadFlow.Model.MenuUser> all = new MenuUser().GetAll();
            string source = string.Empty;
            StringBuilder builder = new StringBuilder();
            DataRow[] rowArray = menuAppDataTable.Select(string.Format("ParentId='{0}'", Guid.Empty.ToString()));
            if (rowArray.Length != 0)
            {
                List<RoadFlow.Model.UserShortcut> listByUserId = new UserShortcut().GetListByUserId(userId);
                if (listByUserId.Count > 0)
                {
                    foreach (RoadFlow.Model.UserShortcut shortcut in listByUserId)
                    {
                        string str2 = string.Empty;
                        if (this.HasUse(shortcut.MenuId, userId, all, out source, out str2, false))
                        {
                            DataRow[] rowArray3 = menuAppDataTable.Select(string.Format("Id='{0}'", shortcut.MenuId.ToString()));
                            if (rowArray3.Length != 0)
                            {
                                DataRow dr = rowArray3[0];
                                string str = dr["IcoColor"].ToString();
                                bool flag = false;
                                foreach (DataRow row2 in menuAppDataTable.Select("ParentID='" + shortcut.MenuId.ToString() + "'"))
                                {
                                    if (this.HasUse(row2["Id"].ToString().ToGuid(), userId, all, out source, out str2, false))
                                    {
                                        flag = true;
                                        break;
                                    }
                                }
                                string[] textArray1 = new string[] {
                                "<div class=\"menulistdiv\" onclick=\"menuClick(this);\" data-id=\"", dr["Id"].ToString().ToUpper(), "\" data-title=\"", dr["Title"].ToString().Trim(), "\" data-model=\"", dr["OpenMode"].ToString(), "\" data-width=\"", dr["Width"].ToString(), "\" data-height=\"", dr["Height"].ToString(), "\" data-childs=\"", flag ? "1" : "0", "\" data-url=\"", this.GetAddress(dr, str2), "\" data-parent=\"", Guid.Empty.ToString(),
                                "\" style=\"", str.IsNullOrEmpty() ? "" : ((string) ("color:" + str.Trim1() + ";")), "\">"
                             };
                                builder.Append(string.Concat((string[])textArray1));
                                builder.Append("<div class=\"menulistdiv1\">");
                                string str4 = dr["Ico"].ToString();
                                if (str4.IsNullOrEmpty())
                                {
                                    builder.Append("<i class=\"fa fa-th-list\"></i>");
                                }
                                else if (str4.IsFontIco())
                                {
                                    builder.Append("<i class=\"fa " + str4 + "\"></i>");
                                }
                                else
                                {
                                    builder.Append("<img src=\"" + str4 + "\" style=\"vertical-align:middle\" alt=\"\"/>");
                                }
                                builder.Append("<span style=\"vertical-align:middle\">" + dr["Title"].ToString().Trim1() + "</span>");
                                builder.Append("</div>");
                                if (flag)
                                {
                                    builder.Append("<div class=\"menulistdiv2\"><i class=\"fa fa-angle-left\"></i></div>");
                                }
                                builder.Append("</div>");
                            }
                        }
                    }
                }
                DataRow[] rowArray2 = menuAppDataTable.Select(string.Format("ParentId='{0}'", rowArray[0]["Id"].ToString()));
                for (int i = 0; i < rowArray2.Length; i++)
                {
                    string str5 = string.Empty;
                    DataRow row3 = rowArray2[i];
                    if (this.HasUse(row3["Id"].ToString().ToGuid(), userId, all, out source, out str5, false))
                    {
                        bool flag2 = false;
                        foreach (DataRow row4 in menuAppDataTable.Select("ParentId='" + row3["Id"].ToString() + "'"))
                        {
                            if (this.HasUse(row4["Id"].ToString().ToGuid(), userId, all, out source, out str5, false))
                            {
                                flag2 = true;
                                break;
                            }
                        }
                        string str6 = row3["IcoColor"].ToString();
                        string[] textArray2 = new string[] {
                        "<div class=\"menulistdiv\" onclick=\"menuClick(this);\" data-id=\"", row3["Id"].ToString().ToUpper(), "\" data-title=\"", row3["Title"].ToString().Trim(), "\" data-model=\"", row3["OpenMode"].ToString(), "\" data-width=\"", row3["Width"].ToString(), "\" data-height=\"", row3["Height"].ToString(), "\" data-childs=\"", flag2 ? "1" : "0", "\" data-url=\"", this.GetAddress(row3, str5), "\" data-parent=\"", Guid.Empty.ToString(),
                        "\" style=\"", str6.IsNullOrEmpty() ? "" : ((string) ("color:" + str6.Trim1() + ";")), "\">"
                     };
                        builder.Append(string.Concat((string[])textArray2));
                        builder.Append("<div class=\"menulistdiv1\">");
                        string str7 = row3["Ico"].ToString();
                        if (str7.IsNullOrEmpty())
                        {
                            builder.Append("<i class=\"fa fa-th-list\"></i>");
                        }
                        else if (str7.IsFontIco())
                        {
                            builder.Append("<i class=\"fa " + str7 + "\"></i>");
                        }
                        else
                        {
                            builder.Append("<img src=\"" + str7 + "\" style=\"vertical-align:middle\" alt=\"\"/>");
                        }
                        builder.Append("<span style=\"vertical-align:middle\">" + row3["Title"].ToString().Trim1() + "</span>");
                        builder.Append("</div>");
                        if (flag2)
                        {
                            builder.Append("<div class=\"menulistdiv2\"><i class=\"fa fa-angle-left\"></i></div>");
                        }
                        builder.Append("</div>");
                    }
                }
            }
            return builder.ToString();
        }

        public string GetUserMenuJsonString(Guid userID, string rootDir = "", bool showSource = false)
        {
            new Menu();
            new AppLibrary();
            DataTable menuAppDataTable = this.menuData.GetMenuAppDataTable();
            if (menuAppDataTable.Rows.Count == 0)
            {
                return "[]";
            }
            DataRow[] rowArray = menuAppDataTable.Select(string.Format("ParentId='{0}'", Guid.Empty.ToString()));
            if (rowArray.Length == 0)
            {
                return "[]";
            }
            List<RoadFlow.Model.MenuUser> all = new MenuUser().GetAll();
            DataRow[] rowArray2 = menuAppDataTable.Select(string.Format("ParentId='{0}'", rowArray[0]["Id"].ToString()));
            StringBuilder builder = new StringBuilder("", 0x3e8);
            DataRow dr = rowArray[0];
            string str = string.Empty;
            builder.Append("{");
            builder.AppendFormat("\"id\":\"{0}\",", dr["Id"].ToString());
            builder.AppendFormat("\"title\":\"{0}\",", dr["Title"].ToString().Trim());
            builder.AppendFormat("\"ico\":\"{0}\",", dr["Ico"].ToString());
            builder.AppendFormat("\"color\":\"{0}\",", dr["IcoColor"].ToString());
            builder.AppendFormat("\"link\":\"{0}\",", this.GetAddress(dr, "").ToString(), str);
            builder.AppendFormat("\"model\":\"{0}\",", dr["OpenMode"].ToString());
            builder.AppendFormat("\"width\":\"{0}\",", dr["Width"].ToString());
            builder.AppendFormat("\"height\":\"{0}\",", dr["Height"].ToString());
            builder.AppendFormat("\"hasChilds\":\"{0}\",", (rowArray2.Length != 0) ? "1" : "0");
            builder.AppendFormat("\"childs\":[", Array.Empty<object>());
            StringBuilder builder2 = new StringBuilder(rowArray2.Length * 100);
            string source = string.Empty;
            if (!showSource)
            {
                List<RoadFlow.Model.UserShortcut> listByUserId = new UserShortcut().GetListByUserId(userID);
                if (listByUserId.Count > 0)
                {
                    builder2.Append("{");
                    builder2.AppendFormat("\"id\":\"{0}\",", Guid.NewGuid());
                    builder2.AppendFormat("\"title\":\"{0}\",", "快捷菜单");
                    builder2.AppendFormat("\"ico\":\"{0}\",", "");
                    builder2.AppendFormat("\"color\":\"{0}\",", "");
                    builder2.AppendFormat("\"link\":\"{0}\",", "");
                    builder2.AppendFormat("\"model\":\"{0}\",", "");
                    builder2.AppendFormat("\"width\":\"{0}\",", "");
                    builder2.AppendFormat("\"height\":\"{0}\",", "");
                    builder2.AppendFormat("\"hasChilds\":\"1\",", Array.Empty<object>());
                    builder2.AppendFormat("\"childs\":[", Array.Empty<object>());
                    StringBuilder builder3 = new StringBuilder();
                    using (List<RoadFlow.Model.UserShortcut>.Enumerator enumerator = listByUserId.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            RoadFlow.Model.UserShortcut shortcut = enumerator.Current;
                            string str3 = string.Empty;
                            List<RoadFlow.Model.MenuUser> list3 = all.FindAll(delegate (RoadFlow.Model.MenuUser p) {
                                return (p.MenuId == shortcut.MenuId) && p.Users.ContainsIgnoreCase(userID.ToString());
                            });
                            if (list3.Count > 0)
                            {
                                StringBuilder builder4 = new StringBuilder();
                                foreach (IGrouping<string, RoadFlow.Model.MenuUser> grouping in Enumerable.GroupBy<RoadFlow.Model.MenuUser, string>((IEnumerable<RoadFlow.Model.MenuUser>)list3.FindAll( key=> !key.Params.IsNullOrEmpty()),  key=>key.Params))
                                {
                                    builder4.Append(grouping.Key.Trim1());
                                    builder4.Append("&");
                                }
                                char[] chArray1 = new char[] { '&' };
                                str3 = builder4.ToString().TrimEnd(chArray1);
                            }
                            if (this.HasUse(shortcut.MenuId, userID, all, out source, out str3, showSource))
                            {
                                DataRow[] rowArray3 = menuAppDataTable.Select(string.Format("ID='{0}'", shortcut.MenuId.ToString()));
                                if (rowArray3.Length != 0)
                                {
                                    DataRow row2 = rowArray3[0];
                                    DataRow[] rowArray4 = menuAppDataTable.Select("ParentID='" + row2["ID"].ToString() + "'");
                                    builder3.Append("{");
                                    builder3.AppendFormat("\"id\":\"{0}\",", row2["ID"].ToString());
                                    builder3.AppendFormat("\"title\":\"{0}\",", row2["Title"].ToString().Trim1());
                                    builder3.AppendFormat("\"ico\":\"{0}\",", row2["Ico"].ToString());
                                    builder3.AppendFormat("\"color\":\"{0}\",", row2["IcoColor"].ToString());
                                    builder3.AppendFormat("\"link\":\"{0}\",", this.GetAddress(row2, str3));
                                    builder3.AppendFormat("\"model\":\"{0}\",", row2["OpenMode"].ToString());
                                    builder3.AppendFormat("\"width\":\"{0}\",", row2["Width"].ToString());
                                    builder3.AppendFormat("\"height\":\"{0}\",", row2["Height"].ToString());
                                    builder3.AppendFormat("\"hasChilds\":\"{0}\",", (rowArray4.Length != 0) ? "1" : "0");
                                    builder3.AppendFormat("\"childs\":[]", Array.Empty<object>());
                                    builder3.Append("},");
                                }
                            }
                        }
                    }
                    builder2.Append((builder3.Length > 0) ? builder3.ToString().TrimEnd(new char[] { ',' }) : "");
                    builder2.Append("]");
                    builder2.Append("}");
                    if (rowArray2.Length != 0)
                    {
                        builder2.Append(",");
                    }
                }
            }
            for (int i = 0; i < rowArray2.Length; i++)
            {
                string str4 = string.Empty;
                DataRow row3 = rowArray2[i];
                if (this.HasUse(row3["Id"].ToString().ToGuid(), userID, all, out source, out str4, showSource))
                {
                    bool flag = false;
                    foreach (DataRow row4 in menuAppDataTable.Select("ParentId='" + row3["Id"].ToString() + "'"))
                    {
                        if (this.HasUse(row4["ID"].ToString().ToGuid(), userID, all, out source, out str4, showSource))
                        {
                            flag = true;
                            break;
                        }
                    }
                    builder2.Append("{");
                    builder2.AppendFormat("\"id\":\"{0}\",", row3["ID"].ToString());
                    builder2.AppendFormat("\"title\":\"{0}{1}\",", row3["Title"].ToString().Trim1(), showSource ? ("<span style='margin-left:4px;color:#666;'>[" + source + "]</span>") : "");
                    builder2.AppendFormat("\"ico\":\"{0}\",", row3["Ico"].ToString());
                    builder2.AppendFormat("\"color\":\"{0}\",", row3["IcoColor"].ToString());
                    builder2.AppendFormat("\"link\":\"{0}\",", this.GetAddress(row3, str4));
                    builder2.AppendFormat("\"model\":\"{0}\",", row3["OpenMode"].ToString());
                    builder2.AppendFormat("\"width\":\"{0}\",", row3["Width"].ToString());
                    builder2.AppendFormat("\"height\":\"{0}\",", row3["Height"].ToString());
                    builder2.AppendFormat("\"hasChilds\":\"{0}\",", flag ? "1" : "0");
                    builder2.AppendFormat("\"childs\":[", Array.Empty<object>());
                    builder2.Append("]");
                    builder2.Append("}");
                    builder2.Append(",");
                }
            }
            char[] trimChars = new char[] { ',' };
            builder.Append(builder2.ToString().TrimEnd(trimChars));
            builder.Append("]");
            builder.Append("}");
            return builder.ToString();
        }

        public string GetUserMenuRefreshJsonString(Guid userID, Guid refreshID, string rootDir = "", bool showSource = false)
        {
            DataTable menuAppDataTable = this.menuData.GetMenuAppDataTable();
            DataView view1 = menuAppDataTable.DefaultView;
            view1.RowFilter = string.Format("ParentId='{0}'", refreshID);
            view1.Sort = "Sort";
            DataTable table2 = view1.ToTable();
            if (table2.Rows.Count == 0)
            {
                return "[]";
            }
            int count = table2.Rows.Count;
            StringBuilder builder = new StringBuilder("[", count * 100);
            List<RoadFlow.Model.MenuUser> all = new MenuUser().GetAll();
            string source = string.Empty;
            foreach (DataRow row in table2.Rows)
            {
                string str2 = string.Empty;
                if (this.HasUse(row["Id"].ToString().ToGuid(), userID, all, out source, out str2, showSource))
                {
                    bool flag = false;
                    foreach (DataRow row2 in menuAppDataTable.Select(string.Format("ParentId='{0}'", row["Id"].ToString())))
                    {
                        if (this.HasUse(row2["ID"].ToString().ToGuid(), userID, all, out source, out str2, showSource))
                        {
                            flag = true;
                            break;
                        }
                    }
                    builder.Append("{");
                    builder.AppendFormat("\"id\":\"{0}\",", row["ID"].ToString());
                    builder.AppendFormat("\"title\":\"{0}{1}\",", row["Title"].ToString().Trim1(), showSource ? ("<span style='margin-left:4px;color:#666;'>[" + source + "]</span>") : "");
                    builder.AppendFormat("\"ico\":\"{0}\",", row["Ico"].ToString());
                    builder.AppendFormat("\"color\":\"{0}\",", row["IcoColor"].ToString());
                    builder.AppendFormat("\"link\":\"{0}\",", this.GetAddress(row, str2));
                    builder.AppendFormat("\"model\":\"{0}\",", row["OpenMode"].ToString());
                    builder.AppendFormat("\"width\":\"{0}\",", row["Width"].ToString());
                    builder.AppendFormat("\"height\":\"{0}\",", row["Height"].ToString());
                    builder.AppendFormat("\"hasChilds\":\"{0}\",", flag ? "1" : "0");
                    builder.AppendFormat("\"childs\":[", Array.Empty<object>());
                    builder.Append("]");
                    builder.Append("}");
                    builder.Append(",");
                }
            }
            char[] trimChars = new char[] { ',' };
            return (builder.ToString().TrimEnd(trimChars) + "]");
        }

        public string GetUserMinMenuHtml(Guid userId)
        {
            DataTable menuAppDataTable = this.menuData.GetMenuAppDataTable();
            if (menuAppDataTable.Rows.Count == 0)
            {
                return "";
            }
            List<RoadFlow.Model.MenuUser> all = new MenuUser().GetAll();
            string source = string.Empty;
            StringBuilder builder = new StringBuilder();
            DataRow[] rowArray = menuAppDataTable.Select(string.Format("ParentId='{0}'", Guid.Empty.ToString()));
            if (rowArray.Length != 0)
            {
                List<RoadFlow.Model.UserShortcut> listByUserId = new UserShortcut().GetListByUserId(userId);
                if (listByUserId.Count > 0)
                {
                    foreach (RoadFlow.Model.UserShortcut shortcut in listByUserId)
                    {
                        string str2 = string.Empty;
                        if (this.HasUse(shortcut.MenuId, userId, all, out source, out str2, false))
                        {
                            DataRow[] rowArray3 = menuAppDataTable.Select(string.Format("Id='{0}'", shortcut.MenuId.ToString()));
                            if (rowArray3.Length != 0)
                            {
                                DataRow dr = rowArray3[0];
                                string str = dr["IcoColor"].ToString();
                                bool flag = false;
                                foreach (DataRow row2 in menuAppDataTable.Select("ParentID='" + shortcut.MenuId.ToString() + "'"))
                                {
                                    if (this.HasUse(row2["Id"].ToString().ToGuid(), userId, all, out source, out str2, false))
                                    {
                                        flag = true;
                                        break;
                                    }
                                }
                                string[] textArray1 = new string[] {
                                "<div class=\"menulistdiv11\" title=\"", dr["Title"].ToString().Trim1(), "\" onclick=\"menuClick1(this);\" data-id=\"", dr["ID"].ToString().ToUpper(), "\" data-title=\"", dr["Title"].ToString().Trim(), "\" data-model=\"", dr["OpenMode"].ToString(), "\" data-width=\"", dr["Width"].ToString(), "\" data-height=\"", dr["Height"].ToString(), "\" data-childs=\"", flag ? "1" : "0", "\" data-url=\"", this.GetAddress(dr, str2),
                                "\" data-parent=\"", Guid.Empty.ToString(), "\" style=\"", str.IsNullOrEmpty() ? "" : ((string) ("color:" + str.Trim1() + ";")), "\">"
                             };
                                builder.Append(string.Concat((string[])textArray1));
                                builder.Append("<div class=\"menulistdiv12\">");
                                string str4 = dr["Ico"].ToString();
                                if (str4.IsNullOrEmpty())
                                {
                                    builder.Append("<i class=\"fa fa-th-list\" style=\"margin-right:3px;vertical-align:middle\"></i>");
                                }
                                else if (str4.IsFontIco())
                                {
                                    builder.Append("<i class=\"fa " + str4 + "\" style=\"margin-right:3px;vertical-align:middle\"></i>");
                                }
                                else
                                {
                                    builder.Append("<img src=\"" + str4 + "\" style=\"vertical-align:middle\" alt=\"\"/>");
                                }
                                builder.Append("</div>");
                                builder.Append("</div>");
                            }
                        }
                    }
                }
                DataRow[] rowArray2 = menuAppDataTable.Select(string.Format("ParentId='{0}'", rowArray[0]["Id"].ToString()));
                for (int i = 0; i < rowArray2.Length; i++)
                {
                    string str5 = string.Empty;
                    DataRow row3 = rowArray2[i];
                    if (this.HasUse(row3["Id"].ToString().ToGuid(), userId, all, out source, out str5, false))
                    {
                        bool flag2 = false;
                        foreach (DataRow row4 in menuAppDataTable.Select("ParentId='" + row3["Id"].ToString() + "'"))
                        {
                            if (this.HasUse(row4["Id"].ToString().ToGuid(), userId, all, out source, out str5, false))
                            {
                                flag2 = true;
                                break;
                            }
                        }
                        string str6 = row3["IcoColor"].ToString();
                        string[] textArray2 = new string[] {
                        "<div class=\"menulistdiv11\" title=\"", row3["Title"].ToString().Trim1(), "\" onclick=\"menuClick1(this);\" data-id=\"", row3["ID"].ToString().ToUpper(), "\" data-title=\"", row3["Title"].ToString().Trim(), "\" data-model=\"", row3["OpenMode"].ToString(), "\" data-width=\"", row3["Width"].ToString(), "\" data-height=\"", row3["Height"].ToString(), "\" data-childs=\"", flag2 ? "1" : "0", "\" data-url=\"", this.GetAddress(row3, str5),
                        "\" data-parent=\"", Guid.Empty.ToString(), "\" style=\"", str6.IsNullOrEmpty() ? "" : ((string) ("color:" + str6.Trim1() + ";")), "\">"
                     };
                        builder.Append(string.Concat((string[])textArray2));
                        builder.Append("<div class=\"menulistdiv12\">");
                        string str7 = row3["Ico"].ToString();
                        if (str7.IsNullOrEmpty())
                        {
                            builder.Append("<i class=\"fa fa-th-list\" style=\"margin-right:3px;vertical-align:middle\"></i>");
                        }
                        else if (str7.IsFontIco())
                        {
                            builder.Append("<i class=\"fa " + str7 + "\" style=\"margin-right:3px;vertical-align:middle\"></i>");
                        }
                        else
                        {
                            builder.Append("<img src=\"" + str7 + "\" style=\"vertical-align:middle\" alt=\"\"/>");
                        }
                        builder.Append("</div>");
                        builder.Append("</div>");
                    }
                }
            }
            return builder.ToString();
        }

        public bool HasUse(Guid menuId, Guid userId, List<RoadFlow.Model.MenuUser> menuusers, out string source, out string params1, bool showSource = false)
        {
            source = string.Empty;
            params1 = string.Empty;
            List<RoadFlow.Model.MenuUser> list = menuusers.FindAll(delegate (RoadFlow.Model.MenuUser p) {
                return (p.MenuId == menuId) && p.Users.ContainsIgnoreCase(userId.ToString());
            });
            if (list.Count <= 0)
            {
                return false;
            }
            if (showSource)
            {
                StringBuilder builder2 = new StringBuilder();
                foreach (RoadFlow.Model.MenuUser user in list)
                {
                    builder2.Append(user.Organizes);
                    builder2.Append(",");
                    if (!user.Params.IsNullOrEmpty())
                    {
                        params1 = user.Params;
                    }
                }
                char[] chArray1 = new char[] { ',' };
                source = new Organize().GetNames(builder2.ToString().TrimEnd(chArray1));
            }
            StringBuilder builder = new StringBuilder();
            foreach (IGrouping<string, RoadFlow.Model.MenuUser> grouping in Enumerable.GroupBy<RoadFlow.Model.MenuUser, string>((IEnumerable<RoadFlow.Model.MenuUser>)list.FindAll(key=> !key.Params.IsNullOrEmpty()), key=>key.Params))
            {
                builder.Append(grouping.Key.Trim());
                builder.Append("&");
            }
            char[] trimChars = new char[] { '&' };
            params1 = builder.ToString().TrimEnd(trimChars);
            return true;
        }

        public bool HasUseButton(Guid menuId, Guid buttonId, Guid userId, List<RoadFlow.Model.MenuUser> menuusers)
        {
            if (menuId.IsEmptyGuid() || userId.IsEmptyGuid())
            {
                return false;
            }
            List<RoadFlow.Model.MenuUser> list = menuusers.FindAll(delegate (RoadFlow.Model.MenuUser p) {
                return ((p.MenuId == menuId) && p.Users.ContainsIgnoreCase(userId.ToString())) && p.Users.ContainsIgnoreCase(userId.ToString());
            });
            if (!Enumerable.Any<RoadFlow.Model.MenuUser>((IEnumerable<RoadFlow.Model.MenuUser>)list))
            {
                return false;
            }
            return list.Exists(delegate (RoadFlow.Model.MenuUser p) {
                return p.Buttons.ContainsIgnoreCase(buttonId.ToString());
            });
        }

        public int Update(RoadFlow.Model.Menu menu)
        {
            return this.menuData.Update(menu);
        }

        public int Update(RoadFlow.Model.Menu[] menus)
        {
            return this.menuData.Update(menus);
        }

       
}



 


  

}
