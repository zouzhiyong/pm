using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using PM.Models;

namespace PM.Areas.Bas.Controllers
{
    public class ImplementationController : Controller
    {
        // GET: Bas/Implementation
        public ActionResult Index()
        {
            findWsInfo();
            return View();
        }

        public object findWsInfo()
        {
            StuInfoDBContext stuContext = new StuInfoDBContext();
            TestDBContext testContext = new TestDBContext();
            var wsList = testContext.T_WS.Where(t => t.IsValid != 0).OrderByDescending(o => new {o.City,o.WSCompany,o.goLiveDate}).ToList();
            var implementItemList = testContext.T_ImplementPersonnel.Where(t => t.IsValid != 0).ToList();
            var statusItemList = testContext.T_Status.Where(t => 1 == 1).ToList();
            foreach (var item in wsList)
            {
                var implementItem1 = implementItemList.Where(t => t.T_Id == item.Personnel1).FirstOrDefault();
                var implementItem2 = implementItemList.Where(t => t.T_Id == item.Personnel2).FirstOrDefault();
                var statusItem = statusItemList.Where(t => t.Id == item.status).FirstOrDefault();

                item.Personnel1Name = implementItem1 == null ? "" : implementItem1.T_Name;
                item.Personnel1Email = implementItem1 == null ? "" : implementItem1.T_Email;
                item.Personnel1Mobile = implementItem1 == null ? "" : implementItem1.T_Mobile;
                item.Personnel2Name = implementItem2 == null ? "" : implementItem2.T_Name;
                item.Personnel2Email = implementItem2 == null ? "" : implementItem2.T_Email;
                item.Personnel2Mobile = implementItem2 == null ? "" : implementItem2.T_Mobile;
                item.statusName = statusItem == null ? "" : statusItem.name;
                item.sszStatus = item.researchBeginDate.Value.CompareTo(Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"))) <= 0 && item.goLiveDate.Value.CompareTo(Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"))) > 0 ? 1 : 0;
                item.ysxStatus = item.goLiveDate.Value.CompareTo(Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"))) <= 0 ? 1 : 0;
                item.wsxStatus = item.goLiveDate.Value.CompareTo(Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"))) > 0 ? 1 : 0;
                item.planStatus = 1;
                ////获取工作日
                //if(DateTime.Now.ToString("yyyy-MM")== item.goLiveDate.Value.ToString("yyyy-MM"))
                //{
                //    string str1 = DateTime.Now.ToString("yyyy-MM-dd");
                //    string str2 = item.goLiveDate.Value.ToString("yyyy-MM-dd");
                //    DateTime d1 = Convert.ToDateTime(str1);
                //    DateTime d2 = Convert.ToDateTime(str2);
                //    DateTime d3 = Convert.ToDateTime(string.Format("{0}-{1}-{2}", d1.Year, d1.Month, d1.Day));
                //    DateTime d4 = Convert.ToDateTime(string.Format("{0}-{1}-{2}", d2.Year, d2.Month, d2.Day));
                //    int days = (d3 - d4).Days;
                //    item.gzrCount = days;
                //}else
                //{
                //    item.gzrCount = 30;
                //}

            }

            var sszWsNumber = wsList.Where(t => t.researchBeginDate.Value.CompareTo(Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"))) <= 0 && t.goLiveDate.Value.CompareTo(Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"))) > 0).Count();
            var ysxWsNumber = wsList.Where(t => t.goLiveDate.Value.CompareTo(Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"))) <= 0).Count();
            var wsxWsNumber = wsList.Where(t => t.goLiveDate.Value.CompareTo(Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"))) > 0).Count();
            var planWsNumber = wsList.Count();

            //获取正式库销量数据
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"
                            select 
                            a.WSID,
                            b.WSName,
                            (select count(0) from Bas_Customer where isnull(isvalid,1)=1 and wsid=a.wsid) as custCount,
                            count(distinct case when convert(varchar(6),a.OrderDate,112)='{1}01'then CONVERT(char(10),a.OrderDate,120) end) as xsCount1,
                            count(distinct case when convert(varchar(6),a.OrderDate,112)='{1}02'then CONVERT(char(10),a.OrderDate,120) end) as xsCount2,
                            count(distinct case when convert(varchar(6),a.OrderDate,112)='{1}03'then CONVERT(char(10),a.OrderDate,120) end) as xsCount3,
                            count(distinct case when convert(varchar(6),a.OrderDate,112)='{1}04'then CONVERT(char(10),a.OrderDate,120) end) as xsCount4,
                            count(distinct case when convert(varchar(6),a.OrderDate,112)='{1}05'then CONVERT(char(10),a.OrderDate,120) end) as xsCount5,
                            count(distinct case when convert(varchar(6),a.OrderDate,112)='{1}06'then CONVERT(char(10),a.OrderDate,120) end) as xsCount6,
                            count(distinct case when convert(varchar(6),a.OrderDate,112)='{1}07'then CONVERT(char(10),a.OrderDate,120) end) as xsCount7,
                            count(distinct case when convert(varchar(6),a.OrderDate,112)='{1}08'then CONVERT(char(10),a.OrderDate,120) end) as xsCount8,
                            count(distinct case when convert(varchar(6),a.OrderDate,112)='{1}09'then CONVERT(char(10),a.OrderDate,120) end) as xsCount9,
                            count(distinct case when convert(varchar(6),a.OrderDate,112)='{1}10'then CONVERT(char(10),a.OrderDate,120) end) as xsCount10,
                            count(distinct case when convert(varchar(6),a.OrderDate,112)='{1}11'then CONVERT(char(10),a.OrderDate,120) end) as xsCount11,
                            count(distinct case when convert(varchar(6),a.OrderDate,112)='{1}12'then CONVERT(char(10),a.OrderDate,120) end) as xsCount12
                            from SFA_Order_Header a 
                            left join Bas_WS b 
                            on a.WSID=b.WSID and isnull(b.isvalid,1)=1 
                            where a.OrderType='51' and  (a.status=3 or a.status=4) and a.wsid in ({0}) 
                            group by a.WSID,b.WSName
                           ");
            List<string> list = new List<string>();
            foreach (var item in wsList)
            {
                list.Add(item.WSID);
            }
            var whereStr = string.Join(",", list.ToArray());
            string str = String.Format(strSql.ToString(), whereStr.ToString(), DateTime.Now.ToString("yyyy"));
            var wsinfoTemp = stuContext.Database.SqlQuery<T_WS>(str).ToList();
            foreach (var item in wsList)
            {
                var wsItem = wsinfoTemp.Where(w => w.WSID == item.WSID).FirstOrDefault();
                if (wsItem != null)
                {
                    item.WSName = wsItem.WSName;
                    item.custCount = wsItem.custCount;
                    item.xsCount1Name = wsItem.xsCount1 == 0 || item.goLiveDate.Value.Month > 1 ? "-" : wsItem.xsCount1.ToString() + " / " + getDays(item.goLiveDate, 1);
                    item.xsCount2Name = wsItem.xsCount2 == 0 || item.goLiveDate.Value.Month > 2 ? "-" : wsItem.xsCount2.ToString() + " / " + getDays(item.goLiveDate, 2);
                    item.xsCount3Name = wsItem.xsCount3 == 0 || item.goLiveDate.Value.Month > 3 ? "-" : wsItem.xsCount3.ToString() + " / " + getDays(item.goLiveDate, 3);
                    item.xsCount4Name = wsItem.xsCount4 == 0 || item.goLiveDate.Value.Month > 4 ? "-" : wsItem.xsCount4.ToString() + " / " + getDays(item.goLiveDate, 4);
                    item.xsCount5Name = wsItem.xsCount5 == 0 || item.goLiveDate.Value.Month > 5 ? "-" : wsItem.xsCount5.ToString() + " / " + getDays(item.goLiveDate, 5);
                    item.xsCount6Name = wsItem.xsCount6 == 0 || item.goLiveDate.Value.Month > 6 ? "-" : wsItem.xsCount6.ToString() + " / " + getDays(item.goLiveDate, 6);
                    item.xsCount7Name = wsItem.xsCount7 == 0 || item.goLiveDate.Value.Month > 7 ? "-" : wsItem.xsCount7.ToString() + " / " + getDays(item.goLiveDate, 7);
                    item.xsCount8Name = wsItem.xsCount8 == 0 || item.goLiveDate.Value.Month > 8 ? "-" : wsItem.xsCount8.ToString() + " / " + getDays(item.goLiveDate, 8);
                    item.xsCount9Name = wsItem.xsCount9 == 0 || item.goLiveDate.Value.Month > 9 ? "-" : wsItem.xsCount9.ToString() + " / " + getDays(item.goLiveDate, 9);
                    item.xsCount10Name = wsItem.xsCount10 == 0 || item.goLiveDate.Value.Month > 10 ? "-" : wsItem.xsCount10.ToString() + " / " + getDays(item.goLiveDate, 10);
                    item.xsCount11Name = wsItem.xsCount11 == 0 || item.goLiveDate.Value.Month > 11 ? "-" : wsItem.xsCount11.ToString() + " / " + getDays(item.goLiveDate, 11);
                    item.xsCount12Name = wsItem.xsCount12 == 0 || item.goLiveDate.Value.Month > 12 ? "-" : wsItem.xsCount12.ToString() + " / " + getDays(item.goLiveDate, 12);
                }
            }

            var obj = new { rows = wsList, sszWsNumber = sszWsNumber, ysxWsNumber = ysxWsNumber, wsxWsNumber = wsxWsNumber, planWsNumber = planWsNumber };
            return Json(obj);
        }

        private int getDays(DateTime? goLiveDate, int month)
        {
            if (goLiveDate.Value.Year == DateTime.Now.Year && goLiveDate.Value.Month == month)
            {
                //当前月上线
                if (DateTime.Now.Month == month)
                {
                    string str1 = DateTime.Now.ToString("yyyy-MM-dd");
                    string str2 = goLiveDate.Value.ToString("yyyy-MM-dd");
                    DateTime d1 = Convert.ToDateTime(str1);
                    DateTime d2 = Convert.ToDateTime(str2);
                    DateTime d3 = Convert.ToDateTime(string.Format("{0}-{1}-{2}", d1.Year, d1.Month, d1.Day));
                    DateTime d4 = Convert.ToDateTime(string.Format("{0}-{1}-{2}", d2.Year, d2.Month, d2.Day));
                    int days = (d3 - d4).Days+1;
                    return days;
                }
                else
                {
                    DateTime dateTime = new DateTime(DateTime.Now.Year, month, 1);
                    string str1 = dateTime.AddDays(1 - dateTime.Day).AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");
                    string str2 = goLiveDate.Value.ToString("yyyy-MM-dd");
                    DateTime d1 = Convert.ToDateTime(str1);
                    DateTime d2 = Convert.ToDateTime(str2);
                    DateTime d3 = Convert.ToDateTime(string.Format("{0}-{1}-{2}", d1.Year, d1.Month, d1.Day));
                    DateTime d4 = Convert.ToDateTime(string.Format("{0}-{1}-{2}", d2.Year, d2.Month, d2.Day));
                    int days = (d3 - d4).Days+1;
                    return days;
                }
            }
            else
            {
                if (DateTime.Now.Month == month)
                {
                    string str1 = DateTime.Now.ToString("yyyy-MM-dd");
                    string str2 = DateTime.Now.ToString("yyyy-MM") + "-1";
                    DateTime d1 = Convert.ToDateTime(str1);
                    DateTime d2 = Convert.ToDateTime(str2);
                    DateTime d3 = Convert.ToDateTime(string.Format("{0}-{1}-{2}", d1.Year, d1.Month, d1.Day));
                    DateTime d4 = Convert.ToDateTime(string.Format("{0}-{1}-{2}", d2.Year, d2.Month, d2.Day));
                    int days = (d3 - d4).Days + 1;
                    return days;
                }
                else
                {
                    DateTime dateTime = new DateTime(DateTime.Now.Year, month, 1);
                    int days = dateTime.AddDays(1 - dateTime.Day).AddMonths(1).AddDays(-1).Day;
                    return days;
                }
            }

        }
    }
}