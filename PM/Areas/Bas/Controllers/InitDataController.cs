using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PM.Common;
using SharpSvn;
using SharpSvn.Security;

namespace PM.Areas.Bas.Controllers
{
    public class InitDataController : Controller
    {
        // GET: Bas/InitData
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

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="fileName">文件夹名称</param>
        /// <returns></returns>
        public JsonResult UpdateSvn(string fileName, string user, string pwd, string type)
        { 
            string result = string.Empty;
            localPath = Server.MapPath("~/" + localPath);
            
            using (SvnClient client = new SvnClient())
            {
                try
                {
                    GetPermission(client, user, pwd);
                    SvnInfoEventArgs serverInfo;
                    SvnInfoEventArgs clientInfo;
                    SvnUriTarget repos = new SvnUriTarget("https://" + (type == "local" ? localSVN : onlineSVN));
                    SvnPathTarget local = new SvnPathTarget(localPath + "\\" + fileName);

                    client.GetInfo(repos, out serverInfo);

                    client.Update(localPath);

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

        public JsonResult FindInitData()
        {
            string fileName = "初始化主数据清单.xlsx";
            string sheetName = "table";
            int headerRowIndex = 0;

            //UpdateSvn(fileName, "zouzhiyong", "zouzhiyong", "online");            

            ArrayList alist = new ArrayList();

            System.Data.DataTable table = NPOIHelper.ImportExcel2007ToDt(Server.MapPath("~/"+localPath) + "\\" + fileName, sheetName, headerRowIndex);
            List<Dictionary<string, object>> CommonRows = new List<Dictionary<string, object>>();
            List<Dictionary<string, object>> ELMSRows = new List<Dictionary<string, object>>();
            List<Dictionary<string, object>> DMSRows = new List<Dictionary<string, object>>();
            List<Dictionary<string, object>> SFARows = new List<Dictionary<string, object>>();
            List<Dictionary<string, object>> VEHICLERows = new List<Dictionary<string, object>>();
            List<Dictionary<string, object>> FINRows = new List<Dictionary<string, object>>();
            List<Dictionary<string, object>> B2BRows = new List<Dictionary<string, object>>();
            Dictionary<string, object> childRow;

            foreach (System.Data.DataRow row in table.Rows)
            {
                childRow = new Dictionary<string, object>();
                foreach (System.Data.DataColumn col in table.Columns)
                {
                    childRow.Add(col.ColumnName, row[col]);                    
                }

                if (childRow["系统"].ToString().ToLower() == "公共项目")
                {
                    CommonRows.Add(childRow);
                }
                if (childRow["系统"].ToString().ToLower() == "elms")
                {
                    ELMSRows.Add(childRow);
                }
                if (childRow["系统"].ToString().ToLower() == "dms")
                {
                    DMSRows.Add(childRow);
                }
                if (childRow["系统"].ToString().ToLower() == "sfa")
                {
                    SFARows.Add(childRow);
                }
                if (childRow["系统"].ToString().ToLower() == "vehicler")
                {
                    VEHICLERows.Add(childRow);
                }
                if (childRow["系统"].ToString().ToLower() == "fin")
                {
                    FINRows.Add(childRow);
                }
                if (childRow["系统"].ToString().ToLower() == "b2b")
                {
                    B2BRows.Add(childRow);
                }
            }

            return Json(new { CommonRows = CommonRows, ELMSRows = ELMSRows, DMSRows = DMSRows, SFARows = SFARows, VEHICLERows = VEHICLERows, FINRows = FINRows , B2BRows = B2BRows });
        }
    }
}