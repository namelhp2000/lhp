using System;
using System.DirectoryServices;

namespace RoadFlow.Utility.WindowInfo
{

    /// <summary>
    /// 在线安装数据库使用检测IIS版本
    /// </summary>
    public class IISWorker
    {
        private static string HostName = "localhost";

        /// <summary>
        /// 获取本地IIS版本
        /// </summary>
        /// <returns></returns>
        public static string GetIIsVersion()
        {
            try
            {
                DirectoryEntry entry = new DirectoryEntry("IIS://" + HostName + "/W3SVC/INFO");
                string version = entry.Properties["MajorIISVersionNumber"].Value.ToString();
                return version;
            }
            catch (Exception se)
            {

                //说明一点:IIS5.0中没有(int)entry.Properties["MajorIISVersionNumber"].Value;属性，将抛出异常 证明版本为 5.0
                return se.ToString();//   string.Empty;
            }
        }







        /// <summary>
        /// 创建虚拟目录网站
        /// </summary>
        /// <param name="webSiteName">网站名称</param>
        /// <param name="physicalPath">物理路径</param>
        /// <param name="domainPort">站点+端口，如192.168.1.23:90</param>
        /// <param name="isCreateAppPool">是否创建新的应用程序池</param>
        /// <returns></returns>
        public static int CreateWebSite(string webSiteName, string physicalPath, string domainPort, bool isCreateAppPool)
        {
            DirectoryEntry root = new DirectoryEntry("IIS://" + HostName + "/W3SVC");
            // 为新WEB站点查找一个未使用的ID
            int siteID = 1;
            foreach (DirectoryEntry e in root.Children)
            {
                if (e.SchemaClassName == "IIsWebServer")
                {
                    int ID = Convert.ToInt32(e.Name);
                    if (ID >= siteID) { siteID = ID + 1; }
                }
            }
            // 创建WEB站点
            DirectoryEntry site = (DirectoryEntry)root.Invoke("Create", "IIsWebServer", siteID);
            site.Invoke("Put", "ServerComment", webSiteName);
            site.Invoke("Put", "KeyType", "IIsWebServer");
            site.Invoke("Put", "ServerBindings", domainPort + ":");
            site.Invoke("Put", "ServerState", 2);
            site.Invoke("Put", "FrontPageWeb", 1);
            site.Invoke("Put", "DefaultDoc", "Default.html");
            // site.Invoke("Put", "SecureBindings", ":443:");
            site.Invoke("Put", "ServerAutoStart", 1);
            site.Invoke("Put", "ServerSize", 1);
            site.Invoke("SetInfo");
            // 创建应用程序虚拟目录

            DirectoryEntry siteVDir = site.Children.Add("Root", "IISWebVirtualDir");
            siteVDir.Properties["AppIsolated"][0] = 2;
            siteVDir.Properties["Path"][0] = physicalPath;
            siteVDir.Properties["AccessFlags"][0] = 513;
            siteVDir.Properties["FrontPageWeb"][0] = 1;
            siteVDir.Properties["AppRoot"][0] = "LM/W3SVC/" + siteID + "/Root";
            siteVDir.Properties["AppFriendlyName"][0] = "Root";

            if (isCreateAppPool)
            {
                DirectoryEntry apppools = new DirectoryEntry("IIS://" + HostName + "/W3SVC/AppPools");

                DirectoryEntry newpool = apppools.Children.Add(webSiteName, "IIsApplicationPool");
                newpool.Properties["AppPoolIdentityType"][0] = "4"; //4
                newpool.Properties["ManagedPipelineMode"][0] = "0"; //0:集成模式 1:经典模式
                newpool.CommitChanges();
                siteVDir.Properties["AppPoolId"][0] = webSiteName;
            }

            siteVDir.CommitChanges();
            site.CommitChanges();
            return siteID;
        }

        /// <summary>
        /// 得到网站的物理路径
        /// </summary>
        /// <param name="rootEntry">网站节点</param>
        /// <returns></returns>
        public static string GetWebsitePhysicalPath(DirectoryEntry rootEntry)
        {
            string physicalPath = "";
            foreach (DirectoryEntry childEntry in rootEntry.Children)
            {
                if ((childEntry.SchemaClassName == "IIsWebVirtualDir") && (childEntry.Name.ToLower() == "root"))
                {
                    if (childEntry.Properties["Path"].Value != null)
                    {
                        physicalPath = childEntry.Properties["Path"].Value.ToString();
                    }
                    else
                    {
                        physicalPath = "";
                    }
                }
            }
            return physicalPath;
        }



        /// <summary>
        /// 创建App池
        /// </summary>
        /// <param name="appPoolName"></param>
        /// <param name="Username"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public static bool CreateAppPool(string appPoolName, string Username, string Password)
        {
            bool issucess = false;
            try
            {
                //创建一个新程序池
                DirectoryEntry newpool;
                DirectoryEntry apppools = new DirectoryEntry("IIS://" + HostName + "/W3SVC/AppPools");
                newpool = apppools.Children.Add(appPoolName, "IIsApplicationPool");

                //设置属性 访问用户名和密码 一般采取默认方式
                newpool.Properties["WAMUserName"][0] = Username;
                newpool.Properties["WAMUserPass"][0] = Password;
                newpool.Properties["AppPoolIdentityType"][0] = "3";
                newpool.CommitChanges();
                issucess = true;
                return issucess;
            }
            catch // (Exception ex) 
            {
                return false;
            }
        }


        /// <summary>
        /// 建立程序池后关联相应应用程序及虚拟目录
        /// </summary>
        public static void SetAppToPool(string appname, string poolName)
        {
            //获取目录
            DirectoryEntry getdir = new DirectoryEntry("IIS://localhost/W3SVC");
            foreach (DirectoryEntry getentity in getdir.Children)
            {
                if (getentity.SchemaClassName.Equals("IIsWebServer"))
                {
                    //设置应用程序程序池 先获得应用程序 在设定应用程序程序池
                    //第一次测试根目录
                    foreach (DirectoryEntry getchild in getentity.Children)
                    {
                        if (getchild.SchemaClassName.Equals("IIsWebVirtualDir"))
                        {
                            //找到指定的虚拟目录.
                            foreach (DirectoryEntry getsite in getchild.Children)
                            {
                                if (getsite.Name.Equals(appname))
                                {
                                    //【测试成功通过】
                                    getsite.Properties["AppPoolId"].Value = poolName;
                                    getsite.CommitChanges();
                                }
                            }
                        }
                    }
                }
            }
        }


    }
}
