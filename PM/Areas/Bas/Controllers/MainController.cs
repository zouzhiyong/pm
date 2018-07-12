using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using PM.Common;
using PM.Models;
using SharpSvn;
using SharpSvn.Security;

namespace PM.Areas.Bas.Controllers
{
    public class MainController : Controller
    {
        // GET: Main
        public ActionResult Index()
        {
            return View();
        }

        /// 本地svn服务器地址
        /// </summary>
        private string localSVN = ConfigurationManager.AppSettings["localSVN"].ToString();
        /// <summary>
        /// 线上svn服务器地址
        /// </summary>
        private string onlineSVN = ConfigurationManager.AppSettings["onlineSVN"].ToString();
        /// <summary>
        /// 本地地址
        /// </summary>
        private string localPath = ConfigurationManager.AppSettings["localPath"].ToString();

        public JsonResult FindHelpList(string fileName, string sheetName, int headerRowIndex)
        {
            //UpdateSvn("", "zouzhiyong", "zouzhiyong", "online");

            ArrayList alist = new ArrayList();

            System.Data.DataTable table = NPOIHelper.ImportExcel2007ToDt(localPath + "\\" + fileName, sheetName, headerRowIndex);
            List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
            Dictionary<string, object> childRow;
            foreach (System.Data.DataRow row in table.Rows)
            {
                childRow = new Dictionary<string, object>();
                foreach (System.Data.DataColumn col in table.Columns)
                {
                    childRow.Add(col.ColumnName, row[col]);
                    if (col.ColumnName == "经销商编码")
                    {
                        if (row[col].ToString() != "")
                        {
                            alist.Add("'" + row[col].ToString() + "'");
                        }
                    }
                }
                parentRow.Add(childRow);
            }


            return Json(parentRow);

        }
        [HttpPost]
        public JsonResult FindSqlList(string where)
        {
            var cache = CacheHelper.GetCache("mydata");
            object obj = null;

            if (cache == null)
            {
                StuInfoDBContext stuContext = new StuInfoDBContext();
                StringBuilder strSql = new StringBuilder();
                StringBuilder whereStr = new StringBuilder();
                for (int i = 0; i < where.Split(',').Length; i++)
                {
                    whereStr.Append(" and  a.WSID<>'" + where.Split(',')[i].ToString() + "'");
                }

                strSql.Append(@"select a.AreaName,a.WSID,a.WSName,
                            min(c.onlineDate) as onlineDate,
							max(c.noLoginWebDay) as noLoginWebDay,
							max(c.noLoginAppDay) as noLoginAppDay,
                            max(f.RecTimestamp) as RecTimestamp,
                            max(g.VisitDate) as VisitDate,
                            max(h.RecTimeStamp) as OrderDate,
                            datediff(day,max(c.noLoginWebDay),GETDATE()) as noLoginWebDayNums,
							datediff(day,max(c.noLoginAppDay),GETDATE()) as noLoginAppDayNums,
                            datediff(day,max(f.RecTimestamp),GETDATE()) as noInputFyDayNums,
                            datediff(day,max(g.VisitDate),GETDATE()) as noBfDayNums,
                            datediff(day,max(h.RecTimeStamp),GETDATE()) as noOrderDayNums,
                            max(c.oneWebDay) as oneWebDay,
							max(c.oneWebWeek) as oneWebWeek,
							max(c.oneWebMonth) as oneWebMonth,
							max(c.oneAppDay) as oneAppDay,
							max(c.oneAppWeek) as oneAppWeek,
							max(c.oneAppMonth) as oneAppMonth,
                            d.userCount,
                            isnull(e.vehCount,0) as vehCount,
                            d.xsdbCount,
                            d.xszgCount,
                            d.xsjlCount,
                            d.xszjCount,
                            d.cxywyCount,
                            d.sjCount,
                            d.qtCount                            
                            from 
							(select t1.WSID,t1.WSName,t1.WSType,t1.IsValid,t3.AreaName+'/'+t2.AreaName as AreaName from Bas_WS t1
							left join Bas_Area t2
							on t1.AreaCode=t2.AreaCode
							left join Bas_Area t3
							on t2.ParentCode=t3.AreaCode
							) a
                            left join (select WSID,UserID from sys_user where left(UserName,5)<>'admin') b
                            on b.WSID=a.WSID
                            left join 
                            (
                                select 
                                    userid,
									min(logintime) as onlineDate,
                                    max(case when workstation='Mobile APP' then logintime end) as noLoginAppDay, 
                                    max(case when workstation='DMS WEB' then logintime end) as noLoginWebDay,
                                    count(DISTINCT case when workstation='DMS WEB' and logintime >=  DATEADD(dd,-1, DATEADD(DD, DATEDIFF(DD, 0, GETDATE()), 0)) then Convert(VARCHAR(30),logintime,112) end ) AS oneWebDay,
									count(DISTINCT case when workstation='DMS WEB' and logintime >=  DATEADD(dd,-7, DATEADD(DD, DATEDIFF(DD, 0, GETDATE()), 0)) then Convert(VARCHAR(30),logintime,112) end ) AS oneWebWeek,
									count(DISTINCT case when workstation='DMS WEB' and logintime >=  DATEADD(dd,-30, DATEADD(DD, DATEDIFF(DD, 0, GETDATE()), 0)) then Convert(VARCHAR(30),logintime,112) end ) AS oneWebMonth,
									count(DISTINCT case when workstation='Mobile APP' and logintime >=  DATEADD(dd,-1, DATEADD(DD, DATEDIFF(DD, 0, GETDATE()), 0)) then Convert(VARCHAR(30),logintime,112) end ) AS oneAppDay,
									count(DISTINCT case when workstation='Mobile APP' and logintime >=  DATEADD(dd,-7, DATEADD(DD, DATEDIFF(DD, 0, GETDATE()), 0)) then Convert(VARCHAR(30),logintime,112) end ) AS oneAppWeek,
									count(DISTINCT case when workstation='Mobile APP' and logintime >=  DATEADD(dd,-30, DATEADD(DD, DATEDIFF(DD, 0, GETDATE()), 0)) then Convert(VARCHAR(30),logintime,112) end ) AS oneAppMonth						
                                from SYS_USERLOG group by userid
                            ) c
                            on b.UserID=c.userid
                            left join
							(
								select wsid, 
                                count(UserID) as userCount,
								count(case when UserType='0001' then UserID end) as xsdbCount,
								count(case when UserType='0002' then UserID end) as xszgCount,
								count(case when UserType='0003' then UserID end) as xsjlCount,
								count(case when UserType='0004' then UserID end) as xszjCount,
                                count(case when UserType='0006' then UserID end) as sjCount,
								count(case when UserType='0007' then UserID end) as cxywyCount,
								count(case when isnull(UserType,'')<>'0001' and isnull(UserType,'')<>'0002' and isnull(UserType,'')<>'0003' and isnull(UserType,'')<>'0004' and isnull(UserType,'')<>'0006' and isnull(UserType,'')<>'0007' then UserID end) as qtCount  
								from sys_user where isnull(IsValid,1)=1  and left(UserName,5)<>'admin' group by wsid
							) d
							on a.wsid=d.wsid
                            left join (select WSID,count(VehID) as vehCount from Bas_Vehicle where isnull(IsValid,1)=1 and isnull(GpsID,'')<>'' group by WSID) e
							on a.WSID=e.WSID
                            left join (select WSID,max(RecTimestamp) as RecTimestamp from view_TMS_Car_Cost group by WSID) f
                            on a.WSID=f.WSID
                            left join (select WSID,max(VisitDate) as VisitDate from SFA_Visit group by WSID) g
                            on a.WSID=g.WSID
                            left join (select WSID,max(RecTimeStamp) as RecTimeStamp from SFA_Order_Header group by WSID) h
							on a.WSID=h.WSID
                            where a.WSType=1 and isnull(a.IsValid,1)=1 
                            {0}
                            group by a.AreaName,a.WSID,a.WSName,e.vehCount,d.userCount,d.xsdbCount,d.xszgCount,d.xsjlCount,d.xszjCount,d.cxywyCount,d.sjCount,d.qtCount
                            order by noLoginWebDay,noLoginAppDay");

                string str = String.Format(strSql.ToString(), whereStr.ToString());

                var userLis = stuContext.Database.SqlQuery<useInfo>(str).ToList();
                var totalWsCount = userLis.Count<useInfo>();
                var totalUserCount = userLis.Sum<useInfo>(t => t.userCount);
                var totalVehCount = userLis.Sum<useInfo>(t => t.vehCount);

                strSql.Clear();
                strSql.Append(@"select c.ApplicationType,count(distinct a.UserID) as moduleCount
                            from sys_User a
                            left join Sys_user_role b
                            on a.UserID=b.UserID
                            inner join (
                            select r1.RoleID,r1.ModuleID,r3.ApplicationType from Sys_RoleRight r1
                            inner join Sys_Role r2
                            on r1.RoleID=r2.RoleID
                            inner join (select ModuleID,ApplicationType,ModuleName from Sys_Module) r3
                            on r1.ModuleID=r3.ModuleID
                            where isnull(r2.IsValid,1)=1 and (len(r1.ModuleID)=2 or left(r1.ModuleID,2)='53')and r3.ApplicationType is not null and r3.ApplicationType<>'9' and r3.ApplicationType<>'0'
                            ) c
                            on c.RoleID=a.RoleID
                            inner join (select WSID,WSName,WSType,IsValid from Bas_WS) d
                            on a.WSID=d.WSID
                            where d.WSType=1 and isnull(d.IsValid,1)=1 and left(a.UserName,5)<>'admin'
                            {0}
                            group by c.ApplicationType order by moduleCount desc");
                str = String.Format(strSql.ToString(), whereStr.ToString());

                var moduleLis = stuContext.Database.SqlQuery<moduleInfo>(str).ToList();

                obj = new { rows = userLis, totalWsCount = totalWsCount, totalUserCount = totalUserCount, totalVehCount = totalVehCount, moduleLis = moduleLis };

                //插入cache 缓存12小时
                double tt = 12 * 60 * 60;
                CacheHelper.SetCache("mydata", obj, DateTime.Now.AddSeconds(tt), TimeSpan.Zero);
            }
            else
            {
                obj = (object)cache;

            }

            return Json(obj);
        }

        [HttpPost]
        public JsonResult ClearCacheData()
        {
            try
            {
                CacheHelper.RemoveAllCache("mydata");
                return Json("清除成功");
            }
            catch (Exception e)
            {
                return Json(e.Message.ToString());
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="fileName">文件夹名称</param>
        /// <returns></returns>
        public JsonResult UpdateSvn(string fileName, string user, string pwd, string type)
        {
            string result = string.Empty;
            using (SvnClient client = new SvnClient())
            {
                try
                {
                    GetPermission(client, user, pwd);
                    SvnInfoEventArgs serverInfo;
                    SvnInfoEventArgs clientInfo;
                    SvnUriTarget repos = new SvnUriTarget("https://" + (type == "local" ? localSVN : onlineSVN) + fileName);
                    SvnPathTarget local = new SvnPathTarget(localPath + "\\" + fileName);

                    client.GetInfo(repos, out serverInfo);

                    client.Update(localPath + "\\" + fileName);

                    client.GetInfo(local, out clientInfo);
                    if (serverInfo.Revision > 0 && clientInfo.Revision > 0)
                    {
                        result = serverInfo.Revision.ToString() + "-" + clientInfo.Revision.ToString();
                    }
                    return Json(new { result = true });
                }
                catch (Exception ex)
                {
                    return Json(new { result = false, msg = ex.Message.ToString() });
                }
            }
        }

        /// <summary>
        /// 获取权限
        /// </summary>
        /// <param name="client"></param>
        private void GetPermission(SvnClient client, string username, string password)
        {
            client.LoadConfiguration(Path.Combine(Path.GetTempPath(), "Svn"), true);
            client.Authentication.UserNamePasswordHandlers +=
                new EventHandler<SvnUserNamePasswordEventArgs>(
                    delegate (object s, SvnUserNamePasswordEventArgs e)
                    {
                        e.UserName = username;
                        e.Password = password;
                    });
            client.Authentication.SslServerTrustHandlers +=
                new EventHandler<SvnSslServerTrustEventArgs>(
                    delegate (object s, SvnSslServerTrustEventArgs e)
                    {
                        e.AcceptedFailures = e.Failures;
                        e.Save = true;
                    });
        }

        public JsonResult FindUseInfo()
        {
            StuInfoDBContext stuContext = new StuInfoDBContext();
            List<useInfo> stuLis = stuContext.Database.SqlQuery<useInfo>("SELECT * FROM Bas_WS").ToList();

            return Json(stuLis);

        }
    }
}
