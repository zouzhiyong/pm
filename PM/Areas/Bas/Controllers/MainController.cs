using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
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
            StuInfoDBContext stuContext = new StuInfoDBContext();
            StringBuilder strSql = new StringBuilder();
            StringBuilder whereStr = new StringBuilder();
            for (int i = 0; i < where.Split(',').Length; i++)
            {
                whereStr.Append(" and  a.WSID<>'" + where.Split(',')[i].ToString() + "'");
            }

            strSql.Append(@"select a.AreaName,a.WSID,a.WSName,
                            min(c.logintime) as onlineDate,
                            max(c.logintimeWeb)  as noLoginWebDay,
                            max(c.logintimeApp) as noLoginAppDay,
                            datediff(day,max(c.logintimeWeb),GETDATE()) as noLoginWebDayNums,
							datediff(day,max(c.logintimeApp),GETDATE()) as noLoginAppDayNums,
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
							(select t1.*,t3.AreaName+'/'+t2.AreaName as AreaName from Bas_WS t1
							left join Bas_Area t2
							on t1.AreaCode=t2.AreaCode
							left join Bas_Area t3
							on t2.ParentCode=t3.AreaCode
							) a
                            left join sys_user b
                            on b.WSID=a.WSID
                            left join 
                            (
                                select 
                                    userid,
                                    logintime,
                                    case when workstation='Mobile APP' then logintime end logintimeApp, 
                                    case when workstation='DMS WEB' then logintime end logintimeWeb
                                from SYS_USERLOG
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
								from sys_user where isnull(IsValid,1)=1 group by wsid
							) d
							on a.wsid=d.wsid
                            left join (select WSID,count(VehID) as vehCount from Bas_Vehicle where isnull(IsValid,1)=1 and isnull(GpsID,'')<>'' group by WSID) e
							on a.WSID=e.WSID
                            where a.WSType=1 and isnull(a.IsValid,1)=1 
                            {0}
                            group by a.AreaName,a.WSID,a.WSName,e.vehCount,d.userCount,d.xsdbCount,d.xszgCount,d.xsjlCount,d.xszjCount,d.cxywyCount,d.sjCount,d.qtCount
                            order by noLoginWebDay,noLoginAppDay");

            string str = String.Format(strSql.ToString(), whereStr.ToString());

            var stuLis = stuContext.Database.SqlQuery<useinfo>(str).ToList();
            var totalWsCount = stuLis.Count<useinfo>();
            var totalUserCount = stuLis.Sum<useinfo>(t => t.userCount);
            var totalVehCount = stuLis.Sum<useinfo>(t => t.vehCount);
            //List<useinfo> newInfo = new List<useinfo>();

            //DateTime newDate = DateTime.Now;
            //foreach (var item in stuLis)
            //{
            //    TimeSpan noLoginWebDayTs = newDate - Convert.ToDateTime(item.noLoginWebDay == null ? Convert.ToDateTime("1900-01-01") : item.noLoginWebDay);
            //    TimeSpan noLoginAppDayTs = newDate - Convert.ToDateTime(item.noLoginAppDay == null ? Convert.ToDateTime("1900-01-01") : item.noLoginAppDay);
            //    item.noLoginWebDayNums = noLoginWebDayTs.Days;
            //    item.noLoginAppDayNums = noLoginAppDayTs.Days;
            //    item.noLoginWebDayStr = item.noLoginWebDayNums.ToString("yyyy-MM-dd HH:mm");
            //    item.noLoginAppDayStr = item.noLoginAppDayNums.ToString("yyyy-MM-dd HH:mm");
            //    newInfo.Add(item);
            //}

            return Json(new { rows=stuLis, totalWsCount= totalWsCount, totalUserCount = totalUserCount , totalVehCount = totalVehCount });
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
            List<useinfo> stuLis = stuContext.Database.SqlQuery<useinfo>("SELECT * FROM Bas_WS").ToList();

            return Json(stuLis);

        }

    }
}